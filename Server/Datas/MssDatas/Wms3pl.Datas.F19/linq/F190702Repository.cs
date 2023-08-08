using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F190702Repository : RepositoryBase<F190702, Wms3plDbContext, F190702Repository>
  {
    private string _connName;
    public F190702Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
    {
      _connName = connName;
    }

    public Tuple<DataSet, string> GetQueryData(string sql, Dictionary<string, object> parameters)
    {
      foreach (var parameterName in parameters.Keys)
      {
        var parameterValue = parameters[parameterName];
        var variableName = string.Format("#@{0}#", parameterName);
        sql = sql.Replace(variableName, parameterValue == null ? string.Empty : parameterValue.ToString());

        #region (原始碼就存在的內容)
        //string convertString = null;
        //if (parameterValue is DateTime)
        //  convertString = ((DateTime)parameterValue).ToString("yyyy/MM/dd");
        //else
        //{
        //  convertString = parameterValue.ToString();
        //}

        //sql = sql.Replace(variableName, convertString); 
        #endregion
      }

      sql = sql.Trim();
      if (!sql.StartsWith("begin", StringComparison.OrdinalIgnoreCase))
        sql = sql.TrimEnd().TrimEnd(';');

      try
      {
        return new Tuple<DataSet, string>(ExecuteDataSetHepler.GetDataSet(_connName, sql), "");
      }
      catch (Exception ex)
      {
        return new Tuple<DataSet, string>(null, "執行FUN_SQL發生錯誤，" + ex.Message);
      }
    }

        public F190702 GetF190702DataByFunId(int funId)
        {
            var query = _db.F190702s
                            .Where(x => x.FUN_ID == funId);
            return query.Select(x => x).SingleOrDefault();
        }
    }
}
