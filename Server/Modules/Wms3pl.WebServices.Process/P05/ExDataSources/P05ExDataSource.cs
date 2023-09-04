using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F05;

namespace Wms3pl.WebServices.Process.P05.ExDataSources
{
	public partial class P05ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<F051201Data> F051201Datas
		{
			get { return new List<F051201Data>().AsQueryable(); }
		}
		public IQueryable<F051202Data> F051202Datas
		{
			get { return new List<F051202Data>().AsQueryable(); }
		}
		public IQueryable<F051201SelectedData> F051201SelectedDatas
		{
			get { return new List<F051201SelectedData>().AsQueryable(); }
		}
		public IQueryable<F051201ReportDataA> F051201ReportDataAs
		{
			get { return new List<F051201ReportDataA>().AsQueryable(); }
		}
		public IQueryable<F051201ReportDataB> F051201ReportDataBs
		{
			get { return new List<F051201ReportDataB>().AsQueryable(); }
		}
		public IQueryable<F050110ReportCustOrdNoData> F050110ReportCustOrdNoDatas
		{
			get { return new List<F050110ReportCustOrdNoData>().AsQueryable(); }
		}

		#region F0513 - 出貨碼頭分配
		public IQueryable<F0513WithF1909> F0513WithF1909s
		{
			get { return new List<F0513WithF1909>().AsQueryable(); }
		}
		#endregion

		#region F0513 - 扣帳作業-批次
		public IQueryable<F0513WithF050801Batch> F0513WithF050801Batchs
		{
			get { return new List<F0513WithF050801Batch>().AsQueryable(); }
		}
		#endregion

		#region F050801 - 出貨抽稽維護
		public IQueryable<F050801WithF055001> F050801WithF055001s
		{
			get { return new List<F050801WithF055001>().AsQueryable(); }
		}
		#endregion

		#region F050805 - 總庫試算缺貨檔
		public IQueryable<F050805Data> F050805Datas
		{
			get { return new List<F050805Data>().AsQueryable(); }
		}
		public IQueryable<F05080501Data> F05080501Datas
		{
			get { return new List<F05080501Data>().AsQueryable(); }
		}
		public IQueryable<F05080502Data> F05080502Datas
		{
			get { return new List<F05080502Data>().AsQueryable(); }
		}
		public IQueryable<F05080504Data> F05080504Datas
		{
			get { return new List<F05080504Data>().AsQueryable(); }
		}
		#endregion

		#region 改到P06ExDataSource
		#region 缺貨作業
		public IQueryable<F051206Pick> F051206Picks
		{
			get { return new List<F051206Pick>().AsQueryable(); }
		}
		public IQueryable<F051206AllocationList> F051206AllocationLists
		{
			get { return new List<F051206AllocationList>().AsQueryable(); }
		}
		public IQueryable<F051206LackList> F051206LackLists
		{
			get { return new List<F051206LackList>().AsQueryable(); }
		}
		#endregion
		#endregion

		public IQueryable<F0010List> F0010Lists
		{
			get { return new List<F0010List>().AsQueryable(); }
		}

		public IQueryable<F050801StatisticsData> F050801StatisticsDatas
		{
			get { return new List<F050801StatisticsData>().AsQueryable(); }
		}
		public IQueryable<PickingStatistics> PickStatisticss
		{
			get { return new List<PickingStatistics>().AsQueryable(); }
		}

		#region 訂單主檔
		public IQueryable<F050101Ex> F050101Exs
		{
			get { return new List<F050101Ex>().AsQueryable(); }
		}
		#endregion

		#region 訂單明細
		public IQueryable<F050102Ex> F050102Exs
		{
			get { return new List<F050102Ex>().AsQueryable(); }
		}
		#endregion

		#region Excel訂單明細
		public IQueryable<F050102Excel> F050102Excels
		{
			get { return new List<F050102Excel>().AsQueryable(); }
		}
		#endregion

		#region 訂單維護，點選出貨明細會用到(P0503020100)
		public IQueryable<F050102WithF050801> F050102WithF050801s
		{
			get { return new List<F050102WithF050801>().AsQueryable(); }
		}
		public IQueryable<P05030201BasicData> P05030201BasicDatas
		{
			get { return new List<P05030201BasicData>().AsQueryable(); }
		}
        public IQueryable<F051202WithF055002> F051202WithF055002s
        {
            get { return new List<F051202WithF055002>().AsQueryable(); }
        }
        #endregion

        #region 未出貨訂單查詢
        public IQueryable<F050801NoShipOrders> F050801NoShipOrderss
		{
			get { return new List<F050801NoShipOrders>().AsQueryable(); }
		}
		#endregion

		#region 貨主訂單手動挑單
		public IQueryable<F050001Data> F050001Datas
		{
			get { return new List<F050001Data>().AsQueryable(); }
		}
		#endregion

		#region 合流作業
		public IQueryable<F050802GroupItem> F050802GroupItems
		{
			get { return new List<F050802GroupItem>().AsQueryable(); }
		}
		#endregion

		#region F710802 作業異動查詢
		public IQueryable<P710802SearchResult> P710802SearchResults
		{
			get { return new List<P710802SearchResult>().AsQueryable(); }
		}
		#endregion

		#region 出貨單 WMS_ORD_NO 對應 050301 相關單據資料
		public IQueryable<F050301WmsOrdNoData> F050301WmsOrdNoDatas
		{
			get { return new List<F050301WmsOrdNoData>().AsQueryable(); }
		}
		#endregion

		#region 出貨發票報表主檔
		public IQueryable<F050801MainDataRpt> F050801MainDataRpts
		{
			get { return new List<F050801MainDataRpt>().AsQueryable(); }
		}
		#endregion

		#region 出貨發票報表明細檔
		public IQueryable<F050801DetailDataRpt> F050801DetailDataRpts
		{
			get { return new List<F050801DetailDataRpt>().AsQueryable(); }
		}
		#endregion

		#region F050801 出貨單 Item 明細
		public IQueryable<F050801ItemData> F050801ItemDatas
		{
			get { return new List<F050801ItemData>().AsQueryable(); }
		}
		#endregion

		#region  F051202VolumnData 揀貨明細更新儲位容積量
		public IQueryable<F051202VolumnData> F051202VolumnDatas
		{
			get { return new List<F051202VolumnData>().AsQueryable(); }
		}
		#endregion

		#region P050303出貨查詢結果
		public IQueryable<P050303QueryItem> P050301QueryItems
		{
			get { return new List<P050303QueryItem>().AsQueryable(); }
		}
		#endregion

		#region F051205Data 
		public IQueryable<F051205Data> F051205Datas
		{
			get { return new List<F051205Data>().AsQueryable(); }
		}
		#endregion

		#region F050801WithBill 出貨拆單
		public IQueryable<F050801WithBill> F050801WithBills
		{
			get { return new List<F050801WithBill>().AsQueryable(); }
		}
		#endregion

		#region F700101CarData 批次出車時段
		public IQueryable<F700101CarData> F700101CarDatas
		{
			get { return new List<F700101CarData>().AsQueryable(); }
		}
		#endregion

		#region F050303 包裝記錄用
		public IQueryable<F055002WithGridLog> F055002WithGridLogs
		{
			get { return new List<F055002WithGridLog>().AsQueryable(); }
		}
		#endregion

		#region F050304 接單平台訂單檔
		public IQueryable<F050304AddEService> F050304Exs
		{
			get { return new List<F050304AddEService>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F05010103> F05010103s
		{
			get { return new List<F05010103>().AsQueryable(); }
		}

		#region 匯總報表
		public IQueryable<P050103PickTime> P050103PickTimes
		{
			get { return new List<P050103PickTime>().AsQueryable(); }
		}

		public IQueryable<P050103PickOrdNo> P050103PickOrdNos
		{
			get { return new List<P050103PickOrdNo>().AsQueryable(); }
		}

		public IQueryable<P050103WmsOrdNo> P050103WmsOrdNos
		{
			get { return new List<P050103WmsOrdNo>().AsQueryable(); }
		}
		public IQueryable<P050103ReportData> F190906Datas
		{
			get { return new List<P050103ReportData>().AsQueryable(); }
		}
		#endregion

		#region F050110 倉別種類
		public IQueryable<WarehouseTypeList> WarehouseTypeLists
		{
			get { return new List<WarehouseTypeList>().AsQueryable(); }
		}
		#endregion

		#region F050110 查詢儲區揀貨列印資料

		public IQueryable<F050110DataSearch> F050110DataSearchs
		{
			get { return new List<F050110DataSearch>().AsQueryable(); }
		}
        public IQueryable<F050110RetailCodeData> F050110RetailCodeDatas
        {
            get { return new List<F050110RetailCodeData>().AsQueryable(); }
        }
        public IQueryable<F050110ReportData> F050110ReportDatas
		{
			get { return new List<F050110ReportData>().AsQueryable(); }
		}
		public IQueryable<F050110ReportStickerData> F050110ReportStickerDatas
		{
			get { return new List<F050110ReportStickerData>().AsQueryable(); }
		}
		#endregion

		#region  P050112 揀貨彙總作業
		public IQueryable<P050112Pick> P050112Picks
		{
			get { return new List<P050112Pick>().AsQueryable(); }
		}

		public IQueryable<P050112Batch> P050112Batches
		{
			get { return new List<P050112Batch>().AsQueryable(); }
		}

		public IQueryable<PutReportData> PutReportDatas
		{
			get { return new List<PutReportData>().AsQueryable(); }
		}

		public IQueryable<PickReportData> PickReportDatas
		{
			get { return new List<PickReportData>().AsQueryable(); }
		}

		public IQueryable<BatchPickStation> BatchPickStations
		{
			get { return new List<BatchPickStation>().AsQueryable(); }
		}
        #endregion

        #region 出貨單批次參數維護

        public IQueryable<F050004WithF190001> F050004WithF190001s
        {
            get { return new List<F050004WithF190001>().AsQueryable(); }
        }

        #endregion

        #region 配庫試算結果查詢

        public IQueryable<P0503050000CalHead> P0503050000CalHeads
        {
            get { return new List<P0503050000CalHead>().AsQueryable(); }
        }

        #endregion

        #region F050113 查詢揀貨彙總列印資料

        public IQueryable<F050113DataSearch> F050113DataSearchs
        {
            get { return new List<F050113DataSearch>().AsQueryable(); }
        }
        public IQueryable<F050113ReportData> F050113ReportDatas
        {
            get { return new List<F050113ReportData>().AsQueryable(); }
        }

        public IQueryable<F050113ReportCustOrdNoData> F050113ReportCustOrdNoDatas
        {
            get { return new List<F050113ReportCustOrdNoData>().AsQueryable(); }
        }
        #endregion

        #region B2C揀貨單列印/補印
        public IQueryable<RP0501010004Model> RP0501010004Models
        {
            get { return new List<RP0501010004Model>().AsQueryable(); }
        }

        public IQueryable<RP0501010005Model> RP0501010005Models
        {
            get { return new List<RP0501010005Model>().AsQueryable(); }
        }
		#endregion

		#region 揀貨單列印
		public IQueryable<BatchPickNoList> BatchPickNoLists
		{
			get { return new List<BatchPickNoList>().AsQueryable(); }
		}

		public IQueryable<RePickNoList> RePickNoLists
		{
			get { return new List<RePickNoList>().AsQueryable(); }
		}

		public IQueryable<RePrintPickNoList> ReprintPickNoLists
		{
			get { return new List<RePrintPickNoList>().AsQueryable(); }
		}
		public IQueryable<SinglePickingReportData> SinglePickingReportDatas
		{
			get { return new List<SinglePickingReportData>().AsQueryable(); }
		}
		public IQueryable<BatchPickingReportData> BatchPickNoReportDatas
		{
			get { return new List<BatchPickingReportData>().AsQueryable(); }
		}
		public IQueryable<SinglePickingTickerData> SinglePickingTickerDatas
		{
			get { return new List<SinglePickingTickerData>().AsQueryable(); }
		}
		
		public IQueryable<BatchPickingTickerData> BatchPickingTickerDatas
		{
			get { return new List<BatchPickingTickerData>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F051201WithF051202> F051201WithF051202s
		{
			get { return new List<F051201WithF051202>().AsQueryable(); }
		}

		public IQueryable<PickDetail> PickDetails
		{
			get { return new List<PickDetail>().AsQueryable(); }
		}

		public IQueryable<F050901WithF055001> F050901WithF055001s
		{
			get { return new List<F050901WithF055001>().AsQueryable(); }
		}

    public IQueryable<F05080505Data> F05080505Datas
    {
      get { return new List<F05080505Data>().AsQueryable(); }
    }

    public IQueryable<F05080506Data> F05080506Datas
    {
      get { return new List<F05080506Data>().AsQueryable(); }
    }

    public IQueryable<PickContainer> PickContainer
    {
      get { return new List<PickContainer>().AsQueryable(); }
    }

    public IQueryable<OrderCancelInfo> OrderCancelInfo
    {
      get { return new List<OrderCancelInfo>().AsQueryable(); }
    }

    public IQueryable<DivideInfo> DivideInfo
    {
      get { return new List<DivideInfo>().AsQueryable(); }
    }

    public IQueryable<DivideDetail> DivideDetail
    {
      get { return new List<DivideDetail>().AsQueryable(); }
    }

    public IQueryable<CollectionRecord> CollectionRecord
    {
      get { return new List<CollectionRecord>().AsQueryable(); }
    }

    public IQueryable<ConsignmentDetail> ConsignmentDetail
    {
      get { return new List<ConsignmentDetail>().AsQueryable(); }
    }
  }
}