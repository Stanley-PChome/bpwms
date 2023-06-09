using System.Collections.Generic;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
    public class CheckVendorService
    {
        private TransApiBaseService _tacService;

        public CheckVendorService()
        {
            _tacService = new TransApiBaseService();
        }

        /// <summary>
        /// 檢核是否大於0
        /// </summary>
        /// <param name="res"></param>
        /// <param name="vendor"></param>
        public void CheckValueNotZero(List<ApiResponse> res, PostVendorDataVendorsModel vendor)
        {
            List<string> failCols = new List<string>();
            List<string> chkCols = new List<string> { "Checkpercent" };

            chkCols.ForEach(colName =>
            {
                if (DataCheckHelper.CheckRequireColumn(vendor, colName) &&
                    !DataCheckHelper.CheckDataNotZero(vendor, colName))
                    failCols.Add(colName);
            });

            if (failCols.Count > 0)
            {
                res.Add(new ApiResponse
                {
                    No = vendor.VnrCode,
                    MsgCode = "20019",
                    MsgContent = string.Format(_tacService.GetMsg("20019"),
                    vendor.VnrCode,
                    string.Join("、", failCols))
                });
            }
        }

        /// <summary>
        /// 檢核狀態是否正確
        /// </summary>
        /// <param name="res"></param>
        /// <param name="vendor"></param>
        public void CheckStatus(List<ApiResponse> res, PostVendorDataVendorsModel vendor)
        {
            List<string> status = new List<string> { "0", "9" };
            if (!status.Contains(vendor.Status))
                res.Add(new ApiResponse { No = vendor.VnrCode, MsgCode = "20773", MsgContent = string.Format(_tacService.GetMsg("20773"), vendor.VnrCode) });
        }

        /// <summary>
        /// 檢核稅別是否正確
        /// </summary>
        /// <param name="res"></param>
        /// <param name="vendor"></param>
        public void CheckTaxType(List<ApiResponse> res, PostVendorDataVendorsModel vendor)
        {
            List<string> taxTypes = new List<string> { "0", "1" };
            if (!string.IsNullOrWhiteSpace(vendor.TaxType) && !taxTypes.Contains(vendor.TaxType))
                res.Add(new ApiResponse { No = vendor.VnrCode, MsgCode = "20774", MsgContent = string.Format(_tacService.GetMsg("20774"), vendor.VnrCode) });
        }
				/// <summary>
				/// 檢查配送方式是否正確
				/// </summary>
				/// <param name="res"></param>
				/// <param name="vnrCodeList"></param>
				/// <param name="vnrReturns"></param>
				public void CheckDeliveryWay(List<ApiResponse> res, PostVendorDataVendorsModel vendor)
				{
					var deliveryWay = new List<string> { "0", "1" };
					if (!deliveryWay.Contains(vendor.DeliveryWay))
						res.Add(new ApiResponse { No = vendor.VnrCode, MsgCode = "20789", MsgContent = string.Format(_tacService.GetMsg("20789"), vendor.VnrCode, "配送方式") });
				}
	}
}
