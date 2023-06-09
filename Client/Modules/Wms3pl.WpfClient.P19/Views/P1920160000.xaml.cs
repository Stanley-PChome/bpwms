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
    /// P1920160000.xaml 的互動邏輯
    /// </summary>
    public partial class P1920160000 : Wms3plUserControl
    {
        public P1920160000()
        {
            InitializeComponent();
            Vm.AddAction += AddCommand_Executed;
            Vm.EditAction += EditCommand_Executed;
            Vm.SearchAction += SearchCommand_Executed;                     
        }

        private void dgScrollIntoView()
        {
            if (F910001List.SelectedItem != null)
                F910001List.ScrollIntoView(F910001List.SelectedItem);
        }

        private void AddCommand_Executed()
        {
            F910001List.Columns[0].IsReadOnly = false;
            F910001List.Columns[1].IsReadOnly = false;           
            dgScrollIntoView();
        }
        private void EditCommand_Executed()
        {           
            F910001List.Columns[1].IsReadOnly = false;          
            dgScrollIntoView();
        }

        private void SearchCommand_Executed()
        {
            F910001List.Columns[0].IsReadOnly = true;
            F910001List.Columns[1].IsReadOnly = true;
            F910001List.Columns[2].IsReadOnly = true;
            F910001List.Columns[3].IsReadOnly = true;
            F910001List.Columns[4].IsReadOnly = true;
            F910001List.Columns[5].IsReadOnly = true;           
            dgScrollIntoView();
        }
    }
}
