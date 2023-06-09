using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	/// <summary>
	/// 集貨入場-入場檢核_傳入
	/// </summary>
	public class CheckContainerCodeForCollectionReq : StaffModel
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
		/// 集貨場編號
		/// </summary>
		public string CollectionCode { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 儲格類型
		/// </summary>
		public string CellType { get; set; }
	}

	/// <summary>
	/// 集貨入場-入場檢核_傳回
	/// </summary>
	public class CheckContainerCodeForCollectionRes
	{
		/// <summary>
		/// 儲格類型
		/// </summary>
		public string CellType { get; set; }
		/// <summary>
		/// 儲格編號
		/// </summary>
		public string CellCode { get; set; }
		/// <summary>
		/// 是否為第一箱
		/// </summary>
		public string IsFirst { get; set; }
		/// <summary>
		/// 訊息
		/// </summary>
		public string Info { get; set; }
	}

	/// <summary>
	/// 集貨入場-入場檢核_傳入
	/// </summary>
	public class ConfirmContainerCodeForCollectionReq : StaffModel
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
		/// 集貨場編號
		/// </summary>
		public string CollectionCode { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 儲格類型
		/// </summary>
		public string CellCode { get; set; }
	}

	/// <summary>
	/// 集貨出場-可出場查詢_傳入
	/// </summary>
	public class GetAvailableCellCodeDataReq : StaffModel
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
		/// 集貨場編號
		/// </summary>
		public string CollectionCode { get; set; }
		/// <summary>
		/// 儲格類型
		/// </summary>
		public string CellType { get; set; }
	}

	/// <summary>
	/// 集貨出場-可出場查詢_回傳
	/// </summary>
	public class GetAvailableCellCodeDataRes
	{
		/// <summary>
		/// 儲格編號
		/// </summary>
		public string CellCode { get; set; }
	}

	/// <summary>
	/// 集貨出場-出場確認_傳入
	/// </summary>
	public class ConfirmCellCodeReq : StaffModel
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
		/// 集貨場編號
		/// </summary>
		public string CollectionCode { get; set; }
		/// <summary>
		/// 儲格類型
		/// </summary>
		public string CellCode { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
	}
}
