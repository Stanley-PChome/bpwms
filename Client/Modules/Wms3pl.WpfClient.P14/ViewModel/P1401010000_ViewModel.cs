using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F14DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P14ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P14ExDataService.ExecuteResult;
using wcf = Wms3pl.WpfClient.ExDataServices.P14WcfService;

namespace Wms3pl.WpfClient.P14.ViewModel
{
	public partial class P1401010000_ViewModel : InputViewModelBase
	{
		public Action<string> ExcelImportItem = delegate { };
		public Action ExcelImportInventoryDetailItem = delegate { };
		public P1401010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetStatusList();
				SetInventoryTypeList();
				SetInventoryYearList();
				SetWareHouseTypeList();
				SetCheckTool();

				SetAllFloorDictionary();
				SetAllChannelDictionary();
				SetAllPlainDictionary();

				SetItemTypeList();
				SetItemLTypeList();
				//SetItemMTypeList();
				//SetItemSTypeList();
				SetReportTypeList();
        SetProcTypeList();
        QueryInventorySDate = DateTime.Today;
				QueryInventoryEDate = DateTime.Today;
				ImportLocCode = "";
				ImportType = ImportType.Excel;
				MakeList = GetBaseTableService.GetF000904List(FunctionCode, "F140111", "MASK_CODE");
				IsCheckAll = false;
				IsNotUsing = true;
			}
		}

		#region Property

		public Action DgScorllInView = delegate { };
		public Action<PrintType> DoReportShow = delegate { };

		#region 新增、編輯欄位顯示

		private string _showColumn = DisplayType.notDisplay.ToString();

		public string ShowColumn
		{
			get { return _showColumn; }
			set
			{
				if (_showColumn == value)
					return;
				Set(() => ShowColumn, ref _showColumn, value);
			}
		}

		#endregion

		#region 物流中心
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				if (_dcList == value)
					return;
				Set(() => DcList, ref _dcList, value);
			}
		}

		private string _selectedQueryDcCode;

		public string SelectedQueryDcCode
		{
			get { return _selectedQueryDcCode; }
			set
			{
				if (_selectedQueryDcCode == value)
					return;
				Set(() => SelectedQueryDcCode, ref _selectedQueryDcCode, value);
			}
		}

		#endregion

		#region 業主
		private string GupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		#endregion

		#region 貨主
		public string CustCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		#endregion

		#region 顯示/隱藏查詢條件
		private bool _showQueryRule = true;

		public bool ShowQueryRule
		{
			get { return _showQueryRule; }
			set
			{
				if (_showQueryRule == value)
					return;
				Set(() => ShowQueryRule, ref _showQueryRule, value);
			}
		}
		#endregion

		#region 盤點類型
		private List<NameValuePair<string>> _inventoryTypeList;

		public List<NameValuePair<string>> InventoryTypeList
		{
			get { return _inventoryTypeList; }
			set
			{
				if (_inventoryTypeList == value)
					return;
				Set(() => InventoryTypeList, ref _inventoryTypeList, value);
			}
		}

		#endregion


		#region 年
		private List<NameValuePair<string>> _inventoryYearList;

		public List<NameValuePair<string>> InventoryYearList
		{
			get { return _inventoryYearList; }
			set
			{
				if (_inventoryYearList == value)
					return;
				Set(() => InventoryYearList, ref _inventoryYearList, value);
			}
		}
		#endregion

		#region 月
		private List<NameValuePair<string>> _inventoryMonthList;

		public List<NameValuePair<string>> InventoryMonthList
		{
			get { return _inventoryMonthList; }
			set
			{
				if (_inventoryMonthList == value)
					return;
				Set(() => InventoryMonthList, ref _inventoryMonthList, value);
			}
		}
		#endregion


		#region 盤點單報表
		private List<InventoryDetailItemsByIsSecond> _reportDataList;

		public List<InventoryDetailItemsByIsSecond> ReportDataList
		{
			get { return _reportDataList; }
			set
			{
				if (_reportDataList == value)
					return;
				Set(() => ReportDataList, ref _reportDataList, value);
			}
		}
		#endregion

		#region 盤點清冊
		private List<InventoryByLocDetail> _reportDataList2;

		public List<InventoryByLocDetail> ReportDataList2
		{
			get { return _reportDataList2; }
			set
			{
				if (_reportDataList2 == value)
					return;
				Set(() => ReportDataList2, ref _reportDataList2, value);
			}
		}

		#endregion


		#region 報表種類
		private List<NameValuePair<string>> _reportTypeList;

		public List<NameValuePair<string>> ReportTypeList
		{
			get { return _reportTypeList; }
			set
			{
				if (_reportTypeList == value)
					return;
				Set(() => ReportTypeList, ref _reportTypeList, value);
			}
		}
		#endregion


		#region
		private string _selectedReportType;

		public string SelectedReportType
		{
			get { return _selectedReportType; }
			set
			{
				if (_selectedReportType == value)
					return;
				Set(() => SelectedReportType, ref _selectedReportType, value);
			}
		}
		#endregion

		private bool _isSave = true;

		#region 查詢

		#region 盤點日期-起
		private DateTime? _queryInventorySDate;

		public DateTime? QueryInventorySDate
		{
			get { return _queryInventorySDate; }
			set
			{
				if (_queryInventorySDate == value)
					return;
				Set(() => QueryInventorySDate, ref _queryInventorySDate, value);
			}
		}
		#endregion

		#region 盤點日期-迄
		private DateTime? _queryInventoryEDate;

		public DateTime? QueryInventoryEDate
		{
			get { return _queryInventoryEDate; }
			set
			{
				if (_queryInventoryEDate == value)
					return;
				Set(() => QueryInventoryEDate, ref _queryInventoryEDate, value);
			}
		}
		#endregion

		#region 盤點單號
		private string _queryInventoryNo;

		public string QueryInventoryNo
		{
			get { return _queryInventoryNo; }
			set
			{
				if (_queryInventoryNo == value)
					return;
				Set(() => QueryInventoryNo, ref _queryInventoryNo, value);
			}
		}
		#endregion

		#region 單據狀態
		private List<NameValuePair<string>> _statusList;

		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				if (_statusList == value)
					return;
				Set(() => StatusList, ref _statusList, value);
			}
		}

		private string _selectedStatus;

		public string SelectedStatus
		{
			get { return _selectedStatus; }
			set
			{
				if (_selectedStatus == value)
					return;
				Set(() => SelectedStatus, ref _selectedStatus, value);
			}
		}

		#endregion

		#region Grid 查詢結果
		private List<F140101Expansion> _f140101List;

		public List<F140101Expansion> F140101List
		{
			get { return _f140101List; }
			set
			{
				if (_f140101List == value)
					return;
				Set(() => F140101List, ref _f140101List, value);
			}
		}



		private F140101Expansion _selectedF140101;

		public F140101Expansion SelectedF140101
		{
			get { return _selectedF140101; }
			set
			{
				if (_selectedF140101 == value)
					return;
				Set(() => SelectedF140101, ref _selectedF140101, value);
				if (value != null)
					ShowQueryRule = false;
				SetWareHouseList();
				InventoryDetailItemList = null;
				QueryDetailItemIsEnabled = value != null;
				QueryDetailBegLocCode = null;
				QueryDetailEndLocCode = null;
				QueryDetailHasFindItem = false;
				QueryDetailItemCode = null;
				QueryDetailItemName = null;
				QueryDetailItemColor = null;
				QueryDetailItemSize = null;
				QueryDetailItemSpec = null;
				QueryDetailSerialNo = null;
        SetProcTypeList();


      }
		}


		#endregion

		#endregion

		#region 明細查詢

		#region Grid 盤點單明細
		private List<InventoryDetailItem> _inventoryDetailItemList;

		public List<InventoryDetailItem> InventoryDetailItemList
		{
			get { return _inventoryDetailItemList; }
			set
			{
				if (_inventoryDetailItemList == value)
					return;
				Set(() => InventoryDetailItemList, ref _inventoryDetailItemList, value);
			}
		}

		private InventoryDetailItem _selectedInventoryDetailItem;

		public InventoryDetailItem SelectedInventoryDetailItem
		{
			get { return _selectedInventoryDetailItem; ; }
			set
			{
				if (_selectedInventoryDetailItem == value)
					return;
				Set(() => SelectedInventoryDetailItem, ref _selectedInventoryDetailItem, value);
			}
		}

		#endregion

		#region 倉別
		private ObservableCollection<NameValuePair<string>> _queryDetailWareHouseList;

		public ObservableCollection<NameValuePair<string>> QueryDetailWareHouseList
		{
			get { return _queryDetailWareHouseList; }
			set
			{
				if (_queryDetailWareHouseList == value)
					return;
				Set(() => QueryDetailWareHouseList, ref _queryDetailWareHouseList, value);
			}
		}
		private string _selectedQueryDetailWareHouseId;

		public string SelectedQueryDetialWareHouseId
		{
			get { return _selectedQueryDetailWareHouseId; }
			set
			{
				if (_selectedQueryDetailWareHouseId == value)
					return;
				Set(() => SelectedQueryDetialWareHouseId, ref _selectedQueryDetailWareHouseId, value);
			}
		}

		#endregion

		#region 儲位範圍-起
		private string _queryDetailBegLocCode;

		public string QueryDetailBegLocCode
		{
			get { return _queryDetailBegLocCode; }
			set
			{
				if (_queryDetailBegLocCode == value)
					return;
				Set(() => QueryDetailBegLocCode, ref _queryDetailBegLocCode, value);
			}
		}
		#endregion

		#region 儲位範圍-迄
		private string _queryDetailEndLocCode;

		public string QueryDetailEndLocCode
		{
			get { return _queryDetailEndLocCode; }
			set
			{
				if (_queryDetailEndLocCode == value)
					return;
				Set(() => QueryDetailEndLocCode, ref _queryDetailEndLocCode, value);
			}
		}
		#endregion

		#region 品號
		private string _queryDetailItemCode;

		public string QueryDetailItemCode
		{
			get { return _queryDetailItemCode; }
			set
			{
				if (_queryDetailItemCode == value)
					return;
				Set(() => QueryDetailItemCode, ref _queryDetailItemCode, value);
			}
		}
		#endregion

		#region 品名
		private string _queryDetailItemName;

		public string QueryDetailItemName
		{
			get { return _queryDetailItemName; }
			set
			{
				if (_queryDetailItemName == value)
					return;
				Set(() => QueryDetailItemName, ref _queryDetailItemName, value);
			}
		}

		#endregion

		#region 尺寸
		private string _queryDetailItemSize;

		public string QueryDetailItemSize
		{
			get { return _queryDetailItemSize; }
			set
			{
				if (_queryDetailItemSize == value)
					return;
				Set(() => QueryDetailItemSize, ref _queryDetailItemSize, value);
			}
		}
		#endregion

		#region 規格
		private string _queryDetailItemSpec;

		public string QueryDetailItemSpec
		{
			get { return _queryDetailItemSpec; }
			set
			{
				if (_queryDetailItemSpec == value)
					return;
				Set(() => QueryDetailItemSpec, ref _queryDetailItemSpec, value);
			}
		}
		#endregion

		#region 顏色
		private string _queryDetailItemColor;

		public string QueryDetailItemColor
		{
			get { return _queryDetailItemColor; }
			set
			{
				if (_queryDetailItemColor == value)
					return;
				Set(() => QueryDetailItemColor, ref _queryDetailItemColor, value);
			}
		}
		#endregion

		#region 序號
		private string _queryDetailSerialNo;
		public string QueryDetailSerialNo
		{
			get { return _queryDetailSerialNo; }
			set
			{
				if (_queryDetailSerialNo == value)
					return;
				Set(() => QueryDetailSerialNo, ref _queryDetailSerialNo, value);
			}
		}
		#endregion

		#region 是否找到商品
		private bool _queryDetailHasFindItem;

		public bool QueryDetailHasFindItem
		{
			get { return _queryDetailHasFindItem; }
			set
			{
				if (_queryDetailHasFindItem == value)
					return;
				Set(() => QueryDetailHasFindItem, ref _queryDetailHasFindItem, value);
			}
		}
		#endregion


		#region 是否啟用查詢商品UserControl
		private bool _queryDetailItemIsEnabled;

		public bool QueryDetailItemIsEnabled
		{
			get { return _queryDetailItemIsEnabled; }
			set
			{
				if (_queryDetailItemIsEnabled == value)
					return;
				Set(() => QueryDetailItemIsEnabled, ref _queryDetailItemIsEnabled, value);
			}
		}
    #endregion

    #region 商品類別
    private List<NameValuePair<string>> _itemTypeList;

    public List<NameValuePair<string>> ItemTypeList
    {
      get { return _itemTypeList; }
      set
      {
        if (_itemTypeList == value)
          return;
        Set(() => ItemTypeList, ref _itemTypeList, value);
      }
    }

    private string _selectedItemType;

    public string SelectedItemType
    {
      get { return _selectedItemType; }
      set
      {
        if (_selectedItemType == value)
          return;
        Set(() => SelectedItemType, ref _selectedItemType, value);
      }
    }
    #endregion


    #endregion

    #region 新增/修改

    #region 盤點單主檔
    private F140101 _addOrEditF140101;

		public F140101 AddOrEditF140101
		{
			get { return _addOrEditF140101; }
			set
			{
				if (_addOrEditF140101 == value)
					return;
				Set(() => AddOrEditF140101, ref _addOrEditF140101, value);
				if (AddOrEditF140101 == null)
					return;
				_addOrEditF140101.PropertyChanged -= AddOrEditF140101_PropertyChanged;
				_addOrEditF140101.PropertyChanged += AddOrEditF140101_PropertyChanged;

			}
		}

		private void AddOrEditF140101_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_addOrEditF140101.PropertyChanged -= AddOrEditF140101_PropertyChanged;

			switch (e.PropertyName)
			{
				case nameof(_addOrEditF140101.INVENTORY_TYPE):
					switch (_addOrEditF140101.INVENTORY_TYPE)
					{
						case "1":
							SetWareHouseTypeList();
							ShowColumn = DisplayType.display.ToString();
							AddOrEditF140101.CHECK_TOOL = "0";
							break;
						case "3":
						case "4":
							SetWareHouseTypeList();
							ShowColumn = DisplayType.display.ToString();
							break;
						default:
							SetWareHouseTypeList();
							ShowColumn = DisplayType.notDisplay.ToString();
							break;
					}
					ClearImportList();
					break;
				case nameof(_addOrEditF140101.CHECK_TOOL):
					DoSearchWareHouse();
					SetAreaList();
					break;
				case nameof(_addOrEditF140101.DC_CODE):
					SetAreaList();
					break;
			}
			_addOrEditF140101.PropertyChanged += AddOrEditF140101_PropertyChanged;
		}
		#endregion

		#region 盤點單名稱

		private string _addOrEditInventoryName;

		public string AddOrEditInventoryName
		{
			get { return _addOrEditInventoryName; }
			set
			{
				if (_addOrEditInventoryName == value)
					return;
				Set(() => AddOrEditInventoryName, ref _addOrEditInventoryName, value);
			}
		}

		#endregion

		#region 新增


		#region 是否可勾選費用
		private bool _canCheckIsCharge;

		public bool CanCheckIsCharge
		{
			get { return _canCheckIsCharge; }
			set
			{
				if (_canCheckIsCharge == value)
					return;
				Set(() => CanCheckIsCharge, ref _canCheckIsCharge, value);
			}
		}
		#endregion


		#region 是否可以輸入週期
		private bool _readOnlySetInventoryCycle;

		public bool ReadOnlyInventoryCycle
		{
			get { return _readOnlySetInventoryCycle; }
			set
			{
				if (_readOnlySetInventoryCycle == value)
					return;
				Set(() => ReadOnlyInventoryCycle, ref _readOnlySetInventoryCycle, value);
			}
		}
		#endregion


		#region 倉別

		private List<NameValuePair<string>> _addOrEditWareHouseList;

		public List<NameValuePair<string>> AddOrEditWareHouseList
		{
			get { return _addOrEditWareHouseList; }
			set
			{
				if (_addOrEditWareHouseList == value)
					return;
				Set(() => AddOrEditWareHouseList, ref _addOrEditWareHouseList, value);
			}
		}

		private string _selectedAddOrEditWareHouseId;

		public string SelectedAddOrEditWareHouseId
		{
			get { return _selectedAddOrEditWareHouseId; }
			set
			{
				if (_selectedAddOrEditWareHouseId == value)
					return;
				Set(() => SelectedAddOrEditWareHouseId, ref _selectedAddOrEditWareHouseId, value);
			}
		}


		#endregion


		#region 倉別型態
		private ObservableCollection<NameValuePair<string>> _warehouseTypeList;

		public ObservableCollection<NameValuePair<string>> WareHouseTypeList
		{
			get { return _warehouseTypeList; }
			set
			{
				if (_warehouseTypeList == value)
					return;
				Set(() => WareHouseTypeList, ref _warehouseTypeList, value);
			}
		}



		private string _selectedWareHouseType;
		public string SelectedWareHouseType
		{
			get { return _selectedWareHouseType; }
			set
			{
				if (_selectedWareHouseType == value)
					return;
				Set(() => SelectedWareHouseType, ref _selectedWareHouseType, value);
			}
		}


		#endregion

		#region Grid 倉別資料
		private List<InventoryWareHouse> _inventoryWareHouseList;

		public List<InventoryWareHouse> InventoryWareHouseList
		{
			get { return _inventoryWareHouseList; }
			set
			{
				if (_inventoryWareHouseList == value)
					return;
				Set(() => InventoryWareHouseList, ref _inventoryWareHouseList, value);
			}
		}


		private InventoryWareHouse _selectedInventoryWareHouse;

		public InventoryWareHouse SelectedInventoryWareHouse
		{
			get { return _selectedInventoryWareHouse; }
			set
			{
				if (_selectedInventoryWareHouse == value)
					return;
				Set(() => SelectedInventoryWareHouse, ref _selectedInventoryWareHouse, value);
				if (_selectedInventoryWareHouse == null) return;
				_selectedInventoryWareHouse.PropertyChanged -= SelectedInventoryWareHouse_PropertyChanged;
				_selectedInventoryWareHouse.PropertyChanged += SelectedInventoryWareHouse_PropertyChanged;

			}
		}

		private ObservableCollection<F140111> _addF140111List;
		public ObservableCollection<F140111> AddF140111List
		{
			get { return _addF140111List; }
			set { Set(ref _addF140111List, value); }
		}

		private SelectionItem<F140111Data> _importData;
		public SelectionItem<F140111Data> ImportData
		{
			get { return _importData; }
			set { Set(ref _importData, value); }
		}
		private SelectionList<F140111Data> _importList;
		public SelectionList<F140111Data> ImportList
		{
			get { return _importList; }
			set
			{
				Set(ref _importList, value);
				if (AddOrEditF140101 != null && AddOrEditF140101.INVENTORY_TYPE == "5")
				{
					if (UserOperateMode == OperateMode.Add)
						_isSave = ImportList != null && ImportList.Any();
					else if (UserOperateMode == OperateMode.Edit)
						_isSave = true;
				}
				else
				{
					_isSave = true;
				}
			}
		}

		private List<F140111Data> _importErrList;
		public List<F140111Data> ImportErrList
		{
			get { return _importErrList; }
			set { Set(ref _importErrList, value); }
		}

		private F140111Data _importErrData;
		public F140111Data ImportErrData
		{
			get { return _importErrData; }
			set { Set(ref _importErrData, value); }
		}

		private ObservableCollection<NameValuePair<string>> _areaList;
		public ObservableCollection<NameValuePair<string>> AreaList
		{
			get { return _areaList; }
			set { Set(ref _areaList, value); }
		}

		private List<NameValuePair<string>> _makeList;
		public List<NameValuePair<string>> MakeList
		{
			get { return _makeList; }
			set { Set(ref _makeList, value); }
		}

		private string _importLocCode;
		public string ImportLocCode
		{
			get { return _importLocCode; }
			set { Set(ref _importLocCode, LocCodeHelper.LocCodeConverter9(value)); }
		}

		private ImportType _importType;
		public ImportType ImportType
		{
			get { return _importType; }
			set
			{
				Set(ref _importType, value);
				ImportErrList = null;
			}
		}

		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				RaisePropertyChanged("IsCheckAll");
				if (ImportList != null)
					ImportList = ImportList.Select(o => o.Item).ToSelectionList(_isCheckAll);
			}
		}

		private bool _isNotUsing;
		public bool IsNotUsing
		{
			get { return _isNotUsing; }
			set { Set(ref _isNotUsing, value); }
		}

		private bool _mTypeIsEnabled;
		public bool MTypeIsEnabled
		{
			get { return _mTypeIsEnabled; }
			set { Set(ref _mTypeIsEnabled, value); }
		}

		private bool _sTypeIsEnabled;
		public bool STypeIsEnabled
		{
			get { return _sTypeIsEnabled; }
			set { Set(ref _sTypeIsEnabled, value); }
		}

		private void SelectedInventoryWareHouse_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_selectedInventoryWareHouse.PropertyChanged -= SelectedInventoryWareHouse_PropertyChanged;
			switch (e.PropertyName)
			{
				case nameof(SelectedInventoryWareHouse.FLOOR_BEGIN):
					FloorBeginChange();
					break;
				case nameof(SelectedInventoryWareHouse.FLOOR_END):
					FloorEndChange();
					if (string.Compare(SelectedInventoryWareHouse.FLOOR_BEGIN, SelectedInventoryWareHouse.FLOOR_END) > 0)
					{
						SelectedInventoryWareHouse.FLOOR_END = SelectedInventoryWareHouse.FLOOR_BEGIN;
						ShowWarningMessage(Properties.Resources.P140101_SelectedValueError);
					}
					break;
				case nameof(SelectedInventoryWareHouse.CHANNEL_BEGIN):
					ChannelBeginChange();
					break;
				case nameof(SelectedInventoryWareHouse.CHANNEL_END):
					ChannelEndChange();
					if (string.Compare(SelectedInventoryWareHouse.CHANNEL_BEGIN, SelectedInventoryWareHouse.CHANNEL_END) > 0)
					{
						SelectedInventoryWareHouse.CHANNEL_END = SelectedInventoryWareHouse.CHANNEL_BEGIN;
						ShowWarningMessage(Properties.Resources.P140101_SelectedValueError);
					}

					break;
				case nameof(SelectedInventoryWareHouse.PLAIN_BEGIN):
					PlainBeginChange();
					break;
				case nameof(SelectedInventoryWareHouse.PLAIN_END):
					if (string.Compare(SelectedInventoryWareHouse.PLAIN_BEGIN, SelectedInventoryWareHouse.PLAIN_END) > 0)
					{
						SelectedInventoryWareHouse.PLAIN_END = SelectedInventoryWareHouse.PLAIN_BEGIN;
						ShowWarningMessage(Properties.Resources.P140101_SelectedValueError);
					}
					break;
			}
			_selectedInventoryWareHouse.PropertyChanged += SelectedInventoryWareHouse_PropertyChanged;
		}



		private void FloorBeginChange()
		{
			SelectedInventoryWareHouse.FLOOR_END = null;
			FloorEndChange();
		}

		private void FloorEndChange()
		{
			SelectedInventoryWareHouse.CHANNEL_BEGIN = null;
			ChannelBeginChange();
		}
		private void ChannelBeginChange()
		{
			SelectedInventoryWareHouse.CHANNEL_END = null;
			ChannelEndChange();
		}

		private void ChannelEndChange()
		{
			SelectedInventoryWareHouse.PLAIN_BEGIN = null;
			PlainBeginChange();
		}

		private void PlainBeginChange()
		{
			SelectedInventoryWareHouse.PLAIN_END = null;
		}
		#endregion


		#region 全選/取消全選 倉別
		private bool _isCheckAllWareHouse;

		public bool IsCheckAllWareHouse
		{
			get { return _isCheckAllWareHouse; }
			set
			{
				if (_isCheckAllWareHouse == value)
					return;
				Set(() => IsCheckAllWareHouse, ref _isCheckAllWareHouse, value);
			}
		}
		#endregion


		#region 通道
		private List<NameValuePair<string>> _channelList;

		public List<NameValuePair<string>> ChannelList
		{
			get { return _channelList; }
			set
			{
				if (_channelList == value)
					return;
				Set(() => ChannelList, ref _channelList, value);
			}
		}
		#endregion


		#region Grid 盤點商品
		private List<InventoryItem> _inventoryItemList;

		public List<InventoryItem> InventoryItemList
		{
			get { return _inventoryItemList; }
			set
			{
				if (_inventoryItemList == value)
					return;
				Set(() => InventoryItemList, ref _inventoryItemList, value);
			}
		}



		private InventoryItem _selectedInventoryItem;

		public InventoryItem SelectedInventoryItem
		{
			get { return _selectedInventoryItem; }
			set
			{
				if (_selectedInventoryItem == value)
					return;
				Set(() => SelectedInventoryItem, ref _selectedInventoryItem, value);
			}
		}

		#endregion


		#region 全選/取消全選 盤點商品
		private bool _isCheckAllItem;

		public bool IsCheckAllItem
		{
			get { return _isCheckAllItem; }
			set
			{
				if (_isCheckAllItem == value)
					return;
				Set(() => IsCheckAllItem, ref _isCheckAllItem, value);
			}
		}
		#endregion

		#region 全部樓層(by WarehouseId)
		private Dictionary<string, List<NameValuePair<string>>> _allFloorDictionary;

		public Dictionary<string, List<NameValuePair<string>>> AllFloorDictionary
		{
			get { return _allFloorDictionary; }
			set
			{
				if (_allFloorDictionary == value)
					return;
				Set(() => AllFloorDictionary, ref _allFloorDictionary, value);
			}
		}
		#endregion

		#region 全部通道(by WarehouseId)
		private Dictionary<string, List<NameValuePair<string>>> _allChannelDictionary;

		public Dictionary<string, List<NameValuePair<string>>> AllChannelDictionary
		{
			get { return _allChannelDictionary; }
			set
			{
				if (_allChannelDictionary == value)
					return;
				Set(() => AllChannelDictionary, ref _allChannelDictionary, value);
			}
		}
		#endregion

		#region 全部座(by WarehouseId)
		private Dictionary<string, List<NameValuePair<string>>> _allPlainDictionary;

		public Dictionary<string, List<NameValuePair<string>>> AllPlainDictionary
		{
			get { return _allPlainDictionary; }
			set
			{
				if (_allPlainDictionary == value)
					return;
				Set(() => AllPlainDictionary, ref _allPlainDictionary, value);
			}
		}
		#endregion

		#region 商品大類
		private List<NameValuePair<string>> _itemLTypeList;

		public List<NameValuePair<string>> ItemLTypeList
		{
			get { return _itemLTypeList; }
			set
			{
				if (_itemLTypeList == value)
					return;
				Set(() => ItemLTypeList, ref _itemLTypeList, value);
			}
		}


		private string _selectedItemLType;

		public string SelectedItemLType
		{
			get { return _selectedItemLType; }
			set
			{
				if (_selectedItemLType == value)
					return;
				Set(() => SelectedItemLType, ref _selectedItemLType, value);
				if (!string.IsNullOrEmpty(SelectedItemLType))
				{
					SetItemMTypeList();
					var list = new List<NameValuePair<string>>();
					list.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
					ItemSTypeList = list;
					SelectedItemSType = ItemSTypeList.FirstOrDefault().Value;
					MTypeIsEnabled = true;
				}
				else
				{
					ItemMTypeList = new List<NameValuePair<string>> { new NameValuePair<string> { Name = Resources.Resources.All, Value = "" } };
					SelectedItemMType = "";
					MTypeIsEnabled = false;
				}
			}
		}

		/// <summary>
		/// 廠商編號
		/// </summary>
		private string _selectedVnrCode;
		public string SelectedVnrCode
		{
			get { return _selectedVnrCode; }
			set { Set(() => SelectedVnrCode, ref _selectedVnrCode, value); }
		}

    /// <summary>
		/// 原廠商編號
		/// </summary>
		private string _selectedOriVnrCode;
    public string SelectedOriVnrCode
    {
      get { return _selectedOriVnrCode; }
      set { Set(() => SelectedOriVnrCode, ref _selectedOriVnrCode, value); }
    }

    /// <summary>
    /// 廠商名稱
    /// </summary>
    private string _selectedVnrName;
		public string SelectedVnrName
		{
			get { return _selectedVnrName; }
			set { Set(() => SelectedVnrName, ref _selectedVnrName, value); }
		}

		#endregion


		#region 商品中類
		private List<NameValuePair<string>> _itemMTypeList;

		public List<NameValuePair<string>> ItemMTypeList
		{
			get { return _itemMTypeList; }
			set
			{
				if (_itemMTypeList == value)
					return;
				Set(() => ItemMTypeList, ref _itemMTypeList, value);
			}
		}


		private string _selectedItemMType;

		public string SelectedItemMType
		{
			get { return _selectedItemMType; }
			set
			{
				if (_selectedItemMType == value)
					return;
				Set(() => SelectedItemMType, ref _selectedItemMType, value);
				if (!string.IsNullOrEmpty(SelectedItemMType))
				{
					SetItemSTypeList();
					STypeIsEnabled = true;
				}
				else
				{
					ItemSTypeList = new List<NameValuePair<string>> { new NameValuePair<string> { Name = Resources.Resources.All, Value = "" } };
					SelectedItemSType = "";
					STypeIsEnabled = false;
				}
			}
		}


		#endregion


		#region 商品小類
		private List<NameValuePair<string>> _itemSTypeList;

		public List<NameValuePair<string>> ItemSTypeList
		{
			get { return _itemSTypeList; }
			set
			{
				if (_itemSTypeList == value)
					return;
				Set(() => ItemSTypeList, ref _itemSTypeList, value);
			}
		}

		private string _selectedItemSType;

		public string SelectedItemSType
		{
			get { return _selectedItemSType; }
			set
			{
				if (_selectedItemSType == value)
					return;
				Set(() => SelectedItemSType, ref _selectedItemSType, value);

			}
		}


		#endregion

		#region 品號
		private string _addItemCode;

		public string AddItemCode
		{
			get { return _addItemCode; }
			set
			{
				if (_addItemCode == value)
					return;
				Set(() => AddItemCode, ref _addItemCode, value);
			}
		}
		#endregion

		#region 品名
		private string _addItemName;

		public string AddItemName
		{
			get { return _addItemName; }
			set
			{
				if (_addItemName == value)
					return;
				Set(() => AddItemName, ref _addItemName, value);
			}
		}

		#endregion

		#region 尺寸
		private string _addItemSize;

		public string AddItemSize
		{
			get { return _addItemSize; }
			set
			{
				if (_addItemSize == value)
					return;
				Set(() => AddItemSize, ref _addItemSize, value);
			}
		}
		#endregion

		#region 規格
		private string _addItemSpec;

		public string AddItemSpec
		{
			get { return _addItemSpec; }
			set
			{
				if (_addItemSpec == value)
					return;
				Set(() => AddItemSpec, ref _addItemSpec, value);
			}
		}
		#endregion

		#region 顏色
		private string _addItemColor;

		public string AddItemColor
		{
			get { return _addItemColor; }
			set
			{
				if (_addItemColor == value)
					return;
				Set(() => AddItemColor, ref _addItemColor, value);
			}
		}
		#endregion

		#region 序號
		private string _addSerialNo;
		public string AddSerialNo
		{
			get { return _addSerialNo; }
			set
			{
				if (_addSerialNo == value)
					return;
				Set(() => AddSerialNo, ref _addSerialNo, value);
			}
		}
		#endregion

		#region 是否找到商品
		private bool _addHasFindItem;

		public bool AddHasFindItem
		{
			get { return _addHasFindItem; }
			set
			{
				if (_addHasFindItem == value)
					return;
				Set(() => AddHasFindItem, ref _addHasFindItem, value);
			}
		}
		#endregion

		#region 匯入商品編號檔案路徑
		private string _importItemCodeFilePath;

		public string ImportItemCodeFilePath
		{
			get { return _importItemCodeFilePath; }
			set
			{
				if (_importItemCodeFilePath == value)
					return;
				Set(() => ImportItemCodeFilePath, ref _importItemCodeFilePath, value);
			}
		}
		#endregion

		#region 匯入檔案路徑參數

		private string _importFilePath;

		public string ImportFilePath
		{
			get { return _importFilePath; }
			set
			{
				_importFilePath = value;
				RaisePropertyChanged("ImportFilePath");
			}
		}
		#endregion

		private List<NameValuePair<string>> _checkToolList;
		public List<NameValuePair<string>> CheckToolList
		{
			get { return _checkToolList; }
			set { Set(ref _checkToolList, value); }
		}

		#endregion

		#region 修改


		#region 顯示隱藏查詢明細條件
		private bool _isShowQueryDetailRule;

		public bool IsShowQueryDetailRule
		{
			get { return _isShowQueryDetailRule; }
			set
			{
				if (_isShowQueryDetailRule == value)
					return;
				Set(() => IsShowQueryDetailRule, ref _isShowQueryDetailRule, value);
			}
		}
		#endregion


		/// <summary>
		/// 暫存有異動資料
		/// </summary>
		private List<InventoryDetailItem> _tempEditInventoryDetailItemList;
		#region Grid 盤點單明細
		private List<InventoryDetailItem> _editInventoryDetailItemList;

		public List<InventoryDetailItem> EditInventoryDetailItemList
		{
			get { return _editInventoryDetailItemList; }
			set
			{
				if (_editInventoryDetailItemList == value)
					return;
				Set(() => EditInventoryDetailItemList, ref _editInventoryDetailItemList, value);
			}
		}

		private InventoryDetailItem _selectedEditInventoryDetailItem;

		public InventoryDetailItem SelectedEditInventoryDetailItem
		{
			get { return _selectedEditInventoryDetailItem; ; }
			set
			{
				if (_selectedEditInventoryDetailItem == value)
					return;
				Set(() => SelectedEditInventoryDetailItem, ref _selectedEditInventoryDetailItem, value);
			}
		}

		#endregion


		#region 全選/取消全選盤點明細
		private bool _isCheckAllEditInventoryDetailItem;

		public bool IsCheckAllEditInventoryDetailItem
		{
			get { return _isCheckAllEditInventoryDetailItem; }
			set
			{
				if (_isCheckAllEditInventoryDetailItem == value)
					return;
				Set(() => IsCheckAllEditInventoryDetailItem, ref _isCheckAllEditInventoryDetailItem, value);
			}
		}
		#endregion

		#region  查詢明細

		#region 品號
		private string _editQueryItemCode;

		public string EditQueryItemCode
		{
			get { return _editQueryItemCode; }
			set
			{
				if (_editQueryItemCode == value)
					return;
				Set(() => EditQueryItemCode, ref _editQueryItemCode, value);
			}
		}
		#endregion

		#region 品名
		private string _editQueryItemName;

		public string EditQueryItemName
		{
			get { return _editQueryItemName; }
			set
			{
				if (_editQueryItemName == value)
					return;
				Set(() => EditQueryItemName, ref _editQueryItemName, value);
			}
		}

		#endregion

		#region 尺寸
		private string _editQueryItemSize;

		public string EditQueryItemSize
		{
			get { return _editQueryItemSize; }
			set
			{
				if (_editQueryItemSize == value)
					return;
				Set(() => EditQueryItemSize, ref _editQueryItemSize, value);
			}
		}
		#endregion

		#region 規格
		private string _editQueryItemSpec;

		public string EditQueryItemSpec
		{
			get { return _editQueryItemSpec; }
			set
			{
				if (_editQueryItemSpec == value)
					return;
				Set(() => EditQueryItemSpec, ref _editQueryItemSpec, value);
			}
		}
		#endregion

		#region 顏色
		private string _editQueryItemColor;

		public string EditQueryItemColor
		{
			get { return _editQueryItemColor; }
			set
			{
				if (_editQueryItemColor == value)
					return;
				Set(() => EditQueryItemColor, ref _editQueryItemColor, value);
			}
		}
		#endregion

		#region 序號
		private string _editQuerySerialNo;
		public string EditQuerySerialNo
		{
			get { return _editQuerySerialNo; }
			set
			{
				if (_editQuerySerialNo == value)
					return;
				Set(() => EditQuerySerialNo, ref _editQuerySerialNo, value);
			}
		}
		#endregion

		#region 是否找到商品
		private bool _editQueryHasFindItem;

		public bool EditQueryHasFindItem
		{
			get { return _editQueryHasFindItem; }
			set
			{
				if (_editQueryHasFindItem == value)
					return;
				Set(() => EditQueryHasFindItem, ref _editQueryHasFindItem, value);
			}
		}
		#endregion


		#region 倉別
		private string _selectedEditQueryDetailWareHouseId;

		public string SelectedEditQueryDetailWareHouseId
		{
			get { return _selectedEditQueryDetailWareHouseId; }
			set
			{
				if (_selectedEditQueryDetailWareHouseId == value)
					return;
				Set(() => SelectedEditQueryDetailWareHouseId, ref _selectedEditQueryDetailWareHouseId, value);
			}
		}
		#endregion

		#region 儲位範圍-起
		private string _editQueryDetailBegLocCode;

		public string EditQueryDetailBegLocCode
		{
			get { return _editQueryDetailBegLocCode; }
			set
			{
				if (_editQueryDetailBegLocCode == value)
					return;
				Set(() => EditQueryDetailBegLocCode, ref _editQueryDetailBegLocCode, value);
			}
		}
		#endregion

		#region 儲位範圍-迄
		private string _editQueryDetailEndLocCode;

		public string EditQueryDetailEndLocCode
		{
			get { return _editQueryDetailEndLocCode; }
			set
			{
				if (_editQueryDetailEndLocCode == value)
					return;
				Set(() => EditQueryDetailEndLocCode, ref _editQueryDetailEndLocCode, value);
			}
		}
		#endregion

		#endregion

		#region 新增明細
		#region 品號
		private string _editItemCode;

		public string EditItemCode
		{
			get { return _editItemCode; }
			set
			{
				if (_editItemCode == value)
					return;
				Set(() => EditItemCode, ref _editItemCode, value);
			}
		}
		#endregion

		#region 品名
		private string _editItemName;

		public string EditItemName
		{
			get { return _editItemName; }
			set
			{
				if (_editItemName == value)
					return;
				Set(() => EditItemName, ref _editItemName, value);
			}
		}

		#endregion

		#region 尺寸
		private string _editItemSize;

		public string EditItemSize
		{
			get { return _editItemSize; }
			set
			{
				if (_editItemSize == value)
					return;
				Set(() => EditItemSize, ref _editItemSize, value);
			}
		}
		#endregion

		#region 規格
		private string _editItemSpec;

		public string EditItemSpec
		{
			get { return _editItemSpec; }
			set
			{
				if (_editItemSpec == value)
					return;
				Set(() => EditItemSpec, ref _editItemSpec, value);
			}
		}
		#endregion

		#region 序號
		private string _editSerialNo;

		public string EditSerialNo
		{
			get { return _editSerialNo; }
			set
			{
				if (_editSerialNo == value)
					return;
				Set(() => EditSerialNo, ref _editSerialNo, value);
			}
		}
		#endregion

		#region 顏色
		private string _editItemColor;

		public string EditItemColor
		{
			get { return _editItemColor; }
			set
			{
				if (_editItemColor == value)
					return;
				Set(() => EditItemColor, ref _editItemColor, value);
			}
		}
		#endregion

		#region 貨主品編
		private string _editCustItemCode;

		public string EditCustItemCode
		{
			get { return _editCustItemCode; }
			set
			{
				if (_editCustItemCode == value)
					return;
				Set(() => EditCustItemCode, ref _editCustItemCode, value);
			}
		}
		#endregion

		#region 國條1
		private string _editEanCode1;

		public string EditEanCode1
		{
			get { return _editEanCode1; }
			set
			{
				if (_editEanCode1 == value)
					return;
				Set(() => EditEanCode1, ref _editEanCode1, value);
			}
		}
		#endregion

		#region 國條2
		private string _editEanCode2;

		public string EditEanCode2
		{
			get { return _editEanCode2; }
			set
			{
				if (_editEanCode2 == value)
					return;
				Set(() => EditEanCode2, ref _editEanCode2, value);
			}
		}
		#endregion

		#region 國條3
		private string _editEanCode3;

		public string EditEanCode3
		{
			get { return _editEanCode3; }
			set
			{
				if (_editEanCode3 == value)
					return;
				Set(() => EditEanCode3, ref _editEanCode3, value);
			}
		}
		#endregion

		#region 是否找到商品
		private bool _editHasFindItem;

		public bool EditHasFindItem
		{
			get { return _editHasFindItem; }
			set
			{
				if (_editHasFindItem == value)
					return;
				Set(() => EditHasFindItem, ref _editHasFindItem, value);
			}
		}
		#endregion

		#region 儲位
		private string _editLocCode;

		public string EditLocCode
		{
			get { return _editLocCode; }
			set
			{
				if (_editLocCode == value)
					return;
				Set(() => EditLocCode, ref _editLocCode, value);
			}
		}
		#endregion

		#region 批號
		private string _editMakeNo;

		public string EditMakeNo
		{
			get { return _editMakeNo; }
			set
			{
				if (_editMakeNo == value)
					return;
				Set(() => EditMakeNo, ref _editMakeNo, value);
			}
		}
		#endregion

		#region 初盤/複盤數量
		private int _editQty;

		public int EditQty
		{
			get { return _editQty; }
			set
			{
				if (_editQty == value)
					return;
				Set(() => EditQty, ref _editQty, value);
			}
		}
		#endregion


		#region 效期
		private DateTime _editValidDate;

		public DateTime EditValidDate
		{
			get { return _editValidDate; }
			set
			{
				if (_editValidDate == value)
					return;
				Set(() => EditValidDate, ref _editValidDate, value);
			}
		}
		#endregion


		#endregion

		#region  匯入盤點結果檔案路徑
		private string _importInventoryDetailItemFilePath;

		public string ImportInventoryDetailItemFilePath
		{
			get { return _importInventoryDetailItemFilePath; }
			set
			{
				_importInventoryDetailItemFilePath = value;
				RaisePropertyChanged("ImportInventoryDetailItemFilePath");
			}
		}
		#endregion


		#region Grid 是否不可以修改初盤數
		private bool _isReadOnlyFirstQty;

		public bool IsReadOnlyFirstQty
		{
			get { return _isReadOnlyFirstQty; }
			set
			{
				if (_isReadOnlyFirstQty == value)
					return;
				Set(() => IsReadOnlyFirstQty, ref _isReadOnlyFirstQty, value);
			}
		}
		#endregion


		#region Grid 是否不可以修改複盤數
		private bool _isReadOnlySecondQty;

		public bool IsReadOnlySecondQty
		{
			get { return _isReadOnlySecondQty; }
			set
			{
				if (_isReadOnlySecondQty == value)
					return;
				Set(() => IsReadOnlySecondQty, ref _isReadOnlySecondQty, value);
			}
		}
		#endregion

		#region Grid 貨主是否不可以修改盤盈可回沖
		private bool _custReadOnlyFlushBack;

		public bool CustReadOnlyFlushBack
		{
			get { return _custReadOnlyFlushBack; }
			set
			{
				if (_custReadOnlyFlushBack == value)
					return;
				Set(() => CustReadOnlyFlushBack, ref _custReadOnlyFlushBack, value);
			}
		}
    #endregion

    #endregion

    #endregion

    #region 盤點階段下拉選單
    /// <summary>
    /// 存完整的盤點階段內容用，避免重複DB撈取
    /// </summary>
    private List<NameValuePair<string>> _fullProcTypeList;

    private List<NameValuePair<string>> _procTypeList;
    /// <summary>
    /// 盤點階段下拉選單
    /// </summary>
    public List<NameValuePair<string>> ProcTypeList
    {
      get { return _procTypeList; }
      set { Set(() => ProcTypeList, ref _procTypeList, value); }
    }

    private string _selectedProcType;
    /// <summary>
    /// 盤點階段下拉選單選中的內容
    /// </summary>
    public string SelectedProcType
    { get { return _selectedProcType; }
      set { Set(() => SelectedProcType, ref _selectedProcType, value); }
    }
    #endregion

    #endregion

    #region 下拉選單資料繫結

    private void SetReportTypeList()
		{
			ReportTypeList = GetBaseTableService.GetF000904List(FunctionCode, "P140101", "Report");
			if (ReportTypeList.Any())
				SelectedReportType = ReportTypeList.First().Value;
		}

		private void SetDcList()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
				SelectedQueryDcCode = DcList.First().Value;
		}

		private void SetStatusList()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F140101", "STATUS", true);
			if (StatusList.Any())
				SelectedStatus = StatusList.First().Value;
		}

		private void SetInventoryTypeList()
		{
			InventoryTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F140101", "INVENTORY_TYPE");
		}



		private void SetWareHouseList()
		{
			if (SelectedF140101 != null)
			{
				var proxy = GetProxy<F19Entities>();
				QueryDetailWareHouseList = proxy.F1980s.Where(o => o.DC_CODE == SelectedF140101.DC_CODE)
					.Select(item => new NameValuePair<string> { Name = item.WAREHOUSE_NAME, Value = item.WAREHOUSE_ID })
					.ToObservableCollection();
				QueryDetailWareHouseList.Insert(0, new NameValuePair<string> { Value = string.Empty, Name = Resources.Resources.All });
				SelectedQueryDetialWareHouseId = string.Empty;
			}
			else
				QueryDetailWareHouseList = new ObservableCollection<NameValuePair<string>>();
		}

		private void SetInventoryYearList()
		{
			var list = new List<NameValuePair<string>>();
			for (int i = DateTime.Now.Year; i <= DateTime.Now.Year + 5; i++)
				list.Add(new NameValuePair<string> { Name = i.ToString(), Value = i.ToString() });
			InventoryYearList = list;
		}

		public void SetInventoryMonthList()
		{
			var list = new List<NameValuePair<string>>();
			if (AddOrEditF140101 != null && AddOrEditF140101.INVENTORY_YEAR.HasValue)
			{
				if (AddOrEditF140101.INVENTORY_YEAR == DateTime.Now.Year)
					for (int i = DateTime.Now.Month; i <= 12; i++)
						list.Add(new NameValuePair<string> { Name = i.ToString(), Value = i.ToString() });
				else
					for (int i = 1; i <= 12; i++)
						list.Add(new NameValuePair<string> { Name = i.ToString(), Value = i.ToString() });
			}
			InventoryMonthList = list;
			if (list.Any() && AddOrEditF140101 != null)
				AddOrEditF140101.INVENTORY_MON = short.Parse(list.First().Value);
		}

		public void SetAddOrEditWareHouseList()
		{
			if (!string.IsNullOrEmpty(AddOrEditF140101.DC_CODE))
			{
				var proxy = GetProxy<F19Entities>();
				var list = proxy.F1980s.Where(o => o.DC_CODE == AddOrEditF140101.DC_CODE)
					.Select(item => new NameValuePair<string> { Name = item.WAREHOUSE_NAME, Value = item.WAREHOUSE_ID })
					.ToList();

				list.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
				AddOrEditWareHouseList = list;
				SelectedAddOrEditWareHouseId = list.First().Value;
			}
			else
				AddOrEditWareHouseList = new List<NameValuePair<string>>();
		}

		private void SetWareHouseTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			var list = proxy.F198001s
				.Select(item => new NameValuePair<string> { Name = item.TYPE_NAME, Value = item.TYPE_ID })
				.ToObservableCollection();
			list.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			WareHouseTypeList = list;
			SelectedWareHouseType = list.First().Value;
		}

		private void SetAreaList()
		{
			var proxy = GetProxy<F19Entities>();
			var f1980s = proxy.F1980s.Where(o => o.DC_CODE == AddOrEditF140101.DC_CODE).ToList();
			var f1919s = proxy.F1919s.Where(o => o.DC_CODE == AddOrEditF140101.DC_CODE).ToList();
			var f191902s = proxy.F191902s.Where(o => o.DC_CODE == AddOrEditF140101.DC_CODE && o.GUP_CODE == AddOrEditF140101.GUP_CODE && o.CUST_CODE == AddOrEditF140101.CUST_CODE).ToList();
			if (AddOrEditF140101.CHECK_TOOL == "0")
			{
				AreaList = (from a in f1980s
										join b in f1919s
										on new { a.WAREHOUSE_ID,a.DC_CODE } equals new { b.WAREHOUSE_ID, b.DC_CODE }
										where false
										select new NameValuePair<string> { Value = b.AREA_CODE, Name = b.AREA_NAME }).ToObservableCollection();
			}

			if (AddOrEditF140101.CHECK_TOOL == "1")
			{
				f191902s = f191902s.Where(o => o.PICK_TOOL == "4").ToList();
				AreaList = (from a in f1980s
										join b in f1919s
										on new { a.WAREHOUSE_ID,a.DC_CODE } equals new { b.WAREHOUSE_ID,b.DC_CODE }
										join c in f191902s
										on new { b.AREA_CODE, b.WAREHOUSE_ID,b.DC_CODE } equals new { c.AREA_CODE, c.WAREHOUSE_ID,c.DC_CODE }
										where false
										select new NameValuePair<string> { Value = b.AREA_CODE, Name = b.AREA_NAME }).ToObservableCollection();
			}
		}

		private void SetCheckTool()
		{
			CheckToolList = GetBaseTableService.GetF000904List(FunctionCode, "F1980", "DEVICE_TYPE");
		}

		private void SetAllFloorDictionary()
		{
			var proxyEx = GetExProxy<P14ExDataSource>();
			var datas = proxyEx.CreateQuery<WareHouseFloor>("GetWareHouseFloor").ToList();
			AllFloorDictionary = datas.GroupBy(o => o.WAREHOUSE_ID).ToDictionary(item => item.Key, item => item.OrderBy(o => o.FLOOR).Select(o => new NameValuePair<string>
			{
				Name = o.FLOOR,
				Value = o.FLOOR
			}).ToList());
		}

		private void SetAllChannelDictionary()
		{
			var proxyEx = GetExProxy<P14ExDataSource>();
			var datas = proxyEx.CreateQuery<WareHouseChannel>("GetWareHouseChannel").ToList();
			AllChannelDictionary = datas.GroupBy(o => o.WAREHOUSE_ID).ToDictionary(item => item.Key, item => item.OrderBy(o => o.CHANNEL).Select(o => new NameValuePair<string>
			{
				Name = o.CHANNEL,
				Value = o.CHANNEL
			}).ToList());
		}

		private void SetAllPlainDictionary()
		{
			var proxyEx = GetExProxy<P14ExDataSource>();
			var datas = proxyEx.CreateQuery<WareHousePlain>("GetWareHousePlain").ToList();
			AllPlainDictionary = datas.GroupBy(o => o.WAREHOUSE_ID).ToDictionary(item => item.Key, item => item.OrderBy(o => o.PLAIN).Select(o => new NameValuePair<string>
			{
				Name = o.PLAIN,
				Value = o.PLAIN
			}).ToList());
		}

		private void SetItemTypeList()
		{
			ItemTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TYPE", true);
			if (ItemTypeList.Any())
				SelectedItemType = ItemTypeList.First().Value;
		}

		private void SetItemLTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			ItemLTypeList = proxy.F1915s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode).Select(item => new NameValuePair<string> { Name = item.CLA_NAME, Value = item.ACODE }).ToList();
			ItemLTypeList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			if (ItemLTypeList.Any())
				SelectedItemLType = ItemLTypeList.First().Value;
		}
		private void SetItemMTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			var tmplist = proxy.F1916s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.ACODE == SelectedItemLType);
			var list = tmplist.Select(item => new NameValuePair<string> { Name = item.CLA_NAME, Value = item.BCODE }).ToList();
			list.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			ItemMTypeList = list;
			if (ItemMTypeList.Any())
				SelectedItemMType = ItemMTypeList.First().Value;
		}

		private void SetItemSTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			var tmplist = proxy.F1917s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.ACODE == SelectedItemLType && o.BCODE == SelectedItemMType);
			var list = tmplist.Select(item => new NameValuePair<string> { Name = item.CLA_NAME, Value = item.CCODE }).ToList();
			list.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			ItemSTypeList = list;
			if (ItemSTypeList.Any())
				SelectedItemSType = ItemSTypeList.First().Value;
		}

    /// <summary>
    /// 設定盤點階段清單內容
    /// </summary>
    private void SetProcTypeList()
    {
      if (_fullProcTypeList == null || !_fullProcTypeList.Any())
        _fullProcTypeList = GetBaseTableService.GetF000904List(FunctionCode, "P1401010000", "PROC_TYPE");

      if (_fullProcTypeList == null || !_fullProcTypeList.Any())
        return;

      if (SelectedF140101 != null)
      {
        if (SelectedF140101.ISSECOND == "1")
          ProcTypeList = _fullProcTypeList;
        else
          ProcTypeList = _fullProcTypeList.Where(x => x.Value == "0").ToList();
      }
      else
        ProcTypeList = _fullProcTypeList;

      if (ProcTypeList != null && ProcTypeList.Any())
        SelectedProcType = ProcTypeList.First().Value;
    }

    #endregion

    #region Method

    public void SetInventoryName()
		{

			if (AddOrEditF140101 != null)
			{

				AddOrEditInventoryName = string.Format("{0}{1}{2}{3}{4}", AddOrEditF140101.INVENTORY_DATE.ToString("yyyyMMdd"), GupCode,
					AddOrEditF140101.INVENTORY_TYPE, AddOrEditF140101.INVENTORY_CYCLE, AddOrEditF140101.CYCLE_TIMES);
			}
			else
				AddOrEditInventoryName = string.Empty;
		}

		public void DoSearchWareHouse()
		{
			IsCheckAllWareHouse = false;
			//執行查詢動
			var proxyEx = GetExProxy<P14ExDataSource>();
			InventoryWareHouseList = proxyEx.CreateQuery<InventoryWareHouse>("GetInventoryWareHouses")
				.AddQueryExOption("dcCode", AddOrEditF140101.DC_CODE)
				.AddQueryExOption("wareHouseType", SelectedWareHouseType ?? "")
				.AddQueryExOption("tool", AddOrEditF140101.CHECK_TOOL)
				.ToList();
		}

		public void CheckAllWareHouse()
		{
			if (InventoryWareHouseList != null)
			{
				foreach (var inventoryWareHouse in InventoryWareHouseList)
					inventoryWareHouse.IsSelected = IsCheckAllWareHouse;
				InventoryWareHouseList = InventoryWareHouseList.ToList();
			}
		}

		public void CheckAllItem()
		{
			if (InventoryItemList != null)
			{
				foreach (var inventoryItem in InventoryItemList)
					inventoryItem.IsSelected = IsCheckAllItem;
				InventoryItemList = InventoryItemList.ToList();
			}
		}

		public void CheckAllEditInventoryDetailItem()
		{
			if (EditInventoryDetailItemList != null)
			{
				foreach (var editInventoryDetailItem in EditInventoryDetailItemList)
					editInventoryDetailItem.IsSelected = IsCheckAllEditInventoryDetailItem;
				EditInventoryDetailItemList = EditInventoryDetailItemList.ToList();
			}
		}

		public void SetIsCanCharge()
		{
			//if (!string.IsNullOrEmpty(AddOrEditF140101.DC_CODE) && !string.IsNullOrEmpty(AddOrEditF140101.INVENTORY_TYPE) && (AddOrEditF140101.INVENTORY_TYPE == "3" || AddOrEditF140101.INVENTORY_TYPE == "4"))
			//{
			//	var proxy = GetProxy<F14Entities>();
			//	var year = AddOrEditF140101.INVENTORY_DATE.Year;
			//	int count =
			//		proxy.F140101s.Where(
			//			o =>
			//				o.DC_CODE == AddOrEditF140101.DC_CODE && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
			//				(o.INVENTORY_TYPE == "3" || o.INVENTORY_TYPE == "4") && o.STATUS != "9" && o.INVENTORY_DATE.Year == year).Count();
			//	if (count >= 2)
			//	{
			//		if (UserOperateMode == OperateMode.Add)
			//			AddOrEditF140101.ISCHARGE = "1";
			//		 CanCheckIsCharge = false;
			//		return;
			//	}
			//}
			CanCheckIsCharge = true;
		}

		public void SetIsCanSetInventoryCycle()
		{
			if (!string.IsNullOrEmpty(AddOrEditF140101.DC_CODE) && !string.IsNullOrEmpty(AddOrEditF140101.INVENTORY_TYPE) && AddOrEditF140101.INVENTORY_TYPE == "1" && AddOrEditF140101.INVENTORY_YEAR.HasValue && AddOrEditF140101.INVENTORY_MON.HasValue)
			{
				var proxy = GetProxy<F14Entities>();
				var item = proxy.F140101s.Where(
					o =>
						o.DC_CODE == AddOrEditF140101.DC_CODE && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
						o.INVENTORY_TYPE == "1" && o.STATUS != "9" && o.INVENTORY_YEAR == AddOrEditF140101.INVENTORY_YEAR &&
						o.INVENTORY_MON == AddOrEditF140101.INVENTORY_MON).FirstOrDefault();
				if (item != null)
				{
					AddOrEditF140101.INVENTORY_CYCLE = item.INVENTORY_CYCLE;
					ReadOnlyInventoryCycle = true;
					return;
				}
			}
			ReadOnlyInventoryCycle = false;
		}

		/// <summary>
		/// //檢查儲位
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="locCode"></param>
		/// <returns></returns>
		public ExecuteResult CheckLocCode(string dcCode, string locCode, string itemCode)
		{
			var proxyEx = GetExProxy<P14ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("CheckLocCode")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("locCode", locCode)
				.AddQueryExOption("itemCode", itemCode)
				.ToList().First();
			return result;
		}


		private void SaveTempInventoryDetailItems()
		{
			if (EditInventoryDetailItemList != null)
			{
				var tempList = _tempEditInventoryDetailItemList ?? new List<InventoryDetailItem>();
				var list =
					EditInventoryDetailItemList.Where(
						o =>
							o.ChangeStatus != "N" || o.FIRST_QTY != o.FIRST_QTY_ORG || o.SECOND_QTY != o.SECOND_QTY_ORG ||
							o.FLUSHBACK != o.FLUSHBACK_ORG).ToList();
				foreach (var inventoryDetailItem in list)
				{
					//Todo makeNo To Key 2019/05/06
					//板號、箱號、批號處理，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
					var boxCtrlNo = string.IsNullOrWhiteSpace(inventoryDetailItem.BOX_CTRL_NO) ? "0" : inventoryDetailItem.BOX_CTRL_NO;
					var makeNo = string.IsNullOrWhiteSpace(inventoryDetailItem.MAKE_NO) ? "0" : inventoryDetailItem.MAKE_NO;
					var palletCtrlNo = string.IsNullOrWhiteSpace(inventoryDetailItem.PALLET_CTRL_NO) ? "0" : inventoryDetailItem.PALLET_CTRL_NO;

					var item =
						tempList.FirstOrDefault(
							o =>
								o.LOC_CODE == inventoryDetailItem.LOC_CODE && o.ITEM_CODE == inventoryDetailItem.ITEM_CODE &&
								o.ENTER_DATE == inventoryDetailItem.ENTER_DATE && o.VALID_DATE == inventoryDetailItem.VALID_DATE && o.ChangeStatus != "D" && o.MAKE_NO == makeNo && o.BOX_CTRL_NO == boxCtrlNo && o.PALLET_CTRL_NO == palletCtrlNo);
					if (item == null)
					{
						if (inventoryDetailItem.ChangeStatus != "A")
							inventoryDetailItem.ChangeStatus = "E";
						tempList.Add(inventoryDetailItem);
					}
					else
					{
						if (item.ChangeStatus != "A")
							item.ChangeStatus = "E";
						item.FIRST_QTY = inventoryDetailItem.FIRST_QTY;
						item.SECOND_QTY = inventoryDetailItem.SECOND_QTY;
						item.FLUSHBACK = inventoryDetailItem.FLUSHBACK;
					}
				}
				_tempEditInventoryDetailItemList = tempList;
			}
		}

		private InventoryDetailItem FindInventoryDetailItems(string locCode, string itemCode, string enterDate, string validDate, string makeNo)
		{
			var proxyEx = GetExProxy<P14ExDataSource>();
			var result = proxyEx.CreateQuery<InventoryDetailItem>("FindInventoryDetailItems")
					.AddQueryExOption("dcCode", AddOrEditF140101.DC_CODE)
					.AddQueryExOption("gupCode", AddOrEditF140101.GUP_CODE)
					.AddQueryExOption("custCode", AddOrEditF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", AddOrEditF140101.INVENTORY_NO)
					.AddQueryExOption("locCode", locCode)
					.AddQueryExOption("itemCode", itemCode)
					.AddQueryExOption("enterDate", enterDate)
					.AddQueryExOption("validDate", validDate)
					.AddQueryExOption("makeNo", makeNo)
					.ToList();
			return result.FirstOrDefault();
		}

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearch(), () => UserOperateMode == OperateMode.Query
						);
			}
		}

		private void DoSearch()
		{
			string message;
			if (ValidateHelper.TryCheckBeginEnd(this, x => x.QueryInventorySDate, x => x.QueryInventoryEDate, Properties.Resources.QueryInventorySDate, out message))
			{
				//執行查詢動
				var proxy = GetExProxy<P14ExDataSource>();
				F140101List = proxy.CreateQuery<F140101Expansion>("GetDatas")
					.AddQueryOption("dcCode", string.Format("'{0}'", SelectedQueryDcCode))
					.AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
					.AddQueryOption("custCode", string.Format("'{0}'", CustCode))
					.AddQueryExOption("inventoryNo", QueryInventoryNo?.ToUpper())
					.AddQueryOption("inventorySDate",
						string.Format("'{0}'", ((QueryInventorySDate.HasValue) ? QueryInventorySDate.Value.ToString("yyyy/MM/dd") : "")))
					.AddQueryOption("inventoryEDate",
						string.Format("'{0}'", ((QueryInventoryEDate.HasValue) ? QueryInventoryEDate.Value.ToString("yyyy/MM/dd") : "")))
					.AddQueryOption("status", string.Format("'{0}'", SelectedStatus)).ToList();
				if (!F140101List.Any())
					ShowMessage(Messages.InfoNoData);
			}
			else
				ShowWarningMessage(message);
		}

		#endregion Search

		#region SearchDetail
		public ICommand SearchDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearchDetail(), () => UserOperateMode == OperateMode.Query && SelectedF140101 != null && !string.IsNullOrWhiteSpace(QueryDetailBegLocCode) && !string.IsNullOrWhiteSpace(QueryDetailEndLocCode)
						);
			}
		}

		/// <summary>
		/// 計算查詢回來的盤點詳細的數量
		/// </summary>
		/// <returns></returns>
		private int CountSearchDetail()
		{
			var proxyWcf = new wcf.P14WcfServiceClient();
			var result = RunWcfMethod<int>(proxyWcf.InnerChannel,
				() => proxyWcf.CountInventoryDetailItems(SelectedF140101.DC_CODE, SelectedF140101.GUP_CODE, SelectedF140101.CUST_CODE, SelectedF140101.INVENTORY_NO,
				UserOperateMode == OperateMode.Query ? SelectedQueryDetialWareHouseId : SelectedEditQueryDetailWareHouseId,
				UserOperateMode == OperateMode.Query ? QueryDetailBegLocCode.Trim() : EditQueryDetailBegLocCode.Trim(),
				UserOperateMode == OperateMode.Query ? QueryDetailEndLocCode.Trim() : EditQueryDetailEndLocCode.Trim(), ((!string.IsNullOrWhiteSpace(QueryDetailItemCode) ? QueryDetailItemCode.Trim() : ""))));

			return result;
		}


		private void DoSearchDetail()
		{
			QueryDetailBegLocCode = LocCodeHelper.LocCodeConverter9(QueryDetailBegLocCode).ToUpper();
			QueryDetailEndLocCode = LocCodeHelper.LocCodeConverter9(QueryDetailEndLocCode).ToUpper();
			string message;
			if (ValidateHelper.TryCheckBeginEndForLoc(this, x => x.QueryDetailBegLocCode, x => x.QueryDetailEndLocCode, Properties.Resources.LocCode,
				out message))
			{
				var count = CountSearchDetail();
				if (count > 500)
					ShowWarningMessage(string.Format(Properties.Resources.P1401010000_ImportSearchDetailCountHint, count));

				//執行查詢動
				var proxyEx = GetExProxy<P14ExDataSource>();

				InventoryDetailItemList = proxyEx.CreateQuery<InventoryDetailItem>("GetInventoryDetailItems")
					.AddQueryExOption("dcCode", SelectedF140101.DC_CODE)
					.AddQueryExOption("gupCode", SelectedF140101.GUP_CODE)
					.AddQueryExOption("custCode", SelectedF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO)
					.AddQueryExOption("wareHouseId", SelectedQueryDetialWareHouseId)
					.AddQueryExOption("begLocCode", QueryDetailBegLocCode.Trim())
					.AddQueryExOption("endLocCode", QueryDetailEndLocCode.Trim())
					.AddQueryExOption("itemCode", ((!string.IsNullOrWhiteSpace(QueryDetailItemCode) ? QueryDetailItemCode.Trim() : "")))
					.AddQueryExOption("procType", SelectedProcType)
          .ToList();
				if (!InventoryDetailItemList.Any())
					ShowMessage(Messages.InfoNoData);
			}
			else
				ShowWarningMessage(message);
		}

		#endregion

		#region SearchItem
		public ICommand SearchItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearchItem(), () => UserOperateMode != OperateMode.Query && AddOrEditF140101 != null && !string.IsNullOrEmpty(AddOrEditF140101.DC_CODE)
						);
			}
		}

		private void DoSearchItem()
		{
			var itemCode = string.IsNullOrWhiteSpace(AddItemCode) ? "" : AddItemCode.Trim();

			if ((AddOrEditF140101.INVENTORY_TYPE == "0" || AddOrEditF140101.INVENTORY_TYPE == "2") &&
				string.IsNullOrWhiteSpace(SelectedItemType) &&
				string.IsNullOrWhiteSpace(SelectedVnrCode) &&
        string.IsNullOrWhiteSpace(SelectedOriVnrCode) &&
        string.IsNullOrWhiteSpace(SelectedVnrName) &&
				string.IsNullOrWhiteSpace(SelectedItemLType) &&
				string.IsNullOrWhiteSpace(SelectedItemMType) &&
				string.IsNullOrWhiteSpace(SelectedItemSType) &&
				string.IsNullOrWhiteSpace(itemCode))
			{
				ShowWarningMessage("[類別、廠商編號、原廠商編號、廠商名稱、大類、中類、小類、品號、商品條碼] 盤點商品篩選至少選擇或輸入一項條件");
				return;
			}

			var list = InventoryItemList ?? new List<InventoryItem>();
			var proxyEx = GetExProxy<P14ExDataSource>();
			var data = proxyEx.CreateQuery<InventoryItem>("GetInventoryItems")
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("type", SelectedItemType)
				.AddQueryExOption("lType", SelectedItemLType)
				.AddQueryExOption("mType", SelectedItemMType)
				.AddQueryExOption("sType", SelectedItemSType)
				.AddQueryExOption("vnrCode", SelectedVnrCode)
        .AddQueryExOption("oriVnrCode", SelectedOriVnrCode)
        .AddQueryExOption("vnrName", SelectedVnrName)
				.AddQueryExOption("itemCode", itemCode).ToList();
			if (!data.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
			data = data.Where(c => list.All(o => o.ITEM_CODE != c.ITEM_CODE)).ToList();
			list.AddRange(data);
			InventoryItemList = list.ToList();
			IsCheckAllItem = false;
		}

		#endregion

		#region AddItem
		public ICommand AddItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAddItem(), () => UserOperateMode != OperateMode.Query && !string.IsNullOrWhiteSpace(AddItemCode) && AddHasFindItem,
						c => DoAddItemComplete()
						);
			}
		}

		private void DoAddItem()
		{

		}

		private void DoAddItemComplete()
		{
			AddItemCode = AddItemCode.ToUpper();
			var list = InventoryItemList ?? new List<InventoryItem>();
			if (list.Any(o => o.ITEM_CODE == AddItemCode))
			{
				ShowWarningMessage(Properties.Resources.P1401010000_ItemIsExistInInventoryItemList);
				return;
			}
			var inventoryItem = new InventoryItem
			{
				ITEM_CODE = AddItemCode,
				ITEM_NAME = AddItemName
			};
			list.Add(inventoryItem);
			InventoryItemList = list.ToList();
			IsCheckAllItem = false;
			ClearUcSearchProduct();

		}

		#endregion

		#region DeleteItem
		public ICommand DeleteItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDeleteItem(), () => UserOperateMode != OperateMode.Query && InventoryItemList != null && InventoryItemList.Any(o => o.IsSelected),
						c => DoDeleteItemComplete()
						);
			}
		}

		private void DoDeleteItem()
		{

		}

		private void DoDeleteItemComplete()
		{
			var list = InventoryItemList ?? new List<InventoryItem>();
			var removeItems = list.Where(o => o.IsSelected).ToList();
			foreach (var removeItem in removeItems)
				list.Remove(removeItem);
			InventoryItemList = list.ToList();
			IsCheckAllItem = false;
		}

		#endregion

		#region ExcelImportItem
		public ICommand ExcelImportItemCommand
		{
			get
			{
        return new RelayCommand<string>(o =>
        {
          DispatcherAction(() =>
          {
            ExcelImportItem(o);
            if (string.IsNullOrEmpty(ImportItemCodeFilePath))
              return;
            DoExcelImportItem(o);
          });
        });
      }
    }

		private void DoExcelImportItem()
		{
			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(ImportItemCodeFilePath, ref errorMeg, -1);
			if (!string.IsNullOrEmpty(errorMeg))
			{
				ImportItemCodeFilePath = null;
				ShowWarningMessage(errorMeg);
				return;
			}
			var list = InventoryItemList ?? new List<InventoryItem>();
			var itemCodeList = list.Select(o => o.ITEM_CODE).ToList();
			if (excelTable.Columns.Count == 0 || excelTable.Rows.Count == 0)
			{
				ImportItemCodeFilePath = null;
				ShowWarningMessage(Properties.Resources.P1401010000_ImportEmptyExcel);
				return;
			}
			if (excelTable.Columns.Count < 1)
			{
				ImportItemCodeFilePath = null;
				ShowWarningMessage(Properties.Resources.P1401010000_ImportErrorExcel);
				return;
			}
			if (itemCodeList.Any() && excelTable.Select(excelTable.Columns[0].ColumnName + " IN('" + string.Join("','", itemCodeList.ToArray()) + "')")
				.Any())
			{
				ImportItemCodeFilePath = null;
				ShowWarningMessage(Properties.Resources.P1401010000_ImportItemIsExistInInventoryItemList);
				return;
			}
      var checkDuplicateItem = excelTable.AsEnumerable().GroupBy(x => x[0].ToString()?.ToUpper()).Where(x => x.Count() > 1).Select(x => x.Key);
      if (checkDuplicateItem.Any())
      {
        ShowWarningMessage($"品號{string.Join(",", checkDuplicateItem)}重複");
        return;
      }

			var proxy = GetProxy<F19Entities>();
			var addList = new List<InventoryItem>();
			foreach (DataRow dataRow in excelTable.Rows)
			{
				var item = proxy.F1903s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.ITEM_CODE.ToUpper() == dataRow[0].ToString().ToUpper().Trim()).FirstOrDefault();
				if (item == null)
				{
					ShowWarningMessage(Properties.Resources.P1401010000_Customer + Wms3plSession.Get<GlobalInfo>().GupName + Properties.Resources.P1401010000_NoItemCode + dataRow[0].ToString() + "]");
					return;
				}
				addList.Add(new InventoryItem { ITEM_CODE = item.ITEM_CODE, ITEM_NAME = item.ITEM_NAME });
			}

			list.AddRange(addList);
			InventoryItemList = list.ToList();
			IsCheckAllItem = false;
			ShowMessage(Messages.InfoImportSuccess);
		}
    #endregion

    private void DoExcelImportItem(string buttonId)
    {
      if (!new[] { "BP1401010013", "BP1401010021" }.Contains(buttonId))
      {
        ShowWarningMessage("無法識別的按鈕編號");
        return;
      }


      var errorMeg = string.Empty;
      var excelTable = DataTableHelper.ReadExcelDataTable(ImportItemCodeFilePath, ref errorMeg, -1);
      if (!string.IsNullOrEmpty(errorMeg))
      {
        ImportItemCodeFilePath = null;
        ShowWarningMessage(errorMeg);
        return;
      }
      var list = InventoryItemList ?? new List<InventoryItem>();
      var itemCodeList = list.Select(o => o.ITEM_CODE).ToList();
      if (excelTable.Columns.Count == 0 || excelTable.Rows.Count == 0)
      {
        ImportItemCodeFilePath = null;
        ShowWarningMessage(Properties.Resources.P1401010000_ImportEmptyExcel);
        return;
      }
      if (excelTable.Columns.Count < 1)
      {
        ImportItemCodeFilePath = null;
        ShowWarningMessage(Properties.Resources.P1401010000_ImportErrorExcel);
        return;
      }
      //if (itemCodeList.Any() && excelTable.Select(excelTable.Columns[0].ColumnName + " IN('" + string.Join("','", itemCodeList.ToArray()) + "')")
      //  .Any())
      //{
      //  ImportItemCodeFilePath = null;
      //  ShowWarningMessage(Properties.Resources.P1401010000_ImportItemIsExistInInventoryItemList);
      //  return;
      //}
      var checkDuplicateItem = excelTable.AsEnumerable().GroupBy(x => x[0].ToString()?.ToUpper()).Where(x => x.Count() > 1).Select(x => x.Key);
      if (checkDuplicateItem.Any())
      {
        if (buttonId == "BP1401010013")
          ShowWarningMessage($"匯入的品編有重複：{string.Join(",", checkDuplicateItem)}");
        else if (buttonId == "BP1401010021")
          ShowWarningMessage($"匯入的貨主品編有重複：{string.Join(",", checkDuplicateItem)}");
        return;
      }

      var addCustItemList = excelTable.AsEnumerable().Select(x => x[0].ToString().ToUpper().Trim()).ToArray();

      if (buttonId == "BP1401010013")
      {
        var dupItemCode = list.Where(a => addCustItemList.Any(b => a.ITEM_CODE == b)).Select(x=>x.ITEM_CODE).Distinct();
        if (dupItemCode.Any())
        {
          ShowWarningMessage($"匯入的品編\r\n{string.Join("\r\n", dupItemCode)}\r\n所對應的商品品號，已存在盤點商品清單中");
          return;
        }
      }
      else if (buttonId == "BP1401010021")
      {
        var dupItemCode = list.Where(a => addCustItemList.Any(b => a.CUST_ITEM_CODE == b)).Select(x => x.CUST_ITEM_CODE).Distinct();
        if (dupItemCode.Any())
        {
          ShowWarningMessage($"匯入的貨主品編\r\n{string.Join("\r\n", dupItemCode)}\r\n所對應的商品品號，已存在盤點商品清單中");
          return;
        }
      }

      var proxy = GetWcfProxy<wcf.P14WcfServiceClient>();
      wcf.CheckInventoryItemRes result = new wcf.CheckInventoryItemRes();

      if (buttonId == "BP1401010013")
        result = proxy.RunWcfMethod(w => w.CheckInventoryItemExist(GupCode, CustCode, addCustItemList, "0"));
      else if (buttonId == "BP1401010021")
        result = proxy.RunWcfMethod(w => w.CheckInventoryItemExist(GupCode, CustCode, addCustItemList, "1"));

      if (!result.IsSuccessed)
      {
        ShowWarningMessage(result.Message);
        return;
      }

      list.AddRange(result.InventoryItems.MapCollection<wcf.InventoryItem, InventoryItem>());
      InventoryItemList = list.ToList();
      IsCheckAllItem = false;
      ShowMessage(Messages.InfoImportSuccess);
    }

    private void ClearImportList()
		{
			ImportList = null;
			ImportErrList = null;
			ImportData = null;
			ImportErrData = null;
			IsNotUsing = true;
			ImportLocCode = string.Empty;
			ImportType = ImportType.Excel;
			SelectedItemType = ItemTypeList.First().Value;
			SelectedItemLType = string.Empty;
			SelectedItemMType = string.Empty;
			SelectedItemSType = string.Empty;
			ClearUcSearchProduct();

		}

		#region SearchEditDetail
		public ICommand SearchEditDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearchEditDetail(), () => UserOperateMode != OperateMode.Query && AddOrEditF140101 != null && !string.IsNullOrWhiteSpace(EditQueryDetailBegLocCode) && !string.IsNullOrWhiteSpace(EditQueryDetailEndLocCode)
						);
			}
		}

		private void DoSearchEditDetail()
		{
			EditQueryDetailBegLocCode = LocCodeHelper.LocCodeConverter9(EditQueryDetailBegLocCode).ToUpper();
			EditQueryDetailEndLocCode = LocCodeHelper.LocCodeConverter9(EditQueryDetailEndLocCode).ToUpper();
			string message;
			if (ValidateHelper.TryCheckBeginEndForLoc(this, x => x.EditQueryDetailBegLocCode, x => x.EditQueryDetailEndLocCode, Properties.Resources.LocCode,
				out message))
			{
				if (EditInventoryDetailItemList != null && EditInventoryDetailItemList.Any(o => (o.FIRST_QTY.HasValue && o.FIRST_QTY < 0) || (o.SECOND_QTY.HasValue && o.SECOND_QTY < 0)))
				{
					if (AddOrEditF140101.STATUS == "0" || AddOrEditF140101.STATUS == "1")
						ShowWarningMessage(Properties.Resources.P1401010000_FirstInventoryDetailQtyNeedMoreThanZero);
					else
						ShowWarningMessage(Properties.Resources.P1401010000_PluralInventoryDetailQtyNeedMoreThanZero);
					return;
				}

				SaveTempInventoryDetailItems();

				var count = CountSearchDetail();
				if (count > 500)
					ShowWarningMessage(string.Format(Properties.Resources.P1401010000_ImportSearchDetailCountHint, count));
				//執行查詢動
				var proxyEx = GetExProxy<P14ExDataSource>();

				var list = proxyEx.CreateQuery<InventoryDetailItem>("GetInventoryDetailItems")
          .AddQueryExOption("dcCode", AddOrEditF140101.DC_CODE)
          .AddQueryExOption("gupCode", AddOrEditF140101.GUP_CODE)
          .AddQueryExOption("custCode", AddOrEditF140101.CUST_CODE)
          .AddQueryExOption("inventoryNo", AddOrEditF140101.INVENTORY_NO)
          .AddQueryExOption("wareHouseId", SelectedEditQueryDetailWareHouseId)
          .AddQueryExOption("begLocCode", EditQueryDetailBegLocCode.Trim())
          .AddQueryExOption("endLocCode", EditQueryDetailEndLocCode.Trim())
          .AddQueryExOption("itemCode", ((!string.IsNullOrWhiteSpace(EditQueryItemCode) ? EditQueryItemCode.Trim() : "")))
          .AddQueryExOption("procType", "")
          .ToList();
        if (!list.Any())
				{
					EditInventoryDetailItemList = list.ToList();
					ShowMessage(Messages.InfoNoData);
				}
				else
				{
					var tempList = _tempEditInventoryDetailItemList ?? new List<InventoryDetailItem>();
					var modifyTempList = tempList.Where(o => o.ChangeStatus == "E" || o.ChangeStatus == "D");
					foreach (var inventoryDetailItem in modifyTempList)
					{
						//Todo makeNo To Key 2019/05/06
						//板號、箱號、批號處理，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
						var boxCtrlNo = string.IsNullOrWhiteSpace(inventoryDetailItem.BOX_CTRL_NO) ? "0" : inventoryDetailItem.BOX_CTRL_NO;
						var makeNo = string.IsNullOrWhiteSpace(inventoryDetailItem.MAKE_NO) ? "0" : inventoryDetailItem.MAKE_NO;
						var palletCtrlNo = string.IsNullOrWhiteSpace(inventoryDetailItem.PALLET_CTRL_NO) ? "0" : inventoryDetailItem.PALLET_CTRL_NO;

						var item = list.FirstOrDefault(
							o =>
								o.ITEM_CODE == inventoryDetailItem.ITEM_CODE && o.LOC_CODE == inventoryDetailItem.LOC_CODE &&
								o.ENTER_DATE == inventoryDetailItem.ENTER_DATE && o.VALID_DATE == inventoryDetailItem.VALID_DATE &&
								o.MAKE_NO == makeNo && o.BOX_CTRL_NO == boxCtrlNo &&
								o.PALLET_CTRL_NO == palletCtrlNo);
						if (item != null)
						{
							item.ChangeStatus = inventoryDetailItem.ChangeStatus;
							item.FIRST_QTY = inventoryDetailItem.FIRST_QTY;
							item.SECOND_QTY = inventoryDetailItem.SECOND_QTY;
							item.FLUSHBACK = inventoryDetailItem.FLUSHBACK;
						}
					}
					var addItems = tempList.Where(
						o =>
							o.WAREHOUSE_ID == SelectedEditQueryDetailWareHouseId && o.LOC_CODE.CompareTo(EditQueryDetailBegLocCode.Trim()) >= 0 &&
							 o.LOC_CODE.CompareTo(EditQueryDetailEndLocCode.Trim()) <= 0 && o.ChangeStatus == "A");
					list.AddRange(addItems);
					EditInventoryDetailItemList = list.Where(o => o.ChangeStatus != "D").OrderBy(o => o.ITEM_CODE).ThenBy(o => o.LOC_CODE).ToList();
				}
			}
			else
				ShowWarningMessage(message);
		}

		#endregion

		#region AddInventoryDetailItem
		public ICommand AddInventoryDetailItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAddInventoryDetailItem(), () => UserOperateMode != OperateMode.Query && !string.IsNullOrWhiteSpace(EditItemCode) && EditHasFindItem && !string.IsNullOrWhiteSpace(EditLocCode) && !string.IsNullOrWhiteSpace(EditMakeNo) && EditQty >= 0 ,
						c => DoAddInventoryDetailItemComplete()
						);
			}
		}

		private void DoAddInventoryDetailItem()
		{

		}

		private void DoAddInventoryDetailItemComplete()
		{
			var proxy = GetProxy<F19Entities>();

			var list = EditInventoryDetailItemList ?? new List<InventoryDetailItem>();

			EditLocCode = LocCodeHelper.LocCodeConverter9(EditLocCode).ToUpper();

			//Todo makeNo To Key 2019/05/06
			//批號處理，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
			var makeNo = string.IsNullOrWhiteSpace(EditMakeNo) ? "0" : EditMakeNo.ToUpper();

			if (list.Any(o => o.ITEM_CODE == EditItemCode && o.VALID_DATE == EditValidDate && o.ENTER_DATE == DateTime.Today && o.LOC_CODE == EditLocCode && o.ChangeStatus != "D" &&
			o.BOX_CTRL_NO == "0" && o.PALLET_CTRL_NO == "0" && o.MAKE_NO == makeNo))
			{
				ShowWarningMessage(Properties.Resources.P1401010000_ExistData);
				return;
			}
			var tempList = _tempEditInventoryDetailItemList ?? new List<InventoryDetailItem>();
			if (tempList.Any(o => o.ITEM_CODE == EditItemCode && o.VALID_DATE == EditValidDate && o.ENTER_DATE == DateTime.Today && o.LOC_CODE == EditLocCode && o.ChangeStatus != "D" &&
			o.BOX_CTRL_NO == "0" && o.PALLET_CTRL_NO == "0" && o.MAKE_NO == makeNo))
			{
				ShowWarningMessage(Properties.Resources.P1401010000_ExistData);
				return;
			}

			if (!list.Any(o => o.ITEM_CODE == EditItemCode && o.VALID_DATE == EditValidDate && o.ENTER_DATE == DateTime.Today && o.LOC_CODE == EditLocCode && o.ChangeStatus == "D" &&
			o.BOX_CTRL_NO == "0" && o.PALLET_CTRL_NO == "0" && o.MAKE_NO == makeNo) &&
				!tempList.Any(o => o.ITEM_CODE == EditItemCode && o.VALID_DATE == EditValidDate && o.ENTER_DATE == DateTime.Today && o.LOC_CODE == EditLocCode && o.ChangeStatus == "D" &&
				o.BOX_CTRL_NO == "0" && o.PALLET_CTRL_NO == "0" && o.MAKE_NO == makeNo) &&
				FindInventoryDetailItems(EditLocCode, EditItemCode, DateTime.Today.ToString("yyyy/MM/dd"), EditValidDate.ToString("yyyy/MM/dd"), makeNo) != null)
			{
				ShowWarningMessage(Properties.Resources.P1401010000_ExistData);
				return;
			}
			if (!CheckInventoryLocItem(EditLocCode, EditItemCode)) //異動盤只能新增本次異動盤同儲位同商品的其他效期商品
			{
				ShowWarningMessage(Properties.Resources.P1401010000_InventoryLocItemNotValid);
				return;
			}

			// 盤點工具為人工新增必須是人工倉儲位;盤點工具為自動新增必須為自動倉儲位
			if(AddOrEditF140101.CHECK_TOOL == "0" && CheckAutoWarehouse())
			{
				ShowWarningMessage("人工盤點單必須輸入人工倉儲位");
				return;
			}
			else if(AddOrEditF140101.CHECK_TOOL != "0")
			{
				if (!CheckAutoWarehouse())
				{
					ShowWarningMessage("自動盤點單必須輸入自動倉儲位");
					return;
				}

				var proxyF14 = GetProxy<F14Entities>();
				List<string> locCodes;
				if (AddOrEditF140101.ISSECOND == "0")// 初盤
				{
					var f140104s = proxyF14.F140104s.Where(o => o.DC_CODE == AddOrEditF140101.DC_CODE && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.INVENTORY_NO == AddOrEditF140101.INVENTORY_NO).ToList();
					locCodes = f140104s.Select(x => x.LOC_CODE).Distinct().ToList();
				}
				else// 複盤
				{
					var f140105s = proxyF14.F140105s.Where(o => o.DC_CODE == AddOrEditF140101.DC_CODE && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.INVENTORY_NO == AddOrEditF140101.INVENTORY_NO).ToList();
					locCodes = f140105s.Select(x => x.LOC_CODE).Distinct().ToList();
				}

				if (locCodes.Any() && !locCodes.Contains(EditLocCode))
				{
					ShowWarningMessage("儲位非此自動倉盤點單選擇的儲位");
					return;
				}
			}

			//檢查儲位
			var result = CheckLocCode(AddOrEditF140101.DC_CODE, EditLocCode.Trim(), EditItemCode.Trim());
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return;
			}
			string wareHouseId = result.Message;

			//取得庫存數
			var qty =
				proxy.F1913s.Where(
					o =>
						o.DC_CODE == AddOrEditF140101.DC_CODE && o.GUP_CODE == AddOrEditF140101.GUP_CODE &&
						o.CUST_CODE == AddOrEditF140101.CUST_CODE && o.LOC_CODE == EditLocCode && o.ITEM_CODE == EditItemCode && o.VALID_DATE == EditValidDate && o.ENTER_DATE == DateTime.Today && o.QTY > 0 &&
						o.BOX_CTRL_NO == "0" && o.PALLET_CTRL_NO == "0" && o.MAKE_NO == makeNo)
					.ToList()
					.Sum(o => o.QTY);
			var f1980 = proxy.F1980s.Where(o => o.WAREHOUSE_ID == wareHouseId && o.DC_CODE == AddOrEditF140101.DC_CODE).First();
			var inventoryDetailItem = new InventoryDetailItem()
			{
				ChangeStatus = "A",
				ITEM_CODE = EditItemCode,
				ITEM_NAME = EditItemName,
				ITEM_COLOR = EditItemColor,
				ITEM_SIZE = EditItemSize,
				ITEM_SPEC = EditItemSpec,
				ENTER_DATE = DateTime.Today,
				VALID_DATE = EditValidDate,
				LOC_CODE = EditLocCode,
				QTY = (int)qty,
				FLUSHBACK_ORG = "0",
				FLUSHBACK = "0",
				WAREHOUSE_ID = f1980.WAREHOUSE_ID,
				WAREHOUSE_NAME = f1980.WAREHOUSE_NAME,
				BOX_CTRL_NO = "0",
				PALLET_CTRL_NO = "0",
				MAKE_NO = makeNo,
				DEVICE_STOCK_QTY = AddOrEditF140101.CHECK_TOOL == "0" ? default(int?) : 0,
				UNMOVE_STOCK_QTY = 0,
				CUST_ITEM_CODE = EditCustItemCode,
				EAN_CODE1 = EditEanCode1,
				EAN_CODE2 = EditEanCode2,
				EAN_CODE3 = EditEanCode3
			};
			if (AddOrEditF140101.STATUS == "1")
			{
				inventoryDetailItem.FIRST_QTY_ORG = EditQty;
				inventoryDetailItem.FIRST_QTY = EditQty;
			}
			if (AddOrEditF140101.STATUS == "2")
			{
				inventoryDetailItem.SECOND_QTY_ORG = EditQty;
				inventoryDetailItem.SECOND_QTY = EditQty;
			}

			if (inventoryDetailItem.FIRST_QTY_ORG == 0 && EditQty != 0)
				inventoryDetailItem.FIRST_QTY_ORG = null;
			if (inventoryDetailItem.FIRST_QTY == 0 && EditQty != 0)
				inventoryDetailItem.FIRST_QTY = null;
			if (inventoryDetailItem.SECOND_QTY_ORG == 0 && EditQty != 0)
				inventoryDetailItem.SECOND_QTY_ORG = null;
			if (inventoryDetailItem.SECOND_QTY == 0 && EditQty != 0)
				inventoryDetailItem.SECOND_QTY = null;
			list.Add(inventoryDetailItem);
			tempList.Add(inventoryDetailItem);
			_tempEditInventoryDetailItemList = tempList;
			EditInventoryDetailItemList = list.ToList();
			IsCheckAllEditInventoryDetailItem = false;
			EditItemCode = null;
			EditItemName = null;
			EditItemSize = null;
			EditItemColor = null;
			EditItemSpec = null;
			EditSerialNo = null;
			EditHasFindItem = false;
			EditQty = 0;
			EditLocCode = null;
			EditValidDate = DateTime.Parse("9999/12/31").Date;
			EditMakeNo = null;
			EditCustItemCode = null;
			EditEanCode1 = null;
			EditEanCode2 = null;
			EditEanCode3 = null;
			SelectedEditInventoryDetailItem = inventoryDetailItem;
			DgScorllInView();
		}

		#endregion

		#region DeleteInventoryDetailItem
		public ICommand DeleteInventoryDetailItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDeleteInventoryDetailItem(), () => UserOperateMode != OperateMode.Query && AddOrEditF140101 != null && AddOrEditF140101.STATUS == "0" && EditInventoryDetailItemList != null && EditInventoryDetailItemList.Any(o => o.IsSelected),
						c => DoDeleteInventoryDetailItemComplete()
						);
			}
		}

		private void DoDeleteInventoryDetailItem()
		{

		}

		private void DoDeleteInventoryDetailItemComplete()
		{
			var list = EditInventoryDetailItemList ?? new List<InventoryDetailItem>();
			var removeItems = list.Where(o => o.IsSelected).ToList();
			var tempList = _tempEditInventoryDetailItemList ?? new List<InventoryDetailItem>();
			foreach (var removeItem in removeItems)
			{
				var item = tempList.FirstOrDefault(o => o.ITEM_CODE == removeItem.ITEM_CODE && o.VALID_DATE == removeItem.VALID_DATE && o.ENTER_DATE == removeItem.ENTER_DATE && o.LOC_CODE == removeItem.LOC_CODE && o.ChangeStatus != "D" &&
				o.BOX_CTRL_NO == removeItem.BOX_CTRL_NO && o.PALLET_CTRL_NO == removeItem.PALLET_CTRL_NO && o.MAKE_NO == removeItem.MAKE_NO);
				if (item != null)
				{
					if (item.ChangeStatus != "A")
						item.ChangeStatus = "D";
					else
						tempList.Remove(item);
				}
				else
				{
					removeItem.ChangeStatus = "D";
					tempList.Add(removeItem);
				}
				list.Remove(removeItem);
			}
			_tempEditInventoryDetailItemList = tempList;
			EditInventoryDetailItemList = list.ToList();
			IsCheckAllEditInventoryDetailItem = false;
		}

		#endregion

		#region ExcelImportInventoryDetailItem
		public ICommand ExcelImportInventoryDetailItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoExcelImportInventoryDetailItem(), () => UserOperateMode != OperateMode.Query
						);
			}
		}

		private void DoExcelImportInventoryDetailItem()
		{
			try
			{
				var errorMeg = string.Empty;
				var excelTable = DataTableHelper.ReadExcelDataTable(ImportInventoryDetailItemFilePath, ref errorMeg);
				if (!string.IsNullOrEmpty(errorMeg))
				{
					ImportInventoryDetailItemFilePath = null;
					ShowWarningMessage(errorMeg);
					return;
				}
				var list = EditInventoryDetailItemList ?? new List<InventoryDetailItem>();
				if (excelTable.Columns.Count == 0 || excelTable.Rows.Count < 1)
				{
					ImportItemCodeFilePath = null;
					ShowWarningMessage(Properties.Resources.P1401010000_ImportEmptyExcel);
					return;
				}
				if (excelTable.Columns.Count < 17)
				{
					ImportItemCodeFilePath = null;
					ShowWarningMessage(Properties.Resources.P1401010000_ImportErrorExcel);
					return;
				}

				var aaaa = excelTable.Rows.Cast<DataRow>().Select((r, i) => new { Row = r, Index = i });


				var importDetail = (from import in excelTable.Rows.Cast<DataRow>().Select((r, i) => new { Row = r, Index = i })
														select new wcf.ImportInventoryDetailItem
														{
															ROWNUMk__BackingField = import.Index,
															ItemCodek__BackingField = import.Row[1]?.ToString().Trim(),
															CUST_ITEM_CODEk__BackingField = import.Row[2]?.ToString().Trim(),
															EAN_CODE1k__BackingField = import.Row[3]?.ToString().Trim(),
															EAN_CODE2k__BackingField = import.Row[4]?.ToString().Trim(),
															EAN_CODE3k__BackingField = import.Row[5]?.ToString().Trim(),
															ITEM_NAMEk__BackingField = import.Row[6]?.ToString().Trim(),
															ValidDatek__BackingField = import.Row[7]?.ToString().Trim(),
															MAKE_NOk__BackingField = import.Row[8]?.ToString().Trim(),
															EnterDatek__BackingField = import.Row[9]?.ToString().Trim(),
															WAREHOUSE_NAMEk__BackingField = import.Row[10]?.ToString().Trim(),
															LocCodek__BackingField = import.Row[11]?.ToString().Trim(),
															QTYk__BackingField = import.Row[12]?.ToString().Trim(),
															UNMOVE_STOCK_QTYk__BackingField = import.Row[13]?.ToString().Trim(),
															DEVICE_STOCK_QTYk__BackingField = import.Row[14]?.ToString().Trim(),
															FIRST_QTYk__BackingField = import.Row[15]?.ToString().Trim(),
															SECOND_QTYk__BackingField = import.Row[16]?.ToString().Trim()
														}).ToArray();

                if (importDetail.Any(o => string.IsNullOrWhiteSpace(o.MAKE_NOk__BackingField)))
                {
                    ImportItemCodeFilePath = null;
                    ShowWarningMessage(Properties.Resources.P1401020000_MakeNoEmpty);
                    return;
                }
                else if (importDetail.Any(o => o.MAKE_NOk__BackingField.Length > 40))
                {
                    ImportItemCodeFilePath = null;
                    ShowWarningMessage(Properties.Resources.P1401020000_MakeNoLengthOver);
                    return;
                }

                var checkRepeatData = importDetail.GroupBy(o => new {
					ItemCodek__BackingField = o.ItemCodek__BackingField.ToUpper(),
					LocCodek__BackingField = o.LocCodek__BackingField.ToUpper(),
					MAKE_NOk__BackingField=o.MAKE_NOk__BackingField.ToUpper(),
					o.ValidDatek__BackingField,
					o.EnterDatek__BackingField
				}).Select(o => new { o.Key, repeat = o.Count() }).ToList();
				if (checkRepeatData.Any(o => o.repeat > 1))
				{
					ShowWarningMessage(Properties.Resources.P140101_ImportRepeatData);
					return;
				}
				var proxyWcf = new wcf.P14WcfServiceClient();
				var result = RunWcfMethod<wcf.ImportInventoryDetailResult>(proxyWcf.InnerChannel,
						() => proxyWcf.ImportInventoryDetailItems(AddOrEditF140101.DC_CODE, AddOrEditF140101.GUP_CODE, AddOrEditF140101.CUST_CODE, AddOrEditF140101.INVENTORY_NO, importDetail));

				if (result != null)
				{
					if (result.InventoryDetailItems == null)
						result.InventoryDetailItems = new List<wcf.InventoryDetailItem>().ToArray();

					if (!result.Result.IsSuccessed)
						ShowWarningMessage(result.Result.Message);
					else
					{
						var data = ExDataMapper.MapCollection<wcf.InventoryDetailItem, InventoryDetailItem>(result.InventoryDetailItems);
						var totalCnt = importDetail.Length;
						var successCnt = data.Count();
						var failureCnt = totalCnt - successCnt;
						var addCnt = 0;
						EditInventoryDetailItemList = data.ToList();
						var tempList = _tempEditInventoryDetailItemList ?? new List<InventoryDetailItem>();
						var addData = data.Where(x => x.ChangeStatus == "A").ToList();
						foreach (var item in addData)
						{
							//Todo makeNo To Key 2019/05/06
							//板號、箱號、批號處理，如果是空值塞"0";批號如果是空值或""通通都塞null，否則塞值
							var boxCtrlNo = string.IsNullOrWhiteSpace(item.BOX_CTRL_NO) ? "0" : item.BOX_CTRL_NO;
							var makeNo = string.IsNullOrWhiteSpace(item.MAKE_NO) ? "0" : item.MAKE_NO;
							var palletCtrlNo = string.IsNullOrWhiteSpace(item.PALLET_CTRL_NO) ? "0" : item.PALLET_CTRL_NO;


							//如果不存在資料庫也不存在Client暫存才代表新增 否則都為更新
							if (!tempList.Any(x => x.LOC_CODE == item.LOC_CODE && x.ITEM_CODE == item.ITEM_CODE && x.VALID_DATE == item.VALID_DATE && x.ENTER_DATE == item.ENTER_DATE && x.ChangeStatus != "D" && x.MAKE_NO == makeNo && x.BOX_CTRL_NO == boxCtrlNo && x.PALLET_CTRL_NO == palletCtrlNo))
								addCnt++;
						}
						SaveTempInventoryDetailItems();
						ShowInfoMessage(string.Format(Properties.Resources.P1401010000_ImportSuccessDetail, successCnt, addCnt, successCnt - addCnt, failureCnt, totalCnt));

						//匯出失敗的Excel
						if (result.FailDetailItems.Any())
						{
							var saveFileDialog = new SaveFileDialog();
							saveFileDialog.DefaultExt = ".xlsx";
							saveFileDialog.Filter = "Excel (.xlsx)|*.xlsx";
							saveFileDialog.RestoreDirectory = true;
							saveFileDialog.OverwritePrompt = true;
							saveFileDialog.Title = Properties.Resources.ExportFailDataDetail;
							saveFileDialog.FileName = string.Format("{0}.xlsx", Properties.Resources.P1401010000_ExcelName);
							bool? isShowOk;

							isShowOk = saveFileDialog.ShowDialog();
							if (isShowOk ?? false)
							{
								var excelExportService = new ExcelExportService();
								excelExportService.CreateNewSheet(Properties.Resources.P1401010000_InventoryDetail + SelectedF140101.INVENTORY_NO);

								var failData = result.FailDetailItems.ToDataTable();

								var showColumnName = new List<string>
										{
												"ROWNUMk__BackingField",
												"ItemCodek__BackingField",
												"CUST_ITEM_CODEk__BackingField",
												"EAN_CODE1k__BackingField",
												"EAN_CODE2k__BackingField",
												"EAN_CODE3k__BackingField",
												"ITEM_NAMEk__BackingField",
												"ValidDatek__BackingField",
												"MAKE_NOk__BackingField",
												"EnterDatek__BackingField",
												"WAREHOUSE_NAMEk__BackingField",
												"LocCodek__BackingField",
												"QTYk__BackingField",
												"UNMOVE_STOCK_QTYk__BackingField",
												"DEVICE_STOCK_QTYk__BackingField",
												"FIRST_QTYk__BackingField",
												"SECOND_QTYk__BackingField",
												"FailMessagek__BackingField"
										};
								var delColumnList = (from DataColumn column in failData.Columns where !showColumnName.Contains(column.ColumnName) select column.ColumnName).ToList();
								foreach (var columnName in delColumnList)
									failData.Columns.Remove(columnName);
								failData.Columns["ROWNUMk__BackingField"].SetOrdinal(0);
								failData.Columns["ROWNUMk__BackingField"].ColumnName = Resources.Resources.ItemNumber;
								failData.Columns["ItemCodek__BackingField"].SetOrdinal(1);
								failData.Columns["ItemCodek__BackingField"].ColumnName = Properties.Resources.ITEM_CODE;
								failData.Columns["CUST_ITEM_CODEk__BackingField"].SetOrdinal(2);
								failData.Columns["CUST_ITEM_CODEk__BackingField"].ColumnName = "貨主品編";
								failData.Columns["EAN_CODE1k__BackingField"].SetOrdinal(3);
								failData.Columns["EAN_CODE1k__BackingField"].ColumnName = "國條";
								failData.Columns["EAN_CODE2k__BackingField"].SetOrdinal(4);
								failData.Columns["EAN_CODE2k__BackingField"].ColumnName = "條碼二";
								failData.Columns["EAN_CODE3k__BackingField"].SetOrdinal(5);
								failData.Columns["EAN_CODE3k__BackingField"].ColumnName = "條碼三";
								failData.Columns["ITEM_NAMEk__BackingField"].SetOrdinal(6);
								failData.Columns["ITEM_NAMEk__BackingField"].ColumnName = Properties.Resources.ITEM_NAME;
								failData.Columns["ValidDatek__BackingField"].SetOrdinal(7);
								failData.Columns["ValidDatek__BackingField"].ColumnName = Properties.Resources.VALID_DATE;
								failData.Columns["MAKE_NOk__BackingField"].SetOrdinal(8);
								failData.Columns["MAKE_NOk__BackingField"].ColumnName = Properties.Resources.MAKE_NO;
								failData.Columns["EnterDatek__BackingField"].SetOrdinal(9);
								failData.Columns["EnterDatek__BackingField"].ColumnName = Properties.Resources.EnterDate;
								failData.Columns["WAREHOUSE_NAMEk__BackingField"].SetOrdinal(10);
								failData.Columns["WAREHOUSE_NAMEk__BackingField"].ColumnName = Properties.Resources.QueryDetialWareHouseId;
								failData.Columns["LocCodek__BackingField"].SetOrdinal(11);
								failData.Columns["LocCodek__BackingField"].ColumnName = Properties.Resources.LocCode;
								failData.Columns["QTYk__BackingField"].SetOrdinal(12);
								failData.Columns["QTYk__BackingField"].ColumnName = "WMS庫存數";
								failData.Columns["UNMOVE_STOCK_QTYk__BackingField"].SetOrdinal(13);
								failData.Columns["UNMOVE_STOCK_QTYk__BackingField"].ColumnName = "WMS未搬動數";
								failData.Columns["DEVICE_STOCK_QTYk__BackingField"].SetOrdinal(14);
								failData.Columns["DEVICE_STOCK_QTYk__BackingField"].ColumnName = "自動倉庫存數";
								failData.Columns["FIRST_QTYk__BackingField"].SetOrdinal(15);
								failData.Columns["FIRST_QTYk__BackingField"].ColumnName = Properties.Resources.FIRST_QTY;
								failData.Columns["SECOND_QTYk__BackingField"].SetOrdinal(16);
								failData.Columns["SECOND_QTYk__BackingField"].ColumnName = Properties.Resources.SECOND_QTY;
								failData.Columns["FailMessagek__BackingField"].SetOrdinal(17);
								failData.Columns["FailMessagek__BackingField"].ColumnName = Properties.Resources.FailMessage;
								var excelExportSource = new ExcelExportReportSource
								{
									Data = failData,
									DataFormatList =
												new List<NameValuePair<string>>
												{
														new NameValuePair<string>() {Name = Properties.Resources.VALID_DATE, Value = "yyyy/m/d"},
														new NameValuePair<string>() {Name = Properties.Resources.EnterDate, Value = "yyyy/m/d"}
												}
								};
								excelExportService.AddExportReportSource(excelExportSource);
								bool isExportSuccess = excelExportService.Export(Path.GetDirectoryName(saveFileDialog.FileName),
										Path.GetFileName(saveFileDialog.FileName));
								var message = new MessagesStruct
								{
									Button = DialogButton.OK,
									Image = DialogImage.Information,
									Message = "",
									Title = Resources.Resources.Information
								};
								message.Message = isExportSuccess ? Properties.Resources.P1401010000_ExportInventoryDetailFailDataSuccess : Properties.Resources.P1401010000_ExportInventoryDetailFailDataFail;
								if (!isExportSuccess)
									message.Image = DialogImage.Warning;
								ShowMessage(message);
							}
						}
						//end
					}
				}
			}
			catch (Exception ex)
			{
				var errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P1401010000xamlcs_ImportFail, true);
				ShowWarningMessage(errorMsg);
			}

		}
		#endregion

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAdd(), () => UserOperateMode == OperateMode.Query,
						c => DoAddComplete()
						);
			}
		}

		private void DoAdd()
		{
			//執行新增動作
		}

		private void DoAddComplete()
		{
			SelectedVnrCode = string.Empty;
			SelectedVnrName = string.Empty;
			UserOperateMode = OperateMode.Add;
			CanCheckIsCharge = true;
			ReadOnlyInventoryCycle = false;
			AddOrEditF140101 = new F140101
			{
				INVENTORY_DATE = DateTime.Today,
				INVENTORY_YEAR = (short?)DateTime.Now.Year,
				SHOW_CNT = "0",
				GUP_CODE = GupCode,
				CUST_CODE = CustCode,
				INVENTORY_NO = "-1",
				CHECK_TOOL = CheckToolList.FirstOrDefault().Value
			};
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedF140101 != null && (SelectedF140101.STATUS == "0" || SelectedF140101.STATUS == "1" || SelectedF140101.STATUS == "2"),
						c => DoEditComplete()
						);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
		}

		private void DoEditComplete()
		{
			var bkItem = ExDataMapper.Clone(SelectedF140101);
			DoSearch();
			SelectedF140101 = F140101List.Find(o => o.DC_CODE == bkItem.DC_CODE && o.GUP_CODE == bkItem.GUP_CODE && o.CUST_CODE == bkItem.CUST_CODE && o.INVENTORY_NO == bkItem.INVENTORY_NO);
			UserOperateMode = OperateMode.Edit;
			CanCheckIsCharge = true;
			AddOrEditF140101 = ExDataMapper.Map<F140101Expansion, F140101>(SelectedF140101);
			SetInventoryName();
			if (AddOrEditF140101.STATUS == "1" || AddOrEditF140101.STATUS == "2")
			{
				var proxy = GetProxy<F19Entities>();
				var item = proxy.F1909s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode).First();
				CustReadOnlyFlushBack = item.FLUSHBACK == "0";
			}
			else
				CustReadOnlyFlushBack = true;
			IsReadOnlyFirstQty = AddOrEditF140101.STATUS != "1";
			IsReadOnlySecondQty = _addOrEditF140101.STATUS != "2";
			SetIsCanCharge();
			SelectedEditQueryDetailWareHouseId = null;
			EditQueryDetailBegLocCode = null;
			EditQueryDetailEndLocCode = null;
			EditQueryItemCode = null;
			EditQueryItemName = null;
			EditQueryItemSize = null;
			EditQueryItemColor = null;
			EditQueryItemSpec = null;
			EditQueryHasFindItem = false;
			EditItemCode = null;
			EditItemName = null;
			EditItemSize = null;
			EditItemColor = null;
			EditItemSpec = null;
			EditSerialNo = null;
			EditHasFindItem = false;
			EditLocCode = null;
			EditMakeNo = null;
			EditQty = 0;
			EditValidDate = DateTime.Parse("9999/12/31").Date;
			IsShowQueryDetailRule = false;

		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCancel(), () => UserOperateMode != OperateMode.Query
						);
			}
		}

		private void DoCancel()
		{
			if (UserOperateMode == OperateMode.Edit && ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.No)
			{
				return;
			}
			//執行取消動作
			AddOrEditF140101 = null;
			ImportList = null;
			IsNotUsing = true;
			ImportType = ImportType.Excel;
			InventoryWareHouseList = null;
			InventoryItemList = null;
			EditInventoryDetailItemList = null;
			_tempEditInventoryDetailItemList = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete

		private bool _isDeleteOk;
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedF140101 != null && ((SelectedF140101.CHECK_TOOL == "0" && (SelectedF140101.STATUS != "5" && SelectedF140101.STATUS != "9")) || (SelectedF140101.CHECK_TOOL != "0" && SelectedF140101.STATUS == "0")),
						c => DoDeleteComplete()
						);
			}
		}

		private void DoDelete()
		{
			_isDeleteOk = false;
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				var proxyEx = GetExProxy<P14ExDataSource>();
				var result = proxyEx.CreateQuery<ExecuteResult>("DeleteF140101")
					.AddQueryOption("dcCode", string.Format("'{0}'", SelectedF140101.DC_CODE))
					.AddQueryOption("gupCode", string.Format("'{0}'", SelectedF140101.GUP_CODE))
					.AddQueryOption("custCode", string.Format("'{0}'", SelectedF140101.CUST_CODE))
					.AddQueryOption("checkTool", string.Format("'{0}'", SelectedF140101.CHECK_TOOL))
					.AddQueryOption("inventoryNo", string.Format("'{0}'", SelectedF140101.INVENTORY_NO)).ToList();

				var res = result.First();
				ShowMessage(res.IsSuccessed ? Messages.Success : new MessagesStruct { Message = res.Message });
				if (res.IsSuccessed)
					_isDeleteOk = true;
			}
		}

		private void DoDeleteComplete()
		{
			if (_isDeleteOk)
				SearchCommand.Execute(null);
		}
		#endregion Delete

		#region Save

		private bool _isSaveOk;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSave(), () => UserOperateMode != OperateMode.Query && _isSave,
						c => DoSaveComplete()
						);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			_isSaveOk = false;
			if (AddOrEditF140101.ISCHARGE == "1" && !AddOrEditF140101.FEE.HasValue)
			{
				ShowWarningMessage(Properties.Resources.P1401010000_checkCount);
				return;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				if (AddOrEditF140101.INVENTORY_TYPE == "0" && (InventoryItemList == null || (InventoryItemList != null && !InventoryItemList.Any())))
				{
					ShowWarningMessage("商品抽盤，盤點商品至少輸入一個品號");
					return;
				}

				if (AddOrEditF140101.INVENTORY_TYPE == "0" || AddOrEditF140101.INVENTORY_TYPE == "1" ||
						AddOrEditF140101.INVENTORY_TYPE == "2" || AddOrEditF140101.INVENTORY_TYPE == "3" || AddOrEditF140101.INVENTORY_TYPE == "4")
				{
					if (!InventoryWareHouseList.Any(o => o.IsSelected))
					{
						ShowWarningMessage(Properties.Resources.P1401010000_AtleastOneInventoryWareHouseData);
						return;
					}
					if (AddOrEditF140101.INVENTORY_TYPE == "1")
					{
						if (!AddOrEditF140101.INVENTORY_CYCLE.HasValue)
						{
							ShowWarningMessage(Properties.Resources.P1401010000_NeedCycle);
							return;
						}
						if (!AddOrEditF140101.CYCLE_TIMES.HasValue)
						{
							ShowWarningMessage(Properties.Resources.P1401010000_NeedCycleTimes);
							return;
						}
						if (AddOrEditF140101.CYCLE_TIMES > AddOrEditF140101.INVENTORY_CYCLE)
						{
							ShowWarningMessage(Properties.Resources.P1401010000_CycleTimesLessEqualThanCycle);
							return;
						}
						if (InventoryWareHouseList.Any(o => o.IsSelected && (string.IsNullOrEmpty(o.FLOOR_BEGIN) || string.IsNullOrEmpty(o.FLOOR_END))))
						{
							ShowWarningMessage(Properties.Resources.P1401010000_NeedSetFloorBegin);
							return;
						}
						if (
							InventoryWareHouseList.Any(
								o => o.IsSelected && (string.IsNullOrEmpty(o.CHANNEL_BEGIN) || string.IsNullOrEmpty(o.CHANNEL_END))))
						{
							ShowWarningMessage(Properties.Resources.P1401010000_NeedSetChannelBegin);
							return;
						}
						if (InventoryWareHouseList.Any(o => o.IsSelected && (string.IsNullOrEmpty(o.PLAIN_BEGIN) || string.IsNullOrEmpty(o.PLAIN_END))))
						{
							ShowWarningMessage(Properties.Resources.P1401010000_NeedSetPlainBegin);
							return;
						}
					}
					if (AddOrEditF140101.INVENTORY_TYPE == "3" || AddOrEditF140101.INVENTORY_TYPE == "4")
					{
						if (InventoryWareHouseList.Any(o => o.IsSelected && (string.IsNullOrEmpty(o.FLOOR_BEGIN) || string.IsNullOrEmpty(o.FLOOR_END))))
						{
							ShowWarningMessage(Properties.Resources.P1401010000_NeedSetFloorBegin);
							return;
						}
						if (
							InventoryWareHouseList.Any(
								o => o.IsSelected && (string.IsNullOrEmpty(o.CHANNEL_BEGIN) || string.IsNullOrEmpty(o.CHANNEL_END))))
						{
							ShowWarningMessage(Properties.Resources.P1401010000_NeedSetChannelBegin);
							return;
						}
						if (InventoryWareHouseList.Any(o => o.IsSelected && (string.IsNullOrEmpty(o.PLAIN_BEGIN) || string.IsNullOrEmpty(o.PLAIN_END))))
						{
							ShowWarningMessage(Properties.Resources.P1401010000_NeedSetPlainBegin);
							return;
						}
					}
				}

				var wcfF140101 = ExDataMapper.Map<F140101, wcf.F140101>(AddOrEditF140101);
				var inventoryWareHouseList = new List<InventoryWareHouse>();

				//全盤、半年盤改成需要設置倉別、盤點樓層、通道、座了，所以跟其他的處理方式都一樣_2019/03/19
				inventoryWareHouseList = InventoryWareHouseList.Where(o => o.IsSelected).ToList();
				var proxyWcf = new wcf.P14WcfServiceClient();
				var result = new wcf.ExecuteResult();

				var wcfInventoryWareHouseList =
					ExDataMapper.MapCollection<InventoryWareHouse, wcf.InventoryWareHouse>(inventoryWareHouseList);
				var wcfInventroyItemList = ExDataMapper.MapCollection<InventoryItem, wcf.InventoryItem>(InventoryItemList ?? new List<InventoryItem>());
				result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
				() => proxyWcf.InsertP140101(wcfF140101, wcfInventoryWareHouseList.ToArray(), wcfInventroyItemList.ToArray()));

				if (result.IsSuccessed)
				{
					var message = new MessagesStruct
					{
						Button = DialogButton.OK,
						Image = DialogImage.Information,
						Message = string.Format(Properties.Resources.P1401010000_InsertSuccess, result.Message),
						Title = Resources.Resources.Information
					};
					ShowMessage(message);

					if(AddOrEditF140101.CHECK_TOOL == "0")
						QueryInventoryNo = result.Message;

					_isSaveOk = true;
				}
				else
					ShowWarningMessage(Properties.Resources.P1401010000_InsertFail + Environment.NewLine + result.Message);
			}
			else
			{
        if(AddOrEditF140101.CHECK_TOOL!="0")
        {
          ShowWarningMessage("非人工倉不可使用盤點單單維護輸入盤點結果");
          return;
        }
				if (EditInventoryDetailItemList != null && EditInventoryDetailItemList.Any(o => (o.FIRST_QTY.HasValue && o.FIRST_QTY < 0) || (o.SECOND_QTY.HasValue && o.SECOND_QTY < 0)))
				{
					if (AddOrEditF140101.STATUS == "0" || AddOrEditF140101.STATUS == "1")
						ShowWarningMessage(Properties.Resources.P1401010000_FirstInventoryDetailQtyNeedMoreThanZero);
					else
						ShowWarningMessage(Properties.Resources.P1401010000_PluralInventoryDetailQtyNeedMoreThanZero);
					return;
				}
				SaveTempInventoryDetailItems();
				var wcfF140101 = ExDataMapper.Map<F140101, wcf.F140101>(AddOrEditF140101);
				//所有明細異動紀錄
				var wcfInventoryDetailItemList =
					ExDataMapper.MapCollection<InventoryDetailItem, wcf.InventoryDetailItem>(_tempEditInventoryDetailItemList ?? new List<InventoryDetailItem>());
				var proxyWcf = new wcf.P14WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
					() => proxyWcf.UpdateP140101(wcfF140101, wcfInventoryDetailItemList.ToArray(), ReadRdpClientSessionInfo.GetRdpClientName()));
				if (result.IsSuccessed)
				{
					ShowMessage(Messages.Success);
					QueryInventoryNo = AddOrEditF140101.INVENTORY_NO;
					_isSaveOk = true;
				}
				else
					ShowWarningMessage(Properties.Resources.P1401010000_UpdateFail + Environment.NewLine + result.Message);
			}
		}

		private void DoSaveComplete()
		{
			if (_isSaveOk)
			{
				SelectedStatus = "";
				SelectedQueryDcCode = AddOrEditF140101.DC_CODE;
				QueryInventorySDate = AddOrEditF140101.INVENTORY_DATE;
				QueryInventoryEDate = AddOrEditF140101.INVENTORY_DATE;
				SearchCommand.Execute(null);
				UserOperateMode = OperateMode.Query;
				AddOrEditF140101 = null;
				InventoryWareHouseList = null;
				InventoryItemList = null;
				EditInventoryDetailItemList = null;
				_tempEditInventoryDetailItemList = null;
				EditInventoryDetailItemList = null;
				AddItemCode = null;
				AddItemName = null;
				ImportList = null;
				IsNotUsing = true;
				MTypeIsEnabled = false;
				STypeIsEnabled = false;
				ImportType = ImportType.Excel;
        SelectedOriVnrCode = null;
      }
		}
		#endregion Save

		#region Print
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
						DoPrint,
						(t) => !IsBusy
								&& UserOperateMode == OperateMode.Query
								&& SelectedF140101 != null
							//	&& SelectedF140101.CHECK_TOOL == "0"
				);
			}
		}

		private void DoPrint(PrintType printType)
		{
			var proxyEx = GetExProxy<P14ExDataSource>();
			switch (SelectedReportType)
			{
				case "01"://盤點單
					ReportDataList = proxyEx.CreateQuery<InventoryDetailItemsByIsSecond>("GetReportData")
						.AddQueryExOption("dcCode", SelectedF140101.DC_CODE)
						.AddQueryExOption("gupCode", SelectedF140101.GUP_CODE)
						.AddQueryExOption("custCode", SelectedF140101.CUST_CODE)
						.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO).ToList();
					break;
				case "02"://盤點清冊
					ReportDataList2 = proxyEx.CreateQuery<InventoryByLocDetail>("GetReportData2")
						.AddQueryExOption("dcCode", SelectedF140101.DC_CODE)
						.AddQueryExOption("gupCode", SelectedF140101.GUP_CODE)
						.AddQueryExOption("custCode", SelectedF140101.CUST_CODE)
						.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO).ToList();
					break;
			}
			DoReportShow(printType);
		}
		#endregion

		#region ExportDetail
		public ICommand ExportDetailCommand
		{
			get
			{
				return new RelayCommand(
						DoExportDetail,
						() => !IsBusy
								&& UserOperateMode == OperateMode.Query
								&& SelectedF140101 != null && !string.IsNullOrWhiteSpace(QueryDetailBegLocCode) && !string.IsNullOrWhiteSpace(QueryDetailEndLocCode)
				);
			}
		}

		private List<InventoryDetailItem> GetExportData()
		{
			string message;
			List<InventoryDetailItem> results = new List<InventoryDetailItem>();
			if (ValidateHelper.TryCheckBeginEndForLoc(this, x => x.QueryDetailBegLocCode, x => x.QueryDetailEndLocCode, Properties.Resources.LocCode,
				out message))
			{
				//執行查詢動
				var proxyEx = GetExProxy<P14ExDataSource>();
				results = proxyEx.CreateQuery<InventoryDetailItem>("GetInventoryDetailItemsExport")
					.AddQueryExOption("dcCode", SelectedF140101.DC_CODE)
					.AddQueryExOption("gupCode", SelectedF140101.GUP_CODE)
					.AddQueryExOption("custCode", SelectedF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO)
					.AddQueryExOption("wareHouseId", SelectedQueryDetialWareHouseId)
					.AddQueryExOption("begLocCode", QueryDetailBegLocCode.Trim())
					.AddQueryExOption("endLocCode", QueryDetailEndLocCode.Trim())
					.AddQueryExOption("itemCode", ((!string.IsNullOrWhiteSpace(QueryDetailItemCode) ? QueryDetailItemCode.Trim() : "")))
					.AddQueryExOption("procType", SelectedProcType)
          .ToList();
				if (!results.Any())
					ShowMessage(Messages.InfoNoData);
			}
			else
				ShowWarningMessage(message);

			return results;
		}


		private void DoExportDetail()
		{
			var saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = ".xlsx";
			saveFileDialog.Filter = "Excel (.xlsx)|*.xlsx";
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.OverwritePrompt = true;
			saveFileDialog.Title = Properties.Resources.ExportDetailWareHouseList;
			bool? isShowOk;

			isShowOk = saveFileDialog.ShowDialog();
			if (isShowOk ?? false)
			{
				var excelExportService = new ExcelExportService();
				excelExportService.CreateNewSheet(Properties.Resources.P1401010000_InventoryDetail + SelectedF140101.INVENTORY_NO);

				var exportResult = GetExportData();

				if (!exportResult.Any())
					return;

				var data = exportResult.ToDataTable();

				var showColumnName = new List<string>
					{
						"ROWNUM",
						"ITEM_CODE",
						"CUST_ITEM_CODE",
						"EAN_CODE1",
						"EAN_CODE2",
						"EAN_CODE3",
						"ITEM_NAME",
						"VALID_DATE",
						"MAKE_NO",
						"ENTER_DATE",
						"WAREHOUSE_NAME",
						"LOC_CODE",
						"QTY",
						"UNMOVE_STOCK_QTY",
						"DEVICE_STOCK_QTY",
						"FIRST_QTY",
						"SECOND_QTY"
					};
				var delColumnList = (from DataColumn column in data.Columns where !showColumnName.Contains(column.ColumnName) select column.ColumnName).ToList();
				foreach (var columnName in delColumnList)
					data.Columns.Remove(columnName);

				data.Columns["ROWNUM"].SetOrdinal(0);
				data.Columns["ROWNUM"].ColumnName = Resources.Resources.ItemNumber;
				data.Columns["ITEM_CODE"].SetOrdinal(1);
				data.Columns["ITEM_CODE"].ColumnName = Properties.Resources.ITEM_CODE;
				data.Columns["CUST_ITEM_CODE"].SetOrdinal(2);
				data.Columns["CUST_ITEM_CODE"].ColumnName = "貨主品編";
				data.Columns["EAN_CODE1"].SetOrdinal(3);
				data.Columns["EAN_CODE1"].ColumnName = "國條";
				data.Columns["EAN_CODE2"].SetOrdinal(4);
				data.Columns["EAN_CODE2"].ColumnName = "條碼二";
				data.Columns["EAN_CODE3"].SetOrdinal(5);
				data.Columns["EAN_CODE3"].ColumnName = "條碼三";
				data.Columns["ITEM_NAME"].SetOrdinal(6);
				data.Columns["ITEM_NAME"].ColumnName = Properties.Resources.ITEM_NAME;
				data.Columns["VALID_DATE"].SetOrdinal(7);
				data.Columns["VALID_DATE"].ColumnName = Properties.Resources.VALID_DATE;
				data.Columns["MAKE_NO"].SetOrdinal(8);
				data.Columns["MAKE_NO"].ColumnName = Properties.Resources.MAKE_NO;
				data.Columns["ENTER_DATE"].SetOrdinal(9);
				data.Columns["ENTER_DATE"].ColumnName = Properties.Resources.EnterDate;
				data.Columns["WAREHOUSE_NAME"].SetOrdinal(10);
				data.Columns["WAREHOUSE_NAME"].ColumnName = Properties.Resources.QueryDetialWareHouseId;
				data.Columns["LOC_CODE"].SetOrdinal(11);
				data.Columns["LOC_CODE"].ColumnName = Properties.Resources.LocCode;
				data.Columns["QTY"].SetOrdinal(12);
				data.Columns["QTY"].ColumnName = "WMS庫存數";
				data.Columns["UNMOVE_STOCK_QTY"].SetOrdinal(13);
				data.Columns["UNMOVE_STOCK_QTY"].ColumnName = "WMS未搬動數";
				data.Columns["DEVICE_STOCK_QTY"].SetOrdinal(14);
				data.Columns["DEVICE_STOCK_QTY"].ColumnName = "自動倉庫存數";
				data.Columns["FIRST_QTY"].SetOrdinal(15);
				data.Columns["FIRST_QTY"].ColumnName = Properties.Resources.FIRST_QTY;
				data.Columns["SECOND_QTY"].SetOrdinal(16);
				data.Columns["SECOND_QTY"].ColumnName = Properties.Resources.SECOND_QTY;


				var excelExportSource = new ExcelExportReportSource
				{
					Data = data,
					DataFormatList =
		new List<NameValuePair<string>>
		{
							new NameValuePair<string>() {Name = Properties.Resources.VALID_DATE, Value = "yyyy/m/d"},
							new NameValuePair<string>() {Name = Properties.Resources.EnterDate, Value = "yyyy/m/d"}
		}
				};

				excelExportService.AddExportReportSource(excelExportSource);
				bool isExportSuccess = excelExportService.Export(Path.GetDirectoryName(saveFileDialog.FileName),
					Path.GetFileName(saveFileDialog.FileName));
				var message = new MessagesStruct
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Message = "",
					Title = Resources.Resources.Information
				};
				message.Message = isExportSuccess ? Properties.Resources.P1401010000_ExportInventoryDetailSuccess : Properties.Resources.P1401010000_ExportInventoryDetailFail;
				if (!isExportSuccess)
					message.Image = DialogImage.Warning;
				ShowMessage(message);
			}
		}
		#endregion

		public ICommand ImportInventoryDetailItemCommand
		{
			get
			{
				return new RelayCommand(() =>
				{
					DispatcherAction(() =>
									{
										ExcelImportInventoryDetailItem();
										if (string.IsNullOrEmpty(ImportInventoryDetailItemFilePath)) return;
										ExcelImportInventoryDetailItemCommand.Execute(null);
									});
				});
			}
		}

		/// <summary>
		///  異動盤只能新增本次異動盤同儲位同商品的其他效期商品
		/// </summary>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		private bool CheckInventoryLocItem(string locCode, string itemCode)
		{
			if (AddOrEditF140101.INVENTORY_TYPE == "2")
			{
				var proxy = GetExProxy<P14ExDataSource>();
				var inventoryLocItemList = proxy.CreateQuery<InventoryLocItem>("GetInventoryLocItems")
					.AddQueryExOption("dcCode", AddOrEditF140101.DC_CODE)
					.AddQueryExOption("gupCode", AddOrEditF140101.GUP_CODE)
					.AddQueryExOption("custCode", AddOrEditF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", AddOrEditF140101.INVENTORY_NO).ToList();
				return inventoryLocItemList.Any(o => o.LOC_CODE == locCode && o.ITEM_CODE == itemCode);
			}
			return true;
		}

		#region SetAllFlushback
		public ICommand SetAllFlushbackCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSetAllFlushback(), () => UserOperateMode != OperateMode.Query && AddOrEditF140101 != null && EditInventoryDetailItemList != null
						);
			}
		}

		private void DoSetAllFlushback()
		{
			if (EditInventoryDetailItemList.Any(o => o.FLUSHBACK == "0"))
				EditInventoryDetailItemList.ForEach(o => o.FLUSHBACK = "1");
			else
				EditInventoryDetailItemList.ForEach(o => o.FLUSHBACK = "0");
		}

		#endregion

		#region 清除UcSearchProdcut
		private void ClearUcSearchProduct()
		{
			AddItemCode = null;
			AddItemName = null;
			AddItemSize = null;
			AddItemColor = null;
			AddItemSpec = null;
			AddHasFindItem = false;
			AddSerialNo = null;

		}

		#endregion

		public bool CheckAutoWarehouse()
		{
			var isAutoWarehouse = false;
			var proxy = GetWcfProxy<wcf.P14WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.GetF1980ByLocCode(AddOrEditF140101.DC_CODE, EditLocCode));
			if(result?.DEVICE_TYPEk__BackingField != "0")
			{
				isAutoWarehouse = true;
			}
			return isAutoWarehouse;
		}
	}

	/// <summary>
	/// Grid欄位顯示的Tpye
	/// </summary>
	public enum DisplayType
	{
		notDisplay,
		display
	}

	public enum ImportType
	{
		Manual = 0,
		Excel = 1
	}
}
