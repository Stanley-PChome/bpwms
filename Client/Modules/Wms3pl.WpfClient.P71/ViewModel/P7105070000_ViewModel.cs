using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7105070000_ViewModel : InputViewModelBase
	{
		private string _userId;
		private string _userName;
		private string _gupCode;
		private string _custCode;
		public P7105070000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitContorls();
			}

		}

		private void InitContorls()
		{
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList != null && DcList.Any()) SelectedDc = DcList.First().Value;

			//配送商
			SetAllIdList(SelectedDc);

			//物流類別
			LogiTypeList = GetLogiTypeList();
			if (LogiTypeList != null && LogiTypeList.Any()) SelectedLogiType = LogiTypeList.First().Value;

			//單據類型
			CustTypeList = GetCustTypeList();
			if (CustTypeList != null && CustTypeList.Any()) SelectedCustType = CustTypeList.First().Value;

			//稅別
			TaxTypeList = GetTaxTypeList();
			if (TaxTypeList != null && TaxTypeList.Any()) SelectedTaxType = TaxTypeList.First().Value;

			//計費方式
			AccKindList = GetAccKindList();
			if (AccKindList != null && AccKindList.Any()) SelectedAccKind = AccKindList.First().Value;

			//狀態
			StatusList = GetStatusList();
			if (StatusList != null && StatusList.Any()) SelectedStatus = StatusList.First().Value;

			//配送溫層
			DelvTmprList = GetDelvTmprList();

			//計費類型
			AccTypeList = GetAccTypeList();

			//配送效率
			DelvEfficList = GetDelvEfficList();
		}

		private static List<NameValuePair<string>> SetAllList(List<NameValuePair<string>> tmpList)
		{
			tmpList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });
			return tmpList;
		}



		private void SetAllIdList(string dcCode)
		{
			var result = GetAllIdList(dcCode);
			SetAllList(result);
			AllIdList = result;
			if (AllIdList != null && AllIdList.Any()) SelectedAllId = AllIdList.First().Value;
		}

		private bool _showQueryDetail;

		public bool ShowQueryDetail
		{
			get { return _showQueryDetail; }
			set
			{
				if (_showQueryDetail == value)
					return;
				Set(() => ShowQueryDetail, ref _showQueryDetail, value);
			}
		}

		#region 下拉選單變更時呼叫
		public void SetDataSourceAfterDcCodeSelected()
		{
			if (EditData == null) return;

			//配送商
			EditData.ALL_ID = string.Empty;
			AllIdListForEdit = GetAllIdList(EditData.DC_CODE);
			if (TempData != null)
			{
				if (AllIdListForEdit.Any(x => x.Value == TempData.ALL_ID))
				{
					EditData.ALL_ID = TempData.ALL_ID;
					TempData.ALL_ID = string.Empty;
				}
			}

			//配送區域
			EditData.ACC_AREA_ID = 0;
			AccAreaListForEdit = null;

			//單量級距
			EditData.ACC_DELVNUM_ID = 0;
			AccDelvNumListForEdit = null;
		}

		public void SetDataSourceAfterAllIdSelected()
		{
			if (EditData == null) return;

			//配送區域
			EditData.ACC_AREA_ID = 0;
			AccAreaListForEdit = GetAccAreaList(EditData.DC_CODE, EditData.ALL_ID);

			//單量級距
			EditData.ACC_DELVNUM_ID = 0;
			AccDelvNumListForEdit = GetAccDelvNumList(EditData.DC_CODE, EditData.ALL_ID);

			if (TempData != null)
			{
				if (AccAreaListForEdit.Any(x => x.Value == TempData.ACC_AREA_ID.ToString()))
				{
					EditData.ACC_AREA_ID = TempData.ACC_AREA_ID;
					TempData.ACC_AREA_ID = 0;
				}
				if (AccDelvNumListForEdit.Any(x => x.Value == TempData.ACC_DELVNUM_ID.ToString()))
				{
					EditData.ACC_DELVNUM_ID = TempData.ACC_DELVNUM_ID;
					TempData.ACC_DELVNUM_ID = 0;
				}
			}
		}

		public void SetInputData()
		{
			if (EditData == null) return;

			EditData.BASIC_VALUE = 0;
			EditData.MAX_WEIGHT = 0;
			EditData.FEE = 0;
			EditData.OVER_VALUE = null;
			EditData.OVER_UNIT_FEE = null;

			if (TempData != null)
			{
				EditData.BASIC_VALUE = TempData.BASIC_VALUE;
				EditData.MAX_WEIGHT = TempData.MAX_WEIGHT;
				EditData.FEE = TempData.FEE;
				EditData.OVER_VALUE = TempData.OVER_VALUE;
				EditData.OVER_UNIT_FEE = TempData.OVER_UNIT_FEE;
				TempData.BASIC_VALUE = 0;
				TempData.MAX_WEIGHT = 0;
				TempData.FEE = 0;
				TempData.OVER_VALUE = null;
				TempData.OVER_UNIT_FEE = null;
			}

			//UI未顯示欄位，避免檢核錯誤
			if (EditData.ACC_KIND == "E" || EditData.ACC_KIND == "F")
				EditData.MAX_WEIGHT = Convert.ToDecimal(0.01);
			if (EditData.ACC_KIND == "E" || EditData.ACC_KIND == "F")
				EditData.BASIC_VALUE = Convert.ToDecimal(1);
		}
		#endregion

		#region 資料來源
		#region 物流中心
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.OrderBy(x => x.Value).ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}

		public List<NameValuePair<string>> DcListForEdit
		{
			get { return _dcList.Where(x => !string.IsNullOrEmpty(x.Value)).OrderBy(x => x.Value).ToList(); }
		}

		//選取的物流中心
		private string _selectedDc = string.Empty;

		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				_selectedDc = value;
				SetAllIdList(_selectedDc);
			}
		}
		#endregion

		#region 配送商(F1947)
		private List<NameValuePair<string>> _allIdList;

		public List<NameValuePair<string>> AllIdList
		{
			get { return _allIdList; }
			set
			{
				if (_allIdList == value) return;
				Set(() => AllIdList, ref _allIdList, value);
			}
		}

		private List<NameValuePair<string>> _allIdListForEdit;

		public List<NameValuePair<string>> AllIdListForEdit
		{
			get { return _allIdListForEdit; }
			set
			{
				if (_allIdListForEdit == value) return;
				Set(() => AllIdListForEdit, ref _allIdListForEdit, value);
			}
		}

		private List<NameValuePair<string>> _allIdListForSelected;

		public List<NameValuePair<string>> AllIdListForSelected
		{
			get { return _allIdListForSelected; }
			set
			{
				if (_allIdListForSelected == value) return;
				Set(() => AllIdListForSelected, ref _allIdListForSelected, value);
			}
		}

		//選取的配送商
		private string _selectedAllId;

		public string SelectedAllId
		{
			get { return _selectedAllId; }
			set
			{
				if (_selectedAllId == value) return;
				Set(() => SelectedAllId, ref _selectedAllId, value);
			}
		}

		//取得配送商
		private List<NameValuePair<string>> GetAllIdList(string dcCode)
		{
			if (string.IsNullOrWhiteSpace(dcCode))
				return new List<NameValuePair<string>>();

			var proxy = GetProxy<F19Entities>();
			var results = (from a in proxy.F1947s
						   where a.DC_CODE == dcCode
						   select new NameValuePair<string>()
						   {
							   Value = a.ALL_ID,
							   Name = a.ALL_COMP
						   }).ToList();
			return results;
		}
		#endregion

		#region 物流類別(01:正物流 02:逆物流)  F000904
		private List<NameValuePair<string>> _logiTypeList;

		public List<NameValuePair<string>> LogiTypeList
		{
			get { return _logiTypeList; }
			set
			{
				if (_logiTypeList == value) return;
				Set(() => LogiTypeList, ref _logiTypeList, value);
			}
		}

		public List<NameValuePair<string>> LogiTypeListForEdit
		{
			get { return _logiTypeList.Where(x => !string.IsNullOrEmpty(x.Value)).OrderBy(x => x.Value).ToList(); ; }
		}

		//選取的物流類別
		private string _selectedLogiType;

		public string SelectedLogiType
		{
			get { return _selectedLogiType; }
			set
			{
				if (_selectedLogiType == value) return;
				Set(() => SelectedLogiType, ref _selectedLogiType, value);
			}
		}

		//取得物流類別
		private List<NameValuePair<string>> GetLogiTypeList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F194707", "LOGI_TYPE");
			SetAllList(data);
			return data;
		}
		#endregion

		#region 單據類型(0:B2B 1:B2C)  F000904
		private List<NameValuePair<string>> _custTypeList;

		public List<NameValuePair<string>> CustTypeList
		{
			get { return _custTypeList; }
			set
			{
				if (_custTypeList == value) return;
				Set(() => CustTypeList, ref _custTypeList, value);
			}
		}

		public List<NameValuePair<string>> CustTypeListForEdit
		{
			get { return _custTypeList.Where(x => !string.IsNullOrEmpty(x.Value)).OrderBy(x => x.Value).ToList(); ; }
		}

		//選取的單據類型
		private string _selectedCustType;

		public string SelectedCustType
		{
			get { return _selectedCustType; }
			set
			{
				if (_selectedCustType == value) return;
				Set(() => SelectedCustType, ref _selectedCustType, value);
			}
		}

		//取得單據類型
		private List<NameValuePair<string>> GetCustTypeList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F194707", "CUST_TYPE");
			SetAllList(data);
			return data;
		}
		#endregion

		#region 稅別(0:未稅 1:含稅)
		private List<NameValuePair<string>> _taxTypeList;

		public List<NameValuePair<string>> TaxTypeList
		{
			get { return _taxTypeList; }
			set
			{
				if (_taxTypeList == value) return;
				Set(() => TaxTypeList, ref _taxTypeList, value);
			}
		}

		public List<NameValuePair<string>> TaxTypeListForEdit
		{
			get { return _taxTypeList.Where(x => !string.IsNullOrEmpty(x.Value)).OrderBy(x => x.Value).ToList(); ; }
		}

		//選取的稅別
		private string _selectedTaxType;

		public string SelectedTaxType
		{
			get { return _selectedTaxType; }
			set
			{
				if (_selectedTaxType == value) return;
				Set(() => SelectedTaxType, ref _selectedTaxType, value);
			}
		}

		//取得稅別
		private List<NameValuePair<string>> GetTaxTypeList()
		{
			var data = new List<NameValuePair<string>>
        {
            new NameValuePair<string>(Properties.Resources.P7105070000_ViewModel_NotTax, "0"),
            new NameValuePair<string>(Properties.Resources.IS_TAX, "1")
        };
			SetAllList(data);
			return data;
		}
		#endregion

		#region 計費方式(A:尺寸、B:材積、C:重量、D:均一價)F000904
		private List<NameValuePair<string>> _accKindList;

		public List<NameValuePair<string>> AccKindList
		{
			get { return _accKindList; }
			set
			{
				if (_accKindList == value) return;
				Set(() => AccKindList, ref _accKindList, value);
			}
		}

		public List<NameValuePair<string>> AccKindListForEdit
		{
			get { return AccKindList.Where(x => !string.IsNullOrEmpty(x.Value)).OrderBy(x => x.Value).ToList(); ; }
		}

		//選取的計費方式
		private string _selectedAccKind;

		public string SelectedAccKind
		{
			get { return _selectedAccKind; }
			set
			{
				if (_selectedAccKind == value) return;
				Set(() => SelectedAccKind, ref _selectedAccKind, value);
			}
		}

		//取得計費方式
		private List<NameValuePair<string>> GetAccKindList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F194707", "ACC_KIND");
			SetAllList(data);
			return data;
		}
		#endregion

		#region 狀態(0:使用中 9:刪除)
		private List<NameValuePair<string>> _statusList;

		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				if (_statusList == value) return;
				Set(() => StatusList, ref _statusList, value);
			}
		}

		//選取的狀態
		private string _selectedStatus;

		public string SelectedStatus
		{
			get { return _selectedStatus; }
			set
			{
				if (_selectedStatus == value) return;
				Set(() => SelectedStatus, ref _selectedStatus, value);
			}
		}

		//取得狀態
		private List<NameValuePair<string>> GetStatusList()
		{
			var data = new List<NameValuePair<string>>();
			data.Add(new NameValuePair<string>(Properties.Resources.P7105050000_ViewModel_Using, "0"));
			data.Add(new NameValuePair<string>(Properties.Resources.P7105070000_ViewModel_Cancel, "9"));
			return data;
		}
		#endregion

		#region 配送溫層(A:常溫、B:低溫)F000904
		private List<NameValuePair<string>> _delvTmprList;

		public List<NameValuePair<string>> DelvTmprList
		{
			get { return _delvTmprList; }
			set
			{
				if (_delvTmprList == value) return;
				Set(() => DelvTmprList, ref _delvTmprList, value);
			}
		}

		//取得配送溫層
		private List<NameValuePair<string>> GetDelvTmprList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F194707", "DELV_TMPR");
			return data;
		}
		#endregion

		#region 配送效率 F190102=>(01:一般、02:3小時、03:6小時、04:9小時)F000904
		private List<NameValuePair<string>> _delvEfficList;

		public List<NameValuePair<string>> DelvEfficList
		{
			get { return _delvEfficList; }
			set
			{
				if (_delvEfficList == value) return;
				Set(() => DelvEfficList, ref _delvEfficList, value);
			}
		}

		//取得配送效率
		private List<NameValuePair<string>> GetDelvEfficList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F190102", "DELV_EFFIC");
			return data;
		}
		#endregion

		#region 計費類型(A:平日、B：假日)F000904
		private List<NameValuePair<string>> _accTypeList;

		public List<NameValuePair<string>> AccTypeList
		{
			get { return _accTypeList; }
			set
			{
				if (_accTypeList == value) return;
				Set(() => AccTypeList, ref _accTypeList, value);
			}
		}

		//取得計費類型
		private List<NameValuePair<string>> GetAccTypeList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F194707", "ACC_TYPE");
			return data;
		}
		#endregion

		#region 配送區域(F194708)
		private List<NameValuePair<string>> _accAreaListForEdit;

		public List<NameValuePair<string>> AccAreaListForEdit
		{
			get { return _accAreaListForEdit; }
			set
			{
				if (_accAreaListForEdit == value) return;
				Set(() => AccAreaListForEdit, ref _accAreaListForEdit, value);
			}
		}

		private List<NameValuePair<string>> _accAreaListForSelected;

		public List<NameValuePair<string>> AccAreaListForSelected
		{
			get { return _accAreaListForSelected; }
			set
			{
				if (_accAreaListForSelected == value) return;
				Set(() => AccAreaListForSelected, ref _accAreaListForSelected, value);
			}
		}

		private List<NameValuePair<string>> GetAccAreaList(string dcCode, string allId)
		{
			if (string.IsNullOrWhiteSpace(dcCode) || string.IsNullOrWhiteSpace(allId))
				return new List<NameValuePair<string>>();

			var proxy = GetProxy<F19Entities>();
			var results = (from a in proxy.F194708s
						   where a.DC_CODE == dcCode && a.ALL_ID == allId
						   select new NameValuePair<string>()
						   {
							   Value = a.ACC_AREA_ID.ToString(),
							   Name = a.ACC_AREA
						   }).ToList();
			return results;
		}
		#endregion

		#region 單量級距(F194709)
		private List<NameValuePair<string>> _accDelvNumListForEdit;

		public List<NameValuePair<string>> AccDelvNumListForEdit
		{
			get { return _accDelvNumListForEdit; }
			set
			{
				if (_accDelvNumListForEdit == value) return;
				Set(() => AccDelvNumListForEdit, ref _accDelvNumListForEdit, value);
			}
		}

		private List<NameValuePair<string>> _accDelvNumListForSelected;

		public List<NameValuePair<string>> AccDelvNumListForSelected
		{
			get { return _accDelvNumListForSelected; }
			set
			{
				if (_accDelvNumListForSelected == value) return;
				Set(() => AccDelvNumListForSelected, ref _accDelvNumListForSelected, value);
			}
		}

		private List<NameValuePair<string>> GetAccDelvNumList(string dcCode, string allId)
		{
			if (string.IsNullOrWhiteSpace(dcCode) || string.IsNullOrWhiteSpace(allId))
				return new List<NameValuePair<string>>();

			var proxy = GetProxy<F19Entities>();
			var results = (from a in proxy.F194709s
						   where a.DC_CODE == dcCode && a.ALL_ID == allId
						   select new NameValuePair<string>()
						   {
							   Value = a.ACC_DELVNUM_ID.ToString(),
							   Name = a.NUM.ToString()
						   }).ToList();
			return results;
		}
		#endregion

		#endregion

		private List<F194707Ex> _records;

		public List<F194707Ex> Records
		{
			get { return _records; }
			set
			{
				if (_records == value)
					return;
				Set(() => Records, ref _records, value);
			}
		}

		private F194707Ex _selectedData;

		public F194707Ex SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				if (_selectedData != null)
				{
					AllIdListForSelected = GetAllIdList(_selectedData.DC_CODE);
					AccAreaListForSelected = GetAccAreaList(_selectedData.DC_CODE, _selectedData.ALL_ID);
					AccDelvNumListForSelected = GetAccDelvNumList(_selectedData.DC_CODE, _selectedData.ALL_ID);
				}
				RaisePropertyChanged("SelectedData");
			}
		}

		private F194707 _tempData;

		public F194707 TempData
		{
			get { return _tempData; }
			set
			{
				if (_tempData == value)
					return;
				Set(() => TempData, ref _tempData, value);
			}
		}

		private F194707 _editData;

		public F194707 EditData
		{
			get { return _editData; }
			set
			{
				if (_editData == value)
					return;
				Set(() => EditData, ref _editData, value);
			}
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoSearch(),
				  () => UserOperateMode == OperateMode.Query,
				  o => DoSearchComplete(),
				  null,
				  () => { SelectedData = null; }
				  );
			}
		}

		private void DoSearchComplete()
		{
			if (UserOperateMode == OperateMode.Query && Records != null && !Records.Any())
				ShowMessage(Messages.InfoNoData);
		}

		private void DoSearch()
		{
			//SelectedData = null;
			var proxyEx = GetExProxy<P71ExDataSource>();
			Records = proxyEx.CreateQuery<F194707Ex>("GetP710507SearchData")
						 .AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
						 .AddQueryOption("allId", string.Format("'{0}'", SelectedAllId))
						 .AddQueryOption("accKind", string.Format("'{0}'", SelectedAccKind))
						 .AddQueryOption("inTax", string.Format("'{0}'", SelectedTaxType))
						 .AddQueryOption("logiType", string.Format("'{0}'", SelectedLogiType))
						 .AddQueryOption("custType", string.Format("'{0}'", SelectedCustType))
						 .AddQueryOption("status", string.Format("'{0}'", SelectedStatus))
						 .ToList();

			if (!Records.Any())
			{
				ShowQueryDetail = false;
				return;
			}

			if (UserOperateMode == OperateMode.Query)
				SelectedData = Records.First();
			else
			{
				SelectedData = Records.Where(x => x.ALL_ID == EditData.ALL_ID
												  && x.DC_CODE == EditData.DC_CODE
												  && x.ACC_AREA_ID == EditData.ACC_AREA_ID
												  && x.DELV_EFFIC == EditData.DELV_EFFIC
												  && x.DELV_TMPR == EditData.DELV_TMPR
												  && x.CUST_TYPE == EditData.CUST_TYPE
												  && x.LOGI_TYPE == EditData.LOGI_TYPE
												  && x.ACC_KIND == EditData.ACC_KIND
												  && x.ACC_DELVNUM_ID == EditData.ACC_DELVNUM_ID
												  && x.ACC_TYPE == EditData.ACC_TYPE
												  && x.BASIC_VALUE == EditData.BASIC_VALUE).SingleOrDefault();
			}

			ShowQueryDetail = SelectedData != null;
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
			AllIdListForEdit = null;
			AccAreaListForEdit = null;
			AccDelvNumListForEdit = null;

			EditData = new F194707
			{
				STATUS = "0"
			};

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
				  () => UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData.STATUS == "0"
				  );
			}
		}

		private void DoEdit()
		{
			AllIdListForEdit = null;
			AccAreaListForEdit = null;
			AccDelvNumListForEdit = null;

			TempData = ExDataMapper.Map<F194707Ex, F194707>(SelectedData);
			EditData = ExDataMapper.Map<F194707Ex, F194707>(SelectedData);
			UserOperateMode = OperateMode.Edit;
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
			//執行取消動作
			TempData = null;
			EditData = null;
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
				  () => UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData.STATUS == "0",
				  o => DoDeleteComplete()
				  );
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			IsSaved = false;

			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
				return;

			var proxy = GetProxy<F19Entities>();
			var f194707 = proxy.F194707s.Where(x => x.ALL_ID == SelectedData.ALL_ID
													&& x.DC_CODE == SelectedData.DC_CODE
													&& x.ACC_AREA_ID == SelectedData.ACC_AREA_ID
													&& x.DELV_EFFIC == SelectedData.DELV_EFFIC
													&& x.DELV_TMPR == SelectedData.DELV_TMPR
													&& x.CUST_TYPE == SelectedData.CUST_TYPE
													&& x.LOGI_TYPE == SelectedData.LOGI_TYPE
													&& x.ACC_KIND == SelectedData.ACC_KIND
													&& x.ACC_DELVNUM_ID == SelectedData.ACC_DELVNUM_ID
													&& x.ACC_TYPE == SelectedData.ACC_TYPE
													&& x.BASIC_VALUE == SelectedData.BASIC_VALUE).SingleOrDefault();
			if (f194707 == null)
			{
				DialogService.ShowMessage(Properties.Resources.P7105070000_ViewModel_WorkAccItemNotFound);
				return;
			}

			if (f194707.STATUS != "0")
			{
				DialogService.ShowMessage(Properties.Resources.P7105070000_ViewModel_WorkAccItemStatus_CannotDelete);
				return;
			}

			f194707.STATUS = "9";
			proxy.UpdateObject(f194707);
			proxy.SaveChanges();

			ShowMessage(Messages.DeleteSuccess);
			IsSaved = true;
		}

		private void DoDeleteComplete()
		{
			if (!IsSaved) return;
			DoSearch();
		}

		#endregion Delete

		#region Save
		private bool IsSaved { get; set; }

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
			IsSaved = false;

			//執行確認儲存動作
			if (ShowMessage(Messages.WarningBeforeSave) != DialogResponse.Yes)
				return;

			if (!IsChecked())
				return;

			ExDataMapper.Trim(EditData);

			var proxy = GetProxy<F19Entities>();
			var f194707 = proxy.F194707s.Where(x => x.ALL_ID == EditData.ALL_ID
													&& x.DC_CODE == EditData.DC_CODE
													&& x.ACC_AREA_ID == EditData.ACC_AREA_ID
													&& x.DELV_EFFIC == EditData.DELV_EFFIC
													&& x.DELV_TMPR == EditData.DELV_TMPR
													&& x.CUST_TYPE == EditData.CUST_TYPE
													&& x.LOGI_TYPE == EditData.LOGI_TYPE
													&& x.ACC_KIND == EditData.ACC_KIND
													&& x.ACC_DELVNUM_ID == EditData.ACC_DELVNUM_ID
													&& x.ACC_TYPE == EditData.ACC_TYPE
													&& x.BASIC_VALUE == EditData.BASIC_VALUE).SingleOrDefault();

			if (UserOperateMode == OperateMode.Add)
			{
				if (f194707 != null)
				{
					ShowMessage(Messages.WarningExist);
					return;
				}

				proxy.AddToF194707s(EditData);
			}
			else
			{
				if (f194707 == null)
				{
					ShowMessage(Messages.WarningBeenDeleted);
					return;
				}
				f194707.IN_TAX = EditData.IN_TAX;
				f194707.MAX_WEIGHT = EditData.MAX_WEIGHT;
				f194707.FEE = EditData.FEE;
				f194707.OVER_VALUE = EditData.OVER_VALUE;
				f194707.OVER_UNIT_FEE = EditData.OVER_UNIT_FEE;
				proxy.UpdateObject(f194707);
			}
			proxy.SaveChanges();

			ShowMessage(Messages.Success);
			IsSaved = true;
		}

		private void DoSaveComplete()
		{
			if (!IsSaved) return;

			//if (UserOperateMode == OperateMode.Add)
			//{
			SelectedDc = EditData.DC_CODE;
			SelectedAllId = EditData.ALL_ID;
			SelectedAccKind = EditData.ACC_KIND;
			SelectedTaxType = EditData.IN_TAX;
			SelectedLogiType = EditData.LOGI_TYPE;
			SelectedCustType = EditData.CUST_TYPE;
			SelectedStatus = EditData.STATUS;
			//}

			DoSearch();
			TempData = null;
			EditData = null;
			UserOperateMode = OperateMode.Query;
		}

		private bool IsChecked()
		{
			if (EditData.ACC_KIND == "E")
				EditData.MAX_WEIGHT = 0;
			if (EditData.ACC_KIND == "E")
				EditData.BASIC_VALUE = 0;

			if (EditData.OVER_VALUE != null && EditData.OVER_UNIT_FEE == null)
			{
				var msg = "";
				switch (EditData.ACC_TYPE)
				{
					case "A":
						msg = Properties.Resources.OVER_Size;
						break;
					case "B":
						msg = Properties.Resources.OVER_UNIT;
						break;
					case "C":
						msg = Properties.Resources.Over_Weight;
						break;
				}
				DialogService.ShowMessage(msg + Properties.Resources.P7105070000_ViewModel_FEE_Required);
				return false;
			}

			return true;
		}
		#endregion Save
	}
}
