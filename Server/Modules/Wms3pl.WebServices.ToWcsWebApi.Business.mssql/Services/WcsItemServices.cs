using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;
using Wms3pl.WebServices.Shared.WcsServices;


namespace Wms3pl.WebServices.ToWcsWebApi.Business.mssql.Services
{
	public class WcsItemServices : WcsBaseService
	{
		
		#region 商品主檔同步
		/// <summary>
		/// 商品主檔同步
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ItemAsync(WcsExportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
      WcsSetting.DcCode = req.DcCode;

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSCH_ITEM, req.DcCode, req.GupCode, req.CustCode, "WcsItemSchedule", req, () =>
			{
				// 取得物流中心服務貨主檔
				var gupCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode)
				.Select(x => new { x.GUP_CODE, x.CUST_CODE }).Distinct().ToList(); // 因為該送出資料不需要DC所以取得GUP_CODE、CUST_CODE來送

				gupCustList.ForEach(item =>
				{
					var result = ExportItemAsyncResults(item.GUP_CODE, item.CUST_CODE);
					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// 商品主檔同步
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public ApiResult ExportItemAsyncResults(string gupCode, string custCode)
		{
			#region 變數設定
			// 商品主檔Url
			var data = new List<ApiResponse>();
			var totalCnt = 0;
			var failCnt = 0;
			var f1901Repo = new F1901Repository(Schemas.CoreSchema);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1905Repo = new F1905Repository(Schemas.CoreSchema);
			var tacService = new TransApiBaseService();
			var f1903AsyncRepo = new F1903_ASYNCRepository(Schemas.CoreSchema);
			#endregion

			#region 主要邏輯
			var dcCodeList = f1901Repo.GetAllDcCodes().ToList();

			//取得商品主檔同步紀錄
			var f1903AsyncList = f1903AsyncRepo.AsForUpdate().GetDatas(gupCode, custCode, GetMidApiMisMax()).ToList();

			//取得每個商品最新寫入的一筆資料
			var asyncList = f1903AsyncList.GroupBy(g => g.ITEM_CODE).Select(x => x.OrderByDescending(o => o.CRT_DATE).FirstOrDefault()).ToList();

			//取出的記錄排除最新要更新的資料，將重複要取消資料的IS_ASYNC壓為9並更新
			var cancelAsyncList = f1903AsyncList.Except(asyncList).ToList();
			cancelAsyncList.ForEach(f => f.IS_ASYNC = "9");
			if (cancelAsyncList.Any())
				f1903AsyncRepo.BulkUpdate(cancelAsyncList);

			// 取得商品主檔資料
			var f1903s = commonService.GetProductList(gupCode, custCode, asyncList.Select(s => s.ITEM_CODE).ToList()).ToList();
			var f1905s = commonService.GetProductSizeList(gupCode, custCode, f1903s.Select(x => x.ITEM_CODE).ToList());

			if (f1903s.Any() && f1905s.Any())
			{
				#region 取得商品主檔資料
				var itemList = (from A in f1903s
								join B in f1905s
								on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
								select new
								{
									F1903 = A,
									Req = new WcsItemSkuModel
									{
										SkuCode = A.ITEM_CODE,
										SkuName = A.ITEM_NAME,
										VendorCode = A.VNR_CODE,
										BarCodeList = (new string[] { A.EAN_CODE1, A.EAN_CODE2, A.EAN_CODE3 }).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray(),
										OwnerSkuId = A.CUST_ITEM_CODE,
										SkuType = A.LTYPE,
										PicUrl = "",
										SkuSpec = A.ITEM_SPEC,
										SkuLength = B.PACK_LENGTH,
										SkuWidth = B.PACK_WIDTH,
										SkuHeight = B.PACK_HIGHT,
										SkuWeight = B.PACK_WEIGHT,
										ShelfLife = A.SAVE_DAY,
										HasExpiry = string.IsNullOrWhiteSpace(A.NEED_EXPIRED) ? 0 : Convert.ToInt32(A.NEED_EXPIRED),
										HasSerial = A.BUNDLE_SERIALNO == "1" ? 1 : 0
									}
								}).OrderBy(x => (x.F1903.UPD_DATE == null ? x.F1903.CRT_DATE : x.F1903.UPD_DATE)).ToList();
				#endregion

				if (itemList.Any())
				{
					totalCnt = itemList.Count;
					// 拆分F1903_ASYNC.IS_ASYNC = N 及 拆分F1903_ASYNC.IS_ASYNC = F
					var failitemList = itemList.Where(x => asyncList.Where(w => w.IS_ASYNC == "F").Select(s => s.ITEM_CODE).Contains(x.F1903.ITEM_CODE)).ToList();
					itemList = itemList.Except(failitemList).ToList();
					var failAsyncList = asyncList.Where(x => x.IS_ASYNC == "F").ToList();
					asyncList = asyncList.Except(failAsyncList).ToList();

					// 分批處理 F1903.IS_ASYNC<> F
					failCnt += CallItemAsyncApiByBatch(gupCode, custCode, itemList.Select(x => x.Req).ToList(), dcCodeList, WcsSetting.ItemMaxCnt, asyncList);

					// 逐筆處理F1903.IS_ASYNC = F
					failCnt += CallItemAsyncApiByBatch(gupCode, custCode, failitemList.Select(x => x.Req).ToList(), dcCodeList, 1, failAsyncList);
				}
			}
			#endregion

			return new ApiResult
			{
				IsSuccessed = true,
				MsgCode = "10005",
				MsgContent = string.Format(tacService.GetMsg("10005"),
					"商品主檔同步", totalCnt - failCnt, failCnt, totalCnt)
			};
		}

		/// <summary>
		/// 批次呼叫商品資訊同步API
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemList"></param>
		/// <param name="dcCodeList"></param>
		/// <param name="itemMaxCnt"></param>
		/// <returns></returns>
		private int CallItemAsyncApiByBatch(string gupCode, string custCode, List<WcsItemSkuModel> itemList, List<string> dcCodeList, int itemMaxCnt, List<F1903_ASYNC> f1903Asyncs)
		{
			var f1903AsyncRepo = new F1903_ASYNCRepository(Schemas.CoreSchema);
			var f0093Repo = new F0093Repository(Schemas.CoreSchema);
			var tacService = new TransApiBaseService();
			var failCnt = 0;
			#region 商品主檔資料分批次(每N筆一個批次)
			int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemList.Count()) / itemMaxCnt));

			for (int i = 0; i < index; i++)
			{
				var currDatas = itemList.Skip(i * itemMaxCnt).Take(itemMaxCnt).ToList();

				var currReq = new WcsItemReq { OwnerCode = custCode, SkuTotal = currDatas.Count, SkuList = currDatas.ToList() };

				var lmsApiResList = new Dictionary<string, List<ApiResult>>();

				var batchNo = DateTime.Now.ToString("yyyyMMddHHmmss");

				foreach (var dcCode in dcCodeList)
				{
					var list = new List<ApiResult>();
					var url = $"v1/{dcCode}/ALL/Item";
          WcsSetting.DcCode = dcCode;

          list.Add(ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_ITEM, dcCode, gupCode, custCode, "WcsItemResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, BatchNo = batchNo, WcsData = isSaveWcsData ? currReq : null }, () =>
					{
						ApiResult result = new ApiResult { IsSuccessed = false };

#if (DEBUG)
						result = WcsApiFuncTest(currReq, "ItemAsync");
#else
						result = WcsApiFunc(currReq, url);
#endif

						foreach (var currData in currDatas)
						{
							list.Add(ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_ITEM, dcCode, gupCode, custCode, "WcsItemByRecord", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, BatchNo = batchNo, SKU = currData }, () =>
							{
								return result;
							}, false));
						}
						return result;
					}, false));

					lmsApiResList.Add(dcCode, list);
				}

				List<string> itemCodes = currDatas.Select(x => x.SkuCode).ToList();
				var isAsync = "Y";
				var errorResList = lmsApiResList.Where(x => x.Value.Any(y => !y.IsSuccessed)).ToList();
				var f0093List = new List<F0093>();
				var errorItemCodeList = new List<string>();
				List<F1903_ASYNC> currentF1903AsyncList = f1903Asyncs.Where(x => itemCodes.Contains(x.ITEM_CODE)).ToList();

				if (errorResList.Any())
				{
					foreach (var errorRes in errorResList)
					{
						var errorList = errorRes.Value.Where(x => !x.IsSuccessed).ToList();
						foreach (var error in errorList)
						{
							try
							{
								if (error.Data != null)
								{
									var errData = JsonConvert.DeserializeObject<List<WcsItemResData>>(JsonConvert.SerializeObject(error.Data));
									var errorItemCodes = errData.Select(x => x.SkuCode).ToList();
									f0093List.AddRange(errorItemCodes.Select(x => new F0093
									{
										DC_CODE = errorRes.Key,
										GUP_CODE = gupCode,
										CUST_CODE = custCode,
										ITEM_CODE = x,
										STATUS = "0",
										MSG_NO = "API20126",
										MSG_CONTENT = string.Format(tacService.GetMsg("20126"), x)
									}));
									errorItemCodeList.AddRange(errorItemCodes);
								}
							}
							catch (Exception ex)
							{
								f0093List.AddRange(itemCodes.Select(x => new F0093
								{
									DC_CODE = errorRes.Key,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									ITEM_CODE = x,
									STATUS = "0",
									MSG_NO = "API20126",
									MSG_CONTENT = string.Format(tacService.GetMsg("20126"), x)
								}));
								errorItemCodeList.AddRange(itemCodes);
							}

						}
					}
					errorItemCodeList = errorItemCodeList.Distinct().ToList();
					itemCodes = itemCodes.Except(errorItemCodeList).ToList();

					var errorF1903AsyncList = currentF1903AsyncList.Where(x => errorItemCodeList.Contains(x.ITEM_CODE)).ToList();
					currentF1903AsyncList = currentF1903AsyncList.Except(errorF1903AsyncList).ToList();

					if (errorF1903AsyncList.Any())
					{
						failCnt += errorF1903AsyncList.Count;
						// 將回傳失敗的商品IS_ASYNC更新為F
						f1903AsyncRepo.UpdateIsAsync("F", batchNo, gupCode, custCode, errorF1903AsyncList.Select(s => s.ID).ToList());
					}
					else
					{
						//若沒有回傳失敗的商品，則將所有商品IS_ASYNC更新成F
						isAsync = "F";
					}
				}
				if (currentF1903AsyncList.Any())
					f1903AsyncRepo.UpdateIsAsync(isAsync, batchNo, gupCode, custCode, currentF1903AsyncList.Select(s => s.ID).ToList());

				if (f0093List.Any())
				{
					f0093Repo.BulkInsert(f0093List);
				}
			}
			return failCnt;
			#endregion
		}
		#endregion

		#region 商品序號同步
		/// <summary>
		/// 商品序號主檔
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ItemSnAsync(WcsExportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
      WcsSetting.DcCode = req.DcCode;

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSCH_ITEMSN, req.DcCode, req.GupCode, req.CustCode, "WcsItemSnSchedule", req, () =>
			{
				// 取得物流中心服務貨主檔
				var gupCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode)
				.Select(x => new { x.GUP_CODE, x.CUST_CODE }).Distinct().ToList(); // 因為該送出資料不需要DC所以取得GUP_CODE、CUST_CODE來送

				gupCustList.ForEach(item =>
				{
					var result = ExportItemSnAsyncResults(item.GUP_CODE, item.CUST_CODE);
					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $" GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;

		}

		public ApiResult ExportItemSnAsyncResults(string gupCode, string custCode)
    {

      ApiResult res = new ApiResult { IsSuccessed = true };
      List<ApiResponse> data = new List<ApiResponse>();
      var totalCnt = 0;
      var failCnt = 0;
      var tacService = new TransApiBaseService();
      var f2501Repo = new F2501Repository(Schemas.CoreSchema);
      //var f0003Repo = new F0003Repository(Schemas.CoreSchema);
      var f1901Repo = new F1901Repository(Schemas.CoreSchema);

      #region 主要邏輯
      var dcCodeList = f1901Repo.GetAllDcCode().ToList();
      //var f0003 = f0003Repo.AsForUpdate().Find(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00" && o.AP_NAME == "ItemSnAsyncLastTime");
      //var ItemSnAsyncLastTime = Convert.ToDateTime(f0003.SYS_PATH);

      var f2501s = f2501Repo.GetiItemSnData(gupCode, custCode);

      if (f2501s.Any())
      {
        #region 取得商品序號資料
        var itemSnList = (from A in f2501s
                          select new
                          {
                            F2501 = A,
                            Req = new WcsSnSkuModel
                            {
                              SkuCode = A.ITEM_CODE,
                              SnCode = A.SERIAL_NO
                            }
                          }).OrderBy(x => (x.F2501.UPD_DATE == null ? x.F2501.CRT_DATE : x.F2501.UPD_DATE)).ToList();
        #endregion



        //20220324 add

        #region 取得最大更新/建立日期
        var maxDatetime = (from A in f2501s
                           let max = Max(A.CRT_DATE, A.UPD_DATE)
                           select max).Max();
        #endregion
        totalCnt = itemSnList.Count;
        // F2501.IS_ASYNC =N,Y及F2501.IS_ASYNC =F

        var failItemSnList = itemSnList.Where(x => x.F2501.IS_ASYNC == "F").ToList();
        itemSnList = itemSnList.Except(failItemSnList).ToList();

        // 分批處理 F1903.IS_ASYNC<> F
        failCnt += CallItemSnAsyncApiByBatch(gupCode, custCode, itemSnList.Select(x => x.Req).ToList(), dcCodeList, WcsSetting.ItemMaxCnt);

        // 逐筆處理F1903.IS_ASYNC = F
        failCnt += CallItemSnAsyncApiByBatch(gupCode, custCode, failItemSnList.Select(x => x.Req).ToList(), dcCodeList, 1);


        #region 更新系統設定最後一次商品同步時間
        //f0003.SYS_PATH = Convert.ToDateTime(maxDatetime).ToString("yyyy/MM/dd HH:mm:ss");
        //f0003Repo.Update(f0003);
        #endregion
        //20220324 add

        #region 每100筆送序號商品資料  mark
//        if (itemSnList.Any())
//        {
//          totalCnt = itemSnList.Count;

//          //#region 取得最大更新/建立日期
//          //var maxDatetime = (from A in f2501s
//          //                   let max = Max(A.CRT_DATE, A.UPD_DATE)
//          //                   select max).Max();
//          //#endregion



//          int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemSnList.Count()) / WcsSetting.ItemMaxCnt));

//          for (int i = 0; i < index; i++)
//          {
//            var currDatas = itemSnList.Skip(i * WcsSetting.ItemMaxCnt).Take(WcsSetting.ItemMaxCnt).ToList();

//            var currReq = new WcsSnReq { OwnerCode = custCode, Action = 1, SkuTotal = currDatas.Count, SkuList = currDatas.Select(x => x.Req).ToList() };

//            var lmsAprResult = new List<ApiResult>();


//            #region 呼叫商品序號同步
//            foreach (var dcCode in dcCodeList)
//            {
//              string url = $"v1/{dcCode}/ALL/Item/Sn";

//							lmsAprResult.Add(ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_ITEMSN, dcCode, gupCode, custCode, "WcsItemSnResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? currReq : null }, () =>
//							{
//								ApiResult result = new ApiResult { IsSuccessed = true };
//#if (DEBUG)
//                result = WcsApiFuncTest(currReq, "ItemSnAsync");
//#else
//                result = WcsApiFunc(currReq, url);
//#endif
//                return result;
//              }, false));
//            }
//            #endregion


//            #region 商品序號處理
//            // 如果API呼叫成功，更新該批次所有商品F2501.IS_ASYNC = Y
//            // 如果API呼叫失敗，更新該批次所有商品F2501.IS_ASYNC = N
//            var updF2501List = currDatas.Select(x => x.F2501).ToList();
//            var isAsync = "Y";
//            if (lmsAprResult.Where(x => !x.IsSuccessed).Any())
//            {
//              failCnt += currDatas.Count;
//              isAsync = "N";
//            }

//            // 更新F2501.ASYNC
//            List<string> serialNos = updF2501List.Select(x => x.SERIAL_NO).ToList();
//            f2501Repo.UpdateIsAsync(isAsync, gupCode, custCode, serialNos);
//            #endregion

//            // 回傳訊息
//            data.AddRange(lmsAprResult.Select(x => new ApiResponse
//            {
//              MsgCode = x.MsgCode,
//              MsgContent = x.MsgContent
//            }).ToList());
//          }

//          #region 更新系統設定最後一次商品同步時間
//          f0003.SYS_PATH = Convert.ToDateTime(maxDatetime).ToString("yyyy/MM/dd HH:mm:ss");
//          f0003Repo.Update(f0003);
//          #endregion
//        }
        #endregion
      }
      #endregion
      return new ApiResult
      {
        IsSuccessed = true,
        MsgCode = "10005",
        MsgContent = string.Format(tacService.GetMsg("10005"),
        "商品序號同步", totalCnt - failCnt, failCnt, totalCnt)
      };
    }
    #endregion

    private int CallItemSnAsyncApiByBatch(string gupCode, string custCode, List<WcsSnSkuModel> itemSnList, List<string> dcCodeList, int itemMaxCnt)
    {
     
      var failCnt = 0;
      var tacService = new TransApiBaseService();
      var f2501Repo = new F2501Repository(Schemas.CoreSchema);
      //var f0003Repo = new F0003Repository(Schemas.CoreSchema);
      var f1901Repo = new F1901Repository(Schemas.CoreSchema);

      //var f0003 = f0003Repo.AsForUpdate().Find(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00" && o.AP_NAME == "ItemSnAsyncLastTime");
      #region 每100筆送序號商品資料
      if (itemSnList.Any())
      {
        int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemSnList.Count()) / WcsSetting.ItemMaxCnt));

        for (int i = 0; i < index; i++)
        {
          var currDatas = itemSnList.Skip(i * WcsSetting.ItemMaxCnt).Take(WcsSetting.ItemMaxCnt).ToList();

          var currReq = new WcsSnReq { OwnerCode = custCode, Action = 1, SkuTotal = currDatas.Count, SkuList = currDatas.ToList() };

          var lmsApiResList = new Dictionary<string, List<ApiResult>>();          

          #region 呼叫商品序號同步
          foreach (var dcCode in dcCodeList)
          {
            var list = new List<ApiResult>();
            string url = $"v1/{dcCode}/ALL/Item/Sn";
            WcsSetting.DcCode = dcCode;

            list.Add(ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_ITEMSN, dcCode, gupCode, custCode, "WcsItemSnResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? currReq : null }, () =>
            {
              ApiResult result = new ApiResult { IsSuccessed = true };
#if (DEBUG)
              result = WcsApiFuncTest(currReq, "ItemSnAsync");
#else
                result = WcsApiFunc(currReq, url);
#endif
              return result;
            }, false));

            lmsApiResList.Add(dcCode, list);
          }
          #endregion
          List<string> itemCodes = currDatas.Select(x => x.SnCode).ToList();
          var isAsync = "Y";
          var errorResList = lmsApiResList.Where(x => x.Value.Any(y => !y.IsSuccessed)).ToList();
          var f0093List = new List<F0093>();
          var errorItemCodeList = new List<string>();
          if (errorResList.Any())
          {
            foreach (var errorRes in errorResList)
            {
              var errorList = errorRes.Value.Where(x => !x.IsSuccessed).ToList();
              foreach (var error in errorList)
              {
                try
                {
                  if (error.Data != null)
                  {
                    var errData = JsonConvert.DeserializeObject<List<WcsItemResData>>(JsonConvert.SerializeObject(error.Data));
                    var errorItemCodes = errData.Select(x => x.SkuCode).ToList();
                    errorItemCodeList.AddRange(errorItemCodes);
                  }
                }
                catch (Exception ex)
                {
                  errorItemCodeList.AddRange(itemCodes);
                }
              }
            }

            errorItemCodeList = errorItemCodeList.Distinct().ToList();

            //var updF2501List = currDatas.Select(x => x.F2501).ToList();

            itemCodes = itemCodes.Except(errorItemCodeList).ToList();
            if (errorItemCodeList.Any())
            {
              // 將IS_ASYNC更新為F         
              f2501Repo.UpdateIsAsync("F", gupCode, custCode, itemCodes);
            }
            failCnt += errorItemCodeList.Count;
            isAsync = "F";
          }
          if (itemCodes.Any())
          {
            f2501Repo.UpdateIsAsync(isAsync, gupCode, custCode, itemCodes);
          }

        }
      }
      #endregion
      return failCnt;
    }

    /// <summary>
    /// 刪除商品序號主檔
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="f060501"></param>
    /// <returns></returns>
    public ApiResult WcsItemSnCancel(string dcCode, string gupCode, string custCode, F060501 f060501)
		{
      #region 變數設定
      // 商品序號主檔Url
      string url = $"v1/{dcCode}/ALL/Item/Sn";
      var res = new ApiResult { IsSuccessed = true };
      var data = new List<ApiResponse>();
      var f055002Repo = new F055002Repository(Schemas.CoreSchema);
      var f02050402Repo = new F02050402Repository(Schemas.CoreSchema);
      var service = new WcsItemSnService();
      List<F02050402> f02050402s = null;
      List<WcsSnSkuModel> itemSnList = new List<WcsSnSkuModel>();
      var SettingsResendCount = GetMidApiRelmt();
      #endregion

      #region 主要邏輯
      if (f060501.WMS_NO.StartsWith("O"))//如果是出貨單要撈F055002
      {
        var f055002s = f055002Repo.GetDatasByWcsExecute(dcCode, gupCode, custCode, f060501.WMS_NO);
        if (f055002s.Any())
          itemSnList = f055002s.Select(x => new WcsSnSkuModel { SkuCode = x.ITEM_CODE, SnCode = x.SERIAL_NO }).ToList();
      }
      else if (f060501.WMS_NO.StartsWith("A"))//如果是進倉單要撈F02050402
      {
        f02050402s = f02050402Repo.GetDatasByWcsExecute(dcCode, gupCode, custCode, f060501.WMS_NO, SettingsResendCount).ToList();
        if (f02050402s.Any())
        {
          itemSnList = f02050402s.Select(x => new WcsSnSkuModel { SkuCode = x.ITEM_CODE, SnCode = x.SERIAL_NO }).ToList();
          f02050402s.ForEach(x =>
          {
            x.STATUS = "1";
            x.RESENT_CNT++;
            x.PROC_DATE = DateTime.Now;
          });
          f02050402Repo.BulkUpdate(f02050402s);
        }
      }
      else
        throw new Exception("無法識別的f060501.WMS_NO");

      #region 每100筆送商品序號資料
      if (itemSnList.Any())
      {
        var errprMsgList = new List<string>();

        int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemSnList.Count()) / WcsSetting.ItemMaxCnt));

        for (int i = 0; i < index; i++)
        {
          var currDatas = itemSnList.Skip(i * WcsSetting.ItemMaxCnt).Take(WcsSetting.ItemMaxCnt).ToList();
          var currReq = new WcsSnReq { OwnerCode = custCode, Action = 2, SkuTotal = currDatas.Count, SkuList = currDatas };
          WcsSetting.DcCode = dcCode;

          ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsItemSnCancelResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, F060501 = f060501, WcsData = isSaveWcsData ? currReq : null }, () =>
          {
            var result = service.ItemSn(url, currReq);
            if (!result.IsSuccessed)
              data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = f060501.WMS_NO });

            if (index > 1)
              errprMsgList.Add($"第{i + 1}批:{result.MsgContent}");
            else
              errprMsgList.Add(result.MsgContent);
            return result;
          }, false);
        }
        // 代表有錯誤
        if (data.Any())
        {
          res.IsSuccessed = false;
          res.Data = data;
        }
       
        res.MsgContent = string.Join(";", errprMsgList);

        //進倉單有獨立的序號表內容，要另外更新
        if (f060501.WMS_NO.StartsWith("A"))
        {
          SetF02050402Status(dcCode, gupCode, custCode, ref f02050402s, data);
          f02050402Repo.BulkUpdate(f02050402s);
        }
        #endregion
      }
      else
				res = new ApiResult { IsSuccessed = true, MsgCode = "23062", MsgContent = _tacService.GetMsg("23062") };
			#endregion

			return res;
		}

		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="crtDate"></param>
		/// <param name="updDate"></param>
		/// <returns></returns>
		private DateTime? Max(DateTime? crtDate, DateTime? updDate)
		{
			if (!crtDate.HasValue && !updDate.HasValue) return crtDate;

			if (!crtDate.HasValue) return updDate;
			if (!updDate.HasValue) return crtDate;

			return crtDate.Value > updDate.Value ? crtDate : updDate;
		}

    /// <summary>
    /// API跑完後，根據狀態更新f02050402.STATUS狀態用
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="f02050402s"></param>
    /// <param name="ApiRes"></param>
    private void SetF02050402Status(string dcCode, string gupCode, string custCode, ref List<F02050402> f02050402s, List<ApiResponse> ApiRes)
    {
      var SettingsResendCount = GetMidApiRelmt();
      if (ApiRes.Any())
      {
        //有錯誤的f02050402紀錄壓上T或F的狀態
        foreach (var resItem in ApiRes)
        {
          foreach (var failF02050402 in f02050402s.Where(x => x.STOCK_NO == resItem.No))
          {
            if (failF02050402.RESENT_CNT >= SettingsResendCount)
              failF02050402.STATUS = "F";
            else
              failF02050402.STATUS = "T";
          }
        }
        //沒錯誤的f02050402紀錄壓上成功狀態
        foreach (var successF02050402 in f02050402s.Where(x => !new[] { "F", "T" }.Contains(x.STATUS)))
          successF02050402.STATUS = "2";

      }
      else
        //都執行成功，直接壓上成功
        f02050402s.ForEach(x => x.STATUS = "2");

    }
  }
}