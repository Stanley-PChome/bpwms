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
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P06WcfService;
using Wms3pl.WpfClient.Services;
using System.Windows;

namespace Wms3pl.WpfClient.P06.ViewModel
{
	public partial class P0602010000_ViewModel : InputViewModelBase
	{
		private readonly F05Entities _proxy;
		private P06ExDataSource proxyEx;
		private readonly F19Entities _proxyDW;
		private string _userId;
		private string _userName;
		private string _custCode;
		private string _gupCode;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };
		public Action GetCurrentTab = delegate { };
		public Action SetCurrentTab = delegate { };
		public Action CollapsedAction = delegate { };
		public Action CancelAction = delegate { };
		public Action GridReadOnlylAction = delegate { };
		public bool IsSave = true;
		public bool IsDelete = true;
		public bool showSaveBtn = false;
		public P0602010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//連接DB物件
				_proxy = GetProxy<F05Entities>();
				proxyEx = GetExProxy<P06ExDataSource>();

				_proxyDW = GetProxy<F19Entities>();
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;//物流中心
				//下拉清單
				REASONList = _proxyDW.F1951s.Where(x => x.UCT_ID.Equals("PK"))
					.OrderBy(o => o.UCC_CODE)
					.ToList();//缺貨原因-揀貨

				REASONList_Allot = _proxyDW.F1951s.Where(x => x.UCT_ID.Equals("MV"))
		.OrderBy(o => o.UCC_CODE)
		.ToList();//缺貨原因-揀貨
				InitControls();
			}
		}

		private void InitControls()
		{
			cbWorkType = 0;

			dpDELV_DATE_Start = DateTime.Today;
			dpDELV_DATE_End = DateTime.Today;
			STATUSList = GetBaseTableService.GetF000904List(FunctionCode, "F051206", "STATUS", true);
	
			if (STATUSList.Any())
				cbSTATUS = STATUSList.First().Value;

			RETURN_FLAGByGridList = GetBaseTableService.GetF000904List(FunctionCode, "F051206", "RETURN_FLAG", false).ToList();


			RETURN_FLAGList = GetBaseTableService.GetF000904List(FunctionCode, "F051206", "RETURN_FLAG", false).Where(x=> x.Value != "4" && x.Value != "5" && x.Value != "2").ToList();
			RETURN_FLAGList.Insert(0, new NameValuePair<string> { Name = "", Value = "" });
			if (RETURN_FLAGList.Any())
				cbRETURN_FLAG = RETURN_FLAGList.First().Value;

			RETURN_FLAGList_ALLOT = GetBaseTableService.GetF000904List(FunctionCode, "F151003", "RETURN_FLAG", false);
			if (RETURN_FLAGList_ALLOT.Any())
				cbRETURN_FLAG_ALLOT = RETURN_FLAGList_ALLOT.First().Value;

			STATUSList_ALLOT = GetBaseTableService.GetF000904List(FunctionCode, "F151003", "STATUS", true);
			if (STATUSList_ALLOT.Any())
				cbSTATUS_ALLOT = STATUSList_ALLOT.First().Value;
			SHOWQUERYPAGE = "Visible";
			SHOWADDPAGE = "Collapsed";
		}


		//public ObservableCollection<NameValuePair<string>> GetPICK_TIMEList()
		//{
		//	F0513List = _proxy.F0513s.Where(x => x.GUP_CODE.Equals(_gupCode))
		//	.Where(x => x.CUST_CODE.Equals(_custCode))
		//	.Where(x => x.DC_CODE.Equals(cbDC_CODE))
		//	.Where(x => x.DELV_DATE.Equals(dpDELV_DATE))
		//	.ToList();//出貨批次紀錄檔
		//	return (from o in
		//				(from o in F0513List
		//				 select new { o.PICK_TIME }).Distinct()
		//			orderby o.PICK_TIME
		//			select new NameValuePair<string>()
		//			{
		//				Name = o.PICK_TIME,
		//				Value = o.PICK_TIME
		//			}).AsQueryable().ToObservableCollection();
		//}

		private string _currentTab;
		public string CURRENT_TAB
		{
			get { return _currentTab; }
			set
			{
				_currentTab = value;
			}
		}

		private string _showquerypage;
		public string SHOWQUERYPAGE
		{
			get { return _showquerypage; }
			set
			{
				_showquerypage = value;
				RaisePropertyChanged("SHOWQUERYPAGE");
			}
		}
		private string _showaddpage;
		public string SHOWADDPAGE
		{
			get { return _showaddpage; }
			set
			{
				_showaddpage = value;
				RaisePropertyChanged("SHOWADDPAGE");
			}
		}

		#region 物流中心
		private string _cbDC_CODE;
		public string cbDC_CODE
		{
			get { return _cbDC_CODE; }
			set
			{
				_cbDC_CODE = value;
				//PICK_TIMEList = GetPICK_TIMEList();
				//if (PICK_TIMEList.Any())
				//	cbPICK_TIME = PICK_TIMEList.First().Value;
				RaisePropertyChanged("cbDC_CODE");
			}
		}
		#region 列表
		private List<NameValuePair<string>> _DcList;
		public List<NameValuePair<string>> DcList
		{
			get { return _DcList; }
			set
			{
				_DcList = value;
				RaisePropertyChanged("DcList");
			}
		}
		#endregion
		#endregion

		#region 批次日期(起)
		private DateTime _dpDELV_DATE_Start;
		public DateTime dpDELV_DATE_Start
		{
			get { return _dpDELV_DATE_Start; }
			set
			{
				_dpDELV_DATE_Start = value;
				//PICK_TIMEList = GetPICK_TIMEList();
				//if (PICK_TIMEList.Any())
				//	cbPICK_TIME = PICK_TIMEList.First().Value;
				RaisePropertyChanged("dpDELV_DATE_Start");
			}
		}
		#endregion

		#region 批次日期(迄)
		private DateTime _dpDELV_DATE_End;
		public DateTime dpDELV_DATE_End
		{
			get { return _dpDELV_DATE_End; }
			set
			{
				_dpDELV_DATE_End = value;
				//PICK_TIMEList = GetPICK_TIMEList();
				//if (PICK_TIMEList.Any())
				//	cbPICK_TIME = PICK_TIMEList.First().Value;
				RaisePropertyChanged("dpDELV_DATE_End");
			}
		}
		#endregion

		#region 批次時段
		private string _cbPICK_TIME;
		public string cbPICK_TIME
		{
			get { return _cbPICK_TIME; }
			set
			{
				_cbPICK_TIME = value;
				RaisePropertyChanged("cbPICK_TIME");
			}
		}
		#region 列表
		private List<F0513> _F0513List;
		public List<F0513> F0513List
		{
			get { return _F0513List; }
			set
			{
				_F0513List = value;
				RaisePropertyChanged("F0513List");
			}
		}
		private ObservableCollection<NameValuePair<string>> _PICK_TIMEList;
		public ObservableCollection<NameValuePair<string>> PICK_TIMEList
		{
			get { return _PICK_TIMEList; }
			set
			{
				_PICK_TIMEList = value;
				RaisePropertyChanged("PICK_TIMEList");
			}
		}
		#endregion
		#endregion

		#region 處理狀態
		private string _cbSTATUS;
		public string cbSTATUS
		{
			get { return _cbSTATUS; }
			set
			{
				_cbSTATUS = value;
				RaisePropertyChanged("cbSTATUS");
			}
		}
		#region 列表
		private List<NameValuePair<string>> _STATUSList;
		public List<NameValuePair<string>> STATUSList
		{
			get { return _STATUSList; }
			set
			{
				_STATUSList = value;
				RaisePropertyChanged("STATUSList");
			}
		}


		#endregion
		#endregion

		#region 揀貨單號
		private string _tbPICK_ORD_NO;
		public string tbPICK_ORD_NO
		{
			get { return _tbPICK_ORD_NO; }
			set
			{
				_tbPICK_ORD_NO = value;
				RaisePropertyChanged("tbPICK_ORD_NO");
			}
		}
		#endregion

		#region 是否全選
		private bool _isCheckAll;
		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				RaisePropertyChanged("IsCheckAll");
			}
		}

		private bool _isCheckAll2;
		public bool IsCheckAll2
		{
			get { return _isCheckAll2; }
			set
			{
				_isCheckAll2 = value;
				RaisePropertyChanged("IsCheckAll2");
			}
		}

		private bool _isCheckAll3;
		public bool IsCheckAll3
		{
			get { return _isCheckAll3; }
			set
			{
				_isCheckAll3 = value;
				RaisePropertyChanged("IsCheckAll3");
			}
		}

		private bool _isCheckAll4;
		public bool IsCheckAll4
		{
			get { return _isCheckAll4; }
			set
			{
				_isCheckAll4 = value;
				RaisePropertyChanged("IsCheckAll4");
			}
		}
		#endregion

		#region Grid 全選-查詢模式
		private RelayCommand<string> _checkSelectedAllCommand;

		/// <summary>
		/// Gets the CheckSelectedAllCommand.
		/// </summary>
		public RelayCommand<string> CheckSelectedAllCommand
		{
			get
			{
				return _checkSelectedAllCommand
					?? (_checkSelectedAllCommand = new RelayCommand<string>(CheckSelectedAll));
			}
		}

		void CheckSelectedAll(string whichGridData)
		{
			if (whichGridData == "PICK_Q")
			{
				foreach (var lackItem in LackList)
				{
					lackItem.ISSELECTED = IsCheckAll;
				}
			}
			else if (whichGridData == "PICK_A")
			{
				foreach (var lackItem in LackList2)
				{
					lackItem.ISSELECTED = IsCheckAll2;
				}
			}
			else if (whichGridData == "ALLOT_Q")
			{
				foreach (var lackItem in LackList_Allot)
				{
					lackItem.ISSELECTED = IsCheckAll3;
				}
			}
			else if (whichGridData == "ALLOT_A")
			{
				foreach (var lackItem in LackList2_Allot)
				{
					lackItem.ISSELECTED = IsCheckAll4;
				}
			}
		}

		#endregion

		#region 作業類型
		private int _cbWorkType;
		public int cbWorkType
		{
			get { return _cbWorkType; }
			set
			{
				_cbWorkType = value;
				SetCurrentTab();
				RaisePropertyChanged("cbWorkType");
			}
		}

		#endregion

		#region 缺貨處理結果(1:缺品出貨2取消訂單)
		private string _cbRETURN_FLAG;
		public string cbRETURN_FLAG
		{
			get { return _cbRETURN_FLAG; }
			set
			{
				_cbRETURN_FLAG = value;
				RaisePropertyChanged("cbRETURN_FLAG");
			}
		}
		private string _cbRETURN_FLAG_ALLOT;
		public string cbRETURN_FLAG_ALLOT
		{
			get { return _cbRETURN_FLAG_ALLOT; }
			set
			{
				_cbRETURN_FLAG_ALLOT = value;
				RaisePropertyChanged("cbRETURN_FLAG_ALLOT");
			}
		}

		#region 列表

		private List<NameValuePair<string>> _RETURN_FLAGByGridList;
		public List<NameValuePair<string>> RETURN_FLAGByGridList
		{
			get { return _RETURN_FLAGByGridList; }
			set
			{
				_RETURN_FLAGByGridList = value;
				RaisePropertyChanged("RETURN_FLAGByGridList");
			}
		}

		private List<NameValuePair<string>> _RETURN_FLAGList;
		public List<NameValuePair<string>> RETURN_FLAGList
		{
			get { return _RETURN_FLAGList; }
			set
			{
				_RETURN_FLAGList = value;
				RaisePropertyChanged("RETURN_FLAGList");
			}
		}

		private List<NameValuePair<string>> _RETURN_FLAGList_ALLOT;
		public List<NameValuePair<string>> RETURN_FLAGList_ALLOT
		{
			get { return _RETURN_FLAGList_ALLOT; }
			set
			{
				_RETURN_FLAGList_ALLOT = value;
				RaisePropertyChanged("RETURN_FLAGList_ALLOT");
			}
		}

		#endregion
		#endregion

		#region 缺貨原因(F1951)
		private string _cbREASON;
		public string cbREASON
		{
			get { return _cbREASON; }
			set
			{
				_cbREASON = value;
				RaisePropertyChanged("cbREASON");
			}
		}
		#region 列表
		private List<F1951> _REASONList;
		public List<F1951> REASONList
		{
			get { return _REASONList; }
			set
			{
				_REASONList = value;
				RaisePropertyChanged("REASONList");
			}
		}

		private List<F1951> _REASONList_Allot;
		public List<F1951> REASONList_Allot
		{
			get { return _REASONList_Allot; }
			set
			{
				_REASONList_Allot = value;
				RaisePropertyChanged("REASONList_Allot");
			}
		}
		#endregion
		#region 查詢結果揀貨列表
		private ObservableCollection<F051206Pick> _PickList;
		public ObservableCollection<F051206Pick> PickList
		{
			get { return _PickList; }
			set
			{
				_PickList = value;
				RaisePropertyChanged("PickList");
			}
		}
		private F051206Pick _PickSelected;
		public F051206Pick PickSelected
		{
			get { return _PickSelected; }
			set
			{
				_PickSelected = value;
				SetLackList();
				CollapsedAction();
				GridReadOnlylAction();
				//SHOWQUERYPAGE = "Visible";
				ShowModifyQtyOutOfStockBtn = (_PickSelected != null && _PickSelected.STATUS == "1" && LackSelected != null) ? Visibility.Visible : Visibility.Hidden;
				if (UserOperateMode == OperateMode.Edit)
					UserOperateMode = OperateMode.Query;
				RaisePropertyChanged("PickSelected");
			}
		}

		private ObservableCollection<F051206Pick> _PickList2;
		public ObservableCollection<F051206Pick> PickList2
		{
			get { return _PickList2; }
			set
			{
				_PickList2 = value;
				RaisePropertyChanged("PickList2");
			}
		}
		private F051206Pick _PickSelected2;
		public F051206Pick PickSelected2
		{
			get { return _PickSelected2; }
			set
			{
				_PickSelected2 = value;
				SetLackList();
				CollapsedAction();
				RaisePropertyChanged("PickSelected2");
			}
		}
		public void SetLackList()
		{
			//if (PickList.Any())
			//{
			proxyEx = GetExProxy<P06ExDataSource>();
			if (SHOWADDPAGE == "Visible")
			{
				if (PickSelected2 != null)
				{
					LackList2 = proxyEx.CreateQuery<F051206LackList>("GetF051206LackLists")
						        .AddQueryExOption("dcCode",PickSelected2.DC_CODE)
										.AddQueryExOption("gupCode",PickSelected2.GUP_CODE)
										.AddQueryExOption("custCode",PickSelected2.CUST_CODE)
										.AddQueryExOption("pickOrdNo", PickSelected2.PICK_ORD_NO)
										.AddQueryExOption("wmsOrdNo", PickSelected2.WMS_ORD_NO)
										.AddQueryExOption("editType", "ADD")
										.AsQueryable().ToObservableCollection();
					if (LackList2.Any())
					{
						CUST_ORD_NO = LackList2.FirstOrDefault().CUST_ORD_NO;
						ORD_NO = LackList2.FirstOrDefault().ORD_NO;
                    }
				}
			}

			else
			{
				if (PickSelected != null)
				{
					//LackList = proxyEx.CreateQuery<F051206LackList>("GetF051206LackLists")
					//        .AddQueryExOption("PICK_ORD_NO", PickSelected.PICK_ORD_NO))
					//        .AddQueryExOption("WMS_ORD_NO", PickSelected.WMS_ORD_NO))
					//        .AddQueryExOption("editType", "EDIT"))
					//        .AsQueryable().ToObservableCollection();
					LackList = proxyEx.CreateQuery<F051206LackList>("GetF051206LackLists")
					.AddQueryExOption("dcCode", PickSelected.DC_CODE)
					.AddQueryExOption("gupCode", PickSelected.GUP_CODE)
					.AddQueryExOption("custCode", PickSelected.CUST_CODE)
					.AddQueryExOption("pickOrdNo", PickSelected.PICK_ORD_NO)
					.AddQueryExOption("wmsOrdNo", PickSelected.WMS_ORD_NO)
					.AddQueryExOption("editType", "EDIT")
					.AsQueryable().Where(o => o.STATUS == PickSelected.STATUS).ToObservableCollection();
					if (LackList.Any())
					{
						CUST_ORD_NO = LackList.First().CUST_ORD_NO;
						ORD_NO = LackList.First().ORD_NO;
                    }
					RaisePropertyChanged("LackList");
				}
			}
            cbRETURN_FLAG = RETURN_FLAGList.Select(x => x.Value).FirstOrDefault();
            RaisePropertyChanged("CUST_ORD_NO");
			RaisePropertyChanged("ORD_NO");
			//}
		}
		#endregion
		#region 查詢結果調撥列表
		private ObservableCollection<F051206AllocationList> _PickList_Allot;
		public ObservableCollection<F051206AllocationList> PickList_Allot
		{
			get { return _PickList_Allot; }
			set
			{
				_PickList_Allot = value;
				RaisePropertyChanged("PickList_Allot");
			}
		}
		private F051206AllocationList _PickSelected_Allot;
		public F051206AllocationList PickSelected_Allot
		{
			get { return _PickSelected_Allot; }
			set
			{
				_PickSelected_Allot = value;
				UserOperateMode = OperateMode.Query;
				setLackList_Allot();
				//SHOWQUERYPAGE = "Visible";
				CollapsedAction();
				GridReadOnlylAction();
				RaisePropertyChanged("PickSelected_Allot");
			}
		}

		private ObservableCollection<F051206AllocationList> _PickList2_Allot;
		public ObservableCollection<F051206AllocationList> PickList2_Allot
		{
			get { return _PickList2_Allot; }
			set
			{
				_PickList2_Allot = value;
				RaisePropertyChanged("PickList2_Allot");
			}
		}
		private F051206AllocationList _PickSelected2_Allot;
		public F051206AllocationList PickSelected2_Allot
		{
			get { return _PickSelected2_Allot; }
			set
			{
				_PickSelected2_Allot = value;
				CollapsedAction();
				setLackList_Allot();
				RaisePropertyChanged("PickSelected2_Allot");
			}
		}

		private void setLackList_Allot()
		{
			//if (PickList.Any())
			//{
			LackList_Allot = new ObservableCollection<F051206LackList_Allot>();
			LackList2_Allot = new ObservableCollection<F051206LackList_Allot>();
			proxyEx = GetExProxy<P06ExDataSource>();
			if (SHOWADDPAGE == "Visible")
			{
				if (PickSelected2_Allot != null)
				{
					LackList2_Allot = proxyEx.CreateQuery<F051206LackList_Allot>("GetF051206LackLists_Allot")
										.AddQueryExOption("dcCode", cbDC_CODE_ALLOT)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
										.AddQueryExOption("allocationNo", PickSelected2_Allot.ALLOCATION_NO)
										.AddQueryExOption("editType", "ADD")
										.AddQueryExOption("status", PickSelected2_Allot.STATUS)
										.AsQueryable().ToObservableCollection();

					ALLOCATION_NO = PickSelected2_Allot.ALLOCATION_NO;
				}
			}
			else
			{
				if (PickSelected_Allot != null)
				{
					LackList_Allot = proxyEx.CreateQuery<F051206LackList_Allot>("GetF051206LackLists_Allot")
										.AddQueryExOption("dcCode", cbDC_CODE_ALLOT)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
										.AddQueryExOption("allocationNo", PickSelected_Allot.ALLOCATION_NO)
										.AddQueryExOption("editType", "QUERY")
										.AddQueryExOption("status", PickSelected_Allot.STATUS)
										.AsQueryable().ToObservableCollection();
					ALLOCATION_NO = PickSelected_Allot.ALLOCATION_NO;
				}
			}
			//RaisePropertyChanged("ALLOCATION_NO");
			//}
		}
		#endregion

		#region 查詢結果缺貨明細
		private ObservableCollection<F051206LackList> _LackList;
		public ObservableCollection<F051206LackList> LackList
		{
			get { return _LackList; }
			set
			{
				_LackList = value;
				if (_LackList != null && _LackList.Any())
				{
					LackSelected = _LackList.First();
				}
				else
				{
					LackSelected = null;
				}
				RaisePropertyChanged("LackList");
			}
		}

		private F051206LackList _LackSelected;
		public F051206LackList LackSelected
		{
			get { return _LackSelected; }
			set
			{
				_LackSelected = value;
				RaisePropertyChanged("LackSelected");
			}
		}

		private ObservableCollection<F051206LackList> _LackList2;
		public ObservableCollection<F051206LackList> LackList2
		{
			get { return _LackList2; }
			set
			{
				_LackList2 = value;
				RaisePropertyChanged("LackList2");
			}
		}

		private F051206LackList _LackSelected2;
		public F051206LackList LackSelected2
		{
			get { return _LackSelected2; }
			set
			{
				_LackSelected2 = value;
				RaisePropertyChanged("LackSelected2");
			}
		}

		private ObservableCollection<F051206LackList_Allot> _LackList_Allot;
		public ObservableCollection<F051206LackList_Allot> LackList_Allot
		{
			get { return _LackList_Allot; }
			set
			{
				_LackList_Allot = value;
				RaisePropertyChanged("LackList_Allot");
			}
		}

		private F051206LackList_Allot _LackSelected_Allot;
		public F051206LackList_Allot LackSelected_Allot
		{
			get { return _LackSelected_Allot; }
			set
			{
				_LackSelected_Allot = value;
				RaisePropertyChanged("LackSelected_Allot");
			}
		}

		private ObservableCollection<F051206LackList_Allot> _LackList2_Allot;
		public ObservableCollection<F051206LackList_Allot> LackList2_Allot
		{
			get { return _LackList2_Allot; }
			set
			{
				_LackList2_Allot = value;
				RaisePropertyChanged("LackList2_Allot");
			}
		}

		private F051206LackList_Allot _LackSelected2_Allot;
		public F051206LackList_Allot LackSelected2_Allot
		{
			get { return _LackSelected2_Allot; }
			set
			{
				_LackSelected2_Allot = value;
				RaisePropertyChanged("LackSelected2_Allot");
			}
		}
		#endregion

		#endregion

		#region 貨主單號
		private string _custordno;
		public string CUST_ORD_NO
		{
			get { return _custordno; }
			set
			{
				_custordno = value;
				RaisePropertyChanged("CUST_ORD_NO");
			}
		}
		#endregion

		#region 調發單號
		private string _allocationno;
		public string ALLOCATION_NO
		{
			get { return _allocationno; }
			set
			{
				_allocationno = value;
				RaisePropertyChanged("ALLOCATION_NO");
			}
		}
		#endregion

		#region 訂單編號
		private string _ordno;
		public string ORD_NO
		{
			get { return _ordno; }
			set
			{
				_ordno = value;
				RaisePropertyChanged("ORD_NO");
			}
		}
		#endregion

		#region 出貨單號
		private string _tbWMS_ORD_NO;
		public string tbWMS_ORD_NO
		{
			get { return _tbWMS_ORD_NO; }
			set
			{
				_tbWMS_ORD_NO = value;
				RaisePropertyChanged("tbWMS_ORD_NO");
			}
		}
		#endregion

		#region 容器編碼
		private string _tbCONTAINER_CODE;
		public string tbCONTAINER_CODE
		{
			get { return _tbCONTAINER_CODE; }
			set
			{
				_tbCONTAINER_CODE = value;
				RaisePropertyChanged("tbCONTAINER_CODE");
			}
		}
		#endregion

		#region 作業人員
		private string _tbCrtOrUpdOpertor;
		public string tbCrtOrUpdOpertor
		{
			get { return _tbCrtOrUpdOpertor; }
			set
			{
				_tbCrtOrUpdOpertor = value;
				RaisePropertyChanged("tbCrtOrUpdOpertor");
			}
		}

		#endregion


		private Visibility _showModifyQtyOutOfStockBtn = Visibility.Hidden;
		public Visibility ShowModifyQtyOutOfStockBtn
		{
			get { return _showModifyQtyOutOfStockBtn; }
			set { Set(ref _showModifyQtyOutOfStockBtn, value); }
		}


		#region
		private bool _lackQtyIsEnabled;

		public bool LackQtyIsEnabled
		{
			get { return _lackQtyIsEnabled; }
			set
			{
				if (_lackQtyIsEnabled == value)
					return;
				Set(() => LackQtyIsEnabled, ref _lackQtyIsEnabled, value);
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
					() => UserOperateMode != OperateMode.Edit,
					o => DoSearchCompleted(),
					null,
					() => GetCurrentTab()
					);
			}
		}

		public ICommand SearchCommand_Add
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Add,
					o => DoSearchCompleted(),
					null,
					() => GetCurrentTab()
					);
			}
		}

		private void DoSearch()
		{
			//LackList = new ObservableCollection<F051206LackList>();
			//LackList2 = new ObservableCollection<F051206LackList>();
			//LackList_Allot = new ObservableCollection<F051206LackList_Allot>();
			//LackList2_Allot = new ObservableCollection<F051206LackList_Allot>();

			LackList = null;
			LackList2 = null;
			LackList_Allot = null;
			LackList2_Allot = null;

			if (CURRENT_TAB.Equals("PICK"))
			{
				#region PICK
				proxyEx = GetExProxy<P06ExDataSource>();
				//if (UserOperateMode == OperateMode.Query || SHOWQUERYPAGE == "Visible")
				if (SHOWQUERYPAGE == "Visible")
				{
					//查詢模式
					PickList = proxyEx.CreateQuery<F051206Pick>("GetGetF051206PicksByQuery")
										.AddQueryExOption("dcCode", cbDC_CODE)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
										.AddQueryExOption("delvDateStart", dpDELV_DATE_Start)
										.AddQueryExOption("delvDateEnd", dpDELV_DATE_End)
										.AddQueryExOption("status", cbSTATUS)
										.AddQueryExOption("pickOrdNo", tbPICK_ORD_NO)
										.AddQueryExOption("wmsOrdNo", tbWMS_ORD_NO)
										.AddQueryExOption("containerCode", tbCONTAINER_CODE)
										.AddQueryExOption("crtOrUpdOpertor", tbCrtOrUpdOpertor)
										.ToObservableCollection();
					if (PickList == null || !PickList.Any())
					{
						DialogService.ShowMessage(Resources.Resources.InfoNoData);
					}
				}
				else
				{
					//新增模式
					PickList2 = proxyEx.CreateQuery<F051206Pick>("GetGetF051206PicksByAdd")
										.AddQueryExOption("dcCode", cbDC_CODE)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
										.AddQueryExOption("delvDateStart", dpDELV_DATE_Start)
										.AddQueryExOption("delvDateEnd", dpDELV_DATE_End)
										.AddQueryExOption("pickordno", tbPICK_ORD_NO)
										.ToObservableCollection();
					if (PickList2 == null || !PickList2.Any())
					{
						DialogService.ShowMessage(Resources.Resources.InfoNoData);
					}
				}
				
				#endregion
			}
			else
			{
				#region ALLOT
				proxyEx = GetExProxy<P06ExDataSource>();
				//if (UserOperateMode == OperateMode.Query || SHOWQUERYPAGE == "Visible")
				if (SHOWQUERYPAGE == "Visible")
				{
					PickList_Allot = proxyEx.CreateQuery<F051206AllocationList>("GetF051206AllocationLists")
															.AddQueryExOption("dcCode", cbDC_CODE)
															.AddQueryExOption("gupCode", _gupCode)
															.AddQueryExOption("custCode", _custCode)
															.AddQueryExOption("editType", "QUERY")
															.AddQueryExOption("status", cbSTATUS_ALLOT)
															.AddQueryExOption("allocation_no", tbPICK_ORD_NO_ALLOT).ToObservableCollection();
					if (PickList_Allot == null || !PickList_Allot.Any())
					{
						DialogService.ShowMessage(Resources.Resources.InfoNoData);
					}
				}
				else
				{
					PickList2_Allot = proxyEx.CreateQuery<F051206AllocationList>("GetF051206AllocationLists")
															.AddQueryExOption("dcCode", cbDC_CODE)
															.AddQueryExOption("gupCode", _gupCode)
															.AddQueryExOption("custCode", _custCode)
															.AddQueryExOption("editType", "ADD")
															.AddQueryExOption("status", "0")
															.AddQueryExOption("allocation_no", tbPICK_ORD_NO_ALLOT).ToObservableCollection();
					if (PickList2_Allot == null || !PickList2_Allot.Any())
					{
						DialogService.ShowMessage(Resources.Resources.InfoNoData);
					}
				}
				#endregion
			}
		}
		private void DoSearchCompleted()
		{
			//EditAction();
			//if (UserOperateMode == OperateMode.Add)
			//{
			//	SHOWQUERYPAGE = "Collapsed";
			//	SHOWADDPAGE = "Visible";
			//	UserOperateMode = OperateMode.Add;
			//}
			//else
			//{
			//	UserOperateMode = OperateMode.Query;
			//	SHOWQUERYPAGE = "Visible";
			//	SHOWADDPAGE = "Collapsed";
			//}

			// 每次查詢後，初始化全選CheckBox
			CUST_ORD_NO = "";
			ORD_NO = "";
			IsCheckAll = false;
			IsCheckAll2 = false;
			IsCheckAll3 = false;
			IsCheckAll4 = false;
			if (CURRENT_TAB.Equals("PICK"))
			{
				#region PICK
				if (SHOWQUERYPAGE == "Visible")
				{
					if (PickList != null && PickList.Any())
						SearchAction();
				}
				else
				{
					if (PickList2 != null && PickList2.Any())
						SearchAction();
				}

				#endregion
			}
			else
			{
				#region ALLOT
				if (SHOWQUERYPAGE == "Visible")
				{
					if (PickList_Allot != null && PickList_Allot.Any())
						SearchAction();
				}
				else
				{
					if (PickList2_Allot != null && PickList2_Allot.Any())
						SearchAction();
				}
				#endregion
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
			//proxyEx = GetExProxy<P06ExDataSource>();
			//PickList = proxyEx.CreateQuery<F051206Pick>("GetF051206Picks")
			//											.AddQueryExOption("dcCode", cbDC_CODE)
			//											.AddQueryExOption("gupCode", _gupCode)
			//											.AddQueryExOption("custCode", _custCode)
			//											.AddQueryExOption("delvDate", dpDELV_DATE.ToString("yyyy/MM/dd"))
			//											.AddQueryExOption("pickTime", cbPICK_TIME)
			//											.AddQueryExOption("editType", "ADD")
			//											.ToObservableCollection();
			//執行新增動作
		}
		private void DoAddCompleted()
		{
			SHOWQUERYPAGE = "Collapsed";
			SHOWADDPAGE = "Visible";
			AddAction();
			ORD_NO = "";
			CUST_ORD_NO = "";
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
					() => (UserOperateMode == OperateMode.Query || UserOperateMode == OperateMode.Add) &&
						((LackList != null && LackList.FirstOrDefault() != null && LackList.FirstOrDefault().STATUS != "2" && LackList.FirstOrDefault().STATUS != "3") ||
						(LackList2 != null && LackList2.FirstOrDefault() != null && LackList2.FirstOrDefault().STATUS != "2" && LackList2.FirstOrDefault().STATUS != "3") ||
						(LackList_Allot != null && LackList_Allot.FirstOrDefault() != null && LackList_Allot.FirstOrDefault().STATUS != "2" && LackList_Allot.FirstOrDefault().STATUS != "3") ||
						(LackList2_Allot != null && LackList2_Allot.FirstOrDefault() != null && LackList2_Allot.FirstOrDefault().STATUS != "2" && LackList2_Allot.FirstOrDefault().STATUS != "3")),
					o => DoEditCompleted()
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;

			// 若要編輯缺貨的話，處理結果預設先都為預設
			cbRETURN_FLAG = RETURN_FLAGList.Select(x => x.Value).FirstOrDefault();
			cbRETURN_FLAG_ALLOT = RETURN_FLAGList_ALLOT.Select(x => x.Value).FirstOrDefault();
			// SetLackQtyIsEnabled();
		}

		private void DoEditCompleted()
		{
			showSaveBtn = true;
			EditAction();
    }

		private void SetLackQtyIsEnabled()
		{
			if (CURRENT_TAB == "PICK")
			{
				if (PickSelected == null)
					return;

				var proxy = GetProxy<F05Entities>();
			var item =	proxy.F051201s.Where(
					o =>
						o.DC_CODE == PickSelected.DC_CODE && o.GUP_CODE == PickSelected.GUP_CODE && o.CUST_CODE == PickSelected.CUST_CODE && o.PICK_ORD_NO == PickSelected.PICK_ORD_NO)
					.First();
				var isPickInMergeBox = proxy.CreateQuery<F052901>("GetDataByPickOrdNo")
					.AddQueryExOption("dcCode", PickSelected.DC_CODE)
					.AddQueryExOption("gupCode", PickSelected.GUP_CODE)
					.AddQueryExOption("custCode", PickSelected.CUST_CODE)
					.AddQueryExOption("pickOrdNo", PickSelected.PICK_ORD_NO).ToList().Any();
			//如果是B2B非台車 或B2C非台車 且未進入合流才可以改缺貨數
			LackQtyIsEnabled = ((item.ORD_TYPE == "0" && item.ISPRINTED == "1") || (item.ORD_TYPE == "1" && item.ISDEVICE == "0")) && !isPickInMergeBox;
				                        

			}
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() => UserOperateMode != OperateMode.Query,
					p => DoCancelCompleted()
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
		}

		private void DoCancelCompleted()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				//PickList = new ObservableCollection<F051206Pick>();
				//LackList = new ObservableCollection<F051206LackList>();
				//PickList2 = new ObservableCollection<F051206Pick>();
				//LackList2 = new ObservableCollection<F051206LackList>();

				//PickList_Allot = new ObservableCollection<F051206AllocationList>();
				//LackList_Allot = new ObservableCollection<F051206LackList_Allot>();
				//PickList2_Allot = new ObservableCollection<F051206AllocationList>();
				//LackList2_Allot = new ObservableCollection<F051206LackList_Allot>();

				PickList = null;
				LackList = null;
				PickList2 = null;
				LackList2 = null;
				CUST_ORD_NO = "";
				ORD_NO = "";
				PickList_Allot = null;
				LackList_Allot = null;
				PickList2_Allot = null;
				LackList2_Allot = null;
				ALLOCATION_NO = "";

				UserOperateMode = OperateMode.Query;
				SHOWQUERYPAGE = "Visible";
				SHOWADDPAGE = "Collapsed";
				CancelAction();
				showSaveBtn = false;
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
				 () => (SHOWQUERYPAGE == "Visible" &&
							 (LackList != null && LackList.Where(x => x.ISSELECTED).FirstOrDefault() != null || LackList_Allot != null && LackList_Allot.Where(x => x.ISSELECTED).FirstOrDefault() != null)),
					o => DoDeleteCompleted()
					);
			}
		}

		private void DoDelete()
		{

			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				if (CURRENT_TAB == "PICK")
				{
					var selectedLackList = LackList.Where(x => x.ISSELECTED).ToArray();
					var wcfF051206LackList = ExDataMapper.MapCollection<F051206LackList, wcf.F051206LackList>(selectedLackList).ToArray();
					var proxy = new wcf.P06WcfServiceClient();
					var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteF051206(wcfF051206LackList, _userId));

					if (result.IsSuccessed)
					{
						ShowMessage(Messages.InfoDeleteSuccess);
					}
					else
					{
						ShowWarningMessage(result.Message);
					}
				}
				else
				{
					var selectedLackList = LackList_Allot.Where(x => x.ISSELECTED).ToArray();
					var wcfF051206LackList = ExDataMapper.MapCollection<F051206LackList_Allot, wcf.F051206LackList_Allot>(selectedLackList).ToArray();
					var proxy = new wcf.P06WcfServiceClient();
					var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteF151003(wcfF051206LackList, _userId));
					if (result.IsSuccessed)
					{
						ShowMessage(Messages.InfoDeleteSuccess);
					}
					else
					{
						ShowWarningMessage(result.Message);
					}
				}
			}
		}
		private void DoDeleteCompleted()
		{
			if (CURRENT_TAB == "PICK")
				SetLackList();
			else
				setLackList_Allot();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode == OperateMode.Edit && showSaveBtn,

					o => DoSaveCompleted()
					);
			}
		}

		private void DoSave()
		{
			IsSave = false;
			if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
			{
				if (SHOWADDPAGE == "Visible")
					DoSaveAdd();
				else if (SHOWQUERYPAGE == "Visible")
					DoSaveEdit();
			}

		}
		private void DoSaveAdd()
		{
			IsSave = true;
			if (CURRENT_TAB == "PICK")
			{
				var selectedLackList = LackList2.Where(x => x.ISSELECTED).ToArray();
				if (!selectedLackList.Any())
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_CheckItemDetail);
					return;
				}
				if (selectedLackList.Any(o => o.LACK_QTY <= 0))
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyGreaterThanZero);
					return;
				}
				if (selectedLackList.Any(o => o.LACK_QTY > o.B_PICK_QTY))
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyNotOverPickQty);
					return;
				}
				var wcfF051206LackList = ExDataMapper.MapCollection<F051206LackList, wcf.F051206LackList>(selectedLackList).ToArray();
				var proxy = new wcf.P06WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertF051206(wcfF051206LackList, _userId));
				IsSave = result.IsSuccessed;
				if (result.IsSuccessed)
					ShowMessage(Messages.InfoAddSuccess);
				else
					ShowWarningMessage(result.Message);
			}
			else
			{
				var selectedLackList_allot = LackList2_Allot.Where(x => x.ISSELECTED).ToArray();
				if (selectedLackList_allot.Count() == 0)
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_CheckItemDetail);
					return;
				}
				if (selectedLackList_allot.Any(x => x.LACK_QTY <= 0))
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyGreaterThanZero);
					return;
				}
				if (selectedLackList_allot.Any(o => o.LACK_QTY > o.SRC_QTY))
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyNotOverTransQty);
					return;
				}
				var wcfF051206LackList_allot = ExDataMapper.MapCollection<F051206LackList_Allot, wcf.F051206LackList_Allot>(selectedLackList_allot).ToArray();
				var proxy = new wcf.P06WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertF151003(wcfF051206LackList_allot, _userId));
				if (result.IsSuccessed)
					ShowMessage(Messages.InfoAddSuccess);
				else
					ShowWarningMessage(result.Message);
			}
		}
		private void DoSaveEdit()
		{
			IsSave = true;
			if (CURRENT_TAB == "PICK")
			{
				//揀貨
				if (LackList.Any(x => x.LACK_QTY == 0)  && cbRETURN_FLAG != "3")
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyNotZero);
					return;
				}
				if (LackList.Any(o => o.LACK_QTY < 0))
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyGreaterThanZero);
					return;
				}
				if (LackList.Any(o => o.LACK_QTY > o.B_PICK_QTY))
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyNotOverPickQty);
					return;
				}
				foreach (var p in LackList)
				{
					if(cbRETURN_FLAG == "3")
					{
						p.LACK_QTY = 0;
					}
					p.RETURN_FLAG = cbRETURN_FLAG;
				}

				var wcfF051206LackList = ExDataMapper.MapCollection<F051206LackList, wcf.F051206LackList>(LackList).ToArray();
				var proxy = new wcf.P06WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateF051206(wcfF051206LackList, _userId));
				if (result.IsSuccessed)
				{
					ShowMessage(Messages.InfoUpdateSuccess);
				}
				else
				{
					ShowWarningMessage(result.Message);
				}
			}
			else
			{
				//調撥 若已找到商品 不用檢核數量 0
				if (LackList_Allot.Any(x => x.LACK_QTY == 0) && cbRETURN_FLAG_ALLOT !="3")
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyNotZero);
					return;
				}
				if (LackList_Allot.Any(o => o.LACK_QTY < 0))
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyGreaterThanZero);
					return;
				}
				if (LackList_Allot.Any(o => o.LACK_QTY > o.SRC_QTY))
				{
					IsSave = false;
					DialogService.ShowMessage(Properties.Resources.P0602010000_ViewModel_OutofStockQtyNotOverTransQty);
					return;
				}
				foreach (var p in LackList_Allot)
				{
					p.STATUS = cbRETURN_FLAG_ALLOT;
				}
				var wcfF051206LackList = ExDataMapper.MapCollection<F051206LackList_Allot, wcf.F051206LackList_Allot>(LackList_Allot).ToArray();
				var proxy = new wcf.P06WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateF151003(wcfF051206LackList, _userId));
				if (result.IsSuccessed)
				{
					ShowMessage(Messages.InfoUpdateSuccess);
				}
				else
				{
					ShowWarningMessage(result.Message);
				}
			}

		}

		private void DoSaveCompleted()
		{
			if (IsSave)
			{
				UserOperateMode = OperateMode.Query;
				GridReadOnlylAction();

				if (CURRENT_TAB == "PICK")
				{
					cbSTATUS = "";
					if (SHOWQUERYPAGE == "Visible")
					{
						tbPICK_ORD_NO = PickSelected.PICK_ORD_NO;
					}
					else
					{
						tbPICK_ORD_NO = PickSelected2.PICK_ORD_NO;
					}
				}
				else
				{
					cbSTATUS_ALLOT = "";
					tbPICK_ORD_NO_ALLOT = ALLOCATION_NO;
				}

				PickList = null;
				LackList = null;
				PickList2 = null;
				LackList2 = null;
				CUST_ORD_NO = "";
				ORD_NO = "";
				PickList_Allot = null;
				LackList_Allot = null;
				PickList2_Allot = null;
				LackList2_Allot = null;
				ALLOCATION_NO = "";

				SHOWQUERYPAGE = "Visible";
				SHOWADDPAGE = "Collapsed";
				showSaveBtn = false;
				SearchCommand.Execute(null);
			}

		}
		#endregion Save

		#region DeleteDetail
		public ICommand DeleteDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDeleteDetail(),
				 () => (UserOperateMode == OperateMode.Edit &&
							 (LackList != null && LackList.Where(x => x.ISSELECTED).FirstOrDefault() != null || LackList_Allot != null && LackList_Allot.Where(x => x.ISSELECTED).FirstOrDefault() != null)),
					o => DoDeleteDetailCompleted()
					);
			}
		}

		private void DoDeleteDetail()
		{
			IsDelete = false;
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				IsDelete = true;
				if (CURRENT_TAB == "PICK")
				{
					var selectedLackList = LackList.Where(x => x.ISSELECTED).ToArray();
					var wcfF051206LackList = ExDataMapper.MapCollection<F051206LackList, wcf.F051206LackList>(selectedLackList).ToArray();
					var proxy = new wcf.P06WcfServiceClient();
					var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteF051206(wcfF051206LackList, _userId));
					if (result.IsSuccessed)
						ShowMessage(Messages.InfoDeleteSuccess);
					else
						ShowWarningMessage(result.Message);
				}
				else
				{
					var selectedLackList = LackList_Allot.Where(x => x.ISSELECTED).ToArray();
					var wcfF051206LackList = ExDataMapper.MapCollection<F051206LackList_Allot, wcf.F051206LackList_Allot>(selectedLackList).ToArray();
					var proxy = new wcf.P06WcfServiceClient();
					var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteF151003(wcfF051206LackList, _userId));
					if (result.IsSuccessed)
						ShowMessage(Messages.InfoDeleteSuccess);
					else
						ShowWarningMessage(result.Message);
				}
			}
		}
		private void DoDeleteDetailCompleted()
		{
			if (IsDelete == true)
			{
				if (CURRENT_TAB == "PICK")
					SetLackList();
				else
					setLackList_Allot();
			}
		}
		#endregion Delete

		#region 調撥用
		private List<NameValuePair<string>> _STATUSList_ALLOT;
		public List<NameValuePair<string>> STATUSList_ALLOT
		{
			get { return _STATUSList_ALLOT; }
			set
			{
				_STATUSList_ALLOT = value;
				RaisePropertyChanged("STATUSList_ALLOT");
			}
		}

		private string _cbSTATUS_ALLOT;
		public string cbSTATUS_ALLOT
		{
			get { return _cbSTATUS_ALLOT; }
			set
			{
				_cbSTATUS_ALLOT = value;
				RaisePropertyChanged("cbSTATUS_ALLOT");
			}
		}

		private string _cbDC_CODE_ALLOT;
		public string cbDC_CODE_ALLOT
		{
			get { return _cbDC_CODE_ALLOT; }
			set
			{
				_cbDC_CODE_ALLOT = value;
				RaisePropertyChanged("cbDC_CODE_ALLOT");
			}
		}

		private string _tbPICK_ORD_NO_ALLOT;
		public string tbPICK_ORD_NO_ALLOT
		{
			get { return _tbPICK_ORD_NO_ALLOT; }
			set
			{
				_tbPICK_ORD_NO_ALLOT = value;
				RaisePropertyChanged("tbPICK_ORD_NO_ALLOT");
			}
		}


		#endregion 調撥用
	}
}
