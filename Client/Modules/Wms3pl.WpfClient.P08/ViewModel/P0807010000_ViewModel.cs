using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.VideoServer;
using Timer = System.Timers.Timer;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;
using wcfShared = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P08ExDataService.ExecuteResult;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.DataServices;
using System.Diagnostics;
using System.Data;
using Wms3pl.WpfClient.Common.Helpers;
using System.Configuration;
using System.IO;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0807010000_ViewModel : InputViewModelBase
	{
		#region Private Variable

		private string _staff = null;
		private string _staffName = null;
		private List<NameValuePair<string>> _statusList;
		private List<NameValuePair<string>> _lackStatusList;
		private List<NameValuePair<string>> _lackRtnStatusList;
		private DeliveryReportService _deliveryReport;
		private VideoServerHelper _videoServerHelper = null;
		private Regex rgx = new Regex(@"^[A-Za-z0-9-]+$");

		/// <summary>
		/// 每次刷讀品號/序號/箱號後的結果
		/// </summary>
		private wcf.ScanPackageCodeResult _scanPackageCodeResult = null;

		/// <summary>
		/// 列印任務控制
		/// </summary>
		private Task<ExDataServices.ShareExDataService.ExecuteResult> _printPassTask = null;
		#endregion

		#region Public Variable

		/// <summary>
		/// 查詢單據完成後委派事件
		/// </summary>
		public Action OnSearchTicketComplete = delegate { };
		/// <summary>
		/// 查詢商品/序號完成後委派事件
		/// </summary>
		public Action OnSearchSerialComplete = delegate { };
		/// <summary>
		/// 暫停包裝完成後委派事件
		/// </summary>
		public Action OnPausePackingComplete = delegate { };
		/// <summary>
		/// 開始包裝完成後委派事件
		/// </summary>
		public Action OnStartPackingComplete = delegate { };
		/// <summary>
		/// 離開包裝完成後委派事件
		/// </summary>
		public Action OnExitPackingComplete = delegate { };
		/// <summary>
		/// 顯示主管解鎖委派事件
		/// </summary>
		public Action OnRequireUnlock = delegate { };
		/// <summary>
		/// 物流中心變更後委派事件
		/// </summary>
		public Action OnDcCodeChanged = delegate { };
		/// <summary>
		/// 滾輪移至指定出貨明細委派事件
		/// </summary>
		public Action OnScrollIntoDeliveryData = () => { };
    /// <summary>
    /// 滾輪移至第一筆出貨明細委派事件
    /// </summary>
    public Action OnScrollFirstDeliveryData = () => { };
    /// <summary>
    /// 滾輪移至刷讀紀錄最後一筆委派事件
    /// </summary>
    public Action OnScrollIntoSerialReadingLog = () => { };

    /// <summary>
    /// Device 的設定 (當物流中心改變時，就會去顯示設定 Device 的畫面)  
    /// </summary>
    public F910501 SelectedF910501 { get; set; }


		#endregion

		#region Constructor
		public P0807010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_staff = Wms3plSession.Get<UserInfo>().Account;
				_staffName = Wms3plSession.Get<UserInfo>().AccountName;

				DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
				if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;

				SourceTypeList = SourceTypeService.GetSourceType(FunctionCode);
				_statusList = PackingService.GetStatusList(FunctionCode);

				_deliveryReport = new DeliveryReportService(FunctionCode);
        _deliveryReport.OnReprintClicked += () => OnReprintClicked();
        _videoServerHelper = new VideoServerHelper(); //錄影設備元件

				#region 僅缺貨包裝使用
				if (IsShortStockPackage)
				{
					_lackStatusList = PackingService.GetLackStatusList(FunctionCode);
					_lackRtnStatusList = PackingService.GetLackRtnStatusList(FunctionCode);
				}

				#endregion

				#region NotUsed Code Backup

				StartPickTimeSchedule();
				#endregion
			}
		}
        #endregion

        #region Property

        #region Form - DC/ GUP/ CUST/ 批次日期/ 品號(或序號)

        #region Lock
        private bool _lock;
        /// <summary>
        /// 物流中心清單
        /// </summary>
        public bool Lock
        {
            get { return _lock; }
            set { _lock = value; RaisePropertyChanged("Lock"); }
        }
        #endregion

        #region 物流中心清單
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

		#region 選取的物流中心
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

		#region 批次日期
		private DateTime _date = DateTime.Today;
		public DateTime Date
		{
			get { return _date; }
			set { _date = value; RaisePropertyChanged("Date"); }
		}
		#endregion

		#region 品號(或序號)
		private string _serial = string.Empty;
		public string Serial
		{
			get { return _serial; }
			set
			{
				//if (rgx.IsMatch(value) || string.IsNullOrWhiteSpace(value))
					Set(ref _serial, value);

			}
		}
		#endregion

		#endregion

		#region Form - 單號 (出貨單, 揀貨單, 合流箱號)
		private string _ticketNo = string.Empty;
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Required_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public string TicketNo
		{
			get { return _ticketNo; }
			set { _ticketNo = value; RaisePropertyChanged("TicketNo"); }
		}

		#endregion

		#region Form - 出貨/缺貨包裝判斷
		private bool _isShortStockPackage = false;
		public bool IsShortStockPackage
		{
			get { return _isShortStockPackage; }
			set { _isShortStockPackage = value; RaisePropertyChanged("IsShortStockPackage"); }
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

		#region Form 來源單據清單

		#region 來源單據清單
		private List<NameValuePair<string>> _sourceType;

		public List<NameValuePair<string>> SourceTypeList
		{
			get { return _sourceType; }
			set
			{
				if (_sourceType == value)
					return;
				Set(() => SourceTypeList, ref _sourceType, value);
			}
		}
		#endregion

		#endregion

		#region Form 顯示/隱藏 UI 代收/配送取貨方式 

		private Visibility _hasTakeMoney = Visibility.Collapsed;
		public Visibility hasTakeMoney
		{
			get { return _hasTakeMoney; }
			set { _hasTakeMoney = value; RaisePropertyChanged("hasTakeMoney"); }
		}

		private Visibility _btnVis = Visibility.Collapsed;
		public Visibility btnVis
		{
			get { return _btnVis; }
			set { _btnVis = value; RaisePropertyChanged("btnVis"); }
		}
		private Visibility _hasData = Visibility.Collapsed;
		public Visibility hasData
		{
			get { return _hasData; }
			set { _hasData = value; RaisePropertyChanged("hasData"); }
		}
		private string _billType;
		public string BillType
		{
			get { return _billType; }
			set { _billType = value; RaisePropertyChanged("BillType"); }
		}
		
		private string _sugBoxNo;
		public string SugBoxNo
		{
			get { return _sugBoxNo; }
			set { _sugBoxNo = value; RaisePropertyChanged("SugBoxNo"); }
		}
		#endregion

		#region Form 配送商/託運單號

		//private string _allId;
		//public string AllId
		//{
		//	get { return _allId; }
		//	set { _allId = value; RaisePropertyChanged("AllId"); }
		//}
		private string _consignNo;
		public string consignNo
		{
			get { return _consignNo; }
			set { _consignNo = value; RaisePropertyChanged("consignNo"); }
		}

		#endregion

		#region Form 客戶名稱
		private string _custName = string.Empty;
		public string CustName
		{
			get { return _custName; }
			set { _custName = value; RaisePropertyChanged("CustName"); }
		}
		#endregion

		#region Form 控制UI 上下部啟用/停用狀態

		#region 控制畫面上半部的狀態
		private bool _validTicket = false;
		/// <summary>
		/// 控制畫面上半部的狀態
		/// </summary>
		public bool ValidTicket
		{
			get { return _validTicket; }
			set { _validTicket = value; RaisePropertyChanged("ValidTicket"); }
		}

		#endregion

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

		#region Form 顯示UI目前商品資訊 (目前只有在缺貨包裝使用)

		private F1903 _itemData = null;
		/// <summary>
		/// 顯示UI目前商品資訊
		/// </summary>
		public F1903 ItemData
		{
			get { return _itemData; }
			set { _itemData = value; RaisePropertyChanged("ItemData"); }
		}

		#endregion

		#region Form Grid 出貨商品明細(UI顯示) 出貨商品清單/選取的出貨商品

		private List<DeliveryData> _dlvData = new List<DeliveryData>();
		/// <summary>
		/// 出貨商品清單
		/// </summary>
		public List<DeliveryData> DlvData
		{
			get { return _dlvData; }
			set
			{
				Set(() => DlvData, ref _dlvData, value);
			}
		}
		private DeliveryData _selectedDeliveryData;
		/// <summary>
		/// 選擇的出貨商品
		/// </summary>
		public DeliveryData SelectedDeliveryData
		{
			get { return _selectedDeliveryData; }
			set
			{
				Set(() => SelectedDeliveryData, ref _selectedDeliveryData, value);
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

		private SearchWmsOrderScanLogRes _selectedSerialReadingStatus;
		/// <summary>
		/// 目前選擇的序號刷讀紀錄
		/// </summary>
		public SearchWmsOrderScanLogRes SelectedSerialReadingStatus
		{
			get { return _selectedSerialReadingStatus; }
			set
			{
				Set(() => SelectedSerialReadingStatus, ref _selectedSerialReadingStatus, value);
			}
		}

		#endregion

		#region Form 目前輸入的包裝數

		private string _packQty = "1";
		/// <summary>
		/// 包裝數
		/// </summary>
		public string PackQty
		{
			get { return _packQty; }
			set { _packQty = value; RaisePropertyChanged("PackQty"); }
		}

		#endregion

		#region Form SA

		private string _sa = "SA";
		public string SA
		{
			get { return _sa; }
			set { _sa = value; RaisePropertyChanged("SA"); }
		}

		#endregion

		#region Form 建議紙箱品名
		private string _boxType = string.Empty;
		public string BoxType
		{
			get { return _boxType; }
			set { _boxType = value; RaisePropertyChanged("BoxType"); RaisePropertyChanged("IsBoxTypeSet"); }
		}

		#endregion

		#region Form 是否顯示建議紙箱

		public string IsBoxTypeSet
		{
			get { return string.IsNullOrWhiteSpace(_boxType) ? "0" : "1"; }
		}

		#endregion

		#region Form - 商品圖檔

		private string _fileName = string.Empty;
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}
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
				if (ItemCode == null || string.IsNullOrWhiteSpace(ItemCode)) _itemImageSource = null;
				else
				{
					_itemImageSource = FileService.GetItemImage(_gupCode, _custCode, ItemCode);
				}
				RaisePropertyChanged("ItemImageSource");
			}
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
			set { _messageForeground = value; RaisePropertyChanged("MessageForeground"); }
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
				PlaySound();
				RaisePropertyChanged("Message");
			}
		}
		#endregion

		#region Form - 列印項目的明細, 有新的報表就加在這, 會自動顯示在右下角的列印區塊
		private ObservableCollection<DynamicButtonData> _reportList = new ObservableCollection<DynamicButtonData>();
		public ObservableCollection<DynamicButtonData> ReportList
		{
			get { return _reportList; }
			set { _reportList = value; RaisePropertyChanged("ReportList"); }
		}
		#endregion

		#region 存放每次刷讀單號後，不會變的固定資料，利用快取加快作業

		#region 貨主主檔
		private F1909 _f1909Data = null;
		/// <summary>
		/// 貨主
		/// </summary>
		public F1909 F1909Data
		{
			get { return _f1909Data; }
			set { _f1909Data = value; RaisePropertyChanged("F1909Data"); }
		}

		#endregion

		#region 門市資料

		private F1910 _f1910Data = null;
		/// <summary>
		/// 門市資料
		/// </summary>
		public F1910 F1910Data
		{
			get { return _f1910Data; }
			set { _f1910Data = value; RaisePropertyChanged("F1910Data"); }
		}

		#endregion

		#region 配送商資料
		/// <summary>
		/// 刷讀單號後，先取得配送商，在列印託運單時，就能加快作業
		/// </summary>
		public F1947 F1947Data { get; set; }
		#endregion

		#region 貨主指定配送商設定檔
		private F194704 _f194704Data = null;
		/// <summary>
		/// 貨主指定配送商設定檔
		/// </summary>
		public F194704 F194704Data
		{
			get { return _f194704Data; }
			set { _f194704Data = value; RaisePropertyChanged("F194704Data"); }
		}

		#endregion

		#region 出貨單對應多個併單的訂單主檔

		/// <summary>
		/// 多個併單的訂單主檔
		/// </summary>
		List<F050301> _f050301s = null;

		public List<F050301> F050301s
		{
			get { return _f050301s; }
			set
			{
				Set(() => F050301s, ref _f050301s, value);
			}
		}

		#endregion

		#region Form 貨主單號清單

		private List<string> _custOrdNos;

		public List<string> CustOrdNos
		{
			get { return _custOrdNos; }
			set
			{
				Set(() => CustOrdNos, ref _custOrdNos, value);
			}
		}

		#endregion

		#region Form Grid 顯示品名資料來源

		private List<NameValuePair<string>> _itemNameList;

		public List<NameValuePair<string>> ItemNameList
		{
			get { return _itemNameList; }
			set
			{
				Set(() => ItemNameList, ref _itemNameList, value);
			}
		}

		#endregion

		#region 商品主檔清單
		private List<F1903> _f1903s;

		public List<F1903> F1903s
		{
			get { return _f1903s; }
			set
			{
				Set(() => F1903s, ref _f1903s, value);
			}
		}

		#endregion

		#region 出貨單資料
		private F050801 _f050801Data = null;
		public F050801 F050801Data
		{
			get { return _f050801Data; }
			set
			{
				_f050801Data = value;
				RaisePropertyChanged("F050801Data");
			}
		}

		#endregion

		#region 出貨單明細
		private List<F050802> _f050802Data = new List<F050802>();
		public List<F050802> F050802Data
		{
			get { return _f050802Data; }
			set { _f050802Data = value; RaisePropertyChanged("F050802Data"); }
		}

		#endregion

		#region 合流箱資料(合流作業使用)
		private F052901 _f052901Data = null;
		public F052901 F052901Data
		{
			get { return _f052901Data; }
			set { _f052901Data = value; RaisePropertyChanged("F052901Data"); }
		}

		#endregion


		#endregion

		#region 目前此箱出貨包裝頭檔

		private F055001 _f055001Data = null;
		/// <summary>
		/// 開始包裝時的主資料 - 出貨包裝頭檔
		/// Memo: 按下開始包裝, 或是刷讀箱號後寫入
		/// </summary>
		public F055001 F055001Data
		{
			get { return _f055001Data; }
			set { _f055001Data = value; RaisePropertyChanged("F055001Data"); }
		}

		#endregion

		#region 目前刷讀的品號

		private string _itemCode = string.Empty;
		/// <summary>
		/// 品號. 刷讀後如果是品號就記錄在此.
		/// </summary>
		public string ItemCode
		{
			get { return _itemCode; }
			set { _itemCode = value; RaisePropertyChanged("ItemCode"); }
		}

		#endregion

		#region 是否缺貨
		private bool _shortInStock = false;
		public bool ShortInStock
		{
			get { return _shortInStock; }
			set { _shortInStock = value; RaisePropertyChanged("ShortInStock"); }
		}
		#endregion

		#region 是否完成包裝
		/// <summary>
		/// 是否完成包裝
		/// </summary>
		public bool IsCompletePackage
		{
			get
			{
				return DlvData.All(x => x.DiffQty <= 0);
			}
		}

		#endregion

		#region 是否為正常刷讀

		/// <summary>
		/// 是否為正常刷讀
		/// </summary>
		public bool IsNormalScan
		{
			get
			{
				return noPlaySoundMessages.Exists(x => x == Message)
					|| string.IsNullOrWhiteSpace(Message)
					|| IsCompletePackage
					|| (_scanPackageCodeResult != null && _scanPackageCodeResult.IsPass);
			}
		}
		#endregion

		#region 正常刷讀-更改訊息顏色  異常刷讀-播放音效+更改訊息顏色

		private void PlaySound()
		{
			if (IsNormalScan)
			{
				MessageBackground = Brushes.White;
				MessageForeground = Brushes.Blue;
				return;
			}
			else
			{
				MessageBackground = Brushes.Red;
				MessageForeground = Brushes.White;
			}
			if (!string.IsNullOrWhiteSpace(Message) && this.SoundOn) PlaySoundHelper.Oo();
		}

		#endregion

		#region 
		private List<LittleWhiteReport> _littleWhiteReport;
		public List<LittleWhiteReport> LittleWhiteReport
		{
			get { return _littleWhiteReport; }
			set
			{
				_littleWhiteReport = value;
				RaisePropertyChanged("LittleWhiteReport");
			}
		}


        public DataTable LittleWhiteReportDT
        {
            get
            {
                _littleWhiteReport.ForEach(x => x.sourceNoBarcode = BarcodeConverter128.StringToBarcode(x.SOURCE_NO));
                return _littleWhiteReport.ToDataTable();
            }
        }
        #endregion

        #region 客戶自訂分類
        private string _custCost;
        public string CustCost
        {
            get { return _custCost; }
            set { Set(() => CustCost, ref _custCost, value); }
        }
        #endregion

        #region 優先處理旗標
        private string _fastDealType;
        public string FastDealType
        {
            get { return _fastDealType; }
            set { Set(() => FastDealType, ref _fastDealType, value); }
        }
        #endregion

        #endregion

        #region Command

        #region ExitPacking 離開包裝

        #region Public Command
        public ICommand ExitPackingCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoExitPacking(),
					() => UserOperateMode == OperateMode.Query,
					o => DoExitPackingComplete()
				);
			}
		}

		#endregion

		#region Private Method

		private void DoExitPacking()
		{
		}
		private void DoExitPackingComplete()
		{
			OnExitPackingComplete();
		}

		#endregion

		#endregion

		#region PausePacking 暫停包裝

		#region Public Command
		/// <summary>
		/// 暫停包裝
		/// </summary>
		public ICommand PausePackingCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPausePacking(),
					() => (F050801Data != null && (F050801Data.STATUS == 0 || F050801Data.STATUS == 1 || F050801Data.STATUS == 2)),
					o =>
					{
						DoPausePackingComplete();
						SearchTicketCommand.Execute(null);
					}
				);
			}
		}

		#endregion

		#region Private Method
		private void DoPausePacking()
		{
      //寫入暫停包裝刷讀記錄
      var wcfProxy = GetWcfProxy<wcf.P08WcfServiceClient>();
      wcfProxy.RunWcfMethod(w => w.LogPausePacking(F050801Data.DC_CODE, F050801Data.GUP_CODE, F050801Data.CUST_CODE, F050801Data.WMS_ORD_NO));
      UpdateSerialRecords();
      EndVideo();
			UserOperateMode = OperateMode.Query;
			InitialData();
		}
		private void DoPausePackingComplete()
		{
			OnPausePackingComplete();
		}

		#endregion

		#endregion

		#region CancelPacking 取消包裝: 呼叫網家LMS 取消申請宅配單Api->取消包裝更新

		#region Public Command
		/// <summary>
		/// 取消包裝
		/// </summary>
		public ICommand CancelPackingCommand
		{
			get
			{
				bool isCancelOk = false;
				return CreateBusyAsyncCommand(
					o => isCancelOk = DoCancelPacking(),
					() => (F050801Data != null && (F050801Data.STATUS == 1 || F050801Data.STATUS == 2 || (F050801Data.STATUS == 0 && SerialReadingLog != null && SerialReadingLog.Any()))),
					o =>
					{
						if (isCancelOk)
						{
							SearchTicketCommand.Execute(null);
							OnPausePackingComplete();
						}
					}
				);
			}
		}
		#endregion

		#region Private Method

		private bool DoCancelPacking()
    {
      var proxyF05 = GetProxy<F05Entities>();
      if (ShowConfirmMessage(String.Format(Properties.Resources.P0807010000_CancelPackingConfirmMsg, Environment.NewLine)) != UILib.Services.DialogResponse.Yes)
        return false;

      // 檢核出貨包裝頭檔是否已經有回填宅配單，有宅配單才需要打取消宅配單LmsApi
      if (F050801Data != null)
      {
        var hasPastNo = proxyF05.F055001s.Where(x => x.DC_CODE == F050801Data.DC_CODE
                          && x.GUP_CODE == F050801Data.GUP_CODE
                          && x.CUST_CODE == F050801Data.CUST_CODE
                          && x.WMS_ORD_NO == F050801Data.WMS_ORD_NO).ToList().Any(x => !string.IsNullOrWhiteSpace(x.PAST_NO));

        if (hasPastNo)
        {
          var res = CancelConsign();
          if (!res.IsSuccessed)
          {
            ShowWarningMessage(res.Message);
            return false;
          }
        }
      }
      //重新取得資料庫的出貨單資料，避免前端暫存的資料是舊的
      var ReGetF050801 = proxyF05.F050801s.Where(x => x.DC_CODE == F050801Data.DC_CODE && x.GUP_CODE == F050801Data.GUP_CODE && x.CUST_CODE == F050801Data.CUST_CODE && x.WMS_ORD_NO == F050801Data.WMS_ORD_NO).First();
      if (ReGetF050801.STATUS == 5)  //5	單據狀態	已出貨
      {
        ShowWarningMessage("該出貨單已出貨，不可取消包裝");
        return false;
      }
      else if (ReGetF050801.STATUS == 6)  //6	單據狀態	已扣帳
      {
        ShowWarningMessage("該出貨單已經扣帳完成，不可取消包裝");
        return false;
      }
      else if (ReGetF050801.STATUS == 9)
      {
        ShowWarningMessage("該出貨單已經取消，不可取消包裝");
        return false;
      }

      UserOperateMode = OperateMode.Query;
      CancelPackingUpdate();
      return true;
    }

    private void CancelPackingUpdate()
		{
			var proxy = new wcf.P08WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
												() => proxy.CancelPacking(F050801Data.DC_CODE, F050801Data.GUP_CODE, F050801Data.CUST_CODE, F050801Data.WMS_ORD_NO));

			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);
		}
		#endregion

		#endregion

		#region StartPacking 開始包裝

		#region Public Command
		public ICommand StartPackingCommand
		{
			get
			{
        Boolean IsSuccess = false;
        return CreateBusyAsyncCommand(
          o => IsSuccess = DoStartPacking(),
          () => (ValidTicket && UserOperateMode == OperateMode.Query && F050801Data != null && F050801Data.STATUS == 0),
          o => { if (IsSuccess) DoStartPackingComplete(); }
        );
			}
		}

		#endregion

		#region Private Method

		private Boolean DoStartPacking()
		{
			var proxy = new wcf.P08WcfServiceClient();

      var wcfF050801 = F050801Data.Map<F050801, wcf.F050801>();
      var checkShipModeResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.StartPackageCheck(wcfF050801, "1"));
      if (!checkShipModeResult.IsSuccessed)
      {
        Message = checkShipModeResult.Message;
        return false;
      }

      UserOperateMode = OperateMode.Edit;
			EnableReadSerial = true;

			StartPackOrNewBoxStartInitMessage();

			btnVis = Visibility.Collapsed;
      return true;
		}
		private void DoStartPackingComplete()
		{
			StartVideo();
			OnStartPackingComplete();
		}

		#endregion

		#endregion

		#region SearchTicket 依揀貨單號/ 合流箱號/ 出貨單號/出貨貼紙條碼 查詢單號

		#region Variable

		/// <summary>
		/// 是否單據檢核成功
		/// </summary>
		private bool _isTicketNoVerified = false;

		#endregion

		#region Public Command

		public ICommand SearchTicketCommand
		{
			get
			{
				bool mode = false;
				return CreateBusyAsyncCommand(
					o =>
					{
						_isTicketNoVerified = DoSearchTicket();
						if (_isTicketNoVerified && o != null)
						{
              mode = (bool)o;
              if (F050801Data != null && mode)
                if (DoStartPacking())
                  DispatcherAction(() => { DoStartPackingComplete(); });
            }
          },
          () => ValidTicket && UserOperateMode == OperateMode.Query && F050801Data != null && F050801Data.STATUS == 0,
          o => DoSearchTicketComplete(),
          null,
					InitialData
				);
			}
		}

		#endregion

		#region Private Method

    /// <summary>
    /// 按下補印箱明細後動作
    /// </summary>
    private void OnReprintClicked()
    {
      DispatcherAction(() =>
      {
        //更新刷讀記錄
        UpdateSerialRecords();
        OnScrollIntoSerialReadingLog();
      });
    }


    private bool DoSearchTicket()
    {
      // 尋找出貨單
      TicketNo = TicketNo.ToUpper();
      var proxy = new wcf.P08WcfServiceClient();
      var result = RunWcfMethod<wcf.F050801>(proxy.InnerChannel,
            () => proxy.SearchWmsOrder(SelectedDc, _gupCode, _custCode, TicketNo));

      if (result == null)
      {
        F050801Data = null;
        if (BarcodeService.IsOrder(TicketNo))
          Message = Properties.Resources.P0807010000_WmsOrdNoNotExist;
        else
          Message = Properties.Resources.P0807010000_QueryBoxNoNotExist;
        EnableReadSerial = false;

        return false;
      }

      var tmpF050801Data = ExDataMapper.Map<wcf.F050801, F050801>(result);
			if (tmpF050801Data.CUST_COST == "MoveOut")
			{
				Message = "跨庫出貨單不允許進行出貨包裝，請至跨庫整箱出貨/新稽核出庫處理";
				return false;
			}

	  var proxyF05 = GetProxy<F05Entities>();
      Date = tmpF050801Data.DELV_DATE;
      // 找到出貨單就可以判斷有無庫存, 再來決定後要面不要做
      CheckStock(tmpF050801Data);
      F050801Data = tmpF050801Data;
      if (!ShortInStock)
      {
        // 找到出貨單且有庫存, 再去找合流箱及揀貨單
        F052901Data = proxyF05.F052901s.Where(x => x.DC_CODE == F050801Data.DC_CODE && x.GUP_CODE == F050801Data.GUP_CODE &&
        x.CUST_CODE == F050801Data.CUST_CODE && x.WMS_ORD_NO == F050801Data.WMS_ORD_NO).FirstOrDefault();
      }

      // 建議紙箱編號(若為null顯示無建議紙箱)
      SugBoxNo = string.IsNullOrWhiteSpace(F050801Data.SUG_BOX_NO) ? "無建議紙箱" : F050801Data.SUG_BOX_NO;

      // 是否超取
      if (tmpF050801Data.CVS_TAKE == "1")
      {
        BillType = Properties.Resources.P0807010000_CVS_TAKE;
        btnVis = Visibility.Visible;
      }

      // 是否自取
      if (tmpF050801Data.SELF_TAKE == "1")
      {
        BillType = Properties.Resources.SELF_TAKE;
        btnVis = Visibility.Collapsed;
      }
      // 是否宅配
      if (tmpF050801Data.CVS_TAKE == "0" && tmpF050801Data.SELF_TAKE == "0")
      {
        BillType = Properties.Resources.P0807010000_HomeDelivery;
        btnVis = Visibility.Collapsed;
      }

      //是否代收
      hasTakeMoney = tmpF050801Data.COLLECT_AMT == 0 ? Visibility.Collapsed : Visibility.Visible;

      hasData = Visibility.Visible;

      var tmpF05030101 = proxyF05.F05030101s.Where(o => o.DC_CODE == tmpF050801Data.DC_CODE && o.GUP_CODE == tmpF050801Data.GUP_CODE && o.CUST_CODE == tmpF050801Data.CUST_CODE && o.WMS_ORD_NO == tmpF050801Data.WMS_ORD_NO).FirstOrDefault();
      var tmpF050301 = proxyF05.F050301s.Where(o => o.DC_CODE == tmpF050801Data.DC_CODE && o.GUP_CODE == tmpF050801Data.GUP_CODE && o.CUST_CODE == tmpF050801Data.CUST_CODE && o.ORD_NO == tmpF05030101.ORD_NO).FirstOrDefault();
      // 通路商
      //AllId = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "CHANNEL").FirstOrDefault(x => x.Value == tmpF050301.CHANNEL)?.Name;

      //顯示客戶自訂分類
      CustCost = GetBaseTableService.GetF000904List(FunctionCode, "F050101 ", "CUST_COST").Where(x => x.Value == tmpF050801Data.CUST_COST).FirstOrDefault()?.Name;

      //優先處理旗標
      FastDealType = GetBaseTableService.GetF000904List(FunctionCode, "F050101 ", "FAST_DEAL_TYPE").Where(x => x.Value == tmpF050801Data.FAST_DEAL_TYPE).FirstOrDefault()?.Name;


      // 箱明細搬到這裡, 因為即使已出貨完仍要取得紙箱需求
      UpdateSerialRecords();


      // 上面讀取到出貨單後，這裡檢查是否是刷虛擬商品
      if (F050801Data.VIRTUAL_ITEM == "1")
      {
        Message = Properties.Resources.P0807010000_VirtualItemError;
        EnableReadSerial = false;
        return false;
      }

      //檢查是否有派車單,才可包裝
      var f050301 = GetF050301S().FirstOrDefault();
      if (f050301 != null && f050301.SP_DELV != "02")
      {
        if (F050801Data.STATUS == 0 && F050801Data.SELF_TAKE == "0" && F050801Data.CVS_TAKE == "0" && !PackingService.IsSameDCInternalTrading(F050801Data))
        {
          var p08ExProxy = GetExProxy<P08ExDataSource>();
          if (!p08ExProxy.ExistsF700102ByWmsOrdNo(F050801Data.DC_CODE, F050801Data.GUP_CODE, F050801Data.CUST_CODE, F050801Data.WMS_ORD_NO, f050301.SOURCE_NO))
          {
            Message = Properties.Resources.P0807010000_F700102NotExist;
            EnableReadSerial = false;
            return false;
          }
        }
      }

      // 檢查宅配是否有設定對應的託運單格式
      if (F050801Data.PRINT_PASS == "1" && (F1947Data == null || string.IsNullOrEmpty(F1947Data.CONSIGN_REPORT)))
      {
        Message = Properties.Resources.P0807010000_ConsignReportNotExist;
        EnableReadSerial = false;
        return false;
      }

      // 出貨單的品項明細
      F050802Data = proxyF05.F050802s.Where(x => x.DC_CODE == F050801Data.DC_CODE && x.GUP_CODE == F050801Data.GUP_CODE
        && x.CUST_CODE == F050801Data.CUST_CODE && x.WMS_ORD_NO == F050801Data.WMS_ORD_NO).ToList();

      SetCustNameInfo(F050801Data);

      // 取得品項後更新刷讀統計
      RefreshDlvData();

      // 如果庫存短缺, 則直接離開不做其它事
      if (ShortInStock && !IsShortStockPackage) return false;

      // 提示訊息
      if (!IsShortStockPackage)
      {
        // 出貨包裝的判斷(出貨單為待處理 & 如果有合流,須先完成合流才可包裝)
        if (F050801Data != null && F050801Data.STATUS == 0 && (F052901Data == null || (F052901Data != null && F052901Data.STATUS == "1")))
          ValidTicket = true;
        else if (F050801Data != null && F050801Data.STATUS == -9999)
          Message = Properties.Resources.P0807010000_ORD_NO_CANCEL;
        else if (F050801Data != null && F050801Data.STATUS != 0)
          Message = Properties.Resources.P0807010000_WmsOrdNoStatusError;
        else if (F052901Data != null && F052901Data.STATUS != "1")
          Message = Properties.Resources.P0807010000_F052901DataExist;
      }
      else
      {
        // 缺貨包裝的判斷
        CheckShortStockStatus(F050801Data);
      }

      if (string.IsNullOrWhiteSpace(Message)) ValidTicket = true;

      // 設定並取得超商取貨/外部取號訂單之託運單資料
      SetSuperBuOrExternalOrderConsign(F050801Data);

      //檢查是否為未集貨訂單
      var f051301s = proxyF05.F051301s.Where(x => x.DC_CODE == tmpF050801Data.DC_CODE &&
                  x.GUP_CODE == tmpF050801Data.GUP_CODE &&
                  x.CUST_CODE == tmpF050801Data.CUST_CODE &&
                  x.WMS_NO == tmpF050801Data.WMS_ORD_NO).ToList();
      if (f051301s != null && f051301s.Any())
      {
        if (f051301s.FirstOrDefault()?.STATUS != "1")
        {
          ValidTicket = false;
          Message = Properties.Resources.P0807010000_UnfinishedCollection;
          return false;
        }
      }

      // 如果狀態不符就直接跳出
      if (!ValidTicket) return false;

      // 找出該作業員最近一次未完成包裝的記錄
      // 這裡要加上取出F055001的資料, 才不用再重KEY一次箱號, 可以輸入單號後就直接開始接續作業
      SetMyLastNoPrintF055001(F050801Data);



      // 取得出貨單明細商品資訊
      GetWmsOrderItemInfo(F050802Data);

      // 取得符合的紙箱需求 (必須先抓到品項明細)
      GetPackageSize();

      // 建議紙箱編號(若為null顯示無建議紙箱)
      SugBoxNo = string.IsNullOrWhiteSpace(F050801Data.SUG_BOX_NO) ? "無建議紙箱" : F050801Data.SUG_BOX_NO;

      return true;
    }

    private void DoSearchTicketComplete()
		{
      OnScrollFirstDeliveryData();
      _scanPackageCodeResult = null;
			RaisePropertyChanged("Message");
			RaisePropertyChanged("SA");
			RaisePropertyChanged("F050801Data");
			RaisePropertyChanged("F1909Data");
			RaisePropertyChanged("DlvData");
			OnSearchTicketComplete();
		}

		/// <summary>
		/// 檢查商品庫存是否足夠, 一併檢查出貨單狀態
		/// 在刷單號時就先檢查, 不用等到刷品號
		/// </summary>
		private void CheckStock(F050801 PrevDeliveryNoteData)
		{
			var proxy = GetProxy<F05Entities>();
			//檢查是否揀貨完成
			if (proxy.F051202s.Where(
				x => x.DC_CODE == PrevDeliveryNoteData.DC_CODE && x.GUP_CODE == PrevDeliveryNoteData.GUP_CODE
					 && x.CUST_CODE == PrevDeliveryNoteData.CUST_CODE && x.WMS_ORD_NO == PrevDeliveryNoteData.WMS_ORD_NO &&
					 x.PICK_STATUS == "0").Count() > 0)
			{
				Message = Properties.Resources.P0807010000_F051201DataExist;
				ShortInStock = true;
			}

			if (PrevDeliveryNoteData.PRINT_PASS == "1")
			{
				SetF1947ByWmsOrdNo(PrevDeliveryNoteData);
			}

			var proxyF19 = GetProxy<F19Entities>();
			// 貨主資訊
			F1909Data = proxyF19.F1909s.Where(x => x.GUP_CODE == PrevDeliveryNoteData.GUP_CODE && x.CUST_CODE == PrevDeliveryNoteData.CUST_CODE).FirstOrDefault();
			//檢查是否需列印發票
			F1909Data.INSTEAD_INVO = (F1909Data.INSTEAD_INVO == "1" && CheckPrintReceipt(PrevDeliveryNoteData)) ? "1" : "0";
			RaisePropertyChanged(() => F1909Data);

			if (PrevDeliveryNoteData.STATUS != 0)
			{
                //no1243 為訂單取消狀態，此狀態於P080701Service.GetWmsOrder指定
                if (PrevDeliveryNoteData.STATUS == -9999)
                {
                    Message = Properties.Resources.P0807010000_ORD_NO_CANCEL;
                    return;
                }

                if (PrevDeliveryNoteData.STATUS <= 6)
				{
					BindingReportButton(PrevDeliveryNoteData);
				}

				Message = string.Format(Properties.Resources.P0807010000_StatusName, _statusList.Where(n => n.Value == PrevDeliveryNoteData.STATUS.ToString()).Select(n => n.Name).First());
				ShortInStock = true;
				return;
			}

			// 如果是缺貨包裝, 直接離開此缺貨判斷
			// 合併缺貨包裝 移除判斷 2018/05/02 xin
			//if (IsShortStockPackage) return;

			CheckShortStockStatus(PrevDeliveryNoteData);
		}
		/// <summary>
		/// 判斷出貨單是否為缺貨狀態
		/// </summary>
		/// <param name="PrevDeliveryNoteData"></param>
		/// <returns></returns>
		private bool CheckShortStockStatus(F050801 PrevDeliveryNoteData)
		{
			if (PrevDeliveryNoteData == null)
				return false;

			// 判斷F051206的狀態, 如果回傳筆數>0, 則表示可以缺貨包裝
			var proxy = GetProxy<F05Entities>();

      var f05120601s = proxy.F05120601s.Where(x =>
        x.DC_CODE == PrevDeliveryNoteData.DC_CODE
        && x.GUP_CODE == PrevDeliveryNoteData.GUP_CODE
        && x.CUST_CODE == PrevDeliveryNoteData.CUST_CODE
        && x.WMS_ORD_NO == PrevDeliveryNoteData.WMS_ORD_NO).ToList();
      if (f05120601s.Any())
      {
        Message = "尚有揀缺未配庫資料，不可包裝";
        ShortInStock = true;
        return false;
      }

      var f051206Datas =
				proxy.F051206s.Where(x => x.DC_CODE == PrevDeliveryNoteData.DC_CODE && x.GUP_CODE == PrevDeliveryNoteData.GUP_CODE
											&& x.CUST_CODE == PrevDeliveryNoteData.CUST_CODE &&
											x.WMS_ORD_NO == PrevDeliveryNoteData.WMS_ORD_NO && x.ISDELETED == "0").ToList();
			if ((!f051206Datas.Any() || f051206Datas.All(n => n.RETURN_FLAG == "3"))) //如果沒有資料或找到商品且程式為缺貨包裝
			{
				//Message = "此出貨單無缺貨，請由出貨包裝處理";
				IsShortStockPackage = false;
				return false;
			}
			if (f051206Datas.Any(o => string.IsNullOrWhiteSpace(o.RETURN_FLAG)))
			{
				Message = Properties.Resources.P0807010000_F051206DataFlagNull;
				ShortInStock = true;
				IsShortStockPackage = true;
				return false;
			}
			if (f051206Datas.Any(o => o.RETURN_FLAG == "2"))
			{
				Message = Properties.Resources.P0807010000_f051206DatasFlag2;
				ShortInStock = true;
				IsShortStockPackage = true;
				return false;
			}
			if (f051206Datas.Any(o => o.RETURN_FLAG == "1"))
			{
				//Message = "此出貨單品項缺貨，請由缺貨包裝處理";
				ShortInStock = true;
				IsShortStockPackage = true;
				return false;
			}


			return true;
		}

		/// <summary>
		/// 檢查原訂單是否需列印發票
		/// </summary>
		private bool CheckPrintReceipt(F050801 f050801)
		{
			return GetF050301S(f050801).Any(n => n.PRINT_RECEIPT == "1");
		}

		/// <summary>
		/// 建議紙箱=計算商品材積, 取回所符合的最小號箱子回來提示使用者
		/// </summary>
		private void GetPackageSize()
		{
			decimal unitSize = 0;
			var proxy = GetProxy<F19Entities>();
			var proxyF00 = GetProxy<F00Entities>();

			// 商品材積
			foreach (var p in F050802Data)
			{
				var tmp = proxy.F1905s.Where(x => x.GUP_CODE == F050801Data.GUP_CODE && x.CUST_CODE == F050801Data.CUST_CODE && x.ITEM_CODE == p.ITEM_CODE).FirstOrDefault();
				if (tmp != null)
					unitSize += tmp.PACK_HIGHT * tmp.PACK_LENGTH * tmp.PACK_WIDTH * (p.B_DELV_QTY ?? 0);
			}

			// 取得預設容積率
			var tmpBoxRate = proxyF00.F0003s.Where(x => x.DC_CODE == SelectedDc && x.AP_NAME == "BoxRate").FirstOrDefault();
			decimal boxRate = 1;
			if (!decimal.TryParse(tmpBoxRate.SYS_PATH, out boxRate)) boxRate = 1;

			// 取得符合的紙箱大小
			var f1905s = proxy.CreateQuery<F1905>("GetCartonSize")
				.AddQueryExOption("gupCode", F050801Data.GUP_CODE)
				.AddQueryExOption("custCode", F050801Data.CUST_CODE)
				.ToList();
			var tmpBox = f1905s.Where(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH * boxRate >= unitSize)
				.OrderBy(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH).FirstOrDefault();
			// 找不到，表示超材，取最大紙箱
			if (tmpBox == null)
				tmpBox = f1905s.OrderByDescending(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH).FirstOrDefault();

			if (tmpBox != null)
			{
				// 紙箱的商品主檔要從 DB 撈，快取的F1903s用儲存出貨商品
				var tmpBoxItem = proxy.F1903s.Where(x => x.ITEM_CODE == tmpBox.ITEM_CODE && x.GUP_CODE == tmpBox.GUP_CODE && x.CUST_CODE == tmpBox.CUST_CODE).FirstOrDefault();
				BoxType = (tmpBoxItem != null) ? tmpBoxItem.ITEM_NAME : string.Empty;
			}
			else
			{
				BoxType = Properties.Resources.P0807010000_BoxIsNull;
			}
		}

		/// <summary>
		/// 更新UI序號刷讀紀錄
		/// </summary>
		private void UpdateSerialRecords()
		{
			var userInfo = Wms3plSession.Get<UserInfo>();

			var proxy = GetExProxy<P08ExDataSource>();
      SerialReadingLog = proxy.CreateQuery<SearchWmsOrderScanLogRes>("SearchWmsOrderScanLog")
                                              .AddQueryExOption("dcCode", _selectedDc)
                                              .AddQueryExOption("gupCode", _gupCode)
                                              .AddQueryExOption("custCode", _custCode)
                                              .AddQueryExOption("wmsOrdNo", F050801Data.WMS_ORD_NO)
                                              .ToObservableCollection();
    }

    /// <summary>
    /// 設定客戶資訊
    /// </summary>
    /// <param name="f050801"></param>
    private void SetCustNameInfo(F050801 f050801)
		{
			var proxyF19 = GetProxy<F19Entities>();
			var custCode = f050801.CUST_CODE;
			if (F1909Data.ALLOWGUP_RETAILSHARE == "1")
				custCode = "0";
			// 門市資訊
			F1910Data = proxyF19.F1910s.Where(x => x.GUP_CODE == F050801Data.GUP_CODE
				&& x.RETAIL_CODE == F050801Data.RETAIL_CODE && x.CUST_CODE == custCode).FirstOrDefault();

			if (F050801Data.ORD_TYPE == "0")
			{
				// B2B資訊, 帶出客戶代號 + 客戶名稱
				// 因RETAIL NAME可能是NULL, 所以多加上NULL的判斷, 避免NULL回來串接時又丟回NULL
				if (F1910Data == null || string.IsNullOrWhiteSpace(F1910Data.RETAIL_NAME))
					CustName = F050801Data.RETAIL_CODE;
				else
					CustName = F050801Data.RETAIL_CODE + " " + F1910Data.RETAIL_NAME;
			}
			else if (F050801Data.ORD_TYPE == "1")
			{
				// B2C資訊, 帶出客戶名稱 (隱碼)
				CustName = F050801Data.CUST_NAME;
			}
		}

		/// <summary>
		/// 設定並取得超商取貨/外部取號訂單之託運單資料(取F050304寫入F050901) 
		/// </summary>
		/// <param name="f050801"></param>
		private void SetSuperBuOrExternalOrderConsign(F050801 f050801)
		{
			var proxyF19 = GetProxy<F19Entities>();
			var proxyF05 = GetProxy<F05Entities>();
			F194704Data = proxyF19.F194704s.Where(x => x.DC_CODE == f050801.DC_CODE && x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE && x.ALL_ID == f050801.ALL_ID).FirstOrDefault();
			if (ValidTicket && (f050801.CVS_TAKE == "1" || (F194704Data != null && F194704Data.GET_CONSIGN_NO == "3")))
			{
				var result = CreateConsignBySuperBussiness(f050801);
				if (!result)
					ValidTicket = false;
				#region 取得超商或外部取號託運單號
				if (result)
				{
					var tmpF050901 = proxyF05.F050901s.Where(a => a.DC_CODE == f050801.DC_CODE && a.CUST_CODE == f050801.CUST_CODE && a.GUP_CODE == f050801.GUP_CODE && a.WMS_NO == f050801.WMS_ORD_NO).FirstOrDefault();
					consignNo = tmpF050901.CONSIGN_NO;
				}
				#endregion

			}

		}

		/// <summary>
		/// 超商取貨-取F050304 寫入F050901(托運單)
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		private bool CreateConsignBySuperBussiness(F050801 f050801)
		{
			var wcfF050801 = ExDataMapper.Map<F050801, wcfShared.F050801>(f050801);
			var proxy = new wcfShared.SharedWcfServiceClient();
			var result = RunWcfMethod<wcfShared.ExecuteResult>(proxy.InnerChannel,
						() => proxy.CreateConsignBySuperBU(wcfF050801));
			Message = result.Message;
			return result.IsSuccessed;
		}

		/// <summary>
		/// 取得出貨單商品明細
		/// </summary>
		/// <param name="f050802s"></param>
		private void GetWmsOrderItemInfo(List<F050802> f050802s)
		{
			var itemCodes = string.Join(",", f050802s.Select(x => x.ITEM_CODE).Distinct().ToList());
			var proxyF19 = GetProxy<F19Entities>();
			F1903s = proxyF19.CreateQuery<F1903>("GetF1903sByItemCodes")
							 .AddQueryExOption("gupCode", F050801Data.GUP_CODE)
							 .AddQueryExOption("custCode", F050801Data.CUST_CODE)
							 .AddQueryExOption("itemCodes", itemCodes)
							 .ToList();

			ItemNameList = F1903s.Select(x => new NameValuePair<string>
			{ Value = x.ITEM_CODE, Name = x.ITEM_NAME }).ToList();

		}

		#endregion

		#endregion

		#region ScanBarcode 查詢 品號/序號/箱號 

		#region Variable

		/// <summary>
		/// 是否完成目前這箱包裝
		/// </summary>
		private bool _finishCurrentBox = false;

		#endregion

		#region Public Method

		/// <summary>
		/// 查詢 品號/序號/箱號 
		/// </summary>
		/// <param name="qty">指定數量</param>
		public void ScanBarcode(int qty = 1)
    {
      var isScan = false;
      try
      {
        if (!Lock)
        {
          Lock = true;
          isScan = true;
          DoSearchSerial(qty);
          DoSearchSerialComplete(true, _scanPackageCodeResult.IsPass);
        }
      }
      finally
      {
        if (isScan && Lock)
          Lock = false;
      }
    }
    #endregion

    #region Private Method
    private void DoSearchSerial(int qty = 1)
		{
			Message = "";
			Serial = Serial.ToUpper();
			if (F050801Data == null)
			{
				Message = Properties.Resources.P0807010000_F050801DataIsNull;
				return;
			}
			// 刷箱號就進去編輯模式
			UserOperateMode = OperateMode.Edit;

			// 準備要刷讀的資料送到Server端處理
			var packgeCode = new wcf.PackgeCode { InputCode = Serial, AddQty = qty };

			// 若有刷過紙箱，則將紙箱帶入刷讀參數，與品號與數量一起刷讀
			if (_scanPackageCodeResult != null && !string.IsNullOrEmpty(_scanPackageCodeResult.BoxNum))
				packgeCode.BoxNum = _scanPackageCodeResult.BoxNum;

			ItemCode = Serial == "NEWBOX" ? string.Empty : Serial;

		
			// 刷讀檢核
			_scanPackageCodeResult = ScanPackageCode(packgeCode);
		
			// 呼叫錄影紀錄
			VideoShowItemByScan(packgeCode);

			if (_scanPackageCodeResult.IsPass && SoundOn)
				PlaySoundHelper.Scan();

			_finishCurrentBox = false;
			if (_scanPackageCodeResult.IsFinishCurrentBox)
			{
				AppendLog();
				_finishCurrentBox = true;
				return;
			}

			if (!_scanPackageCodeResult.IsPass)
				Message = _scanPackageCodeResult.Message;
			else if (_scanPackageCodeResult.IsCarton)
				InitialDataForNewBox();
			else
			{
				if (!string.IsNullOrWhiteSpace(_scanPackageCodeResult.Message))
					Message = _scanPackageCodeResult.Message;

				RefreshImage(_scanPackageCodeResult.ItemCode);

				ItemData = GetF1903(F050801Data.GUP_CODE, F050801Data.CUST_CODE, _scanPackageCodeResult.ItemCode);
				ItemCode = _scanPackageCodeResult.ItemCode;
			}
			AppendLog();
			PackQty = "1";
			SetMyLastNoPrintF055001(F050801Data);
		}

		private void DoSearchSerialComplete(bool initPackQty = true, bool isClose = true)
		{
			//更新圖片
			ItemImageSource = null;

			if (initPackQty)
				PackQty = "1";

			RefreshDlvData();
      OnScrollFirstDeliveryData();

      OnScrollIntoSerialReadingLog();

			if (_finishCurrentBox && isClose)
			{
				Serial = string.Empty;
				FinishCurrentBox();
			}
			else
			{
				ScrollIntoDeliveryData();
				OnSearchSerialComplete();
			
			}
			
			if (!IsNormalScan && F1909Data.MANAGER_LOCK == "3")
				OnRequireUnlock();
			else if (!IsNormalScan && F1909Data.MANAGER_LOCK == "1" && !string.IsNullOrEmpty(Message))
				ShowWarningMessage(Message);
			
		}

		/// <summary>
		/// 出貨包裝/缺貨包裝 刷讀紙箱、品號、序號
		/// </summary>
		/// <param name="packgeCode"></param>
		/// <returns></returns>
		private wcf.ScanPackageCodeResult ScanPackageCode(wcf.PackgeCode packgeCode)
		{
			packgeCode.F050801Item = ExDataMapper.Map<F050801, wcf.F050801>(F050801Data);
			packgeCode.F050301s = GetF050301S().MapCollection<F050301, wcf.F050301>().ToArray();
			packgeCode.F1903s = F1903s.MapCollection<F1903, wcf.F1903>().ToArray();
			packgeCode.F050802s = F050802Data.MapCollection<F050802, wcf.F050802>().ToArray();

			var proxy = new wcf.P08WcfServiceClient();
			var result = RunWcfMethod<wcf.ScanPackageCodeResult>(proxy.InnerChannel,
						() => proxy.ScanPackageCode(packgeCode));

			if (result.IsPass && result.IsCarton)
				packgeCode.BoxNum = result.BoxNum;

			return result;
		}

		/// <summary>
		/// 刷讀新箱子, 並且刷了箱號後, 把表單列印藏起來
		/// </summary>
		private void InitialDataForNewBox()
		{
			ReportList = new ObservableCollection<DynamicButtonData>();
		}

		/// <summary>
		/// 將視野移到刷讀後的商品上(Grid卷軸移到目前刷讀後商品)
		/// </summary>
		private void ScrollIntoDeliveryData()
		{
			if (!string.IsNullOrEmpty(ItemCode))
			{
				SelectedDeliveryData = DlvData.FirstOrDefault(x => x.ItemCode == ItemCode);
				OnScrollIntoDeliveryData();
			}
		}

		/// <summary>
		/// 出貨單取得配送商資料
		/// </summary>
		/// <param name="f050801"></param>
		private void SetF1947ByWmsOrdNo(F050801 f050801)
		{
			var proxy = GetProxy<F19Entities>();
			if (f050801.CVS_TAKE == "1")
				F1947Data = proxy.F1947s.Where(x => x.DC_CODE == f050801.DC_CODE && x.ALL_ID == f050801.ALL_ID).FirstOrDefault();
			else
				F1947Data = proxy.CreateQuery<F1947>("GetAllIdByWmsOrdNo")
								.AddQueryExOption("dcCode", f050801.DC_CODE)
								.AddQueryExOption("gupCode", f050801.GUP_CODE)
								.AddQueryExOption("custCode", f050801.CUST_CODE)
								.AddQueryExOption("wmsOrdNo", f050801.WMS_ORD_NO)
								.ToList()
								.FirstOrDefault();
		}


		/// <summary>
		/// 取得商品主檔
		/// </summary>
		/// <param name="proxy"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		private F1903 GetF1903(string gupCode, string custCode, string itemCode)
		{
			return F1903s.Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.CUST_CODE == custCode).FirstOrDefault();
		}

		#endregion

		#endregion

		#region UpdatePackQty 更新包裝數量

		#region Public Command
		public ICommand UpdatePackQtyCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoUpdatePackQty(),
					// 只有在刷讀品號時才能設定數量
					() => (F050801Data != null && !string.IsNullOrWhiteSpace(PackQty) && !string.IsNullOrWhiteSpace(ItemCode)),
					o => DoSearchSerialComplete(false)
				);
			}
		}
		#endregion

		#region Private Method
		private void DoUpdatePackQty()
		{
			Message = string.Empty;
			int packQty = 0;

			if (int.TryParse(PackQty, out packQty))
				DoSearchSerial(packQty);
			else
				Message = Properties.Resources.P0807010000_PackQtyForamtError;

		}

        #endregion

        #endregion

        #region Unlock 手動解刷讀鎖

        #region Public Command
        /// <summary>
        /// 手動解刷讀鎖
        /// </summary>
        public ICommand UnlockCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoUnlock(),
                    () => UserOperateMode == OperateMode.Edit
                );
            }
        }

        #endregion

        #region Private Method

        private void DoUnlock()
        {
            if (Lock)
            {
                Lock = false;
                ShowInfoMessage("刷讀鎖已解除");
            }
            else
            {
                ShowWarningMessage("不需要手動解刷讀鎖");
            }
        }
    #endregion

    #endregion

    #region CloseBox 手動關箱 / 關箱流程: 呼叫網家LMS 申請宅單Api->秤重->列印報表->關箱更新

    #region Public Command
    /// <summary>
    /// 手動關箱
    /// </summary>
    public ICommand CloseBoxCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => {},
            () => (F050801Data != null && (F050801Data.STATUS == 1 || F050801Data.STATUS == 2 || (F050801Data.STATUS == 0 && SerialReadingLog != null && SerialReadingLog.Any()))),
            o =>FinishCurrentBox(true)
        );
      }
    }

    #endregion

    #region Private Method

    /// <summary>
    /// 結束本次的裝箱之後要做的事 (列印報表/ 標籤)
    /// 1. 更新哩程碑
    /// 2. 顯示提示訊息
    /// 3. 列印報表
    /// </summary>
    private void FinishCurrentBox(bool isManualClose = false)
		{
			_finishCurrentBox = false;

			// 尚未輸入紙箱，重複刷NEWBOX 就會到這邊
			if (F055001Data != null && string.IsNullOrEmpty(F055001Data.BOX_NUM))
			{
				Message = Properties.Resources.P0803010000_CaseNoIsNull2;
				return;
			}

			// 2. 提示訊息 - 依條件有不同訊息
			if (SelectedF910501.WEIGHING_ERROR == "0")
				Message = Properties.Resources.P0807010000_PackComplete;

			if (IsCompletePackage)
				EndVideo();

			FinishPackageUpdateCommand.Execute(isManualClose);

		}

		private void DoCompletePackage()
		{
			if (IsCompletePackage)
			{
				var proxy = new wcf.P08WcfServiceClient();

				// 新增訂單回檔歷程紀錄表
				RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
								() => proxy.AddF050305Data(F050801Data.DC_CODE, F050801Data.GUP_CODE, F050801Data.CUST_CODE, new string[] { F050801Data.WMS_ORD_NO }, "3"));
				// 所有包裝完成後才更新F160204
				UpdateF160204();
				DispatcherAction(() =>
				{
					// 完成所有包裝, 鎖定畫面, 只保留列印表單
					EnableReadSerial = false;
					UserOperateMode = OperateMode.Query;

					OnSearchTicketComplete();
				});

			}
		}

		/// <summary>
		/// 列印報表
		/// </summary>
		/// <returns></returns>
		private List<Task> PrintReportAsync()
		{
			
			var tasks = new List<Task>();

			// 3.2 箱明細 - PrevDlvData
			// PRINT_BOX才能列印，但PRINT_DETAIL_FLAG=1則不自動印，可重印
			var p08Proxy = GetExProxy<P08ExDataSource>();
			var info = p08Proxy.GetDelvdtlInfo(F050801Data.DC_CODE, F050801Data.GUP_CODE, F050801Data.CUST_CODE, F050801Data.WMS_ORD_NO).ToList().FirstOrDefault();
			// 當SOURCE_TYPE=13，不需要列印箱明細
			if (F050801Data.SOURCE_TYPE == "13")
			{
				SetMyLastNoPrintF055001(F050801Data);

				tasks.Add(Task.Run(() =>
				{
					var littleWhiteReport = p08Proxy.GetLittleWhiteReport(F055001Data.DC_CODE, F055001Data.GUP_CODE, F055001Data.CUST_CODE, F055001Data.WMS_ORD_NO).ToList();

					TxtLog($"取得廠退小白標明細，資料筆數{littleWhiteReport.Count}");
					TxtLog($"列印廠退小白標明細");
					_deliveryReport.PrintRtnLable(SelectedF910501, littleWhiteReport);

				}));
			}
			else
			{
				if (F050801Data.PRINT_BOX == "1" && F050801Data.PRINT_DETAIL_FLAG == "0" && F1909Data.ISPRINTDELVDTL == "1")
				{
					tasks.Add(Task.Run(() =>
					{
						var f050301s = GetF050301S(F050801Data);

						var boxDetailList = p08Proxy.GetDeliveryReport(F055001Data.DC_CODE, F055001Data.GUP_CODE, F055001Data.CUST_CODE, F055001Data.WMS_ORD_NO, F055001Data.PACKAGE_BOX_NO).ToList();

						TxtLog($"取得箱明細，資料筆數{boxDetailList.Count}");

						if (info.AUTO_PRINT_DELVDTL == "1")
						{
							// PcHome 
							var pcHomeData = p08Proxy.GetPcHomeDelivery(F055001Data.DC_CODE, F055001Data.GUP_CODE, F055001Data.CUST_CODE, F055001Data.WMS_ORD_NO).FirstOrDefault();
							var count = pcHomeData != null ? 1 : 0;
							TxtLog($"取得箱頭檔，資料筆數{count}");
							TxtLog($"列印箱明細");

              _deliveryReport.PrintBoxData(boxDetailList, SelectedF910501, f050301s, F1909Data, info, pcHomeData);
						}
					}));
				}
			}

			return tasks;
		}

		private ICommand _finishPackageUpdateCommand;
		/// <summary>
		/// NEWBOX或完成包裝後，要更新的資訊。
		/// </summary>
		public ICommand FinishPackageUpdateCommand
		{
			get
			{
        Boolean ExecIsSuccess = false;
        return _finishPackageUpdateCommand ??
				(_finishPackageUpdateCommand = CreateBusyAsyncCommand(o =>
				{
          Boolean? isManualCloseBox = (Boolean?)o;
          ExecIsSuccess = true;
					
          SetMyLastNoPrintF055001(F050801Data);

					var wcfF050801 = F050801Data.Map<F050801, wcf.F050801>();
					var wcfF055001 = F055001Data.Map<F055001, wcf.F055001>();

					TxtLog($"關第{F055001Data.PACKAGE_BOX_NO}箱開始");

					var proxyEnt = GetProxy<F05Entities>();
          Boolean f055002DataIsExist = false;
          if (F055001Data != null && F055001Data.PRINT_FLAG == 0)
          {
            f055002DataIsExist = proxyEnt.F055002s.Where(x => x.DC_CODE == F050801Data.DC_CODE
                                 && x.GUP_CODE == F050801Data.GUP_CODE
                                 && x.CUST_CODE == F050801Data.CUST_CODE
                                 && x.WMS_ORD_NO == F050801Data.WMS_ORD_NO
                                 && x.PACKAGE_BOX_NO == F055001Data.PACKAGE_BOX_NO).Count() > 0;
          }
          if (!f055002DataIsExist)
          {
            Message = Properties.Resources.CloseBoxErrorMsg;
            ShowWarningMessage(Properties.Resources.CloseBoxErrorMsg);
            ExecIsSuccess = false;
						TxtLog($"關第{F055001Data.PACKAGE_BOX_NO}箱失敗，{Properties.Resources.CloseBoxErrorMsg}");
						return;
          }

					TxtLog($"後端關箱處理中");
					var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
          var result = proxy.RunWcfMethod(w => w.FinishCurrentBox(wcfF050801, wcfF055001, IsCompletePackage, isManualCloseBox ?? false));

          #region 託運單錯誤訊息顯示
          if (_printPassTask != null)
					{
						_printPassTask.Wait();
						if (!_printPassTask.Result.IsSuccessed)
							ShowWarningMessage(_printPassTask.Result.Message);
						_printPassTask = null;
					}
          #endregion

          if (!result.IsSuccessed)
          {
            Message = result.Message;
						TxtLog($"後端關箱失敗，{result.Message}");
						if (string.IsNullOrEmpty(result.LMSMessage))
						{
							ShowWarningMessage(result.Message);
						}
						else
						{
							ShowWarningMessage(result.LMSMessage);
							TxtLog($"LMS取得宅單失敗，{result.Message}");
						}
            ExecIsSuccess = false;
          }
          else
          {
						if (!string.IsNullOrEmpty(result.Message))
						{
							Message = result.Message;
							TxtLog($"後端關箱成功，{result.Message}");
						}
						else
							TxtLog($"後端關箱成功");

						SetMyLastNoPrintF055001(F050801Data);

						if(!IsCompletePackage)
							StartPackOrNewBoxStartInitMessage();

            //SerialReadingLog.Add(new SerialReadingStatus { IsPass = "1", Message = "人員按下手動關箱" });

          }

					if (result.IsSuccessed)
					{
            // 3.0 準備好有哪些東西要印
            BindingReportButton(F050801Data);

            //寫入F05500101列印箱明細
            var exproxy = GetExProxy<P08ExDataSource>();
            exproxy.LogPrintBoxDetailPacking(F055001Data.DC_CODE, F055001Data.GUP_CODE, F055001Data.CUST_CODE, F055001Data.WMS_ORD_NO, F055001Data.PACKAGE_BOX_NO.ToString());

            var tasks = PrintReportAsync();

            F050801Data = result.F050801Data.Map<wcf.F050801, F050801>();
						F055001Data = result.F055001Data.Map<wcf.F055001, F055001>();
						if (_scanPackageCodeResult != null)
							_scanPackageCodeResult.BoxNum = null;
						
					}
					else
					{
						var f05Proxy = GetProxy<F05Entities>();
						F050801Data = f05Proxy.F050801s.FindByKey(F050801Data);
						SetMyLastNoPrintF055001(F050801Data);
					}
					RefreshDlvData();
					TxtLog($"關第{F055001Data.PACKAGE_BOX_NO}箱結束");
				},
				() => true,
				o =>
				{
          OnScrollFirstDeliveryData();
          if (!ExecIsSuccess)
            return;
					if (IsCompletePackage)
					{
						DoCompletePackage();
					}
					else
					{
            UpdateSerialRecords();
            OnScrollIntoSerialReadingLog();
            //OnSearchSerialComplete();
          }
        }));
			}
		}

		#endregion

		#endregion

		#region Help 求救

		#region Public Command

		/// <summary>
		/// 求救
		/// </summary>
		public ICommand HelpCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoHelp(),
					() => true,
					o => DoHelpComplete()
				);
			}
		}

		#endregion

		#region Private Method
		/// <summary>
		/// 求救, 寫入F0010
		/// </summary>
		private void DoHelp()
		{
			var proxy = GetProxy<F00Entities>();
			var PrevDeliveryNoteData = new F0010()
			{
				DC_CODE = SelectedDc,
				HELP_TYPE = "02",
				DEVICE_PC = Wms3plSession.Get<GlobalInfo>().ClientIp,
				ORD_NO = (F050801Data != null ? F050801Data.WMS_ORD_NO : ""),
				STATUS = "0",
				GUP_CODE = _gupCode,
				CUST_CODE = _custCode
			};
			proxy.AddToF0010s(PrevDeliveryNoteData);
			proxy.SaveChanges();
			Message = Properties.Resources.P0802010000_Help + Message;
		}
		private void DoHelpComplete()
		{

		}

		#endregion

		#endregion

		#region RePacking 重新取得託運單號(外部單號使用F050304 TO F050901)

		#region Public Command
		/// <summary>
		/// 重新取得託運單號(外部單號使用F050304 TO F050901)
		/// </summary>
		public ICommand RePackingCommand
		{
			get
			{
				bool isRePackOk = false;
				return CreateBusyAsyncCommand(
					o => { isRePackOk = DoRePacking(); },
					() => (F050801Data != null && (F050801Data.STATUS == 1 || F050801Data.STATUS == 2 || (F050801Data.STATUS == 0 && SerialReadingLog != null && SerialReadingLog.Any() && F050801Data.CVS_TAKE == "1"))),
					o =>
					{
						if (isRePackOk)
						{
							SearchTicketCommand.Execute(null);
							DoRePackingComplete();
						}
					}
				);
			}
		}

		#endregion

		#region Private Method

		private bool DoRePacking()
		{

			UserOperateMode = OperateMode.Query;
			return true;
		}

		private void DoRePackingComplete()
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("DoRePacking")
								.AddQueryExOption("dcCode", SelectedDc)
								.AddQueryExOption("gupCode", _gupCode)
								.AddQueryExOption("custCode", _custCode)
								.AddQueryExOption("wmsOrdCode", TicketNo)
								.ToList()
								.FirstOrDefault();

			ShowResultMessage(result.IsSuccessed, result.IsSuccessed ? Properties.Resources.P0807010000_RePackingComplete : Properties.Resources.P0807010000_RePackingFail + result.Message);
		}
		#endregion

		#endregion

		#endregion

		#region Share Method

		/// <summary>
		/// 初始化設定
		/// </summary>
		private void InitialData()
		{
			F050801Data = null;
			F050802Data = new List<F050802>();
			F052901Data = null;
			F055001Data = null;
			ItemData = null;
			F1909Data = null;
			F1910Data = null;
			F194704Data = null;
			BoxType = string.Empty;
			SA = string.Empty;
			ItemCode = string.Empty;
			SerialReadingLog = new ObservableCollection<SearchWmsOrderScanLogRes>();
			DlvData = new List<DeliveryData>();
			Message = string.Empty;
			Serial = string.Empty;
			ShortInStock = false;
			EnableReadSerial = false;
			ValidTicket = false;
			RefreshImage("");
			PackQty = "1";
			ReportList = new ObservableCollection<DynamicButtonData>();
			//ReportRtnList = new ObservableCollection<DynamicButtonData>();
			CustName = string.Empty;
			hasData = Visibility.Collapsed;
			btnVis = Visibility.Collapsed;
			_f050301s = null;
			_finishCurrentBox = false;
			CustOrdNos = new List<string>();
		}

		/// <summary>
		/// 設定最後一箱箱明細頭檔
		/// </summary>
		/// <param name="f050801"></param>
		private void SetMyLastNoPrintF055001(F050801 f050801)
		{
			var proxy = GetProxy<F05Entities>();
			F055001Data = proxy.F055001s.Where(x => x.DC_CODE == f050801.DC_CODE
												&& x.GUP_CODE == f050801.GUP_CODE
												&& x.CUST_CODE == f050801.CUST_CODE
												&& x.WMS_ORD_NO == f050801.WMS_ORD_NO)
										.OrderByDescending(x => x.PACKAGE_BOX_NO)
										.FirstOrDefault();
		}

		/// <summary>
		/// 開始包裝/原箱商品開始/新箱開始 初始化訊息內容
		/// </summary>
		private void StartPackOrNewBoxStartInitMessage()
		{
			// 是否還有原箱商品尚未刷讀完成
			var hasOrginalItemNotPackFinish = (from detail in DlvData
																				 where detail.AllowOrdItem == "1" && detail.DiffQty > 0
																				 select detail).Any();

			// 出貨單含有原箱，且原箱商品尚未刷讀
			if (F050801Data.ALLOWORDITEM == "1" && hasOrginalItemNotPackFinish)
				// 請刷讀原箱品號/序號/箱號
				Message = Properties.Resources.P0807010000_ScanOrginalItem;
			else if (F055001Data == null || F055001Data.PRINT_FLAG == 1)
				// 請刷讀箱號
				Message = Properties.Resources.P0803010000_CaseNoIsNull2;
			else if (F055001Data != null && F055001Data.BOX_NUM == "ORI" && string.IsNullOrWhiteSpace(F055001Data.PAST_NO) && F050801Data.SOURCE_TYPE != "13")
				Message = Properties.Resources.P0807010000_PreBoxHasOrginalItemNotClose;
			else
				// 請刷讀品號/序號/盒號/箱號
				Message = Messages.ScanSerialorBox;

		}

		/// <summary>
		/// 新增刷讀紀錄
		/// </summary>
		private void AppendLog()
		{
			var list = SerialReadingLog == null ? new List<SearchWmsOrderScanLogRes>() : SerialReadingLog.ToList();
			list.Add(new SearchWmsOrderScanLogRes { IsPass = _scanPackageCodeResult.IsPass ? "1" : "0", ItemCode = _scanPackageCodeResult.ItemCode, Message = _scanPackageCodeResult.Message, SerialNo = _scanPackageCodeResult.SerialNo });
			SerialReadingLog = list.ToObservableCollection();
		}

		/// <summary>
		/// 取得訂單主檔資料
		/// </summary>
		/// <param name="f050801">若沒填，則使用已刷讀單號後的F050801Data做查詢</param>
		/// <returns></returns>
		private List<F050301> GetF050301S(F050801 f050801 = null)
		{
			if (_f050301s == null || !_f050301s.Any())
			{
				_f050301s = GetDataByWmsOrdNo(f050801 ?? F050801Data);

				var custOrdNos = _f050301s.Select(x => x.CUST_ORD_NO).Distinct().ToList();
				var lines = new List<string>();
				var query = custOrdNos.AsEnumerable();
				while (query.Any())
				{
					lines.Add(string.Join("　", query.Take(3)));
					query = query.Skip(3);
				}
				CustOrdNos = lines;
			}

			return _f050301s;
		}

		/// <summary>
		/// 出貨單取得併單訂單資料
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		private List<F050301> GetDataByWmsOrdNo(F050801 f050801)
		{
			var proxy = GetProxy<F05Entities>();
			var list = proxy.CreateQuery<F050301>("GetDataByWmsOrdNo")
							.AddQueryExOption("dcCode", f050801.DC_CODE)
							.AddQueryExOption("gupCode", f050801.GUP_CODE)
							.AddQueryExOption("custCode", f050801.CUST_CODE)
							.AddQueryExOption("wmsOrdNo", f050801.WMS_ORD_NO)
							.ToList();
			return list;
		}


		/// <summary>
		/// 刷品號或序號時, 更新圖檔的顯示
		/// </summary>
		public void RefreshImage(string itemCode)
		{
			if (string.IsNullOrEmpty(itemCode))
			{
				ItemCode = null;
				ItemImageSource = null;
			}
			RaisePropertyChanged("ItemImageSource");
		}

		/// <summary>
		/// 產生列印表單的按鈕
		/// </summary>
		private void BindingReportButton(F050801 prevDeliveryNoteData)
		{
      //避免使用者要補印已完成單據造成寫入F05500101"人員按下補印"訊息會發生錯誤用
      F055001 tmpF055001 = null;
      if (F055001Data != null)
        tmpF055001 = F055001Data;
      else
      {
        var proxy = GetProxy<F05Entities>();
        tmpF055001 = proxy.F055001s.Where(x => x.DC_CODE == prevDeliveryNoteData.DC_CODE
                          && x.GUP_CODE == prevDeliveryNoteData.GUP_CODE
                          && x.CUST_CODE == prevDeliveryNoteData.CUST_CODE
                          && x.WMS_ORD_NO == prevDeliveryNoteData.WMS_ORD_NO)
                      .OrderByDescending(x => x.PACKAGE_BOX_NO)
                      .FirstOrDefault();
      }
      ReportList = _deliveryReport.BindingReportButton(prevDeliveryNoteData, tmpF055001,
				SelectedF910501, F1947Data, F1909Data, CreateBusyAsyncCommand, GetF050301S(prevDeliveryNoteData), false);
    }

    //private void BindingReportRtnButton(F055001 F055001Data)
    //{
    //	ReportRtnList = _deliveryReport.BindingReportRtnButton(F055001Data,SelectedF910501, CreateBusyAsyncCommand);
    //}

    /// <summary>
    /// 每次刷讀都重新更新一次總刷讀數量
    /// Memo: 計算F055002中, 同樣出貨單號的資料數
    /// </summary>
    private void RefreshDlvData()
    {
      if (F050801Data == null) return;
      
      var packageBoxNo = F055001Data == null ? (short)0 : F055001Data.PACKAGE_BOX_NO;
      if (F055001Data != null && F055001Data.PRINT_FLAG == 1)
        packageBoxNo = (short)0;

      DlvData = PackingService.RefreshReadCount(DlvData, F050801Data.DC_CODE, F050801Data.GUP_CODE, F050801Data.CUST_CODE, F050801Data.WMS_ORD_NO, packageBoxNo, FunctionCode);
      RaisePropertyChanged("DlvData");

      // 如果都出完貨了就跑關箱流程
      if (IsCompletePackage && F055001Data != null) _finishCurrentBox = true;
    }



    #region Video Method
    private void StartVideo()
		{
			var result = _videoServerHelper.ConnectVideoServer(SelectedF910501.VIDEO_URL,
																SelectedF910501.VIDEO_ERROR == "1",
																SelectedF910501.VIDEO_NO,
																_staff,
																_staffName);
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return;
			}

			result = _videoServerHelper.VideoStartSessionByCustOrderNo(F050801Data.WMS_ORD_NO,
																		F050801Data.RETAIL_CODE,
																		F1910Data?.RETAIL_NAME,
																		F050801Data.DELV_DATE);
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return;
			}
		}

		private void VideoShowItemByScan(wcf.PackgeCode packgeCode)
		{
			var itemName = F1903s.Where(x => x.ITEM_CODE == packgeCode.InputCode).SelectFirstOrDefault(x => x.ITEM_NAME);
			var orderQty = F050802Data.Where(x => x.ITEM_CODE == packgeCode.InputCode).SelectFirstOrDefault(x => x.B_DELV_QTY ?? 0);
			// 錄影機台命令發送
			var videoResult = _videoServerHelper.VideoShowItemByScan(F050801Data.WMS_ORD_NO,
																	F055001Data?.PAST_NO,
																	itemName,
																	packgeCode.InputCode,
																	orderQty,
																	packgeCode.AddQty,
																	Convert.ToString(F055001Data?.PACKAGE_BOX_NO),
																	null);
#if DEBUG
			if (!videoResult.IsSuccessed)
				ShowWarningMessage(videoResult.Message);
#endif
		}

		private void EndVideo()
		{
			var result = _videoServerHelper.VideoEndSession(F050801Data.WMS_ORD_NO, F055001Data?.PAST_NO, null);
			if (!result.IsSuccessed) ShowWarningMessage(result.Message);
		}
		#endregion

		#region 網家 LMS API 申請宅配單 /取消申請宅配單

		/// <summary>
		/// 網家LMS-申請宅配單
		/// </summary>
		/// <returns></returns>
		private wcf.ExecuteResult ApplyConsign()
		{
			// 廠退出貨不需要申請宅單
			if (F050801Data.SOURCE_TYPE == "13")
				return new wcf.ExecuteResult { IsSuccessed = true };

			SetMyLastNoPrintF055001(F050801Data);
			var proxy = GetProxy<F05Entities>();
			bool f055002DataIsExist = false;
			if (F055001Data != null && F055001Data.PRINT_FLAG == 0)
			{
				f055002DataIsExist = proxy.F055002s.Where(x => x.DC_CODE == F050801Data.DC_CODE
														 && x.GUP_CODE == F050801Data.GUP_CODE
														 && x.CUST_CODE == F050801Data.CUST_CODE
														 && x.WMS_ORD_NO == F050801Data.WMS_ORD_NO
														 && x.PACKAGE_BOX_NO == F055001Data.PACKAGE_BOX_NO).Count() > 0;
			}


			if (!f055002DataIsExist)
				return new wcf.ExecuteResult { IsSuccessed = false, Message = Properties.Resources.CloseBoxErrorMsg };

			var proxyWcf = new wcf.P08WcfServiceClient();
			var res = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
																					() => proxyWcf.ApplyConsign(SelectedDc, _gupCode, _custCode, F050801Data.WMS_ORD_NO, F055001Data.PACKAGE_BOX_NO));

			if (!res.IsSuccessed)
			{
				Message = Properties.Resources.P0807010000_PreBoxHasNotClose;
			}
			return res;
		}

		/// <summary>
		/// 網家LMS-取消申請宅配單
		/// </summary>
		/// <returns></returns>
		private wcf.ExecuteResult CancelConsign()
		{
			var proxy = new wcf.P08WcfServiceClient();
			var res = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
																					() => proxy.CancelConsign(SelectedDc, _gupCode, _custCode, F050801Data.WMS_ORD_NO));

			return res;
		}

		#endregion

		#endregion

		#region NotUsed Code Backup

		#region 每一分鐘取一次出車時間排程

		#region Public Variable

		public Timer timer = new Timer();

		#endregion

		#region Public Property

		#region Form - 時間提醒的背景顏色/ 可出車時間
		public Brush InfoCarStatusColor
		{
			get
			{
				if (InfoCarStatus == null) return Brushes.White;
				return (Brush)InfoCarStatus["Color"];
			}
		}
		public string InfoCarStatusText
		{
			get
			{
				if (InfoCarStatus == null) return string.Empty;
				return (string)InfoCarStatus["Text"];
			}
		}
		private Dictionary<string, object> _infoCarStatus = Common.InfoCarStatus.OverThirty;
		public Dictionary<string, object> InfoCarStatus
		{
			get
			{
				return _infoCarStatus;
			}
			set
			{
				_infoCarStatus = value;
				RaisePropertyChanged("InfoCarStatusText");
				RaisePropertyChanged("InfoCarStatusColor");
			}
		}
		private TimeSpan _pickTime = TimeSpan.MinValue;
		public TimeSpan PickTime
		{
			get { return _pickTime; }
			set
			{
				_pickTime = value;

				var surplusTime = _pickTime.Add(-DateTime.Now.TimeOfDay);

				if (surplusTime.TotalMinutes >= 30)
					_infoCarStatus = Common.InfoCarStatus.OverThirty;
				else if (surplusTime.TotalMinutes >= 15)
					_infoCarStatus = Common.InfoCarStatus.InThirty;
				else
					_infoCarStatus = Common.InfoCarStatus.InFifteen;

				RaisePropertyChanged("PickTime");
			}
		}
		#endregion

		#endregion

		#region Private Method

		private void StartPickTimeSchedule()
		{
			//初始化執行時所需的值及資料
			timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			timer.Interval = 1 * 60000; // Memo: 每一分鐘取一次出車時間
			timer.Enabled = true;
		}
		/// <summary>
		/// 更新下次出車時間的提醒
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			SearchPickTimeCommand.Execute(null);
			RaisePropertyChanged("InfoCarStatusColor");
			RaisePropertyChanged("InfoCarStatusText");
		}

		#endregion

		#region Public Method

		#region SearchPickTimeCommand 查詢最近可出車時間
		/// <summary>
		/// 尋找最近可出車時間. 不套用BusyCommand因為不需要出現Loading畫面
		/// </summary>
		public ICommand SearchPickTimeCommand
		{
			get
			{
				return new AsyncDelegateCommand(
					o => DoSearchPickTime(),
					() => true,
					o => DoSearchPickTimeComplete()
				);
			}
		}
		/// <summary>
		/// 查找時間
		/// </summary>
		private void DoSearchPickTime()
		{
			var proxy = GetExProxy<P08ExDataSource>();
			var nearestTakeTime = proxy.CreateQuery<string>("GetNearestTakeTime")
				 .AddQueryExOption("dcCode", SelectedDc)
							 .AddQueryExOption("gupCode", _gupCode)
							 .AddQueryExOption("custCode", _custCode)
							 .ToList()
											 .FirstOrDefault();

			TimeSpan nearestTakeTimeSpan;
			TimeSpan.TryParse(nearestTakeTime, out nearestTakeTimeSpan);
			PickTime = nearestTakeTimeSpan;
		}
		/// <summary>
		/// 查找時間之後要做更新畫面動作
		/// </summary>
		private void DoSearchPickTimeComplete()
		{
			RaisePropertyChanged("InfoCarStatusColor");
			RaisePropertyChanged("InfoCarStatusText");
		}
		#endregion

		#endregion

		#endregion

		#endregion

		#region 完成所有包裝更新F160204

		private wcf.ExecuteResult UpdateF160204()
		{
			var proxy = new wcf.P08WcfServiceClient();
			var res = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
																					() => proxy.UpdateF160204(SelectedDc, _gupCode, _custCode, F050801Data.WMS_ORD_NO));

			return res;
		}
		#endregion

		private  void TxtLog(string message)
		{
			var folderPath = Path.Combine(ConfigurationManager.AppSettings["ShareFolderTemp"].ToString(), "PackageLog");
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			var fileName = $"{DateTime.Today.ToString("yyyyMMdd")}.log";
			var fullFileName = Path.Combine(folderPath, fileName);

			try
			{
				using (var sw = new StreamWriter(fullFileName, true))
				{
					sw.WriteLine($"時間:{DateTime.Now.ToString("yyyyy/MM/dd HH:mm:ss")} 出貨單號:{ F050801Data.WMS_ORD_NO } =>"+message);
				}
			}
			catch (Exception ex)
			{
				fileName = $"{DateTime.Today.ToString("yyyyMMdd")}_{Guid.NewGuid().ToString()}.log";
				fullFileName = Path.Combine(folderPath, fileName);
				using (var sw = new StreamWriter(fullFileName, true))
				{
					sw.WriteLine($"時間:{DateTime.Now.ToString("yyyyy/MM/dd HH:mm:ss")} 出貨單號:{ F050801Data.WMS_ORD_NO } =>" + message);
				}
			}

		}
	}
}
