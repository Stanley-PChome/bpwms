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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
    /// <summary>
    /// P1604010000.xaml 的互動邏輯
    /// </summary>
    public partial class P1604010000 : Wms3plUserControl
    {
        public P1604010000()
        {
            InitializeComponent();
            Vm.ExcelImport += ExcelImport;

        }

        private void btnLockSearch_Click(object sender, RoutedEventArgs e)
        {
            var win = new P1604010100();
            win.SetBaseData(Vm.SelectedDc, Vm._gupCode, Vm._custCode, Vm.WarehouseList, Vm.ScrapResonList);
            var result = win.ShowDialog();
            if (result == true)
            {
                var ScrapDetailByLoc = win.Vm.ScrapAddDetailList.Where(x => x.IsSelected).Select(it => it.Item).ToList();
                Vm.ScrapDetailSaveByLoc(ScrapDetailByLoc);
            }
        }

        bool ImportResultData = false;
        private void ExcelImport()
        {
            var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P1604010000"));

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

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            var win = new WinSearchProduct();
            win.GupCode = Vm._gupCode;
            win.CustCode = Vm._custCode;
            win.Owner = this.Parent as Window;
            win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.Value)
            {
                SetSearchData(win.SelectData);
            }
        }

        private void SetSearchData(F1903 f1903)
        {
            if (f1903 != null)
            {
                Vm.CurrentDetailRecord.ITEM_CODE = f1903.ITEM_CODE;
                Vm.CurrentDetailRecord.ITEM_NAME = f1903.ITEM_NAME;
            }
            else
            {
                Vm.CurrentDetailRecord.ITEM_CODE = string.Empty;
                Vm.CurrentDetailRecord.ITEM_NAME = string.Empty;
            }
        }
    }
}
