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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.UILib;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
//using Wms3pl.WpfClient.

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public class P7106010000_ViewModel : InputViewModelBase
	{
		public P7106010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				if (Wms3plSession.Get<UserInfo>().Account == "wms")
					IsWmsUser = Visibility.Visible;
				else
					IsWmsUser = Visibility.Collapsed;

				InitData();
			}

		}

		#region Property
		private Visibility _isWmsUser;
		public Visibility IsWmsUser
		{
			get { return _isWmsUser; }
			set { Set(ref _isWmsUser, value); }
		}

		private SelectTab _selectedTab;
		public SelectTab SelectedTab
		{
			get { return _selectedTab; }
			set
			{
				Set(ref _selectedTab, value);
				SelectedTopic = string.Empty;
				SelectedSubTopic = string.Empty;
			}
		}

		private bool _enableMain;
		public bool EnableMain
		{
			get { return _enableMain; }
			set { Set(ref _enableMain, value); }
		}

		private bool _enableLang;
		public bool EnableLang
		{
			get { return _enableLang; }
			set { Set(ref _enableLang, value); }
		}

		private ObservableCollection<NameValuePair<string>> _topicList;
		public ObservableCollection<NameValuePair<string>> TopicList
		{
			get { return _topicList; }
			set { Set(ref _topicList, value); }
		}

		private string _selectedTopic;
		public string SelectedTopic
		{
			get { return _selectedTopic; }
			set
			{
				Set(ref _selectedTopic, value);
				SetSubTopicList();
			}
		}

		private ObservableCollection<NameValuePair<string>> _subTopicList;
		public ObservableCollection<NameValuePair<string>> SubTopicList
		{
			get { return _subTopicList; }
			set { Set(ref _subTopicList, value); }
		}

		private string _selectedSubTopic;
		public string SelectedSubTopic
		{
			get { return _selectedSubTopic; }
			set
			{
				Set(ref _selectedSubTopic, value);
				_searchDatas = new List<F000904>();
				DgSource = null;
				DgLangSource = null;
				_tmpLangs = new List<P710601LangData>();
			}
		}

		private SelectionList<F000904> _dgSource;
		public SelectionList<F000904> DgSource
		{
			get { return _dgSource; }
			set { Set(ref _dgSource, value); }
		}

		private SelectionItem<F000904> _selectDgSource;
		public SelectionItem<F000904> SelectDgSource
		{
			get { return _selectDgSource; }
			set { Set(ref _selectDgSource, value); }
		}

		private ObservableCollection<P710601LangData> _dgLangSource;
		public ObservableCollection<P710601LangData> DgLangSource
		{
			get { return _dgLangSource; }
			set { Set(ref _dgLangSource, value); }
		}

		private P710601LangData _selectDgLangSource;
		public P710601LangData SelectDgLangSource
		{
			get { return _selectDgLangSource; }
			set { Set(ref _selectDgLangSource, value); }
		}

		private bool _isCheckAll;
		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				Set(ref _isCheckAll, value);
				if (DgSource != null)
					DgSource = new SelectionList<F000904>(DgSource.Select(o => o.Item).ToList(), IsCheckAll);
			}
		}

		private List<F000904> _searchDatas;

		private string _addTopic;
		public string AddTopic
		{
			get { return _addTopic; }
			set { Set(ref _addTopic, value); }
		}
		private string _addSubTop;
		public string AddSubTop
		{
			get { return _addSubTop; }
			set { Set(ref _addSubTop, value); }
		}
		private string _addSubTopName;
		public string AddSubTopName
		{
			get { return _addSubTopName; }
			set { Set(ref _addSubTopName, value); }
		}
		private string _addValue;
		public string AddValue
		{
			get { return _addValue; }
			set { Set(ref _addValue, value); }
		}
		private string _addName;
		public string AddName
		{
			get { return _addName; }
			set { Set(ref _addName, value); }
		}

		private string _isUsage;
		public string IsUsage
		{
			get { return _isUsage; }
			set { Set(ref _isUsage, value); }
		}

		private ObservableCollection<NameValuePair<string>> _langList;
		public ObservableCollection<NameValuePair<string>> LangList
		{
			get { return _langList; }
			set { Set(ref _langList, value); }
		}

		private string _selectedLang;
		public string SelectedLang
		{
			get { return _selectedLang; }
			set
			{
				Set(ref _selectedLang, value);
				DgLangSource = null;
				_tmpLangs = new List<P710601LangData>();
			}
		}

		private List<P710601LangData> _tmpLangs;
		#endregion

		#region Method
		/// <summary>
		/// 初始化畫面
		/// </summary>
		private void InitData()
		{
			SetTopicList();
			GetLangList();
			EnableMain = true;
			EnableLang = true;
			SearchCommand.Execute(null);
		}
		/// <summary>
		/// 設定Topic選單
		/// </summary>
		private void SetTopicList()
		{
			var proxy = GetProxy<F00Entities>();
			var topList = proxy.F000904s.ToList().Select(o => o.TOPIC).Distinct().ToList();
			TopicList = (from a in topList select new NameValuePair<string> { Value = a, Name = a }).OrderBy(o => o.Value).ToObservableCollection();
			TopicList.Insert(0, new NameValuePair<string> { Value = string.Empty, Name = Resources.Resources.All });
			SelectedTopic = TopicList.FirstOrDefault().Value;
		}
		/// <summary>
		/// 設定Subtopic選單
		/// </summary>
		private void SetSubTopicList()
		{
			if (!string.IsNullOrEmpty(SelectedTopic))
			{
				var proxy = GetProxy<F00Entities>();
				var subTopic = proxy.F000904s.ToList().Where(o => o.TOPIC == SelectedTopic).ToList();
				SubTopicList = (from a in subTopic.GroupBy(o => o.SUBTOPIC) select new NameValuePair<string> { Value = a.Key, Name = a.Key }).ToObservableCollection();
				SubTopicList.Insert(0, new NameValuePair<string> { Value = string.Empty, Name = Resources.Resources.All });
				SelectedSubTopic = SubTopicList.FirstOrDefault().Value;
			}
			else
			{
				SubTopicList = new ObservableCollection<NameValuePair<string>>();
				SubTopicList.Insert(0, new NameValuePair<string> { Value = string.Empty, Name = Resources.Resources.All });
				SelectedSubTopic = SubTopicList.FirstOrDefault().Value;
			}
		}
		/// <summary>
		/// 取得語系選單
		/// </summary>
		private void GetLangList()
		{
			var proxy = GetProxy<F00Entities>();
			var topList = proxy.F000904s.ToList().Where(o => o.TOPIC == "Global" && o.SUBTOPIC == "LANG").ToList();
			LangList = (from a in topList
						where a.ISUSAGE == "1"
						select new NameValuePair<string> { Value = a.VALUE, Name = a.NAME }).ToObservableCollection();
			if (LangList == null || !LangList.Any())
				LangList = topList.Where(o=>o.VALUE == "zh-TW").Select(o=>new NameValuePair<string> { Value = o.VALUE,Name = o.NAME}).ToObservableCollection();

			if (LangList != null && LangList.Any())
				SelectedLang = LangList.FirstOrDefault().Value;
		}
		/// <summary>
		/// 設定是否啟用Tab
		/// </summary>
		private void SetEnableTab()
		{
			switch (SelectedTab)
			{
				case SelectTab.Tab1:
					EnableMain = true;
					EnableLang = false;
					break;
				case SelectTab.Tab2:
					EnableMain = false;
					EnableLang = true;
					break;
			}
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

		private void DoSearch()
		{
			//執行查詢動作
			if (SelectedTab == SelectTab.Tab1)
			{
				var proxy = GetProxy<F00Entities>();
				_searchDatas = proxy.F000904s.ToList();
				if (!string.IsNullOrEmpty(SelectedTopic))
					_searchDatas = _searchDatas.Where(o => o.TOPIC == SelectedTopic).ToList();
				if (!string.IsNullOrEmpty(SelectedSubTopic))
					_searchDatas = _searchDatas.Where(o => o.SUBTOPIC == SelectedSubTopic).ToList();
				var temp = new List<F000904>();
				_searchDatas.ForEach(o => temp.Add(AutoMapper.Mapper.DynamicMap<F000904>(o)));
				DgSource = temp.ToSelectionList();
			}
			else if (SelectedTab == SelectTab.Tab2)
			{
				var proxyEx = GetExProxy<P71ExDataSource>();
				DgLangSource = proxyEx.CreateQuery<P710601LangData>("GetP710601LangDataList")
				.AddQueryExOption("topic", SelectedTopic)
				.AddQueryExOption("subtopic", SelectedSubTopic)
				.AddQueryExOption("lang", SelectedLang)
				.ToObservableCollection();
				if (_tmpLangs == null)
				{
					_tmpLangs = new List<P710601LangData>();
				}
				var langList = DgLangSource.ToList();
				langList.ForEach(o => _tmpLangs.Add(AutoMapper.Mapper.DynamicMap<P710601LangData>(o)));
			}
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query && SelectedTab == SelectTab.Tab1
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作
			_searchDatas = new List<F000904>();
			DgSource = null;
			SetEnableTab();
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedTab == SelectTab.Tab1 ? (DgSource != null && DgSource.Any()) : (UserOperateMode == OperateMode.Query && SelectedTab == SelectTab.Tab2 ? DgLangSource != null && DgLangSource.Any() : false)
					);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
			SetEnableTab();
			UserOperateMode = OperateMode.Edit;
		}
		#endregion Edit

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
			DoSearch();
			EnableMain = true;
			EnableLang = true;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				bool isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoDelete(), () => UserOperateMode == OperateMode.Query && DgSource != null && DgSource.Where(x => x.IsSelected).Any(), o =>
					{
						if (isSuccess)
						{
							SetTopicList();
							SearchCommand.Execute(null);
						}
					}
					);
			}
		}

		private bool DoDelete()
		{
			//執行刪除動作
			var deletedData = DgSource.Where(x => x.IsSelected).Select(o => o.Item).ToList();
			var wcfDeletedData = new List<wcf.F000904>();
			deletedData.ForEach(o => { wcfDeletedData.Add(o.Map<F000904, wcf.F000904>()); });
			var proxy = GetWcfProxy<wcf.P71WcfServiceClient>();
			var reslut = proxy.RunWcfMethod(x => x.DeletedOrUpdateP710601(true, wcfDeletedData.ToArray()));
			if (reslut.IsSuccessed)
			{
				ShowMessage(Messages.DeleteSuccess);
				return true;
			}
			else
				return false;
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(), () => UserOperateMode != OperateMode.Query, o =>
					{
						SetTopicList();
						GetLangList();
						SearchCommand.Execute(null);
						AddTopic = string.Empty;
						AddSubTop = string.Empty;
						AddSubTopName = string.Empty;
						AddName = string.Empty;
						AddValue = string.Empty;
						IsUsage = "0";
					}
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			var reslut = new wcf.ExecuteResult();
			var proxy = GetWcfProxy<wcf.P71WcfServiceClient>();
			if (UserOperateMode == OperateMode.Edit)
			{
				if (SelectedTab == SelectTab.Tab1)
				{
					var editData = DgSource.Select(o => o.Item).ToList();

					var updates = new List<F000904>();
					updates = (from a in editData
							   join b in _searchDatas
							   on new { a.TOPIC, a.SUBTOPIC, a.VALUE } equals new { b.TOPIC, b.SUBTOPIC, b.VALUE }
							   where (a.NAME != b.NAME) || (a.ISUSAGE != b.ISUSAGE)
							   select a).ToList();

					var wcfDeletedData = new List<wcf.F000904>();
					updates.ForEach(o => { wcfDeletedData.Add(o.Map<F000904, wcf.F000904>()); });

					reslut = proxy.RunWcfMethod(x => x.DeletedOrUpdateP710601(false, wcfDeletedData.ToArray()));
					if (reslut.IsSuccessed)
						ShowMessage(Messages.InfoUpdateSuccess);
				}
				else
				{
					var insetrUpdate = new List<P710601LangData>();
					insetrUpdate = (from a in DgLangSource
									join b in _tmpLangs
									on new { a.TOPIC, a.SUBTOPIC, a.VALUE } equals new { b.TOPIC, b.SUBTOPIC, b.VALUE }
									where (a.LANGNAME != b.LANGNAME)
									select a).ToList();
					var wcfInsertUpdateData = new List<wcf.P710601LangData>();
					insetrUpdate.ForEach(o => wcfInsertUpdateData.Add(o.Map<P710601LangData, wcf.P710601LangData>()));
					reslut = proxy.RunWcfMethod(x => x.InsertOrUpdateLang(SelectedTopic, SelectedSubTopic, SelectedLang, wcfInsertUpdateData.ToArray()));
				}
			}
			else if (UserOperateMode == OperateMode.Add)
			{
				var AddData = DgSource.Select(o => o.Item).ToList();
				var wcfDeletedData = new List<wcf.F000904>();
				AddData.ForEach(o => { wcfDeletedData.Add(o.Map<F000904, wcf.F000904>()); });

				reslut = proxy.RunWcfMethod(x => x.AddP710601Data(wcfDeletedData.ToArray()));
				if (reslut.IsSuccessed)
					ShowMessage(Messages.InfoAddSuccess);
			}
			UserOperateMode = OperateMode.Query;
			EnableLang = true;
			EnableMain = true;
			return reslut.IsSuccessed;
		}
		#endregion Save

		#region Insert
		public ICommand InsertCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }, () => true, o =>
					{
						if (!CheckInsertData())
							return;
						F000904 insertData = new F000904
						{
							TOPIC = AddTopic,
							SUBTOPIC = AddSubTop,
							SUB_NAME = AddSubTopName,
							NAME = AddName,
							VALUE = AddValue,
							ISUSAGE = IsUsage
						};
						if (DgSource == null)
							DgSource = new SelectionList<F000904>(new List<SelectionItem<F000904>> { new SelectionItem<F000904>(insertData) });
						else
							DgSource.Add(new SelectionItem<F000904>(insertData));
					}
					);
			}
		}

		private bool CheckInsertData()
		{
			if (string.IsNullOrEmpty(AddTopic))
			{
				ShowWarningMessage(Properties.Resources.P71060000_TopicNull);
				return false;
			}
			if (string.IsNullOrEmpty(AddSubTop))
			{
				ShowWarningMessage(Properties.Resources.P71060000_SubTopicNull);
				return false;
			}

			if (string.IsNullOrEmpty(AddSubTopName))
			{
				ShowWarningMessage(Properties.Resources.P71060000_SubNameNull);
				return false;
			}

			if (string.IsNullOrEmpty(AddValue))
			{
				ShowWarningMessage(Properties.Resources.P71060000_ValueNull);
				return false;
			}

			if (string.IsNullOrEmpty(AddName))
			{
				ShowWarningMessage(Properties.Resources.P71060000_NameNull);
				return false;
			}

			if (DgSource != null)
			{
				var repeatData = DgSource.Select(o => o.Item).ToList().FirstOrDefault(o => o.TOPIC == AddTopic && o.SUBTOPIC == AddSubTop && o.VALUE == AddValue);
				if (repeatData != null)
				{
					ShowWarningMessage(Properties.Resources.P71060000_DataRepeat);
					return false;
				}
			}

			var proxy = GetProxy<F00Entities>();
			var existData = proxy.F000904s.ToList().FirstOrDefault(o => o.TOPIC == AddTopic && o.SUBTOPIC == AddSubTop && o.VALUE == AddValue);
			if (existData != null)
			{
				ShowWarningMessage(Properties.Resources.P71060000_DataExist);
				return false;
			}

			return true;
		}
		#endregion

		#region RemoveCommand
		public ICommand RemoveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }, () => DgSource != null && DgSource.Where(x => x.IsSelected).Any(), o =>
					{
						var deletedData = DgSource.Where(x => x.IsSelected).ToList();
						deletedData.ForEach(x => DgSource.Remove(x));
					}
					);
			}
		}
		#endregion
	}
	public enum SelectTab : int
	{
		Tab1 = 0,
		Tab2 = 1
	}
}
