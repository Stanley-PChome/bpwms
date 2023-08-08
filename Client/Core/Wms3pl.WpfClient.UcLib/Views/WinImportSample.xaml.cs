using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.UcLib.Views
{
    /// <summary>
    /// WinImportSample.xaml 的互動邏輯
    /// </summary>
    public partial class WinImportSample : Wms3plWindow
    {
        public Action<bool> ImportResult= delegate { };

        /// <summary>
        /// 傳入格式:CustCode,FunctionID
        /// </summary>
        /// <param name="funCode">程式代碼</param>
        public WinImportSample(string custFunCode, string fileName = null) : base(false)
        {
            _CustFunctionCode = custFunCode;
            _fileName = fileName;
            InitializeComponent();                 
        }

        private string _CustFunctionCode = string.Empty;
        private string _fileName;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImportResult(Vm.SelectType2);
            //匯出範本
            if (Vm.SelectType1)
                Vm.GetSample(_CustFunctionCode, _fileName);
            
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
