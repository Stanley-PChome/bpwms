using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.UILib
{
	class Wms3plContainer
	{
		private readonly Function _function;

		public Wms3plContainer(Function function, bool enableLog = true)
		{
			_function = function;
			if (enableLog)
				LogUsage("New");
		}

		/// <summary>
		/// 檢查該登入者是否有權限使用 function
		/// </summary>
		/// <returns></returns>
		public bool CheckPermission()
		{
			//使用者可以用的功能
			var functions = Wms3plSession.Get<IEnumerable<Function>>();

			bool userCanExecute = functions.Any(f => f.Id == _function.Id);
			return userCanExecute;
		}

		public void LogUsage(string message)
		{
			var account = Wms3plSession.Get<UserInfo>().Account;
			var entry = new LogEntry
			{
				Message = message,
				Categories = new[] { "Usage" }
			};
			if (_function != null)
			{
				entry.ExtendedProperties.Add("FunctionId", _function.Id);
				entry.ExtendedProperties.Add("FunctionName", _function.Name);
			}
			entry.ExtendedProperties.Add("Account", account);
			entry.ExtendedProperties.Add("Type", this.GetType().Name);
			Logger.Write(entry);

			Task.Run(() =>
			{
				try
				{
					var proxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "LogUsage");
					proxy.InsertF0050(message, _function.Id, _function.Name);
				}
				catch { }
			});
			
		}


	}
}
