using System.Windows;
using System.Windows.Controls;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClients.SharedViews
{
  /// <summary>
  /// Interaction logic for ForgetPassword.xaml
  /// </summary>
  public partial class ForgetPasswordPage: Page
  {
    public ForgetPasswordPage()
    {
      InitializeComponent();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      if (this.NavigationService.CanGoBack)
      {
        this.NavigationService.GoBack();
      }

    }

    private void btnSendClick(object sender, RoutedEventArgs e)
    {
      using (var locator = new ServiceLocator())
      {
        var loginService = locator.GetService<ILoginService>();
        loginService.ResetPassword(txtUserName.Text.Trim(), txtEmail.Text);
        MessageBox.Show("密碼已經出。回到上一步後，請填入新的密碼。");
        if (this.NavigationService.CanGoBack)
        {
          this.NavigationService.GoBack();
        }
      }
    }
  }
}
