using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Wms3pl.WpfClient.Services
{
    public class ExcelToPDFService
    {
        /// <summary>
        /// 把Excel檔案轉成PDF檔(僅接受xls,xlsx格式)
        /// </summary>
        /// <param name="sourceFilePath">來源檔案完整路徑及檔名</param>
        /// <param name="destinationFilePath">PDF檔案儲存路徑及檔名</param>
        public void ExcelToPDF(String sourceFilePath, String destinationFilePath)
        {
            try
            {
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(sourceFilePath);
                workbook.ConverterSetting.SheetFitToPage = true;
                workbook.SaveToFile(destinationFilePath, FileFormat.PDF);
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// 把Excel檔案轉成PDF檔
        /// </summary>
        /// <param name="sourceFile">來源資料</param>
        /// <param name="destinationFilePath">PDF檔案儲存路徑及檔名</param>
        public void ExcelToPDF(byte[] sourceFile, String destinationFilePath)
        {
            try
            {
                Workbook workbook = new Workbook();
                Stream stream = new MemoryStream(sourceFile);
                workbook.LoadFromStream(stream);
                workbook.ConverterSetting.SheetFitToPage = true;
                workbook.SaveToFile(destinationFilePath, FileFormat.PDF);
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// 列印Excel文件
        /// </summary>
        /// <param name="sourceFilePath">要列印的檔案路徑</param>
        /// <param name="PrinterName">印表機名稱(從f910501取得)</param>
        /// <param name="copies">列印份數</param>
        /// <param name="PrintWorkSheet">要列印的工作表，不設定就只印第1個工作表，從0開始計算</param>
        public void Print(String sourceFilePath, string PrinterName, Int16 copies = 1, List<int> PrintWorkSheet = null)
        {
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(sourceFilePath);

            Workbook PrintBook = new Workbook();
            if (PrintWorkSheet != null)
            {
                if (PrintWorkSheet.Count > 3)
                    throw new Exception("不可列印超過3頁的工作表");
                if (PrintWorkSheet.Any(x => x > workbook.Worksheets.Count - 1))
                    throw new Exception("要列印的工作表索引大於檔案索引");
                PrintBook.Worksheets.Clear();
                foreach (var WorkSheetIndex in PrintWorkSheet)
                    PrintBook.Worksheets.AddCopy(workbook.Worksheets[WorkSheetIndex]);

                foreach (var WorkSheetItem in PrintBook.Worksheets)
                {
                    WorkSheetItem.PageSetup.FitToPagesWide = 1;
                }
            }
            else
            {
                PrintBook = workbook;
                PrintBook.Worksheets[0].PageSetup.FitToPagesWide = 1;
            }

            PrintDialog dialog = new PrintDialog();
            dialog.AllowPrintToFile = true;
            dialog.AllowCurrentPage = true;
            dialog.AllowSomePages = true;
            dialog.UseEXDialog = true;
            PrintBook.PrintDialog = dialog;
            PrintDocument printDoc = PrintBook.PrintDocument;
            printDoc.PrinterSettings.PrinterName = PrinterName;
            printDoc.PrinterSettings.Copies = copies;
            printDoc.Print();
        }

        /// <summary>
        /// 列印excel文件
        /// </summary>
        /// <param name="sourceFilePath">要列印的內容</param>
        /// <param name="PrinterName">印表機名稱(從f910501取得)</param>
        /// <param name="copies">列印份數</param>
        /// <param name="PrintWorkSheet">要列印的工作表，不設定就只印第1個工作表，從0開始計算</param>
        public void Print(byte[] sourceFile, string PrinterName, Int16 copies = 1, List<int> PrintWorkSheet = null)
        {
            Workbook workbook = new Workbook();
            Stream stream = new MemoryStream(sourceFile);
            workbook.LoadFromStream(stream);

            Workbook PrintBook = new Workbook();
            if (PrintWorkSheet != null)
            {
                if (PrintWorkSheet.Count > 3)
                    throw new Exception("不可列印超過3頁的工作表");

                if (PrintWorkSheet.Any(x => x > workbook.Worksheets.Count - 1))
                    throw new Exception("要列印的工作表索引大於檔案索引");
                PrintBook.Worksheets.Clear();
                foreach (var WorkSheetIndex in PrintWorkSheet)
                    PrintBook.Worksheets.AddCopy(workbook.Worksheets[WorkSheetIndex]);

                foreach (var WorkSheetItem in PrintBook.Worksheets)
                {
                    WorkSheetItem.PageSetup.FitToPagesWide = 1;
                }
            }
            else
            {
                PrintBook = workbook;
                PrintBook.Worksheets[0].PageSetup.FitToPagesWide = 1;
            }


            PrintDialog dialog = new PrintDialog();
            dialog.AllowPrintToFile = true;
            dialog.AllowCurrentPage = true;
            dialog.AllowSomePages = true;
            dialog.UseEXDialog = true;
            PrintBook.PrintDialog = dialog;
            PrintDocument printDoc = PrintBook.PrintDocument;
            printDoc.PrinterSettings.PrinterName = PrinterName;
            printDoc.PrinterSettings.Copies = copies;
            printDoc.Print();
        }

    }
}
