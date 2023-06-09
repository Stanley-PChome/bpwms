using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700501Repository : RepositoryBase<F700501, Wms3plDbContext, F700501Repository>
	{
		public F700501Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
        public IQueryable<F700501Ex> GetF700501ForMessageData()
        {
            return _db.F700501s
                .Where(x => x.CRT_DATE.Date == DateTime.Today.Date)
                .Where(x => x.SCHEDULE_TYPE == "W")
                .Where(x => x.MESSAGE_ID == null)
                .Select(x => new F700501Ex
                {
                    SCHEDULE_NO = x.SCHEDULE_NO,
                    SCHEDULE_DATE = x.SCHEDULE_DATE,
                    SCHEDULE_TIME = x.SCHEDULE_TIME,
                    SCHEDULE_TYPE = x.SCHEDULE_TYPE,
                    IMPORTANCE = x.IMPORTANCE,
                    SUBJECT = x.SUBJECT,
                    DC_CODE = x.DC_CODE,
                    CRT_STAFF = x.CRT_STAFF,
                    CRT_DATE = x.CRT_DATE,
                    CRT_NAME = x.CRT_NAME,
                    UPD_STAFF = x.UPD_STAFF,
                    UPD_DATE = x.UPD_DATE,
                    UPD_NAME = x.UPD_NAME,
                    FILE_NAME = x.FILE_NAME,
                    STATUS = x.STATUS,
                    CONTENT = x.CONTENT,
                });
        }
    }
}
