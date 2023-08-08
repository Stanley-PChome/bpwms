using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710706Service
	{
		private WmsTransaction _wmsTransaction;
		public P710706Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}


		public IQueryable<F700701QueryData> GetF700701QueryData(string dcCode, string importSDate, string importEDate)
		{
			//投入日期
			var coverImportSDate = (string.IsNullOrEmpty(importSDate)) ? ((DateTime?)null) : Convert.ToDateTime(importSDate);
			var coverImportEDate = (string.IsNullOrEmpty(importEDate)) ? ((DateTime?)null) : Convert.ToDateTime(importEDate);

			var repF700701 = new F700701Repository(Schemas.CoreSchema);
			return repF700701.GetF700701QueryData(dcCode, coverImportSDate, coverImportEDate);
		}

		public ExecuteResult InsertF700701(F700701 f700701)
		{
			var repF700701 = new F700701Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700701Item = repF700701.Find(o => o.DC_CODE == f700701.DC_CODE && o.IMPORT_DATE == f700701.IMPORT_DATE && o.GRP_ID == f700701.GRP_ID);
			if (f700701Item == null)
			{
				repF700701.Add(f700701);
			}
			else
			{
				return new ExecuteResult(false, "相同物流中心、投入日期、工作群組重覆!");
			}

			return new ExecuteResult(true, "新增成功!");
		}

		public ExecuteResult UpdateF700701(F700701 f700701)
		{
			var repF700701 = new F700701Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700701Item = repF700701.Find(o => o.DC_CODE == f700701.DC_CODE && o.IMPORT_DATE == f700701.IMPORT_DATE && o.GRP_ID == f700701.GRP_ID);
			if (f700701Item != null)
			{
				f700701Item.PERSON_NUMBER = f700701.PERSON_NUMBER;
				f700701Item.WORK_HOUR = f700701.WORK_HOUR;
				f700701Item.SALARY = f700701.SALARY;
				repF700701.Update(f700701Item);
			}
			else
			{
				return new ExecuteResult(false, "查無此資料!");
			}
			return new ExecuteResult(true, "更新成功!");
		}

	}
}
