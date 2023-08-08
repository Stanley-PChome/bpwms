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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1905040000.xaml 的互動邏輯
	/// </summary>
	public partial class P1905040000 : Wms3plUserControl
	{
		public P1905040000()
		{
			InitializeComponent();
			Vm.UserOperateModeFocus += UserOperateModeFocus;
			Vm.BringItemIntoView += (x) =>
			{
				base.ControlView(() =>
				{
					var itemFullPath = GetItemPath(x);
					var item = tvTreeView.GetItemByPath(itemFullPath, "||");
					// Focus到所選的項目
					if (item != null) item.BringIntoView();
				});
			};
			SetFocusedElement(txtSearchGroupName);
		}

		/// <summary>
		/// 取得項目的完整路徑
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		private string GetItemPath(P190504FunNode src)
		{
			string result = src.ToString();
			if (src.Parent != null)
				result = GetItemPath(src.Parent) + "||" + result;
			return result;
		}

		public P1905040000(string funCode)
			: this()
		{
			Vm.LastSelectedFunctionId = funCode;
		}


		private void UserOperateModeFocus(OperateMode userOperateMode)
		{
			switch (userOperateMode)
			{
				case OperateMode.Add:
				case OperateMode.Edit:
					SetFocusedElement(txtGroupName);
					break;
				case OperateMode.Query:
					SetFocusedElement(txtSearchGroupName);
					break;
			}
		}

		private void RadTreeView_Checked(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			tvTreeView.Checked -= RadTreeView_Checked;
			DoCheckNodes(e);
			tvTreeView.Checked += RadTreeView_Checked;
		}

		private void RadTreeView_Unchecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			tvTreeView.Unchecked -= RadTreeView_Unchecked;
			DoCheckNodes(e);
			tvTreeView.Unchecked += RadTreeView_Unchecked;
		}

		void DoCheckNodes(Telerik.Windows.RadRoutedEventArgs e)
		{
			var radTreeViewItem = e.OriginalSource as RadTreeViewItem;
			var locNode = radTreeViewItem.Item as P190504FunNode;
			if (locNode != null)
				Vm.DoCheckNodes(locNode);
		}
	}
}
