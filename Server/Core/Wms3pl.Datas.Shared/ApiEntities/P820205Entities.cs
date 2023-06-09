using System.Collections.Generic;
using Wms3pl.Datas.F05;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 批次訂單轉入排程_傳入
	/// </summary>
	public class BatchTransApiOrdersReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
	}

	public class F050102WithF05010301WithF50104Model
	{
		public List<F050102> F050102List { get; set; }
		public List<F05010301> F05010301List { get; set; }
		public List<F050104> F050104List { get; set; }
	}

	public class CreateF050102TmpModel
	{
		public string ChannelItemNo { get; set; }
		public string ItemCode { get; set; }
		public string ItemSeq { get; set; }
		public string ItemDesc { get; set; }
		public int Qty { get; set; }
		public string VnrCode { get; set; }
		public string MakeNo { get; set; }
    public string serialNo { get; set; }
		public List<PostCreateOrderServiceItemsModel> ServiceItemDetails { get; set; }
	}
}
