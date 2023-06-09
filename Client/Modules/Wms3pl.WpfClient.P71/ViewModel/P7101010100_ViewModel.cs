using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.P71.Entities;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;

namespace Wms3pl.WpfClient.P71.ViewModel
{
    public partial class P7101010100_ViewModel : InputViewModelBase
    {
        public Action AdjustFrom = delegate { };
        public P7101010100_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                SetDcList();
                SetTempTypeList();
                SetDeviceTypeList();
                SetPickFloor();
                SetHandyList();
                SetWarehourseLocTypeList();
                GetLocSetDatas();
                SetListData();

                //WarehouseId = Properties.Resources.P7101010100_ViewModel_SysAutoNo;
                HorDistance = 10;
                SettingType = SettingMode.LocArea;
                _userId = Wms3plSession.CurrentUserInfo.Account;
            }

        }

        #region Property

        private string _userId;
        private bool IsSuccess;
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
                GetLocSetDatas();
                SetListData();
                ClaerLocList();
                GetLocCount();
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

        private ObservableCollection<P710101MasterData> _oldMasterDataList;
        public ObservableCollection<P710101MasterData> OldMasterDataList
        {
            get { return _oldMasterDataList; }
            set { Set(ref _oldMasterDataList, value); }
        }

        private ObservableCollection<P710101MasterData> _masterDataList;
        public ObservableCollection<P710101MasterData> MasterDataList
        {
            get { return _masterDataList; }
            set { Set(ref _masterDataList, value); }
        }

        private ObservableCollection<wcf.P710101DetailData> _oldEditDetailDataList;
        public ObservableCollection<wcf.P710101DetailData> OldEditDetailDataList
        {
            get { return _oldEditDetailDataList; }
            set { Set(ref _oldEditDetailDataList, value); }
        }

        private P710101MasterData _selectMasertData;
        public P710101MasterData SelectMasertData
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
                                if (ExcludeLoc.Contains(loc))
                                    ExcludeLoc.Remove(loc);
                            }
                            var oldLoc = DetailDataList.Where(o => !tmpData.Contains(o.Item.LOC_CODE)).Select(o => o.Item.LOC_CODE).ToList();
                            foreach (var loc in oldLoc)
                            {
                                if (!ExcludeLoc.Contains(loc))
                                    ExcludeLoc.Add(loc);
                            }

                        }
                    }
                }
                if (value != null && !isRefresh)
                    GetDetailData(SelectedQueryFloor, SelectedAreaType);
            }
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

        private List<string> _tempLocCodeList;
        public List<string> TempLocCodeList
        {
            get { return _tempLocCodeList; }
            set { Set(ref _tempLocCodeList, value); }
        }

        private bool isRefresh = false;

        private SelectionItem<wcf.P710101DetailData> _selectedDetailData;
        public SelectionItem<wcf.P710101DetailData> SelectedDetailData
        {
            get { return _selectedDetailData; }
            set
            {
                Set(ref _selectedDetailData, value);
                if (_selectedDetailData == null) return;
                _selectedDetailData.IsSelectedPropertyChange = () =>
                {
                    //var nowCount = DetailDataList.Where(o => o.IsSelected).Count();
                    //SelectMasertData.ChangeCount = nowCount - SelectMasertData.OldLocCount;
                    //SelectMasertData.NowLocCount = nowCount;
                    //RefreshDataGrid();
                };
            }
        }

        private bool _isEdit;
        public bool IsEdit
        {
            get { return _isEdit; }
            set { Set(ref _isEdit, value); }
        }

        private bool _isNotEdit;
        public bool IsNotEdit
        {
            get { return _isNotEdit; }
            set { Set(ref _isNotEdit, value); }
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
        #endregion

        #region 倉別屬性


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

        private List<NameValuePair<string>> _gridWarehouseList;
        public List<NameValuePair<string>> GridWarehouseList
        {
            get { return _gridWarehouseList; }
            set
            {
                _gridWarehouseList = value;
                RaisePropertyChanged("GridWarehouseList");
            }
        }

        private List<NameValuePair<string>> _locStatusList;
        public List<NameValuePair<string>> LocStatusList
        {
            get { return _locStatusList; }
            set { Set(ref _locStatusList, value); }
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

        private Visibility _lbWarehouseTypeVisibility=Visibility.Visible;
        public Visibility lbWarehouseTypeVisibility
        {
            get { return _lbWarehouseTypeVisibility; }
            set
            {
                _lbWarehouseTypeVisibility = value;
                RaisePropertyChanged("lbWarehouseTypeVisibility");
            }
        }

        private Visibility _txtWarehouseIDVisibility = Visibility.Collapsed;
        public Visibility txtWarehouseIDVisibility
        {
            get { return _txtWarehouseIDVisibility; }
            set
            {
                _txtWarehouseIDVisibility = value;
                RaisePropertyChanged("txtWarehouseIDVisibility");
            }
        }

        //WarehouseId數字部分
        private string _warehouseNum = "";
        public string WarehouseNum
        {
            get { return _warehouseNum; }
            set
            {
                _warehouseNum = value;
                RaisePropertyChanged("WarehouseNum");
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

        // 設定倉庫類別
        private List<NameValuePair<string>> _deviceTypeList;
        public List<NameValuePair<string>> DeviceTypeList
        {
            get { return _deviceTypeList; }
            set
            {
                _deviceTypeList = value;
                RaisePropertyChanged("DeviceTypeList");
            }
        }

        private string _selectedDeviceType = "";
        public string SelectedDeviceType
        {
            get { return _selectedDeviceType; }
            set
            {
                _selectedDeviceType = value;
                RaisePropertyChanged("SelectedDeviceType");
            }
        }

        // 設定揀貨樓層
        private List<NameValuePair<string>> _pickFloorList;
        public List<NameValuePair<string>> PickFloorList
        {
            get { return _pickFloorList; }
            set
            {
                Set(() => PickFloorList, ref _pickFloorList, value);
            }
        }

        private string _selectedPickFloor;
        public string SelectedPickFloor
        {
            get { return _selectedPickFloor; }
            set
            {
                Set(() => SelectedPickFloor, ref _selectedPickFloor, value);
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

        private bool _isJobSelectedAll;
        public bool IsJobSelectedAll
        {
            get { return _isJobSelectedAll; }
            set
            {
                Set(ref _isJobSelectedAll, value);
                foreach (var locCode in DetailDataList)
                {
                    if (!locCode.Item.IsEditData) continue;

                    locCode.IsSelected = (locCode.Item.IsEditData && _isJobSelectedAll);
                    if (_isJobSelectedAll)
                    {
                        if (ExcludeLoc.Contains(locCode.Item.LOC_CODE))
                            ExcludeLoc.Remove(locCode.Item.LOC_CODE);
                    }
                    else
                    {
                        if (!ExcludeLoc.Contains(locCode.Item.LOC_CODE))
                            ExcludeLoc.Add(locCode.Item.LOC_CODE);
                    }
                }

                //if (!IsEdit)
                //{
                //	SelectMasertData.NowLocCount = _isJobSelectedAll ? DetailDataList.Where(o => !ExcludeLoc.Contains(o.Item.LOC_CODE)).Count() : 0;
                //	SelectMasertData.ChangeCount = SelectMasertData.NowLocCount;
                //}
                //else
                //{
                //	SelectMasertData.NowLocCount = DetailDataList.Where(o => o.IsSelected).Count();
                //	SelectMasertData.ChangeCount = _isJobSelectedAll ? DetailDataList.Count() - SelectMasertData.OldLocCount : DetailDataList.Where(o => !ExcludeLoc.Contains(o.Item.LOC_CODE)).Count() - SelectMasertData.OldLocCount;
                //}
                //RefreshDataGrid();
            }
        }

        private List<string> _excludeLoc;
        public List<string> ExcludeLoc
        {
            get { return _excludeLoc; }
            set { Set(ref _excludeLoc, value); }
        }

        private ObservableCollection<wcf.P710101DetailData> _nowLocData;
        public ObservableCollection<wcf.P710101DetailData> NowLocData
        {
            get { return _nowLocData; }
            set { Set(ref _nowLocData, value); }
        }


        private List<string> _eidtexcludeLoc;
        public List<string> EditExcludeLoc
        {
            get { return _eidtexcludeLoc; }
            set { Set(ref _eidtexcludeLoc, value); }
        }

        private SettingMode _settingType;
        public SettingMode SettingType
        {
            get { return _settingType; }
            set { Set(ref _settingType, value); }
        }

        private string _locText;
        public string LocText
        {
            get { return _locText; }
            set { Set(ref _locText, value); }
        }

        #endregion

        #region 儲位屬性

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
                SelectedHandy = f1942Item.HANDY;
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

        private decimal _horDistance;

        public decimal HorDistance
        {
            get { return _horDistance; }
            set
            {
                _horDistance = value;
                RaisePropertyChanged("HorDistance");
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

        private ObservableCollection<NameValuePair<string>> _queryFloorList;
        public ObservableCollection<NameValuePair<string>> QueryFloorList
        {
            get { return _queryFloorList; }
            set
            {
                _queryFloorList = value;
                RaisePropertyChanged("QueryFloorList");
            }
        }

        private ObservableCollection<NameValuePair<string>> _areaType;
        public ObservableCollection<NameValuePair<string>> AreaType
        {
            get { return _areaType; }
            set { Set(ref _areaType, value); }
        }

        private void CountMasterEditData(P710101MasterData data)
        {
            //已被使用儲位或原本不存在的空儲位
            //編輯時排除非本倉儲位本倉還須被計入
            var findOldDetailDataList = OldDetailDataList.Where(x => x.CHANNEL == data.ChannelNo && x.PLAIN == data.PlainNo && x.LOC_LEVEL == data.LocLevelNo && x.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && x.WAREHOUSE_ID != WarehouseId).Select(x => x.LOC_CODE).ToList();
            int oldcount = NowLocData.Where(o => findOldDetailDataList.Contains(o.LOC_CODE)).Count();
            //原本新增儲位不存在於F191201的設定當設定加入時會出現在明細中計算要排除
            oldcount += EditExcludeLoc == null ? 0 : OldDetailDataList.Where(o => EditExcludeLoc.Contains(o.LOC_CODE) && o.CHANNEL == data.ChannelNo && o.PLAIN == data.PlainNo && o.LOC_LEVEL == data.LocLevelNo && o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor).Count();

            //目前明細的儲位
            var loc = OldDetailDataList.Where(o => o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && o.CHANNEL == data.ChannelNo && o.PLAIN == data.PlainNo && o.LOC_LEVEL == data.LocLevelNo).ToList();
            var oldCount = OldEditDetailDataList.Where(o => o.CHANNEL == data.ChannelNo && o.PLAIN == data.PlainNo && o.LOC_LEVEL == data.LocLevelNo && o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor);

            //原始儲位 需減去已排除儲位
            if (oldCount != null && oldCount.Any())
                data.OldLocCount = oldCount.Count() - oldcount;
            else
                data.OldLocCount = oldCount.Count();
            //現在儲位數 = 目前明細的儲位 - 被排除的儲位
            data.NowLocCount = loc.Count() - loc.Where(o => ExcludeLoc.Contains(o.LOC_CODE)).Count();
            data.ChangeCount = data.NowLocCount - data.OldLocCount;
            ChangeMasterStatus(data.ChannelNo, data.PlainNo, data.LocLevelNo, data);
        }

        private void CountMasterAddData(List<wcf.P710101DetailData> newData, List<wcf.P710101DetailData> oldData, P710101MasterData data)
        {
            int oldcount = oldData.Where(x => x.CHANNEL == data.ChannelNo && x.PLAIN == data.PlainNo && x.LOC_LEVEL == data.LocLevelNo && x.LOC_CODE.Substring(0, 1) == SelectedQueryFloor).Count();
            //目前明細的儲位
            var loc = newData.Where(o => o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && o.CHANNEL == data.ChannelNo && o.PLAIN == data.PlainNo && o.LOC_LEVEL == data.LocLevelNo).ToList();

            //現在儲位數 = 目前明細的儲位 - 被排除的儲位
            data.NowLocCount = loc.Count() - loc.Where(o => ExcludeLoc.Contains(o.LOC_CODE)).Count();
            data.ChangeCount = data.NowLocCount - data.OldLocCount;
            ChangeMasterStatus(data.ChannelNo, data.PlainNo, data.LocLevelNo, data);
        }

        public void CountMasterLoc(P710101MasterData old, bool isFirstEnter = false)
        {
            if (OldMasterDataList != null)
            {
                if (!IsEdit)
                {
                    var oldData = NowLocData.Except(OldDetailDataList).ToList();
                    var newData = OldDetailDataList.Except(oldData).ToList();
                    if (isFirstEnter)
                    {
                        var distinct = ExcludeLoc.Where(o => o.Substring(0, 1) == SelectedQueryFloor).Select(o => o.Substring(0, 7)).Distinct().ToList();

                        foreach (var d in distinct)
                        {
                            var channel = d.Substring(1, 2);
                            var plain = d.Substring(3, 2);
                            var loclevel = d.Substring(5, 2);
                            var data = OldMasterDataList.FirstOrDefault(o => o.ChannelNo == channel && o.PlainNo == plain && o.LocLevelNo == loclevel);
                            CountMasterAddData(newData, oldData, data);
                        }
                    }
                    else
                    {
                        CountMasterAddData(newData, oldData, old);
                    }
                }
                else
                {
                    if (OldEditDetailDataList == null || !OldEditDetailDataList.Any()) return;

                    if (!isFirstEnter)
                        CountMasterEditData(old);
                }

                RefreshDataGrid();
            }

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
                CountMasterLoc(null, true);
            }
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

        private int _locCount;

        public int LocCount
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


        private List<NameValuePair<string>> _settingStatusList;
        public List<NameValuePair<string>> SettingStatusList
        {
            get { return _settingStatusList; }
            set { Set(ref _settingStatusList, value); }
        }
        #endregion

        #endregion

        #region 下拉式選單資料來源

        #region 物流中心 業主 貨主
        /// <summary>
        /// 設定儲位下拉資料選項
        /// </summary>
        private void SetListData()
        {
            SetWarehouseTypeList();
            SetFloorList();
            SetMinChanelList();
            SetMinPlainList();
            SetMinLocLevelList();
            SetMinLocTypeList();
            SetSettingStatusList();
            SetAreaList();
            LocStatusList = GetBaseTableService.GetF000904List(FunctionCode, "P7101010000", "LocStatus");
        }
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
            {
                SelectedWarehouseType = WarehouseTypeList.First().Value;

            }
            var dataw = proxy.F1980s.Where(x => x.DC_CODE == SelectedDcCode).OrderBy(x => x.WAREHOUSE_ID);
            GridWarehouseList = (from o in dataw
                                 select new NameValuePair<string>
                                 {
                                     Name = o.WAREHOUSE_ID + " - " + o.WAREHOUSE_NAME,
                                     Value = o.WAREHOUSE_ID
                                 }).ToList();
        }
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

        public void SetDeviceTypeList()
        {
            DeviceTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1980", "DEVICE_TYPE"); ;
            if (DeviceTypeList.Any())
                SelectedDeviceType = DeviceTypeList.First().Value;
        }

        public void SetPickFloor()
        {
            PickFloorList = GetProxy<F19Entities>().F191201s.Where(x => x.TYPE == "6").Select(x => new NameValuePair<string>
            {
                Name = x.VALUE,
                Value = x.VALUE
            }).ToList();
            if (PickFloorList.Any())
            {
                SelectedPickFloor = PickFloorList.First().Value;
            }
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
            WarehouseLocTypeList = locTypeList;
            if (WarehouseLocTypeList.Any())
                SelectedWarehouseLocType = WarehouseLocTypeList.First().Value;
        }

        /// <summary>
        /// 設定便利性清單
        /// </summary>
        public void SetHandyList()
        {
            var list = new List<NameValuePair<string>>();
            list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_L, Value = "1" });
            list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_M, Value = "2" });
            list.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_H, Value = "3" });
            HandyeList = list;
            if (HandyeList.Any())
                SelectedHandy = HandyeList.First().Value;
        }
        #endregion

        #region 儲位範圍
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
                SelectedMaxChannel = MaxChannelList.FirstOrDefault(o => o.Value == startIndex)?.Value ?? MaxChannelList.LastOrDefault().Value;
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
                SelectedMaxPlain = MaxPlainList.FirstOrDefault(o => o.Value == startIndex)?.Value ?? MaxPlainList.LastOrDefault().Value;
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
                SelectedMaxLocLevel = MaxLocLevelList.FirstOrDefault(o => o.Value == startIndex)?.Value ?? MaxLocLevelList.LastOrDefault().Value;
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
                SelectedMaxLocType = MaxLocTypeList.FirstOrDefault(o => o.Value == startIndex)?.Value ?? MaxLocTypeList.LastOrDefault().Value;
        }
        #endregion

        private void SetSettingStatusList()
        {
            SettingStatusList = GetBaseTableService.GetF000904List(FunctionCode, "P7101010000", "SettingStatus");
        }

        #endregion

        #region 計算儲位數



        #endregion

        #region ChangeUseMode

        private void ChangeUseModeTypeDisplay()
        {
            VisibilityGupAndCust = (_displayUseModelType == UseModelType.Headquarters) ? Visibility.Visible : Visibility.Hidden;
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
            int intwarehouseNum;
            decimal num;
            //執行確認儲存動作
            string msg = string.Empty;

            if (RentBeginDate.HasValue && !RentEndDate.HasValue)
                msg = Properties.Resources.P7101010100_ViewModel_RentBeginDateRentEndDate_Required;
            else if (RentEndDate.HasValue && !RentBeginDate.HasValue)
                msg = Properties.Resources.P7101010100_ViewModel_RentEndDateRentBeginDate_Required;
            else if (RentBeginDate.HasValue && RentEndDate.HasValue && RentBeginDate > RentEndDate)
                msg = Properties.Resources.P7101010100_ViewModel_RentBeginDate_GreaterThan_RentEndDate;
            else if (!decimal.TryParse(_horDistance.ToString(), out num))
                msg = Properties.Resources.P7101010100_ViewModel_HorDistance_Num_Required;
            else if (string.IsNullOrEmpty(_warehouseName))
                msg = Properties.Resources.P7101010100_ViewModel_WarehouseName_Required;
            else if (WarehouseNum.Length != 2)
                msg = Properties.Resources.P7101010100_ViewModel_WarehouseNum_Length_short;
            else if (!int.TryParse(WarehouseNum, out intwarehouseNum))
                msg = Properties.Resources.P7101010100_ViewModel_WarehouseNum_Num_Required;

            WarehouseId = SelectedWarehouseType + WarehouseNum;

            if (msg.Length == 0 && !IsEdit)
            {
                var proxy = GetProxy<F19Entities>();
                if (proxy.F1980s.Where(x => x.WAREHOUSE_ID == WarehouseId && x.DC_CODE == SelectedDcCode).ToList().Any())
                {
                    msg = Properties.Resources.P7101010100_ViewModel_WarehouseId_Exist;
                }
            }


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
                if (TempLocCodeList != null && TempLocCodeList.Any())
                {
                    var tmpData = TempLocCodeList;
                    var nowData = DetailDataList.Where(o => o.IsSelected);
                    var isModify = (!tmpData.All(x => nowData.Any(y => y.Item.LOC_CODE == x)) || tmpData.Count() != nowData.Count());

                    if (isModify && !isRefresh)
                    {
                        if (ShowConfirmMessage(Properties.Resources.P7101010100_ViewModel_DataEditCheck) == DialogResponse.Yes)
                            ConfirmData();
                    }
                }
                var proxywcf = new wcf.P71WcfServiceClient();
                var f1980Data = new ExDataServices.P71WcfService.F1980Data
                {
                    DC_CODE = _selectedDcCode,
                    WAREHOUSE_ID = WarehouseId,
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
                    HOR_DISTANCE = _horDistance,
                    RENT_BEGIN_DATE = _rentBeginDate,
                    RENT_END_DATE = _rentEndDate,
                    DEVICE_TYPE = _selectedDeviceType,
                    PICK_FLOOR = _selectedPickFloor,

                    IsModifyDate = true,
                };
                List<wcf.F1912> newData = new List<wcf.F1912>();
                var saveData = OldDetailDataList.Where(o => !ExcludeLoc.Contains(o.LOC_CODE)).ToList();
                foreach (var sData in saveData)
                {
                    var data = new wcf.F1912();
                    data.LOC_CODEk__BackingField = sData.LOC_CODE;
                    data.CHANNELk__BackingField = sData.CHANNEL;
                    data.PLAINk__BackingField = sData.PLAIN;
                    data.LOC_LEVELk__BackingField = sData.LOC_LEVEL;
                    data.LOC_TYPEk__BackingField = sData.LOC_CODE.Substring(7, 2);
                    data.LOC_TYPE_IDk__BackingField = SelectedWarehouseLocType;
                    data.FLOORk__BackingField = sData.LOC_CODE.Substring(0, 1);
                    newData.Add(data);
                }
                var result = new wcf.ExecuteResult();
                if (!IsEdit)
                    result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.InsertF1980Data(f1980Data, _userId, newData.ToArray()));
                else
                    result = RunWcfMethod<wcf.ExecuteResult>(proxywcf.InnerChannel, () => proxywcf.UpdateF1980Data(f1980Data, _userId, newData.ToArray()));
                if (result.IsSuccessed)
                {
                    IsSuccess = true;
                    if (!IsEdit)
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

        /// <summary>
        /// 取得儲位設定資料
        /// </summary>
        private void GetLocSetDatas()
        {
            var proxy = GetProxy<F19Entities>();
            LocSetData = proxy.F191201s.Where(x => x.DC_CODE == SelectedDcCode).OrderBy(o => o.VALUE).ToList();
        }
        /// <summary>
        /// 取得明細資料
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="areaType"></param>
        public void GetDetailData(string floor = null, string areaType = null)
        {
            if (OldDetailDataList == null || SelectedQueryFloor == null) return;
            DetailDataList = new SelectionList<wcf.P710101DetailData>(new ObservableCollection<wcf.P710101DetailData>());
            var conidtion = SelectMasertData;
            var newLocCodes = OldDetailDataList.Where(o => o.CHANNEL == conidtion.ChannelNo && o.PLAIN == conidtion.PlainNo && o.LOC_LEVEL == conidtion.LocLevelNo).ToList();

            if (NowLocData == null) return;
            var nowLocData = NowLocData.Where(x => newLocCodes.Select(o => o.LOC_CODE).Contains(x.LOC_CODE)).ToObservableCollection();// RunWcfMethod<ObservableCollection<wcf.P710101DetailData>>(proxywcf.InnerChannel, () => proxywcf.GetLocDetailData(SelectedDcCode, newLocCodes.Select(o => o.LOC_CODE).ToArray()).ToObservableCollection());
            foreach (var locData in newLocCodes)
            {
                var gridData = new wcf.P710101DetailData();
                var exist = nowLocData.Where(o => o.LOC_CODE == locData.LOC_CODE).FirstOrDefault();

                if (exist != null)
                {
                    if (!IsEdit)
                    {
                        exist.IsEditData = false;
                        gridData = ExDataMapper.Map<wcf.P710101DetailData, wcf.P710101DetailData>(exist);
                        var itemData = new SelectionItem<wcf.P710101DetailData>(gridData, false);
                        DetailDataList.Add(itemData);
                    }
                    else
                    {
                        if (ExcludeLoc.Contains(locData.LOC_CODE))
                        {
                            exist.IsEditData = exist.WAREHOUSE_ID == WarehouseId && exist.IsEdit == 1 ? true : false;
                            gridData = ExDataMapper.Map<wcf.P710101DetailData, wcf.P710101DetailData>(exist);
                            var itemData = new SelectionItem<wcf.P710101DetailData>(gridData, false);
                            DetailDataList.Add(itemData);
                        }
                        else
                        {
                            exist.IsEditData = exist.WAREHOUSE_ID == WarehouseId && exist.IsEdit == 1 ? true : false;
                            gridData = ExDataMapper.Map<wcf.P710101DetailData, wcf.P710101DetailData>(exist);
                            var itemData = new SelectionItem<wcf.P710101DetailData>(gridData, exist.WAREHOUSE_ID == WarehouseId ? true : false);
                            DetailDataList.Add(itemData);
                        }
                    }
                }
                else
                {
                    locData.IsEditData = true;
                    locData.STATUS = "-1";
                    gridData = ExDataMapper.Map<wcf.P710101DetailData, wcf.P710101DetailData>(locData);
                    if (ExcludeLoc.Contains(locData.LOC_CODE))
                    {
                        var itemData = new SelectionItem<wcf.P710101DetailData>(gridData, false);
                        DetailDataList.Add(itemData);
                    }
                    else
                    {
                        var itemData = new SelectionItem<wcf.P710101DetailData>(gridData, true);
                        DetailDataList.Add(itemData);
                    }
                }

                if (!string.IsNullOrWhiteSpace(floor))
                    DetailDataList = new SelectionList<wcf.P710101DetailData>(DetailDataList.Where(x => x.Item.LOC_CODE.Substring(0, 1) == floor));
                if (!string.IsNullOrWhiteSpace(areaType) && areaType != "0")
                    DetailDataList = new SelectionList<wcf.P710101DetailData>(DetailDataList.Where(x => x.Item.AREA_CODE.Contains(areaType)));

                TempLocCodeList = DetailDataList.Where(x => x.IsSelected).Select(x => x.Item.LOC_CODE).ToList();
            }
        }
        /// <summary>
        /// 設定查詢樓層資料
        /// </summary>
        private void SetQueryFloorData()
        {
            if (QueryFloorList == null || !QueryFloorList.Any())
                QueryFloorList = new ObservableCollection<NameValuePair<string>>();
            if (SettingType == SettingMode.LocArea)
            {
                if (!QueryFloorList.Select(x => x.Value).Contains(SelectedFloor))
                {
                    QueryFloorList.Add(new NameValuePair<string> { Name = SelectedFloor, Value = SelectedFloor });
                    QueryFloorList = QueryFloorList.OrderBy(o => o.Value).ToObservableCollection();
                    SelectedQueryFloor = QueryFloorList.FirstOrDefault(o => o.Value == SelectedFloor).Value;
                }
            }
            else
            {
                var floor = LocText.Substring(0, 1);
                if (!QueryFloorList.Select(x => x.Value).Contains(floor))
                {
                    QueryFloorList.Add(new NameValuePair<string> { Name = floor, Value = floor });
                    QueryFloorList = QueryFloorList.OrderBy(o => o.Value).ToObservableCollection();
                    SelectedQueryFloor = QueryFloorList.FirstOrDefault(o => o.Value == floor).Value;
                }
            }
        }
        /// <summary>
        /// 設定儲區資料
        /// </summary>
        private void SetAreaList()
        {
            AreaType = new ObservableCollection<NameValuePair<string>>();
            AreaType.Add(new NameValuePair<string> { Name = Properties.Resources.P7101010100_ViewModel_NoneSelect, Value = "-1" });
            AreaType.Add(new NameValuePair<string> { Name = Resources.Resources.All, Value = "0" });
            SelectedAreaType = AreaType.FirstOrDefault(o => o.Value == "0").Value;
        }
        /// <summary>
        /// 儲位計算
        /// </summary>
        private void GetLocCount()
        {
            if (!IsEdit)
                LocCount = OldDetailDataList.Where(o => !ExcludeLoc.Contains(o.LOC_CODE)).Count();
            else
            {
                LocCount = OldDetailDataList.Where(o => !ExcludeLoc.Contains(o.LOC_CODE)).Count() - OldLocTotalCount;
                LocTotalCount = OldLocTotalCount;
            }
        }
        /// <summary>
        /// 改變MasterData狀態
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="plain"></param>
        /// <param name="locLevel"></param>
        /// <param name="existM"></param>
        private void ChangeMasterStatus(string channel, string plain, string locLevel, P710101MasterData existM)
        {
            if (OldEditDetailDataList != null && OldEditDetailDataList.Any())
            {
                var old = OldEditDetailDataList.Where(o => o.CHANNEL == channel && o.PLAIN == plain && o.LOC_LEVEL == locLevel && o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && !EditExcludeLoc.Contains(o.LOC_CODE));
                var now = OldDetailDataList.Where(o => o.CHANNEL == channel && o.PLAIN == plain && o.LOC_LEVEL == locLevel && o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && !ExcludeLoc.Contains(o.LOC_CODE));

                if (existM.OldLocCount != 0)
                {
                    if (now.Count() == 0)
                        existM.SettingStatus = "3";
                    else if (old.All(x => now.Any(y => y.LOC_CODE == x.LOC_CODE)) && old.Count() == now.Count())
                        existM.SettingStatus = "0";
                    else
                        existM.SettingStatus = "2";
                }
                else
                    existM.SettingStatus = "1";
            }
        }
        private void EditGridLocDataByArea(List<F1912> datas)
        {
            if (OldMasterDataList == null || !OldMasterDataList.Any())
                OldMasterDataList = new ObservableCollection<P710101MasterData>();
            if (OldDetailDataList == null || !OldDetailDataList.Any())
                OldDetailDataList = new List<wcf.P710101DetailData>();

            foreach (var data in datas.GroupBy(o => o.FLOOR))
            {
                var channels = datas.Where(o => o.FLOOR == data.Key).Select(o => o.CHANNEL).ToList().Distinct().ToList();
                var plainsG = datas.GroupBy(o => new { o.PLAIN, o.CHANNEL }).ToList();
                var loclevesG = datas.GroupBy(o => new { o.PLAIN, o.CHANNEL, o.LOC_LEVEL }).ToList();

                foreach (var c in channels)
                {
                    var plains = plainsG.Where(o => o.Key.CHANNEL == c).Select(o => o.Key.PLAIN).ToList();
                    foreach (var p in plains)
                    {
                        var locleves = loclevesG.Where(o => o.Key.CHANNEL == c && o.Key.PLAIN == p).Select(o => o.Key.LOC_LEVEL).ToList();
                        foreach (var l in locleves)
                        {
                            var locTypes = datas.Where(x => x.FLOOR == data.Key && x.CHANNEL == c && x.PLAIN == p && x.LOC_LEVEL == l);
                            var existM = OldMasterDataList.Where(x => x.FloorNo == data.Key && x.ChannelNo == c && x.PlainNo == p && x.LocLevelNo == l).FirstOrDefault();
                            if (existM == null)
                            {
                                var masterData = new P710101MasterData();
                                masterData.FloorNo = data.Key;
                                masterData.ChannelNo = c;
                                masterData.PlainNo = p;
                                masterData.LocLevelNo = l;
                                masterData.ChangeCount = locTypes.Count();
                                masterData.OldLocCount = 0;
                                masterData.NowLocCount = locTypes.Count();
                                masterData.SettingStatus = "1";
                                OldMasterDataList.Add(masterData);
                                var notExist = locTypes.Where(x => !OldDetailDataList.Select(o => o.LOC_CODE).Contains(x.LOC_CODE));
                                foreach (var e in notExist)
                                {
                                    var detailData = new wcf.P710101DetailData();
                                    detailData.CHANNEL = c;
                                    detailData.LOC_CODE = e.LOC_CODE;
                                    detailData.PLAIN = p;
                                    detailData.LOC_LEVEL = l;
                                    detailData.AREA_CODE = "-1";
                                    OldDetailDataList.Add(detailData);
                                }
                            }
                            else
                            {
                                var notExist = locTypes.Where(x => !OldDetailDataList.Select(o => o.LOC_CODE).Contains(x.LOC_CODE));
                                foreach (var e in notExist)
                                {
                                    var detailData = new wcf.P710101DetailData();
                                    detailData.CHANNEL = e.CHANNEL;
                                    detailData.LOC_CODE = e.LOC_CODE;
                                    detailData.PLAIN = e.PLAIN;
                                    detailData.LOC_LEVEL = e.LOC_LEVEL;
                                    detailData.AREA_CODE = "-1";
                                    OldDetailDataList.Add(detailData);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 儲位區間新增
        /// </summary>
        /// <param name="isEditInit"></param>
        public void AddGridLocDataByArea(bool isEditInit = false, string channelData = "", List<string> plainData = null, List<string> locLeveData = null, List<string> locTypeData = null)
        {
            var channels = new List<NameValuePair<string>>();
            var plains = new List<NameValuePair<string>>();
            var locLeves = new List<NameValuePair<string>>();
            var locTypes = new List<NameValuePair<string>>();

            if (isEditInit)
            {
                channels.Add(new NameValuePair<string> { Name = channelData, Value = channelData });
                plains = (from p in plainData
                          select new NameValuePair<string> { Name = p, Value = p }).ToList();
                locLeves = (from l in locLeveData
                            select new NameValuePair<string> { Name = l, Value = l }).ToList();
                locTypes = (from l in locTypeData
                            select new NameValuePair<string> { Name = l, Value = l }).ToList();
            }
            else
            {
                channels = MinChannelList.Where(x => string.Compare(x.Value, SelectedMinChannel) >= 0 && string.Compare(x.Value, SelectedMaxChannel) <= 0).ToList();
                plains = MinPlainList.Where(x => string.Compare(x.Value, SelectedMinPlain) >= 0 && string.Compare(x.Value, SelectedMaxPlain) <= 0).ToList();
                locLeves = MinLocLevelList.Where(x => string.Compare(x.Value, SelectedMinLocLevel) >= 0 && string.Compare(x.Value, SelectedMaxLocLevel) <= 0).ToList();
                locTypes = MinLocTypeList.Where(x => string.Compare(x.Value, SelectedMinLocType) >= 0 && string.Compare(x.Value, SelectedMaxLocType) <= 0).ToList();

            }
            if (OldMasterDataList == null || !OldMasterDataList.Any())
                OldMasterDataList = new ObservableCollection<P710101MasterData>();
            if (OldDetailDataList == null || !OldDetailDataList.Any())
                OldDetailDataList = new List<wcf.P710101DetailData>();

            foreach (var channel in channels)
            {
                foreach (var plain in plains)
                {
                    foreach (var locLeve in locLeves)
                    {
                        var existM = OldMasterDataList.Where(x => x.FloorNo == SelectedFloor && x.ChannelNo == channel.Value && x.PlainNo == plain.Value && x.LocLevelNo == locLeve.Value).FirstOrDefault();
                        if (existM == null)
                        {
                            var masterData = new P710101MasterData();
                            masterData.FloorNo = SelectedFloor;
                            masterData.ChannelNo = channel.Value;
                            masterData.PlainNo = plain.Value;
                            masterData.LocLevelNo = locLeve.Value;
                            masterData.ChangeCount = locTypes.Count();
                            masterData.OldLocCount = 0;
                            masterData.NowLocCount = locTypes.Count();
                            masterData.SettingStatus = "1";
                            OldMasterDataList.Add(masterData);
                            foreach (var locType in locTypes)
                            {
                                string locCode = SelectedFloor + channel.Value + plain.Value + locLeve.Value + locType.Value;
                                var existD = OldDetailDataList.Where(x => x.LOC_CODE == locCode);
                                if (!existD.Any())
                                {
                                    var detailData = new wcf.P710101DetailData();
                                    detailData.CHANNEL = channel.Value;
                                    detailData.LOC_CODE = locCode;
                                    detailData.PLAIN = plain.Value;
                                    detailData.LOC_LEVEL = locLeve.Value;
                                    detailData.AREA_CODE = "-1";
                                    OldDetailDataList.Add(detailData);
                                }
                            }
                        }
                        else
                        {
                            foreach (var locType in locTypes)
                            {
                                string locCode = SelectedFloor + channel.Value + plain.Value + locLeve.Value + locType.Value;
                                var existD = OldDetailDataList.Where(x => x.LOC_CODE == locCode);
                                if (!existD.Any())
                                {
                                    var detailData = new wcf.P710101DetailData();
                                    detailData.CHANNEL = channel.Value;
                                    detailData.LOC_CODE = locCode;
                                    detailData.PLAIN = plain.Value;
                                    detailData.LOC_LEVEL = locLeve.Value;
                                    detailData.AREA_CODE = "-1";
                                    OldDetailDataList.Add(detailData);
                                }
                            }
                            var detailCount = OldDetailDataList.Where(x => x.CHANNEL == channel.Value && x.PLAIN == plain.Value && x.LOC_LEVEL == locLeve.Value && x.LOC_CODE.Substring(0, 1) == SelectedQueryFloor).ToList();
                            if (ExcludeLoc != null)
                            {
                                existM.ChangeCount = detailCount.Where(o => !ExcludeLoc.Contains(o.LOC_CODE)).Count();
                                existM.NowLocCount = detailCount.Count();
                            }
                        }
                    }
                }
            }
            OldMasterDataList = OldMasterDataList.OrderBy(o => o.FloorNo).ThenBy(o => o.ChannelNo).ThenBy(o => o.PlainNo).ThenBy(o => o.LocLevelNo).ToObservableCollection();

            GetExcludeLoc();

            if (!isEditInit)
                SelectMasertData = OldMasterDataList.LastOrDefault();
            RefreshDataGrid();
        }

        private void GetExcludeLoc()
        {
            var proxywcf = new wcf.P71WcfServiceClient();
            NowLocData = RunWcfMethod<ObservableCollection<wcf.P710101DetailData>>(proxywcf.InnerChannel, () => proxywcf.GetLocDetailData(SelectedDcCode, OldDetailDataList.Select(x => x.LOC_CODE).ToArray()).ToObservableCollection());
            if (ExcludeLoc == null)
            {
                if (!IsEdit)
                {
                    ExcludeLoc = NowLocData.Select(x => x.LOC_CODE).ToList();
                }
                else
                {
                    if (OldEditDetailDataList == null)
                    {
                        ExcludeLoc = new List<string>();
                        var newLocData = OldDetailDataList.Where(x => string.IsNullOrWhiteSpace(x.WAREHOUSE_ID)).ToList();
                        foreach (var nLoc in newLocData)
                        {
                            if (!ExcludeLoc.Contains(nLoc.LOC_CODE))
                                ExcludeLoc.Add(nLoc.LOC_CODE);
                        }
                    }
                }
            }
            else
            {
                if (!IsEdit)
                {
                    var excludeList = NowLocData.Where(x => !ExcludeLoc.Contains(x.LOC_CODE)).Select(x => x.LOC_CODE).ToList();
                    foreach (var e in excludeList)
                        if (!ExcludeLoc.Contains(e))
                            ExcludeLoc.Add(e);
                }
                else
                {
                    if (OldEditDetailDataList == null)
                    {
                        var newLocData = OldDetailDataList.Where(x => string.IsNullOrWhiteSpace(x.WAREHOUSE_ID)).ToList();
                        foreach (var nLoc in newLocData)
                        {
                            if (!ExcludeLoc.Contains(nLoc.LOC_CODE))
                                ExcludeLoc.Add(nLoc.LOC_CODE);
                        }
                    }
                }
            }

            if (IsEdit)
            {
                if (OldEditDetailDataList == null || !OldEditDetailDataList.Any())
                {
                    var editData = NowLocData.Where(o => o.WAREHOUSE_ID == WarehouseId).Select(o => o.LOC_CODE);
                    foreach (var data in editData)
                    {
                        ExcludeLoc.Remove(data);
                        var detailData = OldDetailDataList.Where(o => o.LOC_CODE == data).FirstOrDefault();
                        if (detailData != null)
                            detailData.WAREHOUSE_ID = WarehouseId;
                    }
                }

                var exists = NowLocData.Where(o => o.WAREHOUSE_ID != WarehouseId).Select(o => o.LOC_CODE).ToList();
                foreach (var exist in exists)
                {
                    if (!ExcludeLoc.Contains(exist))
                        ExcludeLoc.Add(exist);
                }
            }
            else
            {
                CountMasterLoc(null, true);
            }
        }
        /// <summary>
        /// 單筆儲位新增
        /// </summary>
        private void AddGridLocDataByCustomize()
        {
            if (LocText.Length != 9)
            {
                ShowWarningMessage(Properties.Resources.P7101010100_ViewModel_LocTextFormatError);
                return;
            }

            var floor = LocText.Substring(0, 1);
            var channel = LocText.Substring(1, 2);
            var plain = LocText.Substring(3, 2);
            var locLevel = LocText.Substring(5, 2);
            var locType = LocText.Substring(7, 2);
            if (!LocSetData.Where(x => x.TYPE == "1" && x.VALUE == floor).Any() ||
            !LocSetData.Where(x => x.TYPE == "2" && x.VALUE == channel).Any() ||
            !LocSetData.Where(x => x.TYPE == "3" && x.VALUE == plain).Any() ||
            !LocSetData.Where(x => x.TYPE == "4" && x.VALUE == locLevel).Any() ||
            !LocSetData.Where(x => x.TYPE == "5" && x.VALUE == locType).Any())
            {
                ShowWarningMessage(Properties.Resources.P7101010100_ViewModel_LocSetDataRangeCheck);
                return;
            }
            bool isNewData = false;

            if (OldMasterDataList == null || !OldMasterDataList.Any())
                OldMasterDataList = new ObservableCollection<P710101MasterData>();
            if (OldDetailDataList == null || !OldDetailDataList.Any())
                OldDetailDataList = new List<wcf.P710101DetailData>();

            var msterExist = OldMasterDataList.Where(x => x.FloorNo == floor && x.ChannelNo == channel && x.PlainNo == plain && x.LocLevelNo == locLevel).FirstOrDefault();
            if (msterExist == null)
            {
                var masterData = new P710101MasterData();
                masterData.FloorNo = floor;
                masterData.ChannelNo = channel;
                masterData.PlainNo = plain;
                masterData.LocLevelNo = locLevel;
                masterData.ChangeCount = 0;
                masterData.OldLocCount = 1;
                masterData.NowLocCount = 1;
                masterData.SettingStatus = "1";
                OldMasterDataList.Add(masterData);
                isNewData = true;
            }
            else
            {
                msterExist.SettingStatus = "2";
            }

            var existD = OldDetailDataList.Where(x => x.LOC_CODE == LocText);
            if (!existD.Any())
            {
                var detailData = new wcf.P710101DetailData();
                detailData.CHANNEL = channel;
                detailData.LOC_CODE = LocText;
                detailData.PLAIN = plain;
                detailData.LOC_LEVEL = locLevel;
                detailData.AREA_CODE = "-1";
                OldDetailDataList.Add(detailData);
                if (msterExist != null)
                {
                    msterExist.NowLocCount = OldDetailDataList.Where(o => o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && o.CHANNEL == msterExist.ChannelNo && o.PLAIN == msterExist.PlainNo && o.LOC_LEVEL == msterExist.LocLevelNo).Count();
                    msterExist.OldLocCount = OldDetailDataList.Where(o => o.LOC_CODE.Substring(0, 1) == SelectedQueryFloor && o.CHANNEL == msterExist.ChannelNo && o.PLAIN == msterExist.PlainNo && o.LOC_LEVEL == msterExist.LocLevelNo).Count();
                }
                isNewData = true;
            }
            if (!isNewData)
            {
                SelectMasertData = OldMasterDataList.FirstOrDefault(x => x.ChannelNo == channel && x.PlainNo == plain && x.LocLevelNo == locLevel);
                return;
            }

            var proxywcf = new wcf.P71WcfServiceClient();
            NowLocData = RunWcfMethod<ObservableCollection<wcf.P710101DetailData>>(proxywcf.InnerChannel, () => proxywcf.GetLocDetailData(SelectedDcCode, OldDetailDataList.Select(x => x.LOC_CODE).ToArray()).ToObservableCollection());
            if (ExcludeLoc == null)
                ExcludeLoc = NowLocData.Select(x => x.LOC_CODE).ToList();
            else
            {
                if (!IsEdit)
                {
                    var excludeList = NowLocData.Where(x => !ExcludeLoc.Contains(x.LOC_CODE)).Select(x => x.LOC_CODE).ToList();
                    foreach (var e in excludeList)
                        if (!ExcludeLoc.Contains(e))
                            ExcludeLoc.Add(e);
                }
            }

            if (IsEdit)
            {
                if (OldEditDetailDataList == null || !OldEditDetailDataList.Any())
                {
                    var editData = NowLocData.Where(o => o.WAREHOUSE_ID == WarehouseId).Select(o => o.LOC_CODE);
                    foreach (var data in editData)
                    {
                        ExcludeLoc.Remove(data);
                        var detailData = OldDetailDataList.Where(o => o.LOC_CODE == data).FirstOrDefault();
                        if (detailData != null)
                            detailData.WAREHOUSE_ID = WarehouseId;
                    }
                }

                var exists = NowLocData.Where(o => o.WAREHOUSE_ID != WarehouseId).Select(o => o.LOC_CODE).ToList();
                foreach (var exist in exists)
                {
                    if (!ExcludeLoc.Contains(exist))
                        ExcludeLoc.Add(exist);
                }
            }

            SelectMasertData = OldMasterDataList.FirstOrDefault(x => x.ChannelNo == channel && x.PlainNo == plain && x.LocLevelNo == locLevel);
            CountMasterLoc(null, true);
        }

        /// <summary>
        /// DC_CODE異動時清除儲位資料
        /// </summary>
        private void ClaerLocList()
        {
            DetailDataList = null;
            OldDetailDataList = new List<ExDataServices.P71WcfService.P710101DetailData>();
            OldMasterDataList = null;
            MasterDataList = null;
            NowLocData = null;
            QueryFloorList = null;
        }
        /// <summary>
        /// 刷新MasterDataGrid
        /// </summary>
        public void RefreshDataGrid()
        {
            if (OldMasterDataList == null || !OldMasterDataList.Any())
            {
                MasterDataList = new ObservableCollection<P710101MasterData>();
                DetailDataList = new SelectionList<ExDataServices.P71WcfService.P710101DetailData>(new List<ExDataServices.P71WcfService.P710101DetailData>());
                return;
            }
            //isRefresh = true;
            var temp = SelectMasertData;
            MasterDataList = OldMasterDataList.Where(o => o.FloorNo == SelectedQueryFloor).ToObservableCollection();
            SelectMasertData = temp;
            //isRefresh = false;
        }

        #region Confirm
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
        /// 確認資料異動
        /// </summary>
        public void ConfirmData()
        {
            if (DetailDataList == null || !DetailDataList.Any())
                return;
            foreach (var data in DetailDataList)
            {
                if (data.IsSelected)
                {
                    if (ExcludeLoc.Contains(data.Item.LOC_CODE))
                        ExcludeLoc.Remove(data.Item.LOC_CODE);
                }
                else
                {
                    if (!ExcludeLoc.Contains(data.Item.LOC_CODE))
                        ExcludeLoc.Add(data.Item.LOC_CODE);
                }
            }
            GetLocCount();
            TempLocCodeList = DetailDataList.Where(x => x.IsSelected).Select(x => x.Item.LOC_CODE).ToList();
            if (DetailDataList.Any())
            {
                var param = DetailDataList.FirstOrDefault().Item;
                var oldSelectMaster = OldMasterDataList.Where(o => o.ChannelNo == param.CHANNEL && o.PlainNo == param.PLAIN && o.LocLevelNo == param.LOC_LEVEL).FirstOrDefault();
                CountMasterLoc(oldSelectMaster);
            }
        }
        #endregion

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

        #region QueryDetail
        public ICommand QueryDetailCommand
        {
            get
            {
                return CreateBusyAsyncCommand(o => { },
                    () => true,
                    o =>
                    {
                        GetDetailData(SelectedQueryFloor, SelectedAreaType);
                    });
            }
        }
        #endregion

        #region AddLoc
        public ICommand AddLocCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => true,
                    o =>
                    {
                        if (SettingType == SettingMode.LocArea)
                        {
                            AddGridLocDataByArea();
                        }
                        else
                        {
                            AddGridLocDataByCustomize();
                        }
                        ConfirmData();
                        SetQueryFloorData();
                        SelectedAreaType = AreaType?.FirstOrDefault(x => x.Value == "0")?.Value;
                    }
                    );
            }
        }
        #endregion

        /// <summary>
        /// 編輯模式下綁定資料
        /// </summary>
        /// <param name="f1980Data"></param>
        public void BindEditData(F1980Data f1980Data)
        {
            if (f1980Data != null)
            {
                IsEdit = true;
                IsNotEdit = false;
                AreaType = new ObservableCollection<NameValuePair<string>>();
                lbWarehouseTypeVisibility = Visibility.Collapsed;
                txtWarehouseIDVisibility = Visibility.Visible;

                SelectedDcCode = f1980Data.DC_CODE;
                SelectedGupCode = f1980Data.GUP_CODE;
                SelectedCustCode = f1980Data.CUST_CODE;
                SelectedWarehouseType = f1980Data.WAREHOUSE_TYPE;
                WarehouseName = f1980Data.WAREHOUSE_Name;
                WarehouseId = f1980Data.WAREHOUSE_ID;
                WarehouseNum = f1980Data.WAREHOUSE_ID.Substring(1);
                SelectedTempType = f1980Data.TMPR_TYPE;
                CalStock = f1980Data.CAL_STOCK == "1";
                CalFee = f1980Data.CAL_FEE == "1";
                HorDistance = (decimal)f1980Data.HOR_DISTANCE;
                SelectedDeviceType = f1980Data.DEVICE_TYPE;
                SelectedPickFloor = f1980Data.PICK_FLOOR;
                SelectedWarehouseLocType = f1980Data.LOC_TYPE_ID;
                var f1942Item = _f1942List.Find(o => o.LOC_TYPE_ID == _selectedWarehouseLocType);
                SelectedHandy = f1942Item.HANDY;
                RentBeginDate = f1980Data.RENT_BEGIN_DATE;
                RentEndDate = f1980Data.RENT_END_DATE;

                var proxy = GetProxy<F19Entities>();
                var data = proxy.F1912s.Where(o => o.WAREHOUSE_ID == WarehouseId && o.DC_CODE == SelectedDcCode && o.GUP_CODE == SelectedGupCode && o.CUST_CODE == SelectedCustCode).ToList();
                var queryFloor = data.GroupBy(o => o.FLOOR).ToList();
                QueryFloorList = (from o in queryFloor
                                  select new NameValuePair<string>
                                  {
                                      Name = o.Key,
                                      Value = o.Key
                                  }).ToObservableCollection();
                SelectedQueryFloor = QueryFloorList.FirstOrDefault().Value;
                var area = data.GroupBy(o => o.AREA_CODE).Select(o => o.Key).ToList();
                var f1919Data = proxy.F1919s.ToList();
                f1919Data = f1919Data.Where(o => area.Contains(o.AREA_CODE) && o.DC_CODE == SelectedDcCode).ToList();
                foreach (var a in f1919Data)
                {
                    if (!a.AREA_CODE.Contains("-1"))
                        AreaType.Add(new NameValuePair<string> { Name = a.AREA_NAME, Value = a.AREA_CODE });
                }

                SelectedFloor = QueryFloorList.FirstOrDefault().Value;
                SelectedMinChannel = MinChannelList.FirstOrDefault().Value;
                SelectedMaxChannel = MaxChannelList.FirstOrDefault().Value;
                SelectedMinPlain = MinPlainList.FirstOrDefault().Value;
                SelectedMaxPlain = MaxPlainList.FirstOrDefault().Value;
                SelectedMinLocLevel = MinLocLevelList.FirstOrDefault().Value;
                SelectedMaxLocLevel = MaxLocLevelList.FirstOrDefault().Value;
                SelectedMinLocType = MinLocTypeList.FirstOrDefault().Value;
                SelectedMaxLocType = MaxLocTypeList.FirstOrDefault().Value;
                SelectedQueryFloor = QueryFloorList.FirstOrDefault().Value;
                //編輯儲位資料處裡
                EditGridLocDataByArea(data.ToList());

                OldMasterDataList = OldMasterDataList.OrderBy(o => o.FloorNo).ThenBy(o => o.ChannelNo).ThenBy(o => o.PlainNo).ThenBy(o => o.LocLevelNo).ToObservableCollection();
                GetExcludeLoc();
                SelectMasertData = OldMasterDataList.LastOrDefault();

                var floor = SelectedQueryFloor ?? SelectedFloor;
                foreach (var masterData in OldMasterDataList)
                {
                    var exist = NowLocData.Where(x => x.CHANNEL == masterData.ChannelNo && x.PLAIN == masterData.PlainNo && x.LOC_LEVEL == masterData.LocLevelNo && x.LOC_CODE.Substring(0, 1) == floor && x.WAREHOUSE_ID == WarehouseId).ToList();
                    var newLocCount = OldDetailDataList.Where(x => x.CHANNEL == masterData.ChannelNo && x.PLAIN == masterData.PlainNo && x.LOC_LEVEL == masterData.LocLevelNo && x.LOC_CODE.Substring(0, 1) == floor && string.IsNullOrWhiteSpace(x.WAREHOUSE_ID)).Count();
                    if (exist.Any())
                    {
                        masterData.NowLocCount = exist.Count();
                        masterData.OldLocCount = exist.Count();
                        masterData.ChangeCount = masterData.NowLocCount - masterData.OldLocCount;
                        masterData.SettingStatus = "0";
                    }
                }
                SelectedFloor = FloorList.FirstOrDefault().Value;
                SelectedQueryFloor = QueryFloorList.FirstOrDefault().Value;
                OldLocTotalCount = OldDetailDataList.Where(o => !ExcludeLoc.Contains(o.LOC_CODE) && o.WAREHOUSE_ID == WarehouseId).Count();
                EditExcludeLoc = ExcludeLoc.ToList();
                OldEditDetailDataList = OldDetailDataList.ToObservableCollection();
                CountMasterLoc(null, true);
                GetLocCount();
            }
            else
            {
                IsEdit = false;
                IsNotEdit = true;
            }
        }
    }

    public enum SettingMode
    {
        LocArea,
        Customize
    }
}
