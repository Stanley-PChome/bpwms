using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7105050000_ViewModel : InputViewModelBase
	{
		public P7105050000_ViewModel()
		{
			Init();
		}

		#region Action
		public Action SetQueryFocus = delegate { };
		public Action SetAddNewFocus = delegate { };
		public Action LockDelvTmprComboBox = delegate { };
		public Action UnlockDelvTmprComboBox = delegate { };
		public Action ResetUI = delegate { };
		public Action ScrollToTop = delegate { };
		#endregion

		#region Common Method/Property
		private void Init()
		{
			//物流中心
			var dcList = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
			dcList.Insert(0, new NameValuePair<string>(Properties.Resources.P7105010000_ViewModel_NONE_SPECIFY, "000"));
			DcList = dcList;
			if (DcList != null && DcList.Any()) SelectedDcForSearch = DcList.First().Value;

			//計價項目
			AccItemKindList = GetAccItemKindList();			
			AddAllToList(AccItemKindList);		
			if (AccItemKindList != null && AccItemKindList.Any()) SelectedAccItemKind = AccItemKindList.First().Value;

			//物流類別
			LogiTypeList = GetLogiTypeList();
			AddAllToList(LogiTypeList);
			if (LogiTypeList != null && LogiTypeList.Any()) SelectedLogiType = LogiTypeList.First().Value;		

			//稅別
			TaxTypeList = GetTaxTypeList();
			AddAllToList(TaxTypeList);
			if (TaxTypeList != null && TaxTypeList.Any()) SelectedTaxType = TaxTypeList.First().Value;

			//計價方式
			AccKindList = GetAccKindList();
			AddAllToList(AccKindList);
			if (AccKindList != null && AccKindList.Any()) SelectedAccKind = AccKindList.First().Value;

			//專車
			SpecialCarTypeList = GetSpecialCarTypeList();
			if (SpecialCarTypeList != null && SpecialCarTypeList.Any()) SelectedSpecialCar = SpecialCarTypeList.First().Value;

			//狀態
			StatusList = GetStatusList();
			if (StatusList != null && StatusList.Any()) SelectedStatus = StatusList.First().Value;

			//配送計價類別 
			DelvAccTypeList = GetDelvAccTypeList();

			//配送效率
			DelvEfficList = GetDelvEfficList();

			//計價單位
			AccUnitList = GetAccUnitList();

			//車輛種類
			CarKindList = GetCarKindList();

			//配送溫層
			DelvTmprList = GetDelvTmprList();

			IsSearchConditionBlockShow = true;

			//計價方式四個區塊的控制
			AccKindValue = "E";
			
		}

		private void AddAllToList(List<NameValuePair<string>> tmpList)
		{
			tmpList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });
		}

		private void RemoveAllFromList(List<NameValuePair<string>> tmpList)
		{
			int item = 0;
			item = tmpList.Where(o => o.Name == Resources.Resources.All).Count();
			if (item > 0)
				tmpList.RemoveRange(0, item);

		}

		private bool _isSearchConditionBlockShow = true;

		public bool IsSearchConditionBlockShow
		{
			get { return _isSearchConditionBlockShow; }
			set
			{
				_isSearchConditionBlockShow = value;
				RaisePropertyChanged("IsSearchConditionBlockShow");
			}
		}

		private string _accKindValue;

		public string AccKindValue
		{
			get { return _accKindValue; }
			set
			{
				_accKindValue = value;
				RaisePropertyChanged("AccKindValue");
			}
		}
		#endregion

		#region 資料來源
		#region 物流中心
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.OrderBy(x => x.Value).ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}

		//選取的物流中心-查詢
		private string _selectedDcForSearch = string.Empty;

		public string SelectedDcForSearch
		{
			get { return _selectedDcForSearch; }
			set
			{
				if (_selectedDcForSearch == value) return;
				Set(() => SelectedDcForSearch, ref _selectedDcForSearch, value);

				//計費區域
				AccAreaList = GetAccAreaList(_selectedDcForSearch);

				if (UserOperateMode == OperateMode.Query)
				{
					if (SearchResults != null) SearchResults.Clear();
					SelectedSearchResult = null;
					IsSpecialCarForSearchResult = false;
					SelectedCarKindForSearchResult = null;
					CarTonneForSearchResult = null;
					TemperatureForSearchResult = null;
				}
			}
		}

		//選取的物流中心-新增
		private string _selectedDcForAdd = string.Empty;

		public string SelectedDcForAdd
		{
			get { return _selectedDcForAdd; }
			set
			{
				if (_selectedDcForAdd == value) return;
				Set(() => SelectedDcForAdd, ref _selectedDcForAdd, value);

				if (UserOperateMode == OperateMode.Add)
				{
					CurrentRecord.DC_CODE = _selectedDcForAdd;

					//計費區域
					AccAreaList = GetAccAreaList(_selectedDcForAdd);
					CurrentRecord.ACC_AREA_ID = 0;

				}
			}
		}

		#endregion

		#region 計價項目(F91000301 WhereItemTypeId=005)
		private List<NameValuePair<string>> _accItemKindList;

		public List<NameValuePair<string>> AccItemKindList
		{
			get { return _accItemKindList; }
			set
			{
				if (_accItemKindList == value) return;
				Set(() => AccItemKindList, ref _accItemKindList, value);
			}
		}

		//選取的計價項目
		private string _selectedAccItemKind;

		public string SelectedAccItemKind
		{
			get { return _selectedAccItemKind; }
			set
			{
				if (_selectedAccItemKind == value) return;
				Set(() => SelectedAccItemKind, ref _selectedAccItemKind, value);
			}
		}

		//取得計價項目
		private List<NameValuePair<string>> GetAccItemKindList()
		{
			var proxy = GetExProxy<P71ExDataSource>();
			var results = proxy.CreateQuery<F91000301Data>("GetAccItemKinds")
							.AddQueryExOption("itemTypeId", "005")
							.ToList()
							.Select(x => new NameValuePair<string>()
							{
								Name = x.ACC_ITEM_KIND_NAME,
								Value = x.ACC_ITEM_KIND_ID
							}
							).ToList();

			
			return results;
		}
		#endregion

		#region 物流類別(01:正物流 02:逆物流)  F000904
		private List<NameValuePair<string>> _logiTypeList;

		public List<NameValuePair<string>> LogiTypeList
		{
			get { return _logiTypeList; }
			set
			{
				if (_logiTypeList == value) return;
				Set(() => LogiTypeList, ref _logiTypeList, value);
			}
		}

		//選取的物流類別
		private string _selectedLogiType;

		public string SelectedLogiType
		{
			get { return _selectedLogiType; }
			set
			{
				if (_selectedLogiType == value) return;
				Set(() => SelectedLogiType, ref _selectedLogiType, value);
			}
		}

		//取得物流類別
		private List<NameValuePair<string>> GetLogiTypeList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F199005", "LOGI_TYPE");			
			return data;
		}
		#endregion		

		#region 稅別(0:未稅 1:含稅)F000904
		private List<NameValuePair<string>> _taxTypeList;

		public List<NameValuePair<string>> TaxTypeList
		{
			get { return _taxTypeList; }
			set
			{
				if (_taxTypeList == value) return;
				Set(() => TaxTypeList, ref _taxTypeList, value);
			}
		}

		//選取的稅別
		private string _selectedTaxType;

		public string SelectedTaxType
		{
			get { return _selectedTaxType; }
			set
			{
				if (_selectedTaxType == value) return;
				Set(() => SelectedTaxType, ref _selectedTaxType, value);
			}
		}

		//取得稅別
		private List<NameValuePair<string>> GetTaxTypeList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F199005", "IN_TAX");
			
			return data;
		}
		#endregion

		#region 計價方式(A:均一價 B:實際尺寸 C:材積 D:重量)F000904
		private List<NameValuePair<string>> _accKindList;

		public List<NameValuePair<string>> AccKindList
		{
			get { return _accKindList; }
			set
			{
				if (_accKindList == value) return;
				Set(() => AccKindList, ref _accKindList, value);
			}
		}

		//選取的計價方式
		private string _selectedAccKind;

		public string SelectedAccKind
		{
			get { return _selectedAccKind; }
			set
			{
				if (_selectedAccKind == value) return;
				Set(() => SelectedAccKind, ref _selectedAccKind, value);
			}
		}

		//取得計價方式
		private List<NameValuePair<string>> GetAccKindList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F199005", "ACC_KIND");
		
			return data;
		}
		#endregion

		#region 專車(-1:全部 1:是 0:否)
		private List<NameValuePair<string>> _specialCarTypeList;

		public List<NameValuePair<string>> SpecialCarTypeList
		{
			get { return _specialCarTypeList; }
			set
			{
				if (_specialCarTypeList == value) return;
				Set(() => SpecialCarTypeList, ref _specialCarTypeList, value);
			}
		}

		//選取的專車
		private string _selectedSpecialCar;

		public string SelectedSpecialCar
		{
			get { return _selectedSpecialCar; }
			set
			{
				if (_selectedSpecialCar == value) return;
				Set(() => SelectedSpecialCar, ref _selectedSpecialCar, value);
			}
		}

		//取得專車
		private List<NameValuePair<string>> GetSpecialCarTypeList()
		{
			var data = new List<NameValuePair<string>>();
			data.Add(new NameValuePair<string>(Resources.Resources.All, "-1"));
			data.Add(new NameValuePair<string>(Resources.Resources.Yes, "1"));
			data.Add(new NameValuePair<string>(Resources.Resources.No, "0"));
			return data;
		}
		#endregion

		#region 狀態(0:使用中 9:刪除)
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

		//選取的狀態
		private string _selectedStatus;

		public string SelectedStatus
		{
			get { return _selectedStatus; }
			set
			{
				if (_selectedStatus == value) return;
				Set(() => SelectedStatus, ref _selectedStatus, value);
			}
		}

		//取得狀態
		private List<NameValuePair<string>> GetStatusList()
		{
			var data = new List<NameValuePair<string>>();
			data.Add(new NameValuePair<string>(Properties.Resources.P7105050000_ViewModel_Using, "0"));
			data.Add(new NameValuePair<string>(Resources.Resources.Delete, "9"));
			return data;
		}
		#endregion

		#region 配送計價類別 F000904(01:無 02:宅配 03:統倉 04:門店)
		private List<NameValuePair<string>> _delvAccTypeList;

		public List<NameValuePair<string>> DelvAccTypeList
		{
			get { return _delvAccTypeList; }
			set
			{
				if (_delvAccTypeList == value) return;
				Set(() => DelvAccTypeList, ref _delvAccTypeList, value);
			}
		}

		//取得配送計價類別
		private List<NameValuePair<string>> GetDelvAccTypeList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F91000301", "DELV_ACC_TYPE");
			return data;
		}
		#endregion

		#region 配送效率(01:一般、02:3小時、03:6小時、04:9小時)F000904
		private List<NameValuePair<string>> _delvEfficList;

		public List<NameValuePair<string>> DelvEfficList
		{
			get { return _delvEfficList; }
			set
			{
				if (_delvEfficList == value) return;
				Set(() => DelvEfficList, ref _delvEfficList, value);
			}
		}

		//取得配送效率
		private List<NameValuePair<string>> GetDelvEfficList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F190102", "DELV_EFFIC");
			return data;
		}
		#endregion

		#region 計價單位(F91000302 Where ItemTypeId=005)
		private ObservableCollection<F91000302Data> allAccUnitList;
		private ObservableCollection<F91000302Data> _accUnitList;

		public ObservableCollection<F91000302Data> AccUnitList
		{
			get { return _accUnitList; }
			set
			{
				if (_accUnitList == value) return;
				Set(() => AccUnitList, ref _accUnitList, value);
			}
		}

		//取得計價單位
		public ObservableCollection<F91000302Data> GetAccUnitList(string logiType = "01")
		{
			if (allAccUnitList == null || !allAccUnitList.Any())
			{
				var proxy = GetExProxy<P19ExDataSource>();
				var results = proxy.CreateQuery<F91000302Data>("GetAccUnitList")
					.AddQueryExOption("itemTypeId", "005").ToObservableCollection();
				allAccUnitList = results;
				return results;
			}

			if (logiType == "02") //逆物流單位不可選箱
			{
				return allAccUnitList.Where(n => n.ACC_UNIT != "01").ToObservableCollection();
			}
			return allAccUnitList;
		}
		#endregion
		#endregion

		#region 查詢結果
		private ObservableCollection<F199005Data> _searchResults;

		public ObservableCollection<F199005Data> SearchResults
		{
			get { return _searchResults; }
			set
			{
				if (_searchResults == value) return;
				Set(() => SearchResults, ref _searchResults, value);
			}
		}

		//選取的查詢結果
		private F199005Data _selectedSearchResult;

		public F199005Data SelectedSearchResult
		{
			get { return _selectedSearchResult; }
			set
			{
				if (_selectedSearchResult == null) AccKindValue = "A";
				if (_selectedSearchResult == value) return;

				Set(() => SelectedSearchResult, ref _selectedSearchResult, value);
				if (_selectedSearchResult == null) return;

				IsSpecialCarForSearchResult = _selectedSearchResult.IS_SPECIAL_CAR == "1" ? true : false;

				if (IsSpecialCarForSearchResult)
				{
					SelectedCarKindForSearchResult = CarKindList.Where(o => o.CAR_KIND_ID == _selectedSearchResult.CAR_KIND_ID).FirstOrDefault();

					if (SelectedCarKindForSearchResult != null)
					{
						CarTonneForSearchResult = SelectedCarKindForSearchResult.CAR_SIZE;
						TemperatureForSearchResult = SelectedCarKindForSearchResult.TMPR_TYPE_TEXT;
					}
				}
				else
				{
					SelectedCarKindForSearchResult = null;
					CarTonneForSearchResult = String.Empty;
					TemperatureForSearchResult = String.Empty;
				}

				if (String.IsNullOrEmpty(_selectedSearchResult.ACC_KIND))
				{
					AccKindValue = "A";
				}
				else
				{
					AccKindValue = _selectedSearchResult.ACC_KIND;
				}
			}
		}

		//取得查詢結果	
		private ObservableCollection<F199005Data> GetSearchResults()
		{
			var proxyEx = GetExProxy<P19ExDataSource>();
			ObservableCollection<F199005Data> results = null;

			if (UserOperateMode == OperateMode.Add && CurrentRecord != null)
			{
				SelectedDcForAdd = CurrentRecord.DC_CODE;

				results = proxyEx.CreateQuery<F199005Data>("GetF199005")
						.AddQueryExOption("dcCode", SelectedDcForAdd)
						.AddQueryExOption("accItemKindId", CurrentRecord.ACC_ITEM_KIND_ID)
						.AddQueryExOption("logiType", CurrentRecord.LOGI_TYPE)						
						.AddQueryExOption("taxType", CurrentRecord.IN_TAX)
						.AddQueryExOption("accKind", CurrentRecord.ACC_KIND)
						.AddQueryExOption("isSpecialCar", CurrentRecord.IS_SPECIAL_CAR)
						.AddQueryExOption("status", CurrentRecord.STATUS)
						.ToObservableCollection();
			}
			else
			{
				if (UserOperateMode == OperateMode.Edit && CurrentRecord != null)
				{
					SelectedTaxType = CurrentRecord.IN_TAX;
				}

				results = proxyEx.CreateQuery<F199005Data>("GetF199005")
						.AddQueryExOption("dcCode", SelectedDcForSearch)
						.AddQueryExOption("accItemKindId", SelectedAccItemKind)
						.AddQueryExOption("logiType", SelectedLogiType)						
						.AddQueryExOption("taxType", SelectedTaxType)
						.AddQueryExOption("accKind", SelectedAccKind)
						.AddQueryExOption("isSpecialCar", SelectedSpecialCar)
						.AddQueryExOption("status", SelectedStatus)
						.ToObservableCollection();
			}

			return results;
		}

		#region 查詢結果-詳細內容區塊
		//是否為專車
		private bool _isSpecialCarForSearchResult;

		public bool IsSpecialCarForSearchResult
		{
			get { return _isSpecialCarForSearchResult; }
			set
			{
				if (_isSpecialCarForSearchResult == value) return;
				Set(() => IsSpecialCarForSearchResult, ref _isSpecialCarForSearchResult, value);
			}
		}

		//車輛種類
		private F194702Data _selectedCarKindForSearchResult;

		public F194702Data SelectedCarKindForSearchResult
		{
			get { return _selectedCarKindForSearchResult; }
			set
			{
				if (_selectedCarKindForSearchResult == value) return;
				Set(() => SelectedCarKindForSearchResult, ref _selectedCarKindForSearchResult, value);
			}
		}

		//噸數
		private string _carTonneForSearchResult;

		public string CarTonneForSearchResult
		{
			get { return _carTonneForSearchResult; }
			set
			{
				if (_carTonneForSearchResult == value) return;
				Set(() => CarTonneForSearchResult, ref _carTonneForSearchResult, value);
			}
		}

		//溫層
		private string _temperatureForSearchResult;

		public string TemperatureForSearchResult
		{
			get { return _temperatureForSearchResult; }
			set
			{
				if (_temperatureForSearchResult == value) return;
				Set(() => TemperatureForSearchResult, ref _temperatureForSearchResult, value);
			}
		}
		#endregion
		#endregion

		#region Search Command
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			SearchResults = GetSearchResults();
		}


		private void DoSearchComplete()
		{
			if (SearchResults == null || SearchResults.Count == 0)
			{
				ShowWarningMessage(Resources.Resources.InfoNoData);
				SelectedSearchResult = null;
			}
			else
			{
				if (UserOperateMode == OperateMode.Query)
				{
					SelectedSearchResult = SearchResults.First();
					ScrollToTop();
				}
				else
				{
					var target = SearchResults.Where(x => x.DC_CODE == CurrentRecord.DC_CODE &&
												x.ACC_ITEM_KIND_ID == CurrentRecord.ACC_ITEM_KIND_ID &&												
												x.LOGI_TYPE == CurrentRecord.LOGI_TYPE &&
												x.ACC_KIND == CurrentRecord.ACC_KIND &&
												x.DELV_EFFIC == CurrentRecord.DELV_EFFIC &&
												x.IS_SPECIAL_CAR == CurrentRecord.IS_SPECIAL_CAR &&
												x.ACC_UNIT == CurrentRecord.ACC_UNIT &&
												x.ACC_NUM == CurrentRecord.ACC_NUM &&
												x.DELV_ACC_TYPE == CurrentRecord.DELV_ACC_TYPE &&
												x.ACC_AREA_ID == CurrentRecord.ACC_AREA_ID &&
												x.DELV_TMPR == CurrentRecord.DELV_TMPR
												).SingleOrDefault();

					SelectedSearchResult = target;

					if (SelectedSearchResult != null)
					{
						SelectedDcForSearch = SelectedSearchResult.DC_CODE;
						SelectedAccItemKind = SelectedSearchResult.ACC_ITEM_KIND_ID;
						SelectedLogiType = SelectedSearchResult.LOGI_TYPE;						
						SelectedTaxType = SelectedSearchResult.IN_TAX;
						SelectedAccKind = SelectedSearchResult.ACC_KIND;
						SelectedSpecialCar = SelectedSearchResult.IS_SPECIAL_CAR;
						SelectedStatus = SelectedSearchResult.STATUS;
					}
				}
			}

			UserOperateMode = OperateMode.Query;
			IsSearchConditionBlockShow = false;
			SetQueryFocus();
		}
		#endregion

		#region 新增/編輯 DataModel
		#region F199005 編輯標的
		private F199005 _currentRecord;

		public F199005 CurrentRecord
		{
			get { return _currentRecord; }
			set
			{
				if (_currentRecord == value) return;
				Set(() => CurrentRecord, ref _currentRecord, value);
			}
		}
		#endregion

		#region 配送計價類別清單
		private List<NameValuePair<string>> _delvAccTypeAddList;
		public List<NameValuePair<string>> DelvAccTypeAddList
		{
			get { return _delvAccTypeAddList; }
			set
			{
				if (_delvAccTypeAddList == value) return;
				Set(() => DelvAccTypeAddList, ref _delvAccTypeAddList, value);
			}
		}

		//依照計價項目取得配送計價類別
		public List<NameValuePair<string>> GetDelvAccTypeList(string accItemKind)
		{
			if (String.IsNullOrEmpty(accItemKind)) return null;

			var proxy = GetExProxy<P71ExDataSource>();
			var results = proxy.CreateQuery<F000904DelvAccType>("GetDelvAccTypes")
							.AddQueryExOption("itemTypeId", "005")
							.AddQueryExOption("accItemKindId", accItemKind)
							.ToList()
							.Select(x => new NameValuePair<string>()
							{
								Name = x.DELV_ACC_TYPE_NAME,
								Value = x.DELV_ACC_TYPE
							}
							).ToList();
			return results;
		}
		#endregion

		#region 專車區塊
		//是否為專車
		private bool _isSpecialCar;

		public bool IsSpecialCar
		{
			get { return _isSpecialCar; }
			set
			{
				CurrentRecord.IS_SPECIAL_CAR = value ? "1" : "0";
				if (_isSpecialCar == value) return;
				Set(() => IsSpecialCar, ref _isSpecialCar, value);

				if (_isEditFirstLoad)
				{
					return;
				}

				if (_isSpecialCar)
				{
					if (CarKindList != null && CarKindList.Count > 0)
					{
						SelectedCarKind = CarKindList[0];
					}
					else
					{
						SelectedCarKind = null;
					}
				}
				else
				{
					SelectedCarKind = null;
				}
			}
		}

		//車輛種類
		private ObservableCollection<F194702Data> _carKindList;

		public ObservableCollection<F194702Data> CarKindList
		{
			get { return _carKindList; }
			set
			{
				if (_carKindList == value) return;
				Set(() => CarKindList, ref _carKindList, value);
			}
		}

		//選取的車輛種類
		private F194702Data _selectedCarKind;

		public F194702Data SelectedCarKind
		{
			get { return _selectedCarKind; }
			set
			{
				if (_selectedCarKind == value) return;
				Set(() => SelectedCarKind, ref _selectedCarKind, value);

				if (_isEditFirstLoad)
				{
					return;
				}

				if (SelectedCarKind == null)
				{
					CurrentRecord.ACC_UNIT = null;
					CurrentRecord.ACC_AREA_ID = 0;
					CurrentRecord.CAR_KIND_ID = null;
					CarTonne = String.Empty;
					Temperature = String.Empty;

					//解除配送溫層
					CurrentRecord.DELV_TMPR = null;
					UnlockDelvTmprComboBox();

					//計價單位加回01
					Add01ToAccUnitList();
				}
				else
				{
					//連動更新 CarTonne，Temperature
					CurrentRecord.CAR_KIND_ID = SelectedCarKind.CAR_KIND_ID;
					CarTonne = SelectedCarKind.CAR_SIZE;
					Temperature = SelectedCarKind.TMPR_TYPE_TEXT;

					//限制配送溫層
					CurrentRecord.DELV_TMPR = SelectedCarKind.TMPR_TYPE;
					LockDelvTmprComboBox();

					//計價單位排除01
					Remove01FromAccUnitList();
				}
			}
		}

		//取得車輛種類
		private ObservableCollection<F194702Data> GetCarKindList()
		{
			var proxy = GetExProxy<P19ExDataSource>();
			var results = proxy.CreateQuery<F194702Data>("GetF194702").ToList();
			return results.ToObservableCollection();
		}

		//噸數
		private string _carTonne;

		public string CarTonne
		{
			get { return _carTonne; }
			set
			{
				if (_carTonne == value) return;
				Set(() => CarTonne, ref _carTonne, value);
			}
		}

		//溫層
		private string _temperature;

		public string Temperature
		{
			get { return _temperature; }
			set
			{
				if (_temperature == value) return;
				Set(() => Temperature, ref _temperature, value);
			}
		}

		//計費區域
		private List<NameValuePair<string>> _accAreaList;

		public List<NameValuePair<string>> AccAreaList
		{
			get { return _accAreaList; }
			set
			{
				if (_accAreaList == value) return;
				Set(() => AccAreaList, ref _accAreaList, value);
			}
		}

		//取得計費區域清單
		private List<NameValuePair<string>> GetAccAreaList(string dc)
		{
			var proxy = GetExProxy<P19ExDataSource>();
			var results = proxy.CreateQuery<F1948Data>("GetF1948")
							.AddQueryExOption("dcCode", dc)
							.ToList()
							.Select(x => new NameValuePair<string>()
							{
								Name = x.ACC_AREA,
								Value = x.ACC_AREA_ID.ToString()
							}
							).ToList();

			results.Insert(0, new NameValuePair<string>(Resources.Resources.All, "0"));

			if (CurrentRecord != null)
				CurrentRecord.ACC_AREA_ID = 0;

			return results;
		}

		private void Add01ToAccUnitList()
		{
			if (AccUnitList.Count == 0)
			{
				AccUnitList.Insert(0, new F91000302Data() { ACC_UNIT_NAME = Properties.Resources.P7105050000_ViewModel_Count, ACC_UNIT = "01" });
			}
			else
			{
				if (AccUnitList[0].ACC_UNIT_NAME != Properties.Resources.P7105050000_ViewModel_Count && AccUnitList[0].ACC_UNIT != "01")
				{
					AccUnitList.Insert(0, new F91000302Data() { ACC_UNIT_NAME = Properties.Resources.P7105050000_ViewModel_Count, ACC_UNIT = "01" });
				}
			}
		}

		private void Remove01FromAccUnitList()
		{
			if (AccUnitList.Count > 0 && AccUnitList[0].ACC_UNIT_NAME == Properties.Resources.P7105050000_ViewModel_Count && AccUnitList[0].ACC_UNIT == "01")
			{
				AccUnitList.RemoveAt(0);
			}
		}
		#endregion

		#region 配送溫層(A:常溫、B：低溫)F000904
		private List<NameValuePair<string>> _delvTmprList;

		public List<NameValuePair<string>> DelvTmprList
		{
			get { return _delvTmprList; }
			set
			{
				if (_delvTmprList == value) return;
				Set(() => DelvTmprList, ref _delvTmprList, value);
			}
		}

		//取得配送溫層
		private List<NameValuePair<string>> GetDelvTmprList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F199005", "DELV_TMPR");
			return data;
		}
		#endregion

		#endregion

		#region Add Command
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query, o => DoAddComplete()
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;



		}

		private void DoAddComplete()
		{

			CurrentRecord = new F199005();
			CurrentRecord.DC_CODE = SelectedDcForAdd = DcList[0].Value;
			CurrentRecord.ACC_NUM = 1;
			CurrentRecord.MAX_WEIGHT = 1;
			CurrentRecord.ITEM_TYPE_ID = "005";
			CurrentRecord.STATUS = "0";
			CurrentRecord.IS_SPECIAL_CAR = "0";

			#region 移除Resources.Resources.All
			RemoveAllFromList(AccItemKindList);
			RemoveAllFromList(LogiTypeList);			
			RemoveAllFromList(TaxTypeList);
			RemoveAllFromList(AccKindList);
			#endregion

			IsSpecialCar = false;

			//專車
			SelectedCarKind = null;
			CarTonne = Temperature = String.Empty;
			ResetUI();
			SetAddNewFocus();
		}
		#endregion

		#region Cancel Command
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query, o => DoCancelComplete()
					);
			}
		}

		private void DoCancel()
		{
			CurrentRecord = null;
			UserOperateMode = OperateMode.Query;
		}

		private void DoCancelComplete()
		{
			#region 加入Resources.Resources.All
			AddAllToList(AccItemKindList);
			AddAllToList(LogiTypeList);			
			AddAllToList(TaxTypeList);
			AddAllToList(AccKindList);
			#endregion

			Add01ToAccUnitList();
		}

		#endregion

		#region Edit Command

		private bool _isEditFirstLoad = false;

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && SelectedSearchResult != null && SelectedSearchResult.STATUS != "9",
					o => DoEditComplete()
					);
			}
		}

		private void DoEdit()
		{
			if (SelectedSearchResult == null)
			{
				ShowWarningMessage(Properties.Resources.P7105020000_ViewModel_SelectData);
				return;
			}

			#region 移除Resources.Resources.All
			RemoveAllFromList(AccItemKindList);
			RemoveAllFromList(LogiTypeList);			
			RemoveAllFromList(TaxTypeList);
			RemoveAllFromList(AccKindList);
			RemoveAllFromList(TaxTypeList);
			#endregion

			
			UserOperateMode = OperateMode.Edit;
			CurrentRecord = ExDataMapper.Map<F199005Data, F199005>(SelectedSearchResult);
			SelectedDcForAdd = CurrentRecord.DC_CODE;
			_isEditFirstLoad = true;
		}

		private void DoEditComplete()
		{
			IsSpecialCar = CurrentRecord.IS_SPECIAL_CAR == "1" ? true : false;

			if (IsSpecialCar)
			{
				if (CarKindList != null)
				{
					SelectedCarKind = CarKindList.Where(o => o.CAR_KIND_ID == CurrentRecord.CAR_KIND_ID).FirstOrDefault();

					if (SelectedCarKind != null)
					{
						CarTonne = SelectedCarKind.CAR_SIZE;
						Temperature = SelectedCarKind.TMPR_TYPE_TEXT;
						LockDelvTmprComboBox();
					}
				}
			}
			else
			{
				SelectedCarKind = null;
				CarTonne = String.Empty;
				Temperature = String.Empty;
				UnlockDelvTmprComboBox();
			}

			_isEditFirstLoad = false;
		}

		#endregion

		#region Delete Command
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query && SelectedSearchResult != null &&
						SelectedSearchResult.STATUS != "9",
					o => DoDeleteComplete()
					);
			}
		}

		private void DoDelete()
		{
			if (SelectedSearchResult != null)
			{
				if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
				var proxy = GetModifyQueryProxy<F19Entities>();

				var target = proxy.F199005s.Where(x => x.DC_CODE == SelectedSearchResult.DC_CODE &&
												x.ACC_ITEM_KIND_ID == SelectedSearchResult.ACC_ITEM_KIND_ID &&												
												x.LOGI_TYPE == SelectedSearchResult.LOGI_TYPE &&
												x.ACC_KIND == SelectedSearchResult.ACC_KIND &&
												x.DELV_EFFIC == SelectedSearchResult.DELV_EFFIC &&
												x.IS_SPECIAL_CAR == SelectedSearchResult.IS_SPECIAL_CAR &&
												x.ACC_UNIT == SelectedSearchResult.ACC_UNIT &&
												x.DELV_ACC_TYPE == SelectedSearchResult.DELV_ACC_TYPE && x.STATUS=="0").FirstOrDefault();

				if (target == null)
				{
					ShowMessage(Messages.WarningBeenDeleted);
				}
				else
				{
					target.STATUS = "9";
					proxy.UpdateObject(target);
					proxy.SaveChanges();
					SelectedSearchResult.STATUS = "9";
					ShowMessage(Messages.InfoDeleteSuccess);
				}
			}
		}

		private void DoDeleteComplete()
		{
			UserOperateMode = OperateMode.Query;
			SearchCommand.Execute(null);
		}
		#endregion

		#region Save Command


		public ICommand SaveCommand
		{
			get
			{
				bool isSaveSuccess = false;

				return CreateBusyAsyncCommand(
					o => isSaveSuccess = DoSave(), () => UserOperateMode != OperateMode.Query, o => DoSaveComplete(isSaveSuccess)
					);
			}
		}

		private bool DoSave()
		{
			if (!CheckInputData()) return false;

			var proxy = GetProxy<F19Entities>();

			var f199005 = proxy.F199005s.Where(x => x.DC_CODE == CurrentRecord.DC_CODE &&
												x.ACC_ITEM_KIND_ID == CurrentRecord.ACC_ITEM_KIND_ID &&
												x.ACC_AREA_ID == CurrentRecord.ACC_AREA_ID &&				
												x.LOGI_TYPE == CurrentRecord.LOGI_TYPE &&
												x.ACC_KIND == CurrentRecord.ACC_KIND &&
												x.DELV_EFFIC == CurrentRecord.DELV_EFFIC &&
												x.IS_SPECIAL_CAR == CurrentRecord.IS_SPECIAL_CAR &&
												x.ACC_UNIT == CurrentRecord.ACC_UNIT &&
												x.DELV_ACC_TYPE == CurrentRecord.DELV_ACC_TYPE &&
												x.DELV_TMPR == CurrentRecord.DELV_TMPR && x.STATUS =="0").SingleOrDefault();

			if (UserOperateMode == OperateMode.Add)
			{
				if (f199005 != null)
				{
					ShowMessage(Messages.WarningExist);
					return false;
				}

				ExDataMapper.Trim(CurrentRecord);

				var proxyWcf = new wcf.P71WcfServiceClient();
				var wcf199005 = ExDataMapper.Map<F199005, wcf.F199005>(CurrentRecord);
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
								() => proxyWcf.InsertF199005(wcf199005));

				if (result.IsSuccessed == false)
				{
					DialogService.ShowMessage(result.Message);
					return false;
				}

				//proxy.AddToF199005s(CurrentRecord);
			}
			else
			{
				if (f199005 == null)
				{
					ShowMessage(Messages.WarningBeenDeleted);
					return false;
				}
				f199005.ACC_NUM = CurrentRecord.ACC_NUM;
				f199005.CAR_KIND_ID = CurrentRecord.CAR_KIND_ID;
				f199005.IN_TAX = CurrentRecord.IN_TAX;
				f199005.MAX_WEIGHT = CurrentRecord.MAX_WEIGHT;
				f199005.FEE = CurrentRecord.FEE;
				f199005.OVER_VALUE = CurrentRecord.OVER_VALUE;
				f199005.OVER_UNIT_FEE = CurrentRecord.OVER_UNIT_FEE;
				f199005.ACC_ITEM_NAME = CurrentRecord.ACC_ITEM_NAME;
				ExDataMapper.Trim(f199005);
				//proxy.UpdateObject(f199005);

				var proxyWcf = new wcf.P71WcfServiceClient();
				var wcf199005 = ExDataMapper.Map<F199005, wcf.F199005>(f199005);
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
								() => proxyWcf.UpdateF199005(wcf199005));

				if (result.IsSuccessed == false)
				{
					DialogService.ShowMessage(result.Message);
					return false;
				}


			}

			//proxy.SaveChanges();
			return true;
		}

		private void DoSaveComplete(bool isSaveSuccess)
		{
			if (!isSaveSuccess)
				return;

			ShowMessage(Messages.Success);

			#region 加入Resources.Resources.All
			AddAllToList(AccItemKindList);
			AddAllToList(LogiTypeList);			
			AddAllToList(TaxTypeList);
			AddAllToList(AccKindList);
			#endregion

			SearchCommand.Execute(null);
		}

		private bool CheckInputData()
		{
			var proxy = GetProxy<F19Entities>();

			if (UserOperateMode == OperateMode.Add)
			{

				var f199005 = proxy.F199005s.Where(x => x.DC_CODE == CurrentRecord.DC_CODE && x.STATUS == "0" &&
														x.ACC_ITEM_NAME == CurrentRecord.ACC_ITEM_NAME).SingleOrDefault();

				if (f199005 != null)
				{
					ShowWarningMessage(Properties.Resources.P7105050000_ViewModel_SameDC_ACCItemName_Duplicate);
					return false;
				}
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				var f199005 = proxy.F199005s.Where(x => x.DC_CODE == CurrentRecord.DC_CODE && x.STATUS == "0" &&
														x.ACC_ITEM_NAME == CurrentRecord.ACC_ITEM_NAME).SingleOrDefault();
				if (f199005 != null && !(f199005.DC_CODE == CurrentRecord.DC_CODE && f199005.ACC_ITEM_KIND_ID == CurrentRecord.ACC_ITEM_KIND_ID && f199005.LOGI_TYPE == CurrentRecord.LOGI_TYPE && f199005.ACC_KIND == CurrentRecord.ACC_KIND && f199005.DELV_EFFIC == CurrentRecord.DELV_EFFIC && f199005.IS_SPECIAL_CAR == CurrentRecord.IS_SPECIAL_CAR && f199005.ACC_AREA_ID == CurrentRecord.ACC_AREA_ID && f199005.ACC_UNIT == CurrentRecord.ACC_UNIT && f199005.DELV_ACC_TYPE == CurrentRecord.DELV_ACC_TYPE && f199005.DELV_TMPR == CurrentRecord.DELV_TMPR))
				{
					ShowWarningMessage(Properties.Resources.P7105050000_ViewModel_SameDC_ACCItemNameDuplicate);
					return false;
				}
			}

			if (CurrentRecord.ACC_KIND == "C" &&
				CurrentRecord.OVER_VALUE.HasValue && CurrentRecord.OVER_VALUE > 0 &&
				!CurrentRecord.OVER_UNIT_FEE.HasValue)
			{
				ShowWarningMessage(Properties.Resources.P7105050000_ViewModel_C_OVER_UNIT_FEE_Required);
				return false;
			}

			if (CurrentRecord.ACC_KIND == "D" &&
				CurrentRecord.OVER_VALUE.HasValue && CurrentRecord.OVER_VALUE > 0 &&
				!CurrentRecord.OVER_UNIT_FEE.HasValue)
			{
				ShowWarningMessage(Properties.Resources.P7105050000_ViewModel_D_OVER_UNIT_FEE_Required);
				return false;
			}

			if (CurrentRecord.ACC_KIND == "E" &&
				CurrentRecord.OVER_VALUE.HasValue && CurrentRecord.OVER_VALUE > 0 &&
				!CurrentRecord.OVER_UNIT_FEE.HasValue)
			{
				ShowWarningMessage(Properties.Resources.P7105050000_ViewModel_E_OVER_UNIT_FEE_Required);
				return false;
			}



			return true;
		}
		#endregion
	}
}
