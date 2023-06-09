using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.P70ExDataService
{
	public partial class P70ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<F700101EX> GetF700101EX(String dcCode, String gupCode, String custCode, String delvDate, String takeDateFrom, String takeDateTo, String distrCarNo, String ordType, String wmsNo, String status, String delvCompany, String chkoutTime, String spCar, String custOrdNos)
		{
			return CreateQuery<F700101EX>("GetF700101EX")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("takeDateFrom", takeDateFrom)
						.AddQueryExOption("takeDateTo", takeDateTo)
						.AddQueryExOption("distrCarNo", distrCarNo)
						.AddQueryExOption("ordType", ordType)
						.AddQueryExOption("wmsNo", wmsNo)
						.AddQueryExOption("status", status)
						.AddQueryExOption("delvCompany", delvCompany)
						.AddQueryExOption("chkoutTime", chkoutTime)
						.AddQueryExOption("spCar", spCar)
						.AddQueryExOption("custOrdNos", custOrdNos);
		}

		public IQueryable<F700101EX> GetF700101ByDistrCarNo(String distrCarNo, String dcCode)
		{
			return CreateQuery<F700101EX>("GetF700101ByDistrCarNo")
						.AddQueryExOption("distrCarNo", distrCarNo)
						.AddQueryExOption("dcCode", dcCode);
		}

		public IQueryable<F700201Ex> GetF700201SearchData(String gupCode, String custCode, String dcCode, String compDateBegin, String compDateEnd, String compNo, String depId, String compType, String status)
		{
			return CreateQuery<F700201Ex>("GetF700201SearchData")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("compDateBegin", compDateBegin)
						.AddQueryExOption("compDateEnd", compDateEnd)
						.AddQueryExOption("compNo", compNo)
						.AddQueryExOption("depId", depId)
						.AddQueryExOption("compType", compType)
						.AddQueryExOption("status", status);
		}

		public IQueryable<F700102Data> GetF700102Datas(String dcCode, String distrCarNo)
		{
			return CreateQuery<F700102Data>("GetF700102Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("distrCarNo", distrCarNo);
		}

		public IQueryable<P700104WmsNoDetialData> GetCheckWmsNoDatas(String dcCode, String gupCode, String custCode, String distrCarNo, String ordType, String wmsNo, String isAdd)
		{
			return CreateQuery<P700104WmsNoDetialData>("GetCheckWmsNoDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("distrCarNo", distrCarNo)
						.AddQueryExOption("ordType", ordType)
						.AddQueryExOption("wmsNo", wmsNo)
						.AddQueryExOption("isAdd", isAdd);
		}

		public IQueryable<F700501Ex> GetF700501Ex(String dcCode, String dateBegin, String dateEnd, String scheduleType)
		{
			return CreateQuery<F700501Ex>("GetF700501Ex")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("dateBegin", dateBegin)
						.AddQueryExOption("dateEnd", dateEnd)
						.AddQueryExOption("scheduleType", scheduleType);
		}

		public IQueryable<F700501Ex> GetF700501ExForScheduleView(String dcCode, String dateBegin, String dateEnd, String workGroup)
		{
			return CreateQuery<F700501Ex>("GetF700501ExForScheduleView")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("dateBegin", dateBegin)
						.AddQueryExOption("dateEnd", dateEnd)
						.AddQueryExOption("workGroup", workGroup);
		}

		public ExecuteResult UpdateF700501ProcessedStatus(String dcCode, String scheduleNo)
		{
			return CreateQuery<ExecuteResult>("UpdateF700501ProcessedStatus")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("scheduleNo", scheduleNo).ToList().FirstOrDefault();
		}

		public IQueryable<WmsDistrCarItem> GetWmsDistrCarItems(String dcCode, String gupCode, String custCode, String status, String delvTmpr, DateTime? delvDate, String pickTime, DateTime? takeDate, String takeTime, String wmsOrdNo, String sourceNo, String custOrdNo, String distrCarNo, String sa)
		{
			return CreateQuery<WmsDistrCarItem>("GetWmsDistrCarItems")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("status", status)
						.AddQueryExOption("delvTmpr", delvTmpr)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("takeDate", takeDate)
						.AddQueryExOption("takeTime", takeTime)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("sourceNo", sourceNo)
						.AddQueryExOption("custOrdNo", custOrdNo)
						.AddQueryExOption("distrCarNo", distrCarNo)
						.AddQueryExOption("sa", sa);
		}
	}
}

