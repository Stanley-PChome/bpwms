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
using Wms3pl.WpfClient.DataServices.F50DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P50ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P50WcfService;
using wcf91 = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P50.ViewModel
{
	public partial class P5001020000_ViewModel : InputViewModelBase
	{
		public Action DoUpLoad = delegate { };
		public Action ReUploadBtn = delegate { };
		public P5001020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				DcList = GetDcList();
				DcListEdit = DcList;
				LocTypeList = GetLocTypeList();
				TmprTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1980", "TMPR_TYPE");
				InTaxList = GetBaseTableService.GetF000904List(FunctionCode, "F199001", "IN_TAX");
				StatusListQuery = GetBaseTableService.GetF000904List(FunctionCode, "P500102", "STATUS", true);
				StatusQuery = StatusListQuery.First().Value;
				AccUnitList = GetAccUnit();
				EnableSDataQuery = DateTime.Today;
				EnableEDataQuery = DateTime.Today;
				Uploadtxt = Properties.Resources.Upload;
				if (DcListEdit != null)
				{
					DcEdit = DcListEdit.First().Value;
					DcQuery = DcListEdit.First().Value;
				}

			}
		}
		#region 參數設定
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

		#region Add Data F500101
		private F500101 _f500101Add;
		public F500101 F500101Add
		{
			get { return _f500101Add; }
			set
			{
				_f500101Add = value;
				RaisePropertyChanged("F500101Add");
			}
		}
		#endregion

		#region Select Data F500101
		private F500101QueryData _selectF500101Data;
		public F500101QueryData SelectF500101Data
		{
			get { return _selectF500101Data; }
			set
			{
				_selectF500101Data = value;
				if (value != null)
					SetSelectData();
				RaisePropertyChanged("SelectF500101Data");
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

		#region 儲位 長/寬/高  參數

		private string _locHeight;
		public string LocHeight
		{
			get { return _locHeight; }
			set
			{
				_locHeight = value;
				RaisePropertyChanged("LocHeight");
			}
		}

		private string _locWidth;
		public string LocWidth
		{
			get { return _locWidth; }
			set
			{
				_locWidth = value;
				RaisePropertyChanged("LocWidth");
			}
		}

		private string _locLength;
		public string LocLength
		{
			get { return _locLength; }
			set
			{
				_locLength = value;
				RaisePropertyChanged("LocLength");
			}
		}

		#endregion

		#region 儲位類型資料 List 參數
		private List<NameValuePair<string>> _locTypeList;
		public List<NameValuePair<string>> LocTypeList
		{
			get { return _locTypeList; }
			set
			{
				_locTypeList = value;
				RaisePropertyChanged("LocTypeList");
			}
		}
		#endregion

		#region 溫度類型資料 List 參數
		private List<NameValuePair<string>> _tmprTypeList;
		public List<NameValuePair<string>> TmprTypeList
		{
			get { return _tmprTypeList; }
			set
			{
				_tmprTypeList = value;
				RaisePropertyChanged("TmprTypeList");
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

		#region 查詢結果 F500101QueryList 參數

		private ObservableCollection<F500101QueryData> _f500101QueryData;

		public ObservableCollection<F500101QueryData> F500101QueryList
		{
			get { return _f500101QueryData; }
			set
			{
				_f500101QueryData = value;
				RaisePropertyChanged("F500101QueryList");
			}
		}
		#endregion

		#region 上傳路徑設定
		public string FileFolderPath
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "QUOTE", SelectF500101Data.DC_CODE, SelectF500101Data.GUP_CODE, SelectF500101Data.CUST_CODE, SelectF500101Data.CRT_DATE.Year.ToString() }); }
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

		#region 上傳按鈕文件設定
		private string _uploadtxt;
		public string Uploadtxt
		{
			get { return _uploadtxt; }
			set
			{
				_uploadtxt = value;
				RaisePropertyChanged("Uploadtxt");
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
			var data = proxy.F199001s.Where(o => o.DC_CODE == dcCode && o.STATUS == "0").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.ACC_ITEM_NAME,
							Value = o.ACC_ITEM_NAME
						}).ToList();
			return list;
		}
		#endregion

		#region 取 ACC_ITEM_KIND_ID 資料
		public string GetAccItemKindId(string dcCode, string accItemName)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F199001s.Where(o => o.DC_CODE == dcCode && o.ACC_ITEM_NAME == accItemName && o.STATUS == "0").FirstOrDefault();
			if (data != null)
				return data.ACC_ITEM_KIND_ID;
			return "0";
		}
		#endregion

		#region 取計價項目-詳細資料
		public void SetAccItemData(string dcCode, string accItemName)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F199001s.Where(o => o.DC_CODE == dcCode && o.ACC_ITEM_NAME == accItemName).First();
			if (F500101Add == null) F500101Add = new F500101();
			if (data != null)
			{
				F500101Add.ACC_ITEM_NAME = data.ACC_ITEM_NAME;
				F500101Add.LOC_TYPE_ID = data.LOC_TYPE_ID;
				F500101Add.TMPR_TYPE = data.TMPR_TYPE;
				F500101Add.ACC_NUM = data.ACC_NUM;
				F500101Add.ACC_UNIT = data.ACC_UNIT;
				F500101Add.UNIT_FEE = data.UNIT_FEE;
				F500101Add.IN_TAX = data.IN_TAX;
				GetLocTypeList(data.LOC_TYPE_ID);
			}
		}
		#endregion

		#region 取儲位料架型態 長/寬/高
		public void GetLocTypeList(string locTypeId)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1942s.Where(o => o.LOC_TYPE_ID == locTypeId).FirstOrDefault();
			if (data != null)
			{
				LocHeight = data.HEIGHT.ToString();
				LocWidth = data.WEIGHT.ToString();
				LocLength = data.LENGTH.ToString();
			}
		}
		#endregion

		#region 取儲位資料 List
		public List<NameValuePair<string>> GetLocTypeList()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1942s.ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.LOC_TYPE_NAME,
							Value = o.LOC_TYPE_ID
						}).ToList();
			return list;
		}
		#endregion

		#region 取計價單位 List
		public List<NameValuePair<string>> GetAccUnit()
		{

			var proxy = GetProxy<F91Entities>();
			var data = proxy.F91000302s.Where(o => o.ITEM_TYPE_ID == "002").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.ACC_UNIT_NAME,
							Value = o.ACC_UNIT
						}).ToList();
			return list;
		}
		#endregion

		#region 儲存資料檢核

		private bool CheckSaveData()
		{
			string errorStr = "";
			if (F500101Add != null)
			{

				if (F500101Add.DISABLE_DATE < F500101Add.ENABLE_DATE)
				{
					errorStr += Properties.Resources.DISABLE_DATE_Invalid;
				}

				if (F500101Add.NET_RATE < 0)
				{
					errorStr += Properties.Resources.NET_RATE_Invalid;
				}

				if (F500101Add.APPROV_UNIT_FEE <= 0)
				{
					errorStr += Properties.Resources.APPROV_UNIT_FEE_Invalid;
				}

				if (F500101Add.ENABLE_DATE <= DateTime.Today)
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

		#region 設定Select Data 資料
		private void SetSelectData()
		{
			if (SelectF500101Data != null)
			{
				//儲位 長寬高
				GetLocTypeList(SelectF500101Data.LOC_TYPE_ID);

				if (F500101Add == null) F500101Add = new F500101();
				DcEdit = SelectF500101Data.DC_CODE;
				F500101Add.QUOTE_NO = SelectF500101Data.QUOTE_NO;
				F500101Add.ACC_ITEM_NAME = SelectF500101Data.ACC_ITEM_NAME;
				F500101Add.ENABLE_DATE = SelectF500101Data.ENABLE_DATE;
				F500101Add.DISABLE_DATE = SelectF500101Data.DISABLE_DATE;
				F500101Add.NET_RATE = (float)SelectF500101Data.NET_RATE;
				F500101Add.LOC_TYPE_ID = SelectF500101Data.LOC_TYPE_ID;
				F500101Add.TMPR_TYPE = SelectF500101Data.TMPR_TYPE;
				F500101Add.ACC_NUM = Convert.ToInt16(SelectF500101Data.ACC_NUM);
				F500101Add.ACC_UNIT = SelectF500101Data.ACC_UNIT;
				F500101Add.UNIT_FEE = SelectF500101Data.UNIT_FEE;
				F500101Add.IN_TAX = SelectF500101Data.IN_TAX;
				F500101Add.APPROV_UNIT_FEE = SelectF500101Data.APPROV_UNIT_FEE;
				F500101Add.MEMO = SelectF500101Data.MEMO;
				F500101Add.DC_CODE = SelectF500101Data.DC_CODE;
				F500101Add.GUP_CODE = SelectF500101Data.GUP_CODE;
				F500101Add.CUST_CODE = SelectF500101Data.CUST_CODE;

				CrtDate = SelectF500101Data.CRT_DATE;
				CrtName = SelectF500101Data.CRT_NAME;
				UpdName = SelectF500101Data.UPD_NAME;
				UpdDate = SelectF500101Data.UPD_DATE;

			}

		}

		#endregion

		#region Clear Item
		private void ClearItem()
		{
			F500101Add = new F500101();
			LocHeight = "";
			LocWidth = "";
			LocLength = "";
			CrtDate = DateTime.Now;
			CrtName = Wms3plSession.Get<UserInfo>().AccountName;
			UpdDate = null;
			UpdName = "";
			F500101Add.DISABLE_DATE = DateTime.Today.AddDays(1);
			F500101Add.ENABLE_DATE = DateTime.Today.AddDays(1);
		}
		#endregion

		#endregion


		#region Command

		#region Approv
		public ICommand ApprovCommand
		{
			get
			{
				bool result = false;
				return CreateBusyAsyncCommand(
					o => result = DoApprov(), () => UserOperateMode == OperateMode.Edit && SelectF500101Data != null
						&& SelectF500101Data.STATUS != "1"
						, o =>
						{
							if (result)
							{
								QuoteQuery = SelectF500101Data.QUOTE_NO;
								StatusQuery = "1";
								ClearItem();
								SearchCommand.Execute(null);
								F500101Add = null;
							}
						}
					);
			}
		}

		private bool DoApprov()
		{
			if (F500101Add.APPROV_UNIT_FEE == null || F500101Add.APPROV_UNIT_FEE <= 0)
			{
				DialogService.ShowMessage(Properties.Resources.APPROV_UNIT_FEE_Empty);
				return false;
			}

			F500101Add.ACC_ITEM_KIND_ID = GetAccItemKindId(DcEdit, F500101Add.ACC_ITEM_NAME);

			var proxy = new wcf.P50WcfServiceClient();
			var wcf500101 = ExDataMapper.Map<F500101, wcf.F500101>(F500101Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF050101Status(wcf500101, "1"));

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
					o => { }, () => UserOperateMode == OperateMode.Query && SelectF500101Data != null
						&& (SelectF500101Data.STATUS == "1" || SelectF500101Data.STATUS == "2") //已核准才可上傳
						&& !string.IsNullOrEmpty(SelectF500101Data.UPLOAD_FILE)
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
					o => DoUpload(), () => UserOperateMode == OperateMode.Edit && SelectF500101Data != null
						&& (SelectF500101Data.STATUS == "1" || SelectF500101Data.STATUS == "2") //已核准才可上傳
						&& string.IsNullOrEmpty(SelectF500101Data.UPLOAD_FILE)
					, o =>
						{
							AddFile();
							//ReUploadBtn();

							StatusQuery = "2";
							ClearItem();
							SearchCommand.Execute(null);
							F500101Add = null;
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
			if ((SelectFileName == null || string.IsNullOrEmpty(SelectFileName)) && string.IsNullOrEmpty(SelectF500101Data.UPLOAD_FILE))
			{
				DialogService.ShowMessage(Properties.Resources.SelectFile);
			}
			else
			{
				var fileName = SelectF500101Data.QUOTE_NO + Path.GetExtension(SelectFileName);
				//copy 檔案
				if (File.Exists(SelectFileName))
				{
					if (!File.Exists(Path.Combine(FileFolderPath, fileName))) //檔案重複 重取編號
					{
						System.IO.File.Copy(SelectFileName, Path.Combine(FileFolderPath, fileName), true);

						F910404 f910404 = new F910404
						{
							QUOTE_NO = SelectF500101Data.QUOTE_NO,
							DC_CODE = SelectF500101Data.DC_CODE,
							GUP_CODE = SelectF500101Data.GUP_CODE,
							CUST_CODE = SelectF500101Data.CUST_CODE,
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
							SelectF500101Data.UPLOAD_FILE = Path.Combine(FileFolderPath, fileName);

							var proxyf50 = new wcf.P50WcfServiceClient();
							var wcf500101 = ExDataMapper.Map<F500101, wcf.F500101>(F500101Add);
							var f50result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
											() => proxyf50.UpdateF050101Status(wcf500101, "2"));

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
							if (F500101QueryList != null && F500101QueryList.Any())
							{
								SelectF500101Data = F500101QueryList.First();
							}
						});
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
			var proxyEx = GetExProxy<P50ExDataSource>();
			var f500101QueryData = proxyEx.CreateQuery<F500101QueryData>("GetF500101QueryData")
					.AddQueryOption("dcCode", string.Format("'{0}'", DcQuery))
					.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
					.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
					.AddQueryOption("enableSDate", string.Format("'{0}'", EnableSDataQuery.ToString()))
					.AddQueryOption("enableEDate", string.Format("'{0}'", EnableEDataQuery.ToString()))
					.AddQueryOption("quoteNo", string.Format("'{0}'", QuoteQuery))
					.AddQueryOption("status", string.Format("'{0}'", StatusQuery));

			F500101QueryList = f500101QueryData.ToObservableCollection();

			if (F500101QueryList == null || F500101QueryList.Count() == 0)
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
			//執行新增動作
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
						&& SelectF500101Data != null
						&& SelectF500101Data.STATUS != "9"
						&& SelectF500101Data.ENABLE_DATE > DateTime.Today,
					o =>
					{
						SetSelectData();
						//ReUploadBtn();
					}
					);
			}
		}

		private void DoEdit()
		{
			if (SelectF500101Data == null)
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
			F500101Add = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectF500101Data != null
						 && SelectF500101Data.STATUS == "0"
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
				//var wcf500101 = ExDataMapper.Map<F500101, wcf.F500101>(F500101Add);

				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
								() => proxy.DeleteF500101(SelectF500101Data.DC_CODE, SelectF500101Data.GUP_CODE
															, SelectF500101Data.CUST_CODE, SelectF500101Data.QUOTE_NO));

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
						&& !(UserOperateMode == OperateMode.Edit && SelectF500101Data != null && SelectF500101Data.STATUS != "0")
					, o =>
						{
							if (result)
							{
								QuoteQuery = "";
								if (UserOperateMode == OperateMode.Edit)
									QuoteQuery = SelectF500101Data.QUOTE_NO;

								EnableSDataQuery = F500101Add.ENABLE_DATE;
								EnableEDataQuery = F500101Add.DISABLE_DATE;
								DcQuery = DcEdit;
								StatusQuery = "0";
								ClearItem();
								SearchCommand.Execute(null);
								F500101Add = null;
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

			F500101Add.ACC_ITEM_KIND_ID = GetAccItemKindId(DcEdit, F500101Add.ACC_ITEM_NAME);
			F500101Add.DC_CODE = DcEdit;
			F500101Add.GUP_CODE = _gupCode;
			F500101Add.CUST_CODE = _custCode;
			var proxy = new wcf.P50WcfServiceClient();
			var wcf500101 = ExDataMapper.Map<F500101, wcf.F500101>(F500101Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.InsertF500101(wcf500101));

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

			F500101Add.ACC_ITEM_KIND_ID = GetAccItemKindId(DcEdit, F500101Add.ACC_ITEM_NAME);
			F500101Add.DC_CODE = DcEdit;
			F500101Add.GUP_CODE = _gupCode;
			F500101Add.CUST_CODE = _custCode;
			var proxy = new wcf.P50WcfServiceClient();
			var wcf500101 = ExDataMapper.Map<F500101, wcf.F500101>(F500101Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF500101(wcf500101));

			if (result.IsSuccessed)
			{
				ShowMessage(Messages.InfoUpdateSuccess);
			}

			return result.IsSuccessed;
		}
		#endregion Save

		#endregion
	}
}
