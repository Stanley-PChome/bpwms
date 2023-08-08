using CrystalDecisions.CrystalReports.Engine;

namespace Wms3pl.WpfClient.UILib
{
  public static class ReportClassExtension
  {
    public static void SetText(this ReportClass report, string textFieldName, string text)
    {
      var to = (TextObject)report.ReportDefinition.ReportObjects[textFieldName];
      if (to != null && text != null) to.Text = text;
    }
  }
}
