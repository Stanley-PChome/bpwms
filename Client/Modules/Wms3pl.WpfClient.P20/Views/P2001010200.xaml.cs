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
using Wms3pl.WpfClient.DataServices.F20DataService;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using exShare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;

namespace Wms3pl.WpfClient.P20.Views
{
	/// <summary>
	/// P2001010200.xaml 的互動邏輯
	/// </summary>
	public partial class P2001010200 : Wms3plWindow
	{
		public P2001010200(string dcCode, string dcName, IEnumerable<exShare.SerialNoResult> otherSerialNoResults)
		{
			InitializeComponent();
			Vm.DcCode = dcCode;
			Vm.DcName = dcName;
			Vm.OtherSerialNoResults = otherSerialNoResults;
			Vm.CancelClick += CancelClick;
			Vm.SaveClick += SaveClick;
			Vm.DgScrollIntoView += DgScrollIntoView;
			Vm.SetErrorFocus += SetErrorFocus;
			Vm.GetItemData += GetItemData;
			Vm.ClearItemData += ClearItemData;
			SetFocusedElement(TxtLocCode);
		}

		private void CancelClick()
		{
			this.DialogResult = false;
			Close();
		}

		private void SaveClick()
		{
			this.DialogResult = true;
			Close();
		}

		private void ScanSerialNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.AddSerialNo();

		}

		private void Clear_OnClick(object sender, RoutedEventArgs e)
		{
			var dep = (DependencyObject)e.OriginalSource;
			while ((dep != null) &&
					!(dep is DataGridRow))
			{
				dep = VisualTreeHelper.GetParent(dep);
			}
			if (dep == null) return;

			DataGridRow row = dep as DataGridRow;
			var serialNoResult = row.Item as exShare.SerialNoResult;
			Vm.DeleteSerialNo(serialNoResult);
		}

		private void VnrCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.GetVnrName();
			if (Vm.F1913Data.VNR_NAME == "")
			{
				DialogService.ShowMessage(Properties.Resources.P2001010200xamlcs_VNR_CODE_NotFound);
				this.Focus();
			}
		}

		private void DgScrollIntoView()
		{
			if (DgList.SelectedItem != null)
				DgList.ScrollIntoView(DgList.SelectedItem);
		}

		private void SetErrorFocus(string errType)
		{
			switch (errType)
			{
				case "1":
					TxtLocCode.Focus();
					break;
				case "2":
					TxtVnrCode.Focus();
					break;
				default:
					break;
			}
		}

		public void GetItemData()
		{
			Vm.F1913Data.ITEM_CODE = ucSearchProduct.ItemCode;
			Vm.F1913Data.ITEM_NAME = ucSearchProduct.ItemName;
			Vm.F1913Data.ITEM_SIZE = ucSearchProduct.ItemSize;
			Vm.F1913Data.ITEM_SPEC = ucSearchProduct.ItemSpec;
			Vm.F1913Data.ITEM_COLOR = ucSearchProduct.ItemColor;
			Vm.HasFindSearchItem = ucSearchProduct.HasItem;
		}

		public void ClearItemData()
		{
			ucSearchProduct.ItemCode = null;
			ucSearchProduct.ItemName = null;
			ucSearchProduct.ItemSize = null;
			ucSearchProduct.ItemSpec = null;
			ucSearchProduct.ItemColor = null;
			ucSearchProduct.HasItem = false;
			


		}
	}
}
