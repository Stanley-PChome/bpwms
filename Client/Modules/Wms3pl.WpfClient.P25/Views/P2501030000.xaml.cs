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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P25.Views
{
	/// <summary>
	/// P2501030000.xaml 的互動邏輯
	/// </summary>
	public partial class P2501030000 : Wms3plUserControl
	{
		public P2501030000()
		{
			InitializeComponent();
            Vm.ExcelImport += ExcelImport;


            SetFocusedElement(txtSearchSerialNo);
		}

        private void ExcelImport()
        {
            bool ImportResultData = false;

            var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P2501030000"));
            win.ImportResult = (t) => { ImportResultData = t; };
            win.ShowDialog();
            Vm.ImportFilePath = null;
            if (ImportResultData)
                Vm.ImportFilePath = OpenFileDialogFun();
        }

        private string OpenFileDialogFun()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xls",
                Filter = "excel files (*.cls,*xlsx)|*.xls*|csv files (*.csv)|*.csv"
            };
            if(dlg.ShowDialog()== true)
            {
                return dlg.FileName;
            }
            return "";
        }

		private void btnSerachProduct_Click(object sender, RoutedEventArgs e)
		{
			WinSearchProduct winSearchProduct = new WinSearchProduct();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			winSearchProduct.GupCode = globalInfo.GupCode;
			winSearchProduct.CustCode = globalInfo.CustCode;
			winSearchProduct.ShowDialog();

			if (winSearchProduct.SelectData != null)
			{
				Vm.EditableF2501WcfData.ITEM_CODE = winSearchProduct.SelectData.ITEM_CODE;
				Vm.EditableF2501WcfData.SERIAL_NO = winSearchProduct.SearchSerialNo;
				Vm.CheckBundleSeriallocCommand.Execute(null);
			}
		}
	}
}
