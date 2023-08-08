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
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P08.Views
{
  /// <summary>
  /// P0808060100.xaml 的互動邏輯
  /// </summary>
  public partial class P0808060100 : Wms3plWindow
  {
    public P0808060100()
    {
      InitializeComponent();
      Vm.DoContainerFocus = ContainerCodeFocus;
    }
    public P0808060100(string dcCode) : base()
    {
      InitializeComponent();
      Vm.DoContainerFocus = ContainerCodeFocus;
    }

    #region 綁定箱號/加箱/重綁箱號

    private bool OpenBoxWin(BindBoxType bindBoxType, bool isAddBox)
    {
      //var orgBoxNo = bindBoxType == BindBoxType.NormalShipBox ? Vm.NormalShipBox.BoxNo : Vm.CancelOrderBox.BoxNo;
      //var win = new P0808040300(Vm.SelectedDc, Vm.CurrentContainerPickInfo, bindBoxType, isAddBox, orgBoxNo);
      //win.Owner = this;
      //win.ShowDialog();
      //if (win.Vm.IsOk)
      //{
      //	if(isAddBox && !string.IsNullOrWhiteSpace(orgBoxNo))
      //	{
      //		Vm.IsBusy = true;
      //		var datas = Vm.GetPrintBoxDetail(bindBoxType, orgBoxNo);
      //		// 列印前一箱明細
      //		Vm.DoPrintBoxDetail(bindBoxType, datas,false);
      //		Vm.IsBusy = false;
      //	}
      //	switch (bindBoxType)
      //	{
      //		case BindBoxType.NormalShipBox:
      //			Vm.NormalShipBox.Init();
      //			Vm.NormalShipBox.BoxNo = win.Vm.BoxInfo.BoxNo;
      //			Vm.NormalShipBox.SowQty = win.Vm.BoxInfo.SowQty;
      //			break;
      //		case BindBoxType.CanelOrderBox:
      //			Vm.CancelOrderBox.Init();
      //			Vm.CancelOrderBox.BoxNo = win.Vm.BoxInfo.BoxNo;
      //			Vm.CancelOrderBox.SowQty = win.Vm.BoxInfo.SowQty;
      //			break;
      //	}
      //}
      //return win.Vm.IsOk;
      return true;
    }

    private void BindBox()
    {
      //if (Vm.CurrentContainerPickInfo != null && Vm.CurrentContainerPickInfo.NormalOrderCnt > 0 && string.IsNullOrWhiteSpace(Vm.NormalShipBox.BoxNo))
      //{
      //	var isOk = OpenBoxWin(BindBoxType.NormalShipBox,  true);
      //	if(!isOk)
      //	{
      //		Vm.Init();
      //		return;
      //	}
      //}

      //if (Vm.CurrentContainerPickInfo != null && Vm.CurrentContainerPickInfo.CancelOrderCnt > 0 && string.IsNullOrWhiteSpace(Vm.CancelOrderBox.BoxNo))
      //{
      //	var isOk = OpenBoxWin(BindBoxType.CanelOrderBox, true);
      //	if (!isOk)
      //	{
      //		Vm.Init();
      //		return;
      //	}
      //}
      //ItemBarCodeFocus();
    }

    private void DoAddBox(BindBoxType bindBoxType)
    {
      OpenBoxWin(bindBoxType, true);
      ItemBarCodeFocus();

    }

    private void DoRebindBox(BindBoxType bindBoxType)
    {
      OpenBoxWin(bindBoxType, false);
      ItemBarCodeFocus();
    }
    #endregion

    #region 刷讀品號/國條/商品序號
    private void ScanItemBarCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Enter)
        return;

    }
    #endregion

    #region Focus

    private void ContainerCodeFocus()
    {
      SetFocusedElement(txtOutContainerCode, true);
    }

    private void ItemBarCodeFocus()
    {
      //SetFocusedElement(TxtScanItemBarCode, true);
    }
    #endregion

    #region 物流中心變更事件
    private void DcChange()
    {
      var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDc);
      if (!openDeviceWindow.Any())
      {
        var deviceWindow = new DeviceWindow(Vm.SelectedDc);
        deviceWindow.Owner = this.Parent as Window;
        deviceWindow.ShowDialog();
      }
    }
    #endregion

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      DispatcherAction(() =>
      {
        ContainerCodeFocus();
      });
    }

    #region 刷讀容器條碼
    private void TxtContainerCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Enter)
        return;

      Vm.SearchCommand.Execute(null);
    }
    #endregion

    #region 箱內明細

    private void BoxDetail_Click(object sender, RoutedEventArgs e)
    {
      //if(Vm.CurrentContainerPickInfo != null)
      //{
      //	var win = new P0808040100(Vm.SelectedDc,Vm.CurrentContainerPickInfo.DelvDate,Vm.CurrentContainerPickInfo.PickTime);
      //	win.Owner = this;
      //	win.ShowDialog();
      //	DispatcherAction(() =>
      //	{
      //		ItemBarCodeFocus();
      //	});
      //}
      //else
      //{
      //	var win = new P0808040100();
      //	win.Owner = this;
      //	win.ShowDialog();
      //	DispatcherAction(() =>
      //	{
      //		ContainerCodeFocus();
      //	});
      //}
    }
    #endregion

    #region 揀貨單批次查詢
    private void CloseBox_Click(object sender, RoutedEventArgs e)
    {
      var win = new P0808040400(Vm.SelectedDc);
      win.Owner = this;
      win.ShowDialog();
      DispatcherAction(() =>
      {
        ContainerCodeFocus();
      });
    }
    #endregion

    private void TxtOutContainerCode_KeyDown(object sender, KeyEventArgs e)
    {

    }
  }
}
