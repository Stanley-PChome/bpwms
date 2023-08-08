using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F19;

namespace Wms3pl.WebServices.Process.P02.Services
{
	public partial class P020104Service
	{
		private WmsTransaction _wmsTransaction;
		public P020104Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region P020104 碼頭期間設定
		/// <summary>
		/// 更新碼頭期間設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		/// <param name="pierCode"></param>
		/// <param name="area"></param>
		/// <param name="allowIn"></param>
		/// <param name="allowOut"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public ExecuteResult UpdateF020104(string dcCode, string beginDate, string endDate, string pierCode, string area, string allowIn, string allowOut, string userId)
		{
			var repo = new F020104Repository(Schemas.CoreSchema, _wmsTransaction);
			var tmp = repo.Find(x => x.DC_CODE.Equals(dcCode)
								&& x.PIER_CODE.Equals(pierCode)
								&& x.BEGIN_DATE== Convert.ToDateTime(beginDate)
								&& x.END_DATE == Convert.ToDateTime(endDate));
			if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = tmp.PIER_CODE+"資料已被刪除, 請重新查詢" };

			tmp.TEMP_AREA = Int16.Parse(area);
			tmp.ALLOW_IN = allowIn;
			tmp.ALLOW_OUT = allowOut;
			tmp.UPD_DATE = DateTime.Now;
			tmp.UPD_STAFF = userId;

			repo.Update(tmp);
			return new ExecuteResult() { IsSuccessed = true };
		}

		/// <summary>
		/// 新增碼頭期間設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		/// <param name="pierCode"></param>
		/// <param name="area"></param>
		/// <param name="allowIn"></param>
		/// <param name="allowOut"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public ExecuteResult InsertF020104(string dcCode, string beginDate, string endDate, string pierCode, string area, string allowIn, string allowOut, string userId)
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var repo = new F020104Repository(Schemas.CoreSchema, _wmsTransaction);
			F020104 f020104Detail = new F020104()
			{
				DC_CODE = dcCode,
				BEGIN_DATE = Convert.ToDateTime(beginDate),
				END_DATE = Convert.ToDateTime(endDate),
				PIER_CODE = pierCode,
				TEMP_AREA = Int16.Parse(area),
				ALLOW_IN = allowIn,
				ALLOW_OUT = allowOut,
				CRT_STAFF = userId,
			};
			repo.Add(f020104Detail);
			result.IsSuccessed = true;

			return result;
		}

		/// <summary>
		/// 碼頭期間設定清單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		/// <param name="pierCode"></param>
		/// <param name="area"></param>
		/// <param name="allowIn"></param>
		/// <param name="allowOut"></param>
		/// <returns></returns>
		public IQueryable<F020104Detail> GetF020104Detail(string dcCode,DateTime beginDate,DateTime endDate, string pierCode, string area, string allowIn,string allowOut)
		{
			var repo = new F020104Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetF020104Detail(dcCode, beginDate, endDate, pierCode, area, allowIn, allowOut);
		}
		#endregion
	}
}
