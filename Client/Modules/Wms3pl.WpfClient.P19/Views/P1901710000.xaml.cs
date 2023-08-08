using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
	public partial class P1901710000 : Wms3plUserControl
	{
    private string OriTextBoxValue;
		public P1901710000()
		{
			InitializeComponent();
		}

    private void NumTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      //Regex re = new Regex("^([0-9]{1,}[.][0-9]{0,2})$");
      //var txt = ((TextBox)sender).Text + e.Text;
      //e.Handled = !re.IsMatch(txt);
    }

    private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (!(sender is TextBox))
        return;
      var tmpTextbox = sender as TextBox;
      OriTextBoxValue = tmpTextbox.Text;
    }

    private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (!(sender is TextBox))
        return;
      var tmpTextbox = sender as TextBox;
      Regex re = new Regex("^([0-9]{1,}[.]{0,1}[0-9]{0,2})$");
      if (!re.IsMatch(tmpTextbox.Text))
      {
        tmpTextbox.Text = OriTextBoxValue;
      }

    }

    private void TextBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      Console.Write("hi");

    }

    private void TextBox_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      Console.Write("hi");
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      Console.Write("hi");
    }
  }
}