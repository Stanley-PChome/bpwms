using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices
{
  public class Global : System.Web.HttpApplication
  {

    void Application_Start(object sender, EventArgs e)
    {
			DbContextOptionsBuilder dbOptBuilder = new DbContextOptionsBuilder();
			var connStr = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["Schema"]].ConnectionString;
			dbOptBuilder = dbOptBuilder.UseSqlServer(connStr);

			var ctx = new Wms3plDbContext(dbOptBuilder.Options);
			var listener = ctx.GetService<DiagnosticSource>();
			var commandLisetener = new CommandLisetener();
			commandLisetener.SqlLogger = new WmsLogger();
			(listener as DiagnosticListener).SubscribeWithAdapter(commandLisetener);
		}

    void Application_End(object sender, EventArgs e)
    {
      //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
      // Code that runs when an unhandled error occurs

    }

		void Session_Start(object sender, EventArgs e)
    {
      // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e)
    {
      // Code that runs when a session ends. 
      // Note: The Session_End event is raised only when the sessionstate mode
      // is set to InProc in the Web.config file. If session mode is set to StateServer 
      // or SQLServer, the event is not raised.

    }



		protected void Application_BeginRequest(object sender, System.EventArgs e)
		{
			var cultureInfo = new CultureInfo(Current.Lang);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			Thread.CurrentThread.CurrentCulture = cultureInfo;
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
            var message = formatter(state, exception);
            Logger.Write(message, "Sql");
        }
    }
}
