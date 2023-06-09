using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P14ExDataService;
using System.Collections.ObjectModel;
using ImportInventorySerial = Wms3pl.WpfClient.ExDataServices.P14ExDataService.ImportInventorySerial;
using wcf = Wms3pl.WpfClient.ExDataServices.P14WcfService;
using p14Ex = Wms3pl.WpfClient.ExDataServices.P14ExDataService;
using System.Windows;

namespace Wms3pl.WpfClient.P14.ViewModel
{
	public partial class P1401030000_ViewModel : InputViewModelBase
	{
		public P1401030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				DcList = GetDcList();
				if (DcList != null)
					SelectedDc = DcList.First().Value;
				StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F140106", "STATUS");
				QueryInventorySDate = DateTime.Today;
				QueryInventoryEDate = DateTime.Today;
				QueryInventoryNo = string.Empty;
				QueryProcWmsNo = string.Empty;
				QueryItemCode = string.Empty;
				//SearchCommand.Execute(null);
				CheckToolList = GetBaseTableService.GetF000904List(FunctionCode, "F1980", "DEVICE_TYPE", true);
				QueryCheckTool = CheckToolList.FirstOrDefault().Value;
        InventoryTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F140101", "INVENTORY_TYPE");
      }
		}

		#region 參數

		private string _gupCode;
		public string _custCode;
		public Action PreImport = delegate { };
        public Action ExcelImport = delegate { };

		private List<wcf.ImportInventorySerial> _tempImportInventorySerialList;

		#region 自動倉顯示
		private Visibility _automaticVisibility = Visibility.Collapsed;
		public Visibility AutomaticVisibility { get { return _automaticVisibility; } set { _automaticVisibility = value; RaisePropertyChanged("AutomaticVisibility"); } }
		#endregion

		#region 人工倉顯示
		private Visibility _artificialVisibility = Visibility.Collapsed;
		public Visibility ArtificialVisibility { get { return _artificialVisibility; } set { _artificialVisibility = value; RaisePropertyChanged("ArtificialVisibility"); } }
		#endregion

		#region DC 參數
		//物流中心List
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

		//Query Dc Value
		private string _selectedDc;
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				_selectedDc = value;
				RaisePropertyChanged("SelectedDc");
			}
		}


		#endregion

		#region 盤點日期-起
		private DateTime? _queryInventorySDate;

		public DateTime? QueryInventorySDate
		{
			get { return _queryInventorySDate; }
			set
			{
				if (_queryInventorySDate == value)
					return;
				Set(() => QueryInventorySDate, ref _queryInventorySDate, value);
			}
		}
		#endregion

		#region 盤點日期-迄
		private DateTime? _queryInventoryEDate;

		public DateTime? QueryInventoryEDate
		{
			get { return _queryInventoryEDate; }
			set
			{
				if (_queryInventoryEDate == value)
					return;
				Set(() => QueryInventoryEDate, ref _queryInventoryEDate, value);
			}
		}
		#endregion

		#region 盤點工具
		private List<NameValuePair<string>> _checkToolList;
		public List<NameValuePair<string>> CheckToolList
		{
			get { return _checkToolList; }
			set { Set(ref _checkToolList, value); }
		}
		private string _queryCheckTool;

		public string QueryCheckTool
		{
			get { return _queryCheckTool; }
			set
			{
				if (_queryCheckTool == value)
					return;
				Set(() => QueryCheckTool, ref _queryCheckTool, value);
			}
		}
		#endregion

		#region 盤點單號
		private string _queryInventoryNo;

		public string QueryInventoryNo
		{
			get { return _queryInventoryNo; }
			set
			{
				if (_queryInventoryNo == value)
					return;
				Set(() => QueryInventoryNo, ref _queryInventoryNo, value);
			}
		}
		#endregion

		#region 處理單號
		private string _queryProcWmsNo;

		public string QueryProcWmsNo
		{
			get { return _queryProcWmsNo; }
			set
			{
				if (_queryProcWmsNo == value)
					return;
				Set(() => QueryProcWmsNo, ref _queryProcWmsNo, value);
			}
		}
		#endregion

		#region 品號
		private string _queryItemCode;

		public string QueryItemCode
		{
			get { return _queryItemCode; }
			set
			{
				if (_queryItemCode == value)
					return;
				Set(() => QueryItemCode, ref _queryItemCode, value);
			}
		}
		#endregion

		private List<NameValuePair<string>> _statusList;

		#region 調整狀態清單
		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				_statusList = value;
				RaisePropertyChanged("StatusList");
			}
		}
		#endregion

		#region 匯入檔案路徑參數

		private string _importFilePath;

        public string ImportFilePath
        {
            get { return _importFilePath; }
            set
            {
                _importFilePath = value;
                RaisePropertyChanged("ImportFilePath");
            }
        }
        #endregion

        #region 查詢結果 F140106QueryList 參數

        private ObservableCollection<F140106QueryData> _f140106QueryList;

		public ObservableCollection<F140106QueryData> F140106QueryList
		{
			get { return _f140106QueryList; }
			set
			{
				_f140106QueryList = value;
				RaisePropertyChanged("F140106QueryList");
			}
		}
		#endregion

		#region 查詢結果_盤點單產生單據
		private List<InventoryDoc> _inventoryDoc;
		public List<InventoryDoc> InventoryDoc
		{
			get { return _inventoryDoc; }
			set { Set(ref _inventoryDoc, value); }
		}
		#endregion

		#region Select Data F140106
		private F140106QueryData _selectF14016Data;
		public F140106QueryData SelectF140106Data
		{
			get { return _selectF14016Data; }
			set
			{
				_selectF14016Data = value;
				if (value != null)
				{
					_tempImportInventorySerialList = null;
					BundleSerialloc = CanImport();
					GetInventoryDetailData(_selectF14016Data.INVENTORY_NO);
					GetInventoryDoc(_selectF14016Data.INVENTORY_NO);
				}
				else
				{
					InventoryDeatilList = null;
					InventoryDoc = null;
				}

				if (SelectF140106Data != null)
				{
					if (SelectF140106Data.IS_AUTOMATIC == "0")
					{
						AutomaticVisibility = Visibility.Collapsed;
						ArtificialVisibility = Visibility.Visible;
					}
					else
					{
						AutomaticVisibility = Visibility.Visible;
						ArtificialVisibility = Visibility.Collapsed;
					}
				}
				
				RaisePropertyChanged("SelectF140106Data");
			}
		}
		#endregion

		private List<p14Ex.F1913Data> _inventoryDeatilList;
		public List<p14Ex.F1913Data> InventoryDeatilList
		{
			get { return _inventoryDeatilList; }
			set { Set(ref _inventoryDeatilList, value); }
		}

		private bool _bundleSerialloc;
		public bool BundleSerialloc
		{
			get { return _bundleSerialloc; }
			set { Set(ref _bundleSerialloc, value); }
		}

    #region 盤點類別
    private List<NameValuePair<string>> _inventoryTypeList;
    /// <summary>
    /// 盤點類別
    /// </summary>
    public List<NameValuePair<string>> InventoryTypeList
    {
      get { return _inventoryTypeList; }
      set
      {
        _inventoryTypeList = value;
        RaisePropertyChanged("InventoryTypeList");
      }
    }
    #endregion 盤點類別

    #endregion

    #region Function

    #region 取物流中心資料

    public List<NameValuePair<string>> GetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			return data;
		}
		#endregion

		public void GetInventoryDetailData(string inventoryNo)
		{
			var proxy = GetExProxy<P14ExDataSource>();
			InventoryDeatilList = proxy.CreateQuery<p14Ex.F1913Data>("GetInventoryDetailData")
									.AddQueryExOption("dcCode", _selectedDc)
									.AddQueryExOption("gupCode", _gupCode)
									.AddQueryExOption("custCode", _custCode)
									.AddQueryExOption("inventoryNo", inventoryNo).ToList();
		}

		public void GetInventoryDoc(string inventoryNo)
		{
			var proxy = GetExProxy<P14ExDataSource>();
			InventoryDoc = proxy.CreateQuery<InventoryDoc>("GetInventoryDoc")
									.AddQueryExOption("dcCode", _selectedDc)
									.AddQueryExOption("gupCode", _gupCode)
									.AddQueryExOption("custCode", _custCode)
									.AddQueryExOption("inventoryNo", inventoryNo).ToList();
			
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
			var proxyEx = GetExProxy<P14ExDataSource>();
			var f140106QueryData = proxyEx.CreateQuery<F140106QueryData>("GetF140106QueryData")
					.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
					.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
					.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
					.AddQueryOption("inventoryDateS",
						string.Format("'{0}'", ((QueryInventorySDate.HasValue) ? QueryInventorySDate.Value.ToString("yyyy/MM/dd") : "")))
					.AddQueryOption("inventoryDateE",
						string.Format("'{0}'", ((QueryInventoryEDate.HasValue) ? QueryInventoryEDate.Value.ToString("yyyy/MM/dd") : "")))
					.AddQueryOption("inventoryNo", string.Format("'{0}'", QueryInventoryNo))
					.AddQueryOption("procWmsNo", string.Format("'{0}'", QueryProcWmsNo))
					.AddQueryOption("itemCode", string.Format("'{0}'", QueryItemCode))
					.AddQueryOption("checkTool", string.Format("'{0}'", QueryCheckTool));

			F140106QueryList = f140106QueryData.ToObservableCollection();

			if (F140106QueryList == null || F140106QueryList.Count() == 0)
			{
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
					o => DoSave(),
					() =>
					{
						return UserOperateMode == OperateMode.Query &&
							   SelectF140106Data != null &&
							   !string.IsNullOrEmpty(SelectF140106Data.INVENTORY_NO) &&
							   (!BundleSerialloc || _tempImportInventorySerialList != null);
					});
			}
		}

		private void DoSave()
		{
			var proxyWcf = new wcf.P14WcfServiceClient();
			var wcfInventroySerialList = _tempImportInventorySerialList ?? new List<wcf.ImportInventorySerial>();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
				() => proxyWcf.UpdateF140101PostingStauts(SelectF140106Data.DC_CODE, SelectF140106Data.GUP_CODE,
					SelectF140106Data.CUST_CODE, SelectF140106Data.INVENTORY_NO, wcfInventroySerialList.ToArray()));

			ShowResultMessage(result);
			UserOperateMode = OperateMode.Query;
			SearchCommand.Execute(null);
		}
		#endregion Save

		#region 匯入序號
		public ICommand ImportCommand
		{
			get
			{
                return CreateBusyAsyncCommand(
                    o => DoImportCommand.Execute(null), () => UserOperateMode == OperateMode.Query
                    && SelectF140106Data != null && BundleSerialloc, null, null);
            }
		}

        public ICommand DoImportCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DispatcherAction(() =>
                    {
                        ExcelImport();
                        DoImport();
                    });
                });
            }
        }

        public void DoImport()
		{
			if (!string.IsNullOrEmpty(ImportFilePath))
			{
				var errorMeg = string.Empty;
				var excelTable = DataTableHelper.ReadExcelDataTable(ImportFilePath, ref errorMeg, 0);
				if (!string.IsNullOrEmpty(errorMeg))
				{
                    ImportFilePath = null;
					ShowWarningMessage(errorMeg);
					return;
				}
				var list = new List<ImportInventorySerial>();
				if (excelTable.Columns.Count == 0 || excelTable.Rows.Count == 0)
				{
                    ImportFilePath = null;
					ShowWarningMessage(Properties.Resources.P1401010000_ImportEmptyExcel);
					return;
				}
				if (excelTable.Columns.Count != 3)
				{
                    ImportFilePath = null;
					ShowWarningMessage(Properties.Resources.P1401010000_ImportErrorExcel);
					return;
				}
				int index = 0;
				foreach (DataRow dataRow in excelTable.Rows)
				{
					index++;
					list.Add(new ImportInventorySerial
					{
						ROWNUM = index,
						LOC_CODE = dataRow[0].ToString(),
						ITEM_CODE = dataRow[1].ToString(),
						SERIAL_NO = dataRow[2].ToString()
					});
				}
				var proxyWcf = new wcf.P14WcfServiceClient();
				var wcfInventroySerialList = ExDataMapper.MapCollection<ImportInventorySerial, wcf.ImportInventorySerial>(list);
				var result = RunWcfMethod<wcf.ImportInventorySerial[]>(proxyWcf.InnerChannel,
					() => proxyWcf.CheckImorImportInventorySerial(SelectF140106Data.DC_CODE, SelectF140106Data.GUP_CODE,
										SelectF140106Data.CUST_CODE, SelectF140106Data.INVENTORY_NO, wcfInventroySerialList.ToArray())).ToList();
				if (result.Any())
				{
					if (!result.First().IsSuccessk__BackingField)
					{
						_tempImportInventorySerialList = null;
						ShowWarningMessage(result.First().Messagek__BackingField);
					}
					else
					{
						_tempImportInventorySerialList = result;
						ShowMessage(Messages.InfoImportSuccess);
					}
				}
				else
					ShowWarningMessage(Properties.Resources.P1401010000_ImportEmptyExcel);
			}
		}
		#endregion
		private bool CanImport()
		{
			if (SelectF140106Data != null)
			{
				var proxyEx = GetExProxy<P14ExDataSource>();
				var data = proxyEx.CreateQuery<ExDataServices.P14ExDataService.F1913Data>("GetF140106QueryDetailData")
						.AddQueryOption("dcCode", string.Format("'{0}'", SelectF140106Data.DC_CODE))
						.AddQueryOption("gupCode", string.Format("'{0}'", SelectF140106Data.GUP_CODE))
						.AddQueryOption("custCode", string.Format("'{0}'", SelectF140106Data.CUST_CODE))
						.AddQueryOption("inventoryNo", string.Format("'{0}'", SelectF140106Data.INVENTORY_NO)).ToList();
				return data.Any(o => o.BUNDLE_SERIALLOC == "1");
			}
			return false;
		}
	}
}
