using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Schedule.S00.Services
{
	/// <summary>
	/// 運費結算
	/// </summary>
	public partial class S000401Service
	{
		private WmsTransaction _wmsTransaction;
		//暫存級距表避免重複撈取
		private Dictionary<string[], List<F5111>> calRangeList;
		public S000401Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 計算運費
		/// </summary>
		/// <param name="calDate">計算日期</param>
		/// <returns></returns>
		public void CalculateShipFee(DateTime calDate)
		{
			var addF5112List = new List<F5112>();
			var f5112Repo = new F5112Repository(Schemas.CoreSchema, _wmsTransaction);
			f5112Repo.Delete(x => x.CAL_DATE == calDate.Date);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var delvNoItems = f050801Repo.GetDelvNoItems(calDate);
			//By 物流中心、業主、貨主、車次、加價費群組計算
			var groupDelvNoItems = delvNoItems.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.DELV_NO, x.EXTRA_FEE ,x.REGION_FEE,x.OIL_FEE,x.OVERTIME_FEE});
			foreach (var groupDelvNoItem in groupDelvNoItems)
			{
				var shipFeeList = new List<decimal>();
				var volumeShipFee = CalculateShipFeeByVolume(groupDelvNoItem.Key.DC_CODE, groupDelvNoItem.Key.GUP_CODE, groupDelvNoItem.Key.CUST_CODE, groupDelvNoItem.ToList());
				shipFeeList.Add(volumeShipFee);
				var weightShipFee = CalculateShipFeeByWeight(groupDelvNoItem.Key.DC_CODE, groupDelvNoItem.Key.GUP_CODE, groupDelvNoItem.Key.CUST_CODE, groupDelvNoItem.ToList());
				shipFeeList.Add(weightShipFee);
				var shipFee = shipFeeList.Max();
        //額外費用另外包含 REGION_FEE 區域加價、 OIL_FEE 油資補貼、 OVERTIME_FEE 超點加價
        var total_extra = groupDelvNoItem.Key.EXTRA_FEE + groupDelvNoItem.Key.REGION_FEE + groupDelvNoItem.Key.OIL_FEE + groupDelvNoItem.Key.OVERTIME_FEE; 
        addF5112List.Add(CreateF5112(calDate, groupDelvNoItem.Key.DC_CODE, groupDelvNoItem.Key.GUP_CODE, groupDelvNoItem.Key.CUST_CODE, groupDelvNoItem.Key.DELV_NO, shipFee, total_extra));
			}
			if (addF5112List.Any())
				f5112Repo.BulkInsert(addF5112List);
		}

		/// <summary>
		/// 建立結算資料表
		/// </summary>
		/// <param name="calDate">計算日期</param>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="delvNo">車次</param>
		/// <param name="shipFee">運費</param>
		/// <param name="extraFee">加價費</param>
		/// <returns></returns>
		private F5112 CreateF5112(DateTime calDate, string dcCode, string gupCode, string custCode, string delvNo, decimal shipFee, decimal extraFee)
		{
			return new F5112
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				DELV_NO = delvNo,
				CAL_COST = shipFee,
				EXTRA_FEE = extraFee,
				CAL_DATE = calDate.Date,
			};
		}

		/// <summary>
		/// 計算運費By商品總材積
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="delvNoItems">計算運費商品資料</param>
		/// <returns></returns>
		private decimal CalculateShipFeeByVolume(string dcCode, string gupCode, string custCode, List<DelvNoItem> delvNoItems)
		{
			//移除商品材積=0不計算
			delvNoItems = delvNoItems.Where(x => (x.PACK_LENGTH * x.PACK_WIDTH * x.PACK_HIGHT > 0)).ToList();
			//總材積(商品為公分要轉換成公尺所以除以100)
			var totalVolume = delvNoItems.Sum(x => x.PACK_LENGTH * x.PACK_WIDTH * x.PACK_HIGHT * x.QTY /100);
			var shipFee = 0;
			if (totalVolume > 0)
			{
				return GetShipFee(dcCode, gupCode, custCode, "01", totalVolume);
			}
			return shipFee;
		}
		/// <summary>
		/// 計算運費By商品總重量
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="delvNoItems">計算運費商品資料</param>
		/// <returns></returns>
		private decimal CalculateShipFeeByWeight(string dcCode, string gupCode, string custCode, List<DelvNoItem> delvNoItems)
		{
			//移除商品重量=0不計算
			delvNoItems = delvNoItems.Where(x => (x.PACK_WEIGHT > 0)).ToList();
			//總重量
			var totalWeight = delvNoItems.Sum(x => x.PACK_WEIGHT * x.QTY);
			var shipFee = 0;
			if (totalWeight > 0)
			{
				return GetShipFee(dcCode, gupCode, custCode, "02", totalWeight);
			}
			return shipFee;
		}

		/// <summary>
		/// 由級距表取得運費
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="calType">計價類型(01:材積 02:重量)</param>
		/// <param name="total"></param>
		/// <returns></returns>
		private decimal GetShipFee(string dcCode, string gupCode, string custCode, string calType, decimal total)
		{
			if (calRangeList == null)
				calRangeList = new Dictionary<string[], List<F5111>>();

			var f5111Repo = new F5111Repository(Schemas.CoreSchema);
			//取得級距表
			var calRange = calRangeList.FirstOrDefault(x => x.Key[0] == dcCode && x.Key[1] == gupCode && x.Key[2] == custCode && x.Key[3] == calType);
			var rangeList = new List<F5111>();
			if (calRange.Equals(default(KeyValuePair<string[], List<F5111>>)))
			{
				rangeList = f5111Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.CAL_TYPE == calType).ToList();
				calRangeList.Add(new string[] { dcCode, gupCode, custCode, calType }, rangeList);
			}
			else
				rangeList = calRange.Value;

			//篩選級距表最大值>=計算總數 排序階層(小=>大)
			var ranges = rangeList.Where(x => x.RANGE_MAX >= total).OrderBy(x => x.RANGE_LEVEL).ToList();
			if (ranges.Any())
			{
				//取第一筆(最接近計算總數級距)
				var range = ranges.First();
				//計算總數小於級距最大值 回傳運費
				if (range.RANGE_MAX > total  )
					return range.COST;
				else
				{
					//計算總數=級距最大值
					//如果包含級距最大值 則回傳運費
					if (range.INCLOUDE == "1")
						return range.COST;
					else
					{
						//如果不包含級距最大值 取下一階層的運費
						var nextRange = ranges.Where(x => x.RANGE_LEVEL > range.RANGE_LEVEL).OrderBy(x => x.RANGE_LEVEL).FirstOrDefault();
						if (nextRange != null)
							return nextRange.COST;
						else
							return 0;
					}
				}
			}
			else
			{
				//計算總數超過級距表最大值 則運費為最大階層運費
				var range = rangeList.OrderByDescending(x => x.RANGE_LEVEL).FirstOrDefault();
				if (range != null)
					return range.COST;
				else
					return 0;
			}
		}


	}
}
