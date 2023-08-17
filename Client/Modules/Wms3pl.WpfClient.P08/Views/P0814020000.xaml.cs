using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0814020000.xaml 的互動邏輯
	/// </summary>
	public partial class P0814020000 : Wms3plWindow
	{
		DispatcherTimer dispatcherTimer;
		private bool _isContainerLock = false;
		private bool _isBarcodeLock = false;
		public P0814020000()
		{
			InitializeComponent();
			Vm.FocusSelectAllContainer += FocusSelectAllContainer;
			Vm.FocusSelectAllSerial += FocusSelectAllSerial;
			Vm.OnExitPackingComplete += ExitPacking;
			Vm.OpenContainerWindow += OpenContainer;
			Vm.OnDcCodeChanged += DcCodeChanged;
			Vm.OnScrollIntoDetailData += ScrollIntoDetailData;
			Vm.OnScrollIntoSerialReadingLog += ScrollIntodgSerialReadingLog;
			Vm.OnStartDispatcher += StartDispatcher;
			Vm.OnStopDispatcher += StopDispatcher;

			Vm.ShipMode = "2";
			dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, Vm.GetAutoFindContainerPeriod()) };
			dispatcherTimer.Tick -= ExecuteDispatcher;
			dispatcherTimer.Tick += ExecuteDispatcher;
		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			DcCodeChanged();
		}

		/// <summary>
		/// 物流中心變換事件
		/// </summary>
		private void DcCodeChanged()
		{
			var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDc);
			if (openDeviceWindow.Any())
			{
				Vm.SelectedF910501 = openDeviceWindow.FirstOrDefault();
			}
			else
			{
        Vm.ShowWarningMessage("請先設定印表機與工作站編號，請至裝置設定維護調整，本功能即將關閉");
        this.Close();
        return;
      }

      // 組印表機資訊、工作站編號
      if (Vm.SelectedF910501 != null)
				Vm.DcChangeSetValue();
		}

		/// <summary>
		/// 容器條碼Enter事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtContainer_KeyDown(object sender, KeyEventArgs e)
		{
      Boolean deviceCheckOk = true;

      if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(txtContainer.Text))
			{
				var isScan = false;
				try
				{
					if (!_isContainerLock)
					{
						Vm.IsBusy = true;
						_isContainerLock = true;
						isScan = true;

						if (!string.IsNullOrWhiteSpace(Vm.ContainerCode))
							Vm.ContainerCode = Vm.ContainerCode.ToUpper();

            // 檢查裝置設定
            deviceCheckOk = CheckDeviceSetting(ref Vm.SelectedF910501, Vm.ShipMode, Vm.SelectedDc);
            if (!deviceCheckOk)
              return;

            Vm.BindingDeviceSetting();
						Vm.IsBusy = false;
						Vm.SearchContainerCommand.Execute(true);
						if (Vm.SoundOn) PlaySoundHelper.Scan();
					}
				}
				finally
				{
          if (!deviceCheckOk)
          {
            Vm.IsBusy = false;
            this.Close();
          }

          if (_isContainerLock && isScan)
					{
						_isContainerLock = false;
					}
				}
			}
		}

		/// <summary>
		/// 商品條碼Enter事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtSerial_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(txtSerial.Text))
			{
				var isScan = false;
				try
				{
					if (!_isBarcodeLock)
					{
						Vm.IsBusy = true;
						_isBarcodeLock = true;
						isScan = true;
						if (!string.IsNullOrWhiteSpace(Vm.Serial))
							Vm.Serial = Vm.Serial.ToUpper();
						Vm.IsBusy = false;
						Vm.ScanBarcodeCommand.Execute("01");
					}

				}
				finally
				{
					if (_isBarcodeLock && isScan)
					{
						_isBarcodeLock = false;
					}
				}
			}
		}

		/// <summary>
		/// 工作站設定
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WorkStationSetting_OnClick(object sender, RoutedEventArgs e)
		{
			// 如果功能為包裝線包裝站，且工作站狀態!=關站(0)
			if (Vm._f1946 != null && Vm._f1946.STATUS != "0")
				StopDispatcher();//停止背景程式[取得容器位置回報已抵達此工作站的容器]

      // 檢查裝置設定
      if (!CheckDeviceSetting(ref Vm.SelectedF910501, Vm.ShipMode, Vm.SelectedDc))
      {
        this.Close();
        return;
      }

      var win = new P0814010100(Vm.SelectedDc, Vm.ShipMode, Vm.DetailData != null && Vm.DetailData.Any(x => x.DiffQty > 0) && (Vm.WmsOrdData?.Result.IsSuccessed == true || Vm.WmsOrdData?.IsCancelOrder == true));
      win.ShowDialog();
      Vm.SelectedF910501.CLOSE_BY_BOXNO = win.Vm.CloseByBoxno;
      Vm.SelectedF910501.NO_SPEC_REPROTS = win.Vm.NoSpecReports;
			Vm.DcChangeSetValue();

			if (Vm._f1946.STATUS == "0")// 關站
			{
				StopDispatcher();
				Vm.Message = "請按[離開]，關閉包裝線包裝站";
				Vm.MessageForeground = Brushes.Blue;
			}
			else if (Vm._f1946.STATUS == "1" || Vm._f1946.STATUS == "2")// 開站 或 暫停
			{
				if (string.IsNullOrWhiteSpace(Vm.ContainerCode))
					StartDispatcher();
			}
			else if (Vm._f1946.STATUS == "3") // 關站中
				StopDispatcher();
		}

		/// <summary>
		/// 補印
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Reprint_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new P0814010200();
			//1.無出貨單時:
			if (Vm.WmsOrdData == null)
			{
				//  開啟[1.2.15 補印UI](null, null, null, null)
				win.ShowDialog();
			}
			else
			{
				//2.有出貨單時:[A]=前端已記錄的1.1.2回傳的物件
				if (Vm.DetailData.All(x => x.TotalPackageQty == 0))
				{
					//(1)如果出貨明細累積包裝數=0 彈出視窗[此出貨單無任何關箱資料，不須補印]
					ShowMessage("此出貨單無任何關箱資料，不須補印");
				}
				else
				{
          //(2)如果出貨明細累績包裝數>0 開啟[1.2.15 補印UI] ([A].DcCode, [A].GupCode[A].CustCode, [A].WmsOrdNo)
          win = new P0814010200(Vm.WmsOrdData.DcCode, Vm.WmsOrdData.GupCode, Vm.WmsOrdData.CustCode, Vm.WmsOrdData.WmsOrdNo);
          win.ShowDialog();
				}
			}
		}

		/// <summary>
		/// 離開
		/// </summary>
		private void ExitPacking()
		{
			this.Close();
		}

		private void Wms3plWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}

		/// <summary>
		/// 容器刷讀
		/// </summary>
		private void OpenContainer()
		{
			var win = new P0814010300(Vm.WmsOrdData.DcCode, Vm.WmsOrdData.GupCode, Vm.WmsOrdData.CustCode, Vm.WmsOrdData.WmsOrdNo, Vm.ContainerCode);
			Vm.ContainerRes = win.ShowDialog();
		}

		/// <summary>
		/// 容器條碼Focus並全選
		/// </summary>
		private void FocusSelectAllContainer()
		{
			SetFocusedElement(txtContainer, true);
		}

		/// <summary>
		/// 商品條碼Focus並全選
		/// </summary>
		private void FocusSelectAllSerial()
		{
			DispatcherAction(() =>
			{
				SetFocusedElement(txtSerial, true);
			});
		}

		private void Wms3plWindow_Activated(object sender, EventArgs e)
		{
			if (Vm.EnableReadSerial) SetFocusedElement(txtSerial);
			else SetFocusedElement(txtContainer);
		}

		void ScrollIntoDetailData()
		{
			DispatcherAction(() =>
			{
				if (dgItems.Items.Count > 0 && Vm.SelectItem != null)
				{
					dgItems.ScrollIntoView(Vm.SelectItem);
					dgItems.SelectedItem = Vm.SelectItem;
				}
			});
		}

		void ScrollIntodgSerialReadingLog()
		{
			DispatcherAction(() =>
			{
				if (dgSerialReadingLog.Items.Count > 0)
				{
					var item = dgSerialReadingLog.Items[dgSerialReadingLog.Items.Count - 1];
					dgSerialReadingLog.ScrollIntoView(item);
					dgSerialReadingLog.SelectedItem = item;
				}
			});
		}

		#region 出貨包裝_檢查裝置設定
		/// <summary>
		/// 出貨包裝_檢查裝置設定
		/// </summary>
		/// <param name="f910501"></param>
		/// <param name="shipMode"></param>
		/// <param name="dcCode"></param>
		public Boolean CheckDeviceSetting(ref F910501 f910501, string shipMode, string dcCode)
		{
      var errMsgs = new List<string>();
      // 是否列印一般出貨小白標
      var f0003 = GetSysSetting(Wms3plSession.Get<GlobalInfo>().FunctionCode, dcCode, "IsPrintShipLittleLabel");
      var isPrintShipLittleLabel = f0003 != null && f0003.SYS_PATH == "0" ? "0" : "1";

      var f910501Exist = OpenDeviceWindow(Wms3plSession.Get<GlobalInfo>().FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, dcCode).FirstOrDefault();

      return Vm._shipPackageService.CheckDeviceSetting(ref f910501, shipMode, dcCode, f0003, f910501Exist);

    }

    /// <summary>
    /// 開啟裝置設定
    /// </summary>
    /// <param name="f910501"></param>
    /// <param name="msg"></param>
    /// <param name="dcCode"></param>
    private void ShowDeviceSettingMsgAndOpenSetting(ref F910501 f910501, string msg, string dcCode)
		{
			if (Vm.ShowWarningMessage(msg) == UILib.Services.DialogResponse.OK)
			{
				var deviceWindow = new DeviceWindow(dcCode);
				deviceWindow.Owner = this;
				deviceWindow.ShowDialog();
				f910501 = deviceWindow.SelectedF910501;
			}
		}
		#endregion

		#region 背景排程

		private void ExecuteDispatcher(object sender, EventArgs e)
		{
			if (!Vm.IsBusy)
				Vm.ExecuteScheduleCommand.Execute(null);
		}

		private void StartDispatcher()
		{
			dispatcherTimer.Start();
			Vm.ExecuteScheduleCommand.Execute(null);
		}

		private void StopDispatcher()
		{
			dispatcherTimer.Stop();
		}
		#endregion

	}
}
