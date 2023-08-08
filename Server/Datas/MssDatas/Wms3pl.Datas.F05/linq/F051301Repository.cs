using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
  public partial class F051301Repository : RepositoryBase<F051301, Wms3plDbContext, F051301Repository>
  {
    public F051301Repository(string connName, WmsTransaction wmsTransaction = null)
          : base(connName, wmsTransaction)
    {
    }


    public F051301 GetData(string dcCode,string gupCode,string custCode,DateTime DelvDate,string pickTime)
    {
      return _db.F051301s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.DELV_DATE == DelvDate && x.PICK_TIME == pickTime).SingleOrDefault();
    }
  }
}
