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
using Microsoft.Win32;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.P02;
using Wms3pl.WpfClient.P15.ViewModel;
using Wms3pl.WpfClient.UILib;


namespace Wms3pl.WpfClient.P15.Views
{
	/// <summary>
	/// P1502010100.xaml 的互動邏輯
	/// </summary>
	public partial class P1502010100 : Wms3plWindow
	{
		public P1502010100(F151001 sourceData)
		{
			InitializeComponent();
			Vm.OnShowP0203010200 += ShowP0203010200;
			if (sourceData == null)
				Vm.SourceData = new F151001();
			else
				Vm.SourceData = sourceData;
			Vm.TxtALLOCATION_NO = sourceData.ALLOCATION_NO;
			Vm.TxtAllocation_Date = sourceData.ALLOCATION_DATE.Date;
			Vm.SearchCommand.Execute(null);
		}

		private void CancelCommand_Click(object sender, RoutedEventArgs e)
		{
			var dr = DialogService.ShowMessage(Properties.Resources.P1502010100xamlcs_CancelEdit, Properties.Resources.P1502010000xamlcs_Information, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.Yes)
				Window.Close();
		}

		private void SaveCommand_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.SaveRuning())
				Window.Close();
			else
				return;
		}

		private bool? ShowP0203010200()
		{
			var selItem = Vm.SelectedDgItem.Item;
			var subwin = new P02.Views.P0203010200(selItem.DC_CODE, selItem.GUP_CODE, selItem.CUST_CODE, selItem.ALLOCATION_DATE, selItem.ALLOCATION_NO, selItem.TAR_DC_CODE, selItem.TAR_WAREHOUSE_ID, selItem.VALID_DATE)
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
			};
			return subwin.ShowDialog();
		}

		private void SugLocCode_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.SelectedDgItem != null)
			{
				var f1510Data = Vm.GetF1510Data(Vm.SelectedDgItem.Item);
				if (f1510Data == null)
					return;
				if (f1510Data.BUNDLE_SERIALLOC == "1")
					DialogService.ShowMessage(Properties.Resources.P1502010100xamlcs_CannotModifyLocCode);
				else
				{
					Vm.UserOperateMode = OperateMode.Add;
					var subwin2 = new P02.Views.P0203010100(f1510Data, "1,3")
					{
						Owner = System.Windows.Window.GetWindow(this),
						WindowStartupLocation = WindowStartupLocation.CenterOwner,
					};
					subwin2.ShowDialog();
					if (subwin2.DialogResult == true)
					{
						Vm.SearchCommand.Execute(null);
					}
					Vm.UserOperateMode = OperateMode.Query;
				}
			}
		}

		private void ImportSerial_OnClick(object sender, RoutedEventArgs e)
		{
			var subwin = new P1502010300(Vm.SelectedDgItem.Item.DC_CODE, Vm.SelectedDgItem.Item.ITEM_CODE, "A1")
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Vm =
				{
					WmsNo = Vm.SelectedDgItem.Item.ALLOCATION_NO,
					VnrCode = Vm.SelectedDgItem.Item.VnrCode
				},
			};
			subwin.ShowDialog();
		}
	}
}
