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
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.UcLib.Views
{
	/// <summary>
	/// UcVendorButton.xaml 的互動邏輯
	/// </summary>
	public partial class UcVendorButton : UserControl
	{
		public UcVendorButton()
		{
			InitializeComponent();
		}

		public string GupCode
		{
			get
			{
				return (string)GetValue(GupCodeProperty);
			}
			set
			{
				SetValue(GupCodeProperty, value);
			}
		}

		public static readonly DependencyProperty GupCodeProperty =
			DependencyProperty.Register("GupCode", typeof(string),
				typeof(UcVendorButton), new PropertyMetadata(string.Empty));


		public F1908 SelectedF1908
		{
			get
			{
				return (F1908)GetValue(SelectedF1908Property);
			}
			set
			{
				SetValue(SelectedF1908Property, value);
			}
		}

		public static readonly DependencyProperty SelectedF1908Property =
			DependencyProperty.Register("SelectedF1908", typeof(F1908),
				typeof(UcVendorButton), new PropertyMetadata(default(F1908)));


		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GupCode))
				GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;

			var win = new WinSearchVendor();
			win.GupCode = GupCode;
			win.Owner = this.Parent as Window;
			win.ShowDialog();
			if (win.DialogResult.HasValue && win.DialogResult.Value)
			{
				SelectedF1908 = win.SelectedItem;
			}
		}
	}
}
