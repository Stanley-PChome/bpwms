using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// P0203010100.xaml 的互動邏輯
	/// </summary>
	public partial class P0203010100 : Wms3plWindow
	{
		public P0203010100()
		{
			InitializeComponent();
		}
		public P0203010100(F1510Data f1510Data,string status)
		{
			InitializeComponent();
			Vm.ExitClick += ExitClick;
			Vm.ClosedSuccessClick += ClosedSuccessClick;
			Vm.Bind(f1510Data,status);
		}
		private void ExitClick()
		{
			if (DialogService.ShowMessage(Properties.Resources.P0203010100_NotRefresh, Properties.Resources.Message, DialogButton.YesNo, DialogImage.Question) == DialogResponse.Yes)
			{
				DialogResult = false;
				Vm.IsChangedData = false;
				Close();
			}
		}
		private void ClosedSuccessClick()
		{
			if (Vm.IsChangedData)
			{
				if (DialogService.ShowMessage(Properties.Resources.P0203010100_DataIsChange, Properties.Resources.Message, DialogButton.YesNo, DialogImage.Question) == DialogResponse.Yes)
				{
					Vm.DoSave();
					DialogResult = true;
				}
			}
			else if (Vm.IsSaveOk)
				DialogResult = true;
			Close();
		}

		private void P0203010100_OnClosing(object sender, CancelEventArgs e)
		{

		}
		private void LocCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				//儲位檢查
				if (Vm.LocCodeCheck())
				{
					LocQty.Focus();
				}
				else
					e.Handled = true;
			}
		}
		private void LocQty_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				//儲位檢查
				if (Vm.LocCodeCheck() && Vm.CheckLocQty())
					Vm.AddF1510Data();
				else
					e.Handled = true;
			}
		}
	}
}
