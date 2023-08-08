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
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1901290000.xaml 的互動邏輯
	/// </summary>
	public partial class P1901910000 : Wms3plUserControl
	{
		public P1901910000()
		{
			InitializeComponent();
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(Vm.AddF1946Data.WORKSTATION_GROUP))
			{
				//Vm.AddWorkstationTypeList = Vm.AllWorkstatoinTypeList.Where(x => x.Value.StartsWith(Vm.AddF1946Data.WORKSTATION_GROUP)).ToList();
				//Vm.AddF1946Data.WORKSTATION_TYPE = Vm.AddWorkstationTypeList.First().Value;
				Vm.AddWorkstationTypeList = new List<NameValuePair<string>>();
				
				if(Vm.AddF1946Data.WORKSTATION_GROUP == "B")
				{
					var typeList = new List<string> { "PA1", "PA2", "PACK" };
					Vm.AddWorkstationTypeList = Vm.AllWorkstatoinTypeList.Where(x => typeList.Contains(x.Value)).ToList();
				}
				else
				{
					Vm.AddWorkstationTypeList = Vm.AllWorkstatoinTypeList.Where(x => x.Value.StartsWith(Vm.AddF1946Data.WORKSTATION_GROUP)).ToList();
				}

				Vm.AddF1946Data.WORKSTATION_TYPE = Vm.AddWorkstationTypeList.FirstOrDefault()?.Value;
			}
			
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Vm.AddF1946Data.WORKSTATION_CODE = Vm.AddF1946Data.WORKSTATION_CODE?.ToUpper();
		}
	}
}
