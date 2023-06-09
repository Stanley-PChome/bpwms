using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Collections.Generic
{
	public static class ObjectExtension
	{
		public static string[] ToSplit(this string obj, char separator)
		{
			if (string.IsNullOrEmpty(obj)) return new string[]{};
			return obj.Split(separator);
		}
	}
}
