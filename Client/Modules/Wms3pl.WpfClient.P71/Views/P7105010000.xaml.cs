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
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
  /// <summary>
  /// P7105010000.xaml 的互動邏輯
  /// </summary>
  public partial class P7105010000 : Wms3plUserControl
  {
    public P7105010000()
    {
			InitializeComponent();

			Vm.AddAction += AddCommand_Executed;
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
			Vm.CancelAction += CancelCommand_Executed;
			Vm.CollapsedQryResultAction += CollapsedQryResult_Excuted;
		}

		private void CollapsedQryResult_Excuted()
		{
			ExpQueryCondition.IsExpanded = false;
			ExpQueryResult.IsExpanded = false;
		}
		private void AddCommand_Executed()
		{
		}
		private void EditCommand_Executed()
		{
		}
		private void SearchCommand_Executed()
		{
            ExpQueryCondition.IsExpanded = true;
			ExpQueryResult.IsExpanded = true;
		}
		private void CancelCommand_Executed()
		{
			ExpQueryCondition.IsExpanded = true;
			ExpQueryResult.IsExpanded = true;
		}

        private void LOC_TYPE_ID_CHANGED(object sender, SelectionChangedEventArgs e)
        {
            if (Vm.UserOperateMode == OperateMode.Add || Vm.UserOperateMode == OperateMode.Edit)
            {
                ComboBox cb = e.Source as ComboBox;
                if (cb.SelectedValue != null)
                {
                    Vm.Get_Loc_Info(cb.SelectedValue.ToString());	
                }

            }
        }



  }
}
