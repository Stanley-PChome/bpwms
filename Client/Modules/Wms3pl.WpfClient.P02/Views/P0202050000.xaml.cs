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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// P0202050000.xaml 的互動邏輯
	/// </summary>
	public partial class P0202050000 : Wms3plUserControl
	{
		public P0202050000()
		{
			InitializeComponent();
            Vm.ExcelImport += ExcelImport;
        }

        bool ImportResultData = false;
        private void ExcelImport()
        {
            var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P0202050000"));

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
				Filter = "excel files (*.xls,*.xlsx)|*.xls*"
			};

			var path = string.Empty;
			if (dlg.ShowDialog() == true)
			{
                String[] ex = dlg.SafeFileName.Split('.');

                //防止*.*的判斷式
                if (ex[ex.Length - 1] != "xls" && ex[ex.Length - 1] != "xlsx")
                {
                    DialogService.ShowMessage("進倉單匯入檔必須為Excel檔案，總共有7欄");
                    dlg = null;
                    return "";
                }
                return dlg.FileName;
            }
            return "";

            
        }
	}
}
