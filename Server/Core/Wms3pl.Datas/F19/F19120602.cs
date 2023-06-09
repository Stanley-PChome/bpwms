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
	[DataServiceKey("ID")]
	[Table("F19120602")]
	/// <summary>
	/// PK區的路線範圍與路順明細設定
	/// </summary>
	public class F19120602 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Required]
		[Key]
		public Int64 ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]

		public string DC_CODE { get; set; }

		/// <summary>
		/// PK區編號
		/// </summary>
		[Required]
		public string PK_AREA { get; set; }

		/// <summary>
		/// PK區順序
		/// </summary>
		[Required]
		public int PK_LINE_SEQ { get; set; }

		/// <summary>
		/// 揀貨(實際)樓層
		/// </summary>
		[Required]
		public string PICK_FLOOR { get; set; }
		/// <summary>
		/// 路線類型0: 魚骨型(頭接頭)、1: S型(尾接尾) F000904 topic = F191206, subtopic = PICK_TYPE
		/// </summary>
		[Required]

		public string PICK_TYPE { get; set; }


		/// <summary>
		/// 儲位比對值(儲位前5碼)
		/// </summary>
		[Required]
		public string CHK_LOC_CODE { get; set; }
		/// <summary>
		/// 路線順序(2碼)
		/// </summary>
		[Required]

		public int LINE_SEQ { get; set; }

		/// <summary>
		/// 座別路順(3碼)
		/// </summary>
		[Required]
		public int PLAIN_SEQ { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }
	}
}
