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
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
  /// <summary>
  /// P1605010000.xaml 的互動邏輯
  /// </summary>
  public partial class P1605010000 : Wms3plUserControl
  {
    public P1605010000()
    {
      InitializeComponent();
    }

    private void UploadFile_OnClick(object sender, RoutedEventArgs e)
    {
      if (Vm.F160501SelectData == null)
      {
        Vm.DialogService.ShowMessage(Properties.Resources.P1605010000xamlcs_ChooseData);
      }
      else
      {

        var isCanEdit = Vm.CanEditImageUpload();
        if (!isCanEdit)
        {
          DialogService.ShowMessage(Properties.Resources.P1605010000xamlcs_StatusInvalidToModify);
          return;
        }
        var win = new P1605010100(Vm.F160501SelectData.DESTROY_NO
                    , Vm.F160501SelectData.DC_CODE
                    , Vm.F160501SelectData.GUP_CODE
                    , Vm.F160501SelectData.CUST_CODE
                    , Vm.F160501SelectData.CRT_DATE
                    , isCanEdit, Vm);
        win.ShowDialog();
      }
    }


    private void ImportItem(object sender, RoutedEventArgs e)
    {
      bool ImportResultData = false;
      var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P1605010000"), "BP1605010009.xlsx");

      //匯入檔案-Type 參數   1 : 一般匯入  2 :虛擬商品序號滙入
      win.ImportResult = (t) => { ImportResultData = t; };
      win.ShowDialog();
      Vm.ImportFilePath = null;
      if (ImportResultData)
      {
        Vm.ImportType = 1;
        Vm.ImportFilePath = OpenFileDialogFun();
        Vm.ImportCommand.Execute(null);
      }
    }
    private void ImportItemSerial(object sender, RoutedEventArgs e)
    {
      bool ImportResultData = false;
      var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P1605010000"), "BP1605010010.xlsx");

      //匯入檔案-Type 參數   1 : 一般匯入  2 :虛擬商品序號滙入
      win.ImportResult = (t) => { ImportResultData = t; };
      win.ShowDialog();
      Vm.ImportFilePath = null;
      if (ImportResultData)
      {
        Vm.ImportType = 2;
        Vm.ImportFilePath = OpenFileDialogFun();
        Vm.ImportCommand.Execute(null);
      }
    }

    private string OpenFileDialogFun()
    {
      var dlg = new Microsoft.Win32.OpenFileDialog
      {
        DefaultExt = ".xls",
        Filter = "excel files (*.xls,*.xlsx)|*.xls*|csv files (*.csv)|*.csv"
      };

      if (dlg.ShowDialog() == true)
      {
        return dlg.FileName;
      }
      return "";
    }

    private void OpenF0501(object sender, MouseButtonEventArgs e)
    {
      if (Vm.SelectedF050801Data != null)
      {
        if (!string.IsNullOrEmpty(Vm.SelectedF050801Data.WMS_ORD_NO))
        {

          var win = new Wms3pl.WpfClient.P05.Views.P0503020100(Vm.SelectedF050801Data.GUP_CODE,
                                     Vm.SelectedF050801Data.CUST_CODE,
                                     Vm.SelectedF050801Data.DC_CODE,
                                     Vm.SelectedF050801Data.WMS_ORD_NO);
          //win.Topmost = true;
          win.ShowDialog();
        }
      }
    }

    private void BeforeItemChanged(object sender, RoutedEventArgs e)
    {
      Vm.ItemScrapQty = 0;
    }

    private void AfterItemChanged(object sender, RoutedEventArgs e)
    {
      Vm.GetScrapCommand.Execute(null);
    }
  }
}
