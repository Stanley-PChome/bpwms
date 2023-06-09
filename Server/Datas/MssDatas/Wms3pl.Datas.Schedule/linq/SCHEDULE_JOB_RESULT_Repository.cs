using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.Schedule
{
	public partial class SCHEDULE_JOB_RESULTRepository : RepositoryBase<SCHEDULE_JOB_RESULT, Wms3plDbContext, SCHEDULE_JOB_RESULTRepository>
	{
        public SCHEDULE_JOB_RESULTRepository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
        {

        }
    }
}
