using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F70;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F70
{
	[TestClass]
	public class F700101RepositoryTest : BaseRepositoryTest
	{
		private F700101Repository _F700101Repo;
		public F700101RepositoryTest()
		{
			_F700101Repo = new F700101Repository(Schemas.CoreSchema);
		}
		//[TestMethod]
		//public void InsertStockByDate()
		//{
		//    _F700101Repo.InsertStockByDate(DateTime.Today);
		//}

		[TestMethod]
		public void DeleteF700101()
		{
			_F700101Repo.DeleteF700101("", "A001");//避免刪除
		}
		[TestMethod]
		public void GetF700101ByDistrCarNo()
		{
			//SELECT * FROM F700101  WHERE  DISTR_CAR_NO = 'ZC2017022000001' AND DC_CODE= '001'
			var r = _F700101Repo.GetF700101ByDistrCarNo("ZC2017022000001", "001");
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetDatas()
		{
			var r = _F700101Repo.GetDatas("001", "01", "010001", DateTime.Parse("2017/02/20"), "11:05", "TCAT", "16:30");
			Trace.Write(JsonSerializer.Serialize(r));
		}

		[TestMethod]
		public void GetDistrCarDatas()
		{
			var r = _F700101Repo.GetDistrCarDatas("001", "01", "010001", DateTime.Parse("2017/02/20"), DateTime.Parse("2017/02/20"), "TCAT");//allId
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetF700101ByWmsOrdNo()
		{
			var r = _F700101Repo.GetF700101ByWmsOrdNo("001", "01", "010001", "ZC2017022000001");
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void UpdateF700101StatusToCancel()
		{
			_F700101Repo.UpdateF700101StatusToCancel("AAA001", new List<string>() { "" });//避免真UPDATE
		}
		[TestMethod]
		public void GetSettleDatas()
		{
			var r = _F700101Repo.GetSettleDatas("001", "01", "010001", DateTime.Parse("2017/02/20"), DateTime.Parse("2017/02/20"), DateTime.Parse("2017/02/20"));
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void FromF700102()
		{
			var r = _F700101Repo.FromF700102("001", "01", "010001", "O2017022100156");
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void BulkUpdatStatus()
		{
			//if (property.Name == "DC_CODE")
			//    parms[2] = property.GetValue(d);
			//if (property.Name == "WMS_            //這支測試有點麻煩，比較怪異的輸入List<T>ORD_NO")
			//    parms[3] = property.GetValue(d);

			var datas = new List<Shared.Entities.EgsReturnConsign> { new Shared.Entities.EgsReturnConsign { DC_CODE = "001", WMS_ORD_NO = "" } };
			_F700101Repo.BulkUpdatStatus(datas);
			//Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetF700101Datas()
		{
			//var r = _F700101Repo.GetF700101Datas("001",string allId,string delvTmpr,DateTime? takeDateFrom,DateTime? takeDateTo,string distrUse,string[] consignNos,string[] detailNos);
			//Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetConsignData()
		{
			var r = _F700101Repo.GetConsignData("001", "ZC2018010400012");
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetEgsReturnConsignsByHaveWmsNoBack()
		{
			var r = _F700101Repo.GetEgsReturnConsignsByHaveWmsNoBack(new Shared.Entities.EgsReturnConsignParam() { DcCode = "001" });
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetEgsReturnConsignsByNoWmsNo()
		{
			var r = _F700101Repo.GetEgsReturnConsignsByNoWmsNo(new Shared.Entities.EgsReturnConsignParam() { DcCode = "001" });
			Trace.Write(JsonSerializer.Serialize(r));
		}

		[TestMethod]
		public void GetDatas_2()
		{
			var r = _F700101Repo.GetDatas("001", "01", "010001", new List<string>() { "O2017022100156" });
			Trace.Write(JsonSerializer.Serialize(r));
		}

		[TestMethod]
		public void GetPackagedWmsOrdNos()
		{
			var r = _F700101Repo.GetPackagedWmsOrdNos("001", new List<string>() { "ZC2017030800049" });
			Trace.Write(JsonSerializer.Serialize(r));
		}
	}
}
