using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Entities
{
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ReportParam
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 物流中心(若值為0或空代表全部)
		/// </summary>
		public string DcCode { get; set; }
		[DataMember]
		/// <summary>
		/// 業主(若值為0或空代表全部)
		/// </summary>
		public string GupCode { get; set; }
		[DataMember]
		/// <summary>
		/// 貨主(若值為0或空代表全部)
		/// </summary>
		public string CustCode { get; set; }
		[DataMember]
		public DateTime? SDate { get; set; }
		[DataMember]
		public DateTime? EDate { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ReportOrderByMonth
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 月份(YYYY/MM)
		/// </summary>
		public string ORDERMONTH { get; set; }
		[DataMember]
		/// <summary>
		/// 業主名稱
		/// </summary>
		public string GUP_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 貨主名稱
		/// </summary>
		public string CUST_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 訂單數
		/// </summary>
		public int ORDCOUNT { get; set; }
		[DataMember]
		/// <summary>
		/// 取消訂單數
		/// </summary>
		public int ORDCOUNT_CANCEL { get; set; }
		[DataMember]
		/// <summary>
		/// 出貨單數
		/// </summary>
		public int DELVCOUNT { get; set; }
		[DataMember]
		/// <summary>
		/// 有效出貨單數(不含取消出貨單)
		/// </summary>
		public int DELVCOUNT_NOCANCEL { get; set; }
		/// <summary>
		/// 工作天數
		/// </summary>
		[DataMember]
		public int WORK_DAY { get; set; }
		/// <summary>
		/// 平均出貨單數(有效出貨單數(不含取消出貨單)/工作天數)
		/// </summary>
		[DataMember]
		public decimal AVG_DELVCOUNT { get; set; }
		
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ReportDelvByMonth
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 月份(YYYY/MM)
		/// </summary>
		public string DELVMONTH { get; set; }
		[DataMember]
		/// <summary>
		/// 業主名稱
		/// </summary>
		public string GUP_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 貨主名稱
		/// </summary>
		public string CUST_NAME { get; set; }
	
		[DataMember]
		/// <summary>
		/// 配送方式(宅配、711超取、全家超取)
		/// </summary>
		public string DELV_TYPE { get; set; }

		[DataMember]
		/// <summary>
		/// 有效出貨單數(不含取消出貨單)
		/// </summary>
		public int DELVCOUNT_NOCANNEL { get; set; }
		[DataMember]
		/// <summary>
		/// 代收總金額
		/// </summary>
		public decimal COLLECT_AMT { get; set; }
	}

	
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ReportStockByMonth
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 月份(YYYY/MM)
		/// </summary>
		public string STOCKMONTH { get; set; }
		[DataMember]
		/// <summary>
		/// 業主名稱
		/// </summary>
		public string GUP_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 貨主名稱
		/// </summary>
		public string CUST_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 進倉單數(結案)
		/// </summary>
		public int STOCKCOUNT { get; set; }
		[DataMember]
		/// <summary>
		/// 進倉驗收品項數
		/// </summary>
		public int RECV_ITEMCOUNT { get; set; }
		[DataMember]
		/// <summary>
		/// 進倉驗收商品數量
		/// </summary>
		public int RECV_ITEMQTY { get; set; }
	}
}
