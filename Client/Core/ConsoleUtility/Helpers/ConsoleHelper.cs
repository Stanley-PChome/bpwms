using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Wms3pl.Common.Extensions;

namespace ConsoleUtility.Helpers
{
	public static class ConsoleHelper
	{
		public static char MailWrap { get { return (char)10; } }
		public static string TypeName { get; set; }
		public static string FilePath { get; set; }

		public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ExceptionPolicy.HandleException(e.ExceptionObject as Exception, "Default Policy");
			Environment.Exit(1);
		}

		/// <summary>
		/// Console傳入參數轉換
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args">傳入Main的Args</param>
		/// <param name="argumentData">指定存放的對應物件</param>
		public static void ArgumentsTransform<T>(string[] args, T argumentData) where T : class
		{
			if (args.Any())
			{
				var argList = new NameValueCollection();
				var argArray = args.Select(arg => (arg.TrimStart('-')).Split('='));
				foreach (var a in argArray)
				{
					argList.Add(a[0], a[1]);
				}
				var classType = argumentData.GetType();
				foreach (var p in classType.GetProperties())
				{
					var value = argList[p.Name];
					if (!string.IsNullOrEmpty(argList[p.Name]))
						TypeUtility.SetNewValue(argumentData, value.DbNullToNull(), p);
					//classType.GetProperty(p.Name).SetValue(argumentData, System.Convert.ChangeType(argList[p.Name], p.PropertyType));
				}
				//foreach (var x in classType.GetProperties())
				//{
				//	if (!string.IsNullOrEmpty(argList[x.Name]))
				//	{
				//		switch (x.GetType().Name)
				//		{
				//			case "System.DateTime":
				//				classType.GetProperty(x.Name).SetValue(argumentData, DateTime.Parse(argList[x.Name]));
				//				break;
				//			default:
				//				classType.GetProperty(x.Name).SetValue(argumentData, argList[x.Name]);
				//				break;
				//		}
				//	}
				//}
			}
		}
		private static object DbNullToNull(this object original)
		{
			return original == DBNull.Value ? null : original;
		}

		public static void Log(string message, bool isShowDatetime = true)
		{
			var nameSpace = new StackTrace(true).GetFrame(1).GetMethod().DeclaringType.Namespace.Split('.').Last();
			//var fileFullName = Path.Combine(FilePath, $"{nameSpace}_{TypeName}.txt");
			var fileFullName = Path.Combine(FilePath, string.Format("{0}{1}.txt", nameSpace, string.IsNullOrWhiteSpace(TypeName) ? string.Empty : "_"+ TypeName.ToString()));
			if (!Directory.Exists(FilePath))
				Directory.CreateDirectory(FilePath);
			using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
				sw.WriteLine(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty, message));
		}

		
	}
}
