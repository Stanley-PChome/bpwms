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
    #region F161201RtnData 退貨API回傳資料
    [DataContract]
	[Serializable]
	[DataServiceKey("RETURN_NO")]
	public class F161201RtnData
	{
        [DataMember]
        public string RETURN_NO { get; set; }
        [DataMember]
		public string COST_CENTER { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string EXCHANGEID { get; set; }
		[DataMember]
		public string ITEM_MAIL { get; set; }
		[DataMember]
		public string BILL_MAIL { get; set; }
	}
    #endregion

    [DataContract]
    [Serializable]
    [DataServiceKey("RETURN_NO")]
    public class F161202RtnData
    {
        [DataMember]
        public string RETURN_NO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string RETURN_SEQ { get; set; }
    }

    #region F1602ImportData 廠退匯入資料
    [DataContract]
    [Serializable]
    [DataServiceKey("DC_CODE,VNR_CODE")]
    public class F1602ImportData
    {
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string ORD_PROP { get; set; }
        [DataMember]
        public string RTN_VNR_TYPE_ID { get; set; }
        [DataMember]
        public string RTN_VNR_CAUSE { get; set; }
        [DataMember]
        public string VNR_CODE { get; set; }
        [DataMember]
        public string MEMO { get; set; }
        [DataMember]
        public string WAREHOUSE_ID { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string LOC_CODE { get; set; }
        [DataMember]
        public int RTN_VNR_QTY { get; set; }
    }
    #endregion

}
