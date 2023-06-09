using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Wms3pl.WpfClient.UILib.Services
{
  public interface IDialogService
  {
    DialogResponse ShowException(String message, DialogImage image = DialogImage.Error);
    DialogResponse ShowMessage(String message, String caption, DialogButton button, DialogImage image);
    void ShowMessage(String message);
    DialogResponse ShowMessage(Window window, String message);

    DialogResponse ShowMessage(Window owner, String message, String caption, DialogButton button,
                               DialogImage image);

    DialogResponse ShowMessage(String message, String caption, DialogButton button, DialogImage image,
                                               MessageBoxResult defaultButton);

    DialogResponse ShowMessage(Window owner, String message, String caption, DialogButton button,
                                               DialogImage image, MessageBoxResult defaultButton);

		DialogResponse ShowMessage(String message, String caption, string yesButtonText, string noButtonText, string cancelButtonText,
																			DialogImage image);

		DialogResponse ShowMessage(Window owner, String message, String caption, string yesButtonText, string noButtonText, string cancelButtonText,
																			DialogImage image);


		FrameworkElement View { get; set; }
  }
}
