
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710702Service
	{
		private WmsTransaction _wmsTransaction;
		public P710702Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F700101DistrCarData> GetDistrCarDatas(string dcCode, string gupCode, string custCode, DateTime? take_SDate, DateTime? take_EDate, string allId)
		{
			var repF700101 = new F700101Repository(Schemas.CoreSchema);
			return repF700101.GetDistrCarDatas(dcCode, gupCode, custCode, take_SDate, take_EDate, allId);
		}

		public IQueryable<F910201ProcessData> GetProcessDatas(string dcCode, string gupCode, string custCode, DateTime? crt_SDate, DateTime? crt_EDate, string outSourceId)
		{
			var repF910201 = new F910201Repository(Schemas.CoreSchema);
			return repF910201.GetProcessDatas(dcCode, gupCode, custCode, crt_SDate, crt_EDate, outSourceId);
		}

		public IQueryable<F51ComplexReportData> GetF51ComplexReportData(string dcCode, DateTime? calSDate, DateTime? calEDate,
			string gupCode, string custCode, string allId)
		{
			var f5102Repo = new F5102Repository(Schemas.CoreSchema);
			return f5102Repo.GetF51ComplexReportData(dcCode, calSDate, calEDate, gupCode, custCode, allId);
		}
	}
}

