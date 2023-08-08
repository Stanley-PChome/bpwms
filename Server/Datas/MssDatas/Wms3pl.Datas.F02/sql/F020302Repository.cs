using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F020302Repository : RepositoryBase<F020302, Wms3plDbContext, F020302Repository>
	{

		public IQueryable<P020205Detail> GetJincangNoFileDetail(string dcCode, string gupCode, string custCode
, string fileName, string poNo)
		{
			var sqlParamers = new List<object> { dcCode, gupCode, custCode, fileName };

			var sql = $@"
                    SELECT 
	                    ROW_NUMBER()OVER(ORDER BY DC_CODE ASC, GUP_CODE ASC,CUST_CODE ASC,FILE_NAME ASC,SERIAL_NO ASC) AS ROWNUM, 
	                    '' SYS_CUST_CODE,
			                     A.FILE_NAME,
			                     A.PO_NO,
			                     A.ITEM_CODE,
                           (SELECT ITEM_NAME
                              FROM F1903
                             WHERE F1903.GUP_CODE = A.GUP_CODE AND F1903.ITEM_CODE = A.ITEM_CODE AND F1903.CUST_CODE = A.CUST_CODE)
                              ITEM_NAME,
                           A.SERIAL_NO,
                           A.SERIAL_LEN,
                           A.VALID_DATE,
	                       B.NAME AS STATUS_NAME
                      FROM F020302 A LEFT JOIN VW_F000904_LANG B ON A.STATUS = B.VALUE AND B.LANG = '{Current.Lang}' 
                     WHERE     A.DC_CODE = @p0
                           AND A.GUP_CODE = @p1
                           AND A.CUST_CODE = @p2
                           AND A.FILE_NAME = @p3
	                       AND B.TOPIC = 'F020302'
                    ";

			if (!string.IsNullOrWhiteSpace(poNo))
			{
				sql += " AND A.PO_NO = @p" + sqlParamers.Count;
				sqlParamers.Add(poNo);
			}

			return SqlQuery<P020205Detail>(sql, sqlParamers.ToArray());
		}

		public void UpdateSerialCancel(string dcCode, string gupCode, string custCode, string stockNo, string itemCode, List<string> serialNos)
		{
			var sqlParamers = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", itemCode),
								new SqlParameter("@p4", stockNo),
								new SqlParameter("@p5", Current.Staff),
								new SqlParameter("@p6", Current.StaffName),
								new SqlParameter("@p7", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
						};
			var sql = @" UPDATE F020302 
                      SET F020302.STATUS = '9',
                          F020302.UPD_DATE = @p7,
                          F020302.UPD_STAFF = @p5,
                          F020302.UPD_NAME = @p6
                    WHERE F020302.DC_CODE = @p0
                      AND F020302.GUP_CODE = @p1
                      AND F020302.CUST_CODE = @p2
                      AND F020302.ITEM_CODE = @p3
                      AND F020302.PO_NO IN( SELECT SHOP_NO 
                                  FROM F010201 B
                                 WHERE B.DC_CODE = F020302.DC_CODE
                                   AND B.GUP_CODE = F020302.GUP_CODE
                                   AND B.CUST_CODE = F020302.CUST_CODE
                                   AND B.STOCK_NO = @p4)
                     ";
			if (serialNos.Any())
			{
				var str = string.Empty;
				foreach (var serial in serialNos)
				{
					var key = "@p" + sqlParamers.Count;
					str += (string.IsNullOrWhiteSpace(str) ? "" : ",") + key;
					sqlParamers.Add(new SqlParameter(key, serial));
				}
				sql += " AND A.SERIAL_NO NOT IN (" + str + ") ";
			}
			ExecuteSqlCommand(sql, sqlParamers.ToArray());
		}
		public void BulkDelete(string dcCode, string gupCode, string custCode, List<string> stockNos)
		{
			var sql = @" DELETE FROM F020302
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND PO_NO = @p3 ";
			var parms = new List<object>();
			sql += parms.CombineSqlInParameters("AND PO_NO", stockNos);
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public void DeleteWithCancelAcceptance(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
		{
			var sql = @" DELETE FROM F020302
										WHERE DC_CODE = @p0
										AND GUP_CODE = @p1
										AND CUST_CODE = @p2
                    AND FILE_NAME LIKE 'USERCHK99_%'
										AND EXISTS
										(SELECT 1
										 FROM F02020104
										 WHERE F02020104.DC_CODE = F020302.DC_CODE
										 AND F02020104.GUP_CODE = F020302.GUP_CODE
										 AND F02020104.CUST_CODE = F020302.CUST_CODE
										 AND F02020104.SERIAL_NO = F020302.SERIAL_NO
										 AND F02020104.PURCHASE_NO = @p3
										 AND F02020104.RT_NO = @p4
										 AND F02020104.ISPASS = '1')";
			ExecuteSqlCommand(sql, new object[] { dcCode, gupCode, custCode, purchaseNo, rtNo });
		}

		public void CancelNotProcessWarehouseInF020302(string dcCode, string gupCode, string custCode, string shopNo)
		{
			string sql = @"
				           update F020302 set STATUS = '9', UPD_DATE = @p6, UPD_STAFF = @p0, UPD_NAME = @p1
                           Where DC_CODE =@p2
				             and GUP_CODE =@p3
				             and CUST_CODE =@p4
                             and PO_NO=@p5
                             and STATUS <> '9'
			               ";
			var sqlParams = new SqlParameter[]
			{
								 new SqlParameter("@p0", Current.Staff),
								 new SqlParameter("@p1", Current.StaffName),
								 new SqlParameter("@p2", dcCode),
								 new SqlParameter("@p3", gupCode),
								 new SqlParameter("@p4", custCode),
								 new SqlParameter("@p5", shopNo),
								 new SqlParameter("@p6", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
			};

			ExecuteSqlCommand(sql, sqlParams);
		}

		/// <summary>
		/// 查詢序號是否有重複（需注意丟入的序號數量不可超過SQL規定的2100-5筆）
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="poNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="largeSerialNo"></param>
		/// <returns></returns>
		public IQueryable<F020302> CheckRepeatSerails(string dcCode, string gupCode, string custCode, string poNo, string itemCode, string[] largeSerialNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
				new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
				new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
				new SqlParameter("@p3", SqlDbType.VarChar) { Value = poNo },
				new SqlParameter("@p4", SqlDbType.VarChar) { Value = itemCode },
			};
			var sql = @"SELECT * FROM F020302 WHERE DC_CODE=@p0 AND GUP_CODE =@p1 AND CUST_CODE =@p2 AND PO_NO !=@p3 AND ITEM_CODE =@p4 AND STATUS='0'";
			sql += parms.CombineSqlInParameters("AND SERIAL_NO", largeSerialNo, SqlDbType.VarChar);
			return SqlQuery<F020302>(sql, parms.ToArray());

		}

		public ItemSn FindPurchaseItemSn(string dcCode, string gupCode, string custCode, List<string> purchaseNoList, string barcode)
		{
			var parms = new List<SqlParameter>
					 {
						 new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
						 new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
						 new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
						 new SqlParameter("@p3",barcode){SqlDbType = SqlDbType.VarChar},
					 };

			var sql = @" SELECT TOP(1) A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE,A.SERIAL_NO
                          FROM F020302 A
                          JOIN F020301 B
                            ON B.DC_CODE = A.DC_CODE
                           AND B.GUP_CODE = A.GUP_CODE
                           AND B.CUST_CODE = A.CUST_CODE
                           AND B.FILE_NAME = A.FILE_NAME
                        WHERE B.DC_CODE = @p0
                          AND B.GUP_CODE = @p1
                          AND B.CUST_CODE = @p2
                          AND A.STATUS = '0'
                          AND A.SERIAL_NO = @p3 ";
			sql += parms.CombineSqlInParameters(" AND B.PURCHASE_NO ", purchaseNoList, SqlDbType.VarChar);
			return SqlQuery<ItemSn>(sql, parms.ToArray()).FirstOrDefault();
		}

		public IQueryable<string> GetCollectionSerials(string dcCode, string gupCode, string custCode, string stockNo, string shopNo, string itemCode,List<string> serialNoList=null)
		{
			var sql = @" SELECT B.SERIAL_NO
                      FROM F020301 A 
                      JOIN F020302 B
                        ON B.DC_CODE = A.DC_CODE
                       AND B.GUP_CODE = A.GUP_CODE
                       AND B.CUST_CODE = A.CUST_CODE
                       AND B.FILE_NAME = A.FILE_NAME 
                     WHERE A.DC_CODE = @p0
                       AND A.GUP_CODE = @p1
                       AND A.CUST_CODE = @p2
                       AND B.STATUS = '0' 
                       AND (B.PO_NO = @p3 or A.PURCHASE_NO = @p4)
                       AND B.ITEM_CODE = @p5 ";
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",shopNo){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p4",stockNo){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p5",itemCode){SqlDbType = SqlDbType.VarChar}
			};
			if (serialNoList!=null && serialNoList.Any())
				sql += parms.CombineSqlInParameters(" AND B.SERIAL_NO ", serialNoList, SqlDbType.VarChar);
			return SqlQuery<string>(sql, parms.ToArray());
		}

		public void DeleteF020302(string dcCode,string gupCode,string custCode,string purchaseNo,string poNo,string itemCode,string serialNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",purchaseNo){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p4",poNo){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p5",itemCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p6",serialNo){SqlDbType = SqlDbType.VarChar}
			};

			var sql = @" DELETE FROM F020302 
                   WHERE DC_CODE = @p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE = @p2
                     AND (PO_NO = @p3 OR PO_NO = @p4)
                     AND ITEM_CODE = @p5
                     AND SERIAL_NO = @p6
                     AND STATUS = '0' ";
			ExecuteSqlCommand(sql, parms.ToArray());
		}

    public void DeleteWithCancelAcceptance(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string rtNo)
    {
      var parameter = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", purchaseNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4", purchaseSeq) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p5", rtNo) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @" DELETE FROM F020302
										WHERE DC_CODE = @p0
										AND GUP_CODE = @p1
										AND CUST_CODE = @p2
                    AND FILE_NAME LIKE 'USERCHK99_%'
										AND EXISTS
										(SELECT 1
										 FROM F02020104
										 WHERE F02020104.DC_CODE = F020302.DC_CODE
										 AND F02020104.GUP_CODE = F020302.GUP_CODE
										 AND F02020104.CUST_CODE = F020302.CUST_CODE
										 AND F02020104.SERIAL_NO = F020302.SERIAL_NO
										 AND F02020104.PURCHASE_NO = @p3
										 AND F02020104.PURCHASE_SEQ = @p4
										 AND F02020104.RT_NO = @p5
										 AND F02020104.ISPASS = '1')";
      ExecuteSqlCommand(sql, parameter.ToArray());
    }
  }
}
