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
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P50WcfService;
using wcf91 = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using Wms3pl.WpfClient.UILib.Services;


namespace Wms3pl.WpfClient.P50.ViewModel
{
	public partial class P5001050000_ViewModel : InputViewModelBase
	{
		public Action ResetUI = delegate { };
		public Action DoUpLoad = delegate { };
		public Action ReUploadBtn = delegate { };
		public P5001050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				DcList = GetDcList();
				DcListEdit = DcList;
				InTaxList = GetBaseTableService.GetF000904List(FunctionCode, "F199003", "IN_TAX");
				StatusListQuery = GetBaseTableService.GetF000904List(FunctionCode, "P500102", "STATUS", true);
				CustTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F199005", "CUST_TYPE");
				LogiTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F199005", "LOGI_TYPE");
				AccKindList = GetBaseTableService.GetF000904List(FunctionCode, "F199005", "ACC_KIND");
				DelvEfficList = GetBaseTableService.GetF000904List(FunctionCode, "F190102", "DELV_EFFIC");
				DelvTmprList = GetBaseTableService.GetF000904List(FunctionCode, "F199005", "DELV_TMPR");
				StatusQuery = StatusListQuery.First().Value;
				AccUnitList = GetAccUnit();
				CarKindList = GetCarKindList();
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
				{
					AccItemListEdit = GetAccItemData(value);
					if (UserOperateMode == OperateMode.Add)
						AccAreaList = GetAccAreaList(value);
				}
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
				{
					AccItemListSelect = GetAccItemData(value);
					AccAreaList = GetAccAreaList(value);
				}
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

		#region Add Data F500102
		private F500102 _f500102Add;
		public F500102 F500102Add
		{
			get { return _f500102Add; }
			set
			{
				_f500102Add = value;
				RaisePropertyChanged("F500102Add");
			}
		}
		#endregion

		#region Select Data F500102
		private F500102QueryData _selectF500102Data;
		public F500102QueryData SelectF500102Data
		{
			get { return _selectF500102Data; }
			set
			{
				_selectF500102Data = value;
				if (value != null)
				{
					DelvAccList = GetDelvAccTypeList(value.ACC_ITEM_KIND_ID);
					GetCarKindData(value.CAR_KIND_ID);
					ApprovOver = false;
					if (value.ACC_KIND != "F")
						ApprovOver = true;  // 顯示 貨主核定超標每單位費用 欄位

					SetSelectData();
				}
				RaisePropertyChanged("SelectF500102Data");
			}
		}
		#endregion

		#region 查詢結果 F500102QueryList 參數
		private ObservableCollection<F500102QueryData> _f500102QueryData;

		public ObservableCollection<F500102QueryData> F500102QueryList
		{
			get { return _f500102QueryData; }
			set
			{
				_f500102QueryData = value;
				RaisePropertyChanged("F500102QueryList");
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

		#region 單據類型 List 參數
		private List<NameValuePair<string>> _custTypeList;
		public List<NameValuePair<string>> CustTypeList
		{
			get { return _custTypeList; }
			set
			{
				_custTypeList = value;
				RaisePropertyChanged("CustTypeList");
			}
		}
		#endregion

		#region 物流類別(01:正物流 02:逆物流)   List 參數
		private List<NameValuePair<string>> _logiTypeList;

		public List<NameValuePair<string>> LogiTypeList
		{
			get { return _logiTypeList; }
			set
			{
				if (_logiTypeList == value) return;
				Set(() => LogiTypeList, ref _logiTypeList, value);
			}
		}

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

		#endregion

		#region 計價方式(A:均一價 B:實際尺寸 C:材積 D:重量)  List 參數
		private List<NameValuePair<string>> _accKindList;

		public List<NameValuePair<string>> AccKindList
		{
			get { return _accKindList; }
			set
			{
				if (_accKindList == value) return;
				Set(() => AccKindList, ref _accKindList, value);
			}
		}
		#endregion

		#region 配送效率(01:一般、02:3小時、03:6小時、04:9小時)  List 參數
		private List<NameValuePair<string>> _delvEfficList;

		public List<NameValuePair<string>> DelvEfficList
		{
			get { return _delvEfficList; }
			set
			{
				if (_delvEfficList == value) return;
				Set(() => DelvEfficList, ref _delvEfficList, value);
			}
		}
		#endregion

		#region 計費區域 List 參數
		private List<NameValuePair<string>> _accAreaList;
		public List<NameValuePair<string>> AccAreaList
		{
			get { return _accAreaList; }
			set
			{
				_accAreaList = value;
				RaisePropertyChanged("AccAreaList");
			}
		}
		#endregion

		#region 車輛種類 List 參數
		private List<NameValuePair<string>> _carKindList;
		public List<NameValuePair<string>> CarKindList
		{
			get { return _carKindList; }
			set
			{
				_carKindList = value;
				RaisePropertyChanged("CarKindList");
			}
		}
		#endregion

		#region 車輛種類-噸數 參數
		private string _carTonne;
		public string CarTonne
		{
			get { return _carTonne; }
			set
			{
				_carTonne = value;
				RaisePropertyChanged("CarTonne");
			}
		}
		#endregion

		#region 車輛種類-溫層 參數
		private string _temperature;
		public string Temperature
		{
			get { return _temperature; }
			set
			{
				_temperature = value;
				RaisePropertyChanged("Temperature");
			}
		}
		#endregion

		#region 配送溫層 List 參數
		private List<NameValuePair<string>> _delvTmprList;
		public List<NameValuePair<string>> DelvTmprList
		{
			get { return _delvTmprList; }
			set
			{
				_delvTmprList = value;
				RaisePropertyChanged("DelvTmprList");
			}
		}
		#endregion

		#region 顯示超收核定價格  參數
		private bool _approvOver;
		public bool ApprovOver
		{
			get { return _approvOver; }
			set
			{
				_approvOver = value;
				RaisePropertyChanged("ApprovOver");
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

		#region 上傳路徑設定
		public string FileFolderPath
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "QUOTE", SelectF500102Data.DC_CODE, SelectF500102Data.GUP_CODE, SelectF500102Data.CUST_CODE, SelectF500102Data.CRT_DATE.Year.ToString() }); }
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
			var data = proxy.F199005s.Where(o => o.DC_CODE == dcCode && o.STATUS == "0").ToList();
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
			var data = proxy.F199005s.Where(o => o.DC_CODE == dcCode && o.ACC_ITEM_NAME == accItemName && o.STATUS == "0").FirstOrDefault();
			if (data != null)
				return data.ACC_ITEM_KIND_ID;
			return "0";
		}
		#endregion

		#region 取計價項目-詳細資料
		public void SetAccItemData(string dcCode, string accItemName)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F199005s.Where(o => o.DC_CODE == dcCode && o.ACC_ITEM_NAME == accItemName && o.STATUS == "0").First();
			if (F500102Add == null) F500102Add = new F500102();
			if (data != null && UserOperateMode == OperateMode.Add)
			{

				F500102Add.APPROV_FEE = null;
				F500102Add.APPROV_OVER_UNIT_FEE = null;
				F500102Add.ACC_ITEM_NAME = data.ACC_ITEM_NAME;
				F500102Add.ACC_NUM = data.ACC_NUM;
				F500102Add.ACC_UNIT = data.ACC_UNIT;
				F500102Add.IN_TAX = data.IN_TAX;
				F500102Add.FEE = data.FEE;
				F500102Add.ITEM_TYPE_ID = "005";
				F500102Add.ACC_ITEM_KIND_ID = data.ACC_ITEM_KIND_ID;
				F500102Add.ACC_ITEM_NAME = data.ACC_ITEM_NAME;				
				F500102Add.LOGI_TYPE = data.LOGI_TYPE;
				F500102Add.ACC_KIND = data.ACC_KIND;
				F500102Add.IS_SPECIAL_CAR = data.IS_SPECIAL_CAR;
				F500102Add.CAR_KIND_ID = data.CAR_KIND_ID;
				GetCarKindData(data.CAR_KIND_ID);
				F500102Add.ACC_AREA_ID = data.ACC_AREA_ID;
				F500102Add.DELV_TMPR = data.DELV_TMPR;
				F500102Add.DELV_EFFIC = data.DELV_EFFIC;
				F500102Add.MAX_WEIGHT = data.MAX_WEIGHT;
				F500102Add.OVER_UNIT_FEE = data.OVER_UNIT_FEE;
				F500102Add.OVER_VALUE = data.OVER_VALUE;

				ApprovOver = false;
				if (data.ACC_KIND != "F")
					ApprovOver = true;  // 顯示 貨主核定超標每單位費用 欄位

				DelvAccList = GetDelvAccTypeList(data.ACC_ITEM_KIND_ID);
				F500102Add.DELV_ACC_TYPE = data.DELV_ACC_TYPE;

			}
		}
		#endregion

		#region 取計價單位 List
		public List<NameValuePair<string>> GetAccUnit()
		{

			var proxy = GetProxy<F91Entities>();
			var data = proxy.F91000302s.Where(o => o.ITEM_TYPE_ID == "005").ToList();
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
												 .AddQueryExOption("itemTypeId", "005")
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

		#region 取計費區域 List
		private List<NameValuePair<string>> GetAccAreaList(string dcCode)
		{
			var proxy = GetExProxy<P19ExDataSource>();
			var results = proxy.CreateQuery<F1948Data>("GetF1948").AddQueryExOption("dcCode", dcCode).ToList()
							.Select(x => new NameValuePair<string>()
							{
								Name = x.ACC_AREA,
								Value = x.ACC_AREA_ID.ToString()
							}
							).ToList();
			results.Insert(0, new NameValuePair<string>(Resources.Resources.All, "0"));
			return results;
		}
		#endregion

		#region 取車輛種類 List
		public List<NameValuePair<string>> GetCarKindList()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F194702s.ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.CAR_KIND_NAME,
							Value = o.CAR_KIND_ID.ToString()
						}).ToList();
			return list;
		}
		#endregion

		#region 取車輛種類 -明細
		public void GetCarKindData(decimal? carKindId)
		{
			CarTonne = "";
			Temperature = "";
			var proxy = GetProxy<F19Entities>();
			var proxy00 = GetProxy<F00Entities>();
			var data = proxy.F194702s.Where(o => o.CAR_KIND_ID == carKindId).FirstOrDefault();
			if (data != null)
			{
				CarTonne = data.CAR_SIZE;
				var data00 = proxy00.F000904s.Where(o => o.TOPIC == "F194702" && o.SUBTOPIC == "TMPR_TYPE").FirstOrDefault();
				if (data00 != null)
				{
					Temperature = data00.NAME;
				}
			}
		}
		#endregion

		#region 設定Select Data 資料
		private void SetSelectData()
		{
			if (SelectF500102Data != null)
			{
				if (F500102Add == null) F500102Add = new F500102();
				DcEdit = SelectF500102Data.DC_CODE;
				F500102Add.QUOTE_NO = SelectF500102Data.QUOTE_NO;
				F500102Add.ACC_ITEM_NAME = SelectF500102Data.ACC_ITEM_NAME;
				F500102Add.ENABLE_DATE = SelectF500102Data.ENABLE_DATE;
				F500102Add.DISABLE_DATE = SelectF500102Data.DISABLE_DATE;
				F500102Add.NET_RATE = (float)SelectF500102Data.NET_RATE;
				F500102Add.ACC_NUM = Convert.ToInt16(SelectF500102Data.ACC_NUM);
				F500102Add.ACC_UNIT = SelectF500102Data.ACC_UNIT;
				F500102Add.IN_TAX = SelectF500102Data.IN_TAX;
				F500102Add.ACC_KIND = SelectF500102Data.ACC_KIND;
				F500102Add.APPROV_FEE = SelectF500102Data.APPROV_FEE;
				F500102Add.APPROV_OVER_UNIT_FEE = SelectF500102Data.APPROV_OVER_UNIT_FEE;
				F500102Add.FEE = SelectF500102Data.FEE;
				F500102Add.MEMO = SelectF500102Data.MEMO;
				F500102Add.DC_CODE = SelectF500102Data.DC_CODE;
				F500102Add.GUP_CODE = SelectF500102Data.GUP_CODE;
				F500102Add.CUST_CODE = SelectF500102Data.CUST_CODE;
				F500102Add.ITEM_TYPE_ID = "005";
				F500102Add.ACC_ITEM_KIND_ID = SelectF500102Data.ACC_ITEM_KIND_ID;				
				F500102Add.LOGI_TYPE = SelectF500102Data.LOGI_TYPE;
				F500102Add.IS_SPECIAL_CAR = SelectF500102Data.IS_SPECIAL_CAR;
				F500102Add.CAR_KIND_ID = SelectF500102Data.CAR_KIND_ID;
				F500102Add.ACC_AREA_ID = SelectF500102Data.ACC_AREA_ID;
				F500102Add.DELV_TMPR = SelectF500102Data.DELV_TMPR;
				F500102Add.DELV_EFFIC = SelectF500102Data.DELV_EFFIC;
				F500102Add.MAX_WEIGHT = SelectF500102Data.MAX_WEIGHT;
				F500102Add.OVER_UNIT_FEE = SelectF500102Data.OVER_UNIT_FEE;
				F500102Add.OVER_VALUE = SelectF500102Data.OVER_VALUE;
				F500102Add.DELV_ACC_TYPE = SelectF500102Data.DELV_ACC_TYPE;

				CrtDate = SelectF500102Data.CRT_DATE;
				CrtName = SelectF500102Data.CRT_NAME;
				UpdName = SelectF500102Data.UPD_NAME;
				UpdDate = SelectF500102Data.UPD_DATE;
				//ReUploadBtn();
			}

		}

		#endregion

		#region 儲存資料檢核
		private bool CheckSaveData()
		{
			string errorStr = "";
			if (F500102Add != null)
			{

				if (F500102Add.DISABLE_DATE < F500102Add.ENABLE_DATE)
				{
					errorStr += Properties.Resources.DISABLE_DATE_Invalid;
				}

				if (F500102Add.NET_RATE < 0)
				{
					errorStr += Properties.Resources.NET_RATE_Invalid;
				}


				if (F500102Add.ACC_KIND == "F" && F500102Add.APPROV_FEE.HasValue && F500102Add.APPROV_FEE <= 0)
				{
					errorStr += Properties.Resources.APPROV_FEE_Invalid;
				}

				if (F500102Add.ACC_KIND != "F" && F500102Add.APPROV_OVER_UNIT_FEE.HasValue && F500102Add.APPROV_OVER_UNIT_FEE <= 0)
				{
					errorStr += Properties.Resources.APPROV_OVER_UNIT_FEE_Invalid;
				}

				if (F500102Add.ENABLE_DATE <= DateTime.Today)
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
			F500102Add = new F500102();
			CrtDate = DateTime.Now;
			CrtName = Wms3plSession.Get<UserInfo>().AccountName;
			UpdDate = null;
			UpdName = "";
			F500102Add.DISABLE_DATE = DateTime.Today.AddDays(1);
			F500102Add.ENABLE_DATE = DateTime.Today.AddDays(1);
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
					o => result = DoApprov(), () => UserOperateMode == OperateMode.Edit && SelectF500102Data != null
						&& SelectF500102Data.STATUS != "1"
						, o =>
						{
							if (result)
							{
								QuoteQuery = SelectF500102Data.QUOTE_NO;
								StatusQuery = "1";
								ClearItem();
								SearchCommand.Execute(null);
								F500102Add = null;
							}
						}
					);
			}
		}

		private bool DoApprov()
		{
			if (F500102Add.ACC_KIND == "F")
			{
				if (F500102Add.APPROV_FEE == null || F500102Add.APPROV_FEE < 0)
				{
					DialogService.ShowMessage(Properties.Resources.Write_APPROV_FEE);
					return false;
				}
			}
			else
			{
				if (F500102Add.APPROV_FEE == null || F500102Add.APPROV_OVER_UNIT_FEE == null || F500102Add.APPROV_FEE < 0 || F500102Add.APPROV_OVER_UNIT_FEE < 0)
				{
					DialogService.ShowMessage(Properties.Resources.Write_APPROV_FEE_Or_APPROV_OVER_UNIT_FEE);
					return false;
				}
			}

			var proxy = new wcf.P50WcfServiceClient();
			var wcf500102 = ExDataMapper.Map<F500102, wcf.F500102>(F500102Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF050102Status(wcf500102, "1"));

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
					o => { }, () => UserOperateMode == OperateMode.Query && SelectF500102Data != null
						&& (SelectF500102Data.STATUS == "1" || SelectF500102Data.STATUS == "2") //已核准才可上傳
						&& !string.IsNullOrEmpty(SelectF500102Data.UPLOAD_FILE)
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
					o => DoUpload(), () => UserOperateMode == OperateMode.Edit && SelectF500102Data != null
						&& (SelectF500102Data.STATUS == "1" || SelectF500102Data.STATUS == "2")  //已核准才可上傳
						&& string.IsNullOrEmpty(SelectF500102Data.UPLOAD_FILE)
					, o =>
					{
						AddFile();
						//ReUploadBtn();

						StatusQuery = "2";
						ClearItem();
						SearchCommand.Execute(null);
						F500102Add = null;
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
			if ((SelectFileName == null || string.IsNullOrEmpty(SelectFileName)) && string.IsNullOrEmpty(SelectF500102Data.UPLOAD_FILE))
			{
				DialogService.ShowMessage(Properties.Resources.SelectFile);
			}
			else
			{
				var fileName = SelectF500102Data.QUOTE_NO + Path.GetExtension(SelectFileName);
				//copy 檔案
				if (File.Exists(SelectFileName))
				{
					if (!File.Exists(Path.Combine(FileFolderPath, fileName))) //檔案重複 重取編號
					{
						System.IO.File.Copy(SelectFileName, Path.Combine(FileFolderPath, fileName), true);

						F910404 f910404 = new F910404
						{
							QUOTE_NO = SelectF500102Data.QUOTE_NO,
							DC_CODE = SelectF500102Data.DC_CODE,
							GUP_CODE = SelectF500102Data.GUP_CODE,
							CUST_CODE = SelectF500102Data.CUST_CODE,
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
							SelectF500102Data.UPLOAD_FILE = Path.Combine(FileFolderPath, fileName);

							var proxyf50 = new wcf.P50WcfServiceClient();
							var wcf500102 = ExDataMapper.Map<F500102, wcf.F500102>(F500102Add);
							var f50result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
											() => proxyf50.UpdateF050102Status(wcf500102, "2"));

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
						if (F500102QueryList != null && F500102QueryList.Any())
						{
							SelectF500102Data = F500102QueryList.First();
						}
					});
			}
		}

		private void DoSearch()
		{
			var proxyEx = GetExProxy<P50ExDataSource>();
			var f500102QueryData = proxyEx.CreateQuery<F500102QueryData>("GetF500102QueryData")
					.AddQueryOption("dcCode", string.Format("'{0}'", DcQuery))
					.AddQueryOption("enableSDate", string.Format("'{0}'", EnableSDataQuery.ToString()))
					.AddQueryOption("enableEDate", string.Format("'{0}'", EnableEDataQuery.ToString()))
					.AddQueryOption("quoteNo", string.Format("'{0}'", QuoteQuery))
					.AddQueryOption("status", string.Format("'{0}'", StatusQuery));

			F500102QueryList = f500102QueryData.ToObservableCollection();

			if (F500102QueryList == null || F500102QueryList.Count() == 0)
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
						, o =>
						{
							ResetUI();
						});
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
						&& SelectF500102Data != null
						&& SelectF500102Data.STATUS != "9"
						&& SelectF500102Data.ENABLE_DATE > DateTime.Today,
					o =>
					{
						SetSelectData();

					});
			}
		}

		private void DoEdit()
		{
			if (SelectF500102Data == null)
			{
				DialogService.ShowMessage(Properties.Resources.SelectEditData);
				return;
			}
			UserOperateMode = OperateMode.Edit;
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
			F500102Add = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectF500102Data != null
						 && SelectF500102Data.STATUS == "0"
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
								() => proxy.DeleteF500102(SelectF500102Data.DC_CODE, SelectF500102Data.GUP_CODE
															, SelectF500102Data.CUST_CODE, SelectF500102Data.QUOTE_NO));

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
					&& !(UserOperateMode == OperateMode.Edit && SelectF500102Data != null && SelectF500102Data.STATUS != "0")
					, o =>
					{
						if (result)
						{
							QuoteQuery = "";
							if (UserOperateMode == OperateMode.Edit)
								QuoteQuery = SelectF500102Data.QUOTE_NO;

							EnableSDataQuery = F500102Add.ENABLE_DATE;
							EnableEDataQuery = F500102Add.DISABLE_DATE;
							DcQuery = DcEdit;
							StatusQuery = "0";
							ClearItem();
							SearchCommand.Execute(null);
							F500102Add = null;
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
			F500102Add.ITEM_TYPE_ID = "005";
			F500102Add.ACC_ITEM_KIND_ID = GetAccItemKindId(DcEdit, F500102Add.ACC_ITEM_NAME);
			F500102Add.DC_CODE = DcEdit;
			F500102Add.GUP_CODE = _gupCode;
			F500102Add.CUST_CODE = _custCode;
			var proxy = new wcf.P50WcfServiceClient();
			var wcf500102 = ExDataMapper.Map<F500102, wcf.F500102>(F500102Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.InsertF500102(wcf500102));

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

			F500102Add.ACC_ITEM_KIND_ID = GetAccItemKindId(DcEdit, F500102Add.ACC_ITEM_NAME);
			F500102Add.DC_CODE = DcEdit;
			F500102Add.GUP_CODE = _gupCode;
			F500102Add.CUST_CODE = _custCode;
			var proxy = new wcf.P50WcfServiceClient();
			var wcf500102 = ExDataMapper.Map<F500102, wcf.F500102>(F500102Add);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF500102(wcf500102));

			if (result.IsSuccessed)
			{
				ShowMessage(Messages.InfoUpdateSuccess);
			}

			return result.IsSuccessed;
		}

		#endregion Save
	}
}
