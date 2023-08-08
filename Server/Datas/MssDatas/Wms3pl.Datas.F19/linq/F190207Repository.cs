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
	public partial class F190207Repository : RepositoryBase<F190207, Wms3plDbContext, F190207Repository>
	{
		public F190207Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		/// <summary>
		/// 取得新ID
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public Int16 GetNewId(string gupCode, string itemCode, string custCode)
		{
			var tmp = this.Filter(x => x.GUP_CODE.Equals(gupCode) && x.ITEM_CODE.Equals(itemCode) && x.CUST_CODE.Equals(custCode))
				.OrderByDescending(x => x.IMAGE_NO).FirstOrDefault();
			if (tmp == null) return 1;
			return Convert.ToInt16(tmp.IMAGE_NO + 1);
		}

		public bool GetDataIsExist(string gupCode, string itemCode, string custCode)
		{
            return _db.F190207s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.CUST_CODE == custCode).Any();
		}
	}
}
