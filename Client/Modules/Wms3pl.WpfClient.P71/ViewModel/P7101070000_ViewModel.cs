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

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7101070000_ViewModel : InputViewModelBase
	{
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		public Action OnSearch = delegate { };
		/// <summary>
		/// 是否要限制不允許勾選貨主/業主
		/// </summary>
		private bool _restrictGupAndCust = false;

		public P7101070000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		public P7101070000_ViewModel(bool restrictGupAndCust = false)
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
			DoSearchWarehouseList();
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
			get { return _warehouseTypeList.OrderBy(x => x.Value).ToObservableCollection(); }
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
			get { return _warehouseList.OrderBy(x => x.Value).ToObservableCollection(); }
			set { _warehouseList = value; RaisePropertyChanged("WarehouseList"); }
		}
		private string _selectedWarehouse = string.Empty;
		public string SelectedWarehouse
		{
			get { return _selectedWarehouse; }
			set { _selectedWarehouse = value; RaisePropertyChanged("SelectedWarehouse"); }
		}
		#endregion
		#region Data - 查詢結果
		private ObservableCollection<F1912StatisticReport> _locList;
		public ObservableCollection<F1912StatisticReport> LocList
		{
			get { return _locList; }
			set { _locList = value; RaisePropertyChanged("LocList"); }
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
			// 執行查詢動作
			var proxyEx = GetExProxy<P71ExDataSource>();
			var result = proxyEx.CreateQuery<F1912StatisticReport>("GetLocStatisticForLocControl")
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				.AddQueryOption("gupCode", string.Format("'{0}'", SelectedGup))
				.AddQueryOption("custCode", string.Format("'{0}'", SelectedCust))
				.AddQueryOption("warehouseType", string.Format("'{0}'", SelectedWarehouseType))
				.AddQueryOption("warehouseId", string.Format("'{0}'", SelectedWarehouse))
				.AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
				.ToList();
			LocList = result.ToObservableCollection();
			if (!result.Any()) ShowMessage(Messages.InfoNoData);
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
				result.Insert(0, new NameValuePair<string>() { Value = "0", Name = Resources.Resources.All });
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
				result.Insert(0, new NameValuePair<string>() { Value = "0", Name = Resources.Resources.All });
				CustList = result;
			}
			SelectedCust = CustList.FirstOrDefault().Value;
		}

		private void DoSearchWarehouseType()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F198001s.Select(x => new NameValuePair<string>() { Value = x.TYPE_ID, Name = x.TYPE_NAME })
				.ToList();
			result.Insert(0, new NameValuePair<string>(Resources.Resources.All, string.Empty));
			WarehouseTypeList = result.ToObservableCollection();
			if (WarehouseTypeList.Any()) SelectedWarehouseType = WarehouseTypeList.FirstOrDefault().Value;
		}

		private void DoSearchWarehouseList()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1980s.Where(x => x.DC_CODE.Equals(SelectedDc) && x.WAREHOUSE_TYPE.Equals(SelectedWarehouseType))
				.Select(x => new NameValuePair<string>() { Value = x.WAREHOUSE_ID, Name = x.WAREHOUSE_NAME })
				.ToList();
			result.Insert(0, new NameValuePair<string>() { Value = "0", Name = Resources.Resources.All });
			WarehouseList = result.ToObservableCollection();
			SelectedWarehouse = WarehouseList.FirstOrDefault().Value;
		}

		#endregion Search
		#endregion
	}
}
