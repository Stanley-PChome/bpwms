using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    #region 收單驗貨上架
    /// <summary>
    /// 收單驗貨上架_傳入參數
    /// </summary>
    public class RecvItemNotifyReq
    {
        /// <summary>
        /// 倉庫代碼
        /// </summary>
        public string WhId { get; set; }
        /// <summary>
        /// 收貨單號
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 工作站ID
        /// </summary>
        public string WorkStationId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string SkuId { get; set; }
        /// <summary>
        /// 時間戳記
        /// </summary>
        public string TimeStamp { get; set; }
    }

    /// <summary>
    /// 收單驗貨上架_回傳
    /// </summary>
    public class RecvItemNotifyRes
    {
        /// <summary>
        /// 回應代碼
        /// </summary>
        public int Code { get; set; }
		/// <summary>
		/// 錯誤資料
		/// </summary>
		public List<OrderErrorData> ErrorData { get; set; }
	}
    #endregion

    #region 配箱資訊同步
    /// <summary>
    /// 配箱資訊同步_傳入參數
    /// </summary>
    public class DistibuteInfoAsyncReq
    {
        /// <summary>
        /// 倉庫代碼
        /// </summary>
        public string WhId { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string OutboundNo { get; set; }
        /// <summary>
        /// 工作站ID
        /// </summary>
        public string WorkStationId { get; set; }
        /// <summary>
        /// 作業人員ID
        /// </summary>
        public string OperationUserId { get; set; }
        /// <summary>
        /// 時間戳記
        /// </summary>
        public string TimeStamp { get; set; }
    }

    /// <summary>
    /// 配箱資訊同步_回傳
    /// </summary>
    public class DistibuteInfoAsyncRes
    {
        /// <summary>
        /// 回應代碼
        /// </summary>
        public int Code { get; set; }
		/// <summary>
		/// 錯誤資料
		/// </summary>
		public List<OutboundErrorData> ErrorData { get; set; }
    }
    #endregion

    #region 封箱資訊同步
    /// <summary>
    /// 封箱資訊同步_傳入參數
    /// </summary>
    public class SealingInfoAsyncReq
    {
        /// <summary>
        /// 倉庫代碼
        /// </summary>
        public string WhId { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string OutboundNo { get; set; }
        /// <summary>
        /// 工作站ID
        /// </summary>
        public string WorkStationId { get; set; }
        /// <summary>
        /// 作業人員ID
        /// </summary>
        public string OperationUserId { get; set; }
        /// <summary>
        /// 宅配單號
        /// </summary>
        public string ShipNo { get; set; }
        /// <summary>
        /// 時間戳記
        /// </summary>
        public string TimeStamp { get; set; }
    }

    /// <summary>
    /// 封箱資訊同步_回傳
    /// </summary>
    public class SealingInfoAsyncRes
    {
        /// <summary>
        /// 回應代碼
        /// </summary>
        public int Code { get; set; }
		/// <summary>
		/// 錯誤資料
		/// </summary>
		public List<OutboundErrorData> ErrorData { get; set; }
	}
	#endregion

	#region ErrorData
	public class OutboundErrorData
	{
		/// <summary>
		/// 錯誤訊息
		/// </summary>
		public string ErrorMsg { get; set; }
		/// <summary>
		/// 錯誤欄位
		/// </summary>
		public List<string> ErrorColumn { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string OutboundNo { get; set; }
	}

	public class OrderErrorData
	{
		/// <summary>
		/// 錯誤訊息
		/// </summary>
		public string ErrorMsg { get; set; }
		/// <summary>
		/// 錯誤欄位
		/// </summary>
		public List<string> ErrorColumn { get; set; }
		/// <summary>
		/// 收貨單號
		/// </summary>
		public string OrderNo { get; set; }
	}
	#endregion
}
