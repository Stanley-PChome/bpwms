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
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7101020200.xaml 的互動邏輯
	/// </summary>
	public partial class P7101020200 : Wms3plWindow
	{
		public P7101020200(F1919Data f1919Data, UseModelType useModelType)
		{
			InitializeComponent();
			Vm.EditF1919Data = f1919Data;
			Vm.View = this;
			Vm.ClosedCancelClick += ClosedCancelClick;
			Vm.ClosedSuccessClick += ClosedSuccessClick;
			Vm.DisplayUseModelType = useModelType;
			Vm.BindData();
		}
		private void ClosedCancelClick()
		{
			DialogResult = false;
			Close();
		}

		private void ClosedSuccessClick()
		{
			DialogResult = true;
			Close();
		}

	}
}
