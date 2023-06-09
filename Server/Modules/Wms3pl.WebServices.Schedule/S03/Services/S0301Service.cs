using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.S03.Services
{
	public partial class S0301Service
	{
		private WmsTransaction _wmsTransaction;
		public S0301Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 排程-信件格式

		public F0020 GetF0020(string msgNo)
		{
			var repoF0020 = new F0020Repository(Schemas.CoreSchema, _wmsTransaction);

			return repoF0020.Find(x => x.MSG_NO == msgNo);
		}

		#endregion

		#region 排程-同電腦包裝的出貨單，檢查第一箱=建議箱

		/// <summary>
		/// 根據材積去取得建議箱(品號)
		/// </summary>
		public string GetBoxNum(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var result = string.Empty;
			decimal unitSize = 0;

			var repoF0003 = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF050802 = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1903 = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1905 = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);

			var f0003s = repoF0003.GetF0003(dcCode, gupCode, custCode);
			var f050802s = repoF050802.GetF050802s(dcCode, gupCode, custCode, wmsOrdNo);

			// 商品材積
			foreach (var p in f050802s)
			{
				var tmp = repoF1905.Find(x => x.GUP_CODE == p.GUP_CODE && x.CUST_CODE == p.CUST_CODE  && x.ITEM_CODE == p.ITEM_CODE);
				if (tmp != null)
					unitSize += tmp.PACK_HIGHT * tmp.PACK_LENGTH * tmp.PACK_WIDTH * (p.B_DELV_QTY ?? 0);
			}

			// 取得預設容積率
			var tmpBoxRate = f0003s.FirstOrDefault(x => x.AP_NAME == "BoxRate");
			decimal boxRate = 1;
			if (!decimal.TryParse(tmpBoxRate.SYS_PATH, out boxRate)) boxRate = 1;

			// 取得符合的紙箱大小
			var f1905s = repoF1905.GetCartonSize(gupCode, custCode, null);
			var tmpBox = f1905s.Where(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH * boxRate >= unitSize)
				.OrderBy(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH).FirstOrDefault();
			// 找不到，表示超材，取最大紙箱
			if (tmpBox == null)
				tmpBox = f1905s.OrderByDescending(x => x.PACK_HIGHT * x.PACK_LENGTH * x.PACK_WIDTH).FirstOrDefault();

			if (tmpBox != null)
			{
				var tmpBoxItem = repoF1903.Find(x => x.ITEM_CODE == tmpBox.ITEM_CODE && x.GUP_CODE == tmpBox.GUP_CODE && x.CUST_CODE == tmpBox.CUST_CODE);
				result = (tmpBoxItem != null) ? tmpBoxItem.ITEM_CODE : string.Empty;
			}

			return result;
		}

		#endregion

		#region 排程-檢查進倉、退貨、出貨、調撥、盤點單，是否有單據預設狀態為預設值，且超過一小時

		public IQueryable<OrderIsProblem> GetOrderIsProblem(DateTime selectDate)
		{
			var repoF010201 = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);

			return repoF010201.GetOrderIsProblem(selectDate).AsQueryable();

		}

		#endregion

		#region 排程-檢查撿貨時間過長(超過F0003設定)

		public IQueryable<ExceedPickFinishTime> GetExceedPickFinishTimeDatas(DateTime selectDate)
		{
			var repoF051201 = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);

			return repoF051201.GetExceedPickFinishTimeDatas(selectDate).AsQueryable();

		}

		#endregion

		#region 行事歷寫至訊息池
		public ExecuteResult InsertSchMessage()
		{
			Current.DefaultStaff = "Schedule";
			Current.DefaultStaffName = "Schedule";
			var shareService = new SharedService(_wmsTransaction);
			var messageService = new MessageService(_wmsTransaction);
			var repo = new F700501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700501Data = repo.GetF700501ForMessageData();
			foreach(var item in f700501Data)
			{
				var messageId = SharedService.GetTableSeqId("SEQ_MESSAGE_ID");
				var result = messageService.AddWmsMessage(item.DC_CODE, "", "", "", item.CONTENT, "0", "", null, null, messageId);
				if (result.IsSuccessed)
					repo.UpdateF700501MessageId(item.DC_CODE, item.SCHEDULE_NO, messageId);
			}
			return new ExecuteResult(true);
		}

		#endregion
	}
}
