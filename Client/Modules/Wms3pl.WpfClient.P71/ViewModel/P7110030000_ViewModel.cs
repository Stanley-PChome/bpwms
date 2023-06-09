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
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7110030000_ViewModel : InputViewModelBase
	{
		public Action<OperateMode> OnFocusAction = delegate { };

		public P7110030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料				
				SetOrdTypeList();

				SetDcList();
				SearchDcItem = DcList.FirstOrDefault();
                SetSplitPickTypeList();
                //SetF190001List();
            }

		}

		#region 查詢結果

		private List<F050004WithF190001> _f050004WithF190001List;

		public List<F050004WithF190001> F050004WithF190001List
		{
			get { return _f050004WithF190001List; }
			set
			{
				_f050004WithF190001List = value;
				RaisePropertyChanged("F050004WithF190001List");
			}
		}

		private F050004WithF190001 _selectedF050004WithF190001;

		public F050004WithF190001 SelectedF050004WithF190001
		{
			get { return _selectedF050004WithF190001; }
			set
			{
				_selectedF050004WithF190001 = value;
				RaisePropertyChanged("SelectedF050004WithF190001");
			}
		}

		#endregion

		#region 單據名稱

		private List<F190001> _f190001List;

		public List<F190001> F190001List
		{
			get { return _f190001List; }
			set
			{
				_f190001List = value;
				RaisePropertyChanged("F190001List");
			}
		}

		private F190001 _selectedF190001;

		public F190001 SelectedF190001
		{
			get { return _selectedF190001; }
			set
			{
				_selectedF190001 = value;
				RaisePropertyChanged("SelectedF190001");

				if (value == null)
				{
					return;
				}

				OrdTypeName = OrdTypeList.Where(item => item.Value == value.TICKET_TYPE).Select(item => item.Name).FirstOrDefault();
				SetOrdPropList(value.TICKET_TYPE);
				OrdPropName = OrdPropList.Where(item => item.Value == value.TICKET_CLASS).Select(item => item.Name).FirstOrDefault();				
			}
		}

		public void SetF190001List()
		{
			var proxy = GetProxy<F19Entities>();
			F190001List =
        proxy.F190001s.Where(item => item.DC_CODE == SelectedDcItem.Value && item.GUP_CODE == SelectedGupItem.Value & item.CUST_CODE == SelectedCustItem.Value && item.TICKET_TYPE == "O")
          .OrderBy(item => item.TICKET_ID).ToList();
		}
		#endregion

		#region 共用 物流中心清單
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
        #endregion

        #region 依溫層、樓層、儲區拆單夏拉是選單

        private List<NameValuePair<string>> _splitPickTypeList;

        public List<NameValuePair<string>> SplitPickTypeList
        {
            get { return _splitPickTypeList; }
            set
            {
                _splitPickTypeList = value;
                RaisePropertyChanged("SplitPickTypeList");
            }
        }   

        public void SetSplitPickTypeList()
        {
            var data = GetBaseTableService.GetF000904List(FunctionCode, "F050004", "SPILT_PICK_TYPE");
            SplitPickTypeList = (from o in data
                                 select new NameValuePair<string>
                                 {
                                     Name = o.Name,
                                     Value = o.Value
                                 }).OrderBy(o => o.Value).ToList();    
        }

        #endregion

        #region 查詢的 物流中心, 業主(清單) 貨主(清單)

        private List<NameValuePair<string>> _searchGupList;

		public List<NameValuePair<string>> SearchGupList
		{
			get { return _searchGupList; }
			set
			{
				_searchGupList = value;
				RaisePropertyChanged("SearchGupList");
			}
		}

		private List<NameValuePair<string>> _searchCustList;

		public List<NameValuePair<string>> SearchCustList
		{
			get { return _searchCustList; }
			set
			{
				_searchCustList = value;
				RaisePropertyChanged("SearchCustList");
			}
		}

		private NameValuePair<string> _searchDcItem;

		public NameValuePair<string> SearchDcItem
		{
			get { return _searchDcItem; }
			set
			{
				_searchDcItem = value;
				RaisePropertyChanged("SearchDcItem");

				if (value != null)
				{
					SetSearchGupList(value.Value);

					if (UserOperateMode == OperateMode.Query)
						SearchGupItem = SearchGupList.FirstOrDefault();
				}
			}
		}

		private NameValuePair<string> _searchGupItem;

		public NameValuePair<string> SearchGupItem
		{
			get { return _searchGupItem; }
			set
			{
				_searchGupItem = value;
				RaisePropertyChanged("SearchGupItem");

				if (value != null && SearchDcItem != null)
				{
					SetSearchCustList(SearchDcItem.Value, value.Value);

					if (UserOperateMode == OperateMode.Query)
						SearchCustItem = SearchCustList.FirstOrDefault();
				}
			}
		}

		private NameValuePair<string> _searchCustItem;

		public NameValuePair<string> SearchCustItem
		{
			get { return _searchCustItem; }
			set
			{
				_searchCustItem = value;
				RaisePropertyChanged("SearchCustItem");
			}
		}

		/// <summary>
		/// 設定業主清單
		/// </summary>
		public void SetSearchGupList(string dcCode)
		{
			var gupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(dcCode);

			gupList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "-1" });
			SearchGupList = gupList;
		}

		/// <summary>
		/// 設定貨主清單
		/// </summary>
		/// <param name="gupCode">業主</param>
		public void SetSearchCustList(string dcCode, string gupCode)
		{
			if (gupCode == null)
				return;

			var custList = Wms3plSession.Get<GlobalInfo>().GetCustDataList(dcCode, gupCode);

			custList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "-1" });
			SearchCustList = custList;
		}

		#endregion

		#region 編輯的 物流中心, 業主(清單) 貨主(清單)

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

		private List<NameValuePair<string>> _custList;
		public List<NameValuePair<string>> CustList
		{
			get { return _custList; }
			set
			{
				_custList = value;
				RaisePropertyChanged("CustList");
			}
		}

		private NameValuePair<string> _selectedDcItem;

		public NameValuePair<string> SelectedDcItem
		{
			get { return _selectedDcItem; }
			set
			{
				_selectedDcItem = value;
				RaisePropertyChanged("SelectedDcItem");

				if (value == null)
				{
					GupList = null;
					return;
				}

				SetGupList(value.Value);

				if (UserOperateMode == OperateMode.Add)
				{
					SelectedGupItem = GupList.FirstOrDefault();
				}
			}
		}

		private NameValuePair<string> _selectedGupItem;

		public NameValuePair<string> SelectedGupItem
		{
			get { return _selectedGupItem; }
			set
			{
				_selectedGupItem = value;
				RaisePropertyChanged("SelectedGupItem");

				if (value == null || SelectedDcItem == null)
				{
					CustList = null;
					return;
				}

				SetCustList(SelectedDcItem.Value, value.Value);

				if (UserOperateMode == OperateMode.Add)
				{
					SelectedCustItem = CustList.FirstOrDefault();
				}
			}
		}

		private NameValuePair<string> _selectedCustItem;

		public NameValuePair<string> SelectedCustItem
		{
			get { return _selectedCustItem; }
			set
			{
				_selectedCustItem = value;
				RaisePropertyChanged("SelectedCustItem");
				if (_selectedCustItem != null)
					SetF190001List();
			}
		}

		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
		}

		/// <summary>
		/// 設定業主清單
		/// </summary>
		public void SetGupList(string dcCode)
		{
			var gupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(dcCode);

			//gupList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "-1" });
			GupList = gupList;
		}

		/// <summary>
		/// 設定貨主清單
		/// </summary>
		/// <param name="gupCode">業主</param>
		public void SetCustList(string dcCode, string gupCode)
		{
			if (gupCode == null)
				return;

			var custList = Wms3plSession.Get<GlobalInfo>().GetCustDataList(dcCode, gupCode);

			//custList.Insert(0, new NameValuePair<string> { Name = Properties.Resources.P7105010000_ViewModel_NONE_SPECIFY, Value = "0" });
			CustList = custList;
		}

		#endregion

		#region 單據類型 單據類別 出貨類型 lable 名稱
		private string _ordTypeName;

		public string OrdTypeName
		{
			get { return _ordTypeName; }
			set
			{
				_ordTypeName = value;
				RaisePropertyChanged("OrdTypeName");
			}
		}

		private string _ordPropName;

		public string OrdPropName
		{
			get { return _ordPropName; }
			set
			{
				_ordPropName = value;
				RaisePropertyChanged("OrdPropName");
			}
		}

		private string _outTypeName;

		public string OutTypeName
		{
			get { return _outTypeName; }
			set
			{
				_outTypeName = value;
				RaisePropertyChanged("OutTypeName");
			}
		}


		#endregion

		#region 單據類型 單據類別清單
		private List<NameValuePair<string>> _ordTypeList;

		public List<NameValuePair<string>> OrdTypeList
		{
			get { return _ordTypeList; }
			set
			{
				_ordTypeList = value;
				RaisePropertyChanged("OrdTypeList");
			}
		}

		private List<NameValuePair<string>> _ordPropList;

		public List<NameValuePair<string>> OrdPropList
		{
			get { return _ordPropList; }
			set
			{
				_ordPropList = value;
				RaisePropertyChanged("OrdPropList");
			}
		}


		public void SetOrdTypeList()
		{
			var proxy = GetProxy<F00Entities>();
			OrdTypeList = proxy.F000901s.OrderBy(item => item.ORD_TYPE)
										.Select(item => new NameValuePair<string>
										{
											Name = item.ORD_NAME,
											Value = item.ORD_TYPE
										})
										.ToList();
		}

		public void SetOrdPropList(string ordType)
		{
			var proxy = GetProxy<F00Entities>();
			OrdPropList = proxy.F000903s.Where(item => item.ORD_PROP.StartsWith(ordType))
										.OrderBy(item => item.ORD_PROP)
										.Select(item => new NameValuePair<string>
										{
											Name = item.ORD_PROP_NAME,
											Value = item.ORD_PROP
										})
										.ToList();
		}
		#endregion
		
		#region 編輯的主檔
		private F050004WithF190001 _editableF050004WithF190001;

		public F050004WithF190001 EditableF050004WithF190001
		{
			get { return _editableF050004WithF190001; }
			set
			{
				_editableF050004WithF190001 = value;
				RaisePropertyChanged("EditableF050004WithF190001");
			}
		}


		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						string dcCode = SearchDcItem.Value;
						string gupCode = (SearchGupItem == null || SearchGupItem == SearchGupList.First()) ? null : SearchGupItem.Value;
						string custCode = (SearchCustItem == null || SearchCustItem == SearchCustList.First()) ? null : SearchCustItem.Value;
						DoSearch(dcCode, gupCode, custCode);
					},
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						SelectedF050004WithF190001 = F050004WithF190001List.FirstOrDefault();
					}
					);
			}
		}

		private void DoSearch(string dcCode, string gupCode, string custCode)
		{
			//執行查詢動
			var proxy = GetExProxy<P71ExDataSource>();
			var query = proxy.CreateQuery<F050004WithF190001>("GetF050004WithF190001s")
							 .AddQueryExOption("dcCode", dcCode)
							 .AddQueryExOption("gupCode", gupCode)
							 .AddQueryExOption("custCode", custCode);

			var list = query.ToList();
			F050004WithF190001List = list;

			if (F050004WithF190001List.Any() == false)
			{
				ShowMessage(Messages.InfoNoData);
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
			OnFocusAction(OperateMode.Add);

            //執行新增動作
            EditableF050004WithF190001 = new F050004WithF190001()
            {
                MERGE_ORDER = "0",
                SPLIT_FLOOR = "0",
                SPLIT_PICK_TYPE = SplitPickTypeList.Any() ? SplitPickTypeList.FirstOrDefault().Value : "0"
            };
           
            //SelectedDcItem = DcList.FirstOrDefault();
            //SelectedF190001 = F190001List.FirstOrDefault();

        }
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && SelectedF050004WithF190001 != null
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			OnFocusAction(OperateMode.Edit);
			//執行編輯動作
			if (SelectedF050004WithF190001 != null)
			{
				EditableF050004WithF190001 = ExDataMapper.Map<F050004WithF190001, F050004WithF190001>(SelectedF050004WithF190001);

			}
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						if (ShowMessage(Messages.WarningBeforeCancel) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						DoCancel();
					},
					() => UserOperateMode != OperateMode.Query

					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			EditableF050004WithF190001 = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
					o =>
					{
						isSaved = false;

						var error = GetEditableDataError(EditableF050004WithF190001);
						if (!string.IsNullOrEmpty(error))
						{
							DialogService.ShowMessage(error);
							return;
						}

						if (ShowMessage(Messages.WarningBeforeUpdate) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						isSaved = DoSave();

						if (isSaved)
						{
							DoSearch(EditableF050004WithF190001.DC_CODE,
									 EditableF050004WithF190001.GUP_CODE,
									 EditableF050004WithF190001.CUST_CODE);
						}
					},
					() => UserOperateMode != OperateMode.Query
						&& EditableF050004WithF190001 != null
						&& EditableF050004WithF190001.TICKET_ID > 0,
					o =>
					{
						if (isSaved)
						{
							SearchDcItem = DcList.Where(item => item.Value == EditableF050004WithF190001.DC_CODE).FirstOrDefault();
							SearchGupItem = SearchGupList.Where(item => item.Value == EditableF050004WithF190001.GUP_CODE).FirstOrDefault();
							SearchCustItem = SearchCustList.Where(item => item.Value == EditableF050004WithF190001.CUST_CODE).FirstOrDefault();
							var ticketId = EditableF050004WithF190001.TICKET_ID;
							EditableF050004WithF190001 = null;

							SelectedF050004WithF190001 = F050004WithF190001List.Where(item => item.TICKET_ID == ticketId).FirstOrDefault();
							UserOperateMode = OperateMode.Query;
						}
					}
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作

			var proxy = GetProxy<F05Entities>();

			if (UserOperateMode == OperateMode.Add)
			{
				var f050004 = ExDataMapper.Map<F050004WithF190001, F050004>(EditableF050004WithF190001);
				proxy.AddToF050004s(f050004);
			}
			else
			{
				var f050004 = GetF050004(proxy);

				if (f050004 == null)
				{
					DialogService.ShowMessage(Properties.Resources.P7110030000_ViewModel_DataNotExist);
					return false;
				}

				f050004.SOUTH_PRIORITY_QTY = EditableF050004WithF190001.SOUTH_PRIORITY_QTY;
				f050004.ORDER_LIMIT = EditableF050004WithF190001.ORDER_LIMIT;
				f050004.DELV_DAY = EditableF050004WithF190001.DELV_DAY;
				f050004.SPLIT_FLOOR = EditableF050004WithF190001.SPLIT_FLOOR;
				f050004.MERGE_ORDER = EditableF050004WithF190001.MERGE_ORDER;
                f050004.SPLIT_PICK_TYPE = EditableF050004WithF190001.SPLIT_PICK_TYPE;

				proxy.UpdateObject(f050004);
			}

			proxy.SaveChanges();
			ShowMessage(Messages.Success);

			return true;
		}

		private F050004 GetF050004(F05Entities proxy)
		{
			var f050004 = (from item in proxy.F050004s
						   where item.TICKET_ID == EditableF050004WithF190001.TICKET_ID
						   where item.DC_CODE == EditableF050004WithF190001.DC_CODE
						   where item.GUP_CODE == EditableF050004WithF190001.GUP_CODE
						   where item.CUST_CODE == EditableF050004WithF190001.CUST_CODE
						   select item).FirstOrDefault();
			return f050004;
		}



		string GetEditableDataError(F050004WithF190001 item)
		{
			item.SPLIT_FLOOR = item.SPLIT_FLOOR ?? "0";
			item.MERGE_ORDER = item.MERGE_ORDER ?? "0";

			if (string.IsNullOrEmpty(item.DC_CODE) || string.IsNullOrEmpty(item.GUP_CODE) || string.IsNullOrEmpty(item.CUST_CODE))
			{
				return Properties.Resources.P7110010000_ViewModel_MustSelectDC_Gup_Cust;
			}

			if (item.TICKET_ID == 0)
			{
				return Properties.Resources.P7110030000_ViewModel_MustSelectTicketName;
			}

			if (item.SOUTH_PRIORITY_QTY < 0)
			{
				return Properties.Resources.P7110030000_ViewModel_SOUTH_PRIORITY_QTY;
			}

			if (item.DELV_DAY < 0)
			{
				return Properties.Resources.P7110030000_ViewModel_DELV_DAY_Incorrect;
			}

			if (item.ORDER_LIMIT <= 1)
			{
				return Properties.Resources.P7110030000_ViewModel_ORDER_LIMIT_Incorrect;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				if (IsRepeatTicket(item))
				{
					return Properties.Resources.P7110030000_ViewModel_SameDC_Gup_Cust_TicketName_CannotRepeat;
				}
			}


			return string.Empty;
		}

		/// <summary>
		/// 是否重複同物流中心、業主、貨主、單據名稱
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		bool IsRepeatTicket(F050004WithF190001 e)
		{
			var proxy = GetProxy<F05Entities>();
			var query = from item in proxy.F050004s
						where item.DC_CODE == e.DC_CODE
						where item.GUP_CODE == e.GUP_CODE
						where item.CUST_CODE == e.CUST_CODE
						where item.TICKET_ID == e.TICKET_ID
						select item;

			return query.FirstOrDefault() != null;
		}
		#endregion Save

	}
}