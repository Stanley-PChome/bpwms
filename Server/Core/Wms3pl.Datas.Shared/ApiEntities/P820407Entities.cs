using System;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 出庫任務取消中介_傳入
    /// </summary>
    public class WcsOutboundCancelReq
    {
        /// <summary>
        /// 業主編號=貨主編號
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// 出庫單號=WMS出貨單號/揀貨單號
        /// </summary>
        public string OrderCode { get; set; }
    }

    /// <summary>
    /// 出庫任務取消中介_回傳
    /// </summary>
    public class WcsOutboundCancelResData
    {
        /// <summary>
        /// 出庫單號/揀貨單號
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 結果狀態 (0=取消成功, 1=取消失敗)
        /// </summary>
        public string Status { get; set; }
    /// <summary>
    /// 完成時間 (yyyy/MM/dd HH:ii:ss)
    /// </summary>
    //public DateTime CompleteTime { get; set; }
    public string CompleteTime { get; set; }
    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string ErrorMsg { get; set; }
    }
}
