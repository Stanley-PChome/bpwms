
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using AutoMapper;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P16.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using System.Data.Objects;
using Wms3pl.Common.Helper;

namespace Wms3pl.WebServices.Process.P70.Services
{
	public partial class P700104Service
	{
		private WmsTransaction _wmsTransaction;
		public P700104Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult DeleteF700101(F700101 f700101)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);

			var distrCar = new DistrCarData
			{
				F700101 = f700101
			};

			// 初始化檢核派車單的服務
			var p70010401Service = new P70010401Service(new List<DistrCarData> { distrCar }, _wmsTransaction);
			var result = p70010401Service.CheckCanEditOrDelete(isEdit: false, dcCode: f700101.DC_CODE, distrCarNos: f700101.DISTR_CAR_NO);
			if (!result.IsSuccessed)
				return result;

			var f700101Data = f700101Repo.Find(x => x.DISTR_CAR_NO == f700101.DISTR_CAR_NO && x.DC_CODE == f700101.DC_CODE);
			f700101Data.STATUS = "9";
			f700101Repo.Update(f700101Data);

			// 派車單移除此出貨單 會將F050801.NO_DELV=0
			var dbF700102s = f700102Repo.GetDatas(f700101.DC_CODE, f700101.DISTR_CAR_NO);
			var setNoDelvF700102List = dbF700102s.Where(db700102 => db700102.ORD_TYPE == "O");

			foreach (var g in setNoDelvF700102List.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE }))
			{
				var setParam = new F050801 { NO_DELV = "0" };
				f050801Repo.UpdateFieldsInWithTrueAndCondition(SET: new { setParam.NO_DELV },
																WHERE: x => x.DC_CODE == g.Key.DC_CODE
																	&& x.GUP_CODE == g.Key.GUP_CODE
																	&& x.CUST_CODE == g.Key.CUST_CODE,
																InFieldName: x => x.WMS_ORD_NO,
																InValues: g.Select(x => x.WMS_NO));
			}


			// 刪除託運單
			foreach (var g in dbF700102s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE }))
			{
				// 託運單的 WMS_NO 可能包含出貨單號、退貨單號、派車單號
				f050901Repo.DeleteInWithTrueAndCondition(x => x.DC_CODE == g.Key.DC_CODE
															&& x.GUP_CODE == g.Key.GUP_CODE
															&& x.CUST_CODE == g.Key.CUST_CODE,
														x => x.WMS_NO,
														g.Select(x => x.WMS_NO).Concat(new List<string> { f700101.DISTR_CAR_NO }));
			}

			// 移除來源單據的派車單號
			foreach (var dbF700102 in dbF700102s)
			{
				var updateSourceResult = p70010401Service.UpdateDistrCarNoByWmsNo(dbF700102, "", Properties.Resources.P700104Service_Delete);
				if (!updateSourceResult.IsSuccessed)
					return updateSourceResult;
			}

			return result;
		}

		public ExecuteResult DeleteF700101ByDistrCarNo(string distrCarNo, string dcCode)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700101Data = f700101Repo.Find(x => x.DISTR_CAR_NO == distrCarNo && x.DC_CODE == dcCode);
			return DeleteF700101(f700101Data);
		}

		public IQueryable<F700101EX> GetF700101ByDistrCarNo(string distrCarNo, string dcCode)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema);
			var result = f700101Repo.GetF700101ByDistrCarNo(distrCarNo, dcCode);
			return result;
		}

	}
}

