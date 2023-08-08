using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Wms3pl.WpfClient.P05.Services
{
    public static class TimeService
    {
			/// <summary>
			/// 時間轉背景色
			/// </summary>
			/// <returns></returns>
			public static Brush ToBackgroundColor(DateTime date, DateTime compareTo)
			{
				var diff = (date - compareTo).TotalMinutes;
				if (diff >= 30) return Brushes.Green;
				if (diff >= 15) return Brushes.Yellow;
				return Brushes.Red;
			}
			/// <summary>
			/// 依照預計完成作業時間及已完成率來判斷要顯示的背景色
			/// </summary>
			/// <param name="pickFinishTime">預計完成作業時間(分鐘)</param>
			/// <param name="pickTime"></param>
			/// <param name="delvDate"></param>
			/// <param name="finishedPercentage">已完成百分比, 例: 0.75</param>
			/// <returns></returns>
			public static Brush ToBackgroundColor(int pickFinishTime, string pickTime, DateTime delvDate, double finishedPercentage)
			{
				DateTime now = DateTime.Now;
				// 批次時間
				DateTime.TryParse(delvDate.ToString("yyyy/MM/dd") + " " + pickTime, out delvDate);
				// 計算現在 - 批次時間
				double passed = 0;
				passed = (now - delvDate).TotalMinutes;
				// 計算應完成百分比
				double shouldFinishPercentage = 0;
				shouldFinishPercentage = (passed / pickFinishTime) * 100;
				if (shouldFinishPercentage > 100) shouldFinishPercentage = 100;
				finishedPercentage = finishedPercentage * 100;
				// 判斷要顯示的顏色
				var diff = finishedPercentage - shouldFinishPercentage;
				if (diff == 0 && shouldFinishPercentage == 100) return Brushes.Green;
				if (diff >= 5) return Brushes.Green;
				if (diff < 5 && diff >= -5) return Brushes.Yellow;
				return Brushes.Red;
			}
    }
}
