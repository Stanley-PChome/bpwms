using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
  public class P0202080000_ViewModel : InputViewModelBase
  {
    public Action SetContainerFocus = delegate { };
    public Action ShowNGItemForm = delegate { };
    public Action ShowSerialNoForm = delegate { };
    public readonly string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
    public readonly string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
    public P0202080000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        SetDcCode();
        SetProcCodeList();
        SetStatusList();
        _RemoveSerialNos = new List<string>();
      }
    }

    private void SetDcCode()
    {
      DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
      if (DcList.Any())
      {
        SelectedDc = DcList.First().Value;
        AddSelectedDc = DcList.First().Value;
      }
    }

    private void SetProcCodeList()
    {
      ProcCodeList = GetBaseTableService.GetF000904List(FunctionCode, "F020504", "PROC_CODE", true);
      if (ProcCodeList.Any())
        SelectedProcCode = ProcCodeList.First().Value;

      AddProcCodeList = ProcCodeList.MapCollection<NameValuePair<string>, NameValuePair<string>>().ToList();
      AddProcCodeList.RemoveAt(0);
      if (AddProcCodeList.Any())
        AddSelectedProcCode = AddProcCodeList.First().Value;
    }

    private void SetStatusList()
    {
      StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F020504", "STATUS", true);
      if (StatusList.Any())
        SelectedStatus = StatusList.First().Value;
    }

    #region UI屬性
    #region 物流中心清單
    private List<NameValuePair<string>> _dcList;
    /// <summary>
    /// 物流中心清單
    /// </summary>
    public List<NameValuePair<string>> DcList
    {
      get { return _dcList; }
      set { Set(() => DcList, ref _dcList, value); }
    }
    #endregion 物流中心清單

    #region 所選的物流中心
    private string _selectedDc;
    /// <summary>
    /// 所選的物流中心
    /// </summary>
    public string SelectedDc
    {
      get { return _selectedDc; }
      set { Set(() => SelectedDc, ref _selectedDc, value); }
    }

    private string _AddSelectedDc;
    /// <summary>
    /// 所選的物流中心
    /// </summary>
    public string AddSelectedDc
    {
      get { return _AddSelectedDc; }
      set { Set(() => AddSelectedDc, ref _AddSelectedDc, value); }
    }

    #endregion 所選的物流中心

    #region 處理日期
    private DateTime? _ProcDateStart = DateTime.Now.Date;
    /// <summary>
    /// 查詢起始處理日期
    /// </summary>
    public DateTime? ProcDateStart
    {
      get { return _ProcDateStart; }
      set { _ProcDateStart = value; RaisePropertyChanged(); }
    }

    private DateTime? _ProcDateEnd = DateTime.Now.Date;
    /// <summary>
    /// 查詢起始處理日期
    /// </summary>
    public DateTime? ProcDateEnd
    {
      get { return _ProcDateEnd; }
      set { _ProcDateEnd = value; RaisePropertyChanged(); }
    }
    #endregion 處理日期

    #region 進倉單號
    private string _StockNo;
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string StockNo
    {
      get { return _StockNo; }
      set { _StockNo = value; RaisePropertyChanged(); }
    }
    #endregion 進倉單號

    #region 驗收單號
    private string _RTNo;
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RTNo
    {
      get { return _RTNo; }
      set { _RTNo = value; RaisePropertyChanged(); }
    }
    #endregion 驗收單號

    #region 處理方式
    private List<NameValuePair<string>> _ProcCodeList;
    /// <summary>
    /// 處理方式清單
    /// </summary>
    public List<NameValuePair<string>> ProcCodeList
    {
      get { return _ProcCodeList; }
      set { Set(() => ProcCodeList, ref _ProcCodeList, value); }
    }
    private string _SelectedProcCode;
    /// <summary>
    /// 所選的處理方式
    /// </summary>
    public string SelectedProcCode
    {
      get { return _SelectedProcCode; }
      set { Set(() => SelectedProcCode, ref _SelectedProcCode, value); }
    }

    private List<NameValuePair<string>> _AddProcCodeList;
    /// <summary>
    /// 處理方式清單
    /// </summary>
    public List<NameValuePair<string>> AddProcCodeList
    {
      get { return _AddProcCodeList; }
      set { Set(() => AddProcCodeList, ref _AddProcCodeList, value); }
    }
    private string _AddSelectedProcCode;
    /// <summary>
    /// 所選的處理方式
    /// </summary>
    public string AddSelectedProcCode
    {
      get { return _AddSelectedProcCode; }
      set
      {
        Set(() => AddSelectedProcCode, ref _AddSelectedProcCode, value);
        RaisePropertyChanged("IsModifyQtyMode");
      }
    }

    public Visibility IsModifyQtyMode
    {
      get { return AddSelectedProcCode == "2" ? Visibility.Visible : Visibility.Collapsed; }
    }

    #endregion 處理方式

    #region 容器條碼
    private string _ContainerCode;
    /// <summary>
    /// 容器條碼
    /// </summary>
    public string ContainerCode
    {
      get { return _ContainerCode; }
      set { _ContainerCode = value; RaisePropertyChanged(); }
    }
    #endregion 容器條碼

    #region 品號
    private string _ItemCode;
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemCode
    {
      get { return _ItemCode; }
      set { _ItemCode = value; RaisePropertyChanged(); }
    }
    #endregion 品號

    #region 處理狀態
    private List<NameValuePair<string>> _StatusList;
    /// <summary>
    /// 處理狀態清單
    /// </summary>
    public List<NameValuePair<string>> StatusList
    {
      get { return _StatusList; }
      set { Set(() => StatusList, ref _StatusList, value); }
    }
    private string _SelectedStatus;
    /// <summary>
    /// 所選的處理狀態
    /// </summary>
    public string SelectedStatus
    {
      get { return _SelectedStatus; }
      set { Set(() => SelectedStatus, ref _SelectedStatus, value); }
    }
    #endregion 處理狀態

    #region 查詢結果
    private ObservableCollection<F020504ExData> _f020504ExDatas;
    /// <summary>
    /// 查詢結果清單
    /// </summary>
    public ObservableCollection<F020504ExData> f020504ExDatas
    {
      get { return _f020504ExDatas; }
      set { _f020504ExDatas = value; RaisePropertyChanged(); }
    }

    private F020504ExData _Selectedf020504ExDatas;
    /// <summary>
    /// 選中的查詢結果
    /// </summary>
    public F020504ExData Selectedf020504ExDatas
    {
      get { return _Selectedf020504ExDatas; }
      set
      {
        _Selectedf020504ExDatas = value;
        RaisePropertyChanged();
        DoGetUnnormalItemRecheckLog();
      }
    }

    #endregion 查詢結果

    #region 複驗處理方式
    private string _ProcType;
    /// <summary>
    /// 0:修改驗收數量 1:修改不良品數
    /// </summary>
    public string ProcType
    {
      get { return _ProcType; }
      set
      {
        _ProcType = value;
        if (value != "0") //如果選擇修改驗收數外的項目，就要把數量改回0
          RemoveRecvQty = 0;
        RaisePropertyChanged();
      }
    }
    #endregion 複驗處理方式

    #region 備註
    private string _AddMemo;
    public string AddMemo
    {
      get { return _AddMemo; }
      set { _AddMemo = value; RaisePropertyChanged(); }
    }
    #endregion 備註

    #region 處理記錄
    private ObservableCollection<UnnormalItemRecheckLog> _UnnormalItemRecheckLogs;
    /// <summary>
    /// 處理記錄清單
    /// </summary>
    public ObservableCollection<UnnormalItemRecheckLog> UnnormalItemRecheckLogs
    {
      get { return _UnnormalItemRecheckLogs; }
      set
      {
        _UnnormalItemRecheckLogs = value; RaisePropertyChanged();
      }
    }

    private UnnormalItemRecheckLog _SelectedUnnormalItemRecheckLog;
    /// <summary>
    /// 選中的處理記錄
    /// </summary>
    public UnnormalItemRecheckLog SelectedUnnormalItemRecheckLog
    {
      get { return _SelectedUnnormalItemRecheckLog; }
      set { _SelectedUnnormalItemRecheckLog = value; RaisePropertyChanged(); }
    }
    #endregion 處理記錄

    #region Selectedtabindex
    private int _Selectedtabindex;
    public int Selectedtabindex
    {
      get { return _Selectedtabindex; }
      set { _Selectedtabindex = value; RaisePropertyChanged(); }
    }

    #endregion SelectIndex

    #region 容器條碼
    private string _AddContainerCode;
    public string AddContainerCode
    {
      get { return _AddContainerCode; }
      set { _AddContainerCode = value; RaisePropertyChanged(); }
    }
    #endregion 容器條碼

    #region 複驗異常容器內容
    private ObservableCollection<ContainerRecheckFaildItem> _ContainerRecheckFaildItems;
    public ObservableCollection<ContainerRecheckFaildItem> ContainerRecheckFaildItems
    {
      get { return _ContainerRecheckFaildItems; }
      set { _ContainerRecheckFaildItems = value; RaisePropertyChanged(); }
    }

    private ContainerRecheckFaildItem _SelectedContainerRecheckFaildItem;
    public ContainerRecheckFaildItem SelectedContainerRecheckFaildItem
    {
      get { return _SelectedContainerRecheckFaildItem; }
      set
      {
        _SelectedContainerRecheckFaildItem = value;
        RaisePropertyChanged();
        RaisePropertyChanged("IsRecheckFaild");
        RaisePropertyChanged("TrialCalRecvQty");
      }
    }

    #endregion 複驗異常容器內容

    #region 是否選取的項目為複驗失敗
    public Boolean IsRecheckFaild
    { get { return SelectedContainerRecheckFaildItem?.STATUS == "3"; } }
    #endregion 是否選取的項目為複驗失敗

    #region 驗收清單
    private P020203Data _p020203Datas;
    public P020203Data p020203Datas
    { get { return _p020203Datas; } }
    #endregion 驗收清單

    #region 移除數量
    private int _RemoveRecvQty;
    public int RemoveRecvQty
    {
      get { return _RemoveRecvQty; }
      set
      {
        _RemoveRecvQty = value;
        RaisePropertyChanged();
        RaisePropertyChanged("TrialCalRecvQty");
      }
    }
    #endregion 移除數量

    #region 試算後驗收量
    public int TrialCalRecvQty
    {
      get { return (SelectedContainerRecheckFaildItem?.QTY ?? 0) - RemoveRecvQty; }
    }
    #endregion 試算後驗收量

    #region 不良品數量
    public int NotGoodQty
    {
      get
      {
        if (f02020109Datas == null || f02020109Datas.Count == 0)
          return 0;
        return f02020109Datas.Where(x =>
          x.DC_CODE == AddSelectedDc &&
          x.GUP_CODE == _gupCode &&
          x.CUST_CODE == _custCode &&
          x.STOCK_NO == SelectedContainerRecheckFaildItem.STOCK_NO &&
          x.STOCK_SEQ == int.Parse(SelectedContainerRecheckFaildItem.STOCK_SEQ)).Sum(x => x?.DEFECT_QTY ?? 0);
      }
    }
    #endregion 不良品數量

    #region 不良品數量內容
    /// <summary>
    /// 會儲存這容器所有的
    /// </summary>
    private List<F02020109Data> _f02020109Datas;
    public List<F02020109Data> f02020109Datas
    {
      get { return _f02020109Datas; }
      set
      {
        _f02020109Datas = value;
        RaisePropertyChanged();
        RaisePropertyChanged("NotGoodQty");
      }
    }
    #endregion 不良品數量內容

    #region 不良品容器編號
    private string _NgContainerCode;
    /// <summary>
    /// 不良品容器編號
    /// </summary>
    public string NgContainerCode
    {
      get { return _NgContainerCode; }
      set
      {
        _NgContainerCode = value;
        RaisePropertyChanged();
      }
    }
    #endregion 不良品容器編號

    #region 移出序號清單
    private List<string> _RemoveSerialNos;
    /// <summary>
    /// 移出序號清單
    /// </summary>
    public List<string> RemoveSerialNos
    {
      get { return _RemoveSerialNos; }
      set { Set(() => RemoveSerialNos, ref _RemoveSerialNos, value); }
    }
    #endregion 移出序號清單

    #endregion UI屬性


    #region ICommand
    #region 查詢
    public ICommand SearchCommand
    {
      get
      {
        ContainerRecheckFaildItem ProcessContainerItem = null;
        return CreateBusyAsyncCommand(
        o =>
        {
          if (o != null)
            ProcessContainerItem = (ContainerRecheckFaildItem)o;
          DoSearch();
        },
        () => UserOperateMode == OperateMode.Query,
        o =>
        {
          if (ProcessContainerItem != null)
          {
            var tmpf020504ExDatas = f020504ExDatas.FirstOrDefault(x => x.STOCK_NO == ProcessContainerItem.STOCK_NO && x.CONTAINER_CODE == ProcessContainerItem.CONTAINER_CODE && x.BIN_CODE == ProcessContainerItem.BIN_CODE);
            if (tmpf020504ExDatas != null)
              Selectedf020504ExDatas = tmpf020504ExDatas;
          }

        });
      }
    }

    private void DoSearch()
    {
      var proxy = GetExProxy<P02ExDataSource>();

      f020504ExDatas = proxy.CreateQuery<F020504ExData>("GetF020504ExDatas")
                       .AddQueryExOption("dcCode", SelectedDc)
                       .AddQueryExOption("gupCode", _gupCode)
                       .AddQueryExOption("custCode", _custCode)
                       .AddQueryExOption("ProcDateStart", ProcDateStart)
                       .AddQueryExOption("ProcDateEnd", ProcDateEnd)
                       .AddQueryExOption("StockNo", StockNo)
                       .AddQueryExOption("RTNo", RTNo)
                       .AddQueryExOption("ProcCode", SelectedProcCode)
                       .AddQueryExOption("ContainerCode", ContainerCode)
                       .AddQueryExOption("ItemCode", ItemCode)
                       .AddQueryExOption("Status", SelectedStatus)
                       .ToObservableCollection();
      if (!f020504ExDatas.Any())
      {
        ShowInfoMessage("查無資料");
        return;
      }
      DispatcherAction(() => { Selectedf020504ExDatas = f020504ExDatas.First(); });
    }

    private void DoGetUnnormalItemRecheckLog()
    {
      UnnormalItemRecheckLogs = null;
      SelectedUnnormalItemRecheckLog = null;
      if (Selectedf020504ExDatas == null)
        return;

      var proxya = GetExProxy<P02ExDataSource>();
      UnnormalItemRecheckLogs = proxya.CreateQuery<UnnormalItemRecheckLog>("GetUnnormalItemRecheckLog")
                                .AddQueryExOption("f020504ID", Selectedf020504ExDatas.ID.ToString())
                                .ToObservableCollection();

      if (!UnnormalItemRecheckLogs.Any())
      {
        ShowInfoMessage("查無處理記錄資料");
        return;
      }
      SelectedUnnormalItemRecheckLog = UnnormalItemRecheckLogs.First();
    }
    #endregion 查詢

    #region 新增
    public ICommand AddCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o =>
          {
            ChangeOperateModeToAdd();
          },
          () => UserOperateMode == OperateMode.Query
        );
      }
    }
    #endregion 新增 

    #region 返回
    public ICommand CancelCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o =>
          {
            ChangeOperateModeToQuery();
            ClearAddViewValue();
          },
          () => UserOperateMode != OperateMode.Query
        );
      }
    }

    #endregion 返回

    #region 查詢複驗異常容器
    public ICommand GetContainerRecheckFaildCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => GetContainerRecheckFaild(),
          completed: o =>
          {
            if (ContainerRecheckFaildItems.Any())
              SelectedContainerRecheckFaildItem = ContainerRecheckFaildItems.First();
          }
          );
      }
    }
    private void GetContainerRecheckFaild()
    {
      var proxy = GetExProxy<P02ExDataSource>();
      ContainerRecheckFaildItems = proxy.CreateQuery<ContainerRecheckFaildItem>("GetContainerRecheckFaildItem")
                                .AddQueryExOption("dcCode", AddSelectedDc)
                                .AddQueryExOption("gupCode", _gupCode)
                                .AddQueryExOption("custCode", _custCode)
                                .AddQueryExOption("ContainerCode", AddContainerCode)
                                .ToObservableCollection();

      if (!ContainerRecheckFaildItems.Any())
      {
        ShowInfoMessage("找不到符合的容器明細資料");
        DispatcherAction(() => { SetContainerFocus(); });
        return;
      }

    }
    #endregion 查詢複驗異常容器

    #region 設定不良品
    public ICommand SetNGItemCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o =>
          {
            GetP020203Datas();
          },
          () => ProcType == "1" && IsRecheckFaild,
           o => { if (_p020203Datas != null) ShowNGItemForm(); }
          );
      }
    }

    public void GetP020203Datas()
    {
      if (SelectedContainerRecheckFaildItem == null)
        return;
      //如果等太久的話，這邊可以優化成檢查_p020203Datas他的資料是否跟現在選擇的有不同，如果沒有不同，就可以跳出這function

      var proxy = GetExProxy<P02ExDataSource>();
      _p020203Datas = proxy.CreateQuery<P020203Data>("GetModifyRecheckNGDatas")
                       .AddQueryExOption("dcCode", AddSelectedDc)
                       .AddQueryExOption("gupCode", _gupCode)
                       .AddQueryExOption("custCode", _custCode)
                       .AddQueryExOption("purchaseNo", SelectedContainerRecheckFaildItem.STOCK_NO)
                       .AddQueryExOption("rtNo", SelectedContainerRecheckFaildItem.RT_NO)
                       .AddQueryExOption("stockSeq", SelectedContainerRecheckFaildItem.STOCK_SEQ)
                       .ToList().FirstOrDefault();
      if (_p020203Datas == null)
      {
        ShowInfoMessage("查無原始驗收資料");
        return;
      }
      _p020203Datas.RECV_QTY = SelectedContainerRecheckFaildItem.QTY;
    }
    #endregion 設定不良品

    #region 複驗通過Command
    public ICommand SaveCommand
    {
      get
      {
        Boolean IsSuccess = false;
        return CreateBusyAsyncCommand(
          o => IsSuccess = DoSave(),
          () =>
          {
            if (SelectedContainerRecheckFaildItem == null)
              return false;
            if (!IsRecheckFaild)
              return false;
            //手動排除異常
            if (AddSelectedProcCode == "1")
              return true;
            if (AddSelectedProcCode == "2" && ProcType == "0" && RemoveRecvQty > 0)
              return true;
            if (AddSelectedProcCode == "2" && ProcType == "1" && NotGoodQty > 0 && !string.IsNullOrWhiteSpace(NgContainerCode))
              return true;

            return false;
          },
          o =>
          {
            if (IsSuccess)
            {
              CleanSearchCondition();
              SearchCommand.Execute(SelectedContainerRecheckFaildItem);

              ClearAddViewValue();
              ChangeOperateModeToQuery();
            }
          }
        );
      }
    }

    private Boolean DoSave()
    {
      var proxy = GetWcfProxy<wcf.P02WcfServiceClient>();
      wcf.ExecuteResult result = null;

      var verifyResult = VerifyAddData();
      if (!verifyResult.IsSuccessed)
      {
        ShowWarningMessage(verifyResult.Message);
        return false;
      }
			var item = SelectedContainerRecheckFaildItem;

			if (AddSelectedProcCode == "1") //手動排除異常
        result = proxy.RunWcfMethod(w => w.InsertManualProcessData(item.F020501_ID, item.F020502_ID, AddMemo));
      else
      {
        if (ProcType == "0") //修改驗收數量
        {
          result = proxy.RunWcfMethod(w => w.InsertModifyRecvQtyData(item.F020501_ID, item.F020502_ID, RemoveRecvQty, AddMemo, RemoveSerialNos.ToArray()));
        }
        if (ProcType == "1") //修改不良品數
        {
          var tmpf02020109Datas = f02020109Datas.MapCollection<F02020109Data, wcf.F02020109Data>();
          result = proxy.RunWcfMethod(w => w.InsertModifyNGQtyData(item.F020501_ID, item.F020502_ID, tmpf02020109Datas.ToArray(), AddMemo, NgContainerCode));
        }
      }

      ShowResultMessage(result);

      return result.IsSuccessed;
    }
    #endregion 複驗通過Command

    public ICommand ShowSerialNoWinCommand
    {
      get { return new RelayCommand(() => ShowSerialNoForm(), () => true); }
    }

    #endregion ICommand

    /// <summary>
    /// 清除新增異常處理畫面內容
    /// </summary>
    private void ClearAddViewValue()
    {
      AddSelectedDc = DcList.FirstOrDefault()?.Value;
      AddContainerCode = "";
      ContainerRecheckFaildItems = new ObservableCollection<ContainerRecheckFaildItem>();
      SelectedContainerRecheckFaildItem = null;
      AddSelectedProcCode = AddProcCodeList.FirstOrDefault()?.Value;
      ProcType = null;
      RemoveRecvQty = 0;
      AddMemo = "";
      f02020109Datas = new List<F02020109Data>();
      NgContainerCode = null;
      RemoveSerialNos = new List<string>();
    }

    /// <summary>
    /// 清除搜尋條件
    /// </summary>
    private void CleanSearchCondition()
    {
      ProcDateStart = DateTime.Now.Date;
      ProcDateEnd = DateTime.Now.Date;
      StockNo = "";
      RTNo = "";
      SelectedProcCode = "";
      ContainerCode = "";
      ItemCode = "";
      SelectedStatus = "";
    }

    private void ChangeOperateModeToQuery()
    {
      UserOperateMode = OperateMode.Query;
      Selectedtabindex = 0;
    }

    private void ChangeOperateModeToAdd()
    {
      UserOperateMode = OperateMode.Add;
      Selectedtabindex = 1;
      DispatcherAction(() =>
      {
        SetContainerFocus();
      });
    }

    /// <summary>
    /// 驗證使用者輸入的
    /// </summary>
    /// <returns></returns>
    public ExecuteResult VerifyAddData()
    {
      if (SelectedContainerRecheckFaildItem.QTY < RemoveRecvQty)
        return new ExecuteResult() { IsSuccessed = false, Message = "移除數量不可大於實際數量" };

      return new ExecuteResult() { IsSuccessed = true };
    }

  }
}
