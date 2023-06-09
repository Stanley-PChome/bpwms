using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Telerik.Windows.Controls.Charting;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7104020000_ViewModel : InputViewModelBase
	{
		public P7104020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				SetDcList();
				//初始化執行時所需的值及資料
				CustName = Wms3plSession.Get<GlobalInfo>().CustName;
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

		#region 業主

		public string GupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		#endregion

		#region 貨主

		public string CustCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		#endregion

		#region 貨主名稱
		private string _custName;

		public string CustName
		{
			get { return _custName; }
			set
			{
				if (_custName == value)
					return;
				Set(() => CustName, ref _custName, value);
			}
		}
		#endregion


		#region 進倉單折線圖資料
		private List<SomeInfo> _lineChartDataByAList;

		public List<SomeInfo> LineChartDataByAList
		{
			get { return _lineChartDataByAList; }
			set
			{
				if (_lineChartDataByAList == value)
					return;
				Set(() => LineChartDataByAList, ref _lineChartDataByAList, value);
			}
		}
		#endregion

		#region 客戶訂單折線圖資料
		private List<SomeInfo> _lineChartDataBySList;

		public List<SomeInfo> LineChartDataBySList
		{
			get { return _lineChartDataBySList; }
			set
			{
				if (_lineChartDataBySList == value)
					return;
				Set(() => LineChartDataBySList, ref _lineChartDataBySList, value);
			}
		}
		#endregion

		#region 退貨單折線圖資料
		private List<SomeInfo> _lineChartDataByRList;

		public List<SomeInfo> LineChartDataByRList
		{
			get { return _lineChartDataByRList; }
			set
			{
				if (_lineChartDataByRList == value)
					return;
				Set(() => LineChartDataByRList, ref _lineChartDataByRList, value);
			}
		}
		#endregion

		#region 加工單折線圖資料
		private List<SomeInfo> _lineChartDataByWList;

		public List<SomeInfo> LineChartDataByWList
		{
			get { return _lineChartDataByWList; }
			set
			{
				if (_lineChartDataByWList == value)
					return;
				Set(() => LineChartDataByWList, ref _lineChartDataByWList, value);
			}
		}
		#endregion


		#region
		private List<ChartSourceByLocType> _chartSourceByLocTypeList;

		public List<ChartSourceByLocType> ChartSourceByLocTypeList
		{
			get { return _chartSourceByLocTypeList; }
			set
			{
				if (_chartSourceByLocTypeList == value)
					return;
				Set(() => ChartSourceByLocTypeList, ref _chartSourceByLocTypeList, value);
			}
		}
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
			TimerCounterByAll = new Timer { Interval = 3600000 };
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

		private void RecalculatePercent(ChartSourceByLocType chartSource)
		{
			foreach (var chart in chartSource.ChartSourcePercentList)
				chart.Percent = Decimal.Round(chart.LocCount / ((chart.TotalLocCount == 0) ? 1 : chart.TotalLocCount), 2, MidpointRounding.AwayFromZero);
			var sumPercent = chartSource.ChartSourcePercentList.Sum(o => o.Percent);
			if (sumPercent > 0)
			{
				var diffPercent = 1 - sumPercent;
				if (diffPercent != 0) //百分比有差異
				{
					if (diffPercent > 0) //差異大於0 加給最小的百分比
						chartSource.ChartSourcePercentList.OrderBy(o => o.Percent).First().Percent += diffPercent;
					else //差異小於0 扣除最大的百分比
						chartSource.ChartSourcePercentList.OrderByDescending(o => o.Percent).First().Percent += diffPercent;
				}
			}
		}

		private List<ChartSourceByLocType> GenerateChart(IEnumerable<NameValuePair<string>> locTypeList,List<DcWmsNoLocTypeItem> dcWmsNoLocTypeItems)
		{
			var list = new List<ChartSourceByLocType>();
			foreach (var nameValuePair in locTypeList)
			{
				var chartSource = new ChartSourceByLocType
				{
					LocTypeName = nameValuePair.Name,
					ChartSourcePercentList = new List<ChartSourcePercent>()
				};
				var item = dcWmsNoLocTypeItems.FirstOrDefault(o => o.LOC_TYPE_ID == nameValuePair.Value);
				if (item == null)
				{
					chartSource.ChartSourcePercentList.Add(new ChartSourcePercent { Label = Properties.Resources.P7104020000_ViewModel_Used, LocCount = 0, Percent = 0, TotalLocCount = 0 });
					chartSource.ChartSourcePercentList.Add(new ChartSourcePercent { Label = Properties.Resources.P7104020000_ViewModel_Avaliable, LocCount = 0, Percent = 0, TotalLocCount = 0 });
				}
				else
				{
					chartSource.ChartSourcePercentList.Add(new ChartSourcePercent { Label = Properties.Resources.P7104020000_ViewModel_Used, LocCount = item.USEDLOCCOUNT, Percent = Decimal.Round(item.USEDLOCCOUNT / ((item.TOTALLOCCOUNT == 0) ? 1 : item.TOTALLOCCOUNT), 2, MidpointRounding.AwayFromZero), TotalLocCount = item.TOTALLOCCOUNT });
					chartSource.ChartSourcePercentList.Add(new ChartSourcePercent { Label = Properties.Resources.P7104020000_ViewModel_Avaliable, LocCount = 0, Percent = Decimal.Round(item.UNUSEDLOCCOUNT / ((item.TOTALLOCCOUNT == 0) ? 1 : item.TOTALLOCCOUNT), 2, MidpointRounding.AwayFromZero), TotalLocCount = item.TOTALLOCCOUNT });
				}
				RecalculatePercent(chartSource);
				list.Add(chartSource);
			}
			return list;
		}

		private List<SomeInfo> GenerateLineChart(List<DcWmsNoDateItem> data)
		{
			var lstData = new List<SomeInfo>();
			var now = DateTime.Now.Date;
			for (DateTime dtime = now.AddDays(-6); dtime <= now; dtime = dtime.AddDays(1))
				lstData.Add(new SomeInfo() { Label = dtime.Date.ToString("MM/dd"), Value = data.Where(o => o.WmsDate == dtime).Sum(o => o.WmsCount) });
			return lstData;
		}
		private void BgWorkerByAllOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
		{
			var now = DateTime.Now.Date;
			//進倉單折線圖
			var proxyEx = GetExProxy<P71ExDataSource>();
			var dataA = proxyEx.CreateQuery<DcWmsNoDateItem>("GetDcWmsNoDateItemsByA")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("begStockDate", now.AddDays(-6).ToString("yyyy/MM/dd"))
				.AddQueryExOption("endStockDate", now.ToString("yyyy/MM/dd")).ToList();
			LineChartDataByAList = GenerateLineChart(dataA);
			//客戶訂單折線圖
			var dataS = proxyEx.CreateQuery<DcWmsNoDateItem>("GetDcWmsNoDateItemsByS")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("begOrdDate", now.AddDays(-6).ToString("yyyy/MM/dd"))
				.AddQueryExOption("endOrdDate", now.ToString("yyyy/MM/dd")).ToList();
			LineChartDataBySList = GenerateLineChart(dataS);
			//退貨單折線圖
			var dataR = proxyEx.CreateQuery<DcWmsNoDateItem>("GetDcWmsNoDateItemsByR")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("begReturnDate", now.AddDays(-6).ToString("yyyy/MM/dd"))
				.AddQueryExOption("endReturnDate", now.ToString("yyyy/MM/dd")).ToList();
			LineChartDataByRList = GenerateLineChart(dataR);
			//加工單折線圖
			var dataW = proxyEx.CreateQuery<DcWmsNoDateItem>("GetDcWmsNoDateItemsByW")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("begFinishDate", now.AddDays(-6).ToString("yyyy/MM/dd"))
				.AddQueryExOption("endFinishDate", now.ToString("yyyy/MM/dd")).ToList();
			LineChartDataByWList = GenerateLineChart(dataW);

			var proxyF19 = GetProxy<F19Entities>();
			var locTypeDatas = proxyF19.F1942s.ToList().Select(item => new NameValuePair<string>{Name = item.LOC_TYPE_NAME,Value = item.LOC_TYPE_ID	});
			var dataLoc = proxyEx.CreateQuery<DcWmsNoLocTypeItem>("GetDcWmsNoLocTypeItems")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode).ToList();
			ChartSourceByLocTypeList = GenerateChart(locTypeDatas, dataLoc);
		}
		#endregion

		#endregion

	}
	#region 圓餅圖資料結構
	public class ChartSourceByLocType
	{
		public string LocTypeName { get; set; }
		public List<ChartSourcePercent> ChartSourcePercentList { get; set; }
	}

	public class ChartSourcePercent
	{
		public string Label { get; set; }
		public decimal Percent { get; set; }
		public string PercentLabel { get { return string.Format("{0}\r\n%", (Percent*100).ToString("N0")); } }
		public decimal LocCount { get; set; }
		public decimal TotalLocCount { get; set; }

	}
	#endregion

	public class SomeInfo
	{
		public double Value { get; set; }
		public string Label { get; set; }

		public string LegendName { get; set; }

	}
}
