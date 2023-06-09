using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.Services.MembershipProviderService;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClients.SharedViews
{
  /// <summary>
  ///   Interaction logic for ChangePasswordWindow.xaml
  /// </summary>
  public partial class ChangePasswordWindow : Window
  {
    public ChangePasswordWindow(bool needOldPassword)
    {
      InitializeComponent();
      NeedOldPassword = needOldPassword;
      if (!needOldPassword)
      {
        oldRow.Height = new GridLength(0);
        txtOldPassword.Visibility = Visibility.Collapsed;
        lblOld.Visibility = Visibility.Collapsed;
      }
    }

    public bool NeedOldPassword { get; set; }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
      if (NeedOldPassword)
      {
        if (txtOldPassword.Password != Wms3plSession.CurrentUserInfo.Password)
        {
          DialogService.Current.ShowMessage("密碼不正確");
          return;
        }
      }

      if (txtPassword.Password != txtConfirmPassword.Password)
      {
        DialogService.Current.ShowMessage("密碼與確認密碼不同");
        return;
      }
      string account = Wms3plSession.CurrentUserInfo.Account;
      string oldPassword = Wms3plSession.CurrentUserInfo.Password;
      using (var locator = new ServiceLocator())
      {
        var service = locator.GetService<ILoginService>("");
        bool isOk;

				try
				{
				isOk = service.ChangePassword(account, oldPassword + "[Schema@]" + Wms3plSession.Get<GlobalInfo>().SchemaName,
                                        txtPassword.Password);
          Wms3plSession.CurrentUserInfo.Password = txtPassword.Password;
        }
				catch (FaultException<FaultDetail> ex) 
				{
					DialogService.Current.ShowMessage(ex.Message);
					this.Focus();
					//lblError.Text = ex.Message;
					//lblError.Visibility = Visibility.Visible;
					return;
       }

        if (isOk)
        {
          DialogService.Current.ShowMessage("密碼已變更");
          if (ComponentDispatcher.IsThreadModal)
            DialogResult = true;
          else
            Close();
        }
        else
        {
          DialogService.Current.ShowMessage("密碼變更失敗");
        }
      }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      if (ComponentDispatcher.IsThreadModal)
        DialogResult = false;
      else
        Close();
    }

    private void MakeVisible(object sender, RoutedEventArgs e)
    {
      lblError.Visibility = Visibility.Collapsed;
    }

    private void txtOldPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.Return || e.Key == Key.Enter)
        txtPassword.Focus();
    }

    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Return || e.Key == Key.Enter)
        txtConfirmPassword.Focus();
    }

    private void txtConfirmPassword_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Return || e.Key == Key.Enter)
        btnLogin.Focus();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      txtOldPassword.Focus();
    }
  }
}