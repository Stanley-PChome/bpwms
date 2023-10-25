using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test
{
	public class BaseRepositoryTest
	{
		public BaseRepositoryTest()
		{
			Schemas.CoreSchema = ConfigurationManager.AppSettings["CoreSchema"];

			DbContextOptionsBuilder dbOptBuilder = new DbContextOptionsBuilder();
			var connStr = ConfigurationManager.ConnectionStrings[Schemas.CoreSchema].ConnectionString;
			dbOptBuilder = dbOptBuilder.UseSqlServer(connStr);
			var ctx = new Wms3plDbContext(dbOptBuilder.Options);
			var listener = ctx.GetService<DiagnosticSource>();
			var commandLisetener = new CommandLisetener();
			commandLisetener.SqlLogger = new WmsLogger();

			(listener as DiagnosticListener).SubscribeWithAdapter(commandLisetener);
		}

    protected void Output(object obj)
    {
      Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
    }

  }


  public class WmsLogger : ILogger
	{
		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
		}
	}
}
