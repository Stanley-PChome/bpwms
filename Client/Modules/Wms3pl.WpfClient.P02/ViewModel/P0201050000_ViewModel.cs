using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
    public partial class P0201050000_ViewModel : InputViewModelBase
    {
        private string _userId;
        public Action OnCheckEmpIDComplete = delegate { };
        public Action OnFocusEmpID = delegate { };
        public Action OnInsertScanCargoDataComplete = delegate { };
        public Action OnInsertScanReceiptDataComplete = delegate { };
        public P0201050000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                InitControls();
            }
        }

        private void InitControls()
        {
            _userId = Wms3plSession.Get<UserInfo>().Account;
            DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;

            ScanCargoDatasDisplayMember = new ObservableCollection<ScanCargoData>();
            ScanCargoDatasValueMember = new ObservableCollection<ScanCargoData>();

            ScanReceiptDatasDisplayMember = new ObservableCollection<ScanReceiptData>();
            ScanReceiptDatasValueMember = new ObservableCollection<ScanReceiptData>();
        }

        private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
        private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

        private string _selectedDc = string.Empty;
        /// <summary>
        /// 選取的物流中心
        /// </summary>
        public string SelectedDc
        {
            get { return _selectedDc; }
            set
            {
                _selectedDc = value;
                RaisePropertyChanged();
                SetLogisticCodeList();
            }
        }
        private List<NameValuePair<string>> _dcList;
        /// <summary>
        /// 物流中心清單
        /// </summary>
        public List<NameValuePair<string>> DcList
        {
            get { return _dcList; }
            set { _dcList = value; RaisePropertyChanged("DcList"); }
        }

        private List<NameValuePair<string>> _LogisticList;
        /// <summary>
        /// 物流中心物流商
        /// </summary>
        public List<NameValuePair<string>> LogisticList
        {
            get { return _LogisticList; }
            set
            {
                _LogisticList = value;
                RaisePropertyChanged();
            }
        }

        private NameValuePair<string> _SelectedLogistic;
        public NameValuePair<string> SelectedLogistic
        {
            get { return _SelectedLogistic; }
            set
            {
                _SelectedLogistic = value;
                RaisePropertyChanged();
            }
        }

        private int _TabControlSelectedIndex;
        /// <summary>
        /// TabControl目前顯示的index 0=刷貨作業 1=刷單作業
        /// </summary>
        public int TabControlSelectedIndex
        {
            get { return _TabControlSelectedIndex; }
            set
            {
                Set(() => TabControlSelectedIndex, ref _TabControlSelectedIndex, value);
                if (TabControlSelectedIndex == 1)
                    LoadScanReceiptCommand.Execute(null);
            }
        }

        private string _empID = string.Empty;
        /// <summary>
        /// 工號
        /// </summary>
        public string EmpID
        {
            get { return _empID; }
            set { Set(() => EmpID, ref _empID, value); }
        }

        private string _empName = string.Empty;
        /// <summary>
        /// 員工名稱
        /// </summary>
        public string EmpName
        {
            get { return _empName; }
            set
            {
                Set(() => EmpName, ref _empName, value);
                if (!string.IsNullOrWhiteSpace(value) && value != "查無此工號")
                {
                    LoadTodayUncheckedCargo(true);
                    IsQueryMode = false;
                    TabControlSelectedIndex = 0;
                    OnCheckEmpIDComplete();
                }
            }
        }

        private string _CargofreightNo = string.Empty;
        /// <summary>
        /// 刷貨作業貨運單號
        /// </summary>
        public string CargofreightNo
        {
            get { return _CargofreightNo; }
            set { Set(() => CargofreightNo, ref _CargofreightNo, value); }
        }

        private string _CargoFreightNoFilter = string.Empty;
        /// <summary>
        /// 刷貨作業貨運單號篩選
        /// </summary>
        public string CargoFreightNoFilter
        {
            get { return _CargoFreightNoFilter; }
            set { Set(() => CargoFreightNoFilter, ref _CargoFreightNoFilter, value); }
        }

        private int _freightOrderQty;
        /// <summary>
        /// 貨單筆數
        /// </summary>
        public int FreightOrderQty
        {
            get { return _freightOrderQty; }
            set { Set(() => FreightOrderQty, ref _freightOrderQty, value); }
        }

        private int _boxQty;
        /// <summary>
        /// 箱數
        /// </summary>
        public int BoxQty
        {
            get { return _boxQty; }
            set { Set(() => BoxQty, ref _boxQty, value); }
        }

        private int _totalBoxQty = 0;
        /// <summary>
        /// 總箱數
        /// </summary>
        public int TotalBoxQty
        {
            get { return _totalBoxQty; }
            set { Set(() => TotalBoxQty, ref _totalBoxQty, value); }
        }

        private bool _loadTodayUncheckedCargoInfo;
        /// <summary>
        /// 載入今日未核的貨物資料
        /// </summary>
        public bool LoadTodayUncheckedCargoInfo
        {
            get { return _loadTodayUncheckedCargoInfo; }
            set
            {
                Set(() => LoadTodayUncheckedCargoInfo, ref _loadTodayUncheckedCargoInfo, value);
                LoadTodayUncheckedCargo(!value);
                CargoFreightNoFilter = "";
            }
        }

        private bool _displayTotalNumberOfCases = true;
        /// <summary>
        /// 顯示總箱數看盤
        /// </summary>
        public bool DisplayTotalNumberOfCases
        {
            get { return _displayTotalNumberOfCases; }
            set { Set(() => DisplayTotalNumberOfCases, ref _displayTotalNumberOfCases, value); }
        }

        private Boolean _IsInputMode = true;
        public Boolean IsQueryMode
        {
            get { return _IsInputMode; }
            set
            {
                _IsInputMode = value; RaisePropertyChanged();
            }
        }

        private ScanCargoData _selectedScanCargoData;
        /// <summary>
        /// 選擇刷貨作業資料
        /// </summary>
        public ScanCargoData SelectedScanCargoData
        {
            get { return _selectedScanCargoData; }
            set { Set(() => SelectedScanCargoData, ref _selectedScanCargoData, value); }
        }

        private Boolean _isCheckAllScanCargoList;
        /// <summary>
        /// 刷貨作業清單全選狀態
        /// </summary>
        public Boolean IsCheckAllScanCargoList
        {
            get { return _isCheckAllScanCargoList; }
            set
            {
                Set(() => IsCheckAllScanCargoList, ref _isCheckAllScanCargoList, value);
                CheckAllScanCargoList(value);
            }
        }

        private ObservableCollection<ScanCargoData> _ScanCargoDatasDisplayMember;
        /// <summary>
        /// 刷貨作業資料
        /// </summary>
        public ObservableCollection<ScanCargoData> ScanCargoDatasDisplayMember
        {
            get { return _ScanCargoDatasDisplayMember; }
            set
            {
                Set(() => ScanCargoDatasDisplayMember, ref _ScanCargoDatasDisplayMember, value);
            }
        }

        private ObservableCollection<ScanCargoData> _ScanCargoDatasValueMember;
        /// <summary>
        /// 刷貨作業資料(User刷入的資料，不包含勾選今日未核的貨物資料的內容)
        /// </summary>
        public ObservableCollection<ScanCargoData> ScanCargoDatasValueMember
        {
            get { return _ScanCargoDatasValueMember; }
            set
            {
                Set(() => ScanCargoDatasValueMember, ref _ScanCargoDatasValueMember, value);
                FreightOrderQty = _ScanCargoDatasValueMember.GroupBy(x => new { x.DC_CODE, x.ALL_ID, x.SHIP_ORD_NO }).Count();
                BoxQty = _ScanCargoDatasValueMember.Sum(x => x.BOX_CNT);
                TotalBoxQty = _ScanCargoDatasValueMember.Sum(x => x.BOX_CNT);
            }
        }

        private ObservableCollection<ReceiptUnCheckData> _ReceiptUnCheckDatasDisplayMember;
        /// <summary>
        /// 刷單作業-未核貨單
        /// </summary>
        public ObservableCollection<ReceiptUnCheckData> ReceiptUnCheckDatasDisplayMember
        {
            get { return _ReceiptUnCheckDatasDisplayMember; }
            set { Set(() => ReceiptUnCheckDatasDisplayMember, ref _ReceiptUnCheckDatasDisplayMember, value); }
        }

        private ObservableCollection<ReceiptUnCheckData> _ReceiptUnCheckDatasValueMember;
        /// <summary>
        /// 刷單作業-未核貨單
        /// </summary>
        public ObservableCollection<ReceiptUnCheckData> ReceiptUnCheckDatasValueMember
        {
            get { return _ReceiptUnCheckDatasValueMember; }
            set { Set(() => ReceiptUnCheckDatasValueMember, ref _ReceiptUnCheckDatasValueMember, value); }
        }


        private ObservableCollection<ScanReceiptData> _ScanReceiptDatasOnlyScanValue;
        /// <summary>
        /// 刷單作業-刷單記錄(User刷入的原始資料資料)
        /// </summary>
        public ObservableCollection<ScanReceiptData> ScanReceiptDatasValueMember
        {
            get { return _ScanReceiptDatasOnlyScanValue; }
            set
            {
                Set(() => ScanReceiptDatasValueMember, ref _ScanReceiptDatasOnlyScanValue, value);
                ReceiptCheckedCount = _ScanReceiptDatasOnlyScanValue.Count;
                ReceiptCheckedBoxQty = _ScanReceiptDatasOnlyScanValue.Sum(x => x.SHIP_BOX_CNT);
                ReceiptUnnormalCount = _ScanReceiptDatasOnlyScanValue.Count(x => x.CHECK_STATUS == "X");
            }
        }

        private ObservableCollection<ScanReceiptData> _ScanReceiptDatas;
        /// <summary>
        /// 刷單作業-刷單記錄
        /// </summary>
        public ObservableCollection<ScanReceiptData> ScanReceiptDatasDisplayMember
        {
            get { return _ScanReceiptDatas; }
            set { Set(() => ScanReceiptDatasDisplayMember, ref _ScanReceiptDatas, value); }
        }


        private ScanReceiptData _SelectScanReceiptData;
        /// <summary>
        /// 刷單作業-選取的刷單記錄
        /// </summary>
        public ScanReceiptData SelectScanReceiptData
        {
            get { return _SelectScanReceiptData; }
            set { Set(() => SelectScanReceiptData, ref _SelectScanReceiptData, value); }
        }


        private ReceiptUnCheckData _SelectReceiptUnCheckData;
        public ReceiptUnCheckData SelectReceiptUnCheckData
        {
            get { return _SelectReceiptUnCheckData; }
            set { Set(() => SelectReceiptUnCheckData, ref _SelectReceiptUnCheckData, value); }
        }

        private string _ReceiptFreightNo;
        /// <summary>
        /// 刷單作業-貨運單號
        /// </summary>
        public string ReceiptFreightNo
        {
            get { return _ReceiptFreightNo; }
            set { Set(() => ReceiptFreightNo, ref _ReceiptFreightNo, value); }
        }


        private int _ReceiptCargoCount = 0;
        /// <summary>
        /// 刷單作業-貨單筆數
        /// </summary>
        public int ReceiptCargoCount
        {
            get { return _ReceiptCargoCount; }
            set { Set(() => ReceiptCargoCount, ref _ReceiptCargoCount, value); }
        }

        private int _ReceiptCargoBoxSum = 0;
        /// <summary>
        /// 刷單作業-貨單筆數
        /// </summary>
        public int ReceiptCargoBoxSum
        {
            get { return _ReceiptCargoBoxSum; }
            set { Set(() => ReceiptCargoBoxSum, ref _ReceiptCargoBoxSum, value); }
        }

        private int _ReceiptConfirmBoxCount = 0;
        /// <summary>
        /// 刷單作業-核對箱數
        /// </summary>
        public int ReceiptConfirmBoxCount
        {
            get { return _ReceiptConfirmBoxCount; }
            set { Set(() => ReceiptConfirmBoxCount, ref _ReceiptConfirmBoxCount, value); }
        }

        private Boolean _ReceiptShowUnnormalData = false;
        /// <summary>
        /// 刷單作業-只顯示異常資料
        /// </summary>
        public Boolean ReceiptShowUnnormalData
        {
            get { return _ReceiptShowUnnormalData; }
            set
            {
                Set(() => ReceiptShowUnnormalData, ref _ReceiptShowUnnormalData, value);
                if (value)
                {
                    var filterdata = ScanReceiptDatasDisplayMember.Where(x => x.CHECK_STATUS == "X").ToObservableCollection();
                    ScanReceiptDatasDisplayMember = filterdata;
                }
                else
                    ScanReceiptDatasDisplayMember = ScanReceiptDatasValueMember;

                ReceiptReceiptFreightNoFilter = "";
            }
        }

        private int _ReceiptUnnormalCount;
        /// <summary>
        /// 刷單作業-異常筆數
        /// </summary>
        public int ReceiptUnnormalCount
        {
            get { return _ReceiptUnnormalCount; }
            set
            {
                Set(() => ReceiptUnnormalCount, ref _ReceiptUnnormalCount, value);

            }
        }

        private string _ReceiptReceiptFreightNoFilter;
        /// <summary>
        /// 刷單作業-刷單記錄-貨運單號篩選
        /// </summary>
        public string ReceiptReceiptFreightNoFilter
        {
            get { return _ReceiptReceiptFreightNoFilter; }
            set { Set(() => ReceiptReceiptFreightNoFilter, ref _ReceiptReceiptFreightNoFilter, value); }
        }

        private string _ReceiptUncheckFreightNoFilter;
        /// <summary>
        /// 刷單作業-未核貨單-貨運單號篩選
        /// </summary>
        public string ReceiptUncheckFreightNoFilter
        {
            get { return _ReceiptUncheckFreightNoFilter; }
            set { Set(() => ReceiptUncheckFreightNoFilter, ref _ReceiptUncheckFreightNoFilter, value); }
        }

        private int _ReceiptCheckedCount = 0;
        /// <summary>
        /// 刷單作業-核對筆數
        /// </summary>
        public int ReceiptCheckedCount
        {
            get { return _ReceiptCheckedCount; }
            set { Set(() => ReceiptCheckedCount, ref _ReceiptCheckedCount, value); }
        }

        private int _ReceiptCheckedBoxQty = 0;
        /// <summary>
        /// 刷單作業-(核對)箱數
        /// </summary>
        public int ReceiptCheckedBoxQty
        {
            get { return _ReceiptCheckedBoxQty; }
            set { Set(() => ReceiptCheckedBoxQty, ref _ReceiptCheckedBoxQty, value); }
        }

        public ICommand ScanCargoDataFindCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => true,
                    o =>
                    {
                        if (!(ScanCargoDatasValueMember?.Any() ?? true))
                            return;

                        var filterData = ScanCargoDatasValueMember.ToList().AsQueryable();
                        if (!string.IsNullOrWhiteSpace(CargoFreightNoFilter))
                        {
                            filterData = filterData.Where(x => x.SHIP_ORD_NO == CargoFreightNoFilter);
                        }
                        ScanCargoDatasDisplayMember = filterData.ToObservableCollection();
                    });
            }
        }

        public ICommand ReturnInitModeCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o =>
                    { },
                    () => true,
                    o =>
                    {
                        EmpID = "";
                        EmpName = "";
                        CargofreightNo = "";
                        CargoFreightNoFilter = "";
                        TabControlSelectedIndex = 0;
                        ScanCargoDatasDisplayMember.Clear();
                        LoadTodayUncheckedCargoInfo = false;
                        ScanCargoDatasValueMember = new ObservableCollection<ScanCargoData>();
                        ScanCargoDatasDisplayMember = new ObservableCollection<ScanCargoData>();

                        ReceiptFreightNo = "";
                        ReceiptConfirmBoxCount = 0;
                        ReceiptCargoCount = 0;
                        ReceiptCargoBoxSum = 0;
                        ReceiptCheckedCount = 0;
                        ReceiptCheckedBoxQty = 0;
                        ScanReceiptDatasValueMember = new ObservableCollection<ScanReceiptData>();
                        ScanReceiptDatasDisplayMember = new ObservableCollection<ScanReceiptData>();
                        IsQueryMode = true;

                        OnFocusEmpID();
                    });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand DeleteScanCargoDataCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o =>
                    { },
                    () => ScanCargoDatasDisplayMember?.Any(x => x.IsSelected) ?? false,
                    o =>
                    {
                        var selectedDeleteData = ScanCargoDatasDisplayMember.Where(x => x.IsSelected).ToList();
                        var wcfsvr = new wcf.P02WcfServiceClient();
                        var tmpData = ExDataMapper.MapCollection<ScanCargoData, wcf.ScanCargoData>(selectedDeleteData).ToArray();
                        var result = RunWcfMethod(wcfsvr.InnerChannel, () => wcfsvr.DeleteF010301ScanCargoDatas(tmpData));

                        ShowResultMessage(result);

                        if (result.IsSuccessed)
                            foreach (var item in selectedDeleteData)
                            {
                                ScanCargoDatasDisplayMember.Remove(item);
                                ScanCargoDatasValueMember.Remove(item);
                            }
                        ScanCargoDatasValueMember = ScanCargoDatasValueMember;
                    });
            }
        }

        public ICommand LoadScanReceiptCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o =>
                    {
                        var exproxy = GetExProxy<P02ExDataSource>();
                        var result = exproxy.CreateQuery<ScanCargoStatistic>("GetF010301ScanCargoStatistic")
                                    .AddQueryExOption("dcCode", SelectedDc)
                                    .AddQueryExOption("LogisticCode", SelectedLogistic.Value).ToList();
                        ReceiptCargoCount = result?.Count() ?? 0;
                        ReceiptCargoBoxSum = result?.Sum(x => x.BOX_QTY) ?? 0;

                        var TodayReceiptData = exproxy.CreateQuery<ScanReceiptData>("GetF010302TodayReceiptData")
                                    .AddQueryExOption("dcCode", SelectedDc)
                                    .AddQueryExOption("LogisticCode", SelectedLogistic.Value).ToObservableCollection();
                        ScanReceiptDatasDisplayMember = TodayReceiptData.ToObservableCollection();
                        ScanReceiptDatasValueMember = TodayReceiptData.ToObservableCollection();
                        LoadUnckeckReceiptCommand.Execute(null);
                    });
            }
        }

        public ICommand LoadUnckeckReceiptCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o =>
                    {
                        var exproxy = GetExProxy<P02ExDataSource>();
                        ReceiptUnCheckDatasDisplayMember = exproxy.CreateQuery<ReceiptUnCheckData>("GetF010301UncheckReceiptShipOrdNo")
                                   .AddQueryExOption("dcCode", SelectedDc)
                                   .AddQueryExOption("LogisticCode", SelectedLogistic.Value)
                                   .ToObservableCollection();

                        ReceiptUnCheckDatasValueMember = ReceiptUnCheckDatasDisplayMember.ToObservableCollection();
                    });
            }
        }

        public ICommand UpdateScanCargoMemoDataCommand
        {
            get
            {
                return CreateBusyAsyncCommand<String>(
                    MemoContext =>
                    {
                        var UpdateData = SelectedScanCargoData;
                        UpdateData.MEMO = MemoContext;

                        if (ScanCargoDatasValueMember != null && ScanCargoDatasValueMember.Count() > 0)
                        {
                            var setScanCargoDatasOnlyScanValue = ScanCargoDatasValueMember.FirstOrDefault(x => x.ID == UpdateData.ID);
                            if (setScanCargoDatasOnlyScanValue != null)
                                setScanCargoDatasOnlyScanValue.MEMO = MemoContext;
                        }

                        var wcfservice = new wcf.P02WcfServiceClient();
                        var result = RunWcfMethod<wcf.ExecuteResult>(wcfservice.InnerChannel,
                            () => wcfservice.UpdateF010301ScanCargoMemo(ExDataMapper.Map<ScanCargoData, wcf.ScanCargoData>(UpdateData)));
                        ShowResultMessage(result);

                    });
            }
        }

        public ICommand UpdateScanCargoBoxCntDataCommand
        {
            get
            {
                return CreateBusyAsyncCommand<Int16>(
                    CargoBoxCnt =>
                    {
                        var UpdateData = SelectedScanCargoData;
                        UpdateData.BOX_CNT = CargoBoxCnt;

                        if (ScanCargoDatasValueMember != null && ScanCargoDatasValueMember.Count() > 0)
                        {
                            var setScanCargoDatasOnlyScanValue = ScanCargoDatasValueMember.FirstOrDefault(x => x.ID == UpdateData.ID);
                            if (setScanCargoDatasOnlyScanValue != null)
                                setScanCargoDatasOnlyScanValue.BOX_CNT = CargoBoxCnt;
                        }

                        var wcfservice = new wcf.P02WcfServiceClient();
                        var result = RunWcfMethod<wcf.ExecuteResult>(wcfservice.InnerChannel,
                            () => wcfservice.UpdateF010301BoxCount(ExDataMapper.Map<ScanCargoData, wcf.ScanCargoData>(UpdateData)));
                        ShowResultMessage(result);
                        if (result.IsSuccessed)
                            ScanCargoDatasValueMember = ScanCargoDatasValueMember;
                    });
            }
        }

        public ICommand UpdateScanReceiptShipBoxCntCommand
        {
            get
            {
                return CreateBusyAsyncCommand<ScanReceiptData>(ShipBoxCnt =>
                {
                    var wcfservice = new wcf.P02WcfServiceClient();
                    var result = RunWcfMethod(wcfservice.InnerChannel,
                        () => wcfservice.UpdateF010302ShipBoxCnt(ExDataMapper.Map<ScanReceiptData, wcf.ScanReceiptData>(ShipBoxCnt)));
                    ShowResultMessage(result);

                    var tmpScanReceiptData = ScanReceiptDatasDisplayMember.First(x => x.ID == ShipBoxCnt.ID);
                    var tmpCHECK_STATUS = tmpScanReceiptData.CHECK_BOX_CNT != tmpScanReceiptData.SHIP_BOX_CNT ? "X" : "";

                    tmpScanReceiptData.SHIP_BOX_CNT = ShipBoxCnt.SHIP_BOX_CNT;
                    tmpScanReceiptData.CHECK_STATUS = tmpCHECK_STATUS;
                    RaisePropertyChanged("ScanReceiptDatas");

                    tmpScanReceiptData = ScanReceiptDatasValueMember.First(x => x.ID == ShipBoxCnt.ID);
                    tmpScanReceiptData.SHIP_BOX_CNT = ShipBoxCnt.SHIP_BOX_CNT;
                    tmpScanReceiptData.CHECK_STATUS = tmpCHECK_STATUS;
                    var TmpUpdateValue = ScanReceiptDatasValueMember;
                    ScanReceiptDatasValueMember = TmpUpdateValue;

                    LoadUnckeckReceiptCommand.Execute(null);
                },
                o => true);
            }
        }

        public ICommand ScanReceiptDatasFilterCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => true,
                    o =>
                    {
                        var filterData = ScanReceiptDatasValueMember.ToList().AsQueryable();
                        if (!string.IsNullOrWhiteSpace(ReceiptReceiptFreightNoFilter))
                            filterData = filterData.Where(x => x.SHIP_ORD_NO == ReceiptReceiptFreightNoFilter);
                        if (ReceiptShowUnnormalData)
                            filterData = filterData.Where(x => x.CHECK_STATUS == "X");

                        ScanReceiptDatasDisplayMember = filterData.ToObservableCollection();
                    });
            }
        }

        public ICommand ReceiptUnCheckDatasFilterCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => true,
                    o =>
                    {
                        var filterData = ReceiptUnCheckDatasValueMember.ToList().AsQueryable();
                        if (!string.IsNullOrWhiteSpace(ReceiptUncheckFreightNoFilter))
                            filterData = filterData.Where(x => x.SHIP_ORD_NO == ReceiptUncheckFreightNoFilter);
                        ReceiptUnCheckDatasDisplayMember = filterData.ToObservableCollection();
                    });
            }
        }

        public ICommand InsertScanCargoDataCommand
        {
            get
            {
                wcf.ScanCargoData result=null;
                return CreateBusyAsyncCommand(
                    o =>
                    {
                        result = InsertScanCargoData();
                    },
                    () => true,
                    o =>
                    {
                        if (result != null)
                            InsertScanCargoDataComplete(result);
                    });
            }
        }

        private void SetLogisticCodeList()
        {
            var proxy = GetProxy<F00Entities>();
            LogisticList = proxy.CreateQuery<F0002>("getLogisticList")
                             .AddQueryExOption("dcCode", SelectedDc)
                             .Select(x => new NameValuePair<string> { Name = x.LOGISTIC_NAME, Value = x.LOGISTIC_CODE })
                             .ToList();
            if (LogisticList == null || !LogisticList.Any())
            {
                ShowMessage(Messages.InfoNoData);
                return;
            }
            SelectedLogistic = LogisticList.First();
        }

        /// <summary>
        /// 新增刷貨作業資料
        /// </summary>
        private wcf.ScanCargoData InsertScanCargoData()
        {
            if (String.IsNullOrWhiteSpace(CargofreightNo))
            {
                ShowInfoMessage("請輸入貨運單號");
                return null;
            }
            var wcfservice = new wcf.P02WcfServiceClient();
            var f010301Data = new ScanCargoData()
            {
                DC_CODE = SelectedDc,
                ALL_ID = SelectedLogistic.Value,
                LOGISTIC_NAME = SelectedLogistic.Name,
                RECV_DATE = DateTime.Now.Date,
                RECV_TIME = DateTime.Now.ToString("HH:mm:ss"),
                RECV_USER = EmpID,
                RECV_NAME = EmpName,
                SHIP_ORD_NO = CargofreightNo,
                BOX_CNT = 1,
                CHECK_STATUS = "0"
            };
            var result = RunWcfMethod(wcfservice.InnerChannel,
                            () => wcfservice.InsertF010301AndGetNewID(ExDataMapper.Map<ScanCargoData, wcf.ScanCargoData>(f010301Data)));
            return result;
        }

        private void InsertScanCargoDataComplete(wcf.ScanCargoData result)
        {
            switch (result.IDk__BackingField)
            {
                case -1:
                    ShowWarningMessage("已有重複資料");
                    return;
                case -9000:
                    ShowWarningMessage("更新刷單資料錯誤");
                    return;
                case -9999:
                    ShowWarningMessage("資料新增錯誤");
                    return;
            }

            var f010301Data = ExDataMapper.Map<wcf.ScanCargoData, ScanCargoData>(result);
            f010301Data.LOGISTIC_NAME = SelectedLogistic.Name;
            ScanCargoDatasDisplayMember.Add(f010301Data);
            ScanCargoDatasDisplayMember = ScanCargoDatasDisplayMember;

            var tmpScanCargoDatas = ScanCargoDatasDisplayMember;

            tmpScanCargoDatas = ScanCargoDatasValueMember;
            tmpScanCargoDatas.Add(f010301Data);
            ScanCargoDatasValueMember = tmpScanCargoDatas;

            CargoFreightNoFilter = "";

            LoadUnckeckReceiptCommand.Execute(null);

            ReceiptReceiptFreightNoFilter = "";
            ReceiptUncheckFreightNoFilter = "";
            ReceiptShowUnnormalData = false;

            OnInsertScanCargoDataComplete();
        }

        public void InsertScanReceiptData()
        {
            if (String.IsNullOrWhiteSpace(ReceiptFreightNo))
            {
                ShowInfoMessage("請輸入貨運單號");
                return;
            }

            var wcfservice = new wcf.P02WcfServiceClient();
            var f010302Data = new ScanReceiptData()
            {
                DC_CODE = SelectedDc,
                ALL_ID = SelectedLogistic.Value,
                CHECK_DATE = DateTime.Now.Date,
                CHECK_TIME = DateTime.Now.ToString("HH:mm:ss"),
                CHECK_USER = EmpID,
                CHECK_NAME = EmpName,
                SHIP_ORD_NO = ReceiptFreightNo,
                SHIP_BOX_CNT = 1,
                CHECK_STATUS = "0"
            };

            var result = RunWcfMethod<wcf.ScanReceiptData>(wcfservice.InnerChannel,
                            () => wcfservice.InsertF010302AndReturnValue(ExDataMapper.Map<ScanReceiptData, wcf.ScanReceiptData>(f010302Data)));

            switch (result.IDk__BackingField)
            {
                case -1:
                    ShowWarningMessage("已有重複資料");
                    return;
                case -9000:
                    ShowWarningMessage("查無此貨運單號");
                    return;
                case -9999:
                    ShowWarningMessage("資料新增錯誤");
                    return;
            }

            f010302Data = ExDataMapper.Map<wcf.ScanReceiptData, ScanReceiptData>(result);
            f010302Data.LOGISTIC_NAME = SelectedLogistic.Name;
            ReceiptConfirmBoxCount = f010302Data.CHECK_BOX_CNT;

            var tmpScanReceiptData = ScanReceiptDatasDisplayMember.ToObservableCollection();
            tmpScanReceiptData.Add(f010302Data);
            ScanReceiptDatasDisplayMember = tmpScanReceiptData.ToObservableCollection();

            tmpScanReceiptData = ScanReceiptDatasValueMember.ToObservableCollection();
            tmpScanReceiptData.Add(f010302Data);
            ScanReceiptDatasValueMember = tmpScanReceiptData;

            LoadUnckeckReceiptCommand.Execute(null);

            OnInsertScanReceiptDataComplete();
        }

        /// <summary>
        /// 載入今日未核的貨物資料CheckBox勾選
        /// </summary>
        /// <param name="IsOwnData">是否只載入此工號的(非user)刷貨記錄</param>
        public void LoadTodayUncheckedCargo(Boolean IsOwnData)
        {
            var proxy = GetExProxy<P02ExDataSource>();
            var result = proxy.CreateQuery<ScanCargoData>("GetF010301UncheckedScanCargoDatas")
                        .AddQueryExOption("dcCode", SelectedDc)
                        .AddQueryExOption("LogisticCode", SelectedLogistic.Value)
                        .AddQueryExOption("RecvUser", IsOwnData ? EmpID : "")
                        .ToList();
            ScanCargoDatasValueMember = result.ToObservableCollection();
            ScanCargoDatasDisplayMember = result.ToObservableCollection();
        }

        /// <summary>
        /// 今日未核的貨物資料CheckBox取消勾選，並還原載入的資料
        /// </summary>
        public void RemoveTodayUncheckedScanCargo()
        {
            ScanCargoDatasDisplayMember = ScanCargoDatasValueMember.ToObservableCollection();
        }

        /// <summary>
        /// 刷貨作業DataGrid全選功能
        /// </summary>
        /// <param name="isChecked"></param>
        private void CheckAllScanCargoList(bool isChecked)
        {
            if (ScanCargoDatasDisplayMember != null)
                foreach (var item in ScanCargoDatasDisplayMember)
                    item.IsSelected = isChecked;
        }


    }

}
