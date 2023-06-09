using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P71.Services
{
  class P710103Service
  {
    private WmsTransaction _wmsTransaction;

    public P710103Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    /// <summary>
    /// 傳回儲位屬性維護清單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="warehourseId"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="warehouseType"></param>
    /// <param name="areaCode"></param>
    /// <param name="locCodeS"></param>
    /// <param name="locCodeE"></param>
    /// <returns></returns>
    public IQueryable<F1912WithF1980> GetF1912WithF1980(string dcCode, string warehouseId, string gupCode, string custCode, string warehouseType, string areaCode, string channel, string locCodeS, string locCodeE,string account)
    {
			var gupCodeList = new List<string>();
			var custCodeList = new List<string>();
			if (string.IsNullOrEmpty(gupCode) || string.IsNullOrEmpty(custCode))
			{
				var f192402Repo = new F192402Repository(Schemas.CoreSchema);
				var f192402s = f192402Repo.GetDatasByEmpId(account).ToList();
				if (string.IsNullOrEmpty(gupCode))
				{
					gupCodeList.Add("0");
					gupCodeList.AddRange(f192402s.Select(x => x.GUP_CODE).Distinct().ToList());
				}
				else
					gupCodeList.Add(gupCode);
				if (string.IsNullOrEmpty(custCode))
				{
					custCodeList.Add("0");
					custCodeList.AddRange(f192402s.Select(x => x.CUST_CODE).Distinct().ToList());
				}
				else
					custCodeList.Add(custCode);
			}
			else
			{
				gupCodeList.Add(gupCode);
				custCodeList.Add(custCode);
			}

			var f1912repo = new F1912Repository(Schemas.CoreSchema);
			var locListForWarehouse = f1912repo.GetLocListForWarehouse(dcCode, warehouseId, gupCodeList, custCodeList, warehouseType, areaCode, channel, locCodeS, locCodeE, account);
			return locListForWarehouse;

    }

      public ExecuteResult UpdateF1912WithF1980Data(F1912WithF1980 f1912withf1980, string userId)
      {
        var result = new ExecuteResult { IsSuccessed = true };
        var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
        var f191202Repo = new F191202Repository(Schemas.CoreSchema, _wmsTransaction);
        decimal usefulVolumn = 0;
        var f1942Repo = new F1942Repository(Schemas.CoreSchema, _wmsTransaction);
        //計算可用容積量
        var f1942Data = f1942Repo.Find(x => x.LOC_TYPE_ID.Equals(f1912withf1980.LOC_TYPE_ID));
        if (f1942Data != null)
        {
          usefulVolumn = SharedService.GetUsefulColumn(f1942Data.LENGTH, f1942Data.DEPTH, f1942Data.HEIGHT, f1942Data.VOLUME_RATE); // VOLUME_RATE 是0~100，所以要除100
        }

        var f1912Item = f1912Repo.Find(o => o.DC_CODE == f1912withf1980.DC_CODE && o.LOC_CODE == f1912withf1980.LOC_CODE);
        f1912Item.LOC_TYPE_ID = f1912withf1980.LOC_TYPE_ID;
        f1912Item.HOR_DISTANCE = f1912withf1980.HOR_DISTANCE;
        f1912Item.RENT_BEGIN_DATE = f1912withf1980.RENT_BEGIN_DATE;
        f1912Item.RENT_END_DATE = f1912withf1980.RENT_END_DATE;
        f1912Item.USEFUL_VOLUMN = usefulVolumn;
        f1912Item.UPD_STAFF = userId;
        f1912Item.UPD_DATE = DateTime.Now;
        f1912Repo.Update(f1912Item);
        f191202Repo.Log(f1912Item, userId, "2", "1");

        return result;
      }
    }
  }
