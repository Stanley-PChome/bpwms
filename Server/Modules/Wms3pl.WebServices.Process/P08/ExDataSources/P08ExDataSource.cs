using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P08.ExDataSources
{
	public partial class P08ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<F051201ReportDataA> F051201ReportDataAs
		{
			get { return new List<F051201ReportDataA>().AsQueryable(); }
		}

		public IQueryable<F0513PickTime> F0513PickTimes
		{
			get { return new List<F0513PickTime>().AsQueryable(); }
		}

		public IQueryable<F051201SelectedData> F051201SelectedDatas
		{
			get { return new List<F051201SelectedData>().AsQueryable(); }
		}
		#region 合流作業

		public IQueryable<F050802Data> F050802Datas
		{
			get { return new List<F050802Data>().AsQueryable(); }
		}

		public IQueryable<F052901> F052901s
		{
			get { return new List<F052901>().AsQueryable(); }
		}

		public IQueryable<F050802ItemName> F050802ItemNames
		{
			get { return new List<F050802ItemName>().AsQueryable(); }
		}

		public IQueryable<F052902Sum> F052902Sums
		{
			get { return new List<F052902Sum>().AsQueryable(); }
		}

		public IQueryable<F050802Report> F050802Reports
		{
			get { return new List<F050802Report>().AsQueryable(); }
		}
		public IQueryable<F051206Sum> F051206Sums
		{
			get { return new List<F051206Sum>().AsQueryable(); }
		}

		public IQueryable<F052902Data> F052902Datas
		{
			get { return new List<F052902Data>().AsQueryable(); }
		}

		public IQueryable<ConfluxBox> ConfluxBoxs
		{
			get { return new List<ConfluxBox>().AsQueryable(); }
		}

		#endregion

		public IQueryable<F055002Summary> F055002Summarys
		{
			get { return new List<F055002Summary>().AsQueryable(); }
		}

		public IQueryable<DeliveryData> DeliveryDatas
		{
			get { return new List<DeliveryData>().AsQueryable(); }
		}

		#region 出貨裝車
		public IQueryable<F050801WithF700102> F050801WithF700102s
		{
			get { return new List<F050801WithF700102>().AsQueryable(); }
		}
		#endregion

		#region 退貨檢驗
		public IQueryable<F161402Data> F161402Datas
		{
			get { return new List<F161402Data>().AsQueryable(); }
		}
		public IQueryable<F190206CheckItemName> F190206CheckItemNames
		{
			get { return new List<F190206CheckItemName>().AsQueryable(); }
		}
		public IQueryable<F16140101Data> F16140101Datas
		{
			get { return new List<F16140101Data>().AsQueryable(); }
		}

		public IQueryable<F161202SelectedData> F161202SelectedDatas
		{
			get { return new List<F161202SelectedData>().AsQueryable(); }
		}

		public IQueryable<F161501> F161501s
		{
			get { return new List<F161501>().AsQueryable(); }
		}
		#endregion

		#region P080301 同倉調撥下架

		public IQueryable<F151002Data> F151002Datas
		{
			get { return new List<F151002Data>().AsQueryable(); }
		}

		public IQueryable<F151002ItemLocData> F151002ItemLocDatas
		{
			get { return new List<F151002ItemLocData>().AsQueryable(); }
		}
		#endregion

		#region P080302 同倉調撥上架

		public IQueryable<F151002DataByTar> F151002DataByTars
		{
			get { return new List<F151002DataByTar>().AsQueryable(); }
		}

		public IQueryable<F151002ItemLocDataByTar> F151002ItemLocDataByTars
		{
			get { return new List<F151002ItemLocDataByTar>().AsQueryable(); }
		}
		#endregion

		#region P080701 託運單
		public IQueryable<F055001Data> F055001Datas
		{
			get { return new List<F055001Data>().AsQueryable(); }
		}
		#endregion

		#region P080701 託運單商品
		public IQueryable<F055002Data> F055002Datas
		{
			get { return new List<F055002Data>().AsQueryable(); }
		}
		#endregion

		#region 送貨單
		public IQueryable<DeliveryNoteData> DeliveryNoteDatas
		{
			get { return new List<DeliveryNoteData>().AsQueryable(); }
		}

		public IQueryable<DeliveryNoteSubData> DeliveryNoteSubDatas
		{
			get { return new List<DeliveryNoteSubData>().AsQueryable(); }
		}

		public IQueryable<DeliveryNoteSubDataA> DeliveryNoteSubDataAs
		{
			get { return new List<DeliveryNoteSubDataA>().AsQueryable(); }
		}

		public IQueryable<DeliveryNoteSubDataB> DeliveryNoteSubDataBs
		{
			get { return new List<DeliveryNoteSubDataB>().AsQueryable(); }
		}
		#endregion

		#region Welcome Letter
		public IQueryable<F05000201Data> F05000201Datas
		{
			get { return new List<F05000201Data>().AsQueryable(); }
		}
		#endregion

		#region 行動盤點

		public IQueryable<InventoryItemQty> InventorItemQties
		{
			get { return new List<InventoryItemQty>().AsQueryable(); }
		}

		public IQueryable<InventoryScanLoc> InventoryLocs
		{
			get { return new List<InventoryScanLoc>().AsQueryable(); }
		}

		public IQueryable<InventoryScanItem> InventoryScanItems
		{
			get { return new List<InventoryScanItem>().AsQueryable(); }
		}

		public IQueryable<InventoryLocItem> InventoryLocItems
		{
			get { return new List<InventoryLocItem>().AsQueryable(); }
		}

		#endregion

		public IQueryable<SerialNoResult> SerialNoResults
		{ get { return new List<SerialNoResult>().AsQueryable(); } }

		#region 申請書配送報表
		public IQueryable<F050801SaDataReport> F050801SaDataReports
		{
			get { return new List<F050801SaDataReport>().AsQueryable(); }
		}
		#endregion

		public IQueryable<DeliveryReport> DeliveryReports
		{
			get { return new List<DeliveryReport>().AsQueryable(); }
		}

		public IQueryable<DelvdtlInfo> DelvdtlInfos
		{
			get { return new List<DelvdtlInfo>().AsQueryable(); }
		}
		public IQueryable<ReturnDetailSummary> ReturnDetailSummarys
		{
			get { return new List<ReturnDetailSummary>().AsQueryable(); }
		}
		public IQueryable<SearchItemResult> SearchItemResults
		{
			get { return new List<SearchItemResult>().AsQueryable(); }
		}

		public IQueryable<F190905> F190905s
		{
			get { return new List<F190905>().AsQueryable(); }
		}
		public IQueryable<F050304> CheckF050304AndF050901s
		{
			get { return new List<F050304>().AsQueryable(); }
		}

		#region 銷貨憑證報表

		public IQueryable<SaleOrderReport> SaleOrderReports
		{
			get { return new List<SaleOrderReport>().AsQueryable(); }
		}

		public IQueryable<SaleOrderDetailReport> SaleOrderDetailReports
		{
			get { return new List<SaleOrderDetailReport>().AsQueryable(); }
		}
		#endregion

		#region 裝車稽核
		public IQueryable<UploadDelvCheckCode> UploadDelvCheckCodes
		{
			get { return new List<UploadDelvCheckCode>().AsQueryable(); }
		}
		#endregion

		#region 批量裝車
		public IQueryable<CarPeriodDelvNo> CarPeriodDelvNos
		{
			get { return new List<CarPeriodDelvNo>().AsQueryable(); }
		}
		public IQueryable<P080902Retail> P080902Retails
		{
			get { return new List<P080902Retail>().AsQueryable(); }
		}

		public IQueryable<P080902Ship> P080902Ships
		{
			get { return new List<P080902Ship>().AsQueryable(); }
		}

		public IQueryable<P080902ShipDetail> P080902ShipDetails
		{
			get { return new List<P080902ShipDetail>().AsQueryable(); }
		}

		public IQueryable<ShipLoadingReport> ShipLoadingReports
		{
			get { return new List<ShipLoadingReport>().AsQueryable(); }
		}
		#endregion

		public IQueryable<P081202Data> P081202Datas
		{
			get { return new List<P081202Data>().AsQueryable(); }
		}

		public IQueryable<P08120201Data> P08120201Datas
		{
			get { return new List<P08120201Data>().AsQueryable(); }
		}

		public IQueryable<AGVItemEnterData> AGVItemEnterDatas
		{
			get { return new List<AGVItemEnterData>().AsQueryable(); }
		}

		public IQueryable<F051201> F051201s
		{
			get { return new List<F051201>().AsQueryable(); }
		}

		public IQueryable<F05120101> F05120101s
		{
			get { return new List<F05120101>().AsQueryable(); }
		}

		public IQueryable<F055003Data> F055003Datas
		{
			get { return new List<F055003Data>().AsQueryable(); }
		}

		#region P0806130000單據工號綁定
		public IQueryable<F0011BindData> F0011BindDatas
		{
			get { return new List<F0011BindData>().AsQueryable(); }
		}
		#endregion

		#region P081203 AGV彙總揀貨
		public IQueryable<P081203Data> P081203Datas
		{
			get { return new List<P081203Data>().AsQueryable(); }
		}

		public IQueryable<BatchPickItemInfo> BatchPickItemInfos
		{
			get { return new List<BatchPickItemInfo>().AsQueryable(); }
		}

		public IQueryable<CallAGVBatchData> CallAGVBatchDatas
		{
			get { return new List<CallAGVBatchData>().AsQueryable(); }
		}

		#endregion

		#region P081204 AGV盤點揀貨
		public IQueryable<P081204Data> P081204Datas
		{
			get { return new List<P081204Data>().AsQueryable(); }
		}

		public IQueryable<P08120401Data> P08120401Datas
		{
			get { return new List<P08120401Data>().AsQueryable(); }
		}
		#endregion

		public IQueryable<P08120502Data> P08120502Datas
		{
			get { return new List<P08120502Data>().AsQueryable(); }
		}

		public IQueryable<P08120503Data> P08120503Datas
		{
			get { return new List<P08120503Data>().AsQueryable(); }
		}
		public IQueryable<P081301StockSumQty> P081301StockSumQties
		{
			get { return new List<P081301StockSumQty>().AsQueryable(); }
		}
		public IQueryable<P08130101MoveLoc> P08130101MoveLocs
		{
			get { return new List<P08130101MoveLoc>().AsQueryable(); }
		}
		public IQueryable<P08130101Stock> P08130101Stocks
		{
			get { return new List<P08130101Stock>().AsQueryable(); }
		}
		public IQueryable<P080901ShipReport> P080901ShipReports
		{
			get { return new List<P080901ShipReport>().AsQueryable(); }
		}
		public IQueryable<P08120503ShelfItem> P08120503ShelfItems
		{
			get { return new List<P08120503ShelfItem>().AsQueryable(); }
		}
        public IQueryable<PcHomeDeliveryReport> PcHomeDeliveryReports
        {
            get { return new List<PcHomeDeliveryReport>().AsQueryable(); }
        }

		public IQueryable<HomeDeliveryOrderNumberData> HomeDeliveryOrderNumberDatas
		{
			get { return new List<HomeDeliveryOrderNumberData>().AsQueryable(); }
		}

		public IQueryable<LittleWhiteReport> LittleWhiteReports
		{
			get { return new List<LittleWhiteReport>().AsQueryable(); }
		}

		#region 稽核出庫-箱內明細
		public IQueryable<P0808040100_BoxData> P0808040100_BoxDatas
		{
			get { return new List<P0808040100_BoxData>().AsQueryable(); }
		}

		public IQueryable<P0808040100_BoxDetailData> P0808040100_BoxDetailDatas
		{
			get { return new List<P0808040100_BoxDetailData>().AsQueryable(); }
		}

		public IQueryable<P0808040100_PrintData> P0808040100_PrintDatas
		{
			get { return new List<P0808040100_PrintData>().AsQueryable(); }
		}
		#endregion

		public IQueryable<BatchPickData> BatchPickDatas
		{
			get { return new List<BatchPickData>().AsQueryable(); }
		}

        #region 單人包裝站
        public IQueryable<SearchWmsOrderPackingDetailRes> SearchWmsOrderPackingDetailResa
        {
            get { return new List<SearchWmsOrderPackingDetailRes>().AsQueryable(); }
        }

        public IQueryable<SearchWmsOrderScanLogRes> SearchWmsOrderScanLogRess
        {
            get { return new List<SearchWmsOrderScanLogRes>().AsQueryable(); }
        }
        #endregion
    }
}
