using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using P19EX = Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P15WcfService;
using System.IO;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0803020000_ViewModel : InputViewModelBase
	{
		public P0803020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
			}

		}

		#region 資料繫結

		public bool IsSrcEnter = false;

		public void InitBindData()
		{
			ActualTarQty = "0";
			IsVoice = true;
            SetMakeNoVaildDateChange();
            SetDcList();
		}

        #endregion

        #region Property

        public bool IsCompleteTarClose = false;

		private string _userId;
		private string _userName;
		public Timer MarqueeTimer;
		public Action RunMarquee = delegate { };
		public Action ExitClick = delegate { };
		public Action SetDefaultFocusClick = delegate { };
		public Action SetAllocationNoFocusClick = delegate { };
		public Action SetScanItemCodeFocusClick = delegate { };
		private string _allocationOrginalStatus;
		public bool IsDifferentWareHouse;

        #region 物流中心

        private string _tempDcCode;
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
				if (!IsSrcEnter)
					AllocationNo = "";
				_selectedF151002DataByTar = null;
				DoSearch(false);
				DoSearchComplete();
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

		#region 調撥單號

		private string _tempAllocationNo;

		private string _allocationNo;
		public string AllocationNo
		{
			get { return _allocationNo; }
			set
			{
				_allocationNo = value;
				RaisePropertyChanged("AllocationNo");
			}
		}
		#endregion

		#region 上架倉別
		private string _wareHouseName;
		public string WareHouseName
		{
			get { return _wareHouseName; }
			set
			{
				_wareHouseName = value;
				RaisePropertyChanged("WareHouseName");
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
				_marqueeMessage = value;
				RaisePropertyChanged("MarqueeMessage");
			}
		}

		#endregion

		#region 儲位
		private string _locCode;
		public string LocCode
		{
			get { return _locCode; }
			set
			{
				_locCode = value;
				RaisePropertyChanged("LocCode");
				if (!string.IsNullOrEmpty(_locCode))
				{
					DisplayLocCode = string.Format("{0}-{1}-{2}-{3}-{4}", _locCode.Substring(0, 1), _locCode.Substring(1, 2),
						_locCode.Substring(3, 2), _locCode.Substring(5, 2), _locCode.Substring(7, 2));
				}
				else
					DisplayLocCode = _locCode;
			}
		}
		private string _displayLocCode;
		public string DisplayLocCode
		{
			get { return _displayLocCode; }
			set
			{
				_displayLocCode = value;
				RaisePropertyChanged("DisplayLocCode");
			}
		}
		#endregion

		#region 儲位應上架數

		private string _bTarQty;
		public string BTarQty
		{
			get { return _bTarQty; }
			set
			{
				_bTarQty = value;
				RaisePropertyChanged("BTarQty");

			}
		}

		#endregion

		#region 儲位實際上架數

		private string _aTarQty;
		public string ATarQty
		{
			get { return _aTarQty; }
			set
			{
				_aTarQty = value;
				RaisePropertyChanged("ATarQty");

			}
		}

		#endregion

		#region 品號

		private string _itemCode;
		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				_itemCode = value;
				ItemImageSource = null;
				RaisePropertyChanged("ItemCode");
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
				_itemName = value;
				RaisePropertyChanged("ItemName");

			}
		}

        #endregion

        #region 批號

        private string _makeNo;
        public string MakeNo
        {
            get { return _makeNo; }
            set
            {
                _makeNo = value;
                RaisePropertyChanged("MakeNo");
            }
        }

        private bool _isEnabledMakeNo;
        public bool IsEnabledMakeNo
        {
            get { return _isEnabledMakeNo; }
            set
            {
                _isEnabledMakeNo = value;
                RaisePropertyChanged("IsEnabledMakeNo");
            }
        }

        private bool _allocationChangMakeNo = false;

        #endregion

        #region 板號

        private string _palletNo;
        public string PalletNo
        {
            get { return _palletNo; }
            set
            {
                _palletNo = value;
                RaisePropertyChanged("PalletNo");
            }
        }

        #endregion

        #region 箱號

        private string _boxCtrlNo;
        public string BoxCrtlNo
        {
            get { return _boxCtrlNo; }
            set
            {
                _boxCtrlNo = value;
                RaisePropertyChanged("BoxCrtlNo");
            }
        }

        #endregion

        #region 刷讀目的儲位

        private string _scanTarLocCode;
		public string ScanTarLocCode
		{
			get { return _scanTarLocCode; }
			set
			{
				_scanTarLocCode = value;
				RaisePropertyChanged("ScanTarLocCode");
			}
		}

		private bool _isEnabledScanTarLocCode;
		public bool IsEnabledScanTarLocCode
		{
			get { return _isEnabledScanTarLocCode; }
			set
			{
				_isEnabledScanTarLocCode = value;
				RaisePropertyChanged("IsEnabledScanTarLocCode");
			}
		}
		#endregion

		#region 刷讀品號

		private string _scanItemCode;
		public string ScanItemCode
		{
			get { return _scanItemCode; }
			set
			{
				_scanItemCode = value;
				RaisePropertyChanged("ScanItemCode");
			}
		}

		private bool _isEnabledScanItemCode;
		public bool IsEnabledScanItemCode
		{
			get { return _isEnabledScanItemCode; }
			set
			{
				_isEnabledScanItemCode = value;
				RaisePropertyChanged("IsEnabledScanItemCode");
			}
		}
		#endregion

		#region 實際上架數

		private long _tempActualTarQty = 0;

		private string _actualTarQty;

		public string ActualTarQty
		{
			get { return _actualTarQty; }
			set
			{
				_actualTarQty = value;
				RaisePropertyChanged("ActualTarQty");
			}
		}

		private bool _isEnabledActualTarQty;
		public bool IsEnabledActualTarQty
		{
			get { return _isEnabledActualTarQty; }
			set
			{
				_isEnabledActualTarQty = value;
				RaisePropertyChanged("IsEnabledActualTarQty");
			}
		}
        #endregion

        #region 是否可以取消上架
        private bool _isClearEnabled = false;
        public bool IsClearEnabled
        {
            get { return _isClearEnabled; }
            set
            {
                _isClearEnabled = value;
                RaisePropertyChanged("IsClearEnabled");
            }
        }
        #endregion

        #region 商品已揀數

        private string _aItemTarQty;
		public string AItemTarQty
		{
			get { return _aItemTarQty; }
			set
			{
				_aItemTarQty = value;
				RaisePropertyChanged("AItemTarQty");

			}
		}
		#endregion

		#region 商品應揀數

		private string _bItemTarQty;
		public string BItemTarQty
		{
			get { return _bItemTarQty; }
			set
			{
				_bItemTarQty = value;
				RaisePropertyChanged("BItemTarQty");

			}
		}
		#endregion

		#region 序號

		private string _serialNo;
		public string SerialNo
		{
			get { return _serialNo; }
			set
			{
				_serialNo = value;
				RaisePropertyChanged("SerialNo");

			}
		}
		#endregion

		#region 序號(顯示用 如果有組合商品會顯示此組合商品關聯的商品序號)
		private string _serialNoByShow;

		public string SERIAL_NOByShow
		{
			get { return _serialNoByShow; }
			set
			{
				if (_serialNoByShow == value)
					return;
				Set(() => SERIAL_NOByShow, ref _serialNoByShow, value);
			}
		}
		#endregion

		#region 效期

		private DateTime? _validDate;

		public DateTime? ValidDate
		{
			get { return _validDate; }
			set
			{
				_validDate = value;
				RaisePropertyChanged("ValidDate");
			}
		}

        private bool _isEnabledValidDate;
		public bool IsEnabledValidDate
		{
			get { return _isEnabledValidDate; }
			set
			{
				_isEnabledValidDate = value;
				RaisePropertyChanged("IsEnabledValidDate");
			}
		}

        private bool _allocationChangValiDate = false; 
       
        #endregion

        #region 最小單位

        private string _retUnit;
		public string RetUnit
		{
			get { return _retUnit; }
			set
			{
				_retUnit = value;
				RaisePropertyChanged("RetUnit");

			}
		}
		#endregion

		#region 盒號
		private string _boxNo;

		public string BoxNo
		{
			get { return _boxNo; }
			set
			{
				if (_boxNo == value)
					return;
				Set(() => BoxNo, ref _boxNo, value);
			}
		}
		#endregion

		#region Form - 上傳圖檔名稱/ ShareFolder路徑

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
					_itemImageSource = FileService.GetItemImage(GupCode, CustCode, ItemCode);
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
				_isVoice = value;
				RaisePropertyChanged("IsVoice");
			}
		}

		#endregion

		#region 調撥單儲位商品資料

		private List<F151002DataByTar> _f151002DataByTars;

		private F151002DataByTar _selectedF151002DataByTar;
		#endregion

		#region 調撥單儲位商品實揀數繫結

		private List<F151002ItemLocDataByTar> _f151002ItemLocDataByTars;

		public List<F151002ItemLocDataByTar> F151002ItemLocDataByTars
		{
			get { return _f151002ItemLocDataByTars; }
			set
			{
				_f151002ItemLocDataByTars = value;
				RaisePropertyChanged("F151002ItemLocDataByTars");
			}
		}

		private F151002ItemLocDataByTar _selectedF151002ItemLocDataByTar;

		public F151002ItemLocDataByTar SelectedF151002ItemLocDataByTar
		{
			get { return _selectedF151002ItemLocDataByTar; }
			set
			{
				_selectedF151002ItemLocDataByTar = value;
				RaisePropertyChanged("SelectedF151002ItemLocDataByTar");
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

		#endregion

        #region 取得批號、效期修改設定資料

        private void SetMakeNoVaildDateChange()
        {
            var proxy = GetProxy<F19Entities>();
            var tmp = proxy.F1909s.Where(x => x.GUP_CODE.Equals(GupCode) && x.CUST_CODE.Equals(CustCode))
               .FirstOrDefault();
            _allocationChangValiDate = tmp.ALLOCATIONCHANGVALIDATE == "2" || tmp.ALLOCATIONCHANGVALIDATE == "3";
            _allocationChangMakeNo = tmp.ALLOCATIONCHANGMAKENO == "2" || tmp.ALLOCATIONCHANGVALIDATE == "3";
        }

        #endregion

        #region 取得商品圖檔

        private void RefreshImage()
		{
			ItemImageSource = null;
			RaisePropertyChanged("ImagePathForDisplay");
			RaisePropertyChanged("ItemImageSource");
		}

		#endregion

		#region 設定調撥單商品儲位資料

		private bool _isSearckOk;
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(false),
					() => true,
					c => DoSearchComplete());
			}
		}

		public void DoSearch(bool isAllowStatus4)
		{
			_isSearckOk = true;
			if (IsDifferentWareHouse && !string.IsNullOrEmpty(AllocationNo) && !AllocationNo.StartsWith("T"))
			{
				var proxy = GetProxy<F15Entities>();
				var item =
					proxy.F151004s.Where(
						o =>
							 o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.MOVE_BOX_NO == AllocationNo.Trim()).OrderByDescending(o=>o.CRT_DATE).FirstOrDefault();
				if (item != null)
				{
					if (IsVoice)
						PlaySoundHelper.Scan();
					AllocationNo = item.ALLOCATION_NO;
				}
				else
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					_isSearckOk = false;
					ShowWarningMessage(Properties.Resources.P0803020000_CaseNoNotExist);
					return;
				}
			}

			if (!isAllowStatus4 && !string.IsNullOrEmpty(_tempAllocationNo))
				ChangeAllocationStatusToOrginal();
			var proxyEx = GetExProxy<P08ExDataSource>();
			_f151002DataByTars = proxyEx.CreateQuery<F151002DataByTar>("GetF151002DataByTars")
				.AddQueryExOption("tarDcCode",  _selectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("allocationNo", AllocationNo)
				.AddQueryExOption("userId", _userId)
				.AddQueryExOption("userName", _userName)
				.AddQueryExOption("isAllowStatus4", isAllowStatus4 ? "1" : "0")
				.AddQueryExOption("isDiffWareHouse", IsDifferentWareHouse ? "1" : "0")
				.ToList();

		}

		private void DoSearchComplete()
		{
			if (_isSearckOk)
			{
				ScanItemCode = "";
				_tempActualTarQty = 0;
				ActualTarQty = "0";
				if (_f151002DataByTars.Any(o => o.STATUS == "1" && o.TAR_QTY > 0))
				{
					_selectedF151002DataByTar = _f151002DataByTars.First(o => o.STATUS == "1" && o.TAR_QTY > 0);
					var proxyEx = GetExProxy<P08ExDataSource>();
					var result = proxyEx.CreateQuery<ExecuteResult>("StartDownOrUpItemChangeStatus")
						.AddQueryExOption("dcCode", _selectedF151002DataByTar.DC_CODE)
						.AddQueryExOption("gupCode", GupCode)
						.AddQueryExOption("custCode", CustCode)
						.AddQueryExOption("allocationNo", _selectedF151002DataByTar.ALLOCATION_NO)
						.AddQueryExOption("isUp", "1")
						.ToList();
					if (result.First().IsSuccessed)
						_allocationOrginalStatus = result.First().Message; //暫存原狀態 (2)
				}
				else
					_selectedF151002DataByTar = null;

				BindLocData();
				SetDefaultFocusClick();
			}
		}

		private void BindLocData()
		{
			bool isOk = false;
			try
			{
				if (_selectedF151002DataByTar != null)
				{
					AllocationNo = _selectedF151002DataByTar.ALLOCATION_NO;
					_tempAllocationNo = AllocationNo;
					_tempDcCode = _selectedF151002DataByTar.DC_CODE;
					WareHouseName = _selectedF151002DataByTar.WAREHOUSE_NAME;
					LocCode = _selectedF151002DataByTar.SUG_LOC_CODE;
					ItemCode = _selectedF151002DataByTar.ITEM_CODE;
					ItemName = _selectedF151002DataByTar.ITEM_NAME;
					ValidDate = _selectedF151002DataByTar.VALID_DATE;
					SerialNo = _selectedF151002DataByTar.SERIAL_NO;
					SERIAL_NOByShow = _selectedF151002DataByTar.SERIAL_NOByShow;
					RetUnit = _selectedF151002DataByTar.RET_UNIT;
					BoxNo = _selectedF151002DataByTar.BOX_NO;
                    MakeNo = _selectedF151002DataByTar.MAKE_NO == "0" ? "" : _selectedF151002DataByTar.MAKE_NO;
                    BoxCrtlNo = _selectedF151002DataByTar.BOX_CTRL_NO == "0" ? "" : _selectedF151002DataByTar.BOX_CTRL_NO;
                    PalletNo = _selectedF151002DataByTar.PALLET_CTRL_NO == "0" ? "" : _selectedF151002DataByTar.PALLET_CTRL_NO;
					
					var proxy = GetProxy<F15Entities>();
					// 取得該調撥單明細中相同品號的清單
					var items = proxy.F151002s.Where(
						o =>
							o.DC_CODE == _tempDcCode && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
							o.ALLOCATION_NO == _tempAllocationNo && o.ITEM_CODE == ItemCode).ToList();

					// 顯示總已上架/總應上架
					AItemTarQty = items.Sum(o => o.A_TAR_QTY).ToString();
					BItemTarQty = items.Sum(o => o.TAR_QTY).ToString();

					// 取得該相同目的儲位與效期與序號的清單
					var locData = items.Where(o => o.SUG_LOC_CODE == LocCode).ToList();
					locData = locData.Where(
								o =>
									(o.VALID_DATE == ValidDate && !o.SRC_VALID_DATE.HasValue) ||
									(o.SRC_VALID_DATE.HasValue && o.SRC_VALID_DATE == ValidDate)).ToList();
                    locData = locData.Where(
                                 o =>
                (o.MAKE_NO == (string.IsNullOrWhiteSpace(MakeNo) ? "0" : MakeNo) && string.IsNullOrWhiteSpace(o.SRC_MAKE_NO)) ||
                ((!string.IsNullOrWhiteSpace(o.SRC_MAKE_NO)) && o.SRC_MAKE_NO == (string.IsNullOrWhiteSpace(MakeNo) ? "0" : MakeNo))).ToList();
                    locData = locData.Where(
                                o =>
                                o.BOX_CTRL_NO == (string.IsNullOrWhiteSpace(BoxCrtlNo) ? "0" : BoxCrtlNo) &&
                                o.PALLET_CTRL_NO == (string.IsNullOrWhiteSpace(PalletNo) ? "0" : PalletNo)).ToList();
                    locData = string.IsNullOrEmpty(SerialNo)
							? locData.Where(o => string.IsNullOrEmpty(o.SERIAL_NO)).ToList()
							: locData.Where(o => o.SERIAL_NO == SerialNo).ToList();

					// 顯示已上架數/應上架數
					var aTarQty = locData.Sum(o => o.A_TAR_QTY);
					ATarQty = aTarQty.ToString();
					BTarQty = _selectedF151002DataByTar.TAR_QTY.ToString();

					IsEnabledActualTarQty = (_selectedF151002DataByTar.STATUS == "1");
					IsEnabledScanItemCode = (_selectedF151002DataByTar.STATUS == "1");                         
                    IsEnabledScanTarLocCode = (_selectedF151002DataByTar.STATUS == "1");
                    IsEnabledValidDate = _allocationChangValiDate ? (_selectedF151002DataByTar.STATUS == "1") : _allocationChangValiDate;
                    IsEnabledMakeNo = _allocationChangMakeNo ? (_selectedF151002DataByTar.STATUS == "1") : _allocationChangMakeNo;
                    
                    BindItemLocActual();

                    if (aTarQty==0)
                    {
                        ActualTarQty = "0";
                        _tempActualTarQty = 0;
                    }
                    else
                    {
                        ActualTarQty = ATarQty;
                        _tempActualTarQty = aTarQty;
                    }
                }
				else
				{
					AllocationNo = "";
					_tempAllocationNo = "";
					_tempDcCode = string.Empty;
					WareHouseName = "";
					LocCode = "";
					ItemCode = "";
					ItemName = "";
					ValidDate = null;
					SerialNo = "";
					RetUnit = "";
					ATarQty = "";
					BTarQty = "";
					AItemTarQty = "";
					BItemTarQty = "";
					_tempActualTarQty = 0;
					ActualTarQty = "";
					ScanTarLocCode = "";
					SERIAL_NOByShow = "";
                    MakeNo = "";
                    BoxCrtlNo = "";
                    PalletNo = "";
					RefreshImage();
					F151002ItemLocDataByTars = null;
					IsEnabledScanTarLocCode = false;
					IsEnabledActualTarQty = false;
					IsEnabledScanItemCode = false;
					IsEnabledValidDate = false;
                    IsEnabledMakeNo = false;
                }
				isOk = true;
			}
			finally
			{
				if (!isOk)
				{
					ChangeAllocationStatusToOrginal();
					ExitClick();
				}
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
					() => _f151002DataByTars != null && _f151002DataByTars.Any(o => o.STATUS == "1" && o.TAR_QTY > 0) && _selectedF151002DataByTar != null && _f151002DataByTars.First(o => o.STATUS == "1" && o.TAR_QTY > 0) != _selectedF151002DataByTar,
					c => DoPrevLocComplete());
			}
		}

		private bool _isPrevOk;
		private void DoPrevLoc()
		{
			_isPrevOk = false;
			try
			{
				_selectedF151002DataByTar =
					_f151002DataByTars.Where(o => o.STATUS == "1" && o.TAR_QTY > 0 && o.ROWNUM < _selectedF151002DataByTar.ROWNUM)
			.OrderByDescending(o => o.ROWNUM).First();
				_isPrevOk = true;
			}
			finally
			{
				if (!_isPrevOk)
				{
					ChangeAllocationStatusToOrginal();
					ExitClick();
				}
			}
		}

		private void DoPrevLocComplete()
		{
			if (_isPrevOk)
			{
				BindLocData();
				ScanTarLocCode = "";
				ScanItemCode = "";
				SetDefaultFocusClick();		
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
					() => _f151002DataByTars != null && _f151002DataByTars.Any(o => o.STATUS == "1" && o.TAR_QTY > 0) && _selectedF151002DataByTar != null && _f151002DataByTars.Last(o => o.STATUS == "1" && o.TAR_QTY > 0) != _selectedF151002DataByTar,
					c => DoNextLocComplete());
			}
		}

		private bool _isNextOk;
		private void DoNextLoc()
		{
			_isNextOk = false;
			try
			{
				_selectedF151002DataByTar =
					_f151002DataByTars.Where(o => o.STATUS == "1" && o.TAR_QTY > 0 && o.ROWNUM > _selectedF151002DataByTar.ROWNUM)
						.OrderBy(o => o.ROWNUM).First();
				_isNextOk = true;
			}
			finally
			{
				if (!_isNextOk)
				{
					ChangeAllocationStatusToOrginal();
					ExitClick();
				}
			}
		}

		private void DoNextLocComplete()
		{
			if (_isNextOk)
			{
				BindLocData();
				ScanItemCode = "";
				ScanTarLocCode = "";
				SetDefaultFocusClick();		
			}
		}

		#endregion

		#region 缺貨

		public ICommand OutOfStockCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoOutOfStock(),
					() => _selectedF151002DataByTar != null && _selectedF151002DataByTar.STATUS == "1",
					c => DoOutOfStockComplete());

			}
		}
		private bool _isOkOutOfStock;
		private void DoOutOfStock()
		{
			_isOkOutOfStock = false;
			if (DialogService.ShowMessage(Properties.Resources.P0803010000_OutOfStockAsk, Properties.Resources.P0801010101_Ask, DialogButton.YesNo, DialogImage.Question) == DialogResponse.Yes)
			{
				try
				{
					var proxyEx = GetExProxy<P08ExDataSource>();
					var data = proxyEx.CreateQuery<ExecuteResult>("OutOfStockByP080302")
						.AddQueryOption("dcCode", string.Format("'{0}'", _tempDcCode))
						.AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
						.AddQueryOption("custCode", string.Format("'{0}'", CustCode))
						.AddQueryOption("allocationNo", string.Format("'{0}'", _tempAllocationNo))
						.AddQueryOption("sugLocCode", string.Format("'{0}'", LocCode))
						.AddQueryOption("itemCode", string.Format("'{0}'", ItemCode))
						.AddQueryOption("validDate", string.Format("'{0}'", _selectedF151002DataByTar.VALID_DATE.ToString("yyyy/MM/dd")))
						.AddQueryOption("serialNo", string.Format("'{0}'", SerialNo ?? ""))
                        .AddQueryOption("makeNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(_selectedF151002DataByTar.MAKE_NO) ? "0" : _selectedF151002DataByTar.MAKE_NO))
                        .AddQueryOption("boxCrtlNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(_selectedF151002DataByTar.BOX_CTRL_NO) ? "0" : _selectedF151002DataByTar.BOX_CTRL_NO))
                        .AddQueryOption("palletNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(_selectedF151002DataByTar.PALLET_CTRL_NO) ? "0" : _selectedF151002DataByTar.PALLET_CTRL_NO))
                        .ToList();
					if (data.First().IsSuccessed)
					{
						DoSearch(true);
						_isOkOutOfStock = true;
					}
				}
				finally
				{
					if (!_isOkOutOfStock)
					{
						ChangeAllocationStatusToOrginal();
						ExitClick();
					}
				}
			}
		}

		private void DoOutOfStockComplete()
		{
			if (_isOkOutOfStock)
				AutoNextItemOrCompleteTar(_selectedF151002DataByTar.ROWNUM);
		}
		#endregion

		#region 求救

		public ICommand HelpCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoHelp(),
					() => _selectedF151002DataByTar != null && _selectedF151002DataByTar.STATUS == "1");
			}
		}

		private void DoHelp()
		{
			if (DialogService.ShowMessage(Properties.Resources.P0803010000_HelpAsk, Properties.Resources.P0801010101_Ask, DialogButton.YesNo, DialogImage.Question) == DialogResponse.Yes)
			{
				bool isOk = false;
				try
				{
					var f0010 = new F0010
					{
						HELP_TYPE = "06", //調撥作業
						ORD_NO = _tempAllocationNo,
						STATUS = "0",
						DEVICE_PC = Wms3plSession.Get<GlobalInfo>().ClientIp,
						DC_CODE = _tempDcCode,
						LOC_CODE = LocCode,
						FLOOR = LocCode.Substring(0, 1),
						GUP_CODE = GupCode,
						CUST_CODE = CustCode
					};
					var proxy = GetModifyQueryProxy<F00Entities>();
					proxy.AddToF0010s(f0010);
					proxy.SaveChanges();
					isOk = true;
				}
				finally
				{
					if (!isOk)
					{
						ChangeAllocationStatusToOrginal();
						ExitClick();
					}
				}
			}
		}



		#endregion

		#region 完成

		public ICommand FinishCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoFinish(),
					() => _f151002DataByTars != null && _f151002DataByTars.Any(o => o.STATUS == "1" && o.TAR_QTY > 0) && _selectedF151002DataByTar != null,
					c => DoFinishComplete());
			}
		}

		private void DoFinish()
		{

		}

		private void DoFinishComplete()
		{
			var isOk = false;
			try
			{
				var message = new MessagesStruct
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Message = "",
					Title = WpfClient.Resources.Resources.Information
				};

				if (int.Parse(BTarQty) > int.Parse(ATarQty))
				{
					message.Message = Properties.Resources.P0803020000_BTarQtyOverATarQty;
					ShowMessage(message);
				}

				AutoNextItemOrCompleteTar(_selectedF151002DataByTar.ROWNUM);
				isOk = true;
			}
			finally
			{
				if (!isOk)
				{
					ChangeAllocationStatusToOrginal();
					ExitClick();
				}
			}
		}

		#endregion

		#region 離開

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
			if (!string.IsNullOrEmpty(_tempAllocationNo))
				ChangeAllocationStatusToOrginal();
		}

		private void DoExitComplete()
		{
			ExitClick();
		}

		#endregion

		#region 自動跳至下一筆未上架(STATUS = 1)或已完成上架再找一筆新的調撥單

		private void AutoNextItemOrCompleteTar(int rowNum)
		{
			if (_f151002DataByTars.Any(o => o.STATUS == "1" && o.TAR_QTY > 0))
			{
				_selectedF151002DataByTar = _f151002DataByTars.Any(o => o.STATUS == "1" && o.TAR_QTY > 0 && o.ROWNUM > rowNum) ? _f151002DataByTars.First(o => o.STATUS == "1" && o.TAR_QTY > 0 && o.ROWNUM > rowNum) : _f151002DataByTars.First(o => o.STATUS == "1" && o.TAR_QTY > 0);
                _tempActualTarQty = 0;
                BindLocData();
				ActualTarQty = "0";
				ScanTarLocCode = "";
				ScanItemCode = "";
				SetDefaultFocusClick();
			}
			else
			{

				//在更新前先檢查 F151003 是否有缺貨, 若缺貨狀態(尚未確認 ) 不得更新F151001主檔狀態
				var f15Proxy = GetProxy<F15Entities>();
				var f151003Data = f15Proxy.F151003s.Where(o => o.DC_CODE == _tempDcCode && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode
									&& o.ALLOCATION_NO == _tempAllocationNo && o.STATUS == "0").FirstOrDefault();
				if (f151003Data != null) //有缺貨資料,且未確認 不得更新F151001
				{
					DialogService.ShowMessage(Properties.Resources.P0803010000_OutOfStockCheckMessage);
					ChangeAllocationStatusToOrginal(true);
					GetNextAllocation();
				}
				else
				{

					var proxyEx = GetExProxy<P08ExDataSource>();
					var result = proxyEx.CreateQuery<ExecuteResult>("ChangeAllocationDownOrUpFinish")
						.AddQueryExOption("dcCode", _tempDcCode)
						.AddQueryExOption("gupCode", GupCode)
						.AddQueryExOption("custCode", CustCode)
						.AddQueryExOption("allocationNo", _tempAllocationNo)
						.AddQueryExOption("isUp", "1")
						.ToList();

					BindLocData();

					var message = new MessagesStruct
					{
						Button = DialogButton.OK,
						Image = DialogImage.Information,
						Message = Properties.Resources.P0803020000_NextItemOrCompleteTar,
						Title = WpfClient.Resources.Resources.Information
					};
					ShowMessage(message);
					if (IsCompleteTarClose)
						ExitClick();
					else
						GetNextAllocation();

				}
			}
		}
		public void GetNextAllocation()
		{
			AllocationNo = "";
			_tempAllocationNo = "";
			ScanItemCode = "";
			_tempActualTarQty = 0;
			ActualTarQty = "0";
			_selectedF151002DataByTar = null;
			SearchCommand.Execute(null);
			SetDefaultFocusClick();
		}
		#endregion

		#region 更改調撥單回原狀態

		private void ChangeAllocationStatusToOrginal(bool lackType =false )
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("ChangeAllocationDownOrUpStatusToOrginal")
				.AddQueryExOption("dcCode", _tempDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("allocationNo", _tempAllocationNo)
				.AddQueryExOption("status", _allocationOrginalStatus)
				.AddQueryExOption("isUp", "1")
				.AddQueryExOption("lackType", lackType)
				.ToList();

		}

		#endregion

		#region grid 商品儲位實揀數資料繫結

		private void BindItemLocActual()
		{

			var proxyEx = GetExProxy<P08ExDataSource>();
			F151002ItemLocDataByTars = proxyEx.CreateQuery<F151002ItemLocDataByTar>("GetF151002ItemLocDataByTars")
				.AddQueryOption("dcCode", string.Format("'{0}'", _tempDcCode))
				.AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", CustCode))
				.AddQueryOption("allocationNo", string.Format("'{0}'", AllocationNo))
				.AddQueryOption("itemCode", string.Format("'{0}'", ItemCode))
				.AddQueryExOption("isDiffWareHouse", IsDifferentWareHouse ? "1" : "0")
				.ToList();

            if (F151002ItemLocDataByTars.Any())
				_selectedF151002ItemLocDataByTar = F151002ItemLocDataByTars.First();
		}

		#endregion

		#region 刷讀品項/條碼/序號

		public void ScanItemCodeAddActualQty()
		{
			bool isOk = false;
			try
			{
				if (!string.IsNullOrEmpty(ScanItemCode))
				{
					var message = new MessagesStruct
					{
						Button = DialogButton.OK,
						Image = DialogImage.Warning,
						Message = "",
						Title = WpfClient.Resources.Resources.Information
					};
					if (!string.IsNullOrEmpty(SerialNo) && ScanItemCode == ItemCode)
					{
						message.Message = Messages.ScanSerialOrBoxMessage;
						if (IsVoice)
							PlaySoundHelper.Oo();
						ShowMessage(message);
					}
					else
					{
						var statusResult = CheckItemLocStatus(_selectedF151002DataByTar.DC_CODE,_tempDcCode, GupCode, CustCode, ItemCode, LocCode, ScanTarLocCode,
							ValidDate.Value.ToString("yyyy/MM/dd"));
						if (!statusResult)
						{
							isOk = true;
							return;
						}
						string scanCode = string.Empty;
						if (ScanItemCode != ItemCode
							&& ScanItemCode != _selectedF151002DataByTar.EAN_CODE1
							&& ScanItemCode != _selectedF151002DataByTar.EAN_CODE2
							&& ScanItemCode != _selectedF151002DataByTar.EAN_CODE3)
						{
							scanCode = ScanItemCode;
						}
						AllocationNo = _tempAllocationNo;
						RunUpdateAdjustActualQty(1, scanCode);
					}
				}
				isOk = true;
			}
			finally
			{
				if (!isOk)
				{
					ChangeAllocationStatusToOrginal();
					ExitClick();
				}
			}
		}

		#endregion

		#region 調整實際上架數

		private void RunUpdateAdjustActualQty(long addQty, string scanCode = null)
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
            var result = proxyEx.CreateQuery<ExecuteResult>("ScanTarLocItemCodeActualQty")
                        .AddQueryOption("tarDcCode", string.Format("'{0}'", _selectedDcCode))
                        .AddQueryOption("dcCode", string.Format("'{0}'", _tempDcCode))
                        .AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
                        .AddQueryOption("custCode", string.Format("'{0}'", CustCode))
                        .AddQueryOption("allocationNo", string.Format("'{0}'", _tempAllocationNo))
                        .AddQueryOption("sugLocCode", string.Format("'{0}'", LocCode))
                        .AddQueryOption("tarLocCode", string.Format("'{0}'", string.IsNullOrWhiteSpace(ScanTarLocCode) ? LocCode : ScanTarLocCode))
                        .AddQueryOption("itemCode", string.Format("'{0}'", ItemCode))
                        .AddQueryOption("serialNo", string.Format("'{0}'", SerialNo ?? ""))
                        .AddQueryOption("orginalValidDate", string.Format("'{0}'", _selectedF151002DataByTar.VALID_DATE.ToString("yyyy/MM/dd")))
                        .AddQueryOption("newValidDate", string.Format("'{0}'", ValidDate.Value.ToString("yyyy/MM/dd")))
                        .AddQueryOption("addActualQty", string.Format("{0}", addQty))
                        .AddQueryOption("userId", string.Format("'{0}'", _userId))
                        .AddQueryOption("wareHouseId", string.Format("'{0}'", _selectedF151002DataByTar.TAR_WAREHOUSE_ID))
                        .AddQueryExOption("scanCode", scanCode ?? "")
                        .AddQueryOption("orginalMakeNo", string.Format("'{0}'", _selectedF151002DataByTar.MAKE_NO))
                        .AddQueryOption("newMakeNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(MakeNo) ? "0" : MakeNo))
                        .AddQueryOption("palletCtrlNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(PalletNo) ? "0" : PalletNo))
                        .AddQueryOption("boxCtrlNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(BoxCrtlNo) ? "0" : BoxCrtlNo))
                        .ToList();
			if (result[0].IsSuccessed)
			{
				_tempActualTarQty += addQty;
				ActualTarQty = _tempActualTarQty.ToString();
				DoSearch(true);
				if (IsVoice)
					PlaySoundHelper.Scan();
				var findItem =
							_f151002DataByTars.First(o => o.ALLOCATION_NO == _tempAllocationNo && o.DC_CODE == _tempDcCode &&
																   o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
																   o.SUG_LOC_CODE == LocCode && o.ITEM_CODE == ItemCode &&
																   o.VALID_DATE == _selectedF151002DataByTar.VALID_DATE &&
                                                                   o.MAKE_NO == _selectedF151002DataByTar.MAKE_NO &&
																   o.SERIAL_NO == SerialNo && o.TAR_QTY > 0);
				if (findItem.STATUS == "1")
				{
					_selectedF151002DataByTar = findItem;
					BindLocData();
				}
				else
					AutoNextItemOrCompleteTar(_selectedF151002DataByTar.ROWNUM);
			}
			else
			{
				ShowWarningMessage(result[0].Message);
				if (IsVoice)
					PlaySoundHelper.Oo();
			}
		}

		public void AdjustActualQty()
		{
			bool isOk = false;
			try
			{
				int qty;
				var message = new MessagesStruct
				{
					Button = DialogButton.OK,
					Image = DialogImage.Warning,
					Message = "",
					Title = WpfClient.Resources.Resources.Information
				};
				if (string.IsNullOrEmpty(ScanItemCode))
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					message.Message = Properties.Resources.P0803010000_ScanItemCode + ItemCode + "]";
					ShowMessage(message);
				}
				else if (!string.IsNullOrEmpty(SerialNo) && ScanItemCode == ItemCode)
				{
					message.Message = Messages.ScanSerialOrBoxMessage;
					if (IsVoice)
						PlaySoundHelper.Oo();

					ShowMessage(message);
				}
				else if (!int.TryParse(ActualTarQty, out qty))
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					message.Message = Properties.Resources.P0803010000_ActualSrcQtyIsNull;
					ShowMessage(message);
					ActualTarQty = _tempActualTarQty.ToString();
				}
				else if ((qty - _tempActualTarQty) > int.Parse(BTarQty) - int.Parse(ATarQty))
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					message.Message = Properties.Resources.P0803010000_ActualSrcQtyError;
					ShowMessage(message);
					ActualTarQty = _tempActualTarQty.ToString();
				}
				else if (qty <= _tempActualTarQty)
				{
					PlaySoundHelper.Oo();
					message.Message = Properties.Resources.P0803020000_ActualTarQtyError;
					ShowMessage(message);
					ActualTarQty = _tempActualTarQty.ToString();

				}
				else
				{
					// 檢查商品儲位狀態
					var statusResult = CheckItemLocStatus(_tempDcCode, _selectedF151002DataByTar.DC_CODE, GupCode, CustCode, ItemCode, LocCode, ScanTarLocCode,
						ValidDate.Value.ToString("yyyy/MM/dd"));
					if (!statusResult)
					{
						isOk = true;
						return;
					}
					string scanCode = string.Empty;
					if (ScanItemCode != ItemCode
						&& ScanItemCode != _selectedF151002DataByTar.EAN_CODE1
						&& ScanItemCode != _selectedF151002DataByTar.EAN_CODE2
						&& ScanItemCode != _selectedF151002DataByTar.EAN_CODE3)
					{
						scanCode = ScanItemCode;
					}
					var inputActualSrcQty = int.Parse(ActualTarQty);
					var addQty = inputActualSrcQty - _tempActualTarQty;
					RunUpdateAdjustActualQty(addQty, scanCode);
				}
				isOk = true;
			}
			finally
			{
				if (!isOk)
				{
					ChangeAllocationStatusToOrginal();
					ExitClick();
				}


			}
		}



		#endregion

		#region 清除儲商品位實際上架數

		public void ClearLocItemActualQty(F151002ItemLocDataByTar f151002ItemLocDataByTar)
		{
			bool isOk = false;
			try
			{
				var proxy = GetProxy<F15Entities>();

					var proxyEx = GetExProxy<P08ExDataSource>();
                    //以後可能會加上板號、箱號
                    var result = proxyEx.CreateQuery<ExecuteResult>("RemoveTarLocItemCodeActualQty")
                        .AddQueryOption("dcCode", string.Format("'{0}'", f151002ItemLocDataByTar.DC_CODE))
                        .AddQueryOption("gupCode", string.Format("'{0}'", f151002ItemLocDataByTar.GUP_CODE))
                        .AddQueryOption("custCode", string.Format("'{0}'", f151002ItemLocDataByTar.CUST_CODE))
                        .AddQueryOption("allocationNo", string.Format("'{0}'", f151002ItemLocDataByTar.ALLOCATION_NO))
                        .AddQueryOption("tarLocCode", string.Format("'{0}'", f151002ItemLocDataByTar.TAR_LOC_CODE))
                        .AddQueryOption("itemCode", string.Format("'{0}'", f151002ItemLocDataByTar.ITEM_CODE))
                        .AddQueryOption("removeValidDate",
                            string.Format("'{0}'", f151002ItemLocDataByTar.TAR_VALID_DATE.ToString("yyyy/MM/dd")))
                        .AddQueryOption("removeMakeNo",
                            string.Format("'{0}'", f151002ItemLocDataByTar.TAR_MAKE_NO))
                        .FirstOrDefault();
                    if (!result.IsSuccessed)
                    {
                        var message = new MessagesStruct
                        {
                            Button = DialogButton.OK,
                            Image = DialogImage.Warning,
                            Message = result.Message,
                            Title = WpfClient.Resources.Resources.Information
                        };
                        ShowMessage(message);
                    }

                    var locData = proxy.F151002s.Where(o =>
						o.DC_CODE == _tempDcCode && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
						o.ALLOCATION_NO == _tempAllocationNo &&
						o.ITEM_CODE == ItemCode && o.SUG_LOC_CODE == LocCode).ToList();
					locData =
						locData.Where(
							o =>
								(o.VALID_DATE == _selectedF151002DataByTar.VALID_DATE && !o.SRC_VALID_DATE.HasValue) ||
								(o.SRC_VALID_DATE.HasValue && o.SRC_VALID_DATE == _selectedF151002DataByTar.VALID_DATE)).ToList();
					locData = string.IsNullOrEmpty(SerialNo)
						? locData.Where(o => string.IsNullOrEmpty(o.SERIAL_NO)).ToList()
						: locData.Where(o => o.SERIAL_NO == SerialNo).ToList();
					var aTarQty = locData.Sum(o => o.A_TAR_QTY);

					if (aTarQty <= _tempActualTarQty)
						_tempActualTarQty = aTarQty;

					ActualTarQty = _tempActualTarQty.ToString();

					DoSearch(true);
					var findItem =
						_f151002DataByTars.First(o => o.ALLOCATION_NO == _tempAllocationNo && o.DC_CODE == _tempDcCode &&
															   o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
															   o.SUG_LOC_CODE == LocCode && o.ITEM_CODE == ItemCode &&
															   o.VALID_DATE == _selectedF151002DataByTar.VALID_DATE &&
                                                               o.MAKE_NO == _selectedF151002DataByTar.MAKE_NO &&
															   o.SERIAL_NO == SerialNo && o.TAR_QTY > 0);
					if (findItem.STATUS == "1")
					{
						_selectedF151002DataByTar = findItem;
						BindLocData();
					}
				isOk = true;
			}
			finally
			{
				if (!isOk)
				{
					ChangeAllocationStatusToOrginal();
					ExitClick();
				}
			}
		}

		#endregion

		#region 檢查目的儲位是否登入者有權限使用
		public void CheckTraLocCode()
		{
			bool isOk = false;

			try
			{
				var proxyEx = GetExProxy<P08ExDataSource>();
				var data = proxyEx.CreateQuery<ExecuteResult>("CheckTarLocCode")
					.AddQueryExOption("dcCode",  _selectedDcCode)
					.AddQueryExOption("wareHouseId", _selectedF151002DataByTar.TAR_WAREHOUSE_ID)
					.AddQueryExOption("locCode", ScanTarLocCode ?? "")
					.AddQueryExOption("userId", _userId)
					.AddQueryExOption("itemCode", ItemCode)
					.ToList();
				if (!data[0].IsSuccessed)
				{
					var message = new MessagesStruct
					{
						Button = DialogButton.OK,
						Image = DialogImage.Warning,
						Message = "",
						Title = WpfClient.Resources.Resources.Information
					};
					message.Message = data[0].Message;
					if (IsVoice)
						PlaySoundHelper.Oo();
					ShowMessage(message);
					SetDefaultFocusClick();
				}
				else
				{
					if (IsVoice)
						PlaySoundHelper.Scan();
					SetScanItemCodeFocusClick();
				}
				isOk = true;
			}
			finally
			{
				if (!isOk)
				{
					ChangeAllocationStatusToOrginal();
					ExitClick();
				}
			}

		}

		#endregion

		#region 檢查商品與儲位狀態
		private bool CheckItemLocStatus(string dcCode,string tarDcCode, string gupCode, string custCode, string itemCode, string srcLocCode, string tarLocCode, string validDate)
		{

			//檢查商品與儲位狀態    
			var proxyP19Ex = GetExProxy<P19EX.P19ExDataSource>();
			var checkResult = proxyP19Ex.CreateQuery<P19EX.ExecuteResult>("CheckItemLocStatus")
											.AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
											.AddQueryOption("tarDcCode", string.Format("'{0}'", tarDcCode))
											.AddQueryOption("gupCode", string.Format("'{0}'", gupCode))
											.AddQueryOption("custCode", string.Format("'{0}'", custCode))
											.AddQueryOption("itemCode", string.Format("'{0}'", itemCode))
											.AddQueryOption("srcLocCode", string.Format("'{0}'", srcLocCode))
											.AddQueryOption("tarLocCode", string.Format("'{0}'", string.IsNullOrWhiteSpace(tarLocCode) ? srcLocCode : tarLocCode))
											.AddQueryOption("validDate", string.Format("'{0}'", validDate)).AsQueryable();
			if (!checkResult.First().IsSuccessed)
			{
				DialogService.ShowMessage(string.Format(Properties.Resources.P0803020000_ItemCode, ItemCode) + checkResult.First().Message);
				return false;
			}
			return true;
		}
        #endregion

        #region 檢查該貨主是否可以取消上架
        public void CheckIsUpShelfCancel()
        {
            var porxy = GetProxy<F19Entities>();
            var isUpShelfCancel = porxy.F1909s.Where(o => o.CUST_CODE == CustCode && o.GUP_CODE == GupCode).FirstOrDefault().IS_UP_SHELF_CANCEL;
            IsClearEnabled = isUpShelfCancel == "1";
        }
        #endregion
    }
}
