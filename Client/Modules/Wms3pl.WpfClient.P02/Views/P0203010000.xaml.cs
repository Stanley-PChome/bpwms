using System;
using System.Collections.Generic;
using System.Data;
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
using Telerik.Windows.Controls;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// P0203010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0203010000 : Wms3plUserControl
	{
		public P0203010000()
		{
			InitializeComponent();
			Vm.OpenSerialLocCheckClick += OpenSerialLocCheckClick;
		}

		#region 開啟序號儲位刷讀視窗
		private void OpenSerialLocCheckClick()
		{
			var win = new P0203010200(Vm.SelectedDcCode, Vm.GupCode, Vm.CustCode, Vm.AllocationDate, Vm.NowAllocationNo, Vm.SelectedF1510Data.TAR_DC_CODE, Vm.SelectedF1510Data.TAR_WAREHOUSE_ID, Vm.SelectedF1510Data.VALID_DATE)
      {
        Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
			};
			win.ShowDialog();
			if (win.DialogResult == true)
			{
				if (Vm.RabQuery1)
					Vm.AllocationNo = Vm.NowAllocationNo;
				else
				{
					Vm.SetAllocationNoTree();
					TreeView.SelectedItem = Vm.AllocationNoList.First().DelvNos.Find(o => o.Id == Vm.NowAllocationNo);
				}
				Vm.SearchCommand.Execute(null);
			}
			Vm.UserOperateMode = OperateMode.Query;
		}

		#endregion

		private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var dep = (e.OriginalSource as DependencyObject);
			while ((dep != null) &&
							!(dep is RadTreeViewItem))
			{
				dep = VisualTreeHelper.GetParent(dep);
			}
			var treeViewItem = dep as RadTreeViewItem;
			if (treeViewItem != null)
			{
				if (!(e.OriginalSource is TextBlock) && !treeViewItem.IsSelected) return;
				if (treeViewItem.IsSelected)
					TreeView.SelectedItem = treeViewItem.Item;
				else
					treeViewItem.IsSelected = true;
			}
		}

		#region Grid 上架儲位 開啟修改上架儲位視窗
		private void BtnLocCode_OnClick(object sender, RoutedEventArgs e)
		{
			var row = (sender as Button).DataContext;
			if (row != null) //取得按鈕按下那一筆資料列資料
			{
				if (Vm.SelectedF1510Data.BUNDLE_SERIALLOC == "1")
				{
					Vm.ShowWarningMessage(Properties.Resources.P0203010000_BundleSerialloc);
					return;
				}
				Vm.UserOperateMode = OperateMode.Add;
				var win = new P0203010100(Vm.SelectedF1510Data,"0,1,3")
				{
					Owner = System.Windows.Window.GetWindow(this),
					WindowStartupLocation = WindowStartupLocation.CenterOwner,
				};
				win.ShowDialog();
				if (win.DialogResult == true)
				{
					if (Vm.RabQuery1)
						Vm.AllocationNo = Vm.NowAllocationNo;
					else
					{
						Vm.SetAllocationNoTree();
						TreeView.SelectedItem = Vm.AllocationNoList.First().DelvNos.Find(o => o.Id == Vm.NowAllocationNo);
					}
					Vm.SearchCommand.Execute(null);
				}
				Vm.UserOperateMode = OperateMode.Query;
			}
		}
		#endregion
	}
}
