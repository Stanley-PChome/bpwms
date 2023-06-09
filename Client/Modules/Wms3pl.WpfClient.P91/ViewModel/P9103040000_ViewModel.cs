using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.LabelPrinter.Bartender;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.DataServices.F91DataService;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common.Helpers;


namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9103040000_ViewModel : InputViewModelBase
	{
		public P9103040000_ViewModel()
		{

			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				LabelType = GetF1970();

				WarrantyType = GetBaseTableService.GetF000904List(FunctionCode, "F1970", "WARRANTY");

				SelectF197001 = new F197001Data();
				SearchF197001 = new F197001();
				WarrantyTypeMonth = SetMonthData();
				WrrantyYear = SetCharList('A', 'Z');
				WrrantyMonth = SetCharList('A', 'L');
				InpPrintPage = 1;
				if (LabelType != null && LabelType.Any())
					SearchF197001.LABEL_CODE = LabelType.SelectFirstOrDefault(x => x.LABEL_CODE);
			}
		}

		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		#region F197001 查詢參數
		private F197001 _searchF197001;
		public F197001 SearchF197001
		{
			get { return _searchF197001; }
			set
			{
				_searchF197001 = value;
				RaisePropertyChanged("SearchF197001");
			}
		}
		#endregion

		#region GV查詢 DGList
		private ObservableCollection<F197001Data> _dgQueryDataF197001;
		public ObservableCollection<F197001Data> DgQueryDataF197001
		{
			get { return _dgQueryDataF197001; }
			set
			{
				_dgQueryDataF197001 = value;
				RaisePropertyChanged("DgQueryDataF197001");
			}
		}
		#endregion

		#region F197001 SelectData  參數
		private F197001Data _selectF197001Data;
		public F197001Data SelectF197001
		{
			get { return _selectF197001Data; }
			set
			{
				_selectF197001Data = value;
				RaisePropertyChanged("SelectF197001");
			}
		}
		#endregion

		#region F197001Data 新增 參數
		private F197001 _addF197001Data;
		public F197001 AddF197001Data
		{
			get { return _addF197001Data; }
			set
			{
				_addF197001Data = value;
				RaisePropertyChanged("AddF197001Data");
			}
		}
		#endregion

		#region 標籤規則參數
		private F1970 _labelSeting;
		public F1970 LableSeting
		{
			get { return _labelSeting; }
			set
			{
				_labelSeting = value;
				RaisePropertyChanged("LableSeting");
			}
		}

		private bool _warrantyGroup;
		public bool WrrantyGroup
		{
			get { return _warrantyGroup; }
			set
			{
				_warrantyGroup = value;
				RaisePropertyChanged("WrrantyGroup");
			}
		}


		#endregion

		#region 標籤類型 List
		private List<F1970> _labelType;
		public List<F1970> LabelType
		{
			get { return _labelType; }
			set
			{
				_labelType = value;
				RaisePropertyChanged("LabelType");
			}
		}

		private F1970 _labelTypeItem;
		public F1970 LabelTypeItem
		{
			get { return _labelTypeItem; }
			set
			{
				_labelTypeItem = value;
				//設定規則
				if (value != null)
				{
					GetF1970LabelSeting(value.LABEL_CODE);
					//變更選項清空值
					ClearAddValue();
					SetAddDefault(LableSeting);
				}
				RaisePropertyChanged("LabelTypeItem");
			}
		}

		private F1970 _labelTypeItemQ;
		public F1970 LabelTypeItemQ
		{
			get { return _labelTypeItemQ; }
			set
			{
				_labelTypeItemQ = value;
				RaisePropertyChanged("LabelTypeItemQ");
			}
		}
		#endregion

		#region 保固日期類型
		private List<NameValuePair<string>> _warrantyType;
		public List<NameValuePair<string>> WarrantyType
		{
			get { return _warrantyType; }
			set
			{
				_warrantyType = value;
				RaisePropertyChanged("WarrantyType");
			}
		}


		private NameValuePair<string> _warrantyTypeItem;
		public NameValuePair<string> WarrantyTypeItem
		{
			get { return _warrantyTypeItem; }
			set
			{
				_warrantyTypeItem = value;
				RaisePropertyChanged("WarrantyTypeItem");
			}
		}
		#endregion

		#region 保固類型 月份參數
		private List<NameValuePair<string>> _warrantyTypeMonth;
		public List<NameValuePair<string>> WarrantyTypeMonth
		{
			get { return _warrantyTypeMonth; }
			set
			{
				_warrantyTypeMonth = value;
				RaisePropertyChanged("WarrantyTypeMonth");
			}
		}

		private NameValuePair<string> _warrantyTypeMonthItem;
		public NameValuePair<string> WarrantyTypeMonthItem
		{
			get { return _warrantyTypeMonthItem; }
			set
			{
				_warrantyTypeMonthItem = value;
				RaisePropertyChanged("WarrantyTypeMonthItem");
			}
		}
		#endregion

		#region 保固日期 年參數
		private List<NameValuePair<string>> _warrantyYear;
		public List<NameValuePair<string>> WrrantyYear
		{
			get { return _warrantyYear; }
			set
			{
				_warrantyYear = value;
				RaisePropertyChanged("WrrantyYear");
			}
		}

		private NameValuePair<string> _warrantyYearItem;
		public NameValuePair<string> WrrantyYearItem
		{
			get { return _warrantyYearItem; }
			set
			{
				_warrantyYearItem = value;
				RaisePropertyChanged("WrrantyYearItem");
			}
		}
		#endregion

		#region 保固日期 月份參數
		private List<NameValuePair<string>> _warrantyMonth;
		public List<NameValuePair<string>> WrrantyMonth
		{
			get { return _warrantyMonth; }
			set
			{
				_warrantyMonth = value;
				RaisePropertyChanged("WrrantyMonth");
			}
		}

		private NameValuePair<string> _warrantyMonthItem;
		public NameValuePair<string> WrrantyMonthItem
		{
			get { return _warrantyMonthItem; }
			set
			{
				_warrantyMonthItem = value;
				RaisePropertyChanged("WrrantyMonthItem");
			}
		}

		#endregion

		#region 輸入Item Code 參數
		private string _inpItemCode;
		public string InpItemCode
		{
			get { return _inpItemCode; }
			set
			{
				_inpItemCode = value;
				RaisePropertyChanged("InpItemCode");
			}
		}

		private string _inpItemName;
		public string InpItemName
		{
			get { return _inpItemName; }
			set
			{
				_inpItemName = value;
				RaisePropertyChanged("InpItemName");
			}
		}

		private string _inpItemCustItemCode;
		public string InpItemCustItemCode
		{
			get { return _inpItemCustItemCode; }
			set
			{
				_inpItemCustItemCode = value;
				RaisePropertyChanged("InpItemCustItemCode");
			}
		}

		private string _inpItemSUGR;
		public string InpItemSUGR
		{
			get { return _inpItemSUGR; }
			set
			{
				_inpItemSUGR = value;
				RaisePropertyChanged("InpItemSUGR");
			}
		}

		#endregion

		#region 輸入VNR Code 參數
		private string _inpVNRCode;
		public string InpVNRCode
		{
			get { return _inpVNRCode; }
			set
			{
				_inpVNRCode = value;
				RaisePropertyChanged("InpVNRCode");
			}
		}

		private string _inpVNRName;
		public string InpVNRName
		{
			get { return _inpVNRName; }
			set
			{
				_inpVNRName = value;
				RaisePropertyChanged("InpVNRName");
			}
		}
		#endregion

		#region 選擇標籤類型參數
		private string _selectValue;
		public string SelectValue
		{
			get { return _selectValue; }
			set
			{
				_selectValue = value;
				RaisePropertyChanged("SelectValue");
			}
		}
		#endregion

		#region 列印張數參數

		private int _inpPrintPage;
		public int InpPrintPage
		{
			get { return _inpPrintPage; }
			set
			{
				_inpPrintPage = value;
				RaisePropertyChanged("InpPrintPage");
			}
		}
		#endregion

		/// <summary>
		/// Device 的設定 (當物流中心改變時，就會去顯示設定 Device 的畫面)  
		/// </summary>
		public F910501 SelectedF910501 { get; set; }

		#region 委外商清單

		private List<NameValuePair<string>> _outsourceList;

		public List<NameValuePair<string>> OutsourceList
		{
			get
			{
				// 有需要在取得
				if (_outsourceList == null)
				{
					var proxy = GetProxy<F19Entities>();
					_outsourceList = proxy.F1928s.Select(x => new NameValuePair<string>(string.Format("{0} {1}", x.OUTSOURCE_ID, x.OUTSOURCE_NAME),
																						x.OUTSOURCE_ID))
												 .ToList();
				}

				return _outsourceList;
			}
			set
			{
				Set(() => OutsourceList, ref _outsourceList, value);
			}
		}
		#endregion

		#region Function

		#region 標籤類型資料
		private List<F1970> GetF1970()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1970s.Where(o => (o.LABEL_TYPE == "0" || o.LABEL_TYPE == "1")
							&& o.STATUS != "9"
							&& o.GUP_CODE == _gupCode
							&& o.CUST_CODE == _custCode)
							.OrderBy(o => o.LABEL_TYPE).ToList();

			return data;
		}
		#endregion

		#region 標籤資料[規則]設定
		private void GetF1970LabelSeting(string labelCode)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1970s.Where(o => o.LABEL_CODE == labelCode
							&& o.GUP_CODE == _gupCode
							&& o.CUST_CODE == _custCode
						).FirstOrDefault();
			if (data != null)
			{
				//取得設定規則參數
				LableSeting = new F1970
				{
					CHECK_STAFF = data.CHECK_STAFF,
					ITEM = data.ITEM,
					ITEM_DESC = data.ITEM_DESC,
					OUTSOURCE = data.OUTSOURCE,
					VNR = data.VNR,
					WARRANTY = data.WARRANTY,
					WARRANTY_D = data.WARRANTY_D,
					WARRANTY_M = data.WARRANTY_M,
					WARRANTY_Y = data.WARRANTY_Y
				};

				if (LableSeting.WARRANTY_Y == "1" || LableSeting.WARRANTY_M == "1" || LableSeting.WARRANTY_D == "1")
				{
					WrrantyGroup = true;
				}
				else
				{
					WrrantyGroup = false;
				}

			}
		}
		#endregion

		#region 設定廠商資料
		public void SetVnrInfo(string gupCode, string custCode)
		{

			var proxy = GetProxy<F19Entities>();
			var item =
				proxy.F1908s.Where(
					o => o.VNR_CODE == InpVNRCode
						&& o.GUP_CODE == gupCode
					).ToList().FirstOrDefault();
			if (item != null)
			{
				AddF197001Data.VNR_CODE = item.VNR_CODE;
				InpVNRName = item.VNR_NAME;
			}
			else
			{
				InpVNRCode = "";
				InpVNRName = "";
				AddF197001Data.VNR_CODE = "";
				DialogService.ShowMessage(Properties.Resources.P9103040000_ViewModel_VnrCodeNotExist);
			}

		}
		#endregion

		#region 取 Item 資訊
		public void GetItemData(string gupCode, string custCode)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1903s.Where(o => o.ITEM_CODE == InpItemCode
								&& o.GUP_CODE == gupCode
                                && o.CUST_CODE == custCode).ToList().FirstOrDefault();
			if (data != null)
			{
				AddF197001Data.ITEM_CODE = data.ITEM_CODE;

				InpItemName = data.ITEM_NAME;
				InpItemSUGR = data.SIM_SPEC;
				GetF1903Data(data.ITEM_CODE, _gupCode);
			}
			else
			{
				InpItemCode = "";
				InpItemName = "";
				InpItemCustItemCode = "";
				InpItemSUGR = "";

				AddF197001Data.ITEM_CODE = "";

				DialogService.ShowMessage(Properties.Resources.P9103040000_ViewModel_ItemCodeNotExist);

			}
		}
		#endregion

		#region 取 Item F1903 資訊
		public void GetF1903Data(string itemCode, string gupCode)
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1903s.Where(o => o.ITEM_CODE == itemCode
								&& o.GUP_CODE == gupCode
								&& o.CUST_CODE == _custCode
								).ToList().FirstOrDefault();
			if (data != null)
			{
				InpItemCustItemCode = data.CUST_ITEM_CODE;
			}
			else
			{
				InpItemCustItemCode = "";
				InpItemSUGR = "";
			}

		}
		#endregion

		#region 設定月份資料
		private List<NameValuePair<string>> SetMonthData()
		{

			var month = new List<NameValuePair<string>>();

			for (int i = 1; i <= 12; i++)
			{
				month.Add(new NameValuePair<string> { Name = i.ToString(), Value = i.ToString() });
			}

			return month;
		}
		#endregion

		#region 設定月份資料
		private List<NameValuePair<string>> SetCharList(char startString, char endString)
		{

			var data = new List<NameValuePair<string>>();

			for (int i = Convert.ToByte(startString); i <= Convert.ToByte(endString); i++)
			{
				data.Add(new NameValuePair<string> { Name = Convert.ToChar(i).ToString(), Value = Convert.ToChar(i).ToString() });
			}

			return data;
		}
		#endregion

		#region 編輯儲存前資料檢查
		private bool CheckAddData()
		{

			var result = true;
			string errorStr = "";

			if (LableSeting != null)
			{
				//商品檢查
				if (LableSeting.ITEM == "1")
				{
					if (string.IsNullOrEmpty(AddF197001Data.ITEM_CODE) || string.IsNullOrEmpty(InpItemName))
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputItemInfo;
					}
				}

				//廠商檢查
				if (LableSeting.VNR == "1")
				{
					if (string.IsNullOrEmpty(AddF197001Data.VNR_CODE) || string.IsNullOrEmpty(InpVNRName))
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputVNRInfo;
					}
				}

				//保固類型檢查
				if (LableSeting.WARRANTY == "1")
				{
					if (string.IsNullOrEmpty(AddF197001Data.WARRANTY) || string.IsNullOrEmpty(AddF197001Data.WARRANTY_S_Y.ToString()) || string.IsNullOrEmpty(AddF197001Data.WARRANTY_S_M))
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputWarrantyTypeInfo;
					}
				}

				//保固日期檢查
				if (LableSeting.WARRANTY_Y == "1" || LableSeting.WARRANTY_M == "1" || LableSeting.WARRANTY_D == "1")
				{
					if (string.IsNullOrEmpty(AddF197001Data.WARRANTY_Y) && LableSeting.WARRANTY_Y == "1")
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputWarrantyDateInfo;
					}
					else if (string.IsNullOrEmpty(AddF197001Data.WARRANTY_M) && LableSeting.WARRANTY_M == "1")
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputWarrantyDateInfo;
					}
					else if (string.IsNullOrEmpty(AddF197001Data.WARRANTY_D.ToString()) && LableSeting.WARRANTY_D == "1")
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputWarrantyDateInfo;
					}
				}

				//委外商檢查
				if (LableSeting.OUTSOURCE == "1")
				{
					if (string.IsNullOrEmpty(AddF197001Data.OUTSOURCE))
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputSubContractorInfo;
					}
					else if (!ValidateHelper.IsMatchAZaz09(AddF197001Data.OUTSOURCE))
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_SubcontractorCode_CNWordOnly;
					}
				}

				//檢驗員檢查
				if (LableSeting.CHECK_STAFF == "1")
				{
					if (string.IsNullOrEmpty(AddF197001Data.CHECK_STAFF))
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputTestInfo;
					}

				}
				//物料說明檢查
				if (LableSeting.ITEM_DESC == "1")
				{
					if (string.IsNullOrEmpty(AddF197001Data.ITEM_DESC_A) && string.IsNullOrEmpty(AddF197001Data.ITEM_DESC_B) && string.IsNullOrEmpty(AddF197001Data.ITEM_DESC_C))
					{
						errorStr += Properties.Resources.P9103040000_ViewModel_InputSourceInstruction;
					}
				}

				if (!string.IsNullOrEmpty(errorStr))
				{
					DialogService.ShowMessage(errorStr);
					return false;
				}

			}

			return result;
		}
		#endregion

		#region 新增-預設值

		private void SetAddDefault(F1970 f1970)
		{

			if (LabelType != null)
			{
				//物料說明
				if (LableSeting.ITEM_DESC == "1")
				{
					AddF197001Data.ITEM_DESC_A = Properties.Resources.P9103040000_ViewModel_cellBattery;
					AddF197001Data.ITEM_DESC_B = Properties.Resources.P9103040000_ViewModel_transline;
				}
				//委外廠商預設值
				if (LableSeting.OUTSOURCE == "1")
					AddF197001Data.OUTSOURCE = "A";
			}

		}

		#endregion

		#region Properties.Resources.P9103040000_ViewModel_Insert_DefaultClear
		private void ClearAddValue()
		{
			if (UserOperateMode == OperateMode.Add)
			{
				//SelectF197001 = new F197001Data();				
				InpItemCode = "";
				InpItemName = "";
				InpItemCustItemCode = "";
				InpItemSUGR = "";
				InpVNRCode = "";
				InpVNRName = "";
				if (AddF197001Data != null)
				{
					AddF197001Data = new F197001();
					AddF197001Data.LABEL_CODE = LabelTypeItem.LABEL_CODE;
				}
			}
		}
		#endregion

		#region 列印

		private Wms3pl.WpfClient.ExDataServices.ShareExDataService.ExecuteResult PrintF197001Data(F197001Data f197001Data)
		{
			var printObj = new LabelPrintHelper(FunctionCode);

			var labelItem = new LableItem();

			//標籤資訊
			labelItem.LableCode = f197001Data.LABEL_CODE;
			labelItem.LableName = f197001Data.LABEL_NAME;

			//業主 ; 貨主
			labelItem.GupCode = f197001Data.GUP_CODE;
			labelItem.CustCode = f197001Data.CUST_CODE;
			//商品資訊
			labelItem.ItemCode = f197001Data.ITEM_CODE;
			labelItem.ItemColor = f197001Data.ITEM_COLOR;
			labelItem.ItemName = f197001Data.ITEM_NAME;
			labelItem.ItemSize = f197001Data.ITEM_SIZE;
			labelItem.ItemSpec = f197001Data.ITEM_SPEC;
			labelItem.ItemDesc1 = string.IsNullOrEmpty(f197001Data.ITEM_DESC_A) ? "" : "1. " + f197001Data.ITEM_DESC_A;
			labelItem.ItemDesc2 = string.IsNullOrEmpty(f197001Data.ITEM_DESC_B) ? "" : "2. " + f197001Data.ITEM_DESC_B;
			labelItem.ItemDesc3 = string.IsNullOrEmpty(f197001Data.ITEM_DESC_C) ? "" : "3. " + f197001Data.ITEM_DESC_C;
			//廠商
			labelItem.VnrCode = f197001Data.VNR_CODE;
			labelItem.VnrName = f197001Data.VNR_NAME;
			//保固資訊
			labelItem.WarrantyType = f197001Data.WARRANTY;
			labelItem.WarrantyTypeYear = f197001Data.WARRANTY_S_Y.ToString();
			labelItem.WarrantyTypeMonth = f197001Data.WARRANTY_S_M;
			labelItem.WarrantyCode = f197001Data.WARRANTY_Y + f197001Data.WARRANTY_M;
			labelItem.WarrantyDate = f197001Data.WARRANTY_D.ToString();
			labelItem.WarrantyCodeDate = f197001Data.WARRANTY_Y + f197001Data.WARRANTY_M + f197001Data.WARRANTY_D;

			//其它
			labelItem.OutSource = f197001Data.OUTSOURCE;
			labelItem.SUGR = f197001Data.SUGR;
			labelItem.CheckStaff = f197001Data.CHECK_STAFF;
			labelItem.CustItemCode = f197001Data.CUST_ITEM_CODE;

			//列印時間
			labelItem.PrintDate = DateTime.Today.ToString("yyyy/MM/dd");
			labelItem.PrintTime = DateTime.Now.ToString("hh:mm:ss");
			labelItem.PrintYearMonth = string.Format(Properties.Resources.P9103040000_ViewModel_YM, DateTime.Today.Year, DateTime.Today.Month);
			//有效期限
			labelItem.ValidDate = DateTime.Today.AddYears(1).ToShortDateString();

			var result = printObj.DoPrint(labelItem, SelectedF910501.LABELING, f197001Data.Qty);
			return result;
		}

		private IEnumerable<Wms3pl.WpfClient.ExDataServices.ShareExDataService.ExecuteResult> PrintF197001Datas(List<F197001Data> f197001Datas)
		{
			foreach (var item in f197001Datas)
			{
				yield return PrintF197001Data(item);
			}
		}

		private void DoPrintLabel()
		{
			var selectedList = DgQueryDataF197001.Where(x => x.IsChecked && x.LABEL_CODE != null).ToList();
			if (!selectedList.Any())
				return;

			var results = PrintF197001Datas(selectedList).ToList();
			if (results.Any(x => !x.IsSuccessed))
			{
				ShowWarningMessage(results.Where(x => !x.IsSuccessed).Select(x => x.Message).FirstOrDefault());
			}
		}

		#endregion

		#endregion

		#region Print
		public ICommand PrintCommand
		{
			get
			{
				bool isPrint = false;
				return CreateBusyAsyncCommand(
					o => isPrint = DoPrint(),
					() => UserOperateMode == OperateMode.Query && DgQueryDataF197001 != null && DgQueryDataF197001.Any(x => x.IsChecked)
					, o =>
					{
						if (isPrint)
						{
							DoPrintLabel();
						}
					});
			}
		}

		private bool DoPrint()
		{
			//執行查詢動作
			var selectedList = DgQueryDataF197001.Where(x => x.IsChecked && x.LABEL_CODE != null).ToList();

			if (selectedList.Any(x => x.Qty <= 0 || x.Qty > 500))
			{
				ShowWarningMessage(Properties.Resources.P9103040000_ViewModel_PrintRange500);
				return false;
			}

			if (ShowMessage(Messages.WarningBeforePrint) != DialogResponse.Yes)
			{
				return false;
			}

			var wcfF197001Datas = selectedList.MapCollection<F197001Data, wcf.F197001>().ToArray();
			var wcfF1970 = LabelTypeItemQ.Map<F1970, wcf.F1970>();

			var proxy = GetWcfProxy<wcf.P19WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.InsertOrUpdateF197001s(wcfF1970, wcfF197001Datas));
			return result.IsSuccessed;
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					, O => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
		}

		private void DoSearchComplete()
		{
			//執行查詢動作
			var proxyP19 = GetExProxy<P19ExDataSource>();
			var f197001QueryData = proxyP19.CreateQuery<F197001Data>("GetF197001Data")
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("labelCode", (SearchF197001.LABEL_CODE == null) ? "" : SearchF197001.LABEL_CODE)
					.AddQueryExOption("itemCode", (SearchF197001.ITEM_CODE == null) ? "" : SearchF197001.ITEM_CODE)
					.AddQueryExOption("vnrCode", (SearchF197001.VNR_CODE == null) ? "" : SearchF197001.VNR_CODE);

			DgQueryDataF197001 = f197001QueryData.ToObservableCollection();

			if (DgQueryDataF197001 == null || DgQueryDataF197001.Count() == 0)
			{
				ShowMessage(Messages.InfoNoData);
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
					, o => DoAddComplete()
					);
			}
		}

		private void DoAdd()
		{
		}
		private void DoAddComplete()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作
			AddF197001Data = new F197001();
			LableSeting = new F1970();
			if (LabelType != null && LabelType.Any())
			{
				LabelTypeItem = LabelType.First();
				LableSeting.LABEL_CODE = LabelTypeItem.LABEL_CODE;
			}

		}

		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query
					, o => DoEditComplete()
					);
			}
		}

		private void DoEdit()
		{


		}

		private void DoEditComplete()
		{
			if (SelectF197001 == null || SelectF197001.LABEL_CODE == null || SelectF197001.LABEL_SEQ == 0)
			{
				DialogService.ShowMessage(Properties.Resources.P9103040000_ViewModel_chooseData);
				return;
			}
			UserOperateMode = OperateMode.Edit;
			if (AddF197001Data == null) AddF197001Data = new F197001();
			GetF1970LabelSeting(SelectF197001.LABEL_CODE);
			LableSeting.LABEL_CODE = SelectF197001.LABEL_CODE;



			InpItemCode = SelectF197001.ITEM_CODE;
			if (!string.IsNullOrEmpty(InpItemCode)) GetItemData(SelectF197001.GUP_CODE, SelectF197001.CUST_CODE);
			InpVNRCode = SelectF197001.VNR_CODE;
			if (!string.IsNullOrEmpty(InpVNRCode)) SetVnrInfo(SelectF197001.GUP_CODE, SelectF197001.CUST_CODE);


			AddF197001Data.WARRANTY = SelectF197001.WARRANTY;
			AddF197001Data.WARRANTY_S_Y = SelectF197001.WARRANTY_S_Y;
			AddF197001Data.WARRANTY_S_M = SelectF197001.WARRANTY_S_M;

			AddF197001Data.WARRANTY_Y = SelectF197001.WARRANTY_Y;
			AddF197001Data.WARRANTY_M = SelectF197001.WARRANTY_M;
			AddF197001Data.WARRANTY_D = SelectF197001.WARRANTY_D;

			AddF197001Data.OUTSOURCE = SelectF197001.OUTSOURCE;
			AddF197001Data.CHECK_STAFF = SelectF197001.CHECK_STAFF;
			AddF197001Data.ITEM_DESC_A = SelectF197001.ITEM_DESC_A;
			AddF197001Data.ITEM_DESC_B = SelectF197001.ITEM_DESC_B;
			AddF197001Data.ITEM_DESC_C = SelectF197001.ITEM_DESC_C;

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
			LableSeting = null;
			AddF197001Data = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && DgQueryDataF197001 != null && DgQueryDataF197001.Any(x => x.IsChecked)
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				var selectedList = DgQueryDataF197001.Where(x => x.IsChecked && x.LABEL_SEQ > 0).Select(x => ExDataMapper.Map<F197001Data, wcf.F197001Data>(x)).ToArray();
				var proxy = new wcf.P19WcfServiceClient();
				var wcf197001 = ExDataMapper.Map<F197001Data, wcf.F197001Data>(SelectF197001);
				var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DelF197001s(selectedList));
				var result = wcfResult.IsSuccessed;
				ShowMessage(Messages.InfoDeleteSuccess);
				DoSearchComplete();

			}

		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			bool result = false;
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
				ClearAddValue();
				DoSearchComplete();
				ShowMessage(Messages.Success);
				UserOperateMode = OperateMode.Query;

			}
		}

		private bool DoSaveAdd()
		{
			var result = CheckAddData();
			if (result)
			{
				AddF197001Data.GUP_CODE = _gupCode;
				AddF197001Data.CUST_CODE = _custCode;

				List<F197001> f197001List = new List<F197001>();
				f197001List.Add(AddF197001Data);

				var proxy = new wcf.P19WcfServiceClient();
				var wcfInsertData = ExDataMapper.MapCollection<F197001, wcf.F197001>(f197001List).ToArray();
				var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertF197001(wcfInsertData));
				result = wcfResult.IsSuccessed;

				if (!result)
					DialogService.ShowMessage(wcfResult.Message);

				SearchF197001.LABEL_CODE = LableSeting.LABEL_CODE;
				SearchF197001.ITEM_CODE = AddF197001Data.ITEM_CODE;
				SearchF197001.VNR_CODE = AddF197001Data.VNR_CODE;
			}
			return result;
		}

		private bool DoSaveUpdate()
		{
			var result = CheckAddData();
			if (result)
			{

				var f197001 = new F197001();
				//不可變更資料
				f197001.LABEL_SEQ = SelectF197001.LABEL_SEQ;
				f197001.LABEL_CODE = SelectF197001.LABEL_CODE;
				f197001.ITEM_CODE = SelectF197001.ITEM_CODE;
				f197001.VNR_CODE = SelectF197001.VNR_CODE;
				f197001.CUST_CODE = SelectF197001.CUST_CODE;
				f197001.GUP_CODE = SelectF197001.GUP_CODE;

				//修改資料
				f197001.WARRANTY = AddF197001Data.WARRANTY;
				f197001.WARRANTY_S_Y = AddF197001Data.WARRANTY_S_Y;
				f197001.WARRANTY_S_M = AddF197001Data.WARRANTY_S_M;
				f197001.WARRANTY_Y = AddF197001Data.WARRANTY_Y;
				f197001.WARRANTY_M = AddF197001Data.WARRANTY_M;
				f197001.WARRANTY_D = AddF197001Data.WARRANTY_D;
				f197001.OUTSOURCE = AddF197001Data.OUTSOURCE;
				f197001.CHECK_STAFF = AddF197001Data.CHECK_STAFF;
				f197001.ITEM_DESC_A = AddF197001Data.ITEM_DESC_A;
				f197001.ITEM_DESC_B = AddF197001Data.ITEM_DESC_B;
				f197001.ITEM_DESC_C = AddF197001Data.ITEM_DESC_C;

				var proxy = new wcf.P19WcfServiceClient();
				var wcf197001 = ExDataMapper.Map<F197001, wcf.F197001>(f197001);
				var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateF197001(wcf197001));
				result = wcfResult.IsSuccessed;
				//更新查詢條件.
				SearchF197001.LABEL_CODE = SelectF197001.LABEL_CODE;
				SearchF197001.ITEM_CODE = SelectF197001.ITEM_CODE;
				SearchF197001.VNR_CODE = SelectF197001.VNR_CODE;
			}
			return result;
		}
		#endregion Save

		private bool _isCheckedAll;

		public bool IsCheckedAll
		{
			get { return _isCheckedAll; }
			set
			{
				Set(() => IsCheckedAll, ref _isCheckedAll, value);
			}
		}


		private ICommand _checkAllCommand;

		/// <summary>
		/// Gets the CheckAllCommand.
		/// </summary>
		public ICommand CheckAllCommand
		{
			get
			{
				return _checkAllCommand
					?? (_checkAllCommand = new RelayCommand(
					() =>
					{
						foreach (var item in DgQueryDataF197001)
						{
							item.IsChecked = IsCheckedAll;
						}
					},
					() => DgQueryDataF197001 != null));
			}
		}

		private ICommand importExcelCommand;

		/// <summary>
		/// Gets the ImportExcelCommand.
		/// </summary>
		public ICommand ImportExcelCommand
		{
			get
			{
				return importExcelCommand
					?? (importExcelCommand = CreateBusyAsyncCommand(
					ImportExcelCommandExecute,
					() => LabelTypeItemQ != null));
			}
		}

		void ImportExcelCommandExecute(object o)
		{
			DgQueryDataF197001 = null;

			string fileName;
			if (!ShowFileDialog(out fileName))
				return;

			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(fileName, ref errorMeg);
			if (!string.IsNullOrEmpty(errorMeg))
			{
				ShowWarningMessage(errorMeg);
				return;
			}

			var query = from col in excelTable.AsEnumerable()
						select new F197001Data
						{
							GUP_CODE = _gupCode,
							CUST_CODE = _custCode,
							LABEL_CODE = SearchF197001.LABEL_CODE,
							LABEL_NAME = LabelTypeItemQ.LABEL_NAME,
							ITEM_CODE = Convert.ToString(col.ItemArray.ElementAtOrDefault(0)),
							VNR_CODE = Convert.ToString(col.ItemArray.ElementAtOrDefault(1)),
							Qty = Convert.ToInt32(col.ItemArray.ElementAtOrDefault(2)),
							WarrantyDate = DateTimeHelper.ConversionDate(Convert.ToString(col.ItemArray.ElementAtOrDefault(3))),
							WARRANTY = Convert.ToString(col.ItemArray.ElementAtOrDefault(4))
						};

			var wcfF197001Datas = query.MapCollection<F197001Data, wcf.F197001Data>().ToArray();
			var wcfF1970 = LabelTypeItemQ.Map<F1970, wcf.F1970>();

			var proxy = GetWcfProxy<wcf.P19WcfServiceClient>();
			wcfF197001Datas = proxy.RunWcfMethod(w => w.ParseImportF197001Data(wcfF1970, wcfF197001Datas));

			var result = proxy.RunWcfMethod(w => w.ValidateImportF197001Data(wcfF1970, wcfF197001Datas));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return;
			}

			DgQueryDataF197001 = wcfF197001Datas.MapCollection<wcf.F197001Data, F197001Data>().ToObservableCollection();
		}



		bool ShowFileDialog(out string fileName)
		{
			fileName = null;
			var dlg = new Microsoft.Win32.OpenFileDialog
			{
				DefaultExt = ".xls",
				Filter = "excel files (*.xls,*.xlsx)|*.xls*|csv files (*.csv)|*.csv"
			};

			if (dlg.ShowDialog() != true)
				return false;

			fileName = dlg.FileName;
			return true;
		}

		void ImportExcelCommandCompleted()
		{

		}
	}
}
