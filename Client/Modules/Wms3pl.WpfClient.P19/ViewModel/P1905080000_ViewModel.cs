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
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1905080000_ViewModel : InputViewModelBase
	{
		/// <summary>
		/// 將移動項目的目的地的控制項做 ScrollIntoView
		/// </summary>
		public Action<ListboxMovingType> ScrollIntoViewLast = delegate { };

		/// <summary>
		/// 編輯模式的焦點設定
		/// </summary>
		public Action EditModeFocus = delegate { };


		private string _userId;

		public P1905080000_ViewModel()
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
			DoSearchDepList();
			SelectedDep = DepList.FirstOrDefault();
			DoSearchAllEmployee();
			DoSearch();
		 	SelectedWorkgroupItem = WorkgroupList.FirstOrDefault();
			IsBusy = false;
		}

		#region 資料連結/ 頁面參數
		//public bool IsCheckDataModified = true;
		//private bool _isEnableEdit = false;
		//public bool IsEnableEdit
		//{
		//	get { return _isEnableEdit; }
		//	set { _isEnableEdit = value; RaisePropertyChanged("IsEnableEdit"); }
		//}
		#region Form - 員工編號/ 姓名
		private string _empId = string.Empty;
		public string EmpId { get { return _empId; } set { _empId = value; } }
		private string _empName = string.Empty;
		public string EmpName { get { return _empName; } set { _empName = value; } }
		#endregion
		#region Form - 工作群組名稱
		private string _searchWorkgroupName = string.Empty;
		public string SearchWorkgroupName
		{
			get { return _searchWorkgroupName; }
			set { _searchWorkgroupName = value; RaisePropertyChanged("SearchWorkgroupName"); }
		}
		#endregion
		#region Form - 作業群組清單
		private F1963 _selectedWorkgroupItem;
		public F1963 SelectedWorkgroupItem
		{
			get { return _selectedWorkgroupItem; }
			set
			{
				_selectedWorkgroupItem = value;
				RaisePropertyChanged("SelectedWorkgroupItem");
				//IsEnableEdit = !(_selectedWorkgroupItem == null);
				if (value != null)
				{
					DoSearchAssignedEmployee();
				}
			}
		}

		private List<F1963> _workgroupList;
		public List<F1963> WorkgroupList
		{
			get { return _workgroupList; }
			set { _workgroupList = value; RaisePropertyChanged("WorkgroupList"); }
		}

		#endregion
		#region data - 員工清單

		private List<F1924> _allEmp;

		/// <summary>
		/// 存放所有員工
		/// </summary>
		public List<F1924> AllEmp
		{
			get { return _allEmp; }
		}

		private SelectionList<F1924> _assignedList;
		public SelectionList<F1924> AssignedList
		{
			get { return _assignedList; } //.OrderBy(x => x.EMP_ID).ToObservableCollection(); }
			set { _assignedList = value; RaisePropertyChanged("AssignedList"); }
		}

		private SelectionList<F1924> _unAssignedList;
		public SelectionList<F1924> UnAssignedList
		{
			get { return _unAssignedList; } //.OrderBy(x => x.EMP_ID).ToObservableCollection(); }
			set { _unAssignedList = value; RaisePropertyChanged("UnAssignedList"); }
		}

		#endregion
		#region Data - 單位
		private NameValuePair<string> _selectedDep;

		public NameValuePair<string> SelectedDep
		{
			get { return _selectedDep; }
			set
			{
				_selectedDep = value;
				RaisePropertyChanged("SelectedDep");

				UnAssignedList = new SelectionList<F1924>(new List<F1924>());
			}
		}


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
						//if (ConfirmBeforeAction() == DialogResponse.Cancel) return;
						DoSearch();
					},
					()=> UserOperateMode == OperateMode.Query,
					o=>
					{
						AssignedList = new SelectionList<F1924>(new List<F1924>());
						SelectedWorkgroupItem = WorkgroupList.FirstOrDefault();
					}

				);
			}
		}

		/// <summary>
		/// 取得Group
		/// </summary>
		private void DoSearch()
		{
			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
            //執行查詢動作
            var f1963s = proxy.F1963s.Where(x => x.ISDELETED == "0");
            if (!string.IsNullOrEmpty(SearchWorkgroupName.Trim()))
            {
                f1963s = f1963s.Where(x => x.WORK_NAME.Contains(SearchWorkgroupName));
            }
            WorkgroupList = f1963s.OrderBy(x => x.WORK_ID).AsQueryable().ToList();            
			// 預選到第一項
			//IsCheckDataModified = false;

			if (!WorkgroupList.Any()) ShowMessage(Messages.InfoNoData);
			//IsCheckDataModified = true;
			proxy = null;
		}
		#endregion Search

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedWorkgroupItem != null
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			EditModeFocus();
			//執行編輯動作
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				DialogResponse dialogResponse = DialogResponse.No;
				return CreateBusyAsyncCommand(
					o =>
					{
						// 檢查資料是否更變
						if (!DataModified())
						{
							dialogResponse = DialogResponse.No;
							return;
						}

						// 資料已更變，是否儲存 (Y/N/C)
						dialogResponse = ShowMessage(Messages.WarningBeforeAdd);

						if (dialogResponse != DialogResponse.Yes)
						{
							return;
						}

						if (!DoSave().IsSuccessed)
						{
							// 若儲存沒有成功，則停留原畫面
							dialogResponse = DialogResponse.Cancel;
						}
					},
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						if (dialogResponse != DialogResponse.Cancel)
						{
							SelectedWorkgroupItem = SelectedWorkgroupItem;
							UserOperateMode = OperateMode.Query;
						}
					});
			}
		}

		private void DoCancel()
		{
			UserOperateMode = OperateMode.Query;
			//執行取消動作
		}
		#endregion Edit

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				DialogResponse dialogResponse = DialogResponse.No; 
				return CreateBusyAsyncCommand(
					o =>
					{
						dialogResponse = ConfirmBeforeSave();
						if (dialogResponse != DialogResponse.Yes)
						{
							return;
						}
							
						if (!DoSave().IsSuccessed)
						{
							dialogResponse = DialogResponse.Yes;
						}
					},
					() => UserOperateMode != OperateMode.Query,
					o=>
					{
						if (dialogResponse == DialogResponse.Yes)
						{
							SelectedWorkgroupItem = SelectedWorkgroupItem;
							UserOperateMode = OperateMode.Query;
						}
					}
				);
			}
		}

		private ExecuteResult DoSave()
		{
			//執行確認儲存動作
			string assigned = this.GetSelectedItems();

			wcf.ExecuteResult result = new wcf.ExecuteResult() { IsSuccessed = true };
			string[] empList = AssignedList.Select(x => x.Item.EMP_ID).OrderBy(empId => empId).ToArray();
			var proxy = new wcf.P19WcfServiceClient();
			result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateP190508(SelectedWorkgroupItem.WORK_ID, empList, this._userId));

			ShowMessage(new List<ExecuteResult>() {
				new ExecuteResult() { IsSuccessed = result.IsSuccessed, Message = result.Message }
			});

			return new ExecuteResult() { IsSuccessed = result.IsSuccessed, Message = result.Message };
		}
		#endregion Save

		#region 搜尋員工
		public ICommand SearchEmployeeCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						// 一併做全員工的更新
						DoSearchAllEmployee();
						DoSearchEmployeeByCondition();
						//MoveToUnAssignedList();
					}
				);
			}
		}
		/// <summary>
		/// 依篩選條件從既有清單搜尋員工
		/// </summary>
		private void DoSearchEmployeeByCondition()
		{
			EmpId = EmpId.Trim();
			EmpName = EmpName.Trim();

			if (AllEmp == null) return;
			// 搜尋員工時, 如果是不指定, 就找出所有員工
			// 如果是全部, 就找出任何一群組的員工
			//ValidEmpList = EmpList.Where(
			//	x => (x.EMP_ID.StartsWith(EmpId.Trim()) || string.IsNullOrWhiteSpace(EmpId))
			//		&& (x.EMP_NAME.Contains(EmpName.Trim()) || string.IsNullOrWhiteSpace(EmpName)))
			//		.ToList();

			var query = AllEmp.AsQueryable();

			if (SelectedDep != null && !string.IsNullOrEmpty(SelectedDep.Value))
			{
				query = query.Where(emp => emp.DEP_ID == SelectedDep.Value);
			}

			if (!string.IsNullOrEmpty(EmpId))
			{
				query = query.Where(emp => emp.EMP_ID.StartsWith(EmpId));
			}

			if (!string.IsNullOrEmpty(EmpName))
			{
				query = query.Where(emp => emp.EMP_NAME.Contains(EmpName));
			}

			if (AssignedList != null)
			{
				// 只顯示不存在已設定人員的員工
				query = query.Where(emp => !AssignedList.Where(item => item.Item.EMP_ID == emp.EMP_ID).Any());
			}

			UnAssignedList = new SelectionList<F1924>(query.OrderBy(emp => emp.EMP_ID));
		}

		/// <summary>
		/// 取得所有人員
		/// </summary>
		private void DoSearchAllEmployee()
		{
			var proxy = GetProxy<F19Entities>();
			_allEmp = proxy.F1924s.Where(emp => emp.ISDELETED == "0")
								 .OrderBy(x => x.EMP_ID)
								 .ToList();
		}

		///// <summary>
		///// 將員工放到未設定的員工清單
		///// </summary>
		//private void MoveToUnAssignedList()
		//{
		//	var result = ValidEmpList.Select(x => new { EMP_ID = x.EMP_ID, EMP_NAME = x.EMP_NAME }).Distinct().ToList();

		//	// 把取回的員工名單加入至UnAssignedList (需判斷是否已存在AssignedList)
		//	var tmpUnAssignedList = new ObservableCollection<F1924>();
		//	if (result == null || !result.Any()) return;
		//	foreach (var p in result.OrderBy(x => x.EMP_ID))
		//	{
		//		var tmpRemove = AssignedList.FirstOrDefault(x => x.EMP_ID.Equals(p.EMP_ID));
		//		// 如果不在AssignedList裡, 則加入至UnAssignedList
		//		if (tmpRemove == null)
		//		{
		//			// 加入前先做轉型
		//			tmpUnAssignedList.Add(AutoMapper.Mapper.DynamicMap<F1924>(p));
		//		}
		//	}
		//	UnAssignedList = tmpUnAssignedList;
		//}
		/// <summary>
		/// 取得該Group的人員
		/// </summary>
		private void DoSearchAssignedEmployee()
		{
			// 看不懂原來的移動邏輯，這邊直接改寫
			var proxy = GetProxy<F19Entities>();
			// 取得這個作業群組下的所有人員編號
			var f192403EmpIdList = proxy.F192403s.Where(x => x.WORK_ID.Equals(SelectedWorkgroupItem.WORK_ID)).ToList().Select(item => item.EMP_ID);

			var query = from empId in f192403EmpIdList
						join emp in AllEmp on empId equals emp.EMP_ID
						where emp.ISDELETED != "1"
						orderby emp.EMP_ID
						select emp;

			AssignedList = new SelectionList<F1924>(query);

			if (UnAssignedList != null)
			{
				// 更新已設定人員時，也將未設定人員清單裡面已出現在已設定人員中移除
				//var updateUnassignEmpList = UnAssignedList.Where(unEmp => !AssignedList.Where(emp => emp.Item.EMP_ID == unEmp.Item.EMP_ID).Any())
				//											.Select(unEmp => unEmp.Item);

				UnAssignedList = null;
			}
						

			//// 清單初始化
			//UnAssignedList = new SelectionList<F1924>();
			//AssignedList = new SelectionList<F1924>();
			//List<F1924> tmpAssigned = new List<F1924>();
			//// 把人員加入到AssignedList, 並從UnAssignedList移除
			//if (SelectedWorkgroupItem != null)
			//{
			//	var proxy = GetProxy<F19Entities>();
			//	var tmp = proxy.F192403s.Where(x => x.WORK_ID.Equals(SelectedWorkgroupItem.WORK_ID)).AsQueryable().ToList();

			//	// 如果是第一次進來, 未查詢員工前, EMPLIST會是空的, 所以要去DB取得員工資料來顯示名稱 (要濾掉ISDELETED)
			//	List<F1924> tmpEmpList = new List<F1924>();
			//	if (EmpList == null || !EmpList.Any())
			//	{
			//		tmpEmpList = proxy.F1924s.Where(x => x.ISDELETED != "1").ToList();
			//	}

			//	if (tmp != null && tmp.Count() > 0) // 這裡不能用Any()做判斷, 會掛掉
			//	{
			//		foreach (var p in tmp.OrderBy(x => x.EMP_ID).ToList())
			//		{
			//			// 轉成員工資料, 如果EMPLIST有資料, 就取EMPLIST的值, 否則取tmpEMPLIST的值
			//			F1924 item = new F1924();
			//			item.EMP_ID = p.EMP_ID;
			//			string empName = string.Empty;
			//			if (EmpList != null && EmpList.Any())
			//			{
			//				var tmpEmp = EmpList.FirstOrDefault(x => x.EMP_ID.Equals(p.EMP_ID));
			//				if (tmpEmp != null) empName = tmpEmp.EMP_NAME;
			//				item.EMP_NAME = empName;
			//				// 加入清單
			//				if (tmpEmp != null)  //沒找到不加進來
			//					tmpAssigned.Add(item);
			//			}
			//			else if (tmpEmpList.Any())
			//			{
			//				var tmpEmp = tmpEmpList.FirstOrDefault(x => x.EMP_ID.Equals(p.EMP_ID));
			//				if (tmpEmp != null) empName = tmpEmp.EMP_NAME;
			//				item.EMP_NAME = empName;
			//				// 加入清單
			//				if (tmpEmp != null)  //沒找到不加進來
			//					tmpAssigned.Add(item);
			//			}
			//		}
			//	}
			//}
			//AssignedList = tmpAssigned.ToObservableCollection();
			//// 搜尋符合條件的員工, 此時會一併將已Assign的員工從UnAssign List移除
			//MoveToUnAssignedList();
		}

		/// <summary>
		/// 搜尋所有單位編號與名稱
		/// </summary>
		private void DoSearchDepList()
		{
			var proxy = GetProxy<F19Entities>();
			var depList = proxy.F1925s.ToList().Select(item => new NameValuePair<string>()
			{
				Name = string.Format("{0}-{1}", item.DEP_ID, item.DEP_NAME),
				Value = item.DEP_ID
			}).ToList();

			depList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });

			DepList = depList;
		}
		#endregion

		#region 移動 (全部選取/ 部份選取)
		public ICommand MoveItemsCommand
		{
			get
			{
				ListboxMovingType type = ListboxMovingType.Assign;
				return CreateBusyAsyncCommand(
					o =>
					{
						Enum.TryParse(o.ToString(), out type);
					},
					()=> UserOperateMode != OperateMode.Query,
					o=>
					{
						if (UnAssignedList == null)
						{
							UnAssignedList = new SelectionList<F1924>(new List<F1924>());
						}

						switch (type)
						{
							case ListboxMovingType.Assign:
								MoveItems(UnAssignedList, AssignedList, isMoveAll: false);
								break;
							case ListboxMovingType.UnAssign:
								MoveItems(AssignedList, UnAssignedList, isMoveAll: false);
								break;
							case ListboxMovingType.AssignAll:
								MoveItems(UnAssignedList, AssignedList, isMoveAll: true);
								break;
							case ListboxMovingType.UnAssignAll:
								MoveItems(AssignedList, UnAssignedList, isMoveAll: true);
								break;
						}
						ScrollIntoViewLast(type);
					}
				);
			}
		}

		///// <summary>
		///// 移動項目
		///// </summary>
		///// <param name="type"></param>
		//public void DoMoveItems(ListboxMovingType type)
		//{
		//	// 因ObservableCollection不允許在多執行緒下變更其內容, 但為達到資料更新畫面即更新, 所以用暫存(tmpList)來搬動項目
		//	List<F1924> tmpAssigned = new List<F1924>();
		//	List<F1924> tmpUnAssigned = new List<F1924>();
		//	tmpAssigned = AssignedList.ToList();
		//	tmpUnAssigned = UnAssignedList.ToList();
		//	switch (type)
		//	{
		//		case ListboxMovingType.Assign:
		//			if (SelectedEmpList == null || !SelectedEmpList.Any()) break;
		//			foreach (var p in SelectedEmpList)
		//			{
		//				tmpAssigned.Add(p);
		//				tmpUnAssigned.Remove(p);
		//			}
		//			AssignedList = tmpAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			UnAssignedList = tmpUnAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			break;
		//		case ListboxMovingType.AssignAll:
		//			if (UnAssignedList == null || !UnAssignedList.Any()) break;
		//			foreach (var p in UnAssignedList)
		//			{
		//				tmpAssigned.Add(new F1924()
		//				{
		//					EMP_ID = p.EMP_ID,
		//					EMP_NAME = p.EMP_NAME
		//				});
		//			}
		//			UnAssignedList = new ObservableCollection<F1924>();
		//			AssignedList = tmpAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			break;
		//		case ListboxMovingType.UnAssign:
		//			if (SelectedEmpList == null || !SelectedEmpList.Any()) break;
		//			foreach (var p in SelectedEmpList)
		//			{
		//				tmpAssigned.Remove(p);
		//				// 這裡要去檢查該員工是否符合查詢條件
		//				tmpUnAssigned.Add(p);
		//			}
		//			AssignedList = tmpAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			UnAssignedList = tmpUnAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			break;
		//		case ListboxMovingType.UnAssignAll:
		//			if (AssignedList == null || !AssignedList.Any()) break;
		//			foreach (var p in AssignedList)
		//			{
		//				// 這裡要去檢查該員工是否符合查詢條件
		//				tmpUnAssigned.Add(new F1924()
		//				{
		//					EMP_ID = p.EMP_ID,
		//					EMP_NAME = p.EMP_NAME
		//				});
		//			}
		//			AssignedList = new ObservableCollection<F1924>();
		//			UnAssignedList = tmpUnAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			break;
		//	}
		//	tmpAssigned = null;
		//	tmpUnAssigned = null;
		//}

		private void MoveItems(SelectionList<F1924> sourceSelectionList, SelectionList<F1924> destinationSelectionList, bool isMoveAll)
		{
			// 先將目的地所有項目設為不選擇
			foreach (var item in destinationSelectionList)
				item.IsSelected = false;

			// 在將要移動的項目放到目的地
			var changedItems = sourceSelectionList.Where(unEmp => unEmp.IsSelected || isMoveAll).ToList();
			foreach (var item in changedItems)
				destinationSelectionList.Add(item);

			// 最後再將來源移除要移動的項目，這樣畫面上有選擇的項目，就只會顯示被移動的項目，其他都不選擇
			foreach (var changedItem in changedItems)
				sourceSelectionList.Remove(changedItem);
		}
		#endregion
		#endregion

		#region 資料編輯檢查
		///// <summary>
		///// 任何操作前做資料編輯檢查
		///// </summary>
		///// <returns></returns>
		//public DialogResponse ConfirmBeforeAction()
		//{
		//	DialogResponse dr = DialogResponse.Yes;
		//	if (!IsCheckDataModified) return dr;
		//	if (DataModified())
		//	{
		//		dr = ShowMessage(Messages.WarningBeforeAdd);
		//		if (dr == DialogResponse.Yes)
		//		{
		//			if (DoSave().IsSuccessed == true)
		//			{
		//				return dr;
		//			}
		//			else
		//			{
		//				return DialogResponse.Cancel;
		//			}
		//		}
		//	}
		//	return dr;
		//}

		/// <summary>
		/// 儲存前做資料確認
		/// </summary>
		/// <returns></returns>
		private DialogResponse ConfirmBeforeSave()
		{
			DialogResponse dr = DialogResponse.Yes;
			if (!DataModified() || SelectedWorkgroupItem == null)
			{
				ShowMessage(Messages.WarningNotModified);
				dr = DialogResponse.Cancel;
				return dr;
			}
			else
			{
				return ShowMessage(Messages.WarningBeforeUpdate);
			}
		}

		/// <summary>
		/// 檢查資料是否有修改
		/// </summary>
		/// <returns></returns>
		public bool DataModified()
		{
			if (SelectedWorkgroupItem == null) return false;
			// 再取一次資料庫的資料
			var proxy = GetProxy<F19Entities>();
			var tmpAssigned = proxy.F192403s.Where(x => x.WORK_ID.Equals(SelectedWorkgroupItem.WORK_ID)).ToList();
			// 取得已刪除的emp id
			var tmpDeleted = proxy.F1924s.Where(x => x.ISDELETED == "1").ToList();
			// 檢查時要濾掉已被刪除的emp id
			var tmpSrc = from a in tmpAssigned
						 join b in tmpDeleted on a.EMP_ID equals b.EMP_ID into d
						 from c in d.DefaultIfEmpty()
						 where c == null
						 select a.EMP_ID;
			string src = string.Empty;
			src = string.Join(",", tmpSrc.OrderBy(x => x));

			// 然後跟目前選擇的員工資料做比對
			string dest = this.GetSelectedItems();

			return !src.Equals(dest);
		}

		/// <summary>
		/// 取得已選擇的員工, 用EMP_ID組成逗號分隔的字串
		/// </summary>
		/// <returns></returns>
		private string GetSelectedItems()
		{
			//string dest = string.Empty;
			//if (AssignedList != null)
			//{
			//	foreach (var p in AssignedList.OrderBy(x => x.EMP_ID).ToList())
			//	{
			//		dest += "," + p.EMP_ID;
			//	}
			//	if (!string.IsNullOrEmpty(dest)) dest = dest.Substring(1);
			//}
			//return dest;

			if (AssignedList == null)
			{
				return string.Empty;
			}

			return string.Join(",", AssignedList.Select(item => item.Item.EMP_ID).OrderBy(empId => empId));
		}
		#endregion
	}

}
