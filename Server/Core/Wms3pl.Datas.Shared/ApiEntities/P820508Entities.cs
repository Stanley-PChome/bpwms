using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 每日庫存快照回傳_傳入
	/// </summary>
	public class SearchUsableContainerReceiptReq
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
		/// 列表筆數
		/// </summary>
		public string ContainerTotal { get; set; }
		
		/// <summary>
		/// 庫存明細
		/// </summary>
		public List<SearchUsableContainerReceiptContainerModel> ContainerList { get; set; } = new List<SearchUsableContainerReceiptContainerModel>();
	}

	/// <summary>
	/// (容器列表)
	/// </summary>
	public class SearchUsableContainerReceiptContainerModel
	{
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		
	}

	public class WcsApiSearchUsableContainerReceiptResultData
	{
		public string ContainerCode { get; set; }
		public int? Status { get; set; }
		public string ErrorColumn { get; set; }
		public WcsApiErrorResult errors { get; set; }
	}
	
}
