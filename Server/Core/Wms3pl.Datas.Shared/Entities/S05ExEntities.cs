using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Entities
{

	public class OrderAllotParam
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string ExecSDate { get; set; }
		public string ExecEDate { get; set; }

	}

	public class OrderAlloct
	{
		public string DC_CODE { get; set; }
	  public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ORD_NO { get; set; }
		public string ORD_SEQ { get; set; }
		public string ITEM_CODE { get; set; }
   
		public int ORD_QTY { get; set; }
		public int A_DELV_QTY { get; set; }
		public string WMS_ORD_NO { get; set; }

		public DateTime APPROVE_DATE { get; set; }

	}

	public class PutAllotParam
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
	}

	public class ToPickLackRtnStockParam
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string EXECSDATE { get; set; }
		public string EXECEDATE { get; set; }
	}
}
