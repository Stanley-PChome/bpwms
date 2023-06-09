using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Wms3pl.WpfClient.Common
{
    public static class InfoCarStatus
    {
		public static Dictionary<string, object> InFifteen = new Dictionary<string, object>()
		{
			{"Text", "小於15分"}, {"Color", Brushes.Red}
		};
		public static Dictionary<string, object> InThirty = new Dictionary<string, object>()
		{
			{"Text", "小於30分"}, {"Color", Brushes.Yellow}
		};
		public static Dictionary<string, object> OverThirty = new Dictionary<string, object>()
		{
			{"Text", "大於30分"}, {"Color", Brushes.LawnGreen}
		};
    }
}
