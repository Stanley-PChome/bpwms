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
	/// P7101020000.xaml 的互動邏輯
	/// </summary>
	public partial class P7101020000 : Wms3plUserControl
	{
		public P7101020000()
		{
			InitializeComponent();
			Vm.DisplayUseModelType = UseModelType.Headquarters;
			Vm.View = this;
			Vm.OpenAddClick += BtnAddClick;
			Vm.OpenEditClick += BtnEditClick;
		}
		private void BtnAddClick()
		{
			var win = new P7101020100(Vm.DisplayUseModelType, Vm.SelectedDcCode, Vm.SelectedGupCode, Vm.SelectedCustCode, Vm.SelectedwarehouseId, null)
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
			win.ShowDialog();
			if (win.DialogResult == true)
			{
				var id = Vm.SelectedwarehouseId;
				Vm.SetWarehouseList();
				if (!string.IsNullOrEmpty(id))
					Vm.SelectedwarehouseId = id;
				Vm.SearchCommand.Execute(null);

			}
			Vm.UserOperateMode = OperateMode.Query;
		}
		private void BtnEditClick()
		{
			var win = new P7101020100(Vm.DisplayUseModelType, Vm.SelectedF1919Data.DC_CODE, Vm.SelectedF1919Data.GUP_CODE, Vm.SelectedF1919Data.CUST_CODE, Vm.SelectedwarehouseId, Vm.SelectedF1919Data)
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
			};
			win.ShowDialog();
			if (win.DialogResult == true)
				Vm.SearchCommand.Execute(null);
			Vm.UserOperateMode = OperateMode.Query;

		}

		private void dglist_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Vm.EditCommand.Execute(null);
		}
	}
}
