/* 2015/03/05 Walter
 * 原本的操作方式為查詢即為編輯模式，由於從 DataGrid 的 SelectionChanged 做太複雜，
 * 同時也去除沒有使用 ReloadCommand，改為有編輯按鈕，簡化邏輯流程，同時修正 DataGrid 會一直跳第一筆 Focus，變成要很複雜的邏輯才能修正。
 * ----------------------------------------
* Note: TreeViewService不做連動勾選, 以免誤判為Properties.Resources.P1905040000_Edit的狀態, 會一直跳出提示訊息
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using System.Text.RegularExpressions;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1905040000_ViewModel : InputViewModelBase
	{
		public Action<OperateMode> UserOperateModeFocus = delegate { };
		public Action<P190504FunNode> BringItemIntoView = (x) => { };
		private string _userId;

		public P1905040000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;

				InitControls();
			}

		}
		#region 頁面初始化
		private void InitControls()
		{
			SearchCommand.Execute(null);
		}
		#endregion

		#region 資料連結/ 頁面參數


		#region Form - 工作群組名稱
		private string _searchGroupName = string.Empty;
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Required_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public string SearchGroupName
		{
			get { return _searchGroupName; }
			set { _searchGroupName = value; RaisePropertyChanged("SearchGroupName"); }
		}
		#endregion
		#region Form - 工作群組清單
		private F1953 _editableGroupItem;

		public F1953 EditableGroupItem
		{
			get { return _editableGroupItem; }
			set
			{
				_editableGroupItem = value;
				RaisePropertyChanged("EditableGroupItem");
			}
		}

		private F1953 _selectedGroupItem;
		public F1953 SelectedGroupItem
		{
			get
			{
				return _selectedGroupItem;
			}
			set
			{
				Set(() => SelectedGroupItem, ref _selectedGroupItem, value);

				EditableGroupItem = (value != null) ? AutoMapper.Mapper.DynamicMap<F1953>(value) : null;
				var grpId = (EditableGroupItem == null) ? 0 : EditableGroupItem.GRP_ID;
				SetP190504FunNodes(grpId);
				SetScheduleAuthorize();
			}
		}



		private void SetScheduleAuthorize()
		{
			var proxy00 = GetProxy<F00Entities>();
			var f000904Data = GetBaseTableService.GetF000904List(FunctionCode, "Schedule", "Authorize");

			var proxy = GetProxy<F19Entities>();
			var f195302Data = new List<F195302>();
			if (SelectedGroupItem != null)
			{
				f195302Data =
					(from p in proxy.F195302s
					 where p.GRP_ID.Equals(SelectedGroupItem.GRP_ID)
					 select p).ToList();
			}

			ScheduleAuthorizeData =
				(from n in f000904Data
				 join p in f195302Data on n.Value equals p.SCHEDULE_ID
					 into temp
				 from ds in temp.DefaultIfEmpty()
				 select new ScheduleAuthorize
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

		private void ResetScheduleAuthorize()
		{
			foreach (var s in ScheduleAuthorizeData)
			{
				s.IsSelected = false;
				s.IsSelectedOld = false;
			}
			IsScheduleAuthorizeAll = false;
		}

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
				//RaisePropertyChanged("IsEnableFunctionList");
			}
		}

		private ScheduleAuthorize _selectScheduleAuthorize;
		public ScheduleAuthorize SelectScheduleAuthorize
		{
			get { return _selectScheduleAuthorize; }
			set { _selectScheduleAuthorize = value; RaisePropertyChanged("SelectScheduleAuthorize"); }
		}

		private SelectionList<ScheduleAuthorize> _scheduleAuthorizeData;
		public SelectionList<ScheduleAuthorize> ScheduleAuthorizeData
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
						DoSearch();
						if (!GroupList.Any())
						{
							ShowMessage(Messages.InfoNoData);
							UserOperateModeFocus(UserOperateMode);
						}
					},
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						// 預選到第一項
						SelectedGroupItem = GroupList.FirstOrDefault();
					}
				);
			}
		}

		private void DoSearch(decimal groupId = 0)
		{
			F19Entities proxy = GetProxy<F19Entities>();
			//執行查詢動作
			if (!String.IsNullOrEmpty(SearchGroupName))
			{
				GroupList = proxy.F1953s.Where(x => x.ISDELETED == "0" && x.GRP_NAME.Contains(SearchGroupName)).OrderBy(x => x.GRP_ID).ToList();
			}
			else
			{
				GroupList = proxy.F1953s.Where(x => x.ISDELETED == "0").OrderBy(x => x.GRP_ID).ToList();
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
					() => UserOperateMode == OperateMode.Query
				);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			UserOperateModeFocus(UserOperateMode);
			EditableGroupItem = new F1953();
			SetP190504FunNodes();
			ResetScheduleAuthorize();
		}

		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && EditableGroupItem != null
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			UserOperateModeFocus(UserOperateMode);
			//RaisePropertyChanged("IsEnableFunctionList");
			//執行編輯動作
		}
		#endregion Edit

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						if (ConfirmBeforeDelete() == DialogResponse.Yes)
						{
							DoDelete();
						}
					},
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						SelectedGroupItem = GroupList.FirstOrDefault();
					}
				);
			}
		}
		private DialogResponse ConfirmBeforeDelete()
		{
			F19Entities proxy = GetProxy<F19Entities>();
			if (proxy.F192401s.Where(x => x.GRP_ID.Equals(SelectedGroupItem.GRP_ID)).Count() > 0)
			{
				ShowMessage(Messages.WarningCannotDeleteGroup);
				return DialogResponse.Cancel;
			}
			DialogResponse dr = ShowMessage(Messages.WarningBeforeDelete);
			proxy = null;
			return dr;
		}
		private void DoDelete()
		{
			//執行刪除動作, 透過Service做刪除才會有Transaction
			P19ExDataSource proxyEx = GetExProxy<P19ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("DeleteP190504")
				.AddQueryExOption("groupId", SelectedGroupItem.GRP_ID.ToString())
				.AddQueryExOption("userId", this._userId)
				.ToList();
			proxyEx = null;
			ShowMessage(result);
			DoSearch();
			//ReloadData();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool saveResult = false;
				decimal grpId = 0;
				return CreateBusyAsyncCommand(
					o =>
					{
						// 外部變數的參考 saveResult 即為擷取委派建立時。 與區域變數不同的是擷取變數的存留期會延伸直到參考匿名方法的委派可進行記憶體回收為止。
						grpId = EditableGroupItem.GRP_ID;
						saveResult = false;

						// 資料檢核
						ExDataMapper.Trim(EditableGroupItem);

						if (!ValidateInputData())
						{
							return;
						}

						if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
						{
							saveResult = DoSave();
						}
					},
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						if (saveResult)
						{
							if (UserOperateMode == OperateMode.Add)
							{
								SelectedGroupItem = GroupList.LastOrDefault();
							}
							else
							{
								SelectedGroupItem = GroupList.FirstOrDefault(item => item.GRP_ID == grpId);
							}

							UserOperateMode = OperateMode.Query;
						}
					}
				);
			}
		}

		private bool ValidateInputData()
		{
			if (ExistsGroupName())
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1905040000_WorkGroupExist });
				return false;
			}

			if (string.IsNullOrWhiteSpace(EditableGroupItem.GRP_NAME))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1905040000_InputWorkGroupName });
				return false;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				if (IsNotSelectedF190504s())
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1905040000_InputExtensionFunc });
					return false;
				}
			}

			return true;
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			// 取得已選取的項目, 之後要寫入到F195301
			var selectedFunctionString = CombinSelectedFunctionString();
			var selectedScheduleIdString = GetScheduleIdString();

			// 叫用ExDataService, 傳入GroupID, GroupName, GroupDesc, ShowInfo, SelectedItems (list<string>)
			wcf.ExecuteResult result;
			var proxyWcf = new wcf.P19WcfServiceClient();
			if (UserOperateMode == OperateMode.Add)
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
							() => proxyWcf.AddP190504(EditableGroupItem.GRP_NAME,
														EditableGroupItem.GRP_DESC,
														EditableGroupItem.SHOWINFO,
														selectedFunctionString,
														selectedScheduleIdString,
														this._userId));
			}
			else
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
							() => proxyWcf.UpdateP190504(EditableGroupItem.GRP_ID.ToString(),
														EditableGroupItem.GRP_NAME,
														EditableGroupItem.GRP_DESC,
														EditableGroupItem.SHOWINFO,
														selectedFunctionString,
														selectedScheduleIdString,
														this._userId));
			}

			if (result.IsSuccessed)
			{
				DoSearch();
			}

			var msg = result.IsSuccessed ? Messages.Success
														: new MessagesStruct() { Message = result.Message };
			ShowMessage(msg);

			return result.IsSuccessed;
		}

		/// <summary>
		/// 工作群組名稱是否重複
		/// </summary>
		/// <returns></returns>
		private bool ExistsGroupName()
		{
			var proxy = GetProxy<F19Entities>();

			var query = (from item in proxy.F1953s
						 where item.GRP_NAME == EditableGroupItem.GRP_NAME
						 where item.GRP_ID != EditableGroupItem.GRP_ID
						 where item.ISDELETED != "1"
						 select item).ToList();

			return query.Any();
		}

		/// <summary>
		/// 是否沒選擇功能選單
		/// </summary>
		/// <returns></returns>
		private bool IsNotSelectedF190504s()
		{
			return string.IsNullOrEmpty(CombinSelectedFunctionString());
		}

		private string CombinSelectedFunctionString()
		{
			return string.Join(",", GetCheckedFunNodes(P190504FunCodes, x => x.IsChecked && x.F1954Data != null).Select(x => x.F1954Data.FUN_CODE));
		}

		private string GetScheduleIdString()
		{
			return string.Join(",", ScheduleAuthorizeData.Where(x => x.IsSelected).Select(x => x.Item.SCHEDULE_ID));
		}


		/// <summary>
		/// 取得所有已選取的項目
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		private List<string> GetAllSelectedTreeViewItems(P190504TreeView parent)
		{
			List<string> items = new List<string>();
			foreach (P190504TreeView item in parent.TreeView)
			{
				if (item.IsChecked == true) items.Add(item.Id);
				List<string> tmp = GetAllSelectedTreeViewItems(item);
				if (tmp != null)
				{
					foreach (string t in tmp) items.Add(t);
				}
			}
			return items.OrderBy(x => x.Trim()).ToList();
		}
		/// <summary>
		/// 依項目ID, 找到項目後設定為選取狀態
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		private void SetTreeViewItemChecked(P190504TreeView parent, string id)
		{
			foreach (P190504TreeView item in parent.TreeView)
			{
				if (item.Id.Equals(id)) { item.IsInitial = true; item.IsChecked = true; return; };
				SetTreeViewItemChecked(item, id);
			}
		}
		/// <summary>
		/// 重設所有項目為未選取
		/// </summary>
		/// <param name="parent"></param>
		private void ResetTreeViewDefaultNonSelected(P190504TreeView parent)
		{
			if (parent == null) return;
			foreach (P190504TreeView item in parent.TreeView)
			{
				item.IsInitial = true;
				item.IsChecked = false;
				ResetTreeViewDefaultNonSelected(item);
			}
		}
		#endregion Save

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
						// 提示視窗選擇 Cancel，表示保持原畫面，就不用更改任何資料
						if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
						{
							if (SelectedGroupItem != null)
								SelectedGroupItem = GroupList.FirstOrDefault(item => item.GRP_ID == SelectedGroupItem.GRP_ID);
							UserOperateMode = OperateMode.Query;
						}

					}
				);
			}
		}



		/// <summary>
		/// 傳回 Yes:儲存並取消, No: 單純取消, Cancel: 保留在原畫面，這些結果是提示視窗的訊息
		/// </summary>
		/// <returns></returns>
		private void DoCancel()
		{
		}

		#endregion Cancel

		#region Select All
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
								SetCheckAll(transEnum);
							}
						},
						(t) => { return UserOperateMode != OperateMode.Query; }
						);
				}
				return _checkAllTask;
			}
		}

		private void SetCheckAll(GroupType groupType)
		{
			if (groupType == GroupType.ScheduleAuthorize ||
					groupType == GroupType.None)
			{
				foreach (var item in ScheduleAuthorizeData)
					item.IsSelected = IsScheduleAuthorizeAll;
			}
		}

		public enum GroupType
		{
			None,
			/// <summary>
			/// 排程權限
			/// </summary>
			ScheduleAuthorize
		}
		#endregion

		#endregion

		public class ScheduleAuthorize : F195302
		{
			public bool Checked { get; set; }
			public string ScheduleName { get; set; }
		}

		#region 每次重新產生樹狀節點時，若 LastSelectedFunctionId 有值，則做 Focus
		/// <summary>
		/// 每次重新產生樹狀節點時，若 LastSelectedFunctionId 有值，則做 Focus
		/// </summary>
		public string LastSelectedFunctionId { get; set; }

		#endregion

		#region 樹狀節點 Binding 物件

		private List<P190504FunNode> _p190504FunCodes;

		public List<P190504FunNode> P190504FunCodes
		{
			get { return _p190504FunCodes; }
			set
			{
				Set(() => P190504FunCodes, ref _p190504FunCodes, value);
			}
		}

		private P190504FunNode _selectedP190504FunNode;

		public P190504FunNode SelectedP190504FunNode
		{
			get { return _selectedP190504FunNode; }
			set
			{
				Set(() => SelectedP190504FunNode, ref _selectedP190504FunNode, value);
				if (value != null)
				{
					LastSelectedFunctionId = value.F1954Data.FUN_CODE;
				}
			}
		}


		#endregion

		#region 產生功能清單樹狀節點物件
		private List<F1954> _f1954List = null;
		/// <summary>
		/// 所有程式功能
		/// </summary>
		public List<F1954> F1954List
		{
			get
			{
				if (_f1954List == null)
				{
					var proxy = GetProxy<F19Entities>();
					_f1954List = proxy.F1954s.Where(x => x.DISABLE == "0")
											 .Select(x => new F1954
														{
															FUN_CODE = x.FUN_CODE,
															FUN_NAME = x.FUN_NAME
														})
											 .ToList();
				}

				return _f1954List;
			}
		}

		public List<F195301> GetF195301List(decimal grpId)
		{
			var proxy = GetProxy<F19Entities>();
			return proxy.F195301s.Where(x => x.GRP_ID == grpId)
								 .Select(x => new F195301
								 {
									 GRP_ID = x.GRP_ID,
									 FUN_CODE = x.FUN_CODE
								 }).ToList();
		}

		void SetP190504FunNodes(decimal grpId = 0)
		{
			// 重新繪製功能清單
			P190504FunCodes = GetP190504FunNodeList(grpId);

			// 將焦點放到最後一次選擇或打勾的項目
			if (LastSelectedFunctionId != null)
			{
				SelectedP190504FunNode = GetCheckedFunNodes(P190504FunCodes,
															x => x.F1954Data != null && x.F1954Data.FUN_CODE == LastSelectedFunctionId).FirstOrDefault();
				if (SelectedP190504FunNode != null)
					BringItemIntoView(SelectedP190504FunNode);
			}
		}

		public List<P190504FunNode> GetP190504FunNodeList(decimal grpId)
		{
			var f1954List = F1954List;
			var f195301List = (grpId == 0) ? new List<F195301>() : GetF195301List(grpId);

			var f1954WithF195301s = (from f1954 in f1954List
									 join f195301 in f195301List
									 on f1954.FUN_CODE equals f195301.FUN_CODE into f195301s
									 from f195301 in f195301s.DefaultIfEmpty()
									 orderby f1954.FUN_CODE
									 select new
									 {
										 F1954 = f1954,
										 IsChecked = f195301 != null
									 }).ToList();

			var p190504FunNodes = new List<P190504FunNode>();
			var root = new P190504FunNode(Properties.Resources.P1905040000_FuncList, null,0);
			root.IsExpend = true;
			p190504FunNodes.Add(root);

			foreach (var p1 in f1954WithF195301s.Where(x => IsLevelNode(x.F1954.FUN_CODE, 0)))
			{
				// P01
				var firstMenu = new P190504FunNode(p1.F1954, p1.IsChecked, root,1);
				root.FunNodes.Add(firstMenu);

				foreach (var p2 in f1954WithF195301s.Where(x => IsLevelNode(x.F1954.FUN_CODE, 1, firstMenu.ModuleId)))
				{
					// P0101
					var secondMenu = new P190504FunNode(p2.F1954, p2.IsChecked, firstMenu,2);
					firstMenu.FunNodes.Add(secondMenu);

					foreach (var p3 in f1954WithF195301s.Where(x => IsLevelNode(x.F1954.FUN_CODE, 2, secondMenu.ModuleId)))
					{
						// P010101 主UI
						var thirdMenu = new P190504FunNode(p3.F1954, p3.IsChecked, secondMenu,3);
						secondMenu.FunNodes.Add(thirdMenu);

						foreach (var p4 in f1954WithF195301s.Where(x => x.F1954.FUN_CODE.StartsWith("B" + thirdMenu.ModuleId + "00")))
						{
							// BP010101XXXX 主UI按鈕
							var fourthMenu = new P190504FunNode(p4.F1954, p4.IsChecked, thirdMenu,4);
							thirdMenu.FunNodes.Add(fourthMenu);
						}

						// 從主UI模組下找子UI
						foreach (var p31 in f1954WithF195301s.Where(x => IsLevelNode(x.F1954.FUN_CODE, 3, thirdMenu.ModuleId)))
						{
							// P01010101 子UI
							var thirdChildMenu = new P190504FunNode(p31.F1954, p31.IsChecked, secondMenu,4);
							secondMenu.FunNodes.Add(thirdChildMenu);

							foreach (var p4 in f1954WithF195301s.Where(x => x.F1954.FUN_CODE.StartsWith("B" + thirdChildMenu.ModuleId)))
							{
								// BP01010101XX 子UI按鈕
								var fourthMenu = new P190504FunNode(p4.F1954, p4.IsChecked, thirdChildMenu,4);
								thirdChildMenu.FunNodes.Add(fourthMenu);
							}
						}
					}
				}
			}

			root.IsChecked = root.FunNodes.Any(x => x.IsChecked);
			return p190504FunNodes;
		}

		/// <summary>
		/// 是否為非按鈕的階層 FunctionId 節點
		/// </summary>
		/// <param name="funId"></param>
		/// <param name="level">從0開始，0就是比對是否為P01</param>
		/// <returns></returns>
		static bool IsLevelNode(string funId, int level, string moduleId = null)
		{
			var skip = 1 + (level * 2);
			var start = funId.Skip(skip).Take(2).Any(x => x != '0');
			var end = funId.Skip(skip + 2).All(x => x == '0');
			var isModule = (moduleId != null) ? funId.StartsWith(moduleId) : true;
			// 不是按鈕，也起始與結尾都符合該階層條件
			return !funId.StartsWith("B") && start && end && isModule;
		}
		#endregion

		#region 打勾連動上下層
		public void DoCheckNodes(P190504FunNode selectedNode)
		{
			LastSelectedFunctionId = (selectedNode.F1954Data == null) ? null : selectedNode.F1954Data.FUN_CODE;
			RecursiveChildren(selectedNode, selectedNode.IsChecked);
			RecursiveParent(selectedNode.Parent);
		}

		private void RecursiveChildren(P190504FunNode current, bool isChecked)
		{
			foreach (var node in current.FunNodes)
			{
				node.IsChecked = isChecked;
				RecursiveChildren(node, isChecked);
			}
		}

		private void RecursiveParent(P190504FunNode current)
		{
			if (current == null)
				return;

			// 功能是其中一個子功能有勾，就算有使用父功能
			current.IsChecked = current.FunNodes.Any(x => x.IsChecked);
			RecursiveParent(current.Parent);
		}
		#endregion

		#region 取回符合條件的節點
		IEnumerable<P190504FunNode> GetCheckedFunNodes(IEnumerable<P190504FunNode> nodes, Func<P190504FunNode, bool> predicate)
		{
			foreach (var node in nodes)
			{
				// 最後傳回位置
				if (predicate(node))
					yield return node;

				foreach (var item in GetCheckedFunNodes(node.FunNodes, predicate))
				{
					yield return item;
				}
			}
		}
		#endregion
	}

}
