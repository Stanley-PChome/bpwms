using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;

namespace Wms3pl.WebServices.Shared.WcsServices
{
	public class WcsInboundCancelService : WcsBaseService
	{
		private WmsTransaction _wmsTransaction;
		public WcsInboundCancelService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ApiResult InboundCancel(string url, WcsInboundCancelReq req)
		{
#if (DEBUG)
            return WcsApiFuncTest(req, "InboundCancel");
#else
            return WcsApiFunc(req, url);
#endif
        }

    /// <summary>
    /// 即時入庫取消
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="allocNo"></param>
    /// <param name="tarWarehouseId"></param>
    /// <returns></returns>
    public ApiResult PromptInboundCancel(string dcCode, string gupCode, string custCode, string allocNo, string tarWarehouseId)
    {
      var result = new ApiResult { IsSuccessed = true };
      var f060101Repo = new F060101Repository(Schemas.CoreSchema, _wmsTransaction);

      var f060101s = f060101Repo.GetDatasByTrueAndCondition(o => o.CMD_TYPE == "1" &&
      o.DC_CODE == dcCode &&
      o.GUP_CODE == gupCode &&
      o.CUST_CODE == custCode &&
      o.WAREHOUSE_ID == tarWarehouseId &&
      o.WMS_NO == allocNo).OrderByDescending(x => x.PROC_DATE).ToList();

      if (!f060101s.Any())
        return result;


      if (f060101s.Any(x => x.STATUS == "1"))
      {
        return new ApiResult() { IsSuccessed = false, MsgCode = "99990", MsgContent = "目前系統正在執行派發入庫任務，請稍後再修改" };
      }

      if (f060101s.First().STATUS != "2")
      {
        var status = new List<string> { "0", "T", "F" };
        var notFinishDatas = f060101s.Where(x => status.Contains(x.STATUS)).ToList();
        if (notFinishDatas.Any())
        {
          notFinishDatas.ForEach(f060101 =>
          {
            f060101.STATUS = "9";// 取消單據流程
            f060101.MESSAGE = "尚未發送，先行取消";
          });
          f060101Repo.BulkUpdate(notFinishDatas);
        }
      } 
      else
      {
        // 入庫任務取消Url
        string url = $"v1/{dcCode}/{tarWarehouseId}/Inbound/Cancel";
        var req = new WcsInboundCancelReq
        {
          OwnerCode = custCode,
          ReceiptCode = f060101s.First().DOC_ID
        };

        WcsSetting.DcCode = dcCode;
        result = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsInboundCancelResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null }, () =>
        {
          return InboundCancel(url, req);
        }, false);

      }
      return result;
    }
  }
}
