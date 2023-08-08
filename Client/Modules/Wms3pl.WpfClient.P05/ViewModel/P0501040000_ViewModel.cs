using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using wcf = Wms3pl.WpfClient.ExDataServices.P05WcfService;
using Wms3pl.WpfClient.P05.Properties;
using System.Dynamic;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0501040000_ViewModel : InputViewModelBase
	{
		public Action StartAutoUpdate = delegate { };
		public Action<Boolean> StopAutoUpdate = delegate { };
		public Action DoPrintReport = delegate { };
		public Action DoPrintSinglePickNosTicker = delegate { };
		public Action DoPrintBatchPickNosTicker = delegate { };
		public F910501 SelectedF910501 { get; set; }
		public P0501040000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcCode();
				SetSourceType();
				SetCustCost();
				GetOrdType();
				GetFastDealType();
				GetPickTools();
			}

		}

		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		#region 物流中心
		// 物流中心清單
		private List<NameValuePair<string>> _dcCodeList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> DcCodeList
		{
			get { return _dcCodeList; }
			set { Set(() => DcCodeList, ref _dcCodeList, value); }
		}

		// 所選擇的物流中心
		private NameValuePair<string> _selectedDcCode;
		public NameValuePair<string> SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				Set(() => SelectedDcCode, ref _selectedDcCode, value);
			}
		}

		//設定物流中心
		public void SetDcCode()
		{
			DcCodeList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodeList.Any())
				SelectedDcCode = DcCodeList.FirstOrDefault();
		}

		#endregion

		#region 自動更新文字內容
		private string _autoUpdateInfo;
		public string AutoUpdateInfo
		{
			get { return _autoUpdateInfo; }
			set { Set(() => AutoUpdateInfo, ref _autoUpdateInfo, value); }
		}
		#endregion

		#region 自動更新按鈕
		// 起動自動更新
		private bool _autoUpdateVisibility;
		public bool AutoUpdateVisibility
		{
			get { return _autoUpdateVisibility; }
			set { Set(() => AutoUpdateVisibility, ref _autoUpdateVisibility, value); }
		}
		#endregion

		#region 自動更新狀態(1:開始自動更新;0:暫停自動更新)
		public bool _autoUpdateStatus = true;
		public bool AutoUpdateStatus
		{
			get { return _autoUpdateStatus; }
			set
			{
				Set(() => AutoUpdateStatus, ref _autoUpdateStatus, value);
			}
		}
		#endregion

		#region 自動更新 啟用/停用 欄位
		private bool _autoUpdateEnable = true;
		public bool AutoUpdateEnable
		{
			get { return _autoUpdateEnable; }
			set { Set(() => AutoUpdateEnable, ref _autoUpdateEnable, value); }
		}
		#endregion

		#region 來源單據
		// 來源單據清單
		private List<NameValuePair<string>> _sourceTypeList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> SourceTypeList
		{
			get { return _sourceTypeList; }
			set { Set(() => SourceTypeList, ref _sourceTypeList, value); }
		}

		//所選擇來源單據
		private NameValuePair<string> _selectedSourceType;
		public NameValuePair<string> SelectedSourceType
		{
			get { return _selectedSourceType; }
			set
			{ Set(() => SelectedSourceType, ref _selectedSourceType, value); }
		}

		// 設定來源單據清單
		public void SetSourceType()
		{
			var proxy = GetProxy<F00Entities>();
			var data = new List<NameValuePair<string>>();
      data = GetBaseTableService.GetF000904List(FunctionCode, "F000902", "SOURCE_TYPE", true);
      SourceTypeList = data;
			if (SourceTypeList.Any())
				SelectedSourceType = SourceTypeList.FirstOrDefault();
		}
		#endregion

		#region 客戶自訂分類
		// 客戶自訂分類清單
		private List<NameValuePair<string>> _custCostList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> CustCostList
		{
			get { return _custCostList; }
			set { Set(() => CustCostList, ref _custCostList, value); }
		}

		//所選擇客戶自訂分類
		private NameValuePair<string> _selectedCustCost;
		public NameValuePair<string> SelectedCustCost
		{
			get { return _selectedCustCost; }
			set { Set(() => SelectedCustCost, ref _selectedCustCost, value); }
		}

		// 設定客戶自訂分類清單
		public void SetCustCost()
		{
			var proxy = GetProxy<F00Entities>();
			List<NameValuePair<string>> data = new List<NameValuePair<string>>();
			CustCostList = GetBaseTableService.GetF000904List(FunctionCode, "F050101 ", "CUST_COST", true);
			if (CustCostList.Any())
				SelectedCustCost = CustCostList.FirstOrDefault();
		}

		#endregion

		#region 優先處理旗標
		private List<NameValuePair<string>> _fastDealTypeList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> FastDealTypeList
		{
			get { return _fastDealTypeList; }
			set { Set(() => FastDealTypeList, ref _fastDealTypeList, value); }
		}

		public void GetFastDealType()
		{
			FastDealTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F050101 ", "FAST_DEAL_TYPE");
		}
		#endregion


		#region 揀貨工具
		private List<NameValuePair<string>> _pickTools;

		public List<NameValuePair<string>> PickTools
		{
			get { return _pickTools; }
			set
			{
				Set(() => PickTools, ref _pickTools, value);
			}
		}

		public void GetPickTools()
		{
			PickTools = GetBaseTableService.GetF000904List(FunctionCode, "F051201", "PICK_TOOL").Where(x => x.Value != "0").ToList();
		}
		#endregion



		#region 批次揀貨單 全選/取消全選

		private bool _isCheckAllBatchPickNo;

		public bool IsCheckAllBatchPickNo
		{
			get { return _isCheckAllBatchPickNo; }
			set
			{
				Set(() => IsCheckAllBatchPickNo, ref _isCheckAllBatchPickNo, value);
				CheckSelectedBatchPickNoAll(value);
			}
		}
		#endregion

		#region 快速補揀單 全選/取消全選
		private bool _isCheckAllRePickNo;

		public bool IsCheckAllRePickNo
		{
			get { return _isCheckAllRePickNo; }
			set
			{
				Set(() => IsCheckAllRePickNo, ref _isCheckAllRePickNo, value);
				CheckSelectedRePickNoAll(value);
			}
		}
		#endregion

		#region 補印揀貨單 全選/取消全選

		private bool _isCheckAllReprintPickNo;

		public bool IsCheckAllReprintPickNo
		{
			get { return _isCheckAllReprintPickNo; }
			set
			{
				Set(() => IsCheckAllReprintPickNo, ref _isCheckAllReprintPickNo, value);
				CheckSelectedReprintPickNo(value);
			}
		}
		#endregion



		#region 批次揀貨單
		private SelectionList<BatchPickNoList> _batchPickNoList;

		public SelectionList<BatchPickNoList> BatchPickNoList
		{
			get { return _batchPickNoList; }
			set
			{
				Set(() => BatchPickNoList, ref _batchPickNoList, value);
			}
		}

		private SelectionItem<BatchPickNoList> _selectBatchPickNo;
		public SelectionItem<BatchPickNoList> SelectBatchPickNo
		{
			get { return _selectBatchPickNo; }
			set
			{
				Set(() => SelectBatchPickNo, ref _selectBatchPickNo, value);
			}
		}



		#endregion

		#region 補揀單
		private SelectionList<RePickNoList> _rePickNoList;

		public SelectionList<RePickNoList> RePickNoList
		{
			get { return _rePickNoList; }
			set
			{
				Set(() => RePickNoList, ref _rePickNoList, value);
			}
		}

		private SelectionItem<RePickNoList> _selectRePickNo;
		public SelectionItem<RePickNoList> SelectRePickNo
		{
			get { return _selectRePickNo; }
			set
			{
				Set(() => SelectRePickNo, ref _selectRePickNo, value);
			}
		}
		#endregion

		#region 單據類型
		private List<NameValuePair<string>> _orderTypeList;
		public List<NameValuePair<string>> OrderTypeList
		{
			get { return _orderTypeList; }
			set { Set(() => OrderTypeList, ref _orderTypeList, value); }
		}
		private void GetOrdType()
		{
			OrderTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F050001", "ORD_TYPE").ToList();
		}


		#endregion

		#region 單一揀貨張數
		private List<SinglePickingReportData> _singlePickingReportDatas;
		public List<SinglePickingReportData> SinglePickingReportDatas
		{
			get { return _singlePickingReportDatas; }
			set { Set(() => SinglePickingReportDatas, ref _singlePickingReportDatas, value); }
		}
		#endregion

		#region 批次揀貨本張數
		private List<BatchPickingReportData> _batchPickingReportDatas;
		public List<BatchPickingReportData> BatchPickingReportDatas
		{
			get { return _batchPickingReportDatas; }
			set { Set(() => BatchPickingReportDatas, ref _batchPickingReportDatas, value); }
		}
		#endregion

		#region 單一揀貨貼紙
		private List<SinglePickingTickerData> _singlePickingTickerData;
		public List<SinglePickingTickerData> SinglePickingTickerDatas
		{
			get { return _singlePickingTickerData; }
			set { Set(() => SinglePickingTickerDatas, ref _singlePickingTickerData, value); }
		}
		#endregion

		#region 批量揀貨貼紙
		private List<BatchPickingTickerData> _batchPickingTickerDatas;
		public List<BatchPickingTickerData> BatchPickingTickerDatas
		{
			get { return _batchPickingTickerDatas; }
			set { Set(() => BatchPickingTickerDatas, ref _batchPickingTickerDatas, value); }
		}
    #endregion

    #region 補印批量揀貨單
    private SelectionList<BatchPickNoList> _ReBatchPickNoList;

    public SelectionList<BatchPickNoList> ReBatchPickNoList
    {
      get { return _ReBatchPickNoList; }
      set { Set(() => ReBatchPickNoList, ref _ReBatchPickNoList, value); }
    }
    #endregion 補印批量揀貨單

    #region 補印批量揀貨單 全選/取消全選
    private bool _isCheckAllReBatchPickNo;
    public bool IsCheckAllReBatchPickNo
    {
      get { return _isCheckAllReBatchPickNo; }
      set
      {
        Set(() => IsCheckAllReBatchPickNo, ref _isCheckAllReBatchPickNo, value);
        foreach (var item in ReBatchPickNoList)
          item.IsSelected = value;
      }
    }
    #endregion 補印批量揀貨單 全選/取消全選

    #region 補印批量揀貨單 補揀日期
    private DateTime? _ReBatchDelvDate = DateTime.Now.Date;
    public DateTime? ReBatchDelvDate
    { get { return _ReBatchDelvDate; }
      set { Set(() => ReBatchDelvDate, ref _ReBatchDelvDate, value); }
    }
    #endregion 補印批量揀貨單 補揀日期

    public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query && (AutoUpdateStatus || SelectedTabIndex == 1),
					c => DoSearchComplete()
					);
			}
		}


		public void DoSearch()
		{
			var proxyEx = GetExProxy<P05ExDataSource>();

			if (SelectedTabIndex == 0)
			{
				var batchPickNo = proxyEx.CreateQuery<BatchPickNoList>("GetBatchPickNoList")
											.AddQueryExOption("dcCode", SelectedDcCode.Value)
											.AddQueryExOption("gupCode", _gupCode)
											.AddQueryExOption("custCode", _custCode)
											.AddQueryExOption("sourceType", SelectedSourceType.Value)
											.AddQueryExOption("custCost", SelectedCustCost.Value)
										 .ToList();
				BatchPickNoList = batchPickNo.ToSelectionList();

				var rePickNo = proxyEx.CreateQuery<RePickNoList>("GetRePickNoList")
											.AddQueryExOption("dcCode", SelectedDcCode.Value)
											.AddQueryExOption("gupCode", _gupCode)
											.AddQueryExOption("custCode", _custCode)
											.AddQueryExOption("sourceType", SelectedSourceType.Value)
											.AddQueryExOption("custCost", SelectedCustCost.Value)
										 .ToList();

				RePickNoList = rePickNo.ToSelectionList();
				RefreshAutoUpdateInfo();
			}
      else if (SelectedTabIndex == 1)
      {
				if (string.IsNullOrEmpty(PickOrdNo) && string.IsNullOrEmpty(WmsOrdNo))
				{
					ShowWarningMessage("揀貨單號與出貨單號必須擇一填入");
					return;
				}
				var reprintPickNo = proxyEx.CreateQuery<RePrintPickNoList>("GetReprintPickNoList")
										.AddQueryExOption("dcCode", SelectedDcCode.Value)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
										.AddQueryExOption("pickOrdNo", PickOrdNo)
										.AddQueryExOption("wmsOrdNo", WmsOrdNo)
									 .ToList();
				ReprintPickNoList = reprintPickNo.ToSelectionList();
			}
      else
      {
        if (!ReBatchDelvDate.HasValue)
        {
          ShowWarningMessage("請輸入批次日期");
          return;
        }
        var reprintPickNo = proxyEx.CreateQuery<BatchPickNoList>("GetReBatchPrintPickNoList")
            .AddQueryExOption("dcCode", SelectedDcCode.Value)
            .AddQueryExOption("gupCode", _gupCode)
            .AddQueryExOption("custCode", _custCode)
            .AddQueryExOption("DelvDate", ReBatchDelvDate.Value)
           .ToList();
        ReBatchPickNoList = reprintPickNo.ToSelectionList();

      }
    }
		public void DoSearchComplete()
		{
			//if (BatchPickNoList != null && BatchPickNoList.Any())
			//{
			//	SelectBatchPickNo = BatchPickNoList.First();
			//	IsCheckAllBatchPickNo = false;
			//}
			//else
			//	ShowMessage(Messages.InfoNoData);
		}



		public void CheckSelectedBatchPickNoAll(bool isChecked)
		{
			if (BatchPickNoList != null)
				foreach (var item in BatchPickNoList)
					item.IsSelected = isChecked;
		}

		public void CheckSelectedRePickNoAll(bool isChecked)
		{
			if (RePickNoList != null)
				foreach (var item in RePickNoList)
					item.IsSelected = isChecked;
		}

		public void CheckSelectedReprintPickNo(bool isChecked)
		{
			if (ReprintPickNoList != null)
				foreach (var item in ReprintPickNoList)
					item.IsSelected = isChecked;
		}


		public void RefreshAutoUpdateInfo()
		{
			AutoUpdateInfo = $"此頁面2分鐘會自動更新，最後更新日{DateTime.Now}";
		}

		#region 揀貨單號
		private string _pickOrdNo;
		public string PickOrdNo
		{
			get { return _pickOrdNo; }
			set { Set(() => PickOrdNo, ref _pickOrdNo, value); }
		}
		#endregion

		#region 出貨單號
		private string _wmsOrdNo;
		public string WmsOrdNo
		{
			get { return _wmsOrdNo; }
			set { Set(() => WmsOrdNo, ref _wmsOrdNo, value); }
		}
		#endregion

		#region 補印揀貨單

		private SelectionList<RePrintPickNoList> _reprintPickNoList;

		public SelectionList<RePrintPickNoList> ReprintPickNoList
		{
			get { return _reprintPickNoList; }
			set
			{
				Set(() => ReprintPickNoList, ref _reprintPickNoList, value);
			}
		}

		#endregion

		#region 列印批次揀貨單
		private bool _isInitDoPrint;
		public ICommand PrintCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						_isInitDoPrint = true;
						DoPrint(true);
					}, () =>
            UserOperateMode == OperateMode.Query &&
            (AutoUpdateStatus || SelectedTabIndex == 1) &&
              ((BatchPickNoList != null && BatchPickNoList.Any(x => x.IsSelected)) ||
              (RePickNoList != null && RePickNoList.Any(x => x.IsSelected)) ||
              (ReprintPickNoList != null && ReprintPickNoList.Any(x => x.IsSelected)) ||
              (ReBatchPickNoList != null && ReBatchPickNoList.Any(x => x.IsSelected))
              ),
          c => DoPrintComplete()
					);
			}
		}

		private List<wcf.BatchPickNoList> _printBatchPickNoList = null;
		private List<wcf.RePickNoList> _printRePickNoList = null;
		private List<wcf.BatchPickNoList> _printBatchPickNoListTemp = null;
		private List<wcf.RePickNoList> _printRePickNoListTemp = null;
		public void DoPrint(bool useLMSRoute)
		{

			var proxyP05 = GetExProxy<P05ExDataSource>();
			if (SelectedTabIndex == 0)
			{
				if (_isInitDoPrint)
				{
					_printBatchPickNoList = ExDataMapper.MapCollection<BatchPickNoList, wcf.BatchPickNoList>(BatchPickNoList.Where(si => si.IsSelected).Select(si => si.Item)).ToList();
					_printRePickNoList = ExDataMapper.MapCollection<RePickNoList, wcf.RePickNoList>(RePickNoList.Where(si => si.IsSelected).Select(si => si.Item)).ToList();
					_printBatchPickNoListTemp = _printBatchPickNoList.Select(a => a).ToList();
					_printRePickNoListTemp = _printRePickNoList.Select(a => a).ToList();
					_isInitDoPrint = false;
				}
				else
				{
					_printBatchPickNoList = _printBatchPickNoListTemp.Select(a => a).ToList();
					// 若_printBatchPickNoListTemp還有資料，代表補揀單尚未執行，所以不需做_printRePickNoList由_printRePickNoListTemp的轉換
					if (_printBatchPickNoListTemp == null || !_printBatchPickNoListTemp.Any())
					{
						_printRePickNoList = _printRePickNoListTemp.Select(a => a).ToList();
					}
				}

				var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
				foreach (var item in _printBatchPickNoList)
				{
					SinglePickingReportDatas = new List<SinglePickingReportData>();
					SinglePickingTickerDatas = new List<SinglePickingTickerData>();
					BatchPickingReportDatas = new List<BatchPickingReportData>();
					BatchPickingTickerDatas = new List<BatchPickingTickerData>();
					var result = proxy.RunWcfMethod(w => w.PrintUpdateBatchPickNo(item, useLMSRoute));
					// 只當LMS取路順失敗後選擇「依儲位順序排列」該筆useLMSRoute的值為false外，其他部分仍應該為true
					useLMSRoute = true;
					if (result.IsSuccessed)
					{
						_printBatchPickNoListTemp.Remove(item);
						// 單一揀貨紙本張數
						var singlePapers = proxyP05.CreateQuery<SinglePickingReportData>("GetSinglePickingReportDatas")
												.AddQueryExOption("dcCode", item.DC_CODE)
												.AddQueryExOption("gupCode", item.GUP_CODE)
												.AddQueryExOption("custCode", item.CUST_CODE)
												.AddQueryExOption("delvDate", item.DELV_DATE)
												.AddQueryExOption("pickTime", item.PICK_TIME)
												.AddQueryExOption("pickOrdNo", "")
											 .ToList();
						SinglePickingReportDatas.AddRange(singlePapers);

						// 批量揀貨紙本張數
						var batchPapers = proxyP05.CreateQuery<BatchPickingReportData>("GetBatchPickingReportDatas")
												.AddQueryExOption("dcCode", item.DC_CODE)
												.AddQueryExOption("gupCode", item.GUP_CODE)
												.AddQueryExOption("custCode", item.CUST_CODE)
												.AddQueryExOption("delvDate", item.DELV_DATE)
												.AddQueryExOption("pickTime", item.PICK_TIME)
												.AddQueryExOption("pickOrdNo", "")
											 .ToList();

						BatchPickingReportDatas.AddRange(batchPapers);

						// 單一揀貨貼紙
						var singlePastes = proxyP05.CreateQuery<SinglePickingTickerData>("GetSinglePickingTickerDatas")
												 .AddQueryExOption("dcCode", item.DC_CODE)
												 .AddQueryExOption("gupCode", item.GUP_CODE)
												 .AddQueryExOption("custCode", item.CUST_CODE)
												 .AddQueryExOption("delvDate", item.DELV_DATE)
												 .AddQueryExOption("pickTime", item.PICK_TIME)
												 .AddQueryExOption("pickOrdNo", "")
											.ToList();
						SinglePickingTickerDatas.AddRange(singlePastes);

						//批量揀貨貼紙
						var batchPastes = proxyP05.CreateQuery<BatchPickingTickerData>("GetBatchPickingTickerDatas")
												 .AddQueryExOption("dcCode", item.DC_CODE)
												 .AddQueryExOption("gupCode", item.GUP_CODE)
												 .AddQueryExOption("custCode", item.CUST_CODE)
												 .AddQueryExOption("delvDate", item.DELV_DATE)
												 .AddQueryExOption("pickTime", item.PICK_TIME)
												 .AddQueryExOption("pickOrdNo", "")
											.ToList();
						BatchPickingTickerDatas.AddRange(batchPastes);
						DoPrintReport();
					}
					else if (!result.IsSuccessed && !string.IsNullOrEmpty(result.Message))
					{
						var confirmResp = ShowConfirmMessage(result.Message, Properties.Resources.Reacquire, Properties.Resources.ArrangedInOrderOfStorage, Properties.Resources.CancelPrinting);
						switch (confirmResp)
						{
							case DialogResponse.Yes:
								DoPrint(true);
								break;
							case DialogResponse.No:
								DoPrint(false);
								break;
							case DialogResponse.Cancel:
								break;
						}

						return;
					}
				}

				foreach (var item in _printRePickNoList)
				{
					SinglePickingReportDatas = new List<SinglePickingReportData>();
					SinglePickingTickerDatas = new List<SinglePickingTickerData>();
					BatchPickingReportDatas = new List<BatchPickingReportData>();
					BatchPickingTickerDatas = new List<BatchPickingTickerData>();
					var result1 = proxy.RunWcfMethod(w => w.PrintUpdateRePickNo(item, useLMSRoute));
					// 只當LMS取路順失敗後選擇「依儲位順序排列」該筆useLMSRoute的值為false外，其他部分仍應該為true
					useLMSRoute = true;
					if (result1.IsSuccessed)
					{
						_printRePickNoListTemp.Remove(item);
						if (item.PICK_TOOL == "1")
						{
							// 單一揀貨紙本張數
							var singlePapers = proxyP05.CreateQuery<SinglePickingReportData>("GetSinglePickingReportDatas")
													.AddQueryExOption("dcCode", item.DC_CODE)
													.AddQueryExOption("gupCode", item.GUP_CODE)
													.AddQueryExOption("custCode", item.CUST_CODE)
													.AddQueryExOption("delvDate", item.DELV_DATE)
													.AddQueryExOption("pickTime", item.PICK_TIME)
													.AddQueryExOption("pickOrdNo", item.PICK_ORD_NO)
												 .ToList();
							SinglePickingReportDatas.AddRange(singlePapers);

							// 批量揀貨紙本張數
							var batchPapers = proxyP05.CreateQuery<BatchPickingReportData>("GetBatchPickingReportDatas")
													.AddQueryExOption("dcCode", item.DC_CODE)
													.AddQueryExOption("gupCode", item.GUP_CODE)
													.AddQueryExOption("custCode", item.CUST_CODE)
													.AddQueryExOption("delvDate", item.DELV_DATE)
													.AddQueryExOption("pickTime", item.PICK_TIME)
													.AddQueryExOption("pickOrdNo", item.PICK_ORD_NO)
												 .ToList();

							BatchPickingReportDatas.AddRange(batchPapers);
						}
						else
						{
							// 單一揀貨貼紙
							var singlePastes = proxyP05.CreateQuery<SinglePickingTickerData>("GetSinglePickingTickerDatas")
													 .AddQueryExOption("dcCode", item.DC_CODE)
													 .AddQueryExOption("gupCode", item.GUP_CODE)
													 .AddQueryExOption("custCode", item.CUST_CODE)
													 .AddQueryExOption("delvDate", item.DELV_DATE)
													 .AddQueryExOption("pickTime", item.PICK_TIME)
													 .AddQueryExOption("pickOrdNo", item.PICK_ORD_NO)
												.ToList();
							SinglePickingTickerDatas.AddRange(singlePastes);

							//批量揀貨貼紙
							var batchPastes = proxyP05.CreateQuery<BatchPickingTickerData>("GetBatchPickingTickerDatas")
													 .AddQueryExOption("dcCode", item.DC_CODE)
													 .AddQueryExOption("gupCode", item.GUP_CODE)
													 .AddQueryExOption("custCode", item.CUST_CODE)
													 .AddQueryExOption("delvDate", item.DELV_DATE)
													 .AddQueryExOption("pickTime", item.PICK_TIME)
													 .AddQueryExOption("pickOrdNo", item.PICK_ORD_NO)
												.ToList();
							BatchPickingTickerDatas.AddRange(batchPastes);
						}
						DoPrintReport();
					}
					else if (!result1.IsSuccessed && !string.IsNullOrEmpty(result1.Message))
					{
						var confirmResp = ShowConfirmMessage(result1.Message, Properties.Resources.Reacquire, Properties.Resources.ArrangedInOrderOfStorage, Properties.Resources.CancelPrinting);
						switch (confirmResp)
						{
							case DialogResponse.Yes:
								DoPrint(true);
								break;
							case DialogResponse.No:
								DoPrint(false);
								break;
							case DialogResponse.Cancel:
								break;
						}

						return;
					}
				}
			}
      else if (SelectedTabIndex == 1)
      {
				SinglePickingReportDatas = new List<SinglePickingReportData>();
				SinglePickingTickerDatas = new List<SinglePickingTickerData>();
				BatchPickingReportDatas = new List<BatchPickingReportData>();
				BatchPickingTickerDatas = new List<BatchPickingTickerData>();

        var a = ReprintPickNoList.Where(x => x.IsSelected).Select(x => x.Item).GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.DELV_DATE, x.PICK_TIME, x.PICK_ORD_NO, x.PICK_TOOL });

        foreach (var item in a)
				{
					if (item.Key.PICK_TOOL == "1")
					{
						// 單一揀貨紙本張數
						var singlePapers = proxyP05.CreateQuery<SinglePickingReportData>("GetSinglePickingReportDatas")
												.AddQueryExOption("dcCode", item.Key.DC_CODE)
												.AddQueryExOption("gupCode", item.Key.GUP_CODE)
												.AddQueryExOption("custCode", item.Key.CUST_CODE)
												.AddQueryExOption("delvDate", item.Key.DELV_DATE)
												.AddQueryExOption("pickTime", item.Key.PICK_TIME)
												.AddQueryExOption("pickOrdNo", item.Key.PICK_ORD_NO)
											 .ToList();
						SinglePickingReportDatas.AddRange(singlePapers);

						// 批量揀貨紙本張數
						var batchPapers = proxyP05.CreateQuery<BatchPickingReportData>("GetBatchPickingReportDatas")
												.AddQueryExOption("dcCode", item.Key.DC_CODE)
												.AddQueryExOption("gupCode", item.Key.GUP_CODE)
												.AddQueryExOption("custCode", item.Key.CUST_CODE)
												.AddQueryExOption("delvDate", item.Key.DELV_DATE)
												.AddQueryExOption("pickTime", item.Key.PICK_TIME)
												.AddQueryExOption("pickOrdNo", item.Key.PICK_ORD_NO)
											 .ToList();

						BatchPickingReportDatas.AddRange(batchPapers);
					}
					else
					{
						// 單一揀貨貼紙
						var singlePastes = proxyP05.CreateQuery<SinglePickingTickerData>("GetSinglePickingTickerDatas")
												 .AddQueryExOption("dcCode", item.Key.DC_CODE)
												 .AddQueryExOption("gupCode", item.Key.GUP_CODE)
												 .AddQueryExOption("custCode", item.Key.CUST_CODE)
												 .AddQueryExOption("delvDate", item.Key.DELV_DATE)
												 .AddQueryExOption("pickTime", item.Key.PICK_TIME)
												 .AddQueryExOption("pickOrdNo", item.Key.PICK_ORD_NO)
											.ToList();
						SinglePickingTickerDatas.AddRange(singlePastes);

						//批量揀貨貼紙
						var batchPastes = proxyP05.CreateQuery<BatchPickingTickerData>("GetBatchPickingTickerDatas")
												 .AddQueryExOption("dcCode", item.Key.DC_CODE)
												 .AddQueryExOption("gupCode", item.Key.GUP_CODE)
												 .AddQueryExOption("custCode", item.Key.CUST_CODE)
												 .AddQueryExOption("delvDate", item.Key.DELV_DATE)
												 .AddQueryExOption("pickTime", item.Key.PICK_TIME)
												 .AddQueryExOption("pickOrdNo", item.Key.PICK_ORD_NO)
											.ToList();
						BatchPickingTickerDatas.AddRange(batchPastes);
					}
				}
				DoPrintReport();
			}
      else if (SelectedTabIndex == 2)
      {
        SinglePickingReportDatas = new List<SinglePickingReportData>();
        SinglePickingTickerDatas = new List<SinglePickingTickerData>();
        BatchPickingReportDatas = new List<BatchPickingReportData>();
        BatchPickingTickerDatas = new List<BatchPickingTickerData>();

        var a = ReBatchPickNoList.Where(x => x.IsSelected).Select(x => x.Item).GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.DELV_DATE, x.PICK_TIME });

        foreach (var item in a)
        {
          // 單一揀貨紙本張數
          var singlePapers = proxyP05.CreateQuery<SinglePickingReportData>("GetSinglePickingReportDatasCheckNotRePick")
                      .AddQueryExOption("dcCode", item.Key.DC_CODE)
                      .AddQueryExOption("gupCode", item.Key.GUP_CODE)
                      .AddQueryExOption("custCode", item.Key.CUST_CODE)
                      .AddQueryExOption("delvDate", item.Key.DELV_DATE)
                      .AddQueryExOption("pickTime", item.Key.PICK_TIME)
                      .AddQueryExOption("pickOrdNo", "")
                      .AddQueryExOption("IsCheckNotRePick", true)

                     .ToList();
          SinglePickingReportDatas.AddRange(singlePapers);

          // 批量揀貨紙本張數
          var batchPapers = proxyP05.CreateQuery<BatchPickingReportData>("GetBatchPickingReportDatasCheckRePrint")
                      .AddQueryExOption("dcCode", item.Key.DC_CODE)
                      .AddQueryExOption("gupCode", item.Key.GUP_CODE)
                      .AddQueryExOption("custCode", item.Key.CUST_CODE)
                      .AddQueryExOption("delvDate", item.Key.DELV_DATE)
                      .AddQueryExOption("pickTime", item.Key.PICK_TIME)
                      .AddQueryExOption("pickOrdNo", "")
                      .AddQueryExOption("IsCheckNotRePick", true)

                     .ToList();

          BatchPickingReportDatas.AddRange(batchPapers);
          // 單一揀貨貼紙
          var singlePastes = proxyP05.CreateQuery<SinglePickingTickerData>("GetSinglePickingTickerDatasCheckRePrint")
                       .AddQueryExOption("dcCode", item.Key.DC_CODE)
                       .AddQueryExOption("gupCode", item.Key.GUP_CODE)
                       .AddQueryExOption("custCode", item.Key.CUST_CODE)
                       .AddQueryExOption("delvDate", item.Key.DELV_DATE)
                       .AddQueryExOption("pickTime", item.Key.PICK_TIME)
                       .AddQueryExOption("pickOrdNo", "")
                      .AddQueryExOption("IsCheckNotRePick", true)

                    .ToList();
          SinglePickingTickerDatas.AddRange(singlePastes);

          //批量揀貨貼紙
          var batchPastes = proxyP05.CreateQuery<BatchPickingTickerData>("GetBatchPickingTickerDatasCheckRePrint")
                       .AddQueryExOption("dcCode", item.Key.DC_CODE)
                       .AddQueryExOption("gupCode", item.Key.GUP_CODE)
                       .AddQueryExOption("custCode", item.Key.CUST_CODE)
                       .AddQueryExOption("delvDate", item.Key.DELV_DATE)
                       .AddQueryExOption("pickTime", item.Key.PICK_TIME)
                       .AddQueryExOption("pickOrdNo", "")
                      .AddQueryExOption("IsCheckNotRePick", true)

                    .ToList();
          BatchPickingTickerDatas.AddRange(batchPastes);
        }
        DoPrintReport();
      }
    }

		public void DoPrintComplete()
		{
			SearchCommand.Execute(null);
		}
		#endregion

		#region 所選擇的Tab
		public int _selectedTabIndex;
		public int SelectedTabIndex
		{
			get { return _selectedTabIndex; }
			set { Set(() => SelectedTabIndex, ref _selectedTabIndex, value); }
		}
		#endregion

		public ICommand AutoUpdateCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAutoUpdate(), () => UserOperateMode == OperateMode.Query,
					c => DoAutoUpdateComplete()
					);
			}
		}

		public void DoAutoUpdate()
		{
			if (AutoUpdateStatus)
			{
				//暫停自動更新，按鈕顯示啟動自動更新
				AutoUpdateEnable = false;
				AutoUpdateStatus = false;
				AutoUpdateVisibility = true;
				AutoUpdateInfo = "";
				StopAutoUpdate(true);
			}
			else
			{
				//啟動自動更新，按鈕顯示暫停自動更新
				AutoUpdateEnable = true;
				AutoUpdateStatus = true;
				AutoUpdateVisibility = false;
				AutoUpdateInfo = $"此頁面2分鐘會自動更新，最後更新日{DateTime.Now}";
				CalcatePercentWithUpdPickTool();
				StartAutoUpdate();
				//DoSearch();
			}

		}

		public void CalcatePercentWithUpdPickTool()
		{
			var calcatePickPercentList = new List<wcf.CalcatePickPercent>();
			var changePickToolList = new List<wcf.ChangePickTool>();

			if (BatchPickNoList != null && BatchPickNoList.Any())
			{
				calcatePickPercentList = BatchPickNoList.Select(x => new wcf.CalcatePickPercent
				{
					DC_CODE = x.Item.DC_CODE,
					GUP_CODE = x.Item.GUP_CODE,
					CUST_CODE = x.Item.CUST_CODE,
					DELV_DATE = x.Item.DELV_DATE,
					PICK_TIME = x.Item.PICK_TIME,
					PDA_PICK_PERCENT = x.Item.PDA_PICK_PERCENT
				}).ToList();
			}

			if (RePickNoList != null && RePickNoList.Any())
			{
				changePickToolList = RePickNoList.Select(x => new wcf.ChangePickTool
				{
					DC_CODE = x.Item.DC_CODE,
					GUP_CODE = x.Item.GUP_CODE,
					CUST_CODE = x.Item.CUST_CODE,
					PICK_ORD_NO = x.Item.PICK_ORD_NO,
					PICK_TOOL = x.Item.PICK_TOOL
				}).ToList();
			}

			if (calcatePickPercentList.Any() || changePickToolList.Any())
			{
				var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
				var result = proxy.RunWcfMethod(w => w.CalcatePercentWithUpdPickTool(SelectedDcCode.Value, _gupCode, _custCode, calcatePickPercentList.ToArray(), changePickToolList.ToArray()));

				if (result.IsSuccessed)
					ShowMessage(Messages.InfoUpdateSuccess);
			}
		}

		public void DoAutoUpdateComplete()
		{
		}


	}
}
