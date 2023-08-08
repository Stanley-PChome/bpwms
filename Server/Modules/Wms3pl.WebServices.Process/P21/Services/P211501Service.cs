
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P19;

namespace Wms3pl.WebServices.Process.P21.Services
{
	public partial class P211501Service
	{
		private WmsTransaction _wmsTransaction;
		public P211501Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<WorkList> GetWorkListDatas(string dcCode, string gupCode, string custCode, string apType, string account)
		{
			//取得今日工作指示查詢
			var F000904Repo = new F000904Repository(Schemas.CoreSchema);
			return F000904Repo.GetWorkListDatas(dcCode, gupCode, custCode, apType, account);
		}
		public IQueryable<WorkList> GetChartListDatas(string dcCode, string gupCode, string custCode, string apType, string account)
		{
			//取得今日出貨工作指示
			var F000904Repo = new F000904Repository(Schemas.CoreSchema);
			return F000904Repo.GetChartListDatas( dcCode,  gupCode,  custCode,  apType,  account);
		}
	}
}

