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
using Wms3pl.WpfClient.ExDataServices.P20ExDataService;
using Wms3pl.WpfClient.UILib;
using exShare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
namespace Wms3pl.WpfClient.P20.Views
{
	/// <summary>
	/// P2001010100.xaml 的互動邏輯
	/// </summary>
	public partial class P2001010100 : Wms3plWindow
	{
		public P2001010100(F1913Data f1913Data,
							KeyValuePair<int, List<exShare.SerialNoResult>> items,
							IEnumerable<exShare.SerialNoResult> otherSerialNoResults)
		{
			InitializeComponent();
			Vm.F1913Data = f1913Data;
			Vm.OtherSerialNoResults = otherSerialNoResults;
			Vm.CancelClick += CancelClick;
			Vm.SaveClick += SaveClick;
			Vm.DgScrollIntoView += DgScrollIntoView;
			Vm.SerialNoResults = items.Value;
			Vm.DataBind();
			SetFocusedElement(TxtAdjustQty);
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

		private void DgScrollIntoView()
		{
			if (DgList.SelectedItem != null)
				DgList.ScrollIntoView(DgList.SelectedItem);
		}
	}
}
