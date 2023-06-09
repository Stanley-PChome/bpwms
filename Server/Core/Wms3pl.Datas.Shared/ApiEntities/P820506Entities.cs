using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 每日庫存快照回傳_傳入
	/// </summary>
	public class ShipToDebitReceiptReq
	{
		/// <summary>
		/// 目的倉庫編號=物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主編號=WMS貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 列表筆數
		/// </summary>
		public int? BoxTotal { get; set; }
		/// <summary>
		/// 箱列表
		/// </summary>
		public List<ShipToDebitReceiptBoxModel> BoxList { get; set; } = new List<ShipToDebitReceiptBoxModel>();
	}

	/// <summary>
	///箱列表
	/// </summary>
	public class ShipToDebitReceiptBoxModel
	{
		/// <summary>
		/// 物流單號
		/// </summary>
		public string ShipCode { get; set; }
		/// <summary>
		/// 物流商編號
		/// </summary>
		public string ShipProvider { get; set; }
		/// <summary>
		/// 分揀機編號
		/// </summary>
		public string SorterCode { get; set; }
		/// <summary>
		/// 目的流道口
		/// </summary>
		public string PortNo { get; set; }
		/// <summary>
		/// 紙箱型號
		/// </summary>
		public string BoxCode { get; set; }
		/// <summary>
		/// 紙箱長度
		/// </summary>
		public decimal? BoxLength { get; set; }
		/// <summary>
		/// 紙箱寬度
		/// </summary>
		public decimal? BoxWidth { get; set; }
		/// <summary>
		/// 紙箱高度
		/// </summary>
		public decimal? BoxHeight { get; set; }
		/// <summary>
		/// 紙箱重量
		/// </summary>
		public decimal? BoxWeight { get; set; }
		/// <summary>
		/// 紀錄時間(yyyy/MM/dd HH:mm:ss)
		/// </summary>
		public string CreateTime { get; set; }
	}

	public class WcsApiShipToDebitReceiptResultData
	{
		public string ShipCode { get; set; }
		public string ErrorColumn { get; set; }
		public WcsApiErrorResult errors { get; set; }
	}
	
	/// <summary>
	/// 容器位置回報_傳入
	/// </summary>
	public class ContainerPositionReceiptReq
	{
		/// <summary>
		/// 業主編號=WMS貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 目的倉庫編號=物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 容器類型(00=不限, 01=M-周轉箱, 02=2L周轉箱)
		/// </summary>
		public string ContainerType { get; set; }
		/// <summary>
		/// 目前抵達位置(配箱站編號、集貨出庫位置)
		/// </summary>
		public string PositionCode { get; set; }
		/// <summary>
		/// 目的位置(配箱站編號)
		/// </summary>
		public string TargetPosCode { get; set; }
		/// <summary>
		/// 紀錄時間(yyyy/MM/dd HH:mm:ss)
		/// </summary>
		public string CreateTime { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string OriOrderCode { get; set; }
		/// <summary>
		/// 單號容器總箱數
		/// </summary>
		public int? BoxTotal { get; set; }
		/// <summary>
		/// 單號容器箱序
		/// </summary>
		public int? BoxSerial { get; set; }

	}
	public class WcsApiContainerPositionReceiptResultData
	{
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 錯誤欄位
		/// </summary>
		public string ErrorColumn { get; set; }
		/// <summary>
		/// 錯誤回應
		/// </summary>
		public WcsApiErrorResult errors { get; set; }
	}
}
