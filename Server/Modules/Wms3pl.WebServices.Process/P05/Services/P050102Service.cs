
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.WebServices.DataCommon;
using System.Data.Objects;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050102Service
	{
		private WmsTransaction _wmsTransaction;
		public P050102Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}


		public IQueryable<F051201ReportDataA> GetF051201ReportDataAs(string dcCode, string gupCode, string custCode,
			DateTime delvDate, string pickTime, string pickOrdNo)
		{
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			if (string.IsNullOrEmpty(pickOrdNo))
			{
				var items = f051201Repo.Filter(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
																				 o.DELV_DATE == delvDate && o.PICK_TIME == pickTime && o.ORD_TYPE == "1" && o.ISPRINTED == "1" && o.ISDEVICE == "0");
				pickOrdNo = string.Join(",", items.Select(o => o.PICK_ORD_NO).ToList());
			}
			var p050101Service = new P050101Service(_wmsTransaction);
			return p050101Service.GetF051201ReportDataAs(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, "1");
		}

		/// <summary>
		/// 更新揀貨單為PDA
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">雇主</param>
		/// <param name="delvDate">批次日期</param>
		/// <param name="pickTime">批次時段</param>
		/// <param name="deviceCount">臺車筆數</param>
		/// <returns></returns>
		public List<string> UpdateF050102IsDevice(string dcCode, string gupCode, string custCode,
			string delvDate, string pickTime, int deviceCount)
		{
			var devicePickOrdNoList = new List<string>();
			var dtmDelvDate = DateTime.Parse(delvDate);

			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var pickOrdNos = f051201Repo.GetDatasByNoVirturlItem(dcCode, gupCode, custCode, dtmDelvDate, pickTime)
						.Where(o => o.PICK_TOOL == "1" && o.PICK_STATUS == 0 && o.DISP_SYSTEM =="0")
								.Take(deviceCount)
								.Select(x => x.PICK_ORD_NO)
								.ToList();
			if (pickOrdNos.Any())
			{
				f051201Repo.UpdateFieldsInWithTrueAndCondition(SET: new { PICK_TOOL="2" },
															   WHERE: x => x.DC_CODE == dcCode
																		&& x.GUP_CODE == gupCode
																		&& x.CUST_CODE == custCode,
															   InFieldName: x => x.PICK_ORD_NO,
															   InValues: pickOrdNos);
			}
			return pickOrdNos;
		}

		public IQueryable<F700101CarData> GetF700101Data(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime
														, string sourceTye, string ordType)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF700101Data(dcCode, gupCode, custCode, delvDate, pickTime,  sourceTye, ordType);

		}

        public IQueryable<RP0501010004Model> GetF051201SingleStickersReportDataAsForB2C(string dcCode, string gupCode, string custCode,
            DateTime delvDate, string pickTime, string pickOrdNo)
        {
            var f051201Repo = new F051201Repository(Schemas.CoreSchema);
            if (string.IsNullOrEmpty(pickOrdNo))
            {
                var items = f051201Repo.Filter(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
                                                                                 o.DELV_DATE == delvDate && o.PICK_TIME == pickTime && o.ORD_TYPE == "1" && o.ISPRINTED == "1" && o.ISDEVICE == "1");
                pickOrdNo = string.Join(",", items.Select(o => o.PICK_ORD_NO).ToList());
            }
            return f051201Repo.GetF051201SingleStickersReportDataAsForB2C(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, "1");
        }

        //GetF051201batchReportDataAsForB2C
        public IQueryable<P050103ReportData> GetF051201BatchReportDataAsForB2C(string dcCode, string gupCode, string custCode,
            DateTime delvDate, string pickTime, string pickOrdNo)
        {
            var f051201Repo = new F051201Repository(Schemas.CoreSchema);
            if (string.IsNullOrEmpty(pickOrdNo))
            {
                var items = f051201Repo.Filter(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
                                                                                 o.DELV_DATE == delvDate && o.PICK_TIME == pickTime && o.ORD_TYPE == "1" && o.ISPRINTED == "1" && o.ISDEVICE == "0");
                pickOrdNo = string.Join(",", items.Select(o => o.PICK_ORD_NO).ToList());
            }
            return f051201Repo.GetF051201BatchReportDataAsForB2C(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, "1");
        }

        public IQueryable<RP0501010005Model> GetF051201BatchStickersReportDataAsForB2C(string dcCode, string gupCode, string custCode,
           DateTime delvDate, string pickTime, string pickOrdNo)
        {
            var f051201Repo = new F051201Repository(Schemas.CoreSchema);
            if (string.IsNullOrEmpty(pickOrdNo))
            {
                var items = f051201Repo.Filter(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
                                                                                 o.DELV_DATE == delvDate && o.PICK_TIME == pickTime && o.ORD_TYPE == "1" && o.ISPRINTED == "1" && o.ISDEVICE == "1");
                pickOrdNo = string.Join(",", items.Select(o => o.PICK_ORD_NO).ToList());
            }
            return f051201Repo.GetF051201BatchStickersReportDataAsForB2C(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, "1");
        }
    }
}

