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
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.UILib;
using PickContainerResult = Wms3pl.WpfClient.ExDataServices.P08WcfService.PickContainerResult;
using OutContainerResult = Wms3pl.WpfClient.ExDataServices.P08WcfService.OutContainerResult;
using PickContainerPutIntoOutContainerResult = Wms3pl.WpfClient.ExDataServices.P08WcfService.PickContainerPutIntoOutContainerResult;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0808050000_ViewModel : InputViewModelBase
	{
		#region Property

		private string _gupCode;
		private string _custCode;
		public Action DoDcChange = delegate { };
		public Action DoContainerCodeFocus = delegate { };
		public Action DoOutContainerCodeFocus = delegate { };
		public Action DoPutIntoConfirmFocus = delegate { };

		private DeliveryReportService _deliveryReport;

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

		#region 揀貨容器
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

		#region 揀貨容器資訊
		private PickContainerResult currentPickContainerResult;

		public PickContainerResult CurrentPickContainerResult
		{
			get { return currentPickContainerResult; }
			set
			{
				Set(() => CurrentPickContainerResult, ref currentPickContainerResult, value);
			}
		}
		#endregion

		#region 跨庫箱號
		private string _scanOutContainerCode;

		public string ScanOutContainerCode
		{
			get { return _scanOutContainerCode; }
			set
			{
				Set(() => ScanOutContainerCode, ref _scanOutContainerCode, value);
			}
		}
		#endregion

		#region 跨庫箱號資訊
		private OutContainerResult currentOutContainerResult;

		public OutContainerResult CurrentOutContainerResult
		{
			get { return currentOutContainerResult; }
			set
			{
				Set(() => CurrentOutContainerResult, ref currentOutContainerResult, value);
			}
		}
		#endregion

		#endregion

		public P0808050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
				if (DcList.Any())
					SelectedDc = DcList.First().Value;
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				Init();

				_deliveryReport = new DeliveryReportService(FunctionCode);
			}
		}

		public void Init()
		{
			UserOperateMode = OperateMode.Query;
			DispatcherAction(() =>
			{
				ScanContainerCode = string.Empty;
				ScanOutContainerCode = string.Empty;
				CurrentPickContainerResult = null;
				CurrentOutContainerResult = null;
				DoContainerCodeFocus();
				Message = "請刷入揀貨容器";
			});
		}

		#region Search 查詢
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearch()
		{
			CurrentPickContainerResult = null;
			if (string.IsNullOrWhiteSpace(ScanContainerCode))
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage("您未輸入揀貨容器，請刷入揀貨容器");
					DoContainerCodeFocus();
				});
				return;
			}
			ScanContainerCode = ScanContainerCode.Trim().ToUpper();

			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ScanPickContainerCode(SelectedDc, _gupCode, _custCode, ScanContainerCode));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				ScanContainerCode = string.Empty;
				DoContainerCodeFocus();
				Message = "請刷入揀貨容器";
				return;
			}
			CurrentPickContainerResult = result;
			Message = "請刷入跨庫箱號";
			DoOutContainerCodeFocus();
		}
		#endregion Search 查詢

		#region ScanOutContainer 刷入跨庫箱號
		public ICommand ScanOutContainerCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoScanOutContainer(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoScanOutContainer()
		{
			CurrentOutContainerResult = null;
			if (string.IsNullOrWhiteSpace(ScanOutContainerCode))
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage("您未輸入跨庫箱號，請刷入跨庫箱號");
					DoOutContainerCodeFocus();
				});
				return;
			}
			ScanOutContainerCode = ScanOutContainerCode.Trim().ToUpper();

			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ScanOutContainerCode(SelectedDc, _gupCode, _custCode, ScanOutContainerCode));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				ScanOutContainerCode = string.Empty;
				DoOutContainerCodeFocus();
				return;
			}
			CurrentOutContainerResult = result;
			Message = "檢核成功，請按下確定放入";
			DoPutIntoConfirmFocus();
		}
		#endregion ScanOutContainer 刷入跨庫箱號

		#region PutIntoConfirmCommand 確定放入
		public ICommand PutIntoConfirmCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoPutIntoConfirm(),
						() => CurrentPickContainerResult != null && CurrentOutContainerResult != null

				);
			}
		}

		private void DoPutIntoConfirm()
		{
			//(1)	揀貨容器若為空白或無值
			if (string.IsNullOrWhiteSpace(ScanContainerCode))
			{
				DispatcherAction(() =>
				{
					Message = "您未輸入揀貨容器，請刷入揀貨容器";
					ShowWarningMessage("您未輸入揀貨容器，請刷入揀貨容器");
				});
				return;
			}
			//(2)	揀貨容器有值，要去空白
			ScanContainerCode = ScanContainerCode.Trim().ToUpper();
			//(3)	跨庫箱號若為空白或無值
			if (string.IsNullOrWhiteSpace(ScanOutContainerCode))
			{
				DispatcherAction(() =>
				{
					Message = "您未輸入跨庫箱號，請刷入跨庫箱號";
					ShowWarningMessage("您未輸入跨庫箱號，請刷入跨庫箱號");
				});
				return;
			}
			//(4)	跨庫箱號有值，要去空白
			ScanOutContainerCode = ScanOutContainerCode.Trim().ToUpper();
			//(5)	如果揀貨容器資訊無資料
			if (CurrentPickContainerResult == null)
			{
				DispatcherAction(() =>
				{
					DoContainerCodeFocus();
					Message = "未進行揀貨容器綁定與檢核，請於揀貨容器輸入框按下Enter";
					ShowWarningMessage("未進行揀貨容器綁定與檢核，請於揀貨容器輸入框按下Enter");
				});
				return;
			}
			//(6)	如果跨庫箱號資訊無資料
			if (CurrentOutContainerResult == null)
			{
				DispatcherAction(() =>
				{
					DoOutContainerCodeFocus();
					Message = "未進行跨庫箱號綁定與檢核，請於跨庫箱號輸入框按下Enter";
					ShowWarningMessage("未進行跨庫箱號綁定與檢核，請於跨庫箱號輸入框按下Enter");
				});
				return;
			}
			//(7)	如果揀貨容器資訊的揀貨容器不等於刷入的揀貨容器或揀貨容器資訊不存在
			if (CurrentPickContainerResult.ContainerCode != ScanContainerCode)
			{
				DispatcherAction(() =>
				{
					CurrentPickContainerResult = null;
					DoContainerCodeFocus();
					Message = "您輸入的揀貨容器與畫面上揀貨容器資訊不一致，請重新刷入揀貨容器";
					ShowWarningMessage("您輸入的揀貨容器與畫面上揀貨容器資訊不一致，請重新刷入揀貨容器");
				});
				return;
			}
			//(8)	如果跨庫資訊的跨庫箱號不等於刷入的跨庫箱號或跨庫箱號資訊不存在
			if (CurrentOutContainerResult.ContainerCode != ScanOutContainerCode)
			{
				DispatcherAction(() =>
				{
					CurrentOutContainerResult = null;
					DoOutContainerCodeFocus();
					Message = "您輸入的跨庫箱號與畫面的跨庫箱號資訊不一致，請重新刷入跨庫箱號";
					ShowWarningMessage("您輸入的跨庫箱號與畫面的跨庫箱號資訊不一致，請重新刷入跨庫箱號");
				});
				return;
			}

			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.PickContainerPutIntoOutContainer(SelectedDc, _gupCode, _custCode, CurrentPickContainerResult, CurrentOutContainerResult));
			if (!result.IsSuccessed)
			{
				if (result.IsOutContainerError)
				{
					DispatcherAction(() =>
					{
						CurrentOutContainerResult = null;
						DoOutContainerCodeFocus();
						ShowWarningMessage(result.Message);
					});
					return;
				}
				if (result.IsPickContainerError)
				{
					DispatcherAction(() =>
					{
						CurrentPickContainerResult = null;
						DoContainerCodeFocus();
						ShowWarningMessage(result.Message);
					});
					return;
				}
				ShowWarningMessage(result.Message);
				return;
			}

			Message = $"揀貨容器{ CurrentPickContainerResult.ContainerCode }已放入，請刷入下一個揀貨容器";
			CurrentPickContainerResult = null;
			CurrentOutContainerResult = result.UpdateOutContainerResult;
			ScanContainerCode = string.Empty;
			DoContainerCodeFocus();
		}
		#endregion

		#region RePackingBoxCommand 重新裝箱

		public ICommand RePackingBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoRePackingBox(), () => CurrentOutContainerResult != null
				);
			}
		}

		private void DoRePackingBox()
		{
			//(1)	跨庫箱號若為空白或無值
			if (string.IsNullOrWhiteSpace(ScanOutContainerCode))
			{
				DispatcherAction(() =>
				{
					Message = "您未輸入跨庫箱號，請刷入跨庫箱號";
					ShowWarningMessage("您未輸入跨庫箱號，請刷入跨庫箱號");
				});
				return;
			}
			//(2)	跨庫箱號有值，要去空白
			ScanOutContainerCode = ScanOutContainerCode.Trim().ToUpper();
			//(3)	跨庫箱號資訊=null
			if (CurrentOutContainerResult == null)
			{
				DispatcherAction(() =>
				{
					DoOutContainerCodeFocus();
					Message = "未進行跨庫箱號綁定與檢核，請於跨庫箱號輸入框按下Enter";
					ShowWarningMessage("未進行跨庫箱號綁定與檢核，請於跨庫箱號輸入框按下Enter");
				});
				return;
			}
			//(4)	如果跨庫資訊的跨庫箱號不等於刷入的跨庫箱號或跨庫箱號資訊不存在
			if (CurrentOutContainerResult.ContainerCode != ScanOutContainerCode)
			{
				DispatcherAction(() =>
				{
					CurrentOutContainerResult = null;
					DoOutContainerCodeFocus();
					Message = "您輸入的跨庫箱號與畫面的跨庫箱號資訊不一致，請重新刷入跨庫箱號";
					ShowWarningMessage("您輸入的跨庫箱號與畫面的跨庫箱號資訊不一致，請重新刷入跨庫箱號");
				});
				return;
			}
			//(5)	先跳出Confirm視窗[請確認是否將裝箱資料刪除，並將商品放回原本的揀或容器中?] 選[是]，往下執行，選否回到原畫面，游標停留在跨庫箱號輸入框。
			if (ShowConfirmMessage("請確認是否將裝箱資料刪除，並將商品放回原本的揀貨容器中?") != DialogResponse.Yes)
			{
				DoOutContainerCodeFocus();
				return;
			}
			// (6) 執行重新裝箱
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.RePackingBox(CurrentOutContainerResult.OutContainerInfo));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				DoOutContainerCodeFocus();
				return;
			}
			else
			{
				Message = $"跨庫箱號{ CurrentOutContainerResult.OutContainerInfo.OUT_CONTAINER_CODE }已釋放，並與放入此跨庫箱號的揀貨容器解除綁定";
				ScanOutContainerCode = string.Empty;
				CurrentOutContainerResult = null;
				DoOutContainerCodeFocus();
			}
		}
		#endregion

		#region CloseBoxCommand 關箱

		public ICommand CloseBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCloseBox(), () => CurrentOutContainerResult != null && CurrentOutContainerResult.TotalPcs > 0
				);
			}
		}

		private void DoCloseBox()
		{
			//(1)	跨庫箱號若為空白或無值
			if (string.IsNullOrWhiteSpace(ScanOutContainerCode))
			{
				DispatcherAction(() =>
				{
					Message = "您未輸入跨庫箱號，請刷入跨庫箱號";
					ShowWarningMessage("您未輸入跨庫箱號，請刷入跨庫箱號");
				});
				return;
			}
			//(2)	跨庫箱號有值，要去空白
			ScanOutContainerCode = ScanOutContainerCode.Trim().ToUpper();
			//(3)	跨庫箱號資訊=null
			if (CurrentOutContainerResult == null)
			{
				DispatcherAction(() =>
				{
					DoOutContainerCodeFocus();
					Message = "未進行跨庫箱號綁定與檢核，請於跨庫箱號輸入框按下Enter";
					ShowWarningMessage("未進行跨庫箱號綁定與檢核，請於跨庫箱號輸入框按下Enter");
				});
				return;
			}
			//(4)	如果跨庫資訊的跨庫箱號不等於刷入的跨庫箱號或跨庫箱號資訊不存在
			if (CurrentOutContainerResult.ContainerCode != ScanOutContainerCode)
			{
				DispatcherAction(() =>
				{
					CurrentOutContainerResult = null;
					DoOutContainerCodeFocus();
					Message = "您輸入的跨庫箱號與畫面的跨庫箱號資訊不一致，請重新刷入跨庫箱號";
					ShowWarningMessage("您輸入的跨庫箱號與畫面的跨庫箱號資訊不一致，請重新刷入跨庫箱號");
				});
				return;
			}

			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.CloseBox(CurrentOutContainerResult.OutContainerInfo));
			if (!result.IsSuccessed)
			{
				DoOutContainerCodeFocus();
				ShowWarningMessage(result.Message);
				return;
			}
			else
			{
				//A.	列印箱明細(請呼叫跨庫訂單整箱出庫-箱內明細列印)
				PrintBoxDetail();
				Message = $"跨庫箱號{ CurrentOutContainerResult.ContainerCode }關箱成功，請將箱明細放入跨庫箱內";
				ShowInfoMessage($"跨庫箱號{ CurrentOutContainerResult.ContainerCode }關箱成功，請將箱明細放入跨庫箱內");
				Init();
			}
		}
		#endregion

		#region BoxDetail 箱內明細
		public ICommand BoxDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoBoxDetail(), () => UserOperateMode == OperateMode.Query
				);
			}
		}

		private void DoBoxDetail()
		{

		}
		#endregion

		#region PrintBoxDetail 列印箱明細
		private void PrintBoxDetail()
		{
			var exproxy = GetExProxy<P08ExDataSource>();
			var PrintDetailData = exproxy.CreateQuery<P0808050000_PrintData>("GetPrintData")
									.AddQueryExOption("F0531ID", CurrentOutContainerResult.OutContainerInfo.F0531_ID.ToString())
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
							.AddQueryExOption("startDate", CurrentOutContainerResult.OutContainerInfo.CRT_DATE)
							.AddQueryExOption("endDate", CurrentOutContainerResult.OutContainerInfo.CRT_DATE)
							.AddQueryExOption("outContainerCode", CurrentOutContainerResult.OutContainerInfo.OUT_CONTAINER_CODE)
							.AddQueryExOption("workType", "0")
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
		#endregion PrintBoxDetail
	}
}
