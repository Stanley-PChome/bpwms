using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.UILib;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using F190102JoinF000904 = Wms3pl.WpfClient.ExDataServices.P19ExDataService.F190102JoinF000904;
using F1934EX = Wms3pl.WpfClient.ExDataServices.P19ExDataService.F1934EX;
using F194702Data = Wms3pl.WpfClient.ExDataServices.P19ExDataService.F194702Data;
using F1947Ex = Wms3pl.WpfClient.ExDataServices.P71ExDataService.F1947Ex;
using F1947JoinF194701 = Wms3pl.WpfClient.ExDataServices.P19ExDataService.F1947JoinF194701;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7109010000_ViewModel : InputViewModelBase
	{
		public Action F194701DgScrollIntoView = delegate { };
		public Action F194708DgScrollIntoView = delegate { };
		public Action F194709DgScrollIntoView = delegate { };
		public Action OpenSearchWin = delegate { };

		public P7109010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				_f1934List = GetF1934List();
				SetCarFee();
				SetPastTypeList();
				SetAllTypeList();
			}
		}

		#region Property

		private List<F1934> _f1934List = new List<F1934>();

		#region 配送商主檔 Property

		#region 物流中心
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

		private NameValuePair<string> _selectedDcItem;

		public NameValuePair<string> SelectedDcItem
		{
			get { return _selectedDcItem; }
			set
			{
				Set(() => SelectedDcItem, ref _selectedDcItem, value);
			}
		}

		#endregion

		#region 碼頭
		private List<string> _pierList;
		public List<string> PierList
		{
			get { return _pierList; }
			set
			{
				_pierList = value;
				RaisePropertyChanged("PierList");
			}
		}
		private string _selectedPierID = string.Empty;
		public string SelectedPierID
		{
			get { return _selectedPierID; }
			set
			{
				_selectedPierID = value;
				RaisePropertyChanged("SelectedPier");
			}
		}
		#endregion


		#region 配送類別
		private List<NameValuePair<string>> _pastTypeList;

		public List<NameValuePair<string>> PastTypeList
		{
			get { return _pastTypeList; }
			set
			{
				if (_pastTypeList == value)
					return;
				Set(() => PastTypeList, ref _pastTypeList, value);
			}
		}


		private string _selectedPastType;

		public string SelectedPastType
		{
			get { return _selectedPastType; }
			set
			{
				if (_selectedPastType == value)
					return;
				Set(() => SelectedPastType, ref _selectedPastType, value);
				BindTimeAndArea();

			}
		}


		#endregion


		private F190102JoinF000904 _F190102S_DELV_EFFIC_Selected = null;
		public F190102JoinF000904 F190102S_DELV_EFFIC_Selected
		{
			get { return _F190102S_DELV_EFFIC_Selected; }
			set
			{
				_F190102S_DELV_EFFIC_Selected = value;
				RaisePropertyChanged("F190102S_DELV_EFFIC_Selected");
			}
		}

		private ObservableCollection<F190102JoinF000904> _F190102S_DELV_EFFIC;
		public ObservableCollection<F190102JoinF000904> F190102S_DELV_EFFIC
		{
			get
			{
				if (_F190102S_DELV_EFFIC == null)
					_F190102S_DELV_EFFIC = new ObservableCollection<F190102JoinF000904>();
				return _F190102S_DELV_EFFIC;
			}
			set
			{
				_F190102S_DELV_EFFIC = value;
				RaisePropertyChanged("F190102S_DELV_EFFIC");
			}
		}

		private F000904 _F000904S_DELV_TMPR_Selected = null;
		public F000904 F000904S_DELV_TMPR_Selected
		{
			get { return _F000904S_DELV_TMPR_Selected; }
			set
			{
				_F000904S_DELV_TMPR_Selected = value;
				RaisePropertyChanged("F000904S_DELV_TMPR_Selected");
			}
		}

		/// <summary>
		/// 配送溫層下拉選單
		/// </summary>
		private ObservableCollection<F000904> _F000904S_DELV_TMPR;
		public ObservableCollection<F000904> F000904S_DELV_TMPR
		{
			get
			{
				if (_F000904S_DELV_TMPR == null)
					return new ObservableCollection<F000904>();
				return _F000904S_DELV_TMPR;
			}
			set
			{
				_F000904S_DELV_TMPR = value;
				RaisePropertyChanged("F000904S_DELV_TMPR");
			}
		}

		#region 配送時段的清單與選擇
		private SelectionList<Tuple<F1947JoinF194701, SelectionList<F1934>>> _tempF194701WithF19470101List;
		private SelectionList<Tuple<F1947JoinF194701, SelectionList<F1934>>> _f194701WithF19470101List;

		public SelectionList<Tuple<F1947JoinF194701, SelectionList<F1934>>> F194701WithF19470101List
		{
			get { return _f194701WithF19470101List; }
			set
			{
				Set(() => F194701WithF19470101List, ref _f194701WithF19470101List, value);
			}
		}

		private SelectionItem<Tuple<F1947JoinF194701, SelectionList<F1934>>> _selectedF194701WithF19470101;

		public SelectionItem<Tuple<F1947JoinF194701, SelectionList<F1934>>> SelectedF194701WithF19470101
		{
			get { return _selectedF194701WithF19470101; }
			set
			{
				Set(() => SelectedF194701WithF19470101, ref _selectedF194701WithF19470101, value);
			}
		}


		#endregion


		#region 配送時段的全選與打勾

		private bool _F194701AllSelectChecked = false;
		public bool F194701AllSelectChecked
		{
			get
			{
				return this._F194701AllSelectChecked;
			}
			set
			{
				this._F194701AllSelectChecked = value;
				RaisePropertyChanged("F194701AllSelectChecked");
			}
		}

		private RelayCommand _f194701AllSelectCheckedCommand;

		/// <summary>
		/// Gets the F194701AllSelectCheckedCommand.
		/// </summary>
		public RelayCommand F194701AllSelectCheckedCommand
		{
			get
			{
				return _f194701AllSelectCheckedCommand
					?? (_f194701AllSelectCheckedCommand = new RelayCommand(
					() =>
					{
						if (!F194701AllSelectCheckedCommand.CanExecute(null))
						{
							return;
						}

						foreach (var item in F194701WithF19470101List)
						{
							item.IsSelected = F194701AllSelectChecked;
						}
					},
					() => UserOperateMode != OperateMode.Query));
			}
		}

		private RelayCommand _f194701SelectCheckedCommand;

		/// <summary>
		/// Gets the F194701SelectCheckedCommand.
		/// </summary>
		public RelayCommand F194701SelectCheckedCommand
		{
			get
			{
				return _f194701SelectCheckedCommand
					?? (_f194701SelectCheckedCommand = new RelayCommand(
					() =>
					{
						if (!F194701SelectCheckedCommand.CanExecute(null))
						{
							return;
						}
					},
					() => UserOperateMode != OperateMode.Query));
			}
		}

		#endregion

		#region 班次配送區域的全選與打勾
		private bool _F1934EXAllSelectChecked = false;
		public bool F1934EXAllSelectChecked
		{
			get
			{
				return this._F1934EXAllSelectChecked;
			}
			set
			{
				this._F1934EXAllSelectChecked = value;
				RaisePropertyChanged("F1934EXAllSelectChecked");
			}
		}

		private RelayCommand _f1934EXAllSelectCheckedCommand;

		/// <summary>
		/// Gets the F1934EXAllSelectCheckedCommand.
		/// </summary>
		public RelayCommand F1934EXAllSelectCheckedCommand
		{
			get
			{
				return _f1934EXAllSelectCheckedCommand
					?? (_f1934EXAllSelectCheckedCommand = new RelayCommand(
					() =>
					{
						if (!F1934EXAllSelectCheckedCommand.CanExecute(null))
						{
							return;
						}

						foreach (var item in SelectedF194701WithF19470101.Item.Item2)
						{
							item.IsSelected = F1934EXAllSelectChecked;
						}
					},
					() => UserOperateMode != OperateMode.Query && SelectedF194701WithF19470101 != null));
			}
		}

		private RelayCommand _f1934EXSelectCheckedCommand;

		/// <summary>
		/// Gets the F1934EXSelectCheckedCommand.
		/// </summary>
		public RelayCommand F1934EXSelectCheckedCommand
		{
			get
			{
				return _f1934EXSelectCheckedCommand
					?? (_f1934EXSelectCheckedCommand = new RelayCommand(
					() =>
					{
						if (!F1934EXSelectCheckedCommand.CanExecute(null))
						{
							return;
						}
					},
					() => UserOperateMode != OperateMode.Query));
			}
		}

		#endregion

		private bool _F1934EX2AllSelectChecked = false;
		public bool F1934EX2AllSelectChecked
		{
			get
			{
				return this._F1934EX2AllSelectChecked;
			}
			set
			{
				this._F1934EX2AllSelectChecked = value;
				RaisePropertyChanged("F1934EX2AllSelectChecked");
			}
		}


		private SelectionItem<Tuple<string, SelectionList<NameValuePair<string>>>> _selectedDelvTimeItem;

		public SelectionItem<Tuple<string, SelectionList<NameValuePair<string>>>> SelectedDelvTimeItem
		{
			get { return _selectedDelvTimeItem; }
			set
			{
				_selectedDelvTimeItem = value;
				RaisePropertyChanged("SelectedDelvTimeItem");
			}
		}

		#region 出車時段

		public DateTime SelectedDelvTime { get; set; }

		#endregion

		#region 配送商屬性
		private F1947 _tempMaster;
		private F1947 _currentMaster;

		public F1947 CurrentMaster
		{
			get { return _currentMaster; }
			set
			{
				_currentMaster = value;
				RaisePropertyChanged("CurrentMaster");
			}
		}



		public void DcCodeChanged()
		{
			if (CurrentMaster != null)
			{
				//SetGupList(CurrentMaster.DC_CODE);
				SetPierList(CurrentMaster.DC_CODE);				
			}
			this.SetF190102S_DELV_EFFIC(true);
		}

		#endregion

		#region 區域
		private List<NameValuePair<string>> _couldivList;

		public List<NameValuePair<string>> CouldivList
		{
			get { return _couldivList; }
			set
			{
				_couldivList = value;
				RaisePropertyChanged("CouldivList");
			}
		}


		#endregion

		#region 配次
		private string _delvTimes;

		public string DelvTimes
		{
			get { return _delvTimes; }
			set
			{
				Set(() => DelvTimes, ref _delvTimes, value);
			}
		}
		#endregion

		#region 出車頻率

		private bool _sun;

		public bool Sun
		{
			get { return _sun; }
			set
			{
				if (_sun == value)
					return;
				Set(() => Sun, ref _sun, value);
			}
		}

		private bool _mon = true;

		public bool Mon
		{
			get { return _mon; }
			set
			{
				if (_mon == value)
					return;
				Set(() => Mon, ref _mon, value);
			}
		}

		private bool _tue = true;

		public bool Tue
		{
			get { return _tue; }
			set
			{
				if (_tue == value)
					return;
				Set(() => Tue, ref _tue, value);
			}
		}

		private bool _wed = true;

		public bool Wed
		{
			get { return _wed; }
			set
			{
				if (_wed == value)
					return;
				Set(() => Wed, ref _wed, value);
			}
		}

		private bool _thu = true;

		public bool Thu
		{
			get { return _thu; }
			set
			{
				if (_thu == value)
					return;
				Set(() => Thu, ref _thu, value);
			}
		}

		private bool _fri = true;

		public bool Fri
		{
			get { return _fri; }
			set
			{
				if (_fri == value)
					return;
				Set(() => Fri, ref _fri, value);
			}
		}

		private bool _sat = true;

		public bool Sat
		{
			get { return _sat; }
			set
			{
				if (_sat == value)
					return;
				Set(() => Sat, ref _sat, value);
			}
		}

		#endregion

		#endregion

		#region 計費區域 Property


		#region 計費區域名稱
		private string _accArea;

		public string AccArea
		{
			get { return _accArea; }
			set
			{
				if (_accArea == value)
					return;
				Set(() => AccArea, ref _accArea, value);
			}
		}
		#endregion


		#region Grid 計費區域
		private List<F194708> _f194708List;

		public List<F194708> F194708List
		{
			get { return _f194708List; }
			set
			{
				if (_f194708List == value)
					return;
				Set(() => F194708List, ref _f194708List, value);
			}
		}

		private F194708 _selectedF194708;

		public F194708 SelectedF194708
		{
			get { return _selectedF194708; }
			set
			{
				if (_selectedF194708 == value)
					return;
				if (value != null)
					SaveF194708ByF1934Ex2List();
				Set(() => SelectedF194708, ref _selectedF194708, value);
				if (value != null)
				{
					AccArea = value.ACC_AREA;
					SetF1934Ex2ListByF194708();
				}
			}
		}

		#endregion

		#region Grid 計費區域郵遞區號

		private SelectionList<F1934EX> _f1934ExList2;

		public SelectionList<F1934EX> F1934ExList2
		{
			get { return _f1934ExList2; }
			set
			{
				if (_f1934ExList2 == value)
					return;
				Set(() => F1934ExList2, ref _f1934ExList2, value);
			}
		}

		#region
		private SelectionItem<F1934EX> _selectedF1934Ex2;

		public SelectionItem<F1934EX> SelectedF1934Ex2
		{
			get { return _selectedF1934Ex2; }
			set
			{
				if (_selectedF1934Ex2 == value)
					return;
				Set(() => SelectedF1934Ex2, ref _selectedF1934Ex2, value);
			}
		}
		#endregion

		#endregion

		#region 所有計費區域郵遞區號資料
		private List<F19470801> _f19470801List;

		public List<F19470801> F19470801List
		{
			get { return _f19470801List; }
			set
			{
				if (_f19470801List == value)
					return;
				Set(() => F19470801List, ref _f19470801List, value);
			}
		}
		#endregion

		#endregion

		#region 單量維護 Property


		#region Grid 單量維護
		private List<F194709> _f194709List;

		public List<F194709> F194709List
		{
			get { return _f194709List; }
			set
			{
				if (_f194709List == value)
					return;
				Set(() => F194709List, ref _f194709List, value);
			}
		}

		private F194709 _selectedF194709;

		public F194709 SelectedF194709
		{
			get { return _selectedF194709; }
			set
			{
				if (_selectedF194709 == value)
					return;
				Set(() => SelectedF194709, ref _selectedF194709, value);
				if (value != null)
					TxtNum = value.NUM;
			}
		}
		#endregion


		#region 單量
		private int _txtNum;

		public int TxtNum
		{
			get { return _txtNum; }
			set
			{
				if (_txtNum == value)
					return;
				Set(() => TxtNum, ref _txtNum, value);
			}
		}
		#endregion


		#endregion

		#region 配送商路線表

		public List<Route> Routes { get; set; }

		private void GetRoutes(string allId,string dcCode)
		{
			var proxy = GetExProxy<ShareExDataSource>();
			Routes = proxy.CreateQuery<Route>("GetRoutes")
				.AddQueryExOption("allId", string.Empty)
				.AddQueryExOption("dcCode", dcCode)
				.ToList();
		}
		#endregion


		#region 配送商類別
		private List<NameValuePair<string>> _allTypeList;

		public List<NameValuePair<string>> AllTypeList
		{
			get { return _allTypeList; }
			set
			{
				Set(() => AllTypeList, ref _allTypeList, value);
			}
		}
		#endregion

		#endregion

		#region 下拉式選單資料來源

		#region 物流中心
		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
		}
		#endregion

		#region 碼頭清單
		public void SetPierList(string dcCode)
		{
			if (dcCode == null)
				return;

			// 碼頭清單(F1981)要過濾畫面上有 key 的條件 2015/02/05
			//var proxy = new wcf.P71WcfServiceClient();
			//var pierCodeList = RunWcfMethod<string[]>(proxy.InnerChannel,
			//                                          () => proxy.GetPierCodeList(dcCode));
			var proxy = GetProxy<F19Entities>();
			var query = from item in proxy.F1981s
									where item.DC_CODE == dcCode
									orderby item.PIER_CODE
									select item;

			var pierList = query.ToList();
			PierList = pierList.Select(item => item.PIER_CODE).ToList();
		}
		#endregion

		#region 取得鄉鎮行政區域

		public SelectionList<F1934EX> GetF1934EXList()
		{
			var P19Proxy = GetExProxy<P19ExDataSource>();
			return P19Proxy.CreateQuery<F1934EX>("GetF1934EXDatas").ToSelectionList();
		}

		/// <summary>
		/// 取得鄉鎮行政區域檔清單
		/// </summary>
		/// <returns></returns>
		public List<F1934> GetF1934List()
		{
			var proxy = GetProxy<F19Entities>();
			return proxy.F1934s.OrderBy(x => x.ZIP_CODE).ToList();
		}

		/// <summary>
		/// 取得班次配送區域設定檔清單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="allId"></param>
		/// <returns></returns>
		public List<F19470101> GetF19470101List(string dcCode, string allId)
		{
			var proxy = GetProxy<F19Entities>();
			return proxy.F19470101s.Where(x => x.ALL_ID == allId && x.DC_CODE == dcCode).ToList();
		}

		#endregion


		#region 配送溫層

		void SetF000904S_DELV_TMPR()
		{
			if (this.F000904S_DELV_TMPR == null || this.F000904S_DELV_TMPR.Count.Equals(0))
			{
				var F00Proxy = GetProxy<F00Entities>();
				this.F000904S_DELV_TMPR = GetBaseTableService.GetF000904List(FunctionCode, "F194701", "DELV_TMPR").Select(o=>new F000904 { VALUE = o.Value,NAME = o.Name}).ToObservableCollection();
			}
			if (this.F000904S_DELV_TMPR.Any())
				this.F000904S_DELV_TMPR_Selected = this.F000904S_DELV_TMPR.First();

		}
		#endregion

		#region 配送效率
		private void SetF190102S_DELV_EFFIC(bool blnForce = false)
		{
			if (blnForce || this.F190102S_DELV_EFFIC == null || this.F190102S_DELV_EFFIC.Count.Equals(0))
			{
				if (SelectedDcItem != null)
				{
					var P19Proxy = GetExProxy<P19ExDataSource>();
					this.F190102S_DELV_EFFIC = this.GetF190102JoinF000904Datas();
				}
			}
			if (F190102S_DELV_EFFIC != null && this.F190102S_DELV_EFFIC.Any())
				this.F190102S_DELV_EFFIC_Selected = this.F190102S_DELV_EFFIC.First();
		}
		#endregion

		#region 配送類別

		private void SetPastTypeList()
		{
			var proxy = GetProxy<F00Entities>();
			var data = proxy.F000904s.Where(o => o.TOPIC == "F194701" && o.SUBTOPIC == "PAST_TYPE").ToList();
			PastTypeList = (from o in data
											select new NameValuePair<string> { Name = o.NAME, Value = o.VALUE }).ToList();
			if (PastTypeList.Any())
				SelectedPastType = PastTypeList.First().Value;
		}

		#endregion
		#region 配送商類別

		private void SetAllTypeList()
		{
			var proxy = GetProxy<F00Entities>();
			var data = proxy.F000904s.Where(o => o.TOPIC == "F1947" && o.SUBTOPIC == "TYPE").ToList();
			AllTypeList = (from o in data
											select new NameValuePair<string> { Name = o.NAME, Value = o.VALUE }).ToList();
		}

		#endregion



		#endregion

		#region Method
		#region 取得出車時段資料
		public List<F1947JoinF194701> GetF194701EXList(string ALL_ID, string DC_CODE)
		{
			var proxy = GetExProxy<P19ExDataSource>();
			return proxy.CreateQuery<F1947JoinF194701>("GetF1947Join194701Datas")
					 .AddQueryExOption("ALL_ID", ALL_ID)
					 .AddQueryExOption("DC_CODE", DC_CODE)
					.ToList();
		}
		#endregion


		#region 取得配送效率
		public ObservableCollection<F190102JoinF000904> GetF190102JoinF000904Datas()
		{
			var proxyEx = GetExProxy<P19ExDataSource>();
			return proxyEx.CreateQuery<F190102JoinF000904>("GetF190102JoinF000904Datas")
					 .AddQueryExOption("DC_CODE", SelectedDcItem.Value)
					 .AddQueryExOption("TOPIC", "F194701")
					 .AddQueryExOption("Subtopic", "DELV_EFFIC")
					 .ToObservableCollection();

		}
		#endregion


		public void SetSearchResult(F1947Ex f1947Ex)
		{
			if (f1947Ex == null)
				return;
			var f1947 = ExDataMapper.Map<F1947Ex, F1947>(f1947Ex);
			_tempMaster = ExDataMapper.Clone(f1947);
			SetSearchData(f1947);
		}

		/// <summary>
		/// 取得該時段的班次配送區域設定檔
		/// </summary>
		/// <param name="f1947JoinF194701"></param>
		/// <param name="f19470101List"></param>		
		/// <returns></returns>
		SelectionList<F1934> GetF1934SelectionList(F1947JoinF194701 f1947JoinF194701, List<F19470101> f19470101List)
		{
			// 只取得該時段的班次配送區域設定檔
			var f19470101Query = from f19470101 in f19470101List
													 where f19470101.DELV_TIME == f1947JoinF194701.DELV_TIME	// 出車時段
													 && f19470101.DELV_TMPR == f1947JoinF194701.DELV_TMPR	// 配送溫層(A:常溫、B:低溫)F000904
													 && f19470101.DELV_EFFIC == f1947JoinF194701.DELV_EFFIC	// 配送效率
													 && f19470101.PAST_TYPE == f1947JoinF194701.PAST_TYPE	// 配送類別
													 select f19470101;

			// 將班次配送區域與所有鄉鎮行政區域結合顯示出來，其中已有班次配送區域的預設會打勾
			var f1934WithF19470101Query = from f1934 in _f1934List
																		join f19470101 in f19470101Query on f1934.ZIP_CODE equals f19470101.ZIP_CODE into f1934WithF19470101
																		from c in f1934WithF19470101.DefaultIfEmpty()
																		select new SelectionItem<F1934>(f1934, c != null);

			return new SelectionList<F1934>(f1934WithF19470101Query);
		}

		private void SetSearchData(F1947 f1947)
		{
			ResetIni();
			CurrentMaster = f1947;
			var f194701List = this.GetF194701EXList(f1947.ALL_ID, f1947.DC_CODE);
			var f19470101List = GetF19470101List(f1947.DC_CODE, f1947.ALL_ID);
			_tempF194701WithF19470101List = f194701List.Select(f194701 => new Tuple<F1947JoinF194701, SelectionList<F1934>>(f194701, GetF1934SelectionList(f194701, f19470101List))).ToSelectionList();
			BindTimeAndArea();

			this.SetF000904S_DELV_TMPR();


			this.SetF190102S_DELV_EFFIC();

			this.SetF194708Data();
			this.SetF19470801Data();

			this.SetF194709Data();

			SetF194703();

		}



		private void ResetIni()
		{
			CurrentMaster = null;
			F194701WithF19470101List = null;
			_tempF194701WithF19470101List = null;
			F194703List = null;
			F190102S_DELV_EFFIC = null;
			F190102S_DELV_EFFIC_Selected = null;
			F000904S_DELV_TMPR = null;
			F000904S_DELV_TMPR_Selected = null;

			F194701AllSelectChecked = false;
			F1934EXAllSelectChecked = false;
			F1934EX2AllSelectChecked = false;

			F194708List = null;
			SelectedF194708 = null;
			F19470801List = null;
			F1934ExList2 = null;
			SelectedF1934Ex2 = null;
			AccArea = null;

			F194709List = null;
			SelectedF194709 = null;
			TxtNum = 0;
		}

		#region 計費區域 Method
		private void SetF194708Data()
		{
			var proxy = GetProxy<F19Entities>();
			F194708List =
				proxy.F194708s.Where(o => o.DC_CODE == CurrentMaster.DC_CODE && o.ALL_ID == CurrentMaster.ALL_ID).ToList();
		}

		private void SetF19470801Data()
		{
			var proxy = GetProxy<F19Entities>();
			F19470801List =
				proxy.F19470801s.Where(o => o.DC_CODE == CurrentMaster.DC_CODE && o.ALL_ID == CurrentMaster.ALL_ID).ToList();
		}

		private void SetF1934Ex2ListByF194708()
		{
			if (SelectedF194708 != null)
			{
				var data = GetF1934EXList();
				var items = F19470801List ?? new List<F19470801>();
				items = items.Where(o => o.ACC_AREA_ID == SelectedF194708.ACC_AREA_ID).ToList();
				foreach (var selectionItem in data)
					selectionItem.IsSelected = (items.Any(o => o.ZIP_CODE == selectionItem.Item.ZIP_CODE));
				F1934ExList2 = data;
			}
		}

		private void SaveF194708ByF1934Ex2List()
		{
			if (SelectedF194708 != null && F1934ExList2 != null)
			{
				var datas = F1934ExList2;
				var items = F19470801List ?? new List<F19470801>();
				var selectedZipCodeList = datas.Where(o => o.IsSelected).Select(o => o.Item.ZIP_CODE).ToList();
				var removeItems = items.Where(o => o.ACC_AREA_ID == SelectedF194708.ACC_AREA_ID && !selectedZipCodeList.Contains(o.ZIP_CODE)).ToList();
				foreach (var f19470801 in removeItems)
					items.Remove(f19470801);
				var addZipCodeList = selectedZipCodeList.Where(o => !items.Any(c => c.ACC_AREA_ID == SelectedF194708.ACC_AREA_ID && c.ZIP_CODE == o)).ToList();

				items.AddRange(from zipCode in addZipCodeList
											 select new F19470801
											 {
												 DC_CODE = CurrentMaster.DC_CODE,
												 ALL_ID = CurrentMaster.ALL_ID,
												 ACC_AREA_ID = SelectedF194708.ACC_AREA_ID,
												 ZIP_CODE = zipCode
											 });
				F19470801List = items;
			}
		}

		private void SetF194709Data()
		{
			var proxy = GetProxy<F19Entities>();
			F194709List =
				proxy.F194709s.Where(o => o.DC_CODE == CurrentMaster.DC_CODE && o.ALL_ID == CurrentMaster.ALL_ID).ToList();
		}

		#endregion


		#region 全選/取消全選 Method


		public void F1934EX2_AllSelect(bool isSelected)
		{
			if (F1934ExList2 != null && F1934ExList2.Any())
				foreach (var f1934EX2 in F1934ExList2)
					f1934EX2.IsSelected = isSelected;
		}
		#endregion


		private void BindTimeAndArea()
		{
			if (SelectedPastType != null && _tempF194701WithF19470101List != null)
			{
				F194701WithF19470101List =
					_tempF194701WithF19470101List.Where(o => o.Item.Item1.PAST_TYPE == SelectedPastType)
						.Select(item => new Tuple<F1947JoinF194701, SelectionList<F1934>>(item.Item.Item1, item.Item.Item2))
						.ToSelectionList();
			}
			else
				F194701WithF19470101List = null;
		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return new RelayCommand(
						() => OpenSearchWin(),
						() => { return UserOperateMode == OperateMode.Query; });
			}
		}

		private void DoSearch()
		{
			//執行查詢動
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
			CurrentMaster = new F1947();
			_tempF194701WithF19470101List = new SelectionList<Tuple<F1947JoinF194701, SelectionList<F1934>>>(new List<Tuple<F1947JoinF194701, SelectionList<F1934>>>());
			F194701WithF19470101List = new SelectionList<Tuple<F1947JoinF194701, SelectionList<F1934>>>(new List<Tuple<F1947JoinF194701, SelectionList<F1934>>>());
			F194703List = new SelectionList<F194703>(new List<F194703>());
			F194708List = new List<F194708>();
			F19470801List = new List<F19470801>();
			this.F1934ExList2 = null;
			this.F194709List = null;

			TxtNum = 0;

			AccArea = string.Empty;

			//if (this.F000904S_DELV_TMPR == null || this.F000904S_DELV_TMPR.Count.Equals(0))
			this.SetF000904S_DELV_TMPR();

			//if (this.F190102S_DELV_EFFIC == null || this.F190102S_DELV_EFFIC.Count.Equals(0))
			this.SetF190102S_DELV_EFFIC(true);

			UserOperateMode = OperateMode.Add;
			//執行新增動作

			Sun = false;
			Mon = true;
			Tue = true;
			Wed = true;
			Thu = true;
			Fri = true;
			Sat = true;

			CarData = new F194703 { FEE = 1 };
		}


		private void RestoreData()
		{
			ResetIni();
			if (_tempMaster != null)
			{
				SetSearchData(_tempMaster);
				_tempMaster = null;
			}
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoEdit(),
						() => UserOperateMode == OperateMode.Query && CurrentMaster != null
						);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;

			Sun = false;
			Mon = true;
			Tue = true;
			Wed = true;
			Thu = true;
			Fri = true;
			Sat = true;

			CarData = new F194703 { FEE = 1 };
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
			if (ShowMessage(Messages.WarningBeforeCancel) != UILib.Services.DialogResponse.Yes)
				return;

			// 當資料沒有改變，或者選擇不用儲存的時候，直接復原資料，並返回查詢模式
			RestoreData();
			UserOperateMode = OperateMode.Query;
			Sun = false;
			Mon = true;
			Tue = true;
			Wed = true;
			Thu = true;
			Fri = true;
			Sat = true;

			CarData = null;

		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDelete(),
						() => UserOperateMode == OperateMode.Query && CurrentMaster != null
						);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (CurrentMaster == null)
			{
				DialogService.ShowMessage(Properties.Resources.P7109010000_ViewModel_CurrentMaster);
				return;
			}

			if (ShowMessage(Messages.WarningBeforeDelete) != UILib.Services.DialogResponse.Yes)
			{
				return;
			}

			var proxy = GetExProxy<P71ExDataSource>();

			var query = proxy.CreateQuery<Wms3pl.WpfClient.ExDataServices.P71ExDataService.ExecuteResult>("DeleteF1947")
					.AddQueryExOption("dcCode", CurrentMaster.DC_CODE)
					.AddQueryExOption("allID", CurrentMaster.ALL_ID);

			var result = query.FirstOrDefault();
			if (result == null)
			{
				DialogService.ShowMessage(Properties.Resources.P7109010000_ViewModel_DeleteError);
			}
			else
			{
				if (result.IsSuccessed)
				{
					ResetIni();
				}

				DialogService.ShowMessage(result.Message);
			}

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
			//儲存最後一次修改的暫存計費時段郵遞區號設定

			//儲存最後一次修改的暫存計費郵遞區號設定
			SaveF194708ByF1934Ex2List();

			// 1. 驗證資料
			string errorMessage;
			if (!TryValidateDoSave(out errorMessage))
			{
				DialogService.ShowMessage(errorMessage);
				return;
			}

			// 2. 儲存資料
			var result = GetDoSaveResult();

			// 3. 回到查詢模式，並顯示訊息
			if (result.IsSuccessed)
			{
				UserOperateMode = OperateMode.Query;

				SetSearchData(CurrentMaster);

			}

			DialogService.ShowMessage(result.Message);
		}

		string ConvertToDelvFreq(F1947JoinF194701 f1947JoinF194701)
		{
			var list = new List<string>();
			if (f1947JoinF194701.Sun == "1")
				list.Add("0");
			if (f1947JoinF194701.Mon == "1")
				list.Add("1");
			if (f1947JoinF194701.Tue == "1")
				list.Add("2");
			if (f1947JoinF194701.Wed == "1")
				list.Add("3");
			if (f1947JoinF194701.Thu == "1")
				list.Add("4");
			if (f1947JoinF194701.Fri == "1")
				list.Add("5");
			if (f1947JoinF194701.Sat == "1")
				list.Add("6");
			return string.Join(",", list);
		}

		private wcf.ExecuteResult GetDoSaveResult()
		{
			var proxy = new wcf.P71WcfServiceClient();

			// 轉換為 Wcf Entity
			var wcfMaster = ExDataMapper.Map<F1947, wcf.F1947>(CurrentMaster);
			wcfMaster.ACC_KINDk__BackingField = "A";

			// 取得配送商出車時段，因出車頻率(0~6:星期日~六,逗號分隔)關係，中間需轉換成逗號分隔
			var f194701Query = from f1947JoinF194701 in _tempF194701WithF19470101List.Select(x => x.Item.Item1)
												 let delvFreq = ConvertToDelvFreq(f1947JoinF194701)
												 select new F194701
												 {
													 PAST_TYPE = f1947JoinF194701.PAST_TYPE,
													 DELV_TIME = f1947JoinF194701.DELV_TIME,
													 DELV_EFFIC = f1947JoinF194701.DELV_EFFIC,
													 DELV_TMPR = f1947JoinF194701.DELV_TMPR,
													 DELV_TIMES = f1947JoinF194701.DELV_TIMES,
													 DELV_FREQ = delvFreq
												 };
			var wcfF194701s = ExDataMapper.MapCollection<F194701, wcf.F194701>(f194701Query).ToArray();

			// 取得只有打勾的班次配送區域
			var f19470101Query = from si in _tempF194701WithF19470101List.Select(x => x.Item)
													 let f1947JoinF194701 = si.Item1
													 from f1934 in si.Item2.Where(x => x.IsSelected).Select(x => x.Item)
													 select new F19470101
													 {
														 PAST_TYPE = f1947JoinF194701.PAST_TYPE,
														 DELV_TIME = f1947JoinF194701.DELV_TIME,
														 DELV_EFFIC = f1947JoinF194701.DELV_EFFIC,
														 DELV_TMPR = f1947JoinF194701.DELV_TMPR,
														 ZIP_CODE = f1934.ZIP_CODE
													 };
			var wcfF19470101s = ExDataMapper.MapCollection<F19470101, wcf.F19470101>(f19470101Query).ToArray();


			//F194708List  所有要存的計費區域
			//F19470801List 所有要存的計費區域郵遞區號設定
			var wcfF194708List = ExDataMapper.MapCollection<F194708, wcf.F194708>(F194708List);
			var wcfF19470801List = ExDataMapper.MapCollection<F19470801, wcf.F19470801>(F19470801List);

			var wcfF194709List = ExDataMapper.MapCollection<F194709, wcf.F194709>(F194709List);

			var wcfF194703List = ExDataMapper.MapCollection<F194703, wcf.F194703>(F194703List.Select(p => p.Item));

			switch (UserOperateMode)
			{
				case OperateMode.Add:
					// 新增
					return RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
															() => proxy.InsertF1947New(wcfMaster, wcfF194701s, wcfF19470101s, wcfF194708List.ToArray(), wcfF19470801List.ToArray(), wcfF194709List.ToArray(), wcfF194703List.ToArray()));
				case OperateMode.Edit:
					return RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
															() => proxy.UpdateF1947New(wcfMaster, wcfF194701s, wcfF19470101s, wcfF194708List.ToArray(), wcfF19470801List.ToArray(), wcfF194709List.ToArray(), wcfF194703List.ToArray()));
				default:
					return new wcf.ExecuteResult() { Message = Properties.Resources.P7109010000_ViewModel_StepError };
			}
		}
		#endregion Save

		#region Add Time
		private RelayCommand _addTimeCommand;

		/// <summary>
		/// Gets the AddTimeCommand.
		/// </summary>
		public RelayCommand AddTimeCommand
		{
			get
			{
				return _addTimeCommand
					?? (_addTimeCommand = new RelayCommand(
					() =>
					{
						if (!AddTimeCommand.CanExecute(null))
						{
							return;
						}

						if (string.IsNullOrWhiteSpace(DelvTimes))
						{
							ShowWarningMessage(Properties.Resources.P7109010000_ViewModel_InputDelv);
							return;
						}

						// 判斷是否已經有相同的出車時段、配送效率、配送溫層，提示訊息比 Disabled 好?
						if (F194701WithF19470101List.Select(x => x.Item.Item1)
													.Any(x => x.PAST_TYPE == SelectedPastType && x.DELV_TIME == GetDelvTimeFormatContent()
															&& x.DELV_EFFIC == F190102S_DELV_EFFIC_Selected.DELV_EFFIC
															&& x.DELV_TMPR == F000904S_DELV_TMPR_Selected.VALUE))
						{
							ShowWarningMessage(Properties.Resources.P7109010000_ViewModel_PAST_TYPE_DELV_EFFIC_DELV_TMPR_Exisst);
							return;
						}

						string selectedDelv_Time = GetDelvTimeFormatContent();
						var f1947JoinF194701 = new F1947JoinF194701
						{
							PAST_TYPE = SelectedPastType,
							DELV_EFFIC = this.F190102S_DELV_EFFIC_Selected.DELV_EFFIC,
							DELV_EFFIC_NAME = this.F190102S_DELV_EFFIC_Selected.DELV_EFFIC_NAME,
							DELV_TMPR_NAME = this.F000904S_DELV_TMPR_Selected.NAME,
							DELV_TMPR = this.F000904S_DELV_TMPR_Selected.VALUE,
							DELV_TIME = selectedDelv_Time,
							DELV_TIMES = DelvTimes,
							Sun = Sun ? "1" : "0",
							Mon = Mon ? "1" : "0",
							Tue = Tue ? "1" : "0",
							Wed = Wed ? "1" : "0",
							Thu = Thu ? "1" : "0",
							Fri = Fri ? "1" : "0",
							Sat = Sat ? "1" : "0"
						};

						_tempF194701WithF19470101List.Add(new SelectionItem<Tuple<F1947JoinF194701, SelectionList<F1934>>>(new Tuple<F1947JoinF194701, SelectionList<F1934>>(f1947JoinF194701, new SelectionList<F1934>(_f1934List))));
						BindTimeAndArea();
						SelectedF194701WithF19470101 = F194701WithF19470101List.Last();
						F194701DgScrollIntoView();

						Sun = false;
						Mon = true;
						Tue = true;
						Wed = true;
						Thu = true;
						Fri = true;
						Sat = true;
						DelvTimes = string.Empty;

					},
					() => UserOperateMode != OperateMode.Query
						// 檢查配送效率、配送溫層已選擇
						&& F190102S_DELV_EFFIC_Selected != null
						&& F000904S_DELV_TMPR_Selected != null
						));
			}
		}

		public string GetDelvTimeFormatContent()
		{
			return SelectedDelvTime.ToString("HH:mm");
		}


		private bool TryValidateDoSave(out string errorMessage)
		{
			errorMessage = null;

			if (string.IsNullOrWhiteSpace(CurrentMaster.PIER_CODE))
			{
				errorMessage = Properties.Resources.P7109010000_ViewModel_PIER_CODE;
			}

			if (string.IsNullOrEmpty(errorMessage) &&
			    (string.IsNullOrWhiteSpace(CurrentMaster.ALL_COMP) || CurrentMaster.ALL_COMP.Length > 30))
			{
				errorMessage = Properties.Resources.P7109010000_ViewModel_ALL_COMP_Name_Limit;
			}

			if (string.IsNullOrEmpty(errorMessage) &&
			    (string.IsNullOrWhiteSpace(CurrentMaster.ALL_ID) || CurrentMaster.ALL_ID.Length > 10))
			{
				errorMessage = Properties.Resources.P7109010000_ViewModel_ALL_ID_Limit;
			}

			if (string.IsNullOrEmpty(errorMessage) && string.IsNullOrWhiteSpace(CurrentMaster.DC_CODE))
			{
				errorMessage = Properties.Resources.P7109010000_ViewModel_SelectDC;
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				foreach (var f194701WithF19470101 in F194701WithF19470101List)
				{
					if (!f194701WithF19470101.Item.Item2.Any(x => x.IsSelected))
					{
						var selectionItem = f194701WithF19470101.Item.Item1;
						errorMessage = string.Format(Properties.Resources.P7109010000_ViewModel_BatchTime_SelectArea, selectionItem.DELV_TIME,
							selectionItem.DELV_EFFIC,
							selectionItem.DELV_TMPR);
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(errorMessage))
			{
				if (F194708List != null)
				{
					F19470801List = F19470801List ?? new List<F19470801>();
					foreach (var selectionItem in F194708List)
					{
						if (F19470801List != null &&
						    !F19470801List.Any(
							    o => o.ACC_AREA_ID == selectionItem.ACC_AREA_ID))
						{
							errorMessage = string.Format(Properties.Resources.P7109010000_ViewModel_ACCAREA, selectionItem.ACC_AREA);
							break;
						}
					}
				}
			}

			if (string.IsNullOrEmpty(errorMessage))
			{				
				//檢查配次路線表是否有此配次設定		
				GetRoutes(CurrentMaster.ALL_ID, CurrentMaster.DC_CODE);
				var list = F194701WithF19470101List.Select(a => new
				{
					a.Item.Item1.DELV_TIME,
					a.Item.Item1.DELV_TIMES,
					F19470101s = a.Item.Item2.Where(b => b.IsSelected).Select(b => b.Item).ToList()
				}).ToList();

				var data = list.Select(
					item => new
					{
						item.DELV_TIME,
						item.DELV_TIMES,
						F19470101Datas =
							item.F19470101s.Where(
								b =>
									!Routes.Any(r => r.AllId == CurrentMaster.ALL_ID && r.DelvTimes == item.DELV_TIMES && r.ZipCode == b.ZIP_CODE))
					}).FirstOrDefault(d => d.F19470101Datas != null && d.F19470101Datas.Any());

				if (data != null && data.F19470101Datas != null)
				{
					errorMessage = string.Format(Properties.Resources.P7109010000_ViewModel_DELV_TIMES_ZIP_CODE_NotSet
						, data.DELV_TIME, data.DELV_TIMES, data.F19470101Datas.First().ZIP_CODE);
				}

			}

			if (string.IsNullOrEmpty(errorMessage) && (F194709List == null || !F194709List.Any()))
				errorMessage = string.Format(Properties.Resources.P7109010000_ViewModel_ListOne);

			return (errorMessage == null);
		}
		#endregion

		#region Remove Time
		private RelayCommand _removeTimeCommand;

		/// <summary>
		/// Gets the RemoveTimeCommand.
		/// </summary>
		public RelayCommand RemoveTimeCommand
		{
			get
			{
				return _removeTimeCommand
					?? (_removeTimeCommand = new RelayCommand(
					() =>
					{
						if (!RemoveTimeCommand.CanExecute(null))
						{
							return;
						}
						var removeData = F194701WithF19470101List.Where(o => o.IsSelected).ToList();
						foreach (var selectionItem in removeData)
						{
							var removeItem = _tempF194701WithF19470101List.First(o => o.Item.Item1 == selectionItem.Item.Item1);
							_tempF194701WithF19470101List.Remove(removeItem);
						}
						BindTimeAndArea();
					},
					() => UserOperateMode != OperateMode.Query
						&& F194701WithF19470101List != null
						&& F194701WithF19470101List.Any(si => si.IsSelected)));
			}
		}
		#endregion

		#region AddArea
		public ICommand AddAreaCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					 o => { },
						() => UserOperateMode != OperateMode.Query && !string.IsNullOrWhiteSpace(AccArea) && (F194708List == null || F194708List.All(o => o.ACC_AREA != AccArea.Trim())),
						o => DoAddAreaCompleted());
			}
		}

		private void DoAddAreaCompleted()
		{
			var list = F194708List ?? new List<F194708>();
			var id = list.Any(o => o.ACC_AREA_ID < 0) ? list.Select(o => o.ACC_AREA_ID).Min() - 1 : -1;
			var f194708 = new F194708
			{
				ACC_AREA_ID = id,
				ALL_ID = CurrentMaster.ALL_ID,
				DC_CODE = CurrentMaster.DC_CODE,
				ACC_AREA = AccArea.Trim()
			};
			list.Add(f194708);
			F194708List = list.ToList();
			SelectedF194708 = F194708List.Last();
			F194708DgScrollIntoView();
		}
		#endregion

		#region EditArea
		public ICommand EditAreaCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => { },
						() => UserOperateMode != OperateMode.Query && SelectedF194708 != null && !string.IsNullOrWhiteSpace(AccArea) && F194708List != null && F194708List.All(o => o.ACC_AREA != AccArea.Trim()),
						o => DoEditArea());
			}
		}

		private void DoEditArea()
		{
			var list = F194708List ?? new List<F194708>();
			var item = list.Find(o => o.ACC_AREA == SelectedF194708.ACC_AREA);
			item.ACC_AREA = AccArea;
			F194708List = list.ToList();
		}

		#endregion

		#region AddNum

		public ICommand AddNumCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					 o => { },
						() => UserOperateMode != OperateMode.Query && (F194709List == null || F194709List.All(o => o.NUM != TxtNum)),
						o => DoAddNumCompleted());
			}
		}

		private void DoAddNumCompleted()
		{
			var list = F194709List ?? new List<F194709>();
			var id = list.Any(o => o.ACC_DELVNUM_ID < 0) ? list.Select(o => o.ACC_DELVNUM_ID).Min() - 1 : -1;
			var f194709 = new F194709
			{
				ACC_DELVNUM_ID = id,
				ALL_ID = CurrentMaster.ALL_ID,
				DC_CODE = CurrentMaster.DC_CODE,
				NUM = TxtNum
			};
			list.Add(f194709);
			F194709List = list.ToList();
			SelectedF194709 = F194709List.Last();
			F194709DgScrollIntoView();
		}

		#endregion

		#region EditNum

		public ICommand EditNumCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => { },
						() => UserOperateMode != OperateMode.Query && SelectedF194709 != null && F194709List != null && F194709List.All(o => o.NUM != TxtNum),
						o => DoEditNum());
			}
		}

		private void DoEditNum()
		{
			var list = F194709List ?? new List<F194709>();
			var item = list.Find(o => o.NUM == SelectedF194709.NUM);
			item.NUM = TxtNum;
			F194709List = list.ToList();
		}
		#endregion

		#endregion

		#region 車輛配送費用

		/// <summary>
		/// CurrentMaster.DC_CODE = 選定的DC
		/// CurrentMaster.ALL_ID = 選定的配送商
		/// </summary>

		private void SetCarFee()
		{
			CarKindList = GetCarKindList();
			InOutList = GetInOutList();
		}

		private void SetF194703()
		{
			GetF194703();
		}

		#region 編輯標的
		private F194703 _carData;

		public F194703 CarData
		{
			get { return _carData; }
			set
			{
				if (_carData == value) return;
				Set(() => CarData, ref _carData, value);
			}
		}
		#endregion

		#region 車輛種類
		private List<NameValuePair<string>> _carKindList;

		public List<NameValuePair<string>> CarKindList
		{
			get { return _carKindList; }
			set
			{
				if (_carKindList == value) return;
				Set(() => CarKindList, ref _carKindList, value);
			}
		}

		//取得車輛種類
		private List<NameValuePair<string>> GetCarKindList()
		{
			var proxy = GetExProxy<P19ExDataSource>();
			var results = proxy.CreateQuery<F194702Data>("GetF194702")
							.ToList()
							.Select(x => new NameValuePair<string>()
							{
								Name = x.CAR_KIND_NAME,
								Value = x.CAR_KIND_ID.ToString()
							}
							).ToList();
			return results;
		}
		#endregion

		#region 正逆物流
		private List<NameValuePair<string>> _inOutList;

		public List<NameValuePair<string>> InOutList
		{
			get { return _inOutList; }
			set
			{
				if (_inOutList == value) return;
				Set(() => InOutList, ref _inOutList, value);
			}
		}

		//取得正逆物流
		private List<NameValuePair<string>> GetInOutList()
		{
			List<NameValuePair<string>> results = new List<NameValuePair<string>>();
			results.Add(new NameValuePair<string>(Properties.Resources.P7109010000_ViewModel_I_EL, "I"));
			results.Add(new NameValuePair<string>(Properties.Resources.P7109010000_ViewModel_O_EL, "O"));
			return results;
		}
		#endregion

		#region 費用
		private Decimal _inputFee;

		public Decimal InputFee
		{
			get { return _inputFee; }
			set
			{
				if (_inputFee == value) return;
				Set(() => InputFee, ref _inputFee, value);
			}
		}
		#endregion

		#region 新增設定
		public ICommand AddCarCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => DoAddCar(),
												() => IsCarNotExist(),
												o => DoAddCarComplete());
			}
		}

		private void DoAddCar()
		{

		}

		private void DoAddCarComplete()
		{
			if (!IsCarNotExist()) return;
			if (F194703List == null) F194703List = new SelectionList<F194703>(new List<F194703>());
			F194703 f194703 = new F194703();
			f194703.DC_CODE = CurrentMaster.DC_CODE;
			f194703.ALL_ID = CurrentMaster.ALL_ID;
			f194703.CAR_KIND_ID = CarData.CAR_KIND_ID;
			f194703.IN_OUT = CarData.IN_OUT;
			f194703.FEE = CarData.FEE;
			F194703List.Add(new SelectionItem<F194703>(f194703));
		}

		private bool IsCarNotExist()
		{
			if (CurrentMaster == null || CarData == null)
				return false;

			if (string.IsNullOrEmpty(CurrentMaster.DC_CODE)
				|| string.IsNullOrEmpty(CurrentMaster.ALL_ID)
				|| CarData.CAR_KIND_ID == 0
				|| string.IsNullOrEmpty(CarData.IN_OUT))
				return false;

			if (CarData.FEE < 1)
				return false;

			if (F194703List == null || !F194703List.Any())
				return true;

			bool isExist = F194703List.Select(p => p.Item)
										.Where(o => o.DC_CODE == CurrentMaster.DC_CODE
												&& o.ALL_ID == CurrentMaster.ALL_ID
												&& o.CAR_KIND_ID == CarData.CAR_KIND_ID
												&& o.IN_OUT == CarData.IN_OUT)
										 .Any();
			return !isExist;
		}
		#endregion

		#region 刪除設定
		public ICommand RemoveCarCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => DoRemoveCar(),
					() => F194703List != null && F194703List.Any(p => p.IsSelected));
			}
		}

		private void DoRemoveCar()
		{
			var temp = F194703List.Where(o => o.IsSelected == false);
			F194703List = new SelectionList<F194703>(temp);
		}
		#endregion

		#region Get F194703

		private void GetF194703()
		{
			var proxyF19 = GetProxy<F19Entities>();
			var data = proxyF19.F194703s.Where(item => item.DC_CODE == CurrentMaster.DC_CODE &&
														item.ALL_ID == CurrentMaster.ALL_ID)
										.OrderBy(item => item.CAR_KIND_ID)
										.ToList();

			F194703List = new SelectionList<F194703>(data);
		}
		#endregion

		private bool _isSelectedAll = false;
		public bool IsSelectedAll
		{
			get { return _isSelectedAll; }
			set
			{
				_isSelectedAll = value;
				RaisePropertyChanged("IsSelectedAll");
			}
		}

		public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckAllItem()
				);
			}
		}

		public void DoCheckAllItem()
		{
			if (F194703List != null)
			{
				foreach (var p in F194703List)
				{
					p.IsSelected = IsSelectedAll;
				}
			}
		}

		private SelectionList<F194703> _f194703List;

		public SelectionList<F194703> F194703List
		{
			get { return _f194703List; }
			set
			{
				_f194703List = value;
				RaisePropertyChanged("F194703List");
			}
		}


		#endregion
	}
}
