using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F196301Repository : RepositoryBase<F196301, Wms3plDbContext, F196301Repository>
    {
        /// <summary>
		/// 批次刪除, 依LocCode
		/// 考量到資料量問題 (可能上萬筆), 如果用Parameter傳入in參數, 可能會有參數數量限制, 因此用此作法
		/// </summary>
		/// <param name="workgroupId"></param>
		/// <param name="dcCode"></param>
		/// <param name="locList"></param>
		public void DeleteLocIn(decimal workgroupId, string dcCode, List<string> locList)
        {
            if (locList.Count == 0)
                return;

            var entities = locList.Select(x => new F196301
            {
                WORK_ID = workgroupId,
                DC_CODE = dcCode,
                LOC_CODE = x
            }).ToList();

            SqlBulkDeleteForAnyCondition(entities, "F196301", new List<string> { "WORK_ID", "DC_CODE", "LOC_CODE" });
        }

    public void DeleteLocIn_2(decimal workgroupId, string dcCode, List<F1912LocData> locList)
    {
      if (locList.Count == 0)
        return;

      var entities = locList.Select(x => new F196301
      {
        WORK_ID = workgroupId,
        DC_CODE = x.DC_CODE,
        LOC_CODE = x.LOC_CODE
      }).ToList();

      SqlBulkDeleteForAnyCondition(entities, "F196301", new List<string> { "WORK_ID", "DC_CODE", "LOC_CODE" });
    }

    /// <summary>
    /// 取得未設定儲位
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="warehouseId"></param>
    /// <param name="floor"></param>
    /// <param name="beginLocCode"></param>
    /// <param name="endLocCode"></param>
    /// <param name="workId"></param>
    /// <returns></returns>
    public IQueryable<F1912LocData> GetNonAllowedF1912LocDatas(string dcCode, string warehouseId, string floor, string beginLocCode, string endLocCode, string workId)
        {
            var parameters = new object[] { dcCode,
                warehouseId, warehouseId,
                floor, floor,
                beginLocCode, beginLocCode,
                endLocCode,endLocCode,
                workId };

            var sql = @"
                            SELECT A.DC_CODE,
                                   A.AREA_CODE,
                                   A.LOC_CODE
                            FROM   F1912 A
                            WHERE  A.DC_CODE = @p0
                            AND    A.WAREHOUSE_ID =
                                   CASE
                                          WHEN @p1 = '' THEN A.WAREHOUSE_ID
                                          ELSE @p2
                                   END
                            AND    A.FLOOR =
                                   CASE
                                          WHEN @p3 = '' THEN A.FLOOR
                                          ELSE @p4
                                   END
                            AND    A.LOC_CODE BETWEEN (
                                   CASE WHEN @p5 = '' THEN A.LOC_CODE
                                          ELSE @p6
                                   END)
                            AND    (
                                          CASE WHEN @p7 = '' THEN A.LOC_CODE
                                                 ELSE @p8
                                          END)
                            AND    NOT EXISTS
                                   (
                                          SELECT 1
                                          FROM   F196301 B
                                          WHERE  B.WORK_ID = @p9
                                          AND    A.DC_CODE = B.DC_CODE
                                          AND    A.LOC_CODE = B.LOC_CODE)
                            ";

            return SqlQuery<F1912LocData>(sql, parameters);
        }

        /// <summary>
		/// 取得未設定儲位
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="warehouseId"></param>
		/// <param name="floor"></param>
		/// <param name="beginLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="excludeLocCodes">排除的儲位編號</param>
		/// <returns></returns>
		public IQueryable<F1912LocData> GetNonAllowedF1912LocDatas(string dcCode, string warehouseId, string floor, string beginLocCode, string endLocCode, IEnumerable<string> excludeLocCodes)
        {
            var parameterList = new List<object> { dcCode, warehouseId, floor, beginLocCode, endLocCode };
            int paramStartIndex = parameterList.Count;
            var notInSql = parameterList.CombineSqlNotInParameters("A.LOC_CODE", excludeLocCodes, ref paramStartIndex);

            var sql = @"SELECT A.DC_CODE, A.AREA_CODE, A.LOC_CODE
						  FROM F1912 A
						 WHERE     A.DC_CODE = @p0
						       AND A.WAREHOUSE_ID = ISNULL ( @p1, A.WAREHOUSE_ID)
						       AND A.FLOOR = ISNULL ( @p2, A.FLOOR)
						       AND A.LOC_CODE BETWEEN ISNULL ( @p3, A.LOC_CODE)
						                          AND ISNULL ( @p4, A.LOC_CODE)
						       AND " + notInSql;

            return SqlQuery<F1912LocData>(sql, parameterList.ToArray());
        }
    }
}
