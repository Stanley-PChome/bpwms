using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Wms3pl.WebServices.DataCommon
{
  public class ExecuteDataSetHepler
  {
    public static DataSet GetDataSet(string connName, string sql)
    {
      try
      {
        var db = DatabaseFactory.CreateDatabase(connName);
        var command = db.GetSqlStringCommand(sql);
        var dataSet = db.ExecuteDataSet(command);
        return dataSet;
      }
      finally
      {
        LogSql(sql, null);
      }
    }

    private static void LogSql(string sql, params object[] parameters)
    {
      try
      {
        if (parameters != null)
        {
          var entry = new LogEntry()
          {
            Message =
              string.Format("Command is executing:\n{0}\nParameters:{1}", sql, string.Join(",", parameters)),
            Categories = new string[] { "Sql" }
          };
          Logger.Write(entry);
        }
        else
        {
          var entry = new LogEntry()
          {
            Message =
              string.Format("Command is executing:\n{0}", sql),
            Categories = new string[] { "Sql" }
          };
          Logger.Write(entry);
        }
      }
      catch
      {
      }
    }
  }
}
