using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050303Service
	{
		private WmsTransaction _wmsTransaction;
    public P050303Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

	  public IQueryable<P050303QueryItem> GetP050303SearchData(string gupCode, string custCode, string dcCode,
		DateTime? delvDateBegin, DateTime? delvDateEnd, string ordNo, string custOrdNo, 
		string wmsOrdNo, string status, string consignNo,string itemCode)
	  {
	    var repF050801 = new F050801Repository(Schemas.CoreSchema);
	    return repF050801.GetP050303SearchData(gupCode, custCode, dcCode, delvDateBegin, delvDateEnd, ordNo, custOrdNo,
		  wmsOrdNo, status, consignNo, itemCode);
	  }

	  public IQueryable<F055002WithGridLog> GetF055002WithGridLog(string dcCode, string gupCode, string custCode, string wmsOrdNo)
	  {
		  var repF055002 = new F055002Repository(Schemas.CoreSchema);
		  return repF055002.GetF055002WithGridLog(dcCode,gupCode, custCode , wmsOrdNo);
	  }

    #region 揀貨完成容器資料
    public IQueryable<PickContainer> GetPickContainerData(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var repo = new F070101Repository(Schemas.CoreSchema);
      return repo.GetPickContainerData(dcCode, gupCode, custCode, wmsNo);
    }
    #endregion 揀貨完成容器資料

    #region 訂單取消資訊
    public IQueryable<OrderCancelInfo> GetOrderCancelInfoDataType1(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var repo = new F1511Repository(Schemas.CoreSchema);
      List<string> pickOrdNos = pickOrdNo.Split(',').ToList();
      return repo.GetOrderCancelInfoData(dcCode, gupCode, custCode, pickOrdNos);
    }

    public IQueryable<OrderCancelInfo> GetOrderCancelInfoDataType2(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var repo = new F151002Repository(Schemas.CoreSchema);
      List<string> pickOrdNos = pickOrdNo.Split(',').ToList();
      return repo.GetOrderCancelInfoData(dcCode, gupCode, custCode, pickOrdNos);
    }
    #endregion 訂單取消資訊

    #region 分貨資訊
    public IQueryable<DivideInfo> GetDivideInfo(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var repo = new F052903Repository(Schemas.CoreSchema);
      return repo.GetDivideInfo(dcCode, gupCode, custCode, wmsNo);
    }

    public IQueryable<DivideDetail> GetDivideDetail(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var repo = new F052903Repository(Schemas.CoreSchema);
      return repo.GetDivideDetail(dcCode, gupCode, custCode, wmsNo);
    }
    #endregion 分貨資訊

    #region 集貨場進出紀錄
    public IQueryable<CollectionRecord> GetCollectionRecord(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var repo = new F051402Repository(Schemas.CoreSchema);
      return repo.GetCollectionRecord(dcCode, gupCode, custCode, wmsNo);
    }
    #endregion 集貨場進出紀錄

    #region 託運單箱內明細資料
    public IQueryable<ConsignmentDetail> GetConsignmentDetail(string dcCode, string gupCode, string custCode, string wmsNo)
    {
      var repo = new F055001Repository(Schemas.CoreSchema);
      return repo.GetConsignmentDetail(dcCode, gupCode, custCode, wmsNo);
    }
    #endregion 託運單箱內明細資料
  }
}