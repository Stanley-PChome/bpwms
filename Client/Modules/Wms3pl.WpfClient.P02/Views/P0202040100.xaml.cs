using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.UILib;
using System;
using System.Windows.Controls;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;

namespace Wms3pl.WpfClient.P02.Views
{
	public partial class P0202040100 : Wms3plWindow
	{
		public P0202040100(F020201WithF02020101 baseData, string dcCode, string rtNo)
		{
			InitializeComponent();
			Vm.BaseData = baseData;
			Vm.SelectedDc = dcCode;
			Vm.RtNo = rtNo;
			Vm.SearchCommand.Execute(null);

		}
		private void btnExit_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
