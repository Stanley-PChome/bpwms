using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P02.ExDataSources
{
    public partial class P02ExDataSource
    {
        public IQueryable<ExecuteResult> ExecuteResults
        {
            get { return new List<ExecuteResult>().AsQueryable(); }
        }

        #region Vendor (廠商)共用

        public IQueryable<VendorInfo> VendorInfos
        {
            get { return new List<VendorInfo>().AsQueryable(); }
        }

        #endregion

        #region P020101 進倉預排

        public IQueryable<F020103Detail> F020103Details
        {
            get { return new List<F020103Detail>().AsQueryable(); }
        }

        #endregion

        #region P020104 碼頭期間設定

        public IQueryable<F020104Detail> F020104Details
        {
            get { return new List<F020104Detail>().AsQueryable(); }
        }

        #endregion

        #region P020201 廠商報到

        public IQueryable<P020201ReportData> P020201ReportDatas
        {
            get { return new List<P020201ReportData>().AsQueryable(); }
        }

        #endregion

        #region P020203 商品檢驗

        public IQueryable<P020203Data> P020203Datas
        {
            get { return new List<P020203Data>().AsQueryable(); }
        }

        public IQueryable<F190206CheckName> F190206CheckNames
        {
            get { return new List<F190206CheckName>().AsQueryable(); }
        }

        public IQueryable<SerialData> SerialDatas
        {
            get { return new List<SerialData>().AsQueryable(); }
        }

        public IQueryable<SerialDataEx> SerialDataExs
        {
            get { return new List<SerialDataEx>().AsQueryable(); }
        }

        public IQueryable<AcceptancePurchaseReport> AcceptancePurchaseReports
        {
            get { return new List<AcceptancePurchaseReport>().AsQueryable(); }
        }

        public IQueryable<AcceptanceReturnData> AcceptanceReturnDatas
        {
            get { return new List<AcceptanceReturnData>().AsQueryable(); }
        }

        public IQueryable<FileUploadData> FileUploadDatas
        {
            get { return new List<FileUploadData>().AsQueryable(); }
        }

        public IQueryable<F051201ReportDataA> F051201ReportDataAs
        {
            get { return new List<F051201ReportDataA>().AsQueryable(); }
        }
        public IQueryable<F151001ReportByAcceptance> F151001ReportByAcceptances
        {
            get { return new List<F151001ReportByAcceptance>().AsQueryable(); }
        }
		public IQueryable<DefectDetailReport> DefectDetialReport
		{
			get { return new List<DefectDetailReport>().AsQueryable(); }
		}
		#endregion

		    #region P020205 進倉序號檔查詢
		public IQueryable<P020205Main> P020205Mains
        {
            get { return new List<P020205Main>().AsQueryable(); }
        }
        public IQueryable<P020205Detail> P020205Details
        {
            get { return new List<P020205Detail>().AsQueryable(); }
        }
        #endregion

        #region P020301 調入上架

        public IQueryable<F1510Data> F1510Datas
        {
            get { return new List<F1510Data>().AsQueryable(); }
        }
        public IQueryable<F1510BundleSerialLocData> F1510BundleSerialLocDatas
        {
            get { return new List<F1510BundleSerialLocData>().AsQueryable(); }
        }

        public IQueryable<F2501ItemData> F2501ItemDatas
        {
            get { return new List<F2501ItemData>().AsQueryable(); }
        }

        public IQueryable<F1510ItemLocData> F1510ItemLocDatas
        {
            get { return new List<F1510ItemLocData>().AsQueryable(); }
        }

        public IQueryable<AllocationBundleSerialLocCount> AllocationBundleSerialLocCounts
        {
            get { return new List<AllocationBundleSerialLocCount>().AsQueryable(); }
        }

        public IQueryable<F15100101Data> F15100101Datas
        {
            get { return new List<F15100101Data>().AsQueryable(); }
        }

        public IQueryable<ImportBundleSerialLoc> ImportBundleSerialLoc
        {
            get { return new List<ImportBundleSerialLoc>().AsQueryable(); }
        }
        #endregion

        #region P020302 調入查詢

        public IQueryable<F1510QueryData> F1510QueryDatas
        {
            get { return new List<F1510QueryData>().AsQueryable(); }
        }

        #endregion


        #region F020302Data  進倉檔

        public IQueryable<F020302Data> F020302Datas
        {
            get { return new List<F020302Data>().AsQueryable(); }
        }

        #endregion


        public IQueryable<F151001WithF02020107> F151001WithF02020107s
        {
            get { return new List<F151001WithF02020107>().AsQueryable(); }
        }

        public IQueryable<F020201WithF02020101> F020201WithF02020101s
        {
            get { return new List<F020201WithF02020101>().AsQueryable(); }
        }

        public IQueryable<F02020109Data> F02020109Datas
        {
            get { return new List<F02020109Data>().AsQueryable(); }
        }

        public IQueryable<F0202Data> F0202Datas
        {
            get { return new List<F0202Data>().AsQueryable(); }
        }

        public IQueryable<VW_F010301> VW_F010301s
        { get { return new List<VW_F010301>().AsQueryable(); } }

        public IQueryable<ScanCargoData> ScanCargoDatas
        {
            get { return new List<ScanCargoData>().AsQueryable(); }
        }

        public IQueryable<ScanCargoStatistic> ScanCargoStatistic
        { get { return new List<ScanCargoStatistic>().AsQueryable(); } }

        public IQueryable<ReceiptUnCheckData> ReceiptUnCheckDatas
        { get { return new List<ReceiptUnCheckData>().AsQueryable(); } }

        public IQueryable<ScanReceiptData> ScanReceiptDatas
        { get { return new List<ScanReceiptData>().AsQueryable(); } }

        public IQueryable<ItemBindContainerData> ItemBindContainerDatas
        { get { return new List<ItemBindContainerData>().AsQueryable(); } }
    

		public IQueryable<ContainerDetailData> ContainerDetailDatas
		{
			get { return new List<ContainerDetailData>().AsQueryable(); }
		}

        public IQueryable<BindContainerData> BindContainerDatas
        { get { return new List<BindContainerData>().AsQueryable(); } }

        public IQueryable<AreaContainerData> AreaContainerDatas
        { get { return new List<AreaContainerData>().AsQueryable(); } }


    
		#region 商品檢驗與容器綁定

		/// <summary>
		/// 驗收單與上架容器查詢
		/// </summary>
		public IQueryable<P020206Data> P020206Datas
		{
			get { return new List<P020206Data>().AsQueryable(); }
		}

		public IQueryable<AcceptanceDetail> AcceptanceDetails
		{
			get { return new List<AcceptanceDetail>().AsQueryable(); }
		}

		public IQueryable<AcceptanceContainerDetail> AcceptanceContainerDetails
		{
			get { return new List<AcceptanceContainerDetail>().AsQueryable(); }
		}

		public IQueryable<DefectDetail> DefectDetails
		{
			get { return new List<DefectDetail>().AsQueryable(); }
		}
    #endregion

    #region 複驗異常處理
    public IQueryable<F020504ExData> F020504ExDatas
    { get { return new List<F020504ExData>().AsQueryable(); } }

    public IQueryable<UnnormalItemRecheckLog> UnnormalItemRecheckLogs
    { get { return new List<UnnormalItemRecheckLog>().AsQueryable(); } }

    public IQueryable<ContainerRecheckFaildItem> ContainerRecheckFaildItems
    { get { return new List<ContainerRecheckFaildItem>().AsQueryable(); } }
    #endregion 複驗異常處理
  }
}
