using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.WebApiLib.Controllers
{
	/// <summary>
	/// API 範例
	/// </summary>
	public class DemoController : ApiController
	{
		[Authorize]
		[HttpGet]
		[Route("api/Demo/Test")]
		public string Test()
		{
			return "test";
		}
		//[Authorize]
		[HttpGet]
		[Route("api/Demo/Test1")]
		public testClass Test1()
		{
			return new testClass { title = "123" };
		}

		[Authorize]
		[HttpPut]
		[Route("api/Demo/Test2")]
		public testClass Test2(testClass test)
		{
			return new testClass { title = "123" };
		}

		public class testClass
		{
			public string title { get; set; }
		}
	}
}
