using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 入庫任務中介_傳入
    /// </summary>
    public class WcsInboundReq
    {
        /// <summary>
        /// 業主編號=貨主編號
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// WMS單號
        /// </summary>
        public string ReceiptCode { get; set; }
        /// <summary>
        /// 入庫類型(0=採購收, 1=常規收, 2=調撥收, 3=ASN收, 4=退貨收, 5=捕貨收, 6=其 他收, 7=虛擬入)
        /// </summary>
        public int ReceiptType { get; set; }
        /// <summary>
        /// 棧板號(一單一板)
        /// </summary>
        public string PalletCode { get; set; }
        /// <summary>
        /// 明細數
        /// </summary>
        public int SkuTotal { get; set; }
		/// <summary>
		/// 入庫方式(0=不指定 1:Apple商品)
		/// </summary>
		public int? PutawayType { get; set; }
		/// <summary>
		/// 明細資料
		/// </summary>
		public List<WcsInboundSkuModel> SkuList { get; set; }
    }

    /// <summary>
    /// 明細資料
    /// </summary>
    public class WcsInboundSkuModel
    {
        /// <summary>
        /// 單據項次
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 商品編號
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 預計入庫數量
        /// </summary>
        public int SkuQty { get; set; }
        /// <summary>
        /// 商品等級(0=殘品/客退品, 1=正品/新品) 
        /// </summary>
        public int SkuLevel { get; set; }
		/// <summary>
		/// 出貨等級 (0=一般、1=客退優先)
		/// </summary>
		public int OutLevel { get; set; }
		/// <summary>
		/// 商品效期(yyyy/mm/dd)
		/// </summary>
		public string ExpiryDate { get; set; }
        /// <summary>
        /// 外部批次號 = WMS商品入庫日(yyyy/mm/dd)
        /// </summary>
        public string OutBatchCode { get; set; }
        /// <summary>
        /// 容器編碼
        /// </summary>
        public string ContainerCode { get; set; }
		/// <summary>
		/// 容器分隔編號(A7)
		/// </summary>
		public string BinCode { get; set; }

	}

    /// <summary>
    /// 入庫任務中介_回傳
    /// </summary>
    public class WcsInboundResData
    {
        /// <summary>
        /// 錯誤單號
        /// </summary>
        public string ReceiptCode { get; set; }
        /// <summary>
        /// 錯誤欄位
        /// </summary>
        public string ErrorColumn { get; set; }
        /// <summary>
        /// AGV錯誤回應
        /// </summary>
        public List<WcsErrorModel> errors { get; set; }
    }
}
