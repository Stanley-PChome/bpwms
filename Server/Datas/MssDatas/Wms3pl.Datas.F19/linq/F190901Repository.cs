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
    public partial class F190901Repository : RepositoryBase<F190901, Wms3plDbContext, F190901Repository>
	{
		public F190901Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		// 取可用發票編號代碼
		public F190901 GetInvoCode(string gupCode, string custCode, string yearCode, string monthCode)
		{
            return _db.F190901s
                        .Where(x => x.GUP_CODE == gupCode
                         && x.CUST_CODE == custCode
                         && x.INVO_YEAR == yearCode
                         && x.INVO_MON == monthCode
                         && (
                                (Decimal.Parse(x.INVO_NO) >= Decimal.Parse(x.INVO_NO_BEGIN)
                                && 
                                Decimal.Parse(x.INVO_NO) < Decimal.Parse(x.INVO_NO_END))
                                || x.INVO_NO == null
                         ))
                         .OrderBy(x=>x.INVO_NO)
                         .ThenBy(x=>x.CRT_DATE)
                         .ThenBy(x=>x.INVO_TITLE)
                         .Select(x=>x)
                         .FirstOrDefault();
		}



		
	}
}