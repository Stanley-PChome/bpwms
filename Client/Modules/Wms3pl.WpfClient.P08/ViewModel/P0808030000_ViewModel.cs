using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0808030000_ViewModel : InputViewModelBase
	{
	
		private string _gupCode;
		private string _custCode;
		private List<NameValuePair<string>> _tmprTypeList;
		public Action OnSearchPastNoComplete = delegate { };
		public P0808030000_ViewModel()
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
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			_tmprTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1980", "TMPR_TYPE");
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
			{
				SelectedDc = DcList.First().Value;
			}
		}

		#region 物流中心清單
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				Set(() => DcList, ref _dcList, value);
			}
		}
		#endregion

		#region 選取的物流中心
		private string _selectedDc;

		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
        if (SelectedDc != value)
        {
          Result = null;
          PastNo = null;
          OnSearchPastNoComplete();
        }

				Set(() => SelectedDc, ref _selectedDc, value);
			}
		}
		#endregion

		#region 宅配單號
		private string _pastNo;
		public string PastNo
		{
			get { return _pastNo; }
			set
			{
				Set(() => PastNo, ref _pastNo, value);
			}
		}
		#endregion

		#region 溫度中文
		private string _tmprType;
		public string TmprType
		{
			get { return _tmprType; }
			set
			{
				Set(() => TmprType, ref _tmprType, value);
			}
		}
		#endregion

		#region 配送商名稱
		private string _transportProviderName;
		public string TransportProviderName
		{
			get { return _transportProviderName; }
			set
			{
				Set(() => TransportProviderName, ref _transportProviderName, value);
			}
		}
		#endregion

		#region 狀態顏色
		private string _statusColor = "";
		public string StatusColor
		{
			get { return _statusColor; }
			set { _statusColor = value; RaisePropertyChanged("StatusColor"); }
		}
		#endregion

		#region 出貨單狀態
		private string _status;
		public string Status
		{
			get { return _status; }
			set { _status = value; RaisePropertyChanged("Status"); }
		}
		#endregion

		#region
		public class ScanResult
		{
			public string wmsOrdNo { get; set; }
			public string scanResult { get; set; }
			public string scanTime { get; set; }
			public string All_Name { get; set; }
		}

		private string LastPastNo;

		/// <summary>
		/// 已刷讀通過筆數
		/// </summary>
		public int ScanCount
		{
			get { return result?.Count(x => x.scanResult == "OK") ?? 0; }
		}

		private List<ScanResult> result;
		public List<ScanResult> Result
		{
			get { return result; }
			set { result = value; RaisePropertyChanged("Result"); RaisePropertyChanged("ScanCount"); }
		}

		private Visibility _tmprTypeVisibility = Visibility.Hidden;
		public Visibility TmprTypeVisibility
		{
			get { return _tmprTypeVisibility; }
			set { _tmprTypeVisibility = value; RaisePropertyChanged("TmprTypeVisibility"); }
		}

		private Visibility _transportProviderNameVisibility = Visibility.Hidden;
		public Visibility TransportProviderNameVisibility
		{
			get { return _transportProviderNameVisibility; }
			set { _transportProviderNameVisibility = value; RaisePropertyChanged("TransportProviderNameVisibility"); }
		}

		#endregion

		#region 語音
		private bool _playSound = true;
		public bool PlaySound
		{
			get { return _playSound; }
			set { _playSound = value; RaisePropertyChanged("PlaySound"); }
		}
		#endregion


		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplite()
					);
			}
		}

		private void DoSearch()
		{
			TransportProviderName = string.Empty;
			TransportProviderNameVisibility = Visibility.Hidden;
			TmprType = string.Empty;
			TmprTypeVisibility = Visibility.Hidden;

			if (string.IsNullOrWhiteSpace(PastNo))
			{
				ShowInfoMessage("請刷讀宅配單號");
				return;
			}

			PastNo = PastNo.ToUpper();
			
			if (LastPastNo == PastNo)
			{
				ShowInfoMessage("宅配單號重複刷讀");
				return;
			}

			//執行查詢動作
			if (!string.IsNullOrWhiteSpace(PastNo))
			{
		
				PastNo = PastNo.ToUpper();
				Status = "";
				var proxyP08 = new wcf.P08WcfServiceClient();
				var result = RunWcfMethod<wcf.HomeDeliveryOrderDebitResult>(proxyP08.InnerChannel,
					() => proxyP08.HomeDeliveryOrderDebit(SelectedDc,_gupCode,_custCode, PastNo));

				if (!result.IsSuccessed)
				{
					//語音
					if (PlaySound)
						PlaySoundHelper.Oo();
					ShowWarningMessage(result.Message);
					TransportProviderName = string.Empty;
					TransportProviderNameVisibility = Visibility.Hidden;
					TmprType = string.Empty;
					TmprTypeVisibility = Visibility.Hidden;
					return;
				}

        LastPastNo = PastNo;

				//語音
				if (PlaySound)
					PlaySoundHelper.Scan();

				TmprType = _tmprTypeList.FirstOrDefault(x => x.Value == result.Data.TMPR_TYPE)?.Name;
				TransportProviderName = result.Data.LOGISTIC_NAME;
				TransportProviderNameVisibility = Visibility.Visible;
				TmprTypeVisibility = Visibility.Visible;
				Status = result.Message;
				StatusColor = "Green";
				var list = Result == null ? new List<ScanResult>() : Result;
				list.Insert(0, new ScanResult
				{
					wmsOrdNo = PastNo,
					scanResult = Status,
					scanTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
					All_Name = TransportProviderName,
				});
				Result = list.ToList();
			}
		}
		private void DoSearchComplite()
		{
			OnSearchPastNoComplete();
		}

	}
}
