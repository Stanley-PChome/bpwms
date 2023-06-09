using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Wms3pl.WpfClient.UILib.Services
{
  public partial class PrismDialogService //: IDialogService
  {
    InteractionRequest<Notification> _notificationRequest;
    InteractionRequest<Confirmation> _confirmationRequest;

    public PrismDialogService()
    {
      _notificationRequest = new InteractionRequest<Notification>();
      _confirmationRequest = new InteractionRequest<Confirmation>();
    }


    public DialogResponse ShowException(string message, DialogImage image = DialogImage.Error)
    {
      throw new NotImplementedException();
    }

    public DialogResponse ShowMessage(string message, string caption, DialogButton button, DialogImage image)
    {
      var notify = new Notification { Content = message, Title = caption };
      _notificationRequest.Raise(notify, s => { });
      return DialogResponse.OK;

    }

    public void ShowMessage(string message)
    {
      var notify = new Notification {Content = message, Title = "訊息"};
      _notificationRequest.Raise(notify, s => {});
    }

    public DialogResponse ShowMessage(Window window, string message)
    {
      throw new NotImplementedException();
    }
  }
}
