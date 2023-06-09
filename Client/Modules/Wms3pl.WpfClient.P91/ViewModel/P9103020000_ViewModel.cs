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
using Wms3pl.WpfClient.DataServices.F91DataService;
using AutoMapper;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;

namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9103020000_ViewModel : InputViewModelBase
	{
		public P9103020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetProcessList();
				SetUnitList();
				SetStatusSearchList();
				SetDcList();
				SearchDcCode = DcList.First().Value;
				SearchStartEnableDate = DateTime.Now.Date;
				SearchEndEnableDate = DateTime.Now.Date.AddDays(1);
				SearchStatus = StatusSearchList.First().Value;
			}
		}

		#region 查詢資料

		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
		#region 物流中心
		private string _searchDcCode;

		public string SearchDcCode
		{
			get { return _searchDcCode; }
			set
			{
				_searchDcCode = value;
				RaisePropertyChanged("SearchDcCode");

				// 更換物流中心時，將查詢結果清除
				if (UserOperateMode == OperateMode.Query)
				{
					F910401List = null;
				}
			}
		}

		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			var data = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
			data.Insert(0, new NameValuePair<string>() { Name = Properties.Resources.NONE_Select, Value = "000" });
			DcList = data;
		}
		#endregion

		#region 搜尋的生效日期
		private DateTime _searchStartEnableDate;

		public DateTime SearchStartEnableDate
		{
			get { return _searchStartEnableDate; }
			set
			{
				_searchStartEnableDate = value;
				RaisePropertyChanged("SearchStartEnableDate");
			}
		}


		private DateTime _searchEndEnableDate;

		public DateTime SearchEndEnableDate
		{
			get { return _searchEndEnableDate; }
			set
			{
				_searchEndEnableDate = value;
				RaisePropertyChanged("SearchEndEnableDate");
			}
		}

		#endregion

		#region 編輯的生效日期



		public DateTime EditableStartEnableDate
		{
			get
			{
				if (EditableF910401 == null)
					return default(DateTime);

				return EditableF910401.ENABLE_DATE;
			}
			set
			{
				if (EditableF910401 == null)
					return;

				if (EditableF910401.ENABLE_DATE == value)
				{
					//RaisePropertyChanged("EditableStartEnableDate");
					return;
				}


				EditableF910401.ENABLE_DATE = value;
				RaisePropertyChanged("EditableStartEnableDate");

				if (UserOperateMode == OperateMode.Add)
					ExecuteAutoSetEnableDisableDate();

				if (UserOperateMode != OperateMode.Query)
					SaveAndValidateF910402List();
			}
		}


		public DateTime EditableEndEnableDate
		{
			get
			{
				if (EditableF910401 == null)
					return default(DateTime);

				return EditableF910401.DISABLE_DATE;
			}
			set
			{
				if (EditableF910401 == null)
					return;

				if (EditableF910401.DISABLE_DATE == value)
				{
					//RaisePropertyChanged("EditableEndEnableDate");
					return;
				}


				EditableF910401.DISABLE_DATE = value;
				RaisePropertyChanged("EditableEndEnableDate");

				if (UserOperateMode == OperateMode.Add)
					ExecuteAutoSetEnableDisableDate();

				if (UserOperateMode != OperateMode.Query)
					SaveAndValidateF910402List();
			}
		}

		#endregion

		#region 報價單編號
		private string _searchQuoteNo = string.Empty;

		public string SearchQuoteNo
		{
			get { return _searchQuoteNo; }
			set
			{
				_searchQuoteNo = value;
				RaisePropertyChanged("SearchQuoteNo");
			}
		}

		#endregion

		#region 單據狀態
		private string _searchStatus;

		public string SearchStatus
		{
			get { return _searchStatus; }
			set
			{
				_searchStatus = value;
				RaisePropertyChanged("SearchStatus");
			}
		}

		private List<NameValuePair<string>> _statusSearchList;
		/// <summary>
		/// 單據狀態列表
		/// </summary>
		public List<NameValuePair<string>> StatusSearchList
		{
			get { return _statusSearchList; }
			set
			{
				_statusSearchList = value;
				RaisePropertyChanged("StatusSearchList");
			}
		}

		public void SetStatusSearchList()
		{
			var pairList = GetBaseTableService.GetF000904List(FunctionCode, "F910401", "STATUS");
			pairList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = "-1" });

			StatusSearchList = pairList;
		}

		public List<NameValuePair<string>> GetStatusList()
		{
			return _statusSearchList;
		}

		#endregion

		#endregion

		#region 查詢結果
		private List<F910401> _f910401List;

		public List<F910401> F910401List
		{
			get { return _f910401List; }
			set
			{
				_f910401List = value;
				RaisePropertyChanged("F910401List");
			}
		}

		#endregion

		#region 編輯與新增資料

		private void InitEditableData()
		{
			EditableF910401 = null;
			f910402List = null;
			EditableF910402List = null;
			f910403DataList = null;
			EditableF910403DataList = null;
		}

		#region 報價單主檔

		#region 主檔 DC
		private NameValuePair<string> _selectedDcItem;

		public NameValuePair<string> SelectedDcItem
		{
			get { return _selectedDcItem; }
			set
			{
				var dcChanged = DcIsChanged(value);

				_selectedDcItem = value;
				RaisePropertyChanged("SelectedDcItem");

				if (dcChanged)
				{
					DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_ReTestActAnalysis);	//與耗材統計
					SaveAndValidateF910402List();
					//NewDetails();
				}
			}
		}

		public bool DcIsChanged(NameValuePair<string> value)
		{
			return (UserOperateMode == OperateMode.Add
					&& _selectedDcItem != null && value != null
					&& _selectedDcItem.Value != value.Value
					&& EditableF910402List != null
					&& EditableF910403DataList != null
					&& (EditableF910402List.Any() || EditableF910403DataList.Any()));
		}
		#endregion

		private F910401 _selectedF910401;

		public F910401 SelectedF910401
		{
			get { return _selectedF910401; }
			set
			{
				_selectedF910401 = value;
				RaisePropertyChanged("SelectedF910401");

				if (value != null)
				{
					// 載入可編輯的內容
					SetEditableF910402List(value.DC_CODE, value.GUP_CODE, value.CUST_CODE, value.QUOTE_NO);
					SetEditableF910403DataList(value.DC_CODE, value.GUP_CODE, value.CUST_CODE, value.QUOTE_NO);

					if (SelectedF910401.STATUS == "2") //結案時抓原先資料
					{
						var proxy = GetProxy<F19Entities>();
						var data = proxy.F1928s.Where(o => o.OUTSOURCE_ID == SelectedF910401.OUTSOURCE_ID).ToList();
						if (data.Any())
						{
							var list = (from o in data
													select new NameValuePair<string>
													{
														Name = o.OUTSOURCE_NAME,
														Value = o.OUTSOURCE_ID
													}).ToList();
							OutSourceList = list;
						};
					}
					else
					{
						// (貨主核定價)價低的委外商動作列表，先載入 ItemSource, 在載入 SelectedValue
						SetOutSourceList(GetInexpensiveOutsources(value.DC_CODE, value.GUP_CODE, value.ENABLE_DATE, value.DISABLE_DATE));
					}

					SetEditableF910404(value.DC_CODE, value.GUP_CODE, value.CUST_CODE, value.QUOTE_NO);
					EditableF910401 = value.Clone();

					// 初始化全選
					CheckAllProcess = CheckAllItem = false;
				}
				else
				{
					EditableF910401 = null;
					EditableF910402List = null;
					EditableF910403DataList = null;
				}

				RaisePropertyChanged("EditableStartEnableDate");
				RaisePropertyChanged("EditableEndEnableDate");
			}
		}

		private F910404 _selectViewFile;
		public F910404 SelectViewFile
		{
			get { return _selectViewFile; }
			set
			{
				_selectViewFile = value;
				RaisePropertyChanged("SelectViewFile");
			}
		}



		private F910401 _editableF910401;

		public F910401 EditableF910401
		{
			get { return _editableF910401; }
			set
			{
				_editableF910401 = value;
				RaisePropertyChanged("EditableF910401");
			}
		}

		public bool ValidateNetRate()
		{
			if (EditableF910401.NET_RATE < 0)
			{
				EditableF910401.NET_RATE = 0;
				return false;
			}
			else if (EditableF910401.NET_RATE > 100)
			{
				EditableF910401.NET_RATE = 100f;
				return false;
			}

			return true;
		}
		/// <summary>
		/// 設定貨主加工申請價
		/// </summary>
		public void SetApplyPrice()
		{
			// 當有輸入或修改成本價與毛利率時，系統自動帶出貨主加工申請價，使用者仍可修改，計算規則如下：
			// 貨主加工申請價 = 成本價 / (1-毛利率)
			if (EditableF910401 != null && IsValidPrice(EditableF910401.COST_PRICE) && IsValidNetRate(EditableF910401.NET_RATE))
			{
				if (EditableF910401.NET_RATE >= 100f)
				{
					EditableF910401.APPLY_PRICE = Math.Round(EditableF910401.COST_PRICE * 100m, 2);
				}
				else
				{
					EditableF910401.APPLY_PRICE = Math.Round(EditableF910401.COST_PRICE / (1m - ((decimal)EditableF910401.NET_RATE / 100m)), 2);
				}
			}
		}

		// 依照資料庫欄位大小做驗證
		public bool IsValidNetRate(float net_rate)
		{
			return (0 <= net_rate && net_rate <= 100);
		}

		public bool IsValidPrice(decimal price)
		{
			return (0 <= price && price <= 1000000000);
		}

		public bool IsValidSeconds(decimal seconds)
		{
			return (0 <= seconds && seconds <= 1000000000);
		}
		#endregion

		#region 委外商

		private NameValuePair<string> _selectedOutsource;

		public NameValuePair<string> SelectedOutsource
		{
			get { return _selectedOutsource; }
			set
			{
				_selectedOutsource = value;
				RaisePropertyChanged("SelectedOutsource");
			}
		}

		private List<NameValuePair<string>> _outSourceList;

		public List<NameValuePair<string>> OutSourceList
		{
			get { return _outSourceList; }
			set
			{
				_outSourceList = value;
				RaisePropertyChanged("OutSourceList");
			}
		}


		/// <summary>
		/// 設定委外商清單於下拉選單，並取得最低價的委外商動作清單
		/// </summary>
		/// <param name="f910302WithF1928List"></param>
		/// <returns></returns>
		public void SetOutSourceList(IEnumerable<wcf.F910302WithF1928> f910302WithF1928List)
		{
			var groups = from g in f910302WithF1928List.GroupBy(item => new { item.OUTSOURCE_ID, item.OUTSOURCE_NAME })
									 orderby g.Sum(item => item.APPROVE_PRICE)
									 select g;

			OutSourceList = groups.Select(g => new NameValuePair<string>()
			{
				Name = g.Key.OUTSOURCE_NAME,
				Value = g.Key.OUTSOURCE_ID
			}).ToList();

			if (!OutSourceList.Any())
			{
				if (EditableF910402List != null && EditableF910402List.Any() && UserOperateMode != OperateMode.Query)
				{
					DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_NoValidSubcontractor);
				}

				RaisePropertyChanged("EditableF910401");
			}
		}

		#endregion

		#region 報價單動作分析明細
		public Action OnAddProcessMode = delegate { };
		public Action OnEditProcessMode = delegate { };
		public Action OnQueryProcessMode = delegate { };
		private OperateMode _processMode = OperateMode.Query;

		public OperateMode ProcessMode
		{
			get { return _processMode; }
			set
			{
				_processMode = value;
				RaisePropertyChanged("ProcessMode");

				switch (value)
				{
					case OperateMode.Add:
						AddF910402 = new F910402();
						OnAddProcessMode();
						break;
					case OperateMode.Edit:
						AddF910402 = SelectedEditableF910402.Item;
						OnEditProcessMode();
						break;
					case OperateMode.Query:
						AddF910402 = null;
						CheckAllProcess = false;
						OnQueryProcessMode();
						break;
				}
			}
		}

		private bool _checkAllProcess;

		public bool CheckAllProcess
		{
			get { return _checkAllProcess; }
			set
			{
				_checkAllProcess = value;
				RaisePropertyChanged("CheckAllProcess");
			}
		}

		private SelectionItem<F910402> _selectedEditableF910402;

		public SelectionItem<F910402> SelectedEditableF910402
		{
			get { return _selectedEditableF910402; }
			set
			{
				//if (AddProcessMode && (value == null || (_selectedEditableF910402 != null && _selectedEditableF910402.Item != value.Item)))
				//{
				//	RaisePropertyChanged("SelectedEditableF910402");
				//	return;
				//}

				_selectedEditableF910402 = value;
				RaisePropertyChanged("SelectedEditableF910402");
			}
		}

		private SelectionList<F910402> _editableF910402List;

		public SelectionList<F910402> EditableF910402List
		{
			get { return _editableF910402List; }
			set
			{
				_editableF910402List = value;
				RaisePropertyChanged("EditableF910402List");
			}
		}

		public void SetOrderbyEditableF910402List(IEnumerable<F910402> list)
		{
			EditableF910402List = new SelectionList<F910402>(list.OrderBy(item => item.PROCESS_ID));
		}

		private List<F910402> f910402List = null;

		public void SetEditableF910402List(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var proxy = GetProxy<F91Entities>();
			var query = from item in proxy.F910402s
									where item.QUOTE_NO == quoteNo
									where item.DC_CODE == dcCode
									where item.CUST_CODE == custCode
									where item.GUP_CODE == gupCode
									orderby item.PROCESS_ID
									select item;

			f910402List = query.ToList();
			RecoveryToF910402List();
		}

		public void SetEditableF910404(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var proxy = GetProxy<F91Entities>();
			var query = proxy.F910404s.Where(o => o.QUOTE_NO == quoteNo && o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode).FirstOrDefault();
			SelectViewFile = null;
			if (query != null)
				SelectViewFile = query;
		}


		private F910402 _addF910402;

		public F910402 AddF910402
		{
			get { return _addF910402; }
			set
			{
				_addF910402 = value;
				RaisePropertyChanged("AddF910402");
			}
		}

		public bool ValidateF910402(F910402 f910402)
		{
			if (string.IsNullOrEmpty(f910402.PROCESS_ID))

				if (!IsValidSeconds(f910402.WORK_HOUR))
				{
					DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_InputCorrectNum_H);
					return false;
				}

			if (!IsValidSeconds(f910402.WORK_COST))
			{
				DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_InputCorrectNum_D);
				return false;
			}
			return false;
			//return !
			//	&& !string.IsNullOrEmpty(f910402.UNIT_ID)
			//	&& 0 <= f910402.WORK_HOUR && f910402.WORK_HOUR <= 1000000000
			//	&& 0 <= f910402.WORK_COST && f910402.WORK_COST <= 1000000000;
		}


		#endregion

		#region 動作清單

		private List<NameValuePair<string>> _processList;

		public List<NameValuePair<string>> ProcessList
		{
			get { return _processList; }
			set
			{
				_processList = value;
				RaisePropertyChanged("ProcessList");
			}
		}

		public void SetProcessList()
		{
			var proxy = GetProxy<F91Entities>();
			var processList = (proxy.F910001s.OrderBy(item => item.PROCESS_ID)
									.ToList()
									.Select(item => new NameValuePair<string>()
									{
										Name = item.PROCESS_ACT,
										Value = item.PROCESS_ID
									})).ToList();

			ProcessList = processList;
		}

		#endregion

		#region 計量單位

		private List<NameValuePair<string>> _unitList;

		public List<NameValuePair<string>> UnitList
		{
			get { return _unitList; }
			set
			{
				_unitList = value;
				RaisePropertyChanged("UnitList");
			}
		}

		public void SetUnitList()
		{
			var proxy = GetProxy<F91Entities>();
			var unitList = (proxy.F91000302s.Where(item => item.ITEM_TYPE_ID.Equals("001"))
									.OrderBy(item => item.ACC_UNIT)
									.ToList()
									.Select(item => new NameValuePair<string>()
									{
										Name = item.ACC_UNIT_NAME,
										Value = item.ACC_UNIT
									})).ToList();

			UnitList = unitList;
		}
		#endregion

		#region 報價單耗材統計明細
		public Action OnAddSuppliesMode = delegate { };
		public Action OnQuerySuppliesMode = delegate { };

		private bool _addSuppliesMode;

		public bool AddSuppliesMode
		{
			get { return _addSuppliesMode; }
			set
			{
				_addSuppliesMode = value;
				RaisePropertyChanged("AddSuppliesMode");

				if (value)
				{
					AddF910403Data = new F910403Data();
					OnAddSuppliesMode();
				}
				else
				{
					AddF910403Data = null;
					CheckAllItem = false;
					OnQuerySuppliesMode();
				}
			}
		}

		private bool _checkAllItem;

		public bool CheckAllItem
		{
			get { return _checkAllItem; }
			set
			{
				_checkAllItem = value;
				RaisePropertyChanged("CheckAllItem");
			}
		}

		private SelectionItem<F910403Data> _selectedEditableF910403;

		public SelectionItem<F910403Data> SelectedEditableF910403
		{
			get { return _selectedEditableF910403; }
			set
			{
				_selectedEditableF910403 = value;
				RaisePropertyChanged("SelectedEditableF910403");
			}
		}

		private SelectionList<F910403Data> _editableF910403DataList;

		public SelectionList<F910403Data> EditableF910403DataList
		{
			get { return _editableF910403DataList; }
			set
			{
				_editableF910403DataList = value;
				RaisePropertyChanged("EditableF910403DataList");
			}
		}

		private void SetOrderbyEditableF910403DataList(IEnumerable<F910403Data> list)
		{
			var aa = new F910403Data();
			EditableF910403DataList = new SelectionList<F910403Data>(list.OrderBy(item => item.ITEM_CODE));
		}

		private List<F910403Data> f910403DataList = null;

		public void SetEditableF910403DataList(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var proxy = GetExProxy<P91ExDataSource>();
			var query = proxy.CreateQuery<F910403Data>("GetF910403DataByQuoteNo")
							 .AddQueryExOption("dcCode", dcCode)
							 .AddQueryExOption("gupCode", gupCode)
							 .AddQueryExOption("custCode", custCode)
							 .AddQueryExOption("quoteNo", quoteNo);

			f910403DataList = query.ToList();
			RecoveryToF910403List();
		}

		#region 新增耗材的項目
		private F910403Data _addF910403Data;

		public F910403Data AddF910403Data
		{
			get { return _addF910403Data; }
			set
			{
				_addF910403Data = value;
				RaisePropertyChanged("AddF910403Data");
			}
		}
		#endregion

		/// <summary>
		/// 是否存在該商品編號
		/// </summary>
		/// <param name="f910403Data"></param>
		/// <returns></returns>
		public F1903WithF1915 ExistsItemCode(F910403Data f910403Data)
		{
			if (string.IsNullOrWhiteSpace(f910403Data.ITEM_CODE))
			{
				return null;
			}

			f910403Data.ITEM_CODE = f910403Data.ITEM_CODE.Trim();

			// 取得該商品編號的名稱與分類名稱
			var proxy = GetExProxy<P91ExDataSource>();
			var query = proxy.CreateQuery<F1903WithF1915>("GetF1903WithF1915")
							 .AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
							 .AddQueryExOption("itemCode", f910403Data.ITEM_CODE)
							 .AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode);

			var itemData = query.ToList().FirstOrDefault();
			return itemData;
		}

		/// <summary>
		/// 是否商品編號重複
		/// </summary>
		/// <param name="f910403Data"></param>
		/// <returns></returns>
		public bool IsItemCodeRepeat(F910403Data f910403Data)
		{
			bool isRepeat = EditableF910403DataList.Where(si => si.Item.ITEM_CODE == f910403Data.ITEM_CODE).Count() > 1;
			return isRepeat;
		}

		/// <summary>
		/// 設定項目的資訊
		/// </summary>
		public void SetItemInfo()
		{
			var list = EditableF910403DataList.Select(item => item.Item.ITEM_CODE).ToList();

			var f910403 = SelectedEditableF910403.Item;

			// 查詢前，先清除項目名稱跟大分類，避免後來殘留
			f910403.ITEM_NAME = null;
			f910403.CLA_NAME = null;

			if (IsItemCodeRepeat(f910403))
			{
				DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_ConsumeItemCode);
				return;
			}

			var itemData = ExistsItemCode(f910403);
			if (itemData == null)
			{
				DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_NoItemCode);
				return;
			}

			f910403.ITEM_NAME = itemData.ITEM_NAME;
			f910403.CLA_NAME = itemData.CLA_NAME;
		}
		#endregion

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

		private bool _canEditData;

		public bool CanEditData
		{
			get { return _canEditData; }
			set
			{
				_canEditData = value;
				RaisePropertyChanged("CanEditData");
			}
		}

		public Action OnFocus = delegate { };

		public void SetUserOperateMode(OperateMode mode)
		{
			UserOperateMode = mode;
			CanEditData = (mode != OperateMode.Query && IsWiatHandleStatus);	// 當新增或為待處理編輯才能做修改
			QuoteHeaderText = GetQuoteHeaderText();
			OnFocus();
		}

		/// <summary>
		/// 是否為待處理狀態
		/// </summary>
		/// <returns></returns>
		public bool IsWiatHandleStatus
		{
			get
			{
				return UserOperateMode == OperateMode.Add || (SelectedF910401 != null && SelectedF910401.STATUS == "0");
			}
		}

		private string GetQuoteHeaderText()
		{
			switch (UserOperateMode)
			{
				case OperateMode.Edit:
					return Properties.Resources.QuoteMaintain;
				case OperateMode.Add:
					return Properties.Resources.P9103020000_ViewModel_QuoteInsert;
				default:
					return string.Empty;
			}
		}

		private bool _queryResultIsExpanded = true;

		public bool QueryResultIsExpanded
		{
			get { return _queryResultIsExpanded; }
			set
			{
				_queryResultIsExpanded = value;
				RaisePropertyChanged("QueryResultIsExpanded");
			}
		}


		#endregion

		#region Check All

		public ICommand CheckAllProcessCommand
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (EditableF910402List != null)
						foreach (var item in EditableF910402List)
							item.IsSelected = CheckAllProcess;
				},
				() => { return true; });
			}
		}

		public ICommand CheckAllItemCommand
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (EditableF910403DataList != null)
						foreach (var item in EditableF910403DataList)
							item.IsSelected = CheckAllItem;
				},
				() => { return true; });
			}
		}

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						if (!F910401List.Any())
						{
							ShowMessage(Messages.InfoNoData);
						}
						else
						{
							SelectedF910401 = F910401List.FirstOrDefault();
						}
					});
			}
		}

		private void DoSearch()
		{
			InitEditableData();

			if (SearchQuoteNo != null)
				SearchQuoteNo = SearchQuoteNo.Trim();

			//執行查詢動作
			var proxy = GetProxy<F91Entities>();
			var query = from item in proxy.F910401s
									where item.DC_CODE == SearchDcCode
									where item.GUP_CODE == _gupCode
									where item.CUST_CODE == _custCode
									where SearchStartEnableDate <= item.ENABLE_DATE
									where item.ENABLE_DATE <= SearchEndEnableDate
									select item;

			// 單據狀態若選擇全部的話，則過濾不顯示已取消的
			if (SearchStatus == StatusSearchList.First().Value)
			{
				query = query.Where(item => item.STATUS != "9");
			}
			else
			{
				query = query.Where(item => item.STATUS == SearchStatus);
			}

			if (!string.IsNullOrEmpty(SearchQuoteNo))
			{
				query = query.Where(item => item.QUOTE_NO == SearchQuoteNo);
			}

			// 排序
			query = from item in query
							orderby item.ENABLE_DATE, item.QUOTE_NO
							select item;

			F910401List = query.ToList();

			SearchResultIsExpanded = !F910401List.Any();
			QueryResultIsExpanded = F910401List.Count > 1;
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
			SetUserOperateMode(OperateMode.Add);
			//執行新增動作

			OutSourceList = null;

			EditableF910401 = new F910401
			{
				GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode,
				CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode
			};

			SetDefaultEnableDisableDate();

			NewDetails();
		}

		private void NewDetails()
		{
			f910402List = new List<F910402>();
			SetOrderbyEditableF910402List(f910402List);
			f910403DataList = new List<F910403Data>();
			SetOrderbyEditableF910403DataList(f910403DataList);
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && SelectedF910401 != null && (SelectedF910401.STATUS == "0" || SelectedF910401.STATUS == "1")
					);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
			SetUserOperateMode(OperateMode.Edit);
			if (!ValidateEnableDisableDate())
			{
				DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_ValidDateInvalid);
			}
		}

		/// <summary>
		/// 驗證生效日期與失效日期是否檢核規則
		/// </summary>
		/// <returns></returns>
		public bool ValidateEnableDisableDate()
		{
			return !(EditableF910401 != null && (DateTime.Now.Date > EditableStartEnableDate || EditableStartEnableDate > EditableEndEnableDate));
		}

		public void ExecuteAutoSetEnableDisableDate()
		{
			if (!ValidateEnableDisableDate())
			{
				DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_ValidDateInvalid_AutoTest);
				SetDefaultEnableDisableDate();
			}
		}

		/// <summary>
		/// 更新編輯的日期
		/// </summary>
		public Action OnUpdateEditableDate = delegate { };

		public void SetDefaultEnableDisableDate()
		{
			var tomorrow = DateTime.Now.Date.AddDays(1);

			if (tomorrow > EditableEndEnableDate)
			{
				EditableF910401.DISABLE_DATE = tomorrow;
				RaisePropertyChanged("EditableEndEnableDate");
			}

			EditableF910401.ENABLE_DATE = tomorrow;
			RaisePropertyChanged("EditableStartEnableDate");
			OnUpdateEditableDate();
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				bool isCancel = false;
				return CreateBusyAsyncCommand(
					o =>
					{
						isCancel = DoCancel();
					},
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						if (isCancel)
						{
							ProcessMode = OperateMode.Query;
							AddSuppliesMode = false;
						}
					}
					);
			}
		}

		private bool DoCancel()
		{
			//執行取消動作

			if (ShowMessage(Messages.WarningBeforeCancel) == UILib.Services.DialogResponse.No)
			{
				return false;
			}

			InitEditableData();

			// 重新載入選擇的報價單
			SelectedF910401 = SelectedF910401;

			SetUserOperateMode(OperateMode.Query);

			return true;
		}

		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query && SelectedF910401 != null && IsWiatHandleStatus,
					o =>
					{
						if (!F910401List.Any())
						{
							ShowMessage(Messages.InfoNoData);
						}
						else
						{
							SelectedF910401 = F910401List.FirstOrDefault();
						}
					}
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (SelectedF910401 == null || !IsWiatHandleStatus)
			{
				return;
			}

			if (ShowMessage(Messages.WarningBeforeDelete) != UILib.Services.DialogResponse.Yes)
			{
				return;
			}

			var proxy = GetProxy<F91Entities>();
			string error = UpdateStatus(proxy, currentStatus: "0", updateStatus: "9");// 標記刪除: 9
			if (!string.IsNullOrEmpty(error))
			{
				DialogService.ShowMessage(error);
			}
			else
			{
				ShowMessage(Messages.InfoDeleteSuccess);
			}

			DoSearch();
		}

		private string UpdateStatus(F91Entities proxy, string currentStatus, string updateStatus)
		{
			var query = from item in proxy.F910401s
									where item.DC_CODE == SelectedF910401.DC_CODE
									where item.GUP_CODE == SelectedF910401.GUP_CODE
									where item.CUST_CODE == SelectedF910401.CUST_CODE
									where item.QUOTE_NO == SelectedF910401.QUOTE_NO
									where item.STATUS == currentStatus
									select item;

			var f910401Entity = query.FirstOrDefault();
			if (f910401Entity == null)
				return Properties.Resources.P9103020000_ViewModel_QuoteNotExist;
			else if (f910401Entity.STATUS == "9")
				return Properties.Resources.P9103020000_ViewModel_QuoteDelete;
			else if (f910401Entity.STATUS != currentStatus)
				return Properties.Resources.P9103020000_ViewModel_QuoteStatus;

			f910401Entity.STATUS = updateStatus;
			proxy.UpdateObject(f910401Entity);
			proxy.SaveChanges();
			return string.Empty;

		}
		#endregion Delete

		#region Save
		/// <summary>
		/// 當新增或為待處理狀態時，才可以儲存
		/// </summary>
		public bool CanSave
		{
			get
			{
				return EditableF910401 != null
					&& (EditableF910401.QUOTE_NO == null || EditableF910401.STATUS == "0")
					&& EditableF910402List != null
					&& EditableF910402List.Any();
				// 耗材不重要，所以耗材可為空
				//&& EditableF910403DataList != null
				//&& EditableF910403DataList.Any();
			}
		}

		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				string editQuoteNo = null;
				return CreateBusyAsyncCommand(
					o =>
					{
						isSaved = false;
						editQuoteNo = null;

						var errorMsg = ValidateSave();
						if (!string.IsNullOrEmpty(errorMsg))
						{
							DialogService.ShowMessage(errorMsg);
							return;
						}

						if (ShowMessage(Messages.WarningBeforeUpdate) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						isSaved = DoSave(isExecuteApproved: false);

						if (isSaved)
						{
							editQuoteNo = EditableF910401.QUOTE_NO;

							AfterSavedSearch();
						}
					},
					() => CanEditData && CanSave && ProcessMode == OperateMode.Query && !AddSuppliesMode,
					o =>
					{
						if (isSaved)
						{
							AfterSavedDisplay(editQuoteNo);
							SetUserOperateMode(OperateMode.Query);
						}
					}
					);
			}
		}

		public void AfterSavedDisplay(string editQuoteNo)
		{
			if (!F910401List.Any())
			{
				ShowMessage(Messages.InfoNoData);
			}
			else
			{
				if (UserOperateMode == OperateMode.Edit)
					SelectedF910401 = F910401List.FirstOrDefault(item => item.QUOTE_NO == editQuoteNo);
				else if (UserOperateMode == OperateMode.Add)
					SelectedF910401 = F910401List.LastOrDefault();
			}
		}

		public void AfterSavedSearch()
		{
			SearchDcCode = EditableF910401.DC_CODE;
			SearchStartEnableDate = SearchEndEnableDate = EditableStartEnableDate;
			SearchQuoteNo = string.Empty;
			SearchStatus = StatusSearchList.Select(item => item.Value).FirstOrDefault();
			DoSearch();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isExecuteApproved">是否核准</param>
		/// <returns></returns>
		private bool DoSave(bool isExecuteApproved)
		{
			//執行確認儲存動作

			var proxy = new wcf.P91WcfServiceClient();

			var f910401 = EditableF910401.Map<F910401, wcf.F910401>();
			var f910402s = EditableF910402List.Select(item => item.Item).MapCollection<F910402, wcf.F910402>().ToArray();
			var f910403s = EditableF910403DataList.Select(item => item.Item).MapCollection<F910403Data, wcf.F910403>().ToArray();

			wcf.ExecuteResult result = new wcf.ExecuteResult() { Message = Properties.Resources.P9103020000_ViewModel_InvalidOperate };
			if (UserOperateMode == OperateMode.Add)
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
														() => proxy.InsertP910302(f910401, f910402s, f910403s));
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
					() => proxy.UpdateP910302(f910401, f910402s, f910403s, isExecuteApproved));
			}


			if (!result.IsSuccessed)
			{
				DialogService.ShowMessage(result.Message);
			}

			return result.IsSuccessed;
		}

		private string ValidateSave()
		{
			if (!CanSave)
			{
				return Properties.Resources.P9103020000_ViewModel_OperatingError;
			}

			if (UserOperateMode == OperateMode.Edit)
			{
				if (string.IsNullOrEmpty(EditableF910401.QUOTE_NO))
				{
					return Properties.Resources.P9103020000_ViewModel_InvalidQuote;
				}

				if (EditableF910401.STATUS == "9")
				{
					return Properties.Resources.P9103020000_ViewModel_QuoteDeleted;
				}
			}

			if (string.IsNullOrEmpty(EditableF910401.DC_CODE))
			{
				return Properties.Resources.P9103020000_ViewModel_SelectDC;
			}

			EditableF910401.QUOTE_NAME = EditableF910401.QUOTE_NAME.Trim();

			if (string.IsNullOrEmpty(EditableF910401.QUOTE_NAME))
			{
				return Properties.Resources.P9103020000_ViewModel_ItemName_Required;
			}

			if (EditableF910401.QUOTE_NAME.Length > 100)
			{
				return Properties.Resources.P9103020000_ViewModel_ItemNameLength100;
			}

			if (!IsValidNetRate(EditableF910401.NET_RATE))
			{
				return Properties.Resources.P9103020000_ViewModel_Rate;
			}

			if (!ValidateEnableDisableDate())
			{
				return Properties.Resources.P9103020000_ViewModel_ValidDateInvalid;
			}

			if (!IsValidPrice(EditableF910401.APPLY_PRICE))
			{
				return Properties.Resources.P9103020000_ViewModel_APPLY_PRICE_Required;
			}

			foreach (var item in EditableF910402List)
			{
				if (string.IsNullOrEmpty(item.Item.PROCESS_ID))
					return Properties.Resources.P9103020000_ViewModel_PROCESS_ID_Required;

				if (string.IsNullOrEmpty(item.Item.UNIT_ID))
					return Properties.Resources.P9103020000_ViewModel_UNIT_ID_Required;
			}

			foreach (var item in EditableF910403DataList)
			{
				if (string.IsNullOrEmpty(item.Item.ITEM_CODE))
					return Properties.Resources.P9103020000_ViewModel_ITEM_CODE_Required;

				if (string.IsNullOrEmpty(item.Item.UNIT_ID))
					return Properties.Resources.P9103020000_ViewModel_UNIT_ID_Required;
			}

			return null;
		}
		#endregion Save

		#region 列印與預覽

		public Action<PrintType> OnPrintAction = delegate { };

		public F910401Report F910401Report { get; set; }

		public List<F910402Report> F910402Reports { get; set; }

		public List<F910403Report> F910403Reports { get; set; }

		public F910401Report GetSelectedF910401Report()
		{
			var proxy = GetExProxy<P91ExDataSource>();
			var query1 = proxy.CreateQuery<F910401Report>("GetF910401Report")
							 .AddQueryExOption("dcCode", SelectedF910401.DC_CODE)
							 .AddQueryExOption("gupCode", SelectedF910401.GUP_CODE)
							 .AddQueryExOption("custCode", SelectedF910401.CUST_CODE)
							 .AddQueryExOption("quoteNo", SelectedF910401.QUOTE_NO);

			var f910401Report = query1.ToList().FirstOrDefault();
			return f910401Report;
		}

		public List<F910402Report> GetSelectedF910402Reports()
		{
			var proxy = GetExProxy<P91ExDataSource>();
			var query2 = proxy.CreateQuery<F910402Report>("GetF910402Reports")
			 .AddQueryExOption("dcCode", SelectedF910401.DC_CODE)
			 .AddQueryExOption("gupCode", SelectedF910401.GUP_CODE)
			 .AddQueryExOption("custCode", SelectedF910401.CUST_CODE)
			 .AddQueryExOption("quoteNo", SelectedF910401.QUOTE_NO);

			var f910402Reports = query2.ToList();
			// 給予動作項次編號
			int rownum = 0;
			foreach (var item in f910402Reports)
			{
				rownum++;
				item.PROCESS_ACT = string.Format("{0}.{1}", rownum, item.PROCESS_ACT);
			}
			return f910402Reports;
		}

		public List<F910403Report> GetSelectedF910403Reports()
		{
			var proxy = GetExProxy<P91ExDataSource>();

			var query3 = proxy.CreateQuery<F910403Report>("GetF910403Reports")
			 .AddQueryExOption("dcCode", SelectedF910401.DC_CODE)
			 .AddQueryExOption("gupCode", SelectedF910401.GUP_CODE)
			 .AddQueryExOption("custCode", SelectedF910401.CUST_CODE)
			 .AddQueryExOption("quoteNo", SelectedF910401.QUOTE_NO);

			var f910403Reports = query3.ToList();
			return f910403Reports;
		}

		public void LoadReportData()
		{
			F910401Report = GetSelectedF910401Report();
			F910402Reports = GetSelectedF910402Reports();
			F910403Reports = GetSelectedF910403Reports();
		}


		#region Preview
		public ICommand PreviewCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPreview(),
					() => UserOperateMode == OperateMode.Query && SelectedF910401 != null,
					o =>
					{
						OnPrintAction(PrintType.Preview);
					}
					);
			}
		}

		private void DoPreview()
		{
			//執行確認儲存動作
			LoadReportData();
		}
		#endregion

		#region PrintCommand
		public ICommand PrintCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPrint(),
					() => UserOperateMode == OperateMode.Query && SelectedF910401 != null,
					o =>
					{
						OnPrintAction(PrintType.ToPrinter);
					}
					);
			}
		}

		private void DoPrint()
		{
			LoadReportData();
		}
		#endregion

		#endregion

		#region ApprovedCommand
		/// <summary>
		/// 核准 Button
		/// </summary>
		public ICommand ApprovedCommand
		{
			get
			{
				bool isSaved = false;
				string editQuoteNo = null;
				return CreateBusyAsyncCommand(
					o =>
					{
						isSaved = false;
						editQuoteNo = null;

						var errorMsg = ValidateSave();
						if (!string.IsNullOrEmpty(errorMsg))
						{
							DialogService.ShowMessage(errorMsg);
							return;
						}

						if (ShowMessage(new MessagesStruct()
						{
							Message = Properties.Resources.P9103020000_ViewModel_UpdateQuoteApprove,
							Title = Properties.Resources.P9103020000_ViewModel_ApproveQuote,
							Button = UILib.Services.DialogButton.YesNo,
							Image = UILib.Services.DialogImage.Question
						}) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						// 更新報價單相關資料表後再更新狀態 -- 問鈺綺: 意思是要先儲存現有資料囉? 所以要能儲存才能用核准??
						// Update F910401, F910402,F910403
						// Update F910401.STATUS=1
						// 4.	《核准》：當輸入貨主加工核定價與狀態為待處理時，方可點選《核准》按鈕，系統提示【是否確認更新報價單為已核准？】，是則將此報價單狀態更新為〈已核准〉，並返回查詢主畫面。
						isSaved = DoSave(isExecuteApproved: true);

						if (isSaved)
						{
							editQuoteNo = EditableF910401.QUOTE_NO;

							AfterSavedSearch();
						}
					},
					() => CanExecuteApproved() && CanSave && ProcessMode == OperateMode.Query && !AddSuppliesMode,
					o =>
					{
						// 若核准 要返回查詢狀態
						if (isSaved)
						{
							AfterSavedDisplay(editQuoteNo);
							SetUserOperateMode(OperateMode.Query);
						}
					}
					);
			}
		}

		private bool CanExecuteApproved()
		{
			return UserOperateMode == OperateMode.Edit
									&& SelectedF910401 != null
									&& EditableF910401 != null
									&& EditableF910401.STATUS == "0"						// 狀態為待處理
									&& EditableF910401.APPROVED_PRICE.HasValue				// 貨主加工核定價
									&& IsValidPrice(EditableF910401.APPROVED_PRICE.Value);
		}

		#endregion

		#region UploadViewCommand

		public ICommand UploadViewCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => UserOperateMode == OperateMode.Query
									&& SelectedF910401 != null
									&& SelectViewFile != null	// 1 = 已核准
					);
			}
		}

		#endregion

		#region UploadCommand


		public ICommand UploadCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoUpload(),
					() => UserOperateMode == OperateMode.Edit
									&& SelectedF910401 != null
									&& SelectedF910401.STATUS == "1"	// 1 = 已核准
					);
			}
		}

		private void DoUpload()
		{
			// F0910404 報價單上傳紀錄檔
			// 上傳檔案Server根目路路徑設定於AppConfig
			// Update F910401.STATUS=2
			//throw new NotImplementedException();
		}

		public bool UpdateStatus()
		{
			var proxy = GetProxy<F91Entities>();
			string error = UpdateStatus(proxy, currentStatus: "1", updateStatus: "2");
			if (!string.IsNullOrEmpty(error))
			{
				DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_QuoteStatusCantUploadImage);
				return false;
			}

			return true;
		}

		public void UploadImagePath(string clientPath, string serverPath)
		{
			var proxy = GetProxy<F91Entities>();
			proxy.AddToF910404s(new F910404()
			{
				QUOTE_NO = SelectedF910401.QUOTE_NO,
				DC_CODE = SelectedF910401.DC_CODE,
				GUP_CODE = SelectedF910401.GUP_CODE,
				CUST_CODE = SelectedF910401.CUST_CODE,
				CRT_STAFF = Wms3plSession.CurrentUserInfo.Account,
				CRT_NAME = Wms3plSession.CurrentUserInfo.AccountName,
				CRT_DATE = DateTime.Now,
				UPLOAD_C_PATH = clientPath,
				UPLOAD_S_PATH = serverPath,
				UPLOAD_NO = 1
			});
			proxy.SaveChanges();
			DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_UploadImageSuccess);

			var quoteNo = SelectedF910401.QUOTE_NO;
			AfterSavedSearch();
			AfterSavedDisplay(quoteNo);
			SetUserOperateMode(OperateMode.Query);
		}

		#endregion

		#region 複製報價單
		public ICommand CopyQuoteCommand
		{
			get
			{
				return new RelayCommand(
					() => CopyQuote(),
					() => UserOperateMode == OperateMode.Query && SelectedF910401 != null
				);
			}
		}

		private void CopyQuote()
		{
			SetUserOperateMode(OperateMode.Add);
			EditableF910401.QUOTE_NO = null;
			EditableF910401.STATUS = null;
			foreach (var item in EditableF910402List)
			{
				item.Item.QUOTE_NO = null;
			}
			foreach (var item in EditableF910403DataList)
			{
				item.Item.QUOTE_NO = null;
			}

			if (!ValidateEnableDisableDate())
			{
				SetDefaultEnableDisableDate();
			}
		}
		#endregion

		#region 新增動作
		public ICommand AddProcessCommand
		{
			get
			{
				return new RelayCommand(
					() => AddProcess(),
					() => CanExecuteProcessButton && ProcessMode == OperateMode.Query
				);
			}
		}

		private void AddProcess()
		{
			ProcessMode = OperateMode.Add;

			var si = new SelectionItem<F910402>(AddF910402);
			EditableF910402List.Add(si);
			SelectedEditableF910402 = si;
		}

		private bool CanExecuteProcessButton
		{
			get
			{
				return EditableF910401 != null
					&& !string.IsNullOrEmpty(EditableF910401.DC_CODE)
					&& EditableStartEnableDate >= DateTime.Now.Date
					&& EditableEndEnableDate >= EditableStartEnableDate
					&& CanEditData;
			}
		}
		#endregion

		#region 編輯動作
		public ICommand EditProcessCommand
		{
			get
			{
				return new RelayCommand(
					() => EditProcess(),
					() => CanExecuteProcessButton && ProcessMode == OperateMode.Query && SelectedEditableF910402 != null
						);
			}
		}

		private void EditProcess()
		{
			if (SelectedEditableF910402 != null)
			{
				ProcessMode = OperateMode.Edit;
			}

		}
		#endregion

		#region 刪除動作
		public ICommand DeleteProcessCommand
		{
			get
			{
				return new RelayCommand(
						() => DeleteProcess(),
						() => CanExecuteProcessButton
							&& ProcessMode == OperateMode.Query
							&& EditableF910402List != null
							&& EditableF910402List.Any(item => item.IsSelected));
			}
		}

		private void DeleteProcess()
		{
			// 刪除已選擇的項目
			SetOrderbyEditableF910402List(EditableF910402List.Where(item => !item.IsSelected).Select(item => item.Item));

			SaveAndValidateF910402List();
		}
		#endregion

		#region 取消動作
		public ICommand CancelProcessCommand
		{
			get
			{
				return new RelayCommand(
						() => CancelProcess(),
						() => CanExecuteProcessButton && ProcessMode != OperateMode.Query
						);
			}
		}

		private void CancelProcess()
		{
			RecoveryToF910402List();
			ProcessMode = OperateMode.Query;
		}

		private void RecoveryToF910402List()
		{
			EditableF910402List = f910402List.MapCollection<F910402, F910402>().OrderBy(item => item.PROCESS_ID).ToSelectionList();
		}
		#endregion

		#region 儲存動作
		public ICommand SaveProcessCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
						o =>
						{
							isSaved = SaveProcess();
						},
						() => CanExecuteProcessButton
							&& (ProcessMode == OperateMode.Edit || (ProcessMode == OperateMode.Add && AddF910402 != null))
							&& CanSaveProcess,
						o =>
						{
							if (isSaved)
							{
								ProcessMode = OperateMode.Query;
								//SetSelectedOutsource();
							}
						}
						);
			}
		}

		private void SetSelectedOutsource()
		{
			SelectedOutsource = OutSourceList.Where(item => item.Value == EditableF910401.OUTSOURCE_ID).FirstOrDefault();
		}

		private bool CanSaveProcess
		{
			get
			{
				return !string.IsNullOrEmpty(AddF910402.PROCESS_ID)
					&& !string.IsNullOrEmpty(AddF910402.UNIT_ID)
					&& IsValidSeconds(AddF910402.WORK_HOUR)
					&& IsValidPrice(AddF910402.WORK_COST);
			}
		}

		private bool IsRepeatProcessId()
		{
			return EditableF910402List.Select(g => g.Item.PROCESS_ID).Distinct().Count() != EditableF910402List.Count;
			//return EditableF910402List.Where(si => si.Item.PROCESS_ID == AddF910402.PROCESS_ID).Count() == 1;
		}

		private bool SaveProcess()
		{
			if (!CanSaveProcess)
			{
				return false;
			}
			if (IsRepeatProcessId())
			{
				DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_ActDuplicate);
				return false;
			}

			if (ProcessMode == OperateMode.Edit)
				SaveF910402List();
			else
				SaveAndValidateF910402List();
			return true;
		}

		public void SaveAndValidateF910402List()
		{
			if (SelectedDcItem == null)
			{
				return;
			}

			// 自動選擇(貨主核定價)價低的委外商與其相關動作金額資訊於動作分析明細的動作金額欄位中
			// 使用 SelectedDcItem.Value 的原因是，因為改變DC後，會呼叫這方法，但EditableF910401.DC_CODE還來不及改變，無法使用新的DC來做驗證
			var f910302WithF1928List = GetInexpensiveOutsources(SelectedDcItem.Value, EditableF910401.GUP_CODE, EditableStartEnableDate, EditableEndEnableDate);

			SetOutSourceList(f910302WithF1928List);

			// (貨主核定價)價低的委外商動作列表
			var query = f910302WithF1928List.GroupBy(item => item.OUTSOURCE_ID).OrderBy(g => g.Sum(item => item.APPROVE_PRICE));

			if (query.Any())
			{
				var inexpensivef910302WithF1928Group = query.First();

				// 相關動作金額資訊於動作分析明細的動作金額欄位中
				foreach (var item in EditableF910402List)
				{
					var approvePrice = inexpensivef910302WithF1928Group.First(f910302 => f910302.PROCESS_ID == item.Item.PROCESS_ID)
																	 .APPROVE_PRICE;
					if (approvePrice.HasValue)
					{
						item.Item.WORK_COST = approvePrice.Value;
					}
				}

				EditableF910401.OUTSOURCE_ID = inexpensivef910302WithF1928Group.Key;	// Key: 最低價格的 OUTSOURCE_ID
			}

			SaveF910402List();
		}

		private void SaveF910402List()
		{
			if (AddF910402 != null)
			{
				var list = EditableF910402List.Where(si => si.Item != AddF910402).Select(si => si.Item).ToList();
				list.Add(AddF910402);

				SetOrderbyEditableF910402List(list);
			}

			f910402List = EditableF910402List.Select(item => Mapper.DynamicMap<F910402>(item.Item)).ToList();
		}

		/// <summary>
		/// 從合約表中，取得排序(貨主核定價)價低的委外商
		/// </summary>
		/// <returns></returns>
		private List<wcf.F910302WithF1928> GetInexpensiveOutsources(string dcCode, string gupCode, DateTime enableDate, DateTime disableDate)
		{
			if (!EditableF910402List.Any())
			{
				return new List<wcf.F910302WithF1928>();
			}

			var proxy = new wcf.P91WcfServiceClient();

			var processIds = EditableF910402List.Select(si => si.Item.PROCESS_ID).ToArray();
			var f910302WithF1928List = RunWcfMethod<wcf.F910302WithF1928[]>(proxy.InnerChannel,
														() => proxy.GetF910302ByProcessIds(dcCode,
																							 gupCode,
																							 enableDate,
																							 disableDate,
																							 processIds));

			// 取得符合所有動作的委外商
			var processIdsString = string.Join(",", processIds.OrderBy(p => p));

			var query = from groupbyOutsourceId in GetGroupByProcessId(f910302WithF1928List)
									where string.Join(",", groupbyOutsourceId.Select(item => item.PROCESS_ID).OrderBy(p => p)) == processIdsString
									select groupbyOutsourceId;

			var list = query.SelectMany(g => g).ToList();
			return list;
		}

		private IEnumerable<IEnumerable<wcf.F910302WithF1928>> GetGroupByProcessId(wcf.F910302WithF1928[] f910302WithF1928List)
		{
			// 將不同委外商分開來
			var groups = from item in f910302WithF1928List
									 group item by item.OUTSOURCE_ID
										 into g
										 select g;

			// 主約與附約有相同加工動作且都符合生效區間則以附約為優先，所以動作可能重複，已附約為優先，在來價格低的
			var query = from g in groups
									let pgroups = from p in g
																group p by p.PROCESS_ID into pg
																select pg.OrderByDescending(s => s.CONTRACT_TYPE).ThenBy(s => s.APPROVE_PRICE).First()
									select pgroups;

			return query;
		}
		#endregion

		#region 新增耗材
		public ICommand AddSuppliesCommand
		{
			get
			{
				return new RelayCommand(
					() => AddSupplies(),
					() => CanExecuteProcessButton && !AddSuppliesMode
				);
			}
		}

		private void AddSupplies()
		{
			AddSuppliesMode = true;

			var si = new SelectionItem<F910403Data>(AddF910403Data);
			EditableF910403DataList.Add(si);
			SelectedEditableF910403 = si;
		}
		#endregion

		#region 刪除耗材
		public ICommand DeleteSuppliesCommand
		{
			get
			{
				return new RelayCommand(
					() => DeleteSupplies(),
					() => CanExecuteProcessButton
							&& !AddSuppliesMode
							&& EditableF910403DataList != null
							&& EditableF910403DataList.Any(item => item.IsSelected)
				);
			}
		}

		private void DeleteSupplies()
		{
			// 刪除已選擇的項目
			SetOrderbyEditableF910403DataList(EditableF910403DataList.Where(item => !item.IsSelected).Select(item => item.Item));

			SaveF910403DataList();
		}
		#endregion

		#region 取消耗材
		public ICommand CancelSuppliesCommand
		{
			get
			{
				return new RelayCommand(
					() => CancelSupplies(),
					() => CanExecuteProcessButton && AddSuppliesMode
				);
			}
		}

		private void CancelSupplies()
		{
			RecoveryToF910403List();
			AddSuppliesMode = false;
		}

		private void RecoveryToF910403List()
		{
			EditableF910403DataList = f910403DataList.MapCollection<F910403Data, F910403Data>().OrderBy(item => item.ITEM_CODE).ToSelectionList();
		}
		#endregion

		#region 儲存耗材
		public ICommand SaveSuppliesCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
					o =>
					{
						isSaved = SaveSupplies();
					},
					() => CanExecuteProcessButton
							&& AddSuppliesMode
							&& AddF910403Data != null
							&& CanSaveSupplies,
					o =>
					{
						if (isSaved)
						{
							AddSuppliesMode = false;
						}
					}
				);
			}
		}

		private bool SaveSupplies()
		{
			if (!CanSaveSupplies)
			{
				return false;
			}
			if (IsRepeatItemCode())
			{
				DialogService.ShowMessage(Properties.Resources.P9103020000_ViewModel_ItemDuplicate);
				return false;
			}

			SaveF910403DataList();
			return true;
		}

		private void SaveF910403DataList()
		{
			if (AddF910403Data != null)
			{
				var list = EditableF910403DataList.Where(si => si.Item != AddF910403Data).Select(si => si.Item).ToList();
				list.Add(AddF910403Data);

				SetOrderbyEditableF910403DataList(list);
			}

			f910403DataList = EditableF910403DataList.Select(item => Mapper.DynamicMap<F910403Data>(item.Item)).ToList();
		}

		private bool IsRepeatItemCode()
		{
			return EditableF910403DataList.Select(g => g.Item.ITEM_CODE).Distinct().Count() != EditableF910403DataList.Count;
		}

		public bool CanSaveSupplies
		{
			get
			{
				return !string.IsNullOrEmpty(AddF910403Data.ITEM_CODE)
					&& !string.IsNullOrEmpty(AddF910403Data.ITEM_NAME)
					&& !string.IsNullOrEmpty(AddF910403Data.UNIT_ID)
					&& AddF910403Data.STANDARD.HasValue
					&& IsValidSeconds(AddF910403Data.STANDARD.Value)
					&& AddF910403Data.STANDARD_COST.HasValue
					&& IsValidPrice((decimal)(AddF910403Data.STANDARD_COST ?? 0));
			}
		}
		#endregion

	}
}
