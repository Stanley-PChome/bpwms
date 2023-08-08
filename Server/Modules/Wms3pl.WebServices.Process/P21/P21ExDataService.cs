using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P21.ExDataSources;
using Wms3pl.WebServices.Process.P21.Services;

namespace Wms3pl.WebServices.Process.P21
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P21ExDataService : DataService<P21ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region P2115010000 今日工作指示查詢

		[WebGet]
		public IQueryable<WorkList> GetWorkListDatas(string dcCode, string gupCode, string custCode, string apType,
			string account)
		{
			var srv = new P211501Service();
			return srv.GetWorkListDatas(dcCode, gupCode, custCode, apType, account);
		}


        #endregion

        #region P2115010000 今日出貨工作指示查詢

       

        [WebGet]
		public IQueryable<WorkList> GetChartListDatas(string dcCode, string gupCode, string custCode, string apType,
			string account)
		{
			var srv = new P211501Service();
			return srv.GetChartListDatas(dcCode, gupCode, custCode, apType, account);
		}

        #endregion
        #region P2102010000 健保追溯報保

        [WebGet]
        public IQueryable<HealthInsuranceReport> GetHealthInsurancePurchaseData(string dcCode, string gupCode, string custCode, DateTime? startDate, DateTime? endDate,string itemCodes)
        {
            var f151001Rep = new F151001Repository(Schemas.CoreSchema);
            var datas = f151001Rep.GetHealthInsurancePurchaseData(dcCode, gupCode, custCode, startDate, endDate,
                itemCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            return datas.AsQueryable();

        }

        [WebGet]
        public IQueryable<HealthInsuranceReport> GetHealthInsuranceSalesData(string dcCode, string gupCode, string custCode, DateTime? startDate, DateTime? endDate, string itemCodes)
        {
            var f050801Rep = new F050801Repository(Schemas.CoreSchema);
            var datas = f050801Rep.GetHealthInsuranceSalesData(dcCode, gupCode, custCode, startDate, endDate, 
                itemCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            return datas.AsQueryable();
        }


		#endregion

		#region 分揀異常回報查詢
		[WebGet]
		public IQueryable<F060802Data> GetF060802Data(string dcCode,string gupCode,string custCode,DateTime? beginCreatDate, DateTime? endCreatDate,string sorterCode, string abnormalCode, string abnormalType)
		{
			var f060802Repo = new F060802Repository(Schemas.CoreSchema);
			return f060802Repo.GetF060802Data(dcCode, gupCode, custCode, beginCreatDate, endCreatDate, sorterCode,abnormalCode, abnormalType);
		}
		#endregion

		#region 自動倉儲位揀貨異常回報
		[WebGet]
		public IQueryable<F060801Data> GetF060801Datas(string dcCode, string gupCode, string custCode, DateTime? beginCrtDate, DateTime? endCrtDate,
			string warehouseId, string abnormaltype, string shelfcode, string ordercode, string bincode, string skucode)
		{
			var f0600801Repo = new F060801Repository(Schemas.CoreSchema);
			return f0600801Repo.GetF060801Datas(dcCode, gupCode, custCode, beginCrtDate, endCrtDate, warehouseId, abnormaltype, shelfcode, ordercode, bincode, skucode);
		}
		#endregion

		#region 查詢4小時未配庫的出貨單
		[WebGet]
		public IQueryable<UndistributedOrder> GetUndistributedOrder(string dcCode, string gupCode, string custCode,
			string ordType, string fastDealType, string custCost, string ordNo, string custOrdNo, bool onlyShowMoreThanFourHours)
		{
			var f050001Repo = new F050001Repository(Schemas.CoreSchema);
			return f050001Repo.GetUndistributedOrder(dcCode, gupCode, custCode, ordType, fastDealType, custCost, ordNo, custOrdNo, onlyShowMoreThanFourHours);
		}

		[WebGet]
		public IQueryable<NotGeneratedPick> GetNotGeneratedPick(string dcCode, string gupCode, string custCode,
			string ordType, string fastDealType, string custCost, string ordNo, string custOrdNo, bool onlyShowMoreThanFourHours)
		{
			var f050306Repo = new F050306Repository(Schemas.CoreSchema);
			return f050306Repo.GetNotGeneratedPick(dcCode, gupCode, custCode, ordType, fastDealType, custCost, ordNo, custOrdNo, onlyShowMoreThanFourHours);
		}
		#endregion
	}
}
