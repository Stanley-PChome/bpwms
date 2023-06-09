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
	public partial class F700701Repository : RepositoryBase<F700701, Wms3plDbContext, F700701Repository>
	{
		public F700701Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
        public IQueryable<F700701QueryData> GetF700701QueryData(string dcCode, DateTime? importSDate, DateTime? importEDate)
        {
            var q =_db.F700701s
                .Where(x => x.DC_CODE == dcCode);

            //投入日期-起
            if (importSDate.HasValue)
            {
                q = q.Where(x => x.IMPORT_DATE >= importSDate.Value);
            }

            //投入日期-迄
            if (importEDate.HasValue)
            {
                q = q.Where(x => x.IMPORT_DATE <= importEDate.Value);
            }
            return q.Select(x=>new F700701QueryData
            {
                IMPORT_DATE = x.IMPORT_DATE,
                GRP_ID = int.Parse(x.GRP_ID.ToString()),
                PERSON_NUMBER = short.Parse(x.PERSON_NUMBER.ToString()),
                WORK_HOUR = x.WORK_HOUR,
                SALARY = x.SALARY,
                DC_CODE = x.DC_CODE,
                CRT_STAFF = x.CRT_STAFF,
                CRT_DATE = x.CRT_DATE,
                CRT_NAME = x.CRT_NAME,
                UPD_STAFF = x.UPD_STAFF,
                UPD_DATE = x.UPD_DATE,
                UPD_NAME = x.UPD_NAME,
            });
        }
    }
}
