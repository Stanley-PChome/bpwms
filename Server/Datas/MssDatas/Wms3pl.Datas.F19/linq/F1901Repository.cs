using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1901Repository : RepositoryBase<F1901, Wms3plDbContext, F1901Repository>
	{
		public F1901Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
            
		public F1901 GetFirstData()
		{
            var query_1 = _db.F1901s.Select(x=>x);
            var result = query_1.OrderBy(x => x.DC_CODE).FirstOrDefault();//.SingleOrDefault() 在ORA中可允許過，但於此不允許 NULL 
            return result;
        }

		public IQueryable<string> GetAllDcCode()
		{
            var query = _db.F1901s.Select(x =>
            new 
            {
                DC_CODE = x.DC_CODE
            });
            return query.Select(x => x.DC_CODE);
        }

		public IQueryable<F1901> GetAll()
		{           
            return _db.F1901s.Select(x => x);
		}

		public List<string> GetDcCodesByDcCode(string dcCode)
		{
			var f1901s = _db.F1901s.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(dcCode))
				f1901s = f1901s.Where(x => x.DC_CODE == dcCode);

			return f1901s.Select(x => x.DC_CODE).ToList();
		}
	}
}
