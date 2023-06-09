using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
    #region 排程-GoHappy 庫存更新API
    [Serializable]
    [DataContract]
    [DataServiceKey("ITEM_CODE", "SIZEID", "PRODUCTID")]
    public class UpdateStockAPI
    {
        [DataMember]
        public string VNR_CODE { get; set; }
        [DataMember]
        public string VNR_NAME { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string SIZEID { get; set; }
        [DataMember]
        public string PRODUCTID { get; set; }
        [DataMember]
        public int QTY { get; set; }
        [DataMember]
        public int SAVEQTY { get; set; }
        [DataMember]
        public string ApiKey { get; set; }
        [DataMember]
        public string Securecode { get; set; }
    }
    #endregion

    #region 排程-GoHappy 轉出Excel使用
    [Serializable]
    [DataContract]
    [DataServiceKey("ITEM_CODE", "SIZEID", "PRODUCTID")]
    public class GoHappyStockToExcel
    {
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string SIZEID { get; set; }
        [DataMember]
        public string PRODUCTID { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string TYPE_NAME { get; set; }
        [DataMember]
        public int QTY { get; set; }
    }
    #endregion

    #region 排程-檢查宅配通可用單號量是否足夠使用
    [Serializable]
    [DataContract]
    [DataServiceKey("ITEM_CODE", "SIZEID", "PRODUCTID")]
    public class F194711EX
    {
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string ALL_ID { get; set; }
        [DataMember]
        public string MEMO { get; set; }
        [DataMember]
        public Decimal COUNTQTY { get; set; }
    }
    #endregion

}
