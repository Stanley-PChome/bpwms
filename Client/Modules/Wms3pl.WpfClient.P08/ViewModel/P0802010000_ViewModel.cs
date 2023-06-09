using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using System.Windows.Media.Imaging;
using Wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.LabelPrinter.Bartender;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using ex = Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using System.IO;
using Wms3pl.WpfClient.DataServices.F91DataService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P08ExDataService.ExecuteResult;
using Wms3pl.WpfClient.VideoServer;
using Wms3pl.WpfClient.P08.Views;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public enum WorkFlow
	{
		/// <summary>
		/// 點收前
		/// </summary>
		BeforeCheckAndAccept,
		/// <summary>
		/// 點收中
		/// </summary>
		CheckAndAccept
	}

	public partial class P0802010000_ViewModel : InputViewModelBase
	{
		private string _gupCode = null;
		private string _custCode = null;
		private string _staff = null;
		private string _staffName = null;
		private string _clientPC = null;
		private VideoServerHelper _videoServerHelper = null;

		public Action OnDcCodeChanged = () => { };
		public Action<SelectionItem<F161402Data>> SetRecordScroll = delegate { };
		public Action<F16140101Data> SetSerialRecordScroll = delegate { };
		public Action SetInputSerialFocus = delegate { };
		public Action SetTxtInputBillNoFocus = delegate { };
		public P0802010000_ViewModel()
		{
			if (!IsInDesignMode)
			{

				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				_staff = Wms3plSession.CurrentUserInfo.Account;
				_staffName = Wms3plSession.CurrentUserInfo.AccountName;
				_clientPC = Wms3plSession.Get<GlobalInfo>().ClientIp;

				//初始化執行時所需的值及資料
				SetInit();
			}
		}

		/// <summary>
		/// Device 的設定 (當物流中心改變時，就會去顯示設定 Device 的畫面)  
		/// </summary>
		public F910501 SelectedF910501 { get; set; }
		public string VideoNo
		{
			get
			{
				return SelectedF910501?.VIDEO_ERROR != "1" ? SelectedF910501?.VIDEO_NO : null;
			}
		}


		#region Form - 可用的DC (物流中心)清單,Gup,Cust等
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				if (Set(() => SelectedDc, ref _selectedDc, value))
				{
					OnDcCodeChanged();
					SetInit();
				}
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

		#region 流程,訊息,效果等
		#region 目前工作流程 NowWorkFlow
		private WorkFlow _nowWorkFlow = WorkFlow.BeforeCheckAndAccept;
		public WorkFlow NowWorkFlow
		{
			get { return _nowWorkFlow; }
			set
			{
				_nowWorkFlow = value;
				RaisePropertyChanged("NowWorkFlow");
			}
		}
		#endregion
		#region Form - 語音
		private bool _playSound = true;
		public bool PlaySound
		{
			get { return _playSound; }
			set { _playSound = value; RaisePropertyChanged("PlaySound"); }
		}
		#endregion
		#region 跑馬燈
		private string _marqueeMessage;
		public string MarqueeMessage
		{
			get { return _marqueeMessage; }
			set
			{
				_marqueeMessage = value;
				RaisePropertyChanged("MarqueeMessage");
			}
		}
		#endregion
		#region 訊息
		private string _notExist;
		public string NotExist { get { return _notExist; } set { _notExist = value; RaisePropertyChanged("NotExist"); } }
		#endregion
		#region 是否為錯誤(訊息)
		private string _isError = string.Empty;
		public string IsError { get { return _isError; } set { _isError = value; RaisePropertyChanged("IsError"); } }
		#endregion
		#region Data - 異常原因
		private List<F1951> _uccList = new List<F1951>();
		public List<F1951> UccList
		{
			get { return _uccList; }
			set { _uccList = value; RaisePropertyChanged("UccList"); }
		}
		#endregion
		#region Form - 圖檔名稱/ ShareFolder路徑
		private string _imgItemCode;
		public string ImgItemCode { get { return _imgItemCode; } set { _imgItemCode = value; ItemImageSource = null; RaisePropertyChanged(); } }
		public bool IsImageUploaded
		{
			get { return _itemImageSource != null; }
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
				if (ImgItemCode == null || string.IsNullOrWhiteSpace(ImgItemCode)) _itemImageSource = null;
				else
				{
					_itemImageSource = FileService.GetItemImage(_gupCode, _custCode, ImgItemCode);
				}
				RaisePropertyChanged("ItemImageSource");
				RaisePropertyChanged("IsImageUploaded");
			}
		}
		#endregion
		#endregion

		#region 退貨主檔等
		#region 刷讀 退貨單/貨主退貨單/原出貨單/商品召回單單號
		private string _inputBillNo;
		public string InputBillNo { get { return _inputBillNo; } set { _inputBillNo = value; RaisePropertyChanged("InputBillNo"); } }
		#endregion
		#region 刷讀 品號/序號
		private string _inputSerialNo;
		public string InputSerailNo { get { return _inputSerialNo; } set { _inputSerialNo = value; RaisePropertyChanged("InputSerailNo"); } }
		#endregion
		#region 退貨主檔
		private F161201 _returnRecordMain;
		public F161201 ReturnRecordMain { get { return _returnRecordMain; } set { _returnRecordMain = value; RaisePropertyChanged("ReturnRecordMain"); } }
		#endregion
		#region 退貨清單
		private SelectionList<F161402Data> _returnRecordItems;
		public SelectionList<F161402Data> ReturnRecordItems { get { return _returnRecordItems; } set { _returnRecordItems = value; RaisePropertyChanged(); } }
		#endregion
		#region 是否可過帳
		public bool IsPosting
		{
			get { return GetIsCanPosting(); }
		}
		#endregion
		#region 是否為待處理或處理中
		public bool IsPending
		{
			get { return GetIsPending(); }
		}
		#endregion
		#region 退貨明細
		private bool _isBoundleSeiral = false;
		public bool IsHasRows { get { return (ReturnRecordDetails == null) ? false : (ReturnRecordDetails.Count() > 0 && ReturnRecordMain.STATUS!="2"); } }
		private SelectionList<F161402Data> _returnRecordDetails;
		public SelectionList<F161402Data> ReturnRecordDetails
		{
			get { return _returnRecordDetails; }
			set
			{
				_returnRecordDetails = value;
				RaisePropertyChanged("ReturnRecordDetails");
				RaisePropertyChanged("IsHasRows");

				if (value != null)
					SerialRecords = GetSerialItems();
			}
		}
		#endregion
		#region 選擇的退貨明細項目
		private SelectionItem<F161402Data> _selectRecordDetail;
		public SelectionItem<F161402Data> SelectRecordDetail
		{
			get { return _selectRecordDetail; }
			set
			{
				_selectRecordDetail = value;
				RaisePropertyChanged("SelectRecordDetail");
			}
		}
		#endregion
		#region 系統產生不明件BarCode
		private string _barCode;
		public string BarCode { get { return _barCode; } set { _barCode = value; RaisePropertyChanged("BarCode"); } }
		#endregion
		#region 是否直接點選列印不明BarCode
		private bool _isNoAnySerialNo = false;
		public bool IsNoAnySerialNo { get { return _isNoAnySerialNo; } set { _isNoAnySerialNo = value; RaisePropertyChanged("IsNoAnySerialNo"); } }
		#endregion
		#region 刷驗數
		private int _scanCheckCount = 1;
		public int ScanCheckCount { get { return _scanCheckCount; } set { _scanCheckCount = value; RaisePropertyChanged("ScanCheckCount"); } }
		#endregion
		#region 預設儲位
		private string _defaultLoc = string.Empty;
		public string DefaultLoc { get { return _defaultLoc; } set { _defaultLoc = value; RaisePropertyChanged("DefaultLoc"); } }
		#endregion
		#region 序號刷讀記錄明細
		private List<F16140101Data> _serialRecords;
		public List<F16140101Data> SerialRecords { get { return _serialRecords; } set { _serialRecords = value; RaisePropertyChanged("SerialRecords"); } }
		#endregion
		#region 選擇的序號刷讀記錄
		private F16140101Data _selectSerialRecord;
		public F16140101Data SelectSerialRecord { get { return _selectSerialRecord; } set { _selectSerialRecord = value; RaisePropertyChanged("SelectSerialRecord"); } }
		#endregion
		#region 檢驗項目
		private List<F190206CheckItemName> _checkItems;
		public List<F190206CheckItemName> CheckItems { get { return _checkItems; } set { _checkItems = value; RaisePropertyChanged("CheckItems"); } }
		#endregion
		#region 檢驗是否通過
		private string _isPass = "1";
		public string IsPass { get { return _isPass; } set { _isPass = value; RaisePropertyChanged("IsPass"); } }
		#endregion
		#region 按鈕確認刷讀
		private bool _isInputSerailNoClick = false;
		public bool IsInputSerailNoClick { get { return _isInputSerailNoClick; } set { _isInputSerailNoClick = value; RaisePropertyChanged(); } }
		#endregion
		#endregion

		#region 退貨彙總
		#region 彙總單號
		private string _selectedGatherNO = string.Empty;
		/// <summary>
		/// 選取的彙總單號
		/// </summary>
		public string SelectedGatherNO
		{
			get { return _selectedGatherNO; }
			set
			{
				_selectedGatherNO = value;
			}
		}
		private List<NameValuePair<string>> _gatherList;
		/// <summary>
		/// 彙總單號清單
		/// </summary>
		public List<NameValuePair<string>> GatherList
		{
			get { return _gatherList.OrderBy(x => x.Value).ToList(); }
			set { _gatherList = value; RaisePropertyChanged("GatherList"); }
		}
		#endregion
		#region 退貨彙總表明細
		private List<F161502> _returnGatherDetails;
		public List<F161502> ReturnGatherDetails { get { return _returnGatherDetails; } set { _returnGatherDetails = value; RaisePropertyChanged("ReturnGatherDetails"); } }
		#endregion
		#region 箱號(彙總單序號)
		private string _boxNo;
		public string BoxNo
		{
			get { return _boxNo; }
			set
			{
				_boxNo = value;
				RaisePropertyChanged("BoxNo");
				RaisePropertyChanged("IsHasBoxNo");
			}
		}
		#endregion
		#region 是否有箱號
		public bool IsHasBoxNo { get { return !(string.IsNullOrEmpty(BoxNo)); } }
		#endregion
		#endregion


		#region Function

		#region 設定初始值
		public void SetInit()
		{
			NowWorkFlow = WorkFlow.BeforeCheckAndAccept;
			MarqueeMessage = string.Empty;
			//清空顯示畫面
			ClearPresent();
			InputBillNo = string.Empty;
			ReturnRecordMain = null;
			ReturnRecordDetails = null;
			SerialRecords = null;
			InputSerailNo = string.Empty;
			ScanCheckCount = 1;
			CheckItems = null;
			BoxNo = string.Empty;
			ItemImageSource = null;

			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (string.IsNullOrEmpty(SelectedDc))
			{
				SelectedDc = DcList.SelectFirstOrDefault(x => x.Value);
			}
			DoSearchGatherList();
			DoSearchUccList();

			_videoServerHelper = new VideoServerHelper();
		}
		#region 產生彙總單清單
		public void DoSearchGatherList()
		{
			var proxy = GetProxy<F16Entities>();
			var results = proxy.F161501s.Where(x => x.DC_CODE == SelectedDc).Select(x =>
				new NameValuePair<string>() { Name = x.GATHER_NO, Value = x.GATHER_NO }
				).ToList();
			results.Insert(0, new NameValuePair<string>() { Name = string.Empty, Value = string.Empty });
			GatherList = results;
			if (GatherList != null && GatherList.Any())
				SelectedGatherNO = GatherList.FirstOrDefault().Value;
		}
		#endregion
		#region 產生異常原因
		private void DoSearchUccList()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1951s.Where(x => x.UCT_ID.Equals("RC")).ToList();
			result.Insert(0, new F1951() { UCC_CODE = null, UCT_ID = "RC", CAUSE = Properties.Resources.P0802010000_Not });
			UccList = result;
		}
		#endregion
		#region 清空顯示畫面
		public void ClearPresent()
		{
			NotExist = string.Empty;
			IsNoAnySerialNo = false;
			BarCode = string.Empty;
			IsError = string.Empty;
			CheckItems = null;
			RaisePropertyChanged(() => ItemImageSource);
			RaisePropertyChanged(() => IsImageUploaded);
		}
		#endregion
		#endregion

		#region 選擇商品後設定刷讀及檢驗數
		public void SetScanInputValue()
		{
			if (NowWorkFlow == WorkFlow.BeforeCheckAndAccept) return;
			//ClearPresent();
			// 商品項目圖檔
			if (SelectRecordDetail != null)
			{
				//InputSerailNo = SelectRecordDetail.Item.ITEM_CODE;
				ScanCheckCount = (SelectRecordDetail.Item.AUDIT_QTY.Value == 0) ? 1 : SelectRecordDetail.Item.AUDIT_QTY.Value;
				GetCheckItems(SelectRecordDetail.Item.ITEM_CODE);
			}
			else
			{
				if (SelectSerialRecord != null)
				{
					GetCheckItems(SelectSerialRecord.ITEM_CODE);
				}
			}

			RaisePropertyChanged(() => ItemImageSource);
			RaisePropertyChanged(() => IsImageUploaded);
		}
		#endregion

		#region 刪除退貨商品
		public void DelReturnDetail(F161402Data delRow)
		{
			// 確認刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != UILib.Services.DialogResponse.Yes)
				return;

           // 刪除f161402,f16140101
           var proxyEx = GetExProxy<P08ExDataSource>();
            var result = proxyEx.DeleteReturnItem(delRow.DC_CODE, delRow.GUP_CODE, delRow.CUST_CODE, delRow.RETURN_NO, delRow.ITEM_CODE, VideoNo);
            if (!result.IsSuccessed)
            {
                NotExist = result.Message;
                IsError = "error";
                PlayOo();
            }

            //從新取得資料
            ReturnRecordDetails = GetF161402Data();
			RaisePropertyChanged("IsPosting");
			RaisePropertyChanged("IsPending");
		}
		#endregion

		#region 刪除序號刷讀記錄
		public void DelReturnSerial(F16140101Data delRow)
		{
			// 確認刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != UILib.Services.DialogResponse.Yes)
				return;

			// 刪除f161402,f16140101
			var proxyEx = GetExProxy<P08ExDataSource>();
			var result = proxyEx.DeleteReturnSerial(delRow.DC_CODE, delRow.GUP_CODE, delRow.CUST_CODE, delRow.RETURN_NO, delRow.ITEM_CODE, delRow.SERIAL_NO, delRow.LOG_SEQ, VideoNo);
			if (!result.IsSuccessed)
			{
				NotExist = result.Message;
				IsError = "error";
				PlayOo();
			}
            else
            {
                //從新取得資料
                ReturnRecordDetails = GetF161402Data();
                RaisePropertyChanged("IsPosting");
                RaisePropertyChanged("IsPending");
            }
        }
		#endregion

		#region 取得是否可以過帳
		private bool GetIsCanPosting()
		{
			if (!IsPending) return false;

			if (ReturnRecordItems == null || ReturnRecordDetails == null)
				return false;

			//1.需有刷驗數才可按過帳
			return ReturnRecordDetails.Max(x => x.Item.AUDIT_QTY) > 0;
		}
		#endregion
		#endregion

		#region Command

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				var _msg = string.Empty;
				return CreateBusyAsyncCommand(
					o => _msg = DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete(_msg)
					);
			}
		}

		private void DoSearchComplete(string Msg)
		{
			RaisePropertyChanged("IsPosting");
			RaisePropertyChanged("IsPending");
			// 顯示
			//0 錯訊聲音
			if (IsError.ToLower() == "error" && PlaySound)
				PlaySoundHelper.Oo();

			//1 訊息
			NotExist = Msg;

			SetScanInputValue();
		}

		private string DoSearch()
		{
			//清空資料
			ClearPresent();

			//執行查詢動
			string Msg = string.Empty;

			//0. 檢查是否有InputBillNo
			if (string.IsNullOrEmpty(InputBillNo.Trim()))
			{
				IsError = "error";
				Msg = Properties.Resources.P0802010000_InputBillNoIsNull;
				return Msg;
			}

			//1. 取得退貨彙總表明細
			if (!string.IsNullOrEmpty(SelectedGatherNO))
				ReturnGatherDetails = GetReturnGatherDetails();

			//2. 查詢F161201

			//2.1 若InputBillNo開頭為R則查RETURN_NO

			F161201 f161201 = null;
			var inputNo = InputBillNo;
			if (inputNo.StartsWith("R"))
			{
				f161201 = GetF161201ByReturnNo(inputNo);
			}
			else if (inputNo.StartsWith("O") || inputNo.StartsWith("C"))
			{
				//2.2 若InputBillNo開頭為O或C則查SOURCE_NO
				f161201 = GetF161201BySourceNo(inputNo);
			}

			//2.3 若為其它或查不到值則查CUST_ORD_NO
			if (f161201 == null)
				f161201 = GetF161201ByCustOrdNo(inputNo);

			if (f161201 == null)
			{
				IsError = "error";
				Msg = Properties.Resources.P0802010000_BillNoIsNoData;
				return Msg;
			}

			// 輸入有可能是任何單據，最後顯示在話面統一為退貨單號，後面的動作在比對單號時，也才能統一
			InputBillNo = f161201.RETURN_NO;

			//指定退貨主檔
			ReturnRecordMain = f161201;

			

			//3. 先查詢F161402->若查無再查詢F161202
			var resultReturnRecords = GetF161402Data();
			if (resultReturnRecords == null || !resultReturnRecords.Any())
				ReturnRecordDetails = GetF161202(f161201.RETURN_NO);
			else
				ReturnRecordDetails = resultReturnRecords;

			//3.1 設定預設儲位
			ReturnRecordItems = GetF161202(f161201.RETURN_NO);
			if (ReturnRecordItems != null && ReturnRecordItems.Any())
				DefaultLoc = ReturnRecordItems.FirstOrDefault().Item.LOC_CODE;

			//2.4 驗證是否無誤條件為Status != 2 And Status !=9 (2:已結案;9:取消)
			if (f161201.STATUS == "2" || f161201.STATUS == "9")
			{
				IsError = "error";
				var errMsg = (f161201.STATUS == "2") ? Properties.Resources.P0802010000_STATUS2 : Properties.Resources.P0802010000_STATUS9;
				Msg = string.Format(Properties.Resources.P0802010000_StatusMsg, errMsg);
				return Msg;
			}

			//4. 取得哪種種類的單號提示 (F000902取中文說明)
			Msg = GetBillType(f161201.SOURCE_TYPE);

			
			return Msg;
		}

		private List<F161502> GetReturnGatherDetails()
		{
			//取得箱號
			var proxy = GetProxy<F16Entities>();
			var results = proxy.F161502s.Where(x => x.DC_CODE == SelectedDc && x.GATHER_NO == SelectedGatherNO)
				.AsQueryable()
				.ToList();
			return results;
		}

		private string GetBillType(string typeCode)
		{
			var proxy = GetProxy<F00Entities>();
			var results = proxy.F000902s.Where(x => x.SOURCE_TYPE == typeCode)
																	.AsQueryable()
																	.ToList();
			var typeName = string.Empty;
			if (results != null && results.Any())
				typeName = results.FirstOrDefault().SOURCE_NAME;
			return typeName;
		}

		private SelectionList<F161402Data> GetF161202(string _returnNo)
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
			var results = proxyEx.CreateQuery<F161402Data>("GetF161202ReturnDetails")
						.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
						.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
						.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
						.AddQueryOption("returnNo", string.Format("'{0}'", _returnNo))
						.ToSelectionList();
			return results;
		}

		private F161201 GetF161201ByCustOrdNo(string custOrdNo)
		{
			var proxy = GetProxy<F16Entities>();
			var list = proxy.F161201s.Where(x => x.DC_CODE == SelectedDc
												&& x.GUP_CODE == _gupCode
												&& x.CUST_CODE == _custCode
												&& x.CUST_ORD_NO == custOrdNo)
										.ToList();
			// 先取處理中，在取待處理，最後再取結案或取消的退貨單
			var item = list.FirstOrDefault(x => x.STATUS == "1");
			if (item == null)
				item = list.FirstOrDefault(x => x.STATUS == "0");
			if (item == null)
				list.FirstOrDefault();
			return item;
		}

		private F161201 GetF161201BySourceNo(string sourceNo)
		{
			var proxy = GetProxy<F16Entities>();
			var query = from x in proxy.F161201s
						where x.DC_CODE == SelectedDc
						where x.GUP_CODE == _gupCode
						where x.CUST_CODE == _custCode
						where x.SOURCE_NO == sourceNo
						select x;

			// 先取處理中，在取待處理，最後再取結案或取消的退貨單
			var list = query.ToList();
			var item = list.FirstOrDefault(x => x.STATUS == "1");
			if (item == null)
				item = list.FirstOrDefault(x => x.STATUS == "0");
			if (item == null)
				list.FirstOrDefault();
			return item;
		}

		private F161201 GetF161201ByReturnNo(string returnNo)
		{
			var proxy = GetProxy<F16Entities>();
			var item = proxy.F161201s.Where(x => x.DC_CODE == SelectedDc
											&& x.GUP_CODE == _gupCode
											&& x.CUST_CODE == _custCode
											&& x.RETURN_NO == returnNo)
										.FirstOrDefault();
			return item;
		}
		#endregion Search

		#region Add
		public ICommand AddRturnMainCommand
		{
			get
			{
				bool isOk = false;
				return CreateBusyAsyncCommand(
					o => isOk = DoAddReturnMain(), () => UserOperateMode == OperateMode.Query,
					o => {
						if (isOk)
						{
							NowWorkFlow = WorkFlow.CheckAndAccept;
							RaisePropertyChanged("IsPosting");
						}

					}
					);
			}
		}

		private bool DoAddReturnMain()
		{
			//執行新增動作
			if (ReturnRecordMain == null)
			{
				NotExist = Properties.Resources.P0802010000_ReturnRecordMainNotExist;
				return false;
			}

			var result = LoadMasterDetailsData();
			if (result)
			{
				UserOperateMode = OperateMode.Edit;
				if (ReturnRecordDetails?.Any() == true)
				{
					StartVideo();
				}
			}
			return result;
		}

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

			result = _videoServerHelper.VideoStartSessionByCustOrderNo(ReturnRecordMain.RETURN_NO,
																		ReturnRecordMain.RTN_CUST_NAME,
																		ReturnRecordMain.RTN_CUST_CODE,
																		ReturnRecordMain.RETURN_DATE);
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return;
			}
		}

		private bool LoadMasterDetailsData()
		{
			var wcfProxy = GetWcfProxy<Wcf.P08WcfServiceClient>();
			var result = wcfProxy.RunWcfMethod(w => w.LoadMasterDetailsData(ReturnRecordMain.DC_CODE, ReturnRecordMain.GUP_CODE, ReturnRecordMain.CUST_CODE, ReturnRecordMain.RETURN_NO));
			if (!result.IsSuccessed)
			{
				NotExist = result.Message;
				return false;
			}
			//從新取得資料
			ReturnRecordDetails = GetF161402Data();
			return true;
		}

		private SelectionList<F161402Data> GetF161402Data()
		{
			if (ReturnRecordMain == null) return null;
			var proxyEx = GetExProxy<P08ExDataSource>();
			var results = proxyEx.CreateQuery<F161402Data>("GetF161402ReturnDetails")
						.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
						.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
						.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
						.AddQueryOption("returnNo", string.Format("'{0}'", ReturnRecordMain.RETURN_NO))
						.AddQueryOption("auditStaff", string.Format("'{0}'", _staff))
						.AddQueryOption("auditName", string.Format("'{0}'", _staffName)).ToSelectionList();

            // 當重新刷讀序號不更新退貨原因
            if (ReturnRecordDetails!=null)
            {
                for (int i = 0; i < results.Count(); i++)
                {
                    var cause = ReturnRecordDetails.Where(x => x.Item.DC_CODE == results[i].Item.DC_CODE &&
                                     x.Item.GUP_CODE == results[i].Item.GUP_CODE &&
                                     x.Item.CUST_CODE == results[i].Item.CUST_CODE &&
                                     x.Item.RETURN_NO == results[i].Item.RETURN_NO &&
                                     x.Item.LOC_CODE == results[i].Item.LOC_CODE &&
                                     x.Item.ROWNUM == results[i].Item.ROWNUM).Select(x => x.Item.CAUSE).SingleOrDefault();
                    results[i].Item.CAUSE = cause;
                }
            }
           
            return results;
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoEdit()
		{
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
					o => DoCancel(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query,
					o => DoSaveComplete()
					);
			}
		}

		private void DoSaveComplete()
		{
			//從新取得資料
			ReturnRecordDetails = GetF161402Data();
		}

		public void DoSave()
		{
			//執行確認儲存動作

			// 儲存F16140101
			foreach (var item in SerialRecords)
				UpdateF16140101(item);

            var wcfProxy = GetWcfProxy<Wcf.P08WcfServiceClient>();
            var datas = ExDataMapper.MapCollection<F161402Data, Wcf.F161402Data>(ReturnRecordDetails.Select(x=> x.Item).ToList()).ToArray();
            var result = wcfProxy.RunWcfMethod(w => w.UpdateF161402(datas));

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save

		#region SearchItem
		private ICommand _searchItemCommand = null;
		public ICommand SearchItemCommand
		{
			get
			{
				return _searchItemCommand ?? (_searchItemCommand = new RelayCommand(SearchItem));
			}
		}

		private void SearchItem()
		{
			if (!IsPending)
				return;

			if (string.IsNullOrWhiteSpace(InputSerailNo))
				return;

			//語音
			if (PlaySound)
				PlaySoundHelper.Scan();

			var result = DoSearchItem();
            ReturnRecordDetails = GetF161402Data();

            // 紀錄刷讀 Log
            InsertF16140102(new F16140102
			{
				DC_CODE = SelectedDc,
				GUP_CODE = _gupCode,
				CUST_CODE = _custCode,
				RETURN_NO = InputBillNo,
				ITEM_CODE = result.ItemCode,
				ISPASS = result.IsPass,
				SERIAL_NO = result.SerialNo,
				AUDIT_QTY = result.AuditQty,
				AUDIT_STAFF = _staff,
				AUDIT_NAME = _staffName,
				MESSAGE = result.Msg,
				CLIENT_PC = _clientPC,
				VIDEO_NO = SelectedF910501?.VIDEO_ERROR != "1" ? SelectedF910501?.VIDEO_NO : null
			});

			DoSearchItemComplete(result.Msg);

			VideoShowItemByScan(result);
		}

		private void VideoShowItemByScan(Wcf.SearchItemResult result)
		{
			// 錄影機台命令發送
			var videoResult = _videoServerHelper.VideoShowItemByScan(result.ReturnNo,
																	result.CustOrderNo,
																	result.ItemName,
																	result.SerialNo ?? result.ItemCode,
																	result.RtnQty ?? 0,
																	result.AuditQty ?? 0,
																	null,
																	result.IsPass);
			if (!videoResult.IsSuccessed) ShowWarningMessage(videoResult.Message);
		}

		private void DoSearchItemComplete(string msg)
		{
			IsInputSerailNoClick = false;
			RaisePropertyChanged("IsPosting");
			RaisePropertyChanged("IsPending");
			// 顯示
			//0 錯訊聲音
			if (IsError.ToLower() == "error" && PlaySound)
				PlaySoundHelper.Oo();

			// 設定Selected
			var results = ReturnRecordDetails.Where(x => x.Item.ITEM_CODE == InputSerailNo);
			if (results != null && results.Any())
			{
				SelectRecordDetail = results.FirstOrDefault();
				ImgItemCode = SelectRecordDetail.Item.ITEM_CODE;
				SetRecordScroll(SelectRecordDetail);
			}
			else
			{
				var resultsSerial = SerialRecords.Where(x => x.SERIAL_NO == InputSerailNo);
				if (resultsSerial != null && resultsSerial.Any())
				{
					SelectSerialRecord = resultsSerial.FirstOrDefault();
					ImgItemCode = SelectSerialRecord.ITEM_CODE;
					SetSerialRecordScroll(SelectSerialRecord);
				}
			}
			SetScanInputValue();
			//	//2 訊息
			if (!string.IsNullOrEmpty(msg))
			{
				IsError = "error";
				NotExist = msg;
			}

			DispatcherAction(() =>
			{
				//將Focus移至刷讀欄位
				SetInputSerialFocus();
			});
		}
		
		private Wcf.SearchItemResult DoSearchItem()
		{
            var item = ReturnRecordDetails.FirstOrDefault(x => x.Item.ITEM_CODE == InputSerailNo || x.Item.EAN_CODE1 == InputSerailNo || x.Item.EAN_CODE2 == InputSerailNo || x.Item.EAN_CODE3 == InputSerailNo);
            int? addAuditQty = null;
            if (IsInputSerailNoClick)
                addAuditQty = ScanCheckCount;
            var wcfProxy = GetWcfProxy<Wcf.P08WcfServiceClient>();
            var result = wcfProxy.RunWcfMethod(w => w.DoSearchItem(SelectedDc, _gupCode, _custCode, InputBillNo, DefaultLoc, InputSerailNo, addAuditQty,item?.Item.LOC_CODE,item?.Item.CAUSE,item?.Item.MEMO));
            if (result.IsPass == "1" && !string.IsNullOrEmpty(result.Msg))
            {
                DialogService.ShowMessage(result.Msg, WpfClient.Resources.Resources.Information, UILib.Services.DialogButton.OK, UILib.Services.DialogImage.Information);
                result.Msg = string.Empty;
            }
            else if (result.IsPass == "0")
                IsError = "error";
            SelectRecordDetail = ReturnRecordDetails.FirstOrDefault(x=> x.Item.ITEM_CODE == result.ItemCode);
            BoxNo = string.Empty;
            if (SelectRecordDetail != null && !string.IsNullOrEmpty(SelectedGatherNO))
                CheckGather(SelectRecordDetail.Item.ITEM_CODE);
            return result;
		}

		private void CheckGather(string itemCode)
		{
			var proxy = GetProxy<F16Entities>();
			var f161502 = proxy.F161502s.Where(x => x.DC_CODE == SelectedDc && x.GATHER_NO == SelectedGatherNO && x.ITEM_CODE == itemCode).FirstOrDefault();
			if (f161502 == null)
			{
				BoxNo = string.Empty;
				return;
			}

			BoxNo = string.Format(Properties.Resources.P0802010000_BoxNo, f161502.GATHER_SEQ);
		}



		private List<F16140101Data> GetSerialItems()
		{
			if (ReturnRecordMain == null)
				return null;

			// 確認是否有資料
			var proxyEx = GetExProxy<P08ExDataSource>();
			var list = proxyEx.GetSerialItems(ReturnRecordMain.DC_CODE, ReturnRecordMain.GUP_CODE, ReturnRecordMain.CUST_CODE, ReturnRecordMain.RETURN_NO).ToList();
			return list;
		}
		private void GetCheckItems(string _itemCode)
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
			var results = proxyEx.CreateQuery<F190206CheckItemName>("GetCheckItems")
						.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
						.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
						.AddQueryOption("itemCode", string.Format("'{0}'", _itemCode))
						.AddQueryOption("checkType", string.Format("'{0}'", "02"))
						.ToList();
			CheckItems = results;
		}

		#endregion Search

		#region ImportSerialComplete
		public ICommand ImportSerialComplete
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoImportSerial(), () => (UserOperateMode == OperateMode.Query && IsPending),
					o => DoImportSerialComplete()
					);
			}
		}

		private void DoImportSerialComplete()
		{
			RaisePropertyChanged("IsPosting");
			RaisePropertyChanged("IsPending");
		}

		private void DoImportSerial()
		{
			//從新取得資料
			ReturnRecordDetails = GetF161402Data();
		}
		#endregion

		#region Posting
		public ICommand PostingCommand
		{
			get
			{
				string _msg = string.Empty;
				return CreateBusyAsyncCommand(
					o => _msg = DoPosting(), () => (UserOperateMode == OperateMode.Query && IsPending && IsPosting),
					o => DoPostingComplete(_msg)
					);
			}
		}

		private void DoPostingComplete(string msg)
		{
			RaisePropertyChanged("IsPending");
			RaisePropertyChanged("IsPosting");
			if (!string.IsNullOrEmpty(msg))
				NotExist = msg;

			var proxy = GetProxy<F16Entities>();
			var item = proxy.F161201s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.RETURN_NO == InputBillNo).FirstOrDefault();
			if(item !=null && item.STATUS =="2")
			{
				var tempInputBillNo = InputBillNo;
				SetInit();
				InputBillNo = tempInputBillNo;
				UserOperateMode = OperateMode.Query;
				SetTxtInputBillNoFocus();
			}
		}

		private string DoPosting()
		{
			//0. 確認是否要過帳
			if (ShowMessage(Messages.WarningBeforePosting) != UILib.Services.DialogResponse.Yes) return string.Empty;

			// 檢查是否有勾選通過,但有異常訊息,且異常狀況未填
			if (!isValid())
				return Properties.Resources.P0802010000_NoInputErrorDesc;

			if (!IsValidBomItem())
				return "";

			//1. 過帳
			var proxyEx = GetExProxy<P08ExDataSource>();
			var results = proxyEx.CreateQuery<ExecuteResult>("DoPosting")
						.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
						.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
						.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
						.AddQueryOption("returnNo", string.Format("'{0}'", InputBillNo))
						.AddQueryOption("auditStaff", string.Format("'{0}'", _staff))
						.AddQueryOption("auditName", string.Format("'{0}'", _staffName))
						.ToList();

			ReturnRecordDetails = GetF161402Data();

			if (results != null || results.Any())
			{
				var result = results.FirstOrDefault();
				IsError = (result.IsSuccessed) ? string.Empty : "error";
				return result.Message;
			}

			return string.Empty;
		}

		private bool IsValidBomItem()
		{
			var proxy = GetWcfProxy<Wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.CheckReturnBomItemNotFullReturn(SelectedDc, _gupCode, _custCode, InputBillNo));
			if (!result.IsSuccessed)
			{
				//var confirm = DialogService.ShowMessage(result.Message, "", UILib.Services.DialogButton.OKCancel, UILib.Services.DialogImage.Question);
				//if (confirm == UILib.Services.DialogResponse.Cancel)
				//	return false;
				ShowWarningMessage(result.Message);
				return false;
			}
			return true;
		}
		#endregion

		#region ForceClose
		public ICommand ForceCloseCommand
		{
			get
			{
				string msg = string.Empty;
				return CreateBusyAsyncCommand(
					o => msg = DoForceClose(), () => (UserOperateMode == OperateMode.Query && IsPending),
					o => DoPostingComplete(msg)
					);
			}
		}

		private bool isValid()
		{
			IsError = string.Empty;

			var f16140101s = GetSerialItems();
			if (f16140101s != null && f16140101s.Any())
			{
				if (f16140101s.Any(x => x.ISPASS == "1" && x.ISPASS2 == "0" &&
															 string.IsNullOrEmpty(x.ERR_CODE)))
				{
					IsError = "error";
					return false;
				}
			}

			return true;
		}

		private string DoForceClose()
		{
			//0. 確認是否要強制結案
			if (ShowMessage(Messages.WarningBeforeForceClose) != UILib.Services.DialogResponse.Yes) return string.Empty;

			// 檢查是否有勾選通過,但有異常訊息,且異常狀況未填
			if (!isValid())
				return Properties.Resources.P0802010000_NoInputErrorDesc;

			//1. 強制結案
			var proxyEx = GetExProxy<P08ExDataSource>();
			var results = proxyEx.CreateQuery<ExecuteResult>("DoForceClose")
						.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
						.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
						.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
						.AddQueryOption("returnNo", string.Format("'{0}'", InputBillNo))
						.AddQueryOption("auditStaff", string.Format("'{0}'", _staff))
						.AddQueryOption("auditName", string.Format("'{0}'", _staffName))
						.ToList();

			ReturnRecordDetails = GetF161402Data();

			if (results != null || results.Any())
			{
				var result = results.FirstOrDefault();
				IsError = (result.IsSuccessed) ? string.Empty : "error";
				return result.Message;
			}
			return string.Empty;
		}
		#endregion

		#region Help 求救
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
		/// <summary>
		/// 求救, 寫入F0010
		/// </summary>
		private void DoHelp()
		{
			var proxy = GetProxy<F00Entities>();
			var data = new F0010()
			{
				DC_CODE = SelectedDc,
				HELP_TYPE = "04",
				DEVICE_PC = _clientPC,
				ORD_NO = InputBillNo,
				GUP_CODE = _gupCode,
				CUST_CODE = _custCode,
				STATUS = "0"
			};
			proxy.AddToF0010s(data);
			proxy.SaveChanges();
			NotExist = Properties.Resources.P0802010000_Help + NotExist;
		}
		public void DoHelpComplete()
		{

		}
		#endregion

		#region CheckBox 勾選
		private ICommand _checkBoxSetCommand;
		public ICommand CheckBoxSetCommand
		{
			get
			{
				return _checkBoxSetCommand ??
							(_checkBoxSetCommand = CreateBusyAsyncCommand<F16140101Data>(ExecuteCheckBoxSetCommand,
																						 CanExecuteCheckBoxSetCommand));
			}
		}

		private bool CanExecuteCheckBoxSetCommand(F16140101Data f16140101Data)
		{
			if (f16140101Data == null)
				return false;

			if (NowWorkFlow != WorkFlow.CheckAndAccept)
				return false;

			// 若狀態為C2 B2，則表示過帳，就不能再異動了
			if (f16140101Data.ISPASS2 == "2")
				return false;

			return true;
		}

		private void ExecuteCheckBoxSetCommand(F16140101Data f16140101Data)
		{
			if (!CanExecuteCheckBoxSetCommand(f16140101Data))
				return;

			// 驗證含有來源單據的刷讀
			if (!string.IsNullOrEmpty(ReturnRecordMain.SOURCE_NO) && !ReturnRecordDetails.Any(x => x.Item.ITEM_CODE == f16140101Data.ITEM_CODE))
			{
				ShowWarningMessage(Properties.Resources.P0802010000_ReturnRecordMainError);
				SerialRecords = GetSerialItems();
				return;
			}

			if (f16140101Data.ISPASS2 == "0"
				&& string.IsNullOrEmpty(f16140101Data.ERR_CODE)
				&& f16140101Data.ITEM_CODE != "XYZ00001")
			{
				ShowWarningMessage(Properties.Resources.P0802010000_ErrCodeIsNull);
				SerialRecords = GetSerialItems();
				return;
			}

			var proxy = GetWcfProxy<Wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ChangeF16140101IsPass(f16140101Data.Map<F16140101Data, Wcf.F16140101>(), ReturnRecordMain.SOURCE_NO));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
			}

			ReturnRecordDetails = GetF161402Data();
			RaisePropertyChanged(() => IsPosting);
			SelectRecordDetail = ReturnRecordDetails.Where(x => x.Item.ITEM_CODE == f16140101Data.ITEM_CODE).FirstOrDefault();
		}
		#endregion

		#region 印列不明件BarCode
		public ICommand PrintBarCodeCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPrintBarCode(),
					() => true
				);
			}
		}

		private void DoPrintBarCode()
		{
			if (SelectedF910501 == null || string.IsNullOrEmpty(SelectedF910501.LABELING))
			{
				ShowWarningMessage(Properties.Resources.P0802010000_LabelingNotExist);
				return;
			}

			var proxy = new Wcf.P08WcfServiceClient();
			BarCode = RunWcfMethod<string>(proxy.InnerChannel,
				() => proxy.GetPintBarCode("ZN"));

			//Print BarCode			
			var result = new Wms3pl.WpfClient.ExDataServices.ShareExDataService.ExecuteResult() { IsSuccessed = false };
			var printObj = new LabelPrintHelper(FunctionCode);
			var labelItem = new LableItem();
			labelItem.LableCode = "FETLB17";
			labelItem.LableName = "BarCode";
			labelItem.GupCode = _gupCode;
			labelItem.CustCode = _custCode;
			//BarCode
			labelItem.EanCode = BarCode;
			result = printObj.DoPrintNoDevice(labelItem, _selectedDc);
			ShowMessage(result.IsSuccessed ? Messages.PrintSuccess : Messages.PrintFailed);
		}

		#endregion

		#region 退貨商品明細
		private ICommand _clearF161402DataCommand;

		/// <summary>
		/// Gets the ClearF161402DataCommand.
		/// </summary>
		public ICommand ClearF161402DataCommand
		{
			get
			{
				return _clearF161402DataCommand ??
						(_clearF161402DataCommand = CreateBusyAsyncCommand<SelectionItem<F161402Data>>(ExecuteClearF161402DataCommand,
																									   CanExecuteClearF161402DataCommand));
			}
		}

		private void ExecuteClearF161402DataCommand(SelectionItem<F161402Data> si)
		{
			if (!CanExecuteClearF161402DataCommand(si))
			{
				return;
			}

			DelReturnDetail(si.Item);
			SetInputSerialFocus();
		}

		private bool CanExecuteClearF161402DataCommand(SelectionItem<F161402Data> si)
		{
			if (si == null)
				return false;

            // HasNotInReturnItem >0 代表此商品有非退貨單商品
            var f161402Data = si.Item;
            return f161402Data.HasNotInReturnItem > 0;
		}
		#endregion 退貨商品明細

		#region 序號商品明細
		private ICommand _clearF16140101DataCommand;

		/// <summary>
		/// Gets the ClearF16140101DataCommand.
		/// </summary>
		public ICommand ClearF16140101DataCommand
		{
			get
			{
				return _clearF16140101DataCommand ?? (_clearF16140101DataCommand = CreateBusyAsyncCommand<F16140101Data>(
						ExecuteClearF16140101DataCommand));
			}
		}

		private void ExecuteClearF16140101DataCommand(F16140101Data f16140101Data)
		{
			if (!CanExecuteClearF16140101DataCommand(f16140101Data))
				return;

			DelReturnSerial(f16140101Data);
			SetInputSerialFocus();
		}

		private bool CanExecuteClearF16140101DataCommand(F16140101Data f16140101Data)
		{
			if (f16140101Data == null)
				return false;

			// 只有打勾通過，且狀態為C2 B2的不能清除，因為過帳後，會將序號狀態改為C2 B2，而一開始就刷C2 B2的話
			//，是不允許被打勾通過的，故邏輯成立
			return !(f16140101Data.ISPASS == "1" && f16140101Data.ISPASS2 == "2");
		}

		#endregion 序號商品明細

		#endregion

		public void CancelCheck()
		{
			if (IsPending)
			{
				DoSave();

				EndVideo();
			}
			SetInit();
		}

		private void EndVideo()
		{
			var result = _videoServerHelper.VideoEndSession(ReturnRecordMain.RETURN_NO, null, null);
			if (!result.IsSuccessed) ShowWarningMessage(result.Message);
		}

		public void PlayOo()
		{
			if (PlaySound)
				PlaySoundHelper.Oo();
		}


		private void UpdateF16140101(F16140101Data item)
		{
			if (item == null) return;
			var proxy = GetProxy<F16Entities>();
			// 確認是否有資料
			var results = proxy.F16140101s.Where(x => x.DC_CODE == item.DC_CODE && 
													x.GUP_CODE == item.GUP_CODE &&
													x.CUST_CODE == item.CUST_CODE && 
													x.RETURN_NO == item.RETURN_NO &&
													x.ITEM_CODE == item.ITEM_CODE && 
													x.SERIAL_NO == item.SERIAL_NO &&
													x.AUDIT_STAFF == _staff &&
													x.AUDIT_NAME == _staffName)
													.AsQueryable()
													.ToList();
			if (results != null && results.Any())
			{
				var f16140101 = results.FirstOrDefault();
				f16140101.ISPASS = item.ISPASS;
				f16140101.ERR_CODE = item.ERR_CODE;
				proxy.UpdateObject(f16140101);
				proxy.SaveChanges();
			}
		}

		/// <summary>
		/// 檢查退貨檢驗頭檔單據狀態是否為待處理
		/// </summary>
		/// <returns></returns>
		private bool GetIsPending()
		{
			IsError = string.Empty;
			NotExist = string.Empty;
			if (ReturnRecordMain == null) return false;
			var proxy = GetProxy<F16Entities>();
			var result = proxy.F161401s.Where(x => x.DC_CODE == ReturnRecordMain.DC_CODE && x.GUP_CODE == ReturnRecordMain.GUP_CODE &&
																				 x.CUST_CODE == ReturnRecordMain.CUST_CODE && x.RETURN_NO == ReturnRecordMain.RETURN_NO)
																	.AsQueryable()
																	.FirstOrDefault();
			if (result == null)
			{
				return false;
			}

			if (result.STATUS == "2")
			{
				IsError = "error";
				NotExist = Properties.Resources.P0802010000_ReturnStatusIsClose;
				return false;
			}
			return true;
		}

		internal void SerachSerialNo(string target)
		{
			if (SerialRecords == null && !SerialRecords.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
			SelectSerialRecord = SerialRecords.Where(x => x.SERIAL_NO == target).FirstOrDefault();
			if (SelectSerialRecord == null)
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
			ImgItemCode = SelectSerialRecord.ITEM_CODE;
			SetSerialRecordScroll(SelectSerialRecord);
		}

		//#region LO 相關
		//private ExecuteResult AddLoFinishQtyScan(F161402Data f161402Data, string serialNo, int addQty)
		//{
		//	var proxyEx = GetExProxy<P08ExDataSource>();
		//	var results = proxyEx.CreateQuery<ExecuteResult>("AddLoFinishQtyScan")
		//		.AddQueryExOption("gupCode", f161402Data.GUP_CODE)
		//		.AddQueryExOption("custCode", f161402Data.CUST_CODE)
		//		.AddQueryExOption("ticketNo", f161402Data.RETURN_NO)
		//		.AddQueryExOption("itemCode", f161402Data.ITEM_CODE)
		//		.AddQueryExOption("serialNo", serialNo)
		//		.AddQueryExOption("addQty", addQty).ToList();
		//	var result = results.FirstOrDefault();
		//	return result;
		//}
		//#endregion LO 相關

		/// <summary>
		/// 新增 退貨檢驗刷驗紀錄檔
		/// </summary>
		/// <param name="f16140102"></param>
		private void InsertF16140102(F16140102 f16140102)
		{
			var wcfF16140102 = f16140102.Map<F16140102, ExDataServices.P08WcfService.F16140102>();

			var proxy = GetWcfProxy<Wcf.P08WcfServiceClient>();
			proxy.RunWcfMethod(w => w.InsertF16140102(wcfF16140102));
		}


		#region BadSetting
		/// <summary>
		/// Gets the BadSetting.
		/// </summary>
		public ICommand BadSettingCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoBadSetting(),()=> CheckIsClose(), o =>
						{
							var win = new P0802010300();
							win.Vm.Init(ReturnRecordMain.DC_CODE, ReturnRecordMain.GUP_CODE, ReturnRecordMain.CUST_CODE, ReturnRecordMain.RETURN_NO);
							win.ShowDialog();
						}
);
			}
		}

		public void DoBadSetting()
		{

		}
		private bool CheckIsClose()
		{
			if (ReturnRecordMain == null)
				return false;
			var proxy = GetProxy<F16Entities>();
			var item = proxy.F161401s.Where(x => x.DC_CODE == ReturnRecordMain.DC_CODE && x.GUP_CODE == ReturnRecordMain.GUP_CODE && x.CUST_CODE == ReturnRecordMain.CUST_CODE && x.RETURN_NO == ReturnRecordMain.RETURN_NO).FirstOrDefault();
			return item == null ? false : (item.STATUS == "2");
		}
		#endregion BadSetting
	}
}
