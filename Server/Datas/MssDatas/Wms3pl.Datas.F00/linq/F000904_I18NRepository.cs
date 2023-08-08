using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
	public partial class F000904_I18NRepository : RepositoryBase<F000904_I18N, Wms3plDbContext, F000904_I18NRepository>
	{
		public F000904_I18NRepository(string connName, WmsTransaction wmsTransaction = null)
			   : base(connName, wmsTransaction)
		{
		}

        public IQueryable<F000904_I18N> GetDatas(string topic, string subTopic)
        {
            return _db.F000904_I18N.Where(x => x.TOPIC == topic &&
                                               x.SUBTOPIC == subTopic);
        }
    }
}
