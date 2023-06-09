using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebServices.PdaWebApi.Controllers
{
	public class TestController : ApiController
	{
		[Authorize]
		[HttpGet]
		[Route("api/Test/Test")]
		public string Test()
		{
			return "test";
		}
		[HttpGet]
		[Route("api/Test/Test1")]
		public testClass Test1()
		{
			return new testClass { title = "123" };
		}
	}

	public class testClass
	{
		public string title { get; set; }
	}
}
