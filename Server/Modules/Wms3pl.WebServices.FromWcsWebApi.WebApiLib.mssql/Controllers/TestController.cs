using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Wms3pl.Datas.Shared.ApiEntities;

namespace Wms3pl.WebServices.FromWcsWebApi.WebApiLib.mssql.Controllers
{
	public class TestResult
	{
		public string Code { get; set; }
		public string Msg { get; set; }
		public object Data { get; set; }
	}
	public class TestController : ApiController
	{
		[Authorize]
		[HttpPost]
		[Route("api/Test/OutWarehouseReceipt")]
		public TestResult OutWarehouseReceipt(OutWarehouseReceiptReq req)
		{
			return new TestResult { Code="200",Msg= "WMS出庫回傳測試資料接收結果", Data = req };
		}
		[Authorize]
		[HttpPost]
		[Route("api/Test/InWarehouseReceipt")]
		public TestResult InWarehouseReceipt(InWarehouseReceiptReq req)
		{
			return new TestResult { Code = "200", Msg = "WMS入庫回傳測試資料接收結果", Data = req };
		}

		[Authorize]
		[HttpGet]
		[Route("api/Test/GetTestData")]
		public TestResult GetTestData()
		{
			return new  TestResult{ Code = "200", Msg = "測試Get成功" };
		}

		[Authorize]
		[HttpPost]
		[Route("api/Test/PostTestData")]
		public TestResult PostTestData()
		{
			return new TestResult { Code = "200", Msg = "測試POST成功" };
		}

	}
}
