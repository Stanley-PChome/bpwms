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
    /// P1920170000.xaml 的互動邏輯
    /// </summary>
    public partial class P1920170000 : Wms3plUserControl
    {
        public P1920170000()
        {
            InitializeComponent();
            Vm.AddAction += AddCommand_Executed;
            Vm.EditAction += EditCommand_Executed;
            Vm.SearchAction += SearchCommand_Executed;
            SetFocusedElement(DcComboBox);
        }

        private void dgScrollIntoView()
        {
            if (this.F0005Grid.SelectedItem != null)
                this.F0005Grid.ScrollIntoView(this.F0005Grid.SelectedItem);
        }

        private void AddCommand_Executed()
        {
            this.F0005Grid.Columns[0].IsReadOnly = false;
            this.F0005Grid.Columns[1].IsReadOnly = false;
            this.F0005Grid.Columns[2].IsReadOnly = false;          
            dgScrollIntoView();
        }
        private void EditCommand_Executed()
        {
            this.F0005Grid.Columns[0].IsReadOnly = true;
            this.F0005Grid.Columns[1].IsReadOnly = false;
            this.F0005Grid.Columns[2].IsReadOnly = false;           
            dgScrollIntoView();
        }

        private void SearchCommand_Executed()
        {
            this.F0005Grid.Columns[0].IsReadOnly = true;
            this.F0005Grid.Columns[1].IsReadOnly = true;
            this.F0005Grid.Columns[2].IsReadOnly = true;
            this.F0005Grid.Columns[3].IsReadOnly = true;
            this.F0005Grid.Columns[4].IsReadOnly = true;
            this.F0005Grid.Columns[5].IsReadOnly = true;           
            dgScrollIntoView();
        }
    }
}
