using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class MessageService
	{
		private WmsTransaction _wmsTransaction;
		public MessageService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult SetWmsMessageReply(decimal messageId,DateTime? replyTime,string replyStatus)
		{
			var f0080Repo = new F0080Repository(Schemas.CoreSchema,_wmsTransaction);
			var f0080 = f0080Repo.Find(x => x.MESSAGE_ID == messageId);
			if (f0080 == null)
				return new ExecuteResult(false, "查無此訊息池 ID");
			f0080.REPLY_TIME = replyTime;
			f0080.STATUS = replyStatus;
			f0080Repo.Update(f0080);
			return new ExecuteResult(true, "設定訊息池狀態");
		}

		public ExecuteResult AddWmsMessage(string dcCode,string gupCode,string custCode,string msgNo, string messageContent,
		string targrtType, string targetCode, DateTime? sendTime, DateTime? expectReplyTime, decimal messageId)
		{
			var f0080Repo = new F0080Repository(Schemas.CoreSchema);
			var f0080 = new F0080
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				MSG_NO = msgNo,
				MESSAGE_ID = messageId,
				MEAAGE_CONTENT = messageContent,
				TARGET_TYPE = targrtType,
				TARGET_CODE = targetCode,
				SEND_TIME = sendTime,
				EXPECT_REPLY_TIME = expectReplyTime,
				STATUS ="00"
			};
			f0080Repo.Add(f0080);
			return new ExecuteResult(true, "新增訊息池成功");
		}

	}
}
