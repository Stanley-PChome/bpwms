using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Telerik.Windows.Controls.TransitionControl;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F14DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P14ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P14.ViewModel
{
  public partial class P1401040000_ViewModel : InputViewModelBase
  {
    public Action<PrintType> DoPrint = delegate { };
    public Action<bool> DoExpandFilter = delegate { };
    public Action<bool> DoExpandDetail = delegate { };
    public P1401040000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        InitControls();
      }

    }

    private void InitControls()
    {
      GetDcList();
    }
        private bool? _isWarehouseChecked = true;

        #region 下拉選單資料來源

        #region 物流中心
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
      if (DcList.Any())
        SelectedDcCode = DcList.First().Value;
    }
    #endregion

    #region 倉別
    private List<NameValuePair<string>> _warehouseList;
    public List<NameValuePair<string>> WarehouseList
    {
      get { return _warehouseList; }
      set
      {
        _warehouseList = value;
        RaisePropertyChanged("WarehouseList");
      }
    }

    private void SetWarehouseList(string dcCode)
    {
      var proxy = GetProxy<F19Entities>();
      WarehouseList = (from a in proxy.F1980s
                       where a.DC_CODE == dcCode
                       select new NameValuePair<string>()
                       {
                         Value = a.WAREHOUSE_ID,
                         Name = a.WAREHOUSE_NAME
                       }).ToList();
      WarehouseList.Insert(0, new NameValuePair<string>(Resources.Resources.All, ""));
      SelectedWarehouseId = WarehouseList.First().Value;
    }
    #endregion

    #endregion

    #region 查詢條件

    private string _selectedDcCode;
    public string SelectedDcCode
    {
      get { return _selectedDcCode; }
      set
      {
        if (_selectedDcCode == value) return;
        Set(() => SelectedDcCode, ref _selectedDcCode, value);
      }
    }

    //過帳日期起
    private DateTime _postingDateBegin = DateTime.Today;
    public DateTime PostingDateBegin
    {
      get { return _postingDateBegin; }
      set
      {
        if (_postingDateBegin == value) return;
        Set(() => PostingDateBegin, ref _postingDateBegin, value);
      }
    }

    //過帳日期迄
    private DateTime _postingDateEnd = DateTime.Today;
    public DateTime PostingDateEnd
    {
      get { return _postingDateEnd; }
      set
      {
        if (_postingDateEnd == value) return;
        Set(() => PostingDateEnd, ref _postingDateEnd, value);
      }
    }

    #endregion

    #region 篩選條件
    //0: 指定倉別 1:指定商品
    private string _filterType = "0";

    public string FilterType
    {
      get { return _filterType; }
      set
      {
        if (_filterType == value)
          return;
        if(value == "0")
                {
                    ClearSearchProduct();
                    FilterProductList = null;
                }
                else
                {
                    SelectedWarehouseId = WarehouseList.First().Value;
                }
        Set(() => FilterType, ref _filterType, value);
      }
    }

    private string _selectedWarehouseId;
    public string SelectedWarehouseId
    {
      get { return _selectedWarehouseId; }
      set
      {
        if (_selectedWarehouseId == value)
          return;
        Set(() => SelectedWarehouseId, ref _selectedWarehouseId, value);
      }
    }

    private SelectionList<F1903> _filterProductList;
    public SelectionList<F1903> FilterProductList
    {
      get { return _filterProductList; }
      set
      {
        if (_filterProductList == value)
          return;
        Set(() => FilterProductList, ref _filterProductList, value);
      }
    }

    #region 商品搜尋用欄位
    private string _itemCode;
    public string ITEM_CODE
    {
      get { return _itemCode; }
      set
      {
        _itemCode = value;
        RaisePropertyChanged("ITEM_CODE");
      }
    }
    private string _itemName;
    public string ITEM_NAME
    {
      get { return _itemName; }
      set
      {
        _itemName = value;
        RaisePropertyChanged("ITEM_NAME");
      }
    }
    private string _itemSize;
    public string ITEM_SIZE
    {
      get { return _itemSize; }
      set
      {
        _itemSize = value;
        RaisePropertyChanged("ITEM_SIZE");
      }
    }
    private string _itemSpec;
    public string ITEM_SPEC
    {
      get { return _itemSpec; }
      set
      {
        _itemSpec = value;
        RaisePropertyChanged("ITEM_SPEC");
      }
    }
    private string _itemColor;
    public string ITEM_COLOR
    {
      get { return _itemColor; }
      set
      {
        _itemColor = value;
        RaisePropertyChanged("ITEM_COLOR");
      }
    }

        private string _serialNo;
        public string SERIAL_NO
        {
            get { return _serialNo; }
            set
            {
                _serialNo = value;
                RaisePropertyChanged("SERIAL_NO");
            }
        }

    private bool _hasItem;

    public bool HasItem
    {
      get { return _hasItem; }
      set
      {
        Set(() => HasItem, ref _hasItem, value);
      }
    }

    #endregion

    //依差異數排序
    private bool _sortByCount;
    public bool SortByCount
    {
      get { return _sortByCount; }
      set
      {
        if (_sortByCount == value)
          return;
        Set(() => SortByCount, ref _sortByCount, value);
      }
    }

    #endregion

    //查詢結果
    private List<F140101> _records;

    public List<F140101> Records
    {
      get { return _records; }
      set
      {
        if (_records == value)
          return;
        Set(() => Records, ref _records, value);
      }
    }

    private F140101 _selectedRecord;

    public F140101 SelectedRecord
    {
      get { return _selectedRecord; }
      set
      {
        _selectedRecord = value;
        SetFilter();
        RaisePropertyChanged("SelectedRecord");
      }
    }

    private void SetFilter()
    {
      FilterType = "0";
      DetailRecords = null;

      if (_selectedRecord == null) return;

      SetWarehouseList(_selectedRecord.DC_CODE);
      DoExpandFilter(true);
    }

    #region Search
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => DoSearchComplete()
            );
      }
    }

    private void DoSearchComplete()
    {
      if (UserOperateMode == OperateMode.Query && Records != null && !Records.Any())
        ShowMessage(Messages.InfoNoData);
      else
        SelectedRecord = Records.First();
    }

    private void DoSearch()
    {
      SelectedRecord = null;
	    PostingDateEnd = PostingDateEnd.AddDays(1);
      var proxy = GetProxy<F14Entities>();
      Records = (from p in proxy.F140101s
                 where p.DC_CODE == SelectedDcCode
                       && p.POSTING_DATE >= PostingDateBegin && p.POSTING_DATE < PostingDateEnd
                       && p.STATUS == "5"
                 select p).ToList();
      if (!Records.Any())
        return;
    }
    #endregion Search


    #region CheckAll
    private bool _isCheckAll;
    public bool IsCheckAll
    {
      get { return _isCheckAll; }
      set
      {
        _isCheckAll = value;
        CheckSelectedAll(_isCheckAll);
        RaisePropertyChanged("IsCheckAll");
      }
    }

    public void CheckSelectedAll(bool isChecked)
    {
      foreach (var p in FilterProductList)
      {
        p.IsSelected = isChecked;
      }
    }

    #endregion

    #region AddItemCommand
    public ICommand AddItemCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => { },
          () => HasItem && !string.IsNullOrWhiteSpace(ITEM_CODE),
          o => { },
          null,
          DoAddItem
          );
      }
    }


    private void DoAddItem()
    {
      if (FilterProductList == null) FilterProductList = new SelectionList<F1903>(new List<F1903>(), false);

      if (FilterProductList.Any(x => x.Item.ITEM_CODE.Equals(ITEM_CODE)))
      {
        DialogService.ShowMessage(Properties.Resources.P1401040000_ItemCodeIsExist);
        return;
      }

      var f1903 = new F1903
      {
        ITEM_CODE = ITEM_CODE,
        ITEM_NAME = ITEM_NAME,
        ITEM_SIZE = ITEM_SIZE,
        ITEM_SPEC = ITEM_SPEC,
        ITEM_COLOR = ITEM_COLOR
      };

      FilterProductList.Add(new SelectionItem<F1903, bool>(f1903, false));
            ClearSearchProduct();
    }
    #endregion AddItemCommand

    #region DeleteItemCommand
    public ICommand DeleteItemCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => { },
          () => FilterProductList != null && FilterProductList.Any(x => x.IsSelected),
          o => { },
          null,
          DoDeleteItem
          );
      }
    }


    private void DoDeleteItem()
    {
      if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
      {
        if (!FilterProductList.Any(x => x.IsSelected))
        {
          DialogService.ShowMessage(Properties.Resources.P1401040000_SelectFilterProductList);
          return;
        }

        var selectedItems = FilterProductList.Where(p => !p.IsSelected).Select(p => p.Item);
        FilterProductList = new SelectionList<F1903>(selectedItems);

        ShowMessage(Messages.InfoDeleteSuccess);
      }
    }
    #endregion DeleteItemCommand


    private List<InventoryQueryDataForDc> _detailRecords;

    public List<InventoryQueryDataForDc> DetailRecords
    {
      get { return _detailRecords; }
      set
      {
        if (_detailRecords == value)
          return;
        Set(() => DetailRecords, ref _detailRecords, value);
      }
    }

    #region SearchDetail
    public ICommand SearchDetailCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoSearchDetail(),
            () => UserOperateMode == OperateMode.Query && SelectedRecord != null,
            o => DoSearchDetailComplete()
            );
      }
    }

    private void DoSearchDetailComplete()
    {
      if (UserOperateMode == OperateMode.Query && DetailRecords != null && !DetailRecords.Any())
        ShowMessage(Messages.InfoNoData);

      DoExpandDetail(true);
    }

    private void DoSearchDetail()
    {
      var proxyEx = GetExProxy<P14ExDataSource>();
      DetailRecords = proxyEx.CreateQuery<InventoryQueryDataForDc>("GetInventoryQueryDatasForDc")
        .AddQueryOption("gupCode", string.Format("'{0}'", SelectedRecord.GUP_CODE))
        .AddQueryOption("custCode", string.Format("'{0}'", SelectedRecord.CUST_CODE))
        .AddQueryOption("dcCode", string.Format("'{0}'", SelectedRecord.DC_CODE))
        .AddQueryOption("inventoryNo", string.Format("'{0}'", SelectedRecord.INVENTORY_NO))
        .AddQueryOption("sortByCount", string.Format("'{0}'", SortByCount ? "1" : "0"))
        .AddQueryOption("warehouseId",
          string.Format("'{0}'", (FilterType == "0") ? ((WarehouseList == null) ? "" : SelectedWarehouseId) : ""))
        .AddQueryOption("itemCodes",
          string.Format("'{0}'",
            (FilterType == "1")
              ? ((FilterProductList == null) ? "" : string.Join(",", FilterProductList.Select(p => p.Item.ITEM_CODE)))
              : ""))
        .ToList();
    }
    #endregion SearchDetail

    #region Print
    public ICommand PrintCommand
    {
      get
      {
        return new RelayCommand<PrintType>(
          (t) =>
          {
            IsBusy = true;
            try
            {
              DoPrint(t);
            }
            catch (Exception ex)
            {
              Exception = ex;
              IsBusy = false;
            }
            IsBusy = false;
          },
        (t) => !IsBusy && UserOperateMode == OperateMode.Query && DetailRecords != null && DetailRecords.Count > 0);
      }
    }

        #endregion Print

        private void ClearSearchProduct()
        {
            ITEM_CODE = string.Empty;
            ITEM_COLOR = string.Empty;
            ITEM_NAME = string.Empty;
            ITEM_SPEC = string.Empty;
            ITEM_SIZE = string.Empty;
            SERIAL_NO = string.Empty;
        }
    }
}
