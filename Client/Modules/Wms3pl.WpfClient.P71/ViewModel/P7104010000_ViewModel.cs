using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F70DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7104010000_ViewModel : InputViewModelBase
	{
		public P7104010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				SetDcList();
				//初始化執行時所需的值及資料
			}
		}

		#region Property

		public Timer TimerCounterByAll;
		public BackgroundWorker BgWorkerByAll;

		#region 物流中心
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				if (_dcList == value)
					return;
				Set(() => DcList, ref _dcList, value);
			}
		}

		private string _selectedDcCode;

		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				if (_selectedDcCode == value)
					return;
				Set(() => SelectedDcCode, ref _selectedDcCode, value);
				if (TimerCounterByAll != null && BgWorkerByAll != null)
				{
					TimerCounterByAll.Stop();
					BgWorkerByAll.CancelAsync();
					TimerCounterByAll.Start();
					BgWorkerByAll.RunWorkerAsync();
				}
			}
		}

		#endregion


		#region 進倉單資料來源
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

		#region 退貨單資料來源
		private List<ChartOrLineSourceByOrdProp> _chartOrLineSourceByOrdPropListR;

		public List<ChartOrLineSourceByOrdProp> ChartOrLineSourceByOrdPropListR
		{
			get { return _chartOrLineSourceByOrdPropListR; }
			set
			{
				if (_chartOrLineSourceByOrdPropListR == value)
					return;
				Set(() => ChartOrLineSourceByOrdPropListR, ref _chartOrLineSourceByOrdPropListR, value);
			}
		}
		#endregion

		#region 調撥單資料來源
		private List<ChartOrLineSourceByOrdProp> _chartOrLineSourceByOrdPropListT;

		public List<ChartOrLineSourceByOrdProp> ChartOrLineSourceByOrdPropListT
		{
			get { return _chartOrLineSourceByOrdPropListT; }
			set
			{
				if (_chartOrLineSourceByOrdPropListT == value)
					return;
				Set(() => ChartOrLineSourceByOrdPropListT, ref _chartOrLineSourceByOrdPropListT, value);
			}
		}
		#endregion

		#region 出貨單資料來源
		private List<ChartOrLineSourceByOrdProp> _chartOrLineSourceByOrdPropListO;

		public List<ChartOrLineSourceByOrdProp> ChartOrLineSourceByOrdPropListO
		{
			get { return _chartOrLineSourceByOrdPropListO; }
			set
			{
				if (_chartOrLineSourceByOrdPropListO == value)
					return;
				Set(() => ChartOrLineSourceByOrdPropListO, ref _chartOrLineSourceByOrdPropListO, value);
			}
		}
		#endregion

		#region 物流中心上班人數
		private int _dcWorkPersonCount;

		public int DcWorkPersonCount
		{
			get { return _dcWorkPersonCount; }
			set
			{
				if (_dcWorkPersonCount == value)
					return;
				Set(() => DcWorkPersonCount, ref _dcWorkPersonCount, value);
			}
		}
		#endregion

		#region 行事曆
		private List<F700501> _f700501List;

		public List<F700501> F700501List
		{
			get { return _f700501List; }
			set
			{
				if (_f700501List == value)
					return;
				Set(() => F700501List, ref _f700501List, value);
			}
		}


		#region
		private F700501 _selectedF700501;

		public F700501 SelectedF700501
		{
			get { return _selectedF700501; }
			set
			{
				if (_selectedF700501 == value)
					return;
				Set(() => SelectedF700501, ref _selectedF700501, value);
			}
		}
		#endregion

		#endregion


		#endregion

		#region 下拉選單資料繫結

		private void SetDcList()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
				SelectedDcCode = DcList.First().Value;
		}

		#endregion

		#region Method

		public void Ini()
		{
			#region 資料更新計時
			BgWorkerByAll = new BackgroundWorker { WorkerSupportsCancellation = true };
			BgWorkerByAll.DoWork += BgWorkerByAllOnDoWork;
			TimerCounterByAll = new Timer { Interval = 600000 };
			TimerCounterByAll.Elapsed += TimerCounterByAllOnElapsed;
			TimerCounterByAll.Start();
			BgWorkerByAll.RunWorkerAsync();
			#endregion

		}

		#region 資料背景程式
		private void TimerCounterByAllOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			if (!BgWorkerByAll.IsBusy && !BgWorkerByAll.CancellationPending)
				BgWorkerByAll.RunWorkerAsync();
		}

		private void RecalculatePercent(ChartOrLineSourceByOrdProp chartSource)
		{
			foreach (var chart in chartSource.ChartSourceList)
				chart.Percent = Decimal.Round(chart.CustTotalCount / ((chart.TotalCount == 0) ?1 :chart.TotalCount), 2, MidpointRounding.AwayFromZero);
			var sumPercent = chartSource.ChartSourceList.Sum(o => o.Percent);
			if (sumPercent > 0)
			{
				var diffPercent = 1 - sumPercent;
				if (diffPercent != 0) //百分比有差異
				{
					if (diffPercent > 0) //差異大於0 加給最小的百分比
						chartSource.ChartSourceList.OrderBy(o => o.Percent).First().Percent += diffPercent;
					else //差異小於0 扣除最大的百分比
						chartSource.ChartSourceList.OrderByDescending(o => o.Percent).First().Percent += diffPercent;
				}
			}
		}

		private List<ChartOrLineSourceByOrdProp> GenerateChart(IEnumerable<NameValuePair<string>> ordPropList,List<F1909> custList, List<DcWmsNoOrdPropItem> dcWmsNoOrdPropItemList)
		{
			var list = new List<ChartOrLineSourceByOrdProp>();
			foreach (var nameValuePair in ordPropList)
			{
				var chartSource = new ChartOrLineSourceByOrdProp
				{
					OrdProp = nameValuePair.Name,
					ChartSourceList = custList.Select(item => new ChartOrLineSource
					{
						CustName = item.CUST_NAME,
						CustFinishCount = dcWmsNoOrdPropItemList.Where(o => o.ORD_PROP == nameValuePair.Value && o.CUST_CODE == item.CUST_CODE).Select(o => o.CUST_FINISHCOUNT).Sum(),
						CustTotalCount = dcWmsNoOrdPropItemList.Where(o=>o.ORD_PROP == nameValuePair.Value && o.CUST_CODE == item.CUST_CODE).Select(o=>o.CUST_TOTALCOUNT).Sum(),
						TotalCount = dcWmsNoOrdPropItemList.Where(o => o.ORD_PROP == nameValuePair.Value).Select(o => o.CUST_TOTALCOUNT).Sum()
					}).ToList()
				};
				RecalculatePercent(chartSource);
				list.Add(chartSource);
			}
			return list;
		}

		private void BgWorkerByAllOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
		{
			var proxy00 = GetProxy<F00Entities>();
			var ordPropList = proxy00.F000903s.Where(
				o => o.ORD_PROP.Contains("A") || o.ORD_PROP.Contains("R") || o.ORD_PROP.Contains("O"))
				.ToList();

			var proxy19 = GetProxy<F19Entities>();
			var custList = proxy19.CreateQuery<F1909>("GetF1909ByDc")
				.AddQueryExOption("dcCode", SelectedDcCode).ToList();

			var proxyEx = GetExProxy<P71ExDataSource>();
			#region 進倉單

			var stockdata = proxyEx.CreateQuery<DcWmsNoOrdPropItem>("GeDcWmsNoOrdPropItemsByA")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("stockDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
			ChartOrLineSourceByOrdPropListA = GenerateChart(ordPropList.Where(o => o.ORD_PROP.Contains("A")).Select(item=>new NameValuePair<string>{Name = item.ORD_PROP_NAME,Value = item.ORD_PROP}).ToList(), custList,
				stockdata);
			#endregion
			#region 退貨單
			var returnData = proxyEx.CreateQuery<DcWmsNoOrdPropItem>("GeDcWmsNoOrdPropItemsByR")
			.AddQueryExOption("dcCode", SelectedDcCode)
			.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
			ChartOrLineSourceByOrdPropListR = GenerateChart(ordPropList.Where(o => o.ORD_PROP.Contains("R")).Select(item => new NameValuePair<string> { Name = item.ORD_PROP_NAME, Value = item.ORD_PROP }).ToList(), custList,
				returnData);
			#endregion
			#region 調撥單
			var allocationData = proxyEx.CreateQuery<DcWmsNoOrdPropItem>("GeDcWmsNoOrdPropItemsByT")
			.AddQueryExOption("dcCode", SelectedDcCode)
			.AddQueryExOption("allocationDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
			ChartOrLineSourceByOrdPropListT = GenerateChart(new List<NameValuePair<string>>{new NameValuePair<string>{Name = Properties.Resources.P7104010000_ViewModel_Transfer,Value = null}}, custList,
				allocationData);
			#endregion
			#region 出貨單
			var wmsData = proxyEx.CreateQuery<DcWmsNoOrdPropItem>("GeDcWmsNoOrdPropItemsByO")
			.AddQueryExOption("dcCode", SelectedDcCode)
			.AddQueryExOption("delvDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
			ChartOrLineSourceByOrdPropListO = GenerateChart(ordPropList.Where(o => o.ORD_PROP.Contains("O")).Select(item => new NameValuePair<string> { Name = item.ORD_PROP_NAME, Value = item.ORD_PROP }).ToList(), custList,
				wmsData);
			#endregion


			var proxyF70 = GetProxy<F70Entities>();
			#region DC上班人數
			DcWorkPersonCount = proxyF70.F700701s.Where(o => o.DC_CODE == SelectedDcCode && o.IMPORT_DATE == DateTime.Today).ToList().Sum(o => o.PERSON_NUMBER);
			#endregion

			#region 行事曆
			F700501List = proxyF70.F700501s.Where(o => o.DC_CODE == SelectedDcCode && o.SCHEDULE_DATE == DateTime.Today).ToList();
			if (F700501List.Any())
				SelectedF700501 = F700501List.First();
			#endregion
		}
		#endregion

		#endregion
	}
	#region 圓餅圖資料結構
	public class ChartOrLineSourceByOrdProp
	{
		public string OrdProp { get; set; }
		public List<ChartOrLineSource> ChartSourceList { get; set; }
	}

	public class ChartOrLineSource
	{
		public string Label { get { return string.Format("{0}\r\n  {1} {2}/{3}", CustName, Percent.ToString("P0"), CustTotalCount, TotalCount); } }
		public string CustName { get; set; }

		public decimal Percent { get; set; }
		public decimal CustFinishCount { get; set; }
		public decimal CustTotalCount { get; set; }
		public decimal TotalCount { get; set; }
	}

	#endregion


}
