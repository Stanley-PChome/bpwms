using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P21ExDataService;
using Wms3pl.WpfClient.Common;
using System.Windows.Media;
using System.Windows;
using Wms3pl.Datas.F19;
using Wms3pl.WpfClient.ExDataServices.P70ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.DataServices.F06DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P21WcfService;

namespace Wms3pl.WpfClient.P21.ViewModel
{
	public partial class P2116020000_ViewModel : InputViewModelBase
	{
		

		public P2116020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetScheduleNameList();
				SetStatus();
				PickNoVisibility = Visibility.Collapsed;
				IsSecondVisibility = Visibility.Collapsed;
				CheckCodeVisibility = Visibility.Collapsed;
			}
		}

		#region Property

		// 業主編號
		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}
		
		// 貨主編號
		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		// 物流中心清單
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

		// 選擇的物流中心
		private string _selectedDcCode;
		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set { Set(() => SelectedDcCode, ref _selectedDcCode, value); }
		}

		// 建立日期(起)
		private DateTime? _beginCreateDate;
		public DateTime? BeginCreateDate
		{
			get { return _beginCreateDate; }
			set { Set(() => BeginCreateDate, ref _beginCreateDate, value); }
		}

		// 建立日期(迄)
		private DateTime? _endCreateDate;
		public DateTime? EndCreateDate
		{
			get { return _endCreateDate; }
			set { Set(() => EndCreateDate, ref _endCreateDate, value); }
		}

		// 排程名稱清單
		private List<NameValuePair<string>> _scheduleNameList;
		public List<NameValuePair<string>> ScheduleNameList
		{
			get { return _scheduleNameList; }
			set { Set(() => ScheduleNameList, ref _scheduleNameList, value); }
		}

		// 選擇的排程名稱
		private string _selectedScheduleName;
		public string SelectedScheduleName
		{
			get { return _selectedScheduleName; }
			set {
				Set(() => SelectedScheduleName, ref _selectedScheduleName, value);
			}
		}

		// 狀態清單
		private List<NameValuePair<string>> _statusList;
		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set { Set(() => StatusList, ref _statusList, value); }
		}

		// 選擇的狀態
		private string _selectedStatus;
		public string SelectedStatus
		{
			get { return _selectedStatus; }
			set { Set(() => SelectedStatus, ref _selectedStatus, value); }
		}

		// 單據編號
		private string _docNum;
		public string DocNum
		{
			get { return _docNum; }
			set { Set(() => DocNum, ref _docNum, value); }
		}

		// 任務單號
		private string _taskNum;
		public string TaskNum
		{
			get { return _taskNum; }
			set { Set(() => TaskNum, ref _taskNum, value); }
		}

		private bool _isSelectedAll = false;
		public bool IsSelectedAll
		{
			get { return _isSelectedAll; }
			set { Set(() => IsSelectedAll, ref _isSelectedAll, value); }
		}
		#endregion

		#region Visibility
		private Visibility _pickNoVisibility;
		public Visibility PickNoVisibility
		{
			get { return _pickNoVisibility; }
			set { Set(() => PickNoVisibility, ref _pickNoVisibility, value); }
		}

		private Visibility _isSecondVisibility;
		public Visibility IsSecondVisibility
		{
			get { return _isSecondVisibility; }
			set { Set(() => IsSecondVisibility, ref _isSecondVisibility, value); }
		}

		private Visibility _checkCodeVisibility;
		public Visibility CheckCodeVisibility
		{
			get { return _checkCodeVisibility; }
			set { Set(() => CheckCodeVisibility, ref _checkCodeVisibility, value); }
		}
		#endregion

		#region ComboBoxBinding
		//取得物流中心清單
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}

		// 取得排程名稱清單
		private void SetScheduleNameList()
		{
			ScheduleNameList = GetBaseTableService.GetF000904List(FunctionCode, "P2116020000", "NAME", false);
			if (ScheduleNameList.Any())
				SelectedScheduleName = ScheduleNameList.First().Value;
		}

		// 取得狀態清單
		private void SetStatus()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "P2116020000", "STATUS", true);
			if (StatusList.Any())
				SelectedStatus = StatusList.First().Value;
		}

		// 入庫指示派發清單
		private List<SelectionItem<TaskDispatchData>> _taskDispatchDataList;
		public List<SelectionItem<TaskDispatchData>> TaskDispatchDataList
		{
			get { return _taskDispatchDataList; }
			set { Set(() => TaskDispatchDataList, ref _taskDispatchDataList, value); }
		}

		// 選擇的入庫指示派發
		private TaskDispatchData _selectedF060101Data;
		public TaskDispatchData SelectedF060101Data
		{
			get { return _selectedF060101Data; }
			set { Set(() => SelectedF060101Data, ref _selectedF060101Data, value); }
		}

		// 出庫指示派發清單
		//private ObservableCollection<F060201Data> _f060201DataList;
		//public ObservableCollection<F060201Data> F060201DataList
		//{
		//	get { return _f060201DataList; }
		//	set { Set(() => F060201DataList, ref _f060201DataList, value); }
		//}

		// 選擇的出庫指示派發
		private TaskDispatchData _selectedF060201Data;
		public TaskDispatchData SelectedF060201Data
		{
			get { return _selectedF060201Data; }
			set { Set(() => SelectedF060201Data, ref _selectedF060201Data, value); }
		}

		// 盤點指示派發清單
		//private ObservableCollection<F060401Data> _f060401DataList;
		//public ObservableCollection<F060401Data> F060401DataList
		//{
		//	get { return _f060401DataList; }
		//	set { Set(() => F060401DataList, ref _f060401DataList, value); }
		//}

		// 選擇的盤點指示派發
		private TaskDispatchData _selectedF060401Data;
		public TaskDispatchData SelectedF060401Data
		{
			get { return _selectedF060401Data; }
			set { Set(() => SelectedF060401Data, ref _selectedF060401Data, value); }
		}

		// 盤點調整指示派發清單
		//private ObservableCollection<F060404Data> _f060404DataList;
		//public ObservableCollection<F060404Data> F060404DataList
		//{
		//	get { return _f060404DataList; }
		//	set { Set(() => F060404DataList, ref _f060404DataList, value); }
		//}

		// 選擇的盤點調整指示派發
		private TaskDispatchData _selectedF060404Data;
		public TaskDispatchData SelectedF060404Data
		{
			get { return _selectedF060404Data; }
			set { Set(() => SelectedF060404Data, ref _selectedF060404Data, value); }
		}

		#endregion

		#region Math
		// 入庫指示派發
		public void GetF060101DataList()
		{
			var proxyP21 = new wcf.P21WcfServiceClient();
			var tmpTaskDispatchDataList = RunWcfMethod<wcf.TaskDispatchData[]>(proxyP21.InnerChannel,
							() => proxyP21.GetF060101Datas(SelectedDcCode, GupCode, CustCode, BeginCreateDate, EndCreateDate, SelectedStatus, DocNum?.Trim()?.Split(','), TaskNum?.Trim()?.Split(','))).AsEnumerable();
			TaskDispatchDataList = tmpTaskDispatchDataList.Select(x => AutoMapper.Mapper.DynamicMap<TaskDispatchData>(x)).ToSelectionList().ToList();
		}

		// 出庫指示派發
		public void GetF060201DataList()
		{
			var proxyP21 = new wcf.P21WcfServiceClient();
			var tmpTaskDispatchDataList = RunWcfMethod<wcf.TaskDispatchData[]>(proxyP21.InnerChannel,
							() => proxyP21.GetF060201Datas(SelectedDcCode, GupCode, CustCode, BeginCreateDate, EndCreateDate, SelectedStatus, DocNum?.Trim()?.Split(','), TaskNum?.Trim()?.Split(','))).AsEnumerable();
			TaskDispatchDataList = tmpTaskDispatchDataList.Select(x => AutoMapper.Mapper.DynamicMap<TaskDispatchData>(x)).ToSelectionList().ToList();
		}

		// 盤點指示派發
		public void GetF060401DataList()
		{
			var proxyP21 = new wcf.P21WcfServiceClient();
			var tmpTaskDispatchDataList = RunWcfMethod<wcf.TaskDispatchData[]>(proxyP21.InnerChannel,
							() => proxyP21.GetF060401Datas(SelectedDcCode, GupCode, CustCode, BeginCreateDate, EndCreateDate, SelectedStatus, DocNum?.Trim()?.Split(','), TaskNum?.Trim()?.Split(','))).AsEnumerable();
			TaskDispatchDataList = tmpTaskDispatchDataList.Select(x => AutoMapper.Mapper.DynamicMap<TaskDispatchData>(x)).ToSelectionList().ToList();
		}

		// 盤點調整指示派發
		public void GetF060404DataList()
		{
			var proxyP21 = new wcf.P21WcfServiceClient();
			var tmpTaskDispatchDataList = RunWcfMethod<wcf.TaskDispatchData[]>(proxyP21.InnerChannel,
							() => proxyP21.GetF060404Datas(SelectedDcCode, GupCode, CustCode, BeginCreateDate, EndCreateDate, SelectedStatus, DocNum?.Trim()?.Split(','), TaskNum?.Trim()?.Split(','))).AsEnumerable();
			TaskDispatchDataList = tmpTaskDispatchDataList.Select(x => AutoMapper.Mapper.DynamicMap<TaskDispatchData>(x)).ToSelectionList().ToList();
		}

		public void DoSearch(bool showOneHundredCountMessage = true)
		{
			DocNum = string.IsNullOrWhiteSpace(DocNum) ? null : DocNum;
			TaskNum = string.IsNullOrWhiteSpace(TaskNum) ? null : TaskNum;
			if (string.IsNullOrWhiteSpace(DocNum) && string.IsNullOrWhiteSpace(TaskNum) && showOneHundredCountMessage)
			{
				if (ShowConfirmMessage("單據號碼或任務單號，若沒輸入條件，將只顯示前100筆資料") != DialogResponse.Yes) return;
			}

			if (DocNum?.Trim()?.Split(',')?.Count() > 10 || TaskNum?.Split(',')?.Count() > 10)
			{
				ShowInfoMessage("單據編號及任務單號不可以超過10組");
				return;
			}

			switch (SelectedScheduleName)
			{
				case "F060101":
					GetF060101DataList();
					PickNoVisibility = Visibility.Collapsed;
					IsSecondVisibility = Visibility.Collapsed;
					CheckCodeVisibility = Visibility.Collapsed;
					break;
				case "F060201":
					GetF060201DataList();
					PickNoVisibility = Visibility.Visible;
					IsSecondVisibility = Visibility.Collapsed;
					CheckCodeVisibility = Visibility.Collapsed;
					break;
				case "F060401":
					GetF060401DataList();
					PickNoVisibility = Visibility.Collapsed;
					IsSecondVisibility = Visibility.Visible;
					CheckCodeVisibility = Visibility.Collapsed;
					break;
				case "F060404":
					GetF060404DataList();
					PickNoVisibility = Visibility.Collapsed;
					IsSecondVisibility = Visibility.Collapsed;
					CheckCodeVisibility = Visibility.Visible;
					break;
				default:
					break;
			}
			
		}

		public void DoOriginalOrderRedistribution()
		{
			if (CheckTaskCount())
			{
				var proxyP21 = new wcf.P21WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyP21.InnerChannel,
								() => proxyP21.OriginalOrderRedistribution(SelectedScheduleName, GetDocIdGroup().ToArray()));
			}
			
		}

		public void DoAssignNewTasks()
		{
			if (CheckTaskCount())
			{
				var proxyP21 = new wcf.P21WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyP21.InnerChannel,
								() => proxyP21.DoAssignNewTasks(SelectedScheduleName, GetDocIdGroup().ToArray()));
			}
		
		}
		
		//全選
		public void DoCheckAllItem()
		{
			if (TaskDispatchDataList == null) return;
			foreach (var p in TaskDispatchDataList)
				p.IsSelected = p.Item.ENABLE?IsSelectedAll: false;
		}

		public bool CheckTaskCount()
		{
			bool checkResult = true;
			var selectedItem = TaskDispatchDataList.Where(x => x.IsSelected);
			if (selectedItem.Count() > 1)
			{
				ShowWarningMessage("只能選擇一個任務單號");
				checkResult = false;
			}
			return checkResult;
		}
		#endregion

		#region ICommand
		
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch()
					);
			}
		}

		//全選
		public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckAllItem()
				);
			}
		}

		// 原單重新派發
		public ICommand OriginalOrderRedistributionCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoOriginalOrderRedistribution(),
					() => TaskDispatchDataList != null && TaskDispatchDataList.Any(x => x.IsSelected),
					o => DoSearch(false)
					);
			}
		}

		// 派發新任務
		public ICommand AssignNewTasksCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAssignNewTasks(),
					() => TaskDispatchDataList != null && TaskDispatchDataList.Any(x=>x.IsSelected),
					o => { DoSearch(false); }
					);
			}
		}

		// 取得欲派發的任務單號
		public List<string> GetDocIdGroup()
		{
			if (TaskDispatchDataList != null)
			{
				var result = (from i in TaskDispatchDataList
							  where i.IsSelected
							  select i.Item.DOC_ID).ToList();
				return result;
			}
			return new List<string>();
		}
		#endregion
	}
}
