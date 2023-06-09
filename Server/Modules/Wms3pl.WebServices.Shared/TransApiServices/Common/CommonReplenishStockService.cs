using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
    public class CommonReplenishStockService
    {
        private WmsTransaction _wmsTransaction;
        public CommonReplenishStockService(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode)
        {
            var replenishService = new ReplenishService(_wmsTransaction);

            // 新增API Log
            return ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "CustProcessApiDatas_ReplenishStock", new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode }, () =>
            {
                var result = replenishService.DailyReplenish(dcCode, gupCode, custCode);
                return result;
            });
        }
    }
}
