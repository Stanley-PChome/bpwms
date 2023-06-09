using Newtonsoft.Json;
using System.Collections.Generic;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Common;

namespace Wms3pl.WebServices.Schedule.ReplenishStock.Services
{
	public class ReplenishStockService
	{
		private WmsTransaction _wmsTransaction;

		public ReplenishStockService()
		{
			_wmsTransaction = new WmsTransaction();
		}

		/// <summary>
		/// 每日補貨排程
		/// </summary>
		/// <param name="req"></param>
		/// <param name="dcCustList"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas_Order(BatchTransApiOrdersReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ProcessApiDatas_ReplenishStock", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

				CommonReplenishStockService crService = new CommonReplenishStockService(_wmsTransaction);
				List<ApiResponse> data = new List<ApiResponse>();

				dcCustList.ForEach(item =>
				{
					var result = crService.ProcessApiDatas(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);

          if (result.IsSuccessed == true)
          {
            _wmsTransaction.Complete();
          }

					data.Add(new ApiResponse
					{
            No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{item.CUST_CODE}",
            MsgContent = JsonConvert.SerializeObject(result.Data)
          });
        });

				res.Data = JsonConvert.SerializeObject(data);

				return res;

			}, true);

			return res;
		}
	}
}
