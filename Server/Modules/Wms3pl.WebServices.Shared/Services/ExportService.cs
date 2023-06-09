using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	public class ExportService
	{
		private WmsTransaction _wmsTransaction;

		public ExportService(WmsTransaction wmsTransation = null)
		{
			_wmsTransaction = wmsTransation;
		}


		public List<F055004> CreateF055004(F050305 f050305)
		{
			var f055004Repo = new F055004Repository(Schemas.CoreSchema);
			var addF055004List = new List<F055004>();

			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			var f050301 = f050301Repo.Find(x => x.DC_CODE == f050305.DC_CODE && x.CUST_CODE == f050305.CUST_CODE && x.GUP_CODE == f050305.GUP_CODE && x.ORD_NO == f050305.ORD_NO);
			var f050302Repo = new F050302Repository(Schemas.CoreSchema);
			var f050302s = f050302Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f050305.DC_CODE && x.GUP_CODE == f050305.GUP_CODE && x.CUST_CODE == f050305.CUST_CODE && x.ORD_NO == f050305.ORD_NO).ToList();
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema);
			var f05030202s = f05030202Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f050305.DC_CODE && x.GUP_CODE == f050305.GUP_CODE && x.CUST_CODE == f050305.CUST_CODE && x.ORD_NO == f050305.ORD_NO).ToList();
			var f055001Repo = new F055001Repository(Schemas.CoreSchema);
			var f055002Repo = new F055002Repository(Schemas.CoreSchema);
			var f055002s = f055002Repo.GetDatasByOrdSeqs(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, f050305.ORD_NO, f05030202s.Select(x => x.ORD_SEQ).ToList());
			var wmsOrdNos = f055002s.Select(x => x.WMS_ORD_NO).Distinct().ToList();
			var f055001s = f055001Repo.GetDatas(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, wmsOrdNos).ToList();
			var isMoveOut = f050301.CUST_COST == "MoveOut";
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			// 揀貨資料
			var f051202s = f051202Repo.GetDatasByWmsOrdNosAndPickStatus1(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, wmsOrdNos).ToList();

			//var pickDatas = (from o in f051202s
			//								 join c in f05030202s
			//								 on new { o.WMS_ORD_NO, o.WMS_ORD_SEQ } equals new { c.WMS_ORD_NO, c.WMS_ORD_SEQ }
			//								 group o by new { c.ORD_NO, c.ORD_SEQ, o.WMS_ORD_NO, o.ITEM_CODE, o.MAKE_NO, o.VALID_DATE } into g
			//								 select new ExportDataPickModel
			//								 {
			//									 ORD_NO = g.Key.ORD_NO,
			//									 ORD_SEQ = g.Key.ORD_SEQ,
			//									 WMS_ORD_NO = g.Key.WMS_ORD_NO,
			//									 ITEM_CODE = g.Key.ITEM_CODE,
			//									 MAKE_NO = g.Key.MAKE_NO,
			//									 VALID_DATE = g.Key.VALID_DATE,
			//									 QTY = g.Sum(x => x.A_PICK_QTY)
			//								 }).ToList();

			// 包裝資料
			var datas = (from B in f055002s
									 join C in f055001s
									 on new { B.WMS_ORD_NO, B.PACKAGE_BOX_NO } equals new { C.WMS_ORD_NO, C.PACKAGE_BOX_NO }
									 join D in f050302s
									 on new {B.ORD_NO,B.ORD_SEQ} equals new { D.ORD_NO,D.ORD_SEQ}
									 orderby D.MAKE_NO descending
									 select new F055004
									 {
										 ORD_NO = B.ORD_NO,
										 ORD_SEQ = B.ORD_SEQ,
										 WMS_NO = B.WMS_ORD_NO,
										 BOX_NO = B.PACKAGE_BOX_NO.ToString(),
										 BOX_NUM = C.BOX_NUM,
										 ITEM_CODE = B.ITEM_CODE,
										 MAKE_NO = D.MAKE_NO,
										 QTY = Convert.ToInt32(B.PACKAGE_QTY)
									 }).ToList();

			// 迴圈包裝資料
			datas.ForEach(data =>
			{
				var currPickDatas = f051202s.Where(x => x.WMS_ORD_NO == data.WMS_NO && x.ITEM_CODE == data.ITEM_CODE).ToList();
				//var currPickDatas = pickDatas.Where(x => x.ORD_NO == data.ORD_NO && x.ORD_SEQ == data.ORD_SEQ && x.WMS_ORD_NO == data.WMS_NO).ToList();
				if (!string.IsNullOrEmpty(data.MAKE_NO))
					currPickDatas = currPickDatas.Where(x => x.MAKE_NO == data.MAKE_NO).ToList();

				currPickDatas.ForEach(pickData =>
				{
					if (data.QTY > 0 && pickData.A_PICK_QTY > 0)
					{
						var currQty = 0;
						
						if (data.QTY == pickData.A_PICK_QTY)
						{
							currQty = data.QTY;
							data.QTY = 0;
							pickData.A_PICK_QTY = 0;
						}
						else if (data.QTY > pickData.A_PICK_QTY)
						{
							currQty = pickData.A_PICK_QTY;
							data.QTY -= pickData.A_PICK_QTY;
							pickData.A_PICK_QTY = 0;
						}
						else if (data.QTY < pickData.A_PICK_QTY)
						{
							currQty = data.QTY;
							pickData.A_PICK_QTY -= data.QTY;
							data.QTY = 0;
						}

						addF055004List.Add(new F055004
						{
                DC_CODE = f050305.DC_CODE,
                GUP_CODE = f050305.GUP_CODE,
                CUST_CODE = f050305.CUST_CODE,
                ORD_NO = data.ORD_NO,
                ORD_SEQ = data.ORD_SEQ,
                WMS_NO = data.WMS_NO,
                BOX_NO = data.BOX_NO,
                BOX_NUM = data.BOX_NUM,
                ITEM_CODE = data.ITEM_CODE,
                QTY = currQty,
                MAKE_NO = pickData.MAKE_NO,
                VALID_DATE = isMoveOut ? pickData.VALID_DATE : (DateTime?)null
             });
					}
				});
			});

			addF055004List = addF055004List.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ORD_NO, x.ORD_SEQ, x.WMS_NO, x.BOX_NO, x.BOX_NUM, x.ITEM_CODE, x.MAKE_NO,x.VALID_DATE })
			.Select(x => new F055004
			{
				DC_CODE = x.Key.DC_CODE,
				GUP_CODE = x.Key.GUP_CODE,
				CUST_CODE = x.Key.CUST_CODE,
				ORD_NO = x.Key.ORD_NO,
				ORD_SEQ = x.Key.ORD_SEQ,
				WMS_NO = x.Key.WMS_NO,
				BOX_NO = x.Key.BOX_NO,
				BOX_NUM = x.Key.BOX_NUM,
				ITEM_CODE = x.Key.ITEM_CODE,
				QTY = x.Sum(z => z.QTY),
				MAKE_NO = x.Key.MAKE_NO,
                VALID_DATE=x.Key.VALID_DATE
			}).ToList();

			if (addF055004List.Any())
				f055004Repo.BulkInsert(addF055004List,"ID");

			return addF055004List;
		}
	}
}
