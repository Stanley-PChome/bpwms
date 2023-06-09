using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F19DataService;
using AutoMapper;
using Wms3pl.WpfClient.DataServices.F00DataService;
using System.Windows.Media.Imaging;
using System.Reflection;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.IO;
using System.ComponentModel;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F91DataService;


namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901020000_ViewModel : InputViewModelBase, IDataErrorInfo
	{
		#region property

		public Action BackToFirstTab = delegate { };
		public string ImportFilePath { get; set; }
		public Action ExcelImport = delegate { };

		#region 圖檔

		private BitmapImage _itemImageSource = null;
		/// <summary>
		/// 顯示圖片
		/// Memo: 可用此方式來避免圖檔被程式咬住而無法刪除或移動
		/// </summary>
		public BitmapImage ItemImageSource
		{
			get
			{
				return _itemImageSource;
			}
			set
			{
				Set(() => ItemImageSource, ref _itemImageSource, value);
			}
		}

		public void LoadImage()
		{
			ItemImageSource = FileService.GetItemImage(SelectedData.GUP_CODE, CustCode, SelectedData.ITEM_CODE);
		}
		#endregion

		#region 業主清單

		private List<NameValuePair<string>> _selectedGupList;

		public List<NameValuePair<string>> SelectedGupList
		{
			get { return _selectedGupList; }
			set
			{
				_selectedGupList = value;
				RaisePropertyChanged();
			}
		}


		public string GupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		public string CustCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }


		#endregion

		#region 查詢條件
		private string _itemCode = string.Empty;
		public string ItemCode { get { return _itemCode; } set { _itemCode = value; RaisePropertyChanged(); } }

		private string _itemName = string.Empty;
		public string ItemName { get { return _itemName; } set { _itemName = value; RaisePropertyChanged(); } }

		private string _itemSpec = string.Empty;
		public string ItemSpec { get { return _itemSpec; } set { _itemSpec = value; RaisePropertyChanged(); } }
		#endregion

		#region 商品清單
		/// <summary>
		/// 商品主檔清單
		/// </summary>
		private List<F1903> _records;
		public List<F1903> Records { get { return _records; } set { _records = value; RaisePropertyChanged(); } }
		/// <summary>
		/// 商品副檔清單
		/// </summary>
		private List<F1903> _recordDetails;
		public List<F1903> RecordDetails { get { return _recordDetails; } set { _recordDetails = value; RaisePropertyChanged(); } }

		private F1903 _selectedData;
		public F1903 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged(() => SelectedData);
				if (value != null)
				{
					SetData(value);
					SetDataByView(value);
					LoadImage();
				}
				else
				{
					CurrentRecordByView = null;
					CurrentRecordVolumeByView = null;
					SelectedItemAttrByView = string.Empty;
					SelectedItemSerialRuleByView = string.Empty;
					PackLengthByView = 0;
					PackHightByView = 0;
					PackWeightByView = 0;
					PackWidthByView = 0;
					ItemImageSource = null;
				}
			}
		}

		#endregion

		#region 是否選取一筆
		private Visibility _hasSelectedRecordVisibility;

		public Visibility HasSelectedRecordVisibility
		{
			get { return _hasSelectedRecordVisibility; }
			set
			{
				if (_hasSelectedRecordVisibility == value)
					return;
				Set(() => HasSelectedRecordVisibility, ref _hasSelectedRecordVisibility, value);
			}
		}
		#endregion
		#region 是否選取一筆
		private bool _hasSelectedRecord;

		public bool HasSelectedRecord
		{
			get { return _hasSelectedRecord; }
			set
			{
				HasSelectedRecordVisibility = value ? Visibility.Visible : Visibility.Collapsed;
				if (_hasSelectedRecord == value)
					return;
				Set(() => HasSelectedRecord, ref _hasSelectedRecord, value);
			}
		}
		#endregion


		#region 商品
		/// <summary>
		/// 商品主檔
		/// </summary>
		private F1903 _currentRecord;
		public F1903 CurrentRecord
		{
			get { return _currentRecord; }
			set { _currentRecord = value; RaisePropertyChanged("CurrentRecord"); }
		}

		/// <summary>
		/// 商品主檔ByView
		/// </summary>
		private F1903 _currentRecordByView;
		public F1903 CurrentRecordByView
		{
			get { return _currentRecordByView; }
			set
			{
				_currentRecordByView = value; RaisePropertyChanged("CurrentRecordByView");
				HasSelectedRecord = value != null;
			}
		}

		/// <summary>
		/// 原始商品主檔
		/// </summary>
		private F1903 _orgRecord;
		public F1903 OrgRecord
		{
			get { return _orgRecord; }
			set { _orgRecord = value; RaisePropertyChanged("OrgRecord"); }
		}

		/// <summary>
		/// 商品材積檔
		/// </summary>
		private F1905 _currentRecordVolume;
		public F1905 CurrentRecordVolume
		{
			get { return _currentRecordVolume; }
			set { _currentRecordVolume = value; RaisePropertyChanged("CurrentRecordVolume"); }
		}
		/// <summary>
		/// 商品材積檔
		/// </summary>
		private F1905 _currentRecordVolumeByView;
		public F1905 CurrentRecordVolumeByView
		{
			get { return _currentRecordVolumeByView; }
			set { _currentRecordVolumeByView = value; RaisePropertyChanged("CurrentRecordVolumeByView"); }
		}
		/// <summary>
		/// 原始商品材積檔
		/// </summary>
		private F1905 _orgRecordVolume;
		public F1905 OrgRecordVolume
		{
			get { return _orgRecordVolume; }
			set { _orgRecordVolume = value; RaisePropertyChanged("OrgRecordVolume"); }
		}
		#region 棧板疊法設定(原始資料)
		private F190305 _orgPalletLevelItem;

		public F190305 OrgPalletLevelItem
		{
			get { return _orgPalletLevelItem; }
			set
			{
				Set(() => OrgPalletLevelItem, ref _orgPalletLevelItem, value);
			}
		}

		#endregion

		#region 棧板疊法設定(顯示用)
		private F190305 _palletLevelItemByView;

		public F190305 PalletLevelItemByView
		{
			get { return _palletLevelItemByView; }
			set
			{
				Set(() => PalletLevelItemByView, ref _palletLevelItemByView, value);
			}
		}

		#endregion

		#region 棧板疊法設定(新增/編輯用)
		private F190305 _palletLevelItem;

		public F190305 PalletLevelItem
		{
			get { return _palletLevelItem; }
			set
			{
				Set(() => PalletLevelItem, ref _palletLevelItem, value);
			}
		}

		#endregion
		#endregion

		#region 大分類
		//主檔
		private List<NameValuePair<string>> _lTypes;
		public List<NameValuePair<string>> LTypes { get { return _lTypes; } set { _lTypes = value; RaisePropertyChanged("LTypes"); } }

		private NameValuePair<string> _selectedLTypeItem;

		public NameValuePair<string> SelectedLTypeItem
		{
			get { return _selectedLTypeItem; }
			set
			{
				Set(() => SelectedLTypeItem, ref _selectedLTypeItem, value);

				if (value == null)
					MTypes = new List<NameValuePair<string>>();
				else
					SetMTypes(value.Value);
			}
		}

		private NameValuePair<string> _searchLTypeItem;

		public NameValuePair<string> SearchLTypeItem
		{
			get { return _searchLTypeItem; }
			set
			{
				Set(() => SearchLTypeItem, ref _searchLTypeItem, value);
			}
		}
		private List<NameValuePair<string>> _searchLTypes;
		public List<NameValuePair<string>> SearchLTypes { get { return _searchLTypes; } set { _searchLTypes = value; RaisePropertyChanged("SearchLTypes"); } }

		#endregion

		#region 中分類

		private List<NameValuePair<string>> _mTypesByView;
		public List<NameValuePair<string>> MTypesByView { get { return _mTypesByView; } set { _mTypesByView = value; RaisePropertyChanged("MTypesByView"); } }


		//主檔
		private List<NameValuePair<string>> _mTypes;
		public List<NameValuePair<string>> MTypes { get { return _mTypes; } set { _mTypes = value; RaisePropertyChanged("MTypes"); } }

		private NameValuePair<string> _selectedMTypeItem;

		public NameValuePair<string> SelectedMTypeItem
		{
			get { return _selectedMTypeItem; }
			set
			{
				Set(() => SelectedMTypeItem, ref _selectedMTypeItem, value);

				if (value == null)
					STypes = new List<NameValuePair<string>>();
				else
					SetSTypes(value.Value);
			}
		}

		#endregion

		#region 小分類


		private List<NameValuePair<string>> _sTypesByView;
		public List<NameValuePair<string>> STypesByView { get { return _sTypesByView; } set { _sTypesByView = value; RaisePropertyChanged("STypesByView"); } }

		//主檔
		private List<NameValuePair<string>> _sTypes;
		public List<NameValuePair<string>> STypes { get { return _sTypes; } set { _sTypes = value; RaisePropertyChanged("STypes"); } }

		private NameValuePair<string> _selectedSTypeItem;

		public NameValuePair<string> SelectedSTypeItem
		{
			get { return _selectedSTypeItem; }
			set
			{
				_selectedSTypeItem = value;
				RaisePropertyChanged("SelectedSTypeItem");
			}
		}
		#endregion

		#region 商品類別
		private List<NameValuePair<string>> _itemTypes;
		public List<NameValuePair<string>> ItemTypes { get { return _itemTypes; } set { _itemTypes = value; RaisePropertyChanged(); } }
		#endregion

		#region 計價類別
		private List<NameValuePair<string>> _accTypes;
		public List<NameValuePair<string>> AccTypes { get { return _accTypes; } set { _accTypes = value; RaisePropertyChanged(); } }
		#endregion

		#region 商品屬性
		private List<NameValuePair<string>> _itemAttrs;
		public List<NameValuePair<string>> ItemAttrs { get { return _itemAttrs; } set { _itemAttrs = value; RaisePropertyChanged(); } }
		private string _selectedItemAttr;
		public string SelectedItemAttr
		{
			get { return _selectedItemAttr; }
			set
			{
				_selectedItemAttr = value;
				if (CurrentRecord != null) CurrentRecord.ITEM_ATTR = value;
				RaisePropertyChanged();
			}
		}

		private string _selectedItemAttrByView;
		public string SelectedItemAttrByView
		{
			get { return _selectedItemAttrByView; }
			set
			{
				_selectedItemAttrByView = value;
				if (CurrentRecordByView != null) CurrentRecordByView.ITEM_ATTR = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 商品保存溫層
		private List<NameValuePair<string>> _itemTmprTypes;
		public List<NameValuePair<string>> ItemTmprTypes { get { return _itemTmprTypes; } set { _itemTmprTypes = value; RaisePropertyChanged(); } }
		#endregion

		#region 序號檢查規則
		private List<NameValuePair<string>> _itemSerialRules;
		public List<NameValuePair<string>> ItemSerialRules { get { return _itemSerialRules; } set { _itemSerialRules = value; RaisePropertyChanged(); } }
		private string _selectedItemSerialRule;
		public string SelectedItemSerialRule
		{
			get { return _selectedItemSerialRule; }
			set
			{
				_selectedItemSerialRule = value;
				if (CurrentRecord != null) CurrentRecord.SERIAL_RULE = value;
				RaisePropertyChanged();
			}
		}

		private string _selectedItemSerialRuleByView;
		public string SelectedItemSerialRuleByView
		{
			get { return _selectedItemSerialRuleByView; }
			set
			{
				_selectedItemSerialRuleByView = value;
				if (CurrentRecordByView != null) CurrentRecordByView.SERIAL_RULE = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 控制效期商品開啟/關閉
		private bool _enableNeedExpired = true;
		public bool EnableNeedExpired
		{
			get { return _enableNeedExpired; }
			set
			{
				_enableNeedExpired = value;
				RaisePropertyChanged("EnableNeedExpired");
			}
		}


		#endregion


		#region 商品負責人員

		private string _itemStaffName;
		public string ItemStaffName
		{
			get { return _itemStaffName; }
			set
			{
				Set(() => ItemStaffName, ref _itemStaffName, value);
			}
		}

		private string _itemStaffNameByView;
		public string ItemStaffNameByView
		{
			get { return _itemStaffNameByView; }
			set
			{
				Set(() => ItemStaffNameByView, ref _itemStaffNameByView, value);
			}
		}
		#endregion

		#region 商品長寬高重量
		private decimal _packLength;
		[Range(typeof(Decimal), "0.01", "99999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal PackLength { get { return _packLength; } set { _packLength = value; RaisePropertyChanged(); } }

		private decimal _packWidth;
		[Range(typeof(Decimal), "0.01", "99999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal PackWidth { get { return _packWidth; } set { _packWidth = value; RaisePropertyChanged(); } }

		private decimal _packHight;
		[Range(typeof(Decimal), "0.01", "99999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal PackHight { get { return _packHight; } set { _packHight = value; RaisePropertyChanged(); } }

		private decimal _packWeight;
		[Range(typeof(Decimal), "0", "9999999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal PackWeight { get { return _packWeight; } set { _packWeight = value; RaisePropertyChanged(); } }
		#endregion

		#region 商品長寬高重量 ByView
		private decimal _packLengthByView;
		[Range(typeof(Decimal), "0.01", "99999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal PackLengthByView { get { return _packLengthByView; } set { _packLengthByView = value; RaisePropertyChanged(); } }

		private decimal _packWidthByView;
		[Range(typeof(Decimal), "0.01", "99999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal PackWidthByView { get { return _packWidthByView; } set { _packWidthByView = value; RaisePropertyChanged(); } }

		private decimal _packHightByView;
		[Range(typeof(Decimal), "0.01", "99999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal PackHightByView { get { return _packHightByView; } set { _packHightByView = value; RaisePropertyChanged(); } }

		private decimal _packWeightByView;
		[Range(typeof(Decimal), "0", "9999999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal PackWeightByView { get { return _packWeightByView; } set { _packWeightByView = value; RaisePropertyChanged(); } }
		#endregion

		#region 上架倉別型態
		private List<NameValuePair<string>> _warehouseTypeList;

		public List<NameValuePair<string>> WarehouseTypeList
		{
			get { return _warehouseTypeList; }
			set
			{
				if (_warehouseTypeList == value)
					return;
				Set(() => WarehouseTypeList, ref _warehouseTypeList, value);
			}
		}

		#endregion

		#region 上架倉別(顯示用)
		private Dictionary<string, List<NameValuePair<string>>> _warehouseIdListGroupByWareHouseType;

		public Dictionary<string, List<NameValuePair<string>>> WarehouseIdListGroupByWareHouseType
		{
			get { return _warehouseIdListGroupByWareHouseType; }
			set
			{
				Set(() => WarehouseIdListGroupByWareHouseType, ref _warehouseIdListGroupByWareHouseType, value);
			}
		}
		#endregion

		#region 上架倉別(新增/編輯用)
		private List<NameValuePair<string>> _warehouseIdList;

		public List<NameValuePair<string>> WareHouseIdList
		{
			get { return _warehouseIdList; }
			set
			{
				Set(() => WareHouseIdList, ref _warehouseIdList, value);
			}
		}
		#endregion

		#region 廠商名稱
		private string _vnrName = string.Empty;
		public string VNR_NAME
		{
			get { return _vnrName; }
			set
			{
				_vnrName = value;
				RaisePropertyChanged("VNR_NAME");
			}
		}

		#endregion

		#region 是否開啟首次進倉日
		private bool _enableFirstInDate = false;
		public bool EnableFirstInDate
		{
			get { return _enableFirstInDate; }
			set
			{
				_enableFirstInDate = value;
				RaisePropertyChanged("EnableFirstInDate");
			}
		}
		#endregion

		private List<NameValuePair<string>> _unitList;

		public List<NameValuePair<string>> UnitList
		{
			get { return _unitList; }
			set
			{
				Set(() => UnitList, ref _unitList, value);
			}
		}

		public void SetUnitList()
		{
			var proxy = GetProxy<F91Entities>();
			UnitList = proxy.F91000302s.Where(x => x.ITEM_TYPE_ID == "001")
										.Select(x => new NameValuePair<string>(x.ACC_UNIT_NAME, x.ACC_UNIT))
										.ToList();
		}

		private void SetWarehouseIdListGroupByWareHouseType()
		{
			var proxy = GetExProxy<P19ExDataSource>();
			var datas = proxy.CreateQuery<WareHouseIdByWareHouseType>("GetWareHouseIdByWareHouseTypeList").AddQueryExOption("gupCode", GupCode).AddQueryExOption("custCode", CustCode).ToList().GroupBy(x => x.WAREHOUSE_TYPE).ToList();
			var allList = new Dictionary<string, List<NameValuePair<string>>>();
			foreach (var item in datas)
			{
				var list = item.Select(x => new NameValuePair<string> { Name = x.WAREHOUSE_NAME, Value = x.WAREHOUSE_ID }).ToList();
				list.Insert(0, new NameValuePair<string> { Name = "", Value = "" });
				allList.Add(item.Key, list);
			}
			WarehouseIdListGroupByWareHouseType = allList;
		}

		#endregion

		#region function
		public P1901020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetSearchGupList();
				SetLTypes();
				DoSearchItemTypes();
				DoSearchAccTypes();
				DoSearchItemAttrs();
				DoSearchItemTmprTypes();
				DoSearchItemSerialRules();
				SetWarehouseTypeList();
				SetUnitList();
				SetWarehouseIdListGroupByWareHouseType();
			}
		}

		private void CountItemSize()
		{
			if (CurrentRecord == null) return;
			CurrentRecord.ITEM_SIZE = string.Format("{0}*{1}*{2}", PackLength, PackWidth, PackHight);
		}
		private void CountItemSizeByView()
		{
			if (CurrentRecordByView == null) return;
			CurrentRecordByView.ITEM_SIZE = string.Format("{0}*{1}*{2}", PackLengthByView, PackWidthByView, PackHightByView);
		}

		/// <summary>
		/// 設定業主清單
		/// </summary>
		public void SetSearchGupList()
		{
			var gupList = Wms3plSession.Get<GlobalInfo>().DcGupCustDatas.Select(x =>
				new NameValuePair<string>() { Name = x.GupName, Value = x.GupCode }).ToList();
			SelectedGupList = gupList.GroupBy(i => i.Value).Select(group => group.First()).ToList();
		}

		private void SetLTypes()
		{
			var proxy = GetProxy<F19Entities>();
			LTypes = proxy.F1915s.Where(n => n.GUP_CODE == GupCode && n.CUST_CODE == CustCode)
								 .OrderBy(x => x.ACODE)
								 .Select(x => new NameValuePair<string>
								 {
									 Name = string.Format("{0}:{1}", x.ACODE, x.CLA_NAME),
									 Value = x.ACODE
								 }).ToList();

			var data = proxy.F1915s.Where(n => n.GUP_CODE == GupCode && n.CUST_CODE == CustCode)
								 .OrderBy(x => x.ACODE)
								 .Select(x => new NameValuePair<string>
								 {
									 Name = string.Format("{0}:{1}", x.ACODE, x.CLA_NAME),
									 Value = x.ACODE
								 }).ToList();
			data.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = "" });
			SearchLTypes = data.OrderBy(o => o.Value).ToList();
			SearchLTypeItem = SearchLTypes.FirstOrDefault();
		}

		private void SetMTypes(string lType)
		{
			var proxy = GetProxy<F19Entities>();
			MTypes = proxy.F1916s.Where(x => x.GUP_CODE == GupCode && x.ACODE == lType && x.CUST_CODE == CustCode)
								 .OrderBy(x => x.BCODE)
								 .Select(x => new NameValuePair<string>
								 {
									 Name = string.Format("{0}:{1}", x.BCODE, x.CLA_NAME),
									 Value = x.BCODE
								 }).ToList();

			if (CurrentRecord != null && UserOperateMode != OperateMode.Query)
				CurrentRecord.MTYPE = MTypes.Select(x => x.Value).FirstOrDefault();
		}

		private void SetMTypesByView(string lType)
		{
			var proxy = GetProxy<F19Entities>();
			MTypesByView = proxy.F1916s.Where(x => x.GUP_CODE == GupCode && x.ACODE == lType && x.CUST_CODE == CustCode)
								 .OrderBy(x => x.BCODE)
								 .Select(x => new NameValuePair<string>
								 {
									 Name = string.Format("{0}:{1}", x.BCODE, x.CLA_NAME),
									 Value = x.BCODE
								 }).ToList();

		}

		private void SetSTypes(string mType)
		{
			var proxy = GetProxy<F19Entities>();
			STypes = proxy.F1917s.Where(x => x.GUP_CODE == GupCode && x.BCODE == mType && x.CUST_CODE == CustCode)
										.OrderBy(x => x.CCODE)
										.Select(x => new NameValuePair<string>
										{
											Name = string.Format("{0}:{1}", x.CCODE,
												x.CLA_NAME),
											Value = x.CCODE
										}).ToList();

			if (CurrentRecord != null && UserOperateMode != OperateMode.Query)
				CurrentRecord.STYPE = STypes.Select(x => x.Value).FirstOrDefault();
		}

		private void SetSTypesByView(string mType)
		{
			var proxy = GetProxy<F19Entities>();
			STypesByView = proxy.F1917s.Where(x => x.GUP_CODE == GupCode && x.BCODE == mType && x.CUST_CODE == CustCode)
										.OrderBy(x => x.CCODE)
										.Select(x => new NameValuePair<string>
										{
											Name = string.Format("{0}:{1}", x.CCODE,
												x.CLA_NAME),
											Value = x.CCODE
										}).ToList();

		}

		private void DoSearchItemTypes()
		{
			ItemTypes = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TYPE");
		}

		private void DoSearchAccTypes()
		{
			AccTypes = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TYPE");
		}

		private void DoSearchItemAttrs()
		{
			ItemAttrs = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "ITEM_ATTR");
			SelectedItemAttr = ItemAttrs.FirstOrDefault().Value;
		}

		private void DoSearchItemTmprTypes()
		{
			ItemTmprTypes = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TMPR_TYPE");
		}

		private void DoSearchItemSerialRules()
		{
			ItemSerialRules = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "SERIAL_RULE");
			SelectedItemSerialRule = ItemSerialRules.FirstOrDefault().Value;
		}

		private void SetWarehouseTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			WarehouseTypeList = proxy.F198001s.Where(x => x.ITEM_PICK_WARE == "1")
												.Select(x => new NameValuePair<string>(x.TYPE_NAME, x.TYPE_ID))
												.ToList();
		}

		public void SetData(F1903 data)
		{
			// 0.先清空Current
			CurrentRecord = null;
			// 1.設定要顯示的資料
			CurrentRecord = AutoMapper.Mapper.DynamicMap<F1903>(data);
			// 2.將原始資料備份起來
			OrgRecord = Mapper.DynamicMap<F1903>(CurrentRecord);

			SelectedItemAttr = (!string.IsNullOrEmpty(CurrentRecord.ITEM_ATTR)) ? CurrentRecord.ITEM_ATTR : string.Empty;

			var proxy = GetProxy<F19Entities>();
			if (UserOperateMode == OperateMode.Add)
			{
				//檢查是否該貨主在貨主主檔裡設定的資料,若有則帶入
				decimal? checkPercent = null;
				string isBoundleSerialloc = "0";
				string isMixLocBatch = "0";
				string isMixLocItem = "1";
				var f1909 = proxy.F1909s.Where(x => x.GUP_CODE == GupCode && x.CUST_CODE == CustCode).FirstOrDefault();
				if (f1909 != null)
				{
					checkPercent = f1909.CHECK_PERCENT;
					isBoundleSerialloc = f1909.BOUNDLE_SERIALLOC;
					isMixLocBatch = f1909.MIX_LOC_BATCH;
					isMixLocItem = f1909.MIX_LOC_ITEM;
				}
				CurrentRecord.CHECK_PERCENT = checkPercent;
				CurrentRecord.MIX_BATCHNO = isMixLocBatch;
				CurrentRecord.LOC_MIX_ITEM = isMixLocItem;
				// 依照 Table Layout 預設值定義
				CurrentRecord.SND_TYPE = "0";
				CurrentRecord.MULTI_FLAG = "0";
				CurrentRecord.ALLOWORDITEM = "0";
				CurrentRecord.BUNDLE_SERIALLOC = "0";
				CurrentRecord.BUNDLE_SERIALNO = "0";
				CurrentRecord.ITEM_EXCHANGE = "0";
				CurrentRecord.ITEM_RETURN = "0";
				CurrentRecord.ITEM_MERGE = "0";
				CurrentRecord.NO_PRICE = "0";
				CurrentRecord.SERIAL_RULE = "0";
				CurrentRecord.ISOEM = "0";
				CurrentRecord.ISBOX = "0";
				CurrentRecord.LG = "0";
				CurrentRecord.CAN_SELL = "1";
				CurrentRecord.CAN_SPILT_IN = "1";
				CurrentRecord.ALLOW_ALL_DLN = 0;
				CurrentRecord.DELV_QTY_AVG = 0;
				CurrentRecord.ORD_SAVE_QTY = 1;
				CurrentRecord.PICK_SAVE_QTY = 1;
				CurrentRecord.PICK_SAVE_ORD = 1;
				CurrentRecord.ISCARTON = "0";
				CurrentRecord.BORROW_DAY = 7; //最大可借天數為必填& default 7
				CurrentRecord.VEN_ORD = 0;
				CurrentRecord.STOP_DATE = DateTime.MaxValue;
				CurrentRecord.BOUNDLE_SERIALREQ = "0";
				CurrentRecord.ISCARTON = "0";
				CurrentRecord.MAKENO_REQU = "0";
				VNR_NAME = GetVnrName(CurrentRecord.VNR_CODE);
				ItemStaffName = null;
			}

			if (CurrentRecord != null)
			{
				if (CurrentRecord.PICK_WARE != null)
				{
					if (CurrentRecord.PICK_WARE != null)
						WareHouseIdList = WarehouseIdListGroupByWareHouseType[CurrentRecord.PICK_WARE];
					else
						WareHouseIdList = new List<NameValuePair<string>>();
				}
				SelectedItemSerialRule = (!string.IsNullOrEmpty(CurrentRecord.SERIAL_RULE)) ? CurrentRecord.SERIAL_RULE : string.Empty;
			}


			// 4.查詢商品材積檔(若狀態為新增不需查)
			var RecordVolumes = new List<F1905>();
			if (UserOperateMode != OperateMode.Add)
			{
				RecordVolumes = proxy.F1905s.Where(x => x.ITEM_CODE == CurrentRecord.ITEM_CODE &&
																							 x.GUP_CODE == CurrentRecord.GUP_CODE &&
																							 x.CUST_CODE == CurrentRecord.CUST_CODE)
																							 .ToList();
			}
			// 4.1 狀態為新增NEW F1905,否則狀查詢材積檔代入
			var tmpVolume = (UserOperateMode == OperateMode.Add || RecordVolumes == null || !RecordVolumes.Any()) ?
				new F1905()
				{
					GUP_CODE = CurrentRecord.GUP_CODE,
					ITEM_CODE = CurrentRecord.ITEM_CODE,
					CUST_CODE = CurrentRecord.CUST_CODE
				} :
				RecordVolumes.FirstOrDefault();
			// 4.2 先清空Current
			CurrentRecordVolume = null;
			// 4.3 設定要顯示的資料
			if (tmpVolume != null)
				CurrentRecordVolume = Mapper.DynamicMap<F1905>(tmpVolume);

			if (CurrentRecordVolume != null)
			{
				// 4.4 將原始資料備份起來
				OrgRecordVolume = Mapper.DynamicMap<F1905>(CurrentRecordVolume);

				PackLength = CurrentRecordVolume.PACK_LENGTH;
				PackWidth = CurrentRecordVolume.PACK_WIDTH;
				PackHight = CurrentRecordVolume.PACK_HIGHT;
				PackWeight = CurrentRecordVolume.PACK_WEIGHT;
			}
			//5.1 先清空Current
			PalletLevelItem = null;

			//5.2 取得棧板疊法設定
			if (UserOperateMode != OperateMode.Add)
			{
				PalletLevelItem = proxy.F190305s.Where(x => x.GUP_CODE == GupCode && x.CUST_CODE == CustCode && x.ITEM_CODE == CurrentRecord.ITEM_CODE).ToList().FirstOrDefault();

			}
			if (UserOperateMode == OperateMode.Add || PalletLevelItem == null)
			{
				//不存在建立新棧板疊法設定
				PalletLevelItem = new F190305
				{
					GUP_CODE = GupCode,
					CUST_CODE = CustCode,
					ITEM_CODE = CurrentRecord.ITEM_CODE,
					PALLET_LEVEL_CASEQTY = 0,
					PALLET_LEVEL_CNT = 0
				};
			}

			OrgPalletLevelItem = Mapper.DynamicMap<F190305>(PalletLevelItem);

			//為修改狀態時，檢查若為效期品，不可改為非效期商品
			EnableNeedExpired = CurrentRecord.NEED_EXPIRED == "1" ? false : true;
		}

		public void SetDataByView(F1903 data)
		{
			// 0.先清空Current
			CurrentRecordByView = null;
			// 1.設定要顯示的資料
			CurrentRecordByView = AutoMapper.Mapper.DynamicMap<F1903>(data);

			SetMTypesByView(CurrentRecordByView.LTYPE);
			SetSTypesByView(CurrentRecordByView.MTYPE);

			SelectedItemAttrByView = (!string.IsNullOrEmpty(CurrentRecordByView.ITEM_ATTR)) ? CurrentRecordByView.ITEM_ATTR : string.Empty;
			VNR_NAME = GetVnrName(CurrentRecordByView.VNR_CODE);
			var proxy = GetProxy<F19Entities>();

			if (CurrentRecordByView != null)
			{
				// 序號檢查規則
				SelectedItemSerialRuleByView = (!string.IsNullOrEmpty(CurrentRecordByView.SERIAL_RULE)) ? CurrentRecordByView.SERIAL_RULE : string.Empty;
				ItemStaffNameByView = string.Empty;
				var f1924 = proxy.F1924s.Where(x => x.EMP_ID == CurrentRecordByView.ITEM_STAFF)
										.Where(x => x.ISDELETED == "0").ToList()
										.FirstOrDefault();
				if (f1924 != null)
				{
					var depName = proxy.F1925s.Where(x => x.DEP_ID == f1924.DEP_ID)
										.ToList()
										.Select(x => string.Format("({0})", x.DEP_NAME))
										.FirstOrDefault() ?? string.Empty;

					ItemStaffNameByView = f1924.EMP_NAME + depName;
				}
			}
			var tmpVolume = proxy.F1905s.Where(x => x.ITEM_CODE == CurrentRecordByView.ITEM_CODE &&
																							x.GUP_CODE == CurrentRecordByView.GUP_CODE &&
																							x.CUST_CODE == CurrentRecordByView.CUST_CODE)
				.ToList().FirstOrDefault();
			// 4.2 先清空Current
			CurrentRecordVolumeByView = null;
			// 4.3 設定要顯示的資料
			if (tmpVolume != null)
				CurrentRecordVolumeByView = Mapper.DynamicMap<F1905>(tmpVolume);

			if (CurrentRecordVolumeByView != null)
			{
				PackLengthByView = CurrentRecordVolumeByView.PACK_LENGTH;
				PackWidthByView = CurrentRecordVolumeByView.PACK_WIDTH;
				PackHightByView = CurrentRecordVolumeByView.PACK_HIGHT;
				PackWeightByView = CurrentRecordVolumeByView.PACK_WEIGHT;
			}

			//5.1 先清空Current
			PalletLevelItemByView = null;
			//5.2 取得棧板疊法設定
			PalletLevelItemByView = proxy.F190305s.Where(x => x.GUP_CODE == GupCode && x.CUST_CODE == CustCode && x.ITEM_CODE == CurrentRecordByView.ITEM_CODE).ToList().FirstOrDefault();
			if (PalletLevelItemByView == null)
			{
				//不存在建立新棧板疊法設定
				PalletLevelItemByView = new F190305
				{
					GUP_CODE = GupCode,
					CUST_CODE = CustCode,
					ITEM_CODE = CurrentRecordByView.ITEM_CODE,
					PALLET_LEVEL_CASEQTY = 0,
					PALLET_LEVEL_CNT = 0
				};
			}
		}

		private void SetItemStaffName(string itemStaff)
		{
			ItemStaffName = string.Empty;
			var proxy = GetProxy<F19Entities>();
			var f1924 = proxy.F1924s.Where(x => x.EMP_ID == itemStaff)
									.Where(x => x.ISDELETED == "0")
									.FirstOrDefault();
			if (f1924 == null)
			{
				ShowWarningMessage(Properties.Resources.P1901020000_NOITEM_INCHARGE);
				return;
			}

			var depName = proxy.F1925s.Where(x => x.DEP_ID == f1924.DEP_ID)
										.ToList()
										.Select(x => string.Format("({0})", x.DEP_NAME))
										.FirstOrDefault() ?? string.Empty;

			ItemStaffName = f1924.EMP_NAME + depName;
		}

		#endregion

		public ICommand SetItemStaffNameCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				o =>
				{
					if (CurrentRecord != null && !string.IsNullOrEmpty(CurrentRecord.ITEM_STAFF))
					{
						SetItemStaffName(CurrentRecord.ITEM_STAFF);
					}
					else
					{
						ItemStaffName = null;
					}
				});
			}
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(ItemCode, ItemName, ItemSpec), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearchComplete()
		{
			if (Records != null && Records.Any())
			{
				if (OrgRecord != null)
					SelectedData = Records.FirstOrDefault(x => x.ITEM_CODE == OrgRecord.ITEM_CODE);
				else
					SelectedData = Records.FirstOrDefault();
			}
		}

		private void DoSearch(string itemCode, string itemName, string itemSpec)
		{
			//檢核是否至少有輸入一查詢條件
			if (string.IsNullOrEmpty(itemCode.Trim()) && string.IsNullOrEmpty(itemName.Trim()) && string.IsNullOrEmpty(itemSpec.Trim()) && string.IsNullOrEmpty(SearchLTypeItem.Value.Trim()))
			{
				if (UserOperateMode == OperateMode.Query)
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901020000_NEED_ONE_CONDITION, Title = Resources.Resources.Information });
				return;
			}

			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			Records = proxy.CreateQuery<F1903>("GetF1903")
							.AddQueryExOption("gupCode", GupCode)
							.AddQueryExOption("custCode", CustCode)
							.AddQueryExOption("itemCodes", itemCode)
							.AddQueryExOption("itemName", itemName)
							.AddQueryExOption("itemSpec", itemSpec)
							.AddQueryExOption("lType", SearchLTypeItem.Value)
							.ToList();

			if (Records == null || !Records.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => {
						// EnableFirstInDate = false;
						DoAdd();
					}, () => UserOperateMode == OperateMode.Query,
					o => DoAddComplete()
					);
			}
		}

		private void DoAddComplete()
		{
			ItemImageSource = null;
		}

		private void DoAdd()
		{
			//執行新增動作
			UserOperateMode = OperateMode.Add;
			SetData(new F1903()
			{
				GUP_CODE = GupCode,
				FRAGILE = "0",            //易碎品
				SPILL = "0",            //液體
				IS_EASY_LOSE = "0",         //是否易遺失
				IS_PRECIOUS = "0",          //貴種品標示	
				IS_MAGNETIC = "0",          //強磁標示
				IS_PERISHABLE = "0",        //易變質
				IS_TEMP_CONTROL = "0",        //需溫控
				C_D_FLAG = "0",           //越庫商品
				BUNDLE_SERIALLOC = "0",             //序號綁儲位
				BUNDLE_SERIALNO = "0",              //序號商品
				ALLOWORDITEM = "0",         //是否允許原箱出貨
				LOC_MIX_ITEM = "0",         //允許儲位混商品
				MIX_BATCHNO = "0",          //是否可混批擺放於儲位
				ITEM_MERGE = "0",                   //是否可併貨包裝
				NO_PRICE = "0",                     //是否無價商品
				ISCARTON = "0",           //是否為紙箱
				NEED_EXPIRED = "0",         //效期商品
				ISOEM = "0",            //是否為自有商品
				ISBOX = "0",            //允許整箱出貨
				MAKENO_REQU = "0",          //批號商品
				//LTYPE = "999",
				ITEM_UNIT = UnitList.Where(x => x.Name.ToUpper() == "PCS").Select(x => x.Value).FirstOrDefault(),
				STOP_DATE = DateTime.MaxValue,
				CUST_CODE = CustCode
			});


		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => {
						//EnableFirstInDate = true;
						DoEdit();
					},
					() => UserOperateMode == OperateMode.Query && SelectedData != null,
					O =>
					{
						LoadImage();
					}
					);
			}
		}

		private void DoEdit()
		{
			if (SelectedData == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901020000_SELECT_NO_DATA, Title = Resources.Resources.Information });
				return;
			}

			SetData(SelectedData);
			SetItemStaffNameCommand.Execute(null);

			UserOperateMode = OperateMode.Edit;
			//執行編輯動作


		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query,
					o => DoCancelComplete()
					);
			}
		}

		private void DoCancelComplete()
		{
			DoSearchComplete();
			BackToFirstTab();
			UserOperateMode = OperateMode.Query;

			RaisePropertyChanged(() => PackLength);
			RaisePropertyChanged(() => PackWidth);
			RaisePropertyChanged(() => PackHight);

		}

		private void DoCancel()
		{
			//執行取消動作
			CurrentRecord = null;
			DoSearch(ItemCode, ItemName, ItemSpec);
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				bool isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null,
					o =>
					{
						if (isSuccess)
						{
							if (SelectedData != null && SelectedData.ITEM_CODE == ItemCode)
							{
								ItemCode = string.Empty;
								ItemName = string.Empty;
							}

							if (Records != null && Records.Count == 1
							|| (string.IsNullOrEmpty(ItemCode) && string.IsNullOrEmpty(ItemName)))
							{
								Records = new List<F1903>();
								SelectedData = null;
							}
							else
							{
								DoSearch(ItemCode, ItemName, ItemSpec);
							}
						}
					}
					);
			}
		}

		private bool DoDelete()
		{
			// 確認是否要刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return false;

			// 如果是刪除資料, 則必須進行DB操作
			var proxyEx = GetExProxy<P19ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("DeleteItem")
				.AddQueryExOption("itemCode", SelectedData.ITEM_CODE)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.ToList()
				.FirstOrDefault();
			if (result.IsSuccessed)
			{
				//刪除實體圖檔
				var message = string.Empty;
				if (!FileService.DeleteItemImage(GupCode, CustCode, SelectedData.ITEM_CODE, null, out message))
				{
					result.Message += Environment.NewLine + message;
				}
			}

			ShowResultMessage(result);
			return result.IsSuccessed;
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(), () => CanExecuteSave(),
					o => DoSaveComplete(isSuccess)
					);
			}
		}


		bool CanExecuteSave()
		{
      if (UserOperateMode != OperateMode.Query && CurrentRecord != null)
        return true;
      return false;
		}

		private void DoSaveComplete(bool isSuccess)
		{
			// 存檔完重新查詢
			if (isSuccess)
			{
				if (UserOperateMode == OperateMode.Add && CurrentRecord != null)
					DoSearch(CurrentRecord.ITEM_CODE, CurrentRecord.ITEM_NAME, CurrentRecord.ITEM_SPEC);
				else
					DoSearch(ItemCode, ItemName, ItemSpec);
				if (Records != null && Records.Any())
					SelectedData = Records.Where(x => x.GUP_CODE == CurrentRecord.GUP_CODE && x.ITEM_CODE == CurrentRecord.ITEM_CODE).FirstOrDefault();
				BackToFirstTab();
				UserOperateMode = OperateMode.Query;
				CurrentRecord = null;
				RaisePropertyChanged(() => PackLength);
				RaisePropertyChanged(() => PackWidth);
				RaisePropertyChanged(() => PackHight);

			}

		}
		private bool DoSave()
		{
			bool isSuccess = false;
      #region 檢查資料
      var errorMsg = GetErrorMsg();
      if (!string.IsNullOrEmpty(errorMsg))
      {
        ShowWarningMessage(errorMsg);
        return false;
      }
      #endregion 檢查資料

      CurrentRecordVolume.PACK_LENGTH = PackLength;
			CurrentRecordVolume.PACK_WIDTH = PackWidth;
			CurrentRecordVolume.PACK_HIGHT = PackHight;
			CurrentRecordVolume.PACK_WEIGHT = PackWeight;

			// 資料未變更時提示訊息
			if (IsDataModified() == DataModifyType.NotModified)
			{
				ShowMessage(Messages.WarningNotModified);
				return false;
			}

			if (CurrentRecordVolume.PACK_LENGTH != OrgRecordVolume.PACK_LENGTH ||
				CurrentRecordVolume.PACK_WIDTH != OrgRecordVolume.PACK_WIDTH ||
				CurrentRecordVolume.PACK_HIGHT != OrgRecordVolume.PACK_HIGHT ||
				CurrentRecord.ITEM_SIZE != OrgRecord.ITEM_SIZE)
			{
				var itemSize = string.Format("{0}*{1}*{2}", CurrentRecordVolume.PACK_LENGTH, CurrentRecordVolume.PACK_WIDTH, CurrentRecordVolume.PACK_HIGHT);
				if (ShowConfirmMessage($"請問商品尺寸是否要依據輸入長寬高更新? (是)存入長寬高組合數{itemSize}，(否)存入商品尺寸{CurrentRecord.ITEM_SIZE}") == DialogResponse.Yes)
				{
					CurrentRecord.ITEM_SIZE = itemSize;
				}
			}

			if (CurrentRecord.STOP_DATE <= DateTime.Today)
			{
				ShowWarningMessage(Properties.Resources.P1901020000_EXIST_ORDER_HAS_ITEM_CODE);
			}

			CurrentRecordVolume.CUST_CODE = CustCode;
			CurrentRecordVolume.ITEM_CODE = CurrentRecord.ITEM_CODE;
			PalletLevelItem.CUST_CODE = CustCode;
			PalletLevelItem.ITEM_CODE = CurrentRecord.ITEM_CODE;

			// 當虛擬商品類別有值時，設為序號商品
			if (!string.IsNullOrWhiteSpace(CurrentRecord.VIRTUAL_TYPE))
			{
				CurrentRecord.BUNDLE_SERIALNO = "1";
			}


			// 儲存資料
			wcf.ExecuteResult result = new wcf.ExecuteResult() { IsSuccessed = true };
			var proxy = new wcf.P19WcfServiceClient();
			var mainItem = new wcf.F1903();
			mainItem = CurrentRecord.Map<F1903, wcf.F1903>();
      mainItem.IS_ASYNCk__BackingField = "N";
			CurrentRecord.CUST_CODE = CustCode;
			CurrentRecord.MIX_BATCHNO = CurrentRecord.MIX_BATCHNO ?? "0";
			CurrentRecord.LOC_MIX_ITEM = CurrentRecord.LOC_MIX_ITEM ?? "0";
			CurrentRecord.LTYPE = SelectedLTypeItem != null ? SelectedLTypeItem.Value : "";
			CurrentRecord.MTYPE = SelectedMTypeItem != null ? SelectedMTypeItem.Value : "";
			CurrentRecord.STYPE = SelectedSTypeItem != null ? SelectedSTypeItem.Value : "";

			var volumeItem = new wcf.F1905();
			volumeItem = CurrentRecordVolume.Map<F1905, wcf.F1905>();

			var palletLevelItem = new wcf.F190305();
			palletLevelItem = PalletLevelItem.Map<F190305, wcf.F190305>();

			//ExDataMapper.Trim(mainItem);
			if (UserOperateMode == OperateMode.Add)
			{
				result = RunWcfMethod<wcf.ExecuteResult>(
					proxy.InnerChannel,
					() => proxy.InsertP190102(mainItem, volumeItem, palletLevelItem));

				isSuccess = (result.IsSuccessed);
				if (isSuccess)
				{
					ItemCode = CurrentRecord.ITEM_CODE;
				}
			}
			else
			{
				result = RunWcfMethod<wcf.ExecuteResult>(
					proxy.InnerChannel,
					() => proxy.UpdateP190102(mainItem, volumeItem, palletLevelItem));

				isSuccess = (result.IsSuccessed);
			}
			ShowResultMessage(result);
			proxy = null;
			return isSuccess;
		}
		private DataModifyType IsDataModified()
		{
			DataModifyType dmt = DataModifyType.NotModified;
			if (UserOperateMode == OperateMode.Add) return DataModifyType.New;
			//主檔
			var props = CurrentRecord.GetType().GetProperties();
			dmt = CheckObject(props, CurrentRecord, OrgRecord);
			if (dmt == DataModifyType.Modified) return dmt;


			//材積檔
			var volumeprops = CurrentRecordVolume.GetType().GetProperties();
			dmt = CheckObject(volumeprops, CurrentRecordVolume, OrgRecordVolume);
			if (dmt == DataModifyType.Modified) return dmt;

			//棧板階層設定
			var palletlevelprops = PalletLevelItem.GetType().GetProperties();
			dmt = CheckObject(palletlevelprops, PalletLevelItem, OrgPalletLevelItem);
			return dmt;
		}
		private DataModifyType CheckObject(PropertyInfo[] props, object CurrentObj, object OrgObj)
		{
			foreach (PropertyInfo prop in props)
			{
				if (prop.Name.ToLower() == "item" || prop.Name.ToLower() == "error") continue;
				var propValue = prop.GetValue(CurrentObj, null);
				var orgPropValue = prop.GetValue(OrgObj, null);
				if ((propValue != null) && propValue.Equals(orgPropValue)) continue;
				if ((propValue == null) && propValue == orgPropValue) continue;
				return DataModifyType.Modified;
			}
			return DataModifyType.NotModified;
		}
    private string GetErrorMsg()
    {
      List<string> errMsgs = new List<string>();
      //商品編號必填檢核
      if (string.IsNullOrEmpty(CurrentRecord.ITEM_CODE))
        errMsgs.Add(Properties.Resources.P1901020000_ITEM_CODE_REQUIRED);
      if (string.IsNullOrWhiteSpace(CurrentRecord.ITEM_NAME))
        errMsgs.Add("品名為必填");

      if (string.IsNullOrWhiteSpace(CurrentRecord.LTYPE))
        errMsgs.Add("大分類為必填");

      if (string.IsNullOrEmpty(CurrentRecord.TYPE))
        errMsgs.Add(Properties.Resources.P1901020000_ITEM_TYPE_Required);

      if (string.IsNullOrWhiteSpace(CurrentRecord.TMPR_TYPE))
        errMsgs.Add("商品保存溫層為必填");

      if (PackLength < 0.01m || PackLength > 99999999m)
        errMsgs.Add("商品長需介於0.01至99999999");
      if (PackWidth < 0.01m || PackWidth > 99999999m)
        errMsgs.Add("商品寬需介於0.01至99999999");
      if (PackHight < 0.01m || PackHight > 99999999m)
        errMsgs.Add("商品高需介於0.01至99999999");

      if (string.IsNullOrWhiteSpace(CurrentRecord.ACC_TYPE))
        errMsgs.Add("計價類別為必填");

      if (string.IsNullOrEmpty(CurrentRecord.PICK_WARE))
        errMsgs.Add(Properties.Resources.P1901020000_WAREHOUSEID_REQUIRED);

      if (CurrentRecord.BUNDLE_SERIALLOC == "1" && CurrentRecord.BUNDLE_SERIALNO != "1")
        errMsgs.Add(Properties.Resources.P1901020000_SERIALNO_BUNDLE_SERIALITEM_MUSTCHECK);

      // ※ P1901020000 商品主檔新增越庫商品不可設定上架倉別為加工倉
      if (CurrentRecord.C_D_FLAG == "1" && CurrentRecord.PICK_WARE == "W")
        errMsgs.Add(Properties.Resources.P1901020000_CrossWareItem_CannotSet_PICK_WARE_W);

      if (CurrentRecord.ALLOWORDITEM == "1" && !string.IsNullOrEmpty(CurrentRecord.VIRTUAL_TYPE))
        errMsgs.Add(Properties.Resources.P1901020000xamlcs_ItemsCannotBeVirtual);

      if (CurrentRecord.ISCARTON == "1" && CurrentRecord.PICK_WARE != "S")
        errMsgs.Add(Properties.Resources.P1901020000_ISCARTON_ONLY_PICK_WARE_S);

      if (CurrentRecord.RET_ORD != null && CurrentRecord.RET_ORD > 0)
        if (CurrentRecord.VEN_ORD == null || CurrentRecord.VEN_ORD.Value == 0)
          errMsgs.Add(Properties.Resources.P1901020000_VEN_ORD_GREATERTHANZERO);

      if (string.IsNullOrWhiteSpace(CurrentRecord.ITEM_UNIT))
        errMsgs.Add("單位為必填");

      if (CurrentRecord.NEED_EXPIRED == "1" && (!CurrentRecord.SAVE_DAY.HasValue || CurrentRecord.SAVE_DAY.Value <= 0))
        errMsgs.Add("當商品為效期商品時，則總保存天數為必填");

      //優先驗證棧板每層箱數、棧板層數是否<1
      if (PalletLevelItem.PALLET_LEVEL_CASEQTY < 1 || _palletLevelItem.PALLET_LEVEL_CNT < 1)
        errMsgs.Add(Properties.Resources.P1901020000_PalletLevelItem);

      // 判斷原F1903.UPD_DATE != 重新取得F1903.UPD_DATE，顯示"商品資料已被修改，請重新搜尋"
      var reSearchF1903 = SearchF1903();
      if (reSearchF1903?.UPD_DATE != OrgRecord.UPD_DATE)
        return "商品資料已經被修改，請重新搜尋";

      if (errMsgs.Any())
        return string.Join("\r\n", errMsgs);

      return string.Empty;
    }

    #endregion Save

    #region Import
    public ICommand ImportCommand
		{
			get
			{
				return new RelayCommand(() => {
					DispatcherAction(() => {
						ExcelImport();
						if (string.IsNullOrEmpty(ImportFilePath)) return;
						DoImportCommand.Execute(null);
					});
				});
			}
		}

		public ICommand DoImportCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => { DoImport(); },
						() => UserOperateMode == OperateMode.Query
						);
			}
		}

		public void DoImport()
		{
			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(ImportFilePath, ref errorMeg);
			var fileInfo = new FileInfo(ImportFilePath);
			if (excelTable != null)
			{
				if (!CheckExcelColumnsFormat(excelTable))
				{
					ShowWarningMessage(Properties.Resources.P1901020000_ExcelColumnsFormatError);
					return;
				}
				var data = new List<string[]>();
				foreach (DataRow row in excelTable.Rows)
				{
					var item = new string[excelTable.Columns.Count];
					foreach (DataColumn column in excelTable.Columns)
					{
						item[column.Ordinal] = row[column].ToString();
					}
					data.Add(item);
				}
				var proxy = new wcf.P19WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(
					proxy.InnerChannel,
					() => proxy.ImportData(GupCode, CustCode, data.ToArray(), fileInfo.FullName));
				DialogService.ShowMessage(result.Message.ToString());
			}
		}

		private bool CheckExcelColumnsFormat(DataTable excelTable)
		{
			var dataTableColumnName = new List<string>
				{
					Properties.Resources.P1901020000_ItemCode,
					Properties.Resources.ItemName,
					Properties.Resources.ITEM_ENGNAME,
					Properties.Resources.ITEM_NICKNAME,
					Properties.Resources.ITEM_TYPE,
					Properties.Resources.ITEM_SPEC,
					Properties.Resources.ItemSpec,
					Properties.Resources.ITEM_COLOR,
					Properties.Resources.ITEM_CLASS,
					Properties.Resources.P1901020000_SIM_Card_Spec_Instruction,
					Properties.Resources.P1901020000_EANCode1,
					Properties.Resources.P1901020000_EANCode2,
					Properties.Resources.P1901020000_EANCode3,
					Properties.Resources.SearchLTypes,
					Properties.Resources.MTYPE,
					Properties.Resources.STYPE,
					Properties.Resources.TYPE,
					Properties.Resources.ItemAttr,
					Properties.Resources.TMPR_TYPE,
					Properties.Resources.ITEM_HUMIDITY,
					Properties.Resources.VIRTUAL_TYPE,
					Properties.Resources.PackLengthByView,
					Properties.Resources.PackWidthByView,
					Properties.Resources.PackHightByView,
					Properties.Resources.PackWeightByView,
					Properties.Resources.MEMO,
					Properties.Resources.P1901020000_Fragile,
					Properties.Resources.P1901020000_Spill,
					Properties.Resources.P1901020000_IsApple,
					Properties.Resources.CHECKPERCENT,
					Properties.Resources.PICK_SAVE_QTY,
					Properties.Resources.SAVE_DAY,
					Properties.Resources.PICK_SAVE_ORD,
					Properties.Resources.ORD_SAVE_QTY,
					Properties.Resources.P1901020000_Borrow_Max_Days,
					Properties.Resources.P1901020000_EnviromentalTax,
					Properties.Resources.SERIALNO_DIGIT,
					Properties.Resources.SERIAL_BEGIN,
					Properties.Resources.ItemSerialRule,
					Properties.Resources.PICK_WARE_ID,
					Properties.Resources.CUST_ITEM_CODE,
					Properties.Resources.P1901020000_CrossWareItem_Note,
					Properties.Resources.BUNDLE_SERIALLOC,
					Properties.Resources.BUNDLE_SERIALNO,
					Properties.Resources.MIX_BATCHNO,
					Properties.Resources.P1901020000_Loc_Mix_Item,
					Properties.Resources.P1901020000_ALLOWORDITEM,
					Properties.Resources.NO_PRICE,
					Properties.Resources.ISCARTON,
					Properties.Resources.VEN_ORD,
					Properties.Resources.RET_ORD,
					Properties.Resources.ALL_DLN,
					Properties.Resources.ALLOW_ALL_DLN,
					Properties.Resources.ITEM_STAFF,
					Properties.Resources.P1901020000_Can_Split_In,
					Properties.Resources.P1901020000_LG,
					Properties.Resources.ACC_TYPE,
					Properties.Resources.DELV_QTY_AVG
				}; //該EXCEL 應有的Table Column Name
			return excelTable.Columns.Cast<DataColumn>()
				.All(column => dataTableColumnName.Contains(column.ColumnName.Trim()));
		}

		#endregion Import



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

		#endregion Paste

		string IDataErrorInfo.Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<P1901020000_ViewModel>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				if (UserOperateMode == OperateMode.Query)
					return string.Empty;
				return InputValidator<P1901020000_ViewModel>.Validate(this, columnName);
			}
		}

		public void GetFindExistItem()
		{
			if (CurrentRecord != null)
			{
				var proxy = GetProxy<F19Entities>();
				var item = proxy.F1903s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.ITEM_CODE == CurrentRecord.ITEM_CODE).FirstOrDefault();
				if (item != null)
				{
					CurrentRecord = item;
					var volumn =
						proxy.F1905s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.ITEM_CODE == CurrentRecord.ITEM_CODE).FirstOrDefault();
					if (volumn != null)
					{
						PackHight = volumn.PACK_HIGHT;
						PackLength = volumn.PACK_LENGTH;
						PackWeight = volumn.PACK_WEIGHT;
						PackWidth = volumn.PACK_WIDTH;
					}
				}
			}

		}

		public string GetVnrName(string vnrCode)
		{
			//bool isFind = false;
			var proxy = GetProxy<F19Entities>();
			var item =
					proxy.F1908s.Where(
							o => o.GUP_CODE == GupCode && o.VNR_CODE == vnrCode && o.STATUS != "9")
							.ToList();

			if (item != null && item.Any())
			{
				return item.FirstOrDefault().VNR_NAME;
			}
			return string.Empty;
		}

		public void SetNeedExpiredIsFalse()
		{
			if (CurrentRecord.NEED_EXPIRED == "0")
			{

				CurrentRecord.SAVE_DAY = null;
				CurrentRecord.ALL_DLN = null;
				CurrentRecord.ALL_SHP = null;
			}
		}

		private F1903 SearchF1903()
		{
			var proxy = GetProxy<F19Entities>();
			var f1903 = proxy.F1903s.Where(x => x.GUP_CODE == CurrentRecord.GUP_CODE && x.CUST_CODE == CurrentRecord.CUST_CODE && x.ITEM_CODE == CurrentRecord.ITEM_CODE).FirstOrDefault();
			return f1903;
		}
	}
}