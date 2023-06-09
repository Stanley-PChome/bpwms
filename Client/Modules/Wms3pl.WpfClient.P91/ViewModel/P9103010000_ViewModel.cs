using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F91DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Text.RegularExpressions;


namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9103010000_ViewModel : InputViewModelBase
	{
		private readonly F91Entities _proxy;
		private string _userId;
		private string _userName;
		private string _custCode;
		private string _gupCode;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };
		public Action DeleteAction = delegate { };
		private bool _isSave;
		public P9103010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_proxy = GetProxy<F91Entities>();
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				GetUnitList();
				//S_ITEM_CODE = "040985";
				S_STATUS = "0";
				S_BOM_TYPE = "0";
			}
		}

		#region
		private ObservableCollection<F910101Ex> _dgList;
		public ObservableCollection<F910101Ex> dgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("dgList");
			}
		}

		private ObservableCollection<F910102Ex> _dgList2;
		public ObservableCollection<F910102Ex> dgList2
		{
			get { return _dgList2; }
			set
			{
				_dgList2 = value;
				RaisePropertyChanged("dgList2");
			}
		}

		private ObservableCollection<F910102Ex> _dgList3;
		public ObservableCollection<F910102Ex> dgList3
		{
			get { return _dgList3; }
			set
			{
				_dgList3 = value;
				RaisePropertyChanged("dgList3");
			}
		}

		private F910101Ex _selectedItem;
		public F910101Ex SELECTED_ITEM
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				SetBomData();
				if (value != null)
				{
					SearchResultIsExpanded = dgList.Count > 1;
				}
				RaisePropertyChanged("SELECTED_ITEM");
			}
		}

		private F910101Ex _newItem;
		public F910101Ex NEW_ITEM
		{
			get { return _newItem; }
			set
			{
				_newItem = value;
				RaisePropertyChanged("NEW_ITEM");
			}
		}

		private F910102Ex _selectedItem2;
		public F910102Ex SELECTED_ITEM2
		{
			get { return _selectedItem2; }
			set
			{
				_selectedItem2 = value;
				RaisePropertyChanged("SELECTED_ITEM2");
			}
		}

		private F910102Ex _selectedItem3;
		public F910102Ex SELECTED_ITEM3
		{
			get { return _selectedItem3; }
			set
			{
				_selectedItem3 = value;
				SetNewItemDetail();
				RaisePropertyChanged("SELECTED_ITEM3");
			}
		}
		#endregion

		private List<NameValuePair<string>> _UnitList;
		public List<NameValuePair<string>> UnitList
		{
			get { return _UnitList; }
			set { _UnitList = value; RaisePropertyChanged("UnitList"); }
		}

		private void GetUnitList()
		{

			var proxy = GetProxy<F91Entities>();
		
			var qry = proxy.F91000302s.Where(o => o.ITEM_TYPE_ID == "001").Select(o => new
					  {
						  o.ACC_UNIT,
						  o.ACC_UNIT_NAME
					  });

			UnitList = qry.ToList().OrderBy(x => x.ACC_UNIT)
												.Select(x => new NameValuePair<string>()
												{
													Name = x.ACC_UNIT_NAME,
													Value = x.ACC_UNIT
												}).AsQueryable().ToList();
		}

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


		#region 查詢模式用欄位
		private string _sitemcode;
		public string S_ITEM_CODE
		{
			get { return _sitemcode; }
			set
			{
				_sitemcode = value;
				RaisePropertyChanged("S_ITEM_CODE");
			}
		}

		private string _searchBomNo = string.Empty;

		public string SearchBomNo
		{
			get { return _searchBomNo; }
			set
			{
				_searchBomNo = value;
				RaisePropertyChanged("SearchBomNo");
			}
		}
		private string _sBOM_TYPE;
		public string S_BOM_TYPE
		{
			get { return _sBOM_TYPE; }
			set
			{
				_sBOM_TYPE = value;
			}
		}

		private string _sstatus;
		public string S_STATUS
		{
			get { return _sstatus; }
			set
			{
				_sstatus = value;
			}
		}
		#endregion

		#region BomData欄位
		private string bitemcode;
		public string B_ITEM_CODE
		{
			get { return bitemcode; }
			set
			{
				bitemcode = value;
				RaisePropertyChanged("B_ITEM_CODE");
			}
		}
		private string bstatus;
		public string B_STATUS
		{
			get { return bstatus; }
			set
			{
				bstatus = value;
				RaisePropertyChanged("B_STATUS");
			}
		}
		private string bitemname;
		public string B_ITEM_NAME
		{
			get { return bitemname; }
			set
			{
				bitemname = value;
				RaisePropertyChanged("B_ITEM_NAME");
			}
		}
		private string bbomtype;
		public string B_BOM_TYPE
		{
			get { return bbomtype; }
			set
			{
				bbomtype = value;
				RaisePropertyChanged("B_BOM_TYPE");
			}
		}
		private string bbomname;
		public string B_BOM_NAME
		{
			get { return bbomname; }
			set
			{
				bbomname = value;
				RaisePropertyChanged("B_BOM_NAME");
			}
		}
		private string bunit;
		public string B_UNIT
		{
			get { return bunit; }
			set
			{
				bunit = value;
				RaisePropertyChanged("B_UNIT");
			}
		}
		private decimal bcheckpercent;
		public decimal B_CHECK_PERCENT
		{
			get { return bcheckpercent; }
			set
			{
				bcheckpercent = value;
				RaisePropertyChanged("B_CHECK_PERCENT");
			}
		}
		private string bspecdesc;
		public string B_SPEC_DESC
		{
			get { return bspecdesc; }
			set
			{
				bspecdesc = value;
				RaisePropertyChanged("B_SPEC_DESC");
			}
		}
		private string bpackagedesc;
		public string B_PACKAGE_DESC
		{
			get { return bpackagedesc; }
			set
			{
				bpackagedesc = value;
				RaisePropertyChanged("B_PACKAGE_DESC");
			}
		}
		private string crtuser;
		public string CRT_STAFF
		{
			get { return crtuser; }
			set
			{
				crtuser = value;
				RaisePropertyChanged("CRT_STAFF");
			}
		}
		private DateTime? crtdate;
		public DateTime? CRT_DATE
		{
			get { return crtdate; }
			set
			{
				crtdate = value;
				RaisePropertyChanged("CRT_DATE");
			}
		}
		private string upduser;
		public string UPD_STAFF
		{
			get { return upduser; }
			set
			{
				upduser = value;
				RaisePropertyChanged("UPD_STAFF");
			}
		}
		private DateTime? upddate;
		public DateTime? UPD_DATE
		{
			get { return upddate; }
			set
			{
				upddate = value;
				RaisePropertyChanged("UPD_DATE");
			}
		}
		#endregion

		#region 新增模式－商品搜尋用欄位
		private string _aitemcode;
		public string A_ITEM_CODE
		{
			get { return _aitemcode; }
			set
			{
				
				var item = StringHelper.ClearUpDataServices(value??"");
				if (item.Length != (value??"").Length)
					A_ITEM_CODE = item;
				else
				{
					_aitemcode = value;
					if(value!=null)
					GetBundleSerialNo();
					RaisePropertyChanged("A_ITEM_CODE");
				}
			}
		}
		private string _aitemname;
		public string A_ITEM_NAME
		{
			get { return _aitemname; }
			set
			{
				_aitemname = value;
				RaisePropertyChanged("A_ITEM_NAME");
			}
		}
		private string _aitemsize;
		public string A_ITEM_SIZE
		{
			get { return _aitemsize; }
			set
			{
				_aitemsize = value;
				RaisePropertyChanged("A_ITEM_SIZE");
			}
		}
		private string _aitemspec;
		public string A_ITEM_SPEC
		{
			get { return _aitemspec; }
			set
			{
				_aitemspec = value;
				RaisePropertyChanged("A_ITEM_SPEC");
			}
		}
		private string _aitemcolor;
		public string A_ITEM_COLOR
		{
			get { return _aitemcolor; }
			set
			{
				_aitemcolor = value;
				RaisePropertyChanged("A_ITEM_COLOR");
			}
		}

		private string _aitemserialno;
		public string A_ITEM_BUNDLE_SERIALNO
		{
			get { return _aitemserialno; }
			set
			{
				_aitemserialno = value;
				RaisePropertyChanged("A_ITEM_BUNDLE_SERIALNO");
			}
		}

		private short _aitemcombinorder;
		public short A_ITEM_COMBIN_ORDERE
		{
			get { return _aitemcombinorder; }
			set
			{
				_aitemcombinorder = value;
				RaisePropertyChanged("A_ITEM_COMBIN_ORDERE");
			}
		}

		private int _aitembomqty;
		public int A_ITEM_BOM_QTY
		{
			get { return _aitembomqty; }
			set
			{
				_aitembomqty = value;
				RaisePropertyChanged("A_ITEM_BOM_QTY");
			}
		}

		#endregion

		private void GetBundleSerialNo()
		{
			// F1903 C ON C.GUP_CODE=A.GUP_CODE AND C.ITEM_CODE=A.MATERIAL_CODE
		
			if (A_ITEM_CODE.Length >= 6)
			{
				
				var proxy19 = GetProxy<F19Entities>();
				var f1903 = proxy19.F1903s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.ITEM_CODE == A_ITEM_CODE).FirstOrDefault();
				if (f1903 != null)
					A_ITEM_BUNDLE_SERIALNO = f1903.BUNDLE_SERIALNO;
				else
					A_ITEM_BUNDLE_SERIALNO = "0";
			}
		}

		private void SetBomData()
		{
			if (SELECTED_ITEM != null && SELECTED_ITEM.ITEM_CODE != null && SELECTED_ITEM.ITEM_CODE != "")
			{
				NEW_ITEM = AutoMapper.Mapper.DynamicMap<F910101Ex>(SELECTED_ITEM);
				//NEW_ITEM = new F910101Ex()
				//{
				//	ITEM_CODE = SELECTED_ITEM.ITEM_CODE,
				//	STATUS = SELECTED_ITEM.STATUS,
				//	ITEM_NAME = SELECTED_ITEM.ITEM_NAME,
				//	BOM_TYPE = SELECTED_ITEM.BOM_TYPE,
				//	BOM_NAME = SELECTED_ITEM.BOM_NAME,
				//	UNIT_ID = SELECTED_ITEM.UNIT_ID,
				//	CHECK_PERCENT = SELECTED_ITEM.CHECK_PERCENT,
				//	SPEC_DESC = SELECTED_ITEM.SPEC_DESC,
				//	PACKAGE_DESC = SELECTED_ITEM.PACKAGE_DESC,
				//	CRT_STAFF = SELECTED_ITEM.CRT_STAFF,
				//	CRT_DATE = SELECTED_ITEM.CRT_DATE,
				//	UPD_STAFF = SELECTED_ITEM.UPD_STAFF,
				//	UPD_DATE = SELECTED_ITEM.UPD_DATE
				//};

				P91ExDataSource proxyEx = GetExProxy<P91ExDataSource>();
				dgList3 = proxyEx.CreateQuery<F910102Ex>("GetF910102Datas")
															.AddQueryExOption("gupCode", SELECTED_ITEM.GUP_CODE)
															.AddQueryExOption("custCode", SELECTED_ITEM.CUST_CODE)
															.AddQueryExOption("bomNo", SELECTED_ITEM.BOM_NO)
															.ToObservableCollection();
			}
		}

		private void ClearBomData()
		{
			//NEW_ITEM = null;
			//NEW_ITEM = new F910101Ex()
			//{
			//	ITEM_CODE = "",
			//	STATUS = null,
			//	ITEM_NAME = "",
			//	BOM_TYPE = null,
			//	BOM_NAME = "",
			//	UNIT_ID = null,
			//	CHECK_PERCENT = 0,
			//	SPEC_DESC = "",
			//	PACKAGE_DESC = ""
			//};
		}

		private void SetNewItemDetail()
		{
			if (SELECTED_ITEM3 != null)
			{
				A_ITEM_CODE = SELECTED_ITEM3.MATERIAL_CODE;
				A_ITEM_NAME = SELECTED_ITEM3.MATERIAL_NAME;
				A_ITEM_SIZE = SELECTED_ITEM3.ITEM_SIZE;
				A_ITEM_SPEC = SELECTED_ITEM3.ITEM_SPEC;
				A_ITEM_COLOR = SELECTED_ITEM3.ITEM_COLOR;
				A_ITEM_BUNDLE_SERIALNO = SELECTED_ITEM3.BUNDLE_SERIALNO;
				A_ITEM_COMBIN_ORDERE = SELECTED_ITEM3.COMBIN_ORDER;
				A_ITEM_BOM_QTY = SELECTED_ITEM3.BOM_QTY;
			}
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o => DoSearchCompleted()
					);
			}
		}

		private void DoSearch()
		{
			//dgList = new ObservableCollection<F910101Ex>();
			//dgList3 = new ObservableCollection<F910102Ex>();
			//SELECTED_ITEM = new F910101Ex();
			//SELECTED_ITEM3 = new F910102Ex();
			dgList3 = null;
			ClearBomData();
			P91ExDataSource proxyEx = GetExProxy<P91ExDataSource>();
			dgList = proxyEx.CreateQuery<F910101Ex>("GetF910101Datas")
														.AddQueryExOption("gupCode", _gupCode)
														.AddQueryExOption("custCode", _custCode)
														.AddQueryExOption("bomNo", SearchBomNo)
														.AddQueryExOption("itemCode", S_ITEM_CODE)
														.AddQueryExOption("status", S_STATUS)
														.AddQueryExOption("bomType", S_BOM_TYPE).ToObservableCollection();

			SearchResultIsExpanded = true;
		}

		private void DoSearchCompleted()
		{
			if (dgList.Count > 0)
			{
				SELECTED_ITEM = dgList.FirstOrDefault();
				SearchAction();
			}
			else
			{
				NEW_ITEM = null;
				DialogService.ShowMessage(Resources.Resources.InfoNoData);
			}

		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				o => DoAdd(),
					() => UserOperateMode == OperateMode.Query,
				o => DoAddCompleted()
				);
			}
		}

		private void DoAdd()
		{
			NEW_ITEM = new F910101Ex()
			{
				STATUS = "0",
				ISPROCESS = "0"
			};
			dgList3 = null;
		}

		private void DoAddCompleted()
		{
			UserOperateMode = OperateMode.Add;
			AddAction();
			//dgList = new ObservableCollection<F910101Ex>();
			//dgList2 = new ObservableCollection<F910102Ex>();
			//SELECTED_ITEM = new F910101Ex();
			//SELECTED_ITEM2 = new F910102Ex();
		}
		#endregion Add

		#region Edit

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && dgList != null && dgList.Count > 0 && dgList.FirstOrDefault().STATUS != "9",
					o => DoEditCompleted()
					);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
		}

		private void DoEditCompleted()
		{
			EditAction();
			UserOperateMode = OperateMode.Edit;
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() => UserOperateMode == OperateMode.Add || UserOperateMode == OperateMode.Edit,
					o => DoCancelCompleted()
					);
			}
		}
		private void DoCancel()
		{

		}
		private void DoCancelCompleted()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				//if (dgList != null)
				//	dgList.Clear();
				//if (dgList3 != null)
				//	dgList3.Clear();
				NEW_ITEM = null;
				dgList3 = null;
				SELECTED_ITEM = SELECTED_ITEM;

				ClearBomData();
				ClearSearchItemField();
				S_ITEM_CODE = "";
				S_STATUS = "0";
				//NEW_ITEM = null;
				UserOperateMode = OperateMode.Query;
				SearchAction();
			}
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query && dgList != null && dgList.Count > 0 && dgList.FirstOrDefault().STATUS != "9",
					o => DoDeleteCompleted()
					);
			}
		}

		private void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				P91ExDataSource proxyEx = GetExProxy<P91ExDataSource>();
				var result = proxyEx.CreateQuery<ExecuteResult>("DeleteF910101")
				.AddQueryExOption("gupCode", SELECTED_ITEM.GUP_CODE)
				.AddQueryExOption("custCode", SELECTED_ITEM.CUST_CODE)
				.AddQueryExOption("bomNo", SELECTED_ITEM.BOM_NO)
				.AddQueryExOption("userId", _userId).ToList();
				if (result.Any())
				{
					var item = result.First();
					if (item.IsSuccessed)
						ShowMessage(Messages.DeleteSuccess);
					else
						ShowResultMessage(item);
				}
				else
				{
					DialogService.ShowMessage(Properties.Resources.P9103010000_ViewModel_ActionNoResponse);
				}

				SELECTED_ITEM = null;
				DoSearch();
			}
		}
		private void DoDeleteCompleted()
		{
			//dgList.Clear();
			//dgList3.Clear();
			//S_ITEM_CODE = "";
			//S_STATUS = "0";
			//ClearBomData();
			ClearSearchItemField();

			UserOperateMode = OperateMode.Query;
			SearchAction();
		}
		#endregion Delete

		#region Save

		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						_isSave = false;

						ExDataMapper.Trim(NEW_ITEM);
						var error = GetEditableError(NEW_ITEM);
						if (!string.IsNullOrEmpty(error))
						{
							DialogService.ShowMessage(error);
							return;
						}

						if (ShowMessage(Messages.WarningBeforeUpdate) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						DoSave();
					},
					() => (UserOperateMode == OperateMode.Add && dgList3 != null && dgList3.Count > 0) || (UserOperateMode == OperateMode.Edit && SELECTED_ITEM != null),
					o => DoSaveCompleted()
					);
			}
		}

		private void DoSave()
		{
			_isSave = true;
			var f910101Ex = new F910101Ex()
			{
				BOM_NO = NEW_ITEM.BOM_NO,
				ITEM_CODE = NEW_ITEM.ITEM_CODE,
				ITEM_NAME = NEW_ITEM.ITEM_NAME,
				BOM_TYPE = NEW_ITEM.BOM_TYPE,
				BOM_NAME = NEW_ITEM.BOM_NAME,
				UNIT_ID = NEW_ITEM.UNIT_ID,
				CHECK_PERCENT = NEW_ITEM.CHECK_PERCENT,
				SPEC_DESC = NEW_ITEM.SPEC_DESC,
				PACKAGE_DESC = NEW_ITEM.PACKAGE_DESC,
				STATUS = NEW_ITEM.STATUS,
				ISPROCESS = NEW_ITEM.ISPROCESS,
				CUST_CODE = _custCode,
				GUP_CODE = _gupCode,
				CRT_STAFF = _userId,
				CRT_DATE = DateTime.Now,
				CRT_NAME = _userId
			};
			var proxy = new wcf.P91WcfServiceClient();
			if (UserOperateMode == OperateMode.Add)
			{
				var wcfF910101Ex = ExDataMapper.Map<F910101Ex, wcf.F910101Ex>(f910101Ex);
				var wcfF910102Ex = ExDataMapper.MapCollection<F910102Ex, wcf.F910102Ex>(dgList3).ToArray();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertF910101(wcfF910101Ex, wcfF910102Ex, _userId));

				ShowResultMessage(result);
			}
			else
			{
				var wcfF910101Ex = ExDataMapper.Map<F910101Ex, wcf.F910101Ex>(f910101Ex);
				var wcfF910102Ex = ExDataMapper.MapCollection<F910102Ex, wcf.F910102Ex>(dgList3).ToArray();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateF910101(wcfF910101Ex, wcfF910102Ex, _userId));
				ShowResultMessage(result);
			}

			_lastF910101Ex = ExDataMapper.Clone(NEW_ITEM);
		}

		string GetEditableError(F910101Ex f910101Ex)
		{
			if (string.IsNullOrWhiteSpace(f910101Ex.BOM_NO))
				return Properties.Resources.P9103010000_ViewModel_CombineNo_Required;

			if (!ValidateHelper.IsMatchAZaz09(f910101Ex.BOM_NO))
				return Properties.Resources.P9103010000_ViewModel_CombineNo_CNWordOnly;

			if (string.IsNullOrWhiteSpace(f910101Ex.ITEM_CODE))
				return Properties.Resources.P9103010000_ViewModel_ProductNo_Required;

			if (!ValidateHelper.IsMatchAZaz09Dash(f910101Ex.ITEM_CODE))
				return Properties.Resources.P9103010000_ViewModel_ProductNo_CNWordOnly;

			if (f910101Ex.CHECK_PERCENT > 100 || f910101Ex.CHECK_PERCENT < 0)
				return Properties.Resources.P9103010000_ViewModel_ProcessTest;


			if (NEW_ITEM.ITEM_CODE == null || NEW_ITEM.ITEM_CODE == "")
			{
				return Properties.Resources.P9103010000_ViewModel_InputProductNo;
			}


			var proxy19 = GetProxy<F19Entities>();
			var f1903 = proxy19.F1903s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.ITEM_CODE == NEW_ITEM.ITEM_CODE).FirstOrDefault();
			if (f1903 == null)
			{
				return Properties.Resources.P9103010000_ViewModel_ReInputProductNo;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				var proxy91 = GetProxy<F91Entities>();
				var f910101 = proxy91.F910101s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.BOM_NO == NEW_ITEM.BOM_NO).FirstOrDefault();
				if (f910101 != null)
				{
					return Properties.Resources.P9103010000_ViewModel_CombinNoExist;
				}
			}


			return string.Empty;
		}

		F910101Ex _lastF910101Ex = null;

		private void DoSaveCompleted()
		{
			if (_isSave == true)
			{
				SearchBomNo = string.Empty;
				if (_lastF910101Ex != null)
					SearchBomNo = _lastF910101Ex.BOM_NO;

				S_ITEM_CODE = string.Empty;
				S_STATUS = "0";

				if (UserOperateMode == OperateMode.Add)
				{
					if (dgList3 != null)
					{
						dgList3.Clear();
						ClearBomData();
						ClearSearchItemField();
						UserOperateMode = OperateMode.Query;
						SearchAction();
						DoSearch();
					}
				}
				else
				{
					dgList.Clear();
					dgList3.Clear();
					ClearBomData();
					ClearSearchItemField();
					UserOperateMode = OperateMode.Query;
					DoSearch();

				}

				NEW_ITEM = null;
				if (_lastF910101Ex != null)
					SELECTED_ITEM = dgList.Where(item => item.BOM_NO == _lastF910101Ex.BOM_NO).FirstOrDefault();
			}
		}
		#endregion Save


		#region AddItemCommand
		public ICommand AddItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoNothing(),
					() => UserOperateMode != OperateMode.Query && NEW_ITEM != null && NEW_ITEM.ITEM_CODE != null && !string.IsNullOrEmpty(A_ITEM_CODE),
					o => DoNothing(),
					null,
					() => DoAddItem()
					);
			}
		}

		private void DoNothing()
		{

		}

		string GetDetailError(bool isEditable)
		{
			if (A_ITEM_CODE == null)
			{
				return Properties.Resources.P9103010000_ViewModel_InputCombineNo;

			}


			var itemRepeatChcekQuery = dgList3.Where(x => x.MATERIAL_CODE.Equals(A_ITEM_CODE));

			if (isEditable)
			{
				itemRepeatChcekQuery = itemRepeatChcekQuery.Where(x => x != SELECTED_ITEM3);
			}

			if (itemRepeatChcekQuery.Any())
			{
				return Properties.Resources.P9103010000_ViewModel_CombineNoDuplicate;
			}

			if (A_ITEM_NAME == null || A_ITEM_NAME == "")
			{
				return Properties.Resources.P9103010000_ViewModel_CombineNoNotExist;

			}
			if (A_ITEM_COMBIN_ORDERE <= 0)
			{
				return Properties.Resources.P9103010000_ViewModel_CombineOrderGreaterThan0;
			}

			var orderRepeatChcekQuery = dgList3.Where(x => x.COMBIN_ORDER == A_ITEM_COMBIN_ORDERE);

			if (isEditable)
			{
				orderRepeatChcekQuery = orderRepeatChcekQuery.Where(x => x != SELECTED_ITEM3);
			}

			if (orderRepeatChcekQuery.Any())
			{
				return Properties.Resources.P9103010000_ViewModel_CombineOrderDuplicate;
			}

			if (A_ITEM_BOM_QTY <= 0)
			{
				return Properties.Resources.P9103010000_ViewModel_QtyMoreThan0;

			}

			return string.Empty;
		}

		private void DoAddItem()
		{
			if (dgList3 == null)
				dgList3 = new ObservableCollection<F910102Ex>();

			var error = GetDetailError(isEditable: false);
			if (!string.IsNullOrEmpty(error))
			{
				DialogService.ShowMessage(error);
				return;
			}

			F910102Ex f910102 = new F910102Ex()
			{
				BOM_QTY = A_ITEM_BOM_QTY,
				BUNDLE_SERIALNO = A_ITEM_BUNDLE_SERIALNO,
				ITEM_SPEC = A_ITEM_SPEC,
				COMBIN_ORDER = A_ITEM_COMBIN_ORDERE,
				CRT_DATE = DateTime.Now,
				CRT_NAME = _userName,
				CRT_STAFF = _userId,
				CUST_CODE = _custCode,
				GUP_CODE = _gupCode,
				BOM_NO = NEW_ITEM.BOM_NO,//成品編號
				ITEM_COLOR = A_ITEM_COLOR,
				ITEM_SIZE = A_ITEM_SIZE,
				MATERIAL_CODE = A_ITEM_CODE,//組合商品編號
				MATERIAL_NAME = A_ITEM_NAME,
			};

			dgList3.Add(f910102);
			RaisePropertyChanged("dgList3");

			ClearSearchItemField();

		}
		#endregion AddItemCommand

		private void ClearSearchItemField()
		{
			A_ITEM_BOM_QTY = 0;
			A_ITEM_BUNDLE_SERIALNO = "0";
			A_ITEM_SPEC = "";
			A_ITEM_COMBIN_ORDERE = 0;
			A_ITEM_COLOR = "";
			A_ITEM_SIZE = "";
			A_ITEM_CODE = "";//組合商品編號
			A_ITEM_NAME = "";
		}

		#region DeleteItemCommand
		public ICommand DeleteItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoNothing(),
					() => UserOperateMode != OperateMode.Query && SELECTED_ITEM3 != null,
					o => DoNothing(),
					null,
					() => DoDeleteItem()
					);
			}
		}


		private void DoDeleteItem()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				if (SELECTED_ITEM3 == null)
				{
					DialogService.ShowMessage(Properties.Resources.P9103010000_ViewModel_SelectDeleteCombineItem);
					return;
				}
				var f919102 = dgList3.Where(x => x.MATERIAL_CODE == SELECTED_ITEM3.MATERIAL_CODE).FirstOrDefault();
				dgList3.Remove(f919102);
				ShowMessage(Messages.InfoDeleteSuccess);
				RaisePropertyChanged("dgList3");
			}
		}
		#endregion AddItemCommand

		#region EditItemCommand
		public ICommand EditItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoNothing(),
					() => UserOperateMode != OperateMode.Query && SELECTED_ITEM3 != null,
					o => DoNothing(),
					null,
					() => DoEditItem()
					);
			}
		}


		private void DoEditItem()
		{
			var error = GetDetailError(true);
			if (!string.IsNullOrEmpty(error))
			{
				DialogService.ShowMessage(error);
				return;
			}

			if (ShowMessage(Messages.WarningBeforeComfirmEditItem) == DialogResponse.Yes)
			{
				var f919102 = dgList3.Where(x => x.MATERIAL_CODE == SELECTED_ITEM3.MATERIAL_CODE).FirstOrDefault();
				f919102.MATERIAL_CODE = A_ITEM_CODE;
				f919102.MATERIAL_NAME = A_ITEM_NAME;
				f919102.ITEM_SIZE = A_ITEM_SIZE;
				f919102.ITEM_SPEC = A_ITEM_SPEC;
				f919102.ITEM_COLOR = A_ITEM_COLOR;
				f919102.BUNDLE_SERIALNO = A_ITEM_BUNDLE_SERIALNO;
				f919102.COMBIN_ORDER = A_ITEM_COMBIN_ORDERE;
				f919102.BOM_QTY = A_ITEM_BOM_QTY;
				RaisePropertyChanged("dgList3");
			}
		}
		#endregion AddItemCommand

		public string GetItemName(string strItemCode)
		{
			var proxy19 = GetProxy<F19Entities>();
			var f1903 = proxy19.F1903s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.ITEM_CODE == strItemCode).FirstOrDefault();
			if (f1903 == null)
			{
				ShowWarningMessage(Properties.Resources.P9103010000_ViewModel_ReInputProductNo);
				return string.Empty;
			}
			else
			{
                return f1903.ITEM_NAME;
            }
		}

	}
}
