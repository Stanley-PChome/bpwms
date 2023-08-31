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
	[DataServiceKey("ID")]
	[Table("F190106")]
	public class F190106 : IAuditInfo
	{
		/// <summary>
		///  流水號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "int")]
		public int ID { get; set; }

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 01:配庫 02:揀貨
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string SCHEDULE_TYPE { get; set; }

		/// <summary>
		/// 開始時間(HH:mm)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string START_TIME { get; set; }

		/// <summary>
		/// 結束時間(HH:mm)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string END_TIME { get; set; }

		/// <summary>
		/// 每幾分鐘執行一次
		/// </summary>
		[Required]
		[Column(TypeName = "tinyint")]
		public Byte PERIOD { get; set; }

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
