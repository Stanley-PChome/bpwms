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
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using System.IO;
using System.Data;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F50DataService;
using Wms3pl.WpfClient.DataServices.Interface;
using System.Data.Services.Client;

namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9103030000_ViewModel : InputViewModelBase
	{
		#region Property,Fields...

		#region Action
		public Action<PrintType> DoPrint = delegate { };
		#endregion

		#region Form - 查詢條件1
		#region UI 連動 binding
		private bool _searchResultIsExpanded;

		public bool SearchResultIsExpanded
		{
			get { return _searchResultIsExpanded; }
			set
			{
				_searchResultIsExpanded = value;
				RaisePropertyChanged("SearchResultIsExpanded");
			}
		}
		#endregion
		#region ComBox - DC/ ContractObjects
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get
			{
				return _dcList;
			}
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}
		private string _selectedDc = string.Empty;
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				_selectedDc = value;
				RaisePropertyChanged("SelectedDc");
			}
		}

		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		public string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		/// <summary>
		/// 合約對象
		/// </summary>
		private List<NameValuePair<string>> _contractObjects;
		public List<NameValuePair<string>> ContractObjects
		{
			get { return _contractObjects; }
			set
			{
				_contractObjects = value;
				RaisePropertyChanged("ContractObjects");
			}
		}

		private string _selectContractObject = "";
		public string SelectContractObject
		{
			get { return _selectContractObject; }
			set
			{
				_selectContractObject = value;
				RaisePropertyChanged("SelectContractObject");
			}
		}

		/// <summary>
		/// 合約分類
		/// </summary>
		public List<NameValuePair<string>> ContractTypeLists
		{
			get
			{
				return new List<NameValuePair<string>>()
					{
						new NameValuePair<string>() {Name = Properties.Resources.P9103030000_ViewModel_MainContract, Value = "0"},
						new NameValuePair<string>() {Name = Properties.Resources.P9103030000_ViewModel_SubContract, Value = "1"}
					};
			}
		}

		/// <summary>
		/// 項目類別
		/// </summary>
		private List<NameValuePair<string>> _itemTypeLists;
		public List<NameValuePair<string>> ItemTypeLists
		{
			get { return _itemTypeLists; }
			set
			{
				_itemTypeLists = value;
				RaisePropertyChanged("ItemTypeLists");
			}
		}

		private Dictionary<string, List<NameValuePair<string>>> _dicItemTypeLists;
		public Dictionary<string, List<NameValuePair<string>>> DicItemTypeLists
		{
			get { return _dicItemTypeLists; }
			set
			{
				_dicItemTypeLists = value;
				RaisePropertyChanged("DicItemTypeLists");
			}
		}

		private string _selectItemType = "";
		public string SelectItemType
		{
			get { return _selectItemType; }
			set
			{
				_selectItemType = value;
				RaisePropertyChanged("SelectItemType");
			}
		}
		/// <summary>
		/// 項目名稱
		/// </summary>
		private Dictionary<string, List<NameValuePair<string>>> _qUOTELists;
		public Dictionary<string, List<NameValuePair<string>>> QUOTELists
		{
			get { return _qUOTELists; }
			set
			{
				_qUOTELists = value;
				RaisePropertyChanged("QUOTELists");
				//if (value != null)
				//{
				//	_quoteNameDictionary = value.ToDictionary(item => item.Value, item => item.Name);
				//}
			}
		}

		//private Dictionary<string, string> _quoteNameDictionary = null;

		private string _selectQUOTE = "";
		public string SelectQUOTE
		{
			get { return _selectQUOTE; }
			set
			{
				_selectQUOTE = value;
				RaisePropertyChanged("SelectQUOTE");
			}
		}
		/// <summary>
		/// 計量單位
		/// </summary>
		private Dictionary<string, List<NameValuePair<string>>> _uNITLists;
		public Dictionary<string, List<NameValuePair<string>>> UNITLists
		{
			get { return _uNITLists; }
			set
			{
				_uNITLists = value;
				RaisePropertyChanged("UNITLists");
			}
		}

		private string _selectUNIT = "";
		public string SelectUNIT
		{
			get { return _selectUNIT; }
			set
			{
				_selectUNIT = value;
				RaisePropertyChanged("SelectUNIT");
			}
		}
		/// <summary>
		/// 加工動作
		/// </summary>
		private List<NameValuePair<string>> _pROCESSLists;
		public List<NameValuePair<string>> PROCESSLists
		{
			get { return _pROCESSLists; }
			set
			{
				_pROCESSLists = value;
				RaisePropertyChanged("PROCESSLists");
			}
		}

		private string _selectPROCESS = "";
		public string SelectPROCESS
		{
			get { return _selectPROCESS; }
			set
			{
				_selectPROCESS = value;
				RaisePropertyChanged("SelectPROCESS");
			}
		}
		#endregion
		#region Other - CRTDateS,CRTDateE,SearchCONTRACT_NO,IsSearchCust,IsSearchOutSource
		/// <summary>
		/// 建立時間-起
		/// </summary>
		private DateTime? _cRTDateS = DateTime.Today;
		public DateTime? CRTDateS
		{
			get { return _cRTDateS; }
			set
			{
				_cRTDateS = value;
				RaisePropertyChanged("CRTDateS");
			}
		}
		/// <summary>
		/// 建立時間-迄
		/// </summary>
		private DateTime? _cRTDateE = DateTime.Today;
		public DateTime? CRTDateE
		{
			get { return _cRTDateE; }
			set
			{
				_cRTDateE = value;
				RaisePropertyChanged("CRTDateE");
			}
		}
		/// <summary>
		/// 合約編號
		/// </summary>
		private string _searchCONTRACT_NO = string.Empty;
		public string SearchCONTRACT_NO { get { return _searchCONTRACT_NO; } set { _searchCONTRACT_NO = value; RaisePropertyChanged("SearchCONTRACT_NO"); } }
		/// <summary>
		/// 貨主
		/// </summary>
		//private bool _isSearchCust = true;
		//public bool IsSearchCust { get { return _isSearchCust; } set { _isSearchCust = value; RaisePropertyChanged("IsSearchCust"); } }
		///// <summary>
		///// 委外商
		///// </summary>
		//private bool _isSearchOutSource;
		//public bool IsSearchOutSource { get { return _isSearchOutSource; } set { _isSearchOutSource = value; RaisePropertyChanged("IsSearchOutSource"); } }

		private string _SearchObjectType = "0";
		public string SearchObjectType
		{
			get { return _SearchObjectType; }
			set
			{
				Set(() => SearchObjectType, ref _SearchObjectType, value);
				// 已做過新增後再次切換合約對象做查詢時，合約編號欄位需清空
				SearchCONTRACT_NO = string.Empty;
				SetObjectType();
				DgList = null;
				_contractDetailCount = 0;
				RaisePropertyChanged("ContractDetailCount");
			}
		}
		#endregion
		#endregion

		#region Form - 查詢條件2
		private List<F910301Data> _dgList;
		public List<F910301Data> DgList { get { return _dgList; } set { _dgList = value; RaisePropertyChanged("DgList"); } }

		private F910301Data _selectedData;
		public F910301Data SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				if (_selectedData != null)
				{
					if (SelectObjectType != _selectedData.OBJECT_TYPE)
					{
						SelectObjectType = _selectedData.OBJECT_TYPE;
					}
				}
				RaisePropertyChanged("SelectedData");

				if (value != null)
				{
					_lastF910301Data = ExDataMapper.Clone(value);
				}

				SearchDetailCommand.Execute(null);
			}
		}
		#endregion

		#region Form - 合約項目
		private int _contractDetailCount = 0;
		public string ContractDetailCount { get { return string.Format(Properties.Resources.P9103030000_ViewModel_Total, _contractDetailCount); } }
		private SelectionList<F910302Data> _serialRecords = new SelectionList<F910302Data>(new List<F910302Data>());
		public SelectionList<F910302Data> SerialRecords { get { return _serialRecords; } set { _serialRecords = value; RaisePropertyChanged("SerialRecords"); } }
		private SelectionItem<F910302Data> _selectSerialRecord;
		public SelectionItem<F910302Data> SelectSerialRecord
		{
			get { return _selectSerialRecord; }
			set
			{
				_selectSerialRecord = value;
				if(UserOperateMode != OperateMode.Query)
					SetToNewSerialRecord();
				RaisePropertyChanged("SelectSerialRecord");
			}
		}
		private F910302Data _newSerialRecord;
		public F910302Data NewSerialRecord { get { return _newSerialRecord; } set { _newSerialRecord = value; RaisePropertyChanged("NewSerialRecord"); } }
		private List<int> OriContractIds { get; set; }
		private List<int> DelContractIds { get; set; }

		private SelectionList<F910302Data> BackUpSerialRecords { get; set; }
		private F910301Data BackUpSelectedData { get; set; }

		#endregion

		#region 檔案上傳 file
		private string _fullPath;
		public string FullPath { get { return _fullPath; } set { _fullPath = value; } }
		private string _filePath;
		public string FilePath { get { return _filePath; } set { _filePath = value; } }
		private string _fileName;
		public string FileName { get { return _fileName; } set { _fileName = value; } }
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
				RaisePropertyChanged("IsCheckAll");
			}
		}
		#endregion

		#region 新增或編輯選擇的合約對象
		private string _selectObjectType = "0";
		public string SelectObjectType
		{
			get { return _selectObjectType; }
			set
			{
				if (SelectedData == null) return;
				if (UserOperateMode != OperateMode.Query && !ChangeContractObject()) return;
				_selectObjectType = value;
				SelectedData.OBJECT_TYPE = _selectObjectType;
				RaisePropertyChanged("SelectObjectType");
			}
		}
		#endregion

		#region 結算週期
		private List<NameValuePair<string>> _closeCycleList;

		public List<NameValuePair<string>> CloseCycleList
		{
			get { return _closeCycleList; }
			set
			{
				if (_closeCycleList == value)
					return;
				Set(() => CloseCycleList, ref _closeCycleList, value);
			}
		}
		#endregion

		#region 結算日
		private Dictionary<string, List<NameValuePair<string>>> _cycleDateList;

		public Dictionary<string, List<NameValuePair<string>>> CycleDateList
		{
			get { return _cycleDateList; }
			set
			{
				if (_cycleDateList == value)
					return;
				Set(() => CycleDateList, ref _cycleDateList, value);
			}
		}
		#endregion


		#region 是否可編輯合約主檔
		private bool _canEditMain;

		public bool CanEditMain
		{
			get { return _canEditMain; }
			set
			{
				if (_canEditMain == value)
					return;
				Set(() => CanEditMain, ref _canEditMain, value);
			}
		}
		#endregion
		
	
		

		#endregion

		#region Function
		public P9103030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControl();
			}
		}

		private void InitControl()
		{
			SetDcList();
			SetObjectType();
			SetItemType();
			//SetQUOTE();
			SetUNIT();
			SetPROCESS();
			SetCloseCycle();
			SetCycleDate();
			SetDicItemTypeLists();
		}

		public void SetContractFee()
		{
			if (NewSerialRecord == null)
				return;
			if (string.IsNullOrEmpty(NewSerialRecord.QUOTE_NO) || NewSerialRecord.ITEM_TYPE == "001")
				NewSerialRecord.CONTRACT_FEE = string.Empty;
			else
				NewSerialRecord.CONTRACT_FEE = GetContractFee();
		}

		private string GetContractFee()
		{
			var proxy = GetExProxy<P91ExDataSource>();
			var results = proxy.CreateQuery<string>("GetContractFee")
				.AddQueryExOption("dcCode", NewSerialRecord.DC_CODE)
				.AddQueryExOption("gupCode", NewSerialRecord.GUP_CODE)
				.AddQueryExOption("custCode", _custCode)
				.AddQueryExOption("quoteNo", NewSerialRecord.QUOTE_NO)
				.AddQueryExOption("itemTye", NewSerialRecord.ITEM_TYPE)
				.ToList()
				.FirstOrDefault();
			return results;
		}

		private void SetDicItemTypeLists()
		{
			Dictionary<string, List<NameValuePair<string>>> dicItemTypeList = new Dictionary<string, List<NameValuePair<string>>>();
			if (ItemTypeLists != null && ItemTypeLists.Any())
			{
				dicItemTypeList.Add("0", ItemTypeLists);
				dicItemTypeList.Add("1", ItemTypeLists.Where(x => x.Value == "001").ToList());
			}
			if (dicItemTypeList != null && dicItemTypeList.Any())
				DicItemTypeLists = dicItemTypeList;
		}

		private void SetCycleDate()
		{
			//0:日-不用輸入,1:週-0-6,2:月-1-31
			Dictionary<string, List<NameValuePair<string>>> dicCycleDate = new Dictionary<string, List<NameValuePair<string>>>();
			dicCycleDate.Add("0", new List<NameValuePair<string>>() { new NameValuePair<string>() { Name = string.Empty, Value = string.Empty } });
			var weeks = EnumHelper.EnumToNameValuePairList<DayOfWeek>();
			dicCycleDate.Add("1", weeks);

			var dayList = new List<NameValuePair<string>>();
			int i = 1;
			while (i < 32)
			{
				dayList.Add(new NameValuePair<string>() { Name = i.ToString(), Value = i.ToString() });
				i++;
			}
			dicCycleDate.Add("2", dayList);
			CycleDateList = dicCycleDate;
		}

		private void SetCloseCycle()
		{
			var proxy = GetProxy<F00Entities>();
			var data = proxy.F000904s.Where(x => x.TOPIC == "F910301" && x.SUBTOPIC == "CLOSE_CYCLE")
				.Select(x =>
					new NameValuePair<string>() { Name = x.NAME, Value = x.VALUE }
					).ToList();
			CloseCycleList = data;
		}

		#region Grid 全選

		public void CheckSelectedAll(bool isChecked)
		{
			foreach (var SerialRecord in SerialRecords)
				if (UserOperateMode == OperateMode.Add || (UserOperateMode == OperateMode.Edit && SerialRecord.Item.ENABLE_DATE > DateTime.Today))
					SerialRecord.IsSelected = isChecked;
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
			data.Insert(0, new NameValuePair<string>() { Name = Properties.Resources.NONE_Select, Value = "000" });
			DcList = data;
			if (DcList.Any())
				SelectedDc = DcList.FirstOrDefault().Value;
		}
		#endregion
		#region 設定合約對象清單
		public void SetObjectType()
		{
			var proxy = GetProxy<F19Entities>();
			List<NameValuePair<string>> data = new List<NameValuePair<string>>();
			if (SearchObjectType == "0")
			{
				data = proxy.F1909s.Where(x => x.GUP_CODE == _gupCode && x.STATUS != "9")
								   .Select(x => new NameValuePair<string>() { Name = x.CUST_NAME, Value = x.UNI_FORM })
								   .ToList();
			}
			else
			{
				data = proxy.F1928s.Where(x => x.STATUS != "9")
								   .Select(x => new NameValuePair<string>() { Name = x.OUTSOURCE_NAME, Value = x.UNI_FORM })
								   .ToList();

				data.Insert(0, new NameValuePair<string>(Resources.Resources.All, string.Empty));
			}
			ContractObjects = data;

			// 新增與編輯後，手動設定，不連動
			if (UserOperateMode == OperateMode.Query)
			{
				if (ContractObjects.Any())
					SelectContractObject = ContractObjects.FirstOrDefault().Value;
			}
		}

		#endregion
		#region 設定項目類別清單
		public void SetItemType()
		{
			var proxy = GetProxy<F91Entities>();
			List<NameValuePair<string>> data = new List<NameValuePair<string>>();
			data = proxy.F910003s.Where(n => n.ISVISABLE == "1" && n.ITEM_TYPE_ID!="008").Select(x =>
				new NameValuePair<string>() { Name = x.ITEM_TYPE, Value = x.ITEM_TYPE_ID }
				).ToList();

			ItemTypeLists = data;
			if (ItemTypeLists.Any())
				SelectItemType = ItemTypeLists.FirstOrDefault().Value;
		}
		#endregion
		#region 設定項目名稱清單
		public bool SetQUOTE()
		{
			Dictionary<string, List<NameValuePair<string>>> dicQuote = new Dictionary<string, List<NameValuePair<string>>>();
			foreach (var item in ItemTypeLists)
			{
				//001	加工計價
				//002	儲位計價
				//003	作業計價
				//004	出貨計價
				//005	派車計價
				//006	其他項目計價
				//007	專案計價
				switch (item.Value)
				{
					case "001":
						var f910401s = GetF910401s();
						if (f910401s != null && f910401s.Count > 0)
							dicQuote.Add(item.Value, f910401s);
						break;
					case "002":
					case "003":
					case "004":
					case "005":
					case "006":
						if (SelectedData != null && !string.IsNullOrEmpty(SelectedData.CUST_CODE))
						{
							var f500101To5s = GetF500101To5NameValuePairList(GetIF500101To5DataServiceQuery(item.Value));
							if (f500101To5s != null && f500101To5s.Count > 0)
								dicQuote.Add(item.Value, f500101To5s);
						}
						break;
					case "007":
						var f19007s = GetF199007s();
						if (f19007s != null && f19007s.Count > 0)
							dicQuote.Add(item.Value, f19007s);
						break;
					default:
						break;
				}

			}
			if (dicQuote == null || !dicQuote.Any())
				QUOTELists = null;
			else
				QUOTELists = dicQuote;

			if (QUOTELists != null && QUOTELists.Any() && !string.IsNullOrEmpty(SelectItemType))
			{
				SelectQUOTE = QUOTELists.Where(x => x.Key == SelectItemType)
										.SelectMany(x => x.Value)
										.Select(x => x.Value)
										.FirstOrDefault();
			}

			return true;
		}

		IQueryable<IF500101To5> GetIF500101To5DataServiceQuery(string itemType)
		{
			var f50Proxy = GetProxy<F50Entities>();
			//001	加工計價
			//002	儲位計價
			//003	作業計價
			//004	出貨計價
			//005	派車計價
			//006	其他項目計價
			//007	專案計價
			switch (itemType)
			{
				case "002":
					return f50Proxy.F500101s;
				case "003":
					return f50Proxy.F500104s;
				case "004":
					return f50Proxy.F500103s;
				case "005":
					return f50Proxy.F500102s;
				case "006":
					return f50Proxy.F500105s;
				default:
					return null;
			}
		}

		private List<NameValuePair<string>> GetF500101To5NameValuePairList<T>(IQueryable<T> dataServiceQuery) where T : IF500101To5
		{
			if (SelectedData == null || UserOperateMode == OperateMode.Query)
				return null;

			var query = from item in dataServiceQuery
						where (item.DC_CODE == SelectedData.DC_CODE || item.DC_CODE == "000")
						where item.GUP_CODE == SelectedData.GUP_CODE
						where item.CUST_CODE == SelectedData.CUST_CODE
						where item.STATUS == "2"   // 已結案的報價單才可顯示
						where item.ENABLE_DATE <= SelectedData.ENABLE_DATE		// 且貨主合約單的起訖日期必須在報價單的範圍內
						where SelectedData.DISABLE_DATE <= item.DISABLE_DATE
						select item;

			var list = query.ToList();
			return list.Select(item => new NameValuePair<string>()
					{
						Name = string.Format("{0}-{1}", item.QUOTE_NO, item.ACC_ITEM_NAME),
						Value = item.QUOTE_NO
					}).ToList();
		}

		private List<NameValuePair<string>> GetF910401s()
		{
			if (SelectedData == null || UserOperateMode == OperateMode.Query)
				return null;

			var proxy = GetProxy<F91Entities>();
			List<NameValuePair<string>> data = new List<NameValuePair<string>>();

			var query = from item in proxy.F910401s
						where (item.DC_CODE == SelectedData.DC_CODE || item.DC_CODE == "000")
						where item.GUP_CODE == SelectedData.GUP_CODE
						where item.CUST_CODE == SelectedData.CUST_CODE
						where item.STATUS == "2"   // 已結案的報價單才可顯示
						where item.ENABLE_DATE <= SelectedData.ENABLE_DATE		// 且貨主合約單的起訖日期必須在報價單的範圍內
						where SelectedData.DISABLE_DATE <= item.DISABLE_DATE
						select new NameValuePair<string>()
						{
							Name = string.Format("{0}-{1}", item.QUOTE_NO, item.QUOTE_NAME),
							Value = item.QUOTE_NO
						};
			return query.ToList();
		}

		private List<NameValuePair<string>> GetF199007s()
		{
			if (SelectedData == null || UserOperateMode == OperateMode.Query)
				return null;

			var proxy = GetProxy<F19Entities>();
			List<NameValuePair<string>> data = new List<NameValuePair<string>>();

			var query = from item in proxy.F199007s
									where (item.DC_CODE == SelectedData.DC_CODE || item.DC_CODE == "000")
									where item.GUP_CODE == SelectedData.GUP_CODE
									where item.CUST_CODE == SelectedData.CUST_CODE
									where item.STATUS == "2"   // 已結案的報價單才可顯示
									where item.ENABLE_DATE <= SelectedData.ENABLE_DATE		// 且貨主合約單的起訖日期必須在報價單的範圍內
									where SelectedData.DISABLE_DATE <= item.DISABLE_DATE
									select new NameValuePair<string>()
									{
										Name = string.Format("{0}-{1}", item.ACC_PROJECT_NO, item.ACC_PROJECT_NAME),
										Value = item.ACC_PROJECT_NO
									};
			return query.ToList();
		}
		#endregion
		#region 設定計量單位清單
		public void SetUNIT()
		{
			if (ItemTypeLists == null) return;

			var dicUnit = new Dictionary<string, List<NameValuePair<string>>>();

			var proxy = GetProxy<F91Entities>();
			var f91000302s = proxy.F91000302s.ToList();	// 全部都會用到，就一次撈出來
			UNITLists = f91000302s.GroupBy(x => x.ITEM_TYPE_ID)
								  .ToDictionary(g => g.Key,
												g => g.Select(x => new NameValuePair<string>(x.ACC_UNIT_NAME, x.ACC_UNIT)).ToList());

			if (!string.IsNullOrEmpty(SelectItemType) && UNITLists.ContainsKey(SelectItemType))
			{
				SelectUNIT = UNITLists[SelectItemType].Select(x => x.Value).FirstOrDefault();
			}
		}
		#endregion
		#region 設定加工動作清單
		public void SetPROCESS()
		{
			var proxy = GetProxy<F91Entities>();
			List<NameValuePair<string>> data = new List<NameValuePair<string>>();
			data = proxy.F910001s.Select(x =>
			new NameValuePair<string>() { Name = x.PROCESS_ACT, Value = x.PROCESS_ID }
			).ToList();

			PROCESSLists = data;
			if (PROCESSLists.Any())
				SelectPROCESS = PROCESSLists.FirstOrDefault().Value;
		}
		#endregion
		#endregion

		#region newRecord

		#region 新增合約主檔及副檔
		private void SetNewRecord()
		{
			SelectedData = new F910301Data();
			SelectedData.GUP_CODE = _gupCode;
			SelectedData.CUST_CODE = _custCode;
			SelectedData.OBJECT_TYPE = "0";
			SelectedData.ENABLE_DATE = DateTime.Today.AddDays(1);
			SelectedData.DISABLE_DATE = DateTime.Today.AddDays(1);
			SelectedData.CLOSE_CYCLE = "1";
			SelectedData.DC_CODE = "000";
			SelectObjectType = "0";
		}

		private void SetNewDetailRecord()
		{
			NewSerialRecord = new F910302Data();
			NewSerialRecord.GUP_CODE = _gupCode;
			if (SelectedData != null)
			{
				NewSerialRecord.DC_CODE = SelectedData.DC_CODE;
				NewSerialRecord.ENABLE_DATE = CanEditMain ? SelectedData.ENABLE_DATE : DateTime.Today.AddDays(1);
				NewSerialRecord.DISABLE_DATE = SelectedData.DISABLE_DATE;
			}

		}
		#endregion

		#region 設定選定合約副檔至NewSerialRecord
		private void SetToNewSerialRecord()
		{
			if (SelectSerialRecord == null) return;
			var newF910302 = new F910302Data();
			NewSerialRecord = new F910302Data();
			newF910302 = AutoMapper.Mapper.DynamicMap<F910302Data>(SelectSerialRecord.Item);
			NewSerialRecord = newF910302;
		}
		#endregion

		#region 合約對象資訊
		public bool GetContractObject()
		{
			var proxy = GetProxy<F19Entities>();
			if (SelectedData == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SelectContractor, Title = Resources.Resources.Information });
				return true;
			}

			//先清空
			ClearContractObject();

			if (SelectedData.OBJECT_TYPE == "0")
			{
				// 更換貨主時，清除計價設定清單
				QUOTELists = null;
				SelectedData.CUST_CODE = null;
				if (UserOperateMode == OperateMode.Add && SerialRecords != null && SerialRecords.Any())
				{
					ShowWarningMessage(Properties.Resources.P9103030000_ViewModel_ChangeCust_ClearContractDetail);
					SerialRecords = new SelectionList<F910302Data>(new List<F910302Data>());
				}

				var f1909 = GetF1909ByUniForm(SelectedData.GUP_CODE, SelectedData.UNI_FORM);
				if (f1909 == null)
					return false;

				SetContractObject(f1909);
				SetQUOTE();
				return true;
			}
			else
			{
				var f1928 = GetF1928ByUniForm(SelectedData.UNI_FORM);
				if (f1928 == null)
					return false;

				SetContractObject(f1928);
				return true;
			}
		}

		/// <summary>
		/// 判斷該統編是否存在
		/// </summary>
		/// <param name="objectType"></param>
		/// <param name="gupCode"></param>
		/// <param name="uniForm"></param>
		/// <returns></returns>
		public bool ExistsUniForm(string objectType, string gupCode, string uniForm, out F1909 f1909, out F1928 f1928)
		{
			f1909 = null;
			f1928 = null;

			if (objectType == "0")
			{
				f1909 = GetF1909ByUniForm(gupCode, uniForm);
				return f1909 != null;
			}
			else
			{
				f1928 = GetF1928ByUniForm(uniForm);
				return f1928 != null;
			}
		}

		/// <summary>
		/// 取得符合統編的第一個貨主
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="uniForm"></param>
		/// <returns></returns>
		public F1909 GetF1909ByUniForm(string gupCode, string uniForm)
		{
			var proxy = GetProxy<F19Entities>();
			return proxy.F1909s.Where(x => x.GUP_CODE == gupCode && x.UNI_FORM == uniForm).FirstOrDefault();
		}

		/// <summary>
		/// 取得符合統編的第一個委外商
		/// </summary>
		/// <param name="uniForm"></param>
		/// <returns></returns>
		public F1928 GetF1928ByUniForm(string uniForm)
		{
			var proxy = GetProxy<F19Entities>();
			return proxy.F1928s.Where(x => x.UNI_FORM == uniForm).FirstOrDefault();
		}

		private void SetContractObject(object contractObj)
		{
			var properties = contractObj.GetType().GetProperties();
			foreach (var prop in properties)
			{
				if (prop.Name.ToUpper() == "CUST_CODE")
					SelectedData.CUST_CODE = (prop.GetValue(contractObj) != null) ? prop.GetValue(contractObj).ToString() : string.Empty;
				else if (prop.Name.ToUpper() == "CUST_NAME")
					SelectedData.OBJECT_NAME = (prop.GetValue(contractObj) != null) ? prop.GetValue(contractObj).ToString() : string.Empty;
				else if (prop.Name.ToUpper() == "OUTSOURCE_NAME")
					SelectedData.OBJECT_NAME = (prop.GetValue(contractObj) != null) ? prop.GetValue(contractObj).ToString() : string.Empty;
				else if (prop.Name.ToUpper() == "CONTACT")
					SelectedData.CONTACT = (prop.GetValue(contractObj) != null) ? prop.GetValue(contractObj).ToString() : string.Empty;
				else if (prop.Name.ToUpper() == "TEL")
					SelectedData.TEL = (prop.GetValue(contractObj) != null) ? prop.GetValue(contractObj).ToString() : string.Empty;
			}
		}

		private void ClearContractObject()
		{
			SelectedData.OBJECT_NAME = string.Empty;
			SelectedData.CONTACT = string.Empty;
			SelectedData.TEL = string.Empty;
		}
		#endregion

		#region 新增明細
		public void DoAddDetail()
		{
			//執行新增項目明細
			if (NewSerialRecord == null) return;
			if (!isDetailValid(NewSerialRecord)) return;

			if (!CanEditMain && NewSerialRecord.CONTRACT_TYPE == "0")
			{
					ShowWarningMessage(Properties.Resources.P9103030000_ViewModel_ContractEffective_CantAddNew);
					return;
			}


			var newSeq = getDetailSeq();

			// 新建一筆記錄
			var newF910302 = new F910302Data();
			newF910302 = AutoMapper.Mapper.DynamicMap<F910302Data>(NewSerialRecord);
			newF910302.CONTRACT_SEQ = newSeq;
			newF910302.DC_CODE = SelectedData.DC_CODE;

			if (newF910302.CONTRACT_TYPE == "0")
			{
				newF910302.SUB_CONTRACT_NO = string.Empty;
				newF910302.ENABLE_DATE = SelectedData.ENABLE_DATE;
				newF910302.DISABLE_DATE = SelectedData.DISABLE_DATE;
			}

			var addRecord = new SelectionItem<F910302Data>(newF910302);
			SerialRecords.Add(addRecord);
			SetNewDetailRecord();
			ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_InsertDetail, Title = Resources.Resources.Information });
		}

		private int getDetailSeq()
		{
			int _newSeq = 1;
			if (SerialRecords == null || !SerialRecords.Any()) return _newSeq;
			var maxSeq = SerialRecords.Max(x => x.Item.CONTRACT_SEQ);
			_newSeq = maxSeq + 1;
			return _newSeq;
		}
		#endregion

		#region 修改明細
		public void DoEditDetail()
		{
			//執行修改項目明細

			if(SelectSerialRecord.Item.ENABLE_DATE <= DateTime.Today)
			{
				ShowWarningMessage(Properties.Resources.P9103030000_ViewModel_ContractEffective_CantEdit);
				return;
			}

			if (NewSerialRecord == null) return;
			if (!isDetailValid(NewSerialRecord, "editDetail")) return;


			// 更新一筆記錄
			var newF910302 = new F910302Data();
			newF910302 = AutoMapper.Mapper.DynamicMap<F910302Data>(NewSerialRecord);
			SerialRecords.Where(x => x.Item.CONTRACT_NO == newF910302.CONTRACT_NO &&
																							 x.Item.CONTRACT_SEQ == newF910302.CONTRACT_SEQ &&
																							 x.Item.DC_CODE == newF910302.DC_CODE &&
																							 x.Item.GUP_CODE == newF910302.GUP_CODE)
																		.Select(x =>
																		{
																			x.Item.SUB_CONTRACT_NO = (newF910302.CONTRACT_TYPE == "1") ? newF910302.SUB_CONTRACT_NO : string.Empty;
																			x.Item.CONTRACT_TYPE = newF910302.CONTRACT_TYPE;
																			x.Item.ENABLE_DATE = (newF910302.CONTRACT_TYPE == "1") ? newF910302.ENABLE_DATE : SelectedData.ENABLE_DATE;
																			x.Item.DISABLE_DATE = (newF910302.CONTRACT_TYPE == "1") ? newF910302.DISABLE_DATE : SelectedData.DISABLE_DATE;
																			x.Item.ITEM_TYPE = newF910302.ITEM_TYPE;
																			x.Item.QUOTE_NO = newF910302.QUOTE_NO;
																			x.Item.UNIT_ID = newF910302.UNIT_ID;
																			x.Item.TASK_PRICE = newF910302.TASK_PRICE;
																			x.Item.WORK_HOUR = newF910302.WORK_HOUR;
																			x.Item.PROCESS_ID = newF910302.PROCESS_ID;
																			x.Item.OUTSOURCE_COST = newF910302.OUTSOURCE_COST;
																			x.Item.APPROVE_PRICE = newF910302.APPROVE_PRICE;
																			x.Item.CONTRACT_TYPENAME = newF910302.CONTRACT_TYPENAME;
																			x.Item.QUOTE_NAME = newF910302.QUOTE_NAME;
																			x.Item.ITEM_TYPE_NAME = newF910302.ITEM_TYPE_NAME;
																			x.Item.UNIT = newF910302.UNIT;
																			x.Item.PROCESS_ACT = newF910302.PROCESS_ACT;
																			x.Item.CONTRACT_FEE = newF910302.CONTRACT_FEE;
																			return x;
																		}).ToSelectionList();

			SetNewDetailRecord();
			SelectSerialRecord = SerialRecords.Where(x => x.Item.CONTRACT_NO == newF910302.CONTRACT_NO &&
															 x.Item.CONTRACT_SEQ == newF910302.CONTRACT_SEQ &&
															 x.Item.DC_CODE == newF910302.DC_CODE &&
															 x.Item.GUP_CODE == newF910302.GUP_CODE).FirstOrDefault();
			ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_Update, Title = Resources.Resources.Information });
		}

		#endregion

		#region 刪除明細
		public void DoDelDetail()
		{
			//執行刪除項目明細

			// 刪除記錄
			if (ShowMessage(Messages.WarningBeforeDelete) != UILib.Services.DialogResponse.Yes) return;
			if (SerialRecords == null) return;

			if (!CanEditMain)
			{
				if (SerialRecords.Any(x => x.IsSelected && x.Item.CONTRACT_TYPE == "0"))
				{
					ShowWarningMessage(Properties.Resources.P9103030000_ViewModel_ContractEffective_CantDelete);
					return;
				}
			}

			if (SerialRecords.Any(x => x.IsSelected && x.Item.CONTRACT_TYPE == "1" && x.Item.ENABLE_DATE <= DateTime.Today))
			{
				ShowWarningMessage(Properties.Resources.P9103030000_ViewModel_SubContractEffective_CantDelete);
				return;
			}

			var delRecords = SerialRecords.Where(x => x.IsSelected);
			if (delRecords.Any(x => x.Item.CONTRACT_TYPE == "0") && SerialRecords.Count() > 1)
			{
				var msg = new MessagesStruct()
				{
					Button = DialogButton.YesNo,
					Image = DialogImage.Warning,
					Message = Properties.Resources.P9103030000_ViewModel_IsDeleteContract
				};
				if (ShowMessage(msg) != UILib.Services.DialogResponse.Yes) return;
				//檢核是否已生效
				if (SerialRecords.Any(x => x.Item.ENABLE_DATE <= DateTime.Now.Date))
				{
					ShowWarningMessage(Properties.Resources.P9103030000_ViewModel_ContractEffective_CanNotDelete);
					return;
				}

				//刪除主約
				SerialRecords = new SelectionList<F910302Data>(new List<F910302Data>());
			}
			else
			{
				//檢核是否已生效
				if (delRecords.Any(x => x.Item.ENABLE_DATE <= DateTime.Now.Date))
				{
					ShowWarningMessage(Properties.Resources.P9103030000_ViewModel_ContractEffective_CanNotDelete);
					return;
				}

				//刪除非主約或只有主約沒附約
				SerialRecords = SerialRecords.Where(x => !x.IsSelected).Select(si => si.Item).ToSelectionList();

				if (SerialRecords == null)
					SerialRecords = new SelectionList<F910302Data>(new List<F910302Data>());
			}
			//將全選取消
			IsCheckAll = false;

			ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_DeleteItemDetail, Title = Resources.Resources.Information });
		}
		#endregion



		#region 取得COMBOX NAME

		public void SetPROCESSName()
		{
			if (PROCESSLists.Any())
				NewSerialRecord.PROCESS_ACT = PROCESSLists.Where(x => x.Value == NewSerialRecord.PROCESS_ID).Select(x => x.Name).FirstOrDefault();
		}

		public void SetQUOTEName()
		{
			if (!string.IsNullOrEmpty(NewSerialRecord.ITEM_TYPE)
				&& !string.IsNullOrEmpty(NewSerialRecord.QUOTE_NO)
				&& QUOTELists != null
				&& QUOTELists.ContainsKey(NewSerialRecord.ITEM_TYPE))
			{
				NewSerialRecord.QUOTE_NAME = QUOTELists[NewSerialRecord.ITEM_TYPE].Where(x => x.Value == NewSerialRecord.QUOTE_NO)
																				  .Select(x => x.Name)
																				  .FirstOrDefault();
			}
		}

		public void SetItemTypeName()
		{
			if (ItemTypeLists.Any())
				NewSerialRecord.ITEM_TYPE_NAME = ItemTypeLists.Where(x => x.Value == NewSerialRecord.ITEM_TYPE).Select(x => x.Name).FirstOrDefault();
		}

		public void SetContractTypeName()
		{
			//if (NewSerialRecord == null)
			//	SetNewDetailRecord();

			// 當改變合約分類時，帶入設定合約分類名稱
			if (ContractTypeLists.Any())
				NewSerialRecord.CONTRACT_TYPENAME = ContractTypeLists.Where(x => x.Value == NewSerialRecord.CONTRACT_TYPE).Select(x => x.Name).FirstOrDefault();
		}

		public void SetUnitName()
		{
			if (!string.IsNullOrEmpty(NewSerialRecord.ITEM_TYPE) && UNITLists.ContainsKey(NewSerialRecord.ITEM_TYPE))
			{
				NewSerialRecord.UNIT = UNITLists[NewSerialRecord.ITEM_TYPE].Where(x => x.Value == NewSerialRecord.UNIT_ID)
																		   .Select(x => x.Name)
																		   .FirstOrDefault();
			}
		}

		#endregion

		#region 匯入Excel明細
		public void DoImportData()
		{
			//執行匯入動作
			if (!CanImport()) return;
			//var proxyEx = GetExProxy<P08ExDataSource>();
			var data = new List<F910302Data>();
			var msg = new MessagesStruct();
			var subFileName = Path.GetExtension(FullPath);
			try
			{
				using (FileStream file = new FileStream(FullPath, FileMode.Open, FileAccess.Read))
				{
					byte[] bytes = new byte[file.Length];
					file.Read(bytes, 0, (int)file.Length);
					file.Position = 0;

					//取得合約單項目明細序號
					var newSeq = getDetailSeq();
					DataTable excelTable = null;

					if (subFileName == ".xlsx")
						excelTable = DataTableExtension3.RenderDataTableFromExcelFor2007(file, 0, 0);
					else if (subFileName == ".xls")
						excelTable = DataTableExtension.RenderDataTableFromExcel(file, 0, 0);
					else
					{
						var result = new ExDataServices.P91ExDataService.ExecuteResult();
						result.IsSuccessed = false;
						result.Message = Properties.Resources.P9103030000_ViewModel_FileFormatError;
						msg = Messages.ErrorImportFailed;
						msg.Message = msg.Message + Environment.NewLine + result.Message;
						ShowMessage(msg);
						return;
					}

					var errMsg = string.Empty;

					if (SelectedData.OBJECT_TYPE == "0")
					{
						// 貨主：合約分類(資料內容:主約、附約)、合約編號、項目類別、項目名稱、計量單位、標準工時(秒)、作業單價(元)、生效日期、失效日期。
						var query = from col in excelTable.AsEnumerable()
									select new
									{
										CONTRACT_TYPENAME = Convert.ToString(col[0]),
										WORK_HOUR = Convert.ToString(col[5]),
										TASK_PRICE = Convert.ToString(col[6]),
										ENABLE_DATE = Convert.ToString(col[7]),
										DISABLE_DATE = Convert.ToString(col[8])
									};

						foreach (var item in query)
						{
							int intVal;
							if (!int.TryParse(item.WORK_HOUR, out intVal))
							{
								errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_WORK_HOUR_Error, item.WORK_HOUR);
								break;
							}

							double doubleVal;
							if (!double.TryParse(item.TASK_PRICE, out doubleVal))
							{
								errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_TASK_PRICE_Error, item.TASK_PRICE);
								break;
							}

							if (item.CONTRACT_TYPENAME != Properties.Resources.P9103030000_ViewModel_MainContract)
							{
								DateTime datetime;
								if (!DateTime.TryParse(item.ENABLE_DATE, out datetime))
								{
									errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_ENABLE_DATE_Error, item.ENABLE_DATE);
									break;
								}

								if (!DateTime.TryParse(item.DISABLE_DATE, out datetime))
								{
									errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_DISABLE_DATE_Error, item.DISABLE_DATE);
									break;
								}

								if (DateTime.Parse(item.ENABLE_DATE) <= DateTime.Today)
								{
									errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_ENABLE_DATE_Invalid, item.ENABLE_DATE);
									break;
								}
							}
							else
							{
								if (!CanEditMain)
								{
									errMsg = Properties.Resources.P9103030000_ViewModel_CantEditMain;
									break;
								}
							}
						}
					}
					else
					{
						// 委外商：合約分類(資料內容:主約、附約)、合約編號、項目類別、加工動作、成本價(元)、貨主核定價(元)、生效日期、失效日期
						var query = from col in excelTable.AsEnumerable()
									select new
									{
										CONTRACT_TYPENAME = Convert.ToString(col[0]),
										OUTSOURCE_COST = Convert.ToString(col[4]),
										APPROVE_PRICE = Convert.ToString(col[5]),
										ENABLE_DATE = Convert.ToString(col[6]),
										DISABLE_DATE = Convert.ToString(col[7])
									};

						foreach (var item in query)
						{
							double doubleVal;
							if (!double.TryParse(item.OUTSOURCE_COST, out doubleVal))
							{
								errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_OUTSOURCE_COST_Error, item.OUTSOURCE_COST);
								break;
							}

							if (!double.TryParse(item.APPROVE_PRICE, out doubleVal))
							{
								errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_APPROVE_PRICE_Error, item.APPROVE_PRICE);
								break;
							}

							if (item.CONTRACT_TYPENAME != Properties.Resources.P9103030000_ViewModel_MainContract)
							{
								DateTime datetime;
								if (!DateTime.TryParse(item.ENABLE_DATE, out datetime))
								{
									errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_ENABLE_DATE_Error, item.ENABLE_DATE);
									break;
								}

								if (!DateTime.TryParse(item.DISABLE_DATE, out datetime))
								{
									errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_DISABLE_DATE_Error, item.DISABLE_DATE);
									break;
								}

								if (DateTime.Parse(item.ENABLE_DATE) <= DateTime.Today)
								{
									errMsg = string.Format(Properties.Resources.P9103030000_ViewModel_ENABLE_DATE_Invalid, item.ENABLE_DATE);
									break;
								}
							}
							else
							{
								if (!CanEditMain)
								{
									errMsg = Properties.Resources.P9103030000_ViewModel_CantEditMain;
									break;
								}
							}
						}
					}

					if (!string.IsNullOrEmpty(errMsg))
					{
						DialogService.ShowMessage(errMsg, Properties.Resources.P9103030000_ViewModel_Error, DialogButton.OK, DialogImage.Error);
						return;
					}

					if (SelectedData.OBJECT_TYPE == "0")
					{
						// 貨主：合約分類(資料內容:主約、附約)、合約編號、項目類別、項目名稱、計量單位、標準工時(秒)、作業單價(元)、生效日期、失效日期。
						data = (from col in excelTable.AsEnumerable()
								select new F910302Data()
								{
									CONTRACT_NO = SelectedData.CONTRACT_NO,
									CONTRACT_SEQ = newSeq++,
									CONTRACT_TYPENAME = Convert.ToString(col[0]),
									SUB_CONTRACT_NO = Convert.ToString(col[1]),
									ITEM_TYPE_NAME = Convert.ToString(col[2]),
									QUOTE_NAME = Convert.ToString(col[3]),
									UNIT = Convert.ToString(col[4]),
									WORK_HOUR = (string.IsNullOrEmpty(Convert.ToString(col[5]))) ? (int?)null : Convert.ToInt32(Convert.ToString(col[5])),
									TASK_PRICE = (string.IsNullOrEmpty(Convert.ToString(col[6]))) ? (decimal?)null : Convert.ToDecimal(Convert.ToString(col[6])),
									ENABLE_DATE = DateTimeHelper.ConversionDate(Convert.ToString(col[7])),
									DISABLE_DATE = DateTimeHelper.ConversionDate(Convert.ToString(col[8])),
									DC_CODE = SelectedData.DC_CODE,
									GUP_CODE = SelectedData.GUP_CODE
								}).ToList();
					}
					else
					{
						// 委外商：合約分類(資料內容:主約、附約)、合約編號、項目類別、加工動作、成本價(元)、貨主核定價(元)、生效日期、失效日期
						data = (from col in excelTable.AsEnumerable()
								select new F910302Data()
								{
									CONTRACT_NO = SelectedData.CONTRACT_NO,
									CONTRACT_SEQ = newSeq++,
									CONTRACT_TYPENAME = Convert.ToString(col[0]),
									SUB_CONTRACT_NO = Convert.ToString(col[1]),
									ITEM_TYPE_NAME = Convert.ToString(col[2]),
									PROCESS_ACT = Convert.ToString(col[3]),
									OUTSOURCE_COST = (string.IsNullOrEmpty(Convert.ToString(col[4]))) ? (decimal?)null : Convert.ToDecimal(Convert.ToString(col[4])),
									APPROVE_PRICE = (string.IsNullOrEmpty(Convert.ToString(col[5]))) ? (decimal?)null : Convert.ToDecimal(Convert.ToString(col[5])),
									ENABLE_DATE = DateTimeHelper.ConversionDate(Convert.ToString(col[6])),
									DISABLE_DATE = DateTimeHelper.ConversionDate(Convert.ToString(col[7])),
									DC_CODE = SelectedData.DC_CODE,
									GUP_CODE = SelectedData.GUP_CODE
								}).ToList();
					}

					// 檢核並取ComboxID
					foreach (var item in data)
					{
						if (SelectedData.OBJECT_TYPE == "0")
							//貨主
							errMsg = GetCustContractID(item);
						else
							//委外商
							errMsg = GetOutSourceContractID(item);

						if (!string.IsNullOrEmpty(errMsg))
							break;
					}

					if (!string.IsNullOrEmpty(errMsg))
					{
						var result = new ExDataServices.P19ExDataService.ExecuteResult();
						result.IsSuccessed = false;
						result.Message = Properties.Resources.P9103030000_ViewModel_ContractError + Environment.NewLine + errMsg;
						msg = Messages.ErrorImportFailed;
						msg.Message = msg.Message + Environment.NewLine + result.Message;
					}
					else
					{
						var backupDetails = SerialRecords.Select(si => ExDataMapper.Map<F910302Data, F910302Data>(si.Item)).ToList();

						// 新增至項目明細
						foreach (var item in data)
						{
							//驗證
							if (!isDetailValid(item))
							{
								// 有錯誤就還原
								SerialRecords = new SelectionList<F910302Data>(backupDetails);
								return;
							}

							var addRecord = new SelectionItem<F910302Data>(item);
							SerialRecords.Add(addRecord);
						}
						msg = Messages.InfoImportSuccess;
					}
				}
			}
			catch (Exception ex)
			{
				var result = new ExDataServices.P19ExDataService.ExecuteResult();
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P9103030000_ViewModel_FileFormatError + Environment.NewLine + ex.Message;
				msg = Messages.ErrorImportFailed;
				msg.Message = msg.Message + Environment.NewLine + result.Message;
			}

			ShowMessage(msg);
		}

		private string GetOutSourceContractID(F910302Data item)
		{
			string _errMsg = string.Empty;
			//合約分類
			item.CONTRACT_TYPE = ContractTypeLists.Where(x => x.Name == item.CONTRACT_TYPENAME).Select(x => x.Value).FirstOrDefault();
			if (string.IsNullOrEmpty(item.CONTRACT_TYPE))
				return string.Format(Properties.Resources.P9103030000_ViewModel_CONTRACT_TYPENAME_Error, item.CONTRACT_TYPENAME);
			//項目類別
			item.ITEM_TYPE = ItemTypeLists.Where(x => x.Name == item.ITEM_TYPE_NAME).Select(x => x.Value).FirstOrDefault();
			if (string.IsNullOrEmpty(item.ITEM_TYPE))
				return string.Format(Properties.Resources.P9103030000_ViewModel_ITEM_TYPE_NAME_Error, item.ITEM_TYPE_NAME);
			//加工動作
			item.PROCESS_ID = PROCESSLists.Where(x => x.Name == item.PROCESS_ACT).Select(x => x.Value).FirstOrDefault();
			if (string.IsNullOrEmpty(item.PROCESS_ID))
				return string.Format(Properties.Resources.P9103030000_ViewModel_PROCESS_ACT_Error, item.PROCESS_ACT);
			return _errMsg;
		}

		private string GetCustContractID(F910302Data item)
		{
			string _errMsg = string.Empty;
			//合約分類
			item.CONTRACT_TYPE = ContractTypeLists.Where(x => x.Name == item.CONTRACT_TYPENAME).Select(x => x.Value).FirstOrDefault();
			if (string.IsNullOrEmpty(item.CONTRACT_TYPE))
				return string.Format(Properties.Resources.P9103030000_ViewModel_CONTRACT_TYPENAME_Error, item.CONTRACT_TYPENAME);
			//項目類別
			item.ITEM_TYPE = ItemTypeLists.Where(x => x.Name == item.ITEM_TYPE_NAME).Select(x => x.Value).FirstOrDefault();
			if (string.IsNullOrEmpty(item.ITEM_TYPE))
				return string.Format(Properties.Resources.P9103030000_ViewModel_ITEM_TYPE_NAME_Error, item.ITEM_TYPE_NAME);
			//項目名稱
			item.QUOTE_NO = string.Empty;

			if (QUOTELists == null)
				return Properties.Resources.P9103030000_ViewModel_QUOTEListsNull;

			if (!string.IsNullOrEmpty(item.ITEM_TYPE) && QUOTELists.ContainsKey(item.ITEM_TYPE))
			{
				item.QUOTE_NO = QUOTELists[item.ITEM_TYPE].Where(x => x.Name == item.QUOTE_NAME)
														  .Select(x => x.Value)
														  .FirstOrDefault();
			}

			if (string.IsNullOrEmpty(item.QUOTE_NO))
				return string.Format(Properties.Resources.P9103030000_ViewModel_QUOTE_NAME_Error, item.QUOTE_NAME);

			//計量單位
			if (!string.IsNullOrEmpty(item.ITEM_TYPE) && UNITLists.ContainsKey(item.ITEM_TYPE))
			{
				// 從顯示名稱找 Value
				item.UNIT_ID = UNITLists[item.ITEM_TYPE].Where(x => x.Name == item.UNIT)
														.Select(x => x.Value)
														.FirstOrDefault();
			}

			if (string.IsNullOrEmpty(item.UNIT_ID))
				return string.Format(Properties.Resources.P9103030000_ViewModel_UNIT_Error, item.UNIT);

			return _errMsg;
		}

		private bool CanImport()
		{
			bool hasFile = false;
			if (!string.IsNullOrEmpty(FullPath))
			{
				var file = new FileInfo(FullPath);
				hasFile = file.Exists;
			}
			return hasFile;
		}
		#endregion

		public bool ChangeContractObject()
		{
			//清空前確認
			if (ShowMessage(Messages.WarningBeforeChangeContractObject) != UILib.Services.DialogResponse.Yes)
				return false;

			ClearContractObject();

			if ((SerialRecords == null || !SerialRecords.Any()) &&
					(NewSerialRecord == null ||
						 (NewSerialRecord.CONTRACT_TYPE == null &&
							NewSerialRecord.ENABLE_DATE == null &&
							NewSerialRecord.DISABLE_DATE == null &&
							NewSerialRecord.ITEM_TYPE == null &&
							NewSerialRecord.QUOTE_NO == null &&
							NewSerialRecord.UNIT_ID == null &&
							NewSerialRecord.TASK_PRICE == null &&
							NewSerialRecord.WORK_HOUR == null &&
							NewSerialRecord.PROCESS_ID == null &&
							NewSerialRecord.OUTSOURCE_COST == null &&
							NewSerialRecord.APPROVE_PRICE == null)
					)
				) return true;

			BackUpSelectedData = null;
			BackUpSerialRecords = null;
			SetNewDetailRecord();
			SerialRecords = new SelectionList<F910302Data>(new List<F910302Data>());
			SelectSerialRecord = new SelectionItem<F910302Data>(new F910302Data());
			return true;
		}

		#region 複製合約
		public void CopyContract()
		{
			if (SelectedData == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SelectedData, Title = Resources.Resources.Information });
				return;
			}
			//備份主檔
			BackUpSelectedData = AutoMapper.Mapper.DynamicMap<F910301Data>(SelectedData);

			//備份副檔
			BackUpSerialRecords = new SelectionList<F910302Data>(new List<F910302Data>());
			foreach (var record in SerialRecords)
			{
				F910302Data backItem = new F910302Data();
				backItem = AutoMapper.Mapper.DynamicMap<F910302Data>(record.Item);
				BackUpSerialRecords.Add(new SelectionItem<F910302Data>(backItem));
			}

			//執行新增動作
			DoAdd();

			// 這裡設定 UserOperateMode 的原因為，原本在 SelectedData 更新後，會設定合約對象，
			// 因為是複製合約單，所以不用顯示更變對象訊息，故設定操作狀態來略過 MsgBox
			UserOperateMode = OperateMode.Query;
			//將備份資料回填
			SelectedData = AutoMapper.Mapper.DynamicMap<F910301Data>(BackUpSelectedData);
			SerialRecords = new SelectionList<F910302Data>(new List<F910302Data>());
			foreach (var newRecord in BackUpSerialRecords)
			{
				F910302Data newItem = new F910302Data();
				newItem = AutoMapper.Mapper.DynamicMap<F910302Data>(newRecord.Item);
				SerialRecords.Add(new SelectionItem<F910302Data>(newItem));
			}
			UserOperateMode = OperateMode.Add;
			//清空備份資料
			BackUpSelectedData = null;
			BackUpSerialRecords = null;
		}
		#endregion

		#endregion

		#region 取得列印報表
		public List<F910301Report> GetContractReport()
		{
			if (SelectedData == null) return null;
			var proxy = GetExProxy<P91ExDataSource>();
			var results = proxy.CreateQuery<F910301Report>("GetContractReports")
				.AddQueryExOption("dcCode", SelectedData.DC_CODE)
				.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
				.AddQueryExOption("contractNo", SelectedData.CONTRACT_NO)
				.ToList();

			return results;
		}
		#endregion

		#region 驗證

		private bool isSearchValid()
		{
			if (string.IsNullOrEmpty(SelectedDc))
			{
				if (DcList != null && DcList.Any())
					SelectedDc = DcList.First().Value;
				//ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9101040000_ViewModel_SelectDC, Title = Resources.Resources.Information });
				//return false;
			}
			if (!CRTDateS.HasValue)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SelectedTime, Title = Resources.Resources.Information });
				return false;
			}
			if (!CRTDateE.HasValue)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SelectedTime, Title = Resources.Resources.Information });
				return false;
			}
			return true;
		}
		private bool isMainRecordValid()
		{
			if (SelectedData == null) return false;

			if (string.IsNullOrEmpty(SelectedData.DC_CODE))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9101030000_ViewModel_SelectDC, Title = Resources.Resources.Information });
				return false;
			}
			if (string.IsNullOrEmpty(SelectedData.CONTRACT_NO))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_CONTRACT_NO, Title = Resources.Resources.Information });
				return false;
			}

			if (!ValidateHelper.IsMatchAZaz09(SelectedData.CONTRACT_NO))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_CONTRACT_NO_CNWordOnly, Title = Resources.Resources.Information });
				return false;
			}

			if (string.IsNullOrEmpty(SelectedData.OBJECT_TYPE))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_Contractor_SET, Title = Resources.Resources.Information });
				return false;
			}
			if (CanEditMain && SelectedData.ENABLE_DATE.Date <= DateTime.Today)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_ENABLE_DATE, Title = Resources.Resources.Information });
				return false;
			}
			if (CanEditMain && SelectedData.DISABLE_DATE.Date < SelectedData.ENABLE_DATE.Date)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_DISABLE_DATE, Title = Resources.Resources.Information });
				return false;
			}

			if (string.IsNullOrEmpty(SelectedData.UNI_FORM))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_UNI_FORM, Title = Resources.Resources.Information });
				return false;
			}

			F1909 f1909;
			F1928 f1928;
			if (!ExistsUniForm(SelectedData.OBJECT_TYPE, SelectedData.GUP_CODE, SelectedData.UNI_FORM, out f1909, out f1928))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_UNI_FORM_NotExist, Title = Resources.Resources.Information });
				return false;
			}

			return true;
		}
		private bool isDetailValid(F910302Data Item, string type = null)
		{
			if (Item == null || SelectedData == null) return false;
			if (string.IsNullOrEmpty(Item.CONTRACT_NO))
				Item.CONTRACT_NO = SelectedData.CONTRACT_NO;
			if (Item.CONTRACT_TYPE == "0")
			{
				Item.ENABLE_DATE = SelectedData.ENABLE_DATE;
				Item.DISABLE_DATE = SelectedData.DISABLE_DATE;
			}
			if (string.IsNullOrEmpty(Item.CONTRACT_TYPE) || string.IsNullOrEmpty(Item.CONTRACT_TYPENAME))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_CONTRACT_TYPENAME, Title = Resources.Resources.Information });
				return false;
			}
			// 合約對象為貨主時，需要檢查明細的項目名稱是否 存在於過濾合約主檔的 生效日期與失效日期條件的項目名稱中
			if (SelectedData.OBJECT_TYPE == "0")
			{
				if (QUOTELists == null || !QUOTELists.Any(x => x.Key == Item.ITEM_TYPE) || !QUOTELists.Where(x => x.Key == Item.ITEM_TYPE)
											 .Select(x => x.Value)
											 .SingleOrDefault().Any(x => x.Value == Item.QUOTE_NO))
				{
					ShowMessage(new MessagesStruct() { Message = string.Format(Properties.Resources.P9103030000_ViewModel_QUOTE_NAME, Item.QUOTE_NAME), Title = Resources.Resources.Information });
					return false;
				}
			}

			if (Item.CONTRACT_TYPE == "1")	//&& (!SerialRecords.Any(x => x.Item.CONTRACT_TYPE == "0"))
			{
				var contractCheckQuery = SerialRecords.Where(x => x.Item.CONTRACT_TYPE == "0");

				if (type == "editDetail")
				{
					contractCheckQuery = contractCheckQuery.Where(x => x.Item.CONTRACT_SEQ != Item.CONTRACT_SEQ);
				}

				if (!contractCheckQuery.Any())
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_contractCheckQueryFalse, Title = Resources.Resources.Information });
					return false;
				}

				if (string.IsNullOrEmpty(Item.SUB_CONTRACT_NO))
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SubcontractNo, Title = Resources.Resources.Information });
					return false;
				}

				if (!ValidateHelper.IsMatchAZaz09(Item.SUB_CONTRACT_NO))
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SubcontractNo_CNWordOnly, Title = Resources.Resources.Information });
					return false;
				}
			}
			else
			{
				Item.ENABLE_DATE = SelectedData.ENABLE_DATE;
				Item.DISABLE_DATE = SelectedData.DISABLE_DATE;
				Item.SUB_CONTRACT_NO = string.Empty;
			}

			if (string.IsNullOrEmpty(Item.ITEM_TYPE) || string.IsNullOrEmpty(Item.ITEM_TYPE_NAME))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SelectItemType, Title = Resources.Resources.Information });
				return false;
			}
			
			//生效日期不可小於等於當天
			if (string.IsNullOrEmpty(Item.CRT_STAFF) && Item.CONTRACT_TYPE == "1" && Item.ENABLE_DATE.Date <= DateTime.Now.Date)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SubcontractEffectDate_GreaterThanToday, Title = Resources.Resources.Information });
				return false;
			}
			if (string.IsNullOrEmpty(Item.CRT_STAFF) && Item.CONTRACT_TYPE == "1" && Item.DISABLE_DATE.Date < Item.ENABLE_DATE.Date)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_DISABLE_DATE, Title = Resources.Resources.Information });
				return false;
			}
			if (string.IsNullOrEmpty(Item.CRT_STAFF) && Item.CONTRACT_TYPE == "1" && (Item.ENABLE_DATE.Date < SelectedData.ENABLE_DATE ||
					Item.DISABLE_DATE.Date > SelectedData.DISABLE_DATE))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_Subcontract_ENABLE_DATE_DISABLE_DATE_Invalid, Title = Resources.Resources.Information });
				return false;
			}

			//貨主合約項目明細可有相同的項目類別、項目名稱，但生效日期、失效日期不可重疊。
			//委外商合約項目明細可有相同的項目類別、加工動作，但生效日期、失效日期不可重疊。
			//相同的項目類別、項目名稱，但生效日期、失效日期不可重疊(生效日<=Main失效日 && 失效日 >= Main生效日
			var objectTypeCheckQuery = from x in SerialRecords.Select(si => si.Item)
									   where x.ITEM_TYPE == Item.ITEM_TYPE
									   && Item.ENABLE_DATE <= x.DISABLE_DATE && Item.DISABLE_DATE >= x.ENABLE_DATE
									   select x;

			if (SelectedData.OBJECT_TYPE == "0")
			{
				// 貨主
				objectTypeCheckQuery = objectTypeCheckQuery.Where(x => x.QUOTE_NO == Item.QUOTE_NO);
			}
			else
			{
				// 委外商
				objectTypeCheckQuery = objectTypeCheckQuery.Where(x => x.PROCESS_ID == Item.PROCESS_ID);
			}

			// 編輯項目時，不算選擇的項目
			if (type == "editDetail")
			{
				objectTypeCheckQuery = objectTypeCheckQuery.Where(x => x.CONTRACT_SEQ != Item.CONTRACT_SEQ);
			}

			if (objectTypeCheckQuery.Any())
			{
				if ((type == null) || (type.ToLower() == "save" && objectTypeCheckQuery.Count() >= 2))
				{
					if (SelectedData.OBJECT_TYPE == "0")
					{
						ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_OBJECT_TYPE_Duplicate, Title = Resources.Resources.Information });
					}
					else
					{
						ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_OBJECT_TYPE_CantDuplicate, Title = Resources.Resources.Information });
					}

					return false;
				}
			}

			//var OverlapDates = SerialRecords.Where(x => x.Item.CONTRACT_TYPE == Item.CONTRACT_TYPE && x.Item.ITEM_TYPE == Item.ITEM_TYPE &&
			//																	 x.Item.QUOTE_NAME == Item.QUOTE_NAME &&
			//																	 Item.ENABLE_DATE <= x.Item.DISABLE_DATE && Item.DISABLE_DATE >= x.Item.ENABLE_DATE).ToList();
			//if (OverlapDates != null && OverlapDates.Any())
			//{
			//	if ((type == null) || (type.ToLower() == "save" && OverlapDates.Count >= 2))
			//	{
			//		ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SAME_OBJECT_TYPE_CANTDUPLICATE, Title = Resources.Resources.Information });
			//		return false;
			//	}
			//}

			// 貨主主約
			if (SelectedData.OBJECT_TYPE == "0" && Item.CONTRACT_TYPE == "0" && !isVaildCustMain(Item))
				return false;
			// 貨主附約
			if (SelectedData.OBJECT_TYPE == "0" && Item.CONTRACT_TYPE == "1" && !isVaildCustRider(Item))
				return false;
			// 委外商主約
			if (SelectedData.OBJECT_TYPE == "1" && Item.CONTRACT_TYPE == "0" && !isVaildOutSourceMain(Item))
				return false;
			// 委外商附約
			if (SelectedData.OBJECT_TYPE == "1" && Item.CONTRACT_TYPE == "1" && !isVaildOutSourceRider(Item))
				return false;

			if (SelectedData.OBJECT_TYPE == "0")
			{
				if (Item.ITEM_TYPE == "001" && (!Item.TASK_PRICE.HasValue || !Item.WORK_HOUR.HasValue
					|| Item.TASK_PRICE < 0 || Item.TASK_PRICE > 1000000000
					|| Item.WORK_HOUR < 0 || Item.WORK_HOUR > 1000000000))
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_TASK_PRICE_WORK_HOUR_OUTRANGE, Title = Resources.Resources.Information });
					return false;
				}

			}
			else
			{
				if (!Item.OUTSOURCE_COST.HasValue || !Item.APPROVE_PRICE.HasValue
					|| Item.OUTSOURCE_COST < 0 || Item.OUTSOURCE_COST > 1000000000
					|| Item.APPROVE_PRICE < 0 || Item.APPROVE_PRICE > 1000000000)
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SubcontractorSource_Price_OutRange, Title = Resources.Resources.Information });
					return false;
				}
			}

			return true;
		}
		#region 貨主主約驗證
		private bool isVaildCustMain(F910302Data Item)
		{
			if (string.IsNullOrEmpty(Item.QUOTE_NO) || string.IsNullOrEmpty(Item.QUOTE_NAME))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SELECT_QUOTE_NAME, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.ITEM_TYPE != "007" && (string.IsNullOrEmpty(Item.UNIT_ID) || string.IsNullOrEmpty(Item.UNIT)))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SELECT_UNIT_ID, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.TASK_PRICE == null && Item.ITEM_TYPE == "001")
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_TASK_PRICE, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.WORK_HOUR == null && Item.ITEM_TYPE == "001")
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_WORK_HOUR, Title = Resources.Resources.Information });
				return false;
			}
			return true;
		}
		#endregion
		#region 貨主附約驗證
		private bool isVaildCustRider(F910302Data Item)
		{
			if (string.IsNullOrEmpty(Item.QUOTE_NO) || string.IsNullOrEmpty(Item.QUOTE_NAME))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SELECT_QUOTE_NAME, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.ITEM_TYPE != "007" && (string.IsNullOrEmpty(Item.UNIT_ID) || string.IsNullOrEmpty(Item.UNIT)))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SELECT_UNIT_ID, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.TASK_PRICE == null && Item.ITEM_TYPE == "001")
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_TASK_PRICE, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.WORK_HOUR == null && Item.ITEM_TYPE == "001")
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_WORK_HOUR, Title = Resources.Resources.Information });
				return false;
			}
			if (string.IsNullOrEmpty(Item.SUB_CONTRACT_NO))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_SUB_CONTRACT_NO, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.ENABLE_DATE == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_ENABLE_DATE, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.DISABLE_DATE == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_DISENABLE_DATE, Title = Resources.Resources.Information });
				return false;
			}
			return true;
		}
		#endregion
		#region 委外商主約驗證
		private bool isVaildOutSourceMain(F910302Data Item)
		{
			//作業動作、成本價、貨主核定價
			if (string.IsNullOrEmpty(Item.PROCESS_ID) || string.IsNullOrEmpty(Item.PROCESS_ACT))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SELECT_PROCESS_ID, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.OUTSOURCE_COST == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_OUTSOURCE_COST, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.APPROVE_PRICE == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_APPROVE_PRICE, Title = Resources.Resources.Information });
				return false;
			}
			return true;
		}
		#endregion
		#region 委外商附約驗證
		private bool isVaildOutSourceRider(F910302Data Item)
		{
			//作業動作、成本價、貨主核定價、合約編號、生效日期、失效日期
			if (string.IsNullOrEmpty(Item.PROCESS_ID) || string.IsNullOrEmpty(Item.PROCESS_ACT))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SELECT_PROCESS_ID, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.OUTSOURCE_COST == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_OUTSOURCE_COST, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.APPROVE_PRICE == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_APPROVE_PRICE, Title = Resources.Resources.Information });
				return false;
			}
			if (string.IsNullOrEmpty(Item.SUB_CONTRACT_NO))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_SUB_CONTRACT_NO, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.ENABLE_DATE == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_ENABLE_DATE, Title = Resources.Resources.Information });
				return false;
			}
			if (Item.DISABLE_DATE == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_INPUT_DISENABLE_DATE, Title = Resources.Resources.Information });
				return false;
			}
			return true;
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
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearchComplete()
		{
			//指定SelectedData
			if (DgList != null && DgList.Any())
				SelectedData = DgList.FirstOrDefault();

			SearchResultIsExpanded = (DgList != null && DgList.Any());
		}

		private void DoSearch()
		{
			//執行查詢動
			//0. 檢查必填欄位
			if (!isSearchValid()) return;
			//1. 取得合約主檔資料
			DgList = GetContractDatas();
		}

		private List<F910301Data> GetContractDatas()
		{
			var proxy = GetExProxy<P91ExDataSource>();
			var results = proxy.CreateQuery<F910301Data>("GetContractDatas")
				.AddQueryExOption("dcCode", SelectedDc)
				.AddQueryExOption("gupCode", _gupCode)
				.AddQueryExOption("contractNo", SearchCONTRACT_NO)
				.AddQueryExOption("objectType", SearchObjectType)
				.AddQueryExOption("beginCreateDate", CRTDateS.Value)
				.AddQueryExOption("endCreateDate", CRTDateE.Value.AddDays(1))
				.AddQueryExOption("uniForm", SelectContractObject)
				.ToList();
			if (!results.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return null;
			}
			return results;
		}


		#endregion Search

		#region SearchDetail
		public ICommand SearchDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchDetail(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearchDetail()
		{
			// 取得項目明細資料
			SerialRecords = GetContractDetails();
			if (SerialRecords != null)
			{
				_contractDetailCount = SerialRecords.Count();
				OriContractIds = SerialRecords.Select(x => x.Item.CONTRACT_SEQ).ToList();
				RaisePropertyChanged("ContractDetailCount");
			}

		}

		private SelectionList<F910302Data> GetContractDetails()
		{
			if (SelectedData == null) return null;
			var proxy = GetExProxy<P91ExDataSource>();
			var results = proxy.CreateQuery<F910302Data>("GetContractDetails")
				.AddQueryExOption("dcCode", SelectedData.DC_CODE)
				.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
				.AddQueryExOption("contractNo", SelectedData.CONTRACT_NO)
				.ToSelectionList();

			return results;
		}
		#endregion

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
			CanEditMain = true;
			RaisePropertyChanged("DcList");
			//執行新增動作
			SetNewRecord();
			SetNewDetailRecord();
			UserOperateMode = OperateMode.Add;
			
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query
							&& SelectedData != null 
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			SelectedData = AutoMapper.Mapper.DynamicMap<F910301Data>(SelectedData);
			RaisePropertyChanged("DcList");
			CanEditMain = SelectedData.ENABLE_DATE > DateTime.Today;
			SetNewDetailRecord();
			SetQUOTE();

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
			SetDefault();

			UserOperateMode = OperateMode.Query;
			if (DgList != null && _lastF910301Data != null)
			{
				SelectedData = DgList.Where(item => item.CONTRACT_NO == _lastF910301Data.CONTRACT_NO).FirstOrDefault();
			}

		}

		private void SetDefault()
		{
			SelectedData = null;
			SelectSerialRecord = null;
			SetNewDetailRecord();
			RaisePropertyChanged("DcList");
			BackUpSelectedData = null;
			BackUpSerialRecords = null;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query
						&& SelectedData != null
						&& SelectedData.ENABLE_DATE > DateTime.Today	// 尚未生效內可刪除

					);
			}
		}

		private void DoDelete()
		{
			// 確認是否要刪除
			var msg = Messages.WarningBeforeDelete;
			msg.Message = Properties.Resources.P9103030000_ViewModel_IsDelete;
			if (ShowMessage(msg) != DialogResponse.Yes) return;

			if (SelectedData == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_SELECT_CONTRACTNO, Title = Resources.Resources.Information });
				return;
			}

			//執行刪除動作
			var proxy = GetExProxy<P91ExDataSource>();
			var result = proxy.DeleteContract(SelectedData.DC_CODE, SelectedData.GUP_CODE, SelectedData.CONTRACT_NO);

			// 成功刪除後重查資料
			if (result.IsSuccessed)
			{
				ShowMessage(Messages.DeleteSuccess);
				DoSearch();
			}
			else
			{
				ShowResultMessage(result);
			}
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(),
					() => UserOperateMode != OperateMode.Query &&
						SelectedData != null && !string.IsNullOrEmpty(SelectedData.CONTRACT_NO),
					o => DoSaveComplete(isSuccess)
					);
			}
		}

		private void DoSaveComplete(bool _isSuccess)
		{
			if (!_isSuccess) return;

			UserOperateMode = OperateMode.Query;


			//將查詢條件
			if (_lastF910301Data != null)
			{
				if (_lastF910301Data.CRT_DATE == default(DateTime))
					_lastF910301Data.CRT_DATE = DateTime.Now;

				SelectedDc = _lastF910301Data.DC_CODE;
				CRTDateS = _lastF910301Data.CRT_DATE.Date;
				CRTDateE = _lastF910301Data.CRT_DATE.Date.AddDays(1);
				SearchCONTRACT_NO = _lastF910301Data.CONTRACT_NO;
				SearchObjectType = _lastF910301Data.OBJECT_TYPE;
				SelectContractObject = _lastF910301Data.UNI_FORM;
			}

			DoSearch();
			SetDefault();
			if (DgList == null || _lastF910301Data == null) return;

			SelectedData = DgList.FirstOrDefault(x => x.CONTRACT_NO == _lastF910301Data.CONTRACT_NO);
			if (SelectedData != null)
				SelectContractObject = SelectedData.UNI_FORM;

		}

		private F910301Data _lastF910301Data = null;

		private bool DoSave()
		{
			//執行確認儲存動作
			if (SelectedData == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_DATA_NOTFOUND, Title = Resources.Resources.Information });
				return false;
			}

			ExDataMapper.Trim(SelectedData);

			//驗證
			if (!isMainRecordValid()) return false;

			//驗證
			if (SerialRecords != null)
			{
				foreach (var item in SerialRecords.Select(x => x.Item).ToList())
				{
					if (!isDetailValid(item, "save")) return false;
				}
			}

			//if (ShowMessage(Messages.WarningBeforeUpdate) != UILib.Services.DialogResponse.Yes)
			//{
			//	return false;
			//}

			wcf.ExecuteResult result = new wcf.ExecuteResult() { Message = Properties.Resources.P9103020000_ViewModel_InvalidOperate };

			var f910301Data = ExDataMapper.Map<F910301Data, wcf.F910301Data>(SelectedData);
			var f910302Datas = ExDataMapper.MapCollection<F910302Data, wcf.F910302Data>(SerialRecords.Select(item => item.Item)).ToArray();

			if (UserOperateMode == OperateMode.Add)
			{
				//新增
				if (IsExistContractData())
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_ContractExist, Title = Resources.Resources.Information });
					return false;
				}

				//執行新增
				result = DoAddContractData(f910301Data, f910302Datas);
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				//更新
				if (!IsExistContractData())
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P9103030000_ViewModel_ContractNotExist, Title = Resources.Resources.Information });
					return false;
				}
				result = DoUpdateContractData(f910301Data, f910302Datas);

			}

			if (result.IsSuccessed)
				_lastF910301Data = ExDataMapper.Clone(SelectedData);

			ShowResultMessage(result);

			return result.IsSuccessed;
		}

		private wcf.ExecuteResult DoUpdateContractData(wcf.F910301Data f910301Data, wcf.F910302Data[] f910302Datas)
		{
			//查出被刪除的ContractSeq
			var ExistContractIds = SerialRecords.Select(y => y.Item.CONTRACT_SEQ).ToList();
			DelContractIds = OriContractIds.Except(ExistContractIds).ToList();

			var proxy = new wcf.P91WcfServiceClient();
			return RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateContractData(
				f910301Data, f910302Datas, DelContractIds.ToArray()));
		}

		private wcf.ExecuteResult DoAddContractData(wcf.F910301Data f910301Data, wcf.F910302Data[] f910302Datas)
		{
			var proxy = new wcf.P91WcfServiceClient();
			return RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertContractData(
				f910301Data, f910302Datas));
		}

		private bool IsExistContractData()
		{
			var proxy = GetProxy<F91Entities>();
			var f910301 = proxy.F910301s.Where(x => x.DC_CODE == SelectedData.DC_CODE &&
																				x.GUP_CODE == SelectedData.GUP_CODE &&
																				x.CONTRACT_NO == SelectedData.CONTRACT_NO).FirstOrDefault();
			if (f910301 == null) return false;
			return true;
		}
		#endregion Save

		#region Print
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
					(t) =>
					{
						IsBusy = true;
						try
						{
							//執行預覽或列印動作
							DoPrint(t);
						}
						catch (Exception ex)
						{
							Exception = ex;
							IsBusy = false;
						}
						IsBusy = false;
					},
				(t) => !IsBusy && UserOperateMode == OperateMode.Query && (SelectedData != null));
			}
		}
		#endregion

		#region 新增明細
		public ICommand AddDetailCommand
		{
			get
			{
				return new RelayCommand(
					() => DoAddDetail(),
					() => NewSerialRecord != null && SelectedData != null && !string.IsNullOrEmpty(SelectedData.DC_CODE) 
				);
			}
		}
		#endregion

		#region 編輯明細
		public ICommand EditDetailCommand
		{
			get
			{
				return new RelayCommand(
					() => DoEditDetail(),
					() => SelectSerialRecord != null 
				);
			}
		}
		#endregion

		#region 刪除明細
		public ICommand DeleteDetailCommand
		{
			get
			{
				return new RelayCommand(
					() => DoDelDetail(),
					() => SerialRecords != null && SerialRecords.Any(si => si.IsSelected)
				);
			}
		}
		#endregion


		#region 匯入Excel明細
		public Action OnImportDetail = delegate { };
		public ICommand ImportDetailCommand
		{
			get
			{
                return new RelayCommand(() =>
                {
                    DispatcherAction(() =>
                    {
                        OnImportDetail();
                        if (string.IsNullOrEmpty(FullPath) && SelectedData != null && !string.IsNullOrEmpty(SelectedData.DC_CODE)) return;
                        DoImportData();
                    });
                });
     //           return new RelayCommand(
					//() => OnImportDetail(),
					//() => SelectedData != null && !string.IsNullOrEmpty(SelectedData.DC_CODE),
     //               DoImportData()
     //           );
			}
		}
		#endregion


		#endregion

		/// <summary>
		/// 當選擇報價單項目後帶出單位。加工作業類別要帶出單價(貨主核定價)
		/// </summary>
		internal void SetSelectedQuoteData()
		{
			if (NewSerialRecord == null || SelectedData == null)
				return;

			//001	加工計價
			//002	儲位計價
			//003	作業計價
			//004	出貨計價
			//005	派車計價
			//006	其他項目計價
			//007	專案計價
			switch (NewSerialRecord.ITEM_TYPE)
			{
				case "001":
					NewSerialRecord.TASK_PRICE = GetApprovedPriceByF910401();
					NewSerialRecord.WORK_HOUR = GetWorkHourByF910402();
					break;
				case "002":
				case "003":
				case "004":
				case "005":
				case "006":
					NewSerialRecord.UNIT_ID = GetAccUnitByF500101To5();
					break;
			}
		}

		/// <summary>
		/// 貨主核定價
		/// </summary>
		/// <returns></returns>
		decimal? GetApprovedPriceByF910401()
		{
			var proxy = GetProxy<F91Entities>();

			var query = from item in proxy.F910401s
						where (item.DC_CODE == SelectedData.DC_CODE || item.DC_CODE == "000")
						where item.GUP_CODE == SelectedData.GUP_CODE
						where item.CUST_CODE == SelectedData.CUST_CODE
						where item.QUOTE_NO == NewSerialRecord.QUOTE_NO
						select item;

			var list = query.ToList();
			return list.Select(x => x.APPROVED_PRICE).FirstOrDefault();
		}

		/// <summary>
		/// 工時
		/// </summary>
		/// <returns></returns>
		int? GetWorkHourByF910402()
		{
			var proxy = GetProxy<F91Entities>();
			var query = from item in proxy.F910402s
									where (item.DC_CODE == SelectedData.DC_CODE || item.DC_CODE == "000")
									where item.GUP_CODE == SelectedData.GUP_CODE
									where item.CUST_CODE == SelectedData.CUST_CODE
									where item.QUOTE_NO == NewSerialRecord.QUOTE_NO
									select item;
			var list = query.ToList();
			return list.Sum(x => x.WORK_HOUR);
		}

		/// <summary>
		/// 單位
		/// </summary>
		/// <returns></returns>
		string GetAccUnitByF500101To5()
		{
			var query = from item in GetIF500101To5DataServiceQuery(NewSerialRecord.ITEM_TYPE)
						where (item.DC_CODE == SelectedData.DC_CODE || item.DC_CODE == "000")
						where item.GUP_CODE == SelectedData.GUP_CODE
						where item.CUST_CODE == SelectedData.CUST_CODE
						where item.QUOTE_NO == NewSerialRecord.QUOTE_NO
						select item;

			var list = query.ToList();
			return list.Select(x => x.ACC_UNIT).FirstOrDefault();
		}
	}
}
