using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F02
{
	public partial class F02020104Repository : RepositoryBase<F02020104, Wms3plDbContext, F02020104Repository>
	{
		public F02020104Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F02020104> GetDatas(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq)
		{
			var reuslt = _db.F02020104s.Where(x => x.DC_CODE == dcCode
																				&& x.GUP_CODE == gupCode
																				&& x.CUST_CODE == custCode
																				&& x.PURCHASE_NO == purchaseNo
																				&& x.PURCHASE_SEQ == purchaseSeq);
			return reuslt;
		}

    public IQueryable<F02020104> GetIsPassDatas(string dcCode, string gupCode, string custCode, string rtNo)
    {
      var result = _db.F02020104s.Where(x => x.DC_CODE == dcCode
                                        && x.GUP_CODE == gupCode
                                        && x.CUST_CODE == custCode
                                        && x.RT_NO == rtNo
                                        && x.ISPASS == "1");
      return result;
    }

    public IQueryable<string> GetSnList(string dcCode, string custCode, string gupCode, List<string> rtNo)
		{
			var result = _db.F02020104s.AsNoTracking().Where(x => x.DC_CODE == dcCode
																											 && x.CUST_CODE == custCode
																											 && x.GUP_CODE == gupCode
																											 && x.ISPASS == "1"
																											 && rtNo.Contains(x.RT_NO))
																								.Select(x => x.SERIAL_NO);
			return result;

		}

		public IQueryable<F02020104> GetSnListForRtNos(string dcCode, string custCode, string gupCode, List<string> rtNos)
		{
			return _db.F02020104s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.CUST_CODE == custCode &&
			x.GUP_CODE == gupCode &&
			rtNos.Contains(x.RT_NO) &&
			x.ISPASS == "1");
		}


		public IQueryable<string> GetExistSerialNos(string dcCode, string gupCode, string custCode, string itemCode, string purchaseNo, string rtNo, IEnumerable<string> serialNos, string isPass)
		{
			var reuslt = GetDatas(dcCode, gupCode, custCode, itemCode, purchaseNo, rtNo, serialNos, isPass)
																.Select(x => x.SERIAL_NO);
			return reuslt;
		}

		public IQueryable<F02020104> GetDatas(string dcCode, string gupCode, string custCode, string itemCode, string purchaseNo, string rtNo, IEnumerable<string> serialNos, string isPass, string purchaseSeq = null)
		{
			var q = _db.F02020104s.Where(x => x.DC_CODE == dcCode
																							&& x.GUP_CODE == gupCode
																							&& x.CUST_CODE == custCode
																							&& x.ITEM_CODE == itemCode
																							&& x.PURCHASE_NO == purchaseNo
																							&& x.RT_NO == rtNo
																							&& serialNos.Contains(x.SERIAL_NO)
																							&& x.ISPASS == isPass);
			if (!string.IsNullOrEmpty(purchaseSeq))
			{
				q = q.Where(x => x.PURCHASE_SEQ == purchaseSeq);
			}

			return q;
		}
	}
}
