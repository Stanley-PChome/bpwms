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
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1902020000.xaml 的互動邏輯
	/// </summary>
	public partial class P1902020000 : Wms3plUserControl
	{
		private string strMode;
		public P1902020000()
		{
			InitializeComponent();
			Vm.AddAction += NewCommand_Executed;
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
		}
		private void dgScrollIntoView()
		{
			if (dgList.SelectedItem != null)
				dgList.ScrollIntoView(dgList.SelectedItem);
		}

		private void NewCommand_Executed()
		{
			strMode = "N";
			dgList.Items.MoveCurrentToLast();
			dgList.Columns[0].IsReadOnly = false;
			dgList.Columns[1].IsReadOnly = false;
			dgScrollIntoView();
		}

		private void EditCommand_Executed()
		{
			strMode = "E";
			dgList.Columns[0].IsReadOnly = true;
			dgList.Columns[1].IsReadOnly = false;
			dgScrollIntoView();
		}

		private void SearchCommand_Executed()
		{
			strMode = "S";
			dgList.Columns[0].IsReadOnly = true;
			dgList.Columns[1].IsReadOnly = true;
			dgScrollIntoView();
		}
	}
}
