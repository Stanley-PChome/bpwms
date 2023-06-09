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
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.P19.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1920180000.xaml 的互動邏輯
	/// </summary>
	public partial class P1920180000 : Wms3plUserControl
	{
		public P1920180000()
		{
			InitializeComponent();
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
			SetFocusedElement(DcComboBox);
		}

		private void EditCommand_Executed()
		{
			ResultDataGrid.Columns[0].IsReadOnly = true;
			ResultDataGrid.Columns[1].IsReadOnly = false;
			ResultDataGrid.Columns[1].Visibility = Visibility.Collapsed;
			ResultDataGrid.Columns[2].IsReadOnly = false;
			ResultDataGrid.Columns[2].Visibility = Visibility.Visible;
			ResultDataGrid.Columns[3].IsReadOnly = false;
			ResultDataGrid.Columns[4].IsReadOnly = false;
			ResultDataGrid.Columns[5].IsReadOnly = true;
			ResultDataGrid.Columns[6].IsReadOnly = true;
			ResultDataGrid.Columns[7].IsReadOnly = true;
			ResultDataGrid.Columns[8].IsReadOnly = true;
			ResultDataGrid.Columns[9].IsReadOnly = true;
		}

		private void SearchCommand_Executed()
		{
			ResultDataGrid.Columns[0].IsReadOnly = true;
			ResultDataGrid.Columns[1].IsReadOnly = true;
			ResultDataGrid.Columns[1].Visibility = Visibility.Visible;
			ResultDataGrid.Columns[2].IsReadOnly = true;
			ResultDataGrid.Columns[2].Visibility = Visibility.Collapsed;
			ResultDataGrid.Columns[3].IsReadOnly = true;
			ResultDataGrid.Columns[4].IsReadOnly = true;
			ResultDataGrid.Columns[5].IsReadOnly = true;
			ResultDataGrid.Columns[6].IsReadOnly = true;
			ResultDataGrid.Columns[7].IsReadOnly = true;
			ResultDataGrid.Columns[8].IsReadOnly = true;			
		}

		private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordBox pb = sender as PasswordBox;			
			Vm.SelectedData.SYS_PATH = pb.Password;
		}		
	}
}
