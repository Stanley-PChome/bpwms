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

namespace Wms3pl.WpfClient.P19.Views
{
    /// <summary>
    /// P1920050000.xaml 的互動邏輯
    /// </summary>
    public partial class P1920050000 : Wms3plUserControl
    {
        public P1920050000()
        {
            InitializeComponent();
            Vm.AddAction += AddCommand_Executed;
            Vm.EditAction += EditCommand_Executed;
            Vm.SearchAction += SearchCommand_Executed;
            SetFocusedElement(txtDEPIDForQuery);
            //SetFocusedElement(DcComboBox);
        }

        private void dgScrollIntoView()
        {
            if (dgList.SelectedItem != null)
                dgList.ScrollIntoView(dgList.SelectedItem);
        }

        private void AddCommand_Executed()
        {
            dgScrollIntoView();
        }
        private void EditCommand_Executed()
        {
            dgScrollIntoView();
        }

        private void SearchCommand_Executed()
        {
            dgScrollIntoView();
        }
    }
}
