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
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using System.Data;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	/// <summary>
	/// 彙總方式
	/// </summary>
	public enum SummaryType
	{
		/// <summary>
		/// 依揀貨單
		/// </summary>
		PickOrder =0,
		/// <summary>
		/// 依出貨單
		/// </summary>
		WmsOrder = 1

	}
	public partial class P0501030000_ViewModel : InputViewModelBase
	{
		public P0501030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				GetOrdTypeList();
				SetPickTime();
				SelectedSummaryType = SummaryType.PickOrder;
			}

		}

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
		private NameValuePair<string> _selectedDcCode;
		public NameValuePair<string> SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				Set(() => SelectedDcCode, ref _selectedDcCode, value);
				if (value != null)
					SetPickTime();
			}
		}

		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (DcList.Any())
				SelectedDcCode = DcList.FirstOrDefault();
		}
		#endregion

		#region 批次日期

		private DateTime _delvDate =DateTime.Today;

		public DateTime DelvDate
		{
			get { return _delvDate; }
			set
			{
				Set(() => DelvDate, ref _delvDate, value);
				if (value != null)
					SetPickTime();
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
				Set(() => PickTimeList, ref _pickTimeList, value);

			}
		}
		private NameValuePair<string> _selectedPickTime;
		public NameValuePair<string> SelectedPickTime
		{
			get { return _selectedPickTime; }
			set
			{
				Set(() => SelectedPickTime, ref _selectedPickTime, value);
			}
		}

		public void SetPickTime()
		{
			ClearGrid();
			//string dcCode, string gupCode, string custCode, string delvDate
			if (SelectedDcCode==null || SelectOrdType==null)
			{
				PickTimeList = null;
				SelectedPickTime = null;
				return;
			}
			var proxyEx = GetExProxy<P05ExDataSource>();
			var data = proxyEx.CreateQuery<P050103PickTime>("GetPickTimes")
				.AddQueryExOption("dcCode", _selectedDcCode.Value)
				.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
				.AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
				.AddQueryExOption("ordType", SelectOrdType.Value)
				.AddQueryExOption("delvDate", DelvDate)
				.ToList();
			if (data.Any())
			{
				var dataList = (from a in data
								select new NameValuePair<string>()
								{
									Value = a.PICK_TIME,
									Name = a.PICK_TIME
								}).ToList();
				dataList.Insert(0,new NameValuePair<string>()
				{
					Value = "",
					Name = Resources.Resources.All
				});
				PickTimeList = dataList.ToList();
				SelectedPickTime = PickTimeList.FirstOrDefault();
			}
			else
			{
				PickTimeList = null;
				SelectedPickTime = null;
			}
		}
		#endregion

		#region 訂單類型
		private List<NameValuePair<string>> _ordtypelist;

		public List<NameValuePair<string>> OrdTypeList
		{
			get { return _ordtypelist; }
			set
			{
				_ordtypelist = value;
				RaisePropertyChanged("OrdTypeList");
			}
		}
		private NameValuePair<string> _selectOrdType;
		public NameValuePair<string> SelectOrdType
		{
			get { return _selectOrdType; }
			set
			{
				Set(() => SelectOrdType, ref _selectOrdType, value);
				if(value!=null)
					SetPickTime();
			}
		}
		public void GetOrdTypeList()
		{
			OrdTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "ORD_TYPE");
			SelectOrdType = OrdTypeList.FirstOrDefault();
		}
		#endregion


		#region 選取的彙總方式
		private SummaryType _selectedSummaryType;

		public SummaryType SelectedSummaryType
		{
			get { return _selectedSummaryType; }
			set
			{
				Set(() => SelectedSummaryType, ref _selectedSummaryType, value);
				ClearGrid();
			}
		}
		#endregion

		#region 是否全選

		private bool _isCheckAllPick;

		public bool IsCheckAllPick
		{
			get { return _isCheckAllPick; }
			set
			{
				Set(() => IsCheckAllPick, ref _isCheckAllPick, value);
				CheckSelectedAll(value);
			}
		}

		private bool _isCheckAllWms;

		public bool IsCheckAllWms
		{
			get { return _isCheckAllWms; }
			set
			{
				Set(() => IsCheckAllWms, ref _isCheckAllWms, value);
				CheckSelectedAll(value);
			}
		}
		#endregion

		#region Grid 全選

		public void CheckSelectedAll(bool isChecked)
		{
			switch(SelectedSummaryType)
			{
				case SummaryType.PickOrder:
					if(P050103PickOrdNoList!=null)
						foreach (var item in P050103PickOrdNoList)
							item.IsSelected = isChecked;
					break;
				case SummaryType.WmsOrder:
					if (P050103WmsOrdNoList != null)
						foreach (var item in P050103WmsOrdNoList)
							item.IsSelected = isChecked;
					break;
			}
		}

		#endregion

		#region ClearGrid
		private void ClearGrid()
		{
			P050103PickOrdNoList = null;
			P050103WmsOrdNoList = null;
		}
		#endregion
		#region SearchCommand

		#region 揀貨單
		private SelectionList<P050103PickOrdNo> _p050103PickOrdNoList;

		public SelectionList<P050103PickOrdNo> P050103PickOrdNoList
		{
			get { return _p050103PickOrdNoList; }
			set
			{
				Set(() => P050103PickOrdNoList, ref _p050103PickOrdNoList, value);
			}
		}

		private SelectionItem<P050103PickOrdNo> _selectedP050103PickOrdNo;

		public SelectionItem<P050103PickOrdNo> SelectedP050103PickOrdNo
		{
			get { return _selectedP050103PickOrdNo; }
			set
			{
				Set(() => SelectedP050103PickOrdNo, ref _selectedP050103PickOrdNo, value);
			}
		}

		#endregion

		#region 出貨單

		private SelectionList<P050103WmsOrdNo> _p050103WmsOrdNoList;

		public SelectionList<P050103WmsOrdNo> P050103WmsOrdNoList
		{
			get { return _p050103WmsOrdNoList; }
			set
			{
				Set(() => P050103WmsOrdNoList, ref _p050103WmsOrdNoList, value);
			}
		}

		private SelectionItem<P050103WmsOrdNo> _selectedP050103WmsOrdNo;

		public SelectionItem<P050103WmsOrdNo> SelectedP050103WmsOrdNo
		{
			get { return _selectedP050103WmsOrdNo; }
			set
			{
				Set(() => SelectedP050103WmsOrdNo, ref _selectedP050103WmsOrdNo, value);
			}
		}
		#endregion

		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					c => DoSearchComplete()
					);
			}
		}
		
		private void DoSearch()
		{
			var proxyEx = GetExProxy<P05ExDataSource>();

			if (SelectedPickTime == null)
			{
				ClearGrid();
				return;
			}
			switch (SelectedSummaryType)
			{
				case SummaryType.PickOrder:
					var data = proxyEx.CreateQuery<P050103PickOrdNo>("GetPickOrderNos")
										.AddQueryExOption("dcCode", _selectedDcCode.Value)
										.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
										.AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
										 .AddQueryExOption("ordType", SelectOrdType.Value)
										.AddQueryExOption("delvDate", DelvDate)
										.AddQueryExOption("pickTime", SelectedPickTime.Value)
									 .ToList();
					P050103PickOrdNoList = data.ToSelectionList();

					break;
				case SummaryType.WmsOrder:
					var data1 = proxyEx.CreateQuery<P050103WmsOrdNo>("GetWmsOrderNos")
										.AddQueryExOption("dcCode", _selectedDcCode.Value)
										.AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
										.AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
										 .AddQueryExOption("ordType", SelectOrdType.Value)
										.AddQueryExOption("delvDate", DelvDate)
										.AddQueryExOption("pickTime", SelectedPickTime.Value)
									 .ToList();
					P050103WmsOrdNoList = data1.ToSelectionList();
					break;
			}
			//執行查詢
			

		}

		private void DoSearchComplete()
		{
			if (P050103PickOrdNoList != null && P050103PickOrdNoList.Any())
			{
				SelectedP050103PickOrdNo = P050103PickOrdNoList.First();
				IsCheckAllPick = false;
			}
			else if (P050103WmsOrdNoList != null && P050103WmsOrdNoList.Any())
			{
				SelectedP050103WmsOrdNo = P050103WmsOrdNoList.First();
				IsCheckAllWms = false;
			}
			else
				ShowMessage(Messages.InfoNoData);
		}
		#endregion

		#region PrintCommand

		public Action<PrintType, DataTable> OnPrintAction = delegate { };

		public ICommand PrintCommand
		{
			get
			{
				return CreateBusyAsyncCommand<PrintType>(
					o => DoPrint(),
					o=> UserOperateMode == OperateMode.Query && ((SelectedSummaryType == SummaryType.PickOrder && P050103PickOrdNoList!=null && P050103PickOrdNoList.Any(x=>x.IsSelected)) || (SelectedSummaryType == SummaryType.WmsOrder && P050103WmsOrdNoList!=null && P050103WmsOrdNoList.Any(x=> x.IsSelected))),
					o => {
						if (P050103ReportDataList != null && P050103ReportDataList.Any()) OnPrintAction(o, P050103ReportDataList.ToDataTable()); }
					);
			}
		}
		private List<P050103ReportData> _p050103ReportDataList;

		public List<P050103ReportData> P050103ReportDataList
		{
			get { return _p050103ReportDataList; }
			set
			{
				Set(() => P050103ReportDataList, ref _p050103ReportDataList, value);
			}
		}
		private bool DoPrint()
		{
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Message = "",
				Title = Resources.Resources.Warning
			};

			string pickOrdNo = "";
			string wmsOrdNo = "";
			switch (SelectedSummaryType)
			{
				case SummaryType.PickOrder:
					if (P050103PickOrdNoList != null && P050103PickOrdNoList.Any())
						pickOrdNo = string.Join(",", P050103PickOrdNoList.Where(o => o.IsSelected).Select(o => o.Item.PICK_ORD_NO).ToArray());
					break;
				case SummaryType.WmsOrder:
					if (P050103WmsOrdNoList != null && P050103WmsOrdNoList.Any())
					{
						pickOrdNo = string.Join(",", P050103WmsOrdNoList.Where(o => o.IsSelected).Select(o => o.Item.PICK_ORD_NO).Distinct().ToArray());
						wmsOrdNo = string.Join(",", P050103WmsOrdNoList.Where(o => o.IsSelected).Select(o => o.Item.WMS_ORD_NO).ToArray());
					}
					break;
			}

			if (string.IsNullOrEmpty(pickOrdNo))
				message.Message = Properties.Resources.P0501010000_NotSelectData;
			
			if (message.Message.Any())
			{
				ShowMessage(message);
				return false;
			}
			PrintReport(pickOrdNo,wmsOrdNo);

			return true;
		}

		public void PrintReport(string pickOrdNos,string wmsOrdNos)
		{
			var proxyEx = GetExProxy<P05ExDataSource>();
			var data = proxyEx.CreateQuery<P050103ReportData>("GetSummaryReport")
							  .AddQueryExOption("dcCode", _selectedDcCode.Value)
							  .AddQueryExOption("gupCode", Wms3plSession.Get<GlobalInfo>().GupCode)
							  .AddQueryExOption("custCode", Wms3plSession.Get<GlobalInfo>().CustCode)
								.AddQueryExOption("ordType", SelectOrdType.Value)
								.AddQueryExOption("delvDate", DelvDate)
							  .AddQueryExOption("pickOrdNo", pickOrdNos)
								.AddQueryExOption("wmsOrdNo",wmsOrdNos)
								.ToList();
			foreach(var item in data)
			{
				item.PICK_ORD_NO = pickOrdNos;
				item.ITEM_CODE_BARCODE = BarcodeConverter128.StringToBarcode(item.ITEM_CODE);
				item.PICK_LOC_BARCODE = BarcodeConverter128.StringToBarcode(item.PICK_LOC);
			}
			P050103ReportDataList = data;
			
		}
		
		#endregion

	}
}
