using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.F05
{
	/// <summary>
	/// 總量揀貨身檔
	/// </summary>
	[Serializable]
	[DataServiceKey("PICK_ORD_NO", "TTL_PICK_SEQ", "GUP_CODE", "CUST_CODE", "DC_CODE")]
	[Table("F051203")]
	public class F051203
	{
		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 揀貨單號
		/// </summary>
		[Key]
		[Required]
		public string PICK_ORD_NO { get; set; }

		/// <summary>
		/// 總量揀貨序號
		/// </summary>
		[Key]
		[Required]
		public string TTL_PICK_SEQ { get; set; }

		/// <summary>
		/// 揀貨儲位
		/// </summary>
		[Required]
		public string PICK_LOC { get; set; }

		/// <summary>
		/// 商品編號
		/// </summary>
		[Required]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 預定揀貨數量
		/// </summary>
		[Required]
		public int B_PICK_QTY { get; set; }

		/// <summary>
		/// 實際揀貨數量
		/// </summary>
		[Required]
		public int A_PICK_QTY { get; set; }

		/// <summary>
		/// 有效日期
		/// </summary>
		[Required]
		public DateTime VALID_DATE { get; set; }

		/// <summary>
		/// 商品批號
		/// </summary>
		public string MAKE_NO { get; set; }

		/// <summary>
		/// 商品序號
		/// </summary>
		public string SERIAL_NO { get; set; }


		/// <summary>
		/// 揀貨狀態0待揀貨1揀貨完成
		/// </summary>
		[Required]
		public string PICK_STATUS { get; set; }

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
		/// 路順
		/// </summary>
		public int? ROUTE_SEQ { get; set; }

		/// <summary>
		/// PK區編號/倉庫編號
		/// </summary>
		public string PK_AREA { get; set; }

		/// <summary>
		/// PK區名稱/倉庫名稱
		/// </summary>

		public string PK_AREA_NAME { get; set; }
	}
}
