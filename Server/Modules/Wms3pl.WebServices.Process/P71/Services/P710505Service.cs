using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710505Service
	{
		private WmsTransaction _wmsTransaction;
		public P710505Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF199005(F199005 f199005)
		{
			var repF199005 = new F199005Repository(Schemas.CoreSchema, _wmsTransaction);

			repF199005.Add(f199005);
			return new ExecuteResult(true, "新增成功!");
		}


		public ExecuteResult UpdateF199005(F199005 f199005)
		{
			var repF199005 = new F199005Repository(Schemas.CoreSchema, _wmsTransaction);

			var f199005UpdateItem = repF199005.Find(x => x.DC_CODE == f199005.DC_CODE &&
												x.ACC_ITEM_KIND_ID == f199005.ACC_ITEM_KIND_ID &&
												x.ACC_AREA_ID == f199005.ACC_AREA_ID &&												
												x.LOGI_TYPE == f199005.LOGI_TYPE &&
												x.ACC_KIND == f199005.ACC_KIND &&
												x.DELV_EFFIC == f199005.DELV_EFFIC &&
												x.IS_SPECIAL_CAR == f199005.IS_SPECIAL_CAR &&
												x.ACC_UNIT == f199005.ACC_UNIT &&
												x.DELV_ACC_TYPE == f199005.DELV_ACC_TYPE && x.STATUS =="0");
			if (f199005UpdateItem != null)
			{
				f199005UpdateItem.CAR_KIND_ID = f199005.CAR_KIND_ID;
				f199005UpdateItem.IN_TAX = f199005.IN_TAX;
				f199005UpdateItem.MAX_WEIGHT = f199005.MAX_WEIGHT;
				f199005UpdateItem.FEE = f199005.FEE;
				f199005UpdateItem.OVER_VALUE = f199005.OVER_VALUE;
				f199005UpdateItem.OVER_UNIT_FEE = f199005.OVER_UNIT_FEE;
				f199005UpdateItem.ACC_ITEM_NAME = f199005.ACC_ITEM_NAME;
				f199005UpdateItem.ACC_NUM = f199005.ACC_NUM;
				repF199005.Update(f199005UpdateItem);
			}
			return new ExecuteResult(true, "更新成功!");
		}
	}
}
