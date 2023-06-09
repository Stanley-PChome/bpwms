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
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using System.Windows;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.LabelPrinter.Bartender;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Services;
using CrystalDecisions.CrystalReports.Engine;
using Wms3pl.WpfClient.P71.Report;
using Wms3pl.WpfClient.Common.Helpers;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public enum ChoseOptionType
	{
		None,
		/// <summary>
		/// 儲位編號
		/// </summary>
		LocCode,
		/// <summary>
		/// 商品編號
		/// </summary>
		ItemCode
	}
	public enum PrintBy
	{
		a4Printer,
		labelPrinter,
		bartender
	}
	public partial class P7101080000_ViewModel : InputViewModelBase
	{
		/// <summary>
		/// 是否要限制不允許勾選貨主/業主
		/// </summary>
		private bool _restrictGupAndCust = false;
		public bool restrictGupAndCust
		{
			get { return _restrictGupAndCust; }
			set { _restrictGupAndCust = value; }
		}
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		public Action<PrintType, PrintBy,ReportClass> DoPrint = delegate { };
		public Action OnDcCodeChanged = () => { };
		public P7101080000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}

		}

		#region **Prop
		#region Form - 是否要顯示GUP/ CUST下拉選單
		public Visibility GupVisibility { get { return (this._restrictGupAndCust ? Visibility.Hidden : Visibility.Visible); } }
		public Visibility CustVisibility { get { return (this._restrictGupAndCust ? Visibility.Hidden : Visibility.Visible); } }
		#endregion
		private ChoseOptionType _choseOption = ChoseOptionType.LocCode;
		public ChoseOptionType ChoseOption
		{
			get { return _choseOption; }
			set
			{
				_choseOption = value;
				RaisePropertyChanged("ChoseOption");
			}
		}

		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				Set(() => SelectedDc, ref _selectedDc, value);
				if (value != null)
					OnDcCodeChanged();
                DoSearchWarehouseList();

            }
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
			set
			{
				_selectedCust = value;
				RaisePropertyChanged("SelectedCust");
			}
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
		#region Form - 儲位編號起迄
		private string _searchStartCode = string.Empty;
		public string SearchStartCode
		{
			get { return _searchStartCode; }
			set
			{
				_searchStartCode = value;
				RaisePropertyChanged("SearchStartCode");
			}
		}
		private string _searchEndCode = string.Empty;
		public string SearchEndCode
		{
			get { return _searchEndCode; }
			set { _searchEndCode = value; RaisePropertyChanged("SearchEndCode"); }
		}
		#endregion

		#region Form - 商品編號
		private string _searchItemCode = string.Empty;
		public string SearchItemCode
		{
			get { return _searchItemCode; }
			set
			{
				_searchItemCode = value;
				RaisePropertyChanged("SearchItemCode");
			}
		}

		private List<NameValuePair<string>> _itemList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> ItemList
		{
			get { return _itemList; }
			set { _itemList = value; RaisePropertyChanged("ItemList"); }
		}
		private NameValuePair<string> _selectedItem;
		public NameValuePair<string> SelectedItem
		{
			get { return _selectedItem; }
			set { _selectedItem = value; RaisePropertyChanged("SelectedItem"); }
		}
		#endregion

		#region 儲位卡資料
		private List<wcf.F1912DataLoc> _locList;
		public List<wcf.F1912DataLoc> LocList
		{
			get { return _locList; }
			set { _locList = value; RaisePropertyChanged("LocList"); }
		}
		#endregion

		#region 新增儲位卡資料R71010803

		private List<wcf.F1912DataLocByLocType> _locListR71010803;
		public List<wcf.F1912DataLocByLocType> LocListR71010803
		{
			get { return _locListR71010803; }
			set { _locListR71010803 = value; RaisePropertyChanged("LocListR71010803"); }
		}
		#endregion
		#region 新增僅列印空儲位
		
		private bool _printEmpty =false;
		public bool printEmpty
		{
			get { return _printEmpty; }
			set { _printEmpty = value; RaisePropertyChanged("printEmpty"); }
		}
		#endregion
		/// <summary>
		/// Device 的設定 (當物流中心改變時，就會去顯示設定 Device 的畫面)  
		/// </summary>
		public F910501 SelectedF910501 { get; set; }

		private bool _isUseLabelPrint;

		public bool IsUseLabelPrint
		{
			get { return _isUseLabelPrint; }
			set
			{
				Set(ref _isUseLabelPrint, value);
			}
		}


		#region 是否使用標籤格式列印
		private bool _isUseLabelStylePrint;

		public bool IsUseLabelStylePrint
		{
			get { return _isUseLabelStylePrint; }
			set
			{
				Set(() => IsUseLabelStylePrint, ref _isUseLabelStylePrint, value);
			}
		}
		#endregion

		#endregion

		#region **Func
		private void InitControls()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
			DoSearchGup();
			DoSearchCust();
			DoSearchWarehouseList();
			GetSize(selectPrinter);
		}

		public Action OnScrollIntoViewItems = () => { };

		private void SearchItems(List<string> itemCodes = null)
		{
			if (!IsValidate()) return;
			List<string> searchlist = new List<string>();
			searchlist.Add(SearchItemCode);
			searchlist = (itemCodes == null) ? searchlist : itemCodes;
			var proxy = new wcf.P71WcfServiceClient();
			var result = RunWcfMethod<List<wcf.F1903>>(proxy.InnerChannel, () => proxy.GetP710108(searchlist.ToArray(), SelectedGup, SelectedCust).ToList());

			if (!result.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}

			var newItems = result.Select(x => new NameValuePair<string>() { Value = x.ITEM_CODEk__BackingField, Name = x.ITEM_NAMEk__BackingField }).ToList();
			var newCount = newItems.Count;

			// 排除已經存在的商品編號
			newItems = newItems.Where(x => !ItemList.Select(item => item.Value).Contains(x.Value)).ToList();

			if (newItems.Count != newCount)
			{
				DialogService.ShowMessage(Properties.Resources.P7101080000_ViewModel_ITEM_CODE_Duplicate);
			}

			ItemList = ItemList.Concat(newItems).OrderBy(x => x.Value).ToList();
			SelectedItem = ItemList.LastOrDefault();
			OnScrollIntoViewItems();
		}

		private bool IsValidate()
		{
			StringBuilder MsgSb = new StringBuilder();
			if (string.IsNullOrEmpty(SelectedDc))
				MsgSb.Append(Properties.Resources.P7101080000_ViewModel_DC_Required).Append(Environment.NewLine);
			if (string.IsNullOrEmpty(SelectedGup) && !GupList.Where(x => x.Name == Resources.Resources.All).Any())
				MsgSb.Append(Properties.Resources.P7101080000_ViewModel_GUP_Required).Append(Environment.NewLine);
			if (string.IsNullOrEmpty(SelectedCust) && CustList.Any())
				MsgSb.Append(Properties.Resources.P7101080000_ViewModel_CUST_Required).Append(Environment.NewLine);
			if (string.IsNullOrEmpty(SelectedWarehouse))
				MsgSb.Append(Properties.Resources.P7101080000_ViewModel_WarehouseID_Required).Append(Environment.NewLine);
			if (MsgSb.Length > 0)
			{
				DialogService.ShowMessage(MsgSb.ToString());
				return false;
			}
			return true;
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
				var proxy = GetProxy<F19Entities>();
				var result = Wms3plSession.Get<GlobalInfo>().GetCustDataList(SelectedDc, SelectedGup);
				result.Insert(0, new NameValuePair<string>() { Value = "0", Name = Resources.Resources.All });
				CustList = result.ToObservableCollection();
			}
			SelectedCust = CustList.FirstOrDefault().Value;
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
				var proxy = GetProxy<F19Entities>();
				var result = Wms3plSession.Get<GlobalInfo>().GetGupDataList(SelectedDc);
				result.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
				GupList = result.ToObservableCollection();
			}
			SelectedGup = GupList.FirstOrDefault().Value;
		}
		private void DoSearchWarehouseList()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1980s.Where(x => x.DC_CODE.Equals(SelectedDc))
			  .Select(x => new NameValuePair<string>() { Value = x.WAREHOUSE_ID, Name = x.WAREHOUSE_NAME })
			  .ToList();
			result.Insert(0, new NameValuePair<string>() { Value = "0", Name = Resources.Resources.All });
			WarehouseList = result.ToObservableCollection();
			SelectedWarehouse = WarehouseList.FirstOrDefault().Value;
		}

		private bool GetPrintLocList()
		{
			if (!IsValidate()) return false;
			var searchStartCondition = string.Empty;
			var searchEndCondition = string.Empty;
			var warehouseCondition = SelectedWarehouse;
			string listItemString = string.Empty;
			switch (ChoseOption)
			{
				case ChoseOptionType.LocCode:
					searchStartCondition = SearchStartCode;
					searchEndCondition = SearchEndCode.PadRight(9, '9');
					//if (string.IsNullOrEmpty(searchStartCondition) || string.IsNullOrEmpty(searchEndCondition))
					//{
					//	DialogService.ShowMessage(Properties.Resources.P7101080000_ViewModel_LocBeginEnd_Required);
					//	return false;
					//}
					break;
				case ChoseOptionType.ItemCode:
					listItemString = string.Join(",", ItemList.Select(o => string.Format("'{0}'", o.Value)));
					if (string.IsNullOrEmpty(listItemString))
					{
						DialogService.ShowMessage(Properties.Resources.P7101080000_ViewModel_ItemCode_Required);
						return false;
					}
					break;
				default:
					break;
			}


			var proxy = new wcf.P71WcfServiceClient();
			var result = RunWcfMethod<List<wcf.F1912DataLoc>>(proxy.InnerChannel, () => proxy.GetPrintDataLoc(searchStartCondition, searchEndCondition, SelectedGup,
			  SelectedDc, SelectedCust, warehouseCondition, listItemString, printEmpty
			  
			  ).ToList());
			if (!result.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return false;
			}
			LocList = result.ToList();

			if (selectPrinter == PrintBy.a4Printer && selectSizeSource.Value == "1")
			{
				var proxyNew = new wcf.P71WcfServiceClient();
				var resultNew = RunWcfMethod<List<wcf.F1912DataLocByLocType>>
					(proxyNew.InnerChannel,
					() => proxyNew.GetPrintDataLocByNewReport(searchStartCondition, searchEndCondition, SelectedGup,
				  SelectedDc, SelectedCust, warehouseCondition, listItemString,printEmpty).ToList());
				if (!resultNew.Any())
				{
					ShowMessage(Messages.InfoNoData);
					return false;
				}
				LocListR71010803 = resultNew.ToList();

			}


			return true;
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => SearchItems(), () => UserOperateMode == OperateMode.Query
				  );
			}
		}

		#endregion Search

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

		#region Past
		public ICommand PastCommand
		{
			get
			{
				return new RelayCommand(
				  () =>
				  {
					  IsBusy = true;
					  try
					  {
						  DoPast();
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
		private void DoPast()
		{
			//檢核必填欄位
			if (!IsValidate()) return;
			//執行複製貼上動作
			#region 基本檢核
			var list = new List<string>();
			var pastData = Clipboard.GetDataObject();
			if (pastData != null && pastData.GetDataPresent(DataFormats.Text))
			{
				var content = pastData.GetData(DataFormats.Text).ToString();
				var contArray = content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
				list.AddRange(contArray);
			}
			if (list.Count == 0)
			{
				DialogService.ShowMessage(Properties.Resources.P7101080000_ViewModel_ClipBoardEmpty);
				return;
			}
			#endregion
			SearchItems(list);
		}
		#endregion

		#region Remove
		public ICommand RemoveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoRemove(), () => UserOperateMode == OperateMode.Query
				  );
			}
		}
		private void DoRemove(bool isAll = false)
		{
			List<NameValuePair<string>> tmpList = new List<NameValuePair<string>>();
			//執行刪除動作
			if (!isAll)
				tmpList = ItemList.Where(o => !o.Equals(SelectedItem)).ToList();
			ItemList = tmpList;
		}
		#endregion Remove

		#region RemoveAll
		public ICommand RemoveAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoRemove(true), () => UserOperateMode == OperateMode.Query
				  );
			}
		}

		#endregion RemoveAll

		#region Print
		private ICommand _printCommand;

		public ICommand PrintCommand
		{
			get
			{
				PrintType printType = PrintType.Preview;
				bool canPrint = false;
				return (_printCommand ?? (_printCommand = CreateBusyAsyncCommand(
				o =>
				{
					printType = (PrintType)o;
					canPrint = GetPrintLocList();
				},
				() => SelectedF910501 != null,
				o =>
				{
					if (canPrint && ((selectPrinter == PrintBy.a4Printer && selectSizeSource.Value == "1") ? LocListR71010803.Any() : LocList.Any()))
					{
						ReportClass rc = DoPrintReport();
						if (rc != null)
						{
							DoPrint(printType, selectPrinter,rc);
						}
					}

				})));

			}
		}
		#endregion

		#region 新增列印規格單選

		private PrintBy _selectPrinter=PrintBy.labelPrinter;

		public PrintBy selectPrinter
		{
			get { return _selectPrinter; }
			set
			{
				Set(() => selectPrinter, ref _selectPrinter, value);
				GetSize(selectPrinter);
			}
		}

		#endregion
		#region GetSize
		public void GetSize(PrintBy selectPrinter)
		{
			string tmpTopic = string.Empty;
			switch (selectPrinter)
			{
				case PrintBy.a4Printer:
					tmpTopic = "A4";
					break;
				case PrintBy.bartender:
					tmpTopic = "bartender";
					break;
				case PrintBy.labelPrinter:
					tmpTopic = "label";
					break;
			}
			sizeSource = GetBaseTableService.GetF000904List(FunctionCode, "P1703030000", tmpTopic, false).Where(x => x.Value == "2").ToList();
			selectSizeSource = sizeSource.FirstOrDefault();
		}
		#endregion
		public ReportClass DoPrintReport()
		{
			ReportClass report = new ReportClass();
			switch (selectPrinter)
			{
				case PrintBy.a4Printer:
					switch (selectSizeSource.Value)
					{
						case "0":
							report = ReportHelper.CreateAndLoadReport<R71010801>();

							break;
						case "1":
							report = ReportHelper.CreateAndLoadReport<R71010803>();

							break;

                        case "2":
                            report = ReportHelper.CreateAndLoadReport<R71010806>();

                            break;

                        case "3":
                            report = ReportHelper.CreateAndLoadReport<R71010805>();

                            break;
                    }
					break;
				case PrintBy.bartender:
					switch (selectSizeSource.Value)
					{
						case "0":
							report = new ReportClass();
							var printObj = new LabelPrintHelper(FunctionCode);

							//標籤資訊
							var labelItems = LocList.Select(x => new LableItem
							{
								LableCode = "FETLB19",
								LableName = Properties.Resources.P7101080000_ViewModel_LocCode_Card,
								LocCode = x.LOC
							}).ToList();

							var result = printObj.DoPrint(labelItems, SelectedF910501.LABELING, 1);
							ShowResultMessage(result);
							report = null;
							break;
					}
					break;
				case PrintBy.labelPrinter:
					switch (selectSizeSource.Value)
					{
						case "0":
							report = ReportHelper.CreateAndLoadReport<R71010802>();
							break;
						case "1":
							report = ReportHelper.CreateAndLoadReport<R71010804>();
							break;
                        case "2":
                            report = ReportHelper.CreateAndLoadReport<R71010807>();
                            break;
                    }
					break;
			}
			return report;
		}
		#region 單一標籤尺寸下拉選單

		private List<NameValuePair<string>> _sizeSource;
		/// <summary>
		/// 標籤尺寸清單
		/// </summary>
		public List<NameValuePair<string>> sizeSource
		{
			get { return _sizeSource; }
			set
			{
				Set(() => sizeSource, ref _sizeSource, value);
				
			}
		}

		private NameValuePair<string> _selectSizeSource;
		/// <summary>
		/// 選標籤尺寸清單
		/// </summary>
		public NameValuePair<string> selectSizeSource
		{
			get { return _selectSizeSource; }
			set
			{
				Set(() => selectSizeSource, ref _selectSizeSource, value);

			}
		}

        #endregion

        public void PrintReport(ReportClass report, F910501 device, PrinterType printerType = PrinterType.A4)
        {
            CrystalReportService crystalReportService;
            if (device == null)
            {
                crystalReportService = new CrystalReportService(report);
                crystalReportService.ShowReport(null, PrintType.Preview);
            }
            else
            {
                crystalReportService = new CrystalReportService(report, device);
                crystalReportService.PrintToPrinter(printerType);
            }
        }
    }

}
