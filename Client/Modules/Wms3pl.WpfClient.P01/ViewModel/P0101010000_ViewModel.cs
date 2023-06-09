using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P01ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P01WcfService;
using System.Collections.Generic;

namespace Wms3pl.WpfClient.P01.ViewModel
{
	public partial class P0101010000_ViewModel : InputViewModelBase
	{
		public Action AddAction = delegate { };
		public Action DeleteAction = delegate { };
		public Action OnFocus = delegate { };
		public Action<PrintType, DataTable> OnPrintAction = delegate { };
		public Action OnAddFocus = delegate { };
		public Action OnEditFocus = delegate { };

		public P0101010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料							
				SetDcList();
				SetStatusList();
				SetCauseList();
				SetTypeList();
				SetOrdpropList();
			}

		}

		#region Property

		#region 業主

		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}

		#endregion

		#region 貨主

		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		#endregion

		#region 物流中心

		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged();
				if (_dcList != null && _dcList.Any())
				{
					SelectedDcCode = _dcList.First().Value;
				}
			}
		}

		private string _selectedDcCode;

		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				_selectedDcCode = value;
				RaisePropertyChanged();
				ShopNoList = null;
			}
		}

		#endregion

		#region 採購日期-起

		private DateTime _sShopDate = DateTime.Today;

		public DateTime SShopeDate
		{
			get { return _sShopDate; }
			set
			{
				_sShopDate = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region 採購日期-迄

		private DateTime _eShopDate = DateTime.Today;

		public DateTime EShopDate
		{
			get { return _eShopDate; }
			set
			{
				_eShopDate = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region 採購單號

		private string _shopNo;

		public string ShopNo
		{
			get { return _shopNo; }
			set
			{
				_shopNo = value;
				RaisePropertyChanged("ShopNo");
			}
		}
		#endregion

		#region 廠商編號

		private string _vnrCode;

		public string VnrCode
		{
			get { return _vnrCode; }
			set
			{
				_vnrCode = value;
				RaisePropertyChanged("VnrCode");
			}
		}

		#endregion

		#region 廠商名稱

		private string _vnrName;

		public string VnrName
		{
			get { return _vnrName; }
			set
			{
				_vnrName = value;
				RaisePropertyChanged("VnrName");
			}
		}

		#endregion

		#region 商品編號

		private string _itemCode;

		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				_itemCode = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region 貨主單號

		private string _custOrdNo;

		public string CustOrdNo
		{
			get { return _custOrdNo; }
			set
			{
				_custOrdNo = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region 單據狀態

		private List<NameValuePair<string>> _statusList;
		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				_statusList = value;
				RaisePropertyChanged();
				if (StatusList != null && StatusList.Any())
				{
					SelectedStatus = StatusList.First().Value;
				}
			}
		}

		private string _selectedStatus;

		public string SelectedStatus
		{
			get { return _selectedStatus; }
			set
			{
				_selectedStatus = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region 採購原因

		private List<NameValuePair<string>> _causeList;
		public List<NameValuePair<string>> CauseList
		{
			get { return _causeList; }
			set
			{
				_causeList = value;
				RaisePropertyChanged();
				if (CauseList != null && CauseList.Any())
				{
					SelectedCause = CauseList.First().Value;
				}
			}
		}

		private string _selectedCause;

		public string SelectedCause
		{
			get { return _selectedCause; }
			set
			{
				_selectedCause = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region 付款方式
		private List<NameValuePair<string>> _payTypeList;
		public List<NameValuePair<string>> PayTypeList
		{
			get { return _payTypeList; }
			set
			{
				_payTypeList = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 稅別樣式
		private List<NameValuePair<string>> _taxTypeList;
		public List<NameValuePair<string>> TaxTypeList
		{
			get { return _taxTypeList; }
			set
			{
				_taxTypeList = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 發票樣式
		private List<NameValuePair<string>> _invoTypeList;
		public List<NameValuePair<string>> InvoTypeList
		{
			get { return _invoTypeList; }
			set
			{
				_invoTypeList = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 作業類別
		private List<NameValuePair<string>> _ordPropList;

		public List<NameValuePair<string>> OrdPropList
		{
			get { return _ordPropList; }
			set
			{
				_ordPropList = value;
				RaisePropertyChanged("OrdPropList");
			}
		}
		#endregion

		#region 資料列集合
		private List<F010101ShopNoList> _shopNoList;
		public List<F010101ShopNoList> ShopNoList
		{
			get { return _shopNoList; }
			set { _shopNoList = value; RaisePropertyChanged(); }
		}

		private F010101ShopNoList _selectShopNo;
		public F010101ShopNoList SelectShopNo
		{
			get { return _selectShopNo; }
			set
			{
				_selectShopNo = value;
				SearchResultIsExpanded = false;
				DoSearchDetail();
				RaisePropertyChanged();
			}
		}

		private F010101Data _shopData;
		public F010101Data ShopData
		{
			get { return _shopData; }
			set { _shopData = value; RaisePropertyChanged(); }
		}

		private ObservableCollection<F010102Data> _shopDetail;
		public ObservableCollection<F010102Data> ShopDetail
		{
			get { return _shopDetail; }
			set { _shopDetail = value; RaisePropertyChanged(); }
		}

		private F010102Data _selectDetail;

		public F010102Data SelectDetail
		{
			get { return _selectDetail; }
			set
			{
				_selectDetail = value;
				RaisePropertyChanged();
			}
		}

		private F010102Data _addDetail;
		public F010102Data AddDetail
		{
			get { return _addDetail; }
			set
			{
				_addDetail = value;
				RaisePropertyChanged();
			}
		}

		private List<F010101ReportData> _reportData;
		public List<F010101ReportData> ReportData
		{
			get { return _reportData; }
			set { _reportData = value; RaisePropertyChanged(); }
		}
		#endregion

		#endregion

		#region 下拉選單資料繫結
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}
		private void SetStatusList()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F010101", "STATUS", true);
		}
		private void SetCauseList()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1951s.Where(n => n.UCT_ID == "PO").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.CAUSE,
							Value = o.UCC_CODE
						}).ToList();
			CauseList = list;

		}
		private void SetTypeList()
		{
			PayTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1908", "PAY_TYPE");
			TaxTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1908", "TAX_TYPE");
			InvoTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1908", "INVO_TYPE");
		}

		public void SetOrdpropList()
		{
			var proxy = GetProxy<F00Entities>();
			var query = from item in proxy.F000903s
						where item.ORD_PROP.StartsWith("A")
						select new NameValuePair<string>
						{
							Name = item.ORD_PROP_NAME,
							Value = item.ORD_PROP
						};
			OrdPropList = query.ToList();
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						if (UserOperateMode == OperateMode.Add)
						{
							SelectShopNo = ShopNoList.LastOrDefault();
						}
						else
						{
							SelectShopNo = ShopNoList.FirstOrDefault();
						}

						UserOperateMode = OperateMode.Query;
					}
					);
			}
		}

		private void DoSearch(F010101Data shopNo = null)
		{
			//執行查詢動作
			var msg = new MessagesStruct()
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Message = Properties.Resources.P010101_StartDateError,
				Title = Properties.Resources.P010101_Message
			};

			var proxy = GetExProxy<P01ExDataSource>();

			if (shopNo != null)
			{
				SelectedDcCode = shopNo.DC_CODE;
				SShopeDate = shopNo.SHOP_DATE;
				EShopDate = shopNo.SHOP_DATE;
				ShopNo = shopNo.SHOP_NO;
				VnrCode = shopNo.VNR_CODE;
				VnrName = string.Empty;
				ItemCode = string.Empty;
				CustOrdNo = string.Empty;
				SelectedStatus = string.Empty;
			}
			else
			{
				if (SShopeDate > EShopDate)
				{
					ShowMessage(msg);
					return;
				}
			}

			ShopNoList = proxy.CreateQuery<F010101ShopNoList>("GetF010101ShopNoList")
					.AddQueryExOption("dcCode", SelectedDcCode)
					.AddQueryExOption("gupCode", GupCode)
					.AddQueryExOption("custCode", CustCode)
					.AddQueryExOption("shopDateS", _sShopDate.ToString("yyyy/MM/dd"))
					.AddQueryExOption("shopDateE", _eShopDate.ToString("yyyy/MM/dd"))
					.AddQueryExOption("shopNo", _shopNo)
					.AddQueryExOption("vnrCode", _vnrCode)
					.AddQueryExOption("vnrName", _vnrName)
					.AddQueryExOption("itemCode", _itemCode)
					.AddQueryExOption("custOrdNo", _custOrdNo)
					.AddQueryExOption("status", _selectedStatus)
					.ToList();

			if (!ShopNoList.Any())
			{
				ShowMessage(Messages.InfoNoData);
			}
			else
			{
				SearchConditionIsExpanded = false;
				SearchResultIsExpanded = true;
			}
		}

		private void DoSearchDetail()
		{
			if (_selectShopNo == null)
			{
				ShopData = null;
				ShopDetail = null;
				return;
			}

			var proxy = GetExProxy<P01ExDataSource>();
      ShopData = proxy.CreateQuery<F010101Data>("GetF010101Datas")
      .AddQueryExOption("dcCode", SelectedDcCode)
      .AddQueryExOption("gupCode", GupCode)
      .AddQueryExOption("custCode", CustCode)
      .AddQueryExOption("shopNo", SelectShopNo.SHOP_NO)
      .FirstOrDefault();

			if (ShopData == null)
			{
				ShowMessage(Messages.InfoNoData);
			}
			else
			{
				ShopDetail = proxy.CreateQuery<F010102Data>("GetF010102Datas")
          .AddQueryExOption("dcCode", SelectedDcCode)
          .AddQueryExOption("gupCode", GupCode)
          .AddQueryExOption("custCode", CustCode)
          .AddQueryExOption("shopNo", SelectShopNo.SHOP_NO)
          .ToObservableCollection();
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
			UserOperateMode = OperateMode.Add;
			AddDetail = new F010102Data();
			ShopDetail = new ObservableCollection<F010102Data>();
			//執行新增動作
			ShopData = new F010101Data()
			{
				DC_CODE = SelectedDcCode,
				GUP_CODE = GupCode,
				CUST_CODE = CustCode,
				SHOP_DATE = DateTime.Today,
				DELIVER_DATE = DateTime.Today,
				INVOICE_DATE = DateTime.Today
			};

			OnAddFocus();
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && ShopData != null && ShopData.STATUS == "0"
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			AddDetail = new F010102Data();
			//執行編輯動作		

			OnEditFocus();
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
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				//執行取消動作
				UserOperateMode = OperateMode.Query;
				ShopData = null;
				ShopDetail = null;
			}
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectShopNo != null && SelectShopNo.STATUS == "0",
					c =>
					{
						UserOperateMode = OperateMode.Query;
						DoSearch();
					}
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			var proxy = GetModifyQueryProxy<F01Entities>();
			var f010101 = proxy.F010101s.Where(x => x.CUST_CODE == CustCode
															 && x.GUP_CODE == GupCode
															 && x.DC_CODE == _selectedDcCode
															 && x.SHOP_NO == _selectShopNo.SHOP_NO
															 && x.STATUS == "0").ToList();
			if (!f010101.Any())
			{
				ShowMessage(Messages.WarningBeenDeleted);
			}
			else
			{
				foreach (var data in f010101)
				{
					data.STATUS = "9";
					data.UPD_DATE = DateTime.Now;
					data.UPD_STAFF = Wms3plSession.CurrentUserInfo.Account;
					data.UPD_NAME = Wms3plSession.CurrentUserInfo.AccountName;
					proxy.UpdateObject(data);
				}

				proxy.SaveChanges();
				ShowMessage(Messages.InfoDeleteSuccess);
			}
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
					o => isSaved = DoSave(), () => UserOperateMode != OperateMode.Query && ShopDetail != null && ShopDetail.Count > 0,
					c => DoSaveComplate(isSaved), e => DoSaveComplate(isSaved)
					);
			}
		}

		private bool DoSave(bool isApproved = false)
		{
			//執行確認儲存動作

			//提示訊息
			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
			{
				return false;
			}

			//檢查欄位
			if (string.IsNullOrEmpty(ShopData.ORD_PROP))
			{
				DialogService.ShowMessage(Properties.Resources.P010101_SelectOrdProp);
				return false;
			}
			//儲存
			var proxy = new wcf.P01WcfServiceClient();

			var f010101 = ExDataMapper.Map<F010101Data, wcf.F010101Data>(ShopData);
			var f010102S = ExDataMapper.MapCollection<F010102Data, wcf.F010102Data>(ShopDetail).ToArray();

			var result = new wcf.ExecuteResult() { Message = Properties.Resources.P010101_SaveError };
			if (UserOperateMode == OperateMode.Add)
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
														() => proxy.InsertP010101(f010101, f010102S));
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
					() => proxy.UpdateP010101(f010101, f010102S, isApproved));
			}

			ShowResultMessage(result);
			return result.IsSuccessed;
		}

		private void DoSaveComplate(bool isSaved)
		{
			//UserOperateMode = OperateMode.Query;
			//DoSearch(ShopData);
			if (isSaved)
			{
				SearchCommand.Execute(null);
			}

		}

		#endregion Save

		#region Approved
		public ICommand ApprovedCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
					o => isSaved = DoSave(true), () => UserOperateMode == OperateMode.Edit && ShopDetail != null && ShopDetail.Count > 0 && ShopData.STATUS == "0",
					c => DoSaveComplate(isSaved), e => DoSaveComplate(isSaved)
					);
			}
		}

		#endregion

		#region SearchItem
		private bool _hasSearchItemData;
		public bool HasSearchItemData
		{
			get { return _hasSearchItemData; }
			set
			{
				_hasSearchItemData = value;
				RaisePropertyChanged();
			}
		}
		#endregion SearchItem

		#region AddDetail

		public ICommand AddDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddDetail(),
					() =>
						((AddDetail != null && HasSearchItemData && AddDetail.SHOP_QTY > 0 &&
							!string.IsNullOrWhiteSpace(AddDetail.ITEM_CODE)))
					, o => DoAddComplate()
					);
			}
		}

		private void DoAddDetail()
		{

		}
		private void DoAddComplate()
		{
			if (AddDetail != null && !string.IsNullOrWhiteSpace(AddDetail.ITEM_CODE))
			{
				//執行新增動作
				if (ShopDetail.Where(o => o.ITEM_CODE == AddDetail.ITEM_CODE).Any())
				{
					DialogService.ShowMessage(Properties.Resources.P010101_ItemCodeRepeat);
				}
				else
				{
					ShopDetail.Add(AddDetail);
					AddAction();
				}
				
				AddDetail = new F010102Data();
		

			}
		}

		#endregion AddDetail

		#region DeleteDetail
		public ICommand DeleteDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDeleteDetail(), () => UserOperateMode != OperateMode.Query && SelectDetail != null
															, o => DelDetailComplate()
					);
			}
		}

		private void DoDeleteDetail()
		{

		}

		private void DelDetailComplate()
		{
			if (SelectDetail != null)
			{
				//執行刪除動作
				ShopDetail.Remove(SelectDetail);
				DeleteAction();
			}
		}

		#endregion DeleteDetail

		/// <summary>
		/// 取得廠商資訊
		/// </summary>
		public void CheckVnrCode()
		{
			ShopData.VNR_NAME = string.Empty;
			ShopData.BUSPER = string.Empty;
			ShopData.TEL = string.Empty;
			ShopData.ADDRESS = string.Empty;
			ShopData.INV_ADDRESS = string.Empty;
			ShopData.INVO_TYPE = string.Empty;
			ShopData.PAY_TYPE = string.Empty;
			ShopData.TAX_TYPE = string.Empty;
			if (string.IsNullOrWhiteSpace(ShopData.VNR_CODE))
			{
				RaisePropertyChanged("ShopData");
				return;
			}

			var proxy = GetProxy<F19Entities>();
			var data =
				proxy.F1908s.Where(n => n.GUP_CODE == GupCode && n.VNR_CODE == ShopData.VNR_CODE && n.STATUS != "9")
					.ToList();
			var f1909 = proxy.F1909s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode).FirstOrDefault();
			if (f1909.ALLOWGUP_VNRSHARE == "0")
				data = data.Where(o=>o.CUST_CODE == CustCode).ToList();
			if (data.Any())
			{
				ShopData.VNR_NAME = data.First().VNR_NAME;
				//ShopData.BUSPER = data.First().BUSPER;
				ShopData.TEL = data.First().TEL;
				ShopData.ADDRESS = data.First().ADDRESS;
				ShopData.INV_ADDRESS = data.First().INV_ADDRESS;
				//ShopData.INVO_TYPE = data.First().INVO_TYPE;
				ShopData.PAY_TYPE = data.First().PAY_TYPE;
				ShopData.TAX_TYPE = data.First().TAX_TYPE;
			}
			else
			{
				DialogService.ShowMessage("查無廠商資料");
				ShopData.VNR_CODE = string.Empty;
			}
			RaisePropertyChanged("ShopData");
		}

		#region Preview
		public ICommand PreviewCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPreview(),
					() => UserOperateMode == OperateMode.Query && SelectShopNo != null && SelectShopNo.STATUS == "1",
					o => OnPrintAction(PrintType.Preview, ReportData.ToDataTable()));
			}
		}

		private void DoPreview()
		{
			//執行確認儲存動作
			LoadReportData();
		}
		#endregion

		#region PrintCommand
		public ICommand PrintCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPrint(),
					() => UserOperateMode == OperateMode.Query && SelectShopNo != null && SelectShopNo.STATUS == "1",
					o => OnPrintAction(PrintType.ToPrinter, ReportData.ToDataTable()));
			}
		}

		private void DoPrint()
		{
			LoadReportData();
		}

		public void LoadReportData()
		{
			var proxy = GetExProxy<P01ExDataSource>();
			ReportData = proxy.CreateQuery<F010101ReportData>("GetF010101Reports")
							 .AddQueryExOption("dcCode", ShopData.DC_CODE)
							 .AddQueryExOption("gupCode", ShopData.GUP_CODE)
							 .AddQueryExOption("custCode", ShopData.CUST_CODE)
							 .AddQueryExOption("shopNo", ShopData.SHOP_NO).ToList();

			ReportData.ForEach(x => x.ShopNoBarcode = BarcodeConverter128.StringToBarcode(x.SHOP_NO));
		}

		#endregion

		#region UI 連動 binding
		private bool _searchResultIsExpanded = false;

		public bool SearchResultIsExpanded
		{
			get { return _searchResultIsExpanded; }
			set
			{
				_searchResultIsExpanded = value;
				RaisePropertyChanged();
			}
		}

		private bool _searchConditionIsExpanded = true;

		public bool SearchConditionIsExpanded
		{
			get { return _searchConditionIsExpanded; }
			set
			{
				Set(() => SearchConditionIsExpanded, ref _searchConditionIsExpanded, value);
			}
		}

		#endregion
	}
}
