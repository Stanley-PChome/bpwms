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
using Telerik.Windows.Controls;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1905070000.xaml 的互動邏輯
	/// </summary>
	public partial class P1905070000 : Wms3plUserControl
	{
		public P1905070000()
		{
			InitializeComponent();
			Vm.UserOperateModeFocus += UserOperateModeFocus;
			SetFocusedElement(txtWorkgroupName);
		}

		private void UserOperateModeFocus(OperateMode obj)
		{
			switch (obj)
			{
				case OperateMode.Add:
				case OperateMode.Edit:
					SetFocusedElement(txtWorkName);
					break;
			}
		}

		private void RadTreeView_Checked(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			nonAllowedTreeView.Checked -= RadTreeView_Checked;
			allowedTreeView.Checked -=RadTreeView_Checked;
			DoCheckNodes(e);
			nonAllowedTreeView.Checked += RadTreeView_Checked;
			allowedTreeView.Checked += RadTreeView_Checked;
		}

		private void RadTreeView_Unchecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			nonAllowedTreeView.Unchecked -= RadTreeView_Unchecked;
			allowedTreeView.Unchecked -= RadTreeView_Unchecked;
			DoCheckNodes(e);
			nonAllowedTreeView.Unchecked += RadTreeView_Unchecked;
			allowedTreeView.Unchecked += RadTreeView_Unchecked;
		}

		void DoCheckNodes(Telerik.Windows.RadRoutedEventArgs e)
		{
			var radTreeViewItem = e.OriginalSource as RadTreeViewItem;
			var locNode = radTreeViewItem.Item as P190507LocNode;
			if (locNode != null)
				Vm.DoCheckNodes(locNode);
		}
	}
}
