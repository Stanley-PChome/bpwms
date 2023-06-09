using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 批次廠商退貨單資料_傳入
	/// </summary>
	public class PostCreateVendorReturnsReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		public PostCreateVendorReturnsResultModel Result { get; set; }
	}

	public class PostCreateVendorReturnsResultModel
	{
		/// <summary>
		/// 總筆數
		/// </summary>
		public int? Total { get; set; }
		/// <summary>
		/// 退貨單主檔
		/// </summary>
		public List<PostCreateVendorReturnsModel> VnrReturns { get; set; }
	}

	/// <summary>
	/// 退貨單主檔
	/// </summary>
	public class PostCreateVendorReturnsModel
	{
		/// <summary>
		/// 貨主退貨單號(唯一值)
		/// </summary>
		public string CustVnrRetrunNo { get; set; }
		/// <summary>
		/// 貨主退貨單號(唯一值)
		/// </summary>
		public string SupCode { get; set; }
		/// <summary>
		/// 單據日期
		/// </summary>
		public DateTime? ReturnDate { get; set; }
		/// <summary>
		/// 單據類型(0:B2B 1:B2C)
		/// </summary>
		public string VnrReturnType { get; set; }
		/// <summary>
		/// 退貨客戶編號
		/// </summary>
		public string CustCategory { get; set; }
		/// <summary>
		/// 退貨類型
		/// </summary>
		public string Memo { get; set; }
		/// <summary>
		/// 退貨原因
		/// </summary>
		public string ProcFlag { get; set; }
		/// <summary>
		/// 批次編號
		/// </summary>
		public string BatchNo { get; set; }
		/// <summary>
		/// 配送方式(0:自取 1:宅配)
		/// </summary>
		public string DeliveryWay { get; set; }
		/// <summary>
		/// 出貨倉別
		/// </summary>
		public string TypeId { get; set; }
		/// <summary>
		/// 進倉單明細
		/// </summary>
		public List<PostCreateVendorReturnsDetailModel> VnrReturnDetails { get; set; } = new List<PostCreateVendorReturnsDetailModel>();
	}

	/// <summary>
	/// 退貨單明細
	/// </summary>
	public class PostCreateVendorReturnsDetailModel
	{
		/// <summary>
		/// 品項編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 通路品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 數量
		/// </summary>
		public int ReturnQty { get; set; }
		/// <summary>
		/// 指定商品的批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 廠退原因
		/// </summary>
		public string VnrReturnCause { get; set; }
		/// <summary>
		/// 廠退原因說明
		/// </summary>
		public string VnrReturnMemo { get; set; }
	}

	public class ThirdPartVendorReturns
	{
		public string RTN_VNR_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
	}
}
