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
	public partial class P5001040000_ViewModel : InputViewModelBase
	{
		public Action DoUpLoad = delegate { };
		public Action ReUploadBtn = delegate { };

		public P5001040000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				DcList = GetDcList();
				DcListEdit = DcList;
				InTaxList = GetBaseTableService.GetF000904List(FunctionCode, "F199003", "IN_TAX");
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

		#region Add Data F500103
		private F500103 _f500103Add;
		public F500103 F500103Add
		{
			get { return _f500103Add; }
			set
			{
				_f500103Add = value;
				RaisePropertyChanged("F500103Add");
			}
		}
		#endregion

		#region Select Data F500103
		private F500103QueryData _selectF500103Data;
		public F500103QueryData SelectF500103Data
		{
			get { return _selectF500103Data; }
			set
			{
				_selectF500103Data = value;
				if (value != null)
				{
					DelvAccList = GetDelvAccTypeList(value.ACC_ITEM_KIND_ID);
					SetSelectData();
				}
				RaisePropertyChanged("SelectF500103Data");
			}
		}
		#endregion

		#region 查詢結果 F500103QueryList 參數
		private ObservableCollection<F500103QueryData> _f500103QueryData;

		public ObservableCollection<F500103QueryData> F500103QueryList
		{
			get { return _f500103QueryData; }
			set
			{
				_f500103QueryData = value;
				RaisePropertyChanged("F500103QueryList");
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

		#region 上傳路徑設定
		public string FileFolderPath
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "QUOTE", SelectF500103Data.DC_CODE, SelectF500103Data.GUP_CODE, SelectF500103Data.CUST_CODE, SelectF500103Data.CRT_DATE.Year.ToString() }); }
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
			var data = proxy.F199003s.Where(o => o.DC_CODE == dcCode && o.STATUS == "0").ToList();
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
			var data = proxy.F199003s.Where(o => o.DC_CODE == dcCode && o.ACC_ITEM_NAME == accItemName && o.STATUS == "0").FirstOrDefault();
			if (data != null)
				return data.ACC_ITEM_KIND_ID;
			return "0";
		}
		#endregion

		#region 取計價項目-詳細資料
		public void SetAccItemData(string dcCode, string accItemName)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F199003s.Where(o => o.DC_CODE == dcCode && o.ACC_ITEM_NAME == accItemName && o.STATUS == "0").First();
			if (F500103Add == null) F500103Add = new F500103();
			if (data != null && UserOperateMode == OperateMode.Add) //新增時選擇才變更
			{
				F500103Add.APPROV_FEE = null;
				F500103Add.APPROV_BASIC_FEE = null;
				F500103Add.APPROV_OVER_FEE = null;
				F500103Add.ACC_ITEM_NAME = data.ACC_ITEM_NAME;
				F500103Add.ACC_NUM = data.ACC_NUM;
				F500103Add.ACC_UNIT = data.ACC_UNIT;
				F500103Add.IN_TAX = data.IN_TAX;
				F500103Add.ACC_KIND = data.ACC_KIND;
				F500103Add.FEE = data.FEE;
				F500103Add.BASIC_FEE = data.BASIC_FEE;
				F500103Add.OVER_FEE = data.OVER_FEE;

				DelvAccList = GetDelvAccTypeList(data.ACC_ITEM_KIND_ID);
				F500103Add.DELV_ACC_TYPE = data.DELV_ACC_TYPE;
			}
		}
		#endregion

		#region 取計價單位 List
		public List<NameValuePair<string>> GetAccUnit()
		{

			var proxy = GetProxy<F91Entities>();
			var data = proxy.F91000302s.Where(o => o.ITEM_TYPE_ID == "004").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.ACC_UNIT_NAME,
							Value = o.ACC_UNIT
						}).ToList();
			return list;
		}
		#endregion

		#region 取配送計價類別 List
		public List<NameValuePair<string>> GetDelvAccTypeList(string accItemKind)
		{
			if (string.IsNullOrEmpty(accItemKind)) return null;
			var proxy = GetExProxy<P71ExDataSource>();
			var results = proxy.CreateQuery<F000904DelvAccType>("GetDelvAccTypes")
												 .AddQueryExOption("itemTypeId", "004")
												 .AddQueryExOption("accItemKindId", accItemKind)
												 .ToList()
												 .Select(x => new NameValuePair<string>()
												 {
													 Name = x.DELV_ACC_TYPE_NAME,
													 Value = x.DELV_ACC_TYPE
												 }
												 ).ToList();
			return results;
		}

		#endregion

		#region 設定Select Data 資料
		private void SetSelectData()
		{
			if (SelectF500103Data != null)
			{
				if (F500103Add == null) F500103Add = new F500103();
				DcEdit = SelectF500103Data.DC_CODE;
				F500103Add.QUOTE_NO = SelectF500103Data.QUOTE_NO;
				F500103Add.ACC_ITEM_NAME = SelectF500103Data.ACC_ITEM_NAME;
				F500103Add.ENABLE_DATE = SelectF500103Data.ENABLE_DATE;
				F500103Add.DISABLE_DATE = SelectF500103Data.DISABLE_DATE;
				F500103Add.NET_RATE = (float)SelectF500103Data.NET_RATE;
				F500103Add.ACC_NUM = Convert.ToInt16(SelectF500103Data.ACC_NUM);
				F500103Add.ACC_UNIT = SelectF500103Data.ACC_UNIT;
				F500103Add.IN_TAX = SelectF500103Data.IN_TAX;
				F500103Add.ACC_KIND = SelectF500103Data.ACC_KIND;
				F500103Add.APPROV_OVER_FEE = SelectF500103Data.APPROV_OVER_FEE;
				F500103Add.APPROV_FEE = SelectF500103Data.APPROV_FEE;
				F500103Add.APPROV_BASIC_FEE = SelectF500103Data.APPROV_BASIC_FEE;
				F500103Add.FEE = SelectF500103Data.FEE;
				F500103Add.OVER_FEE = SelectF500103Data.OVER_FEE;
				F500103Add.BASIC_FEE = SelectF500103Data.BASIC_FEE;
				F500103Add.MEMO = SelectF500103Data.MEMO;
				F500103Add.DC_CODE = SelectF500103Data.DC_CODE;
				F500103Add.GUP_CODE = SelectF500103Data.GUP_CODE;
				F500103Add.CUST_CODE = SelectF500103Data.CUST_CODE;
				F500103Add.DELV_ACC_TYPE = SelectF500103Data.DELV_ACC_TYPE;

				CrtDate = SelectF500103Data.CRT_DATE;
				CrtName = SelectF500103Data.CRT_NAME;
				UpdName = SelectF500103Data.UPD_NAME;
				UpdDate = SelectF500103Data.UPD_DATE;
				//ReUploadBtn();
			}

		}

		#endregion

		#region 儲存資料檢核
		private bool CheckSaveData()
		{
			string errorStr = "";
			if (F500103Add != null)
			{

				if (F500103Add.DISABLE_DATE < F500103Add.ENABLE_DATE)
				{
					errorStr += Properties.Resources.DISABLE_DATE_Invalid;
				}

				if (F500103Add.NET_RATE < 0)
				{
					errorStr += Properties.Resources.NET_RATE_Invalid;
				}


				if (F500103Add.ACC_KIND == "A" && F500103Add.APPROV_FEE.HasValue && F500103Add.APPROV_FEE <= 0)
				{
					errorStr += Properties.Resources.APPROV_FEE_Invalid;
				}

				if (F500103Add.ACC_KIND == "B" && F500103Add.APPROV_BASIC_FEE.HasValue && F500103Add.APPROV_BASIC_FEE <= 0)
				{
					errorStr += Properties.Resources.APPROV_BASIC_FEE_Invalid;
				}

				if (F500103Add.ACC_KIND == "B" && F500103Add.APPROV_OVER_FEE.HasValue && F500103Add.APPROV_OVER_FEE <= 0)
				{
					errorStr += Properties.Resources.APPROV_OVER_FEE_Invalid;
				}

				if (F500103Add.ENABLE_DATE <= DateTime.Today)
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
			F500103Add = new F500103();
			CrtDate = DateTime.Now;
			CrtName = Wms3plSession.Get<UserInfo>().AccountName;
			UpdDate = null;
			UpdName = "";
			F500103Add.DISABLE_DATE = DateTime.Today.AddDays(1);
			F500103Add.ENABLE_DATE = DateTime.Today.AddDays(1);
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
					o => result = DoApprov(), () => UserOperateMode == OperateMode.Edit && SelectF500103Data != null
						&& SelectF500103Data.STATUS != "1"
						, o =>
						{
							if (result)
							{
								QuoteQuery = SelectF500103Data.QUOTE_NO;
								StatusQuery = "1";
								ClearItem();
								SearchCommand.Execute(null);
								F500103Add = null;
							}
						}
					);
			}
		}

		private bool DoApprov()
		{
			if (F500103Add.ACC_KIND == "A")
			{
				if (F500103Add.APPROV_FEE == null || F500103Add.APPROV_FEE < 0)
				{
					DialogService.ShowMessage(Properties.Resources.Write_APPROV_FEE);
					return false;
				}
			}
			else
			{
				if (F500103Add.APPROV_BASIC_FEE == null || F500103Add.APPROV_OVER_FEE == null || F500103Add.APPROV_BASIC_FEE < 0 || F500103Add.APPROV_OVER_FEE < 0)
				{
					DialogService.ShowMessage(Properties.Resources.Write_APPROV_BASIC_FEE_Or_APPROV_OVER_FEE);
					return false;
				}
			}

			var proxy = new wcf.P50WcfServiceClient();
			var wcf500103 = ExDataMapper.Map<F500103, wcf.F500103>(F500103Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF050103Status(wcf500103, "1"));

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
					o => { }, () => UserOperateMode == OperateMode.Query && SelectF500103Data != null
						&& (SelectF500103Data.STATUS == "1" || SelectF500103Data.STATUS == "2") //已核准才可上傳
						&& !string.IsNullOrEmpty(SelectF500103Data.UPLOAD_FILE)
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
					o => DoUpload(), () => UserOperateMode == OperateMode.Edit && SelectF500103Data != null
						&& (SelectF500103Data.STATUS == "1" || SelectF500103Data.STATUS == "2")  //已核准才可上傳
						&& string.IsNullOrEmpty(SelectF500103Data.UPLOAD_FILE)
					, o =>
					{
						AddFile();
						//ReUploadBtn();

						StatusQuery = "2";
						ClearItem();
						SearchCommand.Execute(null);
						F500103Add = null;
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
			if ((SelectFileName == null || string.IsNullOrEmpty(SelectFileName)) && string.IsNullOrEmpty(SelectF500103Data.UPLOAD_FILE))
			{
				DialogService.ShowMessage(Properties.Resources.SelectFile);
			}
			else
			{
				var fileName = SelectF500103Data.QUOTE_NO + Path.GetExtension(SelectFileName);
				//copy 檔案
				if (File.Exists(SelectFileName))
				{
					if (!File.Exists(Path.Combine(FileFolderPath, fileName))) //檔案重複 重取編號
					{
						System.IO.File.Copy(SelectFileName, Path.Combine(FileFolderPath, fileName), true);

						F910404 f910404 = new F910404
						{
							QUOTE_NO = SelectF500103Data.QUOTE_NO,
							DC_CODE = SelectF500103Data.DC_CODE,
							GUP_CODE = SelectF500103Data.GUP_CODE,
							CUST_CODE = SelectF500103Data.CUST_CODE,
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
							SelectF500103Data.UPLOAD_FILE = Path.Combine(FileFolderPath, fileName);

							var proxyf50 = new wcf.P50WcfServiceClient();
							var wcf500103 = ExDataMapper.Map<F500103, wcf.F500103>(F500103Add);
							var f50result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
											() => proxyf50.UpdateF050103Status(wcf500103, "2"));

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
						if (F500103QueryList != null && F500103QueryList.Any())
						{
							SelectF500103Data = F500103QueryList.First();
						}
					});
			}
		}

		private void DoSearch()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			var f500103QueryData = proxyEx.CreateQuery<F500103QueryData>("GetF500103QueryData")
					.AddQueryOption("dcCode", string.Format("'{0}'", DcQuery))
					.AddQueryOption("enableSDate", string.Format("'{0}'", EnableSDataQuery.ToString()))
					.AddQueryOption("enableEDate", string.Format("'{0}'", EnableEDataQuery.ToString()))
					.AddQueryOption("quoteNo", string.Format("'{0}'", QuoteQuery))
					.AddQueryOption("status", string.Format("'{0}'", StatusQuery));

			F500103QueryList = f500103QueryData.ToObservableCollection();
			if (F500103QueryList == null || F500103QueryList.Count() == 0)
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
						&& SelectF500103Data != null
						&& SelectF500103Data.STATUS != "9"
						&& SelectF500103Data.ENABLE_DATE > DateTime.Today,
					o =>
					{
						SetSelectData();

					});
			}
		}

		private void DoEdit()
		{
			if (SelectF500103Data == null)
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
			F500103Add = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectF500103Data != null
						 && SelectF500103Data.STATUS == "0"
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
								() => proxy.DeleteF500103(SelectF500103Data.DC_CODE, SelectF500103Data.GUP_CODE
															, SelectF500103Data.CUST_CODE, SelectF500103Data.QUOTE_NO));

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
						&& !(UserOperateMode == OperateMode.Edit && SelectF500103Data != null && SelectF500103Data.STATUS != "0")
					, o =>
					{
						if (result)
						{
							QuoteQuery = "";
							if (UserOperateMode == OperateMode.Edit)
								QuoteQuery = SelectF500103Data.QUOTE_NO;

							EnableSDataQuery = F500103Add.ENABLE_DATE;
							EnableEDataQuery = F500103Add.DISABLE_DATE;
							DcQuery = DcEdit;
							StatusQuery = "0";
							ClearItem();
							SearchCommand.Execute(null);
							F500103Add = null;
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
			F500103Add.ITEM_TYPE_ID = "004";
			F500103Add.ACC_ITEM_KIND_ID = GetAccItemKindId(DcEdit, F500103Add.ACC_ITEM_NAME);
			F500103Add.DC_CODE = DcEdit;
			F500103Add.GUP_CODE = _gupCode;
			F500103Add.CUST_CODE = _custCode;
			var proxy = new wcf.P50WcfServiceClient();
			var wcf500103 = ExDataMapper.Map<F500103, wcf.F500103>(F500103Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.InsertF500103(wcf500103));

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

			F500103Add.ACC_ITEM_KIND_ID = GetAccItemKindId(DcEdit, F500103Add.ACC_ITEM_NAME);
			F500103Add.DC_CODE = DcEdit;
			F500103Add.GUP_CODE = _gupCode;
			F500103Add.CUST_CODE = _custCode;
			var proxy = new wcf.P50WcfServiceClient();
			var wcf500103 = ExDataMapper.Map<F500103, wcf.F500103>(F500103Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF500103(wcf500103));

			if (result.IsSuccessed)
			{
				ShowMessage(Messages.InfoUpdateSuccess);
			}

			return result.IsSuccessed;
		}


		#endregion Save
	}
}
