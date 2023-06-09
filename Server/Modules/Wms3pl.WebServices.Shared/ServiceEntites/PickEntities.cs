using System.Collections.Generic;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
	#region 揀貨單共用涵式參數物件
	public class PickConfirmParam
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
		/// <summary>
		/// 揀貨單號
		/// </summary>
		public string PickNo { get; set; }

		/// <summary>
		/// 指定開始時間(yyyy/MM/dd HH:mm:ss)
		/// </summary>
		public string StartTime { get; set; }
		/// <summary>
		/// 指定完成時間(yyyy/MM/dd HH:mm:ss)
		/// </summary>
		public string CompleteTime { get; set; }
		/// <summary>
		/// 調撥明細清單
		/// </summary>
		public List<PickConfirmDetail> Details { get; set; }
		/// <summary>
		/// 單據編號
		/// </summary>
		public string WmsNo { get; set; }
    /// <summary>
    /// 任務單號
    /// </summary>
    public string DocID { get; set; }
		/// <summary>
		/// 員工編號
		/// </summary>
		public string EmpId { get; set; }
    /// <summary>
    /// 待新增的容器資料
    /// </summary>
    public List<F060206> ContainerData { get; set; }
  }

  public class PickConfirmDetail
	{
		/// <summary>
		/// 揀貨項次編號
		/// </summary>
		public string Seq { get; set; }
		/// <summary>
		/// 實際揀貨數量
		/// </summary>
		public int Qty { get; set; }
	}
	#endregion

	#region 產生揀貨資料共用物件

	public class CreatePick
	{
		public List<F05030101> F05030101s { get; set; }
		public List<F05030202> F05030202s { get; set; }
		public List<F050801> F050801s { get; set; }
		public List<F050802> F050802s { get; set; }
		public List<F051201> F051201s { get; set; }
		public List<F051202> F051202s { get; set; }
		public List<F051203> F051203s { get; set; }
		public List<F1511> F1511s { get; set; }
		public F0513 F0513 { get; set; }
		public List<F051301> F051301s { get; set; }
		public List<F060201> F060201s { get; set; }
    public List<F050306_HISTORY> F050306_HISTORYs { get; set; }
	}

	public class WmsOrderItemSumQty
	{
		public string ItemCode { get; set; }
		public int Qty { get; set; }
	}

	#endregion


	public class StockLack
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public int LackQty { get; set; }
		public string PickLackWarehouseId { get; set; }
		public string PickLackLocCode { get; set; }
		public F051202 F051202 { get; set; }
		public F1511 F1511 { get; set; }
		public List<F1913> ReturnStocks { get; set; }
	}

	public class StockLackResult : ExecuteResult
	{
		public List<ReturnNewAllocation> ReturnNewAllocations { get; set; }
		public List<F1913> ReturnStocks { get; set; }
		public List<F191302> AddF191302List { get; set; }
		public F051202 UpdF051202 { get; set; }
		public F1511 UpdF1511 { get; set; }
	}

    public class AutoCreatePickRes : ApiResult
    {
        /// <summary>
        /// 處理的揀貨資料
        /// </summary>
        public List<F050306> ProcF050306s { get; set; }
    }
}
