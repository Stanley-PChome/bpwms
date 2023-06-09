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
    public partial class F194713Repository : RepositoryBase<F194713, Wms3plDbContext, F194713Repository>
    {
        public F194713Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public F194713 Get(string dcCode, string gupCode, string custCode, string allId, string eservice)
        {
            var result = _db.F194713s.Where(x => x.DC_CODE == dcCode
                                        && x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && x.ALL_ID == allId
                                        && x.ESERVICE == eservice).FirstOrDefault();
            return result;
        }
        public IQueryable<EServiceItem> GetAllEServiceItem()
        {

            var result = _db.F194713s.AsNoTracking().Select(x => new EServiceItem {
                ESERVICE = x.ESERVICE,
                ESERVICE_NAME = x.ESERVICE_NAME
            }).Distinct();
            return result;
        }

        public IQueryable<F194713> GetDatas(string dcCode, string gupCode, string custCode, List<string> allIdList, List<string> eserviceList)
        {
            var result = _db.F194713s.Where(x => x.DC_CODE == dcCode
                                        && x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && allIdList.Contains(x.ALL_ID)
                                        && eserviceList.Contains(x.ESERVICE));
            return result;
        }
    }
}
