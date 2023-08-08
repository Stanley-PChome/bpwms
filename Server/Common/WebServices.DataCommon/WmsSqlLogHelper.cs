using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Wms3pl.WebServices.DataCommon
{
	public static class WmsSqlLogHelper
	{
		public static void Log(DbContext context, string sqlWithParams, DateTime beforeExecuteTime, bool isException, char preDbParamSymbol)
		{
			// 遇到transationscope 會變成分散式交易，造成連線關不掉，先註解掉
//#if DEBUG
//			var sql = $@"INSERT INTO F005001 (MACHINE,
//											 SQLSTR,
//											 FUN_ID,
//											 CRT_STAFF,
//											 CRT_NAME,
//											 CRT_DATE,
//											 UPD_DATE,
//											 UPD_STAFF
//											 )
//							 VALUES ( {preDbParamSymbol}p0,
//									  {preDbParamSymbol}p1,
//									  {preDbParamSymbol}p2,
//									  {preDbParamSymbol}p3,
//									  {preDbParamSymbol}p4,
//									  {preDbParamSymbol}p5,
//									  {preDbParamSymbol}p6,
//									  {preDbParamSymbol}p7)";

//			var parameters = new object[] { Current.DeviceIp,
//											sqlWithParams,
//											Current.FunctionCode,
//											Current.Staff,
//											Current.StaffName,
//											beforeExecuteTime,
//											DateTime.Now,
//											isException ? "1" : null };

//			//using (var ts = new TransactionScope(TransactionScopeOption.Suppress))
//			//{
//			context.Database.ExecuteSqlCommand(sql, parameters.ToArray());
//			//}
//#endif
		}
	}
}
