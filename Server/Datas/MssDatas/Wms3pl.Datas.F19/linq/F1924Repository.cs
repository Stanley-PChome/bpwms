using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Wms3pl.Datas.Shared.Pda.Entitues;

namespace Wms3pl.Datas.F19
{
	public partial class F1924Repository : RepositoryBase<F1924, Wms3plDbContext, F1924Repository>
	{
		public F1924Repository(string connName, WmsTransaction wmsTransaction = null)
		: base(connName, wmsTransaction)
		{
		}

		#region P1905 權限功能
		/// <summary>
		/// 傳回F1924 + F192403 List
		/// </summary>
		/// <returns></returns>
		public IQueryable<F1924Data> F1924WithF192403()
		{
			var f1924s = _db.F1924s.AsNoTracking().Where(x => x.ISDELETED == "0");
			var f192403s = _db.F192403s.AsNoTracking();

			var result = (from A in f1924s
										join B in f192403s on A.EMP_ID equals B.EMP_ID into ps
										from B in ps.DefaultIfEmpty()
										orderby A.EMP_ID
										select new F1924Data
										{
											EMP_ID = A.EMP_ID,
											EMP_NAME = A.EMP_NAME,
											EMAIL = A.EMAIL,
											CRT_STAFF = A.CRT_STAFF,
											CRT_DATE = A.CRT_DATE,
											UPD_STAFF = A.UPD_STAFF,
											UPD_DATE = A.UPD_DATE,
											ISDELETED = A.ISDELETED,
											PACKAGE_UNLOCK = A.PACKAGE_UNLOCK,
											CRT_NAME = A.CRT_NAME,
											UPD_NAME = A.UPD_NAME,
											ISCOMMON = A.ISCOMMON,
											DEP_ID = A.DEP_ID,
											WORK_ID = B.WORK_ID,
										});

			return result;
		}
		#endregion

		#region P1906 使用者安全控管設定
		/// <summary>
		/// 取得使用者密碼
		/// </summary>
		/// <returns></returns>
		public IQueryable<GetUserPassword> GetUserPassword(string empId)
		{
			var f1924s = _db.F1924s.AsNoTracking().Where(x => x.EMP_ID == empId);
			var f1952s = _db.F1952s.AsNoTracking();

			var result = from A in f1924s
									 join B in f1952s on A.EMP_ID equals B.EMP_ID into C
									 from B in C.DefaultIfEmpty()
									 select new GetUserPassword
									 {
										 EMP_ID = A.EMP_ID,
										 PASSWORD = B.PASSWORD
									 };
			return result.AsQueryable();
		}
		#endregion

		public bool IsCanUseful(string pickStaff, string btId)
		{
			var f1924s = _db.F1924s.AsNoTracking().Where(x => x.EMP_ID == pickStaff
																							 && x.ISDELETED == "0");
			var f192401s = _db.F192401s.AsNoTracking();
			var f1953s = _db.F1953s.AsNoTracking();
			var f195301s = _db.F195301s.AsNoTracking();
			var f1954s = _db.F1954s.AsNoTracking().Where(x => x.FUN_CODE == btId);

			var result = from A in f1924s
									 join B in f192401s on A.EMP_ID equals B.EMP_ID
									 join C in f1953s on B.GRP_ID equals C.GRP_ID
									 join D in f195301s on C.GRP_ID equals D.GRP_ID
									 join E in f1954s on D.FUN_CODE equals E.FUN_CODE
									 select new { A, B, C, D, E };

			return (result != null && result.Any());
		}

		public IQueryable<GrpMailMobile> GetActiveDatas(List<decimal> grpIds)
		{
			var f1924s = _db.F1924s.AsNoTracking().Where(x => x.ISDELETED != "1");
			var f192401s = _db.F192401s.AsNoTracking().Where(x => grpIds.Contains(x.GRP_ID));
			var f1953s = _db.F1953s.AsNoTracking().Where(x => x.ISDELETED != "1");

			var result = from A in f1924s
									 join B in f192401s on A.EMP_ID equals B.EMP_ID
									 join C in f1953s on B.GRP_ID equals C.GRP_ID
									 select new GrpMailMobile
									 {
										 GRP_ID = B.GRP_ID,
										 EMAIL = A.EMAIL,
										 MOBILE = A.MOBILE
									 };

			return result.AsQueryable();
		}

		public IQueryable<F1924> GetEmpHasAuth(string empID)
		{
			var f1924s = _db.F1924s.Where(x => x.EMP_ID == empID);
			var f192401s = _db.F192401s;
			var f1953s = _db.F1953s.Where(x => x.SHOWINFO == "1");
			var result = from A in f1924s
									 join B in f192401s on A.EMP_ID equals B.EMP_ID
									 join C in f1953s on B.GRP_ID equals C.GRP_ID
									 select new F1924
									 {
										 EMP_ID = A.EMP_ID,
										 EMP_NAME = A.EMP_NAME,
										 EMAIL = A.EMAIL,
										 CRT_STAFF = A.CRT_STAFF,
										 CRT_DATE = A.CRT_DATE,
										 UPD_STAFF = A.UPD_STAFF,
										 UPD_DATE = A.UPD_DATE,
										 ISDELETED = A.ISDELETED,
										 PACKAGE_UNLOCK = A.PACKAGE_UNLOCK,
										 CRT_NAME = A.CRT_NAME,
										 UPD_NAME = A.UPD_NAME,
										 ISCOMMON = A.ISCOMMON,
										 DEP_ID = A.DEP_ID,
										 MOBILE = A.MOBILE,
										 SHORT_MOBILE = A.SHORT_MOBILE,
										 TEL_EXTENSION = A.TEL_EXTENSION,
										 EXTENSION_A = A.EXTENSION_A,
										 EXTENSION_B = A.EXTENSION_B,
										 EXTENSION_C = A.EXTENSION_C,
										 EXTENSION_D = A.EXTENSION_D,
										 EXTENSION_E = A.EXTENSION_E,
										 MENUSTYLE = A.MENUSTYLE,
										 MENU_CODE = A.MENU_CODE
									 };

			return result.AsQueryable();
		}

		public UserInfo GetUserInfo(string accNo)
		{
			var result = _db.F1924s.AsNoTracking().Where(x => x.EMP_ID == accNo).
					Select(x => new UserInfo
					{
						AccNo = x.EMP_ID,
						UserName = x.EMP_NAME
					});

			return result.FirstOrDefault();
		}

		public IQueryable<F1924> CheckAcc(string empID)
		{
			var result = _db.F1924s.Where(x => x.EMP_ID == empID
																	&& x.ISDELETED == "0");

			return result;
		}

		/// <summary>
		/// 取得人員名稱
		/// </summary>
		/// <param name="accNo">帳號</param>
		/// <returns></returns>
		public string GetEmpName(string accNo)
		{
			var result = _db.F1924s.AsNoTracking().Where(x => x.EMP_ID == accNo)
																						.Select(x => x.EMP_NAME).FirstOrDefault();
			if (string.IsNullOrEmpty(result))
				result = "支援人員";

			return result;
		}

		public IQueryable<F1924> GetDatasForEmpIds(List<string> empIds)
		{
			return _db.F1924s.AsNoTracking().Where(x => empIds.Contains(x.EMP_ID) && x.ISDELETED == "0");
		}
	}
}