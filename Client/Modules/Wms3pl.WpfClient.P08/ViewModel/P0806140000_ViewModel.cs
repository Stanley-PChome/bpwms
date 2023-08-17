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
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public enum NextStep
	{
		/// <summary>
		/// 無
		/// </summary>
		None = 0,
		/// <summary>
		/// 包裝站
		/// </summary>
		PackageStation = 1,
		/// <summary>
		/// 集貨場
		/// </summary>
		CollectStation = 2,
		/// <summary>
		/// 異常區
		/// </summary>
		ErrorStation = 3

	}
	/// <summary>
	/// 揀貨分貨作業模式
	/// </summary>
	public enum PickAllotMode
	{
		/// <summary>
		/// 刷讀單據條碼
		/// </summary>
		ScanWmsNoBarCode,
		/// <summary>
		/// 綁定箱號
		/// </summary>
		BindingBox,
		/// <summary>
		/// 分貨中
		/// </summary>
		Sowing
	}

	/// <summary>
	/// 分貨場-層
	/// </summary>
	public class PickAllotLayer : InputViewModelBase
	{

		#region 分貨站層號
		private int _pickAllotLayerId;

		public int PickAllotLayerId
		{
			get { return _pickAllotLayerId; }
			set
			{
				Set(() => PickAllotLayerId, ref _pickAllotLayerId, value);
			}
		}
		#endregion


		#region 分貨層儲格編號清單
		private List<PickAllotLoc> _pickAllotLocs;

		public List<PickAllotLoc> PickAllotLocs
		{
			get { return _pickAllotLocs; }
			set
			{
				Set(() => PickAllotLocs, ref _pickAllotLocs, value);
			}
		}
		#endregion

	}

	/// <summary>
	/// 分貨儲格編號
	/// </summary>
	public class PickAllotLoc : ViewModelBase
	{
		// 灰色
		public static SolidColorBrush defaultColor = new SolidColorBrush(Color.FromRgb(119, 119, 119));
		// 淺灰色
		public static SolidColorBrush bindBoxColor = new SolidColorBrush(Color.FromRgb(231, 230, 230));
		// 橘色
		public static SolidColorBrush packageColor = new SolidColorBrush(Color.FromRgb(255, 192, 0));
		// 綠色
		public static SolidColorBrush collectColor = new SolidColorBrush(Color.FromRgb(146, 208, 80));
		// 紫色
		public static SolidColorBrush errorColor = new SolidColorBrush(Color.FromRgb(168, 110, 212));

		public static SolidColorBrush selectedColor = new SolidColorBrush(Color.FromRgb(255, 255, 0));

		public PickAllotLoc()
		{
			Init();
		}

		#region 分貨儲格編號
		private string _locNo;

		public string LocNo
		{
			get { return _locNo; }
			set
			{
				Set(() => LocNo, ref _locNo, value);
			}
		}
		#endregion

		#region 是否綁定

		public bool IsBinding
		{
			get { return !string.IsNullOrEmpty(BoxNo); }
		}
		#endregion

		#region 是否顯示箱號
		private Visibility _showBoxNo;
		public Visibility ShowBoxNo
		{
			get { return _showBoxNo; }
			set
			{
				Set(() => ShowBoxNo, ref _showBoxNo, value);
			}
		}
		#endregion

		#region 箱號
		private string _BoxNo;

		public string BoxNo
		{
			get { return _BoxNo; }
			set
			{
				Set(() => BoxNo, ref _BoxNo, value);
				if (string.IsNullOrEmpty(value))
					ShowBoxNo = Visibility.Hidden;
				else
					ShowBoxNo = Visibility.Visible;
			}
		}
		#endregion

		#region 單據號碼
		private string _wmsNo;

		public string WmsNo
		{
			get { return _wmsNo; }
			set
			{
				Set(() => WmsNo, ref _wmsNo, value);
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

		#region 是否顯示總數
		private Visibility _showTotalQty;
		public Visibility ShowTotalQty
		{
			get { return _showTotalQty; }
			set
			{
				Set(() => ShowTotalQty, ref _showTotalQty, value);
			}
		}
		#endregion

		#region 總數
		private int _totalQty;

		public int TotalQty
		{
			get { return _totalQty; }
			set
			{
				Set(() => TotalQty, ref _totalQty, value);
				if (value == 0)
					ShowTotalQty = Visibility.Hidden;
				else
					ShowTotalQty = Visibility.Visible;
			}
		}
		#endregion

		#region 是否顯示已播數
		private Visibility _showSowQty;
		public Visibility ShowSowQty
		{
			get { return _showSowQty; }
			set
			{
				Set(() => ShowSowQty, ref _showSowQty, value);
			}
		}
		#endregion

		#region 已播數
		private int _sowQty;

		public int SowQty
		{
			get { return _sowQty; }
			set
			{
				Set(() => SowQty, ref _sowQty, value);
				if (value == 0 && !IsBinding)
					ShowSowQty = Visibility.Hidden;
				else
					ShowSowQty = Visibility.Visible;
			}
		}
		#endregion

		#region 下一步驟
		private NextStep _nextStep;

		public NextStep NextStep
		{
			get { return _nextStep; }
			set
			{
				Set(() => NextStep, ref _nextStep, value);
			}
		}
		#endregion


		#region 是否顯示集貨場編號
		private Visibility _showCollectionCode;
		public Visibility ShowCollectionCode
		{
			get { return _showCollectionCode; }
			set
			{
				Set(() => ShowCollectionCode, ref _showCollectionCode, value);
			}
		}
		#endregion

		#region 集貨場編號
		private string _collectionCode;

		public string CollectionCode
		{
			get { return _collectionCode; }
			set
			{
				Set(() => CollectionCode, ref _collectionCode, value);

			}
		}
		#endregion

		#region 分貨狀態
		private string _status;

		public string Status
		{
			get { return _status; }
			set
			{
				Set(() => Status, ref _status, value);
			}
		}
		#endregion

		#region 分貨明細
		private List<ShipOrderPickAllotDetail> _details;

		public List<ShipOrderPickAllotDetail> Details
		{
			get { return _details; }
			set
			{
				Set(() => Details, ref _details, value);
			}
		}
		#endregion
		
		/// <summary>
		/// 計算儲格完成
		/// </summary>
		public void CountFinish()
		{
			if (string.IsNullOrEmpty(CollectionCode) || NextStep != NextStep.CollectStation)
				ShowCollectionCode = Visibility.Hidden;
			else
				ShowCollectionCode = Visibility.Visible;
			switch (NextStep)
			{
				case NextStep.None:
					BoxColor = defaultColor;
					break;
				case NextStep.PackageStation:
					BoxColor = packageColor;
					break;
				case NextStep.CollectStation:
					BoxColor = collectColor;
					break;
				case NextStep.ErrorStation:
					BoxColor = errorColor;
					break;
			}
		}

		/// <summary>
		/// 初始化設定
		/// </summary>
		public void Init()
		{
			BoxColor = defaultColor;
			ShowBoxNo = Visibility.Hidden;
			ShowTotalQty = Visibility.Hidden;
			ShowSowQty = Visibility.Hidden;
			ShowCollectionCode = Visibility.Hidden;
			NextStep = NextStep.None;
			Details = new List<ShipOrderPickAllotDetail>();
			Status = "0";
			CollectionCode = string.Empty;
			TotalQty = 0;
			WmsNo = string.Empty;
			BoxNo = string.Empty;
		}

	}

	public class P0806140000_ViewModel : InputViewModelBase
	{
		#region Property

		public Action TxtWmsNoBarCodeFocus = delegate { };
		public Action TxtContainerCodeFocus = delegate { };
		public Action TxtItemBarCodeFocus = delegate { };
		public Action OpenBoxDetail = delegate { };


		#region 取得業主編號
		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}
		#endregion

		#region 取得貨主編號
		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}
		#endregion


		#region 目前揀貨分貨模式
		private PickAllotMode _currentPickAllotMode;

		public PickAllotMode CurrentPickAllotMode
		{
			get { return _currentPickAllotMode; }
			set
			{
				Set(() => CurrentPickAllotMode, ref _currentPickAllotMode, value);
			}
		}
		#endregion

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

		#region 選取的物流中心編號
		private string _selectedDc;

		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				Set(() => SelectedDc, ref _selectedDc, value);
			}
		}
		#endregion

		#region 單據條碼
		private string _scanWmsNoBarCode;

		public string ScanWmsNoBarCode
		{
			get { return _scanWmsNoBarCode; }
			set
			{
				Set(() => ScanWmsNoBarCode, ref _scanWmsNoBarCode, value);
			}
		}
		#endregion

		#region 出貨單數
		private int? _shipCnt;

		public int? ShipCnt
		{
			get { return _shipCnt; }
			set
			{
				Set(() => ShipCnt, ref _shipCnt, value);
			}
		}
		#endregion

		#region 已綁定出貨單數
		private int? _bindShipCnt;

		public int? BindShipCnt
		{
			get { return _bindShipCnt; }
			set
			{
				Set(() => BindShipCnt, ref _bindShipCnt, value);
			}
		}
		#endregion

		#region 刷讀容器條碼
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

		#region 刷讀品號/國條/序號
		private string _scanItemBarcCode;

		public string ScanItemBarCode
		{
			get { return _scanItemBarcCode; }
			set
			{
				Set(() => ScanItemBarCode, ref _scanItemBarcCode, value);
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

		#region 分貨站層清單
		private List<PickAllotLayer> _pickAllotLayers;

		public List<PickAllotLayer> PickAllotLayers
		{
			get { return _pickAllotLayers; }
			set
			{
				Set(() => PickAllotLayers, ref _pickAllotLayers, value);
			}
		}
		#endregion

		#region 是否分貨完成
		private bool _isAllotCompleted;

		public bool IsAllotCompleted
		{
			get { return _isAllotCompleted; }
			set
			{
				Set(() => IsAllotCompleted, ref _isAllotCompleted, value);
			}
		}
		#endregion

		#region 鎖定單據條碼
		private bool _lockSearchOrder;

		public bool LockSearchOrder
		{
			get { return _lockSearchOrder; }
			set
			{
				Set(() => LockSearchOrder, ref _lockSearchOrder, value);
			}
		}
		#endregion

		#region 鎖定綁定容器
		private bool _lockContainerCode;

		public bool LockContainerCode
		{
			get { return _lockContainerCode; }
			set
			{
				Set(() => LockContainerCode, ref _lockContainerCode, value);
			}
		}
		#endregion

		private PickAllotLoc _prePickAllotLoc;

		private List<ShipOrderPickAllot> _tempShipOrderPickAllot;

		#endregion


		public P0806140000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
				if (DcList.Any())
					SelectedDc = DcList.First().Value;
			}

		}

		#region SearchOrder 查詢單據
		/// <summary>
		/// Gets the SearchOrder.
		/// </summary>
		public ICommand SearchOrderCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearchOrder(), () => UserOperateMode == OperateMode.Query
);
			}
		}

		public void DoSearchOrder()
		{
			var tmpScanWmsNoBarCode = "";
			var isScan = false;
			try
			{
				if (!LockSearchOrder)
				{
					LockSearchOrder = true;
					isScan = true;
					if (!string.IsNullOrWhiteSpace(ScanWmsNoBarCode))
						ScanWmsNoBarCode = ScanWmsNoBarCode.ToUpper();
					tmpScanWmsNoBarCode = ScanWmsNoBarCode;
					var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
					var result = proxy.RunWcfMethod(w => w.CheckPickAllotOrder(SelectedDc, GupCode, CustCode, tmpScanWmsNoBarCode));

					if (!result.IsSuccessed)
					{
						if (result.No == "F")
						{
							ShowWarningMessage(result.Message);
							DispatcherAction(() =>
							{
								TxtWmsNoBarCodeFocus();
							});
							return;
						}
						if (result.No == "C")
						{
							if (ShowConfirmMessage(result.Message) == DialogResponse.No)
							{
								DispatcherAction(() =>
								{
									TxtWmsNoBarCodeFocus();
								});
								return;
							}
						}
					}

					// 取得或建立分貨資料
					var pickAllotData = proxy.RunWcfMethod(w => w.GetAndCreatePickAllotDatas(SelectedDc, GupCode, CustCode, tmpScanWmsNoBarCode));
					_tempShipOrderPickAllot = pickAllotData.ShipOrderPickAllots.ToList();

					DispatcherAction(() =>
					{
						RefreshSowBox((!result.IsSuccessed && result.No == "FS"));
					});

					// 揀貨單已分貨完成
					if (!result.IsSuccessed && result.No == "FS")
					{
						IsAllotCompleted = true;
						ShowInfoMessage(result.Message);
						return;
					}
					IsAllotCompleted = false;
					ShipCnt = pickAllotData.WmsOrdCnt;
					BindShipCnt = pickAllotData.ShipOrderPickAllots.Count(x => !string.IsNullOrEmpty(x.ContainerCode));
					if (ShipCnt == BindShipCnt)
					{
						DispatcherAction(() =>
						{
							CurrentPickAllotMode = PickAllotMode.Sowing;
							TxtItemBarCodeFocus();
						});

					}
					else
					{
						DispatcherAction(() =>
						{
							CurrentPickAllotMode = PickAllotMode.BindingBox;
							TxtContainerCodeFocus();
						});
					}
				}
			}
			finally
			{
				if (isScan && LockSearchOrder)
				{
					LockSearchOrder = false;
					ScanWmsNoBarCode = tmpScanWmsNoBarCode;
				}
					
			}
			
		}
		#endregion

		#region PauseAllot 暫停分貨
		/// <summary>
		/// Gets the PauseAllot.
		/// </summary>
		public ICommand PauseAllotCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoPauseAllot(), () => UserOperateMode == OperateMode.Query

);
			}
		}

		public void DoPauseAllot()
		{
			if (CurrentPickAllotMode == PickAllotMode.BindingBox)
			{
				if (ShowConfirmMessage(Properties.Resources.P0806140000_BindContainerNotFinishConfirm) == DialogResponse.No)
				{
					DispatcherAction(() =>
					{
						TxtContainerCodeFocus();
					});
					return;
				}

			}
			else
			{
				if (ShowConfirmMessage(Properties.Resources.P0806140000_PauseSowConfirm) == DialogResponse.No)
				{
					DispatcherAction(() =>
					{
						TxtItemBarCodeFocus();
					});
					return;
				}

			}
			DispatcherAction(() =>
			{
				TxtWmsNoBarCodeFocus();
				Init();
			});
		}
		#endregion

		#region OutOfStock 缺貨
		/// <summary>
		/// Gets the OutOfStock.
		/// </summary>
		public ICommand OutOfStockCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoOutOfStock(), () => UserOperateMode == OperateMode.Query
);
			}
		}

		public void DoOutOfStock()
		{
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();

			// 執行分貨處理
			var shipOrderPickAllots = proxy.RunWcfMethod(w => w.OutOfStocks(SelectedDc, GupCode, CustCode, ScanWmsNoBarCode));
			_tempShipOrderPickAllot = shipOrderPickAllots.ToList();
			DispatcherAction(() =>
			{
				RefreshSowBox(true);
				IsAllotCompleted = true;
				if (ShowInfoMessage(Properties.Resources.P0806140000_SowFinishMsg) == DialogResponse.OK)
				{
					ScanItemBarCode = null;
					ItemName = null;
					CurrentPickAllotMode = PickAllotMode.ScanWmsNoBarCode;
					TxtWmsNoBarCodeFocus();
				}
			});
		}
		#endregion

		#region BindContainer 綁定容器
		/// <summary>
		/// Gets the BindBox.
		/// </summary>
		public ICommand BindContainerCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoBindContainer(), () => UserOperateMode == OperateMode.Query
);
			}
		}

		public void DoBindContainer()
		{
			var isScan = false;
			try
			{
				IsBusy = true;
				if (!LockContainerCode)
				{
					LockContainerCode = true;
					isScan = true;
					var containerCode = ScanContainerCode;
					if (string.IsNullOrWhiteSpace(containerCode))
					{
						ShowWarningMessage(Properties.Resources.P0806140000_PleaseInputContainerCode);
						TxtContainerCodeFocus();
						return;
					}

					containerCode = containerCode.ToUpper();
					if (containerCode.StartsWith("O") || containerCode.StartsWith("P"))
					{
						ShowWarningMessage(Properties.Resources.P0806140000_ContainerFirstWordCannotOorP);
						TxtContainerCodeFocus();
						return;
					}


					var isBindOtherLoc = (from ly in PickAllotLayers
																from loc in ly.PickAllotLocs
																where loc.IsBinding && loc.BoxNo == containerCode
																select loc).FirstOrDefault();

					if (isBindOtherLoc != null)
					{
						ShowWarningMessage(string.Format(Properties.Resources.P0806140000_ContainerIsBindOtherLocPosition, isBindOtherLoc.LocNo));
						TxtContainerCodeFocus();
						return;
					}

					var layer = PickAllotLayers.FirstOrDefault(x => x.PickAllotLocs.Any(y => string.IsNullOrEmpty(y.BoxNo)));
					var box = layer.PickAllotLocs.FirstOrDefault(x => string.IsNullOrEmpty(x.BoxNo));
					var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();

					// 檢查容器編號
          // No.2093 若容器使用中，不讓人員選是否使用容器，直接跳出訊息並檔下
					var result = proxy.RunWcfMethod(w => w.CheckContainer(SelectedDc, containerCode));
					if (!result.IsSuccessed)
					{
            ShowWarningMessage(result.Message);
            TxtContainerCodeFocus();
						return;
					}
					if (_tempShipOrderPickAllot.Max(x => Convert.ToInt32(x.PickLocNo)) >= Convert.ToInt32(box.LocNo)) //刷太快會造成PickLocNo<box.LocNo而查無資料
					{
						// 更新暫存資料
						var shipOrderPickAllot = _tempShipOrderPickAllot.First(x => x.PickLocNo == box.LocNo);
						shipOrderPickAllot.ContainerCode = containerCode;
						shipOrderPickAllot.Status = "1";
					}




					// 累加綁定單數
					BindShipCnt++;

					// 如果累加綁訂單數=出貨單數
					if (ShipCnt == BindShipCnt)
					{
						// 進行綁定完成處理
						var shipOrderPickAllots = proxy.RunWcfMethod(w => w.BindContainerFinished(SelectedDc, GupCode, CustCode, ScanWmsNoBarCode, _tempShipOrderPickAllot.ToArray()));
						if (shipOrderPickAllots != null && shipOrderPickAllots.Any())
						{
							_tempShipOrderPickAllot = shipOrderPickAllots.ToList();
							DispatcherAction(() =>
							{
								box.BoxNo = _tempShipOrderPickAllot.Last().ContainerCode;
								box.Status = "1";
								box.BoxColor = PickAllotLoc.selectedColor;
								RefreshSowBox(false);
								CurrentPickAllotMode = PickAllotMode.Sowing;
								ScanContainerCode = null;
							});

							TxtItemBarCodeFocus();
						}
					}
					else
					{
						box.BoxNo = containerCode;
						box.Status = "1";
						box.BoxColor = PickAllotLoc.selectedColor;
						TxtContainerCodeFocus();
					}
				}
			}
			finally
			{
				if (isScan && LockContainerCode)
				{
					LockContainerCode = false;
				}
				IsBusy = false;
			}


		}
		#endregion


		#region BoxDetail
		/// <summary>
		/// 箱內明細
		/// </summary>
		public ICommand BoxDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoBoxDetail(), () => UserOperateMode == OperateMode.Query && (IsAllotCompleted || CurrentPickAllotMode == PickAllotMode.Sowing)
);
			}
		}

		public void DoBoxDetail()
		{
			DispatcherAction(() =>
			{
				OpenBoxDetail();
			});
		}
		#endregion BoxDetail


		#region ScanItemBarCode 刷讀品號/國條/序號
		/// <summary>
		/// Gets the ScanItemBarCode.
		/// </summary>
		public ICommand ScanItemBarCodeCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoScanItemBarCode(), () => UserOperateMode == OperateMode.Query
);
			}
		}

		public void DoScanItemBarCode()
		{
			try
			{
				IsBusy = true;
				if (string.IsNullOrWhiteSpace(ScanItemBarCode))
				{
					ShowWarningMessage(Properties.Resources.P0806140000_PleaseScanItem);

					DispatcherAction(() =>
					{
						TxtItemBarCodeFocus();
					});
					return;
				}

				ScanItemBarCode = ScanItemBarCode.ToUpper();

				var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
				// 執行分貨處理
				var result = proxy.RunWcfMethod(w => w.SowItem(SelectedDc, GupCode, CustCode, ScanWmsNoBarCode, ScanItemBarCode));
				if (!result.IsSuccessed)
				{

					ShowWarningMessage(result.Message);
					DispatcherAction(() =>
					{
						ItemName = result.ItemName;
						TxtItemBarCodeFocus();
					});
					return;
				}

				DispatcherAction(() =>
				{
					if (_prePickAllotLoc != null)
						_prePickAllotLoc.BoxColor = PickAllotLoc.bindBoxColor;

					var layer = PickAllotLayers.First(x => x.PickAllotLocs.Any(y => y.LocNo == result.PickLocNo));
					var box = layer.PickAllotLocs.First(x => x.LocNo == result.PickLocNo);
					var detail = box.Details.First(x => x.PickOrdSeq == result.PickOrdSeq);
					detail.ASetQty += 1;
					box.SowQty += 1;
					box.BoxColor = PickAllotLoc.selectedColor;

					_prePickAllotLoc = box;
					ItemName = result.ItemName;

					if (result.IsPickSowFinished)
					{
						if (ShowInfoMessage(Properties.Resources.P0806140000_SowFinishMsg) == DialogResponse.OK)
						{
							AllBoxToFinished(result.CancelWmsOrdNos.ToList());
							IsAllotCompleted = true;
							ScanItemBarCode = null;
							ItemName = null;
							CurrentPickAllotMode = PickAllotMode.ScanWmsNoBarCode;
							TxtWmsNoBarCodeFocus();
						}
					}
					else
					{
						TxtItemBarCodeFocus();
					}
				});
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				IsBusy = false;
			}
		}
		#endregion

		/// <summary>
		/// 初始化分貨作業
		/// </summary>
		public void Init()
		{
			CreatePickAllot();
			BindShipCnt = null;
			ShipCnt = null;
			IsAllotCompleted = false;
			_prePickAllotLoc = null;
			ScanContainerCode = null;
			ScanItemBarCode = null;
			_tempShipOrderPickAllot = null;
			ItemName = null;
			CurrentPickAllotMode = PickAllotMode.ScanWmsNoBarCode;
		}
		/// <summary>
		/// 產生分貨站
		/// </summary>
		private void CreatePickAllot()
		{
			DispatcherAction(() =>
			{
				var pickAllotLayers = new List<PickAllotLayer>();
				for (var layer = 1; layer <= 6; layer++)
				{
					var pickAllotLayer = new PickAllotLayer();
					pickAllotLayer.PickAllotLayerId = layer;
					pickAllotLayer.PickAllotLocs = new List<PickAllotLoc>();
					for (var loc = 1; loc <= 4; loc++)
					{

						pickAllotLayer.PickAllotLocs.Add(new PickAllotLoc
						{
							LocNo = ((layer - 1) * 4 + loc).ToString().PadLeft(2, '0'),
						});
					}
					pickAllotLayers.Add(pickAllotLayer);
				}

				PickAllotLayers = pickAllotLayers;

			});
		}

		private NextStep GetNextStep(string nextStep)
		{
			switch (nextStep)
			{
				case "2":
					return NextStep.CollectStation;
				case "3":
					return NextStep.PackageStation;
				case "4":
					return NextStep.ErrorStation;
				default:
					return NextStep.None;
			}
		}

		/// <summary>
		/// 顯示所有箱分貨下一站位置
		/// </summary>
		private void AllBoxToFinished(List<string> cancelWmsOrdNos)
		{
			foreach (var layer in PickAllotLayers)
			{
				foreach (var loc in layer.PickAllotLocs)
				{
					if (loc.IsBinding)
					{
						if (cancelWmsOrdNos.Contains(loc.WmsNo))
							loc.NextStep = NextStep.ErrorStation;

						loc.CountFinish();
						loc.Status = "2";
					}

				}
			}
		}

		/// <summary>
		/// 更新播種牆資訊
		/// </summary>
		private void RefreshSowBox(bool isFinished)
		{
			// 更新綁定容器明細、總數、已播種數、顏色
			foreach (var layer in PickAllotLayers)
			{
				foreach (var loc in layer.PickAllotLocs)
				{
					var item = _tempShipOrderPickAllot.FirstOrDefault(x => x.PickLocNo == loc.LocNo);
					if (item != null)
					{

						if (CurrentPickAllotMode != PickAllotMode.BindingBox)
						{
							loc.Init();
							loc.BoxNo = item.ContainerCode;
							loc.WmsNo = item.WmsOrdNo;
							loc.CollectionCode = item.ColletionCode;
							loc.NextStep = GetNextStep(item.NextStep);
							loc.Status = item.Status;
							loc.Details = item.ShipOrderPickAllotDetails.ToList();
							loc.TotalQty = item.ShipOrderPickAllotDetails.Sum(x => x.BSetQty);
							loc.SowQty = item.ShipOrderPickAllotDetails.Sum(x => x.ASetQty);
							loc.BoxColor = loc.IsBinding ? PickAllotLoc.bindBoxColor : PickAllotLoc.defaultColor;
							if (isFinished)
								loc.CountFinish();
						}
						else
						{
							loc.Details = item.ShipOrderPickAllotDetails.ToList();
							loc.BoxColor = PickAllotLoc.bindBoxColor;
							loc.TotalQty = loc.Details.Sum(x => x.BSetQty);
							loc.SowQty = loc.Details.Sum(x => x.ASetQty);
						}
					}
					else
						loc.Init();
				}
			}
		}
	}
}
