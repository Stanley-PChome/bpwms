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

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1905050000_ViewModel : InputViewModelBase
	{
		private string _userId;

		/// <summary>
		/// 將移動項目的目的地的控制項做 ScrollIntoView
		/// </summary>
		public Action<P190505MovingType> ScrollIntoViewLast = delegate { };

		/// <summary>
		/// 編輯模式的焦點設定
		/// </summary>
		public Action EditModeFocus = delegate { };

		public P1905050000_ViewModel()
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
			//DoSearchWorkgroup();
			DoSearchDepList();
			SelectedDep = DepList.FirstOrDefault();
			DoSearchAllEmployee();
			DoSearch();
			SelectedGroupItem = GroupList.FirstOrDefault();
			IsBusy = false;
		}

		#region 資料連結/ 頁面參數
		//private bool IsCheckDataModified = true;
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
		private string _searchGroupName = string.Empty;
		public string SearchGroupName
		{
			get { return _searchGroupName; }
			set { _searchGroupName = value; RaisePropertyChanged("SearchGroupName"); }
		}
		#endregion
		#region Form - 工作群組清單
		private F1953 _selectedGroupItem;
		public F1953 SelectedGroupItem
		{
			get { return _selectedGroupItem; }
			set
			{
				_selectedGroupItem = value;
				RaisePropertyChanged("SelectedGroupItem");

				if (value == null)
				{
					return;
				}

				AssignEmpItems = new SelectionList<F1924>(GetF1924DataQuery(value.GRP_ID));
				FromUnassignRemoveAssignedItems();
				//IsEnableEdit = !(_selectedGroupItem == null);
				//DoSearchAssignedEmployee();
			}
		}

		/// <summary>
		/// 取得要儲存已設定員工的所有編號，也是用來取得比對資料是否修改過的資料方法
		/// </summary>
		/// <returns></returns>
		public string GetAssignEmpString(IEnumerable<F1924> items)
		{
			return string.Join(",", items.GroupBy(emp => emp.EMP_ID).Select(g => g.Key).OrderBy(empId => empId));
		}

		private void FromUnassignRemoveAssignedItems()
		{
			if (UnassignEmpItems != null)
			{
				// 更新已設定人員時，也將未設定人員清單裡面已出現在已設定人員中移除
				var updateUnassignEmpList = UnassignEmpItems.Where(unEmp => !AssignEmpItems.Where(emp => emp.Item.EMP_ID == unEmp.Item.EMP_ID).Any())
															.Select(unEmp => unEmp.Item);
				//.ToList();
				UnassignEmpItems = new SelectionList<F1924>(updateUnassignEmpList);
			}
		}

		private IEnumerable<F1924> GetF1924DataQuery(decimal grpId)
		{
			// 取得位於這個工作群組的所有員工編號
			var proxy = GetProxy<F19Entities>();
			var empListbyGroup = proxy.F192401s.Where(emp => emp.GRP_ID == grpId).ToList();

			// 從所有員工取得位於這群組內的員工與中文名稱等
			var query = from empGroup in empListbyGroup
						join emp in AllEmp on empGroup.EMP_ID equals emp.EMP_ID
						where emp.ISDELETED != "1"
						orderby emp.EMP_ID
						select emp;
			return query;
		}

		/// <summary>
		/// 已設定的資料是否已經修改
		/// </summary>
		public bool AssignEmpModified
		{
			get
			{
				var uiAssign = GetAssignEmpString(AssignEmpItems.Select(s => s.Item));
				var dbAssign = GetAssignEmpString(GetF1924DataQuery(SelectedGroupItem.GRP_ID));
				return uiAssign != dbAssign;
			}
		}
		///// <summary>
		///// 選擇Group後, 初始化項目的選取狀態
		///// </summary>
		//private void SetDefaultEmpList()
		//{
		//	UnAssignedList = null;
		//	AssignedList = null;
		//	if (SelectedGroupItem == null) return;

		//	DoSearchAssignedEmployee();
		//}
		/// <summary>
		/// 工作群組清單
		/// </summary>
		private List<F1953> _groupList;
		public List<F1953> GroupList
		{
			get { return _groupList; }
			set
			{
				_groupList = value;
				RaisePropertyChanged("GroupList");
				RaisePropertyChanged("IsEnableFunctionList");
			}
		}
		#endregion
		//#region Form - 作業群組
		//private F1963 _workgroup;
		//public F1963 Workgroup
		//{
		//	get { return _workgroup; }
		//	set { _workgroup = value; }
		//}
		//private List<F1963> _workgroupList = new List<F1963>();
		//public List<F1963> WorkgroupList
		//{
		//	get { return _workgroupList; }
		//	set { _workgroupList = value; RaisePropertyChanged("WorkgroupList"); }
		//}
		//#endregion
		#region data - 員工清單
		private SelectionList<F1924> _unassignEmpItems = new SelectionList<F1924>(new List<F1924>());

		public SelectionList<F1924> UnassignEmpItems
		{
			get { return _unassignEmpItems; }
			set
			{
				_unassignEmpItems = value;
				RaisePropertyChanged("UnassignEmpItems");
			}
		}

		private SelectionList<F1924> _assignEmpItems = new SelectionList<F1924>(new List<F1924>());

		public SelectionList<F1924> AssignEmpItems
		{
			get { return _assignEmpItems; }
			set
			{
				_assignEmpItems = value;
				RaisePropertyChanged("AssignEmpItems");
			}
		}

		private List<F1924> _allEmp;

		/// <summary>
		/// 存放所有員工
		/// </summary>
		public List<F1924> AllEmp
		{
			get { return _allEmp; }
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

				UnassignEmpItems = new SelectionList<F1924>(new List<F1924>());
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
						if (!GroupList.Any())
						{
							ShowMessage(Messages.InfoNoData);
						}
					},
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						AssignEmpItems = new SelectionList<F1924>(new List<F1924>());
						SelectedGroupItem = GroupList.FirstOrDefault();
					}
				);
			}
		}

		/// <summary>
		/// 取得Group
		/// </summary>
		private void DoSearch()
		{
			SearchGroupName = SearchGroupName.Trim();

			//執行查詢動作
			var proxy = GetProxy<F19Entities>();

			var query = proxy.F1953s.Where(item=>item.ISDELETED == "0");

			if (!string.IsNullOrWhiteSpace( SearchGroupName ))
			{
				query = query.Where(item => item.GRP_NAME.Contains(SearchGroupName));
			}

			GroupList = query.OrderBy(item => item.GRP_ID).ToList();
			//GroupList = proxy.F1953s.Where(x => x.ISDELETED == "0" && (x.GRP_NAME.Contains(SearchGroupName)
			//	|| String.IsNullOrEmpty(SearchGroupName)))
			//	.OrderBy(x => x.GRP_ID).ToList();
			// 預選到第一項
			//IsCheckDataModified = false;
			
			//IsCheckDataModified = true;
		}

		///// <summary>
		///// 取得作業群組
		///// </summary>
		//private void DoSearchWorkgroup()
		//{
		//	//執行查詢動作
		//	var proxy = GetProxy<F19Entities>();
		//	//執行查詢動作
		//	WorkgroupList = proxy.F1963s.Where(x=>x.ISDELETED =="0").OrderBy(x => x.WORK_ID).ToList();
		//	// 預選到第一項
		//	WorkgroupList.Insert(0, new F1963()
		//	{
		//		WORK_ID = 0,
		//		WORK_NAME = Resources.Resources.All,
		//		WORK_DESC = Resources.Resources.All
		//	});
		//	WorkgroupList.Insert(0, new F1963()
		//	{
		//		WORK_ID = -1,
		//		WORK_NAME = Properties.Resources.P1905050000_NoSpecify,
		//		WORK_DESC = Properties.Resources.P1905050000_NoSpecify
		//	});
		//	Workgroup = WorkgroupList.FirstOrDefault();
		//}

		#endregion Search

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query
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
				return CreateBusyAsyncCommand(
					o =>
					{
						if (AssignEmpModified)
						{
							switch (ShowMessage(Messages.WarningBeforeAdd))
							{
								case DialogResponse.Yes:
									if (!DoSave().IsSuccessed)
										return;
									break;
								case DialogResponse.No:
									SelectedGroupItem = SelectedGroupItem;
									break;
								case DialogResponse.Cancel:
									return;
							}
						}

						UserOperateMode = OperateMode.Query;
					},
					() => UserOperateMode != OperateMode.Query
				);
			}
		}
		#endregion

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						if (!AssignEmpModified)
						{
							ShowMessage(Messages.WarningNotModified);
							return;
						}
						else if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
						{
							if (DoSave().IsSuccessed)
							{
								UserOperateMode = OperateMode.Query;
							}
						}

						//if (ConfirmBeforeSave() != DialogResponse.Yes) return;
						//DoSave();
					},
					() => UserOperateMode != OperateMode.Query
				);
			}
		}

		private ExecuteResult DoSave()
		{
			//執行確認儲存動作
			string assigned = GetAssignEmpString(AssignEmpItems.Select(s => s.Item)); //this.GetSelectedItems();

			P19ExDataSource proxyEx = GetExProxy<P19ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("UpdateP190505")
				.AddQueryOption("groupId", string.Format("'{0}'", SelectedGroupItem.GRP_ID.ToString()))
				.AddQueryOption("empId", string.Format("'{0}'", assigned))
				.AddQueryOption("userId", string.Format("'{0}'", this._userId))
				.ToList();
			ShowMessage(result);

			return result.FirstOrDefault();
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
						// 一併做全員工(含作業群組編號)的更新
						DoSearchAllEmployee();
						//DoSearchEmployeeByCondition();
						//MoveToUnAssignedList();

						SetUnassignEmpItemsByCondition();

						if (!UnassignEmpItems.Any())
						{
							ShowMessage(Messages.InfoNoData);
						}
					}
				);
			}
		}

		/// <summary>
		/// 根據過濾條件來顯示未設定人員
		/// </summary>
		private void SetUnassignEmpItemsByCondition()
		{
			EmpId = EmpId.Trim();
			EmpName = EmpName.Trim();

			if (AllEmp == null)
			{
				return;
			}

			// 用唯一的員工清單去篩選條件
			var query = AllEmp.AsEnumerable(); // 之後不會有作業群組的條件 .Where(emp => emp.WORK_ID == Workgroup.WORK_ID);

			if (SelectedDep != null && !string.IsNullOrEmpty(SelectedDep.Value))
			{
				query = query.Where(emp => emp.DEP_ID == SelectedDep.Value);
			}

			if (!string.IsNullOrWhiteSpace(EmpId))
			{
				query = query.Where(emp => emp.EMP_ID.StartsWith(EmpId));
			}

			if (!string.IsNullOrWhiteSpace(EmpName))
			{
				query = query.Where(emp => emp.EMP_NAME.Contains(EmpName));
			}

			// 只顯示不存在於已設定的人員裡
			query = query.Where(emp => !AssignEmpItems.Where(assign => assign.Item.EMP_ID == emp.EMP_ID).Any());

			// TODO 3/6 Walter 因為暫時沒過濾作業群組，所以這邊先用 group by 做單一，等到單位條件加進來後，要移除下面這行
			query = query.GroupBy(emp => emp.EMP_ID).Select(g => g.FirstOrDefault());

			UnassignEmpItems = new SelectionList<F1924>(query);
		}
		///// <summary>
		///// 依篩選條件從既有清單搜尋員工
		///// </summary>
		//private void DoSearchEmployeeByCondition()
		//{
		//	decimal? tmpWorkgroup = 0;
		//	if (Workgroup != null) tmpWorkgroup = Workgroup.WORK_ID;
		//	if (EmpWorkList == null) return;
		//	// 搜尋員工時, 如果是不指定, 就找出所有員工
		//	// 如果是全部, 就找出任何一群組的員工
		//	ValidEmpList = EmpWorkList.Where(
		//		x => (x.EMP_ID.StartsWith(EmpId.Trim()) || string.IsNullOrWhiteSpace(EmpId))
		//			&& (x.EMP_NAME.Contains(EmpName.Trim()) || string.IsNullOrWhiteSpace(EmpName))
		//			&& ((x.WORK_ID != null && tmpWorkgroup == 0)
		//				|| (x.WORK_ID == tmpWorkgroup && tmpWorkgroup > 0)
		//				|| tmpWorkgroup == -1))
		//			.ToList();
		//}

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
		///// 取得該Group的人員
		///// </summary>
		//private void DoSearchAssignedEmployee()
		//{
		//	// 清單初始化
		//	UnAssignedList = new ObservableCollection<F1924>();
		//	AssignedList = new ObservableCollection<F1924>();
		//	List<F1924> tmpAssigned = new List<F1924>();
		//	// 把人員加入到AssignedList, 並從UnAssignedList移除
		//	if (SelectedGroupItem != null)
		//	{
		//		var proxy = GetProxy<F19Entities>();
		//		var tmp = proxy.F192401s.Where(x => x.GRP_ID.Equals(SelectedGroupItem.GRP_ID)).AsQueryable().ToList();

		//		// 如果是第一次進來, 未查詢員工前, EMPLIST會是空的, 所以要去DB取得員工資料來顯示名稱 (要濾掉ISDELETED)
		//		List<F1924> tmpEmpList = new List<F1924>();
		//		if (EmpWorkList == null || !EmpWorkList.Any())
		//		{
		//			tmpEmpList = proxy.F1924s.Where(x => x.ISDELETED != "1").ToList();
		//		}

		//		if (tmp != null && tmp.Count() > 0) // 這裡不能用Any()做判斷, 會掛掉
		//		{
		//			foreach (var p in tmp.OrderBy(x => x.EMP_ID).ToList())
		//			{
		//				// 轉成員工資料, 如果EMPLIST有資料, 就取EMPLIST的值, 否則取tmpEMPLIST的值
		//				F1924 item = new F1924();
		//				item.EMP_ID = p.EMP_ID;
		//				string empName = string.Empty;
		//				if (EmpWorkList != null && EmpWorkList.Any())
		//				{
		//					var tmpEmp = EmpWorkList.FirstOrDefault(x => x.EMP_ID.Equals(p.EMP_ID));
		//					if (tmpEmp != null) empName = tmpEmp.EMP_NAME;
		//					item.EMP_NAME = empName;
		//					// 加入清單
		//					if (tmpEmp != null)  //沒找到不加進來
		//						tmpAssigned.Add(item);
		//				}
		//				else if (tmpEmpList.Any())
		//				{
		//					var tmpEmp = tmpEmpList.FirstOrDefault(x => x.EMP_ID.Equals(p.EMP_ID));
		//					if (tmpEmp != null) empName = tmpEmp.EMP_NAME;
		//					item.EMP_NAME = empName;
		//					// 加入清單
		//					if (tmpEmp != null)  //沒找到不加進來
		//						tmpAssigned.Add(item);
		//				}
		//			}
		//		}
		//	}
		//	AssignedList = tmpAssigned.ToObservableCollection();
		//	// 搜尋符合條件的員工, 此時會一併將已Assign的員工從UnAssign List移除
		//	MoveToUnAssignedList();
		//}
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
				P190505MovingType type = P190505MovingType.None;
				return CreateBusyAsyncCommand(
					o =>
					{
						Enum.TryParse(o.ToString(), out type);
						//if (Enum.TryParse(o.ToString(), out type))
						//{
						//	DoMoveItems(type);
						//}
					},
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						switch (type)
						{
							case P190505MovingType.Assign:
								MoveItems(UnassignEmpItems, AssignEmpItems, isMoveAll: false);
								break;
							case P190505MovingType.UnAssign:
								MoveItems( AssignEmpItems, UnassignEmpItems,isMoveAll: false);
								break;
							case P190505MovingType.AssignAll:
								MoveItems(UnassignEmpItems, AssignEmpItems, isMoveAll: true);
								break;
							case P190505MovingType.UnAssignAll:
								MoveItems(AssignEmpItems, UnassignEmpItems, isMoveAll: true);
								break;
						}
						ScrollIntoViewLast(type);
					}
				);
			}
		}

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

		///// <summary>
		///// 移動項目
		///// </summary>
		///// <param name="type"></param>
		//public void DoMoveItems(P190505MovingType type)
		//{
		//	// 因ObservableCollection不允許在多執行緒下變更其內容, 但為達到資料更新畫面即更新, 所以用暫存(tmpList)來搬動項目
		//	List<F1924> tmpAssigned = new List<F1924>();
		//	List<F1924> tmpUnAssigned = new List<F1924>();
		//	tmpAssigned = AssignedList.ToList();
		//	tmpUnAssigned = UnAssignedList.ToList();
		//	switch (type)
		//	{
		//		case P190505MovingType.Assign:
		//			if (SelectedEmpList == null || !SelectedEmpList.Any()) break;
		//			foreach (var p in SelectedEmpList)
		//			{
		//				tmpAssigned.Add(p);
		//				tmpUnAssigned.Remove(p);
		//			}
		//			AssignedList = tmpAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			UnAssignedList = tmpUnAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			break;
		//		case P190505MovingType.AssignAll:
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
		//		case P190505MovingType.UnAssign:
		//			if (SelectedEmpList == null || !SelectedEmpList.Any()) break;
		//			foreach (var p in SelectedEmpList)
		//			{
		//				tmpAssigned.Remove(p);

		//				// Memo: 不檢查了 - 2015/1/30
		//				// 這裡要去檢查該員工是否符合查詢條件
		//				// if (IsValidEmployee(p.EMP_ID)) 
		//				tmpUnAssigned.Add(p);
		//			}
		//			AssignedList = tmpAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			UnAssignedList = tmpUnAssigned.OrderBy(x => x.EMP_ID).ToObservableCollection();
		//			break;
		//		case P190505MovingType.UnAssignAll:
		//			if (AssignedList == null || !AssignedList.Any()) break;
		//			foreach (var p in AssignedList)
		//			{
		//				// Memo: 不檢查了 - 2015/1/30
		//				// 這裡要去檢查該員工是否符合查詢條件
		//				// if (!IsValidEmployee(p.EMP_ID)) continue;
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

		///// <summary>
		///// 儲存前做資料確認
		///// </summary>
		///// <returns></returns>
		//private DialogResponse ConfirmBeforeSave()
		//{
		//	DialogResponse dr = DialogResponse.Yes;
		//	if (!DataModified() || SelectedGroupItem == null)
		//	{
		//		ShowMessage(Messages.WarningNotModified);
		//		dr = DialogResponse.Cancel;
		//		return dr;
		//	}
		//	else
		//	{
		//		return ShowMessage(Messages.WarningBeforeUpdate);
		//	}
		//}

		///// <summary>
		///// 檢查資料是否有修改
		///// </summary>
		///// <returns></returns>
		//public bool DataModified()
		//{
		//	if (SelectedGroupItem == null) return false;
		//	// 再取一次資料庫的資料
		//	var proxy = GetProxy<F19Entities>();
		//	var tmpAssigned = proxy.F192401s.Where(x => x.GRP_ID.Equals(SelectedGroupItem.GRP_ID)).ToList();
		//	// 取得已刪除的emp id
		//	var tmpDeleted = proxy.F1924s.Where(x => x.ISDELETED == "1").ToList();
		//	// 檢查時要濾掉已被刪除的emp id
		//	var tmpSrc = from a in tmpAssigned
		//				 join b in tmpDeleted on a.EMP_ID equals b.EMP_ID into d
		//				 from c in d.DefaultIfEmpty()
		//				 where c == null
		//				 select a.EMP_ID;
		//	string src = string.Empty;
		//	src = string.Join(",", tmpSrc.OrderBy(x => x));

		//	// 然後跟目前選擇的員工資料做比對
		//	string dest = this.GetSelectedItems();

		//	return !src.Equals(dest);
		//}

		///// <summary>
		///// 取得已選擇的員工, 用EMP_ID組成逗號分隔的字串
		///// </summary>
		///// <returns></returns>
		//private string GetSelectedItems()
		//{
		//	string dest = string.Empty;
		//	if (AssignedList != null)
		//	{
		//		foreach (var p in AssignedList.OrderBy(x => x.EMP_ID).ToList())
		//		{
		//			dest += "," + p.EMP_ID;
		//		}
		//		if (!string.IsNullOrEmpty(dest)) dest = dest.Substring(1);
		//	}
		//	return dest;
		//}
		#endregion
	}

	#region enum
	public enum P190505MovingType
	{
		None,
		AssignAll,
		UnAssignAll,
		Assign,
		UnAssign
	}
	#endregion
}
