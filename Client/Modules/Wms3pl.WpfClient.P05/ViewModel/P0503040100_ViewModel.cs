using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.UILib;
using Wcf = Wms3pl.WpfClient.ExDataServices.P05WcfService;
using T05Wcf = Wms3pl.WpfClient.ExDataServices.T05WcfService;
using System.Windows;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P05.ViewModel
{
  public partial class P0503040100_ViewModel : InputViewModelBase
  {
    private string _gupCode;
    private string _custCode;
    private string _userId;
    private string _userName;
    public Action CloseWindows = delegate { };

    public P0503040100_ViewModel()
    {
      if (!IsInDesignMode)
      {
        _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
        _userId = Wms3plSession.Get<UserInfo>().Account;
        _userName = Wms3plSession.Get<UserInfo>().AccountName;
        IsExpendTotalStock = true;
				SetPriorityList();
			}
    }


    #region Property

    #region 呼叫位置
    /// <summary>
    /// 呼叫位置 0:手動挑單 1:配庫試算結果查詢
    /// </summary>
    public string Flag;
    #endregion 呼叫位置


    public Visibility IsAllotButtonShow
    { get { return Flag == "0" ? Visibility.Visible : Visibility.Collapsed; } }

    #region 物流中心
    private string _dcCode;

    public string DcCode
    {
      get { return _dcCode; }
      set
      {
        Set(() => DcCode, ref _dcCode, value);
      }
    }
    #endregion

    #region 物流中心名稱
    private string _dcName;

    public string DcName
    {
      get { return _dcName; }
      set
      {
        Set(() => DcName, ref _dcName, value);
      }
    }
    #endregion

    #region 試算編號
    private string _calNo;

    public string CalNo
    {
      get { return _calNo; }
      set
      {
        Set(() => CalNo, ref _calNo, value);
      }
    }
    #endregion

    #region 試算頭檔
    private F05080503 _master;

    public F05080503 Master
    {
      get { return _master; }
      set
      {
        Set(() => Master, ref _master, value);
      }
    }
    #endregion

    #region 各儲區出貨占比資料
    private ObservableCollection<F05080504Data> _areaDelvRatioDetails;

    public ObservableCollection<F05080504Data> AreaDelvRatioDetails
    {
      get { return _areaDelvRatioDetails; }
      set
      {
        Set(() => AreaDelvRatioDetails, ref _areaDelvRatioDetails, value);
      }
    }
    #endregion

    #region 商品總庫缺貨資料
    private SelectionList<F050805Data> _calculationDatas;
    public SelectionList<F050805Data> CalculationDatas
    {
      get { return _calculationDatas; }
      set
      {
        Set(() => CalculationDatas, ref _calculationDatas, value);
      }
    }
    #endregion

    #region 選取的商品總庫資料
    private SelectionItem<F050805Data> _selectedCalculation;

    public SelectionItem<F050805Data> SelectedCalculation
    {
      get { return _selectedCalculation; }
      set
      {
        Set(() => SelectedCalculation, ref _selectedCalculation, value);
        if (SelectedCalculation != null)
          DoSearchOutStockOrderDetailByItemCode(SelectedCalculation.Item.ITEM_CODE);
      }
    }
    #endregion

    #region 缺貨訂單資料
    private SelectionList<F05080501Data> _outStcokOrders;
    public SelectionList<F05080501Data> OutStockOrders
    {
      get { return _outStcokOrders; }
      set
      {
        Set(() => OutStockOrders, ref _outStcokOrders, value);
      }
    }
    #endregion

    #region 缺貨訂單明細
    private List<F05080502Data> _allOutStockOrderDetails { get; set; }
    private ObservableCollection<F05080502Data> _outStockOrderDetails;
    public ObservableCollection<F05080502Data> OutStockOrderDetails
    {
      get { return _outStockOrderDetails; }
      set
      {
        Set(() => OutStockOrderDetails, ref _outStockOrderDetails, value);
      }
    }
    #endregion

    #region 是否全選_缺貨商品清單

    private bool _isCheckAll;

    public bool IsCheckAll
    {
      get { return _isCheckAll; }
      set
      {
        _isCheckAll = value;
        Set(() => IsCheckAll, ref _isCheckAll, value);
        CheckSelectedAll(value);
      }
    }
    #endregion

    #region 是否全選_缺貨訂單清單

    private bool _isCheckAllOrder;

    public bool IsCheckAllOrder
    {
      get { return _isCheckAllOrder; }
      set
      {
        Set(() => IsCheckAllOrder, ref _isCheckAllOrder, value);
        CheckSelectedAllOrder(value);
      }
    }
    #endregion

    #region 是否全選_各訂單預計揀貨倉別
    private bool _isCheckAllWarehouseOrder;
    public bool IsCheckAllWarehouseOrder
    {
      get { return _isCheckAllWarehouseOrder; }
      set
      {
        _isCheckAllWarehouseOrder = value;
        Set(() => IsCheckAllOrder, ref _isCheckAllOrder, value);
        CheckSelectedAllWarehouseOrder(value);
      }
    }
    #endregion 是否全選_各訂單預計揀貨倉別

    #region 調整Grid高
    private double _height;

    public double Height
    {
      get { return _height; }
      set
      {
        Set(() => Height, ref _height, value);
      }
    }
    #endregion

    #region 是否展開商品總庫缺貨資料
    private bool _isExpendTotalStock;

    public bool IsExpendTotalStock
    {
      get { return _isExpendTotalStock; }
      set
      {
        Set(() => IsExpendTotalStock, ref _isExpendTotalStock, value);
        if (value)
        {
          Height = 300;
        }
        else
        {
          Height = 480;
        }
      }
    }
    #endregion

    #region TabControlSelectedIndex
    private int _SelectedTabIndex = 0;
    public int SelectedTabIndex
    {
      get { return _SelectedTabIndex; }
      set { _SelectedTabIndex = value; RaisePropertyChanged(); }
    }
    #endregion TabControlSelectedIndex

    #region 各訂單預計揀貨倉別清單
    private SelectionList<F05080505Data> _PlanPickWarehouse;
    /// <summary>
    /// 各訂單預計揀貨倉別清單
    /// </summary>
    public SelectionList<F05080505Data> PlanPickWarehouse
    {
      get { return _PlanPickWarehouse; }
      set { _PlanPickWarehouse = value; RaisePropertyChanged(); }
    }
    #endregion 各訂單預計揀貨倉別清單

    #region 各訂單商品預計揀貨倉別明細清單
    private ObservableCollection<F05080506Data> _PlanPickWarehouseDetail;
    /// <summary>
    /// 各訂單商品預計揀貨倉別明細清單
    /// </summary>
    public ObservableCollection<F05080506Data> PlanPickWarehouseDetail
    {
      get { return _PlanPickWarehouseDetail; }
      set { _PlanPickWarehouseDetail = value; RaisePropertyChanged(); }
    }
		#endregion 各訂單商品預計揀貨倉別明細清單

		#region 指定自動倉揀貨優先處理旗標清單
		private List<NameValuePair<string>> _priorityList;

		public List<NameValuePair<string>> PriorityList
		{
			get { return _priorityList; }
			set
			{
				Set(() => PriorityList, ref _priorityList, value);
			}
		}
		#endregion
    #region 是否有按下配庫按鈕
    public Boolean IsDoAllocation { get; set; } = false;
    #endregion

    #endregion


		#region 選擇指定自動倉揀貨優先處理旗標代碼
		private string _selectedPriorityCode;

		public string SelectedPriorityCode
		{
			get { return _selectedPriorityCode; }
			set
			{
				Set(() => SelectedPriorityCode, ref _selectedPriorityCode, value);
			}
		}
		#endregion


		#region Method & Constructor
		public void StartLoad()
    {
      var dcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
      var dc = dcList.FirstOrDefault(x => x.Value == DcCode);
      if (dc != null)
        DcName = dc.Name;

      if (!string.IsNullOrWhiteSpace(CalNo))
        DoSearch();
    }

		#region 指定自動倉優先處理旗標清單
		private void SetPriorityList()
		{
			var proxy = GetProxy<F19Entities>();
			var list = proxy.F1956s.Where(x => x.IS_SHOW == "1").Select(x => new NameValuePair<string>
			{
				Name = x.PRIORITY_NAME,
				Value = x.PRIORITY_CODE
			}).ToList();
			list.Insert(0, new NameValuePair<string> { Name = "不指定", Value = "" });
			PriorityList = list;
			if (list.Any())
				SelectedPriorityCode = list.First().Value;

		}

		#endregion

		#region Grid 全選-缺貨商品清單

		public void CheckSelectedAll(bool isChecked)
    {
      if (CalculationDatas != null)
      {
        foreach (var calculation in CalculationDatas)
        {
          if (calculation.Item.SUG_RESUPPLY_STOCK_QTY > 0)
            calculation.IsSelected = isChecked;
        }
      }
    }

    #endregion

    #region Grid 全選-缺貨訂單清單

    public void CheckSelectedAllOrder(bool isChecked)
    {
      if (OutStockOrders != null)
      {
        foreach (var outStockOrder in OutStockOrders)
        {
          outStockOrder.IsSelected = isChecked;
        }
      }
    }

    #endregion

    #region Grid 全選-各訂單預計揀貨倉別
    public void CheckSelectedAllWarehouseOrder(bool isChecked)
    {
      foreach (var item in PlanPickWarehouse)
      {
        if (item.Item.IS_LACK_ORDER == "是")
          continue;
        item.IsSelected = isChecked;
      }
    }
    #endregion Grid 全選-各訂單預計揀貨倉別

    #region 查詢-試算結果

    private void DoSearch()
    {
      var proxyF05 = GetProxy<F05Entities>();
      Master = proxyF05.F05080503s.Where(x => x.DC_CODE == DcCode && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.CAL_NO == CalNo).ToList().FirstOrDefault();
      var proxy = GetExProxy<P05ExDataSource>();
      var data3s = proxy.CreateQuery<F05080504Data>("GetF05080504Datas")
          .AddQueryExOption("dcCode", DcCode)
          .AddQueryExOption("gupCode", _gupCode)
          .AddQueryExOption("custCode", _custCode)
          .AddQueryExOption("calNo", CalNo)
          .ToList();
      AreaDelvRatioDetails = data3s.ToObservableCollection();

      var datas = proxy.CreateQuery<F050805Data>("GetF050805Datas")
          .AddQueryExOption("dcCode", DcCode)
          .AddQueryExOption("gupCode", _gupCode)
          .AddQueryExOption("custCode", _custCode)
          .AddQueryExOption("calNo", CalNo)
          .ToList();
      CalculationDatas = datas.ToSelectionList();
      CheckSelectedAll(true);

      var data1s = proxy.CreateQuery<F05080501Data>("GetF05080501Datas")
          .AddQueryExOption("dcCode", DcCode)
          .AddQueryExOption("gupCode", _gupCode)
          .AddQueryExOption("custCode", _custCode)
          .AddQueryExOption("calNo", CalNo)
          .ToList();

      OutStockOrders = data1s.ToSelectionList();

      var data2s = proxy.CreateQuery<F05080502Data>("GetF05080502Datas")
          .AddQueryExOption("dcCode", DcCode)
          .AddQueryExOption("gupCode", _gupCode)
          .AddQueryExOption("custCode", _custCode)
          .AddQueryExOption("calNo", CalNo)
          .ToList();

      _allOutStockOrderDetails = data2s;

      PlanPickWarehouse = proxy.CreateQuery<F05080505Data>("GetF05080505Datas")
          .AddQueryExOption("dcCode", DcCode)
          .AddQueryExOption("gupCode", _gupCode)
          .AddQueryExOption("custCode", _custCode)
          .AddQueryExOption("calNo", CalNo)
          .AddQueryExOption("flag", Flag)
          .ToSelectionList();

      PlanPickWarehouseDetail = proxy.CreateQuery<F05080506Data>("GetF05080506Datas")
         .AddQueryExOption("dcCode", DcCode)
         .AddQueryExOption("gupCode", _gupCode)
         .AddQueryExOption("custCode", _custCode)
         .AddQueryExOption("calNo", CalNo)
         .AddQueryExOption("flag", Flag)
         .ToObservableCollection();
    }
    #endregion

    //public Wcf.ExecuteResult P05030401CreateAllocation(List<P05030401Data> datas)
    //{
    //	var proxy = GetWcfProxy<Wcf.P05WcfServiceClient>();
    //	var wcfDatas = ExDataMapper.MapCollection<P05030401Data, Wcf.P05030401Data>(datas).ToArray();
    //	var result = proxy.RunWcfMethod<Wcf.ExecuteResult>(w => w.SaveP05030401CreateAllocation(DcCode, _gupCode, _custCode, wcfDatas));

    //	return result;
    //}
    #endregion

    #region 查詢訂單缺貨明細
    /// <summary>
    /// 查詢訂單缺貨明細
    /// </summary>
    public ICommand SearchOutStockOrderDetailCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
                o => DoSearchOutStockOrderDetail(), () => OutStockOrders != null && OutStockOrders.Any(x => x.IsSelected)
        );
      }
    }

    public void DoSearchOutStockOrderDetail()
    {
      var ordNos = OutStockOrders.Where(x => x.IsSelected).Select(x => x.Item.ORD_NO).ToList();
      var data2s = _allOutStockOrderDetails.Where(x => ordNos.Contains(x.ORD_NO)).ToList();
      OutStockOrderDetails = data2s.ToObservableCollection();
    }
    public void DoSearchOutStockOrderDetailByItemCode(string itemCode)
    {
      var data2s = _allOutStockOrderDetails.Where(x => x.ITEM_CODE == itemCode).ToList();
      OutStockOrderDetails = data2s.ToObservableCollection();
    }
    #endregion MyCommand

    #region 產生補貨建議調撥單
    /// <summary>
    /// Gets the Save.
    /// </summary>
    public ICommand SaveCommand
    {
      get
      {
        Wcf.ExecuteResult result = null;
        return CreateBusyAsyncCommand(
                o => result = DoSave(),
                () => CalculationDatas != null && CalculationDatas.Any(x => x.IsSelected) && SelectedTabIndex == 0,
                o =>
                {
                  if (result.IsSuccessed)
                  {
                    ShowInfoMessage(result.Message);
                    CloseWindows();
                  }
                  else
                  {
                    ShowWarningMessage(result.Message);
                  }

                }
);
      }
    }

    public Wcf.ExecuteResult DoSave()
    {
      var proxy = GetWcfProxy<Wcf.P05WcfServiceClient>();
      var result = proxy.RunWcfMethod<Wcf.ExecuteResult>(w => w.SaveP05030401CreateAllocation(DcCode, _gupCode, _custCode, CalNo,
          CalculationDatas.Where(x => x.IsSelected == true).Select(x => x.Item.ID).ToArray()));

      return result;
    }
    #endregion Save

    #region Exit
    public ICommand ExitCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => { }, () => true,
            o => CloseWindows()
            );
      }
    }
    #endregion

    #region 配庫處理
    public ICommand AllocationCommand
    {
      get
      {
        Boolean IsSuccess = false;
        return CreateBusyAsyncCommand(
          o => IsSuccess = DoAllocation(),
          () => true,
          o => { if (IsSuccess) DoSearch(); }
          );
      }
    }
    private Boolean DoAllocation()
    {
      if (!PlanPickWarehouse.Any(x => x.IsSelected))
      {
        ShowWarningMessage(Properties.Resources.P0503040000_Pls_Select_Item);
        return false;
      }


      var OrderNos = PlanPickWarehouse.Where(x => x.IsSelected).Select(x => x.Item.ORD_NO);

      var wcfproxy = new T05Wcf.T05WcfServiceClient();
      var results = RunWcfMethod<T05Wcf.ExecuteResult[]>(wcfproxy.InnerChannel, () => wcfproxy.AllotStocks(OrderNos.ToArray(),SelectedPriorityCode));
      var hasShowMsg = false;
      foreach (var result in results)
      {
        if (result.IsSuccessed && string.IsNullOrEmpty(result.Message))
          continue;
        ShowResultMessage(result);
        hasShowMsg = true;
      }

      if (results.All(x => x.IsSuccessed))
        IsDoAllocation = true;
       
      if (!hasShowMsg)
        ShowMessage(new MessagesStruct() { Message = Properties.Resources.P0503040000_AllotStocksSuccess, Title = Resources.Resources.Information });
      return true;
    }
    #endregion 配庫處理


  }
}
