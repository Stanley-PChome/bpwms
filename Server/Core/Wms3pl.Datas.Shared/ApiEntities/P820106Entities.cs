using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 供應商主檔_傳入
    /// </summary>
    public class PostVendorDataReq
    {
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        public PostVendorDataResultModel Result { get; set; }
    }

    public class PostVendorDataResultModel
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        public int? Total { get; set; }
        /// <summary>
        /// 供應商資料物件清單
        /// </summary>
        public List<PostVendorDataVendorsModel> Vendors { get; set; }
    }

    /// <summary>
    /// 供應商資料物件
    /// </summary>
    public class PostVendorDataVendorsModel
    {
        /// <summary>
        /// 供應商編號
        /// </summary>
        public string VnrCode { get; set; }
        /// <summary>
        /// 供應商名稱
        /// </summary>
        public string VnrName { get; set; }
        /// <summary>
        /// 狀態(0:使用中、9:刪除)
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string UniForm { get; set; } = null;
        /// <summary>
        /// 負責人
        /// </summary>
        public string Boss { get; set; } = null;
        /// <summary>
        /// 廠商電話
        /// </summary>
        public string Tel { get; set; } = null;
        /// <summary>
        /// 郵遞區號
        /// </summary>
        public string Zip { get; set; } = null;
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = null;
        /// <summary>
        /// 貨物聯絡人
        /// </summary>
        public string ItemContact { get; set; } = null;
        /// <summary>
        /// 貨物聯絡人電話
        /// </summary>
        public string ItemTel { get; set; } = null;
        /// <summary>
        /// 貨物聯絡人手機
        /// </summary>
        public string ItemCel { get; set; } = null;
        /// <summary>
        /// 貨物聯絡人信箱
        /// </summary>
        public string ItemMail { get; set; } = null;
        /// <summary>
        /// 帳務聯絡人
        /// </summary>
        public string BillContact { get; set; } = null;
        /// <summary>
        /// 帳務聯絡人電話
        /// </summary>
        public string BillTel { get; set; } = null;
        /// <summary>
        /// 帳務聯絡人手機
        /// </summary>
        public string BillCel { get; set; } = null;
        /// <summary>
        /// 帳務聯絡人信箱
        /// </summary>
        public string BillMail { get; set; } = null;
        /// <summary>
        /// 發票郵遞區號
        /// </summary>
        public string InvZip { get; set; } = null;
        /// <summary>
        /// 發票地址
        /// </summary>
        public string InvAddress { get; set; } = null;
        /// <summary>
        /// 稅別(0:否、1:是)
        /// </summary>
        public string TaxType { get; set; }
        /// <summary>
        /// 抽驗比例(%)
        /// </summary>
        public decimal? Checkpercent { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Memo { get; set; }

		    /// <summary>
				/// 配送方式(0:自取;1:宅配)
				/// </summary>
		    public string DeliveryWay { get; set; }

		}

    public class PostVendorGroupModel
    {
        /// <summary>
        /// 同供應商編號最後一筆
        /// </summary>
        public PostVendorDataVendorsModel LastData { get; set; }
        /// <summary>
        /// 同供應商編號筆數
        /// </summary>
        public int Count { get; set; }
    }
}
