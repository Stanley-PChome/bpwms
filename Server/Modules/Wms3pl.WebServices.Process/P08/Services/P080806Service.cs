using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public class P080806Service : P080805Service
	{
		public P080806Service(WmsTransaction wmsTransaction = null) : base(wmsTransaction)
		{
		}

    private ContainerService _ContainerService;
    public ContainerService ContainerService
    {
      get
      {
        if (_ContainerService == null)
          _ContainerService = new ContainerService(wmsTransaction);

        return _ContainerService;
      }
      set
      {
        _ContainerService = value;
      }
    }

    /// <summary>
    /// 刷讀容器條碼
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public BindingPickContainerResult ScanBindingPickContainerCode(string dcCode, string gupCode, string custCode, string containerCode)
		{
			try
			{
				var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
				var f0536Repo = new F0536Repository(Schemas.CoreSchema, wmsTransaction);
				var f0534Repo = new F0534Repository(Schemas.CoreSchema, wmsTransaction);
				var f053601Repo = new F053601Repository(Schemas.CoreSchema, wmsTransaction);
				var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);
				var f0530Repo = new F0530Repository(Schemas.CoreSchema, wmsTransaction);
				var f070101Repo = new F070101Repository(Schemas.CoreSchema, wmsTransaction);
				var f0535Repo = new F0535Repository(Schemas.CoreSchema, wmsTransaction);
				var f0537Repo = new F0537Repository(Schemas.CoreSchema, wmsTransaction);
				var f051202Repo = new F051202Repository(Schemas.CoreSchema, wmsTransaction);
				F053602 f053602 = null;

				//(1)	揀貨容器鎖定
				var lockResult = LockPickContainer(containerCode);
				if (!lockResult.IsSuccessed)
					return new BindingPickContainerResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	[A]=取得第一筆符合的綁定中揀貨容器資料
				var bindingPickContainerInfo = f0701Repo.GetBindingPickContainerInfo(dcCode, gupCode, custCode, containerCode);

				//(3)	如果[A] 不存在
				if (bindingPickContainerInfo == null)
					return new BindingPickContainerResult { IsSuccessed = false, Message = $"揀貨容器{ containerCode }不存在" };

				//(4)	如果[A]存在，且[A].TOTAL=0
				if (bindingPickContainerInfo.TOTAL == 0)
				{
					//A.	新增F0536，資料來源[A]，F0536.STATUS=9(空箱釋放)
					f0536Repo.Add(new F0536
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						DC_CODE = bindingPickContainerInfo.DC_CODE,
						GUP_CODE = bindingPickContainerInfo.GUP_CODE,
						CUST_CODE = bindingPickContainerInfo.CUST_CODE,
						PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
						CONTAINER_CODE = bindingPickContainerInfo.CONTAINER_CODE,
						MOVE_OUT_TARGET = bindingPickContainerInfo.MOVE_OUT_TARGET,
						DEVICE_TYPE = bindingPickContainerInfo.DEVICE_TYPE,
						TOTAL = bindingPickContainerInfo.TOTAL,
						STATUS = "9",
						HAS_CP_ITEM = bindingPickContainerInfo.HAS_CP_ITEM
					});

					//B.	更新F0534.STATUS=9(空箱釋放)
					f0534Repo.UpdateStatusById(bindingPickContainerInfo.F0534_ID, "9");

          //C.	呼叫容器釋放共用函數
          ContainerService.DelContainer(dcCode, gupCode, custCode, bindingPickContainerInfo.PICK_ORD_NO);

					//D.	新增F053602 (Log)
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = containerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "1",
						MESSAGE = $"此揀貨容器無任何商品，不需要進行分貨，已將此揀貨容器釋放",
					};
					f053602Repo.Add(f053602);
					//E.	回傳訊息[此揀貨容器無任何商品，不需要進行分貨，已將此揀貨容器釋放]
					return new BindingPickContainerResult { IsSuccessed = true, Message = f053602.MESSAGE, IsReleaseContainer = true };
				}
				//(5)	如果[A]存在，且[A].TOTAL>0，往下執行
				//(6)	[B]=檢查是否有產生過跨庫調撥揀貨容器分貨頭檔
				var f0536 = f0536Repo.GetDataByF0701Id(bindingPickContainerInfo.F0701_ID);

				//(7)	如果[B]存在
				if (f0536 != null)
				{
					//A.	[C]=取得跨庫調撥揀貨容器分貨明細檔
					var bindingPickContainerDetails = f053601Repo.GetBindingPickContainerDetails(bindingPickContainerInfo.F0701_ID).ToList();
					//B.	[A]=[B]
					bindingPickContainerInfo.MOVE_OUT_TARGET = f0536.MOVE_OUT_TARGET;
					bindingPickContainerInfo.DEVICE_TYPE = f0536.DEVICE_TYPE;
					bindingPickContainerInfo.TOTAL = f0536.TOTAL;
					bindingPickContainerInfo.HAS_CP_ITEM = f0536.HAS_CP_ITEM;
					//C.	[A].ItemList = [C]
					bindingPickContainerInfo.ItemList = bindingPickContainerDetails;
					// 揀貨容器含有取消訂單商品，再檢查一次是否取消訂單商品已經分貨完成，若是，就不用綁定取消容器
					if (f0536.HAS_CP_ITEM == "1")
					{
						//A.	[G]=取得揀貨單是否已經產生跨庫調撥揀貨單含有取消訂單分貨明細檔
						var f0537s = f0537Repo.GetDatasByPickOrdNo(bindingPickContainerInfo.DC_CODE,
																	bindingPickContainerInfo.GUP_CODE,
																	bindingPickContainerInfo.CUST_CODE,
																	bindingPickContainerInfo.PICK_ORD_NO).ToList();
						var itemCodes = bindingPickContainerInfo.ItemList.Select(s => s.ITEM_CODE).Distinct().ToList();
						bindingPickContainerInfo.HAS_CP_ITEM = GetHasCpItem(f0537s, itemCodes);
						bindingPickContainerInfo.ALL_CP_ITEM = GetAllCpItem(f0537s, itemCodes, bindingPickContainerDetails);
					}

					//D.	新增F053602 (Log)
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = containerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "1",
						MESSAGE = "開始分貨",
					};
					f053602Repo.Add(f053602);
					return new BindingPickContainerResult { IsSuccessed = true, BindingPickContainerInfo = bindingPickContainerInfo };
				}
				//(8)	如果[B]不存在，往下執行
				//(9)	[D]=檢查揀貨容器是否正在被跨出整箱出貨功能使用中
				var f0530 = f0530Repo.GetF0530s(dcCode, gupCode, custCode, new List<long> { bindingPickContainerInfo.F0701_ID }).FirstOrDefault();

				//(10)	如果[D]存在
				//A.	如果[D].WORK_TYPE=0
				if (f0530 != null && f0530.WORK_TYPE == "0")
					return new BindingPickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }已在[跨庫調撥整箱出貨功能]使用並放入未關箱調撥箱內" };

				//(12)	[E] = 取得容器明細資料
				var pickContainerDetails = f070101Repo.GetContainerDetailByF0701Id(bindingPickContainerInfo.F0701_ID).ToList();

				//(13)	新增F053601跨庫調撥揀貨容器分貨明細檔
				foreach (var pickContainerDetail in pickContainerDetails)
				{
					f053601Repo.Add(new F053601
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						ITEM_CODE = pickContainerDetail.ITEM_CODE,
						B_SET_QTY = pickContainerDetail.B_SET_QTY,
						A_SET_QTY = 0,
						STATUS = "0"
					});
				}
				//(14)	設定[A].ItemList = [E]
				bindingPickContainerInfo.ItemList = pickContainerDetails;
				//(15)	[F]=檢查揀貨單是否含有取消出貨單
				var cancelWmsOrdNos = f0535Repo.CheckPickOrdNoIsCancel(bindingPickContainerInfo.DC_CODE,
																		bindingPickContainerInfo.GUP_CODE,
																		bindingPickContainerInfo.CUST_CODE,
																		bindingPickContainerInfo.PICK_ORD_NO);
				//(16)	如果[F]有取消訂單
				if (cancelWmsOrdNos != null && cancelWmsOrdNos.Any())
				{
					//A.	[G]=取得揀貨單是否已經產生跨庫調撥揀貨單含有取消訂單分貨明細檔
					var f0537s = f0537Repo.GetDatasByPickOrdNo(bindingPickContainerInfo.DC_CODE,
																bindingPickContainerInfo.GUP_CODE,
																bindingPickContainerInfo.CUST_CODE,
																bindingPickContainerInfo.PICK_ORD_NO).ToList();
					var itemCodes = bindingPickContainerInfo.ItemList.Select(s => s.ITEM_CODE).Distinct().ToList();

					//D.	如果[G]不存在
					if (f0537s == null || !f0537s.Any())
					{
						//a.	[H]=取得揀貨單取單取消明細
						var f051202s = f051202Repo.GetDatasWithPickByWmsOrdNos(bindingPickContainerInfo.DC_CODE,
																		bindingPickContainerInfo.GUP_CODE,
																		bindingPickContainerInfo.CUST_CODE,
																		bindingPickContainerInfo.PICK_ORD_NO,
																		cancelWmsOrdNos.ToList()).ToList();
						//b.	新增F0537 跨庫調撥揀貨單含有取消訂單分貨明細檔
						foreach (var f051202 in f051202s)
						{
							var f0537 = new F0537
							{
								DC_CODE = f051202.DC_CODE,
								GUP_CODE = f051202.GUP_CODE,
								CUST_CODE = f051202.CUST_CODE,
								PICK_ORD_NO = f051202.PICK_ORD_NO,
								PICK_ORD_SEQ = f051202.PICK_ORD_SEQ,
								WMS_ORD_NO = f051202.WMS_ORD_NO,
								WMS_ORD_SEQ = f051202.WMS_ORD_SEQ,
								ITEM_CODE = f051202.ITEM_CODE,
								B_SET_QTY = f051202.B_PICK_QTY,
								A_SET_QTY = 0,
								STATUS = "0"
							};
							f0537s.Add(f0537);
							f0537Repo.Add(f0537);
						}
					}
					bindingPickContainerInfo.HAS_CP_ITEM = GetHasCpItem(f0537s, itemCodes);
					bindingPickContainerInfo.ALL_CP_ITEM = GetAllCpItem(f0537s, itemCodes, pickContainerDetails);
				}
				//(17)	如果[F]無取消訂單
				else
				{
					//A.	設定[A]. 是否含有訂單取消商品 = 0(否)
					bindingPickContainerInfo.HAS_CP_ITEM = "0";
				}
				//(11)	如果[D]不存在
				//A.	新增F0530資料，資料來源從[A]寫入，WORK_TYPE設定為1
				if (f0530 == null)
				{
					f0530 = new F0530
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						DC_CODE = bindingPickContainerInfo.DC_CODE,
						GUP_CODE = bindingPickContainerInfo.GUP_CODE,
						CUST_CODE = bindingPickContainerInfo.CUST_CODE,
						PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
						CONTAINER_CODE = bindingPickContainerInfo.CONTAINER_CODE,
						MOVE_OUT_TARGET = bindingPickContainerInfo.MOVE_OUT_TARGET,
						DEVICE_TYPE = bindingPickContainerInfo.DEVICE_TYPE,
						TOTAL = bindingPickContainerInfo.TOTAL,
						WORK_TYPE = "1",
					};
					f0530Repo.Add(f0530);
				}
				//(19)	新增F0536
				f0536Repo.Add(new F0536
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					DC_CODE = bindingPickContainerInfo.DC_CODE,
					GUP_CODE = bindingPickContainerInfo.GUP_CODE,
					CUST_CODE = bindingPickContainerInfo.CUST_CODE,
					PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
					CONTAINER_CODE = bindingPickContainerInfo.CONTAINER_CODE,
					MOVE_OUT_TARGET = bindingPickContainerInfo.MOVE_OUT_TARGET,
					DEVICE_TYPE = bindingPickContainerInfo.DEVICE_TYPE,
					TOTAL = bindingPickContainerInfo.TOTAL,
					STATUS = "0",
					HAS_CP_ITEM = bindingPickContainerInfo.HAS_CP_ITEM
				});
				//(20)	新增F053602 (Log)
				f053602 = new F053602
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					SCAN_CODE = containerCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					IS_PASS = "1",
					MESSAGE = "開始分貨",
				};
				f053602Repo.Add(f053602);
				return new BindingPickContainerResult { IsSuccessed = true, BindingPickContainerInfo = bindingPickContainerInfo };
			}
			catch (Exception ex)
			{
				return new BindingPickContainerResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockContainerProcess(new List<string>() { containerCode });
			}
		}

		/// <summary>
		/// 刷讀正常出貨跨庫箱號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="containerCode"></param>
		/// <param name="bindingPickContainerInfo"></param>
		/// <returns></returns>
		public OutContainerResult ScanNormalContainerCode(string dcCode, string gupCode, string custCode, string containerCode, BindingPickContainerInfo bindingPickContainerInfo)
		{
			try
			{
				var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				F053602 f053602 = null;
				string f053602_Msg = string.Empty;

				//(1)	跨庫箱號鎖定
				var lockResult = LockOutContainer(containerCode);
				if (!lockResult.IsSuccessed)
					return new OutContainerResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	[A] =取得跨庫箱號檢核設定[F0003 .SYS_PATH WHERE AP_NAME=<參數1> AND AP_NAME= CrossDCContainer]
				var crossDCContainer = CommonService.GetSysGlobalValue(dcCode, "CrossDCContainer");

				//(3)	如果<參數6>的開頭不是[A]，
				if (crossDCContainer != null && !containerCode.StartsWith(crossDCContainer))
				{
					//A.	新增F053602
					f053602_Msg = $"跨庫的容器條碼開頭必須是{ crossDCContainer }";
					if (bindingPickContainerInfo != null)
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = containerCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = f053602_Msg,
						};
						AddFaliureLog(f053602);
					}
					//B.	回傳訊息[跨庫箱號必須開頭為[A]]
					return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
				}
				//(4)	如果<參數6>的開頭是[A]，往下執行
				//(5)	[B] = 檢查跨庫箱號是否正在使用中
				var outContainerInfo = f0531Repo.GetOutContainerInfo(dcCode, containerCode);
				//如果[B]存在
				if (outContainerInfo != null)
				{
					//(6)	如果[B]存在，但SOW_TYPE=1(取消訂單)，則顯示[此跨庫箱號XXX為取消訂單使用中的容器，不可使用]
					if (outContainerInfo.SOW_TYPE == "1")
					{
						//A.	新增F053602
						f053602_Msg = $"此跨庫箱號為取消訂單使用中的容器，不可使用";
						if (bindingPickContainerInfo != null)
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = containerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = f053602_Msg,
							};
							AddFaliureLog(f053602);
						}
						//B.	回傳[此跨庫箱號已被[取消出貨容器]使用中，不可使用]
						return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
					}

					//(7)	如果[B]存在，且[B].GUP_CODE <> <參數2>
					if (outContainerInfo.GUP_CODE != gupCode)
					{
						//A.	新增F053602
						f053602_Msg = $"此跨庫箱號已被其他業主綁定使用中，不可使用";
						if (bindingPickContainerInfo != null)
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = containerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = f053602_Msg,
							};
							AddFaliureLog(f053602);
						}
						//B.	回傳[此跨庫箱號已被其他業主綁定使用中，不可使用]
						return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
					}

					//(8)	如果[B]存在，且[B].CUST_CODE <> <參數3>
					if (outContainerInfo.CUST_CODE != custCode)
					{
						//A.	新增F053602
						f053602_Msg = $"此跨庫箱號已被其他貨主綁定使用中，不可使用";
						if (bindingPickContainerInfo != null)
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = containerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = f053602_Msg,
							};
							AddFaliureLog(f053602);
						}
						//B.	回傳[此跨庫箱號已被其他貨主綁定使用中，不可使用]
						return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
					}

					//(9)	如果[B]存在，且[B].WORK_TYPE=0
					if (outContainerInfo.WORK_TYPE == "0")
					{
						//A.	新增F053602
						f053602_Msg = $"此跨庫箱號已被[跨庫訂單整箱出庫]功能綁定使用中，不可使用";
						if (bindingPickContainerInfo != null)
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = containerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = f053602_Msg,
							};
							AddFaliureLog(f053602);
						}
						//B.	回傳[此跨庫箱號已被[跨庫訂單整箱出庫]功能綁定使用中，不可使用]
						return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
					}

					if (outContainerInfo.WORK_TYPE == "1" && bindingPickContainerInfo == null)
						return new OutContainerResult { IsSuccessed = true, OutContainerInfo = outContainerInfo };

					//(10)	如果[B]存在，且 [B].WORK_TYPE=1 AND MOVE_OUT_TARGET <> <參數4>
					if (outContainerInfo.WORK_TYPE == "1" && outContainerInfo.MOVE_OUT_TARGET != bindingPickContainerInfo.MOVE_OUT_TARGET)
					{
						//A.	新增F053602
						f053602_Msg = $"此跨庫箱號綁定的目的地與揀貨容器不同，不可使用";
						if (bindingPickContainerInfo != null)
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = containerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = f053602_Msg,
							};
							AddFaliureLog(f053602);
						}
						//B.	回傳[此跨庫箱號綁定的目的地與揀貨容器不同，不可使用]
						return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
					}

					//(11)	如果[B]存在，且[B].WORK_TYPE=1 AND MOVE_OUT_TARGET =<參數4>
					if (outContainerInfo.WORK_TYPE == "1" && outContainerInfo.MOVE_OUT_TARGET == bindingPickContainerInfo.MOVE_OUT_TARGET)
					{
						//此跨庫箱號是否有綁過揀貨容器
						var f053201 = f053201Repo.GetDataByF0531AndF0701Id(outContainerInfo.F0531_ID, bindingPickContainerInfo.F0701_ID);
						//F053201不存在
						if (f053201 == null)
						{
							//新增F053201
							f053201 = new F053201
							{
								F0531_ID = outContainerInfo.F0531_ID,
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								DC_CODE = dcCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
								CONTAINER_CODE = bindingPickContainerInfo.CONTAINER_CODE,
								STATUS = "0",
							};
							f053201Repo.Add(f053201);
						}

						//A.	新增F053602
						f053602_Msg = $"跨庫箱號綁定完成";
						if (bindingPickContainerInfo != null)
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = containerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "1",
								MESSAGE = f053602_Msg,
							};
							f053602Repo.Add(f053602);
						}
						//B.	回傳資料
						return new OutContainerResult { IsSuccessed = true, OutContainerInfo = outContainerInfo };
					}
				}
				//(12)	如果[B]不存在，往下執行
				else
				{
					//(13)	[C]= 檢查跨庫箱號是否存在已關箱未出貨
					var f0532_NotShipData = f0532Repo.GetCloseContainer(dcCode, containerCode);

					if (f0532_NotShipData != null)
					{
						//(14)	如果[C]存在，[C].SOW_TYPE=0
						if (f0532_NotShipData.SOW_TYPE == "0")
						{
							//A.	新增F053602
							f053602_Msg = $"此跨庫箱號已關箱，不可使用";
							if (bindingPickContainerInfo != null)
							{
								f053602 = new F053602
								{
									F0701_ID = bindingPickContainerInfo.F0701_ID,
									SCAN_CODE = containerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "0",
									MESSAGE = f053602_Msg,
								};
								AddFaliureLog(f053602);
							}
							//B.	回傳訊息[此跨庫箱號已關箱，不可使用]
							return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
						}

						//(15)	如果[C]存在，[C].SOW_TYPE=1
						if (f0532_NotShipData.SOW_TYPE == "1")
						{
							//A.	新增F053602
							f053602_Msg = $"此跨庫箱號已被訂單取消使用並關箱，不可使用";
							if (bindingPickContainerInfo != null)
							{
								f053602 = new F053602
								{
									F0701_ID = bindingPickContainerInfo.F0701_ID,
									SCAN_CODE = containerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "0",
									MESSAGE = f053602_Msg,
								};
								AddFaliureLog(f053602);
							}
							//B.	回傳訊息[此跨庫箱號已被訂單取消使用並關箱，不可使用]
							return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
						}
					}
					if (bindingPickContainerInfo == null)
						return new OutContainerResult { IsSuccessed = false, Message = "未刷入揀貨容器，不可綁定跨庫箱號" };

					//(16)	如果[C]不存在，往下執行
					//(17)	[D] =F0531 取號
					var newF0531Id = GetF0531NextId();
					//(18)	[E] = 取得箱序
					var newContainerSeq = f0532Repo.GetNewContainerSeq("0");
					//(19)	如果[E]無資料，[E]=1
					if (newContainerSeq == 0)
						newContainerSeq = 1;

					//(20)	新增F0531
					var f0531 = new F0531
					{
						ID = newF0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						OUT_CONTAINER_SEQ = newContainerSeq,
						MOVE_OUT_TARGET = bindingPickContainerInfo.MOVE_OUT_TARGET,
						TOTAL = 0,
						WORK_TYPE = "1",
						SOW_TYPE = "0",
						STATUS = "0",
					};
					f0531Repo.Add(f0531);

					//(21)	新增F0532
					var f0532 = new F0532
					{
						F0531_ID = newF0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						OUT_CONTAINER_SEQ = newContainerSeq,
						MOVE_OUT_TARGET = bindingPickContainerInfo.MOVE_OUT_TARGET,
						TOTAL = 0,
						WORK_TYPE = "1",
						SOW_TYPE = "0",
						STATUS = "0",
					};
					f0532Repo.Add(f0532);

					//(22)	新增F053201
					var f053201 = new F053201
					{
						F0531_ID = newF0531Id,
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
						CONTAINER_CODE = bindingPickContainerInfo.CONTAINER_CODE,
						STATUS = "0",
					};
					f053201Repo.Add(f053201);

					//(23)	新增F053602
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = containerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "1",
						MESSAGE = $"跨庫箱號綁定完成",
					};
					f053602Repo.Add(f053602);

					//(24)	新增F0533
					var f0533 = new F0533
					{
						F0531_ID = newF0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						MOVE_OUT_TARGET = bindingPickContainerInfo.MOVE_OUT_TARGET,
						WORK_TYPE = "1",
						STATUS = "0",
					};
					f0533Repo.Add(f0533);

					//(25)	回傳資料
					return new OutContainerResult
					{
						IsSuccessed = true,
						ContainerCode = containerCode,
						OutContainerInfo = new OutContainerInfo
						{
							F0531_ID = newF0531Id,
							DC_CODE = dcCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							OUT_CONTAINER_CODE = containerCode,
							MOVE_OUT_TARGET = bindingPickContainerInfo.MOVE_OUT_TARGET,
							CROSS_NAME = bindingPickContainerInfo.CROSS_NAME,
							WORK_TYPE = "1",
							STATUS = "0",
							CRT_DATE = DateTime.Now,
							TOTAL = 0,
						},
						MoveOutTargetName = "未綁定",
						TotalPcs = 0
					};
				}

				f053602 = new F053602
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					SCAN_CODE = containerCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					IS_PASS = "0",
					MESSAGE = $"此跨庫箱號{ containerCode }作業類型異常，不可使用",
				};
				AddFaliureLog(f053602);
				//回傳揀貨容器資訊
				return new OutContainerResult { IsSuccessed = false, Message = $"此跨庫箱號{ containerCode }作業類型異常，不可使用" };
			}
			catch (Exception ex)
			{
				return new OutContainerResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockOutContainerProcess(new List<string>() { containerCode });
			}
		}

		/// <summary>
		/// 刷讀取消訂單容器條碼
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="containerCode"></param>
		/// <param name="bindingPickContainerInfo"></param>
		/// <returns></returns>
		public OutContainerResult ScanCancelContainerCode(string dcCode, string gupCode, string custCode, string containerCode, BindingPickContainerInfo bindingPickContainerInfo)
		{
			try
			{
				var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				F053602 f053602 = null;
				string f053602_Msg = string.Empty;

				//(1)	取消訂單容器條碼鎖定
				var lockResult = LockCancelContainer(containerCode);
				if (!lockResult.IsSuccessed)
					return new OutContainerResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	[A]=檢查容器條碼是否存在F0701
				var f0701 = f0701Repo.GetDataByTypeAndContainer(dcCode, "0", containerCode);

				//(3)	如果[A]存在
				if (f0701 != null)
				{
					//A.	[B]=檢查是否有綁定取消訂單容器但未關箱
					var f0531 = f0531Repo.GetDataByF0701Id(f0701.ID);
					if (f0531 != null)
					{
						//[B].SOW_TYPE = 0
						if (f0531.SOW_TYPE == "0")
						{
							//a.	新增F053602
							f053602_Msg = $"此容器已被正常出貨跨庫箱綁定，不可使用";
							if (bindingPickContainerInfo != null)
							{
								f053602 = new F053602
								{
									F0701_ID = bindingPickContainerInfo.F0701_ID,
									SCAN_CODE = containerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "0",
									MESSAGE = f053602_Msg,
								};
								AddFaliureLog(f053602);
							}
							//b.	回傳[此容器已被正常出貨跨庫箱號使用中，不可使用]
							return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
						}

						//B.	如果[B]存在，[B].GUP_CODE <> <參數2>
						if (f0531.GUP_CODE != gupCode)
						{
							//a.	新增F053602
							f053602_Msg = $"此容器已被其他業主綁定使用中，不可使用";
							if (bindingPickContainerInfo != null)
							{
								f053602 = new F053602
								{
									F0701_ID = bindingPickContainerInfo.F0701_ID,
									SCAN_CODE = containerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "0",
									MESSAGE = f053602_Msg,
								};
								AddFaliureLog(f053602);
							}
							//b.	回傳[此容器已被其他業主綁定使用中，不可使用]
							return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
						}
						//C.	如果[B]存在，[B].CUST_CODE <> <參數3>
						if (f0531.CUST_CODE != custCode)
						{
							//a.	新增F053602
							f053602_Msg = $"此容器已被其他貨主綁定使用中，不可使用";
							if (bindingPickContainerInfo != null)
							{
								f053602 = new F053602
								{
									F0701_ID = bindingPickContainerInfo.F0701_ID,
									SCAN_CODE = containerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "0",
									MESSAGE = f053602_Msg,
								};
								AddFaliureLog(f053602);
							}
							//b.	回傳[此容器已被其他貨主綁定使用中，不可使用]
							return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
						}
						//D.	如果[B]存在，[B].GUP_CODE =<參數2> AND [B].CUST_CODE=<參數3>
						if (f0531.GUP_CODE == gupCode && f0531.CUST_CODE == custCode)
						{
							//a.	新增F053602
							f053602_Msg = $"取消訂單容器綁定完成";
							if (bindingPickContainerInfo != null)
							{
								//此取消訂單容器條碼是否有綁過揀貨容器
								var f053201 = f053201Repo.GetDataByF0531AndF0701Id(f0531.ID, bindingPickContainerInfo.F0701_ID);
								//F053201不存在
								if (f053201 == null)
								{
									//新增F053201
									f053201 = new F053201
									{
										F0531_ID = f0531.ID,
										F0701_ID = bindingPickContainerInfo.F0701_ID,
										DC_CODE = dcCode,
										GUP_CODE = gupCode,
										CUST_CODE = custCode,
										PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
										CONTAINER_CODE = bindingPickContainerInfo.CONTAINER_CODE,
										STATUS = "0",
									};
									f053201Repo.Add(f053201);
								}

								f053602 = new F053602
								{
									F0701_ID = bindingPickContainerInfo.F0701_ID,
									SCAN_CODE = containerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "1",
									MESSAGE = f053602_Msg,
								};
								f053602Repo.Add(f053602);
							}
							//b.	回傳結果
							return new OutContainerResult
							{
								IsSuccessed = true,
								ContainerCode = containerCode,
								TotalPcs = f0531.TOTAL.Value,
								OutContainerInfo = new OutContainerInfo
								{
									OUT_CONTAINER_CODE = containerCode,
									F0531_ID = f0531.ID,
									F0701_ID = f0531.F0701_ID.Value,
									TOTAL = f0531.TOTAL.Value,
									CRT_DATE = f0531.CRT_DATE,
								}
							};
						}
					}
					//E.	如果[B]不存在
					else
					{
						//a.	[M]=檢查是否有綁定取消訂單容器但已關箱未解除綁定
						var f0532 = f0532Repo.GetDataByF0701Id(f0701.ID);
						//b.	如果[M]存在
						if (f0532 != null)
						{
							//a.	新增F053602
							f053602_Msg = $"此取消訂單容器條碼已關箱未解除綁定，不可使用";
							if (bindingPickContainerInfo != null)
							{
								f053602 = new F053602
								{
									F0701_ID = bindingPickContainerInfo.F0701_ID,
									SCAN_CODE = containerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "0",
									MESSAGE = f053602_Msg,
								};
								AddFaliureLog(f053602);
							}
							//b.	回傳[此取消訂單容器條碼已關箱未解除綁定，不可使用]
							return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
						}
						//c.	如果[M]不存在
						else
						{
							//a.	新增F053602
							f053602_Msg = $"此容器已被使用，不可使用";
							if (bindingPickContainerInfo != null)
							{
								f053602 = new F053602
								{
									F0701_ID = bindingPickContainerInfo.F0701_ID,
									SCAN_CODE = containerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "0",
									MESSAGE = f053602_Msg,
								};
								AddFaliureLog(f053602);
							}
							//b.	回傳[此容器已被使用，不可使用]
							return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
						}
					}

				}
				//如果[A]不存在
				else
				{
					//(4)	如果[A]不存在，檢查容器號碼是否在進行中的容器分貨類型非取消訂單
					//A.	資料表:F0531
					var f0531_NormalContainer = f0531Repo.GetDataByContainerCode(dcCode, containerCode, "0");
					//C.	如果有資料
					if (f0531_NormalContainer != null)
					{
						//a.	新增F053602
						f053602_Msg = $"此容器已綁定正常出貨使用中的容器，不可使用";
						if (bindingPickContainerInfo != null)
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = containerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = f053602_Msg,
							};
							AddFaliureLog(f053602);
						}
						//b.	回傳[此容器已綁定正常出貨使用中的容器，不可使用]
						return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
					}

					//(5)	如果[A]不存在，檢查容器號碼是否已關箱的容器分貨類型非取消訂單
					//A.	資料表:F0532
					var f0532_NormalContainer = f0532Repo.GetCloseContainer(dcCode, containerCode, "0");
					//C.	如果有資料
					if (f0532_NormalContainer != null)
					{
						//a.	新增F053602
						f053602_Msg = $"此容器已綁定正常出貨已關箱的容器，不可使用";
						if (bindingPickContainerInfo != null)
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = containerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = f053602_Msg,
							};
							AddFaliureLog(f053602);
						}
						//b.	回傳[此容器已綁定正常出貨已關箱的容器，不可使用]
						return new OutContainerResult { IsSuccessed = false, Message = f053602_Msg };
					}

					if (bindingPickContainerInfo == null)
						return new OutContainerResult { IsSuccessed = false, Message = "未刷入揀貨容器，不可綁定跨庫箱號" };

					//A.	[C] = F0701 ID取號
					var newF0701Id = ContainerService.GetF0701NextId();
					//B.	[D] = F0531 ID取號
					var newF0531Id = GetF0531NextId();
					//C.	[E] = 取得箱序
					var newContainerSeq = f0532Repo.GetNewContainerSeq("1");
					//D.	如果[E]無資料，[E]=1
					if (newContainerSeq == 0)
						newContainerSeq = 1;
					//E.	新增F0701
					f0701 = new F0701
					{
						ID = newF0701Id,
						CONTAINER_CODE = containerCode,
						DC_CODE = dcCode,
						CUST_CODE = custCode,
						WAREHOUSE_ID = "NA",
						CONTAINER_TYPE = "0",
					};
					f0701Repo.Add(f0701);
					//F.	新增F0531
					var f0531 = new F0531
					{
						ID = newF0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						OUT_CONTAINER_SEQ = newContainerSeq,
						MOVE_OUT_TARGET = null,
						TOTAL = 0,
						WORK_TYPE = "1",
						SOW_TYPE = "1",
						STATUS = "0",
						F0701_ID = newF0701Id,
					};
					f0531Repo.Add(f0531);
					//G.	新增F0532
					var f0532 = new F0532
					{
						F0531_ID = newF0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						OUT_CONTAINER_SEQ = newContainerSeq,
						MOVE_OUT_TARGET = null,
						TOTAL = 0,
						WORK_TYPE = "1",
						SOW_TYPE = "1",
						STATUS = "0",
						F0701_ID = newF0701Id,
					};
					f0532Repo.Add(f0532);
					//H.	新增F053201
					var f053201 = new F053201
					{
						F0531_ID = newF0531Id,
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
						CONTAINER_CODE = bindingPickContainerInfo.CONTAINER_CODE,
						STATUS = "0",
					};
					f053201Repo.Add(f053201);
					//I.	新增F0533
					var f0533 = new F0533
					{
						F0531_ID = newF0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						WORK_TYPE = "1",
						STATUS = "0",
					};
					f0533Repo.Add(f0533);
					//J.	新增F053602
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = containerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "1",
						MESSAGE = $"取消訂單容器綁定完成",
					};
					f053602Repo.Add(f053602);
					//
					return new OutContainerResult
					{
						IsSuccessed = true,
						ContainerCode = containerCode,
						TotalPcs = 0,
						OutContainerInfo = new OutContainerInfo
						{
							OUT_CONTAINER_CODE = containerCode,
							F0531_ID = newF0531Id,
							F0701_ID = newF0701Id,
							TOTAL = 0,
							CRT_DATE = DateTime.Now,
						}
					};

				}

				//回傳揀貨容器資訊
				return new OutContainerResult { IsSuccessed = false, Message = $"此取消訂單容器{ containerCode }作業類型異常，不可使用" };
			}
			catch (Exception ex)
			{
				return new OutContainerResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockCancelContainerProcess(new List<string>() { containerCode });
			}
		}

		/// <summary>
		/// 刷讀商品條碼
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemBarcode"></param>
		/// <param name="bindingPickContainerInfo"></param>
		/// <param name="normalContainer"></param>
		/// <param name="cancelContainer"></param>
		/// <returns></returns>
		public ScanItemBarcodeResult ScanItemBarcodeFromP080806(string dcCode, string gupCode, string custCode, string itemBarcode,
														BindingPickContainerInfo bindingPickContainerInfo,
														OutContainerInfo normalContainer, OutContainerInfo cancelContainer)
		{
			var itemService = new ItemService();
			var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
			//(1)	以下回傳失敗都要新增F053602(Log)
			var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, wmsTransaction);
			var f053601Repo = new F053601Repository(Schemas.CoreSchema, wmsTransaction);
			var f0537Repo = new F0537Repository(Schemas.CoreSchema, wmsTransaction);
			var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
			//var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);
			var f053202Repo = new F053202Repository(Schemas.CoreSchema, wmsTransaction);
			var f0536Repo = new F0536Repository(Schemas.CoreSchema, wmsTransaction);
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
			var f0530Repo = new F0530Repository(Schemas.CoreSchema, wmsTransaction);
			var f0534Repo = new F0534Repository(Schemas.CoreSchema, wmsTransaction);
			F053602 f053602;

			//揀貨容器所有商品全取消時不會刷讀正常稽核箱(normalContainer=null)
			if (normalContainer != null)
			{
				//(2)	[A] = 檢查正常出貨稽核箱號是否已關箱或出貨
				var normalF0532 = f0532Repo.GetCloseOrShipDataByF0531Id(normalContainer.F0531_ID);
				//(3)	如果[A]有資料
				if (normalF0532 != null)
				{
					//A.	回傳失敗訊息[稽核箱已關箱或出貨，請重新綁定新的稽核箱號]
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = itemBarcode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "0",
						MESSAGE = $"稽核箱已關箱或出貨，請重新綁定新的稽核箱號",
					};
					AddFaliureLog(f053602);
					return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE, bindNewNormalContainer = true };
				}
			}
			//(4)	如果[A]無資料，往下執行
			//(5)	如果<參數7> = true
			if (bindingPickContainerInfo.HAS_CP_ITEM == "1")
			{
				//A.	[B] = 檢查取消訂單容器是否已關箱或出貨
				var cancelF0532 = f0532Repo.GetCloseOrShipDataByF0531Id(cancelContainer.F0531_ID);
				//B.	如果[B]有資料
				if (cancelF0532 != null)
				{
					//A.	回傳失敗訊息[取消訂單容器已關箱或出貨，請重新綁定新的容器]
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = itemBarcode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "0",
						MESSAGE = $"取消訂單容器已關箱或出貨，請重新綁定新的容器",
					};
					AddFaliureLog(f053602);
					return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE, bindNewCancelContainer = true };
				}
				//C.	如果[B]無資料，往下執行
			}
			//(6)	如果<參數7>=false，往下執行
			//(7)	[C] =檢查商品條碼是否為揀貨單商品條碼
			var checkItem = bindingPickContainerInfo.ItemList.Where(w => w.ITEM_CODE == itemBarcode || w.EAN_CODE1 == itemBarcode
																		|| w.EAN_CODE2 == itemBarcode || w.EAN_CODE3 == itemBarcode);
			List<string> currentItemCodeList = null;
			F2501 f2501 = null;
			//(8)	如果[C]存在
			if (checkItem.Any())
			{
				//A.	[D] 符合的商品品號清單= <參數4>.ITEM_CODE
				currentItemCodeList = checkItem.Select(s => s.ITEM_CODE).ToList();
				//B.	往下執行
			}
			//(9)	如果[C]不存在，檢查是否為序號
			else
			{
				//A.	[E] = 呼叫ItemService. FindItems(<參數2>,<參數3>,<參數10>,回傳f2501)
				var findItemCodes = itemService.FindItems(gupCode, custCode, itemBarcode, ref f2501);
				//B.	如果[E]無資料
				if (findItemCodes == null || !findItemCodes.Any())
				{
					//a.	回傳失敗訊息[商品條碼不存在]
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = itemBarcode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "0",
						MESSAGE = $"商品條碼不存在",
					};
					AddFaliureLog(f053602);
					return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE };
				}
				//C.	如果[E]存在
				else
				{
					//a.	[F]=檢查是否存在此揀貨單商品
					checkItem = bindingPickContainerInfo.ItemList.Where(w => findItemCodes.Contains(w.ITEM_CODE));
					//b.	[F]不存在，回傳錯誤訊息[刷讀的商品條碼非此揀貨單商品]
					if (!checkItem.Any())
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = itemBarcode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"刷讀的商品條碼非此揀貨單商品",
						};
						AddFaliureLog(f053602);
						return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE };
					}
					//c.	 [F]存在，[D] 符合的商品品號清單=[F]，往下執行
					currentItemCodeList = checkItem.Select(s => s.ITEM_CODE).ToList();
				}
				//D.	[G]=取得[E]有回傳F2501(序號) 
				//E.	如果[G]存在，進行序號檢核
				if (f2501 != null)
				{
					//a.	檢查序號狀態是否在庫，[G].STATUS <> A1，回傳錯誤訊息[此商品序號非在庫序號，不可出貨]
					if (f2501.STATUS != "A1")
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = itemBarcode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此商品序號非在庫序號，不可出貨",
						};
						AddFaliureLog(f053602);
						return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE };
					}
					//b.	檢查序號是否為不良品序號，[G]. ACTIVATED = 1，回傳錯誤訊息[此商品序號為不良品序號，不可出貨]
					if (f2501.ACTIVATED == "1")
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = itemBarcode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此商品序號為不良品序號，不可出貨",
						};
						AddFaliureLog(f053602);
						return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE };
					}
					//c.	檢查序號是否凍結，呼叫F2501Repoistory. GetSerialIsFreeze(<參數2>,<參數3>,02,[G].SERIAL_NO)，如果序號凍結，回傳錯誤訊息[此商品序號已凍結，不可出貨]
					var f250102s = f2501Repo.GetSerialIsFreeze(gupCode, custCode, "02", new List<string> { f2501.SERIAL_NO });
					if (f250102s.Any())
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = itemBarcode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此商品序號已凍結，不可出貨",
						};
						AddFaliureLog(f053602);
						return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE };
					}
					//d.	[S]=檢查序號是否存在取消訂單非已出貨箱內明細中
					var isSnInCancelBox = f0532Repo.CheckSnInCancelBoxData(gupCode, custCode, f2501.SERIAL_NO);
					//e.	如果[S]存在，回傳錯誤訊息[此商品序號已放入取消訂單容器，不可出貨]
					if (isSnInCancelBox)
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = itemBarcode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此商品序號已放入取消訂單容器，不可出貨",
						};
						AddFaliureLog(f053602);
						return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE };
					}
					//f.	序號檢核正確，往下執行
				}
			}
			//F.	如果[G]不存在，往下執行
			//(10)	[K]=是否為正常出貨=false
			var isNormalShipItem = false;
			//(11)	[J]=取得第一筆符合可分貨商品分貨的資料
			var f053601 = f053601Repo.GetAllotDetail(bindingPickContainerInfo.F0701_ID, currentItemCodeList, "0");
			//A.	如果[J]無資料，回傳錯誤訊息[此商品已分貨完成]
			if (f053601 == null)
			{
				f053602 = new F053602
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					SCAN_CODE = itemBarcode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					IS_PASS = "0",
					MESSAGE = $"此商品已分貨完成",
				};
				AddFaliureLog(f053602);
				return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE };
			}
			//B.	如果[J]有資料，往下執行
			//(12)	[L]=檢查商品是否為序號商品
			var currentItem = bindingPickContainerInfo.ItemList.Find(w => w.ITEM_CODE == f053601.ITEM_CODE);
			//(13)	如果[L].BUNDLE_SERIAL_NO=1 AND [G]=NULL
			if (currentItem.BUNDLE_SERIALNO == "1" && f2501 == null)
			{
				//A.	回傳錯訊息[此商品為序號商品，請刷讀商品序號]
				f053602 = new F053602
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					SCAN_CODE = itemBarcode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					IS_PASS = "0",
					MESSAGE = $"此商品為序號商品，請刷讀商品序號",
				};
				AddFaliureLog(f053602);
				return new ScanItemBarcodeResult { IsSuccessed = false, Message = f053602.MESSAGE };
			}
			//(14)	如果<參數7>=true
			//A.	[H]=取得第一筆符合取消訂單商品分貨的資料
			var f0537 = f0537Repo.GetCancelAllotDetail(dcCode, gupCode, custCode, bindingPickContainerInfo.PICK_ORD_NO,
														new List<string> { f053601.ITEM_CODE }, "0");
			//B.	如果[H]存在
			if (bindingPickContainerInfo.HAS_CP_ITEM == "1" && f0537 != null)
			{
				//a.	設定[H]. A_SET_QTY +=1;
				f0537.A_SET_QTY += 1;
				//b.	如果[H].B_SET_QTY = [H].A_SET_QTY，[H].STATUS=1(分貨完成)
				if (f0537.B_SET_QTY == f0537.A_SET_QTY)
					f0537.STATUS = "1";
				//c.	更新F0537=[H]
				f0537Repo.Update(f0537);
				//d.	設定[J].A_SET_QTY+=1;
				f053601.A_SET_QTY += 1;
				//e.	如果[J].B_SET_QTY = [J].A_SET_QTY，[J].STATUS=1(分貨完成)
				if (f053601.B_SET_QTY == f053601.A_SET_QTY)
					f053601.STATUS = "1";
				//f.	更新F053601=[J]
				f053601Repo.Update(f053601);
				//g.	更新商品資料暫存檔實際分貨數[<參數4>.實際分貨數+=1]
				bindingPickContainerInfo.ItemList.Find(x => x.ITEM_CODE == f053601.ITEM_CODE).A_SET_QTY += 1;
				//h.	更新F0531.TOTAL+=1 WHERE ID=<參數9>
				f0531Repo.UpdateTotalAndTargetById(cancelContainer.F0531_ID, 1);
				//i.	更新F0532.TOTAL+=1 WHERE F0531_ID=<參數9>
				f0532Repo.UpdateTotalAndTargetById(cancelContainer.F0531_ID, 1);
				//j.	新增F053201
				var f053202 = new F053202
				{
					F0531_ID = cancelContainer.F0531_ID,
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
					ITEM_CODE = f053601.ITEM_CODE,
					SERIAL_NO = f2501 != null ? f2501.SERIAL_NO : null,
					QTY = 1,
					WMS_ORD_NO = f0537.WMS_ORD_NO,
					WMS_ORD_SEQ = f0537.WMS_ORD_SEQ
				};
				f053202Repo.Add(f053202);
				//k.	新增F053602 (log)
				f053602 = new F053602
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					SCAN_CODE = itemBarcode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					ITEM_CODE = f053601.ITEM_CODE,
					QTY = 1,
					SERIAL_NO = f2501 != null ? f2501.SERIAL_NO : null,
					IS_PASS = "1",
					MESSAGE = $"分貨到[取消訂單容器{ cancelContainer.OUT_CONTAINER_CODE }]商品成功",
				};
				f053602Repo.Add(f053602);
				//l.	[K]=false
				isNormalShipItem = false;
			}
			//C.	如果[H]不存在，往下執行
			//(15)	如果<參數7>=false 或[H]不存在
			else
			{
				//A.	如果序號[G]存在
				if (f2501 != null)
				{
					//a.	更新F2501.STATUS=C1 
					f2501.STATUS = "C1";
					f2501Repo.Update(f2501);
				}
				//B.	設定[J].A_SET_QTY+=1
				f053601.A_SET_QTY += 1;
				//C.	如果[J].B_SET_QTY = [J].A_SET_QTY，[J].STATUS=1(分貨完成)
				if (f053601.B_SET_QTY == f053601.A_SET_QTY)
					f053601.STATUS = "1";
				//D.	更新F053601=[J]
				f053601Repo.Update(f053601);
				//E.	更新商品資料暫存檔實際分貨數[<參數4>.實際分貨數+=1]
				bindingPickContainerInfo.ItemList.Find(x => x.ITEM_CODE == f053601.ITEM_CODE).A_SET_QTY += 1;
				//F.	更新F0531.TOTAL+=1 WHERE ID=<參數8>
				f0531Repo.UpdateTotalAndTargetById(normalContainer.F0531_ID, 1);
				//G.	更新F0532.TOTAL+=1 WHERE F0531_ID=<參數8>
				f0532Repo.UpdateTotalAndTargetById(normalContainer.F0531_ID, 1);
				//H.	新增F053201
				var f053202 = new F053202
				{
					F0531_ID = normalContainer.F0531_ID,
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					PICK_ORD_NO = bindingPickContainerInfo.PICK_ORD_NO,
					ITEM_CODE = f053601.ITEM_CODE,
					SERIAL_NO = f2501 != null ? f2501.SERIAL_NO : null,
					QTY = 1,
					WMS_ORD_NO = null,
					WMS_ORD_SEQ = null
				};
				f053202Repo.Add(f053202);
				//I.	新增F053602 (log)
				f053602 = new F053602
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					SCAN_CODE = itemBarcode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					ITEM_CODE = f053601.ITEM_CODE,
					QTY = 1,
					SERIAL_NO = f2501 != null ? f2501.SERIAL_NO : null,
					IS_PASS = "1",
					MESSAGE = $"分貨到[稽核箱號{ normalContainer.OUT_CONTAINER_CODE }]商品成功",
				};
				f053602Repo.Add(f053602);
				//J.	[K]=true
				isNormalShipItem = true;
			}
			//取得此揀貨容器所有分貨資料
			var allF053601 = f053601Repo.GetDatasByF0701Id(bindingPickContainerInfo.F0701_ID).ToList();
			//同步更新此次刷讀的資料
			allF053601.Find(x => x.ID == f053601.ID).A_SET_QTY = f053601.A_SET_QTY;
			//(16)	[S]=檢查揀貨容器是分貨完成
			//前端用Command執行刷讀時，bindingPickContainerInfo會有舊資料問題，導致無法分貨完成，前端改直接呼叫Function並將此段判斷改成依F053601資料判斷分貨結果
			var allPickAllotFinish = allF053601.All(a => a.B_SET_QTY - a.A_SET_QTY == 0); //bindingPickContainerInfo.ItemList.All(a => a.B_SET_QTY - a.A_SET_QTY == 0);

			//(17)	如果[S]=true(所有揀貨容器商品都分貨完成)
			if (allPickAllotFinish)
			{
				//A.	更新F0536.STATUS=1(分貨完成) 
				f0536Repo.UpdateStatusByF0701Id(bindingPickContainerInfo.F0701_ID, "1");
        //B.	揀貨容器釋放，呼叫容器釋放共用
        ContainerService.DelContainer(dcCode, gupCode, custCode, bindingPickContainerInfo.PICK_ORD_NO);
				//C.	使用中揀貨容器釋放，刪除F0530 WHERE F0701_ID=<參數5>
				f0530Repo.DeleteByF0701Id(bindingPickContainerInfo.F0701_ID);
				//D.	新增F053602(log)
				f053602 = new F053602
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					SCAN_CODE = "SowFinishBox",
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					ITEM_CODE = null,
					QTY = null,
					SERIAL_NO = null,
					IS_PASS = "1",
					MESSAGE = $"揀貨容器分貨完成",
				};
				f053602Repo.Add(f053602);

				//更新F0534.STATUS=1(已放入並關閉稽核箱待分配)
				f0534Repo.UpdatePartialCloseByF0701Id(bindingPickContainerInfo.F0701_ID);
			}
			//(18)	回傳結果
			return new ScanItemBarcodeResult
			{
				IsSuccessed = true,
				ITEM_CODE = f053601.ITEM_CODE,
				ITEM_NAME = currentItem.ITEM_NAME,
				IsNormalShipItem = isNormalShipItem,
				IsFinishAllot = allPickAllotFinish,
				BindingPickContainerInfo = bindingPickContainerInfo,
			};
		}


		/// <summary>
		/// 取得揀貨容器商品是否有含有取消訂單商品
		/// </summary>
		/// <param name="f0537s"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		public string GetHasCpItem(List<F0537> f0537s, List<string> itemCodes)
		{
			//B.	如果[G]存在，且[G]有任何一筆STATUS=0且[G].ITEM_CODE存在於[A].ItemList清單中
			//a.	設定[A]. 是否含有訂單取消商品 = 1(是)
			if (f0537s != null && f0537s.Any(a => a.STATUS == "0" && itemCodes.Contains(a.ITEM_CODE)))
				return "1";
			//C.	如果[G]存在，且[G]所有明細STATUS=1 或[G].ITEM_CODE不存在於[A].ItemList清單中
			//a.	設定[A]. 是否含有訂單取消商品 = 0(否)
			else if (f0537s != null && f0537s.Any() && (f0537s.All(x => x.STATUS == "1") || f0537s.All(a => !itemCodes.Contains(a.ITEM_CODE))))
				return "0";
			else
				return "0";
		}

		/// <summary>
		/// 取得揀貨容器商品是否都為取消訂單商品，且所以商品可播數小於等於可播取消商品數量
		/// </summary>
		/// <param name="f0537s"></param>
		/// <param name="itemCodes"></param>
		/// <returns></returns>
		public string GetAllCpItem(List<F0537> f0537s, List<string> itemCodes, List<BindingPickContainerDetail> pickContainerDetails)
		{
			if (f0537s.Any())
			{
				var cpItemCodes = f0537s.Where(x => x.STATUS == "0").Select(x => x.ITEM_CODE).Distinct().ToList();
				if (itemCodes.All(x => cpItemCodes.Contains(x)))
				{
					foreach (var pickContainerDetail in pickContainerDetails)
					{
						var pickContainerNowSowQty = pickContainerDetail.B_SET_QTY - pickContainerDetail.A_SET_QTY;
						var cancelNoSowQty = f0537s.Where(x => x.ITEM_CODE == pickContainerDetail.ITEM_CODE).Sum(x => x.B_SET_QTY - x.A_SET_QTY);
						if (pickContainerNowSowQty > cancelNoSowQty)
						{
							return "0";
						}
					}
					return "1";
				}
				return "0";
			}
			return "0";

		}

		/// <summary>
		/// 刷讀失敗即時寫入Log
		/// </summary>
		/// <param name="f053602"></param>
		public void AddFaliureLog(F053602 f053602)
		{
			var f053602Repo = new F053602Repository(Schemas.CoreSchema);
			f053602Repo.Add(f053602);
		}

		/// <summary>
		/// 取得揀貨容器未完成分貨資料
		/// </summary>
		/// <param name="f0701_Id"></param>
		/// <returns></returns>
		public IQueryable<F053601_NotAllotData> GetNotAllotDataInPickContainer(long f0701_Id)
		{
			var f053601Repo = new F053601Repository(Schemas.CoreSchema);
			//(1)	[C]=  取得揀貨容器未完成分貨資料
			//(2)	回傳[C]
			return f053601Repo.GetNotAllotDataInPickContainer(f0701_Id);
		}

		/// <summary>
		/// 人員手動揀貨容器完成
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="bindingPickContainerInfo"></param>
		/// <returns></returns>
		public ExecuteResult ManualContainerFinish(string dcCode, string gupCode, string custCode, BindingPickContainerInfo bindingPickContainerInfo)
		{
			try
			{
				var f053601Repo = new F053601Repository(Schemas.CoreSchema, wmsTransaction);
				var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);
				var f0536Repo = new F0536Repository(Schemas.CoreSchema, wmsTransaction);
				var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
				var f0537Repo = new F0537Repository(Schemas.CoreSchema, wmsTransaction);
				var f0530Repo = new F0530Repository(Schemas.CoreSchema, wmsTransaction);
				var f0534Repo = new F0534Repository(Schemas.CoreSchema, wmsTransaction);
				var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);
				var f053202Repo = new F053202Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	揀貨容器鎖定
				var lockResult = LockPickContainer(bindingPickContainerInfo.CONTAINER_CODE);
				if (!lockResult.IsSuccessed)
					return new ExecuteResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	更新F053601.STATUS=2(缺貨確認)
				f053601Repo.UpdateStatusByF0701Id(bindingPickContainerInfo.F0701_ID, "2");

				//(3)	新增F053602(log)
				var f053602 = new F053602
				{
					F0701_ID = bindingPickContainerInfo.F0701_ID,
					SCAN_CODE = "UserLackConfirm",
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					IS_PASS = "1",
					MESSAGE = $"人員確認揀貨容器已無商品，將剩餘分貨數設為缺貨",
				};
				f053602Repo.Add(f053602);

				//(4)	更新F0536.STATUS=2 (人員手動分貨完成)
				f0536Repo.UpdateStatusByF0701Id(bindingPickContainerInfo.F0701_ID, "2");

        //(5)	釋放揀貨容器 DELETE F0701 WHERE ID =<參數1>
        ContainerService.DelContainer(dcCode, gupCode, custCode, bindingPickContainerInfo.PICK_ORD_NO);

        // 釋放進行中揀貨容器
        f0530Repo.DeleteByF0701Id(bindingPickContainerInfo.F0701_ID);

				//更新F0534.STATUS=1(已放入並關閉稽核箱待分配)
				f0534Repo.UpdatePartialCloseByF0701Id(bindingPickContainerInfo.F0701_ID);

				//(6)	檢查是否為揀貨單最後一箱
				//A.	[D] = 取得揀貨單還有綁定的容器筆數不含目前作業這箱
				var containerCnt = f0701Repo.GetContainerCntExceptId(dcCode, gupCode, custCode, bindingPickContainerInfo.PICK_ORD_NO, bindingPickContainerInfo.F0701_ID);
				//B.	如果[D]筆數=0
				if (containerCnt == 0)
				{
					//a.	更新取消訂單揀貨單分貨明細紀錄F0537.STATUS=2(缺貨確認)
					f0537Repo.UpdateStatusByPickOrdNo(dcCode, gupCode, custCode, bindingPickContainerInfo.PICK_ORD_NO, "2");
				}
				//C.	如果[D]筆數>0，不處理往下執行

				//取得揀貨容器綁定的跨庫箱/取消容器F0531_ID
				var f0531_IDs = f053201Repo.GeF0531IdsByF0701Id(bindingPickContainerInfo.F0701_ID);
				foreach (var f0531_ID in f0531_IDs)
				{
					//判斷是否容器中存在揀貨容器的商品
					var existedItem = f053202Repo.IsExistPickItem(f0531_ID, bindingPickContainerInfo.F0701_ID);
					//容器內不存在揀貨容器商品，則刪除F053201綁定關係
					if (!existedItem)
					{
						f053201Repo.DeleteBindingContainer(f0531_ID, bindingPickContainerInfo.F0701_ID);
					}
				}

				return new ExecuteResult { IsSuccessed = true };
			}
			catch (Exception ex)
			{
				return new ExecuteResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockContainerProcess(new List<string>() { bindingPickContainerInfo.CONTAINER_CODE });
			}
		}

		/// <summary>
		/// 正常出貨跨庫箱號關箱
		/// </summary>
		/// <param name="containerInfo"></param>
		/// <param name="bindingPickContainerInfo"></param>
		/// <returns></returns>
		public ExecuteResult CloseNormalContainer(OutContainerInfo containerInfo, BindingPickContainerInfo bindingPickContainerInfo)
		{
			try
			{
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				//var f0534Repo = new F0534Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	跨庫箱號鎖定
				var lockResult = LockOutContainer(containerInfo.OUT_CONTAINER_CODE);
				if (!lockResult.IsSuccessed)
					return new ExecuteResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	[A]=檢查是否有存在作業中的跨庫箱號
				var f0531 = f0531Repo.GetDataById(containerInfo.F0531_ID);

				//(3)	若[A]不存在，回傳訊息[此跨庫箱號XXX已關箱或已出貨，不可進行關箱]
				if (f0531 == null)
					return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號{ containerInfo.OUT_CONTAINER_CODE }已關箱或已出貨，不可進行關箱" };

				//A.	更新F0532.STATUS=1(關箱),CLOSE_STAFF=目前登入者帳號,CLOSE_NAME=目前登入者姓名,CLOSE_DATE=系統時間 WHERE F0531_ID=<參數1>
				f0532Repo.UpdateCloseInfoByF0531Id(containerInfo.F0531_ID);
				//B.	新增F0533 STATUS=2(已關箱)
				var f0533 = new F0533
				{
					F0531_ID = f0531.ID,
					DC_CODE = f0531.DC_CODE,
					GUP_CODE = f0531.GUP_CODE,
					CUST_CODE = f0531.CUST_CODE,
					OUT_CONTAINER_CODE = f0531.OUT_CONTAINER_CODE,
					MOVE_OUT_TARGET = f0531.MOVE_OUT_TARGET,
					WORK_TYPE = f0531.WORK_TYPE,
					STATUS = "2",
					TOTAL_QTY = f0531.TOTAL,
				};
				f0533Repo.Add(f0533);
				////C.	更新F0534.STATUS=1(已放入並關閉稽核箱待分配)
				//f0534Repo.UpdatePartialCloseByF0531Id(containerInfo.F0531_ID);
				//D.	刪除F0531 WHERE ID=<參數1>
				f0531Repo.Delete(x => x.ID == containerInfo.F0531_ID);

				if (bindingPickContainerInfo != null)
				{
					var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);
					var f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = "CloseNormalBox",
						IS_PASS = "1",
						MESSAGE = $"人員按下跨庫箱號{ containerInfo.OUT_CONTAINER_CODE }關箱",
					};
					f053602Repo.Add(f053602);
				}

				return new ExecuteResult { IsSuccessed = true };
			}
			catch (Exception ex)
			{
				return new ExecuteResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockOutContainerProcess(new List<string>() { containerInfo.OUT_CONTAINER_CODE });
			}
		}

		/// <summary>
		/// 訂單取消容器關箱
		/// </summary>
		/// <param name="containerInfo"></param>
		/// <param name="bindingPickContainerInfo"></param>
		/// <returns></returns>
		public ExecuteResult CloseCancelContainer(OutContainerInfo containerInfo, BindingPickContainerInfo bindingPickContainerInfo)
		{
			try
			{
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				//var f0534Repo = new F0534Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	取消訂單容器條碼鎖定
				var lockResult = LockCancelContainer(containerInfo.OUT_CONTAINER_CODE);
				if (!lockResult.IsSuccessed)
					return new ExecuteResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	[A]=檢查是否有存在作業中的取消訂單容器條碼
				var f0531 = f0531Repo.GetDataById(containerInfo.F0531_ID);

				//(3)	若[A]不存在，回傳訊息[此容器條碼XXX已關箱，不可進行關箱]
				if (f0531 == null)
					return new ExecuteResult { IsSuccessed = false, Message = $"此容器條碼{ containerInfo.OUT_CONTAINER_CODE }已關箱，不可進行關箱" };

				//A.	更新F0532.STATUS=1(關箱),CLOSE_STAFF=目前登入者帳號,CLOSE_NAME=目前登入者姓名,CLOSE_DATE=系統時間 WHERE F0531_ID=<參數1>
				f0532Repo.UpdateCloseInfoByF0531Id(containerInfo.F0531_ID);
				//B.	新增F0533 STATUS=2(已關箱)
				var f0533 = new F0533
				{
					F0531_ID = f0531.ID,
					DC_CODE = f0531.DC_CODE,
					GUP_CODE = f0531.GUP_CODE,
					CUST_CODE = f0531.CUST_CODE,
					OUT_CONTAINER_CODE = f0531.OUT_CONTAINER_CODE,
					MOVE_OUT_TARGET = f0531.MOVE_OUT_TARGET,
					WORK_TYPE = f0531.WORK_TYPE,
					STATUS = "2",
					TOTAL_QTY = f0531.TOTAL,
				};
				f0533Repo.Add(f0533);
				////C.	更新F0534.STATUS=1(已放入並關閉稽核箱待分配)
				//f0534Repo.UpdatePartialCloseByF0531Id(containerInfo.F0531_ID);
				//D.	刪除F0531 WHERE ID=<參數1>
				f0531Repo.Delete(x => x.ID == containerInfo.F0531_ID);

				if (bindingPickContainerInfo != null)
				{
					var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);
					var f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = "CloseNormalBox",
						IS_PASS = "1",
						MESSAGE = $"人員按下取消訂單容器{ containerInfo.OUT_CONTAINER_CODE }關箱",
					};
					f053602Repo.Add(f053602);
					if (bindingPickContainerInfo.HAS_CP_ITEM == "1")
					{
						var f0537Repo = new F0537Repository(Schemas.CoreSchema, wmsTransaction);
						var f0537s = f0537Repo.GetDatasByPickOrdNo(bindingPickContainerInfo.DC_CODE,
																	bindingPickContainerInfo.GUP_CODE,
																	bindingPickContainerInfo.CUST_CODE,
																	bindingPickContainerInfo.PICK_ORD_NO).ToList();
						var itemCodes = bindingPickContainerInfo.ItemList.Select(s => s.ITEM_CODE).Distinct().ToList();
						var no = GetHasCpItem(f0537s, itemCodes) == "1" ? "1" : "2"; // No=1 還有取消訂單商品 // No=2 已無取消訂單商品
						return new ExecuteResult { IsSuccessed = true, No = no };
					}
				}

				return new ExecuteResult { IsSuccessed = true, No = "0" }; //不判斷取消訂單商品


			}
			catch (Exception ex)
			{
				return new ExecuteResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockCancelContainerProcess(new List<string>() { containerInfo.OUT_CONTAINER_CODE });
			}
		}

		/// <summary>
		/// 重綁稽核箱號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="oriContainerInfo"></param>
		/// <param name="newContainerCode"></param>
		/// <param name="bindingPickContainerInfo"></param>
		/// <returns></returns>
		public ExecuteResult RebindNormalContainer(string dcCode, string gupCode, string custCode, OutContainerInfo oriContainerInfo, string newContainerCode, BindingPickContainerInfo bindingPickContainerInfo)
		{
			try
			{
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	跨庫箱號鎖定
				var lockResult = LockOutContainer(newContainerCode);
				if (!lockResult.IsSuccessed)
					return new ExecuteResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	[A] =取得跨庫箱號檢核設定[F0003 .SYS_PATH WHERE AP_NAME=<參數1> AND AP_NAME= CrossDCContainer]
				var crossDCContainer = CommonService.GetSysGlobalValue(dcCode, "CrossDCContainer");

				F053602 f053602;

				//(3)	如果<參數2>的開頭不是[A]，回傳訊息[跨庫箱號必須開頭為[A]]
				if (crossDCContainer != null && !newContainerCode.StartsWith(crossDCContainer))
				{
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = newContainerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "0",
						MESSAGE = $"跨庫的容器條碼開頭必須是{ crossDCContainer }",
					};
					AddFaliureLog(f053602);
					return new ExecuteResult { IsSuccessed = false, Message = $"跨庫的容器條碼開頭必須是{ crossDCContainer }" };
				}

				//(4)	如果<參數2>的開頭是[A]，往下執行
				//(5)	[B] = 檢查跨庫箱號是否正在使用中
				var newContainerInfo = f0531Repo.GetOutContainerInfo(dcCode, newContainerCode);
				//如果[B]存在
				if (newContainerInfo != null)
				{

					//(6)	如果[B]存在，[B].SOW_TYPE=1 (取消訂單)
					if (newContainerInfo.SOW_TYPE == "1")
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = newContainerCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此跨庫箱號為取消訂單使用中的容器，不可使用",
						};
						AddFaliureLog(f053602);
						return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號為取消訂單使用中的容器，不可使用" };
					}

					//(7)	如果[B]存在，且[B].GUP_CODE <> <參數4>，回傳[此跨庫箱號已被其他業主綁定使用中，不可使用]
					if (newContainerInfo.GUP_CODE != gupCode)
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = newContainerCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此跨庫箱號已被其他業主綁定使用中，不可使用",
						};
						AddFaliureLog(f053602);
						return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號已被其他業主綁定使用中，不可使用" };

					}

					//(8)	如果[B]存在，且[B].CUST_CODE <> <參數5>，回傳[此跨庫箱號已被其他貨主綁定使用中，不可使用]
					if (newContainerInfo.CUST_CODE != custCode)
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = newContainerCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此跨庫箱號已被其他貨主綁定使用中，不可使用",
						};
						AddFaliureLog(f053602);
						return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號已被其他貨主綁定使用中，不可使用" };
					}

					//(9)	如果[B]存在，且[B].WORK_TYPE=0，回傳[此跨庫箱號已被[跨庫訂單整箱出庫]功能綁定使用中，不可使用]
					if (newContainerInfo.WORK_TYPE == "0")
					{
						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = newContainerCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此跨庫箱號已被[跨庫訂單整箱出庫]功能綁定使用中，不可使用",
						};
						AddFaliureLog(f053602);
						return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號已被[跨庫訂單整箱出庫]功能綁定使用中，不可使用" };
					}

					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = newContainerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "0",
						MESSAGE = $"此跨庫箱號已被使用，不可使用",
					};
					AddFaliureLog(f053602);
					//(10)	如果[B]存在，回傳[此跨庫箱號已被使用，不可使用]
					return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號已被使用，不可使用" };
				}
				//(11)	如果[B]不存在，往下執行
				else
				{
					//(12)	[C]= 檢查跨庫箱號是否存在已關箱未出貨
					var f0532_NotShipData = f0532Repo.GetCloseContainer(dcCode, newContainerCode);

					//如果[C]存在
					if (f0532_NotShipData != null)
					{
						//(13)	如果[C]存在，但SOW_TYPE=1(取消訂單)，則顯示[此跨庫箱號XXX為取消訂單已關箱的容器，不可使用]
						if (f0532_NotShipData.SOW_TYPE == "1")
						{
							f053602 = new F053602
							{
								F0701_ID = bindingPickContainerInfo.F0701_ID,
								SCAN_CODE = newContainerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = $"此跨庫箱號為取消訂單已關箱的容器，不可使用",
							};
							AddFaliureLog(f053602);
							return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號為取消訂單已關箱的容器，不可使用" };
						}

						f053602 = new F053602
						{
							F0701_ID = bindingPickContainerInfo.F0701_ID,
							SCAN_CODE = newContainerCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此跨庫箱號已關箱，不可使用",
						};
						AddFaliureLog(f053602);
						//(14)	如果[C]存在，回傳[此跨庫箱號已關箱，不可使用]
						return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號已關箱，不可使用" };
					}

					//(15)	如果[C]不存在，往下執行
					//(16)	更新F0531.OUT_CONTAINER_CODE = <參數2> WHERE ID=<參數1>
					f0531Repo.UpdateContainerCodeById(oriContainerInfo.F0531_ID, newContainerCode);

					//(17)	更新F0532.OUT_CONTAINER_CODE = <參數2> WHERE F0531_ID = <參數1>
					f0532Repo.UpdateContainerCodeByF0531Id(oriContainerInfo.F0531_ID, newContainerCode);

					//(18)	更新F0533.OUT_CONTAINER_CODE =<參數2> WHERE F0531_ID=<參數1>
					f0533Repo.UpdateContainerCodeByF0531Id(oriContainerInfo.F0531_ID, newContainerCode);

					//(19)	新增F053602(log)
					f053602 = new F053602
					{
						F0701_ID = bindingPickContainerInfo.F0701_ID,
						SCAN_CODE = newContainerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "1",
						MESSAGE = $"人員重綁跨庫箱號由{ oriContainerInfo.OUT_CONTAINER_CODE }更換成{ newContainerCode }",
					};
					f053602Repo.Add(f053602);

					return new ExecuteResult { IsSuccessed = true };
				}
			}
			catch (Exception ex)
			{
				return new ExecuteResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockOutContainerProcess(new List<string>() { newContainerCode });
			}
		}
		/// <summary>
		/// 重綁取消訂單容器
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="oriContainerInfo"></param>
		/// <param name="newContainerCode"></param>
		/// <returns></returns>
		public ExecuteResult RebindCancelContainer(string dcCode, string gupCode, string custCode, OutContainerInfo oriContainerInfo, string newContainerCode)
		{
			try
			{
				var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);

				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				var f0534Repo = new F0534Repository(Schemas.CoreSchema, wmsTransaction);
				var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	取消訂單容器條碼鎖定
				var lockResult = LockCancelContainer(newContainerCode);
				if (!lockResult.IsSuccessed)
					return new ExecuteResult { IsSuccessed = false, Message = lockResult.Message };

				F053602 f053602;

				//(2)	[A] = 檢查容器是否綁定中
				var f0701 = f0701Repo.GetDataByTypeAndContainer(dcCode, "0", newContainerCode);

				//(3)	如果[A]有資料
				if (f0701 != null)
				{
					//A.	[B] = 檢查取消訂單容器條碼是否正在使用中
					var newContainerInfo = f0531Repo.GetOutContainerInfo(dcCode, newContainerCode);
					//如果[B]存在
					if (newContainerInfo != null)
					{
						//B.	如果[B]存在，且[B].GUP_CODE <> <參數4>，回傳[此取消訂單容器條碼已被其他業主綁定使用中，不可使用]
						if (newContainerInfo.GUP_CODE != gupCode)
						{
							f053602 = new F053602
							{
								F0701_ID = oriContainerInfo.F0701_ID,
								SCAN_CODE = newContainerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = $"此取消訂單容器條碼已被其他業主綁定使用中，不可使用",
							};
							AddFaliureLog(f053602);
							return new ExecuteResult { IsSuccessed = false, Message = $"此取消訂單容器條碼已被其他業主綁定使用中，不可使用" };

						}

						//C.	如果[B]存在，且[B].CUST_CODE <> <參數5>，回傳[此取消訂單容器條碼已被其他貨主綁定使用中，不可使用]
						if (newContainerInfo.CUST_CODE != custCode)
						{
							f053602 = new F053602
							{
								F0701_ID = oriContainerInfo.F0701_ID,
								SCAN_CODE = newContainerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = $"此取消訂單容器條碼已被其他貨主綁定使用中，不可使用",
							};
							AddFaliureLog(f053602);
							return new ExecuteResult { IsSuccessed = false, Message = $"此取消訂單容器條碼已被其他貨主綁定使用中，不可使用" };

						}

						//D.	如果[B]存在，但SOW_TYPE=0(正常出貨)，則顯示[此取消訂單容器條碼為正常出貨使用中的容器，不可使用]
						if (newContainerInfo.SOW_TYPE == "0")
						{
							f053602 = new F053602
							{
								F0701_ID = oriContainerInfo.F0701_ID,
								SCAN_CODE = newContainerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = $"此取消訂單容器條碼為正常出貨使用中的容器，不可使用"
							};
							AddFaliureLog(f053602);
							return new ExecuteResult { IsSuccessed = false, Message = $"此取消訂單容器條碼為正常出貨使用中的容器，不可使用" };
						}

						f053602 = new F053602
						{
							F0701_ID = oriContainerInfo.F0701_ID,
							SCAN_CODE = newContainerCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							IS_PASS = "0",
							MESSAGE = $"此取消訂單容器條碼已被使用，不可使用"
						};
						AddFaliureLog(f053602);
						//E.	如果[B]存在，回傳[此取消訂單容器條碼已被使用，不可使用]
						return new ExecuteResult { IsSuccessed = false, Message = $"此取消訂單容器條碼已被使用，不可使用" };
					}
					//F.	如果[B]不存在，往下執行
					else
					{
						//G.	[C]= 檢查取消訂單容器條碼是否存在已關箱未解除綁定
						var f0532_NotShipData = f0532Repo.GetCloseContainer(dcCode, newContainerCode);

						//如果[C]存在
						if (f0532_NotShipData != null)
						{
							//H.	如果[C]存在，但SOW_TYPE=0(正常出貨)，則顯示[此取消訂單容器條碼為正常出貨已關箱的容器，不可使用]
							if (f0532_NotShipData.SOW_TYPE == "0")
							{
								f053602 = new F053602
								{
									F0701_ID = oriContainerInfo.F0701_ID,
									SCAN_CODE = newContainerCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									IS_PASS = "0",
									MESSAGE = $"此取消訂單容器條碼為正常出貨已關箱的容器，不可使用"
								};
								AddFaliureLog(f053602);
								return new ExecuteResult { IsSuccessed = false, Message = $"此取消訂單容器條碼為正常出貨已關箱的容器，不可使用" };
							}

							f053602 = new F053602
							{
								F0701_ID = oriContainerInfo.F0701_ID,
								SCAN_CODE = newContainerCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								IS_PASS = "0",
								MESSAGE = $"此取消訂單容器條碼已關箱未解除綁定，不可使用"
							};
							AddFaliureLog(f053602);
							//I.	如果[C]存在，回傳[此取消訂單容器條碼已關箱未解除綁定，不可使用]
							return new ExecuteResult { IsSuccessed = false, Message = $"此取消訂單容器條碼已關箱未解除綁定，不可使用" };
						}

						//J.	如果[C]不存在，回傳[此容器已被使用，不可使用]
						return new ExecuteResult { IsSuccessed = false, Message = $"此容器已被使用，不可使用" };
					}
				}
				//(4)	如果[A]無資料
				else
				{
					//A.	更新F0531.OUT_CONTAINER_CODE = <參數2> WHERE ID=<參數1>
					f0531Repo.UpdateContainerCodeById(oriContainerInfo.F0531_ID, newContainerCode);

					//B.	更新F0532.OUT_CONTAINER_CODE = <參數2> WHERE F0531_ID = <參數1>
					f0532Repo.UpdateContainerCodeByF0531Id(oriContainerInfo.F0531_ID, newContainerCode);

					//C.	更新F0533.OUT_CONTAINER_CODE =<參數2> WHERE F0531_ID=<參數1>
					f0533Repo.UpdateContainerCodeByF0531Id(oriContainerInfo.F0531_ID, newContainerCode);

					//D.	更新F0701.CONTAINER_OCDE = <參數2> WHERE ID=<參數6>
					f0701Repo.UpdateContainerCodeById(oriContainerInfo.F0701_ID, newContainerCode);

					//E.	新增F053602(log)
					f053602 = new F053602
					{
						F0701_ID = oriContainerInfo.F0701_ID,
						SCAN_CODE = newContainerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						IS_PASS = "1",
						MESSAGE = $"人員重綁取消訂單容器由{ oriContainerInfo.OUT_CONTAINER_CODE }更換成{ newContainerCode }",
					};
					f053602Repo.Add(f053602);

					return new ExecuteResult { IsSuccessed = true };
				}
			}
			catch (Exception ex)
			{
				return new ExecuteResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockCancelContainerProcess(new List<string>() { newContainerCode });
			}
		}

		public IQueryable<MoveOutPickOrders> GetMoveOutPickOrders(string dcCode, string gupCode, string custCode, DateTime startDate, DateTime endDate, string moveOutTarget, string pickContainerCode)
		{
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
			return f0701Repo.GetMoveOutPickOrders(dcCode, gupCode, custCode, startDate, endDate, moveOutTarget, pickContainerCode);
		}

		public ExecuteResult StopAllot(StopAllotParam param)
		{
			var f053602Repo = new F053602Repository(Schemas.CoreSchema, wmsTransaction);
			var f053202Repo = new F053202Repository(Schemas.CoreSchema, wmsTransaction);
			var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);

			//當存在跨庫容器時，判斷是否該跨庫箱中存在揀貨容器的商品
			if (param.NORMAL_F0531_ID.HasValue)
			{
				var existedInNormal = f053202Repo.IsExistPickItem(param.NORMAL_F0531_ID.Value, param.PICK_F0701_ID);
				//跨庫箱內不存在揀貨容器商品，則刪除F053201綁定關係
				if (!existedInNormal)
				{
					f053201Repo.DeleteBindingContainer(param.NORMAL_F0531_ID.Value, param.PICK_F0701_ID);
				}
			}

			//當存在取消訂單容器時，判斷是否該取消訂單容器中存在揀貨容器的商品
			if (param.CANCEL_F0531_ID.HasValue)
			{
				var existedInCancel = f053202Repo.IsExistPickItem(param.CANCEL_F0531_ID.Value, param.PICK_F0701_ID);
				//取消訂單容器內不存在揀貨容器商品，則刪除F053201綁定關係
				if (!existedInCancel)
				{
					f053201Repo.DeleteBindingContainer(param.CANCEL_F0531_ID.Value, param.PICK_F0701_ID);
				}
			}

			var f053602 = new F053602
			{
				F0701_ID = param.PICK_F0701_ID,
				SCAN_CODE = "UserStopAllot",
				IS_PASS = "1",
				MESSAGE = "暫停分貨",
			};
			f053602Repo.Add(f053602);

			return new ExecuteResult(true);
		}

	}
}
