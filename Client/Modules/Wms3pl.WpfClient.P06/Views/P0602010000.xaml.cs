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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using System.Windows.Controls.Primitives;

namespace Wms3pl.WpfClient.P06.Views
{
	/// <summary>
	/// P0602010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0602010000 : Wms3plUserControl
	{
		public P0602010000()
		{
			InitializeComponent();
			Vm.AddAction += AddCommand_Executed;
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
			Vm.GetCurrentTab += GetCurrentTab;
			Vm.SetCurrentTab += SetCurrentTab;
			Vm.CollapsedAction += Collapsed_Executed;
			Vm.CancelAction += CancelCommand_Executed;
			Vm.GridReadOnlylAction += GridReadOnlylAction_Executed;
		}

		private void AddCommand_Executed()
		{
			ExpQueryCondition1.IsExpanded = true;
			ExpQueryCondition2.IsExpanded = true;
			ExpQueryCondition1_Allot.IsExpanded = true;
			ExpQueryCondition2_Allot.IsExpanded = true;

			ExpQueryResult1.IsExpanded = true;
			ExpQueryResult2.IsExpanded = true;
			ExpQueryResult1_Allot.IsExpanded = true;
			ExpQueryResult2_Allot.IsExpanded = true;
		}

		private void EditCommand_Executed()
		{
			//btnSearch.IsEnabled = false;
			//btnEdit.IsEnabled = false;
			dgLack.Columns[0].IsReadOnly = false;
			dgLack.Columns[7].IsReadOnly = !Vm.LackQtyIsEnabled;
			dgLack.Columns[8].IsReadOnly = false;
			dgLack.Columns[9].IsReadOnly = false;

			dgLack2.Columns[0].IsReadOnly = false;
			dgLack2.Columns[6].IsReadOnly = false;
			dgLack2.Columns[7].IsReadOnly = false;
			dgLack2.Columns[8].IsReadOnly = false;

			dgLack_Allot.Columns[0].IsReadOnly = false;
			//dgLack_Allot.Columns[5].IsReadOnly = false;
			dgLack_Allot.Columns[6].IsReadOnly = false;
			dgLack_Allot.Columns[7].IsReadOnly = false;

			dgLack2_Allot.Columns[0].IsReadOnly = false;
			dgLack2_Allot.Columns[5].IsReadOnly = false;
			dgLack2_Allot.Columns[6].IsReadOnly = false;
			dgLack2_Allot.Columns[7].IsReadOnly = false;
		}

		private void SearchCommand_Executed()
		{
			//btnSearch.IsEnabled = true;
			//btnEdit.IsEnabled = true;
			ExpQueryCondition1.IsExpanded = false;
			ExpQueryCondition2.IsExpanded = false;
			ExpQueryCondition1_Allot.IsExpanded = false;
			ExpQueryCondition2_Allot.IsExpanded = false;

			ExpQueryResult1.IsExpanded = true;
			ExpQueryResult2.IsExpanded = true;
			ExpQueryResult1_Allot.IsExpanded = true;
			ExpQueryResult2_Allot.IsExpanded = true;
		}

		private void CancelCommand_Executed()
		{
			//btnSearch.IsEnabled = true;
			ExpQueryCondition1.IsExpanded = true;
			ExpQueryCondition2.IsExpanded = true;
			ExpQueryCondition1_Allot.IsExpanded = true;
			ExpQueryCondition2_Allot.IsExpanded = true;
			SetGridReadOnly();
		}

		private void GridReadOnlylAction_Executed()
		{
			SetGridReadOnly();
		}

		private void SetGridReadOnly()
		{
			dgLack.Columns[0].IsReadOnly = true;
			dgLack.Columns[7].IsReadOnly = true;
			dgLack.Columns[8].IsReadOnly = true;
			dgLack.Columns[9].IsReadOnly = true;

			dgLack2.Columns[0].IsReadOnly = true;
			dgLack2.Columns[6].IsReadOnly = true;
			dgLack2.Columns[7].IsReadOnly = true;
			dgLack2.Columns[8].IsReadOnly = true;

			dgLack_Allot.Columns[0].IsReadOnly = true;
			dgLack_Allot.Columns[5].IsReadOnly = true;
			dgLack_Allot.Columns[6].IsReadOnly = true;
			dgLack_Allot.Columns[7].IsReadOnly = true;

			dgLack2_Allot.Columns[0].IsReadOnly = true;
			dgLack2_Allot.Columns[5].IsReadOnly = true;
			dgLack2_Allot.Columns[6].IsReadOnly = true;
			dgLack2_Allot.Columns[7].IsReadOnly = true;
		}

		private void GetCurrentTab()
		{
			if (PickTab.IsSelected)
				Vm.CURRENT_TAB = "PICK";
			else
				Vm.CURRENT_TAB = "ALLOT";
		}

		private void SetCurrentTab()
		{
			if (Vm.cbWorkType==0)
				PickTab.IsSelected = true;
			else
				AllotTab.IsSelected = true;
		}

		private void Collapsed_Executed()
		{
			ExpQueryResult1.IsExpanded = false;
			ExpQueryResult2.IsExpanded = false;
			ExpQueryResult1_Allot.IsExpanded = false;
			ExpQueryResult2_Allot.IsExpanded = false;
		}

		private void ModifyQtyOutOfStock_Click(object sender, RoutedEventArgs e)
		{
			var win = new P0602010100(Vm.LackSelected);
			win.ShowDialog();
			if (win.LackQty == 0 && Vm.LackList.Count == 1)
			{
				Vm.SearchCommand.Execute(null);
			}
			else
			{
				Vm.SetLackList();
			}
		}
	}
}
