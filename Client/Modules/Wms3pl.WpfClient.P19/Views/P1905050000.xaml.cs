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
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// Wms3plWpfClientUserControl1.xaml 的互動邏輯
	/// </summary>
	public partial class P1905050000 : Wms3plUserControl
	{
		public P1905050000()
		{
			InitializeComponent();
			Vm.ScrollIntoViewLast += Vm_ScrollIntoViewLast;
			Vm.EditModeFocus += Vm_EditModeFocus;
			SetFocusedElement(txtGroupName);
		}

		/// <summary>
		/// 編輯時，將焦點放到人員查詢條件的第一個欄位
		/// </summary>
		private void Vm_EditModeFocus()
		{
			SetFocusedElement(txtEmpId);
		}

		/// <summary>
		/// 當移動員工時，也做 ListBox 捲軸移到最後的項目
		/// </summary>
		/// <param name="obj"></param>
		private void Vm_ScrollIntoViewLast(ViewModel.P190505MovingType obj)
		{
			switch (obj)
			{
				case ViewModel.P190505MovingType.Assign:
				case ViewModel.P190505MovingType.AssignAll:
					lstAssigned.ScrollIntoView(Vm.AssignEmpItems.LastOrDefault());
					break;

				case ViewModel.P190505MovingType.UnAssign:
				case ViewModel.P190505MovingType.UnAssignAll:
					lstUnAssigned.ScrollIntoView(Vm.UnassignEmpItems.LastOrDefault());
					break;
			}
		}

		//private void btnAssign_Click(object sender, RoutedEventArgs e)
		//{
		//	Vm.SelectedEmpList = new List<DataServices.F19DataService.F1924>();
		//	// 從已設定人員移除
		//	if (((Button)sender).Name.Equals("btnUnAssign"))
		//	{
		//		foreach (DataServices.F19DataService.F1924 p in lstAssigned.SelectedItems)
		//		{
		//			Vm.SelectedEmpList.Add(p);
		//		}
		//		//Vm.DoMoveItems(ViewModel.P190505MovingType.UnAssign);
		//	}
		//	// 加入已設定人員
		//	if (((Button)sender).Name.Equals("btnAssign"))
		//	{
		//		foreach (DataServices.F19DataService.F1924 p in lstUnAssigned.SelectedItems)
		//		{
		//			Vm.SelectedEmpList.Add(p);
		//		}
		//		//Vm.DoMoveItems(ViewModel.P190505MovingType.Assign);
		//	}
		//}

		//Object _SelectedItem; //_SelectedItem is used to avoid the repeated loops
		//DataGridCell _FocusedCell; //_FocusedCell is used to restore focus
		//private void dgGroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	DataGrid obj = (DataGrid)sender;
		//	if (_SelectedItem == obj.SelectedItem)
		//	{
		//		return;
		//	}
		//	_SelectedItem = obj.SelectedItem;

		//	if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != null &&
		//		Vm.ConfirmBeforeAction() == DialogResponse.Cancel)
		//	{
		//		_SelectedItem = e.RemovedItems[0];
		//		Dispatcher.BeginInvoke(
		//		  new Action(() =>
		//		  {
		//			  obj.SelectedItem = e.RemovedItems[0];
		//			  if (_FocusedCell != null)
		//			  {
		//				  _FocusedCell.Focus();
		//			  }
		//		  }), System.Windows.Threading.DispatcherPriority.Send);
		//		return;
		//	}
		//	Vm.SelectedGroupItem = (F1953)_SelectedItem;
		//}

	}
}
