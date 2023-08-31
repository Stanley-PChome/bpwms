using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	public class VendorReturnReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		public List<VnrReturn> VnrReturns { get; set; } = new List<VnrReturn>();

		/// <summary>
		/// 
		/// </summary>
		public class VnrReturn
		{
			/// <summary>
			/// 客戶廠退單號
			/// </summary>
			public string CustVnrReturnNo { get; set; }
			/// <summary>
			/// 單號類型 "RO": 還貨
			/// </summary>
			public string OrderType { get; set; }
			/// <summary>
			/// 處理狀態 0:單據建立 1:開始揀貨 2:開始包裝 5:廠退出貨完成 9:單據取消
			/// </summary>
			public string Status { get; set; }
			/// <summary>
			/// 作業時間(yyyy/MM/dd HH:mm:ss) 當狀態為0, 此為收到訂單的時間點。當狀態為1，此為開始揀貨的時間點，以此類推
			/// </summary>
			public string WorkTime { get; set; }
			public List<VnrReturnDetail> VnrReturnDetails { get; set; } = new List<VnrReturnDetail>();
		}

		public class VnrReturnDetail
		{
			/// <summary>
			/// 原單據的品號項次
			/// </summary>
			public string ItemSeq { get; set; }
			/// <summary>
			/// 商品編號
			/// </summary>
			public string ItemCode { get; set; }
			/// <summary>
			/// 實際廠商退貨數量
			/// </summary>
			public int ActQty { get; set; }
			public List<VnrReturnMakeNoDetail> MakeNoDetails { get; set; } = new List<VnrReturnMakeNoDetail>();
		}

		public class VnrReturnMakeNoDetail
		{
			/// <summary>
			/// 本商品的序號清單
			/// </summary>
			public List<string> SnList { get; set; } = new List<string>();
			/// <summary>
			/// 原單據的品號項次
			/// </summary>
			public string MakeNo { get; set; }
			/// <summary>
			/// 商品編號
			/// </summary>
			public int MakeNoQty { get; set; }
		}

		public class VnrReturnDetailTemp
		{
			public string Seq { get; set; }
			public string ItemSeq { get; set; }
			public string ItemCode { get; set; }
			public int ActQty { get; set; }
		}

		public class VnrReturnDetailCal
		{
			public string WmsOrdNo { get; set; }
			public string WmsOrdSeq { get; set; }
			public string Seq { get; set; }
			public string ItemSeq { get; set; }
			public string ItemCode { get; set; }
			public int A_ActQty { get; set; }
			public int B_ActQty { get; set; }
		}
	}
}
