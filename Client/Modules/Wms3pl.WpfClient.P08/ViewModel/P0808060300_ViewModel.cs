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
  public class P0808060300_ViewModel : InputViewModelBase
  {
    #region Property

    private string _gupCode;
    private string _custCode;
    public Action DoContainerFocus = delegate { };


    #region 物流中心清單
    private List<Common.NameValuePair<string>> _dcList;

    public List<Common.NameValuePair<string>> DcList
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
        RaisePropertyChanged();
      }
    }
    #endregion

    #region 跨庫目的地
    private List<Common.NameValuePair<string>> _moveOutDcList;

    public List<Common.NameValuePair<string>> MoveOutDcList
    {
      get { return _moveOutDcList; }
      set
      {
        Set(() => MoveOutDcList, ref _moveOutDcList, value);

        if (value != null && value.Any())
        {
          SelectMoveOutDc = value.FirstOrDefault().Value;
        }
      }
    }
    #endregion

    #region 選取跨庫目的地

    private string _selectMoveOutDc;

    public string SelectMoveOutDc
    {
      get { return _selectMoveOutDc; }
      set
      {
        Set(() => SelectMoveOutDc, ref _selectMoveOutDc, value);
      }
    }
    #endregion

    #region 揀貨容器條碼
    private string _pickContainerCode;
    /// <summary>
    /// 揀貨容器條碼
    /// </summary>
		public string PickContainerCode
    {
      get { return _pickContainerCode; }
      set
      {
        Set(() => PickContainerCode, ref _pickContainerCode, value);
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

    #region 揀貨單查詢結果
    private ObservableCollection<ExDataServices.P08ExDataService.MoveOutPickOrders> _results;
    /// <summary>
    /// 揀貨單查詢結果
    /// </summary>
    public ObservableCollection<ExDataServices.P08ExDataService.MoveOutPickOrders> Results
    {
      get { return _results; }
      set
      {
        Set(() => Results, ref _results, value);
      }
    }
    #endregion

    #endregion

    public P0808060300_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
        DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;

        if (DcList.Any())
          SelectedDc = DcList.First().Value;

        _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

        MoveOutDcList = GetBaseTableService.GetMoveOutDcList(FunctionCode);

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
          o => DoSearch(), () => UserOperateMode == OperateMode.Query);
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
      Results = exProxy.CreateQuery<MoveOutPickOrders>("GetMoveOutPickOrders")
                     .AddQueryExOption("dcCode", SelectedDc)
                     .AddQueryExOption("gupCode", _gupCode)
                     .AddQueryExOption("custCode", _custCode)
                     .AddQueryExOption("startDate", StartDate.Value)
                     .AddQueryExOption("endDate", EndDate.Value)
                     .AddQueryExOption("moveOutTarget", SelectMoveOutDc)
                     .AddQueryExOption("pickContainerCode", PickContainerCode)
                     .ToObservableCollection();

      if (Results == null || !Results.Any())
        ShowWarningMessage("查無資料");
    }
    #endregion Search
  }
}
