using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;

using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using System.Data;
using System.ComponentModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P02ExDataService.ExecuteResult;
using System.Windows;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.P02.Properties;
using exshare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClients.SharedViews.Views;
using System.Windows.Media;
using System.IO;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202030000_ViewModel : InputViewModelBase
	{
		/// <summary>
		/// 產生驗收單後, 後續要做的事
		/// </summary>
		public Action AfterDoAcceptance = delegate { };
		public Action SerialAndAlloctionReport = delegate { };
		public Action OnDcCodeChanged = () => { };
		public Action OnEdit = delegate { };
		public Action OnCancel = delegate { };

		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private string _isUpLoad = "0";
		/// <summary>
		/// 前一張進倉單單號，判斷是否為第一次刷入的單號並呼叫影資API第一段
		/// </summary>
		private string _lastPurchaseNo;
		/// <summary>
		/// Device 的設定 (當物流中心改變時，就會去顯示設定 Device 的畫面)  
		/// </summary>
		public F910501 SelectedF910501 { get; set; }

		public P0202030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
				SetStatus();

				var proxy = GetProxy<F19Entities>();
				ShowUnitTrans = proxy.F1909s.Where(o => o.GUP_CODE == _gupCode && o.CUST_CODE == _custCode).FirstOrDefault().SHOW_UNIT_TRANS;
				FastPassTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "FAST_PASS_TYPE");
			}
		}

		private void InitControls()
		{
			var proxy = GetProxy<F19Entities>();
			var item = proxy.F1909s.Where(o => o.GUP_CODE == _gupCode && o.CUST_CODE == _custCode).FirstOrDefault();
			OutOfStockVisiblity = Visibility.Collapsed;
			IsEnabledEdit = false;
			if (item != null)
			{
				IsPickLocFirst = item.ISPICKLOCFIRST == "1";
				OutOfStockVisiblity = item.ISOUTOFSTOCKRECV == "1" ? Visibility.Visible : Visibility.Collapsed;
				IsEnabledEdit = item.SPILT_INCHECK == "1";
			}

			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
		}

		#region 資料連結/ 頁面參數
		private void PageRaisePropertyChanged()
		{
			RaisePropertyChanged("ItemVerifyEnabled");
			RaisePropertyChanged("ReadSerialNoEnabled");
			RaisePropertyChanged("SpecialPurchaseEnabled");
			RaisePropertyChanged("AcceptanceEnabled");
			RaisePropertyChanged("UploadFileEnabled");
			RaisePropertyChanged("PrintBoxTagEnabled");
			RaisePropertyChanged("CollectSerialNoEnabled");
			RaisePropertyChanged("IsEditMode");
			RaisePropertyChanged("SelectedCustOrdNo");
			RaisePropertyChanged("SetDefectQtyEnabled");
		}
		#region Form - 群組標題
		public string GroupHeader
		{
			get { return string.Format("{0}{1}        {2}{3}", Properties.Resources.P0202030000_PurchaseNo, SelectedPurchaseNo, Properties.Resources.P0202030000_CustOrdNo, string.IsNullOrEmpty(SelectedCustOrdNo) ? "" : SelectedCustOrdNo); }
		}
		#endregion
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				if (Set(() => SelectedDc, ref _selectedDc, value))
				{

					OnDcCodeChanged();
					SetTarWarehouse();
				}
			}
		}
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}
		#endregion

		#region Form - 進倉單號
		private string _selectedPurchaseNo;
		/// <summary>
		/// 新增資料時需輸入進倉單號
		/// </summary>
		public string SelectedPurchaseNo
		{
			get { return _selectedPurchaseNo; }
			set
			{
				Set(() => SelectedPurchaseNo, ref _selectedPurchaseNo, value);
			}
		}
		#endregion

		#region Form - 貨主單號

		public string _selectedCustOrdNo = string.Empty;
		public string SelectedCustOrdNo
		{
			get { return _selectedCustOrdNo; }
			set
			{
				Set(() => SelectedCustOrdNo, ref _selectedCustOrdNo, value);
			}
		}
		#endregion

		#region 自有商品判斷用Flag
		public bool HadViewItemCodeSelf = false;
		public string SelectedPurchaseNoItemCodeSelfTemp = string.Empty;
		#endregion

		#region 包裝參考變數
		private string _showUnitTrans;

		public string ShowUnitTrans
		{
			get { return _showUnitTrans; }
			set
			{
				Set(() => ShowUnitTrans, ref _showUnitTrans, value);
			}
		}

		#endregion

		#region Form - 驗收單號
		private string _rtNo = string.Empty;
		/// <summary>
		/// 新增資料時需輸入進倉單號
		/// </summary>
		public string RtNo
		{
			get { return _rtNo; }
			set { _rtNo = value; }
		}
		#endregion
		#region Form - 預定進貨日
		private DateTime _selectedDt = DateTime.Today;
		public DateTime SelectedDt
		{
			get { return _selectedDt; }
			set
			{
				Set(() => SelectedDt, ref _selectedDt, value);
			}
		}
		#endregion
		#region Form - 編輯狀態
		public bool IsReadOnly
		{
			get { return true; } // UserOperateMode == OperateMode.Edit; }
		}
		#endregion
		#region 採購單號
		private string _shopNo;
		public string ShopNo
		{
			get { return _shopNo; }
			set { _shopNo = value; }
		}
		#endregion


		#region Data - 查詢結果
		private ObservableCollection<P020203Data> _dgList = new ObservableCollection<P020203Data>();
		public ObservableCollection<P020203Data> DgList
		{
			get { return _dgList; }
			set
			{
				Set(() => DgList, ref _dgList, value);
				RaisePropertyChanged("GroupHeader");
			}
		}
		private P020203Data _selectedData;
		public P020203Data SelectedData
		{
			get { return _selectedData; }
			set
			{
				Set(() => SelectedData, ref _selectedData, value);
				PageRaisePropertyChanged();
			}
		}
		/// <summary>
		/// 給特殊採購使用的資料. 僅篩選出未檢驗, 或是序號未刷讀完的項目
		/// 必須回傳新集合, 否則新視窗的資料會跟原資料連動
		/// </summary>
		public List<P020203Data> DataForSpecialPurchase
		{
			get
			{
				return DgList.Where(x => x.CHECK_ITEM == "0"
					|| (x.CHECK_SERIAL == "0" && x.BUNDLE_SERIALNO == "1"))
					.Select(AutoMapper.Mapper.DynamicMap<P020203Data>).ToList();
			}
		}
		#endregion
		#region Report - 驗收單報表
		//良品
		private List<AcceptancePurchaseReport> _acceptanceReportData = new List<AcceptancePurchaseReport>();
		public List<AcceptancePurchaseReport> AcceptanceReportData
		{
			get { return _acceptanceReportData; }
			set { _acceptanceReportData = value; }
		}
		public DataTable AcceptanceReportDataTable
		{
			get
			{
				_acceptanceReportData.ForEach(x => x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE));
				_acceptanceReportData.ForEach(x => x.OrdNoBarcode = BarcodeConverter128.StringToBarcode(x.ORDER_NO));
				return _acceptanceReportData.ToDataTable("ado");
			}
		}

		//不良品
		private List<AcceptancePurchaseReport> _acceptanceReportData1 = new List<AcceptancePurchaseReport>();
		public List<AcceptancePurchaseReport> AcceptanceReportData1
		{
			get { return _acceptanceReportData1; }
			set { _acceptanceReportData1 = value; }
		}
		public DataTable AcceptanceReportDataTable1
		{
			get
			{
				_acceptanceReportData1.ForEach(x => x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE));
				_acceptanceReportData1.ForEach(x => x.OrdNoBarcode = BarcodeConverter128.StringToBarcode(x.ORDER_NO));
				return _acceptanceReportData1.ToDataTable("ado");
			}
		}
		#endregion
		#region Report - 揀貨單報表
		private List<F051201ReportDataA> _f051201ReportDataADatas = new List<F051201ReportDataA>();
		public List<F051201ReportDataA> F051201ReportDataADatas
		{
			get { return _f051201ReportDataADatas; }
			set { _f051201ReportDataADatas = value; }
		}
		public DataTable F051201ReportDataADataDataTable
		{
			get
			{
				F051201ReportDataADatas.ForEach(x =>
				{
					x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
					x.WmsOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO);
					x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
				});
				return F051201ReportDataADatas.ToDataTable("ado");
			}
		}
		#endregion
		#region Report - 商品序號報表

		private List<wcf.AcceptanceSerialData> _acceptanceSerialDatas = new List<wcf.AcceptanceSerialData>();
		public List<wcf.AcceptanceSerialData> AcceptanceSerialDatas
		{
			get { return _acceptanceSerialDatas; }
			set { _acceptanceSerialDatas = value; }
		}
		public DataTable AcceptanceSerialDataTable
		{
			get
			{
				_acceptanceSerialDatas.ForEach(x => x.BARCODE = BarcodeConverter128.StringToBarcode(x.SERIAL_NO));
				return _acceptanceSerialDatas.ToDataTable("ado");
			}
		}
		#endregion

		private List<F151001ReportByAcceptance> _f151001ReportByAcceptance;
		public List<F151001ReportByAcceptance> F151001ReportByAcceptance
		{
			get { return _f151001ReportByAcceptance; }
			set
			{
				_f151001ReportByAcceptance = value;
				RaisePropertyChanged("F151001ReportByAcceptance");
			}
		}
		public DataTable F151001ReportByAcceptanceDataTable
		{
			get
			{
				F151001ReportByAcceptance.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));
				return F151001ReportByAcceptance.ToDataTable("ado");
			}
		}

		private List<F151001ReportByAcceptance> _f151001ReportByAcceptanceDefect;
		public List<F151001ReportByAcceptance> F151001ReportByAcceptanceDefect
		{
			get { return _f151001ReportByAcceptanceDefect; }
			set
			{
				_f151001ReportByAcceptanceDefect = value;
				RaisePropertyChanged("F151001ReportByAcceptanceDefect");
			}
		}
		public DataTable F151001ReportByAcceptanceDefectDataTable
		{
			get
			{
				_f151001ReportByAcceptanceDefect.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));
				return _f151001ReportByAcceptanceDefect.ToDataTable("ado");
			}
		}

		#region 不良品明細(報表)
		private List<DefectDetailReport> _defectDetailReport;
		public List<DefectDetailReport> DefectDetailReport
		{
			get { return _defectDetailReport; }
			set
			{
				_defectDetailReport = value;
				RaisePropertyChanged("DefectDetailReport");
			}
		}
		public DataTable DefectDetailReportDataTable
		{
			get
			{
				return _defectDetailReport.ToDataTable("ado");
			}
		}
		#endregion

		#region 單據狀態List

		private List<NameValuePair<string>> _statusList;

		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				Set(() => StatusList, ref _statusList, value);
			}
		}

		#endregion

		#region 快速通關分類List

		private List<NameValuePair<string>> _fastPassTypeList;

		public List<NameValuePair<string>> FastPassTypeList
		{
			get { return _fastPassTypeList; }
			set
			{
				Set(() => FastPassTypeList, ref _fastPassTypeList, value);
			}
		}

		#endregion

		#region 單據狀態資料

		public void SetStatus()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F020201", "STATUS");
		}

		#endregion

		#region 上架倉別List
		private List<NameValuePair<string>> _warehouseList;

		public List<NameValuePair<string>> WarehouseList
		{
			get { return _warehouseList; }
			set
			{
				Set(() => WarehouseList, ref _warehouseList, value);
			}
		}
		#endregion

		#region 上架倉別

		private void SetTarWarehouse()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1980s.Where(x => x.DC_CODE == SelectedDc && x.WAREHOUSE_TYPE != "T" && x.WAREHOUSE_TYPE != "I" && x.WAREHOUSE_TYPE != "D" && x.WAREHOUSE_TYPE != "M")
				.Select(x => new NameValuePair<string>
				{
					Name = string.Format("{0} {1}", x.WAREHOUSE_ID, x.WAREHOUSE_NAME),
					Value = x.WAREHOUSE_ID
				}).ToList();

			result.Insert(0, new NameValuePair<string> { Name = "", Value = "" });
			WarehouseList = result;
		}

		#endregion

		#region 取得揀貨區優先儲位
		private bool _isPickLocFirst;

		public bool IsPickLocFirst
		{
			get { return _isPickLocFirst; }
			set
			{
				Set(() => IsPickLocFirst, ref _isPickLocFirst, value);
			}
		}
		#endregion

		#region 是否可使用缺貨確認
		private Visibility _outOfStockVisiblity;

		public Visibility OutOfStockVisiblity
		{
			get { return _outOfStockVisiblity; }
			set
			{
				Set(() => OutOfStockVisiblity, ref _outOfStockVisiblity, value);
			}
		}
		#endregion

		#region 是否可使用分批驗收
		private bool _isEnabledEdit;

		public bool IsEnabledEdit
		{
			get { return _isEnabledEdit; }
			set
			{
				Set(() => IsEnabledEdit, ref _isEnabledEdit, value);
			}
		}
		#endregion


		#region 快速通關名稱
		private string _fastPassTypeName;

		public string FastPassTypeName
		{
			get { return _fastPassTypeName; }
			set
			{
				Set(() => FastPassTypeName, ref _fastPassTypeName, value);
			}
		}
		#endregion


		#region
		private Brush _fastPassTypeColor;

		public Brush FastPassTypeColor
		{
			get { return _fastPassTypeColor; }
			set
			{
				Set(() => FastPassTypeColor, ref _fastPassTypeColor, value);
			}
		}
		#endregion

		/// <summary>
		/// 判斷是由"商品檢驗"(0)使用，還是商品檢驗與容器綁定(1)使用
		/// </summary>
		public String RT_MODE = "0";
		#endregion

		#region Function

		public bool ExistsF020301Data()
		{

			var proxy = GetExProxy<P02ExDataSource>();
			var query = proxy.CreateQuery<ExecuteResult>("ExistsF020301Data")
							.AddQueryExOption("dcCode", SelectedData.DC_CODE)
							.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
							.AddQueryExOption("custCode", SelectedData.CUST_CODE)
							.AddQueryExOption("purchaseNo", SelectedData.PURCHASE_NO)
							.AddQueryExOption("itemCode", SelectedData.ITEM_CODE);
			var result = query.ToList().FirstOrDefault();
			return result.IsSuccessed;

		}

		#endregion

		public void TarWarehouseChange(string purchaseNo, string purchaseSeq, string tarWarehouseId)
		{
			var item = DgList.FirstOrDefault(x => x.PURCHASE_NO == purchaseNo && x.PURCHASE_SEQ == purchaseSeq);
			if (item != null)
			{
				var proxy = GetProxy<F02Entities>();
				var f02020101 = proxy.F02020101s.Where(x => x.DC_CODE == item.DC_CODE && x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.PURCHASE_NO == item.PURCHASE_NO && x.PURCHASE_SEQ == item.PURCHASE_SEQ).FirstOrDefault();
				if (f02020101 != null && f02020101.TARWAREHOUSE_ID != tarWarehouseId)
				{
					f02020101.TARWAREHOUSE_ID = tarWarehouseId;
					proxy.UpdateObject(f02020101);
					proxy.SaveChanges();
				}

			}
		}

		/// <summary>
		/// 送訊號給影資系統告知驗收單開始or結束
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="stockNo">收貨單號</param>
		/// <param name="ItemNos">商品ID，驗收開始(刷入進倉單時)不需塞此項目，驗收結束(驗收完成)需要</param>
		private bool CallVideoService(String dcCode, String gupCode, String custCode, String stockNo, List<String> ItemNos = null)
		{
			//如果前一張單刷入的單號與現在這單號不同才進行呼叫影資系統，避免User操作時更新畫面就呼叫一次
			//另外判斷如果是驗收完成呼叫(ItemNos有資料)，要可以執行內容
			if (_lastPurchaseNo != SelectedPurchaseNo || ItemNos != null)
			{
				var wcfproxy = GetWcfProxy<wcf.P02WcfServiceClient>();
				var callRecvVedio = wcfproxy.RunWcfMethod(w => w.CallRecvVedio(dcCode, gupCode, custCode, stockNo, ItemNos?.ToArray()));
				if (!string.IsNullOrWhiteSpace(callRecvVedio.Message))
					ShowWarningMessage(callRecvVedio.Message);
				return callRecvVedio.IsSuccessed;
			}
			return true;
		}

		#region Command

			#region Search
		public ICommand SearchCommand
		{
			get
			{

				return CreateBusyAsyncCommand(
					o =>
					{
						bool showTotalRecvQtyMsg = o == null ? false : Convert.ToBoolean(o);
						DoSearch(showTotalRecvQtyMsg);
					},
					() => UserOperateMode == OperateMode.Query
							&& !string.IsNullOrWhiteSpace(SelectedPurchaseNo),
					o =>
					{ SetSelectedData(); }
				);
			}
		}

		public void DoSearch(bool showTotalRecvQtyMsg = false)
		{
			// 0. 無條件將資料初始化
			DgList = new ObservableCollection<P020203Data>();
			SelectedData = null;
			SelectedPurchaseNo = SelectedPurchaseNo.Trim();
			var f01Proxy = GetProxy<F01Entities>();
			var proxyP02Wcf = new wcf.P02WcfServiceClient();

			// 1.1 檢查單是否正確
			if (!CheckPurchaseNo())
				return;

			//取得進倉主檔
			var item = f01Proxy.F010201s.Where(o => o.DC_CODE == SelectedDc && o.GUP_CODE == this._gupCode && o.CUST_CODE == this._custCode
						&& o.STOCK_NO == SelectedPurchaseNo).FirstOrDefault();

			SelectedCustOrdNo = item.CUST_ORD_NO ?? "";
			SelectedDt = item.DELIVER_DATE;
			ShopNo = item.SHOP_NO;

			// 今日已驗收XXX數量
			if (showTotalRecvQtyMsg)
			{
				var todayRecvQty = RunWcfMethod<int>(proxyP02Wcf.InnerChannel, () => proxyP02Wcf.GetTodayRecvQty(SelectedDc, this._gupCode, this._custCode, SelectedPurchaseNo, DateTime.Today));
				if (todayRecvQty > 0)
				{
					ShowMessage(new MessagesStruct
					{
						Button = DialogButton.OK,
						Image = DialogImage.Information,
						Message = string.Format(Properties.Resources.TodayAcceptancedQty, todayRecvQty),
						Title = Properties.Resources.Message
					});
				}
			}

			// 快速通關分類
			var fastPassType = FastPassTypeList.FirstOrDefault(x => x.Value == item.FAST_PASS_TYPE);
			if (fastPassType != null)
			{
				FastPassTypeName = fastPassType.Name;
				if (fastPassType.Value == "3")
					FastPassTypeColor = Brushes.Red;
				else
					FastPassTypeColor = Brushes.Black;
			}
			else
			{
				FastPassTypeColor = Brushes.Black;
				FastPassTypeName = string.Empty;
			}

			// 1.2 當進倉單沒有填採購單號時，詢問是否繼續作業，是的話，採購單號自動填入進倉單號
			if (!new PurchaseService().CheckShopNo(this, SelectedDc, _gupCode, _custCode, SelectedPurchaseNo))
				return;

			// 當進倉單狀態為0(待處理)，顯示提示訊息"進倉單尚未進行進貨點收"
			if (item.STATUS == "0")
			{
				DialogService.ShowMessage(Properties.Resources.P0202030000_NotCheck);
				return;
			}

			var check = CallVideoService(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.STOCK_NO);
			if (!check)
				return;

			// 2. 如果存在進倉單號, 則產生F02020101, 成功回傳true
			var resultUpdate = RunWcfMethod<wcf.ExecuteResult>(proxyP02Wcf.InnerChannel, () => proxyP02Wcf.UpdateP020203(SelectedDc, this._gupCode, this._custCode, SelectedPurchaseNo, WarehouseList.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => x.Value).ToArray(), RT_MODE));

			// 3. 如果資料寫入正常時, 讀出
			if (resultUpdate.IsSuccessed)
			{
				RtNo = resultUpdate.No;
				var proxy = GetExProxy<P02ExDataSource>();
				var result = proxy.CreateQuery<P020203Data>("GetP020203Datas")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", this._gupCode)
					.AddQueryExOption("custCode", this._custCode)
					.AddQueryExOption("purchaseNo", SelectedPurchaseNo)
					.AddQueryExOption("rtNo", RtNo)
					.AddQueryExOption("isPickLocFirst", IsPickLocFirst)
					.ToList();

				if (!result.Any()) ShowMessage(Messages.InfoNoData);
				else
				{
					var porxy = GetProxy<F19Entities>();
					_isUpLoad = porxy.F1909s.Where(o => o.CUST_CODE == this._custCode && o.GUP_CODE == this._gupCode).FirstOrDefault().ISUPLOADFILE;
					DgList = result.ToObservableCollection();

					if (SelectedPurchaseNoItemCodeSelfTemp != SelectedPurchaseNo)
					{
						SelectedPurchaseNoItemCodeSelfTemp = SelectedPurchaseNo;
						HadViewItemCodeSelf = false;
					}
					if (!HadViewItemCodeSelf)
					{
						var isOemItems = result.Where(x => x.ISOEM == "1").Select(x => x.ITEM_CODE).Distinct().ToList();
						if (isOemItems.Any())
							DialogService.ShowMessage(string.Format(Properties.Resources.P0202030000_IsOemItemMsg, string.Join("、", isOemItems)));
						HadViewItemCodeSelf = true;
					}

					if (resultUpdate.IsSuccessed && !string.IsNullOrWhiteSpace(resultUpdate.Message))
					{
						ShowMessage(new MessagesStruct
						{
							Button = DialogButton.OK,
							Image = DialogImage.Information,
							Message = resultUpdate.Message,
							Title = Properties.Resources.Message
						});
					}
				}
			}
			else
			{
				ShowMessage(new List<ExecuteResult>() {
										new ExecuteResult() { IsSuccessed = false, Message = resultUpdate.Message}
								});
			}



			_lastPurchaseNo = SelectedPurchaseNo;
		}
		#endregion

		private void SetSelectedData()
		{
			if (DgList != null && DgList.Any())
				SelectedData = DgList.First();
		}

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				bool result = false;
				return CreateBusyAsyncCommand(
					o => result = DoEdit(),
					() =>
					{
						// 鎖定已驗收/已上傳就不能編輯, 否則會出現錯誤 (F02020101沒資料)
						return UserOperateMode == OperateMode.Query && SelectedData != null && (SelectedData.STATUS == "0") && IsEnabledEdit;
					},
					o =>
					{
						if (result)
						{
							UserOperateMode = OperateMode.Edit;
							PageRaisePropertyChanged();
							OnEdit();
						}
					}
				);
			}
		}
		public bool DoEdit()
		{

			var proxy = GetProxy<F01Entities>();
			var f010201Data = proxy.F010201s.Where(x => x.DC_CODE == SelectedData.DC_CODE && x.GUP_CODE == SelectedData.GUP_CODE
														&& x.CUST_CODE == SelectedData.CUST_CODE && x.STOCK_NO == SelectedData.PURCHASE_NO).FirstOrDefault();

			if (f010201Data != null && !string.IsNullOrEmpty(f010201Data.SOURCE_NO))
			{
				var proxy02 = GetProxy<F00Entities>();
				var f000904Data = proxy02.F000902s.Where(x => x.SOURCE_TYPE == f010201Data.SOURCE_TYPE).FirstOrDefault();
				if (f000904Data != null)
				{
					DialogService.ShowMessage(string.Format(Properties.Resources.P0202030000_CheckQtyNotChange, f000904Data.SOURCE_NAME));
					return false;
				}
			}

			var a = Properties.Resources.Account;

			return true;
		}

		#endregion

		#region Cancel

		private bool _isCancel;
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit && SelectedData != null;
					},
					o =>
					{
						if (_isCancel)
							OnCancel();
					}
				);
			}
		}
		public void DoCancel()
		{
			_isCancel = false;
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				// 取消編輯商品檢驗
				UserOperateMode = OperateMode.Query;
				SearchCommand.Execute(null);
				_isCancel = true;
			}
		}

		#endregion

		#region Save

		private bool _isSaveOk = false;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => _isSaveOk = DoSave(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit && SelectedData != null;
					},
					c =>
					{
						if (_isSaveOk)
							OnCancel();
					}
					);
			}
		}

		public bool DoSave()
		{
			if (DoCheckData()) // 檢查輸入的驗收數是否正確
			{
				if (ShowMessage(Messages.WarningBeforeSaveP020203) != DialogResponse.Yes) return false;
				var proxy = GetExProxy<P02ExDataSource>();
				var result = proxy.CreateQuery<ExecuteResult>("UpdateP020203RecvQty")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", this._gupCode)
					.AddQueryExOption("custCode", this._custCode)
					.AddQueryExOption("purchaseNo", SelectedPurchaseNo)
					.AddQueryExOption("purchaseSeq", SelectedData.PURCHASE_SEQ)
					.AddQueryExOption("recvQty", SelectedData.RECV_QTY.ToString())
										.AddQueryExOption("userId", this._userId)
										.AddQueryExOption("rtNo", SelectedData.RT_NO)
										.ToList();
				ShowMessage(result);
				if (result[0].IsSuccessed)
				{
					UserOperateMode = OperateMode.Query;
					SearchCommand.Execute(null);
					return true;
				}
				else
					return false;
			}
			return false;
		}

		/// <summary>
		/// 檢查輸入的驗收總量是否正確
		/// </summary>
		/// <returns></returns>
		private bool DoCheckData()
		{
			var proxy = GetProxy<F01Entities>();
			if (SelectedData.RECV_QTY == null || SelectedData.RECV_QTY < 0)
			{
				ShowWarningMessage(Messages.WarningInvalidRecvQty.Message +
													 (SelectedData.ORDER_QTY - SelectedData.SUM_RECV_QTY));
				return false;
			}
			if (SelectedData.ORDER_QTY < SelectedData.RECV_QTY + SelectedData.SUM_RECV_QTY)
			{
				ShowWarningMessage("驗收總數量不得超過進貨數");
				return false;
			}
			return true;
		}

		/// <summary>
		/// 1.檢查進倉單號與預定進貨日期是否正確
		/// 2.檢查F010201尚未填採購單號且該採購單有序號商品(內部交易除外)。
		/// </summary>
		/// <returns>true: 有此單, false: 無此單</returns>
		private bool CheckPurchaseNo()
		{
			var proxy = GetExProxy<P02ExDataSource>();

			var result = proxy.CreateQuery<ExecuteResult>("CheckPurchaseNo")
								.AddQueryExOption("dcCode", SelectedDc)
								.AddQueryExOption("gupCode", _gupCode)
								.AddQueryExOption("custCode", _custCode)
								.AddQueryExOption("purchaseNo", SelectedPurchaseNo)
								.ToList()
								.FirstOrDefault();
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return result.IsSuccessed;
			}
			else
				SelectedPurchaseNo = result.No;
			return true;
		}

		/// <summary>
		/// 檢查是否有可進貨的商品序號檔
		/// </summary>
		/// <returns>false: 無, true: 有</returns>
		public bool DoCheckF2501()
		{
			var proxy = GetProxy<F25Entities>();
			var tmp = proxy.F2501s.Where(x => x.GUP_CODE.Equals(this._gupCode) && x.CUST_CODE.Equals(this._custCode))
				.Count() > 0;
			return tmp;
		}
		#endregion Save

		#region 驗收確認
		public ICommand AccpetanceCommand
		{
			get
			{
				bool _canDoAcceptanceReport = false;
				return CreateBusyAsyncCommand(
					o =>
					{
						if (!(_canDoAcceptanceReport = IsRecvQtyEqualsSerialTotal()))
						{
							return;
						}

						_canDoAcceptanceReport = DoAcceptance(Convert.ToBoolean(o));
					},
					//驗收確認點選條件：列表中商品已檢驗 且 序號商品已刷讀序號 且 驗收總量 > 0的資料 且狀態非"已驗收"
					() => UserOperateMode == OperateMode.Query && CanAcceptance(),
					o =>
					{
						// 只在驗收單產生成功才顯示報表
						if (_canDoAcceptanceReport)
						{
							// 列印驗收單及揀貨單
							if (Print()) AfterDoAcceptance();
							// 列印序號報表和調撥單貼紙
							if (PrintAcceptanceSerialAndAcceptanceAlloction()) SerialAndAlloctionReport();
						}
					}
				);
			}
		}

		/// <summary>
		/// 驗收確認
		/// </summary>
		protected bool DoAcceptance(bool chkTarWh)
		{
			// 按下驗收確認時，檢查上架倉別是否為空白，若是，跳出訊息顯示"請輸入上架倉別"，回到畫面上，若否=驗收完成
			if (chkTarWh && DgList != null && DgList.Any(x => string.IsNullOrWhiteSpace(x.TARWAREHOUSE_ID)))
			{
				ShowWarningMessage(Properties.Resources.P0202030000_TarWhIsEmpty);
				return false;
			}

			// 按下驗收確認時，如果驗收總量>=10000，跳出confirm視窗，顯示"驗收總量大於10000, 請確認?"，若按"是" =>驗收完成 若"否"=>回到畫面上
			if (DgList != null && DgList.Sum(x => x.RECV_QTY) >= 10000 && ShowConfirmMessage(Properties.Resources.AcceptanceConfirmMsg) == DialogResponse.No)
				return false;

			F051201ReportDataADatas = null;
			var proxy = new wcf.P02WcfServiceClient();
			var proxyEntities = GetProxy<F02Entities>();
			var acp = new wcf.AcceptanceConfirmParam
			{
				DcCode = SelectedDc,
				GupCode = this._gupCode,
				CustCode = this._custCode,
				PurchaseNo = SelectedPurchaseNo,
				RTNo = RtNo,
				IsPickLocFirst = IsPickLocFirst,
				RT_MODE = RT_MODE
			};
			var tmp = RunWcfMethod<wcf.AcceptanceReturnData>(proxy.InnerChannel, () => proxy.NewAcceptancePurchase(acp));
			if (tmp != null && tmp.ExecuteResult.IsSuccessed)
			{
				//通知影資系統驗收完成
				//_lastPurchaseNo = null; //先把保存的前一張進倉單清空，避免後續呼叫CallVideoService時不會執行的問題
				var ItemDoneVideoData = DgList.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.PURCHASE_NO });
				foreach (var item in ItemDoneVideoData)
        {
          //CallVideoService(item.Key.DC_CODE, item.Key.GUP_CODE, item.Key.CUST_CODE, item.Key.PURCHASE_NO, item.Select(x => x.ITEM_CODE).ToList());
          //#2149 1. 驗收完成時，呼叫 收單驗貨上架API時，單號的位置改傳驗收單號   
          CallVideoService(item.Key.DC_CODE, item.Key.GUP_CODE, item.Key.CUST_CODE, tmp.RT_NO , item.Select(x => x.ITEM_CODE).ToList());
        }
					

				// 寫入成功
				AcceptanceReportData = ReportService.GetAcceptancePurchaseReport(SelectedDc, this._gupCode, this._custCode, SelectedPurchaseNo, tmp.RT_NO, FunctionCode, "0", "0");
				AcceptanceReportData1 = ReportService.GetAcceptancePurchaseReport(SelectedDc, this._gupCode, this._custCode, SelectedPurchaseNo, tmp.RT_NO, FunctionCode, "1", "0");
				var porxyEx = GetExProxy<P02ExDataSource>();
				if (!string.IsNullOrEmpty(tmp.OrderNo))
				{
					F051201ReportDataADatas = porxyEx.CreateQuery<F051201ReportDataA>("GetF051201ReportDataAs")
						.AddQueryExOption("dcCode", SelectedDc)
						.AddQueryExOption("gupCode", _gupCode)
						.AddQueryExOption("custCode", _custCode)
						.AddQueryExOption("ordNo", tmp.OrderNo).ToList();
				}

				AcceptanceSerialDatas = tmp.AcceptanceSerialDatas.ToList();

				// 調撥單貼紙資料
				List<string> defectWarehouse = proxyEntities.F02020109s.Where(x => x.DC_CODE == SelectedDc &&
				x.GUP_CODE == this._gupCode &&
				x.CUST_CODE == this._custCode &&
				x.STOCK_NO == SelectedPurchaseNo).ToList().Select(x => x.WAREHOUSE_ID).ToList(); //不良品的倉別編號
				var f151001ReportByAcceptance = porxyEx.CreateQuery<F151001ReportByAcceptance>("GetF151001ReportByAcceptance")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("purchaseNo", SelectedPurchaseNo)
					.AddQueryExOption("rtNo", tmp.RT_NO).ToList();
				F151001ReportByAcceptance = f151001ReportByAcceptance.Where(x => !defectWarehouse.Contains(x.WAREHOUSE_ID)).ToList();
				F151001ReportByAcceptanceDefect = f151001ReportByAcceptance.Except(F151001ReportByAcceptance).ToList();
				// 不良品明細資料
				DefectDetailReport = GetExProxy<P02ExDataSource>().CreateQuery<DefectDetailReport>("GetDefectDetailReportData")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("rtNo", tmp.RT_NO).ToList();


				DoSearch();
				SetSelectedData();

				if (DgList != null && DgList.Any())
					SelectedData = DgList.First();

				var overWarehouseItemMessage = "";
				if (tmp.IsOverWarehouseItem)
					overWarehouseItemMessage = Environment.NewLine + Properties.Resources.P0202030000_OverWarehouseItem;
				if (!string.IsNullOrEmpty(tmp.OrderNo))
					overWarehouseItemMessage += Environment.NewLine + Properties.Resources.P0202030000_OverWarehouseItemGetPickNo;
				if (tmp.HasVirtualItem)
					overWarehouseItemMessage += Environment.NewLine + Properties.Resources.P0202030000_OverWarehouseItemVirtual;
				var abnormalStatusMessage = "";
				var f15Proxy = GetProxy<F15Entities>();
				// 找出異常狀態的調撥單
				var abnormalAllallocation = f15Proxy.F151001s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.SOURCE_NO == acp.PurchaseNo && x.STATUS == "8");
				foreach (var abnormalItem in abnormalAllallocation)
				{
					//顯示異常的調撥單號及備註
					abnormalStatusMessage += Environment.NewLine + abnormalItem.ALLOCATION_NO + "調撥單狀態異常，麻煩請現場人員至調撥單維護處理";
				}

				ShowMessage(new MessagesStruct
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Message = string.Format(Properties.Resources.P0202030000_CheckSuccess, overWarehouseItemMessage, abnormalStatusMessage),
					Title = Properties.Resources.Message
				});
				return true;
			}
			else
			{
				ShowMessage(new List<ExecuteResult>() {
										new ExecuteResult() { IsSuccessed = false, Message = tmp.ExecuteResult.Message}
								});
				return false;
			}
		}
		#endregion

		/// <summary>
		/// 若F0030.SYS_PATH=0不列印驗收單、揀貨單，若F0030.SYS_PATH=1，則列印驗收單、揀貨單
		/// </summary>
		/// <returns></returns>
		protected bool Print()
		{
			var proxy = GetProxy<F00Entities>();
			return proxy.F0003s.Where(x => x.AP_NAME == "AutoPrintReports" &&
																		 x.CUST_CODE == _custCode &&
																		 x.GUP_CODE == _gupCode &&
																		 x.DC_CODE == SelectedDc).Select(x => x.SYS_PATH)
												 .FirstOrDefault() == "1" ? true : false;
		}

		/// <summary>
		/// 若F0003.SYS_PATH=0不自動印調撥貼紙及序號表，若F0030.SYS_PATH=1，則列印調撥貼紙及序號表
		/// </summary>
		/// <returns></returns>
		public bool PrintAcceptanceSerialAndAcceptanceAlloction()
		{
			var proxy = GetProxy<F00Entities>();
			return proxy.F0003s.Where(x => x.AP_NAME == "AutoPrintInNote" &&
											 x.CUST_CODE == _custCode &&
											 x.GUP_CODE == _gupCode &&
											 x.DC_CODE == SelectedDc).Select(x => x.SYS_PATH)
								 .FirstOrDefault() == "1" ? true : false;
		}

		#region 取消驗收
		public ICommand CancelAcceptanceCommand
		{
			get
			{
				bool isCancelSuccessed = true;
				return CreateBusyAsyncCommand(
					o =>
					{
						if (ShowConfirmMessage(Properties.Resources.P0202030000_IsCancelCheck) == DialogResponse.Yes)
						{
							var proxy = new wcf.P02WcfServiceClient();
							var tmp = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.CancelAcceptance(SelectedDc, _gupCode, _custCode, SelectedPurchaseNo, RtNo));
							if (!tmp.IsSuccessed)
							{
								ShowWarningMessage(string.Format("{0}{1}{2}", Properties.Resources.P0202030000_CancelCheckFail, Environment.NewLine, tmp.Message));
								isCancelSuccessed = false;
							}
							else
								ShowMessage(Messages.Success);
						}
					},
					//驗收確認點選條件：列表中商品已檢驗 且 狀態非"已驗收" 且已有商品進行過商品檢驗
					() => UserOperateMode == OperateMode.Query && DgList != null && DgList.Any(x => x.CHECK_ITEM == "1" && x.STATUS == "0"),
					o =>
					{
						// 只在驗收單產生成功才顯示報表
						if (isCancelSuccessed)
							SearchCommand.Execute(null);
					}
				);
			}
		}

		#endregion

		#region 缺貨確認
		/// <summary>
		/// Gets the OutOfStock.
		/// </summary>
		public ICommand OutOfStockCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoOutOfStock(), () => UserOperateMode == OperateMode.Query && OutOfStockVisiblity == Visibility.Visible && SelectedData != null && SelectedData.BUNDLE_SERIALNO == "1" && SelectedData.SERAIL_COUNT < SelectedData.ORDER_QTY
				);
			}
		}

		public void DoOutOfStock()
		{
			if (ShowConfirmMessage(string.Format(Properties.Resources.P0202030000_ItemCheckQtyInsufficient, SelectedData.ITEM_NAME, SelectedData.ORDER_QTY - SelectedData.SERAIL_COUNT)) == DialogResponse.Yes)
			{
				var proxy = new wcf.P02WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel
					, () => proxy.OutOfStock(SelectedDc, this._gupCode, this._custCode, SelectedPurchaseNo,
									SelectedData.ITEM_CODE, SelectedData.RT_NO));
				if (!result.IsSuccessed)
					ShowWarningMessage(result.Message);
				else
				{
					ShowMessage(Messages.Success);
					SearchCommand.Execute(null);
				}

			}
		}
		#endregion OutOfStock

		#region 快速檢驗
		public ICommand QuickCheckCommand
		{
			get
			{
				var isOk = false;
				return CreateBusyAsyncCommand(
				o => isOk = QuickCheck(), () => UserOperateMode == OperateMode.Query && DgList.Any(o => o.STATUS == "0" && o.IsNotNeedCheckScan == "0" && string.IsNullOrEmpty(o.VIRTUAL_TYPE) && o.IS_QUICK_CHECK == "1"), o => { if (isOk) SearchCommand.Execute(null); }
				);
			}

		}

		private bool QuickCheck()
		{
			var proxy = new wcf.P02WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.QuickUpdateF02020102(SelectedDc, this._gupCode, this._custCode, SelectedPurchaseNo, RtNo));
			ShowResultMessage(result);
			return result.IsSuccessed;
		}
		#endregion

		#region 其它按鈕

		public bool ItemVerifyEnabled
		{
			get
			{
				return UserOperateMode == OperateMode.Query &&
					SelectedData != null &&
					SelectedData.STATUS == "0" &&
					SelectedData.IsNotNeedCheckScan == "0" &&
					string.IsNullOrEmpty(SelectedData.VIRTUAL_TYPE);
			}
		}

		public bool ReadSerialNoEnabled
		{
			get
			{
				return UserOperateMode == OperateMode.Query &&
					SelectedData != null &&
					SelectedData.BUNDLE_SERIALNO == "1" &&
					SelectedData.STATUS == "0" &&
					SelectedData.IsNotNeedCheckScan == "0" &&
					string.IsNullOrEmpty(SelectedData.VIRTUAL_TYPE);
			}
		}
		private PurchaseService _purchaseService = null;

		// P40. 商品未檢驗或是序號刷讀有未檢驗通過的商品，使用者才可點選《特殊採購》開啟特殊採購畫面
		public bool SpecialPurchaseEnabled
		{
			get
			{
				if (UserOperateMode != OperateMode.Query)
					return false;

				return (_purchaseService ?? (_purchaseService = new PurchaseService())).CanExecuteSpecial(SelectedData);
			}
		}
		// 驗收確認點選條件：列表中商品已檢驗 且 序號商品已刷讀序號 且 驗收總量 > 0的資料 且狀態非"已驗收"
		//public bool AcceptanceEnabled
		//{
		//	get
		//	{
		//		return UserOperateMode == OperateMode.Query && CanAcceptance();
		//	}
		//}
		/// <summary>
		/// 有驗收單狀態為已驗收時, 就允許上傳檔案
		/// </summary>
		public bool UploadFileEnabled { get { return UserOperateMode == OperateMode.Query && DgList.Any(x => x.STATUS == "1" && _isUpLoad == "1"); } }
		public bool PrintBoxTagEnabled { get { return UserOperateMode == OperateMode.Query && DgList.Any(x => x.HasRecvData == "1"); } }
		public bool CollectSerialNoEnabled { get { return UserOperateMode == OperateMode.Query && SelectedData != null && SelectedData.STATUS == "0" && (SelectedData.BUNDLE_SERIALNO == "1"); } }
		//public bool SetDefectQtyEnabled { get { return UserOperateMode == OperateMode.Query && SelectedData != null; } }
		/// <summary>
		/// 取出列表中, 商品已檢驗 且 序號商品已刷讀序號 且 驗收總量 > 0的資料 且狀態非"已驗收"
		/// </summary>
		/// <returns></returns>
		private IEnumerable<P020203Data> GetItemsCanDoAcceptance()
		{
			return DgList.Where(
				o => o.RECV_QTY > 0 && o.STATUS == "0" && (o.CHECK_ITEM == "1" && (o.BUNDLE_SERIALNO == "0" || o.CHECK_SERIAL == "1"))).ToList();
		}
		/// <summary>
		/// 檢查進貨數 等於 驗收總量
		/// </summary>
		/// <returns></returns>
		private bool CheckDgList()
		{
			foreach (var d in DgList)
			{
				if (d.ORDER_QTY != d.RECV_QTY)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 可驗收
		/// </summary>
		protected bool CanAcceptance()
		{
			// 若超收時，需檢核序號收集數量是否與驗收總量相等
			var f020302s = GetProxy<F02Entities>().F020302s.Where(x => x.PO_NO == SelectedPurchaseNo || x.PO_NO == ShopNo);
			foreach (var Dgitem in DgList.Where(x => x.BUNDLE_SERIALNO == "1"))
			{
				var serialCount = f020302s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.ITEM_CODE == Dgitem.ITEM_CODE && x.STATUS == "0").ToList().Count();
				if (serialCount != Dgitem.RECV_QTY)
				{
					return false;
				}
			}


			if (DgList == null || !DgList.Any() || DgList.Any(o => o.STATUS != "0") || DgList.Sum(x => x.RECV_QTY) == 0)
				return false;

			return DgList.Where(x => x.RECV_QTY > 0).All(x => x.STATUS == "0" && x.CHECK_ITEM == "1" && (x.BUNDLE_SERIALNO == "0" || x.CHECK_SERIAL == "1"));
			////抽驗 都通過
			//var result = (!DgList.Any(o => o.RECV_QTY > 0 && o.STATUS == "0"
			//					&& (o.CHECK_ITEM == "0" || (o.BUNDLE_SERIALNO == "1" && o.CHECK_SERIAL == "0"))));

			//return result;
		}
		/// <summary>
		/// 檢查進倉驗收數量
		/// </summary>
		protected bool IsRecvQtyEqualsSerialTotal()
		{
			// 無序號商品則不用檢查驗收數量
			if (DgList.All(o => o.BUNDLE_SERIALNO == "0"))
				return true;

			// 若序號商品都已經打勾特採了，就不需要檢查驗收數量
			if (DgList.Where(x => x.BUNDLE_SERIALNO == "1").All(x => x.ISSPECIAL == "1"))
				return true;

			//若有序號商品 再檢查驗收總數 = F020302 數量
			var porxyEx = GetExProxy<P02ExDataSource>();
			var data = DgList.First();
			var result = porxyEx.CreateQuery<ExecuteResult>("IsRecvQtyEqualsSerialTotal")
								.AddQueryExOption("dcCode", SelectedDc)
								.AddQueryExOption("gupCode", _gupCode)
								.AddQueryExOption("custCode", _custCode)
								.AddQueryExOption("purchaseNo", data.PURCHASE_NO)
								.ToList()
								.First();

			if (!result.IsSuccessed)
				ShowWarningMessage(result.Message);

			return result.IsSuccessed;
		}

		#endregion


		#region 重新取得LMS上架倉別指示命令
		public ICommand RegetLmsApiStowShelfAreaGuideCommand
		{
			get
			{
				wcf.ExecuteResult result = null;
				return CreateBusyAsyncCommand(
						o =>
						{
							var wcfproxy = GetWcfProxy<wcf.P02WcfServiceClient>();
							result = wcfproxy.RunWcfMethod(w => w.ReGetLmsApiStowShelfAreaGuide(SelectedDc, this._gupCode, this._custCode, SelectedPurchaseNo, WarehouseList.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => x.Value).ToArray()));
						},
						() => DgList?.Any() ?? false,
						o =>
						{
							if (result.IsSuccessed)
								SearchCommand.Execute(null);
							else
								ShowResultMessage(result);
						}
				);
			}
		}
		#endregion 重新取得LMS上架倉別指示命令

		#endregion
	}
}

