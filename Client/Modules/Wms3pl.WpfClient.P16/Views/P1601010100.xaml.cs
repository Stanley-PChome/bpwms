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
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
  /// <summary>
  /// P1601010100.xaml 的互動邏輯
  /// </summary>
  public partial class P1601010100 : Wms3plWindow
  {
    public P1601010100()
    {
      InitializeComponent();
      Vm.DgItemSource = new List<P1601010100_ViewModel.DgDataClass>()
			  {
				  new P1601010100_ViewModel.DgDataClass()
				  {
					  Str1 = "2014/12/4",
					  Str2 = "11112222",
					  Str3 = Properties.Resources.P1601010100xamlcs_Processed,
					  Str4 = "1",
					  Str5 = "123456789",
					  Str6 = "HTC M8",
					  Str7 = "3",
				    Str8 = "",
            Str9="3",
            Str10 = Properties.Resources.P1601010100xamlcs_None,
            Str11 = "",
            Str12 = "1234567890",
            Str13 = Properties.Resources.Inches5,
            Str14 = "16G",
            Str15 = Properties.Resources.Black,
            Str16 = "",
            Str17 = "",
            Str18 = "3",
            Str19 = "",
			      Bool1 = true
				  }
		    };
    }
  }
}
