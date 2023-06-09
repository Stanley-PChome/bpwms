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

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7107060000.xaml 的互動邏輯
	/// </summary>
	public partial class P7107060000 : Wms3plUserControl
	{
		public P7107060000()
		{
			InitializeComponent();
			Vm.OnAddProcessMode += AddProcessMode_Executed;
			Vm.OnEditProcessMode += EditProcessMode_Executed;
			Vm.OnQueryProcessMode += QueryProcessMode_Executed;
		}

		private void AddProcessMode_Executed()
		{
			gdProcess.Columns[0].IsReadOnly = false;
			gdProcess.Columns[1].IsReadOnly = false;
			gdProcess.Columns[2].IsReadOnly = false;
			gdProcess.Columns[3].IsReadOnly = false;
			gdProcess.Columns[4].IsReadOnly = false;
			dgProcessScrollIntoView();
		}

		private void EditProcessMode_Executed()
		{
			gdProcess.Columns[0].IsReadOnly = true;
			gdProcess.Columns[1].IsReadOnly = true;
			gdProcess.Columns[2].IsReadOnly = false;
			gdProcess.Columns[3].IsReadOnly = false;
			gdProcess.Columns[4].IsReadOnly = false;
			dgProcessScrollIntoView();
		}

		private void QueryProcessMode_Executed()
		{
			gdProcess.Columns[0].IsReadOnly = true;
			gdProcess.Columns[1].IsReadOnly = true;
			gdProcess.Columns[2].IsReadOnly = true;
			gdProcess.Columns[3].IsReadOnly = true;
			gdProcess.Columns[4].IsReadOnly = true;
			dgProcessScrollIntoView();
		}

		private void dgProcessScrollIntoView()
		{
			if (gdProcess.SelectedItem != null)
			{
				gdProcess.ScrollIntoView(gdProcess.SelectedItem);

			}
		}
	}
}
