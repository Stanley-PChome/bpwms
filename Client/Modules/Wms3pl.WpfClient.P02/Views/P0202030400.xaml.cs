using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.UILib;
using System;
using System.Windows.Controls;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P02.Views
{
	public partial class P0202030400 : Wms3plWindow
	{
		public P0202030400(List<P020203Data> baseData, string dcCode, string purchaseNo)
		{
			InitializeComponent();
			Vm.SelectedDc = dcCode;
			Vm.PurchaseNo = purchaseNo;
			Vm.BaseData = baseData;
			Vm.OnCancelComplete += OnCancel;
		}

		private void OnCancel()
		{
			this.Close();
		}

		private bool _isCheckUCC = true;
		/// <summary>
		/// 1. 判斷能不能點選先入帳進貨
		/// 2. 選擇了原因之後判斷要不要啟用TextBox
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
			var rowIndex = dgList.Items.IndexOf(row.Item);
			DataGridCell cell = DataGridHelper.GetCell(dgList, rowIndex, 7);
			F1951 itemData = null;
			if (e.AddedItems.Count > 0) itemData = e.AddedItems[0] as F1951;

			// 1. 已檢驗但序號未刷驗，原因才可選擇《先入帳進貨 》
			if (_isCheckUCC)
			{
				var tmp = row.Item as P020203Data;
				if (!(tmp.CHECK_SERIAL == "0" && tmp.CHECK_ITEM == "1") 
					&& (itemData.CAUSE.Equals(Properties.Resources.P0202030400_UccCode201) || itemData.UCC_CODE.Equals("201")))
				{
					Vm.ShowMessage(Messages.WarningNotAllowedUCCTypeForSpecialPurchase);
					_isCheckUCC = false;
					if (e.RemovedItems.Count == 0)
						cb.SelectedItem = null;
					else
						cb.SelectedItem = e.RemovedItems[0];
					_isCheckUCC = true;
					return;
				}
			}

			// 2. 啟用/停用其它原因欄位
			if (itemData != null && (itemData.CAUSE.Equals(Properties.Resources.P0202030400_Other) || itemData.UCC_CODE.Equals("200")))
			{
				cell.IsEnabled = true;
			}
			else
			{
				cell.IsEnabled = false;
			}
		}
	}
}
