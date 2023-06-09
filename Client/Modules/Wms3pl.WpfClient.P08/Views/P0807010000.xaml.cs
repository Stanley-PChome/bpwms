using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P08.Views
{
  /// <summary>
  /// P0807010000.xaml 的互動邏輯
  /// </summary>
  public partial class P0807010000 : Wms3plWindow
  {
    public P0807010000()
    {
      InitializeComponent();
      Vm.OnSearchTicketComplete += FocusAfterTicketNo;
      Vm.OnSearchSerialComplete += FocusAfterSerial;
      Vm.OnStartPackingComplete += FocusAfterSerial;
      Vm.OnPausePackingComplete += FocusTicketNo;
      Vm.OnExitPackingComplete += ExitPacking;
      Vm.OnRequireUnlock += OnRequireUnlock;

      Vm.OnDcCodeChanged += DcCodeChanged;
      Vm.OnScrollIntoDeliveryData += ScrollIntoDeliveryData;
      Vm.OnScrollFirstDeliveryData += ScrollFirstDeliveryData;
      Vm.OnScrollIntoSerialReadingLog += ScrollIntodgSerialReadingLog;
      FocusTicketNo();
    }

    private void Window_OnLoaded(object sender, RoutedEventArgs e)
    {
      DcCodeChanged();
    }

    private void txtTicketNo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(txtTicketNo.Text))
      {
        Vm.SearchTicketCommand.Execute(true);
        if (Vm.SoundOn) PlaySoundHelper.Scan();
      }
    }

    private void Wms3plWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Vm.timer.Stop();
      Vm.timer = null;
    }
    private void FocusTicketNo()
    {
      //txtTicketNo.Text = string.Empty;
      SetFocusedElement(txtTicketNo);
    }

    private void FocusAfterTicketNo()
    {
      SetFocusedElement(txtTicketNo);

    }

    /// <summary>
    /// 品號/序號/箱號刷讀後, 重新FOCUS欄位
    /// </summary>
    private void FocusAfterSerial()
    {
      SetFocusedElement(txtSerial, true);
      DispatcherAction(() =>
      {
        if (dgSerialReadingLog.Items.Count > 0) dgSerialReadingLog.SelectedItem = dgSerialReadingLog.Items[0];
      });
    }

    private void txtSerial_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(txtSerial.Text))
      {
        DispatcherAction(() =>
        {
          Vm.ScanBarcode();
          // SetFocusedElement(txtSerial, true);
        });
      }
    }
    private void ExitPacking()
    {
      if (Vm.ShowMessage(Messages.WarningBeforeExit) == UILib.Services.DialogResponse.OK)
        this.Close();
    }

    /// <summary>
    /// 主管解鎖
    /// </summary>
    private void OnRequireUnlock()
    {
      var win = new P0807010200(Vm.SelectedDc, Vm._gupCode, Vm._custCode) { Owner = Wms3plViewer.GetWindow(this) };
      win.ShowDialog();
      SetFocusedElement(txtTicketNo);
    }

    private void Wms3plWindow_Activated(object sender, EventArgs e)
    {
      if (Vm.EnableReadSerial) SetFocusedElement(txtSerial);
      else SetFocusedElement(txtTicketNo);
    }


    private void DcCodeChanged()
    {
      var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDc);
      if (openDeviceWindow.Any())
      {
        Vm.SelectedF910501 = openDeviceWindow.FirstOrDefault();
      }
      else
      {
        var deviceWindow = new DeviceWindow(Vm.SelectedDc);
        deviceWindow.Owner = this;
        deviceWindow.ShowDialog();
        Vm.SelectedF910501 = deviceWindow.SelectedF910501;
      }

    }

    void ScrollIntoDeliveryData()
    {
      if (dgItems.SelectedItem != null)
      {
        dgItems.ScrollIntoView(dgItems.SelectedItem);
      }
    }

    void ScrollFirstDeliveryData()
    {
      if (Vm.DlvData != null && Vm.DlvData.Any())
        dgItems.ScrollIntoView(Vm.DlvData.First());
    }

    void ScrollIntodgSerialReadingLog()
    {
      if (dgSerialReadingLog.Items.Count > 0)
      {
        dgSerialReadingLog.ScrollIntoView(dgSerialReadingLog.Items[dgSerialReadingLog.Items.Count - 1]);
      }
    }

    private void StartPacking_Click(object sender, RoutedEventArgs e)
    {
      txtTicketNo_KeyDown(sender, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter));
    }
  }
}
