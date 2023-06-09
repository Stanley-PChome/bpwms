using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using F16Entities = Wms3pl.WpfClient.DataServices.F16DataService.F16Entities;
using wcf = Wms3pl.WpfClient.ExDataServices.P16WcfService;


namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1602020000_ViewModel : InputViewModelBase
	{
		public Action DisableDetailDataGrid = () => { };
		public P1602020000_ViewModel()
		{
			UserOperateMode = OperateMode.Query;
			Init();
		}

		#region 初始化

		private void Init()
		{
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

			//查詢條件初始值
			DcCodeForSearch = DcCodes[0].Value;
			CreateDateBeginForSearch = DateTime.Today;
			CreateDateEndForSearch = DateTime.Today;
			//IsSelfTakeForAddNewSearch = false;

			var proxyF19 = GetProxy<F19Entities>();
			var f194704s = proxyF19.F194704s.Where(o => o.DC_CODE == DcCodeForSearch && o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode && o.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode).ToList();
			var f1947s = proxyF19.F1947s.Where(o => o.DC_CODE == DcCodeForSearch).ToList();
			AllIdList = (from a in f194704s
						 join b in f1947s
						 on a.ALL_ID equals b.ALL_ID
						 select new NameValuePair<string> { Name = b.ALL_COMP, Value = a.ALL_ID }).ToList();
			//AllIdList.Insert(0, new NameValuePair<string> { Name = string.Empty, Value = string.Empty });
			var f1909 = proxyF19.F1909s.Where(o => o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode && o.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode).ToList().FirstOrDefault();

			DelvTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F160201", "DELIVERY_WAY");
			//DelvTypeList.Insert(0, new NameValuePair<string> { Value = string.Empty,Name = string.Empty });
			if (f1909 != null && f1909.VNR_RTN_TYPE == "1")
			{
				SelectedDelvType = DelvTypeList.FirstOrDefault(o => o.Value == "0").Value;
				IsDelvTypeUsable = false;
			}
			else
			{
				IsDelvTypeUsable = true;
			}
			WarehouseTypeList = (from o in proxyF19.F190002s.Where(x=> x.TICKET_ID == 4).ToList()
													 join c in proxyF19.F198001s.ToList()
													 on new { TYPE_ID = o.WAREHOUSE_TYPE } equals new { c.TYPE_ID }
													 select new NameValuePair<string>
													 {
														 Name = c.TYPE_NAME,
														 Value = c.TYPE_ID
													 }).ToList();
		}
		#endregion

		#region 操作控制

		#region 查詢頁面
		public Action SetQueryFocus = delegate { };
		private bool _isSearchConditionError; //查詢條件檢查結果

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

		private bool _isShowSearchResult = true;

		public bool IsShowSearchResult
		{
			get { return _isShowSearchResult; }
			set
			{
				_isShowSearchResult = value;
				RaisePropertyChanged("IsShowSearchResult");
			}
		}

		#endregion

		#region 新增頁面

		public Action SetQueryFocusForAddNew = delegate { };
		private bool _isShowSearchConditionForAddNew = true;

		public bool IsShowSearchConditionForAddNew
		{
			get { return _isShowSearchConditionForAddNew; }
			set
			{
				_isShowSearchConditionForAddNew = value;
				RaisePropertyChanged("IsShowSearchConditionForAddNew");
			}
		}

		#endregion

		#endregion

		#region 顯示提示訊息

		private MessagesStruct _messagesStruct;

		public void ShowMessage(string message)
		{
			_messagesStruct.Message = message;
			ShowMessage(_messagesStruct);
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

		#endregion

		#region 新增功能

		//廠退單的查詢結果
		private SelectionList<F160201Data> _addNewSearchResult;

		public SelectionList<F160201Data> AddNewSearchResult
		{
			get { return _addNewSearchResult; }
			set
			{
				_addNewSearchResult = value;
				RaisePropertyChanged("AddNewSearchResult");
			}
		}

		//廠退出貨明細
		private SelectionList<F160204Detail> _addNewReturnWmsDetails;

		public SelectionList<F160204Detail> AddNewReturnWmsDetails
		{
			get { return _addNewReturnWmsDetails; }
			set
			{
				_addNewReturnWmsDetails = value;
				RaisePropertyChanged("AddNewReturnWmsDetails");
			}
		}

		#region 新增按鈕 Command

		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(),
					() => UserOperateMode == OperateMode.Query,
					o => DoAddComplete()
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
		}

		private void DoAddComplete()
		{
			if (DcCodes != null && DcCodes.Count > 0)
			{
				DcCodeForAddNewSearch = DcCodes[0].Value;
			}

			CreateDateBeginForAddNewSearch = DateTime.Today;
			CreateDateEndForAddNewSearch = DateTime.Today;

			if (ReturnTypes.Count > 0)
				ReturnVendorTypeForAddNewSearch = ReturnTypes[0].Value;

			AddNewSearchResult = null;
			AddNewReturnWmsDetails = null;

			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "P1602020000");
			var chgVenderOrd = (from a in proxy.F1909s
								where a.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode
								&& a.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode
								select a.CHG_VENDER_ORD).SingleOrDefault();

			//原廠退單出貨時， Grid完全不開放修改，不能進行勾選，不能改廠退出貨數量
			if (chgVenderOrd == "1")
				DisableDetailDataGrid();

			SelectedDelvType = DelvTypeList.FirstOrDefault().Value;

			SelectedWarehouseType = WarehouseTypeList.FirstOrDefault().Value;
		}

		#endregion

		#region 查詢廠退單區塊中的查詢條件

		private string _dcCodeForAddNewSearch = String.Empty;

		public string DcCodeForAddNewSearch
		{
			get { return _dcCodeForAddNewSearch; }
			set
			{
				_dcCodeForAddNewSearch = value;
				RaisePropertyChanged("DcCodeForAddNewSearch");
			}
		}

		private DateTime? _createDateBeginForAddNewSearch;

		public DateTime? CreateDateBeginForAddNewSearch
		{
			get { return _createDateBeginForAddNewSearch; }
			set
			{
				_createDateBeginForAddNewSearch = value;
				RaisePropertyChanged("CreateDateBeginForAddNewSearch");
			}
		}

		private DateTime? _createDateEndForAddNewSearch;

		public DateTime? CreateDateEndForAddNewSearch
		{
			get { return _createDateEndForAddNewSearch; }
			set
			{
				_createDateEndForAddNewSearch = value;
				RaisePropertyChanged("CreateDateEndForAddNewSearch");
			}
		}

		private DateTime? _returnVendorDateBeginForAddNewSearch;

		public DateTime? ReturnVendorDateBeginForAddNewSearch
		{
			get { return _returnVendorDateBeginForAddNewSearch; }
			set
			{
				_returnVendorDateBeginForAddNewSearch = value;
				RaisePropertyChanged("ReturnVendorDateBeginForAddNewSearch");
			}
		}

		private DateTime? _returnVendorDateEndForAddNewSearch;

		public DateTime? ReturnVendorDateEndForAddNewSearch
		{
			get { return _returnVendorDateEndForAddNewSearch; }
			set
			{
				_returnVendorDateEndForAddNewSearch = value;
				RaisePropertyChanged("ReturnVendorDateEndForAddNewSearch");
			}
		}

		private string _returnVendorNoForAddNewSearch = String.Empty;

		public string ReturnVendorNoForAddNewSearch
		{
			get { return _returnVendorNoForAddNewSearch; }
			set
			{
				_returnVendorNoForAddNewSearch = value;
				RaisePropertyChanged("ReturnVendorNoForAddNewSearch");
			}
		}

		private string _returnVendorTypeForAddNewSearch = String.Empty;

		public string ReturnVendorTypeForAddNewSearch
		{
			get { return _returnVendorTypeForAddNewSearch; }
			set
			{
				_returnVendorTypeForAddNewSearch = value;
				RaisePropertyChanged("ReturnVendorTypeForAddNewSearch");
			}
		}

		private string _vendorCodeForAddNewSearch = String.Empty;

		public string VendorCodeForAddNewSearch
		{
			get { return _vendorCodeForAddNewSearch; }
			set
			{
				_vendorCodeForAddNewSearch = value;
				RaisePropertyChanged("VendorCodeForAddNewSearch");
			}
		}

		private string _vendorNameForAddNewSearch = String.Empty;

		public string VendorNameForAddNewSearch
		{
			get { return _vendorNameForAddNewSearch; }
			set
			{
				_vendorNameForAddNewSearch = value;
				RaisePropertyChanged("VendorNameForAddNewSearch");
			}
		}


		#region 貨主單號for新增查詢用
		private string _custOrdNoForAddNewSearch;

		public string CustOrdNoForAddNewSearch
		{
			get { return _custOrdNoForAddNewSearch; }
			set
			{
				Set(() => CustOrdNoForAddNewSearch, ref _custOrdNoForAddNewSearch, value);
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


		#region 選取的出貨倉別
		private string _selectedWarehouseType;

		public string SelectedWarehouseType
		{
			get { return _selectedWarehouseType; }
			set
			{
				Set(() => SelectedWarehouseType, ref _selectedWarehouseType, value);
			}
		}
		#endregion



		//private bool _isSelfTakeForAddNewSearch;

		//public bool IsSelfTakeForAddNewSearch
		//{
		//	get { return _isSelfTakeForAddNewSearch; }
		//	set
		//	{
		//		_isSelfTakeForAddNewSearch = value;
		//		RaisePropertyChanged("IsSelfTakeForAddNewSearch");
		//		IsNotSelfTake = !value;
		//		SelectedAllId = string.Empty;
		//	}
		//}

		private List<NameValuePair<string>> _allIdList;
		public List<NameValuePair<string>> AllIdList
		{
			get { return _allIdList; }
			set { Set(ref _allIdList, value); }
		}

		private string _selectedAllId;
		public string SelectedAllId
		{
			get { return _selectedAllId; }
			set { Set(ref _selectedAllId, value); }
		}

		private List<NameValuePair<string>> _delvTypeList;
		public List<NameValuePair<string>> DelvTypeList
		{
			get { return _delvTypeList; }
			set { Set(ref _delvTypeList, value); }
		}

		private string _selectedDelvType;
		public string SelectedDelvType
		{
			get { return _selectedDelvType; }
			set { Set(ref _selectedDelvType, value);
				if (!string.IsNullOrWhiteSpace(_selectedDelvType) && _selectedDelvType == "2")
					IsNotSelfTake = true;
				else
				{
					IsNotSelfTake = false;
					if(AllIdList.Any())
						SelectedAllId = AllIdList.FirstOrDefault().Value;
				}
			}
		}		

		private bool _isNotSelfTake;
		public bool IsNotSelfTake
		{
			get { return _isNotSelfTake; }
			set { Set(ref _isNotSelfTake, value); }
		}

		private bool _isDelvTypeUsable;
		public bool IsDelvTypeUsable
		{
			get { return _isDelvTypeUsable; }
			set { Set(ref _isDelvTypeUsable, value); }
		}
		#endregion

		#region 查詢廠退單區塊中的查詢按鈕

		public ICommand AddNewSearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddNewSearch(),
					() => UserOperateMode == OperateMode.Add,
					o => DoAddNewSearchComplete()
					);
			}
		}

		private void DoAddNewSearch()
		{
			IsShowSearchConditionForAddNew = true;
			_isSearchConditionError = false;

			//檢查查詢條件
			if (!CreateDateBeginForAddNewSearch.HasValue || !CreateDateEndForAddNewSearch.HasValue)
			{
				_messagesStruct.Message = Properties.Resources.P1602010000_CRT_DateEmpty;
				ShowMessage(_messagesStruct);
				_isSearchConditionError = true;
				return;
			}

			if (CreateDateBeginForAddNewSearch.Value > CreateDateEndForAddNewSearch.Value)
			{
				DateTime temp1 = CreateDateBeginForAddNewSearch.Value;
				CreateDateBeginForSearch = CreateDateEndForAddNewSearch.Value;
				CreateDateEndForAddNewSearch = temp1;
			}

			//if (!ReturnVendorDateBeginForAddNewSearch.HasValue || !ReturnVendorDateEndForAddNewSearch.HasValue)
			//{
			//	_messagesStruct.Message = Properties.Resources.P1602020000_VNR_RTN_Date_Required;
			//	ShowMessage(_messagesStruct);
			//	_isSearchConditionError = true;
			//	return;
			//}

			if (ReturnVendorDateBeginForAddNewSearch.HasValue && ReturnVendorDateEndForAddNewSearch.HasValue && ReturnVendorDateBeginForAddNewSearch.Value > ReturnVendorDateEndForAddNewSearch.Value)
			{
				DateTime temp1 = ReturnVendorDateBeginForAddNewSearch.Value;
				ReturnVendorDateBeginForAddNewSearch = ReturnVendorDateEndForAddNewSearch.Value;
				ReturnVendorDateEndForAddNewSearch = temp1;
			}
			if(ReturnVendorDateEndForAddNewSearch.HasValue)
				ReturnVendorDateEndForAddNewSearch = ReturnVendorDateEndForAddNewSearch.Value.AddDays(1);

			var proxy = GetExProxy<P16ExDataSource>();

			List<F160201Data> data = proxy.CreateQuery<F160201Data>("GetF160201DatasNotFinish")
				.AddQueryExOption("dcCode", DcCodeForAddNewSearch)
				.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
				.AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
				.AddQueryExOption("createBeginDateTime", CreateDateBeginForAddNewSearch)
				.AddQueryExOption("createEndDateTime", CreateDateEndForAddNewSearch.Value.AddDays(1))
				.AddQueryExOption("returnBeginDateTime", ReturnVendorDateBeginForAddNewSearch)
				.AddQueryExOption("returnEndDateTime", ReturnVendorDateEndForAddNewSearch)
				.AddQueryExOption("deliveryWay", SelectedDelvType)
				.AddQueryExOption("returnNo", ReturnVendorNoForAddNewSearch)
				.AddQueryExOption("returnType", ReturnVendorTypeForAddNewSearch)
				.AddQueryExOption("vendorCode", VendorCodeForAddNewSearch)
				.AddQueryExOption("vendorName", VendorNameForAddNewSearch)
				.AddQueryExOption("typeId",SelectedWarehouseType)
				.AddQueryExOption("custOrdNo",CustOrdNoForAddNewSearch)
				.ToList();

			AddNewSearchResult = new SelectionList<F160201Data>(data);
		}

		private void DoAddNewSearchComplete()
		{
			if (_isSearchConditionError) return;
			SetQueryFocus();

			if (AddNewSearchResult == null)
			{
				_messagesStruct.Message = Properties.Resources.P1602010000_NoData;
				ShowMessage(_messagesStruct);
			}
			else
			{
				if (!AddNewSearchResult.Any())
				{
					_messagesStruct.Message = Properties.Resources.P1602010000_NoData;
					ShowMessage(_messagesStruct);
				}
				else
				{
					IsShowSearchResult = true;
				}
			}
		}

		#endregion

		#region 查詢廠退單區塊中的 DataGrid

		private bool _isAddNewSearchResultSelectedAll = false;

		public bool IsAddNewSearchResultSelectedAll
		{
			get { return _isAddNewSearchResultSelectedAll; }
			set
			{
				_isAddNewSearchResultSelectedAll = value;
				RaisePropertyChanged("IsAddNewSearchResultSelectedAll");
			}
		}

		public ICommand AddNewSearchResultSelectedAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddNewSearchResultSelectedAll()
					);
			}
		}

		public void DoAddNewSearchResultSelectedAll()
		{
			if (UserOperateMode == OperateMode.Add && AddNewSearchResult != null)
			{
				foreach (var p in AddNewSearchResult)
				{
					p.IsSelected = IsAddNewSearchResultSelectedAll;
				}
			}
		}

		private SelectionItem<F160201Data> _addNewSearchResultSelectedItem;

		public SelectionItem<F160201Data> AddNewSearchResultSelectedItem
		{
			get { return _addNewSearchResultSelectedItem; }
			set
			{
				_addNewSearchResultSelectedItem = value;
				RaisePropertyChanged("AddNewSearchResultSelectedItem");
			}
		}


		#endregion

		#region 匯入明細 - 將查詢廠退單區塊中的 DataGrid 所勾選的明細匯入


		//取得查詢廠退單區塊中的 DataGrid 所勾選的廠退單
		public List<F160201Data> GetCheckItemsFromAddNewSearchResult()
		{
			List<F160201Data> result = new List<F160201Data>();

			if (UserOperateMode == OperateMode.Add)
			{
				if (AddNewSearchResult != null)
				{
					var sel = AddNewSearchResult.Where(x => x.IsSelected == true).ToList();
					result = (from i in sel select i.Item).ToList();
				}
			}
			return result;
		}

		public ICommand ImportDetailDataCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoImportDetailData(),
					() => GetCheckItemsFromAddNewSearchResult().Any(),
					o => DoImportDetailDataComplete()
					);
			}
		}

		private void DoImportDetailData()
		{
			if (AddNewReturnWmsDetails == null)
			{
				AddNewReturnWmsDetails = new SelectionList<F160204Detail>(new List<F160204Detail>());
			}
		}

		private void DoImportDetailDataComplete()
		{
			List<F160201Data> f160201Datas = GetCheckItemsFromAddNewSearchResult();
			ResetCheckBoxAfterImport();

			if (f160201Datas != null && f160201Datas.Count > 0)
			{
				var compareDeliveryWay = AddNewReturnWmsDetails.Any() ? AddNewReturnWmsDetails.First().Item.DELIVERY_WAY : f160201Datas.First().DELIVERY_WAY;
				var compareTypeId = AddNewReturnWmsDetails.Any() ? AddNewReturnWmsDetails.First().Item.TYPE_ID : f160201Datas.First().TYPE_ID;
				var compareItemTel = AddNewReturnWmsDetails.Any() ? AddNewReturnWmsDetails.First().Item.ITEM_TEL : f160201Datas.First().ITEM_TEL;
				var compareItemContact = AddNewReturnWmsDetails.Any() ? AddNewReturnWmsDetails.First().Item.ITEM_CONTACT : f160201Datas.First().ITEM_CONTACT;
				var compareItemAddress = AddNewReturnWmsDetails.Any() ? AddNewReturnWmsDetails.First().Item.ADDRESS : f160201Datas.First().ADDRESS;
				var compareVnrCode = AddNewReturnWmsDetails.Any() ? AddNewReturnWmsDetails.First().Item.VNR_CODE : f160201Datas.First().VNR_CODE;
				var compareOrdProp = AddNewReturnWmsDetails.Any() ? AddNewReturnWmsDetails.First().Item.ORD_PROP : f160201Datas.First().ORD_PROP;
				foreach (var f160201Data in f160201Datas)
        {
					if(f160201Data.DELIVERY_WAY != compareDeliveryWay)
					{
						_messagesStruct.Message = string.Format(Properties.Resources.P1602020000_DiffDeliveryWay, f160201Data.RTN_VNR_NO);
						ShowMessage(_messagesStruct);
						return;
					}
					if (f160201Data.TYPE_ID != compareTypeId)
					{
						_messagesStruct.Message = string.Format(Properties.Resources.P1602020000_DiffTypeId, f160201Data.RTN_VNR_NO);
						ShowMessage(_messagesStruct);
						return;
					}
					if (f160201Data.ITEM_TEL != compareItemTel)
					{
						_messagesStruct.Message = string.Format(Properties.Resources.P1602020000_DiffItemTel, f160201Data.RTN_VNR_NO);
						ShowMessage(_messagesStruct);
						return;
					}
					if (f160201Data.ITEM_CONTACT != compareItemContact)
					{
						_messagesStruct.Message = string.Format(Properties.Resources.P1602020000_DiffItemContact, f160201Data.RTN_VNR_NO);
						ShowMessage(_messagesStruct);
						return;
					}
					if (f160201Data.ADDRESS != compareItemAddress)
					{
						_messagesStruct.Message = string.Format(Properties.Resources.P1602020000_DiffItemAddress, f160201Data.RTN_VNR_NO);
						ShowMessage(_messagesStruct);
						return;
					}
					if (f160201Data.VNR_CODE != compareVnrCode)
					{
						_messagesStruct.Message = string.Format(Properties.Resources.P1602020000_DiffVnrCode, f160201Data.RTN_VNR_NO);
						ShowMessage(_messagesStruct);
						return;
					}
					if (f160201Data.ORD_PROP != compareOrdProp)
					{
						_messagesStruct.Message = string.Format(Properties.Resources.P1602020000_DiffOrdProp, f160201Data.RTN_VNR_NO);
						ShowMessage(_messagesStruct);
						return;
					}
					if(AddNewReturnWmsDetails.Any(x=> x.Item.RTN_VNR_NO == f160201Data.RTN_VNR_NO))
					{
						_messagesStruct.Message = string.Format(Properties.Resources.P1602020000_SameRtnVnrNo, f160201Data.RTN_VNR_NO);
						ShowMessage(_messagesStruct);
						return;
					}
        }

				//產生AddNewReturnWmsDetails
				var proxy = GetExProxy<P16ExDataSource>();

				foreach (var f160201Data in f160201Datas)
				{
					List<F160204Detail> data = proxy.CreateQuery<F160204Detail>("ConvertToF160204Detail")
						.AddQueryExOption("dcCode", DcCodeForAddNewSearch)
						.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
						.AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
						.AddQueryExOption("returnNo", f160201Data.RTN_VNR_NO)
						.ToList();

					string message = String.Empty;

					foreach (var f in data)
					{
						f.RTN_VNR_NO = f160201Data.RTN_VNR_NO;
						f.VNR_CODE = f160201Data.VNR_CODE;
						f.VNR_NAME = f160201Data.VNR_NAME;
						f.ORD_PROP = f160201Data.ORD_PROP;
						f.DELIVERY_WAY = f160201Data.DELIVERY_WAY;
						f.TYPE_ID = f160201Data.TYPE_ID;
						f.ITEM_TEL = f160201Data.ITEM_TEL;
						f.ITEM_CONTACT = f160201Data.ITEM_CONTACT;
						f.ADDRESS = f160201Data.ADDRESS;
						SelectionItem<F160204Detail> selectionItem = new SelectionItem<F160204Detail>(f);
						AddNewReturnWmsDetails.Add(selectionItem);
					}
				}
			}

			IsAddNewReturnWmsDetailsSelectedAll = false;
			IsAddNewSearchResultSelectedAll = false;
		}


		private void ResetCheckBoxAfterImport()
		{
			if (AddNewSearchResult != null)
			{
				foreach (var item in AddNewSearchResult)
				{
					item.IsSelected = false;
				}
			}
		}

		#endregion

		#region 刪除明細 - 將已匯入廠退出貨明細的 DataGrid 所勾選的明細刪除

		//取得查詢廠退單區塊中的 DataGrid 所勾選的廠退單
		public List<F160204Detail> GetCheckItemsFromAddNewReturnWmsDetails()
		{
			List<F160204Detail> result = new List<F160204Detail>();

			if (UserOperateMode == OperateMode.Add)
			{
				if (AddNewReturnWmsDetails != null)
				{
					var sel = AddNewReturnWmsDetails.Where(x => x.IsSelected == true).ToList();
					result = (from i in sel select i.Item).ToList();
				}
			}

			return result;
		}

		public ICommand DeleteDetailDataCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDeleteDetailData(),
					() => GetCheckItemsFromAddNewReturnWmsDetails().Any(),
					o => DoDeleteDetailDataComplete()
					);
			}
		}

		private void DoDeleteDetailData()
		{

		}

		private void DoDeleteDetailDataComplete()
		{
			if (AddNewReturnWmsDetails == null) return;
			var sel = AddNewReturnWmsDetails.Where(x => x.IsSelected == false).ToList();
			List<F160204Detail> result = (from i in sel select i.Item).ToList();
			AddNewReturnWmsDetails = new SelectionList<F160204Detail>(result);
			IsAddNewReturnWmsDetailsSelectedAll = false;
			IsAddNewSearchResultSelectedAll = false;
		}

		#endregion

		#region 廠退出貨明細

		private bool _isAddNewReturnWmsDetailsSelectedAll = false;

		public bool IsAddNewReturnWmsDetailsSelectedAll
		{
			get { return _isAddNewReturnWmsDetailsSelectedAll; }
			set
			{
				_isAddNewReturnWmsDetailsSelectedAll = value;
				RaisePropertyChanged("IsAddNewReturnWmsDetailsSelectedAll");
			}
		}

		public ICommand AddNewReturnWmsDetailsSelectedAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddNewReturnWmsDetailsSelectedAll()
					);
			}
		}

		public void DoAddNewReturnWmsDetailsSelectedAll()
		{
			if (UserOperateMode == OperateMode.Add && AddNewReturnWmsDetails != null)
			{
				foreach (var p in AddNewReturnWmsDetails)
				{
					p.IsSelected = IsAddNewReturnWmsDetailsSelectedAll;
				}
			}
		}

		#endregion


		#endregion

		#region 查詢功能

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

		//廠退出貨單號
		private string _returnWmsNo = String.Empty;

		public string ReturnWmsNo
		{
			get { return _returnWmsNo; }
			set
			{
				_returnWmsNo = value;
				RaisePropertyChanged("ReturnWmsNo");
			}
		}

		//廠退單號
		private string _returnVendorNo = String.Empty;

		public string ReturnVendorNo
		{
			get { return _returnVendorNo; }
			set
			{
				_returnVendorNo = value;
				RaisePropertyChanged("ReturnVendorNo");
			}
		}

		//出貨單號
		private string _wmsOrderNo = String.Empty;

		public string WmsOrderNo
		{
			get { return _wmsOrderNo; }
			set
			{
				_wmsOrderNo = value;
				RaisePropertyChanged("WmsOrderNo");
			}
		}

        /// <summary>
        /// 員工編號
        /// </summary>
        private string _empId = string.Empty;
        public string EmpId
        {
            get { return _empId; }
            set
            {
                _empId = value;
                RaisePropertyChanged("EmpId");
            }
        }

        /// <summary>
        /// 員工姓名
        /// </summary>
        private string _empName = string.Empty;
        public string EmpName
        {
            get { return _empName; }
            set
            {
                _empName = value;
                RaisePropertyChanged("EmpName");
            }
        }


		#region
		private string _custOrdNo;

		public string CustOrdNo
		{
			get { return _custOrdNo; }
			set
			{
				Set(() => CustOrdNo, ref _custOrdNo, value);
			}
		}
		#endregion

		#endregion

		#region 查詢結果及細項

		private List<F160204SearchResult> _searchResultForSerach;

		public List<F160204SearchResult> SearchResultForSerach
		{
			get { return _searchResultForSerach; }
			set
			{
				_searchResultForSerach = value;
				RaisePropertyChanged("SearchResultForSerach");
			}
		}

		private F160204SearchResult _searchResultForSerachSelectedItem;

		public F160204SearchResult SearchResultForSerachSelectedItem
		{
			get { return _searchResultForSerachSelectedItem; }
			set
			{
				_searchResultForSerachSelectedItem = value;
				RaisePropertyChanged("SearchResultForSerachSelectedItem");
				SearchDetail();
				SearchOrder();
			}
		}

		private List<F160204SearchResult> _searchResultForSerachDetail;

		public List<F160204SearchResult> SearchResultForSerachDetail
		{
			get { return _searchResultForSerachDetail; }
			set
			{
				_searchResultForSerachDetail = value;
				RaisePropertyChanged("SearchResultForSerachDetail");
			}
		}

		private ObservableCollection<F050801WmsOrdNo> _f050801Q;
		public ObservableCollection<F050801WmsOrdNo> F050801Q
		{
			get { return _f050801Q; }
			set
			{
				_f050801Q = value;
				RaisePropertyChanged("F050801Q");
			}
		}

		#endregion

		#region 查詢按鈕 Command

		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			IsShowSearchCondition = true;
			_isSearchConditionError = false;
			SearchResultForSerachDetail = null;
			F050801Q = null;

			//檢查查詢條件
			if (!CreateDateBeginForSearch.HasValue || !CreateDateEndForSearch.HasValue)
			{
				_messagesStruct.Message = Properties.Resources.P1602010000_CRT_DateEmpty;
				ShowMessage(_messagesStruct);
				_isSearchConditionError = true;
				return;
			}

			if (CreateDateBeginForSearch.Value > CreateDateEndForSearch.Value)
			{
				DateTime temp1 = CreateDateBeginForSearch.Value;
				CreateDateBeginForSearch = CreateDateEndForSearch.Value;
				CreateDateEndForSearch = temp1;
			}

			var proxy = GetExProxy<P16ExDataSource>();

			SearchResultForSerach = proxy.CreateQuery<F160204SearchResult>("GetF160204SearchResult")
				.AddQueryExOption("dcCode", DcCodeForSearch)
				.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
				.AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
				.AddQueryExOption("createBeginDateTime", CreateDateBeginForSearch.Value.ToString("yyyy/MM/dd"))
				.AddQueryExOption("createEndDateTime", CreateDateEndForSearch.Value.ToString("yyyy/MM/dd"))
				.AddQueryExOption("returnWmsNo", ReturnWmsNo)
				.AddQueryExOption("returnVnrNo", ReturnVendorNo)
				.AddQueryExOption("orderNo", WmsOrderNo)
        .AddQueryExOption("empId",EmpId)
        .AddQueryExOption("empName",EmpName)
				.AddQueryExOption("custOrdNo",CustOrdNo)
				.ToList();

    }

		private void DoSearchComplete()
		{
			if (_isSearchConditionError) return;
			SetQueryFocus();

			if (SearchResultForSerach == null)
			{
				_messagesStruct.Message = Properties.Resources.P1602010000_NoData;
				ShowMessage(_messagesStruct);
			}
			else
			{
				if (!SearchResultForSerach.Any())
				{
					_messagesStruct.Message = Properties.Resources.P1602010000_NoData;
					ShowMessage(_messagesStruct);
				}
				else
				{
					SearchResultForSerachSelectedItem = SearchResultForSerach[0];
					IsShowSearchCondition = false;
					IsShowSearchResult = true;
				}
			}
		}

		#endregion

		#region 單據明細

		private void SearchDetail()
		{
			if (SearchResultForSerachSelectedItem == null)
			{
				return;
			}

			var proxy = GetExProxy<P16ExDataSource>();

			SearchResultForSerachDetail = proxy.CreateQuery<F160204SearchResult>("GetF160204SearchResultDetail")
				.AddQueryExOption("dcCode", DcCodeForSearch)
				.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
				.AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
				.AddQueryExOption("returnWmsNo", SearchResultForSerachSelectedItem.RTN_WMS_NO)
				.ToList();
		}

		#endregion

		#region

		private void SearchOrder()
		{
			if (SearchResultForSerachSelectedItem == null)
			{
				return;
			}

			//var proxy = GetProxy<F05Entities>();
			//SearchF050001 = proxy.F050001s.Where(x => x.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode &&
			//										  x.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode &&
			//										  x.SOURCE_NO == SearchResultForSerachSelectedItem.RTN_WMS_NO).ToList();

			var proxyShare = GetExProxy<ShareExDataSource>();
			F050801Q = proxyShare.CreateQuery<F050801WmsOrdNo>("GetF050801ListBySourceNo")
								.AddQueryExOption("dcCode", DcCodeForSearch)
								.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
								.AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
								.AddQueryExOption("sourceNo", SearchResultForSerachSelectedItem.RTN_WMS_NO).ToList().ToObservableCollection();

		}

		#endregion

		#endregion

		#region 確認(儲存)

		private bool _checkError;

		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode != OperateMode.Query,
					o => DoSaveComplete()
					);
			}
		}

		private void DoSave()
		{
			_checkError = false;
			bool isConfirm;
			_messagesStruct.Message = CheckInputError(out isConfirm);

			if (!String.IsNullOrEmpty(_messagesStruct.Message))
			{
				if (!isConfirm)
				{
					ShowMessage(_messagesStruct);
					_checkError = true;
				}
				else
				{
					if (ShowConfirmMessage(_messagesStruct.Message) == UILib.Services.DialogResponse.No)
						_checkError = true;
				}

			}
		}

		private void DoSaveComplete()
		{
			if (_checkError) return;

			foreach (var item in AddNewReturnWmsDetails)
			{
				item.Item.RTN_WMS_QTY = item.Item.RTN_VNR_QTY_REMAINDER;
				item.Item.DISTR_CAR = string.Empty;
				item.Item.ALL_ID = string.Empty;
			}

			

			var f160204s =
				ExDataMapper.MapCollection<F160204Detail, wcf.F160204>(AddNewReturnWmsDetails.Select(si => si.Item)).ToArray();
			var proxy = new wcf.P16WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertF160204(f160204s));
			DialogService.ShowMessage(result.Message);

			if (result.IsSuccessed)
			{
				UserOperateMode = OperateMode.Query;
				DoSearch();

				if (SearchResultForSerach != null && SearchResultForSerach.Count > 0)
					SearchResultForSerachSelectedItem = SearchResultForSerach.Last();
			}
		}

		private string CheckInputError(out bool isConfirm)
		{
			isConfirm = false;
			if (AddNewReturnWmsDetails == null || AddNewReturnWmsDetails.Count == 0)
			{
				return Properties.Resources.P1602020000_VNR_RTN_Detail_NotExportYet;
			}

			//檢查指定出貨倉別的庫存是否足夠
			var itemRtnQtys = AddNewReturnWmsDetails.GroupBy(x=> new { x.Item.ITEM_CODE, x.Item.MAKE_NO }).Select(a => new wcf.ItemRtnQty { ItemCode = a.Key.ITEM_CODE, MakeNo = a.Key.MAKE_NO, RtnQty = a.Sum(x=> x.Item.RTN_VNR_QTY_REMAINDER) });
			var proxy = new wcf.P16WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
				() => proxy.ValidateVendorRtnStack(
					DcCodeForAddNewSearch,
					Wms3plSession.Get<GlobalInfo>().GupCode,
					Wms3plSession.Get<GlobalInfo>().CustCode,
					AddNewReturnWmsDetails.First().Item.TYPE_ID,
					itemRtnQtys.ToArray()));
			if (!string.IsNullOrEmpty(result.Message))
			{
				isConfirm = result.IsSuccessed;
				return result.Message;
			}

			foreach (var item in AddNewReturnWmsDetails)
			{
				if (item.Item.RTN_VNR_QTY_GRAND_TOTAL == item.Item.RTN_VNR_QTY_SUM)
				{
					return item.Item.ITEM_CODE + Properties.Resources.P1602020000_GrandVNR_RTNCountValid;
				}

				if (item.Item.RTN_VNR_QTY_REMAINDER > item.Item.RTN_VNR_QTY_SUM)
				{
					return item.Item.ITEM_CODE + Properties.Resources.P1602020000_VNR_RTN_DELV_Invalid;
				}

				if (item.Item.RTN_VNR_QTY_SUM > item.Item.RTN_VNR_QTY_GRAND_TOTAL && item.Item.RTN_VNR_QTY_REMAINDER <= 0)
				{
					return item.Item.ITEM_CODE + Properties.Resources.P1602020000_VNR_RTN_DELV_Empty;
				}
			}

			var f19Proxy = GetProxy<F19Entities>();
			var f1909 = f19Proxy.F1909s.Where(x => x.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode && x.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode).FirstOrDefault();
			if(f1909!=null)
			{
				switch(f1909.VNR_RTN_TYPE)
				{
					case "0":
						return string.Empty;
					case "1":
						isConfirm = true;
						return Properties.Resources.P1602020000_ConfirmSystemToMinusStock;
				}
			}
			return String.Empty;
		}

		#endregion

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query, o => DoCancelComplete()
					);
			}
		}

		private void DoCancel()
		{
			DcCodeForSearch = DcCodes[0].Value;
			CreateDateBeginForSearch = DateTime.Today;
			CreateDateEndForSearch = DateTime.Today;
			IsShowSearchResult = true;
			IsShowSearchCondition = true;
		}

		private void DoCancelComplete()
		{
			UserOperateMode = OperateMode.Query;
			AddNewSearchResult = null;
			AddNewReturnWmsDetails = null;
			ReturnVendorNoForAddNewSearch = String.Empty;
			ReturnVendorTypeForAddNewSearch = String.Empty;
			VendorCodeForAddNewSearch = String.Empty;
			VendorNameForAddNewSearch = String.Empty;
			//IsSelfTakeForAddNewSearch = false;
			SelectedDelvType = DelvTypeList.FirstOrDefault().Value;
			ReturnWmsNo = String.Empty;
			ReturnVendorNo = String.Empty;
			WmsOrderNo = String.Empty;
			SearchResultForSerach = null;
			SearchResultForSerachDetail = null;
			F050801Q = null;
		}
		#endregion Cancel
	}
}
