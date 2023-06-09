using System;

namespace Wms3pl.WebServices.DataCommon
{
	public interface IAuditInfo
	{
		string CRT_STAFF { get; set; }
		string CRT_NAME { get; set; }
		DateTime CRT_DATE { get; set; }
		string UPD_STAFF { get; set; }
		string UPD_NAME { get; set; }
		DateTime? UPD_DATE { get; set; }
	}
}
