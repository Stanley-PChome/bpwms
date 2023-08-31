using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	public class ExportService
	{
		#region Repository
		private F055004Repository _f055004Repo;
		public F055004Repository F055004Repo
		{
			get { return _f055004Repo == null ? _f055004Repo = new F055004Repository(Schemas.CoreSchema) : _f055004Repo; }
			set { _f055004Repo = value; }
		}

		private F050301Repository _f050301Repo;
		public F050301Repository F050301Repo
		{
			get { return _f050301Repo == null ? _f050301Repo = new F050301Repository(Schemas.CoreSchema) : _f050301Repo; }
			set { _f050301Repo = value; }
		}

		private F05030101Repository _f05030101Repo;
		public F05030101Repository F05030101Repo
		{
			get { return _f05030101Repo == null ? _f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction) : _f05030101Repo; }
			set { _f05030101Repo = value; }
		}

		private F050302Repository _f050302Repo;
		public F050302Repository F050302Repo
		{
			get { return _f050302Repo == null ? _f050302Repo = new F050302Repository(Schemas.CoreSchema) : _f050302Repo; }
			set { _f050302Repo = value; }
		}

		private F05030202Repository _f05030202Repo;
		public F05030202Repository F05030202Repo
		{
			get { return _f05030202Repo == null ? _f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction) : _f05030202Repo; }
			set { _f05030202Repo = value; }
		}

		private F055001Repository _f055001Repo;
		public F055001Repository F055001Repo
		{
			get { return _f055001Repo == null ? _f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction) : _f055001Repo; }
			set { _f055001Repo = value; }
		}

		private F055002Repository _f055002Repo;
		public F055002Repository F055002Repo
		{
			get { return _f055002Repo == null ? _f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction) : _f055002Repo; }
			set { _f055002Repo = value; }
		}

		private F051202Repository _f051202Repo;
		public F051202Repository F051202Repo
		{
			get { return _f051202Repo == null ? _f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction) : _f051202Repo; }
			set { _f051202Repo = value; }
		}

		#endregion Repository

		private List<F05030101> _f05030101CacheList;
		private List<F05030202> _f05030202CacheList;
		private List<F055001> _f055001CacheList;
		private List<F055002> _f055002CacheList;

		private WmsTransaction _wmsTransaction;

		public ExportService(WmsTransaction wmsTransation = null)
		{
			_wmsTransaction = wmsTransation;
		}

		public List<string> GetWmsOrdNos(F050305 f050305)
		{
			if (_f05030101CacheList == null) _f05030101CacheList = new List<F05030101>();

			var curnDatas = _f05030101CacheList.Where(x => x.DC_CODE == f050305.DC_CODE
											&& x.GUP_CODE == f050305.GUP_CODE
											&& x.CUST_CODE == f050305.CUST_CODE
											&& x.ORD_NO == f050305.ORD_NO).ToList();

			if (!curnDatas.Any())
			{
				curnDatas = F05030101Repo.GetDatasByOrdNo(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, f050305.ORD_NO).ToList();
				_f05030101CacheList.AddRange(curnDatas);
			}
			return curnDatas.Select(x => x.WMS_ORD_NO).ToList();
		}

		public List<F055001> GetF055001s(F050305 f050305)
		{
			if (_f055001CacheList == null) _f055001CacheList = new List<F055001>();

			var wmsOrdNos = GetWmsOrdNos(f050305);
			var curnDatas = _f055001CacheList.Where(x => x.DC_CODE == f050305.DC_CODE
											&& x.GUP_CODE == f050305.GUP_CODE
											&& x.CUST_CODE == f050305.CUST_CODE
											&& wmsOrdNos.Contains(x.WMS_ORD_NO)).ToList();

			if (!curnDatas.Any())
			{
				curnDatas = F055001Repo.GetDatasByWmsOrdNos(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, wmsOrdNos).ToList();
				_f055001CacheList.AddRange(curnDatas);
			}
			return curnDatas;
		}

		public List<F055002> GetF055002s(F050305 f050305)
		{
			if (_f055002CacheList == null) _f055002CacheList = new List<F055002>();

			var curnDatas = _f055002CacheList.Where(x => x.DC_CODE == f050305.DC_CODE
											&& x.GUP_CODE == f050305.GUP_CODE
											&& x.CUST_CODE == f050305.CUST_CODE
											&& x.ORD_NO == f050305.ORD_NO).ToList();

			if (!curnDatas.Any())
			{
				curnDatas = F055002Repo.GetDatasByOrdNo(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, f050305.ORD_NO).ToList();
				_f055002CacheList.AddRange(curnDatas);
			}
			return curnDatas;
		}

		public List<F055004> GetF055004s(F050305 f050305)
		{
			return F055004Repo.GetDatasByOrdNo(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, f050305.ORD_NO).ToList();
		}

		public List<F05030202> GetF05030202s(string dcCode, string gupCode, string custCode, List<string> ordNos)
		{
			if (_f05030202CacheList == null) _f05030202CacheList = new List<F05030202>();

			var curnDatas = _f05030202CacheList.Where(x => x.DC_CODE == dcCode
											&& x.GUP_CODE == gupCode
											&& x.CUST_CODE == custCode
											&& ordNos.Contains(x.ORD_NO)).ToList();

			if (!curnDatas.Any())
			{
				curnDatas = F05030202Repo.GetDatasByOrdNos(dcCode, gupCode, custCode, ordNos).ToList();
				_f05030202CacheList.AddRange(curnDatas);
			}
			return curnDatas;
		}

		public List<F055004> CreateF055004(F050305 f050305)
		{
			var addF055004List = new List<F055004>();

			//判斷是否為跨庫訂單
			var isMoveOut = F050301Repo.IsMoveOut(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, f050305.ORD_NO);

			//取得訂單指定批號
			var orderWithMakeNo = F050302Repo.GetMakeNosByOrdNo(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, f050305.ORD_NO).ToList();

			//取得所有訂單項次
			var f05030202s = GetF05030202s(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, new List<string> { f050305.ORD_NO });
			var currentOrdSeqs = f05030202s.Select(x => x.ORD_SEQ).ToList();

			var f055001s = GetF055001s(f050305);
			var f055002s = GetF055002s(f050305).Where(x => currentOrdSeqs.Contains(x.ORD_SEQ)).ToList();
			// 揀貨資料
			var f051202s = F051202Repo.GetDatasByWmsOrdNosAndPickStatus1(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, GetWmsOrdNos(f050305)).ToList();

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
						 join D in orderWithMakeNo
						 on new { B.ORD_NO, B.ORD_SEQ } equals new { D.ORD_NO, D.ORD_SEQ }
						 orderby B.SERIAL_NO descending, D.MAKE_NO descending
						 select new F055004
						 {
							 ORD_NO = B.ORD_NO,
							 ORD_SEQ = B.ORD_SEQ,
							 WMS_NO = B.WMS_ORD_NO,
							 BOX_NO = B.PACKAGE_BOX_NO.ToString(),
							 BOX_NUM = C.BOX_NUM,
							 ITEM_CODE = B.ITEM_CODE,
							 MAKE_NO = D.MAKE_NO,
							 QTY = Convert.ToInt32(B.PACKAGE_QTY),
							 SERIAL_NO = B.SERIAL_NO
						 }).ToList();

			// 迴圈包裝資料
			datas.ForEach(data =>
			{
				var currPickDatas = f051202s.Where(x => x.WMS_ORD_NO == data.WMS_NO && x.ITEM_CODE == data.ITEM_CODE);
				//var currPickDatas = pickDatas.Where(x => x.ORD_NO == data.ORD_NO && x.ORD_SEQ == data.ORD_SEQ && x.WMS_ORD_NO == data.WMS_NO).ToList();
				if (!string.IsNullOrEmpty(data.MAKE_NO))
					currPickDatas = currPickDatas.Where(x => x.MAKE_NO == data.MAKE_NO);

				foreach (var pickData in currPickDatas)
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
							VALID_DATE = isMoveOut ? pickData.VALID_DATE : (DateTime?)null,
							SERIAL_NO = data.SERIAL_NO
						});
					}
				}
			});

			addF055004List = addF055004List.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ORD_NO, x.ORD_SEQ, x.WMS_NO, x.BOX_NO, x.BOX_NUM, x.ITEM_CODE, x.MAKE_NO, x.VALID_DATE, x.SERIAL_NO })
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
				VALID_DATE = x.Key.VALID_DATE,
				SERIAL_NO = x.Key.SERIAL_NO
			}).ToList();

			if (addF055004List.Any())
				F055004Repo.BulkInsert(addF055004List, "ID");

			return addF055004List;
		}
	}
}
