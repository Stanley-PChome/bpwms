using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	#region 進貨容器待關箱資料查詢(RecvNotCloseBindContainerQuery)
	/// <summary>
	/// 進貨容器待關箱資料查詢_傳入
	/// </summary>
	public class RecvNotCloseBindContainerQueryReq : StaffModel
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
		/// 容器條碼
		/// </summary>
		public string ContainerCode { get; set; }
	}

	/// <summary>
	/// 進貨容器待關箱資料查詢_傳出
	/// </summary>
	public class RecvNotCloseBindContainerQueryRes
	{
		/// <summary>
		/// 進貨容器流水號
		/// </summary>
		public long F020501_ID { get; set; }

		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupCode { get; set; }

		/// <summary>
		/// 上架區域
		/// </summary>
		public string CustCode { get; set; }

		/// <summary>
		/// 上架區域
		/// </summary>
		public string TypeCode { get; set; }

		/// <summary>ccc
		/// 上架區域名稱
		/// </summary>
		public string TypeName { get; set; }

		/// <summary>
		/// 容器條碼
		/// </summary>
		public string ContainerCode { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		public string TarWarehouseId { get; set; }

		/// <summary>
		/// 上架倉庫名稱
		/// </summary>
		public string TarWarehouseName { get; set; }
	}
	#endregion 進貨容器待關箱資料查詢

	#region 進貨容器關箱確認(RecvCloseBoxConfirm)
	/// <summary>
	/// 進貨容器關箱確認_傳入
	/// </summary>
	public class RecvCloseBoxConfirmReq : StaffModel
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
		/// 進貨容器識別碼
		/// </summary>
		public long F020501_ID { get; set; }
	}
	#endregion 進貨容器關箱確認
}
