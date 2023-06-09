namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 補貨超揀申請_傳入
    /// </summary>
    public class OverPickApplyReq
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
        /// 儲區編號=倉別編號
        /// </summary>
        public string ZoneCode { get; set; }
        /// <summary>
        /// 出庫單號
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 明細項次
        /// </summary>
        public int? RowNum { get; set; }
        /// <summary>
        /// 商品編號
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 實際下架數量
        /// </summary>
        public int? SkuQty { get; set; }
	}

    public class WcsApiOverPickApplyReceiptResultData
    {
        public string ErrorColumn { get; set; }
        public WcsApiErrorResult errors { get; set; }
    }
}
