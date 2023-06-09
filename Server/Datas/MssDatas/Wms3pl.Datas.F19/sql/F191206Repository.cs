using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191206Repository : RepositoryBase<F191206, Wms3plDbContext, F191206Repository>
    {
        public IQueryable<F191206> GetF191206sResult(string dcCode, string pickFloor, string pkArea)
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", pickFloor),
            };
            var sql = $@"SELECT * 
                        FROM F191206
                        WHERE DC_CODE = @p0
                              AND PICK_FLOOR = @p1";
            if (!string.IsNullOrEmpty(pkArea))
            {
                //sql += $"     AND PK_AREA LIKE '%' + @p" + param.Count + "+'%'";
                sql += $" AND PK_AREA = @p" + param.Count;
                param.Add(new SqlParameter("@p" + param.Count, pkArea));
            }
            var result = SqlQuery<F191206>(sql, param.ToArray());
            return result;
        }

        /// <summary>
        /// 先增資料前檢查F191206是否有重複用
        /// </summary>
        /// <param name="f191206"></param>
        /// <returns></returns>
        public Boolean CheckF191206Duplicate(F191206 f191206)
        {
            var sql = @"SELECT TOP 1 
                            * 
                        FROM F191206 
                        WHERE PK_AREA=@p0 AND DC_CODE=@p1";

            var param = new[] {
                new SqlParameter("@p0",f191206.PK_AREA),
                new SqlParameter("@p1",f191206.DC_CODE)
            };

            return SqlQuery<F191206>(sql, param).Any();
        }

        public IQueryable<F191206> GetF191206s(String DC_CODE, String PK_AREA)
        {
            var param = new List<SqlParameter>()
            {
                new SqlParameter("@p0",DC_CODE),
                new SqlParameter("@p1",PK_AREA)
            };
            var sql = @"SELECT
                            * 
                        FROM 
                            F191206 
                        WHERE 
                            DC_CODE=@p0 
                            AND PK_AREA=@p1";
            return SqlQuery<F191206>(sql, param.ToArray());
        }
 
    }
}
