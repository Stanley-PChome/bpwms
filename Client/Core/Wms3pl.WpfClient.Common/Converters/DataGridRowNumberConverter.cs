using System;
using System.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class DataGridRowNumberConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//get the grid and the item
			Object item = values[0];
			DataGrid grid = values[1] as DataGrid;

			int index = grid.Items.IndexOf(item) + 1;

			return index.ToString();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
