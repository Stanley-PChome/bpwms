using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	/// <summary>
	/// 廠退便利倉-進場檢核_傳入
	/// </summary>
	public class VnrConvenientBookinReq : StaffModel
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
		/// 便利倉編號
		/// </summary>
		public string ConvenientCode { get; set; }
		/// <summary>
		/// 廠退出貨單號
		/// </summary>
		public string RtnWmsNo { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-進場檢核_傳回
	/// </summary>
	public class VnrConvenientBookinRes
	{
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 廠商名稱
		/// </summary>
		public string VnrName { get; set; }
		/// <summary>
		/// 廠退出貨單號
		/// </summary>
		public string VnrWmsNo { get; set; }
		/// <summary>
		/// 預約儲格編號
		/// </summary>
		public string BookinCellCode { get; set; }
		/// <summary>
		/// 儲格清單
		/// </summary>
		public List<VnrConvenientBookinCellModel> CellList { get; set; }
	}

	public class VnrConvenientBookinCellModel
	{
		/// <summary>
		/// 儲格編號
		/// </summary>
		public string CellCode { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-進場確認_傳入
	/// </summary>
	public class VnrConvenientBookinConfirmReq : StaffModel
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
		/// 便利倉編號
		/// </summary>
		public string ConvenientCode { get; set; }
		/// <summary>
		/// 廠退出貨單號
		/// </summary>
		public string RtnWmsNo { get; set; }
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 使用者刷入的便利倉儲格編號
		/// </summary>
		public string CellCode { get; set; }
		/// <summary>
		/// 系統預約的便利倉儲格編號
		/// </summary>
		public string BookinCellCode { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-出場查詢_傳入
	/// </summary>
	public class VnrConvenientOutQueryReq : StaffModel
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
		/// 便利倉編號
		/// </summary>
		public string ConvenientCode { get; set; }
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-出場查詢_傳回
	/// </summary>
	public class VnrConvenientOutQueryRes
	{
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 廠商名稱
		/// </summary>
		public string VnrName { get; set; }
		/// <summary>
		/// 訊息
		/// </summary>
		public string Info { get; set; }
		/// <summary>
		/// 儲格清單
		/// </summary>
		public List<VnrConvenientOutQueryCellModel> CellList { get; set; }
	}

	public class VnrConvenientOutQueryCellModel
	{
		/// <summary>
		/// 儲格編號
		/// </summary>
		public string CellCode { get; set; }
		/// <summary>
		/// 廠退出貨單清單
		/// </summary>
		public List<VnrConvenientOutQueryVnrWmsNoModel> WmsNoList { get; set; }
	}

	public class VnrConvenientOutQueryVnrWmsNoModel
	{
		/// <summary>
		/// 廠退出貨單號
		/// </summary>
		public string VnrWmsNo { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-出場儲格明細_傳入
	/// </summary>
	public class VnrConvenientOutCellDetailReq : StaffModel
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
		/// 便利倉編號
		/// </summary>
		public string ConvenientCode { get; set; }
		/// <summary>
		/// 儲格編號
		/// </summary>
		public string CellCode { get; set; }
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-出場儲格明細_傳回
	/// </summary>
	public class VnrConvenientOutCellDetailRes
	{
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 廠商名稱
		/// </summary>
		public string VnrName { get; set; }
		/// <summary>
		/// 儲格編號
		/// </summary>
		public string CellCode { get; set; }
		/// <summary>
		/// 訊息
		/// </summary>
		public string Info { get; set; }
		/// <summary>
		/// 廠退出貨單清單
		/// </summary>
		public List<VnrConvenientOutCellDetailWmsNoModel> WmsNoList { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-出場儲格明細_廠退出貨單_傳回
	/// </summary>
	public class VnrConvenientOutCellDetailWmsNoModel
	{
		/// <summary>
		/// 廠退出貨單號
		/// </summary>
		public string VnrWmsNo { get; set; }
		/// <summary>
		/// 貨主單號
		/// </summary>
		public string CustOrdNo { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-出場確認_傳入
	/// </summary>
	public class VnrConvenientOutConfirmReq : StaffModel
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
		/// 便利倉編號
		/// </summary>
		public string ConvenientCode { get; set; }
		/// <summary>
		/// 儲格編號
		/// </summary>
		public string CellCode { get; set; }
		/// <summary>
		/// 廠退出貨單號
		/// </summary>
		public string RtnWmsNo { get; set; }
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
	}

	/// <summary>
	/// 廠退便利倉-出場確認_傳回
	/// </summary>
	public class VnrConvenientOutConfirmRes
	{
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 廠商名稱
		/// </summary>
		public string VnrName { get; set; }
		/// <summary>
		/// 是否釋放儲格
		/// </summary>
		public bool IsReleaseCell { get; set; }
		/// <summary>
		/// 訊息
		/// </summary>
		public string Info { get; set; }
	}
}
