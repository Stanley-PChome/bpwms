using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;

namespace Wms3pl.WpfClient.P19.Services
{
	//public static class FunctionStatus {
	//	public static string UnPublished = "2";
	//	public static string UnAuthorized = "0";
	//	public static string Authorized = "1";
	//	public static string Modified = "3";
	//	public static string Disabled = "9";
	//	public static string All = "";
	//}

	public static class P190502Datas
	{
		public static List<NameValuePair<string>> Functions
		{
			get
			{
				return new List<NameValuePair<string>>() {
					new NameValuePair<string>() {Name=Properties.Resources.Pmodel, Value="P"},
					new NameValuePair<string>() {Name=Properties.Resources.Tmodel, Value="T"},
					new NameValuePair<string>() {Name=Properties.Resources.Amodel, Value="A"},
					new NameValuePair<string>() {Name=Properties.Resources.Smodel, Value="S"},
					new NameValuePair<string>() {Name=Properties.Resources.Rmodel, Value="R"}
				};
			}
		}

		public static List<NameValuePair<string>> Stratum
		{
			get
			{
				return new List<NameValuePair<string>>() {
					new NameValuePair<string>() {Name=Properties.Resources.Stage1, Value="1"},
					new NameValuePair<string>() {Name=Properties.Resources.Stage2, Value="2"},
					new NameValuePair<string>() {Name=Properties.Resources.Stage3, Value="3"}
				};
			}
		}
	}
}
