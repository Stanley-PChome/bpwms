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
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1604010100_ViewModel : InputViewModelBase
	{
		public Action DoClose = delegate { };
		public P1604010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region Property,Field
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string DcCode { get; set; }

		#region 倉別代碼
		private string _warehouseId;
		public string WarehouseId
		{
			get { return _warehouseId; }
			set
			{
				if (_warehouseId == value)
					return;
				Set(() => WarehouseId, ref _warehouseId, value);
			}
		}
		#endregion

		#region 倉別清單
		private List<NameValuePair<string>> _warehouseList;

		public List<NameValuePair<string>> WarehouseList
		{
			get { return _warehouseList; }
			set
			{
				if (_warehouseList == value)
					return;
				if (value != null && value.Any())
					WarehouseId = value.FirstOrDefault().Value;
				Set(() => WarehouseList, ref _warehouseList, value);
			}
		}
		#endregion

		#region 商品代碼
		private string _itemCode;

		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				if (_itemCode == value)
					return;
				Set(() => ItemCode, ref _itemCode, value);
			}
		}
		#endregion

		#region 商品名稱
		private string _itemName;

		public string ItemName
		{
			get { return _itemName; }
			set
			{
				if (_itemName == value)
					return;
				Set(() => ItemName, ref _itemName, value);
			}
		}
		#endregion

		#region 有效日期-起
		private DateTime _validDateStart = DateTime.Now;

		public DateTime ValidDateStart
		{
			get { return _validDateStart; }
			set
			{
				if (_validDateStart == value)
					return;
				Set(() => ValidDateStart, ref _validDateStart, value);
			}
		}
		#endregion

		#region 有效日期-迄
		private DateTime _validDateEnd = Convert.ToDateTime("9999/12/31");

		public DateTime ValidDateEnd
		{
			get { return _validDateEnd; }
			set
			{
				if (_validDateEnd == value)
					return;
				Set(() => ValidDateEnd, ref _validDateEnd, value);
			}
		}
		#endregion

		#region 報廢商品儲位清單
		private SelectionList<F160402AddData> _scrapAddDetailList;

		public SelectionList<F160402AddData> ScrapAddDetailList
		{
			get { return _scrapAddDetailList; }
			set
			{
				if (_scrapAddDetailList == value)
					return;
				Set(() => ScrapAddDetailList, ref _scrapAddDetailList, value);
			}
		}
		#endregion

		#region 選擇的報廢商品儲位
		private SelectionItem<F160402AddData> _selectedScrapAddDetail;

		public SelectionItem<F160402AddData> SelectedScrapAddDetail
		{
			get { return _selectedScrapAddDetail; }
			set
			{
				if (_selectedScrapAddDetail == value)
					return;
				Set(() => SelectedScrapAddDetail, ref _selectedScrapAddDetail, value);
			}
		}
		#endregion

		#region 報廢原因清單
		private List<NameValuePair<string>> _scrapResonList;

		public List<NameValuePair<string>> ScrapResonList
		{
			get { return _scrapResonList; }
			set
			{
				if (_scrapResonList == value)
					return;
				Set(() => ScrapResonList, ref _scrapResonList, value);
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
				CheckSelectedAll(_isCheckAll, ScrapAddDetailList);
				RaisePropertyChanged();
			}
		}
		#endregion
		#endregion

		#region Func
		#region Grid Checkbox 全選 -Add
		public void CheckSelectedAll(bool isChecked, SelectionList<F160402AddData> dgData)
		{
			if (dgData != null)
			{
				foreach (var dgDataItem in dgData)
					dgDataItem.IsSelected = isChecked;
			}
		}

		#endregion
		#endregion

		#region command

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearch(), () => UserOperateMode == OperateMode.Query
						);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
			if (string.IsNullOrEmpty(WarehouseId) ||
					(string.IsNullOrEmpty(ItemCode) && string.IsNullOrEmpty(ItemName)))
			{
				var errMsg = string.Empty;
				if (string.IsNullOrEmpty(WarehouseId))
					errMsg = Properties.Resources.P1604010100_ChooseWarehouseName;
				else if ((string.IsNullOrEmpty(ItemCode) && string.IsNullOrEmpty(ItemName)))
					errMsg = Properties.Resources.P1604010100_ItemCode_ItemNameEmpty;
				ShowMessage(new MessagesStruct() { Message = errMsg, Title = Resources.Resources.Information });
				return;
			}
			ScrapAddDetailList = null;
			ScrapAddDetailList = GetF160402AddDatas(ItemCode, ItemName, WarehouseId,
				ValidDateStart.ToString("yyyy/MM/dd"), ValidDateEnd.ToString("yyyy/MM/dd"));
			if (ScrapAddDetailList == null || !ScrapAddDetailList.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
			SelectedScrapAddDetail = ScrapAddDetailList.FirstOrDefault();
		}

		private SelectionList<F160402AddData> GetF160402AddDatas(string itemCode, string itemName, string wareHouseId, string validDateStart, string validDateEnd)
		{
			var proxyEx = GetExProxy<P16ExDataSource>();

			var results = proxyEx.CreateQuery<F160402AddData>("GetF160402AddScrapDetails")
				 .AddQueryOption("dcCode", string.Format("'{0}'", DcCode))
				 .AddQueryOption("gupCode", string.Format("'{0}'", GupCode))
				 .AddQueryOption("custCode", string.Format("'{0}'", CustCode))
				 .AddQueryOption("itemCode", string.Format("'{0}'", itemCode))
				 .AddQueryOption("wareHouseId", string.Format("'{0}'", wareHouseId))
				 .AddQueryOption("itemName", string.Format("'{0}'", itemName))
				 .AddQueryOption("validDateStart", string.Format("'{0}'", validDateStart))
				 .AddQueryOption("validDateEnd", string.Format("'{0}'", validDateEnd))
				 .ToSelectionList();

			return results;
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
				bool isSaved = false;
				return CreateBusyAsyncCommand(
						o => isSaved = DoSave(), () => UserOperateMode != OperateMode.Query ||
							(ScrapAddDetailList != null && ScrapAddDetailList.Any(x => x.IsSelected)),
						o => DoSaveComplete(isSaved)
						);
			}
		}

		private void DoSaveComplete(bool isSaved)
		{
			if (isSaved)
				DoClose();
		}

		private bool DoSave()
		{
			//執行確認儲存動作

			//驗證
			return isVaild();
		}

		private bool isVaild()
		{
			if (!ScrapAddDetailList.Any(x => x.IsSelected))
			{
				ShowWarningMessage(Properties.Resources.P1604010100_ChooseData);
				return false;
			}
			foreach (var Scrap in ScrapAddDetailList.Where(x => x.IsSelected))
			{
				if (Scrap.Item.SCRAP_QTY == null || Scrap.Item.SCRAP_QTY.Value <= 0)
				{
					ShowWarningMessage(Properties.Resources.P1604010100_ScrapCount_Required);
					return false;
				}
				if (string.IsNullOrEmpty(Scrap.Item.SCRAP_CAUSE))
				{
					ShowWarningMessage(Properties.Resources.P1604010100_ChooseScrapCause);
					return false;
				}
			}
			return true;
		}
		#endregion Save
		#endregion
	}
}
