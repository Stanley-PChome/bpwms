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
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7105030000_ViewModel : InputViewModelBase
	{
		public Action<F199003Data> SetCtrlView = delegate { };
		public P7105030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}

		}
		#region Property,Field
		private string _gupCode;
		private string _custCode;
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				_selectedDc = value;
				RaisePropertyChanged("SelectedDc");
			}
		}
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.OrderBy(x => x.Value).ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}

		/// <summary>
		/// 物流中心清單-新增
		/// </summary>
		public List<NameValuePair<string>> DcListAdd
		{
			get { return _dcList.Where(x => !string.IsNullOrEmpty(x.Value)).OrderBy(x => x.Value).ToList(); }
		}
		#endregion

		#region Form - 選擇的計價項目
		private string _selectedAccItemKind;

		public string SelectedAccItemKind
		{
			get { return _selectedAccItemKind; }
			set
			{
				if (_selectedAccItemKind == value)
					return;
				Set(() => SelectedAccItemKind, ref _selectedAccItemKind, value);
			}
		}
		#endregion
		#region Form - 可用的計價項目清單
		private List<NameValuePair<string>> _accItemKindList;

		public List<NameValuePair<string>> AccItemKindList
		{
			get { return _accItemKindList; }
			set
			{
				if (_accItemKindList == value)
					return;
				Set(() => AccItemKindList, ref _accItemKindList, value);
			}
		}
		#endregion
		#region Form - 可用的計價項目清單-新增/編輯

		public List<NameValuePair<string>> AccItemKindAddList
		{
			get { return _accItemKindList.Where(x => !string.IsNullOrEmpty(x.Value)).OrderBy(x => x.Value).ToList(); ; }
		}
		#endregion

		#region Form - 可用的配送計價類別清單-新增/編輯
		private List<NameValuePair<string>> _delvAccTypeAddList;
		public List<NameValuePair<string>> DelvAccTypeAddList
		{
			get { return _delvAccTypeAddList; }
			set
			{
				if (_delvAccTypeAddList == value)
					return;
				Set(() => DelvAccTypeAddList, ref _delvAccTypeAddList, value);
			}
		}
		#endregion

		#region Form - 選擇的計費方式
		private string _selectedAccKind;

		public string SelectedAccKind
		{
			get { return _selectedAccKind; }
			set
			{
				if (_selectedAccKind == value)
					return;
				Set(() => SelectedAccKind, ref _selectedAccKind, value);
			}
		}
		#endregion
		#region Form - 可用的計費方式清單
		private List<NameValuePair<string>> _accKindList;

		public List<NameValuePair<string>> AccKindList
		{
			get { return _accKindList; }
			set
			{
				if (_accKindList == value)
					return;
				Set(() => AccKindList, ref _accKindList, value);
			}
		}
		#endregion

		#region Form - 可用的計價單位清單
		private List<NameValuePair<string>> _accUnitList;

		public List<NameValuePair<string>> AccUnitList
		{
			get { return _accUnitList; }
			set
			{
				if (_accUnitList == value)
					return;
				Set(() => AccUnitList, ref _accUnitList, value);
			}
		}
		#endregion

		#region Form - 稅別清單
		private List<NameValuePair<string>> _taxList;

		public List<NameValuePair<string>> TaxList
		{
			get { return _taxList; }
			set
			{
				if (_taxList == value)
					return;
				Set(() => TaxList, ref _taxList, value);
			}
		}
		#endregion


		#region Form - 選擇的計價狀態
		private string _selectedStatus;

		public string SelectedStatus
		{
			get { return _selectedStatus; }
			set
			{
				if (_selectedStatus == value)
					return;
				Set(() => SelectedStatus, ref _selectedStatus, value);
			}
		}
		#endregion
		#region Form - 計價狀態
		private List<NameValuePair<string>> _statusList;

		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				if (_statusList == value)
					return;
				Set(() => StatusList, ref _statusList, value);
			}
		}
		#endregion

		#region 選擇的出貨計價項目
		private F199003Data _selectedData;

		public F199003Data SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData == value)
					return;
				if (value != null)
					DelvAccTypeAddList = GetDelvAccTypeList(value.ACC_ITEM_KIND_ID);
				Set(() => SelectedData, ref _selectedData, value);
			}
		}
		#endregion
		#region 出貨計價項目清單
		private List<F199003Data> _records;

		public List<F199003Data> Records
		{
			get { return _records; }
			set
			{
				if (_records == value)
					return;
				Set(() => Records, ref _records, value);
			}
		}
		#endregion

		#region 出貨計價項目設定
		private F199003 _currentRecord;

		public F199003 CurrentRecord
		{
			get { return _currentRecord; }
			set
			{
				if (_currentRecord == value)
					return;
				Set(() => CurrentRecord, ref _currentRecord, value);
			}
		}
		#endregion

		#endregion

		#region Function
		private void InitControls()
		{
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			var dcList = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
			dcList.Insert(0, new NameValuePair<string>(Properties.Resources.P7105010000_ViewModel_NONE_SPECIFY, "000"));
			DcList = dcList;
			SetAllList(DcList);
			if (DcList != null && DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
			//計價項目
			AccItemKindList = GetAccItemKindList();
			if (AccItemKindList != null && AccItemKindList.Any()) SelectedAccItemKind = AccItemKindList.FirstOrDefault().Value;
			//計費方式
			AccKindList = GetAccKindListList();
			if (AccKindList != null && AccKindList.Any()) SelectedAccKind = AccKindList.FirstOrDefault().Value;
			//計價單位
			AccUnitList = GetAccUnitList();
			//稅別清單
			TaxList = GetTaxList();
			//計價狀態
			StatusList = GetStatusList();
			if (StatusList != null && StatusList.Any()) SelectedStatus = StatusList.FirstOrDefault().Value;
		}

		private List<NameValuePair<string>> GetStatusList()
		{
		
			var results = GetBaseTableService.GetF000904List(FunctionCode, "F199003", "STATUS").ToList();
		
			if (results != null)
				SetAllList(results);
			return results;
		}

		private List<NameValuePair<string>> GetTaxList()
		{
	
			var results = GetBaseTableService.GetF000904List(FunctionCode, "F199003", "IN_TAX").ToList();
		
			return results;
		}

		private List<NameValuePair<string>> GetAccUnitList()
		{
			var proxy = GetProxy<F91Entities>();
			var results = proxy.F91000302s.Where(x => x.ITEM_TYPE_ID == "004")
				.OrderBy(x => x.ACC_UNIT)
				.Select(x => new NameValuePair<string> { Name = x.ACC_UNIT_NAME, Value = x.ACC_UNIT })
				.ToList();
			return results;
		}

		private List<NameValuePair<string>> GetAccKindListList()
		{
		
			var results = GetBaseTableService.GetF000904List(FunctionCode, "F199003", "ACC_KIND").ToList();
		
			if (results != null)
				SetAllList(results);
			return results;
		}


		public List<NameValuePair<string>> GetDelvAccTypeList(string accItemKind)
		{
			if (string.IsNullOrEmpty(accItemKind)) return null;
			var proxy = GetExProxy<P71ExDataSource>();
			var results = proxy.CreateQuery<F000904DelvAccType>("GetDelvAccTypes")
												 .AddQueryExOption("itemTypeId", "003")
												 .AddQueryExOption("accItemKindId", accItemKind)
												 .ToList()
												 .Select(x => new NameValuePair<string>()
												 {
													 Name = x.DELV_ACC_TYPE_NAME,
													 Value = x.DELV_ACC_TYPE
												 }
												 ).ToList();
			return results;
		}

		private List<NameValuePair<string>> GetAccItemKindList()
		{
			var proxy = GetExProxy<P71ExDataSource>();
			var results = proxy.CreateQuery<F91000301Data>("GetAccItemKinds")
												 .AddQueryExOption("itemTypeId", "004")
												 .ToList()
												 .Select(x => new NameValuePair<string>()
												 {
													 Name = x.ACC_ITEM_KIND_NAME,
													 Value = x.ACC_ITEM_KIND_ID
												 }
												 ).ToList();
			if (results != null)
				SetAllList(results);
			return results;
		}

		private static List<NameValuePair<string>> SetAllList(List<NameValuePair<string>> tmpList)
		{
			tmpList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });
			return tmpList;
		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				F199003 tmpData = null;
				OperateMode mode = OperateMode.Query;
				return CreateBusyAsyncCommand(
					o =>
					{
						if (o != null)
						{
							mode = (OperateMode)((Dictionary<string, object>)o)["Mode"];
							tmpData = ((Dictionary<string, object>)o)["Record"] as F199003;
						}
						DoSearch(mode, tmpData);
					}, () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete(tmpData)
					);
			}
		}

		private void DoSearchComplete(F199003 data)
		{
			if (Records == null || !Records.Any()) return;
			if (data == null)
				SelectedData = Records.FirstOrDefault();
			else
				SelectedData = Records.Where(x => x.DC_CODE == data.DC_CODE &&
																					x.ACC_ITEM_KIND_ID == data.ACC_ITEM_KIND_ID &&
																					x.ACC_KIND == data.ACC_KIND &&
																					x.ACC_UNIT == data.ACC_UNIT &&
																					x.ACC_NUM == data.ACC_NUM &&
																					x.DELV_ACC_TYPE == data.DELV_ACC_TYPE)
															.SingleOrDefault();
			if (SelectedData != null)
			{
				SelectedDc = SelectedData.DC_CODE;
				SetCtrlView(SelectedData);
			}
		}

		private void DoSearch(OperateMode userMode, F199003 data)
		{
			//執行查詢動
			Records = GetF199003Datas(userMode, data);
			if (Records == null || !Records.Any())
				ShowMessage(Messages.InfoNoData);
		}

		private List<F199003Data> GetF199003Datas(OperateMode mode, F199003 keyData)
		{
			var proxyEx = GetExProxy<P71ExDataSource>();
			List<F199003Data> results = new List<F199003Data>();
			if (mode == OperateMode.Add && keyData != null)
			{
				results = proxyEx.CreateQuery<F199003Data>("GetShippingValuation")
												 .AddQueryExOption("dcCode", keyData.DC_CODE)
												 .AddQueryExOption("accItemKindId", keyData.ACC_ITEM_KIND_ID)
												 .AddQueryExOption("accKind", keyData.ACC_KIND)
												 .AddQueryExOption("status", keyData.STATUS)
												 .ToList();
				SelectedDc = keyData.DC_CODE;
				SelectedAccItemKind = keyData.ACC_ITEM_KIND_ID;
				SelectedAccKind = keyData.ACC_KIND;
				SelectedStatus = keyData.STATUS;
			}
			else
			{
				results = proxyEx.CreateQuery<F199003Data>("GetShippingValuation")
												 .AddQueryExOption("dcCode", SelectedDc)
												 .AddQueryExOption("accItemKindId", SelectedAccItemKind)
												 .AddQueryExOption("accKind", SelectedAccKind)
												 .AddQueryExOption("status", SelectedStatus)
												 .ToList();
			}
			return results;
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
			UserOperateMode = OperateMode.Add;
			//執行新增動作
			CurrentRecord = new F199003()
			{
				ACC_KIND = "A",
				STATUS = "0",
				ACC_ITEM_KIND_ID = "01",
				ITEM_TYPE_ID = "004"
			};
			DelvAccTypeAddList = GetDelvAccTypeList(CurrentRecord.ACC_ITEM_KIND_ID);
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData != null &&
						Records != null && Records.Any() && SelectedData.STATUS != "9"
					);
			}
		}


		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
			if (SelectedData == null)
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_SelectData);
				return;
			}
			CurrentRecord = ExDataMapper.Map<F199003Data, F199003>(SelectedData);
			if (string.IsNullOrEmpty(CurrentRecord.ACC_KIND))
				CurrentRecord.ACC_KIND = "A";
			if (string.IsNullOrEmpty(CurrentRecord.STATUS))
				CurrentRecord.STATUS = "0";
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
			CurrentRecord = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData.STATUS != "9",
					o => DoDeleteComplete()
					);
			}
		}

		private void DoDeleteComplete()
		{
			SearchCommand.Execute(null);
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
				return;

			var proxy = GetExProxy<P71ExDataSource>();
			var query = proxy.CreateQuery<ExecuteResult>("DeleteP7105030000")
											 .AddQueryExOption("dcCode", SelectedData.DC_CODE)
											 .AddQueryExOption("accItemKindId", SelectedData.ACC_ITEM_KIND_ID)
											 .AddQueryExOption("accKind", SelectedData.ACC_KIND)
											 .AddQueryExOption("accUnit", SelectedData.ACC_UNIT)
											 .AddQueryExOption("delvAccType", SelectedData.DELV_ACC_TYPE);

			var result = query.ToList().FirstOrDefault();
			ShowResultMessage(result);
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(), () => UserOperateMode != OperateMode.Query && CurrentRecord != null,
					o => DoSaveComplete(isSuccess)
					);
			}
		}

		private void DoSaveComplete(bool isSuccess)
		{
			if (!isSuccess)
				return;
			ShowMessage(Messages.Success);
			var tmpRecord = ExDataMapper.Map<F199003, F199003>(CurrentRecord);
			Dictionary<string, object> dicParam = new Dictionary<string, object>();
			dicParam.Add("Mode", UserOperateMode);
			dicParam.Add("Record", tmpRecord);
			SearchCommand.Execute(dicParam);
			CurrentRecord = null;
			UserOperateMode = OperateMode.Query;
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			if (ShowMessage(Messages.WarningBeforeSave) != DialogResponse.Yes) return false;
			// 檢查資料
			if (!isValid()) return false;
			var proxy = GetProxy<F19Entities>();


			var f199003 = proxy.F199003s.Where(x => x.DC_CODE == CurrentRecord.DC_CODE &&
																							x.ACC_ITEM_KIND_ID == CurrentRecord.ACC_ITEM_KIND_ID &&
																							x.ACC_KIND == CurrentRecord.ACC_KIND &&
																							x.ACC_UNIT == CurrentRecord.ACC_UNIT &&
																							x.DELV_ACC_TYPE == CurrentRecord.DELV_ACC_TYPE && x.STATUS =="0")
																	.SingleOrDefault();
			if (UserOperateMode == OperateMode.Add)
			{
				if (f199003 != null)
				{
					ShowMessage(Messages.WarningExist);
					return false;
				}
				ExDataMapper.Trim(CurrentRecord);
				proxy.AddToF199003s(CurrentRecord);
			}
			else
			{
				if (f199003 == null)
				{
					ShowMessage(Messages.WarningBeenDeleted);
					return false;
				}
				f199003.ACC_NUM = CurrentRecord.ACC_NUM;
				f199003.ACC_ITEM_NAME = CurrentRecord.ACC_ITEM_NAME;
				f199003.IN_TAX = CurrentRecord.IN_TAX;
				if (f199003.ACC_KIND == "A")
					f199003.FEE = CurrentRecord.FEE;
				else
				{
					f199003.BASIC_FEE = CurrentRecord.BASIC_FEE;
					f199003.OVER_FEE = CurrentRecord.OVER_FEE;
				}
				ExDataMapper.Trim(f199003);
				proxy.UpdateObject(f199003);
			}
			proxy.SaveChanges();
			return true;
		}

		private bool isValid()
		{
			if (CurrentRecord == null)
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_NoData_Insert_Edit);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.DC_CODE))
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_SelectDC);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.ACC_ITEM_KIND_ID))
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_SelectACCItem);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.DELV_ACC_TYPE))
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_SelectDeliverPlanType);
				return false;
			}
			if (CurrentRecord.ACC_NUM < 1)
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_ACCNum_GreaterThanOne);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.ACC_UNIT))
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_SelectACCUnit);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.ACC_KIND))
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_SelectACCWay);
				return false;
			}
			if (CurrentRecord.ACC_KIND == "A" && CurrentRecord.FEE < 0)
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_SingleFee_GreaterThanZero);
				return false;
			}
			if (CurrentRecord.ACC_KIND == "B" &&
				(CurrentRecord.BASIC_FEE < 0 || CurrentRecord.OVER_FEE < 0))
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_ConditionFee_GreaterThanZero);
				return false;
			}
			if (UserOperateMode == OperateMode.Add)
			{
				var f199003s = GetF199003ByAccName(CurrentRecord.DC_CODE, CurrentRecord.ACC_ITEM_NAME);
				if (f199003s != null && f199003s.Any())
				{
					ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_ACCSetName_Duplicate);
					return false;
				}
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				var f199003 = GetF199003ByAccName(CurrentRecord.DC_CODE, CurrentRecord.ACC_ITEM_NAME).FirstOrDefault();
				if (f199003 != null && !(f199003.DC_CODE == CurrentRecord.DC_CODE && f199003.ACC_ITEM_KIND_ID == CurrentRecord.ACC_ITEM_KIND_ID  && f199003.ACC_KIND == CurrentRecord.ACC_KIND && f199003.ACC_UNIT == CurrentRecord.ACC_UNIT && f199003.DELV_ACC_TYPE == CurrentRecord.DELV_ACC_TYPE))
				{
					ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_ACCSetName_Duplicate);
					return false;
				}
			}
			return true;
		}
		private List<F199003> GetF199003ByAccName(string dcCode, string accItemName)
		{
			var proxy = GetProxy<F19Entities>();
			var results = proxy.F199003s.Where(x => x.DC_CODE == dcCode && x.ACC_ITEM_NAME == accItemName && x.STATUS =="0").ToList();
			return results;
		}
		#endregion Save
		#endregion
	}
}
