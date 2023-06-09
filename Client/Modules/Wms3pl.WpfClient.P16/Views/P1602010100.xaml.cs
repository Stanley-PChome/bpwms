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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
  /// <summary>
  /// P1602010100.xaml 的互動邏輯
  /// </summary>
	public partial class P1602010100 : Wms3plWindow
	{
		public List<F160201ReturnDetail> ReturnData { get; set; }

		public P1602010100()
		{
			InitializeComponent();
		}

		public P1602010100(F160201 f160201Data, string vendorName, SelectionList<F160201ReturnDetail> existF160201DetailList,
			F160201ReturnDetail editDetail = null)
		{
			InitializeComponent();
			Vm.AddNewF160201 = f160201Data;
			Vm.VendorName = vendorName;
			Vm.ExistF160201DetailList = existF160201DetailList;

			if (editDetail == null)
			{
				Vm.IsEditDetail = false;
			}
			else
			{
				Vm.IsEditDetail = true;
				Vm.EditTarget = editDetail;
				Vm.ItemCode = editDetail.ITEM_CODE;
				Vm.ItemName = editDetail.ITEM_NAME;
				Vm.ReturnTotalCount = editDetail.RTN_VNR_QTY.ToString();
				SetModifyMode(true);
			}
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			Window.Close();
			//var dr = DialogService.ShowMessage(Resources.Resources.WarningBeforeCancel, Resources.Resources.Information, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			//if (dr == UILib.Services.DialogResponse.Yes)
			//{
			//	Window.Close();
			//}
			//else
			//{
			//	Window.Activate();
			//}
		}

		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{
			Vm.SaveRuning();
			ReturnData = Vm.ReturnData;
			Window.Close();
		}

		public void SetModifyMode(bool value)
		{
			ItemNameTextBox.IsEnabled = !value;
			ItemCodeTextBox.IsEnabled = !value;
		}
	}
}
