using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using F151001Data = Wms3pl.WpfClient.ExDataServices.P15ExDataService.GetF150201CSV;
using System.Reflection;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P15WcfService;
using Wms3pl.WpfClient.Services;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows;

namespace Wms3pl.WpfClient.P15.ViewModel
{
    public partial class P1502010000_ViewModel : InputViewModelBase
	{
    public Action ExcelImport = delegate { };
    public Action IsExpend = delegate { };
    public Action FinishedOffShelfLackProcess = delegate { };
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

    #region pa
    private string _SelectedDcCode;

		public string SelectedDcCode
		{
			get { return _SelectedDcCode; }
			set
			{
				_SelectedDcCode = value;
				RaisePropertyChanged("SelectedDcCode");
			}
		}
		#endregion

		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		public Action<PrintType> DoPrintReport = delegate { };
		public Action<PrintType> DoPrintStickerReport = delegate { };
		public Action DoPost = delegate { };
		public Action DoSendCar = delegate { };
		public bool IsFirstAddorUpdate;
    public string GupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
    public string CustCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
    private string InventoryLossWHId { get; set; }

    private F151001 _addOrUpdateF151001;
    public F151001 AddOrUpdateF151001
    {
      get { return _addOrUpdateF151001; }
			set
			{
				_addOrUpdateF151001 = value;
				RaisePropertyChanged("AddOrUpdateF151001");
			}
		}

		#region Form - 查詢狀態
		private bool _isSearch = true;
		public bool IsSearch
		{
			get { return _isSearch; }
			set
			{
				_isSearch = value;
				RaisePropertyChanged("IsSearch");
			}
		}
		#endregion
		#region Form - 選取狀態
		private bool _isSelectedData = false;
		public bool IsSelectedData
		{
			get { return _isSelectedData; }
			set
			{
				_isSelectedData = value;
				RaisePropertyChanged("IsSelectedData");
			}
		}
		#endregion
		#region Form - 過帳狀態
		private bool _isPosting = false;
		public bool IsPosting
		{
			get { return _isPosting; }
			set
			{
				_isPosting = value;
				RaisePropertyChanged("IsPosting");
			}
		}
		#endregion
		#region 匯出csv

		private bool _showQueryResult;
		public bool ShowQueryResult
		{
			get { return _showQueryResult; }
			set
			{
				if (_showQueryResult == value) return;
				Set(() => ShowQueryResult, ref _showQueryResult, value);
			}
		}

		private DataTable _data;
		public DataTable Data
		{
			get { return _data; }
			set
			{
				_data = value;
				ShowQueryResult = _data != null;
				RaisePropertyChanged("Data");
				//Set(() => Data, ref _data, value);
			}
		}
		#endregion
		#region Grid 查詢結果

		private List<F151001Data> _f151001Datas;

		public List<F151001Data> F151001Datas
		{
			get { return _f151001Datas; }
			set
			{
				_f151001Datas = value;
				RaisePropertyChanged();
			}
		}

		private F151001Data _selectedF151001Data;

		public F151001Data SelectedF151001Data
		{
			get { return _selectedF151001Data; }
			set
			{

				_selectedF151001Data = value;

				RaisePropertyChanged("SelectedF151001Data");
				// BindDetailData();

			}
		}

		private F151001Data _shopData;
		public F151001Data ShopData
		{
			get { return _shopData; }
			set { _shopData = value; RaisePropertyChanged(); }
		}

		#endregion


		#region Form - 查詢
		#region Form - 物流中心
		private List<NameValuePair<string>> _dcCodes;

		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				RaisePropertyChanged("DcCodes");
			}
		}

		private string _sourceDcCode;
		public string SourceDcCode
		{
			get { return _sourceDcCode; }
			set
			{
				_sourceDcCode = value;
				RaisePropertyChanged("SourceDcCode");
				DgList = new List<P1502010000Data>();
				SourceWarehouseList = GetUserWarehouse(value, true);
				if (SourceWarehouseList.Any())
					SelectSourceWarehouse = SourceWarehouseList.First().Value;
			}
		}

		private string _targetDcCode;
		public string TargetDcCode
		{
			get { return _targetDcCode; }
			set
			{
				_targetDcCode = value;
				RaisePropertyChanged("TargetDcCode");
				DgList = new List<P1502010000Data>();
				TargetWarehouseList = GetUserWarehouse(value, true);
				if (TargetWarehouseList.Any())
					SelectTargetWarehouse = TargetWarehouseList.First().Value;
			}
		}

		#endregion
		#region Form - 建立日期起迄
		private DateTime _cRTDateS = DateTime.Today;
		public DateTime CRTDateS
		{
			get { return _cRTDateS; }
			set { _cRTDateS = value; RaisePropertyChanged("CRTDateS"); }
		}
		private DateTime _cRTDateE = DateTime.Today;
		public DateTime CRTDateE
		{
			get { return _cRTDateE; }
			set { _cRTDateE = value; RaisePropertyChanged("CRTDateE"); }
		}
		#endregion
		#region Form - 調撥單號
		private string _txtSearchAllocationNo;
		public string TxtSearchAllocationNo
		{
			get { return _txtSearchAllocationNo; }
			set
			{
				_txtSearchAllocationNo = value;
				RaisePropertyChanged("TxtSearchAllocationNo");
			}
		}
        #endregion
        #region Form - 容器條碼
        private string _txtSearchContainerCode;
        public string TxtSearchContainerCode
        {
            get { return _txtSearchContainerCode; }
            set
            {
                _txtSearchContainerCode = value;
                RaisePropertyChanged("TxtSearchContainerCode");
            }
        }
        #endregion
        #region Form - 單據狀態
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

		private string _selectSTATUS;
		public string SelectSTATUS
		{
			get { return _selectSTATUS; }
			set
			{
				_selectSTATUS = value;
				RaisePropertyChanged("SelectSTATUS");
			}
		}
		#endregion
		#region Form - 過帳日期
		private DateTime? _postingDateS;
		public DateTime? PostingDateS
		{
			get { return _postingDateS; }
			set
			{
				_postingDateS = value;
				RaisePropertyChanged("PostingDateS");
			}
		}

		private DateTime? _postingDateE;
		public DateTime? PostingDateE
		{
			get { return _postingDateE; }
			set
			{
				_postingDateE = value;
				RaisePropertyChanged("PostingDateE");
			}
		}
        #endregion
        #region Form - 單據狀態
        private List<NameValuePair<string>> _allocationTypeList;

        public List<NameValuePair<string>> AllocationTypeList
        {
            get { return _allocationTypeList; }
            set
            {
                _allocationTypeList = value;
                RaisePropertyChanged("AllocationTypeList");
            }
        }

        private string _selectAllocationType;
        public string SelectAllocationType
        {
            get { return _selectAllocationType; }
            set
            {
                _selectAllocationType = value;
                RaisePropertyChanged("SelectAllocationType");
            }
        }
        #endregion
        #region Form - 來源倉別
        private List<NameValuePair<string>> _sourceWarehouseList;
		public List<NameValuePair<string>> SourceWarehouseList
		{
			get { return _sourceWarehouseList; }
			set { _sourceWarehouseList = value; RaisePropertyChanged("SourceWarehouseList"); }
		}
		private string _selectSourceWarehouse;
		public string SelectSourceWarehouse
		{
			get { return _selectSourceWarehouse; }
			set { _selectSourceWarehouse = value; RaisePropertyChanged("SelectSourceWarehouse"); }
		}
		#endregion
		#region Form - 目的倉別
		private List<NameValuePair<string>> _targetWarehouseList;
		public List<NameValuePair<string>> TargetWarehouseList
		{
			get { return _targetWarehouseList; }
			set { _targetWarehouseList = value; RaisePropertyChanged("TargetWarehouseList"); }
		}
		private string _selectTargetWarehouse;
		public string SelectTargetWarehouse
		{
			get { return _selectTargetWarehouse; }
			set { _selectTargetWarehouse = value; RaisePropertyChanged("SelectTargetWarehouse"); }
		}
		#endregion
		#region Form - 來源單號
		private string _txtSearchSourceNo;
		public string TxtSearchSourceNo
		{
			get { return _txtSearchSourceNo; }
			set
			{
				_txtSearchSourceNo = value;
				RaisePropertyChanged("TxtSearchSourceNo");
			}
		}
		#endregion
		#region 作業人員
		private string _txtOperator;
		public string TxtOperator
		{
			get { return _txtOperator; }
			set{Set(()=> TxtOperator,ref _txtOperator,value);}
		}
		#endregion
		#region Data - 資料List
		private List<P1502010000Data> _dgList;
		public List<P1502010000Data> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private P1502010000Data _selectedData;

		public P1502010000Data SelectedData
		{
			get { return _selectedData; }
			set
			{
				//if (value != null)
				//{
				//	if (value.SRC_DC_CODE != SelectedData.SRC_DC_CODE ||
				//	 value.SRC_WAREHOUSE_ID != SelectedData.SRC_WAREHOUSE_ID ||
				//	 value.TAR_DC_CODE != SelectedData.TAR_DC_CODE ||
				//	 value.TAR_WAREHOUSE_ID != SelectedData.TAR_WAREHOUSE_ID)
				//	{
				//		if (!CheckChange(SelectedData, value))
				//			return;
				//	}
				//	if (!string.IsNullOrWhiteSpace(value.SOURCE_NO))
				//	{
				//		string checkstr = CheckSourceType(value.SOURCE_NO);
				//		if (!string.IsNullOrEmpty(checkstr))
				//		{
				//			MessagesStruct alertmsg = new MessagesStruct()
				//			{
				//				Message = checkstr,
				//				Button = DialogButton.OK,
				//				Image = DialogImage.Warning,
				//				Title = Resources.Resources.Information
				//			};
				//			ShowMessage(alertmsg);
				//		}
				//	}
				//}
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
				GetDetailData("01");
				IsSearch = _selectedData == null;
				IsSelectedData = _selectedData != null;
				IsExpend();
				// 若目的倉別為為空，不綁定SourceWarehouseList(若綁定會顯示全部)
				if (_selectedData != null)
					ShowSourceWarehouse = string.IsNullOrWhiteSpace(_selectedData.SRC_WAREHOUSE_ID) ? string.Empty : SourceWarehouseList.Where(x => x.Value == _selectedData.SRC_WAREHOUSE_ID).FirstOrDefault()?.Name;
				else
					ShowSourceWarehouse = string.Empty;

			}
		}

		// 來源倉別顯示文字
		public string _showSourceWarehouse;
		public string ShowSourceWarehouse
		{
			get { return _showSourceWarehouse; }
			set { Set(() => ShowSourceWarehouse, ref _showSourceWarehouse, value); }
		}
		#endregion
		#region Data - 明細筆數
		private int? _selectDetailCount;
		public int? SelectDetailCount
		{
			get { return _selectDetailCount; }
			set
			{
				_selectDetailCount = value;
				RaisePropertyChanged("SelectDetailCount");
			}
		}
		#endregion
		#region Data - 調撥單明細List
		private List<F151001DetailDatas> _dgDetailList;
		public List<F151001DetailDatas> DgDetailList
		{
			get { return _dgDetailList; }
			set
			{
				_dgDetailList = value;
				RaisePropertyChanged("DgDetailList");
			}
		}

		private F151001DetailDatas _selectedDetailData;

		public F151001DetailDatas SelectedDetailData
		{
			get { return _selectedDetailData; }
			set
			{
				_selectedDetailData = value;
				RaisePropertyChanged("SelectedDetailData");
			}
		}

		private bool _isEditSelectedAll = false;
		public bool IsEditSelectedAll
		{
			get { return _isEditSelectedAll; }
			set { _isEditSelectedAll = value; RaisePropertyChanged("IsEditSelectedAll"); }
		}
		#endregion
		#endregion

		#region Form - 新增修改
		#region Data - 調撥單明細List
		private List<SelectionItem<F151001DetailDatas>> _dgItemList;
		public List<SelectionItem<F151001DetailDatas>> DgItemList
		{
			get { return _dgItemList; }
			set
			{
				_dgItemList = value;
				if (_dgItemList != null)
					SelectDetailCount = _dgItemList.Count;
				RaisePropertyChanged("DgItemList");
			}
		}

		private SelectionItem<F151001DetailDatas> _selectedDgItem;
		public SelectionItem<F151001DetailDatas> SelectedDgItem
		{
			get { return _selectedDgItem; }
			set
			{
				_selectedDgItem = value;
				RaisePropertyChanged("SelectedDgItem");
			}
		}

		private List<F151001DetailDatas> _addItemList;
		public List<F151001DetailDatas> AddItemList
		{
			get { return _addItemList; }
			set
			{
				_addItemList = value;
				RaisePropertyChanged("AddItemList");
			}
		}
		#endregion
		#region Form - 新增/更新 來源倉別

		private List<NameValuePair<string>> _addOrUpdateSourceWarehouseList;
		public List<NameValuePair<string>> AddOrUpdateSourceWarehouseList
		{
			get { return _addOrUpdateSourceWarehouseList; }
			set { _addOrUpdateSourceWarehouseList = value; RaisePropertyChanged("AddOrUpdateSourceWarehouseList"); }
		}
		#endregion
		#region Form -新增/更新 目的倉別
		private List<NameValuePair<string>> _addOrUpdateTargetWarehouseList;
		public List<NameValuePair<string>> AddOrUpdateTargetWarehouseList
		{
			get { return _addOrUpdateTargetWarehouseList; }
			set { _addOrUpdateTargetWarehouseList = value; RaisePropertyChanged("AddOrUpdateTargetWarehouseList"); }
		}

		#endregion


		#region 是否可修改來源DC 來源倉
		private bool _canEditSrc;

		public bool CanEditSrc
		{
			get { return _canEditSrc; }
			set
			{
				if (_canEditSrc == value)
					return;
				Set(() => CanEditSrc, ref _canEditSrc, value);
			}
		}
		#endregion


		#region
		private bool _canEditTar;

		public bool CanEditTar
		{
			get { return _canEditTar; }
			set
			{
				if (_canEditTar == value)
					return;
				Set(() => CanEditTar, ref _canEditTar, value);
			}
		}
		#endregion



		#region 是否可修改來源儲位
		private bool _readOnlySrcLocCode;

		public bool ReadOnlySrcLocCode
		{
			get { return _readOnlySrcLocCode; }
			set
			{
				if (_readOnlySrcLocCode == value)
					return;
				Set(() => ReadOnlySrcLocCode, ref _readOnlySrcLocCode, value);
			}
		}
		#endregion

		#region 是否可修改下架數
		private bool _readOnlySrcQty;

		public bool ReadOnlySrcQty
		{
			get { return _readOnlySrcQty; }
			set
			{
				if (_readOnlySrcQty == value)
					return;
				Set(() => ReadOnlySrcQty, ref _readOnlySrcQty, value);
			}
		}
		#endregion


		#region 是否可修改上架儲位
		private bool _readOnlyTarLocCode;

		public bool ReadOnlyTarLocCode
		{
			get { return _readOnlyTarLocCode; }
			set
			{
				if (_readOnlyTarLocCode == value)
					return;
				Set(() => ReadOnlyTarLocCode, ref _readOnlyTarLocCode, value);
			}
		}
		#endregion


		#region 是否可修改上架數
		private bool _readOnlyTarQty;

		public bool ReadOnlyTarQty
		{
			get { return _readOnlyTarQty; }
			set
			{
				if (_readOnlyTarQty == value)
					return;
				Set(() => ReadOnlyTarQty, ref _readOnlyTarQty, value);
			}
		}
		#endregion



		#endregion

		#region Data - 列印
		#region Data - 列印資料表
		private List<F151001ReportData> _f151001ReportData;
		public List<F151001ReportData> F151001ReportData
		{
			get { return _f151001ReportData; }
			set
			{
				_f151001ReportData = value;
				RaisePropertyChanged("PrintF161601Data");
			}
		}


		public DataTable F151001ReportDataDT
		{
			get
			{
				_f151001ReportData.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));
				_f151001ReportData.ForEach(x => x.ItemBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE));

				return _f151001ReportData.ToDataTable();
			}
		}
		#endregion
		#region Data - 列印資料表By展開效期入庫日
		private List<F151001ReportDataByExpendDate> _f151001ReportDataByExpendDate;
		public List<F151001ReportDataByExpendDate> F151001ReportDataByExpendDate
		{
			get { return _f151001ReportDataByExpendDate; }
			set
			{
				_f151001ReportDataByExpendDate = value;
				RaisePropertyChanged("PrintF161601Data");
			}
		}


		public DataTable F151001ReportDataDTByExpendDate
		{
			get
			{
				_f151001ReportDataByExpendDate.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));
				_f151001ReportDataByExpendDate.ForEach(x => x.ItemBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE));

				return _f151001ReportDataByExpendDate.ToDataTable();
			}
		}
		#endregion
		#region Data - 列印調撥單貼紙
		private List<wcf.F151001ReportDataByTicket> _f151001ReportDataTicket;
		public List<wcf.F151001ReportDataByTicket> F151001ReportDataByTicket
		{
			get { return _f151001ReportDataTicket; }
			set
			{
				_f151001ReportDataTicket = value;
				RaisePropertyChanged("F151001ReportDataByTicket");
			}
		}


		public DataTable F151001ReportDataDTByTicket
		{
			get
			{
				_f151001ReportDataTicket.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));

				return _f151001ReportDataTicket.ToDataTable();
			}
		}
    #endregion
    #endregion

    #region Data - 是否有輸入缺貨
    public Boolean IsInputLack;
    #endregion Data - 是否有輸入缺貨

    #region Data - 是否已過帳
    public Boolean IsPosted;
    #endregion Data - 是否已過帳

    #endregion

    #region 函式
    public P1502010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}

		}

		private void InitControls()
		{

			_userId = Wms3plSession.Get<UserInfo>().Account;
			_userName = Wms3plSession.Get<UserInfo>().AccountName;
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			GetSourceTypeName();
			if (DcCodes.Any())
			{
				SourceDcCode = DcCodes.First().Value;
				TargetDcCode = DcCodes.First().Value;
			}
			StatusList = GetStatusList();
            if (StatusList.Any())
				SelectSTATUS = StatusList.First().Value;

            AllocationTypeList = GetAllocationTypeList();
            if (AllocationTypeList.Any())
                SelectAllocationType = AllocationTypeList.First().Value;

            if (SourceWarehouseList.Any())
				SelectSourceWarehouse = SourceWarehouseList.First().Value;
			if (TargetWarehouseList.Any())
				SelectTargetWarehouse = TargetWarehouseList.First().Value;

            GetInventoryLossWHId();
        }

    private void GetInventoryLossWHId()
    {
      var proxy = GetProxy<F00Entities>();
      var f0003 = proxy.F0003s.Where(o => o.AP_NAME == "InventoryLossWHId" && o.DC_CODE == TargetDcCode && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode).FirstOrDefault();
      InventoryLossWHId = f0003 == null ? string.Empty : f0003.SYS_PATH;
    }

    public List<NameValuePair<string>> GetStatusList()
		{
			var proxy = GetProxy<F00Entities>();
			var data = proxy.F000904s.Where(o => o.TOPIC == "F151001" && o.SUBTOPIC == "STATUS").ToList();
			var results = (from o in data
						   select new NameValuePair<string>
						   {
							   Name = o.NAME,
							   Value = o.VALUE
						   }).ToList();
			results.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			return results;
		}

        public List<NameValuePair<string>> GetAllocationTypeList()
        {
            var proxy = GetProxy<F00Entities>();
            var data = proxy.F000904s.Where(o => o.TOPIC == "F151001" && o.SUBTOPIC == "ALLOCATION_TYPE").ToList();
            var results = (from o in data
                           select new NameValuePair<string>
                           {
                               Name = o.NAME,
                               Value = o.VALUE
                           }).ToList();
            results.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
            return results;
        }

        public List<NameValuePair<string>> GetUserWarehouse(string dcCode, bool hasAll)
		{
			var result = new List<NameValuePair<string>>();
			var qry = GetProxy<F19Entities>().F1980s;
			
			result = (from o in qry
					  where o.DC_CODE == dcCode
					  select new NameValuePair<string>
					  {
						  Name = string.Format("{0} {1}", o.WAREHOUSE_ID, o.WAREHOUSE_NAME),
						  Value = o.WAREHOUSE_ID
					  }).ToList();
			if (hasAll)
				result.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			
			return result;
		}

		private List<NameValuePair<string>> _sourceTypeList;

		public List<NameValuePair<string>> SourceTypeList
		{
			get { return _sourceTypeList; }
			set
			{
				if (_sourceTypeList == value)
					return;
				Set(() => SourceTypeList, ref _sourceTypeList, value);
			}
		}

		/// <summary>
		/// 來源單據Value,來源單據名稱Name
		/// </summary>
		/// <returns></returns>
		public void GetSourceTypeName()
		{
			var proxy = GetProxy<F00Entities>();
			SourceTypeList = (from a in proxy.F000902s
							  select new NameValuePair<string>
							  {
								  Value = a.SOURCE_TYPE,
								  Name = a.SOURCE_NAME
							  }).ToList();
		}

		public string GetWarehouseHouseName(string dcCode, string warehouseId)
		{
			var proxy = GetProxy<F19Entities>();
			var qry = proxy.F1980s.Where(x => x.DC_CODE.Equals(dcCode) && x.WAREHOUSE_ID.Equals(warehouseId)).FirstOrDefault();
			return qry == null ? null : qry.WAREHOUSE_NAME;
		}

		public List<string> GetItemData(string gupCode, string itemCode)
		{
			var proxy = GetProxy<F19Entities>();
			var qry = proxy.F1903s.Where(x => x.GUP_CODE.Equals(gupCode) && x.ITEM_CODE.Equals(itemCode) && x.CUST_CODE == CustCode).FirstOrDefault();
			var items = new List<string>(4);
			if (qry != null)
			{
				items.Add(qry.ITEM_CODE);
				items.Add(qry.ITEM_NAME);
				items.Add(qry.ITEM_COLOR);
				items.Add(qry.ITEM_SIZE);
				items.Add(qry.ITEM_SPEC);
				items.Add(qry.CUST_ITEM_CODE);
				items.Add(qry.EAN_CODE1);
			}
			return items;
		}

		private void GetDetailData(string action)
		{
			if (SelectedData != null)
			{
				DgDetailList = new List<F151001DetailDatas>();
				DgItemList = new List<SelectionItem<F151001DetailDatas>>();
				var proxyEx = GetExProxy<P15ExDataSource>();
				var detailqry = proxyEx.CreateQuery<F151001DetailDatas>("GetF151001DetailDatas")
															 .AddQueryExOption("dcCode", SelectedData.DC_CODE)
															 .AddQueryExOption("gupCode", SelectedData.GUP_CODE)
															 .AddQueryExOption("custCode", SelectedData.CUST_CODE)
															 .AddQueryExOption("allocationNo", SelectedData.ALLOCATION_NO)
															 .AddQueryExOption("isExpendDate", SelectedData.ISEXPENDDATE)
                               .AddQueryExOption("action", action)
                               .ToList();
				if (detailqry.Any())
				{
					DgDetailList = detailqry.OrderBy(x => x.ROWNUM).ToList();
					DgItemList = ExDataMapper.MapCollection<F151001DetailDatas, F151001DetailDatas>(detailqry).OrderBy(x => x.ROWNUM).ToSelectionList().ToList();
					SelectDetailCount = DgDetailList.Count;
				}
				else
				{
          ShowMessage(Messages.InfoNoData);
          DgDetailList = new List<F151001DetailDatas>();
					DgItemList = new List<SelectionItem<F151001DetailDatas>>();
					SelectDetailCount = 0;
				}
			}
		}

		public void ReturnDetailData()
		{
			if (AddItemList != null)
			{
				DgItemList = AddItemList.ToSelectionList().ToList();
				if (DgItemList != null && DgItemList.Any())
				{
					SelectDetailCount = DgItemList.Count;
					DgItemList = DgItemList.OrderBy(x => x.Item.ROWNUM).ToList();
				}
			}
		}


		public bool CheckChange(F151001 oldData, F151001 newData)
		{
			if (newData == new F151001())
				return true;
			if (oldData == new F151001())
				return true;
			if (newData.SRC_DC_CODE != oldData.SRC_DC_CODE && !string.IsNullOrEmpty(oldData.SRC_DC_CODE))
			{
				if (DgItemList != null && DgItemList.Any())
				{
					MessagesStruct message = new MessagesStruct();
					message.Message = Properties.Resources.P1502010000_ConfirmToDelAllDetailDataByModifyDC;
					message.Button = DialogButton.YesNo;
					message.Image = DialogImage.Warning;
					message.Title = Resources.Resources.Warning;
					if (ShowMessage(message) != DialogResponse.Yes)
						return false;
					else
					{
						DgItemList = new List<SelectionItem<F151001DetailDatas>>();
						return true;
					}
				}
			}
			if (newData.SRC_WAREHOUSE_ID != oldData.SRC_WAREHOUSE_ID && !string.IsNullOrEmpty(oldData.SRC_WAREHOUSE_ID))
			{
				if (DgItemList.Any())
				{
					MessagesStruct message = new MessagesStruct();
					message.Message = Properties.Resources.P1502010000_ConfirmToDelAllDetailDataByModifyWarehouseId;
					message.Button = DialogButton.YesNo;
					message.Image = DialogImage.Warning;
					message.Title = Resources.Resources.Warning;
					if (ShowMessage(message) != DialogResponse.Yes)
						return false;
					else
					{
						DgItemList = new List<SelectionItem<F151001DetailDatas>>();
						return true;
					}
				}
			}
			if (newData.TAR_DC_CODE != oldData.TAR_DC_CODE && !string.IsNullOrEmpty(oldData.TAR_DC_CODE))
			{
				if (DgItemList.Any())
				{
					MessagesStruct message = new MessagesStruct();
					message.Message = Properties.Resources.P1502010000_ConfirmToReturnTAR_QTYByModifyDC;
					message.Button = DialogButton.YesNo;
					message.Image = DialogImage.Warning;
					message.Title = Resources.Resources.Warning;
					if (ShowMessage(message) != DialogResponse.Yes)
						return false;
					else
					{
						var tmpDg = DgItemList.ToList();
						foreach (var item in tmpDg)
						{
							item.Item.TAR_QTY = item.Item.SRC_QTY;
						}
						DgItemList = tmpDg.ToList();
						return true;
					}
				}
			}
			if (newData.TAR_WAREHOUSE_ID != oldData.TAR_WAREHOUSE_ID && !string.IsNullOrEmpty(oldData.TAR_WAREHOUSE_ID))
			{
				MessagesStruct message = new MessagesStruct();
				message.Message = Properties.Resources.P1502010000_ConfirmToReturnTAR_QTYByModifyWarehouseId;
				message.Button = DialogButton.YesNo;
				message.Image = DialogImage.Warning;
				message.Title = Resources.Resources.Warning;
				if (ShowMessage(message) != DialogResponse.Yes)
					return false;
				else
				{
					var tmpDg = DgItemList.ToList();
					foreach (var item in tmpDg)
					{
						item.Item.TAR_QTY = item.Item.SRC_QTY;
					}
					DgItemList = tmpDg.ToList();
					return true;
				}
			}
			return true;
		}

		public List<F151001DetailDatas> GetCheckSearch()
		{
			var result = new List<F151001DetailDatas>();
			if (DgItemList != null)
			{
				result = (from i in DgItemList
						  where i.IsSelected
						  select i.Item).ToList();
			}
			return result;
		}

		public static DataTable ListToDataTable(List<F151001ReportData> list)
		{
			var dt = new DataTable();
			foreach (PropertyInfo info in typeof(F151001ReportData).GetProperties())
				dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
			foreach (F151001ReportData t in list)
			{
				DataRow row = dt.NewRow();
				foreach (PropertyInfo info in typeof(F151001ReportData).GetProperties())
				{
					if (!info.CanWrite)
						continue;
					row[info.Name] = info.GetValue(t, null);
				}
				dt.Rows.Add(row);
			}
			dt.TableName = "ReportData";
			return dt;
		}

		public string CheckItemQty(F151001 f151001, List<SelectionItem<F151001DetailDatas>> itemList)
		{
			if (itemList == null || !itemList.Any())
				return Properties.Resources.P1502010000_EmptyitemList;


			var proxyEx = GetExProxy<P15ExDataSource>();
			var f19Proxy = GetProxy<F19Entities>();
			var downStatus = new List<string> { "0","1","2" };
			var upStatus = new List<string> { "3", "4" };
			foreach (var item in itemList)
			{
				if (downStatus.Contains(f151001.STATUS) &&  !string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID))
				{
					if (string.IsNullOrWhiteSpace(item.Item.SRC_LOC_CODE))
						return Properties.Resources.P1502010000_EmptySRC_LOC_CODE;
					var nowStatusId = GetNowStatusId(f19Proxy, item.Item.DC_CODE, item.Item.SRC_LOC_CODE);
					if (string.IsNullOrEmpty(nowStatusId))
					{
						return string.Format(Properties.Resources.P1502010000_SRC_LOC_CODENotFound, item.Item.SRC_LOC_CODE);
					}

					if (nowStatusId == "03" || nowStatusId == "04")
					{
						return string.Format(Properties.Resources.P1502010000_LocCodeExportStatusCanNotTransfer, item.Item.SRC_LOC_CODE);
					}
				}
				if (upStatus.Contains(f151001.STATUS) && !string.IsNullOrWhiteSpace(f151001.TAR_WAREHOUSE_ID))
				{
					if (string.IsNullOrWhiteSpace(item.Item.TAR_LOC_CODE))
						return Properties.Resources.P1502010000_PleaseTextTarLocCode;
					var nowStatusId = GetNowStatusId(f19Proxy, item.Item.TAR_DC_CODE, item.Item.TAR_LOC_CODE);
					if (string.IsNullOrEmpty(nowStatusId))
					{
						return string.Format(Properties.Resources.P1502010000_TARSRC_LOC_CODENotFound, item.Item.TAR_LOC_CODE);
					}

					if (nowStatusId == "02" || nowStatusId == "04")
					{
						return string.Format(Properties.Resources.P1502010000_LocCodeImportStatusCanNotTransfer, item.Item.TAR_LOC_CODE);
					}
				}
				if (downStatus.Contains(f151001.STATUS) && !string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID))
				{
					var f151002ItemQty = 0;
					var list = proxyEx.CreateQuery<F1913WithF1912Moved>("GetF1913WithF1912Moveds")
						.AddQueryExOption("dcCode", item.Item.DC_CODE)
						.AddQueryExOption("gupCode", item.Item.GUP_CODE)
						.AddQueryExOption("custCode", item.Item.CUST_CODE)
						.AddQueryExOption("srcLocCodeS", item.Item.SRC_LOC_CODE)
						.AddQueryExOption("srcLocCodeE", item.Item.SRC_LOC_CODE)
						.AddQueryExOption("itemCode", item.Item.ITEM_CODE)
						.AddQueryExOption("itemName", "")
						.AddQueryExOption("srcWarehouseId", item.Item.SRC_WAREHOUSE_ID)
					   .AddQueryExOption("isExpendDate", AddOrUpdateF151001.ISEXPENDDATE)
						 .AddQueryExOption("makeNoList", item.Item.MAKE_NO)
						.ToList();
					if (AddOrUpdateF151001.ISEXPENDDATE == "1")
						list = list.Where(x => x.ENTER_DATE == item.Item.ENTER_DATE && x.VALID_DATE == item.Item.VALID_DATE && x.MAKE_NO == item.Item.MAKE_NO).ToList();
					if (!list.Any() && string.IsNullOrEmpty(item.Item.ALLOCATION_NO))
					{
						return Properties.Resources.P1502010000_ItemNotFound + item.Item.ITEM_NAME + Properties.Resources.P1502010000_InventoryData;
					}

					//編輯 要把原本F151002 ItemCode 數量加回來. 判斷才不會出錯
					if (!string.IsNullOrEmpty(item.Item.ALLOCATION_NO))
					{
						var f151002Datas = proxyEx.CreateQuery<F151002ItemData>("GetF151002ItemQty")
							.AddQueryExOption("dcCode", item.Item.DC_CODE)
							.AddQueryExOption("gupCode", item.Item.GUP_CODE)
							.AddQueryExOption("custCode", item.Item.CUST_CODE)
							.AddQueryExOption("allocationNo", item.Item.ALLOCATION_NO)
							.AddQueryExOption("itemCode", item.Item.ITEM_CODE)
							.AddQueryExOption("locCodeS", item.Item.SRC_LOC_CODE)
							 .AddQueryExOption("isExpendDate", AddOrUpdateF151001.ISEXPENDDATE).ToList();
						if (AddOrUpdateF151001.ISEXPENDDATE == "1")
						{
							var f151002Data = f151002Datas.FirstOrDefault(x => x.VALID_DATE == item.Item.VALID_DATE && x.ENTER_DATE == item.Item.ENTER_DATE);
							if (f151002Data != null)
								f151002ItemQty = f151002Data.SRC_QTY;
						}
						else
						{
							var f151002Data = f151002Datas.FirstOrDefault();
							if (f151002Data != null)
								f151002ItemQty = f151002Data.SRC_QTY;
						}
					}

					if ((list.Sum(x => x.QTY) + f151002ItemQty < item.Item.SRC_QTY))
					{
						return string.Format("商品「{0}」、效期：{1}、入庫日：{2}，數量不足{3}",
											 item.Item.ITEM_CODE,
											 Convert.ToDateTime(item.Item.VALID_DATE).ToString("yyyy-MM-dd"),
											 Convert.ToDateTime(item.Item.ENTER_DATE).ToString("yyyy-MM-dd"),
											 item.Item.SRC_QTY - (list.Sum(x => x.QTY) + f151002ItemQty));
						//return Properties.Resources.P1502010000_Item + item.Item.ITEM_NAME + Properties.Resources.P1502010000_ItemQtyInsufficient;
					}
				}
			}

			return string.Empty;
		}

		private static string GetNowStatusId(F19Entities f19Proxy, string dcCode, string locCode)
		{
			return f19Proxy.F1912s.Where(x => x.LOC_CODE == locCode && x.DC_CODE == dcCode).Select(x => x.NOW_STATUS_ID).FirstOrDefault();
		}

		public string CheckSourceType(string sourceNo)
		{
			string alertMessage = "";
			if (!string.IsNullOrEmpty(sourceNo))
			{
				switch (sourceNo.Trim().Substring(0, 1))
				{
					case "A":
						SelectedData.SOURCE_TYPE = "04";
						break;
					case "S":
						SelectedData.SOURCE_TYPE = "01";
						break;
					case "R":
						SelectedData.SOURCE_TYPE = "02";
						break;
					case "W":
						SelectedData.SOURCE_TYPE = "10";
						break;
					case "T":
						SelectedData.SOURCE_TYPE = "12";
						break;
					default:
						SelectedData.SOURCE_TYPE = "";
						alertMessage = Properties.Resources.P1502010000_SOURCE_TYPEIsEmpty;
						break;
				}
			}
			return alertMessage;
		}

		#endregion

		#region Command
		#region CheckAll
		public ICommand CheckEditAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCheckEditAllItem()
				);
			}
		}

		public void DoCheckEditAllItem()
		{
			if (DgItemList != null)
				foreach (var p in DgItemList)
					p.IsSelected = IsEditSelectedAll;
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearch(), () => UserOperateMode == OperateMode.Query
						);
			}
		}

		public void DoSearch()
		{
			//調撥單查詢
			if (CheckSearchData())
			{
				var proxyEx = GetExProxy<P15ExDataSource>();
				var data = proxyEx.CreateQuery<P1502010000Data>("GetAllocationData")
					.AddQueryExOption("srcDcCode", SourceDcCode)
					.AddQueryExOption("tarDcCode", TargetDcCode)
					.AddQueryExOption("gupCode", GupCode)
					.AddQueryExOption("custCode", CustCode)
					.AddQueryExOption("crtDateS", CRTDateS)
					.AddQueryExOption("crtDateE", CRTDateE)
					.AddQueryExOption("postingDateS", PostingDateS)
					.AddQueryExOption("postingDateE", PostingDateE)
					.AddQueryExOption("allocationNo", TxtSearchAllocationNo)
					.AddQueryExOption("status", SelectSTATUS)
					.AddQueryExOption("sourceNo", TxtSearchSourceNo)
					.AddQueryExOption("userName", TxtOperator)
                    .AddQueryExOption("containerCode", TxtSearchContainerCode)
                    .AddQueryExOption("allocationType", SelectAllocationType).ToList();

				//var data = qry.ToList();
				var warehouseIdListBySource = SourceWarehouseList.Select(o => o.Value).ToList();
				var warehouseIdListByTar = TargetWarehouseList.Select(o => o.Value).ToList();
				data = string.IsNullOrEmpty(SelectSourceWarehouse) ? data.Where(o => warehouseIdListBySource.Contains(o.SRC_WAREHOUSE_ID) || string.IsNullOrEmpty(o.SRC_WAREHOUSE_ID)).ToList() : data.Where(o => o.SRC_WAREHOUSE_ID == SelectSourceWarehouse).ToList();
				data = string.IsNullOrEmpty(SelectTargetWarehouse) ? data.Where(o => warehouseIdListByTar.Contains(o.TAR_WAREHOUSE_ID) || string.IsNullOrEmpty(o.TAR_WAREHOUSE_ID)).ToList() : data.Where(o => o.TAR_WAREHOUSE_ID == SelectTargetWarehouse).ToList();
				DgList = data.OrderBy(o => o.ALLOCATION_NO).ToList();
				if (!DgList.Any())
				{
					ShowMessage(Messages.InfoNoData);
					SelectedData = null;
				}
			}

			// 原調調撥單查詢
			//var message = string.Empty;
			//if (CRTDateS > CRTDateE)
			//	message = Properties.Resources.P1502010000_CRTDateIllegal;
			//else if (PostingDateS.HasValue && PostingDateE.HasValue && PostingDateS > PostingDateE)
			//	message = Properties.Resources.P1502010000_PostingDateIllegal;
			//if ((!PostingDateS.HasValue && PostingDateE.HasValue) || (PostingDateS.HasValue && !PostingDateE.HasValue))
			//	message = Properties.Resources.P1502010000_CRTDateEmpty;
			//if (!string.IsNullOrEmpty(message))
			//{
			//	ShowWarningMessage(message);
			//	return;
			//}
			////執行查詢動
			//var proxy = GetProxy<F15Entities>();
			//var crtEndDate = CRTDateE.AddDays(1).AddSeconds(-1);
			//var qry = proxy.F151001s.Where(x => x.SRC_DC_CODE == SourceDcCode &&
			//									x.TAR_DC_CODE == TargetDcCode &&
			//									x.GUP_CODE == GupCode &&
			//									x.CUST_CODE == CustCode &&
			//									x.CRT_ALLOCATION_DATE >= CRTDateS &&
			//																		x.CRT_ALLOCATION_DATE <= crtEndDate);

			//var tmpData = qry.ToList();

			//if (PostingDateS.HasValue && PostingDateE.HasValue)
			//{
			//	var postEdate = PostingDateE.Value.AddDays(1).AddSeconds(-1);
			//	qry = qry.Where(o => o.POSTING_DATE >= PostingDateS && o.POSTING_DATE <= postEdate);
			//}
			//if (!string.IsNullOrWhiteSpace(TxtSearchAllocationNo))
			//	qry = qry.Where(o => o.ALLOCATION_NO == TxtSearchAllocationNo);
			//qry = string.IsNullOrEmpty(SelectSTATUS) ? qry.Where(o => o.STATUS != "9") : qry.Where(o => o.STATUS == SelectSTATUS);
			//if (!string.IsNullOrWhiteSpace(TxtSearchSourceNo))
			//	qry = qry.Where(o => o.SOURCE_NO == TxtSearchSourceNo);
			//if(!string.IsNullOrWhiteSpace(TxtOperator))
			//	qry = qry.Where(o => o.CRT_NAME == TxtOperator || o.UPD_NAME == TxtOperator);
			
		}

		private bool CheckSearchData()
		{
			var message = string.Empty;
			if (CRTDateS > CRTDateE)
			{
				message +=  string.IsNullOrWhiteSpace(message) ? Properties.Resources.P1502010000_CRTDateIllegal : Environment.NewLine+ Properties.Resources.P1502010000_CRTDateIllegal;
			}

			if (PostingDateS.HasValue && PostingDateE.HasValue && PostingDateS > PostingDateE)
			{
				message += string.IsNullOrWhiteSpace(message)? Properties.Resources.P1502010000_PostingDateIllegal: Environment.NewLine + Properties.Resources.P1502010000_PostingDateIllegal;
			}

			if ((!PostingDateS.HasValue && PostingDateE.HasValue) || (PostingDateS.HasValue && !PostingDateE.HasValue))
			{
				message += string.IsNullOrWhiteSpace(message) ? Properties.Resources.P1502010000_CRTDateEmpty: Environment.NewLine + Properties.Resources.P1502010000_CRTDateEmpty;
			}

			if (!string.IsNullOrEmpty(message))
			{
				ShowWarningMessage(message);
				return false;
			}
			return true;


		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAdd(), () => UserOperateMode == OperateMode.Query
					);

			}
		}

		public void FormatData(bool isFirst = true)
		{
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			DgItemList = new List<SelectionItem<F151001DetailDatas>>();
			IsFirstAddorUpdate = true;
			//執行新增動作
			AddOrUpdateF151001 = new F151001
			{
				DC_CODE = DcCodes.First().Value,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				CRT_STAFF = _userId,
				CRT_NAME = _userName,
				CRT_DATE = DateTime.Now,
				SRC_DC_CODE = DcCodes.First().Value,
				TAR_DC_CODE = DcCodes.First().Value,
				ALLOCATION_DATE = DateTime.Now.Date,
				CRT_ALLOCATION_DATE = DateTime.Now.Date,
				STATUS = "0",
				LOCK_STATUS = "0",
				SEND_CAR = "0",
				ISEXPENDDATE = "1"
			};

			if (!isFirst)
			{
				AddOrUpdateF151001.SRC_WAREHOUSE_ID = AddOrUpdateSourceWarehouseList.First().Value;
				AddOrUpdateF151001.TAR_WAREHOUSE_ID = AddOrUpdateTargetWarehouseList.First().Value;
			}

			DgItemList = new List<SelectionItem<F151001DetailDatas>>();
			CanEditSrc = true;
			CanEditTar = true;
			ReadOnlySrcLocCode = false;
			ReadOnlySrcQty = false;
			ReadOnlyTarLocCode = false;
			ReadOnlyTarQty = true;
		}

		private void DoAdd()
		{
			FormatData();
			UserOperateMode = OperateMode.Add;
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && SelectedData != null
								&& (SelectedData?.STATUS == "0" || SelectedData?.STATUS == "1" || SelectedData?.STATUS == "8" || SelectedData?.STATUS == "3"||
								(SelectedData?.STATUS == "4" && CheckAutomaticWarehouse(SelectedData.TAR_DC_CODE,SelectedData.TAR_WAREHOUSE_ID)))
                                && (string.IsNullOrWhiteSpace(InventoryLossWHId) ? true : SelectedData.TAR_WAREHOUSE_ID != InventoryLossWHId),	
					c => DoEditComplete());
			}
		}

		private void DoEdit()
		{
			IsFirstAddorUpdate = true;
			UserOperateMode = OperateMode.Edit;
			AddOrUpdateF151001 = ExDataMapper.Map<P1502010000Data, F151001>(SelectedData);
			GetDetailData("02");

		}

		private void DoEditComplete()
		{
			CanEditSrc = true;
			CanEditTar = true;
			ReadOnlySrcLocCode = false;
			ReadOnlySrcQty = false;
			ReadOnlyTarLocCode = false;
			//上架數固定不能改
			ReadOnlyTarQty = true;

			// 有來源單據 都不能改來源倉庫、來源儲位與來源數量
			if(!string.IsNullOrWhiteSpace(AddOrUpdateF151001.SOURCE_NO))
			{
				CanEditSrc = false;
				ReadOnlySrcLocCode = true;
				ReadOnlySrcQty = true;

				// 純下架單且不是異常 不可更改 上架倉庫與上架儲位
				if (string.IsNullOrEmpty(AddOrUpdateF151001.TAR_WAREHOUSE_ID) && AddOrUpdateF151001.STATUS !="8")
				{
					CanEditTar = false;
					ReadOnlyTarLocCode = true;
				}
				// 純上架單且不是異常 不可更改上架倉庫 但可以改上架儲位
				if (string.IsNullOrEmpty(AddOrUpdateF151001.SRC_WAREHOUSE_ID) && AddOrUpdateF151001.STATUS != "8")
				{
					CanEditTar = false;
				}
			}
			else
			{
				// 純下架單且不是異常 不可更改 來源倉庫、來源儲位與來源數量、上架倉庫與上架儲位
				if (string.IsNullOrEmpty(AddOrUpdateF151001.TAR_WAREHOUSE_ID) && AddOrUpdateF151001.STATUS != "8")
				{
					CanEditSrc = false;
					ReadOnlySrcLocCode = true;
					ReadOnlySrcQty = true;
					CanEditTar = false;
					ReadOnlyTarLocCode = true;
				}
				// 純上架單 不可更改來源倉庫、來源儲位與來源數量、上架倉庫 但可以改上架倉別&儲位
				if (string.IsNullOrEmpty(AddOrUpdateF151001.SRC_WAREHOUSE_ID))
				{
					CanEditSrc = false;
					ReadOnlySrcLocCode = true;
					ReadOnlySrcQty = true;
					CanEditTar = new[] { "1", "6" }.Contains(AddOrUpdateF151001.ALLOCATION_TYPE) ? true : false;
				}

        // 調撥單狀態為 已下架處理或上架處裡中
        if (AddOrUpdateF151001.STATUS == "3" || AddOrUpdateF151001.STATUS == "4")
				{
					CanEditSrc = false;
					ReadOnlySrcLocCode = true;
					ReadOnlySrcQty = true;
				}
			}
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
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				//執行取消動作
				UserOperateMode = OperateMode.Query;
				AddOrUpdateF151001 = null;
			}
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
                // 當調撥單的來源倉別為自動倉且無來源單號且單據狀態=0(待處理) OR 2(下架處理中)，刪除按鈕變更也要可以按
                return CreateBusyAsyncCommand(
						o => DoDelete(),
						() => UserOperateMode == OperateMode.Query && DgList != null && DgList.Any() && SelectedData != null && string.IsNullOrWhiteSpace(SelectedData.SOURCE_NO) && 
                        ((SelectedData.STATUS == "0" || SelectedData.STATUS == "1" || SelectedData.STATUS == "8") ||
                        (SelectedData.SRC_WH_DEVICE_TYPE != "0" && (SelectedData.STATUS == "0" || SelectedData.STATUS == "2"))));
            }
        }

		private void DoDelete()
		{
			//執行刪除動作
			if (SelectedData != null)
			{
				if (SelectedData.STATUS == "0" || SelectedData.STATUS == "1" || SelectedData.STATUS == "8" 
                    || (string.IsNullOrWhiteSpace(SelectedData.SOURCE_NO) && SelectedData.SRC_WH_DEVICE_TYPE != "0" && (SelectedData.STATUS == "0" || SelectedData.STATUS == "2")))
				{
					// 確認是否要刪除
					if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
					// 先判斷存不存在該項目, 不存在就回傳該資料已糟刪除
					var proxy = GetModifyQueryProxy<F15Entities>();
					var f151001 = proxy.F151001s.Where(x => x.CUST_CODE == SelectedData.CUST_CODE
														&& x.GUP_CODE == SelectedData.GUP_CODE
														&& x.DC_CODE == SelectedData.DC_CODE
														&& x.ALLOCATION_NO == SelectedData.ALLOCATION_NO)
							.ToList().FirstOrDefault();

					if (f151001 == null || f151001.STATUS == "9")
						ShowMessage(Messages.WarningBeenDeleted);
					else if (f151001.STATUS != "0" && f151001.STATUS != "1" && f151001.STATUS != "8" &&
                        !(string.IsNullOrWhiteSpace(SelectedData.SOURCE_NO) && SelectedData.SRC_WH_DEVICE_TYPE != "0" && (SelectedData.STATUS == "0" || SelectedData.STATUS == "2")))
					{
						ShowWarningMessage(Properties.Resources.P1502010000_TranferStatusCannotDelete);
						DoSearch();
					}
					else
					{
                        var proxyWcf = new wcf.P15WcfServiceClient();
                        var wcfReq = ExDataMapper.Map<P1502010000Data, wcf.P1502010000Data>(SelectedData);
                        var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.DeleteF151001Datas(wcfReq));
                        if (result.IsSuccessed)
						{
							ShowMessage(Messages.InfoDeleteSuccess);
							TxtSearchAllocationNo = "";
							TxtSearchSourceNo = "";
							DoSearch();
						}
						else
							ShowMessage(new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = string.IsNullOrWhiteSpace(result.Message) ? Properties.Resources.P1502010000_DeleteFail : result.Message, Title = Resources.Resources.Information });
					}
				}
			}
		}
		#endregion Delete

		#region AddDetail
		public ICommand AddDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAddDetail(), () => UserOperateMode != OperateMode.Query && AddOrUpdateF151001 != null && (AddOrUpdateF151001.STATUS == "0" || AddOrUpdateF151001.STATUS == "1" || AddOrUpdateF151001.STATUS == "8") && string.IsNullOrEmpty(AddOrUpdateF151001.SOURCE_NO)
						);
			}
		}

		private void DoAddDetail()
		{
			//執行刪除動作
		}


		#endregion DeleteDetail

		#region DeleteDetail
		public ICommand DeleteDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDeleteDetail(), () => UserOperateMode != OperateMode.Query && GetCheckSearch().Any() && AddOrUpdateF151001 != null && (AddOrUpdateF151001.STATUS == "0" || AddOrUpdateF151001.STATUS == "1" || AddOrUpdateF151001.STATUS == "8") && string.IsNullOrEmpty(AddOrUpdateF151001.SOURCE_NO)
																										, o => DelDetailComplate()
						);
			}
		}

		private void DoDeleteDetail()
		{
			//執行刪除動作
		}

		private void DelDetailComplate()
		{
			if (UserOperateMode != OperateMode.Query)
			{
				var delItemList = GetCheckSearch();
				if (delItemList != null && delItemList.Any())
				{
					// 確認是否要刪除
					if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
					//執行刪除動作
					var tmpAddItemList = DgItemList.Where(x => x.IsSelected == false).AsQueryable().ToList();
					int tmpRowNum = 0;
					if (tmpAddItemList.Any())
					{
						foreach (var tmpitem in tmpAddItemList)
						{
							tmpRowNum += 1;
							tmpitem.Item.ROWNUM = tmpRowNum;
						}
					}
					DgItemList = tmpAddItemList.ToList();
				}
			}
		}
		#endregion DeleteDetail

		#region ExportSerial
		public ICommand ExportSerialCommand
		{
			get
			{
				return new RelayCommand(
						DoExportSerial,
						() => !IsBusy && UserOperateMode == OperateMode.Query
				);
			}
		}

		private void DoExportSerial()
		{
			var saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = ".xlsx";
			saveFileDialog.Filter = "Excel (.xlsx)|*.xlsx";
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.OverwritePrompt = true;
			saveFileDialog.Title = Properties.Resources.P1502010000_ExportSerialData;
			bool? isShowOk;

			var exportResult = GetExportSerialData();
			if (!exportResult.Any())
				return;

			isShowOk = saveFileDialog.ShowDialog();
			if (isShowOk ?? false)
			{
				var excelExportService = new ExcelExportService();
				excelExportService.CreateNewSheet(Properties.Resources.P1502010000_DetailSerialData + SelectedData.ALLOCATION_NO);

				var data = exportResult.ToDataTable();

				var showColumnName = new List<string>
					{
						"ITEM_CODE",
						"SERIAL_NO",
						"TAR_LOC_CODE"
					};
				var delColumnList = (from DataColumn column in data.Columns where !showColumnName.Contains(column.ColumnName) select column.ColumnName).ToList();
				foreach (var columnName in delColumnList)
					data.Columns.Remove(columnName);

				data.Columns["ITEM_CODE"].SetOrdinal(0);
				data.Columns["ITEM_CODE"].ColumnName = Properties.Resources.ITEM_CODE;
				data.Columns["SERIAL_NO"].SetOrdinal(1);
				data.Columns["SERIAL_NO"].ColumnName = Properties.Resources.SERIAL_NO;
				data.Columns["TAR_LOC_CODE"].SetOrdinal(2);
				data.Columns["TAR_LOC_CODE"].ColumnName = Properties.Resources.LOC_CODE;

				var excelExportSource = new ExcelExportReportSource
				{
					Data = data
				};

				excelExportService.AddExportReportSource(excelExportSource);
				bool isExportSuccess = excelExportService.Export(Path.GetDirectoryName(saveFileDialog.FileName),
					Path.GetFileName(saveFileDialog.FileName));
				var message = new MessagesStruct
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Message = "",
					Title = Resources.Resources.Information
				};
				message.Message = isExportSuccess ? Properties.Resources.P1502010000_ExportSerialDataSuccess : Properties.Resources.P1502010000_ExportSerialDataFail;
				if (!isExportSuccess)
					message.Image = DialogImage.Warning;
				ShowMessage(message);
			}
		}

		private List<P150201ExportSerial> GetExportSerialData()
		{
			//執行查詢動
			var proxyEx = GetExProxy<P15ExDataSource>();
			var results = proxyEx.CreateQuery<P150201ExportSerial>("GetExportSerial")
				.AddQueryExOption("dcCode", SelectedData.DC_CODE)
				.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
				.AddQueryExOption("custCode", SelectedData.CUST_CODE)
				.AddQueryExOption("allocationNo", SelectedData.ALLOCATION_NO)
				.ToList();
			if (!results.Any())
				ShowMessage(Messages.InfoNoData);

			return results;
		}

		#endregion

		#region Save

		private bool _isSaveOk;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSave(),
						() => UserOperateMode != OperateMode.Query && (AddOrUpdateF151001 != null
						&& (AddOrUpdateF151001.STATUS == "0"  || AddOrUpdateF151001.STATUS == "1" || AddOrUpdateF151001.STATUS == "8" || AddOrUpdateF151001.STATUS == "3" || AddOrUpdateF151001.STATUS == "4"
												))
						,
						o => SaveComplate()
						);
			}
		}

		private void DoSave()
		{
			_isSaveOk = false;
			AddOrUpdateF151001.DC_CODE = AddOrUpdateF151001.SRC_DC_CODE;
			AddOrUpdateF151001.TAR_WAREHOUSE_ID = AddOrUpdateF151001.TAR_WAREHOUSE_ID?.ToUpper();
			AddOrUpdateF151001.SRC_WAREHOUSE_ID = AddOrUpdateF151001.SRC_WAREHOUSE_ID?.ToUpper();
			if (UserOperateMode == OperateMode.Edit)
			{
				var proxyF15 = GetProxy<F15Entities>();
				var f151001 = proxyF15.F151001s.Where(
					o =>
						o.DC_CODE == AddOrUpdateF151001.DC_CODE && o.GUP_CODE == AddOrUpdateF151001.GUP_CODE &&
						o.CUST_CODE == AddOrUpdateF151001.CUST_CODE && o.ALLOCATION_NO == AddOrUpdateF151001.ALLOCATION_NO).FirstOrDefault();
				if (f151001 != null && !((f151001.STATUS == "0" && !string.IsNullOrWhiteSpace(f151001.TAR_WAREHOUSE_ID)) || f151001.STATUS == "1" || f151001.STATUS == "8" || f151001.STATUS == "3" || f151001.STATUS == "4"))
				{
					ShowWarningMessage(Properties.Resources.P1502010000_TranferStatusCannotModify);
					AddOrUpdateF151001 = ExDataMapper.Map<F151001, F151001>(f151001);
					DoSearch();
					return;
				}
				var f151002List = proxyF15.F151002s.Where(o =>
					o.DC_CODE == AddOrUpdateF151001.DC_CODE && o.GUP_CODE == AddOrUpdateF151001.GUP_CODE &&
					o.CUST_CODE == AddOrUpdateF151001.CUST_CODE && o.ALLOCATION_NO == AddOrUpdateF151001.ALLOCATION_NO).ToList();
				if (f151001 != null && (f151001.STATUS == "0" || f151001.STATUS == "1") && f151002List.Any(o => o.STATUS == "1" || o.A_SRC_QTY > 0))
				{
					ShowWarningMessage(Properties.Resources.P1502010000_ItemDiscontinuedCannotModify);
					return;
				}
				if (f151001 != null && f151002List.Any(o => o.STATUS == "2" || o.A_TAR_QTY > 0))
				{
					ShowWarningMessage(f151001.STATUS == "3" ? Properties.Resources.P1502010000_ItemOntheMarketCannotModify : Properties.Resources.P1502010000_CheckOutItemCannotModify);
					return;
				}
			}
			if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
			{
				if (DgItemList != null && DgItemList.Any())
				{
					#region 調撥明細檢核
					DgItemList.ForEach(x =>
					{
						x.Item.SRC_LOC_CODE = x.Item.SRC_LOC_CODE.ToUpper();
						x.Item.TAR_LOC_CODE = x.Item.TAR_LOC_CODE.ToUpper();

					});

					var message = CheckItemQty(AddOrUpdateF151001, DgItemList);
					if (!string.IsNullOrEmpty(message))
					{
						ShowWarningMessage(message);
						return;
					}

					var proxyP19Ex = GetExProxy<P19ExDataSource>();
					var proxyEx = GetExProxy<P15ExDataSource>();

					foreach (var ditem in DgItemList)
					{
						//ditem.Item.SUG_LOC_CODE = ditem.Item.TAR_LOC_CODE;
						ditem.Item.TAR_WAREHOUSE_ID = AddOrUpdateF151001.TAR_WAREHOUSE_ID;
						if (ditem.Item.TAR_QTY == 0 || (!string.IsNullOrEmpty(AddOrUpdateF151001.SRC_WAREHOUSE_ID) && ditem.Item.TAR_QTY > ditem.Item.SRC_QTY))
						{
							ShowWarningMessage(Properties.Resources.P1502010000_CheckEnoughTAR_Qty);
							return;
						}
						if (ditem.Item.STATUS == "8" && string.IsNullOrEmpty(ditem.Item.TAR_WAREHOUSE_ID))
						{
							ShowWarningMessage(Properties.Resources.P1502010000_ChooseTARWarehouseID);
							return;
						}
						if (!string.IsNullOrWhiteSpace(AddOrUpdateF151001.TAR_WAREHOUSE_ID))
						{
							//儲位倉別檢查
							//若儲位權限需檢核,可以加在這 2015/5/12 Sam
							var tmpresult = proxyEx.CreateQuery<ExDataServices.P15ExDataService.ExecuteResult>("CheckLocCodeInWarehouseId")
								.AddQueryExOption("dcCode", ditem.Item.TAR_DC_CODE)
								.AddQueryExOption("warehouseId", AddOrUpdateF151001.TAR_WAREHOUSE_ID)
								.AddQueryExOption("locCode", ditem.Item.TAR_LOC_CODE.Replace("-", ""))
								.ToList();
							if (!tmpresult[0].IsSuccessed)
							{
								ShowWarningMessage(Properties.Resources.P1502010000_CheckLocCodeInWarehouseId);
								return;
							}
						}
					}

					#endregion

					#region Add or Update
					var proxyWcf = new wcf.P15WcfServiceClient();

					var wcfDgItemList = ExDataMapper.MapCollection<F151001DetailDatas, wcf.F151001DetailDatas>(DgItemList.Select(o => o.Item).ToArray()).ToArray();
					var wcfF151001 = ExDataMapper.Map<F151001, wcf.F151001>(AddOrUpdateF151001);
					var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.InsertOrUpdateF151001Datas(wcfF151001, wcfDgItemList, _userId, _userName));
					if (result.IsSuccessed)
						ShowMessage(UserOperateMode == OperateMode.Add ? Messages.InfoAddSuccess : Messages.InfoUpdateSuccess);
					else
					{
						var error = UserOperateMode == OperateMode.Add ? Messages.ErrorAddFailed : Messages.ErrorUpdateFailed;
						var errorMessage = error.Message;
						errorMessage += Environment.NewLine + result.Message;
						ShowWarningMessage(errorMessage);
						return;
					}

					#endregion
				}
				else
				{
					ShowWarningMessage(Properties.Resources.P1502010000_NoDetailData);
					return;
				}

				_isSaveOk = true;
			}
		}

		private void SaveComplate()
		{
			if (_isSaveOk)
			{
				if (AddOrUpdateF151001 != null)
				{
					//先把搜尋改成該筆資料
					CRTDateS = AddOrUpdateF151001.CRT_ALLOCATION_DATE;
					CRTDateE = AddOrUpdateF151001.CRT_ALLOCATION_DATE;
					PostingDateS = null;
					PostingDateE = null;
					SourceDcCode = AddOrUpdateF151001.SRC_DC_CODE;
					TargetDcCode = AddOrUpdateF151001.TAR_DC_CODE;
					TxtSearchAllocationNo = "";
					SelectSourceWarehouse =  "";
					SelectTargetWarehouse = "";
				}
				UserOperateMode = OperateMode.Query;
				DoSearch();
				if(DgList.Any(x=> x.ALLOCATION_NO == AddOrUpdateF151001.ALLOCATION_NO))
				{
					SelectedData = DgList.FirstOrDefault(x => x.ALLOCATION_NO == AddOrUpdateF151001.ALLOCATION_NO);
				}
				else
				{
					IsSearch = true;
					IsSelectedData = false;
				}
			
				string isSendCar = AddOrUpdateF151001 != null ? AddOrUpdateF151001.SEND_CAR : "0";
				if (isSendCar ==  "1")
				{
					var message = new MessagesStruct
					{
						Button = DialogButton.YesNo,
						Image = DialogImage.Question,
						Message = Properties.Resources.P1502010000_DoSendCarCheck,
						Title = Resources.Resources.Warning
					};
					if (ShowMessage(message) == DialogResponse.Yes)
						DoSendCar();
				}
				AddOrUpdateF151001 = null;
				DgItemList = null;
			}
		}
		#endregion Save

		#region Posting
		public ICommand PostingCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoPosting(), () => UserOperateMode == OperateMode.Query && SelectedData != null && (( SelectedData.STATUS =="1" || SelectedData.STATUS == "3") && !string.IsNullOrEmpty(SelectedData.TAR_WAREHOUSE_ID) && !CheckAutomaticWarehouse(SelectedData.TAR_DC_CODE,SelectedData.TAR_WAREHOUSE_ID))
				);
			}
		}

		private void DoPosting()
		{

		}
		#endregion

		#region 紙本下架完成
		public ICommand FinishedOffShelfCommand
		{
            get
            {
                return CreateBusyAsyncCommand(
                        o => DoFinishedOffShelf(), () => UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData?.STATUS == "1" && !CheckAutomaticWarehouse(SelectedData.SRC_DC_CODE, SelectedData.SRC_WAREHOUSE_ID) && (string.IsNullOrEmpty(SelectedData.TAR_WAREHOUSE_ID) || CheckAutomaticWarehouse(SelectedData.TAR_DC_CODE, SelectedData.TAR_WAREHOUSE_ID)),
                        o => DoFinishedOffShelfComplete(),
                        e => { },
                        () => { FinishedOffShelfLackProcess(); }
                );
            }
        }

    public void DoFinishedOffShelf()
    {
      var proxy = new wcf.P15WcfServiceClient();
      var result = new wcf.ExecuteResult();
      if (IsInputLack)
        //在缺貨UI上已經呼叫過，不用再呼叫一次
        result.IsSuccessed = true;
      else
        result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                              () => proxy.FinishedOffShelf(SelectedData.DC_CODE, SelectedData.GUP_CODE
                                             , SelectedData.CUST_CODE, SelectedData.ALLOCATION_NO));
      if (IsPosted)
        ShowInfoMessage(Properties.Resources.P1502010000_NO_LACK_TO_COMPLETE);

      if (!result.IsSuccessed)
        ShowResultMessage(result);
    }

    public void DoFinishedOffShelfComplete()
		{
			DateTime tmpCRTDateS = new DateTime();
			DateTime tmpCRTDateE = new DateTime();
			string tmpSourceDcCode = "";
			string tmpTargetDcCode = "";
			string tmpTxtSearchALLOCATION_NO = "";

			if (SelectedData != null)
			{
				tmpCRTDateS = SelectedData.CRT_ALLOCATION_DATE;
				tmpCRTDateE = SelectedData.CRT_ALLOCATION_DATE;
				tmpSourceDcCode = SelectedData.SRC_DC_CODE;
				tmpTargetDcCode = SelectedData.TAR_DC_CODE;
				tmpTxtSearchALLOCATION_NO = SelectedData.ALLOCATION_NO;

				CRTDateS = tmpCRTDateS;
				CRTDateE = tmpCRTDateE;
				PostingDateS = null;
				PostingDateE = null;
				SourceDcCode = tmpSourceDcCode;
				TargetDcCode = tmpTargetDcCode;
				TxtSearchAllocationNo = "";
				SelectSourceWarehouse = "";
				SelectTargetWarehouse = "";
				DoSearch();
				if (DgList.Any(x => x.ALLOCATION_NO == tmpTxtSearchALLOCATION_NO))
				{
					SelectedData = DgList.FirstOrDefault(x => x.ALLOCATION_NO == tmpTxtSearchALLOCATION_NO);
				}
			}
		


		}
		#endregion

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

		private void Import()
		{
			string fullFilePath = ImportFilePath;
			var errorMeg = string.Empty;

			var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);

			if (excelTable == null)
			{
				if (!string.IsNullOrWhiteSpace(errorMeg))
				{
					DialogService.ShowMessage(errorMeg);
					return;
				}
				DialogService.ShowMessage(Properties.Resources.P1502010000_CSVisnotCorrectOrOpening);
				return;
			}

			if (excelTable.Columns.Count < 12)
			{
				DialogService.ShowMessage(Properties.Resources.P1502010000_ColumnCountNotSufficient);
				return;
			}

			// 驗證欄位
			int index = 2;
			List<string> errMessageList = new List<string>();
			List<int> vDateExistCount = new List<int>();
			List<int> vDateNotExistCount = new List<int>();

			foreach (var col in excelTable.AsEnumerable())
			{
				string err = string.Empty;
				for (int i = 0; i <= 7; i++)
				{
					if (string.IsNullOrWhiteSpace(Convert.ToString(col[i])))
						err += string.Format(Properties.Resources.P1502010000_NotCorrectCol, col.Table.Columns[i]) + ",";
				}

				if (string.IsNullOrWhiteSpace(Convert.ToString(col[10])))
					err += string.Format(Properties.Resources.P1502010000_NotCorrectCol, col.Table.Columns[10]) + ",";

				Int16 qty;
				var notQty = Int16.TryParse(Convert.ToString(col[11]), out qty) ? null : col[11];
				if (notQty != null)
					err += string.Format(Properties.Resources.P1502010000_NotCorrectCount, notQty) + ",";

				if (!string.IsNullOrWhiteSpace(Convert.ToString(col[8])))
				{
					vDateExistCount.Add(index);
					DateTime vdate;
					var notDate = DateTime.TryParse(Convert.ToString(col[8]), out vdate) ? null : col[8];
					if (notDate != null)
						err += string.Format(Properties.Resources.P1502010000_NotCorrectDate, notQty) + ",";
				}
				else
					vDateNotExistCount.Add(index);

				if (!string.IsNullOrWhiteSpace(err))
				{
					err = err.Substring(0, err.Length - 1);
					err = string.Format(Properties.Resources.P1502010000_NotCorrectIndex, index, err);
					errMessageList.Add(err);
				}
				index++;
			}

			if (vDateExistCount.Count != 0 && vDateNotExistCount.Count != 0)
			{
				string err = string.Empty;
				if (vDateExistCount.Count < vDateNotExistCount.Count)
				{
					err = string.Format(Properties.Resources.P1502010000_ExcelVExist,
						Environment.NewLine, string.Join(",", vDateExistCount));
					errMessageList.Add(err);
				}
				else
				{
					err = string.Format(Properties.Resources.P1502010000_ExcelVNotExist,
						Environment.NewLine, string.Join(",", vDateNotExistCount));
					errMessageList.Add(err);
				}
			}


			if (errMessageList.Count != 0)
			{
				errMessageList.Insert(0, string.Format("{0}{1}{2}{3}{4}{5}", Properties.Resources.P1502010000_ExcelRule, Environment.NewLine,
					Properties.Resources.P1502010000_ExcelRule1, Environment.NewLine,
					Properties.Resources.P1502010000_ExcelRule2, Environment.NewLine
					));
				string resultMessage = string.Empty;
				foreach (var item in errMessageList)
					resultMessage += item + "\r\n";
				DispatcherAction(() =>
				{
					var viewer = new ScrollViewer
					{
						FontSize = 20,
						Content = new TextBox
						{
							Text = resultMessage,
						},
						HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
					};
					var win = new Window { Content = viewer, WindowStartupLocation = WindowStartupLocation.CenterScreen };
					win.Title = Properties.Resources.P1502010000_ImportTransferDataResult;
					win.ShowDialog();
				});
				return;
			}

			//Int16 qty;
			//var notQty = excelTable.AsEnumerable().Where(col => !Int16.TryParse(Convert.ToString(col[11]), out qty)).Select(col => col[11]).FirstOrDefault();
			//if (notQty != null)
			//{
			//	ShowWarningMessage(string.Format(Properties.Resources.P1502010000_NotCorrectCount, notQty));
			//	return;
			//}

			// DateTime vdate;
			// var notDate = excelTable.AsEnumerable().Where(col => !DateTime.TryParse(Convert.ToString(col[8]), out vdate)).Select(col => col[8]).FirstOrDefault();
			// if (notDate != null)
			// {
			//     ShowWarningMessage(string.Format(Properties.Resources.P1502010000_NotCorrectDate, notQty));
			//     return;
			// }

			var queryData = (from col in excelTable.AsEnumerable()
							 select new F150201ImportData
							 {
								 GUP_CODE = Convert.ToString(col[0]),
								 CUST_CODE = Convert.ToString(col[1]),
								 SRC_DC_CODE = Convert.ToString(col[2]),
								 TAR_DC_CODE = Convert.ToString(col[3]),
								 SRC_WAREHOUSE_ID = Convert.ToString(col[4]),
								 TAR_WAREHOUSE_ID = Convert.ToString(col[5]),
								 ITEM_CODE = Convert.ToString(col[6]),
								 SRC_LOC_CODE = LocCodeHelper.LocCodeConverter9(Convert.ToString(col[7])),
								 VALID_DATE = string.IsNullOrWhiteSpace(Convert.ToString(col[8])) ? (DateTime?)null : (DateTime?)(Convert.ToDateTime(col[8])),
								 MAKE_NO = Convert.ToString(col[9]),
								 TAR_LOC_CODE = LocCodeHelper.LocCodeConverter9(Convert.ToString(col[10])),
								 QTY = Convert.ToInt16(col[11])
							 }).ToList();

			if (queryData != null && queryData.Any())
			{

				#region Add or Update

				var proxy = new wcf.P15WcfServiceClient();
				var importData = ExDataMapper.MapCollection<F150201ImportData, wcf.F150201ImportData>(queryData).ToArray();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
														() => proxy.ImportF150201Data(GupCode, CustCode
																					 , fullFilePath, importData));
				DispatcherAction(() =>
				{
					var viewer = new ScrollViewer
					{
						FontSize = 20,
						Content = new TextBox
						{
							Text = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", Properties.Resources.P1502010000_ExcelRule, Environment.NewLine,
					Properties.Resources.P1502010000_ExcelRule1, Environment.NewLine,
					Properties.Resources.P1502010000_ExcelRule2, Environment.NewLine,
					Environment.NewLine,
					result.Message)
						},
						HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
					};
					var win = new Window { Content = viewer, WindowStartupLocation = WindowStartupLocation.CenterScreen };
					win.Title = Properties.Resources.P1502010000_ImportTransferDataResult;
					win.ShowDialog();
				});


				#endregion

			}
			else
			{
				ShowWarningMessage(Properties.Resources.P1502010000_NoDetailData);
				return;
			}
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

				errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P1502010000_ImportFail, true);


				return null;
			}





		}
		#endregion

		#endregion

		#region Print
		public ICommand PrintAllocStickerCommand
		{
			get
			{
				

				return new RelayCommand<PrintType>(
						DoAllocStickerPrint,
						(t) => !IsBusy && SelectedData != null
								 && UserOperateMode == OperateMode.Query

				);
			}
		}

		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
						DoPrint,
						(t) => !IsBusy && SelectedData != null
								 && UserOperateMode == OperateMode.Query &&  CanExcutePrint()
				);
			}
		}

		public bool CanExcutePrint()
		{
			if (SelectedData.STATUS == "5")
				return true;
			if ((!CheckAutomaticWarehouse(SelectedData.SRC_DC_CODE,SelectedData.SRC_WAREHOUSE_ID) && (SelectedData.STATUS == "0" || SelectedData.STATUS == "1")))
			{
				return true;
			}
			if(!CheckAutomaticWarehouse(SelectedData.TAR_DC_CODE,SelectedData.TAR_WAREHOUSE_ID) && SelectedData.STATUS == "3")
			{
				return true;
			}

			return false;
		}

		private void DoAllocStickerPrint(PrintType printType)
		{
            var proxyWcf = new wcf.P15WcfServiceClient();
            var wcfReq = ExDataMapper.Map<P1502010000Data, wcf.P1502010000Data>(SelectedData);
            var result = RunWcfMethod<wcf.F151001ReportDataByTicket>(proxyWcf.InnerChannel, () => proxyWcf.GetF151001ReportDataByTicket(wcfReq));
            F151001ReportDataByTicket = new List<wcf.F151001ReportDataByTicket> { result };
			DoPrintStickerReport(PrintType.ToPrinter);
		}

		private void DoPrint(PrintType printType)
		{
			var message = new MessagesStruct();
			if (SelectedData.STATUS != "5")
			{
				message = new MessagesStruct
				{
					Button = DialogButton.YesNo,
					Image = DialogImage.Warning,
					Message = Properties.Resources.P1502010000_GenerateAndPrintTransfer,
					Title = Resources.Resources.Warning
				};
				if (ShowMessage(message) != DialogResponse.Yes) return;
			}
			//只有未列印待處理更新
			if (SelectedData.STATUS == "0")
				UpdateIsPrinted();
			//取得報表資料
			DateTime tmpCRTDateS = new DateTime();
			DateTime tmpCRTDateE = new DateTime();
			string tmpSourceDcCode = "";
			string tmpTargetDcCode = "";
			string tmpTxtSearchALLOCATION_NO = "";
			string tmpSelectSourceWarehouse = "";
			string tmpSelectTargetWarehouse = "";
			if (SelectedData != null)
			{
				tmpCRTDateS = SelectedData.CRT_ALLOCATION_DATE;
				tmpCRTDateE = SelectedData.CRT_ALLOCATION_DATE;
				tmpSourceDcCode = SelectedData.SRC_DC_CODE;
				tmpTargetDcCode = SelectedData.TAR_DC_CODE;
				tmpTxtSearchALLOCATION_NO = SelectedData.ALLOCATION_NO;
				tmpSelectSourceWarehouse = SelectedData.SRC_WAREHOUSE_ID;
				tmpSelectTargetWarehouse = SelectedData.TAR_WAREHOUSE_ID;
			}
			bool isHasData = false;
			var proxyEx = GetExProxy<P15ExDataSource>();
			if (SelectedData.ISEXPENDDATE == "1" || SelectedData.STATUS == "5")
			{
			  F151001ReportDataByExpendDate = proxyEx.CreateQuery<F151001ReportDataByExpendDate>("GetF151001ReportDataByExpendDate")
																				.AddQueryExOption("dcCode", SelectedData.DC_CODE)
																				.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
																				.AddQueryExOption("custCode", SelectedData.CUST_CODE)
																				.AddQueryExOption("allocationNo", SelectedData.ALLOCATION_NO)
																				.ToList();
				isHasData = F151001ReportDataByExpendDate.Any();
			}
			else
			{
				F151001ReportData = proxyEx.CreateQuery<F151001ReportData>("GetF151001ReportData")
																				.AddQueryExOption("dcCode", SelectedData.DC_CODE)
																				.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
																				.AddQueryExOption("custCode", SelectedData.CUST_CODE)
																				.AddQueryExOption("allocationNo", SelectedData.ALLOCATION_NO)
																				.ToList();
				isHasData = F151001ReportData.Any();
			}

			
			if (isHasData)
			{
				DoPrintReport(printType);
				//先把搜尋改成該筆資料
				CRTDateS = tmpCRTDateS;
				CRTDateE = tmpCRTDateE;
				PostingDateS = null;
				PostingDateE = null;
				SourceDcCode = tmpSourceDcCode;
				TargetDcCode = tmpTargetDcCode;
				TxtSearchAllocationNo = "";
				SelectSourceWarehouse = "";
				SelectTargetWarehouse = "";
				DoSearch();
				if (DgList.Any(x => x.ALLOCATION_NO == tmpTxtSearchALLOCATION_NO))
				{
					SelectedData = DgList.FirstOrDefault(x => x.ALLOCATION_NO == tmpTxtSearchALLOCATION_NO);
				}
			}
			else
				message.Message = Properties.Resources.P1502010000_EmptyReportDataCannotViewOrPrint;
		}

		private void UpdateIsPrinted()
		{
			if (SelectedData != null)
			{
				var proxyEx = GetModifyQueryProxy<F15Entities>();
				var f15001s = proxyEx.F151001s.Where(x => x.DC_CODE.Equals(SelectedData.DC_CODE)
																																				 && x.GUP_CODE.Equals(SelectedData.GUP_CODE)
																																				 && x.CUST_CODE.Equals(SelectedData.CUST_CODE)
																																				 && x.ALLOCATION_NO.Equals(SelectedData.ALLOCATION_NO))
																																.AsQueryable();
				if (f15001s != null && f15001s.Count() > 0)
				{
					var f151001 = f15001s.FirstOrDefault();
					if (f151001.STATUS == "0")
					{
						f151001.STATUS = "1";
						proxyEx.UpdateObject(f151001);
						proxyEx.SaveChanges();
					}
				}
				var qry = proxyEx.F1511s.Where(x => x.DC_CODE.Equals(SelectedData.DC_CODE)
																																				 && x.GUP_CODE.Equals(SelectedData.GUP_CODE)
																																				 && x.CUST_CODE.Equals(SelectedData.CUST_CODE)
																																				 && x.ORDER_NO.Equals(SelectedData.ALLOCATION_NO))
																												.AsQueryable().ToList();
				if (qry != null && qry.Any())
				{
					foreach (var item in qry)
					{
						item.STATUS = "1";
						proxyEx.UpdateObject(item);
					}
					proxyEx.SaveChanges();
				}
			}
		}
    #endregion


    #region ExportCsvCommand  調撥單匯出
    public ICommand ExportCsvCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => CsvDoSearch(),
          () => UserOperateMode == OperateMode.Query
          );
      }
    }
    private void CsvDoSearch()
    {
      var message = string.Empty;
      if (CRTDateS > CRTDateE)
        message = Properties.Resources.P1502010000_CRTDateIllegal;
      else if (PostingDateS.HasValue && PostingDateE.HasValue && PostingDateS > PostingDateE)
        message = Properties.Resources.P1502010000_PostingDateIllegal;
      if ((!PostingDateS.HasValue && PostingDateE.HasValue) || (PostingDateS.HasValue && !PostingDateE.HasValue))
        message = Properties.Resources.P1502010000_CRTDateEmpty;
      if (!string.IsNullOrEmpty(message))
      {
        ShowWarningMessage(message);
        return;
      }


      var proxyEx = GetExProxy<P15ExDataSource>();
      F151001Datas = proxyEx.CreateQuery<GetF150201CSV>("GetF150201CSVData")
            .AddQueryExOption("gupCode", GupCode)
            .AddQueryExOption("custCode", CustCode)
            .AddQueryExOption("SourceDcCode", SourceDcCode)                                                                //來源物流
            .AddQueryExOption("TargetDcCode", TargetDcCode)                                                                //目的物流
            .AddQueryExOption("CRTDateS", CRTDateS)                                                                       //建立時間 起
            .AddQueryExOption("CRTDateE", CRTDateE)                                                                       //建立時間 迄
            .AddQueryExOption("TxtSearchAllocationNo", TxtSearchAllocationNo == null ? " " : TxtSearchAllocationNo)       //調撥單號
            .AddQueryExOption("PostingDateS", PostingDateS)                                                               //過帳日期 起
            .AddQueryExOption("PostingDateE", PostingDateE)                                                               //過帳日期 迄
            .AddQueryExOption("SourceWarehouseList", SelectSourceWarehouse)                                               //來源倉別
            .AddQueryExOption("TargetWarehouseList", SelectTargetWarehouse)                                               //目的倉別
            .AddQueryExOption("StatusList", SelectSTATUS)                                                                 //單具狀態
            .AddQueryExOption("TxtSearchSourceNo", TxtSearchSourceNo == null ? " " : TxtSearchSourceNo)                   //來源單號
             .ToList();

      if (F151001Datas != null && F151001Datas.Any())
      {
        SelectedF151001Data = F151001Datas.First();
        var detailDataTable = F151001Datas.ToDataTable();

        var showColumnName = new List<string>
          {
          "GUP_CODE",
          "CUST_CODE",
          "SRC_DC_CODE",
          "TAR_DC_CODE",
          "SRC_WAREHOUSE_ID",
          "TAR_WAREHOUSE_ID",
          "ITEM_CODE",
          "SRC_LOC_CODE",
          "TAR_LOC_CODE",
          "SRC_QTY"
        };

        var delColumnList = new List<string>();
        foreach (DataColumn column in detailDataTable.Columns)
        {
          if (!showColumnName.Contains(column.ColumnName))
            delColumnList.Add(column.ColumnName);
        }
        //定義欄位名稱
        foreach (var columnName in delColumnList)
          detailDataTable.Columns.Remove(columnName);
        detailDataTable.Columns["GUP_CODE"].SetOrdinal(0);
        detailDataTable.Columns["GUP_CODE"].ColumnName = Properties.Resources.P1502010000_Gup_Code;
        detailDataTable.Columns["CUST_CODE"].SetOrdinal(1);
        detailDataTable.Columns["CUST_CODE"].ColumnName = Properties.Resources.P1502010000_Cust_Code;
        detailDataTable.Columns["SRC_DC_CODE"].SetOrdinal(2);
        detailDataTable.Columns["SRC_DC_CODE"].ColumnName = Properties.Resources.P1502010000_SRC_DC_CODE;
        detailDataTable.Columns["TAR_DC_CODE"].SetOrdinal(3);
        detailDataTable.Columns["TAR_DC_CODE"].ColumnName = Properties.Resources.P1502010000_TAR_DC_CODE;
        detailDataTable.Columns["SRC_WAREHOUSE_ID"].SetOrdinal(4);
        detailDataTable.Columns["SRC_WAREHOUSE_ID"].ColumnName = Properties.Resources.P1502010000_SRC_WAREHOUSE_ID;
        detailDataTable.Columns["TAR_WAREHOUSE_ID"].SetOrdinal(5);
        detailDataTable.Columns["TAR_WAREHOUSE_ID"].ColumnName = Properties.Resources.P1502010000_TAR_WAREHOUSE_ID;
        detailDataTable.Columns["ITEM_CODE"].SetOrdinal(6);
        detailDataTable.Columns["ITEM_CODE"].ColumnName = Properties.Resources.P1502010000_ITEM_CODE;
        detailDataTable.Columns["SRC_LOC_CODE"].SetOrdinal(7);
        detailDataTable.Columns["SRC_LOC_CODE"].ColumnName = Properties.Resources.P1502010000_SRC_LOC_CODE;
        detailDataTable.Columns["TAR_LOC_CODE"].SetOrdinal(8);
        detailDataTable.Columns["TAR_LOC_CODE"].ColumnName = Properties.Resources.P1502010000_TAR_LOC_CODE;
        detailDataTable.Columns["SRC_QTY"].SetOrdinal(9);
        detailDataTable.Columns["SRC_QTY"].ColumnName = Properties.Resources.P1502010000_QRT;


        Data = detailDataTable;

        //匯出CSV檔
        string destFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
        var saveFileDialog = new SaveFileDialog
        {
          DefaultExt = "xlsx",
          Filter = "Excel (.xlsx)|*.xlsx",
          RestoreDirectory = true,
          OverwritePrompt = true,
          Title = Properties.Resources.P1502010000_destFileName,
          FileName = destFileName
        };

        var isShowOk = saveFileDialog.ShowDialog();
        if (isShowOk != true) return;

        //匯出從CSV檔改為Excel檔
        //var f = new FileInfo(saveFileDialog.FileName);
        var isExportSuccess = true;

        try
        {
          var excelExportService = new ExcelExportService();
          excelExportService.CreateNewSheet("Sheet0");
          var excelReportDataSource = new ExcelExportReportSource();
          excelReportDataSource.Data = Data;
          excelExportService.AddExportReportSource(excelReportDataSource);
          isExportSuccess = excelExportService.Export(Path.GetDirectoryName(saveFileDialog.FileName),
          Path.GetFileName(saveFileDialog.FileName));
          //Data.ExportDataAsCSVString(f.Directory + "/" + f.Name);
        }
        catch (Exception)
        {
          isExportSuccess = false;
        }

        DialogService.ShowMessage(isExportSuccess ? Properties.Resources.P1502010000_ExportQuertResultSuccess : Properties.Resources.P1502010000_ExportQuertResultFail);
        return;
      }
      else
        ShowMessage(Messages.InfoNoData);

    }
    #endregion

    #endregion

    #region 判斷上架倉別是否為自動倉(true:是自動倉，false:人工倉)
    public bool CheckAutomaticWarehouse(string dcCode,string warehouseId)
		{
			if (string.IsNullOrWhiteSpace(warehouseId))
				return false;

			var proxy = GetProxy<F19Entities>();
			return proxy.F1980s.Where(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId).FirstOrDefault()?.DEVICE_TYPE == "0" ? false : true;
		}

    #endregion

    /// <summary>
    /// 檢查是否有輸入過缺貨記錄，避免重複跳出視窗 true=有 false=沒有
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="AllocationNo"></param>
    /// <returns></returns>
    public Boolean IsProcessedLack(String dcCode, String gupCode, String custCode, String AllocationNo)
    {
      var proxy = GetProxy<F15Entities>();
      //是否已輸入過缺貨內容
      return proxy.F151003s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALLOCATION_NO == AllocationNo).ToList().Any();
    }

  }
}
