using System;
using System.Data;
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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P16WcfService;
using wcf19 = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using wcf70 = Wms3pl.WpfClient.ExDataServices.P70WcfService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
//using Wms3pl.WpfClient.P70.Views;


namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1605010000_ViewModel : InputViewModelBase
	{
		private string _gupCode;
		public string _custCode;
		private string _destoryNo;
		private string _original_DISTR_CAR;
		public P1605010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				DcList = GetDcList();
				DcListAdd = DcList;
				DcListQuery = DcList;
				DcItemaQuery = DcList.First();
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				var proxy = GetProxy<F00Entities>();
				StatusQuery = GetBaseTableService.GetF000904List(FunctionCode, "F160501", "STATUS", true);
				if (StatusQuery != null) { StatusItemQuery = StatusQuery.First(); }
				F160501Query = new F160501();
				IsExpandQuery = true;
				DetailCtn = 0;
				CrtSDate = DateTime.Today;
				CrtEDate = DateTime.Today;
			}

		}

		private bool _caneditdistrcar;
		public bool CanEditDistrCar
		{
			get { return _caneditdistrcar; }
			set
			{
				_caneditdistrcar = value;
				RaisePropertyChanged("CanEditDistrCar");
			}
		}

		#region DC 參數

		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心選項
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		// 新增物流中心選項
		private List<NameValuePair<string>> _dcListAdd;
		public List<NameValuePair<string>> DcListAdd
		{
			get { return _dcListAdd; }
			set
			{
				_dcListAdd = value;
				RaisePropertyChanged("DcListAdd");
			}
		}

		private NameValuePair<string> _dcItemaAdd;
		public NameValuePair<string> DcItemaAdd
		{
			get { return _dcItemaAdd; }
			set
			{
				_dcItemaAdd = value;
				RaisePropertyChanged("DcItemaAdd");
			}
		}

		// 查詢物流中心選項
		private List<NameValuePair<string>> _dcListQuery;
		public List<NameValuePair<string>> DcListQuery
		{
			get { return _dcListQuery; }
			set
			{
				_dcListQuery = value;
				RaisePropertyChanged("DcListQuery");
			}
		}

		private NameValuePair<string> _dcItemaQuery;
		public NameValuePair<string> DcItemaQuery
		{
			get { return _dcItemaQuery; }
			set
			{
				_dcItemaQuery = value;
				F160501QueryData = null;
				DgF160502Data = null;
				DetailCtn = 0;
				RaisePropertyChanged("DcItemaQuery");
			}
		}


		#endregion

		#region Form 業主參數設定

		private NameValuePair<string> _gupItemAdd;
		public NameValuePair<string> GupItemAdd
		{
			get { return _gupItemAdd; }
			set
			{
				_gupItemAdd = value;
				RaisePropertyChanged("GupItemAdd");
			}
		}

		#endregion

		#region Data 商品搜尋用欄位
		private string _itemcode;
		public string ItemCode
		{
			get { return _itemcode; }
			set
			{
                if (string.IsNullOrWhiteSpace(value))
                {
                    ItemDestoryQty = 0;
                }
				_itemcode = value;
				RaisePropertyChanged("ItemCode");
			}
		}
		private string _itemname;
		public string ItemName
		{
			get { return _itemname; }
			set
			{
				_itemname = value;
				RaisePropertyChanged("ItemName");
			}
		}
		private string _itemsize;
		public string ItemSize
		{
			get { return _itemsize; }
			set
			{
				_itemsize = value;
				RaisePropertyChanged("ItemSize");
			}
		}
		private string _itemspec;
		public string ItemSpec
		{
			get { return _itemspec; }
			set
			{
				_itemspec = value;
				RaisePropertyChanged("ItemSpec");
			}
		}
		private string _itemcolor;
		public string ItemColor
		{
			get { return _itemcolor; }
			set
			{
				_itemcolor = value;
				RaisePropertyChanged("ItemColor");
			}
		}

        private string _serialNo;
        public string SerialNo
        {
            get { return _serialNo; }
            set
            {
                _serialNo = value;
                RaisePropertyChanged("SerialNo");
            }
        }

		private int _itemScrapQty;
		public int ItemScrapQty
		{
			get { return _itemScrapQty; }
			set
			{
				_itemScrapQty = value;
				RaisePropertyChanged("ItemScrapQty");
			}
		}

		private int _itemDestoryQty;
		public int ItemDestoryQty
		{
			get { return _itemDestoryQty; }
			set
			{
				_itemDestoryQty = value;
				RaisePropertyChanged("ItemDestoryQty");
			}
		}

		private bool _hasSearchItemData;

		public bool HasSearchItemData
		{
			get { return _hasSearchItemData; }
			set
			{
				Set(() => HasSearchItemData, ref _hasSearchItemData, value);
			}
		}
		#endregion

		#region 是否全選 參數

		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				CheckSelectedAll(_isCheckAll, DgListAdd);
				RaisePropertyChanged();
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

		#region 派車 Flag

		private bool _isCheckDistrCar;

		public bool IsCheckDistrCar
		{
			get { return _isCheckDistrCar; }
			set
			{
				_isCheckDistrCar = value;
				RaisePropertyChanged("IsCheckDistrCar");
			}
		}
		#endregion

		#region 匯入是否有虛擬-序號商品Flag 參數

		private bool _checkSerialImport;

		public bool CheckSerialImport
		{
			get { return _checkSerialImport; }
			set
			{
				_checkSerialImport = value;
				RaisePropertyChanged("CheckSerialImport");
			}
		}
		#endregion

		#region 新增-主檔 參數

		private F160501 _f160501AddData;

		public F160501 F160501AddData
		{
			get { return _f160501AddData; }
			set
			{
				_f160501AddData = value;
				RaisePropertyChanged("F160501AddData");
			}
		}
		#endregion

		#region GV Detail Data - Add 參數
		private SelectionList<F160502Data> _dgListAdd;
		public SelectionList<F160502Data> DgListAdd
		{
			get { return _dgListAdd; }
			set
			{
				_dgListAdd = value;
				RaisePropertyChanged("DgListAdd");
			}
		}
		#endregion

		#region GV 序號暫存 Table - 參數
		private List<F160502Data> _dgSerialList;
		public List<F160502Data> DgSerialList
		{
			get { return _dgSerialList; }
			set
			{
				_dgSerialList = value;
				RaisePropertyChanged("DgSerialList");
			}
		}
		#endregion

		#region GV 每次匯入暫存參數.  - 參數
		//每次匯入前會先清空 , 僅記錄一次匯入資料
		private List<F160502Data> _dgTmpList;
		public List<F160502Data> DgTmpList
		{
			get { return _dgTmpList; }
			set
			{
				_dgTmpList = value;
				RaisePropertyChanged("DgTmpList");
			}
		}
		#endregion

		#region 查詢 - 參數

		#region 查詢主檔 參數

		private F160501 _f160501Query;

		public F160501 F160501Query
		{
			get { return _f160501Query; }
			set
			{
				_f160501Query = value;
				RaisePropertyChanged("F160501Query");
			}
		}
		#endregion

		#region 查詢結果 參數

		private ObservableCollection<F160501Data> _f160501QueryData;

		public ObservableCollection<F160501Data> F160501QueryData
		{
			get { return _f160501QueryData; }
			set
			{
				_f160501QueryData = value;
				RaisePropertyChanged("F160501QueryData");
			}
		}
		#endregion

		#region 查詢資料選擇 參數

		private F160501Data _f160501SelectData;

		public F160501Data F160501SelectData
		{
			get { return _f160501SelectData; }
			set
			{
				_f160501SelectData = value;
				SelectGvDataCommand.Execute(null);
				RaisePropertyChanged("F160501SelectData");
			}
		}
		#endregion

		#region Grid 內部交易 出貨明細 DgF050801Data 資料
		private ObservableCollection<F050801WmsOrdNo> _dgF050801Data;
		public ObservableCollection<F050801WmsOrdNo> DgF050801Data
		{
			get { return _dgF050801Data; }
			set { _dgF050801Data = value; RaisePropertyChanged("DgF050801Data"); }
		}
		#endregion

		#region Grid 內部交易主檔明細 DgF160502Data 資料
		private ObservableCollection<F160502Data> _dgF160502Data;
		public ObservableCollection<F160502Data> DgF160502Data
		{
			get { return _dgF160502Data; }
			set { _dgF160502Data = value; RaisePropertyChanged("DgF160502Data"); }
		}
		#endregion

		#region Grid 出貨資料選取
		private F050801WmsOrdNo _selectedF050801Data;
		public F050801WmsOrdNo SelectedF050801Data
		{
			get { return _selectedF050801Data; }
			set
			{
				_selectedF050801Data = value;
				RaisePropertyChanged("SelectedF050801Data");

			}
		}
		#endregion

		#region 單據狀態參數
		private List<NameValuePair<string>> _statusQuery;
		public List<NameValuePair<string>> StatusQuery
		{
			get { return _statusQuery; }
			set
			{
				_statusQuery = value;
				RaisePropertyChanged("StatusQuery");
			}
		}

		private NameValuePair<string> _statusItemQuery;
		public NameValuePair<string> StatusItemQuery
		{
			get { return _statusItemQuery; }
			set
			{
				_statusItemQuery = value;
				RaisePropertyChanged("StatusItemQuery");
			}
		}
		#endregion

		#region 查詢修件展開Flag 參數

		private bool _isExpandQuery;
		public bool IsExpandQuery
		{
			get { return _isExpandQuery; }
			set
			{
				_isExpandQuery = value;
				RaisePropertyChanged("IsExpandQuery");
			}
		}
		#endregion

		#region 查詢-出貨單 參數

		private string _ordNoQuery;
		public string OrdNoQuery
		{
			get { return _ordNoQuery; }
			set
			{
				_ordNoQuery = value;
				RaisePropertyChanged("OrdNoQuery");
			}
		}
		#endregion

		#region 建立日期查詢  參數

		private DateTime? _crtSDate;
		public DateTime? CrtSDate
		{
			get { return _crtSDate; }
			set
			{
				_crtSDate = value;
				RaisePropertyChanged("CrtSDate");
			}
		}

		private DateTime? _crtEDate;
		public DateTime? CrtEDate
		{
			get { return _crtEDate; }
			set
			{
				_crtEDate = value;
				RaisePropertyChanged("CrtEDate");
			}
		}

		#endregion

		#region 過帳日期查詢 - 參數

		private string _postingSDate;
		public string PostingSDate
		{
			get { return _postingSDate; }
			set
			{
				_postingSDate = value;
				RaisePropertyChanged("PostingSDate");
			}
		}

		private string _postingEDate;
		public string PostingEDate
		{
			get { return _postingEDate; }
			set
			{
				_postingEDate = value;
				RaisePropertyChanged("PostingEDate");
			}
		}

		#endregion

		#region 查詢後明細總數
		private int _detailCtn;
		public int DetailCtn
		{
			get { return _detailCtn; }
			set
			{
				_detailCtn = value;
				RaisePropertyChanged("DetailCtn");
			}
		}

		#endregion

		#region 匯入檔案-Type 參數   1 : 一般匯入  2 :虛擬商品序號滙入
		private int _importType;
		public int ImportType
		{
			get { return _importType; }
			set
			{
				_importType = value;
				RaisePropertyChanged("ImportType");
			}
		}

		#endregion

		#region 匯入檔案該筆 Excel 總數
		private int _importCount;
		public int ImportCount
		{
			get { return _importCount; }
			set
			{
				_importCount = value;
				RaisePropertyChanged("ImportCount");
			}
		}

		#endregion


		#endregion

		#region Funcion

		#region 取物流中心資料

		public List<NameValuePair<string>> GetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			return data;
		}
		#endregion

		#region Grid Checkbox 全選 -Add
		public void CheckSelectedAll(bool isChecked, SelectionList<F160502Data> dgData)
		{
			if (dgData != null)
			{
				foreach (var dgDataItem in dgData)
					dgDataItem.IsSelected = isChecked;
			}
		}

		#endregion

		#region 檢查是商品是否為-虛擬商品
		public bool CheckItemType(string itemCode)
		{
			bool result = false;
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1903s.Where(o => o.ITEM_CODE == itemCode && o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode && o.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode).FirstOrDefault();
			//判斷是否為虛擬商品
			if (data != null)
			{
				result = (data.VIRTUAL_TYPE == null || string.IsNullOrEmpty(data.VIRTUAL_TYPE)) ? false : true;
			}
			return result;
		}

		#endregion

		#region 取商品資訊並且判斷是不為虛擬序號商品
		private F160502Data GetItemData(string itemCode)
		{
			F160502Data f160502Data = new F160502Data();
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1903s.Where(o => o.ITEM_CODE == itemCode && o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode && o.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode).FirstOrDefault();
			var f1903data = proxy.F1903s.Where(o => o.ITEM_CODE == itemCode
																			&& o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode
																			&& o.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode
																			).FirstOrDefault();

			//判斷是否為虛擬商品
			if (data != null && !string.IsNullOrEmpty(data.ITEM_NAME))
			{
				f160502Data.DC_CODE = DcItemaAdd.Value;
				f160502Data.GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode;
				f160502Data.CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode;
				f160502Data.ITEM_CODE = data.ITEM_CODE;
				f160502Data.ITEM_NAME = data.ITEM_NAME;
				f160502Data.ITEM_SIZE = data.ITEM_SIZE;
				f160502Data.ITEM_SPEC = data.ITEM_SPEC;
				f160502Data.ITEM_COLOR = data.ITEM_COLOR;
				f160502Data.SCRAP_QTY = 0;
				f160502Data.VIRTUAL_TYPE = (data.VIRTUAL_TYPE == null || string.IsNullOrEmpty(data.VIRTUAL_TYPE)) ? Resources.Resources.No : Resources.Resources.Yes;
				//序號商品
				f160502Data.BUNDLE_SERIALNO = (f1903data == null || string.IsNullOrEmpty(f1903data.BUNDLE_SERIALNO)
																				|| f1903data.BUNDLE_SERIALNO == "0") ? "0" : "1";
			}
			return f160502Data;
		}
		#endregion

		#region 取報廢數量
		public void GetScrapItemStock()
		{
			ItemScrapQty = 0;

			var proxy = new wcf19.P19WcfServiceClient();
			var wcfResult = RunWcfMethod<int>(proxy.InnerChannel, () =>
					proxy.GetScrapItemStock(DcItemaAdd.Value
											, Wms3plSession.Get<GlobalInfo>().GupCode
											, Wms3plSession.Get<GlobalInfo>().CustCode
											, ItemCode));

			ItemScrapQty = wcfResult;
		}
		#endregion

		#region 新增-檢查判斷
		private bool CheckAddData()
		{
			string errorStr = "";
			if (UserOperateMode == OperateMode.Add && Convert.ToDateTime(F160501AddData.DESTROY_DATE) < DateTime.Today)
			{
				errorStr += Properties.Resources.P1605010000_DestroyDateInvalid;
			}
			//if (DgListAdd == null || DgListAdd.Count == 0)
			//{
			//	errorStr += Properties.Resources.P1605010000_ItemDetail_Required;
			//}

			if (DgListAdd != null && DgListAdd.Count > 0)
			{
				errorStr += CheckSerialQty();
			}

			if (!string.IsNullOrEmpty(errorStr))
			{
				DialogService.ShowMessage(errorStr);
				return false;
			}
			return true;
		}

		#endregion

		#region 虛擬序號商品數量是否與 Grid 數量相符
		private string CheckSerialQty()
		{
			string errorStr = "";
			if (DgSerialList != null)
			{
				//找出所有 - 虛擬序號商品
				var dglistItem = DgListAdd.Where(o => o.Item.VIRTUAL_TYPE == Resources.Resources.Yes && o.Item.BUNDLE_SERIALNO == "1").Select(o => o.Item).ToList();
				foreach (var item in dglistItem)
				{
					var itemSerialCount = DgSerialList.Where(x => x.ITEM_CODE == item.ITEM_CODE
																									&& x.DC_CODE == item.DC_CODE
																									&& x.GUP_CODE == item.GUP_CODE
																									&& x.CUST_CODE == item.CUST_CODE);
					if (item.DESTROY_QTY != itemSerialCount.Count())
					{
						errorStr += string.Format(Properties.Resources.P1605010000_VirtualSerialItem_InCompatible, item.ITEM_CODE, itemSerialCount.Count());
					}
				}


				// 從匯入虛擬序號商品 , 找出 Grid 沒有資料 Item
				var serialItem = DgSerialList.Where(x => x.DC_CODE == DcItemaAdd.Value
																								&& x.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode
																								&& x.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode
																								).GroupBy(x => x.ITEM_CODE).ToList();
				foreach (var item in serialItem)
				{
					var itemSerialCount = DgListAdd.Where(o => o.Item.ITEM_CODE == item.Key
																							&& o.Item.DC_CODE == DcItemaAdd.Value
																							&& o.Item.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode
																							&& o.Item.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode
																							).Select(o => o.Item).ToList();
					if (itemSerialCount == null || itemSerialCount.Count == 0)
					{
						errorStr += string.Format(Properties.Resources.P1605010000_ReImportVirtualItemSerialNo, item.Key);
					}
				}
			}
			return errorStr;
		}

		#endregion

		#region 檢查此狀態是否可以變更資料
		public bool CheckStatus(string dcCode,string gupCode,string custCode, string destoryNo)
		{
			//狀態 : 待處理  可修改
			var proxyP16 = GetExProxy<P16ExDataSource>();
			var f160501StatusData = proxyP16.CreateQuery<F160501Status>("GetF160501Status")
					.AddQueryExOption("dcCode", dcCode)
					.AddQueryExOption("gupCode", gupCode)
					.AddQueryExOption("custCode", custCode)
					.AddQueryExOption("destoryNo", destoryNo).ToList();

			return f160501StatusData.Where(x => x.STATUS == "0").Any();
		}
		#endregion

		#region 新增-預設值清空
		private void ClearAddValue()
		{
			if (UserOperateMode == OperateMode.Add)
			{
				DgListAdd = null;
				DgTmpList = null;
				DgSerialList = null;
				IsCheckDistrCar = false;
				F160501AddData.MEMO = "";
				F160501AddData.DESTROY_DATE = DateTime.Today.Date;
				DcItemaAdd = DcList.First();
				ClearItemValue();
			}
		}
		#endregion

		#region 清除GV Detail 新增預設值
		public void ClearItemValue()
		{
			ItemCode = "";
			ItemName = "";
			ItemSize = "";
			ItemSpec = "";
			ItemColor = "";
			ItemScrapQty = 0;
			ItemDestoryQty = 0;
            SerialNo = "";
		}
		#endregion

		#endregion

		#region Command

		private RelayCommand _setDefaultScrapCommand;

		/// <summary>
		/// Gets the SetDefaultScrapCommand.
		/// </summary>
		public RelayCommand SetDefaultScrapCommand
		{
			get
			{
				return _setDefaultScrapCommand
					?? (_setDefaultScrapCommand = new RelayCommand(
					() =>
					{
						ItemScrapQty = 0;
					}));
			}
		}

		#region 取報廢量 Command
		private ICommand _getScrapCommand;
		public ICommand GetScrapCommand
		{
			get
			{
				return _getScrapCommand ?? (_getScrapCommand = CreateBusyAsyncCommand(
						o => GetScrapItemStock()
						));
			}
		}
        #endregion

        #region Import Excel

        public ICommand ImportCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DispatcherAction(() =>
                    {
                        if (string.IsNullOrEmpty(ImportFilePath)) return;
                        ImportExcelCommand.Execute(null);
                    });
                });
            }
        }

        public ICommand ImportExcelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o =>
						{
							DoImport();
						}, () => UserOperateMode == OperateMode.Query
						, o =>
						{
							#region Properties.Resources.P1605010000_CheckCount

							if (DgListAdd == null) DgListAdd = new SelectionList<F160502Data>(new List<F160502Data>());
							var tmpResult = DgTmpList.Select(AutoMapper.Mapper.DynamicMap<F160502Data>).ToList();
							foreach (var item in tmpResult)
							{

								if (item.VIRTUAL_TYPE == Resources.Resources.Yes && item.BUNDLE_SERIALNO == "1")
								{
									//1 .進行判斷現行Grid(DgListAdd) 沒有該筆資料時才新增		
									//2 .若是虛擬商品 , 但由一般匯入時 且 Grid 已有資料就直接加上數量
									//3 .若有資料再判斷數量 DgListAdd < DgSerialList (ItemCode) 時再修正 DgListAdd 數量
									F160502Data itemData = DgListAdd.Where(x => x.Item.ITEM_CODE == item.ITEM_CODE
																					&& x.Item.DC_CODE == item.DC_CODE
																					&& x.Item.GUP_CODE == item.GUP_CODE
																					&& x.Item.CUST_CODE == item.CUST_CODE).Select(x => x.Item).ToList().FirstOrDefault();

									if ((itemData == null || itemData.DESTROY_QTY == 0))
									{	// 1 Step	
										if (DgSerialList.Count() > 0 || ImportType == 1)
										{
											DgListAdd.Add(new SelectionItem<F160502Data>(item));

										}
									}
									else if (ImportType == 1 && item.DESTROY_QTY > 0)
									{	//2 Step
										itemData.DESTROY_QTY += item.DESTROY_QTY;
									}
									else
									{	// 3 Step
										int itemSerialCount = DgSerialList.Where(x => x.ITEM_CODE == item.ITEM_CODE
																					&& x.DC_CODE == item.DC_CODE
																					&& x.GUP_CODE == item.GUP_CODE
																					&& x.CUST_CODE == item.CUST_CODE).Count();
										if (itemData.DESTROY_QTY < itemSerialCount)
										{
											itemData.DESTROY_QTY = itemSerialCount;
										}
									}
								}
								else
								{
									var itemData = DgListAdd.Where(x => x.Item.ITEM_CODE == item.ITEM_CODE
																					&& x.Item.DC_CODE == item.DC_CODE
																					&& x.Item.GUP_CODE == item.GUP_CODE
																					&& x.Item.CUST_CODE == item.CUST_CODE).Select(x => x.Item).FirstOrDefault();
                                    if (itemData == null)
                                    {
                                        DgListAdd.Add(new SelectionItem<F160502Data>(item));
                                    }
                                    else
                                    {
                                        itemData.DESTROY_QTY++;
                                    }
								}
							}

							#endregion
						});
			}
		}

		#region 匯入 -Main

		public void DoImport()
		{
            if (string.IsNullOrEmpty(ImportFilePath)) return;
            DgTmpList = new List<F160502Data>();  //預設先清空
			if (DgSerialList == null) DgSerialList = new List<F160502Data>();

			string fullFilePath = ImportFilePath;

			var msg = new MessagesStruct()
			{
				Button = DialogButton.OK,
				Image = DialogImage.Information,
				Message = Properties.Resources.P1604010000_SaveSuccess,
				Title = Resources.Resources.Information
			};

			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);
			if (excelTable != null)
			{
				ImportCount = 0;
				string errorItem = "";
				//一般匯入
				if (ImportType == 1)
				{
					//讀取Excel 欄位
					try
					{
						var queryData = (from col in excelTable.AsEnumerable()
										 select new F160502Data
										 {
											 ITEM_CODE = Convert.ToString(col[0]),
											 DESTROY_QTY = Convert.ToInt16(col[1])
										 }).ToList();
						ImportCount = queryData.Count();
						foreach (var items in queryData)
						{
							errorItem += CheckImportQty(items, items.DESTROY_QTY, 1);
						}
					}
					catch
					{
						DialogService.ShowMessage(Properties.Resources.P1605010000_ImportFormatInCorrect);
					}
				}
				else //虛擬商品序號
				{
					//讀取Excel 欄位
					CheckSerialImport = false;
					var queryData = (from col in excelTable.AsEnumerable()
									 select new F160502Data
									 {
										 ITEM_SERIALNO = Convert.ToString(col[0])
									 }).ToList();

					if (queryData != null && queryData.Any())
					{
						ImportCount = queryData.Count();
						foreach (var items in queryData)
						{
							errorItem += CheckImportQty(items, 1, 1);
						}
					}
				}

				if (!string.IsNullOrEmpty(errorItem))
				{
					DialogService.ShowMessage(errorItem);
				}
				else
				{
					DialogService.ShowMessage(string.Format(Properties.Resources.P1605010000_ImportSuccessCount, ImportCount.ToString()));
				}
				ClearItemValue();
			}
			else if (string.IsNullOrWhiteSpace(errorMeg))
			{
				msg = new MessagesStruct() { Message = Properties.Resources.P1604010000_NoData, Button = DialogButton.OKCancel, Image = DialogImage.Warning, Title = Resources.Resources.Information };
				ShowMessage(msg);
			}
			else
			{
				msg = new MessagesStruct() { Button = DialogButton.OKCancel, Image = DialogImage.Warning, Message = errorMeg, Title = Resources.Resources.Information };
				ShowMessage(msg);
			}

		}

		#endregion

		#region 檢查 匯入時數量問題

		private string CheckImportQty(F160502Data itemData, int destroyQty, int importType)
		{
			string errorItem = "";
			F2501 f2501data = null;
			//用序號碼取ItemCode 找出是否有該序號商品
			if (!string.IsNullOrEmpty(itemData.ITEM_SERIALNO))
			{
				var proxyF2501 = GetProxy<F25Entities>();
				f2501data = proxyF2501.F2501s.Where(o => o.SERIAL_NO == itemData.ITEM_SERIALNO
														&& o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode
														&& o.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode
														).FirstOrDefault();
				if (f2501data != null)
					itemData.ITEM_CODE = f2501data.ITEM_CODE;
			}

			var resultData = GetItemData(itemData.ITEM_CODE);
			//匯入時 : 非虛擬商品要判斷 報癈量大於銷毀量		
			if (string.IsNullOrEmpty(resultData.ITEM_CODE))
			{
				errorItem += string.Format(Properties.Resources.P1605010000_ItemDataEmpty, itemData.ITEM_CODE);
			}
			else
			{
				ItemCode = resultData.ITEM_CODE;
				ItemName = resultData.ITEM_NAME;
				//取得報廢數量
				GetScrapItemStock();
				resultData.SCRAP_QTY = ItemScrapQty;
				resultData.DESTROY_QTY = destroyQty;

				if (destroyQty > ItemScrapQty && resultData.VIRTUAL_TYPE == Resources.Resources.No)
				{
					//非虛擬商品序號
					errorItem += string.Format(Properties.Resources.P1605010000_ItemDestroyCountInCorrect, itemData.ITEM_CODE);
				}
				else if (DgListAdd != null && resultData.VIRTUAL_TYPE == Resources.Resources.No
							 && DgListAdd.Where(x => x.Item.ITEM_CODE == ItemCode).Count() > 0)
				{
					//非虛擬商品序號
					errorItem += string.Format(Properties.Resources.P1605010000_ItemDuplicate, itemData.ITEM_CODE);
				}
				else
				{
					//若是虛擬序號商品時,暫存表與序號List 都新增 , 最後至匯入結束function 判斷數量, 是否新增至 DgListAdd Grid
					if (resultData.VIRTUAL_TYPE == Resources.Resources.Yes && resultData.BUNDLE_SERIALNO == "1")
					{
						if (f2501data != null)
						{
							var proxyF160504 = GetProxy<F16Entities>();
							itemData.ITEM_CODE = f2501data.ITEM_CODE;
							//排除 F160504 重複資料
							var f160504data = proxyF160504.F160504s.Where(o => o.SERIAL_NO == f2501data.SERIAL_NO && o.DC_CODE == DcItemaAdd.Value
																							&& o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode
																							&& o.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode
																							);
							//排除目前己匯入的序號 重複資料
							var serialdata = DgSerialList.Where(o => o.ITEM_SERIALNO == itemData.ITEM_SERIALNO && o.DC_CODE == DcItemaAdd.Value
																							&& o.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode
																							&& o.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode
																							).FirstOrDefault(); ;
							//若狀態在編輯時 f160504data 在排除自己銷毀單據
							if (UserOperateMode == OperateMode.Edit && !string.IsNullOrEmpty(F160501SelectData.DESTROY_NO))
								f160504data = f160504data.Where(o => o.DESTROY_NO != F160501SelectData.DESTROY_NO);

							if (f160504data.FirstOrDefault() == null && serialdata == null)
							{
								resultData.ITEM_SERIALNO = itemData.ITEM_SERIALNO;
								if (ImportType == 2)  //真正虛擬商品序號匯入時才加入至 DgSerialList 且DgSerialList 沒有重複
								{
									DgSerialList.Add(resultData);
								}
							}
							else if (ImportType == 2 && f160504data.FirstOrDefault() != null)
							{
								errorItem += string.Format(Properties.Resources.P1605010000_ImportExcludeDuplicateSerialNo, f2501data.SERIAL_NO);
							}

							DgTmpList.Add(resultData);

						}

						//一般匯入有虛擬商品序號flag 註記, 一定要序號匯入才解除
						if (ImportType == 1)
						{
							CheckSerialImport = true;
							DgTmpList.Add(resultData);
						}
					}
					else
					{
						//非虛擬時直接新增
						DgTmpList.Add(resultData);
					}
				}
			}
			return errorItem;
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
									if (F160501QueryData != null && F160501QueryData.Count > 0)
									{
										if (F160501SelectData == null) { F160501SelectData = new F160501Data(); }
										IsExpandQuery = false;
										F160501SelectData = F160501QueryData.First();

									}
									else
									{
										DgF160502Data = null;
										DetailCtn = 0;
										ShowMessage(Messages.InfoNoData);
									}
								}
						);
			}
		}

		private void DoSearch()
		{
			var proxyEx = GetExProxy<P16ExDataSource>();
			var f160501QueryData = proxyEx.CreateQuery<F160501Data>("Get160501QueryData")
					.AddQueryExOption("dcItem", DcItemaQuery.Value)
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("destoryNo", F160501Query.DESTROY_NO)
					.AddQueryExOption("postingSDate", PostingSDate)
					.AddQueryExOption("postingEDate", PostingEDate)
					.AddQueryExOption("custOrdNo", F160501Query.CUST_ORD_NO)
					.AddQueryExOption("status", StatusItemQuery.Value)
					.AddQueryExOption("ordNo", OrdNoQuery)
					.AddQueryExOption("crtSDate", CrtSDate.HasValue ? CrtSDate.Value.ToString("yyyy/MM/dd") : "")
					.AddQueryExOption("crtEDate", CrtEDate.HasValue ? CrtEDate.Value.ToString("yyyy/MM/dd") : "");

			F160501QueryData = f160501QueryData.ToObservableCollection();
		}

		public bool CanEditImageUpload()
		{
			var proxyEx = GetExProxy<P16ExDataSource>();
			var f160501 = proxyEx.Get160501QueryData(F160501SelectData.DC_CODE, F160501SelectData.GUP_CODE, F160501SelectData.CUST_CODE, F160501SelectData.DESTROY_NO, null, null, null, null, null, null, null).ToList().FirstOrDefault();
			return f160501 != null && (f160501.STATUS == "2" || f160501.STATUS == "4");
		}

		#endregion Search

		#region AddDetail -Add Grid
		public ICommand AddDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				o => DoAddDetail(), () => UserOperateMode != OperateMode.Query, o => DoAddDetailCompleted()
				);
			}
		}
		private void DoAddDetail()
		{

		}
		private void DoAddDetailCompleted()
		{
			if (DgListAdd == null)
			{
				DgListAdd = new SelectionList<F160502Data>(new List<F160502Data>());
			}
			//檢查是否為虛擬商品
			var resultData = GetItemData(ItemCode);

			if (string.IsNullOrEmpty(ItemCode) || string.IsNullOrEmpty(ItemName)
																					|| (ItemScrapQty == 0 && resultData.VIRTUAL_TYPE == Resources.Resources.No) || ItemDestoryQty == 0)
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000_ScrapCountEmpty);
			}
			else if (ItemDestoryQty > ItemScrapQty && resultData.VIRTUAL_TYPE == Resources.Resources.No)
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000_DestroyCountInvalid);
			}
			else
			{
				var itemData = DgListAdd.Where(x => x.Item.ITEM_CODE == resultData.ITEM_CODE
																								&& x.Item.DC_CODE == resultData.DC_CODE
																								&& x.Item.GUP_CODE == resultData.GUP_CODE
																								&& x.Item.CUST_CODE == resultData.CUST_CODE).Select(x => x.Item).FirstOrDefault();

				if (resultData.BUNDLE_SERIALNO == "1" && resultData.VIRTUAL_TYPE == Resources.Resources.Yes)
					CheckSerialImport = true;

				if (itemData != null && !(resultData.BUNDLE_SERIALNO == "1" && resultData.VIRTUAL_TYPE == Resources.Resources.Yes))
				{
					DialogService.ShowMessage(Properties.Resources.P1605010000_ItemDuplicate1);
				}
				else if (resultData.BUNDLE_SERIALNO == "1" && resultData.VIRTUAL_TYPE == Resources.Resources.Yes && itemData != null)
				{
					itemData.DESTROY_QTY += ItemDestoryQty;
				}
				else
				{
					DgListAdd.Add(new SelectionItem<F160502Data>(new F160502Data
					{
						DC_CODE = DcItemaAdd.Value,
						GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode,
						CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode,
						ITEM_CODE = ItemCode,
						ITEM_NAME = ItemName,
						ITEM_SIZE = ItemSize,
						ITEM_SPEC = ItemSpec,
						ITEM_COLOR = ItemColor,
						VIRTUAL_TYPE = resultData.VIRTUAL_TYPE,
						BUNDLE_SERIALNO = resultData.BUNDLE_SERIALNO,
						SCRAP_QTY = ItemScrapQty,
						DESTROY_QTY = ItemDestoryQty,
					}));

				}

				ClearItemValue();
			}

		}

		#endregion

		#region DelDetail -Add Grid
		public ICommand DelDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				o => DoDelDetail(), () => UserOperateMode != OperateMode.Query && DgListAdd?.Where(item => item.IsSelected).Count()!=0
                );
			}
		}
		private void DoDelDetail()
		{
			if (DgListAdd != null)
			{
				//若是虛擬商品序號 , 數量與 DgSerialList 相同時 (刪除 DgSerialList)
				var DelDgListAdd = DgListAdd.Where(x => x.IsSelected && x.Item.VIRTUAL_TYPE == Resources.Resources.Yes
																						&& x.Item.BUNDLE_SERIALNO == "1").Select(x => x.Item).ToList();

				foreach (var item in DelDgListAdd)
				{
					var serialItem = DgSerialList.Where(x => x.ITEM_CODE == item.ITEM_CODE && x.DC_CODE == item.DC_CODE
																																									&& x.GUP_CODE == item.GUP_CODE
																																									&& x.CUST_CODE == item.CUST_CODE);
					if (serialItem != null)
					{
						DgSerialList = DgSerialList.Where(x => x.ITEM_CODE != item.ITEM_CODE).ToList();
					}
				}

				if (DgSerialList.Count() == 0)
					CheckSerialImport = false;

				DgListAdd = new SelectionList<F160502Data>(DgListAdd.Where(item => !item.IsSelected).Select(item => item.Item));
			}

			IsCheckAll = false;
		}
		#endregion

		#region GetSelectGvData
		public ICommand SelectGvDataCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
								o => DoGetGvData(), () => UserOperateMode == OperateMode.Query);
			}
		}

		private void DoGetGvData()
		{

			if (F160501SelectData != null)
			{
				DcItemaAdd = DcItemaQuery;
				IsCheckDistrCar = _f160501SelectData.DISTR_CAR == "1" ? true : false;
				//取銷毀明細
				var proxyP16 = GetExProxy<P16ExDataSource>();
				var f160502QueryData = proxyP16.CreateQuery<F160502Data>("Get160502DetailData")
				   	.AddQueryExOption("dcCode", F160501SelectData.DC_CODE ?? "")
						.AddQueryExOption("gupCode", F160501SelectData.GUP_CODE ?? "")
						.AddQueryExOption("custCode", F160501SelectData.CUST_CODE ?? "")
						.AddQueryExOption("destoryNo", F160501SelectData.DESTROY_NO ?? "");

				if (f160502QueryData != null)
				{
					DgF160502Data = f160502QueryData.ToObservableCollection();
					DetailCtn = DgF160502Data.Count;
				}
				else
				{
					DgF160502Data = null;
					DetailCtn = 0;
				}

				//取出貨明細
				if (F160501SelectData != null)
				{
					var proxyShare = GetExProxy<ShareExDataSource>();
					var f050801QueryData = proxyShare.CreateQuery<F050801WmsOrdNo>("GetF050801ListBySourceNo")
							.AddQueryExOption("dcCode", F160501SelectData.DC_CODE ?? "")
							.AddQueryExOption("gupCode", F160501SelectData.GUP_CODE ?? "")
							.AddQueryExOption("custCode", F160501SelectData.CUST_CODE ?? "")
							.AddQueryExOption("sourceNo", F160501SelectData.DESTROY_NO ?? "").ToList();
					if (f050801QueryData != null)
					{
						DgF050801Data = f050801QueryData.ToObservableCollection();
					}

				}
			}
		}
		#endregion

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
			//預設設定
			if (F160501AddData == null) F160501AddData = new F160501();
			ClearAddValue();
			if (DgSerialList == null) DgSerialList = new List<F160502Data>();
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoEdit(), () => UserOperateMode == OperateMode.Query && F160501SelectData != null && F160501SelectData.STATUS == "0"
						, o =>
						{
							//DoSearch();
						});
				;
			}
		}

		private void DoEdit()
		{

			if (F160501SelectData == null)
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000xamlcs_ChooseData);
			}
			else if (!CheckStatus(F160501SelectData.DC_CODE, F160501SelectData.GUP_CODE, F160501SelectData.CUST_CODE,F160501SelectData.DESTROY_NO))
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000xamlcs_StatusInvalidToModify);
			}
			else
			{
				if (DgSerialList == null) DgSerialList = new List<F160502Data>();

				foreach (var items in DgF160502Data)
				{
					ItemCode = items.ITEM_CODE;
					ItemName = items.ITEM_NAME;
					GetScrapItemStock();
					items.SCRAP_QTY = ItemScrapQty;
				}
				DgListAdd = new SelectionList<F160502Data>(DgF160502Data.ToList());

				//取序號資料 F160504
				var proxyP16 = GetExProxy<P16ExDataSource>();
				var f160504QueryData = proxyP16.CreateQuery<F160502Data>("Get160504SerialData")
						.AddQueryExOption("dcCode", F160501SelectData.DC_CODE)
						.AddQueryExOption("gupCode", F160501SelectData.GUP_CODE)
						.AddQueryExOption("custCode", F160501SelectData.CUST_CODE)
						.AddQueryExOption("destoryNo", F160501SelectData.DESTROY_NO ?? "");

				if (f160504QueryData != null)
				{
					DgSerialList = f160504QueryData.ToList();
				}
				ClearItemValue();
				UserOperateMode = OperateMode.Edit;
				_original_DISTR_CAR = F160501SelectData.DISTR_CAR;
				CanEditDistrCar = F160501SelectData.EDI_FLAG == "1" || F160501SelectData.EDI_FLAG == "2" ? false : true;
			}

			//執行編輯動作
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => { }, () => UserOperateMode != OperateMode.Query
						, o =>
						{
							DoCancel();
						});
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			if (UserOperateMode != OperateMode.Edit)
			{
				ClearAddValue();
				SearchCommand.Execute(null);
			}
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDelete(), () => UserOperateMode == OperateMode.Query && F160501SelectData != null && F160501SelectData.STATUS == "0"
								, o =>
								{
									SearchCommand.Execute(null);
								}
						);
			}
		}

		private void DoDelete()
		{

			if (F160501SelectData == null)
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000xamlcs_ChooseData);
			}
			else if (!CheckStatus(F160501SelectData.DC_CODE, F160501SelectData.GUP_CODE, F160501SelectData.CUST_CODE,F160501SelectData.DESTROY_NO))
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000xamlcs_StatusInvalidToModify);
			}
			else
			{
				if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
				{
					F160501 f160501Data = new F160501
					{
						DESTROY_NO = F160501SelectData.DESTROY_NO,
						DC_CODE = F160501SelectData.DC_CODE,
						GUP_CODE = F160501SelectData.GUP_CODE,
						CUST_CODE = F160501SelectData.CUST_CODE,
						DESTROY_DATE = F160501SelectData.DESTROY_DATE,
						DISTR_CAR = F160501SelectData.DISTR_CAR,
						STATUS = "0",
						MEMO = F160501SelectData.MEMO,
					};


					var proxy = new wcf.P16WcfServiceClient();
					var wcf160501 = ExDataMapper.Map<F160501, wcf.F160501>(f160501Data);
					var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteF160501s(wcf160501));

					var result = wcfResult.IsSuccessed;

					if (!result)
						DialogService.ShowMessage(wcfResult.Message);
				}
			}



		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				var result = false;
				return CreateBusyAsyncCommand(
						o => result = DoSave(),
						() => UserOperateMode != OperateMode.Query && DgListAdd?.Count()>0,
						o =>
						{
							if (result)
							{
								DoSaveCompleted();
								//SearchCommand.Execute(null);
							}
						}
						);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			bool result = false;

			//有虛擬 且 序號商品 未按匯入序號商品動作
			if (CheckSerialImport)
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000_ChooseVirtualData);
				return false;
			}
			if (UserOperateMode == OperateMode.Add)
			{
				result = DoSaveAdd();
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				result = DoSaveUpdate();
			}

			if (result)
			{
				//ClearAddValue();
				//ShowMessage(Messages.Success);
				//UserOperateMode = OperateMode.Query;
			}

			return result;

		}

		private bool DoSaveAdd()
		{
			var result = false;


			//檢查判斷
			result = CheckAddData();

			//若明細為空值 , 系統自動將報廢倉內所有品項與數量帶入並產生銷毀單 BY DC GUP CUST
			if (DgListAdd == null || DgListAdd.Count == 0)
			{
				if (DialogService.ShowMessage(Properties.Resources.P1605010000_DestroyAllScrapStockItemCheck, Resources.Resources.Information, DialogButton.YesNo, DialogImage.Question) == DialogResponse.No)
					return false;

					//報廢倉-序號商品
					var proxyEx = GetExProxy<P16ExDataSource>();
					var f1913ScrapData = proxyEx.CreateQuery<F160502Data>("GetF1913ScrapData")
								.AddQueryExOption("dcCode", DcItemaAdd.Value)
								.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
								.AddQueryExOption("custCode",  Wms3plSession.Get<GlobalInfo>().CustCode)
								.ToSelectionList();
					//報廢倉-虛擬序號商品


					var f1913ScrapVirtualData = f1913ScrapData.Where(o => o.Item.VIRTUAL_TYPE == Resources.Resources.Yes
																		&& o.Item.BUNDLE_SERIALNO == "1")
																.Select(o => o.Item).ToList();

					if (f1913ScrapData != null && f1913ScrapData.Any())
						DgListAdd = f1913ScrapData;
					if (f1913ScrapVirtualData != null && f1913ScrapVirtualData.Any())
						DgSerialList = f1913ScrapVirtualData;

					if (DgListAdd == null)
					{
						DialogService.ShowMessage(Properties.Resources.P1605010000_ScrapStockEmpty);
						return false;
					}
				
			}
			if (result)
			{
				F160501 f160501Data = new F160501
				{
					DC_CODE = DcItemaAdd.Value,
					GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode,
					CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode,
					DESTROY_DATE = F160501AddData.DESTROY_DATE,
					DISTR_CAR = F160501AddData.DISTR_CAR == null ? "0" : F160501AddData.DISTR_CAR.ToLower() == "true" ? "1" : "0",
					STATUS = "0",
					MEMO = F160501AddData.MEMO,
				};

				var list = (from i in DgListAdd select i.Item).ToList();
				var proxy = new wcf.P16WcfServiceClient();
				var wcf160501 = ExDataMapper.Map<F160501, wcf.F160501>(f160501Data);
				var wcfDetailData = ExDataMapper.MapCollection<F160502Data, wcf.F160502Data>(list).ToArray();
				var wcfSerialData = ExDataMapper.MapCollection<F160502Data, wcf.F160502Data>(DgSerialList).ToArray();
				var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
												() => proxy.InsertF160501s(wcf160501, wcfDetailData, wcfSerialData));

				result = wcfResult.IsSuccessed;

				if (result)
				{
					DialogService.ShowMessage(string.Format(Properties.Resources.P1605010000_InsertSuccess, wcfResult.Message));
					_destoryNo = wcfResult.Message;
					_original_DISTR_CAR = "";
					F160501Query.DESTROY_NO = wcfResult.Message;
				}
				else
				{
					DialogService.ShowMessage(wcfResult.Message);
				}
			}

			return result;
		}


		private bool DoSaveUpdate()
		{
			var result = false;

			if (!CheckStatus(F160501SelectData.DC_CODE, F160501SelectData.GUP_CODE, F160501SelectData.CUST_CODE,F160501SelectData.DESTROY_NO))
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000xamlcs_StatusInvalidToModify);
				return false;
			}

			result = CheckAddData();
			if (result)
			{
				F160501 f160501Data = new F160501
				{
					DESTROY_NO = F160501SelectData.DESTROY_NO,
					DC_CODE = F160501SelectData.DC_CODE,
					GUP_CODE = F160501SelectData.GUP_CODE,
					CUST_CODE = F160501SelectData.CUST_CODE,
					DESTROY_DATE = F160501SelectData.DESTROY_DATE,
					DISTR_CAR = F160501SelectData.DISTR_CAR,
					STATUS = "0",
					MEMO = F160501SelectData.MEMO,
				};

				var list = (from i in DgListAdd select i.Item).ToList();
				var proxy = new wcf.P16WcfServiceClient();
				var wcf160501 = ExDataMapper.Map<F160501, wcf.F160501>(f160501Data);
				var wcfDetailData = ExDataMapper.MapCollection<F160502Data, wcf.F160502Data>(list).ToArray();
				var wcfSerialData = ExDataMapper.MapCollection<F160502Data, wcf.F160502Data>(DgSerialList).ToArray();
				var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateF160501s(wcf160501, wcfDetailData, wcfSerialData));

				result = wcfResult.IsSuccessed;

				if (result)
				{
					DialogService.ShowMessage(string.Format(Properties.Resources.P1605010000_UpdateSuccess, F160501SelectData.DESTROY_NO));
					_destoryNo = F160501SelectData.DESTROY_NO;

				}
				else
				{
					DialogService.ShowMessage(wcfResult.Message);
				}

				F160501Query.DESTROY_NO = F160501SelectData.DESTROY_NO;

			}
			return result;
		}

		private void DoSaveCompleted()
		{
			//ShowMessage(Messages.Success);
			DoDistrCar(UserOperateMode);
			SearchCommand.Execute(null);
			ClearAddValue();
			UserOperateMode = OperateMode.Query;
		}

		private void DoDistrCar(OperateMode orginalOpMode)
		{
			F160501 f160501 = new F160501();
			if (orginalOpMode == OperateMode.Add)
			{
				f160501.DESTROY_NO = _destoryNo;
				f160501.DC_CODE = DcItemaAdd.Value;
				f160501.GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode;
				f160501.CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode;
				f160501.DESTROY_DATE = F160501AddData.DESTROY_DATE;
				f160501.DISTR_CAR = F160501AddData.DISTR_CAR == null ? "0" : F160501AddData.DISTR_CAR.ToLower() == "true" ? "1" : "0";
				f160501.STATUS = "0";
				f160501.MEMO = F160501AddData.MEMO;
				_original_DISTR_CAR = "";
			}
			else
			{
				f160501.DESTROY_NO = F160501SelectData.DESTROY_NO;
				f160501.DC_CODE = F160501SelectData.DC_CODE;
				f160501.GUP_CODE = F160501SelectData.GUP_CODE;
				f160501.CUST_CODE = F160501SelectData.CUST_CODE;
				f160501.DESTROY_DATE = F160501SelectData.DESTROY_DATE;
				f160501.DISTR_CAR = F160501SelectData.DISTR_CAR;
				f160501.STATUS = "0";
				f160501.MEMO = F160501SelectData.MEMO;
			}

			if (f160501.DISTR_CAR == "1" && _original_DISTR_CAR != "1")
			{
				if (ShowMessage(new MessagesStruct()
				{
					Message = Properties.Resources.P1605010000_ManuallyDelv,
					Button = DialogButton.YesNo,
					Title = Properties.Resources.P1605010000_Hint,
					Image = DialogImage.Question

				}) == DialogResponse.Yes)
				{

					var function = FormService.GetFunctionFromSession("P7001040000");
					if (function == null)
					{
						DialogService.ShowMessage(Properties.Resources.P1605010000_NoPermission);
					}
				}
			}
		}
		#endregion Save


		#region Shipment
		/// <summary>
		/// 出貨
		/// </summary>
		public ICommand ShipmentCommand
		{
			get
			{
				var isOk = false;
				return CreateBusyAsyncCommand(
						o => isOk = DoShipment(), () => UserOperateMode == OperateMode.Query && F160501SelectData!=null && F160501SelectData.STATUS == "0",
						o =>
						{
							if (isOk)
							{
								F160501Query.DESTROY_NO = F160501SelectData.DESTROY_NO;
								SearchCommand.Execute(null);
							}
								
						}
);
			}
		}

		public bool DoShipment()
		{
			var f19Proxy = GetProxy<F19Entities>();
			var f1909 = f19Proxy.F1909s.Where(x => x.GUP_CODE == Wms3plSession.Get<GlobalInfo>().GupCode && x.CUST_CODE == Wms3plSession.Get<GlobalInfo>().CustCode).FirstOrDefault();
			if (f1909 != null)
			{
				switch (f1909.DESTROY_TYPE)
				{
					case "0":
						break;
					case "1":
						if (ShowConfirmMessage(Properties.Resources.P1602020000_ConfirmSystemToMinusStock) == DialogResponse.No)
							return false;
						break;
				}
			}
			var wcf160501Data = ExDataMapper.Map<F160501Data, wcf.F160501Data>(F160501SelectData);
			var proxy = new wcf.P16WcfServiceClient();
			var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
											() => proxy.P160501Shipment(wcf160501Data));
			ShowWarningMessage(wcfResult.Message);
			return wcfResult.IsSuccessed;
		}
		#endregion Shipment



		#endregion

	}
}
