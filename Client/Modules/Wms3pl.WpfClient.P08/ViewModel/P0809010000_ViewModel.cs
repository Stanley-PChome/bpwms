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
using Wms3pl.WpfClient.DataServices.F70DataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using System.Data;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices;
using wcf05 = Wms3pl.WpfClient.ExDataServices.P05WcfService;
using System.IO;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0809010000_ViewModel : InputViewModelBase
	{
		private string _gupCode;
		private string _custCode;
		private string _userId;
		public Action SearchAction = delegate { };
		public Action PrintAction = delegate { };
		public P0809010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				InitControls();
			}
		}

		private void InitControls()
		{
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			_userId = Wms3plSession.Get<UserInfo>().Account;
			//RFClient 
			//_gupCode = "01";
			//_custCode = "010001";
			GetDcCodes();
			//GetChkOutTime();
			//GetDelvCompany();
		}

		private List<F050801WithF700102> _dataList;
		public List<F050801WithF700102> DataList { get { return _dataList; } set { _dataList = value; RaisePropertyChanged("DataList"); } }

		private List<P080901ShipReport> _rptDataList;
		public List<P080901ShipReport> RptDataList { get { return _rptDataList; } set { _rptDataList = value; RaisePropertyChanged("RptDataList"); } }

		private F050801WithF700102 _selectedData;
		public F050801WithF700102 SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))
				{
					//ShowMessage("編輯狀態不可選取!");
					return;
				}
				else
				{
					_selectedData = value;
					RaisePropertyChanged("UploadFileEnabled");
					RaisePropertyChanged("ViewFileEnabled");
					RaisePropertyChanged("PrintEnabled");
					RaisePropertyChanged("UpdCarNoEnabled");
					RaisePropertyChanged("SelectedData");
				}
			}
		}
		public bool UploadFileEnabled
		{
			get
			{
				if (SelectedData == null)
					return false;
				else
					return !(string.IsNullOrEmpty(SelectedData.CAR_NO_A) && string.IsNullOrEmpty(SelectedData.CAR_NO_B) && string.IsNullOrEmpty(SelectedData.CAR_NO_C));
			}
		}
		public bool ViewFileEnabled
		{
			get
			{
				if (SelectedData == null)
					return false;
				else
					return SelectedData.ISSEAL.Equals("1");
			}
		}
		public bool PrintEnabled
		{
			get
			{
				if (SelectedData == null)
					return false;
				else
					return !(string.IsNullOrEmpty(SelectedData.CAR_NO_A) && string.IsNullOrEmpty(SelectedData.CAR_NO_B) && string.IsNullOrEmpty(SelectedData.CAR_NO_C));
			}
		}
		public bool UpdCarNoEnabled
		{
			get
			{
				if (SelectedData == null)
					return false;
				else
					return true;

			}
		}


		#region 物流中心
		private List<NameValuePair<string>> _dcCodes;
		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				RaisePropertyChanged();
			}
		}
		private void GetDcCodes()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any())
				SelectedDcCode = DcCodes.First().Value;
		}

		private string _selectedDcCode;

		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				_selectedDcCode = value;
				if (value != null)
				{
					GetChkOutTime();
					GetDelvCompany();
				}

				DataList = null;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 批次日期
		private DateTime _searchTakeDate = DateTime.Today;
		public DateTime SearchTakeDate
		{
			get { return _searchTakeDate; }
			set
			{
				_searchTakeDate = value;
				GetChkOutTime();
				DataList = null;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 出車時段
		private ObservableCollection<NameValuePair<string>> _chkOutTime;
		public ObservableCollection<NameValuePair<string>> ChkOutTime
		{
			get { return _chkOutTime; }
			set
			{
				_chkOutTime = value;
				DataList = null;
				RaisePropertyChanged();
			}
		}
		private void GetChkOutTime()
		{
			var proxy = GetProxy<F70Entities>();
			var data = proxy.CreateQuery<F700102>("GetF700102List")
							.AddQueryExOption("dcCode", SelectedDcCode)
							.AddQueryExOption("gupCode", _gupCode)
							.AddQueryExOption("custCode", _custCode)
							.AddQueryExOption("takeDate", SearchTakeDate)
							.ToList();
			var chkOutTimeList = data.Select(o => o.TAKE_TIME).Distinct();
			ChkOutTime = (from o in chkOutTimeList
						  select new NameValuePair<string>
						  {
							  Name = o,
							  Value = o
						  }).ToObservableCollection();
			if (ChkOutTime.Any())
				SelectedChkOutTime = ChkOutTime.First().Value;
		}

		private string _selectedChkOutTime;
		public string SelectedChkOutTime
		{
			get { return _selectedChkOutTime; }
			set
			{
				_selectedChkOutTime = value;
				DataList = null;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 配送商
		private ObservableCollection<NameValuePair<string>> _delvCompany;
		public ObservableCollection<NameValuePair<string>> DelvCompany
		{
			get { return _delvCompany; }
			set
			{
				_delvCompany = value;
				RaisePropertyChanged();
			}
		}
		private void GetDelvCompany()
		{
			var proxy = GetProxy<F19Entities>();
			DelvCompany = proxy.CreateQuery<F1947>("GetAllowedF1947s")
								.AddQueryExOption("dcCode", SelectedDcCode)
								.AddQueryExOption("gupCode", _gupCode)
								.AddQueryExOption("custCode", _custCode)
								.Where(x => x.TYPE == "0")
								.OrderBy(item => item.ALL_COMP)
								.Select(o => new NameValuePair<string>
								{
									Name = o.ALL_COMP,
									Value = o.ALL_ID
								}).ToObservableCollection();
			if (DelvCompany.Any())
				SelectedDelvCompany = DelvCompany.First().Value;
		}
		private string _selectedDelvCompany;
		public string SelectedDelvCompany
		{
			get { return _selectedDelvCompany; }
			set
			{
				_selectedDelvCompany = value;
				DataList = null;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region 印列出車明細
		public ICommand PrintCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoGetRptData(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						PrintAction();
					}
					);
			}
		}

		private void DoGetRptData()
		{
			var proxy = GetExProxy<P08ExDataSource>();
			var result = proxy.CreateQuery<P080901ShipReport>("GetF050801WithF700102sForReport")
							.AddQueryExOption("dcCode", SelectedDcCode)
							.AddQueryExOption("gupCode", _gupCode)
							.AddQueryExOption("custCode", _custCode)
							.AddQueryExOption("takeDate", SelectedData.TAKE_DATE)
							.AddQueryExOption("takeTime", SelectedData.TAKE_TIME)
							.AddQueryExOption("allId", SelectedDelvCompany)
							.ToList();

			if (result != null && result.Count() > 0)
			{
				RptDataList = result;
			}

		}
		#endregion

		#region 查詢
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						Car_No_A = "";
						Car_No_B = "";
						Car_No_C = "";
						RaisePropertyChanged("Car_No_A");
						RaisePropertyChanged("Car_No_B");
						RaisePropertyChanged("Car_No_C");
						SearchAction();
					}
					);
			}
		}

		private void DoSearch()
		{
			SelectedData = null;
			var proxy = GetExProxy<P08ExDataSource>();
			var result = proxy.CreateQuery<F050801WithF700102>("GetF050801WithF700102s")
							.AddQueryExOption("dcCode", SelectedDcCode)
							.AddQueryExOption("gupCode", _gupCode)
							.AddQueryExOption("custCode", _custCode)
							.AddQueryExOption("takeDate", SearchTakeDate)
							.AddQueryExOption("checkoutTime", SelectedChkOutTime)
							.AddQueryExOption("allId", SelectedDelvCompany)
							.AddQueryExOption("checkWmsStatus", "1")
							.ToList();
			if (result.Any())
			{
				DataList = result;
				SelectedData = DataList.FirstOrDefault();
			}
			else
			{
				DataList = null;
				ShowMessage(Messages.InfoNoData);
			}

		}
		#endregion Search

		#region 更新車號
		private string _carnoa;
		public string Car_No_A
		{
			get { return _carnoa; }
			set
			{
				_carnoa = value.Trim().Replace(" ", "");
				RaisePropertyChanged("Car_No_A");
			}
		}
		private string _carnob;
		public string Car_No_B
		{
			get { return _carnob; }
			set
			{
				_carnob = value.Trim().Replace(" ", "");
				RaisePropertyChanged("Car_No_B");
			}
		}
		private string _carnoc;
		public string Car_No_C
		{
			get { return _carnoc; }
			set
			{
				_carnoc = value.Trim().Replace(" ", "");
				RaisePropertyChanged("Car_No_C");
			}
		}
		public ICommand UpdCarNoCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoUpdCarNo(),
					() => true,
					p => DoUpdCompleted()
					);
			}
		}

		private bool _updCarNoSuccess;
		private void DoUpdCarNo()
		{
			_updCarNoSuccess = true;
			#region 車號檢查
			if (string.IsNullOrEmpty(Car_No_A) && string.IsNullOrEmpty(Car_No_B) && string.IsNullOrEmpty(Car_No_C))
			{
				_updCarNoSuccess = false;
				DialogService.ShowMessage(Properties.Resources.P0809010000_CarNoIsNull);
				return;
			}
			if ((!string.IsNullOrEmpty(Car_No_A) && IsValidCarNo(Car_No_A) == false) || (!string.IsNullOrEmpty(Car_No_B) && IsValidCarNo(Car_No_B) == false) ||
				(!string.IsNullOrEmpty(Car_No_C) && IsValidCarNo(Car_No_C) == false))
			{
				_updCarNoSuccess = false;
				DialogService.ShowMessage(Properties.Resources.P0809010000_CarNoFormatError);
				return;
			}
			#endregion

			var proxyEx = GetExProxy<P08ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("UpdateCarNo")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", _gupCode)
				.AddQueryExOption("custCode", _custCode)
				.AddQueryExOption("delvDate", SearchTakeDate.ToString("yyyy/MM/dd"))
				.AddQueryExOption("takeTime", SelectedChkOutTime)
				.AddQueryExOption("allId", SelectedDelvCompany)
				.AddQueryExOption("carNoA", Car_No_A)
				.AddQueryExOption("carNoB", Car_No_B)
				.AddQueryExOption("carNoC", Car_No_C)
				.AddQueryExOption("needSeal", SelectedData.NEED_SEAL).ToList();
			if (result.First().IsSuccessed)
				_updCarNoSuccess = true;
			else
			{
				ShowWarningMessage(Properties.Resources.P0809010000_UpdateCarNoFail);
				_updCarNoSuccess = false;
			}
		}

		private void DoUpdCompleted()
		{
			if (_updCarNoSuccess)
			{
				PrintCommand.Execute(null);
				var strSEAL = "";
				if (SelectedData.NEED_SEAL.Equals("1")) { strSEAL = Properties.Resources.P0809010000_NEED_SEAL; }
				DialogService.ShowMessage(Properties.Resources.P0809010000_NEED_SEAL_Msg + strSEAL);
				SearchCommand.Execute(null);
				UserOperateMode = OperateMode.Query;
			}
		}
		#endregion

		#region 上傳圖檔
		public string ShareFolderItemFiles
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "CarSeal", SelectedDcCode, this._gupCode, this._custCode, SearchTakeDate.Year.ToString() }); }
		}
		public string ImageDirectory
		{
			get { return ShareFolderItemFiles + SearchTakeDate.ToString("yyyyMMdd") + SelectedData.TAKE_TIME.Replace(":", ""); }
		}
		private string _rtNo = string.Empty;

		private List<string> _fileList;
		public List<string> FileList
		{
			get { return _fileList; }
			set { _fileList = value; }
		}
		public ICommand UploadCommand
		{
			get
			{
				//o => DoSearch(), () => UserOperateMode == OperateMode.Query
				return CreateBusyAsyncCommand(
					o => DoUpload(),
					() => true,
					o => DoUploadCompleted()
				);
			}
		}

		private bool _isUploadSuccess;
		public void DoUpload()
		{
			_isUploadSuccess = false;

			if (Directory.Exists(ImageDirectory))
			{
				var dr = DialogService.ShowMessage(Properties.Resources.P0809010000_ImageDirectoryExistMsg, WpfClient.Resources.Resources.Information, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
				if (dr == UILib.Services.DialogResponse.No)
					return;
				else
					Directory.Delete(ImageDirectory, true);
			}

			Directory.CreateDirectory(ImageDirectory);
			short fileNo = 1;
			string fileHeaderName = DateTime.Now.ToString("HHmmss");
			foreach (var p in FileList)
			{
				var newFileName = Path.Combine(ImageDirectory, string.Format("{0}_{1}.jpg", fileHeaderName, fileNo.ToString("00")));
				System.IO.File.Copy(p, newFileName, true);
				fileNo++;
			}

			var proxyEx = GetExProxy<P08ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("UpdateIsSeal")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", _gupCode)
				.AddQueryExOption("custCode", _custCode)
				.AddQueryExOption("delvDate", SearchTakeDate.ToString("yyyy/MM/dd"))
				.AddQueryExOption("takeTime", SelectedChkOutTime)
				.AddQueryExOption("allId", SelectedDelvCompany).ToList();
			if (result.First().IsSuccessed)
			{
				DialogService.ShowMessage(Properties.Resources.P0809010000_UpdateIsSealSuccess);
				_isUploadSuccess = true;

			}
			else
			{
				ShowWarningMessage(Properties.Resources.P0809010000_UpdateIsSealFail);
				_isUploadSuccess = false;
			}




		}

		public void DoUploadCompleted()
		{
			if (_isUploadSuccess)
				SearchCommand.Execute(null);
			//DoSearch();
		}
		#endregion

		public bool IsValidCarNo(String carno)
		{
			System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9\-]+$");
			return reg1.IsMatch(carno);
		}


		#region 變更調撥單時更新來源單據狀態
		private void UpdateSourceNo(F050801 f050801)
		{
			if (f050801 != null && !string.IsNullOrEmpty(f050801.WMS_ORD_NO))
			{
				var proxy05 = new wcf05.P05WcfServiceClient();
				var wcf050801 = ExDataMapper.Map<F050801, wcf05.F050801>(f050801);
				var wcfResult = RunWcfMethod<wcf05.ExecuteResult>(proxy05.InnerChannel, () => proxy05.UpdateSourceNoStatus(wcf050801));

			}
		}
		#endregion
	}
}
