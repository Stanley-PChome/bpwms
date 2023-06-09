using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 門市主檔_傳入
    /// </summary>
    public class PostRetailDataReq
    {
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        public PostRetailDataResultModel Result { get; set; }
    }

    public class PostRetailDataResultModel
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        public int? Total { get; set; }
        /// <summary>
        /// 門市資料物件清單
        /// </summary>
        public List<PostRetailDataSalesModel> Sales { get; set; }
    }

    /// <summary>
    /// 門市資料物件
    /// </summary>
    public class PostRetailDataSalesModel
    {
        /// <summary>
        /// 門市編號
        /// </summary>
        public string SalesBaseNo { get; set; }
        /// <summary>
        /// 門市名稱
        /// </summary>
        public string SalesBaseName { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 聯絡郵件信箱
        /// </summary>
        public string Mail { get; set; }
        /// <summary>
        /// 聯絡地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 門市簡稱
        /// </summary>
        public string ShortSalesBaseName { get; set; } = null;
        /// <summary>
        /// 統一編號
        /// </summary>
        public string UnifiedBusinessNo { get; set; } = null;
        /// <summary>
        /// 門市群組
        /// </summary>
        public string SalesBaseGroup { get; set; } = null;
    }

    public class PostRetailSalesGroupModel
    {
        /// <summary>
        /// 同門市編號最後一筆
        /// </summary>
        public PostRetailDataSalesModel LastData { get; set; }
        /// <summary>
        /// 同門市編號筆數
        /// </summary>
        public int Count { get; set; }
    }
}
