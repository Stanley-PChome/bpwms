using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F25DataService;

namespace Wms3pl.WpfClient.UcLib.ViewModel
{
	public partial class WinSearchProduct_ViewModel : InputViewModelBase
	{
		/// <summary>
		/// 商品搜尋最大筆數
		/// </summary>
		const int ItemSearchMaximum = 100;
		/// <summary>
		/// 背景自動搜尋商品的品號長度條件最少要幾碼
		/// </summary>
		const int TempSearchItemCodeLowestLength = 6;
		/// <summary>
		/// 選擇品號搜尋時，可以讓搜尋按鈕搜尋的長度
		/// </summary>
		const int CanSearchItemCodeLowestLength = 3;

		public Action DoExit = delegate { };
		public Action SetSearchItemCodeFocus = delegate { };

		private string _gupCode;

		public string GupCode
		{
			get { return _gupCode; }
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_gupCode = value;
				}
			}
		}

		private string _custCode;

		public string CustCode
		{
			get { return _custCode; }
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_custCode = value;
				}
			}
		}

		/// <summary>
		/// 是否從快取搜尋
		/// </summary>
		public bool IsFromCahceSearchItemCode
		{
			get
			{
				return (TempCustItemList != null && TempCustItemList.Any())
				&& (TempItemList != null && TempItemList.Any())
				&& (SearchItemCode != null && SearchItemCode.Length < TempSearchItemCodeLowestLength);
			}
		}
		
		public WinSearchProduct_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();

			}

		}

		private void InitControls()
		{
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		}

		#region 查詢條件
		/// <summary>
		/// 查詢之品號
		/// </summary>
		private string _searchItemCode = string.Empty;
		public string SearchItemCode
		{
			get
			{
				return _searchItemCode;
			}
			set
			{
				_searchItemCode = value;
				if (_searchItemCode.Length == TempSearchItemCodeLowestLength)
				{
					SearchTempCommand.Execute(null);
				}
				else if (_searchItemCode.Length > TempSearchItemCodeLowestLength)
				{
					if (TempItemList != null)
					{
						SetSearchConditions();
					}
					else
					{
						SearchTempCommand.Execute(null);
					}
				}
				else
				{
					if (TempItemList != null)
					{
						TempItemList.Clear();
						ItemList = null;
						ItemSizes = null;
						ItemSpecs = null;
						ItemColors = null;
					}
				}
				RaisePropertyChanged("SearchItemCode");
			}
		}

		#region 尺寸
		private List<NameValuePair<string>> _itemSizes;
		public List<NameValuePair<string>> ItemSizes { get { return _itemSizes; } set { _itemSizes = value; RaisePropertyChanged("ItemSizes"); } }
		private string _searchItemSize;
		public string SearchItemSize
		{
			get { return _searchItemSize; }
			set
			{
				_searchItemSize = value;
				RaisePropertyChanged("SearchItemSize");
			}
		}
		#endregion

		#region 規格
		private List<NameValuePair<string>> _itemSpecs;
		public List<NameValuePair<string>> ItemSpecs { get { return _itemSpecs; } set { _itemSpecs = value; RaisePropertyChanged("ItemSpecs"); } }
		private string _searchItemSpec;
		public string SearchItemSpec
		{
			get { return _searchItemSpec; }
			set
			{
				_searchItemSpec = value;
				RaisePropertyChanged("SearchItemSpec");
			}
		}
		#endregion

		#region 顏色
		private List<NameValuePair<string>> _itemColors;
		public List<NameValuePair<string>> ItemColors { get { return _itemColors; } set { _itemColors = value; RaisePropertyChanged("ItemColors"); } }
		private string _searchItemColor;
		public string SearchItemColor
		{
			get { return _searchItemColor; }
			set
			{
				_searchItemColor = value;
				RaisePropertyChanged("SearchItemColor");
			}
		}
		#endregion



		private string _searchItemName = string.Empty;

		public string SearchItemName
		{
			get { return _searchItemName; }
			set
			{
				Set(() => SearchItemName, ref _searchItemName, value);
			}
		}

		private string _searchEanCode = string.Empty;

		public string SearchEanCode
		{
			get { return _searchEanCode; }
			set
			{
				Set(() => SearchEanCode, ref _searchEanCode, value);
			}
		}

		#region 序號
		private string _searchSerialNo = string.Empty;
		public string SearchSerialNo
		{
			get
			{
				return _searchSerialNo;
			}
			set
			{
				Set(() => SearchSerialNo, ref _searchSerialNo, value);
			}
		}
		#endregion

		private bool? _isItemCodeChecked = true;

		public bool? IsItemCodeChecked
		{
			get { return _isItemCodeChecked; }
			set
			{
				Set(() => IsItemCodeChecked, ref _isItemCodeChecked, value);
				SearchItemName = string.Empty;
				SearchSerialNo = string.Empty;
				//國際條碼外部傳入不初始化SearchEanCode
				SearchEanCode = IsEanCode? SearchEanCode:string.Empty;
			}
		}

		private bool? _isItemNameChecked = false;

		public bool? IsItemNameChecked
		{
			get { return _isItemNameChecked; }
			set
			{
				Set(() => IsItemNameChecked, ref _isItemNameChecked, value);
				SearchItemCode = string.Empty;
				SearchSerialNo = string.Empty;
				SearchEanCode = string.Empty;
			}
		}

		#region 是否由外部傳入的EanCode
		private bool _isEanCode;
		public bool IsEanCode
		{
			get { return _isEanCode; }
			set { Set(() => IsEanCode, ref _isEanCode, value); }
		}
		#endregion

		private bool? _isSerialNoChecked = false;
		public bool? IsSerialNoChecked
		{
			get { return _isSerialNoChecked; }
			set
			{
				Set(() => IsSerialNoChecked, ref _isSerialNoChecked, value);
				SearchItemCode = string.Empty;
				SearchItemName = string.Empty;
				SearchEanCode = string.Empty;
			}
		}

		private bool? _isEanCodeChecked = false;
		public bool? IsEanCodeChecked
		{
			get { return _isEanCodeChecked; }
			set
			{
				Set(() => IsEanCodeChecked, ref _isEanCodeChecked, value);
				SearchItemCode = string.Empty;
				SearchItemName = string.Empty;
				SearchSerialNo = string.Empty;
			}
		}


		private void SetSearchConditions()
		{
			var itemSizes = TempItemList.Where(a => a.ITEM_CODE.StartsWith(SearchItemCode) && !string.IsNullOrEmpty(a.ITEM_SIZE)).Select(a => new NameValuePair<string>
			{
				Name = a.ITEM_SIZE,
				Value = a.ITEM_SIZE
			}).Distinct().ToList();
			itemSizes.Insert(0, new NameValuePair<string> { Name = "全部", Value = "All" });
			ItemSizes = itemSizes;

			var itemSpecs = TempItemList.Where(a => a.ITEM_CODE.StartsWith(SearchItemCode) && !string.IsNullOrEmpty(a.ITEM_SPEC)).Select(a => new NameValuePair<string>
			{
				Name = a.ITEM_SPEC,
				Value = a.ITEM_SPEC
			}).Distinct().ToList();
			itemSpecs.Insert(0, new NameValuePair<string> { Name = "全部", Value = "All" });
			ItemSpecs = itemSpecs;

			var itemColors = TempItemList.Where(a => a.ITEM_CODE.StartsWith(SearchItemCode) && !string.IsNullOrEmpty(a.ITEM_COLOR)).Select(a => new NameValuePair<string>
			{
				Name = a.ITEM_COLOR,
				Value = a.ITEM_COLOR
			}).Distinct().ToList();
			itemColors.Insert(0, new NameValuePair<string> { Name = "全部", Value = "All" });
			ItemColors = itemColors;
		}
		#endregion

		#region 預取商品清單
		/// <summary>
		/// 預取商品清單
		/// </summary>
		private List<F1903> _tempItemList;
		public List<F1903> TempItemList { get { return _tempItemList; } set { _tempItemList = value; RaisePropertyChanged("TempItemList"); } }

		/// <summary>
		/// 預取貨主商品清單
		/// </summary>
		private List<F1903> _tempCustItemList;
		public List<F1903> TempCustItemList { get { return _tempCustItemList; } set { _tempCustItemList = value; RaisePropertyChanged("TempCustItemList"); } }
		#endregion

		#region Search
		public ICommand SearchTempCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoTempSearch(), () => UserOperateMode == OperateMode.Query,
					o => SetSearchItemCodeFocus());
			}
		}

		private void DoTempSearch()
		{
			//執行查詢動
			var proxy = GetProxy<F19Entities>();
			var f1903Result = proxy.F1903s
				.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == CustCode)
				.Where(x => x.SND_TYPE == "0");
			if (!string.IsNullOrWhiteSpace(SearchItemCode))
			{
				f1903Result = f1903Result.Where(x => x.ITEM_CODE.StartsWith(SearchItemCode));
			}

			TempCustItemList = f1903Result.Take(ItemSearchMaximum).ToList();

			var f1903sResult = proxy.F1903s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode);
			if (!string.IsNullOrWhiteSpace(SearchItemCode))
			{
				f1903sResult = f1903sResult.Where(x => x.ITEM_CODE.StartsWith(SearchItemCode));
			}
			TempItemList = f1903sResult.Take(ItemSearchMaximum).ToList();

			SetSearchConditions();
		}	
		#endregion Search

		#region 商品清單
		/// <summary>
		/// 商品清單
		/// </summary>
		private List<F1903> _itemList;
		public List<F1903> ItemList { get { return _itemList; } set { _itemList = value; RaisePropertyChanged("ItemList"); } }
		private F1903 _selectItem;
		public F1903 SelectItem
		{
			get { return _selectItem; }
			set
			{
				_selectItem = value;
				RaisePropertyChanged("SelectItem");
			}
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					CanExecuteSearch
					);
			}
		}

		bool CanExecuteSearch()
		{
			if (UserOperateMode != OperateMode.Query)
				return false;

			if (IsItemCodeChecked == true)
			{
				return _searchItemCode.Length >= CanSearchItemCodeLowestLength;
			}
			else if (IsItemNameChecked == true)
			{
				return !string.IsNullOrWhiteSpace(SearchItemName);
			}
			else if (IsSerialNoChecked == true)
			{
				return !string.IsNullOrWhiteSpace(SearchSerialNo);
			}
			else if (IsEanCodeChecked == true)
			{
				return !string.IsNullOrWhiteSpace(SearchEanCode);
			}
			return false;
		}

		private void DoSearch()
		{
			//執行查詢動
			if (IsItemCodeChecked == true)
			{
				DoSearchItemCode();
			}
			else if (IsItemNameChecked == true)
			{
				DoSearchItemName();
			}
			else if (IsSerialNoChecked == true)
			{
				DoSearchSerialNo();
			}
			else if (IsEanCodeChecked == true)
			{
				DoSearchEanCode();
			}


			if (ItemList == null || !ItemList.Any())
				ShowMessage(Messages.InfoNoData);
		}

		private void DoSearchItemCode()
		{
			var proxy = GetProxy<F19Entities>();

			IQueryable<F1903> q;
			if (IsFromCahceSearchItemCode)
				q = TempCustItemList.AsQueryable();
			else
				q = proxy.F1903s;

			q = q.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == CustCode && x.SND_TYPE == "0");

			if (!string.IsNullOrWhiteSpace(SearchItemCode))
			{
				q = q.Where(x => x.ITEM_CODE.StartsWith(SearchItemCode));
			}
			if (!string.IsNullOrEmpty(SearchItemSize) && SearchItemSize != "All")
			{
				q = q.Where(x => x.ITEM_SIZE == SearchItemSize);
			}
			if (!string.IsNullOrEmpty(SearchItemSpec) && SearchItemSpec != "All")
			{
				q = q.Where(x => x.ITEM_SPEC == SearchItemSpec);
			}
			if (!string.IsNullOrEmpty(SearchItemColor) && SearchItemColor != "All")
			{
				q = q.Where(x => x.ITEM_COLOR == SearchItemColor);
			}

			if (!IsFromCahceSearchItemCode)
			{
				q = q.Take(ItemSearchMaximum);
			}
			var gupItems = q.ToList();
			ItemList = gupItems;
		}

		private void DoSearchItemName()
		{
			var proxy = GetProxy<F19Entities>();
			ItemList = proxy.CreateQuery<F1903>("GetF1903sByItemName")
							.AddQueryExOption("gupCode", GupCode)
							.AddQueryExOption("custCode", CustCode)
							.AddQueryExOption("itemName", SearchItemName)
							.AddQueryExOption("itemSearchMaximum", ItemSearchMaximum)
							.ToList();
		}

		private void DoSearchSerialNo()
		{
			var proxy = GetProxy<F19Entities>();
			ItemList = proxy.CreateQuery<F1903>("GetF1903sBySerialNo")
							.AddQueryExOption("gupCode", GupCode)
							.AddQueryExOption("custCode", CustCode)
							.AddQueryExOption("serialNo", SearchSerialNo)
							.ToList();
		}

		public void DoSearchEanCode()
		{
			var proxy = GetProxy<F19Entities>();
			ItemList = proxy.F1903s.Where(x => x.SND_TYPE == "0" &&
			x.GUP_CODE == GupCode &&
			x.CUST_CODE == CustCode &&
			(x.EAN_CODE1 == SearchEanCode || x.EAN_CODE2 == SearchEanCode || x.EAN_CODE3 == SearchEanCode)).ToList();
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

		#region ConfirmCommand
		public ICommand ConfirmCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoConfirm(), () => SelectItem != null,
					o => DoExit()
					);
			}
		}

		private void DoConfirm()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save
	}
}
