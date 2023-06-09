using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;

namespace Wms3pl.WebServices.Shared.Lms.Services
{
  public class PrintService : LmsBaseService
  {
    private WmsTransaction _wmsTransaction;

    public PrintService(WmsTransaction wmsTransation = null)
    {
      _wmsTransaction = wmsTransation;
    }

    /// <summary>
    /// 取得訂單出貨列印清單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="custOrdNo"></param>
    /// <param name="boxSno"></param>
    /// <returns></returns>
    public ApiResult GetShipPrintList(string dcCode, string gupCode, string custCode, string custOrdNo, string boxSno)
    {
			string url = $"printapi/PrintJobList/{dcCode}|{custOrdNo}|{boxSno}";

			return ApiLogHelper.CreateApiLogInfo(ApiLogType.LMSAPI_F000906, dcCode, gupCode, custCode, "LmsPrintJobList", new { LmsApiUrl = $"{LmsSetting.ApiUrl.Replace("ecoms/wmsapi/BPWMS/", "")}{url}", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { WhId = dcCode, CustOrdNo = custOrdNo, BoxSno = boxSno } }, () =>
      {
				var res = new ApiResult();
#if DEBUG
				res.IsSuccessed = true;
				res.MsgCode = "200";
				res.MsgContent = "Success";
				res.Data = new PrintJobListRes[]
				{
					new PrintJobListRes
					{
						Name="宅配單",
						Url="https://lms.agroup.tw/printapi/PdfTest/1"
					},
				};
#else
				res = LmsApiGetAsync<PrintJobListRes, ErrorData>(url);
#endif
				return res;
      }, false);
    }
  }
}
