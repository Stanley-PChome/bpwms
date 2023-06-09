using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.Lms.Services
{
	public class BatchPickService : LmsBaseService
	{
		private WmsTransaction _wmsTransaction;

		public BatchPickService(WmsTransaction wmsTransation = null)
		{
			_wmsTransaction = wmsTransation;
		}
		public BatchPickAllotRes BatchPickAllot(string dcCode, string gupCode, string custCode, string orderType, List<BatchPickAllotOrderData> OrderList)
		{
			BatchPickAllotRes batchPickAllotRes = new BatchPickAllotRes();
			var lmsApiReq = new BatchPickAllotReq
			{
				DcCode = dcCode,
				CustCode = custCode,
				OrderType = orderType,
				OrderList = OrderList
			};

      #region 新增API Log
      var res = ApiLogHelper.CreateApiLogInfo(ApiLogType.LMSAPI_F000906, dcCode, gupCode, custCode, "LmsBatchPickAllot", new { LmsApiUrl = LmsSetting.ApiUrl.Replace("ecoms/wmsapi/BPWMS/", "") + "wmsext-panel/api/PickingBatch/Assign", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode }, WmsData = isSaveWmsData ? lmsApiReq : null }, () =>
       {
         ApiResult result;
         result = LmsApiRtnMultiFunc<dynamic, BatchPickAllotPickingBatchData, ErrorData>(lmsApiReq, "wmsext-panel/api/PickingBatch/Assign");
         
         #region 模擬LMS回傳內容
         /*
         result = new ApiResult();
         result.IsSuccessed = true;
         result.MsgCode = "200";
         result.MsgContent = "Success";
         //單一揀貨 人工倉 兩張單 同PK區 PickingType=1
         //result.Data = JsonConvert.DeserializeObject<List<BatchPickAllotPickingBatchData>>(@"[{""PickingBatchNo"":""P0001"",""PickingType"":""1"",""CreateTime"":""2022-11-15 17:33:53"",""PickingList"":[{""PickingNo"":""P001"",""PickingSystem"":0,""PickAreaID"":""P001"",""PickAreaName"":""P001區"",""ContainerType"":""00"",""NextStepCode"":""3"",""TargetCode"":null,""Details"":[{""ItemCode"":""NN007"",""Qty"":31,""PickAreaID"":""P001"",""PickAreaName"":""P001區"",""LocCode"":""A01010102"",""ValidDate"":""2023-12-15"",""EnterDate"":""2021-12-05"",""MakeNo"":""220301001"",""SerialNo"":null,""Orders"":[{""WmsNo"":""S20221115000009"",""WmsSeq"":""000001"",""Qty"":31}]}]},{""PickingNo"":""P002"",""PickingSystem"":0,""PickAreaID"":""P001"",""PickAreaName"":""P001區"",""ContainerType"":""00"",""NextStepCode"":""3"",""TargetCode"":null,""Details"":[{""ItemCode"":""NN007"",""Qty"":31,""PickAreaID"":""P001"",""PickAreaName"":""P001區"",""LocCode"":""A01010102"",""ValidDate"":""2023-12-15"",""EnterDate"":""2021-12-05"",""MakeNo"":""220301001"",""SerialNo"":null,""Orders"":[{""WmsNo"":""S20221115000010"",""WmsSeq"":""000002"",""Qty"":31}]}]}]}]");

         //一單分貨 兩張單 不同品項 自動倉 PickingType=2
         //result.Data = JsonConvert.DeserializeObject<List<BatchPickAllotPickingBatchData>>(@"[{""PickingBatchNo"":""PB201"",""PickingType"":""2"",""CreateTime"":""2022-11-16 11:50:06"",""PickingList"":[{""PickingNo"":""P001"",""PickingSystem"":1,""PickAreaID"":""G86"",""PickAreaName"":""AGV倉"",""ContainerType"":""00"",""NextStepCode"":""1"",""TargetCode"":null,""Details"":[{""ItemCode"":""HH003"",""Qty"":5,""PickAreaID"":""G86"",""PickAreaName"":""AGV倉"",""LocCode"":""A09010101"",""ValidDate"":""2023-12-31"",""EnterDate"":""2021-03-18"",""MakeNo"":""TF210318003"",""SerialNo"":null,""Orders"":[{""WmsNo"":""S20221116000011"",""WmsSeq"":""000001"",""Qty"":5}]},{""ItemCode"":""AGV001"",""Qty"":5,""PickAreaID"":""G86"",""PickAreaName"":""AGV倉"",""LocCode"":""A09010101"",""ValidDate"":""2023-12-31"",""EnterDate"":""2022-07-06"",""MakeNo"":""220706001"",""SerialNo"":null,""Orders"":[{""WmsNo"":""S20221116000012"",""WmsSeq"":""000002"",""Qty"":5}]}]}]}]");

         //多單集貨 一張單 不同品項 自動倉 PickingType=2
         result.Data = JsonConvert.DeserializeObject<List<BatchPickAllotPickingBatchData>>(@"[{""PickingBatchNo"":""PB201"",""PickingType"":""3"",""CreateTime"":""2022-11-16 14:54:56"",""PickingList"":[{""PickingNo"":""P001"",""PickingSystem"":1,""PickAreaID"":null,""PickAreaName"":null,""ContainerType"":""00"",""NextStepCode"":""1"",""TargetCode"":null,""Details"":[{""ItemCode"":""NN007"",""Qty"":5,""PickAreaID"":""P001"",""PickAreaName"":""P001區"",""LocCode"":""A01010102"",""ValidDate"":""2023-12-15"",""EnterDate"":""2021-12-05"",""MakeNo"":""220301001"",""SerialNo"":null,""Orders"":[{""WmsNo"":""S20221116000017"",""WmsSeq"":""000001"",""Qty"":5}]},{""ItemCode"":""NN007"",""Qty"":5,""PickAreaID"":""P002"",""PickAreaName"":""P002區"",""LocCode"":""A02010103"",""ValidDate"":""2023-12-15"",""EnterDate"":""2022-04-12"",""MakeNo"":""220706001"",""SerialNo"":null,""Orders"":[{""WmsNo"":""S20221116000017"",""WmsSeq"":""000002"",""Qty"":5}]}]}]}]");

         //  */
         #endregion

         return result;
       }, false);
      #endregion

      batchPickAllotRes.IsSuccessed = res.MsgCode == "200" ? true : false;
      batchPickAllotRes.Message = res.MsgContent;

      if (res.IsSuccessed)
      {
        batchPickAllotRes.Data = JsonConvert.DeserializeObject<List<BatchPickAllotPickingBatchData>>(JsonConvert.SerializeObject(res.Data));
      }
      else
      {
        var errorData = JsonConvert.DeserializeObject<ErrorData>(JsonConvert.SerializeObject(res.Data));
        batchPickAllotRes.ErrorData = new BatchPickAllotErrorData { ErrorColumn = errorData?.ErrorColumn };
      }
      return batchPickAllotRes;
    }

  }
}
