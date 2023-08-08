using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 複驗比例確認_傳入
    /// </summary>
    public class DoubleCheckConfirmReq
    {
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
        /// <summary>
        /// 貨主進貨單號
        /// </summary>
        public string CustInNo { get; set; }
        /// <summary>
        /// 商品明細
        /// </summary>
        public List<DoubleCheckConfirmItem> ItemList { get; set; } = new List<DoubleCheckConfirmItem>();
    }

    public class DoubleCheckConfirmItem
    {
        /// <summary>
        /// 商品編號
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 驗收數量
        /// </summary>
        public int Qty { get; set; }
    }

    public class DoubleCheckConfirmData
    {
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
        /// <summary>
        /// 是否需要複驗(0否1是)
        /// </summary>
        public string IsNeedDoubleCheck { get; set; }
        /// <summary>
        /// 抽驗數量
        /// </summary>
        public int Qty { get; set; }
    }

    /// <summary>
    /// 上架倉別指示_傳入
    /// </summary>
    public class StowShelfAreaAssognReq
    {
        /// <summary>
        /// 倉庫代碼
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 貨主進貨單號
        /// </summary>
        public string CustInNo { get; set; }
        /// <summary>
        /// 商品編號
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 待上架數量
        /// </summary>
        public int Qty { get; set; }

    }

    public class StowShelfAreaAssignData
    {
        /// <summary>
        /// 揀區(A)/補區(C)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 上架倉別代碼
        /// </summary>
        public string ShelfAreaCode { get; set; }
        /// <summary>
        /// 上架數量
        /// </summary>
        public int Qty { get; set; }
    }
}
