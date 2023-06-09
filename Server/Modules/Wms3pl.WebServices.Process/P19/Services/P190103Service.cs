
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P190103Service
	{
		private WmsTransaction _wmsTransaction;
		public P190103Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult SaveItemPack(List<F190301> delItems, List<F190301> addItems, List<F190301> editItems)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var repoF190301 = new F190301Repository(Schemas.CoreSchema, _wmsTransaction);
            var repoF000904_I18N = new F000904_I18NRepository(Schemas.CoreSchema, _wmsTransaction);

            // 驗證系統單位
            var f190301s = repoF190301.GetDatasByF190301s(addItems.Union(editItems).ToList());
            f190301s = f190301s.Where(x => !editItems.Any(z => x.GUP_CODE == z.GUP_CODE &&
                                                               x.ITEM_CODE == z.ITEM_CODE &&
                                                               x.UNIT_ID == z.UNIT_ID));
            if(addItems.Any())
                f190301s = f190301s.Union(addItems);

            if(editItems.Any())
                f190301s = f190301s.Union(editItems);

            if(delItems.Any())
                f190301s.Where(x => !delItems.Any(z => x.GUP_CODE == z.GUP_CODE &&
                                                   x.ITEM_CODE == z.ITEM_CODE &&
                                                   x.UNIT_ID == z.UNIT_ID));

            var sysUnits = repoF000904_I18N.GetDatasByTrueAndCondition(o => o.TOPIC == "F190301" && o.SUBTOPIC == "SYS_UNIT" && o.LANG == Current.Lang);

            var data = (from A in f190301s.Where(x => !string.IsNullOrWhiteSpace(x.SYS_UNIT))
                        join B in sysUnits
                        on A.SYS_UNIT equals B.VALUE
                        select new { F190301 = A, Name = B.NAME }).GroupBy(x => x.Name)
                                                                  .Select(x => new { Name = x.Key, Count = x.Count() })
                                                                  .Where(x => x.Count > 1)
                                                                  .ToList();

            if (data.Any())
            {
                string sysUnitNames = string.Join("、", data.Select(x => x.Name).ToList());
                result.IsSuccessed = false;
                result.Message = string.Format(Properties.Resources.P190110Service_SysUnitCase_Duplicate, sysUnitNames);
            }
            else
            {
                // 刪除
                // 0. 先檢查是否已存在
                if (delItems != null)
			    {
			    	foreach (var delitem in delItems)
			    	{
			    		var f190301del = repoF190301.Find(x => x.GUP_CODE == delitem.GUP_CODE &&
			    															x.ITEM_CODE == delitem.ITEM_CODE &&
			    															x.UNIT_ID == delitem.UNIT_ID);
			    		if (f190301del == null)
			    		{
			    			// 資料不存在
			    			result.IsSuccessed = false;
			    			result.Message = Properties.Resources.P190103Service_DataNotFound_CannotDelete;
			    			return result;
			    		}
			    		// 1. 刪除
			    		repoF190301.Delete(x => x.GUP_CODE == delitem.GUP_CODE &&
			    															x.ITEM_CODE == delitem.ITEM_CODE &&
			    															x.UNIT_ID == delitem.UNIT_ID);
			    	}
			    }
			    
			    // 新增
			    // 0. 先檢查是否已存在
			    if (addItems != null)
			    {
			    	foreach (var item in addItems)
			    	{
			    		var f190301 = repoF190301.Find(x => x.GUP_CODE == item.GUP_CODE &&
			    															x.ITEM_CODE == item.ITEM_CODE &&
			    															x.UNIT_ID == item.UNIT_ID);
			    		if (f190301 != null)
			    		{
			    			// 資料已存在
			    			result.IsSuccessed = false;
			    			result.Message = Properties.Resources.P190103Service_DataExist_CannotInsert;
			    			return result;
			    		}
			    		// 1. 新增
			    		repoF190301.Add(item);
			    	}
			    }

			    //更新
			    if (editItems != null)
			    {
			    	foreach (var editItem in editItems)
			    	{
			    		var f190301edit = repoF190301.Find(x => x.GUP_CODE == editItem.GUP_CODE &&
			    															x.ITEM_CODE == editItem.ITEM_CODE &&
			    															x.UNIT_ID == editItem.UNIT_ID);
			    		if (f190301edit == null)
			    		{
			    			// 資料不存在
			    			result.IsSuccessed = false;
			    			result.Message = Properties.Resources.P190103Service_DataExist_CannotUpdate;
			    			return result;
			    		}
			    		f190301edit.UNIT_LEVEL = editItem.UNIT_LEVEL;
			    		f190301edit.UNIT_QTY = editItem.UNIT_QTY;
			    		f190301edit.LENGTH = editItem.LENGTH;
			    		f190301edit.WIDTH = editItem.WIDTH;
			    		f190301edit.HIGHT = editItem.HIGHT;
			    		f190301edit.WEIGHT = editItem.WEIGHT;
                        f190301edit.SYS_UNIT = editItem.SYS_UNIT;
                        // 1. 更新
                        repoF190301.Update(f190301edit);
			    	}
			    }
            }

            repoF190301 = null;
			return result;
		}
	}
}

