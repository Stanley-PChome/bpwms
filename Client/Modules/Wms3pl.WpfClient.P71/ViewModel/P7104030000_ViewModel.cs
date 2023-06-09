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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7104030000_ViewModel : InputViewModelBase
	{
		public P7104030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				NowTime = DateTime.Now;

			}

		}

		#region Property
		public Timer TimerCounterByNowTime;
		public Timer TimerCounterByAll;
		public BackgroundWorker BgWorkerByNowTime;
		public BackgroundWorker BgWorkerByAll;
		private string _ordType;

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

		private string  _selectedDcCode;

		public string  SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				if (_selectedDcCode == value)
					return;
				Set(() => SelectedDcCode, ref _selectedDcCode, value);
				if (TimerCounterByAll != null && BgWorkerByAll!=null)
				{
					TimerCounterByAll.Stop();
					BgWorkerByAll.CancelAsync();
					TimerCounterByAll.Start();
					BgWorkerByAll.RunWorkerAsync();
				}
			}
		}
		
		#endregion

		#region 進度條清單
		private List<LineBarInfo> _lineBarInfoList;

		public List<LineBarInfo> LineBarInfoList
		{
			get { return _lineBarInfoList; }
			set
			{
				if (_lineBarInfoList == value)
					return;
				Set(() => LineBarInfoList, ref _lineBarInfoList, value);
			}
		}
		#endregion

		#region 目前時間
		private DateTime _nowTime;

		public DateTime NowTime
		{
			get { return _nowTime; }
			set
			{
				if (_nowTime == value)
					return;
				Set(() => NowTime, ref _nowTime, value);
			}
		}
		#endregion

		#region Grid -驗收處理中-超過30分鐘未完成 -退貨處理中-超過30分鐘未完成 -加工處理中-超過預計完成時間
		private List<DcWmsNoStatusItem> _receProcessList;

		public List<DcWmsNoStatusItem> ReceProcessList
		{
			get { return _receProcessList; }
			set
			{
				if (_receProcessList == value)
					return;
				Set(() => ReceProcessList, ref _receProcessList, value);
			}
		}

		private DcWmsNoStatusItem _selectedReceProcess;

		public DcWmsNoStatusItem SelectedReceProcess
		{
			get { return _selectedReceProcess; }
			set
			{
				if (_selectedReceProcess == value)
					return;
				Set(() => SelectedReceProcess, ref _selectedReceProcess, value);
			}
		}

		#endregion

		#region Grid -驗收待上架-超過30分鐘未完成 、 -退貨待上架-超過30分鐘未處理
		private List<DcWmsNoStatusItem> _receUnUpLocList;

		public List<DcWmsNoStatusItem> ReceUnUpLocList
		{
			get { return _receUnUpLocList; }
			set
			{
				if (_receUnUpLocList == value)
					return;
				Set(() => ReceUnUpLocList, ref _receUnUpLocList, value);
			}
		}

		private DcWmsNoStatusItem _selectedReceUnUpLoc;

		public DcWmsNoStatusItem SelectedReceUnUpLoc
		{
			get { return _selectedReceUnUpLoc; }
			set
			{
				if (_selectedReceUnUpLoc == value)
					return;
				Set(() => SelectedReceUnUpLoc, ref _selectedReceUnUpLoc, value);
			}
		}

		#endregion

		#region Grid -緊急- 
		private List<DcWmsNoStatusItem> _helpList;

		public List<DcWmsNoStatusItem> HelpList
		{
			get { return _helpList; }
			set
			{
				if (_helpList == value)
					return;
				Set(() => HelpList, ref _helpList, value);
			}
		}

		private DcWmsNoStatusItem _selectedHelp;

		public DcWmsNoStatusItem SelectedHelp
		{
			get { return _selectedHelp; }
			set
			{
				if (_selectedHelp == value)
					return;
				Set(() => SelectedHelp, ref _selectedHelp, value);
			}
		}

		#endregion

		#region 進貨狀況控管

		#region 進倉品項數
		private int? _itemCntByA;

		public int? ItemCntByA
		{
			get { return _itemCntByA; }
			set
			{
				if (_itemCntByA == value)
					return;
				Set(() => ItemCntByA, ref _itemCntByA, value);
			}
		}
		#endregion

		#region 進貨總數量
		private int? _totalQtyByA;

		public int? TotalQtyByA
		{
			get { return _totalQtyByA; }
			set
			{
				if (_totalQtyByA == value)
					return;
				Set(() => TotalQtyByA, ref _totalQtyByA, value);
			}
		}
		#endregion

		#region 驗收總數量
		private int? _finishRecvQty;

		public int? FinishRecvQty
		{
			get { return _finishRecvQty; }
			set
			{
				if (_finishRecvQty == value)
					return;
				Set(() => FinishRecvQty, ref _finishRecvQty, value);
			}
		}
		#endregion

		#region 抽驗總數量
		private int? _finishCheckQty;

		public int? FinishCheckQty
		{
			get { return _finishCheckQty; }
			set
			{
				if (_finishCheckQty == value)
					return;
				Set(() => FinishCheckQty, ref _finishCheckQty, value);
			}
		}
		#endregion


	

		#endregion

		#region 退貨狀況控管

		#region 門退品項數
		private int? _itemCntByR2;

		public int? ItemCntByR2
		{
			get { return _itemCntByR2; }
			set
			{
				if (_itemCntByR2 == value)
					return;
				Set(() => ItemCntByR2, ref _itemCntByR2, value);
			}
		}
		#endregion

		#region 門退總量
		private int? _totalQtyByR2;

		public int? TotalQtyByR2
		{
			get { return _totalQtyByR2; }
			set
			{
				if (_totalQtyByR2 == value)
					return;
				Set(() => TotalQtyByR2, ref _totalQtyByR2, value);
			}
		}
		#endregion

		#region 客退品項數
		private int? _itemCntByR1;

		public int? ItemCntByR1
		{
			get { return _itemCntByR1; }
			set
			{
				if (_itemCntByR1 == value)
					return;
				Set(() => ItemCntByR1, ref _itemCntByR1, value);
			}
		}
		#endregion

		#region 客退總量
		private int? _totalQtyByR1;

		public int? TotalQtyByR1
		{
			get { return _totalQtyByR1; }
			set
			{
				if (_totalQtyByR1 == value)
					return;
				Set(() => TotalQtyByR1, ref _totalQtyByR1, value);
			}
		}
		#endregion

		#region R3換貨單退貨 R3
		private int? _itemCntByR3;

		public int? ItemCntByR3
		{
			get { return _itemCntByR3; }
			set
			{
				if (_itemCntByR3 == value)
					return;
				Set(() => ItemCntByR3, ref _itemCntByR3, value);
			}
		}
		#endregion

		#region R3換貨單退貨 R3
		private int? _totalQtyByR3;

		public int? TotalQtyByR3
		{
			get { return _totalQtyByR3; }
			set
			{
				if (_totalQtyByR3 == value)
					return;
				Set(() => TotalQtyByR3, ref _totalQtyByR3, value);
			}
		}
		#endregion

		#region W041RMA退貨 R4
		private int? _itemCntByR4;

		public int? ItemCntByR4
		{
			get { return _itemCntByR4; }
			set
			{
				if (_itemCntByR4 == value)
					return;
				Set(() => ItemCntByR4, ref _itemCntByR4, value);
			}
		}
		#endregion

		#region W041RMA退貨 R4
		private int? _totalQtyByR4;

		public int? TotalQtyByR4
		{
			get { return _totalQtyByR4; }
			set
			{
				if (TotalQtyByR4 == value)
					return;
				Set(() => TotalQtyByR4, ref _totalQtyByR4, value);
			}
		}
		#endregion

		#region W042RTO退貨 R5
		private int? _itemCntByR5;

		public int? ItemCntByR5
		{
			get { return _itemCntByR5; }
			set
			{
				if (TotalQtyByR5 == value)
					return;
				Set(() => ItemCntByR5, ref _itemCntByR5, value);
			}
		}
		#endregion

		#region W042RTO退貨 R5
		private int? _totalQtyByR5;

		public int? TotalQtyByR5
		{
			get { return _totalQtyByR5; }
			set
			{
				if (TotalQtyByR5 == value)
					return;
				Set(() => TotalQtyByR5, ref _totalQtyByR5, value);
			}
		}
		#endregion

		#region 七天鑑賞期退貨 R6
		private int? _itemCntByR6;

		public int? ItemCntByR6
		{
			get { return _itemCntByR6; }
			set
			{
				if (ItemCntByR6 == value)
					return;
				Set(() => ItemCntByR6, ref _itemCntByR6, value);
			}
		}
		#endregion

		#region 七天鑑賞期退貨 R6
		private int? _totalQtyByR6;

		public int? TotalQtyByR6
		{
			get { return _totalQtyByR6; }
			set
			{
				if (TotalQtyByR6 == value)
					return;
				Set(() => TotalQtyByR6, ref _totalQtyByR6, value);
			}
		}
		#endregion

		#region 出貨拒收退回 R7
		private int? _itemCntByR7;

		public int? ItemCntByR7
		{
			get { return _itemCntByR7; }
			set
			{
				if (ItemCntByR7 == value)
					return;
				Set(() => ItemCntByR7, ref _itemCntByR7, value);
			}
		}
		#endregion

		#region 出貨拒收退回 R7
		private int? _totalQtyByR7;

		public int? TotalQtyByR7
		{
			get { return _totalQtyByR7; }
			set
			{
				if (TotalQtyByR7 == value)
					return;
				Set(() => TotalQtyByR7, ref _totalQtyByR7, value);
			}
		}
		#endregion

		#region 其他退貨單 R8
		private int? _itemCntByR8;

		public int? ItemCntByR8
		{
			get { return _itemCntByR8; }
			set
			{
				if (ItemCntByR8 == value)
					return;
				Set(() => ItemCntByR8, ref _itemCntByR8, value);
			}
		}
		#endregion

		#region 其他退貨單 R8
		private int? _totalQtyByR8;

		public int? TotalQtyByR8
		{
			get { return _totalQtyByR8; }
			set
			{
				if (TotalQtyByR8 == value)
					return;
				Set(() => TotalQtyByR8, ref _totalQtyByR8, value);
			}
		}
		#endregion

		#region 派車取件作業 R9
		private int? _itemCntByR9;

		public int? ItemCntByR9
		{
			get { return _itemCntByR9; }
			set
			{
				if (ItemCntByR9 == value)
					return;
				Set(() => ItemCntByR9, ref _itemCntByR9, value);
			}
		}
		#endregion

		#region 派車取件作業 R9
		private int? _totalQtyByR9;

		public int? TotalQtyByR9
		{
			get { return _totalQtyByR9; }
			set
			{
				if (TotalQtyByR9 == value)
					return;
				Set(() => TotalQtyByR9, ref _totalQtyByR9, value);
			}
		}
		#endregion

		#endregion

		#region 加工狀況控管


		#region 加工單總量
		private int _countByW;

		public int CountByW
		{
			get { return _countByW; }
			set
			{
				if (_countByW == value)
					return;
				Set(() => CountByW, ref _countByW, value);
			}
		}
		#endregion

		#region 已完成數量
		private int _totalQtyByW;

		public int TotalQtyByW
		{
			get { return _totalQtyByW; }
			set
			{
				if (_totalQtyByW == value)
					return;
				Set(() => TotalQtyByW, ref _totalQtyByW, value);
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
		/// <summary>
		/// W=>加工	A=>進貨 R=>退貨
		/// </summary>
		/// <param name="ordType"></param>
		public void Ini(string ordType)
		{
			_ordType = ordType;

			#region 時間計時
			BgWorkerByNowTime = new BackgroundWorker { WorkerSupportsCancellation = true };
			BgWorkerByNowTime.DoWork += BgWorkerByNowTimeOnDoWork;
			TimerCounterByNowTime = new Timer { Interval = 1000 };
			TimerCounterByNowTime.Elapsed += TimerCounterByNowTimeOnElapsed;
			TimerCounterByNowTime.Start();
			#endregion
			#region 資料更新計時
			BgWorkerByAll = new BackgroundWorker { WorkerSupportsCancellation = true };
			BgWorkerByAll.DoWork += BgWorkerByAllOnDoWork;
			TimerCounterByAll = new Timer { Interval = 600000 };
			TimerCounterByAll.Elapsed += TimerCounterByAllOnElapsed;
			TimerCounterByAll.Start();
			BgWorkerByAll.RunWorkerAsync();
			#endregion

		}

		#region 時間背景程式
		private void TimerCounterByNowTimeOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			if (!BgWorkerByNowTime.IsBusy && !BgWorkerByNowTime.CancellationPending)
				BgWorkerByNowTime.RunWorkerAsync();
		}
		private void BgWorkerByNowTimeOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
		{
			NowTime = DateTime.Now;
		}
		#endregion
		#region 資料背景程式
		private void TimerCounterByAllOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			if (!BgWorkerByAll.IsBusy && !BgWorkerByAll.CancellationPending)
				BgWorkerByAll.RunWorkerAsync();
		}

		
		private void BgWorkerByAllOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
		{
			var proxy01 = GetProxy<F01Entities>();
			var proxy02 = GetProxy<F02Entities>();
			var proxy16 = GetProxy<F16Entities>();
			var proxy91 = GetProxy<F91Entities>();
			var proxyEx = GetExProxy<P71ExDataSource>();
			var lineBarInfoList = new List<LineBarInfo>();
			switch (_ordType)
			{
				case "A": //進貨控管
					//已進倉
					var inHouseCount =
						proxy01.F010201s.Where(o => o.DC_CODE == SelectedDcCode && o.STOCK_DATE == DateTime.Today && (o.STATUS == "1" || o.STATUS == "2" || o.STATUS == "3")).Count();
					//預計進倉
					var preInHouseCount = proxy01.F010201s.Where(o => o.DC_CODE == SelectedDcCode && o.STOCK_DATE == DateTime.Today && o.STATUS!="9").Count();
					//進貨驗收已完成
					var receCount = proxy01.F010201s.Where(o => o.DC_CODE == SelectedDcCode && o.STOCK_DATE == DateTime.Today && o.STATUS == "2").ToList().Select(o => o.STOCK_NO).Distinct().Count();
					//進貨驗收未完成
					var unReceCount = proxy01.F010201s.Where(o => o.DC_CODE == SelectedDcCode && o.STOCK_DATE == DateTime.Today && o.STATUS != "2").ToList().Select(o => o.STOCK_NO).Distinct().Count();
					
					var waitOrUpLocDatas = proxyEx.CreateQuery<F020201Data>("GetDatasByWaitOrUpLoc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("receDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//驗收完成已上架數
					var upCount = 0;
					//驗收完成待上架數
					var waitCount = 0;
					var purchaseNoList = waitOrUpLocDatas.Select(x => new { x.PURCHASE_NO, x.F010201_STATUS }).Distinct().ToList();
          foreach (var purchaseNo in purchaseNoList)
          {
              var totalcount = waitOrUpLocDatas.Where(x => x.PURCHASE_NO == purchaseNo.PURCHASE_NO).Count();
              var count = waitOrUpLocDatas.Where(x => x.PURCHASE_NO == purchaseNo.PURCHASE_NO && x.F151001_STATUS == "5").Count();
              if (totalcount == count && purchaseNo.F010201_STATUS == "2")
              {
                  upCount += 1;
              }
							else
							{
									waitCount += 1;
							}
          }
					//var upCount = waitOrUpLocDatas.Where(o => o.F151001_STATUS == "5").Select(o => o.PURCHASE_NO).Distinct().Count();
					//var waitCount = waitOrUpLocDatas.Where(o => o.F151001_STATUS != "5").Select(o => o.PURCHASE_NO).Distinct().Count();
					var f010202Datas = proxy01.CreateQuery<F010202>("GetDatasByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("stockDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//進倉品項數
					var itemCntByA = f010202Datas.Select(o => o.ITEM_CODE).Distinct().Count();
					//進倉總數量
					var itemQtyByA = f010202Datas.Sum(o => o.STOCK_QTY);
					var f20201List = proxy02.F020201s.Where(o => o.DC_CODE == SelectedDcCode && o.RECE_DATE == DateTime.Today).ToList();
					//已驗收數量
					var receQtyByA = f20201List.Sum(o => o.RECV_QTY);
					//已抽驗數量
					var checkQtyByA = f20201List.Sum(o => o.CHECK_QTY);
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_InStock_ExpectStock, FinishCount = inHouseCount, UnFinishCount = preInHouseCount });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_Check_Finish_NotYet, FinishCount = receCount, UnFinishCount = unReceCount });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_Check_OnSale_NotYet, FinishCount = upCount, UnFinishCount = waitCount });
					ItemCntByA = itemCntByA;
					TotalQtyByA = itemQtyByA;
					FinishRecvQty = receQtyByA;
					FinishCheckQty = checkQtyByA;
					//驗收處理中
					ReceProcessList = proxyEx.CreateQuery<DcWmsNoStatusItem>("GetReceProcessOver30MinDatasByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("receDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					if(ReceProcessList.Any())
						SelectedReceProcess = ReceProcessList.First();
					//驗收待上架
					ReceUnUpLocList = proxyEx.CreateQuery<DcWmsNoStatusItem>("GetReceUnUpLocOver30MinDatasByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("receDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					if (ReceUnUpLocList.Any())
						SelectedReceUnUpLoc = ReceUnUpLocList.First();
					break;
				case "R"://退貨控管
					var data = proxy16.F161201s.Where(o => o.DC_CODE == SelectedDcCode && o.RETURN_DATE == DateTime.Today && o.STATUS != "9" && (o.ORD_PROP =="R2" || o.ORD_PROP =="R1")).ToList();
					//退貨完成-已上架
					var upLocCount = proxy16.CreateQuery<F161601>("GetUpLocDataByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("rtnApplyDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList().Count;
					//退貨完成-未上架
					var waitUpLocCount = proxy16.CreateQuery<F161601>("GetWaitUpLocDataByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("rtnApplyDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList().Count;

					var datasByR2 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R2")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//門退品項數
					ItemCntByR2 = datasByR2.Select(o => o.ITEM_CODE).Distinct().Count();
					//門退總量
					TotalQtyByR2 = datasByR2.Sum(o => o.RTN_QTY);
					var datasByR1 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R1")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//客退品項數
					ItemCntByR1 = datasByR1.Select(o => o.ITEM_CODE).Distinct().Count();
					//客退總量
					TotalQtyByR1 = datasByR1.Sum(o => o.RTN_QTY);

					var datasByR3 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R3")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//R3換貨單退貨
					ItemCntByR3 = datasByR3.Select(o => o.ITEM_CODE).Distinct().Count();
					//R3換貨單退貨
					TotalQtyByR3 = datasByR3.Sum(o => o.RTN_QTY);

					var datasByR4 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R4")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//RMA退貨
					ItemCntByR4 = datasByR4.Select(o => o.ITEM_CODE).Distinct().Count();
					//RMA退貨
					TotalQtyByR4 = datasByR4.Sum(o => o.RTN_QTY);

					var datasByR5 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R5")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//RTO退貨
					ItemCntByR5 = datasByR5.Select(o => o.ITEM_CODE).Distinct().Count();
					//RTO退貨
					TotalQtyByR5 = datasByR5.Sum(o => o.RTN_QTY);

					var datasByR6 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R6")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//七天鑑賞期退貨
					ItemCntByR6 = datasByR6.Select(o => o.ITEM_CODE).Distinct().Count();
					//七天鑑賞期退貨
					TotalQtyByR6 = datasByR6.Sum(o => o.RTN_QTY);

					var datasByR7 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R7")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//出貨拒收退回
					ItemCntByR7 = datasByR7.Select(o => o.ITEM_CODE).Distinct().Count();
					//出貨拒收退回
					TotalQtyByR7 = datasByR7.Sum(o => o.RTN_QTY);

					var datasByR8 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R8")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//其他退貨單
					ItemCntByR8 = datasByR8.Select(o => o.ITEM_CODE).Distinct().Count();
					//其他退貨單
					TotalQtyByR8 = datasByR8.Sum(o => o.RTN_QTY);

					var datasByR9 = proxy16.CreateQuery<F161202>("GetF161202ByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("ordProp", "R9")
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					//派車取件作業
					ItemCntByR9 = datasByR9.Select(o => o.ITEM_CODE).Distinct().Count();
					//派車取件作業
					TotalQtyByR9 = datasByR9.Sum(o => o.RTN_QTY);

					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_RetailRTN, FinishCount = data.Count(o => o.ORD_PROP == "R2" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R2" && (o.STATUS == "0" || o.STATUS == "1")) });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_CustRTN, FinishCount = data.Count(o => o.ORD_PROP == "R1" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R1" && (o.STATUS == "0" || o.STATUS == "1")) });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_Change, FinishCount = data.Count(o => o.ORD_PROP == "R3" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R3" && (o.STATUS == "0" || o.STATUS == "1")) });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_RMA_RTN, FinishCount = data.Count(o => o.ORD_PROP == "R4" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R4" && (o.STATUS == "0" || o.STATUS == "1")) });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_RTO_RTN, FinishCount = data.Count(o => o.ORD_PROP == "R5" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R5" && (o.STATUS == "0" || o.STATUS == "1")) });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_SevenDaysRTN, FinishCount = data.Count(o => o.ORD_PROP == "R6" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R6" && (o.STATUS == "0" || o.STATUS == "1")) });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_RefuseRtn, FinishCount = data.Count(o => o.ORD_PROP == "R7" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R7" && (o.STATUS == "0" || o.STATUS == "1")) });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_Other_RTN_NO, FinishCount = data.Count(o => o.ORD_PROP == "R8" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R8" && (o.STATUS == "0" || o.STATUS == "1")) });
					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_DELV_PickUp, FinishCount = data.Count(o => o.ORD_PROP == "R9" && o.STATUS == "2"), UnFinishCount = data.Count(o => o.ORD_PROP == "R9" && (o.STATUS == "0" || o.STATUS == "1")) });
					

					lineBarInfoList.Add(new LineBarInfo { Name = Properties.Resources.P7104030000_ViewModel_RTN_Complete, FinishCount = upLocCount, UnFinishCount = waitUpLocCount });
					ReceProcessList = proxyEx.CreateQuery<DcWmsNoStatusItem>("GetReturnProcessOver30MinByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					if(ReceProcessList.Any())
						SelectedReceProcess = ReceProcessList.First();
					ReceUnUpLocList = proxyEx.CreateQuery<DcWmsNoStatusItem>("GetReturnWaitUpLocOver30MinByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("rtnApplyDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					if (ReceUnUpLocList.Any())
						SelectedReceUnUpLoc = ReceUnUpLocList.First();
					HelpList = proxyEx.CreateQuery<DcWmsNoStatusItem>("GetReturnNoHelpByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("returnDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					if (HelpList.Any())
						SelectedHelp = HelpList.First();
					break;
				case "W"://流通加工控管
					//取得加工線資料
					var produceList = proxy91.F910004s.Where(o => o.DC_CODE == SelectedDcCode && o.STATUS == "0").ToList();
					//取得個加工線 未完成 已完成 加工單數
					var produceLineStatusList = proxyEx.CreateQuery<ProduceLineStatusItem>("GetProduceLineStatusItems")
					.AddQueryExOption("dcCode", SelectedDcCode)
					.AddQueryExOption("finishDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					lineBarInfoList.AddRange(from f910004 in produceList let item = produceLineStatusList.FirstOrDefault(o => o.PRODUCE_NO == f910004.PRODUCE_NO) select new LineBarInfo {Name = string.Format(Properties.Resources.P7104030000_ViewModel_DONE_NOTYET, f910004.PRODUCE_NAME), FinishCount = (item == null) ? 0 : item.FINISHCOUNT, UnFinishCount = (item == null) ? 0 : item.UNFINISHCOUNT});
					//加工單總量
					CountByW = proxy91.F910201s.Where(o=>o.DC_CODE == SelectedDcCode && o.FINISH_DATE == DateTime.Today && o.STATUS!="9").ToList().Count;
					//已完成數量
					TotalQtyByW = proxy91.F910201s.Where(o=>o.DC_CODE == SelectedDcCode && o.FINISH_DATE == DateTime.Today && o.STATUS =="2").ToList().Count;
					ReceProcessList = proxyEx.CreateQuery<DcWmsNoStatusItem>("GetWorkProcessOverFinishTimeByDc")
						.AddQueryExOption("dcCode", SelectedDcCode)
						.AddQueryExOption("finishDate", DateTime.Today.ToString("yyyy/MM/dd")).ToList();
					if(ReceProcessList.Any())
						SelectedReceProcess = ReceProcessList.First();
					break;
			}
			LineBarInfoList = lineBarInfoList.ToList();
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
			//執行查詢動
		}
		#endregion Search

	}
	

	public class LineBarInfo
	{
		public string Name { get; set; }
		public int FinishCount { get; set; }

		public int UnFinishCount { get; set; }

		public int TotalCount { get { return FinishCount + UnFinishCount; } }
	}

    public class PurchaseNoInfo
    {
        public string PURCHASE_NO { get; set; }
        public string STATUS { get; set; }
    }
	
}
