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
    /// P1920090000.xaml 的互動邏輯
    /// </summary>
    public partial class P1920090000 : Wms3plUserControl
    {
        #region "Constructor"

        public P1920090000()
        {
            InitializeComponent();
            Vm.AddAction += AddAction;
            Vm.EditAction += EditCommand_Executed;
            Vm.SearchAction += SearchCommand_Executed;
        }

        #endregion

        #region "Method"

        private void dgScrollIntoView()
        {
            switch (Vm.OpMode)
            {
                case ViewModel.P1920090000_ViewModel.enumOpMode.F160203:
                    if(this.F160203_List.SelectedItem!=null)
                        this.F160203_List.ScrollIntoView(this.F160203_List.SelectedItem);                   
                    break;
                case ViewModel.P1920090000_ViewModel.enumOpMode.F161203:
                    if (this.F161203_List.SelectedItem != null)
                        this.F161203_List.ScrollIntoView(this.F161203_List.SelectedItem); 
                    break;
                default:
                    break;
            }          
        }
        private void AddAction()
        {
           
            switch (Vm.OpMode)
            {
                case ViewModel.P1920090000_ViewModel.enumOpMode.F160203:                  
                    this.F160203_List.Columns[0].IsReadOnly = false;
                    this.F160203_List.Columns[1].IsReadOnly = false;
                    break;
                case ViewModel.P1920090000_ViewModel.enumOpMode.F161203:
                    this.F161203_List.Columns[0].IsReadOnly = false;
                    this.F161203_List.Columns[1].IsReadOnly = false;
                    break;
                default:
                    break;
            }
            this.cbRETURNTYPE.IsEnabled = false;
            dgScrollIntoView();
        }
        private void EditCommand_Executed()
        {

            switch(Vm.OpMode)
            {
                case ViewModel.P1920090000_ViewModel.enumOpMode.F160203:
                    this.F160203_List.Columns[0].IsReadOnly = true;
                    this.F160203_List.Columns[1].IsReadOnly = false;
                    break;
                case ViewModel.P1920090000_ViewModel.enumOpMode.F161203:
                    this.F161203_List.Columns[0].IsReadOnly = true;
                    this.F161203_List.Columns[1].IsReadOnly = false;
                    break;
                default:
                    break;
            }
            this.cbRETURNTYPE.IsEnabled = false;          
            dgScrollIntoView();
        }

        private void SearchCommand_Executed()
        {
            object obj = null;

            switch (Vm.OpMode)
            {
                case ViewModel.P1920090000_ViewModel.enumOpMode.F160203:
                    obj = this.F160203_List;
                  
                    break;
                case ViewModel.P1920090000_ViewModel.enumOpMode.F161203:
                    obj = this.F161203_List;
                   
                    break;
                default:
                    break;
            }

            if(obj!=null)
            {
                UILib.Controls.ValidationDataGrid DataGrid = (UILib.Controls.ValidationDataGrid)obj;
                DataGrid.Columns[0].IsReadOnly = true;
                DataGrid.Columns[1].IsReadOnly = true;
            }
           
            this.cbRETURNTYPE.IsEnabled = true;      

            dgScrollIntoView();
        }

        #endregion







    }
}
