using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.UILib;
using BindingPickContainerInfo = Wms3pl.WpfClient.ExDataServices.P08WcfService.BindingPickContainerInfo;
using ContainerPickInfo = Wms3pl.WpfClient.ExDataServices.P08WcfService.ContainerPickInfo;
using OutContainerInfo = Wms3pl.WpfClient.ExDataServices.P08WcfService.OutContainerInfo;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	#region class: SowContainer
	public class SowContainer : InputViewModelBase
	{
		public static SolidColorBrush defaultColor = new SolidColorBrush(Color.FromScRgb(0, 255, 255, 255));
		public static SolidColorBrush bindNormalShipColor = new SolidColorBrush(Color.FromRgb(189, 215, 238));
		public static SolidColorBrush bindCancelOrderColor = new SolidColorBrush(Color.FromRgb(255, 151, 151));

		public SowContainer()
		{
			Init();
		}

		#region 箱號類型
		private BindBoxType _boxType;

		public BindBoxType BoxType
		{
			get { return _boxType; }
			set
			{
				Set(() => BoxType, ref _boxType, value);
			}
		}
		#endregion

		#region 儲格背景顏色
		private Brush _boxColor;

		public Brush BoxColor
		{
			get { return _boxColor; }
			set
			{
				Set(() => BoxColor, ref _boxColor, value);
			}
		}
		#endregion

		private OutContainerInfo _info;
		public OutContainerInfo Info
		{
			get { return _info; }
			set
			{
				Set(() => Info, ref _info, value);
			}
		}

		public void Init()
		{
			Info = null;
			NoBindInit();
		}

		public void NoBindInit()
		{
			BoxColor = defaultColor;
		}

		public void BindInit()
		{
			switch (BoxType)
			{
				case BindBoxType.NormalShipBox:
					BoxColor = bindNormalShipColor;
					break;
				case BindBoxType.CanelOrderBox:
					BoxColor = bindCancelOrderColor;
					break;
			}
		}
	}
	#endregion

	public class P0808060000_ViewModel : InputViewModelBase
	{
		#region Property

		private string _gupCode;
		private string _custCode;
		public Action DoContainerFocus = delegate { };
		public Action DoNormalContainerFocus = delegate { };
		public Action DoCancelContainerFocus = delegate { };
		public Action DoNormalContainerEnable = delegate { };
		public Action DoNormalContainerDisable = delegate { };
		public Action DoCancelContainerEnable = delegate { };
		public Action DoCancelContainerDisable = delegate { };
		public Action DoItemBarCodeEnable = delegate { };
		public Action DoItemBarCodeDisable = delegate { };
		public Action<BindBoxType> DoReBindBox = delegate { };
		public Action DoItemBarCodeFocus = delegate { };
		public Action DoDcChange = delegate { };
		private DeliveryReportService _deliveryReport;
		private bool isLockItemBarcode = false;

		#region 物流中心清單
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				Set(() => DcList, ref _dcList, value);
			}
		}
		#endregion

		#region 選取物流中心編號
		private string _selectedDc;

		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				Set(() => SelectedDc, ref _selectedDc, value);
				if (value != null)
				{
					DispatcherAction(() =>
					{
						DoDcChange();
					});
				}
			}
		}
		#endregion

		#region 訊息
		private string _message;

		public string Message
		{
			get { return _message; }
			set
			{
				Set(() => Message, ref _message, value);
			}
		}
		#endregion

		#region 揀貨容器編號
		private string _scanContainerCode;

		public string ScanContainerCode
		{
			get { return _scanContainerCode; }
			set
			{
				Set(() => ScanContainerCode, ref _scanContainerCode, value);
			}
		}
		#endregion

		#region 容器揀貨資訊
		private BindingPickContainerInfo currentContainerPickInfo;

		public BindingPickContainerInfo CurrentContainerPickInfo
		{
			get { return currentContainerPickInfo; }
			set
			{
				Set(() => CurrentContainerPickInfo, ref currentContainerPickInfo, value);
			}
		}
		#endregion

		#region 正常出貨容器編號
		private string _normalContainerCode;

		public string NormalContainerCode
		{
			get { return _normalContainerCode; }
			set
			{
				Set(() => NormalContainerCode, ref _normalContainerCode, value);
			}
		}
		#endregion

		#region 取消訂單容器編號
		private string _cancelContainerCode;

		public string CancelContainerCode
		{
			get { return _cancelContainerCode; }
			set
			{
				Set(() => CancelContainerCode, ref _cancelContainerCode, value);
			}
		}
		#endregion

		#region 正常出貨容器
		private SowContainer _normalshipBox;

		public SowContainer NormalShipBox
		{
			get { return _normalshipBox; }
			set
			{
				Set(() => NormalShipBox, ref _normalshipBox, value);
			}
		}
		#endregion

		#region 取消訂單容器
		private SowContainer _canelOrderBox;

		public SowContainer CancelOrderBox
		{
			get { return _canelOrderBox; }
			set
			{
				Set(() => CancelOrderBox, ref _canelOrderBox, value);
			}
		}
		#endregion

		#region 刷讀商品條碼/序號
		private string _scanBarCode;

		public string ScanBarCode
		{
			get { return _scanBarCode; }
			set
			{
				Set(() => ScanBarCode, ref _scanBarCode, value);
			}
		}
		#endregion

		#region 品名
		private string _itemName;

		public string ItemName
		{
			get { return _itemName; }
			set
			{
				Set(() => ItemName, ref _itemName, value);
			}
		}
		#endregion
		#endregion

		public P0808060000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
				if (DcList.Any())
					SelectedDc = DcList.First().Value;
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				_deliveryReport = new DeliveryReportService(FunctionCode);
				Init();
			}
		}

		public void Init()
		{
			UserOperateMode = OperateMode.Query;
			DispatcherAction(() =>
			{
				isLockItemBarcode = false;
				ScanContainerCode = string.Empty;
				CurrentContainerPickInfo = null;
				ScanBarCode = string.Empty;
				ItemName = string.Empty;
				NormalContainerCode = string.Empty;
				NormalShipBox = new SowContainer() { BoxType = BindBoxType.NormalShipBox };
				CancelContainerCode = string.Empty;
				CancelOrderBox = new SowContainer() { BoxType = BindBoxType.CanelOrderBox };
				DoContainerFocus();
				DoNormalContainerEnable();
				DoCancelContainerEnable();
				DoItemBarCodeDisable();
				Message = "請刷入揀貨容器";
			});
		}

		#region Search 查詢
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearch(),
						() => UserOperateMode == OperateMode.Query
				);
			}
		}

		private void DoSearch()
		{
			//(1)	檢查物流中心是否有值
			if (string.IsNullOrWhiteSpace(SelectedDc))
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage("請先選擇物流中心");
				});
				return;
			}
			//(2)	檢查揀貨容器是否有值
			if (string.IsNullOrWhiteSpace(ScanContainerCode))
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage("請刷入揀貨容器");
					DoContainerFocus();
				});
				return;
			}
			ScanContainerCode = ScanContainerCode.Trim().ToUpper();
			//(3)	後端處理
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ScanBindingPickContainerCode(SelectedDc, _gupCode, _custCode, ScanContainerCode));
			//回傳失敗
			if (!result.IsSuccessed || result.IsReleaseContainer)
			{
				ShowWarningMessage(result.Message);
				DoContainerFocus();
				return;
			}
			//(4)回傳成功
			UserOperateMode = OperateMode.Edit;
			CurrentContainerPickInfo = result.BindingPickContainerInfo;
			NormalContainerCode = string.Empty;
			NormalShipBox.Info = null;
			CancelContainerCode = string.Empty;
			CancelOrderBox.Info = null;

			if(result.BindingPickContainerInfo.ALL_CP_ITEM == "1")
			{
				DoNormalContainerDisable();
				DoCancelContainerEnable();
				DoCancelContainerFocus();
				Message = "揀貨容器全部為取消訂單商品，請刷入取消訂單容器條碼";
			}
			else
			{
				DoNormalContainerEnable();
				DoCancelContainerDisable();
				DoNormalContainerFocus();
				Message = "請刷入跨庫箱號";
			}

			
		}
		#endregion Search

		#region StopAllot 暫停分貨
		public ICommand StopAllotCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoStopAllot(),
						() => UserOperateMode != OperateMode.Query
				);
			}
		}

		private void DoStopAllot()
		{
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.StopAllot(CurrentContainerPickInfo.F0701_ID));
			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);
			else
				Init();
		}
		#endregion StopAllot

		#region ScanNormalContainer 刷讀跨庫箱號(正常出貨)
		/// <summary>
		/// Gets the ScanNormalContainer
		/// </summary>
		public ICommand ScanNormalContainerCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoScanNormalContainer(),
						() => true //UserOperateMode != OperateMode.Query && NormalShipBox.Info == null
				);
			}
		}

		public void DoScanNormalContainer()
		{
			//A.	若無值，訊息保持顯示[請刷入跨庫箱號]，不往下處理
			if (string.IsNullOrWhiteSpace(NormalContainerCode))
			{
				Message = "請刷入跨庫箱號";
				return;
			}
			//B.	若有值，將跨庫箱號去除空白並轉大寫後，呼叫後端處理
			NormalContainerCode = NormalContainerCode.Trim().ToUpper();
			//(2)	後端處理
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ScanNormalContainerCode(SelectedDc, _gupCode, _custCode, NormalContainerCode, CurrentContainerPickInfo));
			//回傳失敗
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				DoNormalContainerFocus();
				return;
			}
			NormalShipBox.Info = result.OutContainerInfo;
			DoNormalContainerDisable();
			if (CurrentContainerPickInfo != null)
			{
				if (CurrentContainerPickInfo.HAS_CP_ITEM == "1" && CancelOrderBox.Info == null)
				{
					DoCancelContainerEnable();
					DoCancelContainerFocus();
					Message = "揀貨容器含有取消訂單商品，請刷入取消訂單容器條碼";
				}
				else
				{
					DoItemBarCodeEnable();
					DoItemBarCodeFocus();
					Message = "請刷讀商品條碼";
				}
			}
		}
		#endregion ScanNormalContainer

		#region ScanCancelContainer 刷讀跨庫箱號(取消訂單)
		/// <summary>
		/// Gets the ScanCancelContainer
		/// </summary>
		public ICommand ScanCancelContainerCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoScanCancelContainer(),
						() => UserOperateMode != OperateMode.Query
								&& CancelOrderBox == null
				);
			}
		}

		public void DoScanCancelContainer()
		{
			//A.	若無值，訊息保持顯示[請刷入跨庫箱號]，不往下處理
			if (string.IsNullOrWhiteSpace(CancelContainerCode))
			{
				Message = "揀貨容器含有取消訂單商品，請刷入取消訂單容器條碼";
				return;
			}
			//B.	若有值，將跨庫箱號去除空白並轉大寫後，呼叫後端處理
			CancelContainerCode = CancelContainerCode.Trim().ToUpper();
			//(2)	後端處理
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ScanCancelContainerCode(SelectedDc, _gupCode, _custCode, CancelContainerCode, CurrentContainerPickInfo));
			//回傳失敗
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				DoCancelContainerFocus();
				return;
			}
			CancelOrderBox.Info = result.OutContainerInfo;
			DoCancelContainerDisable();
			if (CurrentContainerPickInfo != null)
			{
				DoItemBarCodeEnable();
				DoItemBarCodeFocus();
				Message = "請刷讀商品條碼";
			}
		}
		#endregion ScanNormalShipBox

		#region ScanItemBarCode 刷讀品號/國條/商品序號
		/// <summary>
		/// Gets the ScanItemBarCode.
		/// </summary>
		public ICommand ScanItemBarCodeCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoScanItemBarCode(),
						() => true //UserOperateMode != OperateMode.Query && NormalShipBox.Info != null && CancelOrderBox.Info != null,
				);
			}
		}

		public void DoScanItemBarCode()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(ScanBarCode))
				{
					Message = "請刷讀商品條碼";
					return;
				}
				if (isLockItemBarcode) return;
				isLockItemBarcode = true;

				ScanBarCode = ScanBarCode.Trim().ToUpper();
				var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
				var result = proxy.RunWcfMethod(w => w.ScanItemBarcodeFromP080806(SelectedDc, _gupCode, _custCode, ScanBarCode,
																				currentContainerPickInfo, NormalShipBox.Info, CancelOrderBox.Info));
				if (!result.IsSuccessed)
				{
					NormalShipBox.NoBindInit();
					CancelOrderBox.NoBindInit();
					ScanBarCode = string.Empty;
					ShowWarningMessage(result.Message);
					if (result.bindNewNormalContainer)
					{
						Message = "請刷入跨庫箱號";
						NormalShipBox.Info = null;
						DoNormalContainerFocus();
					}
					else if (result.bindNewCancelContainer)
					{
						Message = "取消訂單容器已關箱或出貨，請重新綁定新的容器";
						CancelOrderBox.Info = null;
						DoCancelContainerFocus();
					}
					DoItemBarCodeFocus();
					return;
				}
				CurrentContainerPickInfo = result.BindingPickContainerInfo;
				ItemName = result.ITEM_NAME;
				if (result.IsNormalShipItem)
				{
					NormalShipBox.BindInit();
					NormalShipBox.Info.TOTAL += 1;
				}
				else
				{
					CancelOrderBox.BindInit();
					CancelOrderBox.Info.TOTAL += 1;
				}
				ScanBarCode = string.Empty;
				DoItemBarCodeFocus();

				if (result.IsFinishAllot)
				{
					ShowInfoMessage("揀貨容器已分貨完成");
					Init();
				}
			}
			finally
			{
				isLockItemBarcode = false;
			}
		}
		#endregion ScanItemBarCode

		#region ContainerFinish 容器完成
		/// <summary>
		/// Gets the ContainerFinish.
		/// </summary>
		public ICommand ContainerFinishCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoContainerFinish(),
						() => UserOperateMode != OperateMode.Query
				);
			}
		}

		public void DoContainerFinish()
		{
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var notAllotDatas = proxy.RunWcfMethod(w => w.GetNotAllotDataInPickContainer(currentContainerPickInfo.F0701_ID));
			StringBuilder notAllotMsg = new StringBuilder();
			notAllotMsg.AppendLine("以下商品未完成分貨，請問確認揀貨容器已無商品?");
			foreach (var notAllotData in notAllotDatas)
			{
				notAllotMsg.AppendLine($"品號{notAllotData.ITEM_CODE} 品名:{notAllotData.ITEM_NAME} 未分貨數:{notAllotData.NOALLOT_QTY}");
			}

			if (ShowConfirmMessage(notAllotMsg.ToString()) == UILib.Services.DialogResponse.No)
			{
				DoItemBarCodeFocus();
				return;
			}
			else
			{
				var result = proxy.RunWcfMethod(w => w.ManualContainerFinish(SelectedDc, _gupCode, _custCode, currentContainerPickInfo));
				if (!result.IsSuccessed)
				{
					ShowWarningMessage(result.Message);
					DoItemBarCodeFocus();
				}
				else
				{
					ShowInfoMessage("揀貨容器已分貨完成");
					Init();
				}
			}
		}
		#endregion ContainerFinish

		#region CloseNormalShipBox 關箱(正常出貨)
		/// <summary>
		/// Gets the CloseNormalShipBox.
		/// </summary>
		public ICommand CloseNormalShipBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCloseNormalShipBox(),
						() => NormalShipBox != null
								&& NormalShipBox.Info != null
								&& NormalShipBox.Info.TOTAL > 0
				);
			}
		}

		public void DoCloseNormalShipBox()
		{
			if (string.IsNullOrWhiteSpace(NormalContainerCode))
			{
				Message = "您未輸入跨庫箱號，請刷入跨庫箱號";
				return;
			}
			NormalContainerCode = NormalContainerCode.Trim();

			if (NormalShipBox.Info == null)
			{
				Message = "未進行跨庫箱號檢核，請於跨庫箱號輸入框按下Enter";
				DoNormalContainerFocus();
				return;
			}

			if (NormalShipBox.Info.OUT_CONTAINER_CODE != NormalContainerCode)
			{
				NormalShipBox.Info = null;
				Message = "您輸入的跨庫箱號與畫面的跨庫箱號資訊不一致，請重新刷入跨庫箱號";
				DoNormalContainerFocus();
				return;
			}

			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();

			var result = proxy.RunWcfMethod(w => w.CloseNormalContainer(NormalShipBox.Info, CurrentContainerPickInfo));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				DoNormalContainerFocus();
			}
			else
			{
				//列印箱明細
				PrintNormalBoxDetail();
				ShowInfoMessage($"跨庫箱號{ NormalShipBox.Info.OUT_CONTAINER_CODE }關箱完成");
				DoItemBarCodeDisable();
				NormalShipBox.NoBindInit();
				NormalShipBox.Info = null;
				NormalContainerCode = string.Empty;
				DoNormalContainerEnable();
				DoNormalContainerFocus();
				Message = "請刷入跨庫箱號";
			}
		}
		#endregion CloseNormalShipBox

		#region CloseCancelOrderBox 關箱(訂單取消)
		/// <summary>
		/// Gets the CloseCancelOrderBox.
		/// </summary>
		public ICommand CloseCancelOrderBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCloseCancelOrderBox(),
						() => CancelOrderBox != null
								&& CancelOrderBox.Info != null
								&& CancelOrderBox.Info.TOTAL > 0
				);
			}
		}

		public void DoCloseCancelOrderBox()
		{
			if (string.IsNullOrWhiteSpace(CancelContainerCode))
			{
				Message = "您未輸入容器條碼，請刷入容器條碼";
				return;
			}
			CancelContainerCode = CancelContainerCode.Trim().ToUpper();

			if (CancelOrderBox.Info == null)
			{
				Message = "未進行取消訂單容器條碼檢核，請於容器條碼輸入框按下Enter";
				DoCancelContainerFocus();
				return;
			}

			if (CancelOrderBox.Info.OUT_CONTAINER_CODE != CancelContainerCode)
			{
				CancelOrderBox.Info = null;
				Message = "您輸入的容器條碼與畫面的容器條碼資訊不一致，請重新刷入容器條碼";
				DoCancelContainerFocus();
				return;
			}

			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();

			var result = proxy.RunWcfMethod(w => w.CloseCancelContainer(CancelOrderBox.Info, CurrentContainerPickInfo));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				DoCancelContainerFocus();
			}
			else
			{
				//列印箱明細
				PrintCancelBoxDetail();
				ShowInfoMessage($"容器條碼{ CancelOrderBox.Info.OUT_CONTAINER_CODE }關箱成功，請移至異常區");
        CancelOrderBox.NoBindInit();
        CancelOrderBox.Info = null;
        CancelContainerCode = string.Empty;
        if (UserOperateMode == OperateMode.Query)
        {
          DoCancelContainerEnable();
          DoContainerFocus();
				}
				else
				{
					if (result.No == "1") //揀貨容器還有取消訂單商品
					{
						DoItemBarCodeDisable();
						DoCancelContainerEnable();
						DoCancelContainerFocus();
						Message = "揀貨容器含有取消訂單商品，請刷入取消訂單容器條碼";
					}
					else // result.No==2  揀貨容器已無取消訂單商品
					{
						CurrentContainerPickInfo.HAS_CP_ITEM = "0"; // 設定揀貨容器為無取消訂單商品
						DoCancelContainerDisable();
						DoItemBarCodeFocus();
						Message = "請刷讀商品條碼";
					}
				}
				
			}
		}
		#endregion CloseCancelOrderBox

		#region RebindNormalShipBox 重綁箱號(正常出貨)
		/// <summary>
		/// Gets the RebindNormalShipBox.
		/// </summary>
		public ICommand RebindNormalShipBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoRebindNormalShipBox(),
						() => UserOperateMode != OperateMode.Query
								&& NormalShipBox.Info != null
								&& NormalShipBox.Info.TOTAL == 0
								&& CurrentContainerPickInfo != null
				);
			}
		}

		public void DoRebindNormalShipBox()
		{
			DispatcherAction(() =>
			{
				DoReBindBox(BindBoxType.NormalShipBox);
			});
		}
		#endregion RebindNormalShipBox

		#region RebindCancelOrderBox 重綁箱號(訂單取消)
		/// <summary>
		/// Gets the RebindCancelOrderBox.
		/// </summary>
		public ICommand RebindCancelOrderBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoRebindCancelOrderBox(),
						() => UserOperateMode != OperateMode.Query
								&& CancelOrderBox.Info != null
								&& CancelOrderBox.Info.TOTAL == 0
								&& CurrentContainerPickInfo != null
				);
			}
		}

		public void DoRebindCancelOrderBox()
		{
			DispatcherAction(() =>
			{
				DoReBindBox(BindBoxType.CanelOrderBox);
			});
		}
		#endregion RebindCancelOrderBox

		#region BoxDetail 箱內明細
		public ICommand BoxDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoBoxDetail(),
						() => true
				);
			}
		}

		private void DoBoxDetail()
		{

		}
		#endregion


		#region PrintBoxDetail 列印箱明細
		public void PrintNormalBoxDetail()
		{
			var exproxy = GetExProxy<P08ExDataSource>();
			var PrintDetailData = exproxy.CreateQuery<P0808050000_PrintData>("GetPrintData")
									.AddQueryExOption("F0531ID", NormalShipBox.Info.F0531_ID.ToString())
									.ToList();

			if (!PrintDetailData.Any())
			{
				ShowWarningMessage("查無列印資料");
				return;
			}

			var exProxy = GetExProxy<P08ExDataSource>();
			var f0532Exs = exProxy.CreateQuery<F0532Ex>("GetF0532Ex")
							.AddQueryExOption("dcCode", SelectedDc)
							.AddQueryExOption("gupCode", _gupCode)
							.AddQueryExOption("custCode", _custCode)
							.AddQueryExOption("startDate", NormalShipBox.Info.CRT_DATE)
							.AddQueryExOption("endDate", NormalShipBox.Info.CRT_DATE)
							.AddQueryExOption("outContainerCode", NormalShipBox.Info.OUT_CONTAINER_CODE)
							.AddQueryExOption("containerSowType", "0")
							.AddQueryExOption("workType", "1")
							.ToObservableCollection();

			if (f0532Exs != null && f0532Exs.Any())
			{
				DispatcherAction(() =>
				{
					var deliveryReport = new Services.DeliveryReportService(FunctionCode);
					deliveryReport.P080805PrintBoxData(f0532Exs.First(), PrintDetailData);
				});
			}
			else
			{
				ShowWarningMessage("取得箱明細表頭失敗");
				return;
			}
		}
		public void PrintCancelBoxDetail()
		{
			var exproxy = GetExProxy<P08ExDataSource>();
			var PrintDetailData = exproxy.CreateQuery<P0808050000_CancelPrintData>("GetCancelPrintData")
									.AddQueryExOption("F0531ID", CancelOrderBox.Info.F0531_ID.ToString())
									.ToList();

			if (!PrintDetailData.Any())
			{
				ShowWarningMessage("查無列印資料");
				return;
			}

			var exProxy = GetExProxy<P08ExDataSource>();
			var f0532Exs = exProxy.CreateQuery<F0532Ex>("GetF0532Ex")
							.AddQueryExOption("dcCode", SelectedDc)
							.AddQueryExOption("gupCode", _gupCode)
							.AddQueryExOption("custCode", _custCode)
							.AddQueryExOption("startDate", CancelOrderBox.Info.CRT_DATE)
							.AddQueryExOption("endDate", CancelOrderBox.Info.CRT_DATE)
							.AddQueryExOption("outContainerCode", CancelOrderBox.Info.OUT_CONTAINER_CODE)
							.AddQueryExOption("containerSowType", "1")
							.AddQueryExOption("workType", "1")
							.ToObservableCollection();

			if (f0532Exs != null && f0532Exs.Any())
			{
				DispatcherAction(() =>
				{
					var deliveryReport = new Services.DeliveryReportService(FunctionCode);
					deliveryReport.P080805PrintCancelBoxData(f0532Exs.First(), PrintDetailData);
				});
			}
			else
			{
				ShowWarningMessage("取得箱明細表頭失敗");
				return;
			}
		}
		#endregion PrintBoxDetail

	}
}
