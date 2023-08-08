using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F056001Repository : RepositoryBase<F056001, Wms3plDbContext, F056001Repository>
	{
		public F056001Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<CartonWorkStationFloorInfo> GetCartonWorkStationFloor()
		{
			List<SqlParameter> param = new List<SqlParameter>();

			var sql = @"SELECT DISTINCT DC_CODE DcCode, FLOOR FloorCode, FLOOR FloorName FROM F056001";

			return SqlQuery<CartonWorkStationFloorInfo>(sql, param.ToArray());
		}

    /// <summary>
    /// 取得系統設定的建議箱號，並根據-符號後的數字去排序
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="WorkStationId"></param>
    /// <returns></returns>
    public IQueryable<CloseShipSysBox> GetCloseShipSysBox(string dcCode, string gupCode, string custCode, string WorkStationId)
    {
      List<SqlParameter> param = new List<SqlParameter>()
      {
        new SqlParameter("@p0" ,dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1" ,gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2" ,custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3" ,WorkStationId) { SqlDbType = SqlDbType.VarChar },

      };

      var sql = @"SELECT BOX_CODE,0 OrderByValue FROM F056001 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND WORKSTATION_CODE=@p3";
      var result = SqlQuery<CloseShipSysBox>(sql, param.ToArray()).ToList();
      foreach (var x in result)
      {
        var SplitValues = x.BOX_CODE.Split('-');
        int OrderByValue = 0;
        if (SplitValues.Count() > 1)
        {
          if (int.TryParse(SplitValues[1], out OrderByValue))
            x.OrderByValue = OrderByValue;
          else
            //如果沒半法轉成數字就把排序排到最後
            x.OrderByValue = int.MaxValue;
        }
        else
          x.OrderByValue = int.MaxValue;
      }
      result = result.OrderBy(x => x.OrderByValue).ThenBy(x => x.BOX_CODE).ToList();
      return result.AsQueryable();

    }
  }
}
