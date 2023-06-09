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
using Wms3pl.WpfClient.P08.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Common.Enums;
using wcf = Wms3pl.WpfClient.ExDataServices.P15WcfService;
using System.IO;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P08ExDataService.ExecuteResult;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0803010000_ViewModel : InputViewModelBase
	{
		public P0803010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
			}

		}


		#region 資料繫結

		public void InitBindData()
		{
			ActualSrcQty = "0";
			IsVoice = true;
            SetMakeNoVaildDateChange();
            SetDcList();
		}

		#endregion

		#region Property

		private string _userId;
		private string _userName;
		public Timer MarqueeTimer;
		public Action RunMarquee = delegate { };
		public Action ExitClick = delegate { };
		public Action SetDefaultFocusClick = delegate { };
		public Action SetCaseNoFocusClick = delegate { };

		public Action SetAllocationNoFocusClick = delegate { };
		public Action OpenTarClick = delegate { };

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
				AllocationNo = "";
				_selectedF151002Data = null;
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

		public string tempAllocationNo;

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

		#region 下架倉別
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

		#region 儲位應下架數

		private string _bSrcQty;
		public string BSrcQty
		{
			get { return _bSrcQty; }
			set
			{
				_bSrcQty = value;
				RaisePropertyChanged("BSrcQty");

			}
		}

		#endregion

		#region 儲位實際下架數

		private string _aSrcQty;
		public string ASrcQty
		{
			get { return _aSrcQty; }
			set
			{
				_aSrcQty = value;
				RaisePropertyChanged("ASrcQty");

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

		#region 實際下架數

		private long _tempActualSrcQty = 0;

		private string _actualSrcQty;

		public string ActualSrcQty
		{
			get { return _actualSrcQty; }
			set
			{
				_actualSrcQty = value;
				RaisePropertyChanged("ActualSrcQty");
			}
		}

		private bool _isEnabledActualSrcQty;
		public bool IsEnabledActualSrcQty
		{
			get { return _isEnabledActualSrcQty; }
			set
			{
				_isEnabledActualSrcQty = value;
				RaisePropertyChanged("IsEnabledActualSrcQty");
			}
		}
		#endregion

		#region 商品已揀數

		private string _aItemSrcQty;
		public string AItemSrcQty
		{
			get { return _aItemSrcQty; }
			set
			{
				_aItemSrcQty = value;
				RaisePropertyChanged("AItemSrcQty");

			}
		}
		#endregion

		#region 商品應揀數

		private string _bItemSrcQty;
		public string BItemSrcQty
		{
			get { return _bItemSrcQty; }
			set
			{
				_bItemSrcQty = value;
				RaisePropertyChanged("BItemSrcQty");

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


		#region 箱號

		public string _tempCaseNo;
		private string _caseNo;

		public string CaseNo
		{
			get { return _caseNo; }
			set
			{
				if (_caseNo == value)
					return;
				Set(() => CaseNo, ref _caseNo, value);
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

		private List<F151002Data> _f151002Datas;

		private F151002Data _selectedF151002Data;
		#endregion

		#region 調撥單儲位商品實揀數繫結

		private List<F151002ItemLocData> _f151002ItemLocDatas;

		public List<F151002ItemLocData> F151002ItemLocDatas
		{
			get { return _f151002ItemLocDatas; }
			set
			{
				_f151002ItemLocDatas = value;
				RaisePropertyChanged("F151002ItemLocDatas");
			}
		}

		private F151002ItemLocData _selectedF151002ItemLocData;

		public F151002ItemLocData SelectedF151002ItemLocData
		{
			get { return _selectedF151002ItemLocData; }
			set
			{
				_selectedF151002ItemLocData = value;
				RaisePropertyChanged("SelectedF151002ItemLocData");
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
            _allocationChangValiDate = tmp.ALLOCATIONCHANGVALIDATE == "1" || tmp.ALLOCATIONCHANGVALIDATE == "3";
            _allocationChangMakeNo = tmp.ALLOCATIONCHANGMAKENO == "1" || tmp.ALLOCATIONCHANGMAKENO == "3";
        }

        #endregion

        #region 取得商品圖檔

        private void RefreshImage()
		{
			ItemImageSource = null;
			RaisePropertyChanged("ItemImageSource");
		}

		#endregion

		#region 設定調撥單商品儲位資料

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

		public void DoSearch(bool isAllowStatus2)	//isAllowStatus2 1:允許Status=2 0:不允許Status = 2
		{
			if (tempAllocationNo != AllocationNo)
			{
				CaseNo = "";
				_tempCaseNo = "";
			}
			if (!isAllowStatus2 && !string.IsNullOrEmpty(tempAllocationNo))
				ChangeAllocationStatusToOrginal();
			var proxyEx = GetExProxy<P08ExDataSource>();
			_f151002Datas = proxyEx.CreateQuery<F151002Data>("GetF151002Datas")
				.AddQueryExOption("srcDcCode",  _selectedDcCode)
				.AddQueryExOption("gupCode",  GupCode)
				.AddQueryExOption("custCode",  CustCode)
				.AddQueryExOption("allocationNo",  AllocationNo)
				.AddQueryExOption("userId",  _userId)
				.AddQueryExOption("userName", _userName)
				.AddQueryExOption("isAllowStatus2", isAllowStatus2 ? "1" : "0")
				.AddQueryExOption("isDiffWareHouse", IsDifferentWareHouse ? "1" : "0")
				.ToList();

		}

		private void DoSearchComplete()
		{
			ScanItemCode = "";
			_tempActualSrcQty = 0;
			ActualSrcQty = "";
			if (_f151002Datas.Any(o => o.STATUS == "0" && o.SRC_QTY > 0))
			{
				_selectedF151002Data = _f151002Datas.First(o => o.STATUS == "0" && o.SRC_QTY > 0);
				var proxyEx = GetExProxy<P08ExDataSource>();
				var result = proxyEx.CreateQuery<ExecuteResult>("StartDownOrUpItemChangeStatus")
					.AddQueryExOption("dcCode", _selectedF151002Data.DC_CODE)
					.AddQueryExOption("gupCode", GupCode)
					.AddQueryExOption("custCode", CustCode)
					.AddQueryExOption("allocationNo", _selectedF151002Data.ALLOCATION_NO)
					.AddQueryExOption("isUp", "0")
					.ToList();
				if (result.First().IsSuccessed)
					_allocationOrginalStatus = result.First().Message; //暫存原狀態 (可能是0 or 1)
			}
			else
				_selectedF151002Data = null;

			BindLocData();
		}

		private void BindLocData()
		{
			bool isOk = false;
			try
			{
				if (_selectedF151002Data != null)
				{
					AllocationNo = _selectedF151002Data.ALLOCATION_NO;
					tempAllocationNo = AllocationNo;
					_tempDcCode = _selectedF151002Data.DC_CODE;
					WareHouseName = _selectedF151002Data.WAREHOUSE_NAME;
					LocCode = _selectedF151002Data.SRC_LOC_CODE;
					ItemCode = _selectedF151002Data.ITEM_CODE;
					ItemName = _selectedF151002Data.ITEM_NAME;
					ValidDate = _selectedF151002Data.VALID_DATE;
					SerialNo = _selectedF151002Data.SERIAL_NO;
					SERIAL_NOByShow = _selectedF151002Data.SERIAL_NOByShow;
					RetUnit = _selectedF151002Data.RET_UNIT;
					BoxNo = _selectedF151002Data.BOX_NO;
                    MakeNo = _selectedF151002Data.MAKE_NO == "0" ? "" : _selectedF151002Data.MAKE_NO;
                    BoxCrtlNo = _selectedF151002Data.BOX_CTRL_NO == "0" ? "" : _selectedF151002Data.BOX_CTRL_NO;
                    PalletNo = _selectedF151002Data.PALLET_CTRL_NO == "0" ? "" : _selectedF151002Data.PALLET_CTRL_NO;
           
					var proxy = GetProxy<F15Entities>();
					// 取得該調撥單明細中相同品號的清單
					var items = proxy.F151002s.Where(
						o =>
							o.DC_CODE == _tempDcCode && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
							o.ALLOCATION_NO == tempAllocationNo && o.ITEM_CODE == ItemCode).ToList();

					// 顯示總已下架/總應下架
					AItemSrcQty = items.Sum(o => o.A_SRC_QTY).ToString();
					BItemSrcQty = items.Sum(o => o.SRC_QTY).ToString();

                    // 取得該相同來源儲位與效期與序號的清單
                    var locData = items.Where(o => o.VALID_DATE == ValidDate && o.SRC_LOC_CODE == LocCode &&
                    o.MAKE_NO == (string.IsNullOrWhiteSpace(MakeNo) ? "0" : MakeNo) &&
                    o.BOX_CTRL_NO == (string.IsNullOrWhiteSpace(BoxCrtlNo) ? "0" : BoxCrtlNo) &&
                    o.PALLET_CTRL_NO == (string.IsNullOrWhiteSpace(PalletNo) ? "0" : PalletNo)).ToList();

					locData = string.IsNullOrEmpty(SerialNo)
							? locData.Where(o => string.IsNullOrEmpty(o.SERIAL_NO)).ToList()
							: locData.Where(o => o.SERIAL_NO == SerialNo).ToList();

					// 顯示已下架數/應下架數
					var aSrcQty = locData.Sum(o => o.A_SRC_QTY);
					ASrcQty = aSrcQty.ToString();
					BSrcQty = _selectedF151002Data.SRC_QTY.ToString();

					IsEnabledActualSrcQty = (_selectedF151002Data.STATUS == "0");
					IsEnabledScanItemCode = (_selectedF151002Data.STATUS == "0");
                    IsEnabledValidDate = _allocationChangValiDate ? (_selectedF151002Data.STATUS == "0") : _allocationChangValiDate;
                    IsEnabledMakeNo = _allocationChangMakeNo ? (_selectedF151002Data.STATUS == "0") : _allocationChangMakeNo;

                    BindItemLocActual();
					if (!IsDifferentWareHouse)
						SetDefaultFocusClick();
					else if (string.IsNullOrEmpty(_tempCaseNo))
					{
						SetCaseNoFocusClick();
					}
					else
						SetDefaultFocusClick();

                    if (aSrcQty == 0)
                    {
                        ActualSrcQty = "0";
                        _tempActualSrcQty = 0;
                    }
                    else
                    {
                        ActualSrcQty = ASrcQty;
                        _tempActualSrcQty = aSrcQty;
                    }
				}
				else
				{
					AllocationNo = "";
					CaseNo = "";
					_tempCaseNo = "";
					tempAllocationNo = "";
					_tempDcCode = string.Empty;
					WareHouseName = "";
					LocCode = "";
					ItemCode = "";
					ItemName = "";
					ValidDate = null;
					SerialNo = "";
					RetUnit = "";
					ASrcQty = "";
					BSrcQty = "";
					AItemSrcQty = "";
					BItemSrcQty = "";
					_tempActualSrcQty = 0;
					ActualSrcQty = "";
					SERIAL_NOByShow = "";
                    MakeNo = "";
                    BoxCrtlNo = "";
                    PalletNo = "";
					RefreshImage();
					F151002ItemLocDatas = null;
					IsEnabledActualSrcQty = false;
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
					() => _f151002Datas != null && _f151002Datas.Any(o => o.STATUS == "0" && o.SRC_QTY > 0) && _selectedF151002Data != null && _f151002Datas.First(o => o.STATUS == "0" && o.SRC_QTY > 0) != _selectedF151002Data,
					c => DoPrevLocComplete());
			}
		}

		private bool _isPrevOk;
		private void DoPrevLoc()
		{
			_isPrevOk = false;
			try
			{
				_selectedF151002Data =
					_f151002Datas.Where(o => o.STATUS == "0" && o.SRC_QTY > 0 && o.ROWNUM < _selectedF151002Data.ROWNUM)
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
					() => _f151002Datas != null && _f151002Datas.Any(o => o.STATUS == "0" && o.SRC_QTY > 0) && _selectedF151002Data != null && _f151002Datas.Last(o => o.STATUS == "0" && o.SRC_QTY > 0) != _selectedF151002Data,
					c => DoNextLocComplete());
			}
		}

		private bool _isNextOk;
		private void DoNextLoc()
		{
			_isNextOk = false;
			try
			{
				_selectedF151002Data =
					_f151002Datas.Where(o => o.STATUS == "0" && o.SRC_QTY > 0 && o.ROWNUM > _selectedF151002Data.ROWNUM)
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
					() => _selectedF151002Data != null && _selectedF151002Data.STATUS == "0",
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
                    var data = proxyEx.CreateQuery<ExecuteResult>("OutOfStockByP080301")
                        .AddQueryOption("dcCode", string.Format("'{0}'", _tempDcCode))
                        .AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
                        .AddQueryOption("custCode", string.Format("'{0}'", CustCode))
                        .AddQueryOption("allocationNo", string.Format("'{0}'", tempAllocationNo))
                        .AddQueryOption("srcLocCode", string.Format("'{0}'", LocCode))
                        .AddQueryOption("itemCode", string.Format("'{0}'", ItemCode))
                        .AddQueryOption("validDate", string.Format("'{0}'", _selectedF151002Data.VALID_DATE.ToString("yyyy/MM/dd")))
                        .AddQueryOption("serialNo", string.Format("'{0}'", SerialNo ?? ""))
                        .AddQueryOption("makeNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(_selectedF151002Data.MAKE_NO) ? "0" : _selectedF151002Data.MAKE_NO))
                        .AddQueryOption("boxCrtlNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(_selectedF151002Data.BOX_CTRL_NO) ? "0" : _selectedF151002Data.BOX_CTRL_NO))
                        .AddQueryOption("palletNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(_selectedF151002Data.PALLET_CTRL_NO) ? "0" : _selectedF151002Data.PALLET_CTRL_NO))
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

		public void DoOutOfStockComplete()
		{
			if (_isOkOutOfStock)
				AutoNextItemOrCompleteSrc(_selectedF151002Data.ROWNUM);

		}

		#endregion

		#region 求救

		public ICommand HelpCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoHelp(),
					() => _selectedF151002Data != null && _selectedF151002Data.STATUS == "0");
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
						ORD_NO = tempAllocationNo,
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
					() => _f151002Datas != null && _f151002Datas.Any(o => o.STATUS == "0" && o.SRC_QTY > 0) && _selectedF151002Data != null,
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

				if (int.Parse(BSrcQty) > int.Parse(ASrcQty))
				{
					message.Message = Properties.Resources.P0803010000_BSrcQtyOverASrcQty;
					ShowMessage(message);
				}

				AutoNextItemOrCompleteSrc(_selectedF151002Data.ROWNUM);
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
			if (!string.IsNullOrEmpty(tempAllocationNo))
				ChangeAllocationStatusToOrginal();
		}

		private void DoExitComplete()
		{
			ExitClick();
		}

		#endregion

		#region 自動跳至下一筆未下架(STATUS = 0)或已完成下架跳出是否要上架 是 開啟上架視窗 否 再找一筆新的調撥單

		private void AutoNextItemOrCompleteSrc(int rowNum)
		{
			if (_f151002Datas.Any(o => o.STATUS == "0" && o.SRC_QTY > 0))
			{
				_selectedF151002Data = _f151002Datas.Any(o => o.STATUS == "0" && o.ROWNUM > rowNum && o.SRC_QTY > 0) ? _f151002Datas.First(o => o.STATUS == "0" && o.ROWNUM > rowNum && o.SRC_QTY > 0) : _f151002Datas.First(o => o.STATUS == "0" && o.SRC_QTY > 0);
                _tempActualSrcQty = 0;
                BindLocData();
				ScanItemCode = "";
				ActualSrcQty = "0";
				SetDefaultFocusClick();
			}
			else
			{
				var message = new MessagesStruct
				{
					Button = DialogButton.YesNo,
					Image = DialogImage.Question,
					Message = Properties.Resources.P0803010000_NextItemOrCompleteSrcAsk,
					Title = WpfClient.Resources.Resources.Information
				};

				//在更新前先檢查 F151003 是否有缺貨, 若缺貨狀態(尚未確認 ) 不得更新F151001主檔狀態
				var f15Proxy = GetProxy<F15Entities>();
				var f151003Data = f15Proxy.F151003s.Where(o => o.DC_CODE == _tempDcCode && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode
									&& o.ALLOCATION_NO == tempAllocationNo && o.STATUS == "0").FirstOrDefault();
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
						.AddQueryExOption("allocationNo", tempAllocationNo)
						.AddQueryExOption("isUp", "0")
						.ToList();

					if (!IsDifferentWareHouse)
					{
						if (ShowMessage(message) == DialogResponse.Yes)
							OpenTarClick();
						else
							GetNextAllocation();
					}
					else
						GetNextAllocation();

				}

			}
		}

		public void GetNextAllocation()
		{
			AllocationNo = "";
			tempAllocationNo = "";
			ScanItemCode = "";
			_tempActualSrcQty = 0;
			ActualSrcQty = "0";
			CaseNo = "";
			_selectedF151002Data = null;
			SearchCommand.Execute(null);
			if (!IsDifferentWareHouse)
				SetDefaultFocusClick();
			else
				SetCaseNoFocusClick();
		}

		#endregion

		#region 更改調撥單回原狀態
		/// <param name="lackType">true 有缺貨商品</param>
		private void ChangeAllocationStatusToOrginal(bool lackType = false)
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("ChangeAllocationDownOrUpStatusToOrginal")
				.AddQueryExOption("dcCode", _tempDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("allocationNo", tempAllocationNo)
				.AddQueryExOption("status", _allocationOrginalStatus)
				.AddQueryExOption("isUp", "0")
				.AddQueryExOption("lackType", lackType)
				.ToList();
		}

		#endregion

		#region grid 商品儲位實揀數資料繫結

		private void BindItemLocActual()
		{

			var proxyEx = GetExProxy<P08ExDataSource>();
			F151002ItemLocDatas = proxyEx.CreateQuery<F151002ItemLocData>("GetF151002ItemLocDatas")
				.AddQueryOption("dcCode", string.Format("'{0}'", _tempDcCode))
				.AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", CustCode))
				.AddQueryOption("allocationNo", string.Format("'{0}'", AllocationNo))
				.AddQueryOption("itemCode", string.Format("'{0}'", ItemCode))
				.AddQueryExOption("isDiffWareHouse", IsDifferentWareHouse ? "1" : "0").ToList();

			if (F151002ItemLocDatas.Any())
				_selectedF151002ItemLocData = F151002ItemLocDatas.First();
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
					if (IsDifferentWareHouse)
					{
                        if (string.IsNullOrEmpty(_tempCaseNo) || string.IsNullOrEmpty(CaseNo))
                        {
                            message.Message = Properties.Resources.P0803010000_CaseNoIsNull;
                            if (IsVoice)
                                PlaySoundHelper.Oo();
                            ShowMessage(message);
                            isOk = true;
                            SetCaseNoFocusClick();
                            return;
                        }
					}

					if (!string.IsNullOrEmpty(SerialNo) && ScanItemCode == ItemCode)
					{
						message.Message = Messages.ScanSerialOrBoxMessage;
						if (IsVoice)
							PlaySoundHelper.Oo();
						ShowMessage(message);
					}
					else
					{
						string scanCode = string.Empty;
						if (
							ScanItemCode != ItemCode
							&& ScanItemCode != _selectedF151002Data.EAN_CODE1
							&& ScanItemCode != _selectedF151002Data.EAN_CODE2
							&& ScanItemCode != _selectedF151002Data.EAN_CODE3)
						{
							scanCode = ScanItemCode;
						}

						AllocationNo = tempAllocationNo;
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

		#region 調整實際下架數

		private void RunUpdateAdjustActualQty(long addQty, string scanCode = null)
		{
			var proxyEx = GetExProxy<P08ExDataSource>();

			var result = proxyEx.CreateQuery<ExecuteResult>("ScanSrcLocItemCodeActualQty")
						.AddQueryExOption("dcCode", _tempDcCode)
						.AddQueryExOption("gupCode", GupCode)
						.AddQueryExOption("custCode", CustCode)
						.AddQueryExOption("allocationNo", tempAllocationNo)
						.AddQueryExOption("srcLocCode", LocCode)
						.AddQueryExOption("itemCode", ItemCode)
						.AddQueryExOption("serialNo", SerialNo ?? "")
						.AddQueryExOption("orginalValidDate", _selectedF151002Data.VALID_DATE.ToString("yyyy/MM/dd"))
						.AddQueryExOption("newValidDate", ValidDate.Value.ToString("yyyy/MM/dd"))
						.AddQueryExOption("addActualQty", addQty)
						.AddQueryExOption("scanCode", scanCode ?? "")
                        .AddQueryOption("orginalMakeNo", string.Format("'{0}'", _selectedF151002Data.MAKE_NO))
                        .AddQueryOption("newMakeNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(MakeNo) ? "0" : MakeNo))
                        .AddQueryOption("palletCtrlNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(PalletNo) ? "0" : PalletNo))
                        .AddQueryOption("boxCtrlNo", string.Format("'{0}'", string.IsNullOrWhiteSpace(BoxCrtlNo) ? "0" : BoxCrtlNo))
                        .ToList();
			if (result[0].IsSuccessed)
			{
				_tempActualSrcQty += addQty;
				ActualSrcQty = _tempActualSrcQty.ToString();
				DoSearch(true);
				if (IsVoice)
					PlaySoundHelper.Scan();
				var findItem = _f151002Datas.First(o => o.ALLOCATION_NO == AllocationNo && o.DC_CODE == _tempDcCode &&
																														o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
																														o.SRC_LOC_CODE == LocCode && o.ITEM_CODE == ItemCode &&
																														o.VALID_DATE == _selectedF151002Data.VALID_DATE && o.SERIAL_NO == SerialNo &&
                                                                                                                        o.MAKE_NO == _selectedF151002Data.MAKE_NO &&
                                                                                                                        o.SRC_QTY > 0);
				if (findItem.STATUS == "0")
				{
					_selectedF151002Data = findItem;
					BindLocData();
				}
				else
					AutoNextItemOrCompleteSrc(_selectedF151002Data.ROWNUM);
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

        if (string.IsNullOrEmpty(_tempCaseNo) || string.IsNullOrEmpty(CaseNo))
        {
          if (this.FunctionCode == "P0803030000") //僅跨倉下架需要判斷箱號
          {
            message.Message = Properties.Resources.P0803010000_CaseNoIsNull;
            if (IsVoice)
              PlaySoundHelper.Oo();
            ShowMessage(message);
            isOk = true;
            SetCaseNoFocusClick();
            return;
          }
        }
               
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
				else if (!int.TryParse(ActualSrcQty, out qty))
				{
					if (IsVoice)
						PlaySoundHelper.Oo();
					message.Message = Properties.Resources.P0803010000_ActualSrcQtyIsNull;
					ShowMessage(message);
					ActualSrcQty = _tempActualSrcQty.ToString();
				}
				else if ((qty - _tempActualSrcQty) > int.Parse(BSrcQty) - int.Parse(ASrcQty))
				{
					PlaySoundHelper.Oo();
					message.Message = Properties.Resources.P0803010000_ActualSrcQtyError;
					ShowMessage(message);
					ActualSrcQty = _tempActualSrcQty.ToString();
				}
				else if (qty <= _tempActualSrcQty)
				{
					PlaySoundHelper.Oo();
					message.Message = Properties.Resources.P0803010000_ActualSrcQtyError2;
					ShowMessage(message);
					ActualSrcQty = _tempActualSrcQty.ToString();
				}
				else
				{
					string scanCode = string.Empty;
					if (
						ScanItemCode != ItemCode
						&& ScanItemCode != _selectedF151002Data.EAN_CODE1
						&& ScanItemCode != _selectedF151002Data.EAN_CODE2
						&& ScanItemCode != _selectedF151002Data.EAN_CODE3)
					{
						scanCode = ScanItemCode;
					}
					AllocationNo = tempAllocationNo;
					var inputActualSrcQty = int.Parse(ActualSrcQty);
					var addQty = inputActualSrcQty - _tempActualSrcQty;
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

		#region 清除儲商品位實際下架數

		public void ClearLocItemActualQty(F151002ItemLocData f151002ItemLocData)
		{
			bool isOk = false;
			try
			{
				var proxy = GetProxy<F15Entities>();
           
					var proxyEx = GetExProxy<P08ExDataSource>();
                    //以後可能會加上板號、箱號
                    var result = proxyEx.CreateQuery<ExecuteResult>("RemoveSrcLocItemCodeActualQty")
                        .AddQueryOption("dcCode", string.Format("'{0}'", f151002ItemLocData.DC_CODE))
                        .AddQueryOption("gupCode", string.Format("'{0}'", f151002ItemLocData.GUP_CODE))
                        .AddQueryOption("custCode", string.Format("'{0}'", f151002ItemLocData.CUST_CODE))
                        .AddQueryOption("allocationNo", string.Format("'{0}'", f151002ItemLocData.ALLOCATION_NO))
                        .AddQueryOption("srcLocCode", string.Format("'{0}'", f151002ItemLocData.SRC_LOC_CODE))
                        .AddQueryOption("itemCode", string.Format("'{0}'", f151002ItemLocData.ITEM_CODE))
                        .AddQueryOption("removeValidDate",
                            string.Format("'{0}'", f151002ItemLocData.SRC_VALID_DATE.ToString("yyyy/MM/dd")))
                            .AddQueryOption("removeMakeNo", string.Format("'{0}'", f151002ItemLocData.SRC_MAKE_NO))
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
						o.ALLOCATION_NO == tempAllocationNo &&
						o.ITEM_CODE == ItemCode && o.VALID_DATE == _selectedF151002Data.VALID_DATE && o.SRC_LOC_CODE == LocCode).ToList();
					locData = string.IsNullOrEmpty(SerialNo)
						? locData.Where(o => string.IsNullOrEmpty(o.SERIAL_NO)).ToList()
						: locData.Where(o => o.SERIAL_NO == SerialNo).ToList();
					var aSrcQty = locData.Sum(o => o.A_SRC_QTY);
					if (aSrcQty <= _tempActualSrcQty)
						_tempActualSrcQty = aSrcQty;
					ActualSrcQty = _tempActualSrcQty.ToString();

					DoSearch(true);
					var findItem = _f151002Datas.First(o => o.ALLOCATION_NO == tempAllocationNo && o.DC_CODE == _tempDcCode &&
															o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
															o.SRC_LOC_CODE == LocCode && o.ITEM_CODE == ItemCode &&
															o.VALID_DATE == _selectedF151002Data.VALID_DATE &&
                                                            o.MAKE_NO == _selectedF151002Data.MAKE_NO &&
															o.SERIAL_NO == SerialNo && o.SRC_QTY > 0);
					if (findItem.STATUS == "0")
					{
						_selectedF151002Data = findItem;
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

		#region 刷讀箱號

		public void ScanCaseNo()
		{
			if (string.IsNullOrWhiteSpace(AllocationNo))
			{
				if (IsVoice)
					PlaySoundHelper.Oo();
				ShowWarningMessage(Properties.Resources.P0803010000_AllocationNoIsNull);
				SetAllocationNoFocusClick();
				return;
			}
			if (string.IsNullOrWhiteSpace(CaseNo))
			{
				if (IsVoice)
					PlaySoundHelper.Oo();
				ShowWarningMessage(Properties.Resources.P0803010000_CaseNoIsNull2);
				SetCaseNoFocusClick();
				return;
			}
			CaseNo = CaseNo.Trim();
			_tempCaseNo = CaseNo;
			var proxyEx = GetExProxy<P08ExDataSource>();
         
            var result = proxyEx.CreateQuery<ExecuteResult>("InsertOrUpdateF151004")
				.AddQueryExOption("dcCode", _tempDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("allocationNo", tempAllocationNo)
				.AddQueryExOption("caseNo", _tempCaseNo).ToList().First();
			if (result.IsSuccessed)
			{
				if (IsVoice)
					PlaySoundHelper.Scan();
				SetDefaultFocusClick();
			}
			else
			{
				if (IsVoice)
					PlaySoundHelper.Oo();
				ShowWarningMessage(result.Message);
			}
		}

		#endregion

	}
}
