
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.WebServices.DataCommon;
using System.Data.Objects;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050103Service
	{
		private WmsTransaction _wmsTransaction;
		public P050103Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 匯總報表
	
		public IQueryable<P050103ReportData> GetSummaryReport(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate, string pickOrdNo, string wmsOrdNo)
		{
			var repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetSummaryReport(dcCode, gupCode, custCode,ordType, delvDate, pickOrdNo, wmsOrdNo);
		}
		#endregion

		#region 批次時段
		public IQueryable<P050103PickTime> GetPickTimes(string dcCode, string gupCode, string custCode,string ordType, DateTime delvDate)
		{
			var repo = new F051201Repository(Schemas.CoreSchema);
			return repo.GetPickTimes(dcCode, gupCode, custCode,ordType, delvDate);
		}

		#endregion

		#region 匯總報表-揀貨單號
		public IQueryable<P050103PickOrdNo> GetPickOrdNos(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate,string pickTime)
		{
			var repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetPickOrderNos(dcCode, gupCode, custCode,ordType, delvDate, pickTime);
		}


		#endregion

		#region 匯總報表-出貨單號
		public IQueryable<P050103WmsOrdNo> GetWmsOrderNos(string dcCode, string gupCode, string custCode, string ordType, DateTime delvDate, string pickTime)
		{
			var repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetWmsOrderNos(dcCode, gupCode, custCode, ordType, delvDate, pickTime);
		}


		#endregion
	}
}
