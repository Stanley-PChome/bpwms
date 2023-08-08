using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F19470101Repository : RepositoryBase<F19470101, Wms3plDbContext, F19470101Repository>
    {
        public F19470101Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }


        public IQueryable<F19470101Datas> GetF19470101Datas(string ALL_ID, string DC_CODE)
        {
            var result = _db.F19470101s.AsNoTracking().Where(x => x.ALL_ID == ALL_ID
                                                            && x.DC_CODE == DC_CODE)
                                                            .Select(x=> new F19470101Datas
                                                            {
                                                                ZIP_CODE = x.ZIP_CODE,
                                                                DELV_EFFIC = x.DELV_EFFIC,
                                                                DELV_TIME = x.DELV_TIME,
                                                                DELV_TMPR = x.DELV_TMPR
                                                            });
            return result;
        }
    }
}
