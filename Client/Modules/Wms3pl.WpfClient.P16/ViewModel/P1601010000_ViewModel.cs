using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P16WcfService;
using wcf70 = Wms3pl.WpfClient.ExDataServices.P70WcfService;
using Wms3pl.WpfClient.ExDataServices.P70ExDataService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1601010000_ViewModel : InputViewModelBase
	{
		public Action ExcelImport = delegate { };
		#region 共用變數/資料連結/頁面參數
		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		public string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
		private string _returnNo;
		private string _original_DISTR_CAR;
		public Action OpenSearchWin = delegate { };
		public Action<SelectionItem<F161201DetailDatas>> AddDetailAction = delegate { };
		public Action DelItemAction = delegate { };

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
				if (_selectDcCode != value)
				{
					DgList = null;
					DgItemList = null;
					SelectedData = null;
				}

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
		#region Form - 退貨單號
		private string _searchReturn_NO = string.Empty;
		public string SearchReturn_NO
		{
			get { return _searchReturn_NO; }
			set
			{
				_searchReturn_NO = value;
				RaisePropertyChanged("SearchReturn_NO");
			}
		}
		#endregion
		#region Form - 過帳日期起迄
		private DateTime? _postingDateS = DateTime.Today;
		public DateTime? PostingDateS
		{
			get { return _postingDateS; }
			set { _postingDateS = value; RaisePropertyChanged("PostingDateS"); }
		}
		private DateTime? _postingDateE = DateTime.Today;
		public DateTime? PostingDateE
		{
			get { return _postingDateE; }
			set { _postingDateE = value; RaisePropertyChanged("PostingDateE"); }
		}
		#endregion
		#region Form - 貨主單號
		private string _searchCUST_ORD_NO = string.Empty;
		public string SearchCUST_ORD_NO
		{
			get { return _searchCUST_ORD_NO; }
			set
			{
				_searchCUST_ORD_NO = value;
				RaisePropertyChanged("SearchCUST_ORD_NO");
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
		#region Form - 來源單號
		private string _searchSOURCE_NO = string.Empty;
		public string SearchSOURCE_NO
		{
			get { return _searchSOURCE_NO; }
			set
			{
				_searchSOURCE_NO = value;
				RaisePropertyChanged("SearchSOURCE_NO");
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
		#endregion

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

		#region Data - 資料List
		private List<F161201> _dgList;
		public List<F161201> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private F161201 _selectedData;

		public F161201 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
				IsSearch = _selectedData == null;
				IsSelectedData = _selectedData != null;
				GetDetailData(_selectedData);
				
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

		public void SetOrdpropList()
		{
			// 退貨單新增編輯模式可選作業類別,R1,2,3
			var proxy = GetProxy<F00Entities>();
			var query = from item in proxy.F000903s
						where item.ORD_PROP.StartsWith("R")
						select new NameValuePair<string>
						{
							Name = item.ORD_PROP_NAME,
							Value = item.ORD_PROP
						};
			OrdPropList = query.ToList();
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
		#region Data - 商品資料List
		private List<F161201DetailDatas> _dgItemList;
		public List<F161201DetailDatas> DgItemList
		{
			get { return _dgItemList; }
			set
			{
				_dgItemList = value;
				RaisePropertyChanged("DgItemList");
			}
		}
		#endregion

		#region Edit Data - 編輯資料欄位
		#region 作業類別
		private NameValuePair<string> _editableOrdProp;

		public NameValuePair<string> EditableOrdProp
		{
			get { return _editableOrdProp; }
			set
			{
				_editableOrdProp = value;
				RaisePropertyChanged("EditableOrdProp");

				if (UserOperateMode != OperateMode.Query && value != null)
				{
					CheckOrdProp(SelectEditData, value.Value);
					//RaisePropertyChanged("SelectEditData"); 
				}


			}
		}

		#endregion
		#region Edit Data - 編輯資料Data
		private F161201 _selectEditData;

		public F161201 SelectEditData
		{
			get { return _selectEditData; }
			set
			{
				_selectEditData = value;
				RaisePropertyChanged("SelectEditData");
			}
		}
		#endregion
		#region Edit Data - 編輯商品資料List
		private SelectionList<F161201DetailDatas> _dgEditItemList;
		public SelectionList<F161201DetailDatas> DgEditItemList
		{
			get { return _dgEditItemList; }
			set
			{
				_dgEditItemList = value;
				if (_dgEditItemList != null)
				{
					if (_dgEditItemList.Any())
						EditDetailCount = _dgEditItemList.Count;
				}
				RaisePropertyChanged("DgEditItemList");
			}
		}

		private bool _isSelectedAll = false;
		public bool IsSelectedAll
		{
			get { return _isSelectedAll; }
			set { _isSelectedAll = value; RaisePropertyChanged("IsSelectedAll"); }
		}

		private SelectionItem<F161201DetailDatas> _selectedEditItem;

		public SelectionItem<F161201DetailDatas> SelectedEditItem
		{
			get { return _selectedEditItem; }
			set
			{
				_selectedEditItem = value;
				RaisePropertyChanged("SelectedEditItem");
			}
		}
		#endregion
		#region Edit Data - 編輯商品明細資料
		private F161201DetailDatas _selectEditItem;

		public F161201DetailDatas SelectEditItem
		{
			get { return _selectEditItem; }
			set
			{
				_selectEditItem = value;
				RaisePropertyChanged("SelectEditItem");
			}
		}
		#endregion
		#region Edit Data - 總計明細筆數
		private int? _editDetailCount;
		public int? EditDetailCount
		{
			get { return _editDetailCount; }
			set
			{
				_editDetailCount = value;
				RaisePropertyChanged("EditDetailCount");
			}
		}
		#endregion
		#region Edit Data - 單據狀態
		private List<NameValuePair<string>> _editSTATUS;

		public List<NameValuePair<string>> EditSTATUS
		{
			get { return _editSTATUS; }
			set
			{
				_editSTATUS = value;
				RaisePropertyChanged("EditSTATUS");
			}
		}
		#endregion
		#region Edit Data - 勾選商品明細資料
		private List<F161201DetailDatas> _editItemGroup;
		public List<F161201DetailDatas> EditItemGroup
		{
			get { return _editItemGroup; }
			set
			{
				_editItemGroup = value;
				RaisePropertyChanged("EditItemGroup");
			}
		}
		#endregion
		#region Edit Data - 可否修改明細資料
		private bool _canEdit = false;
		public bool CanEdit
		{
			get { return _canEdit; }
			set
			{
				_canEdit = value;
				RaisePropertyChanged("CanEdit");
			}
		}
		#endregion

		private bool _hasRetailForEdit;

		public bool HasRetailForEdit
		{
			get { return _hasRetailForEdit; }
			set
			{
				Set(() => HasRetailForEdit, ref _hasRetailForEdit, value);
			}
		}

		#endregion

		#region AddNew Data - 新增資料欄位
		#region 作業類別
		private NameValuePair<string> _addOrdProp;

		public NameValuePair<string> AddOrdProp
		{
			get { return _addOrdProp; }
			set
			{
				_addOrdProp = value;
				RaisePropertyChanged("AddOrdProp");

				if (UserOperateMode != OperateMode.Query && value != null)
				{
					CheckOrdProp(AddNewData, value.Value);
					//RaisePropertyChanged("AddNewData"); 
				}

			}
		}


		#endregion
		#region Add Form - 出貨日期起迄
		private DateTime _asDEVL_DATES = DateTime.Today.AddDays(-10);
		public DateTime ASDEVL_DATES
		{
			get { return _asDEVL_DATES; }
			set { _asDEVL_DATES = value; RaisePropertyChanged("ASDEVL_DATES"); }
		}
		private DateTime _asDEVL_DATEE = DateTime.Today;
		public DateTime ASDEVL_DATEE
		{
			get { return _asDEVL_DATEE; }
			set { _asDEVL_DATEE = value; RaisePropertyChanged("ASDEVL_DATEE"); }
		}
		#endregion
		#region Add Form - 客戶代號
		private string _asCUST_NO;
		public string ASCUST_NO
		{
			get { return _asCUST_NO; }
			set
			{
				_asCUST_NO = value;
				RaisePropertyChanged("ASCUST_NO");
			}
		}
		#endregion
		#region Add Form - 客戶名稱
		private string _asCUST_NAME;
		public string ASCUST_NAME
		{
			get { return _asCUST_NAME; }
			set
			{
				_asCUST_NAME = value;
				RaisePropertyChanged("ASCUST_NAME");
			}
		}
        #endregion

        #region Add Form - 出貨單號
        private string _asWMS_ORD_NO;
        public string ASWMS_ORD_NO
        {
            get { return _asWMS_ORD_NO; }
            set
            {
                _asWMS_ORD_NO = value;
                RaisePropertyChanged("ASWMS_ORD_NO");
            }
        }
        #endregion

        #region Add Form - 貨主單號
        private string _asCUST_ORD_NO;
        public string ASCUST_ORD_NO
        {
            get { return _asCUST_ORD_NO; }
            set
            {
                _asCUST_ORD_NO = value;
                RaisePropertyChanged("ASCUST_ORD_NO");
            }
        }
        #endregion

        private bool _hasRetailForAdd;

		public bool HasRetailForAdd
		{
			get { return _hasRetailForAdd; }
			set
			{
				Set(() => HasRetailForAdd, ref _hasRetailForAdd, value);
			}
		}


		#region Add Form - 出貨資料List
		private List<CustomerData> _dgAddSearchList;
		public List<CustomerData> DgAddSearchList
		{
			get { return _dgAddSearchList; }
			set
			{
				_dgAddSearchList = value;
				RaisePropertyChanged("DgAddSearchList");
			}
		}

		private CustomerData _selF050801;
		public CustomerData SelF050801
		{
			get { return _selF050801; }
			set
			{
				_selF050801 = value;
				RaisePropertyChanged("SelF050801");
			}
		}
		#endregion

		#region Add Data - F161201Data
		private F161201 _addNewData;
		public F161201 AddNewData
		{
			get { return _addNewData; }
			set
			{
				_addNewData = value;
				RaisePropertyChanged("AddNewData");
			}
		}
		#endregion
		#region Add Data - 新增商品明細資料
		private F161201DetailDatas _selectAddItem;

		public F161201DetailDatas SelectAddItem
		{
			get { return _selectAddItem; }
			set
			{
				_selectAddItem = value;
				RaisePropertyChanged("SelectAddItem");
			}
		}
		#endregion
		#region Add Data - 新增商品資料List
		private SelectionList<F161201DetailDatas> _dgAddItemList;
		public SelectionList<F161201DetailDatas> DgAddItemList
		{
			get { return _dgAddItemList; }
			set
			{
				_dgAddItemList = value;
				RaisePropertyChanged("DgAddItemList");
			}
		}

		private SelectionItem<F161201DetailDatas> _selectedAddItem;

		public SelectionItem<F161201DetailDatas> SelectedAddItem
		{
			get { return _selectedAddItem; }
			set
			{
				_selectedAddItem = value;
				RaisePropertyChanged("SelectedAddItem");
			}
		}
		#endregion
		#region Add Data - 勾選商品明細資料
		private List<F161201DetailDatas> _addItemGroup;
		public List<F161201DetailDatas> AddItemGroup
		{
			get { return _addItemGroup; }
			set
			{
				_addItemGroup = value;
				RaisePropertyChanged("AddItemGroup");
			}
		}
		#endregion
		#endregion

		#region UI Layout Binding
		private bool _isHandling;

		/// <summary>
		/// 單據狀態是否為處理中
		/// </summary>
		public bool IsHandling
		{
			get { return _isHandling; }
			set
			{
				_isHandling = value;
				RaisePropertyChanged("IsHandling");
			}
		}

		/// <summary>
		/// 取得單據狀態是否為處理中，用來禁用編輯時的欄位，只能修改來源單號，在來源單號欄位輸入換貨單號
		/// </summary>
		/// <param name="isHandling"></param>
		public void SetHandlingStatus(bool isHandling)
		{
			IsHandling = isHandling && SelectedData.STATUS == "1";
		}


		private bool _rtnCustCodeIsReadOnly;

		public bool RtnCustCodeIsReadOnly
		{
			get { return _rtnCustCodeIsReadOnly; }
			set
			{
				_rtnCustCodeIsReadOnly = value;
				RaisePropertyChanged("RtnCustCodeIsReadOnly");
			}
		}

		public void RtnCustCodeTextChanged(string rtnCustCode)
		{
			if (UserOperateMode == OperateMode.Edit && IsHandling)
				return;

			// 當有輸入退貨客戶代號時，客戶名稱清空不可輸入
			RtnCustCodeIsReadOnly = !string.IsNullOrWhiteSpace(rtnCustCode) || IsHandling;

			if (RtnCustCodeIsReadOnly)
			{
				bool isAutoResearch = false;
				if (UserOperateMode == OperateMode.Add)
				{
					isAutoResearch = !string.IsNullOrEmpty(AddNewData.RTN_CUST_NAME);
					AddNewData.RTN_CUST_NAME = string.Empty;
				}
				else if (UserOperateMode == OperateMode.Edit)
				{
					isAutoResearch = !string.IsNullOrEmpty(SelectEditData.RTN_CUST_NAME);
					SelectEditData.RTN_CUST_NAME = string.Empty;
				}

				if (isAutoResearch)
				{
					GetCustCodeInfo(rtnCustCode, false);
				}
			}
		}

		public void GetCustCodeInfo(string rtnCustCode, bool isShowMsg = true)
		{
			if (UserOperateMode == OperateMode.Edit && IsHandling)
				return;

			if (UserOperateMode == OperateMode.Add)
			{
				AddNewData.RTN_CUST_NAME = GetRETAIL_NAME(rtnCustCode, isShowMsg);
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				SelectEditData.RTN_CUST_NAME = GetRETAIL_NAME(rtnCustCode, isShowMsg);
			}
		}

		#endregion

		#region 將匯入資料為請輸入改成清空資料

		private void ChangeSelectedData(F161201 selectedItem)
		{
			SelectedData.DC_CODE = selectedItem.DC_CODE == Properties.Resources.P1601010000_Input ? string.Empty : selectedItem.DC_CODE;
			SelectedData.RTN_CUST_NAME = selectedItem.RTN_CUST_NAME == Properties.Resources.P1601010000_Input ? string.Empty : selectedItem.RTN_CUST_NAME;
			SelectedData.RTN_TYPE_ID = selectedItem.RTN_TYPE_ID == Properties.Resources.P1601010000_Input ? string.Empty : selectedItem.RTN_TYPE_ID;
		
			SelectedData.RTN_CAUSE = selectedItem.RTN_CAUSE == Properties.Resources.P1601010000_Input ? string.Empty : selectedItem.RTN_CAUSE;
			SelectedData.ORD_PROP = selectedItem.ORD_PROP == Properties.Resources.P1601010000_Input ? string.Empty : selectedItem.ORD_PROP;
			SelectedData.ADDRESS = selectedItem.ADDRESS == Properties.Resources.P1601010000_Input ? string.Empty : selectedItem.ADDRESS;
		
			SelectedData.CONTACT = selectedItem.CONTACT == Properties.Resources.P1601010000_Input ? string.Empty : selectedItem.CONTACT;
			SelectedData.TEL = selectedItem.TEL == Properties.Resources.P1601010000_Input ? string.Empty : selectedItem.TEL;

		}
		#endregion
		#endregion

		private bool _caneditdistrcar;
		public bool CanEditDistrCar
		{
			get { return _caneditdistrcar; }
			set
			{
				_caneditdistrcar = value;
				RaisePropertyChanged("CanEditDistrCar");
			}
		}

		#region 函式
		public P1601010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			GetDcCodeList();
			SearchSTATUS = GetStatusList();
			SelectSTATUS = SearchSTATUS.FirstOrDefault().Value;
			EditSTATUS = GetStatusList();

			SetOrdpropList();
			SourceTypeList = GetSourceTypeList();
			RtnTypeList = GetRtnTypeList();
			RtnCauseList = GetRtnCauseList();
		}

		private void GetDcCodeList()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any()) SelectDcCode = DcCodes.FirstOrDefault().Value;
		}

		public string DcCodeToName(string strDcCode)
		{
			var dcCodeList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			return dcCodeList.Where(x => x.Value.Equals(strDcCode)).FirstOrDefault().Name;
		}

		private void GetDetailData(F161201 SelData)
		{
			DgItemList = null;
			if (SelData != null)
			{
				var proxy = GetProxy<F16Entities>();
				var qry = proxy.F161202s.Where(x => x.DC_CODE.Equals(SelData.DC_CODE)
																				 && x.GUP_CODE == SelData.GUP_CODE
																				 && x.CUST_CODE == SelData.CUST_CODE
																				 && x.RETURN_NO == SelData.RETURN_NO)
																.AsQueryable().ToList();
				if (qry != null && qry.Any())
					SelectDetailCount = qry.Count;
				else
				{
					ShowMessage(Messages.InfoNoData);
					return;
				}

				var proxyEx = GetExProxy<P16ExDataSource>();
				var detailqry = proxyEx.CreateQuery<F161201DetailDatas>("GetF161201DetailDatas")
											.AddQueryExOption("dcCode", SelData.DC_CODE)
											.AddQueryExOption("gupCode", SelData.GUP_CODE)
											.AddQueryExOption("custCode", SelData.CUST_CODE)
											.AddQueryExOption("returnNo", SelData.RETURN_NO)
											.AsQueryable();

				DgItemList = detailqry.ToList();

			}
		}

		public List<NameValuePair<string>> GetStatusList()
		{

			List<NameValuePair<string>> result = new List<NameValuePair<string>>();
            result = GetBaseTableService.GetF000904List(FunctionCode, "F161201", "STATUS", true);
        
            return result.ToList();
		}

		private List<NameValuePair<string>> _sourceTypeList;

		public List<NameValuePair<string>> SourceTypeList
		{
			get { return _sourceTypeList; }
			set
			{
				_sourceTypeList = value;
				RaisePropertyChanged("SourceTypeList");
			}
		}


		/// <summary>
		/// 來源單據Value,來源單據名稱Name
		/// </summary>
		/// <returns></returns>
		public List<NameValuePair<string>> GetSourceTypeList()
		{
			var proxy = GetProxy<F00Entities>();
			var qry = from a in proxy.F000902s
					  select new NameValuePair<string>()
					  {
						  Value = a.SOURCE_TYPE,
						  Name = a.SOURCE_NAME
					  };

			return qry.ToList();
		}

		private List<NameValuePair<string>> _rtnTypeList;

		public List<NameValuePair<string>> RtnTypeList
		{
			get { return _rtnTypeList; }
			set
			{
				_rtnTypeList = value;
				RaisePropertyChanged("RtnTypeList");
			}
		}


		/// <summary>
		/// 退貨類型,名稱
		/// </summary>
		/// <returns></returns>
		public List<NameValuePair<string>> GetRtnTypeList()
		{
			List<NameValuePair<string>> result = new List<NameValuePair<string>>();
			var proxy = GetProxy<F16Entities>();
			var qry = (from a in proxy.F161203s
					   select new NameValuePair<string>()
					   {
						   Value = a.RTN_TYPE_ID,
						   Name = a.RTN_TYPE_NAME
					   }).AsQueryable();
			if (qry.Count() > 0)
			{
				result = qry.ToList();
			}
			return result;
		}

		private List<NameValuePair<string>> _rtnCauseList;

		public List<NameValuePair<string>> RtnCauseList
		{
			get { return _rtnCauseList; }
			set
			{
				_rtnCauseList = value;
				RaisePropertyChanged("RtnCauseList");
			}
		}


		public List<NameValuePair<string>> GetRtnCauseList()
		{
			var proxy = GetProxy<F19Entities>();
			var qry = from a in proxy.F1951s
					  where a.UCT_ID == "RT"
					  select new NameValuePair<string>()
						 {
							 Value = a.UCC_CODE,
							 Name = a.CAUSE
						 };
			return qry.ToList();
		}

		public void GetAddSearchList()
		{
			if (string.IsNullOrWhiteSpace(AddNewData.DC_CODE))
			{
				DialogService.ShowMessage(Properties.Resources.P1601010000_ChooseDC);
				return;
			}

			ValidateHelper.AutoChangeBeginEnd(this, x => x.ASDEVL_DATES, x => x.ASDEVL_DATEE);

			var proxyEx = GetExProxy<P16ExDataSource>();
			var query = proxyEx.CreateQuery<CustomerData>("GetCustomerDatas")
										.AddQueryExOption("dcCode", AddNewData.DC_CODE)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
										.AddQueryExOption("beginDelvDate", ASDEVL_DATES)
										.AddQueryExOption("endDelvDate", ASDEVL_DATEE)
										.AddQueryExOption("retailCode", ASCUST_NO)
										.AddQueryExOption("custName", ASCUST_NAME)
                                        .AddQueryExOption("wmsOrdNo", ASWMS_ORD_NO)
                                        .AddQueryExOption("custOrdNo", ASCUST_ORD_NO);

			DgAddSearchList = query.ToList();
		}


		public string GetRETAIL_NAME(string strRetailCode, bool isShowMsg = true)
		{
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			var proxy = GetProxy<F19Entities>();
			var f1909 = proxy.F1909s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).FirstOrDefault();
			var filterCustCode = custCode;
			if (f1909 != null && f1909.ALLOWGUP_RETAILSHARE == "1")
				filterCustCode = "0";
			var qry = from a in proxy.F1910s
					  where a.GUP_CODE == gupCode
							 && a.RETAIL_CODE == strRetailCode
							 && a.CUST_CODE == filterCustCode
								select a;

			var result = qry.FirstOrDefault();
			if (result == null)
			{
				if (isShowMsg)
				{
					ShowMessage(Messages.InfoNoData);
				}

				return null;
			}
			else
			{
				return result.RETAIL_NAME;
			}
		}


		/// <summary>
		/// 取得已勾選的資料列
		/// </summary>
		/// <returns></returns>
		private List<F161201DetailDatas> GetEditItemGroup()
		{
			var sel = DgEditItemList.Where(x => x.IsSelected == true).ToList();
			var result = (from i in sel
						  select i.Item).ToList();
			return result;
		}

		private List<F161201DetailDatas> GetAddItemGroup()
		{
			var sel = DgAddItemList.Where(x => x.IsSelected == true).ToList();
			var result = (from i in sel
						  select i.Item).ToList();
			return result;
		}

		#endregion

		void CheckOrdProp(F161201 f161201, string newOrdProp)
		{
			if (newOrdProp != "R3")
			{
				f161201.SOURCE_TYPE = f161201.SOURCE_NO = string.Empty;
			}
		}
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
			if (UserOperateMode == OperateMode.Add)
			{
				foreach (var p in DgAddItemList)
				{
					p.IsSelected = IsSelectedAll;
				}
			}
			if (UserOperateMode == OperateMode.Edit)
			{
				foreach (var p in DgEditItemList)
				{
					p.IsSelected = IsSelectedAll;
				}
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
			//執行查詢動
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			MessagesStruct SearchDateMsg = new MessagesStruct();
			SearchDateMsg.Message = Properties.Resources.P1601010000_BeginDateLessThanEndDate;
			SearchDateMsg.Button = DialogButton.OK;
			SearchDateMsg.Image = DialogImage.Warning;
			SearchDateMsg.Title = Resources.Resources.Information;

			if (string.IsNullOrWhiteSpace(SelectDcCode))
			{
				ShowMessage(SearchDateMsg);
				return;
			}
			if (CRTDateS != null && CRTDateE != null)
			{
				if (CRTDateS > CRTDateE)
				{
					ShowMessage(Messages.WarningNoDcCode);
					return;
				}
			}
			if (PostingDateS != null && PostingDateE != null)
			{
				if (PostingDateS > PostingDateE)
				{
					ShowMessage(Messages.WarningNoDcCode);
					return;
				}
			}

			SearchReturn_NO = SearchReturn_NO.Trim();
			SearchCUST_ORD_NO = SearchCUST_ORD_NO.Trim();
			SearchSOURCE_NO = SearchSOURCE_NO.Trim();

			Search(SelectDcCode, gupCode, custCode, CRTDateS, CRTDateE,
				PostingDateS, PostingDateE, SearchReturn_NO, SearchCUST_ORD_NO, SearchSOURCE_NO);

			if (!DgList.Any())
			{
				ShowMessage(Messages.InfoNoData);
			}
			SelectedData = null;

		}

		/// <summary>
		/// 根據畫面查詢條件來搜尋查詢結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="startCreateDate"></param>
		/// <param name="endCreateDate"></param>
		/// <param name="startPostingDate"></param>
		/// <param name="endPostingDate"></param>
		/// <param name="custOrdNo"></param>
		/// <param name="sourceNo"></param>
		void Search(string dcCode, string gupCode, string custCode,
					DateTime? startCreateDate, DateTime? endCreateDate,
					DateTime? startPostingDate, DateTime? endPostingDate,
					string returnNo, string custOrdNo, string sourceNo)
		{
			var proxy = GetProxy<F16Entities>();
			var query = from item in proxy.F161201s
						where item.DC_CODE == dcCode
						where item.GUP_CODE == gupCode
						where item.CUST_CODE == custCode
						select item;

			if (startCreateDate.HasValue && endCreateDate.HasValue)
			{
				endCreateDate = endCreateDate.Value.AddDays(1);
				query = query.Where(item => startCreateDate <= item.CRT_DATE && item.CRT_DATE < endCreateDate);
			}

			if (startPostingDate.HasValue && endPostingDate.HasValue)
			{
				endPostingDate = endPostingDate.Value.AddDays(1);
				query = query.Where(item => item.POSTING_DATE == null || (startPostingDate <= item.POSTING_DATE && item.POSTING_DATE < endPostingDate));
			}

			if (!string.IsNullOrWhiteSpace(returnNo))
			{
				query = query.Where(item => item.RETURN_NO == returnNo);
			}

			if (!string.IsNullOrWhiteSpace(custOrdNo))
			{
				query = query.Where(item => item.CUST_ORD_NO == custOrdNo);
			}

			if (!string.IsNullOrWhiteSpace(sourceNo))
			{
				query = query.Where(item => item.SOURCE_NO == sourceNo);
			}

			if (!string.IsNullOrWhiteSpace(SelectSTATUS))
			{
				query = query.Where(x => x.STATUS == SelectSTATUS);
			}
			else
			{
				query = query.Where(x => x.STATUS != "9");
			}

			DgList = query.OrderBy(item => item.RETURN_NO).ToList();
		}

		void Search(F161201 f161201)
		{
			if (f161201.CRT_DATE == default(DateTime))
			{
				f161201.CRT_DATE = DateTime.Now;
			}

			Search(f161201.DC_CODE, f161201.GUP_CODE, f161201.CUST_CODE, f161201.CRT_DATE.Date, f161201.CRT_DATE.Date,
				   f161201.POSTING_DATE, f161201.POSTING_DATE, f161201.RETURN_NO, f161201.CUST_ORD_NO, f161201.SOURCE_NO);
		}
		#endregion Search

		#region SearchItem
		private bool _hasAddSearchItemData;
		public bool HasAddSearchItemData
		{
			get { return _hasAddSearchItemData; }
			set
			{
				_hasAddSearchItemData = value;
				RaisePropertyChanged("HasAddSearchItemData");
			}
		}
		private bool _hasEditSearchItemData;
		public bool HasEditSearchItemData
		{
			get { return _hasEditSearchItemData; }
			set { _hasEditSearchItemData = value; RaisePropertyChanged("HasEditSearchItemData"); }
		}
		#endregion SearchItem

		#region GetAddItem
		public ICommand GetAddItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => GetAddSearchList()
					);
			}
		}
		#endregion

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query, o => AddComplate()
					);
			}
		}

		private void DoAdd()
		{
			RtnCustCodeIsReadOnly = false;

			AddNewData = new F161201();
			DgAddSearchList = new List<CustomerData>();
			DgAddItemList = new SelectionList<F161201DetailDatas>(new List<F161201DetailDatas>());

			AddNewData.RETURN_DATE = DateTime.Today;
			AddNewData.GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode;
			AddNewData.CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode;
			AddNewData.STATUS = "0";
			AddNewData.DISTR_CAR = "0";
			SelectAddItem = new F161201DetailDatas();
			//執行新增動作
		}

		private void AddComplate()
		{
			//GetAddSearchList();
			UserOperateMode = OperateMode.Add;
		}
		#endregion Add


		#region ImportExcel
		public ICommand ImportCommand
		{
            get
            {
                return new RelayCommand(() => {
                    DispatcherAction(() => {
                        ExcelImport();
                        if (string.IsNullOrEmpty(ImportFilePath)) return;
                        DoImportCommand.Execute(null);
                    });
                });
            }
   //         get
			//{
			//	return CreateBusyAsyncCommand(
			//		o => Import(), () => UserOperateMode == OperateMode.Query
			//		);
			//}
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
			//ExcelImport();
			//if (string.IsNullOrEmpty(ImportFilePath)) return;
			
			string fullFilePath = ImportFilePath;
			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);

            var proxy = new wcf.P16WcfServiceClient();
            var result = new wcf.ExecuteResult();

            if (excelTable == null)
            {
                DialogService.ShowMessage("請檢查匯入檔案是否正確或開啟中!");
                return;
            }

            if (excelTable.Columns.Count < 24)
            {
                DialogService.ShowMessage("退貨單匯入檔必須為Excel檔案，總共有24欄");
                return;
            }

            if (excelTable.Columns.Count == 15) //Hiiir格式
            {
                var queryData = (from col in excelTable.AsEnumerable()
                                 select new F1612ImportData
                                 {
                                     GUP_CODE = string.Empty,
                                     CUST_CODE = string.Empty,
                                     ORD_PROP = "R1",
                                     COST_CENTER = string.Empty,
                                     RETAIL_CODE = string.Empty,
                                     CONTACT = Convert.ToString(col[8]),
                                     TEL = Convert.ToString(col[9]),
                                     ADDRESS = Convert.ToString(col[10]),
                                     DISTR_CAR = "Y",
                                     MEMO = Convert.ToString(col[13]),
                                     CUST_ORD_NO = Convert.ToString(col[3]),
                                     ITEM_CODE = Convert.ToString(col[11]),
                                     RTN_QTY = 1,
                                     RTN_TYPE_ID = "2",
                                     RTN_CAUSE = "999"
                                 }).ToList();


                var importData = ExDataMapper.MapCollection<F1612ImportData, wcf.F1612ImportData>(queryData).ToArray();
                result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                                                        () => proxy.ImportF1612ForHiiir(SelectDcCode, _gupCode, _custCode
                                                                                    , fullFilePath, importData));
            }
            else //標準格式
            {
                var queryData = (from col in excelTable.AsEnumerable()
                                 select new F1612ImportData
                                 {
                                     GUP_CODE = Convert.ToString(col[0]),
                                     CUST_CODE = Convert.ToString(col[1]),
                                     ORD_PROP = Convert.ToString(col[2]),
                                     COST_CENTER = Convert.ToString(col[3]),
                                     RETAIL_CODE = Convert.ToString(col[5]),
                                     CONTACT = Convert.ToString(col[6]),
                                     TEL = Convert.ToString(col[7]),
                                     ADDRESS = Convert.ToString(col[8]),
                                     DISTR_CAR = Convert.ToString(col[9]),
                                     MEMO = Convert.ToString(col[11]),
                                     CUST_ORD_NO = Convert.ToString(col[12]),
                                     ITEM_CODE = Convert.ToString(col[14]),
                                     RTN_QTY = Convert.ToInt16(col[16]),
                                     RTN_TYPE_ID = Convert.ToString(col[22]),
                                     RTN_CAUSE = Convert.ToString(col[23]),
                                 }).ToList();

                
                var importData = ExDataMapper.MapCollection<F1612ImportData, wcf.F1612ImportData>(queryData).ToArray();
                result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                                                        () => proxy.ImportF1612Data(SelectDcCode, _gupCode, _custCode
                                                                                    , fullFilePath, importData));
            }
			

			DialogService.ShowMessage(result.Message.ToString());


		}
	
		#endregion ImportExcel


		public ICommand ImportWmsOrdDetailsCommand
		{
			get
			{
				IList<F161201DetailDatas> wmsOrdItems = null;
				return CreateBusyAsyncCommand(
					o =>
					{
						var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
						var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
						wmsOrdItems = GetItemsByWmsOrdNo(SelF050801.DC_CODE, gupCode, custCode, SelF050801.WMS_ORD_NO);
					},
					() => SelF050801 != null,
					o => ImportWmsOrdDetailsCompleted(wmsOrdItems)
					);
			}
		}


		public void ImportWmsOrdDetailsCompleted(IList<F161201DetailDatas> wmsOrdItems)
		{
			if (wmsOrdItems == null)
				return;

			// 將要匯入的項目與已存在的項目做差異比較，在將不重複的項目新增到明細中
			var compare1 = wmsOrdItems.Select(item => item.ITEM_CODE);
			var compare2 = DgAddItemList.Select(si => si.Item.ITEM_CODE);
			var noRepeatWmsOrdItems = compare1.Except(compare2).Select(itemCode => wmsOrdItems.FirstOrDefault(wmsOrdItem => wmsOrdItem.ITEM_CODE == itemCode));
			foreach (var wmsOrdItem in noRepeatWmsOrdItems)
			{
				DgAddItemList.Add(new SelectionItem<F161201DetailDatas>(wmsOrdItem) { IsSelected = true });
			}

			if (DgAddItemList.Any())
			{
				AddDetailAction(DgAddItemList.LastOrDefault());
			}

			// 將出貨單號帶入右方的原出貨單號輸入框
			AddNewData.WMS_ORD_NO = SelF050801.WMS_ORD_NO;
			AddNewData.RTN_CUST_CODE = SelF050801.RETAIL_CODE;
			AddNewData.RTN_CUST_NAME = SelF050801.CUST_NAME;
			AddNewData.ADDRESS = SelF050801.ADDRESS;
			AddNewData.TEL = SelF050801.CONTACT_TEL;
			AddNewData.CONTACT = SelF050801.CONTACT;

		}

		/// <summary>
		/// 新增退貨單時，可用出貨單號來匯入出貨單的 Item
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		public IList<F161201DetailDatas> GetItemsByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var proxyEx = GetExProxy<P16ExDataSource>();
			var query = proxyEx.CreateQuery<F161201DetailDatas>("GetItemsByWmsOrdNo")
										.AddQueryExOption("dcCode", dcCode)
										.AddQueryExOption("gupCode", gupCode)
										.AddQueryExOption("custCode", custCode)
										.AddQueryExOption("wmsOrdNo", wmsOrdNo);
			var list = query.ToList();
			return list;
		}

		#region AddDetail
		public ICommand AddDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddDetail(),
					() => (UserOperateMode == OperateMode.Edit && SelectEditItem != null && SelectEditItem.RTN_QTY > 0 && SelectEditItem.ITEM_CODE != null)
						|| (UserOperateMode == OperateMode.Add && HasAddSearchItemData && SelectAddItem != null && SelectAddItem.RTN_QTY > 0 && SelectAddItem.ITEM_CODE != null)
					, o => AddDetailComplate()
					);
			}
		}



		private void DoAddDetail()
		{
			//執行新增動作
		}

		private void AddDetailComplate()
		{
			if (UserOperateMode == OperateMode.Add)
			{
				if (SelectAddItem != null && !string.IsNullOrWhiteSpace(SelectAddItem.ITEM_CODE))
				{
					// 若已存在該品項編號，則只會將畫面帶到該品項上，並且若尚未填退貨數量，則也會一併更新
					var existsItem = DgAddItemList.Where(si => si.Item.ITEM_CODE == SelectAddItem.ITEM_CODE).FirstOrDefault();
					if (existsItem != null)
					{
						if (existsItem.Item.RTN_QTY == 0)
						{
							existsItem.Item.RTN_QTY = SelectAddItem.RTN_QTY;
						}
						else
						{
							DialogService.ShowMessage(string.Format(Properties.Resources.P1601010000_ItemCodeExist, existsItem.Item.ITEM_CODE));
						}
						AddDetailAction(existsItem);

					}
					else
					{
						// 尚未存在品號則新增明細
						var newSelectedItem = new SelectionItem<F161201DetailDatas>(SelectAddItem) { IsSelected = true };
						DgAddItemList.Add(newSelectedItem);
						AddDetailAction(newSelectedItem);
					}

					SelectAddItem = new F161201DetailDatas();
				}
			}

			// 會重複寫 Code 是因為 UI 沒拆開，屬性又分開命名，只能這樣改...>"<
			if (UserOperateMode == OperateMode.Edit)
			{
				if (SelectEditItem != null && !string.IsNullOrWhiteSpace(SelectEditItem.ITEM_CODE))
				{
					// 若已存在該品項編號，則只會將畫面帶到該品項上，並且若尚未填退貨數量，則也會一併更新
					var existsItem = DgEditItemList.Where(si => si.Item.ITEM_CODE == SelectEditItem.ITEM_CODE).FirstOrDefault();
					if (existsItem != null)
					{
						if (existsItem.Item.RTN_QTY == 0)
						{
							existsItem.Item.RTN_QTY = SelectEditItem.RTN_QTY;
						}
						else
						{
							DialogService.ShowMessage(string.Format(Properties.Resources.P1601010000_ItemCodeExist, existsItem.Item.ITEM_CODE));
						}
						AddDetailAction(existsItem);
					}
					else
					{
						// 尚未存在品號則新增明細
						var newSelectedItem = new SelectionItem<F161201DetailDatas>(SelectEditItem) { IsSelected = true };
						DgEditItemList.Add(newSelectedItem);
						AddDetailAction(newSelectedItem);
					}

					SelectEditItem = new F161201DetailDatas();
				}
			}
		}
		#endregion AddDetail

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query
						&& SelectedData != null
						//單據狀態為”待處理”、”已處理”可編輯
						//&& SelectedData.STATUS != "1"
						&& SelectedData.STATUS != "2"
						&& SelectedData.STATUS != "9"
						&& IsCanEditOrDelete()
					);
			}
		}

		bool IsCanEditOrDelete()
		{
			// 若為退貨類型R3，或沒有填來源單號才可編輯或刪除
			return SelectedData.ORD_PROP == "R3" || string.IsNullOrEmpty(SelectedData.SOURCE_NO);
		}

		private void DoEdit()
		{
			//執行編輯動作

			if (SelectedData != null)
			{
				ChangeSelectedData(SelectedData);
				_original_DISTR_CAR = SelectedData.DISTR_CAR;
				RtnCustCodeIsReadOnly = !string.IsNullOrEmpty(SelectedData.RTN_CUST_CODE);

				if (SelectedData.STATUS == "1")
					CanEdit = true;
				else
					CanEdit = false;

				var proxyP70 = GetExProxy<P70ExDataSource>();
				var f700101 = proxyP70.CreateQuery<F700101EX>("GetF700101ByDistrCarNo")
											.AddQueryExOption("distrCarNo",SelectedData.DISTR_CAR_NO)
											.AddQueryExOption("dcCode", SelectedData.DC_CODE)
											.ToList();

				if (!f700101.Any())
					CanEditDistrCar = true;
				else
				{
					if (f700101.First().STATUS == "2")
						CanEditDistrCar = true;
					else
						CanEditDistrCar = f700101.First().STATUS == "9";
				}

				SelectEditData = ExDataMapper.Map<F161201, F161201>(SelectedData);

				var cloneDetails = DgItemList.Select(item => ExDataMapper.Map<F161201DetailDatas, F161201DetailDatas>(item));
				DgEditItemList = new SelectionList<F161201DetailDatas>(cloneDetails);
				SelectEditItem = new F161201DetailDatas();
			
			}
			UserOperateMode = OperateMode.Edit;
			SetHandlingStatus(true);
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() => UserOperateMode != OperateMode.Query,
					o => DoCancelCompleted()
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				if (UserOperateMode == OperateMode.Add)
				{
					AddNewData = null;
					ASDEVL_DATES = DateTime.Today.AddDays(-10);
					ASDEVL_DATEE = DateTime.Today;
					DgAddSearchList = new List<CustomerData>();
				}
				else
				{
					SelectEditData = null;
					DgEditItemList = null;
				}
				SetHandlingStatus(false);
				UserOperateMode = OperateMode.Query;
			}
		}

		private void DoCancelCompleted()
		{

		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query
								&& SelectedData != null
								&& SelectedData.STATUS == "0"
								&& IsCanEditOrDelete()
					);
			}
		}

		private void DoDelete()
		{
			if (SelectedData != null)
			{
				// 確認是否要刪除
				if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

				//執行刪除動作
				//只能狀態為0的才可刪除
				var proxyEx = GetExProxy<P16ExDataSource>();
				var result = proxyEx.CreateQuery<Wms3pl.WpfClient.ExDataServices.P16ExDataService.ExecuteResult>("DeleteP160101")
					.AddQueryExOption("returnNo", SelectedData.RETURN_NO)
					.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
					.AddQueryExOption("custCode", SelectedData.CUST_CODE)
					.AddQueryExOption("dcCode", SelectedData.DC_CODE).FirstOrDefault();

				if (result != null && !result.IsSuccessed)
					ShowWarningMessage(result.Message);

				SelectedData = null;
				UserOperateMode = OperateMode.Query;
				DoSearch();
			}
		}
		#endregion Delete

		#region DeleteDetail
		public ICommand DeleteDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDeleteDetail(), () => UserOperateMode != OperateMode.Query
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
			if (UserOperateMode == OperateMode.Add)
			{
				AddItemGroup = GetAddItemGroup();
				if (AddItemGroup != null && AddItemGroup.Any())
				{
					// 確認是否要刪除
					if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
					//執行刪除動作
					DgAddItemList = new SelectionList<F161201DetailDatas>(DgAddItemList.Where(x => !x.IsSelected).Select(x => x.Item));

					DelItemAction();
					UserOperateMode = OperateMode.Add;
				}
			}
			if (UserOperateMode == OperateMode.Edit)
			{
				EditItemGroup = GetEditItemGroup();
				if (EditItemGroup != null && EditItemGroup.Any())
				{
					// 確認是否要刪除
					if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
					//執行刪除動作
					DgEditItemList = new SelectionList<F161201DetailDatas>(DgEditItemList.Where(x => x.IsSelected == false).Select(x => x.Item));

					int rowNum = 0;
					foreach (var item in DgEditItemList.Select(x => x.Item))
					{
						item.ROWNUM = ++rowNum;
					}

					RaisePropertyChanged("DgEditItemList");
					DelItemAction();
					UserOperateMode = OperateMode.Edit;
				}
			}
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
					() => (UserOperateMode == OperateMode.Add) ? UserOperateMode != OperateMode.Query && DgAddItemList != null && DgAddItemList.Any()
															   : UserOperateMode != OperateMode.Query && DgEditItemList != null && DgEditItemList.Any(),
					o => SaveComplate(isSaved)
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			if (UserOperateMode == OperateMode.Add)
				return DoAddSave();
			if (UserOperateMode == OperateMode.Edit)
				return DoEditSave();

			return false;
		}

		private bool DoAddSave()
		{
			if (AddNewData == null || DgAddItemList == null)
				return false;

			if (!DgAddItemList.Any())
			{
				MessagesStruct alertMessage = new MessagesStruct();
				alertMessage.Button = DialogButton.OK;
				alertMessage.Title = Resources.Resources.Information;
				alertMessage.Image = DialogImage.Warning;
				alertMessage.Message = "未勾選退貨明細!";
				ShowMessage(alertMessage);
				return false;
			}

			var error = GetEditableError(AddNewData);
			if (!string.IsNullOrEmpty(error))
			{
				DialogService.ShowMessage(error);
				return false;
			}

			var detailError = GetEditableDetailsError(DgAddItemList.Select(si => si.Item));
			if (!string.IsNullOrEmpty(detailError))
			{
				DialogService.ShowMessage(detailError);
				return false;
			}

			var f161201 = ExDataMapper.Map<F161201, wcf.F161201>(AddNewData);
			var f161202s = ExDataMapper.MapCollection<F161201DetailDatas, wcf.F161202>(DgAddItemList.Select(si => si.Item)).ToArray();

			var proxy = new wcf.P16WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
													() => proxy.InsertP160101(f161201, f161202s));

			if (result.IsSuccessed)
			{
				DialogService.ShowMessage(string.Format("退貨單號：{0} 新增成功", result.Message));
				_returnNo = result.Message;
				Search(AddNewData);
			}
			else
				DialogService.ShowMessage(result.Message);


			return result.IsSuccessed;
		}

		private bool DoEditSave()
		{
			if (SelectEditData == null || DgEditItemList == null)
				return false;

			if (!DgEditItemList.Any())
			{
				MessagesStruct alertMessage = new MessagesStruct();
				alertMessage.Button = DialogButton.OK;
				alertMessage.Title = Resources.Resources.Information;
				alertMessage.Image = DialogImage.Warning;
				alertMessage.Message = "未勾選退貨明細!";
				ShowMessage(alertMessage);
				return false;
			}

			var error = GetEditableError(SelectEditData);
			if (!string.IsNullOrEmpty(error))
			{
				DialogService.ShowMessage(error);
				return false;
			}

			var detailError = GetEditableDetailsError(DgEditItemList.Select(si => si.Item));
			if (!string.IsNullOrEmpty(detailError))
			{
				DialogService.ShowMessage(detailError);
				return false;
			}

			var f161201 = ExDataMapper.Map<F161201, wcf.F161201>(SelectEditData);
			var f161202s = ExDataMapper.MapCollection<F161201DetailDatas, wcf.F161202>(DgEditItemList.Select(si => si.Item)).ToArray();

			var proxy = new wcf.P16WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
													() => proxy.UpdateP160101(f161201, f161202s));

			if (result.IsSuccessed)
			{
				DialogService.ShowMessage(string.Format("退貨單號：{0} 更新成功", SelectEditData.RETURN_NO));
				_returnNo = SelectEditData.RETURN_NO;
                SelectSTATUS = "";

                Search(SelectEditData);
			}
			else
				DialogService.ShowMessage(result.Message);

			return result.IsSuccessed;
		}

		private string GetEditableError(F161201 f161201)
		{
			ExDataMapper.Trim(f161201);

			if (string.IsNullOrWhiteSpace(f161201.DC_CODE))
			{
				return Properties.Resources.P1602010000xamlcs_ChooseDC;
			}
			if (string.IsNullOrWhiteSpace(f161201.CUST_CODE))
			{
				ShowMessage(Messages.WarningNoCustCode);
				return "請選擇業主";
			}
			if (string.IsNullOrWhiteSpace(f161201.GUP_CODE))
			{
				ShowMessage(Messages.WarningNoGupCode);
				return "請選擇貨主";
			}

			if (!string.IsNullOrEmpty(f161201.CUST_ORD_NO))
			{
				if (f161201.CUST_ORD_NO.Length > 20)
					return "貨主單號長度不可超過20";

				//if (!ValidateHelper.IsMatchAZaz09(f161201.CUST_ORD_NO))
				//	return "貨主單號只能輸入英數";
			}

			if (!string.IsNullOrEmpty(f161201.WMS_ORD_NO))
			{
				if (f161201.WMS_ORD_NO.Length > 20)
					return "出貨單號長度不可超過20";

				if (!ValidateHelper.IsMatchAZaz09(f161201.WMS_ORD_NO))
					return "出貨單號只能輸入英數";
			}

			if (!string.IsNullOrEmpty(f161201.RTN_CUST_CODE))
			{
				if (f161201.RTN_CUST_CODE.Length > 20)
					return "退貨客戶代號長度不可超過20";

				if (!ValidateHelper.IsMatchAZaz09(f161201.RTN_CUST_CODE))
					return "退貨客戶代號只能輸入英數";
			}

			if (f161201.ORD_PROP.Equals("R2", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrEmpty(f161201.RTN_CUST_CODE))
					return "作業類別為R2門市/關店退貨，退貨客戶代號則必填!";
			}

			if (!string.IsNullOrEmpty(f161201.RTN_CUST_CODE))
			{
				GetCustCodeInfo(f161201.RTN_CUST_CODE);
				if (f161201.RTN_CUST_NAME == null)
				{
					return "退貨客戶代碼不存在";
				}
			}

			if (string.IsNullOrWhiteSpace(f161201.RTN_CUST_NAME))
				return "未輸入退貨客戶名稱!";

			if (string.IsNullOrWhiteSpace(f161201.RTN_TYPE_ID))
				return "未選擇退貨類型!";

			if (string.IsNullOrWhiteSpace(f161201.RTN_CAUSE))
				return "未選擇退貨原因!";

			if (f161201.RTN_CUST_NAME.Length > 200)
				return "退貨客戶名稱長度不可超過200";

			if (!string.IsNullOrEmpty(f161201.SOURCE_NO))
			{
				if (f161201.SOURCE_NO.Length > 20)
					return "來源單號長度不可超過20";

				if (!ValidateHelper.IsMatchAZaz09(f161201.SOURCE_NO))
					return "來源單號只能輸入英數";
			}

			if (!string.IsNullOrEmpty(f161201.COST_CENTER) && f161201.COST_CENTER.Length > 50)
				return "成本中心長度不可超過50";

			if (string.IsNullOrWhiteSpace(f161201.ADDRESS))
				return "未輸入聯絡地址!";

			if (f161201.ADDRESS.Length > 200)
				return "地址長度不可超過200";

			if (string.IsNullOrEmpty(f161201.CONTACT))
				return "未輸入聯絡人!";

			if (f161201.CONTACT.Length > 20)
				return "聯絡人長度不可超過20";

			if (string.IsNullOrWhiteSpace(f161201.TEL))
				return "未輸入聯絡電話!";

			if (f161201.TEL.Length > 20)
				return "電話長度不可超過20";

			//if (!ValidateHelper.IsMatchPhone(f161201.TEL))
			//	return "電話格式錯誤";

			if (!string.IsNullOrEmpty(f161201.MEMO) && f161201.MEMO.Length > 300)
				return "備註長度不可超過300";

			if (!string.IsNullOrEmpty(f161201.WMS_ORD_NO) && !ExistsWmsOrdNo(f161201))
				return "貨主原出貨單號不存在";

			var changeTypeError = GetChangeTypeError(f161201);
			if (!string.IsNullOrEmpty(changeTypeError))
				return changeTypeError;

			CheckOrdProp(f161201, f161201.ORD_PROP);

			return string.Empty;
		}

		bool ExistsWmsOrdNo(F161201 f161201)
		{
			var proxy = GetProxy<F05Entities>();
			var query = from item in proxy.F050801s
						where item.WMS_ORD_NO == f161201.WMS_ORD_NO
						where item.DC_CODE == f161201.DC_CODE
						where item.GUP_CODE == f161201.GUP_CODE
						where item.CUST_CODE == f161201.CUST_CODE
						select item;

			return query.FirstOrDefault() != null;
		}

		string GetChangeTypeError(F161201 f161201)
		{
            //f161201.ORD_PROP == "R3" 已無此類型
            return string.Empty;
		}

		public void CheckSourceNo(F161201 f161201)
		{
			f161201.SOURCE_TYPE = string.Empty;

			var changeTypeError = GetChangeTypeError(f161201);
			if (!string.IsNullOrEmpty(changeTypeError))
				DialogService.ShowMessage(changeTypeError);
		}

		private string GetEditableDetailsError(IEnumerable<F161201DetailDatas> f161201Details)
		{
			foreach (var item in f161201Details)
			{
				if (item.RTN_QTY <= 0)
				{
					return "退貨數量不可小於等於 0";
				}
			}

			return string.Empty;
		}

		private void SaveComplate(bool isSaved)
		{
			if (isSaved)
			{
				if (UserOperateMode == OperateMode.Add)
				{
					SelectedData = DgList.LastOrDefault();
					_original_DISTR_CAR = "";
				}
				else
				{
					SelectedData = DgList.Where(item => item.RETURN_NO == SelectEditData.RETURN_NO).FirstOrDefault();
				}

				if (SelectedData != null)
					SetHandlingStatus(false);
				var orginalOpMode = UserOperateMode;
				UserOperateMode = OperateMode.Query;

				if (SelectedData.DISTR_CAR == "1" && _original_DISTR_CAR != "1")
				{
					if (ShowMessage(new MessagesStruct()
						{
							Message = Properties.Resources.P1605010000_ManuallyDelv,
							Button = DialogButton.YesNo,
							Title = Properties.Resources.P1605010000_Hint,
							Image = DialogImage.Question

						}) == DialogResponse.Yes)
					{
						var function = FormService.GetFunctionFromSession("P7001040000");
						if (function == null)
						{
							DialogService.ShowMessage(Properties.Resources.P1605010000_NoPermission);
						}
					}
				}
				else if (SelectedData.DISTR_CAR == "0")
				{
					if (orginalOpMode == OperateMode.Edit && !string.IsNullOrEmpty(SelectEditData.DISTR_CAR_NO))
					{
						var wcf70Proxy = new wcf70.P70WcfServiceClient();
						var result = RunWcfMethod<wcf70.ExecuteResult>(wcf70Proxy.InnerChannel, () => wcf70Proxy.DeleteF700101ByDistrCarNo(SelectEditData.DISTR_CAR_NO, SelectEditData.DC_CODE));
					}
				}
			}

		}
		#endregion Save
		#endregion
	}
}
