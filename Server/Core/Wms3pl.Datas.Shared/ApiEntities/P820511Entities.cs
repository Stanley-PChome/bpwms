using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    public class AutoLocAbnormalNotifyReq
    {
        /// <summary>
        /// 業主編號=WMS貨主編號
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// 目的倉庫編號=物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 儲區編號
        /// </summary>
        public string ZoneCode { get; set; }
        /// <summary>
        /// 異常類型(1=揀缺, 2=商品錯誤)
        /// </summary>
        public int AbnormalType { get; set; }
        /// <summary>
        /// 貨架編號
        /// </summary>
        public string ShelfCode { get; set; }
        /// <summary>
        /// 儲位編號
        /// </summary>
        public string BinCode { get; set; }
        /// <summary>
        /// 異常單號
        /// </summary>
        public string OrderCode { set; get; }
        /// <summary>
        /// 異常品號
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 異常數量
        /// </summary>
        public int SkuQty { get; set; }
        /// <summary>
        /// 作業人員
        /// </summary>
        public string Operator { get; set; }
    }

    //public class WcsApiAutoLocAbnormalNotifyResultData
    //{
    //    /// <summary>
    //    /// 回應代碼
    //    /// </summary>
    //    public string Code { get; set; }
    //    /// <summary>
    //    /// 代碼訊息
    //    /// </summary>
    //    public string Msg { get; set; }
    //    /// <summary>
    //    /// 代碼訊息
    //    /// </summary>
    //    public List<AutoLocAbnormalNotifyErrorData> Data;
    //}

    public class WcsApiAutoLocAbnormalNotifyResultData
    {
        /// <summary>
        /// 錯誤欄位
        /// </summary>
        public string ErrorColumn { get; set; }
        /// <summary>
        /// 錯誤回應
        /// </summary>
        public WcsApiErrorResult errors { get; set; }
    }
}
