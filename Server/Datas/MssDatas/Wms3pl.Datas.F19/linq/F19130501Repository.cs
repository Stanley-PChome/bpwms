using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F19130501Repository : RepositoryBase<F19130501, Wms3plDbContext, F19130501Repository>
  { 
    public F19130501Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
    { }

    /// <summary>
    /// 取得批次快速移轉序號排程要處理的資料
    /// </summary>
    /// <returns></returns>
    public IQueryable<F19130501> GetProcessDatas(string Mode)
    {
      var result = _db.F19130501s
        .Where(x => x.STATUS == "0");


      if (Mode == "01")
        result = result.Where(x => x.ACTION_TYPE == "A");
      else
        result = result.Where(x => x.ACTION_TYPE == "D");

      return result.OrderBy(x=>x.CRT_DATE).Take(5000);
    }

  }
}
