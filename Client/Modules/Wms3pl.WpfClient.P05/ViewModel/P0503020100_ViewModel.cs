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
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0503020100_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		public Action ExitClick = delegate { };
		private string _userId;
		private string _userName;

		private string _datacount;
		public string DATA_COUNT
		{
			get { return _datacount; }
			set { _datacount = value; RaisePropertyChanged("DATA_COUNT"); }
		}
		private string _gupcode;
		public string GUP_CODE
		{
			get { return _gupcode; }
			set { _gupcode = value; RaisePropertyChanged("GUP_CODE"); }
		}
		private string _custcode;
		public string CUST_CODE
		{
			get { return _custcode; }
			set { _custcode = value; RaisePropertyChanged("CUST_CODE"); }
		}
		private string _dccode;
		public string DC_CODE
		{
			get { return _dccode; }
			set { _dccode = value; RaisePropertyChanged("DC_CODE"); }
		}
		private string _wmsordno;
		public string WMS_ORD_NO
		{
			get { return _wmsordno; }
			set { _wmsordno = value; RaisePropertyChanged("WMS_ORD_NO"); }
		}

		#region 基本資料
		private P05030201BasicData _basicdata;
		public P05030201BasicData BasicData
		{
			get { return _basicdata; }
			set
			{
				_basicdata = value;
				RaisePropertyChanged("BasicData");
			}
		}
		#endregion

		#region 物流中心
		private List<NameValuePair<string>> _dclist;
		public List<NameValuePair<string>> DC_LIST
		{
			get { return _dclist; }
			set
			{
				_dclist = value;
				RaisePropertyChanged("DCLIST");
			}
		}
		public void GET_DC_LIST()
		{
			DC_LIST = Wms3plSession.Get<GlobalInfo>().DcCodeList;
		}
		#endregion

		#region STATUS_LIST
		private List<NameValuePair<string>> _statuslist;
		public List<NameValuePair<string>> STATUS_LIST
		{
			get { return _statuslist; }
			set
			{
				_statuslist = value;
				RaisePropertyChanged("STATUS_LIST");
			}
		}
		public void GET_STATUS_LIST()
		{
			STATUS_LIST = GetBaseTableService.GetF000904List(FunctionCode, "P050302", "STATUS");
		}
		#endregion

		#region 出貨明細
		private ObservableCollection<F050102WithF050801> _goodslist;
		public ObservableCollection<F050102WithF050801> GoodsList
		{
			get { return _goodslist; }
			set
			{
				_goodslist = value;
				RaisePropertyChanged("GoodsList");
			}
		}
		#endregion

		#region 包裝序號刷驗記錄
		private ObservableCollection<F05500101> _readserialnolist;
		public ObservableCollection<F05500101> ReadSerialNoList
		{
			get { return _readserialnolist; }
			set
			{
				_readserialnolist = value;
				RaisePropertyChanged("ReadSerialNoList");
			}
		}
		#endregion

		#region 包裝列印表單
		private ObservableCollection<PrintListClass> _printlist;
		public ObservableCollection<PrintListClass> PrintList
		{
			get { return _printlist; }
			set
			{
				_printlist = value;
				RaisePropertyChanged("PrintList");
			}
		}

		public class PrintListClass
		{
			public string DOC_NAME { get; set; }
			public string MEMO { get; set; }
		}
		#endregion

		#region LACK_DO_STATUS_LIST
		private List<NameValuePair<string>> _lackdostatuslist;
		public List<NameValuePair<string>> LACK_DO_STATUS_LIST
		{
			get { return _lackdostatuslist; }
			set
			{
				_lackdostatuslist = value;
				RaisePropertyChanged("LACK_DO_STATUS_LIST");
			}
		}
		public void GET_LACK_DO_STATUS_LIST()
		{
			LACK_DO_STATUS_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F051206", "RETURN_FLAG");
		}
		#endregion

		#region SOURCE_TYPE_LIST
		private List<NameValuePair<string>> _sourcetypelist;
		public List<NameValuePair<string>> SOURCE_TYPE_LIST
		{
			get { return _sourcetypelist; }
			set
			{
				_sourcetypelist = value;
				RaisePropertyChanged("SOURCE_TYPE_LIST");
			}
		}
		private void GET_SOURCE_TYPE_LIST()
		{
			var proxy = GetProxy<F00Entities>();
			SOURCE_TYPE_LIST = proxy.F000902s.Select(x => new NameValuePair<string>(x.SOURCE_NAME, x.SOURCE_TYPE)).ToList();
		}
		#endregion

		private List<NameValuePair<string>> _allList;
		/// <summary>
		/// 配速商清單
		/// </summary>
		public List<NameValuePair<string>> AllList
		{
			get
			{
				return _allList;
			}
			set
			{
				Set(() => AllList, ref _allList, value);
			}
		}

		void SetAllList()
		{
			var proxy = GetProxy<F19Entities>();
			AllList = proxy.F1947s.Where(x => x.DC_CODE == DC_CODE)
								  .Select(x => new NameValuePair<string>(x.ALL_COMP, x.ALL_ID))
								  .ToList();
		}


		private List<NameValuePair<string>> _distrCarStatus;

		public List<NameValuePair<string>> DistrCarStatus
		{
			get { return _distrCarStatus; }
			set
			{
				Set(() => DistrCarStatus, ref _distrCarStatus, value);
			}
		}

		void SetDistrCarStatus()
		{
			DistrCarStatus = GetBaseTableService.GetF000904List(FunctionCode, "F700101", "STATUS");
		}

		private List<NameValuePair<string>> _statusList;
		/// <summary>
		/// 用出貨單狀態 與 揀貨單狀態(+10)合併的 Sataus List
		/// </summary>
		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				Set(() => StatusList, ref _statusList, value);
			}
		}

		void SetStatusList()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "P05030201", "STATUS");
		}


		private List<F050901> _f050901List;

		public List<F050901> F050901List
		{
			get { return _f050901List; }
			set
			{
				Set(() => F050901List, ref _f050901List, value);
			}
		}

		void SetF050901List()
		{
			var proxy = GetProxy<F05Entities>();
			var query = from item in proxy.F050901s
						where item.WMS_NO == WMS_ORD_NO
						where item.DC_CODE == DC_CODE
						where item.GUP_CODE == GUP_CODE
						where item.CUST_CODE == CUST_CODE
						orderby item.CONSIGN_NO
						select new F050901
						{
							CONSIGN_NO = item.CONSIGN_NO,
							PAST_DATE = item.PAST_DATE,
							SEND_DATE = item.SEND_DATE,
							STATUS = item.STATUS
						};
			F050901List = query.ToList();
		}

		private List<NameValuePair<string>> _consignStatusList;

		public List<NameValuePair<string>> ConsignStatusList
		{
			get { return _consignStatusList; }
			set
			{
				Set(() => ConsignStatusList, ref _consignStatusList, value);
			}
		}

		void SetConsignStatusList()
		{
			ConsignStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F050901", "STATUS");
		}

		#endregion

		public P0503020100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				InitControls();
			}
		}
		private void InitControls()
		{
			GET_DC_LIST();
			GET_SOURCE_TYPE_LIST();
			GET_LACK_DO_STATUS_LIST();
			SetDistrCarStatus();
			SetStatusList();
			SetConsignStatusList();
		}

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

		#region Exit
		public ICommand ExitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoExit(),
					() => UserOperateMode == OperateMode.Query,
					o => DoExitCompleted()
					);
			}
		}

		private void DoExit()
		{
			//執行刪除動作
		}

		private void DoExitCompleted()
		{
			ExitClick();
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
			var proxyP05Ex = GetExProxy<P05ExDataSource>();
			//基本資料
			SetAllList();

			var item = proxyP05Ex.GetP05030201BasicData(GUP_CODE, CUST_CODE, DC_CODE, WMS_ORD_NO, null).ToList().FirstOrDefault();
			item.SOURCE_NO = proxyP05Ex.GetSourceNosByWmsOrdNo(GUP_CODE, CUST_CODE, DC_CODE, WMS_ORD_NO);
			BasicData = item;

			//出貨明細
			GoodsList = proxyP05Ex.CreateQuery<F050102WithF050801>("GetF050102WithF050801s")
				.AddQueryOption("gupCode", string.Format("'{0}'", GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", CUST_CODE))
				.AddQueryOption("dcCode", string.Format("'{0}'", DC_CODE))
				.AddQueryOption("wmsordno", string.Format("'{0}'", WMS_ORD_NO))
				.ToObservableCollection();
			DATA_COUNT = GoodsList.Count.ToString();

			//包裝序號刷驗紀錄 F05500101
			var proxyP05 = GetProxy<F05Entities>();
			ReadSerialNoList =
				proxyP05.F05500101s.Where(
					x => x.DC_CODE == DC_CODE && x.GUP_CODE == GUP_CODE && x.CUST_CODE == CUST_CODE && x.WMS_ORD_NO == WMS_ORD_NO)
					.OrderBy(o => o.CRT_DATE).ToObservableCollection();

			//包裝列印表單
			proxyP05 = GetProxy<F05Entities>();
			var data =
				proxyP05.F050801s.Where(
					x => x.DC_CODE == DC_CODE && x.GUP_CODE == GUP_CODE && x.CUST_CODE == CUST_CODE && x.WMS_ORD_NO == WMS_ORD_NO)
					.FirstOrDefault();

			//出貨單關聯訂單資料
			var f050301Datas = proxyP05.CreateQuery<F050301>("GetDataByWmsOrdNo")
				.AddQueryOption("dcCode", string.Format("'{0}'", DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", CUST_CODE))
				.AddQueryOption("wmsOrdNo", string.Format("'{0}'", WMS_ORD_NO))
				.ToList();

			SetF050901List();

			PrintList = new ObservableCollection<PrintListClass>();

			if (data.PRINT_PASS == "1")
				AddToPrintList(Properties.Resources.P0503020100_PastNo, "");
			if (f050301Datas.First().PRINT_RECEIPT == "1")
				AddToPrintList(Properties.Resources.P0503020100_Receipt, "");
			if (data.HELLO_LETTER == "1")
				AddToPrintList("Welcome Letter", "");
			if (data.PRINT_BOX == "1")
				AddToPrintList(Properties.Resources.P0503020100_Box, "");
			if (data.PRINT_DELV == "1")
				AddToPrintList(Properties.Resources.P0503020100_Delv, "");
			if (data.SELF_TAKE == "1")
				AddToPrintList(Properties.Resources.P0503020100_Take, "");
		}

		private void AddToPrintList(string docname, string memo)
		{
			PrintListClass a = new PrintListClass()
			{
				DOC_NAME = docname,
				MEMO = memo
			};
			PrintList.Add(a);
		}
		#endregion Search




	}
}
