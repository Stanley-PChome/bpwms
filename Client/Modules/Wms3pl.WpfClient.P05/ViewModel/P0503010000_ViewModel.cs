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
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0503010000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		private string _gupCode;
		private string _custCode;
		public DeliveryReportService DeliveryReport;

		#region Form - 查詢
		#region Form - 物流中心
		private List<NameValuePair<string>> _dcCodes;

		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				RaisePropertyChanged("DcCodes");
			}
		}

		private string _selectDcCode;
		public string SelectDcCode
		{
			get { return _selectDcCode; }
			set
			{
				_selectDcCode = value;
				RaisePropertyChanged("SelectDcCode");
				DgList = new List<SelectionItem<F050801NoShipOrders>>();
				SetCheckOutTime = new List<NameValuePair<string>>();
				SelectPIER_CODE = "";
				ButtonList = new ObservableCollection<DynamicButtonData>();
				SearchPickTime = GetPickTimeList();
				SelectPickTime = SearchPickTime.FirstOrDefault().Value;
			}
		}
		#endregion
		#region Form - 批次日期
		private DateTime? _searchDelvDate = DateTime.Today;
		public DateTime? SearchDelvDate
		{
			get { return _searchDelvDate; }
			set
			{
				_searchDelvDate = value;
				RaisePropertyChanged("SearchDelvDate");
				SearchPickTime = GetPickTimeList();
				SelectPickTime = SearchPickTime.FirstOrDefault().Value;
			}
		}
		#endregion
		#region Form - 狀態
		private List<NameValuePair<string>> _allStatusList;

		public List<NameValuePair<string>> AllStatusList
		{
			get { return _allStatusList; }
			set
			{
				Set(() => AllStatusList, ref _allStatusList, value);
			}
		}


		private List<NameValuePair<string>> _searchSTATUS;

		public List<NameValuePair<string>> SearchSTATUS
		{
			get { return _searchSTATUS; }
			set
			{
				_searchSTATUS = value;
				RaisePropertyChanged("SearchSTATUS");
			}
		}

		private string _selectSTATUS;
		public string SelectSTATUS
		{
			get { return _selectSTATUS; }
			set
			{
				_selectSTATUS = value;
				RaisePropertyChanged("SelectSTATUS");
			}
		}
		#endregion
		#region Form - 批次時段
		private List<NameValuePair<string>> _searchPickTime;

		public List<NameValuePair<string>> SearchPickTime
		{
			get { return _searchPickTime; }
			set
			{
				_searchPickTime = value;
				RaisePropertyChanged("SearchPickTime");
			}
		}

		private string _selectPickTime;
		public string SelectPickTime
		{
			get { return _selectPickTime; }
			set
			{
				_selectPickTime = value;
				RaisePropertyChanged("SelectPickTime");
			}
		}
		#endregion
		#region Form - 訂單單號
		private string _searchORD_NO;
		public string SearchORD_NO
		{
			get { return _searchORD_NO; }
			set
			{
				_searchORD_NO = value;
				RaisePropertyChanged("SearchORD_NO");
			}
		}
		#endregion
		#region Form - 貨主單號
		private string _searchCUST_ORD_NO;
		public string SearchCUST_ORD_NO
		{
			get { return _searchCUST_ORD_NO; }
			set
			{
				_searchCUST_ORD_NO = value;
				RaisePropertyChanged("SearchCUST_ORD_NO");
			}
		}
		#endregion


		#region Data - 資料List
		private List<SelectionItem<F050801NoShipOrders>> _dgList;
		public List<SelectionItem<F050801NoShipOrders>> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private SelectionItem<F050801NoShipOrders> _selectedData;

		public SelectionItem<F050801NoShipOrders> SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
				GetF050801();
			}
		}
		#endregion
		#region Form - 勾選所有
		private bool _isCheckAll = false;
		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set { _isCheckAll = value; RaisePropertyChanged("IsCheckAll"); }
		}
		#endregion
		#region Data - 總計貨主單數
		private int? _detailCount;
		public int? DetailCount
		{
			get { return _detailCount; }
			set
			{
				_detailCount = value;
				RaisePropertyChanged("DetailCount");
			}
		}
		#endregion
		#region Form - 查詢狀態
		private bool _isSearch = true;
		public bool IsSearch
		{
			get { return _isSearch; }
			set
			{
				_isSearch = value;
				RaisePropertyChanged("IsSearch");
			}
		}
		#endregion
		#region Data - 出貨資料List
		private List<F050801> _dgWmsOrdNoList;
		public List<F050801> DgWmsOrdNoList
		{
			get { return _dgWmsOrdNoList; }
			set
			{
				_dgWmsOrdNoList = value;
				RaisePropertyChanged("DgWmsOrdNoList");
			}
		}

		private F050801 _selectWMSData;

		public F050801 SelectWMSData
		{
			get { return _selectWMSData; }
			set
			{
				_selectWMSData = value;
				RaisePropertyChanged("SelectWMSData");
				CheckButton();
			}
		}
		#endregion
		#endregion

		#region Set - 出車時間設定
		#region Form - 出車日期
		private DateTime? _setCheckoutDate = DateTime.Today;
		public DateTime? SetCheckoutDate
		{
			get { return _setCheckoutDate; }
			set { _setCheckoutDate = value; RaisePropertyChanged("SetCheckoutDate"); }
		}
		#endregion
		#region Form - 出車時段
		private List<NameValuePair<string>> _setCheckOutTime;

		public List<NameValuePair<string>> SetCheckOutTime
		{
			get { return _setCheckOutTime; }
			set
			{
				_setCheckOutTime = value;
				RaisePropertyChanged("SetCheckOutTime");
			}
		}

		private string _selectSetCheckOutTime;
		public string SelectSetCheckOutTime
		{
			get { return _selectSetCheckOutTime; }
			set
			{
				_selectSetCheckOutTime = value;
				RaisePropertyChanged("SelectSetCheckOutTime");
			}
		}
		#endregion
		#region Form - 出貨碼頭
		private string _selectPIER_CODE;
		public string SelectPIER_CODE
		{
			get { return _selectPIER_CODE; }
			set { _selectPIER_CODE = value; RaisePropertyChanged("SelectPIER_CODE"); }
		}
		#endregion
		#region From - 補印表單
		private ObservableCollection<DynamicButtonData> _buttonList = new ObservableCollection<DynamicButtonData>();
		public ObservableCollection<DynamicButtonData> ButtonList
		{
			get { return _buttonList; }
			set { _buttonList = value; RaisePropertyChanged("ButtonList"); }
		}
		#endregion
		#endregion
		#region Print 列印按鈕設定

		private F1909 _f1909Data = null;
		/// <summary>
		/// 貨主
		/// </summary>
		public F1909 F1909Data
		{
			get { return _f1909Data; }
			set { _f1909Data = value; RaisePropertyChanged("F1909Data"); }
		}

		private F055001 _f055001Data = null;
		/// <summary>
		/// 開始包裝時的主資料 - 出貨包裝頭檔
		/// Memo: 按下開始包裝, 或是刷讀箱號後寫入
		/// </summary>
		public F055001 F055001Data
		{
			get { return _f055001Data; }
			set { _f055001Data = value; RaisePropertyChanged("F055001Data"); }
		}

		private F050801 _f050801Data = null;
		public F050801 F050801Data
		{
			get { return _f050801Data; }
			set
			{
				_f050801Data = value;
				RaisePropertyChanged("F050801Data");
			}
		}
		#endregion

		#endregion

		#region 函式
		public P0503010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			_userId = Wms3plSession.Get<UserInfo>().Account;
			_userName = Wms3plSession.Get<UserInfo>().AccountName;
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			DcCodes = GetDcCodeList();
			SelectDcCode = DcCodes.FirstOrDefault().Value;

			// DataGrid ItemsSource 的狀態
			AllStatusList = GetStatusList();
			// 查詢條件沒取消這個狀態
			SearchSTATUS = AllStatusList.Where(x => x.Value != "9").ToList();
			SelectSTATUS = SearchSTATUS.FirstOrDefault().Value;
			DeliveryReport = new DeliveryReportService(FunctionCode);

			var proxyF19 = GetProxy<F19Entities>();
			F1909Data = proxyF19.F1909s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode).FirstOrDefault();
		}

		public List<NameValuePair<string>> GetDcCodeList()
		{
			return Wms3plSession.Get<GlobalInfo>().DcCodeList;
		}

		public List<NameValuePair<string>> GetStatusList()
		{
			return GetBaseTableService.GetF000904List(FunctionCode, "P050301", "STATUS", true);
		}

		public List<NameValuePair<string>> GetPickTimeList()
		{
			List<NameValuePair<string>> result = new List<NameValuePair<string>>();
			result.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
			if (!string.IsNullOrEmpty(SelectDcCode))
			{
				if (SearchDelvDate != null)
				{
					var proxy = GetProxy<F05Entities>();
					var qry = (from a in proxy.F050801s
							   where a.DC_CODE.Equals(SelectDcCode) && a.GUP_CODE.Equals(_gupCode) && a.CUST_CODE.Equals(_custCode)
								&& a.DELV_DATE == new DateTime(SearchDelvDate.Value.Year, SearchDelvDate.Value.Month, SearchDelvDate.Value.Day)
							   select a).ToList();
					var pickTimeList = qry.Select(o => o.PICK_TIME).Distinct().OrderBy(n => n);

					if (pickTimeList.Any())
					{
						int tIndex = 1;
						foreach (var item in pickTimeList)
						{
							result.Insert(tIndex, new NameValuePair<string>() { Value = item, Name = item });
							tIndex += 1;
						}
					}
				}
			}
			return result.ToList();
		}

		#region 顯示出貨單/補印表單相關

		#region 顯示補印表單按鈕
		private void BindingReportButton(F050801 data)
		{
			SetF1947ByWmsOrdNo(data);
			var f050301List = GetDataByWmsOrdNo(data);
			ButtonList = DeliveryReport.BindingReportButton(data, F055001Data, null, F1947Data, F1909Data, CreateBusyAsyncCommand, f050301List);
		}

		List<F050301> GetDataByWmsOrdNo(F050801 f050801)
		{
			var proxy = GetProxy<F05Entities>();
			var list = proxy.CreateQuery<F050301>("GetDataByWmsOrdNo")
							.AddQueryExOption("dcCode", f050801.DC_CODE)
							.AddQueryExOption("gupCode", f050801.GUP_CODE)
							.AddQueryExOption("custCode", f050801.CUST_CODE)
							.AddQueryExOption("wmsOrdNo", f050801.WMS_ORD_NO)
							.ToList();
			return list;
		}
		#endregion

		/// <summary>
		/// 刷讀單號後，先取得配送商，在列印託運單時，就能加快作業
		/// </summary>
		public F1947 F1947Data { get; set; }
		public void SetF1947ByWmsOrdNo(F050801 f050801)
		{
			var proxy = GetProxy<F19Entities>();
			var f1947 = proxy.CreateQuery<F1947>("GetAllIdByWmsOrdNo")
							.AddQueryExOption("dcCode", f050801.DC_CODE)
							.AddQueryExOption("gupCode", f050801.GUP_CODE)
							.AddQueryExOption("custCode", f050801.CUST_CODE)
							.AddQueryExOption("wmsOrdNo", f050801.WMS_ORD_NO)
							.ToList()
							.FirstOrDefault();

			if (f1947 == null || string.IsNullOrEmpty(f1947.CONSIGN_REPORT))
			{
				ShowWarningMessage(Properties.Resources.P0503010000_ConsignReportIsNull);
			}

			F1947Data = f1947;
		}

		#region 出貨明細
		public void GetF050801()
		{
			if (SelectedData != null)
			{
				var proxy = GetProxy<F05Entities>();
				var tmpF050801 = proxy.CreateQuery<F050801>("GetF050801ByOrderNo")
				  .AddQueryExOption("dcCode", SelectedData.Item.DC_CODE)
				  .AddQueryExOption("gupCode", SelectedData.Item.GUP_CODE)
				  .AddQueryExOption("custCode", SelectedData.Item.CUST_CODE)
				  .AddQueryExOption("ordNo", SelectedData.Item.ORD_NO).ToList();
				if (tmpF050801.Any())
				{
					DgWmsOrdNoList = tmpF050801.ToList();
				}
				else
				{
					DgWmsOrdNoList = new List<F050801>();
				}
			}
		}
		#endregion

		public void CheckButton()
		{
			if (SelectWMSData != null)
			{
				ButtonList = new ObservableCollection<DynamicButtonData>();
				//設定不出貨才可以補印表單
				if (Convert.ToString(SelectWMSData.STATUS).Equals("9"))
				{
					F050801Data = SelectWMSData;
					BindingReportButton(F050801Data);
				}
			}
		}
		#endregion
		#endregion

		#region Command

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch()
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
			if (SearchDelvDate == null)
			{
				MessagesStruct SearchMsg = new MessagesStruct();
				SearchMsg.Message = Properties.Resources.P0503010000_SearchDelvDateIsNull;
				SearchMsg.Button = DialogButton.OK;
				SearchMsg.Image = DialogImage.Warning;
				SearchMsg.Title = Properties.Resources.P0503010000_Alert;
				ShowMessage(SearchMsg);
				return;
			}
			//DgWmsOrdNoList = new List<F050801>();
			SelectPIER_CODE = null;
			var proxyEx = GetExProxy<P05ExDataSource>();
			var qry = proxyEx.CreateQuery<F050801NoShipOrders>("GetF050801NoShipOrders")
										.AddQueryExOption("dcCode", SelectDcCode)
										.AddQueryExOption("gupCode", _gupCode)
										.AddQueryExOption("custCode", _custCode)
										.AddQueryExOption("delvDate", SearchDelvDate.Value.ToString("yyyy/MM/dd"))
										.AddQueryExOption("pickTime", SelectPickTime)
										.AddQueryExOption("status", SelectSTATUS)
										.AddQueryExOption("ordNo", SearchORD_NO)
										.AddQueryExOption("custOrdNo", SearchCUST_ORD_NO)
					.AsQueryable().ToList();
			if (qry.Any())
			{
				DgList = qry.ToSelectionList().ToList();
				//以貨主單號計數
				DetailCount = qry.ToList().GroupBy(x => x.ORD_NO).Count();
				DgWmsOrdNoList = null;
				IsSearch = false;
			}
			else
			{
				DgList = new List<SelectionItem<F050801NoShipOrders>>();
				ButtonList = new ObservableCollection<DynamicButtonData>();
				DetailCount = 0;
				DgWmsOrdNoList = null;
				ShowMessage(Messages.InfoNoData);
			}
		}
		#endregion Search


		#endregion

	}
}
