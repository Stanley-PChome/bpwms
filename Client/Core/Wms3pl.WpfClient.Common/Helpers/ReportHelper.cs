using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common.Helpers
{
	public class ReportHelper
	{
		public static ReportClass CreateAndLoadReport<T>()
		{
			var info = Wms3plSession.Get<GlobalInfo>();
			var lang = string.Empty;
			if (info != null && !string.IsNullOrEmpty(info.Lang) && info.Lang.ToUpper() != "ZH-TW")
				lang = info.Lang;

			var type = typeof(T);
			var reportFullTypeName = $"{type.FullName}{lang.Replace("-", "_")},{type.Assembly.FullName}";
			var reportFileName = $"{type.Name}.rpt";
			if (!string.IsNullOrEmpty(lang))
				reportFileName = $"{type.Name}{lang.Replace("-", "_")}.rpt";

			var report = Activator.CreateInstance(Type.GetType(reportFullTypeName)) as ReportClass;
			report.Load(reportFileName);

			return report;
		}
	}
}
