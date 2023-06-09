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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;

namespace Wms3pl.WpfClient.P16.ViewModel
{


	public partial class P1601050000_ViewModel : InputViewModelBase
	{
		public P1601050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料

				ReportTypeList = GetBaseTableService.GetF000904List(FunctionCode, "P1601050000", "ReportType");
				DcList = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
				StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F160201", "STATUS", true);

				ReportTypeForSearch = ReportTypeList.SelectFirstOrDefault(x => x.Value);
				DcCodeForSearch = DcList.SelectFirstOrDefault(x => x.Value);
				StatusForSearch = StatusList.SelectFirstOrDefault(x => x.Value);
			}

		}

		#region 查詢條件的ItemsSource

		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		private List<NameValuePair<string>> _reportTypeList;

		public List<NameValuePair<string>> ReportTypeList
		{
			get { return _reportTypeList; }
			set
			{
				Set(() => ReportTypeList, ref _reportTypeList, value);
			}
		}


		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				Set(() => DcList, ref _dcList, value);
			}
		}


		private List<NameValuePair<string>> _statusList;

		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				Set(() => StatusList, ref _statusList, value);
			}
		}


		private string _reportTypeForSearch = string.Empty;

		public string ReportTypeForSearch
		{
			get { return _reportTypeForSearch; }
			set
			{
				Set(() => ReportTypeForSearch, ref _reportTypeForSearch, value);

				P17ReturnAuditReportList = null;
				RTO17840ReturnAuditReportList = null;
				B2CReturnAuditReportList = null;
				P106ReturnNotMoveDetailList = null;
				TxtFormatReturnDetailList = null;
				ReturnSerailNoByTypeList = null;
				P015ForecastReturnDetailList = null;
			}
		}

		private string _dcCodeForSearch = string.Empty;

		public string DcCodeForSearch
		{
			get { return _dcCodeForSearch; }
			set
			{
				Set(() => DcCodeForSearch, ref _dcCodeForSearch, value);
			}
		}

		private DateTime? _beginReturnDate = DateTime.Today;

		public DateTime? BeginReturnDate
		{
			get { return _beginReturnDate; }
			set
			{
				Set(() => BeginReturnDate, ref _beginReturnDate, value);
			}
		}

		private DateTime? _endReturnDate = DateTime.Today;

		public DateTime? EndReturnDate
		{
			get { return _endReturnDate; }
			set
			{
				Set(() => EndReturnDate, ref _endReturnDate, value);
			}
		}

		private string _returnNoForSearch = string.Empty;

		public string ReturnNoForSearch
		{
			get { return _returnNoForSearch; }
			set
			{
				Set(() => ReturnNoForSearch, ref _returnNoForSearch, value);
			}
		}

		private string _statusForSearch;

		public string StatusForSearch
		{
			get { return _statusForSearch; }
			set
			{
				Set(() => StatusForSearch, ref _statusForSearch, value);
			}
		}


		#endregion

		#region 報表的ItemsSource

		private List<P160201Report> _p160201ReportList;

		public List<P160201Report> P160201ReportList
		{
			get { return _p160201ReportList; }
			set
			{
				Set(() => P160201ReportList, ref _p160201ReportList, value);
			}
		}


		private List<P17ReturnAuditReport> _p17ReturnAuditReportList;

		public List<P17ReturnAuditReport> P17ReturnAuditReportList
		{
			get { return _p17ReturnAuditReportList; }
			set
			{
				Set(() => P17ReturnAuditReportList, ref _p17ReturnAuditReportList, value);
			}
		}

		private List<RTO17840ReturnAuditReport> _rTO17840ReturnAuditReportList;

		public List<RTO17840ReturnAuditReport> RTO17840ReturnAuditReportList
		{
			get { return _rTO17840ReturnAuditReportList; }
			set
			{
				Set(() => RTO17840ReturnAuditReportList, ref _rTO17840ReturnAuditReportList, value);
			}
		}

		private List<B2CReturnAuditReport> _b2CReturnAuditReportList;

		public List<B2CReturnAuditReport> B2CReturnAuditReportList
		{
			get { return _b2CReturnAuditReportList; }
			set
			{
				Set(() => B2CReturnAuditReportList, ref _b2CReturnAuditReportList, value);
			}
		}

		private List<P106ReturnNotMoveDetail> _p106ReturnNotMoveDetailList;

		public List<P106ReturnNotMoveDetail> P106ReturnNotMoveDetailList
		{
			get { return _p106ReturnNotMoveDetailList; }
			set
			{
				Set(() => P106ReturnNotMoveDetailList, ref _p106ReturnNotMoveDetailList, value);
			}
		}

		private List<TxtFormatReturnDetail> _txtFormatReturnDetailList;

		public List<TxtFormatReturnDetail> TxtFormatReturnDetailList
		{
			get { return _txtFormatReturnDetailList; }
			set
			{
				Set(() => TxtFormatReturnDetailList, ref _txtFormatReturnDetailList, value);
			}
		}

		private List<ReturnSerailNoByType> _returnSerailNoByTypeList;

		public List<ReturnSerailNoByType> ReturnSerailNoByTypeList
		{
			get { return _returnSerailNoByTypeList; }
			set
			{
				Set(() => ReturnSerailNoByTypeList, ref _returnSerailNoByTypeList, value);
			}
		}

		private List<P015ForecastReturnDetail> _p015ForecastReturnDetailList;

		public List<P015ForecastReturnDetail> P015ForecastReturnDetailList
		{
			get { return _p015ForecastReturnDetailList; }
			set
			{
				Set(() => P015ForecastReturnDetailList, ref _p015ForecastReturnDetailList, value);
			}
		}

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				bool hasItems = false;
				return CreateBusyAsyncCommand(
					o => hasItems = DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						if (!hasItems)
							ShowMessage(Messages.InfoNoData);
					}
					);
			}
		}

		private bool DoSearch()
		{
			//執行查詢動作
			var proxy = GetExProxy<P16ExDataSource>();

			switch (ReportTypeForSearch)
			{
				case "0":	// P017_退貨報表
					return (P160201ReportList = SearchReportList(proxy.GetP160201Reports)).Any();

				case "1":	// P107_退貨記錄總表報表
					return (P17ReturnAuditReportList = SearchReportList(proxy.GetP17ReturnAuditReports)).Any();

				case "2":	// RTO17840退貨記錄表
					return (RTO17840ReturnAuditReportList = SearchReportList(proxy.GetRTO17840ReturnAuditReports)).Any();

				case "3":	// B2C退貨記錄表(Friday退貨記錄表)
					return (B2CReturnAuditReportList = SearchReportList(proxy.GetB2CReturnAuditReports)).Any();

				case "4":	// P106_退貨未上架明細表
					return (P106ReturnNotMoveDetailList = SearchReportList(proxy.GetP106ReturnNotMoveDetails)).Any();

				case "5":	// 退貨詳細資料
					return (TxtFormatReturnDetailList = SearchReportList(proxy.GetTxtFormatReturnDetails)).Any();

				case "6":	// 退貨資料1
				case "7":	// 退貨資料2
					return (ReturnSerailNoByTypeList = SearchReportList(proxy.GetReturnSerailNosByType)).Any();

				case "8":	// P015_預計退貨明細表
					return (P015ForecastReturnDetailList = SearchReportList(proxy.GetP015ForecastReturnDetails)).Any();

				default:
					return false;
			}
		}

		List<T> SearchReportList<T>(Func<String, String, String, DateTime?, DateTime?, String, String, IQueryable<T>> func)
		{
			return func.Invoke(DcCodeForSearch, _gupCode, _custCode, BeginReturnDate, EndReturnDate, ReturnNoForSearch, StatusForSearch).ToList();
		}

		List<T> SearchReportList<T>(Func<String, String, String, DateTime?, DateTime?, String, String, String, IQueryable<T>> func)
		{
			// 參考倉儲作業工作報表格式範本 61-7, 61-8
			var commaSeparatorTypes = ReportTypeForSearch == "6" ? "02,03,04" : "01,02,03,04";

			return func.Invoke(DcCodeForSearch, _gupCode, _custCode, BeginReturnDate, EndReturnDate, ReturnNoForSearch, StatusForSearch, commaSeparatorTypes).ToList();
		}
		#endregion Search

		#region Print
		public ICommand PrintCommand
		{
			get
			{
				PrintType printType = PrintType.Preview;
				bool hasItems = false;
				return CreateBusyAsyncCommand(
					o =>
					{
						printType = (PrintType)o;
						hasItems = DoSearch();
					},
					() => UserOperateMode == OperateMode.Query,
					o => OnPrintCompleted(printType, hasItems)
					);
			}
		}

		public Action<PrintType, bool> OnPrintCompleted;
		#endregion Print

		#region Export
		public ICommand ExportCommand
		{
			get
			{
				bool hasItems = false;
				return CreateBusyAsyncCommand(
					o => hasItems = DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o => OnExportCompleted(hasItems)
					);
			}
		}

		public Action<bool> OnExportCompleted;


		public IEnumerable<string> GetReportLines()
		{
			switch (ReportTypeForSearch)
			{
				case "5":	// 退貨詳細資料
					return GetTxtFormatReturnDetailsLines(TxtFormatReturnDetailList);

				case "6":	// 退貨資料1
				case "7":	// 退貨資料2
					return GetReturnSerailNoByTypeLines(ReturnSerailNoByTypeList);
			}

			return null;
		}

		IEnumerable<string> GetTxtFormatReturnDetailsLines(IEnumerable<TxtFormatReturnDetail> items)
		{
			yield return Properties.Resources.P1601050000_ColumnName1;

			foreach (var x in items)
				yield return string.Join("|", new string[] { x.SERIAL_NO, x.CELL_NUM, x.WMS_ORD_NO, x.SECOND_ITEM_CODE, x.ITEM_CODE, x.RTN_CUST_CODE, x.RTN_CUST_NAME, (x.DELV_DATE.HasValue ? x.DELV_DATE.Value.ToString("yyyy/MM/dd") : string.Empty) });

			yield return string.Empty;	// 換行
			yield return Properties.Resources.P1601050000_Footer;

			foreach (var g in items.GroupBy(x => new { x.ITEM_CODE, x.SECOND_ITEM_CODE }))
				yield return string.Join(" ", new string[] { g.Key.ITEM_CODE, g.Key.SECOND_ITEM_CODE, g.Count().ToString() });
		}

		IEnumerable<string> GetReturnSerailNoByTypeLines(IEnumerable<ReturnSerailNoByType> items)
		{
			yield return Properties.Resources.P1601050000_ColumnName2;

			foreach (var x in items)
				yield return string.Join("|", new string[] { x.SERIAL_NO, x.CELL_NUM, x.STATUS });
		}

		#endregion Export

	}
}
