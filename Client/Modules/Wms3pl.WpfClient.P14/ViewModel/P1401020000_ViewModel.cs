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
using Wms3pl.WpfClient.DataServices.F14DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P14ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P14WcfService;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P14.ViewModel
{
	public partial class P1401020000_ViewModel : InputViewModelBase
	{
		public P1401020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				InitData();
			}

		}


		private void InitData()
		{
			//var userInfo = Wms3plSession.Get<UserInfo>();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();

			DcList = globalInfo.DcCodeList;
			if (DcList.Any())
				SelectDcCode = DcList.Select(x => x.Value).FirstOrDefault();//倉別選單在此載入

			InventoryStartDate = DateTime.Today;
			InventoryEndDate = DateTime.Today;
			IsWareHouse = true;
			IsDifferenceProduct = true;

			ReloadInventoryType();
			ReloadInventoryCycle();
			ReloadStatus();
		}

		#region Property

		public Action<PrintType> DoPrint = delegate { };

		/**********************[盤點單查詢]**********************/

		#region DC

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

		private string _selectedDcCode;

		public string SelectDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				if (_selectedDcCode == value)
					return;
				_selectedDcCode = value;
				ReloadWareHouse();
				RaisePropertyChanged("SelectDcCode");
			}
		}

		#endregion

		#region 類別(Inventory_Type)

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

		private string _selectedInventoryType;
		/// <summary>
		/// 選擇盤點類別
		/// </summary>
		public string SelectInventoryType
		{
			get { return _selectedInventoryType; }
			set
			{
				if (_selectedInventoryType == value)
					return;
				_selectedInventoryType = value;
				RaisePropertyChanged("SelectInventoryType");
			}
		}


		#endregion

		#region 週期(Inventory_Cycle)

		private List<NameValuePair<string>> _inventoryCycleList;

		public List<NameValuePair<string>> InventoryCycleList
		{
			get { return _inventoryCycleList; }
			set
			{
				_inventoryCycleList = value;
				RaisePropertyChanged("InventoryCycleList");
			}
		}

		private string _selectInventoryCycle;

		public string SelectInventoryCycle
		{
			get { return _selectInventoryCycle; }
			set
			{
				if (_selectInventoryCycle == value)
					return;
				_selectInventoryCycle = value;
				RaisePropertyChanged("SelectInventoryCycle");
			}
		}


		#endregion

		#region 年月(盤點年/月)

		private DateTime? _inventoryYearMonth;
		/// <summary>
		/// 選擇盤點年/月
		/// </summary>
		public DateTime? InventoryYearMonth
		{
			get { return _inventoryYearMonth; }
			set
			{
				if (_inventoryYearMonth == value)
					return;
				_inventoryYearMonth = value;
				RaisePropertyChanged("InventoryYearMonth");
			}
		}


		#endregion

		#region 盤點單號

		private string _inventoryNo;
		/// <summary>
		/// 盤點單號
		/// </summary>
		public string InventoryNo
		{
			get { return _inventoryNo; }
			set
			{
				if (_inventoryNo == value)
					return;
				_inventoryNo = value;
				RaisePropertyChanged("InventoryNo");
			}
		}

		#endregion

		#region 盤點起訖日

		private DateTime? _inventoryStartDate;
		/// <summary>
		/// 盤點開始日
		/// </summary>
		public DateTime? InventoryStartDate
		{
			get { return _inventoryStartDate; }
			set
			{
				if (_inventoryStartDate == value)
					return;
				_inventoryStartDate = value;
				RaisePropertyChanged("InventoryStartDate");
			}
		}

		private DateTime? _inventoryEndDate;
		/// <summary>
		/// 盤點結束日
		/// </summary>
		public DateTime? InventoryEndDate
		{
			get { return _inventoryEndDate; }
			set
			{
				if (_inventoryEndDate == value)
					return;
				_inventoryEndDate = value;
				RaisePropertyChanged("InventoryEndDate");
			}
		}

		#endregion

		#region 盤點狀態(STATUS)

		private List<NameValuePair<string>> _statusList;
		/// <summary>
		/// 盤點狀態
		/// </summary>
		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				_statusList = value;
				RaisePropertyChanged("StatusList");
			}
		}

		private string _selectStatus;
		/// <summary>
		/// 選擇盤點狀態
		/// </summary>
		public string SelectStatus
		{
			get { return _selectStatus; }
			set
			{
				if (_selectStatus == value)
					return;
				_selectStatus = value;
				RaisePropertyChanged("SelectStatus");
			}
		}


		#endregion

		/**********************[盤點單查詢結果]**********************/

		#region 盤點單資料

		private List<F140101Expansion> _f140101List;

		public List<F140101Expansion> F140101List
		{
			get { return _f140101List; }
			set
			{
				if (_f140101List == value)
					return;
				Set(() => F140101List, ref _f140101List, value);
			}
		}

		private F140101Expansion _selectedF140101;

		public F140101Expansion SelectedF140101
		{
			get { return _selectedF140101; }
			set
			{
				if (_selectedF140101 == value)
					return;
				Set(() => SelectedF140101, ref _selectedF140101, value);
                IsWareHouse = true;
                SelectedWareHouseId = "";

        if (InventoryDetailData != null) {
          InventoryDetailData.Clear(); 
          RaisePropertyChanged("InventoryDetailData");
        }
      }
		}
		#endregion

		/**********************[盤點單明細查詢]**********************/

		#region 倉別

		private bool _isWareHouse;
		/// <summary>
		/// 是否選擇指定倉別
		/// </summary>
		public bool IsWareHouse
		{
			get { return _isWareHouse; }
			set
			{
				if (_isWareHouse == value)
					return;
				_isWareHouse = value;
				RaisePropertyChanged("IsWareHouse");
				IsItemCode = !_isWareHouse;
                ClearItem();

            }
		}

		private List<NameValuePair<string>> _wareHouseList;
		/// <summary>
		/// 倉別資料
		/// </summary>
		public List<NameValuePair<string>> WareHouseList
		{
			get { return _wareHouseList; }
			set
			{
				_wareHouseList = value;
				RaisePropertyChanged("WareHouseList");
			}
		}
		private string _selectedWareHouseId;
		/// <summary>
		/// 選擇倉別
		/// </summary>
		public string SelectedWareHouseId
		{
			get { return _selectedWareHouseId; }
			set
			{
				_selectedWareHouseId = value;
				RaisePropertyChanged("SelectedWareHouseId");
			}
		}
		#endregion

		#region 商品搜尋用欄位

		private bool _isItemCode;

		public bool IsItemCode
		{
			get { return _isItemCode; }
			set
			{
				if (_isItemCode == value)
					return;
				_isItemCode = value;
				RaisePropertyChanged("IsItemCode");
				IsWareHouse = !_isItemCode;
                SelectedWareHouseId = "";
                ProductData = null;
            }
		}

		private string _itemCode;
		/// <summary>
		/// 商品品號
		/// </summary>
		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				_itemCode = value;
				RaisePropertyChanged("ItemCode");
			}
		}
		private string _itemName;
		/// <summary>
		/// 商品品名
		/// </summary>
		public string ItemName
		{
			get { return _itemName; }
			set
			{
				_itemName = value;
				RaisePropertyChanged("ItemName");
			}
		}
		private string _itemSize;
		public string ItemSize
		{
			get { return _itemSize; }
			set
			{
				_itemSize = value;
				RaisePropertyChanged("ItemSize");
			}
		}
		private string _itemSpec;
		public string ItemSpec
		{
			get { return _itemSpec; }
			set
			{
				_itemSpec = value;
				RaisePropertyChanged("ItemSpec");
			}
		}
		private string _itemColor;
		public string ItemColor
		{
			get { return _itemColor; }
			set
			{
				_itemColor = value;
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


		#endregion

		#region 儲存輸入品號DataGrid
		private List<EasyProductData> _productData;
		/// <summary>
		/// 盤點單明細資料
		/// </summary>
		public List<EasyProductData> ProductData
		{
			get { return _productData; }
			set
			{
				if (_productData == value)
					return;
				_productData = value;
				RaisePropertyChanged("ProductData");
			}
		}

		private EasyProductData _selectProductData;
		public EasyProductData SelectProductData
		{
			get { return _selectProductData; }
			set
			{
				if (_selectProductData == value)
					return;
				_selectProductData = value;
				RaisePropertyChanged("SelectProductData");
			}
		}
		#endregion

		#region 篩選商品條件

		private bool _isAllProduct;
		/// <summary>
		/// 全部商品
		/// </summary>
		public bool IsAllProduct
		{
			get { return _isAllProduct; }
			set
			{
				if (_isAllProduct == value)
					return;

				_isAllProduct = value;
				RaisePropertyChanged("IsAllProduct");
			}
		}

		private bool _isDifferenceProduct;
		/// <summary>
		/// 全部有差異商品
		/// </summary>
		public bool IsDifferenceProduct
		{
			get { return _isDifferenceProduct; }
			set
			{
				if (_isDifferenceProduct == value)
					return;
				_isDifferenceProduct = value;
				RaisePropertyChanged("IsDifferenceProduct");
			}
		}

		private bool _isRangeProduct;
		/// <summary>
		/// 差異數量介於
		/// </summary>
		public bool IsRangeProduct
		{
			get { return _isRangeProduct; }
			set
			{
				if (_isRangeProduct == value)
					return;
				_isRangeProduct = value;
				RaisePropertyChanged("IsRangeProduct");
			}
		}

		private int? _rangeStart;
		/// <summary>
		/// 差異數量比對開始值
		/// </summary>
		public int? RangeStart
		{
			get { return _rangeStart; }
			set
			{
				if (_rangeStart == value)
					return;
				_rangeStart = value;
				RaisePropertyChanged("RangeStart");
			}
		}

		private int? _rangeEnd;
		/// <summary>
		/// 差異數量比對結束值
		/// </summary>
		public int? RangeEnd
		{
			get { return _rangeEnd; }
			set
			{
				if (_rangeEnd == value)
					return;
				_rangeEnd = value;
				RaisePropertyChanged("RangeEnd");
			}
		}

		#endregion

		/**********************[盤點單明細查詢結果]**********************/

		#region 盤點單明細資料

		private ObservableCollection<InventoryDetailItemsByIsSecond> _inventoryDetail;
		/// <summary>
		/// 盤點單明細資料
		/// </summary>
		public ObservableCollection<InventoryDetailItemsByIsSecond> InventoryDetailData
		{
			get { return _inventoryDetail; }
			set
			{
				if (_inventoryDetail == value)
					return;
				_inventoryDetail = value;
				RaisePropertyChanged("InventoryDetailData");
			}
		}

		private InventoryDetailItemsByIsSecond _selectInventoryDetailData;
		public InventoryDetailItemsByIsSecond SelectInventoryDetailData
		{
			get { return _selectInventoryDetailData; }
			set
			{
				if (_selectInventoryDetailData == value)
					return;
				_selectInventoryDetailData = value;
				RaisePropertyChanged("SelectInventoryDetailData");
			}
		}

		private bool _isEditSelectedAll = false;
		public bool IsEditSelectedAll
		{
			get { return _isEditSelectedAll; }
			set { _isEditSelectedAll = value; RaisePropertyChanged("IsEditSelectedAll"); }
		}

		#endregion

		#region 紀錄上一次條件一、二資料
		private bool _lastTimeIsWareHouse { get; set; }
		private string _lastTimeSelectedWareHouseId { get; set; }
		private bool _lastTimeIsDifferenceProduct { get; set; }
		#endregion

		#endregion

		#region Search 盤點單主檔
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearch(),
						() => UserOperateMode == OperateMode.Query
						);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
			string message;
			if (ValidateHelper.TryCheckBeginEnd(this, x => x.InventoryStartDate, x => x.InventoryEndDate, Properties.Resources.QueryInventorySDate, out message))
			{
				var globalInfo = Wms3plSession.Get<GlobalInfo>();
				//var proxy = GetProxy<F14Entities>();
				var proxyEx = GetExProxy<P14ExDataSource>();
				F140101List = proxyEx.CreateQuery<F140101Expansion>("GetDatasExpansion")
					.AddQueryOption("dcCode", QueryFormat(SelectDcCode))
					.AddQueryOption("gupCode", QueryFormat(globalInfo.GupCode))
					.AddQueryOption("custCode", QueryFormat(globalInfo.CustCode))
					.AddQueryExOption("inventoryNo", InventoryNo)
					.AddQueryOption("inventoryType", QueryFormat(SelectInventoryType))
					.AddQueryOption("inventorySDate",
						QueryFormat(((InventoryStartDate.HasValue) ? InventoryStartDate.Value.ToString("yyyy/MM/dd") : string.Empty)))
					.AddQueryOption("inventoryEDate",
						QueryFormat(((InventoryEndDate.HasValue) ? InventoryEndDate.Value.ToString("yyyy/MM/dd") : "")))
					.AddQueryOption("inventoryCycle",
						QueryFormat(SelectInventoryCycle))
					.AddQueryOption("inventoryYear",
						QueryFormat(InventoryYearMonth.HasValue ? InventoryYearMonth.Value.Year.ToString() : string.Empty))
					.AddQueryOption("inventoryMonth",
						QueryFormat(InventoryYearMonth.HasValue ? InventoryYearMonth.Value.Month.ToString() : string.Empty))
					.AddQueryOption("status",
						QueryFormat(SelectStatus))
					.ToList();
				if (!F140101List.Any())
					ShowMessage(Messages.InfoNoData);
				InventoryDetailData = null;
                SelectedF140101 = F140101List.FirstOrDefault();

            }
			else
				ShowWarningMessage(message);
		}
		#endregion Search

		#region Add 指定商品
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoAdd(),
						() => UserOperateMode == OperateMode.Query && IsItemCode
						);
			}
		}

		private void DoAdd()
		{
			//UserOperateMode = OperateMode.Add;
			//執行新增動作

			var easyProduct = new EasyProductData() { ProductId = ItemCode, ProductName = ItemName };

			if (ProductData == null)
			{
				ProductData = new List<EasyProductData>();
			}
			if (string.IsNullOrWhiteSpace(ItemCode))
			{
				ShowWarningMessage(Properties.Resources.P1401020000_ItemCodeIsEmpty);
				return;
			}
			if (ProductData.Any(x => x.ProductId == ItemCode))
			{
				ShowWarningMessage(Properties.Resources.P1401020000_AlreadyExistItem);
				return;
			}
			var tmpProductData = ProductData.ToList();
			tmpProductData.Add(easyProduct);
			ProductData = tmpProductData;
			//新增完後清除欄位
			ClearItem();
		}
		#endregion Add

		#region Delete 指定商品
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDelete(),
						() => UserOperateMode == OperateMode.Query && IsItemCode
						);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (ProductData != null && ProductData.Any() && SelectProductData != null)
			{
				var tmp = ProductData.ToList();
				tmp.Remove(SelectProductData);
				ProductData = tmp;
			}
		}
		#endregion Delete

		#region Search 盤點單明細資料
		public ICommand SearchDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearchDetail(),
						() => CanSerachDetail() //UserOperateMode == OperateMode.Query
						);
			}
		}

		/// <summary>
		/// 檢查查詢明細資料的條件
		/// </summary>
		private bool CanSerachDetail(bool showMessage = false)
		{
			var errorMsg = string.Empty;
			var check = true;
			if (F140101List != null && F140101List.Any() && SelectedF140101 != null)
			{
				if (IsItemCode)//選擇指定商品
				{
					//未輸入任何商品資料
					if (ProductData == null || !ProductData.Any())
					{
						errorMsg = Properties.Resources.P1401020000_ItemIsEmpty;
						check = false;
					}
				}
				if (IsRangeProduct)//選擇差異區間
				{
					if (RangeStart.HasValue == false || RangeEnd.HasValue == false)
					{
						errorMsg = Properties.Resources.P1401020000_RangeDiffIsEmpty;
						check = false;
					}
					//if (RangeStart.Value > RangeEnd.Value)
					//{
					//	var tmpEnd = RangeStart.Value;
					//	RangeStart = RangeEnd.Value;
					//	RangeEnd = tmpEnd;
					//}
				}
			}
			else
			{
				errorMsg = Properties.Resources.P1401020000_SelectInventoryDataFirst;
				check = false;
			}

			if (check && showMessage)
			{
				ShowWarningMessage(errorMsg);
			}
			return check;
		}

		private void DoSearchDetail()
		{
			var result = CheckInventoryDetailHasEnterQty();
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return;
			}
			InventoryDetailData = GetInventoryDetailItems();
			if (!InventoryDetailData.Any())
				ShowMessage(Messages.InfoNoData);
		}

		public ObservableCollection<InventoryDetailItemsByIsSecond> GetInventoryDetailItems(string isReportTag = "0")
		{
			JavaScriptSerializer js = new JavaScriptSerializer();

			// 紀錄上一次條件一、二參數
			var lastTimeIsWareHouse = js.Serialize(IsWareHouse);
			_lastTimeIsWareHouse = js.Deserialize<bool>(lastTimeIsWareHouse);
			var lastTimeSelectedWareHouseId = js.Serialize(SelectedWareHouseId);
			_lastTimeSelectedWareHouseId = js.Deserialize<string>(lastTimeSelectedWareHouseId);
			var lastTimeIsDifferenceProduct = js.Serialize(IsDifferenceProduct);
			_lastTimeIsDifferenceProduct = js.Deserialize<bool>(lastTimeIsDifferenceProduct);

			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyEx = GetExProxy<P14ExDataSource>();
			var result = proxyEx.CreateQuery<InventoryDetailItemsByIsSecond>("GetInventoryDetailItemsByIsSecond")
				.AddQueryOption("dcCode", QueryFormat(SelectDcCode))
				.AddQueryOption("gupCode", QueryFormat(globalInfo.GupCode))
				.AddQueryOption("custCode", QueryFormat(globalInfo.CustCode))
				.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO)
				.AddQueryOption("isSecond", QueryFormat(SelectedF140101.ISSECOND))
				.AddQueryOption("wareHouseId", IsWareHouse ? QueryFormat(SelectedWareHouseId) : "")
				.AddQueryOption("itemCodes", IsItemCode ? QueryFormat(GetEnterItemCodes()) : "")
				//基本上畫面有檔，若選Range，則必須有填值，否則查詢按鈕將不顯示
				.AddQueryOption("differencesRangeStart", QueryFormat(IsDifferenceProduct ? "" : RangeStart.ToString()))
				.AddQueryOption("differencesRangeEnd", QueryFormat(IsDifferenceProduct ? "" : RangeEnd.ToString()))
				.AddQueryOption("isRepotTag", QueryFormat(isReportTag))
				.AddQueryOption("showCnt", QueryFormat("1"))
				//.AddQueryOption("status", QueryFormat(status))
				.ToObservableCollection();

			IsEditSelectedAll = false;
			return result;
		}

		public List<P140102ReportData> GetReportData()
		{
			JavaScriptSerializer js = new JavaScriptSerializer();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyEx = GetExProxy<P14ExDataSource>();
			var result = proxyEx.CreateQuery<P140102ReportData>("GetP140102ReportData")
				.AddQueryOption("dcCode", QueryFormat(SelectDcCode))
				.AddQueryOption("gupCode", QueryFormat(globalInfo.GupCode))
				.AddQueryOption("custCode", QueryFormat(globalInfo.CustCode))
				.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO)
				.AddQueryOption("isSecond", QueryFormat(SelectedF140101.ISSECOND))
				.ToList();

			IsEditSelectedAll = false;
			return result;
		}

		public ExecuteResult CheckInventoryDetailHasEnterQty()
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyEx = GetExProxy<P14ExDataSource>();
			return proxyEx.CreateQuery<ExecuteResult>("CheckInventoryDetailHasEnterQty")
				.AddQueryOption("dcCode", QueryFormat(SelectDcCode))
				.AddQueryOption("gupCode", QueryFormat(globalInfo.GupCode))
				.AddQueryOption("custCode", QueryFormat(globalInfo.CustCode))
				.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO).First();
		}

		/// <summary>
		/// 將ItemCode用'^'串接起來
		/// </summary>
		private string GetEnterItemCodes()
		{
			var itemCodes = string.Empty;
			if (!IsWareHouse && ProductData != null && ProductData.Any())
			{
				itemCodes = ProductData.Aggregate(itemCodes, (x, data) => x + (data.ProductId + '^'));
			}
			return itemCodes.TrimEnd('^');
		}

		#region CheckAll
		public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCheckAllItem()
				);
			}
		}

		public void DoCheckAllItem()
		{
			if (InventoryDetailData != null)
				foreach (var p in InventoryDetailData)
					p.IsSelected = IsEditSelectedAll;
		}
		#endregion

		#endregion

		#region RegenerateInventory 重新產生複盤資料
		public ICommand RegenerateInventoryCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoRegenerateInventory(),
						() => F140101List != null && F140101List.Any() && SelectedF140101 != null //UserOperateMode == OperateMode.Query
						);
			}
		}

		private void DoRegenerateInventory()
		{
			//檢核若尚未初盤點完成，提示[此盤點單XX未完成初盤，無法產生複盤資料]。
			var checkFirstIsFinishRes = CheckInventoryIsFinish();
			if (!checkFirstIsFinishRes.IsSuccessed)
			{
				ShowWarningMessage(checkFirstIsFinishRes.Message);
				return;
			}

			// 檢核上一次條件一是否為倉別 And 是否倉別為全部 And 條件二是否為全部有差異商品
			if (!(_lastTimeIsWareHouse && _lastTimeIsDifferenceProduct && string.IsNullOrWhiteSpace(_lastTimeSelectedWareHouseId)))
			{
				ShowWarningMessage(Properties.Resources.P1401020000_RegenerateInventoryConditionError);
				return;
			}

			// 若沒有任何盤點差異筆數(盤點差異數都為0)，提示訊息"本次盤點無差異，不需建立複盤單 "
			if (InventoryDetailData != null && InventoryDetailData.All(x => Convert.ToInt32(x.DIFF_QTY) == 0 && Convert.ToInt32(x.STOCK_DIFF_QTY) == 0))
			{
				ShowWarningMessage(Properties.Resources.P1401020000_NotNeedRegenerate);
				return;
			}

			//複盤差異數資料已存在，確認清空資料重新產生複盤資料
			var check = CheckF140105Exist();
			var confirm = true;
			if (check.IsSuccessed)
			{
				confirm = ShowConfirmMessage(Properties.Resources.P1401020000_CheckingRegenerPluralDiffData) == DialogResponse.Yes;
			}

			if (confirm)
			{
				var globalInfo = Wms3plSession.Get<GlobalInfo>();
				var proxyEx = GetExProxy<P14ExDataSource>();
				var result = proxyEx.CreateQuery<ExecuteResult>("ReInsertF140105")
					.AddQueryOption("dcCode", string.Format("'{0}'", SelectDcCode))
					.AddQueryOption("gupCode", string.Format("'{0}'", globalInfo.GupCode))
					.AddQueryOption("custCode", string.Format("'{0}'", globalInfo.CustCode))
					.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO)
					.AddQueryOption("clientName", string.Format("'{0}'", ReadRdpClientSessionInfo.GetRdpClientName()))
					.ToList().FirstOrDefault();
				ShowResultMessage(result);
				if (result.IsSuccessed)
				{
					ReSearch();
				}
			}

		}

		public ExecuteResult CheckF140105Exist()
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyEx = GetExProxy<P14ExDataSource>();
			return proxyEx.CreateQuery<ExecuteResult>("CheckF140105Exist")
				.AddQueryOption("dcCode", QueryFormat(SelectDcCode))
				.AddQueryOption("gupCode", QueryFormat(globalInfo.GupCode))
				.AddQueryOption("custCode", QueryFormat(globalInfo.CustCode))
				.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO)
				.ToList().FirstOrDefault();
		}

		public ExecuteResult CheckInventoryIsFinish()
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyEx = GetExProxy<P14ExDataSource>();
			return proxyEx.CreateQuery<ExecuteResult>("CheckInventoryIsFinish")
				.AddQueryOption("dcCode", QueryFormat(SelectDcCode))
				.AddQueryOption("gupCode", QueryFormat(globalInfo.GupCode))
				.AddQueryOption("custCode", QueryFormat(globalInfo.CustCode))
				.AddQueryExOption("inventoryNo", SelectedF140101.INVENTORY_NO)
				.ToList().FirstOrDefault();
		}

		#endregion

		#region SaveInventory 盤點確認
		public ICommand SaveInventoryCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSaveInventory(),
						() => F140101List != null && F140101List.Any() && SelectedF140101 != null //UserOperateMode == OperateMode.Query
						);
			}
		}

		private void DoSaveInventory()
		{
			// 檢核是否有查詢
			if (InventoryDetailData == null)
			{
				ShowWarningMessage(Properties.Resources.P1401020000_CheckingIsCanSaveInventoryData);
				return;
			}

			// 檢核上一次條件一是否為倉別 And 是否倉別為全部 And 條件二是否為全部有差異商品
			if (!(_lastTimeIsWareHouse && _lastTimeIsDifferenceProduct && string.IsNullOrWhiteSpace(_lastTimeSelectedWareHouseId)))
			{
				ShowWarningMessage(Properties.Resources.P1401020000_SaveInventoryConditionError);
				return;
			}

			// 檢核是否已完成盤點
			var checkResult = CheckInventoryDetailHasEnterQty();
			if (!checkResult.IsSuccessed)
			{
				if (ShowConfirmMessage(Properties.Resources.P1401020000_CheckingGenerInventoryDiff) == DialogResponse.No)
					return;
			}

			// 檢核是否有勾選
			if(!InventoryDetailData.Where(x => x.IsSelected).Any())
			{
				if (ShowConfirmMessage(Properties.Resources.P1401020000_NotDiffMsg) == DialogResponse.No)
					return;
			}

            var datas = ExDataMapper.MapCollection<InventoryDetailItemsByIsSecond, wcf.InventoryDetailItemsByIsSecond>(InventoryDetailData);
            var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyWcf = new wcf.P14WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
				() => proxyWcf.InsertF140106(SelectDcCode, globalInfo.GupCode, globalInfo.CustCode, SelectedF140101.INVENTORY_NO, string.Format("'{0}'", ReadRdpClientSessionInfo.GetRdpClientName()), datas.ToArray()));

			ShowResultMessage(result);
			if (result.IsSuccessed)
			{
				ReSearch();
			}
		}
		#endregion

		#region CaseClosed 結案
		public ICommand CaseClosedCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCaseClosed(),
						() => F140101List != null && F140101List.Any() && SelectedF140101 != null
						);
			}
		}

		private void DoCaseClosed()
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyWcf = new wcf.P14WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
				() => proxyWcf.CaseClosedP140102(SelectDcCode, globalInfo.GupCode, globalInfo.CustCode, SelectedF140101.INVENTORY_NO));

			ShowResultMessage(result);
			if (result.IsSuccessed)
				ReSearch();
		}
		#endregion

		#region Data
		/// <summary>
		/// 重新載入[倉別]資料 - 選單類別
		/// </summary>
		private void ReloadWareHouse()
		{
			var proxy = GetProxy<F19Entities>();
			WareHouseList = (from o in proxy.F1980s
							 where o.DC_CODE == SelectDcCode
							 select new NameValuePair<string>
							 {
								 Name = o.WAREHOUSE_NAME,
								 Value = o.WAREHOUSE_ID
							 }).ToList();
			WareHouseList.Insert(0, new NameValuePair<string> { Value = "", Name = Resources.Resources.All });
			SelectedWareHouseId = WareHouseList.Select(x => x.Value).FirstOrDefault();
		}

		/// <summary>
		/// 重新載入[盤點類別]
		/// </summary>
		private void ReloadInventoryType()
		{
			InventoryTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F140101", "INVENTORY_TYPE", true);
			SelectInventoryType = InventoryTypeList.Select(x => x.Value).FirstOrDefault();
		}
		/// <summary>
		/// 重新載入[週期]
		/// </summary>
		private void ReloadInventoryCycle()
		{
			InventoryCycleList = GetBaseTableService.GetF000904List(FunctionCode, "F140101", "INVENTORY_CYCLE", true);
			SelectInventoryCycle = InventoryTypeList.Select(x => x.Value).FirstOrDefault();
		}
		/// <summary>
		/// 重新載入[盤點狀態]
		/// </summary>
		private void ReloadStatus()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F140102", "STATUS", true);
			SelectStatus = InventoryTypeList.Select(x => x.Value).FirstOrDefault();
		}

		private void ClearItem()
		{
			ItemCode = string.Empty;
			ItemColor = string.Empty;
			ItemName = string.Empty;
			ItemSize = string.Empty;
			ItemSpec = string.Empty;
            SerialNo = string.Empty;
		}

		#endregion

		#region Print Or Preview
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
				(t) =>
				{
					IsBusy = true;
					try
					{
						DoPrint(t);
					}
					catch (Exception ex)
					{
						Exception = ex;
						IsBusy = false;
					}
					IsBusy = false;
				},
				(t) => true);
			}
		}


		#endregion

		#region 重新查詢畫面資料

		private void ReSearch()
		{
			DoSearch();

			SelectedF140101 = null;
			InventoryDetailData = new ObservableCollection<InventoryDetailItemsByIsSecond>();
			/*
			//產生複盤單&盤點確認，皆會重新查詢，若是
			//1. 產生複盤單 => SelectedF140101 應該保留，重新查詢後仍要選取在該盤點單
			//2. 盤點確認   => SelectedF140101 清空，Detail清空
			if (F140101List.Any(x => x.INVENTORY_NO == SelectedF140101.INVENTORY_NO))//產生複盤單
			{
				DoSearchDetail();
			}
			else//盤點確認
			{
				SelectedF140101 = null;
				InventoryDetailData = new List<InventoryDetailItemsByIsSecond>();
			}*/
		}

		#endregion



	}

	public class EasyProductData
	{
		public string ProductId { get; set; }
		public string ProductName { get; set; }
	}

}
