using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P16WcfService;

namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1601020000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		private bool _isError;
		public Action<PrintType> DoPrintReport = delegate { };
		public string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		public string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
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

		private string _selectDcCode;
		public string SelectDcCode
		{
			get { return _selectDcCode; }
			set
			{
				_selectDcCode = value;
				RaisePropertyChanged("SelectDcCode");
			}
		}
		#endregion
		#region Form - 建立日期起迄
		private DateTime? _cRTDateS = DateTime.Today;
		public DateTime? CRTDateS
		{
			get { return _cRTDateS; }
			set { _cRTDateS = value; RaisePropertyChanged("CRTDateS"); }
		}
		private DateTime? _cRTDateE = DateTime.Today;
		public DateTime? CRTDateE
		{
			get { return _cRTDateE; }
			set { _cRTDateE = value; RaisePropertyChanged("CRTDateE"); }
		}
		#endregion
		#region Form - 調撥申請單號
		private string _txtSearchRTN_APPLY_NO;
		public string TxtSearchRTN_APPLY_NO
		{
			get { return _txtSearchRTN_APPLY_NO; }
			set
			{
				_txtSearchRTN_APPLY_NO = value;
				RaisePropertyChanged("TxtSearchRTN_APPLY_NO");
			}
		}
		#endregion
		#region Form - 單據狀態
		private List<NameValuePair<string>> _searchSTATUS;

		public List<NameValuePair<string>> SearchSTATUS
		{
			get { return _searchSTATUS; }
			set
			{
				_searchSTATUS = value;
				RaisePropertyChanged("SearchSTATUS");
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

		private bool _serachResultIsExpanded = true;

		public bool SerachResultIsExpanded
		{
			get { return _serachResultIsExpanded; }
			set
			{
				_serachResultIsExpanded = value;
				RaisePropertyChanged("SerachResultIsExpanded");
			}
		}

		#endregion
		#region Data - 資料List
		private List<F161601> _dgList;
		public List<F161601> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private F161601 _selectedData;

		public F161601 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
				IsSearch = (_selectedData == null);
				GetDetailData(_selectedData);
				IsSelectedData = _selectedData != null;

        if (value != null)
					_savedF161601Clone = ExDataMapper.Clone(value);
			}
		}

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

		private string _selectedDC_CODE;
		public string SelectedDC_CODE
		{
			get { return _selectedDC_CODE; }
			set
			{
				_selectedDC_CODE = value;
				RaisePropertyChanged("SelectedDC_CODE");
			}
		}
		#endregion
		#region Data - 總計明細筆數
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
		#region Data - 調撥申請單明細List
		private List<F161601DetailDatas> _dgItemList;
		public List<F161601DetailDatas> DgItemList
		{
			get { return _dgItemList; }
			set
			{
				_dgItemList = value;
				RaisePropertyChanged("DgItemList");
			}
		}
		#endregion
		#region Data - 調撥單單號List
		private List<F151001Data> _dgItemNoList;
		public List<F151001Data> DgItemNoList
		{
			get { return _dgItemNoList; }
			set
			{
				_dgItemNoList = value;
				RaisePropertyChanged("DgItemNoList");
			}
		}
		#endregion
		#endregion

		private NameValuePair<string> _editableDcItem;

		public NameValuePair<string> EditableDcItem
		{
			get { return _editableDcItem; }
			set
			{
				_editableDcItem = value;
				RaisePropertyChanged("EditableDcItem");

				if (value == null || UserOperateMode != OperateMode.Add)
					return;

				// 更換物流中心的話，直接清除明細
				if (DgDetailItemList != null && DgDetailItemList.Any(item => item.Item.DC_CODE != value.Value))
					DgDetailItemList = new SelectionList<F161601DetailDatas>(new List<F161601DetailDatas>());

				if (DgSearchLocList != null && DgSearchLocList.Any(item => item.Item.DC_CODE != value.Value))
					DgSearchLocList = new SelectionList<F161401ReturnWarehouse>(new List<F161401ReturnWarehouse>());

        WarehouseList = GetUserWarehouse();
        if (WarehouseList?.Any() ?? false)
          SelectWarehouse = WarehouseList.FirstOrDefault();

      }
    }


		#region Form - 新增修改
		#region Form - 查詢-退貨單號
		private string _searchReturn_No = string.Empty;
		public string SearchReturn_No
		{
			get { return _searchReturn_No; }
			set { _searchReturn_No = value; RaisePropertyChanged("SearchReturn_No"); }
		}
		#endregion
		#region Form - 查詢-儲位
		private string _searchLoc_Code = string.Empty;
		public string SearchLoc_Code
		{
			get { return _searchLoc_Code; }
			set { _searchLoc_Code = value; RaisePropertyChanged("SearchLoc_Code"); }
		}
		#endregion
		#region Form - 查詢-品號
		private string _searchItem_Code = string.Empty;
		public string SearchItem_Code
		{
			get { return _searchItem_Code; }
			set { _searchItem_Code = value; RaisePropertyChanged("SearchItem_Code"); }
		}
		#endregion
		#region Form - 查詢-品名
		private string searchItem_Name = string.Empty;
		public string SearchItem_Name
		{
			get { return searchItem_Name; }
			set { searchItem_Name = value; RaisePropertyChanged("SearchItem_Name"); }
		}
		#endregion
		#region Form - 使用者倉別
		private List<F1980> _warehouseList;
		public List<F1980> WarehouseList
		{
			get { return _warehouseList; }
			set { _warehouseList = value; RaisePropertyChanged("WarehouseList"); }
		}

		private F1980 _selectWarehouse;
		public F1980 SelectWarehouse
		{
			get { return _selectWarehouse; }
			set { _selectWarehouse = value; RaisePropertyChanged("SelectWarehouse"); }
		}
		#endregion

		#region Data - 總計明細筆數
		private int? _detailCount;
		public int? DetailCount
		{
			get { return _detailCount; }
			set
			{
				_detailCount = value;
				RaisePropertyChanged("DetailCount");
			}
		}
		#endregion
		#region Data - 查詢-退貨暫存倉List
		private SelectionList<F161401ReturnWarehouse> _dgSearchLocList;
		public SelectionList<F161401ReturnWarehouse> DgSearchLocList
		{
			get { return _dgSearchLocList; }
			set
			{
				_dgSearchLocList = value;
				RaisePropertyChanged("DgSearchLocList");
			}
		}

		private bool _isSelectedAll = false;
		public bool IsSelectedAll
		{
			get { return _isSelectedAll; }
			set { _isSelectedAll = value; RaisePropertyChanged("IsSelectedAll"); }
		}

		private SelectionItem<F161401ReturnWarehouse> _selectedSearchItem;
		public SelectionItem<F161401ReturnWarehouse> SelectedSearchItem
		{
			get { return _selectedSearchItem; }
			set
			{
				_selectedSearchItem = value;
				RaisePropertyChanged("SelectedSearchItem");
				if (_selectedSearchItem != null)
				{
					if (_selectedSearchItem.Item.ITEM_CODE.Trim().Equals("XYZ00001"))
						_selectedSearchItem.IsSelected = false;
				}
			}
		}
		#endregion
		#region Data - 調撥申請商品明細List
		private SelectionList<F161601DetailDatas> _dgDetailItemList = new SelectionList<F161601DetailDatas>(new List<F161601DetailDatas>());
		public SelectionList<F161601DetailDatas> DgDetailItemList
		{
			get { return _dgDetailItemList; }
			set
			{
				_dgDetailItemList = value;
				if (_dgDetailItemList != null && _dgDetailItemList.Any())
					DetailCount = _dgDetailItemList.Count();
				RaisePropertyChanged("DgDetailItemList");
			}
		}

		private List<F161601DetailDatas> _oldDetailItemList;

		private bool _isEditSelectedAll = false;
		public bool IsEditSelectedAll
		{
			get { return _isEditSelectedAll; }
			set { _isEditSelectedAll = value; RaisePropertyChanged("IsEditSelectedAll"); }
		}

		private SelectionItem<F161601DetailDatas> _selEditDetailData;

		public SelectionItem<F161601DetailDatas> SelEditDetailData
		{
			get { return _selEditDetailData; }
			set
			{
				_selEditDetailData = value;
				RaisePropertyChanged("SelEditDetailData");
			}
		}
		#endregion
		#endregion

		#region Data - 列印
		#region Data - 列印資料表
		private List<P160102Report> _p160102Reports;
		public List<P160102Report> P160102Reports
		{
			get { return _p160102Reports; }
			set
			{
				_p160102Reports = value;
				RaisePropertyChanged("P160102Reports");
			}
		}

		public DataTable P160102ReportsDT
		{
			get
			{
				_p160102Reports.ForEach(x =>
				{
					x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO);
					x.RtnApplyNoBarcode = BarcodeConverter128.StringToBarcode(x.RTN_APPLY_NO);
					x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
				});

				return _p160102Reports.ToDataTable();
			}
		}
		#endregion
		#endregion

		#region ItemSource

		private NameValuePair<string> _selectedVnrItem;

		public NameValuePair<string> SelectedVnrItem
		{
			get { return _selectedVnrItem; }
			set
			{
				_selectedVnrItem = value;
				RaisePropertyChanged("SelectedVnrItem");
			}
		}


		private List<NameValuePair<string>> _vnrList;

		public List<NameValuePair<string>> VnrList
		{
			get { return _vnrList; }
			set
			{
				_vnrList = value;
				RaisePropertyChanged("VnrList");
			}
		}

		public void SetVnrList()
		{
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			var proxy = GetProxy<F19Entities>();
			var query = from item in proxy.F1908s
						where item.GUP_CODE == gupCode
						select new NameValuePair<string>
						{
							Name = item.VNR_NAME,
							Value = item.VNR_CODE
						};

			VnrList = query.ToList();
		}

		#endregion
		#endregion

		#region 函式
		public P1601020000_ViewModel()
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
			DcCodes = GetDcCodeList();
			if (DcCodes.Any()) SelectDcCode = DcCodes.FirstOrDefault().Value;
			SearchSTATUS = GetStatusList();
			SelectSTATUS = SearchSTATUS.FirstOrDefault().Value;
			SetVnrList();
			SelectedVnrItem = VnrList.FirstOrDefault();
		}

		private void Clear()
		{
			SelectedData = null;
			TxtSearchRTN_APPLY_NO = "";
			SearchReturn_No = "";
			SearchLoc_Code = "";
			SearchItem_Code = "";
			SearchItem_Name = "";
			SelectSTATUS = SearchSTATUS.FirstOrDefault().Value;
			DgItemList = new List<F161601DetailDatas>();
			DgItemNoList = new List<F151001Data>();
			DgSearchLocList = new SelectionList<F161401ReturnWarehouse>(new List<F161401ReturnWarehouse>());
			DgDetailItemList = new SelectionList<F161601DetailDatas>(new List<F161601DetailDatas>());
		}
		public List<NameValuePair<string>> GetDcCodeList()
		{
			return Wms3plSession.Get<GlobalInfo>().DcCodeList;
		}

		public List<NameValuePair<string>> GetStatusList()
		{
			List<NameValuePair<string>> result = new List<NameValuePair<string>>();
			result.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			result.Insert(1, new NameValuePair<string>() { Value = "0", Name = Properties.Resources.P1601010000_Pending });
			result.Insert(2, new NameValuePair<string>() { Value = "2", Name = Properties.Resources.P1601010000_CloseCase });
			result.Insert(3, new NameValuePair<string>() { Value = "9", Name = Properties.Resources.P1601010000_Cancel });
			return result.ToList();
		}

		public List<F1980> GetUserWarehouse()
		{
      var proxy = GetProxy<F19Entities>();
      return proxy.F1980s.Where(x => x.DC_CODE == EditableDcItem.Value).ToList()
                      .Where(x => !new[] { "T", "I", "D", "M", "R" }.Contains(x.WAREHOUSE_TYPE)).ToList();
		}
		private void GetDetailData(F161601 SelData)
		{
			if (UserOperateMode == OperateMode.Query && SelData != null && !string.IsNullOrEmpty(SelData.RTN_APPLY_NO))
			{
				var proxy = GetProxy<F16Entities>();
				var qry = proxy.F161602s.Where(x => x.DC_CODE.Equals(SelData.DC_CODE)
																				 && x.GUP_CODE == SelData.GUP_CODE
																				 && x.CUST_CODE == SelData.CUST_CODE
																				 && x.RTN_APPLY_NO == SelData.RTN_APPLY_NO)
																.AsQueryable().ToList();
				if (qry != null && qry.Any())
					SelectDetailCount = qry.Count;

				DgItemList = new List<F161601DetailDatas>();
				var proxyEx = GetExProxy<P16ExDataSource>();
				var detailqry = proxyEx.CreateQuery<F161601DetailDatas>("GetF161601DetailDatas")
											.AddQueryExOption("dcCode", SelData.DC_CODE)
											.AddQueryExOption("gupCode", SelData.GUP_CODE)
											.AddQueryExOption("custCode", SelData.CUST_CODE)
											.AddQueryExOption("rtnApplyNo", SelData.RTN_APPLY_NO)
											.AsQueryable();
				if (detailqry != null)
				{
					DgItemList = detailqry.ToList();
				}
				DgItemNoList = new List<F151001Data>();
				var proxyNo = GetExProxy<P15ExDataSource>();
				var NoQry = proxyNo.CreateQuery<F151001Data>("GetF151001Datas")
											.AddQueryExOption("dcCode", SelData.DC_CODE)
											.AddQueryExOption("gupCode", SelData.GUP_CODE)
											.AddQueryExOption("custCode", SelData.CUST_CODE)
											.AddQueryExOption("sourceNo", SelData.RTN_APPLY_NO)
											.AsQueryable();

				if (NoQry != null && NoQry.Count() > 0)
					DgItemNoList = NoQry.ToList();
			}
		}

		public void SearchRtnItem()
		{
			SearchReturn_No = SearchReturn_No.Trim();
			SearchLoc_Code = LocCodeHelper.LocCodeConverter9(SearchLoc_Code.Trim());
			SearchItem_Code = SearchItem_Code.Trim();

			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			var proxyEx = GetExProxy<P16ExDataSource>();
			var query = proxyEx.CreateQuery<F161401ReturnWarehouse>("GetF161401ReturnWarehouse")
										.AddQueryExOption("dcCode", SelectedData.DC_CODE)
										.AddQueryExOption("gupCode", gupCode)
										.AddQueryExOption("custCode", custCode)
										.AddQueryExOption("returnNo", SearchReturn_No)
										.AddQueryExOption("locCode", SearchLoc_Code)
										.AddQueryExOption("itemCode", SearchItem_Code)
										.AddQueryExOption("itemName", string.Format("%{0}%", SearchItem_Name));

			// 已儲位與品號為群組顯示不重複的項目
			var rtnList = query.ToList();
			DgSearchLocList = rtnList.OrderByDescending(item => item.LOC_QTY)
									 .ToSelectionList();
		}

		public static DataTable ListToDataTable(List<P160102Report> list)
		{
			DataTable dt = new DataTable();

			foreach (PropertyInfo info in typeof(P160102Report).GetProperties())
			{
				if (info.CanWrite)
					dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
			}
			foreach (var t in list)
			{
				DataRow row = dt.NewRow();
				foreach (PropertyInfo info in typeof(P160102Report).GetProperties())
				{
					if (info.CanWrite)
						row[info.Name] = info.GetValue(t, null);
				}
				dt.Rows.Add(row);
			}
			dt.TableName = "ReportData";
			return dt;
		}
        #endregion
        

        #region Command
        #region CheckAll
        public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckAllItem()
				);
			}
		}

		public void DoCheckAllItem()
		{
			foreach (var p in DgSearchLocList)
			{
				p.IsSelected = IsSelectedAll;
			}
		}

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
			foreach (var p in DgDetailItemList)
			{
				p.IsSelected = IsEditSelectedAll;
			}
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

		private void DoSearch()
		{
			UserOperateMode = OperateMode.Query;
			//執行查詢動作
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			MessagesStruct SearchDateMsg = new MessagesStruct();
			SearchDateMsg.Message = Properties.Resources.P1601010000_BeginDateLessThanEndDate;
			SearchDateMsg.Button = DialogButton.OK;
			SearchDateMsg.Image = DialogImage.Warning;
			SearchDateMsg.Title = Resources.Resources.Information;
			if (string.IsNullOrWhiteSpace(SelectDcCode))
			{
				ShowMessage(Messages.WarningNoDcCode);
				return;
			}
			if (CRTDateS != null && CRTDateE != null)
			{
				if (CRTDateS > CRTDateE)
				{
					ShowMessage(SearchDateMsg);
					return;
				}
			}
			var proxy = GetProxy<F16Entities>();
			var qry = proxy.F161601s.Where(x => x.DC_CODE.Equals(SelectDcCode)
																			 && x.GUP_CODE == gupCode
																			 && x.CUST_CODE == custCode)
															.Where(x => (x.RTN_APPLY_DATE >= CRTDateS || CRTDateS == null))
															.Where(x => (x.RTN_APPLY_DATE <= CRTDateE.Value.AddDays(1).AddSeconds(-1) || CRTDateE == null))
															.Where(x => (x.RTN_APPLY_NO.Equals(TxtSearchRTN_APPLY_NO) || string.IsNullOrEmpty(TxtSearchRTN_APPLY_NO)))
															.Where(x => (x.STATUS.Equals(SelectSTATUS) || (string.IsNullOrWhiteSpace(SelectSTATUS) && !x.STATUS.Equals("9"))))
															.OrderBy(item => item.RTN_APPLY_NO);
			DgList = qry.ToList();

			if (DgList.Any())
			{
				SerachResultIsExpanded = true;
			}
			else
			{
				ShowMessage(Messages.InfoNoData);
				DgList = new List<F161601>();
			}
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

		private void DoAdd()
		{
			//執行新增動作
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			SelectedData = new F161601();
			SelectedData.STATUS = "0";
			SelectedData.DC_CODE = DcCodes.FirstOrDefault().Value;
			SelectedData.GUP_CODE = gupCode;
			SelectedData.CUST_CODE = custCode;
			UserOperateMode = OperateMode.Add;
		}
		#endregion Add

		#region SearchRtnDetailCommand 從退貨檢驗單裡面查詢調撥申請商品
		/// <summary>
		/// 最後查詢的退貨編號
		/// </summary>
		private string _lastSearchRtnNo = string.Empty;
		public ICommand SearchRtnDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						SearchRtnItem();
						_lastSearchRtnNo = SearchReturn_No;
						IsSelectedAll = false;
					},
					() => !this.IsBusy && SelectedData != null && !string.IsNullOrEmpty(SelectedData.DC_CODE));
			}
		}
		#endregion

		#region AddDetail
		public ICommand AddDetailCommand
		{
			get
			{
				return new RelayCommand(
						AddDetailComplate,
						() => DgSearchLocList != null && DgSearchLocList.Any(si => si.IsSelected)
					 );
			}
		}
		private void AddDetailComplate()
		{
			// 如果品號為XYZ00001(虛擬商品)則不可勾選，因不可勾選，若勾選後，直接排除匯入的部分
			var selectedRtnQuery = DgSearchLocList.Where(si => si.IsSelected && si.Item.ITEM_CODE != "XYZ00001")
												  .Select(si => si.Item)
												  .Select(item => new F161601DetailDatas
												  {
													  ROWNUM = item.ROWNUM,
													  DC_CODE = item.DC_CODE,
													  GUP_CODE = item.GUP_CODE,
													  CUST_CODE = item.CUST_CODE,
													  ITEM_CODE = item.ITEM_CODE,
													  ITEM_COLOR = item.ITEM_COLOR,
													  ITEM_NAME = item.ITEM_NAME,
													  ITEM_SIZE = item.ITEM_SIZE,
													  ITEM_SPEC = item.ITEM_SPEC,
													  LOC_QTY = (int)item.LOC_QTY,
													  MEMO = SelectedData.MEMO,
													  MOVED_QTY = (int)item.LOC_QTY, // 上架數量則預設帶入此品項在退貨暫存倉的庫存數
													  TMPR_TYPE = item.TMPR_TYPE,

													  RTN_APPLY_DATE = SelectedData.RTN_APPLY_DATE,
													  RTN_APPLY_NO = SelectedData.RTN_APPLY_NO,
													  SRC_LOC = item.LOC_CODE,
													  STATUS = SelectedData.STATUS,
													  //TRA_LOC = item.LOC_CODE,
													  VNR_CODE = item.VNR_CODE,
													  VNR_NAME = item.VNR_NAME,
													  POST_QTY = string.IsNullOrEmpty(_lastSearchRtnNo) ? null : new Nullable<int>(item.MOVED_QTY),   // 如果查詢條件有輸入退貨單號,則從退貨單F161402.MOVED_QTY帶入,如無則空
																																					  //WAREHOUSE_ID = 由畫面選擇
																																					  //WAREHOUSE_NAME = 由畫面選擇
													  VALID_DATE = item.VALID_DATE ,
													  ENTER_DATE = item.ENTER_DATE ,
													  BOX_CTRL_NO = item.BOX_CTRL_NO,
													  PALLET_CTRL_NO = item.PALLET_CTRL_NO,
													  MAKE_NO = item.MAKE_NO,
													  TAR_BOX_CTRL_NO = item.TAR_BOX_CTRL_NO,
													  TAR_MAKE_NO = item.TAR_MAKE_NO,
													  TAR_PALLET_CTRL_NO = item.TAR_PALLET_CTRL_NO,
													  TAR_VALID_DATE = item.TAR_VALID_DATE
												  });

			var repeatItems = selectedRtnQuery.Where(item => DgDetailItemList.Any(si => si.Item.ITEM_CODE == item.ITEM_CODE && si.Item.SRC_LOC == item.SRC_LOC && si.Item.VALID_DATE == item.VALID_DATE && si.Item.ENTER_DATE == item.ENTER_DATE))
												.Select(item => new { item.ITEM_CODE, item.SRC_LOC, item.LOC_QTY, item.MOVED_QTY, item.VALID_DATE, item.ENTER_DATE })
												.ToList();

            MessagesStruct message = new MessagesStruct();
            message.Message = string.Format(Properties.Resources.P1601020000_SameItemCode, string.Join(",", repeatItems.Select(item => item.ITEM_CODE)));
            message.Button = DialogButton.YesNo;
            message.Image = DialogImage.Warning;
            message.Title = Resources.Resources.Warning;
            if (repeatItems.Any())
            {
                if (ShowMessage(message) != DialogResponse.Yes)
                {
                    var re = DgSearchLocList.Where(item => DgDetailItemList.Any(si => si.Item.ITEM_CODE == item.Item.ITEM_CODE && si.Item.SRC_LOC == item.Item.LOC_CODE && si.Item.VALID_DATE == item.Item.VALID_DATE && si.Item.ENTER_DATE == item.Item.ENTER_DATE));
                    // 將已匯入的項目移除
                    DgSearchLocList = new SelectionList<F161401ReturnWarehouse>(DgSearchLocList.Where(si => !si.IsSelected || si.Item.ITEM_CODE == "XYZ00001").Union(re).Select(si => si.Item));
                }
                else
                {
                    // 若有重複的品項、儲位，就順更新最新的庫存庫
                    foreach (var repeatItem in repeatItems)
                    {
                        DgDetailItemList.Where(si => si.Item.ITEM_CODE == repeatItem.ITEM_CODE && si.Item.SRC_LOC == repeatItem.SRC_LOC && si.Item.VALID_DATE == repeatItem.VALID_DATE && si.Item.ENTER_DATE == repeatItem.ENTER_DATE).First().Item.LOC_QTY = repeatItem.LOC_QTY;
                        DgDetailItemList.Where(si => si.Item.ITEM_CODE == repeatItem.ITEM_CODE && si.Item.SRC_LOC == repeatItem.SRC_LOC && si.Item.VALID_DATE == repeatItem.VALID_DATE && si.Item.ENTER_DATE == repeatItem.ENTER_DATE).First().Item.MOVED_QTY = repeatItem.MOVED_QTY;
                    }

                    // 將已匯入的項目移除
                    DgSearchLocList = new SelectionList<F161401ReturnWarehouse>(DgSearchLocList.Where(si => !si.IsSelected || si.Item.ITEM_CODE == "XYZ00001").Select(si => si.Item));

                }

                selectedRtnQuery = selectedRtnQuery.Where(item => !DgDetailItemList.Any(si => si.Item.ITEM_CODE == item.ITEM_CODE && si.Item.SRC_LOC == item.SRC_LOC && si.Item.VALID_DATE == item.VALID_DATE));
                // 將舊來源與新來源合併
                selectedRtnQuery.ToList().ForEach(o => DgDetailItemList.Add(new SelectionItem<F161601DetailDatas>(o)));// new SelectionList<F161601DetailDatas>(DgDetailItemList.Select(si => si.Item).Concat(selectedRtnQuery));

            }
            else
            {
                // 將已匯入的項目移除
                DgSearchLocList = new SelectionList<F161401ReturnWarehouse>(DgSearchLocList.Where(si => !si.IsSelected || si.Item.ITEM_CODE == "XYZ00001").Select(si => si.Item));
                selectedRtnQuery = selectedRtnQuery.Where(item => !DgDetailItemList.Any(si => si.Item.ITEM_CODE == item.ITEM_CODE && si.Item.SRC_LOC == item.SRC_LOC && si.Item.VALID_DATE == item.VALID_DATE));
                // 將舊來源與新來源合併
                selectedRtnQuery.ToList().ForEach(o => DgDetailItemList.Add(new SelectionItem<F161601DetailDatas>(o)));// new SelectionList<F161601DetailDatas>(DgDetailItemList.Select(si => si.Item).Concat(selectedRtnQuery));

            }

            IsSelectedAll = false;
			IsEditSelectedAll = false;

		}
		#endregion AddDetail

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData.STATUS == "0"
					);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
			var proxyEx = GetExProxy<P16ExDataSource>();
			var detailqry = proxyEx.CreateQuery<F161601DetailDatas>("GetF161601DetailDatas")
										.AddQueryExOption("dcCode", SelectedData.DC_CODE)
										.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
										.AddQueryExOption("custCode", SelectedData.CUST_CODE)
										.AddQueryExOption("rtnApplyNo", SelectedData.RTN_APPLY_NO)
										.AsQueryable();
			DgDetailItemList = detailqry.ToSelectionList();
			UserOperateMode = OperateMode.Edit;
		}
		#endregion Edit

		#region SetWarehouseCommand
		public ICommand SetWarehouseCommand
		{
			get
			{
				return new RelayCommand(
					DoSetWarehouse,
					() => SelectWarehouse != null && DgDetailItemList.Any(si => si.IsSelected)
						);
			}
		}

		string GetTmprTypeText(string tmprType)
		{
			switch (tmprType)
			{
				case "01":
					return Properties.Resources.P1601020000_RegularTemperature;
				case "02":
					return Properties.Resources.P1601020000_LowTemperature;
				case "03":
					return Properties.Resources.P1601020000_Frozen;
				default:
					return string.Empty;
			}
		}

		private void DoSetWarehouse()
		{
			var checkDatas = ExDataMapper.MapCollection<F161601DetailDatas, wcf.F161601DetailDatas>(DgDetailItemList.Where(si => si.IsSelected).Select(si => si.Item)).ToArray();
			var proxy = new wcf.P16WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult[]>(proxy.InnerChannel,
														() => proxy.CheckItemLocTmpr(SelectDcCode, _gupCode, _custCode, SelectWarehouse.WAREHOUSE_ID, checkDatas)).ToList().FirstOrDefault();
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return;
			}
			//var query = DgDetailItemList.Where(si => si.IsSelected).Select(x => x.Item).Where(x => x.TMPR_TYPE != SelectWarehouse.TMPR_TYPE);
			//if (query.Any())
			//{
			//	var itemTmprs = string.Join(",\r\n", query.Select(x => new { x.ITEM_CODE, x.TMPR_TYPE }).Distinct().Select(x => string.Format(Properties.Resources.P1601020000_Is, x.ITEM_CODE, GetTmprTypeText(x.TMPR_TYPE))));
			//	var message = string.Format(Properties.Resources.P1601020000_itemTmprs, SelectWarehouse.WAREHOUSE_NAME, GetTmprTypeText(SelectWarehouse.TMPR_TYPE), itemTmprs);
			//	message = message.Replace("\\r\\n", Environment.NewLine);
			//	ShowWarningMessage(message);
			//	return;
			//}

			foreach (var si in DgDetailItemList.Where(si => si.IsSelected))
			{
				si.Item.WAREHOUSE_ID = SelectWarehouse.WAREHOUSE_ID;
				si.Item.WAREHOUSE_NAME = SelectWarehouse.WAREHOUSE_NAME;
			}
		}
		#endregion AddDetail

		#region SetVnrCommand

		public ICommand SetVnrCommand
		{
			get
			{
				return new RelayCommand(
						DoSetVnr,
						() => SelectedVnrItem != null && DgDetailItemList.Any(si => si.IsSelected)
					 );
			}
		}

		private void DoSetVnr()
		{
			foreach (var si in DgDetailItemList.Where(si => si.IsSelected))
			{
				si.Item.VNR_CODE = SelectedVnrItem.Value;
				si.Item.VNR_NAME = SelectedVnrItem.Name;
			}
		}

		#endregion

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				bool isCancel = false;
				return CreateBusyAsyncCommand(
					o => isCancel = DoCancel(),
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						if (isCancel && _savedF161601Clone != null && DgList != null)
						{
							SelectedData = DgList.FirstOrDefault(item => item.RTN_APPLY_NO == _savedF161601Clone.RTN_APPLY_NO);
						}
					}
					);
			}
		}

		private bool DoCancel()
		{
			//執行取消動作
			if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes)
				return false;

			Clear();

			UserOperateMode = OperateMode.Query;
			return true;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData.STATUS == "0"
					);
			}
		}

		private void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
				return;

			var exProxy = GetExProxy<P16ExDataSource>();
			var query = exProxy.CreateQuery<Wms3pl.WpfClient.ExDataServices.P16ExDataService.ExecuteResult>("DeleteP160102")
										.AddQueryExOption("dcCode", SelectedData.DC_CODE)
										.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
										.AddQueryExOption("custCode", SelectedData.CUST_CODE)
										.AddQueryExOption("rtnApplyNo", SelectedData.RTN_APPLY_NO);

			var result = query.ToList().FirstOrDefault();
			ShowResultMessage(result);

			Clear();
			DoSearch();

		}
		#endregion Delete

		#region DeleteDetail
		public ICommand DeleteDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDeleteDetail(),
					() => UserOperateMode != OperateMode.Query && DgDetailItemList.Any(si => si.IsSelected),
					o => DelDetailComplate()
					);
			}
		}

		private void DoDeleteDetail()
		{
			//執行刪除動作
		}

		private void DelDetailComplate()
		{

			// 確認是否要刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			//執行刪除動作
			DgDetailItemList = new SelectionList<F161601DetailDatas>(DgDetailItemList.Where(si => !si.IsSelected).Select(si => si.Item));

		}
		#endregion DeleteDetail

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
					o => isSaved = DoSave(),
					() => UserOperateMode != OperateMode.Query && SelectedData != null && DgDetailItemList.Any(),
					o => SaveComplate(isSaved)
					);
			}
		}

		string GetEditableError(F161601 e)
		{
			if (string.IsNullOrEmpty(e.DC_CODE))
				return Properties.Resources.P1602010000xamlcs_ChooseDC;

			return string.Empty;
		}

		string GetEditableDetailsError(IEnumerable<F161601DetailDatas> detailQuery)
		{
			if (!detailQuery.Any())
				return Properties.Resources.P1601020000_ChooseTransferApplicateItem;

			if (detailQuery.Any(item => string.IsNullOrEmpty(item.WAREHOUSE_ID)))
				return Properties.Resources.P1601020000_Tar_WarehouseID_NotSet;

			if (detailQuery.Any(item => item.MOVED_QTY > item.LOC_QTY))
				return Properties.Resources.P1601020000_OnSaleQty_GreaterThan_StockQty;

			if (detailQuery.Any(item => item.MOVED_QTY == 0))
				return Properties.Resources.P1601020000_OnSaleQtyEmpty;
			return string.Empty;
		}

		private bool DoSave()
		{
			ExDataMapper.Trim(SelectedData);

			var error = GetEditableError(SelectedData);

			if (string.IsNullOrEmpty(error))
				error = GetEditableDetailsError(DgDetailItemList.Select(si => si.Item));

			if (!string.IsNullOrEmpty(error))
			{
				ShowWarningMessage(error);
				return false;
			}
			foreach (var dg in DgDetailItemList.Select(o => o.Item).ToList())
			{
				dg.TAR_BOX_CTRL_NO = string.IsNullOrWhiteSpace(dg.TAR_BOX_CTRL_NO) ? "0": dg.TAR_BOX_CTRL_NO;
				dg.TAR_PALLET_CTRL_NO = string.IsNullOrWhiteSpace(dg.TAR_PALLET_CTRL_NO) ? "0": dg.TAR_PALLET_CTRL_NO;
				dg.TAR_MAKE_NO = string.IsNullOrWhiteSpace(dg.TAR_MAKE_NO) ? "0": dg.TAR_MAKE_NO;
			}
			if(DgDetailItemList.Any(x => x.Item.VALID_DATE != x.Item.TAR_VALID_DATE || x.Item.MAKE_NO != x.Item.TAR_MAKE_NO))
			{
				if (ShowConfirmMessage(Properties.Resources.P1601020000_DetailHasChangeValidDateOrMakeNoConfirmAdustStock) == DialogResponse.No)
					return false;
			}  
			wcf.ExecuteResult result = new wcf.ExecuteResult() { Message = Properties.Resources.P1601020000_NotWork };
			var f161601 = ExDataMapper.Map<F161601, wcf.F161601>(SelectedData);
			var f161602s = ExDataMapper.MapCollection<F161601DetailDatas, wcf.F161602>(DgDetailItemList.Select(si => si.Item)).ToArray();

			var proxy = new wcf.P16WcfServiceClient();
			if (UserOperateMode == OperateMode.Add)
			{				
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
														() => proxy.InsertP160102(f161601, f161602s));
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
														() => proxy.UpdateP160102(f161601, f161602s));
			}

			ShowResultMessage(result);

			if (result.IsSuccessed)
			{
				_savedF161601Clone = ExDataMapper.Clone(SelectedData);
				Clear();
				CRTDateS = CRTDateE = DateTime.Today;
				IsSearch = true;
				IsSelectedData = false;

				DoSearch();
			}

			return result.IsSuccessed;
		}

		/// <summary>
		/// 紀錄每次最後儲存的物件，方便在搜尋後可以指定剛剛查詢的物件
		/// </summary>
		private F161601 _savedF161601Clone = null;

		private void SaveComplate(bool isSaved)
		{
			if (isSaved)
			{
				SelectedData = string.IsNullOrEmpty(_savedF161601Clone.RTN_APPLY_NO) ? DgList.LastOrDefault()
																					 : DgList.FirstOrDefault(item => item.RTN_APPLY_NO == _savedF161601Clone.RTN_APPLY_NO);
				UserOperateMode = OperateMode.Query;
			}
		}
		#endregion Save

		#region Print
		public ICommand PrintCommand
		{
			get
			{
				return  CreateBusyAsyncCommand<PrintType>(
					o=> DoPrint(o),
					(t) => !IsBusy && SelectedData != null && UserOperateMode == OperateMode.Query);
			}
		}

		private void DoPrint(PrintType printType)
		{
			//只有未列印且沒有補印資料才更新
			if (SelectedData.STATUS == "0")
			{
				var message = new MessagesStruct
				{
					Button = DialogButton.YesNo,
					Image = DialogImage.Warning,
					Message = Properties.Resources.P1601020000_GeneratePrint_TransferApplicateNo,
					Title = Resources.Resources.Warning
				};
				if (ShowMessage(message) != DialogResponse.Yes)
					return;

				if (!UpdateIsPrinted())
					return;
			}
			//取得報表資料

			var proxyEx = GetExProxy<P16ExDataSource>();
			P160102Reports = proxyEx.CreateQuery<P160102Report>("GetP160102Reports")
											.AddQueryExOption("dcCode", SelectedData.DC_CODE)
											.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
											.AddQueryExOption("custCode", SelectedData.CUST_CODE)
											.AddQueryExOption("rtnApplyNo", SelectedData.RTN_APPLY_NO).ToList();

			if (!P160102Reports.Any())
			{
				ShowWarningMessage(Properties.Resources.P1601020000_NoDataToPrint);
				return;
			}

			if (SelectedData != null)
			{
				_savedF161601Clone = ExDataMapper.Clone(SelectedData);

				Clear();
				CRTDateS = CRTDateE = _savedF161601Clone.RTN_APPLY_DATE;
				IsSearch = true;
				IsSelectedData = false;
				DoSearch();
				SaveComplate(true);
				DispatcherAction(() =>
				{
					DoPrintReport(printType);
				});
			}
		}



		private bool UpdateIsPrinted()
		{
			var proxyEx = GetExProxy<P16ExDataSource>();
			var result = proxyEx.CreateQuery<Wms3pl.WpfClient.ExDataServices.P16ExDataService.ExecuteResult>("PrintP160102")
				.AddQueryExOption("dcCode", SelectedData.DC_CODE)
				.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
				.AddQueryExOption("custCode", SelectedData.CUST_CODE)
				.AddQueryExOption("rtnApplyNo", SelectedData.RTN_APPLY_NO).ToList().FirstOrDefault();

			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);

			return result.IsSuccessed;
		}
		#endregion


		#endregion
	}
}
