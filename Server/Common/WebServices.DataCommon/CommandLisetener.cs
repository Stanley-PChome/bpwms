using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
    public class CommandLisetener
    {
        public ILogger SqlLogger { get; set; }
        public CommandLisetener()
        {

        }


        [DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuting")]
        public void OnCommandExecuting(DbCommand command, DbCommandMethod executeMethod, Guid commandId, Guid connectionId, bool async, DateTimeOffset startTime)
        {
            SqlLogger.LogError($"CommandExecuting\r\nCommand: {command.CommandText}\r\nParameters: {GetParametersLogString(command)}");
        }

        [DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")]
        public void OnCommandExecuted(object result, bool async)
        {
            //var r = result as Microsoft.EntityFrameworkCore.Storage.RelationalDataReader;
            //if (r != null)
            //{
            //	var commandText = r.DbCommand.CommandText;
            //}
        }

        [DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandError")]
        public void OnCommandError(Exception exception, bool async)
        {
            SqlLogger.LogError(exception, "Execute Sql Exception\r\n");

        }


        private string GetParametersLogString(DbCommand command)
        {
            var parms = new List<string>();
            foreach (DbParameter param in command.Parameters)
            {
                parms.Add($"{param.ParameterName} = {param.Value}");
            }

            return string.Join(" ; ", parms);
        }
    }
}
