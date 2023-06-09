using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901910000_ViewModel : InputViewModelBase
	{

		public P1901910000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料

				InitControls();
			}

		}

		private void InitControls()
		{
			SetDcCode();
			AllWorkstatoinTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1946", "TYPE", true);
			SetWorkstationGroup();
			SetWorkstationStatus();
		}

		#region Property
		//物流中心清單
		private List<NameValuePair<string>> _dcList;
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set { Set(() => DcList, ref _dcList, value); }
		}

		//所選的物流中心
		private string _selectedDc;
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { Set(() => SelectedDc, ref _selectedDc, value); }
		}

		// 工作站群組清單
		private List<NameValuePair<string>> _workstationGroupList;
		public List<NameValuePair<string>> WorkstationGroupList
		{
			get { return _workstationGroupList; }
			set { Set(() => WorkstationGroupList, ref _workstationGroupList, value); }
		}
		// 所選的工作站群組
		private string _selectedWorkstationGroup;
		public string SelectedWorkstationGroup
		{
			get { return _selectedWorkstationGroup; }
			set
			{
				WorkstationTypeList = new List<NameValuePair<string>>();
				Set(() => SelectedWorkstationGroup, ref _selectedWorkstationGroup, value);
				
				if(SelectedWorkstationGroup == "B")
				{
					var typeList = new List<string> { "PA1", "PA2", "PACK" };
					WorkstationTypeList = AllWorkstatoinTypeList.Where(x => typeList.Contains(x.Value) || string.IsNullOrWhiteSpace(x.Value)).ToList();
				}
				else
				{
					WorkstationTypeList = AllWorkstatoinTypeList.Where(x => x.Value.StartsWith(SelectedWorkstationGroup) || string.IsNullOrWhiteSpace(x.Value)).ToList();
				}
				SelectedWorkstationType = WorkstationTypeList.FirstOrDefault()?.Value;
			}
		}


		// 工作站類型清單
		public List<NameValuePair<string>> _allWorkstationTypeList;
		public List<NameValuePair<string>> AllWorkstatoinTypeList
		{
			get { return _allWorkstationTypeList; }
			set { Set(() => AllWorkstatoinTypeList, ref _allWorkstationTypeList, value); }
		}

		// 切換顯示的工作站類型
		private List<NameValuePair<string>> _workstationTypeList;
		public List<NameValuePair<string>> WorkstationTypeList
		{
			get { return _workstationTypeList; }
			set { Set(() => WorkstationTypeList, ref _workstationTypeList, value); }
		}

		// 所選的工作站類型
		private string _selectedWorkstationType;
		public string SelectedWorkstationType
		{
			get { return _selectedWorkstationType; }
			set { Set(() => SelectedWorkstationType, ref _selectedWorkstationType, value); }
		}

		// 所有工作站狀態清單
		private List<NameValuePair<string>> workstationStatusList;
		public List<NameValuePair<string>> WorkstationStatusList
		{
			get { return workstationStatusList; }
			set { Set(() => WorkstationStatusList, ref workstationStatusList, value); }
		}

		//所選的工作站狀態
		private string _selectedWorkstationStatus;
		public string SelectedWorkstationStatus
		{
			get { return _selectedWorkstationStatus; }
			set { Set(() => SelectedWorkstationStatus, ref _selectedWorkstationStatus, value); }
		}

		// 新增狀態的工作站類型清單
		private List<NameValuePair<string>> _addWorkstationTypeList;
		public List<NameValuePair<string>> AddWorkstationTypeList
		{
			get { return _addWorkstationTypeList; }
			set { Set(() => AddWorkstationTypeList, ref _addWorkstationTypeList, value); }
		}

		// 
		private string _workstationCode;
		public string WorkstationCode
		{
			get { return _workstationCode; }
			set { Set(() => WorkstationCode, ref _workstationCode, value); }
		}
		// 
		private F1946 _addF1946Data;
		public F1946 AddF1946Data
		{
			get { return _addF1946Data; }
			set
			{
				Set(() => AddF1946Data, ref _addF1946Data, value);
			}
		}

		// 查詢的資料結果
		private List<F1946> _searchF1946Datas;
		public List<F1946> SearchF1946Datas
		{
			get { return _searchF1946Datas; }
			set { Set(() => SearchF1946Datas, ref _searchF1946Datas, value); }
		}

		// 新增的資料結果
		private List<F1946> _addF1946Datas = new List<F1946>();
		public List<F1946> AddF1946Datas
		{
			get { return _addF1946Datas; }
			set { Set(() => AddF1946Datas, ref _addF1946Datas, value); }
		}

		//查詢畫面選得的資料
		public F1946 _selectedData;
		public F1946 SelectedData
		{
			get { return _selectedData; }
			set { Set(() => SelectedData, ref _selectedData, value); }
		}

		// 新增狀態的工作站群組清單
		public string _addWorkstationGroup;
		public string AddWorkstationGroup
		{
			get { return _addWorkstationGroup; }
			set {
				Set(() => AddWorkstationGroup, ref _addWorkstationGroup, value);
				if (!string.IsNullOrWhiteSpace(AddWorkstationGroup))
				{
					AddWorkstationTypeList = AllWorkstatoinTypeList.Where(x => x.Value.StartsWith(AddWorkstationGroup)).ToList();
					AddF1946Data.WORKSTATION_TYPE = AddWorkstationTypeList.First().Value;
				}
			}
		}

		// 新增狀態的工作站類型
		public string _addWorkstationType;
		public string AddWorkstationType
		{
			get { return _addWorkstationType; }
			set { Set(() => AddWorkstationType, ref _addWorkstationType, value); }
		}
		#endregion


		#region Math
		// 設定物流中心
		public void SetDcCode()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
			{
				SelectedDc = DcList.First().Value;
			}
		}

		// 設定工作站群組
		public void SetWorkstationGroup()
		{
			WorkstationGroupList = GetBaseTableService.GetF000904List(FunctionCode, "F1946", "GROUP");
			if (WorkstationGroupList.Any())
			{
				SelectedWorkstationGroup = WorkstationGroupList.First().Value;
			}
		}

		// 設定工作站狀態
		public void SetWorkstationStatus()
		{
			WorkstationStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F1946", "STATUS", true);
			SelectedWorkstationStatus = WorkstationStatusList.First().Value;
		}

		// 檢查新增資料
		private bool CheckSaveData()
		{
			bool isCheck = true;
			var proxy = GetProxy<F19Entities>();
			if (string.IsNullOrWhiteSpace(AddF1946Data.DC_CODE) && isCheck)
			{
				DialogService.ShowMessage("請輸入物流中心");
				isCheck = false;
			}
			if (string.IsNullOrWhiteSpace(AddF1946Data.WORKSTATION_GROUP) && isCheck)
			{
				DialogService.ShowMessage("請輸入工作站群組");
				isCheck = false;
			}
			if (string.IsNullOrWhiteSpace(AddF1946Data.WORKSTATION_TYPE) && isCheck)
			{
				DialogService.ShowMessage("請輸入工作站類型");
				isCheck = false;
			}
			if (string.IsNullOrWhiteSpace(AddF1946Data.WORKSTATION_CODE) && isCheck)
			{
				DialogService.ShowMessage("請輸入工作站編號");
				isCheck = false;
			}
			if (AddF1946Data.WORKSTATION_CODE?.Trim().Length > 4 && isCheck)
			{
				DialogService.ShowMessage("工作站號碼應該只有4碼");
				isCheck = false;
			}
			if (AddF1946Data.WORKSTATION_GROUP?.Trim().Substring(0, 1) != AddF1946Data.WORKSTATION_CODE?.Trim().Substring(0, 1) && isCheck)
			{
				DialogService.ShowMessage($"工作站號碼的第一碼應該是{AddF1946Data.WORKSTATION_GROUP.Trim().Substring(0, 1)}");
				isCheck = false;
			}

			var isExistF1946 = proxy.F1946s.Where(x => x.DC_CODE == AddF1946Data.DC_CODE &&
			x.WORKSTATION_GROUP == AddF1946Data.WORKSTATION_GROUP &&
			x.WORKSTATION_TYPE == AddF1946Data.WORKSTATION_TYPE &&
			x.WORKSTATION_CODE == AddF1946Data.WORKSTATION_CODE).ToList().Any();

			//檢查要新增的資料是否存在F1946
			if (isExistF1946 && isCheck)
			{
				DialogService.ShowMessage("資料已存在");
				isCheck = false;
			}

			return isCheck;
		}
		#endregion

		#region Command
		#region SearchCommand
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearch(), () => UserOperateMode == OperateMode.Query
						);
			}
		}
		public void DoSearch()
		{
			var proxy = GetProxy<F19Entities>();
			SearchF1946Datas = proxy.CreateQuery<F1946>("GetWorkstationList")
				.AddQueryExOption("dcCode", SelectedDc)
				.AddQueryExOption("workstationGroup", SelectedWorkstationGroup)
				.AddQueryExOption("workstationType", SelectedWorkstationType)
				.AddQueryExOption("status", SelectedWorkstationStatus)
				.AddQueryExOption("workstationCode", WorkstationCode)
				.ToList();
		}
		#endregion

		#region AddCommand
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
			AddF1946Data = new F1946();
			AddF1946Datas = new List<F1946>();
			AddF1946Data.DC_CODE = DcList.FirstOrDefault()?.Value;
		}
		#endregion

		#region CancelCommand
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
			UserOperateMode = OperateMode.Query;
			DoSearch();
		}
		#endregion

		#region SaveCommand
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSave(), () => UserOperateMode == OperateMode.Add || UserOperateMode == OperateMode.Edit
						);
			}
		}
		private void DoSave()
		{
			UserOperateMode = OperateMode.Query;
			
		}
		#endregion

		#region AddWorkstationCommand
		public ICommand AddWorkstationCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAddWorkstation(), () => UserOperateMode == OperateMode.Add
						);
			}
		}

		// 新增工作站
		private void DoAddWorkstation()
		{
			AddF1946Data.WORKSTATION_CODE = AddF1946Data.WORKSTATION_CODE?.ToUpper();
			if (CheckSaveData())
			{
				var proxy = new wcf.P19WcfServiceClient();
				var f1946Data = ExDataMapper.Map<F1946, wcf.F1946>(AddF1946Data);
				var result = RunWcfMethod(proxy.InnerChannel, () => proxy.InsertF1946(f1946Data));
				if (!result.IsSuccessed)
				{
					ShowWarningMessage(result.Message);
				}
				else
				{
					AddF1946Datas.Add(AddF1946Data);
					AddF1946Datas = Mapper.Map<List<F1946>, List<F1946>>(AddF1946Datas);
					AddF1946Data = new F1946();
					AddF1946Data.DC_CODE= DcList.FirstOrDefault()?.Value;
					AddWorkstationTypeList = null;
					DialogService.ShowMessage("已更新");
				}
				
			}
		}
		#endregion
		#region DeleteCommand
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData.STATUS == "0"
						);
			}
		}

		private void DoDelete()
		{
			var proxy = new wcf.P19WcfServiceClient();
			var f1946Data = ExDataMapper.Map<F1946, wcf.F1946>(SelectedData);
			var result = RunWcfMethod(proxy.InnerChannel, () => proxy.DeleteF1946(f1946Data));
			if (!result.IsSuccessed)
			{
				DialogService.ShowMessage(result.Message);
			}
			else
			{
				DialogService.ShowMessage("已刪除");
			}
			DoSearch();
		}
		#endregion
		#endregion
	}
}