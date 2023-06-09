using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Entities
{
	#region P2001010000 異動調整作業(訂單,商品,盤點庫存)
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F200101Data
	{
		public int ROWNUM { get; set; }
		public string ADJUST_NO { get; set; }

		public string ADJUST_TYPE { get; set; }

		public string ADJUST_TYPE_NAME { get; set; }

		public string WORK_TYPE { get; set; }

		public string WORK_TYPE_NAME { get; set; }

		public int IsCanEdit { get; set; }

		public DateTime ADJUST_DATE { get; set; }

		public string CRT_STAFF { get; set; }

		public string CRT_NAME { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }

		public string UPD_NAME { get; set; }
		public DateTime? UPD_DATE { get; set; }

		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }

		public string CUST_CODE { get; set; }
		public string SOURCE_TYPE { get; set; }
		public string SOURCE_NAME { get; set; }
		public string SOURCE_NO { get; set; }

	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F200102Data
	{
		public int ROWNUM { get; set; }
		public string ADJUST_NO { get; set; }
		public int ADJUST_SEQ { get; set; }

		public DateTime? DELV_DATE { get; set; }

		public string PICK_TIME { get; set; }

		public string CUST_ORD_NO { get; set; }

		public string ORG_PICK_TIME { get; set; }

		public string ALL_ID { get; set; }

		public string ALL_COMP { get; set; }


		public string ADDRESS { get; set; }

		public string NEW_DC_CODE { get; set; }

		public string NEW_DC_NAME { get; set; }

		public string CAUSE { get; set; }

		public string CAUSENAME { get; set; }

		public string CAUSE_MEMO { get; set; }

		public string UPD_STAFF { get; set; }
		public string UPD_NAME { get; set; }

		public DateTime? UPD_DATE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }

		public string CUST_CODE { get; set; }

		public string CHECKOUT_TIME { get; set; }

		public string ORD_NO { get; set; }

		public string CRT_STAFF { get; set; }
		public string CRT_NAME { get; set; }
		public DateTime CRT_DATE { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F200103Data
	{
		public int ROWNUM { get; set; }
		public string ADJUST_NO { get; set; }
		public int ADJUST_SEQ { get; set; }

		public string WAREHOUSE_ID { get; set; }

		public string WAREHOUSE_NAME { get; set; }

		public string ITEM_CODE { get; set; }

		public string ITEM_NAME { get; set; }

		public string LOC_CODE { get; set; }

		public string ITEM_SIZE { get; set; }

		public string ITEM_SPEC { get; set; }

		public string ITEM_COLOR { get; set; }

		public DateTime VALID_DATE { get; set; }

		public int ITEM_QTY { get; set; }

		public int ADJ_QTY_IN { get; set; }

		public int ADJ_QTY_OUT { get; set; }

		public string CAUSE { get; set; }

		public string CAUSENAME { get; set; }

		public string CAUSE_MEMO { get; set; }

		public DateTime? UPD_DATE { get; set; }

		public string UPD_NAME { get; set; }

		public string UPD_STAFF { get; set; }

		public string DC_CODE { get; set; }

		public string DC_NAME { get; set; }

		public string GUP_CODE { get; set; }

		public string CUST_CODE { get; set; }

		public DateTime ENTER_DATE { get; set; }

		public string VNR_CODE { get; set; }

		public string VNR_NAME { get; set; }

		public string BUNDLE_SERIALNO { get; set; }

		public string WORK_TYPE { get; set; }

		public string MAKE_NO { get; set; }
	}

	#endregion
}
