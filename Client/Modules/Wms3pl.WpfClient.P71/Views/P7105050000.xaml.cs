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
using Microsoft.Data.OData.Query.SemanticAst;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7105050000.xaml 的互動邏輯
	/// </summary>
	public partial class P7105050000 : Wms3plUserControl
	{
		public P7105050000()
		{
			InitializeComponent();
			Vm.SetQueryFocus += SetQueryFocus;
			Vm.SetAddNewFocus += SetAddNewFocus;
			Vm.LockDelvTmprComboBox += LockDelvTmprComboBox;
			Vm.UnlockDelvTmprComboBox += UnlockDelvTmprComboBox;
			Vm.ResetUI += ResetUI;
			Vm.ScrollToTop += ScrollToTop;
			SetFocusedElement(DcComboBoxForSearch);
		}

		private void AccItemKindCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add || Vm.UserOperateMode == OperateMode.Edit)
			{
				if (Vm.CurrentRecord == null) return;
				Vm.DelvAccTypeAddList = Vm.GetDelvAccTypeList(Vm.CurrentRecord.ACC_ITEM_KIND_ID);
			}
		}

		private void AccKindCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb.SelectedIndex == -1) return;

			if (Vm.CurrentRecord != null)
			{
				AccKindDetailA.Visibility = Vm.CurrentRecord.ACC_KIND == "F" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailB.Visibility = Vm.CurrentRecord.ACC_KIND == "C" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailC.Visibility = Vm.CurrentRecord.ACC_KIND == "D" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailD.Visibility = Vm.CurrentRecord.ACC_KIND == "E" ? Visibility.Visible : Visibility.Collapsed;				
			}
		}

		private void SetQueryFocus()
		{
			SetFocusedElement(DcComboBoxForSearch);
		}

		private void SetAddNewFocus()
		{
			SetFocusedElement(DcComboBoxForAddNew);
		}

		private void LockDelvTmprComboBox()
		{
			DelvTmprComboBox.IsEnabled = false;
		}

		private void UnlockDelvTmprComboBox()
		{
			DelvTmprComboBox.IsEnabled = true;
		}

		private void ResetUI()
		{
			SpecialCarCheckBox.IsChecked = false;
			DelvTmprComboBox.IsEnabled = true;
			SpecialCarInputGrid.IsEnabled = false;
			AccKindDetailA.Visibility = Visibility.Collapsed;
			AccKindDetailB.Visibility = Visibility.Collapsed;
			AccKindDetailC.Visibility = Visibility.Collapsed;
			AccKindDetailD.Visibility = Visibility.Collapsed;
		}

		private void ScrollToTop()
		{
			if (SearchResultDataGrid.Items.Count > 0)
				SearchResultDataGrid.ScrollIntoView(SearchResultDataGrid.Items[0]);
		}

		private void LogType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Vm.AccUnitList = Vm.GetAccUnitList(Vm.CurrentRecord != null ? Vm.CurrentRecord.LOGI_TYPE : "01");
		}
	}
}
