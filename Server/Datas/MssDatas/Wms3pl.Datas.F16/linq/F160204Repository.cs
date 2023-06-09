using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using static Wms3pl.Datas.Shared.ApiEntities.VendorReturnReq;

namespace Wms3pl.Datas.F16
{
	public partial class F160204Repository : RepositoryBase<F160204, Wms3plDbContext, F160204Repository>
	{
		public F160204Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{

		}

		public List<VnrReturnDetail> GetVnrReturnDetails(F050305 f050305, List<F055004> f055004List, List<F160204> f160204List, List<F05030202> f05030202List, List<F050802> f050802List)
		{
			var rtnWmsSeqs = f160204List.Select(x => x.RTN_WMS_SEQ.ToString()).ToList();

			var f05030202s = f05030202List.Where(o => f160204List.Select(x => x.RTN_WMS_SEQ).Contains(Convert.ToInt32(o.ORD_SEQ))).ToList();

			var f055002s = _db.F055002s.AsNoTracking().Where(x => x.DC_CODE == f050305.DC_CODE && x.GUP_CODE == f050305.GUP_CODE && x.CUST_CODE == f050305.CUST_CODE && x.ORD_NO == f050305.ORD_NO && f05030202s.Select(z => z.ORD_SEQ).Contains(x.ORD_SEQ) && !string.IsNullOrWhiteSpace(x.SERIAL_NO));

			var f055004s = f055004List.GroupBy(x => new { x.ITEM_CODE, x.ORD_SEQ, x.MAKE_NO }).Select(x => new
			{
				ItemCode = x.Key.ITEM_CODE,
				Seq = Convert.ToInt32(x.Key.ORD_SEQ),
				MakeNo = x.Key.MAKE_NO,
				Qty = x.Sum(z => z.QTY)
			});

			List<VnrReturnDetailTemp> detailDatas = new List<VnrReturnDetailTemp>();

			// 當F050305.STATUS=3(包裝完成)
			if (f050305.STATUS == "3")
			{
				var detailDatasTmp = (from A in f05030202s
														 join B in f160204List
														 on Convert.ToInt32(A.ORD_SEQ) equals B.RTN_WMS_SEQ
														 select new VnrReturnDetailCal
														 {
															 WmsOrdNo = A.WMS_ORD_NO,
															 WmsOrdSeq = A.WMS_ORD_SEQ,
															 Seq = Convert.ToInt32(A.ORD_SEQ),
															 ItemSeq = B.RTN_VNR_SEQ.ToString(),
															 ItemCode = B.ITEM_CODE,
															 B_ActQty = A.B_DELV_QTY,
															 A_ActQty = Convert.ToInt32(A.A_DELV_QTY),
														 }).ToList();

				// 取得出貨單明細F050802.B_DELV_QTY去分配F05030202.A_DELV_QTY
				detailDatasTmp.ForEach(detail =>
				{
					if (detail.A_ActQty != detail.B_ActQty)
					{
						var currF050802s = f050802List.Where(x => x.WMS_ORD_NO == detail.WmsOrdNo && x.WMS_ORD_SEQ == detail.WmsOrdSeq && x.B_DELV_QTY > 0).ToList();

						currF050802s.ForEach(f050802 =>
						{
							if (f050802.B_DELV_QTY == detail.B_ActQty)
							{
								detail.A_ActQty = detail.B_ActQty;
								f050802.B_DELV_QTY = 0;
							}
							else if (f050802.B_DELV_QTY > detail.B_ActQty)
							{
								detail.A_ActQty = detail.B_ActQty;
								f050802.B_DELV_QTY -= detail.B_ActQty;
							}
							else
							{
								detail.A_ActQty = Convert.ToInt32(f050802.B_DELV_QTY);
								f050802.B_DELV_QTY = 0;
							}
						});
					}
				});

				detailDatas = detailDatasTmp.Select(x => new VnrReturnDetailTemp
				{
					Seq = x.Seq,
					ItemSeq = x.ItemSeq,
					ItemCode = x.ItemCode,
					ActQty = x.A_ActQty
				}).ToList();
			}
			else
			{
				detailDatas = (from A in f05030202s
											 join B in f160204List
											 on Convert.ToInt32(A.ORD_SEQ) equals B.RTN_WMS_SEQ
											 select new VnrReturnDetailTemp
											 {
												 Seq = Convert.ToInt32(A.ORD_SEQ),
												 ItemSeq = B.RTN_VNR_SEQ.ToString(),
												 ItemCode = B.ITEM_CODE,
												 ActQty = Convert.ToInt32(A.A_DELV_QTY),
											 }).ToList();
			}

			List<VnrReturnDetail> result = new List<VnrReturnDetail>();
			if (detailDatas.Any())
			{
				result = detailDatas.GroupBy(x => new { x.ItemSeq, x.ItemCode })
															.Select(x => new VnrReturnDetail
															{
																ItemSeq = x.Key.ItemSeq.PadLeft(2, '0'),
																ItemCode = x.Key.ItemCode,
																ActQty = x.Sum(z => z.ActQty),
																MakeNoDetails = f055004s.Where(z => x.Select(y => y.Seq).Contains(z.Seq) && z.ItemCode == x.Key.ItemCode)
																												.Select(z => new VnrReturnMakeNoDetail
																												{
																													MakeNo = z.MakeNo,
																													MakeNoQty = z.Qty,
																													SnList = f055002s.Where(y => y.ITEM_CODE == x.Key.ItemCode && z.Seq == Convert.ToInt32(y.ORD_SEQ)).OrderBy(y => y.SERIAL_NO).Select(y => y.SERIAL_NO).ToList()
																												}).ToList()
															}).ToList();
			}

			return result;
		}

		public IQueryable<F160204> GetDatasByExportVendorReturn(string dcCode, string gupCode, string custCode, List<string> sourceNos)
		{
			return _db.F160204s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			sourceNos.Contains(x.RTN_WMS_NO));
		}
	}
}
