using System;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 入庫任務取消中介_傳入
    /// </summary>
    public class WcsInboundCancelReq
    {
        /// <summary>
        /// 業主編號=WMS貨主編號
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// 入庫單號
        /// </summary>
        public string ReceiptCode { get; set; }
    }

    /// <summary>
    /// 入庫任務取消中介_回傳
    /// </summary>
    public class WcsInboundCancelResData
    {
        /// <summary>
        /// 入庫單號
        /// </summary>
        public string ReceiptCode { get; set; }
        /// <summary>
        /// 結果狀態 (0=取消成功, 1=取消失敗)
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 完成時間(yyyy/MM/dd HH:ii:ss)
        /// </summary>
        public string CompleteTime { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
