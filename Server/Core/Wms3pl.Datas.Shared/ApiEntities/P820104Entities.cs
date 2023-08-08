using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 商品階層檔_傳入
    /// </summary>
    public class PostItemLevelReq
    {
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        public PostItemLevelResultModel Result { get; set; }
    }

    public class PostItemLevelResultModel
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        public int? Total { get; set; }
        /// <summary>
        /// 商品階層物件清單
        /// </summary>
        public List<PostItemLevelLevelsModel> ItemLevels { get; set; }
    }

    /// <summary>
    /// 商品階層物件
    /// </summary>
    public class PostItemLevelLevelsModel
    {
        /// <summary>
        /// 商品編號
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 商品階層(1:第一階,2:第二階,3:第三階)
        /// </summary>
        public int? ItemLevel { get; set; }
        /// <summary>
        /// 商品單位編號
        /// </summary>
        public string UnitId { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int? UnitQty { get; set; }
        /// <summary>
        /// 長度
        /// </summary>
        public decimal? Length { get; set; }
        /// <summary>
        /// 寬度
        /// </summary>
        public decimal? Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public decimal? Height { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal? Weight { get; set; }
        /// <summary>
        /// 系統單位(01:盒入數;02:箱入數)
        /// </summary>
        public string SysUnit { get; set; }
    }

    public class PostItemLevelsGroupModel
    {
        /// <summary>
        /// 同商品編號&商品單位編號最後一筆
        /// </summary>
        public PostItemLevelLevelsModel LastData { get; set; }
        /// <summary>
        /// 同商品編號&商品單位編號筆數
        /// </summary>
        public int Count { get; set; }
    }
}
