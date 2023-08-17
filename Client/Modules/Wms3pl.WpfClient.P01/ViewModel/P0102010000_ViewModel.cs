using AutoMapper;
using CrystalDecisions.Shared.Json;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P01ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using exshare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P01WcfService;

namespace Wms3pl.WpfClient.P01.ViewModel
{
	public partial class P0102010000_ViewModel : InputViewModelBase
	{
		public Action ExcelImport = delegate { };
		public Action DoPrintPalletReport = delegate { };
		public P0102010000_ViewModel()
		{

			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				BegStockDate = DateTime.Today;
				EndStockDate = DateTime.Today;
				SetDcList();
				SetStatusList();
				SetOrdpropList();
				SetFastPassType();
				SetBoolkingInPeriod();
				SetUserClosedList();

				var proxy = GetProxy<F19Entities>();
				var f1909 = proxy.F1909s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode).FirstOrDefault();
				ShowUnitTrans = f1909.SHOW_UNIT_TRANS;
				AllowWarehouseinClosed = f1909.ALLOW_WAREHOUSEIN_CLOSED == "1";
			}
		}

		#region Property

		private readonly string _userId;
		private readonly string _userName;
		public Action SetTxtVnrCodeFocusClick = delegate { };
		public Action SetTxtCustOrdNoFocusClick = delegate { };
		public Action OnDcCodeChanged = delegate { };// 物流中心改變時，顯示 Device


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

		#region 物流中心

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

		private string _SelectedDcCode;

		public string SelectedDcCode
		{
			get { return _SelectedDcCode; }
			set
			{
				_SelectedDcCode = value;
				RaisePropertyChanged("SelectedDcCode");
				if (value != null)
					OnDcCodeChanged();
			}
		}

		#endregion

		#region 建立日期-起

		private DateTime? _begStockDate;

		public DateTime? BegStockDate
		{
			get { return _begStockDate; }
			set
			{
				_begStockDate = value;
				RaisePropertyChanged("BegStockDate");
			}
		}

		#endregion

		#region 建立日期-迄

		private DateTime? _endStockDate;

		public DateTime? EndStockDate
		{
			get { return _endStockDate; }
			set
			{
				_endStockDate = value;
				RaisePropertyChanged("EndStockDate");
			}
		}

		#endregion

		#region 進倉單號

		private string _stockNo;

		public string StockNo
		{
			get { return _stockNo; }
			set
			{
				_stockNo = value;
				RaisePropertyChanged("StockNo");
			}
		}
		#endregion

		#region 廠商編號

		private string _vnrCode;

		public string VnrCode
		{
			get { return _vnrCode; }
			set
			{
				_vnrCode = value;
				RaisePropertyChanged("VnrCode");
			}
		}

		#endregion

		#region 廠商名稱

		private string _vnrName;

		public string VnrName
		{
			get { return _vnrName; }
			set
			{
				_vnrName = value;
				RaisePropertyChanged("VnrName");
			}
		}

		#endregion

		#region 貨主單號

		private string _custOrdNo;

		public string CustOrdNo
		{
			get { return _custOrdNo; }
			set
			{
				_custOrdNo = value;
				RaisePropertyChanged("CustOrdNo");
			}
		}

		#endregion

		#region 來源單號

		private string _sourceNo;

		public string SourceNo
		{
			get { return _sourceNo; }
			set
			{
				_sourceNo = value;
				RaisePropertyChanged("SourceNo");
			}
		}

		#endregion

		#region 單據狀態

		private List<NameValuePair<string>> _statusList;
		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				_statusList = value;
				RaisePropertyChanged("StatusList");
			}
		}

		private string _selectedStatus;

		public string SelectedStatus
		{
			get { return _selectedStatus; }
			set
			{
				_selectedStatus = value;
				RaisePropertyChanged("SelectedStatus");
			}
		}

        #endregion

        #region 單據筆數

        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;
                RaisePropertyChanged("TotalCount");
            }
        }

        #endregion

        #region 總品項數

        private int _totalItemCount;
        public int TotalItemCount
        {
            get { return _totalItemCount; }
            set
            {
                _totalItemCount = value;
                RaisePropertyChanged("TotalItemCount");
            }
        }

        #endregion

        #region 明細筆數

        private int _totalDetailCount;
		public int TotalDetailCount
		{
			get { return _totalDetailCount; }
			set
			{
				_totalDetailCount = value;
				RaisePropertyChanged("TotalDetailCount");
			}
		}

		#endregion

		#region 新增/修改明細筆數

		private int _addOrModifyTotalDetailCount;
		public int AddOrModifyTotalDetailCount
		{
			get { return _addOrModifyTotalDetailCount; }
			set
			{
				_addOrModifyTotalDetailCount = value;
				RaisePropertyChanged("AddOrModifyTotalDetailCount");
			}
		}

		#endregion

		#region Grid資料繫結資料

		private List<F010201Data> _f010201Datas;
		public List<F010201Data> F010201Datas
		{
			get { return _f010201Datas; }
			set
			{
				_f010201Datas = value;
				RaisePropertyChanged("F010201Datas");
			}
		}

		private F010201Data _selectedF010201Data;

		public F010201Data SelectedF010201Data
		{
			get { return _selectedF010201Data; }
			set
			{
				_selectedF010201Data = value;
				RaisePropertyChanged("SelectedF010201Data");
				BindDetail();
			}
		}

		#endregion

		#region Grid 明細 繫結資料

		private ObservableCollection<F010202Data> _f010202Datas;

		public ObservableCollection<F010202Data> F010202Datas
		{
			get { return _f010202Datas; }
			set
			{
				_f010202Datas = value;
				RaisePropertyChanged("F010202Datas");
			}
		}

		private F010202Data _selectedF010202Data;

		public F010202Data SelectedF010202Data
		{
			get { return _selectedF010202Data; }
			set
			{
				_selectedF010202Data = value;
				RaisePropertyChanged("SelectedF010202Data");
			}
		}

		#endregion

		#region 新增/修改主明細資料

		private F010201Data _addOrModifyF010201Data;

		public F010201Data AddOrModifyF010201Data
		{
			get { return _addOrModifyF010201Data; }
			set
			{
				_addOrModifyF010201Data = value;
				RaisePropertyChanged("AddOrModifyF010201Data");
			}
		}

		#endregion

		#region 新增/修改 明細項目

		private F010202Data _addOrModifyF010202Data;

		public F010202Data AddOrModifyF010202Data
		{
			get { return _addOrModifyF010202Data; }
			set
			{
				_addOrModifyF010202Data = value;
				RaisePropertyChanged("AddOrModifyF010202Data");
			}
		}

		#endregion

		#region Grid 新增/修改 明細 繫結資料

		private List<F010202Data> _tempAddOrModifyF010202Datas;
		private List<F010202Data> _addOrModifyF010202Datas;

		public List<F010202Data> AddOrModifyF010202Datas
		{
			get { return _addOrModifyF010202Datas; }
			set
			{
				_addOrModifyF010202Datas = value;
				RaisePropertyChanged("AddOrModifyF010202Datas");
			}
		}

		private F010202Data _selectedAddOrModifyF010202Data;

		public F010202Data SelectedAddOrModifyF010202Data
		{
			get { return _selectedAddOrModifyF010202Data; }
			set
			{
				_selectedAddOrModifyF010202Data = value;
				RaisePropertyChanged("SelectedAddOrModifyF010202Data");
			}
		}

		private bool _hasFindSearchItem;
		public bool HasFindSearchItem
		{
			get { return _hasFindSearchItem; }
			set
			{
				_hasFindSearchItem = value;
				RaisePropertyChanged("HasFindSearchItem");
			}
		}

		#endregion

		#region 顯示/隱藏查詢

		private bool _isShowQuery = true;
		public bool IsShowQuery
		{
			get { return _isShowQuery; }
			set
			{
				_isShowQuery = value;
				RaisePropertyChanged("IsShowQuery");
			}
		}

		#endregion

		#region 顯示/隱藏查詢結果

		private bool _isShowQueryResult = true;
		public bool IsShowQueryResult
		{
			get { return _isShowQueryResult; }
			set
			{
				_isShowQueryResult = value;
				RaisePropertyChanged("IsShowQueryResult");
			}
		}

		#endregion

		#region 訂貨數

		private int? _stockQty;
		public int? StockQty
		{
			get { return _stockQty; }
			set
			{
				_stockQty = value;
				RaisePropertyChanged("StockQty");

			}
		}

        #endregion

        #region 批號

        /// <summary>
        /// 批號
        /// </summary>
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

        #endregion

        #region 作業類別
        private List<NameValuePair<string>> _ordPropList;

		public List<NameValuePair<string>> OrdPropList
		{
			get { return _ordPropList; }
			set
			{
				_ordPropList = value;
				RaisePropertyChanged("OrdPropList");
			}
		}
		#endregion

		#region 快速通關作業清單
		private List<NameValuePair<string>> _fastPassTypeList;
		public List<NameValuePair<string>> FastPassTypeList
		{
			get { return _fastPassTypeList; }
			set { Set(() => FastPassTypeList, ref _fastPassTypeList, value); }
		}
		#endregion

		#region 預定進倉時段清單
		private List<NameValuePair<string>> _bookingInPeriodList;
		public List<NameValuePair<string>> BookingInPeriodList
		{
			get { return _bookingInPeriodList; }
			set { Set(() => BookingInPeriodList, ref _bookingInPeriodList, value); }
		}
		#endregion


		private bool _shopNoIsReadOnly;

		public bool ShopNoIsReadOnly
		{
			get { return _shopNoIsReadOnly; }
			set
			{
				_shopNoIsReadOnly = value;
				RaisePropertyChanged("ShopNoIsReadOnly");
			}
		}


		#region 匯入檔案路徑參數

		private string _importFilePath;

		public string ImportFilePath
		{
			get { return _importFilePath; }
			set
			{
				_importFilePath = value;
				RaisePropertyChanged("ImportFilePath");
			}
		}
		#endregion

		#region 
		private string _showUnitTrans;

		public string ShowUnitTrans
		{
			get { return _showUnitTrans; }
			set
			{
				_showUnitTrans = value;
				RaisePropertyChanged("ShowUnitTrans");
			}
		}

		#endregion




		#region 棧板標籤資料
		private List<P010201PalletData> _palletDatas;

		public List<P010201PalletData> PalletDatas
		{
			get { return _palletDatas; }
			set
			{
				Set(() => PalletDatas, ref _palletDatas, value);
			}
		}
		#endregion


		#region
		private F910501 _selectedF910501;

		public F910501 SelectedF910501
		{
			get { return _selectedF910501; }
			set
			{
				Set(() => SelectedF910501, ref _selectedF910501, value);
			}
		}
		#endregion

		#region 強制結案下拉選單

		private List<NameValuePair<string>> _userClosedList;
		public List<NameValuePair<string>> UserClosedList
		{
			get { return _userClosedList; }
			set
			{
				_userClosedList = value;
				RaisePropertyChanged("UserClosedList");
			}
		}

		private string _selectedUserClosed;

		public string SelectedUserClosed
		{
			get { return _selectedUserClosed; }
			set
			{
				_selectedUserClosed = value;
				RaisePropertyChanged("SelectedUserClosed");
			}
		}

		#endregion

		public bool _allowWarehouseinClosed;
		public bool AllowWarehouseinClosed
		{
			get { return _allowWarehouseinClosed; }
			set
			{
				_allowWarehouseinClosed = value;
				RaisePropertyChanged("AllowWarehouseinClosed");
			}
		}

		#endregion

		#region 下拉選單資料繫結
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}

		private void SetStatusList()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "STATUS", true);
			SelectedStatus = StatusList.First().Value;
		}

		public void SetOrdpropList()
		{
			// 退貨單新增編輯模式可選作業類別,R1,2,3
			var proxy = GetProxy<F00Entities>();
			var query = from item in proxy.F000903s
						where item.ORD_PROP.StartsWith("A")
						select new NameValuePair<string>
						{
							Name = item.ORD_PROP_NAME,
							Value = item.ORD_PROP
						};
			OrdPropList = query.ToList();
		}

		/// <summary>
		/// 快速通關作業清單
		/// </summary>
		public void SetFastPassType()
		{
			FastPassTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "FAST_PASS_TYPE");
		}

		public void SetBoolkingInPeriod()
		{
			BookingInPeriodList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "BOOKING_IN_PERIOD");
		}

		private void SetUserClosedList()
		{
			UserClosedList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "USER_CLOSED", true);
			SelectedUserClosed = UserClosedList.First().Value;
		}

		#endregion

		void SetOperateMode(OperateMode operateMode)
		{
			UserOperateMode = operateMode;

			// *[編輯時] 有貨主單號就算EDI，有填貨主單號，就不能修改採購單號SHOP_NO
			ShopNoIsReadOnly = (operateMode == OperateMode.Edit && SelectedF010201Data != null && !string.IsNullOrEmpty(SelectedF010201Data.CUST_ORD_NO));
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, c => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			var proxyEx = GetExProxy<P01ExDataSource>();
			F010201Datas = proxyEx.CreateQuery<F010201Data>("GetF010201Datas")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("begStockDate", (BegStockDate.HasValue) ? BegStockDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : "")
				.AddQueryExOption("endStockDate", (EndStockDate.HasValue) ? EndStockDate.Value.AddHours(24).AddSeconds(-1).ToString("yyyy/MM/dd HH:mm:ss") : "")
				.AddQueryExOption("stockNo", StockNo)
				.AddQueryExOption("vnrCode", VnrCode)
				.AddQueryExOption("vnrName", VnrName)
				.AddQueryExOption("custOrdNo", CustOrdNo)
				.AddQueryExOption("sourceNo", SourceNo)
				.AddQueryExOption("status", SelectedStatus)
				.AddQueryExOption("userClosed", SelectedUserClosed).ToList();

      TotalCount = F010201Datas.Count();
			//總品項數計算
			var proxy = new wcf.P01WcfServiceClient();
			TotalItemCount = RunWcfMethod<int>(proxy.InnerChannel, () => proxy.GetInboundCnt(SelectedDcCode, GupCode, CustCode, F010201Datas.Select(x => x.STOCK_NO).ToArray()));
    }

		private void DoSearchComplete(bool isShowMsg = true)
		{
			IsShowQueryResult = true;
			if (F010201Datas != null && F010201Datas.Any())
			{
				if (SelectedF010201Data == null)
					SelectedF010201Data = F010201Datas.First();
				else
				{
					var item =
						F010201Datas.FirstOrDefault(
							o =>
								o.DC_CODE == SelectedF010201Data.DC_CODE && o.GUP_CODE == SelectedF010201Data.GUP_CODE &&
								o.CUST_CODE == SelectedF010201Data.CUST_CODE && o.STOCK_NO == SelectedF010201Data.STOCK_NO);
					SelectedF010201Data = null;
					SelectedF010201Data = item ?? F010201Datas.First();
				}
				IsShowQuery = false;
			}
			else
			{
				F010202Datas = null;
				SelectedF010201Data = null;
				TotalDetailCount = 0;
				if (isShowMsg)
					ShowMessage(Messages.InfoNoData);
			}
		}

		private void BindDetail()
		{
			if (SelectedF010201Data != null)
			{
				IsShowQueryResult = true;
				var proxyEx = GetExProxy<P01ExDataSource>();
				F010202Datas = proxyEx.CreateQuery<F010202Data>("GetF010202Datas")
					.AddQueryExOption("dcCode", SelectedF010201Data.DC_CODE)
					.AddQueryExOption("gupCode",SelectedF010201Data.GUP_CODE)
					.AddQueryExOption("custCode", SelectedF010201Data.CUST_CODE)
					.AddQueryExOption("stockNo", SelectedF010201Data.STOCK_NO).ToObservableCollection();

				if (F010202Datas.Any())
					SelectedF010202Data = F010202Datas.First();
				TotalDetailCount = F010202Datas.Count;
			}
		}

		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query,
					c => DoAddComplete()
					);
			}
		}

		private void DoAdd()
		{
			SetOperateMode(OperateMode.Add);
			AddOrModifyF010201Data = new F010201Data
			{
				GUP_CODE = GupCode,
				CUST_CODE = CustCode,
				STOCK_DATE = DateTime.Today,
				DELIVER_DATE = DateTime.Today
			};
			_tempAddOrModifyF010202Datas = new List<F010202Data>();
			AddOrModifyF010202Datas = _tempAddOrModifyF010202Datas;
			AddOrModifyF010202Data = new F010202Data
			{
				GUP_CODE = GupCode,
				CUST_CODE = CustCode,
				VALI_DATE = DateTime.MaxValue.Date
			};
			AddOrModifyTotalDetailCount = AddOrModifyF010202Datas.Count;
			//執行新增動作
		}

		private void DoAddComplete()
		{
			SetTxtVnrCodeFocusClick();
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedF010201Data != null && (SelectedF010201Data.STATUS == "0" || SelectedF010201Data.STATUS == "8"),
					c => DoEditComplete()
					);
			}
		}

		private void DoEdit()
		{
			SetOperateMode(OperateMode.Edit);
			StockQty = null;

			AddOrModifyF010201Data = Mapper.Map<F010201Data>(SelectedF010201Data);
			_tempAddOrModifyF010202Datas = Mapper.Map<List<F010202Data>>(F010202Datas);
			AddOrModifyF010202Datas = _tempAddOrModifyF010202Datas;
			if (AddOrModifyF010202Datas.Any())
				SelectedAddOrModifyF010202Data = AddOrModifyF010202Datas.First();
            AddOrModifyF010202Data = new F010202Data
            {
                GUP_CODE = GupCode,
                CUST_CODE = CustCode,
                VALI_DATE = DateTime.MaxValue.Date
			};
			AddOrModifyTotalDetailCount = AddOrModifyF010202Datas.Count;
			//執行編輯動作
		}

		private void DoEditComplete()
		{
			SetTxtCustOrdNoFocusClick();
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query,
					c => DoCancelComplete()
					);
			}
		}

		private void DoCancel()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes)
			{
				return;
			}
			//執行取消動作
			AddOrModifyF010201Data = null;
			_tempAddOrModifyF010202Datas = null;
			AddOrModifyF010202Datas = null;
			AddOrModifyF010202Data = null;
			AddOrModifyTotalDetailCount = 0;

			SetOperateMode(OperateMode.Query);
			SelectedStatus = "";
			DoSearch();
		}

		private void DoCancelComplete()
		{
			DoSearchComplete(false);
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedF010201Data != null && SelectedF010201Data.STATUS == "0"
					);
			}
		}

		private void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
			{
				return;
			}
			//執行刪除動作
			//只能狀態為0的才可刪除
			var proxyEx = GetExProxy<P01ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("DeleteP010201")
				.AddQueryExOption("stockNo", SelectedF010201Data.STOCK_NO)
				.AddQueryExOption("gupCode", SelectedF010201Data.GUP_CODE)
				.AddQueryExOption("custCode", SelectedF010201Data.CUST_CODE)
				.AddQueryExOption("dcCode", SelectedF010201Data.DC_CODE).FirstOrDefault();

			if (result != null && !result.IsSuccessed)
				ShowWarningMessage(result.Message);

			SearchCommand.Execute(null);
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode != OperateMode.Query && AddOrModifyF010202Datas != null && AddOrModifyF010202Datas.Any(),
					c => DoSaveComplete()
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			if (SetVnrInfo())
			{
				if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
				{
					return;
				}
				if (AddOrModifyTotalDetailCount >= 1)
				{
					var proxy = new wcf.P01WcfServiceClient();
					var f010201Data = ExDataMapper.Map<F010201Data, wcf.F010201Data>(AddOrModifyF010201Data);
					var f010202Datas = ExDataMapper.MapCollection<F010202Data, wcf.F010202Data>(_tempAddOrModifyF010202Datas).ToArray();
					var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
						() => proxy.InsertOrUpdateP010201(f010201Data, f010202Datas));

					if (!result.IsSuccessed)
						DialogService.ShowMessage(result.Message);
					else
					{
						ShowMessage(UserOperateMode == OperateMode.Add ? Messages.InfoAddSuccess : Messages.InfoUpdateSuccess);

						SetOperateMode(OperateMode.Query);
					}
				}
				else
				{
					ShowWarningMessage("進倉明細筆數需大於等於1");
				}


			}
		}

		private void DoSaveComplete()
		{
			if (UserOperateMode == OperateMode.Query)
			{
				SelectedStatus = "";

				if (string.IsNullOrEmpty(AddOrModifyF010201Data.STOCK_NO))
				{
					DoSearch();
					if (F010201Datas != null && F010201Datas.Count > 0)
						SelectedF010201Data = F010201Datas.Last();
				}
				else
				{
					SearchCommand.Execute(null);
				}
			}
		}
		#endregion Save

		#region ImportExcel
		public ICommand ImportCommand
		{
			get
			{
                return new RelayCommand(() =>
                     {
                         DispatcherAction(() =>
                         {
                             ExcelImport();
                             if (string.IsNullOrEmpty(ImportFilePath)) return;
                             DoImportCommand.Execute(null);
                         });
                     });
			}
		}

        public ICommand DoImportCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { Import(); },
                    () => UserOperateMode == OperateMode.Query
                    );
            }
        }

        private DialogResponse show20Detail(string tmpString2)
		{
			return ShowConfirmMessage(Properties.Resources.P010201_EarlyTodayInfo + Environment.NewLine + tmpString2);

		}

		private void Import()
		{
			string fullFilePath = ImportFilePath;
			var errorMeg = string.Empty;
            //var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);

            var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);
            if (excelTable == null)
			{
				DialogService.ShowMessage(Properties.Resources.P010201_ExcelIsOpen);
				return;
			}

			if (excelTable.Columns.Count < 10)
			{
				DialogService.ShowMessage(Properties.Resources.P010201_ImportColumnMessage);
				return;
			}
			if (excelTable.Columns[0].ColumnName != "PO_NO" ||
				excelTable.Columns[1].ColumnName != Properties.Resources.VnrCode ||
				excelTable.Columns[2].ColumnName != "廠商名稱" ||
				excelTable.Columns[3].ColumnName != "快速通關分類" ||
				excelTable.Columns[4].ColumnName != "預定進貨時段" ||
				excelTable.Columns[5].ColumnName != "商品編號" ||
				excelTable.Columns[6].ColumnName != "商品名稱" ||
				excelTable.Columns[7].ColumnName != "實際訂貨量" ||
				excelTable.Columns[8].ColumnName != "效期" ||
				excelTable.Columns[9].ColumnName != "批號")
            {
                DialogService.ShowMessage(Properties.Resources.P010201_ImportFormatError);
                return;
            }

			// 檢查快速通關分類需相同且為必填
			if (excelTable.AsEnumerable().Select(x => x[3]).Distinct().Count() > 1 || excelTable.AsEnumerable().Any(x => string.IsNullOrWhiteSpace(x[3].ToString())))
			{
				DialogService.ShowMessage("快速通關分類需相同且為必填");
				return;
			}

			// 檢查預定進貨時段需相同且為必填
			if (excelTable.AsEnumerable().Select(x => x[4]).Distinct().Count() > 1 || excelTable.AsEnumerable().Any(x => string.IsNullOrWhiteSpace(x[4].ToString())))
			{
				DialogService.ShowMessage("快速通關份類需相同且為必填");
				return;
			}

			string earlyTodayProduct = string.Empty;
			foreach (var i in excelTable.AsEnumerable())
			{
                if (string.IsNullOrEmpty(i[1].ToString().Trim()) || string.IsNullOrEmpty(i[0].ToString().Trim()) || string.IsNullOrEmpty(i[5].ToString().Trim()) || string.IsNullOrEmpty(i[7].ToString().Trim()))
                {
                    DialogService.ShowMessage(Properties.Resources.P010201_ImportFailNull);
                    return;
                }

                if (GetTryParseDate(i[6]) < DateTime.Today)
				{
					earlyTodayProduct += string.Format(Properties.Resources.P010201_EarlyTodayProduct, i[0], i[5]);
				}
				string msg = CheckValidQty(Convert.ToInt16(i[7]), Convert.ToString(i[5]));
				if (!string.IsNullOrEmpty(msg))
				{
					ShowWarningMessage(string.Format(Properties.Resources.P010201_CheckValidQtyMessage, i[0], i[5], msg));
					return;
				}

				// 檢查快速通關分類
				var fastPassType = FastPassTypeList.Where(x => x.Name == i[3].ToString().Trim()).FirstOrDefault();
				if (fastPassType == null)
				{
					DialogService.ShowMessage("快速通關分類須為一般、快速或急件");
					return;
				}
				else
				{
					i[3] = fastPassType.Value;
				}

				// 檢查預定進貨時段
				var bookingInPeriod = BookingInPeriodList.Where(x => x.Name == i[4].ToString().Trim()).FirstOrDefault();
				if (bookingInPeriod == null)
				{
					DialogService.ShowMessage("預定進貨時段須為上午或下午");
					return;
				}
				else
				{
					i[4] = bookingInPeriod.Value;
				}
			}

			if (!string.IsNullOrEmpty(earlyTodayProduct))
			{
				var tmpString = earlyTodayProduct.Split(';');
				string tmpString2 = string.Empty;
				if (tmpString.Count() > 20)
				{
					for (int i = 1; i <= tmpString.Count(); i++)
					{
						tmpString2 += i % 2 == 1 ? tmpString[i - 1] + " " : tmpString[i - 1] + Environment.NewLine;
						if (i % 20 == 0)
						{
							if (show20Detail(tmpString2) == DialogResponse.No)
							{
								tmpString2 = string.Empty;
								continue;
							}
						}
						else if (i == tmpString.Count()) { if (show20Detail(tmpString2) == DialogResponse.No) { return; } }
					}
				}
				else
				{
					for (int i = 1; i <= tmpString.Count(); i++)
					{
						tmpString2 += i % 2 == 1 ? tmpString[i - 1] + " " : tmpString[i - 1] + Environment.NewLine;
					}
					if (show20Detail(tmpString2) == DialogResponse.No) { return; }

				}
			}

			var queryData = (from col in excelTable.AsEnumerable()
							 select new F010201ImportData
							 {
								 PO_NO = Convert.ToString(col[0]),
								 VNR_CODE = Convert.ToString(col[1]),
								 VNR_NAME = Convert.ToString(col[2]),
								 FAST_PASS_TYPE = Convert.ToString(col[3]),
								 BOOKING_IN_PERIOD = Convert.ToString(col[4]),
								 ITEM_CODE = Convert.ToString(col[5]).Trim(),
								 ITEM_NAME = Convert.ToString(col[6]),
								 STOCK_QTY = Convert.ToInt16(col[7]),
								 VALI_DATE = GetTryParseDate(col[8]),
                                 MAKE_NO=Convert.ToString(col[9])
							 }).ToList();


			var importData = ExDataMapper.MapCollection<F010201ImportData, wcf.F010201ImportData>(queryData).ToArray();
			var proxy = new wcf.P01WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
													() => proxy.ImportF10201Data(SelectedDcCode, GupCode, CustCode
																				, fullFilePath, importData));

			DialogService.ShowMessage(result.Message.ToString());
		}

		DateTime? GetTryParseDate(object o)
		{
			DateTime date;
			if (!DateTime.TryParse(Convert.ToString(o), out date))
				return null;

			return date;
		}

		#region CSV TO DATABLE
		public static DataTable ConvertCSVtoDataTable(string strFilePath)
		{
			string errorMsg = string.Empty;
			try
			{
				DataTable dt = new DataTable();
				using (StreamReader sr = new StreamReader(strFilePath, Encoding.GetEncoding(950)))
				{
					string[] headers = sr.ReadLine().Split(',');
					foreach (string header in headers)
					{
						dt.Columns.Add(header);
					}
					while (!sr.EndOfStream)
					{
						string[] rows = sr.ReadLine().Split(',');
						DataRow dr = dt.NewRow();
						for (int i = 0; i < headers.Length; i++)
						{
							dr[i] = rows[i];
						}
						dt.Rows.Add(dr);

					}
				}
				return dt;
			}
			catch (Exception ex)
			{

				errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P010201_ImportFail, true);


				return null;
			}





		}
		#endregion


		#endregion ImportExcel

		#region 新增明細

		private bool _isAddSuccess;
		public ICommand AddDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddDetail(), () => UserOperateMode != OperateMode.Query,
					c => DoAddDetailComplete()
					);
			}
		}

		private void DoAddDetail()
		{
			_isAddSuccess = false;
			if (AddOrModifyF010202Data.VALI_DATE == null)
			{
				ShowWarningMessage(Properties.Resources.P010201_ValidDateIsNull);
				return;
			}

			if (HasFindSearchItem && StockQty.HasValue)
			{
				var proxyEx = GetExProxy<exshare.ShareExDataSource>();
                //包裝參考計算
				var volumeUnit = proxyEx.CreateQuery<exshare.ExecuteResult>("GetSingleVolumeUnit")
				.AddQueryExOption("gupCode", GupCode)
                .AddQueryExOption("custCode", CustCode)
                .AddQueryExOption("itemCode", AddOrModifyF010202Data.ITEM_CODE)
				.AddQueryExOption("stockQty", StockQty ?? 0).ToList().FirstOrDefault();

                var item = new F010202Data
                {
                    ChangeFlag = "A",
                    DC_CODE = AddOrModifyF010201Data.DC_CODE,
                    GUP_CODE = GupCode,
                    CUST_CODE = CustCode,
                    STOCK_NO = AddOrModifyF010201Data.STOCK_NO,
                    STOCK_QTY = StockQty ?? 0,
                    STOCK_SEQ = AddOrModifyF010202Datas.Any() ? AddOrModifyF010202Datas.Max(o => o.STOCK_SEQ) + 1 : 1,
                    ITEM_CODE = AddOrModifyF010202Data.ITEM_CODE,
                    ITEM_NAME = AddOrModifyF010202Data.ITEM_NAME,
                    ITEM_SPEC = AddOrModifyF010202Data.ITEM_SPEC,
                    ITEM_COLOR = AddOrModifyF010202Data.ITEM_COLOR,
                    ITEM_SIZE = AddOrModifyF010202Data.ITEM_SIZE,
                    VALI_DATE = AddOrModifyF010202Data.VALI_DATE,
                    MAKE_NO = MakeNo,
                    UNIT_TRANS = volumeUnit.Message
                };

				string msg = CheckValidQty(StockQty, AddOrModifyF010202Data.ITEM_CODE);
				if (_tempAddOrModifyF010202Datas.Where(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE
														&& o.CUST_CODE == item.CUST_CODE && o.ITEM_CODE == item.ITEM_CODE
														&& o.VALI_DATE == item.VALI_DATE)
														.Count() > 0)
				{
					DialogService.ShowMessage(Properties.Resources.P010201_ItemCodeRepeat);
				}
				else if (!string.IsNullOrEmpty(msg))
				{
					DialogService.ShowMessage(msg);

				}
				else
				{
					_tempAddOrModifyF010202Datas.Add(item);
					HasFindSearchItem = false;
					_isAddSuccess = true;
				}

				StockQty = null;
                MakeNo = null;
                AddOrModifyF010202Data = new F010202Data
                {
                    GUP_CODE = GupCode,
                    CUST_CODE = CustCode,
                    VALI_DATE = DateTime.MaxValue.Date
				};


			}
			else
			{
				var message = !HasFindSearchItem ? Properties.Resources.P010201_ItemCodeIsNull : Properties.Resources.P010201_OrderQtyIsNull;
				ShowWarningMessage(message);
			}
		}

		private void DoAddDetailComplete()
		{
			if (_isAddSuccess)
			{
				AddOrModifyF010202Datas = _tempAddOrModifyF010202Datas.Where(o => o.ChangeFlag != "D").ToList();
				if (AddOrModifyF010202Datas.Any())
					SelectedAddOrModifyF010202Data = AddOrModifyF010202Datas.First();

				AddOrModifyTotalDetailCount = AddOrModifyF010202Datas.Count;
			}
		}

		#endregion

		#region 刪除明細
		public ICommand DeleteDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDeleteDetail(), () => UserOperateMode != OperateMode.Query && AddOrModifyF010202Datas != null && AddOrModifyF010202Datas.Any() && SelectedAddOrModifyF010202Data != null,
					c => DoDeleteDetailComplete()
					);
			}
		}

		private void DoDeleteDetail()
		{
			var item = _tempAddOrModifyF010202Datas.FirstOrDefault(
				o =>
					o.DC_CODE == AddOrModifyF010201Data.DC_CODE && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode &&
					o.STOCK_NO == AddOrModifyF010201Data.STOCK_NO && o.ITEM_CODE == SelectedAddOrModifyF010202Data.ITEM_CODE &&
					o.STOCK_SEQ == SelectedAddOrModifyF010202Data.STOCK_SEQ && o.ChangeFlag != "D");

			if (item.ChangeFlag == "A")
				_tempAddOrModifyF010202Datas.Remove(item);
			else
				item.ChangeFlag = "D";
		}

		private void DoDeleteDetailComplete()
		{
			AddOrModifyF010202Datas = _tempAddOrModifyF010202Datas.Where(o => o.ChangeFlag != "D").ToList();
			if (AddOrModifyF010202Datas.Any())
				SelectedAddOrModifyF010202Data = AddOrModifyF010202Datas.First();
			AddOrModifyTotalDetailCount = AddOrModifyF010202Datas.Count;
		}


		#endregion

		#region 設定廠商資料

		public bool SetVnrInfo()
		{
			bool isFind = false;
			var proxy = GetProxy<F19Entities>();
			var item =
				proxy.F1908s.Where(
					o => o.GUP_CODE == GupCode && o.VNR_CODE == AddOrModifyF010201Data.VNR_CODE && o.STATUS != "9")
					.ToList();
			var f1909 = proxy.F1909s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode).FirstOrDefault();
			if (f1909.ALLOWGUP_VNRSHARE == "0")
				item = item.Where(o => o.CUST_CODE == CustCode).ToList();
			if (item != null && item.Any())
			{
				AddOrModifyF010201Data.VNR_NAME = item.FirstOrDefault().VNR_NAME;
				AddOrModifyF010201Data.VNR_ADDRESS = item.FirstOrDefault().ADDRESS;
				isFind = true;
			}
			else
			{
				AddOrModifyF010201Data.VNR_NAME = "";
				AddOrModifyF010201Data.VNR_ADDRESS = "";
				ShowWarningMessage(Properties.Resources.P010201_VendorNoNotExist);
			}
			return isFind;
		}

		private string _checkedVnrCode = null;

		public void CheckVnrCode()
		{
			if (!string.IsNullOrEmpty(AddOrModifyF010201Data.VNR_CODE) && AddOrModifyF010201Data.VNR_CODE != _checkedVnrCode)
			{
				_checkedVnrCode = AddOrModifyF010201Data.VNR_CODE;
				SetVnrInfo();
			}
		}
		#endregion

		#region 檢查商品訂購數
		public string CheckValidQty(int? qty, string itemCode)
		{
			var proxy = GetProxy<F19Entities>();
			//貨主商品主檔
			var f1903 = proxy.F1903s.Where(a => a.ITEM_CODE == itemCode && a.GUP_CODE == GupCode && a.CUST_CODE == CustCode);
			int? venOrd = 0; //供應商最低訂量
			int? retOrd = 0; //訂購倍數
			long ordSaveQty = 0; //採購安全庫存量 
			int stockQty = 0; //目前F1913中的庫存量
			if (f1903 != null && f1903.ToList().Any())
			{
				venOrd = f1903.FirstOrDefault().VEN_ORD == null ? 0 : f1903.FirstOrDefault().VEN_ORD;
				retOrd = f1903.FirstOrDefault().RET_ORD == null ? 0 : f1903.FirstOrDefault().RET_ORD;
				ordSaveQty = f1903.FirstOrDefault().ORD_SAVE_QTY;
				stockQty = GetF1913WithF1912Qty(itemCode, "F1903");
			}

			if (qty <= 0)
			{
				return Properties.Resources.P010201_OrderQtyIsZero;
			}

			if (venOrd > 0)
			{
				if (venOrd > qty)
				{
					return Properties.Resources.P010201_OrderQtyLessThanVentorQty;
				}
			}
			if (retOrd > 0)
			{
				//1.訂購數 需要大於 供應商最低訂量 *訂購倍數
				//2.訂購數 需要等於 供應商最低訂量的倍數
				//3.訂購數 除以供應商最低訂量 不可小於訂購倍數
				if (retOrd * venOrd > qty || qty % venOrd != 0 || qty / venOrd < retOrd)
				{
					return Properties.Resources.P010201_OrderQtyError;
				}
			}
			if (ordSaveQty > 0)
			{
				if (stockQty + qty < ordSaveQty)
				{
					return string.Format(Properties.Resources.P010201_OrderQtyOverStockQty, ordSaveQty);
				}
			}
			return string.Empty;
		}
		private int GetF1913WithF1912Qty(string itemCode, string dataTable)
		{
			var porxyEx = GetExProxy<P01ExDataSource>();
			//string dcCode, string gupCode, string custCode, string itemCode, string dataTable
			var f1913withf1912Qtys = porxyEx.CreateQuery<F1913WithF1912Qty>("GetF1913WithF1912Qty")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("itemCode", itemCode)
				.AddQueryExOption("dataTable", dataTable)
				.ToList();
			if (f1913withf1912Qtys != null && f1913withf1912Qtys.Any())
			{
				return Convert.ToInt32(f1913withf1912Qtys.FirstOrDefault().QTY);
			}
			else
			{
				return 0;
			}
		}
		#endregion


		#region PrintPallet 列印棧板標籤
		/// <summary>
		/// 列印棧板標籤
		/// </summary>
		public ICommand PrintPalletCommand
		{
			get
			{
				var isOk = false;
				return CreateBusyAsyncCommand(
						o => isOk = DoPrintPallet(), () => UserOperateMode == OperateMode.Query && SelectedF010201Data != null,
						o =>
						{
							if (isOk)
								DoPrintPalletReport();
						}
);
			}
		}

		public bool DoPrintPallet()
		{
			var proxy = GetProxy<F01Entities>();
            var datas = proxy.F010203s.Where(x => x.DC_CODE == SelectedF010201Data.DC_CODE && x.GUP_CODE == SelectedF010201Data.GUP_CODE && x.CUST_CODE == SelectedF010201Data.CUST_CODE && x.STOCK_NO == SelectedF010201Data.STOCK_NO && x.STICKER_TYPE == "1").ToList();
            bool isCountPallet = true;
			var isOkCount = true;
			if (datas.Any())
			{
				var msg = new MessagesStruct
				{
					Button = DialogButton.YesNoCancel,
					Image = DialogImage.Question,
					Message =  Properties.Resources.P010201_ConfirmPalletStickerExistToDo.Replace("\\n",Environment.NewLine),
					Title = Properties.Resources.Ask
				};
				var dialogResponse = DialogService.ShowMessage(msg.Message, msg.Title, msg.Button, msg.Image, MessageBoxResult.Yes);
				if (dialogResponse == DialogResponse.Cancel)
					return false;
				if (dialogResponse == DialogResponse.Yes)
					isCountPallet = false;
			}
			if(isCountPallet)
			{
				var proxyWcf = new wcf.P01WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
					() => proxyWcf.CountPallet(SelectedF010201Data.DC_CODE, SelectedF010201Data.GUP_CODE, SelectedF010201Data.CUST_CODE, SelectedF010201Data.STOCK_NO));
				if (!result.IsSuccessed)
					ShowWarningMessage(result.Message);
				else if (!string.IsNullOrWhiteSpace(result.Message))
					ShowWarningMessage(result.Message);
				isOkCount = result.IsSuccessed;
			}
			if(isOkCount)
			{
				var proxyEx = GetExProxy<P01ExDataSource>();
				var data = proxyEx.CreateQuery<P010201PalletData>("GetP010201PalletDatas")
					.AddQueryExOption("dcCode", SelectedF010201Data.DC_CODE)
					.AddQueryExOption("gupCode", SelectedF010201Data.GUP_CODE)
					.AddQueryExOption("custCode", SelectedF010201Data.CUST_CODE)
					.AddQueryExOption("stockNo", SelectedF010201Data.STOCK_NO).ToList();

				foreach (var item in data)
				{
					item.STICKER_BARCODE = BarcodeConverter128.StringToBarcode(item.STICKER_NO);
				}
				PalletDatas = data;
				return true;
			}
			return false;

		}
		#endregion PrintPallet

		#region UserClose 進倉單強制結案
		/// <summary>
		/// 列印棧板標籤
		/// </summary>
		public ICommand UserCloseCommand
		{
			get
			{
				var isOK = false;
				return CreateBusyAsyncCommand(
						o => isOK = DoUserClose(),
						() => SelectedF010201Data != null && SelectedF010201Data.CUST_COST == "In" && SelectedF010201Data.STATUS == "1",
						o => { if (isOK) DoUserCloseComplete(); }
					);
			}
		}

		public bool DoUserClose()
		{
			if (string.IsNullOrWhiteSpace(SelectedF010201Data.USER_CLOSED_MEMO))
			{
				ShowWarningMessage("強制結案需要填寫備註原因才可執行");
				return false;
			}
			var proxyWcf = new wcf.P01WcfServiceClient();

			var param = new UserCloseStockParam()
			{
				DC_CODE = SelectedF010201Data.DC_CODE,
				GUP_CODE = SelectedF010201Data.GUP_CODE,
				CUST_CODE = SelectedF010201Data.CUST_CODE,
				STOCK_NO = SelectedF010201Data.STOCK_NO,
				IS_USER_CLOSED = "0",
				USER_CLOSED_MEMO = SelectedF010201Data.USER_CLOSED_MEMO,
			};
			var result = RunWcfMethod<wcf.UserCloseExecuteResult>(proxyWcf.InnerChannel,
				() => proxyWcf.UserCloseStock(ExDataMapper.Map<UserCloseStockParam, wcf.UserCloseStockParam>(param)));

			if (!result.IsSuccessed && result.NeedConfirm)
			{
				if (ShowConfirmMessage(result.Message) == DialogResponse.Yes)
				{
					param.IS_USER_CLOSED = "1";
					result = RunWcfMethod<wcf.UserCloseExecuteResult>(proxyWcf.InnerChannel,
						() => proxyWcf.UserCloseStock(ExDataMapper.Map<UserCloseStockParam, wcf.UserCloseStockParam>(param)));
				}
				else
				{
					return false;
				}
			}
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return false;
			}
			return true;
		}

		public void DoUserCloseComplete()
		{
			SearchCommand.Execute(null);
		}
		#endregion PrintPallet
	}
}
