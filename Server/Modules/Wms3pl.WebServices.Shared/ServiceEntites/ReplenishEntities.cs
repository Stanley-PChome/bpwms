using System.Collections.Generic;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
    /// <summary>
    /// 補貨處理回傳物件
    /// </summary>
    public class ReplenishProcessRes
    {
        /// <summary>
        /// 計算後商品需求量清單
        /// </summary>
        public List<ItemNeedQtyModel> ItemSuggetReplenishList { get; set; } = new List<ItemNeedQtyModel>();
        /// <summary>
        /// 產生的補貨調撥單清單
        /// </summary>
        public List<ExecuteResult> ReturnAllocationList { get; set; } = new List<ExecuteResult>();
    }

    /// <summary>
    /// 商品需求量物件
    /// </summary>
    public class ItemNeedQtyModel
    {
        /// <summary>
        /// 品號
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 指定批號
        /// </summary>
        public string MakeNo { get; set; }
		   /// <summary>
			 /// 指定序號
			 /// </summary>
		    public string SerialNo { get; set; }
        /// <summary>
        /// 商品需求量
        /// </summary>
        public long NeedQty { get; set; }
        /// <summary>
        /// 計算後補貨建議量
        /// </summary>
        public int SuggestReplenishQty { get; set; }
        /// <summary>
        /// 調撥單號清單
        /// </summary>
        public List<string> AllocationNos { get; set; } = new List<string>();
    }

    public enum ReplenishType
    {
        Daily,
        Manual
    }
}
