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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.ExDataServices;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7110010000_ViewModel : InputViewModelBase
	{
		public P7110010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetTicketTypeList();
				SearchTicketTypeItem = TicketTypeList.FirstOrDefault();				
				SetMilestoneList();
				SetDcList();
				SearchDcItem = DcList.FirstOrDefault();

			}

		}

		#region 搜尋的 物流中心 業主 貨主清單

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

		#region 搜尋的單據類型

		private NameValuePair<string> _searchTicketTypeItem;

		public NameValuePair<string> SearchTicketTypeItem
		{
			get { return _searchTicketTypeItem; }
			set
			{
				_searchTicketTypeItem = value;
				RaisePropertyChanged("SearchTicketTypeItem");
			}
		}


		private List<NameValuePair<string>> _ticketTypeList;

		public List<NameValuePair<string>> TicketTypeList
		{
			get { return _ticketTypeList; }
			set
			{
				_ticketTypeList = value;
				RaisePropertyChanged("TicketTypeList");
			}
		}

		public void SetTicketTypeList()
		{
			var proxy = GetProxy<F00Entities>();
			var list = proxy.F000901s.Where(n => n.ISVISABLE == "1").ToList();

			var query = from item in list
						select new NameValuePair<string>
						{
							Name = item.ORD_NAME,
							Value = item.ORD_TYPE
						};

			TicketTypeList = query.ToList();
		}
		#endregion

		#region 搜尋結果
		public class F190001DataWithF19000103Data
		{
			public F190001Data F190001Data { get; set; }
			public F19000103Data F19000103Data { get; set; }
		}

		private List<F190001DataWithF19000103Data> _f190001DataWithF19000103DataList;

		public List<F190001DataWithF19000103Data> F190001DataWithF19000103DataList
		{
			get { return _f190001DataWithF19000103DataList; }
			set
			{
				_f190001DataWithF19000103DataList = value;
				RaisePropertyChanged("F190001DataWithF19000103DataList");
			}
		}

		private F190001DataWithF19000103Data _selectedF190001DataWithF19000103Data;

		public F190001DataWithF19000103Data SelectedF190001DataWithF19000103Data
		{
			get { return _selectedF190001DataWithF19000103Data; }
			set
			{
				_selectedF190001DataWithF19000103Data = value;
				RaisePropertyChanged("SelectedF190001DataWithF19000103Data");

				if (value == null)
				{
					EditableF190001 = null;
					return;
				}

				EditableF190001 = ExDataMapper.Map<F190001Data, F190001>(value.F190001Data);
			}
		}


		#endregion

		#region 新增編輯的 物流中心 業主 貨主清單
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

				if (SelectedDcItem == null || SelectedGupItem == null || SelectedCustItem == null)
				{
					F1947List = null;
					return;
				}

				SetF1947List(SelectedDcItem.Value, SelectedGupItem.Value, SelectedCustItem.Value);
				if (UserOperateMode == OperateMode.Add)
				{
					SelectedF1947 = F1947List.FirstOrDefault();
				}
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

		#region 單據類型 單據類別清單
		
		private List<NameValuePair<string>> _ticketClassList;

		public List<NameValuePair<string>> TicketClassList
		{
			get { return _ticketClassList; }
			set
			{
				_ticketClassList = value;
				RaisePropertyChanged("TicketClassList");
			}
		}

		private NameValuePair<string> _selectedOrdTypeItem;

		public NameValuePair<string> SelectedOrdTypeItem
		{
			get { return _selectedOrdTypeItem; }
			set
			{
				_selectedOrdTypeItem = value;
				RaisePropertyChanged("SelectedOrdTypeItem");
				if (_selectedOrdTypeItem != null && (_selectedOrdTypeItem.Value == "O" || _selectedOrdTypeItem.Value == "R"))
				{
					IsDelivery = true;
				}
				else
				{
					IsDelivery = false;
				}
				if (value == null)
				{
					TicketClassList = null;
					return;
				}

				SetTicketClassList(value.Value);
				if (UserOperateMode == OperateMode.Add)
				{
					SelectedTicketClassItem = TicketClassList.FirstOrDefault();
				}
				
			}
		}

		private NameValuePair<string> _selectedTicketClassItem;

		public NameValuePair<string> SelectedTicketClassItem
		{
			get { return _selectedTicketClassItem; }
			set
			{
				_selectedTicketClassItem = value;
				RaisePropertyChanged("SelectedTicketClassItem");

				if (SelectedOrdTypeItem == null || value == null)
				{
					EditableF19000103 = null;
					return;
				}

				SetF19000103(SelectedOrdTypeItem.Value, SelectedTicketClassItem.Value);
			}
		}


		public void SetTicketClassList(string ordType)
		{
			var proxy = GetProxy<F00Entities>();
			TicketClassList = proxy.F000906s.Where(item => item.TICKET_TYPE==ordType)
										.OrderBy(item => item.TICKET_CLASS)
										.Select(item => new NameValuePair<string>
										{
											Name = item.TICKET_CLASS_NAME,
											Value = item.TICKET_CLASS
										})
										.ToList();
		}
		#endregion

		#region 配送商清單
		private List<F1947> _f1947List;

		public List<F1947> F1947List
		{
			get { return _f1947List; }
			set
			{
				_f1947List = value;
				RaisePropertyChanged("F1947List");
			}
		}

		private F1947 _selectedF1947;

		public F1947 SelectedF1947
		{
			get { return _selectedF1947; }
			set
			{
				_selectedF1947 = value;
				RaisePropertyChanged("SelectedF1947");

				if (value == null)
				{
					return;
				}

				IsNotSelectedF1947 = string.IsNullOrEmpty(value.ALL_COMP);
				if (value != F1947List.FirstOrDefault())
				{
					//EditableF190001.FAST_DELIVER = value.CAN_FAST;
				}
			}
		}


        private void SetF1947List(string dcCode, string gupCode, string custCode)
        {
            var proxy = GetProxy<F19Entities>();
            var list = proxy.CreateQuery<F1947>("GetAllowedF1947s")
                                    .AddQueryExOption("dcCode", dcCode)
                                    .AddQueryExOption("gupCode", gupCode)
                                    .AddQueryExOption("custCode", custCode)
																		.Where(x=>x.TYPE == "0")
                                    .ToList();

            list.Insert(0, new F1947() { ALL_COMP = string.Empty, ALL_ID = "-1" });
            F1947List = list;
        }

		private bool _isNotSelectedF1947 = true;

		public bool IsNotSelectedF1947
		{
			get { return _isNotSelectedF1947; }
			set
			{
				_isNotSelectedF1947 = value;
				RaisePropertyChanged("IsNotSelectedF1947");
			}
		}


		private bool _canSelectedF1947 = true;

		public bool CanSelectedF1947
		{
			get { return _canSelectedF1947; }
			set
			{
				_canSelectedF1947 = value;
				RaisePropertyChanged("CanSelectedF1947");
			}
		}

		public void SetCanSelectedF1947()
		{
			if (EditableF190001 == null || F1947List == null)
			{
				return;
			}

			CanSelectedF1947 = (EditableF190001.SHIPPING_ASSIGN == "0");
			if (!CanSelectedF1947)
			{
				SelectedF1947 = F1947List.FirstOrDefault();
			}
		}

		private bool _isDelivery = true;

		public bool IsDelivery
		{
			get { return _isDelivery; }
			set
			{
				_isDelivery = value;
				RaisePropertyChanged("IsDelivery");
			}
		}

		#endregion

		#region 里程碑清單
		private List<NameValuePair<string>> _milestoneList;

		public List<NameValuePair<string>> MilestoneList
		{
			get { return _milestoneList; }
			set
			{
				_milestoneList = value;
				RaisePropertyChanged("MilestoneList");
			}
		}

		public void SetMilestoneList()
		{
			var proxy = GetProxy<F19Entities>();
			var query = from item in proxy.F19000102s
						orderby item.MILESTONE_NO
						select new NameValuePair<string>
						{
							Value = item.MILESTONE_NO,
							Name = item.MILESTONE_NAME
						};

			MilestoneList = query.ToList();
		}
		#endregion

		#region 出貨類型
		private NameValuePair<string> _selectedOutTypeItem;

		public NameValuePair<string> SelectedOutTypeItem
		{
			get { return _selectedOutTypeItem; }
			set
			{
				_selectedOutTypeItem = value;
				RaisePropertyChanged("SelectedOutTypeItem");
			}
		}

		#endregion

		#region 新增編輯主檔、明細

		private F190001 _editableF190001;

		public F190001 EditableF190001
		{
			get { return _editableF190001; }
			set
			{
				_editableF190001 = value;
				RaisePropertyChanged("EditableF190001");
			}
		}



		private F19000103 _editableF19000103;
		/// <summary>
		/// 新增時，依照單據類型與單據類別自動帶入的所有里程碑
		/// </summary>
		public F19000103 EditableF19000103
		{
			get { return _editableF19000103; }
			set
			{
				_editableF19000103 = value;
				RaisePropertyChanged("EditableF19000103");
			}
		}

		public void SetF19000103(string ticketType, string ticketClass)
		{
			var proxy = GetProxy<F19Entities>();
			var query = from item in proxy.F19000103s
						where item.TICKET_TYPE == ticketType
						where item.TICKET_CLASS == ticketClass
						select item;

			EditableF19000103 = query.FirstOrDefault();
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
						string gupCode = (SearchGupItem == null || SearchGupItem.Value == "-1") ? null : SearchGupItem.Value;
						string custCode = (SearchCustItem == null || SearchCustItem.Value == "-1") ? null : SearchCustItem.Value;
						string ticketType = SearchTicketTypeItem.Value;
						DoSearch(dcCode, gupCode, custCode, ticketType);
					},
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						if (F190001DataWithF19000103DataList.Any())
						{
							SelectedF190001DataWithF19000103Data = F190001DataWithF19000103DataList.First();
						}
						else
						{
							ShowMessage(Messages.InfoNoData);
						}
					}
					);
			}
		}

		private void DoSearch(string dcCode, string gupCode, string custCode, string ticketType)
		{
			//執行查詢動作

			var proxy = GetExProxy<P71ExDataSource>();
			var query = proxy.CreateQuery<F190001Data>("GetF190001Data")
							 .AddQueryExOption("dcCode", dcCode)
							 .AddQueryExOption("gupCode", gupCode)
							 .AddQueryExOption("custCode", custCode)
							 .AddQueryExOption("ticketType", ticketType);

			var result = query.ToList();


			var list = (from g in result.GroupBy(item => item.TICKET_ID)
						orderby g.Key
						select new F190001DataWithF19000103Data
						{
							F190001Data = g.First(),
							F19000103Data = GetF19000103Data(g)
						}).ToList();

			F190001DataWithF19000103DataList = list;


		}

		/// <summary>
		/// 為了要顯示在查詢結果，將同一主檔的明細設定到同一個類別中，方便 binding
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		private static F19000103Data GetF19000103Data(IGrouping<decimal, F190001Data> g)
		{
			F19000103Data f19000103Data = new F19000103Data();

			foreach (var item in g)
			{
				var noProperty = typeof(F19000103Data).GetProperty("MILESTONE_NO_" + item.SORT_NO);
				noProperty.SetValue(f19000103Data, item.MILESTONE_NO);

				var nameProperty = typeof(F19000103Data).GetProperty("MILESTONE_NO_" + item.SORT_NO + "_NAME");
				nameProperty.SetValue(f19000103Data, item.MILESTONE_NAME);
			}

			return f19000103Data;
		}
		#endregion Search

		#region Add
		public Action OnAddFocus = delegate { };

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
			//執行新增動作
			EditableF190001 = new F190001
			{
				SHIPPING_ASSIGN = "0",
				FAST_DELIVER = "0",
				PRIORITY = 0
			};

			SelectedDcItem = DcList.FirstOrDefault();
			SelectedOutTypeItem = TicketTypeList.FirstOrDefault();
			OnAddFocus();
		}
		#endregion Add

		#region Edit
		public Action OnEditFocus = delegate { };

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && SelectedF190001DataWithF19000103Data != null && EditableF190001 != null
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作

			SetF1947List(EditableF190001.DC_CODE, EditableF190001.GUP_CODE, EditableF190001.CUST_CODE);
			SetF19000103(EditableF190001.TICKET_TYPE, EditableF190001.TICKET_CLASS);
			SelectedF190001DataWithF19000103Data = SelectedF190001DataWithF19000103Data;
			OnEditFocus();
			
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
					},
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						if (ShowMessage(Messages.WarningBeforeCancel) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						DoCancelCompleted();
					}
					);
			}
		}

		private void DoCancelCompleted()
		{
			//執行取消動作

			UserOperateMode = OperateMode.Query;
			EditableF190001 = null;
			EditableF19000103 = null;
			F1947List = null;
			SelectedF190001DataWithF19000103Data = SelectedF190001DataWithF19000103Data;
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
						var msg = GetEditableDataError();
						if (!string.IsNullOrEmpty(msg))
						{
							DialogService.ShowMessage(msg);
							return;
						}

						if (ShowMessage(Messages.WarningBeforeUpdate) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						isSaved = DoSave();

						if (isSaved)
						{
							DoSearch(EditableF190001.DC_CODE, EditableF190001.GUP_CODE, EditableF190001.CUST_CODE, EditableF190001.TICKET_TYPE);
						}
					},
					() => UserOperateMode != OperateMode.Query && EditableF19000103 != null,
					o =>
					{
						if (isSaved)
						{
							SearchDcItem = DcList.FirstOrDefault(item => item.Value == EditableF190001.DC_CODE);
							SearchGupItem = SearchGupList.FirstOrDefault(item => item.Value == EditableF190001.GUP_CODE);
							SearchCustItem = SearchCustList.FirstOrDefault(item => item.Value == EditableF190001.CUST_CODE);
							SearchTicketTypeItem = TicketTypeList.FirstOrDefault(item => item.Value == EditableF190001.TICKET_TYPE);

							EditableF190001 = null;
							EditableF19000103 = null;

							SelectedF190001DataWithF19000103Data = F190001DataWithF19000103DataList.FirstOrDefault();
							UserOperateMode = OperateMode.Query;
						}
					}
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			var proxyF19 = GetProxy<F19Entities>();
			var account = Wms3plSession.CurrentUserInfo.Account;
			var accountName = Wms3plSession.CurrentUserInfo.AccountName;

			if (EditableF190001.SHIPPING_ASSIGN == null)
			{
				EditableF190001.SHIPPING_ASSIGN = "0";
			}

			if (EditableF190001.FAST_DELIVER == null)
			{
				EditableF190001.FAST_DELIVER = "0";
			}

			if (EditableF190001.ASSIGN_DELIVER == "-1")
			{
				EditableF190001.ASSIGN_DELIVER = null;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				AddF190001(proxyF19, account, accountName);

				var addedF190001 = GetLastF190001(proxyF19);

				if (addedF190001 != null && EditableF19000103 != null)
				{
					// 寫入里程碑資料
					AddMilestones(account, accountName, addedF190001);
				}
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				var f190001 = proxyF19.F190001s.Where(item => item.TICKET_ID == EditableF190001.TICKET_ID).FirstOrDefault();

				if (f190001 == null)
				{
					DialogService.ShowMessage(Properties.Resources.P7110010000_ViewModel_TICKET_ID_NotFound);
					return false;
				}

				//只可修改單據名稱、單據類型、單據類別、自動派車、快速到貨、配送商
				EditF190001(proxyF19, account, accountName, f190001);

				if (EditableF19000103 != null)
				{
					// 為了要有修改紀錄... 先刪除要更新不存在的排序里程碑，在找到原本存在的做修改更新
					DeleteNotExistsSortNo(proxyF19, f190001);

					EditMilestones(account, accountName, f190001);
				}
			}

			ShowMessage(Messages.Success);
			return true;
		}

		/// <summary>
		/// 編輯所有有效的里程碑，不存在的則新增
		/// </summary>
		/// <param name="proxyF19"></param>
		/// <param name="account"></param>
		/// <param name="accountName"></param>
		/// <param name="f190001"></param>
		private void EditMilestones(string account, string accountName, F190001 f190001)
		{
			foreach (var p in typeof(F19000103).GetProperties())
			{
				if (!p.Name.StartsWith("MILESTONE_NO"))
				{
					continue;
				}

				var sortNo = p.Name.Substring(p.Name.Length - 1);
				var milestoneNo = Convert.ToString(p.GetValue(EditableF19000103));

				if (string.IsNullOrWhiteSpace(milestoneNo))
				{
					continue;
				}

				F19Entities proxyF19 = GetProxy<F19Entities>();
				var f19000101 = (from item in proxyF19.F19000101s
								 where item.TICKET_ID == f190001.TICKET_ID
								 where item.SORT_NO == sortNo
								 select item).FirstOrDefault();

				if (f19000101 != null)
				{
					f19000101.MILESTONE_NO = milestoneNo;
					f19000101.UPD_DATE = DateTime.Now;
					f19000101.UPD_STAFF = account;
					f19000101.UPD_NAME = accountName;
					proxyF19.UpdateObject(f19000101);
				}
				else
				{
					f19000101 = new F19000101()
					{
						TICKET_ID = f190001.TICKET_ID,
						SORT_NO = sortNo,
						MILESTONE_NO = milestoneNo,
						CRT_DATE = DateTime.Now,
						CRT_STAFF = account,
						CRT_NAME = accountName
					};

					proxyF19.AddToF19000101s(f19000101);
				}
				proxyF19.SaveChanges();
			}

		}

		/// <summary>
		/// 刪除不存在要更新編輯的里程碑排序項目
		/// </summary>
		/// <param name="proxyF19"></param>
		/// <param name="f190001"></param>
		private void DeleteNotExistsSortNo(F19Entities proxyF19, F190001 f190001)
		{
			var allSortNo = GetAllSortNo(EditableF19000103);

			var query = from item in proxyF19.F19000101s
						where item.TICKET_ID == f190001.TICKET_ID
						where !allSortNo.Contains(item.SORT_NO)
						select item;

			var deleteList = query.ToList();
			if (deleteList.Any())
			{
				foreach (var item in deleteList)
				{
					proxyF19.DeleteObject(item);
				}
				proxyF19.SaveChanges();
			}
		}

		/// <summary>
		/// 取得所有有效的里程碑排序字串
		/// </summary>
		/// <param name="f19000103"></param>
		/// <returns></returns>
		private string GetAllSortNo(F19000103 f19000103)
		{
			var query = from p in typeof(F19000103).GetProperties()
						where p.Name.StartsWith("MILESTONE_NO")
						let milestoneNo = Convert.ToString(p.GetValue(f19000103))
						where !string.IsNullOrWhiteSpace(milestoneNo)
						let sortNo = p.Name.Substring(p.Name.Length - 1)
						select sortNo;

			return string.Concat(query);
		}

		/// <summary>
		/// 編輯貨主單據主檔
		/// </summary>
		/// <param name="proxyF19"></param>
		/// <param name="account"></param>
		/// <param name="accountName"></param>
		/// <param name="f190001"></param>
		private void EditF190001(F19Entities proxyF19, string account, string accountName, F190001 f190001)
		{
			f190001.TICKET_NAME = EditableF190001.TICKET_NAME;
			f190001.TICKET_TYPE = EditableF190001.TICKET_TYPE;
			f190001.TICKET_CLASS = EditableF190001.TICKET_CLASS;
			f190001.SHIPPING_ASSIGN = EditableF190001.SHIPPING_ASSIGN;
			f190001.FAST_DELIVER = EditableF190001.FAST_DELIVER;
			f190001.ASSIGN_DELIVER = EditableF190001.ASSIGN_DELIVER;			
			EditableF190001.UPD_DATE = DateTime.Now;
			EditableF190001.UPD_STAFF = account;
			EditableF190001.UPD_NAME = accountName;
			proxyF19.UpdateObject(f190001);
			proxyF19.SaveChanges();
		}

		/// <summary>
		/// 新增貨主單據主檔
		/// </summary>
		/// <param name="proxyF19"></param>
		/// <param name="account"></param>
		/// <param name="accountName"></param>
		private void AddF190001(F19Entities proxyF19, string account, string accountName)
		{
			EditableF190001.CRT_DATE = DateTime.Now;
			EditableF190001.CRT_STAFF = account;
			EditableF190001.CRT_NAME = accountName;

			proxyF19.AddToF190001s(EditableF190001);
			proxyF19.SaveChanges();
		}

		/// <summary>
		/// 新增里程碑
		/// </summary>
		/// <param name="account"></param>
		/// <param name="accountName"></param>
		/// <param name="addedF190001"></param>
		private void AddMilestones(string account, string accountName, F190001 addedF190001)
		{

			foreach (var p in typeof(F19000103).GetProperties())
			{
				if (!p.Name.StartsWith("MILESTONE_NO"))
				{
					continue;
				}

				var sortNo = p.Name.Substring(p.Name.Length - 1);
				var milestoneNo = Convert.ToString(p.GetValue(EditableF19000103));

				if (string.IsNullOrWhiteSpace(milestoneNo))
				{
					continue;
				}

				var f19000101 = new F19000101()
				{
					TICKET_ID = addedF190001.TICKET_ID,
					SORT_NO = sortNo,
					MILESTONE_NO = milestoneNo,
					CRT_DATE = DateTime.Now,
					CRT_STAFF = account,
					CRT_NAME = accountName
				};

				F19Entities proxyF19 = GetProxy<F19Entities>();
				proxyF19.AddToF19000101s(f19000101);
				proxyF19.SaveChanges();
			}

		}

		/// <summary>
		/// 取得最後新增的貨主單據主檔
		/// </summary>
		/// <param name="proxyF19"></param>
		/// <returns></returns>
		private F190001 GetLastF190001(F19Entities proxyF19)
		{
			var f190001 = (from item in proxyF19.F190001s
						   where item.TICKET_TYPE == EditableF190001.TICKET_TYPE
						   where item.TICKET_CLASS == EditableF190001.TICKET_CLASS						   
						   where item.DC_CODE == EditableF190001.DC_CODE
						   where item.GUP_CODE == EditableF190001.GUP_CODE
						   where item.CUST_CODE == EditableF190001.CUST_CODE
						   orderby item.TICKET_ID descending
						   select item).FirstOrDefault();

			return f190001;
		}

		public string GetEditableDataError()
		{

			if (string.IsNullOrEmpty(EditableF190001.DC_CODE) || string.IsNullOrEmpty(EditableF190001.GUP_CODE) || string.IsNullOrEmpty(EditableF190001.CUST_CODE))
			{
				return Properties.Resources.P7110010000_ViewModel_MustSelectDC_Gup_Cust;
			}

			if (string.IsNullOrWhiteSpace(EditableF190001.TICKET_NAME))
			{
				return Properties.Resources.P7110010000_ViewModel_Ticket_Name_Required;
			}

			EditableF190001.TICKET_NAME = EditableF190001.TICKET_NAME.Trim();
			if (EditableF190001.TICKET_NAME.Length > 50)
			{
				return Properties.Resources.P7110010000_ViewModel_TicketName_RequiredAndLimitLength;
			}

			if (string.IsNullOrEmpty(EditableF190001.TICKET_TYPE) || string.IsNullOrEmpty(EditableF190001.TICKET_CLASS))
			{
				return Properties.Resources.P7110010000_ViewModel_MustSelectTicketType;
			}

			if (EditableF19000103 == null)
			{
				return Properties.Resources.P7110010000_ViewModel_MustHaveOneMileStone;
			}

			// 不可新增重複的物流中心+業主+貨主+單據類型+單據類別
			if (IsRepeat() && UserOperateMode == OperateMode.Add)
			{
				return Properties.Resources.P7110010000_ViewModel_RepeatDC_GUP_CUST_TICKET_CannotAddNew;
			}

			return string.Empty;
		}

		public bool IsRepeat()
		{
			var proxy = GetProxy<F19Entities>();
			var query = from item in proxy.F190001s
				where item.DC_CODE == EditableF190001.DC_CODE
				where item.GUP_CODE == EditableF190001.GUP_CODE
				where item.CUST_CODE == EditableF190001.CUST_CODE
				where item.TICKET_TYPE == EditableF190001.TICKET_TYPE
				where item.TICKET_CLASS == EditableF190001.TICKET_CLASS
				select item;

			return query.FirstOrDefault() != null;
		}
		#endregion Save

	}
}
