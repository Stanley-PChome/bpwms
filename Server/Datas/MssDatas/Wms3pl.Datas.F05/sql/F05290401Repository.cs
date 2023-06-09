using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F05290401Repository : RepositoryBase<F05290401, Wms3plDbContext, F05290401Repository>
	{
	   public IQueryable<F05290401> GetDatas(string dcCode,string gupCode,string custCode,string pickOrdNo,string containerCode)
		{
			var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo, containerCode };
			var sql = @" SELECT *
                     FROM F05290401
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND PICK_ORD_NO = @p3
                      AND CONTAINER_CODE = @p4 ";
			return SqlQuery<F05290401>(sql, parms);
		}

		public IQueryable<F05290401> GetDatasByItems(string dcCode,string gupCode,string custCode,string pickOrdNo,string containerCode,List<string> itemCodes)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, pickOrdNo, containerCode };
			var sql = @" SELECT *
                     FROM F05290401
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND PICK_ORD_NO = @p3
                      AND CONTAINER_CODE = @p4 ";
			if (itemCodes.Any())
				sql += parms.CombineSqlInParameters(" AND ITEM_CODE", itemCodes);

			return SqlQuery<F05290401>(sql, parms.ToArray());
		}
		public IQueryable<F05290401> GetLackDatasByPickOrdNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo };
			var sql = @" SELECT *
                     FROM F05290401 A
                     JOIN F052904 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.PICK_ORD_NO = A.PICK_ORD_NO
                      AND B.CONTAINER_CODE = A.CONTAINER_CODE
                    WHERE B.DC_CODE = @p0
                      AND B.GUP_CODE = @p1
                      AND B.CUST_CODE = @p2
                      AND B.PICK_ORD_NO = @p3
                      AND B.STATUS ='3' -- 缺貨
                      AND A.B_SET_QTY > A.A_SET_QTY ";
			return SqlQuery<F05290401>(sql, parms);
		}

		public IQueryable<F05290401> GetLackDatasByPickContainerCode(string dcCode, string gupCode, string custCode, string pickOrdNo, string containerCode)
		{
			var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo, containerCode };
			var sql = @" SELECT *
                     FROM F05290401
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND PICK_ORD_NO = @p3
                      AND CONTAINER_CODE = @p4
                      AND B_SET_QTY > A_SET_QTY ";
			return SqlQuery<F05290401>(sql, parms);
		}
	}
}
