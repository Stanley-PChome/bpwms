using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
using Wms3pl.WpfClient.P71.Entities;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using System.Diagnostics;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7101020100_ViewModel : InputViewModelBase
	{
		public Action AdjustFrom = delegate { };
		public P7101020100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetATypeList();
                SetPickTypeList();
                SetPickToolList();
                SetPickSEQList();
                SetSortByList();
                SetPutToolList();
                SetPickUnitList();
                SetPickMarterialList();
				SetMoveToolList();

				//CreateStorageAreaPickCheckEnabled = false;
				CreateStorageAreaPickCheck = "0";

                _userId = Wms3plSession.CurrentUserInfo.Account;
				AreaCode = Properties.Resources.P7101010100_ViewModel_SysAutoNo;
			}

		}

		#region Property

		private string _userId;
		private bool IsSuccess = false;
		private bool _isConfirm = false;
		private bool _isChangeSelectData = false;
		private ExDataServices.P71ExDataService.F1980Data _f1980Data;

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

		#region 物流中心 貨主 業主

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

        private List<NameValuePair<string>> _pickTypeList;

        public List<NameValuePair<string>> PickTypeList
        {
            get { return _pickTypeList; }
            set
            {
                _pickTypeList = value;
                RaisePropertyChanged("PICK_TYPE");
            }
        }

        private string _selectedPickType="";

        public string SelectedPickType
        {
            get { return _selectedPickType; }
            set
            {
                _selectedPickType = value;
                RaisePropertyChanged("SelectedPickType");
            }
        }

        private List<NameValuePair<string>> _pickToolList;

        public List<NameValuePair<string>> PickToolList
        {
            get { return _pickToolList; }
            set
            {
                _pickToolList = value;
                RaisePropertyChanged("PickToolList");
            }
        }

        private string _selectedPickTool = "";

        public string SelectedPickTool
        {
            get { return _selectedPickTool; }
            set
            {
                _selectedPickTool = value;
                RaisePropertyChanged("SelectedPickTool");
            }
        }

        private List<NameValuePair<string>> _pickSEQList;

        public List<NameValuePair<string>> PickSEQList
        {
            get { return _pickSEQList; }
            set
            {
                _pickSEQList = value;
                RaisePropertyChanged("PickSEQList");
            }
        }

        private string _selectedPickSEQ = "";

        public string SelectedPickSEQ
        {
            get { return _selectedPickSEQ; }
            set
            {
                _selectedPickSEQ = value;
                RaisePropertyChanged("SelectedPickSEQ");
            }
        }

        private List<NameValuePair<string>> _sortByList;

        public List<NameValuePair<string>> SortByList
        {
            get { return _sortByList; }
            set
            {
                _sortByList = value;
                RaisePropertyChanged("SortByList");
            }
        }

		private string _selectedSortBy = "";

        public string SelectedSortBy
        {
            get { return _selectedSortBy; }
            set
            {
                _selectedSortBy = value;
                RaisePropertyChanged("SelectedSortBy");
            }
		}

		private List<NameValuePair<string>> _moveToolList;
		public List<NameValuePair<string>> MoveToolList
		{
			get { return _moveToolList; }
			set { Set(ref _moveToolList, value); }
		}
		private string _selectedMoveTool;
		public string SelectedMoveTool
		{
			get { return _selectedMoveTool; }
			set { Set(ref _selectedMoveTool, value); }
		}

		private List<NameValuePair<string>> _putToolList;

        public List<NameValuePair<string>> PutToolList
        {
            get { return _putToolList; }
            set
            {
                _putToolList = value;
                RaisePropertyChanged("PutToolList");
            }
        }

        private string _selectedPutTool = "";

        public string SelectedPutTool
        {
            get { return _selectedPutTool; }
            set
            {
                _selectedPutTool = value;
                RaisePropertyChanged("SelectedPutTool");
            }
        }

        private List<NameValuePair<string>> _pickUnitList;

        public List<NameValuePair<string>> PickUnitList
        {
            get { return _pickUnitList; }
            set
            {
                _pickUnitList = value;
                RaisePropertyChanged("PickUnitList");
            }
        }

        private string _selectedPickUnit = "";

        public string SelectedPickUnit
        {
            get { return _selectedPickUnit; }
            set
            {
                _selectedPickUnit = value;
                RaisePropertyChanged("SelectedPickUnit");
            }
        }

        private List<NameValuePair<string>> _pickMarterialList;

        public List<NameValuePair<string>> PickMarterialList
        {
            get { return _pickMarterialList; }
            set
            {
                _pickMarterialList = value;
                RaisePropertyChanged("PickMarterialList");
            }
        }

        private string _selectedPickMarterial = "";

        public string SelectedPickMarterial
        {
            get { return _selectedPickMarterial; }
            set
            {
                _selectedPickMarterial = value;
                RaisePropertyChanged("SelectedPickMarterial");
            }
        }

        private List<NameValuePair<string>> _deliveryMarterialList;

        public List<NameValuePair<string>> DeliveryMarterialList
        {
            get { return _deliveryMarterialList; }
            set
            {
                _deliveryMarterialList = value;
                RaisePropertyChanged("DeliveryMarterialList");
            }
        }

        private string _selectedDeliveryMarterial = "";

        public string SelectedDeliveryMarterial
        {
            get { return _selectedDeliveryMarterial; }
            set
            {
                _selectedDeliveryMarterial = value;
                RaisePropertyChanged("SelectedDeliveryMarterial");
            }
        }

        private string _checkSingleBox;

        public string CheckSingleBox
        {
            get { return _checkSingleBox; }
            set
            {
                _checkSingleBox = value;
                RaisePropertyChanged("CheckSingleBox");
            }
        }

        private string _checkPickCheck;

        public string CheckPickCheck
        {
            get { return _checkPickCheck; }
            set
            {
                _checkPickCheck = value;
                RaisePropertyChanged("CheckPickCheck");
            }
        }

        #endregion

        private bool _isBindSuccess = true;
        public bool IsBindSuccess
        {
            get { return _isBindSuccess; }
            set { Set(ref _isBindSuccess, value); }
        }


        /// <summary>
        /// 建立儲區揀貨設定屬性
        /// </summary>
        private string _createStorageAreaPickCheck;

        public string CreateStorageAreaPickCheck
        {
            get { return _createStorageAreaPickCheck; }
            set
            {
                _createStorageAreaPickCheck = value;
                if (CreateStorageAreaPickCheck == "1")
                {
                    CreateStorageAreaPickCheckEnabled = true;
                }
                else
                {
                    CreateStorageAreaPickCheckEnabled = false;
                }
                RaisePropertyChanged("CreateStorageAreaPickCheck");
            }
        }

        private bool _createStorageAreaPickCheckEnabled;
        public bool CreateStorageAreaPickCheckEnabled
        {
            get { return _createStorageAreaPickCheckEnabled; }
            set
            {
                _createStorageAreaPickCheckEnabled = value;
                RaisePropertyChanged("CreateStorageAreaPickCheckEnabled");
            }
        }

        #region 儲位範圍

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
				if (!string.IsNullOrWhiteSpace(value))
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

		private List<F191201> _locSetData;
		public List<F191201> LocSetData
		{
			get { return _locSetData; }
			set { Set(ref _locSetData, value); }
		}
		#endregion

		#endregion

		#region 下拉式選單資料來源

		#region  物流中心 貨主 業主
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

        public void SetPickTypeList()
        {
            var proxy = GetProxy<F00Entities>();
            var data = proxy.F000904_I18N.Where(o => o.TOPIC == "F191902" && o.SUBTOPIC == "PICK_TYPE" && o.LANG == Wms3plSession.Get<GlobalInfo>().Lang).ToList();
            var pickTypeList = (from o in data
                            select new NameValuePair<string>()
                            {
                                Name = o.NAME,
                                Value = o.VALUE
                            }).ToList();
            PickTypeList = pickTypeList;
            if (PickTypeList.Any())
                SelectedPickType = pickTypeList.First().Value;    
        }

        public void SetPickToolList()
        {
            var proxy = GetProxy<F00Entities>();
            var data = proxy.F000904_I18N.Where(o => o.TOPIC == "F191902" && o.SUBTOPIC == "PICK_TOOL" && o.LANG == Wms3plSession.Get<GlobalInfo>().Lang).ToList();
            var pickToolList = (from o in data
                                select new NameValuePair<string>()
                                {
                                    Name = o.NAME,
                                    Value = o.VALUE
                                }).ToList();
            PickToolList = pickToolList;
            if (PickToolList.Any())
                SelectedPickTool = pickToolList.First().Value;
        }

        public void SetPickSEQList()
        {
            var proxy = GetProxy<F00Entities>();
            var data = proxy.F000904_I18N.Where(o => o.TOPIC == "F191902" && o.SUBTOPIC == "PICK_SEQ" && o.LANG == Wms3plSession.Get<GlobalInfo>().Lang).ToList();
            var pickSEQList = (from o in data
                                select new NameValuePair<string>()
                                {
                                    Name = o.NAME,
                                    Value = o.VALUE
                                }).ToList();
            PickSEQList = pickSEQList;
            if (PickSEQList.Any())
                SelectedPickSEQ = pickSEQList.First().Value;
        }

        public void SetSortByList()
        {
            var proxy = GetProxy<F00Entities>();
            var data = proxy.F000904_I18N.Where(o => o.TOPIC == "F191902" && o.SUBTOPIC == "SORT_BY" && o.LANG == Wms3plSession.Get<GlobalInfo>().Lang).ToList();
            var sortByList = (from o in data
                               select new NameValuePair<string>()
                               {
                                   Name = o.NAME,
                                   Value = o.VALUE
                               }).ToList();
            SortByList = sortByList;
            if (SortByList.Any())
                SelectedSortBy = sortByList.First().Value;
        }

		public void SetMoveToolList()
		{
			MoveToolList = GetBaseTableService.GetF000904List(FunctionCode, "F191902", "MOVE_TOOL");
			SelectedMoveTool = MoveToolList.FirstOrDefault().Value;
		}


		public void SetPutToolList()
        {
            var proxy = GetProxy<F00Entities>();
            var data = proxy.F000904_I18N.Where(o => o.TOPIC == "F191902" && o.SUBTOPIC == "PUT_TOOL" && o.LANG == Wms3plSession.Get<GlobalInfo>().Lang).ToList();
            var putToolList = (from o in data
                              select new NameValuePair<string>()
                              {
                                  Name = o.NAME,
                                  Value = o.VALUE
                              }).ToList();
            PutToolList = putToolList;
            if (PutToolList.Any())
                SelectedPutTool = putToolList.First().Value;
        }

        public void SetPickUnitList()
        {
            var proxy = GetProxy<F91Entities>();
            var data = proxy.F91000302s.Where(o => o.ITEM_TYPE_ID == "001").ToList();
            var pickUnitList = (from o in data
                                select new NameValuePair<string>()
                                {
                                    Name = o.ACC_UNIT_NAME,
                                    Value = o.ACC_UNIT
                                }).ToList();
            PickUnitList = pickUnitList;
            if (PickUnitList.Any())
                SelectedPickUnit = pickUnitList.First().Value;
        }

        public void SetPickMarterialList()
        {
            var proxy = GetProxy<F00Entities>();
            var data = proxy.F000904s.Where(o => o.TOPIC == "F1944" && o.SUBTOPIC == "PICK_MARTERIAL").ToList();
            var pickMarterialList = (from o in data
                                     select new NameValuePair<string>()
                                     {
                                         Name = o.NAME,
                                         Value = o.VALUE
                                     }).ToList();
            PickMarterialList = pickMarterialList;
            if (PickMarterialList.Any())
                SelectedPickMarterial = pickMarterialList.First().Value;
        }

        /// <summary>
        /// 出貨材積類型下拉是選單資料代入
        /// </summary>
        public void SetDeliveryMarterialList()
        {
            var proxy = GetProxy<F19Entities>();
            List<F1903> data = new List<F1903>();
            List<NameValuePair<string>> deliveryMarterialList = new List<NameValuePair<string>>();
            if (SelectedGupCode=="0")
            {
                //業主貨主都共用
                if (SelectedCustCode == "0")
                {
                    data = proxy.F1903s.Where(o => o.ISCARTON == "1").ToList();                        
                }
                //業主共用，貨主不共用
                else
                {
                   data = proxy.F1903s.Where(o => o.ISCARTON == "1" && o.CUST_CODE == SelectedCustCode).ToList();                             
                }
            }
            else
            {
                //業主不共用，貨主共用
                if (SelectedCustCode == "0")
                {
                    data = proxy.F1903s.Where(o => o.ISCARTON == "1" && o.GUP_CODE == SelectedGupCode).ToList();                   
                }
                //業主不共用，或主不共用
                else
                {
                    data = proxy.F1903s.Where(o => o.ISCARTON == "1" && o.CUST_CODE == SelectedCustCode && o.GUP_CODE == SelectedGupCode).ToList();                
                }
            }
            deliveryMarterialList = (from o in data
                                     select new NameValuePair<string>()
                                     {
                                         Name = o.ITEM_NAME,
                                         Value = o.ITEM_CODE
                                     }).ToList();
            DeliveryMarterialList = deliveryMarterialList;
            if (DeliveryMarterialList.Any())
                SelectedDeliveryMarterial = deliveryMarterialList.First().Value;
        }

        #endregion

        #region 儲位範圍

        /// <summary>
        /// 設定樓層清單
        /// </summary>
        public void SetFloorList()
		{

			FloorList = (from a in LocSetData
						 where a.TYPE == "1" && a.VALUE == _f1980Data.FLOOR && a.DC_CODE == _f1980Data.DC_CODE
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
			MaxChannelList = MinChannelList.Where(o => string.Compare(o.Value, startIndex) >= 0).ToList(); ;
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
			MaxPlainList = MinPlainList.Where(o => string.Compare(o.Value, startIndex) >= 0).ToList(); ;
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
			MaxLocLevelList = MinLocLevelList.Where(o => string.Compare(o.Value, startIndex) >= 0).ToList(); ;
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
			MaxLocTypeList = MinLocTypeList.Where(o => string.Compare(o.Value, startIndex) >= 0).ToList();
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

		#region
		private bool isRefresh = false;

		private List<F1912> _f1912Data;
		public List<F1912> F1912Data
		{
			get { return _f1912Data; }
			set { Set(ref _f1912Data, value); }
		}

		private ObservableCollection<P710102MasterData> _oldMasterDataList;
		public ObservableCollection<P710102MasterData> OldMasterDataList
		{
			get { return _oldMasterDataList; }
			set { Set(ref _oldMasterDataList, value); }
		}

		private ObservableCollection<P710102MasterData> _masterDataList;
		public ObservableCollection<P710102MasterData> MasterDataList
		{
			get { return _masterDataList; }
			set { Set(ref _masterDataList, value); }
		}

		private P710102MasterData _selectMasertData;
		public P710102MasterData SelectMasertData
		{
			get { return _selectMasertData; }
			set
			{
				Set(ref _selectMasertData, value);
				if (TempLocCodeList != null && DetailDataList != null)
				{
					var tmpData = TempLocCodeList;
					var nowData = DetailDataList.Where(o => o.IsSelected);
					var isModify = (!tmpData.All(x => nowData.Any(y => y.Item.LOC_CODE == x)) || tmpData.Count() != nowData.Count());

					if (isModify && !isRefresh)
					{
						if (ShowConfirmMessage(Properties.Resources.P7101010100_ViewModel_DataEditCheck) == DialogResponse.Yes)
							ConfirmData();
						else
						{
							var oldSelectLoc = DetailDataList.Where(o => tmpData.Contains(o.Item.LOC_CODE)).Select(o => o.Item.LOC_CODE).ToList();
							foreach (var loc in oldSelectLoc)
							{
								if (!SelectedLoc.Contains(loc))
									SelectedLoc.Add(loc);
							}
							var oldLoc = DetailDataList.Where(o => !tmpData.Contains(o.Item.LOC_CODE)).Select(o => o.Item.LOC_CODE).ToList();
							foreach (var loc in oldLoc)
							{
								if (SelectedLoc.Contains(loc))
									SelectedLoc.Remove(loc);
							}
							var param = DetailDataList.FirstOrDefault().Item;
							var oldSelectMaster = OldMasterDataList.Where(o => o.ChannelNo == param.CHANNEL && o.PlainNo == param.PLAIN && o.LocLevelNo == param.LOC_LEVEL).FirstOrDefault();

							CountMasterLoc(oldSelectMaster);
						}
					}
				}
				if (value != null && !isRefresh)
					GetDetailData(SelectedQueryFloor, SelectedAreaType);
				_isChangeSelectData = true;
				if (!_isConfirm && !isRefresh)
					IsJobSelectedAll = false;
				else
					_isConfirm = false;
				_isChangeSelectData = false;
			}
		}

		private ObservableCollection<wcf.P710101DetailData> _oldEditDetailDataList;
		public ObservableCollection<wcf.P710101DetailData> OldEditDetailDataList
		{
			get { return _oldEditDetailDataList; }
			set { Set(ref _oldEditDetailDataList, value); }
		}

		private List<wcf.P710101DetailData> _oldDetailDataList;
		public List<wcf.P710101DetailData> OldDetailDataList
		{
			get { return _oldDetailDataList; }
			set { Set(ref _oldDetailDataList, value); }
		}

		private SelectionList<wcf.P710101DetailData> _detailDataList;
		public SelectionList<wcf.P710101DetailData> DetailDataList
		{
			get { return _detailDataList; }
			set { Set(ref _detailDataList, value); }
		}

		private List<string> _selectedLoc;
		public List<string> SelectedLoc
		{
			get { return _selectedLoc; }
			set { Set(ref _selectedLoc, value); }
		}

		private List<string> _editSelectedLoc;
		public List<string> EditSelectedLoc
		{
			get { return _editSelectedLoc; }
			set { Set(ref _editSelectedLoc, value); }
		}

		private ObservableCollection<wcf.P710101DetailData> _nowLocData;
		public ObservableCollection<wcf.P710101DetailData> NowLocData
		{
			get { return _nowLocData; }
			set { Set(ref _nowLocData, value); }
		}

		private ObservableCollection<NameValuePair<string>> _queryFloorList;
		public ObservableCollection<NameValuePair<string>> QueryFloorList
		{
			get { return _queryFloorList; }
			set { Set(ref _queryFloorList, value); }
		}

		private ObservableCollection<NameValuePair<string>> _areaType;
		public ObservableCollection<NameValuePair<string>> AreaType
		{
			get { return _areaType; }
			set { Set(ref _areaType, value); }
		}

		private string _selectedAreaType;
		public string SelectedAreaType
		{
			get { return _selectedAreaType; }
			set
			{
				Set(ref _selectedAreaType, value);
				if (!string.IsNullOrWhiteSpace(_selectedAreaType))
					GetDetailData(SelectedQueryFloor, _selectedAreaType);
			}
		}

		private List<string> _tempLocCodeList;
		public List<string> TempLocCodeList
		{
			get { return _tempLocCodeList; }
			set { Set(ref _tempLocCodeList, value); }
		}

		private string _selectedQueryFloor;
		public string SelectedQueryFloor
		{
			get { return _selectedQueryFloor; }
			set
			{
				Set(ref _selectedQueryFloor, value);
				if (!string.IsNullOrWhiteSpace(SelectedQueryFloor))
				{
					RefreshDataGrid();
					SelectMasertData = MasterDataList?.FirstOrDefault();
				}
				CountMasterLoc(SelectMasertData, true);
			}
		}

		private int _oldLocTotalCount;
		public int OldLocTotalCount
		{
			get { return _oldLocTotalCount; }
			set { Set(ref _oldLocTotalCount, value); }
		}

		private int _locTotalCount;
		public int LocTotalCount
		{
			get { return _locTotalCount; }
			set { Set(ref _locTotalCount, value); }
		}

		private List<NameValuePair<string>> _locStatusList;
		public List<NameValuePair<string>> LocStatusList
		{
			get { return _locStatusList; }
			set { Set(ref _locStatusList, value); }
		}

		private List<NameValuePair<string>> _settingStatusList;
		public List<NameValuePair<string>> SettingStatusList
		{
			get { return _settingStatusList; }
			set { Set(ref _settingStatusList, value); }
		}

		private bool _isJobSelectedAll;
		public bool IsJobSelectedAll
		{
			get { return _isJobSelectedAll; }
			set
			{
				Set(ref _isJobSelectedAll, value);
				if (!_isChangeSelectData)
				{
					foreach (var locCode in DetailDataList)
					{
						if (!locCode.Item.IsEditData) continue;

						locCode.IsSelected = (locCode.Item.IsEditData && _isJobSelectedAll);
						if (_isJobSelectedAll)
						{
							if (SelectedLoc.Contains(locCode.Item.LOC_CODE))
								SelectedLoc.Remove(locCode.Item.LOC_CODE);
						}
						else
						{
							if (!SelectedLoc.Contains(locCode.Item.LOC_CODE))
								SelectedLoc.Add(locCode.Item.LOC_CODE);
						}
					}
				}
			}
		}

		private bool _isAdd;
		public bool IsAdd
		{
			get { return _isAdd; }
			set { Set(ref _isAdd, value); }
		}
		#endregion

		#region 資料繫結

		public Boolean Bind(string dcCode, string gupCode, string custCode, string warehouseId)
		{
			var proxyEx = GetExProxy<P71ExDataSource>();
            var data = proxyEx.CreateQuery<ExDataServices.P71ExDataService.F1980Data>("GetF1980Datas")
                .AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
                .AddQueryOption("gupCode", string.Format("'{0}'", gupCode))
                .AddQueryOption("custCode", string.Format("'{0}'", custCode))
                .AddQueryOption("warehourseId", string.Format("'{0}'", warehouseId))
                .AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
                .Where(o=>o.WAREHOUSE_ID== warehouseId)
				.ToList();
            if (data == null || data.Count == 0)
                return false;
			_f1980Data = data.First(o => o.WAREHOUSE_ID == warehouseId);

			SelectedDcCode = _f1980Data.DC_CODE;
			SelectedGupCode = _f1980Data.GUP_CODE;
			SelectedCustCode = _f1980Data.CUST_CODE;
			SelectedDcName = DcList.Find(o => o.Value == SelectedDcCode).Name;
			SelectedGupName = GupList.Find(o => o.Value == SelectedGupCode).Name;
			SelectedCustName = CustList.Find(o => o.Value == SelectedCustCode).Name;
			GetLocSetDatas();
			WarehouseId = warehouseId;
			WarehouseName = _f1980Data.WAREHOUSE_Name;
			BindLocData();
            SetDeliveryMarterialList();

            return true;
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
                    DC_CODE = _selectedDcCode,
                    WAREHOUSE_ID = _warehouseId,
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
                    AREA_CODE = IsAdd ? "-1" : AreaCode,
                    AREA_NAME = _areaName,
                    ATYPE_CODE = _selectedAType,
                    PICK_TYPE = _selectedPickType ?? " ",
                    PICK_TOOL = _selectedPickTool ?? " ",
                    PUT_TOOL = _selectedPutTool ?? " ",
                    PICK_SEQ = _selectedPickSEQ ?? " ",
                    SORT_BY = _selectedSortBy ?? " ",
                    SINGLE_BOX = _checkSingleBox ?? " ",
                    PICK_CHECK = _checkPickCheck ?? " ",
                    PICK_UNIT = _selectedPickUnit ?? " ",
                    PICK_MARTERIAL = _selectedPickMarterial ?? " ",
                    DELIVERY_MARTERIAL = _selectedDeliveryMarterial ?? " ",
                    IsCreateStorageAreaPickSetting = CreateStorageAreaPickCheck == "1" ? true : false,
					MOVE_TOOL = SelectedMoveTool ?? " "
				};

                var result = new wcf.ExecuteResult();
                if (IsAdd)
                    result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.InsertF1919Data(f1919Data, _userId, SelectedLoc.ToArray()));
                else
                {
                    //如果有勾就直接修改
                    if (CreateStorageAreaPickCheck=="1")
                    {
                        result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.UpdateF1919Data(f1919Data, _userId, SelectedLoc.ToArray()));
                    }
                    //如果沒有勾，就詢問是否要刪除
                    else
                    {
												var proxy = GetProxy<F19Entities>();
												var f191902 = proxy.F191902s.Where(o => o.DC_CODE == f1919Data.DC_CODE &&
												o.GUP_CODE == (f1919Data.GUP_CODE ?? "0") &&
												o.CUST_CODE == (f1919Data.CUST_CODE ?? "0") &&
												o.WAREHOUSE_ID == f1919Data.WAREHOUSE_ID &&
												o.AREA_CODE == f1919Data.AREA_CODE).FirstOrDefault();

												if (f191902 != null)
												{
													if (ShowMessage(new MessagesStruct() { Button = DialogButton.YesNo, Image = DialogImage.Warning, Message = Properties.Resources.P7101020100_DeleteSetting }) == DialogResponse.Yes)
													{
														result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.UpdateF1919Data(f1919Data, _userId, SelectedLoc.ToArray()));
													}
													else
														return;
												}
												else
												{
													result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.UpdateF1919Data(f1919Data, _userId, SelectedLoc.ToArray()));
												}
                    }
                }

                if (result.IsSuccessed)
                {
                    IsSuccess = true;
                    if (IsAdd)
                        ShowMessage(Messages.InfoAddSuccess);
                    else
                        ShowMessage(Messages.InfoUpdateSuccess);
                }
                else
                {
                    var error = Messages.ErrorAddFailed;
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

		public ICommand ConfirmCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => { },
					  () => true, o =>
					  {
						  ConfirmData();
					  });
			}
		}

		/// <summary>
		/// 設定儲區資料
		/// </summary>
		private void SetAreaList()
		{
			AreaType = new ObservableCollection<NameValuePair<string>>();
			AreaType.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_NoneSelect, Value = "-1 " });
			AreaType.Add(new NameValuePair<string> { Name = Resources.Resources.All, Value = "0" });
			SelectedAreaType = AreaType.FirstOrDefault(o => o.Value == "0").Value;
		}

		/// <summary>
		/// 取得儲位設定資料
		/// </summary>
		private void GetLocSetDatas()
		{
			var proxy = GetProxy<F19Entities>();
			LocSetData = proxy.F191201s.Where(x => x.DC_CODE == SelectedDcCode).OrderBy(o => o.VALUE).ToList();
			SetAreaList();
			SetFloorList();
			SetMinChanelList();
			SetMinPlainList();
			SetMinLocLevelList();
			SetMinLocTypeList();
			LocStatusList = GetBaseTableService.GetF000904List(FunctionCode, "P7101010000", "LocStatus");
			SettingStatusList = GetBaseTableService.GetF000904List(FunctionCode, "P7101010000", "SettingStatus");
		}

		private void BindLocData()
		{
			if (AreaType == null)
				AreaType = new ObservableCollection<NameValuePair<string>>();
			if (SelectedLoc == null)
				SelectedLoc = new List<string>();
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1912s.Where(o => o.WAREHOUSE_ID == WarehouseId);
            var queryFloor = data.OrderBy(o => o.FLOOR).ToList().Select(o => o.FLOOR).Distinct();
            QueryFloorList = (from o in queryFloor
                              select new NameValuePair<string>
                              {
                                  Name = o,
                                  Value = o
                              }).ToObservableCollection();
            SelectedQueryFloor = QueryFloorList.FirstOrDefault().Value;
			var area = data.OrderBy(o => o.AREA_CODE).ToList().Select(o => o.AREA_CODE).Distinct().ToList();
			var f1919Data = proxy.F1919s.ToList();
			f1919Data = f1919Data.Where(o => area.Contains(o.AREA_CODE) && o.DC_CODE == SelectedDcCode && o.WAREHOUSE_ID == WarehouseId).ToList();
			foreach (var a in f1919Data)
			{
				if (!a.AREA_CODE.Contains("-1"))
					AreaType.Add(new NameValuePair<string> { Name = a.AREA_NAME, Value = a.AREA_CODE });
			}

			F1912Data = proxy.F1912s.Where(x => x.DC_CODE == SelectedDcCode && x.WAREHOUSE_ID == WarehouseId).OrderBy(o => o.LOC_CODE).ToList();
			//F1912Data = detailLocData;
			var masterLocData = from a in F1912Data
								group a by new { a.CHANNEL, a.PLAIN, a.LOC_LEVEL, a.FLOOR } into b
								select new { Channel = b.Key.CHANNEL, Plain = b.Key.PLAIN, LocLevel = b.Key.LOC_LEVEL, Floor = b.Key.FLOOR, LocCount = b.Count() };
			var proxywcf = new wcf.P71WcfServiceClient();
			OldDetailDataList = RunWcfMethod(proxywcf.InnerChannel, () => proxywcf.GetLocDetailData(SelectedDcCode, F1912Data.Select(x => x.LOC_CODE).ToArray()).ToObservableCollection()).ToList();
			OldEditDetailDataList = OldDetailDataList.ToObservableCollection();
			OldMasterDataList = (from a in masterLocData
								 select new P710102MasterData
								 {
									 Floor = a.Floor,
									 ChannelNo = a.Channel,
									 LocLevelNo = a.LocLevel,
									 PlainNo = a.Plain,
									 TotelLocCount = a.LocCount,
									 SettingStatus = "0"
								 }).ToObservableCollection();
			RefreshDataGrid();
			SelectMasertData = MasterDataList.FirstOrDefault();
			GetDetailData(SelectedQueryFloor, SelectedAreaType);
		}

		private void RefreshDataGrid()
		{
			if (OldDetailDataList == null || !OldDetailDataList.Any()) return;
			isRefresh = true;
			var temp = SelectMasertData;
			MasterDataList = OldMasterDataList.Where(o => o.Floor == SelectedQueryFloor).ToObservableCollection();
			SelectMasertData = temp;
			isRefresh = false;
		}

		private void CountMasterData(P710102MasterData data)
		{
            var OldDetailDataListWhere = OldDetailDataList.Where(x => x.CHANNEL == data.ChannelNo && x.PLAIN == data.PlainNo && x.LOC_LEVEL == data.LocLevelNo && x.LOC_CODE.Substring(0, 1) == SelectedQueryFloor).Select(x => x.LOC_CODE).ToList();

            var selectCount = SelectedLoc.Where(o => OldDetailDataListWhere.Contains(o)).Count();

            if (IsAdd)
			{
				data.NowLocCount = selectCount;
				data.ChangeCount = selectCount;
			}
			else
			{
				var oldcount = EditSelectedLoc.Where(o => OldDetailDataListWhere.Contains(o)).Count();
				data.NowLocCount = selectCount;
				data.OldLocCount = oldcount;
				data.ChangeCount = data.NowLocCount - data.OldLocCount;
			}
            ChangeMasterStatus(data.ChannelNo, data.PlainNo, data.LocLevelNo, data);
        }

        public void CountMasterLoc(P710102MasterData old, bool isFirstEnter = false,bool isAdjust = false)
		{
			if (!isFirstEnter)
			{

				var data = OldMasterDataList.FirstOrDefault(o => o.ChannelNo == old.ChannelNo && o.PlainNo == old.PlainNo && o.LocLevelNo == old.LocLevelNo);

				CountMasterData(data);
			}
			else if (!isAdjust)
			{
                //if (SelectedLoc == null || !SelectedLoc.Any()) return;
                //var distinct = SelectedLoc.Where(o => o.Substring(0, 1) == SelectedQueryFloor).Select(o => o.Substring(0, 7)).Distinct().ToList();
                //foreach (var d in distinct)
                //{
                //    var channel = d.Substring(1, 2);
                //    var plain = d.Substring(3, 2);
                //    var loclevel = d.Substring(5, 2);
                //    var data = OldMasterDataList.FirstOrDefault(o => o.ChannelNo == channel && o.PlainNo == plain && o.LocLevelNo == loclevel);
                //    CountMasterData(data);
                //}
            }
			else
			{
				if (SelectedLoc == null || !SelectedLoc.Any())
				{
					foreach (var data in OldMasterDataList)
					{
						if (IsAdd)
						{
							data.NowLocCount = 0;
							data.ChangeCount = 0;
						}
						else
						{
                            var tmplist = OldDetailDataList.Where(x => x.CHANNEL == data.ChannelNo && x.PLAIN == data.PlainNo && x.LOC_LEVEL == data.LocLevelNo && x.LOC_CODE.Substring(0, 1) == SelectedQueryFloor).Select(x => x.LOC_CODE).ToList();
                            var oldcount = EditSelectedLoc.Count(o => tmplist.Contains(o));
                            //var oldcount = EditSelectedLoc.Where(o => OldDetailDataList.Where(x => x.CHANNEL == data.ChannelNo && x.PLAIN == data.PlainNo && x.LOC_LEVEL == data.LocLevelNo && x.LOC_CODE.Substring(0, 1) == SelectedQueryFloor).Select(x => x.LOC_CODE).Contains(o)).Count();
							data.NowLocCount = 0;
							data.OldLocCount = oldcount;
							data.ChangeCount = data.NowLocCount - data.OldLocCount;
						}
						ChangeMasterStatus(data.ChannelNo, data.PlainNo, data.LocLevelNo, data);
					}
				}
				else
				{
                    foreach (var data in OldMasterDataList)
					{
                        CountMasterData(data);
					}
                }
            }
			RefreshDataGrid();
		}


		/// <summary>
		/// 取得明細資料
		/// </summary>
		/// <param name="floor"></param>
		/// <param name="areaType"></param>
		public void GetDetailData(string floor = null, string areaType = null)
		{
			if (OldDetailDataList == null || SelectedQueryFloor == null || SelectMasertData == null) return;
			DetailDataList = new SelectionList<wcf.P710101DetailData>(new ObservableCollection<wcf.P710101DetailData>());
			var conidtion = SelectMasertData;
			var newLocCodes = OldDetailDataList.Where(o => o.CHANNEL == conidtion.ChannelNo && o.PLAIN == conidtion.PlainNo && o.LOC_LEVEL == conidtion.LocLevelNo).ToList();

			foreach (var locData in newLocCodes)
			{
				var gridData = new wcf.P710101DetailData();

				gridData = ExDataMapper.Map<wcf.P710101DetailData, wcf.P710101DetailData>(locData);
				gridData.IsEditData = gridData.AREA_CODE.Trim() == "-1";
				gridData.IsEditData = gridData.STATUS != "1";
				var itemData = new SelectionItem<wcf.P710101DetailData>(gridData, SelectedLoc.Contains(locData.LOC_CODE));
				DetailDataList.Add(itemData);


				if (!string.IsNullOrWhiteSpace(floor))
					DetailDataList = new SelectionList<wcf.P710101DetailData>(DetailDataList.Where(x => x.Item.LOC_CODE.Substring(0, 1) == floor));
				if (!string.IsNullOrWhiteSpace(areaType) && areaType != "0")
					DetailDataList = new SelectionList<wcf.P710101DetailData>(DetailDataList.Where(x => x.Item.AREA_CODE.Contains(areaType)));

				TempLocCodeList = DetailDataList.Where(x => x.IsSelected).Select(x => x.Item.LOC_CODE).ToList();
			}
		}


		/// <summary>
		/// 改變MasterData狀態
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="plain"></param>
		/// <param name="locLevel"></param>
		/// <param name="existM"></param>
		private void ChangeMasterStatus(string channel, string plain, string locLevel, P710102MasterData existM)
		{
			if (OldEditDetailDataList != null && OldEditDetailDataList.Any())
			{
				var old = OldEditDetailDataList.Where(o => o.CHANNEL == channel && o.PLAIN == plain && o.LOC_LEVEL == locLevel && o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && !EditSelectedLoc.Contains(o.LOC_CODE));
				var now = OldDetailDataList.Where(o => o.CHANNEL == channel && o.PLAIN == plain && o.LOC_LEVEL == locLevel && o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && !SelectedLoc.Contains(o.LOC_CODE));

				if (existM.OldLocCount == 0 && existM.NowLocCount == existM.ChangeCount)
				{
					existM.SettingStatus = "1";
				}
				else
				{
					if (existM.OldLocCount != 0 && existM.NowLocCount == 0)
					{
						existM.SettingStatus = "3";
					}
					else if (old.All(x => now.Any(y => y.LOC_CODE == x.LOC_CODE)) && old.Count() == now.Count())
					{
						existM.SettingStatus = "0";
					}
					else
					{
						existM.SettingStatus = "2";
					}
				}
			}
		}

		public ICommand AdjustCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => { },
					  () => OldMasterDataList != null && OldMasterDataList.Any(), o =>
					  {
						  AdjustFrom();
					  });
			}
		}

		/// <summary>
		/// 確認資料異動
		/// </summary>
		public void ConfirmData()
		{
			_isConfirm = true;
			if (!DetailDataList.Any())
				return;

			foreach (var data in DetailDataList)
			{
				if (data.IsSelected)
				{
					if (!SelectedLoc.Contains(data.Item.LOC_CODE))
						SelectedLoc.Add(data.Item.LOC_CODE);
				}
				else
				{
					if (SelectedLoc.Contains(data.Item.LOC_CODE))
						SelectedLoc.Remove(data.Item.LOC_CODE);
				}
			}
			GetLocCount();
			var param = DetailDataList.FirstOrDefault().Item;
			var oldSelectMaster = OldMasterDataList.Where(o => o.ChannelNo == param.CHANNEL && o.PlainNo == param.PLAIN && o.LocLevelNo == param.LOC_LEVEL).FirstOrDefault();
			CountMasterLoc(oldSelectMaster);
			TempLocCodeList = DetailDataList.Where(x => x.IsSelected).Select(x => x.Item.LOC_CODE).ToList();
		}
		/// <summary>
		/// 儲位計算
		/// </summary>
		private void GetLocCount()
		{
			if (IsAdd)
				LocCount = SelectedLoc.Count();
			else
			{
				LocCount = SelectedLoc.Count() - OldLocTotalCount;
				LocTotalCount = OldLocTotalCount;
			}
		}

		public void BindEditData(F1919Data editData)
		{
            if (!string.IsNullOrEmpty(editData.SINGLE_BOX) || !string.IsNullOrEmpty(editData.PICK_CHECK) ||
                !string.IsNullOrEmpty(editData.PICK_TYPE) || !string.IsNullOrEmpty(editData.PICK_TOOL) ||
                !string.IsNullOrEmpty(editData.PICK_SEQ) || !string.IsNullOrEmpty(editData.SORT_BY) ||
                 !string.IsNullOrEmpty(editData.PUT_TOOL) || !string.IsNullOrEmpty(editData.PICK_UNIT) ||
                 !string.IsNullOrEmpty(editData.PICK_MARTERIAL) || !string.IsNullOrEmpty(editData.DELIVERY_MARTERIAL))
                CreateStorageAreaPickCheck = "1";
            
          
            SelectedDcCode = editData.DC_CODE;
			SelectedGupCode = editData.GUP_CODE;
			SelectedCustCode = editData.CUST_CODE;
			SelectedDcName = editData.DC_NAME;
			SelectedGupName = editData.GUP_NAME;
			SelectedCustName = editData.CUST_NAME;
			AreaName = editData.AREA_NAME;
			AreaCode = editData.AREA_CODE;
			SelectedAType = editData.ATYPE_CODE;
            SelectedPickType = editData.PICK_TYPE;
            SelectedPickTool = editData.PICK_TOOL;
            SelectedPickSEQ = editData.PICK_SEQ;
            SelectedSortBy = editData.SORT_BY;
            SelectedPutTool = editData.PUT_TOOL;
            SelectedPickUnit = editData.PICK_UNIT;
            SelectedPickMarterial = editData.PICK_MARTERIAL;
            SelectedDeliveryMarterial = editData.DELIVERY_MARTERIAL;
			SelectedMoveTool = editData.MOVE_TOOL;
			CheckSingleBox = editData.SINGLE_BOX;
            CheckPickCheck = editData.PICK_CHECK;
            var proxy = GetProxy<F19Entities>();
			var oldData = proxy.F1912s.Where(o => o.WAREHOUSE_ID == WarehouseId && o.AREA_CODE == AreaCode && o.DC_CODE == SelectedDcCode).ToList();
			SelectedLoc = oldData.Select(o => o.LOC_CODE).ToList();
			EditSelectedLoc = oldData.Select(o => o.LOC_CODE).ToList();
			GetDetailData(SelectedQueryFloor, SelectedAreaType);
			OldLocTotalCount = SelectedLoc.Count();
			CountMasterLoc(SelectMasertData, true);
			GetLocCount();
		}
	}
}
