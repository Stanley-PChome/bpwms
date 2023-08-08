using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P15.Views
{
    /// <summary>
    /// P1504010000.xaml 的互動邏輯
    /// </summary>
    public partial class P1504010000 : Wms3plUserControl
	{
		public P1504010000()
		{
			InitializeComponent();
			Vm.AddAction += AddCommand_Executed;
			Vm.EditAction += EditCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
			Vm.SetCurrentTab += SetCurrentTab;
			Vm.CollapsedAction += Collapsed_Executed;
			Vm.CancelAction += CancelCommand_Executed;
			Vm.GridReadOnlylAction += GridReadOnlylAction_Executed;
		}

		private void AddCommand_Executed()
		{
			ExpQueryCondition1_Allot.IsExpanded = true;
			ExpQueryCondition2_Allot.IsExpanded = true;
			ExpQueryResult1_Allot.IsExpanded = true;
			ExpQueryResult2_Allot.IsExpanded = true;
		}

		private void EditCommand_Executed()
		{
			dgLack_Allot.Columns[0].IsReadOnly = false;
			//dgLack_Allot.Columns[5].IsReadOnly = false;
			dgLack_Allot.Columns[6].IsReadOnly = false;
			dgLack_Allot.Columns[7].IsReadOnly = false;
            dgLack_Allot.Columns[8].IsReadOnly = false;

            dgLack2_Allot.Columns[0].IsReadOnly = false;
			dgLack2_Allot.Columns[5].IsReadOnly = false;
			dgLack2_Allot.Columns[6].IsReadOnly = false;
			dgLack2_Allot.Columns[7].IsReadOnly = false;
		}

		private void SearchCommand_Executed()
		{
			//btnSearch.IsEnabled = true;
			//btnEdit.IsEnabled = true;
			ExpQueryCondition1_Allot.IsExpanded = false;
			ExpQueryCondition2_Allot.IsExpanded = false;
            
			ExpQueryResult1_Allot.IsExpanded = true;
			ExpQueryResult2_Allot.IsExpanded = true;
		}

		private void CancelCommand_Executed()
		{
			//btnSearch.IsEnabled = true;
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
			dgLack_Allot.Columns[0].IsReadOnly = true;
			dgLack_Allot.Columns[5].IsReadOnly = true;
			dgLack_Allot.Columns[6].IsReadOnly = true;
			dgLack_Allot.Columns[7].IsReadOnly = true;
            dgLack_Allot.Columns[8].IsReadOnly = true;

            dgLack2_Allot.Columns[0].IsReadOnly = true;
			dgLack2_Allot.Columns[5].IsReadOnly = true;
			dgLack2_Allot.Columns[6].IsReadOnly = true;
			dgLack2_Allot.Columns[7].IsReadOnly = true;
		}

		private void SetCurrentTab()
		{
			AllotTab.IsSelected = true;
		}

		private void Collapsed_Executed()
		{
			ExpQueryResult1_Allot.IsExpanded = false;
			ExpQueryResult2_Allot.IsExpanded = false;
		}


	}
}
