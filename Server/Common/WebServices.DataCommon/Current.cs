using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace Wms3pl.WebServices.DataCommon
{
	public class Current
	{
		private static IPrincipal User
		{
			get
			{
				if(HttpContext.Current!=null)
					return HttpContext.Current.User;
				return null;
			}
		}

		public static string DefaultStaff
		{
			get
			{
			 if(HttpContext.Current!=null)
				{
					var cookie = HttpContext.Current.Request.Cookies["DefaultStaff"];
					return (cookie == null) ? "System" : cookie.Value;
				}
				return "System";
			}
			set
			{
				if (HttpContext.Current != null)
				{
					var cookie = HttpContext.Current.Request.Cookies["DefaultStaff"];
					if (cookie == null)
					{
						cookie = new HttpCookie("DefaultStaff");
						HttpContext.Current.Request.Cookies.Add(cookie);
					}
					cookie.Value = value;
				}
			}
		}

		public static string DefaultStaffName
		{
			get
			{
				if (HttpContext.Current != null)
				{
					var cookie = HttpContext.Current.Request.Cookies["DefaultStaffName"];
					return (cookie == null) ? "System" : cookie.Value;
				}
				return "System";
			}
			set
			{
				if (HttpContext.Current != null)
				{
					var cookie = HttpContext.Current.Request.Cookies["DefaultStaffName"];
					if (cookie == null)
					{
						cookie = new HttpCookie("DefaultStaffName");
						HttpContext.Current.Request.Cookies.Add(cookie);
					}
					cookie.Value = value;
				}
			}
		}

		public static string Staff
		{
			get
			{
				if (User!=null && !string.IsNullOrEmpty(User.Identity.Name))
					return User.Identity.Name;
				else
					return DefaultStaff;
			}
		}


		private static string _staffName = "";
		public static string StaffName
		{
			get
			{
				_staffName = "";
				if (HttpContext.Current != null && HttpContext.Current.Request.Headers["userName"] != null)
					_staffName = HttpContext.Current.Request.Headers["userName"];
				if (string.IsNullOrEmpty(_staffName))
				{
					var headerIndex = -1;
					if (OperationContext.Current != null)
					{
						headerIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader("userName", "http://Wms3pl");
					}
					if (headerIndex < 0)
					{
						if (string.IsNullOrEmpty(_staffName))
							_staffName = DefaultStaffName;
						if (string.IsNullOrEmpty(_staffName))
							_staffName = DefaultStaff;
						return HttpUtility.UrlDecode(_staffName);
					}
					//var r = OperationContext.Current.IncomingMessageHeaders.GetReaderAtHeader(headerIndex).ReadSubtree();
					//var data = XElement.Load(r);
					//_staffName = (string)data;
				}
				if (string.IsNullOrEmpty(_staffName))
					_staffName = DefaultStaffName;
				if (string.IsNullOrEmpty(_staffName))
					_staffName = DefaultStaff;
				return HttpUtility.UrlDecode(_staffName);
			}
		}

		//private static string _deviceName = "N/A";
		//public static string DeviceName
		//{
		//	get
		//	{
		//		if (HttpContext.Current != null && HttpContext.Current.Request.Headers["deviceName"] != null)
		//			_deviceName = HttpContext.Current.Request.Headers["deviceName"];
		//		if (string.IsNullOrEmpty(_deviceName))
		//		{
		//			var headerIndex = -1;
		//			if (OperationContext.Current != null)
		//			{
		//				headerIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader("deviceName", "http://Wms3pl");
		//			}
		//			if (headerIndex < 0)
		//			{
		//				return _deviceName;
		//			}
		//			var r = OperationContext.Current.IncomingMessageHeaders.GetReaderAtHeader(headerIndex).ReadSubtree();
		//			var data = XElement.Load(r);
		//			_deviceName = (string)data;
		//		}
		//		return _deviceName;
		//	}
		//}

		private static string _deviceIp = "";
		public static string DeviceIp
		{
			get
			{
				_deviceIp = "";
				if (HttpContext.Current != null && HttpContext.Current.Request.Headers["clientIp"] != null)
					_deviceIp = HttpContext.Current.Request.Headers["clientIp"];
				if (string.IsNullOrEmpty(_deviceIp))
				{
					var headerIndex = -1;
					if (OperationContext.Current != null)
					{
						headerIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader("clientIp", "http://Wms3pl");
					}
					if (headerIndex < 0)
					{
						if (string.IsNullOrEmpty(_deviceIp))
							_deviceIp = "0.0.0.0";
						return HttpUtility.UrlDecode(_deviceIp);
					}
					var r = OperationContext.Current.IncomingMessageHeaders.GetReaderAtHeader(headerIndex).ReadSubtree();
					var data = XElement.Load(r);
					_deviceIp = (string)data;
				}
				if (string.IsNullOrEmpty(_deviceIp))
					_deviceIp = "0.0.0.0";
				return HttpUtility.UrlDecode(_deviceIp);
			}
		}
		public static bool IsNotSecretePersonalDataSys {
			get
			{
				if (HttpContext.Current != null)
				{
					var cookie = HttpContext.Current.Request.Cookies["IsNotSecretePersonalDataSys"];
					return (cookie == null) ? false : cookie.Value.ToUpper() == "TRUE";
				}
				return false;
			}
			set
			{
				if (HttpContext.Current != null)
				{
					var cookie = HttpContext.Current.Request.Cookies["IsNotSecretePersonalDataSys"];
					if (cookie == null)
					{
						cookie = new HttpCookie("IsNotSecretePersonalDataSys");
						HttpContext.Current.Request.Cookies.Add(cookie);
					}
					if (value)
						cookie.Value = "TRUE";
				}
			} 
		}
		public static bool IsSecretePersonalData
		{
			get
			{
				var isSecrete = "FALSE";
				if (HttpContext.Current != null)
				{
					isSecrete = HttpContext.Current.Request.Headers["isSecrete"];
					if (string.IsNullOrEmpty(isSecrete))
						return !IsNotSecretePersonalDataSys;
					else
						isSecrete = isSecrete.ToUpper();
				}
				return (isSecrete == "TRUE");
			}
		}
		public static string FunctionCode
		{
			get
			{
				var functionCode = "SystemFunction";
				if (HttpContext.Current != null)
				{
					functionCode = HttpContext.Current.Request.Headers["functionCode"];
					if (string.IsNullOrEmpty(functionCode))
						return "SystemFunction";				}
				return functionCode;
			}
		}

		public static string Lang
		{
			get
			{
				var lang = "zh-TW";
				if (HttpContext.Current != null)
				{
					lang = HttpContext.Current.Request.Headers["Lang"];
					if (string.IsNullOrEmpty(lang))
						return "zh-TW";
				}
				return lang;
			}
		}

		private static Dictionary<string, string> _schemas;
		public static Dictionary<string, string> Schemas
		{
			get
			{
				if (_schemas == null)
				{
					_schemas = new Dictionary<string, string>();
					for (var i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
					{
						var name = ConfigurationManager.ConnectionStrings[i].Name;
						var connStr = ConfigurationManager.ConnectionStrings[i].ConnectionString.ToUpper();
						if (string.IsNullOrEmpty(connStr)) continue;
						var schema = string.Empty;
						var sIdx = connStr.IndexOf("USER ID") + 7;
						if (sIdx < 7) continue;
						var eIdx = connStr.IndexOf(";", sIdx);
						if (eIdx != -1)
							schema = connStr.Substring(sIdx, eIdx - sIdx);
						else
							schema = connStr.Substring(sIdx);
						schema = schema.Replace("=", "").Trim();
						_schemas.Add(name, schema);
					}
				}
				return _schemas;
			}
		}

		public static bool IsAllowSameAccountMulitLogin()
		{
			return ConfigurationManager.AppSettings["IsAllowSameAccountMultiLogin"].ToString() == "1";
		}
	}
}
