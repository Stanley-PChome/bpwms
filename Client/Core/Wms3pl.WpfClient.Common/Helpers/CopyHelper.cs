using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common.Helpers
{
	public static class CopyHelper
	{
		public static T DeepCopy<T>(T obj)
		{
			if (obj == null)
				throw new ArgumentNullException();
			return (T)Copy(obj);
		}

		private static object Copy(object obj)
		{
			if (obj == null)
				return null;
			var type = obj.GetType();
			if (type.IsValueType || type == typeof(string))
			{
				return obj;
			}
			else if (type.IsArray)
			{
				var elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
				var array = obj as Array;
				var copied = Array.CreateInstance(elementType, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					copied.SetValue(Copy(array.GetValue(i)), i);
				}
				return Convert.ChangeType(copied, obj.GetType());
			}
			else if (type.IsClass)
			{
				if (typeof(MulticastDelegate).IsAssignableFrom(type))
				{
					return null;
				}
				var toret = Activator.CreateInstance(obj.GetType());
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (FieldInfo field in fields)
				{
					var fieldValue = field.GetValue(obj);
					if (fieldValue == null)
						continue;
					field.SetValue(toret, Copy(fieldValue));
				}
				return toret;
			}
			else
				throw new ArgumentException("Unknown Type");
		}
	}

}
