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
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using ex19 = Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P16WcfService;
using Wms3pl.WpfClient.Services;
using System.Data;
using Wms3pl.WpfClient.P16.Views;

namespace Wms3pl.WpfClient.P16.ViewModel
{
    public partial class P1604010000_ViewModel : InputViewModelBase
	{
        public Action ExcelImport = delegate { };
        public P1604010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				Init();
			}
		}

		#region Property,Field

		public string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		public string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }

		#region 單據狀態
		private List<NameValuePair<string>> _statusList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> StatusList { get { return _statusList; } set { _statusList = value; RaisePropertyChanged("StatusList"); } }

		private string _selectedStatus = string.Empty;
		/// <summary>
		/// 選取的單據狀態
		/// </summary>
		public string SelectedStatus
		{
			get { return _selectedStatus; }
			set
			{
				_selectedStatus = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region DC 參數
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
				RaisePropertyChanged();
			}
		}
		private List<NameValuePair<string>> _dcList = new List<NameValuePair<string>>();
		/// <summary>
		/// 物流中心選項
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.OrderBy(x => x.Value).ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}
		#endregion

		#region 報廢原因
		private string _selectedScrapReson = string.Empty;
		/// <summary>
		/// 選取的報廢原因
		/// </summary>
		public string SelectedScrapReson
		{
			get { return _selectedScrapReson; }
			set
			{
				_selectedScrapReson = value;
			}
		}
		private List<NameValuePair<string>> _scrapResonList = new List<NameValuePair<string>>();
		/// <summary>
		/// 報廢原因選項
		/// </summary>
		public List<NameValuePair<string>> ScrapResonList
		{
			get { return _scrapResonList.OrderBy(x => x.Value).ToList(); }
			set { _scrapResonList = value; RaisePropertyChanged("ScrapResonList"); }
		}

		#endregion

		#region 倉別
		private string _selectedWarehouse = string.Empty;
		/// <summary>
		/// 選取的倉別
		/// </summary>
		public string SelectedWarehouse
		{
			get { return _selectedWarehouse; }
			set
			{
				_selectedWarehouse = value;
				RaisePropertyChanged();
			}
		}
		private List<NameValuePair<string>> _warehouseList = new List<NameValuePair<string>>();
		/// <summary>
		/// 倉別選項
		/// </summary>
		public List<NameValuePair<string>> WarehouseList
		{
			get { return _warehouseList.OrderBy(x => x.Value).ToList(); }
			set { _warehouseList = value; RaisePropertyChanged("WarehouseList"); }
		}

		#endregion

		#region 是否全選 參數

		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				CheckSelectedAll(_isCheckAll, ScrapDetailList);
				RaisePropertyChanged();
			}
		}
		#endregion
		#region 匯入檔案路徑參數

		private string _importFilePath;

		public string ImportFilePath
		{
			get { return _importFilePath; }
			set
			{
				_importFilePath = value;
				RaisePropertyChanged("ImportFilePath");
			}
		}
		#endregion

		#region 新增(修改)-副檔(明細) 參數

		private F160402Data _currentDetailRecord;

		public F160402Data CurrentDetailRecord
		{
			get { return _currentDetailRecord; }
			set
			{
				_currentDetailRecord = value;
				RaisePropertyChanged();
			}
		}
        #endregion

        #region Excel匯入用

        private F160402Data _currentDetailExcel;

        public F160402Data CurrentDetailExcel
        {
            get { return _currentDetailExcel; }
            set
            {
                _currentDetailExcel = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region 報廢單主檔清單
        private List<F160401> _scrapList;
		public List<F160401> ScrapList { get { return _scrapList; } set { _scrapList = value; RaisePropertyChanged(); } }

		private F160401 _selectedScrap;
		public F160401 SelectedScrap
		{
			get { return _selectedScrap; }
			set
			{
				_selectedScrap = value;
				SearchDetailCommand.Execute(null);
				SearchAllocationCommand.Execute(null);
				RaisePropertyChanged();
			}
		}
		private F160401 _currentScrap;
		public F160401 CurrentScrap
		{
			get { return _currentScrap; }
			set { _currentScrap = value; RaisePropertyChanged(); }
		}
		#endregion
		#region 報廢單明細清單
		private SelectionList<F160402Data> _scrapDetailList;
		public SelectionList<F160402Data> ScrapDetailList { get { return _scrapDetailList; } set { _scrapDetailList = value; RaisePropertyChanged(); } }

		private SelectionItem<F160402Data> _selectedScrapDetail;
		public SelectionItem<F160402Data> SelectedScrapDetail
		{
			get { return _selectedScrapDetail; }
			set
			{
				_selectedScrapDetail = value;
				RaisePropertyChanged();
			}
		}
		#endregion
		#region 調撥單明細
		private List<F151001> _allocationList;
		public List<F151001> AllocationList { get { return _allocationList; } set { _allocationList = value; RaisePropertyChanged(); } }
		#endregion

		#region 報廢單新增明細來源資料
		private List<F160402AddData> _scrapAddDetailList;
		public List<F160402AddData> ScrapAddDetailList { get { return _scrapAddDetailList; } set { _scrapAddDetailList = value; RaisePropertyChanged(); } }
		#endregion

		#region UI 連動 binding
		private bool _searchResultIsExpanded = true;

		public bool SearchResultIsExpanded
		{
			get { return _searchResultIsExpanded; }
			set
			{
				_searchResultIsExpanded = value;
				RaisePropertyChanged("SearchResultIsExpanded");
			}
		}

		//外層群組標題
		private string _quoteHeaderText;

		public string QuoteHeaderText
		{
			get { return _quoteHeaderText; }
			set
			{
				_quoteHeaderText = value;
				RaisePropertyChanged("QuoteHeaderText");
			}
		}
		#endregion

		#region 建立日期-起
		private DateTime _crtDateStart = DateTime.Now;
		public DateTime CrtDateStart { get { return _crtDateStart; } set { _crtDateStart = value; RaisePropertyChanged(); } }
		#endregion
		#region 建立日期-迄
		private DateTime _crtDateEnd = DateTime.Now;
		public DateTime CrtDateEnd { get { return _crtDateEnd; } set { _crtDateEnd = value; RaisePropertyChanged(); } }
		#endregion
		#region 報廢單號
		private string _scrapNo = string.Empty;
		public string ScrapNo { get { return _scrapNo; } set { _scrapNo = value; RaisePropertyChanged(); } }
		#endregion
		#region 貨主單號
		private string _custOrdNo = string.Empty;
		public string CustOrdNo { get { return _custOrdNo; } set { _custOrdNo = value; RaisePropertyChanged(); } }
		#endregion
		#region 總計明細筆數
		private string _totalDetailCount = string.Empty;
		public string TotalDetailCount
		{
			get
			{
				return (string.IsNullOrEmpty(_totalDetailCount)) ? string.Empty : string.Format(Properties.Resources.P1604010000_TotalCount, _totalDetailCount);
			}
			set { _totalDetailCount = value; RaisePropertyChanged(); }
		}
		#endregion

		private bool _isEditScrapDetails;
		/// <summary>
		/// 是否編輯報廢明細中
		/// </summary>
		public bool IsEditScrapDetails
		{
			get { return _isEditScrapDetails; }
			set
			{
				Set(() => IsEditScrapDetails, ref _isEditScrapDetails, value);
			}
		}


		#endregion  Property,Field

		#region Funcion

		#region 初始化
		private void Init()
		{
			DcList = GetDcList();
			ScrapResonList = GetScrapResonList();
			WarehouseList = GetWarehouseList();
			StatusList = DoSearchStatusList();
			QuoteHeaderText = GetQuoteHeaderText();
		}
		#endregion

		#region 取得單據狀態
		private List<NameValuePair<string>> DoSearchStatusList()
		{
			var proxy = GetProxy<F00Entities>();
			var results = proxy.F000904s.Where(x => x.TOPIC == "F160401" && x.SUBTOPIC == "STATUS")
											.OrderBy(x => x.VALUE)
											.Select(x => new NameValuePair<string>() { Name = x.NAME, Value = x.VALUE })
											.ToList();
			if (results != null)
				results.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });

			if (results != null && results.Any())
				SelectedStatus = results.FirstOrDefault().Value;
			return results;
		}
		#endregion

		#region 取物流中心資料

		public List<NameValuePair<string>> GetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (data != null && data.Any())
				SelectedDc = data.FirstOrDefault().Value;
			return data;
		}
		#endregion

		#region 取倉別資料
		private List<NameValuePair<string>> GetWarehouseList()
		{

			var proxyP19Ex = GetExProxy<ex19.P19ExDataSource>();
			var data = proxyP19Ex.CreateQuery<ex19.F1912WareHouseData>("GetCustWarehouseDatas")
										.AddQueryExOption("dcCode", SelectedDc)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
                                        .Where(x=>!x.WAREHOUSE_ID.StartsWith("D")) // 排除WAREHOUSE_ID 是D開頭的倉
                                        .OrderBy(x => x.WAREHOUSE_ID)
										.Select(o => new NameValuePair<string>()
										{
											Name = o.WAREHOUSE_NAME,
											Value = o.WAREHOUSE_ID
										}).ToList();


			if (data != null && data.Any())
				SelectedWarehouse = data.FirstOrDefault().Value;
			return data;
		}
		#endregion
		#region 取報廢原因資料
		private List<NameValuePair<string>> GetScrapResonList()
		{
			var proxy = GetProxy<F19Entities>();

			var data = proxy.F1951s.Where(x => x.UCT_ID == "SC")
														 .OrderBy(x => x.UCC_CODE)
														 .Select(x => new NameValuePair<string>() { Name = x.CAUSE, Value = x.UCC_CODE })
											.ToList();
			if (data != null && data.Any())
				SelectedScrapReson = data.FirstOrDefault().Value;
			return data;
		}
		#endregion
		#region Grid Checkbox 全選 -Add
		public void CheckSelectedAll(bool isChecked, SelectionList<F160402Data> dgData)
		{
			foreach (var dgDataItem in dgData)
				dgDataItem.IsSelected = isChecked;
		}

		#endregion

		#region 取群組標題
		private string GetQuoteHeaderText()
		{
			switch (UserOperateMode)
			{
				case OperateMode.Edit:
					return Properties.Resources.P1604010000_ScrapItem;
				case OperateMode.Add:
					return Properties.Resources.DESTROY_ITEM;
				default:
					return Properties.Resources.SCRAP_DETAIL;
			}
		}
		#endregion

		#region 查詢儲位帶入存檔

		public void ScrapDetailSaveByLoc(List<F160402AddData> ScrapDetailByLoc)
		{
			var tmpLocList = ExDataMapper.MapCollection<F160402AddData, F160402Data>(ScrapDetailByLoc).ToList();
			var tmpOriList = (ScrapDetailList != null) ? ExDataMapper.MapCollection<F160402Data, F160402Data>(ScrapDetailList.Select(si => si.Item)).ToList() : new List<F160402Data>();
            string errMsg = string.Empty;
            foreach (var item in tmpLocList)
            {              
                var errMsgTemp = CheckStock(item);
                errMsg = string.IsNullOrWhiteSpace(errMsgTemp) ? errMsg : (string.IsNullOrWhiteSpace(errMsg) ? errMsgTemp : string.Format("{0}{1}{2}", errMsg, "\n", errMsgTemp));
                if (string.IsNullOrWhiteSpace(errMsgTemp))
                {
                    AddOriScrap(ref tmpOriList, item);
                    ScrapDetailList = tmpOriList.ToSelectionList();
                }
            }
            if (!string.IsNullOrWhiteSpace(errMsg))
                ShowMessage(new MessagesStruct() { Message = errMsg, Title = Resources.Resources.Information });
            //SaveCommand.Execute("PASS");
        }
        #endregion

        /// <summary>
        /// 檢核報廢數量是否超過庫存
        /// </summary>
        /// <param name="itemCode">商品代碼</param>
        /// <param name="locCode">儲位</param>
        /// <param name="warehouseID">倉別</param>
        /// <param name="allQTY">庫存總數</param>
        /// <param name="scrapQTY">報廢數</param>
        /// <param name="isModifyConfirm">是否為確認修改</param>
        /// <returns>超過報廢數量的品號、儲位、倉別的訊息</returns>
        public string CheckStock(F160402Data item, bool isModifyConfirm = false)
        {
            string result = string.Empty;

            var proxy = new wcf.P16WcfServiceClient();

            var scrapNo = (SelectedScrap == null) ? string.Empty : SelectedScrap.SCRAP_NO;

            var tempList = new List<F160402Data>() { item };

            //取得所有Grid中已加入的明細
            var tmpList = (ScrapDetailList != null) ? ExDataMapper.MapCollection<F160402Data, F160402Data>(ScrapDetailList.Select(si => si.Item)).ToList() : new List<F160402Data>();

            //本次已加入之全部報廢資料
            var totalScrapData = tmpList.Where(x => x.ITEM_CODE == item.ITEM_CODE &&
                                                     x.LOC_CODE == item.LOC_CODE &&
                                             x.WAREHOUSE_ID == item.WAREHOUSE_ID &&
                                             x.VALID_DATE == item.VALID_DATE && x.BOX_CTRL_NO == item.BOX_CTRL_NO && x.PALLET_CTRL_NO == item.PALLET_CTRL_NO &&
                                             x.MAKE_NO == item.MAKE_NO
                                             ).ToList();

            //本次已加入之全部報廢數
            var totalScrapQty = totalScrapData.Sum(x => x.SCRAP_QTY);

            //其它已加入F160402的報廢詳細資料(其他待處理的報廢單)  
            var alreadyScrapAllData = ExDataMapper.MapCollection<wcf.F160402, F160402>(RunWcfMethod(proxy.InnerChannel, () => proxy.GetF160402ScrapData(
                                                                       item.DC_CODE,
                                                                       item.GUP_CODE,
                                                                       item.CUST_CODE,
                                                                       item.SCRAP_NO,
                                                                       item.ITEM_CODE,
                                                                       item.LOC_CODE,
                                                                       item.WAREHOUSE_ID).ToList()).ToArray());

            //其它已加入F160402的報廢數(其他待處理的報廢單)
            var alreadyScrapAllQty = alreadyScrapAllData.Where(x => x.ITEM_CODE == item.ITEM_CODE &&
                                                               x.LOC_CODE == item.LOC_CODE &&
                                                               x.WAREHOUSE_ID == item.WAREHOUSE_ID &&
                                                               x.VALID_DATE == item.VALID_DATE && x.BOX_CTRL_NO == item.BOX_CTRL_NO && x.PALLET_CTRL_NO == item.PALLET_CTRL_NO &&
                                                               x.MAKE_NO == item.MAKE_NO
                                                               ).Sum(x => x.SCRAP_QTY);

            //總庫存數(該唯一商品)
            var totalQty = item.QTY;
            //本次欲加入之報廢數
            var ScrapQty = item.SCRAP_QTY;

            if(isModifyConfirm)
            {
                if ((totalQty - totalScrapQty - alreadyScrapAllQty) < 0)
                {
                    var errMsg = string.Format(Properties.Resources.P1604010000_Item_Loc_Warehouse_StockQtyInSufficient,
                        item.ITEM_CODE,
                        item.LOC_CODE,
                        item.WAREHOUSE_ID);
                    result = errMsg;
                }
            }
            else
            {
                if ((totalQty - totalScrapQty - alreadyScrapAllQty - ScrapQty) < 0)
                {
                    var errMsg = string.Format(Properties.Resources.P1604010000_Item_Loc_Warehouse_StockQtyInSufficient,
                        item.ITEM_CODE,
                        item.LOC_CODE,
                        item.WAREHOUSE_ID);
                    result = errMsg;
                }
            }
           

            return result;
        }

		#endregion

		#region command

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearch(o), () => UserOperateMode == OperateMode.Query
						);
			}
		}

		private void DoSearch(object o)
		{
			//執行查詢動作
			string scrapNo = null;
			string selectScrapNo = null;
            if (o != null)
            {
                scrapNo = ((Dictionary<string, string>)o)["scrapNo"];
                selectScrapNo = ((Dictionary<string, string>)o)["selectScrapNo"];
            }
            else
            {
                scrapNo = ScrapNo;
            }
			ScrapList = GetF160401Datas(scrapNo, selectScrapNo);
		}

		private List<F160401> GetF160401Datas(string scrapNo, string selectScrapNo)
		{
			var proxy = GetProxy<F16Entities>();
			var f160401s = proxy.F160401s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode &&
																							 ((CrtDateStart == null) || x.CRT_DATE >= CrtDateStart.Date) &&
																							 ((CrtDateEnd == null) || x.CRT_DATE <= CrtDateEnd.Date.AddDays(1)) &&
																							 (x.SCRAP_NO == scrapNo || string.IsNullOrEmpty(scrapNo)) &&
																							 (x.CUST_ORD_NO == CustOrdNo || string.IsNullOrEmpty(CustOrdNo)) &&
																							 (x.STATUS == SelectedStatus || (string.IsNullOrEmpty(SelectedStatus) && x.STATUS != "9")))
																	 .ToList();
			if (f160401s == null || !f160401s.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return null;
			}
			SelectedScrap = (string.IsNullOrEmpty(selectScrapNo)) ? f160401s.FirstOrDefault() : f160401s.SingleOrDefault(x => x.SCRAP_NO == selectScrapNo);
			return f160401s;
		}

		#endregion Search

		#region SearchDetail
		public ICommand SearchDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearchDetail(), () => UserOperateMode == OperateMode.Query,
						o => DoSearchDetailComplete()
						);
			}
		}

		private void DoSearchDetailComplete()
		{
			SelectedScrapDetail = null;
		}

		private void DoSearchDetail()
		{
			//執行查詢動作
			ScrapDetailList = null;
			if (SelectedScrap != null)
				ScrapDetailList = GetF160402Datas(SelectedScrap);
		}

		private SelectionList<F160402Data> GetF160402Datas(F160401 selectedScrap)
		{
			var proxyEx = GetExProxy<P16ExDataSource>();
			var results = proxyEx.CreateQuery<F160402Data>("GetF160402ScrapDetails")
						.AddQueryExOption("dcCode", SelectedDc)
						.AddQueryExOption("gupCode", _gupCode)
						.AddQueryExOption("custCode", _custCode)
						.AddQueryExOption("scrapNo", selectedScrap.SCRAP_NO)
						.ToSelectionList();
			return results;
		}

		private void SetCurrentDetailRecord()
		{
			CurrentDetailRecord = new F160402Data()
			{
				DC_CODE = (SelectedScrap != null) ? SelectedScrap.DC_CODE : SelectedDc,
				GUP_CODE = (SelectedScrap != null) ? SelectedScrap.GUP_CODE : _gupCode,
				CUST_CODE = (SelectedScrap != null) ? SelectedScrap.CUST_CODE : _custCode
			};
		}
		#endregion

		#region SearchAllocation
		public ICommand SearchAllocationCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearchAllocation(), () => UserOperateMode == OperateMode.Query
						);
			}
		}

		private void DoSearchAllocation()
		{
			//執行查詢動作
			AllocationList = null;
			if (SelectedScrap != null)
			{
				AllocationList = GetF151001Datas(SelectedScrap);
			}
		}

		private List<F151001> GetF151001Datas(F160401 selectedScrap)
		{
			var proxy = GetProxy<F15Entities>();
			var results = proxy.F151001s.Where(x => x.DC_CODE == selectedScrap.DC_CODE && x.GUP_CODE == selectedScrap.GUP_CODE &&
																							x.CUST_CODE == selectedScrap.CUST_CODE && x.SOURCE_NO == selectedScrap.SCRAP_NO)
																	.AsQueryable()
																	.ToList();
			return results;
		}
		#endregion

		#region SearchOri
		public ICommand SearchOriCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => DoSearchOri(o));
			}
		}

		private bool DoSearchOri(object parameter = null,bool isExcelImport = false)
		{
            F160402Data CurrentDetailTemp = null;
            if (isExcelImport)
                CurrentDetailTemp = CurrentDetailExcel;
            else
                CurrentDetailTemp = CurrentDetailRecord;
           

            //執行查詢動作
            if (CurrentDetailTemp == null)
				return false;

			if (!SetItemName(parameter))
			{
				return false;
			}

			if (string.IsNullOrEmpty(CurrentDetailTemp.ITEM_CODE)
				|| string.IsNullOrEmpty(CurrentDetailTemp.LOC_CODE)
				|| string.IsNullOrEmpty(CurrentDetailTemp.WAREHOUSE_ID))
			{
				return false;
			}

            CurrentDetailTemp.LOC_CODE = LocCodeHelper.LocCodeConverter9(CurrentDetailTemp.LOC_CODE);
            ScrapAddDetailList = GetF160402AddDatas(CurrentDetailTemp.ITEM_CODE, CurrentDetailTemp.LOC_CODE, CurrentDetailTemp.WAREHOUSE_ID);

			if (ScrapAddDetailList == null || !ScrapAddDetailList.Any())
			{
				ShowWarningMessage(string.Format(Properties.Resources.P1604010000_Item_Loc_Warehouse,
                    CurrentDetailTemp.ITEM_CODE,
                    CurrentDetailTemp.LOC_CODE,
					WarehouseList.Where(x => x.Value == CurrentDetailTemp.WAREHOUSE_ID).Select(x => x.Name).FirstOrDefault(),
					Messages.InfoNoData.Message));

                CurrentDetailTemp.ALL_QTY = 0;
				return false;
			}

			//設定報廢明細

			var f160402Data = ExDataMapper.Map<F160402AddData, F160402Data>(ScrapAddDetailList.FirstOrDefault());
			f160402Data.SCRAP_CAUSE = CurrentDetailTemp.SCRAP_CAUSE;
			f160402Data.SCRAP_QTY = CurrentDetailTemp.SCRAP_QTY;
            CurrentDetailTemp = f160402Data;
            if (isExcelImport)
                CurrentDetailExcel = CurrentDetailTemp;
            else
                CurrentDetailRecord = CurrentDetailTemp;

            return true;
		}

		private static bool IsFromItemCodeRaise(object parameter)
		{
			return Convert.ToString(parameter) == "ItemCode";
		}

		private bool SetItemName(object parameter)
		{
			if (IsFromItemCodeRaise(parameter))
			{
				CurrentDetailRecord.ALL_QTY = 0;
				CurrentDetailRecord.ITEM_NAME = string.Empty;
				if (!string.IsNullOrEmpty(CurrentDetailRecord.ITEM_CODE))
				{
					var f1903 = GetProxy<F19Entities>().F1903s.Where(x => x.ITEM_CODE == CurrentDetailRecord.ITEM_CODE && x.GUP_CODE == _gupCode && x.CUST_CODE == CurrentDetailRecord.CUST_CODE).FirstOrDefault();
					if (f1903 == null)
					{
						ShowMessage(Messages.InfoNoData);
					}
					else
					{
						CurrentDetailRecord.ITEM_NAME = f1903.ITEM_NAME;
						return true;
					}
				}

				return false;
			}

			return true;
		}

		private List<F160402AddData> GetF160402AddDatas(string itemCode, string locCode, string wareHouseId)
		{
			var proxyEx = GetExProxy<P16ExDataSource>();
			var results = proxyEx.CreateQuery<F160402AddData>("GetF160402AddScrapDetails")
						.AddQueryExOption("dcCode", SelectedDc)
						.AddQueryExOption("gupCode", _gupCode)
						.AddQueryExOption("custCode", _custCode)
						.AddQueryExOption("wareHouseId", wareHouseId)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("locCode", locCode)
						.ToList();
			return results;
		}
		#endregion

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAdd(), () => UserOperateMode == OperateMode.Query,
						o => DoAddComplete()
						);
			}
		}

		private void DoAddComplete()
		{
			CurrentScrap = new F160401()
			{
				DC_CODE = SelectedDc,
				GUP_CODE = _gupCode,
				CUST_CODE = _custCode
			};
			CurrentDetailRecord = new F160402Data()
			{
				DC_CODE = CurrentScrap.DC_CODE,
				GUP_CODE = CurrentScrap.GUP_CODE,
				CUST_CODE = CurrentScrap.CUST_CODE
			};
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作

			QuoteHeaderText = GetQuoteHeaderText();

			ScrapDetailList = null;
            SelectedScrap = null;
        }
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoEdit(),
						() =>
						{
							return (UserOperateMode == OperateMode.Query && SelectedScrap != null &&
											SelectedScrap.STATUS == "0");
						}
						);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
			QuoteHeaderText = GetQuoteHeaderText();
			if (SelectedScrapDetail == null)
				CurrentDetailRecord = new F160402Data()
				{
					DC_CODE = SelectedScrap.DC_CODE,
					GUP_CODE = SelectedScrap.GUP_CODE,
					CUST_CODE = SelectedScrap.CUST_CODE,
					SCRAP_NO = SelectedScrap.SCRAP_NO
				};
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
			CurrentDetailRecord = null;
            if (UserOperateMode == OperateMode.Add)
            {
                ScrapList = null;
                ScrapDetailList = null;
            }
            else
                SearchCommand.Execute(null);
			UserOperateMode = OperateMode.Query;
			QuoteHeaderText = GetQuoteHeaderText();
			CurrentScrap = null;
			IsEditScrapDetails = false;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedScrap != null &&
																	 SelectedScrap.STATUS == "0"
						);
			}
		}

		private void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
				return;

			var proxy = GetExProxy<P16ExDataSource>();
			var query = proxy.CreateQuery<ExecuteResult>("DeleteP160401")
											 .AddQueryExOption("dcCode", SelectedScrap.DC_CODE)
											 .AddQueryExOption("gupCode", SelectedScrap.GUP_CODE)
											 .AddQueryExOption("custCode", SelectedScrap.CUST_CODE)
											 .AddQueryExOption("scrapNo", SelectedScrap.SCRAP_NO);

			var result = query.ToList().FirstOrDefault();
			ShowResultMessage(result);
            ScrapNo = string.Empty;
			DoSearch(null);
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				var result = new wcf.ExecuteResult();
				return CreateBusyAsyncCommand(
					(p) => result = DoSave(p), () => UserOperateMode != OperateMode.Query && !IsEditScrapDetails,
					o => DoSaveComplete(result)
					);
			}
		}

		private void DoSaveComplete(wcf.ExecuteResult result)
		{
			if (result != null && result.IsSuccessed)
			{
				var scrapNo = result.Message;
				Dictionary<string, string> dicScrap = new Dictionary<string, string>();
                dicScrap.Add("scrapNo", scrapNo);
                dicScrap.Add("selectScrapNo", scrapNo);
                ScrapNo = scrapNo;

                UserOperateMode = OperateMode.Query;
				QuoteHeaderText = GetQuoteHeaderText();
				ShowMessage(Messages.Success);
				CurrentDetailRecord = null;
				SearchCommand.Execute(dicScrap);
				IsEditScrapDetails = false;
			}
			CurrentScrap = null;
		}

		private wcf.ExecuteResult DoSave(object p)
		{
			//執行確認儲存動作
			if (p == null)
			{
				if (ShowMessage(Messages.WarningBeforeSave) != DialogResponse.Yes) return null;
			}

            wcf.ExecuteResult result = new wcf.ExecuteResult() { Message = Properties.Resources.P1601020000_NotWork };

            if (ScrapDetailList != null && ScrapDetailList.Any())
            {
                var proxy = new wcf.P16WcfServiceClient();
                var f160401 = (UserOperateMode == OperateMode.Add) ? ExDataMapper.Map<F160401, wcf.F160401>(CurrentScrap) : ExDataMapper.Map<F160401, wcf.F160401>(SelectedScrap);
                var f160402s = ExDataMapper.MapCollection<F160402Data, wcf.F160402>(ScrapDetailList.Select(item => item.Item)).ToArray();

                result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                    () => proxy.SaveScrapDetails(f160401, f160402s));


                if (!result.IsSuccessed)
                {
                    DialogService.ShowMessage(result.Message);
                }
            }

			return result;
		}


		#endregion Save

		#region Approve
		public ICommand ApproveCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoApprove(),
					() => UserOperateMode == OperateMode.Edit
						&& SelectedScrap != null
						&& !IsEditScrapDetails
						&& ScrapDetailList != null
						&& ScrapDetailList.Any(),
					o => DoApproveComplete(isSuccess)
					);
			}
		}

		private void DoApproveComplete(bool isSuccess)
		{
			if (isSuccess)
			{
				UserOperateMode = OperateMode.Query;
				QuoteHeaderText = GetQuoteHeaderText();
				ShowMessage(Messages.Success);
				Dictionary<string, string> dicScrap = new Dictionary<string, string>();
				dicScrap.Add("scrapNo", string.Empty);
				dicScrap.Add("selectScrapNo", SelectedScrap.SCRAP_NO);
				CurrentDetailRecord = null;
				SearchCommand.Execute(dicScrap);
			}
			CurrentScrap = null;
		}

		private bool DoApprove()
		{
			//1.執行確認核准動作
			if (ShowMessage(new MessagesStruct() { Button=DialogButton.YesNo,Image=DialogImage.Warning, Message = Properties.Resources.WarningApproveSave, Title = Resources.Resources.Warning }) != DialogResponse.Yes) return false;
			var proxy = new wcf.P16WcfServiceClient();
			var f160401 = (UserOperateMode == OperateMode.Add) ? ExDataMapper.Map<F160401, wcf.F160401>(CurrentScrap) : ExDataMapper.Map<F160401, wcf.F160401>(SelectedScrap);
			var f160402s = ExDataMapper.MapCollection<F160402Data, wcf.F160402>(ScrapDetailList.Select(item => item.Item)).ToArray();

			var srcItemLocQtyItems = ScrapDetailList.Select(item => new wcf.SrcItemLocQtyItem()
			{
				ItemCode = item.Item.ITEM_CODE,
				LocCode = item.Item.LOC_CODE,
				Qty = item.Item.SCRAP_QTY,
				WarehouseId = item.Item.WAREHOUSE_ID,
                VALID_DATE = item.Item.VALID_DATE,
                PalletCtrlNo = item.Item.PALLET_CTRL_NO,
                BoxCtrlNo = item.Item.BOX_CTRL_NO,
                MakeNo = item.Item.MAKE_NO 
			}).ToArray();

			wcf.ExecuteResult result = new wcf.ExecuteResult() { Message = Properties.Resources.P1601020000_NotWork };

			result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
				() => proxy.ApproveScrapDetails(f160401, f160402s, srcItemLocQtyItems));


			if (!result.IsSuccessed)
			{
				DialogService.ShowMessage(result.Message);
			}

			return result.IsSuccessed;
		}
		#endregion

		#region AddDetail
		public ICommand AddDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAddDetail(), () =>
							{
								return (UserOperateMode != OperateMode.Query &&
											CurrentDetailRecord != null &&
											!IsEditScrapDetails &&
											!string.IsNullOrEmpty(CurrentDetailRecord.ITEM_CODE) &&
											!string.IsNullOrEmpty(CurrentDetailRecord.LOC_CODE) &&
											!string.IsNullOrEmpty(CurrentDetailRecord.WAREHOUSE_ID) &&
											!string.IsNullOrEmpty(CurrentDetailRecord.SCRAP_CAUSE) &&
											CurrentDetailRecord.ALL_QTY > 0 &&
											CurrentDetailRecord.SCRAP_QTY > 0);
							},
							o => SetCurrentDetailRecord()
						);
			}
		}

        /// <summary>
        /// Excel匯入、新增都是跑這方法
        /// </summary>
        /// <returns></returns>
		private bool DoAddDetail(bool isExcelImport = false)
		{
            F160402Data CurrentDetailTemp = null;
            if (isExcelImport)
                CurrentDetailTemp = CurrentDetailExcel;
            else
                CurrentDetailTemp = CurrentDetailRecord;

            if (CurrentDetailTemp == null || CurrentDetailTemp.SCRAP_QTY == 0)
			{
				var errMsg = Properties.Resources.P1604010000_ScrapDataNull;
				ShowMessage(new MessagesStruct() { Message = errMsg, Title = Resources.Resources.Information });
				return false;
			}

            var proxy = new wcf.P16WcfServiceClient();

            var scrapNo = (SelectedScrap == null) ? string.Empty : SelectedScrap.SCRAP_NO;

            //取得所有Grid中已加入的明細
            var tmpList = (ScrapDetailList != null) ? ExDataMapper.MapCollection<F160402Data, F160402Data>(ScrapDetailList.Select(si => si.Item)).ToList() : new List<F160402Data>();
           
            //本次已加入之全部報廢數(以將要加入的資料作為查詢計算)
            var totalScrapQty = tmpList.Where(x => x.ITEM_CODE == CurrentDetailTemp.ITEM_CODE &&
                                                   x.LOC_CODE == CurrentDetailTemp.LOC_CODE &&
                                                   x.WAREHOUSE_ID == CurrentDetailTemp.WAREHOUSE_ID)
                                                   .Sum(x => x.SCRAP_QTY);
          
            //其它已加入F160402的報廢詳細資料(其他待處理的報廢單)  
            var alreadyScrapAllData = ExDataMapper.MapCollection<wcf.F160402, F160402>(RunWcfMethod(proxy.InnerChannel,() => proxy.GetF160402ScrapData(
                                                                      CurrentDetailTemp.DC_CODE,
                                                                      CurrentDetailTemp.GUP_CODE,
                                                                      CurrentDetailTemp.CUST_CODE,
                                                                      scrapNo,
                                                                      CurrentDetailTemp.ITEM_CODE,
                                                                      CurrentDetailTemp.LOC_CODE,
                                                                      CurrentDetailTemp.WAREHOUSE_ID).ToList()).ToArray());
                       
            //其它已加入F160402的報廢數(其他待處理的報廢單)
            var alreadyScrapAllQty = alreadyScrapAllData.Sum(o => o.SCRAP_QTY);

            //總庫存數(該儲位不分板、箱、批號的加總)
            var totalQty = CurrentDetailTemp.ALL_QTY;
           
            //本次欲加入之報廢數
            var ScrapQty = CurrentDetailTemp.SCRAP_QTY;
          
            //計算這次加入的數量，會不會超過庫存的總數
            if ((totalQty - totalScrapQty - alreadyScrapAllQty - ScrapQty) < 0)
            {
                var errMsg = string.Format(Properties.Resources.P1604010000_Item_Loc_Warehouse_StockQtyInSufficient,
                    CurrentDetailTemp.ITEM_CODE,
                    CurrentDetailTemp.LOC_CODE,
                    CurrentDetailTemp.WAREHOUSE_ID);
                ShowMessage(new MessagesStruct() { Message = errMsg, Title = Resources.Resources.Information });
                return false;
            }
            //從已撈出的來源清單中,依序(效期)去挑出每筆項目庫存數,至達到總報廢數為止
            foreach (var item in ScrapAddDetailList)//ScrapAddDetailList查詢時所撈回來的該倉別該儲位該商品的所有詳細資料
            {
				//庫存數為0不處理
				if (item.QTY == 0) continue;
				var addF160402Data = ExDataMapper.Map<F160402AddData, F160402Data>(item);
				addF160402Data.SCRAP_CAUSE = CurrentDetailTemp.SCRAP_CAUSE;
				addF160402Data.SCRAP_NO = scrapNo;

                //唯一商品所用掉的總數(此單Grid中明細的)
                var scrapByItemQty = tmpList.Where(o => o.ITEM_CODE == item.ITEM_CODE && o.LOC_CODE == item.LOC_CODE && o.VALID_DATE == item.VALID_DATE &&
                o.MAKE_NO == item.MAKE_NO && o.BOX_CTRL_NO == item.BOX_CTRL_NO && o.PALLET_CTRL_NO == item.PALLET_CTRL_NO &&
                o.WAREHOUSE_ID == item.WAREHOUSE_ID).Sum(o => o.SCRAP_QTY);

                //其他報廢單的唯一商品所用掉的總數
                var scrapByAlreadyQty = alreadyScrapAllData.Where(o => o.ITEM_CODE == item.ITEM_CODE && o.LOC_CODE == item.LOC_CODE && o.VALID_DATE == item.VALID_DATE &&
                o.MAKE_NO == item.MAKE_NO && o.BOX_CTRL_NO == item.BOX_CTRL_NO && o.PALLET_CTRL_NO == item.PALLET_CTRL_NO &&
                o.WAREHOUSE_ID == item.WAREHOUSE_ID).Sum(o => o.SCRAP_QTY);

                //該商品剩下來的數量
                int hasQty = (int)item.QTY - scrapByItemQty - scrapByAlreadyQty;

                //用光了不處理，直接跳下一筆，不然會加入0銷毀的商品
                if (hasQty == 0)
                    continue;

                //這邊有問題 原因不同 刷讀  item.QTY 是該商品的總數(對到所有的key)
                if (ScrapQty > hasQty)
				{
					//item全撈
					addF160402Data.SCRAP_QTY = hasQty;
					AddOriScrap(ref tmpList, addF160402Data);
                    ScrapQty -= hasQty;
				}
				else
				{
					//ScrapQty全撈
					addF160402Data.SCRAP_QTY = ScrapQty;
					AddOriScrap(ref tmpList, addF160402Data);
					break;
				}
			}
			ScrapDetailList = tmpList.ToSelectionList();
			return true;
		}

		private void AddOriScrap(ref List<F160402Data> tmpList, F160402Data addF160402Data)
		{
			var SameScrap = tmpList.Where(x => x.DC_CODE == addF160402Data.DC_CODE &&
											x.GUP_CODE == addF160402Data.GUP_CODE && x.CUST_CODE == addF160402Data.CUST_CODE &&
											x.ITEM_CODE == addF160402Data.ITEM_CODE && x.LOC_CODE == addF160402Data.LOC_CODE &&
											x.VALID_DATE == addF160402Data.VALID_DATE &&	// x.SCRAP_NO == addF160402Data.SCRAP_NO &&	若是新增明細的就不會有SCRAP_NO
                                            x.MAKE_NO == addF160402Data.MAKE_NO && x.PALLET_CTRL_NO == addF160402Data.PALLET_CTRL_NO && x.BOX_CTRL_NO == addF160402Data.BOX_CTRL_NO &&
											x.WAREHOUSE_ID == addF160402Data.WAREHOUSE_ID && x.SCRAP_CAUSE == addF160402Data.SCRAP_CAUSE)
											.FirstOrDefault();
			if (SameScrap != null)
			{
                // 按下<確定>鍵，將所勾選之報廢商品帶回之新增畫面資料表中Grid，若已有明細存於資料表grid中，且數量不同，則於查詢子畫面中之設定數量，會蓋過原先資料表grid中該報廢商品之數量
                SameScrap.SCRAP_QTY += addF160402Data.SCRAP_QTY;
              
                return;
			}
			tmpList.Add(addF160402Data);
		}
		#endregion

		#region DeleteDetail
		public ICommand DeleteDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDeleteDetail(), () =>
						{
							return (UserOperateMode != OperateMode.Query &&
										!IsEditScrapDetails &&
										ScrapDetailList != null &&
										ScrapDetailList.Any() &&
										ScrapDetailList.Any(x => x.IsSelected));
						},
						o => DoDeleteDetailComplete()
						);
			}
		}

		private void DoDeleteDetailComplete()
		{
			if (ScrapDetailList != null && ScrapDetailList.Any())
				SelectedScrapDetail = ScrapDetailList.FirstOrDefault();
			else
			{
				SetCurrentDetailRecord();
			}
		}

		private void DoDeleteDetail()
		{

			// 刪除記錄
			if (ShowMessage(Messages.WarningBeforeDelete) != UILib.Services.DialogResponse.Yes) return;

			if (ScrapDetailList == null || !ScrapDetailList.Any() || !ScrapDetailList.Any(x => x.IsSelected))
				return;

			ScrapDetailList = ScrapDetailList.Where(x => !x.IsSelected).Select(si => si.Item).ToSelectionList();

			if (ScrapDetailList == null)
				ScrapDetailList = new SelectionList<F160402Data>(new List<F160402Data>());

			//將全選取消
			IsCheckAll = false;

			ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1604010000_DeleteItemDetail, Title = Resources.Resources.Information });
		}
		#endregion

		#region Import Excel

		public ICommand ImportExcelCommand
		{
            get
            {
                return new RelayCommand(() =>
                {
                    DispatcherAction(() =>
                    {
                        ExcelImport();
                        if (string.IsNullOrEmpty(ImportFilePath)) return;
                        DoImportCommand.Execute(null);
                    });
                });
            }
		}

        public ICommand DoImportCommand
        {
            get
            {
                var resultData = new List<F160402Data>();
                return CreateBusyAsyncCommand(
                    o => { resultData = DoImport(); },
                    () => !IsEditScrapDetails && UserOperateMode == OperateMode.Query
                    );
            }
        }
        public List<F160402Data> DoImport()
		{
			List<F160402Data> resultItem = new List<F160402Data>();
			string fullFilePath = ImportFilePath;
			var msg = new MessagesStruct()
			{
				Button = DialogButton.OK,
				Image = DialogImage.Information,
				Message = Properties.Resources.P1604010000_SaveSuccess,
				Title = Resources.Resources.Information
			};

			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);
			if (excelTable != null)
			{
				var dataTableColumnName = new List<string>
				{
					Properties.Resources.ITEM_CODE,
					Properties.Resources.WAREHOUSE_ID,
					Properties.Resources.SearchLoc_Code,
					Properties.Resources.P1604010000_ScrapCount,
					Properties.Resources.SCRAP_CAUSE
				};//該EXCEL 應有的Table Column Name
				var check = excelTable.Columns.Cast<DataColumn>()
					.All(column => dataTableColumnName.Contains(column.ColumnName.Trim()));
				if (!check)
				{
					msg = new MessagesStruct() { Message = Properties.Resources.P1604010000_ImportTableFormatError, Button = DialogButton.OKCancel, Image = DialogImage.Warning, Title = Resources.Resources.Information };
					ShowMessage(msg);
					return null;
				}
				var queryData = (from col in excelTable.AsEnumerable()
								 select new F160402Data
								 {
									 ITEM_CODE = Convert.ToString(col[0]),
									 WAREHOUSE_ID = Convert.ToString(col[1]),
									 LOC_CODE = LocCodeHelper.LocCodeConverter9(Convert.ToString(col[2])),
									 SCRAP_QTY = Convert.ToInt16(col[3]),
									 SCRAP_CAUSE = Convert.ToString(col[4])
								 }).ToList();
				if (queryData != null && queryData.Any())
				{
					foreach (var item in queryData)
					{
                        CurrentDetailExcel = ExDataMapper.Map<F160402Data, F160402Data>(item);
                        var isSuccess = DoSearchOri(null, true);

						if (isSuccess)
						{
                            CurrentDetailExcel.SCRAP_QTY = item.SCRAP_QTY;
                            CurrentDetailExcel.SCRAP_CAUSE = item.SCRAP_CAUSE;
							isSuccess = DoAddDetail(true);
						}
						if (!isSuccess)
						{
							SearchDetailCommand.Execute(null);
                            CurrentDetailExcel = null;
                            return null;
						}
					}
				}
			}
			else if (string.IsNullOrWhiteSpace(errorMeg))
			{
				msg = new MessagesStruct() { Message = Properties.Resources.P1604010000_NoData, Button = DialogButton.OKCancel, Image = DialogImage.Warning, Title = Resources.Resources.Information };
				ShowMessage(msg);
			}
			else
			{
				msg = new MessagesStruct() { Button = DialogButton.OKCancel, Image = DialogImage.Warning, Message = errorMeg, Title = Resources.Resources.Information };
				ShowMessage(msg);
			}
			return resultItem;
		}

		#endregion

		private ICommand _scrapLocSearchCommand;

		/// <summary>
		/// Gets the ScrapLocSearchCommand.
		/// </summary>
		public ICommand ScrapLocSearchCommand
		{
			get
			{
				List<F160402AddData> scrapAddDetailList = null;
				return _scrapLocSearchCommand
					?? (_scrapLocSearchCommand = CreateBusyAsyncCommand(
					(o) =>
					{
						// 2.若有新的報廢明細，加入到明細中，並檢核
						if (scrapAddDetailList != null)
							ScrapDetailSaveByLoc(scrapAddDetailList);
					},
					canExecute: () => !IsEditScrapDetails,
					completed: null,
					error: null,
					preAction: () =>
					{
						// 1.開啟報廢商品儲位查詢視窗，並回傳新加入的報廢明細
						var win = new P1604010100();
						win.SetBaseData(SelectedDc, _gupCode, _custCode, WarehouseList, ScrapResonList);
						var result = win.ShowDialog();

						scrapAddDetailList = null;
						if (result == true)
						{
							scrapAddDetailList = win.Vm.ScrapAddDetailList.Where(x => x.IsSelected).Select(it => it.Item).ToList();
						}
					}));
			}
		}

		private RelayCommand _editDetailCommand;

		/// <summary>
		/// Gets the EditDetailCommand.
		/// </summary>
		public RelayCommand EditDetailCommand
		{
			get
			{
				return _editDetailCommand
					?? (_editDetailCommand = new RelayCommand(
					() =>
					{
						if (!EditDetailCommand.CanExecute(null))
						{
							return;
						}

						IsEditScrapDetails = true;
					},
					() => !IsEditScrapDetails && SelectedScrapDetail != null));
			}
		}

		private RelayCommand _saveDetailCommand;

		/// <summary>
		/// Gets the SaveDetailCommand.
		/// </summary>
		public RelayCommand SaveDetailCommand
		{
			get
			{
				return _saveDetailCommand
					?? (_saveDetailCommand = new RelayCommand(
					() =>
					{
						if (!SaveDetailCommand.CanExecute(null))
						{
							return;
						}

						var selectedItem = SelectedScrapDetail.Item;
						var error = GetDetailError(selectedItem);

						if (!string.IsNullOrEmpty(error))
						{
							ShowWarningMessage(error);
							return;
						}

						IsEditScrapDetails = false;
					},
					() => IsEditScrapDetails && SelectedScrapDetail != null));
			}
		}

		private string GetDetailError(F160402Data selectedItem)
		{
			if (selectedItem.SCRAP_QTY <= 0)
				return Properties.Resources.P1604010000_ScrapCountEmpty;

            if (selectedItem.SCRAP_QTY > selectedItem.QTY)
                return Properties.Resources.P1604010000_ScrapCountInvalid;
            else
            {
                var errMsg = CheckStock(selectedItem, true);
                if (!string.IsNullOrWhiteSpace(errMsg))
                    return errMsg;
            }
          
			return string.Empty;
		} 
		#endregion


	}
}
