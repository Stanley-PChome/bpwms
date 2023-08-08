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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
    /// <summary>
    /// P1920070000.xaml 的互動邏輯
    /// </summary>
    public partial class P1920070000 : Wms3plUserControl
    {
        public P1920070000()
        {
            InitializeComponent();
            Vm.AddAction += AddAction;
            Vm.EditAction += EditCommand_Executed;
            Vm.SearchAction += SearchCommand_Executed;
        }
        private void dgScrollIntoView()
        {
            if (dgList.SelectedItem != null)
                dgList.ScrollIntoView(dgList.SelectedItem);
        }
        private void AddAction()
        {
            dgList.Columns[0].IsReadOnly = false;
            dgList.Columns[1].IsReadOnly = false;
            dgList.Columns[2].IsReadOnly = false;
            cbCOUDIVID.IsEnabled = false;
            dgScrollIntoView();
        }
        private void EditCommand_Executed()
        {
            dgList.Columns[0].IsReadOnly = true;
            dgList.Columns[1].IsReadOnly = false;
            dgList.Columns[2].IsReadOnly = false;
            cbCOUDIVID.IsEnabled = false;
            dgScrollIntoView();
        }

        private void SearchCommand_Executed()
        {
            dgList.Columns[0].IsReadOnly = true;
            dgList.Columns[1].IsReadOnly = true;
            dgList.Columns[2].IsReadOnly = true;
            cbCOUDIVID.IsEnabled = true;
            dgScrollIntoView();
        }

    }
}
