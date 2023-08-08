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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices.P50ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P50WcfService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P50.ViewModel
{
	public partial class P5001010000_ViewModel : InputViewModelBase
	{
		public Action<F199007Data> SetCtrlView = delegate { };
		public P5001010000_ViewModel()
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
			}
		}
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
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

		#region From - 查詢建立日期(起)
		private DateTime _crtDateStart = DateTime.Now.Date;

		public DateTime CrtDateStart
		{
			get { return _crtDateStart; }
			set
			{
				if (_crtDateStart == value)
					return;
				Set(() => CrtDateStart, ref _crtDateStart, value);
			}
		}
		#endregion
		#region From - 查詢建立日期(迄)
		private DateTime _crtDateEnd = DateTime.Now.Date;

		public DateTime CrtDateEnd
		{
			get { return _crtDateEnd; }
			set
			{
				if (_crtDateEnd == value)
					return;
				Set(() => CrtDateEnd, ref _crtDateEnd, value);
			}
		}
		#endregion
		#region From - 查詢專案編號
		private string _accProjectNo;

		public string AccProjectNo
		{
			get { return _accProjectNo; }
			set
			{
				if (_accProjectNo == value)
					return;
				Set(() => AccProjectNo, ref _accProjectNo, value);
			}
		}
		#endregion
		#region From - 專案日期生效日
		private DateTime? _enableDate = null;

		public DateTime? EnableDate
		{
			get { return _enableDate; }
			set
			{
				if (_enableDate == value)
					return;
				Set(() => EnableDate, ref _enableDate, value);
			}
		}
		#endregion
		#region From - 專案日期失效日
		private DateTime? _disableDate = null;

		public DateTime? DisableDate
		{
			get { return _disableDate; }
			set
			{
				if (_disableDate == value)
					return;
				Set(() => DisableDate, ref _disableDate, value);
			}
		}
		#endregion
		#region  From - 報價單號
		private string _quoteNo;

		public string QuoteNo
		{
			get { return _quoteNo; }
			set
			{
				if (_quoteNo == value)
					return;
				Set(() => QuoteNo, ref _quoteNo, value);
			}
		}
		#endregion
		#region From - 專案名稱
		private string _accProjectName;

		public string AccProjectName
		{
			get { return _accProjectName; }
			set
			{
				if (_accProjectName == value)
					return;
				Set(() => AccProjectName, ref _accProjectName, value);
			}
		}
		#endregion

		#region 選擇的專案計價項目
		private F199007Data _selectedData;

		public F199007Data SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData == value)
					return;
				if (value != null)
					CurrentRecord = ExDataMapper.Map<F199007Data, F199007>(value);
				else
					CurrentRecord = null;
				Set(() => SelectedData, ref _selectedData, value);
			}
		}
		#endregion

		#region 專案計價項目清單
		private List<F199007Data> _records;

		public List<F199007Data> Records
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

		#region 專案計價項目設定
		private F199007 _currentRecord;

		public F199007 CurrentRecord
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
			var dcList = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList.OrderBy(x => x.Value));
			dcList.Insert(0, new NameValuePair<string>(Properties.Resources.NoSpecify, "000"));
			DcList = dcList;
			SetAllList(DcList);
			if (DcList != null && DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
			//計費方式
			AccKindList = GetAccKindListList();
			if (AccKindList != null && AccKindList.Any()) SelectedAccKind = AccKindList.FirstOrDefault().Value;
			TaxList = GetTaxList();
			//計價狀態
			StatusList = GetStatusList();
			if (StatusList != null && StatusList.Any()) SelectedStatus = StatusList.FirstOrDefault().Value;
		}

		private List<NameValuePair<string>> GetStatusList()
		{
		
			var results = GetBaseTableService.GetF000904List(FunctionCode, "F199007", "STATUS").ToList();
			if (results != null)
				SetAllList(results);
			return results;
		}

		private List<NameValuePair<string>> GetTaxList()
		{			
			var results = GetBaseTableService.GetF000904List(FunctionCode, "F199007", "IN_TAX").ToList();
			
			return results;
		}

		private List<NameValuePair<string>> GetAccKindListList()
		{
		
			var results = GetBaseTableService.GetF000904List(FunctionCode, "F199007", "ACC_KIND").ToList();
			
			return results;
		}

		private static List<NameValuePair<string>> SetAllList(List<NameValuePair<string>> tmpList)
		{
			tmpList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });
			return tmpList;
		}

		private bool ValidateSearchDate(out string errorMsg)
		{
			if (!ValidateHelper.TryCheckBeginEnd(this, x => x.CrtDateStart, x => x.CrtDateEnd, Resources.Resources.CRT_DATE, out errorMsg))
				return false;

			if (!ValidateHelper.TryCheckBeginEnd(this, x => x.EnableDate, x => x.DisableDate, Properties.Resources.EnableDate, out errorMsg))
				return false;

			return true;
		}

		private bool ValidateAddOrEditDate(out string errorMsg)
		{

			if (CurrentRecord == null)
			{
				errorMsg = string.Empty;
				return false;
			}
			if (!ValidateHelper.TryCheckBeginEnd(CurrentRecord, x => x.ENABLE_DATE, x => x.DISABLE_DATE, Properties.Resources.EnableDate, out errorMsg))
				return false;

			return true;
		}
		#endregion

		#region Command

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				F199007 tmpData = null;
				OperateMode mode = OperateMode.Query;
				return CreateBusyAsyncCommand(
					o =>
					{
						if (o != null)
						{
							mode = (OperateMode)((Dictionary<string, object>)o)["Mode"];
							tmpData = ((Dictionary<string, object>)o)["Record"] as F199007;
						}
						DoSearch(mode, tmpData);
					}, () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete(tmpData)
					);
			}
		}

		private void DoSearchComplete(F199007 data)
		{
			if (Records == null || !Records.Any()) return;
			if (data == null)
				SelectedData = Records.FirstOrDefault();
			else
				SelectedData = Records.Where(x => x.DC_CODE == data.DC_CODE &&
																					x.GUP_CODE == data.GUP_CODE &&
																					x.CUST_CODE == data.CUST_CODE &&
																					x.ACC_PROJECT_NO == data.ACC_PROJECT_NO)
															.SingleOrDefault();
			if (SelectedData != null)
				SetCtrlView(SelectedData);
		}

		private void DoSearch(OperateMode userMode, F199007 data)
		{
			//執行查詢動
			var errMsg = string.Empty;
			if (!ValidateSearchDate(out errMsg))
			{
				ShowWarningMessage(errMsg);
				return;
			}
			Records = GetF199007Datas(userMode, data);
			if (Records == null || !Records.Any())
				ShowMessage(Messages.InfoNoData);
		}

		private List<F199007Data> GetF199007Datas(OperateMode mode, F199007 keyData)
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			List<F199007Data> results = new List<F199007Data>();
			if (mode == OperateMode.Add && keyData != null)
			{
				results = proxyEx.CreateQuery<F199007Data>("GetProjectValuation")
												 .AddQueryExOption("dcCode", keyData.DC_CODE)
												 .AddQueryExOption("gupCode", keyData.GUP_CODE)
												 .AddQueryExOption("custCode", keyData.CUST_CODE)
												 .AddQueryExOption("crtDateStare", keyData.CRT_DATE.ToShortDateString())
												 .AddQueryExOption("crtDateEnd", keyData.CRT_DATE.ToShortDateString())
												 .AddQueryExOption("accProjectNo", keyData.ACC_PROJECT_NO)
												 .AddQueryExOption("enableDate", keyData.ENABLE_DATE.ToShortDateString())
												 .AddQueryExOption("disableDate", keyData.DISABLE_DATE.ToShortDateString())
												 .AddQueryExOption("quoteNo", keyData.QUOTE_NO)
												 .AddQueryExOption("status", keyData.STATUS)
												 .AddQueryExOption("accProjectName", keyData.ACC_PROJECT_NAME)
												 .ToList();
			}
			else
			{
				var enableDateString = (EnableDate != null) ? EnableDate.Value.ToShortDateString() : string.Empty;
				var disableDateString = (DisableDate != null) ? DisableDate.Value.ToShortDateString() : string.Empty;
				results = proxyEx.CreateQuery<F199007Data>("GetProjectValuation")
												 .AddQueryExOption("dcCode", SelectedDc)
												 .AddQueryExOption("gupCode", _gupCode)
												 .AddQueryExOption("custCode", _custCode)
												 .AddQueryExOption("crtDateStare", CrtDateStart.ToShortDateString())
												 .AddQueryExOption("crtDateEnd", CrtDateEnd.ToShortDateString())
												 .AddQueryExOption("accProjectNo", AccProjectNo)
												 .AddQueryExOption("enableDate", enableDateString)
												 .AddQueryExOption("disableDate", disableDateString)
												 .AddQueryExOption("quoteNo", QuoteNo)
												 .AddQueryExOption("status", SelectedStatus)
												 .AddQueryExOption("accProjectName", AccProjectName)
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
			CurrentRecord = new F199007()
			{
				GUP_CODE = _gupCode,
				CUST_CODE = _custCode,
				STATUS = "0",
				ENABLE_DATE = DateTime.Today.AddDays(1),
				DISABLE_DATE = DateTime.Today.AddDays(1)
			};
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData != null &&
						Records != null && Records.Any() && SelectedData.STATUS != "9" && SelectedData.STATUS == "0"
						&& SelectedData.ENABLE_DATE > DateTime.Today
					);
			}
		}


		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
			if (SelectedData == null)
			{
				ShowWarningMessage(Properties.Resources.SelectData);
				UserOperateMode = OperateMode.Query;
				return;
			}
			CurrentRecord = ExDataMapper.Map<F199007Data, F199007>(SelectedData);
			if (CurrentRecord.ENABLE_DATE != null && CurrentRecord.ENABLE_DATE.Date <= DateTime.Now.Date)
			{
				ShowWarningMessage(Properties.Resources.PRO_ENABLE_CannotEdit);
				UserOperateMode = OperateMode.Query;
				return;
			}
			if (CurrentRecord.STATUS != "0")
			{
				ShowWarningMessage(Properties.Resources.Status_Invalid_Edit);
				UserOperateMode = OperateMode.Query;
				return;
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
			//執行取消動作
			if (SelectedData != null)
				CurrentRecord = ExDataMapper.Map<F199007Data, F199007>(SelectedData);
			else
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
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData.STATUS != "9" && SelectedData.STATUS == "0",
					o => DoDeleteCompleted()
					);
			}
		}

		private void DoDeleteCompleted()
		{
			SearchCommand.Execute(null);
		}

		private void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
				return;

			if (SelectedData == null)
			{
				ShowWarningMessage(Properties.Resources.SelectData);
				return;
			}
			if (SelectedData.ENABLE_DATE.Date <= DateTime.Now.Date)
			{
				ShowWarningMessage(Properties.Resources.PRO_ENABLE_CannotDelete);
				return;
			}
			if (SelectedData.STATUS != "0")
			{
				ShowWarningMessage(Properties.Resources.Status_Invalid_Delete);
				return;
			}

			var proxy = GetExProxy<P50ExDataSource>();
			var query = proxy.CreateQuery<ExecuteResult>("DeleteP5001010000")
											 .AddQueryExOption("dcCode", SelectedData.DC_CODE)
				 .AddQueryExOption("gupCode", SelectedData.GUP_CODE)
				 .AddQueryExOption("custCode", SelectedData.CUST_CODE)
				 .AddQueryExOption("accProjectNo", SelectedData.ACC_PROJECT_NO);

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
			{
				ShowMessage(Messages.Failed);
				return;
			}
			ShowMessage(Messages.Success);
			var tmpRecord = ExDataMapper.Map<F199007, F199007>(CurrentRecord);
			CurrentRecord = null;
			Dictionary<string, object> dicParam = new Dictionary<string, object>();
			dicParam.Add("Mode", UserOperateMode);
			dicParam.Add("Record", tmpRecord);
			SearchCommand.Execute(dicParam);
			UserOperateMode = OperateMode.Query;
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			if (ShowMessage(Messages.WarningBeforeSave) != DialogResponse.Yes) return false;
			// 檢查資料
			if (!isValid()) return false;
			var proxy = GetProxy<F19Entities>();


			var f199007 = proxy.F199007s.Where(x => x.DC_CODE == CurrentRecord.DC_CODE &&
																							x.GUP_CODE == CurrentRecord.GUP_CODE &&
																							x.CUST_CODE == CurrentRecord.CUST_CODE &&
																							x.ACC_PROJECT_NO == CurrentRecord.ACC_PROJECT_NO)
																	.SingleOrDefault();
			if (UserOperateMode == OperateMode.Add)
			{
				if (f199007 != null)
				{
					ShowMessage(Messages.WarningExist);
					return false;
				}

				//取流水號
				var wcfProxy = new wcf.P50WcfServiceClient();
				var projectNo = RunWcfMethod<string>(wcfProxy.InnerChannel,
						() => wcfProxy.GetProjectNo("ZD"));

				CurrentRecord.ACC_PROJECT_NO = projectNo;

				ExDataMapper.Trim(CurrentRecord);
				proxy.AddToF199007s(CurrentRecord);
			}
			else
			{
				if (f199007 == null)
				{
					ShowMessage(Messages.WarningBeenDeleted);
					return false;
				}
				f199007.QUOTE_NO = CurrentRecord.QUOTE_NO;
				f199007.ENABLE_DATE = CurrentRecord.ENABLE_DATE.Date;
				f199007.DISABLE_DATE = CurrentRecord.DISABLE_DATE.Date;
				f199007.ACC_KIND = CurrentRecord.ACC_KIND;
				f199007.ACC_PROJECT_NAME = CurrentRecord.ACC_PROJECT_NAME;
				f199007.FEE = CurrentRecord.FEE;
				f199007.IN_TAX = CurrentRecord.IN_TAX;
				f199007.SERVICES = CurrentRecord.SERVICES;

				ExDataMapper.Trim(f199007);
				proxy.UpdateObject(f199007);
			}
			proxy.SaveChanges();
			return true;
		}

		private bool isValid()
		{
			if (CurrentRecord == null)
			{
				ShowWarningMessage(Properties.Resources.NoCurrentRecord_InsertOrEdit);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.DC_CODE))
			{
				ShowWarningMessage(Properties.Resources.SelectDC);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.QUOTE_NO))
			{
				ShowWarningMessage(Properties.Resources.Write_Quote_No);
				return false;
			}
			if (CurrentRecord.ENABLE_DATE == null)
			{
				ShowWarningMessage(Properties.Resources.Select_Enable_Date);
				return false;
			}
			if (CurrentRecord.ENABLE_DATE.Date <= DateTime.Now.Date)
			{
				ShowWarningMessage(Properties.Resources.Enable_Date_GreaterThanToday);
				return false;
			}
			if (CurrentRecord.DISABLE_DATE == null)
			{
				ShowWarningMessage(Properties.Resources.Select_Disable_Date);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.ACC_PROJECT_NAME))
			{
				ShowWarningMessage(Properties.Resources.Write_ACC_PROJECT_NAME);
				return false;
			}
			if (CurrentRecord.FEE < 0)
			{
				ShowWarningMessage(Properties.Resources.FEE_GreaterThanZero_Required);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.ACC_KIND))
			{
				ShowWarningMessage(Properties.Resources.SelectACC_KIND);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.IN_TAX))
			{
				ShowWarningMessage(Properties.Resources.SelectIN_TAX);
				return false;
			}
			if (string.IsNullOrEmpty(CurrentRecord.SERVICES))
			{
				ShowWarningMessage(Properties.Resources.Write_SERVICES);
				return false;
			}
			if (CurrentRecord.STATUS != "0")
			{
				ShowWarningMessage(Properties.Resources.Status_Invalid_Edit);
				return false;
			}
			var errMsg = string.Empty;
			if (!ValidateAddOrEditDate(out errMsg))
			{
				ShowWarningMessage(errMsg);
				return false;
			}

			return true;
		}
		#endregion Save

		#endregion
	}
}
