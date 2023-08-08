using System.Collections.Generic;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 揀貨批次分配_傳入
    /// </summary>
    public class BatchPickAllotReq
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
        /// 單據類型(1=訂單 2=調撥出 3=廠退出)
        /// </summary>
        public string OrderType { get; set; }
        /// <summary>
        /// (單據列表)
        /// </summary>
        public List<BatchPickAllotOrderData> OrderList { get; set; }
    }

    public class BatchPickAllotOrderData
    {
        /// <summary>
        /// 出貨單號 ※已配庫出貨單
        /// </summary>
        public string WmsNo { get; set; }
        /// <summary>
        /// 目的地代碼
        /// OrderType=1，A7包裝大小線、12庫為空白
        ///	OrderType=2，調撥出目的地編號
        ///	OrderType = 3，廠商編號
        /// </summary>
        public string TargetCode { get; set; }
        /// <summary>
        /// (出貨明細)
        /// </summary>
        public List<BatchPickAllotItemData> Items { get; set; }
    }

    public class BatchPickAllotItemData
	{
		/// <summary>
		/// 明細項次 ※出貨單明細項次(同項次可能多筆
		/// </summary>
		public string WmsSeq { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 揀貨數量
		/// </summary>
		public int Qty { get; set; }
		/// <summary>
		/// 揀貨系統(0: 人工倉、1: 自動倉)
		/// </summary>
		public int PickingSystem { get; set; }
		/// <summary>
		/// 揀貨儲位編號
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 效期(格式:yyyy/MM/dd)
		/// </summary>
		public string ValidDate { get; set; }
		/// <summary>
		/// 入庫日(格式:yyyy/MM/dd)
		/// </summary>
		public string EnterDate { get; set; }
		/// <summary>
		/// 批號(格式:yyyy/MM/dd)
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 序號(序號綁儲位使用)
		/// </summary>
		public string SerialNo { get; set; }
	}

    public class BatchPickAllotRes
    {
        public bool IsSuccessed { get; set; }
        public string Message { get; set; }
        public string No { get; set; }
        public List<BatchPickAllotPickingBatchData> Data { get; set; }
        public BatchPickAllotErrorData ErrorData { get; set; }
    }

    public class BatchPickAllotPickingBatchData
    {
        public string PickingBatchNo { get; set; }
        public string PickingType { get; set; }
        public string CreateTime { get; set; }
        public List<BatchPickAllotPickingListData> PickingList { get; set; }
    }
    /// <summary>
    /// 揀貨清單
    /// </summary>
    public class BatchPickAllotPickingListData
    {
        public string PickingNo { get; set; }
		public int PickingSystem { get; set; }		
        public string PickAreaID { get; set; }
        public string PickAreaName { get; set; }
        public string ContainerType { get; set; }
        public string NextStepCode { get; set; }
        public string TargetCode { get; set; }
        public List<BatchPickAllotDetailsData> Details { get; set; }
    }

    /// <summary>
    /// 揀貨明細
    /// </summary>
    public class BatchPickAllotDetailsData
    {
        public string ItemCode { get; set; }
        public int Qty { get; set; }
        public string PickAreaID { get; set; }
        public string PickAreaName { get; set; }
        public string LocCode { get; set; }
        public string ValidDate { get; set; }
        public string EnterDate { get; set; }
        public string MakeNo { get; set; }
        public string SerialNo { get; set; }
        public List<BatchPickAllotOrdersData> Orders { get; set; }
    }

    /// <summary>
    /// 單據清單
    /// </summary>
    public class BatchPickAllotOrdersData
    {
        public string WmsNo { get; set; }
        public string WmsSeq { get; set; }
        public int Qty { get; set; }
    }

    //public class BatchPickAllotErrorData
    //{
    //    public string[] ErrorColumn { get; set; }
    //}

    public class BatchPickAllotErrorData : ErrorData
    { }
}
