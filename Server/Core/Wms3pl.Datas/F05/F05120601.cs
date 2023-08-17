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
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F05120601")]
	public class F05120601 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

	
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 揀貨序號
		/// </summary>

		[Required]
    [Column(TypeName = "varchar(4)")]
    public string PICK_ORD_SEQ { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>

		[Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

		/// <summary>
		/// 出貨單序號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string WMS_ORD_SEQ { get; set; }

		/// <summary>
		/// 揀貨儲位
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(14)")]
    public string PICK_LOC { get; set; }

		/// <summary>
		/// 品號
		/// </summary>

		[Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

		/// <summary>
		/// 效期
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime VALID_DATE { get; set; }

		/// <summary>
		/// 入庫日
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime ENTER_DATE { get; set; }

    /// <summary>
    /// 批號
    /// </summary>

    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }

    /// <summary>
    /// 序號
    /// </summary>

    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }

		/// <summary>
		/// 缺貨數量
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int LACK_QTY { get; set; }

		/// <summary>
		/// 處理狀態(0: 待扣庫、1:配庫處理中、2:已扣庫、3:缺貨、4:待補貨、9: 取消)
		/// </summary>

		[Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

		/// <summary>
		/// 訂單類別(0:B2B,1:B2C)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string ORD_TYPE { get; set; }

    /// <summary>
    /// 來源單據
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string SOURCE_TYPE { get; set; }

    /// <summary>
    /// 客戶自訂分類(Out:一般出貨 MoveOut:集貨出貨(跨庫調撥) CVS:超商出貨
    /// </summary>

    [Column(TypeName = "nvarchar(10)")]
    public string CUST_COST { get; set; }

    /// <summary>
    /// 優先處理旗標(1:一般 2:優先 3:急件)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string FAST_DEAL_TYPE { get; set; }

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
		/// 建立人名
		/// </summary>
		[Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>   
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")] public DateTime? UPD_DATE { get; set; }

	
	}
}
