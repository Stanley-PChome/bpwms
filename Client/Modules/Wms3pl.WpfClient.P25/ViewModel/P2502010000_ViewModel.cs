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
using Wms3pl.WpfClient.Common.Helpers;

namespace Wms3pl.WpfClient.P25.ViewModel
{
	public enum SplitItem
	{
		Item,
		System,
		Serial
	}
	public partial class P2502010000_ViewModel : InputViewModelBase
	{
		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		public P2502010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				QueryData = new F2501();
				//初始化執行時所需的值及資料
				//DcList = GetDcList();								//設定DC					
				StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F2501", "STATUS", true);   //設定序號狀態			
                                                                                                  //TypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TYPE", true);     //設定序號狀態
        var proxy00 = GetProxy<F00Entities>();
        var ordPropList = proxy00.F000903s.Where(
        o => o.ORD_PROP.Equals("A1") || o.ORD_PROP.Equals("A2") || o.ORD_PROP.Equals("J1")
          || o.ORD_PROP.Equals("O3") || o.ORD_PROP.Equals("T1") )
           .Select(x => new NameValuePair<string>(x.ORD_PROP_NAME, x.ORD_PROP))
        .ToList();
        ordPropList.Insert(0, new NameValuePair<string> { Name = "全部", Value = "" });
        OpTypeList = ordPropList;


        //OpTypeList = GetF000904List(FunctionCode, "F1903", "TYPE", true);			//設定序號狀態
        OpItemType = OpTypeList.Select(x => x.Value).FirstOrDefault();
        QueryData.DC_CODE = "";								//預設下拉全部
				QueryData.STATUS = "";								//預設下拉全部				
				ItemTypeQ = "";										//預設下拉全部	
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

    #region 作業類別參數
    private List<NameValuePair<string>> _optypeList;
    /// <summary>
    /// 作業類別
    /// </summary>
		public List<NameValuePair<string>> OpTypeList
    {
      get { return _optypeList; }
      set
      {
        _optypeList = value;
        RaisePropertyChanged("OpTypeList");
      }
    }
    #endregion 作業類別參數

    private string _opitemType = string.Empty;
    /// <summary>
    /// 作業類別
    /// </summary>
    public string OpItemType
    {
      get { return _opitemType; }
      set
      {
        Set(() => OpItemType, ref _opitemType, value);
      }
    }
    #region 序號類號參數
    //  private List<NameValuePair<string>> _typeList;
    //public List<NameValuePair<string>> TypeList
    //{
    //	get { return _typeList; }
    //	set
    //	{
    //		_typeList = value;
    //		RaisePropertyChanged("TypeList");
    //	}
    //}
    #endregion

    #region 日期查詢 -建立/新增 參數

    private string _crtSDate;
		public string CrtSDate
		{
			get { return _crtSDate; }
			set
			{
				_crtSDate = value;
				RaisePropertyChanged("CrtSDate");
			}
		}

		private string _crtEDate;
		public string CrtEDate
		{
			get { return _crtEDate; }
			set
			{
				_crtEDate = value;
				RaisePropertyChanged("CrtEDate");
			}
		}

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

		#region ItemType 參數
		private string _itemTypeQ;
		public string ItemTypeQ
		{
			get { return _itemTypeQ; }
			set
			{
				_itemTypeQ = value;
				RaisePropertyChanged("ItemTypeQ");
			}
		}
		#endregion

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

		#region 查詢條件Class參數
		private F2501 _queryData;
		public F2501 QueryData
		{
			get { return _queryData; }
			set
			{
				_queryData = value;
				RaisePropertyChanged("QueryData");
			}
		}
		#endregion

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

		#region GV查詢 DGList
		private ObservableCollection<F2501QueryData> _dgQueryDataF2501;
		public ObservableCollection<F2501QueryData> DgQueryDataF2501
		{
			get { return _dgQueryDataF2501; }
			set
			{
				_dgQueryDataF2501 = value;
				RaisePropertyChanged("DgQueryDataF2501");
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
			if (DgQueryDataF2501 == null || DgQueryDataF2501.Count == 0)
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
					FileName = Properties.Resources.P2502010000_ViewModel_SerialFileSearch + DateTime.Now.ToString("yyyy-MM-dd"),
					Title = Properties.Resources.P2502010000_ViewModel_SelectStoragePath
				};

				if ((bool)openFileDialog.ShowDialog())
				{
					var f = new FileInfo(openFileDialog.FileName);
					openFileDialog.FileName = f.Directory + "/" + f.Name;
					SaveToCSV(DgQueryDataF2501, openFileDialog.FileName);
				}
			}
		}

		public void SaveToCSV(IList<F2501QueryData> ary, string FilePath)
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
                    data = title;
                    data += "\n";
                    foreach (var elem in ary)
					{
						//日期轉換
						var converVALID_DATE = elem.VALID_DATE == null ? "" : ((DateTime)elem.VALID_DATE).ToString("yyyy/MM/dd");
						var converInDate = elem.IN_DATE == null ? "" : ((DateTime)elem.IN_DATE).ToString("yyyy/MM/dd HH:mm");
						var converCrtDate = elem.CRT_DATE == null ? "" : ((DateTime)elem.CRT_DATE).ToString("yyyy/MM/dd HH:mm");
						var converUpdDate = elem.UPD_DATE == null ? "" : ((DateTime)elem.UPD_DATE).ToString("yyyy/MM/dd HH:mm");

						data += elem.GUP_NAME + "," + elem.CUST_NAME + "," + elem.ITEM_CODE + "," + elem.ITEM_NAME + "," + elem.ITEM_SPEC;
						data += "," + elem.STATUS_NAME + "," + elem.SERIAL_NO;
						data += "," + converVALID_DATE;

						data += "," + elem.PO_NO + "," + elem.WMS_NO + "," + converInDate + "," + elem.ORD_PROP_NAME;
						data += "," + elem.ACTIVATED + "," + elem.PROCESS_NO;
						data += "," + elem.VNR_NAME + "," + elem.CLIENT_IP + "," + elem.ITEM_UNIT;
						data += "," + converCrtDate + "," + elem.CRT_NAME + "," + converUpdDate + "," + elem.UPD_NAME;

						data += "\n";
                    }
                    wr.Write(data);
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

			if (string.IsNullOrEmpty(QueryData.ITEM_CODE) && string.IsNullOrEmpty(QueryData.BOX_SERIAL)
				&& string.IsNullOrEmpty(QueryData.SERIAL_NO) && string.IsNullOrEmpty(QueryData.CELL_NUM)
				&& string.IsNullOrEmpty(QueryData.WMS_NO) && string.IsNullOrEmpty(QueryData.PO_NO)
				&& string.IsNullOrEmpty(QueryData.BATCH_NO))
			{
				DialogService.ShowMessage(Properties.Resources.P2502010000_ViewModel_SearchCondition_Required);
			}
			else
			{
        long? _combinNo = QueryData.COMBIN_NO == null ? 0 : QueryData.COMBIN_NO;
        var wcfproxy = GetWcfProxy<wcf.P25WcfServiceClient>();
        //var callRecvVedio = wcfproxy.RunWcfMethod(w => w.CallRecvVedio(dcCode, gupCode, custCode, stockNo, ItemNos?.ToArray()));
        var f2501QueryData = wcfproxy.RunWcfMethod(w => w.Get2501QueryData(_gupCode, _custCode
          , string.Format("{0}", StringHelper.JoinSplitDistinct(QueryData.ITEM_CODE, ","))
          , string.Format("{0}", QueryData.BOX_SERIAL)
          , string.Format("{0}", QueryData.BATCH_NO)
          , string.Format("{0}", StringHelper.JoinSplitDistinct(QueryData.SERIAL_NO, ","))
          , string.Format("{0}", QueryData.CELL_NUM)
          , string.Format("{0}", QueryData.PO_NO)
          , string.Format("{0}", StringHelper.JoinSplitDistinct(QueryData.WMS_NO, ","))
          , string.Format("{0}", QueryData.STATUS)
          , string.Format("{0}", string.IsNullOrEmpty(OpItemType) ? null : OpItemType)
          , string.Format("{0}", QueryData.RETAIL_CODE)
          , short.Parse(_combinNo.ToString())
          , string.Format("{0}", QueryData.CRT_NAME)
         , CrtSDate
         , CrtEDate
         , UpdSDate
          , UpdEDate));
        //,
        //, custCode, stockNo, ItemNos?.ToArray()));


        //var proxyEx = GetExProxy<P25ExDataSource>();
        //var f52501QueryData = proxyEx.CreateQuery<F2501QueryData>("Get2501QueryData")
        //	.AddQueryExOption("gupCode", _gupCode)
        //	.AddQueryExOption("custCode", _custCode)
        //	.AddQueryOption("itemCode", string.Format("'{0}'", StringHelper.JoinSplitDistinct(QueryData.ITEM_CODE, ",")))
        //	.AddQueryOption("boxSerial", string.Format("'{0}'", QueryData.BOX_SERIAL))
        //	.AddQueryOption("batchNo", string.Format("'{0}'", QueryData.BATCH_NO))
        //	.AddQueryOption("serialNo", string.Format("'{0}'", StringHelper.JoinSplitDistinct(QueryData.SERIAL_NO, ",")))
        //	.AddQueryOption("cellNum", string.Format("'{0}'", QueryData.CELL_NUM))
        //	.AddQueryOption("poNo", string.Format("'{0}'", QueryData.PO_NO))
        //	.AddQueryOption("wmsNo", string.Format("'{0}'", StringHelper.JoinSplitDistinct(QueryData.WMS_NO, ",")))
        //	.AddQueryOption("status", string.Format("'{0}'", QueryData.STATUS))
        //	.AddQueryOption("itemType", string.Format("'{0}'", ItemTypeQ))
        //	.AddQueryOption("retailCode", string.Format("'{0}'", QueryData.RETAIL_CODE))
        //	.AddQueryOption("combinNo", QueryData.COMBIN_NO == null ? 0 : QueryData.COMBIN_NO)
        //	.AddQueryOption("crtName", string.Format("'{0}'", QueryData.CRT_NAME))
        //	.AddQueryOption("crtSDate", string.Format("'{0}'", CrtSDate))
        //	.AddQueryOption("crtEDate", string.Format("'{0}'", CrtEDate))
        //	.AddQueryOption("updSDate", string.Format("'{0}'", UpdSDate))
        //	.AddQueryOption("updEDate", string.Format("'{0}'", UpdEDate));

        //DgQueryDataF2501 = f52501QueryData.ToObservableCollection();
        DgQueryDataF2501 = ExDataMapper.MapCollection<wcf.F2501QueryData,F2501QueryData>(f2501QueryData).ToObservableCollection();

        if ((DgQueryDataF2501 == null || !DgQueryDataF2501.Any()))
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
