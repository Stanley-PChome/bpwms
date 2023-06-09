using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910302Repository : RepositoryBase<F910302, Wms3plDbContext, F910302Repository>
    {
        public IQueryable<F910302Data> GetContractDetails(string dcCode, string gupCode, string contractNo)
        {
            var paramers = new List<SqlParameter>();

            paramers.Add(new SqlParameter("@p0", string.IsNullOrEmpty(dcCode) ? (object)DBNull.Value : dcCode));
            paramers.Add(new SqlParameter("@p1", gupCode));
            paramers.Add(new SqlParameter("@p2", string.IsNullOrWhiteSpace(contractNo) ? (object)DBNull.Value : contractNo));

            var strSQL = @"SELECT DISTINCT a.*,a.CONTRACT_TYPE,CASE WHEN a.CONTRACT_TYPE = '0' THEN N'主約' ELSE  N'附約'  END  as CONTRACT_TYPENAME,
														CASE WHEN a.ITEM_TYPE = '001' THEN 
																 a.QUOTE_NO + '-' + ba.QUOTE_NAME
																 WHEN a.ITEM_TYPE = '002' THEN
																 a.QUOTE_NO + '-' + bb.ACC_ITEM_NAME 				 
																 WHEN a.ITEM_TYPE = '003' THEN
																 a.QUOTE_NO + '-' + be.ACC_ITEM_NAME 				 
																 WHEN a.ITEM_TYPE = '004' THEN
																 a.QUOTE_NO + '-' + bd.ACC_ITEM_NAME 				 
																 WHEN a.ITEM_TYPE = '005' THEN
																 a.QUOTE_NO + '-' + bc.ACC_ITEM_NAME 				 
																 WHEN a.ITEM_TYPE = '006' THEN
																 a.QUOTE_NO + '-' + bf.ACC_ITEM_NAME 	
                                 WHEN a.ITEM_TYPE = '007' THEN
                                 a.QUOTE_NO + '-' + bg.ACC_PROJECT_NAME				 
														END AS QUOTE_NAME,
														c.ITEM_TYPE as ITEM_TYPE_NAME,d.ACC_UNIT_NAME as UNIT,e.PROCESS_ACT  
											 FROM (SELECT bb.*,cc.CUST_CODE 
															 FROM F910301 aa 
												  LEFT JOIN F910302 bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE AND aa.CONTRACT_NO = bb.CONTRACT_NO
													LEFT JOIN F1909 cc ON aa.GUP_CODE = cc.GUP_CODE AND aa.UNI_FORM = cc.UNI_FORM AND cc.STATUS != '9') a 
								  LEFT JOIN F910401 ba ON a.DC_CODE = ba.DC_CODE AND a.GUP_CODE = ba.GUP_CODE AND a.CUST_CODE = ba.CUST_CODE AND a.QUOTE_NO = ba.QUOTE_NO
									LEFT JOIN F500101 bb ON a.DC_CODE = bb.DC_CODE AND a.GUP_CODE = bb.GUP_CODE AND a.QUOTE_NO = bb.QUOTE_NO
									LEFT JOIN F500102 bc ON a.DC_CODE = bc.DC_CODE AND a.GUP_CODE = bc.GUP_CODE AND a.QUOTE_NO = bc.QUOTE_NO
									LEFT JOIN F500103 bd ON a.DC_CODE = bd.DC_CODE AND a.GUP_CODE = bd.GUP_CODE AND a.QUOTE_NO = bd.QUOTE_NO
									LEFT JOIN F500104 be ON a.DC_CODE = be.DC_CODE AND a.GUP_CODE = be.GUP_CODE AND a.QUOTE_NO = be.QUOTE_NO
									LEFT JOIN F500105 bf ON a.DC_CODE = bf.DC_CODE AND a.GUP_CODE = bf.GUP_CODE AND a.QUOTE_NO = bf.QUOTE_NO
								  LEFT JOIN F910003 c ON a.ITEM_TYPE = c.ITEM_TYPE_ID
								  LEFT JOIN F91000302 d ON a.ITEM_TYPE = d.ITEM_TYPE_ID AND a.UNIT_ID = d.ACC_UNIT
								  LEFT JOIN F910001 e ON a.PROCESS_ID = e.PROCESS_ID
                  LEFT JOIN F199007 bg ON a.DC_CODE = bg.DC_CODE AND a.GUP_CODE = bg.GUP_CODE AND a.CUST_CODE = bg.CUST_CODE AND a.QUOTE_NO = bg.ACC_PROJECT_NO
								      WHERE a.DC_CODE = @p0 
												AND a.GUP_CODE = @p1
												AND a.CONTRACT_NO = @p2 ";

            return SqlQuery<F910302Data>(strSQL, paramers.ToArray());
        }
    }
}
