using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.ExDataServices.P19ExDataService
{
	public partial class P19ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<Wms3pl.Datas.F19.F1929> GetDatas()
		{
			return CreateQuery<Wms3pl.Datas.F19.F1929>("GetDatas")
						;
		}

		public IQueryable<F1929WithF1909Test> GetF1929WithF1909Tests(String gupCode)
		{
			return CreateQuery<F1929WithF1909Test>("GetF1929WithF1909Tests")
						.AddQueryExOption("gupCode", gupCode);
		}

		public IQueryable<ExecuteResult> UpdateWithTrans1(String gupCode)
		{
			return CreateQuery<ExecuteResult>("UpdateWithTrans1")
						.AddQueryExOption("gupCode", gupCode);
		}

		public IQueryable<ExecuteResult> UpdateWithTrans2(String gupCode)
		{
			return CreateQuery<ExecuteResult>("UpdateWithTrans2")
						.AddQueryExOption("gupCode", gupCode);
		}

		public IQueryable<ExecuteResult> UpdateWithoutTrans1(String gupCode)
		{
			return CreateQuery<ExecuteResult>("UpdateWithoutTrans1")
						.AddQueryExOption("gupCode", gupCode);
		}

		public IQueryable<ExecuteResult> UpdateWithoutTrans2(String gupCode)
		{
			return CreateQuery<ExecuteResult>("UpdateWithoutTrans2")
						.AddQueryExOption("gupCode", gupCode);
		}

		public IQueryable<FunctionShowInfo> GetFunctionShowInfos(String account)
		{
			return CreateQuery<FunctionShowInfo>("GetFunctionShowInfos")
						.AddQueryExOption("account", account);
		}

		public IQueryable<ExecuteResult> DeleteUser(String empId, String userId)
		{
			return CreateQuery<ExecuteResult>("DeleteUser")
						.AddQueryExOption("empId", empId)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<F190101Data> GetF190101MappingTable(String gupId)
		{
			return CreateQuery<F190101Data>("GetF190101MappingTable")
						.AddQueryExOption("gupId", gupId);
		}

		public IQueryable<Wms3pl.Datas.F19.F1954> GetUserFunctions(String account)
		{
			return CreateQuery<Wms3pl.Datas.F19.F1954>("GetUserFunctions")
						.AddQueryExOption("account", account);
		}

		public IQueryable<Wms3pl.Datas.F19.F1954> GetUserFunctionsForPda(String account, String custCode, String dcCode)
		{
			return CreateQuery<Wms3pl.Datas.F19.F1954>("GetUserFunctionsForPda")
						.AddQueryExOption("account", account)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode);
		}

		public IQueryable<String> SaveUserFunctions(String account)
		{
			return CreateQuery<String>("SaveUserFunctions")
						.AddQueryExOption("account", account);
		}


		public IQueryable<ExecuteResult> DeleteP190504(String groupId, String userId)
		{
			return CreateQuery<ExecuteResult>("DeleteP190504")
						.AddQueryExOption("groupId", groupId)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<F1924Data> F1924WithF192403()
		{
			return CreateQuery<F1924Data>("F1924WithF192403")
						;
		}

		public IQueryable<ExecuteResult> UpdateP190505(String groupId, String empId, String userId)
		{
			return CreateQuery<ExecuteResult>("UpdateP190505")
						.AddQueryExOption("groupId", groupId)
						.AddQueryExOption("empId", empId)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<GetUserPassword> GetUserPassword(String empid)
		{
			return CreateQuery<GetUserPassword>("GetUserPassword")
						.AddQueryExOption("empid", empid);
		}

		public IQueryable<EmpWithFuncionName> EmpWithFuncionName(String Empid)
		{
			return CreateQuery<EmpWithFuncionName>("EmpWithFuncionName")
						.AddQueryExOption("Empid", Empid);
		}

		public IQueryable<Wms3pl.Datas.F19.F1912> GetAssignedLoc(String workgroupId, String dcCode)
		{
			return CreateQuery<Wms3pl.Datas.F19.F1912>("GetAssignedLoc")
						.AddQueryExOption("workgroupId", workgroupId)
						.AddQueryExOption("dcCode", dcCode);
		}

		public IQueryable<Wms3pl.Datas.F19.F1912> GetUnAssignedLoc(String workgroupId, String dcCode, String warehouseId, String floor, String startLocCode, String endLocCode)
		{
			return CreateQuery<Wms3pl.Datas.F19.F1912>("GetUnAssignedLoc")
						.AddQueryExOption("workgroupId", workgroupId)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("floor", floor)
						.AddQueryExOption("startLocCode", startLocCode)
						.AddQueryExOption("endLocCode", endLocCode);
		}

		

		public IQueryable<ExecuteResult> DeleteDC(String dcCode)
		{
			return CreateQuery<ExecuteResult>("DeleteDC")
						.AddQueryExOption("dcCode", dcCode);
		}

		public IQueryable<F1905Data> GetPackCase(String gupCode, String custCode, String itemCode, String itemName)
		{
			return CreateQuery<F1905Data>("GetPackCase")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName);
		}

		public ExecuteResult DeleteItem(String gupCode, String custCode, String itemCode)
		{
			return CreateQuery<ExecuteResult>("DeleteItem")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode).ToList().FirstOrDefault();
		}

		public IQueryable<WareHouseIdByWareHouseType> GetWareHouseIdByWareHouseTypeList()
		{
			return CreateQuery<WareHouseIdByWareHouseType>("GetWareHouseIdByWareHouseTypeList")
						;
		}

		public IQueryable<F1947WithF194701> GetF1947WithF194701Datas(String dcCode, String gupCode, String custCode, String delvTime)
		{
			return CreateQuery<F1947WithF194701>("GetF1947WithF194701Datas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvTime", delvTime);
		}

		public IQueryable<UserWarehouse> GetUserWarehouse(String userId, String gupCode, String custCode)
		{
			return CreateQuery<UserWarehouse>("GetUserWarehouse")
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public IQueryable<F190301Data> GetItemPack(String gupCode, String itemCode, String itemName)
		{
			return CreateQuery<F190301Data>("GetItemPack")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName);
		}

		public IQueryable<F197001Data> GetF197001Data(String gupCode, String custCode, String labelCode, String itemCode, String vnrCode)
		{
			return CreateQuery<F197001Data>("GetF197001Data")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("labelCode", labelCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("vnrCode", vnrCode);
		}

		public IQueryable<DistributionData> GetDistributionData(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<DistributionData>("GetDistributionData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public IQueryable<ExecuteResult> CheckItemLocStatus(String dcCode, String tarDcCode, String gupCode, String custCode, String itemCode, String srcLocCode, String tarLocCode, String validDate)
		{
			return CreateQuery<ExecuteResult>("CheckItemLocStatus")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("tarDcCode", tarDcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("srcLocCode", srcLocCode)
						.AddQueryExOption("tarLocCode", tarLocCode)
						.AddQueryExOption("validDate", validDate);
		}

		public IQueryable<F1909EX> GetP1909EXDatas()
		{
			return CreateQuery<F1909EX>("GetP1909EXDatas")
						;
		}

		public String GetFloors(String dcCode, String warehouseId)
		{
			return CreateQuery<String>("GetFloors")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("warehouseId", warehouseId).ToList().FirstOrDefault();
		}

		public String GetChanels(String dcCode, String warehouseId, String floor)
		{
			return CreateQuery<String>("GetChanels")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("floor", floor).ToList().FirstOrDefault();
		}

		public IQueryable<F1912LocData> GetNonAllowedF1912LocDatas(String dcCode, String warehouseId, String floor, String beginLocCode, String endLocCode, String workId)
		{
			return CreateQuery<F1912LocData>("GetNonAllowedF1912LocDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("warehouseId", warehouseId)
						.AddQueryExOption("floor", floor)
						.AddQueryExOption("beginLocCode", beginLocCode)
						.AddQueryExOption("endLocCode", endLocCode)
						.AddQueryExOption("workId", workId);
		}

		public IQueryable<F1912LocData> GetAllowedF1912LocDatas(String workId)
		{
			return CreateQuery<F1912LocData>("GetAllowedF1912LocDatas")
						.AddQueryExOption("workId", workId);
		}

		public IQueryable<F199002Data> GetJobValuation(String dcCode, String accItemKindId, String OrdType, String accKind, String accUnit, String status)
		{
			return CreateQuery<F199002Data>("GetJobValuation")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("accItemKindId", accItemKindId)
						.AddQueryExOption("OrdType", OrdType)
						.AddQueryExOption("accKind", accKind)
						.AddQueryExOption("accUnit", accUnit)
						.AddQueryExOption("status", status);
		}

		public IQueryable<F1934EX> GetF1934EXDatas()
		{
			return CreateQuery<F1934EX>("GetF1934EXDatas")
						;
		}

		public IQueryable<F1947JoinF194701> GetF1947Join194701Datas(String ALL_ID, String DC_CODE)
		{
			return CreateQuery<F1947JoinF194701>("GetF1947Join194701Datas")
						.AddQueryExOption("ALL_ID", ALL_ID)
						.AddQueryExOption("DC_CODE", DC_CODE);
		}

		public IQueryable<F19470101Datas> GetF19470101Datas(String ALL_ID, String DC_CODE)
		{
			return CreateQuery<F19470101Datas>("GetF19470101Datas")
						.AddQueryExOption("ALL_ID", ALL_ID)
						.AddQueryExOption("DC_CODE", DC_CODE);
		}

		public IQueryable<F199005Data> GetF199005(String dcCode, String accItemKindId, String logiType, String taxType, String accKind, String isSpecialCar, String status)
		{
			return CreateQuery<F199005Data>("GetF199005")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("accItemKindId", accItemKindId)
						.AddQueryExOption("logiType", logiType)
						.AddQueryExOption("taxType", taxType)
						.AddQueryExOption("accKind", accKind)
						.AddQueryExOption("isSpecialCar", isSpecialCar)
						.AddQueryExOption("status", status);
		}

		public IQueryable<F91000302Data> GetAccUnitList(String itemTypeId)
		{
			return CreateQuery<F91000302Data>("GetAccUnitList")
						.AddQueryExOption("itemTypeId", itemTypeId);
		}

		public IQueryable<F194702Data> GetF194702()
		{
			return CreateQuery<F194702Data>("GetF194702")
						;
		}

		public IQueryable<F1948Data> GetF1948(String dcCode)
		{
			return CreateQuery<F1948Data>("GetF1948")
						.AddQueryExOption("dcCode", dcCode);
		}

		public IQueryable<F190102JoinF000904> GetF190102JoinF000904Datas(String DC_CODE, String TOPIC, String Subtopic)
		{
			return CreateQuery<F190102JoinF000904>("GetF190102JoinF000904Datas")
						.AddQueryExOption("DC_CODE", DC_CODE)
						.AddQueryExOption("TOPIC", TOPIC)
						.AddQueryExOption("Subtopic", Subtopic);
		}

		public ExecuteResult DeleteP190109(String gupCode, String vnrCode)
		{
			return CreateQuery<ExecuteResult>("DeleteP190109")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("vnrCode", vnrCode).ToList().FirstOrDefault();
		}

		public IQueryable<F0003Ex> GetF0003(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<F0003Ex>("GetF0003")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public IQueryable<F91000302SearchData> GetF91000302Data(String itemTypeId, String accUnit, String accUnitName)
		{
			return CreateQuery<F91000302SearchData>("GetF91000302Data")
						.AddQueryExOption("itemTypeId", itemTypeId)
						.AddQueryExOption("accUnit", accUnit)
						.AddQueryExOption("accUnitName", accUnitName);
		}

		public IQueryable<P192019Item> GetP192019SearchData(String gupCode,String custCode, String clsCode, String clsName, String clsType)
		{
			return CreateQuery<P192019Item>("GetP192019SearchData")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode",custCode)
						.AddQueryExOption("clsCode", clsCode)
						.AddQueryExOption("clsName", clsName)
						.AddQueryExOption("clsType", clsType);
		}

		public F1952Ex ActivityGetF1952Ex(String empId)
		{
			return CreateQuery<F1952Ex>("ActivityGetF1952Ex")
						.AddQueryExOption("empId", empId).ToList().FirstOrDefault();
		}

		public ExecuteResult InsertF0050(String message, String funId, String functionName)
		{
			return CreateQuery<ExecuteResult>("InsertF0050")
						.AddQueryExOption("message", message)
						.AddQueryExOption("funId", funId)
						.AddQueryExOption("functionName", functionName).ToList().FirstOrDefault();
		}

		public IQueryable<F1912WareHouseData> GetCustWarehouseDatas(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<F1912WareHouseData>("GetCustWarehouseDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public IQueryable<F1924Data> GetF1924DataByAccount(String account, String password)
		{
			return CreateQuery<F1924Data>("GetF1924DataByAccount")
						.AddQueryExOption("account", account)
						.AddQueryExOption("password", password);
		}
	}
}

