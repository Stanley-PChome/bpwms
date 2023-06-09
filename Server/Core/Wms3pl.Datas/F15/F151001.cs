namespace Wms3pl.Datas.F15
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調撥單頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ALLOCATION_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F151001")]
  public class F151001 : IAuditInfo
  {

	  /// <summary>
	  /// 調撥單日期
	  /// </summary>
    [Required]
	  public DateTime ALLOCATION_DATE { get; set; }

	  /// <summary>
	  /// 調撥單號
	  /// </summary>
    [Key]
    [Required]
	  public string ALLOCATION_NO { get; set; }

	  /// <summary>
	  /// 建立調撥單日期
	  /// </summary>
    [Required]
	  public DateTime CRT_ALLOCATION_DATE { get; set; }

	  /// <summary>
	  /// 過帳日期
	  /// </summary>
	  public DateTime? POSTING_DATE { get; set; }

	  /// <summary>
	  /// 調撥單狀態(0待處理1已列印調撥單2:下架處理中3已下架處理4上架處理中5已上架處理9取消)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 目的物流中心
	  /// </summary>
    [Required]
	  public string TAR_DC_CODE { get; set; }

	  /// <summary>
	  /// 目的倉別(F1980)
	  /// </summary>
	  public string TAR_WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 來源倉別
	  /// </summary>
	  public string SRC_WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 來源物流中心
	  /// </summary>
    [Required]
	  public string SRC_DC_CODE { get; set; }

	  /// <summary>
	  /// 來源單據(F000902)
	  /// </summary>
	  public string SOURCE_TYPE { get; set; }

	  /// <summary>
	  /// 來源單號
	  /// </summary>
	  public string SOURCE_NO { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
	  public string BOX_NO { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

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
	  /// 是否派車(0否 1是)
	  /// </summary>
    [Required]
	  public string SEND_CAR { get; set; }

	  /// <summary>
	  /// ******待刪除******
	  /// </summary>
	  public string SRC_MOVE_STAFF { get; set; }

	  /// <summary>
	  /// ******待刪除******
	  /// </summary>
	  public string SRC_MOVE_NAME { get; set; }

	  /// <summary>
	  /// 調撥Device鎖定狀態0(未下架) 1(下架中) 2(下架完成) 3(上架中)4(上架完成)
	  /// </summary>
    [Required]
	  public string LOCK_STATUS { get; set; }

	  /// <summary>
	  /// ******待刪除******
	  /// </summary>
	  public string TAR_MOVE_STAFF { get; set; }

	  /// <summary>
	  /// ******待刪除******
	  /// </summary>
	  public string TAR_MOVE_NAME { get; set; }

	  /// <summary>
	  /// 是否展開效期和入庫日
	  /// </summary>
    [Required]
	  public string ISEXPENDDATE { get; set; }

	  /// <summary>
	  /// 作業工具
	  /// </summary>
    [Required]
	  public string MOVE_TOOL { get; set; }

	  /// <summary>
	  /// 是否為搬移單(0:否;1:是)
	  /// </summary>
    [Required]
	  public string ISMOVE_ORDER { get; set; }

		/// <summary>
		/// 下架開始時間
		/// </summary>
		public DateTime? SRC_START_DATE { get; set; }

		/// <summary>
		/// 上架開始時間
		/// </summary>
		public DateTime? TAR_START_DATE { get; set; }

        /// <summary>
        /// 調撥單類型 (0: 一般調撥單、1: 虛擬庫存回復單、2: 每日補貨單、3:配庫補貨單(含純上架單)、4:驗收上架單、5:補貨純下架單)
        /// </summary>
        public string ALLOCATION_TYPE { get; set; }

        /// <summary>
        /// 棧板/載具/容器編號
        /// </summary>
        public string CONTAINER_CODE { get; set; }

        /// <summary>
        /// 容器F0701.ID
        /// </summary>
        public Int64? F0701_ID { get; set; }

        /// <summary>
        /// 補貨預定上架倉別
        /// </summary>
        public string PRE_TAR_WAREHOUSE_ID { get; set; }
    }
}
        