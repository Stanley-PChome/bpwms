using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{




    [Serializable]
    [DataContract]
    [DataServiceKey("COLLECT_NO")]
    public class F010301Main
    {
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string COLLECT_NO { get; set; }
        [DataMember]
        public string STOCK_NO { get; set; }
        [DataMember]
        public string VNR_CODE { get; set; }
        [DataMember]
        public string VNR_NAME { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public DateTime CRT_DATE { get; set; }
        [DataMember]
        public string CRT_STAFF { get; set; }
    }

    

    [Serializable]
    [DataContract]
    [DataServiceKey("box_no")]
    public class F010302Detail
    {
       
        [DataMember]
        public string box_no { get; set; }
        [DataMember]
        public string item_code { get; set; }
        [DataMember]
        public string loc_code { get; set; }
        [DataMember]
        public string collect_status { get; set; }
        [DataMember]
        public int collect_qty { get; set; }
        
    }



    [Serializable]
    [DataContract]
    [DataServiceKey("Collect_Seq")]
    public class F010302CollectDetail
    {
        [DataMember]
        public int Collect_Seq { get; set; }
        [DataMember]
        public string Collect_no { get; set; }
        [DataMember]
        public string Collect_Date { get; set; }
        [DataMember]
        public string Collect_Time { get; set; }
        [DataMember]
        public string Loz_code { get; set; }
        [DataMember]
        public string Loc_code { get; set; }
        [DataMember]
        public string Box_no { get; set; }
        [DataMember]
        public string Item_code { get; set; }
        [DataMember]
        public string Collect_status { get; set; }
        [DataMember]
        public string Collect_status_name { get; set; }
        [DataMember]
        public int Collect_qty { get; set; }
        [DataMember]
        public string Collect_mk { get; set; }
    }



    #region API 出貨確認(出貨覆檔)
    [Serializable]
    [DataContract]
    [DataServiceKey("STOCK_NO")]
    public class RecOrderConfirmInInfo
    {
        [DataMember]
        public string STOCK_NO { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public decimal STATUS { get; set; }
        [DataMember]
        public string Check_STATUS { get; set; }
        [DataMember]
        public string DELV_DATE { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public int SumRECV_QTY { get; set; }
        [DataMember]
        public string CRT_DATE { get; set; }
    }
    #endregion

}
