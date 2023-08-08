using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
//using Wms3pl.Datas.F19;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.P71.Entities;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P71WcfService;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7101020101.xaml 的互動邏輯
	/// </summary>
	public partial class P7101020101 : Wms3plWindow
	{
		public List<string> EditData;
		public P7101020101(ObservableCollection<P710102MasterData> datas, List<P710101DetailData> OldDetailDataList,List<string> SelectedLoc, ObservableCollection<NameValuePair<string>> areaType, string areaName)
		{
			InitializeComponent();
			Vm.ClosedCancelClick += ClosedCancelClick;
			Vm.ClosedSuccessClick += ClosedSuccessClick;
            Vm.AreaName = areaName;
            Vm.BindData(datas, OldDetailDataList, SelectedLoc, areaType);
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
