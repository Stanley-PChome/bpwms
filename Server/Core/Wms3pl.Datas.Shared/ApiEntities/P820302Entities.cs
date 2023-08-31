using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	public class SaleOrderReplyReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		public List<SaleOrderReplyWarehouseOut> WarehouseOuts { get; set; } = new List<SaleOrderReplyWarehouseOut>();
	}

	public class SaleOrderReplyWarehouseOut
	{
		/// <summary>
		/// 貨主出貨單號
		/// </summary>
		public string CustOrdNo { get; set; }
		/// <summary>
		/// 單號類型
		/// </summary>
		public string OrderType { get; set; }
		/// <summary>
		/// 處理狀態
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 作業時間
		/// </summary>
		public string WorkTime { get; set; }
		/// <summary>
		/// 實際出貨箱數
		/// </summary>
		public int? TotalBoxNum { get; set; }
		public List<SaleOrderReplyWarehouseOutDetail> WarehouseOutDetails { get; set; } = new List<SaleOrderReplyWarehouseOutDetail>();
		public List<SaleOrderReplyPackage> Packages { get; set; } = new List<SaleOrderReplyPackage>();
		public List<LackDetail> LackDetails { get; set; } = new List<LackDetail>();
	}

	public class SaleOrderReplyWarehouseOutDetail
	{
		/// <summary>
		/// 原單據的品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 實際出貨數量
		/// </summary>
		public int ActQty { get; set; }
   
    }

    public class SaleOrderReplyPackage
	{
		/// <summary>
		/// 原進倉單的品號項次
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 箱數編號
		/// </summary>
		public int? BoxNo { get; set; }
		/// <summary>
		/// 箱號
		/// </summary>
		public string BoxNum { get; set; }
		/// <summary>
		/// 宅配單號
		/// </summary>
		public string TransportCode { get; set; }
		/// <summary>
		/// 物流商編號
		/// </summary>
		public string TransportProvider { get; set; }
		/// <summary>
		/// 出貨時間=包裹過刷扣帳時間
		/// </summary>
		public string ShipmentTime { get; set; }
		public List<SaleOrderReplyPackageDetail> Details { get; set; } = new List<SaleOrderReplyPackageDetail>();
	}

	public class SaleOrderReplyPackageDetail
	{
		/// <summary>
		/// 原單據的品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 出貨商品數量
		/// </summary>
		public int OutQty { get; set; }
		public List<SaleOrderReplyPackageMakeNoDetail> MakeNoDetails { get; set; } = new List<SaleOrderReplyPackageMakeNoDetail>();
	}

	public class SaleOrderReplyPackageMakeNoDetail
	{
		/// <summary>
		/// 商品批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 商品批號數量
		/// </summary>
		public int MakeNoQty { get; set; }
		/// <summary>
		/// 本商品的序號清單
		/// </summary>
		public List<string> SnList { get; set; } = new List<string>();

		/// <summary>
		/// 效期
		/// </summary>
		public String ValidDate { get; set; }

	}

	public class LackDetail
	{
		/// <summary>
		/// 原單據的品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 揀缺數量
		/// </summary>
		public int LackQty { get; set; }
		/// <summary>
		/// 揀缺原因
		/// </summary>
		public string LackCause { get; set; }
		/// <summary>
		/// 揀缺原因備註
		/// </summary>
		public string LackCauseMemo { get; set; }
	}
}
