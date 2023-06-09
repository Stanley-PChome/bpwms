using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wms3pl.Common.Extensions
{
  public static class CloneExtensions
  {
    public static void CloneProperties<T1, T2>(this T1 origin, T2 destination)
    {
      if (destination == null) throw new ArgumentNullException("destination", "Destination object must first be instantiated.");

      foreach (var destinationProperty in destination.GetType().GetProperties())
      {
        if (origin != null && destinationProperty.CanWrite)
        {
          origin.GetType().GetProperties().Where(x => x.CanRead && (x.Name == destinationProperty.Name && x.PropertyType == destinationProperty.PropertyType))
            .ToList()
            .ForEach(x => destinationProperty.SetValue(destination, x.GetValue(origin, null), null));
        }
      }
    }
  }
}
