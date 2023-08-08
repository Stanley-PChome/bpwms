using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 上架倉別指示_傳入
    /// </summary>
    public class StowShelfAreaGuideReq
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
        /// 上架類型(1=新品進貨 2=庫內補貨 3=跨庫調入)
        /// </summary>
        public string StowType { get; set; }
        /// <summary>
        /// 貨主進貨單號
        /// 貨主進倉單號(StowType=1)
        /// WMS調撥單號(StowType= 2)
        /// 跨庫進貨單號(StowType= 3)
        /// </summary>
        public string CustInNo { get; set; }
        /// <summary>
        /// (商品明細)
        /// </summary>
        public List<StowShelfAreaGuideItemData> ItemList { get; set; }
    }

    public class StowShelfAreaGuideItemData
    {
        /// <summary>
        /// 商品編號
        /// </summary>
        public string ItemCode { get; set; }
    }

    public class StowShelfAreaGuideData
	{
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 上架倉別代碼
		/// </summary>
		public string ShelfAreaCode { get; set; }
	}
}
