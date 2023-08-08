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
    /// P7109020000.xaml 的互動邏輯
    /// </summary>
    public partial class P7109020000 : Wms3plUserControl
    {
        public P7109020000()
        {
            InitializeComponent();
            Vm.AddMode += AddMode;
			Vm.EditMode += EditMode;
            Vm.QueryMode += QueryMode;
        }

        public void UpdateAllItemSource(string dcCode)
        {
            if (!string.IsNullOrEmpty(dcCode) && Vm.SearchDcCode != dcCode)
            {
                Vm.SearchDcCode = dcCode;
                Vm.DcCodeSelectionChangedCommand.Execute(null);
            }
        }

        public void AddMode()
        {
			dgF194704.Columns[0].IsReadOnly = true;
			dgF194704.Columns[1].IsReadOnly = true;
			dgF194704.Columns[2].IsReadOnly = false;
			dgF194704.Columns[3].IsReadOnly = false;
			dgF194704.Columns[4].IsReadOnly = false;
			dgF194704.Columns[5].IsReadOnly = false;
			dgF194704.Columns[6].IsReadOnly = false;
			dgF194704.Columns[7].IsReadOnly = false;
			dgF194704.Columns[8].IsReadOnly = false;
			dgF194704.Columns[9].IsReadOnly = false;
			dgF194704.Columns[10].IsReadOnly = true;
			dgF194704.Columns[11].IsReadOnly = true;

			dgF194704ScrollIntoView();
        }

		public void EditMode()
		{
			dgF194704.Columns[0].IsReadOnly = true;
			dgF194704.Columns[1].IsReadOnly = true;
			dgF194704.Columns[2].IsReadOnly = true;
			dgF194704.Columns[3].IsReadOnly = false;
			dgF194704.Columns[4].IsReadOnly = false;
			dgF194704.Columns[5].IsReadOnly = false;
			dgF194704.Columns[6].IsReadOnly = false;
			dgF194704.Columns[7].IsReadOnly = false;
			dgF194704.Columns[8].IsReadOnly = false;
			dgF194704.Columns[9].IsReadOnly = false;
			dgF194704.Columns[10].IsReadOnly = true;
			dgF194704.Columns[11].IsReadOnly = true;
			dgF194704ScrollIntoView();
		}

		public void QueryMode()
        {
            foreach (var col in dgF194704.Columns)
            {
                col.IsReadOnly = true;
            }
            dgF194704ScrollIntoView();
        }

        private void dgF194704ScrollIntoView()
        {
            if (dgF194704.SelectedItem != null)
            {
                dgF194704.ScrollIntoView(dgF194704.SelectedItem);
            }
        }

        private void gupItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gupCode = Convert.ToString((e.Source as ComboBox).SelectedValue);
            Vm.SetCustList(Vm.SearchDcCode, gupCode);
        }
    }
}
