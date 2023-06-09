using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
  /// <summary>
  /// P0202030700.xaml 的互動邏輯
  /// </summary>
  public partial class P0202030700 : Wms3plWindow
  {
    public List<F02020109Data> f02020109Datas
    {
      get { return Vm.F02020109Datas; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseData"></param>
    /// <param name="Mode">1:商品檢驗 2:複驗異常處理</param>
    public P0202030700(P020203Data baseData, int Mode = 1, List<F02020109Data> f02020109Datas = null)
    {
      InitializeComponent();
      if (!new[] { 1, 2 }.Contains(Mode))
        throw new Exception("參數Mode無法辨識");
      Vm.SysMode = Mode;
      Vm.BaseData = baseData;
      Vm.GetUserWarehouse(baseData.DC_CODE);
      Vm.ItemCode = baseData.ITEM_CODE;
      Vm.ItemName = baseData.ITEM_NAME;
      Vm.ValiDate = baseData.VALI_DATE;
      Vm.MakeNo = baseData.MAKE_NO;
      Vm.StockNo = baseData.PURCHASE_NO;
      Vm.DcCode = baseData.DC_CODE;
      Vm.CancelComplete += CancelComplete;
      Vm.SaveComplete += SaveComolete;
      Vm.StockSeq = Convert.ToInt32(baseData.PURCHASE_SEQ);
      Vm.RecvQty = baseData.RECV_QTY;

      if (Mode==2)
      {

        Vm.F02020109Datas = f02020109Datas;
        Vm._tempF02020109Datas = f02020109Datas;
				if (f02020109Datas != null && f02020109Datas.Any())
					Vm.SelectedWarehouse = f02020109Datas.First().WAREHOUSE_ID;

      }
      Vm.OnAddScanInputComplete += FocusAfterScanInput;

    }
    private void ExitCommand_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void CancelComplete()
    {
      this.Close();
    }

    private void SaveComolete()
    {
      this.DialogResult = true;
      this.Close();
    }

    private void ScanInput_OnKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(txtScanInput.Text))
      {
        //語音
        if (Vm.PlaySound)
          PlaySoundHelper.Scan();
        Vm.AddDetailCommand.Execute(null);
        //SetDefaultFocusClick(obj);
        //SetFocusedElement(txtScanInput, true);
        FocusAfterScanInput();
      }
    }

    private void FocusAfterScanInput()
    {
      SetFocusedElement(txtScanInput, true);
    }

  }
}
