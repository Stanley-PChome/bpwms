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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices;
using AutoMapper;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P19.ViewModel
{

	public partial class P1901010000_ViewModel : InputViewModelBase
	{

		private bool _isInit = true;
		private string _userId;
		public P1901010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料

				var F00Proxy = GetProxy<F00Entities>();
				this.F000904S_DELV_EFFIC = F00Proxy.F000904s.Where(o => o.TOPIC == "F190102"
																&& o.SUBTOPIC == "DELV_EFFIC").ToObservableCollection();

				if (this.F000904S_DELV_EFFIC.Count > 0)
				{
					for (int i = 0; i < this.F000904S_DELV_EFFIC.Count; i++)
						this.OrderList.Add((i + 1).ToString());
					this.Order_Selected = this.OrderList.First();
				}
				else
				{
					this.OrderList = null;
				}

				if (this.F000904S_DELV_EFFIC.Count > 0)
					this.F000904S_DELV_EFFIC_Selected = this.F000904S_DELV_EFFIC.First();



				_userId = Wms3plSession.Get<UserInfo>().Account;
				InitControls();
			}

		}

		#region **Proc

		#region Master
		/// <summary>
		/// DC清單
		/// </summary>
		private List<F1901> _dCList;
		public List<F1901> DCList
		{
			get { return _dCList; }
			set { _dCList = value; RaisePropertyChanged("DCList"); }
		}


		/// <summary>
		/// 存放當前的DC資訊
		/// </summary>
		private F1901 _currentRecord;
		public F1901 CurrentRecord
		{
			get { return _currentRecord; }
			set
			{
				_currentRecord = value;
				RaisePropertyChanged("CurrentRecord");
			}
		}
		/// <summary>
		/// 原始DC資訊
		/// </summary>
		private F1901 _orgRecord;
		public F1901 OrgRecord { get { return _orgRecord; } set { _orgRecord = value; } }

		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}

		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		#endregion

		#region Service Cust
		/// <summary>
		/// 服務客戶清單
		/// </summary>
		private SelectionList<F1909EX> _dCServiceList;
		public SelectionList<F1909EX> DCServiceList
		{
			get { return _dCServiceList; }
			set { _dCServiceList = value; RaisePropertyChanged("DCServiceList"); }
		}

		//private SelectionList<F1909> _dCServiceList;
		//public SelectionList<F1909> DCServiceList
		//{
		//    get { return _dCServiceList; }
		//    set { _dCServiceList = value; RaisePropertyChanged("DCServiceList"); }
		//}

		/// <summary>
		/// 原始DC已選之服務客戶
		/// </summary>
		private string _orgDCService;
		public string OrgDCService
		{
			get { return _orgDCService; }
			set { _orgDCService = value; }
		}

		private string _Order_Selected = string.Empty;
		public string Order_Selected
		{
			get
			{
				return this._Order_Selected;
			}
			set
			{
				this._Order_Selected = value;
				RaisePropertyChanged("Order_Selected");
			}
		}
		private ObservableCollection<string> _OrderList;
		public ObservableCollection<string> OrderList
		{
			get
			{
				if (this._OrderList == null) _OrderList = new ObservableCollection<string>();
				return _OrderList;
			}
			set
			{
				_OrderList = value;
				RaisePropertyChanged("OrderList");
			}
		}

		private ObservableCollection<F000904> _F000904S_DELV_EFFIC;
		public ObservableCollection<F000904> F000904S_DELV_EFFIC
		{
			get { return _F000904S_DELV_EFFIC; }
			set
			{
				_F000904S_DELV_EFFIC = value;
				RaisePropertyChanged("F000904S_DELV_EFFIC");
			}
		}

		private F000904 _F000904S_DELV_EFFIC_Selected;
		public F000904 F000904S_DELV_EFFIC_Selected
		{
			get { return _F000904S_DELV_EFFIC_Selected; }
			set
			{
				_F000904S_DELV_EFFIC_Selected = value;
				RaisePropertyChanged("F000904S_DELV_EFFIC_Selected");
			}
		}

		private string _Selected_DELV_EFFIC = null;
		public string Selected_DELV_EFFIC
		{
			get
			{
				return this._Selected_DELV_EFFIC;
			}
			set
			{
				this._Selected_DELV_EFFIC = value;
				RaisePropertyChanged("Selected_DELV_EFFIC");
			}
		}

		private bool _dC_CODETextBox_IsEnabled = false;
		public bool dC_CODETextBox_IsEnabled
		{
			get
			{
				return this._dC_CODETextBox_IsEnabled;
			}
			set
			{
				this._dC_CODETextBox_IsEnabled = value;
				RaisePropertyChanged("dC_CODETextBox_IsEnabled");
			}
		}
		#endregion

		#endregion

		#region **Func


		private void InitControls()
		{
			IsBusy = true;
			CreateDCServiceList();
			GetZipCodes();
			DoSearch();
			IsBusy = false;
			_isInit = false;
		}

		private void GetZipCodes()
		{
			var F19proxy = GetProxy<F19Entities>();
			F1934List = F19proxy.F1934s.ToObservableCollection();
			if (F1934List.Any())
			{
				SelectedZipCode = F1934List.First().ZIP_CODE;
				this.F1934_Selected = F1934List.First();

			}

		}

		private bool _IsCanEdit = false;
		public bool IsCanEdit
		{
			get
			{
				return this._IsCanEdit;
			}
			set
			{
				this._IsCanEdit = value;
				RaisePropertyChanged("IsCanEdit");
			}
		}

		private string _AddNewDCCode = string.Empty;
		public string AddNewDCCode
		{
			get
			{
				return this._AddNewDCCode;
			}
			set
			{
				this._AddNewDCCode = value;

			}
		}

		private ObservableCollection<F1934> _F1934List;

		public ObservableCollection<F1934> F1934List
		{
			get { return _F1934List; }
			set
			{
				_F1934List = value;
			}
		}

		private F1934 _F1934_Selected = null;
		public F1934 F1934_Selected
		{
			get
			{
				return this._F1934_Selected;
			}
			set
			{
				this._F1934_Selected = value;
				RaisePropertyChanged("F1934_Selected");
			}
		}

		private string _SelectedZipCode = null;
		public string SelectedZipCode
		{
			get
			{
				return this._SelectedZipCode;
			}
			set
			{
				this._SelectedZipCode = value;
				RaisePropertyChanged("SelectedZipCode");
			}
		}

		private List<NameValuePair<string>> _custCodeList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> CustCodeList
		{
			get { return _custCodeList; }
			set { _custCodeList = value; RaisePropertyChanged("CustCodeList"); }
		}

		private string _selectedCustCode;
		public string SelectedCustCode
		{
			get { return _selectedCustCode; }
			set
			{
				if (value == null) return;
				_selectedCustCode = value; RaisePropertyChanged("SelectedCustCode");
			}
		}

		private ObservableCollection<F190102> _F190102List = null;
		public ObservableCollection<F190102> F190102List
		{
			get
			{
				if (this._F190102List == null)
					this._F190102List = new ObservableCollection<F190102>();
				return this._F190102List;
			}
			set
			{
				this._F190102List = value;
				RaisePropertyChanged("F190102List");
			}
		}
		private F190102 _F190102_Selected = null;
		public F190102 F190102_Selected
		{
			get
			{
				return this._F190102_Selected;
			}
			set
			{
				this._F190102_Selected = value;
				RaisePropertyChanged("F190102_Selected");
			}
		}

		private ObservableCollection<F050006> _F050006List = null;
		public ObservableCollection<F050006> F050006List
		{
			get
			{
				if (this._F050006List == null)
					this._F050006List = new ObservableCollection<F050006>();
				return this._F050006List;
			}
			set
			{
				this._F050006List = value;
				RaisePropertyChanged("F050006List");
			}
		}

		private F050006 _F050006_Selected = null;
		public F050006 F050006_Selected
		{
			get
			{
				return this._F050006_Selected;
			}
			set
			{
				this._F050006_Selected = value;
				RaisePropertyChanged("F050006_Selected");
			}
		}

		public bool ValidateF1912Exists(string LOC_CODE)
		{
			bool ValidResult = false;
			var proxy = GetProxy<F19Entities>();
			var F1912 = proxy.F1912s.Where(o => o.LOC_CODE.Equals(LOC_CODE)).SingleOrDefault();
			ValidResult = (F1912 != null);
			proxy = null;
			return ValidResult;
		}

		public bool ValidateF1912Exists(SelectionList<F1909EX> DCServiceList)
		{
			bool ValidResult = false;
			var proxy = GetProxy<F19Entities>();

			var F1912s = (from F1912 in proxy.F1912s select new { LOC_CODE = F1912.LOC_CODE }).ToList().Select(x => x.LOC_CODE);
			var DCService = DCServiceList.Where(o => string.IsNullOrWhiteSpace(o.Item.LOC_CODE) == false).Select(o => o.Item.LOC_CODE).Distinct().ToList();

			var expectedList = DCService.Except(F1912s);
			ValidResult = (expectedList.Count().Equals(0));

			proxy = null;
			return ValidResult;
		}

		public bool ValidLOCCODEAuthority(string GUP_CODE, string CUST_CODE, string LOC_CODE)
		{
			bool ValidResult = false;
			var proxy = GetProxy<F19Entities>();
			var F1912 = proxy.F1912s.Where(o => o.LOC_CODE.Equals(LOC_CODE)
											 && (o.CUST_CODE.Equals(CUST_CODE) || o.CUST_CODE.Equals("0"))
											 && (o.GUP_CODE.Equals(GUP_CODE) || o.GUP_CODE.Equals("0"))
											 ).SingleOrDefault();

			ValidResult = (F1912 != null);
			return ValidResult;
		}

		public bool ValidLOCCODEAuthority(SelectionList<F1909EX> DCServiceList)
		{

			var proxy = GetProxy<F19Entities>();
			var DCService = DCServiceList.Where(o => string.IsNullOrWhiteSpace(o.Item.LOC_CODE) == false).Select(o => new { LOC_CODE = o.Item.LOC_CODE, GUP_CODE = o.Item.GUP_CODE, CUST_CODE = o.Item.CUST_CODE }).Distinct().ToList();

			foreach (var dc in DCService)
			{
				var F1912 = proxy.F1912s.Where(o => o.LOC_CODE.Equals(dc.LOC_CODE)
										   && (o.CUST_CODE.Equals(dc.CUST_CODE) || o.CUST_CODE.Equals("0"))
										   && (o.GUP_CODE.Equals(dc.GUP_CODE) || o.GUP_CODE.Equals("0"))
										   ).SingleOrDefault();

				if (F1912 == null)
					return false;
			}

			return true;
		}

		/// <summary>
		/// 取得服務客戶
		/// </summary>
		public void CreateDCServiceList()
		{
			//執行查詢動作
			//var proxy = GetProxy<F19Entities>();
			var proxy = GetExProxy<P19ExDataSource>();

			//執行查詢動作                
			DCServiceList = proxy.CreateQuery<F1909EX>("GetP1909EXDatas").ToList().ToSelectionList();
			//DCServiceList = proxy.F1909s.OrderBy(x => x.CUST_CODE).ToList().ToSelectionList();

			proxy = null;
		}
		/// <summary>
		/// 取得已選取的服務客戶
		/// </summary>
		/// <returns></returns>
		private string GetSelectedDCService()
		{
			string result = string.Empty;
			var tmp = DCServiceList.Where(x => x.IsSelected == true).Select(x => x.Item.CUST_CODE).OrderBy(x => x).ToList();
			if (tmp.Any()) result = string.Join(",", tmp);
			return result;
		}
		private void SetData(F1901 data = null)
		{

			var tmp = (data ?? DCList.FirstOrDefault());

			// 先清空資料
			CurrentRecord = null;
			// 設定要顯示的資料
			if (tmp != null)
				CurrentRecord = Mapper.DynamicMap<F1901>(tmp);

			// 將原始資料備份起來, 以備做資料是否有編輯的檢查
			if (CurrentRecord != null)
			{
				OrgRecord = Mapper.DynamicMap<F1901>(CurrentRecord);
			}


			RaisePropertyChanged("CurrentRecord");

			// 沒有資料時不做下列動作
			if (DCList == null || DCList.Count() == 0) return;

			// 如果有資料再Binding新資料進來
			if (CurrentRecord != null)
			{
				// 取得F1909 (DC對應的貨主檔)
				var proxy = GetProxy<F19Entities>();

				var f190101 = proxy.F190101s.Where(x => x.DC_CODE.Equals(CurrentRecord.DC_CODE)).ToList();

				var F190904s = proxy.F190904s.Where(x => x.DC_CODE.Equals(CurrentRecord.DC_CODE)).ToList();

				//走訪所有客戶服務清單
				foreach (var p in DCServiceList)
				{
					p.IsSelected = (f190101.Where(o=>o.DC_CODE == CurrentRecord.DC_CODE).Select(x => x.CUST_CODE).Contains(p.Item.CUST_CODE));
					var F190904 = F190904s.Where(x => x.CUST_CODE.Equals(p.Item.CUST_CODE)
											&& x.GUP_CODE.Equals(p.Item.GUP_CODE)
											&& x.DC_CODE.Equals(CurrentRecord.DC_CODE)
										 ).SingleOrDefault();

					if (F190904 != null)
						p.Item.LOC_CODE = F190904.LOC_CODE;
					else
						p.Item.LOC_CODE = string.Empty;

				}

				CustCodeList = new List<NameValuePair<string>>();
				CustCodeList = (from i in DCServiceList
												where i.IsSelected
												select new NameValuePair<string>
												{
													Name = $"{i.Item.CUST_NAME} {i.Item.CUST_CODE}",
													Value = i.Item.CUST_CODE
												}).ToList();
				SelectedCustCode = CustCode;

				var F05Proxy = GetProxy<F05Entities>();

				this.F050006List = F05Proxy.F050006s.Where(x => x.DC_CODE == CurrentRecord.DC_CODE).OrderBy(x => x.ZIP_CODE).ThenBy(x => x.ZIP_CODE).ToObservableCollection();

				if (this.F050006List.Count > 0)
					F050006_Selected = this.F050006List.First();

				this.F190102List = proxy.F190102s.Where(x => x.DC_CODE == CurrentRecord.DC_CODE).ToObservableCollection();
				if (this.F190102List.Count > 0)
					F190102_Selected = this.F190102List.First();



				// 將服務客戶原始資料備份起來, 以備做資料是否有編輯的檢查
				OrgDCService = GetSelectedDCService();

				proxy = null;
			}
		}

		private DialogResponse ConfirmToUpdate()
		{
			DialogResponse dr = DialogResponse.Yes;
			DataModifyType dmt = IsDataModified();
			if (dmt == DataModifyType.Modified || dmt == DataModifyType.New)
			{
				dr = ShowMessage(Messages.WarningBeforeAdd);
				if (dr == DialogResponse.Yes)
				{
					// 佔測到有修改, 並且同意先儲存時, 要做儲存資料的動作
					DoSave();
				}
			}
			return dr;
		}
		/// <summary>
		/// 操作之前先檢查資料是否有被更改, 以及是否被刪除
		/// </summary>
		/// <returns></returns>
		private DataModifyType IsDataModified()
		{
			if (UserOperateMode == OperateMode.Add) return DataModifyType.New;
			var CurrentService = GetSelectedDCService();
			if (CurrentRecord.DC_CODE != OrgRecord.DC_CODE || CurrentRecord.DC_NAME != OrgRecord.DC_NAME ||
				CurrentRecord.ADDRESS != OrgRecord.ADDRESS || CurrentRecord.TEL != OrgRecord.TEL ||
				CurrentRecord.FAX != OrgRecord.FAX || CurrentRecord.BOSS != OrgRecord.BOSS ||
				CurrentRecord.MAIL_BOX != OrgRecord.MAIL_BOX || CurrentRecord.SHORT_CODE != OrgRecord.SHORT_CODE ||
				CurrentRecord.LAND_AREA != OrgRecord.LAND_AREA || CurrentRecord.BUILD_AREA != OrgRecord.BUILD_AREA)
				return DataModifyType.Modified;
			if (CurrentService != OrgDCService)
				return DataModifyType.Modified;

			return DataModifyType.NotModified;
		}
		private bool isValid()
		{
			if (string.IsNullOrWhiteSpace(CurrentRecord.DC_CODE))
			{
				ShowMessage(Messages.WarningNoDcCode);
				return false;
			}
			if (string.IsNullOrWhiteSpace(CurrentRecord.DC_NAME))
			{
				ShowMessage(Messages.WarningNoDcName);
				return false;
			}

			if (!ValidateF1912Exists(this.DCServiceList))
			{
				ShowMessage(Messages.InfoNoData);
				return false;
			}

			if (!ValidLOCCODEAuthority(this.DCServiceList))
			{
				ShowMessage(Messages.InfoNoData);
				return false;
			}

			return true;
		}
		#endregion

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && CurrentRecord != null,
					o => EditComplate()
					);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
		}

		private void EditComplate()
		{
			//EditAction();
			UserOperateMode = OperateMode.Edit;
			this.IsCanEdit = true;
		}
		#endregion

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

		private bool _IsDoSearch = false;
		private void DoSearch()
		{
			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			//執行查詢動作(取得所有物流中心)
			DCList = proxy.F1901s
			  .OrderBy(x => x.DC_CODE).ToList();
			if ((DCList == null || !DCList.Any()) && !_isInit)
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}

			if (UserOperateMode == OperateMode.Add)
			{

			}

			F1901 F1901 = null;
			if (this.AddNewDCCode + "" != "")
				F1901 = DCList.Where(x => x.DC_CODE.Equals(this.AddNewDCCode)).SingleOrDefault();

			UserOperateMode = OperateMode.Query;
			this.IsCanEdit = false;

			SetData(F1901);
		}


		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoAdd(), () => this.UserOperateMode == OperateMode.Query
				  );
			}
		}

		private void DoAdd()
		{

			// 如果有變更, 或是有新增時, 先確認是否繼續操作
			if (DCList.Any() && ConfirmToUpdate() == DialogResponse.Cancel) return;


			UserOperateMode = OperateMode.Add;

			this.dC_CODETextBox_IsEnabled = UserOperateMode == OperateMode.Add;

			this.IsCanEdit = true;

			//執行新增動作
			SetData(new F1901());
		}
		#endregion Add


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
					// 如果是新增資料, 則將資料還原即可
					SetData();
				else
					// 否則, 從OrgData還原, 並SetData()
					SetData(OrgRecord);
				UserOperateMode = OperateMode.Query;
				this.IsCanEdit = false;
			}
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoDelete(), () => UserOperateMode == OperateMode.Query
				  );
			}
		}

		private void DoDelete()
		{
			// 確認是否要刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			//執行刪除動作

			// 如果是刪除資料, 則必須進行DB操作
			var proxy = GetExProxy<P19ExDataSource>();
			var result = proxy.CreateQuery<ExecuteResult>("DeleteDC")
			  .AddQueryOption("dcCode", string.Format("'{0}'", CurrentRecord.DC_CODE))
			  .ToList();
			ShowMessage(result);
			if (result.First().IsSuccessed)
			{
				// 刪除成功時重新載入資料
				DoSearch();
				SetData();
			}
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoSave(), () => UserOperateMode != OperateMode.Query || (CurrentRecord != null)
				  );
			}
		}

		private void DoSave()
		{
			// 資料未變更時提示訊息
			//if (IsDataModified() == DataModifyType.NotModified)
			//{
			//    ShowMessage(Messages.WarningNotModified);
			//    return;
			//}
			// 檢查資料
			if (!isValid()) return;

			// 儲存資料
			var saveObj = new wcf.F1901()
			{
				DC_CODEk__BackingField = CurrentRecord.DC_CODE,
				DC_NAMEk__BackingField = CurrentRecord.DC_NAME,
				TELk__BackingField = CurrentRecord.TEL,
				FAXk__BackingField = CurrentRecord.FAX,
				ADDRESSk__BackingField = CurrentRecord.ADDRESS,
				LAND_AREAk__BackingField = (short?)CurrentRecord.LAND_AREA,
				BUILD_AREAk__BackingField = (short?)CurrentRecord.BUILD_AREA,
				SHORT_CODEk__BackingField = CurrentRecord.SHORT_CODE,
				BOSSk__BackingField = CurrentRecord.BOSS,
				MAIL_BOXk__BackingField = CurrentRecord.MAIL_BOX,
				ZIP_CODEk__BackingField = CurrentRecord.ZIP_CODE
			};
			List<wcf.F190101> items = new List<wcf.F190101>();
			items = DCServiceList.Where(x => (x.IsSelected)).Select(o => new wcf.F190101
			{
				DC_CODEk__BackingField = CurrentRecord.DC_CODE,
				CUST_CODEk__BackingField = o.Item.CUST_CODE,
				GUP_CODEk__BackingField = o.Item.GUP_CODE
			}
														).ToList();

			List<wcf.F190904> F190904s = new List<wcf.F190904>();
			F190904s = DCServiceList.Where(x => (x.IsSelected) && !string.IsNullOrWhiteSpace(x.Item.LOC_CODE)).Select(o => new wcf.F190904
			{
				DC_CODEk__BackingField = CurrentRecord.DC_CODE,
				CUST_CODEk__BackingField = o.Item.CUST_CODE,
				GUP_CODEk__BackingField = o.Item.GUP_CODE,
				LOC_CODEk__BackingField = o.Item.LOC_CODE
			}
			).ToList();
			List<wcf.F050006> F050006s = new List<wcf.F050006>();
			F050006s = this.F050006List.Select(o => new wcf.F050006
			{
				DC_CODEk__BackingField = CurrentRecord.DC_CODE,
				CUST_CODEk__BackingField = o.CUST_CODE,
				GUP_CODEk__BackingField = o.GUP_CODE,
				ZIP_CODEk__BackingField = o.ZIP_CODE,
			}
		   ).ToList();

			List<wcf.F190102> F190102s = new List<wcf.F190102>();
			F190102s = this.F190102List.Select(o => new wcf.F190102
			{
				DC_CODEk__BackingField = CurrentRecord.DC_CODE,
				DELV_EFFICk__BackingField = o.DELV_EFFIC,
				SORTk__BackingField = o.SORT,

			}
		   ).ToList();

			var proxy = new wcf.P19WcfServiceClient();
			wcf.ExecuteResult result = new wcf.ExecuteResult() { IsSuccessed = true };

			//if (UserOperateMode == OperateMode.Add)
			//    result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertP190101(saveObj, items.ToArray(), this._userId));
			//else
			//    result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateP190101(saveObj, items.ToArray(), this._userId));

			if (UserOperateMode == OperateMode.Add)
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertP190101New1(saveObj, items.ToArray(), F190904s.ToArray(), F050006s.ToArray(), F190102s.ToArray(), this._userId, GupCode, CustCode));
				this.AddNewDCCode = saveObj.DC_CODEk__BackingField;
			}
			else
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateP190101New1(saveObj, items.ToArray(), F190904s.ToArray(), F050006s.ToArray(), F190102s.ToArray(), this._userId, GupCode, CustCode));


			if (result.IsSuccessed == true)
			{
				// 存檔完重新查詢
				var tmpDC = CurrentRecord;
				DoSearch();
				// 更新到目前選取的資料暫存檔
				//SetData(tmpDC);
				this.dC_CODETextBox_IsEnabled = UserOperateMode == OperateMode.Query;
			}

			ShowResultMessage(result);
		}
		#endregion Save

		public void F050006DoDelete()
		{

		}

		public ICommand F050006DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => F050006DoDelete(),
				  () => F050006DoDeleteBefore(),
				  o => F050006DoDeleteComplate()
				  );
			}
		}

		public bool F050006DoDeleteBefore()
		{
			return ((UserOperateMode == OperateMode.Query || UserOperateMode == OperateMode.Add)
					&& this.F050006_Selected != null && this.F050006List.Any());
		}

		public bool F190102DoDeleteBefore()
		{
			return ((UserOperateMode == OperateMode.Query || UserOperateMode == OperateMode.Add)
					&& this.F190102_Selected != null && this.F190102List.Any());
		}

		public void F190102DoDelete()
		{

		}

		public void F190102DoDeleteComplate()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
				return;
			if (this.F190102_Selected != null)
			{
				this.F190102List.Remove(this.F190102_Selected);
				this.F190102_Selected = null;
			}

		}

		public void F050006DoDeleteComplate()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

			if (this.F050006_Selected != null)
			{
				this.F050006List.Remove(this.F050006_Selected);
				this.F050006_Selected = null;
			}

		}

		public void F190102_DELV_EFFIC_DoAddComplate()
		{

			if (this.F190102List.Where(x => x.DC_CODE == this.CurrentRecord.DC_CODE
									   && x.DELV_EFFIC == this.Selected_DELV_EFFIC
									   && x.SORT.ToString() == this.Order_Selected).Any())
			{
				DialogService.ShowMessage(Properties.Resources.P1901010000_DELV_EFFIC_EXIST);
				return;
			}

			var DELV_EFFICs = this.F190102List.GroupBy(x => x.DELV_EFFIC).Select(g =>
							  new { DELV_EFFIC = g.Key, Count = g.Count() }).Where(x => x.DELV_EFFIC.ToString() == this.Selected_DELV_EFFIC.ToString() && x.Count > 0).ToList();
			if (DELV_EFFICs.Count > 0)
			{
				DialogService.ShowMessage(Properties.Resources.P1901010000_DELV_EFFIC_DUPLICATE);
				return;
			}

			var SORTs = this.F190102List.GroupBy(x => x.SORT).Select(g =>
								new { SORT = g.Key, Count = g.Count() }).Where(x => x.SORT.ToString() == this.Order_Selected.ToString() && x.Count > 0).ToList();
			if (SORTs.Count > 0)
			{
				DialogService.ShowMessage(Properties.Resources.P1901010000_DELV_EFFIC_ORDER_DUPLICATE);
				return;
			}

			F190102 NewF190102 = new F190102
			{
				DC_CODE = this.CurrentRecord.DC_CODE,
				DELV_EFFIC = this.Selected_DELV_EFFIC,
				SORT = Int16.Parse(this.Order_Selected)
			};
			this.F190102List.Add(NewF190102);
			RaisePropertyChanged("F190102List");
		}

		public void F190102_DELV_EFFIC_DoAdd()
		{
		}

		public ICommand F190102_DELV_EFFIC_DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => F190102DoDelete(),
				  () => F190102DoDeleteBefore(),
				  o => F190102DoDeleteComplate()
				  );
			}
		}

		public ICommand F190102_DELV_EFFIC_AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				   o => F190102_DELV_EFFIC_DoAdd(),
				   () => F190102_DELV_EFFIC_DoAddBefore(),
				   o => F190102_DELV_EFFIC_DoAddComplate()
				   );
			}
		}

		public bool F190102_DELV_EFFIC_DoAddBefore()
		{
			return ((UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add)
					&& this.CurrentRecord.DC_CODE + "" != "");
		}

		public void F050006DoAdd()
		{
		}

		public void F050006DoAddComplate()
		{
			if (this.F050006List.Where(x => x.ZIP_CODE == this.SelectedZipCode && x.CUST_CODE == SelectedCustCode).Any())
			{
				DialogService.ShowMessage(Properties.Resources.P1901010000_ZIP_CODE_EXIST);
				return;
			}
			F050006 NewF050006 = new F050006
			{
				ZIP_CODE = this.SelectedZipCode,
				DC_CODE = this.CurrentRecord.DC_CODE,
				GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode,
				CUST_CODE = SelectedCustCode ?? Wms3plSession.Get<GlobalInfo>().CustCode
			};
			this.F050006List.Add(NewF050006);
			RaisePropertyChanged("F050006List");
		}

		public ICommand F050006AddCommand
		{
			get
			{

				return CreateBusyAsyncCommand(
				   o => F050006DoAdd(), () => F050006DoAddBefore(), o => F050006DoAddComplate()
				   );
			}
		}

		public bool F050006DoAddBefore()
		{
			return ((UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add)
					&& this.CurrentRecord.DC_CODE + "" != "");
		}

		#region 上下筆移動
		public ICommand MoveFirstCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o =>
				  {
					  DoMoveRecord(RecordMoveType.First);
				  },
				  () => UserOperateMode == OperateMode.Query && !IsFirstRecord() && DCList.Any()
				);
			}
		}
		public ICommand MovePreviousCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o =>
				  {
					  DoMoveRecord(RecordMoveType.Previous);
				  },
				  () => UserOperateMode == OperateMode.Query && !IsFirstRecord() && DCList.Any()
				);
			}
		}
		public ICommand MoveNextCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o =>
				  {
					  DoMoveRecord(RecordMoveType.Next);
				  },
				  () => UserOperateMode == OperateMode.Query && !IsLastRecord() && DCList.Any()
				);
			}
		}
		public ICommand MoveLastCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o =>
				  {
					  DoMoveRecord(RecordMoveType.Last);
				  },
					() => UserOperateMode == OperateMode.Query && !IsLastRecord() && DCList.Any()
				);
			}
		}

		private void DoMoveRecord(RecordMoveType type)
		{
			if (ConfirmToUpdate() == DialogResponse.Cancel) return;
			switch (type)
			{
				case RecordMoveType.First:
					SetData(DCList.FirstOrDefault());
					break;
				case RecordMoveType.Previous:
					SetData(DCList.LastOrDefault(x => string.Compare(CurrentRecord.DC_CODE, x.DC_CODE, StringComparison.Ordinal) > 0));
					break;
				case RecordMoveType.Next:
					SetData(DCList.FirstOrDefault(x => string.Compare(x.DC_CODE, CurrentRecord.DC_CODE, StringComparison.Ordinal) > 0));
					break;
				case RecordMoveType.Last:
					SetData(DCList.LastOrDefault());
					break;
			}
		}

		private bool IsFirstRecord()
		{
			if (CurrentRecord == null) return true;
			if (DCList.FindIndex(x => x.DC_CODE.Equals(CurrentRecord.DC_CODE)) == 0) return true;
			return false;
		}
		private bool IsLastRecord()
		{
			if (CurrentRecord == null) return true;
			if (DCList.FindIndex(x => x.DC_CODE.Equals(CurrentRecord.DC_CODE)) == DCList.Count() - 1) return true;
			return false;
		}
		#endregion

		private ICommand _dcCodeCheckCommand = null;

		public ICommand DcCodeCheckCommand
		{
			get
			{

				return _dcCodeCheckCommand ?? (_dcCodeCheckCommand = CreateBusyAsyncCommand(
				   o => DoDcCodeCheck(),
				   () => UserOperateMode == OperateMode.Add && CurrentRecord != null
				   ));
			}
		}

		private void DoDcCodeCheck()
		{
			var proxy = GetProxy<F19Entities>();
			if (proxy.F1901s.Where(x => x.DC_CODE == CurrentRecord.DC_CODE).Count() > 0)
			{
				ShowWarningMessage(Properties.Resources.P1901010000_DC_CODE_EXIST);
			}
		}
	}


	public class DELVEFFICCodeToDELVEFFICNameConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter,
				System.Globalization.CultureInfo culture)
		{

			P1901010000_ViewModel P1901010000_ViewModel = (P1901010000_ViewModel)parameter;

			string DELVEFFICCode = (string)value;

			var F000904 = P1901010000_ViewModel.F000904S_DELV_EFFIC.Where(x => x.VALUE.ToString() == DELVEFFICCode).SingleOrDefault();

			return (F000904 == null ? value : F000904.NAME);
		}

		public object ConvertBack(object value, Type targetType, object parameter,
				System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}


	public class ZipCodeToZipNameConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter,
				System.Globalization.CultureInfo culture)
		{

			P1901010000_ViewModel P1901010000_ViewModel = (P1901010000_ViewModel)parameter;

			string ZipCode = (string)value;

			var F1934 = P1901010000_ViewModel.F1934List.Where(x => x.ZIP_CODE.ToString() == ZipCode).SingleOrDefault();

			return (F1934 == null ? value : F1934.ZIP_NAME);
		}

		public object ConvertBack(object value, Type targetType, object parameter,
				System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}


}
