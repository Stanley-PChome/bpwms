using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F25
{
	public partial class F2501Repository : RepositoryBase<F2501, Wms3plDbContext, F2501Repository>
	{
		public F2501Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="batchNo"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetDatasByBatchNo(string gupCode, string custCode, string batchNo)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.BATCH_NO == batchNo
							select a;
			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="boxSerial"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetDatasByBoxSerial(string gupCode, string custCode, string boxSerial)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.BOX_SERIAL == boxSerial
							select a;

			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="batchNo"></param>
		/// <param name="boxSerial"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetDatasByBatchNoAndBoxSerial(string gupCode, string custCode, string batchNo, string boxSerial)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.BATCH_NO == batchNo
							&& a.BOX_SERIAL == boxSerial
							select a;
			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="caseNo"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetDatasByCaseNo(string gupCode, string custCode, string caseNo)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.CASE_NO == caseNo
							select a;
			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="batchNo"></param>
		/// <returns></returns>
		public F2501 GetFirstDataByBatchNo(string gupCode, string custCode, string batchNo)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.BATCH_NO == batchNo
							select a;
			q = q.OrderBy(c => c.GUP_CODE).ThenBy(c => c.CUST_CODE).ThenBy(c => c.SERIAL_NO);
			return q.FirstOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="boxSerial"></param>
		/// <returns></returns>
		public F2501 GetFirstDataByBoxSerial(string gupCode, string custCode, string boxSerial)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.BOX_SERIAL == boxSerial
							select a;
			q = q.OrderBy(c => c.GUP_CODE).ThenBy(c => c.CUST_CODE).ThenBy(c => c.SERIAL_NO);
			return q.FirstOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="caseNo"></param>
		/// <returns></returns>
		public F2501 GetFirstDataByCaseNo(string gupCode, string custCode, string caseNo)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.CASE_NO == caseNo
							select a;
			q = q.OrderBy(c => c.GUP_CODE).ThenBy(c => c.CUST_CODE).ThenBy(c => c.SERIAL_NO);
			return q.FirstOrDefault();
		}

	

		/// <summary>
		/// 
		/// </summary>
		/// <param name="custCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="serialNoBegin"></param>
		/// <param name="serialNoEnd"></param>
		/// <param name="validDateBegin"></param>
		/// <param name="validDateEnd"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetF2501DataBySerialNoAndValidDate(string custCode, string gupCode, string serialNoBegin,
			string serialNoEnd, DateTime? validDateBegin, DateTime? validDateEnd)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							select a;

			if (!string.IsNullOrWhiteSpace(serialNoBegin) && string.IsNullOrWhiteSpace(serialNoEnd))
			{
				q = q.Where(c => c.SERIAL_NO == serialNoBegin);
			}
			else if (!string.IsNullOrWhiteSpace(serialNoBegin) && !string.IsNullOrWhiteSpace(serialNoEnd))
			{
				q = q.Where(c => string.Compare(c.SERIAL_NO, serialNoBegin) >= 0 && string.Compare(c.SERIAL_NO, serialNoEnd) <= 0);
			}

			if (validDateBegin.HasValue)
			{
				q = q.Where(c => c.VALID_DATE >= validDateBegin.Value);
			}
			if (validDateEnd.HasValue)
			{
				q = q.Where(c => c.VALID_DATE <= validDateEnd);
			}
			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="combinNo"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetDatasByCombinNo(string gupCode, string custCode, long combinNo)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.COMBIN_NO == combinNo
							select a;
			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="serialNos"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetF2501sByBoundleItemCode(string gupCode, string custCode, IEnumerable<string> serialNos)
		{
			var q = from a in _db.F2501s
							join k in _db.F2501s on new { a.GUP_CODE, a.CUST_CODE, a.BOUNDLE_ITEM_CODE }
							equals new { k.GUP_CODE, k.CUST_CODE, k.BOUNDLE_ITEM_CODE }
							where k.GUP_CODE == gupCode
							&& k.CUST_CODE == custCode
							&& serialNos.Contains(k.SERIAL_NO)
							select a;
			return q;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="serialNos">要撈出的序號</param>
		/// <returns></returns>
		public IQueryable<F2501> GetF2501s(string gupCode, string custCode, IEnumerable<string> serialNos)
		{
            List<string> tmpserialNos=new List<string>();
            foreach (var item in serialNos)
                tmpserialNos.Add(item.ToUpper());

            var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& tmpserialNos.Contains(a.SERIAL_NO.ToUpper())
							select a;
			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <param name="postingStatus"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetF2501sByF16140101(string dcCode, string gupCode, string custCode, string returnNo, string postingStatus)
		{
			var q = from a in _db.F16140101s
							join b in _db.F2501s on new { a.GUP_CODE, a.CUST_CODE, a.SERIAL_NO }
							equals new { b.GUP_CODE, b.CUST_CODE, b.SERIAL_NO }
							where a.DC_CODE == dcCode
							&& a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& a.RETURN_NO == returnNo
							&& !string.IsNullOrEmpty(a.SERIAL_NO)
							&& a.ISPASS == "1"
							&& b.STATUS != postingStatus
							select b;
			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="snList"></param>
		/// <returns></returns>
		public IQueryable<F2501> GetDatas(string gupCode, string custCode, List<string> snList)
		{
			var q = from a in _db.F2501s
							where a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& snList.Contains(a.SERIAL_NO)
							select a;

			return q;
		}

		public IQueryable<P2501Data> GetDataPprocessing2(string custCode, string gupCode, string[] snList)
		{
			var f2501s = _db.F2501s.AsNoTracking().Where(x => x.CUST_CODE == custCode
																								&& x.GUP_CODE == gupCode);
			if (snList.Length > 0)
			{
				f2501s = f2501s.Where(x => snList.Contains(x.SERIAL_NO));
			}

			var result = f2501s.Select(x => new P2501Data
			{
				CustNo = x.CUST_CODE,
				ItemNo = x.ITEM_CODE,
				Sn = x.SERIAL_NO,
				ValidDate = x.VALID_DATE,
				Status = x.STATUS
			});

			return result;
		}

		/// <summary>
		/// 取得商品序號清單
		/// </summary>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<GetAllocItemSn> GetSnList(string dcCode, string gupCode, string custCode, List<string> snList)
		{
			var result = _db.F2501s.AsNoTracking().Where(x => x.CUST_CODE == custCode
																								&& x.GUP_CODE == gupCode
																								&& snList.Contains(x.SERIAL_NO))
																						.Select(x => new GetAllocItemSn
																						{
																							ItemNo = x.ITEM_CODE,
																							Sn = x.SERIAL_NO,
																							ValidDate = x.VALID_DATE,
																							Status = x.STATUS
																						});

			return result;
		}

		/// <summary>
		/// 取得商品序號清單
		/// </summary>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public IQueryable<GetAllocItemSn> GetSnList(string gupCode, string custCode, List<string> itemCode, List<string> snList)
		{
			//序號綁儲位的品號
			var bundleSerialItemCodes = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && itemCode.Contains(x.ITEM_CODE) && x.BUNDLE_SERIALLOC == "1").Select(x => x.ITEM_CODE).ToList();
			//序號商品或一般商品
			var notbundleSerialItemCodes = itemCode.Except(bundleSerialItemCodes);
			//序號綁儲位商品序號
			var bundleSerials = (from o in _db.F2501s.AsNoTracking()
													 where o.GUP_CODE == gupCode &&
																 o.CUST_CODE == custCode &&
																 bundleSerialItemCodes.Contains(o.ITEM_CODE) &&
																 snList.Contains(o.SERIAL_NO)
													 select o);
			// 序號商品序號
			var notbundleSerials = (from o in _db.F2501s.AsNoTracking()
															where o.GUP_CODE == gupCode &&
																		o.CUST_CODE == custCode &&
																		notbundleSerialItemCodes.Contains(o.ITEM_CODE) &&
																		o.STATUS == "A1"
															select o);
			var serials = bundleSerials.Union(notbundleSerials);

			var result = serials.Select(x => new GetAllocItemSn
			{
				ItemNo = x.ITEM_CODE,
				Sn = x.SERIAL_NO,
				ValidDate = x.VALID_DATE.Value,
				Status = x.STATUS
			});
			return result;
		}

		/// <summary>
		/// 取得序號清單
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemNo"></param>
		/// <param name="serialNo"></param>
		/// <returns></returns>
		public IQueryable<GetSerialRes> GetP810109Data(string gupCode, string custCode, string itemNo, string serialNo)
		{
			var f2501s = _db.F2501s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			var f1903s = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && f2501s.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE));

			if (!string.IsNullOrWhiteSpace(itemNo))
			{
				var itemCodes = f1903s.Where(x =>
				x.GUP_CODE == gupCode &&
				x.CUST_CODE == custCode &&
				(x.ITEM_CODE == itemNo ||
				x.EAN_CODE1 == itemNo ||
				x.EAN_CODE2 == itemNo ||
				x.EAN_CODE3 == itemNo)).Select(x => x.ITEM_CODE).ToList();

				f2501s = f2501s.Where(x => itemCodes.Contains(x.ITEM_CODE));
				f1903s = f1903s.Where(x => itemCodes.Contains(x.ITEM_CODE));
			}

			if (!string.IsNullOrWhiteSpace(serialNo))
				f2501s = f2501s.Where(x => x.SERIAL_NO == serialNo);

			var f1908s = _db.F1908s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && f2501s.Select(z => z.VNR_CODE).Contains(x.VNR_CODE));

			var f000904s = _db.F000904s.AsNoTracking().Where(x => x.TOPIC == "F2501" && x.SUBTOPIC == "STATUS");

			var data = from A in f2501s
								 join B in f1903s
								 on new { A.ITEM_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.ITEM_CODE, B.GUP_CODE, B.CUST_CODE }
								 join C in f1908s
								 on new { A.VNR_CODE, A.GUP_CODE, A.CUST_CODE } equals new { C.VNR_CODE, C.GUP_CODE, C.CUST_CODE } into subC
								 from C in subC.DefaultIfEmpty()
								 join D in f000904s
								 on A.STATUS equals D.VALUE
								 select new GetSerialRes
								 {
									 CustNo = custCode,
									 Sn = A.SERIAL_NO,
									 ItemNo = A.ITEM_CODE,
									 ItemName = B.ITEM_NAME,
									 VnrName = C == null ? string.Empty : $"{C.VNR_CODE} {C.VNR_NAME}",
									 WmsNo = A.WMS_NO,
									 EnterDate = A.IN_DATE,
									 Status = D.NAME
								 };

			return data;
		}

		public IQueryable<F2501> GetDatasForWcsSchedule(string gupCode, string custCode, List<string> itemCodes)
		{
			var statusList = new List<string> { "C1", "D2" };
			return _db.F2501s.AsNoTracking().Where(x =>
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			!statusList.Contains(x.STATUS) &&
			itemCodes.Contains(x.ITEM_CODE));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="snList"></param>
		/// <returns></returns>
		public List<F2501> GetDatasAndCombin(string gupCode, string custCode, List<string> snList)
		{
			var f2501s = _db.F2501s.Where(a => a.GUP_CODE == gupCode
							&& a.CUST_CODE == custCode
							&& snList.Contains(a.SERIAL_NO)).ToList();

			var combinF2501s = new List<F2501>();
			var combins = f2501s.Where(x => x.COMBIN_NO != null).Select(x => x.COMBIN_NO).ToList();
			if (combins.Any())
			{
				combinF2501s = _db.F2501s.Where(a => a.GUP_CODE == gupCode &&
														a.CUST_CODE == custCode &&
														combins.Contains(a.COMBIN_NO)).ToList();

				f2501s.AddRange(combinF2501s);
				f2501s = f2501s.Distinct().ToList();
			}

			return f2501s;
		}

		public IQueryable<F2501> GetSnByItemStatus(string gupCode, string custCode, string itemCode, string status)
		{
			var f2501s = _db.F2501s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.ITEM_CODE == itemCode);

			if (!string.IsNullOrWhiteSpace(status))
				f2501s = f2501s.Where(x => x.STATUS == status);

			return f2501s;
		}
	}
}
