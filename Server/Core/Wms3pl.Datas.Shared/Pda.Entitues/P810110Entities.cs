namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	/// <summary>
	/// 進倉驗收查詢_傳入
	/// </summary>
	public class GetTransferStockReceivedDataReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
	}

	/// <summary>
	/// 進倉驗收查詢_傳回
	/// </summary>
	public class GetTransferStockReceivedDataRes
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 進倉單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 明細序號
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		public string ItemName { get; set; }
		/// <summary>
		/// 商品條碼1
		/// </summary>
		public string EanCode1 { get; set; }
		/// <summary>
		/// 商品條碼2
		/// </summary>
		public string EanCode2 { get; set; }
		/// <summary>
		/// 商品條碼3
		/// </summary>
		public string EanCode3 { get; set; }
		/// <summary>
		/// 序號清單
		/// </summary>
		public string SnList { get; set; }
		/// <summary>
		/// 數量
		/// </summary>
		public int ItemQty { get; set; }
	}

	/// <summary>
	/// 進倉驗收檢核_傳入
	/// </summary>
	public class ConfirmTransferStockReceivedDataReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 商品條碼(品號、國條1~3、序號)
		/// </summary>
		public string ItemCode { get; set; }
	}

	/// <summary>
	/// 進倉上架查詢_傳入
	/// </summary>
	public class GetTransferStockInDataReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
	}

	/// <summary>
	/// 進倉上架查詢_傳入
	/// </summary>
	public class GetTransferStockInDataRes
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 倉別名稱
		/// </summary>
		public string WarehouseName { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string LocCode { get; set; }
	}

	/// <summary>
	/// 進倉上架檢核_傳入
	/// </summary>
	public class ConfirmTransferStockInDataReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string LocCode { get; set; }
	}
}
