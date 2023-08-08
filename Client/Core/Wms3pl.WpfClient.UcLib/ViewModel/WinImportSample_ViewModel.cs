using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;

namespace Wms3pl.WpfClient.UcLib.ViewModel
{
    public class WinImportSample_ViewModel : InputViewModelBase
    {   
        public WinImportSample_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                InitSet();
            }
        }
      
        /// <summary>
        /// 設定雲端下載的URL
        /// </summary>
        private void SetConnectionPath()
        {
            var proxy = GetProxy<F00Entities>();
            var f0003 = proxy.F0003s.Where(o => o.AP_NAME == "DownloadSample" && o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00").FirstOrDefault();
            if (f0003 != null)
                _firstPath = f0003.SYS_PATH;     
        }

        /// <summary>
        /// 初始化設定
        /// </summary>
        private void InitSet()
        {
            SelectType1 = true;
            SetConnectionPath();
        }

        #region 屬性

        #region RadioButton選項

        /// <summary>
        /// 下載空白表格
        /// </summary>
        private bool _selectType1;

        public bool SelectType1
        {
            get { return _selectType1; }
            set
            {
                _selectType1 = value;
                RaisePropertyChanged("SelectType1");
            }
        }

        /// <summary>
        /// 匯入單據
        /// </summary>
        private bool _selectType2;   
        public bool SelectType2
        {
            get { return _selectType2; }
            set
            {
                _selectType2 = value;
                RaisePropertyChanged("SelectType2");
            }
        }

        #endregion

        /// <summary>
        /// 用來存取路徑(可能是網址，也可能是ftp)
        /// </summary>
        private string _firstPath = string.Empty;

        #endregion

        /// <summary>
        /// 匯出範本
        /// </summary>
        /// <param name="fileNameString"></param>
        /// <param name="fileName"></param>
        public void GetSample(string fileNameString, string fileName)
        {
            if (string.IsNullOrEmpty(_firstPath))
            {
                ShowWarningMessage("請確認連線狀況");
                return;
            }

            string filePath = string.Empty;

            filePath = Path.Combine(_firstPath, fileNameString.Split(',')[0], fileNameString.Split(',')[1]);
         
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "All|*";
            //保存對話框是否記憶上次打開的目錄
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Title = "下載空白表格";
            
            var proxy = new wcf.SharedWcfServiceClient();

            var sampleDataResult = RunWcfMethod<wcf.ByteData>(proxy.InnerChannel,
                () => proxy.GetSampleExcel(filePath, fileName));

            if (sampleDataResult.IsSucess)
            {
                try
                {
                    var sampleData = sampleDataResult.Data.Split(',').Select(byte.Parse).ToArray();

                    saveFileDialog.FileName = Path.GetFileName(sampleDataResult.FileName);

                    var isShowOk = saveFileDialog.ShowDialog();
                    if (isShowOk != true) return;
                    else
                        File.WriteAllBytes(saveFileDialog.FileName, sampleData);
                }
                catch (Exception ex)
                {
                    ShowWarningMessage("系統錯誤");
                }              
            }
            else
                ShowWarningMessage(sampleDataResult.Message);
            
           
        }

        /// <summary>
        /// 取html的方式取得檔案名稱
        /// 暫時不用此方法
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<string> GetFilesPath(string filePath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(filePath);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            List<string> tempList = new List<string>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                for (int i = 0; i < data.Length; i++)
                {
                    if (i + 2 < data.Length)
                    {
                        if ((data[i].ToString() + data[i + 1] + data[i + 2]) == "</A")
                        {
                            StringBuilder temp = new StringBuilder();
                            for (int j = i; j > 0; j--)
                            {
                                if (data[j - 1] == '>')
                                    break;
                                temp.Insert(0, data[j - 1]);
                            }
                            tempList.Add(temp.ToString());
                            temp = new StringBuilder();
                        }
                    }
                }

                if (tempList.Exists(o => o == "[移至上層目錄]"))
                    tempList.Remove("[移至上層目錄]");
                if (tempList.Exists(o => o == "web.config"))
                    tempList.Remove("web.config");
            }
            return tempList;
        }
    }
}
