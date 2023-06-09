using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P16WcfService;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1602030000_ViewModel : InputViewModelBase
	{

		public Action OnSearchComplete = delegate { };
		public Action OpenDeviceWindow = delegate { };
    public Action DoFocustxtMaxSheetNum = delegate { };
    #region 共用參數
    public string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		public string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		private string shareFolderTemp { get { return ConfigurationManager.AppSettings["ShareFolderTemp"]; } }

    /// <summary>
    /// 最大可申請宅單數
    /// </summary>
    private const int MaxSheetNum = 100;

    #endregion
    public P1602030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}

		}

		private void InitControls()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
			{
				SelectedDc = DcList.First().Value;
			}
		}

		#region 物流中心
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
				Set(() => SelectedDc, ref _selectedDc, value);
			}
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
		#endregion

		#region 廠退出貨單號
		private string _wmsOrdNo;
		public string WmsOrdNo
		{
			get { return _wmsOrdNo; }
			set
			{
				Set(() => WmsOrdNo, ref _wmsOrdNo, value);
			}
		}
		#endregion

		#region 顯示/隱藏已扣帳
		private Visibility _debitedVisibility = Visibility.Hidden;
		public Visibility DebitedVisibility
		{
			get { return _debitedVisibility; }
			set
			{
				Set(() => DebitedVisibility, ref _debitedVisibility, value);
			}
		}
		#endregion

		#region 廠退作業與影資系統整合
		private F0003 _f0003;
		public F0003 F0003
		{
			get { return _f0003; }
			set { Set(() => F0003, ref _f0003, value); }
		}
		#endregion

		#region Device設定
		private F910501 _f910501;
		public F910501 F910501
		{
			get { return _f910501; }
			set { Set(() => F910501, ref _f910501, value); }
		}
		#endregion

		#region 查詢結果
		#region 廠商編號
		private string _vnrCode;
		public string VnrCode
		{
			get { return _vnrCode; }
			set
			{
				Set(() => VnrCode, ref _vnrCode, value);
			}
		}

		#endregion

		#region 廠商名稱
		private string _vnrName;
		public string VnrName
		{
			get { return _vnrName; }
			set
			{
				Set(() => VnrName, ref _vnrName, value);
			}
		}
		#endregion

		#region 出貨方式
		// 出貨方式清單
		public List<NameValuePair<string>> DeliveryWayList { get { return GetBaseTableService.GetF000904List(FunctionCode, "F160201", "DELIVERY_WAY", false); } }
		// 所選擇的出貨方式
		private string _selectedDeliveryWay;
		public string SelectedDeliveryWay
		{
			get { return _selectedDeliveryWay; }
			set
			{
				Set(() => SelectedDeliveryWay, ref _selectedDeliveryWay, value);
				if (ComfirmEnable)
				{
					if (_selectedDeliveryWay == "0")
					{
						EnableAllId = false;
						EnableMaxSheetNum = false;
						SelectedAllId = null;
						SelectedMaxSheetNum = "1";
						EnableMemo = true;
					}
					else
					{
						EnableAllId = ComfirmEnable ? true : false;
						EnableMaxSheetNum = ComfirmEnable ? true : false;
						EnableMemo = false;
						Memo = string.Empty;
					}
					ReturnDetailButtonEnable = false;
					HomeDeliveryButtonEnable = false;
				}
				else
				{
					EnableAllId = false;
					EnableMaxSheetNum = false;
					EnableMemo = false;
					if(F160204Data.PROC_FLAG == "3")
					{
						ReturnDetailButtonEnable = true;
						HomeDeliveryButtonEnable = SelectedDeliveryWay == "1" ? true : false;
					}
					else
					{
						ReturnDetailButtonEnable = false;
						HomeDeliveryButtonEnable = false;
					}
				}
				
			}
		}
		#endregion

		#region 備註/自取訊息
		private string _memo;
		public string Memo
		{
			get { return _memo; }
			set
			{
				Set(() => Memo, ref _memo, value);
			}
		}

		private bool _enableMemo;
		public bool EnableMemo
		{
			get { return _enableMemo; }
			set
			{
				Set(() => EnableMemo, ref _enableMemo, value);
			}
		}
		#endregion

		#region 宅配商
		// 宅配商清單
		public List<NameValuePair<string>> AllIdList {
			get { return GetAllIdList(); }
		}
		// 所選擇的出貨方式
		private string _selectedAllId;
		public string SelectedAllId
		{
			get { return _selectedAllId; }
			set
			{
				Set(() => SelectedAllId, ref _selectedAllId, value);
			}
		}
		// Enable/Disable宅配商
		private bool _enableAllId;
		public bool EnableAllId
		{
			get { return _enableAllId; }
			set
			{
				Set(() => EnableAllId, ref _enableAllId, value);
			}
		}
		#endregion

		#region 宅配單張數
		// 宅配單張數清單
		//public List<NameValuePair<string>> MaxSheetNumList { get { return GetBaseTableService.GetF000904List(FunctionCode, "F160204", "MAX_SHEET_NUM", false).OrderBy(x => Convert.ToInt32(x.Value)).ToList(); } }

		// 所選擇的宅配單張數
		private string _selectedMaxSheetNum;
		public string SelectedMaxSheetNum
		{
			get { return _selectedMaxSheetNum; }
			set
			{
				Set(() => SelectedMaxSheetNum, ref _selectedMaxSheetNum, value);
			}
		}

		// Enable/Disable宅配單張數
		private bool _enableMaxSheetNum;
		public bool EnableMaxSheetNum
		{
			get { return _enableMaxSheetNum; }
			set
			{
				Set(() => EnableMaxSheetNum, ref _enableMaxSheetNum, value);
			}
		}
		#endregion

		#endregion

		#region 補印廠退明細表
		public void PrintReturnDetail(bool isReprint)
		{
			var proxyWcf = new wcf.P16WcfServiceClient();
			var vendorReturnOrderDetailFileResult = RunWcfMethod<wcf.ExecuteLmsPdfApiResult>(proxyWcf.InnerChannel,
					() => proxyWcf.GetVendorReturnOrderDetailFile(SelectedDc, _gupCode, _custCode, WmsOrdNo, isReprint?1:0, null));
			if (!vendorReturnOrderDetailFileResult.IsSuccessed)
			{
				DialogService.ShowMessage($"TMS回傳:{vendorReturnOrderDetailFileResult.Message}");
				return;
			}

			//設定第一台印表機
			var _proxy = ConfigurationHelper.GetProxy<F91Entities>(false, FunctionCode);
			var existF910501 = _proxy.F910501s.Where(x => x.DEVICE_IP == Wms3plSession.Get<GlobalInfo>().ClientIp && x.DC_CODE == SelectedDc).ToList();
			if(existF910501 == null || string.IsNullOrWhiteSpace(existF910501.FirstOrDefault()?.PRINTER))
			{
				DialogService.ShowMessage(Properties.Resources.P1602030000_VendorReturnDetailNotSetPrint);
				return;
			}
			string printName = existF910501.FirstOrDefault().PRINTER;

			
			printPdf(vendorReturnOrderDetailFileResult.Data, printName, "RtnDetail", WmsOrdNo, isReprint);
		}
		#endregion

		#region 補印宅配單
		public void PrintHomeDeliveryOrder(bool isReprint)
		{
			var proxyWcf = new wcf.P16WcfServiceClient();
			var vendorReturnOrderFile = RunWcfMethod<wcf.ExecuteLmsPdfApiResult>(proxyWcf.InnerChannel,
				() => proxyWcf.GetVendorReturnOrderFile(SelectedDc, _gupCode, _custCode, WmsOrdNo, isReprint ? 1 : 0, null));
			if (!vendorReturnOrderFile.IsSuccessed)
			{
				DialogService.ShowMessage($"TMS回傳:{vendorReturnOrderFile.Message}");
				return;
			}

			// 設定印表機為印表機2
			
			var _proxy = ConfigurationHelper.GetProxy<F91Entities>(false, FunctionCode);
			var existF910501 = _proxy.F910501s.Where(x => x.DEVICE_IP == Wms3plSession.Get<GlobalInfo>().ClientIp && x.DC_CODE == SelectedDc).ToList();
			if (existF910501 == null || string.IsNullOrWhiteSpace(existF910501.FirstOrDefault()?.MATRIX_PRINTER))
			{
				DialogService.ShowMessage(Properties.Resources.P1602030000_VendorReturnNotSetPrint);
				return;
			}
			string printName = existF910501.FirstOrDefault().MATRIX_PRINTER;

			printPdf(vendorReturnOrderFile.Data, printName,"DeliveryOrder", WmsOrdNo, isReprint);
		}
		#endregion

		#region
		private F160204Data _f160204Data;
		public F160204Data F160204Data
		{
			get { return _f160204Data; }
			set
			{
				Set(() => F160204Data, ref _f160204Data, value);
			}
		}
		#endregion
		public ICommand ComfirmCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => Comfirm()
					);
			}
		}

		#region 出貨確認
		public void Comfirm()
		{
			if (!CheckData()) return;
			

			if (F160204Data.PROC_FLAG == "2")
			{
				var proxyWcf = new wcf.P16WcfServiceClient();
				var res = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
						() => proxyWcf.ApplyVendorReturnOrder(SelectedDc, _gupCode, _custCode, WmsOrdNo, SelectedDeliveryWay, SelectedAllId, SelectedMaxSheetNum, Memo));
				if (!res.IsSuccessed)
				{
					DialogService.ShowMessage($"TMS回傳:{res.Message}");
					return;
				}

				// 當F0003.SYS_PATH = 1且為宅配才呼叫
				if (F0003?.SYS_PATH == "1" && SelectedDeliveryWay == "1")
				{

					if (string.IsNullOrWhiteSpace(res.No))
					{
						ShowWarningMessage($"LMS回覆: 取得宅配單成功，但沒有回傳宅配單號");
					}

					var proxy = GetWcfProxy<wcf.P16WcfServiceClient>();
					var result = proxy.RunWcfMethod(w => w.SealingInfoAsync(SelectedDc, _gupCode, _custCode, new wcf.SealingInfoAsyncReq
					{
						WhId = SelectedDc,
						OutboundNo = WmsOrdNo,
						WorkStationId = F910501?.WORKSTATION_CODE,
						OperationUserId = Wms3plSession.Get<UserInfo>().Account,
						ShipNo = res.No,
						TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
					}));
					if (!result.IsSuccessed)
					{
						ShowWarningMessage(result.Message);
					}
				}


				var proxyEx = GetExProxy<P16ExDataSource>();
				proxyEx.CreateQuery<ExecuteResult>("UpdateF160204Data")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("wmsOrdNo", WmsOrdNo)
					.AddQueryExOption("deliveryWay", SelectedDeliveryWay)
					.AddQueryExOption("allId", SelectedAllId)
					.AddQueryExOption("procFlag", "3")
					.AddQueryExOption("sheetNum", SelectedMaxSheetNum)
					.AddQueryExOption("memo", Memo)
					.ToList();

				// 列印廠退出貨明細
				PrintReturnDetail(false);

				if (SelectedDeliveryWay == "1")
				{
					//列印廠退宅配單
					PrintHomeDeliveryOrder(false);
				}
			}

			SearchCommand.Execute(null);
		}
		#endregion

		#region 出貨確認啟用
		private bool _comfirmEnable = false;
		public bool ComfirmEnable
		{
			get { return _comfirmEnable; }
			set
			{
				Set(() => ComfirmEnable, ref _comfirmEnable, value);
			}
		}
		#endregion

		#region 控制補印廠退明細表和補印宅配單

		private bool _returnDetailButtonEnable = false;
		public bool ReturnDetailButtonEnable
		{
			get { return _returnDetailButtonEnable; }
			set
			{
				Set(() => ReturnDetailButtonEnable, ref _returnDetailButtonEnable, value);
			}
		}

		#endregion

		#region 控制補印宅配單
		private bool _homeDeliveryButtonEnable = false;
		public bool HomeDeliveryButtonEnable
		{
			get { return _homeDeliveryButtonEnable; }
			set
			{
				Set(() => HomeDeliveryButtonEnable, ref _homeDeliveryButtonEnable, value);
			}
		}
		#endregion


		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			//語音
			if (PlaySound)
				PlaySoundHelper.Scan();
			//執行查詢動作
			WmsOrdNo = WmsOrdNo.ToUpper();
			if (!string.IsNullOrWhiteSpace(WmsOrdNo))
			{
				GetF160204();
				if (F160204Data == null)
				{
					ShowMessage(Messages.InfoNoData);
					return;
				}
			
				switch (F160204Data.PROC_FLAG)
				{
					case "0":
						ComfirmEnable = false;
						DialogService.ShowMessage(Properties.Resources.P1602030000_NotYetPackaged);
						DebitedVisibility = Visibility.Hidden;
						break;
					case "1":
						ComfirmEnable = false;
						DialogService.ShowMessage(Properties.Resources.P1602030000_NotYetPackaged);
						DebitedVisibility = Visibility.Hidden;
						EnableMemo = false;
						break;
					case "2":
						ComfirmEnable = true;
						DebitedVisibility = Visibility.Hidden;
						break;
					case "3":
						ComfirmEnable = false;
						DebitedVisibility = Visibility.Visible;
						EnableMemo = false;
						break;
					default:
						ComfirmEnable = false;
						EnableMemo = false;
						DebitedVisibility = Visibility.Hidden;
						break;

				}
				VnrCode = F160204Data.VNR_CODE;
				VnrName = F160204Data.VNR_NAME;
				SelectedDeliveryWay = F160204Data.DELIVERY_WAY;
				SelectedAllId = SelectedDeliveryWay == "0" ? null : F160204Data.ALL_ID;
				SelectedMaxSheetNum = SelectedDeliveryWay == "0" ? "1" : F160204Data.SHEET_NUM.ToString();
        if (string.IsNullOrWhiteSpace(SelectedMaxSheetNum))
          SelectedMaxSheetNum = "1";
        Memo = F160204Data.MEMO;
				if(F0003?.SYS_PATH == "1")
				{
					var proxy = GetWcfProxy<wcf.P16WcfServiceClient>();
					var result = proxy.RunWcfMethod(w => w.DistibuteInfoAsync(SelectedDc, _gupCode, _custCode, new wcf.DistibuteInfoAsyncReq
					{
						WhId = SelectedDc,
						OutboundNo = WmsOrdNo,
						WorkStationId = F910501?.WORKSTATION_CODE,
						OperationUserId = Wms3plSession.Get<UserInfo>().Account,
						TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
					}));
					if (!result.IsSuccessed)
					{
						ShowWarningMessage(result.Message);
					}
				}
			}
		}

		public void GetF160204()
		{
			var proxyEx = GetExProxy<P16ExDataSource>();
			F160204Data = proxyEx.CreateQuery<F160204Data>("GetF160204Data")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("wmsOrdNo", WmsOrdNo).ToObservableCollection().FirstOrDefault();
		}

		private bool CheckData()
		{
			bool result = true;
			string message = "";
			if (string.IsNullOrWhiteSpace(SelectedDeliveryWay))
			{
				message += message.Length > 0 ? Environment.NewLine : string.Empty;
				message += Properties.Resources.P1602030000_PleaceSelectedDeliveryWay;
			}
			else if (SelectedDeliveryWay != "0")
			{
				if (string.IsNullOrWhiteSpace(SelectedAllId))
				{
					message += message.Length > 0 ? Environment.NewLine : string.Empty;
					message += Properties.Resources.P1602030000_PleaceSelectedAllId;

				}
				if (string.IsNullOrWhiteSpace(SelectedMaxSheetNum))
				{
					message += message.Length > 0 ? Environment.NewLine : string.Empty;
					message += "請輸入宅配單張數";

				}
			}
			else if(SelectedDeliveryWay == "0" && string.IsNullOrWhiteSpace(Memo))
			{
				message += message.Length > 0 ? Environment.NewLine : string.Empty;
				message += Properties.Resources.P1602030000_PleaseEnterMomo;
			}

      var chkMaxSheetNumRes = DoCheckMaxSheetNum();
      if(!chkMaxSheetNumRes.IsSuccessed)
      {
        message += message.Length > 0 ? Environment.NewLine : string.Empty;
        message += chkMaxSheetNumRes.Message;
      }

      if (message.Length > 0)
			{
				DialogService.ShowMessage(message);
				result = false;
			}
			return result;
		}

		private void DoSearchComplete()
		{

			OnSearchComplete();
		}

		private void printPdf(byte[] pdfData,string printName,string type,string fileName,bool isReprint) {
			var folder = Path.Combine(shareFolderTemp, type);
			// Log資料夾路徑
			var logFolder = Path.Combine(shareFolderTemp, type, "log");
			Int16 copies = 1;
			//存檔路徑不存在新增
			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
			var filePath = $"{folder}\\{fileName}.pdf";
			if (File.Exists(filePath))
			{
				try
				{
					File.Delete(filePath);
				}
				catch (Exception ex)
				{
					// 檢查log存檔路徑不存在新增
					if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
					// 建立log
					using (var stream = new StreamWriter($"{logFolder}\\{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.log"))
					{
						stream.WriteLine(ex);
					}

					// 刪除失敗，重新建立檔案
					fileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
					filePath = $"{Path.Combine(shareFolderTemp, type)}\\{fileName}.pdf";
					
				}
			}
				

			//File.WriteAllBytes(filePath, pdfData);

			using (var stream = new FileStream(filePath, FileMode.CreateNew))
			{
				// 將檔案儲存在目標路徑
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					writer.Write(pdfData, 0, pdfData.Length);
				}
			}

			var printPdfService = new PrintPdfService();
			//列印兩張廠退明細
			if (!isReprint && type == "RtnDetail")
			{
				copies = 2;
			}
			// 列印PDF
			printPdfService.Print(pdfData, FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, SelectedDc, printName, copies);

			// 刪除舊的pdf檔案(非此廠退出貨單的檔案)
		  foreach(var file in Directory.GetFiles(folder))
			{
				var fileInfo = new FileInfo(file);
				var newFileName = fileInfo.Name.Replace(".pdf","");
				if (newFileName != fileName)
				{
					try
					{
						if(File.Exists(file))
							File.Delete(file);
					}
					catch (Exception ex)
					{
						// 檢查log存檔路徑不存在新增
						if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
						// 建立log
						using (var stream = new StreamWriter($"{ logFolder }\\{newFileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.log"))
						{
							stream.WriteLine(ex);
						}
					}
				}
			}

			DialogService.ShowMessage("列印成功");
		}

		// 取得宅配商清單
		public List<NameValuePair<string>> GetAllIdList()
		{
			var proxy = ConfigurationHelper.GetProxy<F00Entities>(false, FunctionCode);
			return proxy.F0002s.Where(x =>  x.DC_CODE == SelectedDc &&
											x.IS_VENDOR_RETURN == "1")
							   .Select(x => new NameValuePair<string> {
									Name = x.LOGISTIC_NAME,
									Value = x.LOGISTIC_CODE
								}).ToList();
		}

		public void ShowErrorWorkstationCode()
		{
			ShowWarningMessage("工作站編號必須是F開頭，請重新設定裝置");
		}

    public ICommand CheckMaxSheetNumCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => { },
          completed: o =>
          {
            var res = DoCheckMaxSheetNum();
            if (!res.IsSuccessed)
            {
              //DoFocustxtMaxSheetNum();
              ShowWarningMessage(res.Message);
            }
          });
      }
    }

    public ExecuteResult DoCheckMaxSheetNum()
    {
      //宅配商出貨才需檢查
      if (SelectedDeliveryWay != "1")
        return new ExecuteResult { IsSuccessed = true };

      var value = 0;
      if (!int.TryParse(SelectedMaxSheetNum, out value))
        return new ExecuteResult { IsSuccessed = false, Message = "請輸入正確的數值" };

      if (value < 1 || value > MaxSheetNum)
        return new ExecuteResult { IsSuccessed = false, Message = "宅配單張數請輸入1-100之數值" };

      return new ExecuteResult { IsSuccessed = true };
    }

  }
}
