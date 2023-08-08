using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 批次商品進倉資料_傳入
	/// </summary>
	public class PostCreateWarehousesReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		public PostCreateWarehousesResultModel Result { get; set; }
	}

	public class PostCreateWarehousesResultModel
	{
		/// <summary>
		/// 總筆數
		/// </summary>
		public int? Total { get; set; }
		/// <summary>
		/// 進倉單主檔
		/// </summary>
		public List<PostCreateWarehouseInsModel> WarehouseIns { get; set; }
	}

	/// <summary>
	/// 進倉單主檔
	/// </summary>
	public class PostCreateWarehouseInsModel
	{
		/// <summary>
		/// 貨主進倉單號(唯一值)
		/// </summary>
		public string CustInNo { get; set; }
		/// <summary>
		/// 貨主單據建立日期
		/// </summary>
		public DateTime? CustCrtDate { get; set; }
		/// <summary>
		/// 預定進倉日期
		/// </summary>
		public DateTime? InDate { get; set; }
		/// <summary>
		/// 採購日期
		/// </summary>
		public DateTime? PoDate { get; set; }
		/// <summary>
		/// 供應商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 採購單號
		/// </summary>
		public string PoNo { get; set; }
		/// <summary>
		/// 作業類別
		/// </summary>
		public string InProp { get; set; }
		/// <summary>
		/// 交易類型代號
		/// </summary>
		public string TranCode { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		public string Memo { get; set; }
		/// <summary>
		/// 狀態(A:新增;U:修改;D:刪除)
		/// </summary>
		public string ProcFlag { get; set; }
		/// <summary>
		/// 批次編號
		/// </summary>
		public string BatchNo { get; set; }
		/// <summary>
		/// 貨主自訂義分類
		/// </summary>
		public string CustCost { get; set; }
		/// <summary>
		/// 快速通關分類，分三級(1)一般、(2)快速、(3)急件
		/// </summary>
		public string FastPassType { get; set; }
		/// <summary>
		/// 預定進倉時段(0: 上午、1: 下午)
		/// </summary>
		public string BoookingInPeriod { get; set; }
		/// <summary>
		/// 進倉單明細
		/// </summary>
		public List<PostCreateWarehouseInDetailModel> WarehouseInDetails { get; set; } = new List<PostCreateWarehouseInDetailModel>();
	}

	/// <summary>
	/// 進倉單明細
	/// </summary>
	public class PostCreateWarehouseInDetailModel
	{
		/// <summary>
		/// 品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 進倉數量
		/// </summary>
		public int InQty { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		public DateTime? ValidDate { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 序號清單
		/// </summary>
		public string[] SnList { get; set; }
		/// <summary>
		/// 容器清單
		/// </summary>
		public List<PostCreateWarehouseInContainerModel> ContainerDatas { get; set; } = new List<PostCreateWarehouseInContainerModel>();
	}

	/// <summary>
	/// 容器資料
	/// </summary>
	public class PostCreateWarehouseInContainerModel
	{
		/// <summary>
		/// 容器編碼
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 進倉數量
		/// </summary>
		public int? InQty { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		public DateTime? ValidDate { get; set; }
		/// <summary>
		/// 序號清單
		/// </summary>
		public List<string> SnList { get; set; }
	}
}
