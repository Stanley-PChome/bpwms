using System;
using System.Data.Objects;
using System.Web;
//using EFTracingProvider;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.WebServices.DataCommon
{
	public class ModelHelper<T> where T : DbContext, new()
	{
		private static object _theObj = new object();
		public static T GetDbContext()
		{
			T context;
			try
			{
				lock (_theObj)
				{
					context = new T();
				}
			}
			catch (Exception exception)
			{
				ExceptionPolicy.HandleException(exception, "Default");
				throw;
			}


			RegisterLogging(context);
			return context;
		}

		public static void RegisterLogging(T context)
		{
			var beforeExecuteTime = DateTime.Now;

			//context.CommandExecuting += (sender, e) =>
			//{
			//	var traceString = e.ToTraceString();
			//	try
			//	{
			//		WmsSqlLogHelper.Log(context, traceString, beforeExecuteTime, false);
			//	}
			//	catch (Exception ex)
			//	{
			//		#region 寫入LOG到文字檔
			//		var entry = new LogEntry()
			//		{
			//			Message = string.Format("Command is executing:\n{0}", traceString),
			//			Categories = new string[] { "Sql" }
			//		};
			//		Logger.Write(entry);
			//		#endregion
			//		throw new Exception(ex.Message, ex);
			//	}
			//};

			//context.CommandFinished += (sender, e) => { LogSql(e); };

			//var objectContext = ((IObjectContextAdapter)context).ObjectContext;
			//objectContext.SavingChanges += new System.EventHandler(context_SavingChanges);
		}



		static void context_SavingChanges(object sender, System.EventArgs e)
		{
			////新增
			//foreach (ObjectStateEntry entry in
			//			((ObjectContext)sender).ObjectStateManager.GetObjectStateEntries(EntityState.Added))
			//{
			//	if (!entry.IsRelationship)
			//	{
			//		var entityType = entry.Entity.GetType();
			//		var propertyInfos = entityType.GetProperties();
			//		if (propertyInfos.Count(p => p.Name == "CRT_DATE") > 0)
			//		{
			//			var propertyInfo = entry.Entity.GetType().GetProperty("CRT_DATE");

			//			//2013.10.03 更新 如果有帶值，則不給 Default 值
			//			var data = propertyInfo.GetValue(entry.Entity, null);
			//			if (data == null || DateTime.Parse(data.ToString()).ToString("yyyy/MM/dd") == "0001/01/01")
			//				propertyInfo.SetValue(entry.Entity, DateTime.Now, null);
			//		}

			//		if ((HttpContext.Current != null) && (HttpContext.Current.User != null))
			//		{
			//			var account = HttpContext.Current.User.Identity.Name;
			//			if (!string.IsNullOrEmpty(account) &&
			//				propertyInfos.Count(p => p.Name == "CRT_STAFF") > 0)
			//			{
			//				var propertyInfo = entry.Entity.GetType().GetProperty("CRT_STAFF");
			//				propertyInfo.SetValue(entry.Entity, account, null);
			//			}
			//		}
			//	}
			//}

			////更新
			//foreach (ObjectStateEntry entry in
			//			((ObjectContext)sender).ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
			//{
			//	if (!entry.IsRelationship)
			//	{
			//		var entityType = entry.Entity.GetType();
			//		var propertyInfos = entityType.GetProperties();

			//		if (propertyInfos.Count(p => p.Name == "UPD_DATE") > 0)
			//		{
			//			var propertyInfo = entry.Entity.GetType().GetProperty("UPD_DATE");

			//			//2013.10.03 更新 如果有帶值，則不給 Default 值
			//			var data = propertyInfo.GetValue(entry.Entity, null);
			//			if (data == null || DateTime.Parse(data.ToString()).ToString("yyyy/MM/dd") == "0001/01/01")
			//				propertyInfo.SetValue(entry.Entity, DateTime.Now, null);
			//		}

			//		if ((HttpContext.Current != null) && (HttpContext.Current.User != null))
			//		{
			//			var account = HttpContext.Current.User.Identity.Name;
			//			if (!string.IsNullOrEmpty(account) && propertyInfos.Count(p => p.Name == "UPD_STAFF") > 0)
			//			{
			//				var propertyInfo = entry.Entity.GetType().GetProperty("UPD_STAFF");
			//				propertyInfo.SetValue(entry.Entity, account, null);
			//			}
			//		}
			//	}
			//}

		}
	}
}
