namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 廠商主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("VNR_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F1908")]
	public class F1908 : IAuditInfo
	{

		/// <summary>
		/// 廠編
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string VNR_CODE { get; set; }

		/// <summary>
		/// 廠商名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(60)")]
		public string VNR_NAME { get; set; }

		/// <summary>
		/// 狀態0使用中 9刪除
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

		/// <summary>
		/// 統一編號
		/// </summary>
		[Column(TypeName = "varchar(16)")]
		public string UNI_FORM { get; set; }

		/// <summary>
		/// 負責人
		/// </summary>
		[Column(TypeName = "nvarchar(40)")]
		public string BOSS { get; set; }

		/// <summary>
		/// 廠商電話
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string TEL { get; set; }

		/// <summary>
		/// 郵遞區號
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string ZIP { get; set; }

		/// <summary>
		/// 地址
		/// </summary>
		[Column(TypeName = "nvarchar(120)")]
		public string ADDRESS { get; set; }

		/// <summary>
		/// 貨物聯絡人
		/// </summary>
		[Column(TypeName = "nvarchar(20)")]
		public string ITEM_CONTACT { get; set; }

		/// <summary>
		/// 貨物聯絡人電話
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string ITEM_TEL { get; set; }

		/// <summary>
		/// 貨物聯絡人手機
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string ITEM_CEL { get; set; }

		/// <summary>
		/// 貨物聯絡人信箱
		/// </summary>
		[Column(TypeName = "varchar(80)")]
		public string ITEM_MAIL { get; set; }

		/// <summary>
		/// 帳務聯絡人
		/// </summary>
		[Column(TypeName = "nvarchar(20)")]
		public string BILL_CONTACT { get; set; }

		/// <summary>
		/// 帳務聯絡人電話
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string BILL_TEL { get; set; }

		/// <summary>
		/// 帳務聯絡人手機
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string BILL_CEL { get; set; }

		/// <summary>
		/// 帳務聯絡人信箱
		/// </summary>
		[Column(TypeName = "varchar(80)")]
		public string BILL_MAIL { get; set; }

		/// <summary>
		/// 發票郵遞區號
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string INV_ZIP { get; set; }

		/// <summary>
		/// 發票地址
		/// </summary>
		[Column(TypeName = "nvarchar(120)")]
		public string INV_ADDRESS { get; set; }

		/// <summary>
		/// 稅別 0否1是
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string TAX_TYPE { get; set; }

		/// <summary>
		/// 幣別(台幣,美金 F000904))
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string CURRENCY { get; set; }

		/// <summary>
		/// 付款條件(F000904 Where Topic=F1909)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string PAY_FACTOR { get; set; }

		/// <summary>
		/// 付款方式 (下拉選單) 現金、票據，電匯 F000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string PAY_TYPE { get; set; }

		/// <summary>
		/// 銀行代碼
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string BANK_CODE { get; set; }

		/// <summary>
		/// 銀行名稱
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string BANK_NAME { get; set; }

		/// <summary>
		/// 銀行帳號
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string BANK_ACCOUNT { get; set; }

		/// <summary>
		/// 前置時間(天數)
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? LEADTIME { get; set; }

		/// <summary>
		/// 訂貨時間(1234567)
		/// </summary>
		[Column(TypeName = "varchar(14)")]
		public string ORD_DATE { get; set; }

		/// <summary>
		/// 訂貨週期(每週訂貨頻率)
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? ORD_CIRCLE { get; set; }

		/// <summary>
		/// 到貨時間(HH:mm)
		/// </summary>
		[Column(TypeName = "varchar(5)")]
		public string DELV_TIME { get; set; }

		/// <summary>
		/// 供應商最低訂量
		/// </summary>
		[Column(TypeName = "int")]
		public Int32? VNR_LIM_QTY { get; set; }

		/// <summary>
		/// 採購安全庫存量
		/// </summary>
		[Column(TypeName = "int")]
		public Int32? ORD_STOCK_QTY { get; set; }

		/// <summary>
		/// 交貨模式編號(F000904：01:自行送貨、02:貨運、03:集約供貨A、04:集約供貨B等)
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string DELI_TYPE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 發票樣式f000904
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string INVO_TYPE { get; set; }

		/// <summary>
		/// 抽驗比例(%)
		/// </summary>
		[Column(TypeName = "decimal(14,11)")]
		public decimal? CHECKPERCENT { get; set; }

		/// <summary>
		/// 廠商類別(0:廠商1維修商)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string VNR_TYPE { get; set; }

		/// <summary>
		/// 擴增預留欄位A
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_A { get; set; }

		/// <summary>
		/// 擴增預留欄位B
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_B { get; set; }

		/// <summary>
		/// 擴增預留欄位C
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_C { get; set; }

		/// <summary>
		/// 擴增預留欄位D
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_D { get; set; }

		/// <summary>
		/// 擴增預留欄位E
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_E { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 廠退說明
		/// </summary>
		[Column(TypeName = "nvarchar(255)")]
		public string RET_MEMO { get; set; }

		/// <summary>
		/// 配送方式 (0: 自取 1:宅配)
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string DELIVERY_WAY { get; set; }
	}
}
