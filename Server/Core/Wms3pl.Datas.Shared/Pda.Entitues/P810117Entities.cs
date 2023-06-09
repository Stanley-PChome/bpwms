using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	#region 紙箱捕貨-接受任務
	/// <summary>
	/// 紙箱捕貨-接受任務_傳入
	/// </summary>
	public class CartonReplenishAcceptReq : StaffModel
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
		/// 紙箱補貨流水號
		/// </summary>
		public Int64 ID { get; set; }
	}

	/// <summary>
	/// 紙箱捕貨-接受任務_傳回
	/// </summary>
	public class CartonReplenishAcceptRes
	{
		/// <summary>
		/// 紙箱補貨流水號
		/// </summary>
		public Int64 ID { get; set; }
		/// <summary>
		/// 訊息
		/// </summary>
		public string Info { get; set; }
	}
	#endregion

	#region 紙箱補貨-放棄任務
	/// <summary>
	/// 紙箱補貨-放棄任務_傳入
	/// </summary>
	public class CartonReplenishRejectReq : StaffModel
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
		/// 紙箱補貨流水號
		/// </summary>
		public Int64 ID { get; set; }
	}

	// <summary>
	/// 紙箱捕貨-放棄任務_傳回
	/// </summary>
	public class CartonReplenishRejectRes
	{
		/// <summary>
		/// 紙箱補貨流水號
		/// </summary>
		public Int64 ID { get; set; }
		/// <summary>
		/// 訊息
		/// </summary>
		public string Info { get; set; }
	}
	#endregion

	#region 紙箱補貨-查詢
	/// <summary>
	/// 紙箱補貨-查詢_傳入
	/// </summary>
	public class GetCartonReplenishReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 樓層
		/// </summary>
		public string Floor { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string WorkStationCode { get; set; }
		/// <summary>
		/// 紙箱條碼
		/// </summary>
		public string BoxCode { get; set; }
	}

	/// <summary>
	///  紙箱補貨-查詢_傳回
	/// </summary>
	public class GetCartonReplenishRes
	{
		/// <summary>
		/// 流水號
		/// </summary>
		public Int64 ID { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string WorkStationCode { get; set; }
		/// <summary>
		/// 紙箱編號
		/// </summary>
		public string BoxCode { get; set; }
		/// <summary>
		/// 通知時間
		/// </summary>
		public DateTime NoticeTime { get; set; }
		/// <summary>
		/// 狀態
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 狀態名稱
		/// </summary>
		public string StatusName { get; set; }
	}
  #endregion

  #region 紙箱補貨-完成補貨
  public class CartonReplenishFinishReq : StaffModel
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
    /// 紙箱補貨流水號
    /// </summary>
    public long ID { get; set; }
    /// <summary>
    /// 補貨數 
    /// </summary>
    public int Qty { get; set; }
  }

  public class CartonReplenishFinishRes
  {
    /// <summary>
    /// 紙箱補貨流水號
    /// </summary>
    public Int64 ID { get; set; }
		/// <summary>
		/// 訊息
		/// </summary>
		public string Info { get; set; }
	}

  #endregion 紙箱補貨-完成補貨
}
