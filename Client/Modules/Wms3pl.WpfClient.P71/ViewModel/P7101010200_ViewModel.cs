using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using F1980Data = Wms3pl.WpfClient.ExDataServices.P71ExDataService.F1980Data;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7101010200_ViewModel : InputViewModelBase
	{
		public P7101010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetTempTypeList();
				SetWarehouseTypeList();
				SetHandyList();
				SetWarehourseLocTypeList();

				SetViewHandyList();
				SetViewWarehourseLocTypeList();

				_userId = Wms3plSession.CurrentUserInfo.Account;
			}

		}

		#region Property
		public F1980Data EditF1980Data { get; set; }
		private string _userId;
		private bool IsSuccess = false;
		private List<F1942> _f1942List;

		public Action ClosedSuccessClick = delegate { };
		public Action ClosedCancelClick = delegate { };

		private UseModelType _displayUseModelType;
		public UseModelType DisplayUseModelType
		{
			get { return _displayUseModelType; }
			set
			{
				_displayUseModelType = value;
				ChangeUseModeTypeDisplay();
			}
		}

		private Visibility _visibilityGupAndCust = Visibility.Hidden;

		public Visibility VisibilityGupAndCust
		{
			get { return _visibilityGupAndCust; }
			set
			{
				_visibilityGupAndCust = value;
				RaisePropertyChanged("VisibilityGupAndCust");
			}
		}
		#region 物流中心 業主 貨主
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		private string _selectedDcCode = "";
		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				_selectedDcCode = value;
				RaisePropertyChanged("SelectedDcCode");
				SetGupList();
			}
		}

		private string _selectedDcName = "";
		public string SelectedDcName
		{
			get { return _selectedDcName; }
			set
			{
				_selectedDcName = value;
				RaisePropertyChanged("SelectedDcName");
			}
		}

		private List<NameValuePair<string>> _gupList;
		public List<NameValuePair<string>> GupList
		{
			get { return _gupList; }
			set
			{
				_gupList = value;
				RaisePropertyChanged("GupList");
			}
		}

		private string _selectedGupCode = "";
		public string SelectedGupCode
		{
			get { return _selectedGupCode; }
			set
			{
				_selectedGupCode = value;
				RaisePropertyChanged("SelectedGupCode");
				SetCustList();
			}
		}

		private string _selectedGupName = "";
		public string SelectedGupName
		{
			get { return _selectedGupName; }
			set
			{
				_selectedGupName = value;
				RaisePropertyChanged("SelectedGupName");
			}
		}

		private List<NameValuePair<string>> _custList;
		public List<NameValuePair<string>> CustList
		{
			get { return _custList; }
			set
			{
				_custList = value;
				RaisePropertyChanged("CustList");
			}
		}

		private string _selectedCustCode = "";
		public string SelectedCustCode
		{
			get { return _selectedCustCode; }
			set
			{
				_selectedCustCode = value;
				RaisePropertyChanged("SelectedCustCode");
			}
		}

		private string _selectedCustName = "";
		public string SelectedCustName
		{
			get { return _selectedCustName; }
			set
			{
				_selectedCustName = value;
				RaisePropertyChanged("SelectedCustName");
			}
		}

		#endregion

		#region 倉別屬性

		private List<NameValuePair<string>> _tempTypeList;
		public List<NameValuePair<string>> TempTypeList
		{
			get { return _tempTypeList; }
			set
			{
				_tempTypeList = value;
				RaisePropertyChanged("TempTypeList");
			}
		}

		private string _selectedTempType = "";
		public string SelectedTempType
		{
			get { return _selectedTempType; }
			set
			{
				_selectedTempType = value;
				RaisePropertyChanged("SelectedTempType");
			}
		}

		private List<NameValuePair<string>> _warehouseTypeList;
		public List<NameValuePair<string>> WarehouseTypeList
		{
			get { return _warehouseTypeList; }
			set
			{
				_warehouseTypeList = value;
				RaisePropertyChanged("WarehouseTypeList");
			}
		}

		private string _selectedWarehouseType = "";
		public string SelectedWarehouseType
		{
			get { return _selectedWarehouseType; }
			set
			{
				_selectedWarehouseType = value;
				RaisePropertyChanged("SelectedWarehouseType");
			}
		}

		private string _selectedWarehouseTypeName = "";
		public string SelectedWarehouseTypeName
		{
			get { return _selectedWarehouseTypeName; }
			set
			{
				_selectedWarehouseTypeName = value;
				RaisePropertyChanged("SelectedWarehouseTypeName");
			}
		}
		private string _warehouseId = "";
		public string WarehouseId
		{
			get { return _warehouseId; }
			set
			{
				_warehouseId = value;
				RaisePropertyChanged("WarehouseId");
			}
		}

		private string _warehouseName = "";
		public string WarehouseName
		{
			get { return _warehouseName; }
			set
			{
				_warehouseName = value;
				RaisePropertyChanged("WarehouseName");
			}
		}

		private bool _calStock;
		public bool CalStock
		{
			get { return _calStock; }
			set
			{
				_calStock = value;
				RaisePropertyChanged("CalStock");
			}
		}

		private bool _calFee;
		public bool CalFee
		{
			get { return _calFee; }
			set
			{
				_calFee = value;
				RaisePropertyChanged("CalFee");
			}
		}

		private List<F191201> _locSetData;
		public List<F191201> LocSetData
		{
			get { return _locSetData; }
			set { Set(ref _locSetData, value); }
		}
		#endregion

		#region 原儲位屬性

		private List<NameValuePair<string>> _viewWarehouselocTypeList;
		public List<NameValuePair<string>> ViewWarehouseLocTypeList
		{
			get { return _viewWarehouselocTypeList; }
			set
			{
				_viewWarehouselocTypeList = value;
				RaisePropertyChanged("ViewWarehouseLocTypeList");
			}
		}

		private string _viewSelectedWarehouseLocType = "";
		public string ViewSelectedWarehouseLocType
		{
			get { return _viewSelectedWarehouseLocType; }
			set
			{
				_viewSelectedWarehouseLocType = value;
				var f1942Item = _f1942List.Find(o => o.LOC_TYPE_ID == _viewSelectedWarehouseLocType);
				if (f1942Item != null)
				{
					ViewSelectedHandy = f1942Item.HANDY;
					ViewSelectedHandyName = HandyeList.Find(o => o.Value == f1942Item.HANDY).Name;
				}
				RaisePropertyChanged("ViewSelectedWarehouseLocType");

			}
		}

		private string _viewSelectedWarehouseLocTypeName = "";
		public string ViewSelectedWarehouseLocTypeName
		{
			get { return _viewSelectedWarehouseLocTypeName; }
			set
			{
				_viewSelectedWarehouseLocTypeName = value;
				RaisePropertyChanged("ViewSelectedWarehouseLocTypeName");

			}
		}

		private List<NameValuePair<string>> _viewHardyList;
		public List<NameValuePair<string>> ViewHandyeList
		{
			get { return _viewHardyList; }
			set
			{
				_viewHardyList = value;
				RaisePropertyChanged("ViewHandyeList");
			}
		}

		private string _viewSelectedHandyName = "";
		public string ViewSelectedHandyName
		{
			get { return _viewSelectedHandyName; }
			set
			{
				_viewSelectedHandyName = value;
				RaisePropertyChanged("ViewSelectedHandyName");
			}
		}

		private string _viewSelectedHandy = "";
		public string ViewSelectedHandy
		{
			get { return _viewSelectedHandy; }
			set
			{
				_viewSelectedHandy = value;
				RaisePropertyChanged("ViewSelectedHandy");
			}
		}

		private decimal? _viewHorDistance;

		public decimal? ViewHorDistance
		{
			get { return _viewHorDistance; }
			set
			{
				_viewHorDistance = value;
				RaisePropertyChanged("ViewHorDistance");
			}
		}

		private DateTime? _viewRentBeginDate;

		public DateTime? ViewRentBeginDate
		{
			get { return _viewRentBeginDate; }
			set
			{
				_viewRentBeginDate = value;
				RaisePropertyChanged("ViewRentBeginDate");
			}
		}

		private DateTime? _viewRentEndDate;

		public DateTime? ViewRentEndDate
		{
			get { return _viewRentEndDate; }
			set
			{
				_viewRentEndDate = value;
				RaisePropertyChanged("ViewRentEndDate");
			}
		}

		#endregion

		#region 修改後儲位屬性

		private List<NameValuePair<string>> _warehouselocTypeList;
		public List<NameValuePair<string>> WarehouseLocTypeList
		{
			get { return _warehouselocTypeList; }
			set
			{
				_warehouselocTypeList = value;
				RaisePropertyChanged("WarehouseLocTypeList");
			}
		}

		private string _selectedWarehouseLocType = "";
		public string SelectedWarehouseLocType
		{
			get { return _selectedWarehouseLocType; }
			set
			{
				_selectedWarehouseLocType = value;
				var f1942Item = _f1942List.Find(o => o.LOC_TYPE_ID == _selectedWarehouseLocType);
				if (f1942Item != null)
					SelectedHandy = f1942Item.HANDY;
				else
					SelectedHandy = "";
				SelectedHandyName = HandyeList.Find(o => o.Value == SelectedHandy).Name;
				RaisePropertyChanged("SelectedWarehouseLocType");

			}
		}

		private List<NameValuePair<string>> _hardyList;
		public List<NameValuePair<string>> HandyeList
		{
			get { return _hardyList; }
			set
			{
				_hardyList = value;
				RaisePropertyChanged("HandyeList");
			}
		}

		private string _selectedHandy = "";
		public string SelectedHandy
		{
			get { return _selectedHandy; }
			set
			{
				_selectedHandy = value;
				RaisePropertyChanged("SelectedHandy");
			}
		}

		private string _selectedHandyName = "";
		public string SelectedHandyName
		{
			get { return _selectedHandyName; }
			set
			{
				_selectedHandyName = value;
				RaisePropertyChanged("SelectedHandyName");
			}
		}

		private string _horDistance;

		public string HorDistance
		{
			get { return _horDistance; }
			set
			{
				_horDistance = value;
				RaisePropertyChanged("HorDistance");
			}
		}

		private bool _isModifyDate;

		public bool IsModifyDate
		{
			get { return _isModifyDate; }
			set
			{
				_isModifyDate = value;
				if (!_isModifyDate)
				{
					RentBeginDate = null;
					RentEndDate = null;
				}
				RaisePropertyChanged("IsModifyDate");
			}
		}

		private DateTime? _rentBeginDate;

		public DateTime? RentBeginDate
		{
			get { return _rentBeginDate; }
			set
			{
				_rentBeginDate = value;
				RaisePropertyChanged("RentBeginDate");
			}
		}

		private DateTime? _rentEndDate;

		public DateTime? RentEndDate
		{
			get { return _rentEndDate; }
			set
			{
				_rentEndDate = value;
				RaisePropertyChanged("RentEndDate");
			}
		}

		#endregion

		#region 原有儲位範圍

		private string _viewSelectedFloor = "1";
		public string ViewSelectedFloor
		{
			get { return _viewSelectedFloor; }
			set
			{
				_viewSelectedFloor = value;
				RaisePropertyChanged("ViewSelectedFloor");
				CountViewLoc();
			}
		}

		private string _viewSelectedMinChannel = "1";
		public string ViewSelectedMinChannel
		{
			get { return _viewSelectedMinChannel; }
			set
			{
				_viewSelectedMinChannel = value;
				RaisePropertyChanged("ViewSelectedMinChannel");
			}
		}

		private string _viewSelectedMaxChannel = "1";
		public string ViewSelectedMaxChannel
		{
			get { return _viewSelectedMaxChannel; }
			set
			{
				_viewSelectedMaxChannel = value;
				RaisePropertyChanged("ViewSelectedMaxChannel");
				if (!string.IsNullOrEmpty(value))
					CountViewLoc();
			}
		}

		private string _viewSelectedMinPlain = "1";
		public string ViewSelectedMinPlain
		{
			get { return _viewSelectedMinPlain; }
			set
			{
				_viewSelectedMinPlain = value;
				RaisePropertyChanged("ViewSelectedMinPlain");
			}
		}


		private string _viewSelectedMaxPlain = "1";
		public string ViewSelectedMaxPlain
		{
			get { return _viewSelectedMaxPlain; }
			set
			{
				_viewSelectedMaxPlain = value;
				RaisePropertyChanged("ViewSelectedMaxPlain");
				if (!string.IsNullOrEmpty(value))
					CountLoc();
			}
		}

		private string _viewSelectedMinLocLevel = "1";
		public string ViewSelectedMinLocLevel
		{
			get { return _viewSelectedMinLocLevel; }
			set
			{
				_viewSelectedMinLocLevel = value;
				RaisePropertyChanged("ViewSelectedMinLocLevel");
			}
		}


		private string _viewSelectedMaxLocLevel = "1";
		public string ViewSelectedMaxLocLevel
		{
			get { return _viewSelectedMaxLocLevel; }
			set
			{
				_viewSelectedMaxLocLevel = value;
				RaisePropertyChanged("ViewSelectedMaxLocLevel");
				if (!string.IsNullOrEmpty(value))
					CountLoc();
			}
		}

		private string _viewSelectedMinLocType = "1";
		public string ViewSelectedMinLocType
		{
			get { return _viewSelectedMinLocType; }
			set
			{
				_viewSelectedMinLocType = value;
				RaisePropertyChanged("ViewSelectedMinLocType");
			}
		}

		private string _viewSelectedMaxLocType = "1";
		public string ViewSelectedMaxLocType
		{
			get { return _viewSelectedMaxLocType; }
			set
			{
				_viewSelectedMaxLocType = value;
				RaisePropertyChanged("ViewSelectedMaxLocType");
				if (!string.IsNullOrEmpty(value))
					CountViewLoc();
			}
		}

		private decimal _viewLocCount;

		public decimal ViewLocCount
		{
			get { return _viewLocCount; }
			set
			{
				_viewLocCount = value;
				RaisePropertyChanged("ViewLocCount");
			}
		}

		#endregion

		#region 修改後儲位範圍

		private List<NameValuePair<string>> _floorList;
		public List<NameValuePair<string>> FloorList
		{
			get { return _floorList; }
			set
			{
				_floorList = value;
				RaisePropertyChanged("FloorList");
			}
		}

		private string _selectedFloor = "1";
		public string SelectedFloor
		{
			get { return _selectedFloor; }
			set
			{
				_selectedFloor = value;
				RaisePropertyChanged("SelectedFloor");
				CountLoc();
			}
		}

		private List<NameValuePair<string>> _minChannelList;
		public List<NameValuePair<string>> MinChannelList
		{
			get { return _minChannelList; }
			set
			{
				_minChannelList = value;
				RaisePropertyChanged("MinChannelList");
			}
		}

		private string _selectedMinChannel = "1";
		public string SelectedMinChannel
		{
			get { return _selectedMinChannel; }
			set
			{
				_selectedMinChannel = value;
				RaisePropertyChanged("SelectedMinChannel");
				if (!string.IsNullOrWhiteSpace(value))
					SetMaxChanelList(value);
			}
		}

		private List<NameValuePair<string>> _maxChannelList;
		public List<NameValuePair<string>> MaxChannelList
		{
			get { return _maxChannelList; }
			set
			{
				_maxChannelList = value;
				RaisePropertyChanged("MaxChannelList");
			}
		}

		private string _selectedMaxChannel = "1";
		public string SelectedMaxChannel
		{
			get { return _selectedMaxChannel; }
			set
			{
				_selectedMaxChannel = value;
				RaisePropertyChanged("SelectedMaxChannel");
				if (!string.IsNullOrEmpty(value))
					CountLoc();
			}
		}

		private List<NameValuePair<string>> _minPlainList;
		public List<NameValuePair<string>> MinPlainList
		{
			get { return _minPlainList; }
			set
			{
				_minPlainList = value;
				RaisePropertyChanged("MinPlainList");
			}
		}

		private string _selectedMinPlain = "1";
		public string SelectedMinPlain
		{
			get { return _selectedMinPlain; }
			set
			{
				_selectedMinPlain = value;
				RaisePropertyChanged("SelectedMinPlain");
				if (!string.IsNullOrWhiteSpace(value))
					SetMaxPlainList(value);
			}
		}

		private List<NameValuePair<string>> _maxPlainList;
		public List<NameValuePair<string>> MaxPlainList
		{
			get { return _maxPlainList; }
			set
			{
				_maxPlainList = value;
				RaisePropertyChanged("MaxPlainList");
			}
		}

		private string _selectedMaxPlain = "1";
		public string SelectedMaxPlain
		{
			get { return _selectedMaxPlain; }
			set
			{
				_selectedMaxPlain = value;
				RaisePropertyChanged("SelectedMaxPlain");
				if (!string.IsNullOrEmpty(value))
					CountLoc();
			}
		}

		private List<NameValuePair<string>> _minLocLevelList;
		public List<NameValuePair<string>> MinLocLevelList
		{
			get { return _minLocLevelList; }
			set
			{
				_minLocLevelList = value;
				RaisePropertyChanged("MinLocLevelList");
			}
		}

		private string _selectedMinLocLevel = "1";
		public string SelectedMinLocLevel
		{
			get { return _selectedMinLocLevel; }
			set
			{
				_selectedMinLocLevel = value;
				RaisePropertyChanged("SelectedMinLocLevel");
				SetMaxLocLevelList(value);
			}
		}

		private List<NameValuePair<string>> _maxLocLevelList;
		public List<NameValuePair<string>> MaxLocLevelList
		{
			get { return _maxLocLevelList; }
			set
			{
				_maxLocLevelList = value;
				RaisePropertyChanged("MaxLocLevelList");
			}
		}

		private string _selectedMaxLocLevel = "1";
		public string SelectedMaxLocLevel
		{
			get { return _selectedMaxLocLevel; }
			set
			{
				_selectedMaxLocLevel = value;
				RaisePropertyChanged("SelectedMaxLocLevel");
				if (!string.IsNullOrEmpty(value))
					CountLoc();
			}
		}

		private List<NameValuePair<string>> _minLocTypeList;
		public List<NameValuePair<string>> MinLocTypeList
		{
			get { return _minLocTypeList; }
			set
			{
				_minLocTypeList = value;
				RaisePropertyChanged("MinLocTypeList");
			}
		}

		private string _selectedMinLocType = "1";
		public string SelectedMinLocType
		{
			get { return _selectedMinLocType; }
			set
			{
				_selectedMinLocType = value;
				RaisePropertyChanged("SelectedMinLocType");
				if (!string.IsNullOrWhiteSpace(value))
					SetMaxLocTypeList(value);
			}
		}

		private List<NameValuePair<string>> _maxLocTypeList;
		public List<NameValuePair<string>> MaxLocTypeList
		{
			get { return _maxLocTypeList; }
			set
			{
				_maxLocTypeList = value;
				RaisePropertyChanged("MaxLocTypeList");
			}
		}

		private string _selectedMaxLocType = "1";
		public string SelectedMaxLocType
		{
			get { return _selectedMaxLocType; }
			set
			{
				_selectedMaxLocType = value;
				RaisePropertyChanged("SelectedMaxLocType");
				if (!string.IsNullOrEmpty(value))
					CountLoc();
			}
		}

		private decimal _locCount;

		public decimal LocCount
		{
			get { return _locCount; }
			set
			{
				_locCount = value;
				RaisePropertyChanged("LocCount");
			}
		}

		private decimal _diffLocCount;

		public decimal DiffLocCount
		{
			get { return _diffLocCount; }
			set
			{
				_diffLocCount = value;
				RaisePropertyChanged("DiffLocCount");
			}
		}
		#endregion

		#endregion

		#region 下拉式選單資料來源
		
		#region 物流中心 業主 貨主
		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (DcList.Any())
				SelectedDcCode = DcList.First().Value;
		}

		/// <summary>
		/// 設定業主清單
		/// </summary>
		public void SetGupList()
		{
			var gupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(_selectedDcCode);
			gupList.Insert(0, new NameValuePair<string> { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
			GupList = gupList;
			SelectedGupCode = gupList.First().Value;
		}

		/// <summary>
		/// 設定貨主清單
		/// </summary>
		public void SetCustList()
		{
			var custList = new List<NameValuePair<string>>();
			if (_selectedGupCode != "0")
			{
				custList = Wms3plSession.Get<GlobalInfo>().GetCustDataList(_selectedDcCode, _selectedGupCode);
				custList.Insert(0, new NameValuePair<string> { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
			}
			else
				custList.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
			CustList = custList;
			SelectedCustCode = custList.First().Value;
		}
		#endregion

		#region 倉別屬性
		/// <summary>
		/// 設定溫別清單
		/// </summary>
		public void SetTempTypeList()
		{
			var list = new List<NameValuePair<string>>();
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_TempN, Value = "01" });
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_TempL, Value = "02" });
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_TempF, Value = "03" });
			TempTypeList = list;
			if (TempTypeList.Any())
				SelectedTempType = TempTypeList.First().Value;
		}

		/// <summary>
		/// 設定倉別型態清單
		/// </summary>
		public void SetWarehouseTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F198001s.OrderBy(x => x.TYPE_ID);
			var warehouseTypeList = (from o in data
									 select new NameValuePair<string>
									 {
										 Name = o.TYPE_NAME,
										 Value = o.TYPE_ID
									 }).ToList();
			WarehouseTypeList = warehouseTypeList;
			if (WarehouseTypeList.Any())
				SelectedWarehouseType = WarehouseTypeList.First().Value;
		}

		#endregion

		#region 儲位屬性

		/// <summary>
		/// 設定儲位料架清單
		/// </summary>
		public void SetWarehourseLocTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			_f1942List = proxy.F1942s.OrderBy(x => x.LOC_TYPE_ID).ToList();
			var locTypeList = (from o in _f1942List
							   select new NameValuePair<string>
							   {
								   Name = o.LOC_TYPE_NAME,
								   Value = o.LOC_TYPE_ID
							   }).ToList();
			locTypeList.Insert(0, new NameValuePair<string> { Name = "", Value = "" });
			WarehouseLocTypeList = locTypeList;
			if (WarehouseLocTypeList.Any())
				SelectedWarehouseLocType = WarehouseLocTypeList.First().Value;
		}

		/// <summary>
		/// 設定儲位料架清單
		/// </summary>
		public void SetViewWarehourseLocTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			_f1942List = proxy.F1942s.OrderBy(x => x.LOC_TYPE_ID).ToList();
			var locTypeList = (from o in _f1942List
							   select new NameValuePair<string>
							   {
								   Name = o.LOC_TYPE_NAME,
								   Value = o.LOC_TYPE_ID
							   }).ToList();
			ViewWarehouseLocTypeList = locTypeList;
			if (ViewWarehouseLocTypeList.Any())
				ViewSelectedWarehouseLocType = ViewWarehouseLocTypeList.First().Value;
		}

		/// <summary>
		/// 設定便利性清單
		/// </summary>
		public void SetHandyList()
		{
			var list = new List<NameValuePair<string>>();
			list.Add(new NameValuePair<string> { Name = "", Value = "" });
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_L, Value = "1" });
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_M, Value = "2" });
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_H, Value = "3" });
			HandyeList = list;
			if (HandyeList.Any())
				SelectedHandy = HandyeList.First().Value;
		}

		/// <summary>
		/// 設定便利性清單
		/// </summary>
		public void SetViewHandyList()
		{
			var list = new List<NameValuePair<string>>();
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_L, Value = "1" });
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_M, Value = "2" });
			list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_H, Value = "3" });
			ViewHandyeList = list;
			if (ViewHandyeList.Any())
				ViewSelectedHandy = ViewHandyeList.First().Value;
		}

		#endregion

		#region 修改後儲位範圍
		/// <summary>
		/// 設定樓層清單
		/// </summary>
		public void SetFloorList()
		{
			FloorList = (from a in LocSetData
						 where a.TYPE == "1"
						 select new NameValuePair<string>
						 {
							 Name = a.VALUE,
							 Value = a.VALUE
						 }).ToList();

			if (FloorList.Any())
				SelectedFloor = FloorList.First().Value;
		}
		
		/// <summary>
		/// 設定通道別(起)清單
		/// </summary>
		public void SetMinChanelList()
		{
			MinChannelList = (from a in LocSetData
							  where a.TYPE == "2"
							  select new NameValuePair<string>
							  {
								  Name = a.VALUE,
								  Value = a.VALUE
							  }).ToList();

			if (MinChannelList.Any())
				SelectedMinChannel = MinChannelList.First().Value;
		}
		/// <summary>
		/// 設定通道別(迄)清單
		/// </summary>
		public void SetMaxChanelList(string startIndex)
		{
			MaxChannelList = MinChannelList;
			if (MaxChannelList.Any())
				SelectedMaxChannel = MaxChannelList.FirstOrDefault(o => o.Value == startIndex)?.Value;
		}

		/// <summary>
		/// 設定座別(起)清單
		/// </summary>
		public void SetMinPlainList()
		{
			MinPlainList = (from a in LocSetData
							where a.TYPE == "3"
							select new NameValuePair<string>
							{ Name = a.VALUE, Value = a.VALUE }).ToList();
							
			if (MinPlainList.Any())
				SelectedMinPlain = MinPlainList.First().Value;
		}

		/// <summary>
		/// 設定座別(迄)清單
		/// </summary>
		public void SetMaxPlainList(string startIndex)
		{
			MaxPlainList = MinPlainList;
			if (MaxPlainList.Any())
				SelectedMaxPlain = MaxPlainList.FirstOrDefault(o => o.Value == startIndex)?.Value;
		}
		/// <summary>
		/// 設定層別(起)清單
		/// </summary>
		public void SetMinLocLevelList()
		{
			MinLocLevelList = (from a in LocSetData
							   where a.TYPE == "4"
							   select new NameValuePair<string>
							   { Name = a.VALUE, Value = a.VALUE }).ToList();
							   
			if (MinLocLevelList.Any())
				SelectedMinLocLevel = MinLocLevelList.First().Value;
		}
		/// <summary>
		/// 設定層別(迄)清單
		/// </summary>
		public void SetMaxLocLevelList(string startIndex)
		{
			MaxLocLevelList = MinLocLevelList;
			if (MaxLocLevelList.Any())
				SelectedMaxLocLevel = MaxLocLevelList.FirstOrDefault(o => o.Value == startIndex)?.Value;
		}
		/// <summary>
		/// 設定儲位別(起)清單
		/// </summary>
		public void SetMinLocTypeList()
		{
			MinLocTypeList = (from a in LocSetData
							  where a.TYPE == "5"
							  select new NameValuePair<string>
							  { Name = a.VALUE, Value = a.VALUE }).ToList();

			if (MinLocTypeList.Any())
				SelectedMinLocType = MinLocTypeList.First().Value;
		}
		/// <summary>
		/// 設定儲位別(迄)清單
		/// </summary>
		public void SetMaxLocTypeList(string startIndex)
		{
			MaxLocTypeList = MinLocTypeList;
			if (MaxLocTypeList.Any())
				SelectedMaxLocType = MaxLocTypeList.FirstOrDefault(o => o.Value == startIndex)?.Value;
		}

		#endregion

		#endregion

		#region ChangeUseMode

		private void ChangeUseModeTypeDisplay()
		{
			VisibilityGupAndCust = (_displayUseModelType == UseModelType.Headquarters) ? Visibility.Visible : Visibility.Hidden;
		}

		#endregion

		#region 計算儲位數
		private void CountViewLoc()
		{
			var channel = LocSetData.Where(o => string.Compare(o.VALUE, _viewSelectedMinChannel) >= 0 && string.Compare(o.VALUE, _viewSelectedMaxChannel) <= 0 && o.TYPE == "2").Count();
			var plain = LocSetData.Where(o => string.Compare(o.VALUE, _viewSelectedMinPlain) >= 0 && string.Compare(o.VALUE, _viewSelectedMaxPlain) <= 0 && o.TYPE == "3").Count();
			var locLevel = LocSetData.Where(o => string.Compare(o.VALUE, _viewSelectedMinLocLevel) >= 0 && string.Compare(o.VALUE, _viewSelectedMaxLocLevel) <= 0 && o.TYPE == "4").Count();
			var locType = LocSetData.Where(o => string.Compare(o.VALUE, _viewSelectedMinLocType) >= 0 && string.Compare(o.VALUE, _viewSelectedMaxLocType) <= 0 && o.TYPE == "5").Count();

			//儲位數 = (通道數) * (座別數) * (層數) * (儲位數)
			ViewLocCount = channel * plain * locLevel * locType;
		}
		private void CountLoc()
		{
			var channel = LocSetData.Where(o => string.Compare(o.VALUE, _selectedMinChannel) >= 0 && string.Compare(o.VALUE, _selectedMaxChannel) <= 0 && o.TYPE == "2").Count();
			var plain = LocSetData.Where(o => string.Compare(o.VALUE, _selectedMinPlain) >= 0 && string.Compare(o.VALUE, _selectedMaxPlain) <= 0 && o.TYPE == "3").Count();
			var locLevel = LocSetData.Where(o => string.Compare(o.VALUE, _selectedMinLocLevel) >= 0 && string.Compare(o.VALUE, _selectedMaxLocLevel) <= 0 && o.TYPE == "4").Count();
			var locType = LocSetData.Where(o => string.Compare(o.VALUE, _selectedMinLocType) >= 0 && string.Compare(o.VALUE, _selectedMaxLocType) <= 0 && o.TYPE == "5").Count();

			//儲位數 = (通道數) * (座別數) * (層數) * (儲位數)
			LocCount = channel * plain * locLevel * locType;
			//儲位差 = 新儲位數 - 原儲位數
			DiffLocCount = LocCount - ViewLocCount;
		}

		

		#endregion

		#region 資料繫結
		/// <summary>
		/// 資料繫結
		/// </summary>
		public void BindData()
		{
			SelectedDcCode = EditF1980Data.DC_CODE;
			SelectedGupCode = EditF1980Data.GUP_CODE;
			SelectedCustCode = EditF1980Data.CUST_CODE;

			SelectedDcName = DcList.Find(o => o.Value == SelectedDcCode).Name;
			SelectedGupName = GupList.Find(o => o.Value == SelectedGupCode).Name;
			SelectedCustName = CustList.Find(o => o.Value == SelectedCustCode).Name;

			GetLocSetDatas();
			SelectedWarehouseType = EditF1980Data.WAREHOUSE_TYPE;

			SelectedWarehouseTypeName = WarehouseTypeList.Find(o => o.Value == SelectedWarehouseType).Name;

			WarehouseName = EditF1980Data.WAREHOUSE_Name;
			WarehouseId = EditF1980Data.WAREHOUSE_ID;
			SelectedTempType = EditF1980Data.TMPR_TYPE;

			ViewHorDistance = EditF1980Data.HOR_DISTANCE ?? 0;
			ViewSelectedWarehouseLocType = EditF1980Data.LOC_TYPE_ID;
			ViewSelectedWarehouseLocTypeName = ViewWarehouseLocTypeList.Find(o => o.Value == ViewSelectedWarehouseLocType).Name;

			ViewRentBeginDate = EditF1980Data.RENT_BEGIN_DATE;
			ViewRentEndDate = EditF1980Data.RENT_END_DATE;

			CalStock = EditF1980Data.CAL_STOCK == "1";
			CalFee = EditF1980Data.CAL_FEE == "1";
			ViewSelectedFloor = EditF1980Data.FLOOR;
			ViewSelectedMinChannel = EditF1980Data.MINCHANNEL;
			ViewSelectedMaxChannel = EditF1980Data.MAXCHANNEL;
			ViewSelectedMinPlain = EditF1980Data.MINPLAIN;
			ViewSelectedMaxPlain = EditF1980Data.MAXPLAIN;
			ViewSelectedMinLocLevel = EditF1980Data.MINLOC_LEVEL;
			ViewSelectedMaxLocLevel = EditF1980Data.MAXLOC_LEVEL;
			ViewSelectedMinLocType = EditF1980Data.MINLOC_TYPE;
			ViewSelectedMaxLocType = EditF1980Data.MAXLOC_TYPE;

			SelectedFloor = EditF1980Data.FLOOR;
			SelectedMinChannel = EditF1980Data.MINCHANNEL;
			SelectedMaxChannel = EditF1980Data.MAXCHANNEL;
			SelectedMinPlain = EditF1980Data.MINPLAIN;
			SelectedMaxPlain = EditF1980Data.MAXPLAIN;
			SelectedMinLocLevel = EditF1980Data.MINLOC_LEVEL;
			SelectedMaxLocLevel = EditF1980Data.MAXLOC_LEVEL;
			SelectedMinLocType = EditF1980Data.MINLOC_TYPE;
			SelectedMaxLocType = EditF1980Data.MAXLOC_TYPE;

		}

		#endregion

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return new RelayCommand(
					DoCancel
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			ClosedCancelClick();
		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => true,
					c => DoSaveComplete()
					);
			}
		}

		private void DoSave()
		{
			decimal num;
			//執行確認儲存動作
			string msg = string.Empty;
			if (IsModifyDate && RentBeginDate.HasValue && !RentEndDate.HasValue)
				msg = Properties.Resources.P7101010100_ViewModel_RentBeginDateRentEndDate_Required;
			else if (IsModifyDate && RentEndDate.HasValue && !RentBeginDate.HasValue)
				msg = Properties.Resources.P7101010100_ViewModel_RentEndDateRentBeginDate_Required;
			else if (IsModifyDate && RentBeginDate.HasValue && RentEndDate.HasValue && RentBeginDate > RentEndDate)
				msg = Properties.Resources.P7101010100_ViewModel_RentBeginDate_GreaterThan_RentEndDate;
			else if (!string.IsNullOrEmpty(_horDistance) && !decimal.TryParse(_horDistance, out num))
				msg = Properties.Resources.P7101010100_ViewModel_HorDistance_Num_Required;
			else if (string.IsNullOrEmpty(_warehouseName))
				msg = Properties.Resources.P7101010100_ViewModel_WarehouseName_Required;


			if (msg.Length > 0)
			{
				ShowMessage(new MessagesStruct
				{
					Button = DialogButton.OK,
					Image = DialogImage.Warning,
					Message = msg,
					Title = Resources.Resources.Warning
				});
			}
			else
			{
				var proxywcf = new wcf.P71WcfServiceClient();
				var f1980Data = new ExDataServices.P71WcfService.F1980Data
				{
					WAREHOUSE_ID = _warehouseId,
					DC_CODE = _selectedDcCode,
					WAREHOUSE_Name = _warehouseName,
					WAREHOUSE_TYPE = _selectedWarehouseType,
					TMPR_TYPE = _selectedTempType,
					CAL_STOCK = CalStock ? "1" : "0",
					CAL_FEE = CalFee ? "1" : "0",
					FLOOR = _selectedFloor,
					MINCHANNEL = _selectedMinChannel,
					MAXCHANNEL = _selectedMaxChannel,
					MINPLAIN = _selectedMinPlain,
					MAXPLAIN = _selectedMaxPlain,
					MINLOC_LEVEL = _selectedMinLocLevel,
					MAXLOC_LEVEL = _selectedMaxLocLevel,
					MINLOC_TYPE = _selectedMinLocType,
					MAXLOC_TYPE = _selectedMaxLocType,
					GUP_CODE = _selectedGupCode,
					CUST_CODE = _selectedCustCode,
					LOC_TYPE_ID = _selectedWarehouseLocType,
					HOR_DISTANCE = null,
					RENT_BEGIN_DATE = _rentBeginDate,
					RENT_END_DATE = _rentEndDate,
					IsModifyDate = _isModifyDate
				};
				if (!string.IsNullOrEmpty(_horDistance))
					f1980Data.HOR_DISTANCE = decimal.Parse(_horDistance);
				var f1912data = new List<wcf.F1912>();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.UpdateF1980Data(f1980Data, _userId, f1912data.ToArray()));
				if (result.IsSuccessed)
				{
					IsSuccess = true;
					ShowMessage(Messages.InfoUpdateSuccess);
				}
				else
				{
					var error = Messages.ErrorUpdateFailed;
					error.Message += Environment.NewLine + result.Message;
					ShowMessage(error);
				}

			}
		}

		private void DoSaveComplete()
		{
			if (IsSuccess)
				ClosedSuccessClick();
		}
		#endregion Save

		/// <summary>
		/// 取得儲位設定資料
		/// </summary>
		private void GetLocSetDatas()
		{
			var proxy = GetProxy<F19Entities>();
			LocSetData = proxy.F191201s.Where(x => x.DC_CODE == SelectedDcCode).OrderBy(o => o.VALUE).ToList();

			SetFloorList();
			SetMinChanelList();
			SetMinPlainList();
			SetMinLocLevelList();
			SetMinLocTypeList();
		}
	}
}
