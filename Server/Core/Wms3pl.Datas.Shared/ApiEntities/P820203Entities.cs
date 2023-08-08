using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 批次客戶退貨單資料_傳入
    /// </summary>
    public class PostCreateReturnsReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        public PostCreateReturnsResultModel Result { get; set; }
    }

    public class PostCreateReturnsResultModel
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        public int? Total { get; set; }
        /// <summary>
        /// 退貨單主檔
        /// </summary>
        public List<PostCreateReturnsModel> Returns { get; set; }
    }

    /// <summary>
    /// 退貨單主檔
    /// </summary>
    public class PostCreateReturnsModel
    {
        /// <summary>
        /// 貨主退貨單號(唯一值)
        /// </summary>
        public string CustReturnNo { get; set; }
        /// <summary>
        /// 單據日期
        /// </summary>
        public DateTime? ReturnDate { get; set; }
        /// <summary>
        /// 單據類型(0:B2B 1:B2C)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 物流中心出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
        /// <summary>
        /// 退貨客戶編號
        /// </summary>
        public string ReturnCustCode { get; set; }
        /// <summary>
        /// 退貨類型
        /// </summary>
        public string ReturnType { get; set; }
        /// <summary>
        /// 退貨原因
        /// </summary>
        public string ReturnCause { get; set; }
        /// <summary>
        /// 批次編號
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 郵遞區號
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string PhoneNo { get; set; }
        /// <summary>
        /// 貨主自訂分類
        /// </summary>
        public string CustCategory { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 狀態(0:待處理 ;D:刪除)
        /// </summary>
        public string ProcFlag { get; set; }
        /// <summary>
        /// 進倉單明細
        /// </summary>
        public List<PostCreateReturnsDetailModel> ReturnDetails { get; set; } = new List<PostCreateReturnsDetailModel>();
    }

    /// <summary>
    /// 退貨單明細
    /// </summary>
    public class PostCreateReturnsDetailModel
    {
        /// <summary>
        /// 品項編號
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 通路品號項次
        /// </summary>
        public string ItemSeq { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int Qty { get; set; }
    }

    public class ThirdPartReturns
    {
        public string RETURN_NO { get; set; }
        public string CUST_ORD_NO { get; set; }
    }
}
