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

using System.Windows;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7101040000_ViewModel : InputViewModelBase
	{
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		public Action OnSearch = delegate { };
		public Action OnSearchItemComplete = delegate { };
		/// <summary>
		/// 是否要限制不允許勾選貨主/業主
		/// </summary>
		private bool _restrictGupAndCust = false;

		public P7101040000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		public P7101040000_ViewModel(bool restrictGupAndCust = false)
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				this._restrictGupAndCust = restrictGupAndCust;
				InitControls();
			}

		}

		private void InitControls()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
			DoSearchGup();
			DoSearchCust();
			DoSearchWarehouseType();
			DoSearchLocStatusList();
			DoSearchUccList();
		}

		#region 資料連結/ 頁面參數
		#region Form - 是否要顯示GUP/ CUST下拉選單
		public Visibility GupVisibility { get { return (this._restrictGupAndCust ? Visibility.Hidden : Visibility.Visible); } }
		public Visibility CustVisibility { get { return (this._restrictGupAndCust ? Visibility.Hidden : Visibility.Visible); } }
		#endregion
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value; DoSearchGup(); }
		}
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.OrderBy(x => x.Value).ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}
		#endregion
		#region Form - 可用的GUP (業主)清單
		private ObservableCollection<NameValuePair<string>> _gupList;
		public ObservableCollection<NameValuePair<string>> GupList
		{
			get { return _gupList.OrderBy(x => x.Value).ToObservableCollection(); }
			set { _gupList = value; RaisePropertyChanged("GupList"); }
		}
		private string _selectedGup = string.Empty;
		public string SelectedGup
		{
			get { return (this._restrictGupAndCust ? this._gupCode : _selectedGup); }
			set { _selectedGup = value; DoSearchCust(); RaisePropertyChanged("SelectedGup"); }
		}
		#endregion
		#region Form - 可用的CUST (貨主)清單
		private ObservableCollection<NameValuePair<string>> _custList;
		public ObservableCollection<NameValuePair<string>> CustList
		{
			get { return _custList.OrderBy(x => x.Value).ToObservableCollection(); }
			set { _custList = value; RaisePropertyChanged("CustList"); }
		}
		private string _selectedCust = string.Empty;
		public string SelectedCust
		{
			get { return (this._restrictGupAndCust ? this._custCode : _selectedCust); }
			set { _selectedCust = value; RaisePropertyChanged("SelectedCust"); }
		}
		#endregion
		#region Form - WarehouseType
		private ObservableCollection<NameValuePair<string>> _warehouseTypeList;
		public ObservableCollection<NameValuePair<string>> WarehouseTypeList
		{
			get { return _warehouseTypeList; }
			set { _warehouseTypeList = value; RaisePropertyChanged("WarehouseTypeList"); }
		}
		public string _selectedWarehouseType = string.Empty;
		public string SelectedWarehouseType
		{
			get { return _selectedWarehouseType; }
			set { _selectedWarehouseType = value; DoSearchWarehouseList(); RaisePropertyChanged("SelectedWarehouseType"); }
		}
		#endregion
		#region Form - Warehouse
		private ObservableCollection<NameValuePair<string>> _warehouseList;
		public ObservableCollection<NameValuePair<string>> WarehouseList
		{
			get { return _warehouseList; }
			set { _warehouseList = value; RaisePropertyChanged("WarehouseList"); }
		}
		private string _selectedWarehouse = string.Empty;
		public string SelectedWarehouse
		{
			get { return _selectedWarehouse; }
			set
			{
				_selectedWarehouse = value;
				RaisePropertyChanged("SelectedWarehouse");
				DoSearchArea();
			}
		}
		#endregion
		#region Form - 儲區
		private ObservableCollection<NameValuePair<string>> _areaList;
		public ObservableCollection<NameValuePair<string>> AreaList
		{
			get { return _areaList; }
			set { _areaList = value; RaisePropertyChanged("AreaList"); }
		}
		private string _selectedArea = string.Empty;
		public string SelectedArea
		{
			get { return _selectedArea; }
			set {
				_selectedArea = value;
				RaisePropertyChanged("SelectedArea");
				DoSearchChannelList();
			}
		}
		#endregion
		#region Form - 管制狀態
		private ObservableCollection<NameValuePair<string>> _locStatusList;
		public ObservableCollection<NameValuePair<string>> LocStatusList
		{
			get { return _locStatusList.OrderBy(x => x.Value).ToObservableCollection(); }
			set { _locStatusList = value; RaisePropertyChanged("LocStatusList"); }
		}
		private string _selectedLocStatus = string.Empty;
		public string SelectedLocStatus
		{
			get { return _selectedLocStatus; }
			set { _selectedLocStatus = value; RaisePropertyChanged("SelectedLocStatus"); DoSearchUccList(); }
		}
		#endregion
		#region 通道
		private ObservableCollection<NameValuePair<string>> _channelList;
		public ObservableCollection<NameValuePair<string>> ChannelList
		{
			get { return _channelList; }
			set { _channelList = value; RaisePropertyChanged("ChannelList"); }
		}
		private string _selectedChannel = string.Empty;
		public string SelectedChannel
		{
			get { return _selectedChannel; }
			set { _selectedChannel = value; RaisePropertyChanged("SelectedChannel"); }
		}
		#endregion
		#region Form - 原因
		private ObservableCollection<NameValuePair<string>> _uccList;
		public ObservableCollection<NameValuePair<string>> UccList
		{
			get { return _uccList.OrderBy(x => x.Value).ToObservableCollection(); }
			set { _uccList = value; RaisePropertyChanged("UccList"); }
		}
		private string _selectedUcc = string.Empty;
		public string SelectedUcc
		{
			get { return _selectedUcc; }
			set { _selectedUcc = value; RaisePropertyChanged("SelectedUcc"); }
		}
		#endregion
		#region Form - 查詢依據
		private bool _searchByLoc = true;
		public bool SearchByLoc
		{
			get { return _searchByLoc; }
			set { _searchByLoc = value; RaisePropertyChanged("SearchByLoc"); }
		}
		private bool _searchByItem = false;
		public bool SearchByItem
		{
			get { return _searchByItem; }
			set { _searchByItem = value; RaisePropertyChanged("SearchByItem"); }
		}
		#endregion
		#region Form - 商品名稱
		private string _selectedItemCode = string.Empty;
		public string SelectedItemCode
		{
			get { return _selectedItemCode; }
			set { _selectedItemCode = value; }
		}
		private ObservableCollection<string> _selectedItemName = new ObservableCollection<string>();
		public ObservableCollection<string> SelectedItemName
		{
			get { return _selectedItemName; }
			set { _selectedItemName = value; RaisePropertyChanged("SelectedItemName"); }
		}
		#endregion
		#region Form - 勾選所有
		private bool _isCheckAll = false;
		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set { _isCheckAll = value; RaisePropertyChanged("IsCheckAll"); }
		}
		#endregion
		#region Form - 管制數量
		private string _selectedCount = string.Empty;
		public string SelectedCount
		{
			get { return _selectedCount; }
			set { _selectedCount = value; RaisePropertyChanged("SelectedCount"); }
		}
		#endregion
		#region Data - 查詢結果
		private ObservableCollection<SelectionItem<F1912StatusEx>> _locList = new ObservableCollection<SelectionItem<F1912StatusEx>>();
		public ObservableCollection<SelectionItem<F1912StatusEx>> LocList
		{
			get { return _locList; }
			set { _locList = value; RaisePropertyChanged("LocList"); SelectedCount = LocList.Where(x => x.IsSelected == true).Count().ToString(); }
		}
		#endregion
		#endregion

		#region Command
		#region CheckAll
		public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckAllItem()
				);
			}
		}

		public void DoCheckAllItem()
		{
			if (LocList == null)
				return;

			foreach (var p in LocList)
			{
				p.IsSelected = IsCheckAll;
			}
			//RaisePropertyChanged("LocList");
		}
		#endregion
		#region Search
		public ICommand SearchItemNameCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchItemName(),
					() => true,
					o => OnSearchItemComplete()
				);
			}
		}
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						DoSearch();
					},
					() =>
					{
						return UserOperateMode == OperateMode.Query;
					},
					o => OnSearch() // 更新表格呈現方式(隱藏/顯示ITEM)
				);
			}
		}

		private void DoSearch()
		{
			IsCheckAll = false;
			RaisePropertyChanged("ItemVisibility");
			//執行查詢動作
			if (SearchByLoc)
			{
				var proxyEx = GetExProxy<P71ExDataSource>();
				var result = proxyEx.CreateQuery<F1912StatusEx>("GetLocListForLocControl")
					.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
					.AddQueryOption("gupCode", string.Format("'{0}'", SelectedGup))
					.AddQueryOption("custCode", string.Format("'{0}'", SelectedCust))
					.AddQueryOption("warehouseType", string.Format("'{0}'", SelectedWarehouseType))
					.AddQueryOption("warehouseId", string.Format("'{0}'", SelectedWarehouse))
					.AddQueryOption("areaId", string.Format("'{0}'", SelectedArea))
					.AddQueryOption("channel", string.Format("'{0}'", SelectedChannel))
					.AddQueryOption("itemCode", string.Format("'{0}'", string.Empty))
					.AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
					.ToList();
				LocList = result.ToSelectionList();
				if (!result.Any()) ShowMessage(Messages.InfoNoData);
			}
			else if (SearchByItem)
			{
				var proxyEx = GetExProxy<P71ExDataSource>();
				var result = proxyEx.CreateQuery<F1912StatusEx2>("GetLocListForLocControlByItemCode")
					.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
					.AddQueryOption("gupCode", string.Format("'{0}'", SelectedGup))
					.AddQueryOption("custCode", string.Format("'{0}'", SelectedCust))
					.AddQueryOption("itemCode", string.Format("'{0}'", SelectedItemCode.Trim()))
					.AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
					.ToList();
				LocList = result.Select(AutoMapper.Mapper.DynamicMap<F1912StatusEx>).ToSelectionList();
				if (!result.Any()) ShowMessage(Messages.InfoNoData);
			}
		}

		private void DoSearchGup()
		{
			if (this._restrictGupAndCust)
			{
				// 如果是從P17進來的, 則限制只能存取當前的GUP
				GupList = new List<NameValuePair<string>>() {
					new NameValuePair<string>() {
						Value = this._gupCode,
						Name = Wms3plSession.Get<GlobalInfo>().GupName
					}
				}.ToObservableCollection();
			}
			else
			{
				var result = Wms3plSession.Get<GlobalInfo>().GetGupDataList(SelectedDc).ToObservableCollection();
				result.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
				GupList = result;
			}
			SelectedGup = GupList.FirstOrDefault().Value;
		}

		private void DoSearchCust()
		{
			if (this._restrictGupAndCust)
			{
				// 如果是從P17進來的, 則限制只能存取當前的CUST
				CustList = new List<NameValuePair<string>>() {
					new NameValuePair<string>() {
						Value = this._custCode,
						Name = Wms3plSession.Get<GlobalInfo>().CustName
					}
				}.ToObservableCollection();
			}
			else
			{
				var result = Wms3plSession.Get<GlobalInfo>().GetCustDataList(SelectedDc, SelectedGup).ToObservableCollection();
				result.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
				CustList = result;
			}
			SelectedCust = CustList.FirstOrDefault().Value;
		}

		private void DoSearchArea()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1919s.Where(x => x.DC_CODE == SelectedDc && x.WAREHOUSE_ID == SelectedWarehouse)
									.OrderBy(x => x.AREA_CODE)
									.Select(x => new NameValuePair<string>() { Value = x.AREA_CODE, Name = x.AREA_NAME })
									.ToList();
			result.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
			AreaList = result.ToObservableCollection();
			if (AreaList.Any()) SelectedArea = AreaList.FirstOrDefault().Value;
		}

		private void DoSearchWarehouseType()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F198001s
				.OrderBy(x => x.TYPE_ID)
				.Select(x => new NameValuePair<string>() { Value = x.TYPE_ID, Name = x.TYPE_NAME })
				.ToList();
			WarehouseTypeList = result.ToObservableCollection();
			if (WarehouseTypeList.Any()) SelectedWarehouseType = WarehouseTypeList.FirstOrDefault().Value;
		}

		private void DoSearchWarehouseList()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1980s.Where(x => x.DC_CODE.Equals(SelectedDc) && x.WAREHOUSE_TYPE.Equals(SelectedWarehouseType))
				.OrderBy(x => x.WAREHOUSE_ID)
				.Select(x => new NameValuePair<string>() { Value = x.WAREHOUSE_ID, Name = x.WAREHOUSE_NAME })
				.ToList();
			WarehouseList = result.ToObservableCollection();
			SelectedWarehouse = WarehouseList.FirstOrDefault().Value;
		}

		private void DoSearchLocStatusList()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1943s.Select(x => new NameValuePair<string>() { Value = x.LOC_STATUS_ID, Name = x.LOC_STATUS_NAME })
				.ToList();
			LocStatusList = result.ToObservableCollection();
			SelectedLocStatus = LocStatusList.FirstOrDefault().Value;
		}

		private void DoSearchChannelList()
		{
			ChannelList = new ObservableCollection<NameValuePair<string>>();
			var proxy = GetProxy<F19Entities>();
			var f1912s = proxy.F1912s.Where(x => x.DC_CODE == SelectedDc && x.WAREHOUSE_ID == SelectedWarehouse).ToList();
			
			if (!string.IsNullOrWhiteSpace(SelectedArea))
			{
				f1912s = f1912s.Where(x => x.AREA_CODE == SelectedArea).ToList();
			}
			var channelList = f1912s.Select(x => x.CHANNEL).Distinct().ToList();
			var result = channelList.Select(x => new NameValuePair<string>() { Value = x, Name = x }).ToList();
			result.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
			ChannelList = result.ToObservableCollection();
			SelectedChannel = ChannelList.FirstOrDefault().Value;
		}

		private void DoSearchUccList()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1951s.Where(x => x.UCT_ID.Equals("LB"))
				.Select(x => new NameValuePair<string>() { Value = x.UCC_CODE, Name = x.CAUSE })
				.ToList();

			result.Insert(0, new NameValuePair<string>(string.Empty, string.Empty));

            //判斷管制狀態為使用中、或其他時的原因項目
            if (SelectedLocStatus == "01")
            {
                result = result.Where(o => o.Name == "").ToList();
            }
            else
            {
                //20211210:顯示全部的原因清單
                //result = result.Where(o => o.Value == "701").ToList();
            }

            UccList = result.ToObservableCollection();
			SelectedUcc = UccList.FirstOrDefault().Value;
		}

		/// <summary>
		/// 依商品編號查找商品名稱
		/// </summary>
		public void DoSearchItemName()
		{
			var tmpList = new List<string>();
			if (string.IsNullOrWhiteSpace(SelectedItemCode)) return;

			var proxy = GetProxy<F19Entities>();
			var result = proxy.CreateQuery<F1903>("GetF1903sByItemCode")
				.AddQueryOption("gupCode", string.Format("'{0}'", SelectedGup))
				.AddQueryOption("itemCode", string.Format("'{0}'", SelectedItemCode))
                .AddQueryOption("custCode", string.Format("'{0}'",SelectedCust))
				.AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
				.ToList();
			if (result != null)
			{
				foreach (var p in result)
				{
					var tmpGup = proxy.F1929s.Where(x => x.GUP_CODE == p.GUP_CODE).FirstOrDefault();
					if (tmpGup != null) tmpList.Add(tmpGup.GUP_NAME + " " + p.ITEM_NAME);
					else tmpList.Add(p.ITEM_NAME);
				}
				SelectedItemName = tmpList.ToObservableCollection();
			}
			else
			{
				ShowMessage(new MessagesStruct()
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Title = Resources.Resources.Information,
					Message = Properties.Resources.P7101040000_ViewModel_ItemCodeIncorrect
				});
			}
		}
		#endregion Search


		#region Edit

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && LocList != null && LocList.Any()
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作

			IsCheckAll = false;
			DoCheckAllItem();
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
						if (ShowMessage(Messages.WarningBeforeCancel) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}
						DoCancel();
					},
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						IsCheckAll = false;
						DoCheckAllItem();
					}
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作

			UserOperateMode = OperateMode.Query;

		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() =>
					{
						// 利用ICommand來更新SelectedCount, 不用再到處塞Trigger
						SelectedCount = LocList.Where(x => x.IsSelected == true).Count().ToString();

						return UserOperateMode != OperateMode.Query && LocList != null && LocList.Any(x => x.IsSelected == true);
					}
				);
			}
		}

		private void DoSave()
		{
			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes) return;

			//執行確認儲存動作
			if (DoCheckData())
			{
				var proxy = new wcf.P71WcfServiceClient();
				List<wcf.F1912StatusEx> tmp = LocList.Where(x => x.IsSelected == true)
					.Select(x => new wcf.F1912StatusEx()
					{
						AREA_CODE = x.Item.AREA_CODE,
						CUST_CODE = x.Item.CUST_CODE,
						DC_CODE = x.Item.DC_CODE,
						GUP_CODE = x.Item.GUP_CODE,
						LOC_CODE = x.Item.LOC_CODE,
						WAREHOUSE_ID = x.Item.WAREHOUSE_ID
					}).ToList();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateP710104(tmp.ToArray(), SelectedLocStatus, SelectedUcc, this._userId));
				if (result.IsSuccessed == true)
				{
					DoSearch();
					UserOperateMode = OperateMode.Query;
				}
				ShowMessage(new List<ExecuteResult>() { new ExecuteResult() { IsSuccessed = result.IsSuccessed, Message = result.Message } });
			}
		}
		private bool DoCheckData()
		{
			string msg = string.Empty;
			if (string.IsNullOrWhiteSpace(SelectedLocStatus)) msg = Properties.Resources.P7101040000_ViewModel_ControlStatusEmpty;
            //空白改成正常，所以空值=正常，因此取消此錯誤訊息。
			//if (string.IsNullOrWhiteSpace(SelectedUcc)) msg = Properties.Resources.P7101040000_ViewModel_ControlReasonEmpty;
			if (string.IsNullOrEmpty(msg))
			{
				return true;
			}
			else
			{
				ShowMessage(new MessagesStruct()
				{
					Button = DialogButton.OK,
					Image = DialogImage.Warning,
					Message = msg,
					Title = Resources.Resources.Information
				}
				);
				return false;
			}
		}
		#endregion Save
		#endregion
	}
}
