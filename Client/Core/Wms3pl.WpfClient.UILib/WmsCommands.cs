using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wms3pl.WpfClient.UILib
{
	public static class WmsCommands
	{
		static WmsCommands()
		{
		}
		static RoutedUICommand copyFieldValue = new RoutedUICommand("Copy value from Field", "CopyFieldValue",
			typeof(WmsCommands));

		public static RoutedUICommand CopyFieldValue { get { return copyFieldValue; } }

	}
}
