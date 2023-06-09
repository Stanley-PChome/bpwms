using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P91.Services
{
	public static class P91CommonDataHelper
	{
		/// <summary>
		/// 加工動作
		/// </summary>
		/// <returns></returns>
		public static List<NameValuePair<string>> BomAction(string functionCode)
		{			
			var result = GetBaseTableService.GetF000904List(functionCode, "P9101030000", "BOMACTION");
			return result;
		}

		/// <summary>
		/// 單據狀態
		/// </summary>
		public static List<NameValuePair<string>> ProcessStatusList(string functionCode)
		{
			var result = GetBaseTableService.GetF000904List(functionCode, "F910201", "STATUS");
			result.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = "-1" });
			return result;
		}

		public static List<NameValuePair<string>> ProcessSourceList(string functionCode)
		{
			var result = GetBaseTableService.GetF000904List(functionCode, "P9101010000", "SOURCE");			
			return result;
		}

		/// <summary>
		/// 流程提示 (改變流程文字顏色)
		/// </summary>
		public class ProcessStatus {
			public Brush Step0 { get; set; }
			public Brush Step1 { get; set; }
			public Brush Step2 { get; set; }
			public Brush Step3 { get; set; }
			public Brush Step4 { get; set; }
			public Brush Step5 { get; set; }
			public ProcessStatus(string status)
			{
				SetStatus(status);
			}
			public void SetStatus(string status)
			{
				Step0 = (status == "0" ? Brushes.Red : Brushes.Black);
				Step1 = (status == "1" ? Brushes.Red : Brushes.Black);
				Step2 = (status == "2" ? Brushes.Red : Brushes.Black);
				Step3 = (status == "3" ? Brushes.Red : Brushes.Black);
				Step4 = (status == "4" ? Brushes.Red : Brushes.Black);
				Step5 = (status == "5" ? Brushes.Red : Brushes.Black);
			}
		}


	}
}
