using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;

using System.Windows;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7101030000_ViewModel : InputViewModelBase
	{
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }

		public Action OnSearch = delegate { };

		/// <summary>
		/// 是否要限制不允許勾選貨主/業主
		/// </summary>
		private bool _restrictGupAndCust = false;

		#region 資料連結/ 頁面參數
		#region Form - Mode Visibility
		private Visibility _visibilityQueryMode;
		public Visibility VisibilityQueryMode
		{
			get { return (UserOperateMode == OperateMode.Query ? Visibility.Visible : Visibility.Collapsed); }
			set { _visibilityQueryMode = value; RaisePropertyChanged("VisibilityQueryMode"); }
		}
		private Visibility _visibilityEditMode;
		public Visibility VisibilityEditMode
		{
			get { return (UserOperateMode == OperateMode.Edit ? Visibility.Visible : Visibility.Collapsed); }
			set { _visibilityEditMode = value; RaisePropertyChanged("VisibilityEditMode"); }
		}
		#endregion
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value;
				DoSearchGup();
				DoSearchWarehouseList();
			}
		}
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.OrderBy(x => x.Value).ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}
		#endregion
		#region Form - 可用的GUP (業主)清單
		private ObservableCollection<NameValuePair<string>> _gupList;
		public ObservableCollection<NameValuePair<string>> GupList
		{
			get { return _gupList.OrderBy(x => x.Value).ToObservableCollection(); }
			set { _gupList = value; RaisePropertyChanged("GupList"); }
		}
		private string _selectedGup = string.Empty;
		public string SelectedGup
		{
			get { return (this._restrictGupAndCust ? this._gupCode : _selectedGup); }
			set { _selectedGup = value; DoSearchCust(); RaisePropertyChanged("SelectedGup"); }
		}
		#endregion
		#region Form - 可用的CUST (貨主)清單
		private ObservableCollection<NameValuePair<string>> _custList;
		public ObservableCollection<NameValuePair<string>> CustList
		{
			get { return _custList.OrderBy(x => x.Value).ToObservableCollection(); }
			set { _custList = value; RaisePropertyChanged("CustList"); }
		}
		private string _selectedCust = string.Empty;
		public string SelectedCust
		{
			get { return (this._restrictGupAndCust ? this._custCode : _selectedCust); }
			set { _selectedCust = value; RaisePropertyChanged("SelectedCust"); }
		}
		#endregion
		#region Form - 倉別型態WarehouseType
		private ObservableCollection<NameValuePair<string>> _warehouseTypeList;
		public ObservableCollection<NameValuePair<string>> WarehouseTypeList
		{
			get { return _warehouseTypeList; }
			set { _warehouseTypeList = value; RaisePropertyChanged("WarehouseTypeList"); }
		}
		public string _selectedWarehouseType = string.Empty;
		public string SelectedWarehouseType
		{
			get { return _selectedWarehouseType; }
			set { _selectedWarehouseType = value; DoSearchWarehouseList(); RaisePropertyChanged("SelectedWarehouseType"); }
		}
		#endregion
		#region Form - 倉別Warehouse
		private ObservableCollection<NameValuePair<string>> _warehouseList;
		public ObservableCollection<NameValuePair<string>> WarehouseList
		{
			get { return _warehouseList; }
			set { _warehouseList = value; RaisePropertyChanged("WarehouseList"); }
		}
		private string _selectedWarehouse = string.Empty;
		public string SelectedWarehouse
		{
			get { return _selectedWarehouse; }
			set 
			{ 
				_selectedWarehouse = value; 
				RaisePropertyChanged("SelectedWarehouse"); 
				DoSearchArea();
			}
		}
		#endregion
		#region Form - 儲位編號起迄
		private string _searchStartCode = string.Empty;
		public string SearchStartCode
		{
			get { return _searchStartCode; }
			set { _searchStartCode = value; RaisePropertyChanged("SearchStartCode"); }
		}
		private string _searchEndCode = string.Empty;
		public string SearchEndCode
		{
			get { return _searchEndCode; }
			set { _searchEndCode = value; RaisePropertyChanged("SearchEndCode"); }
		}
		#endregion
		#region Form - 儲區
		private ObservableCollection<NameValuePair<string>> _areaList;
		public ObservableCollection<NameValuePair<string>> AreaList
		{
			get { return _areaList; }
			set { _areaList = value; RaisePropertyChanged("AreaList"); }
		}
		private string _selectedArea = string.Empty;
		public string SelectedArea
		{
			get { return _selectedArea; }
			set { _selectedArea = value; RaisePropertyChanged("SelectedArea"); DoSearchChannelList(); }
		}
		#endregion
		#region 通道
		private ObservableCollection<NameValuePair<string>> _channelList;
		public ObservableCollection<NameValuePair<string>> ChannelList
		{
			get { return _channelList; }
			set { _channelList = value; RaisePropertyChanged("ChannelList"); }
		}
		private string _selectedChannel = string.Empty;
		public string SelectedChannel
		{
			get { return _selectedChannel; }
			set { _selectedChannel = value; RaisePropertyChanged("SelectedChannel"); }
		}
		#endregion
		#region Form - 查詢依據
		private bool _searchByLoc = false;
		public bool SearchByLoc
		{
			get { return _searchByLoc; }
			set { _searchByLoc = value; RaisePropertyChanged("SearchByLoc"); }
		}
		private bool _searchByArea = true;
		public bool SearchByArea
		{
			get { return _searchByArea; }
			set { _searchByArea = value; RaisePropertyChanged("SearchByArea"); }
		}
		#endregion
		#region Data - 查詢結果
		private List<F1912WithF1980> _locList = new List<F1912WithF1980>(new List<F1912WithF1980>());
		public List<F1912WithF1980> LocList
		{
			get { return _locList; }
			set
			{
				_locList = value;
				RaisePropertyChanged("LocList");
			}
		}
		#endregion
		#region Form - DataGrid選取
		private F1912WithF1980 _selectData;

		public F1912WithF1980 SelectData
		{
			get { return _selectData; }
			set
			{
				_selectData = value;
				RaisePropertyChanged("SelectData");
			}
		}
		#endregion
		#region Form - DataGrid勾選
		private List<F1912WithF1980> _selectedDataList;

		public List<F1912WithF1980> SelectedDataList
		{
			get { return _selectedDataList; }
			set
			{
				_selectedDataList = value;
				RaisePropertyChanged("SelectedDataList");
			}
		}
		#endregion

		#region Form - 料架設備
		private ObservableCollection<NameValuePair<string>> _locTypeList;
		public ObservableCollection<NameValuePair<string>> LOCTypeList
		{
			get { return _locTypeList; }
			set
			{
				_locTypeList = value;
				RaisePropertyChanged("LOCTypeList");
			}
		}
		private string _selectedLOCType = string.Empty;
		public string SelectedLOCType
		{
			get { return _selectedLOCType; }
			set {
                _selectedLOCType = value;
                DoSearchLOCType();
                RaisePropertyChanged("SelectedLOCType"); }
		}
        //是否調整
        private bool _isSaveLocType = false;
        public bool IsSaveLocType
        {
            get { return _isSaveLocType; }
            set { _isSaveLocType = value; RaisePropertyChanged("IsSaveLocType"); }
        }
        #endregion
        #region Form - 水平距離
        private string _txtHOR_DISTANCE;
		public string txtHOR_DISTANCE
		{
			get { return _txtHOR_DISTANCE; }
            set { _txtHOR_DISTANCE = value; RaisePropertyChanged("txtHOR_DISTANCE"); }
            //set
            //{
            //    float f;
            //    if (float.TryParse(value, out f) == false) return;
            //    _txtHOR_DISTANCE = value;
            //    RaisePropertyChanged("txtHOR_DISTANCE");
            //}

        }

        //是否調整
        private bool _isSaveHorDistance= false;
        public bool IsSaveHorDistance
        {
            get { return _isSaveHorDistance; }
            set { _isSaveHorDistance = value; RaisePropertyChanged("IsSaveHorDistance"); }
        }
        #endregion
        #region Form - 便利性
        private string _txtHANDY;
		public string txtHANDY
		{
			get { return _txtHANDY; }
			set
			{
				_txtHANDY = value;
				RaisePropertyChanged("txtHANDY");
			}
		}

        //是否調整
        private bool _isSaveHandy = false;
        public bool IsSaveHandy
        {
            get { return _isSaveHandy; }
            set { _isSaveHandy = value; RaisePropertyChanged("IsSaveHandy"); }
        }
        #endregion
        #region Form - 租用期限(起)
        private DateTime? _dtRENT_BEGIN_DATE;
		public DateTime? dtRENT_BEGIN_DATE
		{
			get { return _dtRENT_BEGIN_DATE; }
			set
			{
				_dtRENT_BEGIN_DATE = value;
				RaisePropertyChanged("dtRENT_BEGIN_DATE");
			}
		}
        //是否調整
        private bool _isSaveRentBaginDate = false;
        public bool IsSaveRentBaginDate
        {
            get { return _isSaveRentBaginDate; }
            set { _isSaveRentBaginDate = value; RaisePropertyChanged("IsSaveRentBaginDate"); }
        }
        #endregion
        #region Form - 租用期限(迄)
        private DateTime? _dtRENT_END_DATE;
		public DateTime? dtRENT_END_DATE
		{
			get { return _dtRENT_END_DATE; }
			set
			{
				_dtRENT_END_DATE = value;
				RaisePropertyChanged("dtRENT_END_DATE");
			}
		}
        //是否調整
        private bool _isSaveRentEndDate = false;
        public bool IsSaveRentEndDate
        {
            get { return _isSaveRentEndDate; }
            set { _isSaveRentEndDate = value; RaisePropertyChanged("IsSaveRentEndDate"); }
        }
        #endregion

        #region Form - 是否要顯示GUP/ CUST下拉選單
        public Visibility GupVisibility { get { return (this._restrictGupAndCust ? Visibility.Hidden : Visibility.Visible); } }
		public Visibility CustVisibility { get { return (this._restrictGupAndCust ? Visibility.Hidden : Visibility.Visible); } }
		#endregion
		#region Form - 勾選所有
		private bool _isCheckAll = false;
		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set { _isCheckAll = value; RaisePropertyChanged("IsCheckAll"); }
		}
		#endregion
		#endregion

		public P7101030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		public P7101030000_ViewModel(bool restrictGupAndCust = false)
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				this._restrictGupAndCust = restrictGupAndCust;
				InitControls();
			}
		}

		#region 函式
		private void InitControls()
		{
			DoSearchWarehouseType();
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any()) SelectedDc = DcList.First().Value;
		
		}

		private void DoSearchGup()
		{
			if (this._restrictGupAndCust)
			{
				// 如果是從P17進來的, 則限制只能存取當前的GUP
				GupList = new List<NameValuePair<string>>() {
					new NameValuePair<string>() {
						Value = this._gupCode,
						Name = Wms3plSession.Get<GlobalInfo>().GupName
					}
				}.ToObservableCollection();
			}
			else
			{
				var result = Wms3plSession.Get<GlobalInfo>().GetGupDataList(SelectedDc);
				result.Insert(0, new NameValuePair<string>() { Value = "0", Name = Resources.Resources.All });
				GupList = result.ToObservableCollection();
			}
			if(GupList.Any())
				SelectedGup = GupList.First().Value;
		}

		private void DoSearchCust()
		{
			if (this._restrictGupAndCust)
			{
				// 如果是從P17進來的, 則限制只能存取當前的CUST
				CustList = new List<NameValuePair<string>>() {
					new NameValuePair<string>() {
						Value = this._custCode,
						Name = Wms3plSession.Get<GlobalInfo>().CustName
					}
				}.ToObservableCollection();
			}
			else
			{
				var gupCode = (SelectedGup == "0") ? null : SelectedGup;
				var result = Wms3plSession.Get<GlobalInfo>().GetCustDataList(SelectedDc, gupCode);
				result.Insert(0, new NameValuePair<string>() { Value = "0", Name = Resources.Resources.All });
				CustList = result.ToObservableCollection();
			}
			if(CustList.Any())
				SelectedCust = CustList.First().Value;
		}

		private void DoSearchArea()
		{
			var result = new List<NameValuePair<string>>();
			if(!string.IsNullOrWhiteSpace(SelectedDc) && !string.IsNullOrWhiteSpace(SelectedWarehouse))
			{
				var proxy = GetProxy<F19Entities>();
				result = proxy.F1919s.Where(x => x.DC_CODE == SelectedDc && x.WAREHOUSE_ID == SelectedWarehouse)
										.OrderBy(x => x.AREA_CODE)
										.Select(x => new NameValuePair<string>() { Value = x.AREA_CODE, Name = x.AREA_NAME })
										.ToList();
			}
			result.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
			AreaList = result.ToObservableCollection();
			if (AreaList.Any())
				SelectedArea = AreaList.First().Value;
		}

		private void DoSearchChannelList()
		{
			var channelList = new List<NameValuePair<string>>();
			if (!string.IsNullOrWhiteSpace(SelectedDc) && !string.IsNullOrWhiteSpace(SelectedWarehouse))
			{
				var proxyEx = GetExProxy<P71ExDataSource>();
				channelList = proxyEx.CreateQuery<NameValueList>("GetDcWarehouseChannels")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("warehouseId", SelectedWarehouse)
					.AddQueryExOption("areaCode", SelectedArea).Select(x => new NameValuePair<string> { Name = x.Name, Value = x.Value }).ToList();
			}
			channelList.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
			ChannelList = channelList.ToObservableCollection();
			if(ChannelList.Any())
				SelectedChannel = ChannelList.First().Value;
		}

		private void DoSearchLOCType()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1942s.Select(x => new NameValuePair<string>() { Value = x.LOC_TYPE_ID, Name = x.HANDY })
				.ToList().Where(x=>x.Value==SelectedLOCType).AsQueryable();
			if(result.Any())
				txtHANDY = result.First().Name;
		}

		private void DoSearchWarehouseType()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F198001s
				.OrderBy(x => x.TYPE_ID)
				.Select(x => new NameValuePair<string>() { Value = x.TYPE_ID, Name = x.TYPE_NAME })
				.ToList();
			WarehouseTypeList = result.ToObservableCollection();
			if (WarehouseTypeList.Any())
				SelectedWarehouseType = WarehouseTypeList.First().Value;
		}

		private void DoSearchWarehouseList()
		{
			var warehouseList = new List<NameValuePair<string>>();
			var proxy = GetProxy<F19Entities>();
			if(!string.IsNullOrWhiteSpace(SelectedDc))
			{
				if (!string.IsNullOrWhiteSpace(SelectedWarehouseType))
				{
					warehouseList = proxy.F1980s.Where(x => x.DC_CODE.Equals(SelectedDc) && x.WAREHOUSE_TYPE.Equals(SelectedWarehouseType))
				.OrderBy(x => x.WAREHOUSE_ID)
				.Select(x => new NameValuePair<string>() { Value = x.WAREHOUSE_ID, Name = x.WAREHOUSE_NAME })
				.ToList();
				}
				else
				{
					warehouseList = proxy.F1980s.Where(x => x.DC_CODE.Equals(SelectedDc) )
				.OrderBy(x => x.WAREHOUSE_ID)
				.Select(x => new NameValuePair<string>() { Value = x.WAREHOUSE_ID, Name = x.WAREHOUSE_NAME })
				.ToList();
				}
			}
			WarehouseList = warehouseList.ToObservableCollection();
			if(WarehouseList.Any())
				SelectedWarehouse = WarehouseList.First().Value;
		}

		private void SetOperateModeVisibility()
		{
			VisibilityQueryMode = UserOperateMode == OperateMode.Query ? Visibility.Visible : Visibility.Collapsed;
			VisibilityEditMode = UserOperateMode == OperateMode.Edit ? Visibility.Visible : Visibility.Collapsed;
			if(UserOperateMode==OperateMode.Edit)
			{
                //預設空值
				SetLOCTypeList();
                txtHOR_DISTANCE = "";
                txtHANDY = "";
                dtRENT_BEGIN_DATE = null;
                dtRENT_END_DATE = null;
                IsSaveHorDistance = false;
                IsSaveLocType = false;
                IsSaveHandy = false;
                IsSaveRentBaginDate = false;
                IsSaveRentEndDate = false;
                //txtHOR_DISTANCE = SelectData.HOR_DISTANCE.ToString();
                //txtHANDY = SelectData.HANDY;
                //            dtRENT_BEGIN_DATE = SelectData.RENT_BEGIN_DATE;
                //            dtRENT_END_DATE = SelectData.RENT_END_DATE;
                //            //dtRENT_BEGIN_DATE = SelectData.RENT_BEGIN_DATE!=null ? SelectData.RENT_BEGIN_DATE.Value : DateTime.Now;
                //            //dtRENT_END_DATE = SelectData.RENT_END_DATE != null ? SelectData.RENT_END_DATE.Value : DateTime.Now;
            }
		}

		private void SetLOCTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1942s.Select(x => new NameValuePair<string>() { Value = x.LOC_TYPE_ID, Name = x.LOC_TYPE_NAME })
				.ToList();
			LOCTypeList = result.ToObservableCollection();
            //if (LOCTypeList.Any())
            //    SelectedLOCType = LOCTypeList.FirstOrDefault().Value;
            //if (SelectData != null)
            //	SelectedLOCType = SelectData.LOC_TYPE_ID;
            //else if (LOCTypeList.Any())
            //	SelectedLOCType = LOCTypeList.FirstOrDefault().Value;
        }

		private List<F1912WithF1980> GetSelectedDataList()
		{
			var sel = LocList.Where(x => x.IsSelected == true).ToList();
			return sel;
		}

        //檢核是否調整選項,若取消則將欄位清空
        public void CheckUpdateSet(string type)
        {
            //水平距離
            if (type== "HorDistance")
            {
                if(!IsSaveHorDistance)
                {
                    txtHOR_DISTANCE = "";
                }
            }
            //料架設備
            if (type== "LocType")
            {
                if (!IsSaveLocType)
                {
                    SelectedLOCType = "";
                }
            }
            //便利性
            if (type == "Handy")
            {
                if (!IsSaveHandy)
                {
                    txtHANDY = "";
                }
            }
            //租用期限(起)
            if (type == "RentBaginDate")
            {
                if (!IsSaveRentBaginDate)
                {
                    dtRENT_BEGIN_DATE = null;
                }
            }
            //租用期限(迄)
            if (type == "RentEndDate")
            {
                if (!IsSaveRentEndDate)
                {
                    dtRENT_END_DATE = null;
                }
            }
        }
        #endregion

        #region Command
        #region CheckAll
        private ICommand _checkAllCommand;
		public ICommand CheckAllCommand
		{
			get
			{
				if (_checkAllCommand == null)
				{
					_checkAllCommand = new RelayCommand(DoCheckAllItem);
				}
				return _checkAllCommand;
			}
		}

		public void DoCheckAllItem()
		{
			foreach (var p in LocList)
			{
				p.IsSelected = IsCheckAll;
			}
		}
		#endregion
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, c => DoSearchComplete()
				);
			}
		}

		private void DoSearch()
		{
			IsCheckAll = false;
			//執行查詢動作
			if (SearchByLoc)
			{
				var proxyEx = GetExProxy<P71ExDataSource>();
        //檢查儲位編號為必填
        if (string.IsNullOrWhiteSpace(SearchStartCode) || string.IsNullOrWhiteSpace(SearchEndCode))
        {
          DialogService.ShowMessage("請輸入儲位編號");
          return;
        }
				var result = proxyEx.CreateQuery<F1912WithF1980>("GetLocListForWarehouse")
					.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
					.AddQueryOption("gupCode", string.Format("'{0}'", (SelectedGup == "0" ? "" : SelectedGup)))
					.AddQueryOption("custCode", string.Format("'{0}'", (SelectedCust == "0" ? "" : SelectedCust)))
					.AddQueryOption("warehouseType", string.Format("'{0}'", SelectedWarehouseType))
					.AddQueryOption("warehouseId", string.Format("'{0}'", SelectedWarehouse))
					.AddQueryOption("locCodeS", string.Format("'{0}'", LocCodeHelper.LocCodeConverter9(SearchStartCode)))
					.AddQueryOption("locCodeE", string.Format("'{0}'", LocCodeHelper.LocCodeConverter9(SearchEndCode)))
					.AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
					.ToList();
				LocList = result;
			
			}
			else if (SearchByArea)
			{
				var proxyEx = GetExProxy<P71ExDataSource>();
				var result = proxyEx.CreateQuery<F1912WithF1980>("GetLocListForWarehouse")
					.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
					.AddQueryOption("gupCode", string.Format("'{0}'", (SelectedGup=="0" ? "":SelectedGup)))
					.AddQueryOption("custCode", string.Format("'{0}'", (SelectedCust=="0" ? "":SelectedCust)))
					.AddQueryOption("warehouseType", string.Format("'{0}'", SelectedWarehouseType))
					.AddQueryOption("warehouseId", string.Format("'{0}'", SelectedWarehouse))
					.AddQueryOption("areaCode", string.Format("'{0}'", SelectedArea))
          .AddQueryOption("channel", string.Format("'{0}'", SelectedChannel))
          .AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
					.ToList();
				LocList = result;
			}
			RaisePropertyChanged();
			if (!LocList.Any()) ShowMessage(Messages.InfoNoData);
		}

		private void DoSearchComplete()
		{
			OnSearch();
		}
		#endregion Search
		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && LocList.Any(x => x.IsSelected == true), c => SetOperateModeVisibility()
				);
			}
		}

		private void DoEdit()
		{
			SelectedDataList = GetSelectedDataList();
			if (SelectedDataList.Any())
				SelectData = SelectedDataList.FirstOrDefault();
			//執行編輯動作
			UserOperateMode = OperateMode.Edit;
		}
		#endregion Edit
		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query,c=>SetOperateModeVisibility()
					);
			}
		}

		private void DoCancel()
		{
			var msg = Messages.WarningBeforeDelete;
			msg.Message = Properties.Resources.P7101030000_ViewModel_CancelEdit;
			if (ShowMessage(msg) != DialogResponse.Yes) return;
			//執行取消動作
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel
		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),() => LocList != null && LocList.Any()
				);
			}
		}

		private void DoSave()
		{
			MessagesStruct SearchDateMsg = new MessagesStruct();
			SearchDateMsg.Message = Properties.Resources.P7101030000_ViewModel_BeginDate_GreaterThan_EndDate;
			SearchDateMsg.Button = DialogButton.OK;
			SearchDateMsg.Image = DialogImage.Warning;
			SearchDateMsg.Title = Resources.Resources.Information;
			if (dtRENT_BEGIN_DATE != null && dtRENT_END_DATE != null)
			{
				if (dtRENT_BEGIN_DATE > dtRENT_END_DATE)
				{
					ShowMessage(SearchDateMsg);
					return;
				}
			}
			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes) return;
			//執行確認儲存動作
			//if (DoCheckData())
			//{
				var proxywcf = new wcf.P71WcfServiceClient();
				var error = Messages.ErrorAddFailed;
				error.Message=string.Empty;
				foreach(var p in SelectedDataList)
				{
				    var f1912withf1980 = new wcf.F1912WithF1980();
					f1912withf1980.DC_CODE = p.DC_CODE;
					f1912withf1980.LOC_CODE = p.LOC_CODE;
					f1912withf1980.WAREHOUSE_ID = p.WAREHOUSE_ID;
                    if(!IsSaveLocType)
                        f1912withf1980.LOC_TYPE_ID = p.LOC_TYPE_ID;
                    else
                        f1912withf1980.LOC_TYPE_ID = SelectedLOCType;
                    if(!IsSaveRentBaginDate)
                        f1912withf1980.RENT_BEGIN_DATE = p.RENT_BEGIN_DATE;
                    else
                        f1912withf1980.RENT_BEGIN_DATE = dtRENT_BEGIN_DATE;
                    if (!IsSaveRentEndDate)
                        f1912withf1980.RENT_END_DATE = p.RENT_END_DATE;
                    else
                        f1912withf1980.RENT_END_DATE = dtRENT_END_DATE;
                    if (!IsSaveHorDistance)
                        f1912withf1980.HOR_DISTANCE = p.HOR_DISTANCE;
                    else
                        f1912withf1980.HOR_DISTANCE = decimal.Parse(txtHOR_DISTANCE);
                    f1912withf1980.HANDY = txtHANDY;

                    var result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.UpdateF1912WithF1980(f1912withf1980, _userId));
				    if (!result.IsSuccessed)
				    {
					error.Message += Environment.NewLine + result.Message;
					}
				}
				if (string.IsNullOrEmpty(error.Message))
				{
					ShowMessage(Messages.InfoUpdateSuccess);
				    UserOperateMode = OperateMode.Query;
					SelectedDataList = null;
				    SetOperateModeVisibility();
				    DoSearch();
                }
				else
					ShowMessage(error);
			//}
		}
		private bool DoCheckData()
		{
			string msg = string.Empty;
			if (string.IsNullOrEmpty(SelectData.HANDY)) msg = Properties.Resources.P7101030000_ViewModel_HandyEmpty;
			if (string.IsNullOrEmpty(msg))
			{
				return true;
			}
			else
			{
				ShowMessage(new MessagesStruct()
				{
					Button = DialogButton.OK,
					Image = DialogImage.Warning,
					Message = msg,
					Title = Resources.Resources.Information
				}
				);
				return false;
			}
		}
		#endregion Save
		#endregion
	}
}
