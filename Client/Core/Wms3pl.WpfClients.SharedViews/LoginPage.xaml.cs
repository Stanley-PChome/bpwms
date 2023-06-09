using System;
using System.Web.ClientServices.Providers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClients.SharedViews.ViewModel;

namespace Wms3pl.WpfClients.SharedViews
{
  /// <summary>
  ///   Interaction logic for LoginWindow.xaml
  /// </summary>
  public partial class LoginPage : Page, IClientFormsAuthenticationCredentialsProvider
  {
    public LoginPage()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(LoginPage_Loaded);

#if DEBUG
      PasswordTextBox.Password = "a1234567";
#else
			PasswordTextBox.Password = string.Empty;
#endif
    }

    void LoginPage_Loaded(object sender, RoutedEventArgs e)
    {
      var vm = this.DataContext as LoginPageViewModel;
			vm.IsShowMessage = Visibility.Hidden;
		vm.ShowLang = vm.ShowLeng() ? Visibility.Visible : Visibility.Hidden;
	    vm.LoginSuccessful = delegate
	    {
		    Action action = () =>
		    {
			    var parentWindow = Window.GetWindow(this);
			    parentWindow.DialogResult = true;
			    parentWindow.Close();
		    };

		    if (this.CheckAccess())
			    action();
		    else
			    this.Dispatcher.Invoke(DispatcherPriority.Normal, action);
	    };
    }
    #region IClientFormsAuthenticationCredentialsProvider Members

    public ClientFormsAuthenticationCredentials GetCredentials()
    {
      var userInfo = Wms3plSession.Get<UserInfo>();
      return new ClientFormsAuthenticationCredentials(userInfo.Account, userInfo.Password, true);
    }

    #endregion

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      var parentWindow = Window.GetWindow(this);
      parentWindow.DialogResult = false;
      parentWindow.Close();
    }

    private void PasswordTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.Return || e.Key == Key.Enter)
        btnLogin.Focus();
    }


		private void ExceptionMessage_Click(object sender, RoutedEventArgs e)
		{
			var vm = this.DataContext as LoginPageViewModel;
			MessageBox.Show(vm.ExceptionMessage,"Exception Message");
		}
	}
}