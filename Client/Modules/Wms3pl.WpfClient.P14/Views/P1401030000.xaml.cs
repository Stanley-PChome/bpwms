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
using Microsoft.Win32;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P14.Views
{
	/// <summary>
	/// P1401030000.xaml 的互動邏輯
	/// </summary>
	public partial class P1401030000 : Wms3plUserControl
	{
		public P1401030000()
		{
			InitializeComponent();
            Vm.ExcelImport += ExcelImport;
            Vm.PreImport += ImportInventoryLocItemSerial_OnClick;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var win = new WinSearchProduct();
			win.GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			win.CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			win.Owner = this.Parent as Window;
			win.ShowDialog();
			if (win.DialogResult.HasValue && win.DialogResult.Value)
				Vm.QueryItemCode = win.SelectData != null ? win.SelectData.ITEM_CODE : string.Empty;
		}

		bool ImportResultData = false;
        private void ExcelImport()
        {
            var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P1401030000"), "BP1401030003.xlsx");

            win.ImportResult = (t) => { ImportResultData = t; };
            win.ShowDialog();
            Vm.ImportFilePath = null;
            if (ImportResultData)
                Vm.ImportFilePath = OpenFileDialogFun();
        }

        private string OpenFileDialogFun()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".xls",
                Filter = "excel files (*.xls,*.xlsx)|*.xls*"
            };

            var path = string.Empty;
            if (dlg.ShowDialog() == true)
            {
                String[] ex = dlg.SafeFileName.Split('.');

                //防止*.*的判斷式
                if (ex[ex.Length - 1] != "xls" && ex[ex.Length - 1] != "xlsx")
                {
                    DialogService.ShowMessage("盤點過帳匯入檔必須為Excel檔案，總共有3欄");
                    dlg = null;
                    return "";
                }
                return dlg.FileName;
            }
            return "";


        }

        private void ImportInventoryLocItemSerial_OnClick()
		{
			var dlg = new OpenFileDialog { DefaultExt = ".xls", Filter = "excel files (*.xls,*.xlsx)|*.xls*" };
            if (dlg.ShowDialog() ?? false)
                Vm.ImportFilePath = dlg.FileName;
			else
				Vm.ImportFilePath = "";
		}
    }

}
