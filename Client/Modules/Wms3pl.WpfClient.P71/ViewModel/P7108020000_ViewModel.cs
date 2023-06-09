using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.UILib;
using P710802SearchResult = Wms3pl.WpfClient.ExDataServices.P05ExDataService.P710802SearchResult;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7108020000_ViewModel : InputViewModelBase
	{
		public P7108020000_ViewModel()
		{
			Init();
		}

		private void Init()
		{
			//物流中心
			DcCodes = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);

			#region 單據類型
			ReceiptTypes = new List<NameValuePair<string>>();
			var proxyF00 = GetProxy<F00Entities>();

			foreach (var a in proxyF00.F000902s.OrderBy(x => x.SOURCE_TYPE))
			{
				switch (a.SOURCE_TYPE)
				{
					case "03":
					case "05":
					case "06":
					case "07":
					case "08":
					case "09":
					case "13":
					case "21":
					case "04":
					case "10":
					case "12":
					case "17":
					case "18":
					case "19":
					case "30":
            NameValuePair<string> item = new NameValuePair<string>(a.SOURCE_NAME, a.SOURCE_TYPE);
						ReceiptTypes.Add(item);
						break;
				}
			}

			NameValuePair<string> item00 = new NameValuePair<string>(Properties.Resources.P7108020000_ViewModel_Export, "00");
			NameValuePair<string> item99 = new NameValuePair<string>(Properties.Resources.P7108020000_AllotStock, "-1");
			NameValuePair<string> itemAll = new NameValuePair<string>(Resources.Resources.All, "");
			ReceiptTypes.Insert(0, item00);
			ReceiptTypes.Insert(0, item99);
			ReceiptTypes.Insert(0, itemAll);
			#endregion

			if (DcCodes.Count > 0) DcCode = DcCodes[0].Value;
			ChangeDateBegin = DateTime.Today;
			ChangeDateEnd = DateTime.Today;
			if (ReceiptTypes.Count > 0) ReceiptType = ReceiptTypes[0].Value;
		}
		/// <summary>
		/// 設定業主清單
		/// </summary>
		private void SetGupList()
		{
			var gupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(_dcCode);
			GupList = gupList;
			SelectedGupCode = gupList.First().Value;
		}

		/// <summary>
		/// 設定貨主清單
		/// </summary>
		private void SetCustList()
		{
			var custList = Wms3plSession.Get<GlobalInfo>().GetCustDataList(_dcCode, _selectedGupCode);
			CustList = custList;
			SelectedCustCode = custList.First().Value;
		}
		#region 查詢Command
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
			//檢查查詢條件
			if (!ChangeDateBegin.HasValue || !ChangeDateEnd.HasValue)
			{
				ShowWarningMessage(Properties.Resources.P7108020000_ViewModel_ModifyData_Required);
				return;
			}

			if (ChangeDateBegin.Value > ChangeDateEnd.Value)
			{
				DateTime temp1 = ChangeDateBegin.Value;
				ChangeDateBegin = ChangeDateEnd.Value;
				ChangeDateEnd = temp1;
			}

			
			//選擇全部時  三段資料 union
			if (string.IsNullOrEmpty(ReceiptType))
			{

				if (new TimeSpan(ChangeDateEnd.Value.Ticks - ChangeDateBegin.Value.Ticks).Days > 90)
				{
					DialogService.ShowMessage(Properties.Resources.P7108020000_ViewModel_QueryPeriod_InNintyDays);
					return;
				}
				var data = GetF710802Type("GetF710802Type1");
				data.AddRange(GetF710802Type("GetF710802Type2"));
				data.AddRange(GetF710802Type("GetF710802Type3"));
				SearchResults = data.ToList();
			}
			else if (ReceiptType == "00" || ReceiptType == "03" || ReceiptType == "05" ||
				ReceiptType == "06" || ReceiptType == "07" || ReceiptType == "08" ||
				ReceiptType == "09" || ReceiptType == "13" || ReceiptType == "-1")
			{
				SearchResults = GetF710802Type("GetF710802Type1");
			}
			else if (ReceiptType == "21" || ReceiptType == "04" || ReceiptType == "10" ||
           ReceiptType == "12" || ReceiptType == "17" || ReceiptType == "18" || ReceiptType == "30")
      {
				SearchResults = GetF710802Type("GetF710802Type2");
			}
			else
			{
				SearchResults = GetF710802Type("GetF710802Type3");
			}

      if (SearchResults == null || !SearchResults.Any())
			{
				ShowWarningMessage(Properties.Resources.P7108020000_ViewModel_NoData);
			}
      else
        SearchResults = SearchResults.OrderBy(x => x.PROC_TIME).ToList();
		}

		private List<P710802SearchResult> GetF710802Type(string methodName)
		{
			var proxy = GetExProxy<P05ExDataSource>();
			return proxy.CreateQuery<P710802SearchResult>(methodName)
							.AddQueryOption("gupCode", string.Format("'{0}'", SelectedGupCode))
							.AddQueryOption("custCode", string.Format("'{0}'", SelectedCustCode))
							.AddQueryOption("dcCode", string.Format("'{0}'", DcCode))
							.AddQueryOption("changeDateBegin", string.Format("'{0}'", ChangeDateBegin.Value.ToString("yyyy/MM/dd")))
							.AddQueryOption("changeDateEnd", string.Format("'{0}'", ChangeDateEnd.Value.ToString("yyyy/MM/dd")))
							.AddQueryOption("itemCode", string.Format("'{0}'", ItemCode))
							.AddQueryOption("itemName", string.Format("'{0}'", HttpUtility.UrlEncode(ItemName)))
							.AddQueryOption("receiptType", string.Format("'{0}'", ReceiptType))
							.AddQueryOption("makeNo", string.Format("'{0}'", MakeNo)).ToList();
		}






		private void DoSearchComplete()
		{			
			UserOperateMode = OperateMode.Query;
		}
		#endregion

		#region 資料來源
		//物流中心
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

		private List<NameValuePair<string>> _gupList;
		public List<NameValuePair<string>> GupList
		{
			get { return _gupList; }
			set
			{
				_gupList = value;
				RaisePropertyChanged("GupList");
			}
		}

		

		private List<NameValuePair<string>> _custList;
		public List<NameValuePair<string>> CustList
		{
			get { return _custList; }
			set
			{
				_custList = value;
				RaisePropertyChanged("CustList");
			}
		}

	

		//單據類別
		private List<NameValuePair<string>> _receiptTypes;

		public List<NameValuePair<string>> ReceiptTypes
		{
			get { return _receiptTypes; }
			set
			{
				_receiptTypes = value;
				RaisePropertyChanged("ReceiptTypes");
			}
		}

		private List<P710802SearchResult> _searchResults;

		public List<P710802SearchResult> SearchResults
		{
			get { return _searchResults; }
			set
			{
				_searchResults = value;
				RaisePropertyChanged("SearchResults");
			}
		}
		#endregion

		#region 查詢條件
		private string _dcCode = String.Empty;

		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				_dcCode = value;
				RaisePropertyChanged("DcCode");				
				SetGupList();
			}
		}

		private string _selectedGupCode = "";
		public string SelectedGupCode
		{
			get { return _selectedGupCode; }
			set
			{
				_selectedGupCode = value;
				RaisePropertyChanged("SelectedGupCode");
				SetCustList();
			}
		}
		private string _selectedCustCode = "";
		public string SelectedCustCode
		{
			get { return _selectedCustCode; }
			set
			{
				_selectedCustCode = value;
				RaisePropertyChanged("SelectedCustCode");
			}
		}
		private string _receiptType = String.Empty;

		public string ReceiptType
		{
			get { return _receiptType; }
			set
			{
				_receiptType = value;
				RaisePropertyChanged("ReceiptType");
			}
		}

		private DateTime? _changeDateBegin;

		public DateTime? ChangeDateBegin
		{
			get { return _changeDateBegin; }
			set
			{
				_changeDateBegin = value;
				RaisePropertyChanged("ChangeDateBegin");
			}
		}

		private DateTime? _changeDateEnd;

		public DateTime? ChangeDateEnd
		{
			get { return _changeDateEnd; }
			set
			{
				_changeDateEnd = value;
				RaisePropertyChanged("ChangeDateEnd");
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
                if (string.IsNullOrEmpty(value))
                    MakeNo = null;
                RaisePropertyChanged("MakeNoEnable");
            }
        }

        private string _MakeNo = String.Empty;

        public string MakeNo
        {
            get { return _MakeNo; }
            set
            {
                _MakeNo = value;
                RaisePropertyChanged();
            }
        }

        public Boolean MakeNoEnable
        {
            get { return !string.IsNullOrEmpty(ItemCode); } 
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
		#endregion




	}
}
