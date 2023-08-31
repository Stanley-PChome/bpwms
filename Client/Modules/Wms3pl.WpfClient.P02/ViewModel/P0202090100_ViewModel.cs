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
  public class P0202090100_ViewModel : InputViewModelBase
  {
    private string _gupCode;
    private string _custCode;
    public Action RePrintItemLabel = delegate { };
    public Action RePrintRecvNote = delegate { };

    public P0202090100_ViewModel()
    {
      SetGlobalInfo();
    }

    private void SetGlobalInfo()
    {
      _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
      _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
    }

    #region Property
    public F910501 SelectedF910501 { get; set; }

    private string _selectedDc;

    public string SelectedDc
    {
      get { return _selectedDc; }
      set
      {
        Set(() => SelectedDc, ref _selectedDc, value);
        RaisePropertyChanged();
      }
    }

    private bool _IsReprintIdLabel;
    public bool IsReprintIdLabel
    {
      get { return _IsReprintIdLabel; }
      set
      {
        Set(() => IsReprintIdLabel, ref _IsReprintIdLabel, value);
        RaisePropertyChanged();
      }
    }

    private bool _IsReprintRecvNote;
    public bool IsReprintRecvNote
    {
      get { return _IsReprintRecvNote; }
      set
      {
        Set(() => IsReprintRecvNote, ref _IsReprintRecvNote, value);
        RaisePropertyChanged();
      }
    }

    private SelectionItem<RecvRecords> _SelectRecords;

    public SelectionItem<RecvRecords> SelectRecords
    {
      get { return _SelectRecords; }
      set { Set(() => SelectRecords, ref _SelectRecords, value); }
    }

    private string _txtCpies;

    public string txtCpies
    {
      get { return _txtCpies; }
      set { Set(() => txtCpies, ref _txtCpies, value); }
    }

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
        _itemLabelData.ForEach(o => o.CUST_ITEM_CODE_BAR = BarcodeConverter128.StringToBarcode(o.CUST_ITEM_CODE));
        return _itemLabelData.ToDataTable("rild");
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

    #endregion Property

    #region Command
    public ICommand RePrint
    {
      get
      {
        return CreateBusyAsyncCommand(
        o =>
        {
          if (IsReprintIdLabel)
          {
            var success = int.TryParse(txtCpies, out int copiesNum);

            if (string.IsNullOrWhiteSpace(txtCpies) || !success || copiesNum <= 0)
            {
              ShowInfoMessage("請輸入列印張數");
              return;
            }

            ItemLabelData = GetItemLabelData(SelectRecords.Item.RT_NO + SelectRecords.Item.RT_SEQ);
            RePrintItemLabel();
          }
          else if (IsReprintRecvNote)
          {
            var proxyEntities = GetProxy<F02Entities>();
            var porxyEx = GetExProxy<P02ExDataSource>();

            AcceptanceReportData = ReportService.GetAcceptancePurchaseReport(SelectedDc, _gupCode, _custCode, SelectRecords.Item.PURCHASE_NO, SelectRecords.Item.RT_NO, FunctionCode, "0", "0");
            AcceptanceReportData1 = ReportService.GetAcceptancePurchaseReport(SelectedDc, _gupCode, _custCode, SelectRecords.Item.PURCHASE_NO, SelectRecords.Item.RT_NO, FunctionCode, "1", "0");

            // 調撥單貼紙資料
            List<string> defectWarehouse = proxyEntities.F02020109s.Where(x => x.DC_CODE == SelectedDc &&
            x.GUP_CODE == this._gupCode &&
            x.CUST_CODE == this._custCode &&
            x.STOCK_NO == SelectRecords.Item.PURCHASE_NO).ToList().Select(x => x.WAREHOUSE_ID).ToList(); //不良品的倉別編號

            var f151001ReportByAcceptance = porxyEx.CreateQuery<F151001ReportByAcceptance>("GetF151001ReportByAcceptance")
              .AddQueryExOption("dcCode", SelectedDc)
              .AddQueryExOption("gupCode", _gupCode)
              .AddQueryExOption("custCode", _custCode)
              .AddQueryExOption("purchaseNo", SelectRecords.Item.PURCHASE_NO)
              .AddQueryExOption("rtNo", SelectRecords.Item.RT_NO).ToList();

            F151001ReportByAcceptance = f151001ReportByAcceptance.Where(x => !defectWarehouse.Contains(x.WAREHOUSE_ID)).ToList();
            F151001ReportByAcceptanceDefect = f151001ReportByAcceptance.Except(F151001ReportByAcceptance).ToList();

            // 不良品明細資料
            DefectDetailReport = GetExProxy<P02ExDataSource>().CreateQuery<DefectDetailReport>("GetDefectDetailReportData")
              .AddQueryExOption("dcCode", SelectedDc)
              .AddQueryExOption("gupCode", _gupCode)
              .AddQueryExOption("custCode", _custCode)
              .AddQueryExOption("rtNo", SelectRecords.Item.RT_NO).ToList();

            SelectedPurchaseNo = SelectRecords.Item.PURCHASE_NO;

            RePrintRecvNote();
          }
        });
      }
    }

    private List<ItemLabelData> GetItemLabelData(string rtNos)
    {
      var proxy = GetExProxy<P02ExDataSource>();

      var labelData = proxy.CreateQuery<ItemLabelData>("GetP020209ItemLabelData")
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
    #endregion Command
  }
}
