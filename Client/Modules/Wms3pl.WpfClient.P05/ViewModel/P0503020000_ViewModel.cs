using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P05WcfService;
using wcf70 = Wms3pl.WpfClient.ExDataServices.P70WcfService;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0503020000_ViewModel : InputViewModelBase
	{
		private string _userId;
		private string _userName;
		private string _gupCode;
		public string _custCode;
		private string _original_DISTR_CAR;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };
		public Action DeleteAction = delegate { };
		public Action CancelAction = delegate { };
		public Action CollapsedQryResultAction = delegate { };
		public Action<string> OnSearchRetailCodeForEdit = (retailCode) => { };
		public bool isSave;
		public bool continueSave;
		public List<serialNoField> serialNo_Data;
		public Action ExcelImport = delegate { };

		public P0503020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				InitControls();
			}
		}

		private void InitControls()
		{
			GET_DC_LIST();
			GetDelvTypeList();
			GET_CHANNEL_LIST();
			GET_SUBCHANNEL_LIST();
			GET_COLLECT_LIST();
			GET_GENDER_LIST();
			GET_ORD_TYPE_LIST();
			GET_POSM_LIST();
			GET_PRINT_RECEIPT_LIST();
			GET_SP_DELV_LIST();
			GET_STATUS_LIST();
			GET_SA_LIST();
			GET_CUST_COST_LIST();
			GET_MOVE_OUT_TARGET_LIST();
			GET_PACKING_TYPE_LIST();
			TEMP_ORD_NO = "";
			STATUS = "A";
			CanEditDetailQty = "1";
			gridQtyReadonly = false;
			CanEditDelvData = true;
			LOCK_NOW = "0";
			CopyEditType = new SendTypeData();
		}

		#region 檔案上傳 file
		private string _fullPath;
		public string FullPath { get { return _fullPath; } set { _fullPath = value; } }
		private string _filePath;
		public string FilePath { get { return _filePath; } set { _filePath = value; } }
		private string _fileName;
		public string FileName { get { return _fileName; } set { _fileName = value; } }
		#endregion 檔案上傳 file

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

		private NameValuePair<string> _editableDcItem;

		public NameValuePair<string> EditableDcItem
		{
			get { return _editableDcItem; }
			set
			{
				_editableDcItem = value;
				RaisePropertyChanged("EditableDcItem");

				if (value == null)
					return;

				GET_ALLID_LIST(value.Value);
			}
		}

		public string CUST_CODE
		{
			get { return _custCode; }
		}

		private bool _canviewSeria;

		public bool CanViewSeria
		{
			get { return _canviewSeria; }
			set { _canviewSeria = value; RaisePropertyChanged("CanViewSeria"); }
		}

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

		private string _caneditdetailqty;

		public string CanEditDetailQty
		{
			get { return _caneditdetailqty; }
			set
			{
				_caneditdetailqty = value;
				RaisePropertyChanged("CanEditDetailQty");
			}
		}

		private bool _gridQtyReadonly;

		public bool gridQtyReadonly
		{
			get { return _gridQtyReadonly; }
			set { _gridQtyReadonly = value; RaisePropertyChanged("gridQtyReadonly"); }
		}

		private string _viewapprove;

		public string ViewApprove
		{
			get { return _viewapprove; }
			set { _viewapprove = value; RaisePropertyChanged("ViewApprove"); }
		}

		private bool _canimportserialno;

		public bool CanImportSerialNo
		{
			get { return _canimportserialno; }
			set
			{
				_canimportserialno = value;
				RaisePropertyChanged("CanImportSerialNo");
			}
		}

		private string _viewTran;

		public string VIEWTRAN
		{
			get { return _viewTran; }
			set
			{
				_viewTran = value;
				RaisePropertyChanged("VIEWTRAN");
			}
		}

		private string _orddetaildesc;

		public string ORDDETAILDESC
		{
			get { return _orddetaildesc; }
			set
			{
				_orddetaildesc = value;
				RaisePropertyChanged("ORDDETAILDESC");
			}
		}

		private bool _caneditdelvdata;

		public bool CanEditDelvData
		{
			get { return _caneditdelvdata; }
			set
			{
				_caneditdelvdata = value;
				RaisePropertyChanged("CanEditDelvData");
			}
		}

		private string _locknow;

		public string LOCK_NOW
		{
			get { return _locknow; }
			set
			{
				_locknow = value;
				RaisePropertyChanged("LOCK_NOW");
			}
		}

		private string _prevspdelv;

		public string PREV_SP_DELV
		{
			get { return _prevspdelv; }
			set
			{
				_prevspdelv = value;
				RaisePropertyChanged("PREV_SP_DELV");
			}
		}

		private bool _hasRetailForAdd;

		public bool HasRetailForAdd
		{
			get { return _hasRetailForAdd; }
			set
			{
				Set(() => HasRetailForAdd, ref _hasRetailForAdd, value);
			}
		}

		private bool _hasRetailForEdit;

		public bool HasRetailForEdit
		{
			get { return _hasRetailForEdit; }
			set
			{
				Set(() => HasRetailForEdit, ref _hasRetailForEdit, value);
			}
		}

		#region
		private string _dcaddress;

		public string DC_ADDRESS
		{
			get { return _dcaddress; }
			set
			{
				_dcaddress = value;
				RaisePropertyChanged("DC_ADDRESS");
			}
		}

		public void GET_DC_ADDRESS(string dcCode)
		{
			var proxyF19 = GetProxy<F19Entities>();
			var dcData = proxyF19.F1901s.Where(x => x.DC_CODE == dcCode).FirstOrDefault();
			if (dcData != null)
			{
				DC_ADDRESS = dcData.ADDRESS;
				if (UserOperateMode == OperateMode.Add && NEW_ITEM != null && NEW_ITEM.SP_DELV == "02")
					NEW_ITEM.ADDRESS = dcData.ADDRESS;
				else if (UserOperateMode == OperateMode.Edit && EDIT_ITEM != null && EDIT_ITEM.SP_DELV == "02")
					EDIT_ITEM.ADDRESS = dcData.ADDRESS;
			}
		}

		#endregion

		#region 查詢欄位設定
		private string _dccode;

		public string DC_CODE
		{
			get { return _dccode; }
			set
			{
				_dccode = value;
				RaisePropertyChanged("DC_CODE");
				GetQueryAllCompList();
			}
		}

		private DateTime _orddatefrom = DateTime.Now;

		public DateTime ORD_DATE_FROM
		{
			get { return _orddatefrom; }
			set
			{
				_orddatefrom = value;
				RaisePropertyChanged("ORD_DATE_FROM");
			}
		}

		private DateTime _orddateto = DateTime.Now;

		public DateTime ORD_DATE_TO
		{
			get { return _orddateto; }
			set
			{
				_orddateto = value;
				RaisePropertyChanged("ORD_DATE_TO");
			}
		}

		private string _ordno;

		public string ORD_NO
		{
			get { return _ordno; }
			set
			{
				_ordno = value;
				RaisePropertyChanged("ORD_NO");
			}
		}

		private DateTime? _arrivedatefrom = null;

		public DateTime? ARRIVE_DATE_FROM
		{
			get { return _arrivedatefrom; }
			set
			{
				_arrivedatefrom = value;
				RaisePropertyChanged("ARRIVE_DATE_FROM");
			}
		}

		private DateTime? _arrivedateto = null;

		public DateTime? ARRIVE_DATE_TO
		{
			get { return _arrivedateto; }
			set
			{
				_arrivedateto = value;
				RaisePropertyChanged("ARRIVE_DATE_TO");
			}
		}

		//貨主單號
		private string _custordno;

		public string CUST_ORD_NO
		{
			get { return _custordno; }
			set
			{
				_custordno = value;
				RaisePropertyChanged("CUST_ORD_NO");
			}
		}

    // 建議物流商
    private string _suglogisticcode;

    public string SUG_LOGISTIC_CODE
    {
      get { return _suglogisticcode; }
      set
      {
        _suglogisticcode = value;
        RaisePropertyChanged("SUG_LOGISTIC_CODE");
      }
    }

    private string _status;

		public string STATUS
		{
			get { return _status; }
			set
			{
				_status = value;
				RaisePropertyChanged("STATUS");
			}
		}

		//客戶代號
		private string _retailcode;

		public string RETAIL_CODE
		{
			get { return _retailcode; }
			set
			{
				_retailcode = value;
				RaisePropertyChanged("RETAIL_CODE");
			}
		}

		private string _custname;

		public string CUST_NAME
		{
			get { return _custname; }
			set
			{
				_custname = value;
				RaisePropertyChanged("CUST_NAME");
			}
		}

		//出貨單號 050801
		private string _wmsordno;

		public string WMS_ORD_NO
		{
			get { return _wmsordno; }
			set
			{
				_wmsordno = value;
				RaisePropertyChanged("WMS_ORd_NO");
			}
		}

		//託運單號 055001
		private string _pastNo;

		public string PAST_NO
		{
			get { return _pastNo; }
			set
			{
				_pastNo = value;
				RaisePropertyChanged("PAST_NO");
			}
		}

		private string _address;

		public string ADDRESS
		{
			get { return _address; }
			set
			{
				_address = value;
				RaisePropertyChanged("ADDRESS");
			}
		}

		//出貨單號

		#endregion

		#region 商品搜尋用欄位
		private string _itemcode;

		public string ITEM_CODE
		{
			get { return _itemcode; }
			set
			{
				_itemcode = value;
				RaisePropertyChanged("ITEM_CODE");
			}
		}

		private string _itemname;

		public string ITEM_NAME
		{
			get { return _itemname; }
			set
			{
				_itemname = value;
				RaisePropertyChanged("ITEM_NAME");
			}
		}

		private string _itemsize;

		public string ITEM_SIZE
		{
			get { return _itemsize; }
			set
			{
				_itemsize = value;
				RaisePropertyChanged("ITEM_SIZE");
			}
		}

		private string _itemspec;

		public string ITEM_SPEC
		{
			get { return _itemspec; }
			set
			{
				_itemspec = value;
				RaisePropertyChanged("ITEM_SPEC");
			}
		}

		private string _itemcolor;

		public string ITEM_COLOR
		{
			get { return _itemcolor; }
			set
			{
				_itemcolor = value;
				RaisePropertyChanged("ITEM_COLOR");
			}
		}

		private string _serialno;
		public string SERIAL_NO
		{
			get { return _serialno; }
			set
			{
				_serialno = value;
				RaisePropertyChanged("SERIAL_NO");
			}
		}

		private int _ordqty;

		public int ORD_QTY
		{
			get { return _ordqty; }
			set
			{
				_ordqty = value;
				RaisePropertyChanged("ORD_QTY");
			}
		}

		// 指定出貨批號
		private string _makeNo;
		public string MAKE_NO
		{
			get { return _makeNo; }
			set
			{
				_makeNo = value;
				RaisePropertyChanged("MAKE_NO");
			}
		}

		private bool makeNOEnable;
		public bool MAKE_NO_ENABLE
		{
			get { return makeNOEnable; }
			set
			{
				makeNOEnable = value;
				RaisePropertyChanged("MAKE_NO_ENABLE");
			}
		}

		#endregion

		#region 訂單主檔
		private ObservableCollection<F050101Ex> _dgordmainlist;

		public ObservableCollection<F050101Ex> dgOrdMainList
		{
			get { return _dgordmainlist; }
			set
			{
				_dgordmainlist = value;
				RaisePropertyChanged("dgOrdMainList");
			}
		}

		private F050101Ex _newitem;

		public F050101Ex NEW_ITEM
		{
			get { return _newitem; }
			set
			{
				if (_newitem != null)
					_newitem.PropertyChanged -= NEW_ITEM_PropertyChanged;
				Set(() => NEW_ITEM, ref _newitem, value);
				if (_newitem != null)
					_newitem.PropertyChanged += NEW_ITEM_PropertyChanged;
			}
		}

		private F050101Ex _editItem;

		public F050101Ex EDIT_ITEM
		{
			get { return _editItem; }
			set
			{
				if (_editItem != null)
					_editItem.PropertyChanged -= EDIT_ITEM_PropertyChanged;
				Set(() => EDIT_ITEM, ref _editItem, value);
				if (_editItem != null)
				{
					_editItem.PropertyChanged += EDIT_ITEM_PropertyChanged;
				}
			}
		}

		private F050101Ex _selectedF050101;

		public F050101Ex SELECTED_F050101
		{
			get { return _selectedF050101; }
			set
			{
				if (_selectedF050101 == value) return;
				_selectedF050101 = value;

				RaisePropertyChanged("SELECTED_F050101");
				SetOrdMainData();
			}
		}

		public NameValuePair<string> _selectEService;

		public NameValuePair<string> selectEService
		{
			get { return _selectEService; }
			set
			{
				if (value == null) return;
				_selectEService = value;
				RaisePropertyChanged("selectEService");
			}
		}


		#region 查詢配送商清單
		private List<NameValuePair<string>> _queryAllCompList;

		public List<NameValuePair<string>> QueryAllCompList
		{
			get { return _queryAllCompList; }
			set
			{
				Set(() => QueryAllCompList, ref _queryAllCompList, value);
			}
		}
		#endregion


		#region 查詢選取的配送商
		private string _selectedAllId;

		public string SelectedAllId
		{
			get { return _selectedAllId; }
			set
			{
				Set(() => SelectedAllId, ref _selectedAllId, value);
			}
		}
		#endregion



		#region 查詢通路商清單
		private List<NameValuePair<string>> _queryChannelList;

		public List<NameValuePair<string>> QueryChannelList
		{
			get { return _queryChannelList; }
			set
			{
				Set(() => QueryChannelList, ref _queryChannelList, value);
			}
		}

		private List<NameValuePair<string>> _querySubChannelList;

		public List<NameValuePair<string>> QuerySubChannelList
		{
			get { return _querySubChannelList; }
			set
			{
				Set(() => QuerySubChannelList, ref _querySubChannelList, value);
			}
		}
		#endregion


		#region 查詢選取的通路商
		private string _selectedChannel;

		public string SelectedChannel
		{
			get { return _selectedChannel; }
			set
			{
				Set(() => SelectedChannel, ref _selectedChannel, value);
			}
		}

		private string _selectedSubChannel;

		public string SelectedSubChannel
		{
			get { return _selectedSubChannel; }
			set
			{
				Set(() => SelectedSubChannel, ref _selectedSubChannel, value);
			}
		}
		#endregion


		#region 查詢 配送方式清單
		private List<NameValuePair<string>> _queryDelvTypeList;

		public List<NameValuePair<string>> QueryDelvTypeList
		{
			get { return _queryDelvTypeList; }
			set
			{
				Set(() => QueryDelvTypeList, ref _queryDelvTypeList, value);
			}
		}
		#endregion


		#region 查詢選取的配送方式
		private string _selectedDelvType;

		public string SelectedDelvType
		{
			get { return _selectedDelvType; }
			set
			{
				Set(() => SelectedDelvType, ref _selectedDelvType, value);
			}
		}
		#endregion


		#region 允許來回件
		private bool _allowRoundPiece;

		public bool AllowRoundPiece
		{
			get { return _allowRoundPiece; }
			set
			{
				Set(() => AllowRoundPiece, ref _allowRoundPiece, value);
			}
		}
		#endregion
		
		private void GetQueryAllCompList()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1947s.Where(x => x.DC_CODE == DC_CODE).ToList().Select(x => new NameValuePair<string> { Name = x.ALL_COMP, Value = x.ALL_ID }).ToList();
			data.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			QueryAllCompList = data;
			SelectedAllId = data.First().Value;
		}

		private void GetDelvTypeList()
		{
			QueryDelvTypeList = GetBaseTableService.GetF000904List(this.FunctionCode, "P050302", "DELV_TYPE", true);
			SelectedDelvType = QueryDelvTypeList.First().Value;
		}





		#region 接單平台訂單檔
		private F050304AddEService _f050304Data;

		public F050304AddEService f050304Data
		{
			get { return _f050304Data; }
			set
			{
				_f050304Data = value;
				RaisePropertyChanged("f050304Data");
			}
		}

		#endregion

		private void SetOrdMainData()
		{
			if (SELECTED_F050101 != null)
			{
				//ReGenTypeID(SELECTED_F050101.DC_CODE);
				ReGenTranCode(SELECTED_F050101.ORD_TYPE);
				EDIT_ITEM = new F050101Ex();
				EDIT_ITEM = ExDataMapper.Clone(SELECTED_F050101);
				GET_DC_ADDRESS(EDIT_ITEM.DC_CODE);

				if (EDIT_ITEM.SPECIAL_BUS == "0" && EDIT_ITEM.SELF_TAKE == "0" && EDIT_ITEM.CVS_TAKE == "0")
				{
					OptType = SendTypeData.ByHouse;
					CopyEditType = SendTypeData.ByHouse;
					EDIT_ITEM.ALL_ID = SELECTED_F050101.ALL_ID;
				}
				if (EDIT_ITEM.SELF_TAKE == "1")
				{
					OptType = SendTypeData.BySelf;
					CopyEditType = SendTypeData.BySelf;
				}
				if (EDIT_ITEM.CVS_TAKE == "1")
				{
					OptType = SendTypeData.ByVendor;
					CopyEditType = SendTypeData.ByVendor;
					F050304AddEService tmpF050304 = new F050304AddEService();
					P05ExDataSource proxyP05_vendor = GetExProxy<P05ExDataSource>();
					var data
					 = proxyP05_vendor.CreateQuery<F050304AddEService>("GetF050304ExDatas")
											 .AddQueryExOption("dcCode", DC_CODE)
											 .AddQueryExOption("gupCode", _gupCode)
											 .AddQueryExOption("custCode", _custCode)
											 .AddQueryExOption("ordNo", EDIT_ITEM.ORD_NO)
											 .ToList();
					if (data.Any())
					{
						tmpF050304 = data.FirstOrDefault();
						f050304Data = tmpF050304;
						CONSIGN_NO = f050304Data.CONSIGN_NO;
						DELV_RETAIL_CODE = f050304Data.DELV_RETAILCODE;
						DELV_RETAIL_NAME = f050304Data.DELV_RETAILNAME;
						EDIT_ITEM.ALL_ID = f050304Data.ALL_ID;
						EDIT_ITEM.ESERVICE = f050304Data.ESERVICE;
					}
				}

				ViewApprove = EDIT_ITEM.STATUS == "0" ? "Visible" : "Collapsed";
				_original_DISTR_CAR = EDIT_ITEM.SPECIAL_BUS;
				CanEditDelvData = EDIT_ITEM.SP_DELV == "02" ? false : true;
				CanEditDistrCar = EDIT_ITEM.EDI_FLAG == "1" || EDIT_ITEM.EDI_FLAG == "2" ? false : true;
				LOCK_NOW = EDIT_ITEM.ORD_TYPE == "0" && EDIT_ITEM.SP_DELV == "02" ? "1" : "0"; //b2b and 互賣
				PREV_SP_DELV = EDIT_ITEM.SP_DELV;
				string tmpSelect = ORD_TYPE_LIST.Where(a => a.Value == EDIT_ITEM.ORD_TYPE).First().Value;
				tmpSelect = (int.Parse(tmpSelect) + 1).ToString();
				GET_TYPEID_LIST(EDIT_ITEM.DC_CODE, tmpSelect);
				EDIT_ITEM.TYPE_ID = SELECTED_F050101.TYPE_ID;
				EDIT_ITEM.COLLECT = SELECTED_F050101.COLLECT;
				EDIT_ITEM.COLLECT_AMT = SELECTED_F050101.COLLECT_AMT;

				#region 訂單明細
				dgOrdDetailList_Display = new ObservableCollection<F050102Ex>();
				P05ExDataSource proxyP05 = GetExProxy<P05ExDataSource>();
				dgOrdDetailList_Display = proxyP05.CreateQuery<F050102Ex>("GetF050102ExDatas")
																										.AddQueryExOption("dcCode", DC_CODE)
																										.AddQueryExOption("gupCode", _gupCode)
																										.AddQueryExOption("custCode", _custCode)
																										.AddQueryExOption("ordNo", EDIT_ITEM.ORD_NO)
																										.ToObservableCollection();
				ORDDETAILDESC = Properties.Resources.P0503020000_DetailTotal + dgOrdDetailList_Display.Count.ToString();
				RaisePropertyChanged("dgOrdDetailList_Display");
				GetSerialNoFromDB();

				#endregion

				#region 出貨明細
				var proxyF05 = GetProxy<F05Entities>();
				dgShipList = proxyF05.CreateQuery<F050801>("GetF050801ByOrderNo")
						.AddQueryExOption("dcCode", EDIT_ITEM.DC_CODE)
						.AddQueryExOption("gupCode", EDIT_ITEM.GUP_CODE)
						.AddQueryExOption("custCode", EDIT_ITEM.CUST_CODE)
						.AddQueryExOption("ordNo", EDIT_ITEM.ORD_NO).ToList();
				#endregion

				CollapsedQryResultAction();
			}
			else
			{
				ORDDETAILDESC = "";
			}
		}

		#endregion

		#region 訂單明細
		private ObservableCollection<F050102Ex> _dgorddetaillist_Display;

		public ObservableCollection<F050102Ex> dgOrdDetailList_Display
		{
			get { return _dgorddetaillist_Display; }
			set
			{
				_dgorddetaillist_Display = value;
				RaisePropertyChanged("dgOrdDetailList_Display");
			}
		}

		private List<F050102Ex> _dgorddetaillist_Insert;

		public List<F050102Ex> dgOrdDetailList_Insert
		{
			get { return _dgorddetaillist_Insert; }
			set
			{
				_dgorddetaillist_Insert = value;
				RaisePropertyChanged("dgOrdDetailList_Insert");
			}
		}

		private F050102Ex _selected_orderdetail;

		public F050102Ex selected_OrderDetail
		{
			get { return _selected_orderdetail; }
			set
			{
				_selected_orderdetail = value;
				RaisePropertyChanged("selected_OrderDetail");
				if (_selected_orderdetail != null)
				{
					ITEM_CODE = _selected_orderdetail.ITEM_CODE;
					ITEM_NAME = _selected_orderdetail.ITEM_NAME;
					ITEM_SPEC = _selected_orderdetail.ITEM_SPEC;
					ITEM_SIZE = _selected_orderdetail.ITEM_SIZE;
					ITEM_COLOR = _selected_orderdetail.ITEM_COLOR;
					ORD_QTY = _selected_orderdetail.ORD_QTY;
					MAKE_NO = _selected_orderdetail.MAKE_NO;
				}
			}
		}

		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				CheckSelectedAll(_isCheckAll);
				RaisePropertyChanged("IsCheckAll");
			}
		}

		public void CheckSelectedAll(bool isChecked)
		{
			if (dgOrdDetailList_Display != null && dgOrdDetailList_Display.Any())
			{
				foreach (var p in dgOrdDetailList_Display)
				{
					p.ISSELECTED = isChecked;
				}
			}
		}

		private bool _isCheckAllByNoDelv;

		public bool IsCheckAllByNoDelv
		{
			get { return _isCheckAllByNoDelv; }
			set
			{
				_isCheckAllByNoDelv = value;
				CheckSelectedAllNoDelv(value);
				RaisePropertyChanged("IsCheckAllByNoDelv");
			}
		}

		private void CheckSelectedAllNoDelv(bool isChecked)
		{
			if (dgOrdDetailList_Display != null && dgOrdDetailList_Display.Any())
			{
				foreach (var p in dgOrdDetailList_Display)
				{
					p.NO_DELV = isChecked ? "1" : "0";
				}
			}
		}

		#endregion

		#region 出貨明細
		private List<F050801> _dgshiplist;

		public List<F050801> dgShipList
		{
			get { return _dgshiplist; }
			set
			{
				_dgshiplist = value;
				RaisePropertyChanged("dgShipList");
			}
		}

		private F050801 _selectedshipdetail;

		public F050801 selected_ShipDetail
		{
			get { return _selectedshipdetail; }
			set
			{
				_selectedshipdetail = value;
				//RaisePropertyChanged("selected_ShipDetail");
				//var myWin = new Wms3pl.WpfClient.P05.Views.P0503020100(EDIT_ITEM.GUP_CODE, EDIT_ITEM.CUST_CODE, EDIT_ITEM.DC_CODE, EDIT_ITEM.WMS_ORD_NO);
				//myWin.Show();
			}
		}

		#endregion

		#region 暫時ORD_NO 匯入小單用，待確認後再寫入真的ORD_NO
		private string _tempordno;

		public string TEMP_ORD_NO
		{
			get { return _tempordno; }
			set { _tempordno = value; }
		}

		#endregion

		#region Search

		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o => DoSearchCompleted()
					);
			}
		}

		public void DoSearch()
		{
			ClearData();
			dgOrdMainList = new ObservableCollection<F050101Ex>();
			P05ExDataSource proxyEx = GetExProxy<P05ExDataSource>();
			dgOrdMainList = proxyEx.CreateQuery<F050101Ex>("GetF050101ExDatas")
																									.AddQueryExOption("gupCode", _gupCode)
																									.AddQueryExOption("custCode", _custCode)
																									.AddQueryExOption("dcCode", DC_CODE)
																									.AddQueryExOption("ordDateFrom", ORD_DATE_FROM.ToString("yyyy/MM/dd"))
																									.AddQueryExOption("ordDateTo", ORD_DATE_TO.ToString("yyyy/MM/dd"))
																									.AddQueryExOption("ordNo", ORD_NO)
																									.AddQueryExOption("arriveDateFrom", ARRIVE_DATE_FROM.HasValue ? Convert.ToDateTime(ARRIVE_DATE_FROM).ToString("yyyy/MM/dd") : "")
																									.AddQueryExOption("arriveDateTo", ARRIVE_DATE_TO.HasValue ? Convert.ToDateTime(ARRIVE_DATE_TO).ToString("yyyy/MM/dd") : "")
																									.AddQueryExOption("custOrdNo", CUST_ORD_NO)
																									.AddQueryExOption("status", STATUS)
																									.AddQueryExOption("retailCode", RETAIL_CODE)
																									.AddQueryExOption("custName", CUST_NAME)
																									.AddQueryExOption("wmsOrdNo", WMS_ORD_NO)
																									.AddQueryExOption("pastNo", PAST_NO)
																									.AddQueryExOption("address", ADDRESS)
																									.AddQueryExOption("channel", SelectedChannel)
																									.AddQueryExOption("delvType", SelectedDelvType)
																									.AddQueryExOption("allId", SelectedAllId)
																									.AddQueryExOption("moveOutTarget", MOVE_OUT_TARGET)
																									.AddQueryExOption("subChannel", SelectedSubChannel)
																									.ToObservableCollection();
		}

		private void DoSearchCompleted()
		{
			if (dgOrdMainList == null || dgOrdMainList.Count == 0)
			{
				ClearData();
				DialogService.ShowMessage(Resources.Resources.InfoNoData);
			}
			else
			{
				if (dgOrdMainList.Count == 1)
				{
					SELECTED_F050101 = dgOrdMainList.First();
					SearchAction();
					CollapsedQryResultAction();
				}
				else
					SearchAction();
			}
		}

		#endregion Search

		#region Add

		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(),
					() => UserOperateMode == OperateMode.Query,
						o => DoAddCompleted()
					);
			}
		}

		private void DoAdd()
		{
			ClearData();

			NEW_ITEM = new F050101Ex();
			NEW_ITEM.ARRIVAL_DATE = DateTime.Today;
			NEW_ITEM.ORD_DATE = DateTime.Today;
			NEW_ITEM.GUP_CODE = _gupCode;
			NEW_ITEM.CUST_CODE = _custCode;
			NEW_ITEM.POSM = "0";
			NEW_ITEM.SA = "0";
			NEW_ITEM.COLLECT = "0";
			//NEW_ITEM.CVS_TAKE = "0";
			//始--配送資料區塊初始化資料
			UserOperateMode = OperateMode.Add;
			EService = new List<NameValuePair<string>>();
			CONSIGN_NO_New = string.Empty;
			DELV_RETAIL_CODE_NEW = string.Empty;
			DELV_RETAIL_NAME_NEW = string.Empty;
			NEW_ITEM.SPECIAL_BUS = "0";
			OptTypeNew = SendTypeData.BySelf;
			NEW_ITEM.CAN_FAST = "0";
			NEW_ITEM.COLLECT_AMT = 0;
			NEW_ITEM.ROUND_PIECE = "0";
			//終--配送資料區塊初始化資料
			TRAN_CODE_LIST = null;
			NEW_ITEM.CUST_COST = "";
			NEW_ITEM.SUG_BOX_NO = "";
		}

		private void DoAddCompleted()
		{
			if (_custCode == "010001")
				VIEWTRAN = "false";
			else
				VIEWTRAN = "true";
			AddAction();
		}

		#endregion Add

		#region Edit

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
						() => UserOperateMode == OperateMode.Query && EDIT_ITEM != null && (EDIT_ITEM.STATUS == "0" || EDIT_ITEM.STATUS == "1"),
					o => DoEditCompleted()
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;

			if (EDIT_ITEM.STATUS == "0")
			{
				//將資料中有請輸入三個字改為空白
				foreach (PropertyInfo pi in typeof(F050101Ex).GetProperties())
				{

					if (pi.CanRead && pi.CanWrite && pi.PropertyType == typeof(string))
					{
						var value = (string)pi.GetValue(EDIT_ITEM, null);
						if (value != null && value == Properties.Resources.P0503020000_Input)
						{
							pi.SetValue(EDIT_ITEM, "");
						}
					}
				}
			}
		}

		private void DoEditCompleted()
		{
			EditAction();
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
		}

		private void DoCancelCompleted()
		{
			if (UserOperateMode == OperateMode.Edit)
			{
				//var dcCode = SELECTED_F050101.DC_CODE;
				//var gupCode = SELECTED_F050101.GUP_CODE;
				//var custCode = SELECTED_F050101.CUST_CODE;
				//var ordNo = SELECTED_F050101.ORD_NO;
				//DoSearch();
				//SELECTED_F050101 = dgOrdMainList.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo);
				SELECTED_F050101 = SELECTED_F050101;
				SetOrdMainData();
				SearchAction();
				ClearSearchProduct();
			}
			else
			{
				ORD_NO = "";
				ORD_DATE_FROM = DateTime.Now;
				ORD_DATE_TO = DateTime.Now;
				ARRIVE_DATE_FROM = null;
				ARRIVE_DATE_TO = null;
				WMS_ORD_NO = "";
				PAST_NO = "";
				CUST_ORD_NO = "";
				STATUS = "A";
				RETAIL_CODE = "";
				CUST_NAME = "";
				ADDRESS = "";
				ClearData();
				CancelAction();
			}
			UserOperateMode = OperateMode.Query;
		}

		#endregion Cancel

		#region Delete

		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
						() => UserOperateMode == OperateMode.Query && EDIT_ITEM != null && (EDIT_ITEM.STATUS == "0" || EDIT_ITEM.STATUS == "1"),
					o => DoDeleteComopleted()
					);
			}
		}

		private void DoDelete()
		{
			isSave = false;
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				isSave = true;
				F050101 tmpF050101 = new F050101();
				tmpF050101.GUP_CODE = EDIT_ITEM.GUP_CODE;
				tmpF050101.CUST_CODE = EDIT_ITEM.CUST_CODE;
				tmpF050101.DC_CODE = EDIT_ITEM.DC_CODE;
				tmpF050101.ORD_NO = EDIT_ITEM.ORD_NO;
				tmpF050101.CVS_TAKE = EDIT_ITEM.CVS_TAKE;
				tmpF050101.CUST_ORD_NO = EDIT_ITEM.CUST_ORD_NO;
				var proxy = new wcf.P05WcfServiceClient();
				var wcfF050101 = ExDataMapper.Map<F050101, wcf.F050101>(tmpF050101);
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteF050101(wcfF050101));
				ShowMessage(new List<Wms3pl.WpfClient.ExDataServices.P05ExDataService.ExecuteResult>() { new Wms3pl.WpfClient.ExDataServices.P05ExDataService.ExecuteResult() { IsSuccessed = result.IsSuccessed, Message = result.Message } });
			}
		}

		private void DoDeleteComopleted()
		{
			if (isSave)
			{
				DC_CODE = EDIT_ITEM.DC_CODE;
				ORD_NO = EDIT_ITEM.ORD_NO;
				ORD_DATE_FROM = (DateTime)EDIT_ITEM.ORD_DATE;
				ORD_DATE_TO = (DateTime)EDIT_ITEM.ORD_DATE;
				ARRIVE_DATE_FROM = null;
				ARRIVE_DATE_TO = null;
				STATUS = "9";
				ClearData();
				SearchCommand.Execute(null);
				UserOperateMode = OperateMode.Query;
			}
		}

		#endregion Delete

		#region Save

		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode != OperateMode.Query && dgOrdDetailList_Display != null && dgOrdDetailList_Display.Count() > 0,
					o => DoSaveCompleted()
					);
			}
		}

		private void DoSave()
		{
			continueSave = true;
			Gen_dgOrdDetailList_Insert();
			if (continueSave)
			{
				if (UserOperateMode == OperateMode.Add)
				{
					DoSaveAdd();
				}
				else
				{
					DoSaveEdit();
				}
			}
		}


		private void DoSaveAdd()
		{
			isSave = false;
			if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
			{
				#region old validation
				//if (dgOrdDetailList_Insert.Where(x => x.BUNDLE_SERIALLOC == "1" && (x.SERIAL_NO == "" || x.SERIAL_NO == null)).FirstOrDefault() != null)
				//{
				//    DialogService.ShowMessage("訂單明細中有序號控管商品，需要匯入序號。");
				//    return;
				//}

				//if (dgOrdDetailList_Display.Where(x => x.ORD_QTY == 0).FirstOrDefault() != null)
				//{
				//    DialogService.ShowMessage("數量不能為0");
				//    return;
				//}

				//DateTime NEW_ORD_DATE = (DateTime)NEW_ITEM.ORD_DATE;
				//TimeSpan s = new TimeSpan(NEW_ITEM.ARRIVAL_DATE.Ticks - NEW_ORD_DATE.Ticks);
				//if (s.TotalDays > 7)
				//{
				//    DialogService.ShowMessage("指定到貨日期不能大於訂單日7天。");
				//    return;
				//}
				#endregion

				if (SaveValidation() == false)
					return;
				ChangeSendType(NEW_ITEM, OptTypeNew);
				F050101 f050101 = new F050101();
				f050101 = ExDataMapper.Map<F050101Ex, F050101>(NEW_ITEM);

				f050101.STATUS = "0";
				f050101.SP_DELV = NEW_ITEM.SP_DELV;
				f050101.GUP_CODE = _gupCode;
				f050101.CUST_CODE = _custCode;
				f050101.CRT_STAFF = _userId;
				f050101.CRT_DATE = DateTime.Now;
				f050101.CRT_NAME = _userName;
				string tmpEservice = string.Empty;
				
				var wcfF050101 = ExDataMapper.Map<F050101, wcf.F050101>(f050101);
				var wcfF050102 = ExDataMapper.MapCollection<F050102Ex, wcf.F050102Ex>(dgOrdDetailList_Insert).ToArray();
				F050304 tmpF050304 = new F050304();
				if (OptTypeNew == SendTypeData.ByVendor)
				{
					//string tmpOrdNo = result.Message.Split('：')[1].Split(' ')[0];
					tmpF050304 = new F050304()
					{
						ALL_ID = NEW_ITEM.ALL_ID, //配送商編號
						ESERVICE = selectEService.Value, //服務商編號
						ORD_NO = "", //訂單編號
						DC_CODE = NEW_ITEM.DC_CODE, //物流中心
						GUP_CODE = NEW_ITEM.GUP_CODE, //業主
						CUST_CODE = NEW_ITEM.CUST_CODE, //貨主
						BATCH_NO = NEW_ITEM.BATCH_NO, //批次號
						CUST_ORD_NO = NEW_ITEM.CUST_ORD_NO, //貨主單號
						CONSIGN_NO = CONSIGN_NO_New, //託運單號/配送編號
						CRT_STAFF = _userId, //建立人員
						CRT_DATE = DateTime.Now, //建立日期
						CRT_NAME = _userName, //建立人名
						DELV_RETAILCODE = DELV_RETAIL_CODE_NEW, //配送門市編號
						DELV_RETAILNAME = DELV_RETAIL_NAME_NEW, //配送門市名稱
					};
					if (tmpF050304.ALL_ID == "711")
					{
						tmpF050304.DELV_DATE = DateTime.Parse(DateTime.Now.AddDays(2).ToString("yyyy/MM/dd"));
						tmpF050304.RETURN_DATE = DateTime.Parse(DateTime.Now.AddDays(9).ToString("yyyy/MM/dd"));
					}
					else if (tmpF050304.ALL_ID == "FAMILY")
					{
						tmpF050304.DELV_DATE = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd"));
					}
				}
				else
				{
					tmpF050304 = null;
				}

				var wcfF050304 = tmpF050304 != null ? ExDataMapper.Map<F050304, wcf.F050304>(tmpF050304) : null;
				var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
				var result = proxy.RunWcfMethod(w => w.InsertF050101(wcfF050101, wcfF050102, wcfF050304));

				if (result.IsSuccessed)
				{
					_original_DISTR_CAR = "";
				}
				isSave = result.IsSuccessed;
				ShowResultMessage(result);
			}
		}

		private void DoSaveEdit()
		{
			isSave = false;
			var proxy = GetProxy<F05Entities>();
			var f050101 = proxy.F050101s.Where(x => x.DC_CODE == EDIT_ITEM.DC_CODE && x.GUP_CODE == EDIT_ITEM.GUP_CODE && x.CUST_CODE == EDIT_ITEM.CUST_CODE && x.ORD_NO == EDIT_ITEM.ORD_NO).FirstOrDefault();
			if (f050101 != null && f050101.STATUS != EDIT_ITEM.STATUS)
			{
				ShowWarningMessage(f050101.STATUS == "9" ? Properties.Resources.P0503020000_OrderIsCancel : Properties.Resources.P0503020000_OrderIsApproved);
				ORD_NO = f050101.ORD_NO;
				UserOperateMode = OperateMode.Query;
				DoSearch();
				DispatcherAction(() =>
				{
					SELECTED_F050101 = dgOrdMainList.FirstOrDefault();
				});
				return;
			}
			var f050301 = proxy.F050301s.Where(x => x.DC_CODE == EDIT_ITEM.DC_CODE && x.GUP_CODE == EDIT_ITEM.GUP_CODE && x.CUST_CODE == EDIT_ITEM.CUST_CODE && x.ORD_NO == EDIT_ITEM.ORD_NO).FirstOrDefault();
			if (f050301 != null)
			{
				ShowWarningMessage(Properties.Resources.P0503020000_OrderIsCreateBatch);
				ORD_NO = f050101.ORD_NO;
				UserOperateMode = OperateMode.Query;
				DoSearch();
				DispatcherAction(() =>
				{
					SELECTED_F050101 = dgOrdMainList.FirstOrDefault();
				});
				return;
			}

			if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
			{
				if (SaveValidation() == false)
					return;
				EditAndApproveSave("EDIT");
			}
		}

		private void DoSaveCompleted(bool isApprove = false)
		{
			if (isSave == true)
			{
				var f050101ex = UserOperateMode == OperateMode.Add ? ExDataMapper.Clone(NEW_ITEM) : ExDataMapper.Clone(EDIT_ITEM);
				f050101ex.ORD_NO = _ordno;
				var f050102exs = ExDataMapper.MapCollection<F050102Ex, F050102Ex>(dgOrdDetailList_Display).ToArray();
				OperateMode opMode = UserOperateMode;

				DC_CODE = UserOperateMode == OperateMode.Add ? NEW_ITEM.DC_CODE : EDIT_ITEM.DC_CODE;

				ORD_DATE_FROM = UserOperateMode == OperateMode.Add ? (DateTime)NEW_ITEM.ORD_DATE : (DateTime)EDIT_ITEM.ORD_DATE;
				ORD_DATE_TO = UserOperateMode == OperateMode.Add ? (DateTime)NEW_ITEM.ORD_DATE : (DateTime)EDIT_ITEM.ORD_DATE;

				UserOperateMode = OperateMode.Query;
				DoSearch();
				if (dgOrdMainList != null)
					SELECTED_F050101 = dgOrdMainList.LastOrDefault();

				if (isApprove)
					DoDistrCar(opMode, f050101ex, f050102exs.ToList());
				ORD_NO = string.IsNullOrEmpty(f050101ex.ORD_NO) ? SELECTED_F050101.ORD_NO : f050101ex.ORD_NO;
				DoSearch();
				SELECTED_F050101 = dgOrdMainList.FirstOrDefault();
				ARRIVE_DATE_FROM = null;
				ARRIVE_DATE_TO = null;
				SearchAction();
				CopyEditType = OptType;
			}
		}

		#endregion Save

		#region Approve

		public ICommand ApproveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoApprove(),
					() => UserOperateMode == OperateMode.Edit && dgOrdDetailList_Display != null && dgOrdDetailList_Display.Count() > 0 && !string.IsNullOrWhiteSpace(EDIT_ITEM.CUST_COST),
					o => DoSaveCompleted(true)
					);
			}
		}

		private void DoApprove()
		{
			isSave = false;
			continueSave = true;
			var proxy = GetProxy<F05Entities>();
			var f050101 = proxy.F050101s.Where(x => x.DC_CODE == EDIT_ITEM.DC_CODE && x.GUP_CODE == EDIT_ITEM.GUP_CODE && x.CUST_CODE == EDIT_ITEM.CUST_CODE && x.ORD_NO == EDIT_ITEM.ORD_NO).FirstOrDefault();
			if (f050101 != null && f050101.STATUS != EDIT_ITEM.STATUS)
			{
				ShowWarningMessage(f050101.STATUS == "9" ? Properties.Resources.P0503020000_OrderIsCancel : Properties.Resources.P0503020000_OrderIsApproved);
				ORD_NO = f050101.ORD_NO;
				UserOperateMode = OperateMode.Query;
				DoSearch();
				DispatcherAction(() =>
				{
					SELECTED_F050101 = dgOrdMainList.FirstOrDefault();
				});
				return;
			}
			var f050301 = proxy.F050301s.Where(x => x.DC_CODE == EDIT_ITEM.DC_CODE && x.GUP_CODE == EDIT_ITEM.GUP_CODE && x.CUST_CODE == EDIT_ITEM.CUST_CODE && x.ORD_NO == EDIT_ITEM.ORD_NO).FirstOrDefault();
			if (f050301 != null)
			{
				ShowWarningMessage(Properties.Resources.P0503020000_OrderIsCreateBatch);
				ORD_NO = f050101.ORD_NO;
				UserOperateMode = OperateMode.Query;
				DoSearch();
				DispatcherAction(() =>
				{
					SELECTED_F050101 = dgOrdMainList.FirstOrDefault();
				});
				return;
			}
			if (ShowMessage(Messages.WarningBeforeApproveF050101) == DialogResponse.Yes)
			{
				Gen_dgOrdDetailList_Insert();
				if (continueSave)
				{
					if (SaveValidation() == false)
						return;

					EditAndApproveSave("APPROVE");
				}
			}
		}

		#endregion

		#region EditAndApproveSave

		private void EditAndApproveSave(string saveType)
		{
			isSave = true;

			F050101 f050101 = new F050101();
			ChangeSendType(EDIT_ITEM, OptType);
			f050101 = ExDataMapper.Map<F050101Ex, F050101>(EDIT_ITEM);
			f050101.GUP_CODE = _gupCode;
			f050101.CUST_CODE = _custCode;
			f050101.UPD_STAFF = _userId;
			f050101.UPD_DATE = DateTime.Now;
			f050101.UPD_NAME = _userName;

			var wcfF050101 = ExDataMapper.Map<F050101, wcf.F050101>(f050101);
			var wcfF050102 = ExDataMapper.MapCollection<F050102Ex, wcf.F050102Ex>(dgOrdDetailList_Insert).ToArray();
			var tmpF050304 = new F050304();
			if (OptType == SendTypeData.ByVendor)
			{
				tmpF050304 = new F050304()
				{
					ALL_ID = EDIT_ITEM.ALL_ID, //配送商編號
					ESERVICE = EDIT_ITEM.ESERVICE, //服務商編號
					ORD_NO = EDIT_ITEM.ORD_NO, //訂單編號
					DC_CODE = EDIT_ITEM.DC_CODE, //物流中心
					GUP_CODE = EDIT_ITEM.GUP_CODE, //業主
					CUST_CODE = EDIT_ITEM.CUST_CODE, //貨主
					BATCH_NO = EDIT_ITEM.BATCH_NO, //批次號
					CUST_ORD_NO = EDIT_ITEM.CUST_ORD_NO, //貨主單號
					CONSIGN_NO = CONSIGN_NO, //託運單號/配送編號
					CRT_STAFF = _userId, //建立人員
					CRT_DATE = DateTime.Now, //建立日期
					CRT_NAME = _userName, //建立人名
					DELV_RETAILCODE = DELV_RETAIL_CODE, //配送門市編號
					DELV_RETAILNAME = DELV_RETAIL_NAME, //配送門市名稱
				};
				if (tmpF050304.ALL_ID == "711")
				{
					tmpF050304.DELV_DATE = DateTime.Parse(DateTime.Now.AddDays(2).ToString("yyyy/MM/dd"));
					tmpF050304.RETURN_DATE = DateTime.Parse(DateTime.Now.AddDays(9).ToString("yyyy/MM/dd"));
				}
				else if (tmpF050304.ALL_ID == "FAMILY")
				{
					tmpF050304.DELV_DATE = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd"));
				}
			}
			else if (CopyEditType == SendTypeData.ByVendor)
			{
				tmpF050304 = new F050304()
				{
					DC_CODE = f050304Data.DC_CODE,
					GUP_CODE = f050304Data.GUP_CODE,
					CUST_CODE = f050304Data.CUST_CODE,
					ORD_NO = f050304Data.ORD_NO
				};
			}
			var wcfF050304 = tmpF050304 != null ? ExDataMapper.Map<F050304, wcf.F050304>(tmpF050304) : new ExDataServices.P05WcfService.F050304();

			//已轉入訂單池 改由核准重新建立更新
			if (EDIT_ITEM.STATUS == "1")
				saveType = "APPROVE";
			
			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();

			var result = new wcf.ExecuteResult();
			if (saveType == "EDIT")
			{
				result = proxy.RunWcfMethod<wcf.ExecuteResult>(w => w.UpdateF050101(wcfF050101, wcfF050102, wcfF050304));
			}
			else if (saveType == "APPROVE")
				result = proxy.RunWcfMethod<wcf.ExecuteResult>(w => w.ApproveF050101(wcfF050101, wcfF050102));

			if (result.IsSuccessed)
			{
				_ordno = EDIT_ITEM.ORD_NO;
			}

			isSave = result.IsSuccessed;
			ShowResultMessage(result);
		}

		#endregion

		#region AddItemCommand

		public ICommand AddItemCommand
		{
			get
			{
				return new RelayCommand(
					DoAddItem,
						() => (UserOperateMode == OperateMode.Add && NEW_ITEM != null && !string.IsNullOrEmpty(NEW_ITEM.DC_CODE) && !string.IsNullOrEmpty(ITEM_NAME) && NEW_ITEM.SP_DELV != "01") ||
								(UserOperateMode == OperateMode.Edit && EDIT_ITEM != null && !string.IsNullOrEmpty(EDIT_ITEM.DC_CODE) && !string.IsNullOrEmpty(ITEM_NAME) && EDIT_ITEM.SP_DELV != "01" && EDIT_ITEM.STATUS == "0")
						);
			}
		}

		//public ICommand AddItemCommand
		//{
		//    get
		//    {
		//        return CreateBusyAsyncCommand(
		//            o => DoNothing(),
		//            () => (UserOperateMode == OperateMode.Add && NEW_ITEM != null && !string.IsNullOrEmpty(NEW_ITEM.DC_CODE) && !string.IsNullOrEmpty(ITEM_NAME) && NEW_ITEM.SP_DELV != "01") ||
		//                (UserOperateMode == OperateMode.Edit && EDIT_ITEM != null && !string.IsNullOrEmpty(EDIT_ITEM.DC_CODE) && !string.IsNullOrEmpty(ITEM_NAME) && EDIT_ITEM.SP_DELV != "01")
		//                ,
		//            o => DoNothing(),
		//            null,
		//            () => DoAddItem()
		//            );
		//    }
		//}


		private void DoNothing()
		{
		}

		private void DoAddItem()
		{
			if (dgOrdDetailList_Display == null)
				dgOrdDetailList_Display = new ObservableCollection<F050102Ex>();

			if (CheckDetail(true))
			{
				var f1903 = FindF1903(ITEM_CODE);
				F050102Ex f050102 = new F050102Ex()
				{
					ROWNUM = 0,
					ORD_NO = (UserOperateMode == OperateMode.Add ? "" : EDIT_ITEM.ORD_NO),
					ORD_SEQ = "",
					SERIAL_NO = "",
					ITEM_CODE = ITEM_CODE,
					ITEM_NAME = ITEM_NAME,
					ITEM_SIZE = ITEM_SIZE,
					ITEM_SPEC = ITEM_SPEC,
					ITEM_COLOR = ITEM_COLOR,
					ORD_QTY = ORD_QTY,
					GUP_CODE = _gupCode,
					CUST_CODE = _custCode,
					DC_CODE = (UserOperateMode == OperateMode.Add ? NEW_ITEM.DC_CODE : EDIT_ITEM.DC_CODE),
					CRT_STAFF = _userId,
					CRT_DATE = DateTime.Now,
					CRT_NAME = _userName,
					ISSELECTED = false,
					BUNDLE_SERIALLOC = (f1903 == null) ? "0" : f1903.BUNDLE_SERIALLOC,
					BUNDLE_SERIALNO = (f1903 == null) ? "0" : f1903.BUNDLE_SERIALNO,
					NO_DELV = "0",
					MAKE_NO = MAKE_NO					
				};


				ClearSearchProduct();
				dgOrdDetailList_Display.Add(f050102);
				dgOrdDetailList_Display = dgOrdDetailList_Display.ToObservableCollection();
				RaisePropertyChanged("dgOrdDetailList_Display");
			}
		}



		#endregion AddItemCommand

		#region DeleteItemCommand

		public ICommand DeleteItemCommand
		{
			get
			{
				return new RelayCommand(
					DoDeleteItem,
						() => UserOperateMode != OperateMode.Query && dgOrdDetailList_Display != null
										&& dgOrdDetailList_Display.Where(x => x.ISSELECTED).FirstOrDefault() != null
										&& ((NEW_ITEM != null && NEW_ITEM.SP_DELV != "01") || (EDIT_ITEM != null && EDIT_ITEM.SP_DELV != "01"))
						);
			}
		}

		//public ICommand DeleteItemCommand
		//{
		//    get
		//    {
		//        return CreateBusyAsyncCommand(
		//            o => DoNothing(),
		//            () => UserOperateMode != OperateMode.Query && dgOrdDetailList_Display != null
		//                    && dgOrdDetailList_Display.Where(x => x.ISSELECTED).FirstOrDefault() != null
		//                    && ((NEW_ITEM != null && NEW_ITEM.SP_DELV != "01") || (EDIT_ITEM != null && EDIT_ITEM.SP_DELV != "01")),
		//            o => DoNothing(),
		//            null,
		//            () => DoDeleteItem()
		//            );
		//    }
		//}

		private void DoDeleteItem()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				if (dgOrdDetailList_Display.Where(x => x.ISSELECTED).FirstOrDefault() == null)
				{
					DialogService.ShowMessage(Properties.Resources.P0503020000_NotSelectDeleteItem);
					return;
				}
				var f010102s = dgOrdDetailList_Display.Where(x => x.ISSELECTED).ToList();
				foreach (var p in f010102s)
				{
					dgOrdDetailList_Display.Remove(p);
				}

				ClearSearchProduct();
				dgOrdDetailList_Display = dgOrdDetailList_Display.ToObservableCollection();
				if (dgOrdDetailList_Display.Any(x => x.BUNDLE_SERIALNO == "1"))
				{
					if (serialNo_Data != null)
						serialNo_Data = null;
				}
				ShowMessage(Messages.InfoDeleteSuccess);
				RaisePropertyChanged("dgOrderList");
			}
		}

		#endregion

		#region 匯入序號 ImportSerialNoCommand

		public ICommand ImportSerialNoCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoNothing(),
						() => UserOperateMode != OperateMode.Query && dgOrdDetailList_Display != null && dgOrdDetailList_Display.Any(x => x.BUNDLE_SERIALNO == "1") && EDIT_ITEM != null && EDIT_ITEM.STATUS == "0",
						o => DoImportSerialNoAction()
						);
			}
		}

		private void DoImportSerialNoAction()
		{
		}

		public void DoImportSerialNo()
		{
			if (serialNo_Data != null && serialNo_Data.Count > 0)
			{
				if (ShowMessage(Messages.WarningSerialNoHasExist) == DialogResponse.Yes)
					serialNo_Data = null;
				else
					return;
			}
			var msg = new MessagesStruct();
			try
			{
				serialNo_Data = new List<serialNoField>();
				List<string> data = System.IO.File.ReadAllLines(FullPath).ToList();
				// 沒有資料時直接跳出
				if (!data.Any()) return;
				//排除第一筆
				bool canAdd = false;
				foreach (var d in data)
				{
					var tmpItem = d.Split(',');
					if (!canAdd)
					{
						canAdd = true;
						continue;
					}

					var v = new serialNoField()
					{
						ITEM_CODE = tmpItem[0],
						SERIAL_NO = tmpItem[1],
						USED = tmpItem[2]
					};
					serialNo_Data.Add(v);
				};

				if (IsValidSerialNo() == false)
				{
					if (serialNo_Data != null || serialNo_Data.Count == 0)
						serialNo_Data = null;
					var result = new ExDataServices.P19ExDataService.ExecuteResult();
					result.Message = Properties.Resources.P0503020000_FileFormatError;
					msg = Messages.ErrorImportFailed;
					msg.Message = msg.Message + Environment.NewLine + result.Message;
					ShowMessage(msg);
					CanViewSeria = false;
					return;
				}
				//目前沒有做序號匯入

				CanEditDetailQty = "0";
				msg = Messages.InfoImportSuccess;
				ShowMessage(msg);
				CanViewSeria = true;
			}
			catch (Exception ex)
			{
				if (serialNo_Data != null || serialNo_Data.Count == 0)
					serialNo_Data = null;
				var result = new ExDataServices.P19ExDataService.ExecuteResult();
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P0503020000_FileFormatError + Environment.NewLine + ex.Message;
				msg = Messages.ErrorImportFailed;
				msg.Message = msg.Message + Environment.NewLine + result.Message;
				ShowMessage(msg);
				CanViewSeria = false;
			}
		}

		private bool IsValidSerialNo()
		{
			//序號不能重覆
			//*需有明細才可匯入序號
			//*匯入的序號與F2501序號檔比對，需為可出貨狀態(A1)
			//*匯入的序號對應的商品需在明細中存在且須為序號綁儲位商品(F1903BOUNDLE_SERIALLOC)
			//*匯入序號UI參考出貨包裝的匯入序號
			//*Ex.依照F2501,每個序號都會有對應商品,假如訂購A商品5個、B商品3個，則匯入的序號必須為5個A商品序號，3個B商品序號
			//(商品序號指3C產品的產品ID或是SIM卡的卡號等…)

			var proxy25 = GetProxy<F25Entities>();
			var proxy19 = GetProxy<F19Entities>();
			bool isValid = true;
			//serialNo_Data = (from col in dt.AsEnumerable()
			//                 select new serialNoField
			//                 {
			//                     ITEM_CODE = "",
			//                     SERIAL_NO = Convert.ToString(col[0]),
			//                     USED = "0"
			//                 }).ToList();

			//匯入的序號與F2501序號檔比對，需為可出貨狀態(A1)
			foreach (var d in serialNo_Data)
			{
				var f2501 = proxy25.F2501s.Where(x => x.SERIAL_NO == d.SERIAL_NO && x.STATUS == "A1").FirstOrDefault();
				if (f2501 == null)
				{
					DialogService.ShowMessage(Properties.Resources.P0503020000_SerialNoA1);
					return false;
				}
				else
				{
					//將序號所對應的產品寫回data_New
					d.ITEM_CODE = f2501.ITEM_CODE;
					//匯入的序號對應的商品需在明細中存在且須為序號綁儲位商品(F1903BOUNDLE_SERIALLOC)
					var f050102 = dgOrdDetailList_Display.Where(x => x.ITEM_CODE == f2501.ITEM_CODE).FirstOrDefault();
					if (f050102 == null)
					{
						DialogService.ShowMessage(Properties.Resources.P0503020000_ImportSerialNoItemExist);
						return false;
					}
					else
					{
						var f1903 = proxy19.F1903s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.BUNDLE_SERIALNO == "1").FirstOrDefault();
						if (f1903 == null)
						{
							DialogService.ShowMessage(Properties.Resources.P0503020000_ImportItemNotSerial);
							return false;
						}
					}
				}
			} //	foreach(var d in data)

			//序號商品數量的序號檢核
			var bundle_Serial_Item = dgOrdDetailList_Display.Where(x => x.BUNDLE_SERIALNO == "1" && x.NO_DELV == "0").ToList();
			foreach (var d in bundle_Serial_Item)
			{
				if (d.ORD_QTY != serialNo_Data.Where(x => x.ITEM_CODE == d.ITEM_CODE).Count())
				{
					DialogService.ShowMessage(Properties.Resources.P0503020000_SerialQtyNotEqualItemQty);
					return false;
				}
				var howManySerialNo = serialNo_Data.Where(x => x.ITEM_CODE == d.ITEM_CODE).GroupBy(x => x.SERIAL_NO).Count();
				if (d.ORD_QTY != howManySerialNo)
				{
					DialogService.ShowMessage(Properties.Resources.P0503020000_SerialNoRepeat);
					return false;
				}
			}
			return isValid;
		}

		public partial class serialNoField
		{
			public string ITEM_CODE { get; set; }
			public string SERIAL_NO { get; set; }
			public string USED { get; set; }
		}

		#endregion
		
		#region 出貨倉別
		public List<NameValuePair<string>> _typeidlist;

		public List<NameValuePair<string>> TYPEID_LIST
		{
			get { return _typeidlist; }
			set
			{
				_typeidlist = value;
				RaisePropertyChanged("TYPEID_LIST");
			}
		}

		public void GET_TYPEID_LIST(string dcCode, string selectTicket)
		{
			if (string.IsNullOrEmpty(selectTicket) || string.IsNullOrEmpty(dcCode))
			{
				TYPEID_LIST = new List<NameValuePair<string>>();
				return;
			}
			var proxy19 = GetProxy<F19Entities>();
			var f190001 = (from p in proxy19.F190001s
										 where p.DC_CODE == dcCode
										 select p).ToList();

			var f190002 = (from p in proxy19.F190002s
										 where p.DC_CODE == dcCode
										 select p).ToList();

			var f198001 = (from a in proxy19.F198001s
										 select a).ToList();

			TYPEID_LIST = (from a in f190001
										 join b in f190002 on a.TICKET_ID equals b.TICKET_ID
										 join c in f198001 on b.WAREHOUSE_TYPE equals c.TYPE_ID
										 where a.DC_CODE == dcCode && a.CUST_CODE == _custCode
										 && a.TICKET_CLASS.Contains("O" + selectTicket)
										 group c by new { c.TYPE_NAME, c.TYPE_ID } into g
										 let warehouse = g.First()
										 select new NameValuePair<string>(warehouse.TYPE_NAME, warehouse.TYPE_ID)
										 ).ToList();

			if (UserOperateMode == OperateMode.Add && NEW_ITEM != null)
			{
				NEW_ITEM.TYPE_ID = TYPEID_LIST.Where(item => item.Value == "G").Select(item => item.Value).FirstOrDefault(); // G =良品倉, 訂單維護出貨倉別TYPE_ID不可不選,預設良品倉
			}

		}

		#endregion

		#region 配送商
		public List<NameValuePair<string>> _allidlist;

		public List<NameValuePair<string>> ALLID_LIST
		{
			get { return _allidlist; }
			set
			{
				_allidlist = value;
				RaisePropertyChanged("ALLID_LIST");
			}
		}

		public void GET_ALLID_LIST(string dcCode)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.CreateQuery<F1947>("GetAllowedF1947s")
															.AddQueryExOption("dcCode", dcCode)
															.AddQueryExOption("gupCode", _gupCode)
															.AddQueryExOption("custCode", _custCode).ToList();
			var list = new List<NameValuePair<string>>();

			if (NEW_ITEM != null && UserOperateMode == OperateMode.Add)
			{
				switch (OptTypeNew)
				{
					case SendTypeData.ByHouse:
						list = data.Where(o => o.TYPE == "0")
				.Select(o => new NameValuePair<string>
				{
					Name = o.ALL_COMP,
					Value = o.ALL_ID
				}).ToList();
						break;

					case SendTypeData.ByVendor:
						list = data.Where(o => o.TYPE == "1")
				.Select(o => new NameValuePair<string>
				{
					Name = o.ALL_COMP,
					Value = o.ALL_ID
				}).ToList();
						break;

					case SendTypeData.BySelf:
						list = new List<NameValuePair<string>>();
						break;
				}
				ALLID_LIST = list;
				if (UserOperateMode != OperateMode.Query)
					NEW_ITEM.ALL_ID = list.FirstOrDefault()?.Value;
			}
			else if (EDIT_ITEM != null && UserOperateMode != OperateMode.Add)
			{
				switch (OptType)
				{
					case SendTypeData.ByHouse:
						list = data.Where(o => o.TYPE == "0")
				.Select(o => new NameValuePair<string>
				{
					Name = o.ALL_COMP,
					Value = o.ALL_ID
				}).ToList();

						break;

					case SendTypeData.ByVendor:
						list = data.Where(o => o.TYPE == "1")
				.Select(o => new NameValuePair<string>
				{
					Name = o.ALL_COMP,
					Value = o.ALL_ID
				}).ToList();
						break;

					case SendTypeData.BySelf:
						list = new List<NameValuePair<string>>();
						break;
				}

				ALLID_LIST = list;
				//EDIT_ITEM.ALL_ID = list.FirstOrDefault()?.Value;
			}
		}

		#endregion

		#region 物流中心
		private List<NameValuePair<string>> _dclist;

		public List<NameValuePair<string>> DC_LIST
		{
			get { return _dclist; }
			set
			{
				_dclist = value;
				RaisePropertyChanged("DCLIST");
			}
		}

		private void GET_DC_LIST()
		{
			DC_LIST = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DC_LIST.Any())
				DC_CODE = DC_LIST.First().Value;
		}

		#endregion

		#region 作業類別
		private List<NameValuePair<string>> _trancodelist;

		public List<NameValuePair<string>> TRAN_CODE_LIST
		{
			get { return _trancodelist; }
			set
			{
				_trancodelist = value;
				RaisePropertyChanged("TRAN_CODE_LIST");
			}
		}

		public void GET_TRAN_CODE_LIST()
		{
			var proxyF00 = GetProxy<F00Entities>();

			TRAN_CODE_LIST = proxyF00.F000903s.Where(x => x.ORD_PROP.StartsWith("O"))
							 .Select(x => new NameValuePair<string>() { Value = x.ORD_PROP, Name = x.ORD_PROP_NAME }).ToList();

			if (TRAN_CODE_LIST.Any())
			{
				if (UserOperateMode == OperateMode.Add && NEW_ITEM != null)
					NEW_ITEM.TRAN_CODE = TRAN_CODE_LIST.First().Value;
				else if (UserOperateMode == OperateMode.Edit && EDIT_ITEM != null)
					EDIT_ITEM.TRAN_CODE = TRAN_CODE_LIST.First().Value;
			}
		}

		#endregion

		#region STATUS_LIST
		private List<NameValuePair<string>> _statuslist;

		public List<NameValuePair<string>> STATUS_LIST
		{
			get { return _statuslist; }
			set
			{
				_statuslist = value;
				RaisePropertyChanged("STATUS_LIST");
			}
		}

		public void GET_STATUS_LIST()
		{
			STATUS_LIST = GetBaseTableService.GetF000904List(FunctionCode, "P050302", "STATUS");
		}

		#endregion

		#region 貨主設定分類
		// 貨主自訂分類清單
		private List<NameValuePair<string>> _custCostList;
		public List<NameValuePair<string>> CUST_COST_LIST
		{
			get { return _custCostList; }
			set
			{
				_custCostList = value;
				RaisePropertyChanged("CUST_COST_LIST");
			}
		}

		// 取得貨主自訂分類清單
		public void GET_CUST_COST_LIST()
		{
			CUST_COST_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "CUST_COST");
		}

		//貨主自訂分類
		private string _custCost;
		public string CUST_COST
		{
			get { return _custCost; }
			set {
				_custCost = value;
				RaisePropertyChanged("CUST_COST");
				MOVE_OUT_TARGET_ENABLE = EDIT_ITEM.CUST_COST == "MoveOut"  ? true : false;
			}
		}
		
		#endregion

		#region 跨庫目的地
		// 跨庫目的清單
		private List<NameValuePair<string>> _moveOutTargetList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> MOVE_OUT_TARGET_LIST
		{
			get { return _moveOutTargetList; }
			set
			{
				_moveOutTargetList = value;
				RaisePropertyChanged("MOVE_OUT_TARGET_LIST");
			}
		}

		private List<NameValuePair<string>> _moveOutTargetShowList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> MOVE_OUT_TARGET_SHOW_LIST
		{
			get { return _moveOutTargetShowList; }
			set
			{
				_moveOutTargetShowList = value;
				RaisePropertyChanged("MOVE_OUT_TARGET_SHOW_LIST");
			}
		}

		// 取得跨庫目的下拉清單
		public void GET_MOVE_OUT_TARGET_LIST()
		{
			var proxy = GetProxy<F00Entities>();
			MOVE_OUT_TARGET_LIST = proxy.F0001s.Select(x => new NameValuePair<string>()
			{
				Name = x.CROSS_NAME,
				Value = x.CROSS_CODE
			}).ToList();
			MOVE_OUT_TARGET_SHOW_LIST.AddRange(MOVE_OUT_TARGET_LIST);
			
			// 下拉選單的跨庫目的地
			MOVE_OUT_TARGET_LIST.Insert(0, new NameValuePair<string> { Name = "全部", Value = "" });
			MOVE_OUT_TARGET = MOVE_OUT_TARGET_LIST.FirstOrDefault().Value;
		}

		// 所選擇的跨庫目的地
		private string _moveOutTarget;
		public string MOVE_OUT_TARGET
		{
			get { return _moveOutTarget; }
			set
			{
				_moveOutTarget = value;
				RaisePropertyChanged("MOVE_OUT_TARGET");
			}
		}

		// 編輯啟用跨庫目的地
		private bool _moveOutTargetEnable;
		public bool MOVE_OUT_TARGET_ENABLE
		{
			get { return _moveOutTargetEnable; }
			set
			{
				_moveOutTargetEnable = value;
				RaisePropertyChanged("MOVE_OUT_TARGET_ENABLE");
			}
		}
		#endregion

		#region 通路類型 CHANNEL_LIST
		private List<NameValuePair<string>> _channellist;

		public List<NameValuePair<string>> CHANNEL_LIST
		{
			get { return _channellist; }
			set
			{
				_channellist = value;
				RaisePropertyChanged("CHANNEL_LIST");
			}
		}

		private List<NameValuePair<string>> _subchannellist;

		public List<NameValuePair<string>> SUBCHANNEL_LIST
		{
			get { return _subchannellist; }
			set
			{
				_subchannellist = value;
				RaisePropertyChanged("SUBCHANNEL_LIST");
			}
		}

		public void GET_CHANNEL_LIST()
		{
			CHANNEL_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "CHANNEL");
			QueryChannelList = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "CHANNEL", true);
			SelectedChannel = QueryChannelList.First().Value;
		}

		public void GET_SUBCHANNEL_LIST()
		{
			SUBCHANNEL_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "SUBCHANNEL");
			QuerySubChannelList = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "SUBCHANNEL", true);
			SelectedSubChannel = QuerySubChannelList.First().Value;
		}

		#endregion

		#region 是否代收 COLLECT_LIST
		private List<NameValuePair<string>> _collectlist;

		public List<NameValuePair<string>> COLLECT_LIST
		{
			get { return _collectlist; }
			set
			{
				_collectlist = value;
				RaisePropertyChanged("COLLECT_LIST");
			}
		}

		public void GET_COLLECT_LIST()
		{
			COLLECT_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "COLLECT");
		}

		#endregion

		#region 性別 GENDER_LIST
		private List<NameValuePair<string>> _genderlist;

		public List<NameValuePair<string>> GENDER_LIST
		{
			get { return _genderlist; }
			set
			{
				_genderlist = value;
				RaisePropertyChanged("GENDER_LIST");
			}
		}

		public void GET_GENDER_LIST()
		{
			GENDER_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "GENDER");
		}

		#endregion

		#region 訂單類型 ORD_TYPE_LIST
		private List<NameValuePair<string>> _ordtypelist;

		public List<NameValuePair<string>> ORD_TYPE_LIST
		{
			get { return _ordtypelist; }
			set
			{
				_ordtypelist = value;
				RaisePropertyChanged("ORD_TYPE_LIST");
			}
		}

		public void GET_ORD_TYPE_LIST()
		{
			ORD_TYPE_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "ORD_TYPE");
		}

		#endregion

		#region POSM包裝量更新 POSM_LIST
		private List<NameValuePair<string>> _posmlist;

		public List<NameValuePair<string>> POSM_LIST
		{
			get { return _posmlist; }
			set
			{
				_posmlist = value;
				RaisePropertyChanged("POSM_LIST");
			}
		}

		public void GET_POSM_LIST()
		{
			POSM_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "POSM");
		}

		#endregion

		#region 特殊出貨 SP_DELV_LIST
		private List<NameValuePair<string>> _spdelvlist;

		public List<NameValuePair<string>> SP_DELV_LIST
		{
			get { return _spdelvlist; }
			set
			{
				_spdelvlist = value;
				RaisePropertyChanged("SP_DELV_LIST");
			}
		}

		public void GET_SP_DELV_LIST()
		{
			SP_DELV_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "SP_DELV");
		}

		#endregion

		#region 列印發票\收據 PRINT_RECEIPT_LIST
		private List<NameValuePair<string>> _printreceiptlist;

		public List<NameValuePair<string>> PRINT_RECEIPT_LIST
		{
			get { return _printreceiptlist; }
			set
			{
				_printreceiptlist = value;
				RaisePropertyChanged("PRINT_RECEIPT_LIST");
			}
		}

		public void GET_PRINT_RECEIPT_LIST()
		{
			PRINT_RECEIPT_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "PRINT_RECEIPT");
		}

		#endregion

		#region SA申裝書 SA_LIST
		private List<NameValuePair<string>> _salist;

		public List<NameValuePair<string>> SA_LIST
		{
			get { return _salist; }
			set
			{
				_salist = value;
				RaisePropertyChanged("SA_LIST");
			}
		}

		public void GET_SA_LIST()
		{
			SA_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "SA");
		}

		#endregion

		#region 配送資料 -選項 -編輯
		private SendTypeData _optType;

		public SendTypeData OptType
		{
			get { return _optType; }
			set
			{
				Set(() => OptType, ref _optType, value);

				if (EDIT_ITEM != null)
				{
					GET_ALLID_LIST(EDIT_ITEM.DC_CODE);
				}
				visEdit = ChangeVisibility(OptType);
				EDIT_ITEM.COLLECT = ChangeCollectId(OptType);
			}
		}

		#endregion

		#region 配送資料 -選項 -新增
		private SendTypeData _optTypeNew;

		public SendTypeData OptTypeNew
		{
			get { return _optTypeNew; }
			set
			{
				Set(() => OptTypeNew, ref _optTypeNew, value);
				if (NEW_ITEM != null)
				{
					if (!string.IsNullOrEmpty(NEW_ITEM.DC_CODE))
					{
						GET_ALLID_LIST(NEW_ITEM.DC_CODE);
						if (OptTypeNew == SendTypeData.ByVendor && !string.IsNullOrEmpty(NEW_ITEM.ALL_ID))
						{
							EService = GET_EService(NEW_ITEM.DC_CODE, NEW_ITEM.ALL_ID);
							NEW_ITEM.ESERVICE = EService.FirstOrDefault().Value;
						}
						NEW_ITEM.COLLECT = ChangeCollectId(OptTypeNew);
					}
				}
				visNew = ChangeVisibility(OptTypeNew);
			}
		}

		#endregion

		#region 編輯時 一開始的配送方式

		private SendTypeData _copyEditType;

		public SendTypeData CopyEditType
		{
			get { return _copyEditType; }
			set
			{
				Set(() => CopyEditType, ref _copyEditType, value);
			}
		}

		#endregion

		#region 超取服務商
		private List<NameValuePair<string>> _EService;

		public List<NameValuePair<string>> EService
		{
			get { return _EService; }
			set
			{
				_EService = value;
				RaisePropertyChanged("EService");
			}
		}

		private List<NameValuePair<string>> GET_EService(string dcCode, string allId)
		{
			var proxy19 = GetProxy<F19Entities>();
			var f190001 = (from p in proxy19.F194713s
										 where p.CUST_CODE == _custCode && p.DC_CODE == dcCode && p.GUP_CODE == _gupCode
										 && p.ALL_ID == allId
										 select p)
										 .Select(o => new NameValuePair<string>
										 {
											 Name = o.ESERVICE_NAME,
											 Value = o.ESERVICE
										 }).ToList(); //用冒號分開ALL_ID 跟 ESERVICE 在存取的時候.split

			return f190001;
		}

		#endregion

		#region 建議出貨包裝線類型
		private List<NameValuePair<string>> _packingTypeList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> PACKING_TYPE_LIST
		{
			get { return _packingTypeList; }
			set
			{
				_packingTypeList = value;
				RaisePropertyChanged("PACKING_TYPE_LIST");
			}
		}

		// 所選擇的建議出貨包裝線類型
		private string _packingType;
		public string PACKING_TYPE
		{
			get { return _packingType; }
			set
			{
				_packingType = value;
				RaisePropertyChanged("PACKING_TYPE");
			}
		}

		public void GET_PACKING_TYPE_LIST()
		{
			var proxy = GetProxy<F00Entities>();
			PACKING_TYPE_LIST = GetBaseTableService.GetF000904List(this.FunctionCode, "F050101", "PACKING_TYPE");
			SelectedPackingType = PACKING_TYPE_LIST.First().Value;
		}

		private string _selectedPackingType;
		public string SelectedPackingType
		{
			get { return _selectedPackingType; }
			set
			{
				_selectedPackingType = value;
				RaisePropertyChanged("SelectedPackingType");
			}
		}
		#endregion

		public void SetOrdTypeForB2BAdd()
		{
			if (NEW_ITEM.ORD_TYPE != "0")
			{
				NEW_ITEM.ORD_TYPE = "0";
				GET_TRAN_CODE_LIST();
				//新增資料時 初始資料
				if (TYPEID_LIST == null || !TYPEID_LIST.Any()) GET_TYPEID_LIST(DC_CODE, "1");
			}
		}

		public void SetOrdTypeForB2BEdit()
		{
			if (EDIT_ITEM.ORD_TYPE != "0")
			{
				EDIT_ITEM.ORD_TYPE = "0";
				GET_TRAN_CODE_LIST();
			}
		}

		private void ClearData()
		{
			if (serialNo_Data != null) //序號
				serialNo_Data = null;
			if (EDIT_ITEM != null)
				EDIT_ITEM = null;
			if (NEW_ITEM != null)
				NEW_ITEM = null;
			if (SELECTED_F050101 != null)
				SELECTED_F050101 = null;
			dgOrdMainList = null;
			dgOrdDetailList_Display = null;
			dgOrdDetailList_Insert = null;
			dgShipList = null;
			//TRAN_CODE_LIST = new List<NameValuePair<string>>();
			CONSIGN_NO = string.Empty;
			DELV_RETAIL_CODE = string.Empty;
			DELV_RETAIL_NAME = string.Empty;
			ClearSearchProduct();

		}

		private void ReGenTranCode(string ordType)
		{
			GET_TRAN_CODE_LIST();
			//GET_TRAN_CODE_LIST(EDIT_ITEM.ORD_TYPE);
			//NEW_ITEM.TRAN_CODE = EDIT_ITEM.TRAN_CODE;
			//RaisePropertyChanged(EDIT_ITEM.TRAN_CODE);
		}

		private bool CanImport()
		{
			bool hasFile = false;
			if (!string.IsNullOrEmpty(FullPath))
			{
				var file = new FileInfo(FullPath);
				hasFile = file.Exists;
			}
			return hasFile;
		}

		private void Gen_dgOrdDetailList_Insert()
		{
			//if (dgOrdDetailList_Display.Where(x => x.BUNDLE_SERIALLOC == "1").FirstOrDefault() != null && (serialNo_Data == null || serialNo_Data.Count == 0))
			//{
			//	DialogService.ShowMessage("訂單明細中有序號綁儲存商品，請匯入序號");
			//	isSave = false;
			//	continueSave = false;
			//	return;
			//}
			//else
			//{
			if (serialNo_Data != null)
			{
				foreach (var sd in serialNo_Data) { sd.USED = "0"; } //Saving process fail in SaveValidation , reset serialNo_Data.USED="0"
			}
			if (dgOrdDetailList_Insert != null)
				dgOrdDetailList_Insert = null;
			dgOrdDetailList_Insert = new List<F050102Ex>();
			int ord_seq = 1;
			foreach (var d in dgOrdDetailList_Display.OrderBy(x => x.ORD_SEQ))
			{
				//2021/01/19 Scott 指定序號功能目前沒開放，若開發再把這段取消註解(但會有bug，這裡的ORD_SEQ 不能從1開始編，後面存檔會有問題)
				//if (d.BUNDLE_SERIALNO == "1" && serialNo_Data != null && serialNo_Data.Any())
				//{
				//    var ord_qty = d.ORD_QTY;
				//    for (int i = 1; i <= ord_qty; i++)
				//    {
				//        F050102Ex f050102 = ExDataMapper.Clone(d);
				//        f050102.ORD_QTY = 1;
				//        f050102.ORD_SEQ = ord_seq.ToString();
				//        var tmp = serialNo_Data.Where(x => x.ITEM_CODE == d.ITEM_CODE && x.USED == "0").FirstOrDefault();
				//        if (tmp != null)
				//        {
				//            tmp.USED = "1";
				//            f050102.SERIAL_NO = tmp.SERIAL_NO;
				//        }
				//        ord_seq++;
				//        dgOrdDetailList_Insert.Add(f050102);
				//    }
				//}
				//else
				//{
				F050102Ex f050102 = ExDataMapper.Clone(d);
				//改由原ORD_SEQ 新增時此為空 由後端產生
				//f050102.ORD_SEQ = ord_seq.ToString();
				//ord_seq++;
				dgOrdDetailList_Insert.Add(f050102);
				//}
			}
			//}
		}

		//編輯時取得已匯入的序號
		private void GetSerialNoFromDB()
		{
			var proxy05 = GetProxy<F05Entities>();
			var f050102First = dgOrdDetailList_Display.FirstOrDefault();
			List<F050102> f050102List;

			f050102List = proxy05.F050102s.Where(x => x.ORD_NO == EDIT_ITEM.ORD_NO && x.GUP_CODE == EDIT_ITEM.GUP_CODE && x.DC_CODE == EDIT_ITEM.DC_CODE && x.CUST_CODE == EDIT_ITEM.CUST_CODE && x.SERIAL_NO != null).ToList();
			serialNo_Data = new List<serialNoField>();
			foreach (var t in f050102List)
			{
				var serialNo = new serialNoField()
				{
					ITEM_CODE = t.ITEM_CODE,
					SERIAL_NO = t.SERIAL_NO,
					USED = "0"
				};
				serialNo_Data.Add(serialNo);
			}
			if (serialNo_Data != null && serialNo_Data.Count > 0)
				CanEditDetailQty = "0";
		}

		private void ClearSearchProduct()
		{
			ITEM_CODE = "";
			ITEM_COLOR = "";
			ITEM_NAME = "";
			ITEM_SPEC = "";
			ITEM_SIZE = "";
			ORD_QTY = 0;
			SERIAL_NO = "";
			MAKE_NO = "";
		}

		private void DoDistrCar(OperateMode orginalOpMode, F050101Ex f050101Ex, List<F050102Ex> f050102Exs)
		{
			if (f050101Ex.SPECIAL_BUS == "0" && _original_DISTR_CAR == "1")
			{
				if (orginalOpMode == OperateMode.Edit && !string.IsNullOrEmpty(f050101Ex.DISTR_CAR_NO))
				{
					var wcf70Proxy = new wcf70.P70WcfServiceClient();
					var result = RunWcfMethod<wcf70.ExecuteResult>(wcf70Proxy.InnerChannel, () => wcf70Proxy.DeleteF700101ByDistrCarNo(f050101Ex.DISTR_CAR_NO, f050101Ex.DC_CODE));
				}
			}
		}

		private bool SaveValidation()
		{
			var tmpTypeId = UserOperateMode == OperateMode.Add ? NEW_ITEM.TYPE_ID : EDIT_ITEM.TYPE_ID;
			if (string.IsNullOrEmpty(tmpTypeId))
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_TmpTypeIdIsNull);
				return false;
			}
			if (dgOrdDetailList_Display.Where(x => x.ORD_QTY <= 0).FirstOrDefault() != null)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_QtyZero);
				return false;
			}

			DateTime ORD_DATE = UserOperateMode == OperateMode.Add ? (DateTime)NEW_ITEM.ORD_DATE : (DateTime)EDIT_ITEM.ORD_DATE;
			DateTime ARRIVAL_DATE = UserOperateMode == OperateMode.Add ? (DateTime)NEW_ITEM.ARRIVAL_DATE : (DateTime)EDIT_ITEM.ARRIVAL_DATE;

			TimeSpan s = new TimeSpan(ARRIVAL_DATE.Ticks - ORD_DATE.Ticks);
			if (s.TotalDays > 7)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_ArrivalDateOver);
				return false;
			}

			var ORD_TYPE = UserOperateMode == OperateMode.Add ? NEW_ITEM.ORD_TYPE : EDIT_ITEM.ORD_TYPE;
			var RETAIL_CODE = UserOperateMode == OperateMode.Add ? NEW_ITEM.RETAIL_CODE : EDIT_ITEM.RETAIL_CODE;
			if (ORD_TYPE == "0" && string.IsNullOrEmpty(RETAIL_CODE))
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_OrderTypeB2BInputCustCode);
				return false;
			}
			if (!string.IsNullOrEmpty(RETAIL_CODE) && ORD_TYPE != "0")
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_OrderTypeB2BError);
				return false;
			}

			var POSM = UserOperateMode == OperateMode.Add ? NEW_ITEM.POSM : EDIT_ITEM.POSM;
			var BATCH_NO = UserOperateMode == OperateMode.Add ? NEW_ITEM.BATCH_NO : EDIT_ITEM.BATCH_NO;
			if (POSM == "1" && string.IsNullOrEmpty(BATCH_NO))
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_BatchNoInput);
				return false;
			}

			var address = UserOperateMode == OperateMode.Add ? NEW_ITEM.ADDRESS : EDIT_ITEM.ADDRESS;
			var sp_delv = UserOperateMode == OperateMode.Add ? NEW_ITEM.SP_DELV : EDIT_ITEM.SP_DELV;
			if (sp_delv == "02" && address != DC_ADDRESS)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_AddressNotEqualDCAddress);
				return false;
			}

			if (string.IsNullOrEmpty(address))
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_OutAddressIsNull);
				return false;
			}

			string age = UserOperateMode == OperateMode.Add ? NEW_ITEM.AGE.ToString() : EDIT_ITEM.AGE.ToString();
			if (!string.IsNullOrEmpty(age) && age.Length > 3)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_AgeFormatError);
				return false;
			}
			string collectType = UserOperateMode == OperateMode.Add ? NEW_ITEM.COLLECT : EDIT_ITEM.COLLECT;
			int collectAmt = UserOperateMode == OperateMode.Add ? Convert.ToInt32(NEW_ITEM.COLLECT_AMT) : Convert.ToInt32(EDIT_ITEM.COLLECT_AMT);

			if (collectType == "0" && collectAmt != 0)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_CollectTypeIsNoAmtNotZero);
				return false;
			}
			else if (collectType == "1" && collectAmt <= 0)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_CollectTypeIsYesAmtZero);
				return false;
			}
			//選超取，要輸入配送門市編號.配送門市名稱.配送編號.超取服務商
			if ((UserOperateMode == OperateMode.Edit && OptType == SendTypeData.ByVendor) ||
					(UserOperateMode == OperateMode.Add && OptTypeNew == SendTypeData.ByVendor))
			{
				string tmpEservice = UserOperateMode == OperateMode.Add ? (selectEService != null ? selectEService.Value : string.Empty) : EDIT_ITEM.ESERVICE;
				string tmpRetailCode = UserOperateMode == OperateMode.Add ? DELV_RETAIL_CODE_NEW : DELV_RETAIL_CODE;
				string tmpRetailName = UserOperateMode == OperateMode.Add ? DELV_RETAIL_NAME_NEW : DELV_RETAIL_NAME;
				string tmpConsignNo = UserOperateMode == OperateMode.Add ? CONSIGN_NO_New : CONSIGN_NO;
				string tmpBatchNo = UserOperateMode == OperateMode.Add ? NEW_ITEM.BATCH_NO : EDIT_ITEM.BATCH_NO;
				if (string.IsNullOrEmpty(tmpRetailCode))
				{
					ShowWarningMessage(Properties.Resources.P0503020000_RetailCodeIsNull);
					return false;
				}
				if (string.IsNullOrEmpty(tmpRetailName))
				{
					ShowWarningMessage(Properties.Resources.P0503020000_RetailNameIsNull);
					return false;
				}
				if (string.IsNullOrEmpty(tmpConsignNo))
				{
					ShowWarningMessage(Properties.Resources.P0503020000_ConsignNoIsNull);
					return false;
				}

				if (string.IsNullOrEmpty(tmpEservice))
				{
					ShowWarningMessage(Properties.Resources.P0503020000_EserviceIsNull);
					return false;
				}
				if (string.IsNullOrEmpty(tmpBatchNo))
				{
					ShowWarningMessage(Properties.Resources.P0503020000_BatchNoIsNull);
					return false;
				}
			}

			if ((UserOperateMode == OperateMode.Edit && OptType == SendTypeData.ByHouse) ||
					(UserOperateMode == OperateMode.Add && OptTypeNew == SendTypeData.ByHouse))
			{
				string tmpAllID = UserOperateMode == OperateMode.Add ? NEW_ITEM.ALL_ID : EDIT_ITEM.ALL_ID;

				if (string.IsNullOrEmpty(tmpAllID))
				{
					ShowWarningMessage(Properties.Resources.P0503020000_AllIDIsNull);
					return false;
				}
			}
			string custName = UserOperateMode == OperateMode.Add ? NEW_ITEM.CUST_NAME : EDIT_ITEM.CUST_NAME;
			if (string.IsNullOrEmpty(custName))
			{
				ShowWarningMessage(Properties.Resources.P0503020000_CustNameIsNull);
				return false;
			}
			var message = new List<string>();
			if (EDIT_ITEM != null && EDIT_ITEM.STATUS == "0")
			{
				//檢查資料是否還有填Properties.Resources.P0503020000_Input字樣
				foreach (PropertyInfo pi in typeof(F050101Ex).GetProperties())
				{
					if (pi.CanRead && pi.CanWrite && pi.PropertyType == typeof(string))
					{
						var value = (string)pi.GetValue(EDIT_ITEM);
						if (value != null && value == Properties.Resources.P0503020000_Input)
						{
							ShowWarningMessage(Properties.Resources.P0503020000_InputDataLack);
							return false;
						}
					}
				}
				foreach (var detail in dgOrdDetailList_Display)
				{

					if (detail.ITEM_CODE.StartsWith("XXX"))
					{
						message.Add(string.Format(Properties.Resources.P0503020000_ImportOrderItemError, detail.ITEM_CODE));
					}
					else
					{
						var f1903 = FindF1903(detail.ITEM_CODE);
						if (f1903 != null && f1903.STOP_DATE.HasValue && f1903.STOP_DATE <= DateTime.Today)
							message.Add(Properties.Resources.P0503020000_ItemStopSelling);

					}
				}
				
			}

			var item = UserOperateMode == OperateMode.Add ? NEW_ITEM : EDIT_ITEM;
			if (item != null)
			{
				if (item.CUST_COST != "MoveOut" && dgOrdDetailList_Display.Any(x => !string.IsNullOrWhiteSpace(x.MAKE_NO)))
				{
					message.Add(Properties.Resources.P0503020000_MakeNoError);
				}
				// 判斷是否重複項目
				bool repeatItemCode = dgOrdDetailList_Display.GroupBy(x => x.ITEM_CODE).Where(g => g.Count() > 1).Count() > 0;
				if (item.CUST_COST != "MoveOut" && repeatItemCode)
				{
					message.Add(Properties.Resources.P0503020000_DetailItmeCodeIsRepect);
				}

				// 判斷指定出貨批號
				bool repeatMakeNo = dgOrdDetailList_Display.GroupBy(x => new { x.ITEM_CODE,x.MAKE_NO }).Where(g => g.Count() > 1).Count() > 0;
				if (repeatMakeNo)
				{
					message.Add(Properties.Resources.P0503020000_DetailMakeNoIsRepect);
				}
			}
			if (message.Any())
			{
				ShowWarningMessage(string.Join("\r\n", message));
				return false;
			}


			if (((UserOperateMode == OperateMode.Edit && EDIT_ITEM.POSM == "1") ||
					(UserOperateMode == OperateMode.Add && NEW_ITEM.POSM == "1")))
			{
				ShowWarningMessage(Properties.Resources.P0503020000_SerialBatchError);
				return true;
			}


			return true;
		}

		#region 編輯 新增時 物件變更觸發事件方法

		private void EDIT_ITEM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_editItem.PropertyChanged -= EDIT_ITEM_PropertyChanged;
			string tmpEService = _editItem.ESERVICE;
			string tmpCollect = _editItem.COLLECT;

			switch (e.PropertyName)
			{
				case nameof(EDIT_ITEM.ESERVICE):
					if (!string.IsNullOrEmpty(tmpEService))
					{
						EDIT_ITEM.ESERVICE = tmpEService;
					}
					break;
				case nameof(EDIT_ITEM.DC_CODE):
					if (EDIT_ITEM.DC_CODE != null)
					{
						EService = GET_EService(EDIT_ITEM.DC_CODE, EDIT_ITEM.ALL_ID);
						SetAllIdRoundPiece(EDIT_ITEM.DC_CODE, EDIT_ITEM.ALL_ID);
					}
					break;
				case nameof(EDIT_ITEM.ALL_ID):
					if (EDIT_ITEM.ALL_ID != null)
					{
						EService = GET_EService(EDIT_ITEM.DC_CODE, EDIT_ITEM.ALL_ID);
						SetAllIdRoundPiece(EDIT_ITEM.DC_CODE, EDIT_ITEM.ALL_ID);
					}
					break;
				case nameof(EDIT_ITEM.CUST_COST):
					if (EDIT_ITEM.CUST_COST != null)
					{
						if (EDIT_ITEM.CUST_COST == "MoveOut" && UserOperateMode != OperateMode.Query)
						{
							MOVE_OUT_TARGET_ENABLE = true;
							EDIT_ITEM.MOVE_OUT_TARGET = MOVE_OUT_TARGET_SHOW_LIST.First().Value;
							MAKE_NO_ENABLE = true;
						}
						else
						{
							MOVE_OUT_TARGET_ENABLE = false;
							EDIT_ITEM.MOVE_OUT_TARGET = null;
							MAKE_NO = null;
							MAKE_NO_ENABLE = false;
						}
					}
					break;
			}
			_editItem.PropertyChanged += EDIT_ITEM_PropertyChanged;
		}

		private void NEW_ITEM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_newitem.PropertyChanged -= NEW_ITEM_PropertyChanged;
			switch (e.PropertyName)
			{
				case nameof(NEW_ITEM.DC_CODE):
					if (NEW_ITEM.DC_CODE != null)
					{
						EService = GET_EService(NEW_ITEM.DC_CODE, NEW_ITEM.ALL_ID);
						SetAllIdRoundPiece(NEW_ITEM.DC_CODE, NEW_ITEM.ALL_ID);
					}
					break;
				case nameof(NEW_ITEM.ALL_ID):
					if (NEW_ITEM.ALL_ID != null)
					{
						EService = GET_EService(NEW_ITEM.DC_CODE, NEW_ITEM.ALL_ID);
						SetAllIdRoundPiece(NEW_ITEM.DC_CODE, NEW_ITEM.ALL_ID);
					}
					break;
				case nameof(NEW_ITEM.CUST_COST):
					if (NEW_ITEM.CUST_COST != null)
					{
						if(NEW_ITEM.CUST_COST == "MoveOut" && UserOperateMode != OperateMode.Query)
						{
							MOVE_OUT_TARGET_ENABLE = true;
							NEW_ITEM.MOVE_OUT_TARGET = MOVE_OUT_TARGET_SHOW_LIST.First().Value;
							MAKE_NO_ENABLE = true;
						}
						else
						{
							MOVE_OUT_TARGET_ENABLE = false;
							NEW_ITEM.MOVE_OUT_TARGET = null;
							MAKE_NO = null;
							MAKE_NO_ENABLE = false;
						}
						
					}
					break;
			}
			_newitem.PropertyChanged += NEW_ITEM_PropertyChanged;
		}

		#endregion

		#region  加入 配送資料操作調整，操作上要整合與防呆

		#region  編輯=> 配送門市編號	&& 配送門市名稱
		private string _delvRetailCode;

		public string DELV_RETAIL_CODE
		{
			get { return _delvRetailCode; }
			set
			{
				Set(() => DELV_RETAIL_CODE, ref _delvRetailCode, value);
			}
		}

		private string _delvRetailName;

		public string DELV_RETAIL_NAME
		{
			get { return _delvRetailName; }
			set
			{
				Set(() => DELV_RETAIL_NAME, ref _delvRetailName, value);
			}
		}

		#endregion

		#region 新增=> 配送門市編號	&& 配送門市名稱

		private string _delvRetailCodeNew;

		public string DELV_RETAIL_CODE_NEW
		{
			get { return _delvRetailCodeNew; }
			set
			{
				Set(() => DELV_RETAIL_CODE_NEW, ref _delvRetailCodeNew, value);
			}
		}

		private string _delvRetailNameNew;

		public string DELV_RETAIL_NAME_NEW
		{
			get { return _delvRetailNameNew; }
			set
			{
				Set(() => DELV_RETAIL_NAME_NEW, ref _delvRetailNameNew, value);
			}
		}

		#endregion

		private string ChangeCollectId(SendTypeData opt)
		{
			switch (opt)
			{
				case SendTypeData.BySelf:
					return "0";
					break;

				default:
					return "1";
			}
		}

		public void ChangeSendType(F050101Ex itemList, SendTypeData opt)
		{
			switch (opt)
			{
				case SendTypeData.ByHouse: //宅配
					itemList.ALL_ID = itemList.ALL_ID; //711 or family
					itemList.SPECIAL_BUS = "0";
					itemList.SELF_TAKE = "0";
					itemList.CVS_TAKE = "0";
					break;

				case SendTypeData.BySelf: //自取
					itemList.SPECIAL_BUS = "0";
					itemList.SELF_TAKE = "1";
					itemList.CVS_TAKE = "0";
					break;

				case SendTypeData.ByVendor: //超取
					itemList.SPECIAL_BUS = "0";
					itemList.SELF_TAKE = "0";
					itemList.CVS_TAKE = "1";
					break;
			}
		}

		public Visibility ChangeVisibility(SendTypeData opt)
		{
			switch (opt)
			{
				case SendTypeData.BySelf:
					return Visibility.Collapsed;

				default:
					return Visibility.Visible;
			}
		}

		private Visibility _visEdit = Visibility.Collapsed;

		public Visibility visEdit
		{
			get { return _visEdit; }
			set
			{
				Set(() => visEdit, ref _visEdit, value);
			}
		}

		private Visibility _visNew = Visibility.Collapsed;

		public Visibility visNew
		{
			get { return _visNew; }
			set
			{
				Set(() => visNew, ref _visNew, value);
			}
		}

		#region 配送編號
		private string _consignNo;

		public string CONSIGN_NO
		{
			get { return _consignNo; }
			set
			{
				Set(() => CONSIGN_NO, ref _consignNo, value);
			}
		}

		#endregion

		#region 新增-配送編號

		//新增配送編號
		private string _consignNoNew;

		public string CONSIGN_NO_New
		{
			get { return _consignNoNew; }
			set
			{
				Set(() => CONSIGN_NO_New, ref _consignNoNew, value);
			}
		}

		#endregion

		#endregion


		#region BatchApprove 批次核准
		/// <summary>
		/// Gets the BatchApprove.
		/// </summary>
		public ICommand BatchApproveCommand
		{
			get
			{
				var message = string.Empty;
				return CreateBusyAsyncCommand(
								o => DoBatchApprove(ref message), () => UserOperateMode == OperateMode.Query, o =>
								{
									if (!string.IsNullOrWhiteSpace(message))
									{
										var viewer = new ScrollViewer
										{
											FontSize = 20,
											Content = new TextBox
											{
												Text = message
											},
											HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
										};
										var win = new Window { Content = viewer };
										win.Title = Properties.Resources.P0503020000_BatchApprove;
										win.ShowDialog();
									}
									if (dgOrdMainList != null && dgOrdMainList.Any())
										SearchCommand.Execute(null);
								}
);
			}
		}

		public void DoBatchApprove(ref string message)
		{
			if (ShowConfirmMessage(Properties.Resources.P0503020000_BatchApproveAsk) == DialogResponse.No)
				return;

			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod<wcf.ExecuteResult>(w => w.BatchApproveF050101(_gupCode, _custCode));
			message = result.Message;

		}
		#endregion BatchApprove

		public ICommand ExcelImportCommand
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
			//    get
			//    {
			//        return CreateBusyAsyncCommand(
			//        o => DoExcelImport(), () => UserOperateMode == OperateMode.Query
			//);
			//    }
		}

		public ICommand DoImportCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => { DoExcelImport(); },
						() => UserOperateMode == OperateMode.Query
						);
			}
		}

		public void DoExcelImport()
		{
			string fullFilePath = ImportFilePath;
			string errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);

			if (!CheckExcelData(excelTable, errorMeg))
				return;

			#region 將轉入的Excel轉出對應欄位名稱
			var excelList = excelTable.Select().ToList().Select(o => new
			{
				DC_CODE = o[0].ToString(),
				CUST_ORD_NO = o[1].ToString(),
				ARRIVAL_DATE = o[2].ToString(),
				ORDER_TYPE = o[3].ToString(),
				RETAIL_CODE = o[4].ToString(),
				CUST_NAME = o[5].ToString(),
				CONTACT_TEL = o[6].ToString(),
				CONSIGNEE = o[7].ToString(),
				TEL = o[8].ToString(),
				ADDRESS = o[9].ToString(),
				COLLECT = o[10].ToString(),
				DELV_WAY = o[11].ToString(),
				ALL_ID = o[12].ToString(),
				MEMO = o[13].ToString(),
				ITEM_CODE = o[14].ToString(),
				ORD_QTY = o[15].ToString()
			}).ToList();
			var masterData = excelList.GroupBy(o => new { o.DC_CODE, o.RETAIL_CODE, o.ARRIVAL_DATE, o.ORDER_TYPE, o.CUST_ORD_NO, o.CUST_NAME, o.CONTACT_TEL, o.CONSIGNEE, o.TEL, o.ADDRESS, o.COLLECT, o.DELV_WAY, o.ALL_ID, o.MEMO }).ToList();
			var orderNoGroup = excelList.Select(o => o.CUST_ORD_NO).Distinct().ToList();
			#endregion

			var f050101List = new List<F050101>();
			var f050102List = new List<F050102Excel>();
			foreach (var g in orderNoGroup)
			{
				if (masterData.Where(o => o.Key.CUST_ORD_NO == g).Count() > 1)
				{
					ShowWarningMessage(Properties.Resources.P050302_ExcelDataRepeat);
					return;
				}
				DateTime arrivalDate = DateTime.Today;

				#region 檢核 B2B與B2C條件必填欄位
				var mData = masterData.Select(o => o.Key).ToList().FirstOrDefault(o => o.CUST_ORD_NO == g);
				var contact = string.Empty;
				var proxy19 = GetProxy<F19Entities>();
				var f1910 = (from p in proxy19.F1910s
										 where CUST_CODE.Equals(CUST_CODE)
										 select p).ToList();

				if (mData.ORDER_TYPE == "B2B")
				{
					if (string.IsNullOrWhiteSpace(mData.RETAIL_CODE))
					{
						ShowWarningMessage(Properties.Resources.P050302_B2BRetailCodeIsNull);
						return;
					}
					contact = f1910.Where(o => o.RETAIL_CODE == mData.RETAIL_CODE).Select(x => x.CONTACT).FirstOrDefault();
					if (!contact.Any())
					{
						ShowWarningMessage(Properties.Resources.P050302_B2BRetailCodeIsNull);
						return;
					}
				}
				else
				{
					if (string.IsNullOrWhiteSpace(mData.CUST_NAME))
					{
						ShowWarningMessage(Properties.Resources.P050302_B2CCustNameIsNull);
						return;
					}
					contact = mData.CUST_NAME;
				}
				#endregion

				#region 新增F050101 資料
				var f050101 = new F050101
				{
					ADDRESS = mData.ADDRESS,
					ALL_ID = mData.ALL_ID,
					ARRIVAL_DATE = arrivalDate,
					CUST_ORD_NO = mData.CUST_ORD_NO,
					COLLECT = mData.COLLECT == Properties.Resources.P050302_ExcelCollectYes ? "1" : "0",
					CONTACT_TEL = mData.CONTACT_TEL,
					DC_CODE = mData.DC_CODE,
					GUP_CODE = _gupCode,
					CUST_CODE = CUST_CODE,
					CUST_NAME = mData.ORDER_TYPE == "B2B" ? string.IsNullOrWhiteSpace(mData.CUST_NAME) ? f1910.FirstOrDefault(o => o.RETAIL_CODE == mData.RETAIL_CODE).RETAIL_NAME : mData.CUST_NAME : mData.CUST_NAME,
					CONSIGNEE = mData.CONSIGNEE,
					ORD_TYPE = mData.ORDER_TYPE == "B2B" ? "0" : "1",
					RETAIL_CODE = mData.RETAIL_CODE,
					TEL = mData.ORDER_TYPE == "B2B" ? string.IsNullOrWhiteSpace(mData.TEL.Trim()) ? f1910.FirstOrDefault(o => o.RETAIL_CODE == mData.RETAIL_CODE).TEL : mData.TEL : mData.TEL,
					MEMO = mData.MEMO,
					STATUS = "0",
					FRAGILE_LABEL = "0",
					GUARANTEE = "0",
					SA = "0",
					GENDER = "0",
					CHANNEL = "00",
					POSM = "0",
					CONTACT = contact,
					SPECIAL_BUS = "0",
					PRINT_RECEIPT = "0",
					HAVE_ITEM_INVO = "0",
					NP_FLAG = "0",
					SA_CHECK_QTY = 0,
					DELV_PERIOD = "4",
					CVS_TAKE = "0",
					SUBCHANNEL = "00",
					TRAN_CODE = mData.ORDER_TYPE == "B2B" ? "O1" : "O3",
					TYPE_ID = "G",
					ORD_DATE = DateTime.Today,
					CAN_FAST = "0",
					SELF_TAKE = mData.DELV_WAY == Properties.Resources.BySelf ? "1" : "0",
					TEL_1 = mData.TEL,
					SP_DELV = "00",
					ROUND_PIECE = "0"
				};
				f050101List.Add(f050101);
				#endregion

				#region 新增F050102 資料
				int seq = 1;
				var excels = excelList.Where(o => o.CUST_ORD_NO == mData.CUST_ORD_NO).ToList();
				foreach (var excel in excels)
				{
					var f050102 = new F050102Excel
					{
						DC_CODE = mData.DC_CODE,
						GUP_CODE = _gupCode,
						ITEM_CODE = excel.ITEM_CODE,
						CUST_CODE = CUST_CODE,
						ORD_QTY = Convert.ToInt32(excel.ORD_QTY),
						NO_DELV = "0",
						ORD_SEQ = seq.ToString(),
						CUST_ORD_NO = mData.CUST_ORD_NO
					};
					f050102List.Add(f050102);
					seq++;
				}
				#endregion
			}
			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();

			var wcfF050101 = ExDataMapper.MapCollection<F050101, wcf.F050101>(f050101List).ToArray();
			var wcfF050102 = ExDataMapper.MapCollection<F050102Excel, wcf.F050102Excel>(f050102List).ToArray();
			var result = proxy.RunWcfMethod(w => w.InsertExcelOrder(wcfF050101, wcfF050102));
			ShowInfoMessage(Messages.InfoImportSuccess.Message);
		}

		/// <summary>
		/// 檢核Excel訂單匯入欄位
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="errorMeg"></param>
		/// <returns></returns>
		private bool CheckExcelData(DataTable dt, string errorMeg)
		{
			//沒有資料判斷
			if (dt == null)
			{
				ShowWarningMessage((string.IsNullOrEmpty(errorMeg)) ? Properties.Resources.P050302_NoData : errorMeg);
				return false;
			}
			//欄位數必須為16
			if (dt.Columns.Count != 16)
			{
				ShowWarningMessage(Properties.Resources.P050302_ExcelFormatError);
				return false;
			}
			List<DataRow> drs = dt.Select().ToList();

			for (int i = 0; i < dt.Columns.Count; i++)
			{
				// 2 = 指定到貨日期(非必填) 4 = 客戶代號(有條件必填) 5 = 客戶名稱(有條件必填) 13 = 注意事項(非必填)
				if (i == 2 || i == 4 || i == 5 || i == 13) continue;
				foreach (var dr in drs)
				{
					if (i == 3)
					{
						//訂單類型必須為B2B或B2C
						if (dr[i].ToString() != "B2B" && dr[i].ToString() != "B2C")
						{
							ShowWarningMessage(Properties.Resources.P050304_ExcelOrderTypeError);
							return false;
						}
					}
					if (i == 11)
					{
						//配送方式只有自取跟宅配
						if (dr[i].ToString() != Properties.Resources.ByVendor && dr[i].ToString() != Properties.Resources.BySelf)
						{
							ShowWarningMessage(Properties.Resources.P050304_DelvWay);
							return false;
						}
					}
					if (string.IsNullOrWhiteSpace(dr[i].ToString()))
					{
						//自取不會有配送商所以不檢查
						if (i == 12 && dr[11].ToString() == Properties.Resources.BySelf)
							continue;

						ShowWarningMessage(string.Format(Properties.Resources.P050302_CheckExcelNull, dt.Columns[i].ColumnName));
						return false;
					}

					if (i == 15)
					{
						//訂購數必須大於0
						if (Convert.ToInt32(dr[i]) <= 0)
						{
							ShowWarningMessage(string.Format(Properties.Resources.P050302_CheckExcelNull, dt.Columns[i].ColumnName));
							return false;
						}
					}
				}
			}
			return true;
		}

		#region EditItem  修改明細
		/// <summary>
		/// Gets the EditItem.
		/// </summary>
		public ICommand EditItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
								o => DoEditItem(), () => UserOperateMode != OperateMode.Query && selected_OrderDetail != null && selected_OrderDetail.BUNDLE_SERIALLOC == "0"
);
			}
		}

		public void DoEditItem()
		{
			if (_selected_orderdetail != null)
			{
				if (CheckDetail(false))
				{
					_selected_orderdetail.ITEM_CODE = ITEM_CODE;
					_selected_orderdetail.ITEM_NAME = ITEM_NAME;
					_selected_orderdetail.ITEM_COLOR = ITEM_COLOR;
					_selected_orderdetail.ITEM_SIZE = ITEM_SIZE;
					_selected_orderdetail.ITEM_SPEC = ITEM_SPEC;
					_selected_orderdetail.ORD_QTY = ORD_QTY;
					_selected_orderdetail.MAKE_NO = MAKE_NO;
					
				}
			}
		}
		#endregion EditItem

		#region 新增/修改商品明細檢核
		private bool CheckDetail(bool isAdd)
		{
			var detail = dgOrdDetailList_Display;
			var excludeItemCode = (isAdd) ? "" : _selected_orderdetail.ITEM_CODE;

			var custCost = (UserOperateMode == OperateMode.Add) ? NEW_ITEM.CUST_COST : EDIT_ITEM.CUST_COST;
			if (custCost != "MoveOut")
			{
				//檢查商品編號重覆
				if (dgOrdDetailList_Display.Any(x => x.ITEM_CODE != excludeItemCode && x.ITEM_CODE.Equals(ITEM_CODE)))
				{
					DialogService.ShowMessage(Properties.Resources.P0503020000_ItemRepeat);
					return false;
				}
			}

			// 檢查批號是重複
			var isRepeatMakeNo = dgOrdDetailList_Display.Any(x => x.MAKE_NO == MAKE_NO && x.ITEM_CODE == ITEM_CODE);
			if (!string.IsNullOrWhiteSpace(MAKE_NO) && isRepeatMakeNo)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_DetailMakeNoIsRepect);
				return false;
			}

			//檢查訂購數量
			int number1 = 0;
			bool canConvert = int.TryParse(ORD_QTY.ToString(), out number1);
			if (canConvert == false)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_QtyFormatError);
				return false;
			}
			if (ORD_QTY <= 0)
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_QtyFormatError);
				return false;
			}

			var f1903 = FindF1903(ITEM_CODE);
			//檢查商品停售日期
			if (f1903.STOP_DATE.HasValue && f1903.STOP_DATE <= DateTime.Today)
			{
				ShowWarningMessage(Properties.Resources.P0503020000_CheckItemStopSelling);
				return false;
			}

			//檢查POSM
			var posm = (UserOperateMode == OperateMode.Add) ? NEW_ITEM.POSM : EDIT_ITEM.POSM;
			if (posm == "1" && f1903.BUNDLE_SERIALNO == "1")
			{
				DialogService.ShowMessage(Properties.Resources.P0503020000_NotSerialLocItem);
				return false;
			}
			//序號商品 提示需提示要重新匯入序號
			//if (f1903.BUNDLE_SERIALNO == "1")
			//{
			//    if (serialNo_Data != null && serialNo_Data.Any())
			//    {
			//        serialNo_Data = null;
			//        DialogService.ShowMessage(f1903.ITEM_NAME + Properties.Resources.P0503020000_AgainSerialNo);
			//        CanEditDetailQty = "1";
			//    }
			//}
			return true;
		}

		#endregion

		#region 共用方法

		private F1903 FindF1903(string itemCode)
		{
			var proxy19 = GetProxy<F19Entities>();
			return proxy19.F1903s.Where(x => x.GUP_CODE == _gupCode
																			&& x.CUST_CODE == _custCode
																			&& x.ITEM_CODE == itemCode)
													 .FirstOrDefault();
		}

		#endregion

		#region 設定配送商是否允許來回件
		private void SetAllIdRoundPiece(string dcCode, string allId)
		{
			var proxy = GetProxy<F19Entities>();
			var f1947 = proxy.F1947s.Where(x => x.DC_CODE == dcCode && x.ALL_ID == allId).FirstOrDefault();
			AllowRoundPiece = false;
			if (f1947 != null)
			{
				AllowRoundPiece = f1947.ALLOW_ROUND_PIECE == "1";
			}
		}
		#endregion
	}

	#region Enum

	public enum SendTypeData
	{
		BySelf = 0, //自取
		ByVendor = 1, //超取
		ByHouse = 2  //宅配
	}

	#endregion
}