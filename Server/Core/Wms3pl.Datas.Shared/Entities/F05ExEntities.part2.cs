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

    #region API 出貨確認(出貨覆檔)
    [Serializable]
	[DataContract]
	[DataServiceKey("ORD_NO")]
	public class ShipOrderConfirmInInfo
    {
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public decimal STATUS { get; set; }
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
        public int QTY { get; set; }
        [DataMember]
        public int PACKAGE_QTY { get; set; }
    }
    #endregion



	public class WmsOrdStatus
	{
		public string DcCode { get; set; }
		public string WmsOrdNo { get; set; }
		public decimal Status { get; set; }
	}
}
