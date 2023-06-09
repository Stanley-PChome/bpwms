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
using AutoMapper;
namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1905070000_ViewModel : InputViewModelBase
	{
		public Action<OperateMode> UserOperateModeFocus = delegate { };


		public P1905070000_ViewModel()
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
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList.ToList();
			SelectedDcItem = DcList.FirstOrDefault();
			DoSearch(true);
			//SelectedWorkgroupItem = WorkgroupList.FirstOrDefault();
			IsBusy = false;
		}

		#region 資料連結/ 頁面參數
		#region Form - 儲位查詢
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// DC選單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
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

				InitNonAllowed();

				if (value != null)
					DoSearchWarehouse();
			}
		}

		private List<NameValuePair<string>> _warehouseList;
		/// <summary>
		/// 倉別選單
		/// </summary>
		public List<NameValuePair<string>> WarehouseList
		{
			get { return _warehouseList; }
			set
			{
				_warehouseList = value;
				RaisePropertyChanged("WarehouseList");
			}
		}
		private NameValuePair<string> _selectedWarehouseItem;
		public NameValuePair<string> SelectedWarehouseItem
		{
			get { return _selectedWarehouseItem; }
			set
			{
				_selectedWarehouseItem = value;
				RaisePropertyChanged("SelectedWarehouseItem");

				InitNonAllowed();

				if (value != null)
					DoSearchFloor();
			}
		}
		private List<NameValuePair<string>> _floorList;
		/// <summary>
		/// 樓層選單
		/// </summary>
		public List<NameValuePair<string>> FloorList
		{
			get { return _floorList; }
			set
			{
				_floorList = value;
				RaisePropertyChanged("FloorList");
			}
		}
		private NameValuePair<string> _selectedFloorItem;
		public NameValuePair<string> SelectedFloorItem
		{
			get { return _selectedFloorItem; }
			set
			{
				_selectedFloorItem = value;
				RaisePropertyChanged("SelectedFloorItem");

				InitNonAllowed();
			}
		}
		private string _startLocCode = string.Empty;
		public string StartLocCode { get { return _startLocCode; } set { _startLocCode = value; } }
		private string _endLocCode = string.Empty;
		public string EndLocCode { get { return _endLocCode; } set { _endLocCode = value; } }
		#endregion
		#region Form - 作業群組名稱
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

				EditableWorkgroupItem = (value == null) ? null : Mapper.DynamicMap<F1963>(value);
			}
		}


		private F1963 _editableWorkgroupItem;
		public F1963 EditableWorkgroupItem
		{
			get { return _editableWorkgroupItem; }
			set
			{
				_editableWorkgroupItem = value;
				RaisePropertyChanged("EditableWorkgroupItem");

				// 搜尋
				if (value != null)
					ShowLocTreeview();
			}
		}

		private List<F1963> _workgroupList;
		public List<F1963> WorkgroupList
		{
			get { return _workgroupList; }
			set { _workgroupList = value; RaisePropertyChanged("WorkgroupList"); }
		}

		#endregion

		private List<F1912LocData> _allowedF1912LocDataList;

		public List<F1912LocData> AllowedF1912LocDataList
		{
			get { return _allowedF1912LocDataList; }
			set
			{
				Set(() => AllowedF1912LocDataList, ref _allowedF1912LocDataList, value);
			}
		}

		private List<F1912LocData> _nonAllowedF1912LocDataList;

		public List<F1912LocData> NonAllowedF1912LocDataList
		{
			get { return _nonAllowedF1912LocDataList; }
			set
			{
				Set(() => NonAllowedF1912LocDataList, ref _nonAllowedF1912LocDataList, value);
			}
		}


		private List<P190507LocNode> _allowedP190507LocNodes;
		/// <summary>
		/// 已設定儲位
		/// </summary>
		public List<P190507LocNode> AllowedP190507LocNodes
		{
			get { return _allowedP190507LocNodes; }
			set
			{
				Set(() => AllowedP190507LocNodes, ref _allowedP190507LocNodes, value);
			}
		}

		private List<P190507LocNode> _nonAllowedP190507LocNodes;
		/// <summary>
		/// 未設定儲位
		/// </summary>
		public List<P190507LocNode> NonAllowedP190507LocNodes
		{
			get { return _nonAllowedP190507LocNodes; }
			set
			{
				Set(() => NonAllowedP190507LocNodes, ref _nonAllowedP190507LocNodes, value);
			}
		}





		/// <summary>
		/// 取得倉別
		/// </summary>
		private void DoSearchWarehouse()
		{
			//var proxy = GetProxy<F19Entities>();
			//var tmp = proxy.F1980s.Where(x => x.DC_CODE.Equals(SelectedDcItem.Value) || string.IsNullOrEmpty(SelectedDcItem.Value))
			//				.OrderBy(x => x.WAREHOUSE_ID).AsQueryable().ToList()
			//				.Select(p => new NameValuePair<string>() { Name = p.WAREHOUSE_NAME, Value = p.WAREHOUSE_ID })
			//				.ToList();

			var proxyP19Ex = GetExProxy<P19ExDataSource>();
			var tmp = proxyP19Ex.CreateQuery<F1912WareHouseData>("GetCustWarehouseDatas")
										.AddQueryExOption("dcCode", SelectedDcItem.Value)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
										.Select(o => new NameValuePair<string>()
										{
											Name = o.WAREHOUSE_NAME,
											Value = o.WAREHOUSE_ID
										}).ToList();



			tmp.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });
			WarehouseList = tmp;

			SelectedWarehouseItem = WarehouseList.FirstOrDefault();
		}

		/// <summary>
		/// 取得樓層
		/// </summary>
		private void DoSearchFloor()
		{
			var proxyP19Ex = GetExProxy<P19ExDataSource>();
			var query = proxyP19Ex.CreateQuery<string>("GetFloors")
										.AddQueryExOption("dcCode", SelectedDcItem.Value)
										.AddQueryExOption("warehouseId", SelectedWarehouseItem.Value);
			var list = query.ToList()
							.FirstOrDefault()
							.Split(',')
							.Select(floor => new NameValuePair<string>(floor, floor))
							.ToList();
			list.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = string.Empty });

			FloorList = list;
			SelectedFloorItem = FloorList.FirstOrDefault();
		}
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
						DoSearch(false);
					},
					() => UserOperateMode == OperateMode.Query
				);
			}
		}

		/// <summary>
		/// 取得作業群組
		/// </summary>
		private void DoSearch(bool firstEnter)
		{
			SearchWorkgroupName = SearchWorkgroupName.Trim();

			if (!firstEnter)
			{
				//執行查詢動作
				var proxy = GetProxy<F19Entities>();
				var query = proxy.F1963s.Where(x => x.ISDELETED == "0");
				if (!string.IsNullOrEmpty(SearchWorkgroupName))
				{
					query = query.Where(x => x.WORK_NAME.Contains(SearchWorkgroupName));
				}
				WorkgroupList = query.OrderBy(x => x.WORK_ID).ToList();

				if (!WorkgroupList.Any())
					ShowMessage(Messages.InfoNoData);
			}
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
					},
					() => UserOperateMode == OperateMode.Query

				);
			}
		}

		/// <summary>
		/// 新增
		/// </summary>
		/// <returns></returns>
		private void DoAdd()
		{
			EditableWorkgroupItem = new F1963();
			UserOperateMode = OperateMode.Add;
			UserOperateModeFocus(OperateMode.Add);
		}
		#endregion

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && EditableWorkgroupItem != null
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
			UserOperateModeFocus(OperateMode.Edit);
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
						if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
						DoDelete();
					},
					() => UserOperateMode == OperateMode.Query && EditableWorkgroupItem != null
				);
			}
		}

		/// <summary>
		/// 刪除
		/// </summary>
		/// <returns></returns>
		private void DoDelete()
		{
			var proxyEx = GetExProxy<P19ExDataSource>();
			var proxy = GetProxy<F19Entities>();
			// 檢查是否已加入人員
			if (proxy.F192403s.Where(x => x.WORK_ID.Equals(EditableWorkgroupItem.WORK_ID)).Count() != 0)
			{
				ShowMessage(Messages.WarningCannotDeleteWordgroup);
				return;
			}

			// 刪除時F1963和F196301皆不做實體刪除, 所以直接在這更新即可
			var tmp = proxy.F1963s.Where(x => x.WORK_ID == EditableWorkgroupItem.WORK_ID).FirstOrDefault();
			if (tmp == null)
			{
				ShowMessage(new MessagesStruct()
				{
					Title = Resources.Resources.Information,
					Button = DialogButton.OK,
					Image = DialogImage.Warning,
					Message = Properties.Resources.P1905070000_WorkGroupNotExist
				});
			}
			else
			{
				tmp.ISDELETED = "1";
				proxy.UpdateObject(tmp);
				proxy.SaveChanges();
				ShowMessage(Messages.Success);
				DoSearch(false);
				ShowLocTreeview();
			}
		}
		#endregion Save

		#region Save
		private ExecuteResult _dataSaved = null;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						_dataSaved = DoSave();
					},
					() => UserOperateMode != OperateMode.Query && EditableWorkgroupItem != null,
					o =>
					{
						if (_dataSaved != null && _dataSaved.IsSuccessed == true)
						{
							// 資料儲存後, 要重新查詢一次, 以在DataGrid帶出資料
							var selectedWorkId = (SelectedWorkgroupItem == null) ? 0 : SelectedWorkgroupItem.WORK_ID;
							DoSearch(false);
							ShowLocTreeview();
							if (UserOperateMode == OperateMode.Add)
								SelectedWorkgroupItem = WorkgroupList.LastOrDefault();
							else
								SelectedWorkgroupItem = (selectedWorkId == 0) ? WorkgroupList.FirstOrDefault() : WorkgroupList.Where(item => item.WORK_ID == selectedWorkId).FirstOrDefault();
							UserOperateMode = OperateMode.Query;
						}
						_dataSaved = null;
					}
				);
			}
		}

		/// <summary>
		/// 儲存
		/// </summary>
		/// <returns></returns>
		private ExecuteResult DoSave()
		{
			ExDataMapper.Trim(EditableWorkgroupItem);
			var wcfF1963 = ExDataMapper.Map<F1963, wcf.F1963>(EditableWorkgroupItem);
			// 確保在移動項目中，可能會發生重複移入儲位(雖然沒遇過，先做好防呆@@")
			var distinctLocDatas = AllowedF1912LocDataList.GroupBy(x => new { x.DC_CODE, x.AREA_CODE, x.LOC_CODE })
															.Select(g => g.First());
			var wcfF196301s = ExDataMapper.MapCollection<F1912LocData, wcf.F196301>(distinctLocDatas).ToArray();

			// 檢查名稱有無重複
			var proxyWcf = new wcf.P19WcfServiceClient();
			var tmpCheckDuplicate = RunWcfMethod<bool>(proxyWcf.InnerChannel, () => proxyWcf.F1963CheckDuplicateByIdName(EditableWorkgroupItem.WORK_ID, EditableWorkgroupItem.WORK_NAME));

			if (tmpCheckDuplicate == true)
			{
				var msg = Messages.WarningDuplicatedData;
				msg.Message = "作業群組名稱" + msg.Message;
				ShowMessage(msg);
				return new ExecuteResult() { IsSuccessed = false };
			}

			if (!wcfF196301s.Any())
			{
				DialogService.ShowMessage("已設定儲位沒有任何項目");
				return new ExecuteResult() { IsSuccessed = false };
			}

			wcf.ExecuteResult tmp;
			if (UserOperateMode == OperateMode.Add)
			{
				// 新增時的儲存
				tmp = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.InsertP190507(wcfF1963, wcfF196301s));
			}
			else
			{
				// 更新時的儲存
				tmp = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.UpdateP190507(wcfF1963, wcfF196301s));
			}
			var result = new ExecuteResult() { IsSuccessed = tmp.IsSuccessed, Message = tmp.Message };
			ShowMessage(new List<ExecuteResult>() { result });

			return result;
		}
		#endregion Save

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => UserOperateMode != OperateMode.Query && EditableWorkgroupItem != null,
					o => DoCancel() // 動作放到Completes後才做, 避免資料異動的檢查邏輯錯誤
				);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				var selectedWorkId = (SelectedWorkgroupItem == null) ? 0 : SelectedWorkgroupItem.WORK_ID;
				DoSearch(false);
				SelectedWorkgroupItem = (selectedWorkId == 0) ? WorkgroupList.FirstOrDefault() : WorkgroupList.Where(item => item.WORK_ID == selectedWorkId).FirstOrDefault();
				UserOperateMode = OperateMode.Query;
			}
		}
		#endregion Cancel

		#region 搜尋儲位
		public ICommand SearchLocCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchLoc(),
          () => SelectedDcItem != null && SelectedWarehouseItem != null && EditableWorkgroupItem != null
          && SelectedFloorItem != null && StartLocCode != null && EndLocCode != null
				);
			}
		}
		/// <summary>
		/// 取得儲位資料, 當按下儲位搜尋時叫用, 存放在LocUnAssignedList, 並且顯示出來
		/// </summary>
		private void DoSearchLoc()
		{
			CheckLocCodeInput();

			NonAllowedF1912LocDataList = GetNonAllowedF1912LocDatas();
			NonAllowedP190507LocNodes = GenP190507LocNodeList(NonAllowedF1912LocDataList);
		}

		/// <summary>
		/// 給使用者防呆用的儲位編號修正
		/// </summary>
		private void CheckLocCodeInput()
		{
			StartLocCode = StartLocCode.Trim();
			EndLocCode = EndLocCode.Trim();

			if (!string.IsNullOrEmpty(StartLocCode) && !string.IsNullOrEmpty(EndLocCode))
			{
				// 範圍填由大到小的話，自動交換
				if (StartLocCode.CompareTo(EndLocCode) > 0)
				{
					var end = StartLocCode;
					StartLocCode = EndLocCode;
					EndLocCode = end;
				}
			}
		}

		private List<F1912LocData> GetNonAllowedF1912LocDatas()
		{
			//var excludeLocCodes = GetLocCodesByDcCode(SelectedDcItem.Value).ToArray();

			//var proxyWcf = new wcf.P19WcfServiceClient();
			//var wcfF1912LocDatas = RunWcfMethod<wcf.F1912LocData[]>(proxyWcf.InnerChannel,
			//						() => proxyWcf.GetNonAllowedF1912LocDatas(SelectedDcItem.Value,
			//																  SelectedWarehouseItem.Value,
			//																  SelectedFloorItem.Value,
			//																  StartLocCode.Replace("-", string.Empty),
			//																  EndLocCode.Replace("-", string.Empty),
			//																  excludeLocCodes));

			var proxyEx = GetExProxy<P19ExDataSource>();
			var f1912LocDatas = proxyEx.CreateQuery<F1912LocData>("GetNonAllowedF1912LocDatas")
																	.AddQueryExOption("dcCode", SelectedDcItem.Value)
																	.AddQueryExOption("warehouseId", SelectedWarehouseItem.Value)
																	.AddQueryExOption("floor", SelectedFloorItem.Value)
																	.AddQueryExOption("beginLocCode", StartLocCode.Replace("-", string.Empty))
																	.AddQueryExOption("endLocCode", EndLocCode.Replace("-", string.Empty))
																	.AddQueryExOption("workId", EditableWorkgroupItem.WORK_ID.ToString())
																	.ToList();

			return f1912LocDatas;//ExDataMapper.MapCollection<wcf.F1912LocData, F1912LocData>(wcfF1912LocDatas).ToList();
		}

		IEnumerable<string> GetLocCodesByDcCode(string dcCode)
		{
			return AllowedF1912LocDataList.Where(x => x.DC_CODE == dcCode)
										.Select(x => x.LOC_CODE);
		}

		/// <summary>
		/// 產生DC、區、樓、通道、座、層樹狀節點
		/// </summary>
		/// <param name="f1912LocDataList"></param>
		/// <returns></returns>
		public List<P190507LocNode> GenP190507LocNodeList(List<F1912LocData> f1912LocDataList)
		{
			var p190507LocDataList = ExDataMapper.MapCollection<F1912LocData, P190507LocData>(f1912LocDataList).ToList();

			var locNodes = new List<P190507LocNode>();

			foreach (var g in p190507LocDataList.GroupBy(x => x.DC_CODE).OrderBy(x => x.Key))
			{
				var locNode = new P190507LocNode(DcList.Where(x => x.Value == g.Key).Select(x => x.Name).FirstOrDefault(), null);
				locNode.IsExpanded = true;
				locNodes.Add(locNode);
				foreach (var gArea in g.GroupBy(x => x.AREA_CODE).OrderBy(x => x.Key))
				{
					var area = new P190507LocNode(gArea.Key + "區", locNode);
					locNode.LocNodes.Add(area);

					foreach (var g1 in gArea.GroupBy(x => x.Floor).OrderBy(x => x.Key))
					{
						var floor = new P190507LocNode(g1.Key.ToString() + "樓", area);
						area.LocNodes.Add(floor);

						foreach (var g2 in g1.GroupBy(x => x.Channel).OrderBy(x => x.Key))
						{
							var channel = new P190507LocNode(g2.Key + "通道", floor);
							floor.LocNodes.Add(channel);

							foreach (var x in g2.OrderBy(x => x.LOC_CODE))
							{
								channel.LocNodes.Add(new P190507LocNode(FormatLocCode(x.LOC_CODE), channel) { LocData = x });
							}
							//foreach (var g3 in g2.GroupBy(x => x.Plain).OrderBy(x => x.Key))
							//{
							//	var plain = new P190507LocNode(g3.Key + "座", channel);
							//	channel.LocNodes.Add(plain);

							//	foreach (var g4 in g3.GroupBy(x => x.LocLevel).OrderBy(x => x.Key))
							//	{
							//		var locLevel = new P190507LocNode(g4.Key + "層", plain);
							//		plain.LocNodes.Add(locLevel);

							//		foreach (var x in g4.OrderBy(x => x.LOC_CODE))
							//		{
							//			locLevel.LocNodes.Add(new P190507LocNode(FormatLocCode(x.LOC_CODE), locLevel) { LocData = x });
							//		}
							//	}
							//}
						}
					}
				}
			}

			return locNodes;
		}

		private string FormatLocCode(string locCode)
		{
			if (locCode.Length != 9)
				return locCode;

			return string.Format("{0}-{1}-{2}-{3}-{4}", locCode.Substring(0, 1), locCode.Substring(1, 2),
				locCode.Substring(3, 2), locCode.Substring(5, 2), locCode.Substring(7, 2));
		}

		/// <summary>
		/// 顯示儲位樹
		/// </summary>
		private void ShowLocTreeview()
		{
			if (EditableWorkgroupItem == null)
				return;

			AllowedF1912LocDataList = GetAllowedF1912LocDatas();
			AllowedP190507LocNodes = GenP190507LocNodeList(AllowedF1912LocDataList);

			InitNonAllowed();

		}

		private void InitNonAllowed()
		{
			NonAllowedF1912LocDataList = new List<F1912LocData>();
			NonAllowedP190507LocNodes = new List<P190507LocNode>();
		}

		void GetNonAllowedLocFromRemoveAllowedLoc()
		{
			// 去掉已授權的儲位
			NonAllowedF1912LocDataList = (from a in NonAllowedF1912LocDataList
																		join b in AllowedF1912LocDataList on a.LOC_CODE equals b.LOC_CODE into d
																		from c in d.DefaultIfEmpty()
																		where c == null
																		select a).ToList();
		}

		/// <summary>
		/// 取得該作業群組的儲位
		/// </summary>
		private List<F1912LocData> GetAllowedF1912LocDatas()
		{
			var proxyEx = GetExProxy<P19ExDataSource>();
			var f1912LocDatas = proxyEx.CreateQuery<F1912LocData>("GetAllowedF1912LocDatas")
										.AddQueryExOption("workId", EditableWorkgroupItem.WORK_ID.ToString())
										.ToList();
			return f1912LocDatas;
		}

		#endregion

		#region 移動 (全部選取/ 部份選取)
		private RelayCommand<P190505MovingType> _moveItemsCommand;
		public ICommand MoveItemsCommand
		{
			get
			{
				return _moveItemsCommand ??
						(
							_moveItemsCommand = new RelayCommand<P190505MovingType>(
										(type) => DoMoveItems(type),
										(type) => CanExecuteMoveItems(type))
						);
			}
		}

		/// <summary>
		/// 移動項目
		/// </summary>
		/// <param name="type"></param>
		public void DoMoveItems(P190505MovingType type)
		{
			switch (type)
			{
				case P190505MovingType.Assign:
					var nonAllowedQuery = GetCheckedLocNodes(NonAllowedP190507LocNodes, node => node.IsChecked && node.LocData != null);
					if (!nonAllowedQuery.Any())
						return;

					AllowedF1912LocDataList.AddRange(ExDataMapper.MapCollection<P190507LocData, F1912LocData>(nonAllowedQuery.Select(node => node.LocData)));
					NonAllowedF1912LocDataList = ExDataMapper.MapCollection<P190507LocData, F1912LocData>(GetCheckedLocNodes(NonAllowedP190507LocNodes, node => !node.IsChecked && node.LocData != null).Select(node => node.LocData))
															 .ToList();
					break;
				case P190505MovingType.AssignAll:
					AllowedF1912LocDataList.AddRange(NonAllowedF1912LocDataList);
					NonAllowedF1912LocDataList.Clear();
					break;
				case P190505MovingType.UnAssign:
					var allowedQuery = GetCheckedLocNodes(AllowedP190507LocNodes, node => node.IsChecked && node.LocData != null);
					if (!allowedQuery.Any())
						return;

					NonAllowedF1912LocDataList.AddRange(ExDataMapper.MapCollection<P190507LocData, F1912LocData>(allowedQuery.Select(node => node.LocData)));
					AllowedF1912LocDataList = ExDataMapper.MapCollection<P190507LocData, F1912LocData>(GetCheckedLocNodes(AllowedP190507LocNodes, node => !node.IsChecked && node.LocData != null).Select(node => node.LocData))
															.ToList();
					break;
				case P190505MovingType.UnAssignAll:
					NonAllowedF1912LocDataList.AddRange(AllowedF1912LocDataList);
					AllowedF1912LocDataList.Clear();
					break;
			}

			AllowedP190507LocNodes = GenP190507LocNodeList(AllowedF1912LocDataList);
			NonAllowedP190507LocNodes = GenP190507LocNodeList(NonAllowedF1912LocDataList);
		}

		public bool CanExecuteMoveItems(P190505MovingType type)
		{
			if (UserOperateMode == OperateMode.Query)
				return false;

			switch (type)
			{
				case P190505MovingType.Assign:
				case P190505MovingType.AssignAll:
					return NonAllowedF1912LocDataList != null && NonAllowedF1912LocDataList.Any();
				case P190505MovingType.UnAssign:
				case P190505MovingType.UnAssignAll:
					return AllowedF1912LocDataList != null && AllowedF1912LocDataList.Any();
			}

			return false;
		}

		IEnumerable<P190507LocNode> GetCheckedLocNodes(IEnumerable<P190507LocNode> nodes, Func<P190507LocNode, bool> predicate)
		{
			foreach (var node in nodes)
			{
				// 傳回目前節點有打勾的，且是最後一層
				if (predicate(node))
					yield return node;

				foreach (var item in GetCheckedLocNodes(node.LocNodes, predicate))
				{
					yield return item;
				}
			}
		}


		public void DoCheckNodes(P190507LocNode selectedNode)
		{
			RecursiveChildren(selectedNode, selectedNode.IsChecked);
			RecursiveParent(selectedNode.Parent, selectedNode.IsChecked);
		}

		private void RecursiveChildren(P190507LocNode current, bool isChecked)
		{
			foreach (var node in current.LocNodes)
			{
				node.IsChecked = isChecked;
				RecursiveChildren(node, isChecked);
			}
		}

		private void RecursiveParent(P190507LocNode current, bool isChecked)
		{
			if (current == null)
				return;

			if (isChecked)
			{
				// 若有勾子節點，則是否全部子節點都勾了
				current.IsChecked = current.LocNodes.All(x => x.IsChecked == isChecked);
			}
			else
			{
				// 子節點一個沒勾，上面的父節點就不用勾了
				current.IsChecked = false;
			}

			RecursiveParent(current.Parent, isChecked);
		}
		#endregion
		#endregion

	}
}
