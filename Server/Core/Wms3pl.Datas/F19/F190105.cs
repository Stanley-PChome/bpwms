using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	/// <summary>
	/// 物流中心主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE")]
	[Table("F190105")]
	public class F190105 : IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 是否開啟自動配庫(1:是;0:否)
		/// </summary>
		[Required]
		public string OPEN_AUTO_ALLOC_STOCK { get; set; }

		/// <summary>
		/// B2B PDA揀貨比例
		/// </summary>
		[Required]
		public decimal B2B_PDA_PICK_PECENT { get; set; }

		/// <summary>
		/// B2C PDA揀貨比例
		/// </summary>
		[Required]
		public decimal B2C_PDA_PICK_PERCENT { get; set; }

		/// <summary>
		/// B2B揀貨單是否自動列印(1:是;0:否)
		/// </summary>
		[Required]
		public string B2B_AUTO_PICK_PRINT { get; set; }

		/// <summary>
		/// B2C揀貨單是否自動列印(1:是;0:否)
		/// </summary>
		[Required]
		public string B2C_AUTO_PICK_PRINT { get; set; }

		/// <summary>
		/// 是否等待人工倉揀貨完成後才派發自動倉揀貨任務(1:是;0:否)
		/// </summary>
		[Required]
		public string WAIT_SEND_AUTO_PICK { get; set; }
		/// <summary>
		/// 是否使用容器(1:是;0:否)
		/// </summary>
		[Required]
		public string USE_CONTAINER { get; set; }

		/// <summary>
		/// 揀貨單最大明細筆數
		/// </summary>
		[Required]
		public int PICKORDER_MAX_RECORD { get; set; }

		/// <summary>
		/// 人工倉單一揀貨單設定:訂單超過N個品項數
		/// </summary>
		[Required]
		public int ORDER_MAX_ITEMCNT { get; set; }

		/// <summary>
		/// 人工倉單一揀貨單設定:訂單明細超過N筆
		/// </summary>
		[Required]
		public int ORDER_MAX_RECORD { get; set; }

		/// <summary>
		/// 是否開啟特殊結構訂單(1:是;0:否)
		/// </summary>
		[Required]
		public string OPEN_SPECIAL_ORDER { get; set; }

		/// <summary>
		/// 出貨模式(1:平時;2:戰時)
		/// </summary>
		[Required]
		public string SHIP_MODE { get; set; }

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
    /// 一般出貨預設揀貨容器代碼(NULL:無  00:不限 01:M-周轉箱 02:2L周轉箱 06:調撥箱)
    /// </summary>
    /// 
    public string DF_NSHIP_CONTAINER_TYPE { get; set; }

    /// <summary>
    /// 跨庫調撥出預設揀貨容器代碼(NULL:無  00:不限 01:M-周轉箱 02:2L周轉箱 06:調撥箱)
    /// </summary>
    public string DF_CSHIP_CONTANER_TYPE { get; set; }

    /// <summary>
    /// 廠退出預設揀貨容器代碼(NULL:無  00:不限 01:M-周轉箱 02:2L周轉箱  06:調撥箱)
    /// </summary>
    public string DF_VNRSHIP_CONTAINER_TYPE { get; set; }

    /// <summary>
    /// 庫內移動出預設揀貨容器代碼(NULL:無  00:不限 01:M-周轉箱 02:2L周轉箱  06:調撥箱)
    /// </summary>
    public string DF_MOVE_CONTAINER_TYPE { get; set; }

    /// <summary>
    /// 設定集貨格方式(01:依訂單容積計算 02:使用指定周轉箱對應方式 )
    /// </summary>
    public string CELL_TYPE_METHOD { get; set; }

    /// <summary>
    /// 是否指定周轉箱(0:否 1:是)
    /// </summary>
    public string IS_DIRECT_CONTAINER_TYPE { get; set; }

    /// <summary>
    /// 基礎周轉箱類型(NULL:無  00:不限 01:M-周轉箱 02:2L周轉箱  06:調撥箱)
    /// </summary>
    public string BASE_CONTAINER_TYPE { get; set; }

    /// <summary>
    /// 基礎周轉箱最長邊(內徑cm)
    /// </summary>
    public decimal? BASE_CONTAINER_MAX_LENGTH { get; set; }

    /// <summary>
    /// 基礎周轉箱次長邊(內徑cm)
    /// </summary>
    public decimal? BASE_CONTAINER_MID_LENGTH { get; set; }

    /// <summary>
    /// 基礎周轉箱最短邊(內徑cm)
    /// </summary>
    public decimal? BASE_CONTAINER_MIN_LENGTH { get; set; }

    /// <summary>
    /// 基礎周轉箱可用容積(cm3)
    /// </summary>
    public decimal? BASE_CONTAINER_VOLUMN { get; set; }

    /// <summary>
    /// 指定限制單一揀貨的貨主清單
    /// </summary>
    public string LIMIT_SINGLEPICK_CUST_LIST { get; set; }

    /// <summary>
    /// 可執行自動配庫的貨主清單(多貨主逗點分隔)
    /// </summary>
    public string AUTO_ALLOT_CUST_LIST { get; set; }
  }
}
