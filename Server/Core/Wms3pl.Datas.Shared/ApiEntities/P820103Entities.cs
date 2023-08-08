using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 商品組合主檔_傳入
    /// </summary>
    public class PostItemBomReq
    {
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        public PostItemBomResultModel Result { get; set; }
    }

    public class PostItemBomResultModel
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        public int? Total { get; set; }
        /// <summary>
        /// 商品組合資料物件清單
        /// </summary>
        public List<PostItemBomModel> Boms { get; set; }
    }

    /// <summary>
    /// 門市資料物件
    /// </summary>
    public class PostItemBomModel
    {
        /// <summary>
        /// 組合編號
        /// </summary>
        public string BomCode { get; set; }
        /// <summary>
        /// 組合名稱
        /// </summary>
        public string BomName { get; set; }
        /// <summary>
        /// 組合類別(0組合 1拆解)
        /// </summary>
        public string BomType { get; set; }
        /// <summary>
        /// 抽驗比例
        /// </summary>
        public float? ChkPercent { get; set; }
        /// <summary>
        /// 是否加工(0:不加工,1:加工)
        /// </summary>
        public string IsProcess { get; set; }
        /// <summary>
        /// 成品編號(組合成品)
        /// </summary>
        public string FgCode { get; set; }
        /// <summary>
        /// 規格說明
        /// </summary>
        public string SpecDesc { get; set; } = null;
        /// <summary>
        /// 包裝敘述
        /// </summary>
        public string PackDescr { get; set; } = null;
        /// <summary>
        /// 商品單位編號
        /// </summary>
        public string UnitId { get; set; }
        /// <summary>
        /// 組合商品狀態(0使用中 9刪除)
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 明細
        /// </summary>
        public List<PostItemBomDetailModel> BomDetails { get; set; }
    }

    /// <summary>
    /// 明細
    /// </summary>
    public class PostItemBomDetailModel
    {
        /// <summary>
        /// 組合順序
        /// </summary>
        public short? CombinSeq { get; set; }
        /// <summary>
        /// 組合單品編號(組合原料)
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 組合單品數量(組合原料數量)
        /// </summary>
        public int Qty { get; set; } = 0;
    }

    public class PostItemBomGroupModel
    {
        public PostItemBomModel LastData { get; set; }
        public int Count { get; set; }
    }
}
