using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.P19.Views;
using Wms3pl.WpfClient.UILib;
using System;
using System.Windows.Controls;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.Common;
using System.Text.RegularExpressions;

namespace Wms3pl.WpfClient.P02.Views
{
	public partial class P0202030100 : Wms3plWindow
	{
		private int? OriginalgSaveDayValue;
		public P0202030100(P020203Data baseData, string dcCode, DateTime deliverDate, string rtNo, bool goReadSerialNo = false)
		{
			InitializeComponent();
      Vm.IsFirstLoad = true;
      // 指定序號輸入後要做的事, 必須在Command OnComplete後再處理, 所以用Action串接
			Vm.ActionAfterCheckSerialNo += AfterCheckSerialNo;
			Vm.OnCancelComplete += OnCancel;
			Vm.DoOpenP1901030000 += OnOpenP1901030000;
			Vm.DoClose += Close;
			Vm.SelectedDc = dcCode;
			Vm.DeliverDate = deliverDate;
			Vm.BaseData = baseData;
			Vm.RtNo = rtNo;
			Vm.FirstInDate = baseData.FIRST_IN_DATE;

			Vm.SaveDay = baseData.SAVE_DAY;
			if (baseData.VALI_DATE.HasValue)
				Vm.ValidDate = baseData.VALI_DATE;
			else
        Vm.ValidDate = new DateTime(9999, 12, 31);

			Vm.NeedExpired = baseData.NEED_EXPIRED;

      Vm.EanCode1 = baseData.EAN_CODE1;
			Vm.EanCode2 = baseData.EAN_CODE2;
			Vm.EanCode3 = baseData.EAN_CODE3;
			Vm.EnableEanCode1 = string.IsNullOrWhiteSpace(baseData.EAN_CODE1) ? true : false;
			Vm.EnableEanCode2 = string.IsNullOrWhiteSpace(baseData.EAN_CODE2) ? true : false;
			Vm.EnableEanCode3 = string.IsNullOrWhiteSpace(baseData.EAN_CODE3) ? true : false;
			
			
			Vm.IsPrecious = baseData.IS_PRECIOUS;
			Vm.IsEasyLose = baseData.IS_EASY_LOSE;
			Vm.IsMagnetic = baseData.IS_MAGNETIC;
			Vm.IsPerishable = baseData.IS_PERISHABLE;
			Vm.Fragile = baseData.FRAGILE;
			Vm.Spill = baseData.SPILL;
			Vm.SelectedTmprType = baseData.TMPR_TYPE;
			Vm.IsTempControl = baseData.IS_TEMP_CONTROL;
			Vm.IsApple = baseData.ISAPPLE;
			Vm.BundleSerial = baseData.BUNDLE_SERIALNO;
			// 是否為首次驗收
			if (Vm.FirstInDate != null)
			{

				//不是首次驗收設定商品長寬高重量
				Vm.ItemLenght = baseData.PACK_LENGTH;
				Vm.ItemWidth = baseData.PACK_WIDTH;
				Vm.ItemHight = baseData.PACK_HIGHT;
				Vm.ItemWeight = baseData.PACK_WEIGHT;
				Vm.EnabledSaveDay = false;
			}
			else
			{
				FristInMessage();
				Vm.EnableItemLenght = true;
				Vm.EnableItemWidth = true;
				Vm.EnableItemHight = true;
				Vm.EnableItemWeight = true;
				Vm.EnableNeedExpired = true;
				Vm.EnableBundleSerial = true;
				Vm.EnabledSaveDay = baseData.NEED_EXPIRED == "1";
			}

			Vm.EnableIsPrecious = true;
			Vm.EnableFragile = true;
			Vm.EnableIsEasyLose = true;
			Vm.EnableSpill = true;
			Vm.EnableIsMagnetic = true;
			Vm.EnableIsPerishable = true;
			Vm.EnableIsTempControl = true;
      Vm.EnableIsApple = true;
      Vm.MakeNo = baseData.MAKE_NO;

			if (goReadSerialNo)
			{
				Vm.SelectedTabIndex = 1;
				txtNewSerialNo.Focus();
			}
			else
			{
				Vm.SelectedTabIndex = 0;
			}
      // ToDo: 載入時初始化Properties.Resources.MEMO欄位的啟用狀態 (若原本UCC沒有選取時, 要Disable備註欄), OnInitial/SourceUpdate都沒作用


      Vm.IsFirstLoad = false;

      Vm.DoFoucusValidDate += () =>
      {
        base.SetFocusedElement(txtValidDate, true);
      };

    }


    /// <summary>
    /// 序號輸入後第一個動作: 選取到新的項目
    /// </summary>
    private void AfterCheckSerialNo()
		{
			int idx = 0;
			// Memo: 如果沒有重新指定ItemsSource, 這裡抓到的Items還是舊的資料, 所以永遠不會Focus到新項目去
			dgSerialList.ItemsSource = Vm.DgSerialList;
			foreach (SerialNoResult item in dgSerialList.Items)
			{
				if (item.SerialNo == Vm.NewSerialNo)
				{
					dgSerialList.ScrollIntoView(item);
					dgSerialList.SelectedIndex = idx;
					dgSerialList.Focus();
					break;
				}
				idx++;
			}

			AfterCheckSerialNo1();
		}

		/// <summary>
		/// 序號輸入後第二個動作: 重新Focus回TextBox
		/// </summary>
		private void AfterCheckSerialNo1()
		{
			ControlView(() =>
			{
				base.SetFocusedElement(txtNewSerialNo, true);
			});
		}

		/// <summary>
		/// 檔案上傳
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnFileUpload_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Multiselect = false;
			dlg.DefaultExt = ".jpg";
			dlg.Filter = Properties.Resources.P0202030100_FileUploadType;
			dlg.FileOk += delegate (object s, CancelEventArgs ev)
			{
				foreach (var p in dlg.FileNames)
				{
					if (new FileInfo(p).Length > Wms3pl.WpfClient.Common.GlobalVariables.FileSizeLimit)
					{
						Vm.ShowMessage(Messages.WarningFileSizeExceedLimits);
						ev.Cancel = true;
					}
				}
			};

			// Get the selected file name and display in a TextBox
			if (dlg.ShowDialog() == true)
			{
				Vm.FileName = dlg.FileName;
				Vm.UploadCommand.Execute(null);
			}
		}

		/// <summary>
		/// 按下Properties.Resources.P0202030600_Cancel/ Properties.Resources.P0202030600_Exit時關閉視窗
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancel()
		{
			this.Close();
		}

		/// <summary>
		/// 選擇了原因之後判斷要不要啟用TextBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = (ComboBox)sender;
			var dep = (DependencyObject)e.OriginalSource;
			while ((dep != null) &&
					!(dep is DataGridRow))
			{
				dep = VisualTreeHelper.GetParent(dep);
			}
			if (dep == null) return;

			DataGridRow row = dep as DataGridRow;
			var rowIndex = dgCheckList.Items.IndexOf(row.Item);
			DataGridCell cell = DataGridHelper.GetCell(dgCheckList, rowIndex, 3);
			if (e.AddedItems.Count == 0) return;
			F1951 itemData = e.AddedItems[0] as F1951;
			if (itemData.CAUSE.Equals(Properties.Resources.P0202030100_Other))
			{
				cell.IsEnabled = true;
			}
			else
			{
				var item = dgCheckList.SelectedItem as F190206CheckName;
				item.MEMO = "";
				cell.IsEnabled = false;
			}
		}

		private void IsPass_OnChecked(object sender, RoutedEventArgs e)
		{
			CheckBox check = (CheckBox)sender;

			var dep = (DependencyObject)e.OriginalSource;
			while ((dep != null) &&
					!(dep is DataGridRow))
			{
				dep = VisualTreeHelper.GetParent(dep);
			}
			if (dep == null) return;
			DataGridRow row = dep as DataGridRow;
			var rowIndex = dgCheckList.Items.IndexOf(row.Item);
			DataGridCell cell2 = DataGridHelper.GetCell(dgCheckList, rowIndex, 2);
			var item = dgCheckList.SelectedItem as F190206CheckName;
			if (item != null)
			{
				DataGridCell cell3 = DataGridHelper.GetCell(dgCheckList, rowIndex, 3);
				if (check.IsChecked ?? false)
				{
					item.UCC_CODE = "";
					item.MEMO = "";
					cell2.IsEnabled = false;
					cell3.IsEnabled = false;
				}
				else
				{
					cell2.IsEnabled = true;
				}
			}
		}

		private void OnOpenP1901030000()
		{
			//開啟商品包裝維護
			var win = new P0202030101()
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				FontSize = Application.Current.MainWindow.FontSize,
			};
			win.ShowDialog();
			Vm.ChangeSelectedTab();
		}

		//首次驗收訊息
		public void FristInMessage()
		{
			if (Vm.FirstInDate == null)
			{
				ShowMessage(Properties.Resources.P0202030100_FristInMessage);
			}
		}
		// 國際條碼一重複
		private void TxtEanCode1_OnKeyDown(object sender, RoutedEventArgs e)
		{
			Vm.LostFocusEanCode1();
		}
		// 國際條碼二重複
		private void TxtEanCode2_OnKeyDown(object sender, RoutedEventArgs e)
		{
			Vm.LostFocusEanCode2();
		}

		// 國際條碼三重複
		private void TxtEanCode3_OnKeyDown(object sender, RoutedEventArgs e)
		{
			Vm.LostFocusEanCode3();
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex re = new Regex("^[1-9]?[0-9]*$");
			var txt = ((TextBox)sender).Text + e.Text;

			e.Handled = !re.IsMatch(txt);
		}

  }
}
