using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{

    #region 出貨匯入資料
    [DataContract]
    [Serializable]
    [DataServiceKey()]
    public class F050101ImportData
    {
        [DataMember] 
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public string TEL { get; set; }
        [DataMember]
        public string TEL_1 { get; set; }
        [DataMember]
        public string TEL_2 { get; set; }
        [DataMember]
        public string ADDRESS { get; set; }
        [DataMember]
        public string CONSIGNEE { get; set; }
        [DataMember]
        public string BATCH_NO { get; set; }
        [DataMember]
        public string CONTACT { get; set; }
        [DataMember]
        public string CONTACT_TEL { get; set; }
        [DataMember]
        public string MEMO { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string CUST_NAME { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public int ORD_QTY { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
        [DataMember]
        public string CRT_STAFF { get; set; }
        [DataMember]
        public string CRT_NAME { get; set; }
        [DataMember]
        public string NO_DELV { get; set; }
        [DataMember]
        public DateTime? ORD_DATE { get; set; }
        [DataMember]
        public DateTime ARRIVAL_DATE { get; set; }
        [DataMember]
        public Decimal COLLECT_AMT { get; set; }
        [DataMember]
        public string ALL_ID { get; set; }
         
    }
    #endregion
}
