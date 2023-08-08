using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	/// <summary>
	/// 出貨批次明細檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DELV_DATE",  "PICK_TIME", "DC_CODE", "GUP_CODE", "CUST_CODE","WMS_NO")]
	[Table("F051301")]
	public class F051301 : IAuditInfo
	{
		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 批次日期
		/// </summary>
		[Key]
		[Required]
		public DateTime DELV_DATE { get; set; }

		/// <summary>
		/// 批次時段
		/// </summary>
		[Key]
		[Required]
		public string PICK_TIME { get; set; }

		/// <summary>
		/// 出貨單號/揀貨單號
		/// </summary>
		[Key]
		[Required]
		public string WMS_NO { get; set; }

		/// <summary>
		/// 集貨場編號
		/// </summary>
		public string COLLECTION_CODE { get; set; }

		/// <summary>
		/// 出貨單需要的集貨格大小(F194501)
		/// </summary>

		public string CELL_TYPE { get; set; }

		/// <summary>
		/// 狀態(0: 需集貨、1:不集貨 、2: 集貨中、3: 出場)
		/// </summary>
		public string STATUS { get; set; }

		/// <summary>
		/// 下一站類型(2: 集貨場、3: 包裝站、4: 異常區、5:廠退集貨場、6:跨庫集貨場)
		/// </summary>
		public string NEXT_STEP { get; set; }

        /// <summary>
        /// 集貨位置(0:人工集貨場 1:自動倉集貨場)
        /// </summary>
        public string COLLECTION_POSITION { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

    /// <summary>
    /// 集貨模式 (0: 一般集貨、1: 特殊集貨)  特殊集貨: 從自動倉轉人工倉後的集貨、包裝異常處理
    /// </summary>
    public string COLLECTION_MODE { get; set; }

    /// <summary>
    /// 發送集貨等待通知模式 (0: 不發送、1: 發送)
    /// </summary>
    public string NOTIFY_MODE { get; set; }

  }
}
