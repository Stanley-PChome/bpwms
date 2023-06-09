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
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P25ExDataService;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices.P01ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P25WcfService;
using System.Security.Permissions;
using System.Windows;
using System.IO;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.P25.ViewModel
{


	public partial class P2502020000_ViewModel : InputViewModelBase
	{
		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		public P2502020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				//初始化執行時所需的值及資料
				//DcList = GetDcList();								//設定DC
				StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F2501", "STATUS", true);		//設定序號狀態
				TypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TYPE", true);			//設定序號狀態
				ItemType = TypeList.Select(x => x.Value).FirstOrDefault();
				QueryData = new F250101();
				QueryData.STATUS = "";								//預設下拉全部													
				IsShowExport = "False";								//預設匯出按鈕 IsEnable=False		

			}
		}

		#region 讀取 物流中心
		public List<NameValuePair<string>> GetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;

			return data;
		}



		#endregion

		#region 序號狀態參數
		private List<NameValuePair<string>> _statusList;
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

		#region 序號類號參數
		private List<NameValuePair<string>> _typeList;
		public List<NameValuePair<string>> TypeList
		{
			get { return _typeList; }
			set
			{
				_typeList = value;
				RaisePropertyChanged("TypeList");
			}
		}
		#endregion

		#region 日期查詢 -建立/新增 參數

		private string _updSDate;
		public string UpdSDate
		{
			get { return _updSDate; }
			set
			{
				_updSDate = value;
				RaisePropertyChanged("UpdSDate");
			}
		}

		private string _updEDate;
		public string UpdEDate
		{
			get { return _updEDate; }
			set
			{
				_updEDate = value;
				RaisePropertyChanged("UpdEDate");
			}
		}
		#endregion

		private string _itemType = string.Empty;

		public string ItemType
		{
			get { return _itemType; }
			set
			{
				Set(() => ItemType, ref _itemType, value);
			}
		}

		private List<NameValuePair<string>> _itemUnitList;

		public List<NameValuePair<string>> ItemUnitList
		{
			get
			{
				return _itemUnitList ?? (_itemUnitList = GetItemUnitList());
			}
			set
			{
				Set(() => ItemUnitList, ref _itemUnitList, value);
			}
		}

		List<NameValuePair<string>> GetItemUnitList()
		{
			return GetProxy<F91Entities>().F91000302s.Where(x => x.ITEM_TYPE_ID == "001")
													 .Select(x => new NameValuePair<string>(x.ACC_UNIT_NAME, x.ACC_UNIT))
													 .ToList();
		}

		#region 匯出 Button 顯示控制參數
		//是否有上傳檔案
		private string _isShowExport;
		public string IsShowExport
		{
			get { return _isShowExport; }
			set
			{
				_isShowExport = value;
				RaisePropertyChanged("IsShowExport");
			}
		}
		#endregion

		#region 查詢條件Class參數
		private F250101 _queryData;
		public F250101 QueryData
		{
			get { return _queryData; }
			set
			{
				_queryData = value;
				RaisePropertyChanged("QueryData");
			}
		}
		#endregion

		#region GV查詢 DGList
		private ObservableCollection<P2502QueryData> _dgQueryDataF250101;
		public ObservableCollection<P2502QueryData> DgQueryDataF250101
		{
			get { return _dgQueryDataF250101; }
			set
			{
				_dgQueryDataF250101 = value;
				RaisePropertyChanged("DgQueryDataF250101");
			}
		}
		#endregion


		#region Function
		public void AppendItemCode(F1903 f1903)
		{

			if (string.IsNullOrEmpty(QueryData.ITEM_CODE))
			{
				QueryData.ITEM_CODE = f1903.ITEM_CODE;
				return;
			}
			var itemCodeList = StringHelper.SplitDistinct(QueryData.ITEM_CODE, ",".ToArray()).ToList();
			itemCodeList.Add(f1903.ITEM_CODE);
			QueryData.ITEM_CODE = string.Join(",", itemCodeList);

		}


		#endregion

		#region ExportExcelCommand
		public ICommand ExportExcelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoExport(), () => UserOperateMode == OperateMode.Query,
					o => DoExportData()
				);
			}
		}
		private void DoExport()
		{
		}

		private void DoExportData()
		{
			//取得Excel資料
			if (DgQueryDataF250101 == null || DgQueryDataF250101.Count == 0)
			{
				DialogService.ShowException(Properties.Resources.P2502010000_ViewModel_NoDataExport);
			}
			else
			{

				var openFileDialog = new Microsoft.Win32.SaveFileDialog()
				{
					DefaultExt = "csv",
					AddExtension = true,
					Filter = "Save Files (*.csv)|*.csv",
					OverwritePrompt = true,
					FileName = Properties.Resources.P2502020000_ViewModel_SerialModifySearch + DateTime.Now.ToString("yyyy-MM-dd"),
					Title = Properties.Resources.P2502010000_ViewModel_SelectStoragePath
				};

				if ((bool)openFileDialog.ShowDialog())
				{
					var f = new FileInfo(openFileDialog.FileName);
					openFileDialog.FileName = f.Directory + "/" + f.Name;
					SaveToCSV(DgQueryDataF250101, openFileDialog.FileName);
				}
			}
		}

		public void SaveToCSV(IList<P2502QueryData> ary, string FilePath)
		{
			try
			{
				string data = "";
				string title = "";
				title += Properties.Resources.P2502010000_ViewModel_Title1;
				title += Properties.Resources.P2502010000_ViewModel_Title2;
				title += Properties.Resources.P2502010000_ViewModel_Title3;
				title += Properties.Resources.P2502010000_ViewModel_Title4;
				title += Properties.Resources.P2502010000_ViewModel_Title5;
				title += Properties.Resources.P2502010000_ViewModel_Title6;
				title += Properties.Resources.P2502010000_ViewModel_Title7;

				using (StreamWriter wr = new StreamWriter(FilePath, false, System.Text.Encoding.GetEncoding(950)))	// big5
				{
					Type elemType = ary.GetType().GetElementType();
					StringBuilder sb = new StringBuilder();
					wr.Write(title);
					foreach (var elem in ary)
					{
						//日期轉換
						var converVALID_DATE = elem.VALID_DATE == null ? "" : ((DateTime)elem.VALID_DATE).ToString("yyyy/MM/dd");
						var converInDate = elem.IN_DATE == null ? "" : ((DateTime)elem.IN_DATE).ToString("yyyy/MM/dd HH:mm");
						var converCrtDate = elem.CRT_DATE == null ? "" : ((DateTime)elem.CRT_DATE).ToString("yyyy/MM/dd HH:mm");
						var converUpdDate = elem.UPD_DATE == null ? "" : ((DateTime)elem.UPD_DATE).ToString("yyyy/MM/dd HH:mm");
						var statusName = StatusList.Where(x => x.Value == elem.STATUS).Select(x => x.Name).FirstOrDefault() ?? string.Empty;
						var itemType = TypeList.Where(x => x.Value == elem.TYPE).Select(x => x.Name).FirstOrDefault() ?? string.Empty;

						data = elem.GUP_NAME + "," + elem.CUST_NAME + "," + elem.ITEM_CODE + "," + elem.ITEM_NAME + "," + elem.ITEM_SPEC;
						data += "," + statusName + "," + elem.SERIAL_NO + "," + itemType ;
						data += "," + converVALID_DATE;

						data += "," + elem.PO_NO + "," + elem.WMS_NO + "," + converInDate + "," + elem.ORD_PROP_NAME;
						data += "," + elem.RETAIL_CODE + "," + elem.ACTIVATED + "," + elem.PROCESS_NO ;
						data += "," + elem.VNR_NAME + "," + elem.SYS_NAME + "," + elem.CAMERA_NO + "," + elem.CLIENT_IP + "," + elem.ITEM_UNIT;
						data += "," + elem.SEND_CUST + "," + converCrtDate + "," + elem.CRT_NAME + "," + converUpdDate + "," + elem.UPD_NAME;

						data += "\n";
						wr.Write(data);
					}
					ShowMessage(Messages.InfoExportSuccess);
				}
			}
			catch (Exception ex)
			{
				if (ex.HResult == -2147024864)
				{
					DialogService.ShowException(Properties.Resources.P2502010000_ViewModel_CheckIsOpenedFile);
				}

			}
		}

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => DoSearchCompleted()
					);
			}
		}
		private void DoSearch()
		{
			ExDataMapper.Trim(QueryData);

			if (string.IsNullOrEmpty(QueryData.ITEM_CODE) && string.IsNullOrEmpty(QueryData.SERIAL_NO)
					&& string.IsNullOrEmpty(QueryData.CELL_NUM) && string.IsNullOrEmpty(QueryData.WMS_NO)
					&& string.IsNullOrEmpty(QueryData.PO_NO) && string.IsNullOrEmpty(QueryData.BATCH_NO)
					&& string.IsNullOrEmpty(QueryData.BOX_SERIAL))
			{
				DialogService.ShowMessage(Properties.Resources.P2502020000_ViewModel_SearchCondition_Required);
			}
			else
			{
				var proxyEx = GetExProxy<P25ExDataSource>();
				var f5250101QueryData = proxyEx.CreateQuery<P2502QueryData>("GetP2502QueryDatas")
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryOption("itemCode", string.Format("'{0}'", StringHelper.JoinSplitDistinct(QueryData.ITEM_CODE, ",")))
					.AddQueryOption("serialNo", string.Format("'{0}'", StringHelper.JoinSplitDistinct(QueryData.SERIAL_NO, ",")))
					.AddQueryOption("batchNo", string.Format("'{0}'", QueryData.BATCH_NO))
					.AddQueryOption("cellNum", string.Format("'{0}'", QueryData.CELL_NUM))
					.AddQueryOption("poNo", string.Format("'{0}'", QueryData.PO_NO))
					.AddQueryOption("wmsNo", string.Format("'{0}'", StringHelper.JoinSplitDistinct(QueryData.WMS_NO, ",")))
					.AddQueryOption("status", string.Format("'{0}'", QueryData.STATUS))
					.AddQueryOption("retailCode", string.Format("'{0}'", QueryData.RETAIL_CODE))
					.AddQueryOption("combinNo", QueryData.COMBIN_NO == null ? 0 : QueryData.COMBIN_NO)
					.AddQueryOption("crtName", string.Format("'{0}'", QueryData.CRT_NAME))
					.AddQueryOption("updSDate", string.Format("'{0}'", UpdSDate))
					.AddQueryOption("updEDate", string.Format("'{0}'", UpdEDate))
					.AddQueryExOption("boxSerial", QueryData.BOX_SERIAL)
					.AddQueryExOption("itemType", ItemType);

				DgQueryDataF250101 = f5250101QueryData.ToObservableCollection();

				if ((DgQueryDataF250101 == null || !DgQueryDataF250101.Any()))
				{
					IsShowExport = "False";
					ShowMessage(Messages.InfoNoData);
				}
				else
				{
					IsShowExport = "True";
				}
			}
		}
		private void DoSearchCompleted()
		{


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

		#region Paste
		public ICommand PasteCommand
		{


			get
			{
				return new RelayCommand<SplitItem>(
				  (t) =>
				  {
					  IsBusy = true;
					  try
					  {
						  DoPaste(t);
					  }
					  catch (Exception ex)
					  {
						  Exception = ex;
						  IsBusy = false;
					  }
					  IsBusy = false;
				  },
				(t) => !IsBusy);
			}
		}

		private void DoPaste(SplitItem splitItem)
		{

			if (Clipboard.ContainsData(DataFormats.Text))
			{
				var pastData = Clipboard.GetDataObject();
				if (pastData != null && pastData.GetDataPresent(DataFormats.Text))
				{
					var content = pastData.GetData(DataFormats.Text).ToString();
					var arr = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

					if (SplitItem.Item == splitItem)
					{
						QueryData.ITEM_CODE = string.Join(",", arr.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)));
					}
					else if (SplitItem.System == splitItem)
					{
						QueryData.WMS_NO = string.Join(",", arr.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)));
					}
					else if (SplitItem.Serial == splitItem)
					{
						QueryData.SERIAL_NO = string.Join(",", arr.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)));
					}

				}
			}


		}



		#endregion Paste
	}
}
