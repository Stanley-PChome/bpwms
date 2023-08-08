using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 商品分類_傳入
    /// </summary>
    public class PostItemCategoryReq
    {
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        public PostItemCategoryResultModel Result { get; set; }
    }

    public class PostItemCategoryResultModel
    {
        /// <summary>
        /// 商品大分類總筆數
        /// </summary>
        public int? LTotal { get; set; }
        /// <summary>
        /// 商品大分類清單
        /// </summary>
        public List<PostItemCategoryLCategorysModel> LCategorys { get; set; }
        /// <summary>
        /// 商品中分類總筆數
        /// </summary>
        public int? MTotal { get; set; }
        /// <summary>
        /// 商品中分類清單
        /// </summary>
        public List<PostItemCategoryMCategorysModel> MCategorys { get; set; }
        /// <summary>
        /// 商品小分類總筆數
        /// </summary>
        public int? STotal { get; set; }
        /// <summary>
        /// 商品小分類清單
        /// </summary>
        public List<PostItemCategorySCategorysModel> SCategorys { get; set; }
    }

    /// <summary>
    /// 商品大分類物件
    /// </summary>
    public class PostItemCategoryLCategorysModel
    {
        /// <summary>
        /// 商品大分類編號
        /// </summary>
        public string LCode { get; set; }
        /// <summary>
        /// 商品大分類名稱
        /// </summary>
        public string LName { get; set; }
        /// <summary>
        /// 商品抽驗比例(%)，預設為0
        /// </summary>
        public decimal? LPercent { get; set; }
    }

    /// <summary>
    /// 商品中分類物件
    /// </summary>
    public class PostItemCategoryMCategorysModel
    {
        /// <summary>
        /// 商品大分類編號
        /// </summary>
        public string LCode { get; set; }
        /// <summary>
        /// 商品中分類編號
        /// </summary>
        public string MCode { get; set; }
        /// <summary>
        /// 商品中分類名稱
        /// </summary>
        public string MName { get; set; }
        /// <summary>
        /// 商品抽驗比例(%)，預設為0
        /// </summary>
        public decimal? MPercent { get; set; }
    }

    /// <summary>
    /// 商品小分類物件
    /// </summary>
    public class PostItemCategorySCategorysModel
    {
        /// <summary>
        /// 商品大分類編號
        /// </summary>
        public string LCode { get; set; }
        /// <summary>
        /// 商品中分類編號
        /// </summary>
        public string MCode { get; set; }
        /// <summary>
        /// 商品小分類編號
        /// </summary>
        public string SCode { get; set; }
        /// <summary>
        /// 商品小分類名稱
        /// </summary>
        public string SName { get; set; }
        /// <summary>
        /// 商品抽驗比例(%)，預設為0
        /// </summary>
        public decimal? SPercent { get; set; }
    }

    public class PostItemCategoryLCategorysGroupModel
    {
        /// <summary>
        /// 同大分類代碼最後一筆
        /// </summary>
        public PostItemCategoryLCategorysModel LastData { get; set; }
        /// <summary>
        /// 同大分類代碼筆數
        /// </summary>
        public int Count { get; set; }
    }

    public class PostItemCategoryMCategorysGroupModel
    {
        /// <summary>
        /// 同中分類代碼最後一筆
        /// </summary>
        public PostItemCategoryMCategorysModel LastData { get; set; }
        /// <summary>
        /// 同中分類代碼筆數
        /// </summary>
        public int Count { get; set; }
    }

    public class PostItemCategorySCategorysGroupModel
    {
        /// <summary>
        /// 同小分類代碼最後一筆
        /// </summary>
        public PostItemCategorySCategorysModel LastData { get; set; }
        /// <summary>
        /// 同小分類代碼筆數
        /// </summary>
        public int Count { get; set; }
    }

    public class MCategorysModel
    {
        /// <summary>
        /// 商品大分類編號
        /// </summary>
        public string LCode { get; set; }
        /// <summary>
        /// 商品中分類編號
        /// </summary>
        public string MCode { get; set; }
    }

    public class SCategorysModel
    {
        /// <summary>
        /// 商品大分類編號
        /// </summary>
        public string LCode { get; set; }
        /// <summary>
        /// 商品中分類編號
        /// </summary>
        public string MCode { get; set; }
        /// <summary>
        /// 商品小分類編號
        /// </summary>
        public string SCode { get; set; }
    }
}
