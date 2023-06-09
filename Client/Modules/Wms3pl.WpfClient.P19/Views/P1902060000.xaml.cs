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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1902060000.xaml 的互動邏輯
	/// </summary>
	public partial class P1902060000 : Wms3plUserControl
	{
		public P1902060000()
		{
			InitializeComponent();
			SetFocusedElement(txtItemCode);
			Vm.AddAction += AddCommand_Executed;
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
		}

		public P1902060000(string item_code,string cust_code,string gup_code)
		{
			InitializeComponent();
			Vm.AddAction += AddCommand_Executed;
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
			Vm.GUP_CODE = gup_code;
			Vm.CUST_CODE = cust_code;
			Vm.ITEM_CODE = item_code;
			Vm.SearchCommand.Execute(null);
		}

		private void dgScrollIntoView()
		{
			if (dgList.SelectedItem != null)
				dgList.ScrollIntoView(dgList.SelectedItem);
		}
		private void AddCommand_Executed()
		{
			dgList.Columns[0].IsReadOnly = false;
			dgList.Columns[1].IsReadOnly = false;
			dgScrollIntoView();
		}
		private void EditCommand_Executed()
		{
			dgList.Columns[0].IsReadOnly = false;
			dgList.Columns[1].IsReadOnly = false;
			dgScrollIntoView();
		}
		private void SearchCommand_Executed()
		{
			dgList.Columns[0].IsReadOnly = true;
			dgList.Columns[1].IsReadOnly = true;
			dgScrollIntoView();
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            var win = new WinSearchProduct();
            win.GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
            win.CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
            win.Owner = this.Parent as Window;
            win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.Value)
            {
                SetSearchData(win.SelectData);
            }
        }

        private void SetSearchData(F1903 f1903)
        {
            if (f1903 != null)
            {
                Vm.ITEM_CODE = f1903.ITEM_CODE;
            }
            else
            {
                Vm.ITEM_CODE = string.Empty;
            }
        }
    }
}
