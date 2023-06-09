using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	/// <summary>
	/// enum type 轉換 bool 值
	/// (可用: RadioButton)
	/// </summary>
	public class EnumToBoolConverter<T> : IValueConverter
	{
		private string _typeName;

		#region Init
		protected EnumToBoolConverter() { }
		/// <summary>
		/// EnumToBoolConverter 建構子
		/// </summary>
		/// <param name="typeName">Type的名稱或代號(偵錯,紀錄,除錯用)</param>
		protected EnumToBoolConverter(string typeName)
		{
			_typeName = typeName;
		}
		#endregion

		#region Converter
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is T) || !(parameter is string)) 
				throw new Exception(string.IsNullOrEmpty(_typeName) ? "Value is not Type !" : string.Format("Value is not Type of {0}!",this._typeName));
			var p = parameter as string;
			var type = Enum.Parse(typeof(T), p);
			return type.Equals((T)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool)) 
				throw new Exception("Value is not Boolen !");
			var p = parameter as string;
			return (p == null) ? DependencyProperty.UnsetValue : Enum.Parse(typeof(T), p);
		}
		#endregion

	}
}
