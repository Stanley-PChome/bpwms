using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;


namespace Wms3pl.WpfClient.P08.ViewModel
{

	public class P0814010100_ViewModel : InputViewModelBase
	{
		private string _gupCode;
		private string _custCode;
		public Action Leave = delegate { };

		public P0814010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			ClientIp = Wms3plSession.Get<GlobalInfo>().ClientIp;
		}
		#region Property
		/// <summary>
		/// 物流中心編號
		/// </summary>
		private string _dcCode;
		public string DcCode
		{
			get { return _dcCode; }
			set { Set(() => DcCode, ref _dcCode, value); }
		}
		public string GupCode;
		public string CustCode;
		public string ClientIp;

		/// <summary>
		/// 出貨模式(1:單人包裝站 2:包裝線包裝站) 
		/// </summary>
		private string _shippingMode;
		public string ShippingMode
		{
			get { return _shippingMode; }
			set { Set(() => ShippingMode, ref _shippingMode, value); }
		}

        /// <summary>
        /// 是否有作業中的單據
        /// </summary>
        private bool? _hasWorkingOrd;
        public bool? HasWorkingOrd
        {
            get { return _hasWorkingOrd; }
            set { Set(() => HasWorkingOrd, ref _hasWorkingOrd, value); }
        }

        /// <summary>
        /// 是否配箱站與封箱站分開
        /// </summary>
        private string _noSpecReprots;
		public string NoSpecReprots
		{
			get { return _noSpecReprots; }
			set { Set(() => NoSpecReprots, ref _noSpecReprots, value); }
		}

		/// <summary>
		/// 工作站編號
		/// </summary>
		private string _workstationCode;
		public string WorkstationCode
		{
			get { return _workstationCode; }
			set { Set(() => WorkstationCode, ref _workstationCode, value); }
		}
		//工作站主檔
		private F1946 _1946Data;
		public F1946 F1946Data
		{
			get { return _1946Data; }
			set { Set(() => F1946Data, ref _1946Data, value); }
		}

		/// <summary>
		/// 工作站類型
		/// </summary>
		private string _workstationType;
		public string WorkstationType
		{
			get { return _workstationType; }
			set { Set(() => WorkstationType, ref _workstationType, value); }
		}
		// 取得工作站類型名稱
		private string _workstationTypeName;
		public string WorkstationTypeName
		{
			get { return _workstationTypeName; }
			set { Set(() => WorkstationTypeName, ref _workstationTypeName, value); }
		}
		/// <summary>
		/// 工作站狀態
		/// </summary>
		private string _status;
		public string Status
		{
			get { return _status; }
			set { Set(() => Status, ref _status, value); }
		}
		/// <summary>
		/// 工作站狀態名稱
		/// </summary>
		private string _statusName;
		public string StatusName
		{
			get { return _statusName; }
			set { Set(() => StatusName, ref _statusName, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		private string _wmsOrdNo;
		public string WmsOrdNo
		{
			get { return _wmsOrdNo; }
			set { Set(() => WmsOrdNo, ref _wmsOrdNo, value); }
		}

		private DateTime _delvDate;
		public DateTime DelvDate
		{
			get { return _delvDate; }
			set { Set(() => DelvDate, ref _delvDate, value); }
		}

		private string _pickTime;
		public string PickTime
		{
			get { return _pickTime; }
			set { Set(() => PickTime, ref _pickTime, value); }
		}

		private string _barCode;
		public string BarCode
		{
			get { return _barCode; }
			set { Set(() => BarCode, ref _barCode, value); }
		}

		private int _qty;
		public int Qty
		{
			get { return _qty; }
			set { Set(() => Qty, ref _qty, value); }
		}

		private string _ordType;
		public string OrdType
		{
			get { return _ordType; }
			set { Set(() => OrdType, ref _ordType, value); }
		}

		private string _packageBoxNo;
		public string PackageBoxNo
		{
			get { return _packageBoxNo; }
			set { Set(() => PackageBoxNo, ref _packageBoxNo, value); }
		}

		/// <summary>
		/// 配箱與封箱站分開
		/// </summary>
		private string _noSpecReports;
		public string NoSpecReports
		{
			get { return _noSpecReports; }
			set { Set(() => NoSpecReports, ref _noSpecReports, value); }
		}

		/// <summary>
		/// 需刷讀紙箱條碼關箱
		/// </summary>
		private string _closeByBoxno;
		public string CloseByBoxno
		{
			get { return _closeByBoxno; }
			set { Set(() => CloseByBoxno, ref _closeByBoxno, value); }
		}

		/// <summary>
		/// 已分配訂單數
		/// </summary>
		private int _waitWmsOrderCnt;
		public int WaitWmsOrderCnt
		{
			get { return _waitWmsOrderCnt; }
			set { Set(() => WaitWmsOrderCnt, ref _waitWmsOrderCnt, value); }
		}

		#endregion

		#region PropertyVisibility
		/// <summary>
		/// 開站按鈕顯示
		/// </summary>
		private Visibility _openStationVisibility;
		public Visibility OpenStationVisibility
		{
			get { return _openStationVisibility; }
			set
			{
				Set(() => OpenStationVisibility, ref _openStationVisibility, value);
			}
		}
		/// <summary>
		/// 關站按鈕顯示
		/// </summary>
		private Visibility _closeStationVisibility;
		public Visibility CloseStationVisibility
		{
			get { return _closeStationVisibility; }
			set
			{
				Set(() => CloseStationVisibility, ref _closeStationVisibility, value);
			}
		}
		/// <summary>
		/// 暫停按鈕顯示
		/// </summary>
		private Visibility _pauseVisibility;
		public Visibility PauseVisibility
		{
			get { return _pauseVisibility; }
			set
			{
				Set(() => PauseVisibility, ref _pauseVisibility, value);
			}
		}
		/// <summary>
		/// 繼續按鈕顯示
		/// </summary>
		private Visibility _continueVisibility;
		public Visibility ContinueVisibility
		{
			get { return _continueVisibility; }
			set
			{
				Set(() => ContinueVisibility, ref _continueVisibility, value);
			}
		}
		/// <summary>
		/// 離開按鈕顯示
		/// </summary>
		private Visibility _leaveVisibility;
		public Visibility LeaveVisibility
		{
			get { return _leaveVisibility; }
			set { Set(() => LeaveVisibility, ref _leaveVisibility, value); }
		}

		/// <summary>
		/// 已分配訂單數顯示
		/// </summary>
		private Visibility _waitWmsOrderCntVisibility;
		public Visibility WaitWmsOrderCntVisibility
		{
			get { return _waitWmsOrderCntVisibility; }
			set { Set(() => WaitWmsOrderCntVisibility, ref _waitWmsOrderCntVisibility, value); }
		}
		#endregion

		#region PropetyEnable

		/// <summary>
		/// 配箱站與封箱站分開Enable
		/// </summary>
		private bool _noSpecReprotsEnable;
		public bool NoSpecReprotsEnable
		{
			get { return _noSpecReprotsEnable; }
			set
			{
				Set(() => NoSpecReprotsEnable, ref _noSpecReprotsEnable, value);
				//NoSpecReprotsEnable = ShippingMode == "1" ? true : false;
			}
		}
		/// <summary>
		/// 需刷讀紙箱條碼關箱
		/// </summary>
		private bool _closeByBoxNoEnable;
		public bool CloseByBoxNoEnable
		{
			get { return _closeByBoxNoEnable; }
			set
			{
				Set(() => CloseByBoxNoEnable, ref _closeByBoxNoEnable, value);
			}
		}

		private bool _pauseEnable;
		public bool PauseEnable
		{
			get { return _pauseEnable; }
			set
			{
				Set(() => PauseEnable, ref _pauseEnable, value);
			}
		}
		#endregion

		#region ICommand
		/// <summary>
		/// 開站Command
		/// </summary>
		public ICommand OpenStationCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoOpenStation(), () => true
					);
			}
		}

		/// <summary>
		/// 關站Command
		/// </summary>
		public ICommand CloseStationCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCloseStation(), () => true
					);
			}
		}

		/// <summary>
		/// 暫停Command
		/// </summary>
		public ICommand PauseStationCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoPauseStation(), () => true
					);
			}
		}

		/// <summary>
		/// 繼續Command
		/// </summary>
		public ICommand ContinueCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoContinue(), () => true
					);
			}
		}

		/// <summary>
		/// 離開Command
		/// </summary>
		public ICommand LeaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoLeave(), () => true
					);
			}
		}
		#endregion

		#region Math
		//取得配箱與封箱站分開
		//取得需刷讀紙箱條碼關箱
		//取得工作站主檔
		//取得工作站編號
		public void GetDeviceSetting()
		{
			var proxyF91 = GetProxy<F91Entities>();
			var proxyF19 = GetProxy<F19Entities>();
			var f190501 = proxyF91.F910501s.Where(x => x.DC_CODE == DcCode && x.DEVICE_IP == ClientIp).FirstOrDefault();
			F1946Data = proxyF19.F1946s.Where(x => x.DC_CODE == DcCode && x.WORKSTATION_CODE == f190501.WORKSTATION_CODE).FirstOrDefault();
			WorkstationCode = F1946Data?.WORKSTATION_CODE;
      WorkstationType = F1946Data?.WORKSTATION_TYPE;
			Status = F1946Data?.STATUS;
			NoSpecReports = ShippingMode == "2" ? "1" : f190501.NO_SPEC_REPROTS;
			CloseByBoxno = f190501.CLOSE_BY_BOXNO;
			GetStatusName();
			GetWorkstationType();
		}

		// 取得工作站狀態名稱
		public void GetStatusName()
		{
			StatusName = GetBaseTableService.GetF000904List(FunctionCode, "F1946", "STATUS").Where(x => x.Value == F1946Data?.STATUS).FirstOrDefault()?.Name;
		}

		//取得工作站類型
		public void GetWorkstationType()
		{
			WorkstationTypeName = GetBaseTableService.GetF000904List(FunctionCode, "F1946", "TYPE").Where(x => x.Value == F1946Data?.WORKSTATION_TYPE).FirstOrDefault()?.Name;
    }

		//開站
		public void DoOpenStation()
		{
			var proxy = new wcf.P08WcfServiceClient();
			if (ShippingMode == "2")
			{
				var setPackageLineStationStatusResult = SetPackageLineStationStatus(DcCode, GupCode, CustCode, WorkstationCode, WorkstationType,"1", NoSpecReports, CloseByBoxno, ClientIp);
				if (!setPackageLineStationStatusResult) return;
			}
			if(ShippingMode == "1")
			{
				var setPackageStationStatusLogResult = SetPackageStationStatusLog(DcCode, WorkstationCode, "1", ShippingMode, NoSpecReports, CloseByBoxno, ClientIp);
				if (!setPackageStationStatusLogResult) return;
			}
			
			LeaveCommand.Execute(null);
		}

		//關站
		public void DoCloseStation()
		{
			var proxy = new wcf.P08WcfServiceClient();
			if (ShippingMode == "2")
			{
				var setPackageLineStationStatusResult = SetPackageLineStationStatus(DcCode, GupCode, CustCode, WorkstationCode, WorkstationType, "0", NoSpecReports, CloseByBoxno, ClientIp);
				if (!setPackageLineStationStatusResult) return;
				
				if (Convert.ToBoolean(HasWorkingOrd))
				{
					ShowInfoMessage("您尚有未完成單據，請完成此單據包裝後才可以來離開工作站");
				}
			}

			if(ShippingMode == "1")
			{
				var setPackageStationStatusLogResult = SetPackageStationStatusLog(DcCode, WorkstationCode, "0", ShippingMode, NoSpecReports, CloseByBoxno, ClientIp);
				if (!setPackageStationStatusLogResult) return;
			}
			LeaveCommand.Execute(null);
			
		}

		//暫停
		public void DoPauseStation()
		{
			var setPackageLineStationStatusResult = SetPackageLineStationStatus(DcCode, GupCode, CustCode, WorkstationCode, WorkstationType, "2", NoSpecReports, CloseByBoxno, ClientIp);
			if (!setPackageLineStationStatusResult) return;
			LeaveCommand.Execute(null);
		}

		// 繼續
		public void DoContinue()
		{
			var setPackageLineStationStatusResult = SetPackageLineStationStatus(DcCode, GupCode, CustCode, WorkstationCode, WorkstationType, "1", NoSpecReports, CloseByBoxno, ClientIp);
			if (!setPackageLineStationStatusResult) return;
			LeaveCommand.Execute(null);
		}

		//離開
		public void DoLeave()
		{
			DispatcherAction(() =>
			{
				Leave();
			});
		}

		//初始化按鈕
		public void InitButten()
		{
			//[開站]
			OpenStationVisibility = F1946Data?.STATUS == "0" ? Visibility.Visible : Visibility.Collapsed;
			//[關站]
			CloseStationVisibility = F1946Data?.STATUS == "1" || F1946Data?.STATUS == "2" ? Visibility.Visible : Visibility.Collapsed;
			//[暫停]
            PauseVisibility = ShippingMode == "2" && CloseStationVisibility == Visibility.Visible && F1946Data?.STATUS != "2" ? Visibility.Visible : Visibility.Collapsed;
            //[繼續]
            ContinueVisibility = ShippingMode == "2" && CloseStationVisibility == Visibility.Visible && F1946Data?.STATUS == "2" ? Visibility.Visible : Visibility.Collapsed;

            // 如果F1946.STATUS = 0(關站)，[配箱與封箱站分開] 、[需刷讀紙箱條碼關箱] checkbox enabled else disabled 
            if (ShippingMode == "1")
            {
                NoSpecReprotsEnable = F1946Data?.STATUS == "0" ? true : false;
            }
            else
            {
                NoSpecReprotsEnable = false;

            }

            CloseByBoxNoEnable = F1946Data?.STATUS == "0" ? true : false;
        }

		/// <summary>
		/// 取得已分配訂單數
		/// </summary>
		public void GetWorkStataionShipData()
		{
			var proxy = new wcf.P08WcfServiceClient();
			WaitWmsOrderCnt = RunWcfMethod<wcf.GetWorkStataionShipDataRes>(proxy.InnerChannel, () => proxy.GetWorkStataionShipData(new GetWorkStataionShipDataReq
			{
				DcCode = DcCode,
				workstationCode = WorkstationCode
			})).WaitContainerCnt;
		}

		// 單人包裝站開站/關站紀錄
		private bool SetPackageStationStatusLog(string dcCode, string workstationCode, string status, string workType, string noSpecReports, string closeByBoxno, string deviceIp)
		{
			var proxy = new wcf.P08WcfServiceClient();
			var result = true;
			var setPackageStationStatusLogResult = RunWcfMethod<wcf.SetPackageStationStatusLogRes>(proxy.InnerChannel, () => proxy.SetPackageStationStatusLog(new SetPackageStationStatusLogReq
			{
				DcCode = dcCode,
				WorkstationCode = workstationCode,
				Status = status,
				WorkType = workType,
				NoSpecReport = noSpecReports,
				CloseByBoxNo = closeByBoxno,
				DeviceIp = deviceIp
			}));
			if (!setPackageStationStatusLogResult.IsSuccessed)
			{
				DialogService.ShowMessage(setPackageStationStatusLogResult.Message);
				result = false;
			}
			return result;
		}

		// 包裝線自動設備開站/暫停/關站
		private bool SetPackageLineStationStatus(string dcCode,string gupCode,string custCode,string workstationCode,string workstationType,string status,string noSpecReports,string closeByBoxno,string deviceIp)
		{
			var proxy = new wcf.P08WcfServiceClient();
			var result = true;
			var setPackageLineStationStatusResult = RunWcfMethod<wcf.SetPackageLineStationStatusRes>(proxy.InnerChannel, () => proxy.SetPackageLineStationStatus(new SetPackageLineStationStatusReq
			{
				DcCode = dcCode,
				GupCode = gupCode,
				CustCode = custCode,
				WorkstationCode = workstationCode,
				WorkstationType = workstationType,
				Status = status,
				NoSpecReports = noSpecReports,
				CloseByBoxno = closeByBoxno,
				DeviceIp = deviceIp,
                HasUndone = Convert.ToBoolean(HasWorkingOrd)
            }));
			//return result;
			if (!setPackageLineStationStatusResult.IsSuccessed)
			{
				DialogService.ShowMessage(setPackageLineStationStatusResult.Message);
				result = false;
			}
			return result;
		}
		#endregion
	}
}
