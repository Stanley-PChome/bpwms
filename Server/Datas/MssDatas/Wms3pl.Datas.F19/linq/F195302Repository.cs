using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F195302Repository : RepositoryBase<F195302, Wms3plDbContext, F195302Repository>
	{
		public F195302Repository(string connName, WmsTransaction wmsTransaction = null)
		 : base(connName, wmsTransaction)
		{
		}

		public List<F195302> AddP195302Detail(List<string> scheduleIdList, decimal grpId)
		{
			var scheIdList = scheduleIdList.Where(x => !string.IsNullOrWhiteSpace(x));

			var schedules = _db.F195302s.AsNoTracking().Where(x => x.GRP_ID == grpId).Select(x => x.SCHEDULE_ID);

			return scheIdList.Except(schedules).Select(x => new F195302
			{
				SCHEDULE_ID = x,
				GRP_ID = grpId
			}).ToList();
		}
	}
}
