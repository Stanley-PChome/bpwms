using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
	[TestClass]
	public class F910201RepositoryTest : BaseRepositoryTest
	{
		private readonly F910201Repository _f910201Repository;
		public F910201RepositoryTest()
		{
			_f910201Repository = new F910201Repository(Schemas.CoreSchema);
		}


		[TestMethod]
		public void GetProcessItemsByBom()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			Output(new { dcCode, gupCode, custCode, processNo });
			var result = _f910201Repository.GetProcessItemsByBom(dcCode, gupCode, custCode, processNo);
			Output(result);
		}

		[TestMethod]
		public void GetProcessItemsNonBom()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			Output(new { dcCode, gupCode, custCode, processNo });
			var result = _f910201Repository.GetProcessItemsNonBom(dcCode, gupCode, custCode, processNo);
			Output(result);
		}

		[TestMethod]
		public void GetProduceLineStatusItems()
		{
			string dcCode = "";
			DateTime finishDate = DateTime.Today;
			Output(new { dcCode, finishDate });
			var result = _f910201Repository.GetProduceLineStatusItems(dcCode, finishDate);
			Output(result);
		}

		[TestMethod]
		public void GetWorkProcessOverFinishTimeByDc()
		{
			string dcCode = "";
			DateTime finishDate = DateTime.Today;
			Output(new { dcCode, finishDate });
			var result = _f910201Repository.GetWorkProcessOverFinishTimeByDc(dcCode, finishDate);
			Output(result);
		}
		[TestMethod]
		public void GetDcWmsNoDateItems()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			DateTime begFinishDate = DateTime.Now;
			DateTime endFinishDate = DateTime.Now.AddDays(1);
			Output(new { dcCode, gupCode, custCode, begFinishDate, endFinishDate });
			var result = _f910201Repository.GetDcWmsNoDateItems(dcCode, gupCode, custCode, begFinishDate, endFinishDate);
			Output(result);
		}

		[TestMethod]
		public void GetProcessDatas()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			DateTime? crt_SDate = null;
			DateTime? crt_EDate = null;
			string outSourceId = "";
			Output(new { dcCode, gupCode, custCode, crt_SDate, crt_EDate, outSourceId });
			var result = _f910201Repository.GetProcessDatas(dcCode, gupCode, custCode, crt_SDate, crt_EDate, outSourceId);
			Output(result);
		}

		[TestMethod]
		public void GetQuoteDatas()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			DateTime settleDate = DateTime.Today;
			List<string> quotes = new List<string>();
			Output(new { dcCode, gupCode, custCode, settleDate, quotes });
			var result = _f910201Repository.GetQuoteDatas(dcCode, gupCode, custCode, settleDate, quotes);
			Output(result);
		}

		private void Output(object obj)
		{
			Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
		}
	}
}
