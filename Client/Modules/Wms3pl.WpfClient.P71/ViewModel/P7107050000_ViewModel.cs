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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P50ExDataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7107050000_ViewModel : InputViewModelBase
	{
		public P7107050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				Init();
			}

		}

		private void Init()
		{
			//var userInfo = Wms3plSession.Get<UserInfo>();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();

			ReloadReportType();
			ReloadWareHouse();

			DcList = globalInfo.DcCodeList;
			if (DcList.Any())
				SelectDcCode = DcList.Select(x => x.Value).FirstOrDefault();//倉別選單在此載入

			GupList = GetGupData();
			SelectedGupCode = GupList.Select(x => x.Value).FirstOrDefault();

			ProductQty = 500;

			EnterDateBegin = DateTime.Today.AddDays(-10);
			EnterDateEnd = DateTime.Today;
		}

		#region Property

		#region 報表類別
		private List<NameValuePair<string>> _reportTypeList;

		public List<NameValuePair<string>> ReportTypeList
		{
			get { return _reportTypeList; }
			set
			{
				_reportTypeList = value;
				RaisePropertyChanged("ReportTypeList");
			}
		}

		private string _selectedReportType;

		public string SelectReportType
		{
			get { return _selectedReportType; }
			set
			{
				if (_selectedReportType == value)
					return;
				_selectedReportType = value;
				RaisePropertyChanged("SelectReportType");
			}
		}
		#endregion

		#region DC

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

		private string _selectedDcCode;

		public string SelectDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				if (_selectedDcCode == value)
					return;
				_selectedDcCode = value;
				GetGupData();
				RaisePropertyChanged("SelectDcCode");
			}
		}

		#endregion

		#region 稽核起訖日

		private DateTime? _selectInventoryDate;
		public DateTime? SelectInventoryDate
		{
			get { return _selectInventoryDate; }
			set
			{
				if (_selectInventoryDate == value)
					return;
				_selectInventoryDate = value;
				RaisePropertyChanged("SelectInventoryDate");
			}
		}

		//private DateTime? _searchStartDate;
		//public DateTime? SearchStartDate
		//{
		//	get { return _searchStartDate; }
		//	set
		//	{
		//		if (_searchStartDate == value)
		//			return;
		//		_searchStartDate = value;
		//		RaisePropertyChanged("SearchStartDate");
		//	}
		//}

		//private DateTime? _searchEndDate;
		//public DateTime? SearchEndDate
		//{
		//	get { return _searchEndDate; }
		//	set
		//	{
		//		if (_searchEndDate == value)
		//			return;
		//		_searchEndDate = value;
		//		RaisePropertyChanged("SearchEndDate");
		//	}
		//}

		#endregion

		#region 業主
		private List<NameValuePair<string>> _gupList;
		public List<NameValuePair<string>> GupList
		{
			get { return _gupList; }
			set
			{
				_gupList = value;
				RaisePropertyChanged("GupList");
			}
		}

		private string _selectedGupCode;
		public string SelectedGupCode
		{
			get { return _selectedGupCode; }
			set
			{
				if (_selectedGupCode == value)
					return;
				_selectedGupCode = value;
				ReloadCustCode();
				ReloadVnrCode();
				RaisePropertyChanged("SelectedGupCode");
			}
		}
		#endregion

		#region 貨主
		private List<NameValuePair<string>> _custList;
		public List<NameValuePair<string>> CustList
		{
			get { return _custList; }
			set
			{
				if (_custList == value)
					return;
				_custList = value;
				ReloadVnrCode();
				RaisePropertyChanged("CustList");
			}
		}

		private string _selectedCustCode;
		public string SelectedCustCode
		{
			get { return _selectedCustCode; }
			set
			{
				_selectedCustCode = value;
				RaisePropertyChanged("SelectedCustCode");
			}
		}
		#endregion

		#region 供應商
		private List<NameValuePair<string>> _vnrList;
		public List<NameValuePair<string>> VnrList
		{
			get { return _vnrList; }
			set
			{
				_vnrList = value;
				RaisePropertyChanged("VnrList");
			}
		}
		private string _selectedVnrCode;
		public string SelectedVnrCode
		{
			get { return _selectedVnrCode; }
			set
			{
				_selectedVnrCode = value;
				RaisePropertyChanged("SelectedVnrCode");
			}
		}
		#endregion

		#region 商品數量
		private int? _productQty;
		public int? ProductQty
		{
			get { return _productQty; }
			set
			{
				if (_productQty == value)
					return;
				_productQty = value;
				RaisePropertyChanged("ProductQty");
			}
		}
		#endregion

		#region 倉別
		private List<NameValuePair<string>> _wareHouseList;
		/// <summary>
		/// 倉別資料
		/// </summary>
		public List<NameValuePair<string>> WareHouseList
		{
			get { return _wareHouseList; }
			set
			{
				_wareHouseList = value;
				RaisePropertyChanged("WareHouseList");
			}
		}
		private string _selectedWareHouseId;
		/// <summary>
		/// 選擇倉別
		/// </summary>
		public string SelectedWareHouseId
		{
			get { return _selectedWareHouseId; }
			set
			{
				_selectedWareHouseId = value;
				RaisePropertyChanged("SelectedWareHouseId");
			}
		}
		#endregion

		#region 起訖入庫日
		private DateTime? _enterDateBegin;

		public DateTime? EnterDateBegin
		{
			get { return _enterDateBegin; }
			set
			{
				Set(() => EnterDateBegin, ref _enterDateBegin, value);
			}
		}

		private DateTime? _enterDateEnd;

		public DateTime? EnterDateEnd
		{
			get { return _enterDateEnd; }
			set
			{
				Set(() => EnterDateEnd, ref _enterDateEnd, value);
			}
		}


		#endregion

		#region 起訖儲位號
		private string _startLocCode;
		public string StartLocCode
		{
			get { return _startLocCode; }
			set
			{
				if (_startLocCode == value)
					return;
				_startLocCode = value;
				RaisePropertyChanged("StartLocCode");
			}
		}

		private string _endLocCode;
		public string EndLocCode
		{
			get { return _endLocCode; }
			set
			{
				if (_endLocCode == value)
					return;
				_endLocCode = value;
				RaisePropertyChanged("EndLocCode");
			}
		}
		#endregion

		#region 起訖品號
		private string _itemCodes;
		public string ItemCodes
		{
			get { return _itemCodes; }
			set
			{
				if (_itemCodes == value)
					return;
				_itemCodes = value;
				RaisePropertyChanged("ItemCodes");
			}
		}

		#endregion

		#region 商品搜尋用欄位

		private bool _isItemCode;

		public bool IsItemCode
		{
			get { return _isItemCode; }
			set
			{
				if (_isItemCode == value)
					return;
				_isItemCode = value;
				RaisePropertyChanged("IsItemCode");
			}
		}

		private string _itemCode;
		/// <summary>
		/// 商品品號
		/// </summary>
		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				_itemCode = value;
				RaisePropertyChanged("ItemCode");
			}
		}
		private string _itemName;
		/// <summary>
		/// 商品品名
		/// </summary>
		public string ItemName
		{
			get { return _itemName; }
			set
			{
				_itemName = value;
				RaisePropertyChanged("ItemName");
			}
		}
		private string _itemSize;
		public string ItemSize
		{
			get { return _itemSize; }
			set
			{
				_itemSize = value;
				RaisePropertyChanged("ItemSize");
			}
		}
		private string _itemSpec;
		public string ItemSpec
		{
			get { return _itemSpec; }
			set
			{
				_itemSpec = value;
				RaisePropertyChanged("ItemSpec");
			}
		}
		private string _itemColor;
		public string ItemColor
		{
			get { return _itemColor; }
			set
			{
				_itemColor = value;
				RaisePropertyChanged("ItemColor");
			}
		}


		#endregion

		#region GridData


		private List<P710705BackWarehouseInventory> _p710705BackWarehouseInventoryData;

		public List<P710705BackWarehouseInventory> P710705BackWarehouseInventoryData
		{
			get { return _p710705BackWarehouseInventoryData; }
			set
			{
				_p710705BackWarehouseInventoryData = value;
				RaisePropertyChanged("P710705BackWarehouseInventoryData");
			}
		}

		private List<P710705MergeExecution> _p710705MergeExecutionData;

		public List<P710705MergeExecution> P710705MergeExecutionData
		{
			get { return _p710705MergeExecutionData; }
			set
			{
				_p710705MergeExecutionData = value;
				RaisePropertyChanged("P710705MergeExecutionData");
			}
		}

		private List<P710705Availability> _p710705AvailabilityData;

		public List<P710705Availability> P710705AvailabilityData
		{
			get { return _p710705AvailabilityData; }
			set
			{
				_p710705AvailabilityData = value;
				RaisePropertyChanged("P710705AvailabilityData");
			}
		}

		private List<P710705ChangeDetail> _p710705ChangeDetailData;

		public List<P710705ChangeDetail> P710705ChangeDetailData
		{
			get { return _p710705ChangeDetailData; }
			set
			{
				_p710705ChangeDetailData = value;
				RaisePropertyChanged("P710705ChangeDetailData");
			}
		}

		private List<P710705WarehouseDetail> _p710705WarehouseDetailData;

		public List<P710705WarehouseDetail> P710705WarehouseDetailData
		{
			get { return _p710705WarehouseDetailData; }
			set
			{
				_p710705WarehouseDetailData = value;
				RaisePropertyChanged("P710705WarehouseDetailData");
			}
		}

		#endregion

		#endregion

		#region 貼上品號
		public ICommand PasteCommand
		{
			get
			{
				return new RelayCommand(
					() =>
					{
						IsBusy = true;
						try
						{
							DoPaste();
						}
						catch (Exception ex)
						{
							Exception = ex;
							IsBusy = false;
						}
						IsBusy = false;
					},
				() => !IsBusy);
			}
		}

		private void DoPaste()
		{
			if (Clipboard.ContainsData(DataFormats.Text))
			{
				var pastData = Clipboard.GetDataObject();
				if (pastData != null && pastData.GetDataPresent(DataFormats.Text))
				{
					var content = pastData.GetData(DataFormats.Text).ToString();
					var contentArray = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
					ItemCodes = string.Join(",", contentArray.Select(p => p).Where(p => !string.IsNullOrWhiteSpace(p)));
				}
			}
		}
		#endregion 貼上品號

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearch()
		{
			switch (SelectReportType)
			{
				case "0":
					P710705BackWarehouseInventoryData = GetP710705BackWarehouseInventory();
					if (P710705BackWarehouseInventoryData == null || !P710705BackWarehouseInventoryData.Any())
					{
						ShowMessage(Messages.InfoNoData);
					}
					break;
				case "1":
					if (!ProductQty.HasValue)
					{
						ShowWarningMessage(Properties.Resources.P7107050000_ViewModel_LocMergeAcc_ItemCountLimit);
						return;
					}
					//else
					//{
					//	if (ProductQty.Value >= 200)
					//	{
					//		if (ShowConfirmMessage(Properties.Resources.P7107050000_ViewModel_LocMergeAcc_ItemCountLimit_SureAcc) == DialogResponse.No)
					//		{
					//			return;
					//		}
					//	}
					//}
					P710705MergeExecutionData = GetP710705MergeExecution();
					if (P710705MergeExecutionData == null || !P710705MergeExecutionData.Any())
					{
						ShowMessage(Messages.InfoNoData);
					}
					break;
				case "2":
					P710705AvailabilityData = GetP710705Availability();
					if (P710705AvailabilityData == null || !P710705AvailabilityData.Any())
					{
						ShowMessage(Messages.InfoNoData);
					}
					break;
				case "3":
					P710705ChangeDetailData = GetP710705ChangeDetail();
					if (P710705ChangeDetailData == null || !P710705ChangeDetailData.Any())
					{
						ShowMessage(Messages.InfoNoData);
					}
					break;
				case "4":
					P710705WarehouseDetailData = GetP710705WarehouseDetail();
					if (P710705WarehouseDetailData == null || !P710705WarehouseDetailData.Any())
					{
						ShowMessage(Messages.InfoNoData);
					}
					break;
			}
		}
		#endregion Search

		#region Print Or Prview

		public Action<PrintType> DoPrint = delegate { };
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
				(t) =>
				{
					IsBusy = true;
					try
					{
						DoPrint(t);
					}
					catch (Exception ex)
					{
						Exception = ex;
						IsBusy = false;
					}
					IsBusy = false;
				},
				(t) => CanPrint());
			}
		}

		private bool CanPrint()
		{
			return true;
		}

		#endregion

		#region Add
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
			//執行新增動作
		}
		#endregion Add

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
			//執行編輯動作
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

			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save

		#region Data

		public List<P710705BackWarehouseInventory> GetP710705BackWarehouseInventory()
		{
			var proxyEx = GetExProxy<P71ExDataSource>();
			return proxyEx.CreateQuery<P710705BackWarehouseInventory>("GetP710705BackWarehouseInventory")
				.AddQueryOption("dcCode", QueryFormat(SelectDcCode))
				.AddQueryOption("gupCode", QueryFormat(SelectedGupCode))
				.AddQueryOption("custCode", QueryFormat(SelectedCustCode))
				.AddQueryOption("vnrCode", QueryFormat(SelectedVnrCode))
				.AddQueryOption("account", QueryFormat(Wms3plSession.CurrentUserInfo.Account))
				.ToList();
		}
		public List<P710705MergeExecution> GetP710705MergeExecution()
		{
			var proxyEx = GetExProxy<P71ExDataSource>();
			return proxyEx.CreateQuery<P710705MergeExecution>("GetP710705MergeExecution")
				.AddQueryExOption("dcCode", SelectDcCode)
				.AddQueryExOption("qty", ProductQty)
				.ToList();
		}

		public List<P710705Availability> GetP710705Availability()
		{
			var proxyEx = GetExProxy<P71ExDataSource>();
			return proxyEx.CreateQuery<P710705Availability>("GetP710705Availability")
				.AddQueryExOption("dcCode", SelectDcCode)
				.AddQueryExOption("gupCode", SelectedGupCode)
				.AddQueryExOption("custCode", SelectedCustCode)
				.AddQueryExOption("inventoryDate", SelectInventoryDate)
				.AddQueryExOption("account", Wms3plSession.CurrentUserInfo.Account)
				.ToList();
		}

		public List<P710705ChangeDetail> GetP710705ChangeDetail()
		{
			ValidateHelper.AutoChangeBeginEnd(this, x => x.EnterDateBegin, x => x.EnterDateEnd, isAutoChangeBeginEnd: true, isSetNullProperty: false);

			var proxyEx = GetExProxy<P71ExDataSource>();
			return proxyEx.GetP710705ChangeDetail(SelectedWareHouseId,
													StartLocCode,
													EndLocCode,
													string.IsNullOrWhiteSpace(ItemCodes) ? String.Empty : ItemCodes.Replace(",", "^"),
													EnterDateBegin,
													EnterDateEnd).ToList();
		}

		public List<P710705WarehouseDetail> GetP710705WarehouseDetail()
		{
			var proxyEx = GetExProxy<P71ExDataSource>();
			return proxyEx.CreateQuery<P710705WarehouseDetail>("GetP710705WarehouseDetail")
				.AddQueryOption("gupCode", QueryFormat(SelectedGupCode))
				.AddQueryOption("custCode", QueryFormat(SelectedCustCode))
				.AddQueryOption("warehouseId", QueryFormat(SelectedWareHouseId))
				.AddQueryOption("srcLocCode", QueryFormat(StartLocCode))
				.AddQueryOption("tarLocCode", QueryFormat(EndLocCode))
				.AddQueryOption("itemCode", QueryFormat(ItemCode))
				.AddQueryOption("account", QueryFormat(Wms3plSession.CurrentUserInfo.Account))
				.ToList();
		}

		#region Reload
		/// <summary>
		/// 重新載入[報表類別]資料
		/// </summary>
		private void ReloadReportType()
		{
			ReportTypeList = GetBaseTableService.GetF000904List(FunctionCode, "P7107050000", "ReportType");
			SelectReportType = ReportTypeList.Select(x => x.Value).FirstOrDefault();
		}
		/// <summary>
		/// 重新載入[貨主]資料
		/// </summary>
		private void ReloadCustCode()
		{
			CustList = GetCustData(SelectedGupCode);
			SelectedCustCode = CustList.Select(x => x.Value).FirstOrDefault();
		}
		/// <summary>
		/// 重新載入[供應商]資料
		/// </summary>
		private void ReloadVnrCode()
		{
			VnrList = GetVnrData(SelectedGupCode);
			SelectedVnrCode = VnrList.Select(x => x.Value).FirstOrDefault();
		}
		/// <summary>
		/// 重新載入[倉別]資料 - 選單類別
		/// </summary>
		private void ReloadWareHouse()
		{
			WareHouseList = GetWareHouse();
			SelectedWareHouseId = WareHouseList.Select(x => x.Value).FirstOrDefault();
		}
		#endregion

		#region 下拉選單資料
		/// <summary>
		/// 取得[業主]資料
		/// </summary>
		public List<NameValuePair<string>> GetGupData(bool isAll = true)
		{
			var data = Wms3plSession.Get<GlobalInfo>().GetGupDataList(SelectDcCode);
			if (isAll)
				data.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			return data;
		}
		/// <summary>
		/// 取得[貨主]資料
		/// </summary>
		private List<NameValuePair<string>> GetCustData(string gupCode, bool isAll = true)
		{
			var data = Wms3plSession.Get<GlobalInfo>().GetCustDataList(SelectDcCode, gupCode);

			if (isAll)
				data.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			return data;
		}
		/// <summary>
		/// 取得[供應商]資料
		/// </summary>
		public List<NameValuePair<string>> GetVnrData(string gupCode, bool isAll = true)
		{
			var proxy = GetProxy<F19Entities>();
			var data = (from x in proxy.F1908s
						where string.IsNullOrWhiteSpace(gupCode) || x.GUP_CODE == gupCode
						select new NameValuePair<string>()
						{
							Value = x.VNR_CODE,
							Name = x.VNR_NAME
						}).ToList();

			if (isAll)
				data.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			return data;
		}
		/// <summary>
		/// 取得[倉別]資料
		/// </summary>
		public List<NameValuePair<string>> GetWareHouse(string dcCode = "", bool isAll = true)
		{
			var proxy = GetProxy<F19Entities>();
			var data = (from o in proxy.F1980s
						where (string.IsNullOrWhiteSpace(dcCode) || (o.DC_CODE == dcCode))
						select new NameValuePair<string>
						{
							Name = o.WAREHOUSE_NAME,
							Value = o.WAREHOUSE_ID
						}).ToList();

			if (isAll)
				data.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			return data;
		}
		#endregion

		#endregion

	}

	public class ReportTypeConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var parameterText = (string)parameter;
			var valueText = (string)value;
			return (parameterText == valueText) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}