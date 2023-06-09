using System.Collections.Generic;

namespace Wms3pl.WpfClient.Services
{
  public interface INewsService
  {
    IEnumerable<News> GetNews();
  }
}