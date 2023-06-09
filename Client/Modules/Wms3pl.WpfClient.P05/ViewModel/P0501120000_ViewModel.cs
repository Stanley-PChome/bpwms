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
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.Datas.F19;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F05DataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P05WcfService;
using Microsoft.Win32;
using System.IO;
using System.Data;
using Wms3pl.WpfClient.DataServices.F00DataService;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public class P0501120000_ViewModel : InputViewModelBase
	{
		public P0501120000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				
				Init();
			}

		}

		#region Property
		public Action OpenAGVStationSetting = delegate { };

		#region 業主編號
		private string _gupCode;

		public string GupCode
		{
			get { return _gupCode; }
			set
			{
				Set(() => GupCode, ref _gupCode, value);
			}
		}
		#endregion


		#region 貨主編號
		private string _custCode;

		public string CustCode
		{
			get { return _custCode; }
			set
			{
				Set(() => CustCode, ref _custCode, value);
			}
		}
		#endregion



		#region Query

		#region 是否顯示查詢條件
		private bool _isSearchExpanded;

		public bool IsSearchExpanded
		{
			get { return _isSearchExpanded; }
			set
			{
				Set(() => IsSearchExpanded, ref _isSearchExpanded, value);
			}
		}
		#endregion


		#region 是否顯示查詢結果
		private bool _isSearchResultExpanded;

		public bool IsSearchResultExpanded
		{
			get { return _isSearchResultExpanded; }
			set
			{
				Set(() => IsSearchResultExpanded, ref _isSearchResultExpanded, value);
			}
		}
		#endregion

		

		#region 物流中心清單
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				Set(() => DcList, ref _dcList, value);
			}
		}
		#endregion

		#region 選取物流中心編號
		private string _selectedDcCode;

		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				Set(() => SelectedDcCode, ref _selectedDcCode, value);
				if (value != null)
				{
					SetCapsMaxRetailQty(value);
				}
			}
		}
		#endregion

		#region 彙總日期(起)
		private DateTime _batchDateS;

		public DateTime BatchDateS
		{
			get { return _batchDateS; }
			set
			{
				Set(() => BatchDateS, ref _batchDateS, value);
			}
		}
		#endregion

		#region 彙總日期(迄)
		private DateTime _batchDateE;

		public DateTime BatchDateE
		{
			get { return _batchDateE; }
			set
			{
				Set(() => BatchDateE, ref _batchDateE, value);
			}
		}
		#endregion

		#region 彙總批號
		private string _batchNo;

		public string BatchNo
		{
			get { return _batchNo; }
			set
			{
				Set(() => BatchNo, ref _batchNo, value);
			}
		}
		#endregion

		#region 揀貨狀態清單
		private List<NameValuePair<string>> _pickStatusList;

		public List<NameValuePair<string>> PickStatusList
		{
			get { return _pickStatusList; }
			set
			{
				Set(() => PickStatusList, ref _pickStatusList, value);
			}
		}
		#endregion

		#region 選取的揀貨狀態
		private string _selectedPickStatus;

		public string SelectedPickStatus
		{
			get { return _selectedPickStatus; }
			set
			{
				Set(() => SelectedPickStatus, ref _selectedPickStatus, value);
				if (value == "9")
					SelectedPutStatus = "9";
			}
		}
		#endregion

		#region 播種狀態清單
		private List<NameValuePair<string>> _putStatusList;

		public List<NameValuePair<string>> PutStatusList
		{
			get { return _putStatusList; }
			set
			{
				Set(() => PutStatusList, ref _putStatusList, value);
			}
		}
		#endregion

		#region 選取的播種狀態
		private string _selectedPutStatus;

		public string SelectedPutStatus
		{
			get { return _selectedPutStatus; }
			set
			{
				Set(() => SelectedPutStatus, ref _selectedPutStatus, value);
			}
		}
		#endregion

		#region 揀貨彙總清單
		private ObservableCollection<P050112Batch> _dgList;

		public ObservableCollection<P050112Batch> DgList
		{
			get { return _dgList; }
			set
			{
				Set(() => DgList, ref _dgList, value);
			}
		}
		#endregion


		#region 選取的揀貨彙總
		private P050112Batch _selectedDgItem;

		public P050112Batch SelectedDgItem
		{
			get { return _selectedDgItem; }
			set
			{
				Set(() => SelectedDgItem, ref _selectedDgItem, value);
			}
		}
		#endregion


		#endregion

		#region Add Query

		#region 選取物流中心編號
		private string _selectedAddDcCode;

		public string SelectedAddDcCode
		{
			get { return _selectedAddDcCode; }
			set
			{
				Set(() => SelectedAddDcCode, ref _selectedAddDcCode, value);
				DispatcherAction(() =>
				{
					SetCapsMaxRetailQty(value);
					SetAreaList();
					SetPickToolList();
				});
				
			}
		}
		#endregion


		#region 批次日期(起)
		private DateTime _delvDateS;

		public DateTime DelvDateS
		{
			get { return _delvDateS; }
			set
			{
				Set(() => DelvDateS, ref _delvDateS, value);
			}
		}
		#endregion

		#region 批次日期(迄)
		private DateTime _delvDateE;

		public DateTime DelvDateE
		{
			get { return _delvDateE; }
			set
			{
				Set(() => DelvDateE, ref _delvDateE, value);
			}
		}
		#endregion


		#region 揀貨工具清單
		private List<NameValuePair<string>> _pickToolList;

		public List<NameValuePair<string>> PickToolList
		{
			get { return _pickToolList; }
			set
			{
				Set(() => PickToolList, ref _pickToolList, value);
			}
		}
		#endregion


		#region  選取的揀貨工具
		private string _selectedPickTool;

		public string SelectedPickTool
		{
			get { return _selectedPickTool; }
			set
			{
				Set(() => SelectedPickTool, ref _selectedPickTool, value);
				DgPickList = null;
				SelectedDgPickItem = null;
			}
		}
		#endregion


		#region 儲區清單
		private List<NameValuePair<string>> _areaList;

		public List<NameValuePair<string>> AreaList
		{
			get { return _areaList; }
			set
			{
				Set(() => AreaList, ref _areaList, value);
			}
		}
		#endregion

		#region 選取儲區
		private string _selectedAreaCode;

		public string SelectedAreaCode
		{
			get { return _selectedAreaCode; }
			set
			{
				Set(() => SelectedAreaCode, ref _selectedAreaCode, value);
			}
		}
		#endregion


		#region 分配份數
		private short _allotCnt;

		public short AllotCnt
		{
			get { return _allotCnt; }
			set
			{
				Set(() => AllotCnt, ref _allotCnt, value);
			}
		}
		#endregion

		#region 是否全選

		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				CheckSelectedAll(_isCheckAll);
				RaisePropertyChanged("IsCheckAll");
			}
		}
		#endregion


		#region 分配方式清單
		private List<NameValuePair<string>> _allotTypeList;

		public List<NameValuePair<string>> AllotTypeList
		{
			get { return _allotTypeList; }
			set
			{
				Set(() => AllotTypeList, ref _allotTypeList, value);
			}
		}
		#endregion


		#region 選取的分配方式
		private string _selectedAllotType;

		public string SelectedAllotType
		{
			get { return _selectedAllotType; }
			set
			{
				Set(() => SelectedAllotType, ref _selectedAllotType, value);
			}
		}
		#endregion


		#region 揀貨清單
		private SelectionList<P050112Pick> _dgPickList;

		public SelectionList<P050112Pick> DgPickList
		{
			get { return _dgPickList; }
			set
			{
				Set(() => DgPickList, ref _dgPickList, value);
			}
		}
		#endregion


		#region 選取的揀貨清單
		private SelectionItem<P050112Pick> _selectedDgPickItem;

		public SelectionItem<P050112Pick> SelectedDgPickItem
		{
			get { return _selectedDgPickItem; }
			set
			{
				Set(() => SelectedDgPickItem, ref _selectedDgPickItem, value);
			}
		}
		#endregion


		#region 最大門市數
		private int _maxCapsRetailCnt;

		public int MaxCapsRetailCnt
		{
			get { return _maxCapsRetailCnt; }
			set
			{
				Set(() => MaxCapsRetailCnt, ref _maxCapsRetailCnt, value);
			}
		}
		#endregion



		#endregion

		#endregion

		#region Method

		private void Init()
		{
			IsSearchExpanded = true;
			IsSearchResultExpanded = true;
			BatchDateS = DateTime.Today;
			BatchDateE = DateTime.Today;
			GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if(DcList.Any())
			SelectedDcCode = DcList.First().Value;
			PickStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F0515", "PICK_STATUS", true);
			if(PickStatusList.Any())
				SelectedPickStatus = PickStatusList.First().Value;
			PutStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F0515", "PUT_STATUS", true);
			if (PutStatusList.Any())
				SelectedPutStatus = PutStatusList.First().Value;
			
			AllotTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F0515", "ALLOT_TYPE").OrderByDescending(x=> x.Value).ToList();
		}
		private void SetCapsMaxRetailQty(string dcCode)
		{
			var proxy = GetProxy<F00Entities>();
			var item = proxy.F0003s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == GupCode && x.CUST_CODE == CustCode && x.AP_NAME == "Caps_Max_RetailQty").ToList().FirstOrDefault();
			if (item != null)
			{
				MaxCapsRetailCnt = int.Parse(item.SYS_PATH);
			}
			else
			{
				MaxCapsRetailCnt = 0;
				ShowWarningMessage(Properties.Resources.P0501120000_ViewModel_UnSetCapsMaxRetailQty);
			}
			
		}
		/// <summary>
		/// 設定揀貨工具清單
		/// </summary>
		private void SetPickToolList()
		{
			var pickToolDatas = GetBaseTableService.GetF000904List(FunctionCode, "F191902", "PICK_TOOL");
			var proxy = GetProxy<F19Entities>();
			var custPickTools = proxy.F191902s.Where(x => x.DC_CODE == SelectedAddDcCode && x.GUP_CODE == GupCode && x.CUST_CODE == CustCode).ToList().Select(x => x.PICK_TOOL).Distinct().ToList();
			PickToolList = pickToolDatas.Where(x => custPickTools.Contains(x.Value)).ToList();
			if (PickToolList.Any())
				SelectedPickTool = PickToolList.First().Value;
		}

		/// <summary>
		/// 設定儲區清單
		/// </summary>
		private void SetAreaList()
		{
			if (SelectedAddDcCode != null)
			{
				var proxy = GetProxy<F19Entities>();
				var areaDatas = proxy.CreateQuery<F1919>("GetDatasByCanToShip")
					.AddQueryExOption("dcCode",SelectedAddDcCode)
					.AddQueryExOption("gupCode",GupCode)
					.AddQueryExOption("custCode",CustCode)
					.ToList().Select(x => new NameValuePair<string> { Name = x.AREA_NAME, Value = x.AREA_CODE }).ToList();
				areaDatas.Insert(0, new NameValuePair<string> { Name = "全部", Value = "" });
				AreaList = areaDatas;
				DispatcherAction(() =>
				{
					if (AreaList.Any())
						SelectedAreaCode = AreaList.First().Value;
				});
			
			}
			else
			{
				AreaList = new List<NameValuePair<string>>();
				SelectedAreaCode = null;
			}

		}

		/// <summary>
		/// 全選/取消全選揀貨單
		/// </summary>
		/// <param name="isCheckAll"></param>
		private void CheckSelectedAll(bool isCheckAll)
		{
			if(DgPickList!=null)
			{
				foreach (var item in DgPickList)
					item.IsSelected = isCheckAll;
			}
		}

		private void Refresh()
		{
			DispatcherAction(() =>
			{
				var batchNo = SelectedDgItem.BATCH_NO;
				DoSearch();
				SelectedDgItem = DgList.FirstOrDefault(x => x.BATCH_NO == batchNo);
			});
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o =>
					{
						IsSearchResultExpanded = true;
						if (DgList!=null && DgList.Any())
							SelectedDgItem = DgList.First();
						else
						{
							SelectedDgItem = null;
							ShowMessage(Messages.InfoNoData);
						}
					}
					);
			}
		}

		private void DoSearch()
		{
			var proxy = GetExProxy<P05ExDataSource>();
			DgList = proxy.CreateQuery<P050112Batch>("GetP050112Batches")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("batchDateS", BatchDateS.ToString("yyyy/MM/dd"))
				.AddQueryExOption("batchDateE", BatchDateE.ToString("yyyy/MM/dd"))
				.AddQueryExOption("batchNo", BatchNo)
				.AddQueryExOption("pickStatus", SelectedPickStatus)
				.AddQueryExOption("putStatus", SelectedPutStatus)
				.ToObservableCollection();
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
			AllotCnt = 4;
			DelvDateS = DateTime.Today;
			DelvDateE = DateTime.Today;
			if(DcList.Any())
				SelectedAddDcCode = DcList.First().Value;
			if (AllotTypeList.Any())
				SelectedAllotType = AllotTypeList.First().Value;
			DgPickList = null;
			IsCheckAll = false;
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
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedDgItem != null && SelectedDgItem.PICK_STATUS == "0"
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (ShowConfirmMessage(string.Format(Properties.Resources.P0501120000_BeforeDeleteConfirm,SelectedDgItem.BATCH_NO)) == UILib.Services.DialogResponse.No)
				return;

			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.DeleteBatchPickData(SelectedDgItem.DC_CODE, SelectedDgItem.GUP_CODE, SelectedDgItem.CUST_CODE, SelectedDgItem.BATCH_NO));
			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);
			else
			{
				ShowMessage(Messages.DeleteSuccess);
				DispatcherAction(() =>
				{
					SearchCommand.Execute(null);
				});
			}
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				var isOk = false;
				return CreateBusyAsyncCommand(
					o => isOk = DoSave(), () => UserOperateMode != OperateMode.Query && DgPickList!=null,
					o => {
						if (isOk)
						{
							UserOperateMode = OperateMode.Query;
							SelectedPickStatus = "";
							SelectedPutStatus = "";
							BatchNo = "";
							BatchDateS = DateTime.Today;
							BatchDateE = DateTime.Today;
							SelectedDcCode = SelectedAddDcCode;
							SearchCommand.Execute(null);
						}
					}
					);
			}
		}

		private bool DoSave()
		{
			if (!BeforeSaveCheck())
				return false;

			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var createBatchPick = new wcf.CreateBatchPick
			{
				DcCode = SelectedAddDcCode,
				GupCode = GupCode,
				CustCode = CustCode,
				AllotCnt = AllotCnt,
				AllotType = SelectedAllotType,
				PickTool = SelectedPickTool,
				PickOrdNos = DgPickList.Where(x => x.IsSelected).Select(x => x.Item.PICK_ORD_NO).ToArray()
			};
			var result = proxy.RunWcfMethod(w => w.CreateBatchPickData(createBatchPick));
			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);
			else
				ShowInfoMessage(result.Message);
			return result.IsSuccessed;
		}

		private bool BeforeSaveCheck()
		{
			var pickOrdNos = DgPickList.Where(x => x.IsSelected).Select(x => x.Item.PICK_ORD_NO);
			if (!pickOrdNos.Any())
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage(Properties.Resources.P0501120000_PleaseCheckedPickOrders);
				});
				return false;
			}
			if (AllotCnt <= 0)
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage(Properties.Resources.P0501120000_AllotCntMustBetterThenZero);
				});
				return false;
			}
			if (string.IsNullOrWhiteSpace(SelectedAllotType))
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage(Properties.Resources.P0501120000_PleaseChooseAllotType);
				});
				return false;
			}
			var agvStations = GetBaseTableService.GetF000904List(FunctionCode, "P081201", "Workstation");
			if (SelectedPickTool == "4" && AllotCnt > agvStations.Count)
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage(string.Format(Properties.Resources.P0501120000_AllotCntMustLessAgvStation, agvStations.Count));
				});
				return false;
			}
			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var retailCnt = proxy.RunWcfMethod(w => w.GetP050112PickRetailCount(SelectedAddDcCode,GupCode,CustCode, pickOrdNos.ToArray()));
			if(SelectedPickTool == "4" && retailCnt > MaxCapsRetailCnt)
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage(string.Format(Properties.Resources.P0501120000_OverCapsUsedQtyWithAgv, retailCnt,MaxCapsRetailCnt));
				});
				return false;
			}
			if(SelectedPickTool !="4" && retailCnt >  MaxCapsRetailCnt)
			{
				if (ShowConfirmMessage(string.Format(Properties.Resources.P0501120000_OverCapsUsedQtyWithOther, retailCnt, MaxCapsRetailCnt)) == UILib.Services.DialogResponse.No)
					return false;
			}
			return true;
		}

		#endregion Save

		#region SearchPick 揀貨單查詢
		/// <summary>
		/// Gets the SearchPick.
		/// </summary>
		public ICommand SearchPickCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearchPick(), () => UserOperateMode != OperateMode.Query,
						o =>
						{
							if (DgPickList != null && DgPickList.Any())
								SelectedDgPickItem = DgPickList.First();
							else
							{
								SelectedDgPickItem = null;
								ShowMessage(Messages.InfoNoData);
							}
						}
);
			}
		}

		public void DoSearchPick()
		{
			var proxy = GetExProxy<P05ExDataSource>();
			DgPickList = proxy.CreateQuery<P050112Pick>("GetP050112PickDatas")
				.AddQueryExOption("dcCode", SelectedAddDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("delvDateS", DelvDateS.ToString("yyyy/MM/dd"))
				.AddQueryExOption("delvDateE", DelvDateE.ToString("yyyy/MM/dd"))
				.AddQueryExOption("pickTool", SelectedPickTool)
				.AddQueryExOption("areaCode", SelectedAreaCode).ToList().ToSelectionList();

		}
		#endregion SearchPick


		#region AGVStartup AGV啟動
		/// <summary>
		/// Gets the AGVStartup.
		/// </summary>
		public ICommand AGVStartupCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAGVStartup(), () => UserOperateMode == OperateMode.Query && SelectedDgItem!=null && SelectedDgItem.PICK_STATUS == "0" && SelectedDgItem.PICK_TOOL =="4"
);
			}
		}

		public void DoAGVStartup()
		{
			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.AGVStartupPick(SelectedDgItem.DC_CODE, SelectedDgItem.GUP_CODE, SelectedDgItem.CUST_CODE, SelectedDgItem.BATCH_NO));
			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);
			else
			{
				ShowMessage(Messages.Success);
				Refresh();
			}
		}
		#endregion AGVStartup


		#region AGVStationSetting AGV 設定
		/// <summary>
		/// Gets the AGVStationSetting.
		/// </summary>
		public ICommand AGVStationSettingCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAGVStationSetting(), () => UserOperateMode == OperateMode.Query && SelectedDgItem != null  && SelectedDgItem.PICK_TOOL == "4",
						o =>
						{
							OpenAGVStationSetting();
						}
);
			}
		}

		public void DoAGVStationSetting()
		{

		}
		#endregion AGVStationSetting


		#region ArtificalPick 人工揀貨
		/// <summary>
		/// Gets the PrintBatchPickOrder.
		/// </summary>
		public ICommand ArtificalPickCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoArtificalPick(), () => UserOperateMode == OperateMode.Query && SelectedDgItem != null && SelectedDgItem.PICK_STATUS != "9" && SelectedDgItem.PUT_STATUS != "3" && SelectedDgItem.PUT_STATUS!="9"
);
			}
		}

		public void DoArtificalPick()
		{
			if(SelectedDgItem.PICK_TOOL == "4")
			{
				if (ShowConfirmMessage(Properties.Resources.P0501120000_ConfirmPickToolFromAGVToPickOrder) == UILib.Services.DialogResponse.No)
					return;
			}
			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ArtificalPick(SelectedDgItem.DC_CODE, SelectedDgItem.GUP_CODE, SelectedDgItem.CUST_CODE, SelectedDgItem.BATCH_NO));
			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);
			else
			{
				ShowMessage(Messages.Success);
				DispatcherAction(() =>
				{
					ExportBatchPickOrderCommand.Execute(null);
					Refresh();
				});
			}
		}
		#endregion ArtificalPick


		#region ExportBatchPickOrder
		/// <summary>
		/// Gets the PrintBatchPickOrder.
		/// </summary>
		public ICommand ExportBatchPickOrderCommand
		{
			get
			{
				List<PickReportData> pickReportDatas = null;
				return CreateBusyAsyncCommand(
						o => pickReportDatas = DoExportBatchPickOrder(), () => UserOperateMode == OperateMode.Query,
						o => DoExportBatchPickOrderComplete(pickReportDatas)
);
			}
		}

		public List<PickReportData> DoExportBatchPickOrder()
		{
			var proxy = GetExProxy<P05ExDataSource>();
			var datas = proxy.CreateQuery<PickReportData>("GetPickReportDatas")
				.AddQueryExOption("dcCode", SelectedDgItem.DC_CODE)
				.AddQueryExOption("gupCode", SelectedDgItem.GUP_CODE)
				.AddQueryExOption("custCode", SelectedDgItem.CUST_CODE)
				.AddQueryExOption("batchNo", SelectedDgItem.BATCH_NO).ToList();
			return datas;
		}
		private void DoExportBatchPickOrderComplete(List<PickReportData> putReportDatas)
		{
			if (putReportDatas == null || !putReportDatas.Any())
			{
				ShowInfoMessage(Properties.Resources.P0501120000_NoDataCanExport);
				return;
			}

			var saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".xls",
				Filter = "excel files |*.xls",
				FileName = SelectedDgItem.BATCH_NO + "_Pick.xls"
			};

			if (saveFileDialog.ShowDialog() == false)
				return;
			var fileInfo = new FileInfo(saveFileDialog.FileName);
			if (fileInfo.Exists && fileInfo.IsLocked())
			{
				ShowWarningMessage(Properties.Resources.P0501120000_FileIsLocked);
				return;
			}
			var titles = new Dictionary<string, string>
						{
							{"ROWNUM", Properties.Resources.P0501120000_ROWNUM},
							{"AREA_CODE",Properties.Resources.P0501120000_AreaCode},
							{"AREA_NAME", Properties.Resources.P0501120000_AreaName},
							{"SHELF_NO", Properties.Resources.P0501120000_ShelfNo},
							{"LOC_CODE", Properties.Resources.P0501120000_LOC_CODE},
							{"ITEM_CODE", Properties.Resources.P0501120000_ITEM_CODE},
							{"ITEM_NAME",Properties.Resources.P0501120000_ITEM_NAME},
							{"B_PICK_QTY", Properties.Resources.P0501120000_BPickQty},
							{"A_PICK_QTY", Properties.Resources.P0501120000_APickQty},

						};
			var dt = putReportDatas.ToDataTable(titles);
			dt.Columns.RemoveAt(0);
			dt.RenderDataTableToExcel(saveFileDialog.FileName);
			ShowInfoMessage(string.Format(Properties.Resources.P0501120000_ExportFileTo, saveFileDialog.FileName));

		}
		#endregion ExportBatchPickOrder



		#region DownToCaps Caps下傳
		/// <summary>
		/// Gets the DownToCaps.
		/// </summary>
		public ICommand DownToCapsCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDownToCaps(), () => UserOperateMode == OperateMode.Query && SelectedDgItem!=null &&  (SelectedDgItem.PICK_STATUS == "1" || SelectedDgItem.PICK_STATUS == "2") && SelectedDgItem.PUT_STATUS == "0"
);
			}
		}

		public void DoDownToCaps()
		{
			if(SelectedDgItem.RETAIL_CNT > MaxCapsRetailCnt)
			{
				DispatcherAction(() =>
				{
					ShowWarningMessage(string.Format(Properties.Resources.P0501120000_OverCapsUsedQty, SelectedDgItem.RETAIL_CNT, MaxCapsRetailCnt));
				});
				return;
			}
			ExecSow("5");
		}
		#endregion DownToCaps

		#region ArtificalSow 人工播種
		/// <summary>
		/// Gets the ArtificalSow.
		/// </summary>
		public ICommand ArtificalSowCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => ExecSow("1"), () => UserOperateMode == OperateMode.Query && SelectedDgItem != null && (SelectedDgItem.PICK_STATUS == "1" || SelectedDgItem.PICK_STATUS == "2") && (SelectedDgItem.PUT_STATUS == "0" || (SelectedDgItem.PUT_STATUS == "1" && SelectedDgItem.PUT_TOOL =="1"))
);
			}
		}


		#endregion ArtificalSow

		#region 播種下傳
		private void ExecSow(string putTool)
		{
			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ExecSow(SelectedDgItem.DC_CODE, SelectedDgItem.GUP_CODE, SelectedDgItem.CUST_CODE, SelectedDgItem.BATCH_NO, putTool));
			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);
			else
			{
				if (putTool == "5")
				{
					var result2 = proxy.RunWcfMethod(w => w.ExecSowToCaps(SelectedDgItem.DC_CODE, SelectedDgItem.GUP_CODE, SelectedDgItem.CUST_CODE, SelectedDgItem.BATCH_NO, putTool));
					if (!result2.IsSuccessed)
						ShowWarningMessage(result2.Message);
					else
					{
						ShowInfoMessage(result2.Message);
						Refresh();
					}
				}
				else
				{
					ShowInfoMessage(result.Message);
					DispatcherAction(() =>
					{
						ExportBatchSowOrderCommand.Execute(null);
						Refresh();
					});

				}
				
			}
		}

		#endregion

		#region ExportBatchSowOrder 匯出播種單

		/// <summary>
		/// Gets the PrintBatchSowOrder.
		/// </summary>
		public ICommand ExportBatchSowOrderCommand
		{
			get
			{
				List<PutReportData> putReportDatas = null;
				return CreateBusyAsyncCommand(
						o => putReportDatas = DoExportBatchSowOrder(),
						() => UserOperateMode == OperateMode.Query ,
						o => DoExportBatchSowOrderComplete(putReportDatas)
);
			}
		}

		public List<PutReportData> DoExportBatchSowOrder()
		{
			var proxy = GetExProxy<P05ExDataSource>();
			var datas = proxy.CreateQuery<PutReportData>("GetPutReportDatas")
				.AddQueryExOption("dcCode", SelectedDgItem.DC_CODE)
				.AddQueryExOption("gupCode", SelectedDgItem.GUP_CODE)
				.AddQueryExOption("custCode", SelectedDgItem.CUST_CODE)
				.AddQueryExOption("batchNo", SelectedDgItem.BATCH_NO).ToList();
			return datas;
		}

		private void DoExportBatchSowOrderComplete(List<PutReportData> putReportDatas)
		{
			if (putReportDatas == null || !putReportDatas.Any())
			{
				ShowInfoMessage(Properties.Resources.P0501120000_NoDataCanExport);
				return;
			}

			var saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".xls",
				Filter = "excel files |*.xls",
				FileName = SelectedDgItem.BATCH_NO + "_Put.xls"
			};

			if (saveFileDialog.ShowDialog() == false)
				return;
			var fileInfo = new FileInfo(saveFileDialog.FileName);
			if (fileInfo.Exists && fileInfo.IsLocked())
			{
				ShowWarningMessage(Properties.Resources.P0501120000_FileIsLocked);
				return;
			}
			var titles = new Dictionary<string, string>
						{
							{"ROWNUM", Properties.Resources.P0501120000_ROWNUM},
							{"LOC_CODE", Properties.Resources.P0501120000_LOC_CODE},
							{"ITEM_CODE", Properties.Resources.P0501120000_ITEM_CODE},
							{"ITEM_NAME",Properties.Resources.P0501120000_ITEM_NAME},
							{"RETAIL_CODE",Properties.Resources.P0501120000_RETAIL_CODE},
							{"RETAIL_NAME", Properties.Resources.P0501120000_RETAIL_NAME},
							{"PLAN_QTY", Properties.Resources.P0501120000_PLAN_QTY},
							{"ACT_QTY", Properties.Resources.P0501120000_ACT_QTY},

						};
			var dt = putReportDatas.ToDataTable(titles);
			dt.Columns.RemoveAt(0);
			dt.RenderDataTableToExcel(saveFileDialog.FileName);
			ShowInfoMessage(string.Format(Properties.Resources.P0501120000_ExportFileTo, saveFileDialog.FileName));

		}
		#endregion PrintBatchSowOrder

		#region CapsReturn Caps回傳
		/// <summary>
		/// Gets the CapsReturn.
		/// </summary>
		public ICommand CapsReturnCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCapsReturn(), () => UserOperateMode == OperateMode.Query && SelectedDgItem != null && SelectedDgItem.PUT_STATUS == "1" && SelectedDgItem.PUT_TOOL == "5"
);
			}
		}

		public void DoCapsReturn()
		{
			var proxyF05 = GetProxy<F05Entities>();
			var f051501s = proxyF05.F051501s.Where(o => o.DC_CODE == SelectedDgItem.DC_CODE && o.GUP_CODE == SelectedDgItem.GUP_CODE && o.CUST_CODE == SelectedDgItem.CUST_CODE && o.BATCH_NO == SelectedDgItem.BATCH_NO).ToList();

			if (f051501s.Any(o => o.STATUS != "3"))
			{
				ShowWarningMessage(string.Format(Properties.Resources.P050112_CapsRrturnMessage, SelectedDgItem.BATCH_NO, f051501s.FirstOrDefault(o => o.STATUS != "3").BATCH_PICK_NO));
				return;
			}
			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.ExecCapsReturn(SelectedDgItem.DC_CODE, SelectedDgItem.GUP_CODE, SelectedDgItem.CUST_CODE, SelectedDgItem.BATCH_NO));
			ShowResultMessage(result.IsSuccessed, result.Message);
			if (result.IsSuccessed)
			{
				var f051602s = proxyF05.F051602s.Where(x => x.DC_CODE == SelectedDgItem.DC_CODE && x.GUP_CODE == SelectedDgItem.GUP_CODE && x.CUST_CODE == SelectedDgItem.CUST_CODE && x.BATCH_NO == SelectedDgItem.BATCH_NO && x.PACK_QTY > x.PLAN_QTY).ToList();
				DispatcherAction(() =>
				{
					if (f051602s.Any())
					{
						ShowWarningMessage(string.Format(Properties.Resources.P0501120000_CapsReturnDataError, f051602s.Count));

						var saveFileDialog = new SaveFileDialog
						{
							DefaultExt = ".xls",
							Filter = "excel files |*.xls",
							FileName = SelectedDgItem.BATCH_NO + "_Caps_Error.xls"
						};

						if (saveFileDialog.ShowDialog() == false)
							return;
						var fileInfo = new FileInfo(saveFileDialog.FileName);
						if (fileInfo.Exists && fileInfo.IsLocked())
						{
							ShowWarningMessage(Properties.Resources.P0501120000_FileIsLocked);
							return;
						}
						var titles = new Dictionary<string, string>
						{
							{"BATCH_NO", Properties.Resources.P0501120000_BatchNo},
							{"RETAIL_CODE",Properties.Resources.P0501120000_RETAIL_CODE},
							{"RETAIL_NAME", Properties.Resources.P0501120000_RETAIL_NAME},
							{"LOC_CODE", Properties.Resources.P0501120000_LOC_CODE},
							{"ITEM_CODE", Properties.Resources.P0501120000_ITEM_CODE},
							{"ITEM_NAME",Properties.Resources.P0501120000_ITEM_NAME},
							{"WMS_ORD_NO",Properties.Resources.WmsOrdNo},
							{"PLAN_QTY", Properties.Resources.P0501120000_PLAN_QTY},
							{"PACK_QTY", Properties.Resources.P0501120000_ACT_QTY},
						};
						var dt = f051602s.ToDataTable(titles);
						dt.RenderDataTableToExcel(saveFileDialog.FileName);
						ShowInfoMessage(string.Format(Properties.Resources.P0501120000_ExportFileTo, saveFileDialog.FileName));
					}
					Refresh();
				});
			}
		}
		#endregion CapsReturn

		#region ArtificalSowReturn 人工播種回傳
		/// <summary>
		/// Gets the ArtificalSowReturn.
		/// </summary>
		public ICommand ArtificalSowReturnCommand
		{
			get
			{
				var filePath = string.Empty;
				return CreateBusyAsyncCommand(
						o => DoArtificalSowReturn(filePath), () => UserOperateMode == OperateMode.Query && SelectedDgItem != null && SelectedDgItem.PUT_STATUS == "1" && SelectedDgItem.PUT_TOOL != "5",
						o => { },
						null,
						() =>
						{
							var openFileDailog = new OpenFileDialog()
							{
								DefaultExt = ".xls",
								Filter = "excel files |*.xls",
							};
							if (openFileDailog.ShowDialog() == true)
							{
								var ex = openFileDailog.SafeFileName.Split('.');

								//防止*.*的判斷式
								if (ex[ex.Length - 1] != "xls" && ex[ex.Length - 1] != "xlsx")
								{
									filePath = string.Empty;
									ShowWarningMessage(Properties.Resources.P0501120000_ImportMustExcel);
								}
								else
									filePath = openFileDailog.FileName;
							}
						}

);
			}
		}

		public void DoArtificalSowReturn(string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
			{
				return;
			}
			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(filePath, ref errorMeg);
			if (excelTable == null)
			{
				ShowWarningMessage(Properties.Resources.PleaseCheckFileIsOpenOrCorrect);
				return;
			}
			else if (excelTable.Columns.Count != 7)
			{
				ShowWarningMessage(string.Format(Properties.Resources.ExcelFileMustEqualColLength,7));
				return;
			}
			else
			{
				foreach(DataRow row in excelTable.Rows)
				{
					var num = 0;
					if(row[5] == null || !int.TryParse(row[5].ToString(),out num))
					{
						ShowWarningMessage(Properties.Resources.P0501120000_PlanQtyMustNumber);
						return;
					}
					if (row[6] == null || !int.TryParse(row[6].ToString(), out num))
					{
						ShowWarningMessage(Properties.Resources.P0501120000_ActQtyMustNumber);
						return;
					}
				}
				var datas = (from item in excelTable.AsEnumerable().Select((r, i) => new { Row = r, Index = i })
									 select new wcf.PutReportData
									 {
										 ROWNUM = item.Index,
										 LOC_CODE = item.Row[0].ToString(),
										 ITEM_CODE = item.Row[1].ToString(),
										 ITEM_NAME = item.Row[2].ToString(),
										 RETAIL_CODE = item.Row[3].ToString(),
										 RETAIL_NAME = item.Row[4].ToString(),
										 PLAN_QTY = int.Parse(item.Row[5].ToString()),
										 ACT_QTY = int.Parse(item.Row[6].ToString())
									 }).ToArray();
				var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
				var result = proxy.RunWcfMethod(w => w.ImportArtificalSowReturn(SelectedDgItem.DC_CODE,SelectedDgItem.GUP_CODE,SelectedDgItem.CUST_CODE,SelectedDgItem.BATCH_NO, datas));
				ShowResultMessage(result.IsSuccessed, result.Message);
				if(result.IsSuccessed)
				{
					Refresh();
				}
			}
		}
		#endregion ArtificalSowReturn

	}
}
