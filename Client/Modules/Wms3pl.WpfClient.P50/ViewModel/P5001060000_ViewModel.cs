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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F50DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P50ExDataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P50WcfService;
using wcf91 = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using Wms3pl.WpfClient.UILib.Services;


namespace Wms3pl.WpfClient.P50.ViewModel
{
	public partial class P5001060000_ViewModel : InputViewModelBase
	{
		public Action DoUpLoad = delegate { };
		public Action ReUploadBtn = delegate { };
		public P5001060000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				DcList = GetDcList();
				DcListEdit = DcList;
				InTaxList = GetBaseTableService.GetF000904List(FunctionCode, "F199003", "IN_TAX");
				DelvAccList = GetBaseTableService.GetF000904List(FunctionCode, "F199006", "DELV_ACC_TYPE");
				StatusListQuery = GetBaseTableService.GetF000904List(FunctionCode, "P500102", "STATUS", true);
				StatusQuery = StatusListQuery.First().Value;
				AccUnitList = GetAccUnit();
				EnableSDataQuery = DateTime.Today;
				EnableEDataQuery = DateTime.Today;
				if (DcListEdit != null)
				{
					DcEdit = DcListEdit.First().Value;
					DcQuery = DcListEdit.First().Value;
				}
			}

		}

		#region 參數

		private string _gupCode;
		private string _custCode;

		#region DC / GUP / CUST 設定
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
		//Edit Dc List
		private List<NameValuePair<string>> _dcListEdit;
		public List<NameValuePair<string>> DcListEdit
		{
			get { return _dcListEdit; }
			set
			{
				_dcListEdit = value;
				RaisePropertyChanged("DcListEdit");
			}
		}
		//Edit Dc Item
		private NameValuePair<string> _dcItemEdit;
		public NameValuePair<string> DcItemEdit
		{
			get { return _dcItemEdit; }
			set
			{
				_dcItemEdit = value;
				RaisePropertyChanged("DcItemEdit");
			}
		}
		//Edit Dc Value
		private string _dcEdit;
		public string DcEdit
		{
			get { return _dcEdit; }
			set
			{
				_dcEdit = value;
				if (value != null)
					AccItemListEdit = GetAccItemData(value);
				RaisePropertyChanged("DcEdit");
			}
		}

		//Query Dc Value
		private string _dcQuery;
		public string DcQuery
		{
			get { return _dcQuery; }
			set
			{
				_dcQuery = value;
				if (value != null)
					AccItemListSelect = GetAccItemData(value);
				RaisePropertyChanged("DcQuery");
			}
		}




		#endregion

		#region 計價項目 設定

		private List<NameValuePair<string>> _accItemListEdit;
		public List<NameValuePair<string>> AccItemListEdit
		{
			get { return _accItemListEdit; }
			set
			{
				_accItemListEdit = value;
				RaisePropertyChanged("AccItemListEdit");
			}
		}

		private List<NameValuePair<string>> _accItemListSelect;
		public List<NameValuePair<string>> AccItemListSelect
		{
			get { return _accItemListSelect; }
			set
			{
				_accItemListSelect = value;
				RaisePropertyChanged("AccItemListSelect");
			}
		}
		//Edit Dc Item
		private NameValuePair<string> _accItemEdit;
		public NameValuePair<string> AccItemEdit
		{
			get { return _accItemEdit; }
			set
			{
				_accItemEdit = value;
				if (value != null)
				{
					SetAccItemData(DcEdit, value.Name);
				}
				RaisePropertyChanged("AccItemEdit");
			}
		}
		//Edit Dc Value
		private string _AccEdit;
		public string AccEdit
		{
			get { return _AccEdit; }
			set
			{
				_AccEdit = value;
				RaisePropertyChanged("AccEdit");
			}
		}

		#endregion

		#region Add Data F500105
		private F500105 _f500105Add;
		public F500105 F500105Add
		{
			get { return _f500105Add; }
			set
			{
				_f500105Add = value;
				RaisePropertyChanged("F500105Add");
			}
		}
		#endregion

		#region 查詢結果 F500105QueryList 參數
		private ObservableCollection<F500105QueryData> _f500105QueryData;

		public ObservableCollection<F500105QueryData> F500105QueryList
		{
			get { return _f500105QueryData; }
			set
			{
				_f500105QueryData = value;
				RaisePropertyChanged("F500105QueryList");
			}
		}
		#endregion

		#region Select Data F500105
		private F500105QueryData _selectF500105Data;
		public F500105QueryData SelectF500105Data
		{
			get { return _selectF500105Data; }
			set
			{
				_selectF500105Data = value;
				if (value != null)
					SetSelectData();

				RaisePropertyChanged("SelectF500105Data");
			}
		}
		#endregion

		#region 稅別 List 參數
		private List<NameValuePair<string>> _inTaxList;
		public List<NameValuePair<string>> InTaxList
		{
			get { return _inTaxList; }
			set
			{
				_inTaxList = value;
				RaisePropertyChanged("InTaxList");
			}
		}
		#endregion

		#region 單據狀態 List 參數
		private List<NameValuePair<string>> _statusListQuery;
		public List<NameValuePair<string>> StatusListQuery
		{
			get { return _statusListQuery; }
			set
			{
				_statusListQuery = value;
				RaisePropertyChanged("StatusListQuery");
			}
		}

		private string _statusQuery;
		public string StatusQuery
		{
			get { return _statusQuery; }
			set
			{
				_statusQuery = value;
				RaisePropertyChanged("StatusQuery");
			}
		}

		#endregion

		#region 計價單位 List 參數
		private List<NameValuePair<string>> _accUnitList;
		public List<NameValuePair<string>> AccUnitList
		{
			get { return _accUnitList; }
			set
			{
				_accUnitList = value;
				RaisePropertyChanged("AccUnitList");
			}
		}
		#endregion

		#region 配送計價類別 List 參數
		private List<NameValuePair<string>> _delvAccList;
		public List<NameValuePair<string>> DelvAccList
		{
			get { return _delvAccList; }
			set
			{
				_delvAccList = value;
				RaisePropertyChanged("DelvAccList");
			}
		}
		#endregion

		#region 單據查詢單號
		private string _quoteQuery;
		public string QuoteQuery
		{
			get { return _quoteQuery; }
			set
			{
				_quoteQuery = value;
				RaisePropertyChanged("QuoteQuery");
			}
		}
		#endregion

		#region 有效日期-查詢  參數

		private DateTime? _enableSDataQuery;
		public DateTime? EnableSDataQuery
		{
			get { return _enableSDataQuery; }
			set
			{
				_enableSDataQuery = value;
				RaisePropertyChanged("EnableSDataQuery");
			}
		}

		private DateTime? _enableEDataQuery;
		public DateTime? EnableEDataQuery
		{
			get { return _enableEDataQuery; }
			set
			{
				_enableEDataQuery = value;
				RaisePropertyChanged("EnableEDataQuery");
			}
		}

		#endregion

		#region 建立人員/時間 異動人員/時間
		private string _crtName;
		public string CrtName
		{
			get { return _crtName; }
			set
			{
				_crtName = value;
				RaisePropertyChanged("CrtName");
			}
		}

		private DateTime? _crtDate;
		public DateTime? CrtDate
		{
			get { return _crtDate; }
			set
			{
				_crtDate = value;
				RaisePropertyChanged("CrtDate");
			}
		}

		private string _updName;
		public string UpdName
		{
			get { return _updName; }
			set
			{
				_updName = value;
				RaisePropertyChanged("UpdName");
			}
		}

		private DateTime? _updDate;
		public DateTime? UpdDate
		{
			get { return _updDate; }
			set
			{
				_updDate = value;
				RaisePropertyChanged("UpdDate");
			}
		}


		#endregion

		#region 儲存資料檢核
		private bool CheckSaveData()
		{
			string errorStr = "";
			if (F500105Add != null)
			{

				if (F500105Add.DISABLE_DATE < F500105Add.ENABLE_DATE)
				{
					errorStr += Properties.Resources.DISABLE_DATE_Invalid;
				}

				if (F500105Add.NET_RATE < 0)
				{
					errorStr += Properties.Resources.NET_RATE_Invalid;
				}


				if (F500105Add.APPROV_FEE.HasValue && F500105Add.APPROV_FEE <= 0)
				{
					errorStr += Properties.Resources.APPROV_FEE_Invalid;
				}

				if (F500105Add.ENABLE_DATE <= DateTime.Today)
				{
					errorStr += Properties.Resources.ENABLE_DATE_Invalid;
				}

				if (!string.IsNullOrEmpty(errorStr))
				{
					DialogService.ShowMessage(errorStr);
					return false;
				}
			}
			return true;
		}

		#endregion

		#region Clear Item
		private void ClearItem()
		{
			F500105Add = new F500105();
			CrtDate = DateTime.Now;
			CrtName = Wms3plSession.Get<UserInfo>().AccountName;
			UpdDate = null;
			UpdName = "";
			F500105Add.DISABLE_DATE = DateTime.Today.AddDays(1);
			F500105Add.ENABLE_DATE = DateTime.Today.AddDays(1);
		}
		#endregion

		#region 上傳路徑設定
		public string FileFolderPath
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "QUOTE", SelectF500105Data.DC_CODE, SelectF500105Data.GUP_CODE, SelectF500105Data.CUST_CODE, SelectF500105Data.CRT_DATE.Year.ToString() }); }
		}
		#endregion

		#region 選擇檔案名稱
		private string _selectFileName = string.Empty;
		public string SelectFileName
		{
			get { return _selectFileName; }
			set
			{
				_selectFileName = value;
				RaisePropertyChanged("SelectFileName");
			}
		}
		#endregion

		#endregion

		#region Function

		#region 取物流中心資料

		public List<NameValuePair<string>> GetDcList()
		{
			var data = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
			data.Insert(0, new NameValuePair<string>(Properties.Resources.NoSpecify, "000"));
			return data;
		}
		#endregion

		#region 取計價項目資料
		public List<NameValuePair<string>> GetAccItemData(string dcCode)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F199006s.Where(o => o.DC_CODE == dcCode && o.STATUS == "0").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.ACC_ITEM_NAME,
							Value = o.ACC_ITEM_NAME
						}).ToList();
			return list;
		}
		#endregion

		#region 取計價項目-詳細資料
		public void SetAccItemData(string dcCode, string accItemName)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F199006s.Where(o => o.DC_CODE == dcCode && o.ACC_ITEM_NAME == accItemName && o.STATUS == "0").First();
			if (F500105Add == null) F500105Add = new F500105();
			if (data != null && UserOperateMode == OperateMode.Add) //新增時選擇才變更
			{
				F500105Add.APPROV_FEE = null;
				F500105Add.ACC_ITEM_NAME = data.ACC_ITEM_NAME;
				F500105Add.ACC_NUM = data.ACC_NUM;
				F500105Add.ACC_UNIT = data.ACC_UNIT;
				F500105Add.IN_TAX = data.IN_TAX;
				F500105Add.FEE = data.FEE;
				F500105Add.DELV_ACC_TYPE = data.DELV_ACC_TYPE;
				F500105Add.ITEM_TYPE_ID = "006";

			}
		}
		#endregion

		#region 取計價單位 List
		public List<NameValuePair<string>> GetAccUnit()
		{

			var proxy = GetProxy<F91Entities>();
			var data = proxy.F91000302s.Where(o => o.ITEM_TYPE_ID == "006").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.ACC_UNIT_NAME,
							Value = o.ACC_UNIT
						}).ToList();
			return list;
		}
		#endregion

		#region 設定Select Data 資料
		private void SetSelectData()
		{
			if (SelectF500105Data != null)
			{
				if (F500105Add == null) F500105Add = new F500105();
				DcEdit = SelectF500105Data.DC_CODE;
				F500105Add.QUOTE_NO = SelectF500105Data.QUOTE_NO;
				F500105Add.ACC_ITEM_NAME = SelectF500105Data.ACC_ITEM_NAME;
				F500105Add.ENABLE_DATE = SelectF500105Data.ENABLE_DATE;
				F500105Add.DISABLE_DATE = SelectF500105Data.DISABLE_DATE;
				F500105Add.NET_RATE = (float)SelectF500105Data.NET_RATE;
				F500105Add.ACC_NUM = Convert.ToInt16(SelectF500105Data.ACC_NUM);
				F500105Add.ACC_UNIT = SelectF500105Data.ACC_UNIT;
				F500105Add.IN_TAX = SelectF500105Data.IN_TAX;
				F500105Add.APPROV_FEE = SelectF500105Data.APPROV_FEE;
				F500105Add.FEE = SelectF500105Data.FEE;
				F500105Add.MEMO = SelectF500105Data.MEMO;
				F500105Add.DC_CODE = SelectF500105Data.DC_CODE;
				F500105Add.GUP_CODE = SelectF500105Data.GUP_CODE;
				F500105Add.CUST_CODE = SelectF500105Data.CUST_CODE;
				F500105Add.DELV_ACC_TYPE = SelectF500105Data.DELV_ACC_TYPE;

				CrtDate = SelectF500105Data.CRT_DATE;
				CrtName = SelectF500105Data.CRT_NAME;
				UpdName = SelectF500105Data.UPD_NAME;
				UpdDate = SelectF500105Data.UPD_DATE;
				//ReUploadBtn();
			}

		}

		#endregion

		#endregion

		#region Approv
		public ICommand ApprovCommand
		{
			get
			{
				bool result = false;
				return CreateBusyAsyncCommand(
					o => result = DoApprov(), () => UserOperateMode == OperateMode.Edit && SelectF500105Data != null
						&& SelectF500105Data.STATUS != "1"
						, o =>
						{
							if (result)
							{
								QuoteQuery = SelectF500105Data.QUOTE_NO;
								StatusQuery = "1";
								ClearItem();
								SearchCommand.Execute(null);
								F500105Add = null;
							}
						}
					);
			}
		}

		private bool DoApprov()
		{
			if (F500105Add.APPROV_FEE == null || F500105Add.APPROV_FEE < 0)
			{
				DialogService.ShowMessage(Properties.Resources.Write_APPROV_FEE);
				return false;
			}

			var proxy = new wcf.P50WcfServiceClient();
			var wcf500105 = ExDataMapper.Map<F500105, wcf.F500105>(F500105Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF050105Status(wcf500105, "1"));

			if (result.IsSuccessed)
			{
				ShowMessage(Messages.InfoUpdateSuccess);
				UserOperateMode = OperateMode.Query;
				return true;
			}
			return false;

		}
		#endregion Approv

		#region ViewUpload
		public ICommand ViewUploadCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }, () => UserOperateMode == OperateMode.Query && SelectF500105Data != null
						&& (SelectF500105Data.STATUS == "1" || SelectF500105Data.STATUS == "2") //已核准才可上傳
						&& !string.IsNullOrEmpty(SelectF500105Data.UPLOAD_FILE)
					);
			}
		}


		#endregion

		#region Upload
		public ICommand UploadCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoUpload(), () => UserOperateMode == OperateMode.Edit && SelectF500105Data != null
						&& (SelectF500105Data.STATUS == "1" || SelectF500105Data.STATUS == "2")  //已核准才可上傳
						&& string.IsNullOrEmpty(SelectF500105Data.UPLOAD_FILE)
					, o =>
					{
						AddFile();
						//ReUploadBtn();

						StatusQuery = "2";
						ClearItem();
						SearchCommand.Execute(null);
						F500105Add = null;
					}
					);
			}
		}

		private void DoUpload()
		{
			DoUpLoad();
		}

		#region 新增檔案
		private void AddFile()
		{
			if ((SelectFileName == null || string.IsNullOrEmpty(SelectFileName)) && string.IsNullOrEmpty(SelectF500105Data.UPLOAD_FILE))
			{
				DialogService.ShowMessage(Properties.Resources.SelectFile);
			}
			else
			{
				var fileName = SelectF500105Data.QUOTE_NO + Path.GetExtension(SelectFileName);
				//copy 檔案
				if (File.Exists(SelectFileName))
				{
					if (!File.Exists(Path.Combine(FileFolderPath, fileName))) //檔案重複 重取編號
					{
						System.IO.File.Copy(SelectFileName, Path.Combine(FileFolderPath, fileName), true);

						F910404 f910404 = new F910404
						{
							QUOTE_NO = SelectF500105Data.QUOTE_NO,
							DC_CODE = SelectF500105Data.DC_CODE,
							GUP_CODE = SelectF500105Data.GUP_CODE,
							CUST_CODE = SelectF500105Data.CUST_CODE,
							UPLOAD_C_PATH = SelectFileName,
							UPLOAD_S_PATH = Path.Combine(FileFolderPath, fileName),
							UPLOAD_NO = 1
						};

						var proxy = new wcf91.P91WcfServiceClient();
						var wcf910404 = ExDataMapper.Map<F910404, wcf91.F910404>(f910404);
						var result = RunWcfMethod<wcf91.ExecuteResult>(proxy.InnerChannel,
										() => proxy.UploadFileF910404(wcf910404));

						if (result.IsSuccessed)
						{
							SelectF500105Data.UPLOAD_FILE = Path.Combine(FileFolderPath, fileName);

							var proxyf50 = new wcf.P50WcfServiceClient();
							var wcf500105 = ExDataMapper.Map<F500105, wcf.F500105>(F500105Add);
							var f50result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
											() => proxyf50.UpdateF050105Status(wcf500105, "2"));


							ShowMessage(Messages.InfoFileUploaded);
							UserOperateMode = OperateMode.Query;
						}
						else
						{
							ShowMessage(Messages.WarningFileUploadedFailure);
						}
					}
				}

				SelectFileName = "";
			}
		}
		#endregion


		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					, o =>
					{
						if (F500105QueryList != null && F500105QueryList.Any())
						{
							SelectF500105Data = F500105QueryList.First();
						}
					});
			}
		}

		private void DoSearch()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			var f500105QueryData = proxyEx.CreateQuery<F500105QueryData>("GetF500105QueryData")
					.AddQueryOption("dcCode", string.Format("'{0}'", DcQuery))
					.AddQueryOption("enableSDate", string.Format("'{0}'", EnableSDataQuery.ToString()))
					.AddQueryOption("enableEDate", string.Format("'{0}'", EnableEDataQuery.ToString()))
					.AddQueryOption("quoteNo", string.Format("'{0}'", QuoteQuery))
					.AddQueryOption("status", string.Format("'{0}'", StatusQuery));

			F500105QueryList = f500105QueryData.ToObservableCollection();
			if (F500105QueryList == null || F500105QueryList.Count() == 0)
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
			ClearItem();
			UserOperateMode = OperateMode.Add;
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query
						&& SelectF500105Data != null
						&& SelectF500105Data.STATUS != "9"
						&& SelectF500105Data.ENABLE_DATE > DateTime.Today,
					o =>
					{
						SetSelectData();
					});
			}
		}

		private void DoEdit()
		{
			if (SelectF500105Data == null)
			{
				DialogService.ShowMessage(Properties.Resources.SelectEditData);
				return;
			}
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
			F500105Add = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectF500105Data != null
						 && SelectF500105Data.STATUS == "0"
					, o =>
					{
						SearchCommand.Execute(null);
					});
			}
		}

		private void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				var proxy = new wcf.P50WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
								() => proxy.DeleteF500105(SelectF500105Data.DC_CODE, SelectF500105Data.GUP_CODE
															, SelectF500105Data.CUST_CODE, SelectF500105Data.QUOTE_NO));

				if (result.IsSuccessed)
				{
					ShowMessage(Messages.InfoUpdateSuccess);
				}
				else
				{
					ShowMessage(Messages.ErrorUpdateFailed);
				}

			}
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool result = false;
				return CreateBusyAsyncCommand(
					o => result = DoSave(), () => UserOperateMode != OperateMode.Query
						&& !(UserOperateMode == OperateMode.Edit && SelectF500105Data != null && SelectF500105Data.STATUS != "0")
					, o =>
					{
						if (result)
						{
							QuoteQuery = "";
							if (UserOperateMode == OperateMode.Edit)
								QuoteQuery = SelectF500105Data.QUOTE_NO;

							EnableSDataQuery = F500105Add.ENABLE_DATE;
							EnableEDataQuery = F500105Add.DISABLE_DATE;
							DcQuery = DcEdit;
							StatusQuery = "0";
							ClearItem();
							SearchCommand.Execute(null);
							F500105Add = null;
							UserOperateMode = OperateMode.Query;
						}
					});
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			bool result = false;
			if (UserOperateMode == OperateMode.Add)
			{
				result = DoAddSave();
			}
			else
			{
				result = DoEditSave();
			}
			return result;
		}

		private bool DoAddSave()
		{
			if (!CheckSaveData())
				return false;
			F500105Add.ITEM_TYPE_ID = "006";
			F500105Add.DC_CODE = DcEdit;
			F500105Add.GUP_CODE = _gupCode;
			F500105Add.CUST_CODE = _custCode;
			var proxy = new wcf.P50WcfServiceClient();
			var wcf500105 = ExDataMapper.Map<F500105, wcf.F500105>(F500105Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.InsertF500105(wcf500105));

			if (result.IsSuccessed)
			{
				ShowMessage(Messages.InfoAddSuccess);
			}

			return result.IsSuccessed;
		}

		private bool DoEditSave()
		{
			if (!CheckSaveData())
				return false;

			F500105Add.DC_CODE = DcEdit;
			F500105Add.GUP_CODE = _gupCode;
			F500105Add.CUST_CODE = _custCode;
			var proxy = new wcf.P50WcfServiceClient();
			var wcf500105 = ExDataMapper.Map<F500105, wcf.F500105>(F500105Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF500105(wcf500105));

			if (result.IsSuccessed)
			{
				ShowMessage(Messages.InfoUpdateSuccess);
			}

			return result.IsSuccessed;
		}


		#endregion Save
	}
}
