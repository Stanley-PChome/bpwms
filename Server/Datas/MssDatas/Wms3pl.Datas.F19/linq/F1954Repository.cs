using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1954Repository : RepositoryBase<F1954, Wms3plDbContext, F1954Repository>
	{
		public F1954Repository(string connName, WmsTransaction wmsTransaction = null)
		 : base(connName, wmsTransaction)
		{
		}

		public IQueryable<F1954> GetDatas()
		{
			var f1954s = _db.F1954s;
			var f1954_I18N = _db.F1954_I18N.Where(x => x.LANG == Current.Lang);

			var result = from A in f1954s
									 join B in f1954_I18N on A.FUN_CODE equals B.FUN_CODE
									 orderby A.FUN_CODE
									 select new F1954
									 {
										 FUN_CODE = A.FUN_CODE,
										 FUN_NAME = B.FUN_NAME,
										 FUN_TYPE = A.FUN_TYPE,
										 FUN_DESC = B.FUN_DESC,
										 CRT_STAFF = A.CRT_STAFF,
										 CRT_DATE = A.CRT_DATE,
										 UPD_STAFF = A.UPD_STAFF,
										 UPD_DATE = A.UPD_DATE,
										 STATUS = A.STATUS,
										 UPLOAD_DATE = A.UPLOAD_DATE,
										 DISABLE = A.DISABLE,
										 CRT_NAME = A.CRT_NAME,
										 UPD_NAME = A.UPD_NAME,
										 MAIN_SHOW = A.MAIN_SHOW,
										 SIDE_SHOW = A.SIDE_SHOW
									 };
			return result;
		}

		public IQueryable<F1954> GetDatas(string account)
		{

			var f192401s = _db.F192401s.Where(x => x.EMP_ID == account);
			var f195301s = _db.F195301s;
			var funCode = from C in f192401s
										join D in f195301s on C.GRP_ID equals D.GRP_ID into DD
										from D in DD.DefaultIfEmpty()
										select D.FUN_CODE;
			var f1954s = _db.F1954s;
			var f1954_I18N = _db.F1954_I18N;
			var result = from A in f1954s
									 join B in f1954_I18N on A.FUN_CODE equals B.FUN_CODE
									 where funCode.Contains(A.FUN_CODE) && B.LANG == Current.Lang
									 orderby A.FUN_CODE
									 select new F1954
									 {
										 FUN_CODE = A.FUN_CODE,
										 FUN_NAME = B.FUN_NAME,
										 FUN_TYPE = A.FUN_TYPE,
										 FUN_DESC = B.FUN_DESC,
										 CRT_STAFF = A.CRT_STAFF,
										 CRT_DATE = A.CRT_DATE,
										 UPD_STAFF = A.UPD_STAFF,
										 UPD_DATE = A.UPD_DATE,
										 STATUS = A.STATUS,
										 UPLOAD_DATE = A.UPLOAD_DATE,
										 DISABLE = A.DISABLE,
										 CRT_NAME = A.CRT_NAME,
										 UPD_NAME = A.UPD_NAME,
										 MAIN_SHOW = A.MAIN_SHOW,
										 SIDE_SHOW = A.SIDE_SHOW
									 };

			return result;
		}

		public IQueryable<FunctionCodeName> GetAllFunctions()
		{
			var f1954s = _db.F1954s.AsNoTracking();
			var f1954_I18N = _db.F1954_I18N.AsNoTracking().Where(x => x.LANG == Current.Lang);
			var result = from A in f1954s
									 join B in f1954_I18N on A.FUN_CODE equals B.FUN_CODE
									 select new FunctionCodeName
									 {
										 FUN_CODE = A.FUN_CODE,
										 FUN_NAME = B.FUN_NAME
									 };
			return result;
		}

		public IQueryable<string> GetAllFunCodes()
		{
			var result = _db.F1954s.AsNoTracking().Select(x => x.FUN_CODE);
			return result;
		}

		public IQueryable<string> GetFunName(string funcNo)
		{
			var result = _db.F1954s.AsNoTracking().Where(x => x.FUN_CODE == funcNo)
																						.Select(x => x.FUN_NAME);

			return result;
		}

		public IQueryable<F1954> GetDataByFunCodes(List<string> funCodeList)
		{
			return _db.F1954s.Where(x => funCodeList.Select(funCode => EntityFunctions.AsNonUnicode(funCode)).Contains(x.FUN_CODE));
		}

		public IQueryable<F1954> GetOtherGrpIdDatas(List<string> funCodeList, decimal grpId)
		{
			var f195301s = _db.F195301s.Where(x => x.GRP_ID != grpId && funCodeList.Contains(x.FUN_CODE));

			var funcodes = f195301s.GroupBy(x => x.FUN_CODE).Where(x => x.Count() == 0).Select(x => EntityFunctions.AsNonUnicode(x.Key)).ToList();

			return _db.F1954s.Where(x => funcodes.Contains(x.FUN_CODE));
		}
	}
}
