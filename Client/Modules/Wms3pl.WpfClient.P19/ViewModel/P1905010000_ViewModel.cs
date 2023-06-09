using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;

using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using AutoMapper;
using Wms3pl.WpfClient.DataServices.F00DataService;
using System.Text.RegularExpressions;
using Wms3pl.WpfClient.DataServices.F06DataService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1905010000_ViewModel : InputViewModelBase
	{
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public F1901 OrgDC;
		private string _userId;
		private bool _isInit = true;

		/// <summary>
		/// 搜尋結果是否有資料
		/// </summary>
		public Visibility DataVisibility
		{
			get
			{
				return (CurrentRecord != null) ? Visibility.Visible : Visibility.Hidden;
			}
		}

		public P1905010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				InitControls();
			}
		}

		private void InitControls()
		{
			IsBusy = true;
			DoSearchGroup();
			DoSearchDc();
			DoSearchCustList();
			DoSearchDepList();
			DoSearch();
			IsBusy = false;
			_isInit = false;
			SetMenuStyleList();
		}

		#region 資料連結/ 頁面參數
		public bool IsNewRecord
		{
			get { return UserOperateMode == OperateMode.Add; }
		}
		private bool _isEnableEdit;
		public bool IsEnableEdit
		{
			get { return _isEnableEdit || CurrentRecord.ISDELETED == "0"; }
			set { _isEnableEdit = value; RaisePropertyChanged("IsEnableEdit"); }
		}

		public bool CurrentRecordIsNotDelete
		{
			get
			{
				return CurrentRecord != null && CurrentRecord.ISDELETED != "1";
			}
		}

		private bool _isCheckedAll;

		public bool IsCheckedAll
		{
			get { return _isCheckedAll; }
			set
			{
				_isCheckedAll = value;
				RaisePropertyChanged("IsCheckedAll");
			}
		}



		public ICommand CheckedAllCommand
		{
			get
			{
				return new RelayCommand(() => CheckedAll(), () => { return UserOperateMode != OperateMode.Query; });
			}
		}

		private void CheckedAll()
		{
			foreach (var item in GroupList)
			{
				item.IsSelected = IsCheckedAll;
			}
		}


		/// <summary>
		/// 帳號狀態
		/// </summary>
		public string AccountStatus
		{
			get
			{
				if (CurrentRecord == null) return string.Empty;
				if (CurrentRecord.ISDELETED == "1") return Properties.Resources.P1905010000_Deleted;
				return Properties.Resources.P1905010000_Using;
			}
		}
		public string AccountStatusColor
		{
			get
			{
				if (CurrentRecord == null) return "Black";
				if (CurrentRecord.ISDELETED == "1") return "Red";
				return "Black";
			}
		}

		#region Form - 搜尋條件
		private string _searchEmpId = string.Empty;
		public string SearchEmpId
		{
			get { return _searchEmpId; }
			set
			{
				_searchEmpId = value;
				RaisePropertyChanged("SearchEmpId");
			}
		}
		#endregion
		#region Data - 貨主
		private P190501TreeView _selectedCust;

		public P190501TreeView SelectedCust
		{
			get { return _selectedCust; }
			set
			{
				_selectedCust = value;
				RaisePropertyChanged("SelectedCust");
			}
		}


		private List<P190501TreeView> _custList;
		public List<P190501TreeView> CustList
		{
			get { return _custList; }
			set { _custList = value; RaisePropertyChanged("CustList"); }
		}
		#endregion

		#region  Data - 物流中心
		private F1901 _dcCode;

		public F1901 DcCode
		{
			get { return _dcCode; }
			set
			{
				Set(() => DcCode, ref _dcCode, value);
				DoSearchGup();
			}
		}
		#endregion


		#region Data - 業主
		private F1929 _gupCode;
		public F1929 GupCode
		{
			get { return _gupCode; }
			set
			{
				//if (ConfirmBeforeChangeGUP() == DialogResponse.Cancel)
				//{
				//	value = _gupCode;
				//	return;
				//}
				_gupCode = value;
				RaisePropertyChanged("GupCode");
				// 選擇業主後, 更新TreeView內容
				DoSearchCustList();
				// 預設選擇第一筆
				SelectedCust = CustList.FirstOrDefault();
				if (CurrentRecord != null) SetData(CurrentRecord);
			}
		}
		public DialogResponse ConfirmBeforeChangeDC()
		{
			DialogResponse dr = DialogResponse.Yes;
			if (CurrentRecord == null) return dr;

			if (UserOperateMode == OperateMode.Edit)
			{
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
			}
			else if (UserOperateMode == OperateMode.Add)
			{
				if (CustsContentIsModified == DataModifyType.Modified)
				{
					DialogService.ShowMessage(Properties.Resources.P1905010000_DC_Gup_Changed, Properties.Resources.P1905010000_Choose_DC_Gup_Changed, DialogButton.OK, DialogImage.Information);

					// 畫面不動
					return DialogResponse.Cancel;
				}
			}

			return dr;
		}
		public DialogResponse ConfirmBeforeChangeGUP()
		{
			DialogResponse dr = DialogResponse.Yes;
			if (CurrentRecord == null) return dr;

			if (UserOperateMode == OperateMode.Edit)
			{
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
			}
			else if (UserOperateMode == OperateMode.Add)
			{
				if (CustsContentIsModified == DataModifyType.Modified)
				{
					DialogService.ShowMessage(Properties.Resources.P1905010000_DC_Gup_Changed, Properties.Resources.P1905010000_Choose_DC_Gup_Changed, DialogButton.OK, DialogImage.Information);

					// 畫面不動
					return DialogResponse.Cancel;
				}
			}

			return dr;
		}
		//private string _gupId = string.Empty;
		//public string GupId
		//{
		//	get { return _gupId; }
		//	set { _gupId = value; }
		//}

		private List<F1901> _dcList;
		public List<F1901> DcList
		{
			get { return _dcList; }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}

		private List<F1929> _gupList;
		public List<F1929> GupList
		{
			get { return _gupList; }
			set { _gupList = value; RaisePropertyChanged("GupList"); }
		}
		#endregion
		#region Data - 單位
		private List<NameValuePair<string>> _depList;

		public List<NameValuePair<string>> DepList
		{
			get { return _depList; }
			set
			{
				_depList = value;
				RaisePropertyChanged("DepList");
			}
		}
		#endregion
		#region Data - 工作群組
		private SelectionList<F1953> _groupList;
		public SelectionList<F1953> GroupList
		{
			get { return _groupList; }
			set { _groupList = value; RaisePropertyChanged("GroupList"); }
		}
		#endregion
		#region Data - 員工清單
		/// <summary>
		/// 用在主檔, 主檔選擇後 (按上下筆), 再去DB取得最新詳細資料, 放入到CurrentEmp
		/// </summary>
		private List<F1924> _empList;
		public List<F1924> EmpList
		{
			get { return _empList; }
			set { _empList = value; IsEnableEdit = value.Any(); RaisePropertyChanged("EmpList"); }
		}
		#endregion
		#region Data - 原始資料/ 修改的資料
		/// <summary>
		/// 存放當前的Employee資訊
		/// </summary>
		private F1924 _currentRecord;
		public F1924 CurrentRecord
		{
			get { return _currentRecord; }
			set
			{
				_currentRecord = value;
				RaisePropertyChanged("CurrentRecord");
				RaisePropertyChanged("CurrentRecordIsNotDelete");
				RaisePropertyChanged("DataVisibility");
			}
		}
		private F1924 _orgRecord;
		public F1924 OrgRecord { get { return _orgRecord; } set { _orgRecord = value; } }
		private string _orgCusts = string.Empty;
		public string OrgCusts { get { return _orgCusts; } set { _orgCusts = value; } }
		private string _orgGroups = string.Empty;
		public string OrgGroups { get { return _orgGroups; } set { _orgGroups = value; } }
		#endregion


		#region 新選單名稱
		private string _newMenuName;

		public string NewMenuName
		{
			get { return _newMenuName; }
			set
			{
				Set(() => NewMenuName, ref _newMenuName, value);
			}
		}
		#endregion


		#region 選單樣式
		private List<NameValuePair<string>> _menuStyleList;

		public List<NameValuePair<string>> MenuStyleList
		{
			get { return _menuStyleList; }
			set
			{
				Set(() => MenuStyleList, ref _menuStyleList, value);
			}
		}
		#endregion


		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query
				);
			}
		}

		private void DoSearch()
		{
			SearchEmpId = SearchEmpId.Trim();

			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			//執行查詢動作，為了上下筆，直接撈出所有員工，搜尋員工改在 Client 做
			EmpList = proxy.F1924s//.Where(x => (x.EMP_ID.Equals(SearchEmpId) || String.IsNullOrEmpty(SearchEmpId)))
				.OrderBy(x => x.EMP_ID).ToList();

			F1924 f1924 = null;
			if (EmpList != null)
			{
				f1924 = EmpList.FirstOrDefault(item => item.EMP_ID == SearchEmpId);
			}
			if (f1924 == null && !_isInit)
			{
				ClearShowData();
				ShowMessage(Messages.InfoNoData);
				//return;
			}
			SetData(f1924);
		}
		/// <summary>
		/// 取得詳細資料, 若傳入的data有值則取該employee的資料, 否則取EmpList首筆資料
		/// </summary>
		/// <param name="data"></param>
		private void SetData(F1924 data = null)
		{
			var tmp = (data ?? EmpList.FirstOrDefault());
			// 先清空資料
			ClearShowData();

			// 設定要顯示的資料
			if (tmp != null)
				CurrentRecord = Mapper.DynamicMap<F1924>(tmp);

			// 將原始資料備份起來, 以備做資料是否有編輯的檢查
			if (CurrentRecord != null)
				OrgRecord = Mapper.DynamicMap<F1924>(CurrentRecord);

			//RaisePropertyChanged("CurrentRecord");
			RaisePropertyChanged("AccountStatus");
			RaisePropertyChanged("AccountStatusColor");
			RaisePropertyChanged("IsNewRecord");

			// 沒有資料時不做下列動作
			if (EmpList == null || EmpList.Count() == 0) return;

			// 如果有資料再Binding新資料進來
			if (CurrentRecord != null)
			{
				// 取得F192401 (員工對應的作業群組)
				var proxy = GetProxy<F19Entities>();
				var f192401 = proxy.F192401s.Where(x => x.EMP_ID.Equals(CurrentRecord.EMP_ID)).ToList().AsQueryable();
				foreach (var p in f192401)
				{
					var tmp2 = GroupList.FirstOrDefault(x => x.Item.GRP_ID.Equals(p.GRP_ID));
					if (tmp2 != null) tmp2.IsSelected = true;

				}
				// 
				//OrgGroups = string.Join(",", f192401.OrderBy(x => x.GRP_ID).Select(x => x.GRP_ID).ToList());
				OrgGroups = GetSelectedGroups();
				// 取得F192402 (員工對應的貨主), 只取當前選定的業主
				var f192402 = proxy.F192402s.Where(x => x.EMP_ID.Equals(CurrentRecord.EMP_ID)).AsQueryable().ToList();
                if (GupCode != null)
                    f192402 = f192402.Where(x => x.GUP_CODE.Equals(GupCode.GUP_CODE)).ToList();

                foreach (var p in f192402)
				{
					SetTreeViewItemChecked(CustList, p.CUST_CODE, p.DC_CODE);
				}
				// 
				OrgCusts = string.Join(",", f192402.Select(x => x.DC_CODE + "|" + x.CUST_CODE).OrderBy(x => x).ToList());
				proxy = null;
			}
			//IsDelete = tmp.ISDELETED == "1";
		}

		private void ClearShowData()
		{
			CurrentRecord = null;
			IsCheckedAll = false;
			foreach (var p in GroupList)
			{
				p.IsSelected = false;
			}
			ResetTreeViewDefaultNonSelected(CustList);
			OrgGroups = string.Empty;
			OrgCusts = string.Empty;
		}
		/// <summary>
		/// 取得工作群組
		/// </summary>
		private void DoSearchGroup()
		{
			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			//執行查詢動作
			GroupList = proxy.F1953s.Where(x => x.ISDELETED == "0").OrderBy(x => x.GRP_ID).ToList().ToSelectionList();
			proxy = null;
		}

		private void DoSearchDc()
		{
			var proxy = GetProxy<F19Entities>();
			var list = proxy.F1901s.OrderBy(x => x.DC_CODE).ToList();
			list.Insert(0, new F1901 { DC_CODE = "", DC_NAME = Resources.Resources.All });
			DcList = list;
			DcCode = DcList.FirstOrDefault();
			proxy = null;
		}
		/// <summary>
		/// 取得業主清單
		/// </summary>
		private void DoSearchGup()
		{
			var proxy = GetProxy<F19Entities>();
			var gupData = proxy.F1929s.OrderBy(x => x.GUP_CODE).ToList();
			if (DcCode != null && string.IsNullOrWhiteSpace(DcCode.DC_CODE))
			{
				GupList = gupData;
			}
			else
			{
				var list = proxy.F190101s.Where(x=> x.DC_CODE == DcCode.DC_CODE).ToList().Select(x=>x.GUP_CODE).Distinct().ToList();
				GupList = gupData.Where(x => list.Any(y => y == x.GUP_CODE)).ToList();
			}
			// 預選到第一項
			//GupId = (GupList.Any() ? GupList.First().GUP_CODE: String.Empty);
			GupCode = GupList.FirstOrDefault();
			proxy = null;
		}
		/// <summary>
		/// 傳回貨主所屬的物流中心 (F190101), 包含DC層
		/// </summary>
		private void DoSearchCustList()
		{
			var treeViewService = new P190501TreeViewService();

			// 透過TreeViewService取回資料
			IEnumerable<P190501TreeView> _all = null;
			if (GupCode != null)
			{
				_all = treeViewService.AllItems(DcCode.DC_CODE,GupCode.GUP_CODE, FunctionCode);
				CustList = treeViewService.MakeTree(_all).ToList();
			}
			else
				CustList = new List<P190501TreeView>();
			_all = null;
			treeViewService = null;
		}
		/// <summary>
		/// 搜尋所有單位編號與名稱
		/// </summary>
		private void DoSearchDepList()
		{
			var proxy = GetProxy<F19Entities>();
			DepList = proxy.F1925s.ToList().Select(item => new NameValuePair<string>()
			{
				Name = string.Format("{0}-{1}", item.DEP_ID, item.DEP_NAME),
				Value = item.DEP_ID
			}).ToList();

		}
		#endregion Search


		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						DoAdd();
						AddAction();
					},
					() => UserOperateMode == OperateMode.Query
				);
			}
		}


		private void DoAdd()
		{
			// 如果有變更, 或是有新增時, 先確認是否繼續操作
			SetMenuStyleList();
			UserOperateMode = OperateMode.Add;
			IsEnableEdit = true;
			SetData(new F1924() { MENUSTYLE = "0" });
			NewMenuName = "";
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && CurrentRecordIsNotDelete
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			NewMenuName = "";
			//執行編輯動作
			EditAction();
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() => UserOperateMode != OperateMode.Query && CurrentRecordIsNotDelete
				);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				SetMenuStyleList();
				if (UserOperateMode == OperateMode.Add)
				{
					IsEnableEdit = EmpList.Any();
					// 如果是新增資料, 則將資料還原即可
					SetData();
				}
				else
				{
					// 否則, 從OrgData還原, 並SetData()
					SetData(OrgRecord);
				}
				UserOperateMode = OperateMode.Query;
				NewMenuName = "";
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
					() => UserOperateMode == OperateMode.Query && EmpList != null && EmpList.Any() && CurrentRecordIsNotDelete
				);
			}
		}

		private void DoDelete()
		{
			// 確認是否要刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			//執行刪除動作

			// 如果是刪除資料, 則必須進行DB操作
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1924s.Where(x => x.EMP_ID == CurrentRecord.EMP_ID).FirstOrDefault();
			if (data == null) ShowMessage(Messages.WarningBeenDeleted);
			else
			{
                wcf.ExecuteResult result = new wcf.ExecuteResult() { IsSuccessed = true };
                var proxyWcf = new wcf.P19WcfServiceClient();
                result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
                () => proxyWcf.DeleteP190501(CurrentRecord.EMP_ID));

                if (result.IsSuccessed)
                {
                    ShowMessage(Messages.InfoDeleteSuccess);
                    SearchEmpId = CurrentRecord.EMP_ID;
                    DoSearch();
                }
                else
                {
                    ShowMessage(Messages.WarningCannotDelete);
                }
			}
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode != OperateMode.Query && CurrentRecordIsNotDelete
				);
			}
		}

		private void DoSave()
		{
			ExDataMapper.Trim(CurrentRecord);

			// 資料未變更時提示訊息
			if (IsDataModified() == DataModifyType.NotModified)
			{
				ShowMessage(Messages.WarningNotModified);
				return;
			}

			// 檢查資料
			if (string.IsNullOrEmpty(CurrentRecord.EMP_ID))
			{
				ShowMessage(Messages.WarningNoEmpID);
				return;
			}
			else if (!ValidateHelper.IsMatchAZaz09(CurrentRecord.EMP_ID))
			{
				DialogService.ShowMessage(Properties.Resources.P1905010000_EmployeeNo_OnlyCNWord);
				return;
			}
			else if (string.IsNullOrEmpty(CurrentRecord.EMP_NAME))
			{
				ShowMessage(Messages.WarningNoEmpName);
				return;
			}
			else if (string.IsNullOrEmpty(CurrentRecord.DEP_ID))
			{
				DialogService.ShowMessage(Properties.Resources.P1905010000_UnitChoose_Required);
				return;
			}
			else if (!string.IsNullOrEmpty(CurrentRecord.MOBILE) && !ValidateHelper.IsMatchPhone(CurrentRecord.MOBILE))
			{
				DialogService.ShowMessage(Properties.Resources.P1905010000_CelNumberFormatError);
				return;
			}
			else if (!string.IsNullOrEmpty(CurrentRecord.SHORT_MOBILE) && !ValidateHelper.IsMatchPhone(CurrentRecord.SHORT_MOBILE))
			{
				DialogService.ShowMessage(Properties.Resources.P1905010000_CelShortNumFormatError);
				return;
			}
			else if (!string.IsNullOrEmpty(CurrentRecord.TEL_EXTENSION) && !ValidateHelper.IsMatchPhone(CurrentRecord.TEL_EXTENSION))
			{
				DialogService.ShowMessage(Properties.Resources.P1905010000_ExtensionFormatError);
				return;
			}

			if(CurrentRecord.MENUSTYLE == "1" && string.IsNullOrWhiteSpace(CurrentRecord.MENU_CODE))
			{
				DialogService.ShowMessage(Properties.Resources.P1905010000_MENU_CODE_Required);
				return;
			}
			if (CurrentRecord.MENUSTYLE == "1" && !string.IsNullOrWhiteSpace(CurrentRecord.MENU_CODE) && !string.IsNullOrWhiteSpace(NewMenuName))
			{
				var item = MenuStyleList.Find(x => x.Value == CurrentRecord.MENU_CODE);
				if (ShowConfirmMessage(string.Format(Properties.Resources.P1905010000_CopyToNewMenuName, item.Name, NewMenuName)) == DialogResponse.No)
					return;
			}

            List<wcf.F192402> tmpF192402 = GetSelectedCustsForWcf(CustList);
            if (!tmpF192402.Any())
            {
                DialogService.ShowMessage(Properties.Resources.P1905010000_DC_CODE_Required);
                return;
            }

            // 執行確認儲存動作
            if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes) return;

			// 先檢查是否主檔已被刪除
			var proxy2 = GetProxy<F19Entities>();

			var tmpCheck = proxy2.F1924s.Where(x => x.EMP_ID.Equals(CurrentRecord.EMP_ID) && !x.ISDELETED.Equals("1")).ToList().AsQueryable();
			if (UserOperateMode != OperateMode.Add && !tmpCheck.Any())
			{
				ShowMessage(Messages.WarningBeenDeleted);
				return;
			}

			// 儲存資料
			string[] groups = GetSelectedGroups().Split(',');
			wcf.ExecuteResult result = new wcf.ExecuteResult() { IsSuccessed = true };
            var proxy = new wcf.P19WcfServiceClient();
			if (UserOperateMode == OperateMode.Add)
			{
				var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
				() => proxy.InsertP190501(gupCode, custCode,
				ExDataMapper.Map<F1924, wcf.F1924>(CurrentRecord), groups, tmpF192402.ToArray(), this._userId,NewMenuName,DcCode.DC_CODE));

				if (result.IsSuccessed == true)
				{
					// 新增完重新查詢
					var tmpEmp = CurrentRecord;
					SearchEmpId = CurrentRecord.EMP_ID;
					DoSearch();
					if (!string.IsNullOrWhiteSpace(NewMenuName))
						SetMenuStyleList();
					// 更新到目前選取的資料暫存檔
					SetData(EmpList.Find(x => x.EMP_ID == tmpEmp.EMP_ID));
					UserOperateMode = OperateMode.Query;
				}
			}
			else
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateP190501(ExDataMapper.Map<F1924, wcf.F1924>(CurrentRecord)
					, groups, tmpF192402.ToArray(), GupCode.GUP_CODE, this._userId, NewMenuName, DcCode.DC_CODE));

				// 儲存成功後更新EmpList的資料
				if (result.IsSuccessed == true)
				{
					proxy2 = GetProxy<F19Entities>(); // 必須要重抓一次Proxy, 否則會用到舊資料 (即使dataservice回傳的是新資料)
					var tmp = proxy2.F1924s.Where(x => x.EMP_ID == CurrentRecord.EMP_ID).FirstOrDefault();
					// 找到在EMPLIST裡的資料, 將之取代成新的資料
					var idx = EmpList.IndexOf(EmpList.Find(x => x.EMP_ID == CurrentRecord.EMP_ID));
					EmpList.RemoveAt(idx);
					EmpList.Insert(idx, tmp);
					if (!string.IsNullOrWhiteSpace(NewMenuName))
						SetMenuStyleList();
					// 更新到目前選取的資料暫存檔
					SetData(tmp);
					UserOperateMode = OperateMode.Query;
				}
			}
			RaisePropertyChanged("IsNewRecord");

			ShowMessage(new List<ExecuteResult>() {
				new ExecuteResult() { IsSuccessed = result.IsSuccessed, Message = result.Message }
			});
			proxy = null;
			

			NewMenuName = "";
		}

		/// <summary>
		/// 操作之前先檢查資料是否有被更改, 以及是否被刪除
		/// </summary>
		/// <returns></returns>
		private DataModifyType IsDataModified()
		{
			if (UserOperateMode == OperateMode.Add) return DataModifyType.New;

			if (CurrentRecordIsModified == DataModifyType.Modified)
				return DataModifyType.Modified;

			if (CustsContentIsModified == DataModifyType.Modified)
				return DataModifyType.Modified;

			return DataModifyType.NotModified;
		}

		private DataModifyType CurrentRecordIsModified
		{
			get
			{
				if (CurrentRecord.EMP_ID != OrgRecord.EMP_ID ||
					CurrentRecord.EMP_NAME != OrgRecord.EMP_NAME ||
					CurrentRecord.EMAIL != OrgRecord.EMAIL ||
					CurrentRecord.ISCOMMON != OrgRecord.ISCOMMON ||
					CurrentRecord.DEP_ID != OrgRecord.DEP_ID ||
					CurrentRecord.TEL_EXTENSION != OrgRecord.TEL_EXTENSION ||
					CurrentRecord.MOBILE != OrgRecord.MOBILE ||
					CurrentRecord.SHORT_MOBILE != OrgRecord.SHORT_MOBILE||
					CurrentRecord.MENUSTYLE != OrgRecord.MENUSTYLE ||
					CurrentRecord.MENU_CODE != OrgRecord.MENU_CODE ||
					!string.IsNullOrWhiteSpace(NewMenuName))
				{
					return DataModifyType.Modified;
				}

				return DataModifyType.NotModified;
			}
		}

		private DataModifyType CustsContentIsModified
		{
			get
			{
				List<string> tmpCusts = GetSelectedCusts(CustList);
				string tmpGroups = GetSelectedGroups();
				var orgCusts = OrgCusts;
				if (!string.IsNullOrWhiteSpace(OrgDC.DC_CODE))
					  orgCusts = string.Join(",",OrgCusts.Split(',').Where(x => x.StartsWith(OrgDC.DC_CODE)));
				if (!string.Join(",", tmpCusts).Equals(orgCusts) ||
					!GetSelectedGroups().Equals(OrgGroups))
				{
					return DataModifyType.Modified;
				}

				return DataModifyType.NotModified;
			}
		}

		#endregion Save

		/// <summary>
		/// 依照id選取treeview裡的項目
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="custCode"></param>
		private void SetTreeViewItemChecked(List<P190501TreeView> parent, string custCode, string dcCode)
		{
			foreach (var p in parent)
			{
				foreach (P190501TreeView item in p.TreeView)
				{
					if (item.Id == custCode && item.Code == dcCode) { item.IsChecked = true; return; };
				}
			}
		}
		/// <summary>
		/// 重設所有項目為未選取
		/// </summary>
		/// <param name="parent"></param>
		private void ResetTreeViewDefaultNonSelected(List<P190501TreeView> parent)
		{
			if (parent == null) return;
			foreach (var p in parent)
			{
				p.IsChecked = false;
				foreach (P190501TreeView item in p.TreeView)
				{
					item.IsChecked = false;
					ResetTreeViewDefaultNonSelected(item.TreeView);
				}
			}
		}

		/// <summary>
		/// 取得已選取的Group
		/// </summary>
		/// <returns></returns>
		private string GetSelectedGroups()
		{
			string result = string.Empty;
			var tmp = GroupList.Where(x => x.IsSelected == true).Select(x => x.Item.GRP_ID).OrderBy(x => x).ToList();
			if (tmp.Any()) result = string.Join(",", tmp);
			return result;
		}

		/// <summary>
		/// 取得已選取的貨主, 供是否有變更的檢查. 傳回字串陣列與原始資料比對較快.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		private List<string> GetSelectedCusts(List<P190501TreeView> parent)
		{
			List<string> items = new List<string>();

			if (!parent.Any())
			{
				return new List<string>();
			}
			foreach (var p in parent)
			{
				//if (p.IsChecked == true && !string.IsNullOrWhiteSpace(p.Id)) items.Add(p.Id);
				//var tmp = GetSelectedCusts(p.TreeView);
				foreach (var q in p.TreeView)
				{
					if (q.IsChecked == true && !string.IsNullOrWhiteSpace(q.Id)) items.Add(q.Code + "|" + q.Id);
				}
				//if (tmp != null) items.AddRange(tmp);
			}
			//foreach (P190501TreeView item in parent.TreeView)
			//{
			//    if (item.IsChecked == true) items.Add(item.Id);
			//    List<string> tmp = GetSelectedCusts(item);
			//    if (tmp != null)
			//    {
			//        foreach (string t in tmp) items.Add(t);
			//    }
			//}
			return items.OrderBy(x => x.Trim()).ToList();
		}

		/// <summary>
		/// 取得已選取的貨主, 供寫入資料時使用
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		private List<wcf.F192402> GetSelectedCustsForWcf(List<P190501TreeView> parent)
		{
			List<wcf.F192402> items = new List<wcf.F192402>();
			foreach (P190501TreeView item in parent)
			{
				foreach (var p in item.TreeView)
				{
					if (p.IsChecked == true) items.Add(new wcf.F192402()
					{
						DC_CODEk__BackingField = p.Src.DC_CODE,
						GUP_CODEk__BackingField = p.Src.GUP_CODE,
						CUST_CODEk__BackingField = p.Src.CUST_CODE
					});
				}
				//if (item.IsChecked == true) items.Add(new wcf.F192402()
				//{
				//    DC_CODEk__BackingField = item.Src.DC_CODE,
				//    CUST_CODEk__BackingField = item.Src.CUST_CODE,
				//    GUP_CODEk__BackingField = item.Src.GUP_CODE
				//});
				//List<wcf.F192402> tmp = GetSelectedCustsForWcf(item);
				//if (tmp != null)
				//{
				//    foreach (var t in tmp) items.Add(t);
				//}
			}
			return items;
		}
		#endregion

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
					() => UserOperateMode == OperateMode.Query && !IsFirstRecord() && EmpList.Any()
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
					() => UserOperateMode == OperateMode.Query && !IsFirstRecord() && EmpList.Any()
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
					() => UserOperateMode == OperateMode.Query && !IsLastRecord() && EmpList.Any()
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
						() => UserOperateMode == OperateMode.Query && !IsLastRecord() && EmpList.Any()
				);
			}
		}

		private void DoMoveRecord(RecordMoveType type)
		{
			switch (type)
			{
				case RecordMoveType.First:
					SetData(EmpList.FirstOrDefault());
					break;
				case RecordMoveType.Previous:
					SetData(EmpList.LastOrDefault(x => string.Compare(CurrentRecord.EMP_ID, x.EMP_ID, StringComparison.Ordinal) > 0));
					break;
				case RecordMoveType.Next:
					SetData(EmpList.FirstOrDefault(x => string.Compare(x.EMP_ID, CurrentRecord.EMP_ID, StringComparison.Ordinal) > 0));
					break;
				case RecordMoveType.Last:
					SetData(EmpList.LastOrDefault());
					break;
			}
		}

		private bool IsFirstRecord()
		{
			//if (CurrentRecord == null) return true;
			var searchEmpId = (CurrentRecord != null) ? CurrentRecord.EMP_ID : SearchEmpId;
			if (EmpList == null || EmpList.FindIndex(x => x.EMP_ID.Equals(searchEmpId)) == 0)
				return true;
			return false;
		}
		private bool IsLastRecord()
		{
			//if (CurrentRecord == null) return true;
			var searchEmpId = (CurrentRecord != null) ? CurrentRecord.EMP_ID : SearchEmpId;

			if (EmpList == null || EmpList.FindIndex(x => x.EMP_ID.Equals(searchEmpId)) == EmpList.Count() - 1)
				return true;
			return false;
		}
		#endregion

		public void SetMenuStyleList()
		{
			var proxy = GetProxy<F19Entities>();
			var datas = proxy.F195402s.ToList().Select(x => new NameValuePair<string> { Name = x.MENU_DESC, Value = x.MENU_CODE }).ToList();
			datas.Insert(0, new NameValuePair<string>("", ""));
			MenuStyleList = datas;
		}
	}

	#region enum
	public enum RecordMoveType
	{
		First, Previous, Next, Last
	}
	public enum DataModifyType
	{
		Deleted, Modified, NotModified, New
	}
	#endregion

}
