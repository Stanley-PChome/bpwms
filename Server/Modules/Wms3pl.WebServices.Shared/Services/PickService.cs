using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Helper;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class SharedService
	{
		private F050306Repository _f050306Repo;
		private CreatePick _createPick;
		private Dictionary<PickTypeEnums, List<F050306>> _tempPickTypeList;
		private WmsLogHelper _wmsLogHelper;
		private F050301Repository _f050301Repo;

    private List<F190105> _f190105s;
    private List<F050301> _f050301s;
    private List<F1905> _f1905s;
    private Stack<string> _newWmsOrdNosStack;
    private Stack<string> _newPickOrdNosStack;

    private List<F051201> _f051201List;
		private List<F051202> _f051202List;
		private List<F051203> _f051203List;
		private List<F1511> _f1511List;
		private List<F0011> _f0011List;
		private List<F050302> _pickLackF050302s;
    private List<F050802> _pickLackF050802s;

    private List<F194501> _f194501s;
    private List<F1945> _f1945s;
    private DateTime? _prePickTime;
    private List<F195601> _f195601List;
    private List<F050801> _f050801List;

    /// <summary>
    /// 商品容積最多基礎周轉箱數 ([C])
    /// </summary>
    private const int ItemMaxTurnoverContainerCnt = 3;

    /// <summary>
    /// 單據最多基礎周轉箱數 ([D])
    /// </summary>
    private const int OrderMaxTurnoverContainerCnt = 5;

    #region Property

    private CommonService _commonService;
		public CommonService CommonService
		{
			get
			{
				if (_commonService == null)
					_commonService = new CommonService();
				return _commonService;
			}
			set
			{
				_commonService = value;
			}
		}

		private OrderService _orderService;
		public OrderService OrderService
		{
			get
			{
				if (_orderService == null)
					_orderService = new OrderService(_wmsTransaction);
				return _orderService;
			}
			set
			{
				_orderService = value;
			}
		}

		private StockService _stockService;
		public StockService StockService
		{
			get
			{
				if (_stockService == null)
					_stockService = new StockService(_wmsTransaction);
				return _stockService;
			}
			set
			{
				_stockService = value;
			}
		}

    private F051201Repository _f051201Repo;
    public F051201Repository f051201Repo
    {
      get
      {
        if (_f051201Repo == null)
          _f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f051201Repo;
      }
      set
      {
        _f051201Repo = value;
      }
    }

    private F051202Repository _f051202Repo;
    public F051202Repository f051202Repo
    {
      get
      {
        if (_f051202Repo == null)
          _f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f051202Repo;
      }
      set
      {
        _f051202Repo = value;
      }
    }

    private F051203Repository _f051203Repo;
    public F051203Repository f051203Repo
    {
      get
      {
        if (_f051203Repo == null)
          _f051203Repo = new F051203Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f051203Repo;
      }
      set
      {
        _f051203Repo = value;
      }
    }

    private F1511Repository _f1511Repo;
    public F1511Repository f1511Repo
    {
      get
      {
        if (_f1511Repo == null)
          _f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f1511Repo;
      }
      set
      {
        _f1511Repo = value;
      }
    }

    private F050101Repository _f050101Repo;
    public F050101Repository f050101Repo
    {
      get
      {
        if (_f050101Repo == null)
          _f050101Repo = new F050101Repository(Schemas.CoreSchema);
        return _f050101Repo;
      }
      set
      {
        _f050101Repo = value;
      }
    }

    private F050801Repository _f050801Repo;
    public F050801Repository f050801Repo
    {
      get
      {
        if (_f050801Repo == null)
          _f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f050801Repo;
      }
      set
      {
        _f050801Repo = value;
      }
    }

    private F0534Repository _f0534Repo;
    public F0534Repository f0534Repo
    {
      get
      {
        if (_f0534Repo == null)
          _f0534Repo = new F0534Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f0534Repo;
      }
      set
      {
        _f0534Repo = value;
      }
    }

    private F05120601Repository _f05120601Repo;
    public F05120601Repository f05120601Repo
    {
      get
      {
        if (_f05120601Repo == null)
          _f05120601Repo = new F05120601Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f05120601Repo;
      }
      set
      {
        _f05120601Repo = value;
      }
    }

    private F0513Repository _f0513Repo;
    public F0513Repository f0513Repo
    {
      get
      {
        if (_f0513Repo == null)
          _f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f0513Repo;
      }
      set
      {
        _f0513Repo = value;
      }
    }

    private F051301Repository _f051301Repo;
    public F051301Repository f051301Repo
    {
      get
      {
        if (_f051301Repo == null)
          _f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f051301Repo;
      }
      set
      {
        _f051301Repo = value;
      }
    }

    private F060702Repository _f060702Repo;
    public F060702Repository f060702Repo
    {
      get
      {
        if (_f060702Repo == null)
          _f060702Repo = new F060702Repository(Schemas.CoreSchema, _wmsTransaction);
        return _f060702Repo;
      }
      set
      {
        _f060702Repo = value;
      }
    }

    private F060201Repository _f060201Repo;
    public F060201Repository f060201Repo
    {
      get
      {
        if (_f060201Repo == null)
          _f060201Repo = new F060201Repository(Schemas.CoreSchema);
        return _f060201Repo;
      }
      set
      {
        _f060201Repo = value;
      }
    }

    private F0011Repository _f0011Repo;
    public F0011Repository f0011Repo
    {
      get
      {
        if (_f0011Repo == null)
          _f0011Repo = new F0011Repository(Schemas.CoreSchema);
        return _f0011Repo;
      }
      set
      {
        _f0011Repo = value;
      }
    }

    private F195601Repository _f195601Repo;
    public F195601Repository f195601Repo
    {
      get
      {
        if (_f195601Repo == null)
          _f195601Repo = new F195601Repository(Schemas.CoreSchema);
        return _f195601Repo;
      }
      set
      {
        _f195601Repo = value;
      }
    }

    #endregion Property

    #region 揀貨單快取

    /// <summary>
    /// 取得揀貨單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="pickOrdNo"></param>
    /// <returns></returns>
    public F051201 GetF051201(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			return GetF051201s(dcCode, gupCode, custCode, new List<string> { pickOrdNo }).FirstOrDefault();
		}

		public List<F051201> GetF051201s(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			if (_f051201List == null)
				_f051201List = new List<F051201>();

			var f051201s = _f051201List.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && pickOrdNos.Contains(x.PICK_ORD_NO)).ToList();
			var noExistPickNos = pickOrdNos.Except(f051201s.Select(x => x.PICK_ORD_NO)).ToList();
			if(noExistPickNos.Any())
			{
				int range = 1000;
				int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(noExistPickNos.Count()) / range));
				for (int i = 0; i < index; i++)
				{
					var currPickNos = noExistPickNos.Skip(i * range).Take(range).ToList();
					var datas = f051201Repo.AsForUpdate().GetF051201s(dcCode, gupCode, custCode, currPickNos).ToList();
					_f051201List.AddRange(datas);
					f051201s.AddRange(datas);
				}

			}
			return f051201s;
		}

		public List<F051202> GetF051202s(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			if (_f051202List == null)
				_f051202List = new List<F051202>();

			var f051202s = _f051202List.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PICK_ORD_NO == pickOrdNo).ToList();
			if (!f051202s.Any())
			{
				var datas = f051202Repo.AsForUpdate().GetDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
				_f051202List.AddRange(datas);
				f051202s.AddRange(datas);
			}
			return f051202s;
		}

		public List<F051203> GetF051203s(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			if (_f051203List == null)
				_f051203List = new List<F051203>();

			var f051203s = _f051203List.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PICK_ORD_NO == pickOrdNo).ToList();
			if (!f051203s.Any())
			{
				var datas = f051203Repo.AsForUpdate().GetDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
				_f051203List.AddRange(datas);
				f051203s.AddRange(datas);
			}
			return f051203s;
		}

		public List<F1511> GetF1511s(string dcCode, string gupCode, string custCode, string orderNo)
		{
			if (_f1511List == null)
				_f1511List = new List<F1511>();

			var f1511List = _f1511List.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORDER_NO == orderNo).ToList();
			if (!f1511List.Any())
			{
				var datas = f1511Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, orderNo).ToList();
				_f1511List.AddRange(datas);
				f1511List.AddRange(datas);
			}
			return f1511List;
		}

		/// <summary>
		/// 取得出貨單快取
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNo"></param>
		/// <returns></returns>
		public F050801 GetF050801(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			if (_f050801List == null)
				_f050801List = new List<F050801>();

			var f050801 = _f050801List.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsOrdNo);
			if (f050801 == null)
			{
				f050801 = f050801Repo.AsForUpdate().GetF050801ByWmsOrdNo(dcCode,gupCode,custCode,wmsOrdNo);
				_f050801List.Add(f050801);
			}
			return f050801;
		}

		/// <summary>
		/// 取得WMS優先權對應表
		/// </summary>
		/// <returns></returns>
		public List<F195601> GetF195601s()
		{
			if(_f195601List == null)
			{
				_f195601List = f195601Repo.GetAll().ToList();
			}
			return _f195601List;
		}

		public F0011 GetDatasForNotClosed(string dcCode, string gupCode, string custCode, string orderNo)
		{
			return GetDatasForNotClosed(dcCode, gupCode, custCode, new List<string> { orderNo }).FirstOrDefault();
		}

		public List<F0011> GetDatasForNotClosed(string dcCode, string gupCode, string custCode, List<string> orderNos)
		{
			if (_f0011List == null)
				_f0011List = new List<F0011>();

			var f0011List = _f0011List.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && orderNos.Contains(x.ORDER_NO)).ToList();

			var noExistOrderNos = orderNos.Except(f0011List.Select(x => x.ORDER_NO)).ToList();
			if (noExistOrderNos.Any())
			{
				int range = 1000;
				int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(noExistOrderNos.Count()) / range));
				for (int i = 0; i < index; i++)
				{
					var currOrderNos = noExistOrderNos.Skip(i * range).Take(range).ToList();
					var datas = f0011Repo.AsForUpdate().GetDatasForNotClosed(dcCode, gupCode, custCode, currOrderNos).ToList();
					_f0011List.AddRange(datas);
					f0011List.AddRange(datas);
				}
			}
			return f0011List;
		}

		#endregion

		#region  人員揀貨紀錄共用

		/// <summary>
		/// 更新人員揀貨完成紀錄 F0011
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNo"></param>
		private void UpdatePickFinishLog(string dcCode, string gupCode, string custCode, string pickOrdNo, DateTime finishTime)
		{
			var f0011 = GetDatasForNotClosed(dcCode, gupCode, custCode, pickOrdNo);
			if (f0011 != null)
			{
				// 更新揀貨完成時間為系統時間,STATUS=1
				f0011.CLOSE_DATE = finishTime;
				f0011.STATUS = "1";
				f0011Repo.Update(f0011);
			}
		}

		private void CreateAutoPickLog(string dcCode, string gupCode, string custCode, string pickOrdNo, string empId, DateTime startTime, DateTime finishTime)
		{
			f0011Repo.Add(new F0011
			{
				DC_CODE = dcCode,
				CUST_CODE = custCode,
				GUP_CODE = gupCode,
				EMP_ID = empId,
				ORDER_NO = pickOrdNo,
				STATUS = "1",
				START_DATE = startTime,
				CLOSE_DATE = finishTime
			});
		}

		#endregion

		#region 揀貨單開始共用
		public void StartPick(string dcCode,string gupCode,string custCode,string empId,List<string> pickOrdNos)
		{
			var empName = CommonService.GetEmpName(empId);
			var f051201s = GetF051201s(dcCode, gupCode, custCode, pickOrdNos);
			var pickWmsOrdNoList = f051202Repo.GetWmsOrdNoListByPickOrdNos(dcCode, gupCode, custCode, pickOrdNos).ToList();
			var f0011List = GetDatasForNotClosed(dcCode, gupCode, custCode, pickOrdNos);
      var pickStartTime = DateTime.Now;
      var updateF050801List = new List<string>();
      var updateF051201List = new List<string>();

      foreach (var f051201 in f051201s)
			{
				if (string.IsNullOrEmpty(f051201.PICK_STAFF) || f051201.PICK_STAFF.ToUpper() != empId.ToUpper())
				{
					if (!f051201.PICK_START_TIME.HasValue)
						f051201.PICK_START_TIME = pickStartTime;

          var wmsOrdNos = pickWmsOrdNoList.Where(x => x.PICK_ORD_NO == f051201.PICK_ORD_NO).Select(x => x.WMS_ORD_NO).ToList();
          updateF050801List.AddRange(wmsOrdNos);

          // 建立人員揀貨記錄
          var f0011 = f0011List.FirstOrDefault(x=> x.DC_CODE == f051201.DC_CODE && x.GUP_CODE == f051201.GUP_CODE && x.CUST_CODE == f051201.CUST_CODE && x.ORDER_NO == f051201.PICK_ORD_NO);

          if (f0011 != null && f0011.EMP_ID.ToUpper() != empId.ToUpper())
					{
						// 更新揀貨完成時間為系統時間,STATUS=1
						f0011.CLOSE_DATE = pickStartTime;
						f0011.STATUS = "1";
						f0011Repo.Update(f0011);
					}

					f0011Repo.Add(new F0011
					{
						DC_CODE = dcCode,
						CUST_CODE = custCode,
						GUP_CODE = gupCode,
						EMP_ID = empId,
						ORDER_NO = f051201.PICK_ORD_NO,
						STATUS = "0",
						START_DATE = pickStartTime
					});

          updateF051201List.Add(f051201.PICK_ORD_NO);
        }
			}

      f050801Repo.UpdateStartTime(dcCode, gupCode, custCode, updateF050801List, pickStartTime, empId, empName);
      f051201Repo.UpdateOrderToStartPick(dcCode, gupCode, custCode, updateF051201List, pickStartTime, 1, empId, empName);
      OrderService.AddF050305(dcCode, gupCode, custCode, pickOrdNos, "1");
		}

		#endregion

		#region 揀貨確認共用
		/// <summary>
		/// 揀貨確認共用
		/// </summary>
		/// <param name="param"></param>
		public void PickConfirm(PickConfirmParam param)
		{
			var containerService = new ContainerService(_wmsTransaction);
			var empName = CommonService.GetEmpName(param.EmpId);
			var startTime = !string.IsNullOrEmpty(param.StartTime) ? DateTime.Parse(param.StartTime) : DateTime.Now;
			var completeTime = !string.IsNullOrEmpty(param.CompleteTime) ? DateTime.Parse(param.CompleteTime) : DateTime.Now;

			// 找出揀貨單主檔
			var f051201 = GetF051201(param.DcCode, param.GupCode, param.CustCode, param.PickNo);

			// 找出揀貨單明細資料
			var f051202s = GetF051202s(param.DcCode, param.GupCode, param.CustCode, param.PickNo).ToList();
			// 找出揀貨單總揀明細資料
			var f051203s = GetF051203s(param.DcCode, param.GupCode, param.CustCode, param.PickNo).ToList();
		
			// 找出虛擬儲位檔資料
			var f1511s = GetF1511s(param.DcCode, param.GupCode, param.CustCode, param.PickNo).ToList();

			// 取得揀貨批次
			var f0513 = f0513Repo.GetF0513(f051201.DC_CODE,f051201.GUP_CODE,f051201.CUST_CODE,f051201.DELV_DATE,f051201.PICK_TIME);

			var addF05120601List = new List<F05120601>();
			var updF051301List = new List<F051301>();

			var isLackPick = false;

			// 更新揀貨明細資料
			foreach (var detail in param.Details)
			{
				F051203 f051203 = null;
				// 人工倉批量揀貨或自動倉特殊結構 更新總揀貨明細
				if ((f051201.DISP_SYSTEM == "0" && f051201.SPLIT_TYPE != "03") || (f051201.DISP_SYSTEM == "1" && (f051201.PICK_TYPE == "4" || f051201.NEXT_STEP == ((int)NextStep.CrossAllotPier).ToString())))
				{
					#region 更新F051203
					f051203 = f051203s.First(x => x.TTL_PICK_SEQ == detail.Seq);
					f051203.A_PICK_QTY = detail.Qty;
					f051203.PICK_STATUS = "1";

          f051203Repo.UpdatePickComplete(f051203.DC_CODE, f051203.GUP_CODE, f051203.CUST_CODE, f051203.PICK_ORD_NO, f051203.TTL_PICK_SEQ,
                                          detail.Qty, completeTime, param.EmpId, empName);
          #endregion
        }
				// 如果為自動倉非跨庫出貨 或 人工倉單一揀貨
				if ((f051201.DISP_SYSTEM == "1" && f051201.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString()) || (f051201.DISP_SYSTEM == "0" && f051201.SPLIT_TYPE == "03"))
				{
					var seqList = new List<string>();
					// 如果為自動倉特殊結構訂單 用總揀數量自動更新F051202
					if (f051201.DISP_SYSTEM == "1" && f051201.PICK_TYPE == "4" && f051203 != null)
					{
						if (string.IsNullOrEmpty(f051203.SERIAL_NO))
							seqList = f051202s.Where(x => x.ITEM_CODE == f051203.ITEM_CODE && x.VALID_DATE == f051203.VALID_DATE && x.MAKE_NO == f051203.MAKE_NO
							&& string.IsNullOrEmpty(x.SERIAL_NO)).Select(x => x.PICK_ORD_SEQ).ToList();
						else
							seqList = f051202s.Where(x => x.ITEM_CODE == f051203.ITEM_CODE && x.VALID_DATE == f051203.VALID_DATE && x.MAKE_NO == f051203.MAKE_NO
							&& x.SERIAL_NO == f051203.SERIAL_NO).Select(x => x.PICK_ORD_SEQ).ToList();
					}
					else
						seqList.Add(detail.Seq);


					foreach (var seq in seqList)
					{
						#region 更新F051202
						var f051202 = f051202s.First(x => x.PICK_ORD_SEQ == seq);
						var diffQty = (detail.Qty > f051202.B_PICK_QTY) ? f051202.B_PICK_QTY : detail.Qty;

						f051202.A_PICK_QTY = diffQty;
						f051202.PICK_STATUS = "1";

            f051202Repo.UpdatePickComplete(f051202.DC_CODE, f051202.GUP_CODE, f051202.CUST_CODE, f051202.PICK_ORD_NO, f051202.PICK_ORD_SEQ,
                                          diffQty, completeTime, param.EmpId, empName);
            #endregion

            #region 更新F1511
            var f1511 = f1511s.First(x => x.ORDER_SEQ == seq);

            f1511Repo.UpdateVirtualMoved(f1511.DC_CODE, f1511.GUP_CODE, f1511.CUST_CODE, f1511.ORDER_NO, f1511.ORDER_SEQ,
                                          diffQty, completeTime, param.EmpId, empName);
            #endregion

            #region 當明細揀貨完成(F051202.PICK_STATUS=1)且實際揀貨數<預計揀貨數 新增揀缺等待扣庫作業F05120601

            if (f051202.PICK_STATUS == "1" && diffQty < f051202.B_PICK_QTY)
						{
							var f05120601 = new F05120601
							{
								DC_CODE = f051202.DC_CODE,
								GUP_CODE = f051202.GUP_CODE,
								CUST_CODE = f051202.CUST_CODE,
								PICK_ORD_NO = f051202.PICK_ORD_NO,
								PICK_ORD_SEQ = f051202.PICK_ORD_SEQ,
								WMS_ORD_NO = f051202.WMS_ORD_NO,
								WMS_ORD_SEQ = f051202.WMS_ORD_SEQ,
								PICK_LOC = f051202.PICK_LOC,
								ITEM_CODE = f051202.ITEM_CODE,
								VALID_DATE = f051202.VALID_DATE,
								MAKE_NO = f051202.MAKE_NO,
								SERIAL_NO = f051202.SERIAL_NO,
								LACK_QTY = f051202.B_PICK_QTY - diffQty,
								STATUS = "0",
								CRT_DATE = completeTime,
								CRT_STAFF = param.EmpId,
								CRT_NAME = empName,
								CUST_COST = f0513.CUST_COST,
								SOURCE_TYPE = f0513.SOURCE_TYPE,
								FAST_DEAL_TYPE = f0513.FAST_DEAL_TYPE,
								ORD_TYPE = f0513.ORD_TYPE,
								ENTER_DATE = f051202.ENTER_DATE
							};
							addF05120601List.Add(f05120601);
							isLackPick = true;
						}

						#endregion
					}
				}
			}

			var wmsOrdNos = f051202s.Select(x => x.WMS_ORD_NO).Distinct().ToList();

			// 完成揀貨明細狀態
			var finishStatusList = new List<string> { "1", "9" };
			if ((f051203s.Any() && f051203s.All(x => finishStatusList.Contains(x.PICK_STATUS))) || (f051202s.Any() && f051202s.All(x => finishStatusList.Contains(x.PICK_STATUS))))
			{
				f051201.PICK_STATUS = 2;
				// 更新揀貨單完成時間
				f051201.PICK_FINISH_DATE = completeTime;
				f051201.UPD_DATE = DateTime.Now;
				f051201.UPD_STAFF = param.EmpId;
				f051201.UPD_NAME = empName;

				if (f051201.DISP_SYSTEM != "0")
				{
					f051201.PICK_STAFF = param.EmpId;
					f051201.PICK_NAME = empName;

					// 更新揀貨單開始時間
					f051201.PICK_START_TIME = startTime;
					CreateAutoPickLog(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.PICK_ORD_NO, param.EmpId, f051201.PICK_START_TIME.Value, f051201.PICK_FINISH_DATE.Value);
					if (f051201.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString())
					{
						// 更新出貨單開始揀貨時間
						f050801Repo.UpdateStartTime(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, wmsOrdNos, f051201.PICK_START_TIME.Value, param.EmpId, empName);
						
						// 無缺貨 
						if(!isLackPick)
							// 更新出貨單揀貨完成時間
							f050801Repo.UpdateCompleteTime(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, wmsOrdNos, f051201.PICK_FINISH_DATE.Value, param.EmpId, empName);
					}

					//No1218需求:若揀貨單為自動倉揀貨，容器資料有多箱時，要轉集貨
					//1.如果揀貨單為自動倉揀貨單(F051201.DISP_SYSTEM!=0) AND 回傳的容器條碼筆數>1
					if (param.ContainerData.Select(x => x.CONTAINERCODE).Distinct().Count() > 1)
					{
            //(1) 如果PICK_TYPE=6 且F051301.STATUS=1(不需集貨) WHERE WMS_NO=第一筆F051202.WMS_ORD_NO更新F051301.STATUS = 0(需集貨)
            //如果是系統報缺就不調整集貨狀態
            if (f051201.PICK_TYPE == "6" && !param.IsAutoWHException)
            {
              var f051301s = f051301Repo.AsForUpdate().GetF051301s(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.DELV_DATE.Value, f051201.PICK_TIME, wmsOrdNos).ToList();
              f051301s.ForEach(x =>
              {
                x.STATUS = "0";
                x.UPD_DATE = DateTime.Now;
                x.UPD_STAFF = param.EmpId;
                x.UPD_NAME = empName;
              });
              updF051301List = f051301s;
            }
            //(2) 如果PICK_TYPE IN (4) 特殊結構揀貨單 且F051301.STATUS=1(不需集貨) WHERE WMS_NO=F051201.PICK_ORD_NO更新F051301.STATUS=0(需集貨)
            //如果是系統報缺就不調整集貨狀態
            else if (f051201.PICK_TYPE == "4" && !param.IsAutoWHException)
            {
              var f051301s = f051301Repo.AsForUpdate().GetF051301s(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.DELV_DATE.Value, f051201.PICK_TIME, new List<string> { f051201.PICK_ORD_NO }).ToList();
              f051301s.ForEach(x =>
              {
                x.STATUS = "0";
                x.UPD_DATE = DateTime.Now;
                x.UPD_STAFF = param.EmpId;
                x.UPD_NAME = empName;
              });
              updF051301List = f051301s;
						}
						//F051201.NEXT_STEP != 6 (調撥場) 且上述變成需集貨，也要更新F051301.NEXT_STEP=2(出貨集貨場)
						if (f051201.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString() & (updF051301List?.Any() ?? false))
							updF051301List.ForEach(x => x.NEXT_STEP = "2");
					}
        }
        else
				{
					UpdatePickFinishLog(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.PICK_ORD_NO, f051201.PICK_FINISH_DATE.Value);
					// 人工倉單一揀貨/人工倉補揀單
					if ((f051201.DISP_SYSTEM == "0" && f051201.SPLIT_TYPE == "03"))
					{
            // 無缺貨 
            if (!isLackPick)
              // 更新出貨單揀貨完成時間
							f050801Repo.UpdateCompleteTime(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, wmsOrdNos, f051201.PICK_FINISH_DATE.Value, param.EmpId, empName);
					}
				}

        // 人工倉單一揀貨(含快速補揀單) 或 自動倉自我滿足揀貨(非跨庫出貨)
        // 如果是系統報缺就不調整集貨狀態
        if (((f051201.DISP_SYSTEM == "0" && f051201.SPLIT_TYPE == "03") ||
          (f051201.DISP_SYSTEM == "1" && f051201.PICK_TYPE == "6" && f051201.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString()))
          && !param.IsAutoWHException)
        {
          // 有缺貨出貨單需更新F051301狀態改為需集貨(0)，下一步改為集貨場
          var lackWmsNos = f051202s.Where(x => x.B_PICK_QTY - x.A_PICK_QTY > 0).Select(x => x.WMS_ORD_NO).Distinct().ToList();
          if (lackWmsNos.Any())
          {
            f051201.NEXT_STEP = ((int)NextStep.CollectionStation).ToString();
            updF051301List = f051301Repo.GetF051301s(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.DELV_DATE.Value, f051201.PICK_TIME, lackWmsNos).ToList();
            updF051301List.ForEach(x =>
            {
              if (x.STATUS == "1")
              {
                x.STATUS = "0";
                x.NEXT_STEP = ((int)NextStep.CollectionStation).ToString();
                x.UPD_DATE = DateTime.Now;
                x.UPD_STAFF = param.EmpId;
                x.UPD_NAME = empName;
              }
            });
          }
        }

        // 人工倉單一揀貨或特殊結構訂單 寫入容器資料(容器編號=揀貨單號)
        if (f051201.DISP_SYSTEM == "0" && (f051201.SPLIT_TYPE == "03" || f051201.PICK_TYPE == ((int)PickTypeEnums.SpecialOrderPick).ToString()))
        {
          containerService.CreateContainer(f051202s
              .GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.PICK_ORD_NO, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO })
              .Select(x => new CreateContainerParam
              {
                DC_CODE = x.Key.DC_CODE,
                GUP_CODE = x.Key.GUP_CODE,
                CUST_CODE = x.Key.CUST_CODE,
                CONTAINER_CODE = x.Key.PICK_ORD_NO,
                CONTAINER_TYPE = "0",
                ITEM_CODE = x.Key.ITEM_CODE,
                WAREHOUSE_ID = "NA",
                VALID_DATE = x.Key.VALID_DATE,
                MAKE_NO = x.Key.MAKE_NO,
                WMS_TYPE = "O",
                QTY = x.Sum(y => y.A_PICK_QTY),
                WMS_NO = (f051201.PICK_TYPE == ((int)PickTypeEnums.SpecialOrderPick).ToString()) ? f051201.PICK_ORD_NO : f051201.SPLIT_CODE,
                SERIAL_NO_LIST = x.Where(y => !string.IsNullOrEmpty(y.SERIAL_NO)).Select(y => y.SERIAL_NO).ToList(),
                PICK_ORD_NO = f051201.PICK_ORD_NO
              }).ToList());
        }

        // 若為跨庫出貨人工倉揀貨單，則寫入容器資料
        if (f051201.DISP_SYSTEM == "0" && f051201.NEXT_STEP == ((int)NextStep.CrossAllotPier).ToString())
				{
					  param.ContainerResults = containerService.CreateContainer(f051203s
            .GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.PICK_ORD_NO, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO })
            .Select(x => new CreateContainerParam
            {
              DC_CODE = x.Key.DC_CODE,
              GUP_CODE = x.Key.GUP_CODE,
              CUST_CODE = x.Key.CUST_CODE,
              CONTAINER_CODE = x.Key.PICK_ORD_NO,
              CONTAINER_TYPE = "0",
              ITEM_CODE = x.Key.ITEM_CODE,
              WAREHOUSE_ID = "NA",
              VALID_DATE = x.Key.VALID_DATE,
              MAKE_NO = x.Key.MAKE_NO,
              WMS_TYPE = "P",
              QTY = x.Sum(y => y.A_PICK_QTY),
              WMS_NO = f051201.PICK_ORD_NO,
              SERIAL_NO_LIST = x.Where(y => !string.IsNullOrEmpty(y.SERIAL_NO)).Select(y => y.SERIAL_NO).ToList(),
              PICK_ORD_NO = f051201.PICK_ORD_NO
            }).ToList());
        }

        // No.2193 如果揀貨單為跨庫調撥，將揀貨容器增加寫入F0534資料表(STATUS=0)
        if (f051201.NEXT_STEP == ((int)NextStep.CrossAllotPier).ToString())
        {
          param.ContainerResults.ForEach(o =>
            f0534Repo.Add(new F0534
            {
              F0701_ID = o.f0701_ID,
              DC_CODE = f051201.DC_CODE,
              GUP_CODE = f051201.GUP_CODE,
              CUST_CODE = f051201.CUST_CODE,
              PICK_ORD_NO = f051201.PICK_ORD_NO,
              CONTAINER_CODE = o.ContainerCode,
              MOVE_OUT_TARGET = f051201.MOVE_OUT_TARGET,
              STATUS = "0",
              DEVICE_TYPE = f051201.DEVICE_TYPE,
              TOTAL = o.Qty
            })
          );
        }

        #region 更新儲位容積

        UpdatePickOrdNoLocVolumn(param.DcCode, param.GupCode, param.CustCode, new List<string> { f051201.PICK_ORD_NO });

        #endregion
      }

      #region 集貨等待通知處理
      if (f051201.DISP_SYSTEM == "1" &&
          f051201.ORD_TYPE == "1" &&
          f051201.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString() &&
          f051201.PICK_STATUS == 2 && !isLackPick)
      {
				var isFindUpdF051301 = false;
				var f051301 = updF051301List.FirstOrDefault(x => x.DC_CODE == f051201.DC_CODE && x.GUP_CODE == f051201.GUP_CODE && x.CUST_CODE == f051201.CUST_CODE && x.DELV_DATE == f051201.DELV_DATE.Value && x.PICK_TIME == f051201.PICK_TIME && x.WMS_NO == param.WmsNo);
				if (f051301 == null)
					f051301 = f051301Repo.GetF051301(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.DELV_DATE.Value, f051201.PICK_TIME, param.WmsNo);
				else
					isFindUpdF051301 = true;


        int f060702Status = 9999;
        //檢查訂單是否取消
        var f050101s = f050101Repo.GetOrdNoByWmsOrdNo(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, param.WmsNo);

        // 此單據為訂單取消，
        // 集貨等待通知 =2(異常處理)
        if (f050101s.Any(x => x.STATUS == "9") || (f051301 != null && f051301.COLLECTION_POSITION == "0"))
          f060702Status = 2;

        if (f060702Status != 9999)
          f060702Repo.Add(new F060702()
          {
            DC_CODE = f051201.DC_CODE,
            CUST_CODE = f051201.CUST_CODE,
            GUP_CODE = f051201.GUP_CODE,
            ORDER_CODE = param.DocID,
            ORI_ORDER_CODE = f051202s.FirstOrDefault()?.WMS_ORD_NO,
            STATUS = f060702Status,
            PROC_FLAG = "0",
            PROC_DATE = DateTime.Now,
            MESSAGE = "",
            RESENT_CNT = 0,
          });

        // 如果集貨等待通知=2(異常處理) 且原集貨場位置=自動倉集貨場 則變更為人工集貨場
        if (f051301 != null && f060702Status == 2 && f051301.COLLECTION_POSITION == "1")
        {
          f051301.COLLECTION_POSITION = "0";
          f051301.UPD_DATE = DateTime.Now;
          f051301.UPD_NAME = empName;
          f051301.UPD_STAFF = param.EmpId;
          if (!isFindUpdF051301)
            updF051301List.Add(f051301);
        }
      }

      #endregion 集貨等待通知處理

      if (updF051301List.Any())
				f051301Repo.BulkUpdate(updF051301List, true);

			if(addF05120601List.Any())
				f05120601Repo.BulkInsert(addF05120601List, true, "ID");

			f051201Repo.Update(f051201, true);
    }

    #endregion

    #region 自動產生揀貨資料排程
    /// <summary>
    /// 自動產生揀貨資料排程
    /// </summary>
    /// <param name="source">資料來源[01:訂單;02:揀缺;03:LMS揀貨批次分配失敗]</param>
    /// <returns></returns>
    public AutoCreatePickRes AutoCreatePick(string source)
		{
			_wmsLogHelper = new WmsLogHelper();
			var f050306s = new List<F050306>();
      var AutoCreatePickSource = new[] { "01", "03" };

      var apiresult = ApiLogHelper.CreateApiLogInfo("0", "0", "0", AutoCreatePickSource.Contains(source) ? "AutoCreatePick" : "AutoCreateLackPick", new { Source = source }, () =>
			 {
				 _f050306Repo = new F050306Repository(Schemas.CoreSchema, _wmsTransaction);
				 var result = new ExecuteResult(false);
				 if (AutoCreatePickSource.Contains(source))
				 {
					 _wmsLogHelper.StartRecord(WmsLogProcType.AutoCreatePick);
					 var limitDcList = GetCanAutoPickDcList();
					 if (limitDcList.Any())
					 {
							 f050306s = _f050306Repo.GetDatasBySource(source, limitDcList).ToList();
						   result = CreatePick(source, f050306s, true);
					 }
					 else
					 {
						 _wmsLogHelper.AddRecord("沒有任何符合自動產揀貨單時間的物流中心");
						 _wmsLogHelper.StopRecord();
						 result.Message = "沒有任何符合自動產揀貨單時間的物流中心";
					 }
				 }
				 else if (source == "02")
				 {
					 _wmsLogHelper.StartRecord(WmsLogProcType.AutoCreateLackPick);
					 f050306s = _f050306Repo.GetDatasBySource(source, new List<string>()).ToList();
					 result = CreatePick(source, f050306s, true);
				 }

				 if(AutoCreatePickSource.Contains(source))
				 {
					 List<F050301> CanceledOrders;
					 // 因為產揀貨單排程會BY 批次進行Commit後會重設WmsTransaction，這樣WmsTransaction記憶體就會不一樣，所以必須重新new新的記憶體和揀貨單服務，才能夠在第一次產完揀貨單後可以將取消訂單正常取消成功
					 _wmsTransaction = new WmsTransaction();
					 var checkResult = AfterCreatePickCheckOrder(out CanceledOrders);
					 if (!checkResult.IsSuccessed)
					 {
						 result.IsSuccessed = false;
						 result.Message += "," + checkResult.Message;
					 }
					 else
						 _wmsTransaction.Complete();

					 if (CanceledOrders.Count > 0)
					 {
						 if (!string.IsNullOrEmpty(result.Message))
							 result.Message += Environment.NewLine + $"訂單被LMS取消{CanceledOrders.Count}筆，取消訂單編號:"+ string.Join("、",CanceledOrders.Select(x=> x.ORD_NO).ToArray());
						 else
							 result.Message = $"訂單被LMS取消{CanceledOrders.Count}筆，取消訂單編號:" + string.Join("、", CanceledOrders.Select(x => x.ORD_NO).ToArray());
					 }
				 }

         return new ApiResult
				 {
					 IsSuccessed = result.IsSuccessed,
					 MsgCode = "",
					 MsgContent = result.Message
         };
			 }, true);

			return new AutoCreatePickRes() { IsSuccessed = apiresult.IsSuccessed, MsgCode = apiresult.MsgCode, MsgContent = apiresult.MsgContent, ProcF050306s = f050306s };
		}

    #region 取得符合自動揀貨時間的物流中心清單
    /// <summary>
    /// 取得符合自動揀貨時間的物流中心清單
    /// </summary>
    /// <returns></returns>
    private List<string> GetCanAutoPickDcList()
		{
			var limitDcList = new List<string>();
			var f190106Repo = new F190106Repository(Schemas.CoreSchema);
			var nowTime = DateTime.Now;
			var dcPickScheduleTimeList = f190106Repo.GetDatasByTrueAndCondition(x => x.SCHEDULE_TYPE == "02").GroupBy(x => x.DC_CODE).ToList();
			foreach (var dc in dcPickScheduleTimeList)
			{
				foreach (var f190106 in dc)
				{
					var startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, int.Parse(f190106.START_TIME.Split(':')[0]), int.Parse(f190106.START_TIME.Split(':')[1]), 0);
					var endTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, int.Parse(f190106.END_TIME.Split(':')[0]), int.Parse(f190106.END_TIME.Split(':')[1]), 0);
					if (endTime < startTime)
						endTime = endTime.AddDays(1);
					if (nowTime >= startTime && nowTime <= endTime)
					{
						var ts = nowTime - startTime;
						if (Math.Floor(ts.TotalMinutes) % f190106.PERIOD == 0)
							limitDcList.Add(dc.Key);
						break;
					}
				}
			}
			return limitDcList;
		}
		#endregion

		#endregion

		#region 產生揀貨資料共用快取

		/// <summary>
		/// 取得配庫後訂單資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordNo"></param>
		/// <returns></returns>
		private F050301 GetF050301(string dcCode, string gupCode, string custCode, string ordNo)
		{
			if (_f050301Repo == null)
				_f050301Repo = new F050301Repository(Schemas.CoreSchema);
			if (_f050301s == null)
				_f050301s = new List<F050301>();
			var f050301 = _f050301s.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo);
			if (f050301 == null)
			{
				f050301 = _f050301Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo);
				if (f050301 != null)
					_f050301s.Add(f050301);
			}
			return f050301;
		}

		/// <summary>
		/// 取得商品材積
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		private List<F1905> GetF1905s(string gupCode,string custCode, List<string> itemCodes)
		{
			if (_f1905s == null)
				_f1905s = new List<F1905>();
			// 找出還沒有快取的F1905
			var newItemCodes = itemCodes.Where(itemCode => _f1905s.All(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE != itemCode)).ToList(); ;
			if (newItemCodes.Any())
			{
				var f1905Rep = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);
				var f1905s = f1905Rep.InWithTrueAndCondition("ITEM_CODE", newItemCodes, a => a.GUP_CODE == gupCode && a.CUST_CODE==custCode).ToList();
				_f1905s.AddRange(f1905s);
			}
			return _f1905s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && itemCodes.Contains(x.ITEM_CODE)).ToList();
		}

    /// <summary>
    /// 取得集貨場資料
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="IsReturn">true:取得廠退集貨場資料 false:取得訂單集貨場資料 null:不篩選</param>
    /// <returns></returns>
    private List<F1945> GetDcF1945s(string dcCode, Boolean? IsReturn = null)
    {
      var f1945Repo = new F1945Repository(Schemas.CoreSchema);
      if (_f1945s == null)
        _f1945s = new List<F1945>();
      var dcF1945s = _f1945s.Where(x => x.DC_CODE == dcCode).ToList();
      if (!dcF1945s.Any())
      {
        dcF1945s = f1945Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode).ToList();
        _f1945s.AddRange(dcF1945s);
      }

      #region 是否為廠退或訂單篩選集貨場
      if (IsReturn.HasValue)
      {
        IEnumerable<F1945> tmpF1945s = new List<F1945>();
        if (IsReturn.Value)
          tmpF1945s = dcF1945s.Where(x => x.COLLECTION_TYPE == "2");

        if (tmpF1945s.Any())
          dcF1945s = tmpF1945s.ToList();
        else
          dcF1945s = dcF1945s.Where(x => x.COLLECTION_TYPE == "0").ToList();
      }
      else
        dcF1945s = dcF1945s.Where(x => x.COLLECTION_TYPE == "0").ToList();
      #endregion 是否為廠退或訂單篩選集貨場

      return dcF1945s;
    }


    private List<F194501> GetDcF194501s(string dcCode)
		{
			var f194501Repo = new F194501Repository(Schemas.CoreSchema);
			if (_f194501s == null)
				_f194501s = new List<F194501>();
			var dcF194501s = _f194501s.Where(x => x.DC_CODE == dcCode).ToList();
			if (!dcF194501s.Any())
			{
				dcF194501s = f194501Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode).ToList();
				_f194501s.AddRange(dcF194501s);
			}
			return dcF194501s;
		}

		#endregion

		#region 產生揀貨資料共用方法


		/// <summary>
		/// 產生揀貨資料
		/// </summary>
		/// <param name="source">資料來源[01:訂單;02:揀缺]</param>
		/// <param name="f050306s">配庫後揀貨資料</param>
		/// <param name="isPickSchedule">是否揀貨排程執行</param>
		/// <param name="f050301s">手動挑單傳入配庫後訂單</param>
		/// <returns></returns>
		public ExecuteResult CreatePick(string source, List<F050306> f050306s, bool isPickSchedule, List<F050301> f050301s = null,bool isUserDirectPriorityCode = false)
		{
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);

			if (_f050301s == null)
				_f050301s = new List<F050301>();

			if (f050301s != null)
				_f050301s.AddRange(f050301s);
			if (!isPickSchedule)
			{
				_wmsLogHelper = new WmsLogHelper();
				_wmsLogHelper.StartRecord(WmsLogProcType.CreatePick);
			}
			var result = new ExecuteResult(true);
			switch (source)
			{
				case "01": //訂單
        case "03": //LMS揀貨批次分配失敗
                   //1.[A] =取得揀貨批次模式(SYS_PATH FROM F0003 WHERE AP_NAME=PickAllotMode,SYS_PATH=1,DC_CODE=00,GUP_CODE=00,CUST_CODE=00)
                   //2.在case source =01，如果[A]=0 則呼叫原本的CreateOrderPick方法，如果[A]=1則呼叫新的CreateOrderPickWithApi方法
          var SysPickMode = f0003Repo.Find(x => x.AP_NAME == "PickAllotMode" && x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00").SYS_PATH;
          if (SysPickMode == "0")
						result = CreateOrderPick(f050306s, isPickSchedule, isUserDirectPriorityCode);
					else
						result = CreateOrderPickWithApi(f050306s, isPickSchedule);

					if (!result.IsSuccessed)
						_wmsLogHelper.AddRecord(result.Message);
					break;
				case "02": //揀缺
					result = CreateLackPick(f050306s);
					if (!result.IsSuccessed)
						_wmsLogHelper.AddRecord(result.Message);
					break;
			}
			_wmsLogHelper.StopRecord();
			return result;
		}

		#region 呼叫LMS API產生揀貨單
		/// <summary>
		/// 呼叫LMS API產生揀貨單
		/// </summary>
		/// <param name="f050306s">F050306清單</param>
		/// <param name="isPickSchedule">是否揀貨排程執行</param>
		/// <returns></returns>
		private ExecuteResult CreateOrderPickWithApi(List<F050306> f050306s, bool isPickSchedule)
		{
			BatchPickService batchPickService = new BatchPickService(_wmsTransaction);
			var faildTransaction = new WmsTransaction();

			var f0093Repo = new F0093Repository(Schemas.CoreSchema, faildTransaction);
			var f050306Repo = new F050306Repository(Schemas.CoreSchema, faildTransaction);
			var f190105Repo = new F190105Repository(Schemas.CoreSchema);
			var result = new ExecuteResult(true);

			List<F1980> f1980s = new List<F1980>(); ;
			String strLMSMsg;
			List<string> errMsg = new List<string>();
			BatchPickAllotOrderData tmpBatchPickAllotOrderData;
      List<String> ErrorBatchPickAllotWmsNo;
      List<BatchPickAllotOrderData> OrderList = new List<BatchPickAllotOrderData>();
			List<BatchPickAllotItemData> BatchPickAllotItemDatas = new List<BatchPickAllotItemData>();
			List<F190105> getDeliverySetttings = new List<F190105>();
			BatchPickAllotRes tmpBatchPickAllotRes = new BatchPickAllotRes();
			//[MM]=建立暫存API產生後資料批次清單
			Dictionary<ApiResultKey, List<BatchPickAllotPickingBatchData>> apiResultDatas = new Dictionary<ApiResultKey, List<BatchPickAllotPickingBatchData>>();

			strLMSMsg = CommonService.GetMsg("API20003");

			int PICKAPI_SEQ = 1;
			foreach (var item in f050306s)
			{
				//[CC].PICKAPI_SEQ = 資料索引+1轉字串6碼不足往前補0
				item.PICKAPI_SEQ = PICKAPI_SEQ.ToString("000000");
				PICKAPI_SEQ++;
			}

			//(2)	[A2] Group by DC_CODE,GUP_CODE,CUST_CODE,ORD_TYPE,SOURCE_TYPE,CUST_COST,FAST_DEAL_TYPE,MOVE_OUT_TARGET,WH_TMPR_TYPE,RTN_VNR_CODE,PACKING_TYPE
			//ORDER BY FAST_DEAL_TYPE DESC
			var f050306GroupKey = f050306s.GroupBy(g =>
					new { DC_CODE = g.DC_CODE, GUP_CODE = g.GUP_CODE, CUST_CODE = g.CUST_CODE, ORD_TYPE = g.ORD_TYPE, SOURCE_TYPE = g.SOURCE_TYPE, CUST_COST = g.CUST_COST, FAST_DEAL_TYPE = g.FAST_DEAL_TYPE, MOVE_OUT_TARGET = g.MOVE_OUT_TARGET, WH_TMPR_TYPE = g.WH_TMPR_TYPE, RTN_VNR_CODE = g.RTN_VNR_CODE, PACKING_TYPE = g.PACKING_TYPE }, (a, b) => new { Key = new ApiResultKey { DC_CODE = a.DC_CODE, GUP_CODE = a.GUP_CODE, CUST_CODE = a.CUST_CODE, ORD_TYPE = a.ORD_TYPE, SOURCE_TYPE = a.SOURCE_TYPE, CUST_COST = a.CUST_COST, FAST_DEAL_TYPE = a.FAST_DEAL_TYPE, MOVE_OUT_TARGET = a.MOVE_OUT_TARGET, WH_TMPR_TYPE = a.WH_TMPR_TYPE, RTN_VNR_CODE = a.RTN_VNR_CODE, PACKING_TYPE = a.PACKING_TYPE }, Group = b }).OrderByDescending(x => x.Key.FAST_DEAL_TYPE);

			//(3)	Foreach [A3] IN [A2]
			foreach (var GroupA3 in f050306GroupKey) //單據主檔
			{
				OrderList.Clear();

				var groupOrders = GroupA3.Group.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.WMS_NO });

				foreach (var groupOrder in groupOrders)
				{
					BatchPickAllotItemDatas = new List<BatchPickAllotItemData>();
					foreach (var GroupA3Item in groupOrder) //單據明細
					{
						BatchPickAllotItemDatas.Add(new BatchPickAllotItemData()
						{
							WmsSeq = GroupA3Item.PICKAPI_SEQ,
							ItemCode = GroupA3Item.ITEM_CODE,
							Qty = GroupA3Item.B_PICK_QTY,
							LocCode = GroupA3Item.PICK_LOC,
							ValidDate = GroupA3Item.VALID_DATE.ToString("yyyy/MM/dd"),
							EnterDate = GroupA3Item.ENTER_DATE.ToString("yyyy/MM/dd"),
							MakeNo = GroupA3Item.MAKE_NO,
							SerialNo = GroupA3Item.SERIAL_NO == "0" ? null : GroupA3Item.SERIAL_NO,
							PickingSystem = GroupA3Item.DEVICE_TYPE == "0" ? 0 : 1
						});
					}

					tmpBatchPickAllotOrderData = new BatchPickAllotOrderData()
					{
						WmsNo = groupOrder.Key.WMS_NO,
						//IF [A3].Key.CUST_COST=MoveOut 設為[A3].MOVE_OUT_TARGET 
						//IF[A3].Key.SOURCE_TYPE = 13
						//設為[A3].Key.RTN_VNR_CODE
						//Else
						//設為[A4].Key.PACKING_TYPE
						TargetCode =
								GroupA3.Key.CUST_COST == "MoveOut" ? GroupA3.Key.MOVE_OUT_TARGET :
										GroupA3.Key.SOURCE_TYPE == "13" ? GroupA3.Key.RTN_VNR_CODE : GroupA3.Key.PACKING_TYPE,
						Items = BatchPickAllotItemDatas
					};
					OrderList.Add(tmpBatchPickAllotOrderData);

				}


				//IF [A3] .Key.CUST_COST=MoveOut 設為2
				//IF [A3] .Key.SOURCE_TYPE=13
				//設為3
				//Else
				//設為1
				var OrderType = GroupA3.Key.CUST_COST?.ToUpper() == "MoveOut".ToUpper() ? "2" : GroupA3.Key.SOURCE_TYPE == "13" ? "3" : "1";
				//a. [A4] = 呼叫揀貨批次分配API，將[A3]組成傳入參數[參考1.2.1 揀貨批次分配API傳入參數對應]
				tmpBatchPickAllotRes = batchPickService.BatchPickAllot(GroupA3.Key.DC_CODE, GroupA3.Key.GUP_CODE, GroupA3.Key.CUST_CODE, OrderType, OrderList);

				if (!tmpBatchPickAllotRes.IsSuccessed)
				{
					errMsg.Add(String.Format(strLMSMsg, tmpBatchPickAllotRes.Message));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule);
					if (isPickSchedule)
						continue;
					else
						break;
				}

        #region no1279 檢查LMS回傳參數是否有錯

        #region 檢查LMS回傳欄位長度＆必填欄位
        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.Where(x => x.PickingBatchNo?.Length > 5).SelectMany(a=>a.PickingList.SelectMany(b=>b.Details.SelectMany(c=>c.Orders.Select(d=>d.WmsNo)))).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "揀貨批次號長度過長"));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
					if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.Where(x => String.IsNullOrWhiteSpace(x.PickingType)).SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo)))).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "揀貨單類型為空值"));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
					if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.Where(x => x.PickingType?.Length > 1).SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo)))).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "揀貨單類型長度過長"));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
					if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList).Where(a => a.PickingNo?.Length > 4).SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo))).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
        {
          errMsg.Add(String.Format(strLMSMsg, "揀貨單號長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList).Where(a => a.PickAreaID?.Length > 5).SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo))).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "揀貨區編號/倉別代號長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList).Where(a => a.PickAreaName?.Length > 30).SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo))).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "揀貨區名稱/倉別名稱長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList).Where(a => a.NextStepCode?.Length > 1).SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo))).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "下一個作業長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList).Where(a => a.TargetCode?.Length > 10).SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo))).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "目的地代碼長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details)).Where(a => a.Qty <= 0).SelectMany(c => c.Orders.Select(d => d.WmsNo)).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
        {
          errMsg.Add(String.Format(strLMSMsg, "揀貨數量小於等於0"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details)).Where(a => a.PickAreaID?.Length > 5).SelectMany(c => c.Orders.Select(d => d.WmsNo)).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "揀貨區編號/倉別長度過長"));
            InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
            if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details)).Where(a => a.PickAreaName?.Length > 20).SelectMany(c => c.Orders.Select(d => d.WmsNo)).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "揀貨區名稱長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details)).Where(a => a.LocCode?.Length > 9).SelectMany(c => c.Orders.Select(d => d.WmsNo)).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "揀貨儲位長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details)).Where(x =>
        {
          DateTime tmpDatetime;
          return !DateTime.TryParse(x.ValidDate, out tmpDatetime);
        }).SelectMany(c => c.Orders.Select(d => d.WmsNo)).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "效期無法辨識"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details)).Where(x =>
        {
          DateTime tmpDatetime;
          return !DateTime.TryParse(x.EnterDate, out tmpDatetime);
        }).SelectMany(c => c.Orders.Select(d => d.WmsNo)).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "入庫日無法辨識"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details)).Where(a => a.MakeNo?.Length > 40).SelectMany(c => c.Orders.Select(d => d.WmsNo)).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "批號長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details)).Where(a => a.SerialNo?.Length > 50).SelectMany(c => c.Orders.Select(d => d.WmsNo)).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "序號長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        //這個沒辦法對應到原本的揀貨批次資料，就只能把這批資料全部打入異常了
				if (tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo)))).Any(x => string.IsNullOrWhiteSpace(x)))
				{
					errMsg.Add(String.Format(strLMSMsg, "出貨單號為空"));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule);
					if (isPickSchedule)
						continue;
					else
						break;
				}

        //這個沒辦法對應到原本的揀貨批次資料，就只能把這批資料全部打入異常了
        if (tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo)))).Any(x => (x?.Length ?? 0) > 20))
				{
					errMsg.Add(String.Format(strLMSMsg, "出貨單號長度過長"));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule);
					if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders))).Where(a => string.IsNullOrWhiteSpace(a.WmsSeq)).Select(d => d.WmsNo).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
        {
          errMsg.Add(String.Format(strLMSMsg, "出貨明細項次為空"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
            continue;
          else
            break;
        }

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders))).Where(a => a.WmsSeq?.Length > 6).Select(d => d.WmsNo).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
        {
          errMsg.Add(String.Format(strLMSMsg, "出貨明細項次長度過長"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders))).Where(a => a.Qty <= 0).Select(d => d.WmsNo).ToList();
        if (ErrorBatchPickAllotWmsNo.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, "需求數量小於等於0"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}


        #endregion 檢查LMS回傳欄位長度＆必填欄位

        #region 檢查PickingSystem
        var CheckRtnPickingSystem = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList).Where(a => !new int[] { 0, 1 }.Contains(a.PickingSystem));
        var GetErrRtnPickingSystem = CheckRtnPickingSystem.Select(x => x.PickingSystem).Distinct();
        if (CheckRtnPickingSystem.Any())
				{
          ErrorBatchPickAllotWmsNo= CheckRtnPickingSystem.SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo))).ToList();

          errMsg.Add(String.Format(strLMSMsg, $"LMS揀貨系統{String.Join(",", GetErrRtnPickingSystem)}不存在"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          if (isPickSchedule)
						continue;
					else
						break;
				}

        //沒辦法判斷是哪幾張揀貨單有問題，全部寫入f0093
        var GetRtnPickingSystem = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.Select(b => b.PickingSystem)).Distinct();
        var GetReqPickSystem = GroupA3.Group.Select(x => x.DEVICE_TYPE == "0" ? 0 : 1).Distinct().Except(GetRtnPickingSystem);
				if (GetReqPickSystem.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, $"缺少LMS揀貨系統{String.Join(",", GetReqPickSystem)}"));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule);
					if (isPickSchedule)
						continue;
					else
						break;
				}

        #endregion 檢查PickingSystem

        #region 檢查PickingType
        var CheckPickingType = (from a in tmpBatchPickAllotRes.Data
                                from b in a.PickingList
                                select new { a.PickingType, b.PickingSystem, b.Details })
                               .Distinct();
        foreach (var item in CheckPickingType)
				{
					//用LMSPackingTypeToWmsPackingType檢查之後如果有新項目在這裡面改就可以
					try
					{ LMSPackingTypeToWmsPackingType(item.PickingType, item.PickingSystem != 0); }
					catch (Exception)
					{
						errMsg.Add(String.Format(strLMSMsg, $"LMS揀貨單類型{item.PickingType}不存在"));
            InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, item.Details.SelectMany(a => a.Orders.Select(b => b.WmsNo)).ToList());
            if (isPickSchedule)
							continue;
						else
							break;
					}
				}
				#endregion 檢查PickingType

				#region 檢查PickAreaID

				var CheckPickAreaID = from a in tmpBatchPickAllotRes.Data
															from b in a.PickingList
															from c in b.Details
                              select new { a.PickingType, b.PickingSystem, b.PickAreaID, c.LocCode, WmsNos = c.Orders.Select(x => x.WmsNo) };
        CheckPickAreaID = CheckPickAreaID.ToList().Where(x => !(x.PickingType == "1" && x.PickingSystem == 0) && String.IsNullOrWhiteSpace(x.PickAreaID));
				if (CheckPickAreaID.Any())
				{
					var UnSetPickAreaID = CheckPickAreaID.Select(x => x.LocCode).Distinct();
					errMsg.Add(String.Format(strLMSMsg, $"LMS PickAreaID未設定(儲位編號:{String.Join(",", UnSetPickAreaID)})"));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, CheckPickAreaID.SelectMany(a=>a.WmsNos).ToList());
					if (isPickSchedule)
						continue;
					else
						break;
				}

				var CheckPickAreaIDWarehouse = from a in tmpBatchPickAllotRes.Data
																			 from b in a.PickingList
																			 where b.PickingSystem == 1
																			 select new { b.PickingSystem, b.PickAreaID, b.Details };
				if (CheckPickAreaIDWarehouse.Any(x => (x.PickAreaID?.Length ?? 0) > 3))
				{
          errMsg.Add(String.Format(strLMSMsg, $"揀貨單 PickingSystem=1， PickAreaID必須為倉庫編號(3碼)"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, CheckPickAreaIDWarehouse.SelectMany(a => a.Details.SelectMany(b => b.Orders.Select(c => c.WmsNo))).ToList());
          if (isPickSchedule)
						continue;
					else
						break;

				}

        var NotExistsWarehouseID = CheckWarehouseExists(ref f1980s, GroupA3.Group.First().DC_CODE, CheckPickAreaIDWarehouse.Select(x => x.PickAreaID).ToList());
        if (NotExistsWarehouseID.Any())
        {
          errMsg.Add(String.Format(strLMSMsg, $"PickingSystem=1， PickAreaID={String.Join(",", NotExistsWarehouseID)} 非系統倉庫編號"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, CheckPickAreaIDWarehouse.SelectMany(a => a.Details.SelectMany(b => b.Orders.Select(c => c.WmsNo))).ToList());
          if (isPickSchedule)
            continue;
          else
            break;
        }

        #endregion 檢查PickAreaID

        #region 檢查出貨明細項次(WmsSeq)
        var CheckWmsSeq = from a in tmpBatchPickAllotRes.Data
													from b in a.PickingList
													from c in b.Details
													from d in c.Orders
													group d by d.WmsSeq into grp
													where grp.Count() > 1
													select grp.Key;
				if (CheckWmsSeq.Any())
				{
					errMsg.Add(String.Format(strLMSMsg, $"LMS WmsSeq重複，重複出貨明細項次[{String.Join(",", CheckWmsSeq)}]"));
					InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule);
					if (isPickSchedule)
						continue;
					else
						break;

				}

        #endregion 檢查出貨明細項次(WmsSeq)

        #region 檢查PickType=4時不可為人工倉揀貨單
        ErrorBatchPickAllotWmsNo = tmpBatchPickAllotRes.Data.Where(x => x.PickingType == "4").SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsNo)))).ToList();
        if (GroupA3.Group.Any(x => x.DEVICE_TYPE == "0") && ErrorBatchPickAllotWmsNo.Any())
        {
          errMsg.Add(String.Format(strLMSMsg, $"含有人工倉揀貨單不可設為PickingType=4"));
          InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule, ErrorBatchPickAllotWmsNo);
          break;
        }
        #endregion 檢查PickType=4時不可為人工倉揀貨單

        #endregion no1279 檢查LMS回傳參數是否有錯


        //d.如果[A4].IsSuccessed= true 
        if (tmpBatchPickAllotRes.IsSuccessed)
				{
					var WmsSeqs = tmpBatchPickAllotRes.Data.SelectMany(a => a.PickingList.SelectMany(b => b.Details.SelectMany(c => c.Orders.Select(d => d.WmsSeq))));
					var A3PickApiSeq = GroupA3.Group.Select(a => a.PICKAPI_SEQ);
					//  (1)[A5]=取得[A4]回傳的所有WmsSeq是否有不存在於[A3].PICKAPI_SEQ
					var CheckPickApiSeqNotExist = WmsSeqs.Except(A3PickApiSeq);

					//  (2)若[A5]有資料，則errMsg.Add(API回傳的明細項次缺少[A5]),訊息內[A5]多筆請用逗點分隔組成字串，若<參數2>=false，離開所有迴圈，若<參數2>=true則contiune;
					if (CheckPickApiSeqNotExist.Any())
					{
						errMsg.Add(String.Format($"API回傳的明細項次缺少{String.Join(",", CheckPickApiSeqNotExist)}"));
						InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule);
						if (isPickSchedule)
							continue;
						else
							break;
					}

					//  (3)[A6]= 取得[A4]回傳的所有WmsSeq是否有重複WmsSeq
					//  (4)若[A6]有資料，則errMsg.Add(API回傳的明細項次重複[A6]),訊息內[A6]多筆請用逗點分隔組成字串，若<參數2>=false，離開所有迴圈，若<參數2>=true則contiune;
					var DuplicateWmsSeq = WmsSeqs.GroupBy(g => g).Where(x => x.Count() > 1).Select(x => x.Key);
					if (DuplicateWmsSeq.Any())
					{
						errMsg.Add(String.Format($"API回傳的明細項次重複{String.Join(",", DuplicateWmsSeq)}"));
						InserLMSPickFaildMsgToF0093(f0093Repo, f050306Repo, GroupA3.Group.ToList(), errMsg.Last(), isPickSchedule);
						if (isPickSchedule)
							continue;
						else
							break;
					}

					//  (5)若[A5]無資料 and [A6]無資料，則 
					//    [MM].Add([A4].Data); 
					if (!CheckPickApiSeqNotExist.Any() && !DuplicateWmsSeq.Any())
						apiResultDatas.Add(GroupA3.Key, tmpBatchPickAllotRes.Data);
				}
			}

			//4. If errMsg.Count > 0 and <參數2>=false
			//回傳new ExecuteResult(false, string.Join(Environment.NewLine, errMsg));
			if (errMsg.Any())
			{
				faildTransaction.Complete();
				if (!isPickSchedule)
				{
					result.IsSuccessed = false;
					result.Message = String.Join(Environment.NewLine, errMsg);
					return result;
				}
			}

			//5.[MM].Count>0，開始產生批次揀貨資料[回傳格式如1.2.2]
			//Foreach [M1] IN [MM]
			foreach (var item in apiResultDatas)
			{
				//(1)	建立新的CreatePick物件
				_createPick = new CreatePick();
				//(2)	[M2] =取得新的揀貨批次時間呼叫GetNewPickTime
				var PickTime = GetNewPickTime(isPickSchedule);
				//(3)	[M3]=取得[M1].PickingList.Details.Orders.WmsSeq所有的明細項次
				var WmsSeqs = item.Value.SelectMany(a0 => a0.PickingList.SelectMany(a => a.Details.SelectMany(b => b.Orders.Select(c => c.WmsSeq))));
				//(4)	[M4]= <參數1> WHERE PICKAPI_SEQ IN(M3)
				var PickApiSeqs = f050306s.Where(x => WmsSeqs.Contains(x.PICKAPI_SEQ)).ToList();
				//(5)	[M5] = [M4] Group by DC_CODE,GUP_CODE,CUST_CODE,WMS_NO, WH_TMPR_TYPE
				var M5 = PickApiSeqs.GroupBy(g => new { g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.WMS_NO, g.WH_TMPR_TYPE });
				//(6)	[M6]=產生出貨單號[GetNewOrdStackCodes("O",[M5].Count)
				_newWmsOrdNosStack = GetNewOrdStackCodes("O", M5.Count());
				//(7)	Foreach [M7] IN [M5]
				//#產生出貨單=>呼叫原CreateWmsOrder方法  
				foreach (var M5Item in M5)
					//#產生出貨單=>呼叫原CreateWmsOrder方法
					CreateWmsOrder(M5Item.Key.DC_CODE, M5Item.Key.GUP_CODE, M5Item.Key.CUST_CODE, M5Item.Key.WMS_NO, M5Item.Key.WH_TMPR_TYPE, PickTime, M5Item.ToList());

				//(1)	取得物流中心出貨指示設定資料F190105 WHERE DC_CODE = [A2].Key.DC_CODE
				if (!getDeliverySetttings.Any(x => x.DC_CODE == item.Key.DC_CODE))
					getDeliverySetttings.Add(f190105Repo.Find(x => x.DC_CODE == item.Key.DC_CODE));

				var dcShipSetting = getDeliverySetttings.FirstOrDefault(x => x.DC_CODE == item.Key.DC_CODE);
				//(8)	#產生揀貨單WithApi(DateTime.Today,[M2],[M1],[M4])
				var CreatePickOrderWithApiRes = CreatePickOrderWithApi(dcShipSetting, DateTime.Now.Date, PickTime, item.Value, PickApiSeqs);
				if (!CreatePickOrderWithApiRes.IsSuccessed)
				{
					errMsg.Add(CreatePickOrderWithApiRes.Message);
					continue;
				}

				//(9)	#產生揀貨批次=>呼叫原#CreatePickSummary
				var CreatePickSummaryRes = CreatePickSummary(
						 dcShipSetting, PickTime, item.Key.DC_CODE,
						 item.Key.GUP_CODE, item.Key.CUST_CODE, item.Key.ORD_TYPE, item.Key.SOURCE_TYPE,
						 item.Key.CUST_COST, item.Key.FAST_DEAL_TYPE);
				if (!CreatePickSummaryRes.IsSuccessed)
				{
					errMsg.Add(CreatePickSummaryRes.Message);
					continue;
				}
				//(10) #整批寫入資料庫=>參考原CreateOrderPick整批寫入資料庫
				PickDataBatchInsertDB(item.Key.MOVE_OUT_TARGET, f050306s, isPickSchedule);
        _wmsTransaction.Complete();
			}
			//6.	If errMsg.Count > 0 and <參數2>=true
			//回傳new ExecuteResult(false, string.Join(Environment.NewLine, errMsg));

			if (errMsg.Any() && isPickSchedule)
			{
				result.IsSuccessed = false;
				result.Message = string.Join(Environment.NewLine, errMsg);
				return result;
			}
			else
				return result;
		}
		#endregion 呼叫LMS API產生揀貨單

		/// <summary>
		/// 檢查LMS揀貨分配回傳資料中的PickAreaID是否有該倉別
		/// </summary>
		/// <param name="f1980s"></param>
		/// <param name="dcCode"></param>
		/// <param name="WarehouseIDs"></param>
		/// <returns></returns>
		private List<string> CheckWarehouseExists(ref List<F1980> f1980s, string dcCode, List<String> WarehouseIDs)
		{
			List<string> NotExistsWarehouseID = new List<string>();
			F1980Repository f1980Repo;
			if (f1980s?.Any() ?? true)
				f1980s = new List<F1980>();

			foreach (var WarehouseID in WarehouseIDs)
			{
				if (!f1980s.Any(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == WarehouseID))
				{
					f1980Repo = new F1980Repository(Schemas.CoreSchema);
					var tmpf1980s = f1980Repo.GetDatas(dcCode, new[] { WarehouseID }.ToList());
					if (tmpf1980s?.Any() ?? false)
					{
						f1980s.AddRange(tmpf1980s);
						continue;
					}
					else
						NotExistsWarehouseID.Add(WarehouseID);
				}
				else
					continue;
			}
			return NotExistsWarehouseID;
		}

		#region 使用LMS API建立揀貨單方法
		/// <summary>
		/// 使用LMS API建立揀貨單方法
		/// </summary>
		/// <param name="BatchDate">批次日期</param>
		/// <param name="BatchTime">批次時段</param>
		/// <param name="apiData">LMS API回傳資料</param>
		/// <param name="f050306s">配庫後揀貨資料清單</param>
		/// <returns></returns>
		private ExecuteResult CreatePickOrderWithApi(F190105 dcShipSetting, DateTime BatchDate, String BatchTime, List<BatchPickAllotPickingBatchData> batchPickAllotPickingBatchData, List<F050306> f050306s)
		{
			List<ExecuteResult> executeResults = new List<ExecuteResult>();
			var result = new ExecuteResult(true);
			var f050306HistoryRepo = new F050306_HISTORYRepository(Schemas.CoreSchema, _wmsTransaction);
			var addF050306HistoryList = new List<F050306_HISTORY>();
			// 是否為跨庫訂單
			var isCrossOrder = f050306s.Any(x => x.CUST_COST== "MoveOut");
			//Foreach G1[Data] IN <參數3>
			foreach (var batchPickItem in batchPickAllotPickingBatchData)
			{
				//[EE]=產生揀貨單號[GetNewOrdStackCodes("P",[G1].Count)
				_newPickOrdNosStack = GetNewOrdStackCodes("P", batchPickItem.PickingList.Count());

				// 如果為跨庫出貨訂單，若回傳LMS揀貨單類型=一單滿足 強制轉成一單分貨處理
				if (isCrossOrder && batchPickItem.PickingType == "1")
					batchPickItem.PickingType = "2";

				switch (batchPickItem.PickingType)
				{
					case "1":   //一單滿足=(單一揀貨)=WMS單一揀貨 (PICK_TYPE=0)自動倉自我滿足揀貨單(PICK_TYPE=6)
											//Foreach G2[PickingList] IN G1
						foreach (var G2 in batchPickItem.PickingList)
						{
							//(1)	[C1] = G1.PickingType convert to PickTypeEnums
							//(2)	[C2] =從<參數4> where PICKAPI_SEQ IN(G2所有的WmsSeq)
							var AllWmsSeq = G2.Details.SelectMany(a => a.Orders.Select(b => b.WmsSeq));
							var C1 = LMSPackingTypeToWmsPackingType(batchPickItem.PickingType, G2.PickingSystem != 0);
							var C2 = f050306s.Where(x => AllWmsSeq.Contains(x.PICKAPI_SEQ)).ToList();
							var f050306HistoryList = new List<F050306_HISTORY>();
							//Foreach [D1] IN [C2]
							foreach (var D1 in C2)
							{
								var GetPKValue = (from a in G2.Details
																	from b in a.Orders
																	where b.WmsSeq == D1.PICKAPI_SEQ
																	select new { a.PickAreaID, a.PickAreaName })
																	.FirstOrDefault();
								//b.	[D1].PK_AREA = [G2].Details.PickAreaID WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA = GetPKValue.PickAreaID;
								//c.	[D1].PK_AREA_NAME = [G2].Details. PickAreaName WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA_NAME = GetPKValue.PickAreaName;
								//a.	新增F050306_HISTORY (參考1.3.3 欄位對應)
								f050306HistoryList.Add(CreateF050306History(D1, batchPickItem, G2));
							}
							//(3)	[C3] = false (太單純，就不另外用變數)
							//(4)	[C4] = G2.PickingSystem=0 then SplitType.ShipOrder else SplitType.Warehouse 
							var C4 = G2.PickingSystem == 0 ? SplitType.ShipOrder : SplitType.Warehouse;
							//(5)	[C5] = G2.PickingSystem=0  
							//then<參數4>.First().WMS_ORD_NO
							//Else G2.PickAreaID
							var C5 = G2.PickingSystem == 0 ? C2.First().WMS_ORD_NO : G2.PickAreaID;
							//(6)	[C6] = G2.PickAreaID 
							var C6 = G2.PickAreaID;
							//(7)	[C7] = G2.PickAreaName
							var C7 = G2.PickAreaName;
							//(8)	[C8]= G2. ContainerType
							var C8 = G2.ContainerType;
							//(9)	[C9] =<參數4>.First().WMS_ORD_NO
							var C9 = C2.First().WMS_ORD_NO;
							//(10)	CreatePickOrder(<參數1>,<參數2>,[C1],[C2],[C3], [C4],[C5],[C6],[C7],[C8],[C9]);
							executeResults.Add(CreatePickOrder(BatchDate, BatchTime, C1, C2, false, C4, C5, C6, C7, C8, C9));
							if (!executeResults.Last().IsSuccessed)
								continue;
							f050306HistoryList.ForEach(x => { x.PICK_ORD_NO = executeResults.Last().No; });
							addF050306HistoryList.AddRange(f050306HistoryList);
						}
						break;

					case "2":   //一單分貨=(一階揀貨)=WMS人工一階彙整單(PICK_TYPE=1)
											//Foreach G2[PickingList] IN G1
						foreach (var G2 in batchPickItem.PickingList)
						{
							//(1)	[C1] = G1.PickingType convert to PickTypeEnums
							//(2)	[C2] =從<參數4> where PICKAPI_SEQ IN(G2所有的WmsSeq)
							var AllWmsSeq = G2.Details.SelectMany(a => a.Orders.Select(b => b.WmsSeq));
							var C1 = LMSPackingTypeToWmsPackingType(batchPickItem.PickingType, G2.PickingSystem != 0,isCrossOrder);
							var C2 = f050306s.Where(x => AllWmsSeq.Contains(x.PICKAPI_SEQ)).ToList();
							var f050306HistoryList = new List<F050306_HISTORY>();
							//Foreach [D1] IN [C2]
							foreach (var D1 in C2)
							{
								//a.	[D1].PK_AREA = [G2].Details.PickAreaID WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA = G2.Details.First(x => x.Orders.Select(a => a.WmsSeq).Contains(D1.PICKAPI_SEQ)).PickAreaID;
								//b.	[D1].PK_AREA_NAME = [G2].Details. PickAreaName WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA_NAME = (from a in G2.Details
																	 from b in a.Orders
																	 where b.WmsSeq == D1.PICKAPI_SEQ
																	 select a.PickAreaName)
																	 .FirstOrDefault();
								//c.	新增F050306_HISTORY (參考1.3.3 欄位對應)
								f050306HistoryList.Add(CreateF050306History(D1, batchPickItem, G2));
							}
							//(3)	如果是自動倉且非跨庫出貨不產生總揀單
							var C3 = G2.PickingSystem != 0 && !isCrossOrder ? false : true;
							//(4)	[C4]= SplitType.PkArea 
							var C4 = G2.PickingSystem != 0 ? SplitType.Warehouse : SplitType.PkArea;
							//(5)	[C5]= G2. PickAreaID
							var C5 = G2.PickAreaID;
							//(6)	[C6]= G2.PickAreaID
							var C6 = G2.PickAreaID;
							//(7)	[C7]= G2. PickAreaName
							var C7 = G2.PickAreaName;
							//(8)	[C8]= G2. ContainerType
							var C8 = G2.ContainerType;
							//(9)	如果是自動倉且非跨庫出貨不產生總揀單 集貨單號為出貨單
							var C9 = G2.PickingSystem != 0 && !isCrossOrder ? C2.First().WMS_ORD_NO : null;
							//(10)	CreatePickOrder(<參數1>,<參數2>,[C1],[C2],[C3], [C4],[C5],[C6] ,[C7],[C8],[C9]);

							executeResults.Add(CreatePickOrder(BatchDate, BatchTime, C1, C2, C3, C4, C5, C6, C7, C8, C9));
							if (!executeResults.Last().IsSuccessed)
								continue;
							f050306HistoryList.ForEach(x => { x.PICK_ORD_NO = executeResults.Last().No; });
							addF050306HistoryList.AddRange(f050306HistoryList);
						}
						break;

					case "3":   //多單集貨=(二階揀貨))=WMS 人工二階揀貨單(PICK_TYPE=3) 或自動倉揀貨單(PICK_TYPE=2)
											//Foreach G2[PickingList] IN G1
						foreach (var G2 in batchPickItem.PickingList)
						{
							//(1)	[C1] = G1.PickingType convert to PickTypeEnums
							var C1 = LMSPackingTypeToWmsPackingType(batchPickItem.PickingType, G2.PickingSystem != 0);
							//(2)	[C2] =從<參數4> where PICKAPI_SEQ IN(G2所有的WmsSeq)
							var AllWmsSeq = G2.Details.SelectMany(a => a.Orders.Select(b => b.WmsSeq));
							var C2 = f050306s.Where(x => AllWmsSeq.Contains(x.PICKAPI_SEQ)).ToList();
							var f050306HistoryList = new List<F050306_HISTORY>();
							//Foreach [D1] IN [C2]
							foreach (var D1 in C2)
							{
								//b.	[D1].PK_AREA = [G2].Details.PickAreaID WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA = G2.Details.First(x => x.Orders.Select(a => a.WmsSeq).Contains(D1.PICKAPI_SEQ)).PickAreaID;
								//c.	[D1].PK_AREA_NAME = [G2].Details. PickAreaName WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA_NAME = (from a in G2.Details
																	 from b in a.Orders
																	 where b.WmsSeq == D1.PICKAPI_SEQ
																	 select a.PickAreaName)
																	 .FirstOrDefault();
								//a.	新增F050306_HISTORY(參考1.3.3 欄位對應)
								f050306HistoryList.Add(CreateF050306History(D1, batchPickItem, G2));
							}
							//(3)	如果是自動倉且非跨庫出貨不產生總揀單
							var C3 = G2.PickingSystem != 0 && !isCrossOrder ? false : true;
							//(4)	[C4]= SplitType.PkArea 
							var C4 = G2.PickingSystem != 0 ? SplitType.Warehouse : SplitType.PkArea;
							//(5)	[C5]= G2. PickAreaID
							var C5 = G2.PickAreaID;
							//(6)	[C6]= G2.PickAreaID
							var C6 = G2.PickAreaID;
							//(7)	[C7]= G2. PickAreaName
							var C7 = G2.PickAreaName;
							//(8)	[C8]= G2. ContainerType
							var C8 = G2.ContainerType;
							//(9)	如果是自動倉且非跨庫出貨不產生總揀單 集貨單號為出貨單
							var C9 = G2.PickingSystem != 0 && !isCrossOrder ? C2.First().WMS_ORD_NO : null;
							//(10)	CreatePickOrder(<參數1>,<參數2>,[C1],[C2],[C3], [C4],[C5],[C6] ,[C7],[C8],[C9]);
							executeResults.Add(CreatePickOrder(BatchDate, BatchTime, C1, C2, C3, C4, C5, C6, C7, C8, C9));
							if (!executeResults.Last().IsSuccessed)
								continue;
							f050306HistoryList.ForEach(x => { x.PICK_ORD_NO = executeResults.Last().No; });
							addF050306HistoryList.AddRange(f050306HistoryList);
						}
						break;

					case "4":   //多單滿足 = WMS自動倉揀貨單(PICK_TYPE=2)
											// Foreach G2 IN G1
						foreach (var G2 in batchPickItem.PickingList)
						{
							//(1)	[C1] = G1.PickingType convert to PickTypeEnums
							var C1 = LMSPackingTypeToWmsPackingType(batchPickItem.PickingType, G2.PickingSystem != 0);
							//(2)	從<參數4> where PICKAPI_SEQ IN(G2所有的WmsSeq) 
							var AllWmsSeq = G2.Details.SelectMany(a => a.Orders.Select(b => b.WmsSeq));
							var C2 = f050306s.Where(x => AllWmsSeq.Contains(x.PICKAPI_SEQ)).ToList();
							var f050306HistoryList = new List<F050306_HISTORY>();
							//Foreach [D1] IN [C2]
							foreach (var D1 in C2)
							{
								//b.	[D1].PK_AREA = [G2].Details.PickAreaID WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA = G2.Details.First(x => x.Orders.Select(a => a.WmsSeq).Contains(D1.PICKAPI_SEQ)).PickAreaID;
								//c.	[D1].PK_AREA_NAME = [G2].Details. PickAreaName WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA_NAME = (from a in G2.Details
																	 from b in a.Orders
																	 where b.WmsSeq == D1.PICKAPI_SEQ
																	 select a.PickAreaName)
																	 .FirstOrDefault();
								//a.	新增F050306_HISTORY(參考1.3.3 欄位對應)
								f050306HistoryList.Add(CreateF050306History(D1, batchPickItem, G2));
							}
							//(3)	[C3]=false
							//(4)	[C4]= SplitType.Warehouse 
							var C4 = SplitType.Warehouse;
							//(5)	[C5]= G2. PickAreaID
							var C5 = G2.PickAreaID;
							//(6)	[C6]= G2.PickAreaID
							var C6 = G2.PickAreaID;
							//(7)	[C7]= G2. PickAreaName
							var C7 = G2.PickAreaName;
							//(8)	[C8]= G2. ContainerType
							var C8 = G2.ContainerType;
							//(9)	[C9] =<參數4>.First().WMS_ORD_NO
							var C9 = C2.First().WMS_ORD_NO;
							//(10)	CreatePickOrder(<參數1>,<參數2>,[C1],[C2],[C3], [C4],[C5],[C6] ,[C7],[C8],[C9]);
							executeResults.Add(CreatePickOrder(BatchDate, BatchTime, C1, C2, false, C4, C5, C6, C7, C8, C9));
							if (!executeResults.Last().IsSuccessed)
								continue;
							f050306HistoryList.ForEach(x => { x.PICK_ORD_NO = executeResults.Last().No; });
							addF050306HistoryList.AddRange(f050306HistoryList);
						}
						break;

					case "5": //特殊分貨11型 (特殊結構訂單: 一單一品一PCS) = WMS特殊結構訂單11(PICK_TYPE=4)
					case "6": //特殊分貨10型 (特殊結構訂單: 一單一品多PCS) = WMS特殊結構訂單10(PICK_TYPE=7)
					case "7": //特殊分貨12型 (特殊結構訂單: 一單兩品一PCS) = WMS特殊結構訂單12(PICK_TYPE=8) 
										//A. [D1] = null
						var D1PickOrder = String.Empty;
						//B. Foreach G2 IN G1
						foreach (var G2 in batchPickItem.PickingList)
						{
							//(1)	[C1] = G1.PickingType convert to PickTypeEnums
							var C1 = LMSPackingTypeToWmsPackingType(batchPickItem.PickingType, G2.PickingSystem != 0);
							//(2)	[C2] =從<參數4> where PICKAPI_SEQ IN(G2所有的WmsSeq) 
							var AllWmsSeq = G2.Details.SelectMany(a => a.Orders.Select(b => b.WmsSeq));
							var C2 = f050306s.Where(x => AllWmsSeq.Contains(x.PICKAPI_SEQ)).ToList();
							var f050306HistoryList = new List<F050306_HISTORY>();
							//Foreach [D1] IN [C2]
							foreach (var D1 in C2)
							{
								//b.	[D1].PK_AREA = [G2].Details.PickAreaID WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA = G2.Details.First(x => x.Orders.Select(a => a.WmsSeq).Contains(D1.PICKAPI_SEQ)).PickAreaID;
								//c.	[D1].PK_AREA_NAME = [G2].Details. PickAreaName WHERE [G2].Details.Orders.WmsSeq=[D1].PICKAPI_SEQ
								D1.PK_AREA_NAME = (from a in G2.Details
																	 from b in a.Orders
																	 where b.WmsSeq == D1.PICKAPI_SEQ
																	 select a.PickAreaName)
																	 .FirstOrDefault();
								//a.	新增F050306_HISTORY(參考1.3.3 欄位對應)
								f050306HistoryList.Add(CreateF050306History(D1, batchPickItem, G2));
							}
							//(3)	[C3]=true
							//(4)	[C4]= G2.PickingSystem=0 then SplitType.PkArea else SplitType.Warehouse 
							var C4 = G2.PickingSystem == 0 ? SplitType.PkArea : SplitType.Warehouse;
							//(5)	[C5]= G2. PickAreaID
							var C5 = G2.PickAreaID;
							//(6)	[C6]= G2.PickAreaID 
							var C6 = G2.PickAreaID;
							//(7)	[C7]= G2.PickAreaName 
							var C7 = G2.PickAreaName;
							//(8)	[C8]= G2. ContainerType
							var C8 = G2.ContainerType;
							//(9)	[C9] = [D1]
							//(10)	CreatePickOrder(<參數1>,<參數2>,[C1],[C2],[C3], [C4],[C5],[C6] ,[C7],[C8],[C9]);
							executeResults.Add(CreatePickOrder(BatchDate, BatchTime, C1, C2, true, C4, C5, C6, C7, C8, D1PickOrder));
							if (!executeResults.Last().IsSuccessed)
								continue;

							//(11)	IF [D1]=null 
							//[D1]=從(11)產生的揀貨單號
							if (String.IsNullOrWhiteSpace(D1PickOrder))
								D1PickOrder = executeResults.Last().No;

							//   B. 如果G1.COUNT >1，更新揀貨單的下一步改為集貨場，條件F051201.MERGE_NO=[D1]
							//if(batchPickItem.PickingList.Count()>1)

							f050306HistoryList.ForEach(x => { x.PICK_ORD_NO = executeResults.Last().No; });
							addF050306HistoryList.AddRange(f050306HistoryList);
						}
						//B. 如果G1.COUNT >1，更新揀貨單的下一步改為集貨場，條件F051201.MERGE_NO=[D1]
						var EditNextSetp = _createPick.F051201s.Where(x => x.MERGE_NO == D1PickOrder);
						foreach (var item in EditNextSetp)
							item.NEXT_STEP = "2";
						break;
				}
			}
			result.IsSuccessed = executeResults.All(x => x.IsSuccessed);
			result.Message = String.Join(Environment.NewLine, executeResults.Where(x => !String.IsNullOrEmpty(x.Message)));
			f050306HistoryRepo.BulkInsert(addF050306HistoryList, "ID");
			// 下發自動倉任務
			CreateAutoPickTasks(dcShipSetting, BatchDate, BatchTime);

			return result;
		}

		/// <summary>
		/// 將LMS API的PackingType轉換成WmsPackingType
		/// </summary>
		/// <param name="LMSPackingTypeCode">LMSPackingType編號</param>
		/// <param name="isAutoWareHouse">是否為自動倉</param>
		/// <returns></returns>
		public PickTypeEnums LMSPackingTypeToWmsPackingType(String LMSPackingTypeCode, Boolean isAutoWareHouse,Boolean isCrossOrder=false)
		{
			if (LMSPackingTypeCode == "1" && !isAutoWareHouse)
				return PickTypeEnums.FullArtificialSinglePick;
			else if (LMSPackingTypeCode == "1" && isAutoWareHouse)
				return PickTypeEnums.AutoSelfPick;
			else if (LMSPackingTypeCode == "2" && !isAutoWareHouse)
				return PickTypeEnums.FullArtificialSelfBatchPick;
			else if (LMSPackingTypeCode == "2" && isAutoWareHouse && !isCrossOrder)
				return PickTypeEnums.AutoSelfPick;
			else if (LMSPackingTypeCode == "2" && isAutoWareHouse && isCrossOrder)
				return PickTypeEnums.AutoPick;
			else if (LMSPackingTypeCode == "3" && !isAutoWareHouse)
				return PickTypeEnums.ArtificialBatchPick;
			else if (LMSPackingTypeCode == "3" && isAutoWareHouse)
				return PickTypeEnums.AutoPick;
			else if (LMSPackingTypeCode == "4")
				return PickTypeEnums.AutoPick;
			else if (LMSPackingTypeCode == "5")
				return PickTypeEnums.SpecialOrderPick;
			else if (LMSPackingTypeCode == "6")
				return PickTypeEnums.SpecialPick1;
			else if (LMSPackingTypeCode == "7")
				return PickTypeEnums.SpecialPick2;
			else
				throw new Exception("無法辨識的LMS PackingType");
		}

		private F050306_HISTORY CreateF050306History(F050306 f050306, BatchPickAllotPickingBatchData batchPickItem, BatchPickAllotPickingListData PickingListItem)
		{
			var GetPickingListItemDetails = from a in PickingListItem.Details
																			from b in a.Orders
																			where b.WmsNo == f050306.WMS_NO && b.WmsSeq == f050306.PICKAPI_SEQ
																			select new { a.PickAreaID, a.PickAreaName, b.WmsNo, b.WmsSeq };
			var tmpGetPickingListItemDetails = GetPickingListItemDetails.First();
			return new F050306_HISTORY()
			{
				DC_CODE = f050306.DC_CODE,
				GUP_CODE = f050306.GUP_CODE,
				CUST_CODE = f050306.CUST_CODE,
				SOURCE = f050306.SOURCE,
				WMS_NO = f050306.WMS_NO,
				WMS_SEQ = f050306.WMS_SEQ,
				WAREHOUSE_ID = f050306.WAREHOUSE_ID,
				PICK_FLOOR = f050306.PICK_FLOOR,
				DEVICE_TYPE = f050306.DEVICE_TYPE,
				WH_TMPR_TYPE = f050306.WH_TMPR_TYPE,
				PICK_LOC = f050306.PICK_LOC,
				PK_AREA = f050306.PK_AREA,
				ITEM_CODE = f050306.ITEM_CODE,
				ORD_TYPE = f050306.ORD_TYPE,
				SOURCE_TYPE = f050306.SOURCE_TYPE,
				VALID_DATE = f050306.VALID_DATE,
				ENTER_DATE = f050306.ENTER_DATE,
				MAKE_NO = f050306.MAKE_NO,
				VNR_CODE = f050306.VNR_CODE,
				SERIAL_NO = f050306.SERIAL_NO,
				PALLET_CTRL_NO = f050306.PALLET_CTRL_NO,
				BOX_CTRL_NO = f050306.BOX_CTRL_NO,
				B_PICK_QTY = f050306.B_PICK_QTY,
				CUST_COST = f050306.CUST_COST,
				FAST_DEAL_TYPE = f050306.FAST_DEAL_TYPE,
				WMS_ORD_NO = f050306.WMS_ORD_NO,
				WMS_ORD_SEQ = f050306.WMS_ORD_SEQ,
				LACK_ID = f050306.LACK_ID,
				MOVE_OUT_TARGET = f050306.MOVE_OUT_TARGET,
				PACKING_TYPE = f050306.PACKING_TYPE,
				CONTAINER_TYPE = f050306.CONTAINER_TYPE,
				PK_AREA_NAME = f050306.PK_AREA_NAME,
				RTN_VNR_CODE = f050306.RTN_VNR_CODE,
				PICKAPI_SEQ = f050306.PICKAPI_SEQ,
				LMS_PICKING_BATCH_NO = batchPickItem.PickingBatchNo,
				LMS_PICKING_TYPE = batchPickItem.PickingType,
				LMS_CREATE_TIME = batchPickItem.CreateTime,
				LMS_PICKING_NO = PickingListItem.PickingNo,
				LMS_PICKING_SYSTEM = PickingListItem.PickingSystem,
				LMS_PICK_AREA_ID = PickingListItem.PickAreaID,
				LMS_PICK_AREA_NAME = PickingListItem.PickAreaName,
				LMS_CONTINER_TYPE = PickingListItem.ContainerType,
				LMS_NEXT_STEP_CODE = PickingListItem.NextStepCode,
				LMS_DTL_PICK_AREA_ID = tmpGetPickingListItemDetails.PickAreaID,
				LMS_DTL_PICK_AREA_NAME = tmpGetPickingListItemDetails.PickAreaName,
				LMS_WMS_NO = tmpGetPickingListItemDetails.WmsNo,
				LMS_WMS_SEQ = tmpGetPickingListItemDetails.WmsSeq,
				CRT_DATE = f050306.CRT_DATE,
				CRT_STAFF = f050306.CRT_STAFF,
				CRT_NAME = f050306.CRT_NAME
			};
		}
		#endregion 使用LMS API建立揀貨單方法

		#region 產生訂單揀貨資料
		/// <summary>
		/// 產生訂單揀貨單
		/// </summary>
		/// <param name="f050306s">配庫後揀貨資料</param>
		/// <param name="isPickSchedule">是否揀貨排程執行</param>
		/// <returns></returns>
		private ExecuteResult CreateOrderPick(List<F050306> f050306s, bool isPickSchedule,bool isUserDirectPriorityCode=false)
		{
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060201Repo = new F060201Repository(Schemas.CoreSchema, _wmsTransaction);

			var groupDcs = f050306s.GroupBy(x => x.DC_CODE).ToList();
			var errMsg = new List<string>();
			foreach (var groupDc in groupDcs)
			{
				_wmsLogHelper.AddRecord("取得物流中心出貨指示設定資料");
				var dcShipSetting = Get190105(groupDc.Key);
				if (dcShipSetting == null)
				{
					errMsg.Add(string.Format("未設定物流中心{0}出貨指示設定資料", groupDc.Key));
					continue;
				}
				var groupBatchPickers = groupDc.GroupBy(x => new
				{
					x.DC_CODE,
					x.GUP_CODE,
					x.CUST_CODE,
					x.ORD_TYPE,
					x.SOURCE_TYPE,
					x.CUST_COST,
					x.FAST_DEAL_TYPE,
					x.MOVE_OUT_TARGET,
					x.RTN_VNR_CODE,
          x.ORDER_CRT_DATE,
          x.ORDER_PROC_TYPE
        }).OrderBy(x => x.Key.ORDER_CRT_DATE).ThenByDescending(x => x.Key.FAST_DEAL_TYPE).ToList();

        foreach (var groupPicker in groupBatchPickers)
				{
          try
          {
            _wmsLogHelper.AddRecord(string.Format("產生揀貨批次開始[{0}]", JsonConvert.SerializeObject(groupPicker.Key)));

            _createPick = new CreatePick();
            // 暫揀貨類型配庫後揀貨資料
            _tempPickTypeList = new Dictionary<PickTypeEnums, List<F050306>>();
            _wmsLogHelper.AddRecord("取得批次時間");
            // 取得新的批次時段
            var pickTime = GetNewPickTime(isPickSchedule);
            _wmsLogHelper.AddRecord("分配揀貨單");
            // 分配揀貨單
            var allotPickRes = AllotPicks(dcShipSetting, pickTime, groupPicker.ToList());
            if (!allotPickRes.IsSuccessed)
            {
              errMsg.Add(allotPickRes.Message);
              continue;
            }
            _wmsLogHelper.AddRecord("建立揀貨單");
            // 建立揀貨單
            var delvDate = DateTime.Today;
            var createPickRes = CreatePickOrders(dcShipSetting, delvDate, pickTime, isUserDirectPriorityCode);
            if (!createPickRes.IsSuccessed)
            {
              errMsg.Add(createPickRes.Message);
              continue;
            }
            _wmsLogHelper.AddRecord("建立揀貨批次");
            // 建立揀貨批次
            var createBatchRes = CreatePickSummary(dcShipSetting, pickTime,
              groupPicker.Key.DC_CODE, groupPicker.Key.GUP_CODE, groupPicker.Key.CUST_CODE,
              groupPicker.Key.ORD_TYPE, groupPicker.Key.SOURCE_TYPE, groupPicker.Key.CUST_COST,
              groupPicker.Key.FAST_DEAL_TYPE, groupPicker.Key.ORDER_CRT_DATE, groupPicker.Key.ORDER_PROC_TYPE, isUserDirectPriorityCode);
            if (!createBatchRes.IsSuccessed)
            {
              errMsg.Add(createBatchRes.Message);
              continue;
            }

            PickDataBatchInsertDB(groupPicker.Key.MOVE_OUT_TARGET, f050306s, isPickSchedule);

						// 只有自動配庫才能 by 揀貨批次commit
						if (isPickSchedule)
              _wmsTransaction.Complete();
          }
          catch (Exception e)
          {
            if (isPickSchedule)
            {
              errMsg.Add(e.ToString());
              continue;
            }
            else
            {
              throw e;
            }
          }
          finally
          {
						// 只有自動配庫才能reset by 揀貨批次commit
						if (isPickSchedule)
							_wmsTransaction = new WmsTransaction();
          }
				}
			}
			if (errMsg.Any())
				return new ExecuteResult(false, string.Join(Environment.NewLine, errMsg));
			else
				return new ExecuteResult(true);
		}

		private void PickDataBatchInsertDB(String MOVE_OUT_TARGET, List<F050306> f050306s, bool isPickSchedule)
		{
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
      var f050306HistoryRepo = new F050306_HISTORYRepository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060201Repo = new F060201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f0535Repo = new F0535Repository(Schemas.CoreSchema, _wmsTransaction);

      _wmsLogHelper.AddRecord("整批寫入資料庫");
			// 整批寫入資料庫
			if (_createPick.F05030101s != null && _createPick.F05030101s.Any())
				f05030101Repo.BulkInsert(_createPick.F05030101s);
			if (_createPick.F05030202s != null && _createPick.F05030202s.Any())
				f05030202Repo.BulkInsert(_createPick.F05030202s);
			if (_createPick.F050801s != null && _createPick.F050801s.Any())
			{
				_createPick.F050801s.ForEach(f => f.MOVE_OUT_TARGET = MOVE_OUT_TARGET);
				f050801Repo.BulkInsert(_createPick.F050801s);
			}
      if (_createPick.F050306_HISTORYs != null && _createPick.F050306_HISTORYs.Any())
        f050306HistoryRepo.BulkInsert(_createPick.F050306_HISTORYs, "ID");
      if (_createPick.F050802s != null && _createPick.F050802s.Any())
				f050802Repo.BulkInsert(_createPick.F050802s);
			if (_createPick.F051201s != null && _createPick.F051201s.Any())
				f051201Repo.BulkInsert(_createPick.F051201s);
			if (_createPick.F051202s != null && _createPick.F051202s.Any())
				f051202Repo.BulkInsert(_createPick.F051202s);
			if (_createPick.F051203s != null && _createPick.F051203s.Any())
				f051203Repo.BulkInsert(_createPick.F051203s);
			if (_createPick.F1511s != null && _createPick.F1511s.Any())
				f1511Repo.BulkInsert(_createPick.F1511s);
			if (_createPick.F0513 != null)
			{
				_createPick.F0513.MOVE_OUT_TARGET = MOVE_OUT_TARGET;
				f0513Repo.Add(_createPick.F0513);
			}
			if (_createPick.F051301s != null && _createPick.F051301s.Any())
				f051301Repo.BulkInsert(_createPick.F051301s);
			if (_createPick.F060201s != null && _createPick.F060201s.Any())
				f060201Repo.BulkInsert(_createPick.F060201s);
      if (isPickSchedule && _createPick.F050306_HISTORYs.Any())
        BatchDeleteF050306(f050306s.Where(o => _createPick.F050306_HISTORYs.Select(z => z.WMS_NO).Contains(o.WMS_NO)).Select(x => x.ID).ToList());
      if (_createPick.F0535s != null && _createPick.F0535s.Any())
        f0535Repo.BulkInsert(_createPick.F0535s);
      _wmsLogHelper.AddRecord("產生揀貨批次結束");
		}

		/// <summary>
		/// 取得物流中心出貨指示設定資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <returns></returns>
		private F190105 Get190105(string dcCode)
		{
			var f190105Repo = new F190105Repository(Schemas.CoreSchema);
			if (_f190105s == null)
				_f190105s = new List<F190105>();
			var f190105 = _f190105s.FirstOrDefault(x => x.DC_CODE == dcCode);
			if (f190105 == null)
				f190105 = f190105Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode).FirstOrDefault();
			if (f190105 != null)
				_f190105s.Add(f190105);

			return f190105;
		}

		/// <summary>
		/// 取得新的批次時段
		/// *排程固定是5秒的倍數增加批次
		/// *手動固定是3秒的倍數增加批次，若秒數剛好是5秒的倍數則在加3秒
		/// </summary>
		/// <param name="isPickSchedule">是否揀貨排程執行</param>
		/// <returns></returns>
		private string GetNewPickTime(bool isPickSchedule)
		{
			var time = DateTime.Now;
			if (_prePickTime.HasValue)
				time = _prePickTime.Value;

			var addSecond = 0;
			if (isPickSchedule)
				addSecond = (5 - (time.Second % 5));
			else
			{
				addSecond = (3 - (time.Second % 3));
				if ((time.Second + addSecond) % 5 == 0)
					addSecond += 3;
			}
			time = time.AddSeconds(addSecond);
			_prePickTime = time;
			return time.ToString("HH:mm:ss");
		}

		#region 分配揀貨單
		/// <summary>
		/// 分配揀貨單類型
		/// </summary>
		/// <param name="dcShipSetting"></param>
		/// <param name="pickTime"></param>
		/// <param name="f050306s"></param>
		/// <returns></returns>
		private ExecuteResult AllotPicks(F190105 dcShipSetting, string pickTime, List<F050306> f050306s)
		{
			_newWmsOrdNosStack = GetNewOrdStackCodes("O", f050306s.GroupBy(x => new { x.WMS_NO, x.WH_TMPR_TYPE }).Count());
			var wmsNos = new List<string>();
			// 開啟特殊結構訂單
			if (dcShipSetting.OPEN_SPECIAL_ORDER == "1")
			{
				wmsNos = AddToPickType4(pickTime, f050306s);
				// 加入訂單符合特殊結構規則揀貨單清單
				f050306s.RemoveAll(x => wmsNos.Contains(x.WMS_NO));
			}

			// 加入訂單符合人工倉單一揀貨規則揀貨單清單
			wmsNos = AddToPickType0(dcShipSetting, pickTime, f050306s);
			f050306s.RemoveAll(x => wmsNos.Contains(x.WMS_NO));

			// 加入訂單符合人工倉自我滿足規則批量揀貨單清單
			wmsNos = AddToPickType1(pickTime, f050306s);
			f050306s.RemoveAll(x => wmsNos.Contains(x.WMS_NO));

			// 加入訂單符合自動倉自我滿足規則揀貨單清單
			wmsNos = AddToPickType6(pickTime, f050306s);
			f050306s.RemoveAll(x => wmsNos.Contains(x.WMS_NO));


			// 加入訂單符合非同條件規則批量揀貨單(非同條件目前是以倉來看，訂單有跨不同倉就是非同條件)
			wmsNos = AddToPickType2nd3(pickTime, f050306s);
			f050306s.RemoveAll(x => wmsNos.Contains(x.WMS_NO));

			return new ExecuteResult(true);
		}

    /// <summary>
    /// 取得指定限制單一揀貨的貨主清單
    /// </summary>
    /// <param name="dcShipSetting"></param>
    /// <returns></returns>
    private List<string> GetLimitSinglePickCustList(F190105 dcShipSetting)
    {
      if (dcShipSetting != null && !string.IsNullOrWhiteSpace(dcShipSetting.LIMIT_SINGLEPICK_CUST_LIST))
        return dcShipSetting.LIMIT_SINGLEPICK_CUST_LIST.Split(',').ToList();
      else
        return new List<string>();
    }

    /// <summary>
    /// 訂單符合單一揀貨規則揀貨清單
    /// </summary>
    /// <param name="dcShipSetting"></param>
    /// <param name="pickTime"></param>
    /// <param name="f050306s"></param>
    /// <returns></returns>
    private List<string> AddToPickType0(F190105 dcShipSetting, string pickTime, List<F050306> f050306s)
		{
      var LimitSinglePickCustList = GetLimitSinglePickCustList(dcShipSetting);

      var orderLIst = new List<string>();
      // 人工倉單一揀貨(訂單所有揀貨明細都在人工倉，且品項數超過單一揀貨品項數或明細筆數超過單一揀貨明細筆數或來源單據為廠退出貨)
      // 如果出貨單揀貨明細都在人工倉揀且出貨單商品處理類別=含安裝型商品[F050306.ORDER_PROC_TYPE=1]，有要算是單一揀貨單
      var orders = (from o in f050306s
                    group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.WMS_NO } into g
                    let ItemCnt = g.Select(x => x.ITEM_CODE).Distinct().Count()
                    let TotalRecord = g.Count()
                    let isAtfl = g.All(x => x.DEVICE_TYPE == "0")
                    let isRtnOrder = g.Any(x => x.SOURCE_TYPE == "13")
                    let isMoveOut = g.Any(x => x.CUST_COST == "MoveOut")
                    let OrderProcType1 = g.Any(x => x.ORDER_PROC_TYPE == "1")
                    let isSinglePickCust = g.All(x => LimitSinglePickCustList.Contains(x.CUST_CODE) && x.DEVICE_TYPE == "0")
                    where isAtfl && !isMoveOut
                      && (
                        ItemCnt > dcShipSetting.ORDER_MAX_ITEMCNT
                        || TotalRecord > dcShipSetting.ORDER_MAX_RECORD
                        || isRtnOrder
                        || OrderProcType1
                        || isSinglePickCust
                        )
                    select g).ToList();
			foreach (var order in orders)
			{
				// 依儲位溫層拆O單
				var orderGroupByTmprs = order.GroupBy(x => x.WH_TMPR_TYPE).ToList();
				foreach (var orderGroupByTmpr in orderGroupByTmprs)
					// 產生出貨單、出貨明細、訂單與出貨單關聯檔、配庫後訂單明細與出貨明細對照表
					CreateWmsOrder(order.Key.DC_CODE, order.Key.GUP_CODE, order.Key.CUST_CODE, order.Key.WMS_NO, orderGroupByTmpr.Key, pickTime, orderGroupByTmpr.ToList());

				if (_tempPickTypeList.ContainsKey(PickTypeEnums.FullArtificialSinglePick))
					_tempPickTypeList[PickTypeEnums.FullArtificialSinglePick].AddRange(order.ToList());
				else
					_tempPickTypeList.Add(PickTypeEnums.FullArtificialSinglePick, order.ToList());
			}
			orderLIst.AddRange(orders.Select(x => x.Key.WMS_NO).ToList());

			return orderLIst;

		}

		/// <summary>
		/// 訂單符合人工倉自我滿足批量揀貨單
		/// </summary>
		/// <param name="pickTime"></param>
		/// <param name="f050306s"></param>
		/// <returns></returns>
		private List<string> AddToPickType1(string pickTime, List<F050306> f050306s)
		{
			var orderLIst = new List<string>();
			// 人工倉自我滿足單批量揀貨單(訂單所有揀貨明細都在人工倉，且都在同一PK區揀貨)
			var orders = (from o in f050306s
										group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.WMS_NO } into g
										let pkCnt = g.Select(x => x.PK_AREA).Distinct().Count()
										let isAtfl = g.All(x => x.DEVICE_TYPE == "0")
										where isAtfl && pkCnt == 1
										select g).ToList();
			foreach (var order in orders)
			{
				// 依儲位溫層拆O單
				var orderGroupByTmprs = order.GroupBy(x => x.WH_TMPR_TYPE).ToList();
				foreach (var orderGroupByTmpr in orderGroupByTmprs)
					// 產生出貨單、出貨明細、訂單與出貨單關聯檔、配庫後訂單明細與出貨明細對照表
					CreateWmsOrder(order.Key.DC_CODE, order.Key.GUP_CODE, order.Key.CUST_CODE, order.Key.WMS_NO, orderGroupByTmpr.Key, pickTime, orderGroupByTmpr.ToList());

				if (_tempPickTypeList.ContainsKey(PickTypeEnums.FullArtificialSelfBatchPick))
					_tempPickTypeList[PickTypeEnums.FullArtificialSelfBatchPick].AddRange(order.ToList());
				else
					_tempPickTypeList.Add(PickTypeEnums.FullArtificialSelfBatchPick, order.ToList());
			}
			orderLIst.AddRange(orders.Select(x => x.Key.WMS_NO).ToList());

			return orderLIst;

		}

		/// <summary>
		/// 訂單符合非同條件規則批量揀貨單
		/// </summary>
		/// <param name="pickTime"></param>
		/// <param name="f050306s"></param>
		/// <returns></returns>
		private List<string> AddToPickType2nd3(string pickTime, List<F050306> f050306s)
		{
			var orderLIst = new List<string>();
			// 非同條件規則批量揀貨單(訂單在人工區多PK區或自動倉多倉或人工+自動倉揀貨)
			var orders = (from o in f050306s
										group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.WMS_NO } into g
										let atflPkCnt = g.Where(x => x.DEVICE_TYPE == "0").Select(x => x.PK_AREA).Distinct().Count()
										let autoWhCnt = g.Where(x => x.DEVICE_TYPE != "0").Select(x => x.WAREHOUSE_ID).Distinct().Count()
										where atflPkCnt + autoWhCnt > 1
										select g).ToList();
			foreach (var order in orders)
			{
				// 依儲位溫層拆O單
				var orderGroupByTmprs = order.GroupBy(x => x.WH_TMPR_TYPE).ToList();
				foreach (var orderGroupByTmpr in orderGroupByTmprs)
					// 產生出貨單、出貨明細、訂單與出貨單關聯檔、配庫後訂單明細與出貨明細對照表
					CreateWmsOrder(order.Key.DC_CODE, order.Key.GUP_CODE, order.Key.CUST_CODE, order.Key.WMS_NO, orderGroupByTmpr.Key, pickTime, orderGroupByTmpr.ToList());

				var groupDeviceTypes = order.GroupBy(x => x.DEVICE_TYPE).ToList();
				foreach (var groupDeviceType in groupDeviceTypes)
				{
					if (groupDeviceType.Key == "0")
					{
						if (_tempPickTypeList.ContainsKey(PickTypeEnums.ArtificialBatchPick))
							_tempPickTypeList[PickTypeEnums.ArtificialBatchPick].AddRange(groupDeviceType.ToList());
						else
							_tempPickTypeList.Add(PickTypeEnums.ArtificialBatchPick, groupDeviceType.ToList());
					}
					else
					{
						if (_tempPickTypeList.ContainsKey(PickTypeEnums.AutoPick))
							_tempPickTypeList[PickTypeEnums.AutoPick].AddRange(groupDeviceType.ToList());
						else
							_tempPickTypeList.Add(PickTypeEnums.AutoPick, groupDeviceType.ToList());
					}
				}
			}
			orderLIst.AddRange(orders.Select(x => x.Key.WMS_NO).ToList());

			return orderLIst;

		}

		/// <summary>
		/// 訂單符合特殊結構規則揀貨單清單 
		/// </summary>
		/// <param name="pickTime"></param>
		/// <param name="ordType"></param>
		/// <param name="sourceType"></param>
		/// <param name="custCost"></param>
		/// <param name="fastDealType"></param>
		/// <param name="f050306s"></param>
		/// <returns></returns>
		private List<string> AddToPickType4(string pickTime, List<F050306> f050306s)
		{
			var orderLIst = new List<string>();
			// 特殊結構揀貨單 (1單1品1PCS) (1單1品多PCS 且必須在同一倉庫揀，如果分倉沒法一起到包裝站)
			var orders = (from o in f050306s
										group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.WMS_NO } into g
										let warehouseCnt = g.Select(x => x.WAREHOUSE_ID).Distinct().Count()
										let ItemCnt = g.Select(x => x.ITEM_CODE).Distinct().Count()
										let TotalQty = g.Sum(x => x.B_PICK_QTY)
										where ItemCnt == 1 && (TotalQty == 1 || (TotalQty > 1 && warehouseCnt == 1))
										select g).ToList();


			foreach (var order in orders)
			{
				// 依儲位溫層拆O單
				var orderGroupByTmprs = order.GroupBy(x => x.WH_TMPR_TYPE).ToList();
				foreach (var orderGroupByTmpr in orderGroupByTmprs)
					// 產生出貨單、出貨明細、訂單與出貨單關聯檔、配庫後訂單明細與出貨明細對照表
					CreateWmsOrder(order.Key.DC_CODE, order.Key.GUP_CODE, order.Key.CUST_CODE, order.Key.WMS_NO, orderGroupByTmpr.Key, pickTime, orderGroupByTmpr.ToList());

				if (_tempPickTypeList.ContainsKey(PickTypeEnums.SpecialOrderPick))
					_tempPickTypeList[PickTypeEnums.SpecialOrderPick].AddRange(order.ToList());
				else
					_tempPickTypeList.Add(PickTypeEnums.SpecialOrderPick, order.ToList());
			}
			orderLIst.AddRange(orders.Select(x => x.Key.WMS_NO).ToList());

			return orderLIst;
		}

		/// <summary>
		/// 訂單符合自動倉自我滿足揀貨單
		/// </summary>
		/// <param name="pickTime"></param>
		/// <param name="f050306s"></param>
		/// <returns></returns>
		private List<string> AddToPickType6(string pickTime, List<F050306> f050306s)
		{
			var orderLIst = new List<string>();
			// 自動倉自我滿足單批量揀貨單(訂單所有揀貨明細都在自動倉，且都在同一倉庫揀貨)
			var orders = (from o in f050306s
										group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.WMS_NO } into g
										let warehouseCnt = g.Select(x => x.WAREHOUSE_ID).Distinct().Count()
										let isAuto = g.All(x => x.DEVICE_TYPE != "0")
										where isAuto && warehouseCnt == 1
										select g).ToList();
			foreach (var order in orders)
			{
				// 依儲位溫層拆O單
				var orderGroupByTmprs = order.GroupBy(x => x.WH_TMPR_TYPE).ToList();
				foreach (var orderGroupByTmpr in orderGroupByTmprs)
					// 產生出貨單、出貨明細、訂單與出貨單關聯檔、配庫後訂單明細與出貨明細對照表
					CreateWmsOrder(order.Key.DC_CODE, order.Key.GUP_CODE, order.Key.CUST_CODE, order.Key.WMS_NO, orderGroupByTmpr.Key, pickTime, orderGroupByTmpr.ToList());

				if (_tempPickTypeList.ContainsKey(PickTypeEnums.AutoSelfPick))
					_tempPickTypeList[PickTypeEnums.AutoSelfPick].AddRange(order.ToList());
				else
					_tempPickTypeList.Add(PickTypeEnums.AutoSelfPick, order.ToList());
			}
			orderLIst.AddRange(orders.Select(x => x.Key.WMS_NO).ToList());

			return orderLIst;

		}

		/// <summary>
		/// 建立出貨單、出貨明細、訂單與出貨單關聯檔、配庫後訂單明細與出貨明細對照表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordNo"></param>
		/// <param name="tmprType"></param>
		/// <param name="pickTime"></param>
		/// <param name="f050306s"></param>
		/// <returns></returns>
		private void CreateWmsOrder(string dcCode, string gupCode, string custCode, string ordNo, string tmprType, string pickTime, List<F050306> f050306s)
		{
			if (_createPick.F050801s == null)
				_createPick.F050801s = new List<F050801>();
			if (_createPick.F050802s == null)
				_createPick.F050802s = new List<F050802>();
			if (_createPick.F05030101s == null)
				_createPick.F05030101s = new List<F05030101>();
			if (_createPick.F05030202s == null)
				_createPick.F05030202s = new List<F05030202>();

			var f050301 = GetF050301(dcCode, gupCode, custCode, ordNo);
			var f1903s = CommonService.GetProductList(gupCode, custCode, f050306s.Select(x => x.ITEM_CODE).Distinct().ToList());
			var isFirstWmsOrder = !_createPick.F05030101s.Any(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo);

			#region 建立出貨單[F050801]

			var f050801 = new F050801
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				WMS_ORD_NO = _newWmsOrdNosStack.Pop(),
				DELV_DATE = DateTime.Today,
				ORD_TYPE = f050301.ORD_TYPE,
				PICK_TIME = pickTime,
				RETAIL_CODE = f050301.RETAIL_CODE,
				PRINT_FLAG = "0",
				STATUS = 0,
				TMPR_TYPE = tmprType,
				SELF_TAKE = "1",
				FRAGILE_LABEL = f1903s.Any(x => x.FRAGILE == "1") ? "1" : "0",
				GUARANTEE = f1903s.Any(x => x.LG == "1") ? "1" : "0",
				HELLO_LETTER = f050301.HELLO_LETTER,
				SA = f050301.SA,
				CUST_NAME = f050301.CUST_NAME,
				INVOICE_PRINT_CNT = 0,
				GENDER = f050301.GENDER,
				AGE = f050301.AGE,
				ORD_PROP = f050301.ORD_PROP,
				// 固定1 不裝車
				NO_LOADING = "1",
				SA_QTY = isFirstWmsOrder ? (f050301.SA_QTY ?? 0) : (short)0,
				NO_AUDIT = "0",
				PRINT_PASS = "0",
				PRINT_DELV = "0",
				// SOURCE_TYPE=08(銷毀)不印箱明細
				PRINT_BOX = f050301.SOURCE_TYPE != "08" ? "1" : "0",
				ARRIVAL_DATE = f050301.ARRIVAL_DATE,
				VIRTUAL_ITEM = "0",
				SOURCE_TYPE = f050301.SOURCE_TYPE,
				ALL_ID = f050301.ALL_ID,
				SPECIAL_BUS = f050301.SPECIAL_BUS,
				ZIP_CODE = f050301.ZIP_CODE,
				CAN_FAST = f050301.CAN_FAST,
				ALLOWORDITEM = f1903s.Any(x => x.ALLOWORDITEM == "1") ? "1" : "0",
				PRINT_DETAIL_FLAG = "0",
				VOLUMN = GetTotalItemsVolume(gupCode, custCode, f050306s.GroupBy(x => x.ITEM_CODE).Select(x => new WmsOrderItemSumQty { ItemCode = x.Key, Qty = x.Sum(y => y.B_PICK_QTY) }).ToList()),
				WEIGHT = GetTotalItemsWeight(gupCode, custCode, f050306s.GroupBy(x => x.ITEM_CODE).Select(x => new WmsOrderItemSumQty { ItemCode = x.Key, Qty = x.Sum(y => y.B_PICK_QTY) }).ToList()),
				COLLECT_AMT = isFirstWmsOrder ? f050301.COLLECT_AMT : 0,
				SA_CHECK_QTY = isFirstWmsOrder ? f050301.SA_CHECK_QTY : (short)0,
				NO_DELV = "0",
				DELV_PERIOD = f050301.DELV_PERIOD,
				CVS_TAKE = f050301.CVS_TAKE,
				SELFTAKE_CHECKCODE = "0",
				CHECK_CODE = f050301.CHECK_CODE,
				A_ARRIVAL_DATE = DateTime.Today.AddDays(1),
				ROUND_PIECE = f050301.ROUND_PIECE,
				CUST_COST = f050301.CUST_COST,
				FAST_DEAL_TYPE = f050301.FAST_DEAL_TYPE,
        SUG_BOX_NO = f050301.SUG_BOX_NO,
        ISPACKCHECK = f050301.ISPACKCHECK,
        PACKING_TYPE = f050301.PACKING_TYPE,
        ORDER_CRT_DATE = f050301.ORDER_CRT_DATE,
        ORDER_PROC_TYPE = f050301.ORDER_PROC_TYPE,
        ORDER_ZIP_CODE = f050301.ORDER_ZIP_CODE,
        IS_NORTH_ORDER = f050301.IS_NORTH_ORDER,
				MOVE_OUT_TARGET = f050301.MOVE_OUT_TARGET,
        SUG_LOGISTIC_CODE = f050301.SUG_LOGISTIC_CODE,
        NP_FLAG = f050301.NP_FLAG,
      };
			_createPick.F050801s.Add(f050801);

			#endregion

			#region 建立訂單與出貨單關聯檔[F05030101]

			var f05030101 = new F05030101
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ORD_NO = ordNo,
				WMS_ORD_NO = f050801.WMS_ORD_NO
			};
			_createPick.F05030101s.Add(f05030101);

			#endregion

			var groupItems = f050306s.GroupBy(x => new { x.ITEM_CODE, x.SERIAL_NO }).ToList();
			var seq = 0;
			foreach (var item in groupItems)
			{
				seq++;
				var allotQty = item.Key.SERIAL_NO == "0" ? item.Sum(x => x.B_PICK_QTY) : 1;

				#region 建立出貨單明細[F050802]
				var f050802 = new F050802
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					WMS_ORD_NO = f050801.WMS_ORD_NO,
					WMS_ORD_SEQ = seq.ToString().PadLeft(4, '0'),
					ITEM_CODE = item.Key.ITEM_CODE,
					ORD_QTY = allotQty,
					B_DELV_QTY = allotQty,
					A_DELV_QTY = 0,
					SERIAL_NO = item.Key.SERIAL_NO == "0" ? string.Empty : item.Key.SERIAL_NO,
				};
				_createPick.F050802s.Add(f050802);
				#endregion

				var groupByWmsSeqs = item.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.WMS_NO, x.WMS_SEQ });
				foreach (var groupByWmsSeq in groupByWmsSeqs)
				{
					#region 建立配庫後訂單明細與出貨明細對照表[F05030202]

					var f05030202 = new F05030202
					{
						DC_CODE = groupByWmsSeq.Key.DC_CODE,
						GUP_CODE = groupByWmsSeq.Key.GUP_CODE,
						CUST_CODE = groupByWmsSeq.Key.CUST_CODE,
						ORD_NO = groupByWmsSeq.Key.WMS_NO,
						ORD_SEQ = groupByWmsSeq.Key.WMS_SEQ,
						WMS_ORD_NO = f050802.WMS_ORD_NO,
						WMS_ORD_SEQ = f050802.WMS_ORD_SEQ,
						B_DELV_QTY = groupByWmsSeq.Sum(x => x.B_PICK_QTY),
						A_DELV_QTY = 0,
					};
					_createPick.F05030202s.Add(f05030202);

					#endregion

					#region 回塞配庫後揀貨資料[F050306] 出貨單與出貨項次
					foreach (var f050306 in groupByWmsSeq)
					{
						f050306.WMS_ORD_NO = f050802.WMS_ORD_NO;
						f050306.WMS_ORD_SEQ = f050802.WMS_ORD_SEQ;
					}
					#endregion
				}
			}
		}

		/// <summary>
		/// 取得訂單總材積
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="wmsOrderItemSumQties"></param>
		/// <returns></returns>
		private decimal GetTotalItemsVolume(string gupCode,string custCode, List<WmsOrderItemSumQty> wmsOrderItemSumQties)
		{
			var itemCodes = wmsOrderItemSumQties.Select(a => a.ItemCode).Distinct().ToList();
			var f1905s = GetF1905s(gupCode, custCode, itemCodes);
			// 商品總體積
			var totalVolume = (from a in f1905s
												 join b in wmsOrderItemSumQties on a.ITEM_CODE equals b.ItemCode
												 select new { a, b })
																			.Sum(x => x.a.PACK_HIGHT * x.a.PACK_LENGTH * x.a.PACK_WIDTH * x.b.Qty);
			return totalVolume;
		}

		/// <summary>
		/// 取得訂單總重量
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="wmsOrderItemSumQties"></param>
		/// <returns></returns>
		private decimal GetTotalItemsWeight(string gupCode,string custCode, List<WmsOrderItemSumQty> wmsOrderItemSumQties)
		{
			var itemCodes = wmsOrderItemSumQties.Select(a => a.ItemCode).Distinct().ToList();
			var f1905s = GetF1905s(gupCode, custCode,itemCodes);
			// 商品總重量
			var totalWeight = (from a in f1905s
												 join b in wmsOrderItemSumQties on a.ITEM_CODE equals b.ItemCode
												 select new { a, b })
												 .Sum(x => x.a.PACK_WEIGHT * x.b.Qty);
			return totalWeight;
		}
		#endregion

		#region 建立揀貨單
		/// <summary>
		/// 建立揀貨單
		/// </summary>
		/// <param name="dcShipSetting">物流中心指示</param>
		/// <param name="delvDate">批次日期</param>
		/// <param name="pickTime">批次時間</param>
		/// <param name="PKArea">揀貨區編號</param>
		/// <param name="PKAreaName">揀貨區名稱</param>
		/// <param name="ContainerType">指定容器</param>
		/// <param name="MergeNo"></param>
		/// <returns></returns>
		private ExecuteResult CreatePickOrders(F190105 dcShipSetting, DateTime delvDate, string pickTime,bool isUserDirectPriorityCode=false)
		{
			var pickOrderMaxRecord = dcShipSetting.PICKORDER_MAX_RECORD;
			// 揀貨單最大出貨單數(分貨作業最大格數)
			var maxOrderCntByPick = 12;

      foreach (var pickType in _tempPickTypeList)
      {
        switch (pickType.Key)
				{
					case PickTypeEnums.AutoPick:
					case PickTypeEnums.AutoSelfPick:
						// 自動倉依倉庫+出貨單拆揀貨單
						var moveOutF050306s = pickType.Value.Where(a => a.CUST_COST == "MoveOut").ToList();
						var otherF050306s = pickType.Value.Where(a => a.CUST_COST != "MoveOut").ToList();
						var groupMoveOutWhs = moveOutF050306s.GroupBy(x => new { x.WAREHOUSE_ID });
            var groupWhAndOrders = otherF050306s.GroupBy(x => new { x.WAREHOUSE_ID, x.WMS_ORD_NO });
            _newPickOrdNosStack = GetNewOrdStackCodes("P", groupMoveOutWhs.Count() + groupWhAndOrders.Count());
            foreach (var g in groupMoveOutWhs)
            {
              CreatePickOrder(delvDate, pickTime, pickType.Key, g.ToList(), true, SplitType.Warehouse, g.Key.WAREHOUSE_ID, g.First().PK_AREA, g.First().PK_AREA_NAME, null, null, null, null, null, isUserDirectPriorityCode);
            }
            foreach (var g in groupWhAndOrders)
            {
              CreatePickOrder(delvDate, pickTime, pickType.Key, g.ToList(), false, SplitType.Warehouse, g.Key.WAREHOUSE_ID, g.First().PK_AREA, g.First().PK_AREA_NAME, null, g.First().WMS_ORD_NO, g.First().ORDER_ZIP_CODE, g.First().IS_NORTH_ORDER, null, isUserDirectPriorityCode, g.First().NP_FLAG);
            }
            break;
					case PickTypeEnums.FullArtificialSinglePick:
            // 人工倉單一揀貨 依出貨單拆揀貨單
            var groupOrders = pickType.Value.OrderByDescending(x => x.IS_NORTH_ORDER).GroupBy(x => new { x.WMS_ORD_NO });
            _newPickOrdNosStack = GetNewOrdStackCodes("P", groupOrders.Count());
            foreach (var g in groupOrders)
            {
              var PK_AREA = g.GroupBy(x => x.PK_AREA).Count() > 1 ? null : g.First().PK_AREA;
              var PK_AREA_NAME = string.IsNullOrWhiteSpace(PK_AREA) ? null : g.First().PK_AREA_NAME;
              CreatePickOrder(delvDate, pickTime, pickType.Key, g.ToList(), false, SplitType.ShipOrder, g.Key.WMS_ORD_NO, PK_AREA, PK_AREA_NAME, null, g.First().WMS_ORD_NO, g.First().ORDER_ZIP_CODE, g.First().IS_NORTH_ORDER, g.First().SUG_LOGISTIC_CODE, isUserDirectPriorityCode);
            }
            break;
					case PickTypeEnums.SpecialOrderPick:
						// 特殊結構訂單依倉庫拆揀貨單
						// 若倉庫為人工倉依照揀貨單上限筆數拆揀貨單
						var groupArtificalWhs = pickType.Value.Where(x => x.DEVICE_TYPE == "0").GroupBy(x => new { x.WAREHOUSE_ID });

						foreach (var g in groupArtificalWhs)
						{
							var pages = g.Count() / pickOrderMaxRecord + ((g.Count() % pickOrderMaxRecord) > 0 ? 1 : 0);
							_newPickOrdNosStack = GetNewOrdStackCodes("P", pages);
							for (var page = 0; page < pages; page++)
								CreatePickOrder(delvDate, pickTime, pickType.Key, g.Skip(page * pickOrderMaxRecord).Take(pickOrderMaxRecord).ToList(), true, SplitType.Warehouse, g.Key.WAREHOUSE_ID, g.First().PK_AREA, g.First().PK_AREA_NAME, null, null, null, null,null, isUserDirectPriorityCode);
						}

						// 若倉庫為自動倉依倉庫拆揀貨單
						var groupAutoWhs = pickType.Value.Where(x => x.DEVICE_TYPE != "0").GroupBy(x => new { x.WAREHOUSE_ID });
						_newPickOrdNosStack = GetNewOrdStackCodes("P", groupAutoWhs.Count());
						foreach (var g in groupAutoWhs)
							CreatePickOrder(delvDate, pickTime, pickType.Key, g.ToList(), true, SplitType.Warehouse, g.Key.WAREHOUSE_ID, g.First().PK_AREA, g.First().PK_AREA_NAME, null, null, null, null,null, isUserDirectPriorityCode);
						break;
					case PickTypeEnums.ArtificialBatchPick:

						// 人工倉依PK區拆揀貨單在依照揀貨單上限筆數拆揀貨單
						var groupWhs = pickType.Value.GroupBy(x => new { x.PK_AREA });
						foreach (var g in groupWhs)
						{
							// 先依儲位排序
							var groupItems = g.GroupBy(x => new { x.PICK_LOC, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO, x.SERIAL_NO }).OrderBy(x => x.Key.PICK_LOC).ToList();
							var pickPages = new Dictionary<int, List<F050306>>();
							var pickDetails = new List<F050306>();
							var recordCnt = 0;
							var wmsOrdNos = new List<string>();
							var page = 1;
							foreach (var group in groupItems)
							{
								var currentNotRepeatWmsOrdNos = group.Select(x => x.WMS_ORD_NO).Distinct().Except(wmsOrdNos).ToList();
								//  累積 加總後資料筆數 < 揀貨單上限筆數 且 出貨單數+本次出貨不重複出貨單數 <= 揀貨單最大出貨單數(分貨作業最大格數)
								if (recordCnt < pickOrderMaxRecord && currentNotRepeatWmsOrdNos.Count + wmsOrdNos.Count <= maxOrderCntByPick)
								{
									pickDetails.AddRange(group.ToList());
									recordCnt++;
									wmsOrdNos.AddRange(currentNotRepeatWmsOrdNos);
								}
								else
								{
									// 如果加總後資料筆數 >= 揀貨單上限筆數 或 出貨單數+本次出貨不重複出貨單數 > 揀貨單最大出貨單數(分貨作業最大格數) 要切新揀貨單
									if ((recordCnt >= pickOrderMaxRecord))
									{
										pickPages.Add(page, pickDetails);
										page++;
										wmsOrdNos = new List<string>();
										pickDetails = new List<F050306>();
										recordCnt = 0;
									}
									var isCreateNewPage = false;
									// 此頁含有本次新增內容的出貨單，將含有此出貨單明細加入到此頁
									var sameWmsNoDatas = group.Where(x => wmsOrdNos.Contains(x.WMS_ORD_NO)).ToList();
									if(sameWmsNoDatas.Any())
										pickDetails.AddRange(sameWmsNoDatas);

									// 非此頁含有的出貨單，以一張一張出貨單嘗試放入此頁，但不可超過揀貨單最大出貨單數(分貨作業最大格數)，若放不下則切新揀貨單
									var gWmsOrders = group.Where(x => !wmsOrdNos.Contains(x.WMS_ORD_NO)).GroupBy(x => x.WMS_ORD_NO);
									foreach(var gWmsOrder in gWmsOrders)
									{
										if (wmsOrdNos.Count + 1 <= maxOrderCntByPick)
										{
											pickDetails.AddRange(gWmsOrder.ToList());
											wmsOrdNos.Add(gWmsOrder.Key);
										}
										else
										{
											pickPages.Add(page, pickDetails);
											page++;
											wmsOrdNos = new List<string> { gWmsOrder.Key };
											pickDetails = gWmsOrder.ToList();
											isCreateNewPage = true;
											recordCnt = 1;
										}
									}
									// 如果沒有產生新的揀貨單，揀貨單筆數+1
									if (!isCreateNewPage)
										recordCnt += 1;
								}
							}
							if (pickDetails.Any())
								pickPages.Add(page, pickDetails);

							_newPickOrdNosStack = GetNewOrdStackCodes("P", pickPages.Count());
							foreach (var pickPage in pickPages)
								CreatePickOrder(delvDate, pickTime, pickType.Key, pickPage.Value, true, SplitType.PkArea, g.Key.PK_AREA, g.First().PK_AREA, g.First().PK_AREA_NAME, null, null, null, null, null,isUserDirectPriorityCode);
						}
						break;
					case PickTypeEnums.FullArtificialSelfBatchPick:
						var groupPkAreaSelfs = pickType.Value.GroupBy(x => new { x.PK_AREA });
						foreach (var g in groupPkAreaSelfs)
						{
							// 先依儲位排序
							var groupWmsOrders = g.OrderBy(x => x.PICK_LOC).GroupBy(x => x.WMS_ORD_NO).ToList();
							var tempList = new List<F050306>();
							var pickPages = new Dictionary<int, List<F050306>>();
							var page = 1;
							var wmsOrdNos = new List<string>();
							foreach (var wmsOrders in groupWmsOrders)
							{
								tempList.AddRange(wmsOrders.ToList());
								wmsOrdNos.Add(wmsOrders.Key);
								var count = tempList.GroupBy(x => new { x.PICK_LOC, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO, x.SERIAL_NO }).Count();
								// 符合揀貨單最大出貨單數 且 總揀明細數小於等於最大揀貨明細數
								if (wmsOrdNos.Count <= maxOrderCntByPick && count <= pickOrderMaxRecord)
								{
									continue;
								}
								else
								{
									if (tempList.Where(x => x.WMS_ORD_NO != wmsOrders.Key).Any())
									{
										pickPages.Add(page, tempList.Where(x => x.WMS_ORD_NO != wmsOrders.Key).ToList());
										page++;
										tempList = new List<F050306>();
										wmsOrdNos = new List<string>();

										var findPage = false;
										// 取得其他揀貨單還可以再放O單
										var canAddPickPages = pickPages.Where(x => x.Value.Select(y => y.WMS_ORD_NO).Distinct().Count() < maxOrderCntByPick).ToList();
										// 試著將此O單明細加入揀貨單，若放進去後沒超過最大揀貨單筆數限制，則將此O單明細放入此揀貨單
										foreach (var pickPage in canAddPickPages)
										{
											pickPage.Value.AddRange(wmsOrders.ToList());
											var c = pickPage.Value.GroupBy(x => new { x.PICK_LOC, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO, x.SERIAL_NO }).Count();
											if (c <= pickOrderMaxRecord)
											{
												findPage = true;
												break;
											}
											else
												pickPage.Value.RemoveAll(x => x.WMS_ORD_NO == wmsOrders.Key);
										}
										// 如果沒有找到可放入此O單明細，則開啟新揀貨單
										if (!findPage)
										{
											tempList.AddRange(wmsOrders.ToList());
											wmsOrdNos.Add(wmsOrders.Key);
										}
									}
									else
									{
										pickPages.Add(page, tempList.ToList());
										page++;
										tempList = new List<F050306>();
										wmsOrdNos = new List<string>();
									}
								}
							}
							if (tempList.Any())
								pickPages.Add(page, tempList);

							_newPickOrdNosStack = GetNewOrdStackCodes("P", pickPages.Count);
							foreach (var pickPage in pickPages)
							{
								// 如果該批次只有一張出貨單並且不是跨庫，直接轉單一揀貨
								if (pickPage.Value.Select(x => x.WMS_ORD_NO).Distinct().Count() == 1 && pickPage.Value.All(a => a.CUST_COST != "MoveOut"))
									CreatePickOrder(delvDate, pickTime, PickTypeEnums.FullArtificialSinglePick, pickPage.Value, false, SplitType.ShipOrder, pickPage.Value.First().WMS_ORD_NO, g.First().PK_AREA, g.First().PK_AREA_NAME, null, pickPage.Value.First().WMS_ORD_NO, pickPage.Value.First().ORDER_ZIP_CODE, pickPage.Value.First().IS_NORTH_ORDER, pickPage.Value.First().SUG_LOGISTIC_CODE ,isUserDirectPriorityCode);
								else
									CreatePickOrder(delvDate, pickTime, pickType.Key, pickPage.Value, true, SplitType.PkArea, g.Key.PK_AREA, g.First().PK_AREA, g.First().PK_AREA_NAME, null, null, null, null,null, isUserDirectPriorityCode);
							}
						}
						break;
					case PickTypeEnums.LackPick:
            // 快速補揀單-人工倉依出貨單拆揀貨單
            var groupArtificalLackOrders = pickType.Value
              .Where(x => x.DEVICE_TYPE == "0")
              .OrderBy(x=>x.ORDER_CRT_DATE)
              .ThenByDescending(x => x.IS_NORTH_ORDER)
              .GroupBy(x => new { x.WMS_ORD_NO });

            _newPickOrdNosStack = GetNewOrdStackCodes("P", groupArtificalLackOrders.Count());

            foreach (var g in groupArtificalLackOrders)
              CreatePickOrder(delvDate, pickTime, pickType.Key, g.ToList(), false, SplitType.ShipOrder, g.Key.WMS_ORD_NO, g.First().PK_AREA, g.First().PK_AREA_NAME, g.First().CONTAINER_TYPE, g.Key.WMS_ORD_NO, g.First().ORDER_ZIP_CODE, g.First().IS_NORTH_ORDER,g.First().SUG_LOGISTIC_CODE, isUserDirectPriorityCode);
            
            // 快速補揀單-自動倉依倉+出貨單拆揀貨單
            var groupAutoLackOrders = pickType.Value
              .Where(x => x.DEVICE_TYPE != "0")
              .OrderBy(x => x.ORDER_CRT_DATE)
              .ThenByDescending(x => x.IS_NORTH_ORDER)
              .GroupBy(x => new { x.WAREHOUSE_ID, x.WMS_ORD_NO });

            _newPickOrdNosStack = GetNewOrdStackCodes("P", groupAutoLackOrders.Count());

            foreach (var g in groupAutoLackOrders)
						{
							var containerType = g.First().CONTAINER_TYPE;
							// 如果物流中心設為指定容器，因為如果一開始為人工倉揀貨單就不會設定指定容器，改抓一開始計算的出貨單指定容器
							if (dcShipSetting.IS_DIRECT_CONTAINER_TYPE == "1" && string.IsNullOrWhiteSpace(containerType))
							{
								var f050801Repo = new F050801Repository(Schemas.CoreSchema);
								var f050801 = f050801Repo.GetData(g.Key.WMS_ORD_NO, g.First().GUP_CODE, g.First().CUST_CODE, g.First().DC_CODE);
								containerType = f050801.CONTAINER_TYPE;
							}

							CreatePickOrder(delvDate, pickTime, pickType.Key, g.ToList(), false, SplitType.Warehouse, g.Key.WAREHOUSE_ID, g.First().PK_AREA, g.First().PK_AREA_NAME, containerType, g.Key.WMS_ORD_NO, g.First().ORDER_ZIP_CODE, g.First().IS_NORTH_ORDER);
						}
						break;
				}
			}

			CreateAutoPickTasks(dcShipSetting, delvDate, pickTime);

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 下發自動倉揀貨任務
		/// </summary>
		/// <param name="dcShipSetting"></param>
		/// <param name="delvDate"></param>
		/// <param name="pickTime"></param>
		private void CreateAutoPickTasks(F190105 dcShipSetting, DateTime delvDate, string pickTime)
		{
			#region 下發自動倉揀貨任務
			var canCreateAutoPickers = new List<F051201>();
			var fullAutoPickTypes = new string[] { ((int)PickTypeEnums.AutoSelfPick).ToString(), ((int)PickTypeEnums.SpecialOrderPick).ToString(), ((int)PickTypeEnums.LackPick).ToString() };
			var fullAutoPickers = _createPick.F051201s.Where(x => x.DELV_DATE == delvDate && x.PICK_TIME == pickTime && x.DISP_SYSTEM != "0" && fullAutoPickTypes.Contains(x.PICK_TYPE)).ToList();
			canCreateAutoPickers.AddRange(fullAutoPickers);
			var notFullAutoPickTypes = new string[] { ((int)PickTypeEnums.AutoPick).ToString() };
			var notFullAutoPickers = _createPick.F051201s.Where(x => x.DELV_DATE == delvDate && x.PICK_TIME == pickTime && notFullAutoPickTypes.Contains(x.PICK_TYPE)).ToList();
			if (notFullAutoPickers.Any())
			{
				// 不需等待人工倉揀貨完成即可下發自動倉揀貨任務
				if (dcShipSetting.WAIT_SEND_AUTO_PICK == "0" && notFullAutoPickers.Any())
					canCreateAutoPickers.AddRange(notFullAutoPickers);
				else //需等待人工倉揀貨完成一樣產生自動倉揀貨任務但狀態為3(等待人工倉揀貨完成)
					CreateAutoPickerTasks(notFullAutoPickers, "3");
			}

			CreateAutoPickerTasks(canCreateAutoPickers, "0");
			#endregion
		}

    /// <summary>
    /// 建立揀貨單、揀貨明細、虛擬庫存、批量揀貨明細，Return的No為揀貨單號
    /// </summary>
    /// <param name="delvDate"></param>
    /// <param name="pickTime"></param>
    /// <param name="pickTypeEnums"></param>
    /// <param name="f050306s"></param>
    /// <param name="isCreateBatchPick"></param>
    /// <param name="splitType"></param>
    /// <param name="splitCode"></param>
    /// <param name="PKArea"></param>
    /// <param name="PKAreaName"></param>
    /// <param name="ContainerType"></param>
    /// <param name="MergeNo"></param>
    /// <returns>執行結果 No為揀貨單號</returns>
    private ExecuteResult CreatePickOrder(DateTime delvDate, string pickTime, PickTypeEnums pickTypeEnums,
        List<F050306> f050306s, bool isCreateBatchPick, SplitType splitType, string splitCode,
        string PKArea, string PKAreaName, string ContainerType, string MergeNo, string orderZipCode = null,
        string isNorthOrder = null, string sugLogisticCode = null, bool isUserDirectPriorityCode = false, string npFlag = "0")
    {
      var result = new ExecuteResult(true);

			if (_createPick.F051201s == null)
				_createPick.F051201s = new List<F051201>();
			if (_createPick.F051202s == null)
				_createPick.F051202s = new List<F051202>();
			if (_createPick.F051203s == null)
				_createPick.F051203s = new List<F051203>();
			if (_createPick.F1511s == null)
				_createPick.F1511s = new List<F1511>();
      if (_createPick.F0535s == null)
        _createPick.F0535s = new List<F0535>();
      if (_createPick.F050306_HISTORYs == null)
        _createPick.F050306_HISTORYs = new List<F050306_HISTORY>();

      var first = f050306s.First();
      var pickOrdNo = _newPickOrdNosStack.Pop();
      result.No = pickOrdNo;
      String tmpMergeNo;
      if (first.CUST_COST?.ToUpper() == "MoveOut".ToUpper())
        tmpMergeNo = null;
      else if (new[] { 1, 3 }.Contains((int)pickTypeEnums))
        tmpMergeNo = null;
      else if (String.IsNullOrWhiteSpace(MergeNo))
        tmpMergeNo = pickOrdNo;
      else
        tmpMergeNo = MergeNo;

      #region 建立揀貨單頭檔[F051201]

      var f051201 = new F051201
      {
        DC_CODE = first.DC_CODE,
				GUP_CODE = first.GUP_CODE,
				CUST_CODE = first.CUST_CODE,
				ORD_TYPE = first.ORD_TYPE,
				PICK_TYPE = ((int)pickTypeEnums).ToString(),
				PICK_ORD_NO = pickOrdNo,
				DELV_DATE = delvDate,
				PICK_TIME = pickTime,
				DISP_SYSTEM = first.DEVICE_TYPE == "0" ? "0" : "1",
				NEXT_STEP = GetNextStep(pickTypeEnums, first),
				ISDEVICE = "0",
				DEVICE_PRINT = "0",
				ISPRINTED = "0",
				PICK_TOOL = first.DEVICE_TYPE == "0" ? "1" : "0",
				PICK_STATUS = 0,
				SPLIT_TYPE = ((int)splitType).ToString().PadLeft(2, '0'),
				SPLIT_CODE = splitCode,
				MOVE_OUT_TARGET = first.MOVE_OUT_TARGET,
				PACKING_TYPE = first.PACKING_TYPE,
        CONTAINER_TYPE = ContainerType,
        PK_AREA = PKArea,
				PK_AREA_NAME = PKAreaName,
				MERGE_NO = tmpMergeNo,
				RTN_VNR_CODE = first.RTN_VNR_CODE,
        ORDER_CRT_DATE = first.ORDER_CRT_DATE,
        ORDER_PROC_TYPE = first.ORDER_PROC_TYPE,
        ORDER_ZIP_CODE = orderZipCode,
        IS_NORTH_ORDER = isNorthOrder,
        PRIORITY_CODE = pickTypeEnums == PickTypeEnums.LackPick ? "SecondPick" : (!isUserDirectPriorityCode && npFlag == "1" ? "AppleProd" : first.FAST_DEAL_TYPE),
        IS_USER_DIRECT_PRIORITY = isUserDirectPriorityCode ? "1" : "0",
				SUG_LOGISTIC_CODE = sugLogisticCode,
				DEVICE_TYPE = first.DEVICE_TYPE,
        NP_FLAG = npFlag,
      };
      if (string.IsNullOrWhiteSpace(f051201.PRIORITY_CODE))
        f051201.PRIORITY_CODE = "1";

      if (f051201.DEVICE_TYPE == "0" && f051201.PRIORITY_CODE != "SecondPick") //人工倉 AND 一般揀貨單
				f051201.PRIORITY_VALUE = int.Parse(f051201.PRIORITY_CODE);
			else if (f051201.DEVICE_TYPE == "0" && f051201.PRIORITY_CODE == "SecondPick") // 人工倉 AND 補揀單
				f051201.PRIORITY_VALUE = 99999;
			else //自動倉抓對應檔
			{
				var item = GetF195601s().FirstOrDefault(x => x.DC_CODE == f051201.DC_CODE && x.PRIORITY_CODE == f051201.PRIORITY_CODE && x.DEVICE_TYPE == f051201.DEVICE_TYPE);
				f051201.PRIORITY_VALUE = item == null ? (int?)null : item.PRIORITY_VALUE;
				if(!f051201.PRIORITY_VALUE.HasValue)
				{
					f051201.PRIORITY_CODE = first.FAST_DEAL_TYPE;
					item = GetF195601s().FirstOrDefault(x => x.DC_CODE == f051201.DC_CODE && x.PRIORITY_CODE == f051201.PRIORITY_CODE && x.DEVICE_TYPE == f051201.DEVICE_TYPE);
					f051201.PRIORITY_VALUE = item == null ? (int?)null : item.PRIORITY_VALUE;
				}
			}
      _createPick.F051201s.Add(f051201);

      #endregion

      //f050306s = f050306s.OrderBy(x => x.PICK_LOC).ThenBy(x => x.ITEM_CODE).ToList();
      // 把F050306的寫入F051202會用到的欄位做群組，避免前端打單的時候用兩個一模一樣的條件導致出現揀貨單出現重複的項目
      // 揀貨單排序(先依儲位ASC、品號ASC排序 未來照路順排)
      var grpF050306 = f050306s.GroupBy(x => new { x.PICK_LOC, x.ITEM_CODE, x.ENTER_DATE, x.VALID_DATE, x.MAKE_NO, x.SERIAL_NO, x.VNR_CODE, x.BOX_CTRL_NO, x.PALLET_CTRL_NO, x.WMS_ORD_NO, x.WMS_ORD_SEQ, x.PK_AREA, x.PK_AREA_NAME }).OrderBy(x => x.Key.PICK_LOC).ThenBy(x => x.Key.ITEM_CODE).ToList();

      var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var seq = 1;
			var routeSeq = 0;
			foreach (var f050306 in grpF050306)
			{
				routeSeq++;
				#region 建立揀貨明細檔[F051202]
				var f051202 = new F051202
				{
          DC_CODE = first.DC_CODE,
          GUP_CODE = first.GUP_CODE,
          CUST_CODE = first.CUST_CODE,
          PICK_ORD_NO = pickOrdNo,
          PICK_ORD_SEQ = seq.ToString().PadLeft(4, '0'),
          PICK_LOC = f050306.Key.PICK_LOC,
          ITEM_CODE = f050306.Key.ITEM_CODE,
          ENTER_DATE = f050306.Key.ENTER_DATE,
          VALID_DATE = f050306.Key.VALID_DATE,
          MAKE_NO = f050306.Key.MAKE_NO,
          SERIAL_NO = f050306.Key.SERIAL_NO == "0" ? string.Empty : f050306.Key.SERIAL_NO,
          VNR_CODE = f050306.Key.VNR_CODE,
          BOX_CTRL_NO = f050306.Key.BOX_CTRL_NO,
          PALLET_CTRL_NO = f050306.Key.PALLET_CTRL_NO,
          WMS_ORD_NO = f050306.Key.WMS_ORD_NO,
          WMS_ORD_SEQ = f050306.Key.WMS_ORD_SEQ,
          PICK_STATUS = "0",
          B_PICK_QTY = f050306.Sum(x => x.B_PICK_QTY),
          A_PICK_QTY = 0,
          ROUTE_SEQ = routeSeq,
          PK_AREA = f050306.Key.PK_AREA,
          PK_AREA_NAME = f050306.Key.PK_AREA_NAME
        };
				_createPick.F051202s.Add(f051202);
				#endregion

				#region 建立虛擬庫存檔[F1511]

				var f1511 = new F1511
				{
					DC_CODE = first.DC_CODE,
					GUP_CODE = first.GUP_CODE,
					CUST_CODE = first.CUST_CODE,
					ORDER_NO = pickOrdNo,
					ORDER_SEQ = seq.ToString().PadLeft(4, '0'),
					LOC_CODE = f050306.Key.PICK_LOC,
					ITEM_CODE = f050306.Key.ITEM_CODE,
					VALID_DATE = f050306.Key.VALID_DATE,
					ENTER_DATE = f050306.Key.ENTER_DATE,
					MAKE_NO = f050306.Key.MAKE_NO,
					SERIAL_NO = f050306.Key.SERIAL_NO,
					BOX_CTRL_NO = f050306.Key.BOX_CTRL_NO,
					PALLET_CTRL_NO = f050306.Key.PALLET_CTRL_NO,
					STATUS = "0",
					B_PICK_QTY = f050306.Sum(x => x.B_PICK_QTY),
					A_PICK_QTY = 0
				};
				_createPick.F1511s.Add(f1511);

        #endregion

        #region 建立F050306History
        foreach (var item in f050306)
        {
          var addF050306History = JsonConvert.DeserializeObject<F050306_HISTORY>(JsonConvert.SerializeObject(item));
          addF050306History.PICK_ORD_NO = pickOrdNo;
          _createPick.F050306_HISTORYs.Add(addF050306History);
        }
        #endregion 建立F050306History
        seq++;
			}

			if (isCreateBatchPick)
			{
        var group = f050306s.GroupBy(x =>
				new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.PICK_LOC, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO, x.SERIAL_NO });
				var f051203Repo = new F051203Repository(Schemas.CoreSchema);
				var batchseq = 1;
				routeSeq = 0;
				foreach (var g in group.OrderBy(a => a.Key.PICK_LOC))
				{
					routeSeq++;

					#region 建立批量揀貨明細[F051203]

					var f051203 = new F051203
					{
						DC_CODE = g.Key.DC_CODE,
						GUP_CODE = g.Key.GUP_CODE,
						CUST_CODE = g.Key.CUST_CODE,
						PICK_ORD_NO = pickOrdNo,
						TTL_PICK_SEQ = batchseq.ToString().PadLeft(4, '0'),
						PICK_LOC = g.Key.PICK_LOC,
						ITEM_CODE = g.Key.ITEM_CODE,
						VALID_DATE = g.Key.VALID_DATE,
						MAKE_NO = g.Key.MAKE_NO,
						SERIAL_NO = g.Key.SERIAL_NO == "0" ? string.Empty : g.Key.SERIAL_NO,
						PICK_STATUS = "0",
						B_PICK_QTY = g.Sum(x => x.B_PICK_QTY),
						A_PICK_QTY = 0,
						ROUTE_SEQ = routeSeq,
						PK_AREA = first.PK_AREA,
						PK_AREA_NAME = first.PK_AREA_NAME
					};

					_createPick.F051203s.Add(f051203);
          #endregion

          batchseq++;
				}

        #region No.2193 如果為跨庫揀貨單，增加寫入F0535資料表

        if (f050306s.Any(o => o.CUST_COST == "MoveOut"))
        {
          var groupForF0535 = f050306s.Where(o => o.CUST_COST == "MoveOut").GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.CUST_COST, x.WMS_ORD_NO });

          foreach (var addF0535 in groupForF0535)
          {
            _createPick.F0535s.Add(new F0535
            {
              DC_CODE = addF0535.Key.DC_CODE,
              GUP_CODE = addF0535.Key.GUP_CODE,
              CUST_CODE = addF0535.Key.CUST_CODE,
              PICK_ORD_NO = pickOrdNo,
              WMS_ORD_NO = addF0535.Key.WMS_ORD_NO,
              STATUS = "0"
            });
          }
        }
        #endregion
      }
      return result;
		}

		/// <summary>
		/// 依揀貨單類型取得下一步作業
		/// </summary>
		/// <param name="pickTypeEnums"></param>
		/// <returns></returns>
		private string GetNextStep(PickTypeEnums pickTypeEnums, F050306 f050306)
		{
			NextStep nextStep;
			//跨庫出貨[CUST_COST=MoveOut]
			if (f050306.CUST_COST == "MoveOut")
				nextStep = NextStep.CrossAllotPier;
			else
			{
				switch (pickTypeEnums)
				{
					case PickTypeEnums.ArtificialBatchPick:
					case PickTypeEnums.FullArtificialSelfBatchPick:
						nextStep = NextStep.AllotStation;
						break;
					case PickTypeEnums.AutoPick:
					case PickTypeEnums.LackPick:
					case PickTypeEnums.SpecialPick2:
						nextStep = NextStep.CollectionStation;
						break;
					case PickTypeEnums.AutoSelfPick:
					case PickTypeEnums.SpecialOrderPick:
					case PickTypeEnums.FullArtificialSinglePick:
					case PickTypeEnums.SpecialPick1:
						nextStep = NextStep.PackingStation;
						break;
					default:
						nextStep = NextStep.NoConfirm;
						break;
				}

			}
			return ((int)nextStep).ToString();
		}

		/// <summary>
		/// 建立自動倉揀貨任務
		/// </summary>
		/// <param name="f051201s">揀貨單</param>
		/// <param name="status">自動倉出庫任務下發狀態(0:待處理 3:等待人工倉揀貨完成)</param>
		private void CreateAutoPickerTasks(List<F051201> f051201s, string status)
		{
			var f060201Repo = new F060201Repository(Schemas.CoreSchema, _wmsTransaction);
			var specialType = ((int)PickTypeEnums.SpecialOrderPick).ToString();
			var specialOrMoveOutPickers = f051201s.Where(x => x.PICK_TYPE == specialType || x.NEXT_STEP == ((int)NextStep.CrossAllotPier).ToString()).ToList();
			var noSpecialPickers = f051201s.Except(specialOrMoveOutPickers).ToList();
			if (_createPick.F060201s == null)
				_createPick.F060201s = new List<F060201>();

			var WmsOrdNoCntList = new Dictionary<string, int>();

			#region 特殊結構或跨庫揀貨單
			foreach (var specialOrMoveOutPicker in specialOrMoveOutPickers)
			{
				var f060201 = new F060201
				{
					DC_CODE = specialOrMoveOutPicker.DC_CODE,
					GUP_CODE = specialOrMoveOutPicker.GUP_CODE,
					CUST_CODE = specialOrMoveOutPicker.CUST_CODE,
					DOC_ID = specialOrMoveOutPicker.PICK_ORD_NO,
					CMD_TYPE = "1",
					PICK_NO = specialOrMoveOutPicker.PICK_ORD_NO,
					WAREHOUSE_ID = specialOrMoveOutPicker.SPLIT_CODE,
					RESENT_CNT = 0,
					STATUS = CheckAutoPickerTasksContainManualTask(specialOrMoveOutPicker.DC_CODE, specialOrMoveOutPicker.GUP_CODE, specialOrMoveOutPicker.CUST_CODE, specialOrMoveOutPicker.PICK_ORD_NO) ? status : "0",
					WMS_NO = specialOrMoveOutPicker.PICK_ORD_NO
				};
				_createPick.F060201s.Add(f060201);
			}
			#endregion

			#region 非特殊結構及跨庫揀貨單
			var group = (from o in noSpecialPickers
									 join d in _createPick.F051202s
									 on new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.PICK_ORD_NO } equals new { d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.PICK_ORD_NO }
									 select new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, d.WMS_ORD_NO, o.PICK_ORD_NO, o.SPLIT_CODE }).Distinct().ToList();
			foreach (var g in group)
			{
				int seq;
				if (WmsOrdNoCntList.ContainsKey(g.WMS_ORD_NO))
					seq = WmsOrdNoCntList[g.WMS_ORD_NO];
				else
				{
					seq = f060201Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == g.DC_CODE && x.GUP_CODE == g.GUP_CODE &&
					x.CUST_CODE == g.CUST_CODE && x.WMS_NO == g.WMS_ORD_NO).Count();

					// 如果產生清單已經有此出貨單，要將筆數加上去
					seq += _createPick.F060201s.Where(x => x.DC_CODE == g.DC_CODE && x.GUP_CODE == g.GUP_CODE &&
					x.CUST_CODE == g.CUST_CODE && x.WMS_NO == g.WMS_ORD_NO).Count();

					WmsOrdNoCntList.Add(g.WMS_ORD_NO, seq);
				}
				var docId = seq == 0 ? g.WMS_ORD_NO : $"{g.WMS_ORD_NO}{seq.ToString().PadLeft(2, '0')}";
				var f060201 = new F060201
				{
					DC_CODE = g.DC_CODE,
					GUP_CODE = g.GUP_CODE,
					CUST_CODE = g.CUST_CODE,
					DOC_ID = docId,
					CMD_TYPE = "1",
					PICK_NO = g.PICK_ORD_NO,
					WAREHOUSE_ID = g.SPLIT_CODE,
					RESENT_CNT = 0,
					STATUS = CheckAutoPickerTasksContainManualTask(g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.PICK_ORD_NO) ? status : "0",
					WMS_NO = g.WMS_ORD_NO
				};
				_createPick.F060201s.Add(f060201);
				WmsOrdNoCntList[g.WMS_ORD_NO]++;
			}
			#endregion

		}

		#endregion

		/// <summary>
		/// 檢查自動倉派發任務是否有包含人工倉任務and下一步不是6(調撥場)，如果有就回傳True
		/// </summary>
		/// <param name="PICK_ORD_NO">揀貨單號</param>
		/// <returns></returns>
		private Boolean CheckAutoPickerTasksContainManualTask(String dcCode, String gupCode, String custCode, String PICK_ORD_NO)
		{
			//No1237增加檢查項目
			var WmsOrdNo = _createPick.F051202s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PICK_ORD_NO == PICK_ORD_NO).First().WMS_ORD_NO;
			var GetPickOrds = from a in _createPick.F051201s
												join b in _createPick.F051202s
															on a.PICK_ORD_NO equals b.PICK_ORD_NO
												where b.WMS_ORD_NO.Equals(WmsOrdNo)
															&& a.DISP_SYSTEM == "0"
															&& a.NEXT_STEP != "6"
												select a;
			return GetPickOrds.Any();
		}

		#region 建立揀貨批次
		private ExecuteResult CreatePickSummary(F190105 dcShipSetting, string pickTime,
	string dcCode, string gupCode, string custCode,
  string ordType, string sourceType, string custCost, string fastDealType, DateTime? orderCrtDate = null, string orderProcType = null,bool isUserDirectPriorityCode=false)
    {
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var nATFLPickTypes = new string[] { ((int)PickTypeEnums.FullArtificialSinglePick).ToString() };
			var bATFLPickTypes = new string[] { ((int)PickTypeEnums.FullArtificialSelfBatchPick).ToString(), ((int)PickTypeEnums.ArtificialBatchPick).ToString() };
			var nAutoPickTypes = new string[] { ((int)PickTypeEnums.AutoPick).ToString(), ((int)PickTypeEnums.AutoSelfPick).ToString() };
			var sPickTypes = new string[] { ((int)PickTypeEnums.SpecialOrderPick).ToString(), ((int)PickTypeEnums.SpecialPick1).ToString(), ((int)PickTypeEnums.SpecialPick2).ToString() };
			var wmsNos = new List<string>();

      // 非特殊結構訂單且非跨庫出貨訂單的出貨單號
      var notSpecialOrderWmsNos = (from o in _createPick.F051201s
                                   join d in _createPick.F051202s
                                   on o.PICK_ORD_NO equals d.PICK_ORD_NO
                                   where !sPickTypes.Contains(o.PICK_TYPE) && o.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString()
                                   select d.WMS_ORD_NO).Distinct().ToList();
      wmsNos.AddRange(notSpecialOrderWmsNos);
      // 特殊結構訂單的揀貨單號
      var specialOrderWmsNos = _createPick.F051201s.Where(x => sPickTypes.Contains(x.PICK_TYPE)).Select(x => x.MERGE_NO).ToList();

      wmsNos.AddRange(specialOrderWmsNos);

      // 跨出出貨訂單揀貨單號
      var crossDcShipPickOrdNos = _createPick.F051201s.Where(x => x.NEXT_STEP == ((int)NextStep.CrossAllotPier).ToString()).Select(x => x.PICK_ORD_NO).ToList();
      //no1174需求 移除揀貨單下一步只是跨出出貨(F051201.NEXT_STEP=6)不寫入wmsNos
      //wmsNos.AddRange(crossDcShipPickOrdNos);

      // 取得出貨單有多個派發揀貨系統且有混人工倉揀貨(人工倉+自動倉) 且非人員指派優先處理旗標，
      // 將自動倉揀貨單WMS處理旗標改為Collecting，並到對應表找到優先權值
      if (!isUserDirectPriorityCode)
			{
        var autoPickList = (from o in _createPick.F051201s
                            join d in _createPick.F051202s
                            on o.PICK_ORD_NO equals d.PICK_ORD_NO
                            where !sPickTypes.Contains(o.PICK_TYPE) && o.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString()
                            group o by d.WMS_ORD_NO into g
                            select new
                            {
                              WMS_ORD_NO = g.Key,
                              DispSystemCnt = g.Select(x => x.DISP_SYSTEM).Distinct().Count(),
                              HasArficalPick = g.Any(x => x.DISP_SYSTEM == "0"),
                              AutoPickList = g.Where(x => x.DISP_SYSTEM != "0").ToList(),
                              NpFlag = g.Any(x => x.NP_FLAG == "1")
                            })
                            .Where(x => x.HasArficalPick && x.DispSystemCnt > 1 && !x.NpFlag).SelectMany(x => x.AutoPickList).ToList();
        // 如果為非人員指派優先處理旗標且出貨單為混人工倉揀貨，會設定為f051201.PRIORITY_CODE = "Collecting"，需排除如果該出貨單有一筆揀貨單NP_FLAG=1時，不變更F051201.PRIORITY_CODE為"Collecting"
        foreach (var pick in autoPickList)
        {
          pick.PRIORITY_CODE = "Collecting";
          var f195601 = GetF195601s().FirstOrDefault(x => x.DC_CODE == pick.DC_CODE && x.PRIORITY_CODE == pick.PRIORITY_CODE && x.DEVICE_TYPE == pick.DEVICE_TYPE);
          pick.PRIORITY_VALUE = f195601 == null ? (int?)null : f195601.PRIORITY_VALUE;
        }
      }


      if (_createPick.F051301s == null)
				_createPick.F051301s = new List<F051301>();

      // 不集貨出貨單(人工倉單一揀貨、人工倉自我滿足批量揀貨單、自動倉自我滿足揀貨單)
      var notCollectPickTypes = new string[] { ((int)PickTypeEnums.FullArtificialSinglePick).ToString(), ((int)PickTypeEnums.FullArtificialSelfBatchPick).ToString(), ((int)PickTypeEnums.AutoSelfPick).ToString() };
      var notCollectWmsOrdNos = (from o in _createPick.F051201s
                                 join c in _createPick.F051202s
                                 on new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.PICK_ORD_NO } equals new { c.DC_CODE, c.GUP_CODE, c.CUST_CODE, c.PICK_ORD_NO }
                                 where notCollectPickTypes.Contains(o.PICK_TYPE)
                                 select c.WMS_ORD_NO).Distinct().ToList();


      //不集貨揀貨單單(跨庫出貨F051201.NEXT_STEP=6])
      notCollectWmsOrdNos.AddRange(crossDcShipPickOrdNos);

      //不集貨揀貨單(特殊結構揀貨單F051201.PICK_TYPE=4)
      notCollectWmsOrdNos.AddRange(specialOrderWmsNos);

      // 只有一般出貨訂單 才需判斷集貨場位置，廠退出貨單、跨庫出貨單預設集貨場為人工集貨場
      var wmsOrdDefaultCollectionPostions =
          (from o in _createPick.F051201s
           join d in _createPick.F051202s
           on new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.PICK_ORD_NO } equals new { d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.PICK_ORD_NO }
           where o.ORD_TYPE == "1" && o.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString()
           group o by d.WMS_ORD_NO into g
           select new
           {
             WMS_ORD_NO = g.Key,
             DefaultCollectionPosition = g.All(x => x.DISP_SYSTEM == "1") ? "1" : "0"
           }).ToList();

      #region 分配人工倉PDA揀貨單比例

      var atflPickTypes = new string[] { ((int)PickTypeEnums.FullArtificialSinglePick).ToString(), ((int)PickTypeEnums.FullArtificialSelfBatchPick).ToString(),
      ((int)PickTypeEnums.ArtificialBatchPick).ToString(), ((int)PickTypeEnums.SpecialOrderPick).ToString() };

      var atflPickers = _createPick.F051201s.Where(x => x.DISP_SYSTEM == "0" && atflPickTypes.Contains(x.PICK_TYPE)).ToList();
      var allotPdaCnt = ordType == "0" ? atflPickers.Count * dcShipSetting.B2B_PDA_PICK_PECENT : atflPickers.Count * dcShipSetting.B2C_PDA_PICK_PERCENT;
      allotPdaCnt = Math.Floor(allotPdaCnt);
      atflPickers.Take((int)allotPdaCnt).ToList().ForEach(x => { x.PICK_TOOL = "2"; });

      #endregion

      #region 建立揀貨批次檔[F0513]

			var f0513 = new F0513
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ORD_TYPE = ordType,
				SOURCE_TYPE = sourceType,
				CUST_COST = custCost,
				FAST_DEAL_TYPE = fastDealType,
				DELV_DATE = DateTime.Today,
				PICK_TIME = pickTime,
				PROC_FLAG = "1",
				RETAIL_QTY = ordType == "0" ?
				(short)_createPick.F050801s.Where(x => !string.IsNullOrWhiteSpace(x.RETAIL_CODE)).Select(x => x.RETAIL_CODE).Distinct().Count()
				: (short)_createPick.F05030101s.Select(x => x.ORD_NO).Distinct().Count(),
				PICK_CNT = _createPick.F051202s.Sum(x => x.B_PICK_QTY),
				SHIP_CNT = _createPick.F050801s.Count,
				ATFL_N_PICK_CNT = _createPick.F051201s.Where(x => nATFLPickTypes.Contains(x.PICK_TYPE) && x.PICK_TOOL == "1").Count(),
				ATFL_B_PICK_CNT = _createPick.F051201s.Where(x => bATFLPickTypes.Contains(x.PICK_TYPE) && x.PICK_TOOL == "1").Count(),
				ATFL_S_PICK_CNT = _createPick.F051201s.Where(x => sPickTypes.Contains(x.PICK_TYPE) && x.DISP_SYSTEM == "0" && x.PICK_TOOL == "1").Count(),
				ATFL_NP_PICK_CNT = _createPick.F051201s.Where(x => nATFLPickTypes.Contains(x.PICK_TYPE) && x.PICK_TOOL == "2").Count(),
				ATFL_BP_PICK_CNT = _createPick.F051201s.Where(x => bATFLPickTypes.Contains(x.PICK_TYPE) && x.PICK_TOOL == "2").Count(),
				ATFL_SP_PICK_CNT = _createPick.F051201s.Where(x => sPickTypes.Contains(x.PICK_TYPE) && x.DISP_SYSTEM == "0" && x.PICK_TOOL == "2").Count(),
				AUTO_N_PICK_CNT = _createPick.F051201s.Where(x => nAutoPickTypes.Contains(x.PICK_TYPE)).Count(),
				AUTO_S_PICK_CNT = _createPick.F051201s.Where(x => sPickTypes.Contains(x.PICK_TYPE) && x.DISP_SYSTEM != "0").Count(),
				REPICK_CNT = 0,
				PDA_PICK_PERCENT = ordType == "0" ? dcShipSetting.B2B_PDA_PICK_PECENT : dcShipSetting.B2B_PDA_PICK_PECENT,
				ISPRINTED = "0",
        ORDER_CRT_DATE = orderCrtDate,
        ORDER_PROC_TYPE = orderProcType
      };
			_createPick.F0513 = f0513;
			#endregion
			#region 計算周轉箱

			var isMoveOutOrder = _createPick.F0513.CUST_COST?.ToUpper() == "MOVEOUT";
			//1.	IF F190105. IS_DIRECT_CONTAINER_TYPE = 0(不指定周轉箱)
			//(1)	如果揀貨單的揀貨批次[F0513].CUST_COST!=MoveOut AND AND 派發系統=自動倉
			//  A.	設定F051201.CONTAINER_TYPE = F190105.DF_NSHIP_CONTAINER_TYPE
			//  B.	設定F051201. CONTAINER_B_CNT =1
			if (dcShipSetting.IS_DIRECT_CONTAINER_TYPE == "0")
      {
        if (!isMoveOutOrder)
        {
          foreach (var f051201 in _createPick.F051201s.Where(x => x.DISP_SYSTEM == "1"))
          {
            f051201.CONTAINER_TYPE = dcShipSetting.DF_NSHIP_CONTAINER_TYPE;
            f051201.CONTAINER_B_CNT = 1;
          }
        }
      }
      else if (dcShipSetting.IS_DIRECT_CONTAINER_TYPE == "1")
      {
        //如果是廠退單
        //(1) 設定F051201.CONTAINER_TYPE = F190105.DF_VNRSHIP_CONTAINER_TYPE
        //(2) 設定F050801.CONTAINER_TYPE = F190105.DF_VNRSHIP_CONTAINER_TYPE
        if (sourceType == "13")
        {
          foreach (var f051201 in _createPick.F051201s)
          {
            f051201.CONTAINER_TYPE = dcShipSetting.DF_VNRSHIP_CONTAINER_TYPE;
            f051201.CONTAINER_B_CNT = 1;
          }
          foreach (var f050801 in _createPick.F050801s)
          {
            f050801.CONTAINER_TYPE = dcShipSetting.DF_VNRSHIP_CONTAINER_TYPE;
            f050801.CONTAINER_B_CNT = 1;
          }
        }
        //2.	IF F190105. IS_DIRECT_CONTAINER_TYPE = 1(指定周轉箱)
        //  A.	如果揀貨單的揀貨批次[F0513].CUST_COST= OUT OR CVS 且揀貨單類型<>特殊結構訂單(4,7,8)
        //  B.	Foreach [A]  IN 所有揀貨明細 GROUP BY WMS_ORD_NO
        //  C.	[B]= 取得出貨單 WHERE WMS_ORD_NO = [A].WMS_ORD_NO
        //  D.	[C]= 取得出貨單明細 WHERE WMS_ORD_NO= [A].WMS_ORD_NO
        //  E.	[D] = 取得揀貨單 WHERE PICK_ORD_NO IN(DISTINCT [A].PICK_ORD_NO)
        //  F.	呼叫[揀貨容器共用函數]. 計算出貨單周轉箱類型(F190105,[B], [C],[D], [A])
        //(1)	如果揀貨單的揀貨批次[F0513].CUST_COST= OUT OR CVS 且揀貨單類別=特殊結構訂單(4,7,8) 
        //  A.	Foreach [A]  IN 所有揀貨單GROUP BY MERGE_NO
        //  B.	[B] =揀貨明細 WHERE PICK_ORD_NO=[A]
        //  C.	呼叫[揀貨容器共用函數]. 計算揀貨單周轉箱類型(F190105,[A],[B])
        else if (!isMoveOutOrder)
        {
          #region 一般揀貨單類型
          var normalF051202s = (from a in _createPick.F051201s.Where(x => !sPickTypes.Contains(x.PICK_TYPE))
                                join b in _createPick.F051202s on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.PICK_ORD_NO } equals new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.PICK_ORD_NO }
                                select b)
                               .GroupBy(x => x.WMS_ORD_NO);
          F050801 f050801;
          List<F050802> f050802s;
          List<F051201> f051201s;
          foreach (var f051202s in normalF051202s)
          {
            f050801 = _createPick.F050801s.First(x => x.WMS_ORD_NO == f051202s.Key);
            f050802s = _createPick.F050802s.Where(x => x.WMS_ORD_NO == f051202s.Key).ToList();
            f051201s = _createPick.F051201s.Where(x => f051202s.Select(a => a.PICK_ORD_NO).Distinct().Contains(x.PICK_ORD_NO)).ToList();
            var res = CalculateShipmentOrderTurnoverContainer(dcShipSetting, f050801, f050802s, f051201s, f051202s.ToList());
            if (!res.IsSuccessed)//如果F190105設定有少就會跳這邊
              return res;
          }
          #endregion

          #region 特殊揀貨單類型


          var specialF051202s = (from a in _createPick.F051201s.Where(x => sPickTypes.Contains(x.PICK_TYPE))
                                 join b in _createPick.F051202s on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.PICK_ORD_NO } equals new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.PICK_ORD_NO }
                                 select new { a.MERGE_NO, b })
                               .GroupBy(x => x.MERGE_NO);
          foreach (var f051202s in specialF051202s)
          {
            f051201s = _createPick.F051201s.Where(x => f051202s.Select(a => a.b.PICK_ORD_NO).Distinct().Contains(x.PICK_ORD_NO)).ToList();
            var res = CalculatePickOrderTurnoverContainer(dcShipSetting, f051201s, f051202s.Select(x=>x.b).ToList());
            if (!res.IsSuccessed)//如果F190105設定有少就會跳這邊
              return res;
          }
          #endregion

        }
      }
      #endregion 計算周轉箱

      // 設定各單據所需集貨格類型
      foreach (var wmsNo in wmsNos)
      {
        #region 建立揀貨批次明細檔[F051301]

        var nextStep = notCollectWmsOrdNos.Contains(wmsNo) ? NextStep.PackingStation : NextStep.CollectionStation;

        if (crossDcShipPickOrdNos.Contains(wmsNo))
          nextStep = NextStep.CrossAllotPier;

        var cellType = GetCargoType(dcShipSetting, dcCode, gupCode, custCode, wmsNo, sourceType == "13");

        var cargoCode = GetCargoCode(dcCode, cellType, sourceType == "13");

        var defaultCollectionPosition = wmsOrdDefaultCollectionPostions.FirstOrDefault(x => x.WMS_ORD_NO == wmsNo);

        var collectionPosition = string.Empty;
        var hasAutoCollectionStation = string.Empty;
        if (cellType == "01") //M-集貨格
          hasAutoCollectionStation = CommonService.GetSysGlobalValue(dcCode, "00", "00", "HasAutoCollectionStation_M-");
        else if (cellType == "02") //2L集貨格
          hasAutoCollectionStation = CommonService.GetSysGlobalValue(dcCode, "00", "00", "HasAutoCollectionStation_2L");


        collectionPosition = hasAutoCollectionStation == "1" && defaultCollectionPosition != null ? defaultCollectionPosition.DefaultCollectionPosition : "0";

        var f051301 = new F051301
        {
          DC_CODE = dcCode,
          GUP_CODE = gupCode,
          CUST_CODE = custCode,
          DELV_DATE = DateTime.Today,
          PICK_TIME = pickTime,
          COLLECTION_CODE = cargoCode,
          CELL_TYPE = cellType,
          STATUS = notCollectWmsOrdNos.Contains(wmsNo) ? "1" : "0",
          NEXT_STEP = ((int)nextStep).ToString(),
          WMS_NO = wmsNo,
          COLLECTION_POSITION = collectionPosition,
          NOTIFY_MODE = collectionPosition == "1" ? "0" : null
        };
        _createPick.F051301s.Add(f051301);

        #endregion 建立揀貨批次明細檔[F051301]

      }

      return new ExecuteResult(true);
    }

    /// <summary>
    /// 取得集貨場編號
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="cellType"></param
    /// <param name="IsReturn">是否為廠退單(SOURCE_TYPE=13)</param>
    /// <returns></returns>
    private string GetCargoCode(string dcCode, string cellType, Boolean IsReturn)
    {
      F1945 dcF1945;
      var dcF1945s = GetDcF1945s(dcCode, IsReturn);
      if (!dcF1945s.Any())
        return "NA";

      dcF1945 = dcF1945s.FirstOrDefault(x => x.CELL_TYPE == cellType);
      if (dcF1945 != null)
        return dcF1945.COLLECTION_CODE;
      else
        return dcF1945s.First().COLLECTION_CODE;
    }

    /// <summary>
    /// 取得集貨格類型
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <param name="IsReturn">是否為廠退單</param>
    /// <returns></returns>
    private string GetCargoType(F190105 f190105, string dcCode, string gupCode, string custCode, string wmsNo, Boolean IsReturn)
    {
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      var dcF1945s = GetDcF1945s(dcCode, IsReturn);

      //IF [A]=0 AND WMS_NO是O單=> 找F051201.MERGE_NO=WMS_NO AND F051201. DISP_SYSTEM=1(自動倉) 
      if (f190105.CELL_TYPE_METHOD == "02")
      {
        var ContainerType = "";

        if (new[] { "O", "P" }.Contains(wmsNo.Substring(0, 1)))
        {
          if (wmsNo.StartsWith("O"))
          {
            //C.	[B] = 取得F050801.CONTANER_TYPE
            ContainerType = _createPick.F050801s.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsNo)?.CONTAINER_TYPE;
            if (string.IsNullOrWhiteSpace(ContainerType) && !IsReturn)
              ContainerType = f190105.DF_NSHIP_CONTAINER_TYPE;

            if (!string.IsNullOrWhiteSpace(ContainerType))
            {
              if (dcF1945s.Select(x => x.CELL_TYPE).Contains(ContainerType))
                return ContainerType;
              else
                ContainerType = "";
            }

            if (string.IsNullOrWhiteSpace(ContainerType))
              return GetMaxVolumeCellType(dcCode, IsReturn);
          }
          else if (wmsNo.StartsWith("P"))
          {
            ContainerType = _createPick.F051201s.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PICK_ORD_NO == wmsNo)?.CONTAINER_TYPE;

            if (!string.IsNullOrWhiteSpace(ContainerType))
            {
              if (dcF1945s.Select(x => x.CELL_TYPE).Contains(ContainerType))
                return ContainerType;
              else
                ContainerType = "";
            }

            if (string.IsNullOrWhiteSpace(ContainerType))
              return GetMaxVolumeCellType(dcCode, IsReturn);
          }
        }
      }

      //如果取不到廠退集貨格，就取訂單的集貨格來計算
      if (IsReturn && (dcF1945s == null || !dcF1945s.Any()))
        dcF1945s = GetDcF1945s(dcCode);

      var cargoTypeList = dcF1945s.Select(x => x.CELL_TYPE).Distinct().ToList();
			var cargoType = string.Empty;
			var dcF194501s = GetDcF194501s(dcCode).Where(x => cargoTypeList.Contains(x.CELL_TYPE)).OrderBy(x => x.MAX_VOLUME).ToList();
			if (!dcF194501s.Any())
				return "NA"; // 沒設定固定NA

			decimal volumn = 0;
			var itemCodeList = new List<string>();
			if (wmsNo.StartsWith("O"))
			{
				var f050801 = _createPick.F050801s.First(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsNo);
				volumn = f050801.VOLUMN ?? 0;
				itemCodeList = _createPick.F050802s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsNo)
					.Select(x => x.ITEM_CODE).Distinct().ToList();
			}
			else
			{
				var itemSumQties = _createPick.F051202s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PICK_ORD_NO == wmsNo)
					.GroupBy(x => x.ITEM_CODE).Select(x => new WmsOrderItemSumQty { ItemCode = x.Key, Qty = x.Sum(y => y.B_PICK_QTY) }).ToList();
				itemCodeList = itemSumQties.Select(x => x.ItemCode).ToList();
				volumn = GetTotalItemsVolume(gupCode,custCode, itemSumQties);
			}

			//no1219如果出貨商品其中有一個商品大分類是家電(F1903.LTYPE=LC00006)，集貨場預設為L
			//no1259如果出貨商品其中有一個商品大分類是家電(F1903.LTYPE=CE)，集貨場預設為L
			if (f1903Repo.GetDatasByItems(gupCode, custCode, itemCodeList).Any(x => new string[] { "LC00006", "CE" }.Contains(x.LTYPE)))
				return "L";

			var f1905s = GetF1905s(gupCode,custCode, itemCodeList);
			var itemSizes = new List<decimal>();
			itemSizes.Add(f1905s.Max(x => x.PACK_LENGTH));
			itemSizes.Add(f1905s.Max(x => x.PACK_WIDTH));
			itemSizes.Add(f1905s.Max(x => x.PACK_HIGHT));
			var itemMaxSize = Math.Ceiling(itemSizes.Max(x => x));

			var canUseCargoTypes = dcF194501s.Where(x => x.MAX_VOLUME >= volumn);
			if (canUseCargoTypes.Any())
			{
				foreach (var canUseCargoType in canUseCargoTypes)
				{
					if (canUseCargoType.LENGTH >= itemMaxSize || canUseCargoType.DEPTH >= itemMaxSize || canUseCargoType.HEIGHT >= itemMaxSize)
					{
						cargoType = canUseCargoType.CELL_TYPE;
						break;
					}
				}
			}
			// 如果找不到就給最大容積的格子大小
			if (string.IsNullOrEmpty(cargoType))
				cargoType = dcF194501s.OrderByDescending(x => x.MAX_VOLUME).First().CELL_TYPE;

			return cargoType;
		}

    /// <summary>
    /// 取得最大容積的集貨格
    /// </summary>
    /// <returns></returns>
    private string GetMaxVolumeCellType(string dcCode, Boolean IsReturn)
    {
      var dcF1945s = GetDcF1945s(dcCode, IsReturn);
      var dcF1945CellTypes = dcF1945s.Select(a => a.CELL_TYPE).Distinct();
      var res = GetDcF194501s(dcCode)
        .Where(x => dcF1945CellTypes.Contains(x.CELL_TYPE))
        .OrderByDescending(x => x.MAX_VOLUME)
        .FirstOrDefault().CELL_TYPE;
      return string.IsNullOrWhiteSpace(res) ? "NA" : res;
    }

    #endregion



    #endregion

    #region 批次刪除F050306
    private void BatchDeleteF050306(List<Int64> ids)
		{
			if (!ids.Any())
				return;

			var f050306Repo = new F050306Repository(Schemas.CoreSchema, _wmsTransaction);
			var batchMaxCount = 1000;
			var batchIdList = new List<List<Int64>>();
			if (ids.Count > batchMaxCount)
			{
				var pages = ids.Count / batchMaxCount + (ids.Count % batchMaxCount > 0 ? 1 : 0);
				for (var page = 0; page < pages; page++)
					batchIdList.Add(ids.Skip(page * batchMaxCount).Take(batchMaxCount).ToList());
			}
			else
				batchIdList.Add(ids);

			foreach (var batchIds in batchIdList)
			{
				f050306Repo.DeleteByIds(batchIds);
			}
		}

		#endregion

		#region 批次刪除F05120601
		private void BatchDeletF05120601(List<Int64> ids)
		{
			if (!ids.Any())
				return;

			var f05120601Repo = new F05120601Repository(Schemas.CoreSchema, _wmsTransaction);
			var batchMaxCount = 1000;
			var batchIdList = new List<List<Int64>>();
			if (ids.Count > batchMaxCount)
			{
				var pages = ids.Count / batchMaxCount + (ids.Count % batchMaxCount > 0 ? 1 : 0);
				for (var page = 0; page < pages; page++)
					batchIdList.Add(ids.Skip(page * batchMaxCount).Take(batchMaxCount).ToList());
			}
			else
				batchIdList.Add(ids);

			foreach (var batchIds in batchIdList)
			{
				f05120601Repo.DeleteByIds(batchIds);
			}
		}

		#endregion

		#region 產生揀缺資料

		/// <summary>
		/// 產生揀缺單
		/// </summary>
		/// <param name="f050306s"></param>
		/// <returns></returns>
		private ExecuteResult CreateLackPick(List<F050306> f050306s)
		{
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060201Repo = new F060201Repository(Schemas.CoreSchema, _wmsTransaction);
      _createPick = new CreatePick();
			var updF0513List = new List<F0513>();
			_wmsLogHelper.AddRecord("取得揀缺的揀貨單資料 開始");

			var groupDcPickTimes = (from o in f050306s
															group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.WMS_NO, o.ORD_TYPE, o.SOURCE_TYPE, o.CUST_COST, o.FAST_DEAL_TYPE } into groupPick
															let f051201 = GetF051201(groupPick.Key.DC_CODE, groupPick.Key.GUP_CODE, groupPick.Key.CUST_CODE, groupPick.Key.WMS_NO)
															orderby groupPick.Key.FAST_DEAL_TYPE
															select new { groupPick, f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.DELV_DATE, f051201.PICK_TIME, f051201.PICK_ORD_NO, f051201.PICK_TYPE, f051201.PICK_TOOL, f051201.SPLIT_TYPE })
						.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.DELV_DATE, x.PICK_TIME });
			_wmsLogHelper.AddRecord("取得揀缺的揀貨單資料 結束");

			var specialPickType = ((int)PickTypeEnums.SpecialOrderPick).ToString();

			foreach (var dcPickTimes in groupDcPickTimes)
			{
				_wmsLogHelper.AddRecord("取得物流中心出貨指示設定資料 開始");
				var dcShipSetting = Get190105(dcPickTimes.Key.DC_CODE);
				_wmsLogHelper.AddRecord("取得物流中心出貨指示設定資料 結束");
				// 暫存揀貨類型配庫後揀貨資料
				_tempPickTypeList = new Dictionary<PickTypeEnums, List<F050306>>();

				_wmsLogHelper.AddRecord("分配揀貨單 開始");
				foreach (var dcPick in dcPickTimes)
				{
          
          // 如果原本揀貨單為特殊結構揀貨單，則新揀貨單也需為特殊結構揀貨單不可為揀缺單
          if (dcPick.PICK_TYPE == specialPickType)
					{
						if (_tempPickTypeList.ContainsKey(PickTypeEnums.SpecialOrderPick))
							_tempPickTypeList[PickTypeEnums.SpecialOrderPick].AddRange(dcPick.groupPick.ToList());
						else
							_tempPickTypeList.Add(PickTypeEnums.SpecialOrderPick, dcPick.groupPick.ToList());
					}
					else
					{
						//原揀貨單為PDA揀貨且尚未完成揀貨且本次配庫後揀貨資料都為人工倉，則合併至原揀貨單否則為新揀缺單
						if (dcPick.PICK_TOOL == "2" && dcPick.groupPick.Any(x => x.DEVICE_TYPE == "0"))
						{
							var autoPicks = dcPick.groupPick.Where(x => x.DEVICE_TYPE != "0").ToList();
							var atflPicks = dcPick.groupPick.Where(x => x.DEVICE_TYPE == "0").ToList();
							var pdaNoPickFinishCount = 3; //PDA剩餘未揀貨明細筆數(超過N筆，就合併到原揀貨單)
							var isAddOrgPickDetail = false;

              // 找出揀貨單明細
              var f051202s = GetF051202s(dcPick.DC_CODE, dcPick.GUP_CODE, dcPick.CUST_CODE, dcPick.PICK_ORD_NO);
							// 找出揀貨單總揀明細資料
							var f051203s = GetF051203s(dcPick.DC_CODE, dcPick.GUP_CODE, dcPick.CUST_CODE, dcPick.PICK_ORD_NO);

							// 單一揀貨 未揀明細>=PDA剩餘未揀貨明細筆數 或 批量揀貨 總揀未揀明細>=PDA剩餘未揀貨明細筆數
							if ((dcPick.SPLIT_TYPE == "03" && f051202s.Count(x => x.PICK_STATUS == "0") >= pdaNoPickFinishCount) || (dcPick.SPLIT_TYPE != "03" && f051203s.Count(x => x.PICK_STATUS == "0") >= pdaNoPickFinishCount))
							{
								_wmsLogHelper.AddRecord(string.Format("原揀貨單{0}為PDA揀貨且未揀明細數大於等於{1}筆，寫入原揀貨單明細 開始", dcPick.PICK_ORD_NO, pdaNoPickFinishCount));
								isAddOrgPickDetail = true;
								//直接寫入原PDA明細(用另一個transation處理，一張揀貨單commit一次)
								var wmsTransation2 = new WmsTransaction();
								var f051202Repo2 = new F051202Repository(Schemas.CoreSchema, wmsTransation2);
								var f051203Repo2 = new F051203Repository(Schemas.CoreSchema, wmsTransation2);
								var f050306Repo2 = new F050306Repository(Schemas.CoreSchema, wmsTransation2);
								var f1511Repo2 = new F1511Repository(Schemas.CoreSchema, wmsTransation2);
								var f05120601Repo2 = new F05120601Repository(Schemas.CoreSchema, wmsTransation2);
								// 產生新的揀貨明細
								var seq = int.Parse(f051202s.OrderByDescending(x => x.PICK_ORD_SEQ).Select(x => x.PICK_ORD_SEQ).First()) + 1;
								var routeSeq = (f051202s.Max(x => x.ROUTE_SEQ) ?? 0) + 1;
								var addF051202List = atflPicks.Select((x, index) => new F051202
								{
									DC_CODE = x.DC_CODE,
									GUP_CODE = x.GUP_CODE,
									CUST_CODE = x.CUST_CODE,
									PICK_ORD_NO = x.WMS_NO,
									PICK_ORD_SEQ = (seq + index).ToString().PadLeft(4, '0'),
									ITEM_CODE = x.ITEM_CODE,
									PICK_LOC = x.PICK_LOC,
									ENTER_DATE = x.ENTER_DATE,
									MAKE_NO = x.MAKE_NO,
									SERIAL_NO = x.SERIAL_NO == "0" ? string.Empty : x.SERIAL_NO,
									VNR_CODE = x.VNR_CODE,
									BOX_CTRL_NO = x.BOX_CTRL_NO,
									PALLET_CTRL_NO = x.PALLET_CTRL_NO,
									WMS_ORD_NO = x.WMS_ORD_NO,
									WMS_ORD_SEQ = x.WMS_ORD_SEQ,
									B_PICK_QTY = x.B_PICK_QTY,
									VALID_DATE = x.VALID_DATE,
									A_PICK_QTY = 0,
									PICK_STATUS = "0",
									ROUTE_SEQ = routeSeq,
                  PK_AREA = x.PK_AREA,
                  PK_AREA_NAME = x.PK_AREA_NAME
                }).ToList();
								f051202Repo2.BulkInsert(addF051202List);
								// 產生虛擬庫存資料
								var addF1511List = addF051202List.Select(x => new F1511
								{
									DC_CODE = x.DC_CODE,
									GUP_CODE = x.GUP_CODE,
									CUST_CODE = x.CUST_CODE,
									ORDER_NO = x.PICK_ORD_NO,
									ORDER_SEQ = x.PICK_ORD_SEQ,
									LOC_CODE = x.PICK_LOC,
									ITEM_CODE = x.ITEM_CODE,
									VALID_DATE = x.VALID_DATE,
									ENTER_DATE = x.ENTER_DATE,
									MAKE_NO = x.MAKE_NO,
									SERIAL_NO = string.IsNullOrWhiteSpace(x.SERIAL_NO) ? "0" : x.SERIAL_NO,
									BOX_CTRL_NO = x.BOX_CTRL_NO,
									PALLET_CTRL_NO = x.PALLET_CTRL_NO,
									STATUS = "0",
									B_PICK_QTY = x.B_PICK_QTY,
									A_PICK_QTY = 0
								});
								f1511Repo2.BulkInsert(addF1511List);
								// 非單一揀貨，將明細加總寫入總揀明細
								if (dcPick.SPLIT_TYPE != "03")
								{
									// 產生新的彙總揀貨明細
									var batchSeq = int.Parse(f051203s.OrderByDescending(x => x.TTL_PICK_SEQ).Select(x => x.TTL_PICK_SEQ).First()) + 1;
									var routeBatchSeq = (f051203s.Max(x => x.ROUTE_SEQ) ?? 0) + 1;
									var addF051203s = addF051202List.GroupBy(x => new
									{
										x.DC_CODE,
										x.GUP_CODE,
										x.CUST_CODE,
										x.PICK_ORD_NO,
										x.PICK_LOC,
										x.ITEM_CODE,
										x.VALID_DATE,
										x.MAKE_NO,
										x.SERIAL_NO
									})
										.Select((x, index) => new F051203
										{
											DC_CODE = x.Key.DC_CODE,
											GUP_CODE = x.Key.GUP_CODE,
											CUST_CODE = x.Key.CUST_CODE,
											PICK_ORD_NO = x.Key.PICK_ORD_NO,
											TTL_PICK_SEQ = (batchSeq + index).ToString().PadLeft(4, '0'),
											PICK_LOC = x.Key.PICK_LOC,
											ITEM_CODE = x.Key.ITEM_CODE,
											MAKE_NO = x.Key.MAKE_NO,
											SERIAL_NO = x.Key.SERIAL_NO == "0" ? string.Empty : x.Key.SERIAL_NO,
											VALID_DATE = x.Key.VALID_DATE,
											B_PICK_QTY = x.Sum(y => y.B_PICK_QTY),
											A_PICK_QTY = 0,
											PICK_STATUS = "0",
											ROUTE_SEQ = routeBatchSeq
										}).ToList();
									f051203Repo2.BulkInsert(addF051203s);
								}
								f050306Repo2.DeleteByIds(atflPicks.Select(x => x.ID).ToList());
								f05120601Repo2.DeleteByIds(atflPicks.Select(x => x.LACK_ID.Value).ToList());

								wmsTransation2.Complete();
								_wmsLogHelper.AddRecord(string.Format("原揀貨單{0}為PDA揀貨且未揀明細數大於等於{1}筆，寫入原揀貨單明細 結束", dcPick.PICK_ORD_NO, pdaNoPickFinishCount));
							}

							if (isAddOrgPickDetail && autoPicks.Any())
							{
								// 配庫後為自動倉直接產生揀缺單
								if (_tempPickTypeList.ContainsKey(PickTypeEnums.LackPick))
									_tempPickTypeList[PickTypeEnums.LackPick].AddRange(autoPicks);
								else
									_tempPickTypeList.Add(PickTypeEnums.LackPick, autoPicks);

								continue;
							}
							if (isAddOrgPickDetail)
								continue;
						}

						// 原本非特殊結構訂單或PDA已完成揀貨直接產生揀缺單
						if (_tempPickTypeList.ContainsKey(PickTypeEnums.LackPick))
							_tempPickTypeList[PickTypeEnums.LackPick].AddRange(dcPick.groupPick.ToList());
						else
							_tempPickTypeList.Add(PickTypeEnums.LackPick, dcPick.groupPick.ToList());
					}
				}
				_wmsLogHelper.AddRecord("分配揀貨單 結束");
				if (!_tempPickTypeList.Any())
					_wmsLogHelper.AddRecord("無資料需產生補揀單");
				else
				{
					_wmsLogHelper.AddRecord("建立快速補揀單、特殊結構揀貨單 開始");
					// 建立快速補揀單、特殊結構揀貨單
					var createPickResult = CreatePickOrders(dcShipSetting, dcPickTimes.Key.DELV_DATE.Value, dcPickTimes.Key.PICK_TIME);
					if (!createPickResult.IsSuccessed)
						return createPickResult;

					_wmsLogHelper.AddRecord("建立快速補揀單、特殊結構揀貨單 結束");
					_wmsLogHelper.AddRecord("更新揀貨批次統計數 開始");
					// 更新揀貨批次統計數
					var f0513 = f0513Repo.Find(x => x.DC_CODE == dcPickTimes.Key.DC_CODE && x.GUP_CODE == dcPickTimes.Key.GUP_CODE && x.CUST_CODE == dcPickTimes.Key.CUST_CODE
																				&& x.DELV_DATE == dcPickTimes.Key.DELV_DATE.Value && x.PICK_TIME == dcPickTimes.Key.PICK_TIME);

					var pickers = _createPick.F051201s.Where(x => x.DC_CODE == dcPickTimes.Key.DC_CODE && x.GUP_CODE == dcPickTimes.Key.GUP_CODE && x.CUST_CODE == dcPickTimes.Key.CUST_CODE
					 && x.DELV_DATE == dcPickTimes.Key.DELV_DATE.Value && x.PICK_TIME == dcPickTimes.Key.PICK_TIME).ToList();

					#region 分配人工倉PDA揀貨單比例

					var atflPickers = pickers.Where(x => x.DISP_SYSTEM == "0").ToList();
					var allotPdaCnt = atflPickers.Count() * f0513.PDA_PICK_PERCENT;
					allotPdaCnt = Math.Floor(allotPdaCnt);
					atflPickers.Take((int)allotPdaCnt).ToList().ForEach(x => { x.PICK_TOOL = "2"; });

					#endregion

					f0513.REPICK_CNT += pickers.Where(x => x.PICK_TYPE == ((int)PickTypeEnums.LackPick).ToString()).Count();
					f0513.ATFL_S_PICK_CNT += pickers.Where(x => x.PICK_TYPE == ((int)PickTypeEnums.SpecialOrderPick).ToString() && x.DISP_SYSTEM == "0" && x.PICK_TOOL == "1").Count();
					f0513.ATFL_SP_PICK_CNT += pickers.Where(x => x.PICK_TYPE == ((int)PickTypeEnums.SpecialOrderPick).ToString() && x.DISP_SYSTEM == "0" && x.PICK_TOOL == "2").Count();
					f0513.AUTO_S_PICK_CNT += pickers.Where(x => x.PICK_TYPE == ((int)PickTypeEnums.SpecialOrderPick).ToString() && x.DISP_SYSTEM != "0").Count();
					updF0513List.Add(f0513);
					_wmsLogHelper.AddRecord("更新揀貨批次統計數 結束");

        }

      }

      _wmsLogHelper.AddRecord("整批異動資料庫 開始");
			// 整批寫入資料庫
			if (_createPick.F051201s != null && _createPick.F051201s.Any())
				f051201Repo.BulkInsert(_createPick.F051201s);
			if (_createPick.F051202s != null && _createPick.F051202s.Any())
				f051202Repo.BulkInsert(_createPick.F051202s);
			if (_createPick.F051203s != null && _createPick.F051203s.Any())
				f051203Repo.BulkInsert(_createPick.F051203s);
			if (_createPick.F1511s != null && _createPick.F1511s.Any())
				f1511Repo.BulkInsert(_createPick.F1511s);
			if (_createPick.F060201s != null && _createPick.F060201s.Any())
				f060201Repo.BulkInsert(_createPick.F060201s);
			if (updF0513List.Any())
				f0513Repo.BulkUpdate(updF0513List);
      if (_createPick.F050306_HISTORYs != null && _createPick.F050306_HISTORYs.Any())
      {
        var f050306HistoryRepo = new F050306_HISTORYRepository(Schemas.CoreSchema, _wmsTransaction);
        f050306HistoryRepo.BulkInsert(_createPick.F050306_HISTORYs, "ID");
      }
      BatchDeleteF050306(f050306s.Select(x => x.ID).ToList());
			BatchDeletF05120601(f050306s.Select(x => x.LACK_ID.Value).ToList());
			_wmsTransaction.Complete();
			_wmsLogHelper.AddRecord("整批異動資料庫 結束");
			return new ExecuteResult(true);
		}

		#endregion

		#endregion

		#region 自動揀缺配庫排程

		/// <summary>
		/// 自動揀缺配庫排程
		/// </summary>
		/// <returns></returns>
		public ApiResult AutoPickLackAllotStock()
		{
			return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSSH_PLS, "0", "0", "0", "AutoPickLackAllotStock", new object { }, () =>
			{
				var result = PickLackAllotStock();
				return new ApiResult { IsSuccessed = result.IsSuccessed, MsgCode = "", MsgContent = result.Message };
			}, true);
		}


		#endregion

		#region 揀缺配庫共用方法

		/// <summary>
		/// 揀缺配庫主流程
		/// </summary>
		/// <returns></returns>
		public ExecuteResult PickLackAllotStock()
		{
			_wmsLogHelper = new WmsLogHelper();
			_wmsLogHelper.StartRecord(WmsLogProcType.PickLackAllotStock);
			var f05120601Repo = new F05120601Repository(Schemas.CoreSchema);
			var f05120601s = new List<F05120601>();
			var checkStatus = false;
			string allotBatchNo=string.Empty;
			try
			{
				_wmsLogHelper.AddRecord("更新揀缺待配庫資料狀態從待扣庫(0)變更為配庫處理中(1) 開始");
				f05120601Repo.UpdateNotAllotStatus("0", "1");
				_wmsLogHelper.AddRecord("更新揀缺待配庫資料狀態從待扣庫(0)變更為配庫處理中(1) 結束");

				_wmsLogHelper.AddRecord("取得揀缺待配庫資料 開始");
				f05120601s = f05120601Repo.GetPickLackAllotDatas().ToList();
				_wmsLogHelper.AddRecord("取得揀缺待配庫資料 結束");

				if (!f05120601s.Any())
				{
					var msg = "無揀缺待配庫資料";
					_wmsLogHelper.AddRecord(msg);
					_wmsLogHelper.StopRecord();
					return new ExecuteResult(false, msg);
				}

				var itemCodes = f05120601s.Select(x => new ItemKey { DcCode = x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
				allotBatchNo = "BP" + DateTime.Now.ToString("yyyyMMddHHmmss");

				_wmsLogHelper.AddRecord("檢查配庫狀態開始");
				checkStatus = StockService.CheckAllotStockStatus(true, allotBatchNo,itemCodes);
				_wmsLogHelper.AddRecord("檢查配庫狀態結束");

				if (!checkStatus)
				{
					var msg = "仍有程序正在配庫揀貨單所配庫商品，請稍待再配庫";
					_wmsLogHelper.AddRecord(msg);
					_wmsLogHelper.StopRecord();
					return new ExecuteResult(false, msg);
				}
				_wmsLogHelper.AddRecord("已更改配庫商品狀態為鎖定，配庫批次號" + allotBatchNo);

				//揀缺配庫處理
				var result = PickLackAllotStockProcess(f05120601s);
				if (!result.IsSuccessed)
					return result;

				_wmsLogHelper.AddRecord("執行db commit 開始");
				_wmsTransaction.Complete();
				_wmsLogHelper.AddRecord("執行db commit 結束");
			}
			finally
			{
				if (f05120601s.Any())
				{
					_wmsLogHelper.AddRecord("更新揀缺待配庫資料狀態從配庫處理中(1)變更為待扣庫(0) 開始");
					f05120601Repo.UpdateNotAllotStatus("1", "0");
					_wmsLogHelper.AddRecord("更新揀缺待配庫資料狀態從配庫處理中(1)變更為待扣庫(0) 結束");
					if (checkStatus)
					{
						_wmsLogHelper.AddRecord("將配庫狀態改回待配庫 開始");
						// 更改配庫狀態為未配庫
						StockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
						_wmsLogHelper.AddRecord("將配庫狀態改回待配庫 結束");
					}
					else
						_wmsLogHelper.AddRecord("仍有程序在配庫中，配庫狀態不須調整");
				}
			}

			_wmsLogHelper.StopRecord();
			return new ExecuteResult(true);
		}

    public string GetPickLossWarehouseId(string dcCode, string gupCode, string custCode)
    {
      var f0003SYS_PATH = CommonService.GetSysGlobalValue(dcCode, gupCode, custCode, "PickLossWHId");
      return string.IsNullOrWhiteSpace(f0003SYS_PATH) ? string.Empty : f0003SYS_PATH;
    }

    private List<F1912> _f1912List;
		public string GetPickLossLoc(string dcCode, string warehouseId)
		{
			if (_f1912List == null)
				_f1912List = new List<F1912>();
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var f1912 = _f1912List.FirstOrDefault(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId);
			if (f1912 == null)
			{
				f1912 = f1912Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.WAREHOUSE_ID == warehouseId).FirstOrDefault();
				if (f1912 != null)
					_f1912List.Add(f1912);
			}
			return f1912 == null ? string.Empty : f1912.LOC_CODE;
		}

		/// <summary>
		/// 揀缺配庫處理
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNo"></param>
		/// <param name="sourceType"></param>
		/// <param name="custCost"></param>
		/// <param name="f05120601s"></param>
		private ExecuteResult PickLackAllotStockProcess(List<F05120601> f05120601s)
		{
			_wmsLogHelper.AddRecord("揀缺配庫處理 開始");
      var f050302Repo = new F050302Repository(Schemas.CoreSchema);
			var f050306Repo = new F050306Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051206Repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05120601Repo = new F05120601Repository(Schemas.CoreSchema, _wmsTransaction);
			var f191302Repo = new F191302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
      var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema);
      var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f060702Repo = new F060702Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060202Repo = new F060202Repository(Schemas.CoreSchema);

			var shardService = new SharedService(_wmsTransaction);
      var addF060702List = new List<F060702>();
      var addF050306List = new List<F050306>();
			var addF051206List = new List<F051206>();
			var updF05120601List = new List<F05120601>();
			var delF05120601List = new List<F05120601>();
			var addF191302List = new List<F191302>();
      var groupDcs = f05120601s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE }).ToList();
      var returnStocks = new List<F1913>();
			var returnAllotList = new List<ReturnNewAllocation>();
			var updF051202List = new List<F051202>();
			var updF1511List = new List<F1511>();
      var addF050802List = new List<F050802>();
      var updF050802List = new List<F050802>();
      var addF050302List = new List<F050302>();
      var updF050302List = new List<F050302>();

      var wmsOrdNos = f05120601s.Select(a => a.WMS_ORD_NO).ToList();
			var wmsOrdStatuses = f050801Repo.GetWmsOrdStatuses(wmsOrdNos).ToList();
      
			foreach (var groupDc in groupDcs)
			{
        _pickLackF050802s = f050802Repo.GetDatas(groupDc.Key.DC_CODE, groupDc.Key.GUP_CODE, groupDc.Key.CUST_CODE, groupDc.Select(x => x.WMS_ORD_NO).Distinct().ToList()).ToList();
        var allF1903s = CommonService.GetProductList(groupDc.Key.GUP_CODE, groupDc.Key.CUST_CODE, groupDc.Select(x => x.ITEM_CODE).Distinct().ToList());

        var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);
        var autoWarehouseList = f1980Repo.GetAutoWarehouseList(groupDc.Key.DC_CODE).ToList();

        _wmsLogHelper.AddRecord(string.Format("揀缺配庫處理-DcCode={0} 開始", groupDc.Key));

				_wmsLogHelper.AddRecord("取得疑似遺失倉-倉庫編號 開始");
        var pickLossWHId = GetPickLossWarehouseId(groupDc.Key.DC_CODE, groupDc.Key.GUP_CODE, groupDc.Key.CUST_CODE);
				if (string.IsNullOrWhiteSpace(pickLossWHId))
				{
					var msg = string.Format(Properties.Resources.PickLossWHIdNotSetting, groupDc.Key);
					_wmsLogHelper.AddRecord(msg);
					return new ExecuteResult(false, msg);
				}
				_wmsLogHelper.AddRecord("取得疑似遺失倉-倉庫編號 結束");

				_wmsLogHelper.AddRecord("取得疑似遺失倉-儲位編號 開始");
				// 疑似遺失倉第一個儲位
				var pickLossLocCode = GetPickLossLoc(groupDc.Key.DC_CODE, pickLossWHId);
				_wmsLogHelper.AddRecord("取得疑似遺失倉-儲位編號 結束");

				foreach (var f05120601 in groupDc)
				{
          var IsBundleLoc = allF1903s.First(x => x.ITEM_CODE == f05120601.ITEM_CODE).BUNDLE_SERIALLOC == "1";

          //抓揀貨單頭檔判斷是否為自動倉用
          var getPickHade = f051201Repo.Find(x => x.DC_CODE == f05120601.DC_CODE && x.GUP_CODE == f05120601.GUP_CODE && x.CUST_CODE == f05120601.CUST_CODE && x.PICK_ORD_NO == f05120601.PICK_ORD_NO);

          //集貨等待通知寫入狀態 null:不通知 其餘比照F060702.STATUS 9999只是暫時的內容
          int? f060702Status = null;
          if (getPickHade.DISP_SYSTEM == "1" &&
              getPickHade.ORD_TYPE == "1" &&
              getPickHade.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString())
            f060702Status = 9999;
          else
            f060702Status = null;

          var wmsOrdStatus = wmsOrdStatuses.FirstOrDefault(a => a.DcCode == f05120601.DC_CODE && a.WmsOrdNo == f05120601.WMS_ORD_NO);
          if (wmsOrdStatus != null)
					{
						if (wmsOrdStatus.Status == 9)
						{
							f05120601.STATUS = "9";
						}
					}


          if (!string.IsNullOrWhiteSpace(f05120601.SERIAL_NO) && f05120601.STATUS == "1" && IsBundleLoc)
          {
            //檢查原始訂單有沒有指定序號，有的話就直接撿缺
            var f050302 = f050302Repo.GetDataByF05120601SerialNo(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, f05120601.WMS_ORD_NO, f05120601.SERIAL_NO);
            if (f050302 != null)
              F05120601Status3Process(f05120601, ref addF051206List, ref f060702Status);
          }

          if (f05120601.STATUS == "1")
          {
            var curf050802 = _pickLackF050802s.Where(x => x.WMS_ORD_NO == f05120601.WMS_ORD_NO && x.SERIAL_NO == f05120601.SERIAL_NO).First();

            var itemStocks = GetItemPickStocks(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, f05120601.ITEM_CODE);

            // 檢查是否為自動倉
            var loc = CommonService.GetLoc(f05120601.DC_CODE, f05120601.PICK_LOC);
            if (!autoWarehouseList.Any(x => x.WAREHOUSE_ID == loc.WAREHOUSE_ID))
            {
              var itemLackLocs = groupDc.Where(x => x.ITEM_CODE == f05120601.ITEM_CODE).Select(x => x.PICK_LOC).Distinct().ToList();
              // 庫存排除該商品原揀貨不足儲位
              itemStocks = itemStocks.Where(x => !itemLackLocs.Contains(x.LOC_CODE)).ToList();
            }
            else
            {
              // 自動倉只有一個儲位，所以依照儲位+效期+入庫日+批號進行庫存排除
              var itemLacks = groupDc.Where(x => x.ITEM_CODE == f05120601.ITEM_CODE).GroupBy(x => new { x.PICK_LOC, x.VALID_DATE, x.ENTER_DATE, x.MAKE_NO }).ToList();
              foreach (var itemLack in itemLacks)
              {
                itemStocks = itemStocks.Except(itemStocks.Where(x => x.LOC_CODE == itemLack.Key.PICK_LOC && x.VALID_DATE == itemLack.Key.VALID_DATE && x.ENTER_DATE == itemLack.Key.ENTER_DATE && x.MAKE_NO == itemLack.Key.MAKE_NO).ToList()).ToList();
              }
            }


            _wmsLogHelper.AddRecord("分配揀區庫存 開始");
            var itemSumQty = itemStocks.Sum(x => x.QTY);
            // 庫存足夠
            if (itemSumQty >= f05120601.LACK_QTY)
            {
              _wmsLogHelper.AddRecord("庫存足夠-產生配庫後揀貨資料 開始");
              var needQty = f05120601.LACK_QTY;
              foreach (var itemStock in itemStocks)
              {
                if (itemStock.QTY >= needQty)
                {
                  // 產生配庫後揀貨資料
                  addF050306List.Add(CreateF050306(itemStock, f05120601, needQty));
                  itemStock.QTY -= needQty;
                  needQty = 0;

                  if (IsBundleLoc)
                    UpdateBundleSerialNoF050802(f05120601, itemStock, curf050802, ref addF050802List, ref updF050802List, ref addF050302List, ref updF050302List);

                  break;
                }
                else
                {
                  // 產生配庫後揀貨資料
                  addF050306List.Add(CreateF050306(itemStock, f05120601, (int)itemStock.QTY));
                  needQty -= (int)itemStock.QTY;
                  itemStock.QTY = 0;

                  if (IsBundleLoc)
                    UpdateBundleSerialNoF050802(f05120601, itemStock, curf050802, ref addF050802List, ref updF050802List, ref addF050302List, ref updF050302List);

                }
              }

              if (f060702Status != null)
              {
                var curF050306 = addF050306List.Where(x => x.DC_CODE == f05120601.DC_CODE && x.GUP_CODE == f05120601.GUP_CODE && x.CUST_CODE == f05120601.CUST_CODE && x.WMS_NO == f05120601.PICK_ORD_NO).ToList();
                //檢查產生的揀貨任務是否有人工倉有的話就發異常
                if (curF050306.Any(x => x.DEVICE_TYPE == "0"))
                  f060702Status = 2;
                else if (curF050306.All(x => x.DEVICE_TYPE != "0"))
                  f060702Status = 1;
                else
                  f060702Status = 2;
              }

              f05120601.STATUS = "2"; //已扣庫

              _wmsLogHelper.AddRecord("庫存足夠-產生配庫後揀貨資料 結束");
            }
            //庫存不足
            else
            {
              _wmsLogHelper.AddRecord("庫存不足-產生揀缺資料 開始");
              F05120601Status3Process(f05120601, ref addF051206List, ref f060702Status);
              _wmsLogHelper.AddRecord("庫存不足-產生揀缺資料 結束");
            }
          }

					switch (f05120601.STATUS)
					{
						case "2":
							updF05120601List.Add(f05120601);
							break;
						case "3":
						case "9":
              delF05120601List.Add(f05120601);
							break;
					}
          PickLackCollectionStatusProcess(f060702Repo, ref addF060702List, f060702Status, f05120601);

          _wmsLogHelper.AddRecord("分配揀區庫存 結束");
				}

				// 庫存足夠或取消-將揀缺庫存調至疑似遺失倉並產生庫存異常明細
				var enoughStockLackDatas = groupDc.Where(x => x.STATUS == "2" || x.STATUS == "9").ToList();
				if (enoughStockLackDatas.Any())
				{
					_wmsLogHelper.AddRecord("庫存足夠或取消-將揀缺進行庫存異常處理 開始");

					foreach (var detail in enoughStockLackDatas)
					{
						var f051202 = f051202Repo.Find(x => x.DC_CODE == detail.DC_CODE && x.GUP_CODE == detail.GUP_CODE && x.CUST_CODE == detail.CUST_CODE && x.PICK_ORD_NO == detail.PICK_ORD_NO && x.PICK_ORD_SEQ == detail.PICK_ORD_SEQ);
						var f1511 = f1511Repo.Find(x => x.DC_CODE == detail.DC_CODE && x.GUP_CODE == detail.GUP_CODE && x.CUST_CODE == detail.CUST_CODE && x.ORDER_NO == detail.PICK_ORD_NO && x.ORDER_SEQ == detail.PICK_ORD_SEQ);
						var stockLack = new StockLack
						{
							DcCode = detail.DC_CODE,
							GupCode = detail.GUP_CODE,
							CustCode = detail.CUST_CODE,
							LackQty = detail.LACK_QTY,
							PickLackWarehouseId = pickLossWHId,
							PickLackLocCode = pickLossLocCode,
							F051202 = f051202,
							F1511 = f1511,
							ReturnStocks = returnStocks
						};
						var result = CreateStockLackProcess(stockLack);
						if (!result.IsSuccessed)
							return new ExecuteResult(result.IsSuccessed, result.Message);

						returnStocks = result.ReturnStocks;
						returnAllotList.AddRange(result.ReturnNewAllocations);
						updF051202List.Add(result.UpdF051202);
						updF1511List.Add(result.UpdF1511);
						addF191302List.AddRange(result.AddF191302List);
					}
					_wmsLogHelper.AddRecord("庫存足夠或取消-將揀缺進行庫存異常處理 結束");
				}
				_wmsLogHelper.AddRecord(string.Format("揀缺配庫處理-DcCode={0} 結束", groupDc.Key));
      }

      _wmsLogHelper.AddRecord("調撥單整批上架 開始");
			// 調撥單整批上架
			shardService.BulkAllocationToAllUp(returnAllotList, returnStocks, false, addF191302List);
			_wmsLogHelper.AddRecord("調撥單整批上架 結束");

			_wmsLogHelper.AddRecord("調撥單整批寫入 開始");
			shardService.BulkInsertAllocation(returnAllotList, returnStocks, true);
			_wmsLogHelper.AddRecord("調撥單整批寫入 結束");

			// 扣除庫存
			_wmsLogHelper.AddRecord("扣除庫存 開始");
			// 扣除庫存
			var checkStock = StockService.DeductStock(addF050306List.Select(x => new OrderStockChange
			{
				DcCode = x.DC_CODE,
				GupCode = x.GUP_CODE,
				CustCode = x.CUST_CODE,
				WmsNo = x.WMS_NO,
				LocCode = x.PICK_LOC,
				ItemCode = x.ITEM_CODE,
				VaildDate = x.VALID_DATE,
				EnterDate = x.ENTER_DATE,
				MakeNo = x.MAKE_NO,
				SerialNo = x.SERIAL_NO,
				BoxCtrlNo = x.BOX_CTRL_NO,
				PalletCtrlNo = x.PALLET_CTRL_NO,
				VnrCode = x.VNR_CODE,
				Qty = x.B_PICK_QTY
			}).ToList());

			if (!checkStock.IsSuccessed)
			{
				var msgList = checkStock.Message.Split(Environment.NewLine.ToArray()).ToList();
				msgList.ForEach(msg =>
				{
					_wmsLogHelper.AddRecord(msg);
				});
				_wmsLogHelper.AddRecord("扣除庫存 結束(庫存不足)");
				_wmsLogHelper.AddRecord("配庫處理結束");
				return checkStock;
			}
			else
				_wmsLogHelper.AddRecord("扣除庫存 結束");

			StockService.SaveChange();

      if (addF060702List.Any())
        f060702Repo.BulkInsert(addF060702List);
      f051202Repo.BulkUpdate(updF051202List);
			f1511Repo.BulkUpdate(updF1511List);
			f050306Repo.BulkInsert(addF050306List, "ID");
			f051206Repo.BulkInsert(addF051206List, true, "LACK_SEQ");
			f05120601Repo.BulkUpdate(updF05120601List);
			BatchDeletF05120601(delF05120601List.Select(x => x.ID).ToList());
			f191302Repo.BulkInsert(addF191302List, true);
      if (addF050802List.Any())
        f050802Repo.BulkInsert(addF050802List);
      if (updF050802List.Any())
        f050802Repo.BulkUpdate(updF050802List);
      if (addF050302List.Any())
        f050302Repo.BulkInsert(addF050302List);
      if (updF050302List.Any())
        f050302Repo.BulkUpdate(updF050302List);

			_wmsLogHelper.AddRecord("揀缺配庫處理 結束");
			return new ExecuteResult(true);
		}

    /// <summary>
    /// 序號綁儲位商品需重新指定出貨序號
    /// </summary>
    /// <param name="f05120601"></param>
    /// <param name="itemStock"></param>
    /// <param name="curf050802"></param>
    /// <param name="addF050802s"></param>
    /// <param name="updF050802s"></param>
    /// <param name="addF050302s"></param>
    /// <param name="updF050302s"></param>
    private void UpdateBundleSerialNoF050802(F05120601 f05120601, ItemLocPriorityInfo itemStock, F050802 curf050802, 
      ref List<F050802> addF050802s, ref List<F050802> updF050802s, ref List<F050302> addF050302s, ref List<F050302> updF050302s)
    {
      //原始配庫為一般序號商品，但後續轉成序號綁儲位商品，拆訂單＆出貨單
      if (string.IsNullOrWhiteSpace(f05120601.SERIAL_NO) && !string.IsNullOrWhiteSpace(itemStock.SERIAL_NO) && itemStock.SERIAL_NO != "0")
        SplitNewOrderDetail(f05120601, itemStock.SERIAL_NO, ref addF050802s, ref updF050802s, ref addF050302s, ref updF050302s);
      //序號綁儲位商品需重新指定出貨序號
      else if (!updF050802s.Any(x => x.WMS_ORD_NO == curf050802.WMS_ORD_NO && x.WMS_ORD_SEQ == curf050802.WMS_ORD_SEQ))
      {
        curf050802.SERIAL_NO = itemStock.SERIAL_NO == "0" ? null : itemStock.SERIAL_NO;
        updF050802s.Add(curf050802);
      }
    }

    
    /// <summary>
    /// 如果撿缺前將序號商品改為序號綁儲位商品，要將原有明細拆出
    /// </summary>
    /// <param name="f05120601"></param>
    /// <param name="serialNo">配庫的序號</param>
    /// <param name="addF050802s"></param>
    /// <param name="updF050802s"></param>
    /// <param name="addF050302s"></param>
    /// <param name="updF050302s"></param>
    private void SplitNewOrderDetail(F05120601 f05120601, string serialNo, ref List<F050802> addF050802s, ref List<F050802> updF050802s, ref List<F050302> addF050302s, ref List<F050302> updF050302s)
    {
      var f050302Repo = new F050302Repository(Schemas.CoreSchema);
      var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
      var f050802Repo = new F050802Repository(Schemas.CoreSchema);

      if (_pickLackF050302s == null)
        _pickLackF050302s = new List<F050302>();

      var ordNo = f05030101Repo.GetDatas(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, new[] { f05120601.WMS_ORD_NO }.ToList()).FirstOrDefault()?.ORD_NO;
      if (!_pickLackF050302s.Any(x => x.DC_CODE == f05120601.DC_CODE && x.GUP_CODE == f05120601.GUP_CODE && x.CUST_CODE == f05120601.CUST_CODE && x.ORD_NO == ordNo))
        _pickLackF050302s.AddRange(f050302Repo.GetDatasByWmsOrdNo(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, f05120601.WMS_ORD_NO).ToList());

      var oriF050302s = _pickLackF050302s.Where(x => x.DC_CODE == f05120601.DC_CODE && x.GUP_CODE == f05120601.GUP_CODE && x.CUST_CODE == f05120601.CUST_CODE && x.ORD_NO == ordNo).ToList();
      var oriF050302 = oriF050302s.FirstOrDefault(x => x.ITEM_CODE == f05120601.ITEM_CODE && string.IsNullOrWhiteSpace(x.SERIAL_NO));
      //訂單數量>1才需要拆明細出來
      if (oriF050302 != null && oriF050302.ORD_QTY > 1)
      {
        var chkF050302exists = updF050302s.FirstOrDefault(x =>
          x.DC_CODE == oriF050302.DC_CODE
          && x.GUP_CODE == oriF050302.GUP_CODE
          && x.CUST_CODE == oriF050302.CUST_CODE
          && x.ORD_NO == oriF050302.ORD_NO);

        if (chkF050302exists == null)
        {
          oriF050302.ORD_QTY -= 1;
          updF050302s.Add(oriF050302);
        }
        else
          oriF050302.ORD_QTY -= 1;

        var newf050302 = JsonConvert.DeserializeObject<F050302>(JsonConvert.SerializeObject(oriF050302));
        newf050302.ORD_QTY = 1;
        newf050302.ORD_SEQ = (oriF050302s.Select(x => Convert.ToInt32(x.ORD_SEQ)).Max(x => x) + 1).ToString();
        newf050302.CRT_DATE = DateTime.Now;
        addF050302s.Add(newf050302);
        _pickLackF050302s.Add(newf050302);
      }

      var oriF050802s = new List<F050802>();
      if (_pickLackF050802s != null && _pickLackF050802s.Any())
        oriF050802s = _pickLackF050802s.Where(x => x.DC_CODE == f05120601.DC_CODE && x.GUP_CODE == f05120601.GUP_CODE && x.CUST_CODE == f05120601.CUST_CODE && x.WMS_ORD_NO == f05120601.WMS_ORD_NO).ToList();
      else
      {
        _pickLackF050802s = f050802Repo.GetDatas(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, f05120601.WMS_ORD_NO).ToList();
        oriF050802s = _pickLackF050802s;
      }
      var oriF050802 = oriF050802s.FirstOrDefault(x => x.ITEM_CODE == f05120601.ITEM_CODE && string.IsNullOrWhiteSpace(x.SERIAL_NO));
      //數量 > 1才需要拆明細出來
      if (oriF050802 != null)
      {
        var IsSplitRec = true;
        var chkF050802exists = updF050802s.FirstOrDefault(x =>
          x.DC_CODE == oriF050802.DC_CODE
          && x.GUP_CODE == oriF050802.GUP_CODE
          && x.CUST_CODE == oriF050802.CUST_CODE
          && x.WMS_ORD_NO == oriF050802.WMS_ORD_NO);

        
        if (chkF050802exists == null)
        {
          if (oriF050802.ORD_QTY > 1)
          {
            oriF050802.ORD_QTY -= 1;
            oriF050802.B_DELV_QTY -= 1;
            updF050802s.Add(oriF050802);
          }
          else
          {
            oriF050802.SERIAL_NO = serialNo;
            updF050802s.Add(oriF050802);
            IsSplitRec = false;
          }
        }
        else
        {
          if (chkF050802exists.ORD_QTY > 1)
          {
            chkF050802exists.ORD_QTY -= 1;
            chkF050802exists.B_DELV_QTY -= 1;
          }
          else
          {
            chkF050802exists.SERIAL_NO = serialNo;
            IsSplitRec = false;
          }
        }

        //如果原始f050802的數量為1的話直接更新原有記錄的serial_no就可以，不用拆明細
        if (IsSplitRec)
        {
          var newf050802 = JsonConvert.DeserializeObject<F050802>(JsonConvert.SerializeObject(oriF050802));
          newf050802.ORD_QTY = 1;
          oriF050802.B_DELV_QTY = 1;
          newf050802.SERIAL_NO = serialNo;
          newf050802.WMS_ORD_SEQ = (oriF050802s.Select(x => Convert.ToInt32(x.WMS_ORD_SEQ)).Max(x => x) + 1).ToString("0000");
          newf050802.CRT_DATE = DateTime.Now;
          addF050802s.Add(newf050802);
        }

      }
    }

    /// <summary>
    /// f05120601.STATUS = 3 (缺貨)時的處理
    /// </summary>
    /// <param name="f05120601"></param>
    /// <param name="addF051206List"></param>
    /// <param name="f060702Status"></param>
    private void F05120601Status3Process(F05120601 f05120601, ref List<F051206> addF051206List, ref int? f060702Status)
    {
      f05120601.STATUS = "3"; //缺貨
                              // 新增揀貨缺貨記錄
      addF051206List.Add(CreateF051206(f05120601));

      //庫存不足，集貨等待通知直接發異常
      if (f060702Status != null)
        f060702Status = 2;
    }
    #region 撿缺集貨等待通知處理
    /// <summary>
    /// 撿缺集貨等待通知處理
    /// </summary>
    /// <param name="addF060702List"></param>
    /// <param name="f060702Status"></param>
    /// <param name="f05120601"></param>
    private void PickLackCollectionStatusProcess(F060702Repository f060702Repo, ref List<F060702> addF060702List, int? f060702Status, F05120601 f05120601)
    {
      var f050101Repo = new F050101Repository(Schemas.CoreSchema);
      var f060202Repo = new F060202Repository(Schemas.CoreSchema);
      var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);

      if (f060702Status != null)
      {
        //檢查訂單是否被取消
        var f050101s = f050101Repo.GetOrdNoByWmsOrdNo(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, f05120601.WMS_ORD_NO);
        if (f050101s.Any(x => x.STATUS == "9"))
          f060702Status = 2;

        if (f060702Status == 9999)
          _wmsLogHelper.AddRecord("未被評估的集貨等待通知狀態");
        else
        {
          //如果前面的判斷已經是要發異常處理，就不檢查是否有發過等待補揀的狀態
          if (f060702Status != 2)
          {
            //已經有等待補檢的狀態，不重複發
            var isExistStatus1Data = f060702Repo.IsExistStatus1Data(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, f05120601.WMS_ORD_NO);
            if (isExistStatus1Data)
              return;
          }

          var f060202Data = f060202Repo.GetDatasByTrueAndCondition(x =>
             x.DC_CODE == f05120601.DC_CODE &&
             x.GUP_CODE == f05120601.GUP_CODE &&
             x.CUST_CODE == f05120601.CUST_CODE &&
             x.PICK_NO == f05120601.PICK_ORD_NO &&
             x.M_STATUS == "3").FirstOrDefault();

          //檢查新增清單內是否已有相同資料
          var curF060702 = addF060702List.FirstOrDefault(x => x.DC_CODE == f05120601.DC_CODE && x.GUP_CODE == f05120601.GUP_CODE && x.CUST_CODE == f05120601.CUST_CODE && x.ORDER_CODE == f060202Data.DOC_ID);
          if (curF060702 == null)
          {
            addF060702List.Add(new F060702()
            {
              DC_CODE = f060202Data.DC_CODE,
              GUP_CODE = f060202Data.GUP_CODE,
              CUST_CODE = f060202Data.CUST_CODE,
              ORDER_CODE = f060202Data.DOC_ID,
              ORI_ORDER_CODE = f060202Data.WMS_NO,
              STATUS = f060702Status.Value,
              PROC_FLAG = "0",
              PROC_DATE = DateTime.Now,
              MESSAGE = "",
            });
          }
          else if (curF060702.STATUS == 1 && f060702Status == 2) //跑產生補撿流程只有 1:等待補撿 & 2:異常處理，只要有一筆出現異常處理，就覆蓋前面的狀態
            curF060702.STATUS = f060702Status.Value;
        }

        //如果集貨等待通知是異常，那就把集貨場改為人工集貨場
        if (f060702Status == 2)
        {
          var currData = f051301Repo.GetDatasByWmsOrdNos(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, new[] { f05120601.WMS_ORD_NO }.ToList()).FirstOrDefault();
          if (currData.COLLECTION_POSITION == "1")
            f051301Repo.UpdateCollectionPosition(currData.DC_CODE, currData.GUP_CODE, currData.CUST_CODE, currData.DELV_DATE, currData.PICK_TIME, currData.WMS_NO, "0");
        }
      }

    }
    #endregion 撿缺集貨等待通知處理

    #region 取得商品揀區庫存
    private List<CustItemStock> _tempCustItemStocks;
		/// <summary>
		/// 取得商品揀區庫存
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		private List<ItemLocPriorityInfo> GetItemPickStocks(string dcCode, string gupCode, string custCode, string itemCode)
		{
			_wmsLogHelper.AddRecord(string.Format("取得商品[{0}]揀區庫存 開始", itemCode));
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			if (_tempCustItemStocks == null)
				_tempCustItemStocks = new List<CustItemStock>();

			var custItemStock = _tempCustItemStocks.FirstOrDefault(x => x.DcCode == dcCode && x.GupCode == gupCode && x.CustCode == custCode && x.ItemCode == itemCode);
			var stocks = new List<ItemLocPriorityInfo>();
			if (custItemStock == null)
			{
				stocks = f1913Repo.GetItemPickLocPriorityInfo(dcCode, gupCode, custCode, new List<string> { itemCode }, false, "G").Where(a => a.QTY > 0).ToList();
				_tempCustItemStocks.Add(new CustItemStock
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					ItemCode = itemCode,
					ItemLocPriorityInfos = stocks
				});
			}
			else
				stocks = custItemStock.ItemLocPriorityInfos.Where(x => x.QTY > 0).ToList();

			stocks = stocks.OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
								.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).ThenBy(a => a.BOX_SERIAL)
								.ThenBy(a => a.CASE_NO).ThenBy(a => a.BATCH_NO).ThenBy(a => a.BOX_CTRL_NO).ThenBy(a => a.PALLET_CTRL_NO).ThenBy(a => a.MAKE_NO).ToList();

			_wmsLogHelper.AddRecord(string.Format("取得商品[{0}]揀區庫存 結束", itemCode));
			return stocks;
		}

		#endregion

		#region 新增配庫後揀貨資料
		/// <summary>
		/// 新增配庫後揀貨資料
		/// </summary>
		/// <param name="itemStock"></param>
		/// <param name="f05120601"></param>
		/// <param name="needQty"></param>
		/// <returns></returns>
		private F050306 CreateF050306(ItemLocPriorityInfo itemStock, F05120601 f05120601, int needQty)
		{
			var f051201 = GetF051201(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, f05120601.PICK_ORD_NO);
			var f050801 = GetF050801(f05120601.DC_CODE, f05120601.GUP_CODE, f05120601.CUST_CODE, f05120601.WMS_ORD_NO);

      return new F050306
      {
        DC_CODE = itemStock.DC_CODE,
        GUP_CODE = itemStock.GUP_CODE,
        CUST_CODE = itemStock.CUST_CODE,
        ORD_TYPE = f05120601.ORD_TYPE,
        SOURCE = "02",
        SOURCE_TYPE = f05120601.SOURCE_TYPE,
        FAST_DEAL_TYPE = f05120601.FAST_DEAL_TYPE,
        CUST_COST = f05120601.CUST_COST,
        WMS_NO = f05120601.PICK_ORD_NO,
        WMS_SEQ = f05120601.PICK_ORD_SEQ,
        WAREHOUSE_ID = itemStock.WAREHOUSE_ID,
        WH_TMPR_TYPE = itemStock.TMPR_TYPE,
        PICK_LOC = itemStock.LOC_CODE,
        PICK_FLOOR = itemStock.PICK_FLOOR,
        DEVICE_TYPE = itemStock.DEVICE_TYPE,
        ITEM_CODE = itemStock.ITEM_CODE,
        B_PICK_QTY = needQty,
        ENTER_DATE = itemStock.ENTER_DATE,
        VALID_DATE = itemStock.VALID_DATE,
        MAKE_NO = itemStock.MAKE_NO,
        BOX_CTRL_NO = itemStock.BOX_CTRL_NO,
        PALLET_CTRL_NO = itemStock.PALLET_CTRL_NO,
        VNR_CODE = itemStock.VNR_CODE,
        SERIAL_NO = itemStock.SERIAL_NO,
        WMS_ORD_NO = f05120601.WMS_ORD_NO,
        WMS_ORD_SEQ = f05120601.WMS_ORD_SEQ,
        LACK_ID = f05120601.ID,
        PACKING_TYPE = f051201.PACKING_TYPE,
        CONTAINER_TYPE = f051201.CONTAINER_TYPE,
        PK_AREA = string.IsNullOrWhiteSpace(itemStock.PK_AREA) ? itemStock.WAREHOUSE_ID : itemStock.PK_AREA,
        PK_AREA_NAME = string.IsNullOrWhiteSpace(itemStock.PK_NAME) ? itemStock.WAREHOUSE_NAME : itemStock.PK_AREA,
        RTN_VNR_CODE = f051201.RTN_VNR_CODE,
        MOVE_OUT_TARGET = f051201.MOVE_OUT_TARGET,
        ORDER_CRT_DATE= f051201.ORDER_CRT_DATE,
        ORDER_PROC_TYPE = f051201.ORDER_PROC_TYPE,
        ORDER_ZIP_CODE = f051201.ORDER_ZIP_CODE,
        IS_NORTH_ORDER = f051201.IS_NORTH_ORDER,
				SUG_BOX_NO = f050801.SUG_BOX_NO,
				SUG_LOGISTIC_CODE = f050801.SUG_LOGISTIC_CODE
      };
		}

		#endregion

		#region 新增揀貨缺貨記錄

		/// <summary>
		/// 新增揀貨缺貨記錄
		/// </summary>
		/// <param name="f05120601"></param>
		/// <returns></returns>
		private F051206 CreateF051206(F05120601 f05120601)
		{
			return new F051206
			{
				DC_CODE = f05120601.DC_CODE,
				GUP_CODE = f05120601.GUP_CODE,
				CUST_CODE = f05120601.CUST_CODE,
				PICK_ORD_NO = f05120601.PICK_ORD_NO,
				PICK_ORD_SEQ = f05120601.PICK_ORD_SEQ,
				ITEM_CODE = f05120601.ITEM_CODE,
				LOC_CODE = f05120601.PICK_LOC,
				WMS_ORD_NO = f05120601.WMS_ORD_NO,
				LACK_QTY = f05120601.LACK_QTY,
				ISDELETED = "0",
				STATUS = "1",
				TRANS_FLAG = "0",
        SERIAL_NO = f05120601.SERIAL_NO,
        CRT_DATE = f05120601.CRT_DATE,
				CRT_STAFF = f05120601.CRT_STAFF,
				CRT_NAME = f05120601.CRT_NAME
			};
		}
		#endregion

		#region 庫存異常處理

		/// <summary>
		/// 庫存異常處理
		/// </summary>
		/// <param name="stockLack"></param>
		/// <returns></returns>
		public StockLackResult CreateStockLackProcess(StockLack stockLack)
		{
			var result = new StockLackResult { IsSuccessed = true, AddF191302List = new List<F191302>(), ReturnNewAllocations = new List<ReturnNewAllocation>() };
			var shardService = new SharedService(_wmsTransaction);

			var srcWareHouseId = CommonService.GetLoc(stockLack.F051202.DC_CODE, stockLack.F051202.PICK_LOC).WAREHOUSE_ID;
			// 產生純上架到疑似遺失倉調撥單
			var newAllocationParam = new NewAllocationItemParam
			{
				TarDcCode = stockLack.DcCode,
				TarWarehouseId = stockLack.PickLackWarehouseId,
				GupCode = stockLack.GupCode,
				CustCode = stockLack.CustCode,
				AllocationDate = DateTime.Now,
				AllocationType = AllocationType.NoSource,
				IsExpendDate = true,
				isIncludeResupply = true,
				ReturnStocks = stockLack.ReturnStocks,
				Memo = Properties.Resources.PickLossToPickLossWarehouse,
				AllocationTypeCode = "7",
				StockDetails = new List<StockDetail>
				{
					new StockDetail
					{
						CustCode = stockLack.F051202.CUST_CODE,
						GupCode = stockLack.F051202.GUP_CODE,
						SrcDcCode = stockLack.F051202.DC_CODE,
						TarDcCode = stockLack.F051202.DC_CODE,
						SrcWarehouseId = "", //純上架不設定來源倉
						TarWarehouseId = stockLack.PickLackWarehouseId,
						SrcLocCode = stockLack.F051202.PICK_LOC,
						TarLocCode = stockLack.PickLackLocCode,
						ItemCode =stockLack.F051202.ITEM_CODE,
						ValidDate = stockLack.F051202.VALID_DATE,
						EnterDate = stockLack.F051202.ENTER_DATE,
						Qty = stockLack.LackQty,
						VnrCode = "000000",
						SerialNo = stockLack.F051202.SERIAL_NO,
						BoxCtrlNo = "0",
						PalletCtrlNo = "0",
						MAKE_NO = string.IsNullOrEmpty(stockLack.F051202.MAKE_NO) ? "0" : stockLack.F051202.MAKE_NO,
					}
				}
			};
			var returnAllocationResult = shardService.CreateOrUpdateAllocation(newAllocationParam);
			if (!returnAllocationResult.Result.IsSuccessed)
				return new StockLackResult { IsSuccessed = returnAllocationResult.Result.IsSuccessed, Message = returnAllocationResult.Result.Message };
			else
			{
				result.ReturnNewAllocations.AddRange(returnAllocationResult.AllocationList);
				result.ReturnStocks = returnAllocationResult.StockList;
				foreach (var allot in returnAllocationResult.AllocationList)
				{
					foreach (var allotDetail in allot.Details)
					{
						//新增到F191302 庫存異常明細表
						result.AddF191302List.Add(new F191302
						{
							DC_CODE = allotDetail.DC_CODE,
							GUP_CODE = allotDetail.GUP_CODE,
							CUST_CODE = allotDetail.CUST_CODE,
							ALLOCATION_NO = allotDetail.ALLOCATION_NO,
							ALLOCATION_SEQ = allotDetail.ALLOCATION_SEQ,
							ITEM_CODE = allotDetail.ITEM_CODE,
							VALID_DATE = allotDetail.VALID_DATE,
							ENTER_DATE = allotDetail.ENTER_DATE,
							MAKE_NO = allotDetail.MAKE_NO,
							SERIAL_NO = allotDetail.SERIAL_NO,
							VNR_CODE = allotDetail.VNR_CODE,
							BOX_CTRL_NO = allotDetail.BOX_CTRL_NO,
							PALLET_CTRL_NO = allotDetail.PALLET_CTRL_NO,
							QTY = (int)allotDetail.TAR_QTY,
							SRC_WMS_NO = stockLack.F051202.PICK_ORD_NO,
							SRC_WAREHOUSE_ID = srcWareHouseId,
							SRC_LOC_CODE = stockLack.F051202.PICK_LOC,
							TAR_WAREHOUSE_ID = stockLack.PickLackWarehouseId,
							TAR_LOC_CODE = stockLack.PickLackLocCode,
							CRT_DATE = stockLack.F051202.CRT_DATE,
							CRT_STAFF = stockLack.F051202.CRT_STAFF,
							CRT_NAME = stockLack.F051202.CRT_NAME,
							PROC_FLAG = "0",
							SRC_TYPE = "0", //揀貨缺貨
						});
					}
				}
			}

			// 調整預計數 = 實揀數
			// stockLack.F051202.B_PICK_QTY = stockLack.F051202.A_PICK_QTY; 這邊不可以改掉，會影響缺貨判斷
			stockLack.F1511.B_PICK_QTY = stockLack.F1511.A_PICK_QTY;
			if (stockLack.F051202.PICK_STATUS == "0")
				stockLack.F051202.PICK_STATUS = "1";
			if (stockLack.F1511.STATUS == "0")
				stockLack.F1511.STATUS = "1";
			result.UpdF051202 = stockLack.F051202;
			result.UpdF1511 = stockLack.F1511;

			return result;
		}

    #endregion

    #endregion

    #region 揀貨虛擬儲位回復
    /// <summary>
    /// 虛擬儲位回復，注意呼叫後要呼叫StockService.SaveChange()去將資料整批寫入
    /// </summary>
    /// <param name="excludeF051202s">因需更新F051201，所以帶入需要排除F051202的(代表已經回復虛擬儲位了)才能正確找到底是不是所有揀貨明細都已回復</param>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="PickOrdNo">揀貨單單號</param>
    /// <param name="warehouseId"></param>
    /// <returns></returns>
    public ExecuteResult PickVirtualLocRecovery(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var addStockList = new List<OrderStockChange>();
      var delF051301List = new List<string>();
      var result = new ExecuteResult(true);

      //只處理P單
      if (!new string[] { "P" }.Contains(pickOrdNo.Substring(0, 1)))
      {
        result = new ExecuteResult(false, $"不可處理P單外之資料，單號：{pickOrdNo}");
        return result;
      }

			var f051201 = GetF051201(dcCode, gupCode, custCode, pickOrdNo);
			var f051202s = GetF051202s(dcCode, gupCode, custCode, pickOrdNo);

			// 取得虛擬儲位
			var f1511s = GetF1511s(dcCode, gupCode, custCode, pickOrdNo);

			// 是否可回復虛擬儲位庫存資料
			bool isReply = f1511s.Any(o => o.STATUS == "0");

			if (isReply)
			{
				var updF051202List = new List<F051202>();

				f1511s = f1511s.Where(o => o.STATUS == "0").ToList();

        addStockList.AddRange(
					f1511s.Select(x => new OrderStockChange()
          {
            DcCode = x.DC_CODE,
            GupCode = x.GUP_CODE,
            CustCode = x.CUST_CODE,
            LocCode = x.LOC_CODE,
            ItemCode = x.ITEM_CODE,
            MakeNo = x.MAKE_NO,
            EnterDate = x.ENTER_DATE.Value,
            VnrCode = "000000",
            VaildDate = x.VALID_DATE.Value,
            SerialNo = string.IsNullOrEmpty(x.SERIAL_NO) ? "0" : x.SERIAL_NO,
            BoxCtrlNo = x.BOX_CTRL_NO,
            PalletCtrlNo = x.PALLET_CTRL_NO,
            Qty = x.B_PICK_QTY,
            WmsNo = x.ORDER_NO
          }));

				f1511s.ForEach(x =>
        {
					var f051202 = f051202s.FirstOrDefault(y => y.PICK_ORD_NO == x.ORDER_NO && y.PICK_ORD_SEQ == x.ORDER_SEQ);
					if(f051202!=null)
					{
						f051202.PICK_STATUS = "9";
						updF051202List.Add(f051202);
					}
          x.STATUS = "9";
        });

				#region 更新F051201
				f051201.PICK_STATUS = 9;
				_f051201Repo.Update(f051201);
				#endregion

				_f1511Repo.BulkUpdate(f1511s);
				_f051202Repo.BulkUpdate(updF051202List);

				if (addStockList.Any())
				{
					StockService.AddStock(addStockList);
					UpdateUsedVolumnByLocCodes(dcCode, gupCode, custCode, addStockList.Select(x => x.LocCode).Distinct());
				}

        // 若出貨單所屬揀貨單明細均已取消，刪除F051301
        var f051202sByWmsNos = f051202Repo.GetDatasByWmsOrdNos(dcCode, gupCode, custCode, f051202s.Select(o => o.WMS_ORD_NO).Distinct().ToList());
        var f051202sGroupBy = f051202sByWmsNos.GroupBy(o => o.WMS_ORD_NO);

        foreach (var group in f051202sGroupBy)
        {
          var notCanceledDetail = group.Where(o => o.PICK_STATUS == "0" && !updF051202List.Select(x => x.PICK_ORD_NO).Contains(o.PICK_ORD_NO));

          if (!notCanceledDetail.Any())
          {
            delF051301List.Add(group.Key);
          }
        }

        f051301Repo.DeleteF051301(dcCode, gupCode, custCode, delF051301List);
      }

      return result;
    }
    #endregion 揀貨虛擬儲位回復

    /// <summary>
    /// CreateOrderPickWithApi Function 呼叫資料後資料KEY
    /// </summary>
    private class ApiResultKey
		{
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			public string ORD_TYPE { get; set; }
			public string SOURCE_TYPE { get; set; }
			public string CUST_COST { get; set; }
			public string FAST_DEAL_TYPE { get; set; }
			public string MOVE_OUT_TARGET { get; set; }
			public string WH_TMPR_TYPE { get; set; }
			public string RTN_VNR_CODE { get; set; }
			public string PACKING_TYPE { get; set; }
		}

        /// <summary>
        /// 產生揀貨單後，重新確認訂單是否有被取消並呼叫出貨單取消共用函數
        /// </summary>
        /// <param name="f050306s">傳入CreatePick的F050306資料</param>
        /// <param name="CanceledOrders">取消的訂單資料</param>
        /// <returns></returns>
        public ExecuteResult AfterCreatePickCheckOrder(out List<F050301> CanceledOrders)
        {
            var f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);
            var orderservice = new OrderService(_wmsTransaction);
            ExecuteResult cancelRes;
            List<F050301> updf050301s = new List<F050301>();
            CanceledOrders = new List<F050301>();

            var f050301s = f050301Repo.GetCancelNotCompleteOrd();

            if (f050301s.Any())
            {
                foreach (var f050301 in f050301s)
                {
                    cancelRes = orderservice.CancelAllocStockOrder(f050301.DC_CODE, f050301.GUP_CODE, f050301.CUST_CODE, new List<string>() { f050301.ORD_NO }, "0", string.Empty, string.Empty, string.Empty, string.Empty, "999", "訂單取消");

                    if (!cancelRes.IsSuccessed)
                        return cancelRes;

                    f050301.PROC_FLAG = "9";
                    CanceledOrders.Add(f050301);
                }

                f050301Repo.BulkUpdate(CanceledOrders);
            }

            return new ExecuteResult(true);
        }

    /// <summary>
    /// 將LMS配庫有錯誤的訂單改為03，並將訊息寫到F0093
    /// </summary>
    /// <param name="f0093Repo"></param>
    /// <param name="f050306Repo"></param>
    /// <param name="f050306s"></param>
    /// <param name="ErrorMessage"></param>
    /// <param name="isPickSchedule"></param>
    /// <param name="ErrorWmsNo"></param>
    public void InserLMSPickFaildMsgToF0093(F0093Repository f0093Repo, F050306Repository f050306Repo, List<F050306> f050306s, String ErrorMessage, Boolean isPickSchedule, List<string> ErrorWmsNo = null)
    {
      //只把有錯的揀貨資料改為錯誤
      var tmpf050306s = f050306s.ToList();
      if (ErrorWmsNo?.Any() ?? false)
        tmpf050306s = tmpf050306s.Where(x => ErrorWmsNo.Contains(x.WMS_NO)).ToList();
      var gF050306 = tmpf050306s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.WMS_NO });

      if (isPickSchedule)
      {
        tmpf050306s.ForEach(x => x.SOURCE = "03");
        f050306Repo.BulkUpdate(tmpf050306s);
      }

      var addF0093 = new List<F0093>();
      foreach (var item in gF050306.Select(x => x.Key))
      {
        addF0093.Add(new F0093()
        {
          DC_CODE = item.DC_CODE,
          GUP_CODE = item.GUP_CODE,
          CUST_CODE = item.CUST_CODE,
          WMS_NO = item.WMS_NO,
          WMS_TYPE = item.WMS_NO.Substring(0, 1),
          STATUS = "0",
          MSG_NO = "API20003",
          MSG_CONTENT = ErrorMessage
        });
      }

      f0093Repo.BulkInsert(addF0093);
		}

    #region 檢查揀貨單是否所有出貨單已取消共用函數
    /// <summary>
    /// 檢查揀貨單對應之所有出貨單是否已取消，若所有出貨單都取消，自動進行虛擬庫存回庫
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="PickOrdNoList">揀貨單單號清單</param>
    /// <returns></returns>
    public List<string> CheckIfAllOrdersCanceledByPickNoList(string dcCode, string gupCode, string custCode, List<string> PickOrdNoList)
		{

			var cancelPickNoList = new List<string>();
      var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
      var f051202Withf050801s = f051202Repo.GetOrderStatusByPickNos(dcCode, gupCode, custCode, PickOrdNoList).ToList();
      foreach (var f051202_PICK_NO_Item in PickOrdNoList)
      {
        var f051202Cancels = f051202Withf050801s.Where(x => x.PICK_ORD_NO == f051202_PICK_NO_Item); ;
				// 揀貨明細所有出貨單狀態都為取消
				if (f051202Cancels.All(x => x.STATUS == 9))
        {
					cancelPickNoList.Add(f051202_PICK_NO_Item);
					// 揀貨單虛擬庫存回庫
					PickVirtualLocRecovery(dcCode, gupCode, custCode, f051202_PICK_NO_Item);
				}
			}
      if(cancelPickNoList.Any())
        StockService.SaveChange();
      return cancelPickNoList;
    }
    #endregion 檢查揀貨單是否所有出貨單已取消共用函數

    #region 揀貨容器共用函數
    /// <summary>
    /// 計算出貨單周轉箱類型
    /// </summary>
    /// <param name="f190105"></param>
    /// <param name="f050801"></param>
    /// <param name="f050802s"></param>
    /// <param name="f051201s">出貨單的F051201清單</param>
    /// <param name="f051202s">出貨單的F051202清單</param>
    /// <returns></returns>
    public ExecuteResult CalculateShipmentOrderTurnoverContainer(F190105 f190105,F050801 f050801,List<F050802> f050802s,List<F051201> f051201s,List<F051202> f051202s)
    {
      if (!f190105.BASE_CONTAINER_MAX_LENGTH.HasValue|| 
        !f190105.BASE_CONTAINER_MID_LENGTH.HasValue|| 
        !f190105.BASE_CONTAINER_MIN_LENGTH.HasValue ||
        !f190105.BASE_CONTAINER_VOLUMN.HasValue )
      {
        return new ExecuteResult(false, "物流中心出貨指示設定資料未設定周轉箱資料");
      }

      var BaseContainerCnt = 0; //[S]
      var ContainerType = f190105.BASE_CONTAINER_TYPE;   //[X]

      //如果揀貨單有人工＋自動，就要指定揀貨容器為DF_NSHIP_CONTAINER_TYPE(2L)
      if (f051201s.Any(x => x.DISP_SYSTEM == "1") && f051201s.Any(x => x.DISP_SYSTEM == "0"))
      {
        ContainerType = f190105.DF_NSHIP_CONTAINER_TYPE;

        SetF051201ContainerType(ContainerType, ref BaseContainerCnt, f051201s.Where(x => x.DISP_SYSTEM != "0").ToList());
        SetF050801ContainerType(ContainerType, BaseContainerCnt, f050801);
        return new ExecuteResult(true);
      }

      //[A] = 取得出貨單明細[F050802]

      //[B] = 取得商品材積資料[F1905] 請用CommonService做cache
      var f1905s = CommonService.GetProductSizeList(f050801.GUP_CODE, f050801.CUST_CODE, f050802s.Select(x => x.ITEM_CODE).ToList());

      /*
      	取得商品[B] [長、寬、高]最大值 > BASE_CONTAINER_MAX_LENGTH
      	取得商品[B] [長、寬、高]最小值 > BASE_CONTAINER_MIN_LENGH
      	取得商品[B] [長、寬、高]次大值 > BASE_CONTAINER_MID_LENGTH
      	取得商品[B]材積[長X寬X高] > BASE_CONTAINER_VOLUMN
      	只要以上條件有一個符合，則[X]=F190105. DF_NSHIP_CONTAINER_TYPE
      */
      var IsChangeContainerType = false;
      foreach (var f1905 in f1905s)
      {
        var ItemSize = new[] { f1905.PACK_HIGHT, f1905.PACK_WIDTH, f1905.PACK_LENGTH }.ToList();

        if (ItemSize.Max() > f190105.BASE_CONTAINER_MAX_LENGTH)
        {
          IsChangeContainerType = true;
          break;
        }
        ItemSize.Remove(ItemSize.Max());

        if (ItemSize.Min() > f190105.BASE_CONTAINER_MIN_LENGTH)
        {
          IsChangeContainerType = true;
          break;
        }
        ItemSize.Remove(ItemSize.Min());

        if (ItemSize.First() > f190105.BASE_CONTAINER_MID_LENGTH)
        {
          IsChangeContainerType = true;
          break;
        }

        if (f1905.PACK_HIGHT * f1905.PACK_WIDTH * f1905.PACK_LENGTH > f190105.BASE_CONTAINER_VOLUMN)
        {
          IsChangeContainerType = true;
          break;
        }
      }

      //以出貨單明細[A]計算所有商品[A]總容積[長X寬X高]*預計出貨數 > BASE_CONTAINER_VOLUMN * [C]
      if (!IsChangeContainerType)
      {
        var f050802WithSpec = (from a in f050802s
                              join b in f1905s
                              on a.ITEM_CODE equals b.ITEM_CODE
                              select new { a.ITEM_CODE, B_DELV_QTY = a.B_DELV_QTY ?? 0, b.PACK_HIGHT, b.PACK_WIDTH, b.PACK_LENGTH })
                              .GroupBy(x=>x.ITEM_CODE);
        foreach (var item in f050802WithSpec)
        {
          if (item.Sum(x=>x.PACK_HIGHT * x.PACK_WIDTH * x.PACK_LENGTH * x.B_DELV_QTY) > f190105.BASE_CONTAINER_VOLUMN * ItemMaxTurnoverContainerCnt)
          {
            IsChangeContainerType = true;
            break;
          }
        }
      }

      //如果揀貨單筆數 <參數4>.Count > [D]
      if (!IsChangeContainerType && f051201s.Count > OrderMaxTurnoverContainerCnt)
        IsChangeContainerType = true;

      if (IsChangeContainerType)
        ContainerType = f190105.DF_NSHIP_CONTAINER_TYPE;

      //計算每一張揀貨單該出貨單商品可裝幾個基礎周轉箱數[S]
      foreach (var f051201 in f051201s)
      {
        var curF051202s = f051202s.Where(x => x.DC_CODE == f051201.DC_CODE && x.GUP_CODE == f051201.GUP_CODE && x.CUST_CODE == f051201.CUST_CODE && x.PICK_ORD_NO == f051201.PICK_ORD_NO);
        var f051202sWithSpec = from a in curF051202s
                               join b in f1905s
                               on a.ITEM_CODE equals b.ITEM_CODE
                               select new { a.ITEM_CODE, a.B_PICK_QTY, b.PACK_WIDTH, b.PACK_HIGHT, b.PACK_LENGTH };
        // 揀貨單基礎周轉箱數[G] = SUM(商品[長X寬X高]*預計揀貨數) / BASE_CONTAINER_VOLUMN，若有餘數則+1
        var baseTurnoverContainerCnt = 
          Convert.ToInt32(Math.Ceiling(f051202sWithSpec.Sum(x => x.PACK_WIDTH * x.PACK_HIGHT * x.PACK_LENGTH * x.B_PICK_QTY) / f190105.BASE_CONTAINER_VOLUMN ?? 0));

        // 如果揀貨單為自動倉揀貨單
        if (f051201.DISP_SYSTEM != "0")
        {
          /*
          更新F051201. CONTAINER_B_CNT = [G]
          [S] +=[G]
          */
          f051201.CONTAINER_B_CNT = baseTurnoverContainerCnt;
          BaseContainerCnt += baseTurnoverContainerCnt;
        }
        else // 如果揀貨單為人工倉揀貨單
        {
          BaseContainerCnt += baseTurnoverContainerCnt;
        }

        //如果[S] > [D]
        //  [X]=F190105. DF_NSHIP_CONTAINER_TYPE
        if (BaseContainerCnt > OrderMaxTurnoverContainerCnt)
          ContainerType = f190105.DF_NSHIP_CONTAINER_TYPE;
      }

      //2.	如果[X]= F190105.BASE_CONTAINER_TYPE
      //(1)	設定出貨單的自動倉揀貨單F051201.CONTAINTR_TYPE =[X]
      if (ContainerType == f190105.BASE_CONTAINER_TYPE)
      {
        foreach (var f051201 in f051201s.Where(x => x.DISP_SYSTEM != "0"))
          f051201.CONTAINER_TYPE = ContainerType;
      }
      //3.	如果[X]!= F190105. BASE_CONTAINER_TYPE 
      //(1)	設定出貨單的自動倉揀貨單F051201.CONTAINTR_TYPE =[X]
      //(2)	設定出貨單的自動倉揀貨單F051201. CONTAINER_B_CNT =1
      //(3)	[S]= <參數4>.Count
      else
      {
        SetF051201ContainerType(ContainerType, ref BaseContainerCnt, f051201s.Where(x => x.DISP_SYSTEM != "0").ToList());
      }
      SetF050801ContainerType(ContainerType, BaseContainerCnt, f050801);
      return new ExecuteResult(true);
    }

    public void SetF051201ContainerType(string containerType, ref int baseContainerCnt, List<F051201> f051201s)
    {
      foreach (var f051201 in f051201s.Where(x => x.DISP_SYSTEM != "0"))
      {
        f051201.CONTAINER_TYPE = containerType;
        f051201.CONTAINER_B_CNT = 1;
      }
      baseContainerCnt = f051201s.Count;
    }

    public void SetF050801ContainerType(string containerType, int baseContainerCnt,F050801 f050801)
    {
      f050801.CONTAINER_TYPE = containerType;
      f050801.CONTAINER_B_CNT = baseContainerCnt;
    }

    /// <summary>
    /// 計算揀貨單周轉箱類型
    /// </summary>
    /// <param name="f190105"></param>
    /// <param name="f051201s"></param>
    /// <param name="f051202s"></param>
    /// <returns></returns>
    public ExecuteResult CalculatePickOrderTurnoverContainer(F190105 f190105, List<F051201> f051201s, List<F051202> f051202s)
    {
      f051201s.ForEach(x => x.CONTAINER_TYPE = f190105.BASE_CONTAINER_TYPE);
      return new ExecuteResult(true);
    }
    #endregion 揀貨容器共用函數
  }
}
