using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F14DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0810010000_ViewModel : InputViewModelBase
	{
		public P0810010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetInventoryTypeList();
			}
		}

		#region Property

		public Action ExitClick = delegate { };
		public Action InventoryNoFocus = delegate { };
		public Action ScanLocCodeFocus = delegate { };
		public Action ScanItemCodeOrSerialNoFocus = delegate { };
		public Action ScanQtyFocus = delegate { };
		private string _exceptInventoryNo;
        private List<string> _inventoryType = new List<string> { "0", "2", "5" };

        #region 物流中心
        private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				if (_dcList == value)
					return;
				Set(() => DcList, ref _dcList, value);
			}
		}

		private string _selectedDcCode;

		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				if (_selectedDcCode == value)
					return;
				Set(() => SelectedDcCode, ref _selectedDcCode, value);
				InventoryNo = "";
				DoSearch();
				AutoBindLocItem();
			}
		}

		#endregion

		#region 業主

		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}

		#endregion

		#region 貨主

		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		#endregion

		#region 盤點類型
		private List<NameValuePair<string>> _inventoryTypeList;

		public List<NameValuePair<string>> InventoryTypeList
		{
			get { return _inventoryTypeList; }
			set
			{
				if (_inventoryTypeList == value)
					return;
				Set(() => InventoryTypeList, ref _inventoryTypeList, value);
			}
		}

		#endregion

		#region 跑馬燈
		private string _marqueeMessage;

		public string MarqueeMessage
		{
			get { return _marqueeMessage; }
			set
			{
				if (_marqueeMessage == value)
					return;
				Set(() => MarqueeMessage, ref _marqueeMessage, value);
			}
		}
		#endregion

		#region Form - 上傳圖檔名稱/ ShareFolder路徑

		/// <summary>
		/// 圖檔路徑
		/// </summary>

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
				if (CurrentInventoryScanItem == null) _itemImageSource = null;
				else
				{
					_itemImageSource = FileService.GetItemImage(GupCode, CustCode, CurrentInventoryScanItem.ITEM_CODE);
				}
				RaisePropertyChanged("ItemImageSource");
			}
		}
		#endregion

		#region 語音
		private bool _isVoice;

		public bool IsVoice
		{
			get { return _isVoice; }
			set
			{
				if (_isVoice == value)
					return;
				Set(() => IsVoice, ref _isVoice, value);
			}
		}
		#endregion

		#region 目前的盤點單
		private F140101 _currentF140101;

		public F140101 CurrentF140101
		{
			get { return _currentF140101; }
			set
			{
				if (_currentF140101 == value)
					return;
				Set(() => CurrentF140101, ref _currentF140101, value);
			}
		}
		#endregion


		#region 目前儲位資訊
		private InventoryScanLoc _currentInventoryScanLoc;

		public InventoryScanLoc CurrentInventoryScanLoc
		{
			get { return _currentInventoryScanLoc; }
			set
			{
				if (_currentInventoryScanLoc == value)
					return;
				Set(() => CurrentInventoryScanLoc, ref _currentInventoryScanLoc, value);
			}
		}
		#endregion


		#region 目前商品資訊
		private InventoryScanItem _currentInventoryScanItem;

		public InventoryScanItem CurrentInventoryScanItem
		{
			get { return _currentInventoryScanItem; }
			set
			{
				if (_currentInventoryScanItem == value)
					return;
				Set(() => CurrentInventoryScanItem, ref _currentInventoryScanItem, value);
				ItemImageSource = null;
			}
		}
		#endregion


		#region 盤點單號

		private string _inventoryNo;

		public string InventoryNo
		{
			get { return _inventoryNo; }
			set
			{
				if (_inventoryNo == value)
					return;
				Set(() => InventoryNo, ref _inventoryNo, value);
			}
		}
		#endregion

		#region 盤點單類別
		private string _inventoryTypeName;

		public string InventoryTypeName
		{
			get { return _inventoryTypeName; }
			set
			{
				if (_inventoryTypeName == value)
					return;
				Set(() => InventoryTypeName, ref _inventoryTypeName, value);
			}
		}
		#endregion

		#region 刷讀儲位
		private string _scanLocCode;

		public string ScanLocCode
		{
			get { return _scanLocCode; }
			set
			{
				if (_scanLocCode == value)
					return;
				Set(() => ScanLocCode, ref _scanLocCode, value);
			}
		}
		#endregion

		#region 刷讀品號/序號
		private string _scanItemCodeOrSerialNo;

		public string ScanItemCodeOrSerialNo
		{
			get { return _scanItemCodeOrSerialNo; }
			set
			{
				if (_scanItemCodeOrSerialNo == value)
					return;
				Set(() => ScanItemCodeOrSerialNo, ref _scanItemCodeOrSerialNo, value);
			}
		}
		#endregion

		#region 輸入數量
		private int _enterQty;

		public int EnterQty
		{
			get { return _enterQty; }
			set
			{
				if (_enterQty == value)
					return;
				Set(() => EnterQty, ref _enterQty, value);
			}
		}
		#endregion

		#region Grid 盤點清單
		private List<InventoryItemQty> _inventoryItemQtyList;

		public List<InventoryItemQty> InventoryItemQtyList
		{
			get { return _inventoryItemQtyList; }
			set
			{
				if (_inventoryItemQtyList == value)
					return;
				Set(() => InventoryItemQtyList, ref _inventoryItemQtyList, value);
			}
		}

		private InventoryItemQty _selectedInventoryItemQty;

		public InventoryItemQty SelectedInventoryItemQty
		{
			get { return _selectedInventoryItemQty; }
			set
			{
				if (_selectedInventoryItemQty == value)
					return;
				Set(() => SelectedInventoryItemQty, ref _selectedInventoryItemQty, value);
			}
		}

		#endregion


		#region 盤點明細 for 異動盤
		private List<InventoryLocItem> _inventoryLocItemList;

		public List<InventoryLocItem> InventoryLocItemList
		{
			get { return _inventoryLocItemList; }
			set
			{
				if (_inventoryLocItemList == value)
					return;
				Set(() => InventoryLocItemList, ref _inventoryLocItemList, value);
			}
		}

		private InventoryLocItem SelectedInventoryLocItem { get; set; }
		#endregion

		private bool _maxUnitQtyEnabled = true;

		public bool MaxUnitQtyEnabled
		{
			get { return _maxUnitQtyEnabled; }
			set
			{
				if (_maxUnitQtyEnabled == value)
					return;
				_maxUnitQtyEnabled = value;
				RaisePropertyChanged("MaxUnitQtyEnabled");
			}
		}

		private bool _minUnitQtyEnabled = true;

		public bool MinUnitQtyEnabled
		{
			get { return _minUnitQtyEnabled; }
			set
			{
				if (_minUnitQtyEnabled == value)
					return;
				_minUnitQtyEnabled = value;
				RaisePropertyChanged("MinUnitQtyEnabled");
			}
		}

		#region 最大單位數量 Label
		private string _lbmaxUnitQty = Properties.Resources.P0810010000_LBMaxUnitQty;

		public string LBMaxUnitQty
		{
			get { return _lbmaxUnitQty; }
			set
			{
				if (_lbmaxUnitQty == value)
					return;
				_lbmaxUnitQty = value;
				RaisePropertyChanged("LBMaxUnitQty");
			}
		}
		#endregion

		#region 最小單位數量 Label
		private string _lbminUnitQty = Properties.Resources.P0810010000_LBMinUnitQty;

		public string LBMinUnitQty
		{
			get { return _lbminUnitQty; }
			set
			{
				if (_lbminUnitQty == value)
					return;
				_lbminUnitQty = value;
				RaisePropertyChanged("LBMinUnitQty");
			}
		}
		#endregion

		#region 最大單位數量
		private int _maxUnitQty;

		public int MaxUnitQty
		{
			get { return _maxUnitQty; }
			set
			{
				if (_maxUnitQty == value)
					return;
				_maxUnitQty = value;
				RaisePropertyChanged("MaxUnitQty");
			}
		}
		#endregion

		#region 最小單位數量
		private int _minUnitQty = 0;

		public int MinUnitQty
		{
			get { return _minUnitQty; }
			set
			{
				if (_minUnitQty == value)
					return;
				Set(() => MinUnitQty, ref _minUnitQty, value);
				RaisePropertyChanged("MinUnitQty");
			}
		}
		#endregion

		#region 箱入數
		private int _inBoxQty = 0;
		public int InBoxQty
		{
			get { return _inBoxQty; }
			set
			{
				if (_inBoxQty == value)
					return;
				Set(() => InBoxQty, ref _inBoxQty, value);
			}
		}
		#endregion

		#endregion

		#region 下拉式選單資料來源

		/// <summary>
		/// 設定DC清單
		/// </summary>
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (DcList.Any())
				SelectedDcCode = DcList.First().Value;
		}
		private void SetInventoryTypeList()
		{
			InventoryTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F140101", "INVENTORY_TYPE");
		}
		#endregion

		#region Method

		public void InitBindData()
		{
			EnterQty = 0;
			IsVoice = true;
			SetDcList();
		}

		private void ClearIni()
		{
			CurrentF140101 = null;
			ClearDetailByLoc();
		}

		private void ClearDetailByLoc()
		{
			_exceptInventoryNo = "";
			CurrentInventoryScanLoc = null;
			EnterQty = 0;
			ScanLocCode = null;
			ScanItemCodeOrSerialNo = null;
			InventoryItemQtyList = null;
			SelectedInventoryItemQty = null;
			ClearDetailByItemCode();
		}

		private void ClearDetailByItemCode()
		{
			CurrentInventoryScanItem = null;
			RaisePropertyChanged("ItemImageSource");
		}

		public void DoScanLocCode()
		{
			if (string.IsNullOrWhiteSpace(ScanLocCode))
			{
				if (IsVoice)
					PlaySoundHelper.Oo();
				ShowWarningMessage(Properties.Resources.P0810010000_ScanLocCode);
				ScanLocCodeFocus();
				return;
			}
			if (CurrentF140101 != null)
			{
				var locCode = ScanLocCode;
				DoUpdateToF140104OrF140105();
				ScanLocCode = locCode;
				var proxyEx = GetExProxy<P08ExDataSource>();
				var result = proxyEx.CreateQuery<InventoryScanLoc>("GetInventoryScanLoc")
					.AddQueryExOption("dcCode", CurrentF140101.DC_CODE)
					.AddQueryExOption("gupCode", CurrentF140101.GUP_CODE)
					.AddQueryExOption("custCode", CurrentF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", CurrentF140101.INVENTORY_NO)
					.AddQueryExOption("locCode", ScanLocCode.Trim()).ToList().First();
				if (result.IsSuccess)
				{
					if (IsVoice)
						PlaySoundHelper.Scan();
					CurrentInventoryScanLoc = result;
					ScanItemCodeOrSerialNoFocus();
				}
				else
				{
					ShowWarningMessage(result.Message);
					ClearDetailByLoc();
				}
			}
		}

		public void DoScanItemCodeOrSerialNo()
		{
			if (CurrentF140101 != null)
			{
				if (string.IsNullOrWhiteSpace(ScanLocCode) || CurrentInventoryScanLoc == null)
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(Properties.Resources.P0810010000_ScanLocCode);
					ScanLocCodeFocus();
					return;
				}
				if (string.IsNullOrWhiteSpace(ScanItemCodeOrSerialNo))
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(Properties.Resources.P0810010000_ScanItemCodeOrSerialNo);
					ScanItemCodeOrSerialNoFocus();
					return;
				}
				if (CurrentInventoryScanLoc != null) //如果已經刷過儲位 取代刷讀儲位(避免他在刷讀儲位亂key)
					ScanLocCode = CurrentInventoryScanLoc.LOC_CODE;
				var proxyEx = GetExProxy<P08ExDataSource>();
				var result = proxyEx.CreateQuery<InventoryScanItem>("GetInventoryScanItem")
					.AddQueryExOption("dcCode", CurrentF140101.DC_CODE)
					.AddQueryExOption("gupCode", CurrentF140101.GUP_CODE)
					.AddQueryExOption("custCode", CurrentF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", CurrentF140101.INVENTORY_NO)
					.AddQueryExOption("locCode", CurrentInventoryScanLoc.LOC_CODE)
					.AddQueryExOption("itemCodeOrSerialNo", ScanItemCodeOrSerialNo.Trim()).ToList().First();
				if (result.IsSuccess)
				{
					if (IsVoice)
						PlaySoundHelper.Scan();
					CurrentInventoryScanItem = result;

					InBoxQty = result.INBOXTQTY ?? 0;

					var unit_level = int.Parse(CurrentInventoryScanItem.PACKCOUNT_MAX_UNIT);
					if (unit_level == 0)
					{
						MaxUnitQtyEnabled = false;
						MinUnitQtyEnabled = false;
					}
					else if (unit_level == 1)
					{
						MaxUnitQtyEnabled = false;
						MinUnitQtyEnabled = true;

						LBMinUnitQty += CurrentInventoryScanItem.MINUNIT;
					}
					else if (unit_level >= 2)
					{
						MaxUnitQtyEnabled = true;
						MinUnitQtyEnabled = true;
						LBMaxUnitQty += CurrentInventoryScanItem.MAXUNIT;
						LBMinUnitQty += CurrentInventoryScanItem.MINUNIT;
					}
					
					ScanQtyFocus();
				}
				else
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(result.Message);
					ClearDetailByItemCode();
				}
			}
		}

		public void DoUpdateQty()
		{
			if (CurrentF140101 != null)
			{
				if (string.IsNullOrWhiteSpace(ScanLocCode) || CurrentInventoryScanLoc == null)
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(Properties.Resources.P0810010000_ScanLocCode);
					ScanLocCodeFocus();
					return;
				}
				if (string.IsNullOrWhiteSpace(ScanItemCodeOrSerialNo) || CurrentInventoryScanItem == null)
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(Properties.Resources.P0810010000_ScanItemCodeOrSerialNo);
					ScanItemCodeOrSerialNoFocus();
					return;
				}
				if (EnterQty < 0)
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(Properties.Resources.P0810010000_EnterQtyZeroError);
					return;
				}
				if (_inventoryType.Contains(CurrentF140101.INVENTORY_TYPE) &&
					!InventoryLocItemList.Any(o => o.LOC_CODE == ScanLocCode && o.ITEM_CODE == ScanItemCodeOrSerialNo))
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(Properties.Resources.P0810010000_LocItemError);
					return;
				}

				if (CurrentInventoryScanLoc != null) //如果已經刷過儲位 取代刷讀儲位(避免他在刷讀儲位亂key)
					ScanLocCode = CurrentInventoryScanLoc.LOC_CODE;
				if (CurrentInventoryScanItem != null)
					ScanItemCodeOrSerialNo = CurrentInventoryScanItem.ITEM_CODE;
				var proxyEx = GetExProxy<P08ExDataSource>();
				var result = proxyEx.CreateQuery<InventoryItemQty>("UpdateToGetInventoryItemQty")
					.AddQueryExOption("dcCode", CurrentF140101.DC_CODE)
					.AddQueryExOption("gupCode", CurrentF140101.GUP_CODE)
					.AddQueryExOption("custCode", CurrentF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", CurrentF140101.INVENTORY_NO)
					.AddQueryExOption("locCode", CurrentInventoryScanLoc.LOC_CODE)
					.AddQueryExOption("itemCode", CurrentInventoryScanItem.ITEM_CODE)
					.AddQueryOption("qty", EnterQty).ToList().First();
				if (result.IsSuccess)
				{
					if (IsVoice)
						PlaySoundHelper.Scan();
					var inventoryItemQtyList = InventoryItemQtyList ?? new List<InventoryItemQty>();
					var item =
						inventoryItemQtyList.FirstOrDefault(o => o.ITEM_CODE == result.ITEM_CODE);
					if (item == null)
						inventoryItemQtyList.Add(result);
					else
						item.QTY = result.QTY;
					InventoryItemQtyList = inventoryItemQtyList.ToList();
					EnterQty = 0;
					ScanItemCodeOrSerialNo = string.Empty;
					ScanItemCodeOrSerialNoFocus();
					if (_inventoryType.Contains(CurrentF140101.INVENTORY_TYPE) && InventoryLocItemList.Any() && SelectedInventoryLocItem != null &&
						InventoryLocItemList.Max(o => o.ROWNUM) != SelectedInventoryLocItem.ROWNUM)
						NextLocCommand.Execute(null);
					else if (_inventoryType.Contains(CurrentF140101.INVENTORY_TYPE))
						ShowWarningMessage(Properties.Resources.P0810010000_DataLast);

				}
				else
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(result.Message);
				}
			}
		}

		public void DoClearInventoryQty()
		{
			var list = InventoryItemQtyList ?? new List<InventoryItemQty>();
			if (SelectedInventoryItemQty != null)
			{
				var proxyEx = GetExProxy<P08ExDataSource>();
				var result = proxyEx.CreateQuery<ExecuteResult>("ClearInventoryItemQty")
					.AddQueryExOption("dcCode", CurrentF140101.DC_CODE)
					.AddQueryExOption("gupCode", CurrentF140101.GUP_CODE)
					.AddQueryExOption("custCode", CurrentF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", CurrentF140101.INVENTORY_NO)
					.AddQueryExOption("locCode", CurrentInventoryScanLoc.LOC_CODE)
					.AddQueryExOption("itemCode", SelectedInventoryItemQty.ITEM_CODE)
					.ToList().First();
				if (!result.IsSuccessed)
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(result.Message);
				}
				else
				{
					list.Remove(SelectedInventoryItemQty);
					InventoryItemQtyList = list.ToList();
				}
				ScanItemCodeOrSerialNoFocus();
			}
		}

		public bool DoUpdateToF140104OrF140105()
		{
			if (CurrentF140101 != null && CurrentInventoryScanLoc != null)
			{
				var proxyEx = GetExProxy<P08ExDataSource>();
				var result = proxyEx.CreateQuery<ExecuteResult>("UpdateToF140104OrF140105")
					.AddQueryExOption("dcCode", CurrentF140101.DC_CODE)
					.AddQueryExOption("gupCode", CurrentF140101.GUP_CODE)
					.AddQueryExOption("custCode", CurrentF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", CurrentF140101.INVENTORY_NO)
					.AddQueryExOption("locCode", CurrentInventoryScanLoc.LOC_CODE)
					.AddQueryExOption("clientName", ReadRdpClientSessionInfo.GetRdpClientName())
					.ToList().First();
				if (!result.IsSuccessed)
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowWarningMessage(result.Message);
				}
				else
					ClearDetailByLoc();
				return result.IsSuccessed;
			}
			return true;

		}

		public void ChangeEnterQty()
		{
			EnterQty = MaxUnitQty * InBoxQty + MinUnitQty;
			RaisePropertyChanged("EnterQty");
		}

		public void ChangeUnitQty() {
			MaxUnitQty = 0;
			MinUnitQty = 0;
			RaisePropertyChanged("MaxUnitQty");
			RaisePropertyChanged("MinUnitQty");
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(true), () => UserOperateMode == OperateMode.Query,
					c => AutoBindLocItem()
					);
			}
		}

		private void DoSearch(bool isAssign = false)
		{
			DoUpdateToF140104OrF140105();
			//執行查詢動
			var proxy = GetProxy<F14Entities>();
			var data = proxy.CreateQuery<F140101>("GetF140101ByUserCanInventory")
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDcCode))
				.AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", CustCode))
				.AddQueryOption("inventoryNo", string.Format("'{0}'", (!string.IsNullOrWhiteSpace(InventoryNo)) ? InventoryNo.Trim() : ""))
				.ToList();

            if (isAssign)
            {
                if (!string.IsNullOrWhiteSpace(InventoryNo))
                {
                    if (IsVoice)
                        PlaySoundHelper.Scan();
                    CurrentF140101 = data.Where(x => x.INVENTORY_NO == InventoryNo).FirstOrDefault();

                    if (CurrentF140101 == null)
                    {
                        CurrentF140101 = data.First(c => c.INVENTORY_NO != _exceptInventoryNo);
                    }

                    InventoryNo = CurrentF140101.INVENTORY_NO;
                    ClearDetailByLoc();
                    GetInventoryLocItem();
                }
            }
            else
            {
                if (data.Any(c => c.INVENTORY_NO != _exceptInventoryNo))
                {
                    if (!string.IsNullOrWhiteSpace(InventoryNo))
                        if (IsVoice)
                            PlaySoundHelper.Scan();
                    CurrentF140101 = data.First(c => c.INVENTORY_NO != _exceptInventoryNo);
                    InventoryNo = CurrentF140101.INVENTORY_NO;
                    ClearDetailByLoc();
                    GetInventoryLocItem();
                }
                else
                {
                    ClearIni();
                    if (!string.IsNullOrWhiteSpace(InventoryNo))
                    {
                        if (IsVoice)
                            PlaySoundHelper.Oo();
                        ShowWarningMessage(Properties.Resources.P0810010000_InventoryNoFail + Environment.NewLine + Properties.Resources.P0810010000_InventoryNoFail1 + Environment.NewLine + Properties.Resources.P0810010000_InventoryNoFail2 +
                                                             Environment.NewLine + Properties.Resources.P0810010000_InventoryNoFail3);
                    }
                }
            }
		}
		#endregion Search

		#region 完成

		private bool _isFinishOk;
		public ICommand FinishCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoFinish(),
					() => InventoryItemQtyList != null && InventoryItemQtyList.Any(),
					c => DoFinishComplete());
			}
		}

		private void DoFinish()
		{
			_isFinishOk = DoUpdateToF140104OrF140105();
		}

		private void DoFinishComplete()
		{
			if (_isFinishOk)
			{
				ClearDetailByLoc();
				ScanLocCodeFocus();
				if (_inventoryType.Contains(CurrentF140101.INVENTORY_TYPE))
				{
					_exceptInventoryNo = InventoryNo;
					InventoryNo = "";
                    DoSearch();
                    AutoBindLocItem();
				}

			}
		}

		#endregion

		#region 離開

		private bool _isExitOk;
		public ICommand ExitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoExit(),
					() => true,
					c => DoExitComplete());
			}
		}

		private void DoExit()
		{
			_isExitOk = false;
			if (DialogService.ShowMessage(WpfClient.Resources.Resources.WarningBeforeClose, Properties.Resources.P0801010101_Ask, DialogButton.YesNo, DialogImage.Question, MessageBoxResult.No) ==
				DialogResponse.Yes)
			{
				DoUpdateToF140104OrF140105();
				_isExitOk = true;
			}
		}

		private void DoExitComplete()
		{
			if (_isExitOk)
				ExitClick();
		}

		#endregion


		#region 取得盤點單明細 for 異動盤

		private void GetInventoryLocItem()
		{
			if (_inventoryType.Contains(CurrentF140101.INVENTORY_TYPE))
			{
				var proxy = GetExProxy<P08ExDataSource>();
				InventoryLocItemList = proxy.CreateQuery<InventoryLocItem>("GetInventoryLocItems")
					.AddQueryExOption("dcCode", CurrentF140101.DC_CODE)
					.AddQueryExOption("gupCode", CurrentF140101.GUP_CODE)
					.AddQueryExOption("custCode", CurrentF140101.CUST_CODE)
					.AddQueryExOption("inventoryNo", CurrentF140101.INVENTORY_NO).ToList();
				SelectedInventoryLocItem = InventoryLocItemList.FirstOrDefault();
			}
			else
			{
				InventoryLocItemList = new List<InventoryLocItem>();
				SelectedInventoryLocItem = null;
			}
		}
		#endregion

		#region 自動刷讀繫結儲位品項

		private void AutoBindLocItem()
		{
			if (SelectedInventoryLocItem != null)
			{
				if (CurrentInventoryScanLoc == null || CurrentInventoryScanLoc.LOC_CODE != SelectedInventoryLocItem.LOC_CODE)
				{
					ScanLocCode = SelectedInventoryLocItem.LOC_CODE;
                   
                    DoScanLocCode();
				}
				ScanItemCodeOrSerialNo = SelectedInventoryLocItem.ITEM_CODE;
				DoScanItemCodeOrSerialNo();
				EnterQty = 0;
				ScanQtyFocus();
			}
		}
		#endregion

		#region 上一筆儲位

		public ICommand PrevLocCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPrevLoc(),
					() => InventoryLocItemList != null && InventoryLocItemList.Any() && SelectedInventoryLocItem != null && SelectedInventoryLocItem.ROWNUM != 1,
					c => DoPrevLocComplete());
			}
		}
		private bool _isPrevLocOk;

		private void DoPrevLoc()
		{
			_isPrevLocOk = false;
			var item =
				InventoryLocItemList.Where(o => o.ROWNUM < SelectedInventoryLocItem.ROWNUM).OrderByDescending(o => o.ROWNUM).First();
			if (SelectedInventoryLocItem != null)
			{
				if (SelectedInventoryLocItem.LOC_CODE != item.LOC_CODE)
				{
					if (ShowConfirmMessage(Properties.Resources.P0810010000_DataFirst + Environment.NewLine + Properties.Resources.P0810010000_NextAsk) == DialogResponse.No)
						return;
				}
			}
			SelectedInventoryLocItem = item;
			_isPrevLocOk = true;
		}

		private void DoPrevLocComplete()
		{
			if (_isPrevLocOk && SelectedInventoryLocItem != null)
			{
				AutoBindLocItem();
			}

		}

		#endregion

		#region 下一筆儲位

		public ICommand NextLocCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoNextLoc(),
					() => InventoryLocItemList != null && InventoryLocItemList.Any() && SelectedInventoryLocItem != null && SelectedInventoryLocItem.ROWNUM != InventoryLocItemList.Max(o => o.ROWNUM),
					c => DoNextLocComplete());
			}
		}

		private bool _isNextLocOk;
		private void DoNextLoc()
		{
			_isNextLocOk = false;
			var item =
				InventoryLocItemList.Where(o => o.ROWNUM > SelectedInventoryLocItem.ROWNUM).OrderBy(o => o.ROWNUM).First();
			if (SelectedInventoryLocItem != null)
			{
				if (SelectedInventoryLocItem.LOC_CODE != item.LOC_CODE)
				{
					if (ShowConfirmMessage(Properties.Resources.P0810010000_LocItemLast + Environment.NewLine + Properties.Resources.P0810010000_NextAsk) == DialogResponse.No)
						return;
				}
			}
			SelectedInventoryLocItem = item;
			_isNextLocOk = true;
		}

		private void DoNextLocComplete()
		{
			if (_isNextLocOk && SelectedInventoryLocItem != null)
			{
				AutoBindLocItem();
			}
		}

		#endregion
	}
}
