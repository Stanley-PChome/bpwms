using System.Collections;

namespace Wms3pl.WpfClient.UILib
{
  public interface IReportService
  {
    void SetDataSource(IEnumerable data);
    void ShowReport(PrintType printType);
  }


}
