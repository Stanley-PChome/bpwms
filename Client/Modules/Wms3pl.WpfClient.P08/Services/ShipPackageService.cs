using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using DeliveryReport = Wms3pl.WpfClient.ExDataServices.P08ExDataService.DeliveryReport;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P08ExDataService.ExecuteResult;
using F050301 = Wms3pl.WpfClient.DataServices.F05DataService.F050301;
using F050801 = Wms3pl.WpfClient.DataServices.F05DataService.F050801;
using LittleWhiteReport = Wms3pl.WpfClient.ExDataServices.P08ExDataService.LittleWhiteReport;
using PcHomeDeliveryReport = Wms3pl.WpfClient.ExDataServices.P08ExDataService.PcHomeDeliveryReport;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.Services
{
    public class ShipPackageService : InputViewModelBase
    {
        /// <summary>
        /// 取得暫存目錄(ShareFolderTemp\ShipPacking)
        /// </summary>
        private string _shareFolderTemp { get { return Path.Combine(ConfigurationManager.AppSettings["ShareFolderTemp"], "ShipPacking"); } }
        /// <summary>
        /// 取得暫存用log目錄位置(ShareFolderTemp\ShipPacking\log)
        /// </summary>
        private string _logFolder { get { return Path.Combine(ConfigurationManager.AppSettings["ShareFolderTemp"], "ShipPacking", "log"); } }
        private string _functionCode;

        public ShipPackageService(string functionCode)
        {
            _functionCode = functionCode;
        }

        private T GetExProxyConnect<T>() where T : DataServiceContext
        {
            return ConfigurationExHelper.GetExProxy<T>(false, _functionCode);
        }

        /// <summary>
        /// 包裝站開站/關站紀錄
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public wcf.SetPackageStationStatusLogRes SetPackageStationStatusLog(wcf.SetPackageStationStatusLogReq req)
        {
            var proxy = GetWcfProxy<P08WcfServiceClient>();
            return proxy.RunWcfMethod(w => w.SetPackageStationStatusLog(req));
        }

        /// <summary>
        /// 出貨容器條碼檢核
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public wcf.CheckShipContainerCodeRes CheckShipContainerCode(wcf.CheckShipContainerCodeReq req)
        {
            var proxy = GetWcfProxy<P08WcfServiceClient>();
            return proxy.RunWcfMethod(w => w.CheckShipContainerCode(req));
        }

        /// <summary>
        /// 查詢與檢核出貨單資訊
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public SearchAndCheckWmsOrderInfoRes SearchAndCheckWmsOrderInfo(SearchAndCheckWmsOrderInfoReq req)
        {
            var proxy = GetWcfProxy<P08WcfServiceClient>();
            return proxy.RunWcfMethod(w => w.SearchAndCheckWmsOrderInfo(req));
        }

        /// <summary>
        /// 刷讀商品條碼
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ScanItemBarcodeRes ScanItemBarcode(ScanItemBarcodeReq req)
        {
            var proxy = GetWcfProxy<P08WcfServiceClient>();
            return proxy.RunWcfMethod(w => w.ScanItemBarcode(req));
        }

        /// <summary>
        /// 使用出貨單容器資料產生箱明細
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public UseShipContainerToBoxDetailRes UseShipContainerToBoxDetail(UseShipContainerToBoxDetailReq req)
        {
            var proxy = GetWcfProxy<P08WcfServiceClient>();
            return proxy.RunWcfMethod(w => w.UseShipContainerToBoxDetail(req));
        }

        /// <summary>
        /// 取消包裝
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public CancelShipOrderRes CancelShipOrder(CancelShipOrderReq req)
        {
            var proxy = GetWcfProxy<P08WcfServiceClient>();
            return proxy.RunWcfMethod(w => w.CancelShipOrder(req));
        }

        /// <summary>
        /// 變更出貨單為所有商品都需過刷
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public wcf.ChangeShipPackCheckRes ChangeShipPackCheck(ChangeShipPackCheckReq req)
        {
            var proxy = GetWcfProxy<P08WcfServiceClient>();
            return proxy.RunWcfMethod(w => w.ChangeShipPackCheck(req));
        }

        /// <summary>
        /// 關箱處理
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public CloseShipBoxRes CloseShipBox(CloseShipBoxReq req)
        {
            var proxy = GetWcfProxy<P08WcfServiceClient>();
            return proxy.RunWcfMethod(w => w.CloseShipBox(req));
        }

        /// <summary>
        /// 取得出貨商品明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        public List<SearchWmsOrderPackingDetailRes> SearchWmsOrderPackingDetail(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var p08Proxy = GetExProxyConnect<P08ExDataSource>();
            return p08Proxy.CreateQuery<SearchWmsOrderPackingDetailRes>("SearchWmsOrderPackingDetail")
                .AddQueryExOption("dcCode", dcCode)
                .AddQueryExOption("gupCode", gupCode)
                .AddQueryExOption("custCode", custCode)
                .AddQueryExOption("wmsOrdNo", wmsOrdNo)
                .ToList();
        }

        /// <summary>
        /// 取得刷讀紀錄
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        public List<SearchWmsOrderScanLogRes> SearchWmsOrderScanLog(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var p08Proxy = GetExProxyConnect<P08ExDataSource>();
            return p08Proxy.CreateQuery<SearchWmsOrderScanLogRes>("SearchWmsOrderScanLog")
                .AddQueryExOption("dcCode", dcCode)
                .AddQueryExOption("gupCode", gupCode)
                .AddQueryExOption("custCode", custCode)
                .AddQueryExOption("wmsOrdNo", wmsOrdNo)
                .ToList();
        }

        /// <summary>
        /// 單人包裝站列印報表
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="shipPackageReports">要列印報表</param>
        /// <param name="printerSet">F910501 印表機設定</param>
        /// <param name="wmsOrdNo">出貨單號</param>
        /// <param name="reprint">是否補印 ReportCode=04為必填</param>
        /// <returns></returns>
        public ExecuteResult PrintShipPackage(string dcCode, string gupCode, string custCode, List<ShipPackageReportModel> shipPackageReports, F910501 printerSet, string wmsOrdNo, int reprint = 0)
        {


#if (DEBUG)
            reprint = 1;    //測試環境都用同一個URL會跳出錯誤，因此強制設定1
#endif

            var results = new List<ExecuteResult>();

            // 依照印表機編號進行多執行緒處理
            var dis = shipPackageReports.GroupBy(x => x.PrinterNo).ToDictionary(x => x.Key, x => x.ToList());

            Parallel.ForEach(dis, ds =>
            {
                Print(dcCode, gupCode, custCode, wmsOrdNo, ds.Value, printerSet, ref results, reprint);
            });

            if (shipPackageReports.Any() && !results.Any())
            {
                var proxy = GetWcfProxy<P08WcfServiceClient>();
                proxy.RunWcfMethod(w => w.UpdateShipReportList(dcCode, gupCode, custCode, wmsOrdNo, shipPackageReports.ToArray()));
            }

            if (results.Any())
                return new ExecuteResult() { IsSuccessed = false, Message = string.Join("\r\n", results.Select(x => x.Message)) };

            return new ExecuteResult() { IsSuccessed = true };
        }

        private void Print(string dcCode, string gupCode, string custCode, string WmsOrdNo, List<ShipPackageReportModel> shipPackageReports, F910501 PrinterSet, ref List<ExecuteResult> results, int Reprint = 0)
        {
            /*
		 1. Foreach [K] IN [H]. ReportList
		 (1) [印表機]= case [K].PrinterNo 
				 when 1 then [印表機1] 
				 when 2 then [印表機2] 
				 when 3 then [快速標籤機]
		 (2)如果[K].ReportCode = 01
				 a.呼叫[1.1.10 取得箱明細報表資料] => 列印箱明細([印表機])
		 (3)如果[K].ReportCode = 02
				 a.呼叫[1.1.11 取得一般出貨小白標資料]=> 列印出貨小白標([印表機])
		 (4)如果[K].ReportCode = 03
				 a.呼叫[1.1.12 取得廠退出貨小白標資料]=> 列印廠退出或小白標([印表機])
		 (5)如果[K].ReportCode = 04
				 a.[L]=呼叫[1.1.13 取得LMS出貨列印檔案] 
				 b.[L].ContentType = word or excel 先轉換成pdf檔案再去列印
						 如果為word檔=application/vnd.ms-word
						 如果為excel檔=application/vnd.ms-excel  
				 c.列印其他單據([印表機])
		 */
            var proxy = GetWcfProxy<P08WcfServiceClient>();

            Boolean IsPrintSuccess = false;
            String printerName;
            PrinterType printerType;
            foreach (var item in shipPackageReports)
            {
                try
                {
                    switch (item.PrinterNo)
                    {
                        case "1":
                            printerName = PrinterSet.PRINTER;
                            printerType = PrinterType.A4;
                            break;
                        case "2":
                            printerName = PrinterSet.MATRIX_PRINTER;
                            printerType = PrinterType.Matrix;
                            break;
                        case "3":
                            printerName = PrinterSet.LABELING;
                            printerType = PrinterType.Label;
                            break;
                        default:
                            results.Add(new ExecuteResult() { IsSuccessed = false, Message = $"{item.ReportName}列印失敗，無法辨識的印表機編號" });
                            continue;
                    }

                    if (new string[] { "01", "02", "03" }.Contains(item.ReportCode) && string.IsNullOrWhiteSpace(WmsOrdNo))
                    {
                        results.Add(new ExecuteResult() { IsSuccessed = false, Message = $"{item.ReportName}列印失敗，出貨單號為必填欄位" });
                        continue;
                    }

                    switch (item.ReportCode)
                    {
                        case "01":

                            var getBoxDetailReportReq = new GetBoxDetailReportReq()
                            {
                                DcCode = dcCode,
                                GupCode = gupCode,
                                CustCode = custCode,
                                PackageBoxNo = item.PackageBoxNo,
                                WmsOrdNo = WmsOrdNo
                            };
                            var getBoxDetailReportRes = proxy.RunWcfMethod(w => w.GetBoxDetailReport(getBoxDetailReportReq));
                            DelvdtlInfo info = new DelvdtlInfo() { DELVDTL_FORMAT = "default" };

                            //列印箱明細
                            item.START_PRINT_TIME = DateTime.Now;
                            PrintBoxData(
                                    ExDataMapper.MapCollection<wcf.BoxDetailData, ExDataServices.P08ExDataService.DeliveryReport>(getBoxDetailReportRes.BoxDetail).ToList(),
                                    PrinterSet,
                                    null,
                                    null,
                                    info,
                                    ExDataMapper.Map<wcf.BoxHeaderData, ExDataServices.P08ExDataService.PcHomeDeliveryReport>(getBoxDetailReportRes.BoxHeader));

                            IsPrintSuccess = true;
                            break;
                        case "02":
                            //var getShipLittleLabelReportReq = new GetShipLittleLabelReportReq()
                            //{
                            //    DcCode = dcCode,
                            //    GupCode = gupCode,
                            //    CustCode = custCode,
                            //    PackageBoxNo = item.PackageBoxNo,
                            //    WmsOrdNo = WmsOrdNo
                            //};
                            //var getShipLittleLabelReportRes = proxy.RunWcfMethod(w => w.GetShipLittleLabelReport(getShipLittleLabelReportReq));

                            var boxLittleLabelDetail = new Box[]
														{
                                new Box
                                {
                                    BoxBarCode = item.CustOrdNo + "|" + item.PackageBoxNo.ToString().PadLeft(3, '0'),
                                    BoxCode = item.CustOrdNo + "|" + item.PackageBoxNo.ToString().PadLeft(3, '0')
                                }
														};
														var getShipLittleLabelReportRes = new GetShipLittleLabelReportRes { BoxLittleLabelDetail = boxLittleLabelDetail };

                            //列印出貨小白標([印表機])
                            item.START_PRINT_TIME = DateTime.Now;
                            PrintShipLittleLabelRepor(PrinterSet, printerType, getShipLittleLabelReportRes);

                            IsPrintSuccess = true;
                            break;
                        case "03":
                            var getRtnShipLittleLabelReportReq = new GetRtnShipLittleLabelReportReq()
                            {
                                DcCode = dcCode,
                                GupCode = gupCode,
                                CustCode = custCode,
                                PackageBoxNo = item.PackageBoxNo,
                                WmsOrdNo = WmsOrdNo
                            };
                            var getRtnShipLittleLabelReportRes = proxy.RunWcfMethod(w => w.GetRtnShipLittleLabelReport(getRtnShipLittleLabelReportReq));

                            //列印廠退出貨小白標([印表機])
                            item.START_PRINT_TIME = DateTime.Now;
                            PrintRtnLable(PrinterSet,
                                    ExDataMapper.MapCollection<wcf.BoxRtnLittleLabel, ExDataServices.P08ExDataService.LittleWhiteReport>(getRtnShipLittleLabelReportRes.BoxRtnLittleLabelDetail).ToList());
                            IsPrintSuccess = true;
                            break;
                        case "04":
                            PrintPdfService printPdfService = new PrintPdfService();

                            var geShipFileReq = new GeShipFileReq()
                            {
                                DcCode = dcCode,
                                GupCode = gupCode,
                                CustCode = custCode,
                                Url = item.ReportUrl,
                                Reprint = Reprint
                            };

                            var geShipFileRes = proxy.RunWcfMethod<GeShipFileRes>(w => w.GeShipFile(geShipFileReq));
                            if (!geShipFileRes.IsSuccessed)
                            {
                                results.Add(new ExecuteResult() { IsSuccessed = false, Message = $"[LMS出貨列印檔案]取得{item.ReportName}檔案失敗" + (string.IsNullOrEmpty(geShipFileRes.Message) ? "" : "，") + geShipFileRes.Message });
                                continue;
                            }
														
                            item.START_PRINT_TIME = DateTime.Now;
                            if (geShipFileRes.ContentType == "application/vnd.ms-word")
                            {
                                var wordToPDFService = new WordToPDFService();
                                wordToPDFService.Print(geShipFileRes.FileBytes, printerName.ToString(), 1);
                            }
                            else if (geShipFileRes.ContentType == "application/vnd.ms-excel")
                            {
                                var excelToPDFService = new ExcelToPDFService();
                                excelToPDFService.Print(geShipFileRes.FileBytes, printerName.ToString(), 1);
                            }
                            else
                            {
                                printPdfService.Print(geShipFileRes.FileBytes, _functionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, dcCode, printerName.ToString(), 1);
                            }
                            IsPrintSuccess = true;
                            break;
                        default:
                            break;
                    }

                    if (IsPrintSuccess)
                        if (item.ISPRINTED == "0" || string.IsNullOrWhiteSpace(item.ISPRINTED))
                        {
                            item.ISPRINTED = "1";
                            item.PRINT_TIME = DateTime.Now;
                        }
                }
                catch (Exception ex)
                {
                    results.Add(new ExecuteResult() { IsSuccessed = false, Message = $"{item.ReportName}列印失敗，{ex.Message}" });
                    continue;
                }
            }
        }

        #region 箱明細
        /// <summary>
        /// 列印箱明細
        /// </summary>
        public void PrintBoxData(List<DeliveryReport> boxDatas, F910501 selectedF910501, List<F050301> f050301s, F1909 f1909, DelvdtlInfo info, PcHomeDeliveryReport pchomeData)
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


        /// <summary>
        /// 產出檔案路徑以及完整檔案路徑
        /// </summary>
        /// <param name="WmsOrdNo">出貨單號</param>
        /// <param name="FileName">檔名</param>
        /// <param name="FilePath">檔案路徑(只有路徑)</param>
        /// <param name="FullFilePath">完整檔案路徑(路徑＋檔名)</param>
        private void CheckTmpFilePath(string WmsOrdNo, String FileName, out String FilePath, out String FullFilePath)
        {
            FilePath = Path.Combine(_shareFolderTemp, WmsOrdNo);
            FullFilePath = Path.Combine(FilePath, FileName);

            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);
        }

        /// <summary>
        /// 刪除列印暫存的PDF檔案
        /// </summary>
        private void DeleteTmpDir(string WmsOrdNo, String FileName)
        {
            String FilePath, FullFilePath;

            CheckTmpFilePath(WmsOrdNo, FileName, out FilePath, out FullFilePath);

            foreach (var dirPath in Directory.GetDirectories(_shareFolderTemp))
            {
                var di = new DirectoryInfo(dirPath);
                //刪除非本次單號的目錄
                if (di.Name != WmsOrdNo)
                {
                    try
                    { di.Delete(true); }
                    catch (Exception ex)
                    {
                        // 檢查log存檔路徑不存在新增
                        if (!Directory.Exists(_logFolder)) Directory.CreateDirectory(_logFolder);
                        // 建立log
                        using (var stream = new StreamWriter($"{_logFolder}\\{WmsOrdNo}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.log"))
                            stream.WriteLine(ex);
                    }
                }
            }

        }

        //列印出貨小白標
        private void PrintShipLittleLabelRepor(F910501 device, PrinterType printerType, GetShipLittleLabelReportRes getRtnShipLittleLabelReportRes)
        {
            ReportClass report = ReportHelper.CreateAndLoadReport<RP0814010000>();
            foreach (var item in getRtnShipLittleLabelReportRes.BoxLittleLabelDetail)
            {
                report.SetParameterValue("Barcode", BarcodeConverter128.StringToBarcode(item.BoxBarCode));
                report.SetParameterValue("BarcodeContent", item.BoxCode);
                PrintReport(report, device, printerType);
            }
        }

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

        public void LogPausePacking(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var wcfproxy = GetWcfProxy<wcf.P08WcfServiceClient>();
            wcfproxy.RunWcfMethod(w => w.LogPausePacking(dcCode, gupCode, custCode, wmsOrdNo));
        }

        /// <summary>
        /// 寫入訂單回檔記錄
        /// </summary>
        /// <param name="addF050305"></param>
        /// <returns></returns>
        public void InsertF050305Data(string dcCode, string gupCode, string custCode, string wmsOrdNo, string status, string procFlag, string WorkStationId)
        {
            var wcfproxy = GetWcfProxy<wcf.P08WcfServiceClient>();
            wcfproxy.RunWcfMethod(w => w.InsertF050305Data(dcCode, gupCode, custCode, wmsOrdNo, status, procFlag, WorkStationId));
        }


    /// <summary>
    /// 單人包裝站、包裝線包裝站檢查裝置維護設定共用
    /// </summary>
    /// <param name="f910501"></param>
    /// <param name="shipMode"></param>
    /// <param name="dcCode"></param>
    /// <param name="IsPrintShipLittleLabel"></param>
    /// <param name="f910501Exist"></param>
    /// <returns></returns>
    public Boolean CheckDeviceSetting(ref F910501 f910501, string shipMode, string dcCode,F0003 IsPrintShipLittleLabel, F910501 f910501Exist)
    {
      var errMsgs = new List<string>();
      // 是否列印一般出貨小白標
      var isPrintShipLittleLabel = IsPrintShipLittleLabel != null && IsPrintShipLittleLabel.SYS_PATH == "0" ? "0" : "1";

      // 如果[C]不存在，彈出訊息視窗，顯示[請先設定印表機與工作站編號]，按下<確認>後，開啟[P1905140000裝置設定維護UI]，儲存後關閉，再回到3
      if (f910501Exist == null)
        errMsgs.Add("請先設定印表機與工作站編號");
      else
      {
        // 如果[C].PRINTER = NULL OR 空白，顯示[未設定印表機1，請設定]，按下<確認>後，開啟[P1905140000裝置設定維護UI]，儲存後關閉，再回到3
        if (string.IsNullOrWhiteSpace(f910501Exist.PRINTER))
          errMsgs.Add("未設定印表機1，請設定");
        // 如果[C].MATRIX_PRINTER = NULL OR 空白，顯示[未設定印表機2，請設定]，按下<確認>後，開啟[P1905140000裝置設定維護UI]，儲存後關閉，再回到3
        else if (string.IsNullOrWhiteSpace(f910501Exist.MATRIX_PRINTER))
          errMsgs.Add("未設定印表機2，請設定");
        // 如果[D]=1 且[C].LABELING = NULL OR 空白，顯示[未設定快速標籤機，請設定]，按下<確認>後，開啟[P1905140000裝置設定維護UI]，儲存後關閉，再回到3
        else if (isPrintShipLittleLabel == "1" && string.IsNullOrWhiteSpace(f910501Exist.LABELING))
          errMsgs.Add("未設定快速標籤機，請設定");
        // <參數1>=1(單人包裝站)，如果[C].WORKSTATION_TYPE not in ( PACK,FACT) 顯示[工作站類型必須選單人包裝站或廠退處理區]
        else if (shipMode == "1" && f910501Exist.WORKSTATION_TYPE != "PACK" && f910501Exist.WORKSTATION_TYPE != "FACT")
          errMsgs.Add("工作站類型必須選單人包裝站或廠退處理區");
        // <參數1>=2(包裝線包裝站)，如果[C].WORKSTATION_TYPE 不是包裝大線站、包裝小線站 顯示[工作站類型必須選包裝線大線或包裝線小線]
        else if (shipMode == "2" && f910501Exist.WORKSTATION_TYPE != "PA1" && f910501Exist.WORKSTATION_TYPE != "PA2")
          errMsgs.Add("工作站類型必須選包裝線大線或包裝線小線");
        else if (string.IsNullOrWhiteSpace(f910501Exist.WORKSTATION_CODE))
          errMsgs.Add("未設定工作站編號");
      }
      if (errMsgs.Any())
      {
        ShowWarningMessage(string.Join("\r\n", errMsgs) + "\r\n請至裝置設定維護調整，本功能即將關閉");
        return false;
      }
      return true;
    }

		public PrintBoxSettingParam GetPrintBoxSetting(string dcCode, string gupCode, string custCode, string ShipMode)
		{
			var wcfproxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			return wcfproxy.RunWcfMethod(w => w.GetPrintBoxSetting(dcCode, gupCode, custCode, ShipMode));
		}

	}
}
