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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.P19.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1920190000.xaml 的互動邏輯
	/// </summary>
	public partial class P1920190000 : Wms3plUserControl
	{
		public P1920190000()
		{
			InitializeComponent();
			Vm.AddAction += AddCommand_Executed;
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
		}
		private P1920190000_ViewModel ViewModel { get { return Resources["Vm"] as P1920190000_ViewModel; } }
		private void dgScrollIntoView()
		{
			switch (Vm.SelectedClassType)
			{
				case "A":
					if (DgListA.SelectedItem != null)
						DgListA.ScrollIntoView(DgListA.SelectedItem);
					break;
				case "B":
					if (DgListB.SelectedItem != null)
						DgListB.ScrollIntoView(DgListB.SelectedItem);
					break;
				case "C":
					if (DgListC.SelectedItem != null)
						DgListC.ScrollIntoView(DgListC.SelectedItem);
					break;
			}

		}

		private void AddCommand_Executed()
		{
			
			switch (Vm.SelectedClassType)
			{
				case "A":
					DgListA.Columns[0].IsReadOnly = false;
					DgListA.Columns[1].IsReadOnly = false;
					DgListA.Columns[2].IsReadOnly = false;
					break;
				case "B":
					DgListB.Columns[0].IsReadOnly = false;
					DgListB.Columns[1].IsReadOnly = false;
					DgListB.Columns[2].IsReadOnly = false;
					DgListB.Columns[3].IsReadOnly = false;
					break;
				case "C":
					DgListC.Columns[0].IsReadOnly = false;
					DgListC.Columns[1].IsReadOnly = false;
					DgListC.Columns[2].IsReadOnly = false;
					DgListC.Columns[3].IsReadOnly = false;
					DgListC.Columns[4].IsReadOnly = false;
					break;
				default:
					break;
			}
			dgScrollIntoView();
		}
		private void EditCommand_Executed()
		{
			switch (Vm.SelectedClassType)
			{
				case "A":
					DgListA.Columns[0].IsReadOnly = true;
					DgListA.Columns[1].IsReadOnly = false;
					DgListA.Columns[2].IsReadOnly = false;
					break;
				case "B":
					DgListB.Columns[0].IsReadOnly = true;
					DgListB.Columns[1].IsReadOnly = true;
					DgListB.Columns[2].IsReadOnly = false;
					DgListB.Columns[3].IsReadOnly = false;
					break;
				case "C":
					DgListC.Columns[0].IsReadOnly = true;
					DgListC.Columns[1].IsReadOnly = true;
					DgListC.Columns[2].IsReadOnly = true;
					DgListC.Columns[3].IsReadOnly = false;
					DgListC.Columns[4].IsReadOnly = false;
					break;
			}
			dgScrollIntoView();
		}

		private void SearchCommand_Executed()
		{
			switch (Vm.SelectedClassType)
			{
				case "A":
					foreach (var cols in DgListA.Columns)
						cols.IsReadOnly = true;
					break;
				case "B":
					foreach (var cols in DgListB.Columns)
						cols.IsReadOnly = true;
					break;
				case "C":
					foreach (var cols in DgListC.Columns)
						cols.IsReadOnly = true;
					break;
			}
			dgScrollIntoView();
		}

		private void CbBType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = ((ComboBox)sender).SelectedItem as NameValuePair<string>;
			if(item!=null)
				Vm.SelectedDataC.BCODE = item.Value;
		}
	}
}
