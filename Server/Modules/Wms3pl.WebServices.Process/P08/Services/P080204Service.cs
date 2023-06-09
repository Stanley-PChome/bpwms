
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using System.Data.Objects;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080204Service
	{
		private WmsTransaction _wmsTransaction;
		public P080204Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<ExecuteResult> AddReturnCheck(string dcCode, string consignee, string receiptDate, string transport, string carNo, string returnNo, string pastNo, string barCode)
		{
			List<ExecuteResult> results = new List<ExecuteResult>();
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var f161301repo = new F161301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161302repo = new F161302Repository(Schemas.CoreSchema, _wmsTransaction);

			var _receiptDate = Convert.ToDateTime(receiptDate);

			//0. 檢核是否存在F161301及F161302
			var f161301CheckExist = f161301repo.Find(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode) && x.CONSIGNEE == consignee &&
																										x.RECEIPT_DATE == _receiptDate &&
																										x.TRANSPORT == transport && x.CAR_NO == carNo);

			var f161302CheckExist = f161302repo.GetCheckExist(dcCode, returnNo, pastNo, barCode)
															.AsQueryable()
															.ToList();

			if (f161302CheckExist != null && f161302CheckExist.Any())
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P080204Service_ReturnNoAndPastNoRepeat;
				results.Add(result);
				return results.AsQueryable();
			}

			string RtnCheckNo = (f161301CheckExist == null) ? DateTime.Now.ToString("yyyyMMdd") + "00001" : f161301CheckExist.RTN_CHECK_NO;

			if (f161301CheckExist == null)
			{
				//1. 新增F161301
				var maxValue = f161301repo.Filter(x => x.DC_CODE == dcCode).Max(x => x.RTN_CHECK_NO);
				if (!string.IsNullOrEmpty(maxValue))
					RtnCheckNo = Convert.ToString(Convert.ToInt64(maxValue) + 1);

				var f161301 = new F161301();
				f161301.RTN_CHECK_NO = RtnCheckNo;
				f161301.DC_CODE = dcCode;
				f161301.CONSIGNEE = consignee;
				f161301.RECEIPT_DATE = Convert.ToDateTime(receiptDate);
				f161301.TRANSPORT = transport;
				f161301.CAR_NO = carNo;
				f161301repo.Add(f161301);
			}

			if (f161302CheckExist == null || !f161302CheckExist.Any())
			{
				//2. 新增F161302
				int RtnSeq = 1;
				var f161302Result = f161302repo.Filter(x => x.DC_CODE == dcCode && x.RTN_CHECK_NO == RtnCheckNo).ToList();
				if (f161302Result != null && f161302Result.Any()) RtnSeq = f161302Result.Max(x => x.RTN_CHECK_SEQ) + 1;

				var f161302 = new F161302();
				f161302.RTN_CHECK_NO = RtnCheckNo;
				f161302.RTN_CHECK_SEQ = RtnSeq;
				f161302.DC_CODE = dcCode;
				if (!string.IsNullOrEmpty(returnNo)) f161302.RETURN_NO = returnNo;
				if (!string.IsNullOrEmpty(pastNo)) f161302.PAST_NO = pastNo;
				if (!string.IsNullOrEmpty(barCode)) f161302.EAN_CODE = barCode;
				f161302repo.Add(f161302);
			}

			results.Add(result);
			return results.AsQueryable();

		}
	}
}

