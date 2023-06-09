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
using ContainerPickInfo = Wms3pl.WpfClient.ExDataServices.P08WcfService.ContainerPickInfo;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{

	public class SowBox : InputViewModelBase
	{
		public static SolidColorBrush defaultColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
		public static SolidColorBrush bindNormalShipColor = new SolidColorBrush(Color.FromRgb(189, 215, 238));
		public static SolidColorBrush bindCancelOrderColor = new SolidColorBrush(Color.FromRgb(255, 151,151));
	

		public SowBox()
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

		#region 箱號
		private string _boxNo;

		public string BoxNo
		{
			get { return _boxNo; }
			set
			{
				Set(() => BoxNo, ref _boxNo, value);
			}
		}
		#endregion

		#region 累積數量
		private int _sowQty;

		public int SowQty
		{
			get { return _sowQty; }
			set
			{
				Set(() => SowQty, ref _sowQty, value);
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

		public void NoBindInit()
		{
			BoxColor = defaultColor;
		}

		public void BindInit()
		{
			switch(BoxType)
			{
				case BindBoxType.NormalShipBox:
					BoxColor = bindNormalShipColor;
					break;
				case BindBoxType.CanelOrderBox:
					BoxColor = bindCancelOrderColor;
					break;
			}
		}

		/// <summary>
		/// 初始化設定
		/// </summary>
		public void Init()
		{
			NoBindInit();
			BoxNo = string.Empty;
			SowQty = 0;
		}

	}

	public class P0808040000_ViewModel : InputViewModelBase
	{
		#region Property

		private string _gupCode;
		private string _custCode;
		public Action DoBindBox = delegate { };
		public Action<BindBoxType> DoAddBox = delegate { };
		public Action<BindBoxType> DoReBindBox = delegate { };
		public Action DoContainerFocus = delegate { };
		public Action DoItemBarCodeFocus = delegate { };
		public Action DoDcChange = delegate { };
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

		#region 容器編號
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


		#region 是否為最後一箱
		private bool _isPickLastBox;

		public bool IsPickLastBox
		{
			get { return _isPickLastBox;  }
			set
			{
				Set(() => IsPickLastBox, ref _isPickLastBox, value);
			}
		}
		#endregion


		#region 正常出貨箱子
		private SowBox _normalshipBox;

		public SowBox NormalShipBox
		{
			get { return _normalshipBox; }
			set
			{
				Set(() => NormalShipBox, ref _normalshipBox, value);
			}
		}
		#endregion


		#region 取消訂單箱子
		private SowBox _canelOrderBox;

		public SowBox CancelOrderBox
		{
			get { return _canelOrderBox; }
			set
			{
				Set(() => CancelOrderBox, ref _canelOrderBox, value);
			}
		}
		#endregion


		#region 容器揀貨資訊
		private ContainerPickInfo currentContainerPickInfo;

		public ContainerPickInfo CurrentContainerPickInfo
		{
			get { return currentContainerPickInfo; }
			set
			{
				Set(() => CurrentContainerPickInfo, ref currentContainerPickInfo, value);
			}
		}
		#endregion


		#endregion

		public P0808040000_ViewModel()
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
				NormalShipBox = new SowBox() { BoxType = BindBoxType.NormalShipBox };
				CancelOrderBox = new SowBox() { BoxType = BindBoxType.CanelOrderBox };
				CurrentContainerPickInfo = null;
				ItemName = string.Empty;
				ScanBarCode = string.Empty;
				IsPickLastBox = false;
				DoContainerFocus();
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
			if (string.IsNullOrWhiteSpace(ScanContainerCode))
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage(Properties.Resources.P0808040000_ScanContainerCode);
					DoContainerFocus();
				});
				return;
			}

			ScanContainerCode = ScanContainerCode.ToUpper();
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ScanContainerCode(SelectedDc, _gupCode, _custCode, ScanContainerCode));
			if(!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				Init();
				return;
			}
			CurrentContainerPickInfo = result.ContainerPickInfo;
			IsPickLastBox = result.IsPickLastBox;
			UserOperateMode = OperateMode.Edit;

			DispatcherAction(() =>
			{
				if (result.NormalBox != null)
				{
					NormalShipBox.Init();
					NormalShipBox.BoxNo = result.NormalBox.BoxNo;
					NormalShipBox.SowQty = result.NormalBox.SowQty;
				}
				if (result.CancelBox != null)
				{
					CancelOrderBox.Init();
					CancelOrderBox.BoxNo = result.CancelBox.BoxNo;
					CancelOrderBox.SowQty = result.CancelBox.SowQty;
				}
				if (result.NormalBox == null || result.CancelBox == null)
				{

					DoBindBox();
				}
				else
					DoItemBarCodeFocus();
			});
		
		}
		#endregion Search

		#region StopAllot 暫停分貨
		public ICommand StopAllotCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoStopAllot(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoStopAllot()
		{
			Init();
		}
		#endregion StopAllot

		#region AddNormalShipBox 加箱(正常出貨)
		/// <summary>
		/// Gets the AddNormalShipBox.
		/// </summary>
		public ICommand AddNormalShipBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAddNormalShipBox(), () => UserOperateMode != OperateMode.Query && NormalShipBox!=null && NormalShipBox.SowQty > 0
);
			}
		}

		public void DoAddNormalShipBox()
		{
			DispatcherAction(() =>
			{
				DoAddBox(BindBoxType.NormalShipBox);
			});
		}
		#endregion AddNormalShipBox

		#region RebindNormalShipBox 重綁箱號(正常出貨)
		/// <summary>
		/// Gets the RebindNormalShipBox.
		/// </summary>
		public ICommand RebindNormalShipBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoRebindNormalShipBox(), () => UserOperateMode != OperateMode.Query && NormalShipBox!=null && NormalShipBox.SowQty == 0
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

		#region AddCancelOrderBox 加箱(取消訂單)
		/// <summary>
		/// Gets the AddCancelOrderBox.
		/// </summary>
		public ICommand AddCancelOrderBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAddCancelOrderBox(), () => UserOperateMode != OperateMode.Query && CancelOrderBox!=null && CancelOrderBox.SowQty > 0 && CurrentContainerPickInfo.CancelOrderCnt>0
);
			}
		}

		public void DoAddCancelOrderBox()
		{
			DispatcherAction(() =>
			{
				DoAddBox(BindBoxType.CanelOrderBox);
			});
		}
		#endregion AddCancelOrderBox

		#region RebindCancelOrderBox 重綁箱號(取消訂單)
		/// <summary>
		/// Gets the RebindCancelOrderBox.
		/// </summary>
		public ICommand RebindCancelOrderBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoRebindCancelOrderBox(), () => UserOperateMode != OperateMode.Query && CancelOrderBox!=null && CancelOrderBox.SowQty == 0 && CurrentContainerPickInfo !=null && CurrentContainerPickInfo.CancelOrderCnt > 0
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


		#region ScanItemBarCode 刷讀品號/國條/商品序號
		/// <summary>
		/// Gets the ScanItemBarCode.
		/// </summary>
		public ICommand ScanItemBarCodeCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoScanItemBarCode(), () => UserOperateMode != OperateMode.Query,
						o => { },
						o => {},
						() =>
						{
							NormalShipBox.NoBindInit();
							CancelOrderBox.NoBindInit();
						}
);
			}
		}

		public void DoScanItemBarCode()
		{
			ScanContainerCode = ScanContainerCode.ToUpper();
			ScanBarCode = ScanBarCode.ToUpper();
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.P080804SowItem(SelectedDc, _gupCode, _custCode, currentContainerPickInfo.PickOrdNo, ScanContainerCode,ScanBarCode,NormalShipBox.BoxNo, CancelOrderBox.BoxNo));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				DispatcherAction(() =>
				{
					DoItemBarCodeFocus();
				});
				return;
			}
			var normalBoxDetails = new List<P0808040100_PrintData>();
			var cancelBoxDetails = new List<P0808040100_PrintData>();
			if (result.IsBatchFinished)
			{
				// 取得正常出貨箱明細
				if (!string.IsNullOrWhiteSpace(NormalShipBox.BoxNo))
					normalBoxDetails = GetPrintBoxDetail(BindBoxType.NormalShipBox, NormalShipBox.BoxNo);

				// 列印取消訂單箱明細
				if (!string.IsNullOrWhiteSpace(CancelOrderBox.BoxNo))
					cancelBoxDetails = GetPrintBoxDetail(BindBoxType.CanelOrderBox, CancelOrderBox.BoxNo);
			}

			DispatcherAction(() =>
			{
				ItemName = result.ItemName;
				switch (result.BoxInfo.SowType)
				{
					case "01": //正常出貨
						NormalShipBox.SowQty = result.BoxInfo.SowQty;
						NormalShipBox.BindInit();
						break;
					case "02": //取消訂單
						CancelOrderBox.SowQty = result.BoxInfo.SowQty;
						CancelOrderBox.BindInit();
						break;
				}
				if(result.IsBatchFinished)
				{
					// 列印正常出貨箱明細
					if (!string.IsNullOrWhiteSpace(NormalShipBox.BoxNo))
						DoPrintBoxDetail(BindBoxType.NormalShipBox, normalBoxDetails);

					// 列印取消訂單箱明細
					if (!string.IsNullOrWhiteSpace(CancelOrderBox.BoxNo))
						DoPrintBoxDetail(BindBoxType.CanelOrderBox, cancelBoxDetails);

					if (ShowInfoMessage(Properties.Resources.P0808040000_ContainerAllotFinish) == UILib.Services.DialogResponse.OK)
					{
						var msg = string.Format(Properties.Resources.P0808040000_BatchFinished,
							currentContainerPickInfo.DelvDate.ToString("yyyy/MM/dd"),
							currentContainerPickInfo.PickTime,
							currentContainerPickInfo.MoveOutTargetName,
							Environment.NewLine);
						if(ShowInfoMessage(msg) == UILib.Services.DialogResponse.OK)
						{
							Init();
						}
						
					}
				}
				else if(result.IsContainerFinished)
				{
					if(ShowInfoMessage(Properties.Resources.P0808040000_ContainerAllotFinish) == UILib.Services.DialogResponse.OK)
					{
						Init();
					}
				}
				else
					DoItemBarCodeFocus();
			});

		}
		#endregion ScanItemBarCode


		#region OutOfStockShip 缺品出貨
		/// <summary>
		/// Gets the OutOfStockShip.
		/// </summary>
		public ICommand OutOfStockShipCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoOutOfStockShip(), () => UserOperateMode != OperateMode.Query
);
			}
		}

		public void DoOutOfStockShip()
		{
			ScanContainerCode = ScanContainerCode.ToUpper();
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.GetPickLackItems(SelectedDc, _gupCode, _custCode, currentContainerPickInfo.PickOrdNo));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return;
			}
			var msgList = new List<string>();
			msgList.Add(Properties.Resources.P0808040000_LackConfirm);
			foreach (var item in result.LackItemDetails)
				msgList.Add(string.Format(Properties.Resources.P0808040000_LackItemInfo, item.ItemCode, item.ItemName, item.LackQty));

			if (ShowConfirmMessage(string.Join(Environment.NewLine, msgList)) == UILib.Services.DialogResponse.Yes)
			{
				var result2 = proxy.RunWcfMethod(w => w.PickOutOfStockComfirm(SelectedDc, _gupCode, _custCode, currentContainerPickInfo.PickOrdNo,ScanContainerCode));
				if (!result2.IsSuccessed)
				{
					ShowWarningMessage(result2.Message);
					DispatcherAction(() =>
					{
						DoItemBarCodeFocus();
					});
					return;
				}

				var normalBoxDetails = new List<P0808040100_PrintData>();
				var cancelBoxDetails = new List<P0808040100_PrintData>();
				if (result2.IsBatchFinished)
				{
					// 取得正常出貨箱明細
					if (!string.IsNullOrWhiteSpace(NormalShipBox.BoxNo))
						normalBoxDetails = GetPrintBoxDetail(BindBoxType.NormalShipBox, NormalShipBox.BoxNo);

					// 列印取消訂單箱明細
					if (!string.IsNullOrWhiteSpace(CancelOrderBox.BoxNo))
						cancelBoxDetails = GetPrintBoxDetail(BindBoxType.CanelOrderBox, CancelOrderBox.BoxNo);
				}

				DispatcherAction(() =>
				{
					if (result2.IsBatchFinished)
					{
						// 列印正常出貨箱明細
						if (!string.IsNullOrWhiteSpace(NormalShipBox.BoxNo))
							DoPrintBoxDetail(BindBoxType.NormalShipBox, normalBoxDetails);

						// 列印取消訂單箱明細
						if (!string.IsNullOrWhiteSpace(CancelOrderBox.BoxNo))
							DoPrintBoxDetail(BindBoxType.CanelOrderBox, cancelBoxDetails);

						if (ShowInfoMessage(Properties.Resources.P0808040000_ContainerAllotFinish) == UILib.Services.DialogResponse.OK)
						{
							var msg = string.Format(Properties.Resources.P0808040000_BatchFinished,
								currentContainerPickInfo.DelvDate.ToString("yyyy/MM/dd"),
								currentContainerPickInfo.PickTime,
								currentContainerPickInfo.MoveOutTargetName,
								Environment.NewLine);
							if (ShowInfoMessage(msg) == UILib.Services.DialogResponse.OK)
								Init();
						}
					}
					else if (result2.IsContainerFinished)
					{
						if (ShowInfoMessage(Properties.Resources.P0808040000_ContainerAllotFinish) == UILib.Services.DialogResponse.OK)
						{
							Init();
						}
					}
				});
			}
			else
			{
				DispatcherAction(() =>
				{
					DoItemBarCodeFocus();
				});
			}
		}
		#endregion OutOfStockShip

		#region ContainerFinish 容器完成
		/// <summary>
		/// Gets the ContainerFinish.
		/// </summary>
		public ICommand ContainerFinishCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoContainerFinish(), () => UserOperateMode != OperateMode.Query
);
			}
		}

		public void DoContainerFinish()
		{
			ScanContainerCode = ScanContainerCode.ToUpper();
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.GetContainerLackItems(SelectedDc, _gupCode, _custCode, currentContainerPickInfo.PickOrdNo,ScanContainerCode));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				DispatcherAction(() =>
				{
					DoItemBarCodeFocus();
				});
				return;
			}
			var msgList = new List<string>();
			msgList.Add(Properties.Resources.P0808040000_ContainerIsEmptyConfirm);
			foreach (var item in result.LackItemDetails)
				msgList.Add(string.Format(Properties.Resources.P0808040000_NotAllottemInfo, item.ItemCode, item.ItemName, item.LackQty));
			if (ShowConfirmMessage(string.Join(Environment.NewLine, msgList)) == UILib.Services.DialogResponse.Yes)
			{
				var result2 = proxy.RunWcfMethod(w => w.ContainerComplete(SelectedDc, _gupCode, _custCode, currentContainerPickInfo.PickOrdNo, ScanContainerCode));
				if (!result2.IsSuccessed)
				{
					ShowWarningMessage(result2.Message);
					DispatcherAction(() =>
					{
						DoItemBarCodeFocus();
					});
					return;
				}
				if (ShowInfoMessage(Properties.Resources.P0808040000_ContainerAllotFinish) == UILib.Services.DialogResponse.OK)
				{
					Init();
				}
			}
			else
			{
				DispatcherAction(() =>
				{
					DoItemBarCodeFocus();
				});
			}

		}
		#endregion ContainerFinish

		#region BoxDetail 箱內明細
		public ICommand BoxDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoBoxDetail(), () => true
				);
			}
		}

		private void DoBoxDetail()
		{

		}
		#endregion


		#region PrintBoxDetail 列印箱明細

		public List<P0808040100_PrintData> GetPrintBoxDetail(BindBoxType bindBoxType, string boxNo)
		{
			return _deliveryReport.GetBoxData(SelectedDc, _gupCode, _custCode,
					 currentContainerPickInfo.DelvDate, currentContainerPickInfo.PickTime, currentContainerPickInfo.MoveOutTarget,
					 boxNo, bindBoxType == BindBoxType.NormalShipBox ? "01" : "02");
		}

		public void DoPrintBoxDetail(BindBoxType bindBoxType, List<P0808040100_PrintData> datas,bool setIsBusy = true)
		{
			if(setIsBusy)
				IsBusy = true;
			_deliveryReport.PrintBoxData(
				SelectedDc,
				bindBoxType == BindBoxType.NormalShipBox ? "01" : "02",
				datas);
			if(setIsBusy)
				IsBusy = false;
		}
		#endregion PrintBoxDetail

	}
}
