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
	/// P1905010000.xaml 的互動邏輯
	/// </summary>
	public partial class P1905010000 : Wms3plUserControl
	{
		public P1905010000()
		{
			InitializeComponent();

			Vm.View = this;
			SetFocusedElement(txtEmpId);
			Vm.AddAction += Vm_AddAction;
			Vm.EditAction += Vm_EditAction;
		}

		private void Vm_EditAction()
		{
			SetFocusedElement(txtEmpName);
		}

		void Vm_AddAction()
		{
			SetFocusedElement(txtEmpIdResult);			
		}

		private bool _isCheckGup = true;
		private bool _isCheckDc = true;
		private bool _isDcNoSave = false;
		/// <summary>
		/// 為了讓選擇業主時, 能夠取消回到上個選擇的項目
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbGup_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox obj = (ComboBox)sender;
			if (_isCheckGup)
			{
				if (!_isDcNoSave)
				{
					var result = Vm.ConfirmBeforeChangeGUP();
					if (result == UILib.Services.DialogResponse.Cancel)
					{
						_isCheckGup = false;
						obj.SelectedItem = e.RemovedItems[0];
						_isCheckGup = true;
						return;
					}
				}
				else
					_isDcNoSave = false;
				Vm.GupCode = (F1929)obj.SelectedItem;
			}
			
		}

		private void cbDc_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox obj = (ComboBox)sender;
			if (_isCheckDc)
			{
				Vm.OrgDC = Vm.DcCode;
				var result = Vm.ConfirmBeforeChangeDC();
				if (result == UILib.Services.DialogResponse.Cancel )
				{
					_isCheckDc = false;
					obj.SelectedItem = e.RemovedItems[0];
					_isCheckDc = true;
					return;
				}
				else if(result == UILib.Services.DialogResponse.No)
				{
					_isDcNoSave = true;
				}
				Vm.DcCode = (F1901)obj.SelectedItem;
			}
		}
	}
}
