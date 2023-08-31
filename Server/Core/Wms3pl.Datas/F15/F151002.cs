namespace Wms3pl.Datas.F15
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 調撥單明細
    /// </summary>
    [Serializable]
    [DataServiceKey("ALLOCATION_NO", "ALLOCATION_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
    [Table("F151002")]
    public class F151002 : IAuditInfo
    {

        /// <summary>
        /// 調撥單號
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string ALLOCATION_NO { get; set; }

        /// <summary>
        /// 調撥單序號
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "smallint")]
        public Int16 ALLOCATION_SEQ { get; set; }

        /// <summary>
        /// 原調撥明細序號
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "smallint")]
        public Int16 ORG_SEQ { get; set; }

        /// <summary>
        /// 調撥單日期
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime ALLOCATION_DATE { get; set; }

        /// <summary>
        /// 調撥狀態0(下架未處理) 1(下架完成上架未處理) 2(上架完成)
        /// </summary>
        [Required]
        [Column(TypeName = "char(1)")]
        public string STATUS { get; set; }

        /// <summary>
        /// 商品編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string ITEM_CODE { get; set; }

        /// <summary>
        /// 下架儲位   (來源儲位)
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(14)")]
        public string SRC_LOC_CODE { get; set; }

        /// <summary>
        /// 建議上架儲位   (建議目的儲位)
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(14)")]
        public string SUG_LOC_CODE { get; set; }

        /// <summary>
        /// 上架儲位   (目的儲位)
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(14)")]
        public string TAR_LOC_CODE { get; set; }

        /// <summary>
        /// 下架數
        /// </summary>
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 SRC_QTY { get; set; }

        /// <summary>
        /// 實際下架數
        /// </summary>
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 A_SRC_QTY { get; set; }

        /// <summary>
        /// 上架數
        /// </summary>
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 TAR_QTY { get; set; }

        /// <summary>
        /// 實際上架數
        /// </summary>
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 A_TAR_QTY { get; set; }

        /// <summary>
        /// 商品序號
        /// </summary>
        [Column(TypeName = "varchar(50)")]
        public string SERIAL_NO { get; set; }

        /// <summary>
        /// 商品效期
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime VALID_DATE { get; set; }

        /// <summary>
        /// 是否已刷讀序號
        /// </summary>
        [Required]
        [Column(TypeName = "char(1)")]
        public string CHECK_SERIALNO { get; set; }

        /// <summary>
        /// 下架人員
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string SRC_STAFF { get; set; }

        /// <summary>
        /// 下架日期時間
        /// </summary>
        [Column(TypeName = "datetime2(0)")]
        public DateTime? SRC_DATE { get; set; }

        /// <summary>
        /// 下架人名
        /// </summary>
        [Column(TypeName = "nvarchar(16)")]
        public string SRC_NAME { get; set; }

        /// <summary>
        /// 上架人員
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string TAR_STAFF { get; set; }

        /// <summary>
        /// 上架日期時間
        /// </summary>
        [Column(TypeName = "datetime2(0)")]
        public DateTime? TAR_DATE { get; set; }

        /// <summary>
        /// 上架人名
        /// </summary>
        [Column(TypeName = "nvarchar(16)")]
        public string TAR_NAME { get; set; }

        /// <summary>
        /// 物流中心
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "varchar(3)")]
        public string DC_CODE { get; set; }

        /// <summary>
        /// 業主
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "varchar(4)")]
        public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "varchar(6)")]
        public string CUST_CODE { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        [Column(TypeName = "datetime2(0)")]
        public DateTime? UPD_DATE { get; set; }

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
        /// 下架效期(下架實際刷讀效期)
        /// </summary>
        [Column(TypeName = "datetime2(0)")]
        public DateTime? SRC_VALID_DATE { get; set; }

        /// <summary>
        /// 上架效期(上架實際刷讀效期)
        /// </summary>
        [Column(TypeName = "datetime2(0)")]
        public DateTime? TAR_VALID_DATE { get; set; }

        /// <summary>
        /// 入庫日
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime ENTER_DATE { get; set; }

        /// <summary>
        /// 廠商編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string VNR_CODE { get; set; }

        /// <summary>
        /// 盒號/儲值卡盒號
        /// </summary>
        [Column(TypeName = "varchar(50)")]
        public string BOX_NO { get; set; }

        /// <summary>
        /// 箱號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string BOX_CTRL_NO { get; set; }

        /// <summary>
        /// 板號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string PALLET_CTRL_NO { get; set; }

        /// <summary>
        /// 棧板貼紙編號F010203
        /// </summary>
        [Column(TypeName = "varchar(24)")]
        public string STICKER_PALLET_NO { get; set; }

        /// <summary>
        /// 批號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(40)")]
        public string MAKE_NO { get; set; }

        /// <summary>
        /// 下架修改批號
        /// </summary>
        [Column(TypeName = "varchar(40)")]
        public string SRC_MAKE_NO { get; set; }

        /// <summary>
        /// 上架修改批號
        /// </summary>
        [Column(TypeName = "varchar(40)")]
        public string TAR_MAKE_NO { get; set; }

        /// <summary>
        /// 容器編碼
        /// </summary>
        [Column(TypeName = "varchar(32)")]
        public string CONTAINER_CODE { get; set; }

        /// <summary>
        /// 入庫標示
        /// </summary>
        [Column(TypeName = "int")]
        public int? RECEIPTFLAG { get; set; }

        /// <summary>
        /// 容器分隔條碼
        /// </summary>
        [Column(TypeName = "varchar(32)")]
        public string BIN_CODE { get; set; }

        /// <summary>
        /// 來源單據類型
        /// </summary>
        [Column(TypeName = "varchar(2)")]
        public string SOURCE_TYPE { get; set; }

        /// <summary>
        /// 來源單號
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string SOURCE_NO { get; set; }

        /// <summary>
        /// 參考單號 (例如進倉的驗收單，因為有兩層，需要另外紀錄)
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string REFENCE_NO { get; set; }

        /// <summary>
        /// 參考序號
        /// </summary>
        [Column(TypeName = "varchar(4)")]
        public string REFENCE_SEQ { get; set; }
    }
}
