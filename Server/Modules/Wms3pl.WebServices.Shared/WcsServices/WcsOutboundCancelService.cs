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
	public class WcsOutboundCancelService : WcsBaseService
	{
		private WmsTransaction _wmsTransaction;
		public WcsOutboundCancelService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		public ApiResult OutboundCancel(string url, WcsOutboundCancelReq req)
		{
#if (DEBUG)
            return WcsApiFuncTest(req, "OutboundCancel");
#else
            return WcsApiFunc(req, url);
#endif
        }

    /// <summary>
    /// 即時出庫取消
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="allocNo"></param>
    /// <param name="srcWarehouseId"></param>
    /// <returns></returns>
    public ApiResult PromptOutboundCancel(string dcCode, string gupCode, string custCode, string allocNo, string srcWarehouseId)
		{
			var result = new ApiResult { IsSuccessed = true };
			var f060201Repo = new F060201Repository(Schemas.CoreSchema, _wmsTransaction);

			var f060201s = f060201Repo.GetDatasByTrueAndCondition(o => o.CMD_TYPE == "1" &&
			o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode &&
			o.WAREHOUSE_ID == srcWarehouseId &&
			o.WMS_NO == allocNo);

      if (f060201s.Any(x => x.STATUS == "1"))
      {
        return new ApiResult() { IsSuccessed = false, MsgCode = "99990", MsgContent = "目前系統正在執行派發出庫任務，請稍後再修改" };
      }

      var status = new List<string> { "0", "T", "F" };
			var notFinishDatas = f060201s.Where(x => status.Contains(x.STATUS)).ToList();
			if (notFinishDatas.Any())
			{
				notFinishDatas.ForEach(f060201 =>
				{
					f060201.STATUS = "9";// 取消單據流程
					f060201.MESSAGE = "尚未發送，先行取消";
				});
				f060201Repo.BulkUpdate(notFinishDatas);
			}
      else
      {
				var finishData = f060201s.Where(x => x.STATUS == "2").OrderByDescending(x => x.PROC_DATE).FirstOrDefault();

				if (finishData == null)
				{
					result = new ApiResult { IsSuccessed = false, MsgCode = "23061", MsgContent = string.Format(_tacService.GetMsg("23061"), allocNo) };
				}
				else
				{
					// 出庫任務取消Url
					string url = $"v1/{dcCode}/{srcWarehouseId}/Outbound/Cancel";
					var req = new WcsOutboundCancelReq
					{
						OwnerCode = custCode,
						OrderCode = finishData.DOC_ID
					};

          WcsSetting.DcCode = dcCode;
          result = ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsOutboundCancelResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null }, () =>
					{
						return OutboundCancel(url, req);
					}, false);
				}
			}
			return result;
		}
	}
}
