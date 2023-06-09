using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075109Repository : RepositoryBase<F075109, Wms3plDbContext, F075109Repository>
	{
    public string LockF075109()
    {
      var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F075109';";
      return SqlQuery<string>(sql).FirstOrDefault();
    }

  }
}
