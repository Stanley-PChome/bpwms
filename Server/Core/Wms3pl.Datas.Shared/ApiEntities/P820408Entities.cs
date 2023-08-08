using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 容器釋放中介_傳入
	/// </summary>
	public class WcsContainerReq
	{
		/// <summary>
		/// 業主編號=貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 筆數
		/// </summary>
		public int ContainerTotal { get; set; }
		/// <summary>
		/// 容器列表
		/// </summary>
		public List<WcsContainerModel> ContainerList { get; set; }
	}

	/// <summary>
	/// 明細資料
	/// </summary>
	public class WcsContainerModel
	{
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
	}

	/// <summary>
	/// 容器釋放中介_回傳
	/// </summary>
	public class WcsContainerResData
	{
		/// <summary>
		/// 錯誤容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 錯誤欄位
		/// </summary>
		public string ErrorColumn { get; set; }
		/// <summary>
		/// AGV錯誤回應
		/// </summary>
		public List<WcsErrorModel> errors { get; set; }
	}
}
