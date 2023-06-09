using System.Collections.Generic;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
    public class CheckOverPickApplyReceiptService
    {
        private TransApiBaseService tacService = new TransApiBaseService();

        /// <summary>
        /// 檢查該出庫單號是否存在於F060201
        /// </summary>
        /// <param name="res"></param>
        /// <param name="receipt"></param>
        /// <param name="f060201"></param>
        public void CheckOrderCodeExistByF060201(List<ApiResponse> res, OverPickApplyReq receipt, F060201 f060201)
        {
            if (f060201 == null)
                res.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "OrderCode", MsgCode = "23063", MsgContent = string.Format(tacService.GetMsg("23063"), receipt.OrderCode) });
        }

        /// <summary>
        /// 檢查該出庫單號是否存在於F151001
        /// </summary>
        /// <param name="res"></param>
        /// <param name="receipt"></param>
        /// <param name="f151001"></param>
        /// 
        public void CheckOrderCodeExistByF151001(List<ApiResponse> res, OverPickApplyReq receipt, F151001 f151001, string wmsNo)
        {
            if (f151001 == null)
                res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "23001", MsgContent = string.Format(tacService.GetMsg("23001"), wmsNo) });
        }

        /// <summary>
        /// 檢查該調撥單狀態
        /// </summary>
        /// <param name="res"></param>
        /// <param name="receipt"></param>
        /// <param name="f151001"></param>
        /// <param name="wmsNo"></param>
        public void CheckAllocNoStatus(List<ApiResponse> res, OverPickApplyReq receipt, F151001 f151001, string wmsNo)
        {
            if (f151001 != null)
            {
                if (f151001.STATUS == "9")
                    res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "23052", MsgContent = string.Format(tacService.GetMsg("23052"), wmsNo) });

                if (f151001.STATUS == "5")
                    res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "23053", MsgContent = string.Format(tacService.GetMsg("23053"), wmsNo) });
            }
        }

        /// <summary>
        /// 檢查該調撥單是否為下架單
        /// </summary>
        /// <param name="res"></param>
        /// <param name="receipt"></param>
        /// <param name="f151001"></param>
        /// <param name="wmsNo"></param>
        public void CheckAllocNoIsSrc(List<ApiResponse> res, OverPickApplyReq receipt, F151001 f151001, string wmsNo)
        {
            var srcStatusList = new List<string> { "1", "2" };
            if (f151001 != null && !srcStatusList.Contains(f151001.STATUS))
                res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "20048", MsgContent = string.Format(tacService.GetMsg("20048"), wmsNo) });
        }

        /// <summary>
        /// 檢核明細項次是否有在調撥明細中
        /// </summary>
        /// <param name="res"></param>
        /// <param name="receipt"></param>
        /// <param name="f151002"></param>
        /// <param name="wmsNo"></param>
        public void CheckRowNumIsExist(List<ApiResponse> res, OverPickApplyReq receipt, F151002 f151002, string wmsNo)
        {
            if (f151002 == null)
                res.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "RowNum", MsgCode = "23057", MsgContent = string.Format(tacService.GetMsg("23057"), wmsNo) });
        }

        /// <summary>
        /// 檢核實際揀貨數量是否大於0、是否小於預計調撥下架數
        /// </summary>
        /// <param name="res"></param>
        /// <param name="receipt"></param>
        /// <param name="f151002"></param>
        /// <param name="wmsNo"></param>
        public void CheckSkuQty(List<ApiResponse> res, OverPickApplyReq receipt, F151002 f151002, string wmsNo)
        {
            if (receipt.SkuQty <= 0)
                res.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "SkuQty", MsgCode = "23056", MsgContent = string.Format(tacService.GetMsg("23056"), wmsNo) });
            
            if (f151002 != null && receipt.SkuQty <= f151002.SRC_QTY)
                res.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "SkuQty", MsgCode = "23005", MsgContent = string.Format(tacService.GetMsg("23005"), wmsNo) });
        }
    }
}
