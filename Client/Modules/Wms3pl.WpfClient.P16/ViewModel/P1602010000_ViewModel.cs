using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.UILib.Services;
using F160201 = Wms3pl.WpfClient.DataServices.F16DataService.F160201;
using F16Entities = Wms3pl.WpfClient.DataServices.F16DataService.F16Entities;
using F1908 = Wms3pl.WpfClient.DataServices.F19DataService.F1908;
using F19Entities = Wms3pl.WpfClient.DataServices.F19DataService.F19Entities;
using wcf = Wms3pl.WpfClient.ExDataServices.P16WcfService;
using Wms3pl.WpfClient.ExDataServices.P16WcfService;


namespace Wms3pl.WpfClient.P16.ViewModel
{
    public partial class P1602010000_ViewModel : InputViewModelBase
    {
        public Action ExcelImport = delegate { };

        public class EdiDetail
		{
			public string ItemCode { set; get; }
			public int Count { set; get; }
		}


        public string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        public string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		#region Constructor
		public P1602010000_ViewModel()
		{
			Init();
		}
		#endregion

		#region 初始化
		private void Init()
		{
			//訊息視窗
			_messagesStruct.Button = DialogButton.OK;
			_messagesStruct.Image = DialogImage.Warning;
			_messagesStruct.Title = Resources.Resources.Information;

			//物流中心 DataSource
			DcCodes = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);

			var proxyF00 = GetProxy<F00Entities>();
			var listChangeTypes = proxyF00.F000903s.Where(item => item.ORD_PROP.StartsWith("V")).ToList();

			var queryChangeTypes = from item in listChangeTypes
								   select new NameValuePair<string>
								   {
									   Name = item.ORD_PROP_NAME,
									   Value = item.ORD_PROP
								   };
			//作業類別 DataSource
			ChangeTypes = queryChangeTypes.ToList();

			var proxyF16 = GetProxy<F16Entities>();
			var listReturnTypes = proxyF16.F160203s
									.OrderBy(item => item.RTN_VNR_TYPE_ID)
									.ToList();

			var queryReturnTypes = from item in listReturnTypes
								   select new NameValuePair<string>
								   {
									   Name = item.RTN_VNR_TYPE_NAME,
									   Value = item.RTN_VNR_TYPE_ID
								   };
			//廠退類型 DataSource
			ReturnTypes = queryReturnTypes.ToList();

			var proxyF19 = GetProxy<F19Entities>();
			var listReturnReasons = proxyF19.F1951s.Where(item => item.UCT_ID == "RV")
										.OrderBy(item => item.UCC_CODE)
										.ToList();

			var queryReturnReasons = from item in listReturnReasons
									 select new NameValuePair<string>
									 {
										 Name = item.CAUSE,
										 Value = item.UCC_CODE
									 };
			//廠退原因 DataSource
			ReturnReasons = queryReturnReasons.ToList();

			var queryReceiptStatus = GetBaseTableService.GetF000904List(FunctionCode, "F160201", "STATUS");
			//單據狀態 DataSource
			ReceiptStatusSource = queryReceiptStatus.ToList();
			ReceiptStatusSource.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = "-1" });

			//查詢條件初始值
			DcCodeForSearch = DcCodes[0].Value;
			ReceiptStatusForSearch = ReceiptStatusSource[0].Value;
			CreateDateBeginForSearch = DateTime.Today;
			CreateDateEndForSearch = DateTime.Today;

			DeliveryWayList = GetBaseTableService.GetF000904List(FunctionCode, "F160201", "DELIVERY_WAY");

			WarehouseTypeList = (from o in proxyF19.F190002s.Where(x=> x.TICKET_ID ==4).ToList()
													 join c in proxyF19.F198001s.ToList()
													 on o.WAREHOUSE_TYPE equals c.TYPE_ID
													 where o.TICKET_ID == 4
													 select new NameValuePair<string>
													 {
														 Name = c.TYPE_NAME,
														 Value = o.WAREHOUSE_TYPE
													 }).ToList();


		}
		#endregion

		#region 顯示提示訊息
		private MessagesStruct _messagesStruct;

		public void ShowMessage(string message)
		{
			_messagesStruct.Message = message;
			ShowMessage(_messagesStruct);
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

        #region 資料來源
        //物流中心
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

        //作業類別
        private List<NameValuePair<string>> _changeTypes;

		public List<NameValuePair<string>> ChangeTypes
		{
			get { return _changeTypes; }
			set
			{
				_changeTypes = value;
				RaisePropertyChanged("ChangeTypes");
			}
		}

		//廠退類型
		private List<NameValuePair<string>> _returnTypes;

		public List<NameValuePair<string>> ReturnTypes
		{
			get { return _returnTypes; }
			set
			{
				_returnTypes = value;
				RaisePropertyChanged("ReturnTypes");
			}
		}

		//廠退原因
		private List<NameValuePair<string>> _returnReasons;

		public List<NameValuePair<string>> ReturnReasons
		{
			get { return _returnReasons; }
			set
			{
				_returnReasons = value;
				RaisePropertyChanged("ReturnReasons");
			}
		}

		//單據狀態
		private List<NameValuePair<string>> _receiptStatusSource;

		public List<NameValuePair<string>> ReceiptStatusSource
		{
			get { return _receiptStatusSource; }
			set
			{
				_receiptStatusSource = value;
				RaisePropertyChanged("ReceiptStatusSource");
			}
		}


		#region 配送方式
		private List<NameValuePair<string>> _deliveryWayList;

		public List<NameValuePair<string>> DeliveryWayList
		{
			get { return _deliveryWayList; }
			set
			{
				Set(() => DeliveryWayList, ref _deliveryWayList, value);
			}
		}
		#endregion


		#region 出貨倉別
		private List<NameValuePair<string>> _warehouseTypeList;

		public List<NameValuePair<string>> WarehouseTypeList
		{
			get { return _warehouseTypeList; }
			set
			{
				Set(() => WarehouseTypeList, ref _warehouseTypeList, value);
			}
		}
		#endregion


		#endregion

		#region 操作控制
		private bool _isShowSearchResult = true;
		private bool _isSearchConditionError;

		public bool IsShowSearchResult
		{
			get { return _isShowSearchResult; }
			set
			{
				_isShowSearchResult = value;
				RaisePropertyChanged("IsShowSearchResult");
			}
		}

		private bool _isShowSearchCondition = true;

		public bool IsShowSearchCondition
		{
			get { return _isShowSearchCondition; }
			set
			{
				_isShowSearchCondition = value;
				RaisePropertyChanged("IsShowSearchCondition");
			}
		}

		private bool _isShowDetailData;

		public bool IsShowDetailData
		{
			get { return _isShowDetailData; }
			set
			{
				_isShowDetailData = value;
				RaisePropertyChanged("IsShowDetailData");
			}
		}

		public Action EditAction = delegate { };

		private bool _isAddNewDetailInputError;

		public bool IsAddNewDetailInputError
		{
			get { return _isAddNewDetailInputError; }
			set
			{
				_isAddNewDetailInputError = value;
				RaisePropertyChanged("IsAddNewDetailInputError");
			}
		}

		private bool _isNotEDI;

		public bool IsNotEDI
		{
			get { return _isNotEDI; }
			set
			{
				_isNotEDI = value;
				RaisePropertyChanged("IsNotEDI");
			}
		}

		private bool _hasVendor;

		public bool HasVendor
		{
			get { return _hasVendor; }
			set
			{
				Set(() => HasVendor, ref _hasVendor, value);


			}
		}

		public void CheckAddressAndSetSelfTake()
		{
			if (HasVendor)
			{
                // 無論廠商是否有地址都將自取打勾
                AddNewF160201.SELF_TAKE = "1";
            }
		}


		private Visibility _ediUiVisibility;

		public Visibility EdiUiVisibility
		{
			get { return _ediUiVisibility; }
			set
			{
				_ediUiVisibility = value;
				RaisePropertyChanged("EdiUiVisibility");
			}
		}

		private string _returnNoBeforeEditSave = String.Empty;

		public Action SetQueryFocus = delegate { };
		public Action SetAddNewFocus = delegate { };
		public Action SetEditFocus = delegate { };
		#endregion

		#region 查詢條件
		private string _dcCodeForSearch = String.Empty;

		public string DcCodeForSearch
		{
			get { return _dcCodeForSearch; }
			set
			{
				_dcCodeForSearch = value;
				RaisePropertyChanged("DcCodeForSearch");
			}
		}

		private DateTime? _createDateBeginForSearch;

		public DateTime? CreateDateBeginForSearch
		{
			get { return _createDateBeginForSearch; }
			set
			{
				_createDateBeginForSearch = value;
				RaisePropertyChanged("CreateDateBeginForSearch");
			}
		}

		private DateTime? _createDateEndForSearch;

		public DateTime? CreateDateEndForSearch
		{
			get { return _createDateEndForSearch; }
			set
			{
				_createDateEndForSearch = value;
				RaisePropertyChanged("CreateDateEndForSearch");
			}
		}

		private string _receiptNoForSearch = String.Empty;

		public string ReceiptNoForSearch
		{
			get { return _receiptNoForSearch; }
			set
			{
				_receiptNoForSearch = value;
				RaisePropertyChanged("ReceiptNoForSearch");
			}
		}

		private DateTime? _postingDateBeginForSearch;

		public DateTime? PostingDateBeginForSearch
		{
			get { return _postingDateBeginForSearch; }
			set
			{
				_postingDateBeginForSearch = value;
				RaisePropertyChanged("PostingDateBeginForSearch");
			}
		}

		private DateTime? _postingDateEndForSearch;

		public DateTime? PostingDateEndForSearch
		{
			get { return _postingDateEndForSearch; }
			set
			{
				_postingDateEndForSearch = value;
				RaisePropertyChanged("PostingDateEndForSearch");
			}
		}

		private string _custReceiptNoForSearch = String.Empty;

		public string CustReceiptNoForSearch
		{
			get { return _custReceiptNoForSearch; }
			set
			{
				_custReceiptNoForSearch = value;
				RaisePropertyChanged("CustReceiptNoForSearch");
			}
		}

		private string _receiptStatusForSearch = String.Empty;

		public string ReceiptStatusForSearch
		{
			get { return _receiptStatusForSearch; }
			set
			{
				_receiptStatusForSearch = value;
				RaisePropertyChanged("ReceiptStatusForSearch");
			}
		}

		private string _vendorCodeForSearch = String.Empty;

		public string VendorCodeForSearch
		{
			get { return _vendorCodeForSearch; }
			set
			{
				if (_vendorCodeForSearch == value)
					return;
				_vendorCodeForSearch = value;

				RaisePropertyChanged("VendorCodeForSearch");
			}
		}

		private void VendorCodeChangeCheck(SelectionList<F160201ReturnDetail> data)
		{
			if (data != null && data.Any() && AddNewF160201 != null)
			{
				var proxyF19 = GetProxy<F19Entities>();
				var itemData = string.Empty;
				var spilt = '、';
				foreach (var d in data)
				{
					var count = proxyF19.F190303s.Where(
						x => x.GUP_CODE == AddNewF160201.GUP_CODE &&
							 x.CUST_CODE == AddNewF160201.CUST_CODE &&
							 x.ITEM_CODE == d.Item.ITEM_CODE &&
							 x.VNR_CODE == AddNewF160201.VNR_CODE).Count();

					if (count == 0)
					{
						itemData += d.Item.ITEM_NAME + spilt;
					}
				}
				itemData = itemData.TrimEnd(spilt);

				if (!string.IsNullOrWhiteSpace(itemData))
				{
					ShowWarningMessage(string.Format(Properties.Resources.P1602010000_VNRItem_ImportStockRecordInvalid, itemData, Environment.NewLine));
				}
			}
		}

		private string _vendorNameForSearch = String.Empty;

		public string VendorNameForSearch
		{
			get { return _vendorNameForSearch; }
			set
			{
				_vendorNameForSearch = value;
				RaisePropertyChanged("VendorNameForSearch");
			}
		}

		private string _custOrderNoForSearch = String.Empty;

		public string CustOrderNoForSearch
		{
			get { return _custOrderNoForSearch; }
			set
			{
				_custOrderNoForSearch = value;
				RaisePropertyChanged("CustOrderNoForSearch");
			}
		}
		#endregion

		#region 查詢結果及細項
		//查詢結果-廠退單清單
		private List<F160201Data> _f160201List;

		public List<F160201Data> F160201List
		{
			get { return _f160201List; }
			set
			{
				_f160201List = value;
				RaisePropertyChanged("F160201List");
			}
		}

		//查詢結果清單中被選取的-單一筆廠退單，點選後顯示廠退明細
		private F160201Data _selectedF160201;

		public F160201Data SelectedF160201
		{
			get { return _selectedF160201; }
			set
			{
				_selectedF160201 = value;
				RaisePropertyChanged("SelectedF160201");

				if (value != null)
				{
					IsShowDetailData = true;

					var proxy = GetProxy<F19Entities>();
					List<F1908> result = (from A in proxy.F1908s
										  where A.VNR_CODE == SelectedF160201.VNR_CODE &&
												A.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode
										  select A
										  ).AsQueryable().ToList();

					if (result.Count > 0)
					{
						foreach (var dcCode in DcCodes)
						{
							if (dcCode.Value == SelectedF160201.DC_CODE)
							{
								SelectedF160201DcName = dcCode.Name;
								break;
							}
						}

						//SelectedF160201ContactAddress = result[0].ADDRESS;
						//SelectedF160201ContactPerson = result[0].ITEM_CONTACT;
						//SelectedF160201ContactTel = result[0].ITEM_TEL;

						var proxy2 = GetExProxy<P16ExDataSource>();

						SelectedF160201DetailList = proxy2.CreateQuery<F160201DataDetail>("GetF160201DataDetails")
											.AddQueryOption("dcCode", string.Format("'{0}'", SelectedF160201.DC_CODE))
											.AddQueryOption("gupCode", string.Format("'{0}'", Wms3plSession.Get<GlobalInfo>().GupCode))
											.AddQueryOption("custCode", string.Format("'{0}'", Wms3plSession.Get<GlobalInfo>().CustCode))
											.AddQueryOption("returnNo", string.Format("'{0}'", SelectedF160201.RTN_VNR_NO)).ToList();
					}
				}
				else
				{
					IsShowDetailData = false;
				}
			}
		}

		//被選取的廠退單的廠退明細
		private List<F160201DataDetail> _selectedF160201DetailList;
		public List<F160201DataDetail> SelectedF160201DetailList
		{
			get { return _selectedF160201DetailList; }
			set
			{
				_selectedF160201DetailList = value;
				RaisePropertyChanged("SelectedF160201DetailList");
			}
		}

		//其他查詢結果細項
		private string _selectedF160201DcName = String.Empty;

		public string SelectedF160201DcName
		{
			get { return _selectedF160201DcName; }
			set
			{
				_selectedF160201DcName = value;
				RaisePropertyChanged("SelectedF160201DcName");
			}
		}

		private string _selectedF160201ContactAddress = String.Empty;

		public string SelectedF160201ContactAddress
		{
			get { return _selectedF160201ContactAddress; }
			set
			{
				_selectedF160201ContactAddress = value;
				RaisePropertyChanged("SelectedF160201ContactAddress");
			}
		}

		private string _selectedF160201ContactPerson = String.Empty;

		public string SelectedF160201ContactPerson
		{
			get { return _selectedF160201ContactPerson; }
			set
			{
				_selectedF160201ContactPerson = value;
				RaisePropertyChanged("SelectedF160201ContactPerson");
			}
		}

		private string _selectedF160201ContactTel = String.Empty;

		public string SelectedF160201ContactTel
		{
			get { return _selectedF160201ContactTel; }
			set
			{
				_selectedF160201ContactTel = value;
				RaisePropertyChanged("SelectedF160201ContactTel");
			}
		}
        #endregion

        #region 商品搜尋用欄位
        private string _itemCode;

        public string ITEM_CODE
        {
            get { return _itemCode; }
            set
            {
                _itemCode = value;
                RaisePropertyChanged("ITEM_CODE");
            }
        }

        private string _itemName;

        public string ITEM_NAME
        {
            get { return _itemName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    RTN_VNR_QTY = 0;
					          MAKE_NO = "";
					          MEMO ="";
					          RTN_VNR_CAUSE ="";
                }
                _itemName = value;
                RaisePropertyChanged("ITEM_NAME");
            }
        }

        private string _itemSize;

        public string ITEM_SIZE
        {
            get { return _itemSize; }
            set
            {
                _itemSize = value;
                RaisePropertyChanged("ITEM_SIZE");
            }
        }

        private string _itemSpec;

        public string ITEM_SPEC
        {
            get { return _itemSpec; }
            set
            {
                _itemSpec = value;
                RaisePropertyChanged("ITEM_SPEC");
            }
        }

        private string _itemColor;

        public string ITEM_COLOR
        {
            get { return _itemColor; }
            set
            {
                _itemColor = value;
                RaisePropertyChanged("ITEM_COLOR");
            }
        }

        private string _serialNo;
        public string SERIAL_NO
        {
            get { return _serialNo; }
            set
            {
                _serialNo = value;
                RaisePropertyChanged("SERIAL_NO");
            }
        }

        private int _rtnVnrQty;

        public int RTN_VNR_QTY
        {
            get { return _rtnVnrQty; }
            set
            {
                _rtnVnrQty = value;
                RaisePropertyChanged("RTN_VNR_QTY");
            }
        }

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

				private string _rtnVnrCause;

				public string RTN_VNR_CAUSE
		{
					get { return _rtnVnrCause; }
					set
					{
				_rtnVnrCause = value;
						RaisePropertyChanged("RTN_VNR_CAUSE");
					}
				}

		private string _memo;

				public string MEMO
		{
					get { return _memo; }
					set
					{
						_memo = value;
						RaisePropertyChanged("MEMO");
					}
				}

		#endregion

		#region 查詢 Command
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => DoSearchCompleted());
			}
		}

		private void DoSearch()
		{
			IsShowSearchCondition = true;
			_isSearchConditionError = false;

			//檢查查詢條件
			if (!CreateDateBeginForSearch.HasValue || !CreateDateEndForSearch.HasValue)
			{
				_messagesStruct.Message = Properties.Resources.P1602010000_CRT_DateEmpty;
				ShowMessage(_messagesStruct);
				_isSearchConditionError = true;
				return;
			}

			ValidateHelper.AutoChangeBeginEnd(this, x => x.CreateDateBeginForSearch, x => x.CreateDateEndForSearch);
			ValidateHelper.AutoChangeBeginEnd(this, x => x.PostingDateBeginForSearch, x => x.PostingDateEndForSearch, true, false);

			var proxy = GetExProxy<P16ExDataSource>();

			F160201List = proxy.CreateQuery<F160201Data>("GetF160201Datas")
								.AddQueryExOption("dcCode", DcCodeForSearch)
								.AddQueryExOption("gupCode", _gupCode)
								.AddQueryExOption("custCode", _custCode)
								.AddQueryExOption("status", ReceiptStatusForSearch)
								.AddQueryExOption("createBeginDateTime", CreateDateBeginForSearch.Value)
								.AddQueryExOption("createEndDateTime", CreateDateEndForSearch.Value.AddDays(1))
								.AddQueryExOption("postingBeginDateTime", PostingDateBeginForSearch.HasValue ? PostingDateBeginForSearch.Value.ToString("yyyy/MM/dd") : String.Empty)
								.AddQueryExOption("postingEndDateTime", PostingDateEndForSearch.HasValue ? PostingDateEndForSearch.Value.AddDays(1).ToString("yyyy/MM/dd") : String.Empty)
								.AddQueryExOption("returnNo", ReceiptNoForSearch)
								.AddQueryExOption("custOrdNo", CustReceiptNoForSearch)
								.AddQueryExOption("vendorCode", VendorCodeForSearch)
								.AddQueryExOption("vendorName", VendorNameForSearch).ToList();
		}

		private void DoSearchCompleted()
		{
			if (_isSearchConditionError) return;
			SetQueryFocus();

			if (F160201List == null)
			{
				_messagesStruct.Message = Properties.Resources.P1602010000_NoData;
				ShowMessage(_messagesStruct);
			}
			else
			{
				if (!F160201List.Any())
				{
					_messagesStruct.Message = Properties.Resources.P1602010000_NoData;
					ShowMessage(_messagesStruct);
				}
				else
				{
					SelectedF160201 = F160201List.FirstOrDefault();
					IsShowSearchCondition = false;
					IsShowSearchResult = true;
				}
			}
		}
		#endregion

		#region 新增資料
		//欲新增的廠退單
		private F160201 _addNewF160201;

		public F160201 AddNewF160201
		{
			get { return _addNewF160201; }
			set
			{
				_addNewF160201 = value;
				RaisePropertyChanged("AddNewF160201");
			}
		}

		//欲新增的廠退單的廠退明細
		private SelectionList<F160201ReturnDetail> _addNewF160201DetailList;

		public SelectionList<F160201ReturnDetail> AddNewF160201DetailList
		{
			get { return _addNewF160201DetailList; }
			set
			{
				_addNewF160201DetailList = value;
				RaisePropertyChanged("AddNewF160201DetailList");
			}
		}

		//由新增廠退明細的傳回結果，會轉換並加入AddNewF160201DetailList
		public List<F160201ReturnDetail> ReturnData { get; set; }

		//新增模式，廠退明細DataGrid 被選取的單一筆 廠退明細
		private SelectionItem<F160201ReturnDetail> _selectedReturnDetaiItemForAddNew;

		public SelectionItem<F160201ReturnDetail> SelectedReturnDetaiItemForAddNew
		{
			get { return _selectedReturnDetaiItemForAddNew; }
			set
			{
				_selectedReturnDetaiItemForAddNew = value;
				RaisePropertyChanged("SelectedReturnDetaiItemForAddNew");
                if (_selectedReturnDetaiItemForAddNew != null)
                {
                    ITEM_CODE = _selectedReturnDetaiItemForAddNew.Item.ITEM_CODE;
                    ITEM_NAME = _selectedReturnDetaiItemForAddNew.Item.ITEM_NAME;
                    ITEM_SPEC = _selectedReturnDetaiItemForAddNew.Item.ITEM_SPEC;
                    ITEM_SIZE = _selectedReturnDetaiItemForAddNew.Item.ITEM_SIZE;
                    ITEM_COLOR = _selectedReturnDetaiItemForAddNew.Item.ITEM_COLOR;
                    RTN_VNR_QTY = _selectedReturnDetaiItemForAddNew.Item.RTN_VNR_QTY;
					          MAKE_NO = _selectedReturnDetaiItemForAddNew.Item.MAKE_NO;
					          RTN_VNR_CAUSE = _selectedReturnDetaiItemForAddNew.Item.RTN_VNR_CAUSE;
					          MEMO = _selectedReturnDetaiItemForAddNew.Item.MEMO;

								}
            }
		}

		private string _addNewF160201VendorName = String.Empty;

		public string AddNewF160201VendorName
		{
			get { return _addNewF160201VendorName; }
			set
			{
				if (_addNewF160201VendorName == value)
					return;

				_addNewF160201VendorName = value;
				VendorCodeChangeCheck(AddNewF160201DetailList);
				RaisePropertyChanged("AddNewF160201VendorName");
			}
		}

		private bool _isEditSelectedAll = false;
		public bool IsEditSelectedAll
		{
			get { return _isEditSelectedAll; }
			set { _isEditSelectedAll = value; RaisePropertyChanged("IsEditSelectedAll"); }
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
			if (UserOperateMode == OperateMode.Add && AddNewF160201DetailList != null)
			{
				foreach (var p in AddNewF160201DetailList)
				{
					p.IsSelected = IsEditSelectedAll;
				}
			}
			else if (UserOperateMode == OperateMode.Edit && EditF160201DetailList != null)
			{
				foreach (var p in EditF160201DetailList)
				{
					p.IsSelected = IsEditSelectedAll;
				}
			}
		}

		public List<NameValuePair<string>> GetDcCodes()
		{
			return DcCodes;
		}
		#endregion

		#region 新增 Command
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => DoAdd(), () => UserOperateMode == OperateMode.Query);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			AddNewF160201 = new F160201();
			AddNewF160201.RTN_VNR_DATE = DateTime.Today;
			//AddNewF160201.POSTING_DATE = DateTime.Today;
			AddNewF160201.GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode;
			AddNewF160201.CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode;
			AddNewF160201DetailList = null;
			//AddNewF160201.DELIVERY_WAY = DeliveryWayList.FirstOrDefault()?.Value;
			//AddNewF160201VendorName = AddNewF160201ContactPerson = AddNewF160201ContactTel = AddNewF160201ContactAddress = String.Empty;
          
            IsNotEDI = true;
			SetAddNewFocus();
            ClearSearchProduct();
        }
		#endregion

		#region 新增儲存
		private bool DoAddSave()
		{
			
			_messagesStruct.Message = CheckInputError(AddNewF160201, AddNewF160201DetailList);

			if (!String.IsNullOrEmpty(_messagesStruct.Message))
			{
				ShowMessage(_messagesStruct);
				return false;
			}
			      AddNewF160201.RTN_VNR_CAUSE = AddNewF160201DetailList.First().Item.RTN_VNR_CAUSE;
			      AddNewF160201.SELF_TAKE = AddNewF160201.DELIVERY_WAY == "0"  ? "1" : "0";

            var f160201 = ExDataMapper.Map<F160201, wcf.F160201>(AddNewF160201);
			var f160202s = ExDataMapper.MapCollection<F160201ReturnDetail, wcf.F160202>(AddNewF160201DetailList.Select(si => si.Item)).ToArray();

			var proxy = new wcf.P16WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
													() => proxy.InsertF160201(f160201, f160202s));

			DialogService.ShowMessage(result.Message);
			return result.IsSuccessed;
		}
		#endregion

		#region 取得廠商資料
		public void SearchVendorData(string vendorCode)
		{
			var proxy = GetProxy<F19Entities>();
			var item = proxy.F1908s.Where(o => o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode &&
												o.VNR_CODE == vendorCode).ToList().FirstOrDefault();

			if (item != null)
			{
				if (UserOperateMode == OperateMode.Add)
				{
					AddNewF160201.VNR_CODE = vendorCode;
					AddNewF160201VendorName = item.VNR_NAME;
					AddNewF160201.ADDRESS = item.ADDRESS;
					AddNewF160201.ITEM_CONTACT = item.ITEM_CONTACT;
					AddNewF160201.ITEM_TEL = item.ITEM_TEL;
					AddNewF160201.DELIVERY_WAY = item.DELIVERY_WAY;
				}
				else
				{
					EditF160201.VNR_CODE = vendorCode;
					EditF160201VendorName = item.VNR_NAME;
					EditF160201.ADDRESS = item.ADDRESS;
					EditF160201.ITEM_CONTACT = item.ITEM_CONTACT;
					EditF160201.ITEM_TEL = item.ITEM_TEL;
					EditF160201.DELIVERY_WAY = item.DELIVERY_WAY;
				}
			}
			else
			{
				if (UserOperateMode == OperateMode.Add)
				{
					AddNewF160201.VNR_CODE = String.Empty;
					AddNewF160201VendorName = String.Empty;
					AddNewF160201.DELIVERY_WAY = string.Empty;
				}
				else
				{
					EditF160201.VNR_CODE = String.Empty;
					EditF160201VendorName = String.Empty;
					EditF160201.DELIVERY_WAY = string.Empty;
				}

				_messagesStruct.Message = Properties.Resources.P1602010000_VNRCodeNotFound;
				ShowMessage(_messagesStruct);
			}
		}
		#endregion

		public List<F160201ReturnDetail> GetCheckSearch()
		{
			List<F160201ReturnDetail> result = new List<F160201ReturnDetail>();

			if (UserOperateMode == OperateMode.Add)
			{
				if (AddNewF160201DetailList != null)
				{
					var sel = AddNewF160201DetailList.Where(x => x.IsSelected == true).ToList();
					result = (from i in sel select i.Item).ToList();
				}
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				if (EditF160201DetailList != null)
				{
					var sel = EditF160201DetailList.Where(x => x.IsSelected == true).ToList();
					result = (from i in sel select i.Item).ToList();
				}
			}

			return result;
		}

        #region 新增明細(新增/修改)
        public ICommand AddDetailCommand
        {
            get
            {
                return CreateBusyAsyncCommand(o => DoAddDetail(), () => !string.IsNullOrEmpty(ITEM_CODE) && !string.IsNullOrEmpty(ITEM_NAME) && RTN_VNR_QTY!=0 && IsNotEDI);
            }
        }
        private void DoAddDetail()
        {

        }
        #endregion

        #region 刪除明細(新增、修改)
        public ICommand DeleteDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => DoDeleteDetail(), () => GetCheckSearch().Any() && IsNotEDI);
			}
		}

		private void DoDeleteDetail()
		{

		}

		public void DeleteDetailComplate(bool isEditMode = false)
		{
			if (UserOperateMode != OperateMode.Query)
			{
				var delItemList = GetCheckSearch();

				if (delItemList != null && delItemList.Any())
				{
					// 確認是否要刪除
					if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

					//執行刪除動作
					SelectionList<F160201ReturnDetail> target = null;

					if (isEditMode)
					{
						target = EditF160201DetailList;
					}
					else
					{
						target = AddNewF160201DetailList;
					}

					var tmpAddItemList = target.Where(x => x.IsSelected == false).ToList();
					List<F160201ReturnDetail> result = (from i in tmpAddItemList select i.Item).ToList();

					if (isEditMode)
					{
						EditF160201DetailList = new SelectionList<F160201ReturnDetail>(result);
					}
					else
					{
						AddNewF160201DetailList = new SelectionList<F160201ReturnDetail>(result);
					}
				}
			}
		}
		#endregion

		#region 修改明細(新增、修改)
		public ICommand ModifyDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => DoModifyDetail(), () => SelectedReturnDetaiItemForAddNew != null && !string.IsNullOrWhiteSpace(ITEM_CODE) && IsNotEDI);
			}
		}

		private void DoModifyDetail()
		{
            if (_selectedReturnDetaiItemForAddNew != null)
            {
                if (CheckDetail(false, AddNewF160201DetailList, SelectedReturnDetaiItemForAddNew))
                {
                    _selectedReturnDetaiItemForAddNew.Item.ITEM_CODE = ITEM_CODE;
                    _selectedReturnDetaiItemForAddNew.Item.ITEM_NAME = ITEM_NAME;
                    _selectedReturnDetaiItemForAddNew.Item.ITEM_COLOR = ITEM_COLOR;
                    _selectedReturnDetaiItemForAddNew.Item.ITEM_SIZE = ITEM_SIZE;
                    _selectedReturnDetaiItemForAddNew.Item.ITEM_SPEC = ITEM_SPEC;
                    _selectedReturnDetaiItemForAddNew.Item.RTN_VNR_QTY = RTN_VNR_QTY;
					          _selectedReturnDetaiItemForAddNew.Item.MAKE_NO = MAKE_NO;
					          _selectedReturnDetaiItemForAddNew.Item.RTN_VNR_CAUSE = RTN_VNR_CAUSE;
										_selectedReturnDetaiItemForAddNew.Item.RTN_VNR_CAUSE_NAME = ReturnReasons.FirstOrDefault(x => x.Value == RTN_VNR_CAUSE)?.Name;
									  _selectedReturnDetaiItemForAddNew.Item.MEMO = MEMO;
                    SERIAL_NO = "";
                }
            }
        }
		#endregion

		#region 取消 Command
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query,
					o => DoCancelComplete()
					);
			}
		}

		private void DoCancel()
		{
			DcCodeForSearch = DcCodes[0].Value;
			ReceiptStatusForSearch = ReceiptStatusSource[0].Value;
			CreateDateBeginForSearch = DateTime.Today;
			CreateDateEndForSearch = DateTime.Today;
			PostingDateBeginForSearch = null;
			PostingDateEndForSearch = null;

			if (UserOperateMode == OperateMode.Edit)
				return;

			IsShowSearchResult = true;
			IsShowSearchCondition = true;
			IsShowDetailData = false;
		}

		private void DoCancelComplete()
		{
			if (UserOperateMode == OperateMode.Edit)
			{
				EditF160201 = null;
				EditF160201DetailList = null;
				UserOperateMode = OperateMode.Query;
				return;
			}

			UserOperateMode = OperateMode.Query;
			F160201List = null;
			SelectedF160201 = null;
			SelectedF160201DetailList = null;
			AddNewF160201 = null;
			AddNewF160201DetailList = null;
			AddNewF160201VendorName = String.Empty;
		}

		#endregion

		#region 編輯資料
		//欲修改的廠退單
		private F160201 _editF160201;

		public F160201 EditF160201
		{
			get { return _editF160201; }
			set
			{
				_editF160201 = value;
				RaisePropertyChanged("EditF160201");
				//GetDetailData(_selectedData);
			}
		}

		//欲修改的廠退單的廠退明細
		private SelectionList<F160201ReturnDetail> _editF160201DetailList;

		public SelectionList<F160201ReturnDetail> EditF160201DetailList
		{
			get { return _editF160201DetailList; }
			set
			{
				_editF160201DetailList = value;
				RaisePropertyChanged("EditF160201DetailList");
			}
		}

		//轉換EDI明細，以商品為主，同一商品併為一筆並將數量加總，儲存檢核時要比對最後編輯結果，兩者的商品種類及加總數量皆要相同
		public List<EdiDetail> EdiDetailData { get; set; }

		private string _oldItemCode = string.Empty;
		private string _oldMakeNo = string.Empty;
		//修改模式，廠退明細DataGrid 被選取的單一筆 廠退明細
		private SelectionItem<F160201ReturnDetail> _selectedReturnDetaiItemForEdit;

		public SelectionItem<F160201ReturnDetail> SelectedReturnDetaiItemForEdit
		{
			get { return _selectedReturnDetaiItemForEdit; }
			set
			{
				_selectedReturnDetaiItemForEdit = value;
				RaisePropertyChanged("SelectedReturnDetaiItemForEdit");

				if (_isNotEDI)
				{
					EdiUiVisibility = Visibility.Collapsed;
				}
				else
				{
					EdiUiVisibility = Visibility.Visible;

					if (_selectedReturnDetaiItemForEdit != null)
					{
						EdiDetail data = EdiDetailData.Where(x => x.ItemCode == _selectedReturnDetaiItemForEdit.Item.ITEM_CODE).FirstOrDefault();

						if (data != null)
						{
							_selectedReturnDetaiItemForEdit.Item.ORIGINAL_RTN_VNR_QTY = data.Count;
						}
                    }
				}
                if (_selectedReturnDetaiItemForEdit != null)
                {
										_oldItemCode = _selectedReturnDetaiItemForEdit.Item.ITEM_CODE;
										_oldMakeNo = _selectedReturnDetaiItemForEdit.Item.MAKE_NO;

										ITEM_CODE = _selectedReturnDetaiItemForEdit.Item.ITEM_CODE;
                    ITEM_NAME = _selectedReturnDetaiItemForEdit.Item.ITEM_NAME;
                    ITEM_SPEC = _selectedReturnDetaiItemForEdit.Item.ITEM_SPEC;
                    ITEM_SIZE = _selectedReturnDetaiItemForEdit.Item.ITEM_SIZE;
                    ITEM_COLOR = _selectedReturnDetaiItemForEdit.Item.ITEM_COLOR;
                    RTN_VNR_QTY = _selectedReturnDetaiItemForEdit.Item.RTN_VNR_QTY;
					          MAKE_NO = _selectedReturnDetaiItemForEdit.Item.MAKE_NO;
					          RTN_VNR_CAUSE = _selectedReturnDetaiItemForEdit.Item.RTN_VNR_CAUSE;
					          MEMO = _selectedReturnDetaiItemForEdit.Item.MEMO;
										SERIAL_NO = "";
                }
                
            }
		}

		private string _editF160201VendorName = String.Empty;

		public string EditF160201VendorName
		{
			get { return _editF160201VendorName; }
			set
			{
				_editF160201VendorName = value;
				RaisePropertyChanged("EditF160201VendorName");
			}
		}
	

		private string _editF160201StatusText = String.Empty;

		public string EditF160201StatusText
		{
			get { return _editF160201StatusText; }
			set
			{
				_editF160201StatusText = value;
				RaisePropertyChanged("EditF160201StatusText");
			}
		}

        #endregion

        #region 編輯 Command
        public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query &&
									SelectedF160201 != null && (SelectedF160201.STATUS == "0")
								, o => DoEditComplete());
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			EdiUiVisibility = Visibility.Collapsed;
            ClearSearchProduct();

        }

		private void DoEditComplete()
		{
			if (SelectedF160201 != null)
			{
				var proxy = GetProxy<F16Entities>();

				var query = from item in proxy.F160201s
							where item.DC_CODE == DcCodeForSearch &&
								  item.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode &&
								  item.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode &&
								  item.RTN_VNR_NO == SelectedF160201.RTN_VNR_NO
							select item;

				if (query.Count() > 0)
				{
					F160201 a = query.First();
					EditF160201 = ExDataMapper.Map<F160201, F160201>(a);
					IsNotEDI = String.IsNullOrEmpty(EditF160201.CUST_ORD_NO);

					var proxy2 = GetExProxy<P16ExDataSource>();

					List<F160201ReturnDetail> details = proxy2.CreateQuery<F160201ReturnDetail>("GetF160201ReturnDetailsForEdit")
						.AddQueryOption("dcCode", string.Format("'{0}'", EditF160201.DC_CODE))
						.AddQueryOption("gupCode", string.Format("'{0}'", Wms3plSession.Get<GlobalInfo>().GupCode))
						.AddQueryOption("custCode", string.Format("'{0}'", Wms3plSession.Get<GlobalInfo>().CustCode))
						.AddQueryOption("vendorCode", String.Format("'{0}'", EditF160201.VNR_CODE))
						.AddQueryOption("returnNo", String.Format("'{0}'", EditF160201.RTN_VNR_NO)).ToList();

					if (!IsNotEDI)
					{
						GenerateEdiDetailData(details);
					}

					EditF160201DetailList = new SelectionList<F160201ReturnDetail>(details);

					var proxy3 = GetProxy<F19Entities>();
					var item2 = proxy3.F1908s.Where(o => o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode &&
														o.VNR_CODE == a.VNR_CODE).ToList().FirstOrDefault();
					if (item2 != null)
					{
						EditF160201.VNR_CODE = a.VNR_CODE;
						EditF160201VendorName = item2.VNR_NAME;
                        //EditF160201ContactAddress = item2.ADDRESS;
                        //EditF160201ContactPerson = item2.ITEM_CONTACT;
                        //EditF160201ContactTel = item2.ITEM_TEL;
                        EditF160201StatusText = SelectedF160201.STATUS_TEXT;
					}
					else
					{
						EditF160201.VNR_CODE = String.Empty;
						EditF160201VendorName = String.Empty;
						//EditF160201ContactAddress = String.Empty;
						//EditF160201ContactPerson = String.Empty;
						//EditF160201ContactTel = String.Empty;
						EditF160201StatusText = String.Empty;
					}
        }

				EditAction();
				SetEditFocus();
			}
		}

		//修改明細
		public ICommand ModifyDetailCommandForEdit
		{
			get
			{
				return CreateBusyAsyncCommand(o => DoModifyDetailForEdit(), () => SelectedReturnDetaiItemForEdit != null );
			}
		}

		private void DoModifyDetailForEdit()
		{
            if (_selectedReturnDetaiItemForEdit != null)
            {
                if (CheckDetail(false, EditF160201DetailList, SelectedReturnDetaiItemForEdit))
                {
                    _selectedReturnDetaiItemForEdit.Item.ITEM_CODE = ITEM_CODE;
                    _selectedReturnDetaiItemForEdit.Item.ITEM_NAME = ITEM_NAME;
                    _selectedReturnDetaiItemForEdit.Item.ITEM_COLOR = ITEM_COLOR;
                    _selectedReturnDetaiItemForEdit.Item.ITEM_SIZE = ITEM_SIZE;
                    _selectedReturnDetaiItemForEdit.Item.ITEM_SPEC = ITEM_SPEC;
                    _selectedReturnDetaiItemForEdit.Item.RTN_VNR_QTY = RTN_VNR_QTY;
					          _selectedReturnDetaiItemForEdit.Item.MAKE_NO = MAKE_NO;
					          _selectedReturnDetaiItemForEdit.Item.RTN_VNR_CAUSE = RTN_VNR_CAUSE;
					          _selectedReturnDetaiItemForEdit.Item.RTN_VNR_CAUSE_NAME = ReturnReasons.FirstOrDefault(x=> x.Value == RTN_VNR_CAUSE)?.Name;
										_selectedReturnDetaiItemForEdit.Item.MEMO = MEMO;
										SERIAL_NO = "";
                }
            }
        }
		#endregion

		#region 刪除 Command
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query &&
						SelectedF160201 != null && (SelectedF160201.STATUS == "0"),
					o => DoDeleteComplete());
			}
		}

		private void DoDelete()
		{
			if (SelectedF160201 != null)
			{
				if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

				var proxy = GetModifyQueryProxy<F16Entities>();
				var f160201 = proxy.F160201s.Where(x => x.CUST_CODE == SelectedF160201.CUST_CODE
														&& x.GUP_CODE == SelectedF160201.GUP_CODE
														&& x.DC_CODE == SelectedF160201.DC_CODE
														&& x.RTN_VNR_NO == SelectedF160201.RTN_VNR_NO).FirstOrDefault();

				if (f160201 == null)
				{
					ShowMessage(Messages.WarningBeenDeleted);
				}
				else if (f160201.STATUS != "0")
				{
					DialogService.ShowMessage(Properties.Resources.P1602010000_ORD_NO_StatusUnabledDelete);
				}
				else
				{
					f160201.STATUS = "9";
					proxy.UpdateObject(f160201);
					proxy.SaveChanges();
					SelectedF160201.STATUS = "9";
					SelectedF160201.STATUS_TEXT = Properties.Resources.P1601010000_Cancel;
					ShowMessage(Messages.InfoDeleteSuccess);
				}

				var proxyWcf = new wcf.P16WcfServiceClient();
				RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
													   () => proxyWcf.DelF075103s(SelectedF160201.CUST_CODE, SelectedF160201.CUST_ORD_NO));
				
			}
		}

		private void DoDeleteComplete()
		{
			UserOperateMode = OperateMode.Query;
            SearchCommand.Execute(null);

        }

		#endregion

		#region 儲存 Command

		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;

				return CreateBusyAsyncCommand(
					o => isSaved = DoSave(),
					() => UserOperateMode == OperateMode.Edit || (UserOperateMode == OperateMode.Add && HasVendor),
					o => SaveComplate(isSaved)
					);
			}
		}

		private bool DoSave()
		{
			if (UserOperateMode == OperateMode.Add)
				return DoAddSave();
			if (UserOperateMode == OperateMode.Edit)
				return DoEditSave();

			return false;
		}

		private void SaveComplate(bool isSaved)
		{
			if (isSaved)
			{
				DoSearch();

				if (UserOperateMode == OperateMode.Add)
				{
					if (F160201List != null && F160201List.Any())
					{
						SelectedF160201 = F160201List.Last();
					}
				}
				else
				{
					SelectedF160201 = F160201List.Find(x => x.RTN_VNR_NO == _returnNoBeforeEditSave);
				}

				IsShowSearchCondition = false;
				IsShowSearchResult = true;
				UserOperateMode = OperateMode.Query;
			}
		}
		#endregion

		#region 編輯儲存
		private bool DoEditSave()
		{
			_returnNoBeforeEditSave = SelectedF160201.RTN_VNR_NO;
			_messagesStruct.Message = CheckInputError(EditF160201, EditF160201DetailList);

			if (!String.IsNullOrEmpty(_messagesStruct.Message))
			{
				ShowMessage(_messagesStruct);
				return false;
			}
			      EditF160201.RTN_VNR_CAUSE = EditF160201DetailList.First().Item.RTN_VNR_CAUSE;
			      EditF160201.SELF_TAKE = EditF160201.DELIVERY_WAY == "0" ? "1" :"0";
            var f160201 = ExDataMapper.Map<F160201, wcf.F160201>(EditF160201);
			var f160202s = ExDataMapper.MapCollection<F160201ReturnDetail, wcf.F160202>(EditF160201DetailList.Select(si => si.Item)).ToArray();

			var proxy = new wcf.P16WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
													() => proxy.UpdateF160201(f160201, f160202s));

			DialogService.ShowMessage(result.Message);

			return result.IsSuccessed;
		}
		#endregion

		#region 廠退明細編輯結果回傳(需轉換)
		public void ConvertReturnDetailData(SelectionList<F160201ReturnDetail> targetSelectionList, F160201ReturnDetail remove = null)
		{
			if (ReturnData != null)
			{
                if (remove != null)
				{
                    foreach (var item in targetSelectionList)
					{
						if (item.Item.ITEM_CODE == remove.ITEM_CODE )
						{
							targetSelectionList.Remove(item);
							break;
						}

                    }
				}

				bool isExist = false;

				foreach (var newData in ReturnData)
				{
					F160201ReturnDetail newRow = new F160201ReturnDetail();
					//newRow.ROWNUM = newData.ROWNUM;
					//newRow.ENTER_DATE = newData.ENTER_DATE;
					//newRow.INVENTORY_QTY = newData.INVENTORY_QTY;
					newRow.ITEM_CODE = newData.ITEM_CODE;
					newRow.ITEM_COLOR = newData.ITEM_COLOR;
					newRow.ITEM_NAME = newData.ITEM_NAME;
					newRow.ITEM_SIZE = newData.ITEM_SIZE;
					newRow.ITEM_SPEC = newData.ITEM_SPEC;
					//newRow.LOC_CODE = newData.LOC_CODE;
					//newRow.MEMO = newData.MEMO;
					//newRow.ROWNUM = newData.ROWNUM;
					newRow.RTN_VNR_QTY = newData.RTN_VNR_QTY;
					//newRow.VALID_DATE = newData.VALID_DATE;
					//newRow.WAREHOUSE_ID = newData.WAREHOUSE_ID;
					//newRow.WAREHOUSE_NAME = newData.WAREHOUSE_NAME;
					newRow.MAKE_NO = newData.MAKE_NO;
					newRow.RTN_VNR_CAUSE = newData.RTN_VNR_CAUSE;
					newRow.RTN_VNR_CAUSE_NAME = newData.RTN_VNR_CAUSE_NAME;
					newRow.MEMO = newData.MEMO;

					SelectionItem<F160201ReturnDetail> newRows = new SelectionItem<F160201ReturnDetail>(newRow);

                    targetSelectionList.Add(newRows);
                }
			}
		}
		#endregion

		#region 儲存檢核
		private string CheckInputError(F160201 f160201, SelectionList<F160201ReturnDetail> detailList)
		{
			ExDataMapper.Trim(f160201);

			if (String.IsNullOrWhiteSpace(f160201.DC_CODE))
			{
				return Properties.Resources.P1602010000xamlcs_ChooseDC;
			}

			if (String.IsNullOrEmpty(f160201.ORD_PROP))
			{
				return Properties.Resources.P1602010000_ChooseWorkType;
			}

			if (String.IsNullOrEmpty(f160201.RTN_VNR_TYPE_ID))
			{
				return Properties.Resources.P1602010000_ChooseVNR_RTN_Reason;
			}

			if (String.IsNullOrEmpty(f160201.VNR_CODE))
			{
				return Properties.Resources.P1602010000_ChooseVNR_RTN_No;
			}

			if (f160201.RTN_VNR_DATE == DateTime.MinValue)
			{
				return Properties.Resources.P1602010000_ChooseVNR_RTN_Date;
			}

			if (String.IsNullOrEmpty(f160201.DELIVERY_WAY))
			{
				return Properties.Resources.P1602010000_ChooseDELIVERY_WAY;
			}

			if (detailList == null || detailList.Count == 0)
			{
				return Properties.Resources.P1602010000_NoVNR_RTN_Detail;
			}

			foreach (var data in detailList)
			{
				if (data.Item.RTN_VNR_QTY < 1)
				{
					return Properties.Resources.P1602010000_VNR_RTN_CountInvalid;
				}

				if (string.IsNullOrWhiteSpace(data.Item.RTN_VNR_CAUSE))
				{
					return $"品號：{data.Item.ITEM_CODE}未設定廠退原因";
				}
				//if (string.IsNullOrWhiteSpace(data.Item.MEMO))
				//{
				//	return $"品號：{data.Item.ITEM_CODE}未設定廠退原因說明";
				//}
			}

			if (f160201.SELF_TAKE == "0")
			{
				if ((UserOperateMode == OperateMode.Add && string.IsNullOrEmpty(AddNewF160201.ADDRESS))
				|| (UserOperateMode == OperateMode.Edit && string.IsNullOrEmpty(EditF160201.ADDRESS)))
				{
					return Properties.Resources.P1602010000_NO_Address;
				}
			}


			if (!IsNotEDI)
			{
				foreach (var item in detailList)
				{
					var d = EdiDetailData.Find(o => o.ItemCode == item.Item.ITEM_CODE);

					if (d == null)
					{
						return item.Item.ITEM_CODE + Properties.Resources.P1602010000_NoItem;
					}
				}

				foreach (var a in EdiDetailData)
				{
					int count = a.Count;
					bool isItemExist = false;

					foreach (var item in detailList)
					{
						if (item.Item.ITEM_CODE == a.ItemCode)
						{
							isItemExist = true;
							count -= item.Item.RTN_VNR_QTY;
						}
					}

					if (!isItemExist)
					{
						return a.ItemCode + Properties.Resources.P1602010000_Item_Required;
					}

					if (count != 0)
					{
						return a.ItemCode + Properties.Resources.P1602010000_RTNItemCount_Equal_VNR_RTN_count;
					}
				}
			}

			return String.Empty;
		}
		#endregion

		#region 產生EDI檢核資料
		private void GenerateEdiDetailData(List<F160201ReturnDetail> details)
		{
			EdiDetailData = new List<EdiDetail>();

			foreach (var item in details)
			{
				AddEdiDetailData(EdiDetailData, item);
			}
		}

		private void AddEdiDetailData(List<EdiDetail> ediDetailData, F160201ReturnDetail item)
		{
			bool _isExist = false;

			foreach (var ediDetail in ediDetailData)
			{
				if (ediDetail.ItemCode == item.ITEM_CODE)
				{
					_isExist = true;
					ediDetail.Count += item.RTN_VNR_QTY;
					break;
				}
			}

			if (!_isExist)
			{
				ediDetailData.Add(new EdiDetail() { ItemCode = item.ITEM_CODE, Count = item.RTN_VNR_QTY });
			}
		}
		#endregion

		#region Print

		public Action<PrintType> DoPrintReport = delegate { };

		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
					DoPrint,
					(t) => !IsBusy && UserOperateMode == OperateMode.Query && SelectedF160201 != null);
			}
		}

		private void DoPrint(PrintType printType)
		{
			DoPrintReport(printType);
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
            if (string.IsNullOrEmpty(ImportFilePath)) return;

            string fullFilePath = ImportFilePath;
            var errorMeg = string.Empty;
            var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);

            var queryData = (from col in excelTable.AsEnumerable()
                             select new F1602ImportData
                             {
                                 DC_CODE = Convert.ToString(col[0]),
                                 ORD_PROP = Convert.ToString(col[1]),
                                 RTN_VNR_TYPE_ID = Convert.ToString(col[2]),
                                 RTN_VNR_CAUSE = Convert.ToString(col[3]),
                                 VNR_CODE = Convert.ToString(col[4]),
                                 MEMO = Convert.ToString(col[5]),
                                 WAREHOUSE_ID = Convert.ToString(col[6]),
                                 ITEM_CODE = Convert.ToString(col[7]),
                                 LOC_CODE = Convert.ToString(col[8]),
                                 RTN_VNR_QTY = Convert.ToInt32(col[9]),
                             }).ToList();

            var proxy = new wcf.P16WcfServiceClient();
            var importData = ExDataMapper.MapCollection<F1602ImportData, wcf.F1602ImportData>(queryData).ToArray();
            var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                                                    () => proxy.ImportF1602Data(SelectDcCode, _gupCode, _custCode
                                                                                , fullFilePath, importData));

            DialogService.ShowMessage(result.Message.ToString());


        }

        #endregion ImportExcel

        public bool CheckDetail(bool isAdd, SelectionList<F160201ReturnDetail> F160201DetailList = null, SelectionItem<F160201ReturnDetail> SelectedReturnDetaiItem = null)
        {
            var detail = F160201DetailList;
            var itemCode = (isAdd) ? ITEM_CODE : _oldItemCode;
			      var makeNo = (isAdd) ? MAKE_NO : _oldMakeNo;
            if (F160201DetailList != null)
            {
				        if(isAdd && F160201DetailList.Any(x=> x.Item.ITEM_CODE == itemCode && x.Item.MAKE_NO == makeNo))
								{
									DialogService.ShowMessage("同商品同批號資料重複");
									return false;
								} 
								if(!isAdd && F160201DetailList.Any(x => x.Item.ITEM_CODE == ITEM_CODE && x.Item.MAKE_NO != makeNo && x.Item.MAKE_NO == MAKE_NO))
								{
									DialogService.ShowMessage("同商品同批號資料重複");
									return false;
								}
						}
						if(string.IsNullOrWhiteSpace(RTN_VNR_CAUSE))
						{
							DialogService.ShowMessage("廠退原因必填");
							return false;
						}
						//if (string.IsNullOrWhiteSpace(MEMO))
						//{
						//	DialogService.ShowMessage("廠退原因說明必填");
						//	return false;
						//}
						return true;
        }
        public void ClearSearchProduct()
        {
            ITEM_CODE = "";
            ITEM_COLOR = "";
            ITEM_NAME = "";
            ITEM_SPEC = "";
            ITEM_SIZE = "";
            RTN_VNR_QTY = 0;
            SERIAL_NO = "";
			      RTN_VNR_CAUSE ="";
			      MAKE_NO ="";
			      MEMO = "";
        }
    }
}
