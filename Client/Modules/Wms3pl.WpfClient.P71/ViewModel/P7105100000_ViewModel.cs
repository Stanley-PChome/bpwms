using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F50DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P50ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7105100000_ViewModel : InputViewModelBase
	{
		public P7105100000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitData();
			}

		}

		private void InitData()
		{
			HasClosed = Visibility.Collapsed;
			GupList = GetGupData();
			SelectedGupCode = GupList.Select(x => x.Value).FirstOrDefault();
			OutSourceList = GetOutSourceData();
			SelectedOutSource = OutSourceList.Select(x => x.Value).FirstOrDefault();

			SelectClearingYearMonth = DateTime.Today;

			ReportTypeMainList = GetReportType("0");
			SelectedReportTypeMain = ReportTypeMainList.Select(x => x.Value).FirstOrDefault();
			ReportTypeDetailList = GetReportType("1");
			SelectedReportTypeDetail = ReportTypeDetailList.Select(x => x.Value).FirstOrDefault();
		}

		#region Property

		public Action<F710510ReportType> DoPreview = delegate { };

		#region 業主
		private List<NameValuePair<string>> _gupList;
		public List<NameValuePair<string>> GupList
		{
			get { return _gupList; }
			set
			{
				_gupList = value;
				RaisePropertyChanged("GupList");
			}
		}

		private string _selectedGupCode;
		public string SelectedGupCode
		{
			get { return _selectedGupCode; }
			set
			{
				_selectedGupCode = value;
				RaisePropertyChanged("SelectedGupCode");
				ReloadCustList();
			}
		}
		#endregion

		#region 貨主
		private List<NameValuePair<string>> _custList;
		public List<NameValuePair<string>> CustList
		{
			get { return _custList; }
			set
			{
				_custList = value;
				RaisePropertyChanged("CustList");
			}
		}

		private string _selectedCustCode;
		public string SelectedCustCode
		{
			get { return _selectedCustCode; }
			set
			{
				_selectedCustCode = value;
				RaisePropertyChanged("SelectedCustCode");
			}
		}
		#endregion

		#region 委外商
		private List<NameValuePair<string>> _outSourceList;
		public List<NameValuePair<string>> OutSourceList
		{
			get { return _outSourceList; }
			set
			{
				_outSourceList = value;
				RaisePropertyChanged("OutSourceList");
			}
		}

		private string _selectedOutSource;
		public string SelectedOutSource
		{
			get { return _selectedOutSource; }
			set
			{
				_selectedOutSource = value;
				RaisePropertyChanged("SelectedOutSource");
			}
		}
		#endregion

		#region 結算期間

		private DateTime? _selectClearingYearMonth;
		/// <summary>
		/// 選擇[結算期間]的年/月
		/// </summary>
		public DateTime? SelectClearingYearMonth
		{
			get { return _selectClearingYearMonth; }
			set
			{
				if (_selectClearingYearMonth == value)
					return;
				_selectClearingYearMonth = value;
				RaisePropertyChanged("SelectClearingYearMonth");
			}
		}

		#endregion


		#region
		private Visibility _hasColsed;

		public Visibility HasClosed
		{
			get { return _hasColsed; }
			set
			{
				if (_hasColsed == value)
					return;
				Set(() => HasClosed, ref _hasColsed, value);
			}
		}
		#endregion
		

		#region [Report總表]下拉選單
		private List<NameValuePair<string>> _reportTypeMainList;
		public List<NameValuePair<string>> ReportTypeMainList
		{
			get { return _reportTypeMainList; }
			set
			{
				_reportTypeMainList = value;
				RaisePropertyChanged("ReportTypeMainList");
			}
		}

		private string _selectedReportTypeMain;
		public string SelectedReportTypeMain
		{
			get { return _selectedReportTypeMain; }
			set
			{
				_selectedReportTypeMain = value;
				RaisePropertyChanged("SelectedReportTypeMain");
			}
		}
		#endregion

		#region [Report明細表]下拉選單
		private List<NameValuePair<string>> _reportTypeDetailList;
		public List<NameValuePair<string>> ReportTypeDetailList
		{
			get { return _reportTypeDetailList; }
			set
			{
				_reportTypeDetailList = value;
				RaisePropertyChanged("ReportTypeDetailList");
			}
		}

		private string _selectedReportTypeDetail;
		public string SelectedReportTypeDetail
		{
			get { return _selectedReportTypeDetail; }
			set
			{
				_selectedReportTypeDetail = value;
				RaisePropertyChanged("SelectedReportTypeDetail");
			}
		}
		#endregion

		#region 結算作業
		private List<F500201ClearingData> _clearingData;
		public List<F500201ClearingData> ClearingData
		{
			get { return _clearingData; }
			set
			{
				_clearingData = value;
				RaisePropertyChanged("ClearingData");
			}
		}

		private F500201ClearingData _selectedClearingData;
		public F500201ClearingData SelectedClearingData
		{
			get { return _selectedClearingData; }
			set
			{
				_selectedClearingData = value;
				RaisePropertyChanged("SelectedClearingData");
				HasClosed = Visibility.Collapsed;
				if (value!=null && value.STATUS == "1")
					HasClosed = Visibility.Visible;
			}
		}

		#endregion

		#region Preview
		public ICommand PreviewCommand
		{
			get
			{
				return new RelayCommand<string>(
				(t) =>
				{
					IsBusy = true;
					try
					{
						//流水條碼為右邊數來四碼
						var rightNum = 4;
						DoPreview((F710510ReportType)Convert.ToInt32(t.Substring(t.Length - rightNum)));
					}
					catch (Exception ex)
					{
						Exception = ex;
						IsBusy = false;
					}
					IsBusy = false;
				},
				(t) => IsSelectedClearingData(t));
			}
		}


		#endregion

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
			ClearingData = GetClearingData();
			if (!ClearingData.Any())
				ShowNoDataMessage();
		}

		public void ShowNoDataMessage()
		{
			ShowMessage(Messages.InfoNoData);
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
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
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

			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSettlementClosing(),
					() => IsSelectedClearingData() && SelectedClearingData.STATUS =="0"
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save

		#region Data
		/// <summary>
		/// 取得[業主]資料
		/// </summary>
		/// <param name="isAll">是否包含[全部]</param>
		public List<NameValuePair<string>> GetGupData(bool isAll = false)
		{
			var data = Wms3plSession.Get<GlobalInfo>().GetGupDataList();

			if (isAll)
				data.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			return data;
		}
		/// <summary>
		/// 取得[貨主]資料
		/// </summary>
		/// <param name="isAll">是否包含[全部]</param>
		private List<NameValuePair<string>> GetCustData(string gupCode, bool isAll = false)
		{
			var data = Wms3plSession.Get<GlobalInfo>().GetCustDataList(null,gupCode);

			if (isAll)
				data.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			return data;
		}
		/// <summary>
		/// 取得[委外商]資料
		/// </summary>
		/// <param name="isAll">是否包含[全部]</param>
		private List<NameValuePair<string>> GetOutSourceData(bool isAll = true)
		{
			var proxy = GetProxy<F19Entities>();
			var data = (from x in proxy.F1928s
									select new NameValuePair<string>()
									{
										Value = x.OUTSOURCE_ID,
										Name = x.OUTSOURCE_NAME
									}).ToList();

			if (isAll)
				data.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			return data;
		}
		/// <summary>
		/// 取得F500202報表類別
		/// </summary>
		/// <param name="reportType">0:總表、1:明細表</param>
		private List<NameValuePair<string>> GetReportType(string reportType = "1")
		{
			var proxy = GetProxy<F50Entities>();
			var data = (from o in proxy.F500202s
									where o.REPORT_TYPE == reportType
									select new NameValuePair<string>
									{
										Name = o.REPORT_NAME,
										Value = o.REPORT_NO,
									}).ToList();
			return data;
		}

		/// <summary>
		/// 重新載入[貨主]資料 - by業主下拉選單
		/// </summary>
		private void ReloadCustList()
		{
			CustList = GetCustData(SelectedGupCode);
			SelectedCustCode = CustList.Select(x => x.Value).FirstOrDefault();
		}

		public List<F500201ClearingData> GetClearingData()
		{			
			var proxyEx = GetExProxy<P50ExDataSource>();
			return proxyEx.CreateQuery<F500201ClearingData>("GetF500201ClearingData")
				.AddQueryExOption("gupCode", SelectedGupCode)
				.AddQueryExOption("custCode", SelectedCustCode)
				.AddQueryExOption("outSourceId", SelectedOutSource)
				.AddQueryExOption("clearingYearMonth", SelectClearingYearMonth.Value.ToString("yyyy/MM"))
				.ToList();
		}

		public List<RP7105100001> GetRp7105100001Data()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			return proxyEx.CreateQuery<RP7105100001>("GetRp7105100001Data")
				.AddQueryExOption("gupCode", SelectedGupCode)
				.AddQueryExOption("custCode", SelectedCustCode)
				.AddQueryExOption("outSourceId", SelectedOutSource)
				.AddQueryExOption("cntDate", SelectedClearingData.CNT_DATE)
				.ToList();
		}

		public List<RP7105100002> GetRp7105100002Data()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			return proxyEx.CreateQuery<RP7105100002>("GetRp7105100002Data")
				.AddQueryExOption("cntDate", SelectedClearingData.CNT_DATE)
				.AddQueryExOption("contractNo", SelectedClearingData.CONTRACT_NO)				
				.ToList();
		}

		public List<RP7105100003> GetRp7105100003Data()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			return proxyEx.CreateQuery<RP7105100003>("GetRp7105100003Data")
				.AddQueryExOption("cntDate", SelectedClearingData.CNT_DATE)
				.AddQueryExOption("contractNo", SelectedClearingData.CONTRACT_NO)				
				.ToList();
		}

		public List<RP7105100004> GetRp7105100004Data()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			return proxyEx.CreateQuery<RP7105100004>("GetRp7105100004Data")
				.AddQueryExOption("cntDate", SelectedClearingData.CNT_DATE)
				.AddQueryExOption("contractNo", SelectedClearingData.CONTRACT_NO)				
				.ToList();
		}

		public List<RP7105100005> GetRp7105100005Data()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			return proxyEx.CreateQuery<RP7105100005>("GetRp7105100005Data")
				.AddQueryExOption("cntDate", SelectedClearingData.CNT_DATE)
				.AddQueryExOption("contractNo", SelectedClearingData.CONTRACT_NO)				
				.ToList();
		}

		/// <summary>
		/// 結算關帳
		/// </summary>
		private void DoSettlementClosing()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("SettlementClosing")
				.AddQueryExOption("cntDate", SelectedClearingData.CNT_DATE)
				.AddQueryExOption("gupCode", SelectedGupCode)
				.AddQueryExOption("custCode", SelectedCustCode)
				.ToList().FirstOrDefault();

			if (result.IsSuccessed)
			{
				ShowWarningMessage(Properties.Resources.P7105100000_ViewModel_DoSettlementClosing_Success);
			}
			else
			{
				ShowResultMessage(result);
			}
		}

		private bool IsSelectedClearingData(string t = "")
		{
			return ClearingData != null &&
			       ClearingData.Any() &&
			       SelectedClearingData != null;
		}

		#endregion
	}

	public enum F710510ReportType
	{
		F7105100001 = 1,
		F7105100002,
		F7105100003,
		F7105100004,
		F7105100005
	}
}
