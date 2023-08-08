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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.P19.Views
{
    /// <summary>
    /// P1901010000.xaml 的互動邏輯
    /// </summary>
    public partial class P1901010000 : Wms3plUserControl
    {
        public P1901010000()
        {
            InitializeComponent();
        }

        private void OnCheckBoxCheckChange(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            var txtLOCCODE = (TextBox)frameworkElement.FindName("txtLOCCODE");
            var chkCUST_NAME = (CheckBox)sender;

            if (!Convert.ToBoolean(chkCUST_NAME.IsChecked))
						{
							var currComBobox = Vm.CustCodeList.Select(a => a).ToList();
							currComBobox.Remove(Vm.CustCodeList.Where(i => i.Name.Contains(chkCUST_NAME.Content.ToString())).FirstOrDefault());
							Vm.CustCodeList = currComBobox;
							txtLOCCODE.Text = string.Empty;
						}
						else
						{
							var currCustData = (from i in Vm.DCServiceList
																	where i.Item.CUST_NAME == chkCUST_NAME.Content.ToString()
																	select new { i.Item.CUST_CODE, i.Item.CUST_NAME }).FirstOrDefault();
							if (currCustData != null)
							{
								var currComBobox = Vm.CustCodeList.Select(a => a).ToList();
								if (currComBobox.Where(o => o.Value == currCustData.CUST_CODE).Any() == false)
								{
									currComBobox.Add(new NameValuePair<string>
									{
										Name = $"{currCustData.CUST_NAME} {currCustData.CUST_CODE}",
										Value = currCustData.CUST_CODE
									});
								}
								Vm.CustCodeList = currComBobox;
							}

							Vm.SelectedCustCode = CustCode;
						}
						
				}

		private void txtLOCCODE_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((!ValidateF1912Exists(sender)) || (!ValidLOCCODEAuthority(sender)))
                Vm.ShowMessage(Messages.InfoNoData);
        }

        private bool ValidateF1912Exists(object sender)
        {
            var frameworkElement = (FrameworkElement)sender;
            var txtLOCCODE = (TextBox)frameworkElement.FindName("txtLOCCODE");

            if (!string.IsNullOrWhiteSpace(txtLOCCODE.Text))
            {
                if (!Vm.ValidateF1912Exists(txtLOCCODE.Text))
                {
                    //Vm.ShowMessage(Messages.InfoNoData);
                    txtLOCCODE.Focus();
                    txtLOCCODE.Text = string.Empty;
                    return false;
                }
            }
            return true;
        }

        private bool ValidLOCCODEAuthority(object sender)
        {
            var frameworkElement = (FrameworkElement)sender;
            var txtLOCCODE = (TextBox)frameworkElement.FindName("txtLOCCODE");
            var txtGUPCODE = (TextBox)frameworkElement.FindName("txtGUPCODE");
            var txtCUSTCODE = (TextBox)frameworkElement.FindName("txtCUSTCODE");
            if (!string.IsNullOrWhiteSpace(txtLOCCODE.Text))
            {
                if (!Vm.ValidLOCCODEAuthority(txtGUPCODE.Text.Trim(), txtCUSTCODE.Text.Trim(), txtLOCCODE.Text.Trim()))
                {
                    txtLOCCODE.Focus();
                    txtLOCCODE.Text = string.Empty;
                    return false;
                }
            }

            return true;
        }

        private void F050006SDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Vm.F050006_Selected != null)
                Vm.F050006DoDeleteComplate();
           
        }
          private void F190102SDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Vm.F190102_Selected != null)
                Vm.F190102DoDeleteComplate();
           
        }
      

    }
}
