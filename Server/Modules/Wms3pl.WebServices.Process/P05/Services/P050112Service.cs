using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050112Service
	{
		private WmsTransaction _wmsTransaction;
		public P050112Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		private F0003 GetF0003(string dcCode,string gupCode,string custCode,string apName)
		{
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);
			return f0003Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.AP_NAME == apName).FirstOrDefault();
		}

		/// <summary>
		/// Caps 設定檢核
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="retailCnt">本次彙總門店數</param>
		/// <param name="capsPlainQty">回傳Caps座數</param>
		/// <param name="capsPerPlainQty">回傳Caps每座最大儲位數</param>
		/// <param name="maxCaseRetailCnt">回傳最大門店數</param>
		/// <param name="batchNo">彙總批號</param>
		/// <returns></returns>
		private ExecuteResult CapsSettingCheck(string dcCode,string gupCode,string custCode,int retailCnt,string pickTool,string putTool,ref int capsPlainQty,ref int capsPerPlainQty,ref int maxCaseRetailCnt,string batchNo = null)
		{
			var maxCaseRetail = GetF0003(dcCode, gupCode, custCode, "Caps_Max_RetailQty");
			if (maxCaseRetail == null)
				return new ExecuteResult(false, Properties.Resources.P050112Service_UnSetCapsMaxRetailQty);
			else
				maxCaseRetailCnt = int.Parse(maxCaseRetail.SYS_PATH);

			var capsPlain = GetF0003(dcCode, gupCode, custCode, "Caps_PlainQty");
			if (capsPlain == null)
				return new ExecuteResult(false, Properties.Resources.P050112Service_UnsetCapsPlainQty);
			else
				capsPlainQty = int.Parse(capsPlain.SYS_PATH);

			var capsPerPlain = GetF0003(dcCode, gupCode, custCode, "Caps_PerPlainQty");
			if (capsPerPlain == null)
				return new ExecuteResult(false, Properties.Resources.P050112Service_UnsetCapsPerPlainQty);
			else
				capsPerPlainQty = int.Parse(capsPerPlain.SYS_PATH);

			//檢核揀貨單門店數是否超過最大門店數
			if (pickTool == "4" && string.IsNullOrEmpty(putTool) && retailCnt > maxCaseRetailCnt)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_OverCapsMaxRetailQty, retailCnt, maxCaseRetailCnt));
			
			//檢核此批次門店數是否超過最大門店數
			if (putTool == "5" && retailCnt > maxCaseRetailCnt)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_RetailCntOverCaseMaxCnt, batchNo, retailCnt, maxCaseRetailCnt));


			//檢核門店數是否超過總座入數
			if (((pickTool == "4" && string.IsNullOrEmpty(putTool)) || putTool == "5") && retailCnt > capsPlainQty * capsPerPlainQty)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_OverTotalPrePlainQty, retailCnt, capsPlainQty * capsPerPlainQty));

			return new ExecuteResult(true);
		}

		#region 產生揀貨彙總資料
		/// <summary>
		/// 產生揀貨彙總資料
		/// </summary>
		/// <param name="createBatchPick">產生揀貨彙總設定</param>
		/// <returns></returns>
		public ExecuteResult CreateBatchPickData(CreateBatchPick createBatchPick)
		{
			var result = BeforeCheckCreateBatchPickData(createBatchPick);
			if (!result.IsSuccessed)
				return result;
			var sharedService = new SharedService(_wmsTransaction);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var stations = f000904Repo.GetAGVStations().ToList();

			var f051201Repo = new F051201Repository(Schemas.CoreSchema,_wmsTransaction);
			var pickSummary = f051201Repo.GetP050112PickSummaries(createBatchPick.DcCode, createBatchPick.GupCode, createBatchPick.CustCode, createBatchPick.PickOrdNos).FirstOrDefault();
			var pickSummaryDetails = f051201Repo.GetP050112PickSummaryDetails(createBatchPick.DcCode, createBatchPick.GupCode, createBatchPick.CustCode, createBatchPick.PickOrdNos).ToList();
			var pickSummaryRetails = f051201Repo.GetP050112PickSummaryRetails(createBatchPick.DcCode, createBatchPick.GupCode, createBatchPick.CustCode, createBatchPick.PickOrdNos).ToList();
			var f0515 = CreateF0515(createBatchPick, pickSummary);
			var f051503List = CreateF051503List(createBatchPick, f0515.BATCH_NO);
			var capsPlainQty = 0;// CAPS 座數
			var capsPerPlainQty = 0; //CAPS 座入數
			var maxCaseRetailCnt = 0; // CAPS 最大門店數
			
			var checkResult = CapsSettingCheck(createBatchPick.DcCode, createBatchPick.GupCode, createBatchPick.CustCode, pickSummaryRetails.Count, f0515.PICK_TOOL, f0515.PUT_TOOL,  ref capsPlainQty, ref capsPerPlainQty, ref maxCaseRetailCnt);
			if (!checkResult.IsSuccessed)
				return checkResult;
			

			var f051504List = CreateF051504List(pickSummaryRetails, f0515.BATCH_NO, capsPlainQty, capsPerPlainQty);

			var list = new List<KeyValuePair<int, List<P050112PickSummaryDetail>>>();
			switch (createBatchPick.AllotType)
			{
				case "001": //品項數平均法
					list = ToSplitPickByAvgItemCnt(f0515, pickSummaryDetails);
					break;
				case "002": //貨架數平均法
					list = ToSplitPickByAgvShelfNo(f0515, pickSummaryDetails);
					break;
			}

			var f0515Repo = new F0515Repository(Schemas.CoreSchema, _wmsTransaction);
			f0515.ALLOT_CNT = (short)list.Count();
			f0515Repo.Add(f0515);

			var f051501List = new List<F051501>();
			var f051502List = new List<F051502>();
			foreach (var master in list)
			{
				var f051501 = new F051501
				{
					BATCH_NO = f0515.BATCH_NO,
					DC_CODE = f0515.DC_CODE,
					GUP_CODE = f0515.GUP_CODE,
					CUST_CODE = f0515.CUST_CODE,
					BATCH_PICK_NO = sharedService.GetNewOrdCode("U"),
					ITEM_CNT = master.Value.Select(x => x.ITEM_CODE).Distinct().Count(),
					TOTAL_QTY = master.Value.Sum(x => x.B_PICK_QTY),
					STATUS = "0",
					STATION_NO = f0515.PICK_TOOL == "4" ? stations[master.Key].VALUE : string.Empty
				};
				f051501List.Add(f051501);
				f051502List.AddRange(
				master.Value.OrderBy(x => x.SHELF_NO).OrderBy(x => x.ITEM_CODE).Select((x, index) => new F051502
				{
					DC_CODE = f0515.DC_CODE,
					GUP_CODE = f0515.GUP_CODE,
					CUST_CODE = f0515.CUST_CODE,
					BATCH_PICK_NO = f051501.BATCH_PICK_NO,
					BATCH_PICK_SEQ = (index+1),
					SHELF_NO = x.SHELF_NO,
					LOC_CODE = x.LOC_CODE,
					ITEM_CODE = x.ITEM_CODE,
					B_PICK_QTY = x.B_PICK_QTY,
					A_PICK_QTY = 0,
					STATUS = "0",
				}));
			}

			var f051501Repo = new F051501Repository(Schemas.CoreSchema, _wmsTransaction);
			f051501Repo.BulkInsert(f051501List);
			var f051502Repo = new F051502Repository(Schemas.CoreSchema, _wmsTransaction);
			f051502Repo.BulkInsert(f051502List);
			var f051503Repo = new F051503Repository(Schemas.CoreSchema, _wmsTransaction);
			f051503Repo.BulkInsert(f051503List);
			var f051504Repo = new F051504Repository(Schemas.CoreSchema, _wmsTransaction);
			f051504Repo.BulkInsert(f051504List);

			f051201Repo.UpdateF051201PickStatus(createBatchPick.DcCode, createBatchPick.GupCode, createBatchPick.CustCode, createBatchPick.PickOrdNos, "1");

			return new ExecuteResult(true,string.Format(Properties.Resources.P050112Service_BatchPickResult,f0515.BATCH_NO,f0515.RETAIL_CNT,f0515.ITEM_CNT,f0515.TOTAL_QTY,f051501List.Count));
		}

		/// <summary>
		/// 品項數平均法
		/// </summary>
		/// <param name="f0515">揀貨彙總批號檔</param>
		/// <param name="pickSummaryDetail">揀貨彙總明細</param>
		private List<KeyValuePair<int, List<P050112PickSummaryDetail>>> ToSplitPickByAvgItemCnt(F0515 f0515,List<P050112PickSummaryDetail> pickSummaryDetail)
		{
			var gItems = pickSummaryDetail.GroupBy(x => x.ITEM_CODE).OrderBy(x=> x.Key).Select(x => x.Key);
			var partialCnt = gItems.Count() / f0515.ALLOT_CNT;
			var remainderCnt = gItems.Count() % f0515.ALLOT_CNT;
			var list = new List<KeyValuePair<int, List<P050112PickSummaryDetail>>>();
			for(var i=0;i<f0515.ALLOT_CNT;i++)
			{
				var partialItems = new List<string>();
				if(i + 1 <= remainderCnt)
					partialItems = gItems.Skip(i * partialCnt + (i*1)).Take(partialCnt + 1).ToList();
				else
					partialItems = gItems.Skip(i * partialCnt + remainderCnt).Take(partialCnt).ToList();

				if(partialItems.Count > 0)
					list.Add(new KeyValuePair<int, List<P050112PickSummaryDetail>>(i, pickSummaryDetail.Where(x => partialItems.Contains(x.ITEM_CODE)).ToList()));
			}
			return list;
		}
		/// <summary>
		/// 貨架數平均法
		/// </summary>
		/// <param name="f0515">揀貨彙總批號檔</param>
		/// <param name="pickSummaryDetail">揀貨彙總明細</param>
		/// <returns></returns>
		private List<KeyValuePair<int, List<P050112PickSummaryDetail>>> ToSplitPickByAgvShelfNo(F0515 f0515, List<P050112PickSummaryDetail> pickSummaryDetail)
		{
			var sortShelfs = SortShelfNosByCust(f0515.CUST_CODE, pickSummaryDetail);
			var partialCnt = sortShelfs.Count() / f0515.ALLOT_CNT;
			var remainderCnt = sortShelfs.Count() % f0515.ALLOT_CNT;
			var list = new List<KeyValuePair<int, List<P050112PickSummaryDetail>>>();
			for (var i = 0; i < f0515.ALLOT_CNT; i++)
			{
				var partialShelfs = new List<string>();
				if (i + 1 <= remainderCnt)
					partialShelfs = sortShelfs.Skip(i * partialCnt + (i * 1)).Take(partialCnt + 1).Select(x=> x.ShelfNo).ToList();
				else
					partialShelfs = sortShelfs.Skip(i * partialCnt + remainderCnt).Take(partialCnt).Select(x => x.ShelfNo).ToList();

				if (partialShelfs.Count > 0)
					list.Add(new KeyValuePair<int, List<P050112PickSummaryDetail>>(i, pickSummaryDetail.Where(x => partialShelfs.Contains(x.SHELF_NO)).ToList()));
			}
			return list;
		}

		/// <summary>
		/// 依貨主自訂貨架排序方式
		/// </summary>
		/// <param name="custCode">貨主編號</param>
		/// <param name="pickSummaryDetail">揀貨彙總明細</param>
		/// <returns></returns>
		private List<ShelfSort> SortShelfNosByCust(string custCode,List<P050112PickSummaryDetail> pickSummaryDetail)
		{
			var shelfSort = new List<ShelfSort>();
			var shelfDatas = pickSummaryDetail.GroupBy(x => x.SHELF_NO).ToList();
			foreach(var item in shelfDatas)
			{
				switch (custCode)
				{	
					default:
						shelfSort.Add(new ShelfSort { ShelfNo = item.Key, SortLevel = 1 });
						break;
				}
			}
			return shelfSort.OrderBy(x => x.SortLevel).ToList();
		}





		/// <summary>
		/// 建立揀貨彙總單
		/// </summary>
		/// <param name="createBatchPick">產生揀貨彙總設定</param>
		/// <param name="pickSummary">揀貨彙總統計</param>
		/// <returns></returns>
		private F0515 CreateF0515(CreateBatchPick createBatchPick,P050112PickSummary pickSummary)
		{
			var sharedService = new SharedService(_wmsTransaction);

			return new F0515
			{
				DC_CODE = createBatchPick.DcCode,
				GUP_CODE = createBatchPick.GupCode,
				CUST_CODE = createBatchPick.CustCode,
			  ALLOT_CNT = (createBatchPick.PickTool == "4") ? createBatchPick.AllotCnt : (short)1,
				ALLOT_TYPE = createBatchPick.AllotType,
				PICK_TOOL = createBatchPick.PickTool,
				BATCH_DATE = DateTime.Today,
				BATCH_NO = sharedService.GetNewOrdCode("K"),
				PICK_STATUS = "0",
				PUT_STATUS = "0",
				PRINT_FLAG = "0",
				ITEM_CNT = pickSummary.ITEM_CNT,
				RETAIL_CNT = pickSummary.RETAIL_CNT,
				TOTAL_QTY = pickSummary.TOTAL_QTY
			};
		}

		/// <summary>
		/// 建立揀貨彙總與揀貨單對應
		/// </summary>
		/// <param name="createBatchPick">產生揀貨彙總設定</param>
		/// <param name="batchNo">彙總批號</param>
		/// <returns></returns>
		private List<F051503> CreateF051503List(CreateBatchPick createBatchPick, string batchNo)
		{
			var f051503List = new List<F051503>();
			foreach (var pickOrdNo in createBatchPick.PickOrdNos)
			{
				var f051503 = new F051503
				{
					DC_CODE = createBatchPick.DcCode,
					GUP_CODE = createBatchPick.GupCode,
					CUST_CODE = createBatchPick.CustCode,
					BATCH_NO = batchNo,
					PICK_ORD_NO = pickOrdNo,
				};
				f051503List.Add(f051503);
			}
			return f051503List;
		}

		/// <summary>
		/// 建立揀貨彙總與門市與CAPS對應
		/// </summary>
		/// <param name="pickSummaryRetails">揀貨彙總門市資料</param>
		/// <param name="batchNo">彙總批號</param>
		/// <param name="capsPlainQty">Caps座數</param>
		/// <param name="capsPerPlainQty">Caps座入數</param>
		/// <returns></returns>
		private List<F051504> CreateF051504List(List<P050112PickSummaryRetail> pickSummaryRetails, string batchNo,int capsPlainQty,int capsPerPlainQty)
		{
			//如果門店數超過Caps總座入數就不產生對應表
			if (pickSummaryRetails.Count > capsPlainQty * capsPerPlainQty)
				return new List<F051504>();
			
			var f051504List = new List<F051504>();
			var channel = 1;
			var currentBoxNo = 1;
			var channelMaxBoxCnt = capsPerPlainQty; 
			foreach(var pickSummayRetail in pickSummaryRetails)
			{
				var f051504 = new F051504
				{
					BATCH_NO = batchNo,
					DC_CODE = pickSummayRetail.DC_CODE,
					GUP_CODE = pickSummayRetail.GUP_CODE,
					CUST_CODE = pickSummayRetail.CUST_CODE,
					RETAIL_CODE = pickSummayRetail.RETAIL_CODE,
					RETAIL_NAME = pickSummayRetail.RETAIL_NAME,
					STATUS = "0",
					CAPS_LOC_CODE = string.Format("{0}{1}", channel.ToString(), currentBoxNo.ToString().PadLeft(2, '0'))
				};
				f051504List.Add(f051504);
				currentBoxNo++;
				if (currentBoxNo > channelMaxBoxCnt)
				{
					channel++;
					currentBoxNo = 1;
				}
			}
			return f051504List;
		}

		/// <summary>
		/// 產生揀貨彙總資料檢查
		/// </summary>
		/// <param name="createBatchPick">產生揀貨彙總設定</param>
		/// <returns></returns>
		private ExecuteResult BeforeCheckCreateBatchPickData(CreateBatchPick createBatchPick)
		{
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			var notUnProcessPickDatas = f051201Repo.GetDatas(createBatchPick.DcCode, createBatchPick.GupCode, createBatchPick.CustCode, createBatchPick.PickOrdNos).Where(x=> x.PICK_STATUS != 0).ToList();
			if(notUnProcessPickDatas.Any())
			{
				var messages = new List<string>();
				var processPicks = notUnProcessPickDatas.Where(x => x.PICK_STATUS == 1).Select(x => x.PICK_ORD_NO).ToList();
				var completePicks = notUnProcessPickDatas.Where(x => x.PICK_STATUS == 2).Select(x => x.PICK_ORD_NO).ToList();
				var cancelPicks = notUnProcessPickDatas.Where(x => x.PICK_STATUS == 9).Select(x => x.PICK_ORD_NO).ToList();
				if (processPicks.Any())
					messages.Add(string.Format(Properties.Resources.P050112Service_PickHasProcess, string.Join("、", processPicks)));
				if (completePicks.Any())
					messages.Add(string.Format(Properties.Resources.P050112Service_PickHasComplete, string.Join("、", completePicks)));
				if (cancelPicks.Any())
					messages.Add(string.Format(Properties.Resources.P050112Service_PickHasCancel, string.Join("、", cancelPicks)));
				messages.Add(Properties.Resources.P050112Service_PleaseReChoosePick);
				return new ExecuteResult(false, string.Join(Environment.NewLine, messages));
			}
			return new ExecuteResult(true);
		}

		#endregion

		#region 刪除揀貨彙總資料

		public ExecuteResult DeleteBatchPickData(string dcCode,string gupCode,string custCode,string batchNo)
		{
			var f0515Repo = new F0515Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0515 = f0515Repo.AsForUpdate().GetData(dcCode, gupCode, custCode, batchNo);
			if (f0515 == null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_BatchPickNoExist, batchNo));
			else if (f0515.PICK_STATUS != "0")
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_BatchPickMustUnProcessToDelete, batchNo));
			else
			{
				f0515.PICK_STATUS = "9";
				f0515.PUT_STATUS = "9";
				f0515Repo.Update(f0515);
				var f051501Repo = new F051501Repository(Schemas.CoreSchema, _wmsTransaction);
				f051501Repo.UpdateStatusByBatcnNo(dcCode, gupCode, custCode, batchNo, "9");
				var f051502Repo = new F051502Repository(Schemas.CoreSchema, _wmsTransaction);
				f051502Repo.UpdateStatusByBatchNo(dcCode, gupCode, custCode, batchNo, "9");
				var f051503Repo = new F051503Repository(Schemas.CoreSchema);
				var pickOrdNos = f051503Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.BATCH_NO == batchNo).Select(x => x.PICK_ORD_NO).ToList();
				var f051504Repo = new F051504Repository(Schemas.CoreSchema, _wmsTransaction);
				f051504Repo.UpdateStatusByBatchNo(dcCode, gupCode, custCode, batchNo, "9");
				var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
				f051201Repo.UpdateF051201PickStatus(dcCode, gupCode, custCode, pickOrdNos, "0");
				return new ExecuteResult(true);
			}
		}

		#endregion

		#region AGV啟動
		public ExecuteResult AGVStartupPick(string dcCode,string gupCode,string custCode,string batchNo)
		{
			var f0515Repo = new F0515Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0515 = f0515Repo.AsForUpdate().GetData(dcCode, gupCode, custCode, batchNo);
			var f051501Repo = new F051501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051501s = f051501Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.BATCH_NO == batchNo).ToList();
			var hasWorkAGVBatchData = f0515Repo.GetAGVHasWorkBatchByNotInBatchNo(dcCode, gupCode, custCode, batchNo);
			if (f0515 == null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_BatchPickNoExist, batchNo));

			if (f0515.PICK_TOOL == "4" && hasWorkAGVBatchData != null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_ThisBatchIsWorkInAGV, hasWorkAGVBatchData.BATCH_NO));
			
			f0515.PICK_STATUS = "1";
			f051501s.ForEach(o => o.STATUS = "1");
			f051501Repo.BulkUpdate(f051501s);
			f0515Repo.Update(f0515);
			var f050803Repo = new F050803Repository(Schemas.CoreSchema, _wmsTransaction);
			f050803Repo.InsertByF0515(f0515.DC_CODE, f0515.GUP_CODE, f0515.CUST_CODE, f0515.BATCH_NO);
			return new ExecuteResult(true);
		}

		#endregion

		#region 人工揀貨
		public ExecuteResult ArtificalPick(string dcCode,string gupCode,string custCode,string batchNo)
		{
			var f0515Repo = new F0515Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0515 = f0515Repo.AsForUpdate().GetData(dcCode, gupCode, custCode, batchNo);
			var f051501Repo = new F051501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051501s = f051501Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.BATCH_NO == batchNo).ToList();
			if (f0515 == null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_BatchPickNoExist, batchNo));
			if(f0515.PICK_STATUS == "0" || f0515.PICK_STATUS == "1")
			{
				f0515.PICK_STATUS = "2";
				f0515.PRINT_FLAG = "1";
				f051501s.ForEach(o => o.STATUS = "3");
				f051501Repo.BulkUpdate(f051501s);
				f0515Repo.Update(f0515);
				var f050803Repo = new F050803Repository(Schemas.CoreSchema, _wmsTransaction);
				f050803Repo.InsertByF0515(f0515.DC_CODE, f0515.GUP_CODE, f0515.CUST_CODE, f0515.BATCH_NO);
			}
			return new ExecuteResult(true);
		}
		#endregion

		#region 人工/CAPS播種

		public ExecuteResult ExecSow(string dcCode, string gupCode, string custCode, string batchNo, string putTool)
		{
			var f0515Repo = new F0515Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051501Repo = new F051501Repository(Schemas.CoreSchema, _wmsTransaction);

			var f0515 = f0515Repo.AsForUpdate().GetData(dcCode, gupCode, custCode, batchNo);
			var f051501s = f051501Repo.AsForUpdate().GetDatasByTrueAndCondition(o=>o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.BATCH_NO == batchNo).ToList();

			var capsPlainQty = 0;// CAPS 座數
			var capsPerPlainQty = 0; //CAPS 座入數
			var maxCaseRetailCnt = 0; // CAPS 最大門店數
			
			if (f0515 == null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_BatchPickNoExist, batchNo));

			if (putTool == "5") //播種工具 = CAPS
			{
				var checkResult = CapsSettingCheck(dcCode, gupCode, custCode, f0515.RETAIL_CNT, f0515.PICK_TOOL,putTool, ref capsPlainQty, ref capsPerPlainQty, ref maxCaseRetailCnt, batchNo);
				if (!checkResult.IsSuccessed)
					return checkResult;
			}

			if (f0515.PUT_STATUS == "0" || f0515.PUT_STATUS == "1")
			{
				if (f0515.PUT_STATUS == "0")
				{
					var f051601Repo = new F051601Repository(Schemas.CoreSchema, _wmsTransaction);
					f051601Repo.InsertF051601ByBatchNo(dcCode, gupCode, custCode, batchNo);
					var f051602Repo = new F051602Repository(Schemas.CoreSchema, _wmsTransaction);
					f051602Repo.InsertF051602ByBatchNo(dcCode, gupCode, custCode, batchNo);
				}
				f0515.PUT_STATUS = "1";
				f0515.PUT_TOOL = putTool;
				f0515.TRANS_DATE = DateTime.Now;
				f0515.TRANS_STAFF = Current.Staff;
				f0515.TRANS_NAME = Current.StaffName;
				f0515Repo.Update(f0515);
			}

			return new ExecuteResult(true,Properties.Resources.P050112Service_SowDataDownLoadSuccess);
		}

		#endregion

		#region 下傳CAPS
		public ExecuteResult ExecSowToCaps(string dcCode, string gupCode, string custCode, string batchNo, string putTool)
		{
			switch (putTool) 
			{
				case "5"://播種工具為CAPS
					break;
			}
			return new ExecuteResult(true, Properties.Resources.P050112Service_SowDataDownLoadSuccess);
		}
		#endregion

		#region CAPS回傳
		public ExecuteResult ExecCapsReturn(string dcCode, string gupCode, string custCode, string batchNo)
		{
			return new ExecuteResult(true);
		}
		#endregion

		#region 人工回傳
		public ExecuteResult ImportArtificalSowReturn(string dcCode, string gupCode, string custCode, string batchNo, List<PutReportData> datas)
		{
			var f0515Repo = new F0515Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0515 = f0515Repo.AsForUpdate().GetData(dcCode, gupCode, custCode, batchNo);
			if (f0515 == null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_BatchPickNoExist, batchNo));
			else if(f0515.PUT_STATUS != "1")
			{
				var f000904Repo = new F000904Repository(Schemas.CoreSchema);
				var statusList = f000904Repo.GetF000904Data("F0515", "PUT_STATUS");
				var statusName = statusList.FirstOrDefault(x => x.VALUE == f0515.PUT_STATUS)?.NAME;
				if (string.IsNullOrEmpty(statusName))
					statusName = "";
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_PutStatusCanotArtificalSowReturn, batchNo, statusName));
			}

			var f051602Repo = new F051602Repository(Schemas.CoreSchema);
			var f051602s = f051602Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.BATCH_NO == batchNo).ToList();
			f051602s.ForEach(x => { x.PACK_QTY = null; });
			var f051603Repo = new F051603Repository(Schemas.CoreSchema, _wmsTransaction);
			var existF051603List = f051603Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.BATCH_NO == batchNo).ToList();
			var findF051602List = new List<F051602>();
			var addF051603List = new List<F051603>();
			var duplcateDatas = datas.GroupBy(x => new { x.LOC_CODE, x.ITEM_CODE, x.RETAIL_CODE }).Where(x=> x.Count() >1);
			if(duplcateDatas.Any())
			{
				return new ExecuteResult(false, Properties.Resources.P050112Service_PutReturnDuplicate);
			}
			foreach (var item in datas)
			{
				var findF051602s = f051602s.Where(x => x.LOC_CODE == item.LOC_CODE && x.ITEM_CODE == item.ITEM_CODE && x.RETAIL_CODE == item.RETAIL_CODE).ToList();
				if (!findF051602s.Any())
					return new ExecuteResult(false,string.Format(Properties.Resources.P050112Service_NotInCurrentBatchPutData,item.LOC_CODE,item.ITEM_CODE,item.RETAIL_CODE,batchNo));
				else if( findF051602s.Sum(x=>x.PLAN_QTY) < item.ACT_QTY)
					return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_PutReturnActQtyOverPlanQty, item.LOC_CODE, item.ITEM_CODE, item.RETAIL_CODE, item.ACT_QTY, findF051602s.Sum(x => x.PLAN_QTY)));
				if (existF051603List.Any(x => x.LOC_CODE == item.LOC_CODE && x.ITEM_CODE == item.ITEM_CODE && x.RETAIL_CODE == item.RETAIL_CODE))
				{
					findF051602List.AddRange(findF051602s);
					continue;
				}

				foreach (var findF051602 in findF051602s)
				{
					long actQty = 0;
					if(item.ACT_QTY >= findF051602.PLAN_QTY)
					{
						actQty = findF051602.PLAN_QTY;
						item.ACT_QTY -= actQty;
					}
					else
					{
						actQty = item.ACT_QTY ?? 0;
						item.ACT_QTY = 0;
					}
					addF051603List.Add(CreateF051603(findF051602,actQty));
					findF051602.PACK_QTY = actQty;
				}
				findF051602List.AddRange(findF051602s);
			}
			var notExistReturnDataF051602s = f051602s.Except(findF051602List);
			addF051603List.AddRange(notExistReturnDataF051602s.Select(x => CreateF051603(x, 0)));

			f0515.PUT_STATUS = "2";
			f0515.RECV_NAME = Current.StaffName;
			f0515.RECV_STAFF = Current.Staff;
			f0515.RECV_DATE = DateTime.Now;
			f0515Repo.Update(f0515);

			f051602Repo.BulkUpdate(f051602s);

			var f051601Repo = new F051601Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051601s = f051601Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.BATCH_NO == batchNo).ToList();
			f051601s.ForEach(x =>
			{
				x.STATUS = "2";
			});
			f051601Repo.BulkUpdate(f051601s);

			f051603Repo.BulkInsert(addF051603List);
			return new ExecuteResult(true);
		}
	  private F051603 CreateF051603(F051602 f051602,long actQty)
		{
			var f051603 = new F051603
			{
				DC_CODE = f051602.DC_CODE,
				GUP_CODE = f051602.GUP_CODE,
				CUST_CODE = f051602.CUST_CODE,
				BATCH_DATE = f051602.BATCH_DATE,
				BATCH_NO = f051602.BATCH_NO,
				CAR_PERIOD = f051602.CAR_PERIOD,
				CUST_ORD_LIST = f051602.CUST_ORD_LIST,
				DELV_NO = f051602.DELV_NO,
				DELV_WAY = f051602.DELV_WAY,
				EMP_ID = Current.Staff,
				ITEM_CODE = f051602.ITEM_CODE,
				ITEM_NAME = f051602.ITEM_NAME,
				ITEM_UNIT = f051602.ITEM_UNIT,
				LOC_CODE = f051602.LOC_CODE,
				RETAIL_CODE = f051602.RETAIL_CODE,
				RETAIL_NAME = f051602.RETAIL_NAME,
				WMS_ORD_NO = f051602.WMS_ORD_NO,
				ORDER_QTY = f051602.ORDER_QTY,
				PLAN_QTY = f051602.PLAN_QTY,
				PACK_TIME = DateTime.Now,
				PACK_QTY = actQty,
				CARTON = f051602.WMS_ORD_NO
				//CARTON = string.Format("{0}{1}B1001", f051602.BATCH_NO, f051602.RETAIL_CODE)
			};
			return f051603;
		}
		#endregion

		#region 調整AGV工作站
		public ExecuteResult AdjustAGVStations(string dcCode,string gupCode,string custCode,string batchNo,List<BatchPickStation> batchPickStations)
		{
			var f0515Repo = new F0515Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0515 = f0515Repo.AsForUpdate().GetData(dcCode, gupCode, custCode, batchNo);
			var f051501Repo = new F051501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051501s = f051501Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.BATCH_NO == batchNo).ToList();
			if (f0515 == null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_BatchPickNoExist, batchNo));
			var updF051501s = new List<F051501>();
			foreach (var f051501 in f051501s)
			{
				var item = batchPickStations.FirstOrDefault(x => x.BATCH_PICK_NO == f051501.BATCH_PICK_NO);
				if(item!=null && item.STATION_NO != f051501.STATION_NO)
				{
					if(f051501.STATUS == "0" || f051501.STATUS == "1")
					{
						f051501.STATION_NO = item.STATION_NO;
						updF051501s.Add(f051501);
					}
					else
					{
						var f000904Repo = new F000904Repository(Schemas.CoreSchema);
						var statusList = f000904Repo.GetF000904Data("F051501", "PUT_STATUS");
						var statusName = statusList.FirstOrDefault(x => x.VALUE == f051501.STATUS)?.NAME;
						if (string.IsNullOrEmpty(statusName))
							statusName = "";
						return new ExecuteResult(false, string.Format(Properties.Resources.P050112Service_BatchPickNoStatusCantToAdjStation, f051501.BATCH_PICK_NO, statusName));
					}
				}
			}
			if (updF051501s.Any())
				f051501Repo.BulkUpdate(updF051501s);
			return new ExecuteResult(true);
		}

		#endregion
	}
}
