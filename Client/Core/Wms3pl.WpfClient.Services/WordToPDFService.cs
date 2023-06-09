using Spire.Doc;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace Wms3pl.WpfClient.Services
{
    public class WordToPDFService
    {
        /// <summary>
        /// 把Word檔案轉成PDF檔(僅接受doc,docx格式)
        /// </summary>
        /// <param name="sourceFilePath">來源檔案完整路徑及檔名</param>
        /// <param name="destinationFilePath">PDF檔案儲存路徑及檔名</param>
        public void WordToPDF(String sourceFilePath, String destinationFilePath)
        {
            try
            {
                Document doc = new Document();
                doc.LoadFromFile(sourceFilePath);
                doc.SaveToFile(destinationFilePath, FileFormat.PDF);
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// 把Word檔案轉成PDF檔(僅接受doc,docx格式)
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFilePath">PDF檔案儲存路徑及檔名</param>
        public void WordToPDF(byte[] sourceFile, String destinationFilePath)
        {
            try
            {
                Document doc = new Document();
                Stream stream = new MemoryStream(sourceFile);
                doc.LoadFromStream(stream, FileFormat.Auto);
                doc.SaveToFile(destinationFilePath, FileFormat.PDF);
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// 列印Word文件
        /// </summary>
        /// <param name="sourceFilePath">要列印的檔案路徑</param>
        /// <param name="PrinterName">印表機名稱(從f910501取得)</param>
        /// <param name="copies">列印份數</param>
        public void Print(String sourceFilePath, string PrinterName, Int16 copies)
        {
            Document doc = new Document();
            doc.LoadFromFile(sourceFilePath);
            PrintDialog dialog = new PrintDialog();
            dialog.AllowPrintToFile = true;
            dialog.AllowCurrentPage = true;
            dialog.AllowSomePages = true;
            dialog.UseEXDialog = true;
            doc.PrintDialog = dialog;
            PrintDocument printDoc = doc.PrintDocument;
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
        public void Print(byte[] sourceFile, string PrinterName, Int16 copies)
        {
            Document doc = new Document();
            Stream stream = new MemoryStream(sourceFile);
            doc.LoadFromStream(stream, FileFormat.Auto);
            PrintDialog dialog = new PrintDialog();
            dialog.AllowPrintToFile = true;
            dialog.AllowCurrentPage = true;
            dialog.AllowSomePages = true;
            dialog.UseEXDialog = true;
            doc.PrintDialog = dialog;
            PrintDocument printDoc = doc.PrintDocument;
            printDoc.PrinterSettings.PrinterName = PrinterName;
            printDoc.PrinterSettings.Copies = copies;
            printDoc.Print();
        }

    }
}
