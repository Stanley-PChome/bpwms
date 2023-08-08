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
    public partial class F199001Repository : RepositoryBase<F199001, Wms3plDbContext, F199001Repository>
    {
        public F199001Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F199001Ex> GetF199001Exs(string dcCode, string locTypeID, string tmprType, string status)
        {

            var f199001s = _db.F199001s.AsNoTracking().Where(x => x.DC_CODE == dcCode);
            var f1942s = _db.F1942s.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(locTypeID) && locTypeID != "")
            {
                f199001s = f199001s.Where(x => x.LOC_TYPE_ID == locTypeID);
            }
            if (!string.IsNullOrWhiteSpace(tmprType) && tmprType != "")
            {
                f199001s = f199001s.Where(x => x.TMPR_TYPE == tmprType);
            }
            if (status == "")
            {
                f199001s = f199001s.Where(x => x.STATUS != "9");
            }
            else
            {
                f199001s = f199001s.Where(x => x.STATUS ==status);
            }

            var result = from A in f199001s
                         join B in f1942s on A.LOC_TYPE_ID equals B.LOC_TYPE_ID
                         select new F199001Ex
                         {
                             DC_CODE = A.DC_CODE,
                             TMPR_TYPE = A.TMPR_TYPE,
                             LOC_TYPE_ID = A.LOC_TYPE_ID,
                             ACC_UNIT = A.ACC_UNIT,
                             ACC_NUM = Convert.ToInt16(A.ACC_NUM),
                             UNIT_FEE = A.UNIT_FEE,
                             CRT_STAFF = A.CRT_STAFF,
                             CRT_NAME = A.CRT_NAME,
                             CRT_DATE = A.CRT_DATE,
                             UPD_STAFF = A.UPD_STAFF,
                             UPD_NAME = A.UPD_NAME,
                             UPD_DATE = A.UPD_DATE,
                             IN_TAX = A.IN_TAX,
                             STATUS = A.STATUS,
                             ITEM_TYPE_ID = A.ITEM_TYPE_ID,
                             ACC_ITEM_KIND_ID = A.ACC_ITEM_KIND_ID,
                             ACC_ITEM_NAME = A.ACC_ITEM_NAME,
                             LENGTH = Convert.ToInt16(B.LENGTH),
                             WEIGHT = Convert.ToInt16(B.WEIGHT),
                             HEIGHT = Convert.ToInt16(B.HEIGHT),
                             DEPTH = Convert.ToInt16(B.DEPTH)
                         };
            return result;
        }

        public IQueryable<F199001> GetF199001SameAccItemName(string dcCode, string accItemName)
        {
            var result = _db.F199001s.Where(x => x.DC_CODE == dcCode
                                           && x.ACC_ITEM_NAME == accItemName);
            return result;
        }
    }

}
