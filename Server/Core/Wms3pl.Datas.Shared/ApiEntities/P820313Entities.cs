using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	public class SysErrorNotifyResultsReq
    {
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 異常代碼
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 異常內容
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 異常單據
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 單據類型
        /// </summary>
        public string OrderType { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        public string ItemCode { get; set; }
    }

	public class SysErrorNotifyResultsRes : WcsApiErrorResult
	{
		/// <summary>
		/// 調整單號
		/// </summary>
		public string ShiftWmsNo { get; set; }
	}
}
