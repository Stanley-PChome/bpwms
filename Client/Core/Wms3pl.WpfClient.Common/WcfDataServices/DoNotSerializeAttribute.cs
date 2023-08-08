using System;

namespace Wms3pl.WpfClient.Common.WcfDataServices
{
  [AttributeUsage(AttributeTargets.Property)]
  public class DoNotSerializeAttribute : Attribute
  {
  }
}
