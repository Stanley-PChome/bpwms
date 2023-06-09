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
    public partial class F020501Repository : RepositoryBase<F020501, Wms3plDbContext, F020501Repository>
    {
        public IQueryable<F020501> GetDataExduleF020501(F020501 f020501)
        {
            var parms = new List<object> { f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, f020501.CONTAINER_CODE, f020501.ID };

            string sql = $@" SELECT * FROM F020501 
                             WHERE DC_CODE = @p0
                             AND GUP_CODE = @p1
                             AND CUST_CODE = @p2
                             AND CONTAINER_CODE = @p3
                             AND ID <> @p4
                            ";

            return SqlQuery<F020501>(sql, parms.ToArray());
        }
        public F020501 GetDataByF0701Id(string dcCode, string gupCode, string custCode, long f0701Id)
        {
            var parms = new List<object> { dcCode, gupCode, custCode, f0701Id };
            string sql = @"
        					SELECT * 
                            FROM F020501
                            WHERE DC_CODE = @p0
                            AND GUP_CODE = @p1
                            AND CUST_CODE = @p2
                            AND F0701_ID = @p3
        					";

            return SqlQuery<F020501>(sql, parms.ToArray()).FirstOrDefault();
        }

        public long Insert(string dcCode, string gupCode, string custCode, string containerCode, long f0701Id, string pickWareId, string status, string allocationNo, string typeCode)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcCode));
            parm.Add(new SqlParameter("@p1", gupCode));
            parm.Add(new SqlParameter("@p2", custCode));
            parm.Add(new SqlParameter("@p3", containerCode));
            parm.Add(new SqlParameter("@p4", f0701Id));
            parm.Add(new SqlParameter("@p5", pickWareId));
            parm.Add(new SqlParameter("@p6", status));
            parm.Add(new SqlParameter("@p7", string.IsNullOrWhiteSpace(allocationNo) ? (object)DBNull.Value : allocationNo));
            parm.Add(new SqlParameter("@p8", Current.Staff));
            parm.Add(new SqlParameter("@p9", Current.StaffName));
            parm.Add(new SqlParameter("@p10", typeCode));

            var sql = @"  DECLARE @a INT;
                            BEGIN TRAN
                            INSERT INTO F020501
                            (
                            DC_CODE,
                            GUP_CODE,
                            CUST_CODE,
                            CONTAINER_CODE,
                            F0701_ID,
                            PICK_WARE_ID,
                            STATUS,
                            ALLOCATION_NO,
                            CRT_DATE,
                            CRT_STAFF,
                            CRT_NAME,
                            TYPE_CODE) 
                            VALUES(
                            @p0,
                            @p1,
                            @p2,
                            @p3,
                            @p4,
                            @p5,
                            @p6,
                            @p7,
                            dbo.GetSysDate(),
                            @p8,
                            @p9,
                            @p10);
                            SELECT @a=CAST(current_value as int)
                            FROM sys.sequences  
                            WHERE name = 'SEQ_F020501_ID' ; 
                            select @a ID
                            COMMIT TRAN;";

            return SqlQuery<long>(sql, parm.ToArray()).Single();
        }

        public string LockF020501()
        {
            var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F020501';";
            return SqlQuery<string>(sql).FirstOrDefault();
        }

        public long GetF020501NextId()
        {
            var sql = @"SELECT NEXT VALUE FOR SEQ_F020501_ID";

            return SqlQuery<long>(sql).Single();
        }
        public IQueryable<ContainerDetailData> GetContainerDetail(string dcCode, string gupCode, string custCode, string f020501Id)
        {
            var sql = @"SELECT B.RT_NO,
						B.BIN_CODE,
						B.ITEM_CODE,
						(SELECT F1903.ITEM_NAME FROM F1903 WHERE F1903.ITEM_CODE = B.ITEM_CODE AND F1903.GUP_CODE = B.GUP_CODE AND F1903.CUST_CODE = B.CUST_CODE) ITEM_NAME,
						B.QTY,
						(SELECT F000904.NAME FROM F000904 WHERE F000904.TOPIC = 'F020502' AND F000904.SUBTOPIC = 'STATUS' AND F000904.VALUE = B.STATUS) STATUS
						FROM F020501 A
						JOIN F020502 B 
						ON A.ID  = B.F020501_ID 
						JOIN F0701 C 
						ON C.ID = A.F0701_ID
						WHERE A.DC_CODE =@p0
						AND A.GUP_CODE  = @p1
						AND A.CUST_CODE  = @p2
						AND A.ID  = @p3";
            var param = new object[] {
                dcCode,
                gupCode,
                custCode,
                f020501Id
            };

            var result = SqlQuery<ContainerDetailData>(sql, param);
            return result;
        }

        public IQueryable<BindContainerData> GetBindContainerData(string dcCode, string gupCode, string custCost, string RTNo, string RTSeq)
        {
            var sql =
@"SELECT a.ID F020501_ID,a.TYPE_CODE,altc.NAME TYPE_CODE_NAME,a.PICK_WARE_ID,a.PICK_WARE_ID + ' '  + c.WAREHOUSE_NAME PICK_WARE_NAME,a.CONTAINER_CODE,b.RT_NO,b.RT_SEQ
FROM F020501 a 
INNER JOIN F020502 b ON a.id=b.F020501_ID 
LEFT JOIN F1980 c ON a.DC_CODE=c.DC_CODE AND a.PICK_WARE_ID=c.WAREHOUSE_ID
LEFT JOIN VW_F000904_LANG alst ON alst.TOPIC='F020501' AND alst.SUBTOPIC='STATUS' AND alst.LANG=@p5 AND alst.VALUE=a.STATUS
LEFT JOIN VW_F000904_LANG altc ON altc.TOPIC='F0205' AND altc.SUBTOPIC='TYPE_CODE' AND altc.LANG=@p5 AND altc.VALUE=a.TYPE_CODE
WHERE a.DC_CODE=@p0 AND a.GUP_CODE=@p1 AND a.CUST_CODE=@p2 AND b.RT_NO=@p3 AND b.RT_SEQ=@p4 AND a.STATUS='0'
GROUP BY a.ID,a.TYPE_CODE,altc.NAME,a.PICK_WARE_ID,c.WAREHOUSE_NAME,a.CONTAINER_CODE,b.RT_NO,b.RT_SEQ";

            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCost},
                new SqlParameter("@p3",SqlDbType.VarChar){Value=RTNo},
                new SqlParameter("@p4",SqlDbType.VarChar){Value=RTSeq},
                new SqlParameter("@p5",SqlDbType.VarChar){Value=Current.Lang}
            };
            return SqlQuery<BindContainerData>(sql, para.ToArray());
        }


        public IQueryable<AreaContainerData> GetAreaContainerData(string dcCode, string gupCode, string custCode, string typeCode, string RTNo, string RTSeq)
        {
            var sql =
                @"SELECT a.CONTAINER_CODE MCONTAINER_CODE,a.TYPE_CODE,b.* FROM F020501 a INNER JOIN F020502 b ON a.ID=b.F020501_ID
WHERE a.DC_CODE=@p0 AND a.GUP_CODE=@p1 AND a.CUST_CODE=@p2 AND a.TYPE_CODE=@p3 AND b.RT_NO=@p4 AND b.RT_SEQ=@p5";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("@p3",SqlDbType.VarChar){Value=typeCode},
                new SqlParameter("@p4",SqlDbType.VarChar){Value=RTNo},
                new SqlParameter("@p5",SqlDbType.VarChar){Value=RTSeq}
            };
            return SqlQuery<AreaContainerData>(sql, para.ToArray());
        }
    
		public IQueryable<AcceptanceContainerDetail> GetAcceptanceContainerDetail(string dcCode,string gupCode,string custCode,string rtNo)
		{
			var param = new List<SqlParameter> {
				new SqlParameter("@p0",Current.Lang),
				new SqlParameter("@p1",dcCode),
				new SqlParameter("@p2",gupCode),
				new SqlParameter("@p3",custCode),
				new SqlParameter("@p4",rtNo),
			};
			var sql = @"SELECT B.RT_NO ,A.CONTAINER_CODE ,B.BIN_CODE ,
						(SELECT NAME  FROM VW_F000904_LANG WHERE TOPIC='F020501' AND SUBTOPIC='STATUS' AND VALUE= A.STATUS AND LANG = @p0) F020501_STATUS,
						(SELECT WAREHOUSE_NAME  FROM F1980 WHERE DC_CODE=A.DC_CODE AND WAREHOUSE_ID=A.PICK_WARE_ID) PICK_WARE_NAME,
						A.ALLOCATION_NO ,
						B.ITEM_CODE ,
						(SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F0205' AND SUBTOPIC='TYPE_CODE' AND VALUE = A.TYPE_CODE) TYPE_CODE_NAME,
						B.QTY,
						(SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F020502' AND SUBTOPIC='STATUS' AND VALUE = B.STATUS and LANG = @p0) F020502_STATUS,
						(SELECT CAUSE FROM F1951 WHERE UCT_ID = 'IQ' AND UCC_CODE = B.RECHECK_CAUSE) RECHECK_CAUSE,
						B.RECHECK_MEMO,
            B.RCV_MEMO
						FROM F020501 A
						JOIN F020502 B 
						ON A.ID = B.F020501_ID 
						WHERE A.DC_CODE =@p1
						AND A.GUP_CODE = @p2
						AND A.CUST_CODE = @p3
						AND B.RT_NO = @p4";
			
			var result = SqlQuery<AcceptanceContainerDetail>(sql, param.ToArray());
			return result;
		}

		public F020501 GetDataById(long id)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",id){ SqlDbType = SqlDbType.BigInt}
			};
			var sql = @" SELECT *
                    FROM F020501 
                   WHERE ID = @p0";
			return SqlQuery<F020501>(sql, parms.ToArray()).FirstOrDefault();
		}

		public IQueryable<RtNoContainerStatus> GetRtNoContainerStatuses(string dcCode,string gupCode,string custCode,List<string> rtNoList,List<long> excludeF020501IdList)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sqlIn = parms.CombineSqlInParameters(" AND B.RT_NO", rtNoList);
			sqlIn += parms.CombineSqlNotInParameters("AND A.ID ", excludeF020501IdList);
			var sql = $@"SELECT B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.STOCK_NO,B.RT_NO,B.F020501_ID,A.STATUS F020501_STATUS,A.ALLOCATION_NO
										FROM F020501 A
										JOIN F020502 B
										ON B.F020501_ID = A.ID
										WHERE B.QTY > 0
										AND B.DC_CODE =@p0 
										AND B.GUP_CODE = @p1
										AND B.CUST_CODE =@p2
										{sqlIn}
										GROUP BY B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.STOCK_NO,B.RT_NO,B.F020501_ID,A.STATUS,A.ALLOCATION_NO ";
			return SqlQuery<RtNoContainerStatus>(sql, parms.ToArray());
		}
	}
}
