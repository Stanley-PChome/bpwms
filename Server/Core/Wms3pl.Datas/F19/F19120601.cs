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

	[Serializable]
	[DataServiceKey("DC_CODE", "PK_AREA", "LINE_SEQ")]
	[Table("F19120601")]
	/// <summary>
	/// PK區的路線範圍與路順明細設定
	/// </summary>
	public class F19120601 : IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		[Key]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// PK區編號
		/// </summary>
		[Required]
		[Key]
		[Column(TypeName = "varchar(5)")]
		public string PK_AREA { get; set; }

		/// <summary>
		/// 路線順序
		/// </summary>
		[Required]
		[Key]
		[Column(TypeName = "int")]
		public int LINE_SEQ { get; set; }

		/// <summary>
		/// 路線頭碼(儲位前5碼)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string BEGIN_LOC_CODE { get; set; }

		/// <summary>
		/// 路線尾碼(儲位前5碼)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string END_LOC_CODE { get; set; }

		/// <summary>
		/// 水平動線(0: 依序進行 1: 單側先行) F000904 topic =F19120601, subtopic = MOVING_HORIZON
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string MOVING_HORIZON { get; set; }

		/// <summary>
		/// 垂直動線 (0: 由小到大 1:由大到小) F000904 topic=F19120601, subtopic=MOVING_VERTICAL
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string MOVING_VERTICAL { get; set; }

		/// <summary>
		/// 處理狀態 (0: 建立 9: 刪除)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string PROC_FLAG { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

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
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

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
	}
}
