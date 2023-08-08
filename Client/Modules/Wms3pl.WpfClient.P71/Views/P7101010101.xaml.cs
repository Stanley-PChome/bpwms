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
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.P71.Entities;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7101010101.xaml 的互動邏輯
	/// </summary>
	public partial class P7101010101 : Wms3plWindow
	{
		public List<P710101MasterData> EditData;
		public P7101010101(ObservableCollection<P710101MasterData> data)
		{
			InitializeComponent();
			Vm.ClosedCancelClick += ClosedCancelClick;
			Vm.ClosedSuccessClick += ClosedSuccessClick;
			Vm.BindData(data);
		}

		private void ClosedCancelClick()
		{
			DialogResult = false;
			Close();
		}

		private void ClosedSuccessClick()
		{
			DialogResult = true;
			EditData = Vm.TempData;
			Close();
		}
	}
}
