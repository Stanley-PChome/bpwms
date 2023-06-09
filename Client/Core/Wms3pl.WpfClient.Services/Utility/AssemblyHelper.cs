using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wms3pl.WpfClient.Services.Utility
{
  class AssemblyHelper
  {
    public string GetAssembliesInfo()
    {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();
      var sb = new StringBuilder();
      foreach (var assembly in assemblies.OrderBy(i => i.GetName().Name))
      {
        var fullName = assembly.GetName().FullName;
        if (fullName.StartsWith("3pl"))
          sb.AppendLine(string.Format("{0}, Built Date={1}", fullName,
            GetBuildDate(assembly)));
      }
      return sb.ToString();
    }

    private DateTime GetBuildDate(Assembly assembly)
    {
      var date = new DateTime(2000, 1, 1);
      var parts = assembly.FullName.Split(',');
      var versionParts = parts[1].Split('.');
      date = date.AddDays(Int32.Parse(versionParts[2]));
      date = date.AddSeconds(Int32.Parse(versionParts[3]) * 2);
      if (System.TimeZoneInfo.Local.IsDaylightSavingTime(date))
        date = date.AddHours(1);
      return date;
    }

  }
}
