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
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0501020000_ViewModel : InputViewModelBase
	{
		public P0501020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetPrintStatus();
				SetTmprTypes();
        _userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				DelvDate = DateTime.Today;
			}

		}
		#region Property

		private readonly string _userId;
		private readonly string _userName;
        /// <summary>
		/// Device 的設定 (當物流中心改變時，就會去顯示設定 Device 的畫面)  
		/// </summary>
		public F910501 SelectedF910501 { get; set; }
        public Action OnDcCodeChanged = () => { };

        public Action<PrintType> DoPrintReport = delegate { };



		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}

		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		#region 開放/停用揀貨單補印

		private bool _isEnabledPickOrdNoPrint;

		public bool IsEnabledPickOrdNoPrint
		{
			get { return _isEnabledPickOrdNoPrint; }
			set
			{
				_isEnabledPickOrdNoPrint = value;
				RaisePropertyChanged("IsEnabledPickOrdNoPrint");
			}
		}

		#endregion

		#region 開放/停用台車分配揀貨單比例

		private bool _isEnabledShareCount;

		public bool IsEnabledShareCount
		{
			get { return _isEnabledShareCount; }
			set
			{
				_isEnabledShareCount = value;
				RaisePropertyChanged("IsEnabledShareCount");
			}
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

		private string _selectedDcCode = "";
		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
                //if (Set(() => SelectedDcCode, ref _selectedDcCode, value))
                //{
                //    OnDcCodeChanged();
                //    SetTarWarehouse();
                //}
                _selectedDcCode = value;
				RaisePropertyChanged("SelectedDcCode");
				F051201Datas = null;
				F051202Datas = null;
				F051201SelectedDatas = null;
				IsEnabledPickOrdNoPrint = false;
                OnDcCodeChanged();
            }
		}

		#endregion

		#region 批次日期

		private DateTime _delvDate;

		public DateTime DelvDate
		{
			get { return _delvDate; }
			set
			{
				_delvDate = value;
				RaisePropertyChanged("DelvDate");
			}
		}

		#endregion

		#region 列印狀態

		private List<NameValuePair<string>> _printStatusList;

		public List<NameValuePair<string>> PrintStatusList
		{
			get { return _printStatusList; }
			set
			{
				_printStatusList = value;
				RaisePropertyChanged("PrintStatusList");
			}
		}

		private string _selectedPrintStatus;

		public string SelectedPrintStatus
		{
			get { return _selectedPrintStatus; }
			set
			{
				_selectedPrintStatus = value;
				RaisePropertyChanged("SelectedPrintStatus");
			}
		}
        #endregion


    #region 揀貨單號

        private string _pickOrdNo;

		public string PickOrdNo
		{
			get { return _pickOrdNo; }
			set
			{
				_pickOrdNo = value;
				RaisePropertyChanged("PickOrdNo");
			}
		}

		#endregion

		#region 挑選批次 Grid List

		private List<F051201Data> _f051201Datas;

		public List<F051201Data> F051201Datas
		{
			get { return _f051201Datas; }
			set
			{
				_f051201Datas = value;
				RaisePropertyChanged("F051201Datas");
			}
		}

		private F051201Data _selectedF051201Data;

		public F051201Data SelectedF051201Data
		{
			get { return _selectedF051201Data; }
			set
			{
				_selectedF051201Data = value;
				RaisePropertyChanged("SelectedF051201Data");
				if (_selectedF051201Data != null && value != null)
				{
					// 這裡常常有非同步問題
					var proxy = GetProxy<F05Entities>();
					int dataCount =
							proxy.CreateQuery<F051201>("GetDatasByNoVirturlItem")
							.AddQueryExOption("dcCode", value.DC_CODE)
							.AddQueryExOption("gupCode", value.GUP_CODE)
							.AddQueryExOption("custCode", value.CUST_CODE)
							.AddQueryExOption("delvDate", value.DELV_DATE)
							.AddQueryExOption("pickTime", value.PICK_TIME)
							.Count();
					VirturalPickOrdQty = (value.PICKCOUNT - dataCount).ToString();
                    SearchF051202DataCommand.Execute(null);
					SearchF700101DataCommand.Execute(null);
					SearchF051201SelectedDataCommand.Execute(null);
				}
				else
				{
					F700101Datas = new List<F700101CarData>();
					F051202Datas = null;
					F051201SelectedDatas = null;
				}
			}
		}


		#endregion

		#region 揀貨單分頁明細 Grid List

		private List<F051202Data> _f051202Datas;

		public List<F051202Data> F051202Datas
		{
			get { return _f051202Datas; }
			set
			{
				_f051202Datas = value;
				RaisePropertyChanged("F051202Datas");
			}
		}


		private List<F700101CarData> _f700101Datas;

		public List<F700101CarData> F700101Datas
		{
			get { return _f700101Datas; }
			set
			{
				_f700101Datas = value;
				RaisePropertyChanged("F700101Datas");
			}
		}

		private F051202Data _selectedF051202Data;

		public F051202Data SelectedF051202Data
		{
			get { return _selectedF051202Data; }
			set
			{
				_selectedF051202Data = value;
                RaisePropertyChanged("SelectedF051202Data");
			}
		}


		#endregion

		#region 揀貨單補印-揀貨單 Grid List

		private List<F051201SelectedData> _f051201SelectedDatas;

		public List<F051201SelectedData> F051201SelectedDatas
		{
			get { return _f051201SelectedDatas; }
			set
			{
				_f051201SelectedDatas = value;
				RaisePropertyChanged("F051201SelectedDatas");
			}
		}

		private F051201SelectedData _selectedF051201SelectedData;

		public F051201SelectedData SelectedF051201SelectedData
		{
			get { return _selectedF051201SelectedData; }
			set
			{
				_selectedF051201SelectedData = value;
				RaisePropertyChanged("SelectedF051201SelectedData");
			}
		}


        #endregion

        #region 揀貨單報表List

        private List<F051201ReportDataA> _f051201ReportDataAs;
		public List<F051201ReportDataA> F051201ReportDataAs
		{
			get { return _f051201ReportDataAs; }
			set
			{
				_f051201ReportDataAs = value;
				RaisePropertyChanged("F051201ReportDataAs");
			}
		}

        #endregion

        #region  B2C揀貨單列印/補印 O單號條碼列印
        private List<RP0501010004Model> _rP0502020004Model;
        public List<RP0501010004Model> RP0501010004Model
        {
            get { return _rP0502020004Model; }
            set
            {
                _rP0502020004Model = value;
                RaisePropertyChanged("RP0501010004Model");
            }
        }
        #endregion

        #region 批量揀貨
        private List<P050103ReportData> _p050103ReportData;
        public List<P050103ReportData> P050103ReportData
        {
            get { return _p050103ReportData; }
            set
            {
                _p050103ReportData = value;
                RaisePropertyChanged("P050103ReportData");
            }
        }
        #endregion

        #region
        private List<RP0501010005Model> _rP0501010005Model;
        public List<RP0501010005Model> RP0501010005Model
        {
            get { return _rP0501010005Model; }
            set
            {
                _rP0501010005Model = value;
                RaisePropertyChanged("RP0501010005Model");
            }
        }
        #endregion

        #region

        #endregion

        #region 台車分配揀貨單-比例

        private string _sharePercent;

		public string SharePercent
		{
			get { return _sharePercent; }
			set
			{
				_sharePercent = value;
				RaisePropertyChanged("SharePercent");
				CountPaperCount();
			}
		}

		#endregion

		#region 台車分配揀貨單-張數
		private string _deviceCount;

		public string DeviceCount
		{
			get { return _deviceCount; }
			set
			{
				_deviceCount = value;
				RaisePropertyChanged("DeviceCount");
			}
		}
		#endregion

		#region 是否全選

		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				CheckSelectedAll(_isCheckAll);
				RaisePropertyChanged("IsCheckAll");
			}
		}
		#endregion

		#region 溫層List
		private List<NameValuePair<string>> _tmprTypeList;

		public List<NameValuePair<string>> TmprTypeList
		{
			get { return _tmprTypeList; }
			set
			{
				if (_tmprTypeList == value)
					return;
				Set(() => TmprTypeList, ref _tmprTypeList, value);
			}
		}

        #endregion


        #region
        private string _virtualPickOrdQty;

		public string VirturalPickOrdQty
		{
			get { return _virtualPickOrdQty; }
			set
			{
				if (_virtualPickOrdQty == value)
					return;
				Set(() => VirturalPickOrdQty, ref _virtualPickOrdQty, value);
			}
		}
		#endregion
		
		#endregion

		#region 依照台車分配比例計算揀貨單張數

		private void CountPaperCount()
		{
			if (_selectedF051201Data != null)
			{
				var proxy = GetProxy<F05Entities>();

				if (_selectedF051201Data.ISPRINTED == "0")
				{
					decimal d;
					if (decimal.TryParse(SharePercent, out d))
					{
						var sharePercent = string.IsNullOrEmpty(SharePercent) ? 0 : decimal.Parse(SharePercent);
						decimal percent = sharePercent / (decimal)100.0;
						int dataCount =
							proxy.CreateQuery<F051201>("GetDatasByNoVirturlItem")
							.AddQueryExOption("dcCode", _selectedF051201Data.DC_CODE)
							.AddQueryExOption("gupCode", _selectedF051201Data.GUP_CODE)
							.AddQueryExOption("custCode", _selectedF051201Data.CUST_CODE)
							.AddQueryExOption("delvDate", _selectedF051201Data.DELV_DATE)
							.AddQueryExOption("pickTime", _selectedF051201Data.PICK_TIME)
							.Where(o=>o.ISDEVICE=="0" && o.PICK_STATUS ==0).Count();				
						DeviceCount = Math.Floor(percent * dataCount).ToString();
					}
					else
					{
						if (!string.IsNullOrEmpty(SharePercent))
						{
							var message = new MessagesStruct
							{
								Button = DialogButton.OK,
								Image = DialogImage.Warning,
								Message = Properties.Resources.P0501020000_DataTypeIsNumber,
								Title = Resources.Resources.Warning
							};
							ShowMessage(message);
							SharePercent = "";
						}
						DeviceCount = "";
					}

				}
				else
				{
					if (!string.IsNullOrEmpty(SharePercent))
						SharePercent = "";
					else
						DeviceCount = proxy.F051201s.Where(
								o =>
									o.DC_CODE == _selectedF051201Data.DC_CODE && o.GUP_CODE == _selectedF051201Data.GUP_CODE &&
									o.CUST_CODE == _selectedF051201Data.CUST_CODE && o.DELV_DATE == _selectedF051201Data.DELV_DATE &&
									o.PICK_TIME == _selectedF051201Data.PICK_TIME && o.ORD_TYPE == "1" && o.ISDEVICE == "1").Count().ToString();

				}
			}
			else
			{
				DeviceCount = "";
			}
		}

		#endregion

		#region 下拉式選單資料來源

		/// <summary>
		/// 設定DC清單
		/// </summary>
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (DcList.Any())
				SelectedDcCode = DcList.First().Value;
		}

		private void SetPrintStatus()
		{
			PrintStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F051201", "ISPRINTED", true);
			if (PrintStatusList.Any())
				SelectedPrintStatus = PrintStatusList.First().Value;
		}

		private void SetTmprTypes()
		{
			TmprTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1980", "TMPR_TYPE");
		}

  
		#endregion

		#region Grid 全選

		public void CheckSelectedAll(bool isChecked)
		{
			if (F051201SelectedDatas != null)
				foreach (var f051201SelectedData in F051201SelectedDatas)
					f051201SelectedData.IsSelected = isChecked;
		}

		#endregion

		#region 批次出車時段 Search
		public ICommand SearchF700101DataCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchF700101Data(), () => UserOperateMode == OperateMode.Query,
					c => { }
					);
			}
		}

		private void DoSearchF700101Data()
		{
			if (_selectedF051201Data != null)
			{
				var proxyEx = GetExProxy<P05ExDataSource>();
				F700101Datas = proxyEx.CreateQuery<F700101CarData>("GetF700101Data")
								.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
								.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
								.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
								.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
								.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME))
								.AddQueryOption("sourceTye", string.Format("'{0}'", _selectedF051201Data.SOURCE_TYPE))
								.AddQueryOption("ordType", string.Format("'{0}'", "1")).ToList();

				
			}
		}

		#endregion

		#region 揀貨單分頁明細 Search
		public ICommand SearchF051202DataCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchF051202Data(), () => UserOperateMode == OperateMode.Query,
					c => DoSearchF051202DataComplete()
					);
			}
		}

		private void DoSearchF051202Data()
		{
			if (_selectedF051201Data != null)
			{
				var proxyEx = GetExProxy<P05ExDataSource>();
				F051202Datas = proxyEx.CreateQuery<F051202Data>("GetF051202DatasForB2C")
															.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
															.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
															.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
															.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
															.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME)).ToList();
			}
		}

		private void DoSearchF051202DataComplete()
		{
			if (F051202Datas != null && F051202Datas.Any())
				SelectedF051202Data = F051202Datas.First();
		}
		#endregion

		#region 揀貨單補印-揀貨單 Search
		public ICommand SearchF051201SelectedDataCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchF051201SelectedData(), () => UserOperateMode == OperateMode.Query,
					c => DoSearchF051201SelectedDataComplete()
					);
			}
		}

		private void DoSearchF051201SelectedData()
		{
			if (_selectedF051201Data != null)
			{
				var proxyEx = GetExProxy<P05ExDataSource>();
				F051201SelectedDatas = proxyEx.CreateQuery<F051201SelectedData>("GetF051201SelectedDatasForB2C")
															.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
															.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
															.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
															.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
															.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME)).ToList();
			}
		}

		private void DoSearchF051201SelectedDataComplete()
		{
			if (F051201SelectedDatas != null && F051201SelectedDatas.Any())
				SelectedF051201SelectedData = F051201SelectedDatas.First();
			CountPaperCount();
			IsCheckAll = false;
			if (_selectedF051201Data == null || _selectedF051201Data.ISPRINTED == "0")
				SharePercent = "";
			PickOrdNo = string.Empty;
			IsEnabledPickOrdNoPrint = _selectedF051201Data != null && _selectedF051201Data.ISPRINTED == "1";
			IsEnabledShareCount = _selectedF051201Data != null && _selectedF051201Data.ISPRINTED == "0";

		}

		#endregion

		#region 挑選批次 Search
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
			//執行查詢
			var proxyEx = GetExProxy<P05ExDataSource>();
			F051201Datas = proxyEx.CreateQuery<F051201Data>("GetF051201DatasForB2C")
                                                        .AddQueryExOption("dcCode", _selectedDcCode)
                                                        .AddQueryExOption("gupCode", GupCode)
                                                        .AddQueryExOption("custCode", CustCode)
                                                        .AddQueryExOption("delvDate", DelvDate)
                                                        .AddQueryExOption("isPrinted", SelectedPrintStatus).ToList();

		}

		private void DoSearchComplete()
		{
			if (F051201Datas.Any())
          SelectedF051201Data = F051201Datas.First();
			else
				ShowMessage(Messages.InfoNoData);
		}
        #endregion Search
        

        #region Print
        private ICommand _printCommand;

		public ICommand PrintCommand
		{
			get
			{
				bool isCompleted = false;
				return _printCommand ??
					(_printCommand = CreateBusyAsyncCommand<PrintType>(o => isCompleted = DoPrint(o),
																	   o => _selectedF051201Data != null,
																	   o =>
																	   {
																		   if (isCompleted)
																			   DoPrintReport(o);
																	   }));
			}
		}

		private bool DoPrint(PrintType printType)
		{
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Message = "",
				Title = Resources.Resources.Warning
			};

			string pickOrdNo = "";
			decimal d = 0;
			if (_selectedF051201Data.ISPRINTED == "0" && string.IsNullOrEmpty(SharePercent))
				message.Message = Properties.Resources.P0501020000_InputPickProportion;
			else if (_selectedF051201Data.ISPRINTED == "0" && !decimal.TryParse(SharePercent, out d))
				message.Message = Properties.Resources.P0501020000_DataTypeIsNumber;
			else if (d < 0 || d > 100)
				message.Message = Properties.Resources.P0501020000_PickProportionError;

			else if (_selectedF051201Data.ISPRINTED == "1")
			{
				if (!string.IsNullOrEmpty(_pickOrdNo))
					pickOrdNo = _pickOrdNo;
				else if (_f051201SelectedDatas != null)
					pickOrdNo = string.Join(",", _f051201SelectedDatas.Where(o => o.IsSelected).Select(o => o.PICK_ORD_NO).ToArray());

				if (string.IsNullOrEmpty(pickOrdNo))
					message.Message = Properties.Resources.P0501010000_NotSelectData;
			}

			if (message.Message.Any())
			{
				ShowMessage(message);
				return false;
			}

			var selectedF051201 = _selectedF051201Data;
			if (selectedF051201 == null)
			{
				ShowWarningMessage(Properties.Resources.P0501020000_NotSelectPick);
				return false;
			}

			var isPrint = true;
			if (selectedF051201.ISPRINTED == "0")
			{
				isPrint = UpdateIsPrintedAndIsDevice();
				if (isPrint)
				{
					DoSearch();
					SelectedF051201Data =
						F051201Datas.FirstOrDefault(
							o => o.DC_CODE == selectedF051201.DC_CODE
								&& o.GUP_CODE == selectedF051201.GUP_CODE
								&& o.CUST_CODE == selectedF051201.CUST_CODE
								&& o.DELV_DATE == selectedF051201.DELV_DATE
								&& o.PICK_TIME == selectedF051201.PICK_TIME);
          }
			}

			if (isPrint)
			{
                // 產生單一揀貨 揀貨單  
                var proxyEx = GetExProxy<P05ExDataSource>();
                F051201ReportDataAs = proxyEx.GetF051201ReportDataAsForB2C(selectedF051201.DC_CODE,
                        selectedF051201.GUP_CODE,
                        selectedF051201.CUST_CODE,
                        selectedF051201.DELV_DATE,
                        selectedF051201.PICK_TIME,
                        pickOrdNo).ToList();
                // 產生單一揀貨 標籤貼紙
                RP0501010004Model = proxyEx.GetF051201SingleStickersReportDataAsForB2C(selectedF051201.DC_CODE,
                        selectedF051201.GUP_CODE,
                        selectedF051201.CUST_CODE,
                        selectedF051201.DELV_DATE,
                        selectedF051201.PICK_TIME,
                        pickOrdNo).ToList();
                //產生批次揀貨 揀貨單
                P050103ReportData = proxyEx.GetF051201BatchReportDataAsForB2C(selectedF051201.DC_CODE,
                        selectedF051201.GUP_CODE,
                        selectedF051201.CUST_CODE,
                        selectedF051201.DELV_DATE,
                        selectedF051201.PICK_TIME,
                        pickOrdNo).ToList();
                //摻生批次揀貨 標籤貼紙
                RP0501010005Model = proxyEx.GetF051201BatchStickersReportDataAsForB2C(selectedF051201.DC_CODE,
                        selectedF051201.GUP_CODE,
                        selectedF051201.CUST_CODE,
                        selectedF051201.DELV_DATE,
                        selectedF051201.PICK_TIME,
                        pickOrdNo).ToList();


            }

			return isPrint;
		}

		private bool UpdateIsPrintedAndIsDevice()
		{
			var proxyEx = GetExProxy<P05ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("UpdateF051201ForB2C")
				.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
				.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
				.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME))
				.AddQueryOption("userId", string.Format("'{0}'", _userId))
				.AddQueryOption("userName", string.Format("'{0}'", _userName))
				.AddQueryOption("deviceCount", string.Format("'{0}'", DeviceCount)).ToList();
			if (!result.First().IsSuccessed)
				ShowWarningMessage(result.First().Message);
			return result.First().IsSuccessed;


		}
		#endregion
	}
}
