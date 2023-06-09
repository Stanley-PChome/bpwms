using System;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
	public class CreateContainerParam
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 倉庫代碼
		/// </summary>
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 容器編碼
		/// </summary>
		public string CONTAINER_CODE { get; set; }
		/// <summary>
		/// 容器類型 (0: 容器、1: 載具)
		/// </summary>
		public string CONTAINER_TYPE { get; set; }
		/// <summary>
		/// 單據編號
		/// </summary>
		public string WMS_NO { get; set; }
		/// <summary>
		/// 單據類型(A:進倉單 O:出貨單)
		/// </summary>
		public string WMS_TYPE { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		public DateTime? VALID_DATE { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 數量
		/// </summary>
		public int QTY { get; set; }
		/// <summary>
		/// 序號
		/// </summary>
		public string SERIAL_NO_LIST { get; set; }
    /// <summary>
    /// 儲格
    /// </summary>
    public string BIN_CODE { get; set; }

    /// <summary>
    /// 揀貨單號
    /// </summary>
    public string PICK_ORD_NO { get; set; }
  }

    /// <summary>
    /// 容器條碼共用服務
    /// </summary>
    public class ChkContainerResult : ExecuteResult
    {
        public string ContainerCode { get; set; }
        public string BinCode { get; set; }
    }
}
