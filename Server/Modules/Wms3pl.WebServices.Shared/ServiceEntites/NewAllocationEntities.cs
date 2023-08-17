using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Shared.Enums;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
  #region 調撥單
  /// <summary>
  /// 建立/重建調撥單參數
  /// </summary>
  public class NewAllocationItemParam
  {
    #region 主檔設定
    /// <summary>
    /// 是否檢核是否已配庫中
    /// </summary>
    public bool IsCheckExecStatus { get; set; } = true;

    /// <summary>
    /// 調撥日期(未設定預設為今天)
    /// </summary>
    public DateTime? AllocationDate { get; set; }

    /// <summary>
    /// 過帳日期
    /// </summary>
    public DateTime? PostingDate { get; set; }

    /// <summary>
    /// 來源單據類型
    /// </summary>
    public string SourceType { get; set; }

    /// <summary>
    /// 來源單據號碼
    /// </summary>
    public string SourceNo { get; set; }

    /// <summary>
    /// 調撥類型(預設為有來源倉也有目的倉)
    /// </summary>
    public AllocationType AllocationType { get; set; } = AllocationType.Both;

    /// <summary>
    /// 業主
    /// </summary>
    public string GupCode { get; set; }
    /// <summary>
    /// 貨主
    /// </summary>
    public string CustCode { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string Memo { get; set; }

    /// <summary>
    /// 是否派車
    /// </summary>
    public bool? SendCar { get; set; }

    /// <summary>
    /// 是否展開效期和入庫日
    /// </summary>
    public bool? IsExpendDate { get; set; } = true;

    /// <summary>
    /// 調撥單類型 (0: 一般調撥單、1: 虛擬庫存回復單、2: 每日補貨單、3:配庫補貨單、4:驗收上架單、5:補貨純下架單、6:補貨純上架單、8:即期品調撥單)
    /// </summary>
    public string AllocationTypeCode { get; set; } = "0";

    /// <summary>
    /// 棧板/載具/容器編號
    /// </summary>
    public string ContainerCode { get; set; }

    /// <summary>
    /// 容器F0701.ID
    /// </summary>
    public Int64? F0701_ID { get; set; }

    /// <summary>
    /// 補貨預定上架倉別
    /// </summary>
    public string PRE_TAR_WAREHOUSE_ID { get; set; }

    #endregion

    #region 來源設定
    /// <summary>
    /// 來源物流中心(純上架可不設定)
    /// </summary>
    public string SrcDcCode { get; set; }

    /// <summary>
    /// 來源商品明細篩選(純上架可不設定)
    /// </summary>
    public List<StockFilter> SrcStockFilterDetails { get; set; }

    /// <summary>
    /// 來源倉別(純上架可不設定)
    /// </summary>
    public string SrcWarehouseId { get; set; }

    /// <summary>
    /// 指定來源倉別型態(可不設定)
    /// </summary>
    public string SrcWarehouseType { get; set; }

    /// <summary>
    /// 指定儲區型態(A:一般揀貨區,B:黃金揀貨區,C:補貨區)
    /// </summary>
    public string ATypeCode { get; set; }
    #endregion

    #region 目的設定
    /// <summary>
    /// 目的物流中心(純下架可不設定)
    /// </summary>
    public string TarDcCode { get; set; }

    /// <summary>
    /// 指定目的倉別(純下架可不設定)
    /// </summary>
    public string TarWarehouseType { get; set; }
    /// <summary>
    /// 指定目的倉別Id(純下架可不設定)
    /// </summary>
    public string TarWarehouseId { get; set; }

    /// <summary>
    /// 指定來源與目的儲位對應(無指定可不設定)(純上架、純下架不須設定)
    /// </summary>
    public List<SrcLocMapTarLoc> SrcLocMapTarLocs { get; set; }

    /// <summary>
    /// 取建議儲位時是否包含補貨區
    /// </summary>
    public bool isIncludeResupply { get; set; } = true;

    /// <summary>
    /// 不允許同商品拆儲位
    /// </summary>
    public bool NotAllowSeparateLoc { get; set; }

    #endregion

    #region for 重建調撥單使用
    /// <summary>
    /// 原調撥單號
    /// </summary>
    public string OrginalAllocationNo { get; set; }
    /// <summary>
    /// 原來源單號
    /// </summary>
    public string OrginalSourceNo { get; set; }
    /// <summary>
    /// 是否刪除原調撥單號或來源單號
    /// </summary>
    public bool IsDeleteOrginalAllocation { get; set; } = false;

    /// <summary>
    /// 排除要刪除的調撥單號
    /// </summary>
    public List<string> ExcludeDeleteAllocationNos { get; set; }
    #endregion

    /// <summary>
    /// 序號清單(for 序號商品)
    /// </summary>
    public List<string> SerialNos { get; set; }

    /// <summary>
    /// 已調整後庫存清單
    /// </summary>
    public List<F1913> ReturnStocks { get; set; }

    /// <summary>
    /// 調撥明細(純上架使用)
    /// </summary>
    public List<StockDetail> StockDetails { get; set; }

    /// <summary>
    /// 是否為搬移單
    /// </summary>
    public bool IsMoveOrder { get; set; }
  }

  /// <summary>
  /// 庫存查詢
  /// </summary>
  public class StockFilter
  {

    /// <summary>
    /// 來源儲位
    /// </summary>
    public string LocCode { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 數量
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// 指定序號綁儲位序號(可不指定)
    /// </summary>
    public List<string> SerialNos { get; set; }

    /// <summary>
    /// 指定入庫日(可不指定)
    /// </summary>
    public List<DateTime> EnterDates { get; set; }

    /// <summary>
    /// 指定效期(可不指定)
    /// </summary>
    public List<DateTime> ValidDates { get; set; }


    /// <summary>
    /// 指定廠商編號(可不指定)
    /// </summary>
    public List<string> VnrCodes { get; set; }

    /// <summary>
    /// 指定箱號(可不指定)
    /// </summary>
    public List<string> BoxCtrlNos { get; set; }
    /// <summary>
    /// 指定板號(可不指定)
    /// </summary>
    public List<string> PalletCtrlNos { get; set; }

    /// <summary>
    ///  是否允續調撥過期商品
    /// </summary>
    public bool isAllowExpiredItem { get; set; }

    /// <summary>
    /// 資料序號(可不設定) 
    /// 若有設定 
    /// 1.可與指定上架儲位對應 
    /// 2.取得建議儲位也依照ID當群組取一筆建議儲位
    /// </summary>
    public int? DataId { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    public List<string> MakeNos { get; set; }

    /// <summary>
    /// 指定來源倉庫(可不指定，如果都是同一個倉，由外部的來源倉指定)
    /// </summary>
    public string SrcWarehouseId { get; set; }

  }

  /// <summary>
  /// 庫存明細
  /// </summary>
  public class StockDetail
  {
    /// <summary>
    /// 業主
    /// </summary>
    public string GupCode { get; set; }
    /// <summary>
    /// 貨主
    /// </summary>
    public string CustCode { get; set; }
    /// <summary>
    /// 來源營業單位
    /// </summary>
    public string SrcDcCode { get; set; }
    /// <summary>
    /// 來源倉號
    /// </summary>
    public string SrcWarehouseId { get; set; }
    /// <summary>
    /// 來源儲位
    /// </summary>
    public string SrcLocCode { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 效期
    /// </summary>
    public DateTime ValidDate { get; set; }
    /// <summary>
    /// 入庫日
    /// </summary>
    public DateTime EnterDate { get; set; }
    /// <summary>
    /// 數量
    /// </summary>
    public decimal Qty { get; set; }
    /// <summary>
    /// 廠商
    /// </summary>
    public string VnrCode { get; set; }
    /// <summary>
    /// 序號
    /// </summary>
    public string SerialNo { get; set; }

    /// <summary>
    /// 盒號
    /// </summary>
    public string BoxNo { get; set; }
    /// <summary>
    /// 儲值卡盒號
    /// </summary>
    public string BatchNo { get; set; }
    /// <summary>
    /// 箱號
    /// </summary>
    public string CaseNo { get; set; }

    /// <summary>
    /// 箱版管理-箱號
    /// </summary>
    public string BoxCtrlNo { get; set; }

    /// <summary>
    /// 箱版管理-板號
    /// </summary>
    public string PalletCtrlNo { get; set; }

    /// <summary>
    /// 目的物流中心
    /// </summary>
    public string TarDcCode { get; set; }

    /// <summary>
    /// 目的倉號
    /// </summary>
    public string TarWarehouseId { get; set; }
    /// <summary>
    /// 目的上架儲位
    /// </summary>
    public string TarLocCode { get; set; }

    /// <summary>
    /// 目的上架廠商
    /// </summary>
    public string TarVnrCode { get; set; }

    /// <summary>
    /// 目的效期
    /// </summary>
    public DateTime? TarValidDate { get; set; }

    /// <summary>
    /// 目的批號
    /// </summary>
    public string TarMakeNo { get; set; }

    /// <summary>
    /// 目的箱號
    /// </summary>
    public string TarBoxCtrlNo { get; set; }

    /// <summary>
    /// 目的板號
    /// </summary>
    public string TarPalletCtrlNo { get; set; }

    /// <summary>
    /// 資料序號(可不設定) 
    /// 若有設定 
    /// 1.可與指定上架儲位對應 
    /// 2.取得建議儲位也依照ID當群組取一筆建議儲位
    /// </summary>
    public int? DataId { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    public string MAKE_NO { get; set; }

    /// <summary>
    /// 作業工具 F191902.MOVE_TOOL
    /// </summary>
    public string MOVE_TOOL { get; set; }

    /// <summary>
    /// 容器分隔條碼
    /// </summary>
    public string BinCode { get; set; }

    /// <summary>
    /// 來源單據類型
    /// </summary>
    public string SourceType { get; set; }

    /// <summary>
    /// 來源單號
    /// </summary>
    public string SourceNo { get; set; }

    /// <summary>
    /// 參考單號 (例如進倉的驗收單，因為有兩層，需要另外紀錄)
    /// </summary>
    public string ReferenceNo { get; set; }

    /// <summary>
    /// 參考序號
    /// </summary>
    public string ReferenceSeq { get; set; }

    /// <summary>
    /// 補貨預定上架倉別
    /// </summary>
    public string PRE_TAR_WAREHOUSE_ID { get; set; }

    /// <summary>
    /// 調撥類型(預設為有來源倉也有目的倉)
    /// </summary>
    public AllocationType AllocationType { get; set; }

    /// <summary>
    /// 調撥單類型 (0: 一般調撥單、1: 虛擬庫存回復單、2: 每日補貨單、3:配庫補貨單(含純上架單)、4:驗收上架單、5:補貨純下架單)
    /// </summary>
    public string AllocationTypeCode { get; set; }
  }

  /// <summary>
  /// 來源儲位商品對應目的儲位(來源資料設定不可重複)
  /// </summary>
  public class SrcLocMapTarLoc
  {
    #region 來源欄位對應
    /// <summary>
    /// 來源儲位(必填)
    /// </summary>
    public string SrcLocCode { get; set; }
    /// <summary>
    /// 品號(必填)
    /// </summary>
    public string ItemCode { get; set; }

    /// <summary>
    /// 來源效期(可不指定)
    /// </summary>
    public DateTime? ValidDate { get; set; }
    /// <summary>
    /// 來源入庫日(可不指定)
    /// </summary>
    public DateTime? EnterDate { get; set; }
    /// <summary>
    /// 來源序號(可不指定)
    /// </summary>
    public string SerialNo { get; set; }
    /// <summary>
    /// 來源廠商(可不指定)
    /// </summary>
    public string VnrCode { get; set; }
    /// <summary>
    /// 來源箱號(可不指定)
    /// </summary>
    public string BoxCtrlNo { get; set; }
    /// <summary>
    /// 來源板號(可不指定)
    /// </summary>
    public string PalletCtrlNo { get; set; }
    /// <summary>
    /// 來源批號(可不指定)
    /// </summary>
    public string MakeNo { get; set; }

    /// <summary>
    /// 資料序號(可不設定) 
    /// 若有設定 
    /// 1.可與指定上架儲位對應 
    /// 2.取得建議儲位也依照ID當群組取一筆建議儲位
    /// </summary>
    public int? DataId { get; set; }

    #endregion
    #region 指定目的欄位對應
    /// <summary>
    /// 指定目的倉庫(可不設定)(有指定目的儲位可不設定)
    /// </summary>
    public string TarWarehouseId { get; set; }

    /// <summary>
    /// 指定目的儲位(可不設定)
    /// </summary>
    public string TarLocCode { get; set; }

    /// <summary>
    /// 指定目的廠商(可不設定)
    /// </summary>
    public string TarVnrCode { get; set; }

    /// <summary>
    /// 指定目的效期(可不設定)
    /// </summary>
    public DateTime? TarValidDate { get; set; }
    /// <summary>
    /// 指定目的批號(可不設定)
    /// </summary>
    public string TarMakeNo { get; set; }

    /// <summary>
    /// 指定目的箱號(可不設定)
    /// </summary>
    public string TarBoxCtrlNo { get; set; }

    /// <summary>
    /// 指定目的棧板(可不設定)
    /// </summary>
    public string TarPalletCtrlNo { get; set; }

    /// <summary>
    /// 容器分隔條碼
    /// </summary>
    public string BinCode { get; set; }
    /// <summary>
    /// 來源單據類型
    /// </summary>
    public string SourceType { get; set; }
    /// <summary>
    /// 來源單號
    /// </summary>
    public string SourceNo { get; set; }
    /// <summary>
    /// 參考單號 (例如進倉的驗收單，因為有兩層，需要另外紀錄)
    /// </summary>
    public string ReferenceNo { get; set; }
    /// <summary>
    /// 參考序號
    /// </summary>
    public string ReferenceSeq { get; set; }
    #endregion

  }

  /// <summary>
  /// 建立/重建調撥單結果
  /// </summary>
  public class ReturnNewAllocationResult
  {
    /// <summary>
    /// 調撥類型(預設為有來源倉也有目的倉)
    /// </summary>
    public AllocationType AllocationType { get; set; } = AllocationType.Both;
    /// <summary>
    /// 調撥單結果
    /// </summary>
    public ExecuteResult Result { get; set; }

    /// <summary>
    /// 調撥單
    /// </summary>
    public List<ReturnNewAllocation> AllocationList { get; set; }

    /// <summary>
    /// 調撥後庫存清單
    /// </summary>
    public List<F1913> StockList { get; set; }
    /// <summary>
    /// 調撥商品序號(非序號綁儲位)
    /// </summary>
    public List<F2501> SerialDataList { get; set; }



  }
  /// <summary>
  /// 調撥單
  /// </summary>
  public class ReturnNewAllocation
  {
    /// <summary>
    /// 調撥單主檔
    /// </summary>
    public F151001 Master { get; set; }
    /// <summary>
    /// 調撥單明細
    /// </summary>
    public List<F151002> Details { get; set; }
    /// <summary>
    /// 調撥單虛擬儲位
    /// </summary>
    public List<F1511> VirtualLocList { get; set; }
    /// <summary>
    /// 調撥單序號刷讀紀錄
    /// </summary>
    public List<F15100101> SerialLogList { get; set; }

    /// <summary>
    /// 建議儲位紀錄表
    /// </summary>
    public List<F191204> SuggestLocRecordList { get; set; }
  }

  /// <summary>
  ///  刪除調撥單參數
  /// </summary>
  public class DeletedAllocationParam
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
    /// 刪除調撥單方式
    /// </summary>
    public DeleteAllocationType DeleteAllocationType { get; set; }
    /// <summary>
    /// 原調撥單號
    /// </summary>
    public string OrginalAllocationNo { get; set; }
    /// <summary>
    ///  原來源單號
    /// </summary>
    public string OrginalSourceNo { get; set; }
    /// <summary>
    /// 已調整後庫存清單
    /// </summary>
    public List<F1913> F1913List { get; set; }
    /// <summary>
    /// 排除要刪除的調撥單號
    /// </summary>
    public List<string> ExcludeDeleteAllocationNos { get; set; }
  }

  /// <summary>
  /// 刪除調撥單結果
  /// </summary>
  public class DeletedAllocationInfo
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
    /// 調撥單主檔
    /// </summary>
    public List<F151001> F151001List { get; set; }
    /// <summary>
    /// 調撥單明細
    /// </summary>
    public List<F151002> F151002List { get; set; }
    /// <summary>
    /// 已調整後庫存清單
    /// </summary>
    public List<F1913> F1913List { get; set; }
  }
  #endregion

	#region 調撥單共用涵式參數物件
	public class AllocationConfirmParam
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
		/// 調撥單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 開始時間(yyyy/MM/dd HH:mm:ss)
		/// </summary>
		public string StartTime { get; set; }
		/// <summary>
		/// 完成時間(yyyy/MM/dd HH:mm:ss)
		/// </summary>
		public string CompleteTime { get; set; }
		/// <summary>
		/// 上/架人員
		/// </summary>
		public string Operator { get; set; }
    /// <summary>
    /// 容器編號
    /// </summary>
    public string ContainerCode { get; set; }
		/// <summary>
		/// 調撥明細清單
		/// </summary>
		public List<AllocationConfirmDetail> Details { get; set; }
    /// <summary>
    /// 建立容器資料的回傳結果
    /// </summary>
    public List<ContainerExecuteResult> ContainerResults { get; set; }

  }

  public class AllocationConfirmDetail
	{
		/// <summary>
		/// 調撥項次編號
		/// </summary>
		public short Seq { get; set; }
		/// <summary>
		/// 實際數量
		/// </summary>
		public int Qty { get; set; }
		/// <summary>
		/// 指定上架儲位
		/// </summary>
		public string TarLocCode { get; set; }
    /// <summary>
    /// 容器編號
    /// </summary>
    public string ContainerCode { get; set; }

  }
  #endregion

  #region 調撥庫存異常處理參數
  public class AllocationStockLack
    {
        public string DcCode { get; set; }
        public string GupCode { get; set; }
        public string CustCode { get; set; }
        public int LackQty { get; set; }
        public string LackWarehouseId { get; set; }
        public string LackLocCode { get; set; }
        public F151002 F151002 { get; set; }
        public F1511 F1511 { get; set; }
        public List<F1913> ReturnStocks { get; set; }
    }

  public class AllocationStockLackkResult : ExecuteResult
  {
    public List<ReturnNewAllocation> ReturnNewAllocations { get; set; }
    public List<F1913> ReturnStocks { get; set; }
    public List<F191302> AddF191302List { get; set; }
    public F151002 UpdF151002 { get; set; }
    public F1511 UpdF1511 { get; set; }
  }

  #endregion 調撥庫存異常處理參數
}
