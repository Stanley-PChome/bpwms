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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7109030000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		public Action ToFirstTab = delegate { };
		public Action OnSaved = delegate { };
		public Action OnUpdateTab = () => { };
		public Action OnDataGridScrollIntoView = () => { };
		private string _gupShare;

		#region 外部參數
		public string NewUniForm { get; set; }
		public void CheckLoad()
		{
			if (UserOperateMode != OperateMode.Query)
				return;

			if (!string.IsNullOrWhiteSpace(NewUniForm))
			{
				DoAdd();
				SelectedData.GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode;
				SelectedData.UNI_FORM = NewUniForm;
			}
		}
		#endregion

		#region 查詢
		#region Form - 業主
		private List<NameValuePair<string>> _gupCodes;

		public List<NameValuePair<string>> GupCodes
		{
			get { return _gupCodes; }
			set
			{
				_gupCodes = value;
				RaisePropertyChanged("GupCodes");
			}
		}

		private string _selectedGupCode;
		public string SelectedGupCode
		{
			get { return _selectedGupCode; }
			set
			{
				_selectedGupCode = value;
				RaisePropertyChanged("SelectedGupCode");
			}
		}
		#endregion
		#region Form - 貨主
		private string _searchCustName;
		public string SearchCustName
		{
			get { return _searchCustName; }
			set
			{
				_searchCustName = value;
				RaisePropertyChanged("SearchCustName");
			}
		}
		#endregion
		#region Data - 資料List
		private List<F1909> _dgList;
		public List<F1909> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}



		private F1909 _selectedDataByView;

		public F1909 SelectedDataByView
		{
			get { return _selectedDataByView; }
			set
			{
				if (_selectedDataByView == value)
					return;
				Set(() => SelectedDataByView, ref _selectedDataByView, value);
			}
		}

		private F1909 _selectedData;

		public F1909 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
				if (value == null)
					SelectedDataByView = null;
				else
				{
					SelectedDataByView = SelectedData;
					SpBox = GetSP_BOXList().Where(x => x.Value == SelectedDataByView.SP_BOX_CODE).FirstOrDefault()?.Name;

					var proxy = GetProxy<F19Entities>();
					DgItemList = proxy.F190902s.Where(x => x.GUP_CODE.Equals(SelectedData.GUP_CODE)
																							&& x.CUST_CODE.Equals(SelectedData.CUST_CODE))
																		 .OrderBy(x => x.DM_SEQ)
																		 .ToObservableCollection();
				}

			}
		}
		#endregion
		#endregion

		#region 新增/編輯
		#region Form - 業主
		private List<NameValuePair<string>> _gupList;

		public List<NameValuePair<string>> GupList
		{
			get { return _gupList; }
			set
			{
				_gupList = value;
				RaisePropertyChanged("GupList");
			}
		}
		#endregion
		#region Form - 幣別
		private List<NameValuePair<string>> _currencyList;

		public List<NameValuePair<string>> CurrencyList
		{
			get { return _currencyList; }
			set
			{
				_currencyList = value;
				RaisePropertyChanged("CurrencyList");
			}
		}
		#endregion

		#region Form - 解鎖功能
		private List<NameValuePair<string>> _lockData;

		public List<NameValuePair<string>> LockData
		{
			get { return _lockData; }
			set
			{
				_lockData = value;
				RaisePropertyChanged("LockData");
			}
		}
		#endregion
		#region Form - 付款條件
		private List<NameValuePair<string>> _pAY_FACTORList;

		public List<NameValuePair<string>> PAY_FACTORList
		{
			get { return _pAY_FACTORList; }
			set
			{
				_pAY_FACTORList = value;
				RaisePropertyChanged("PAY_FACTORList");
			}
		}
		#endregion
		#region Form - 付款方式
		private List<NameValuePair<string>> _pAY_TYPEList;

		public List<NameValuePair<string>> PAY_TYPEList
		{
			get { return _pAY_TYPEList; }
			set
			{
				_pAY_TYPEList = value;
				RaisePropertyChanged("PAY_TYPEList");
			}
		}
		#endregion
		#region Form - 指定退貨物流中心
		private List<NameValuePair<string>> _dcCodes;

		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				RaisePropertyChanged("DcCodes");
			}
		}
		#endregion

		#region Form - 進貨箱入數與商品主檔不吻合
		private List<NameValuePair<string>> _special_InList;

		public List<NameValuePair<string>> Special_InList
		{
			get { return _special_InList; }
			set
			{
				_special_InList = value;
				RaisePropertyChanged("Special_InList");
			}
		}
		#endregion

		#region Form - 是否列印進倉棧板標籤

		private List<NameValuePair<string>> _isPrintInstockpallersticker_List;

		public List<NameValuePair<string>> IsPrintInstockpallersticker_List
		{
			get { return _isPrintInstockpallersticker_List; }
			set
			{
				_isPrintInstockpallersticker_List = value;
				RaisePropertyChanged("IsPrintInstockpallersticker_List");
			}
		}
		#endregion

		#region Form - 特殊紙箱
		private List<NameValuePair<string>> _sP_BOXList;

		public List<NameValuePair<string>> SP_BOXList
		{
			get { return _sP_BOXList; }
			set
			{
				_sP_BOXList = value;
				RaisePropertyChanged("SP_BOXList");
			}
		}
		#endregion
		#region Form - DM名稱
		private string _dmName;

		public string DmName
		{
			get { return _dmName; }
			set
			{
				_dmName = value;
				RaisePropertyChanged("DmName");
			}
		}
		#endregion
		#region Form - DM起迄
		private DateTime _dmBegin;

		public DateTime DmBegin
		{
			get { return _dmBegin; }
			set
			{
				_dmBegin = value;
				RaisePropertyChanged("DmBegin");
			}
		}
		private DateTime _dmEnd;

		public DateTime DmEnd
		{
			get { return _dmEnd; }
			set
			{
				_dmEnd = value;
				RaisePropertyChanged("DmEnd");
			}
		}
		#endregion
		#region Form - 列印工具
		private List<NameValuePair<string>> _pRINT_TYPEList;

		public List<NameValuePair<string>> PRINT_TYPEList
		{
			get { return _pRINT_TYPEList; }
			set
			{
				_pRINT_TYPEList = value;
				RaisePropertyChanged("PRINT_TYPEList");
			}
		}
		#endregion
		#region Form - 部分出貨方式
		private List<NameValuePair<string>> _sPILT_OUTCHECKWAYList;

		public List<NameValuePair<string>> SPILT_OUTCHECKWAYList
		{
			get { return _sPILT_OUTCHECKWAYList; }
			set
			{
				_sPILT_OUTCHECKWAYList = value;
				RaisePropertyChanged("SPILT_OUTCHECKWAYList");
			}
		}
		#endregion
		#region Form - 預設郵遞區號
		private List<NameValuePair<string>> _zIP_CODEList;

		public List<NameValuePair<string>> ZIP_CODEList
		{
			get { return _zIP_CODEList; }
			set
			{
				_zIP_CODEList = value;
				RaisePropertyChanged("ZIP_CODEList");
			}
		}
		#endregion
		#region Data - DM資料List
		private ObservableCollection<F190902> _dgItemList;
		public ObservableCollection<F190902> DgItemList
		{
			get { return _dgItemList; }
			set
			{
				_dgItemList = value;
				RaisePropertyChanged("DgItemList");
			}
		}

		private F190902 _selectedDMData;

		public F190902 SelectedDMData
		{
			get { return _selectedDMData; }
			set
			{
				_selectedDMData = value;
				RaisePropertyChanged("SelectedDMData");
			}
		}
		#endregion
		#region Form - 廠退方式
		private List<NameValuePair<string>> _vnrRtnType;

		public List<NameValuePair<string>> VnrRtnType
		{
			get { return _vnrRtnType; }
			set
			{
				_vnrRtnType = value;
				RaisePropertyChanged("VnrRtnType");
			}
		}
		#endregion
		#region Form - 報廢銷毀方式
		private List<NameValuePair<string>> _destroyType;

		public List<NameValuePair<string>> DestroyType
		{
			get { return _destroyType; }
			set
			{
				_destroyType = value;
				RaisePropertyChanged("DestroyType");
			}
		}
		#endregion
		#region 
		private string _spBox;
		public string SpBox
		{
			get { return _spBox; }
			set
			{
				_spBox = value;
				RaisePropertyChanged("SpBox");
			}
		}

		#endregion


		#endregion
		#endregion

		#region 函式
		public P7109030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			_userId = Wms3plSession.Get<UserInfo>().Account;
			_userName = Wms3plSession.Get<UserInfo>().AccountName;
			//Wms3plSession.Get<GlobalInfo>().GetGupDataList("");
			GupCodes = GetGupList(true);
			if (GupCodes.Any())
				SelectedGupCode = GupCodes.FirstOrDefault().Value;
			#region Add/Edit
			GupList = GetGupList();
			CurrencyList = GetCurrencyList();
			PAY_FACTORList = GetPAY_FACTORList();
			PAY_TYPEList = GetPAY_TYPEList();
			DcCodes = GetDcCodeList();
			Special_InList = GetSpecial_InList();
			LockData = GetLockList();
			PRINT_TYPEList = GetPRINT_TYPEList();
			SPILT_OUTCHECKWAYList = GetSPILT_OUTCHECKWAYList();
			ZIP_CODEList = GetZIP_CODEList();
			IsPrintInstockpallersticker_List = GetIsPrintInstockpallersticker_List();
			VnrRtnType = GetVnrRtnType();
			DestroyType = GetDestroyType();
			#endregion
		}

		public List<NameValuePair<string>> GetGupList(bool canAll = false)
		{
			var proxy = GetProxy<F19Entities>();
			var query = from item in proxy.F1929s
									select new NameValuePair<string>()
									{
										Value = item.GUP_CODE,
										Name = item.GUP_NAME
									};

			var list = query.ToList();

			if (canAll)
			{
				list.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			}
			return list;
		}

		/// <summary>
		/// 取得 F000904 的列舉值
		/// </summary>
		/// <param name="topic"></param>
		/// <param name="subTopic"></param>
		/// <returns></returns>
		public List<NameValuePair<string>> GetF000904NameValueList(string topic, string subTopic)
		{
			return GetBaseTableService.GetF000904List(FunctionCode, topic, subTopic);
		}

		public List<NameValuePair<string>> GetCurrencyList()
		{
			return GetF000904NameValueList("F1909", "CURRENCY");
		}

		public List<NameValuePair<string>> GetPAY_FACTORList()
		{
			return GetF000904NameValueList("F1909", "PAY_FACTOR");
		}

		public List<NameValuePair<string>> GetPAY_TYPEList()
		{
			return GetF000904NameValueList("F1909", "PAY_TYPE");
		}

		private List<NameValuePair<string>> GetDcCodeList()
		{
			return Wms3plSession.Get<GlobalInfo>().DcCodeList;
		}

		public List<NameValuePair<string>> GetSpecial_InList()
		{
			return GetF000904NameValueList("F1909", "SPECIAL_IN");
		}

		public List<NameValuePair<string>> GetIsPrintInstockpallersticker_List()
		{
			return GetF000904NameValueList("F1909", "IS_PRINT_INSTOCKPALLETSTICKER");
		}

		public List<NameValuePair<string>> GetLockList()
		{
			return GetF000904NameValueList("F1909", "MANAGER_LOCK");
		}
		public List<NameValuePair<string>> GetSP_BOXList()
		{
			if (SelectedData == null)
				return new List<NameValuePair<string>>();

			var proxy = GetProxy<F19Entities>();
			var query = proxy.CreateQuery<F1903>("GetF1903sByCarton")
																							.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
																							.AddQueryExOption("custCode", SelectedData.CUST_CODE)
																							.AddQueryExOption("isCarton", "1")
																							.Select(o => new NameValuePair<string>(o.ITEM_NAME, o.ITEM_CODE));

			return query.ToList();
		}
		public List<NameValuePair<string>> GetPRINT_TYPEList()
		{
			return GetF000904NameValueList("F1909", "PRINT_TYPE");
		}
		public List<NameValuePair<string>> GetSPILT_OUTCHECKWAYList()
		{
			return GetF000904NameValueList("F1909", "SPILT_OUTCHECKWAY");
		}
		public List<NameValuePair<string>> GetZIP_CODEList()
		{
			return GetF000904NameValueList("F1909", "ZIP_CODE");
		}
		public List<NameValuePair<string>> GetVnrRtnType()
		{
			return GetF000904NameValueList("F1909", "VNR_RTN_TYPE");
		}
		public List<NameValuePair<string>> GetDestroyType()
		{
			return GetF000904NameValueList("F1909", "DESTROY_TYPE");
		}
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

		private void DoSearch()
		{
			DgList = new List<F1909>();
			//執行查詢動
			var proxy = GetProxy<F19Entities>();
			var qry = proxy.CreateQuery<F1909>("GetF1909Datas")
				.AddQueryOption("gupCode", string.Format("'{0}'", SelectedGupCode))
				.AddQueryOption("custName", string.Format("'{0}'", SearchCustName))
				.AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
				.ToList();

			if (qry != null && qry.Any())
				DgList = qry;
			else
				ShowMessage(Messages.InfoNoData);
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
			SelectedData = new F1909
			{
				PAY_FACTOR = "0",
				PAY_TYPE = "0",
				ORDER_ADDRESS = "0",
				MIX_LOC_BATCH = "0",
				MIX_LOC_ITEM = "1",
				DC_TRANSFER = "0",
				BOUNDLE_SERIALLOC = "0",
				FLUSHBACK = "0",
				SAM_ITEM = "0",
				INSIDER_TRADING = "0",
				SPILT_ORDER = "0",
				B2C_CAN_LACK = "1",
				CAN_FAST = "0",
				INSTEAD_INVO = "0",
				SPILT_INCHECK = "0",
				SPECIAL_IN = "0",
				NEED_SEAL = "0",
				DM = "0",
				RIBBON = "0",
				CUST_BOX = "0",
				SP_BOX = "0",
				TAX_TYPE = "0",
				AUTO_GEN_RTN = "1",
				STATUS = "0",
				CURRENCY = "NTD",
				GUPSHARE = "0",
				CUST_CODE = Properties.Resources.P7109030000_ViewModel_AutoNo,
				ISPRINTDELV = "0",
				ISPRINTDELVDTL = "0",
				ISPICKLOCFIRST = "0",
				ISOUTOFSTOCKRECV = "0",
				ISDELV_NOLOADING = "0",
				MANAGER_LOCK = "1",
				ISPICKSHOWCUSTNAME = "0",
				ISBACK_DISTR = "0",
				CAL_CUFT = "0",
				CUFT_FACTOR = 2700,
				CUFT_BLUK = 2,
				PRINT_TYPE = "0",
				SHOW_UNIT_TRANS = "1",
				ISLATEST_VALID_DATE = "0",
				ISB2B_ALONE_OUT = "0",
				ISALLOW_DELV_DAY = "0",
				ZIP_CODE = "",
				SPILT_OUTCHECK = "0",
				SPILT_OUTCHECKWAY = "0",
				ISDELV_LOADING_CHECKCODE = "0",
				IS_SINGLEBOXCHECK = "0",
				ISSHIFTITEM = "0",
				NEED_ITEMSPEC = "0",
				IS_QUICK_CHECK = "0",
				INSTOCKAUTOCLOSED = 0,
				ALLOWOUTSHIPDETLOG = "0",
				SPILT_VENDER_ORD = "0",
				CHG_VENDER_ORD = "0",
				ALLOW_ADVANCEDSTOCK = "0",
				PRINT2PDF = "0",
				VNR_RTN_TYPE = "0",
				DESTROY_TYPE = "0",
				ALLOW_EDIT_BOX_QTY = "0",
				SHOW_QTY = "0",
				SHOW_MESSAGE = "0",
				SELFTAKE_CHECKCODE = "0",
				MIX_SERIAL_NO = "0",
				ISPRINT_SELFTAKE = "0",
				ALLOWREPEAT_ITEMBARCODE = "0",
				IS_ORDDATE_TODAY = "1",
				ALLOWGUP_ITEMCATEGORYSHARE = "0",
				ALLOWGUP_VNRSHARE = "0",
				ALLOWGUP_RETAILSHARE = "0",
				ALLOWRT_SPECIALBUY = "0",
				ALLOW_NOPRINTPICKORDER = "0",
				ALLOW_NOSHIPPACKAGE = "0",
				ALLOW_ADDBOXNOCHECK = "0",
				ISPICKSHOWVALIDDATE = "0",
				ISALLOCATIONSHOWVALIDDATE = "0",
				PACKCOUNT_MAX_UNIT = "0",
				IS_PRINT_INSTOCKPALLETSTICKER = "0",
				SUGGEST_LOC_TYPE = "0",
				ALLOW_CANCEL_LACKORD = "0",
				ALLOCATIONCHANGVALIDATE = "1",
				ALLOCATIONCHANGMAKENO = "1",
				ALLOW_ORDER_NO_DELV = "0"

			};

			_gupShare = "0";

			DgItemList = new ObservableCollection<F190902>();
			SetDefaultByDM();
			SP_BOXList = new List<NameValuePair<string>>();
			UserOperateMode = OperateMode.Add;
			OnUpdateTab();
			//執行新增動作
		}
		#endregion Add

		#region AddDM
		ICommand _addDMCommand;
		public ICommand AddDMCommand
		{
			get
			{
				return _addDMCommand ??
						(_addDMCommand = new RelayCommand(
							 () => DoAddDM(),
							 () => UserOperateMode != OperateMode.Query
																												&& !string.IsNullOrWhiteSpace(DmName)
																												&& DmBegin != null
																												&& DmEnd != null));
			}
		}

		void SetDefaultByDM()
		{
			DmBegin = DateTime.Today;
			DmEnd = DateTime.Today;
			DmName = string.Empty;
		}

		string GetEditableDmError()
		{

			DmName = DmName.Trim();
			if (string.IsNullOrEmpty(DmName))
			{
				return Properties.Resources.P7109030000_ViewModel_Input_DMName;
			}

			if (DgItemList.Where(item => item.DM_NAME == DmName)
										.Where(item => (item.BEGIN_DATE <= DmBegin && DmBegin <= item.END_DATE)
																 || (item.BEGIN_DATE <= DmEnd && DmEnd <= item.END_DATE)
																 || (DmBegin <= item.BEGIN_DATE && item.END_DATE <= DmEnd))
										.Any())
			{
				return Properties.Resources.P7109030000_ViewModel_Same_DMName_DatePeriod_Duplicate;
			}

			return string.Empty;
		}

		private void DoAddDM()
		{

			var error = GetEditableDmError();
			if (!string.IsNullOrEmpty(error))
			{
				ShowWarningMessage(error);
				return;
			}

			DgItemList.Add(new F190902
			{
				DM_NAME = DmName,
				BEGIN_DATE = DmBegin,
				END_DATE = DmEnd
			});

			DgItemList = DgItemList.OrderBy(item => item.DM_NAME).ThenBy(item => item.BEGIN_DATE).ToObservableCollection();

			SetDefaultByDM();

		}
		#endregion AddDM

		#region DelDM
		private ICommand _delDMCommand;
		public ICommand DelDMCommand
		{
			get
			{
				return _delDMCommand ?? (
								_delDMCommand = new RelayCommand(
								() => DelDMComplate(),
								() => UserOperateMode != OperateMode.Query
								));
			}
		}

		private void DoDelDM()
		{
		}

		private void DelDMComplate()
		{
			if (SelectedDMData == null)
				return;

			DgItemList.Remove(SelectedDMData);
		}
		#endregion DelDM

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData != null,
										o =>
										{
											SetDefaultByDM();
											var tmpSelectedData = SelectedData.Clone();
											SP_BOXList = GetSP_BOXList();
											UserOperateMode = OperateMode.Edit;
											OnUpdateTab();
											_gupShare = SelectedData.GUPSHARE;
											SP_BOXList = SP_BOXList;
											SelectedData = tmpSelectedData;
										}
					);
			}
		}

		private void DoEdit()
		{

			SelectedData = AutoMapper.Mapper.DynamicMap<F1909>(SelectedData);



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
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				if (UserOperateMode == OperateMode.Add)
				{
					SelectedData = null;
				}
				else
				{
					SelectedData = DgList.Where(item => item.GUP_CODE == SelectedData.GUP_CODE && item.CUST_CODE == SelectedData.CUST_CODE).FirstOrDefault();
				}
				ToFirstTab();
				UserOperateMode = OperateMode.Query;
			}
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null
					);
			}
		}

		private void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
				return;

			var proxy = GetExProxy<P71ExDataSource>();
			var query = proxy.CreateQuery<Wms3pl.WpfClient.ExDataServices.P71ExDataService.ExecuteResult>("DeleteP710903")
											 .AddQueryExOption("gupCode", SelectedData.GUP_CODE)
											 .AddQueryExOption("custCode", SelectedData.CUST_CODE);

			var result = query.ToList().FirstOrDefault();
			ShowResultMessage(result);
			DoSearch();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				string gupCode = string.Empty;
				string custCode = string.Empty;
				return CreateBusyAsyncCommand(
					o =>
					{
						// 新增後，紀錄剛新增那筆貨主代碼
						gupCode = string.Empty;
						custCode = string.Empty;
						if (SelectedData != null)
						{
							gupCode = SelectedData.GUP_CODE;
							custCode = SelectedData.CUST_CODE;
						}

						isSaved = DoSave();
					},
					() => CanSave(SelectedData),
					o =>
					{
						// 查詢後，選擇剛新增那筆
						if (isSaved)
						{
							if (DgList != null && !string.IsNullOrEmpty(custCode))
							{
								var f1909 = DgList.Where(item => item.GUP_CODE == gupCode && item.CUST_CODE == custCode).FirstOrDefault();
								if (f1909 != null)
								{
									SelectedData = f1909;
									OnDataGridScrollIntoView();
									OnSaved();
								}
							}
						}
					}
					);
			}
		}

		bool CanSave(F1909 e)
		{
			if (UserOperateMode == OperateMode.Query)
				return false;

			if (e == null)
				return false;

			if (e.DM == "1" && !DgItemList.Any())
				return false;

			if (e.INSIDER_TRADING == "1" && (!e.INSIDER_TRADING_LIM.HasValue || e.INSIDER_TRADING_LIM.Value <= 0))
				return false;

			if (e.RIBBON == "1" && (!e.RIBBON_BEGIN_DATE.HasValue || !e.RIBBON_END_DATE.HasValue))
				return false;

			if (e.SP_BOX == "1" && (string.IsNullOrEmpty(e.SP_BOX_CODE) || !e.SPBOX_BEGIN_DATE.HasValue || !e.SPBOX_END_DATE.HasValue))
				return false;

			return true;
		}

		private bool CheckMail(string mail)
		{
			if (!string.IsNullOrWhiteSpace(mail))
			{
				var checkMail = new CheckEmailHelper();
				var mailList = mail.Split(',');
				return mailList.All(checkMail.IsValidEmail);
			}
			return true;
		}

		private bool DoSave()
		{
			ExDataMapper.Trim(SelectedData);
			bool isOk = true;
			isOk = CheckMail(SelectedData.ITEM_MAIL);
			if (!isOk)
			{
				ShowWarningMessage(Properties.Resources.P7109030000_ViewModel_Cust_Email_FormatError);
				return false;
			}
			isOk = CheckMail(SelectedData.BILL_MAIL);
			if (!isOk)
			{
				ShowWarningMessage(Properties.Resources.P7109030000_ViewModel_BillContact_Email_FormatError);
				return false;
			}
			if (_gupShare != SelectedData.GUPSHARE && SelectedData.GUPSHARE == "1")
			{
				if (ShowConfirmMessage(Properties.Resources.P7109030000_ViewModel_Gup_Item_Sharing_SureExecute) == DialogResponse.No)
					isOk = false;
			}
			if (isOk)
			{
				//執行確認儲存動作
				if (UserOperateMode == OperateMode.Add)
					return DoAddSave();
				if (UserOperateMode == OperateMode.Edit)
					return DoEditSave();
			}
			return false;
		}

		private bool DoAddSave()
		{
			if (SelectedData != null)
			{
				MessagesStruct alertMessage = new MessagesStruct();
				alertMessage.Button = DialogButton.OK;
				alertMessage.Title = Resources.Resources.Information;
				alertMessage.Image = DialogImage.Warning;
				alertMessage.Message = Properties.Resources.P7109030000_ViewModel_SelectData;

				if (string.IsNullOrWhiteSpace(SelectedData.GUP_CODE))
				{
					ShowMessage(Messages.WarningNoGupCode);
					return false;
				}
				if (string.IsNullOrWhiteSpace(SelectedData.CUST_CODE))
				{
					ShowMessage(Messages.WarningNoCustCode);
					return false;
				}
				if (string.IsNullOrWhiteSpace(SelectedData.CUST_NAME))
				{
					ShowMessage(Messages.WarningNoCustName);
					return false;
				}
				if (string.IsNullOrWhiteSpace(SelectedData.CURRENCY))
				{
					alertMessage.Message = Properties.Resources.P7109030000_ViewModel_SelectCurrency;
					ShowMessage(alertMessage);
					return false;
				}
				if (SelectedData.RIBBON_BEGIN_DATE != null && SelectedData.RIBBON_END_DATE != null)
				{
					if (SelectedData.RIBBON_BEGIN_DATE > SelectedData.RIBBON_END_DATE)
					{
						alertMessage.Message = Properties.Resources.P7101030000_ViewModel_BeginDate_GreaterThan_EndDate;
						ShowMessage(alertMessage);
						return false;
					}
				}
				if (SelectedData.SPBOX_BEGIN_DATE != null && SelectedData.SPBOX_END_DATE != null)
				{
					if (SelectedData.SPBOX_BEGIN_DATE > SelectedData.SPBOX_END_DATE)
					{
						alertMessage.Message = Properties.Resources.P7101030000_ViewModel_BeginDate_GreaterThan_EndDate;
						ShowMessage(alertMessage);
						return false;
					}
				}

				var error = GetEditableError(SelectedData);
				if (!string.IsNullOrEmpty(error))
				{
					DialogService.ShowMessage(error);
					return false;
				}



				var proxy = new wcf.P71WcfServiceClient();
				var f1909 = ExDataMapper.Map<F1909, wcf.F1909>(SelectedData);
				var f190902s = ExDataMapper.MapCollection<F190902, wcf.F190902>(DgItemList).ToArray();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
						() => proxy.InsertOrUpdateP710903(f1909, f190902s, isAdd: true));

				ShowResultMessage(result);
				if (result.IsSuccessed)
				{
					ToFirstTab();
					UserOperateMode = OperateMode.Query;
					SelectedGupCode = string.Empty;
					SearchCustName = string.Empty;
					DoSearch();
				}
				else
					return false;

			}
			return true;
		}

		private bool DoEditSave()
		{
			if (SelectedData != null)
			{
				MessagesStruct alertMessage = new MessagesStruct();
				alertMessage.Button = DialogButton.OK;
				alertMessage.Title = Resources.Resources.Information;
				alertMessage.Image = DialogImage.Warning;
				alertMessage.Message = Properties.Resources.P7109030000_ViewModel_SelectData;

				if (string.IsNullOrWhiteSpace(SelectedData.GUP_CODE))
				{
					ShowMessage(Messages.WarningNoGupCode);
					return false;
				}
				if (string.IsNullOrWhiteSpace(SelectedData.CUST_CODE))
				{
					ShowMessage(Messages.WarningNoCustCode);
					return false;
				}
				if (string.IsNullOrWhiteSpace(SelectedData.CUST_NAME))
				{
					ShowMessage(Messages.WarningNoCustName);
					return false;
				}
				if (string.IsNullOrWhiteSpace(SelectedData.CURRENCY))
				{
					alertMessage.Message = Properties.Resources.P7109030000_ViewModel_SelectCurrency;
					ShowMessage(alertMessage);
					return false;
				}
				if (SelectedData.RIBBON_BEGIN_DATE != null && SelectedData.RIBBON_END_DATE != null)
				{
					if (SelectedData.RIBBON_BEGIN_DATE > SelectedData.RIBBON_END_DATE)
					{
						alertMessage.Message = Properties.Resources.P7101030000_ViewModel_BeginDate_GreaterThan_EndDate;
						ShowMessage(alertMessage);
						return false;
					}
				}
				if (SelectedData.SPBOX_BEGIN_DATE != null && SelectedData.SPBOX_END_DATE != null)
				{
					if (SelectedData.SPBOX_BEGIN_DATE > SelectedData.SPBOX_END_DATE)
					{
						alertMessage.Message = Properties.Resources.P7101030000_ViewModel_BeginDate_GreaterThan_EndDate;
						ShowMessage(alertMessage);
						return false;
					}
				}

				var error = GetEditableError(SelectedData);
				if (!string.IsNullOrEmpty(error))
				{
					DialogService.ShowMessage(error);
					return false;
				}

				var proxy = new wcf.P71WcfServiceClient();
				var f1909 = ExDataMapper.Map<F1909, wcf.F1909>(SelectedData);
				var f190902s = ExDataMapper.MapCollection<F190902, wcf.F190902>(DgItemList).ToArray();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
						() => proxy.InsertOrUpdateP710903(f1909, f190902s, isAdd: false));
				ShowResultMessage(result);
				if (result.IsSuccessed)
				{
					ToFirstTab();
					UserOperateMode = OperateMode.Query;
					SelectedGupCode = string.Empty;
					SearchCustName = string.Empty;
					DoSearch();
				}
				else
					return false;
			}
			return true;
		}


		private string GetEditableError(F1909 e)
		{
			if (e == null)
				return string.Empty;

			if (string.IsNullOrWhiteSpace(e.CUST_CODE))
			{
				return Properties.Resources.P7109030000_ViewModel_CustCode_Empty;
			}

			//if (!ValidateHelper.IsMatchAZaz09(e.CUST_CODE))
			//{
			//	return Properties.Resources.P7109030000_ViewModel_CustCode_CNWordOnly;
			//}

			if (string.IsNullOrWhiteSpace(e.SYS_CUST_CODE))
			{
				return Properties.Resources.P7109030000_ViewModel_SYS_CustCode_Empty;
			}

			if (!ValidateHelper.IsMatchAZaz09(e.SYS_CUST_CODE))
			{
				return Properties.Resources.P7109030000_ViewModel_SYS_CustCode_CNWordOnly;
			}

			var proxy = GetModifyQueryProxy<F19Entities>();

			if (UserOperateMode == OperateMode.Add)
			{
				if (proxy.F1909s.Where(item => item.GUP_CODE == e.GUP_CODE
					&& (item.CUST_CODE == e.CUST_CODE || item.SYS_CUST_CODE == e.SYS_CUST_CODE)).Count() > 0)
				{
					return Properties.Resources.P7109030000_ViewModel_CustCode_SYSCustCode_Duplicate;
				}
			}
			else
			{
				if (proxy.F1909s.Where(item => item.GUP_CODE == e.GUP_CODE && item.CUST_CODE == e.CUST_CODE && item.STATUS != "9").Count() == 0)
				{
					return Properties.Resources.P7109030000_ViewModel_Cust_Deleted;
				}

				if (proxy.F1909s.Where(item => item.GUP_CODE == e.GUP_CODE
					&& item.CUST_CODE != e.CUST_CODE
					&& item.SYS_CUST_CODE == e.SYS_CUST_CODE).Count() > 0)
				{
					return Properties.Resources.P7109030000_ViewModel_SYS_GupCode_Duplicate;
				}
			}

			if (!string.IsNullOrEmpty(e.UNI_FORM))
			{
				bool isCheckUniForm = true;
				if (UserOperateMode == OperateMode.Edit)
				{
					var f1909Entity = proxy.F1909s.Where(x => x.GUP_CODE == e.GUP_CODE
															 && x.CUST_CODE == e.CUST_CODE)
													.FirstOrDefault();
					isCheckUniForm = f1909Entity.UNI_FORM != e.UNI_FORM;
				}

				if (isCheckUniForm)
				{
					var exproxy = GetExProxy<P71ExDataSource>();
					var uniFormExists = exproxy.CreateQuery<bool>("ExistsUniForm")
											 .AddQueryExOption("uniForm", e.UNI_FORM)
											 .ToList().FirstOrDefault();

					if (uniFormExists)
						return Properties.Resources.P7109030000_ViewModel_UniFormExists;
				}
			}

			return string.Empty;
		}
		#endregion Save
		#endregion
	}
}
