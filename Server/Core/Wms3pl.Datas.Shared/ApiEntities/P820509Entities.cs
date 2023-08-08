using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	public class WcsStationReq
	{
		/// <summary>
		/// 業主編號=貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 列表筆數
		/// </summary>
		public int StationTotal { get; set; }
		public List<WcsStationModel> StationList { get; set; }
	}

	/// <summary>
	/// 明細資料
	/// </summary>
	public class WcsStationModel
	{
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string StationCode { get; set; }
		/// <summary>
		/// 工作站類型(PA1、PA2)
		/// </summary>
		public string StationType { get; set; }
		/// <summary>
		/// 工作站狀態(0=關站, 1=開站, 2=暫停 )
		/// </summary>
		public int Status { get; set; }
	}

	/// <summary>
	/// 容器釋放中介_回傳
	/// </summary>
	public class WcsStationResData
	{
		/// <summary>
		/// 錯誤工作站編號
		/// </summary>
		public string StationCode { get; set; }
		/// <summary>
		/// 錯誤欄位
		/// </summary>
		public string ErrorColumn { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<WcsErrorModel> errors { get; set; }
	}
}
