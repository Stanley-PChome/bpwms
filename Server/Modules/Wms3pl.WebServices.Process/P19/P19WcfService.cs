
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	/// <summary>
	/// 系統功能
	/// </summary>
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P19WcfService
	{


		#region 範例用，以後移除

		[OperationContract]
		public IQueryable<F1929WithF1909Test> GetF1929WithF1909Tests(string gupCode)
		{
			var repository = new F1929Repository(Schemas.CoreSchema);
			return repository.GetF1929WithF1909Tests(gupCode);
		}
		#endregion 範例用，以後移除
		/// <summary>
		/// 更新權限設定
		/// </summary>
		[OperationContract]
		public ExecuteResult UpdateP190504(string groupId, string groupName, string groupDesc
				, string showInfo, string funCodeList, string scheduleList, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190504Service(wmsTransaction);
			ExecuteResult result;
			result = srv.UpdateP190504(groupId, groupName, groupDesc, showInfo,
					funCodeList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
					scheduleList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
					userId);

			if (result.IsSuccessed) wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 新增主檔F1953，然後新增明細資料F195301、F195302
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="groupDesc"></param>
		/// <param name="showInfo"></param>
		/// <param name="funCodeList">選擇的F195301的ID資料</param>
		/// <param name="scheduleList">選擇的F195302的ID資料</param>
		/// <param name="userId"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult AddP190504(string groupName, string groupDesc, string showInfo
				, string funCodeList, string scheduleList, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190504Service(wmsTransaction);
			var saveTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
			var result = srv.AddP190504(groupName, groupDesc, showInfo, userId, saveTime);

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
				// 於成功新增後, 再取得所新增的ID, 然後寫入明細資料
				wmsTransaction = new WmsTransaction();
				srv = new P190504Service(wmsTransaction);

				var grpId = srv.GetF1953_GRP_ID(userId, saveTime);
				if (grpId == null) return new ExecuteResult() { IsSuccessed = false };

				result = srv.AddP190504Detail(funCodeList.Split(',').ToList(), grpId.Value);
				if (!result.IsSuccessed) return new ExecuteResult() { IsSuccessed = false };

				result = srv.AddP195302Detail(scheduleList.Split(',').ToList(), grpId.Value);
				if (!result.IsSuccessed) return new ExecuteResult() { IsSuccessed = false };

				wmsTransaction.Complete();
			}
			return result;
		}

		/// <summary>
		/// 檢查F1963有無重複資料, KEY值為NAME, 若存在WORK ID不同的項目, 則回傳true代表有重複
		/// 不在ViewModel寫是考量未來效能問題
		/// </summary>
		/// <param name="workgroupId"></param>
		/// <param name="workgroupName"></param>
		/// <returns></returns>
		[OperationContract]
		public bool F1963CheckDuplicateByIdName(decimal workgroupId, string groupName)
		{
			var srv = new P190507Service();
			var result = srv.F1963CheckDuplicateByIdName(workgroupId, groupName);
			return result;
		}

		[OperationContract]
		public ExecuteResult InsertP190507(F1963 f1963, List<F196301> f196301s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190507Service(wmsTransaction);
			ExecuteResult result;
			result = srv.InsertP190507(f1963, f196301s);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateP190507(F1963 f1963, List<F196301> f196301s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190507Service(wmsTransaction);
			ExecuteResult result;
			result = srv.UpdateP190507(f1963, f196301s);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateP190508(decimal workgroupId, List<string> empList, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190508Service(wmsTransaction);
			ExecuteResult result;
			result = srv.UpdateP190508(workgroupId, empList, userId);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateP190501(F1924 emp, List<string> groups, List<F192402> custs, string gupCode, string userId, string newMenuName, string dcCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190501Service(wmsTransaction);
			ExecuteResult result;
			result = srv.UpdateP190501(emp, groups, custs, gupCode, userId, newMenuName, dcCode);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertP190501(string gupCode, string custCode, F1924 emp, List<string> groups, List<F192402> custs, string userId, string newMenuName, string dcCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190501Service(wmsTransaction);
			ExecuteResult result;
			result = srv.InsertP190501(gupCode, custCode, emp, groups, custs, userId, newMenuName, dcCode);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult DeleteP190501(string empId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190501Service(wmsTransaction);
			ExecuteResult result;
			result = srv.DeleteP190501(empId);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}


		#region P190506
		[OperationContract]
		public ExecuteResult UpdateP190506(string empId, string password, string confirmPassword, List<decimal> addgroups, List<decimal> removegroups, List<decimal> addworkgroups, List<decimal> removeworkgroups, List<string> scheduleIdList, string checkpackage)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190506Service(wmsTransaction);

			var result = srv.UpdateP190506(empId, password, confirmPassword, addgroups, removegroups, addworkgroups, removeworkgroups, scheduleIdList, checkpackage);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion


		#region P190101 DC主檔維護
		[OperationContract]
		public ExecuteResult UpdateP190101(F1901 dc, List<F190101> dcService, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190101Service(wmsTransaction);
			ExecuteResult result;
			result = srv.UpdateP190101(dc, dcService, userId);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateP190101New1(F1901 dc, List<F190101> dcService, List<F190904> F190904List, List<F050006> F050006List, List<F190102> F190102List, string userId, string gupCode, string custCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190101Service(wmsTransaction);
			ExecuteResult result;
			result = srv.UpdateP190101(dc, dcService, F190904List, F050006List, F190102List, userId, gupCode, custCode);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertP190101(F1901 dc, List<F190101> dcService, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190101Service(wmsTransaction);
			ExecuteResult result;
			result = srv.InsertP190101(dc, dcService, userId);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertP190101New1(F1901 dc, List<F190101> dcService, List<F190904> F190904List, List<F050006> F050006List, List<F190102> F190102List, string userId, string gupCode, string custCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190101Service(wmsTransaction);
			ExecuteResult result;
			result = srv.InsertP190101(dc, dcService, F190904List, F050006List, F190102List, userId, gupCode, custCode);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		//[OperationContract]
		//public ExecuteResult InsertP190101New1(F1901 dc, List<F190101> dcService, List<F190904> F190904List, List<F050006> F050006List, List<F190102> F190102List, string userId)
		//{
		//    var wmsTransaction = new WmsTransaction();
		//    var srv = new P190101Service(wmsTransaction);
		//    ExecuteResult result;
		//    result = srv.InsertP190101(dc, dcService, F190904List, F050006List, F190102List, userId);

		//    if (result.IsSuccessed == true) wmsTransaction.Complete();

		//    return result;
		//}
		#endregion

		#region P190102 商品主檔維護
		// 使用F1903為產品主檔
		[OperationContract]
    public ExecuteResult UpdateP190102(F1903 SubItem, F1905 VolumeItem, F190305 palletLevel)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P190102Service(wmsTransaction);
      ExecuteResult result;
      result = srv.UpdateP190102(SubItem, VolumeItem, palletLevel);

      if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertP190102(F1903 SubItem, F1905 VolumeItem, F190305 palletLevel)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190102Service(wmsTransaction);
			ExecuteResult result;
      SubItem.ITEM_CODE = SubItem.ITEM_CODE?.ToUpper();
      SubItem.EAN_CODE1 = SubItem.EAN_CODE1?.ToUpper();
      SubItem.EAN_CODE2 = SubItem.EAN_CODE2?.ToUpper();
      SubItem.EAN_CODE3 = SubItem.EAN_CODE3?.ToUpper();
      SubItem.EAN_CODE4 = SubItem.EAN_CODE4?.ToUpper();

      result = srv.InsertP190102(SubItem, VolumeItem, palletLevel);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P190103 商品包裝維護

		[OperationContract]
		public ExecuteResult SaveItemPack(List<F190301> delItems, List<F190301> addItems, List<F190301> editItems)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190103Service(wmsTransaction);
			ExecuteResult result;
			result = srv.SaveItemPack(delItems, addItems, editItems);

			if (result.IsSuccessed == true) wmsTransaction.Complete();

			return result;
		}
		#endregion



		#region 新增 F197001 標籤資料
		[OperationContract]
		public ExecuteResult InsertF197001(List<F197001> f197001Data)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P197001Service(wmsTransaction);
			ExecuteResult result;
			result = srv.InsertF197001(f197001Data);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region 修改 F197001 標籤資料
		[OperationContract]
		public ExecuteResult UpdateF197001(F197001 f197001Data)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P197001Service(wmsTransaction);
			ExecuteResult result;
			result = srv.UpdateF197001(f197001Data);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region 刪除 F197001 標籤資料
		[OperationContract]
		public ExecuteResult DelF197001s(F197001Data[] f197001Datas)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P197001Service(wmsTransaction);
			ExecuteResult result;
			result = srv.DelF197001s(f197001Datas);
			if (result.IsSuccessed == true)
				wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region 取得報廢量
		[OperationContract]
		public int GetScrapItemStock(string dcCode, string gupCode, string custCode, string itemCode)
		{
			var srv = new P1913Service();
			var result = srv.GetScrapItemStock(dcCode, gupCode, custCode, itemCode);
			return result;
		}
		#endregion


		#region 作業群組設定

		/// <summary>
		/// 取得未設定儲位
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="warehouseId"></param>
		/// <param name="floor"></param>
		/// <param name="beginLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="excludeLocCodes">排除的儲位編號</param>
		/// <returns></returns>
		[OperationContract]
		public IQueryable<F1912LocData> GetNonAllowedF1912LocDatas(string dcCode, string warehouseId, string floor, string beginLocCode, string endLocCode, string[] excludeLocCodes)
		{
			var f196301Repo = new F196301Repository(Schemas.CoreSchema);
			return f196301Repo.GetNonAllowedF1912LocDatas(dcCode, warehouseId, floor, beginLocCode, endLocCode, excludeLocCodes);
		}

		#endregion

		[OperationContract]
		public ExecuteResult UpdateF0003(F0003 f0003Data)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P192018Service(wmsTransaction);
			ExecuteResult result = srv.UpdateF0003(f0003Data);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult SaveData(string gid, string staff, string name, List<string> listQid)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190701Service(wmsTransaction);
			var result = srv.SaveData(gid, staff, name, listQid);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public Tuple<DataSet, string> GetQueryData(decimal qid, List<object> listParameters)
		{
			var srv = new P190702Service();
			return srv.GetQueryData(qid, listParameters);
		}

		[OperationContract]
		public Dictionary<string, List<KeyValuePair<string, string>>> GetQueryDataByCombo()
		{
			var srv = new P190702Service();
			return srv.GetComboQueryData();
		}

		/// <summary>
		/// 解析匯入的標籤內容
		/// </summary>
		/// <param name="f1970"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		[OperationContract]
		public IQueryable<F197001Data> ParseImportF197001Data(F1970 f1970, List<F197001Data> list)
		{
			var srv = new P197001Service();
			return srv.ParseImportF197001Data(f1970, list);
		}

		/// <summary>
		/// 驗證匯入的標籤設定內容
		/// </summary>
		/// <param name="f1970"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult ValidateImportF197001Data(F1970 f1970, List<F197001Data> list)
		{
			var srv = new P197001Service();
			return srv.ValidateImportF197001Data(f1970, list);
		}

		/// <summary>
		/// 列印標籤時，自動更新或新增標籤
		/// </summary>
		/// <param name="f1970"></param>
		/// <param name="list"></param>
		[OperationContract]
		public ExecuteResult InsertOrUpdateF197001s(F1970 f1970, List<F197001> list)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P197001Service(wmsTransaction);
			var result = srv.InsertOrUpdateF197001s(f1970, list);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult ImportData(string gupCode, string custCode, List<string[]> items, string fileName)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190102Service(wmsTransaction);
			var result = srv.ImportData(gupCode, custCode, items, fileName);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		#region 商品大中小分類
		[OperationContract]
		public ExecuteResult DeleteCls(P192019Data data)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P192019Service(wmsTransation);
			var result = service.DeleteCls(data);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult InsertOrUpdateCls(P192019Data data, bool isAdd)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P192019Service(wmsTransation);
			var result = service.InsertOrUpdateCls(data, isAdd);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}
		#endregion

		[OperationContract]
		public ExecuteResult InsertF191201Datas(List<F191201> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P192021Service(wmsTransaction);
			var result = srv.InsertF191201Datas(datas);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult DeletedF191201Datas(List<F191201> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P192021Service(wmsTransaction);
			var result = srv.DeletedF191201Datas(datas);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult InsertOrUpdateF1910(F1910 data, bool isAdd)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P190110Service(wmsTransaction);
			var result = service.InsertOrUpdateF1910(data, isAdd);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult InsertOrUpdateF194716(F194716 data, List<F19471601Data> dtlItems, bool isAdd)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P190116Service(wmsTransaction);
			var result = service.InsertOrUpdateF194716(data, dtlItems, isAdd);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		#region P1905130000 平台系統帳號管理

		[OperationContract]
		public ExecuteResult F0070LoginDatasDelete(List<F0070LoginData> f0070LD)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190513Service(wmsTransaction);
			var result = srv.F0070LoginDatasDelete(f0070LD.ToList());
			if (result.IsSuccessed) wmsTransaction.Complete();
			return result;
		}

		#endregion

		[OperationContract]
		public ExecuteResult UpdateF190105AndF190106(F190105 data, List<F190106Data> addF190106, List<F190106Data> delF190106)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190105Service(wmsTransaction);
			ExecuteResult result = srv.UpdateF190105AndF190106(data, addF190106.ToList(), delF190106.ToList());
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}



		#region P1901810000 PK區設定維護
		[OperationContract]
		public IQueryable<String> GetF191201Floors(string dcCode)
		{
			var repo = new F191201Repository(Schemas.CoreSchema);
			return repo.GetF191201Floors(dcCode);

		}

		[OperationContract]
		public IQueryable<String> GetF1912LocCodeList(string dcCode, string BeginLocCode, string EndLocCode)
		{
			var repo = new F1912Repository(Schemas.CoreSchema);
			return repo.GEtF1912LocCodeList(dcCode, BeginLocCode, EndLocCode);
		}

		[OperationContract]
		public ExecuteResult InsertOrUpdateF191206(F191206 f191206, F19120601[] f19120601s, F19120602[] f19120602s, bool isAdd)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P191206Service(wmsTransaction);
			var result = service.InsertOrUpdateF191206(f191206, f19120601s, f19120602s, isAdd);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion P1901810000 PK區設定維護


		#region 工作站設定
		/// <summary>
		/// 新增工作站
		/// </summary>
		/// <param name="f1946"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult InsertF1946(F1946 f1946)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190191Service(wmsTransaction);
			ExecuteResult result = srv.InsertF1946(f1946);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 刪除工作站
		/// </summary>
		/// <param name="f1946"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult DeleteF1946(F1946 f1946)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190191Service(wmsTransaction);
			ExecuteResult result = srv.DeleteF1946(f1946);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion


		#region 集貨格類型維護
		// 新增/編輯集貨格類型
		[OperationContract]
		public ExecuteResult InsertOrUpdateF194501(string dcCode, string cellType, string cellName, int length, int depth, int heigth, string volumeRate, string typeMode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190193Service(wmsTransaction);
			ExecuteResult result = srv.InsertOrUpdateF194501(dcCode, cellType, cellName, length, depth, heigth, volumeRate, typeMode);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		// 刪除集貨格類型
		[OperationContract]
		public ExecuteResult DeleteF194501(string dcCode, string cellType)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190193Service(wmsTransaction);
			ExecuteResult result = srv.DeleteF194501(dcCode, cellType);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region 集貨場維護
		/// <summary>
		/// 新增集貨場及集貨場集貨格
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="CollectionType"></param>
		/// <param name="CollectionCode"></param>
		/// <param name="CollectionName"></param>
		/// <param name="Addf1945CellLists"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult InsertF1945(string dcCode, string gupCode, string custCode, string CollectionType, string CollectionCode, string CollectionName, List<F1945CellList> Addf1945CellLists)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190192Service(wmsTransaction);
			var result = srv.InsertF1945(dcCode, gupCode, custCode, CollectionType, CollectionCode, CollectionName, Addf1945CellLists);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public Boolean IsCellNumCanReduce(string dcCode, string CollectionCode, String CellType, int CellNum)
		{
			var srv = new P190192Service();
			return srv.IsCellNumCanReduce(dcCode, CollectionCode, CellType, CellNum);
		}

		[OperationContract]
		public ExecuteResult UpdateF051401(string dcCode, string gupCode, string custCode, string CollectionType, string CollectionCode, string CollectionName, List<F1945CellList> F1945CellLists)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P190192Service(wmsTransaction);
			var result = srv.UpdateF051401(dcCode, gupCode, custCode, CollectionType, CollectionCode, CollectionName, F1945CellLists);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;

		}
        #endregion 集貨場維護

        #region 物流商維護
        // 新增/編輯物流商主檔
        [OperationContract]
        public ExecuteResult InsertOrUpdateF0002(string dcCode, string logisticCode, string logistcName, string isPierRecvPoint, string isVendorReturn, string typeMode)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190104Service(wmsTransaction);
            ExecuteResult result = srv.InsertOrUpdateF0002(dcCode, logisticCode, logistcName, isPierRecvPoint, isVendorReturn, typeMode);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return result;
        }

        // 刪除物流商主檔
        [OperationContract]
        public ExecuteResult DeleteF0002(string dcCode, string logisticCode)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190104Service(wmsTransaction);
            ExecuteResult result = srv.DeleteF0002(dcCode, logisticCode);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return result;
        }
        #endregion
    }
}

