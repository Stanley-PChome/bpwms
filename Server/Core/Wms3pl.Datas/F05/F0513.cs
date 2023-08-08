namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 出貨批次紀錄檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DELV_DATE", "CUST_CODE", "PICK_TIME", "GUP_CODE", "DC_CODE")]
	[Table("F0513")]
	public class F0513 : IAuditInfo
	{

		/// <summary>
		/// 批次日期
		/// </summary>
		[Key]
		[Required]
		public DateTime DELV_DATE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 訂單類別
		/// </summary>
		[Required]
		public string ORD_TYPE { get; set; }

		/// <summary>
		/// 批次時段
		/// </summary>
		[Key]
		[Required]
		public string PICK_TIME { get; set; }

		/// <summary>
		/// 門市(客戶)數
		/// </summary>
		public Int16? RETAIL_QTY { get; set; }

		/// <summary>
		/// 揀貨次數
		/// </summary>
		public Int32? PICK_CNT { get; set; }

		/// <summary>
		/// 處理註記(0:待配庫 1:已配庫 2待出貨 6:已扣帳
		/// </summary>
		public string PROC_FLAG { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		public string PIER_CODE { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		public string CHECKOUT_TIME { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		public string ALL_ID { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		public string CAR_NO_A { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		public string CAR_NO_B { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		public string CAR_NO_C { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		public string ISSEAL { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		[Required]
		public string RETURN_FLAG { get; set; }

		/// <summary>
		/// *****待刪除*****
		/// </summary>
		public DateTime? RETURN_DATE { get; set; }

		/// <summary>
		/// 來源單據
		/// </summary>
		public string SOURCE_TYPE { get; set; }

		/// <summary>
		/// 揀貨方式
		/// </summary>
		public string PICK_TYPE { get; set; }

		/// <summary>
		/// 出貨單數
		/// </summary>
		[Required]
		public int SHIP_CNT { get; set; }

		/// <summary>
		/// 人工倉單一揀貨單數(紙本)
		/// </summary>
		[Required]
		public int ATFL_N_PICK_CNT { get; set; }

		/// <summary>
		/// 人工倉批量揀貨單數(紙本)
		/// </summary>
		[Required]
		public int ATFL_B_PICK_CNT { get; set; }

		/// <summary>
		/// 人工倉特殊結構單數(紙本)
		/// </summary>
		[Required]
		public int ATFL_S_PICK_CNT { get; set; }

		/// <summary>
		/// 人工倉單一揀貨單數(PDA)
		/// </summary>
		[Required]
		public int ATFL_NP_PICK_CNT { get; set; }

		/// <summary>
		/// 人工倉批量揀貨單數(PDA)
		/// </summary>
		[Required]
		public int ATFL_BP_PICK_CNT { get; set; }

		/// <summary>
		/// 人工倉特殊結構單數(PDA)
		/// </summary>
		[Required]
		public int ATFL_SP_PICK_CNT { get; set; }

		/// <summary>
		/// 自動倉揀貨單數
		/// </summary>
		[Required]
		public int AUTO_N_PICK_CNT { get; set; }

		/// <summary>
		/// 自動倉特殊結構單數
		/// </summary>
		[Required]
		public int AUTO_S_PICK_CNT { get; set; }

		/// <summary>
		/// 補揀單數
		/// </summary>
		[Required]
		public int REPICK_CNT { get; set; }



		/// <summary>
		/// 客戶自訂分類(Out:一般出貨 MoveOut:集貨出貨(跨庫調撥) CVS:超商出貨
		/// </summary>
		public string CUST_COST { get; set; }

		/// <summary>
		/// 優先處理旗標(1:一般 2:優先 3:急件)
		/// </summary>
		public string FAST_DEAL_TYPE { get; set; }

		[Required]
		/// <summary>
		/// 是否列印 (0:未列印 1:已列印)
		/// </summary>
		public string ISPRINTED { get; set; }

		[Required]
		/// <summary>
		/// PDA揀貨比例
		/// </summary>
		public decimal PDA_PICK_PERCENT { get; set; }
		/// <summary>
		/// 跨庫調撥的目的地名稱
		/// </summary>
		public string MOVE_OUT_TARGET { get; set; }

    /// <summary>
    /// 訂單建立日期 (只存日期不存時間)
    /// </summary>
    public DateTime? ORDER_CRT_DATE { get; set; }

    /// <summary>
    /// 商品處理類別(0:一般 1:含安裝型商品)
    /// </summary>
    public string ORDER_PROC_TYPE { get; set; }

  }
}
