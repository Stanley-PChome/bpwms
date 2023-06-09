using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Prism.Logging;

namespace Wms3pl.WpfClient.Controles
{
  public class EnterpriseLibraryLoggerAdapter : ILoggerFacade
  {
    public void Log(string message, Category category, Priority priority)
    {
      Logger.Write(message, category.ToString(), (int)priority);
    }
  }
}
