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
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE","CUST_CODE","CHANNEL", "ALL_ID","CUSTOMER_ID","TMPR_TYPE","CONSIGN_TYPE")]
	public class F194712EX
	{
		public string DC_CODE { get; set; }
		public string DC_NAME { get; set; }
		public string GUP_CODE { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string CHANNEL { get; set; }
		public string CHANNEL_NAME { get; set; }
		public string ALL_ID { get; set; }
		public string ALL_COMP { get; set; }
		public string CUSTOMER_ID { get; set; }
		public string CONSIGN_TYPE { get; set; }
		public string CONSIGN_TYPE_NAME { get; set; }
		public int SAVE_QTY { get; set; }
		public int PATCH_QTY { get; set; }
		public int UNUSEDQTY { get; set; }
		public string ISTEST { get; set; }

	}
}
