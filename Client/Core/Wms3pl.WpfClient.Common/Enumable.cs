using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.Common
{
	public static class EnumableExtensions
	{
		public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
		{
			return new ObservableCollection<T>(col);
		}

		/// <summary>
		/// 相等於 Select(selector).FirstOrDefault();
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="source"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static TResult SelectFirstOrDefault<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
		{
			return source.Select(selector).FirstOrDefault();
		}
	}
}
