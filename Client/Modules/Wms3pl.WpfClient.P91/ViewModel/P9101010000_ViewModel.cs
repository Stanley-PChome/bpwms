using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.P91.Services;
using Wms3pl.WpfClient.DataServices.F91DataService;
using System.Windows.Media.Imaging;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.UILib.Services;
using System.Reflection;
using Wms3pl.WpfClient.DataServices.F15DataService;

namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9101010000_ViewModel : InputViewModelBase
	{
		public Action<PrintType> DoPrintReport = delegate { };
		public Action<PrintType> DoPrintPickTicketReport = delegate { };
		public Action actionAfterCreatePickTicket = delegate { };
		public Action actionForCreatePickTicket = delegate { };
		public Action actionForAfterUpdate = delegate { };
		public Action OnEditFocus = delegate { };
		public P9101010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				GetF1928();
				GetCommonData();
				InitialValue();
			}

		}

		#region 資料連結
		#region Form - DC/ GUP/ CUST/ 預計完工日(起迄)
		private string _selectedDc { get; set; }
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value; RaisePropertyChanged("SelectedDC"); }
		}
		private string _selectedGup = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _selectedCust = Wms3plSession.Get<GlobalInfo>().CustCode;

		private DateTime _startFinishDt = DateTime.Today;
		public DateTime StartFinishDt
		{
			get { return _startFinishDt; }
			set { _startFinishDt = value; RaisePropertyChanged("StartFinishDt"); }
		}

		private DateTime _endFinishDt = DateTime.Today;
		public DateTime EndFinishDt
		{
			get { return _endFinishDt; }
			set { _endFinishDt = value; RaisePropertyChanged("EndFinishDt"); }
		}

		#endregion
		#region Form - 加工單編號/ 單據狀態/ 委外商/ 成品編號
		private string _processNo = string.Empty;
		public string ProcessNo
		{
			get { return _processNo; }
			set { _processNo = value; RaisePropertyChanged("ProcessNo"); }
		}

		private string _processStatus = "-1";
		public string ProcessStatus
		{
			get { return _processStatus; }
			set { _processStatus = value; RaisePropertyChanged("ProcessStatus"); }
		}

		private string _processType = "";
		public string ProcessType
		{
			get { return _processType; }
			set { _processType = value; RaisePropertyChanged("ProcessType"); }
		}

		private string _outsourceId = string.Empty;
		public string OutsourceId
		{
			get { return _outsourceId; }
			set { _outsourceId = value; RaisePropertyChanged("OutsourceId"); }
		}

		private string _itemCode = string.Empty;
		public string ItemCode
		{
			get { return _itemCode; }
			set { _itemCode = value; RaisePropertyChanged("ItemCode"); }
		}
		#endregion
		#region Data - DC/ 委外商/ 單據狀態/ 加工單來源
		private List<NameValuePair<string>> _dcList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}

		private List<NameValuePair<string>> _outsourceList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> OutsourceList
		{
			get { return _outsourceList; }
			set { _outsourceList = value; RaisePropertyChanged("OutsourceList"); }
		}

		private List<NameValuePair<string>> _processStatusList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> ProcessStatusList
		{
			get { return _processStatusList; }
			set { _processStatusList = value; RaisePropertyChanged("ProcessStatusList"); }
		}

		private List<NameValuePair<string>> _processTypeList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> ProcessTypeList
		{
			get { return _processTypeList; }
			set { _processTypeList = value; RaisePropertyChanged("ProcessTypeList"); }
		}

		private List<NameValuePair<string>> _processSourceList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> ProcessSourceList
		{
			get { return _processSourceList; }
			set { _processSourceList = value; RaisePropertyChanged("ProcessSourceList"); }
		}
		#endregion
		#region Data - 流通加工單資料/ 選擇的加工單/ 編輯中的加工單
		private ObservableCollection<F910201> _f910201Data = new ObservableCollection<F910201>();
		public ObservableCollection<F910201> F910201Data
		{
			get { return _f910201Data; }
			set { _f910201Data = value; RaisePropertyChanged("F910201Data"); }
		}

		private F910201 _selectedF910201;
		public F910201 SelectedF910201
		{
			get { return _selectedF910201; }
			set
			{
				_selectedF910201 = value;
				RaisePropertyChanged("SelectedF910201");

				ItemImageSource = null;
				UnfinishedCount = string.Empty;
				if (value != null) DoSearchDetail(value, 0);
			}
		}

		private F910201 _editableF910201 = null;
		public F910201 EditableF910201
		{
			get { return _editableF910201; }
			set
			{
				_editableF910201 = value;
				ItemImageSource = null;
				UnfinishedCount = string.Empty;
				if (value != null) DoSearchDetail(value, 1);
				RaisePropertyChanged("EditableF910201");
			}
		}

		private Boolean _editf910205 = true;
		public Boolean Editf910205
		{
			get { return _editf910205; }
			set
			{
				_editf910205 = value;
				RaisePropertyChanged("Editf910205");
			}
		}

		private Boolean _editable = true;
		public Boolean Editable
		{
			get { return _editable; }
			set
			{
				_editable = value;
				RaisePropertyChanged("Editable");
			}
		}

		private Boolean _editreadonly = false;
		public Boolean Editreadonly
		{
			get { return _editreadonly; }
			set
			{
				_editreadonly = value;
				RaisePropertyChanged("Editreadonly");
			}
		}

		private Boolean _editreadonlyDesc = false;
		public Boolean EditreadonlyDesc
		{
			get { return _editreadonlyDesc; }
			set
			{
				_editreadonlyDesc = value;
				RaisePropertyChanged("EditreadonlyDesc");
			}
		}

		#endregion
		#region Data - 詳細資料 (加工項目/ 報價動作/ 耗材資訊)
		private List<F910401> _f910401Data = new List<F910401>();
		public List<F910401> F910401Data
		{
			get { return _f910401Data; }
			set { _f910401Data = value; RaisePropertyChanged("F910401Data"); }
		}

		private List<F910402Detail> _f910402Data = new List<F910402Detail>();
		public List<F910402Detail> F910402Data
		{
			get { return _f910402Data; }
			set { _f910402Data = value; RaisePropertyChanged("F910402Data"); }
		}

		private List<F910403Detail> _f910403Data = new List<F910403Detail>();
		public List<F910403Detail> F910403Data
		{
			get { return _f910403Data; }
			set { _f910403Data = value; RaisePropertyChanged("F910403Data"); }
		}

		private List<F910401> _editableF910401Data = new List<F910401>();
		public List<F910401> EditableF910401Data
		{
			get { return _editableF910401Data; }
			set { _editableF910401Data = value; RaisePropertyChanged("EditableF910401Data"); }
		}

		private List<F910402Detail> _editableF910402Data = new List<F910402Detail>();
		public List<F910402Detail> EditableF910402Data
		{
			get { return _editableF910402Data; }
			set { _editableF910402Data = value; RaisePropertyChanged("EditableF910402Data"); }
		}

		private List<F910403Detail> _editableF910403Data = new List<F910403Detail>();
		public List<F910403Detail> EditableF910403Data
		{
			get { return _editableF910403Data; }
			set { _editableF910403Data = value; RaisePropertyChanged("EditableF910403Data"); }
		}

		private List<P910101Report> _reportData = new List<P910101Report>();
		public List<P910101Report> ReportData
		{
			get { return _reportData; }
			set { _reportData = value; RaisePropertyChanged("ReportData"); }
		}
		#endregion
		#region Form - 商品圖檔/ 商品名稱/ 加工品名稱/ 未完成數
		private BitmapImage _itemImageSource = null;
		/// <summary>
		/// 顯示圖片
		/// Memo: 可用此方式來避免圖檔被程式咬住而無法刪除或移動
		/// </summary>
		public BitmapImage ItemImageSource
		{
			get
			{
				return _itemImageSource;
			}
			set
			{
				DispatcherAction(() =>
				{
					if (UserOperateMode == OperateMode.Query)
					{
						if (SelectedF910201 == null || string.IsNullOrWhiteSpace(SelectedF910201.ITEM_CODE)) _itemImageSource = null;
						else
						{
							_itemImageSource = FileService.GetItemImage(_selectedGup, _selectedCust, SelectedF910201.ITEM_CODE);
						}
					}
					else
					{
						if (EditableF910201 == null || string.IsNullOrWhiteSpace(EditableF910201.ITEM_CODE)) _itemImageSource = null;
						else
						{
							_itemImageSource = FileService.GetItemImage(_selectedGup, _selectedCust, EditableF910201.ITEM_CODE);
						}
					}
					RaisePropertyChanged("ItemImageSource");
				});
			}
		}

		private BitmapImage _editableItemImageSource = null;
		/// <summary>
		/// 顯示圖片
		/// Memo: 可用此方式來避免圖檔被程式咬住而無法刪除或移動
		/// </summary>
		public BitmapImage EditableItemImageSource
		{
			get
			{
				return _editableItemImageSource;
			}
			set
			{
				DispatcherAction(() =>
				{
					if (EditableF910201 == null || string.IsNullOrWhiteSpace(EditableF910201.ITEM_CODE)) _itemImageSource = null;
					else
					{
						_itemImageSource = FileService.GetItemImage(_selectedGup, _selectedCust, EditableF910201.ITEM_CODE);
					}
					RaisePropertyChanged("EditableItemImageSource");
				});
			}
		}

		private string _editableItemName = string.Empty;
		public string EditableItemName
		{
			get { return _editableItemName; }
			set { _editableItemName = value; RaisePropertyChanged("EditableItemName"); }
		}

		private string _editableItemBomName = string.Empty;
		public string EditableItemBomName
		{
			get { return _editableItemBomName; }
			set { _editableItemBomName = value; RaisePropertyChanged("EditableItemBomName"); }
		}

		private string _editableUnfinishedCount = string.Empty;
		public string EditableUnfinishedCount
		{
			get { return _editableUnfinishedCount; }
			set { _editableUnfinishedCount = value; RaisePropertyChanged("EditableUnfinishedCount"); }
		}

		private string _itemName = string.Empty;
		public string ItemName
		{
			get { return _itemName; }
			set { _itemName = value; RaisePropertyChanged("ItemName"); }
		}

		private string _itemBomName = string.Empty;
		public string ItemBomName
		{
			get { return _itemBomName; }
			set { _itemBomName = value; RaisePropertyChanged("ItemBomName"); }
		}

		private string _unfinishedCount = string.Empty;
		public string UnfinishedCount
		{
			get { return _unfinishedCount; }
			set { _unfinishedCount = value; RaisePropertyChanged("UnfinishedCount"); }
		}
		#endregion
		#region Data - 選擇的加工項目/ 符合的動作分析/ 符合的耗材統計
		private F910401 _selectedF910401 = null;
		public F910401 SelectedF910401
		{
			get { return _selectedF910401; }
			set
			{
				_selectedF910401 = value;
				if (value != null)
				{
					F910401OutsourceName = DoGetOursourceName(value.OUTSOURCE_ID);
					// 取得符合第一個報價單項目的動作清單
					F910402Data = GetF910402(value.DC_CODE, value.GUP_CODE, value.CUST_CODE, value.QUOTE_NO);
					// 取得符合第一個報價單項目的耗材資訊
					F910403Data = GetF910403(value.DC_CODE, value.GUP_CODE, value.CUST_CODE, value.QUOTE_NO);
				}
				else F910401OutsourceName = string.Empty;
				RaisePropertyChanged("SelectedF910401");
			}
		}

		private F910401 _editableSelectedF910401 = null;
		public F910401 EditableSelectedF910401
		{
			get { return _editableSelectedF910401; }
			set
			{
				_editableSelectedF910401 = value;
				if (value != null)
				{
					EditableF910401OutsourceName = DoGetOursourceName(value.OUTSOURCE_ID);
					// 取得符合第一個報價單項目的動作清單
					//EditableF910402Data = GetF910402(value.DC_CODE, EditableF910201.GUP_CODE, value.QUOTE_NO);
					EditableF910402Data = GetF910402(value.DC_CODE, value.GUP_CODE, value.CUST_CODE, value.QUOTE_NO);
					// 取得符合第一個報價單項目的耗材資訊
					//EditableF910403Data = GetF910403(value.DC_CODE, EditableF910201.GUP_CODE, value.QUOTE_NO);
					EditableF910403Data = GetF910403(value.DC_CODE, value.GUP_CODE, value.CUST_CODE, value.QUOTE_NO);
				}
				else EditableF910401OutsourceName = string.Empty;
				RaisePropertyChanged("EditableSelectedF910401");
			}
		}


		private string _editableF910401OutsourceName = string.Empty;
		public string EditableF910401OutsourceName
		{
			get { return _editableF910401OutsourceName; }
			set { _editableF910401OutsourceName = value; RaisePropertyChanged("EditableF910401OutsourceName"); }
		}

		private string _f910401OutsourceName = string.Empty;
		public string F910401OutsourceName
		{
			get { return _f910401OutsourceName; }
			set { _f910401OutsourceName = value; RaisePropertyChanged("F910401OutsourceName"); }
		}

		#endregion
		
		#endregion
		#region Command
		#region Search - 查詢流通加工單資料
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
				);
			}
		}

		private void DoSearch()
		{
			ClearData();
			//執行查詢動作
			var proxy = GetProxy<F91Entities>();
			List<F910201> data;
			if (ProcessStatus != "-1")
			{
				// 選擇單一狀態時
				data = proxy.F910201s.Where(x => x.DC_CODE == SelectedDc
					  && x.GUP_CODE == _selectedGup
					  && x.CUST_CODE == _selectedCust
					  && x.FINISH_DATE >= StartFinishDt
					  && x.FINISH_DATE < EndFinishDt.AddDays(1)
					  && (x.ITEM_CODE == ItemCode || string.IsNullOrWhiteSpace(ItemCode))
					  && (x.OUTSOURCE_ID == OutsourceId || OutsourceId == "-1")
					  && x.STATUS == ProcessStatus
					  && ((x.PROC_TYPE == ProcessType && !string.IsNullOrEmpty(ProcessType)) || (string.IsNullOrEmpty(ProcessType) && 1 == 1))
					  && (x.PROCESS_NO == ProcessNo || string.IsNullOrWhiteSpace(ProcessNo))).OrderBy(x => x.FINISH_DATE).ThenBy(x => x.PROCESS_NO).ToList();
			}
			else
			{
				// 選擇全部時, 排除"取消"的加工單
				data = proxy.F910201s.Where(x => x.DC_CODE == SelectedDc
					  && x.GUP_CODE == _selectedGup
					  && x.CUST_CODE == _selectedCust
					  && x.FINISH_DATE >= StartFinishDt
					  && x.FINISH_DATE < EndFinishDt.AddDays(1)
					  && (x.ITEM_CODE == ItemCode || string.IsNullOrWhiteSpace(ItemCode))
					  && (x.OUTSOURCE_ID == OutsourceId || OutsourceId == "-1")
					  && x.STATUS != "9"
					  && ((x.PROC_TYPE == ProcessType && !string.IsNullOrEmpty(ProcessType)) || (string.IsNullOrEmpty(ProcessType) && 1 == 1))
					  && (x.PROCESS_NO == ProcessNo || string.IsNullOrWhiteSpace(ProcessNo))).OrderBy(x => x.FINISH_DATE).ThenBy(x => x.PROCESS_NO).ToList();
			}
			if (data == null || !data.Any())
			{
				ShowMessage(Messages.InfoNoData);
			}
			F910201Data = data.ToObservableCollection();
		}

		private void DoSearchComplete()
		{
			// 查詢完成後要執行的動作
			//actionForAfterUpdate();
			SelectedF910201 = F910201Data.FirstOrDefault();
		}

		private void ClearData()
		{
			SelectedF910201 = null;
			EditableF910201 = null;
			F910402Data = null;
			F910403Data = null;
			F910401Data = null;
			ItemName = string.Empty;
		}

		#endregion Search

		#region Search - 查詢加工單明細資料
		public ICommand SearchDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchDetail(SelectedF910201, 0),
					() => true,
					o => DoSearchDetailComplete()
				);
			}
		}
		private void DoSearchDetail(F910201 data, int srcType = 0)
		{
			var proxy = GetProxy<F91Entities>();
			var proxy2 = GetProxy<F19Entities>();

			// 取得報價單項目
			// Memo: 也許可以提到畫面載入時就讀取, 但可能操作者一直都沒關畫面, 也許會造成新資料讀不到. 所以一樣保留在這裡每次點選時做查詢.
			if (srcType == 0)
			{
				// 計算未完成數
				UnfinishedCount = (data.PROCESS_QTY - data.A_PROCESS_QTY-data.BREAK_QTY).ToString();

				F910401Data = new List<F910401>();
				EditableF910401Data = new List<F910401>();
				F910402Data = new List<F910402Detail>();
				F910403Data = new List<F910403Detail>();

				//GetItemName 會抓設定檔給 最小數量箱/盒 , 要以主檔為主


				// 取得商品名稱
				ItemName = GetOnlyItemName(_selectedGup, data.ITEM_CODE, _selectedCust);
				ItemBomName = GetBomName(_selectedGup, _selectedCust, data.ITEM_CODE_BOM);




				var tmpF910401 = proxy.F910401s.Where(x => (x.DC_CODE == SelectedDc || x.DC_CODE == "000") && x.GUP_CODE == _selectedGup && x.CUST_CODE == _selectedCust && x.QUOTE_NO == data.QUOTE_NO).ToList();
				if (tmpF910401.Any())
				{
					var proxyEx = GetExProxy<P91ExDataSource>();
					F910401Data = tmpF910401.OrderBy(x => x.QUOTE_NO).ToList();
					SelectedF910401 = F910401Data.FirstOrDefault();
					var tmpQuoteNo = SelectedF910401.QUOTE_NO;
					// 取得符合第一個報價單項目的動作清單
					F910402Data = GetF910402(SelectedF910401.DC_CODE, SelectedF910401.GUP_CODE, SelectedF910401.CUST_CODE, SelectedF910401.QUOTE_NO);
					// 取得符合第一個報價單項目的耗材資訊
					F910403Data = GetF910403(SelectedF910401.DC_CODE, SelectedF910401.GUP_CODE, SelectedF910401.CUST_CODE, SelectedF910401.QUOTE_NO);
				}
			}
			else
			{
				// 計算未完成數
				EditableUnfinishedCount = (data.PROCESS_QTY - data.A_PROCESS_QTY - data.BREAK_QTY).ToString();

				EditableF910401Data = new List<F910401>();
				EditableF910402Data = new List<F910402Detail>();
				EditableF910403Data = new List<F910403Detail>();
				var tmpF910401 = new List<F910401>();
				// 取得商品名稱
				EditableItemName = GetOnlyItemName(_selectedGup, data.ITEM_CODE, _selectedCust);
				EditableItemBomName = GetBomName(_selectedGup, _selectedCust, data.ITEM_CODE_BOM);
				if (UserOperateMode == OperateMode.Add)
				{
					//取得貨主統一編號
					var tmpF1909 =
					  proxy2.F1909s.Where(x => x.CUST_CODE == _selectedCust && x.GUP_CODE == _selectedGup).FirstOrDefault();
					// Memo: Disable_Date必須要大於今天
					var proxyF91 = GetProxy<F91Entities>();
					tmpF910401 = proxyF91.CreateQuery<F910401>("GetF910301WithF910401")
					  .AddQueryExOption("gupCode", _selectedGup)
					  .AddQueryExOption("dcCode", SelectedDc)
					  .AddQueryExOption("uniForm", tmpF1909.UNI_FORM)
					  .AddQueryExOption("enableDate", DateTime.Today.ToString("yyyy/MM/dd"))
					  .ToList();
				}
				else
				{
					tmpF910401 = proxy.F910401s.Where(x => (x.DC_CODE == SelectedDc || x.DC_CODE == "000") && x.GUP_CODE == _selectedGup && x.CUST_CODE == _selectedCust && x.QUOTE_NO == data.QUOTE_NO).ToList();
				}
				if (tmpF910401.Any())
				{
					var proxyEx = GetExProxy<P91ExDataSource>();
					EditableF910401Data = tmpF910401.OrderBy(x => x.QUOTE_NO).ToList();

					// 只有在新增時才帶到第一項
					if (UserOperateMode == OperateMode.Add)
					{
						EditableSelectedF910401 = EditableF910401Data.FirstOrDefault();
						var tmpQuoteNo = EditableSelectedF910401.QUOTE_NO;
					}
					else
					{
						EditableSelectedF910401 =
						  EditableF910401Data.Where(
							x =>
							  x.QUOTE_NO == EditableF910201.QUOTE_NO && (x.DC_CODE == EditableF910201.DC_CODE || x.DC_CODE == "000") &&
								x.GUP_CODE == EditableF910201.GUP_CODE && x.CUST_CODE == EditableF910201.CUST_CODE).FirstOrDefault();
					}
					if (EditableSelectedF910401 != null)
					{
						// 取得符合第一個報價單項目的動作清單
						EditableF910402Data = GetF910402(EditableSelectedF910401.DC_CODE, EditableSelectedF910401.GUP_CODE, EditableSelectedF910401.CUST_CODE,
					EditableSelectedF910401.QUOTE_NO);
						// 取得符合第一個報價單項目的耗材資訊
						EditableF910403Data = GetF910403(EditableSelectedF910401.DC_CODE, EditableSelectedF910401.GUP_CODE, EditableSelectedF910401.CUST_CODE,
					EditableSelectedF910401.QUOTE_NO);
					}
				}
				//var proxyF91 = GetExProxy<P91ExDataSource>();
				//var tmpF910401 = proxyF91.CreateQuery<F910401>("GetF910301WithF910401")
				//.AddQueryOption("gupCode", string.Format("'{0}'", _selectedGup))
				//.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				//.AddQueryOption("uniForm", string.Format("'{0}'", tmpF1909.UNI_FORM))			
				//.ToList();

			}
		}

		private List<F910402Detail> GetF910402(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var proxy = GetExProxy<P91ExDataSource>();
			var result = proxy.CreateQuery<F910402Detail>("GetF910402Detail")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("quoteNo", quoteNo)
				.AsQueryable();
			return result.ToList();
		}
		private List<F910403Detail> GetF910403(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var proxy = GetExProxy<P91ExDataSource>();
			var result = proxy.CreateQuery<F910403Detail>("GetF910403Detail")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("quoteNo", quoteNo)
				.AsQueryable();
			return result.ToList();
		}

		private void DoSearchDetailComplete()
		{

		}

		private string DoGetOursourceName(string outsourceId)
		{
			var proxy = GetProxy<F19Entities>();
			var tmp = proxy.F1928s.Where(x => x.OUTSOURCE_ID == outsourceId).FirstOrDefault();
			if (tmp == null) return string.Empty;
			return tmp.OUTSOURCE_NAME;
		}
		#endregion

		#region 取得下拉選單資料/ 取得商品名稱
		private void GetCommonData()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			ProcessStatusList = P91CommonDataHelper.ProcessStatusList(FunctionCode);
			ProcessSourceList = P91CommonDataHelper.ProcessSourceList(FunctionCode);
			ProcessTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F910201", "PROC_TYPE", true);
		}
		private void GetF1928()
		{
			var proxy = GetProxy<F19Entities>();
			var tmp = proxy.F1928s.Where(x => x.STATUS != "9").ToList();
			OutsourceList = tmp.OrderBy(x => x.OUTSOURCE_NAME).Select(x => new NameValuePair<string>()
			{
				Name = x.OUTSOURCE_NAME,
				Value = x.OUTSOURCE_ID
			}).ToList();
			OutsourceList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = "-1" });
		}
		private void InitialValue()
		{
			SelectedDc = (DcList.FirstOrDefault() == null) ? string.Empty : DcList.FirstOrDefault().Value;
			ProcessStatus = (ProcessStatusList.FirstOrDefault() == null) ? String.Empty : ProcessStatusList.FirstOrDefault().Value;
			OutsourceId = (OutsourceList.FirstOrDefault() == null) ? string.Empty : OutsourceList.FirstOrDefault().Value;
		}

		public string GetOnlyItemName(string gupCode, string itemCode, string custCode)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1903s.Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.CUST_CODE == custCode).FirstOrDefault();
			if (data == null)
			{
				return string.Empty;
			}
			DispatcherAction(() =>
			{
				ItemImageSource = null;
			});
			return data.ITEM_NAME;
		}

        /// <summary>
        /// 取得商品名稱
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public string GetItemName(string gupCode, string itemCode, string custCode)
		{

			if (EditableF910201 != null)
			{
				EditableF910201.CASE_QTY = 0;
				EditableF910201.BOX_QTY = 0;
			}
			var proxy = GetProxy<F19Entities>();
			var proxy91 = GetProxy<F91Entities>();
			var data = proxy.F1903s.Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.CUST_CODE == custCode).FirstOrDefault();
			if (data == null)
			{
				return string.Empty;
			}
			// 取得盒/箱最小單位數
			var unitF91000302 = proxy91.F91000302s.Where(x => x.ACC_UNIT_NAME == Properties.Resources.P9101010000_ViewModel_Box && x.ITEM_TYPE_ID == "001").FirstOrDefault();
			var boxData = proxy.F190301s.Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.UNIT_ID == unitF91000302.ACC_UNIT).FirstOrDefault();
			if (SelectedF910201 != null && boxData != null)
			{
				//SelectedF910201.BOX_QTY = boxData.UNIT_QTY;
			}
			if (EditableF910201 != null && boxData != null)
			{
				EditableF910201.BOX_QTY = boxData.UNIT_QTY;
			}
			unitF91000302 = proxy91.F91000302s.Where(x => x.ACC_UNIT_NAME == Resources.Resources.Box && x.ITEM_TYPE_ID == "001").FirstOrDefault();
			boxData = proxy.F190301s.Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.UNIT_ID == unitF91000302.ACC_UNIT).FirstOrDefault();
			if (SelectedF910201 != null && boxData != null)
			{
				//SelectedF910201.CASE_QTY = boxData.UNIT_QTY;
			}
			if (EditableF910201 != null && boxData != null)
			{
				EditableF910201.CASE_QTY = boxData.UNIT_QTY;
			}
			ItemImageSource = null;
			return data.ITEM_NAME;
		}

		/// <summary>
		/// 取得組合說明
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public string GetBomName(string gupCode, string custCode, string itemCode)
		{
			var proxy = GetProxy<F91Entities>();
			var data = proxy.F910101s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.BOM_NO == itemCode).FirstOrDefault();
			if (data == null) return string.Empty;
			return data.BOM_NAME;
		}
		#endregion

		#region 檢查加工項目生效日期是否符合完工日期
		/// <summary>
		/// 檢查加工項目生效日期是否符合完工日期
		/// </summary>
		/// <returns></returns>
		public bool IsValidF910401()
		{
			if (EditableSelectedF910401 == null) return true;
			if (EditableF910201 == null) return true;
			if (EditableSelectedF910401.ENABLE_DATE <= EditableF910201.FINISH_DATE
				&& EditableSelectedF910401.DISABLE_DATE >= EditableF910201.FINISH_DATE) return true;
			ShowMessage(Messages.WarningNotValidFinishDate);
			return false;
		}
		#endregion

		#region Add - 新增加工單
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
										o => DoAdd(), () => UserOperateMode == OperateMode.Query, o => DoAddComplete()
					);
			}
		}

		private void DoAdd()
		{
			//執行新增動作, 新增時上方的按鈕都不可以按
			UserOperateMode = OperateMode.Add;
			Editable = true;
			Editreadonly = false;
			EditreadonlyDesc = false;
		}
		private void DoAddComplete()
		{
			EditableF910201 = new F910201()
			{
				FINISH_DATE = DateTime.Today,
				FINISH_TIME = DateTime.Now.ToString("HH:mm"),
				DC_CODE = SelectedDc,
				GUP_CODE = _selectedGup,
				CUST_CODE = _selectedCust,
				PROCESS_SOURCE = ProcessSourceList.FirstOrDefault()?.Value,
				OUTSOURCE_ID = OutsourceList.FirstOrDefault(x => x.Value != "-1")?.Value
			};
		}
		#endregion

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedF910201 != null && SelectedF910201.STATUS != "9"
					);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
			UserOperateMode = OperateMode.Edit;
			SetEditableF910201();
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() => UserOperateMode == OperateMode.Add || UserOperateMode == OperateMode.Edit
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				if (UserOperateMode == OperateMode.Add)
				{
					ClearData();
				}
				UserOperateMode = OperateMode.Query;
			}
		}
		#endregion Cancel

		#region Delete - 刪除加工單, 僅在狀態為"未加工"時才能刪除
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query
						&& SelectedF910201 != null
						&& SelectedF910201.STATUS == "0"
						&& SelectedF910201.PROC_STATUS == "0"
				);
			}
		}

		/// <summary>
		/// 刪除加工單, 僅在狀態為"未加工"時才能刪除
		/// </summary>
		private void DoDelete()
		{
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			var proxy = GetExProxy<P91ExDataSource>();

			var result = proxy.CreateQuery<ExecuteResult>("DeleteF910201")
			  .AddQueryExOption("processNo", SelectedF910201.PROCESS_NO)
			  .AddQueryExOption("gupCode", SelectedF910201.GUP_CODE)
			  .AddQueryExOption("custCode", SelectedF910201.CUST_CODE)
			  .AddQueryExOption("dcCode", SelectedDc).ToList();

			ShowMessage(result);
			//ShowMessage(Messages.InfoDeleteSuccess);	
			if (result.FirstOrDefault().IsSuccessed != true) return;
			ProcessNo = string.Empty;
			DoSearch();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
		  () => (UserOperateMode == OperateMode.Add || (UserOperateMode == OperateMode.Edit && SelectedF910201.STATUS == "0" && Editf910205)) && EditableF910201 != null,
					o => LocateCurrentData()
					);
			}
		}

		private bool _isSaved = false;
		private void DoSave()
		{
			//執行確認儲存動作
			var isPass = CheckDataBeforeSave();
			if (!string.IsNullOrWhiteSpace(isPass))
			{
				ShowMessage(new MessagesStruct() { Button = UILib.Services.DialogButton.OK, Image = UILib.Services.DialogImage.Information, Message = isPass, Title = Resources.Resources.Information });
				return;
			}

			if (!IsValidF910401()) return;

			if (!CheckOrderNo()) return;

			if (UserOperateMode == OperateMode.Add)
				DoInsert();
			else
				DoUpdate();
		}

		/// <summary>
		/// 新增資料時跑這一段
		/// </summary>
		private void DoInsert()
		{
			_isSaved = false;
			var proxy = GetExProxy<P91ExDataSource>();
			var result = proxy.CreateQuery<ExecuteResult>("InsertF910201")
		.AddQueryExOption("dcCode", EditableF910201.DC_CODE)
		.AddQueryExOption("gupCode", EditableF910201.GUP_CODE)
		.AddQueryExOption("custCode", EditableF910201.CUST_CODE)
		.AddQueryExOption("processSource", EditableF910201.PROCESS_SOURCE)
		.AddQueryExOption("outsourceId", EditableF910201.OUTSOURCE_ID)
		.AddQueryExOption("finishDate", EditableF910201.FINISH_DATE.ToString("yyyy/MM/dd"))
		.AddQueryExOption("itemCode", EditableF910201.ITEM_CODE)
		.AddQueryExOption("itemCodeBom", EditableF910201.ITEM_CODE_BOM)
		.AddQueryExOption("processQty", EditableF910201.PROCESS_QTY)
		.AddQueryExOption("boxQty", EditableF910201.BOX_QTY)
		.AddQueryExOption("caseQty", EditableF910201.CASE_QTY)
		.AddQueryExOption("orderNo", EditableF910201.ORDER_NO)
		.AddQueryExOption("quoteNo", EditableSelectedF910401.QUOTE_NO)
		.AddQueryExOption("memo", EditableF910201.MEMO)
		.AddQueryExOption("finishTime", Convert.ToDateTime(EditableF910201.FINISH_TIME).ToString("HH:mm")).ToList();

			ShowMessage(result);
			if (result.FirstOrDefault().IsSuccessed == true)
			{
				_isSaved = true;
				EditableF910201.PROCESS_NO = result.FirstOrDefault().Message;
				EditableF910201.STATUS = "0";
				EditableF910201.OUTSOURCE_ID = null;
				// LocateCurrentData();
				//UserOperateMode = OperateMode.Query;
			}
		}

		/// <summary>
		/// 更新資料時跑這一段
		/// </summary>
		private void DoUpdate()
		{
			_isSaved = false;
			var checkPickData = CheckF910205();
			if (checkPickData == 2 && SelectedF910201.PROCESS_QTY != EditableF910201.PROCESS_QTY)
			{
				// 揀料單開始揀貨則不可修改加工數量
				ShowMessage(Messages.WarningCannotUpdateProcessQty);
				return;
			}
			if (checkPickData == 1 && SelectedF910201.PROCESS_QTY != EditableF910201.PROCESS_QTY)
			{
				if (ShowMessage(Messages.WarningBeforeUpdateProcessQty) == UILib.Services.DialogResponse.No)
					return;
			}
			// 準備更新資料, 更新前再檢查一次加工單狀態
			var processStatus = RecheckProcessStatus();
			if (!processStatus)
			{
				ShowMessage(Messages.WarningCannotUpdateF910201);
				return;
			}
			var proxy = GetExProxy<P91ExDataSource>();

			var result = proxy.CreateQuery<ExecuteResult>("UpdateF910201")
				.AddQueryExOption("dcCode", EditableF910201.DC_CODE)
				.AddQueryExOption("gupCode", EditableF910201.GUP_CODE)
				.AddQueryExOption("custCode", EditableF910201.CUST_CODE)
				.AddQueryExOption("processNo", EditableF910201.PROCESS_NO)
				.AddQueryExOption("processSource", EditableF910201.PROCESS_SOURCE)
				.AddQueryExOption("outsourceId", EditableF910201.OUTSOURCE_ID)
				.AddQueryExOption("finishDate", EditableF910201.FINISH_DATE.ToString("yyyy/MM/dd"))
				.AddQueryExOption("itemCode", EditableF910201.ITEM_CODE)
				.AddQueryExOption("itemCodeBom", EditableF910201.ITEM_CODE_BOM)
				.AddQueryExOption("processQty", EditableF910201.PROCESS_QTY)
				.AddQueryExOption("boxQty", EditableF910201.BOX_QTY)
				.AddQueryExOption("caseQty", EditableF910201.CASE_QTY)
				.AddQueryExOption("orderNo", EditableF910201.ORDER_NO)
				.AddQueryExOption("quoteNo", EditableSelectedF910401.QUOTE_NO)
				.AddQueryExOption("memo", EditableF910201.MEMO)
				.AddQueryExOption("breakQty", EditableF910201.BREAK_QTY).ToList();

			ShowMessage(result);

			if (result != null && result.FirstOrDefault().IsSuccessed == true)  _isSaved = true;  

			//LocateCurrentData();
			//UserOperateMode = OperateMode.Query;
		}

		/// <summary>
		/// 新增資料後, 回到查詢畫面並且查詢到該筆資料
		/// </summary>
		private void LocateCurrentData()
		{
			if (_isSaved == false) return;
			_isSaved = true;
			SelectedDc = EditableF910201.DC_CODE;
			StartFinishDt = EditableF910201.FINISH_DATE;
			EndFinishDt = EditableF910201.FINISH_DATE;
			ProcessNo = EditableF910201.PROCESS_NO;
			ProcessStatus = EditableF910201.STATUS;
			UserOperateMode = OperateMode.Query;
			DoSearch();
			SelectedF910201 = F910201Data.FirstOrDefault();
			//actionForAfterUpdate();
		}
		#endregion Save

		#endregion

		#region Command - 加工單按鈕
		#region 編訂組合
		public ICommand BomCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => true
				);
			}
		}
		#endregion

		#region 品號標籤設定
		public ICommand SetLabelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => true
				);
			}
		}
		#endregion

		#region 開立揀料單
		private bool _isPrintPickTicket = false;
		private bool _isOpenPickTicketWindow = false;
		public ICommand PickCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => UserOperateMode != OperateMode.Query && EditableF910201 != null && EditableF910201.STATUS == "0",
					o => DoPickCommandComplete()
				);
			}
		}



		private void TryCreatePickTicket()
		{
			_isPrintPickTicket = false;
			_isOpenPickTicketWindow = false;

			EditableF910201 = FindByKey<F910201>(EditableF910201);

			// 檢查是否已開立揀料單, 是的話詢問要不要列印
			if (EditableF910201.PROC_STATUS != "0")
			{
				var msg = ShowMessage(Messages.InfoPickNoExists);
				if (msg == UILib.Services.DialogResponse.Yes)
				{
					// 列印揀料單
					_isPrintPickTicket = true;
				}
			}
			else
			{
				if (ShowConfirmMessage(Properties.Resources.P9101010000_ViewModel_SaveAndStartPick) == UILib.Services.DialogResponse.No)
					return;
				if (!CommonHelper.ComparePropertiesValue(EditableF910201, SelectedF910201))
					DoUpdate();

				string pickNo = CreatePickTicket();
				if (!string.IsNullOrEmpty(pickNo))
				{
					SetEditableF910201();
					var msg = ShowMessage(Messages.InfoPickNoExists);
					if (msg == UILib.Services.DialogResponse.Yes)
					{
						// 列印揀料單
						_isPrintPickTicket = true;
					}
				}
				else
					_isOpenPickTicketWindow = true;
			}
		}

		private void DoPickCommandComplete()
		{
			TryCreatePickTicket();
			if (_isPrintPickTicket) DoPrintPickTicket(PrintType.Preview);
			if (_isOpenPickTicketWindow) actionForCreatePickTicket();
			_isPrintPickTicket = false;
			_isOpenPickTicketWindow = false;
		}
		#endregion

		#region 預選標籤
		public ICommand PrintLBSetCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => UserOperateMode != OperateMode.Query && EditableF910201 != null && EditableF910201.STATUS == "0"
					
				);
			}
		}
		#endregion

		#region 選擇生產線
		public ICommand LineCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => UserOperateMode != OperateMode.Query && EditableF910201 != null && EditableF910201.STATUS == "0"
				);
			}
		}
		#endregion

		#region 加工完成

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gtStatus">大於這個狀態就不能修改</param>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool CheckCanEdit(string gtStatus, string message)
		{
			var f910201 = FindByKey<F910201>(EditableF910201);
			if (f910201.STATUS.CompareTo(gtStatus) > 0)
			{
				ShowWarningMessage(message);
				EditableF910201 = f910201;
				return false;
			}

			return true;
		}

		public ICommand FinishCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => UserOperateMode != OperateMode.Query && EditableF910201 != null && EditableF910201.CRT_STAFF != null
						&& EditableF910201.STATUS.CompareTo("1") <= 0 
				);
			}
		}
		#endregion

		#region 上架回倉
		public ICommand ReturnCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { },
					() => UserOperateMode != OperateMode.Query && EditableF910201 != null && EditableF910201.STATUS != "9" && !string.IsNullOrEmpty(EditableF910201.PROC_STATUS)  //&& EditableF910201.PROC_STATUS.CompareTo("0") > 0
				);
			}
		}
		#endregion

		#region 列印流通加工單
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
					DoPrint,
				(t) => !IsBusy && UserOperateMode == OperateMode.Query && SelectedF910201 != null);
			}
		}

		public void DoPrint(PrintType printType)
		{
			var proxy = GetProxy<F91Entities>();
			SelectedF910201 = proxy.F910201s.Where(x => x.DC_CODE == SelectedF910201.DC_CODE
													&& x.GUP_CODE == SelectedF910201.GUP_CODE
													&& x.CUST_CODE == SelectedF910201.CUST_CODE
													&& x.PROCESS_NO == SelectedF910201.PROCESS_NO)
											.FirstOrDefault();
			if (SelectedF910201 == null)
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}

			var outsourceName = OutsourceList.Where(x => x.Value == SelectedF910201.OUTSOURCE_ID).Select(x => x.Name).FirstOrDefault() ?? string.Empty;
			var tmp = new P910101Report()
			{
				DC_CODE = SelectedF910201.DC_CODE,
				GUP_CODE = SelectedF910201.GUP_CODE,
				CUST_CODE = SelectedF910201.CUST_CODE,
				A_PROCESS_QTY = SelectedF910201.A_PROCESS_QTY,
				PROCESS_NO = SelectedF910201.PROCESS_NO,
				BOX_QTY = SelectedF910201.BOX_QTY,
				CASE_QTY = SelectedF910201.CASE_QTY,
				BREAK_QTY = SelectedF910201.BREAK_QTY,
				FINISH_DATE = SelectedF910201.FINISH_DATE,
				ITEM_CODE = SelectedF910201.ITEM_CODE,
				ITEM_NAME = ItemName,
				ITEM_CODE_BOM = SelectedF910201.ITEM_CODE_BOM,
				ITEM_NAME_BOM = ItemBomName,
				MEMO = SelectedF910201.MEMO,
				ORDER_NO = SelectedF910201.ORDER_NO,
				OUTSOURCE_ID = SelectedF910201.OUTSOURCE_ID,
				PROCESS_QTY = SelectedF910201.PROCESS_QTY,
				UNFINISHED_QTY = SelectedF910201.PROCESS_QTY - SelectedF910201.A_PROCESS_QTY,
				OUTSOURCE_NAME = outsourceName,
				QUOTE_NO = SelectedF910201.QUOTE_NO,
				QUOTE_NAME = SelectedF910401.QUOTE_NAME ?? "",
				CRT_DATE = SelectedF910201.CRT_DATE,
				CRT_NAME = SelectedF910201.CRT_NAME,
				PROCESS_SOURCE = SelectedF910201.PROCESS_SOURCE == "1" ? Resources.Resources.Dc : Properties.Resources.CUST_CODE

			};
			ReportData = new List<P910101Report>() { tmp };
			DoPrintReport(printType);
		}
		#endregion
		#endregion

		#region 儲存前的資料檢核
		/// <summary>
		/// 儲存前的資料檢核
		/// </summary>
		/// <returns></returns>
		private string CheckDataBeforeSave()
		{
			string result = CheckData();
			if (!string.IsNullOrWhiteSpace(result)) return result;
			result = CheckFinishDate();
			if (!string.IsNullOrWhiteSpace(result)) return result;
			return string.Empty;
		}

		/// <summary>
		/// 檢查加工單欄位
		/// 加工單必填欄位：物流中心、加工單來源、加工委外商、完工日期、成品編號、加工數量
		/// </summary>
		/// <returns></returns>
		private string CheckData()
		{
			var proxy = GetProxy<F19Entities>();
			if (string.IsNullOrWhiteSpace(EditableF910201.DC_CODE) || string.IsNullOrWhiteSpace(EditableF910201.GUP_CODE) || string.IsNullOrWhiteSpace(EditableF910201.CUST_CODE))
				return Properties.Resources.P9101010000_ViewModel_NoDCInfo;
			if (EditableF910201.FINISH_DATE < DateTime.Today)
				return Properties.Resources.P9101010000_ViewModel_FINISH_DATE_Invalid;
			if (EditableF910201.FINISH_TIME == string.Empty)
				return Properties.Resources.P9101010000_ViewModel_FINISH_TIME_Empty;
			if (string.IsNullOrWhiteSpace(EditableF910201.ITEM_CODE))
				return Properties.Resources.P9101010000_ViewModel_ITEM_CODE_Empty;
			var tmpItem = proxy.F1903s.Where(x => x.GUP_CODE == EditableF910201.GUP_CODE && x.ITEM_CODE == EditableF910201.ITEM_CODE && x.CUST_CODE == EditableF910201.CUST_CODE).FirstOrDefault();
			if (tmpItem == null) return Properties.Resources.P9101010000_ViewModel_ITEM_CODE_Error;
			if (EditableF910201.PROCESS_QTY <= 0)
				return Properties.Resources.P9101010000_ViewModel_PROCESS_QTY;
			if (EditableSelectedF910401 == null) return Properties.Resources.P9101010000_ViewModel_ProcessItem_Notdifine;
			return string.Empty;
		}

		/// <summary>
		/// 進倉單號有輸入則檢核是否有此進倉單號
		/// Memo: 由於進倉單號必為A開頭，因此User輸入進倉單號後系統可先檢核是否為A開頭，檢核通過再去資料庫撈
		/// </summary>
		/// <returns></returns>
		private bool CheckOrderNo()
		{
			if (string.IsNullOrWhiteSpace(EditableF910201.ORDER_NO)) return true;

			if (!EditableF910201.ORDER_NO.StartsWith("A"))
			{
				ShowMessage(Messages.WarningInvalidOrderNo);
				return false;
			}

			var proxy = GetProxy<F02Entities>();
			var tmp =
				proxy.F020201s.Where(
					x =>
						x.DC_CODE == EditableF910201.DC_CODE && x.GUP_CODE == EditableF910201.GUP_CODE &&
						x.CUST_CODE == EditableF910201.CUST_CODE && x.PURCHASE_NO == EditableF910201.ORDER_NO &&
						x.ITEM_CODE == EditableF910201.ITEM_CODE && x.SPECIAL_CODE == "201").FirstOrDefault();
			if (tmp == null)
			{
				ShowMessage(Messages.WarningInvalidOrderNo);
				return false;
			}
			return true;
		}

		/// <summary>
		/// 加工單完工日期不可小於今日
		/// </summary>
		/// <returns></returns>
		private string CheckFinishDate()
		{
			if (EditableF910201.FINISH_DATE < DateTime.Today) return Properties.Resources.P9101010000_ViewModel_FINISH_DATE_Invalid;
			return string.Empty;
		}
		/// <summary>
		/// 檢查調撥單狀態
		/// </summary>
		/// <returns>True: 有資料, False: 沒資料</returns>
		public bool CheckProcStatus()
		{
			var proxy = GetProxy<F15Entities>();

            if (IsAllocationExist())
            {
                var qry = proxy.F151001s.Where(x => x.DC_CODE == _selectedDc &&
                x.GUP_CODE == _selectedGup &&
                                                    x.CUST_CODE == _selectedCust &&
                                                    x.SOURCE_NO == EditableF910201.PROCESS_NO &&
                                                    x.STATUS == "5").ToList();
                //LOCK_STATUS= 2 =>下架完成
                if (qry.Any())
                    return true;
                return false;
            }
            else
                return true;
		}

        public bool IsAllocationExist()
        {
            bool resutl = false;
            var proxy = GetProxy<F91Entities>();
            var getPickData = proxy.F910205s.Where(o => o.DC_CODE == _selectedDc &&
            o.GUP_CODE == _selectedGup && o.CUST_CODE == _selectedCust &&
            o.PROCESS_NO == EditableF910201.PROCESS_NO).ToList();
            foreach (var item in getPickData)
            {
                var getAllocation = proxy.F91020502s.Where(o => o.DC_CODE == _selectedDc &&
            o.GUP_CODE == _selectedGup && o.CUST_CODE == _selectedCust &&
            o.PICK_NO == item.PICK_NO).FirstOrDefault();
                if (getAllocation != null)
                {
                    resutl = true;
                    return resutl;
                }
                else
                    resutl = false;
            }
                return resutl;
        }

        /// <summary>
        /// 檢查揀料單建立沒, 如果還沒建立則回傳0
        /// 如果已建立, 但未開始揀貨, 則回傳1
        /// 如果已建立, 且已開始揀貨 (F91020501有值), 則回傳2
        /// </summary>
        /// <returns>0: 未建立揀料單, 1: 已建立揀料單但未揀貨, 2: 已開始揀貨</returns>
        private int CheckF910205()
		{
			var proxy = GetProxy<F91Entities>();

			// 用F910201的PROC_STATUS判斷有沒有開立揀料單, PROC_STATUS >= 1則代表已開立
			var tmp910201 = proxy.F910201s.Where(x => x.DC_CODE == EditableF910201.DC_CODE && x.GUP_CODE == EditableF910201.GUP_CODE && x.CUST_CODE == EditableF910201.CUST_CODE && x.PROCESS_NO == EditableF910201.PROCESS_NO).FirstOrDefault();
			if (tmp910201.PROC_STATUS == "0") return 0;

			// 用F910205判斷是否開立揀料單
			var tmp = proxy.F910205s.Where(x => x.DC_CODE == EditableF910201.DC_CODE && x.GUP_CODE == EditableF910201.GUP_CODE && x.CUST_CODE == EditableF910201.CUST_CODE && x.PROCESS_NO == EditableF910201.PROCESS_NO).FirstOrDefault();
			if (tmp == null) return 0;

			// 用F91020501判斷是否已開始揀貨
			var tmp2 = proxy.F91020501s.Where(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE && x.PICK_NO == tmp.PICK_NO).FirstOrDefault();
			if (tmp2 == null) return 1;
			return 2;
		}

		/// <summary>
		/// 存檔前重新檢查加工單狀態
		/// </summary>
		/// <returns>True: 可儲存, False: 不可儲存</returns>
		private bool RecheckProcessStatus()
		{
			var proxy = GetProxy<F91Entities>();
			var tmp = proxy.F910201s.Where(x => x.DC_CODE == EditableF910201.DC_CODE && x.GUP_CODE == EditableF910201.GUP_CODE && x.CUST_CODE == EditableF910201.CUST_CODE && x.PROCESS_NO == EditableF910201.PROCESS_NO).FirstOrDefault();
			if (tmp.STATUS == "2" || tmp.STATUS == "3" || tmp.STATUS == "9") return false;
			return true;
		}

		/// <summary>
		/// 建立揀料單. 庫存夠的時候回傳新的揀料單號, 否則回傳空(NULL)
		/// </summary>
		/// <returns></returns>
		private string CreatePickTicket()
		{
			// 狀況1
			if (string.IsNullOrWhiteSpace(EditableF910201.ITEM_CODE_BOM))
			{
				var bomStock = GetBomStock("1");
				var pickDatas = CopyBomQtyDataToPickData(bomStock, "1");
				if (!CheckBomQty(bomStock))
				{
					// 庫存不夠
					return null;
				}
				else
				{
					// 所需揀料數=此加工單成品的加工料數(F910201.ProcessQty)
					return DoCreatePickTicket(pickDatas);
				}
			}
			else // 狀況2
			{
				// 2.1 檢查BOM_TYPE
				var proxy = GetProxy<F91Entities>();
				var tmp910101 = proxy.F910101s.Where(x => x.GUP_CODE == EditableF910201.GUP_CODE && x.CUST_CODE == EditableF910201.CUST_CODE && x.BOM_NO == EditableF910201.ITEM_CODE_BOM).First();

				if (tmp910101.BOM_TYPE == "0") // 2.1 組合商品
				{
					// 檢查揀料庫存是否足夠
					var bomStock = GetBomStock("0");
					var pickDatas = CopyBomQtyDataToPickData(bomStock, "2");
					if (!CheckBomQty(bomStock))
					{
						// 庫存不夠
						return null;
					}
					else
					{
						// 建立揀料單
						// 依照F910102的MATERIAL_CODE設定其揀料數
						return DoCreatePickTicket(pickDatas);
					}
				}
				else // 2.2 拆解商品, 只要看ITEM_CODE的加工庫存數量
				{
					var bomStock = GetBomStock("1");
					var pickDatas = CopyBomQtyDataToPickData(bomStock, "3");
					if (!CheckBomQty(bomStock))
					{
						// 庫存不夠
						return null;
					}
					else
					{
						// 建立揀料單
						// 所需揀料數=此加工單成品的加工料數(F910201.ProcessQty)
						// 一樣要抓出F910102的MATERIAL_CODE, 並將揀料數皆設為0
						return DoCreatePickTicket(pickDatas);
					}
				}
			}
			return string.Empty;
		}

		private string DoCreatePickTicket(List<wcf.PickData> pickDatas)
		{
			var proxyWcf = new wcf.P91WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.CreateP910205(EditableF910201.DC_CODE, EditableF910201.GUP_CODE, EditableF910201.CUST_CODE, EditableF910201.PROCESS_NO, pickDatas.ToArray()));
			if (result.IsSuccessed == false)
			{
				ShowWarningMessage(result.Message);
				return null;
			}
			else
				return result.Message;
		}

		/// <summary>
		/// 檢查加工倉庫存
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool CheckBomQty(List<BomQtyData> bomDatas)
		{
			// 假設一個加工商品會在多個加工倉都有庫存, 所以要再計算每個加工商品的庫存總數, 才能判斷庫存數到底夠不夠
			foreach (var p in bomDatas)
			{
				if ((p.AVAILABLE_QTY ?? 0) < (p.NEED_QTY ?? 0))
					return false;
			}
			return true;
		}

		/// <summary>
		/// 取得加工倉庫存
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private List<BomQtyData> GetBomStock(string type)
		{
			var proxyEx = GetExProxy<P91ExDataSource>();
			var tmp = proxyEx.CreateQuery<BomQtyData>("GetBomQtyData")
				.AddQueryExOption("dcCode", EditableF910201.DC_CODE)
				.AddQueryExOption("gupCode", EditableF910201.GUP_CODE)
				.AddQueryExOption("custCode", EditableF910201.CUST_CODE)
				.AddQueryExOption("processNo", EditableF910201.PROCESS_NO)
				.AddQueryExOption("type", type)
				.ToList();
			return tmp;
		}

		private List<wcf.PickData> CopyBomQtyDataToPickData(List<BomQtyData> bomQtyDatas, string isBomItem)
		{

			return bomQtyDatas.Select(a => new wcf.PickData
			{
				A_PROCESS_QTY = (int)(a.NEED_QTY ?? 0),
				ITEM_CODE = isBomItem == "3" ? a.ITEM_CODE : isBomItem == "1" ? a.ITEM_CODE : isBomItem == "2" ? a.MATERIAL_CODE : "",
				QTY = (int)(a.AVAILABLE_QTY ?? 0),
				WAREHOUSE_TYPE = a.WAREHOUSE_TYPE
			}).ToList();
		}

		public void SetEditableF910201()
		{
			EditableF910201 = null;
			var proxy = GetProxy<F91Entities>();
			var tmp = proxy.F910201s.Where(x => x.DC_CODE == SelectedF910201.DC_CODE && x.GUP_CODE == SelectedF910201.GUP_CODE && x.CUST_CODE == SelectedF910201.CUST_CODE && x.PROCESS_NO == SelectedF910201.PROCESS_NO).FirstOrDefault();
			if (tmp == null)
			{
				ShowMessage(Messages.WarningBeenDeleted);
			}
			else
			{
				//加工揀料單 有加工揀料單.不可編輯
				var f910205Data = proxy.F910205s.Where(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE && x.PROCESS_NO == tmp.PROCESS_NO).FirstOrDefault();
				Editf910205 = f910205Data == null;

				EditableSelectedF910401 = proxy.F910401s.Where(x => (x.DC_CODE == tmp.DC_CODE || x.DC_CODE == "000") && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE && x.QUOTE_NO == tmp.QUOTE_NO).FirstOrDefault();
				EditableF910201 = tmp;
				//加工單狀態若為待處理則可修改資料
				Editable = EditableF910201.STATUS == "0" && f910205Data == null && EditableF910201.PROC_TYPE=="0";
				Editreadonly = !Editable;
				EditreadonlyDesc = !(EditableF910201.STATUS == "0" && f910205Data == null);
			}

		}

		private List<PickReport> _pickReports;
		public List<PickReport> PickReports
		{
			get { return _pickReports; }
			set
			{
				Set(() => PickReports, ref _pickReports, value);
			}
		}


		public void DoPrintPickTicket(PrintType printType)
		{
			var proxyEx = GetExProxy<P91ExDataSource>();
			PickReports = proxyEx.CreateQuery<PickReport>("GetPickTicketReport")
				.AddQueryExOption("dcCode", EditableF910201.DC_CODE)
				.AddQueryExOption("gupCode", EditableF910201.GUP_CODE)
				.AddQueryExOption("custCode", EditableF910201.CUST_CODE)
				.AddQueryExOption("processNo", EditableF910201.PROCESS_NO)
				.ToList();

			DoPrintPickTicketReport(printType);
		}

		#endregion

	}
}
