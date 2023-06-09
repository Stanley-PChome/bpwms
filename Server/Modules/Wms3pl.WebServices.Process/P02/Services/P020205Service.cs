using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P02.Services
{
	public partial class P020205Service
	{
		private WmsTransaction _wmsTransaction;

		public P020205Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<P020205Main> GetJincangNoFileMain(string dcCode, string gupCode, string custCode,
															DateTime importStartDate, DateTime importEndDate,
															string poNo)
		{
			var f020301Repo = new F020301Repository(Schemas.CoreSchema);
			return f020301Repo.GetJincangNoFileMain(dcCode, gupCode, custCode, importStartDate, importEndDate, poNo);
		}

		public IQueryable<P020205Detail> GetJincangNoFileDetail(string dcCode, string gupCode, string custCode, string fileName, string poNo)
		{
			var f020302Repo = new F020302Repository(Schemas.CoreSchema);
			return f020302Repo.GetJincangNoFileDetail(dcCode, gupCode, custCode, fileName, poNo);
		}

		/// <summary>
		/// 進倉序號匯入有錯時，可以刪除資料。
		/// 新增刪除功能，只有當該檔案中，有序號資料未匯入與未抽驗 (F02020104) 時才可以進行刪除。F020302 狀態改為9。
		/// </summary>
		/// <param name="p020205Main"></param>
		/// <returns></returns>
		public ExecuteResult DeleteP020205(P020205Main p020205Main)
		{
			var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);

			#region 檢查是否可刪除
			var dbF020301 = f020301Repo.Find(x => x.DC_CODE == p020205Main.DC_CODE
												&& x.GUP_CODE == p020205Main.GUP_CODE
												&& x.CUST_CODE == p020205Main.CUST_CODE
												&& x.FILE_NAME == p020205Main.FILE_NAME);

			if (dbF020301.STATUS != "0")
				return new ExecuteResult(false, "此進倉序號主檔狀態已無法刪除!");



			var isAcceptance = f020302Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(p020205Main.DC_CODE)
												&& x.GUP_CODE == EntityFunctions.AsNonUnicode(p020205Main.GUP_CODE)
												&& x.CUST_CODE == EntityFunctions.AsNonUnicode(p020205Main.CUST_CODE)
												&& x.FILE_NAME == p020205Main.FILE_NAME)
											.Any(x => x.STATUS == "1");
			if (isAcceptance)
			{
				return new ExecuteResult(false, "無法刪除!此進倉序號檔已完成檢驗確認!");
			}
			#endregion

			// 刪除隨機抽驗
			f02020104Repo.UpdateIsPass0ByF020302(p020205Main.DC_CODE, p020205Main.GUP_CODE, p020205Main.CUST_CODE, p020205Main.FILE_NAME);

			#region 修改狀態
			var dbF020302s = f020302Repo.AsForUpdate()
										.GetDatasByTrueAndCondition(x => x.DC_CODE == p020205Main.DC_CODE
																	&& x.GUP_CODE == p020205Main.GUP_CODE
																	&& x.CUST_CODE == p020205Main.CUST_CODE
																	&& x.FILE_NAME == p020205Main.FILE_NAME).ToList();

			dbF020301.STATUS = "9";
			dbF020302s.ForEach(x => x.STATUS = "9");

			f020301Repo.Update(dbF020301);
			f020302Repo.BulkUpdate(dbF020302s);
			#endregion

			return new ExecuteResult(true);
		}
	}
}
