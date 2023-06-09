using System;
using System.Windows.Data;
using System.Xml;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class StringToXmlDocumentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string xml = value as string;
      if (xml == null) xml = "<r></r>";
      var doc = new XmlDocument();
      doc.LoadXml(xml);
      return doc;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
