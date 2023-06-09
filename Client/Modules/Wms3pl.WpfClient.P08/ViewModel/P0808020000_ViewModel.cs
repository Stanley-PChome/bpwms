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

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using wcf05 = Wms3pl.WpfClient.ExDataServices.P05WcfService;
using P08ExDataSrv = Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;
using System.Data;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0808020000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		private bool _isInit = true;
		public Action OnSearchCommon = delegate { };

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
			}
		}
		#endregion
		#region Form - 批次日期(起)
		private DateTime? _beginDelvDate = DateTime.Today;
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Required_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public DateTime? BeginDelvDate
		{
			get { return _beginDelvDate; }
			set { _beginDelvDate = value; RaisePropertyChanged("BeginDelvDate"); }
		}
		#endregion
		#region Form - 批次日期(迄)
		private DateTime? _endDelvDate = DateTime.Today;
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Required_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public DateTime? EndDelvDate
		{
			get { return _endDelvDate; }
			set { _endDelvDate = value; RaisePropertyChanged("EndDelvDate"); }
		}
		#endregion
		#region Form - 出貨單號
		private string _txtWMS_ORD_NO;
		public string TxtWMS_ORD_NO
		{
			get { return _txtWMS_ORD_NO; }
			set
			{
				_txtWMS_ORD_NO = value;
				RaisePropertyChanged("TxtWMS_ORD_NO");
			}
		}
		#endregion
		#region Form - 託運單號
		private string _txtPAST_NO;
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Required_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public string TxtPAST_NO
		{
			get { return _txtPAST_NO; }
			set
			{
				_txtPAST_NO = value;
				RaisePropertyChanged("TxtPAST_NO");
			}
		}
		#endregion
		#region Form - 語音
		private bool _playSound = true;
		public bool PlaySound
		{
			get { return _playSound; }
			set { _playSound = value; RaisePropertyChanged("PlaySound"); }
		}
		#endregion
		#region Data - 批次時段
		private string _laPICK_TIME;
		public string LaPICK_TIME
		{
			get { return _laPICK_TIME; }
			set
			{
				_laPICK_TIME = value;
				RaisePropertyChanged("LaPICK_TIME");
			}
		}
		#endregion
		#region Data - 出貨單包裝狀態
		private bool _package = false;
		public bool Package
		{
			get { return _package; }
			set { _package = value; RaisePropertyChanged("Package"); }
		}

		private string _packageColor = "Red";
		public string PackageColor
		{
			get { return _packageColor; }
			set { _packageColor = value; RaisePropertyChanged("PackageColor"); }
		}
		#endregion
		#region Data - 批次時段包裝狀態
		private string _groupColor = "Red";
		public string GroupColor
		{
			get { return _groupColor; }
			set { _groupColor = value; RaisePropertyChanged("GroupColor"); }
		}
		#endregion
		#region Data - 配送商
		private string _laALL_COMP;
		public string LaALL_COMP
		{
			get { return _laALL_COMP; }
			set
			{
				_laALL_COMP = value;
				RaisePropertyChanged("LaALL_COMP");
			}
		}
		#endregion
		#region Data - 預計出車時段
		private string _laCHECKOUT_TIME;
		public string LaCHECKOUT_TIME
		{
			get { return _laCHECKOUT_TIME; }
			set
			{
				_laCHECKOUT_TIME = value;
				RaisePropertyChanged("LaCHECKOUT_TIME");
			}
		}
		#endregion
		#region Data - 溫層
		private string _laTMPR_TYPE;
		public string LaTMPR_TYPE
		{
			get { return _laTMPR_TYPE; }
			set
			{
				_laTMPR_TYPE = value;
				RaisePropertyChanged("LaTMPR_TYPE");
			}
		}
		#endregion
		#region Data - 出貨碼頭
		private string _laPIER_CODE;
		public string LaPIER_CODE
		{
			get { return _laPIER_CODE; }
			set
			{
				_laPIER_CODE = value;
				RaisePropertyChanged("LaPIER_CODE");
			}
		}
		#endregion
		#region Data - 顯示訊息
		private string _falseMessage;
		public string FalseMessage
		{
			get { return _falseMessage; }
			set
			{
				_falseMessage = value;
				RaisePropertyChanged("FalseMessage");
			}
		}
		#endregion
		#region Data - 出貨驗證碼
		private string _delvCheckCode;

		public string DelvCheckCode
		{
			get { return _delvCheckCode; }
			set
			{
				Set(() => DelvCheckCode, ref _delvCheckCode, value);
			}
		}
		#endregion

		#region Data - 託運單出貨狀態
		private string _shipColor = "Red";
		public string ShipColor
		{
			get { return _shipColor; }
			set { _shipColor = value; RaisePropertyChanged("ShipColor"); }
		}
		#endregion

		#region 是否顯示出貨驗證碼與上傳出貨驗證碼
		private bool _isShowDelvCheck;

		public bool IsShowDelvCheck
		{
			get { return _isShowDelvCheck; }
			set
			{
				Set(() => IsShowDelvCheck, ref _isShowDelvCheck, value);
			}
		}
		#endregion


		#region 上傳出貨驗證碼檔案路徑
		private string _uploadDelvCheckCodeFilePath;

		public string UploadDelvCheckCodeFilePath
		{
			get { return _uploadDelvCheckCodeFilePath; }
			set
			{
				Set(() => UploadDelvCheckCodeFilePath, ref _uploadDelvCheckCodeFilePath, value);
			}
		}
		#endregion



		private List<NameValuePair<string>> _tmprTyprList;

		public List<NameValuePair<string>> TmprTyprList
		{
			get { return _tmprTyprList; }
			set
			{
				Set(() => TmprTyprList, ref _tmprTyprList, value);
			}
		}


		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}
		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}
		#endregion


		#region 出貨狀態說明
		private string _shipDesc;

		public string ShipDesc
		{
			get { return _shipDesc; }
			set
			{
				Set(() => ShipDesc, ref _shipDesc, value);
			}
		}
		#endregion


		#region 函式
		public P0808020000_ViewModel()
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
			GetDcCodes();
			TmprTyprList = GetBaseTableService.GetF000904List(FunctionCode, "F1980", "TMPR_TYPE", false);
			IsShowDelvCheck = false;
			var proxy = GetModifyQueryProxy<F19Entities>();
			var f1909 = proxy.F1909s.Where(x => x.GUP_CODE == GupCode && x.CUST_CODE == CustCode).FirstOrDefault();
			IsShowDelvCheck = (f1909 != null && f1909.ISDELV_LOADING_CHECKCODE == "1");
		}

		private void ClearControl()
		{
			TxtWMS_ORD_NO = string.Empty;
			LaPICK_TIME = string.Empty;
			LaALL_COMP = string.Empty;
			LaCHECKOUT_TIME = string.Empty;
			LaTMPR_TYPE = string.Empty;
			LaPIER_CODE = string.Empty;
			GroupColor = "Red";
			ShipColor = "Red";
			ShipDesc = string.Empty;
			FalseMessage = string.Empty;
			Package = false;
		}

		private bool CheckPickTime(F050801 chkData)
		{
			var proxy = GetModifyQueryProxy<F05Entities>();
			var f050801s = proxy.F050801s.Where(x => x.DC_CODE.Equals(chkData.DC_CODE)
																						&& x.GUP_CODE.Equals(chkData.GUP_CODE)
																						&& x.CUST_CODE.Equals(chkData.CUST_CODE)
																						&& x.DELV_DATE.Equals(chkData.DELV_DATE)
																						&& x.PICK_TIME.Equals(chkData.PICK_TIME))
																		.AsQueryable().ToList();
			if (f050801s != null && f050801s.Any())
			{
				int chkStatus = 1;
				foreach (var item in f050801s)
				{
					if (item.STATUS == 0)
						chkStatus = 0;
				}
				if (chkStatus == 0)
					return false;
				else
					return true;
			}
			else
				return true;
		}

		private void GetDcCodes()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any())
				SelectDcCode = DcCodes.First().Value;
		}

		/// <summary>
		/// 取得配送商編號,預計出車時段,出貨碼頭
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <param name="pickTime"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns>string[]</returns>
		private string[] GetALL_ID(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string wmsOrdNo)
		{
			string[] _all_ID = new string[3];
			var proxy = GetProxy<F70Entities>();
			var f700102 = proxy.F700102s.Where(
				o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_NO == wmsOrdNo)
				.ToList()
				.FirstOrDefault();
			if (f700102 != null)
			{
				var f700101 = proxy.F700101s.Where(o => o.DC_CODE == f700102.DC_CODE && o.DISTR_CAR_NO == f700102.DISTR_CAR_NO).ToList().FirstOrDefault();
				if (f700101 != null)
				{
					_all_ID[0] = f700101.ALL_ID;
					_all_ID[1] = f700102.TAKE_TIME;
					_all_ID[2] = f700101.PIER_CODE;
				}
			}
			return _all_ID;
		}

		/// <summary>
		/// 取得配送商名稱
		/// </summary>
		/// <param name="_aLL_ID">配送商編號ALL_ID</param>
		/// <returns></returns>
		private string GetALL_COMP(string dcCode, string _aLL_ID)
		{
			string _all_COMP = string.Empty;
			if (!string.IsNullOrWhiteSpace(_aLL_ID))
			{
				var proxy = GetProxy<F19Entities>();
				var qry = proxy.F1947s.Where(x => x.DC_CODE == dcCode && x.ALL_ID.Equals(_aLL_ID)).ToList();
				if (qry != null && qry.Any())
				{
					_all_COMP = qry.First().ALL_COMP;
				}
			}
			return _all_COMP;
		}

		private string GetTMPR_TYPE_COMP(string _tMPR_TYPE)
		{
			if (TmprTyprList == null)
				return null;

			return TmprTyprList.Where(x => x.Value == _tMPR_TYPE).Select(x => x.Name).FirstOrDefault();
		}

		private void GetMessage(int? status, string message = null)
		{
			switch (status)
			{
				case null:
					Package = true;
					PackageColor = "Red";
					FalseMessage = Properties.Resources.P0808020000_WmsOrdNoIsNull;
					ShipColor = "Red";
					//語音
					if (PlaySound)
						PlaySoundHelper.Oo();
					break;
				case 0:
					Package = true;
					PackageColor = "Red";
					FalseMessage = Properties.Resources.P0808020000_PackNotComplete;
					ShipColor = "Red";
					//語音
					if (PlaySound)
						PlaySoundHelper.Oo();
					break;
				case 1:
					Package = false;
					PackageColor = "Green";
					break;
				case 99: //其他錯誤訊息
					Package = true;
					PackageColor = "Red";
					FalseMessage = message;
					ShipColor = "Red";
					//語音
					if (PlaySound)
						PlaySoundHelper.Oo();
					break;
				case 100:   // 部分稽核完成
					Package = (!string.IsNullOrEmpty(message));
					PackageColor = "Green";
					if (!string.IsNullOrEmpty(message))
						FalseMessage = message;
					ShipColor = "Green";
					break;
				default:
					Package = true;
					PackageColor = "Green";
					FalseMessage = Properties.Resources.P0808020000_WmsOrdNoPackError;
					ShipColor = "Green";
					break;
			}
			switch (ShipColor)
			{
				case "Green":
					ShipDesc = Properties.Resources.P0808020000_CanShip;
					break;
				case "Red":
					ShipDesc = Properties.Resources.P0808020000_CanNotShip;
					break;
			}
		}
		private void SetData(F055001 f055001)
		{
			if (f055001 != null && !string.IsNullOrWhiteSpace(f055001.WMS_ORD_NO))
			{
				//取得出貨單資料
				var proxy = GetProxy<F05Entities>();
				var qry = proxy.F050801s.Where(x => x.WMS_ORD_NO.Equals(f055001.WMS_ORD_NO) && x.DC_CODE == f055001.DC_CODE
																				&& x.GUP_CODE == f055001.GUP_CODE && x.CUST_CODE == f055001.CUST_CODE).ToList();
				ClearControl();
				if (qry != null && qry.Any())
				{
					var selData = qry.First();
					//出貨單號
					TxtWMS_ORD_NO = selData.WMS_ORD_NO;
					//出貨驗證碼
					DelvCheckCode = selData.DELV_CHECKCODE;
					//該出貨單包裝狀態
					GetMessage(int.Parse(selData.STATUS.ToString()));
					if (selData.STATUS >= 1)
					{
						if (selData.STATUS == 1)
							DoUpdate(selData, f055001);
						//批次時段
						LaPICK_TIME = selData.PICK_TIME;
						//配送商編號,預計出車時段,出貨碼頭
						string[] getDetail = GetALL_ID(selData.DC_CODE, selData.GUP_CODE, selData.CUST_CODE, selData.DELV_DATE, selData.PICK_TIME, selData.WMS_ORD_NO);
						//配送商
						LaALL_COMP = GetALL_COMP(selData.DC_CODE, getDetail[0]);
						//預計出車時段
						LaCHECKOUT_TIME = getDetail[1];
						//溫層
						LaTMPR_TYPE = GetTMPR_TYPE_COMP(selData.TMPR_TYPE);
						//出貨碼頭
						LaPIER_CODE = getDetail[2];
						//檢查批次時段燈號
						GroupColor = CheckPickTime(selData) ? "Green" : "Red";
					}
				}
				else
				{
					GetMessage(null);
				}
			}
			else
				ClearControl();

		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => SearchComplate()
					);
			}
		}

		private void DoSearch()
		{
			//語音
			if (PlaySound)
				PlaySoundHelper.Scan();
			//執行查詢動作
			if (!string.IsNullOrWhiteSpace(TxtPAST_NO) && BeginDelvDate != null && EndDelvDate != null)
			{
				var proxy = GetProxy<F05Entities>();
				var qry = proxy.F055001s.Where(x => x.PAST_NO.Equals(TxtPAST_NO) && x.DELV_DATE >= BeginDelvDate && x.DELV_DATE <= EndDelvDate
										&& x.DC_CODE == SelectDcCode && x.GUP_CODE == GupCode && x.CUST_CODE == CustCode).OrderByDescending(o => o.DELV_DATE).ToList();
				if (qry != null && qry.Any())
				{
					var f055001 = qry.First();
					f055001.STATUS = "1";
					f055001.AUDIT_DATE = DateTime.Now;  // 裝車稽核須更新F055001稽核時間
					proxy.UpdateObject(f055001);
					proxy.SaveChanges();
					SetData(f055001);
				}
				else
				{
					ClearControl();
					GetMessage(null);
				}
			}
			else
				ClearControl();
		}

		private void SearchComplate()
		{
			OnSearchCommon();
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel()
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作

		}
		#endregion Cancel

		#region Update
		private void DoUpdate(F050801 SelectedData, F055001 f055001)
		{
			var proxyEx = GetExProxy<P08ExDataSrv.P08ExDataSource>();
			var results = proxyEx.CreateQuery<P08ExDataSrv.ExecuteResult>("SetWmsOrdAudited")
				.AddQueryExOption("wmsOrdNo", SelectedData.WMS_ORD_NO)
				.AddQueryExOption("gupCode", SelectedData.GUP_CODE)
				.AddQueryExOption("custCode", SelectedData.CUST_CODE)
				.AddQueryExOption("dcCode", SelectedData.DC_CODE)
								.AddQueryExOption("pastNo", f055001.PAST_NO).ToList();
			var result = results.FirstOrDefault();
			if (result != null)
			{
				GetMessage(result.IsSuccessed ? 100 : 99, result.Message);
			}
		}
		#endregion


		#region UpLoadDelvCheckCode
		/// <summary>
		/// 上傳出貨驗證碼
		/// </summary>
		public ICommand UpLoadDelvCheckCodeCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoUpLoadDelvCheckCode(), () => UserOperateMode == OperateMode.Query && IsShowDelvCheck
);
			}
		}

		public void DoUpLoadDelvCheckCode()
		{
			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(UploadDelvCheckCodeFilePath, ref errorMeg, 0);
			if (!string.IsNullOrEmpty(errorMeg))
			{
				UploadDelvCheckCodeFilePath = null;
				ShowWarningMessage(errorMeg);
				return;
			}
			var list = new List<wcf.UploadDelvCheckCode>();
			if (excelTable.Columns.Count == 0 || excelTable.Rows.Count == 0)
			{
				UploadDelvCheckCodeFilePath = null;
				ShowWarningMessage(Properties.Resources.P0808020000_UpLoadExcelIsNull);
				return;
			}
			if (excelTable.Columns.Count != 2)
			{
				UploadDelvCheckCodeFilePath = null;
				ShowWarningMessage(Properties.Resources.P0808020000_UpLoadExcelFormatError);
				return;
			}
			foreach (DataRow row in excelTable.Rows)
			{
				list.Add(new wcf.UploadDelvCheckCode
				{
					CONSIGN_NO = row[0]?.ToString().Trim(),
					DELV_CHECKCODE = row[1]?.ToString().Trim()
				});
			}
			var proxy = new wcf.P08WcfServiceClient();
			var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UploadDelvCheckCode(SelectDcCode, GupCode, CustCode, list.ToArray()));
			if (wcfResult.IsSuccessed)
				ShowInfoMessage(Properties.Resources.P0808020000_UpLoadSuccess);
			else
				ShowWarningMessage(wcfResult.Message);
		}
		#endregion UpLoadDelvCheckCode

		#endregion

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
