using Microsoft.AspNet.SignalR.Hubs;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.SignalR
{
	public static class HubCallerContextExtenions
	{
		/// <summary>
		/// 取得已授權的 User Name，未授權則顯示 System
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string GetStaff(this HubCallerContext context)
		{
			var identity = context.User.Identity;

			if (!identity.IsAuthenticated || string.IsNullOrEmpty(identity.Name))
				return "System";    // 要與 DefaultStaff 相同

			return identity.Name;
		}
		/// <summary>
		/// 取得已授權的 User Name，未授權則顯示 System
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string GetStaffName(this HubCallerContext context)
		{
			if (context.Request.Headers != null && context.Request.Headers["userName"] != null)
				return context.Request.Headers["userName"];
			return "System";
		}

		public static string GetClientHostName(this HubCallerContext context)
		{

			if (context.Request.Headers != null && context.Request.Headers["clientIp"] != null)
				return context.Request.Headers["clientIp"];
			return "localPC";
		}

		public static string GetSchema(this HubCallerContext context)
		{
			if(context.Request.Headers!=null && context.Request.Headers["schema"] !=null)
			{
				return DbSchemaHelper.ChangeRealSchema(context.Request.Headers["schema"]);
			}
			return null;
		}
	}
}
