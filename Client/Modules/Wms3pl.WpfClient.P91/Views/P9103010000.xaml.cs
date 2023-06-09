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
using Wms3pl.WpfClient.P91.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9103010000.xaml 的互動邏輯
	/// </summary>
	public partial class P9103010000 : Wms3plUserControl
	{
		public P9103010000()
		{
			{
				InitializeComponent();
				SetFocusedElement(txtSearchBomNo);
				Vm.AddAction += AddCommand_Executed;
				Vm.EditAction += EditCommand_Executed;
				Vm.SearchAction += SearchCommand_Executed;
				Vm.DeleteAction += DeleteCommand_Executed;
			}
		}
	
		private void AddCommand_Executed()
		{
			SetFocusedElement(txtBomNo);
		}
		private void EditCommand_Executed()
		{
			SetFocusedElement(txtBomName);
			ExpQueryCondition.IsExpanded = false;
			//ExpQueryResult.IsExpanded = false;
		}
		private void SearchCommand_Executed()
		{
			SetFocusedElement(txtSearchBomNo);
			if (Vm.dgList !=null && Vm.dgList.Count>0)
			{
				ExpQueryCondition.IsExpanded = false;
				//ExpQueryResult.IsExpanded = false;
			}
			else
			{
				ExpQueryCondition.IsExpanded = true;
				//ExpQueryResult.IsExpanded = true;
			}

		}
		private void DeleteCommand_Executed()
		{

		}

		private void Txt_ITEM_CODE_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{

				if (txtItemCode.Text.Trim() != "")
				{
					//Txt_RETAIL_CODE.Text = "";
					txtItemName.Text = Vm.GetItemName(txtItemCode.Text.Trim());
				}

			}
		}

	}
}