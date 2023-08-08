using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P21ExDataService;
using Wms3pl.WpfClient.Common;
using System.Windows.Media;
using System.Windows;
using Wms3pl.Datas.F19;
using Wms3pl.WpfClient.ExDataServices.P70ExDataService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P21.ViewModel
{
	public enum appType
	{
		office = 1,
		RF = 2
	}
	public partial class P2115010000_ViewModel : InputViewModelBase
	{
		public P2115010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitCommand.Execute(null);
			}
		}

		#region Property

		#region
		private string _gupCode = string.Empty;
		public string GupCode
		{
			get { return _gupCode; }
			set { _gupCode = value; }
		}
		private string _custCode = string.Empty;
		public string CustCode
		{
			get { return _custCode; }
			set { _custCode = value; }
		}
		private string _account = string.Empty;
		public string Account
		{
			get { return _account; }
			set { _account = value; }
		}
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
		#endregion

		#region office或RF介面
		private appType _myAppType = appType.office;
		public appType MyAppType
		{
			get { return _myAppType; }
			set { _myAppType = value; }
		}
		#endregion


		private List<P211501_WorkList> _myWorkList;

		public List<P211501_WorkList> MyWorkList
		{
			get { return _myWorkList; }
			set
			{
				if (_myWorkList == value)
					return;
				Set(() => MyWorkList, ref _myWorkList, value);
			}
		}

		private List<P211501_WorkList> _WorkList0603;

		public List<P211501_WorkList> WorkList0603
		{
			get { return _WorkList0603; }
			set
			{
				if (_WorkList0603 == value)
					return;
				Set(() => WorkList0603, ref _WorkList0603, value);
			}
		}

		private List<P211501_WorkList> _todayWorkList;

		public List<P211501_WorkList> TodayWorkList
		{
			get { return _todayWorkList; }
			set
			{
				if (_todayWorkList == value)
					return;
				Set(() => TodayWorkList, ref _todayWorkList, value);
			}
		}

		private List<NameValuePair<string>> _funcList;

		public List<NameValuePair<string>> FuncList
		{
			get { return _funcList; }
			set
			{
				if (_funcList == value)
					return;
				Set(() => FuncList, ref _funcList, value);
			}
		}

		#endregion



		#region 所有行事曆資料
		private List<F700501Ex> _allScheduleList;

		public List<F700501Ex> AllScheduleList
		{
			get { return _allScheduleList; }
			set
			{
				Set(() => AllScheduleList, ref _allScheduleList, value);
			}
		}
		#endregion


		#region 行事曆資料-BindGrid
		private List<F700501Ex> _scheduleList;

		public List<F700501Ex> ScheduleList
		{
			get { return _scheduleList; }
			set
			{
				Set(() => ScheduleList, ref _scheduleList, value);
			}
		}
		#endregion


		#region 行事曆分類
		private SelectionList<NameValuePair<string>> _subjectList;

		public SelectionList<NameValuePair<string>> SubjectList
		{
			get { return _subjectList; }
			set
			{
				Set(() => SubjectList, ref _subjectList, value);
			}
		}
		#endregion


		#endregion

		#region Command
		#region Init
		public ICommand InitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => InitControls(), () => UserOperateMode == OperateMode.Query,
					o => InitComplete()
					);
			}
		}

		private void InitComplete()
		{
			SearchCommand.Execute(null);
		}
		private void InitControls()
		{
			GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			Account = Wms3plSession.CurrentUserInfo.Account;
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList != null && DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
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
			MyWorkList = GetfunctionIds();
			ChartOrLineSourceByOrdPropListA = GenerateChart(WorkList0603, FuncList);
			SetScehudleList();
		}


		private void SetScehudleList()
		{
			var proxyP70Ex = GetExProxy<P70ExDataSource>();
			var data = proxyP70Ex.CreateQuery<F700501Ex>("GetF700501Ex")
			.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
			.AddQueryOption("dateBegin", string.Format("'{0}'", DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd")))
			.AddQueryOption("dateEnd", string.Format("'{0}'", DateTime.Today.ToString("yyyy/MM/dd")))
			.AddQueryOption("scheduleType", string.Format("'{0}'", "W")).OrderByDescending(x => x.SCHEDULE_DATE).ThenByDescending(x => x.SCHEDULE_TIME).ToList();

			SubjectList = data.Select(x => x.SUBJECT).Distinct().Select(x => new NameValuePair<string> { Name = x, Value = x }).ToSelectionList(true);
			AllScheduleList = data;
			ScheduleList = data;


		}

		private List<P211501_WorkList> GetfunctionIds()
		{
			var proxyEx = GetExProxy<P21ExDataSource>();

			var results = proxyEx.CreateQuery<WorkList>("GetWorkListDatas")
												 .AddQueryExOption("dcCode", SelectedDc)
												 .AddQueryExOption("gupCode", GupCode)
												 .AddQueryExOption("custCode", CustCode)
												 .AddQueryExOption("apType", MyAppType.ToString())
												 .AddQueryExOption("account", Account)
												 .AsQueryable()
												 .ToList();

			var resultsChart = proxyEx.CreateQuery<WorkList>("GetChartListDatas")
												 .AddQueryExOption("dcCode", SelectedDc)
												 .AddQueryExOption("gupCode", GupCode)
												 .AddQueryExOption("custCode", CustCode)
												 .AddQueryExOption("apType", MyAppType.ToString())
												 .AddQueryExOption("account", Account)
												 .AsQueryable()
												 .ToList();



			var proxyP19Ex = GetExProxy<P19ExDataSource>();
			//var resultsFun = proxyP19Ex.CreateQuery<F1954>("GetUserFunctions")
			//									 .AddQueryExOption("account", Account)
			//									 .AsQueryable()
			//									 .ToList();
			var resultsFun = Wms3plSession.Get<IEnumerable<Function>>();
			List<P211501_WorkList> tmpList = new List<P211501_WorkList>();
			List<P211501_WorkList> tmpList0603 = new List<P211501_WorkList>();
			tmpList0603.Add(new P211501_WorkList { ROWNUM = 0 });
			List<NameValuePair<string>> d = new List<NameValuePair<string>>();

			foreach (var a in results)
			{
				if (resultsFun.Where(item => item.Id == a.FUNC_ID).ToList().Count > 0)
				{
					if ((a.VALUE != "06" || a.FUNC_ID == "P0503020000" || a.FUNC_ID == "P1602010000") && a.VALUE != "03")
					{
						var tmpCnt = a.COUNTS_B == 0 ? 0 : (Math.Round((Math.Abs((a.COUNTS_C - a.COUNTS_B)) / a.COUNTS_B), 2) * 100);
						var tmpBackColor = Brushes.White;
						var tmpFontColor = Brushes.Black;
						if (tmpCnt > 80) { tmpBackColor = Brushes.Green; }
						else if (tmpCnt > 60 && tmpCnt < 80) { tmpBackColor = Brushes.Yellow; }
						else if (tmpCnt > 0 && tmpCnt <= 60)
						{
							tmpBackColor = Brushes.Red;
						}
						else
						{
							tmpBackColor = Brushes.Transparent;
						}

						var workList = new P211501_WorkList()
						{
							ROWNUM = a.ROWNUM + 3,
							COUNTS = a.COUNTS,
							COUNTS_B = a.COUNTS_B,
							COUNTS_C = a.COUNTS_C,
							FUNC_ID = a.FUNC_ID,
							FUNC_NAME = a.FUNC_NAME,
							NAME = a.NAME,
							VALUE = a.VALUE,
							PercentCnt = tmpCnt.ToString() + "%",
							BackColorCombine = tmpBackColor,
							ForeColorCombine = tmpFontColor,
							func1Enable = resultsFun.Where(item => item.Id == a.FUNC_ID).ToList().Count > 0 ? true : false,
							func2Enable = true,
							func3Enable = true
						};

						if (a.FUNC_ID == "P0102010000")
						{
                            //判斷使用者權限如有商品檢驗與容器綁定則顯示該功能，否則顯示商品檢驗
                            var functions = Wms3plSession.Get<IEnumerable<Function>>();
                            bool isEnabled = functions != null && functions.Any(f => f.Id == "P0202060000");
                            if (isEnabled)
                            {
                                workList.ROWNUM = 1;
                                workList.FUNC_IDB = "P0202060000";
                                workList.FUNCBVIS = Visibility.Visible;
                                workList.FUNC_NAMEB = Properties.Resources.ProductInspectionContainer; // "商品檢驗與容器綁定"
                                workList.func2Enable = resultsFun.Where(item => item.Id == "P0202060000").ToList().Count > 0 ? true : false;
                            }
                            else
                            {
                                workList.ROWNUM = 1;
                                workList.FUNC_IDB = "P0202030000";
                                workList.FUNCBVIS = Visibility.Visible;
                                workList.FUNC_NAMEB = Properties.Resources.ProductInspection; // "商品檢驗"
                                workList.func2Enable = resultsFun.Where(item => item.Id == "P0202030000").ToList().Count > 0 ? true : false;
                            }
                            
                            
							

							workList.FUNC_IDC = "P0203010000";
							workList.FUNCCVIS = Visibility.Visible;
							workList.FUNC_NAMEC = Properties.Resources.TransferringShelf; // "調撥上架"
							workList.func3Enable = resultsFun.Where(item => item.Id == "P0203010000").ToList().Count > 0 ? true : false;
						}
						else if (a.FUNC_ID == "P0503020000")
						{
                            #region 訂單維護要加在Grid

                            //若沒資料,則顯示進度為0%
                            workList.ROWNUM = 2;
                            workList.FUNC_IDB = "P0501040000";
                            workList.FUNCBVIS = Visibility.Visible;
                            workList.FUNC_NAMEB = Properties.Resources.PrintedPickNo; // "揀貨單列印"
                            workList.func2Enable = resultsFun.Where(item => item.Id == "P0501040000").ToList().Count > 0 ? true : false;
                            
                            #endregion
                        }
                        else if (a.FUNC_ID == "P1601010000")
						{
							#region 退貨單維護

							workList.ROWNUM = 3;
							workList.FUNC_IDB = "P0802010000";
							workList.FUNCBVIS = Visibility.Visible;
							workList.FUNC_NAMEB = Properties.Resources.ReturnInspection; // "退貨檢驗"
							workList.func2Enable = resultsFun.Where(item => item.Id == "P0802010000").ToList().Count > 0 ? true : false;

							workList.FUNC_IDC = "P1601020000";
							workList.FUNCCVIS = Visibility.Visible;
							workList.FUNC_NAMEC = Properties.Resources.ReturnShelfApplication; // "退貨上架申請"
							workList.func3Enable = resultsFun.Where(item => item.Id == "P1601020000").ToList().Count > 0 ? true : false;
							
							#endregion 退貨單維護
						}
						else if (a.FUNC_ID == "P1602010000")
						{
							#region 廠退單維護

							workList.ROWNUM = 4;
							workList.FUNC_IDB = "P1602020000";
							workList.FUNCBVIS = Visibility.Visible;
							workList.FUNC_NAMEB = Properties.Resources.VendorReturnShipping; // "廠退出貨"
							workList.func2Enable = resultsFun.Where(item => item.Id == "P1602020000").ToList().Count > 0 ? true : false;
							
							#endregion 廠退單維護
						}
						tmpList.Add(workList);
					}
				}
			}
			foreach (var a in resultsChart)
			{
				var tmpCnt = a.COUNTS_B == 0 ? 0 : (Math.Round((Math.Abs((a.COUNTS_C - a.COUNTS_B)) / a.COUNTS_B), 2) * 100);
				var tmpBackColor = Brushes.White;
				var tmpFontColor = Brushes.Black;
				if (tmpCnt > 80) { tmpBackColor = Brushes.Green; }
				else if (tmpCnt > 60 && tmpCnt < 80) { tmpBackColor = Brushes.Yellow; }
				else { tmpBackColor = Brushes.Red; }
				switch (a.FUNC_ID)
				{
					case "P0503020000":
						a.ROWNUM = 1;
						break;
					case "P0501040000":
						a.ROWNUM = 2;
						break;
					case "P0807010000":
						a.ROWNUM = 3;
						break;
					case "P0601010000":
						a.ROWNUM = 4;
						break;

				}

				tmpList0603.Add(new P211501_WorkList()
				{
					ROWNUM = a.ROWNUM,
					COUNTS = a.COUNTS,
					COUNTS_B = a.COUNTS_B,
					COUNTS_C = a.COUNTS_C,
					FUNC_ID = a.FUNC_ID,
					FUNC_NAME = a.FUNC_NAME,
					NAME = a.NAME,
					VALUE = a.VALUE,
					PercentCnt = tmpCnt.ToString() + "%",
					BackColorCombine = tmpBackColor,
					ForeColorCombine = tmpFontColor,
					func1Enable = true,
					func2Enable = true,
					func3Enable = true
				});
			}


			var changeList = tmpList0603.OrderBy(a => a.ROWNUM).ToList();
			foreach (var i in changeList)
			{
				d.Add(new NameValuePair<string>() { Name = i.FUNC_NAME, Value = i.ROWNUM.ToString() });
			}
			changeList[0].VisEnable = Visibility.Collapsed;
			changeList[changeList.Count() - 1].VisEnable = Visibility.Collapsed;

			WorkList0603 = changeList;
			FuncList = d;
			return tmpList.OrderBy(a => a.ROWNUM).ToList();
		}

		public class P211501_WorkList
		{
			private decimal _rownum; public decimal ROWNUM { get { return _rownum; } set { _rownum = value; } }
			private string _value; public string VALUE { get { return _value; } set { _value = value; } }
			private string _name; public string NAME { get { return _name; } set { _name = value; } }
			private string _funcId; public string FUNC_ID { get { return _funcId; } set { _funcId = value; } }
			private string _funcName; public string FUNC_NAME { get { return _funcName; } set { _funcName = value; } }
			private string _funcIdB; public string FUNC_IDB { get { return _funcIdB; } set { _funcIdB = value; } }
			private string _funcNameB; public string FUNC_NAMEB { get { return _funcNameB; } set { _funcNameB = value; } }
			private string _funcIdC; public string FUNC_IDC { get { return _funcIdC; } set { _funcIdC = value; } }
			private string _funcNameC; public string FUNC_NAMEC { get { return _funcNameC; } set { _funcNameC = value; } }
			private decimal _counts; public decimal COUNTS { get { return _counts; } set { _counts = value; } }
			private decimal _countsB; public decimal COUNTS_B { get { return _countsB; } set { _countsB = value; } }
			private decimal _countsC; public decimal COUNTS_C { get { return _countsC; } set { _countsC = value; } }
			private string _PercentCnt; public string PercentCnt { get { return _PercentCnt; } set { _PercentCnt = value; } }
			private Visibility _VisEnable; public Visibility VisEnable { get { return _VisEnable; } set { _VisEnable = value; } }
			private Visibility _FUNCBVIS = Visibility.Collapsed; public Visibility FUNCBVIS { get { return _FUNCBVIS; } set { _FUNCBVIS = value; } }
			private Visibility _FUNCCVIS = Visibility.Collapsed; public Visibility FUNCCVIS { get { return _FUNCCVIS; } set { _FUNCCVIS = value; } }
			private bool _func1Enable; public bool func1Enable { get { return _func1Enable; } set { _func1Enable = value; } }
			private bool _func2Enable; public bool func2Enable { get { return _func2Enable; } set { _func2Enable = value; } }
			private bool _func3Enable; public bool func3Enable { get { return _func3Enable; } set { _func3Enable = value; } }
			private System.Windows.Media.Brush _BackColorCombine = Brushes.White; public System.Windows.Media.Brush BackColorCombine { get { return _BackColorCombine; } set { _BackColorCombine = value; } }

			private System.Windows.Media.Brush _ForeColorCombine = Brushes.Black; public System.Windows.Media.Brush ForeColorCombine { get { return _ForeColorCombine; } set { _ForeColorCombine = value; } }

		}

		#region 今天工作項目
		private List<ChartOrLineSourceByOrdProp> _chartOrLineSourceByOrdPropListA;

		public List<ChartOrLineSourceByOrdProp> ChartOrLineSourceByOrdPropListA
		{
			get { return _chartOrLineSourceByOrdPropListA; }
			set
			{
				if (_chartOrLineSourceByOrdPropListA == value)
					return;
				Set(() => ChartOrLineSourceByOrdPropListA, ref _chartOrLineSourceByOrdPropListA, value);
			}
		}
		#endregion

		#region gen圓餅圖資料
		private List<ChartOrLineSourceByOrdProp> GenerateChart(List<P211501_WorkList> ordPropList, List<NameValuePair<string>> funcList)
		{
			var list = new List<ChartOrLineSourceByOrdProp>();
			List<NameValuePair<string>> doAndunDo = new List<NameValuePair<string>>();
			doAndunDo.Add(new NameValuePair<string>() { Name = Properties.Resources.DoneNumberOfJobToday, Value = "0" });
			doAndunDo.Add(new NameValuePair<string>() { Name = Properties.Resources.UndoneNumberOfJobToday.Replace("\r", "").Replace("\n", ""), Value = "1" });

			foreach (var ord in funcList)
			{
				var tmpdata = ordPropList.Where(o => o.FUNC_NAME == ord.Name).First();
				var chartSource = new ChartOrLineSourceByOrdProp
				{
					OrdProp = ord.Name,
					ChartSourceList = doAndunDo.Select(item => new ChartOrLineSource
					{
						DoAndUndo = item.Name,
						CustCount = item.Value == "0" ? Math.Abs(tmpdata.COUNTS_B - tmpdata.COUNTS_C) : tmpdata.COUNTS_C,
						CustTotalCount = item.Value == "0" ? Math.Abs(tmpdata.COUNTS_B - tmpdata.COUNTS_C) : tmpdata.COUNTS_C,
						TotalCount = tmpdata.COUNTS_B,
						Percent = item.Value == "0" ? Decimal.Round(Math.Abs(tmpdata.COUNTS_B - tmpdata.COUNTS_C) / ((tmpdata.COUNTS_B == 0) ? 1 : tmpdata.COUNTS_B), 2, MidpointRounding.AwayFromZero) : Decimal.Round(tmpdata.COUNTS_C / ((tmpdata.COUNTS_B == 0) ? 1 : tmpdata.COUNTS_B), 2, MidpointRounding.AwayFromZero),
						ListType = item.Name
					}).ToList(),

					VisEnable = tmpdata.VisEnable,
					heightValue = tmpdata.ROWNUM == 0 ? 150 : 0,
					widthValue = tmpdata.ROWNUM == 0 ? 150 : 0,
					FontSize = 15
				};

				list.Add(chartSource);
			}
			return list.OrderBy(o => o.Rownum).ToList();
		}

		#endregion


		#region 圓餅圖資料結構
		public class ChartOrLineSourceByOrdProp
		{
			public string OrdProp { get; set; }
			public decimal Rownum { get; set; }
			public List<ChartOrLineSource> ChartSourceList { get; set; }
			public Visibility VisEnable { get; set; }
			public int widthValue { get; set; }
			public int heightValue { get; set; }
			public int FontSize { get; set; }
		}

		public class ChartOrLineSource
		{
			public string Label { get { return string.Format("{0} \r\n {1}", CustTotalCount, Percent.ToString("P0")); } }
			public string DoAndUndo { get; set; }
			public decimal Percent { get; set; }
			public decimal CustCount { get; set; }
			public decimal CustTotalCount { get; set; }
			public decimal TotalCount { get; set; }
			public string ListType { get; set; }

		}
		#endregion

		#endregion Search


		#region RefreshSchedule
		/// <summary>
		/// Gets the RefreshSchedule.
		/// </summary>
		public ICommand RefreshScheduleCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoRefreshSchedule(), () => UserOperateMode == OperateMode.Query
);
			}
		}

		private void DoRefreshSchedule()
		{
			var list = new List<string>();
			foreach (var item in SubjectList)
			{
				if (item.IsSelected)
					list.Add(item.Item.Value);
			}
			ScheduleList = AllScheduleList.Where(x => list.Contains(x.SUBJECT)).ToList();
		}
		#endregion RefreshSchedule

		#endregion

	}
}
