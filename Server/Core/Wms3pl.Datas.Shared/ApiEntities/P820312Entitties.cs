using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	public class P820312Entitties
	{
	}
	public class VnrReturnReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<VnrReturnOrder> Orders { get; set; }
	}

	public class VnrReturnOrder
	{
		/// <summary>
		/// 廠退出貨單號
		/// </summary>
		public string WmsOrdNo { get; set; }
		/// <summary>
		/// 配送方式
		/// </summary>
		public int DeliveryType { get; set; }
		/// <summary>
		/// 指定物流商
		/// </summary>
		public string ShipProvider { get; set; }
		/// <summary>
		/// 指定物流箱數
		/// </summary>
		public int? ShipBoxNo { get; set; }
		/// <summary>
		/// 自取單號
		/// </summary>
		public string ShipSelfMemo { get; set; }
		public List<VnrReturnOrderDetail> VnrReturns { get; set; }

	}

	public class VnrReturnOrderDetail
	{
		/// <summary>
		/// 貨主單號
		/// </summary>
		public string CustVnrReturnNo { get; set; }
	}

	public class VnrReturnOrderPrintReq
	{
		public string MacAddr { get; set; }
		public string Username { get; set; }
		public int? Sno { get; set; } = null;
		public int RePrint { get; set; } = 1;
	}

	public class VnrReturnOrderPrintTestReq
	{
		public string macAddr { get; set; }
		public string username { get; set; }
		public int? sno { get; set; } = null;
		public int rePrint { get; set; } = 1;
	}
}
