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
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public partial class P0501010000_ViewModel : InputViewModelBase
	{
		public P0501010000_ViewModel()
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
				_selectedDcCode = value;
				RaisePropertyChanged("SelectedDcCode");
				F051201Datas = null;
				F051202Datas = null;
				F051201SelectedDatas = null;
				IsEnabledPickOrdNoPrint = false;
				RbReportTypeA = true;
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

		#region 列印報表種類

		private bool _rbReportTypeA;

		public bool RbReportTypeA
		{
			get { return _rbReportTypeA; }
			set
			{
				_rbReportTypeA = value;
				RaisePropertyChanged("RbReportTypeA");
			}
		}

		private bool _rbReportTypeB;

		public bool RbReportTypeB
		{
			get { return _rbReportTypeB; }
			set
			{
				_rbReportTypeB = value;
				RaisePropertyChanged("RbReportTypeB");
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

		private F051201Data _selectedF051201Data;

		public F051201Data SelectedF051201Data
		{
			get { return _selectedF051201Data; }
			set
			{
				_selectedF051201Data = value;
				RaisePropertyChanged("SelectedF051201Data");
				if (_selectedF051201Data != null)
				{

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

		private List<F051201ReportDataB> _f051201ReportDataBs;
		public List<F051201ReportDataB> F051201ReportDataBs
		{
			get { return _f051201ReportDataBs; }
			set
			{
				_f051201ReportDataBs = value;
				RaisePropertyChanged("F051201ReportDataBs");
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
			var proxyEx = GetExProxy<P05ExDataSource>();
			F051202Datas = proxyEx.CreateQuery<F051202Data>("GetF051202DatasForB2B")
														.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
														.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
														.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
														.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
														.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME)).ToList();
		}

		private void DoSearchF051202DataComplete()
		{
			if (F051202Datas != null && F051202Datas.Any())
				SelectedF051202Data = F051202Datas.First();
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
			var proxyEx = GetExProxy<P05ExDataSource>();
			F700101Datas = proxyEx.CreateQuery<F700101CarData>("GetF700101Data")
									.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
									.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
									.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
									.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
									.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME))
									.AddQueryOption("sourceTye", string.Format("'{0}'", _selectedF051201Data.SOURCE_TYPE))
									.AddQueryOption("ordType", string.Format("'{0}'", "0")).ToList(); //B2B

		
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
			var proxyEx = GetExProxy<P05ExDataSource>();
			F051201SelectedDatas = proxyEx.CreateQuery<F051201SelectedData>("GetF051201SelectedDatasForB2B")
														.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
														.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
														.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
														.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
														.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME)).ToList();
		}

		private void DoSearchF051201SelectedDataComplete()
		{
			if (F051201SelectedDatas != null && F051201SelectedDatas.Any())
				SelectedF051201SelectedData = F051201SelectedDatas.First();
			RbReportTypeA = true;
			PickOrdNo = string.Empty;
			IsCheckAll = false;
			IsEnabledPickOrdNoPrint = _selectedF051201Data.ISPRINTED == "1";
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
			F051201Datas = proxyEx.CreateQuery<F051201Data>("GetF051201DatasForB2B")
                                                        .AddQueryExOption("dcCode", _selectedDcCode)
                                                        .AddQueryExOption("gupCode", GupCode)
                                                        .AddQueryExOption("custCode", CustCode)
                                                        .AddQueryExOption("delvDate", DelvDate)
                                                        .AddQueryExOption("isPrinted", SelectedPrintStatus).ToList();

		}

		private void DoSearchComplete()
		{
			if (F051201Datas != null && F051201Datas.Any())
				SelectedF051201Data = F051201Datas.First();
			else
				ShowMessage(Messages.InfoNoData);
		}
		#endregion Search

		#region Print
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand<PrintType>(
					DoPrint,
				(t) => !IsBusy && _selectedF051201Data != null);
			}
		}

		private void DoPrint(PrintType printType)
		{
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Message = "",
				Title = Resources.Resources.Warning
			};
			if (_selectedF051201Data.ISPRINTED == "0") //再做一次確認以免重覆新增或更新
			{
				var dcCode = _selectedF051201Data.DC_CODE;
				var gupCode = _selectedF051201Data.GUP_CODE;
				var custCode = _selectedF051201Data.CUST_CODE;
				var delvDate = _selectedF051201Data.DELV_DATE;
				var pickTime = _selectedF051201Data.PICK_TIME;
				DoSearch();
				SelectedF051201Data =
					F051201Datas.Find(
						o =>
							o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.DELV_DATE == delvDate &&
							o.PICK_TIME == pickTime);
			}

			string pickOrdNo = "";
			// 未列印且但有揀貨單已列印 或本批次揀貨單已列印 =>為補印列印 
			if (_selectedF051201Data.ISPRINTED == "1")
			{
				if (!string.IsNullOrEmpty(_pickOrdNo))
					pickOrdNo = _pickOrdNo;
				else if (_f051201SelectedDatas != null)
					pickOrdNo = string.Join(",", _f051201SelectedDatas.Where(o => o.IsSelected).Select(o => o.PICK_ORD_NO).ToArray());

				if (string.IsNullOrEmpty(pickOrdNo))
					message.Message = Properties.Resources.P0501010000_NotSelectData;
			}

			if (message.Message.Length == 0)
			{
				var isPrint = true;
				if (_selectedF051201Data.ISPRINTED == "0" && (_f051201SelectedDatas == null || !_f051201SelectedDatas.Any()))
					//只有未列印且沒有補印資料才更新
					isPrint = UpdateIsPrinted();
				if (isPrint)
				{
					//取得報表資料
					bool isHasData;
					var proxyEx = GetExProxy<P05ExDataSource>();
					if (RbReportTypeA)
					{
						F051201ReportDataAs = proxyEx.GetF051201ReportDataAsForB2B(_selectedF051201Data.DC_CODE,
																					_selectedF051201Data.GUP_CODE,
																					_selectedF051201Data.CUST_CODE,
																					_selectedF051201Data.DELV_DATE,
																					_selectedF051201Data.PICK_TIME,
																					pickOrdNo).ToList();

						isHasData = F051201ReportDataAs.Any();
					}
					else
					{
						F051201ReportDataBs = proxyEx.CreateQuery<F051201ReportDataB>("GetF051201ReportDataBsForB2B")
							.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
							.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
							.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
							.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
							.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME))
							.AddQueryOption("pickOrdNo", string.Format("'{0}'", pickOrdNo)).ToList();
						isHasData = F051201ReportDataBs.Any();
					}
					if (isHasData)
						DoPrintReport(printType);
					else
						message.Message = Properties.Resources.P0501010000_DataNotExist;

					var dcCode = _selectedF051201Data.DC_CODE;
					var gupCode = _selectedF051201Data.GUP_CODE;
					var custCode = _selectedF051201Data.CUST_CODE;
					var delvDate = _selectedF051201Data.DELV_DATE;
					var pickTime = _selectedF051201Data.PICK_TIME;
					DoSearch();
					SelectedF051201Data =
						F051201Datas.Find(
							o =>
								o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.DELV_DATE == delvDate &&
								o.PICK_TIME == pickTime);

				}
			}
			if (message.Message.Length > 0)
				ShowMessage(message);
		}

		private bool UpdateIsPrinted()
		{
			var proxyEx = GetExProxy<P05ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("UpdateF051201ForB2B")
				.AddQueryOption("dcCode", string.Format("'{0}'", _selectedF051201Data.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", _selectedF051201Data.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", _selectedF051201Data.CUST_CODE))
				.AddQueryOption("delvDate", string.Format("'{0}'", _selectedF051201Data.DELV_DATE.ToString("yyyy/MM/dd")))
				.AddQueryOption("pickTime", string.Format("'{0}'", _selectedF051201Data.PICK_TIME))
				.AddQueryOption("userId", string.Format("'{0}'", _userId))
				.AddQueryOption("userName", string.Format("'{0}'", _userName)).ToList();

			if (!result.First().IsSuccessed)
				ShowWarningMessage(result.First().Message);
			return result.First().IsSuccessed;
		}
		#endregion
	}
}
