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

namespace Wms3pl.WpfClient.UcLib.Views
{
	/// <summary>
	/// P1901020100.xaml 的互動邏輯
	/// </summary>
	public partial class WinSearchProduct : Wms3plWindow
	{
		public WinSearchProduct(bool isEanCode = false) : base(false)
		{
			InitializeComponent();
			Vm.DoExit += DoExitWin;
			Vm.IsEanCode = isEanCode;
			Vm.SetSearchItemCodeFocus += SetSearchItemCodeFocus;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//if (this.DialogResult == null)
			//{
			//	var dr = DialogService.ShowMessage("確定要離開?", "訊息", UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			//	if (dr == UILib.Services.DialogResponse.No)
			//		e.Cancel = true;
			//}
		}

		private void DoExitWin()
		{
			this.DialogResult = true;
			this.Close();
		}

		public F1903 SelectData { get { return Vm.SelectItem; } }
		public string SearchSerialNo { get { return Vm.SearchSerialNo; } }

		public string GupCode
		{
			get
			{
				return Vm.GupCode;
			}
			set
			{
				Vm.GupCode = value;
			}
		}

		public string CustCode
		{
			get
			{
				return Vm.CustCode;
			}
			set
			{
				Vm.CustCode = value;
			}
		}

		public string SearchEanCode
		{
			get
			{
				return Vm.SearchEanCode;
			}
			set
			{
				Vm.SearchEanCode = value;
			}
		}

		public bool? IsItemCodeChecked
		{
			get
			{
				return Vm.IsEanCodeChecked;
			}
			set
			{
				Vm.IsEanCodeChecked = value;
			}
		}

		public void DoSearchEanCode()
		{
			Vm.DoSearchEanCode();
		}

		private void ExitCommand_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void SetSearchItemCodeFocus()
		{
			DispatcherAction(() =>
			{
				txtSearchItemCode.Focus();
				txtSearchItemCode.SelectionStart = txtSearchItemCode.Text.Length;
			});
		}
	}
}
