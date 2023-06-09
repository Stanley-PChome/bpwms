using Newtonsoft.Json;
using System.Collections.Generic;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.ForeignWebApi.Business.LmsServices;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.Export.Services
{
	public class ExportServices
	{
		private WmsTransaction _wmsTransaction;

		public ExportServices()
		{
			_wmsTransaction = new WmsTransaction();
		}

		/// <summary>
		/// 商品進倉
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ExportWarehouseInResults(ExportResultReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportWarehouseInResults", req, () =>
      {
        // 取得物流中心服務貨主檔
        CommonService commonService = new CommonService();
        var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

        ExportWarehouseInService ewService = new ExportWarehouseInService(_wmsTransaction);

        // Call 匯出商品進倉結果
        dcCustList.ForEach(item =>
        {
          var result = ewService.ExportWarehouseInResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);

          data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });

        });

        res.Data = JsonConvert.SerializeObject(data);

				return res;

			}, true);

			return res;
		}

        /// <summary>
        /// 客戶訂單出貨結果
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult ExportOrderResults(ExportResultReq req)
        {
            ApiResult res = new ApiResult { IsSuccessed = true };
            List<ApiResponse> data = new List<ApiResponse>();

            // 新增API Log
            res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportOrderResults", req, () =>
            {
                // 取得物流中心服務貨主檔
                CommonService commonService = new CommonService();
                var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

                ExportOrderService eoService = new ExportOrderService(_wmsTransaction);

                // Call 匯出訂單出貨結果
                dcCustList.ForEach(item =>
                {
                    var result = eoService.ExportOrderResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);

                    data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });

                });

                res.Data = JsonConvert.SerializeObject(data);

                return res;

            }, true);

            return res;
        }

        /// <summary>
        /// 廠退出貨結果
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult ExportVendorReturnResults(ExportResultReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportVendorReturnResults", req, () =>
			{
							// 取得物流中心服務貨主檔
							CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

				ExportVendorReturnService ewService = new ExportVendorReturnService(_wmsTransaction);

							// 匯出廠退出貨結果
							dcCustList.ForEach(item =>
							{
						var result = ewService.ExportVendorReturnResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);

						data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
					});

				res.Data = JsonConvert.SerializeObject(data);

				return res;

			}, true);

			return res;
		}

		/// <summary>
		/// 庫存警示回報排程
		/// </summary>
		/// <returns></returns>
		public ApiResult ExpStockAlertResults(ExportResultReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExpStockAlertResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

				ExpStockAlertService ewService = new ExpStockAlertService(_wmsTransaction);

				// 匯出庫存警示回報結果
				dcCustList.ForEach(item =>
				{
					var result = ewService.ExpStockAlertResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);

					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});

				res.Data = JsonConvert.SerializeObject(data);

				return res;

			}, true);

			return res;
		}

		/// <summary>
		/// 單箱出貨扣帳
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ExportHomeDeliveryResults(ExportResultReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportHomeDeliveryResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

				ExportHomeDeliveryService ewService = new ExportHomeDeliveryService(_wmsTransaction);

				// 匯出單箱出貨扣帳
				dcCustList.ForEach(item =>
				{
					var result = ewService.ExportHomeDeliveryResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);

					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});

				res.Data = JsonConvert.SerializeObject(data);

				return res;

			}, true);

			return res;
		}

		/// <summary>
		/// 庫存異動回報
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ExportStockMovementResults(ExportResultReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportStockMovementResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

				ExportStockMovementService esService = new ExportStockMovementService(_wmsTransaction);

				// 庫存異動回報
				dcCustList.ForEach(item =>
				{
					var result = esService.ExportStockMovementResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);

					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});

				res.Data = JsonConvert.SerializeObject(data);

				return res;

			}, true);

			return res;
		}

        /// <summary>
        /// LMS系統異常通知
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult ExportSysErrorNotifyResults(ExportResultReq req)
        {
            ApiResult res = new ApiResult { IsSuccessed = true };
            List<ApiResponse> data = new List<ApiResponse>();

            // 新增API Log
            res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportSysErrorNotifyResults", req, () =>
            {
                // 取得物流中心服務貨主檔
                CommonService commonService = new CommonService();
                var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

                ExportSysErrorNotifyService esService = new ExportSysErrorNotifyService(_wmsTransaction);

                // LMS系統異常通知
                dcCustList.ForEach(item =>
                {
                    var result = esService.ExportSysErrorNotifyResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);

                    data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
                });

                res.Data = JsonConvert.SerializeObject(data);

                return res;

            }, true);

            return res;
        }
    }
}
