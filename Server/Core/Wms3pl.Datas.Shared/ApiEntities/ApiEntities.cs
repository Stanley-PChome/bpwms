using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    public class ApiResponse
    {
        /// <summary>
        /// 訊息代碼
        /// </summary>
        public string MsgCode { get; set; }
        /// <summary>
        /// 訊息內容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 單號
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 錯誤欄位
        /// </summary>
        public string ErrorColumn { get; set; }

		public string WmsNo { get; set; }

		public int? Status { get; set; }

		public string UserId { get; set; }
	}

    public class ApiCkeckColumnModel
    {
        public string Name { get; set; }
        public bool Nullable { get; set; } = true;
        public Type Type { get; set; }
        public int MaxLength { get; set; }
        public bool IsDate { get; set; } = false;
        public bool IsDateTime { get; set; } = false;
    }

    public class AddMessageReq
    {
        public string Guid { get; set; }
        public string TicketType { get; set; }
        public string DcCode { get; set; }
        public string GupCode { get; set; }
        public string CustCode { get; set; }
        public string MsgNo { get; set; }
        public string MessageContent { get; set; }
        public string NotifyOrdNo { get; set; }
        public string TargetType { get; set; }
        public string TargetCode { get; set; }
        public bool IsNoTransaction { get; set; } = false;
    }

    public class ApiResult
    {
        public bool IsSuccessed { get; set; }
        public string MsgCode { get; set; }
        public string MsgContent { get; set; }
        public object Data { get; set; }
        public int InsertCnt { get; set; }
        public int UpdateCnt { get; set; }
        public int SuccessCnt { get; set; }
        public int FailureCnt { get; set; }
        public int TotalCnt { get; set; }
        /// <summary>
        /// 原始API回傳內容
        /// </summary>
        public string OriAPIReturnMessage { get; set; }
		    public string HttpCode { get; set; }
		    public string HttpContent { get; set; }
    }

    public class LmsDataResult
    {
        public string DcCode { get; set; }
        public string CustCode { get; set; }
        public string CustOrdNo { get; set; }
        public string TransportCode { get; set; }
        public string TransportProvider { get; set; }
    }

    public class WcsResult
    {
        public string Code { get; set; }
        public string Msg { get; set; }
        public List<object> Data { get; set; }
    }

    public class WcsErrorModel
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class WcsErrorResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class WcsApiResult<T>
    {
        public string Code { get; set; }
        public string Msg { get; set; }
        public List<T> Data { get; set; }
    }

    public class WcsApiErrorResult
    {
        public string MsgCode { get; set; }
        public string MsgContent { get; set; }
    }

	public class LmsVendorReturnOrderData
	{
		/// <summary>
		/// 廠退出貨單號
		/// </summary>
		public string WmsOrdNo { get; set; }
		public List<LmsVendorReturnOrderDetails> Details { get; set; }
	}

	public class LmsVendorReturnOrderDetails
	{
		/// <summary>
		/// 宅配單號
		/// </summary>
		public string TransportCode { get; set; }
		/// <summary>
		/// 物流商編號
		/// </summary>
		public string TransportProvider { get; set; }
	}

	public class LmsApiResult
	{
		public string Msg { get; set; }
		public string Code { get; set; }
		public byte[] Data { get; set; }
        public string MsgCode { get; set; }
        public string MsgContent { get; set; }
		public string ContentType { get; set; }

	}

	public class LmsResult
	{
		public bool IsSuccessed { get; set; }
		public string MsgCode { get; set; }
		public string MsgContent { get; set; }
		public int SuccessCnt { get; set; }
		public int FailureCnt { get; set; }
		public List<object> Data { get; set; }
	}

	public class LmsResponse
	{
		public string MsgCode { get; set; }
		public string MsgContent { get; set; }
		public string WmsNo { get; set; }
	}

	public class LmsApiMultiResult<T, TErr>
	{
		public string Code { get; set; }
		public string Msg { get; set; }
		public T[] Data { get; set; }
		public TErr ErrorData { get; set; }
	}

	public class PickLocRouteData
	{
		public string PickAreaID { get; set; }
		public PickLocRoute[] RouteList { get; set; }
	}

	public class PickLocRoute
	{
		public string LocCode { get; set; }
		public int RouteSeq { get; set; }
	}

	public class ErrorData
	{
		public string ErrorColumn { get; set; }
	}
}
