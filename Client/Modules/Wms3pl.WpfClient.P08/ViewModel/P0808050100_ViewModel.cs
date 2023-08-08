using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using wcf=Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.ViewModel
{
  public class P0808050100_ViewModel : InputViewModelBase
  {
    #region Property

    private string _gupCode;
    private string _custCode;
    public string srcView;

    public Action DoContainerFocus = delegate { };


    #region 物流中心清單
    private List<NameValuePair<string>> _dcList;

    public List<NameValuePair<string>> DcList
    {
      get { return _dcList; }
      set
      {
        Set(() => DcList, ref _dcList, value);
      }
    }
    #endregion

    #region 選取物流中心編號
    private string _selectedDc;

    public string SelectedDc
    {
      get { return _selectedDc; }
      set
      {
        Set(() => SelectedDc, ref _selectedDc, value);
        Results = null;
        BoxDetailList = null;
        RaisePropertyChanged();
      }
    }
    #endregion

    #region 跨庫箱號狀態
    private List<NameValuePair<string>> _containerStatusList;

    public List<NameValuePair<string>> ContainerStatusList
    {
      get { return _containerStatusList; }
      set
      {
        Set(() => ContainerStatusList, ref _containerStatusList, value);
        if (value != null && value.Any())
          SelectContainerStatus = value.FirstOrDefault().Value;
      }
    }
    #endregion

    #region 選取跨庫箱號狀態

    private string _selectContainerStatus;

    public string SelectContainerStatus
    {
      get { return _selectContainerStatus; }
      set
      {
        Set(() => SelectContainerStatus, ref _selectContainerStatus, value);
      }
    }
    #endregion

    #region 容器類型
    private List<NameValuePair<string>> _containerSowTypeList;

    public List<NameValuePair<string>> ContainerSowTypeList
    {
      get { return _containerSowTypeList; }
      set
      {
        Set(() => ContainerSowTypeList, ref _containerSowTypeList, value);
        if (value != null && value.Any())
          SelectContainerSowType = value.FirstOrDefault().Value;
      }
    }
    #endregion

    #region 選取容器類型

    private string _selectContainerSowType;

    public string SelectContainerSowType
    {
      get { return _selectContainerSowType; }
      set
      {
        Set(() => SelectContainerSowType, ref _selectContainerSowType, value);
      }
    }
    #endregion


    #region 跨庫箱號
    private string _outContainerCode;
    /// <summary>
    /// 跨庫箱號
    /// </summary>
		public string OutContainerCode
    {
      get { return _outContainerCode; }
      set
      {
        value = value.Replace(" ", string.Empty);

        Set(() => OutContainerCode, ref _outContainerCode, value);
      }
    }
    #endregion

    #region 建立日期-起日
    private DateTime? _startDate = DateTime.Today;
    /// <summary>
    /// 建立日期-起日
    /// </summary>
    public DateTime? StartDate
    {
      get { return _startDate; }
      set { Set(() => StartDate, ref _startDate, value); }
    }
    #endregion

    #region 建立日期-迄日
    private DateTime? _endDate = DateTime.Today;
    /// <summary>
    /// 建立日期-迄日
    /// </summary>
    public DateTime? EndDate
    {
      get { return _endDate; }
      set { Set(() => EndDate, ref _endDate, value); }
    }
    #endregion

    #region 查詢容器資料清單
    private ObservableCollection<F0532Ex> _results;
    /// <summary>
    /// 查詢容器資料清單
    /// </summary>
    public ObservableCollection<F0532Ex> Results
    {
      get { return _results; }
      set
      {
        Set(() => Results, ref _results, value);
      }
    }

    private F0532Ex _selectData;
    public F0532Ex SelectData
    {
      get { return _selectData; }
      set
      {
        Set(() => SelectData, ref _selectData, value);

        if (SelectData != null)
        {
          var exProxy = GetExProxy<P08ExDataSource>();
          BoxDetailList = exProxy.CreateQuery<F053202Ex>("GetF053202Ex")
                         .AddQueryExOption("F0531ID", SelectData.F0531_ID.ToString())
                         .ToObservableCollection();

          CurrentOutContainerCode = SelectData.OUT_CONTAINER_CODE;
          CurrentContainerStatus = SelectData.STATUS_NAME;
          CurrentMoveOutTarget = SelectData.MOVE_OUT_TARGET_NAME;
          CurrentTotalPcs = SelectData.TOTAL_PCS;
        }
      }
    }
    #endregion

    #region 查詢容器明細資料清單
    private ObservableCollection<F053202Ex> _boxDetailList;
    /// <summary>
    /// 查詢容器明細資料清單
    /// </summary>
    public ObservableCollection<F053202Ex> BoxDetailList
    {
      get { return _boxDetailList; }
      set
      {
        Set(() => BoxDetailList, ref _boxDetailList, value);
      }
    }

    //private F0532Ex _selectData;
    //public F0532Ex SelectData
    //{
    //  get { return _selectData; }
    //  set
    //  {
    //    Set(() => SelectData, ref _selectData, value);

    //    var exProxy = GetExProxy<P08ExDataSource>();
    //    Results = exProxy.CreateQuery<F053202Ex>("GetF053202Ex")
    //                   .AddQueryExOption("F0531_ID", SelectData.F0531_ID)
    //                   .ToObservableCollection();

    //  }
    //}

    #endregion

    #region 跨庫箱號(清單顯示)
    private string _currentOutContainerCode;
    /// <summary>
    /// 跨庫箱號(清單顯示)
    /// </summary>
		public string CurrentOutContainerCode
    {
      get { return _currentOutContainerCode; }
      set
      {
        Set(() => CurrentOutContainerCode, ref _currentOutContainerCode, value);
      }
    }
    #endregion

    #region 跨庫箱號狀態(清單顯示)
    private string _currentContainerStatus;
    /// <summary>
    /// 跨庫箱號狀態(清單顯示)
    /// </summary>
		public string CurrentContainerStatus
    {
      get { return _currentContainerStatus; }
      set
      {
        Set(() => CurrentContainerStatus, ref _currentContainerStatus, value);
      }
    }
    #endregion

    #region 目的地(清單顯示)
    private string _currentMoveOutTarget;
    /// <summary>
    /// 目的地(清單顯示)
    /// </summary>
		public string CurrentMoveOutTarget
    {
      get { return _currentMoveOutTarget; }
      set
      {
        Set(() => CurrentMoveOutTarget, ref _currentMoveOutTarget, value);
      }
    }
    #endregion

    #region 總PCS數(清單顯示)
    private int _currentTotalPcs;
    /// <summary>
    /// 總PCS數(清單顯示)
    /// </summary>
		public int CurrentTotalPcs
    {
      get { return _currentTotalPcs; }
      set
      {
        Set(() => CurrentTotalPcs, ref _currentTotalPcs, value);
      }
    }
    #endregion

    #endregion

    public P0808050100_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
        DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
        if (DcList.Any())
          SelectedDc = DcList.First().Value;
        _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
        ContainerStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F0532", "STATUS", true);
        ContainerSowTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F0532", "SOW_TYPE", true);
        Init();
      }

    }

    public void Init()
    {
      UserOperateMode = OperateMode.Query;
      DispatcherAction(() =>
      {
        DoContainerFocus();
      });
    }

    #region Search 查詢
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSearch(), () => UserOperateMode == OperateMode.Query,
          o =>
          {
            if(Results != null && Results.Any())
            {
              SelectData = Results.First();
            }
          }
          );
      }
    }

    private void DoSearch()
    {
      if (!StartDate.HasValue && !EndDate.HasValue)
      {
        ShowInfoMessage("請輸入建立日期起迄");
        return;
      }
      else if (!StartDate.HasValue && EndDate.HasValue)
      {
        ShowInfoMessage("請選擇建立日期起");
        return;
      }
      else if (StartDate.HasValue && !EndDate.HasValue)
      {
        ShowInfoMessage("請選擇建立日期迄");
        return;
      }
      else if ((StartDate.Value.AddMonths(2) - EndDate.Value).TotalDays < 0)
      {
        ShowInfoMessage("建立日期範圍不可超過三個月");
        return;
      }

      var exProxy = GetExProxy<P08ExDataSource>();
      Results = exProxy.CreateQuery<F0532Ex>("GetF0532Ex")
                     .AddQueryExOption("dcCode", SelectedDc)
                     .AddQueryExOption("gupCode", _gupCode)
                     .AddQueryExOption("custCode", _custCode)
                     .AddQueryExOption("startDate", StartDate.Value)
                     .AddQueryExOption("endDate", EndDate.Value)
                     .AddQueryExOption("status", SelectContainerStatus)
                     .AddQueryExOption("containerSowType", SelectContainerSowType)
                     .AddQueryExOption("outContainerCode", OutContainerCode)
                     .AddQueryExOption("workType", srcView == "P0808050000" ? "0" : "1")
                     .ToObservableCollection();

      if (Results == null || !Results.Any())
      {
        BoxDetailList = null;
        ShowWarningMessage("查無資料");
      }
    }
    #endregion Search

    #region 關箱Command
    public ICommand CloseBoxCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoCloseBox(),
            () => SelectData != null && SelectData.STATUS == "0",
            o => PrintCommand.Execute(null)
        );
      }
    }

    private Boolean DoCloseBox()
    {
      var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
      var result = proxy.RunWcfMethod(w => w.CloseBox(new wcf.OutContainerInfo { F0531_ID = SelectData.F0531_ID, OUT_CONTAINER_CODE = SelectData.OUT_CONTAINER_CODE }));

      ShowResultMessage(result);

      return result.IsSuccessed;
    }
    #endregion

    #region 重新裝箱Command
    public ICommand RePackBoxCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoRePackBox(), () => SelectData != null && SelectData.STATUS == "0"
        );
      }
    }

    private void DoRePackBox()
    {
      var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
      var result = proxy.RunWcfMethod(w => w.RePackingBox(new wcf.OutContainerInfo { F0531_ID = SelectData.F0531_ID, OUT_CONTAINER_CODE = SelectData.OUT_CONTAINER_CODE }));

      ShowResultMessage(result);
    }

    #endregion

    #region 列印Command
    public ICommand PrintCommand
    {
      get
      {
        List<P0808050000_PrintData> PrintDetailData = null;
        List<P0808050000_CancelPrintData> PrintCancelDetailData = null;

        return CreateBusyAsyncCommand(
          o =>
          {
            PrintDetailData = null;
            PrintCancelDetailData = null;

            if (SelectData.SOW_TYPE == "1")
              PrintCancelDetailData = GetCancelPrintData();
            else
              PrintDetailData = GetPrintData();
          },
          () => SelectData != null,
          o =>
          {
            if (PrintDetailData != null)
            {
              DoPrint(PrintDetailData);
            }
            else if (PrintCancelDetailData != null)
            {
              DoPrintCancel(PrintCancelDetailData);
            }
          });
      }
    }

    private List<P0808050000_PrintData> GetPrintData()
    {
      var exproxy = GetExProxy<P08ExDataSource>();
      var PrintDetailData = exproxy.CreateQuery<P0808050000_PrintData>("GetPrintData")
        .AddQueryExOption("F0531ID", SelectData.F0531_ID.ToString())
        .ToList();

      if (!PrintDetailData.Any())
      {
        ShowWarningMessage("查無列印資料");
        return null;
      }

      return PrintDetailData;
    }

    private List<P0808050000_CancelPrintData> GetCancelPrintData()
    {
      var exproxy = GetExProxy<P08ExDataSource>();
      var PrintDetailData = exproxy.CreateQuery<P0808050000_CancelPrintData>("GetCancelPrintData")
        .AddQueryExOption("F0531ID", SelectData.F0531_ID.ToString())
        .ToList();

      if (!PrintDetailData.Any())
      {
        ShowWarningMessage("查無列印資料");
        return null;
      }

      return PrintDetailData;
    }

    private void DoPrint(List<P0808050000_PrintData> PrintDetailData)
    {
      var deliveryReport = new Services.DeliveryReportService(FunctionCode);
      deliveryReport.P080805PrintBoxData(SelectData, PrintDetailData);
    }

    private void DoPrintCancel(List<P0808050000_CancelPrintData> PrintDetailData)
    {
      var deliveryReport = new Services.DeliveryReportService(FunctionCode);
      deliveryReport.P080805PrintCancelBoxData(SelectData, PrintDetailData);
    }
    #endregion
  }
}
