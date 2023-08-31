using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
	/// <summary>
	/// 快速移轉庫存調整單
	/// </summary>
	public class CommonFlashStockTransferService
	{
		public CommonFlashStockTransferService(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		#region Private Property
		private List<ApiCkeckColumnModel> itemCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ItemCode",  Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "AdjQty",    Type = typeof(int),      MaxLength = 9 , Nullable = false },
			new ApiCkeckColumnModel{  Name = "ValidDate", Type = typeof(DateTime) , Nullable = false },
			new ApiCkeckColumnModel{  Name = "MakeNo",    Type = typeof(string),   MaxLength = 40, Nullable = false },
			new ApiCkeckColumnModel{  Name = "SnList",    Type = typeof(string[]) }
		};

		private WmsTransaction _wmsTransation;
		/// <summary>
		/// 商品資料清單
		/// </summary>
		private List<CommonProduct> _f1903List;
		#endregion

		#region 主邏輯
		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(FlashStockTransferDataReq req)
		{
			var commonService = new CommonService();
			var ctaService = new CheckTransApiService();
			var tacService = new TransApiBaseService();
			var csService = new CheckStockService();
			var f191304Repo = new F191304Repository(Schemas.CoreSchema);
			var res = new ApiResult { IsSuccessed = true };

			#region 資料檢核
			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckCustCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核儲位編號 必填、是否存在
			ctaService.CheckLocCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 交易編號(不可重複)
			ctaService.CheckTransactionNo(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核傳入物件
			ctaService.CheckResult(ref res, req);
			if (!res.IsSuccessed)
				return res;
			#endregion

			#region 紀錄接收到的交易編號
			var isAdd = false;
			var f191304Res = f191304Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
			() =>
			{
				var lockF191304 = f191304Repo.LockF191304();
				var f191304 = f191304Repo.Find(o => o.DC_CODE == req.DcCode && o.TRANSACTION_NO == req.TransactionNo, isForUpdate: true, isByCache: false);
				if (f191304 == null)
				{
					f191304 = new F191304 { DC_CODE = req.DcCode, TRANSACTION_NO = req.TransactionNo };
					f191304Repo.Add(f191304);
					isAdd = true;
				}
				else
				{
					f191304 = null; // 代表F191304已存在資料
				}
				return f191304;
			});
			if (f191304Res == null)// 代表F191304已存在資料
				return new ApiResult { IsSuccessed = false, MsgCode = "20009", MsgContent = tacService.GetMsg("20009") };
			#endregion

			#region 資料處理
			try
			{
				// 取得業主編號
				string gupCode = commonService.GetGupCode(req.CustCode);

        //把序號內容轉大寫
        foreach (var a in req.Result)
          for (int i = 0; i < a.SnList.Count(); i++)
            a.SnList[i] = a.SnList[i]?.ToUpper();

        res = ProcessApiDatas(req.DcCode, gupCode, req.CustCode, req.LocCode, req.TransactionNo, req.Result);
			}
			catch (Exception ex)
			{
				if (isAdd)
					f191304Repo.DelF191304ByKey(req.DcCode, req.TransactionNo);
				throw ex;
			}
			#endregion

			return res;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, string locCode, string transactionNo, List<FlashStockTransferDataResult> items)
		{
			#region 變數
			var tacService = new TransApiBaseService();
			var sharedService = new SharedService(_wmsTransation);
			var commonService = new CommonService();
			var f191304Repo = new F191304Repository(Schemas.CoreSchema);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var res = new ApiResult();
			#endregion

			#region Private Property
			var itemCodes = items.Where(x => !string.IsNullOrWhiteSpace(x.ItemCode)).Select(x => x.ItemCode).Distinct().ToList();

			// 商品資料清單
			_f1903List = commonService.GetProductList(gupCode, custCode, itemCodes);
			#endregion

			#region 檢核
			List<FlashStockTransferData> data = new List<FlashStockTransferData>();

			// 檢查該儲位是否有現有庫存
			//var isExist = f1913Repo.LocCodeIsExistStock(dcCode, locCode);
			//if (isExist)
			//{
			//	f191304Repo.DelF191304ByKey(dcCode, locCode);
			//	return new ApiResult { IsSuccessed = false, MsgCode = "20010", MsgContent = tacService.GetMsg("20010") };
			//}

			items.ForEach(item =>
			{
				var resl = CheckItem(dcCode, gupCode, custCode, locCode, item);

				if (!resl.IsSuccessed)
					data.AddRange((List<FlashStockTransferData>)resl.Data);
			});
			#endregion

			#region 建立調整單
			if (!data.Any())
			{
				var today = DateTime.Today;

				// 儲位資料
				var f1912 = commonService.GetLoc(dcCode, locCode);

				var adjustStockDetailList = items.Select(x => new AdjustStockDetail
				{
					LocCode = locCode,
					ItemCode = x.ItemCode,
					ValidDate = x.ValidDate.Value,
					EnterDate = today,
					MakeNo = x.MakeNo,
					PalletCtrlNo = "0",
					BoxCtrlNo = "0",
					WarehouseId = f1912.WAREHOUSE_ID,
					Cause = "900",
					CasueMemo = "交易編號：" + transactionNo,
					WORK_TYPE = "0",
					AdjQty = x.AdjQty,
					SerialNoList = x.SnList
				}).ToList();

				var adjustOrderParam = new AdjustOrderParam
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					AdjustType = AdjustType.FastStockTransfer,
					CheckSerialItem = true,
					WorkType = null,
					AdjustStockDetails = adjustStockDetailList,

				};
				var adjustOrderSerivce = new AdjustOrderService(_wmsTransation);
				var result = adjustOrderSerivce.CreateAdjustOrder(adjustOrderParam);
				if(result.IsSuccessed)
				{
					_wmsTransation.Complete();
				}
				else
				{
					data.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = null, MsgCode = "99999", MsgContent = result.Message });
				}
			}
			else
			{
				f191304Repo.DelF191304ByKey(dcCode, transactionNo);
			}
			#endregion

			if (data.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21002", MsgContent = tacService.GetMsg("21002"), Data = data };
			else
				return new ApiResult { IsSuccessed = true, MsgCode = "21001", MsgContent = tacService.GetMsg("21001") };
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
		public ApiResult CheckItem(string dcCode, string gupCode, string custCode, string locCode, FlashStockTransferDataResult item)
		{
			ApiResult result = new ApiResult();
			List<FlashStockTransferData> data = new List<FlashStockTransferData>();

			// 預設值設定
			data.AddRange((List<FlashStockTransferData>)CheckDefaultSetting(dcCode, gupCode, custCode, item).Data);

			// 共用欄位格式檢核
			data.AddRange((List<FlashStockTransferData>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, locCode, item).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<FlashStockTransferData>)CheckCustomColumnType(dcCode, gupCode, custCode, item).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<FlashStockTransferData>)CheckCommonColumnData(dcCode, gupCode, custCode, locCode, item).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<FlashStockTransferData>)CheckCustomColumnValue(dcCode, gupCode, custCode, item).Data);
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;

		}
		#endregion

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, FlashStockTransferDataResult item)
		{
			// 請預留方法
			ApiResult res = new ApiResult { IsSuccessed = true };
			res.Data = new List<FlashStockTransferData>();
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
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, string locCode, FlashStockTransferDataResult item)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<FlashStockTransferData> data = new List<FlashStockTransferData>();

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
				data.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = item.ItemCode, MsgCode = "20117", MsgContent = string.Format(tacService.GetMsg("20117"), string.Join("、", itemIsNullList)) });

			// 最大長度訊息
			if (itemIsExceedMaxLenthList.Any())
			{
				List<string> errorMsgList = itemIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();
				string errorMsg = string.Join("、", errorMsgList);
				data.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = item.ItemCode, MsgCode = "20118", MsgContent = string.Format(tacService.GetMsg("20118"), errorMsg) });
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
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, FlashStockTransferDataResult item)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<FlashStockTransferData>();
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
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, string locCode, FlashStockTransferDataResult item)
		{
			CheckFlashStockTransferService cfService = new CheckFlashStockTransferService();
			ApiResult res = new ApiResult();
			List<FlashStockTransferData> data = new List<FlashStockTransferData>();

			var f1903 = _f1903List.Find(o => o.ITEM_CODE == item.ItemCode);

			// 檢查品號是否存在
			cfService.CheckItemCodeIsExist(ref data, locCode, f1903, item.ItemCode);

			// 檢核序號清單
			cfService.CheckSnList(ref data, locCode, f1903, item);

			// 檢核調整數量
			cfService.CheckAdjQty(ref data, locCode, item);

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
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, FlashStockTransferDataResult item)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<FlashStockTransferData>();
			return res;
		}
		#endregion
	}
}
