using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1945Repository : RepositoryBase<F1945, Wms3plDbContext, F1945Repository>
	{
		/// <summary>
		/// 取得該使用者集貨場資料
		/// </summary>
		/// <param name="empId"></param>
		/// <returns></returns>
		public IQueryable<CollectionInfo> GetCollectionCode(string empId)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", empId));

			string sql = $@" SELECT DISTINCT 
											b.DC_CODE DcCode,
											b.COLLECTION_CODE CollectionCode,
											b.COLLECTION_NAME CollectionName
											FROM  F192402 a, F1945 b 
											Where b.DC_CODE = A.DC_CODE AND a.EMP_ID=@p0 ";
			var result = SqlQuery<CollectionInfo>(sql, parm.ToArray());
			return result;
		}

        #region P1901920000集貨場維護
        public IQueryable<F1945CollectionList> GetF1945CollectionList(string dcCode, string CollectionCode, string CollectionType)
        {
            var para = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@p0",SqlDbType=SqlDbType.VarChar,Value=dcCode}
            };
            var sql = $@"
                SELECT DISTINCT a.DC_CODE,a.COLLECTION_CODE,a.COLLECTION_NAME,a.COLLECTION_TYPE,b.NAME COLLECTION_TYPE_NAME
                FROM F1945 a 
                INNER JOIN VW_F000904_LANG b 
                  ON b.TOPIC ='F1945' AND b.SUBTOPIC='COLLECTION_TYPE' AND a.COLLECTION_TYPE =b.VALUE AND b.LANG='{Current.Lang}' 
                WHERE DC_CODE=@p0";

            if (!String.IsNullOrWhiteSpace(CollectionCode))
            {
                sql += @" AND a.COLLECTION_CODE=@p" + para.Count;
                para.Add(new SqlParameter() { ParameterName = "@p" + para.Count, SqlDbType = SqlDbType.VarChar, Value = CollectionCode });
            }
            if (!String.IsNullOrWhiteSpace(CollectionType))
            {
                sql += @" AND a.COLLECTION_TYPE=@p" + para.Count;
                para.Add(new SqlParameter() { ParameterName = "@p" + para.Count, SqlDbType = SqlDbType.Char, Value = CollectionType });
            }

            var result = SqlQuery<F1945CollectionList>(sql, para.ToArray());
            return result;
        }

        public IQueryable<F1945CellList> GetF1945CellList(string dcCode, string CollectionCode)
        {
            var para = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@p0",SqlDbType=SqlDbType.VarChar,Value=dcCode},
                new SqlParameter() { ParameterName = "@p1", SqlDbType = SqlDbType.VarChar, Value = CollectionCode }
            };
            var sql = $@"
                SELECT a.DC_CODE, a.CELL_TYPE, c.CELL_NAME, a.CELL_START_CODE, a.CELL_NUM, a.CRT_DATE, a.CRT_NAME, a.UPD_NAME, a.UPD_DATE,'N' MODIFY_MODE,CAST(1 as bit) IS_SHOW_DELETE_BUTTON 
                FROM F1945 a 
                LEFT JOIN F194501 c 
                  ON a.DC_CODE =c.DC_CODE AND a.CELL_TYPE =c.CELL_TYPE WHERE a.DC_CODE=@p0 AND a.COLLECTION_CODE=@p1";
            
            var result = SqlQuery<F1945CellList>(sql, para.ToArray());
            return result;
        }

        #endregion P1901920000集貨場維護

    }
}
