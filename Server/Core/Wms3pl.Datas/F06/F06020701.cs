namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 出貨明細人員明細紀錄資料檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F06020701")]
    public class F06020701 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        public long ID { get; set; }

		/// <summary>
		/// F060207流水號
		/// </summary>
		[Required]
		public long F060207_ID { get; set; }

		/// <summary>
		/// 出貨任務單號
		/// </summary>
		[Required]
		public string ORDERCODE { get; set; }

		/// <summary>
		/// 明細項次
		/// </summary>
		[Required]
		public int ROWNUM { get; set; }

		/// <summary>
		/// 庫內品號
		/// </summary>
		[Required]
		public string SKUCODE { get; set; }

		/// <summary>
		/// 裝箱數量
		/// </summary>
		[Required]
		public int SKUQTY { get; set; }

		/// <summary>
		/// 商品等級(0=殘品 1=正品)
		/// </summary>
		[Required]
		public int SKULEVEL { get; set; }

		/// <summary>
		/// 商品效期
		/// </summary>
		public DateTime? EXPIRYDATE { get; set; }

		/// <summary>
		/// 外部批次號(入庫日(yyMMdd)+序號(3碼數字) 或)
		/// </summary>
		public string OUTBATCHCODE { get; set; }

		/// <summary>
		/// 商品序號清單 (紀錄本箱中的序號)
		/// </summary>
		public string SERIALNUMLIST { get; set; }

		/// <summary>
		/// 容器分隔編號
		/// </summary>
		public string BINCODE { get; set; }

		/// <summary>
		/// 揀貨完成時間
		/// </summary>
		[Required]
		public DateTime COMPLETE_TIME { get; set; }

		/// <summary>
		/// 是否出庫單最後一箱(0=否 1=是)
		/// </summary>
		[Required]
		public int ISLASTCONTAINER { get; set; }

		/// <summary>
		/// 出貨單總箱數((出庫單最後一箱時顯示))
		/// </summary>
		public int? CONTAINER_TOTAL { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
        public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人員名稱
		/// </summary>
		[Required]
        public string CRT_NAME { get; set; }


		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人員編號
		/// </summary>
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動人員名稱
		/// </summary>
		public string UPD_NAME { get; set; }
    }
}
