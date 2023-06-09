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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.ExDataServices;

namespace Wms3pl.WpfClient.P71.ViewModel
{

	public partial class P7107020000_ViewModel : InputViewModelBase
	{
		public Action<PrintType> DoPrintReport = delegate { };
		public P7107020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}

		}

		#region Properties,Fields
		private bool isHasRecords = false;
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
				GupCodes = GetGupList(_selectedDc, true);
				if (UserOperateMode == OperateMode.Query && GupCodes.Any())
					SelectedGupCode = GupCodes.FirstOrDefault().Value;
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
		#endregion
		#region ALL業主
		private Dictionary<string, List<NameValuePair<string>>> _allGupList;

		public Dictionary<string, List<NameValuePair<string>>> AllGupList
		{
			get { return _allGupList; }
			set
			{
				if (_allGupList == value)
					return;
				Set(() => AllGupList, ref _allGupList, value);
			}
		}
		#endregion
		#region ALL貨主
		private Dictionary<string, List<NameValuePair<string>>> _allCustList;

		public Dictionary<string, List<NameValuePair<string>>> AllCustList
		{
			get { return _allCustList; }
			set
			{
				if (_allCustList == value)
					return;
				Set(() => AllCustList, ref _allCustList, value);
			}
		}
		#endregion
		#region Form - 業主
		private List<NameValuePair<string>> _gupCodes;

		public List<NameValuePair<string>> GupCodes
		{
			get { return _gupCodes; }
			set
			{
				_gupCodes = value;
				RaisePropertyChanged("GupCodes");
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
				CustCodes = GetCustList(_selectedGupCode, true);
				if (UserOperateMode == OperateMode.Query && CustCodes.Any())
					SelectedCustCode = CustCodes.FirstOrDefault().Value;
			}
		}
		#endregion
		#region Form - 貨主
		private List<NameValuePair<string>> _custCodes;

		public List<NameValuePair<string>> CustCodes
		{
			get { return _custCodes; }
			set
			{
				_custCodes = value;
				RaisePropertyChanged("CustCodes");
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
		#region From - 查詢收貨日期(起)
		private DateTime _selectDateStart = DateTime.Now.Date;

		public DateTime SelectDateStart
		{
			get { return _selectDateStart; }
			set
			{
				if (_selectDateStart == value)
					return;
				Set(() => SelectDateStart, ref _selectDateStart, value);
			}
		}
		#endregion
		#region From - 查詢收貨日期(迄)
		private DateTime _selectDateEnd = DateTime.Now.Date;

		public DateTime SelectDateEnd
		{
			get { return _selectDateEnd; }
			set
			{
				if (_selectDateEnd == value)
					return;
				Set(() => SelectDateEnd, ref _selectDateEnd, value);
			}
		}
		#endregion
		#region Form - 選擇的報表類別
		private string _selectedReportType;

		public string SelectedReportType
		{
			get { return _selectedReportType; }
			set
			{
				if (_selectedReportType == value)
					return;
				SetClear();
				Set(() => SelectedReportType, ref _selectedReportType, value);
			}
		}
		#endregion
		#region Form - 可用的報表類別清單
		private List<NameValuePair<string>> _reportTypeList;

		public List<NameValuePair<string>> ReportTypeList
		{
			get { return _reportTypeList; }
			set
			{
				if (_reportTypeList == value)
					return;
				Set(() => ReportTypeList, ref _reportTypeList, value);
			}
		}
		#endregion
		#region Form - 選擇的配送廠商
		private string _selectedAllId;

		public string SelectedAllId
		{
			get { return _selectedAllId; }
			set
			{
				if (_selectedAllId == value)
					return;
				Set(() => SelectedAllId, ref _selectedAllId, value);
			}
		}
		#endregion
		#region Form - 配送廠商清單
		private Dictionary<string, List<NameValuePair<string>>> _allIdListDict;

		public Dictionary<string, List<NameValuePair<string>>> AllIdListDict
		{
			get { return _allIdListDict; }
			set
			{
				if (_allIdListDict == value)
					return;
				Set(() => AllIdListDict, ref _allIdListDict, value);
			}
		}
		#endregion
		#region 供應商
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
		private string _selectedVnrCode;
		public string SelectedVnrCode
		{
			get { return _selectedVnrCode; }
			set
			{
				_selectedVnrCode = value;
				RaisePropertyChanged("SelectedVnrCode");
			}
		}
		#endregion
		#region Form - 選擇的委外商
		private string _selectedOutSource;

		public string SelectedOutSource
		{
			get { return _selectedOutSource; }
			set
			{
				if (_selectedOutSource == value)
					return;
				Set(() => SelectedOutSource, ref _selectedOutSource, value);
			}
		}
		#endregion
		#region Form - 委外商清單
		private List<NameValuePair<string>> _outSourceList;

		public List<NameValuePair<string>> OutSourceList
		{
			get { return _outSourceList; }
			set
			{
				if (_outSourceList == value)
					return;
				Set(() => OutSourceList, ref _outSourceList, value);
			}
		}
		#endregion

		#region 綜合清單

		private List<F51ComplexReportData> _integrateRecords;

		public List<F51ComplexReportData> IntegrateRecords
		{
			get { return _integrateRecords; }
			set
			{
				if (_integrateRecords == value)
					return;
				Set(() => IntegrateRecords, ref _integrateRecords, value);
			}
		}
		#endregion
		#region 派車清單

		private List<F700101DistrCarData> _distrCarRecords;

		public List<F700101DistrCarData> DistrCarRecords
		{
			get { return _distrCarRecords; }
			set
			{
				if (_distrCarRecords == value)
					return;
				Set(() => DistrCarRecords, ref _distrCarRecords, value);
			}
		}
		#endregion
		#region 流通加工記錄表清單

		private List<F910201ProcessData> _processRecords;

		public List<F910201ProcessData> ProcessRecords
		{
			get { return _processRecords; }
			set
			{
				if (_processRecords == value)
					return;
				Set(() => ProcessRecords, ref _processRecords, value);
			}
		}
		#endregion
		#endregion

		#region Function
		private void InitControls()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList != null && DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
			SetAllGupList();
			SetAllCustList();
			if (GupCodes.Any())
				SelectedGupCode = GupCodes.FirstOrDefault().Value;
			if (CustCodes.Any())
				SelectedCustCode = CustCodes.FirstOrDefault().Value;
			//可用的報表類別清單
			ReportTypeList = GetBaseTableService.GetF000904List(FunctionCode, "P7107020000", "REPORT_TYPE");
			SelectedReportType = ReportTypeList.Select(x => x.Value).FirstOrDefault();
			//供應商
			VnrList = GetVnrData(SelectedGupCode);
			SelectedVnrCode = VnrList.Select(x => x.Value).FirstOrDefault();
			//配送廠商清單
			AllIdListDict = GetAllIdListDict();
			if (AllIdListDict != null && AllIdListDict.Where(x => x.Key == SelectedDc).Any() &&
					AllIdListDict.Where(x => x.Key == SelectedDc).SingleOrDefault().Value.Any())
				SelectedAllId = AllIdListDict.Where(x => x.Key == SelectedDc).SingleOrDefault().Value.FirstOrDefault().Value;
			//委外商清單
			OutSourceList = GetOutSourceList();
			SelectedOutSource = OutSourceList.Select(x => x.Value).FirstOrDefault();
		}

		private List<NameValuePair<string>> GetOutSourceList()
		{
			var proxy = GetProxy<F19Entities>();
			var results = proxy.F1928s.Where(x => x.STATUS != "9").ToList()
																.OrderBy(x => x.OUTSOURCE_NAME)
																.Select(x => new NameValuePair<string>()
																	{
																		Name = x.OUTSOURCE_NAME,
																		Value = x.OUTSOURCE_ID
																	}).ToList();
			results.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });
			return results;
		}

		private Dictionary<string, List<NameValuePair<string>>> GetAllIdListDict()
		{
			var proxy = GetProxy<F19Entities>();
			var results = proxy.F1947s.ToList()
				.GroupBy(o => o.DC_CODE).ToDictionary(item => item.Key,
																							item => item.Select(o => new NameValuePair<string>
																							{
																								Name = o.ALL_COMP,
																								Value = o.ALL_ID
																							}).Distinct().ToList());

			if (results != null && results.Any())
			{
				results.ToList().ForEach(x => x.Value.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All }));
			}
			return results;
		}
		/// <summary>
		/// 取得[供應商]資料
		/// </summary>
		public List<NameValuePair<string>> GetVnrData(string gupCode, bool isAll = true)
		{
			var proxy = GetProxy<F19Entities>();
			var data = (from x in proxy.F1908s
						where string.IsNullOrWhiteSpace(gupCode) || x.GUP_CODE == gupCode
						select new NameValuePair<string>()
						{
							Value = x.VNR_CODE,
							Name = x.VNR_NAME
						}).ToList();

			if (isAll)
				data.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			return data;
		}
		public List<NameValuePair<string>> GetGupList(string dcCode, bool canAll = false)
		{
			var query = from item in Wms3plSession.Get<GlobalInfo>().DcGupCustDatas
						where item.DcCode == dcCode
						group item by new { item.GupCode, item.GupName } into g
						select new NameValuePair<string>()
						{
							Value = g.Key.GupCode,
							Name = g.Key.GupName
						};
			var list = query.ToList();
			if (canAll)
			{
				list.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
			}
			return list;
		}

		public List<NameValuePair<string>> GetCustList(string gupCode, bool canAll = false)
		{
			var query = from item in Wms3plSession.Get<GlobalInfo>().DcGupCustDatas
						where item.GupCode == gupCode
						group item by new { item.CustCode, item.CustName } into g
						select new NameValuePair<string>()
						{
							Value = g.Key.CustCode,
							Name = g.Key.CustName
						};
			var list = query.ToList();
			if (canAll)
			{
				list.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
			}
			return list;
		}

		private void SetAllGupList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcGupCustDatas;
			AllGupList = data.GroupBy(o => o.DcCode).ToDictionary(item => item.Key, item => item.Select(o => new NameValuePair<string>
			{
				Name = o.GupName,
				Value = o.GupCode
			}).Distinct().ToList());
		}
		private void SetAllCustList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcGupCustDatas;
			var data2 = (from o in data
						 select new { o.GupCode, o.CustCode, o.CustName }).Distinct();
			AllCustList = data2.GroupBy(o => o.GupCode).ToDictionary(item => item.Key, item => item.Select(o => new NameValuePair<string>
			{
				Name = o.CustName,
				Value = o.CustCode
			}).ToList());
		}
		private void SetClear()
		{
			isHasRecords = false;
		}
		#endregion

		#region Command
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
			switch (SelectedReportType)
			{
				case "2"://綜合
					IntegrateRecords = GetIntegrateRecords();
					isHasRecords = (IntegrateRecords != null && IntegrateRecords.Any());
					break;
				case "3"://派車
					DistrCarRecords = GetDistrCarRecords();
					isHasRecords = (DistrCarRecords != null && DistrCarRecords.Any());
					break;
				case "4"://流通加工記錄表
					ProcessRecords = GetProcessRecords();
					isHasRecords = (ProcessRecords != null && ProcessRecords.Any());
					break;
				default:
					isHasRecords = false;
					break;
			}

			if (!isHasRecords)
				ShowMessage(Messages.InfoNoData);
		}

		private List<F910201ProcessData> GetProcessRecords()
		{
			var proxyEx = GetExProxy<P71ExDataSource>();

			var results = proxyEx.CreateQuery<F910201ProcessData>("GetProcessDatas")
												 .AddQueryExOption("dcCode", SelectedDc)
												 .AddQueryExOption("crtSDate", SelectDateStart.ToShortDateString())
												 .AddQueryExOption("crtEDate", SelectDateEnd.ToShortDateString())
												 .AddQueryExOption("outSourceId", SelectedOutSource)
												 .AsQueryable()
												 .ToList();
			return results;
		}

		private List<F700101DistrCarData> GetDistrCarRecords()
		{
			var proxyEx = GetExProxy<P71ExDataSource>();
			var results = proxyEx.GetDistrCarDatas(SelectedDc,
													SelectedGupCode,
													SelectedCustCode,
													SelectDateStart,
													SelectDateEnd,
													SelectedAllId).ToList();
			return results;
		}

		private List<F51ComplexReportData> GetIntegrateRecords()
		{
			var proxyEx = GetExProxy<P71ExDataSource>();

			var results = proxyEx.CreateQuery<F51ComplexReportData>("GetF51ComplexReportData")
												 .AddQueryExOption("dcCode", SelectedDc)
												 .AddQueryExOption("calSDate", SelectDateStart.ToShortDateString())
												 .AddQueryExOption("calEDate", SelectDateEnd.ToShortDateString())
												 .AddQueryExOption("gupCode", SelectedGupCode)
												 .AddQueryExOption("custCode", SelectedCustCode)
												 .AddQueryExOption("allId", SelectedAllId)
												 .AsQueryable()
												 .ToList();
			return results;
		}
		#endregion Search
		#region Print
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
						DoPrint,
						(t) => !IsBusy && isHasRecords && UserOperateMode == OperateMode.Query);
			}
		}

		private void DoPrint(PrintType printType)
		{
			//取得報表資料
			DoPrintReport(printType);
		}

		#endregion
		#endregion
	}
}