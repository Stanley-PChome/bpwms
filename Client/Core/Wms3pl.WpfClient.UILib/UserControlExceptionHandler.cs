using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;

namespace Wms3pl.WpfClient.UILib
{
  [ConfigurationElementType(typeof (CustomHandlerData))]
  public class UserControlExceptionHandler : IExceptionHandler
  {
    public UserControlExceptionHandler(NameValueCollection ignore)
    {
    }

    #region IExceptionHandler Members

    public Exception HandleException(Exception exception, Guid handlingInstanceId)
    {
      var uiException = exception as Wms3plUIException;
      if (uiException != null)
        uiException.Container.ShowExceptionMessage(uiException);
      return exception;
    }

    #endregion
  }
}