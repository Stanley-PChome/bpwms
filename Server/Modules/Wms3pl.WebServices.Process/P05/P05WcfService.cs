using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Common.Collections;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P05.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P05.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P05WcfService
	{

		[OperationContract]
		public ExecuteResult InsertF050101(F050101 f050101, ObservableCollection<F050102Ex> f050102Exs, F050304 f050304 = null)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);
			var result = srv.InsertF050101(f050101, f050102Exs);
			var result_f050304 = new ExecuteResult { IsSuccessed = false };

			if (result.IsSuccessed)
			{
				if (f050304 != null && f050101.CVS_TAKE == "1")
				{
					f050304.ORD_NO = result.Message.Split('：')[1].Split(' ')[0];
					result_f050304 = srv.InsertF050304(f050304);
					if (!result_f050304.IsSuccessed)
						return result_f050304;
				}
				else
				{
					result_f050304.IsSuccessed = true;
				}
			}
			if (result_f050304.IsSuccessed && result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			return result;
		}
		[OperationContract]
		public ExecuteResult InsertExcelOrder(List<F050101> f050101s, List<F050102Excel> f050102s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);
			var result = srv.InsertExcelOrder(f050101s, f050102s);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF050101(F050101 f050101, ObservableCollection<F050102Ex> f050102Exs, F050304 f050304 = null)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);
			var f05101Repo = new F050101Repository(Schemas.CoreSchema, wmsTransaction);
			F050101 f050101Data = f05101Repo.Find(x => x.GUP_CODE == f050101.GUP_CODE && x.CUST_CODE == f050101.CUST_CODE && x.DC_CODE == f050101.DC_CODE && x.ORD_NO == f050101.ORD_NO);
			var result_f050304 = new ExecuteResult { IsSuccessed = false };

			if (f050101Data.CVS_TAKE == "0" && f050101.CVS_TAKE == "1")
			{
				result_f050304 = srv.InsertF050304(f050304);
			}
			else if (f050101Data.CVS_TAKE == "0" && f050101.CVS_TAKE == "0")
			{
				result_f050304.IsSuccessed = true;
			}
			else if (f050101Data.CVS_TAKE == "1" && f050101.CVS_TAKE == "0")
			{
				result_f050304 = srv.DeleteF050304(f050304);
			}
			else if (f050101Data.CVS_TAKE == "1" && f050101.CVS_TAKE == "1")
			{
				result_f050304 = srv.UpdateF050304(f050304);
			}
			if (!result_f050304.IsSuccessed)
			{
				return result_f050304;
			}
			var result = srv.UpdateF050101(f050101, f050102Exs, f050304);
			if (result.IsSuccessed && result_f050304.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult DeleteF050101(F050101 f050101)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);
			//var result_f050304 = new ExecuteResult { IsSuccessed = false };
			//if (f050101.CVS_TAKE == "1")
			//{
			//	result_f050304 = srv.DeleteF050304(new F050304()
			//	{
			//		DC_CODE = f050101.DC_CODE,
			//		GUP_CODE = f050101.GUP_CODE,
			//		CUST_CODE = f050101.CUST_CODE,
			//		ORD_NO = f050101.ORD_NO
			//	});
			//}
			//else
			//{
			//	result_f050304.IsSuccessed = true;
			//}
			var result = srv.DeleteF050101(f050101);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult ApproveF050101(F050101 f050101, ObservableCollection<F050102Ex> f050102Exs)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);
			var result = srv.ApproveF050101(f050101, f050102Exs);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		/// <summary>
		/// 出貨抽稽維護 設定不出貨
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="ordNoList"></param>
		[OperationContract]
		public ExecuteResult UpdateStatusCancelByWmsOrdNo(string dcCode, string gupCode, string custCode, string[] ordNoList, string[] wmsOrdNoList)
		{
			var wmsTransaction = new WmsTransaction();
			var p050801Service = new P050801Service(wmsTransaction);
			var result = p050801Service.UpdateStatusCancelByWmsOrdNo(dcCode, gupCode, custCode, ordNoList, wmsOrdNoList);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		


		#region 呼叫共用function 來更新 來源單據狀態 (調撥)
		[OperationContract]
		public ExecuteResult UpdateSourceNoStatus(F050801 f050801)
		{
			// 呼叫共用function 來更新 來源單據狀態
			var wmsTransaction = new WmsTransaction();
			var sharedSrv = new SharedService(wmsTransaction);
			ExecuteResult result = new ExecuteResult { IsSuccessed = true };

			var dataResult = sharedSrv.UpdateSourceNoStatus(SourceType.Order, f050801.DC_CODE, f050801.GUP_CODE
										, f050801.CUST_CODE, f050801.WMS_ORD_NO, f050801.STATUS.ToString());

			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region 若訂單類型為超取 則執行insertF050304
		[OperationContract]
		public ExecuteResult InsertF050304(F050304 f050304)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);
			var result = srv.InsertF050304(f050304);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;

		}
		#endregion

		#region 若訂單類型為非超取 則執行DeleteF050304
		[OperationContract]
		public ExecuteResult DeleteF050304(F050304 f050304)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);

			var result = srv.DeleteF050304(f050304);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;

		}
		#endregion

		#region 若訂單類型為超取 且更新資料 則執行UpdateF050304
		[OperationContract]
		public ExecuteResult UpdateF050304(F050304 f050304)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);

			var result = srv.UpdateF050304(f050304);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;

		}
		#endregion

		#region 更新配送門市
		[OperationContract]
		public ExecuteResult UpdateOrderDelvRetail(string dcCode, string GupCode, string custCode, string ordNo, string retailCode, string retailName)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050302Service(wmsTransation);
			var result = service.UpdateOrderDelvRetail(dcCode, GupCode, custCode, ordNo, retailCode, retailName);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		#endregion

		[OperationContract]
		public ExecuteResult BatchApproveF050101(string gupCode, string custCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050302Service(wmsTransaction);
			var result = srv.BatchApprove(gupCode, custCode);
			wmsTransaction.Complete();

			return result;
		}
	

		#region 貨主訂單手動挑單-總庫試算

		[OperationContract]
		public ExecuteResult AllotStockTrialCalculation(string dcCode, string gupCode, string custCode, List<string> ordNos)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050304Service(wmsTransaction);
			var result = srv.AllotStockTrialCalculation(dcCode, gupCode, custCode, ordNos);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		

		[OperationContract]
		public ExecuteResult SaveP05030401CreateAllocation(string dcCode, string gupCode, string custCode,string calNo, List<decimal> ids)
		{
			var wmsTransaction = new WmsTransaction();
			var result = new ExecuteResult(true);
			var srv = new P050304Service(wmsTransaction);
				result = srv.CreateAllocation(dcCode, gupCode, custCode, calNo, ids);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		public List<F1903> GetF1903s(string gupCode, string custCode, IEnumerable<string> itemCodes)
		{
			var _f1903Rep = new F1903Repository(Schemas.CoreSchema);
			var _f1903sDict = new Dictionary<Keys<string, string, string>, F1903>();

			var newItemCodes = itemCodes.Where(itemCode => !_f1903sDict.ContainsKey(new Keys<string, string, string>(gupCode, custCode, itemCode))).ToList();
			if (newItemCodes.Any())
			{
				var f1903s = _f1903Rep.InWithTrueAndCondition("ITEM_CODE", newItemCodes, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode);
				foreach (var f1903 in f1903s)
					_f1903sDict.Add(new Keys<string, string, string>(gupCode, custCode, f1903.ITEM_CODE), f1903);
			}

			return itemCodes.Where(itemCode => _f1903sDict.ContainsKey(new Keys<string, string, string>(gupCode, custCode, itemCode)))
							.Select(itemCode => _f1903sDict[new Keys<string, string, string>(gupCode, custCode, itemCode)])
							.ToList();
		}

		/// <summary>
		/// 透過商品不足的數量與現有虛擬儲位庫存的數量，計算出要從補貨區取得的數量
		/// </summary>
		/// <param name="notEnoughItems"></param>
		/// <param name="virtualQtyItems"></param>
		/// <returns></returns>
		private static List<ItemQty> GetResupplyQtyItems(List<ItemQty> notEnoughItems, List<ItemQty> virtualQtyItems)
		{
			var query = from notEnoughItem in notEnoughItems
									join virtualQtyItem in virtualQtyItems
									on notEnoughItem.ItemCode equals virtualQtyItem.ItemCode into b
									from c in b.DefaultIfEmpty()
										// 虛擬儲位庫存數量: 用來在 select 時，取得應該要從補貨區取得的數量計算
										// 1.若虛擬儲位沒庫存
										// 2.若虛擬儲位庫存大於不足的揀貨區庫存數，則只要取不足的揀貨數量
										// 3.若虛擬儲位庫存小於不足的揀貨區庫存數，則取虛擬儲位庫存數
									let virtualQty = (c == null) ? 0
																	 : (c.Qty > notEnoughItem.Qty) ? notEnoughItem.Qty
																								 : c.Qty
									// 如果虛擬儲位庫存數大於不足的揀貨庫存數，就不用從補貨區取得，所以只需要有 不足的揀貨庫存數 > 虛擬儲位庫存數
									where notEnoughItem.Qty > virtualQty
									select new ItemQty
									{
										ItemCode = notEnoughItem.ItemCode,
										LocCode = notEnoughItem.LocCode,
										Qty = notEnoughItem.Qty - virtualQty
									};

			return query.ToList();
		}
		#endregion

		#region  P050112 揀貨彙總作業
		[OperationContract]
		public int GetP050112PickRetailCount(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetDatasByPickOrdNos(dcCode, gupCode, custCode, pickOrdNos).Select(x => x.RETAIL_CODE).Distinct().Count();
		}
		[OperationContract]
		public ExecuteResult CreateBatchPickData(CreateBatchPick createBatchPick)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.CreateBatchPickData(createBatchPick);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult DeleteBatchPickData(string dcCode, string gupCode, string custCode, string batchNo)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.DeleteBatchPickData(dcCode,gupCode,custCode,batchNo);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult AGVStartupPick(string dcCode, string gupCode, string custCode, string batchNo)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.AGVStartupPick(dcCode, gupCode, custCode, batchNo);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult ArtificalPick(string dcCode, string gupCode, string custCode, string batchNo)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.ArtificalPick(dcCode, gupCode, custCode, batchNo);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult ExecSow(string dcCode, string gupCode, string custCode, string batchNo, string putTool)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.ExecSow(dcCode, gupCode, custCode, batchNo,putTool);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]

		public ExecuteResult ExecSowToCaps(string dcCode, string gupCode, string custCode, string batchNo, string putTool)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.ExecSowToCaps(dcCode, gupCode, custCode, batchNo, putTool);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult ExecCapsReturn(string dcCode, string gupCode, string custCode, string batchNo)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.ExecCapsReturn(dcCode, gupCode, custCode, batchNo);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult ImportArtificalSowReturn(string dcCode,string gupCode,string custCode,string batchNo,List<PutReportData> datas)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.ImportArtificalSowReturn(dcCode, gupCode, custCode, batchNo,datas);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult AdjustAGVStations(string dcCode, string gupCode, string custCode, string batchNo, List<BatchPickStation> batchPickStations)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P050112Service(wmsTransation);
			var result = service.AdjustAGVStations(dcCode, gupCode, custCode, batchNo, batchPickStations);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
        #endregion

        #region B2C揀貨單列印/補印 O單號條碼列印
        [OperationContract]
        public IQueryable<RP0501010004Model> GetPrintDataWmsOrdNo()
        {
            return new List<RP0501010004Model>().AsQueryable();
        }
        #endregion

        #region B2C揀貨單列印/補印 P單號條碼列印
        [OperationContract]
        public IQueryable<RP0501010005Model> GetPrintDataPickOrdNo()
        {
            return new List<RP0501010005Model>().AsQueryable();
        }
        #endregion

        #region B2C揀貨單列印/補印 揀貨總表列印
        [OperationContract]
        public IQueryable<P050103ReportData> GetPrintPickData()
        {
            return new List<P050103ReportData>().AsQueryable();
        }
		#endregion

		#region 列印揀貨單_批次揀貨單百分比計算&補揀單修改揀貨工具
		[OperationContract]
		public ExecuteResult CalcatePercentWithUpdPickTool(string dcCode, string gupCode, string custCode, List<CalcatePickPercent> calcatePickPercentList, List<ChangePickTool> changePickToolList)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050104Service(wmsTransaction);
			var result = srv.CalcatePercentWithUpdPickTool(dcCode, gupCode, custCode, calcatePickPercentList, changePickToolList);
			wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region 列印批次揀貨單
		[OperationContract]
		public ExecuteResult PrintUpdateBatchPickNo(BatchPickNoList batchPickNoList, bool useLMSRoute)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050104Service(wmsTransaction);
			var result = srv.PrintUpdateBatchPickNo(batchPickNoList, useLMSRoute);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return result;
		}

		#endregion

		#region 列印補揀單
		[OperationContract]
		public ExecuteResult PrintUpdateRePickNo(RePickNoList rePickNoList, bool useLMSRoute)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P050104Service(wmsTransaction);
			var result = srv.PrintUpdateRePickNo(rePickNoList, useLMSRoute);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return result;
		}
		#endregion
	}
}
