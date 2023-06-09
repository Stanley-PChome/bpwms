
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	/// <summary>
	/// 系統功能
	/// </summary>
	public partial class P190507Service
	{
		private WmsTransaction _wmsTransaction;
		public P190507Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F1912> GetAssignedLoc(string workgroupId, string dcCode)
		{
			var repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);

			var result = repo.GetAssignedLoc(workgroupId, dcCode);
			
			return result;
		}
		public IQueryable<F1912> GetUnAssignedLoc(string workgroupId, string dcCode, string warehouseId
			, string floor, string startLocCode, string endLocCode)
		{
			var repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);

			var result = repo.GetUnAssignedLoc(workgroupId, dcCode, warehouseId, floor, startLocCode, endLocCode);

			return result;
		}

		public ExecuteResult InsertP190507(F1963 f1963, List<F196301> f196301s)
		{
			var repo = new F196301Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1963 = new F1963Repository(Schemas.CoreSchema, _wmsTransaction);

			if (f1963 == null || f196301s == null || !f196301s.Any())
			{
				return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P190507Service_WorkGroupParam_Required};
			}

			// pre. 檢查名稱
			if (repoF1963.CheckDuplicateByIdName(0, f1963.WORK_NAME))
			{
				return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P190507Service_WorkGroupName_Duplicate};
			}

			// 0. 取得新ID
			decimal workgroupId = repoF1963.GetNewId();

			// 1. 寫入F1963
			repoF1963.Add(new F1963()
			{
				WORK_ID = workgroupId,
				WORK_DESC = f1963.WORK_DESC,
				WORK_NAME = f1963.WORK_NAME
			});

			// 2. 更新F196301
			foreach (var dcGroup in f196301s.GroupBy(item => item.DC_CODE))
			{
				string dcCode = dcGroup.Key;
				List<string> locList = dcGroup.Select(item => item.LOC_CODE).ToList();

				// 2.1寫入新資料
				foreach (var p in locList)
				{
					repo.Add(new F196301()
					{
						WORK_ID = workgroupId,
						LOC_CODE = p,
						DC_CODE = dcCode
					});
				}
			}
			return new ExecuteResult() { IsSuccessed = true, Message = workgroupId.ToString() };
		}

		public bool F1963CheckDuplicateByIdName(decimal workgroupId, string groupName)
		{
			var repo = new F1963Repository(Schemas.CoreSchema);
			return repo.CheckDuplicateByIdName(workgroupId, groupName);
		}

		public ExecuteResult UpdateP190507(F1963 f1963, List<F196301> f196301s)
		{
			var repo = new F196301Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1963 = new F1963Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF196301 = new F196301Repository(Schemas.CoreSchema, _wmsTransaction);
			
			if (f1963 == null || f196301s == null || !f196301s.Any())
			{
				return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P190507Service_WorkGroupParam_Required};
			}

			// pre. 檢查名稱
			if (repoF1963.CheckDuplicateByIdName(f1963.WORK_ID, f1963.WORK_NAME))
			{
				return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P190507Service_WorkGroupName_Duplicate};
			}

			// 0. 先檢查該群組是否仍存在
			var f1963Data = repoF1963.Find(x => x.WORK_ID == f1963.WORK_ID);
			if (f1963Data == null)
			{
				return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P190505Service_WorkGroupDeleted};
			}

			// 1. 更新F1963
			f1963Data.WORK_NAME = f1963.WORK_NAME;
			f1963Data.WORK_DESC = f1963.WORK_DESC;
			repoF1963.Update(f1963Data);

			// 2. 更新F196301
			foreach (var dcGroup in f196301s.GroupBy(item => item.DC_CODE))
			{
				string dcCode = dcGroup.Key;

        #region 20230310 kk mark
    //    List<string> locList = dcGroup.Select(item => item.LOC_CODE).ToList();   

    //    //取得已經設定的項目
    //    var exisLocsData = repo.GetAllowedF1912LocDatas(f1963.WORK_ID.ToString()).Select(o => o.LOC_CODE);
    
    //    // 2.1取得使用者要移除的項目
    //    //var dataNotIn = repo.DataNotIn(f1963.WORK_ID, dcCode, locList);
    //    var dataNotIn = exisLocsData.Except(locList).ToList();    

    //    // 2.2進行批次刪除									
    //    repo.DeleteLocIn(f1963.WORK_ID, dcCode, dataNotIn);       
    //    //取得已存在的儲位
    //    //var existLocs = repo.GetDatas(f1963.WORK_ID, dcCode,locList).Select(x=> x.LOC_CODE).ToList();
    //    var existLocs = repo.GetAllowedF1912LocDatas(f1963.WORK_ID.ToString()).Select(o => o.LOC_CODE);      
    //    //排除已存在儲位 剩下為新增儲位
    //    locList = locList.Except(existLocs).ToList();
 
    //    var addf196301List = new List<F196301>();
				//// 2.3寫入新資料
				//foreach (var p in locList)
				//{
				//	addf196301List.Add(new F196301()
				//	{
				//		WORK_ID = f1963.WORK_ID,
				//		DC_CODE = dcCode,
				//		LOC_CODE = p
				//	});
				//}
				//repo.BulkInsert(addf196301List);
        #endregion 20230310 kk mark

        List<F1912LocData> locList_2 = dcGroup.Select(r => new F1912LocData
        {
          AREA_CODE = "",
          DC_CODE = r.DC_CODE,
          LOC_CODE = r.LOC_CODE
        }).ToList();


        //取得已經設定的項目

        List<F1912LocData> exisLocsData_2 = repo.GetAllowedF1912LocDatas(f1963.WORK_ID.ToString()).Where(o => o.DC_CODE.Equals(dcCode)).ToList();

        // 2.1取得使用者要移除的項目
        List<F1912LocData> dataNotIn_2 = F1912_Except(exisLocsData_2, locList_2);
        // 2.2進行批次刪除									
        repo.DeleteLocIn_2(f1963.WORK_ID, dcCode, dataNotIn_2);
        //取得已存在的儲位
        List<F1912LocData> existLocs_2 = repo.GetAllowedF1912LocDatas(f1963.WORK_ID.ToString()).Where(o => o.DC_CODE.Equals(dcCode)).ToList();
        //排除已存在儲位 剩下為新增儲位
        locList_2 = F1912_Except(locList_2, existLocs_2);
        var addf196301List_2 = new List<F196301>();
        // 2.3寫入新資料
        foreach (F1912LocData p in locList_2)
        {
          addf196301List_2.Add(new F196301()
          {
            WORK_ID = f1963.WORK_ID,
            DC_CODE = p.DC_CODE ,
            LOC_CODE = p.LOC_CODE
          });
        }
        repo.BulkInsert(addf196301List_2);
      }

			
			return new ExecuteResult() { IsSuccessed = true };
		}

    public List<F1912LocData> F1912_Except(List<F1912LocData> All_Data, List<F1912LocData> dataNotIn)
    {
      List<F1912LocData> result=new List<F1912LocData>();
      foreach ( F1912LocData item in  All_Data)
      {
        bool flg = false;
        foreach (F1912LocData item_NotIn in dataNotIn)
        {         
          if(item.DC_CODE.Equals(item_NotIn.DC_CODE) && (item.LOC_CODE.Equals(item_NotIn.LOC_CODE)))
          { flg = true; }         
        }
        if(flg==false)
        {
          result.Add(new F1912LocData()
          {
            AREA_CODE = "",
            DC_CODE = item.DC_CODE,
            LOC_CODE = item.LOC_CODE
          });
          
        }
      
      }

      return result;
    }

  }
}

