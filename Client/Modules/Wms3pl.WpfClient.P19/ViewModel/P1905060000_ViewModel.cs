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
using Wms3pl.Common.Security;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using System.Data;
using System.IO;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1905060000_ViewModel : InputViewModelBase
	{
		public Action ClearPassWord = delegate { };

		public Func<string> GetePassword = () => { return null; };
		public Func<string> GetConfirmPassword = () => { return null; };

		public Action<OperateMode> UserOperateModeFocus = delegate { };

		public P1905060000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			IsBusy = true;
			DoSearchWorkgroup();
			DoSearchAllEmployee();
			DoSearchGroup();
			SetDefault();
			SelectEmp = EmpList.FirstOrDefault();
			IsBusy = false;
		}

		#region 資料連結/ 頁面參數

		#region Form - 員工編號/ 姓名
		private string _searchempId = string.Empty;
		public string SearchEmpId { get { return _searchempId; } set { _searchempId = value; RaisePropertyChanged("SearchEmpId"); } }

		private string _empId = string.Empty;
		public string EmpId { get { return _empId; } set { _empId = value; RaisePropertyChanged("EmpId"); } }
		private string _empName = string.Empty;
		public string EmpName { get { return _empName; } set { _empName = value; RaisePropertyChanged("EmpName"); } }
		#endregion

		#region Form - 工作群組名稱
		private string _searchGroupName = string.Empty;
		public string SearchGroupName
		{
			get { return _searchGroupName; }
			set { _searchGroupName = value; RaisePropertyChanged("SearchGroupName"); }
		}
		#endregion

		#region Form - 密碼/確認密碼
		//private string _password=string.Empty;
		//public string PassWord { get { return _password; } set { _password = value; } }
		//private string _checkpassword = string.Empty;
		//public string CheckPassWord { get { return _checkpassword; } set { _checkpassword = value; } }

		public const string PasswordLabelPropertyName = "PasswordLabel";
		private string _passwordLabel = "";
		public string PasswordLabel
		{
			get
			{
				return _passwordLabel;
			}

			set
			{
				if (_passwordLabel == value) return;
				_passwordLabel = value;
				RaisePropertyChanged(PasswordLabelPropertyName);
			}
		}
		#endregion

		#region Form - 包裝刷驗解鎖權限設定
		private string _checkpackage = "0";
		public string CheckPackage
		{
			get { return _checkpackage; }
			set { _checkpackage = value; RaisePropertyChanged("CheckPackage"); }
		}
		#endregion

		#region Data -匯出Excel
		private List<EmpWithFuncionName> _exceldata;
		public List<EmpWithFuncionName> ExcelData
		{
			get { return _exceldata; }
			set { _exceldata = value; RaisePropertyChanged("ExcelData"); }
		}

		#endregion


		#region Data - 工作群組

		private SelectionList<F1953> _groupList;
		/// <summary>
		/// 呈現在DataGrid上
		/// </summary>
		public SelectionList<F1953> GroupList
		{
			get { return _groupList; }
			set { _groupList = value; RaisePropertyChanged("GroupList"); }
		}

		private bool _isFuncSelectedAll = false;
		public bool IsFuncSelectedAll
		{
			get { return _isFuncSelectedAll; }
			set { _isFuncSelectedAll = value; RaisePropertyChanged("IsFuncSelectedAll"); }
		}


		#endregion

		#region Data - 作業群組
		private SelectionList<F1963> _workgroupList;
		/// <summary>
		/// 呈現在DataGrid上
		/// </summary>
		public SelectionList<F1963> WorkgroupList
		{
			get { return _workgroupList; }
			set { _workgroupList = value; RaisePropertyChanged("WorkgroupList"); }
		}

		private bool _isJobSelectedAll = false;
		public bool IsJobSelectedAll
		{
			get { return _isJobSelectedAll; }
			set { _isJobSelectedAll = value; RaisePropertyChanged("IsJobSelectedAll"); }
		}
		#endregion

		#region Data - 員工清單
		private List<F1924> _empList;
		public List<F1924> EmpList
		{
			get { return _empList; }
			set { _empList = value; RaisePropertyChanged("EmpList"); }
		}

		private F1924 _selectEmp;
		public F1924 SelectEmp
		{
			get { return _selectEmp; }
			set
			{
				_selectEmp = value;
				SetEmp(_selectEmp);
				SetScheduleAuthorize(_selectEmp);
				RaisePropertyChanged("SelectEmp");
			}
		}
		#endregion

		#region Data - 排程權限
		private P1905040000_ViewModel.ScheduleAuthorize _selectScheduleAuthorize;
		public P1905040000_ViewModel.ScheduleAuthorize SelectScheduleAuthorize
		{
			get { return _selectScheduleAuthorize; }
			set { _selectScheduleAuthorize = value; RaisePropertyChanged("SelectScheduleAuthorize"); }
		}

		private SelectionList<P1905040000_ViewModel.ScheduleAuthorize> _scheduleAuthorizeData;
		public SelectionList<P1905040000_ViewModel.ScheduleAuthorize> ScheduleAuthorizeData
		{
			get { return _scheduleAuthorizeData; }
			set { _scheduleAuthorizeData = value; RaisePropertyChanged("ScheduleAuthorizeData"); }
		}

		private bool _isScheduleAuthorizeAll = false;
		public bool IsScheduleAuthorizeAll
		{
			get { return _isScheduleAuthorizeAll; }
			set { _isScheduleAuthorizeAll = value; RaisePropertyChanged("IsScheduleAuthorizeAll"); }
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
				 o =>
				 {
					 SetDefault();
					 DoSearchAllEmployee();  //查詢員工資料
				 },
				() => UserOperateMode == OperateMode.Query,
				o =>
				{
					SelectEmp = EmpList.FirstOrDefault();
				});
			}
		}

		#region password
		/// <summary>
		/// 取得user password
		/// </summary>
		private string SearchEmpPassword()
		{
			var proxy = GetExProxy<P19ExDataSource>();
			var result = proxy.CreateQuery<GetUserPassword>("GetUserPassword")
								.AddQueryExOption("EmpId", EmpId)
								.FirstOrDefault();

			if (result == null)
				// 假資料的 Id 後面有空白字元的話，則會查不到該員工
				return string.Empty;
			else if (result.PASSWORD == null)
				return Properties.Resources.P1905060000_NoPassword;
			else
				return Properties.Resources.P1905060000_SetNoPassword;
		}


		#endregion password

		#region 人員
		/// <summary>
		/// 取得所有人員
		/// </summary>
		private void DoSearchAllEmployee()
		{
			SearchEmpId = SearchEmpId.Trim();

			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			var query = proxy.F1924s.Where(item => item.ISDELETED == "0").AsQueryable();

			if (!string.IsNullOrEmpty(SearchEmpId))
			{
				query = query.Where(item => item.EMP_ID.StartsWith(SearchEmpId));
			}

			EmpList = query.OrderBy(item => item.EMP_ID).ToList();

			//EmpList = (string.IsNullOrEmpty(SearchEmpId)) ? proxy.F1924s.OrderBy(o => o.EMP_ID).ToList() : proxy.F1924s.Where(x => x.EMP_ID.Equals(SearchEmpId)).OrderBy(o => o.EMP_ID).ToList();
			if (!EmpList.Any())
				ShowMessage(Messages.InfoNoData);
		}
		#endregion 人員

		#region 工作群組
		/// <summary>
		/// 取得工作Group
		/// </summary>
		private void DoSearchGroup()
		{
			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
            //執行查詢動作
            var f1953s = proxy.F1953s.Where(item => item.ISDELETED == "0");
            if (!string.IsNullOrEmpty(SearchGroupName))
            {
                f1953s = f1953s.Where(x => x.GRP_NAME.Contains(SearchGroupName));
            }
            GroupList = f1953s.OrderBy(x => x.GRP_ID).ToSelectionList();
			proxy = null;
		}
		/// <summary>
		/// 設定工作群組勾選
		/// </summary>
		/// <param name="data"></param>
		private void SetGroupData()
		{

			foreach (var p in GroupList)
			{
				p.IsSelected = false;
				p.IsSelectedOld = false;
			}

			// 取得F192401 (員工對應的工作群組)
			var proxy = GetProxy<F19Entities>();
			var f192401 = proxy.F192401s.Where(x => x.EMP_ID.Equals(EmpId)).ToList();
			foreach (var p in f192401)
			{
				var tmp = GroupList.FirstOrDefault(x => x.Item.GRP_ID.Equals(p.GRP_ID));
				if (tmp == null) continue;
				tmp.IsSelected = true;
				tmp.IsSelectedOld = true;
			}
			proxy = null;
		}

		#endregion 工作群組

		#region 作業群組
		/// <summary>
		/// 取得作業群組
		/// </summary>
		private void DoSearchWorkgroup()
		{
			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			//執行查詢動作
			WorkgroupList = proxy.F1963s.Where(item => item.ISDELETED == "0")
				.OrderBy(x => x.WORK_ID).ToSelectionList();
			proxy = null;
		}

		/// <summary>
		/// 設定作業群組勾選
		/// </summary>
		/// <param name="data"></param>
		private void SetWorkGroupData()
		{

			foreach (var p in WorkgroupList)
			{
				p.IsSelected = false;
				p.IsSelectedOld = false;
			}

			// 取得F192403 (員工對應的作業群組)
			var proxy = GetProxy<F19Entities>();
			var f192403 = proxy.F192403s.Where(x => x.EMP_ID.Equals(EmpId)).ToList();
			foreach (var p in f192403)
			{
				var tmp2 = WorkgroupList.FirstOrDefault(x => x.Item.WORK_ID.Equals(p.WORK_ID));
				if (tmp2 == null) continue;
				tmp2.IsSelected = true;
				tmp2.IsSelectedOld = true;
			}
			proxy = null;

		}
		#endregion 作業群組

		#region ExcelDarta
		/// <summary>
		/// 取得user password
		/// </summary>
		private bool DoGetExportData()
		{
			var proxy = GetExProxy<P19ExDataSource>();
			var result = proxy.CreateQuery<EmpWithFuncionName>("EmpWithFuncionName")
				.AddQueryOption("EmpId", string.Format("'{0}'", EmpId.Trim())).ToList();
			if (!result.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return false;
			}
			ExcelData = result;
			return true;
		}
		#endregion ExcelDarta

		#endregion Search

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectEmp != null
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			UserOperateModeFocus(OperateMode.Edit);
			//執行編輯動作
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes)
						return;

					ClearPassWord();
					SelectEmp = SelectEmp;
					UserOperateMode = OperateMode.Query;
				},
				() => UserOperateMode != OperateMode.Query);
			}
		}
		#endregion Edit

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

						if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
							return;

						var password = GetePassword();
						var confirmPassword = GetConfirmPassword();

						// 若無修改則不用儲存
						if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(confirmPassword) && !DataModified())
						{
							ShowMessage(Messages.WarningNotModified);
							return;
						}
						else if (string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
						{
							ShowWarningMessage(Properties.Resources.P1905060000_InputPassWord);
							return;
						}
						else if (!string.IsNullOrEmpty(password) && string.IsNullOrEmpty(confirmPassword))
						{
							ShowWarningMessage(Properties.Resources.P1905060000_InputConfirmPassword);
							return;
						}

						isSaved = DoSave(password, confirmPassword);

					},
					CanExecuteSaveCommand,
					o =>
					{
						if (isSaved)
						{
							ClearPassWord();
							UserOperateMode = OperateMode.Query;
						}
					});
			}
		}

		bool CanExecuteSaveCommand()
		{
			if (UserOperateMode == OperateMode.Query)
				return false;

			if (SelectEmp == null)
				return false;

			return true;
		}

		public bool DataModified()
		{
			var groupListModified = GroupList.Any(item => item.IsSelected != item.IsSelectedOld);
			var workGroupsModified = WorkgroupList.Any(item => item.IsSelected != item.IsSelectedOld);
			var packageUnlockModified = SelectEmp.PACKAGE_UNLOCK != CheckPackage;
			var scheduleAuthorizeDataModified = ScheduleAuthorizeData.Any(item => item.IsSelected != item.IsSelectedOld);
			return groupListModified || workGroupsModified || packageUnlockModified || scheduleAuthorizeDataModified;
		}

		public bool DoSave(string password, string confirmPassword)
		{
			if (SelectEmp == null)
				return false;

			var addgroups = GetAddGroups().ToArray();
			var romovegroups = GetRemoveGroups().ToArray();
			var addworkgroups = GetAddWorkGroups().ToArray();
			var removeworkgroups = GetRemoveWorkGroups().ToArray();
			var scheduleIdList = GetScheduleIdList().ToArray();


			var proxy = new wcf.P19WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
				() => proxy.UpdateP190506(EmpId,
					password,
					confirmPassword,
					addgroups,
					romovegroups,
					addworkgroups,
					removeworkgroups,
					scheduleIdList,
					CheckPackage));
			ShowResultMessage(result);

			return result.IsSuccessed;
		}

		/// <summary>
		/// 取得已選取的Group
		/// </summary>
		/// <returns></returns>
		private IEnumerable<decimal> GetAddWorkGroups()
		{
			return WorkgroupList.Where(x => x.IsSelectedOld != x.IsSelected && x.IsSelected)
													.Select(x => x.Item.WORK_ID)
													.OrderBy(x => x);
		}
		private IEnumerable<decimal> GetRemoveWorkGroups()
		{
			return WorkgroupList.Where(x => x.IsSelectedOld != x.IsSelected && x.IsSelectedOld)
													.Select(x => x.Item.WORK_ID)
													.OrderBy(x => x);
		}

		/// <summary>
		/// 取得已選取的Group
		/// </summary>
		/// <returns></returns>
		private IEnumerable<decimal> GetAddGroups()
		{
			return GroupList.Where(x => x.IsSelectedOld != x.IsSelected && x.IsSelected)
											.Select(x => x.Item.GRP_ID)
											.OrderBy(x => x);
		}

		private IEnumerable<decimal> GetRemoveGroups()
		{
			return GroupList.Where(x => x.IsSelectedOld != x.IsSelected && x.IsSelectedOld)
											.Select(x => x.Item.GRP_ID)
											.OrderBy(x => x);
		}


		#endregion Save

		#region exportExcel
		public void SaveToCSV(List<EmpWithFuncionName> ary, string FilePath)
		{
			string data = "";
			string title = Properties.Resources.P1905060000_ExcelTitle;
			StreamWriter wr = new StreamWriter(FilePath, false, System.Text.Encoding.GetEncoding(950));	// big5
			//取得陣列元素的型別
			Type elemType = ary.GetType().GetElementType();
			//PropertyInfo[] props = elemType.GetProperties();
			StringBuilder sb = new StringBuilder();
			//藉由foreach巡迴每一元件，透過Refelction取出屬性值
			wr.Write(title);
			foreach (var elem in ary)
			{
				data += elem.FUN_CODE + "," + elem.FUN_NAME + "," + elem.BTNAME + "," + elem.BTOPT;

				data += "\n";
				wr.Write(data);
				data = "";
			}

			wr.Dispose();
			wr.Close();

		}
		#endregion

		private void SetEmp(F1924 theEmp = null)
		{
			EmpId = (theEmp != null) ? theEmp.EMP_ID : string.Empty;
			EmpName = (theEmp != null) ? theEmp.EMP_NAME : string.Empty;
			PasswordLabel = (theEmp != null) ? SearchEmpPassword() : string.Empty;
			CheckPackage = (theEmp != null) ? theEmp.PACKAGE_UNLOCK : "0";
			//IsEnableEdit = (theEmp != null);
			SetGroupData();
			IsFuncSelectedAll = GroupList.All(item => item.IsSelected);
			SetWorkGroupData();
			IsJobSelectedAll = WorkgroupList.All(item => item.IsSelected);
		}
		private void SetDefault()
		{
			//IsEnableEdit = false;
			SetEmp();
			SetScheduleAuthorize();
			IsFuncSelectedAll = false;
			IsJobSelectedAll = false;
			IsScheduleAuthorizeAll = false;
			SetAllCheckBox(GroupType.None);
		}

		private void SetAllCheckBox(GroupType groupType)
		{
			if (groupType == GroupType.FunctionGroup ||
					groupType == GroupType.None)
			{
				foreach (var item in GroupList)
					item.IsSelected = IsFuncSelectedAll;
			}

			if (groupType == GroupType.JobGroup ||
					groupType == GroupType.None)
			{
				foreach (var item in WorkgroupList)
					item.IsSelected = IsJobSelectedAll;
			}

			if (groupType == GroupType.ScheduleAuthorize ||
					groupType == GroupType.None)
			{
				foreach (var item in ScheduleAuthorizeData)
					item.IsSelected = IsScheduleAuthorizeAll;
			}
		}

		private List<string> GetScheduleIdList()
		{
			return ScheduleAuthorizeData.Where(x => x.IsSelected).Select(x => x.Item.SCHEDULE_ID).ToList();
		}

		private void SetScheduleAuthorize(F1924 empData = null)
		{
			var proxy00 = GetProxy<F00Entities>();
			var f000904Data = GetBaseTableService.GetF000904List(FunctionCode, "Schedule", "Authorize");


			var proxy = GetProxy<F19Entities>();
			var f192405Data = new List<F192405>();
			if (empData != null)
			{
				f192405Data =
					(from p in proxy.F192405s
					 where p.EMP_ID == empData.EMP_ID
					 select p).ToList();
			}

			ScheduleAuthorizeData =
				(from n in f000904Data
				 join p in f192405Data on n.Value equals p.SCHEDULE_ID
					 into temp
				 from ds in temp.DefaultIfEmpty()
				 select new P1905040000_ViewModel.ScheduleAuthorize
				 {
					 Checked = (ds != null),
					 ScheduleName = n.Name,
					 SCHEDULE_ID = n.Value
				 }).ToSelectionList();

			foreach (var s in ScheduleAuthorizeData)
			{
				s.IsSelected = s.Item.Checked;
				s.IsSelectedOld = s.Item.Checked;
			}

			IsScheduleAuthorizeAll = ScheduleAuthorizeData.All(x => x.IsSelected);
		}
		#endregion

		#region **Action
		private ICommand _checkAllTask;
		public ICommand CheckAllTask
		{
			get
			{
				if (_checkAllTask == null)
				{
					_checkAllTask = new RelayCommand<string>(
						(t) =>
						{
							GroupType transEnum;
							if (Enum.TryParse(t, out transEnum))
							{
								SetAllCheckBox(transEnum);
							}
						},
						(t) => { return UserOperateMode != OperateMode.Query; }
						);
				}
				return _checkAllTask;
			}
		}

		public ICommand ExportExcelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					 o => DoExportData(),
							() => SelectEmp != null
				);
			}
		}

		private void DoExportData()
		{
			//取得Excel資料
			if (!DoGetExportData()) return;
			var openFileDialog = new Microsoft.Win32.SaveFileDialog()
			{
				DefaultExt = "csv",
				AddExtension = true,
				Filter = "save files (*.csv)|*.csv",
				OverwritePrompt = true,
				FileName = Properties.Resources.P1905060000_PeopleFuncList,
				Title = Properties.Resources.P1905060000_DestSavePath
			};

			if ((bool)openFileDialog.ShowDialog())
			{
				var f = new FileInfo(openFileDialog.FileName);
				openFileDialog.FileName = f.Directory + "/" + f.Name;
				SaveToCSV(ExcelData, openFileDialog.FileName);
			}
		}

		public enum GroupType
		{
			None,
			/// <summary>
			/// 工作群組
			/// </summary>
			FunctionGroup,
			/// <summary>
			/// 作業群組
			/// </summary>
			JobGroup,
			/// <summary>
			/// 排程權限
			/// </summary>
			ScheduleAuthorize
		}
		#endregion

	}

}
