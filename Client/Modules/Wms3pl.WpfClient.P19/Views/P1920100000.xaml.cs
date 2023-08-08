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
    /// P1920100000.xaml 的互動邏輯
    /// </summary>
    public partial class P1920100000 : Wms3plUserControl
    {
        public P1920100000()
        {
            InitializeComponent();
            Vm.AddAction += AddCommand_Executed;
            Vm.EditAction += EditCommand_Executed;
            Vm.SearchAction += SearchCommand_Executed;
            SetFocusedElement(txtCAR_KIND_ID);           
        }

        private void dgScrollIntoView()
        {
            if (F194702List.SelectedItem != null)
                F194702List.ScrollIntoView(F194702List.SelectedItem);
        }

        private void AddCommand_Executed()
        {
            F194702List.Columns[0].IsReadOnly = false;
            F194702List.Columns[1].IsReadOnly = false;
            F194702List.Columns[2].IsReadOnly = false;
            F194702List.Columns[3].IsReadOnly = false;
            
            dgScrollIntoView();
        }
        private void EditCommand_Executed()
        {
            F194702List.Columns[0].IsReadOnly = true;
            F194702List.Columns[1].IsReadOnly = false;
            F194702List.Columns[2].IsReadOnly = false;
            F194702List.Columns[3].IsReadOnly = false;
            dgScrollIntoView();
        }

        private void SearchCommand_Executed()
        {
           
            F194702List.Columns[0].IsReadOnly = true;
            F194702List.Columns[1].IsReadOnly = true;
            F194702List.Columns[2].IsReadOnly = true;
            F194702List.Columns[3].IsReadOnly = true;          
            dgScrollIntoView();
        }
    }
}
