using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
	/// <summary>
	/// 批次快速移轉庫存排程
	/// </summary>
	public class BatchFastTransferStockService
	{
		private List<ApiCkeckColumnModel> itemCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ItemCode",  Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "AdjQty",    Type = typeof(int),      MaxLength = 9 , Nullable = false },
			new ApiCkeckColumnModel{  Name = "ValidDate", Type = typeof(DateTime) , Nullable = false },
			new ApiCkeckColumnModel{  Name = "MakeNo",    Type = typeof(string),   MaxLength = 40, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SnList",    Type = typeof(string[]) }
		};
		/// <summary>
		/// 商品資料清單
		/// </summary>
		//private List<F1903> _f1903List;
		private TransApiBaseService _tacService;
		private CommonService _commonService;
		private SharedService _sharedService;
		private SerialNoService _serialNoService;
		private List<string> _msgList;
		private int _errorCnt;
		private Stack<string> _newOrderNoList { get; set; }
		private readonly object balanceLock = new object();
		public BatchFastTransferStockService()
		{
			_tacService = new TransApiBaseService();
			_commonService = new CommonService();
			_sharedService = new SharedService();
			_serialNoService = new SerialNoService();

		}

		/// <summary>
		/// 進入點
		/// </summary>
		/// <returns></returns>
		public ApiResult ExecBatchFastTransferStock()
		{
			var f191305Repo = new F191305Repository(Schemas.CoreSchema);
			var procDatas = f191305Repo.AsForUpdate().GetNoProcessDatas().ToList();

			_msgList = new List<string>();
			_msgList.Add(Environment.NewLine);
			if (!procDatas.Any())
			{
				_msgList.Add("無資料，不需處理");
				return new ApiResult { IsSuccessed = true, MsgCode = "200", MsgContent = string.Join(Environment.NewLine, _msgList) };
			}

			return ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSH_F009008, "0", "0", "0", "ExecBatchFastTransferStock", new object { }, () =>
			{

				_newOrderNoList = _sharedService.GetNewOrdStackCodes("J", procDatas.Count);

				////一筆一筆處理並commit;
				var range = 8; // 一次跑8筆資料同步執行
				int pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(procDatas.Count()) / range));
				var dis = new Dictionary<int,List<F191305>>();
				for (var i = 0; i < range; i++)
				{
					var datas = procDatas.Skip(i * pageCount).Take(pageCount).ToList();
					dis.Add(i,datas);
				}
				Parallel.ForEach(dis, ds =>
				{
					foreach (var d in ds.Value)
					{
						var adjustNo = string.Empty;
						lock (balanceLock)
						{
							adjustNo = _newOrderNoList.Pop();
						}
						Process(d, adjustNo);
					}
				});

				var k = 0;
				var failed = true;
				while (k < 5 && failed) { 
						try
						{
							f191305Repo.BulkUpdate(procDatas);
							failed = false;
						}
						catch (Exception ex)
						{
							k++;
						Thread.Sleep(2000);
					}
			}; 


				//for (var i =0;i< pages; i++)
				//{
				//	var datas = procDatas.Skip(i * range).Take(range).ToList();
				//	var tasks = new List<Task>();
				//	foreach (var data in datas)
				//	{
				//		var adjustNo = _newOrderNoList.Pop();
				//		var task = Task.Run(() => Process(data,adjustNo));
				//		tasks.Add(task);
				//	}
				//	Task.WhenAll(tasks.ToArray()).Wait();
					
				//}

				return new ApiResult { IsSuccessed = _errorCnt == 0 , MsgCode = _errorCnt == 0 ? "200" : "99999", MsgContent = string.Join(Environment.NewLine, _msgList) };
			}, true);
		}


		private  bool Process(F191305 data,string adjustNo)
		{
			//var f191305Repo = new F191305Repository(Schemas.CoreSchema);
			try
			{
				var wmsTransaction = new WmsTransaction();

				var req = JsonConvert.DeserializeObject<BatchFlashStockTransferReq>(data.SEND_DATA);
				var res = CheckData(req, data);
				if (!res.IsSuccessed)
				{
					data.STATUS = "2";
				}
				if (data.STATUS == "0")
				{
					// 取得業主編號
					//string gupCode = _commonService.GetGupCode(req.CustCode);
					string gupCode = "10";
					res = ProcessApiDatas(req.DcCode, gupCode, req.CustCode, req.LocCode, req.TransactionNo, req.Result, wmsTransaction, adjustNo);
					if (!res.IsSuccessed)
						data.STATUS = "2";
					else
						data.STATUS = "1";
				}

				if (data.STATUS == "1")
				{
					wmsTransaction.Complete();
					_msgList.Add(string.Format("{0} [交易編號:{1}]處理成功 {2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), data.TRANSACTION_NO, Environment.NewLine));
				}
				else
				{
					var datas = new List<string>();
					if (res.Data != null)
					{
						datas = ((List<BatchFlashStockTransferCheck>)res.Data).Select(x => string.Format("{0} {1} {2}", !string.IsNullOrWhiteSpace(x.LocCode) ? "儲位:" + x.LocCode : "", !string.IsNullOrWhiteSpace(x.ItemCode) ? "品號:" + x.ItemCode : "", x.MsgContent)).ToList();
					}
					else
						datas.Add(res.MsgContent);
					_errorCnt++;
					_msgList.Add(string.Format("{0} [交易編號:{1}]處理失敗 {2} {3} {2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), data.TRANSACTION_NO, Environment.NewLine, string.Join(Environment.NewLine, datas)));
				}
			}
			catch (Exception ex)
			{
				_errorCnt++;
				data.STATUS = "0";
				_msgList.Add(string.Format("{0} [交易編號:{1}]發生錯誤 {2} {3} {2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), data.TRANSACTION_NO, Environment.NewLine, ex.Message));
			}
			//finally
			//{
			//	f191305Repo.Update(data);
			//}
			return true;
		}

		private ApiResult CheckData(BatchFlashStockTransferReq req, F191305 f191305)
		{
			var res = new ApiResult { IsSuccessed = true };
			if (req.DcCode != "12")
				return new ApiResult { IsSuccessed = false, MsgCode = "20051", MsgContent = _tacService.GetMsg("20051") };
			if (req.CustCode != "010001")
				return new ApiResult { IsSuccessed = false, MsgCode = "20052", MsgContent = _tacService.GetMsg("20052") };
#if DEBUG
			if (req.LocCode != "A01010405")
				return new ApiResult { IsSuccessed = false, MsgCode = "20116", MsgContent = _tacService.GetMsg("20116") };
#else
      if (req.LocCode != "A0101AA01")
				return new ApiResult { IsSuccessed = false, MsgCode = "20116", MsgContent = _tacService.GetMsg("20116") };
#endif

			var ctaService = new CheckTransApiService();
			//// 檢核物流中心 必填、是否存在
			//ctaService.CheckDcCode(ref res, req);
			//if (!res.IsSuccessed)
			//	return res;

			////檢核貨主編號 必填、是否存在
			//	ctaService.CheckCustCode(ref res, req);
			//if (!res.IsSuccessed)
			//	return res;

			//// 檢核儲位編號 必填、是否存在
			//ctaService.CheckLocCode(ref res, req);
			//if (!res.IsSuccessed)
			//	return res;

			if (f191305.DC_CODE != req.DcCode)
			{
				res.IsSuccessed = false;
				res.MsgContent = "F191305.DC_CODE與JSON DC_CODE不一致";
				return res;
			}

			if (f191305.TRANSACTION_NO != req.TransactionNo)
			{
				res.IsSuccessed = false;
				res.MsgContent = "F191305.TRANSACTION_NO與JSON TRANSACTION_NO不一致";
				return res;
			}

			// 檢核傳入物件
			ctaService.CheckResult(ref res, req);
			if (!res.IsSuccessed)
				return res;

			return res;
		}

		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, string locCode, string transationNo, List<BatchFlashStockTransferResult> items,WmsTransaction wmsTransaction,string adjustNo)
		{
			#region 變數

			var res = new ApiResult();
			#endregion

			#region Private Property
			//var itemCodes = items.Where(x => !string.IsNullOrWhiteSpace(x.ItemCode)).Select(x => x.ItemCode).Distinct().ToList();

			// 商品資料清單
			//_f1903List = _commonService.GetProductList(gupCode, custCode, itemCodes);
			#endregion
		
			#region 檢核
			var data = new List<BatchFlashStockTransferCheck>();

			items.ForEach(item =>
			{
				//// 強制把序號轉大寫
				//if (item.SnList != null && item.SnList.Any())
				//{
				//	for (var i = 0; i < item.SnList.Count; i++)
				//	{
				//		if (!string.IsNullOrWhiteSpace(item.SnList[i]))
				//			item.SnList[i] = item.SnList[i].ToUpper();
				//	}
				//	// 排除空白或null序號
				//	item.SnList = item.SnList.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
				//}
				var resl = CheckItem(dcCode, gupCode, custCode, locCode, item);

				if (!resl.IsSuccessed)
					data.AddRange((List<BatchFlashStockTransferCheck>)resl.Data);
			});

			// 同一交易編號有序號重複也要顯示錯誤訊息
			//var repeatSns = items.Where(x => x.SnList != null && x.SnList.Any()).SelectMany(y => y.SnList).GroupBy(x => x).Select(z => new { SerialNo = z.Key, Cnt = z.Count() }).Where(x => x.Cnt > 1).Select(x => x.SerialNo).ToList();
			//if (repeatSns.Any())
			//	data.Add(new BatchFlashStockTransferCheck {  MsgCode = "20121", MsgContent = string.Format(_tacService.GetMsg("20121"), string.Join("、", repeatSns)) });

			#endregion
			#region 建立調整單
			if (!data.Any())
			{
				var today = DateTime.Today;

				// 儲位資料
				//var f1912 = _commonService.GetLoc(dcCode, locCode);
				string warehouseId =  "G01";
#if DEBUG
				warehouseId = "G01";
#endif

				var result = CreateAdjustOrder(new AdjustOrderParam
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					AdjustType = AdjustType.FastStockTransfer,
					SourceType = null,
					SourceNo = null,
					WorkType = null,
					CheckSerialItem = false,
					AdjustStockDetails = items.Select(x => new AdjustStockDetail
					{
						WarehouseId = warehouseId,
						LocCode = locCode,
						ItemCode = x.ItemCode?.ToUpper(),
						ValidDate = x.ValidDate.Value,
						EnterDate = today,
						MakeNo = x.MakeNo,
						PalletCtrlNo = "0",
						BoxCtrlNo = "0",
						WORK_TYPE = "0", //調入
						AdjQty = x.AdjQty,
						Cause = "900",
						CasueMemo = "交易編號" + transationNo,
						SerialNoList = x.SnList
					}).ToList()
				}, wmsTransaction,adjustNo);
				if (!result.IsSuccessed)
					data.Add(new BatchFlashStockTransferCheck { MsgCode = "99999", MsgContent = result.Message });
			}
			#endregion
			if (data.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21002", MsgContent = _tacService.GetMsg("21002"), Data = data };
			else
				return new ApiResult { IsSuccessed = true, MsgCode = "21001", MsgContent = _tacService.GetMsg("21001") };

		}

		/// <summary>
		/// 資料處理3
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public ApiResult CheckItem(string dcCode, string gupCode, string custCode, string locCode, BatchFlashStockTransferResult item)
		{
		
			ApiResult result = new ApiResult();
			List<BatchFlashStockTransferCheck> data = new List<BatchFlashStockTransferCheck>();

			// 預設值設定
			data.AddRange((List<BatchFlashStockTransferCheck>)CheckDefaultSetting(dcCode, gupCode, custCode, item).Data);

			// 共用欄位格式檢核
			data.AddRange((List<BatchFlashStockTransferCheck>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, locCode, item).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<BatchFlashStockTransferCheck>)CheckCustomColumnType(dcCode, gupCode, custCode, item).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				//共用欄位資料檢核
				data.AddRange((List<BatchFlashStockTransferCheck>)CheckCommonColumnData(dcCode, gupCode, custCode, locCode, item).Data);

				//貨主自訂欄位資料檢核
				data.AddRange((List<BatchFlashStockTransferCheck>)CheckCustomColumnValue(dcCode, gupCode, custCode, item).Data);
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			
			return result;

		}

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, BatchFlashStockTransferResult item)
		{
			// 請預留方法
			ApiResult res = new ApiResult { IsSuccessed = true };
			res.Data = new List<BatchFlashStockTransferCheck>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, string locCode, BatchFlashStockTransferResult item)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			var data = new List<BatchFlashStockTransferCheck>();

			#region 檢查訂單欄位必填、最大長度
			List<string> itemIsNullList = new List<string>();
			List<ApiCkeckColumnModel> itemIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			itemCheckColumnList.ForEach(column =>
			{
				// 必填
				if (!column.Nullable)
				{
					if (!DataCheckHelper.CheckRequireColumn(item, column.Name))
						itemIsNullList.Add(column.Name);
				}

				// 最大長度
				if (column.MaxLength > 0)
				{
					if (!DataCheckHelper.CheckDataMaxLength(item, column.Name, column.MaxLength))
						itemIsExceedMaxLenthList.Add(column);
				}
			});

			// 必填訊息
			if (itemIsNullList.Any())
				data.Add(new BatchFlashStockTransferCheck { LocCode = locCode, ItemCode = item.ItemCode, MsgCode = "20117", MsgContent = string.Format(tacService.GetMsg("20117"), string.Join("、", itemIsNullList)) });

			// 最大長度訊息
			if (itemIsExceedMaxLenthList.Any())
			{
				List<string> errorMsgList = itemIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();
				string errorMsg = string.Join("、", errorMsgList);
				data.Add(new BatchFlashStockTransferCheck { LocCode = locCode, ItemCode = item.ItemCode, MsgCode = "20118", MsgContent = string.Format(tacService.GetMsg("20118"), errorMsg) });
			}
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, BatchFlashStockTransferResult item)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<BatchFlashStockTransferCheck>();
			return res;
		}

		/// <summary>
		/// 共用欄位資料檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, string locCode, BatchFlashStockTransferResult item)
		{
			ApiResult res = new ApiResult();
			List<BatchFlashStockTransferCheck> data = new List<BatchFlashStockTransferCheck>();

			//var f1903 = _f1903List.Find(o => o.ITEM_CODE == item.ItemCode);

			// 檢查品號是否存在
			//CheckItemCodeIsExist(ref data, locCode, f1903, item.ItemCode);

			// 檢核序號清單
			//CheckSnList(ref data, locCode, f1903, item);

			// 檢核調整數量
			CheckAdjQty(ref data, locCode, item);

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, BatchFlashStockTransferResult item)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<BatchFlashStockTransferCheck>();
			return res;
		}


		/// <summary>
		/// 檢核品號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="locCode"></param>
		/// <param name="f1903"></param>
		/// <param name="itemCode"></param>
		public void CheckItemCodeIsExist(ref List<BatchFlashStockTransferCheck> res, string locCode, F1903 f1903, string itemCode)
		{
			if (f1903 == null)
				res.Add(new BatchFlashStockTransferCheck { LocCode = locCode, ItemCode = itemCode, MsgCode = "20119", MsgContent = _tacService.GetMsg("20119") });
		}

		/// <summary>
		/// 檢核序號清單
		/// </summary>
		/// <param name="res"></param>
		/// <param name="locCode"></param>
		/// <param name="f1903"></param>
		/// <param name="item"></param>
		public void CheckSnList(ref List<BatchFlashStockTransferCheck> res, string locCode, F1903 f1903, BatchFlashStockTransferResult item)
		{
			if (f1903 != null)
			{
			
					if (item.SnList != null && item.SnList.Any())
					{
					var repeatSns = item.SnList.GroupBy(x => x).Select(z => new { SerialNo = z.Key, Cnt = z.Count() }).Where(x => x.Cnt > 1).Select(x => x.SerialNo).ToList();
						if (repeatSns.Any())
							res.Add(new BatchFlashStockTransferCheck { LocCode = locCode, ItemCode = f1903.ITEM_CODE, MsgCode = "20121", MsgContent = string.Format(_tacService.GetMsg("20121"), string.Join("、", repeatSns)) });


					if (_serialNoService.CommonService == null)
							_serialNoService.CommonService = _commonService;

						var serialNoRes = _serialNoService.SerialNoStatusCheck(f1903.GUP_CODE, f1903.CUST_CODE, item.SnList, "A1");
						var notPassSn = serialNoRes.Where(x => !x.Checked).ToList();
						if (notPassSn.Any())
						{
							res.AddRange(notPassSn.Select(x => new BatchFlashStockTransferCheck
							{
								LocCode = locCode,
								ItemCode = f1903.ITEM_CODE,
								MsgCode = "20074",
								MsgContent = x.Message
							}).ToList());
						}
					}
			}
		}

		/// <summary>
		/// 檢核調整數量
		/// </summary>
		/// <param name="res"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="item"></param>
		public void CheckAdjQty(ref List<BatchFlashStockTransferCheck> res, string locCode, BatchFlashStockTransferResult item)
		{
			if (item.AdjQty < 1)
				res.Add(new BatchFlashStockTransferCheck { LocCode = locCode, ItemCode = item.ItemCode, MsgCode = "20123", MsgContent = _tacService.GetMsg("20123") });
		}
		#endregion

		public ExecuteResult CreateAdjustOrder(AdjustOrderParam adjustOrderParam,WmsTransaction wmsTransaction,string adjustNo)
		{
			if (adjustOrderParam.AdjustStockDetails == null)
				return new ExecuteResult(false, "無調整明細，不可建立調整單");

			var f200101Repo = new F200101Repository(Schemas.CoreSchema, wmsTransaction);
			var f200103Repo = new F200103Repository(Schemas.CoreSchema, wmsTransaction);
			var f20010301Repo = new F20010301Repository(Schemas.CoreSchema, wmsTransaction);
			var f191303Repo = new F191303Repository(Schemas.CoreSchema, wmsTransaction);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, wmsTransaction);
			var stockService = new StockService(wmsTransaction, false);

			var addF2501List = new List<F2501>();
			var updF2501List = new List<F2501>();
			var addF200103List = new List<F200103>();
			var addF20010301List = new List<F20010301>();
			var addF191303List = new List<F191303>();
			var addStockList = new List<OrderStockChange>();
			var rmStockList = new List<OrderStockChange>();

			//var logSeq = 1;

			#region 建立調整單頭檔
			var f200101 = new F200101
			{
				DC_CODE = adjustOrderParam.DcCode,
				GUP_CODE = adjustOrderParam.GupCode,
				CUST_CODE = adjustOrderParam.CustCode,
				ADJUST_DATE = DateTime.Now.Date,
				ADJUST_NO = adjustNo,
				ADJUST_TYPE = ((int)adjustOrderParam.AdjustType).ToString(),
				SOURCE_TYPE = adjustOrderParam.SourceType,
				SOURCE_NO = adjustOrderParam.SourceNo,
				STATUS = "0",
				WORK_TYPE = adjustOrderParam.WorkType
			};
			#endregion

			#region 建立調整單明細檔、序號調整、F20010301 調整單序號刷入紀錄、庫存調整

			for (var index = 0; index < adjustOrderParam.AdjustStockDetails.Count; index++)
			{
				var x = adjustOrderParam.AdjustStockDetails[index];

				var casueMemo = "批次快速庫存移轉";
				casueMemo += x.CasueMemo;

				#region 新增調整單明細
				addF200103List.Add(new F200103
				{
					DC_CODE = adjustOrderParam.DcCode,
					GUP_CODE = adjustOrderParam.GupCode,
					CUST_CODE = adjustOrderParam.CustCode,
					ADJUST_NO = adjustNo,
					ADJUST_SEQ = index + 1,
					CAUSE = x.Cause,
					CAUSE_MEMO = casueMemo,
					WAREHOUSE_ID = x.WarehouseId,
					LOC_CODE = x.LocCode,
					ITEM_CODE = x.ItemCode,
					VALID_DATE = x.ValidDate,
					ENTER_DATE = x.EnterDate,
					MAKE_NO = x.MakeNo,
					VNR_CODE = "000000",
					BOX_CTRL_NO = x.BoxCtrlNo,
					PALLET_CTRL_NO = x.PalletCtrlNo,
					WORK_TYPE = x.WORK_TYPE,
					ADJ_QTY = x.AdjQty,
					STATUS = "0"
				});
				#endregion

				//#region 序號調整、F20010301 調整單序號刷入紀錄
				//if (x.SerialNoList != null)
				//{
				//	var dbSnList = _commonService.GetItemSerialList(adjustOrderParam.GupCode, adjustOrderParam.CustCode, x.SerialNoList);

				//	foreach (var sn in x.SerialNoList)
				//	{
				//		if (x.WORK_TYPE == "0")
				//		{
				//			var dbSn = dbSnList.FirstOrDefault(y => y.SERIAL_NO == sn);
				//			if (dbSn == null)
				//			{
				//				addF2501List.Add(new F2501
				//				{
				//					GUP_CODE = adjustOrderParam.GupCode,
				//					CUST_CODE = adjustOrderParam.CustCode,
				//					SERIAL_NO = sn,
				//					ITEM_CODE = x.ItemCode,
				//					IN_DATE = DateTime.Today,
				//					VALID_DATE = x.ValidDate,
				//					WMS_NO = adjustNo,
				//					PROCESS_NO = adjustNo,
				//					STATUS = "A1", //調入序號
				//					ORD_PROP = "J1",//內部調整
				//					ACTIVATED = "0",
				//					SEND_CUST = "0",
				//					CLIENT_IP = Current.DeviceIp
				//				});
				//			}
				//			else
				//			{
				//				if (dbSn.STATUS != "A1")
				//				{
				//					dbSn.ITEM_CODE = x.ItemCode;
				//					dbSn.IN_DATE = DateTime.Today;
				//					dbSn.VALID_DATE = x.ValidDate;
				//					dbSn.WMS_NO = adjustNo;
				//					dbSn.PROCESS_NO = adjustNo;
				//					dbSn.PO_NO = null;
				//					dbSn.STATUS = "A1";//調入序號
				//					dbSn.ORD_PROP = "J1";//內部調整
				//					dbSn.VNR_CODE = null;
				//					dbSn.CLIENT_IP = Current.DeviceIp;
				//					updF2501List.Add(dbSn);
				//				}
				//				else
				//				{
				//					return new ExecuteResult(false, string.Format("商品序號{0}已存在庫內，不可調入此序號", sn));
				//				}
				//			}
				//		}
				//		else
				//		{

				//			var dbSn = dbSnList.FirstOrDefault(y => y.SERIAL_NO == sn);
				//			if (dbSn == null)
				//			{
				//				return new ExecuteResult(false, string.Format("商品序號{0}不存在庫內，不可調出此序號", sn));
				//			}
				//			else
				//			{
				//				dbSn.IN_DATE = DateTime.Today;
				//				dbSn.WMS_NO = adjustNo;
				//				dbSn.STATUS = "C1"; //調出序號
				//				updF2501List.Add(dbSn);
				//			}
				//		}


				//		//快速移轉庫存 增加寫入F20010301 調整單序號刷入紀錄
				//		if (adjustOrderParam.AdjustType == AdjustType.FastStockTransfer)
				//		{
				//			addF20010301List.Add(new F20010301
				//			{
				//				ADJUST_NO = adjustNo,
				//				ADJUST_SEQ = index + 1,
				//				LOG_SEQ = logSeq,
				//				SERIAL_NO = sn,
				//				SERIAL_STATUS = x.WORK_TYPE == "0" ? "A1" : "C1",
				//				ISPASS = "1",
				//				DC_CODE = adjustOrderParam.DcCode,
				//				GUP_CODE = adjustOrderParam.GupCode,
				//				CUST_CODE = adjustOrderParam.CustCode
				//			});
				//			logSeq++;
				//		}
				//	}
				//}
				//#endregion

				#region 庫存異動紀錄


				if (x.WORK_TYPE == "0") // 調入
				{
					//if (f1903.BUNDLE_SERIALLOC == "1" && x.SerialNoList!=null) //序號綁儲位 依序號拆庫存
					//{
					//	addStockList.AddRange(x.SerialNoList.Select(y => new OrderStockChange
					//	{
					//		DcCode = adjustOrderParam.DcCode,
					//		GupCode = adjustOrderParam.GupCode,
					//		CustCode = adjustOrderParam.CustCode,
					//		ItemCode = x.ItemCode,
					//		LocCode = x.LocCode,
					//		VaildDate = x.ValidDate,
					//		EnterDate = x.EnterDate,
					//		BoxCtrlNo = x.BoxCtrlNo,
					//		PalletCtrlNo = x.PalletCtrlNo,
					//		VnrCode = "000000",
					//		MakeNo = x.MakeNo,
					//		WmsNo = adjustNo,
					//		SerialNo = y,
					//		Qty = 1
					//	}));
					//}
					//else
					//{
					addStockList.Add(new OrderStockChange
					{
						DcCode = adjustOrderParam.DcCode,
						GupCode = adjustOrderParam.GupCode,
						CustCode = adjustOrderParam.CustCode,
						ItemCode = x.ItemCode,
						LocCode = x.LocCode,
						VaildDate = x.ValidDate,
						EnterDate = x.EnterDate,
						BoxCtrlNo = x.BoxCtrlNo,
						PalletCtrlNo = x.PalletCtrlNo,
						VnrCode = "000000",
						MakeNo = x.MakeNo,
						WmsNo = adjustNo,
						SerialNo = "0",
						Qty = x.AdjQty
					});
				}
				//}
				else
				{
					//if (f1903.BUNDLE_SERIALLOC == "1" && x.SerialNoList != null) //序號綁儲位 依序號拆庫存
					//{
					//	rmStockList.AddRange(x.SerialNoList.Select(y => new OrderStockChange
					//	{
					//		DcCode = adjustOrderParam.DcCode,
					//		GupCode = adjustOrderParam.GupCode,
					//		CustCode = adjustOrderParam.CustCode,
					//		ItemCode = x.ItemCode,
					//		LocCode = x.LocCode,
					//		VaildDate = x.ValidDate,
					//		EnterDate = x.EnterDate,
					//		BoxCtrlNo = x.BoxCtrlNo,
					//		PalletCtrlNo = x.PalletCtrlNo,
					//		VnrCode = "000000",
					//		MakeNo = x.MakeNo,
					//		WmsNo = adjustNo,
					//		SerialNo = y,
					//		Qty = 1
					//	}));
					//}
					//else
					//{
					rmStockList.Add(new OrderStockChange
					{
						DcCode = adjustOrderParam.DcCode,
						GupCode = adjustOrderParam.GupCode,
						CustCode = adjustOrderParam.CustCode,
						ItemCode = x.ItemCode,
						LocCode = x.LocCode,
						VaildDate = x.ValidDate,
						EnterDate = x.EnterDate,
						BoxCtrlNo = x.BoxCtrlNo,
						PalletCtrlNo = x.PalletCtrlNo,
						VnrCode = "000000",
						MakeNo = x.MakeNo,
						WmsNo = adjustNo,
						SerialNo = "0",
						Qty = x.AdjQty
					});
				}
				//}

				#endregion

			}

			#endregion

			#region 庫存調整

			if (addStockList.Any())
			{
				stockService.AddStock(addStockList);
			}
			if (rmStockList.Any())
			{
				string allotBatchNo = string.Empty;
				var isPass = false;
				try
				{
					var rmItemCodes = rmStockList.Select(x => new ItemKey { DcCode = x.DcCode, GupCode = x.GupCode, CustCode = x.CustCode, ItemCode = x.ItemCode }).Distinct().ToList();
					allotBatchNo = "BJ" + DateTime.Now.ToString("yyyyMMddHHmmss");
					isPass = stockService.CheckAllotStockStatus(true, allotBatchNo, rmItemCodes);
					if (!isPass)
						return new ExecuteResult(false, "仍有程序正在配庫調整單所配庫商品，不可建立調整單");

					var res = stockService.DeductStock(rmStockList);
					if (!res.IsSuccessed)
						return res;
				}
				finally
				{
					if(isPass)
						// 更改配庫狀態為未配庫
						stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
				}
			}
			#endregion

			#region 倉庫搬動紀錄
			var group = addF200103List.GroupBy(x => new { x.WAREHOUSE_ID, x.WORK_TYPE, x.ITEM_CODE, x.CAUSE }).ToList();
			foreach (var x in group)
			{
				var casueMemo = "批次快速庫存移轉";

				addF191303List.Add(new F191303
				{
					DC_CODE = f200101.DC_CODE,
					GUP_CODE = f200101.GUP_CODE,
					CUST_CODE = f200101.CUST_CODE,
					SHIFT_WMS_NO = f200101.ADJUST_NO,
					SHIFT_TYPE = "1",
					SRC_WAREHOUSE_TYPE = x.Key.WAREHOUSE_ID.Substring(0, 1),
					SRC_WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
					TAR_WAREHOUSE_TYPE = x.Key.WAREHOUSE_ID.Substring(0, 1),
					TAR_WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
					ITEM_CODE = x.Key.ITEM_CODE,
					SHIFT_CAUSE = x.Key.CAUSE,
					SHIFT_CAUSE_MEMO = casueMemo,
					SHIFT_TIME = DateTime.Now,
					SHIFT_QTY = x.Sum(z => z.ADJ_QTY),
					PROC_FLAG = "0"
				});
			}
			#endregion


			// 更新儲位容積
			//SharedService.UpdateUsedVolumnByLocCodes(f200101.DC_CODE, f200101.GUP_CODE, f200101.CUST_CODE, addF200103List.Select(x => x.LOC_CODE).Distinct());


			#region db commit

			f200101Repo.Add(f200101);

			if (addF2501List.Any())
				f2501Repo.BulkInsert(addF2501List);

			if (updF2501List.Any())
				f2501Repo.BulkUpdate(updF2501List);

			if (addF200103List.Any())
				f200103Repo.BulkInsert(addF200103List);

			if (addF20010301List.Any())
				f20010301Repo.BulkInsert(addF20010301List);

			if (addF191303List.Any())
				f191303Repo.BulkInsert(addF191303List);


			stockService.SaveChange();

			#endregion

			return new ExecuteResult(true);
		}

	}
}
