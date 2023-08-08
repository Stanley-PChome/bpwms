
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using System.Data.Objects;
using Wms3pl.Datas.F25;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050101Service
	{
		private WmsTransaction _wmsTransaction;
		public P050101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F051201Data> GetF051201Datas(string dcCode, string gupCode, string custCode, DateTime delvDate,
			string isPrinted, string ordType)
		{
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			return f051201Repo.GetF051201Datas(dcCode, gupCode, custCode, delvDate, isPrinted, ordType);
		}

		public IQueryable<F051202Data> GetF051202Datas(string dcCode, string gupCode, string custCode, string delvDate,
			string pickTime, string ordType)
		{
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			return f051202Repo.GetF051202Datas(dcCode, gupCode, custCode, delvDate, pickTime, ordType);
		}

		public IQueryable<F051201SelectedData> GetF051201SelectedDatas(string dcCode, string gupCode, string custCode,
			string delvDate, string pickTime, string ordType)
		{
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			return f051201Repo.GetF051201SelectedDatas(dcCode, gupCode, custCode, delvDate, pickTime, ordType, "1");
		}

		/// <summary>
		/// 揀貨單報表
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">雇主</param>
		/// <param name="delvDate">批次日期</param>
		/// <param name="pickTime">批次時段</param>
		/// <param name="pickOrdNo">揀貨單號(以逗點分隔) NULL OR 空白 表示全部</param>
		/// <returns></returns>
		public IQueryable<F051201ReportDataA> GetF051201ReportDataAs(string dcCode, string gupCode, string custCode,
			DateTime delvDate, string pickTime, string pickOrdNo)
		{
			return GetF051201ReportDataAs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, "0");
		}

		public IQueryable<F051201ReportDataA> GetF051201ReportDataAs(string dcCode, string gupCode, string custCode,
			DateTime delvDate, string pickTime, string pickOrdNo, string ordType = null)
		{
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var isShowValidDate = f1909Repo.GetDatasByTrueAndCondition(o=>o.GUP_CODE == gupCode && o.CUST_CODE == custCode).FirstOrDefault().ISPICKSHOWVALIDDATE == "1";
			var f051201ReportDataAList = f051201Repo.GetF051201ReportDataAs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, isShowValidDate, ordType);

			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var serialNos = f051201ReportDataAList.Where(x => !string.IsNullOrEmpty(x.SERIAL_NO)).Select(x => x.SERIAL_NO).Distinct().ToList();

			if (serialNos.Any())
			{
				var f2501s = f2501Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
												&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
												&& serialNos.Contains(x.SERIAL_NO)
												&& x.COMBIN_NO != null
												&& x.BOUNDLE_ITEM_CODE == null).ToList();

				if (f2501s.Any())
				{
					var f2501sByBoundleItemCode = f2501Repo.GetF2501sByBoundleItemCode(gupCode, custCode, f2501s.Select(x => x.SERIAL_NO).Distinct());

					foreach (var f051201Report in f051201ReportDataAList.Where(x => !string.IsNullOrEmpty(x.SERIAL_NO)))
					{
						// A+B=C(組合商品)，只顯示A與B序號
						var cF2501 = f2501s.FirstOrDefault(x => x.SERIAL_NO == f051201Report.SERIAL_NO);
						var abF2501s = f2501sByBoundleItemCode.Where(x => x.COMBIN_NO == cF2501.COMBIN_NO && x.BOUNDLE_ITEM_CODE != null).ToList();
						f051201Report.SERIAL_NO = string.Join(Environment.NewLine, abF2501s.OrderBy(x => x.ITEM_CODE).Select(x => x.SERIAL_NO));
					}
				}
			}

			return f051201ReportDataAList;
		}

		/// <summary>
		/// 播種單報表
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">雇主</param>
		/// <param name="delvDate">批次日期</param>
		/// <param name="pickTime">批次時段</param>
		/// <param name="pickOrdNo">揀貨單號(以逗點分隔) NULL OR 空白 表示全部</param>
		/// <returns></returns>
		public IQueryable<F051201ReportDataB> GetF051201ReportDataBs(string dcCode, string gupCode, string custCode,
		string delvDate, string pickTime, string pickOrdNo)
		{
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			return f051201Repo.GetF051201ReportDataBs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, "0");
		}

		/// <summary>
		/// 更新列印狀態
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">雇主</param>
		/// <param name="delvDate">批次日期</param>
		/// <param name="pickTime">批次時段</param>
		/// <param name="userId">使用者id</param>
		/// <param name="userName">使用者名稱</param>
		/// <returns>回傳已列印的單號</returns>
		public ExecuteResult UpdateF051201IsPrinted(string dcCode, string gupCode, string custCode,
			string delvDate, string pickTime, string userId, string userName, out List<string> pickOrdNoList,
            bool isB2B = true, List<string> exceptDevicePickOrdNoList = null, bool isDevice = false,
            List<F051201> deviceF051201s = null)
		{
			var dtmdelvDate = DateTime.Parse(delvDate);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
            var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);

			var ordType = "0";
			if (!isB2B)
				ordType = "1";
			var items = f051201Repo.Filter(o => o.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
											&& o.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
											&& o.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
											&& o.DELV_DATE == dtmdelvDate
											&& o.PICK_TIME == EntityFunctions.AsNonUnicode(pickTime)
											&& o.ORD_TYPE == EntityFunctions.AsNonUnicode(ordType)
											&& o.PICK_TOOL == "1"
											&& o.PICK_STATUS == 0).ToList();


			//if (exceptDevicePickOrdNoList != null)
			//	items = items.Where(o => !exceptDevicePickOrdNoList.Contains(o.PICK_ORD_NO)).ToList();

			if (isDevice)
				items = deviceF051201s;

			pickOrdNoList = items.Select(x => x.PICK_ORD_NO).ToList();

			if (!pickOrdNoList.Any())
				return new ExecuteResult(true);

			f051201Repo.UpdateFieldsInWithTrueAndCondition(SET: new
														   {
															   ISPRINTED = "1",
															   PICK_STATUS = 1,
															   //PICK_FINISH_DATE = DateTime.Now,
															   //PICK_STAFF = userId,
															   //PICK_NAME = userName,
															   DEVICE_PRINT = (isDevice) ? "1" : "0"
														   },
														   WHERE: o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode,
														   InFieldName: x => x.PICK_ORD_NO,
														   InValues: pickOrdNoList);

			//f051202Repo.UpdateAPickQtyBeBPickQty(dcCode, gupCode, custCode, pickOrdNoList);
			//f1511Repo.UpdateAPickQtyBeBPickQty(dcCode, gupCode, custCode, pickOrdNoList);

			sharedService.UpdatePickOrdNoLocVolumn(dcCode, gupCode, custCode, pickOrdNoList);
			return new ExecuteResult { IsSuccessed = true };
		}

        public IQueryable<F051201ReportDataA> GetF051201ReportDataIsDiviceAsForB2C(string dcCode, string gupCode, string custCode,
            DateTime delvDate, string pickTime, string pickOrdNo, string ordType = null)
        {
            var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
            var isShowValidDate = f1909Repo.GetDatasByTrueAndCondition(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode).FirstOrDefault().ISPICKSHOWVALIDDATE == "1";
            var f051201ReportDataAList = f051201Repo.GetF051201ReportDataAs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, isShowValidDate, ordType);

            var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
            var serialNos = f051201ReportDataAList.Where(x => !string.IsNullOrEmpty(x.SERIAL_NO)).Select(x => x.SERIAL_NO).Distinct().ToList();

            if (serialNos.Any())
            {
                var f2501s = f2501Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
                                                && x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
                                                && serialNos.Contains(x.SERIAL_NO)
                                                && x.COMBIN_NO != null
                                                && x.BOUNDLE_ITEM_CODE == null).ToList();

                if (f2501s.Any())
                {
                    var f2501sByBoundleItemCode = f2501Repo.GetF2501sByBoundleItemCode(gupCode, custCode, f2501s.Select(x => x.SERIAL_NO).Distinct());

                    foreach (var f051201Report in f051201ReportDataAList.Where(x => !string.IsNullOrEmpty(x.SERIAL_NO)))
                    {
                        // A+B=C(組合商品)，只顯示A與B序號
                        var cF2501 = f2501s.FirstOrDefault(x => x.SERIAL_NO == f051201Report.SERIAL_NO);
                        var abF2501s = f2501sByBoundleItemCode.Where(x => x.COMBIN_NO == cF2501.COMBIN_NO && x.BOUNDLE_ITEM_CODE != null).ToList();
                        f051201Report.SERIAL_NO = string.Join(Environment.NewLine, abF2501s.OrderBy(x => x.ITEM_CODE).Select(x => x.SERIAL_NO));
                    }
                }
            }

            return f051201ReportDataAList;
        }

        
    }
}

