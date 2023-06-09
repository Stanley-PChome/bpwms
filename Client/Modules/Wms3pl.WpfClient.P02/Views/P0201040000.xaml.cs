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
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// Interaction logic for P0201040000.xaml
	/// </summary>
	public partial class P0201040000 : Wms3plUserControl
	{
		public P0201040000()
		{
			InitializeComponent();
			SetFocusedElement(DcComboBox);
			Vm.OnAddFocus += OnAddFocus;
			Vm.OnEditFocus += OnEditFocus;
		}

		private void OnEditFocus()
		{
			SetFocusedElement(TempAreaCounTextBox);
		}

		private void OnAddFocus()
		{
			SetFocusedElement(BeginDatePickerForAddNew);
		}
	}
}
