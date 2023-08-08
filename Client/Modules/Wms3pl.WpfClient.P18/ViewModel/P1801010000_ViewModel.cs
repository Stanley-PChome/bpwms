using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P18ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P18.ViewModel
{
  public partial class P1801010000_ViewModel : InputViewModelBase
  {
    private string _gupCode;
    private string _custCode;
    public Action<bool> SetGridVisiablity = delegate { };

    public P1801010000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
        InitControls();
      }
    }

    private void SetSearchControl()
    {
      //結算期間
      if (SearchStockType != "2")
      {
        CloseDateBegin = null;
        CloseDateEnd = null;
      }
      else
      {
        if (CloseDateBegin == null && CloseDateEnd == null)
        {
          CloseDateBegin = DateTime.Today.AddDays(-10);
          CloseDateEnd = DateTime.Today;
        }
      }
      //儲位
      if (SearchStockType != "0")
      {
        LocCodeBegin = string.Empty;
        LocCodeEnd = string.Empty;
        MakeNo = string.Empty;
        BoxCtrlNoBegin = string.Empty;
        BoxCtrlNoEnd = string.Empty;
        PalletCtrlNoBegin = string.Empty;
        PalletCtrlNoEnd = string.Empty;
        IsExpand = "0";
      }
      //倉別
      if (SearchStockType == "2")
      {
        foreach (var w in WarehouseList)
          w.IsSelected = false;
        IsCheckAll = false;
        VnrCode = null;
      }
    }

    private void InitControls()
    {
      GetDcList();
      GetCategoryList();
      GetLTypeList();
      GetMTypeList();
      GetSTypeList();
    }

    #region ComboBox DataSource

    //物流中心
    private List<NameValuePair<string>> _dclist;

    public List<NameValuePair<string>> DcList
    {
      get { return _dclist; }
      set
      {
        _dclist = value;
        RaisePropertyChanged("DcList");
      }
    }

    private void GetDcList()
    {
      DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
      DcCode = DcList.FirstOrDefault().Value;
    }

    //類別
    private List<NameValuePair<string>> _categoryList;

    public List<NameValuePair<string>> CategoryList
    {
      get { return _categoryList; }
      set
      {
        _categoryList = value;
        RaisePropertyChanged("CategoryList");
      }
    }

    private void GetCategoryList()
    {
      CategoryList = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TYPE");
      CategoryList.Insert(0, new NameValuePair<string>(Resources.Resources.All, ""));
      CategoryBegin = CategoryList.FirstOrDefault().Value;
      CategoryEnd = CategoryList.FirstOrDefault().Value;
    }

    //大分類
    private List<NameValuePair<string>> _lTypeList;

    public List<NameValuePair<string>> LTypeList
    {
      get { return _lTypeList; }
      set
      {
        _lTypeList = value;
        RaisePropertyChanged("LTypeList");
      }
    }

    private void GetLTypeList()
    {
      var proxy = GetProxy<F19Entities>();
      LTypeList = (from a in proxy.F1915s
                   where a.CUST_CODE == _custCode
                   select new NameValuePair<string>()
                   {
                     Value = a.ACODE,
                     Name = string.Format("{0} {1}", a.ACODE, a.CLA_NAME)
                   }).ToList();
      LTypeList.Insert(0, new NameValuePair<string>(Resources.Resources.All, ""));
      LTypeBegin = LTypeList.FirstOrDefault().Value;
      LTypeEnd = LTypeList.FirstOrDefault().Value;
    }

    //中分類
    private List<NameValuePair<string>> _mTypeList;

    public List<NameValuePair<string>> MTypeList
    {
      get { return _mTypeList; }
      set
      {
        _mTypeList = value;
        RaisePropertyChanged("MTypeList");
      }
    }

    private void GetMTypeList()
    {
      var proxy = GetProxy<F19Entities>();
      MTypeList = (from a in proxy.F1916s
                   where a.CUST_CODE == _custCode
                   select new NameValuePair<string>()
                   {
                     Value = a.BCODE,
                     Name = string.Format("{0} {1}", a.BCODE, a.CLA_NAME)
                   }).ToList();
      MTypeList.Insert(0, new NameValuePair<string>(Resources.Resources.All, ""));
      MTypeBegin = LTypeList.FirstOrDefault().Value;
      MTypeEnd = LTypeList.FirstOrDefault().Value;
    }

    //小分類
    private List<NameValuePair<string>> _sTypeList;

    public List<NameValuePair<string>> STypeList
    {
      get { return _sTypeList; }
      set
      {
        _sTypeList = value;
        RaisePropertyChanged("STypeList");
      }
    }

    private void GetSTypeList()
    {
      var proxy = GetProxy<F19Entities>();
      STypeList = (from a in proxy.F1917s
                   where a.CUST_CODE == _custCode
                   select new NameValuePair<string>()
                   {
                     Value = a.CCODE,
                     Name = string.Format("{0} {1}", a.CCODE, a.CLA_NAME)
                   }).ToList();
      STypeList.Insert(0, new NameValuePair<string>(Resources.Resources.All, ""));
      STypeBegin = LTypeList.FirstOrDefault().Value;
      STypeEnd = LTypeList.FirstOrDefault().Value;
    }

    #endregion ComboBox DataSource

    #region Grid DataSource

    private ObservableCollection<StockQueryData1> _dgQueryData1;

    public ObservableCollection<StockQueryData1> DgQueryData1
    {
      get { return _dgQueryData1; }
      set
      {
        _dgQueryData1 = value;
        RaisePropertyChanged("DgQueryData1");
      }
    }

    private StockQueryData1 _selectDgQueryData;

    public StockQueryData1 selectDgQueryData
    {
      get { return _selectDgQueryData; }
      set
      {
        _selectDgQueryData = value;
        RaisePropertyChanged("selectDgQueryData");
      }
    }

    private List<StockQueryData1> _listQueryData2;

    public List<StockQueryData1> ListQueryData2
    {
      get { return _listQueryData2; }
      set
      {
        _listQueryData2 = value;
        RaisePropertyChanged("ListQueryData2");
      }
    }

    private DataTable _dtQueryData2;

    public DataTable DtQueryData2
    {
      get
      {
        if (_dtQueryData2 == null)
        {
          _dtQueryData2 = new DataTable();
          _dtQueryData2.Columns.Add(Resources.Resources.ItemNumber);
          _dtQueryData2.Columns.Add(Properties.Resources.CUST_NAME);
          _dtQueryData2.Columns.Add(Properties.Resources.ItemCode);
          _dtQueryData2.Columns.Add(Properties.Resources.ITEM_NAME);
          _dtQueryData2.Columns.Add(Properties.Resources.LTypeBegin);
          _dtQueryData2.Columns.Add(Properties.Resources.MTypeBegin);
          _dtQueryData2.Columns.Add(Properties.Resources.P1801010000_STYPE_NAME);
          _dtQueryData2.Columns.Add(Properties.Resources.ITEM_SPEC);
          _dtQueryData2.Columns.Add(Properties.Resources.ITEM_SIZE);
          _dtQueryData2.Columns.Add(Properties.Resources.ITEM_COLOR);
          _dtQueryData2.Columns.Add("廠商編號");
          _dtQueryData2.Columns.Add(Properties.Resources.P1801010000_LimitQty);
          _dtQueryData2.Columns.Add(Properties.Resources.P1801010000_TotalInventory);
        }
        return _dtQueryData2;
      }
      set
      {
        _dtQueryData2 = value;
        RaisePropertyChanged("DtQueryData2");
      }
    }

    private ObservableCollection<StockQueryData3> _dgQueryData3;

    public ObservableCollection<StockQueryData3> DgQueryData3
    {
      get { return _dgQueryData3; }
      set
      {
        _dgQueryData3 = value;
        RaisePropertyChanged("DgQueryData3");
      }
    }

    #endregion Grid DataSource

    #region 倉別

    private SelectionList<NameValuePair<string>> _warehouseList;

    public SelectionList<NameValuePair<string>> WarehouseList
    {
      get { return _warehouseList; }
      set
      {
        _warehouseList = value;
        RaisePropertyChanged("WarehouseList");
      }
    }

    //public List<F1912> GetF1912List()
    //{
    //	var proxy = GetProxy<F19Entities>();
    //	return proxy.F1912s.Where(b => (b.CUST_CODE == _custCode && b.GUP_CODE == _gupCode)
    //	|| (b.CUST_CODE == _custCode && b.GUP_CODE == "0")
    //	|| (b.CUST_CODE == "0" && b.GUP_CODE == _gupCode) 
    //	|| (b.CUST_CODE == "0" && b.GUP_CODE == "0")).ToList();
    //}

    //public List<F1980> GetF1980List()
    //{
    //	var proxy = GetProxy<F19Entities>();
    //	return proxy.F1980s.Where(a => a.DC_CODE == DcCode).ToList();
    //}

    private void SetWarehouseList()
    {
      var proxyEx = GetExProxy<P18ExDataSource>();
      var tmpData = proxyEx.CreateQuery<F1912WareHouseData>("GetWarehouseDatas")
        .AddQueryExOption("dcCode", DcCode)
        .AddQueryExOption("gupCode", _gupCode)
        .AddQueryExOption("custCode", _custCode).ToList();
      WarehouseList = (from a in tmpData
                       select new NameValuePair<string>
                       { Value = a.WAREHOUSE_ID, Name = a.WAREHOUSE_NAME }).ToSelectionList();
    }

    private bool _isCheckAll;

    public bool IsCheckAll
    {
      get { return _isCheckAll; }
      set
      {
        _isCheckAll = value;
        RaisePropertyChanged("IsCheckAll");
      }
    }

    #endregion 倉別

    #region CheckAll

    public ICommand CheckAllCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoCheckAllItem()
        );
      }
    }

    public void DoCheckAllItem()
    {
      foreach (var p in WarehouseList)
      {
        p.IsSelected = IsCheckAll;
      }
    }

    #endregion CheckAll

    #region 查詢條件

    //物流中心
    private string _dcCode;

    public string DcCode
    {
      get { return _dcCode; }
      set
      {
        Set(() => DcCode, ref _dcCode, value);

        if (value != null)
        {
          SetWarehouseList();
          DgQueryData1 = null;
          DtQueryData2 = null;
          DgQueryData3 = null;
          IsCheckAll = false;
        }
      }
    }

    //SearchStockType:0-儲位儲存,1-商品各倉儲存,2-商品迴轉率
    private string _searchStockType = "0";

    public string SearchStockType
    {
      get { return _searchStockType; }
      set
      {
        _searchStockType = value;
        SetSearchControl();

        RaisePropertyChanged("SearchStockType");
      }
    }

    //類別
    private string _categoryBegin;

    public string CategoryBegin
    {
      get { return _categoryBegin; }
      set
      {
        _categoryBegin = value;
        RaisePropertyChanged("CategoryBegin");
      }
    }

    private string _categoryEnd;

    public string CategoryEnd
    {
      get { return _categoryEnd; }
      set
      {
        _categoryEnd = value;
        RaisePropertyChanged("CategoryEnd");
      }
    }

    //大分類
    private string _lTypeBegin;

    public string LTypeBegin
    {
      get { return _lTypeBegin; }
      set
      {
        _lTypeBegin = value;
        RaisePropertyChanged("LTypeBegin");
      }
    }

    private string _lTypeEnd;

    public string LTypeEnd
    {
      get { return _lTypeEnd; }
      set
      {
        _lTypeEnd = value;
        RaisePropertyChanged("LTypeEnd");
      }
    }

    //中分類
    private string _mTypeBegin;

    public string MTypeBegin
    {
      get { return _mTypeBegin; }
      set
      {
        _mTypeBegin = value;
        RaisePropertyChanged("MTypeBegin");
      }
    }

    private string _mTypeEnd;

    public string MTypeEnd
    {
      get { return _mTypeEnd; }
      set
      {
        _mTypeEnd = value;
        RaisePropertyChanged("MTypeEnd");
      }
    }

    //小分類
    private string _sTypeBegin;

    public string STypeBegin
    {
      get { return _sTypeBegin; }
      set
      {
        _sTypeBegin = value;
        RaisePropertyChanged("STypeBegin");
      }
    }

    private string _sTypeEnd;

    public string STypeEnd
    {
      get { return _sTypeEnd; }
      set
      {
        _sTypeEnd = value;
        RaisePropertyChanged("STypeEnd");
      }
    }

    //入庫期間
    private DateTime? _enterDateBegin;

    public DateTime? EnterDateBegin
    {
      get { return _enterDateBegin; }
      set
      {
        _enterDateBegin = value;
        RaisePropertyChanged("EnterDateBegin");
      }
    }

    private DateTime? _enterDateEnd;

    public DateTime? EnterDateEnd
    {
      get { return _enterDateEnd; }
      set
      {
        _enterDateEnd = value;
        RaisePropertyChanged("EnterDateEnd");
      }
    }

    //商品效期
    private DateTime? _validDateBegin;

    public DateTime? ValidDateBegin
    {
      get { return _validDateBegin; }
      set
      {
        _validDateBegin = value;
        RaisePropertyChanged("ValidDateBegin");
      }
    }

    private DateTime? _validDateEnd;

    public DateTime? ValidDateEnd
    {
      get { return _validDateEnd; }
      set
      {
        _validDateEnd = value;
        RaisePropertyChanged("ValidDateEnd");
      }
    }

    //結算期間
    private DateTime? _closeDateBegin;

    public DateTime? CloseDateBegin
    {
      get { return _closeDateBegin; }
      set
      {
        _closeDateBegin = value;
        RaisePropertyChanged("CloseDateBegin");
      }
    }

    private DateTime? _closeDateEnd;

    public DateTime? CloseDateEnd
    {
      get { return _closeDateEnd; }
      set
      {
        _closeDateEnd = value;
        RaisePropertyChanged("CloseDateEnd");
      }
    }

    //儲位
    private string _locCodeBegin = string.Empty;

    public string LocCodeBegin
    {
      get { return _locCodeBegin; }
      set
      {
        _locCodeBegin = value;
        RaisePropertyChanged("LocCodeBegin");
      }
    }

    private string _locCodeEnd = string.Empty;

    public string LocCodeEnd
    {
      get { return _locCodeEnd; }
      set
      {
        _locCodeEnd = value;
        RaisePropertyChanged("LocCodeEnd");
      }
    }

    //品號
    private string _itemCode = string.Empty;

    public string ItemCode
    {
      get { return _itemCode; }
      set
      {
        _itemCode = value;
        RaisePropertyChanged("ItemCode");
      }
    }

    //批號
    private string _makeNo = string.Empty;

    public string MakeNo
    {
      get { return _makeNo; }
      set
      {
        _makeNo = value;
        RaisePropertyChanged("MakeNo");
      }
    }

    //箱號頭
    private string _boxCtrlNoBegin = string.Empty;
    /// <summary>
    /// 起使箱號(已被廠商編號取代，不再使用的屬性)
    /// </summary>
    public string BoxCtrlNoBegin
    {
      get { return _boxCtrlNoBegin; }
      set
      {
        _boxCtrlNoBegin = value;
        RaisePropertyChanged("BoxCtrlNoBegin");
      }
    }

    //箱號尾
    private string _boxCtrlNoEnd = string.Empty;
    /// <summary>
    /// 結束箱號(已被廠商編號取代，不再使用的屬性)
    /// </summary>
    public string BoxCtrlNoEnd
    {
      get { return _boxCtrlNoEnd; }
      set
      {
        _boxCtrlNoEnd = value;
        RaisePropertyChanged("BoxCtrlNoEnd");
      }
    }

    private string _vnrCode;
    /// <summary>
    /// 廠商編號
    /// </summary>
    public string VnrCode
    {
      get { return _vnrCode; }
      set { Set(() => VnrCode, ref _vnrCode, value); }
    }

    //板號頭
    private string _palletCtrlNoBegin = string.Empty;

    public string PalletCtrlNoBegin
    {
      get { return _palletCtrlNoBegin; }
      set
      {
        _palletCtrlNoBegin = value;
        RaisePropertyChanged("PalletCtrlNoBegin");
      }
    }

    //板號尾
    private string _palletCtrlNoEnd = string.Empty;

    public string PalletCtrlNoEnd
    {
      get { return _palletCtrlNoEnd; }
      set
      {
        _palletCtrlNoEnd = value;
        RaisePropertyChanged("PalletCtrlNoEnd");
      }
    }

    //序號管理
    private string _isBoundleSerialNo;

    public string IsBoundleSerialNo
    {
      get { return _isBoundleSerialNo; }
      set
      {
        _isBoundleSerialNo = value;
        RaisePropertyChanged("IsBoundleSerialNo");
      }
    }

    //序號綁儲位
    private string _isBoundleSerialLoc;

    public string IsBoundleSerialLoc
    {
      get { return _isBoundleSerialLoc; }
      set
      {
        _isBoundleSerialLoc = value;
        RaisePropertyChanged("IsBoundleSerialLoc");
      }
    }

    //組合商品
    private string _isMultiFlag;

    public string IsMultiFlag
    {
      get { return _isMultiFlag; }
      set
      {
        _isMultiFlag = value;
        RaisePropertyChanged("IsMultiFlag");
      }
    }

    //流通加工
    private string _isPickWareW;

    public string IsPickWareW
    {
      get { return _isPickWareW; }
      set
      {
        _isPickWareW = value;
        RaisePropertyChanged("IsPickWareW");
      }
    }

    //虛擬商品
    private string _isVirtualType;

    public string IsVirtualType
    {
      get { return _isVirtualType; }
      set
      {
        _isVirtualType = value;
        RaisePropertyChanged("IsVirtualType");
      }
    }

    //效期與入庫日展開
    private string _isExpand;

    public string IsExpand
    {
      get { return _isExpand; }
      set
      {
        _isExpand = value;
        RaisePropertyChanged("IsExpand");
      }
    }

    #endregion 查詢條件

    #region 效期Enable

    private bool _isEnable;

    public bool isEnable
    {
      get { return _isEnable; }
      set
      {
        _isEnable = value;
        RaisePropertyChanged("isEnable");
      }
    }

    #endregion 效期Enable

    #region Search

    private bool IsSearchChecked { get; set; }

    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSearch(),
          () => UserOperateMode == OperateMode.Query,
          o => DoSearchComplete(),
          null,
          CheckSearchData
          );
      }
    }

    private void CheckSearchData()
    {
      IsSearchChecked = false;
      if (SearchStockType == "2")
      {
        if (CloseDateBegin == null || CloseDateEnd == null)
        {
          DialogService.ShowMessage(Properties.Resources.P1801010000_CloseDateEmpty);
          return;
        }
        if (CloseDateEnd.Value < CloseDateBegin.Value)
        {
          DialogService.ShowMessage(Properties.Resources.P1801010000_CloseDateInvalid);
          return;
        }
      }

      if (
        !((!string.IsNullOrEmpty(CategoryBegin) && !string.IsNullOrEmpty(CategoryEnd))
        || (!string.IsNullOrEmpty(LTypeBegin) && !string.IsNullOrEmpty(LTypeEnd))
        || (!string.IsNullOrEmpty(MTypeBegin) && !string.IsNullOrEmpty(MTypeEnd))
        || (!string.IsNullOrEmpty(STypeBegin) && !string.IsNullOrEmpty(STypeEnd))
        || (!string.IsNullOrEmpty(ItemCode))
                || (!string.IsNullOrEmpty(MakeNo))
                || (!string.IsNullOrEmpty(LocCodeBegin) && !string.IsNullOrEmpty(LocCodeEnd))
        ))
      {
        DialogService.ShowMessage(Properties.Resources.P1801010000_CategoryTypeItemCodeLocCodeIsNullOrEmpty);
        return;
      }

      // 將儲位改為9碼
      LocCodeBegin = LocCodeHelper.LocCodeConverter9(LocCodeBegin);
      LocCodeEnd = LocCodeHelper.LocCodeConverter9(LocCodeEnd);
      IsSearchChecked = true;
    }

		private void SetQueryData2()
		{
			var items = (from p in ListQueryData2
						 group p by new
						 {
							 p.CUST_CODE,
							 p.CUST_NAME,
							 p.ITEM_CODE,
							 p.ITEM_NAME,
							 p.LTYPE,
							 p.LTYPE_NAME,
							 p.MTYPE,
							 p.MTYPE_NAME,
							 p.STYPE,
							 p.STYPE_NAME,
							 p.ITEM_SPEC,
							 p.ITEM_SIZE,
							 p.ITEM_COLOR,
							 p.VNR_CODE,
							 p.RESERVATION_QTY,
							 p.PICK_SUM_QTY,
							 p.ALLOCATION_SUM_QTY,
							 p.PROCESS_PICK_SUM_QTY,
							 p.VIRTUAL_STOCK_QTY,
							 p.A7_PRE_TAR_QTY
						 }
							 into g
						 select new StockQueryData1
						 {
							 CUST_CODE = g.Key.CUST_CODE,
							 CUST_NAME = g.Key.CUST_NAME,
							 ITEM_CODE = g.Key.ITEM_CODE,
							 ITEM_NAME = g.Key.ITEM_NAME,
							 VNR_CODE = g.Key.VNR_CODE,
							 LTYPE = g.Key.LTYPE,
							 LTYPE_NAME = g.Key.LTYPE_NAME,
							 MTYPE = g.Key.MTYPE,
							 MTYPE_NAME = g.Key.MTYPE_NAME,
							 STYPE = g.Key.STYPE,
							 STYPE_NAME = g.Key.STYPE_NAME,
							 ITEM_SPEC = g.Key.ITEM_SPEC,
							 ITEM_SIZE = g.Key.ITEM_SIZE,
							 ITEM_COLOR = g.Key.ITEM_COLOR,
							 RESERVATION_QTY = g.Key.RESERVATION_QTY,
							 PICK_SUM_QTY = g.Key.PICK_SUM_QTY,
							 ALLOCATION_SUM_QTY = g.Key.ALLOCATION_SUM_QTY,
							 PROCESS_PICK_SUM_QTY = g.Key.PROCESS_PICK_SUM_QTY,
							 VIRTUAL_STOCK_QTY = g.Key.VIRTUAL_STOCK_QTY,
							 A7_PRE_TAR_QTY = g.Key.A7_PRE_TAR_QTY
						 }).ToList();

			var dt = new DataTable();
			dt.Columns.Add(Resources.Resources.ItemNumber);
			dt.Columns.Add(Properties.Resources.CUST_NAME);
			dt.Columns.Add(Properties.Resources.ItemCode);
			dt.Columns.Add(Properties.Resources.ITEM_NAME);
			dt.Columns.Add(Properties.Resources.LTypeBegin);
			dt.Columns.Add(Properties.Resources.MTypeBegin);
			dt.Columns.Add(Properties.Resources.P1801010000_STYPE_NAME);
			dt.Columns.Add(Properties.Resources.ITEM_SPEC);
			dt.Columns.Add(Properties.Resources.ITEM_SIZE);
			dt.Columns.Add(Properties.Resources.ITEM_COLOR);
			dt.Columns.Add("廠商編號");
			dt.Columns.Add(Properties.Resources.P1801010000_LimitQty);
			dt.Columns.Add(Properties.Resources.P1801010000_RESERVATION_QTY);
			dt.Columns.Add(Properties.Resources.P1801010000_TotalInventory);
			dt.Columns.Add(Properties.Resources.P1801010000_PickingQty);
			foreach (var w in WarehouseList)
				dt.Columns.Add(w.Item.Name.Replace(".", "\x2024")); //DataGrid Header有特殊符號會使下方資料顯示不出來，將"."轉Unicode即正常顯示
			dt.Columns.Add(Properties.Resources.P1801010000_PICK_SUM_QTY);
			dt.Columns.Add(Properties.Resources.P1801010000_ALLOCATION_SUM_QTY);
			dt.Columns.Add(Properties.Resources.P1801010000_PROCESS_PICK_SUM_QTY);
			//dt.Columns.Add(Properties.Resources.P1801010000_LEND_QTY);
			dt.Columns.Add("待虛擬庫存回復數量");
			dt.Columns.Add("A7補貨倉待上架數量");

			var index = 1;
			foreach (var d in items)
			{
				DataRow row = dt.NewRow();
				row[0] = index;
				row[1] = d.CUST_NAME;
				row[2] = d.ITEM_CODE;
				row[3] = d.ITEM_NAME;
				row[4] = d.LTYPE_NAME;
				row[5] = d.MTYPE_NAME;
				row[6] = d.STYPE_NAME;
				row[7] = d.ITEM_SPEC;
				row[8] = d.ITEM_SIZE;
				row[9] = d.ITEM_COLOR;
				row[10] = d.VNR_CODE;

				// 管制數
				var LimitQty = ListQueryData2.Where(p => p.ITEM_CODE == d.ITEM_CODE && new[] { "02", "03", "04" }.Contains(p.NOW_STATUS_ID)).Sum(p => p.QTY);
				row[11] = LimitQty;

				// 預約數
				row[12] = d.RESERVATION_QTY;
				// 總庫存數
				row[13] = ListQueryData2.Where(p => p.ITEM_CODE == d.ITEM_CODE).Sum(p => p.QTY);
				// 可揀數 = 庫存總數 (良品倉總庫存+加工倉總庫存) - 管制數
				//no1213 可揀數計算排除不可出貨數量(只計算G和W倉)
				var PickingQty = ListQueryData2.Where(p => p.ITEM_CODE == d.ITEM_CODE && new string[] { "G", "W" }.Contains(p.WAREHOUSE_TYPE)).Sum(p => p.QTY);
				row[14] = PickingQty - LimitQty;

				var col = 15;
				foreach (var w in WarehouseList)
				{
					var qty =
					  ListQueryData2.Where(p => p.ITEM_CODE == d.ITEM_CODE && p.WAREHOUSE_ID == w.Item.Value).Sum(p => p.QTY);
					row[col] = (w.IsSelected) ? qty.ToString() : string.Empty;
					col++;
				}

				// 出貨虛擬、調撥虛擬、加工虛擬、借出外送數
				row[col] = d.PICK_SUM_QTY;
				row[col + 1] = d.ALLOCATION_SUM_QTY;
				row[col + 2] = d.PROCESS_PICK_SUM_QTY;
				//row[col + 3] = d.LEND_QTY;

				//待虛擬庫存回復數量、A7補貨倉待上架數量
				row[col + 3] = d.VIRTUAL_STOCK_QTY;
				row[col + 4] = d.A7_PRE_TAR_QTY;

				dt.Rows.Add(row);
				index++;
			}

			DtQueryData2 = dt;
		}

    private void DoSearchComplete()
    {
      if (!IsSearchChecked) return;

      if (SearchStockType == "0")
      {
        if (DgQueryData1 != null && !DgQueryData1.Any())
          ShowMessage(Messages.InfoNoData);
        SetGridVisiablity(IsExpand == "1" || IsBoundleSerialLoc == "1");
      }
      else if (SearchStockType == "1")
      {
        if (ListQueryData2 != null && !ListQueryData2.Any())
          ShowMessage(Messages.InfoNoData);
        else
          SetQueryData2();
      }
      else if (SearchStockType == "2")
      {
        if (DgQueryData3 != null && !DgQueryData3.Any())
          ShowMessage(Messages.InfoNoData);
      }
    }

    private void DoSearch()
    {
      if (!IsSearchChecked) return;

      DgQueryData1 = null;
      DgQueryData3 = null;
      ListQueryData2 = null;
      DtQueryData2 = null;

      var proxyEx = GetExProxy<P18ExDataSource>();

      string errorMsg;
      if (!ValidateSerach(out errorMsg))
      {
        ShowWarningMessage(errorMsg);
        return;
      }

      if (SearchStockType == "0")
      {
        var data = proxyEx.CreateQuery<StockQueryData1>("GetStockQueryData1")
                  .AddQueryExOption("gupCode", _gupCode)
                  .AddQueryExOption("custCode", _custCode)
                  .AddQueryExOption("dcCode", DcCode)
                  .AddQueryExOption("typeBegin", CategoryBegin)
                  .AddQueryExOption("typeEnd", CategoryEnd)
                  .AddQueryExOption("lTypeBegin", LTypeBegin)
                  .AddQueryExOption("lTypeEnd", LTypeEnd)
                  .AddQueryExOption("mTypeBegin", MTypeBegin)
                  .AddQueryExOption("mTypeEnd", MTypeEnd)
                  .AddQueryExOption("sTypeBegin", STypeBegin)
                  .AddQueryExOption("sTypeEnd", STypeEnd)
                  .AddQueryExOption("enterDateBegin", EnterDateBegin)
                  .AddQueryExOption("enterDateEnd", EnterDateEnd)
                  .AddQueryExOption("validDateBegin", ValidDateBegin)
                  .AddQueryExOption("validDateEnd", ValidDateEnd)
                  .AddQueryExOption("locCodeBegin", LocCodeBegin.Replace("-", ""))
                  .AddQueryExOption("locCodeEnd", LocCodeEnd.Replace("-", ""))
                  .AddQueryExOption("itemCodes", string.Join(",", GetSplitContent(ItemCode)))
                  .AddQueryExOption("wareHouseIds", string.Join(",", WarehouseList.Where(p => p.IsSelected).Select(p => p.Item.Value)))
                  .AddQueryExOption("boundleSerialNo", IsBoundleSerialNo)
                  .AddQueryExOption("boundleSerialLoc", IsBoundleSerialLoc)
                  .AddQueryExOption("multiFlag", IsMultiFlag)
                  .AddQueryExOption("packWareW", IsPickWareW)
                  .AddQueryExOption("virtualType", IsVirtualType)
                  .AddQueryExOption("expend", IsExpand)
                  .AddQueryExOption("boxCtrlNoBegin", BoxCtrlNoBegin)
                  .AddQueryExOption("boxCtrlNoEnd", BoxCtrlNoEnd)
                  .AddQueryExOption("palletCtrlNoBegin", PalletCtrlNoBegin)
                  .AddQueryExOption("palletCtrlNoEnd", PalletCtrlNoEnd)
                  .AddQueryExOption("makeNo", MakeNo)
                  .AddQueryExOption("vnrCode", VnrCode)
                  .ToList();
        bool isTotalVolumeOrUnitNull = false;
        foreach (var d in data)
        {
          d.LOC_CODE = FormatLocCode(d.LOC_CODE);
          if (d.TOTAL_VOLUME == null || d.UNIT == null)
            isTotalVolumeOrUnitNull = true;
        }
        if (isTotalVolumeOrUnitNull)
          ShowWarningMessage(Properties.Resources.P1801010000TotalVolumeOrUnitNull);

        DgQueryData1 = data.ToObservableCollection();
      }
      else if (SearchStockType == "1")
      {
        ListQueryData2 = proxyEx.CreateQuery<StockQueryData1>("GetStockQueryData2")
                  .AddQueryExOption("gupCode", _gupCode)
                  .AddQueryExOption("custCode", _custCode)
                  .AddQueryExOption("dcCode", DcCode)
                  .AddQueryExOption("typeBegin", CategoryBegin)
                  .AddQueryExOption("typeEnd", CategoryEnd)
                  .AddQueryExOption("lTypeBegin", LTypeBegin)
                  .AddQueryExOption("lTypeEnd", LTypeEnd)
                  .AddQueryExOption("mTypeBegin", MTypeBegin)
                  .AddQueryExOption("mTypeEnd", MTypeEnd)
                  .AddQueryExOption("sTypeBegin", STypeBegin)
                  .AddQueryExOption("sTypeEnd", STypeEnd)
                  .AddQueryExOption("enterDateBegin", EnterDateBegin)
                  .AddQueryExOption("enterDateEnd", EnterDateEnd)
                  .AddQueryExOption("validDateBegin", ValidDateBegin)
                  .AddQueryExOption("validDateEnd", ValidDateEnd)
                  .AddQueryExOption("itemCodes", string.Join(",", GetSplitContent(ItemCode)))
                  .AddQueryExOption("wareHouseIds", string.Join(",", WarehouseList.Where(p => p.IsSelected).Select(p => p.Item.Value)))
                  .AddQueryExOption("boundleSerialNo", IsBoundleSerialNo)
                  .AddQueryExOption("boundleSerialLoc", IsBoundleSerialLoc)
                  .AddQueryExOption("multiFlag", IsMultiFlag)
                  .AddQueryExOption("packWareW", IsPickWareW)
                  .AddQueryExOption("virtualType", IsVirtualType)
                  .AddQueryExOption("boxCtrlNoBegin", BoxCtrlNoBegin)
                  .AddQueryExOption("boxCtrlNoEnd", BoxCtrlNoEnd)
                  .AddQueryExOption("palletCtrlNoBegin", PalletCtrlNoBegin)
                  .AddQueryExOption("palletCtrlNoEnd", PalletCtrlNoEnd)
                  .AddQueryExOption("vnrCode", VnrCode)
                  .ToList();
      }
      else if (SearchStockType == "2")
      {
        DgQueryData3 = proxyEx.CreateQuery<StockQueryData3>("GetStockQueryData3")
          .AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
          .AddQueryOption("custCode", string.Format("'{0}'", _custCode))
          .AddQueryOption("dcCode", string.Format("'{0}'", DcCode))
          .AddQueryOption("typeBegin", string.Format("'{0}'", CategoryBegin))
          .AddQueryOption("typeEnd", string.Format("'{0}'", CategoryEnd))
          .AddQueryOption("lTypeBegin", string.Format("'{0}'", LTypeBegin))
          .AddQueryOption("lTypeEnd", string.Format("'{0}'", LTypeEnd))
          .AddQueryOption("mTypeBegin", string.Format("'{0}'", MTypeBegin))
          .AddQueryOption("mTypeEnd", string.Format("'{0}'", MTypeEnd))
          .AddQueryOption("sTypeBegin", string.Format("'{0}'", STypeBegin))
          .AddQueryOption("sTypeEnd", string.Format("'{0}'", STypeEnd))
          .AddQueryOption("enterDateBegin",
          string.Format("'{0}'", (EnterDateBegin == null) ? "" : EnterDateBegin.Value.ToString("yyyy/MM/dd")))
          .AddQueryOption("enterDateEnd",
          string.Format("'{0}'", (EnterDateEnd == null) ? "" : EnterDateEnd.Value.ToString("yyyy/MM/dd")))
          .AddQueryOption("validDateBegin",
          string.Format("'{0}'", (ValidDateBegin == null) ? "" : ValidDateBegin.Value.ToString("yyyy/MM/dd")))
          .AddQueryOption("validDateEnd",
          string.Format("'{0}'", (ValidDateEnd == null) ? "" : ValidDateEnd.Value.ToString("yyyy/MM/dd")))
          .AddQueryOption("closeDateBegin", string.Format("'{0}'", (CloseDateBegin == null) ? "" : CloseDateBegin.Value.ToString("yyyy/MM/dd")))
          .AddQueryOption("closeDateEnd", string.Format("'{0}'", (CloseDateEnd == null) ? "" : CloseDateEnd.Value.ToString("yyyy/MM/dd")))
          .AddQueryOption("itemCodes",
          string.Format("'{0}'",
            string.Join(",", GetSplitContent(ItemCode))))
          .AddQueryOption("boundleSerialNo", string.Format("'{0}'", IsBoundleSerialNo))
          .AddQueryOption("boundleSerialLoc", string.Format("'{0}'", IsBoundleSerialLoc))
          .AddQueryOption("multiFlag", string.Format("'{0}'", IsMultiFlag))
          .AddQueryOption("packWareW", string.Format("'{0}'", IsPickWareW))
          .AddQueryOption("virtualType", string.Format("'{0}'", IsVirtualType))
          .AddQueryOption("boxCtrlNoBegin", BoxCtrlNoBegin)
          .AddQueryOption("boxCtrlNoEnd", BoxCtrlNoEnd)
          .AddQueryOption("palletCtrlNoBegin", PalletCtrlNoBegin)
          .AddQueryOption("palletCtrlNoEnd", PalletCtrlNoEnd)
          .ToObservableCollection();
      }
    }

    private IEnumerable<string> GetSplitContent(string text)
    {
      return text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).Distinct();
    }

    private bool ValidateSerach(out string errorMsg)
    {
      if (!ValidateHelper.TryCheckBeginEnd(this, x => x.EnterDateBegin, x => x.EnterDateEnd, Properties.Resources.ENTER_DATE, out errorMsg))
        return false;

      if (!ValidateHelper.TryCheckBeginEnd(this, x => x.ValidDateBegin, x => x.ValidDateEnd, Properties.Resources.P1801010000_ValidDateEnd, out errorMsg))
        return false;

      if (!ValidateHelper.TryCheckBeginEnd(this, x => x.CloseDateBegin, x => x.CloseDateEnd, Properties.Resources.P1801010000_CloseDateEnd, out errorMsg))
        return false;

      if (SearchStockType == "0" && !ValidateHelper.TryCheckBeginEndForLoc(this, x => x.LocCodeBegin, x => x.LocCodeEnd, Properties.Resources.LocCodeBegin, out errorMsg))
        return false;

      // 類別 大分類 中分類 小分類
      ValidateHelper.AutoChangeBeginEnd(this, x => x.CategoryBegin, x => x.CategoryEnd);
      ValidateHelper.AutoChangeBeginEnd(this, x => x.LTypeBegin, x => x.LTypeEnd);
      ValidateHelper.AutoChangeBeginEnd(this, x => x.MTypeBegin, x => x.MTypeEnd);
      ValidateHelper.AutoChangeBeginEnd(this, x => x.STypeBegin, x => x.STypeEnd);

      return true;
    }

    #endregion Search

    #region Paste

    public ICommand PasteCommand
    {
      get
      {
        return new RelayCommand(
          () =>
          {
            IsBusy = true;
            try
            {
              DoPaste();
            }
            catch (Exception ex)
            {
              Exception = ex;
              IsBusy = false;
            }
            IsBusy = false;
          },
        () => !IsBusy);
      }
    }


    private void DoPaste()
    {
      if (Clipboard.ContainsData(DataFormats.Text))
      {
        var pastData = Clipboard.GetDataObject();
        if (pastData != null && pastData.GetDataPresent(DataFormats.Text))
        {
          var content = pastData.GetData(DataFormats.Text).ToString();
          var arr = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
          ItemCode = string.Join(",", arr.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)));
        }
      }
    }

    /// <summary>
    /// 批號貼上
    /// </summary>
    public ICommand PasteMakeNoCommand
    {
      get
      {
        return new RelayCommand(
          () =>
          {
            IsBusy = true;
            try
            {
              DoPasteMakeNo();
            }
            catch (Exception ex)
            {
              Exception = ex;
              IsBusy = false;
            }
            IsBusy = false;
          },
        () => !IsBusy);
      }
    }

    private void DoPasteMakeNo()
    {
      if (Clipboard.ContainsData(DataFormats.Text))
      {
        var pastData = Clipboard.GetDataObject();
        if (pastData != null && pastData.GetDataPresent(DataFormats.Text))
        {
          var content = pastData.GetData(DataFormats.Text).ToString();
          var arr = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
          MakeNo = string.Join(",", arr.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)));
        }
      }
    }

    #endregion Paste

    public void AppendItemCode(F1903 f1903)
    {
      var itemCodeList = GetSplitContent(ItemCode).ToList();
      itemCodeList.Add(f1903.ITEM_CODE);

      ItemCode = string.Join(",", itemCodeList);
    }

    private string FormatLocCode(string locCode)
    {
      if (locCode.Length != 9)
        return locCode;

      return string.Format("{0}-{1}-{2}-{3}-{4}", locCode.Substring(0, 1), locCode.Substring(1, 2),
        locCode.Substring(3, 2), locCode.Substring(5, 2), locCode.Substring(7, 2));
    }

    #region 效期調整

    public Action OpenValidDate = delegate { };

    public ICommand ValidCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => { },
          () => selectDgQueryData != null &&
                selectDgQueryData.VALID_DATE != DateTime.MinValue && IsExpand == "1",
          o =>
          {
            if (CheckIsEditable())
              OpenValidDate();
          });
      }
    }

    /// <summary>
    /// 取得DB設定看是否可修改效期、批貨、數量
    /// </summary>
    /// <returns></returns>
    private Boolean CheckIsEditable()
    {
      var f00proxy = GetProxy<F00Entities>();
      var f19Proxy = GetProxy<F19Entities>();
      var IsAuto = f19Proxy.F1980s.Where(x => x.DC_CODE == selectDgQueryData.DC_CODE && x.WAREHOUSE_ID == selectDgQueryData.WAREHOUSE_ID).FirstOrDefault().DEVICE_TYPE != "0";

      if (IsAuto)
      {
        if (f00proxy.F0003s.Where(x => x.DC_CODE == selectDgQueryData.DC_CODE && x.AP_NAME == "AllowPersonModifyAutoWareStock").FirstOrDefault().SYS_PATH == "0")
        {
          ShowWarningMessage("自動倉不允許修改效期與批號");
          return false;
        }
      }

      return true;
    }
    #endregion 效期調整
  }
}