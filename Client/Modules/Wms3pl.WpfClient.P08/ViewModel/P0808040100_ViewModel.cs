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
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0808040100_ViewModel : InputViewModelBase
	{

		#region Property

		private readonly string _userId;
		private readonly string _userName;
		private DeliveryReportService _deliveryReport;
		public Action DoDcChange = delegate { };

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
				RaisePropertyChanged("DcList");
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
				if (value != null)
					DoDcChange();

				SetPickTimeList();
			}
		}
		#endregion

		#region 批次日期
		private DateTime? _delvDate;

		public DateTime? DelvDate
		{
			get { return _delvDate; }
			set
			{
				_delvDate = value;
				RaisePropertyChanged("DelvDate");
				SetPickTimeList();
			}
		}
		#endregion

		#region 批次時段
		private List<NameValuePair<string>> _pickTimeList;

		public List<NameValuePair<string>> PickTimeList
		{
			get { return _pickTimeList; }
			set
			{
				_pickTimeList = value;
				RaisePropertyChanged("PickTimeList");
			}
		}

		private string _SelectPickTime;

		public string SelectPickTime
		{
			get { return _SelectPickTime; }
			set
			{
				_SelectPickTime = value;
				RaisePropertyChanged("SelectPickTime");
			}
		}
		#endregion

		#region 分貨類型
		private List<NameValuePair<string>> _sowTypeList;
		public List<NameValuePair<string>> SowTypeList
		{
			get { return _sowTypeList; }
			set
			{
				_sowTypeList = value;
				RaisePropertyChanged("SowTypeList");
			}
		}

		private string _selectSowType;

		public string SelectSowType
		{
			get { return _selectSowType; }
			set
			{
				_selectSowType = value;
				RaisePropertyChanged("SelectSowType");
			}
		}
		#endregion

		#region 箱號狀態
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

		private string _selectStatus;

		public string SelectStatus
		{
			get { return _selectStatus; }
			set
			{
				_selectStatus = value;
				RaisePropertyChanged("SelectStatus");
			}
		}
		#endregion

		#region 箱號
		private List<NameValuePair<string>> _boxNoList;
		public List<NameValuePair<string>> BoxNoList
		{
			get { return _boxNoList; }
			set
			{
				_boxNoList = value;
				RaisePropertyChanged("BoxNoList");
			}
		}

		private string _selectBoxNo;

		public string SelectBoxNo
		{
			get { return _selectBoxNo; }
			set
			{
				_selectBoxNo = value;
				RaisePropertyChanged("SelectBoxNo");

				if (!string.IsNullOrWhiteSpace(_selectBoxNo))
				{
					BoxData = BoxDataList.Where(x => x.BOX_NUM == _selectBoxNo).First();
					DoSearchDetail();
				}
			}
		}
		#endregion

		#region Grid資料繫結資料

		private P0808040100_BoxData _boxData;

		public P0808040100_BoxData BoxData
		{
			get { return _boxData; }
			set
			{
				_boxData = value;
				RaisePropertyChanged("BoxData");
			}
		}

		private List<P0808040100_BoxData> _boxDataList;

		public List<P0808040100_BoxData> BoxDataList
		{
			get { return _boxDataList; }
			set
			{
				_boxDataList = value;
			}
		}

		private P0808040100_BoxDetailData _selectedData;

		public P0808040100_BoxDetailData SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");

			}
		}

		private List<P0808040100_BoxDetailData> _boxDetailData;

		public List<P0808040100_BoxDetailData> BoxDetailData
		{
			get { return _boxDetailData; }
			set
			{
				_boxDetailData = value;
				RaisePropertyChanged("BoxDetailData");
			}
		}
		#endregion

		#endregion

		#region 下拉選單資料繫結
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any() && string.IsNullOrWhiteSpace(SelectDcCode))
				SelectDcCode = DcList.First().Value;
		}

		private void SetPickTimeList()
		{
			var proxy = GetProxy<F05Entities>();
			var data = proxy.F052905s.Where(o =>
			o.DC_CODE == SelectDcCode &&
			o.GUP_CODE == GupCode &&
			o.CUST_CODE == CustCode &&
			o.DELV_DATE == DelvDate).ToList();

			var list = data.GroupBy(x => x.PICK_TIME).Select(o => new NameValuePair<string>
			{
				Name = o.Key,
				Value = o.Key
			}).OrderBy(x=>x.Name).ToList();

			PickTimeList = list;

			if (PickTimeList != null && PickTimeList.Any() && string.IsNullOrWhiteSpace(SelectPickTime))
				SelectPickTime = PickTimeList.First().Value;
			else
				SelectPickTime = null;
		}

		private void SetSowTypeList()
		{
			SowTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F052905", "SOW_TYPE", true);
			SelectSowType = SowTypeList.First().Value;
		}

		private void SetStatusList()
		{
			StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F052905", "STATUS", true);
			SelectStatus = StatusList.First().Value;
		}
		#endregion

		public P0808040100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_deliveryReport = new DeliveryReportService(FunctionCode);

				SetDcList();
				SetSowTypeList();
				SetStatusList();

				if (DelvDate == null)
					DelvDate = DateTime.Today;
			}
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				o => DoSearch(),
				() => true,
				c => DoSearchComplete());
			}
		}

		private string message { get; set; }
		private void DoSearch()
		{
			message = string.Empty;

			if (string.IsNullOrWhiteSpace(SelectDcCode))
				message = string.Format(Properties.Resources.P0808040100_ShowErrorMsg, Properties.Resources.P0808040100_Dc);
			else if (DelvDate == null)
				message = string.Format(Properties.Resources.P0808040100_ShowErrorMsg, Properties.Resources.P0808040100_DelvDate);
			else if (string.IsNullOrWhiteSpace(SelectPickTime))
				message = string.Format(Properties.Resources.P0808040100_ShowErrorMsg, Properties.Resources.P0808040100_PickTime);

			if (!string.IsNullOrEmpty(message))
			{
				ShowWarningMessage(message);
				return;
			}

			var proxyEx = GetExProxy<P08ExDataSource>();
			BoxDataList = proxyEx.CreateQuery<P0808040100_BoxData>("GetBoxData")
									.AddQueryExOption("dcCode", SelectDcCode)
									.AddQueryExOption("gupCode", GupCode)
									.AddQueryExOption("custCode", CustCode)
									.AddQueryExOption("delvDate", DelvDate)
									.AddQueryExOption("pickTime", SelectPickTime)
									.AddQueryExOption("sowType", SelectSowType)
									.AddQueryExOption("status", SelectStatus)
									.ToList();
		}

		private void DoSearchComplete()
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				if (BoxDataList != null && BoxDataList.Any())
				{
					BoxNoList = BoxDataList.Select(x => new NameValuePair<string> { Value = x.BOX_NUM, Name = x.BOX_NUM }).ToList();
					BoxData = BoxDataList.First();
					BoxDetailData = new List<P0808040100_BoxDetailData>();
					SelectBoxNo = BoxData.BOX_NUM;
				}
				else
				{
					BoxNoList = new List<NameValuePair<string>>();
					BoxData = new P0808040100_BoxData();
					BoxDetailData = new List<P0808040100_BoxDetailData>();
					SelectBoxNo = null;

					ShowMessage(Messages.InfoNoData);
				}
			}
		}

		private void DoSearchDetail()
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
			BoxDetailData = proxyEx.CreateQuery<P0808040100_BoxDetailData>("GetBoxDetailData")
									.AddQueryExOption("refId", BoxData.ID)
									.ToList();

			if (!BoxDetailData.Any())
				ShowWarningMessage(Properties.Resources.P0808040100_DetailNullMsg);
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
		}
		#endregion Cancel

		#region Print
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
					DoPrint,
					(t) => !IsBusy && UserOperateMode == OperateMode.Query && BoxDetailData != null && BoxDetailData.Any() && 
					(BoxData != null && (BoxData.STATUS == "1" || BoxData.STATUS == "2"))
					);
			}
		}

		private void DoPrint(PrintType printType)
		{
			var datas = _deliveryReport.GetBoxData(BoxData.DC_CODE,
					BoxData.GUP_CODE,
					BoxData.CUST_CODE,
					BoxData.DELV_DATE,
					BoxData.PICK_TIME,
					BoxData.MOVE_OUT_TARGET,
					BoxData.CONTAINER_CODE,
					BoxData.SOW_TYPE);

			_deliveryReport.PrintBoxData(
					BoxData.DC_CODE,
					BoxData.SOW_TYPE,
					datas);
		}
		#endregion Print
	}
}
