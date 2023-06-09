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
	/// P1905080000.xaml 的互動邏輯
	/// </summary>
	public partial class P1905080000 : Wms3plUserControl
	{
		public P1905080000()
		{
			InitializeComponent();
			Vm.ScrollIntoViewLast += Vm_ScrollIntoViewLast;
			Vm.EditModeFocus += Vm_EditModeFocus;
			SetFocusedElement(txtWorkgroupName);
		}

		/// <summary>
		/// 編輯時，將焦點放到人員查詢條件的第一個欄位
		/// </summary>
		private void Vm_EditModeFocus()
		{
			SetFocusedElement(txtSearchEmpId);
		}

		/// <summary>
		/// 當移動員工時，也做 ListBox 捲軸移到最後的項目
		/// </summary>
		/// <param name="obj"></param>
		private void Vm_ScrollIntoViewLast(ListboxMovingType obj)
		{
			switch (obj)
			{
				case ListboxMovingType.Assign:
				case ListboxMovingType.AssignAll:
					lstAssigned.ScrollIntoView(Vm.AssignedList.LastOrDefault());
					break;

				case ListboxMovingType.UnAssign:
				case ListboxMovingType.UnAssignAll:
					lstUnAssigned.ScrollIntoView(Vm.UnAssignedList.LastOrDefault());
					break;
			}
		}

		///// <summary>
		///// ListBox間的資料移動
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
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
		//	}
		//	// 加入已設定人員
		//	if (((Button)sender).Name.Equals("btnAssign"))
		//	{
		//		foreach (DataServices.F19DataService.F1924 p in lstUnAssigned.SelectedItems)
		//		{
		//			Vm.SelectedEmpList.Add(p);
		//		}
		//	}
		//}

		//Object _SelectedItem; //_SelectedItem is used to avoid the repeated loops
		//DataGridCell _FocusedCell; //_FocusedCell is used to restore focus
		///// <summary>
		///// 為讓User能取消選取 (當資料有異動, 要取消選取的動作時)
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	DataGrid obj = (DataGrid)sender;
		//	if (_SelectedItem == obj.SelectedItem)
		//	{
		//		return;
		//	}
		//	_SelectedItem = obj.SelectedItem;

		//	if (Vm.IsCheckDataModified && e.RemovedItems.Count > 0 && e.RemovedItems[0] != null &&
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
		//	Vm.SelectedWorkgroupItem = (F1963)_SelectedItem;
		//}

	}
}
