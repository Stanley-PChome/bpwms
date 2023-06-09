using System;
using System.Web.ClientServices.Providers;
using System.Windows;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClients.SharedViews
{
  /// <summary>
  ///   Interaction logic for LoginWindow.xaml
  /// </summary>
  public partial class LoginWindow : Window, IClientFormsAuthenticationCredentialsProvider
  {
    public LoginWindow()
    {
      InitializeComponent();
    }

    #region IClientFormsAuthenticationCredentialsProvider Members

    public ClientFormsAuthenticationCredentials GetCredentials()
    {
      var userInfo = Wms3plSession.Get<UserInfo>();
      return new ClientFormsAuthenticationCredentials(userInfo.Account, userInfo.Password, true);
    }

    #endregion

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);
      if (IsActive && WindowStartupLocation == WindowStartupLocation.CenterScreen)
      {
        WindowStartupLocation = WindowStartupLocation.Manual;
        Left = SystemParameters.PrimaryScreenWidth/2 - ActualWidth/2;
        Top = SystemParameters.PrimaryScreenHeight/2 - ActualHeight/2;
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      var loginPage = new LoginPage();
      frame.NavigationService.Navigate(loginPage);

    }
  }
}