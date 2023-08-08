using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
  public class P0202090000_ViewModel : InputViewModelBase
  {
    private string _gupCode;
    private string _custCode;

    public Action PrintItemLabel = delegate { };
    public Action PrintRecvNote = delegate { };

    /// <summary>
		/// Device 的設定 (當物流中心改變時，就會去顯示設定 Device 的畫面)  
		/// </summary>
		public F910501 SelectedF910501 { get; set; }

    public P0202090000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        SetGlobalInfo();
        PrintStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F020201", "PRINT_MODE");
      }
    }

    private void SetGlobalInfo()
    {
      DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
      _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
      _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

      if (DcList.Any())
        SelectedDc = DcList.First().Value;
    }

    #region Properties

    #region 物流中心
    private List<NameValuePair<string>> _dcList;

    public List<NameValuePair<string>> DcList
    {
      get { return _dcList; }
      set { Set(() => DcList, ref _dcList, value); }
    }

    private string _selectedDc;

    public string SelectedDc
    {
      get { return _selectedDc; }
      set {
        Set(() => SelectedDc, ref _selectedDc, value);
        RecvRecords = null;
        SelectRecords = null;
        RaisePropertyChanged();
      }
    }

    #endregion 物流中心

    #region 驗收日期
    private DateTime? _RecvDateBegin = DateTime.Now.Date;

    public DateTime? RecvDateBegin
    {
      get { return _RecvDateBegin; }
      set { Set(() => RecvDateBegin, ref _RecvDateBegin, value); }
    }

    private DateTime? _RecvDateEnd = DateTime.Now.Date;

    public DateTime? RecvDateEnd
    {
      get { return _RecvDateEnd; }
      set { Set(() => RecvDateEnd, ref _RecvDateEnd, value); }
    }
    #endregion 驗收日期

    #region 進倉單號
    private string _PurchaseNo;

    public string PurchaseNo
    {
      get { return _PurchaseNo; }
      set { Set(() => PurchaseNo, ref _PurchaseNo, value); }
    }
    #endregion 進倉單號

    #region 貨主單號
    private string _CustOrdNo;

    public string CustOrdNo
    {
      get { return _CustOrdNo; }
      set { Set(() => CustOrdNo, ref _CustOrdNo, value); }
    }
    #endregion 貨主單號

    #region 列印狀態
    private List<NameValuePair<string>> _PrintStatusList;

    public List<NameValuePair<string>> PrintStatusList
    {
      get { return _PrintStatusList; }
      set {
        Set(() => PrintStatusList, ref _PrintStatusList, value);

        if (value != null && value.Any())
        {
          value.RemoveAt(0);
          SelectedPrintStatus = value.FirstOrDefault().Value;
        }
      }
    }

    private string _SelectedPrintStatus;

    public string SelectedPrintStatus
    {
      get { return _SelectedPrintStatus; }
      set { Set(() => SelectedPrintStatus, ref _SelectedPrintStatus, value); }
    }
    #endregion 列印狀態

    #region 棧板容器
    private string _PalletLocation;

    public string PalletLocation
    {
      get { return _PalletLocation; }
      set { Set(() => PalletLocation, ref _PalletLocation, value); }
    }
    #endregion 棧板容器

    #region 品號
    private string _ItemCode;

    public string ItemCode
    {
      get { return _ItemCode; }
      set { Set(() => ItemCode, ref _ItemCode, value); }
    }
    #endregion 品號

    #region 驗收人員
    private string _RecvStaff;

    public string RecvStaff
    {
      get { return _RecvStaff; }
      set { Set(() => RecvStaff, ref _RecvStaff, value); }
    }
    #endregion 驗收人員

    #region 驗收檔查詢結果
    private SelectionList<RecvRecords> _RecvRecords;

    public SelectionList<RecvRecords> RecvRecords
    {
      get { return _RecvRecords; }
      set { Set(() => RecvRecords, ref _RecvRecords, value); }
    }

    private SelectionItem<RecvRecords> _SelectRecords;

    public SelectionItem<RecvRecords> SelectRecords
    {
      get { return _SelectRecords; }
      set { Set(() => SelectRecords, ref _SelectRecords, value); }
    }
    #endregion 驗收檔查詢結果

    #region 全選/取消全選
    private bool _IsCheckAll;
    public bool IsCheckAll
    {
      get { return _IsCheckAll; }
      set
      {
        Set(() => IsCheckAll, ref _IsCheckAll, value);

        if (RecvRecords.Any(o => o.Item.IsEnabled))
        {
          foreach (var item in RecvRecords)
            item.IsSelected = value;
        }
      }
    }
    #endregion 全選/取消全選

    #region 商品標籤
    private List<ItemLabelData> _itemLabelData = new List<ItemLabelData>();

    public List<ItemLabelData> ItemLabelData
    {
      get { return _itemLabelData; }
      set { _itemLabelData = value; }
    }

    public DataTable ItemLabelDataTable
    {
      get
      {
        _itemLabelData.ForEach(o => {
          if (!string.IsNullOrWhiteSpace(o.CUST_ITEM_CODE))
            o.CUST_ITEM_CODE_BAR = BarcodeConverter128.StringToBarcode(o.CUST_ITEM_CODE);
        });
        return _itemLabelData.ToDataTable("ild");
      }
    }
    #endregion 商品標籤

    #region 驗收單報表
    //良品
    private List<AcceptancePurchaseReport> _acceptanceReportData = new List<AcceptancePurchaseReport>();
    public List<AcceptancePurchaseReport> AcceptanceReportData
    {
      get { return _acceptanceReportData; }
      set { _acceptanceReportData = value; }
    }
    public DataTable AcceptanceReportDataTable
    {
      get
      {
        _acceptanceReportData.ForEach(x => x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE));
        _acceptanceReportData.ForEach(x => x.OrdNoBarcode = BarcodeConverter128.StringToBarcode(x.ORDER_NO));
        return _acceptanceReportData.ToDataTable("ado");
      }
    }

    //不良品
    private List<AcceptancePurchaseReport> _acceptanceReportData1 = new List<AcceptancePurchaseReport>();
    public List<AcceptancePurchaseReport> AcceptanceReportData1
    {
      get { return _acceptanceReportData1; }
      set { _acceptanceReportData1 = value; }
    }
    public DataTable AcceptanceReportDataTable1
    {
      get
      {
        _acceptanceReportData1.ForEach(x => x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE));
        _acceptanceReportData1.ForEach(x => x.OrdNoBarcode = BarcodeConverter128.StringToBarcode(x.ORDER_NO));
        return _acceptanceReportData1.ToDataTable("ado");
      }
    }

    private List<F151001ReportByAcceptance> _f151001ReportByAcceptance;
    public List<F151001ReportByAcceptance> F151001ReportByAcceptance
    {
      get { return _f151001ReportByAcceptance; }
      set
      {
        _f151001ReportByAcceptance = value;
        RaisePropertyChanged("F151001ReportByAcceptance");
      }
    }
    public DataTable F151001ReportByAcceptanceDataTable
    {
      get
      {
        F151001ReportByAcceptance.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));
        return F151001ReportByAcceptance.ToDataTable("ado");
      }
    }

    private List<F151001ReportByAcceptance> _f151001ReportByAcceptanceDefect;
    public List<F151001ReportByAcceptance> F151001ReportByAcceptanceDefect
    {
      get { return _f151001ReportByAcceptanceDefect; }
      set
      {
        _f151001ReportByAcceptanceDefect = value;
        RaisePropertyChanged("F151001ReportByAcceptanceDefect");
      }
    }
    public DataTable F151001ReportByAcceptanceDefectDataTable
    {
      get
      {
        _f151001ReportByAcceptanceDefect.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));
        return _f151001ReportByAcceptanceDefect.ToDataTable("ado");
      }
    }
    #endregion 驗收單報表

    #region 不良品明細(報表)
    private List<DefectDetailReport> _defectDetailReport;
    public List<DefectDetailReport> DefectDetailReport
    {
      get { return _defectDetailReport; }
      set
      {
        _defectDetailReport = value;
        RaisePropertyChanged("DefectDetailReport");
      }
    }
    public DataTable DefectDetailReportDataTable
    {
      get
      {
        return _defectDetailReport.ToDataTable("ado");
      }
    }
    #endregion

    #region Form - 進倉單號
    private string _selectedPurchaseNo;

    public string SelectedPurchaseNo
    {
      get { return _selectedPurchaseNo; }
      set
      {
        Set(() => SelectedPurchaseNo, ref _selectedPurchaseNo, value);
      }
    }
    #endregion

    #endregion Properties


    #region Command

    #region 查詢

    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
        o =>
        {
          DoSearch();
        });
      }
    }

    private void DoSearch()
    {
      var proxy = GetExProxy<P02ExDataSource>();

      RecvRecords = proxy.CreateQuery<RecvRecords>("GetF020209RecvRecord")
                       .AddQueryExOption("dcCode", SelectedDc)
                       .AddQueryExOption("gupCode", _gupCode)
                       .AddQueryExOption("custCode", _custCode)
                       .AddQueryExOption("RecvDateBegin", RecvDateBegin)
                       .AddQueryExOption("RecvDateEnd", RecvDateEnd)
                       .AddQueryExOption("PurchaseNo", PurchaseNo)
                       .AddQueryExOption("CustOrdNo", CustOrdNo)
                       .AddQueryExOption("PrintMode", SelectedPrintStatus)
                       .AddQueryExOption("PalletLocation", PalletLocation)
                       .AddQueryExOption("ItemCode", ItemCode)
                       .AddQueryExOption("RecvStaff", RecvStaff)
                       .ToSelectionList();

      if (!RecvRecords.Any())
      {
        ShowInfoMessage("查無資料");
        return;
      }

      SelectRecords = RecvRecords.FirstOrDefault();
      ItemLabelData = null;
    }

    #endregion 查詢

    #region 列印

    public ICommand PrintCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
        o =>
        {
          var selectedData = RecvRecords.Where(x => x.IsSelected);
          var printItemLabel = selectedData.Where(x => x.Item.IS_PRINT_ITEM_ID_RAW == "1").Select(x => x.Item.RT_NO);
          var printRecvNote = selectedData.Where(x => x.Item.IS_PRINT_RECVNOTE_RAW == "1");

          if (printItemLabel.Any())
          {
            var toProcessData = GetItemLabelData(string.Join(",", printItemLabel));

            foreach (var data in toProcessData)
            {
              ItemLabelData = new List<ItemLabelData> { data };
              PrintItemLabel();
            }
          }

          if (printRecvNote.Any())
          {
            foreach (var recvNote in printRecvNote)
            {
              var proxyEntities = GetProxy<F02Entities>();
              var porxyEx = GetExProxy<P02ExDataSource>();

              AcceptanceReportData = ReportService.GetAcceptancePurchaseReport(SelectedDc, _gupCode, _custCode, recvNote.Item.PURCHASE_NO, recvNote.Item.RT_NO, FunctionCode, "0", "0");
              AcceptanceReportData1 = ReportService.GetAcceptancePurchaseReport(SelectedDc, _gupCode, _custCode, recvNote.Item.PURCHASE_NO, recvNote.Item.RT_NO, FunctionCode, "1", "0");

              // 調撥單貼紙資料
              List<string> defectWarehouse = proxyEntities.F02020109s.Where(x => x.DC_CODE == SelectedDc &&
              x.GUP_CODE == this._gupCode &&
              x.CUST_CODE == this._custCode &&
              x.STOCK_NO == recvNote.Item.PURCHASE_NO && x.RT_NO == recvNote.Item.RT_NO).ToList().Select(x => x.WAREHOUSE_ID).Distinct().ToList(); //不良品的倉別編號

              var f151001ReportByAcceptance = porxyEx.CreateQuery<F151001ReportByAcceptance>("GetF151001ReportByAcceptance")
                .AddQueryExOption("dcCode", SelectedDc)
                .AddQueryExOption("gupCode", _gupCode)
                .AddQueryExOption("custCode", _custCode)
                .AddQueryExOption("purchaseNo", recvNote.Item.PURCHASE_NO)
                .AddQueryExOption("rtNo", recvNote.Item.RT_NO).ToList();

              F151001ReportByAcceptance = f151001ReportByAcceptance.Where(x => !defectWarehouse.Contains(x.WAREHOUSE_ID)).ToList();
              F151001ReportByAcceptanceDefect = f151001ReportByAcceptance.Except(F151001ReportByAcceptance).ToList();

              // 不良品明細資料
              DefectDetailReport = GetExProxy<P02ExDataSource>().CreateQuery<DefectDetailReport>("GetDefectDetailReportData")
                .AddQueryExOption("dcCode", SelectedDc)
                .AddQueryExOption("gupCode", _gupCode)
                .AddQueryExOption("custCode", _custCode)
                .AddQueryExOption("rtNo", recvNote.Item.RT_NO).ToList();

              SelectedPurchaseNo = recvNote.Item.PURCHASE_NO;

              PrintRecvNote();
            }
          }
        },
        () => RecvRecords != null && RecvRecords.Any(x => x.IsSelected),
        o =>
        {
          var processNo = RecvRecords.Where(x => x.IsSelected).Select(x => x.Item.RT_NO);
          UpdatePrintRecvNote(processNo.ToList());
          DoSearch();
        }
        );
      }
    }

    private void UpdatePrintRecvNote(List<string> rtNoList)
    {
      var wcfproxy = GetWcfProxy<wcf.P02WcfServiceClient>();
      var result = wcfproxy.RunWcfMethod(w => w.UpdateRecvNotePrintInfo(SelectedDc, this._gupCode, this._custCode, rtNoList.ToArray()));
      if (!result.IsSuccessed)
        ShowWarningMessage(result.Message);
    }

    private List<ItemLabelData> GetItemLabelData(string rtNos)
    {
      var proxy = GetExProxy<P02ExDataSource>();

      var labelData = proxy.CreateQuery<ItemLabelData>("GetF020209ItemLabelData")
                           .AddQueryExOption("dcCode", SelectedDc)
                           .AddQueryExOption("gupCode", _gupCode)
                           .AddQueryExOption("custCode", _custCode)
                           .AddQueryExOption("rtNos", rtNos)
                           .ToList();

      if (!labelData.Any())
      {
        ShowWarningMessage("查無列印資料");
        return null;
      }

      return labelData;
    }

    #endregion 列印

    #region 補印

    public ICommand RePrintCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => {

          },
          () => SelectRecords != null && SelectRecords.Item.IsEnabledString == "2");
      }
    }

    public void DoRePrint(RecvRecords _selectedRecord, string reportType, string Copies)
    {
      if (reportType == "IDLabel")
      {
        ItemLabelData = GetItemLabelData(_selectedRecord.RT_NO);
        PrintItemLabel();
      }
    }

    #endregion 補印

    #endregion Command

  }
}
