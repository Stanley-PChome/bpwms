using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.UILib;

namespace CrystalReport.Test
{
	public class BaseTest
	{
		public BaseTest()
		{
			new Application();

			Wms3plSession.Set<GlobalInfo>(new GlobalInfo { GupCode = "01", GupName = "測試業主名稱", CustCode = "010001", CustName = "測試貨主名稱" });
			Wms3plSession.Set<UserInfo>(new UserInfo{ Account = "WMS", AccountName = "WMS"});
		}

		#region Connection
		public bool IsSecretePersonalData { get; set; }

		public string FunctionCode { get; set; }

		public T GetProxy<T>() where T : DataServiceContext
		{
			return ConfigurationHelper.GetProxy<T>(IsSecretePersonalData, FunctionCode);
		}
		public T GetProxyLongTermSchema<T>() where T : DataServiceContext
		{
			return ConfigurationHelper.GetProxyLongTermSchema<T>(IsSecretePersonalData, FunctionCode);
		}
		public T GetModifyQueryProxy<T>() where T : DataServiceContext
		{
			return ConfigurationHelper.GetProxy<T>(false, FunctionCode);
		}
		public T GetExProxy<T>() where T : DataServiceContext
		{
			return ConfigurationExHelper.GetExProxy<T>(IsSecretePersonalData, FunctionCode);
		}
		#endregion

		#region Preview

		/// <summary>
		/// 泛用報表顯示
		/// </summary>
		protected void CallReport<T>(DataTable data) where T : ReportClass, new ()
		{
			var report = new T();
			report.SetDataSource(data);
			CallReport(report);
		}
		protected void CallReport<T>(T report) where T : ReportClass
		{
			report.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
			var win = new Wms3plViewer();
			win.CallReport(report, PrintType.Preview);
		}

		#endregion

	}
}
