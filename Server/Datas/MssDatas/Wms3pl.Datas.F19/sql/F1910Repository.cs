using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1910Repository : RepositoryBase<F1910, Wms3plDbContext, F1910Repository>
    {
        /// <summary>
        /// 取得已過濾人員權限的門市主檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="retailCode"></param>
        /// <param name="retailName"></param>
        /// <returns></returns>
        public IQueryable<F1910> GetAllowedF1910s(string gupCode, string custCode, string retailCode, string retailName)
        {
            var sql = @"
                        -- 用 DISTINCT 是因人員權限包含 DC_CODE 可能會重複
                        SELECT DISTINCT A.*                        
                        FROM   F1910 A
                               JOIN F1909 B
                                 ON B.GUP_CODE = A.GUP_CODE
                                    AND CASE
                                          WHEN B.ALLOWGUP_RETAILSHARE = '1' THEN '0'
                                          ELSE B.CUST_CODE
                                        END = A.CUST_CODE
                               JOIN F192402 C
                                 ON C.GUP_CODE = B.GUP_CODE
                                    AND C.CUST_CODE = B.CUST_CODE
                        WHERE  C.CUST_CODE = @p0
                               AND C.EMP_ID = @p1
                               AND ( CASE
                                       WHEN @p2 = '' THEN '1'
                                       ELSE C.GUP_CODE
                                     END ) = ( CASE
                                                 WHEN @p2 = '' THEN '1'
                                                 ELSE @p2
                                               END )
                               AND ( CASE
                                       WHEN @p3 = '' THEN '1'
                                       ELSE A.RETAIL_CODE
                                     END ) = ( CASE
                                                 WHEN @p3 = '' THEN '1'
                                                 ELSE @p3
                                               END ) 
                        ";

            var parameter = new List<SqlParameter>()
            {
                new SqlParameter("@p0", custCode),
                new SqlParameter("@p1", Current.Staff),
                new SqlParameter("@p2", gupCode),
                new SqlParameter("@p3", retailCode)
            };

            if (!string.IsNullOrEmpty(retailName))
            {
                sql += @" AND A.RETAIL_NAME LIKE '%' + @p4 + '%' ";
                parameter.Add(new SqlParameter("@p4", retailName));
            }
            
            return SqlQuery<F1910>(sql, parameter.ToArray());
        }

        public void UpdateHasKey(F1910 data, string oldChannel)
        {
            var sql = @" 
                        UPDATE F1910
                          SET RETAIL_NAME = @p0,
                              CONTACT = @p1,
                              TEL = @p2,
                              MAIL = @p3,
                              ADDRESS = @p4,
                              CHANNEL = @p5,
                              UPD_DATE = @p6,
                              UPD_STAFF = @p7,
                              UPD_NAME = @p8,
                              SHORT_SALESBASE_NAME = @p9,
                              UNIFIED_BUSINESS_NO = @p10,
                              TEL2 = @p11,
                              FAX = @p12,
                              NOTE = @p13,
                              SALES_BASE_GROUP = @p14,
                              SELF_TAKE = @p15,
                              DELV_NO = @p16,
                              NEED_SHIPPING_MARK = @p17,
                              CUSTOM_DELVDAYS_TYPE = @p18,
                              DELV_DAYS = @p19,
                              DELV_DAYS_LIMIT = @p20,
                              DELV_DAYS_INFO = @p21
                        WHERE GUP_CODE = @p22
                          AND CUST_CODE = @p23
                          AND CHANNEL = @p24
                          AND RETAIL_CODE = @p25
                        ";
            var parms = new List<object> {data.RETAIL_NAME,data.CONTACT,data.TEL,data.MAIL,data.ADDRESS,
                data.CHANNEL,DateTime.Now,Current.Staff,Current.StaffName,

                data.SHORT_SALESBASE_NAME,
                data.UNIFIED_BUSINESS_NO,
                data.TEL2,
                data.FAX,
                data.NOTE,
                //一般設定
                data.SALES_BASE_GROUP,
                data.SELF_TAKE,
                data.DELV_NO,
                data.NEED_SHIPPING_MARK,
                //允出設定
                data.CUSTOM_DELVDAYS_TYPE,
                data.DELV_DAYS,
                data.DELV_DAYS_LIMIT,
                data.DELV_DAYS_INFO,

                data.GUP_CODE, data.CUST_CODE, oldChannel, data.RETAIL_CODE
            };
            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
