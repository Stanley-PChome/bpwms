using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F00;

namespace Wms3pl.WebServices.Schedule.S99.Services
{
	public partial class S9901Service
	{
		private WmsTransaction _wmsTransaction;
		public S9901Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}


		#region 排程-訊息發送
		public IQueryable<WMSMessage> GetWmsMessages()
		{
			var rep = new F0080Repository(Schemas.CoreSchema, _wmsTransaction);
			var loMessages = rep.GetWMSMessages().ToList();
			var dcWmsMessages = loMessages.Where(a => a.TARGET_TYPE == "0");
			var custWmsMessages = loMessages.Where(a => a.TARGET_TYPE == "1");
			var allocWmsMessages = loMessages.Where(a => a.TARGET_TYPE == "2");

			#region DC
			//依貨主群組化資料
			var gDcLoMessages = dcWmsMessages.GroupBy(a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE });
			var f190003Rep = new F190003Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1924Rep = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var gDcLoMessage in gDcLoMessages)
			{
				var workNos = gDcLoMessage.Select(a => a.TARGET_CODE).ToList();
				//取出訊息通知設定
				var f190003s = f190003Rep.InWithTrueAndCondition("WORK_NO", workNos, a => a.DC_CODE == gDcLoMessage.Key.DC_CODE && a.GUP_CODE == gDcLoMessage.Key.GUP_CODE && a.CUST_CODE == gDcLoMessage.Key.CUST_CODE).ToList();
				//依天數群組化資料
				var gDayDcLoMessages = gDcLoMessage.GroupBy(g => new { Days = (!g.DAYS.HasValue || g.DAYS.Value < 7) ? 0 : ((g.DAYS.Value >= 7 && g.DAYS.Value < 14) ? 7 : 14) });
				var grpInfos = (from a in new List<object>()
												select new { WorkNo = "", GrpId = (decimal?)0, IsMail = false, IsSms = false }).ToList();
				foreach (var gDayDcLoMessage in gDayDcLoMessages)
				{
					//依不同天數，取得該天數需通知的工作群組
					if (gDayDcLoMessage.Key.Days == 0)
					{
						grpInfos = (from a in f190003s
												join b in gDayDcLoMessage on a.WORK_NO equals b.TARGET_CODE
												select new { WorkNo = a.WORK_NO, GrpId = a.GRP_ID_1, IsMail = a.IS_MAIL_1 == "1", IsSms = a.IS_SMS_1 == "1" }).ToList();
					}
					else if (gDayDcLoMessage.Key.Days == 7)
					{
						grpInfos = (from a in f190003s
												join b in gDayDcLoMessage on a.WORK_NO equals b.TARGET_CODE
												select new { WorkNo = a.WORK_NO, GrpId = a.GRP_ID_7, IsMail = a.IS_MAIL_7 == "1", IsSms = a.IS_SMS_7 == "1" }).ToList();
					}
					else if (gDayDcLoMessage.Key.Days == 14)
					{
						grpInfos = (from a in f190003s
												join b in gDayDcLoMessage on a.WORK_NO equals b.TARGET_CODE
												select new { WorkNo = a.WORK_NO, GrpId = a.GRP_ID_14, IsMail = a.IS_MAIL_14 == "1", IsSms = a.IS_SMS_14 == "1" }).ToList();
					}
					var grpIds = grpInfos.Where(a => a.GrpId.HasValue).Select(a => a.GrpId.Value).Distinct().ToList();
					//依工作群組取得要發送Email及Mobile
					var grpMailMobiles = f1924Rep.GetActiveDatas(grpIds);
					foreach (var grpInfo in grpInfos)
					{
						var tmpDcLoMessages = gDayDcLoMessage.Where(a => a.TARGET_CODE == grpInfo.WorkNo).ToList();
						var tmpGrpMailMobiles = grpMailMobiles.Where(a => a.GRP_ID == grpInfo.GrpId).ToList();
						foreach (var dcLoMessage in tmpDcLoMessages)
						{
							dcLoMessage.IsMail = grpInfo.IsMail;
							dcLoMessage.IsSms = grpInfo.IsSms;
							dcLoMessage.ReceiverMails = tmpGrpMailMobiles.Select(a => a.EMAIL).ToList();
							dcLoMessage.ReceiverMobiles = tmpGrpMailMobiles.Select(a => a.MOBILE).ToList();
						}
					}
				}

			}
			#endregion DC

			#region 貨主
			var f1909Rep = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var custCodes = custWmsMessages.Select(a => a.TARGET_CODE).ToList();
			var f1909s = f1909Rep.InWithTrueAndCondition("CUST_CODE", custCodes);
			foreach (var f1909 in f1909s)
			{
				var tmpCustLoMessages = custWmsMessages.Where(a => a.TARGET_CODE == f1909.CUST_CODE);
				foreach (var custLoMessage in tmpCustLoMessages)
				{
					custLoMessage.IsMail = true;
					custLoMessage.IsSms = false; //貨主一律發Mail
					custLoMessage.ReceiverMails = f1909.ITEM_MAIL.Split(',').ToList();
				}
			}
			#endregion 貨主

			#region 配送商 (目前無配送商聯絡資料，此段先不做)
			//var f1947Rep = new F1947Repository(Schemas.CoreSchema, _wmsTransaction);
			//var allIds = allocLoMessages.Select(a => a.TARGET_CODE).ToList();
			//var f1947s = f1947Rep.InWithTrueAndCondition("ALL_ID", allIds);
			//foreach (var f1947 in f1947s)
			//{
			//	var tmpAllocLoMessages = allocLoMessages.Where(a => a.TARGET_CODE == f1947.ALL_ID);
			//	foreach (var allocLoMessage in tmpAllocLoMessages)
			//	{
			//		allocLoMessage.IsMail = true;
			//		allocLoMessage.IsSms = false;
			//		//allocLoMessage.ReceiverMails = f1947.ITEM_MAIL.Split(',').ToList();
			//	}
			//}
			#endregion 配送商

			return loMessages.AsQueryable();
		}

		public void SetMessageStatus(List<WMSMessage> wmsMessages)
		{
			var wmsMessageIds_0 = wmsMessages.Where(a => string.IsNullOrEmpty(a.STATUS) || a.STATUS == "00").Select(a => a.MESSAGE_ID).ToList();
			var wmsMessageIds_7 = wmsMessages.Where(a => a.STATUS == "01").Select(a => a.MESSAGE_ID).ToList();
			var wmsMessageIds_14 = wmsMessages.Where(a => a.STATUS == "02").Select(a => a.MESSAGE_ID).ToList();
			var rep = new F0080Repository(Schemas.CoreSchema, _wmsTransaction);
			rep.UpdatetStatus(wmsMessageIds_0, "01");
			rep.UpdatetStatus(wmsMessageIds_7, "02");
			rep.UpdatetStatus(wmsMessageIds_14, "03");
		}
		#endregion 排程-訊息發送

	}
}
