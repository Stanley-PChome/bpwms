using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F25
{
	public partial class F2501Repository : RepositoryBase<F2501, Wms3plDbContext, F2501Repository>
	{
		public IQueryable<F2501QueryData> Get2501QueryData(string gupCode, string custCode,
			 string[] itemCode, string boxSerial, string batchNo, string[] serialNo, string cellNum, string poNo
			, string[] wmsNo, string status, string OrdProp, string retailCode, Int16? combinNo
			, string crtName, DateTime? crtSDate, DateTime? crtEDate, DateTime? updSDate, DateTime? updEDate)
		{

      //var sqlParamers = new List<object> { gupCode, custCode };
      var sqlParamers = new List<SqlParameter>
      {
        new SqlParameter("@p0",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1",custCode) { SqlDbType = System.Data.SqlDbType.VarChar }
      };

      var sql = $@"	      
          select a.SERIAL_NO,			a.GUP_CODE,			a.CUST_CODE,			ISNULL(a.BOUNDLE_ITEM_CODE, a.ITEM_CODE) AS ITEM_CODE,              b.ITEM_SPEC,
                  a.STATUS,			b.TYPE,					a.BOX_SERIAL,			a.BATCH_NO,				a.TAG3G,
		            a.PUK,				a.VALID_DATE,			a.CASE_NO,				a.PO_NO,				a.WMS_NO,
			          a.IN_DATE,			h.ORD_PROP_NAME,		a.RETAIL_CODE,		case	when a.ACTIVATED = '1' then '是'
																					          else '否'
																			          end         ACTIVATED,	a.PROCESS_NO,
		          a.COMBIN_NO,		a.CELL_NUM,				i.VNR_NAME,				j.VNR_NAME  SYS_NAME,	a.CAMERA_NO,
		          CLIENT_IP,			b.ITEM_UNIT,			case	when a.SEND_CUST = '1' then '是'
														          else '否'
													          end		SEND_CUST,		f.NAME  STATUS_NAME,	g.NAME  ITEM_TYPE,
		          b.ITEM_NAME,		d.GUP_NAME,				e.CUST_NAME,			a.CRT_DATE,				a.CRT_NAME,
		          a.UPD_DATE,			a.UPD_NAME,				a.BOUNDLE_ITEM_CODE,	a.COMBIN_NO	,			b.BUNDLE_SERIALLOC
              , case when a.STATUS ='A1' and b.BUNDLE_SERIALLOC !=1 then '1' else '0' end CanDelete
          from   F2501 a
          left join F1903 b		on a.ITEM_CODE = b.ITEM_CODE        and a.GUP_CODE = b.GUP_CODE
						          and a.CUST_CODE = b.CUST_CODE
          join F1929 d on d.GUP_CODE = a.GUP_CODE
          join F1909 e on e.GUP_CODE = a.GUP_CODE and a.CUST_CODE = e.CUST_CODE
          left join VW_F000904_LANG f     on f.TOPIC = 'F2501'	and f.SUBTOPIC = 'STATUS'
							          AND f.VALUE = a.STATUS	AND f.LANG = '{Current.Lang}'
          left join VW_F000904_LANG g     on g.TOPIC = 'F1903'    and g.SUBTOPIC = 'TYPE'
							          AND g.VALUE = b.TYPE	AND g.LANG = '{Current.Lang}'
          left join F000903 h     on h.ORD_PROP = a.ORD_PROP
          left join F1908 i     on i.VNR_CODE = a.VNR_CODE    and i.GUP_CODE = a.GUP_CODE
							          and i.CUST_CODE = a.CUST_CODE
          left join F1908 j     on j.VNR_CODE = a.SYS_VNR     and j.GUP_CODE = a.GUP_CODE
							          and j.CUST_CODE= a.CUST_CODE
          where  a.GUP_CODE = @p0
          AND a.CUST_CODE = @p1 			";
			var filterSql = string.Empty;

			//ITEM_CODE
			if (itemCode.Any())
			{
        //filterSql += sqlParamers.CombineSqlInParameters(" AND ", " COALESCE(NULLIF(a.BOUNDLE_ITEM_CODE,''), a.ITEM_CODE) ", itemCode);
        //filterSql += "AND  isnull(a.BOUNDLE_ITEM_CODE, a.ITEM_CODE) = @itemcode";
        //sqlParamers.Add(new SqlParameter("@itemCode", itemCode) { SqlDbType = System.Data.SqlDbType.VarChar });
        filterSql += sqlParamers.CombineSqlInParameters(" AND isnull(a.BOUNDLE_ITEM_CODE,a.ITEM_CODE) ", itemCode, SqlDbType.VarChar);
       
      }

			//SERIAL_NO
			if (serialNo.Any())
			{
				filterSql += sqlParamers.CombineSqlInParameters(" AND a.SERIAL_NO ", serialNo,SqlDbType.VarChar);
			}

			//WMS_NO
			if (wmsNo.Any())
			{
				filterSql += sqlParamers.CombineSqlInParameters(" AND a.WMS_NO ", wmsNo , SqlDbType.VarChar);
			}

			//BOX_SERIAL
			if (!string.IsNullOrEmpty(boxSerial))
			{
				filterSql += " AND a.BOX_SERIAL = @pboxSerial ";
        sqlParamers.Add(new SqlParameter("@boxSerial", boxSerial) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

			//BATCH_NO
			if (!string.IsNullOrEmpty(batchNo))
			{
        filterSql += " AND a.BATCH_NO = @batchNo ";
        sqlParamers.Add(new SqlParameter("@batchNo", batchNo) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

			//CELL_NUM
			if (!string.IsNullOrEmpty(cellNum))
			{
        filterSql += " AND a.CELL_NUM = @pcellNum ";
        sqlParamers.Add(new SqlParameter("@cellNum", cellNum) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

			//PO_NO
			//sql += sqlParamers.CombineNotNullOrEmpty(" AND a.PO_NO = @p{0} ", poNo);
      if (!string.IsNullOrEmpty(poNo))
      {
        //filterSql += sqlParamers.Combine(" AND a.PO_NO = @p{0} ", poNo);
        filterSql += " AND a.PO_NO = @poNo ";
        sqlParamers.Add(new SqlParameter("@poNo", poNo) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

      //STATUS
      if (!string.IsNullOrEmpty(status))
			{
				//filterSql += sqlParamers.Combine(" AND a.STATUS = @p{0} ", status);
        filterSql += " AND a.STATUS = @status ";
        sqlParamers.Add(new SqlParameter("@status", status) { SqlDbType = System.Data.SqlDbType.VarChar });
      }
			//itemType
			if (!string.IsNullOrEmpty(OrdProp))
			{
				//filterSql += sqlParamers.Combine(" AND b.TYPE = @p{0} ", itemType);
        filterSql += " AND h.ORD_PROP = @OrdProp ";
        sqlParamers.Add(new SqlParameter("@OrdProp", OrdProp) { SqlDbType = System.Data.SqlDbType.VarChar });
      }
			//RETAIL_CODE 客戶代號
			//if (!string.IsNullOrEmpty(retailCode))
			//{
			//	filterSql += sqlParamers.Combine(" AND a.RETAIL_CODE = @p{0} ", retailCode);
			//}
			//COMBIN_NO BOUNDEL_ID組合商品編號
			if (combinNo.HasValue && combinNo != 0)
			{
				//filterSql += sqlParamers.Combine(" AND a.COMBIN_NO = @p{0} ", combinNo);
        filterSql += " AND a.COMBIN_NO = @combinNo ";
        sqlParamers.Add(new SqlParameter("@combinNo", combinNo) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

			//CRT_NAME
			if (!string.IsNullOrEmpty(crtName))
			{
				//filterSql += sqlParamers.Combine(" AND a.CRT_NAME = @p{0} ", crtName);
        filterSql += " AND a.CRT_NAME = @crtName ";
        sqlParamers.Add(new SqlParameter("@crtName", crtName) { SqlDbType = System.Data.SqlDbType.VarChar });
      }

			//建立日期-起
			if (crtSDate.HasValue)
			{
				//filterSql += sqlParamers.Combine(" AND a.CRT_DATE >= @p{0} ", crtSDate);
        filterSql += " AND a.CRT_DATE >= @crtSDate ";
        sqlParamers.Add(new SqlParameter("@crtSDate", crtSDate) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }
			//建立日期-迄
			if (crtEDate.HasValue)
			{
				crtEDate = crtEDate.Value.AddDays(1);
				//filterSql += sqlParamers.Combine(" AND a.CRT_DATE <= @p{0} ", crtEDate);
        filterSql += " AND a.CRT_DATE <= @crtEDate ";
        sqlParamers.Add(new SqlParameter("@crtEDate", crtEDate) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }
			//修改日期-起
			if (updSDate.HasValue)
			{
				//filterSql += sqlParamers.Combine(" AND a.UPD_DATE >= @p{0} ", updSDate);
        filterSql += " AND a.UPD_DATE >= @updSDate ";
        sqlParamers.Add(new SqlParameter("@updSDate", updSDate) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }
			//修改日期-迄
			if (updEDate.HasValue)
			{
				updEDate = updEDate.Value.AddDays(1);
				//filterSql += sqlParamers.Combine(" AND a.UPD_DATE <= @p{0} ", updEDate);
        filterSql += " AND a.UPD_DATE <= @updEDate ";
        sqlParamers.Add(new SqlParameter("@updEDate", updEDate) { SqlDbType = System.Data.SqlDbType.DateTime2 });
      }

			sql += filterSql;
			sql += " order by a.SERIAL_NO ";
			var result = SqlQuery<F2501QueryData>(sql, sqlParamers.ToArray()).AsQueryable();
			return result;

		}

		//檢查序號凍結狀態
		public IQueryable<F250102Data> GetSerialIsFreeze(string gupCode, string custCode, string controlType, IEnumerable<string> serialNos)
		{
			var sql = @"
                        SELECT C.GUP_CODE,
							                           C.CUST_CODE,
							                           C.SERIAL_NO,
							                           D.FREEZE_TYPE,
							                           E.CONTROL
						                          FROM F2501 C
							                           JOIN F250102 D
								                          ON     C.GUP_CODE = D.GUP_CODE
									                         AND C.CUST_CODE = D.CUST_CODE
									                         AND (   (    C.SERIAL_NO >= D.SERIAL_NO_BEGIN
											                          AND C.SERIAL_NO <= D.SERIAL_NO_END
											                          AND LEN (D.SERIAL_NO_BEGIN) = LEN (C.SERIAL_NO)) --序號符合也算
										                          OR (    C.BOX_SERIAL = D.BOX_SERIAL
											                          AND D.BOX_SERIAL IS NOT NULL)                   --盒號符合也算
										                          OR (C.BATCH_NO = D.BATCH_NO AND D.BATCH_NO IS NOT NULL) --儲值卡盒號也算
																								                         )
									                         AND (D.FREEZE_BEGIN_DATE <= @p0 AND @p1 <= D.FREEZE_END_DATE)
							                           JOIN F25010201 E
								                          ON     D.LOG_SEQ = E.FREEZE_LOG_SEQ
									                         AND D.GUP_CODE = E.GUP_CODE
									                         AND D.CUST_CODE = E.CUST_CODE
						                         WHERE     D.FREEZE_TYPE = '0'                                          -- 凍結中
							                           AND E.CONTROL = @p2
							                           AND C.GUP_CODE = @p3
							                           AND C.CUST_CODE = @p4
                        ";

			var paramList = new List<object>
						{
								DateTime.Today,
								DateTime.Today,
								controlType,
								gupCode,
								custCode
						};

			sql += paramList.CombineSqlInParameters("AND C.SERIAL_NO", serialNos);

			return SqlQuery<F250102Data>(sql, paramList.ToArray());
		}

       

       

      

        

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="serialNo"></param>
		/// <returns></returns>
		public F2501WcfData GetF2501WcfData(string gupCode, string custCode, string serialNo)
		{
			var sql = @"
                        SELECT    UPPER(A.SERIAL_NO) SERIAL_NO,
                                  A.ITEM_CODE,
                                  A.BOX_SERIAL,
                                  A.TAG3G,
                                  A.CELL_NUM,
                                  A.PUK,
                                  A.STATUS,
                                  A.GUP_CODE,
                                  A.CUST_CODE,
                                  A.CRT_DATE,
                                  A.CRT_STAFF,
                                  A.UPD_DATE,
                                  A.UPD_STAFF,
                                  A.CRT_NAME,
                                  A.UPD_NAME,
                                  A.BATCH_NO,
                                  A.VALID_DATE,
                                  A.PO_NO,
                                  A.ACTIVATED,
                                  A.SEND_CUST,
                                  A.WMS_NO,
                                  A.VNR_CODE,
                                  A.SYS_VNR,
                                  A.PROCESS_NO,
                                  A.ORD_PROP,
                                  A.CASE_NO,
                                  A.IN_DATE,
                                  A.RETAIL_CODE,
                                  A.COMBIN_NO,
                                  A.CAMERA_NO,
                                  A.CLIENT_IP,
                                  B.DC_CODE,
                                  B.LOC_CODE,
                                  C.WAREHOUSE_ID,
                                  D.BUNDLE_SERIALLOC,
                                  B.BOX_CTRL_NO,
                                  B.PALLET_CTRL_NO,
                                  B.MAKE_NO
                        FROM      F2501 A
                        LEFT JOIN F1913 B
                        ON        A.SERIAL_NO = B.SERIAL_NO
                        AND       A.GUP_CODE = B.GUP_CODE
                        AND       A.CUST_CODE = B.CUST_CODE
                        LEFT JOIN F1912 C
                        ON        B.LOC_CODE = C.LOC_CODE
                        AND       B.DC_CODE = C.DC_CODE
                        JOIN      F1903 D
                        ON        A.ITEM_CODE = D.ITEM_CODE
                        AND       A.GUP_CODE = D.GUP_CODE
                        AND       A.CUST_CODE = D.CUST_CODE
                        WHERE     A.SERIAL_NO = @p0
                        AND       A.GUP_CODE = @p1
                        AND       A.CUST_CODE = @p2
                        ORDER BY A.GUP_CODE, A.CUST_CODE, A.SERIAL_NO
                        ";
			return SqlQuery<F2501WcfData>(sql, new object[] { serialNo, gupCode, custCode }).FirstOrDefault();
		}

        

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="serialNo"></param>
		public void ProcessClearCombinNo(string gupCode, string custCode, List<string> serialNo)
		{
			if (!serialNo.Any())
				return;
			var parameters = new List<object>
						{
								Current.Staff,
								Current.StaffName,
                DateTime.Now,
                gupCode,
								custCode
						};
			var inSql = parameters.CombineSqlInParameters("AND SERIAL_NO", serialNo);

			var sql = $@"
                        UPDATE F2501
                        SET    BOUNDLE_ITEM_CODE = NULL,
                               COMBIN_NO = NULL,
                               UPD_STAFF=@p0,
                               UPD_NAME = @p1,
                               UPD_DATE = @p2
                        WHERE  GUP_CODE = @p3
                        AND    CUST_CODE = @p4
                        AND    COMBIN_NO IS NOT NULL {inSql}
                        ";
			ExeSqlCmdCountMustGreaterZero(sql, "序號無組合編號，不可進行拆解", parameters.ToArray());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allocationNo"></param>
		/// <param name="itemCodes"></param>
		/// <param name="countColumnName"></param>
		/// <param name="isUp"></param>
		/// <returns></returns>
		public IQueryable<ClearSerialBoxOrCaseNo> GetcClearSerialBoxOrCaseNoesByAllocation(string dcCode, string gupCode, string custCode,
			string allocationNo, List<string> itemCodes, string countColumnName, bool isUp)
		{
			var locColumn = isUp ? "TAR_LOC_CODE" : "SRC_LOC_CODE";
			var sql = @"
SELECT ROW_NUMBER() OVER(ORDER BY A.{0},A.ITEM_CODE ) AS ROWNUM,
       A.{0} AS BoxOrCaseNo
FROM   (
                 SELECT    A.{0},
                           A.ITEM_CODE,
                           Count(DISTINCT ISNULL(B.{1}, '999999999')) COUNTLOC
                 FROM      F2501 A
                 LEFT JOIN
                           (
                                      SELECT     B.GUP_CODE,
                                                 B.CUST_CODE,
                                                 B.SERIAL_NO,
                                                 B.ITEM_CODE,
                                                 B.{1},
                                                 C.{0}
                                      FROM       F151002 B
                                      INNER JOIN F2501 C
                                      ON         B.GUP_CODE = C.GUP_CODE
                                      AND        B.CUST_CODE = C.CUST_CODE
                                      AND        B.SERIAL_NO = C.SERIAL_NO
                                      WHERE      B.DC_CODE =@p0
                                      AND        B.GUP_CODE =@p1
                                      AND        B.CUST_CODE =@p2
                                      AND        B.ALLOCATION_NO=@p3 ) B
                 ON        B.GUP_CODE = A.GUP_CODE
                 AND       B.CUST_CODE = A.CUST_CODE
                 AND       B.SERIAL_NO = A.SERIAL_NO
                 AND       B.{0} = A.{0}
                 WHERE     A.GUP_CODE = @p4
                 AND       A.CUST_CODE= @p5
                 AND       A.{0} IS NOT NULL
                 AND       A.{0} <> '' {2}
                 GROUP BY  A.{0},
                           A.ITEM_CODE) A";
			var param = new List<object>
						{
								dcCode,gupCode,custCode,allocationNo,gupCode,custCode
						};
			var sqlIn = param.CombineSqlInParameters("AND A.ITEM_CODE", itemCodes);
			sql = string.Format(sql, countColumnName, locColumn, sqlIn);
			if (countColumnName == "CASE_NO")
			{
				sql += string.Format(@"
                                       LEFT JOIN (SELECT ITEM_CODE,
                                                         UNIT_QTY
                                                  FROM   F190301 B
                                                  WHERE  GUP_CODE = @p6
                                                     AND UNIT_ID = '05') B
                                              ON B.ITEM_CODE = A.ITEM_CODE
                                WHERE  A.COUNTLOC > ISNULL(B.UNIT_QTY, 1) 
                                ", param.Count);
				param.Add(gupCode);
			}
			else
			{
				sql += " WHERE A.COUNTLOC > 1 ";
			}


			return SqlQuery<ClearSerialBoxOrCaseNo>(sql, param.ToArray());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="listSerialNo"></param>
		/// <param name="validDate"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="userId"></param>
		/// <param name="userName"></param>
		public void UpdateF2501ValidDate(List<string> listSerialNo, DateTime validDate, string gupCode, string custCode,
			string userId, string userName)
		{
			var parameters = new List<SqlParameter>
															{
																new SqlParameter("@p0", validDate),
																new SqlParameter("@p1", userId),
																new SqlParameter("@p2", userName),
																new SqlParameter("@p3", gupCode),
																new SqlParameter("@p4", custCode),
                                new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
                              };

			var sql = @"
                        UPDATE F2501
                        SET    VALID_DATE = @p0,
                               UPD_STAFF = @p1,
                               UPD_NAME = @p2,
                               UPD_DATE = @p5
                        WHERE  GUP_CODE = @p3
                           AND CUST_CODE = @p4
                           AND SERIAL_NO IN ( {0} ) 
                        ";

			var serialNos = string.Format("'{0}'", string.Join("','", listSerialNo));
			ExecuteSqlCommand(string.Format(sql, serialNos), parameters.ToArray());
		}

		public IQueryable<P2501Data> GetDataPprocessing1(string custCode, string gupCode, DateTime updDate, int rownum)
		{
			string sql = @"SELECT CUST_CODE as CustNo,
                                ITEM_CODE as ItemNo,
                                SERIAL_NO as Sn,
                                VALID_DATE as ValidDate,
                                STATUS as Status,
                                UPD_DATE as UpdDate,
                                CRT_DATE as CrtDate
                            FROM F2501 
                            WHERE CUST_CODE = @p0
                            AND GUP_CODE = @p1
                            AND ISNULL(UPD_DATE, CRT_DATE) > @p2 
                            ORDER BY ISNULL(UPD_DATE, CRT_DATE)
                            offset (@p3) rows
                            fetch next (1000) rows only
                            ";
			var paramers = new[] {
								new SqlParameter("@p0", custCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", updDate.ToString("yyyy-MM-dd HH:mm:ss")),
								new SqlParameter("@p3",  rownum>1000?rownum-1:rownum)
						};
			var result = SqlQuery<P2501Data>(sql, paramers);
			return result;
		}
		
		/// <summary>
		/// 更新序號
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="oldSn"></param>
		/// <param name="newSn"></param>
		public void UpdateNewSnByOldSn(string gupCode, string custCode, string oldSn, string newSn)
		{
			var parameters = new SqlParameter[]
			{
								new SqlParameter("@p0", newSn),
								new SqlParameter("@p1", Current.Staff),
								new SqlParameter("@p2", Current.StaffName),
								new SqlParameter("@p3", gupCode),
								new SqlParameter("@p4", custCode),
								new SqlParameter("@p5", oldSn),
                new SqlParameter("@p6", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

			var sql = @"
                            UPDATE F2501
                            SET    SERIAL_NO = @p0,
                                   UPD_STAFF = @p1,
                                   UPD_NAME = @p2,
                                   UPD_DATE = @p6
                            WHERE  GUP_CODE = @p3
                               AND CUST_CODE = @p4
                               AND SERIAL_NO = @p5 
                            ";

			ExecuteSqlCommand(sql, parameters.ToArray());
		}
		public F2501 GetDataByBarCode(string gupCode,string custCode,string barcode)
		{
			var parms = new List<object> { gupCode, custCode, barcode };
			var sql = @" SELECT *
                     FROM F2501
                    WHERE GUP_CODE = @p0
                      AND CUST_CODE = @p1
                      AND SERIAL_NO = @p2 ";
			return SqlQuery<F2501>(sql, parms.ToArray()).FirstOrDefault();
		}


		public F1903 GetF1903DataBySerialNo(string gupCode, string custCode, string serialNo)
		{
			var parms = new List<object> { gupCode, custCode, serialNo };
			var sql = @" SELECT B.*
                     FROM F2501 A
                     JOIN f1903 B 
                     ON A.GUP_CODE =B.GUP_CODE 
                     AND A.CUST_CODE =B.CUST_CODE 
                     AND A.ITEM_CODE =B.ITEM_CODE 
                    WHERE A.GUP_CODE = @p0
                      AND A.CUST_CODE = @p1
                      AND A.SERIAL_NO = @p2 
					  AND A.STATUS  = 'A1'";
			return SqlQuery<F1903>(sql, parms.ToArray()).FirstOrDefault();
		}

		public IQueryable<F2501> GetiItemSnData(string gupCode,string custCode)
		{
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1",custCode) { SqlDbType = System.Data.SqlDbType.VarChar }
      };

      //var parms = new List<object> { gupCode, custCode };
			var sql = @"SELECT * FROM F2501 
						WHERE GUP_CODE  = @p0
						  AND CUST_CODE = @p1
						  AND IS_ASYNC  IN( 'N','F')
						  AND STATUS    = 'A1'
						ORDER BY IS_ASYNC , ISNULL(UPD_DATE,CRT_DATE)";

			return SqlQuery<F2501>(sql, parms.ToArray());
		}

		public void UpdateIsAsync(string isAsync, string gupCode, string custCode, List<string> serialNos)
		{
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",isAsync) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2",custCode) { SqlDbType = System.Data.SqlDbType.VarChar }
      };

      var sql = @"UPDATE F2501
        					SET IS_ASYNC = @p0 
                  WHERE GUP_CODE = @p1
								    AND CUST_CODE = @p2";
          
			
      sql += parms.CombineSqlInParameters(" AND SERIAL_NO", serialNos, System.Data.SqlDbType.VarChar);
      ExecuteSqlCommand(sql, parms.ToArray());
		}

    public void DeleteBySnList(string gupCode, string custCode, IEnumerable<string> SnList)
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1",custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", Current.Staff ){ SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p3", Current.StaffName){ SqlDbType = System.Data.SqlDbType.VarChar },
		new SqlParameter("@p4", DateTime.Now){ SqlDbType = System.Data.SqlDbType.DateTime2 },
	  };
      var sql = @" Update  F2501
                  set ORD_PROP = 'UD',
                      UPD_STAFF = @p2,
                      UPD_NAME = @p3,
                      UPD_DATE = @p4
                   WHERE GUP_CODE = @p0
                     AND CUST_CODE = @p1 ";
      if (!SnList.Any())
        sql += " AND 1= 0";
      else
        sql += parms.CombineSqlInParameters(" AND SERIAL_NO", SnList, System.Data.SqlDbType.VarChar);

      ExecuteSqlCommand(sql, parms.ToArray());
    }

    public IQueryable<F2501> GetDatasBySql(string gupCode, string custCode, IEnumerable<string> snList)
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1",custCode) { SqlDbType = System.Data.SqlDbType.VarChar }
      };
      var sql = @" SELECT * FROM F2501
                   WHERE GUP_CODE = @p0
                     AND CUST_CODE = @p1 ";
      if (!snList.Any())
        sql += " AND 1= 0";
      else
        sql += parms.CombineSqlInParameters(" AND SERIAL_NO", snList, System.Data.SqlDbType.VarChar);

      return SqlQuery<F2501>(sql, parms.ToArray());
    }

		public int GetCountBySnTag(string gupCode,string custCode,string snTag)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",gupCode) {SqlDbType = System.Data.SqlDbType.VarChar},
				new SqlParameter("@p1",custCode){SqlDbType = System.Data.SqlDbType.VarChar}
			};
			var sql = $@" SELECT ISNULL(COUNT(*),0)
                     FROM F2501 
                    WHERE GUP_CODE = @p0
                      AND CUST_CODE = @p1
                      AND SERIAL_NO LIKE '{snTag}%'";
			return SqlQuery<int>(sql, parms.ToArray()).FirstOrDefault();
		}

    public void UpdateSerialActivated(string gupCode, string custCode, List<string> serialNos, string isActivated)
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",isActivated){SqlDbType = System.Data.SqlDbType.VarChar},
        new SqlParameter("@p1",gupCode){SqlDbType = System.Data.SqlDbType.VarChar},
        new SqlParameter("@p2",custCode){SqlDbType = System.Data.SqlDbType.VarChar}
      };

      var sql = @"UPDATE F2501 SET ACTIVATED = @p0
                    WHERE GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND SERIAL_NO IN ({0})";

      StringBuilder sqlIn = new StringBuilder();

      foreach (var serialNo in serialNos)
      {
        sqlIn.Append($"@p{parms.Count},");
        parms.Add(new SqlParameter($"@p{parms.Count}", SqlDbType.VarChar) { Value = serialNo });
      }

      sqlIn.Remove(sqlIn.Length - 1, 1);
      sql = string.Format(sql, sqlIn.ToString());

      ExecuteSqlCommand(sql, parms.ToArray());
    }

    public string GetSrialNoToProcess(string gupCode, string custCode, string serialNo)
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode) {SqlDbType = System.Data.SqlDbType.VarChar},
        new SqlParameter("@p1", custCode) {SqlDbType = System.Data.SqlDbType.VarChar},
        new SqlParameter("@p2", serialNo) {SqlDbType = System.Data.SqlDbType.VarChar}
      };

      var sql = @"
                SELECT 
                  SERIAL_NO 
                FROM 
                  F2501 
                WHERE 
                  GUP_CODE = @p0 
                  AND CUST_CODE = @p1 
                  AND SERIAL_NO = @p2
                ";

      return SqlQuery<string>(sql, parms.ToArray()).FirstOrDefault();
    }


		public void UpdateOrdPropAndWmsOrdNo(string gupCode, string custCode, string ordProp, string wmsOrdNo, List<string> snList)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", ordProp) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", wmsOrdNo) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", custCode) { SqlDbType = SqlDbType.VarChar });

			var sql = @"UPDATE F2501
						   SET ORD_PROP = @p0, WMS_NO = @p1
						 WHERE GUP_CODE = @p2 AND CUST_CODE = @p3
						";
			if (snList.Any())
				sql += sqlParameter.CombineSqlInParameters(" AND SERIAL_NO ", snList, SqlDbType.VarChar);

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

    public IQueryable<string> GetDataForWcsApiOutWarehouseReceipt(string gupCode, string custCode, List<WcsApiOutWarehouseReceiptItemSerialModel> itemSerialNos)
    {
      if (itemSerialNos == null || !itemSerialNos.Any())
        return new List<string>().AsQueryable();

      //避免參數超過2100個，要做分批查詢
      var paraCount = 0;
      var batchCount = 200;
      var batchItemSerialNos = new List<List<WcsApiOutWarehouseReceiptItemSerialModel>>();
      var tmpBatchItemSerialNos = new List<WcsApiOutWarehouseReceiptItemSerialModel>();
      var res = new List<string>();
      paraCount = 2; // gup & cust
      foreach (var itemSerialNo in itemSerialNos)
      {
        //參數量超過設定值，就截斷
        if (paraCount > batchCount)
        {
          batchItemSerialNos.Add(tmpBatchItemSerialNos);
          tmpBatchItemSerialNos = new List<WcsApiOutWarehouseReceiptItemSerialModel>();
          paraCount = 2;
        }

        paraCount += 1 + itemSerialNo.SerialNos.Count; // 3 itemCode
        tmpBatchItemSerialNos.Add(itemSerialNo);

      }
      if (tmpBatchItemSerialNos.Count > 0)
        batchItemSerialNos.Add(tmpBatchItemSerialNos);

      foreach (var batchItemSerialNo in batchItemSerialNos)
      {
        var para = new List<SqlParameter>
        {
          new SqlParameter("@p0", SqlDbType.VarChar) { Value = gupCode },
          new SqlParameter("@p1", SqlDbType.VarChar) { Value = custCode }
        };

        // and ((ITEM_CODE=@p0 and SERIAL_NO in (@p0x)) or (ITEM_CODE=@p1 and SERIAL_NO in(@p1x) )
        string sql2 = "(";
        var sql2List = new List<string>();
        foreach (var item in batchItemSerialNo)
        {
          var itemParaCnt = para.Count;
          para.Add(new SqlParameter($"@p{itemParaCnt}", SqlDbType.VarChar) { Value = item.ItemCode });
          sql2List.Add($"(ITEM_CODE=@p{itemParaCnt} {para.CombineSqlInParameters("AND SERIAL_NO", item.SerialNos, SqlDbType.VarChar)})");
        }

        sql2 += string.Join(" OR ", sql2List) + ")";

        var sql = $@"SELECT SERIAL_NO FROM F2501 WHERE GUP_CODE=@p0 AND CUST_CODE=@p1 AND {sql2}";

        res.AddRange(SqlQueryWithSqlParameterSetDbType<string>(sql, para.ToArray()).ToList());
      }
      res = res.Distinct().ToList();

      return res.AsQueryable();

      #region 原LINQ
      /*
      return _db.F2501s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      itemSerialNos.Any(z => z.ItemCode == x.ITEM_CODE &&
      z.SerialNos.Contains(x.SERIAL_NO)));
      */
      #endregion
    }

  }
}
