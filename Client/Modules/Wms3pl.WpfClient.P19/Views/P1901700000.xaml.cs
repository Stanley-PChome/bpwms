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

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1901700000.xaml 的互動邏輯
	/// </summary>
	public partial class P1901700000 : Wms3plUserControl
	{
		public P1901700000()
		{
			InitializeComponent();
		}

        private void TxtEditLABEL_CODE_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Vm.IsChinceString(this.txtEditLABEL_CODE.Text))
                this.txtEditLABEL_CODE.Text = "";
        }
    }
}
