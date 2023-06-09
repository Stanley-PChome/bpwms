using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wms3pl.Common.Extensions
{
  public class TypeUtility
  {
    public static bool IsNullableType(Type type)
    {
      if (!type.IsValueType) return true; // ref-type 
      if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T> 
      return false; // value-type 
    }

		public static bool TryGetUnderlyingType(Type type, out Type underlyingType)
		{
			if (type.IsValueType)
			{
				underlyingType = Nullable.GetUnderlyingType(type);
				return underlyingType != null;
			}

			underlyingType = null;
			return false;
		}

		public static void SetNewValue<T>(T tempMyClass, object propertyValue, PropertyInfo pi)
    {
			var targetType = pi.PropertyType;

			Type underlyingType;
			if (TryGetUnderlyingType(pi.PropertyType, out underlyingType))
				targetType = underlyingType;

			object value = Convert.ChangeType(propertyValue, targetType);
			pi.SetValue(tempMyClass, value, null);
		}
  }
}
