using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;


namespace Wms3pl.Datas.F50
{
	public partial class F500103Repository : RepositoryBase<F500103, Wms3plDbContext, F500103Repository>
	{
        public IQueryable<F500103QueryData> GetF500103QueryData(string dcCode, DateTime? enableSDate, DateTime? enableEDate
            , string quoteNo, string status)
        {
            var sqlParamers = new List<object>
            {
                dcCode
            };

            var sql = $@"						
						SELECT
							A.*
							,B.NAME STATUS_NAME
							,ISNULL(C.DC_NAME, N'不指定') AS DC_NAME
							,UPLOAD_S_PATH UPLOAD_FILE
						FROM F500103 A
						JOIN VW_F000904_LANG B ON B.TOPIC  ='P500102' AND B.SUBTOPIC ='STATUS' AND B.VALUE =A.STATUS AND B.LANG = '{Current.Lang}'
						LEFT JOIN F1901 C ON C.DC_CODE=A.DC_CODE     
						LEFT JOIN F910404 D ON D.QUOTE_NO = A.QUOTE_NO AND D.DC_CODE = A.DC_CODE AND D.GUP_CODE =A.GUP_CODE AND D.CUST_CODE = A.CUST_CODE
						WHERE A.DC_CODE =@p0
			";

            //單據編號
            if (!string.IsNullOrEmpty(quoteNo))
            {
                sql += " AND A.QUOTE_NO = @p" + sqlParamers.Count;
                sqlParamers.Add(quoteNo);
            }

            //Status
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND A.STATUS = @p" + sqlParamers.Count;
                sqlParamers.Add(status);
            }
            else
            {
                sql += " AND A.STATUS <> '9'";
            }

            //有效日期-起
            if (enableSDate.HasValue)
            {
                sql += " AND A.ENABLE_DATE >= @p" + sqlParamers.Count;
                sqlParamers.Add(enableSDate);
            }

            //有效日期-迄
            if (enableEDate.HasValue)
            {
                enableEDate = enableEDate.Value.AddDays(1);
                sql += " AND A.ENABLE_DATE <= @p" + sqlParamers.Count;
                sqlParamers.Add(enableEDate);
            }

            sql += " order by A.QUOTE_NO ";
            var result = SqlQuery<F500103QueryData>(sql, sqlParamers.ToArray()).AsQueryable();
            return result;
        }
    }
}
