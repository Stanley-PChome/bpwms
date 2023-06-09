using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{

	public class OrderStockChange
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }

		public string CustCode { get; set; }
		
		public string WmsNo { get; set; }

		public string LocCode { get; set; }

		public string ItemCode { get; set; }

		public DateTime VaildDate { get; set; }
		public DateTime EnterDate { get; set; }
		public string MakeNo { get; set; }
		public string SerialNo { get; set; }
		public string VnrCode { get; set; }
		public string BoxCtrlNo { get; set; }
    public string PalletCtrlNo { get; set; }
		public int Qty { get; set; }

	}

	public class StockChange
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }

		public string CustCode { get; set; }

		public string LocCode { get; set; }

		public string ItemCode { get; set; }

		public DateTime ValidDate { get; set; }
		public DateTime EnterDate { get; set; }
		public string MakeNo { get; set; }
		public string SerialNo { get; set; }
		public string VnrCode { get; set; }
		public string BoxCtrlNo { get; set; }
		public string PalletCtrlNo { get; set; }
		public long Qty { get; set; }
	}
}
