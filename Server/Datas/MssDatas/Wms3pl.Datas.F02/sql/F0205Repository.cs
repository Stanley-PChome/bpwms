using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F0205Repository : RepositoryBase<F0205, Wms3plDbContext, F0205Repository>
    {
        /// <summary>
        /// P0202060100用，檢查商品是否均已放入容器內，是的話就回傳true
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="RTNo"></param>
        /// <param name="RTSEQ"></param>
        /// <returns></returns>
        public Boolean CheckAllContainerIsDone(string dcCode, string gupCode, string custCode, string RTNo, string RTSEQ)
        {
            var sql =
                @"SELECT TOP 1 * FROM F0205 A WHERE a.DC_CODE=@p0 AND a.GUP_CODE=@p1 AND a.CUST_CODE=@p2 AND a.RT_NO=@p3 AND a.RT_SEQ=@p4 AND B_QTY-A_QTY>0";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("@p3",SqlDbType.VarChar){Value=RTNo},
                new SqlParameter("@p4",SqlDbType.VarChar){Value=RTSEQ}
            };
            return !SqlQuery<F0205>(sql, para.ToArray()).Any();

        }

        /// <summary>
        /// P0202060100用，取得畫面所需資訊
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="RTNo"></param>
        /// <param name="RTSEQ"></param>
        /// <returns></returns>
        public IQueryable<ItemBindContainerData> GetItemBindContainerData(string dcCode, string gupCode, string custCode, string RTNo, String RTSEQ)
        {
            var sql =
                @"SELECT a.*,b.ITEM_NAME,c.WAREHOUSE_ID+' '+c.WAREHOUSE_NAME PICK_WARE_Name FROM F0205 a 
                    INNER JOIN F1903 b ON a.ITEM_CODE=b.ITEM_CODE AND a.GUP_CODE=b.GUP_CODE AND a.CUST_CODE=b.CUST_CODE 
                    LEFT JOIN F1980 c ON a.DC_CODE=c.DC_CODE AND a.PICK_WARE_ID=c.WAREHOUSE_ID
                WHERE a.DC_CODE=@p0 AND a.GUP_CODE=@p1 AND a.CUST_CODE=@p2 AND a.RT_NO=@p3 AND a.RT_SEQ=@p4";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("@p3",SqlDbType.VarChar){Value=RTNo},
                new SqlParameter("@p4",SqlDbType.VarChar){Value=RTSEQ}
            };
            return SqlQuery<ItemBindContainerData>(sql, para.ToArray());
        }

    public F0205 GetData(string dcCode, string gupCode, string custCode, string rtNo, string rtSeq, string typeCode)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", rtNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", rtSeq) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p5", typeCode) { SqlDbType = SqlDbType.Char },
      };

      var sql = @"SELECT TOP 1 * FROM F0205 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND RT_NO=@p3 AND RT_SEQ=@p4 AND TYPE_CODE=@p5";
      return SqlQuery<F0205>(sql, para.ToArray()).FirstOrDefault();
    }


    public IQueryable<F0205> GetDatas(string dcCode, string gupCode, string custCode, IEnumerable<string> rtNos)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0", dcCode){ SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode){ SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode){ SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"SELECT * FROM F0205 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2";
      var sqlItemFilter = para.CombineSqlInParameters(" AND RT_NO", rtNos, SqlDbType.VarChar);
      sql += sqlItemFilter;

      return SqlQuery<F0205>(sql, para.ToArray());
    }

  }
}
