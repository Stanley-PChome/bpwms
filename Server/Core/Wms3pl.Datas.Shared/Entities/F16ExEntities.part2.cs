using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
    #region API 退貨回傳檔(退貨回覆檔)
    [Serializable]
    [DataContract]
    [DataServiceKey("RETURN_NO")]
    public class RetOrderConfirmInInfo
    {
        [DataMember]
        public string RETURN_NO { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string DELV_DATE { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public int MOVED_QTY { get; set; }
    }
    #endregion


    #region API 廠退確認(廠退覆檔)
    [Serializable]
    [DataContract]
    [DataServiceKey("RTN_VNR_NO")]
    public class RtnOrderConfirmInInfo
    {
        [DataMember]
        public string RTN_VNR_NO { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string NAME { get; set; }
        [DataMember]
        public string DELV_DATE { get; set; }
        [DataMember]
        public string ALL_ID { get; set; }
        [DataMember]
        public string CONSIGN_NO { get; set; }
        [DataMember]
        public string PACKAGENO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
        [DataMember]
        public string QTY { get; set; }
        [DataMember]
        public string PACKAGE_QTY { get; set; }
    }
    #endregion
}
