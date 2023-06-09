using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

//using Wms3pl.Datas.Shared.Entities;
using System.Data;
using System.Windows.Media.Imaging;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202040000_ViewModel : InputViewModelBase
	{
		private string _userId;
		//public Action OnShowDetail = delegate { };
		//public Action OnShowList = delegate { };
		public Action OnPreviewComplete = delegate { };
		public P0202040000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
				//SetStatus();
			}
		}

		private void InitControls()
		{
			_userId = Wms3plSession.Get<UserInfo>().Account;
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
			//DoSearchUccList();
		}

		#region 資料連結/ 頁面參數
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value; }
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
		#region Form - 查詢條件
		private DateTime? _startDt = null;
		private DateTime? _endDt = null;
		private string _purchaseNo = string.Empty;
		private string _rtNo = string.Empty;
		private string _vnrCode = string.Empty;
		private string _vnrName = string.Empty;
        private string _vnrNameConditon = string.Empty;
        private string _custOrdNo = string.Empty;
        private string _allocationNo = string.Empty;

        public DateTime? StartDt
		{
			get { return _startDt; }
			set { _startDt = value; RaisePropertyChanged("StartDt"); }
		}
		public DateTime? EndDt
		{
			get { return _endDt; }
			set { _endDt = value; RaisePropertyChanged("EndDt"); }
		}
		public string PurchaseNo
		{
			get { return _purchaseNo; }
			set { _purchaseNo = value; RaisePropertyChanged("PurchaseNo"); }
		}
		public string RtNo
		{
			get { return _rtNo; }
			set { _rtNo = value; RaisePropertyChanged("RtNo"); }
		}
        /// <summary>
        /// 貨主單號查詢條件
        /// </summary>
        public string CustOrdNo
        {
            get { return _custOrdNo; }
            set { _custOrdNo = value; RaisePropertyChanged("CustOrdNo"); }
        }
        /// <summary>
        /// 調撥單號查詢條件
        /// </summary>
        public string AllocationNo
        {
            get { return _allocationNo; }
            set { _allocationNo = value; RaisePropertyChanged("AllocationNo"); }
        }

        /// <summary>
        /// 廠商名稱查詢條件
        /// </summary>
        public string VnrNameConditon
        {
            get { return _vnrNameConditon; }
            set { _vnrNameConditon = value; RaisePropertyChanged("VnrNameConditon"); }
        }

        public string VnrCode
		{
			get { return _vnrCode; }
			set
			{
				_vnrCode = value;
				RaisePropertyChanged("VnrCode");
				if (string.IsNullOrEmpty(VnrCode))
					VnrName = string.Empty;
			}
		}
		public string VnrName
		{
			get { return _vnrName; }
			set { _vnrName = value; RaisePropertyChanged("VnrName"); }
		}
		#endregion
		#region Data - 查詢結果
		private ObservableCollection<P020203Data> _dgList = new ObservableCollection<P020203Data>();
		public ObservableCollection<P020203Data> DgList
		{
			get { return _dgList; }
			set { _dgList = value; RaisePropertyChanged("DgList"); RaisePropertyChanged("GroupHeader"); }
		}
		private P020203Data _selectedData;
		public P020203Data SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
				if(SelectedData != null)
				{
					ShowDetailCommand.Execute(null);
				}
			}
		}
		private F020201WithF02020101 _selectedDetailData;
		public F020201WithF02020101 SelectedDetailData
		{
			get { return _selectedDetailData; }
			set
			{
				_selectedDetailData = value;
				RaisePropertyChanged("SelectedDetailData");
				//DoSearchCheckItems();
				//ItemImageSource = null;
				//ShowImage();
			}
		}
		private ObservableCollection<F020201WithF02020101> _dgList2 = new ObservableCollection<F020201WithF02020101>();
		/// <summary>
		/// 展開明細時使用的資料集
		/// </summary>
		public ObservableCollection<F020201WithF02020101> DgList2
		{
			get { return _dgList2; }
			set { _dgList2 = value; RaisePropertyChanged("DgList2"); }
		}

		#endregion
		//#region Data - 商品檢驗項目
		//private ObservableCollection<F190206CheckName> _checkList = new ObservableCollection<F190206CheckName>();
		//public ObservableCollection<F190206CheckName> CheckList
		//{
		//	get { return _checkList; }
		//	set { _checkList = value; RaisePropertyChanged("CheckList"); }
		//}
		//#endregion
		//#region Data - 檢驗未通過原因
		//private List<F1951> _uccList = new List<F1951>();
		//public List<F1951> UccList
		//{
		//	get { return _uccList; }
		//	set { _uccList = value; RaisePropertyChanged("UccList"); }
		//}
		//#endregion
		#region Report - 驗收單報表(良品)
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
		#endregion

		#region Report - 驗收單報表(不良品)
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


		#region 調撥單貼紙(良品)
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
				_f151001ReportByAcceptance.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));
				return _f151001ReportByAcceptance.ToDataTable("ado");
			}
		}
		#endregion
		#region 調撥單貼紙(不良品)
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
		#endregion
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

		#region Image


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
				if (SelectedDetailData == null || string.IsNullOrWhiteSpace(SelectedDetailData.ITEM_CODE)) _itemImageSource = null;
				else
				{
					_itemImageSource = FileService.GetItemImage(_gupCode, _custCode, SelectedDetailData.ITEM_CODE);
				}
				RaisePropertyChanged("ItemImageSource");
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
				_statusList = value;
				RaisePropertyChanged("StatusList");
			}
		}

		#endregion

		#region 驗收總數小計
		private int? _recvQtySubtotal;
		public int? RecvQtySubtotal
		{
			get { return _recvQtySubtotal; }
			set
			{
				Set(() => RecvQtySubtotal, ref _recvQtySubtotal, value);
			}
		}
		#endregion

		#region 良品數小計
		private decimal? _sumRecvQtySubtotal;
		public decimal? SumRecvQtySubtotal
		{
			get { return _sumRecvQtySubtotal; }
			set
			{
				Set(() => SumRecvQtySubtotal, ref _sumRecvQtySubtotal, value);
			}
		}
		#endregion

		#region 不良品數小計
		private int? _defectQtySubtotal;
		public int? DefectQtySubtotal
		{
			get { return _defectQtySubtotal; }
			set
			{
				Set(() => DefectQtySubtotal, ref _defectQtySubtotal, value);
			}
		}
		#endregion

		#region 訊息
		private string _message;
		public string Message
		{
			get { return _message; }
			set { Set(() => Message, ref _message, value); }
		}
		#endregion



		#endregion

		//#region 單據狀態資料

		//public void SetStatus()
		//{
		//	StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F020201", "STATUS");
		//}

		//#endregion
		#region Command
		public ICommand PreviewCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPreview(),
					() => DgList.Any() && UserOperateMode == OperateMode.Query && SelectedData != null,
					o => OnPreviewComplete()
				);
			}
		}
		private void DoPreview()
		{
			var proxyEntities = GetProxy<F02Entities>();
			// 報表資料(良品)
			AcceptanceReportData = ReportService.GetAcceptancePurchaseReport(SelectedDc, this._gupCode, this._custCode, SelectedData.PURCHASE_NO, SelectedData.RT_NO, FunctionCode,"0", "0");
			//報表資料(不良品)
			AcceptanceReportData1 = ReportService.GetAcceptancePurchaseReport(SelectedDc, this._gupCode, this._custCode, SelectedData.PURCHASE_NO, SelectedData.RT_NO, FunctionCode, "1","0");
			var defectDetail = proxyEntities.F02020109s.Where(x => x.DC_CODE == SelectedDc &&
				x.GUP_CODE == this._gupCode &&
				x.CUST_CODE == this._custCode &&
				x.STOCK_NO == SelectedData.PURCHASE_NO).ToList();

			List<string> defectWarehouse = proxyEntities.F02020109s.Where(x => x.DC_CODE == SelectedDc &&
				x.GUP_CODE == this._gupCode &&
				x.CUST_CODE == this._custCode &&
				x.STOCK_NO == SelectedData.PURCHASE_NO).ToList().Select(x => x.WAREHOUSE_ID).ToList(); //不良品的倉別編號
			var f151001ReportByAcceptance = GetExProxy<P02ExDataSource>().CreateQuery<F151001ReportByAcceptance>("GetF151001ReportByAcceptance")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("purchaseNo", SelectedData.PURCHASE_NO)
					.AddQueryExOption("rtNo", SelectedData.RT_NO)
					.AddQueryExOption("allocationNo", SelectedData.ALLOCATION_NO).ToList();
			
			F151001ReportByAcceptance = f151001ReportByAcceptance.Where(x => !defectWarehouse.Contains(x.WAREHOUSE_ID)).ToList();
			F151001ReportByAcceptanceDefect = f151001ReportByAcceptance.Except(F151001ReportByAcceptance).ToList();
			DefectDetailReport = GetExProxy<P02ExDataSource>().CreateQuery<DefectDetailReport>("GetDefectDetailReportData")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("rtNo", SelectedData.RT_NO).ToList();

		}
		#region Search
		/// <summary>
		/// 顯示清單
		/// </summary>
		public ICommand ShowListCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						DgList2 = new ObservableCollection<F020201WithF02020101>();
					},
					() => true
					//o => OnShowList()
				);
			}
		}
		/// <summary>
		/// 顯示明細
		/// </summary>
		public ICommand ShowDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
										o => DoShowDetail(),
					() => UserOperateMode == OperateMode.Query && SelectedData != null,
					o => OnShowDetailComplete()
				);
			}
		}

		private void DoShowDetail()
		{
			var proxy = GetExProxy<P02ExDataSource>();
			DgList2 = proxy.CreateQuery<F020201WithF02020101>("GetF020201WithF02020101s")
					.AddQueryExOption("dcCode", SelectedData.DC_CODE)
					.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
					.AddQueryExOption("custCode", SelectedData.CUST_CODE)
					.AddQueryExOption("purchaseNo", SelectedData.PURCHASE_NO)
					.AddQueryExOption("rtNo", SelectedData.RT_NO)
					.ToList().ToObservableCollection();
		}

		private void OnShowDetailComplete()
		{
			if (DgList2 != null && DgList2.Any())
			{
				SelectedDetailData = DgList2.FirstOrDefault();
			}
			//OnShowDetail();
		}

		public ICommand ShowDetailItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => { },
						() => UserOperateMode == OperateMode.Query && SelectedDetailData != null
								&& SelectedDetailData.BUNDLE_SERIALNO == "1"
				);
			}
		}
		/// <summary>
		/// 取得商品檢驗項目
		/// </summary>
		//public void DoSearchCheckItems()
		//{
		//	if (SelectedDetailData == null) return;
		//	var proxy = GetExProxy<P02ExDataSource>();
		//	var result = proxy.CreateQuery<F190206CheckName>("GetItemCheckList")
		//		.AddQueryExOption("dcCode", SelectedDc)
		//		.AddQueryExOption("gupCode", this._gupCode)
		//		.AddQueryExOption("custCOde", this._custCode)
		//		.AddQueryExOption("purchaseNO", SelectedDetailData.PURCHASE_NO)
		//		.AddQueryExOption("purchaseSeq", SelectedDetailData.PURCHASE_SEQ)
		//		.AddQueryExOption("itemCode", SelectedDetailData.ITEM_CODE)
		//		.AddQueryExOption("rtNo", SelectedDetailData.RT_NO)
		//		.AddQueryExOption("checkType", "00") // 進貨檢驗
		//		.ToList();
		//	CheckList = result.ToObservableCollection();
		//}
		//private void DoSearchUccList()
		//{
		//	var proxy = GetProxy<F19Entities>();
		//	var result = proxy.F1951s.Where(x => x.UCT_ID.Equals("CC")).ToList();
		//	UccList = result;
		//}
		private void ShowImage()
		{
			ItemImageSource = null;
			RaisePropertyChanged("ItemImageSource");
		}
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
			Message = string.Empty;
			if (!string.IsNullOrWhiteSpace(CustOrdNo) || !string.IsNullOrWhiteSpace(PurchaseNo))
			{
				var proxyF01 = GetProxy<F01Entities>();
				var f010201 = new F010201();
				var f010201s = proxyF01.F010201s.Where(x => x.GUP_CODE == _gupCode &&
									   x.CUST_CODE == _custCode );
				if (!string.IsNullOrWhiteSpace(CustOrdNo))
				{
					f010201 = f010201s.Where(x => x.CUST_ORD_NO == CustOrdNo).FirstOrDefault();
				}
				if (!string.IsNullOrWhiteSpace(PurchaseNo))
				{
					f010201 = f010201s.Where(x => x.STOCK_NO == PurchaseNo).FirstOrDefault(); 
				}
				if (f010201?.CUST_COST == "MoveIn")
				{
					Message = "此為跨庫調撥上架的進倉單，並未進行驗收，無驗收單資料";
					return;
				}
				
			}

			if (string.IsNullOrWhiteSpace(Message))
			{
				var proxy = GetExProxy<P02ExDataSource>();

				var result = proxy.CreateQuery<P020203Data>("GetP020204")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", this._gupCode)
					.AddQueryExOption("custCode", this._custCode)
					.AddQueryExOption("purchaseNo", PurchaseNo)
					.AddQueryExOption("rtNo", RtNo)
					.AddQueryExOption("vnrCode", VnrCode)
					.AddQueryExOption("custOrdNo", CustOrdNo)
					.AddQueryExOption("allocationNo", AllocationNo)
					.AddQueryExOption("vnrNameConditon", VnrNameConditon)
					.AddQueryExOption("startDt", StartDt?.ToString("yyyy/MM/dd"))
					.AddQueryExOption("endDt", EndDt?.AddDays(1).ToString("yyyy/MM/dd"))
					.ToList();
				DgList = result.ToObservableCollection();


				RecvQtySubtotal = DgList.Sum(x => x.RECV_QTY);
				SumRecvQtySubtotal = DgList.Sum(x => x.SUM_RECV_QTY);
				DefectQtySubtotal = DgList.Sum(x => x.DEFECT_QTY);
			}

			
		}
		public void DoSearchComplete()
		{
			DoSearchVnr();
			if (DgList.Any())
			{
				SelectedData = DgList.FirstOrDefault();
				//OnShowList();
			}
			else
			{
				
				DialogService.ShowMessage(!string.IsNullOrWhiteSpace(Message)?Message: Messages.InfoNoData.Message);
				DgList2 = null;
			}
				
		}
		public ICommand SearchVnrCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchVnr(),
					() => true
				);
			}
		}
		private void DoSearchVnr()
		{
			var proxy = GetProxy<F19Entities>();
			var tmp = proxy.F1908s.Where(x => x.VNR_CODE == VnrCode && x.GUP_CODE == this._gupCode).FirstOrDefault();
			if (tmp == null) VnrName = string.Empty;
			else VnrName = tmp.VNR_NAME;
		}

		#endregion
		#endregion

		#region 效期調整
		public Action OpenViewImage = delegate { };

		public ICommand ShowDetailImageCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
					},
					() => true,
					o => { OpenViewImage(); }

				);
			}
		}
		#endregion
	}
}
