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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using System.Windows;
using Wcf = Wms3pl.WpfClient.ExDataServices.T05WcfService;
using Wcf05 = Wms3pl.WpfClient.ExDataServices.P05WcfService;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0503040000_ViewModel : InputViewModelBase
	{
		#region Property,Fields...
		public Action OpenP05030401 = delegate { };

		#region Form - 查詢條件1
		#region ComBox - DC/ ORDType
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get
			{
				return (UserOperateMode != OperateMode.Query) ? _dcList.Where(x => x.Value != "000").ToList() : _dcList;
			}
			set { _dcList = value; RaisePropertyChanged(); }
		}
		private string _selectedDc = string.Empty;
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				_selectedDc = value;
				RaisePropertyChanged();
			}
		}

		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		/// <summary>
		/// 訂單類型
		/// </summary>
		private List<NameValuePair<string>> _oRDTypeList;
		public List<NameValuePair<string>> ORDTypeList
		{
			get { return _oRDTypeList; }
			set
			{
				_oRDTypeList = value;
				RaisePropertyChanged();
			}
		}

		private OrderType _orderType;
		public OrderType OrderType
		{
			get { return _orderType; }
			set { Set(ref _orderType, value); }
		}

		private string _selectedORDType = string.Empty;
		public string SelectedORDType
		{
			get { return _selectedORDType; }
			set
			{
				_selectedORDType = value;
				RaisePropertyChanged();

				OrderType = value == "0" ? OrderType.B2B : OrderType.B2C;
				SetSourceType();
				//if (OrderType == OrderType.B2B)
				//{
				//	if (SourceTypeList.Any())
				//	{
				//		SourceTypeList.Remove(SourceTypeList.Where(o => o.Value == "").FirstOrDefault());
				//		SelectedSourceType = SourceTypeList.Where(o => o.Value == "01").FirstOrDefault().Value;
				//	}
				//}
				//else if (OrderType == OrderType.B2C)
				//{
				//	SourceTypeList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
				//	SelectedSourceType = SourceTypeList.FirstOrDefault().Value;
				//}
				SerialRecords = null;
				//RaisePropertyChanged("SourceTypeList");
			}
		}
		/// <summary>
		/// 來源單據
		/// </summary>
		private List<NameValuePair<string>> _sourceTypeList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> SourceTypeList
		{
			get { return _sourceTypeList; }
			set
			{
				_sourceTypeList = value;
				RaisePropertyChanged();
			}
		}

		private string _selectedSourceType = string.Empty;
		public string SelectedSourceType
		{
			get { return _selectedSourceType; }
			set
			{
				_selectedSourceType = value;
				RaisePropertyChanged();
			}
		}
    #endregion

    #region 通路類型 CHANNEL_LIST
    private List<NameValuePair<string>> _channellist;

    public List<NameValuePair<string>> CHANNEL_LIST
    {
      get { return _channellist; }
      set
      {
        _channellist = value;
        RaisePropertyChanged("CHANNEL_LIST");
      }
    }

    private List<NameValuePair<string>> _subchannellist;

    public List<NameValuePair<string>> SUBCHANNEL_LIST
    {
      get { return _subchannellist; }
      set
      {
        _subchannellist = value;
        RaisePropertyChanged("SUBCHANNEL_LIST");
      }
    }


    #region 查詢選取的通路商
    private string _selectedChannel;

    public string SelectedChannel
    {
      get { return _selectedChannel; }
      set
      {
        Set(() => SelectedChannel, ref _selectedChannel, value);
      }
    }
    #endregion 查詢選取的通路商

    #region 查詢選取的通路商
    private string _selectedSubChannel;

    public string SelectedSubChannel
    {
      get { return _selectedSubChannel; }
      set
      {
        Set(() => SelectedSubChannel, ref _selectedSubChannel, value);
      }
    }
    #endregion 查詢選取的通路商
    public void GET_CHANNEL_LIST()
    {
      CHANNEL_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "CHANNEL");
      SelectedChannel = CHANNEL_LIST?.First().Value;
    }

    public void GET_SUBCHANNEL_LIST()
    {
      SUBCHANNEL_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "SUBCHANNEL");
      SelectedSubChannel = SUBCHANNEL_LIST?.First().Value;
    }

    #endregion  通路類型 CHANNEL_LIST

    #region Other - OrdSDate,OrdEDate,ArrivalSDate,ArrivalEDate,ORD_NO,CUST_ORD_NO,CONSIGNEE,ITEM_CODE,ITEM_NAME,IsBatch
    /// <summary>
    /// 訂單日期-起
    /// </summary>
    private DateTime _ordSDate = DateTime.Now;
		public DateTime OrdSDate { get { return _ordSDate; } set { _ordSDate = value; RaisePropertyChanged(); } }
		/// <summary>
		/// 訂單日期-迄
		/// </summary>
		private DateTime _ordEDate = DateTime.Now.AddDays(10);
		public DateTime OrdEDate { get { return _ordEDate; } set { _ordEDate = value; RaisePropertyChanged(); } }
		/// <summary>
		/// 指定到貨日期-起
		/// </summary>
		private DateTime? _arrivalSDate; //= DateTime.Now;
		public DateTime? ArrivalSDate { get { return _arrivalSDate; } set { _arrivalSDate = value; RaisePropertyChanged(); } }
		/// <summary>
		/// 指定到貨日期-迄
		/// </summary>
		private DateTime? _arrivalEDate; // = DateTime.Now.AddDays(10);
		public DateTime? ArrivalEDate { get { return _arrivalEDate; } set { _arrivalEDate = value; RaisePropertyChanged(); } }
		/// <summary>
		/// 貨主自訂分類
		/// </summary>
		private string _cUST_COST = string.Empty;
		public string CUST_COST { get { return _cUST_COST; } set { _cUST_COST = value.Trim(); RaisePropertyChanged("CUST_COST"); } }
		/// <summary>
		/// 訂單單號
		/// </summary>
		private string _oRD_NO = string.Empty;
		public string ORD_NO { get { return _oRD_NO; } set { _oRD_NO = value.Trim(); RaisePropertyChanged(); } }

		/// <summary>
		/// 貨主單號
		/// </summary>
		private string _cUST_ORD_NO = string.Empty;
		public string CUST_ORD_NO { get { return _cUST_ORD_NO; } set { _cUST_ORD_NO = value.Trim(); RaisePropertyChanged(); } }

		/// <summary>
		/// 收件人
		/// </summary>
		private string _cONSIGNEE = string.Empty;
		public string CONSIGNEE { get { return _cONSIGNEE; } set { _cONSIGNEE = value.Trim(); RaisePropertyChanged(); } }
		/// <summary>
		/// 客戶編號
		/// </summary>
		private string _rETAIL_CODE;
		public string RETAIL_CODE
		{
			get { return _rETAIL_CODE; }
			set { Set(ref _rETAIL_CODE, value); }
		}
		/// <summary>
		/// 出車時段
		/// </summary>
		private List<NameValuePair<string>> _car_PeriodList;
		public List<NameValuePair<string>> CAR_PERIODList
		{
			get { return _car_PeriodList; }
			set
			{
				_car_PeriodList = value;
				RaisePropertyChanged("CAR_PERIODList");
			}
		}
		/// <summary>
		/// 出車時段
		/// </summary>
		private string _SelectedCar_Period = string.Empty;
		public string SelectedCAR_PERIOD
		{
			get { return _SelectedCar_Period; }
			set
			{
				_SelectedCar_Period = value;
				RaisePropertyChanged("SelectedCAR_PERIOD");
			}
		}
		/// <summary>
		/// 車次/路順
		/// </summary>
		private string _dELV_NO;
		public string DELV_NO
		{
			get { return _dELV_NO; }
			set { Set(ref _dELV_NO, value); }
		}

		/// <summary>
		/// 品號
		/// </summary>
		private string _iTEM_CODE = string.Empty;
		public string ITEM_CODE { get { return _iTEM_CODE; } set { _iTEM_CODE = value.Trim(); RaisePropertyChanged(); } }

		/// <summary>
		/// 品名
		/// </summary>
		private string _iTEM_NAME = string.Empty;
		public string ITEM_NAME { get { return _iTEM_NAME; } set { _iTEM_NAME = value.Trim(); RaisePropertyChanged(); } }

        /// <summary>
        /// 序號
        /// </summary>
        private string _SERIAL_NO = string.Empty;
        public string SERIAL_NO { get { return _SERIAL_NO; } set { _SERIAL_NO = value?.Trim(); RaisePropertyChanged(); } }

        /// <summary>
        /// 是否以批次號配庫
        /// </summary>
        private bool _isBatch = false;
		public bool IsBatch { get { return _isBatch; } set { _isBatch = value; RaisePropertyChanged(); } }
		#endregion
		#endregion

		#region 是否顯示作業類別
		public Visibility IsShowTransaction { get { return (_custCode == "010001") ? Visibility.Hidden : Visibility.Visible; } }
		#endregion

		#region 是否全選

		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				CheckSelectedAll(_isCheckAll);
				RaisePropertyChanged();
			}
		}
		#endregion

		#region Form - 尚未配庫的訂單資料

		private SelectionList<F050001Data> _serialRecords = new SelectionList<F050001Data>(new List<F050001Data>());
		public SelectionList<F050001Data> SerialRecords { get { return _serialRecords; } set { _serialRecords = value; RaisePropertyChanged(); } }
		private SelectionItem<F050001Data> _selectSerialRecord;
		public SelectionItem<F050001Data> SelectSerialRecord
		{
			get { return _selectSerialRecord; }
			set
			{
				_selectSerialRecord = value;
				RaisePropertyChanged();
			}
		}


		#region
		private string _calNo;

		public string CalNo
		{
			get { return _calNo; }
			set
			{
				Set(() => CalNo, ref _calNo, value);
			}
		}
		#endregion
		private List<string> OrderNos { get; set; }

		private string _bATCH_NO = string.Empty;
		public string BATCH_NO { get { return _bATCH_NO; } set { _bATCH_NO = value.Trim(); RaisePropertyChanged(); } }
		#endregion

		#region 優先處理旗標清單
		private List<NameValuePair<string>> _fastDealTypeList;
		public List<NameValuePair<string>> FastDealTypeList
		{
			get { return _fastDealTypeList; }
			set
			{
				Set(() => FastDealTypeList, ref _fastDealTypeList, value);
			}
		}

		private string _selectFastDealType;
		public string SelectFastDealType
		{
			get { return _selectFastDealType; }
			set
			{
				Set(() => SelectFastDealType, ref _selectFastDealType, value);
			}
		}
		#endregion

		#region 客戶自訂分類
		private List<NameValuePair<string>> _custCostList;
		public List<NameValuePair<string>> CustCostList
		{
			get { return _custCostList; }
			set { Set(() => CustCostList, ref _custCostList, value); }
		}
		private string _selectCustCost;
		public string SelectCustCost
		{
			get { return _selectCustCost; }
			set {
				Set(() => SelectCustCost, ref _selectCustCost, value);
				if (CrossCodeList != null && CrossCodeList.Any())
				{
					if (SelectCustCost == "MoveOut")
					{
						CrossCodeEnable = true;
					}
					else
					{
						SelectCrossCode = CrossCodeList.FirstOrDefault().Value;
						CrossCodeEnable = false;
					}
				}
			}
			
		}
		#endregion

		#region 跨庫目的地
		private List<NameValuePair<string>> _crossCodeList;
		public List<NameValuePair<string>> CrossCodeList
		{
			get { return _crossCodeList; }
			set { Set(() => CrossCodeList, ref _crossCodeList, value); }
		}
		private string _selectCrossCode;
		public string SelectCrossCode
		{
			get { return _selectCrossCode; }
			set { Set(() => SelectCrossCode, ref _selectCrossCode, value); }
		}

		// 允許使用跨庫目的地
		private bool _crossCodeEnable;
		public bool CrossCodeEnable
		{
			get { return _crossCodeEnable; }
			set { Set(() => CrossCodeEnable, ref _crossCodeEnable, value); }
		}
		#endregion


		#region 指定自動倉揀貨優先處理旗標清單
		private List<NameValuePair<string>> _priorityList;

		public List<NameValuePair<string>> PriorityList
		{
			get { return _priorityList; }
			set
			{
				Set(() => PriorityList, ref _priorityList, value);
			}
		}
		#endregion


		#region 選擇指定自動倉揀貨優先處理旗標代碼
		private string _selectedPriorityCode;

		public string SelectedPriorityCode
		{
			get { return _selectedPriorityCode; }
			set
			{
				Set(() => SelectedPriorityCode, ref _selectedPriorityCode, value);
			}
		}
		#endregion


		#endregion



		#region Function
		public P0503040000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetOrdType();
				SetSourceType();
				SetCAR_PERIOD();
				SetFastDealType();
				SetCustCost();
				SetCrossCode();
        GET_CHANNEL_LIST();
        GET_SUBCHANNEL_LIST();
				SetPriorityList();

			}

    }

		#region Grid 全選

		public void CheckSelectedAll(bool isChecked)
		{
			if (SerialRecords != null)
			{
				foreach (var SerialRecord in SerialRecords)
					SerialRecord.IsSelected = isChecked;
			}
		}

		#endregion

		#region COMBOX - Init
		#region 設定DC清單
		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			var data = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
			//data.Insert(0, new NameValuePair<string>() { Name = "不指定", Value = "000" });
			DcList = data;
			if (DcList.Any())
				SelectedDc = DcList.FirstOrDefault().Value;
		}
		#endregion
		#region 訂單類型
		public void SetOrdType()
		{
			ORDTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F050001", "ORD_TYPE");
			if (ORDTypeList.Any())
				SelectedORDType = ORDTypeList.FirstOrDefault(o=>o.Name =="B2C").Value;
		}

		#endregion
		#region 來源單據

		public void SetSourceType()
		{
			var proxy = GetProxy<F00Entities>();
			List<NameValuePair<string>> data = new List<NameValuePair<string>>();
			//B2B:0
			data = proxy.F000902s.Select(o => new NameValuePair<string> { Name = o.SOURCE_NAME, Value = o.SOURCE_TYPE }).ToList();
			//data.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			if(OrderType == OrderType.B2B)
			{
				SourceTypeList = data.Where(x=>x.Value == "13").ToList(); //訂單類型選擇B2B，來源單據只有顯示廠退出貨
			}
			else if (OrderType == OrderType.B2C)
			{
				SourceTypeList = data.Where(x => x.Value == "01").ToList(); //訂單類型選擇B2C，來源單據只有顯示訂單
			}
			
			if (SourceTypeList.Any())
				SelectedSourceType = SourceTypeList.FirstOrDefault().Value;
			//SelectedSourceType = SourceTypeList.FirstOrDefault().Value;
		}

		public void ChengeSourceType()
		{

		}

		public void ChangeSourceType()
		{ 

		}
		#endregion
		#region 出車時段

		public void SetCAR_PERIOD()
		{
			var proxy = GetProxy<F00Entities>();
			List<NameValuePair<string>> data = new List<NameValuePair<string>>();

			data = proxy.F000904s.Where(o => o.TOPIC == "F194716" && o.SUBTOPIC == "CAR_PERIOD").Select(o => new NameValuePair<string> { Name = o.NAME, Value = o.VALUE }).ToList();
			data.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
			CAR_PERIODList = data;

			if (CAR_PERIODList.Any())
				SelectedCAR_PERIOD = CAR_PERIODList.FirstOrDefault().Value;
		}
		#endregion
		#region 優先處理旗標
		private void SetFastDealType()
		{
			FastDealTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "FAST_DEAL_TYPE", true);
			if (FastDealTypeList.Any())
				SelectFastDealType = FastDealTypeList.FirstOrDefault().Value;
		}
		#endregion


		#region 客戶自訂分類
		private void SetCustCost()
		{
			CustCostList = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "CUST_COST", true);
			if (FastDealTypeList.Any())
				SelectCustCost = CustCostList.FirstOrDefault().Value;
		}

		#endregion

		#region 跨庫目的地
		private void SetCrossCode()
		{
			var f01Proxy = GetProxy<F00Entities>();
			CrossCodeList = f01Proxy.F0001s.Where(x => x.CROSS_TYPE == "01").Select(x => new NameValuePair<string> {
				Name = x.CROSS_NAME,
				Value = x.CROSS_CODE
			}).ToList();
			if (CrossCodeList.Any() && CrossCodeList != null)
			{
				// 加入全部選項
				CrossCodeList.Insert(0, new NameValuePair<string> { Name = "全部", Value = "" });
				SelectCrossCode = CrossCodeList.FirstOrDefault().Value;
			}
				
		}

		#endregion

		#region 指定自動倉優先處理旗標清單
		private void SetPriorityList()
		{
			var proxy = GetProxy<F19Entities>();
			var list = proxy.F1956s.Where(x => x.IS_SHOW == "1").Select(x => new NameValuePair<string>
			{
				Name = x.PRIORITY_NAME,
				Value = x.PRIORITY_CODE
			}).ToList();
			list.Insert(0, new NameValuePair<string> { Name = "不指定", Value = "" });
			PriorityList = list;
			if (list.Any())
				SelectedPriorityCode = list.First().Value;

		}

		#endregion

		#endregion

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

		private void DoSearch(bool isSysSearch = false)
		{
			//執行查詢動

			var dcCode = SelectedDc;
			var gupCode = _gupCode;
			var custCode = _custCode;
			var proxy = GetExProxy<P05ExDataSource>();
			var query = proxy.CreateQuery<F050001Data>("GetF050001Datas")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("ordType", SelectedORDType)
				.AddQueryExOption("ordSDate", OrdSDate.ToString("yyyy/MM/dd"))
				.AddQueryExOption("ordEDate", OrdEDate.ToString("yyyy/MM/dd"))
				.AddQueryExOption("arrivalSDate", ArrivalSDate != null ? ArrivalSDate.Value.ToString("yyyy/MM/dd") : "")
				.AddQueryExOption("arrivalEDate", ArrivalEDate != null ? ArrivalEDate.Value.ToString("yyyy/MM/dd") : "")
				.AddQueryExOption("ordNo", ORD_NO)
				.AddQueryExOption("custOrdNo", CUST_ORD_NO)
				.AddQueryExOption("consignee", CONSIGNEE)
				.AddQueryExOption("itemCode", ITEM_CODE)
				.AddQueryExOption("itemName", string.Empty)
				.AddQueryExOption("sourceType", SelectedSourceType)
				.AddQueryExOption("retailCode", RETAIL_CODE)
				.AddQueryExOption("carPeriod", SelectedCAR_PERIOD == "全部" ? "" : SelectedCAR_PERIOD)
				.AddQueryExOption("delvNo", DELV_NO)
        .AddQueryExOption("custCost", SelectCustCost)
        .AddQueryExOption("fastDealType", SelectFastDealType)
        .AddQueryExOption("crossCode", SelectCrossCode)
        .AddQueryExOption("channel", SelectedChannel)
        .AddQueryExOption("subChannel", SelectedSubChannel);

      SerialRecords = query.OrderBy(a => a.ORD_NO).ToSelectionList();

			if (SerialRecords == null || !SerialRecords.Any())
			{
				if (!isSysSearch)
					ShowMessage(Messages.InfoNoData);
				SelectSerialRecord = null;
				return;
			}
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
					o => DoSave(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save

		#region Allocation
		public ICommand Allocation
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAllocation(), () => true
					);
			}
		}

		private void DoAllocation()
		{
      if (SerialRecords == null)
				SerialRecords = new List<F050001Data>().ToSelectionList();

      if (!SerialRecords.Any(x => x.IsSelected))
      {
        ShowWarningMessage(Properties.Resources.P0503040000_Pls_Select_Item);
        return;
      }

      if (IsBatch)
			{
				if (string.IsNullOrEmpty(BATCH_NO))
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P0503020000_BatchNoIsNull, Title = Resources.Resources.Information });
					return;
				}
				OrderNos = SerialRecords.Where(x => x.Item.BATCH_NO == BATCH_NO).Select(x => x.Item.ORD_NO).ToList();
			}
			else
				OrderNos = SerialRecords.Where(x => x.IsSelected).Select(x => x.Item.ORD_NO).ToList();

			//配庫
			AllotStocks();
			DoSearch();
		}

		private void AllotStocks()
		{
			var wcfproxy = new Wcf.T05WcfServiceClient();
			var results = RunWcfMethod<Wcf.ExecuteResult[]>(wcfproxy.InnerChannel, () => wcfproxy.AllotStocks(OrderNos.ToArray(),SelectedPriorityCode));
			var hasShowMsg = false;
			foreach (var result in results)
			{
				if (result.IsSuccessed && string.IsNullOrEmpty(result.Message))
					continue;
				ShowResultMessage(result);
				hasShowMsg = true;
			}

			if (!hasShowMsg)
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P0503040000_AllotStocksSuccess, Title = Resources.Resources.Information });
		}
		#endregion

		#region Calculation
		public ICommand CalculationCommand
		{
			get
			{
        Boolean IsSuccess = false;
        return CreateBusyAsyncCommand(
          o => IsSuccess = DoCalculation(),
          () => true,
          o => { if (IsSuccess) OpenP05030401(); }
          );
      }
    }

		private Boolean DoCalculation()
		{
      if (!SerialRecords?.Any(x => x.IsSelected) ?? true)
      {
        ShowWarningMessage(Properties.Resources.P0503040000_Pls_Select_Item);
        return false;
      }
      OrderNos = new List<string>();

			if (SerialRecords == null)
				SerialRecords = new List<F050001Data>().ToSelectionList();

			if (IsBatch)
			{
				if (string.IsNullOrEmpty(BATCH_NO))
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P0503020000_BatchNoIsNull, Title = Resources.Resources.Information });
					return false;
				}
				OrderNos = SerialRecords.Where(x => x.Item.BATCH_NO == BATCH_NO).Select(x => x.Item.ORD_NO).ToList();
			}
			else
				OrderNos = SerialRecords.Where(x => x.IsSelected).Select(x => x.Item.ORD_NO).ToList();

			var proxy = GetWcfProxy<Wcf05.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.AllotStockTrialCalculation(SelectedDc, _gupCode, _custCode, OrderNos.ToArray()));
			if (result.IsSuccessed)
			{
				CalNo = result.No;
			}
			else
				ShowWarningMessage(result.Message);

      return true;
		}
		#endregion

		#endregion

	}

	public enum OrderType
	{
		B2B = 0,
		B2C = 1
	}
}
