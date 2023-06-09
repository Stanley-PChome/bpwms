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
using Wms3pl.WpfClient.P05.Views;
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
	/// <summary>
	/// P1603010000.xaml 的互動邏輯
	/// </summary>
	public partial class P1603010000 : Wms3plUserControl
	{
		public P1603010000()
		{
			InitializeComponent();
			Vm.AddAction += AddCommand_Executed;
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
			Vm.CancelAction += CancelCommand_Executed;
			Vm.CollapsedQryResultAction += CollapsedQryResult_Excuted;
			Vm.AddDetailAction += detailItem =>
			{
				ScrollIntoView(dgDetailForAdd, detailItem);
			};
			Vm.AddDetailForEditAction += detailItem =>
			{
				ScrollIntoView(ItemDetailList, detailItem);
			};
		}

		private void AddCommand_Executed()
		{
			//SetFocusedElement(Txt_RETAIL_CODE);
		}

		private void EditCommand_Executed()
		{
			//SetFocusedElement(txt_CUST_ORD_NO);
			ItemDetailList.Columns[0].Visibility = Visibility.Visible;
			ItemDetailList.Columns[7].IsReadOnly = false;

			//if (CHK_RTN_FIRST_EDIT.IsChecked==true)
			//	CHK_RTN_AFTER_EDIT.IsEnabled = false;
			//else
			//	CHK_RTN_AFTER_EDIT.IsEnabled = true;

			//if (CHK_RTN_AFTER_EDIT.IsChecked == true)
			//	CHK_RTN_FIRST_EDIT.IsEnabled = false;
			//else
			//	CHK_RTN_FIRST_EDIT.IsEnabled = true;

		}

		private void SearchCommand_Executed()
		{
			//	SetFocusedElement(txtSearchItemCode);
			ExpQueryCondition.IsExpanded = false;
			ExpQueryResult.IsExpanded = true;
		}

		private void CancelCommand_Executed()
		{
			//SetFocusedElement(txtBomName);
			ItemDetailList.Columns[0].Visibility = Visibility.Collapsed;
			ItemDetailList.Columns[7].IsReadOnly = true;
			ExpQueryCondition.IsExpanded = true;
			ExpQueryResult.IsExpanded = true;

		}

		private void CollapsedQryResult_Excuted()
		{
			ExpQueryCondition.IsExpanded = false;
			ExpQueryResult.IsExpanded = false;
		}


		private void Chk_Rtn_First_Click(object sender, RoutedEventArgs e)
		{
			//CheckBox cb = e.Source as CheckBox;
			//if (cb.IsChecked == true)
			//{
			//	if (cb.Name.Equals("CHK_RTN_FIRST_ADD"))
			//	{
			//		CHK_RTN_AFTER_ADD.IsEnabled = false;
			//	}
			//	else
			//	{
			//		CHK_RTN_AFTER_EDIT.IsEnabled = false;
			//	}
			//}
			//else
			//{
			//	if (cb.Name.Equals("CHK_RTN_FIRST_ADD"))
			//	{
			//		CHK_RTN_AFTER_ADD.IsEnabled = true;
			//	}
			//	else
			//	{
			//		CHK_RTN_AFTER_EDIT.IsEnabled = true;
			//	}
			//}
		}

		private void Chk_Rtn_After_Click(object sender, RoutedEventArgs e)
		{
			//CheckBox cb = e.Source as CheckBox;
			//if (cb.IsChecked == true)
			//{
			//	if (cb.Name.Equals("CHK_RTN_AFTER_ADD"))
			//	{
			//		CHK_RTN_FIRST_ADD.IsEnabled = false;
			//	}
			//	else
			//	{
			//		CHK_RTN_FIRST_EDIT.IsEnabled = false;
			//	}
			//}
			//else
			//{
			//	if (cb.Name.Equals("CHK_RTN_AFTER_ADD"))
			//	{
			//		CHK_RTN_FIRST_ADD.IsEnabled = true;
			//	}
			//	else
			//	{
			//		CHK_RTN_FIRST_EDIT.IsEnabled = true;
			//	}
			//}

			Vm.RaiseChgCustNameIsReadOnly();
		}

		private void F050801_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (Vm.selectedF050801Q != null)
			{
				var win = new P0503020100(Vm.selectedF050801Q.GUP_CODE, Vm.selectedF050801Q.CUST_CODE, Vm.selectedF050801Q.DC_CODE, Vm.selectedF050801Q.WMS_ORD_NO)
				{
					Owner = System.Windows.Window.GetWindow(this),
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				};
				win.ShowDialog();
			}
		}

		private void Txt_Retail_Code_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.SetRetailInfo();
			}
		}

		private void Txt_Retail_Code_LostFocus(object sender, RoutedEventArgs e)
		{
			Vm.SetRetailInfo();
		}
	}
}