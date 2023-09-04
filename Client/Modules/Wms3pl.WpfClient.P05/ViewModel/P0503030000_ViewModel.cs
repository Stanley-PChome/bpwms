using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0503030000_ViewModel : InputViewModelBase
	{
		private string _gupCode;
		private string _custCode;
		public P0503030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				InitControls();
			}

		}

		private void InitControls()
		{
			GetDcList();
			GetStatusList();

			GET_DC_LIST();
			GET_SOURCE_TYPE_LIST();
			GET_LACK_DO_STATUS_LIST();
			SetDistrCarStatus();
			SetStatusListForDelv();
			SetConsignStatusList();
            DelvDateBegin = DateTime.Today;
            DelvDateEnd = DateTime.Today;

        }

		#region 查詢條件
		private string _dcCode;
		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				if (_dcCode == value) return;
				Set(() => DcCode, ref _dcCode, value);
			}
		}

		private List<NameValuePair<string>> _dcList;
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				if (_dcList == value) return;
				Set(() => DcList, ref _dcList, value);
			}
		}

		private void GetDcList()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
				DcCode = DcList.First().Value;

		}

		private DateTime? _delvDateBegin;
		public DateTime? DelvDateBegin
		{
			get { return _delvDateBegin; }
			set
			{
				if (_delvDateBegin == value) return;
				Set(() => DelvDateBegin, ref _delvDateBegin, value);
			}
		}

		private DateTime? _delvDateEnd;
		public DateTime? DelvDateEnd
		{
			get { return _delvDateEnd; }
			set
			{
				if (_delvDateEnd == value) return;
				Set(() => DelvDateEnd, ref _delvDateEnd, value);
			}
		}

		private string _ordNo;
		public string OrdNo
		{
			get { return _ordNo; }
			set
			{
				if (_ordNo == value) return;
				Set(() => OrdNo, ref _ordNo, value);
			}
		}

		private string _custOrdNo;
		public string CustOrdNo
		{
			get { return _custOrdNo; }
			set
			{
				if (_custOrdNo == value) return;
				Set(() => CustOrdNo, ref _custOrdNo, value);
			}
		}

		private string _consignNo;

		public string ConsignNo
		{
			get { return _consignNo; }
			set
			{
				Set(() => ConsignNo, ref _consignNo, value);
			}
		}


		private string _wmsOrdNo;
		public string WmsOrdNo
		{
			get { return _wmsOrdNo; }
			set
			{
				if (_wmsOrdNo == value) return;
				Set(() => WmsOrdNo, ref _wmsOrdNo, value);
			}
		}

		private string _status;
		public string Status
		{
			get { return _status; }
			set
			{
				if (_status == value) return;
				Set(() => Status, ref _status, value);
			}
		}

		private List<NameValuePair<string>> _statusList;
		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				if (_statusList == value) return;
				Set(() => StatusList, ref _statusList, value);
			}
		}
		public void GetStatusList()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "P050303", "STATUS");
			StatusList.Insert(0, new NameValuePair<string>(Resources.Resources.All, ""));
			Status = StatusList.First().Value;
		}

        //品號
        private string _itemCode = string.Empty;

        public string ItemCode
        {
            get { return _itemCode; }
            set
            {
                _itemCode = value;
                RaisePropertyChanged("ItemCode");
            }
        }

        #endregion

        private ObservableCollection<P050303QueryItem> _mainList;
		public ObservableCollection<P050303QueryItem> MainList
		{
			get { return _mainList; }
			set
			{
				if (_mainList == value) return;
				Set(() => MainList, ref _mainList, value);
			}
		}

		private P050303QueryItem _selectedMainItem;
		public P050303QueryItem SelectedMainItem
		{
			get { return _selectedMainItem; }
			set
			{
				_selectedMainItem = value;
				if (_selectedMainItem != null)
					SetDelvDetail(_selectedMainItem.GUP_CODE, _selectedMainItem.CUST_CODE, _selectedMainItem.DC_CODE,
					  _selectedMainItem.WMS_ORD_NO,_selectedMainItem.ORD_NO);
				ShowDelvData = _selectedMainItem != null;
				RaisePropertyChanged("SelectedMainItem");
			}
		}

		private bool _showDelvData;
		/// <summary>
		/// 是否顯示出貨內容
		/// </summary>
		public bool ShowDelvData
		{
			get { return _showDelvData; }
			set
			{
				if (_showDelvData == value) return;
				Set(() => ShowDelvData, ref _showDelvData, value);
			}
		}

		private void ClearData()
		{
			SelectedMainItem = null;
			MainList = null;
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					c => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			if (DelvDateBegin == null && DelvDateEnd == null && string.IsNullOrWhiteSpace(OrdNo) &&
				string.IsNullOrWhiteSpace(CustOrdNo) && string.IsNullOrWhiteSpace(WmsOrdNo)
				&& string.IsNullOrWhiteSpace(ConsignNo))
			{
				DialogService.ShowMessage(Properties.Resources.P0503030000_ConsignNoIsNull);
				return;
			}

			ClearData();
			var proxyEx = GetExProxy<P05ExDataSource>();
			MainList = proxyEx.CreateQuery<P050303QueryItem>("GetP050303SearchData")
							  .AddQueryExOption("gupCode", _gupCode)
							  .AddQueryExOption("custCode", _custCode)
							  .AddQueryExOption("dcCode", DcCode)
							  .AddQueryExOption("delvDateBegin", DelvDateBegin)
							  .AddQueryExOption("delvDateEnd", DelvDateEnd)
							  .AddQueryExOption("ordNo", OrdNo)
							  .AddQueryExOption("custOrdNo", CustOrdNo)
							  .AddQueryExOption("status", Status)
							  .AddQueryExOption("wmsOrdNo", WmsOrdNo)
							  .AddQueryExOption("consignNo", ConsignNo)
                              .AddQueryExOption("itemCode", ItemCode)
                              .ToObservableCollection();
		}

		private void DoSearchComplete()
		{
			if (MainList == null || MainList.Count == 0)
				ShowMessage(Messages.InfoNoData);
			else
				SelectedMainItem = MainList.First();
		}
		#endregion Search

		#region 出貨內容

		#region 共用變數/資料連結/頁面參數

		private string _datacount;
		public string DATA_COUNT
		{
			get { return _datacount; }
			set { _datacount = value; RaisePropertyChanged("DATA_COUNT"); }
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

		#region 揀貨單資料
		private ObservableCollection<F051201WithF051202> _pickList;
		public ObservableCollection<F051201WithF051202> PickList
		{
			get { return _pickList; }
			set { Set(() => PickList, ref _pickList, value); }
		}
		#endregion

		#region 揀貨單明細資料
		private ObservableCollection<PickDetail> _pickDetailList;
		public ObservableCollection<PickDetail> PickDetailList
		{
			get { return _pickDetailList; }
			set { Set(() => PickDetailList, ref _pickDetailList, value); }
		}
    #endregion

    #region 揀貨完成容器資料
    private ObservableCollection<PickContainer> _PickContainer;
    public ObservableCollection<PickContainer> PickContainer
    {
      get { return _PickContainer; }
      set { Set(() => PickContainer, ref _PickContainer, value); }
    }
    #endregion 揀貨完成容器資料

    #region 訂單取消資訊
    private ObservableCollection<OrderCancelInfo> _OrderCancelInfo;
    public ObservableCollection<OrderCancelInfo> OrderCancelInfo
    {
      get { return _OrderCancelInfo; }
      set { Set(() => OrderCancelInfo, ref _OrderCancelInfo, value); }
    }
    #endregion 訂單取消資訊

    #region 分貨資訊
    private ObservableCollection<DivideInfo> _DivideInfo;
    public ObservableCollection<DivideInfo> DivideInfo
    {
      get { return _DivideInfo; }
      set { Set(() => DivideInfo, ref _DivideInfo, value); }
    }

    private ObservableCollection<DivideDetail> _DivideDetail;
    public ObservableCollection<DivideDetail> DivideDetail
    {
      get { return _DivideDetail; }
      set { Set(() => DivideDetail, ref _DivideDetail, value); }
    }
    #endregion 分貨資訊

    #region 集貨場進出紀錄
    private ObservableCollection<CollectionRecord> _CollectionRecord;
    public ObservableCollection<CollectionRecord> CollectionRecord
    {
      get { return _CollectionRecord; }
      set { Set(() => CollectionRecord, ref _CollectionRecord, value); }
    }
    #endregion 集貨場進出紀錄

    #region 託運單箱內明細
    private ObservableCollection<ConsignmentDetail> _ConsignmentDetail;
    public ObservableCollection<ConsignmentDetail> ConsignmentDetail
    {
      get { return _ConsignmentDetail; }
      set { Set(() => ConsignmentDetail, ref _ConsignmentDetail, value); }
    }
    #endregion 託運單箱內明細

    #region 出貨序號
    private ObservableCollection<F051202WithF055002> _outSerialNoList;
        public ObservableCollection<F051202WithF055002> OutSerialNoList
        {
            get { return _outSerialNoList; }
            set
            {
                _outSerialNoList = value;
                RaisePropertyChanged("OutSerialNoList");
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


		#region 操作刷驗記錄記錄
		private ObservableCollection<F055002WithGridLog> _readOperationlist;
		public ObservableCollection<F055002WithGridLog> ReadOperationlList
		{
			get { return _readOperationlist; }
			set
			{
				_readOperationlist = value;
				RaisePropertyChanged("ReadOperationlList");
			}
		}
		#endregion

		#region 包裝列印表單
		private List<PrintListClass> _printlist;
		public List<PrintListClass> PrintList
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

		void SetAllList(string dcCode)
		{
			var proxy = GetProxy<F19Entities>();
			AllList = proxy.F1947s.Where(x => x.DC_CODE == dcCode)
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

		private List<NameValuePair<string>> _statusListForDelv;
		/// <summary>
		/// 用出貨單狀態 與 揀貨單狀態(+10)合併的 Sataus List
		/// </summary>
		public List<NameValuePair<string>> StatusListForDelv
		{
			get { return _statusListForDelv; }
			set
			{
				Set(() => StatusListForDelv, ref _statusListForDelv, value);
			}
		}

		void SetStatusListForDelv()
		{
			StatusListForDelv = GetBaseTableService.GetF000904List(FunctionCode, "P05030201", "STATUS");
		}


		private ObservableCollection<F050901WithF055001> _consignmentNote;

		public ObservableCollection<F050901WithF055001> ConsignmentNote
		{
			get { return _consignmentNote; }
			set
			{
				Set(() => ConsignmentNote, ref _consignmentNote, value);
			}
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

		private void SetDelvDetail(string gupCode, string custCode, string dcCode, string wmsOrdNo,string ordNo)
		{
			var proxyP05Ex = GetExProxy<P05ExDataSource>();
			//基本資料
			SetAllList(dcCode);

			var item = proxyP05Ex.GetP05030201BasicData(gupCode, custCode, dcCode, wmsOrdNo,ordNo).ToList().FirstOrDefault();
			item.SOURCE_NO = proxyP05Ex.GetSourceNosByWmsOrdNo(gupCode, custCode, dcCode, wmsOrdNo);
			BasicData = item;

			//出貨明細
			GoodsList = proxyP05Ex.CreateQuery<F050102WithF050801>("GetF050102WithF050801s")
			  .AddQueryOption("gupCode", string.Format("'{0}'", gupCode))
			  .AddQueryOption("custCode", string.Format("'{0}'", custCode))
			  .AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
			  .AddQueryOption("wmsordno", string.Format("'{0}'", wmsOrdNo))
			  .ToObservableCollection();
			DATA_COUNT = GoodsList.Count.ToString();

			//揀貨單資料
			PickList = proxyP05Ex.CreateQuery<F051201WithF051202>("GetF051201WithF051202s")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("wmsOrdNo", wmsOrdNo).ToObservableCollection();

			//揀貨單明細資料
			PickDetailList = proxyP05Ex.CreateQuery<PickDetail>("GetPickDetails")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("wmsOrdNo", wmsOrdNo).ToObservableCollection();

      //揀貨完成容器資料
      PickContainer = proxyP05Ex.CreateQuery<PickContainer>("GetPickContainer")
        .AddQueryExOption("dcCode", dcCode)
        .AddQueryExOption("gupCode", gupCode)
        .AddQueryExOption("custCode", custCode)
        .AddQueryExOption("wmsNo", wmsOrdNo).ToObservableCollection();

      //訂單取消資訊
      if (BasicData.STATUS == 29)
      {
        OrderCancelInfo = proxyP05Ex.CreateQuery<OrderCancelInfo>("GetOrderCancelInfoType1")
          .AddQueryExOption("dcCode", dcCode)
          .AddQueryExOption("gupCode", gupCode)
          .AddQueryExOption("custCode", custCode)
          .AddQueryExOption("pickOrdNo", string.Join(",", PickList.Select(o => o.PICK_ORD_NO))).ToObservableCollection();

        if (!OrderCancelInfo.Any())
        {
          OrderCancelInfo = proxyP05Ex.CreateQuery<OrderCancelInfo>("GetOrderCancelInfoType2")
            .AddQueryExOption("dcCode", dcCode)
            .AddQueryExOption("gupCode", gupCode)
            .AddQueryExOption("custCode", custCode)
            .AddQueryExOption("pickOrdNo", string.Join(",", PickList.Select(o => o.PICK_ORD_NO))).ToObservableCollection();
        }
      }
      else
      {
        OrderCancelInfo = null;
      }

      //分貨資訊
      DivideInfo = proxyP05Ex.CreateQuery<DivideInfo>("GetDivideInfo")
        .AddQueryExOption("dcCode", dcCode)
        .AddQueryExOption("gupCode", gupCode)
        .AddQueryExOption("custCode", custCode)
        .AddQueryExOption("wmsNo", wmsOrdNo).ToObservableCollection();

      //分貨明細資料
      DivideDetail = proxyP05Ex.CreateQuery<DivideDetail>("GetDivideDetail")
        .AddQueryExOption("dcCode", dcCode)
        .AddQueryExOption("gupCode", gupCode)
        .AddQueryExOption("custCode", custCode)
        .AddQueryExOption("wmsNo", wmsOrdNo).ToObservableCollection();

      //集貨場進出紀錄
      CollectionRecord = proxyP05Ex.CreateQuery<CollectionRecord>("GetCollectionRecord")
        .AddQueryExOption("dcCode", dcCode)
        .AddQueryExOption("gupCode", gupCode)
        .AddQueryExOption("custCode", custCode)
        .AddQueryExOption("wmsNo", wmsOrdNo).ToObservableCollection();

      //出貨序號
      OutSerialNoList = proxyP05Ex.CreateQuery<F051202WithF055002>("GetF051202WithF055002s")
               .AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
              .AddQueryOption("gupCode", string.Format("'{0}'", gupCode))
              .AddQueryOption("custCode", string.Format("'{0}'", custCode))
              .AddQueryOption("wmsordno", string.Format("'{0}'", wmsOrdNo)).ToObservableCollection();

      //包裝序號刷驗紀錄 F05500101
      var proxyP05 = GetProxy<F05Entities>();
			ReadSerialNoList =
			  proxyP05.F05500101s.Where(
				x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsOrdNo)
				.OrderBy(o => o.CRT_DATE).ThenBy(o => o.LOG_SEQ).ToObservableCollection();

			//操作刷驗記錄記錄 F055002
			ReadOperationlList = proxyP05Ex.CreateQuery<F055002WithGridLog>("GetF055002WithGridLog")				
			  .AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
			  .AddQueryOption("gupCode", string.Format("'{0}'", gupCode))
			  .AddQueryOption("custCode", string.Format("'{0}'", custCode))
			  .AddQueryOption("wmsordno", string.Format("'{0}'", wmsOrdNo))
			  .ToObservableCollection();

			//包裝列印表單
			proxyP05 = GetProxy<F05Entities>();
			var data =
			  proxyP05.F050801s.Where(
				x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsOrdNo)
				.FirstOrDefault();

			//出貨單關聯訂單資料
			var f050301Datas = proxyP05.CreateQuery<F050301>("GetDataByWmsOrdNo")
			  .AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
			  .AddQueryOption("gupCode", string.Format("'{0}'", gupCode))
			  .AddQueryOption("custCode", string.Format("'{0}'", custCode))
			  .AddQueryOption("wmsOrdNo", string.Format("'{0}'", wmsOrdNo))
			  .ToList();

			//託運單
			ConsignmentNote = proxyP05Ex.CreateQuery<F050901WithF055001>("GetF050901WithF055001s")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("wmsOrdNo", wmsOrdNo).ToObservableCollection();

      //託運單箱內明細資料
      ConsignmentDetail = proxyP05Ex.CreateQuery<ConsignmentDetail>("GetConsignmentDetail")
        .AddQueryExOption("dcCode", dcCode)
        .AddQueryExOption("gupCode", gupCode)
        .AddQueryExOption("custCode", custCode)
        .AddQueryExOption("wmsNo", wmsOrdNo).ToObservableCollection();

      PrintList = new List<PrintListClass>();
			var printList = new List<PrintListClass>();

			if (data.PRINT_PASS == "1")
				AddToPrintList(ref printList, Properties.Resources.P0503020100_PastNo, "");
			if (f050301Datas.First().PRINT_RECEIPT == "1")
				AddToPrintList(ref printList, Properties.Resources.P0503020100_Receipt, "");
			if (data.HELLO_LETTER == "1")
				AddToPrintList(ref printList, "Welcome Letter", "");
			if (data.PRINT_BOX == "1")
				AddToPrintList(ref printList, Properties.Resources.P0503020100_Box, "");
			if (data.PRINT_DELV == "1")
				AddToPrintList(ref printList, Properties.Resources.P0503020100_Delv, "");
			if (data.SELF_TAKE == "1")
				AddToPrintList(ref printList, Properties.Resources.P0503020100_Take, "");

			PrintList = printList;
		}

		private void AddToPrintList(ref List<PrintListClass> printList, string docname, string memo)
		{
			PrintListClass a = new PrintListClass()
			{
				DOC_NAME = docname,
				MEMO = memo
			};
			printList.Add(a);
		}

		#endregion
	}
}
