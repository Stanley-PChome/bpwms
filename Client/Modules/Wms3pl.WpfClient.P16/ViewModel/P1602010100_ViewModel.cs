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
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;


namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1602010100_ViewModel : InputViewModelBase
	{
		private MessagesStruct _messagesStruct;
		public bool IsEditDetail { set; get; }
		private List<NameValuePair<string>> _warehouses;

		public List<NameValuePair<string>> Warehouses
		{
			get { return _warehouses; }
			set
			{
				_warehouses = value;
				RaisePropertyChanged("Warehouses");
			}
		}

		private F160201 _addNewF160201;

		public F160201 AddNewF160201
		{
			get { return _addNewF160201; }
			set
			{
				_addNewF160201 = value;

				foreach (var dcCode in Wms3plSession.Get<GlobalInfo>().DcCodeList)
				{
					if (dcCode.Value == _addNewF160201.DC_CODE)
					{
						DcName = dcCode.Name;
						break;
					}
				}

				var proxyF19 = GetProxy<F19Entities>();


				var proxyP19Ex = GetExProxy<P19ExDataSource>();
				var query = proxyP19Ex.CreateQuery<F1912WareHouseData>("GetCustWarehouseDatas")
											.AddQueryExOption("dcCode", _addNewF160201.DC_CODE)
											.AddQueryExOption("gupCode", _addNewF160201.GUP_CODE)
											.AddQueryExOption("custCode", _addNewF160201.CUST_CODE);
				

				var listWareHousesR = query.Where(item => item.DC_CODE == _addNewF160201.DC_CODE &&
											item.WAREHOUSE_TYPE == "R")
										.OrderBy(item => item.WAREHOUSE_ID)
										.ToList();

				var listWareHouses = query.Where(item => item.DC_CODE == _addNewF160201.DC_CODE &&
											item.WAREHOUSE_TYPE != "R")
										.OrderBy(item => item.WAREHOUSE_ID)
										.ToList();

				//排序 R 在最上面
				listWareHousesR.AddRange(listWareHouses);

				var queryReturnReasons = from item in listWareHousesR
										 select new NameValuePair<string>
										 {
											 Name = item.WAREHOUSE_NAME,
											 Value = item.WAREHOUSE_ID
										 };

				Warehouses = queryReturnReasons.ToList();

				if (Warehouses.Count > 0)
				{
					SearchWarehouseId = Warehouses[0].Value;
				}

				RaisePropertyChanged("AddNewF160201");
			}
		}

		private List<SelectionItem<F160201ReturnDetail>> _returnDetailList;

		public List<SelectionItem<F160201ReturnDetail>> ReturnDetailList
		{
			get { return _returnDetailList; }
			set
			{
				_returnDetailList = value;
				RaisePropertyChanged("ReturnDetailList");
			}
		}

		//廠退主畫面上已經加入的廠退明細，用來檢查相同的商品(倉別、儲位、品號)不能再重複加入
		public SelectionList<F160201ReturnDetail> ExistF160201DetailList { set; get; }

		//修改明細的標的
		public F160201ReturnDetail EditTarget { set; get; }


		private string _dcName = String.Empty;

		public string DcName
		{
			get { return _dcName; }
			set
			{
				_dcName = value;
				RaisePropertyChanged("DcName");
			}
		}

		private string _searchWarehouseId = String.Empty;

		public string SearchWarehouseId
		{
			get { return _searchWarehouseId; }
			set
			{
				_searchWarehouseId = value;
				RaisePropertyChanged("SearchWarehouseId");
			}
		}

		private DateTime? _enterDateBegin;

		public DateTime? EnterDateBegin
		{
			get { return _enterDateBegin; }
			set
			{
				_enterDateBegin = value;
				RaisePropertyChanged("EnterDateBegin");
			}
		}

		private DateTime? _enterDateEnd;

		public DateTime? EnterDateEnd
		{
			get { return _enterDateEnd; }
			set
			{
				_enterDateEnd = value;
				RaisePropertyChanged("EnterDateEnd");
			}
		}

		private DateTime? _validDateBegin;

		public DateTime? ValidDateBegin
		{
			get { return _validDateBegin; }
			set
			{
				_validDateBegin = value;
				RaisePropertyChanged("ValidDateBegin");
			}
		}

		private DateTime? _validDateEnd;

		public DateTime? ValidDateEnd
		{
			get { return _validDateEnd; }
			set
			{
				_validDateEnd = value;
				RaisePropertyChanged("ValidDateEnd");
			}
		}

		private string _locBegin = String.Empty;

		public string LocBegin
		{
			get { return _locBegin; }
			set
			{
				_locBegin = value;
				RaisePropertyChanged("LocBegin");
			}
		}

		private string _locEnd = String.Empty;

		public string LocEnd
		{
			get { return _locEnd; }
			set
			{
				_locEnd = value;
				RaisePropertyChanged("LocEnd");
			}
		}

		private string _itemCode = String.Empty;

		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				_itemCode = value;
				RaisePropertyChanged("ItemCode");
			}
		}

		private string _itemName = String.Empty;

		public string ItemName
		{
			get { return _itemName; }
			set
			{
				_itemName = value;
				RaisePropertyChanged("ItemName");
			}
		}

		private string _vendorName = String.Empty;

		public string VendorName
		{
			get { return _vendorName; }
			set
			{
				_vendorName = value;
				RaisePropertyChanged("VendorName");
			}
		}

		private bool _isEditSelectedAll = false;
		public bool IsEditSelectedAll
		{
			get { return _isEditSelectedAll; }
			set { _isEditSelectedAll = value; RaisePropertyChanged("IsEditSelectedAll"); }
		}

		public ICommand CheckEditAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckEditAllItem()
				);
			}
		}

		public void DoCheckEditAllItem()
		{
			if (ReturnDetailList != null)
			{
				foreach (var p in ReturnDetailList)
				{
					p.IsSelected = IsEditSelectedAll;
				}
			}
		}

		private string _returnTotalCount = String.Empty;

		public string ReturnTotalCount
		{
			get { return _returnTotalCount; }
			set
			{
				_returnTotalCount = value;
				RaisePropertyChanged("ReturnTotalCount");
			}
		}

		private List<F160201ReturnDetail> _returnData;
		public List<F160201ReturnDetail> ReturnData
		{
			get { return _returnData; }
			set
			{
				_returnData = value;
				RaisePropertyChanged("ReturnData");
			}
		}

		public P1602010100_ViewModel()
		{
			UserOperateMode = OperateMode.Query;
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				//, () => UserOperateMode == OperateMode.Query
				return CreateBusyAsyncCommand(
					o => DoSearch()
					);
			}
		}

		private void DoSearch()
		{
			//檢查查詢條件
			//if (!EnterDateBegin.HasValue || !EnterDateEnd.HasValue)
			//{
			//	_messagesStruct.Message = Properties.Resources.P1602010100_InStockDate_Required;
			//	ShowMessage(_messagesStruct);
			//	return;
			//}

			//if (!ValidDateBegin.HasValue || !ValidDateEnd.HasValue)
			//{
			//	_messagesStruct.Message = Properties.Resources.P1602010100_ValidateDate_Required;
			//	ShowMessage(_messagesStruct);
			//	return;
			//}
			LocBegin = LocBegin.Trim();
			LocEnd = LocEnd.Trim();
			ItemCode = ItemCode.Trim();
			ItemName = ItemName.Trim();

			ValidateHelper.AutoChangeBeginEnd(this, (x) => x.EnterDateBegin, (x) => x.EnterDateEnd);
			ValidateHelper.AutoChangeBeginEnd(this, (x) => x.ValidDateBegin, (x) => x.ValidDateEnd);
			ValidateHelper.AutoChangeBeginEndForLoc(this, x => x.LocBegin, x => x.LocEnd);

			if (string.IsNullOrEmpty(LocBegin) && string.IsNullOrEmpty(LocEnd)
				&& string.IsNullOrEmpty(ItemCode) && string.IsNullOrEmpty(ItemName))
			{
				ShowWarningMessage(Properties.Resources.P1602010100_LocCode_ItemCode_ItemName_RequiredOne);
				return;
			}

			var proxyEx = GetExProxy<P16ExDataSource>();

			/*
			 GetF160201ReturnDetails(string dcCode, string gupCode, string custCode,
			string vendorCode, string warehouseId, string enterDateBegin, string enterDateEnd, string validDateBegin, string validDateEnd,
			string locBegin, string locEnd, string itemCode, string itemName)
			 */
			var qry = proxyEx.CreateQuery<F160201ReturnDetail>("GetF160201ReturnDetails")
										.AddQueryExOption("dcCode", AddNewF160201.DC_CODE)
										.AddQueryExOption("gupCode", AddNewF160201.GUP_CODE)
										.AddQueryExOption("custCode", AddNewF160201.CUST_CODE)
										.AddQueryExOption("warehouseId", SearchWarehouseId)
										.AddQueryExOption("enterDateBegin", EnterDateBegin)
										.AddQueryExOption("enterDateEnd", EnterDateEnd)
										.AddQueryExOption("validDateBegin", ValidDateBegin)
										.AddQueryExOption("validDateEnd", ValidDateEnd)
										.AddQueryExOption("locBegin", LocBegin)
										.AddQueryExOption("locEnd", LocEnd)
										.AddQueryExOption("itemCode", ItemCode)
										.AddQueryExOption("itemName", ItemName)
										.AsQueryable();

			if (qry != null && qry.Count() > 0)
			{
				ReturnDetailList = qry.ToSelectionList().ToList();
			}
			else
			{
				_messagesStruct.Message = Properties.Resources.P1602010000_NoData;
				ShowMessage(_messagesStruct);
				ReturnDetailList = null;
			}
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel()
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			UserOperateMode = OperateMode.Query;

		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => ReturnDetailList != null && GetAddItemGroup().Count != 0
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			//UserOperateMode = OperateMode.Query;

		}
		#endregion

		public List<F160201ReturnDetail> GetAddItemGroup()
		{
			var sel = ReturnDetailList.Where(x => x.IsSelected == true).ToList();
			var result = (from i in sel
						  select i.Item).ToList();
			return result;
		}

		public void SaveRuning()
		{
			var addDetailList = GetAddItemGroup();
			ReturnData = new List<F160201ReturnDetail>();
			bool isExist = false;

			foreach (var addItem in addDetailList)
			{
				isExist = false;

				foreach (var existItem in ExistF160201DetailList)
				{
					if (IsEditDetail)
					{
						if (EditTarget.WAREHOUSE_ID == addItem.WAREHOUSE_ID &&
							EditTarget.LOC_CODE == addItem.LOC_CODE &&
							EditTarget.ITEM_CODE == addItem.ITEM_CODE)
						{
							continue;
						}
					}

					if (existItem.Item.WAREHOUSE_ID == addItem.WAREHOUSE_ID &&
						existItem.Item.LOC_CODE == addItem.LOC_CODE &&
						existItem.Item.ITEM_CODE == addItem.ITEM_CODE)
					{
						isExist = true;
						break;
					}
				}

				if (isExist)
				{
					_messagesStruct.Message = Properties.Resources.P1602010100_WarehouseID + addItem.WAREHOUSE_NAME +
											  Properties.Resources.P1602010100_LocCode + addItem.LOC_CODE +
											  Properties.Resources.P1602010100_ItemCode + addItem.ITEM_CODE +
											  Properties.Resources.P1602010100_Exist;

					ShowMessage(_messagesStruct);
					continue;
				}

				F160201ReturnDetail data = new F160201ReturnDetail();
				data.ENTER_DATE = addItem.ENTER_DATE;
				data.RTN_VNR_QTY = data.INVENTORY_QTY = addItem.INVENTORY_QTY;
				data.ITEM_CODE = addItem.ITEM_CODE;
				data.ITEM_COLOR = addItem.ITEM_COLOR;
				data.ITEM_NAME = addItem.ITEM_NAME;
				data.ITEM_SIZE = addItem.ITEM_SIZE;
				data.ITEM_SPEC = addItem.ITEM_SPEC;
				data.LOC_CODE = addItem.LOC_CODE;
				data.MEMO = addItem.MEMO;
				data.ROWNUM = addItem.ROWNUM;
				data.VALID_DATE = addItem.VALID_DATE;
				data.WAREHOUSE_ID = addItem.WAREHOUSE_ID;
				data.WAREHOUSE_NAME = addItem.WAREHOUSE_NAME;
				ReturnData.Add(data);
			}
		}
	}
}
