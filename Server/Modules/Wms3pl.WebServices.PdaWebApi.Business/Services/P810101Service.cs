using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810101Service
	{
		private WmsTransaction _wmsTransation;
		public P810101Service(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		/// <summary>
		/// 商品主檔同步
		/// </summary>
		/// <param name="getBatchItemReq"></param>
		/// <returns></returns>
		public ApiResult GetBatchItem(GetBatchItemReq getBatchItemReq)
		{
			P81Service p81Service = new P81Service();
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			GetBatchItemRes getBatchItemRes = new GetBatchItemRes();
			ApiResult apiResult = new ApiResult();

			// 帳號檢核
			IQueryable<F1924> checkAcc = p81Service.CheckAcc(getBatchItemReq.AccNo);

			// 檢核人員功能權限
			int checkAccFunction = p81Service.CheckAccFunction(getBatchItemReq.FuncNo, getBatchItemReq.AccNo);

			// 檢核人員貨主權限
			int checkAccCustCode = p81Service.CheckAccCustCode(getBatchItemReq.CustNo, getBatchItemReq.AccNo);

			// 取得業主編號
			var gupCode = p81Service.GetGupCode(getBatchItemReq.CustNo);

			if (string.IsNullOrWhiteSpace(getBatchItemReq.FuncNo) ||
					string.IsNullOrWhiteSpace(getBatchItemReq.AccNo) ||
					string.IsNullOrWhiteSpace(getBatchItemReq.CustNo) ||
					getBatchItemReq.BatchSkipNum < 0 ||
					checkAcc.Count() == 0 ||
					checkAccFunction == 0 ||
					checkAccCustCode == 0)
			{
				apiResult.IsSuccessed = false;
				apiResult.MsgCode = "20069";
				apiResult.MsgContent = p81Service.GetMsg("20069");
			}
			else
			{
				if (!getBatchItemReq.ItemNoList.Any())
				{
					// 資料處理1
					var getDataPprocessing1 = f1903Repo.GetDataPprocessing1(getBatchItemReq.CustNo, gupCode, getBatchItemReq.LastAsyncDate, getBatchItemReq.BatchSkipNum);

					List<ItemList> itemList = new List<ItemList>();

					if (getDataPprocessing1.Any())
					{
						foreach (var item in getDataPprocessing1)
						{
							ItemList itemListObject = new ItemList
							{
								CustNo = item.CustNo,
								ItemNo = item.ItemNo,
								ItemName = item.ItemName,
								Unit = p81Service.GetItemUnit(item.Unit),
								ItemSize = item.ItemSize,
								BoxQty = item.BoxQty,
								ItemColor = item.ItemColor,
								ItemSpec = item.ItemSpec,
								ItemLength = item.ItemLength,
								ItemWidth = item.ItemWidth,
								ItemHeight = item.ItemHeight,
								ItemWeight = item.ItemWeight,
								SnType = item.SnType,
								Barcode1 = item.Barcode1,
								Barcode2 = item.Barcode2,
								Barcode3 = item.Barcode3
							};
							itemList.Add(itemListObject);
						}
						getBatchItemRes.BatchLastDate = getDataPprocessing1.Last().UpdDate != null ? getDataPprocessing1.Last().UpdDate.Value : getDataPprocessing1.Last().CrtDate;
					}
					else
					{
						getBatchItemRes.BatchLastDate = getBatchItemReq.LastAsyncDate;
					}

					getBatchItemRes.BatchTotal = (getBatchItemReq.BatchSkipNum > 1000 ? getBatchItemReq.BatchSkipNum - 1 : getBatchItemReq.BatchSkipNum) + getDataPprocessing1.Count();

					getBatchItemRes.IsLastBatch = getDataPprocessing1.Count() < 1000 ? true : false;
					getBatchItemRes.ItemList = itemList;
				}
				else
				{
					// 資料處理2
					var getDataPprocessing2 = f1903Repo.GetDataPprocessing2(getBatchItemReq.CustNo, gupCode, getBatchItemReq.ItemNoList);

					List<ItemList> itemList = new List<ItemList>();
					foreach (var item in getDataPprocessing2)
					{
						ItemList itemListObject = new ItemList
						{
							CustNo = item.CustNo,
							ItemNo = item.ItemNo,
							ItemName = item.ItemName,
							Unit = p81Service.GetItemUnit(item.Unit),
							ItemSize = item.ItemSize,
							BoxQty = item.BoxQty,
							ItemColor = item.ItemColor,
							ItemSpec = item.ItemSpec,
							ItemLength = item.ItemLength,
							ItemWidth = item.ItemWidth,
							ItemHeight = item.ItemHeight,
							ItemWeight = item.ItemWeight,
							SnType = item.SnType,
							Barcode1 = item.Barcode1,
							Barcode2 = item.Barcode2,
							Barcode3 = item.Barcode3,
						};
						itemList.Add(itemListObject);
					}

					getBatchItemRes.BatchTotal = getDataPprocessing2.Count();
					getBatchItemRes.BatchLastDate = getBatchItemReq.LastAsyncDate;
					getBatchItemRes.IsLastBatch = true;
					getBatchItemRes.ItemList = itemList;
				}
				apiResult.IsSuccessed = true;
				apiResult.MsgCode = "10001";
				apiResult.MsgContent = "取得資料成功";
				apiResult.Data = getBatchItemRes;
			}
			return apiResult;
		}

		/// <summary>
		/// 商品序號主檔同步
		/// </summary>
		/// <param name="getBatchItemSerialReq"></param>
		/// <returns></returns>
		public ApiResult GetBatchItemSerial(GetBatchItemSerialReq getBatchItemSerialReq)
		{
			P81Service p81Service = new P81Service();
			var f2501Repo = new F2501Repository(Schemas.CoreSchema);
			GetBatchItemSerialRes getBatchItemSerialRes = new GetBatchItemSerialRes();
			ApiResult apiResult = new ApiResult();

			// 帳號檢核
			IQueryable<F1924> checkAcc = p81Service.CheckAcc(getBatchItemSerialReq.AccNo);

			// 檢核人員功能權限
			int checkAccFunction = p81Service.CheckAccFunction(getBatchItemSerialReq.FuncNo, getBatchItemSerialReq.AccNo);

			// 檢核人員貨主權限
			int checkAccCustCode = p81Service.CheckAccCustCode(getBatchItemSerialReq.CustNo, getBatchItemSerialReq.AccNo);

			// 取得業主編號
			var getGupCode = p81Service.GetGupCode(getBatchItemSerialReq.CustNo);

			// 驗證失敗回傳
			if (string.IsNullOrWhiteSpace(getBatchItemSerialReq.FuncNo) ||
					string.IsNullOrWhiteSpace(getBatchItemSerialReq.AccNo) ||
					string.IsNullOrWhiteSpace(getBatchItemSerialReq.CustNo) ||
					getBatchItemSerialReq.BatchSkipNum < 0 ||
					checkAcc.Count() == 0 ||
					checkAccFunction == 0 ||
					checkAccCustCode == 0)
			{
				apiResult.IsSuccessed = false;
				apiResult.MsgCode = "20069";
				apiResult.MsgContent = p81Service.GetMsg("20069");
			}
			else
			{
				if (!getBatchItemSerialReq.SnList.Any())
				{
					// 資料處理1
					var getDataPprocessing1 = f2501Repo.GetDataPprocessing1(getBatchItemSerialReq.CustNo, getGupCode, getBatchItemSerialReq.LastAsyncDate, getBatchItemSerialReq.BatchSkipNum);

					List<ItemSnList> itemSnList = new List<ItemSnList>();
					if (getDataPprocessing1.Any())
					{
						foreach (var item in getDataPprocessing1)
						{
							ItemSnList itemSnListObject = new ItemSnList
							{
								CustNo = item.CustNo,
								ItemNo = item.ItemNo,
								Sn = item.Sn,
								ValidDate = item.ValidDate,
								Status = item.Status
							};
							itemSnList.Add(itemSnListObject);
						}

						getBatchItemSerialRes.BatchLastDate = getDataPprocessing1.Last().UpdDate != null ? getDataPprocessing1.Last().UpdDate.Value : getDataPprocessing1.Last().CrtDate;
					}
					else
					{
						getBatchItemSerialRes.BatchLastDate = getBatchItemSerialReq.LastAsyncDate;
					}

					getBatchItemSerialRes.BatchTotal = (getBatchItemSerialReq.BatchSkipNum > 1000 ? getBatchItemSerialReq.BatchSkipNum - 1 : getBatchItemSerialReq.BatchSkipNum) + getDataPprocessing1.Count();
					getBatchItemSerialRes.IsLastBatch = getDataPprocessing1.Count() < 1000 ? true : false;
					getBatchItemSerialRes.ItemSnList = itemSnList;
				}
				else
				{
					// 資料處理2
					var getDataPprocessing2 = f2501Repo.GetDataPprocessing2(getBatchItemSerialReq.CustNo, getGupCode, getBatchItemSerialReq.SnList);

					List<ItemSnList> itemSnList = new List<ItemSnList>();
					foreach (var item in getDataPprocessing2)
					{
						ItemSnList itemSnListObject = new ItemSnList
						{
							CustNo = item.CustNo,
							ItemNo = item.ItemNo,
							Sn = item.Sn,
							ValidDate = item.ValidDate,
							Status = item.Status
						};
						itemSnList.Add(itemSnListObject);
					}

					getBatchItemSerialRes.BatchTotal = getDataPprocessing2.Count();
					getBatchItemSerialRes.BatchLastDate = getBatchItemSerialReq.LastAsyncDate;
					getBatchItemSerialRes.IsLastBatch = true;
					getBatchItemSerialRes.ItemSnList = itemSnList;
				}
				apiResult.IsSuccessed = true;
				apiResult.MsgCode = "10001";
				apiResult.MsgContent = p81Service.GetMsg("10001");
				apiResult.Data = getBatchItemSerialRes;
			}
			return apiResult;
		}

		/// <summary>
		/// 依據商品搜尋條件至找出商品
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <param name="allocNo"></param>
		/// <returns></returns>
		public ApiResult GetItemByCondition(GetItemByConditionReq req, string gupCode)
		{
			var res = new GetItemByConditionRes();
			var p81Service = new P81Service();

			// 帳號檢核
			IQueryable<F1924> checkAcc = p81Service.CheckAcc(req.AccNo);

			// 檢核人員貨主權限
			int checkAccCustCode = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 驗證失敗回傳
			if (string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					string.IsNullOrWhiteSpace(req.Condition) ||
					string.IsNullOrWhiteSpace(req.WmsNo) ||
					checkAcc.Count() == 0 ||
					checkAccCustCode == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			var itemService = new ItemService();
			var commonService = new CommonService();
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);

			F2501 f2501 = null;
			var itemCodes = itemService.FindItems(gupCode, req.CustNo, req.Condition, ref f2501);
			if (f2501 == null)
			{
				#region 代表該條件(req.Condition)不是序號(可能是品號、國條1~3、甚至找不到該條件資料)
				if (itemCodes.Any())
				{
					res.ItemList = commonService.GetProductList(gupCode, req.CustNo, itemCodes).Select(f1903 => new GetItemByConditionItem
					{
						ItemNo = f1903.ITEM_CODE,
						SnType = f1903.BUNDLE_SERIALLOC == "1" ? "2" : f1903.BUNDLE_SERIALNO == "1" ? "1" : "0" // 序號綁儲位(2) 序號商品(1) 一般商品(0)
					}).OrderBy(x => x.SnType).ToList();

					res.ConditionType = "1";
				}
				#endregion
			}
			else
			{
				#region 代表該條件(req.Condition)是序號
				// 序號一定只有一筆品號
				var itemCode = itemCodes.First();

				// 取得該商品用以找出是否為序號綁儲位(2) 序號商品(1) 一般商品(0)
				var f1903 = f1903Repo.Find(o =>
				o.GUP_CODE == gupCode
				&& o.CUST_CODE == req.CustNo
				&& o.ITEM_CODE == itemCode);

				res.ItemList = new List<GetItemByConditionItem>
				{
					new GetItemByConditionItem
					{
						ItemNo = itemCode,
						SnType = f1903.BUNDLE_SERIALLOC == "1" ? "2" : f1903.BUNDLE_SERIALNO == "1" ? "1" : "0" // 序號綁儲位(2) 序號商品(1) 一般商品(0)
					}
				};

				// 如果是序號綁儲位必須要在該調撥單、揀貨單內
				if (f1903.BUNDLE_SERIALLOC == "1")
				{
					if (req.WmsNo.StartsWith("T"))
					{
						var f151002Repo = new F151002Repository(Schemas.CoreSchema);
						var f151002 = f151002Repo.Find(o =>
						o.DC_CODE == req.DcNo &&
						o.GUP_CODE == gupCode &&
						o.CUST_CODE == req.CustNo &&
						o.ALLOCATION_NO == req.WmsNo &&
						o.SERIAL_NO == f2501.SERIAL_NO);

						if (f151002 == null)
							res.ItemList = new List<GetItemByConditionItem>();
					}
					else if (req.WmsNo.StartsWith("P"))
					{
						var f051202Repo = new F051202Repository(Schemas.CoreSchema);
						var f051202 = f051202Repo.Find(o =>
						o.DC_CODE == req.DcNo &&
						o.GUP_CODE == gupCode &&
						o.CUST_CODE == req.CustNo &&
						o.PICK_ORD_NO == req.WmsNo &&
						o.SERIAL_NO == f2501.SERIAL_NO);

						if (f051202 == null)
							res.ItemList = new List<GetItemByConditionItem>();
					}
				}
				#endregion

				res.ConditionType = "2";
				res.SnStatus = f2501.STATUS;
			}

			return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001"), Data = res }; ;
		}
	}
}
