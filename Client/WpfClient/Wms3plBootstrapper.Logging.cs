using Microsoft.Practices.Prism.Logging;
using Wms3pl.WpfClient.Controles;

namespace Wms3pl.WpfClient
{
  partial class Wms3plBootstrapper
  {
    private readonly EnterpriseLibraryLoggerAdapter _logger = new EnterpriseLibraryLoggerAdapter();

    protected override ILoggerFacade CreateLogger()
    {
      return _logger;
    }
  }
}
