using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrystalDecisions.CrystalReports.Engine;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.LabelPrinter.Bartender;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Common.Helpers;

namespace Wms3pl.WpfClient.P08.Services
{
	public partial class DeliveryReportService:Wms3plWindow
	{
    public Action OnReprintClicked = delegate { };
    private string _functionCode;

		public DeliveryReportService(string functionCode)
		{
			_functionCode = functionCode;
		}

		private Func<Action<object>, Func<bool>, Action<object>, Action<Exception>, Action, AsyncDelegateCommand>
				_createBusyAsyncCommand = null;

		private AsyncDelegateCommand CreateBusyAsyncCommand(Action<object> action,
				Func<bool> canExecute = null,
				Action<object> completed = null,
				Action<Exception> error = null,
				Action preAction = null)
		{
			return _createBusyAsyncCommand(action, canExecute, completed, error, preAction);
		}

		private T GetProxyConnect<T>() where T : DataServiceContext
		{
			return ConfigurationHelper.GetProxy<T>(false, _functionCode);
		}

		private T GetExProxyConnect<T>() where T : DataServiceContext
		{
			return ConfigurationExHelper.GetExProxy<T>(false, _functionCode);
		}

		#region 產生列印表單按鈕
		/// <summary>
		/// 產生列印表單按鈕
		/// </summary>
		/// <param name="deliveryData">出貨單</param>		
		/// <param name="isSingleBox">是否為單箱列印表單</param>
		/// <param name="prevDlvData">出貨刷讀明細</param>
		/// <param name="f055001Data">包裝主檔資料</param>
		/// <param name="f910501Data">列印Device設定</param>
		/// <param name="createBusyAsyncCommand">按鈕物件</param>
		/// <param name="f050301Datas">訂單資料</param>
		/// <param name="showPrintInvoice">是否產生列印發票按鈕</param>
		/// <returns></returns>
		public ObservableCollection<DynamicButtonData> BindingReportButton(F050801 deliveryData,
						F055001 f055001Data, F910501 f910501Data, F1947 f1947Data, F1909 f1909,
						Func<Action<object>, Func<bool>, Action<object>, Action<Exception>, Action, AsyncDelegateCommand>
								createBusyAsyncCommand, List<F050301> f050301Datas = null, bool showPrintInvoice = true)
		{
			_createBusyAsyncCommand = createBusyAsyncCommand;

			var proxy = GetProxyConnect<F19Entities>();

			// 當在輸入單號時就判斷到已完成包裝時, 這時要把所有可以印出來的箱明細都列出來			
			var tmpReportList = new List<DynamicButtonData>();
			var p08Proxy = GetExProxyConnect<P08ExDataSource>();
			var delvdtlInfo = p08Proxy.GetDelvdtlInfo(deliveryData.DC_CODE, deliveryData.GUP_CODE, deliveryData.CUST_CODE, deliveryData.WMS_ORD_NO).ToList().FirstOrDefault();
      

      //補印箱明細，如果F050801.SHIP_MODE=3(外部出貨包裝紀錄)，不顯示補印按鈕
      if (deliveryData.SHIP_MODE == "3")
        return new ObservableCollection<DynamicButtonData>();

      if (delvdtlInfo.SOURCE_TYPE == "13")
			{
				List<LittleWhiteReport> littleWhiteReport = null;
				tmpReportList.Add(
				new DynamicButtonData(Properties.Resources.DeliveryReportService_RtnLableReport, CreateBusyAsyncCommand(
							o => littleWhiteReport = p08Proxy.GetLittleWhiteReport(deliveryData.DC_CODE, deliveryData.GUP_CODE, deliveryData.CUST_CODE, deliveryData.WMS_ORD_NO).ToList(),
							() => true,
              o =>
              {
                p08Proxy.LogPrintBoxDetailPacking(f055001Data.DC_CODE, f055001Data.GUP_CODE, f055001Data.CUST_CODE, f055001Data.WMS_ORD_NO, f055001Data.PACKAGE_BOX_NO.ToString());
                PrintRtnLable(f910501Data, littleWhiteReport);
                OnReprintClicked();
              }), 
              "BP0807010051"));
      }
      else
			{
				bool isSingleBox = f055001Data != null ;
				if (!isSingleBox)
				{
					// 產生按鈕
					if (f1909.ISPRINTDELVDTL == "1")
					{
						List<DeliveryReport> boxDetailList = null;

						// PcHome
						var pcHomeData = p08Proxy.GetPcHomeDelivery(deliveryData.DC_CODE, deliveryData.GUP_CODE, deliveryData.CUST_CODE, deliveryData.WMS_ORD_NO).FirstOrDefault();

            tmpReportList.Add(
            new DynamicButtonData(Properties.Resources.DeliveryReportService_BoxDetailReport, CreateBusyAsyncCommand(
                //產生箱明細資料
                o => boxDetailList = p08Proxy.GetDeliveryReport(deliveryData.DC_CODE, deliveryData.GUP_CODE, deliveryData.CUST_CODE, deliveryData.WMS_ORD_NO, null).ToList(),
                () => true,
                o =>
                {
                  p08Proxy.LogPrintBoxDetailPacking(f055001Data.DC_CODE, f055001Data.GUP_CODE, f055001Data.CUST_CODE, f055001Data.WMS_ORD_NO, f055001Data.PACKAGE_BOX_NO.ToString());
                  PrintBoxData(boxDetailList, f910501Data, f050301Datas, f1909, delvdtlInfo, pcHomeData);
                  OnReprintClicked();
                }),
                "BP0807010050"));
          }
        }
        else
				{
					// 產生按鈕

					List<DeliveryReport> boxDetailList = null;
					if (f1909.ISPRINTDELVDTL == "1")
					{
						// PcHome
						var pcHomeData = p08Proxy.GetPcHomeDelivery(deliveryData.DC_CODE, deliveryData.GUP_CODE, deliveryData.CUST_CODE, deliveryData.WMS_ORD_NO).FirstOrDefault();

						tmpReportList.Add(
						new DynamicButtonData(Properties.Resources.DeliveryReportService_BoxDetailReport, CreateBusyAsyncCommand(
								o => boxDetailList = p08Proxy.GetDeliveryReport(f055001Data.DC_CODE, f055001Data.GUP_CODE, f055001Data.CUST_CODE, f055001Data.WMS_ORD_NO, f055001Data.PACKAGE_BOX_NO).ToList(),
								() => true,
                o =>
                {
                  p08Proxy.LogPrintBoxDetailPacking(f055001Data.DC_CODE, f055001Data.GUP_CODE, f055001Data.CUST_CODE, f055001Data.WMS_ORD_NO, f055001Data.PACKAGE_BOX_NO.ToString());
                  PrintBoxData(boxDetailList, f910501Data, f050301Datas, f1909, delvdtlInfo, pcHomeData);
                  OnReprintClicked();
                }),
                "BP0807010050"));
          }
        }
      }
			return tmpReportList.ToObservableCollection();
		}
		#endregion

		#region 執行列印報表

		private void PrintReport(ReportClass report, F910501 device, PrinterType printerType = PrinterType.A4)
		{
			CrystalReportService crystalReportService;
			if (device == null)
			{
				crystalReportService = new CrystalReportService(report);
				crystalReportService.ShowReport(null, PrintType.Preview);
			}
			else
			{
				crystalReportService = new CrystalReportService(report, device);
				crystalReportService.PrintToPrinter(printerType);
			}
		}

		#endregion

		#region 託運單
		public Wms3pl.WpfClient.ExDataServices.ShareExDataService.ExecuteResult PrintData(F050801 deliveryData, List<F055001Data> datas, F910501 selectedF910501, F194704 f194704Data)
		{
			if (datas.Any())
			{
				var proxy = GetProxyConnect<F19Entities>();
				var f194713 = proxy.F194713s.Where(x => x.DC_CODE == deliveryData.DC_CODE && x.GUP_CODE == deliveryData.GUP_CODE && x.CUST_CODE == deliveryData.CUST_CODE && x.ALL_ID == deliveryData.ALL_ID && x.ESERVICE == datas.First().ESERVICE).FirstOrDefault();
				var reportNameSpaceFormat = "Wms3pl.WpfClient.P08.Report.{0}";
				var reportName = string.Format("RP0807010010_{0}_Default.rpt", deliveryData.ALL_ID);
				if (f194713 != null && !string.IsNullOrWhiteSpace(f194713.DELV_FORMAT))
					reportName = f194713.DELV_FORMAT;

				var itemData = GetConsignItemData(deliveryData);
				switch (deliveryData.ALL_ID)
				{
					case "711":
					case "FAMILY":
						//var report1 = Activator.CreateInstance(Type.GetType(string.Format(reportNameSpaceFormat, reportName.Replace(".rpt", "")))) as ReportClass;
						//report1.Load(reportName);
						var info = Wms3plSession.Get<GlobalInfo>();
						var lang = string.Empty;
						if (info != null && !string.IsNullOrEmpty(info.Lang) && info.Lang.ToUpper() != "ZH-TW")
							lang = info.Lang;

						var type = Type.GetType(string.Format(reportNameSpaceFormat, reportName.Replace(".rpt", "")));
						var reportFullTypeName = $"{type.FullName}{lang.Replace("-", "_")},{type.Assembly.FullName}";
						var reportFileName = $"{type.Name}.rpt";
						if (!string.IsNullOrEmpty(lang))
							reportFileName = $"{type.Name}{lang.Replace("-", "_")}.rpt";

						var report1 = Activator.CreateInstance(Type.GetType(reportFullTypeName)) as ReportClass;
						report1.Load(reportFileName);
						report1.SetDataSource(datas);
						PrintReport(report1, selectedF910501, PrinterType.Matrix);
						break;
					case "HCT":
						//var report2 = new RP0807010010_HCT();
						//report2.Load("RP0807010010_HCT.rpt");
						var report2 = ReportHelper.CreateAndLoadReport<RP0807010010_HCT>();
						report2.SetDataSource(datas);
						PrintReport(report2, selectedF910501);
						break;
					case "KTJ":
						var report8 = ReportHelper.CreateAndLoadReport<RP0807010010_KTJ>();
						report8.SetDataSource(datas);
						PrintReport(report8, selectedF910501);
						break;
					case "TCAT":
						if (f194704Data != null && f194704Data.CONSIGN_FORMAT == "01") //熱感式
						{
							//var report4 = new RP0807010010_TCAT();
							//report4.Load("RP0807010010_TCAT.rpt");
							var report4 = ReportHelper.CreateAndLoadReport<RP0807010010_TCAT>();
							report4.SetDataSource(datas);
							PrintReport(report4, selectedF910501);
						}
						else if (f194704Data != null && f194704Data.CONSIGN_FORMAT == "02") //二模
						{

						}
						break;
					case "YUNDA":
						//韵達快遞
						//var report6 = new RP0807010010_YUNDA();
						//report6.Load("RP0807010010_YUNDA.rpt");
						var report6 = ReportHelper.CreateAndLoadReport<RP0807010010_YUNDA>();
						report6.SetDataSource(datas);
						report6.Subreports[0].SetDataSource(itemData);
						PrintReport(report6, selectedF910501);
						break;
					case "STO":
						//申通快遞
						//var report5 = new RP0807010010_STO();
						//report5.Load("RP0807010010_STO.rpt");
						var report5 = ReportHelper.CreateAndLoadReport<RP0807010010_STO>();
						report5.SetDataSource(datas);
						report5.Subreports[0].SetDataSource(itemData);
						PrintReport(report5, selectedF910501);
						break;
					case "CEMS":
						//CEMS快遞
						//var report7 = new RP0807010010_CEMS();
						//report7.Load("RP0807010010_CEMS.rpt");
						var report7 = ReportHelper.CreateAndLoadReport<RP0807010010_CEMS>();
						report7.SetDataSource(datas);
						report7.Subreports[0].SetDataSource(itemData);
						report7.Subreports[1].SetDataSource(itemData);
						PrintReport(report7, selectedF910501);
						break;
				}
			}
			return new ExDataServices.ShareExDataService.ExecuteResult { IsSuccessed = true };
		}
		/// <summary>
		/// 列印托運單
		/// </summary>
		public Wms3pl.WpfClient.ExDataServices.ShareExDataService.ExecuteResult PrintPass(F050801 deliveryData, List<LableItem> labelItems)
		{
			if (!labelItems.Any())
				return new Wms3pl.WpfClient.ExDataServices.ShareExDataService.ExecuteResult { IsSuccessed = false, Message = Properties.Resources.DeliveryReportService_LabelItemsIsNull };

			var printObj = new LabelPrintHelper(_functionCode);
			var result = printObj.DoPrintNoDevice(labelItems, deliveryData.DC_CODE);
			return result;
		}

		public List<LableItem> GetPassLabelItems(F050801 deliveryData, short? packageBoxNo, string consignReport)
		{
			var labelItems = PackingService.PassDataToLabel(GetConsignData(deliveryData, packageBoxNo));
			foreach (var labelItem in labelItems)
			{
				labelItem.LableCode = consignReport;
			}
			return labelItems;
		}

		public string GetConsignReport(F050801 deliveryData)
		{
			var proxy = GetProxyConnect<F19Entities>();
			var f1947 = proxy.CreateQuery<F1947>("GetAllIdByWmsOrdNo")
																	.AddQueryExOption("dcCode", deliveryData.DC_CODE)
																	.AddQueryExOption("gupCode", deliveryData.GUP_CODE)
																	.AddQueryExOption("custCode", deliveryData.CUST_CODE)
																	.AddQueryExOption("wmsOrdNo", deliveryData.WMS_ORD_NO)
																	.ToList().FirstOrDefault();

			if (f1947 == null || string.IsNullOrEmpty(f1947.CONSIGN_REPORT))
			{
				DialogService.ShowMessage(Properties.Resources.P0807010000_ConsignReportNotExist);
				return string.Empty;
			}
			return f1947.CONSIGN_REPORT;
		}

		public List<F055002Data> GetConsignItemData(F050801 deliveryData, string pastNo = null)
		{
			var proxyF08 = GetExProxyConnect<P08ExDataSource>();
			var consignData = proxyF08.CreateQuery<F055002Data>("GetConsignItemData")
					.AddQueryExOption("dcCode", deliveryData.DC_CODE)
					.AddQueryExOption("gupCode", deliveryData.GUP_CODE)
					.AddQueryExOption("custCode", deliveryData.CUST_CODE)
					.AddQueryExOption("wmsOrdNo", deliveryData.WMS_ORD_NO)
					.AddQueryExOption("pastNo", pastNo)
					.ToList();
			return consignData;
		}

		public List<F055001Data> GetConsignData(F050801 deliveryData, short? packageBoxNo = null)
		{
			// 產生託運單資料集
			var proxyF08 = GetExProxyConnect<P08ExDataSource>();
			var consignData = proxyF08.CreateQuery<F055001Data>("GetConsignData")
					.AddQueryExOption("dcCode", deliveryData.DC_CODE)
					.AddQueryExOption("gupCode", deliveryData.GUP_CODE)
					.AddQueryExOption("custCode", deliveryData.CUST_CODE)
					.AddQueryExOption("wmsOrdNo", deliveryData.WMS_ORD_NO)
					.AddQueryExOption("packageBoxNo", packageBoxNo == null ? "" : packageBoxNo.ToString())
					.ToList();

			if (!consignData.Any())
			{
				return new List<F055001Data>();
			}

			var consignDataList = new List<F055001Data>();
			var packageBoxNos = consignData.Select(x => x.PACKAGE_BOX_NO).Distinct().ToList();

			foreach (var curPackageBoxNo in packageBoxNos)
			{
				var printConsignData = new F055001Data();
				var pastData = consignData.FirstOrDefault(x => x.PACKAGE_BOX_NO == curPackageBoxNo);
				if (pastData == null)
					continue;
				printConsignData.PACKAGE_BOX_NO = pastData.PACKAGE_BOX_NO;
				printConsignData.ARRIVAL_DATE = pastData.ARRIVAL_DATE;
				printConsignData.DELV_DATE = pastData.DELV_DATE;
				printConsignData.CUST_ORD_NO = pastData.CUST_ORD_NO;
				printConsignData.COLLECT_AMT = pastData.COLLECT_AMT;
				printConsignData.ERST_NO = pastData.ERST_NO;
				printConsignData.TOTAL_AMOUNT = pastData.TOTAL_AMOUNT;
				//是否代收
				var collect = (pastData.COLLECT == "1") ? Properties.Resources.BillType : Properties.Resources.DeliveryReportService_NotCollect;
				printConsignData.COLLECT = collect;
				printConsignData.CONSIGNEE = pastData.CONSIGNEE;
				printConsignData.ADDRESS = pastData.ADDRESS;
				printConsignData.TEL = pastData.TEL;
				printConsignData.PAST_NO = pastData.PAST_NO;
				printConsignData.SA_QTY = pastData.SA_QTY;
				//備註-有多筆需顯示多行
				var sb = new StringBuilder();
				foreach (var memo in consignData.Where(x => x.PACKAGE_BOX_NO == curPackageBoxNo).Select(x => x.MEMO))
				{
					if (string.IsNullOrEmpty(memo)) continue;
					sb.AppendLine(memo);
				}
				printConsignData.MEMO = sb.ToString();
				printConsignData.SHORT_NAME = pastData.SHORT_NAME;
				printConsignData.CUST_TEL = pastData.CUST_TEL;
				printConsignData.CUST_ADDRESS = pastData.CUST_ADDRESS;
				printConsignData.RETAIL_CODE = pastData.RETAIL_CODE;
				printConsignData.CUST_NAME = pastData.CUST_NAME;
				printConsignData.PRINT_TIME = pastData.PRINT_TIME;
				printConsignData.ROUTE = pastData.ROUTE;
				printConsignData.FIXED_CODE = pastData.FIXED_CODE;
				printConsignData.ADDRESS_TYPE = pastData.ADDRESS_TYPE;
				printConsignData.CONSIGN_ID = pastData.CONSIGN_ID;
				printConsignData.CONSIGN_MEMO = pastData.CONSIGN_MEMO;
				printConsignData.CONSIGN_NAME = pastData.CONSIGN_NAME;

				if (deliveryData.ALL_ID == "711" || deliveryData.ALL_ID == "FAMILY")
				{
					printConsignData.RETAIL_DELV_DATE = pastData.RETAIL_DELV_DATE;
					printConsignData.RETAIL_RETURN_DATE = pastData.RETAIL_RETURN_DATE;
					printConsignData.RETAIL_NAME = pastData.RETAIL_NAME;
					printConsignData.ESERVICE = pastData.ESERVICE;
					printConsignData.ESERVICE_NAME = pastData.ESERVICE_NAME;
					printConsignData.ESHOP = pastData.ESHOP;
					printConsignData.ESHOP_ID = pastData.ESHOP_ID;
					printConsignData.PLATFORM_NAME = pastData.PLATFORM_NAME;
					printConsignData.VNR_NAME = pastData.VNR_NAME;
					printConsignData.CUST_INFO = pastData.CUST_INFO;
					printConsignData.NOTE1 = pastData.NOTE1;
					printConsignData.NOTE2 = pastData.NOTE2;
					printConsignData.NOTE3 = pastData.NOTE3;
					printConsignData.SHOW_ISPAID_NOTE = pastData.SHOW_ISPAID_NOTE;
					printConsignData.IDENTIFIER = pastData.IDENTIFIER;
					printConsignData.INVOICE = pastData.INVOICE;
					printConsignData.INVOICE_DATE = pastData.INVOICE_DATE;
					if (deliveryData.ALL_ID == "711")
						printConsignData.PAST_NOByCode128 = BarcodeConverter128.StringToBarcode(pastData.RETAIL_CODE + pastData.ESHOP + (pastData.ESHOP_ID ?? "") + pastData.PAST_NO.Replace("\r\n", ""));

					printConsignData.LAB_VNR_NAME = pastData.LAB_VNR_NAME;
					printConsignData.LAB_NOTE1 = pastData.LAB_NOTE1;
					printConsignData.LAB_NOTE2 = pastData.LAB_NOTE2;
					printConsignData.LAB_NOTE3 = pastData.LAB_NOTE3;
					printConsignData.BARCODE_TYPE = pastData.BARCODE_TYPE;
					printConsignData.ISPRINTSTAR = pastData.ISPRINTSTAR;
					printConsignData.LAB_CUST_INFO = pastData.LAB_CUST_INFO;
					if (string.IsNullOrWhiteSpace(printConsignData.PAST_NO))
					{
						printConsignData.PAST_NOBarCode = "";
						printConsignData.PAST_NOBarCodeShow = "";
					}
					switch (printConsignData.BARCODE_TYPE)
					{
						case "0": //code39
							if (!string.IsNullOrWhiteSpace(printConsignData.PAST_NO))
							{
								switch (deliveryData.ALL_ID)
								{
									case "FAMILY":
										//全家code39條碼規則:*+母廠編+託運單號+*
										printConsignData.PAST_NOBarCode = string.Format("*{0}{1}*", printConsignData.ESHOP, printConsignData.PAST_NO.Replace("\r\n", ""));
										if (printConsignData.ISPRINTSTAR == "1")
											printConsignData.PAST_NOBarCodeShow = printConsignData.PAST_NOBarCode;
										else
											printConsignData.PAST_NOBarCodeShow = string.Format("{0}{1}", printConsignData.ESHOP, printConsignData.PAST_NO.Replace("\r\n", ""));
										break;
									case "711":
										//711code39條碼規則:門市+母廠編+子廠編+託運單號
										printConsignData.PAST_NOBarCode = string.Format("*{0}{1}{2}{3}*", pastData.RETAIL_CODE, pastData.ESHOP, (pastData.ESHOP_ID ?? ""), pastData.PAST_NO.Replace("\r\n", ""));
										if (printConsignData.ISPRINTSTAR == "1")
											printConsignData.PAST_NOBarCodeShow = printConsignData.PAST_NOBarCode;
										else
											printConsignData.PAST_NOBarCodeShow = string.Format("{0}{1}{2}{3}", pastData.RETAIL_CODE, printConsignData.ESHOP, (pastData.ESHOP_ID ?? ""), printConsignData.PAST_NO.Replace("\r\n", ""));
										break;
								}
							}
							break;
						case "1": //code128
							if (!string.IsNullOrWhiteSpace(printConsignData.PAST_NO))
							{
								switch (deliveryData.ALL_ID)
								{
									case "FAMILY":
										//全家code128條碼規則:母廠編+託運單號(未確認)
										printConsignData.PAST_NOBarCode = string.Format("{0}{1}", printConsignData.ESHOP, printConsignData.PAST_NO.Replace("\r\n", ""));
										if (printConsignData.ISPRINTSTAR == "0")
											printConsignData.PAST_NOBarCodeShow = printConsignData.PAST_NOBarCode;
										else
											printConsignData.PAST_NOBarCodeShow = string.Format("*{0}{1}*", printConsignData.ESHOP, printConsignData.PAST_NO.Replace("\r\n", ""));
										break;
									case "711":
										//code128條碼規則:門市+母廠編+子廠編+託運單號
										printConsignData.PAST_NOBarCode = string.Format("{0}{1}{2}{3}", pastData.RETAIL_CODE, pastData.ESHOP, (pastData.ESHOP_ID ?? ""), pastData.PAST_NO.Replace("\r\n", ""));
										if (printConsignData.ISPRINTSTAR == "0")
											printConsignData.PAST_NOBarCodeShow = printConsignData.PAST_NOBarCode;
										else
											printConsignData.PAST_NOBarCodeShow = string.Format("*{0}*", printConsignData.PAST_NOBarCode);
										break;
								}
								printConsignData.PAST_NOBarCode = BarcodeConverter128.StringToBarcode(printConsignData.PAST_NOBarCode);
							}
							break;
					}
				}
				if (deliveryData.ALL_ID == "TCAT")
				{
					printConsignData.ALL_ID = pastData.ALL_ID;
					printConsignData.CHANNEL = pastData.CHANNEL;
					printConsignData.EGS_SUDA7_DASH = pastData.EGS_SUDA7_DASH;
					printConsignData.EGS_SUDA7 = pastData.EGS_SUDA7;
					printConsignData.ORD_NO = pastData.ORD_NO;
					printConsignData.TCAT_ARRIVAL_DATE = pastData.TCAT_ARRIVAL_DATE;
					printConsignData.TCAT_DELV_DATE = pastData.TCAT_DELV_DATE;
					printConsignData.TCAT_PLACE = pastData.TCAT_PLACE;
					printConsignData.TCAT_MEMO = pastData.TCAT_MEMO;
					printConsignData.TCAT_SIZE = pastData.TCAT_SIZE;
					printConsignData.TCAT_TIME = pastData.TCAT_TIME;
					printConsignData.CHANNEL_NAME = pastData.CHANNEL_NAME;
					printConsignData.CHANNEL_ADDRESS = pastData.CHANNEL_ADDRESS;
					printConsignData.CHANNEL_TEL = pastData.CHANNEL_TEL;
					printConsignData.VERSION_NUMBER = pastData.VERSION_NUMBER;
					printConsignData.CUSTOMER_ID = pastData.CUSTOMER_ID;
				}
				if (deliveryData.ALL_ID == "HCT")
				{
					printConsignData.PACK_WEIGHT = pastData.PACK_WEIGHT;
					printConsignData.PIECES = pastData.PIECES;
					printConsignData.HCT_STATION = pastData.HCT_STATION;
					printConsignData.CHANNEL_NAME = pastData.CHANNEL_NAME;
					printConsignData.CHANNEL_ADDRESS = pastData.CHANNEL_ADDRESS;
					printConsignData.CHANNEL_TEL = pastData.CHANNEL_TEL;
					if (printConsignData.PAST_NO.Length == 10)
					{
						printConsignData.PAST_NOBarCode = string.Format("D{0}D", printConsignData.PAST_NO);
						printConsignData.PAST_NOBarCodeShow = string.Format("{0}-{1}-{2}", printConsignData.PAST_NO.Substring(0, 3), printConsignData.PAST_NO.Substring(3, 3), printConsignData.PAST_NO.Substring(6, 4));
					}
					//全部託運單列印才印總件數
					if (packageBoxNo == null)
						printConsignData.PIECES = packageBoxNos.Count().ToString();
				}

				if (deliveryData.ALL_ID == "KTJ")
				{
					printConsignData.PAST_NOByCode128 = BarcodeConverter128.StringToBarcode(printConsignData.PAST_NO);
					printConsignData.KTJ_STATION = pastData.KTJ_STATION;
					printConsignData.KTJ_STATION_S = pastData.KTJ_STATION_S;
					printConsignData.KTJ_STATION_NAME = pastData.KTJ_STATION_NAME;
					printConsignData.CHANNEL_NAME = pastData.CHANNEL_NAME;
					printConsignData.CHANNEL_ADDRESS = pastData.CHANNEL_ADDRESS;
					printConsignData.CHANNEL_TEL = pastData.CHANNEL_TEL;
				}

				if (deliveryData.ALL_ID == "YUNDA" || deliveryData.ALL_ID == "STO" || deliveryData.ALL_ID == "CEMS")
				{
					printConsignData.PAST_NOByCode128 = BarcodeConverter128.StringToBarcode(printConsignData.PAST_NO);
					printConsignData.CONCENTRATED = pastData.CONCENTRATED;
					printConsignData.CONCENTRATED_NOByCode128 = BarcodeConverter128.StringToBarcode(pastData.CONCENTRATED_NO);
					printConsignData.CONCENTRATED_NO = pastData.CONCENTRATED_NO;
					printConsignData.SHIPPING_AREA_NO = pastData.SHIPPING_AREA_NO;
					printConsignData.SHIPPINGCITY = pastData.SHIPPINGCITY;
					printConsignData.ALL_ID = pastData.ALL_ID;
					printConsignData.CHANNEL = pastData.CHANNEL;
					printConsignData.EGS_SUDA7_DASH = pastData.EGS_SUDA7_DASH;
					printConsignData.EGS_SUDA7 = pastData.EGS_SUDA7;
					printConsignData.ORD_NO = pastData.ORD_NO;
					printConsignData.ORD_NOByCode128 = BarcodeConverter128.StringToBarcode(pastData.ORD_NO);
					printConsignData.CHANNEL_NAME = pastData.CHANNEL_NAME;
					printConsignData.CHANNEL_ADDRESS = pastData.CHANNEL_ADDRESS;
					printConsignData.CHANNEL_TEL = pastData.CHANNEL_TEL;
					printConsignData.VERSION_NUMBER = pastData.VERSION_NUMBER;
					printConsignData.CUSTOMER_ID = pastData.CUSTOMER_ID;
					printConsignData.SHIPPING_FLAG = pastData.SHIPPING_FLAG;
					printConsignData.PACK_WEIGHT = pastData.PACK_WEIGHT;
					printConsignData.PACK_INSURANCE = pastData.PACK_INSURANCE;
				}

				//配送商若是黑貓要call EGS API
				//if (deliveryData.ALL_ID == "BLACKCAT")
				//{
				//	var egsData = proxyF08.CreateQuery<F055001Data>("GetEgsData")
				//	.AddQueryExOption("dcCode", deliveryData.DC_CODE)
				//	.AddQueryExOption("gupCode", deliveryData.GUP_CODE)
				//	.AddQueryExOption("custCode", deliveryData.CUST_CODE)
				//	.AddQueryExOption("wms_ord_no", deliveryData.WMS_ORD_NO)
				//	.AddQueryExOption("address", pastData.ADDRESS)
				//	.AddQueryExOption("custAddress", pastData.CUST_ADDRESS)
				//	.ToList().FirstOrDefault();

				//	printConsignData.VERSION_DATA = egsData.VERSION_DATA;
				//	printConsignData.VERSION_NUMBER = egsData.VERSION_NUMBER;
				//	printConsignData.EGS_SUDA5 = egsData.EGS_SUDA5;
				//	printConsignData.EGS_BASE = egsData.EGS_BASE;
				//	printConsignData.EGS_SUDA7 = egsData.EGS_SUDA7;
				//	printConsignData.EGS_SUDA7_DASH = egsData.EGS_SUDA7_DASH;

				//	var egsVolume = "";
				//	var size = (egsData.EGS_VOLUME.ToString() == "" ? 0 : Convert.ToInt64(egsData.EGS_VOLUME.ToString()));
				//	if (size <= 60)
				//		egsVolume = "60";
				//	else if (size <= 90)
				//		egsVolume = "90";
				//	else if (size <= 120)
				//		egsVolume = "120";
				//	else if (size <= 150)
				//		egsVolume = "150";
				//	else
				//		egsVolume = "150";
				//	printConsignData.EGS_VOLUME = egsVolume;

				//	printConsignData.CUST_ZIP = egsData.CUST_ZIP;
				//	printConsignData.MEMO = pastData.MEMO;
				//}

				consignDataList.Add(printConsignData);
			}
			return consignDataList;
		}

    #endregion

		#region 箱明細
		/// <summary>
		/// 列印箱明細
		/// </summary>
		public void PrintBoxData( List<DeliveryReport> boxDatas, F910501 selectedF910501, List<F050301> f050301s, F1909 f1909, DelvdtlInfo info, PcHomeDeliveryReport pchomeData)
		{
			ReportClass report = null;
			if (info.DELVDTL_FORMAT == "default")
			{
				report = ReportHelper.CreateAndLoadReport<R0807010001_Pchome>();
				foreach (var item in boxDatas)
				{
					item.PackageBoxBarCode = string.IsNullOrWhiteSpace(pchomeData.CUST_ORD_NO) ? string.Empty : BarcodeConverter128.StringToBarcode(pchomeData.CUST_ORD_NO + "|" + item.PackageBoxNo.ToString().PadLeft(3, '0'));
				}
				report.SetDataSource(boxDatas);

				report.SetParameterValue("出貨明細", Properties.Resources.ShippingList);
				report.SetParameterValue("出貨單號", Properties.Resources.DeliveryReportService_WmsOrdNo + "：");
				report.SetParameterValue("箱次", Properties.Resources.Id + "：");
				report.SetParameterValue("宅配類別", Properties.Resources.PRINT_MEMO + "：");
				report.SetParameterValue("訂單編號", Properties.Resources.SearchOrdNo + "：");
				report.SetParameterValue("轉單日期", Properties.Resources.TransferOrderDate + "：");
				report.SetParameterValue("印單日期", Properties.Resources.PrintDate + "：");
				report.SetParameterValue("序號", Properties.Resources.SerialNo);
				report.SetParameterValue("品名", Properties.Resources.ITEM_NAME);
				report.SetParameterValue("數量", Properties.Resources.A_SRC_QTY);
				report.SetParameterValue("CUST_ORD_NO", string.IsNullOrWhiteSpace(pchomeData.CUST_ORD_NO) ? string.Empty : pchomeData.CUST_ORD_NO);
				report.SetParameterValue("PRINT_CUST_ORD_NO", string.IsNullOrWhiteSpace(pchomeData.PRINT_CUST_ORD_NO) ? string.Empty : pchomeData.PRINT_CUST_ORD_NO);
				report.SetParameterValue("PRINT_MEMO", string.IsNullOrWhiteSpace(pchomeData.PRINT_MEMO) ? string.Empty : pchomeData.PRINT_MEMO);
				report.SetParameterValue("CRT_DATE", pchomeData.CRT_DATE == null ? string.Empty : ((DateTime)pchomeData.CRT_DATE).ToString("yyyy/MM/dd HH:mm:ss").Replace("-", "/"));
        if (pchomeData.ORDER_PROC_TYPE == "1" && pchomeData.IS_NORTH_ORDER == "1")
          report.SetParameterValue("郵遞區號", (pchomeData.ORDER_ZIP_CODE.Length > 3 ? pchomeData.ORDER_ZIP_CODE.Substring(0, 3) : pchomeData.ORDER_ZIP_CODE) + " 北北基");
        else if (pchomeData.ORDER_PROC_TYPE == "1" && pchomeData.IS_NORTH_ORDER != "1")
          report.SetParameterValue("郵遞區號", (pchomeData.ORDER_ZIP_CODE.Length > 3 ? pchomeData.ORDER_ZIP_CODE.Substring(0, 3) : pchomeData.ORDER_ZIP_CODE) + " 非北北基");
        else
          report.SetParameterValue("郵遞區號", "");
        report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
			}

			PrintReport(report, selectedF910501, PrinterType.A4);
		}

		#endregion

		#region AGV

		public void PrintAGVBoxReport(F910501 selectedF910501, List<F055003Data> data)
		{
			var report = ReportHelper.CreateAndLoadReport<RP0812010100>();
			report.SetDataSource(data);
			PrintReport(report, selectedF910501);
		}

		public void PrintAGVBoxIntactReport(F910501 selectedF910501, List<F055003Data> data)
		{
			var report = ReportHelper.CreateAndLoadReport<RP0812010200>();
			report.SetDataSource(data);
			PrintReport(report, selectedF910501);
		}

		#endregion

		#region 廠退小白標
		public void PrintRtnLable(F910501 selectedF910501, List<LittleWhiteReport> data)
		{
			var report = ReportHelper.CreateAndLoadReport<P0807010002>();
			foreach (var item in data)
			{
				item.sourceNoBarcode = string.IsNullOrWhiteSpace(item.SOURCE_NO) ? string.Empty : BarcodeConverter128.StringToBarcode(item.SOURCE_NO);
			}
			report.SetDataSource(data.ToDataTable());
			PrintReport(report, selectedF910501, PrinterType.A4);
		}

		#endregion

		#region 稽核出庫-箱內明細

		/// <summary>
		/// 取得稽核出庫箱明細資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <param name="pickTime"></param>
		/// <param name="moveOutTarget"></param>
		/// <param name="containerCode"></param>
		/// <param name="sowType"></param>
		/// <returns></returns>
		public List<P0808040100_PrintData> GetBoxData(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string moveOutTarget, string containerCode, string sowType)
		{
			var data = new List<P0808040100_PrintData>();
			var p08Proxy = GetExProxyConnect<P08ExDataSource>();
			if (sowType == "01")
			{
				data = p08Proxy.CreateQuery<P0808040100_PrintData>("GetPrintBoxData")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("delvDate", delvDate)
				.AddQueryExOption("pickTime", pickTime)
				.AddQueryExOption("moveOutTarget", moveOutTarget)
				.AddQueryExOption("containerCode", containerCode)
				.AddQueryExOption("sowType", sowType)
				.ToList();
			}
			else
			{
				data = p08Proxy.CreateQuery<P0808040100_PrintData>("GetPrintBoxData")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("delvDate", delvDate)
				.AddQueryExOption("pickTime", pickTime)
				.AddQueryExOption("moveOutTarget", moveOutTarget)
				.AddQueryExOption("containerCode", containerCode)
				.AddQueryExOption("sowType", sowType)
				.ToList();

			}
			return data;
		}

		/// <summary>
		/// 列印稽核出庫箱明細
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="sowType">播種類別</param>
		/// <param name="datas">報表資料</param>
		public void PrintBoxData(string dcCode,string sowType,List<P0808040100_PrintData> datas)
		{
			var selectedF910501 = OpenDeviceWindow(_functionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, dcCode).First();
		
			var p08Proxy = GetExProxyConnect<P08ExDataSource>();
			ReportClass report = null;
			if (sowType == "01")
			{
				report = ReportHelper.CreateAndLoadReport<R0808040101>();
			}
			else
			{
				report = ReportHelper.CreateAndLoadReport<R0808040102>();
			}

			if (datas.Any())
			{
                datas.ForEach(x => x.WMS_ORD_NO_BARCODE = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO));

                report.SetDataSource(datas);

				var firstData = datas.First();

				report.SetParameterValue("PACKAGE_BOX_BAR_CODE", BarcodeConverter128.StringToBarcode(firstData.PACKAGE_BOX));
				report.SetParameterValue("PACKAGE_BOX", firstData.PACKAGE_BOX);
				report.SetParameterValue("出貨箱明細", sowType == "01"?Properties.Resources.P0808040100_RP_Titie_1: Properties.Resources.P0808040100_RP_Titie_2);
				report.SetParameterValue("目的地", Properties.Resources.P0808040100_MoveOutTarget + "：");
				report.SetParameterValue("MOVE_OUT_TARGET", firstData.MOVE_OUT_TARGET);
				report.SetParameterValue("箱次", Properties.Resources.P0808040100_RP_PackageBoxNum + "：");
				report.SetParameterValue("CONTAINER_SEQ", firstData.CONTAINER_SEQ);
				report.SetParameterValue("頁數", Properties.Resources.P0808040100_RP_PageNum + "：");
				report.SetParameterValue("批次日期", Properties.Resources.P0808040100_DelvDate + "：");
				report.SetParameterValue("DELV_DATE", firstData.DELV_DATE);
				report.SetParameterValue("批次時段", Properties.Resources.P0808040100_PickTime + "：");
				report.SetParameterValue("PICK_TIME", firstData.PICK_TIME);
				report.SetParameterValue("印單日期", Properties.Resources.P0808040100_RP_PrintDate + "：");
				report.SetParameterValue("PRINT_DATE", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").Replace("-", "/"));
				report.SetParameterValue("序號", Properties.Resources.SerialNo);
				report.SetParameterValue("品名", Properties.Resources.ITEM_NAME);
				report.SetParameterValue("數量", Properties.Resources.A_SRC_QTY);

				if(sowType == "02")
					report.SetParameterValue("訂單單號", Properties.Resources.P0808040100_WMS_ORD_NO);

				report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;

				PrintReport(report, selectedF910501, PrinterType.A4);
			}
		}
    #endregion

    #region MyRegion

    /// <summary>
    /// 列印稽核出庫箱明細
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="sowType">播種類別</param>
    /// <param name="datas">報表資料</param>
    public void P080805PrintBoxData(F0532Ex f0532Ex, List<P0808050000_PrintData> datas)
    {
      var selectedF910501 = OpenDeviceWindow(_functionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, f0532Ex.DC_CODE).First();

      ReportClass report = ReportHelper.CreateAndLoadReport<P0808050000>();

      if (datas.Any())
      {
        f0532Ex.OUT_CONTAINER_CODE_BARCODE = BarcodeConverter128.StringToBarcode(f0532Ex.OUT_CONTAINER_CODE);

        report.SetDataSource(datas);

        report.SetParameterValue("PACKAGE_BOX_BAR_CODE", f0532Ex.OUT_CONTAINER_CODE_BARCODE);
        report.SetParameterValue("PACKAGE_BOX", f0532Ex.OUT_CONTAINER_CODE);
        report.SetParameterValue("出貨箱明細", "出貨箱明細");
        report.SetParameterValue("目的地", "目的地：");
        report.SetParameterValue("MOVE_OUT_TARGET", f0532Ex.MOVE_OUT_TARGET_NAME);
        report.SetParameterValue("頁數", "頁數：");
        report.SetParameterValue("建立時間", "建立時間：");
        report.SetParameterValue("CRT_DATE", f0532Ex.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"));
        report.SetParameterValue("關箱時間", "關箱時間：");
        report.SetParameterValue("CLOSE_DATE", f0532Ex.CLOSE_DATE.HasValue ? f0532Ex.CLOSE_DATE.Value.ToString("yyyy/MM/dd HH:mm:ss") : "");
        report.SetParameterValue("印單時間", "印單時間：");
        report.SetParameterValue("PRINT_DATE", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        report.SetParameterValue("序號", Properties.Resources.SerialNo);
        report.SetParameterValue("品名", Properties.Resources.ITEM_NAME);
        report.SetParameterValue("數量", Properties.Resources.A_SRC_QTY);

        report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;

        PrintReport(report, selectedF910501, PrinterType.A4);
      }
    }

    /// <summary>
    /// 列印稽核出庫取消箱明細
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="sowType">播種類別</param>
    /// <param name="datas">報表資料</param>
    public void P080805PrintCancelBoxData(F0532Ex f0532Ex, List<P0808050000_CancelPrintData> datas)
    {
      var selectedF910501 = OpenDeviceWindow(_functionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, f0532Ex.DC_CODE).First();

      ReportClass report = ReportHelper.CreateAndLoadReport<P0808050000_CN>();

      if (datas.Any())
      {
        f0532Ex.OUT_CONTAINER_CODE_BARCODE = BarcodeConverter128.StringToBarcode(f0532Ex.OUT_CONTAINER_CODE);

        foreach (var data in datas)
        {
          if (!string.IsNullOrWhiteSpace(data.ORD_NO))
          {
            data.ORDER_NO_BAR = BarcodeConverter128.StringToBarcode(data.ORD_NO);
          }
        }

        report.SetDataSource(datas);

        report.SetParameterValue("PACKAGE_BOX_BAR_CODE", f0532Ex.OUT_CONTAINER_CODE_BARCODE);
        report.SetParameterValue("PACKAGE_BOX", f0532Ex.OUT_CONTAINER_CODE);
        report.SetParameterValue("出貨箱明細", "取消箱明細");
        report.SetParameterValue("頁數", "頁數：");
        report.SetParameterValue("建立時間", "建立時間：");
        report.SetParameterValue("CRT_DATE", f0532Ex.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"));
        report.SetParameterValue("關箱時間", "關箱時間：");
        report.SetParameterValue("CLOSE_DATE", f0532Ex.CLOSE_DATE.HasValue ? f0532Ex.CLOSE_DATE.Value.ToString("yyyy/MM/dd HH:mm:ss") : "");
        report.SetParameterValue("印單時間", "印單時間：");
        report.SetParameterValue("PRINT_DATE", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        report.SetParameterValue("序號", Properties.Resources.SerialNo);
        report.SetParameterValue("品名", Properties.Resources.ITEM_NAME);
        report.SetParameterValue("數量", Properties.Resources.A_SRC_QTY);
        report.SetParameterValue("數量", Properties.Resources.A_SRC_QTY);
        report.SetParameterValue("訂單單號", "訂單單號");

        report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;

        PrintReport(report, selectedF910501, PrinterType.A4);
      }
    }
    #endregion
  }
}
