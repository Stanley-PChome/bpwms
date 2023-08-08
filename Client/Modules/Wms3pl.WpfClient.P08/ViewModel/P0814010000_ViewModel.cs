using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
  public partial class P0814010000_ViewModel : InputViewModelBase
  {
    #region Private Variable
    private Regex rgx = new Regex(@"^[0-9-]+$");
    #endregion

    #region Public Variable
    /// <summary>
    /// Focus並全選容器條碼委派事件
    /// </summary>
    public Action FocusSelectAllContainer = delegate { };
    /// <summary>
    /// Focus並全選商品條碼委派事件
    /// </summary>
    public Action FocusSelectAllSerial = delegate { };
    /// <summary>
    /// 離開包裝完成後委派事件
    /// </summary>
    public Action OnExitPackingComplete = delegate { };
    /// <summary>
    /// 物流中心變更後委派事件
    /// </summary>
    public Action OnDcCodeChanged = delegate { };
    /// <summary>
    /// 滾輪移至指定出貨明細委派事件
    /// </summary>
    public Action OnScrollIntoDetailData = () => { };
    /// <summary>
    /// 滾輪移至刷讀紀錄最後一筆委派事件
    /// </summary>
    public Action OnScrollIntoSerialReadingLog = () => { };
    /// <summary>
    /// 開啟容器刷讀UI委派事件
    /// </summary>
    public Action OpenContainerWindow = delegate { };
    /// <summary>
    /// Device 的設定 (當物流中心改變時，就會去顯示設定 Device 的畫面)  
    /// </summary>
    public F910501 SelectedF910501;
    public F1946 _f1946 { get; set; }
    /// <summary>
    /// 包裝模式 (1:單人包裝站 2:包裝線包裝站)
    /// </summary>
    public string ShipMode { get; set; } = "1";
    public bool? ContainerRes;
    public SearchWmsOrderPackingDetailRes SelectItem;
    public ShipPackageService _shipPackageService;
    public List<NameValuePair<string>> _workStationStatusList;
    #endregion

    #region Constructor
    public P0814010000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        NoSpecReport = ShipMode == "1" ? "0" : "1";
        CloseByBoxno = "0";
        InitialData();
        _shipPackageService = new ShipPackageService(FunctionCode);
        DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
        if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
        _workStationStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F1946", "STATUS", false);
      }
    }
    #endregion

    #region Property

    #region 業主編號
    public string _gupCode
    {
      get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
    }

    #endregion

    #region 貨主編號
    public string _custCode
    {
      get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
    }
    #endregion

    #region 配箱站與封箱站分開
    private string _noSpecReport;
    public string NoSpecReport
    {
      get { return _noSpecReport; }
      set { _noSpecReport = value; }
    }
    #endregion

    #region 需刷讀紙箱條碼關箱
    private string _closeByBoxno;
    public string CloseByBoxno
    {
      get { return _closeByBoxno; }
      set { _closeByBoxno = value; }
    }
    #endregion

    #region 是否人員按下加箱
    /// <summary>
    /// 是否人員按下加箱
    /// </summary>
    private bool IsUserAppendBox { get; set; }
    #endregion


    #region 出貨商品總數
    private int _ordItemCnt;
    public int OrdItemCnt
    {
      get { return _ordItemCnt; }
      set
      {
        _ordItemCnt = value;
        StrOrdItemCnt = string.Format(Properties.Resources.P0814010000_ItemTotalCnt, Convert.ToString(_ordItemCnt));
      }
    }
    #endregion

    #region Form 物流中心清單
    private List<NameValuePair<string>> _dcList;
    /// <summary>
    /// 物流中心清單
    /// </summary>
    public List<NameValuePair<string>> DcList
    {
      get { return _dcList; }
      set { _dcList = value; RaisePropertyChanged("DcList"); }
    }
    #endregion

    #region Form 選取的物流中心
    private string _selectedDc = string.Empty;
    public string SelectedDc
    {
      get { return _selectedDc; }
      set
      {
        _selectedDc = value;
        RaisePropertyChanged("SelectedDc");
        if (value != null)
          OnDcCodeChanged();
      }
    }
    #endregion

    #region Form 顯示出貨商品總數
    private string _strOrdItemCnt;
    public string StrOrdItemCnt
    {
      get { return _strOrdItemCnt; }
      set
      {
        _strOrdItemCnt = value;
        RaisePropertyChanged("StrOrdItemCnt");
      }
    }
    #endregion

    #region Form 品號(或序號)
    private string _serial = string.Empty;
    public string Serial
    {
      get { return _serial; }
      set
      {
        _serial = value;
        RaisePropertyChanged("Serial");
      }
    }
    #endregion

    #region Form 工作站編號
    private string _workStationCode;
    public string WorkStationCode
    {
      get { return _workStationCode; }
      set { _workStationCode = value; RaisePropertyChanged("WorkStationCode"); }
    }
    #endregion

    #region Form 工作站狀態
    private string _workStationStatus;
    public string WorkStationStatus
    {
      get { return _workStationStatus; }
      set { _workStationStatus = value; RaisePropertyChanged("WorkStationStatus"); }
    }
    #endregion

    #region Form 印表機資訊
    private string _printDesc;
    public string PrintDesc
    {
      get { return _printDesc; }
      set { _printDesc = value; RaisePropertyChanged("PrintDesc"); }
    }
    #endregion

    #region Form - 容器條碼
    private string _containerCode = string.Empty;
    //[Required(AllowEmptyStrings = false)]
    //[Display(Name = "Required_ErrorMessage", ResourceType = typeof(Resources.Resources))]
    public string ContainerCode
    {
      get { return _containerCode; }
      set
      {
        _containerCode = value;
        RaisePropertyChanged("ContainerCode");
      }
    }

    #endregion

    #region Form - 語音

    private bool _soundOn = true;
    public bool SoundOn
    {
      get { return _soundOn; }
      set { _soundOn = value; RaisePropertyChanged("SoundOn"); }
    }
    #endregion

    #region Form 控制UI 下半部啟用/停用狀態

    #region 控制畫面下半部的狀態
    private bool _enableReadSerial = false;
    /// <summary>
    /// 控制畫面下半部的狀態
    /// </summary>
    public bool EnableReadSerial
    {
      get { return _enableReadSerial; }
      set { _enableReadSerial = value; RaisePropertyChanged("EnableReadSerial"); }
    }

    #endregion

    #endregion

    #region Form 出貨單資訊
    private wcf.SearchAndCheckWmsOrderInfoRes _wmsOrdData = null;
    public wcf.SearchAndCheckWmsOrderInfoRes WmsOrdData
    {
      get { return _wmsOrdData; }
      set
      {
        _wmsOrdData = value;
        RaisePropertyChanged("WmsOrdData");
      }
    }
    #endregion

    #region Form Grid 出貨單商品資訊
    private List<SearchWmsOrderPackingDetailRes> _detailData = new List<SearchWmsOrderPackingDetailRes>();
    public List<SearchWmsOrderPackingDetailRes> DetailData
    {
      get { return _detailData; }
      set
      {
        _detailData = value;
        RaisePropertyChanged("DetailData");
      }
    }

    private SearchWmsOrderPackingDetailRes _selectedDetailData;
    /// <summary>
    /// 選擇的出貨商品
    /// </summary>
    public SearchWmsOrderPackingDetailRes SelectedDetailData
    {
      get { return _selectedDetailData; }
      set
      {
        Set(() => SelectedDetailData, ref _selectedDetailData, value);
      }
    }
    #endregion

    #region Form Grid 序號/品號刷讀記錄(UI顯示)  刷讀記錄清單 / 選取的刷讀記錄

    private ObservableCollection<SearchWmsOrderScanLogRes> _serialReadingLog = new ObservableCollection<SearchWmsOrderScanLogRes>();
    /// <summary>
    /// 序號/品號刷讀記錄
    /// </summary>
    public ObservableCollection<SearchWmsOrderScanLogRes> SerialReadingLog
    {
      get { return _serialReadingLog; }
      set { _serialReadingLog = value; RaisePropertyChanged("SerialReadingLog"); }
    }

    private SearchWmsOrderScanLogRes _selectedSerialReading;
    /// <summary>
    /// 目前選擇的序號刷讀紀錄
    /// </summary>
    public SearchWmsOrderScanLogRes SelectedSerialReading
    {
      get { return _selectedSerialReading; }
      set
      {
        Set(() => SelectedSerialReading, ref _selectedSerialReading, value);
      }
    }

    #endregion

    #region Form 目前輸入的包裝數

    private string _packQty;
    /// <summary>
    /// 包裝數
    /// </summary>
    public string PackQty
    {
      get { return _packQty; }
      set { _packQty = value; RaisePropertyChanged("PackQty"); }
    }

    #endregion

    #region Form - 訊息
    /// <summary>
    /// 定義不出現提示音的訊息
    /// </summary>
    private List<string> noPlaySoundMessages = new List<string>() {
                Properties.Resources.P0807010000_FindBox,Properties.Resources.P0803010000_CaseNoIsNull2, Messages.ScanSerialorBox,Properties.Resources.P0807010000_AllowOrdItem,Properties.Resources.P0807010000_ScanOrginalItem
        };
    private SolidColorBrush _messageForeground = Brushes.Blue;
    public SolidColorBrush MessageForeground
    {
      get { return _messageForeground; }
      set
      {
        _messageForeground = value;
        RaisePropertyChanged("MessageForeground");

        if (MessageForeground == Brushes.White)
          MessageBackground = Brushes.Blue;
        else
          MessageBackground = Brushes.White;
      }
    }
    private SolidColorBrush _messageBackground = Brushes.White;
    public SolidColorBrush MessageBackground
    {
      get { return _messageBackground; }
      set { _messageBackground = value; RaisePropertyChanged("MessageBackground"); }
    }
    private string _message = string.Empty;
    public string Message
    {
      get { return _message; }
      set
      {
        _message = value;
        RaisePropertyChanged("Message");
      }
    }
    #endregion

    #endregion

    #region 畫面控制設定 Show/Hidden && Disabled/Enabled

    #region 按鈕_工作站設定 IsEnabled
    private bool _workStationSettingEnabled;
    /// <summary>
    /// 按鈕_工作站設定 IsEnabled
    /// </summary>
    public bool WorkStationSettingEnabled
    {
      get { return _workStationSettingEnabled; }
      set
      {
        _workStationSettingEnabled = value;
        RaisePropertyChanged("WorkStationSettingEnabled");
      }
    }
    #endregion

    #region 按鈕_補印 IsEnabled
    private bool _reprintEnabled;
    /// <summary>
    /// 按鈕_補印 IsEnabled
    /// </summary>
    public bool ReprintEnabled
    {
      get { return _reprintEnabled; }
      set
      {
        _reprintEnabled = value;
        RaisePropertyChanged("ReprintEnabled");
      }
    }
    #endregion

    #region 輸入框容器條碼 IsEnabled
    private bool _dcEnabled;
    /// <summary>
    /// 物流中心下拉選單 IsEnabled
    /// </summary>
    public bool DcEnabled
    {
      get { return _dcEnabled; }
      set
      {
        _dcEnabled = value;
        RaisePropertyChanged("DcEnabled");
      }
    }
    #endregion

    #region 輸入框容器條碼 IsEnabled
    private bool _containerCodeEnabled;
    /// <summary>
    /// 輸入框容器條碼 IsEnabled
    /// </summary>
    public bool ContainerEnabled
    {
      get { return _containerCodeEnabled; }
      set
      {
        _containerCodeEnabled = value;
        RaisePropertyChanged("ContainerEnabled");
      }
    }
    #endregion

    #region 按鈕_開始包裝 IsEnabled
    private bool _startPackingEnabled;
    /// <summary>
    /// 按鈕_開始包裝 IsEnabled
    /// </summary>
    public bool StartPackingEnabled
    {
      get { return _startPackingEnabled; }
      set
      {
        _startPackingEnabled = value;
        RaisePropertyChanged("StartPackingEnabled");
      }
    }
    #endregion

    #region 按鈕_包裝完成 IsEnabled
    private bool _endPackingEnabled;
    /// <summary>
    /// 按鈕_包裝完成 IsEnabled
    /// </summary>
    public bool EndPackingEnabled
    {
      get { return _endPackingEnabled; }
      set
      {
        _endPackingEnabled = value;
        RaisePropertyChanged("EndPackingEnabled");
      }
    }
    #endregion

    #region 按鈕_取消包裝 IsEnabled
    private bool _cancelPackingEnabled;
    /// <summary>
    /// 按鈕_取消包裝 IsEnabled
    /// </summary>
    public bool CancelPackingEnabled
    {
      get { return _cancelPackingEnabled; }
      set
      {
        _cancelPackingEnabled = value;
        RaisePropertyChanged("CancelPackingEnabled");
      }
    }
    #endregion

    #region 按鈕_手動關箱 IsEnabled
    private bool _closeBoxEnabled;
    /// <summary>
    /// 按鈕_手動關箱 IsEnabled
    /// </summary>
    public bool CloseBoxEnabled
    {
      get { return _closeBoxEnabled; }
      set
      {
        _closeBoxEnabled = value;
        RaisePropertyChanged("CloseBoxEnabled");
      }
    }
    #endregion

    #region 畫面其他按鈕、輸入框 IsEnabled
    private bool _itemVerifyEnabled;
    /// <summary>
    /// 畫面其他按鈕、輸入框 IsEnabled
    /// </summary>
    public bool ItemVerifyEnabled
    {
      get { return _itemVerifyEnabled; }
      set
      {
        _itemVerifyEnabled = value;
        RaisePropertyChanged("ItemVerifyEnabled");
      }
    }
    #endregion

    #region 按鈕 離開
    private Visibility _btnExitVisibility = Visibility.Collapsed;
    public Visibility BtnExitVisibility { get { return _btnExitVisibility; } set { _btnExitVisibility = value; RaisePropertyChanged("BtnExitVisibility"); } }
    #endregion

    #region 按鈕 暫停包裝
    private Visibility _btnPausePackingVisibility = Visibility.Collapsed;
    public Visibility BtnPausePackingVisibility { get { return _btnPausePackingVisibility; } set { _btnPausePackingVisibility = value; RaisePropertyChanged("BtnPausePackingVisibility"); } }
    #endregion

    #region 按鈕 工作站設定
    private Visibility _btnWorkStationSettingVisibility = Visibility.Visible;
    public Visibility BtnWorkStationSettingVisibility { get { return _btnWorkStationSettingVisibility; } set { _btnWorkStationSettingVisibility = value; RaisePropertyChanged("BtnWorkStationSettingVisibility"); } }
    #endregion

    #region 按鈕 補印
    private Visibility _btnReprintVisibility = Visibility.Visible;
    public Visibility BtnReprintVisibility { get { return _btnReprintVisibility; } set { _btnReprintVisibility = value; RaisePropertyChanged("BtnReprintVisibility"); } }
    #endregion

    #region 按鈕 開始包裝
    private Visibility _btnStartPackingVisibility = Visibility.Collapsed;
    public Visibility BtnStartPackingVisibility { get { return _btnStartPackingVisibility; } set { _btnStartPackingVisibility = value; RaisePropertyChanged("BtnStartPackingVisibility"); } }
    #endregion

    #region 按鈕 包裝完成
    private Visibility _btnEndPackingVisibility = Visibility.Collapsed;
    public Visibility BtnEndPackingVisibility { get { return _btnEndPackingVisibility; } set { _btnEndPackingVisibility = value; RaisePropertyChanged("BtnEndPackingVisibility"); } }
    #endregion

    #region 按鈕 取消包裝
    private Visibility _btnCancelPackingVisibility = Visibility.Collapsed;
    public Visibility BtnCancelPackingVisibility { get { return _btnCancelPackingVisibility; } set { _btnCancelPackingVisibility = value; RaisePropertyChanged("BtnCancelPackingVisibility"); } }
    #endregion

    #region 按鈕 取消包裝
    private Visibility _btnManualCloseBoxVisibility = Visibility.Collapsed;
    public Visibility BtnManualCloseBoxVisibility { get { return _btnManualCloseBoxVisibility; } set { _btnManualCloseBoxVisibility = value; RaisePropertyChanged("BtnManualCloseBoxVisibility"); } }
    #endregion

    #region 按鈕  取消到站紀錄
    private Visibility _btnCancelArriveRecordVisibility = Visibility.Collapsed;
    public Visibility BtnCancelArriveRecordVisibility { get { return _btnCancelArriveRecordVisibility; } set { _btnCancelArriveRecordVisibility = value; RaisePropertyChanged("BtnCancelArriveRecordVisibility"); } }
    #endregion

    #region 依照模式顯示/隱藏、Enabled/Disabled 控制項
    public Mode Mode { get; set; }
    protected void ChangeMode(Mode mode)
    {
      Mode = mode;
      switch (mode)
      {
        case Mode.InitCloseStation:     // 無資料、關站
          InitialData();
          ContainerCode = string.Empty;
          BtnExitVisibility = Visibility.Visible;
          BtnPausePackingVisibility = Visibility.Collapsed;
          BtnWorkStationSettingVisibility = Visibility.Visible;
          BtnReprintVisibility = Visibility.Visible;
          BtnStartPackingVisibility = Visibility.Visible;
          BtnEndPackingVisibility = Visibility.Collapsed;
          BtnCancelPackingVisibility = Visibility.Visible;
          BtnManualCloseBoxVisibility = Visibility.Collapsed;
          BtnCancelArriveRecordVisibility = Visibility.Collapsed;
          DcEnabled = true;
          ContainerEnabled = false;                                         //(2) 容器條碼 Disabled
          WorkStationSettingEnabled = true;                                 //(1) 工作站設定Enabled
          StartPackingEnabled = false;                                      //(3) 開始包裝 disabled
          CancelPackingEnabled = false;                                     //(4) 取消包裝 disabled
          ItemVerifyEnabled = false;                                        //(5) 畫面其他按鈕、輸入框 disabled
          ReprintEnabled = true;
          CloseBoxEnabled = false;
          EnableReadSerial = false;
          EndPackingEnabled = false;
          break;
        case Mode.InitOpenStation:      // 開站/關站中
          BtnExitVisibility = Visibility.Visible;
          BtnPausePackingVisibility = Visibility.Collapsed;
          BtnWorkStationSettingVisibility = Visibility.Visible;
          BtnReprintVisibility = Visibility.Visible;
          BtnStartPackingVisibility = Visibility.Visible;
          BtnEndPackingVisibility = (ShipMode == "2" && WmsOrdData != null && WmsOrdData.Result.IsSuccessed && WmsOrdData.IsPackCheck == "否" && DetailData.Any(x => x.DiffQty > 0)) ? Visibility.Visible : Visibility.Collapsed;
          BtnCancelPackingVisibility = Visibility.Visible;
          BtnManualCloseBoxVisibility = Visibility.Collapsed;
          BtnCancelArriveRecordVisibility = Visibility.Visible;
          WorkStationSettingEnabled = _f1946.STATUS == "3" ? false : true;  //(1) 工作站設定Disabled
          DcEnabled = true;
          if (ShipMode == "1")
            ContainerEnabled = true;                                      //(2) 容器條碼 Enabled，游標Focus
          StartPackingEnabled = true;                                       //(3) 開始包裝 enabled
          CancelPackingEnabled = false;                                     //(4) 取消包裝 disabled
          ItemVerifyEnabled = DetailData.Any()
              && WmsOrdData?.Result.IsSuccessed == true;                    //(5) 畫面其他按鈕、輸入框 disabled
          FocusSelectAllContainer();                                        // 游標Focus容器條碼
          ReprintEnabled = true;
          CloseBoxEnabled = false;
          EnableReadSerial = true;
          EndPackingEnabled = false;
          break;
        case Mode.OrderPackingFinished: // 出貨單已包裝完成
          BtnExitVisibility = Visibility.Visible;
          BtnPausePackingVisibility = Visibility.Collapsed;
          BtnWorkStationSettingVisibility = Visibility.Visible;
          BtnReprintVisibility = Visibility.Visible;
          BtnStartPackingVisibility = Visibility.Visible;
          BtnEndPackingVisibility = Visibility.Collapsed;
          BtnCancelPackingVisibility = Visibility.Visible;
          BtnManualCloseBoxVisibility = Visibility.Collapsed;
          BtnCancelArriveRecordVisibility = Visibility.Collapsed;
          EnableReadSerial = false;
          DcEnabled = false;
          if (ShipMode == "1")
            ContainerEnabled = true;
          FocusSelectAllContainer();
          ItemVerifyEnabled = false;
          DcEnabled = false;
          WorkStationSettingEnabled = true;
          ReprintEnabled = true;
          StartPackingEnabled = true;
          CloseBoxEnabled = true;
          CancelPackingEnabled = true;

          break;
        case Mode.OrderShipedAndCanceled:          // 出貨單已出貨 出貨單已取消
          BtnExitVisibility = Visibility.Visible;
          BtnPausePackingVisibility = Visibility.Collapsed;
          BtnWorkStationSettingVisibility = Visibility.Visible;
          BtnReprintVisibility = Visibility.Visible;
          BtnStartPackingVisibility = Visibility.Visible;
          BtnEndPackingVisibility = Visibility.Collapsed;
          BtnCancelPackingVisibility = Visibility.Visible;
          BtnManualCloseBoxVisibility = Visibility.Collapsed;

          DcEnabled = false;
          if (ShipMode == "1")
            ContainerEnabled = true;
          FocusSelectAllContainer();
          ItemVerifyEnabled = false;
          DcEnabled = false;
          WorkStationSettingEnabled = true;
          ReprintEnabled = true;
          StartPackingEnabled = true;
          CloseBoxEnabled = true;
          CancelPackingEnabled = false;
          EnableReadSerial = false;
          break;
        case Mode.Packing:              // 包裝中
          BtnExitVisibility = Visibility.Collapsed;
          BtnPausePackingVisibility = Visibility.Visible;
          BtnWorkStationSettingVisibility = Visibility.Visible;
          BtnReprintVisibility = Visibility.Visible;
          BtnStartPackingVisibility = Visibility.Collapsed;
          BtnEndPackingVisibility = Visibility.Visible;
          BtnCancelPackingVisibility = Visibility.Visible;
          BtnManualCloseBoxVisibility = Visibility.Visible;

          var hasTotalPackageQty = DetailData.Any(x => x.TotalPackageQty > 0);
          var hasPackageQty = DetailData.Any(x => x.PackageQty > 0);

          EndPackingEnabled = (WmsOrdData.Status == 0 || WmsOrdData.Status == 2) && hasTotalPackageQty && hasPackageQty;
          CancelPackingEnabled = hasTotalPackageQty;

          //ReprintEnabled = hasTotalPackageQty;
          CloseBoxEnabled = hasPackageQty;
          ItemVerifyEnabled = true;
          FocusSelectAllSerial();
          ContainerEnabled = false;

          if (DetailData.Any(x => x.DiffQty > 0) && WmsOrdData.IsPackCheck == "否")// f.訊息 = (f - 1) 如果[G].DiffQty>0 and[F]. IsPackCheck= 否
            Message = "商品不需要過刷，可直接按下<包裝完成>";
          else if (DetailData.Any(x => x.DiffQty > 0) && WmsOrdData.IsPackCheck == "是")// (f-2) 如果[G].DiffQty>0 and[F].IsPackCheck=是
            Message = "所有商品需要過刷，請刷讀商品條碼";
          else if (DetailData.All(x => x.DiffQty == 0) && CloseByBoxno == "1" && WmsOrdData.OrderType == "01")// (f-3)如果[G].DiffQty=0 AND 工作站設定設為[需刷讀紙箱條碼關箱] = 1 AND [F]. OrderType=01(一般出貨)
            Message = "請刷讀紙箱條碼關箱";
          else if (DetailData.All(x => x.DiffQty == 0) && CloseByBoxno == "0" || WmsOrdData.OrderType == "02")// (f-4)如果[G].DiffQty=0 AND(工作站設定設為[需刷讀紙箱條碼關箱]= 0   OR[F]. OrderType= 02(廠退出貨))
            Message = "請按下<手動關箱> 按鈕進行關箱";
          MessageForeground = Brushes.Blue;
          break;
      }
    }
    #endregion

    #region 變更物流中心畫面設定
    /// <summary>
    /// 變更物流中心畫面設定
    /// </summary>
    public void DcChangeSetValue()
    {
      // 組印表機資訊
      if (SelectedF910501 != null)
      {
        BindingDeviceSetting();
        ChangeInitMode();
      }
    }

    public void BindingDeviceSetting()
    {
      List<string> printDesc = new List<string>();

      // 印表機1
      if (!string.IsNullOrWhiteSpace(SelectedF910501.PRINTER))
        printDesc.Add(string.Format(Properties.Resources.P0814010000_PrintDesc_1, SelectedF910501.PRINTER));
      // 印表機2
      if (!string.IsNullOrWhiteSpace(SelectedF910501.MATRIX_PRINTER))
        printDesc.Add(string.Format(Properties.Resources.P0814010000_PrintDesc_2, SelectedF910501.MATRIX_PRINTER));
      // 印表機3
      if (!string.IsNullOrWhiteSpace(SelectedF910501.LABELING))
        printDesc.Add(string.Format(Properties.Resources.P0814010000_PrintDesc_3, SelectedF910501.LABELING));
      PrintDesc = printDesc.Any() ? string.Join("\r\n", printDesc) : string.Empty;

      // 工作站編號
      WorkStationCode = SelectedF910501.WORKSTATION_CODE;
      NoSpecReport = SelectedF910501.NO_SPEC_REPROTS == "1" ? "1" : "0";
      CloseByBoxno = SelectedF910501.CLOSE_BY_BOXNO == "1" ? "1" : "0";

      // 工作站狀態
      if (!string.IsNullOrWhiteSpace(WorkStationCode))
      {
        var proxyF19 = GetProxy<F19Entities>();
        _f1946 = proxyF19.F1946s.Where(x => x.DC_CODE == SelectedDc && x.WORKSTATION_CODE == WorkStationCode).FirstOrDefault();

        if (_f1946 != null)
        {
          var f000904 = _workStationStatusList.Where(x => x.Value == _f1946.STATUS).FirstOrDefault();
          WorkStationStatus = f000904 == null ? string.Empty : f000904.Name;
        }
      }
      else
      {
        WorkStationStatus = string.Empty;
      }
    }
    #endregion

    #region 初始畫面控制判斷
    /// <summary>
    /// 初始畫面控制判斷
    /// </summary>
    public void ChangeInitMode()
    {
      if (_f1946 != null && (_f1946.STATUS == "1" || _f1946.STATUS == "2" || _f1946.STATUS == "3"))
      {
        ChangeMode(Mode.InitOpenStation);
        return;
      }
      ChangeMode(Mode.InitCloseStation);
    }
    #endregion

    #endregion

    public CloseBoxParam _closeBoxParam { get; set; }

    #region Command

    #region ExitPacking 離開
    public ICommand ExitPackingCommand
    {
      get
      {
        var isClose = false;
        return CreateBusyAsyncCommand(
            o => isClose = DoExitPacking(),
            () => true,
            o => DoExitPackingComplete(isClose)
        );
      }
    }
    private bool DoExitPacking()
    {
      var proxyF19 = GetProxy<F19Entities>();
      _f1946 = proxyF19.F1946s.Where(x => x.DC_CODE == SelectedDc && x.WORKSTATION_CODE == WorkStationCode).FirstOrDefault();

      // 如果[C]資料不存在或[C].STATUS not in (0, 3)(不是關站、關站中)
      var statusList = new List<string> { "0", "3" };
      if (_f1946 == null || (_f1946 != null && !statusList.Contains(_f1946.STATUS)))
      {
        ShowWarningMessage("您必須到工作站設定進行[關站]，才可以離開包裝站");
        return false;
      }
      else if (ShipMode == "2" && _f1946 != null && _f1946.STATUS == "3" && DetailData.Any(x => x.DiffQty > 0))
      {
        ShowWarningMessage("您尚有未完成單據，請完成此單據包裝後才可以來離開工作站");
        return false;
      }

      // 如果F1946資料存在且F1946.STATUS=3(關站中) 呼叫 包裝站開站/關站紀錄([A],[B],02),1]
      if (_f1946 != null && _f1946.STATUS == "3")
        _shipPackageService.SetPackageStationStatusLog(new wcf.SetPackageStationStatusLogReq
        {
          DcCode = SelectedDc,
          WorkstationCode = _f1946.WORKSTATION_CODE,
          Status = "0",
          WorkType = "02",
          DeviceIp = Wms3plSession.Get<GlobalInfo>().ClientIp,
          CloseByBoxNo = CloseByBoxno,
          NoSpecReport = NoSpecReport
        });

      return true;
    }
    private void DoExitPackingComplete(bool isClose)
    {
      if (isClose)
        OnExitPackingComplete();
    }
    #endregion

    #region PausePacking 暫停包裝
    /// <summary>
    /// 暫停包裝
    /// </summary>
    public ICommand PausePackingCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoPausePacking(),
            () => true,
            o => DoPausePackingComplete()
        );
      }
    }
    private void DoPausePacking()
    {
      _shipPackageService.LogPausePacking(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo);
      InitialData();
      ChangeInitMode();
    }
    private void DoPausePackingComplete()
    {
    }
    #endregion

    #region WorkStationSetting 工作站設定
    /// <summary>
    /// 工作站設定
    /// </summary>
    public ICommand WorkStationSettingCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoWorkStationSetting(),
            () => true,
            o => DoWorkStationSettingComplete()
        );
      }
    }
    private void DoWorkStationSetting()
    {
    }
    private void DoWorkStationSettingComplete()
    {
    }
    #endregion

    #region Reprint 補印
    /// <summary>
    /// 補印
    /// </summary>
    public ICommand ReprintCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoReprint(),
            () => true,
            o => DoReprintComplete()
        );
      }
    }
    private void DoReprint()
    {
    }
    private void DoReprintComplete()
    {
    }
    #endregion

    #region StartPacking 開始包裝
    public ICommand StartPackingCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoStartPacking(),
            () => true,
            o => DoStartPackingComplete()
        );
      }
    }
    private void DoStartPacking()
    {
      DoSearchContainer();
    }
    private void DoStartPackingComplete()
    {
      if (SerialReadingLog.Any())
        OnScrollIntoSerialReadingLog();
    }
    #endregion

    #region EndPacking 包裝完成
    public ICommand EndPackingCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => { },
            completed: o => DoEndPackingComplete(),
            preAction: () => DoEndPacking()
        );
      }
    }

    private void DoEndPacking()
    {
      if (ShipMode == "1" && WmsOrdData.IsPackCheck == "否" && WmsOrdData.BoxCnt > 1)// 是否過刷=否 && 如果[[SS].BoxCnt > 1 and 此功能為單人包裝站，
      {
        //開啟[1.2.23容器刷讀UI]( [SS].DcCode,[SS].GupCode,[SS].CustCode,[SS].WmsOrdNo,容器條碼輸入框值)
        OpenContainerWindow();

        if (ContainerRes == false)// a.如果回傳結果false
        {
          //   (a-1) 顯示訊息[尚有出貨單容器未刷讀，不可包裝完成，若容器抵達要刷讀容器，請在按下 < 包裝完成 > 按鈕]。
          //   (a-2) 商品條碼focus
          Message = "尚有出貨單容器未刷讀，不可包裝完成，若容器抵達要刷讀容器，請在按下 < 包裝完成 > 按鈕";
          MessageForeground = Brushes.Red;
          FocusSelectAllSerial();// 游標Focus商品條碼
        }
      }

    }

    public virtual void DoEndPackingComplete()
    {
      if (WmsOrdData.IsPackCheck == "否")// 是否過刷=否
      {
        //(1) 如果[[SS].BoxCnt > 1 and 此功能為單人包裝站，
        if (WmsOrdData.BoxCnt > 1 && ContainerRes == false)
          return;

        _shipPackageService.InsertF050305Data(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo, "6", "1", WorkStationCode);

        // [A]=呼叫[1.1.7 使用出貨單容器資料產生箱明細]([SS].DcCode,[SS].GupCode,[SS].CustCode,[SS].WmsOrdNo)
        var res = _shipPackageService.UseShipContainerToBoxDetail(new UseShipContainerToBoxDetailReq
        {
          DcCode = SelectedDc,
          GupCode = _gupCode,
          CustCode = _custCode,
          WmsOrdNo = WmsOrdData.WmsOrdNo,
          DelvDate = WmsOrdData.DelvDate,
          PickTime = WmsOrdData.PickTime,
          SubBoxNo = WmsOrdData.SugBoxNo,
          ShipMode = ShipMode,
          WorkstationId = WorkStationCode
        });

        // 如果[A].IsSuccessed = false
        if (!res.IsSuccessed)
        {
          Message = res.Message;
          MessageForeground = Brushes.Red;
          FocusSelectAllSerial();// 游標Focus商品條碼
        }
        else// 如果[A].IsSuccessed = true
        {
          //  a. [B]=呼叫[1.1.3 查詢出貨商品包裝明細]
          DetailData = _shipPackageService.SearchWmsOrderPackingDetail(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo);

          if (DetailData.Any(x => x.DiffQty > 0)) // 如果[B].DiffQty 有一筆 > 0
          {
            var msg = "";
            if (ShipMode == "1")
              msg = "本出貨單需要的商品未全部到齊，無法出貨，請逐筆過刷商品。(系統自動進行取消包裝)";
            else
              msg = "本出貨單需要的商品未全部到齊，無法出貨，請將商品放入最後一個周轉箱中，並送至異常區處理。(系統自動進行取消包裝)";
            Message = msg;
            MessageForeground = Brushes.Red;
            if (ShowWarningMessage(msg) == UILib.Services.DialogResponse.OK)
            {
              // (c - 3)當人員按下確定後
              // (c - 3 - 1)[C] = 呼叫[1.1.8 取消包裝]([SS].DcCode,[SS].GupCode,[SS].CustCode,[SS].WmsOrdNo)
              var cancelRes = _shipPackageService.CancelShipOrder(new CancelShipOrderReq
              {
                DcCode = WmsOrdData.DcCode,
                GupCode = WmsOrdData.GupCode,
                CustCode = WmsOrdData.CustCode,
                WmsOrdNo = WmsOrdData.WmsOrdNo
              });

              if (!cancelRes.IsSuccessed)// (c-3-2)如果[C].IsSuccessed=false
              {
                // (c-3-2-1) [訊息]=C.Message =>紅字
                // (c-3-2-2)彈出視窗顯示[C.Message]
                Message = cancelRes.Message;
                MessageForeground = Brushes.Red;
              }
              else // (c-3-3)如果[C].IsSuccessed = true
              {
                // (c-3-3-1)清除所有出貨資訊，回到可刷讀容器條碼畫面
                // (c-3-3-2)容器條碼Focus
                InitialData();
                ChangeInitMode();
              }
            }
          }
          else if (DetailData.All(x => x.DiffQty == 0)) // 如果所有[B].DiffQty=0
          {
            //  (d-1) 如果是需要刷讀紙箱，訊息顯示[請刷讀紙箱進行關箱]
            if (CloseByBoxno == "1")
            {
              Message = "請刷讀紙箱進行關箱";
              MessageForeground = Brushes.Red;
            }
            else
            {
              // (d-1) 呼叫[1.2.12 手動關箱/系統自動關箱]([A].PackageBoxNo)
              _closeBoxParam = new CloseBoxParam { PackageBoxNo = res.PackageBoxNo };
              CloseBoxCommand.Execute(_closeBoxParam);
            }
          }
        }
      }
      else// 是否過刷=是
      {
        //(1) 如果[B].DiffQty 有一筆 > 0
        //     a. [訊息] = 尚有商品未完成刷讀，不可包裝完成，請刷讀商品條碼
        //     b.彈出視窗[尚有商品未完成刷讀，不可包裝完成，請刷讀商品條碼]，
        //       按下確定後，商品條碼Focus
        if (DetailData.Any(x => x.DiffQty > 0))
        {
          var msg = "尚有商品未完成刷讀，不可包裝完成，請刷讀商品條碼";
          Message = msg;
          MessageForeground = Brushes.Red;
          if (ShowWarningMessage(msg) == UILib.Services.DialogResponse.OK)
            FocusSelectAllSerial();// 游標Focus商品條碼
        }
        else if (DetailData.All(x => x.DiffQty == 0))//(2) 如果所有[B].DiffQty = 0 呼叫[1.2.12 手動關箱 / 系統自動關箱](null)
        {
          if (CloseByBoxno == "1")
          {
            Message = "請刷讀紙箱進行關箱";
            MessageForeground = Brushes.Red;
          }
          else
          {
            _closeBoxParam = new CloseBoxParam();
            CloseBoxCommand.Execute(_closeBoxParam);
          }
        }
      }
    }
    #endregion

    #region CancelPacking 取消包裝
    /// <summary>
    /// 取消包裝
    /// </summary>
    public ICommand CancelPackingCommand
    {
      get
      {
        string action = string.Empty;
        return CreateBusyAsyncCommand(
            o => action = DoCancelPacking(),
            () => true,
            o => DoCancelPackingComplete(action)
        );
      }
    }
    public virtual string DoCancelPacking()
    {
      if (ShowConfirmMessage(String.Format(Properties.Resources.P0807010000_CancelPackingConfirmMsg, Environment.NewLine)) != UILib.Services.DialogResponse.Yes)
      {
        if (WmsOrdData.Status == 0)
          FocusSelectAllSerial();// 游標Focus商品條碼
        else
          FocusSelectAllContainer();// 游標Focus容器條碼
      }
      else
      {
        //(1)[B] = 呼叫[1.1.8 取消包裝]([SS].DcCode,[SS].GupCode,[SS].CustCode,[SS].WmsOrdNo)
        var cancelRes = _shipPackageService.CancelShipOrder(new CancelShipOrderReq
        {
          DcCode = WmsOrdData.DcCode,
          GupCode = WmsOrdData.GupCode,
          CustCode = WmsOrdData.CustCode,
          WmsOrdNo = WmsOrdData.WmsOrdNo
        });

        if (!cancelRes.IsSuccessed)
        {
          //a.[訊息]=[B].Message
          //b.跳出視窗顯示[B].Message
          //c.更新前端出貨單資訊Status=[B].Status
          Message = cancelRes.Message;
          MessageForeground = Brushes.Red;
          ShowWarningMessage(cancelRes.Message);
          WmsOrdData.Status = cancelRes.Status;
        }
        else
        {
          //a.容器條碼=[SS].WmsOrdNo
          //b.清除所有出貨資訊，回到可刷讀容器條碼畫面
          //c.容器條碼Focus
          ContainerCode = WmsOrdData.WmsOrdNo;
          InitialData();
          ChangeInitMode();
        }
      }
      return null;
    }

    public virtual void DoCancelPackingComplete(string action)
    {
    }
    #endregion

    #region CloseBox 手動關箱
    /// <summary>
    /// 手動關箱
    /// </summary>
    public ICommand CloseBoxCommand
    {
      get
      {
        CloseShipBoxRes closeRes = null;
        return CreateBusyAsyncCommand(
            o =>
            {
              var param = (CloseBoxParam)o;

              if (param == null)
              {
                if (ShowConfirmMessage("您確定要手動關箱?") == UILib.Services.DialogResponse.Yes)
                  closeRes = DoCloseBox(param?.PackageBoxNo, param?.ItemCode, param?.IsIsAppendBox, true);
                else
                  FocusSelectAllSerial();// 游標Focus商品條碼
              }
              else
              {
                closeRes = DoCloseBox(param.PackageBoxNo, param.ItemCode, param.IsIsAppendBox);
              }
            },
            () => true,
            o =>
            {
              if (closeRes != null)
                DoCloseBoxComplete(closeRes);
            }
        );
      }
    }

    private CloseShipBoxRes DoCloseBox(int? packageBoxNo = null, string itemCode = null, bool? isIsAppendBox = false, bool isManualCloseBox = false)
    {
      LogHelper.Log(FunctionCode, "關箱 開始 出貨單號" + WmsOrdData.WmsOrdNo + "箱序" + (packageBoxNo.HasValue ? packageBoxNo.Value.ToString() : "無指定箱序關箱"));
      // 呼叫[1.1.6 關箱處理] ([A],[B],[C],[D].WmsOrdNo,<參數1>,[D].OrderType,[E], [F],[D].SugBoxNo)
      var closeRes = _shipPackageService.CloseShipBox(new CloseShipBoxReq
      {
        DcCode = SelectedDc,
        GupCode = _gupCode,
        CustCode = _custCode,
        WmsOrdNo = WmsOrdData.WmsOrdNo,
        PackageBoxNo = packageBoxNo,
        OrdType = WmsOrdData.OrderType,
        PackageMode = NoSpecReport == "1" ? "02" : "01",
        IsScanBox = CloseByBoxno,
        SugBoxNo = WmsOrdData.SugBoxNo,
        ShipMode = ShipMode,
        WorkStationId = SelectedF910501.WORKSTATION_CODE,
        IsAppendBox = Convert.ToBoolean(isIsAppendBox),
        IsManualCloseBox = isManualCloseBox
      });
      LogHelper.Log(FunctionCode, "關箱 結束 出貨單號" + WmsOrdData.WmsOrdNo + "箱序" + (packageBoxNo.HasValue ? packageBoxNo.Value.ToString() : "無指定箱序關箱"));

      //  (2)[G] = 呼叫[1.1.3 查詢出貨商品包裝明細]
      DetailData = _shipPackageService.SearchWmsOrderPackingDetail(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo);
      LogHelper.Log(FunctionCode, "查詢出貨商品包裝明細 結束 出貨單號" + WmsOrdData.WmsOrdNo + "箱序" + (packageBoxNo.HasValue ? packageBoxNo.Value.ToString() : "無指定箱序關箱"));
      // [G]與商品明細Grid繫結，如果[F].ItemCode有值，選取該商品Row
      if (!string.IsNullOrWhiteSpace(itemCode))
      {
        SelectedDetailData = DetailData.Where(x => x.ItemCode == itemCode).FirstOrDefault();
        SelectItem = SelectedDetailData;
      }

      if (DetailData.Any())
        OnScrollIntoDetailData();

      OrdItemCnt = DetailData.Sum(x => x.ShipQty);

      ChangeMode(Mode.Packing);

      //  (3)訊息 =[H].Message
      Message = closeRes.Message;

      if (!closeRes.IsSuccessed)
      {
        MessageForeground = Brushes.Red;
        ShowWarningMessage(closeRes.Message);
      }
      else
      {
        MessageForeground = Brushes.Blue;
      }

      return closeRes;
    }

    public virtual void DoCloseBoxComplete(CloseShipBoxRes closeRes)
    {
      if (closeRes.IsSuccessed)
      {
        //  (1) 呼叫[1.2.14 列印報表]([H].ReportList)
        if (closeRes.ReportList.Any())
        {
          LogHelper.Log(FunctionCode, "列印報表 開始 出貨單號" + WmsOrdData.WmsOrdNo);

          IsBusy = true;
          var result = _shipPackageService.PrintShipPackage(SelectedDc, _gupCode, _custCode, closeRes.ReportList.ToList(), SelectedF910501, WmsOrdData.WmsOrdNo);

          if (!result.IsSuccessed)
          {
            ShowWarningMessage(result.Message);
          }

          IsBusy = false;
          LogHelper.Log(FunctionCode, "列印報表 結束 出貨單號" + WmsOrdData.WmsOrdNo);
        }
        if (DetailData.All(x => x.DiffQty == 0))//a.如果所有[G].DiffQty = 0(所有商品都沒差異代表整張單包裝完成)
        {

          if (closeRes.LastPackageBoxNo == null || closeRes.LastPackageBoxNo == 0)//  (a - 2)如果[H].LastPackageBoxNo無值
          {
            _shipPackageService.InsertF050305Data(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo, "3", "0", WorkStationCode);
            LogHelper.Log(FunctionCode, "寫入包裝完成 結束 出貨單號" + WmsOrdData.WmsOrdNo);
            // 清除所有出貨資訊，回到可刷讀容器條碼畫面
            // 容器條碼Focus
            //InitialData();
            ChangeMode(Mode.OrderPackingFinished);
          }
          else// 如果[H].LastPackageBoxNo有值
          {
            // 呼叫[1.2.12 手動關箱 / 系統自動關箱]([H].LastPackageBoxNo)
            _closeBoxParam = new CloseBoxParam { PackageBoxNo = closeRes.LastPackageBoxNo };
            CloseBoxCommand.Execute(_closeBoxParam);

          }
        }
        else//b.如果所有[G].DiffQty != 0(還有差異)
        {
          FocusSelectAllSerial();// 游標Focus商品條碼
        }

      }
    }
    #endregion

    #region SearchTicket 依容器條碼/出貨單號 查詢單號

    #region Public Command

    public ICommand SearchContainerCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoSearchContainer(),
            () => true,
            o => DoSearchContainerComplete()
        );
      }
    }

    #endregion

    #region Private Method

    protected void DoSearchContainer()
    {
      LogHelper.Log(FunctionCode, "查詢容器開始");
      // 呼叫[1.1.1 出貨容器條碼檢核]([A],[B],[C],[D],1)
      var chkRes = _shipPackageService.CheckShipContainerCode(new wcf.CheckShipContainerCodeReq
      {
        DcCode = SelectedDc,
        GupCode = _gupCode,
        CustCode = _custCode,
        ContainerCode = ContainerCode,
        ShipMode = ShipMode
      });

      //7.如果[E].Result.IsSuccessed = false
      if (!chkRes.Result.IsSuccessed)
      {
        InitialData();
        ChangeInitMode();

        // 訊息 = [E].Result.Message 紅字
        Message = chkRes.Result.Message;
        MessageForeground = Brushes.Red;
        return;
      }
      else
      {
        Serial = string.Empty;
        PackQty = "1";
      }

      LogHelper.Log(FunctionCode, "查詢容器結束");


      if (!chkRes.IsSpecialOrder)//8.如果[E]. Result.IsSuccessed = true AND[E]. IsSpecialOrder=0 (非特殊結構訂單)
      {
        //  (1) [F] = 呼叫[1.1.2 查詢與檢核出貨單資訊]
        WmsOrdData = _shipPackageService.SearchAndCheckWmsOrderInfo(new wcf.SearchAndCheckWmsOrderInfoReq
        {
          DcCode = SelectedDc,
          GupCode = _gupCode,
          CustCode = _custCode,
          WmsOrdNo = chkRes.WmsNo,
          WorkStationId = WorkStationCode,
          ShipMode = ShipMode,
          NoSpecReprots = NoSpecReport,
          CloseByBoxno = CloseByBoxno
        });

        LogHelper.Log(FunctionCode, "查詢與檢核出貨單資訊結束，出貨單號:" + WmsOrdData.WmsOrdNo);

        //(2) [G]= 呼叫[1.1.3 查詢出貨商品包裝明細]
        DetailData = _shipPackageService.SearchWmsOrderPackingDetail(SelectedDc, _gupCode, _custCode, chkRes.WmsNo);
        LogHelper.Log(FunctionCode, "查詢出貨商品包裝明細結束");
        //  [G] 與商品明細Grid繫結，不選取資料，統計出貨商品總數:SUM([G].ShipQty)
        OrdItemCnt = DetailData.Sum(x => x.ShipQty);

        // 開啟出貨單商品明細、刷讀紀錄
        EnableReadSerial = true;

        //  [H]= 呼叫[1.1.4 查詢出貨單刷讀紀錄]
        SerialReadingLog = _shipPackageService.SearchWmsOrderScanLog(SelectedDc, _gupCode, _custCode, chkRes.WmsNo).ToObservableCollection();
        LogHelper.Log(FunctionCode, "查詢出貨單刷讀紀錄結束");

        //  [H] 與刷讀紀錄Grid繫結，Scrollbar Grid到最後一筆並選取
        if (SerialReadingLog.Any())
          SelectedSerialReading = SerialReadingLog.Last();

        if (!WmsOrdData.Result.IsSuccessed)
        {
          // a.訊息=[F].Message 紅字
          // b.容器條碼focus
          Message = WmsOrdData.Result.Message;
          MessageForeground = Brushes.Red;

          if (WmsOrdData.Status == 2)
          {
            ChangeMode(Mode.OrderPackingFinished);
            return;
          }
          else if (WmsOrdData.Status == 5 || WmsOrdData.Status == 6 || WmsOrdData.Status == 9)
          {
            ChangeMode(Mode.OrderShipedAndCanceled);
            return;
          }
        }
        else
        {
          if (ShipMode == "1")
          {
            if (DetailData.Any(x => x.IsOriItem == "1"))
            {
              if (ShowConfirmMessage("出貨明細中含有原箱商品，請確認是否繼續包裝?") == UILib.Services.DialogResponse.Yes)
                ChangeMode(Mode.Packing);
              else
                DoPausePacking();
            }
            else
              ChangeMode(Mode.Packing);
          }
          else
            ChangeMode(Mode.Packing);
          return;
        }
      }
      else //9. 如果[E]. Result.IsSuccessed = true AND[E]. IsSpecialOrder=1 (特殊結構訂單)
      {
        //  開啟[特殊結構包裝UI] (等待設計中)
      }

    }

    protected void DoSearchContainerComplete()
    {
      if (SerialReadingLog.Any())
        OnScrollIntoSerialReadingLog();

      if (DetailData.Any())
        OnScrollIntoDetailData();

      LogHelper.Log(FunctionCode, "查詢容器 結束");

    }
    #endregion

    #endregion

    #region Scan 刷讀商品條碼

    #region Public Command

    public ICommand ScanBarcodeCommand
    {
      get
      {
        bool isCloseBox = false;
        return CreateBusyAsyncCommand(
            o =>
            {
              isCloseBox = ScanBarcode(1, Convert.ToString(o));
            },
            () => true,
            o => ScanBarcodeComplete(isCloseBox)
        );
      }
    }

    #endregion

    #region Private Method

    private bool ScanBarcode(int packQty, string action)
    {
      LogHelper.Log(FunctionCode, "刷讀商品 開始 出貨單號" + WmsOrdData.WmsOrdNo);

      var isCarton = WmsOrdData.CartonItemList.Select(x => x.ItemCode).Contains(Serial);

      var msgRes = UILib.Services.DialogResponse.No;

      //如果刷入的是紙箱條碼 AND 需要刷讀紙箱條碼關箱 = false，
      if (isCarton && CloseByBoxno != "1")
      {
        msgRes = ShowConfirmMessage("您設定為不需要刷讀紙箱關箱，你是否要手動刷讀紙箱關箱？");
        //conifrm訊息”您設定為不需要刷讀紙箱關箱，你是否要手動刷讀紙箱關箱 ?”
        if (msgRes != UILib.Services.DialogResponse.Yes)
        {
          //如果選擇”是” : 就讓它刷讀紙箱關箱往下走， 如果否就返回
          FocusSelectAllSerial();// 游標Focus商品條碼
          return false;
        }
      }

      //(2)	功能為包裝線包裝站[ShipMode=2]，人員刷入的商品非紙箱[isCarton=false]
      //  A.	設定是否人員按下加箱[A]=false
      if (ShipMode == "2" && !isCarton)
        IsUserAppendBox = false;

      //(1)	功能為包裝線包裝站[ShipMode=2]，人員刷入的商品為紙箱[isCarton=true]
      //  A.	人員未按下加箱[A]=false
      //  B.	需刷讀紙箱關箱[CloseByBoxno]=1 或人員決定要刷讀紙箱關箱
      //  C.	還有商品未完成刷讀[任一筆明細DetailData.DiffQty>0]
      //  D.	符合以上條件，回傳訊息[尚有商品未完成刷讀，請先按下[加箱]按鈕後，在刷讀紙箱條碼]
      if (ShipMode == "2" && isCarton 
        && !IsUserAppendBox 
        && (CloseByBoxno == "1" || msgRes == UILib.Services.DialogResponse.Yes) 
        && DetailData.Any(x => x.DiffQty > 0))
      {
        Message = "尚有商品未完成刷讀，請先按下[加箱]按鈕後，再刷讀紙箱條碼";
        MessageForeground = Brushes.Red;
        return false;
      }


      // 如果[D]. IsPackCheck = 否 (不須商品過刷) && [E]的條碼不是紙箱條碼
      if (WmsOrdData.IsPackCheck == "否" && !isCarton)
      {
        //(1)[A] = 跳出Confirm，訊息為[加箱後所有商品都需過刷，您確定要加箱?]
        if (ShowConfirmMessage("此單不需進行商品過刷，您確定要變更為所有商品過刷嗎？") != UILib.Services.DialogResponse.Yes)
        {
          // 如果[A]=否，商品條碼Focus
          FocusSelectAllSerial();// 游標Focus商品條碼
          return false;
        }
        else
        {
          LogHelper.Log(FunctionCode, "變更出貨單為所有商品都需過刷 開始");
          // a.[B] = 呼叫[1.1.16 變更出貨單為所有商品都需過刷]
          var changeRes = _shipPackageService.ChangeShipPackCheck(new ChangeShipPackCheckReq
          {
            DcCode = WmsOrdData.DcCode,
            GupCode = WmsOrdData.GupCode,
            CustCode = WmsOrdData.CustCode,
            WmsOrdNo = WmsOrdData.WmsOrdNo
          });

          if (!changeRes.IsSuccessed)
          {
            //b.[B].IsSuccessed = false，[訊息]=[B].Message，商品條碼focus
            Message = changeRes.Message;
            MessageForeground = Brushes.Red;
            FocusSelectAllSerial();// 游標Focus商品條碼
            return false;
          }
          else//c.[B].IsSuccessed = true，
          {
            //(c-1) [SS].IsPackCheck = 是
            WmsOrdData.IsPackCheck = "是";

            //(c-2) [訊息]= 所有商品需要過刷，請刷讀商品條碼 訊息顯示藍字
            Message = "所有商品需要過刷，請刷讀商品條碼";
            MessageForeground = Brushes.Blue;
          }
          LogHelper.Log(FunctionCode, "變更出貨單為所有商品都需過刷 結束");
        }
      }

						LogHelper.Log(FunctionCode, "刷讀商品條碼 開始");
            //6.[F] = 呼叫[1.1.5 刷讀商品條碼]([A],[B],[C],[D].WmsOrdNo,[D].DelvDate,[D].PickTime,
            //[E],1,[D].OrderType,[D].ItemList,[D].CartonItemList)
            var scanRes = _shipPackageService.ScanItemBarcode(new wcf.ScanItemBarcodeReq
            {
                DcCode = SelectedDc,
                GupCode = _gupCode,
                CustCode = _custCode,
                WmsOrdNo = WmsOrdData.WmsOrdNo,
                DelvDate = WmsOrdData.DelvDate,
                PickTime = WmsOrdData.PickTime,
                BarCode = Serial,
                Qty = packQty,
                OrdType = WmsOrdData.OrderType,
                ItemList = WmsOrdData.ItemList,
                BoxItemList = WmsOrdData.CartonItemList,
                Action = action,
                NoSpecReprots = NoSpecReport,
                WorkstationId = WorkStationCode
            });

      //7.如果[F].IsSuccessed = false
      if (!scanRes.IsSuccessed)
      {
        //  (1) 商品條碼Focus
        //  (2) 訊息=[F].Message
        FocusSelectAllSerial();// 游標Focus商品條碼
        Message = scanRes.Message;
        MessageForeground = Brushes.Red;
      }
      LogHelper.Log(FunctionCode, "刷讀商品條碼 結束");
      // 開啟出貨單商品明細、刷讀紀錄
      EnableReadSerial = true;
      LogHelper.Log(FunctionCode, "查詢出貨單刷讀紀錄 開始");
      //8. [H]= 呼叫[1.1.4 查詢出貨單刷讀紀錄]
      //  (1) [H] 與刷讀紀錄Grid繫結，Scrollbar Grid到最後一筆並選取
      SerialReadingLog = _shipPackageService.SearchWmsOrderScanLog(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo).ToObservableCollection();
      LogHelper.Log(FunctionCode, "查詢出貨單刷讀紀錄 結束");
      //  [H] 與刷讀紀錄Grid繫結，Scrollbar Grid到最後一筆並選取
      if (SerialReadingLog.Any())
        SelectedSerialReading = SerialReadingLog.Last();

      if (SerialReadingLog.Any())
        OnScrollIntoSerialReadingLog();

      if (scanRes.IsSuccessed)
      {
        //9.如果[F].IsSuccessed = true and[F].IsCloseBox=false(不關箱)
        if (!scanRes.IsCloseBox)
        {
          LogHelper.Log(FunctionCode, "查詢出貨商品包裝明細 開始");
          // (1) [G]= 呼叫[1.1.3 查詢出貨商品包裝明細]
          DetailData = _shipPackageService.SearchWmsOrderPackingDetail(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo);
          LogHelper.Log(FunctionCode, "查詢出貨商品包裝明細 結束");
          // [G]與商品明細Grid繫結，如果[F].ItemCode有值，選取該商品Row
          if (!string.IsNullOrWhiteSpace(scanRes.ItemCode))
          {
            SelectedDetailData = DetailData.Where(x => x.ItemCode == scanRes.ItemCode).FirstOrDefault();
            SelectItem = SelectedDetailData;
          }

          if (DetailData.Any())
            OnScrollIntoDetailData();

          OrdItemCnt = DetailData.Sum(x => x.ShipQty);

          var isFinish = DetailData.All(x => x.DiffQty == 0);

          if (isFinish && CloseByBoxno == "0")// IF [G]所有.DiffQty = 0  AND 是否刷讀紙箱關箱=否
          {
            _closeBoxParam = new CloseBoxParam();
            return true;
          }
          if (isFinish && CloseByBoxno == "1")// IF [G]所有.DiffQty = 0  AND 是否刷讀紙箱關箱=是
          {
            //(2) 商品條碼Focus
            FocusSelectAllSerial();

            //(3) 訊息=請刷讀紙箱條碼關箱
            Message = "請刷讀紙箱條碼關箱";
            MessageForeground = Brushes.Blue;

            ChangeMode(Mode.Packing);
          }
          else
          {
            //(2) 商品條碼Focus
            FocusSelectAllSerial();

            //(3) 訊息=所有商品需要過刷，請刷讀商品條碼
            Message = "所有商品需要過刷，請刷讀商品條碼";
            MessageForeground = Brushes.Blue;

            ChangeMode(Mode.Packing);
          }
        }
        else
        {
          //10.如果[F].IsSuccessed = true and[F].IsCloseBox=true(關箱) 呼叫[1.2.12手動關箱 / 系統自動關箱] ([F].PackageBoxNo)
          _closeBoxParam = new CloseBoxParam { PackageBoxNo = scanRes.PackageBoxNo, ItemCode = scanRes.ItemCode, IsIsAppendBox = IsUserAppendBox };
          LogHelper.Log(FunctionCode, "刷讀商品 結束");

          return true;
        }
      }
      LogHelper.Log(FunctionCode, "刷讀商品 結束 出貨單號" + WmsOrdData.WmsOrdNo);

      return false;
    }

    private void ScanBarcodeComplete(bool isCloseBox)
    {
      if (isCloseBox)
        CloseBoxCommand.Execute(_closeBoxParam);
    }
    #endregion

    #endregion

    #region UpdatePackQty 更新包裝數量

    #region Public Command
    public ICommand UpdatePackQtyCommand
    {
      get
      {
        bool isCloseBox = false;
        return CreateBusyAsyncCommand(
            o => isCloseBox = DoUpdatePackQty(),
            () => true,
            o => DoUpdatePackQtyComplete(isCloseBox)
        );
      }
    }
    #endregion

    #region Private Method
    private bool DoUpdatePackQty()
    {
      // 檢查商品條碼是否有值，若無值，訊息[調整包裝數，商品條碼不可為空白] 
      if (string.IsNullOrWhiteSpace(Serial))
      {
        Message = "調整包裝數，商品條碼不可為空白";
        MessageForeground = Brushes.Red;
        return false;
      }

      // 檢查商品條碼是否為紙箱條碼，若是，訊息[調整包裝數，商品條碼不可為紙箱條碼] 
      if (WmsOrdData.CartonItemList.Select(x => x.ItemCode).Contains(Serial))
      {
        Message = "調整包裝數，商品條碼不可為紙箱條碼";
        MessageForeground = Brushes.Red;
        return false;
      }

      // 檢查包裝數是否有值，若無值，訊息[調整包裝數，包裝數不可為空白，請輸入包裝數] 
      if (string.IsNullOrWhiteSpace(PackQty))
      {
        Message = "調整包裝數，商品條碼不可為紙箱條碼";
        MessageForeground = Brushes.Red;
        return false;
      }

      // 檢查包裝數是否為數值，若非數值，訊息[調整包裝數，包裝數必須是數值]
      if (!rgx.IsMatch(PackQty))
      {
        Message = "調整包裝數，包裝數必須是數值";
        MessageForeground = Brushes.Red;
        return false;
      }

      // 檢查包裝數是否大於0，若小於0，訊息[調整包裝數，包裝數必須大於0] 
      if (Convert.ToInt32(PackQty) <= 0)
      {
        Message = "調整包裝數，包裝數必須大於0";
        MessageForeground = Brushes.Red;
        return false;
      }

      Serial = Serial.ToUpper();

      return ScanBarcode(Convert.ToInt32(PackQty), "02");
    }

    private void DoUpdatePackQtyComplete(bool isCloseBox)
    {
      if (isCloseBox)
        CloseBoxCommand.Execute(_closeBoxParam);
    }
    #endregion

    #endregion

    #region AppendBox 加箱

    #region Public Command
    public ICommand AppendBoxCommand
    {
      get
      {
        bool isCloseBox = false;
        return CreateBusyAsyncCommand(
            o => isCloseBox = DoAppendBox(),
            () => true,
            o => DoAppendBoxComplete(isCloseBox)
        );
      }
    }
    #endregion

    #region Private Method
    private bool DoAppendBox()
    {
      LogHelper.Log(FunctionCode, "加箱 開始 出貨單號" + WmsOrdData.WmsOrdNo);
      if (WmsOrdData.IsPackCheck == "否")//2.如果[SS].IsPackCheck = 否(不須商品過刷)
      {
        //(1)[A] = 跳出Confirm，訊息為[加箱後所有商品都需過刷，您確定要加箱?]
        if (ShowConfirmMessage(Properties.Resources.P0814010000_AppendBoxConfirmMsg) != UILib.Services.DialogResponse.Yes)
        {
          //(2) 如果[A] = 否，商品條碼Focus
          FocusSelectAllSerial();// 游標Focus商品條碼
        }
        else//(3) 如果{ A}= 是，
        {
          LogHelper.Log(FunctionCode, "變更出貨單為所有商品都需過刷 開始 出貨單號" + WmsOrdData.WmsOrdNo);
          // a.[B] = 呼叫[1.1.16 變更出貨單為所有商品都需過刷]
          var changeRes = _shipPackageService.ChangeShipPackCheck(new ChangeShipPackCheckReq
          {
            DcCode = WmsOrdData.DcCode,
            GupCode = WmsOrdData.GupCode,
            CustCode = WmsOrdData.CustCode,
            WmsOrdNo = WmsOrdData.WmsOrdNo
          });

          if (!changeRes.IsSuccessed)
          {
            //b.[B].IsSuccessed = false，[訊息]=[B].Message，商品條碼focus
            Message = changeRes.Message;
            MessageForeground = Brushes.Red;
          }
          else//c.[B].IsSuccessed = true，
          {
            //(c-1) [SS].IsPackCheck = 是
            WmsOrdData.IsPackCheck = "是";

            //(c-2) [訊息]= 所有商品需要過刷，請刷讀商品條碼 訊息顯示藍字
            Message = "所有商品需要過刷，請刷讀商品條碼";
            MessageForeground = Brushes.Blue;
            //(c-3) 商品條碼focus
          }
          FocusSelectAllSerial();// 游標Focus商品條碼
          LogHelper.Log(FunctionCode, "變更出貨單為所有商品都需過刷 結束 出貨單號" + WmsOrdData.WmsOrdNo);

        }
      }
      else//3.如果[SS].IsPackCheck = 是(商品須過刷)
      {
        
        if (ShipMode == "2" && DetailData.Any(x => x.PackageQty > 0))
          IsUserAppendBox = true;


        //(1)[K] = 檢查出貨商品是否有一筆包裝數 > 0
        if (DetailData.Any(x => x.PackageQty > 0))
        {
          //(3)[K]=true 呼叫[1.2.12 手動關箱 / 系統自動關箱] (null)
          _closeBoxParam = new CloseBoxParam { IsIsAppendBox = true };
          return true;
        }
        else//(2)[K] = false
        {
          //   a. [訊息] = 尚未刷讀任何商品，不可加箱 => 紅字   b.商品條碼 focus
          Message = "尚未刷讀任何商品，不可加箱";
          MessageForeground = Brushes.Red;
          FocusSelectAllSerial();// 游標Focus商品條碼
        }
      }

      return false;
    }

    private void DoAppendBoxComplete(bool isCloseBox)
    {
      if (isCloseBox)
        CloseBoxCommand.Execute(_closeBoxParam);
    }
    #endregion

    #endregion

    #endregion

    #region Share Method

    /// <summary>
    /// 初始化設定
    /// </summary>
    public void InitialData()
    {
      UserOperateMode = OperateMode.Query;
      WmsOrdData = null;
      OrdItemCnt = 0;
      DetailData = new List<SearchWmsOrderPackingDetailRes>();
      SerialReadingLog = new ObservableCollection<SearchWmsOrderScanLogRes>();
      Message = string.Empty;
      Serial = string.Empty;
      PackQty = "1";
      FocusSelectAllContainer();// 游標Focus容器條碼
      SelectItem = null;
      if (ShipMode == "2")
        ContainerCode = string.Empty;
    }
    #endregion
  }

  public enum Mode
  {
    /// <summary>
    /// 關站
    /// </summary>
    InitCloseStation,
    /// <summary>
    /// 開站/關站中
    /// </summary>
    InitOpenStation,
    /// <summary>
    /// 出貨單已包裝完成
    /// </summary>
    OrderPackingFinished,
    /// <summary>
    /// 出貨單已出貨
    /// </summary>
    OrderShipedAndCanceled,
    /// <summary>
    /// 包裝中
    /// </summary>
    Packing
  }

  public class CloseBoxParam
  {
    public int? PackageBoxNo { get; set; }
    public string ItemCode { get; set; }
    public bool? IsIsAppendBox { get; set; }
  }
}
