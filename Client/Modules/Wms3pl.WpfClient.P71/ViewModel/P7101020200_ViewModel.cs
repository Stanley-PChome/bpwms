using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using F1919Data = Wms3pl.WpfClient.ExDataServices.P71ExDataService.F1919Data;
using Wms3pl.WpfClient.P71.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7101020200_ViewModel : InputViewModelBase
	{
		public P7101020200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetATypeList();

				_userId = Wms3plSession.CurrentUserInfo.Account;
			}

		}
		#region Property
		public F1919Data EditF1919Data { get; set; }
		private string _userId;
		private bool IsSuccess = false;
		private ExDataServices.P71ExDataService.F1980Data _f1980Data;
		private LocService Loc = new LocService();


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

		#region 儲區屬性

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
		private string _areaCode = "";
		public string AreaCode
		{
			get { return _areaCode; }
			set
			{
				_areaCode = value;
				RaisePropertyChanged("AreaCode");
			}
		}

		private string _areaName = "";
		public string AreaName
		{
			get { return _areaName; }
			set
			{
				_areaName = value;
				RaisePropertyChanged("AreaName");
			}
		}


		private List<NameValuePair<string>> _aTypeList;
		public List<NameValuePair<string>> ATypeList
		{
			get { return _aTypeList; }
			set
			{
				_aTypeList = value;
				RaisePropertyChanged("ATypeList");
			}
		}
		private string _selectedAType = "";
		public string SelectedAType
		{
			get { return _selectedAType; }
			set
			{
				_selectedAType = value;
				RaisePropertyChanged("SelectedAType");
			}
		}
		private string _selectedATypeName = "";
		public string SelectedATypeName
		{
			get { return _selectedATypeName; }
			set
			{
				_selectedATypeName = value;
				RaisePropertyChanged("SelectedATypeName");
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
				SetMaxChanelList(Loc.ChangeWordToNumber(value));
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
				SetMaxPlainList(Loc.ChangeWordToNumber(value));
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
				SetMaxLocLevelList(int.Parse(value));
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
				SetMaxLocTypeList(int.Parse(value));
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
			gupList.Insert(0, new NameValuePair<string>() { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
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
				custList.Insert(0, new NameValuePair<string>() { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
			}
			else
				custList.Add(new NameValuePair<string>() { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
			CustList = custList;
			SelectedCustCode = custList.First().Value;
		}

		#endregion

		#region 儲區屬性
		/// <summary>
		/// 設定儲區型態清單
		/// </summary>
		public void SetATypeList()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F191901s.OrderBy(x => x.ATYPE_CODE);
			var aTypeList = (from o in data
											 select new NameValuePair<string>()
											 {
												 Name = o.ATYPE_NAME,
												 Value = o.ATYPE_CODE
											 }).ToList();
			ATypeList = aTypeList;
			if (ATypeList.Any())
				SelectedAType = aTypeList.First().Value;
		}
		#endregion

		#region 修改後儲位範圍
		/// <summary>
		/// 設定樓層清單
		/// </summary>
		public void SetFloorList()
		{

			if (FloorList == null) FloorList = new List<NameValuePair<string>>();

			if (_f1980Data != null && _f1980Data.FLOOR != null && !string.IsNullOrEmpty(_f1980Data.FLOOR))
				FloorList.Add(new NameValuePair<string>()
				{
					Name = _f1980Data.FLOOR.ToString().PadLeft(1, '0'),
					Value = _f1980Data.FLOOR.ToString().PadLeft(1, '0')
				});
			if (FloorList.Any())
				SelectedFloor = FloorList.First().Value;


		}

		private List<NameValuePair<string>> SetList(int startIndex, int endIndex, int padleft = 2)
		{
			var list = new List<NameValuePair<string>>();
			for (int i = startIndex; i <= endIndex; i++)
				list.Add(new NameValuePair<string>() { Name = Loc.ChangeNumberToWord(i).PadLeft(padleft, '0'), Value = Loc.ChangeNumberToWord(i).PadLeft(padleft, '0') });
			return list;
		}

		/// <summary>
		/// 設定通道別(起)清單
		/// </summary>
		public void SetMinChanelList()
		{
			MinChannelList = SetList(Loc.ChangeWordToNumber(_f1980Data.MINCHANNEL), Loc.ChangeWordToNumber(_f1980Data.MAXCHANNEL));
			if (MinChannelList.Any())
				SelectedMinChannel = MinChannelList.First().Value;
		}
		/// <summary>
		/// 設定通道別(迄)清單
		/// </summary>
		public void SetMaxChanelList(int startIndex = 1)
		{
			MaxChannelList = SetList(startIndex, Loc.ChangeWordToNumber(_f1980Data.MAXCHANNEL));
			if (MaxChannelList.Any())
				SelectedMaxChannel = MaxChannelList.First().Value;
		}

		/// <summary>
		/// 設定座別(起)清單
		/// </summary>
		public void SetMinPlainList()
		{
			MinPlainList = SetList(Loc.ChangeWordToNumber(_f1980Data.MINPLAIN), Loc.ChangeWordToNumber(_f1980Data.MAXPLAIN));
			if (MinPlainList.Any())
				SelectedMinPlain = MinPlainList.First().Value;
		}

		/// <summary>
		/// 設定座別(迄)清單
		/// </summary>
		public void SetMaxPlainList(int startIndex = 1)
		{
			MaxPlainList = SetList(startIndex, Loc.ChangeWordToNumber(_f1980Data.MAXPLAIN));
			if (MaxPlainList.Any())
				SelectedMaxPlain = MaxPlainList.First().Value;
		}
		/// <summary>
		/// 設定層別(起)清單
		/// </summary>
		public void SetMinLocLevelList()
		{
			MinLocLevelList = SetList(int.Parse(_f1980Data.MINLOC_LEVEL), int.Parse(_f1980Data.MAXLOC_LEVEL));
			if (MinLocLevelList.Any())
				SelectedMinLocLevel = MinLocLevelList.First().Value;
		}
		/// <summary>
		/// 設定層別(迄)清單
		/// </summary>
		public void SetMaxLocLevelList(int startIndex = 1)
		{
			MaxLocLevelList = SetList(startIndex, int.Parse(_f1980Data.MAXLOC_LEVEL));
			if (MaxLocLevelList.Any())
				SelectedMaxLocLevel = MaxLocLevelList.First().Value;
		}
		/// <summary>
		/// 設定儲位別(起)清單
		/// </summary>
		public void SetMinLocTypeList()
		{
			MinLocTypeList = SetList(int.Parse(_f1980Data.MINLOC_TYPE), int.Parse(_f1980Data.MAXLOC_TYPE));
			if (MinLocTypeList.Any())
				SelectedMinLocType = MinLocTypeList.First().Value;
		}
		/// <summary>
		/// 設定儲位別(迄)清單
		/// </summary>
		public void SetMaxLocTypeList(int startIndex = 1)
		{
			MaxLocTypeList = SetList(startIndex, int.Parse(_f1980Data.MAXLOC_TYPE));
			if (MaxLocTypeList.Any())
				SelectedMaxLocType = MaxLocTypeList.First().Value;
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
			var viewMinChannel = Loc.ChangeWordToNumber(_viewSelectedMinChannel);//int.Parse(_viewSelectedMinChannel);
			var viewMaxChannel = Loc.ChangeWordToNumber(_viewSelectedMaxChannel);//int.Parse(_viewSelectedMaxChannel);
			var viewMinPlain = Loc.ChangeWordToNumber(_viewSelectedMinPlain);
			var viewMaxPlain = Loc.ChangeWordToNumber(_viewSelectedMaxPlain);
			var viewMinLocLevel = int.Parse(_viewSelectedMinLocLevel);
			var viewMaxLocLevel = int.Parse(_viewSelectedMaxLocLevel);
			var viewMinLocType = int.Parse(_viewSelectedMinLocType);
			var viewMaxLocType = int.Parse(_viewSelectedMaxLocType);

			//儲位數 = (通道迄-通道起 + 1) * (座別迄-座別起 + 1) * (層數迄-層數起 + 1) * (儲位迄-儲位起 + 1)
			ViewLocCount = (viewMaxChannel - viewMinChannel + 1) * (viewMaxPlain - viewMinPlain + 1) * (viewMaxLocLevel - viewMinLocLevel + 1) *
								 (viewMaxLocType - viewMinLocType + 1);
		}
		private void CountLoc()
		{
			var minChannel = Loc.ChangeWordToNumber(_selectedMinChannel);// int.Parse(_selectedMinChannel);
			var maxChannel = Loc.ChangeWordToNumber(_selectedMaxChannel);//int.Parse(_selectedMaxChannel);
			var minPlain = Loc.ChangeWordToNumber(_selectedMinPlain);
			var maxPlain = Loc.ChangeWordToNumber(_selectedMaxPlain);
			var minLocLevel = int.Parse(_selectedMinLocLevel);
			var maxLocLevel = int.Parse(_selectedMaxLocLevel);
			var minLocType = int.Parse(_selectedMinLocType);
			var maxLocType = int.Parse(_selectedMaxLocType);
			//儲位數 = (通道迄-通道起 + 1) * (座別迄-座別起 + 1) * (層數迄-層數起 + 1) * (儲位迄-儲位起 + 1)
			LocCount = (maxChannel - minChannel + 1) * (maxPlain - minPlain + 1) * (maxLocLevel - minLocLevel + 1) *
								 (maxLocType - minLocType + 1);
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
			SelectedDcCode = EditF1919Data.DC_CODE;
			SelectedGupCode = EditF1919Data.GUP_CODE;
			SelectedCustCode = EditF1919Data.CUST_CODE;

			SelectedDcName = DcList.Find(o => o.Value == SelectedDcCode).Name;
			SelectedGupName = GupList.Find(o => o.Value == SelectedGupCode).Name;
			SelectedCustName = CustList.Find(o => o.Value == SelectedCustCode).Name;

			WarehouseName = EditF1919Data.WAREHOUSE_Name;
			WarehouseId = EditF1919Data.WAREHOUSE_ID;
			AreaCode = EditF1919Data.AREA_CODE;
			AreaName = EditF1919Data.AREA_NAME;
			SelectedAType = EditF1919Data.ATYPE_CODE;
			SelectedATypeName = ATypeList.Find(o => o.Value == SelectedAType).Name;

			ViewSelectedFloor = EditF1919Data.FLOOR;
			ViewSelectedMinChannel = EditF1919Data.MINCHANNEL;
			ViewSelectedMaxChannel = EditF1919Data.MAXCHANNEL;
			ViewSelectedMinPlain = EditF1919Data.MINPLAIN;
			ViewSelectedMaxPlain = EditF1919Data.MAXPLAIN;
			ViewSelectedMinLocLevel = EditF1919Data.MINLOC_LEVEL;
			ViewSelectedMaxLocLevel = EditF1919Data.MAXLOC_LEVEL;
			ViewSelectedMinLocType = EditF1919Data.MINLOC_TYPE;
			ViewSelectedMaxLocType = EditF1919Data.MAXLOC_TYPE;

			var proxyEx = GetExProxy<P71ExDataSource>();
			var data = proxyEx.CreateQuery<ExDataServices.P71ExDataService.F1980Data>("GetF1980Datas")
				.AddQueryOption("dcCode", string.Format("'{0}'", EditF1919Data.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", EditF1919Data.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", EditF1919Data.CUST_CODE))
				.AddQueryOption("warehourseId", string.Format("'{0}'", EditF1919Data.WAREHOUSE_ID))
				.AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
				.ToList();
			_f1980Data = data.First();
			//_f1980Data.MAXCHANNEL = Loc.ChangeWordToNumber(_f1980Data.MAXCHANNEL).ToString();
			//_f1980Data.MINCHANNEL = Loc.ChangeWordToNumber(_f1980Data.MINCHANNEL).ToString();
			//EditF1919Data.MAXCHANNEL = Loc.ChangeWordToNumber(EditF1919Data.MAXCHANNEL).ToString();
			//EditF1919Data.MINCHANNEL = Loc.ChangeWordToNumber(EditF1919Data.MINCHANNEL).ToString();
			SetFloorList();
			SetMinChanelList();
			SetMinPlainList();
			SetMinLocLevelList();
			SetMinLocTypeList();

			SelectedFloor = EditF1919Data.FLOOR;
			SelectedMinChannel = EditF1919Data.MINCHANNEL;
			SelectedMaxChannel = EditF1919Data.MAXCHANNEL;
			SelectedMinPlain = EditF1919Data.MINPLAIN;
			SelectedMaxPlain = EditF1919Data.MAXPLAIN;
			SelectedMinLocLevel = EditF1919Data.MINLOC_LEVEL;
			SelectedMaxLocLevel = EditF1919Data.MAXLOC_LEVEL;
			SelectedMinLocType = EditF1919Data.MINLOC_TYPE;
			SelectedMaxLocType = EditF1919Data.MAXLOC_TYPE;

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
			//decimal num = 0;
			//執行確認儲存動作
			string msg = string.Empty;
			if (string.IsNullOrEmpty(_areaName))
				msg = Properties.Resources.P7101020100_ViewModel_AreaName_Required;


			if (msg.Length > 0)
			{
				ShowMessage(new MessagesStruct()
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
				var f1919Data = new ExDataServices.P71WcfService.F1919Data()
				{
					WAREHOUSE_ID = _warehouseId,
					DC_CODE = _selectedDcCode,
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
					AREA_CODE =  _areaCode,
					AREA_NAME = _areaName,
					ATYPE_CODE = _selectedAType 
				};
				var locData = new List<string>();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.UpdateF1919Data(f1919Data, _userId, locData.ToArray()));
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
	}
}
