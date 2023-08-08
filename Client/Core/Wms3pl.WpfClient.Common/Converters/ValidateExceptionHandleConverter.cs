using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class ValidateExceptionHandleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var erros = value as System.Collections.ObjectModel.ReadOnlyObservableCollection<System.Windows.Controls.ValidationError>;
			if (erros != null && erros.Any())
			{
				var error = erros.Where(a => a.Exception != null)
								 .Where(a => !(a.Exception is FormatException || a.Exception is OverflowException)).FirstOrDefault();
				if (error != null)
					throw new Exception(string.Format("Validation Exception:{0}", error.Exception.Message), error.Exception);
			}
			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
