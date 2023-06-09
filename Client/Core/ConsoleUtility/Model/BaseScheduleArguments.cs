using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUtility.Model
{
	/// <summary>
	/// for 系統排程相關(檢查全部，而不指定檢查固定DC，客戶)
	/// </summary>
	public class BaseSystemScheduleArguments
	{
		public string SelectDate { get; set; }
	}

	/// <summary>
	/// for Edi Schedule使用
	/// </summary>
	public class BaseEdiScheduleArguments
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string SelectDate { get; set; }
	}



}
