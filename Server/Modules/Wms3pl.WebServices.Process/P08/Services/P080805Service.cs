using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
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
	public class P080805Service
	{
		private WmsTransaction _wmsTransaction;
		private bool isLockPickContainer = false;
		private bool isLockOutContainer = false;
		private bool isLockCancelContainer = false;

		public WmsTransaction wmsTransaction
		{
			get
			{
				return _wmsTransaction;
			}
		}

		private CommonService _commonService;
		public CommonService CommonService
		{
			get
			{
				if (_commonService == null) _commonService = new CommonService();
				return _commonService;
			}
			set
			{
				_commonService = value;
			}
		}
		/// <summary>
		/// 是否在容器鎖定狀態
		/// </summary>

		public P080805Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 容器鎖定/解鎖
		/// <summary>
		/// 揀貨容器鎖定
		/// </summary>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ExecuteResult LockPickContainer(string containerCode)
		{
			//要注意呼叫這個fun＆ContainerTargetProcess是同一個物件不然會導致不會解鎖
			var _f076105Repo = new F076105Repository(Schemas.CoreSchema);

			var f076105 = _f076105Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
															new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
															() =>
															{
																var lockF076105 = _f076105Repo.LockF076105();
																var chkF076105 = _f076105Repo.Find(x => x.CONTAINER_CODE == containerCode);
																if (chkF076105 != null)
																	return null;
																var newF076105 = new F076105() { CONTAINER_CODE = containerCode };
																_f076105Repo.Add(newF076105);
																isLockPickContainer = true;
																return newF076105;
															});

			if (f076105 == null)
				return new ExecuteResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }目前正在處理中，請稍後再試" };

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 揀貨容器解鎖
		/// </summary>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ExecuteResult UnlockContainerProcess(List<string> containerCodeList)
		{
			var _f076105Repo = new F076105Repository(Schemas.CoreSchema);
			if (isLockPickContainer)
			{
				_f076105Repo.DeleteByContainerCode(containerCodeList);
				isLockPickContainer = false;
			}
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 跨庫箱號鎖定
		/// </summary>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ExecuteResult LockOutContainer(string containerCode)
		{
			//要注意呼叫這個fun＆ContainerTargetProcess是同一個物件不然會導致不會解鎖
			var _f076106Repo = new F076106Repository(Schemas.CoreSchema);

			var f076106 = _f076106Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
															new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
															() =>
															{
																var lockF076106 = _f076106Repo.LockF076106();
																var chkF076106 = _f076106Repo.Find(x => x.CONTAINER_CODE == containerCode);
																if (chkF076106 != null)
																	return null;
																var newF076106 = new F076106() { CONTAINER_CODE = containerCode };
																_f076106Repo.Add(newF076106);
																isLockOutContainer = true;
																return newF076106;
															});

			if (f076106 == null)
				return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號{ containerCode }目前正在處理中，請稍後再試" };

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 跨庫箱號解鎖
		/// </summary>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ExecuteResult UnlockOutContainerProcess(List<string> containerCodeList)
		{
			var _f076106Repo = new F076106Repository(Schemas.CoreSchema);
			if (isLockOutContainer)
			{
				_f076106Repo.DeleteByContainerCode(containerCodeList);
				isLockOutContainer = false;
			}
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 取消訂單容器條碼鎖定
		/// </summary>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ExecuteResult LockCancelContainer(string containerCode)
		{
			//要注意呼叫這個fun＆ContainerTargetProcess是同一個物件不然會導致不會解鎖
			var _f076107Repo = new F076107Repository(Schemas.CoreSchema);

			var f076107 = _f076107Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
															new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
															() =>
															{
																var lockF076107 = _f076107Repo.LockF076107();
																var chkF076107 = _f076107Repo.Find(x => x.CONTAINER_CODE == containerCode);
																if (chkF076107 != null)
																	return null;
																var newF076107 = new F076107() { CONTAINER_CODE = containerCode };
																_f076107Repo.Add(newF076107);
																isLockCancelContainer = true;
																return newF076107;
															});

			if (f076107 == null)
				return new ExecuteResult { IsSuccessed = false, Message = $"此容器條碼{ containerCode }目前正在處理中，請稍後再試" };

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 取消訂單容器條碼解鎖
		/// </summary>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ExecuteResult UnlockCancelContainerProcess(List<string> containerCodeList)
		{
			var _f076107Repo = new F076107Repository(Schemas.CoreSchema);
			if (isLockCancelContainer)
			{
				_f076107Repo.DeleteByContainerCode(containerCodeList);
				isLockCancelContainer = false;
			}
			return new ExecuteResult(true);
		}
		#endregion

		/// <summary>
		/// 刷讀揀貨容器
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public PickContainerResult ScanPickContainerCode(string dcCode, string gupCode, string custCode, string containerCode)
		{
			try
			{
				var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
				var f0530Repo = new F0530Repository(Schemas.CoreSchema, wmsTransaction);
				var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);
				var f050801Repo = new F050801Repository(Schemas.CoreSchema, wmsTransaction);
				var f070102Repo = new F070102Repository(Schemas.CoreSchema, wmsTransaction);
				var f2501Repo = new F2501Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	揀貨容器鎖定
				var lockResult = LockPickContainer(containerCode);
				if (!lockResult.IsSuccessed)
					return new PickContainerResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	取得揀貨容器資料[A]
				var pickContainerInfos = f0701Repo.GetPickContainerInfo(dcCode, containerCode).ToList();

				//(3)	如果[A]無資料，顯示[找不到揀貨容器XXX容器資料]，若[A]有資料，往下執行
				if (!pickContainerInfos.Any())
					return new PickContainerResult { IsSuccessed = false, Message = $"找不到揀貨容器{ containerCode }容器資料" };

				//(4)	檢查[A]是否混不同業主，若有[此揀貨容器XXX混不同業主，請到新稽核出庫功能作業]
				if (pickContainerInfos.Any(a => a.GUP_CODE != gupCode))
					return new PickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }混不同業主，請到新稽核出庫功能作業" };

				//(5)	檢查[A]是否混不同貨主，若有[此揀貨容器XXX混不同貨主，請到新稽核出庫功能作業]
				if (pickContainerInfos.Any(a => a.CUST_CODE != custCode))
					return new PickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }混不同貨主，請到新稽核出庫功能作業" };

				//(6)	[K] =取得揀貨容器是否存在於使用中的容器
				var f0530s = f0530Repo.GetF0530s(pickContainerInfos.Select(s => s.F0701_ID).ToList());

				//(7)	如果[K]有資料
				if (f0530s.Any())
				{
					//A.	[K1]=檢查是否含有任一筆在新稽核出庫作業使用中的容器
					var f0530sUsingNewMoveOut = f0530s.Where(w => w.WORK_TYPE == "1");
					//B.	如果[K1]有資料，回傳[此揀貨容器XXX已在新稽核出庫作業，請到新稽核出庫功能作業]
					if (f0530sUsingNewMoveOut.Any())
						return new PickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }已在新稽核出庫作業，請到新稽核出庫功能作業" };
					//C.	如果[K1]無資料
					if (!f0530sUsingNewMoveOut.Any())
					{
						//a.	[A]= [A]排除[K]已經放入稽核箱的揀貨容器
						pickContainerInfos = pickContainerInfos.Where(w => !f0530s.Select(s => s.F0701_ID).Contains(w.F0701_ID)).ToList();
						//b.	如果[A]已無任何資料，回傳[此揀貨容器XXX都已放入跨庫箱號，不可重複作業]
						if (!pickContainerInfos.Any())
							return new PickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }都已放入跨庫容器，不可重複作業" };
					}
				}

				//(8)	如果[K]無資料，往下執行
				//(11)	檢查[A]是否有不同目的地[[A].MOVE_OUT_TARGET]，若有顯示[此揀貨容器XXX有不同目的地，請到新稽核出庫功能作業]
				if (pickContainerInfos.Select(s => s.MOVE_OUT_TARGET).Distinct().Count() > 1)
					return new PickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }有不同目的地，請到新稽核出庫功能作業" };

				//(12)	檢查[A]是否含有人工倉揀貨單[[A].DEVICE_TYPE=0]，若有顯示[此揀貨容器XXX含有人工倉揀貨單，請到新稽核出庫功能作業]
				if (pickContainerInfos.Any(a => a.DEVICE_TYPE == "0"))
					return new PickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }含有人工倉揀貨單，請到新稽核出庫功能作業" };

				//(13)	檢查[A]揀貨單中是否有取消的訂單，若有筆數則回傳[此揀貨容器XXX揀貨單中含有取消訂單商品，請到新稽核出庫功能作業]
				if (f050801Repo.IsCancelOrdByPickOrdNo(pickContainerInfos.Select(s => s.PICK_ORD_NO).ToList()))
					return new PickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }含有取消訂單商品，請到新稽核出庫功能作業" };

				//(15)	[B] = 取得容器商品明細
				var pickContainerDetails = f070102Repo.GetContainerDetailByF0701Ids(pickContainerInfos.Select(s => s.F0701_ID).ToList());

				//(16)	[C] = 取得容器明細中為序號商品，但未提供商品序號的品號清單
				var errorSerialItemList = pickContainerDetails.Where(w => w.BUNDLE_SERIALNO == "1")
												.GroupBy(g => new { g.ITEM_CODE })
												.Where(w => w.Sum(s => string.IsNullOrWhiteSpace(s.SERIAL_NO) ? 0 : 1) == 0)
												.Select(s => s.Key.ITEM_CODE)
												.ToList();

				//(17)	若[C]有資料，顯示[此揀貨容器XXX含有序號商品未提供序號，請到新稽核出庫功能作業]
				if (errorSerialItemList.Any())
					return new PickContainerResult { IsSuccessed = false, Message = $"此揀貨容器{ containerCode }含有序號商品未提供序號，請到新稽核出庫功能作業" };

				//(24)	如果[D]無資料，往下執行
				//(25)	以上檢核都通過，回傳揀貨容器資訊，顯示於畫面上
				return new PickContainerResult
				{
					IsSuccessed = true,
					ContainerCode = containerCode,
					PickContainerInfos = pickContainerInfos,
					MoveOutTargetName = pickContainerInfos.FirstOrDefault().CROSS_NAME,
					TotalPcs = pickContainerInfos.Sum(s => s.TOTAL)
				};
			}
			catch (Exception ex)
			{
				return new PickContainerResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockContainerProcess(new List<string>() { containerCode });
			}
		}

		/// <summary>
		/// 刷讀跨庫箱號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public OutContainerResult ScanOutContainerCode(string dcCode, string gupCode, string custCode, string containerCode)
		{
			try
			{
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	跨庫箱號鎖定
				var lockResult = LockOutContainer(containerCode);
				if (!lockResult.IsSuccessed)
					return new OutContainerResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	檢查跨庫箱號是否為99Z開頭
				var crossDCContainer = CommonService.GetSysGlobalValue(dcCode, "CrossDCContainer");
				if (crossDCContainer != null && !containerCode.StartsWith(crossDCContainer))
					return new OutContainerResult { IsSuccessed = false, Message = $"跨庫的容器條碼開頭必須是{ crossDCContainer }" };

				//(3)	[A]=檢查跨庫箱號是否正在使用中
				var outContainerInfo = f0531Repo.GetOutContainerInfo(dcCode, containerCode);

				//(4)	如果[A]存在，但SOW_TYPE=1(取消訂單)，則顯示[此跨庫箱號XXX為取消訂單使用中的容器，不可使用]
				if (outContainerInfo != null && outContainerInfo.SOW_TYPE == "1")
					return new OutContainerResult { IsSuccessed = false, Message = $"此跨庫箱號{ containerCode }為取消訂單使用中的容器，不可使用" };

				//(5)	如果[A]存在，但作業類型=1(新稽核出庫)，則顯示[此跨庫箱號XXX已在新稽核出庫使用，不可使用]
				if (outContainerInfo != null && outContainerInfo.WORK_TYPE == "1")
					return new OutContainerResult { IsSuccessed = false, Message = $"此跨庫箱號{ containerCode }已在新稽核出庫使用，不可使用" };

				//(6)	如果[A]存在，作業類型=0(跨庫訂單整箱出庫)，則將資料顯示在畫面上
				if (outContainerInfo != null && outContainerInfo.WORK_TYPE == "0")
				{
					return new OutContainerResult
					{
						IsSuccessed = true,
						ContainerCode = containerCode,
						OutContainerInfo = outContainerInfo,
						MoveOutTargetName = outContainerInfo.CROSS_NAME,
						TotalPcs = outContainerInfo.TOTAL
					};
				}

				//(7)	[B]=取得該誇庫箱號是否關箱
				var f0532_NotShipData = f0532Repo.GetCloseContainer(dcCode, containerCode);

				//(8)	如果[A]不存在，[B]存在
				if (outContainerInfo == null && f0532_NotShipData != null)
				{
					//如果[B].SOW_TYPE=0，回傳訊息:此跨庫箱號已關箱，不可使用
					if (f0532_NotShipData.SOW_TYPE == "0")
						return new OutContainerResult { IsSuccessed = false, Message = $"此跨庫箱號{ containerCode }已關箱，不可使用" };
					//如果[B].SOW_TYPE=1，回傳訊息:此跨庫箱號已被訂單取消使用並關箱，不可使用]
					else if (f0532_NotShipData.SOW_TYPE == "1")
						return new OutContainerResult { IsSuccessed = false, Message = $"此跨庫箱號{ containerCode }已被訂單取消使用並關箱，不可使用" };
				}

				//(9)	如果[A]不存在，[B]也不存在
				if (outContainerInfo == null && f0532_NotShipData == null)
				{
					//A.	[C] = 取得箱序
					var newContainerSeq = f0532Repo.GetNewContainerSeq("0");
					//B.	如果[C]無資料，[C]=1
					if (newContainerSeq == 0)
						newContainerSeq = 1;

					//C.	新增F0531、F0532 (OUT_CONTAINER_SEQ=[C])
					var f0531Id = GetF0531NextId();
					var f0531 = new F0531
					{
						ID = f0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						OUT_CONTAINER_SEQ = newContainerSeq,
						WORK_TYPE = "0",
						SOW_TYPE = "0",
						STATUS = "0",
					};
					var f0532 = new F0532
					{
						F0531_ID = f0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						OUT_CONTAINER_SEQ = newContainerSeq,
						WORK_TYPE = "0",
						SOW_TYPE = "0",
						STATUS = "0",
					};
					//D.	新增F0533 (Status=0開啟中)
					var f0533 = new F0533
					{
						F0531_ID = f0531Id,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						OUT_CONTAINER_CODE = containerCode,
						WORK_TYPE = "0",
						STATUS = "0",
					};
					f0531Repo.Add(f0531);
					f0532Repo.Add(f0532);
					f0533Repo.Add(f0533);

					return new OutContainerResult
					{
						IsSuccessed = true,
						ContainerCode = containerCode,
						OutContainerInfo = new OutContainerInfo
						{
							F0531_ID = f0531Id,
							DC_CODE = dcCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							OUT_CONTAINER_CODE = containerCode,
							WORK_TYPE = "0",
							STATUS = "0",
							CRT_DATE = DateTime.Now,
						},
						MoveOutTargetName = "未綁定",
						TotalPcs = 0
					};
				}

				//回傳揀貨容器資訊
				return new OutContainerResult { IsSuccessed = false, Message = $"此跨庫箱號{ containerCode }資料異常，不可使用" };
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

		public long GetF0531NextId()
		{
			var f0531Repo = new F0531Repository(Schemas.CoreSchema);
			var f0531 = f0531Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
														new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
														() =>
														{
															var lockF0531 = f0531Repo.LockF0531();
															var id = f0531Repo.GetF0531NextId();
															return new F0531
															{
																ID = id
															};
														});
			return f0531.ID;
		}

		/// <summary>
		/// 揀貨容器確定放入到跨庫箱內
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickContainer"></param>
		/// <param name="outContainer"></param>
		/// <returns></returns>
		public PickContainerPutIntoOutContainerResult PickContainerPutIntoOutContainer(string dcCode, string gupCode, string custCode, PickContainerResult pickContainer, OutContainerResult outContainer)
		{
			try
			{
				var f0530Repo = new F0530Repository(Schemas.CoreSchema, wmsTransaction);
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);
				var f053202Repo = new F053202Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
				var f070102Repo = new F070102Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	揀貨容器鎖定
				var lockResult = LockPickContainer(pickContainer.ContainerCode);
				if (!lockResult.IsSuccessed)
					return new PickContainerPutIntoOutContainerResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	跨庫箱號鎖定
				lockResult = LockOutContainer(outContainer.ContainerCode);
				if (!lockResult.IsSuccessed)
					return new PickContainerPutIntoOutContainerResult { IsSuccessed = false, Message = lockResult.Message };

				//(3)	[B] = 取得使用中的跨庫出貨容器[F0531]
				var f0531 = f0531Repo.GetDataById(outContainer.OutContainerInfo.F0531_ID);

				//(4)	如果[B]不存在
				if (f0531 == null)
				{
					//A.	[C] = 取得已關箱的跨庫出貨容器[F0532]
					var f0532 = f0532Repo.GetDataByF0531Id(outContainer.OutContainerInfo.F0531_ID);
					//B.	如果[C]存在
					if (f0532 != null)
					{
						return new PickContainerPutIntoOutContainerResult
						{
							IsSuccessed = false,
							IsOutContainerError = true,
							Message = $"此跨庫箱號{ outContainer.ContainerCode }已關箱或出貨，請重新刷入跨庫箱號"
						};
					}
					//C.	如果[C]不存在
					else
					{
						return new PickContainerPutIntoOutContainerResult
						{
							IsSuccessed = false,
							IsOutContainerError = true,
							Message = $"此跨庫箱號{ outContainer.ContainerCode }已解除綁定，請重新刷入跨庫箱號"
						};
					}
				}
				//(5)	如果[B]存在
				else
				{
					//A.	如果[B].作業類型=1
					if (f0531.WORK_TYPE == "1")
					{
						return new PickContainerPutIntoOutContainerResult
						{
							IsSuccessed = false,
							IsOutContainerError = true,
							Message = $"此跨庫箱號{ outContainer.ContainerCode }已在新稽核出庫作業，不可使用，請重新刷入跨庫箱號"
						};
					}
					//B.	如果[B].作業類型=0(跨庫訂單整箱出庫)，但[B].GUP_CODE<><參數4>.GUP_CODE OR [B].CUST_CODE <> <參數4>.CUST_CODE
					else if (f0531.WORK_TYPE == "0" && (f0531.GUP_CODE != gupCode || f0531.CUST_CODE != custCode))
					{
						return new PickContainerPutIntoOutContainerResult
						{
							IsSuccessed = false,
							IsOutContainerError = true,
							Message = $"此跨庫箱號{ outContainer.ContainerCode }與揀貨容器業主或貨主不同，不可以混業主或貨主放同一個跨庫箱號"
						};
					}
				}

				//(6)	[D]= 取得揀貨容器綁定[F0701]
				var f0701_IDs = pickContainer.PickContainerInfos.Select(s => s.F0701_ID).ToList();
				var f0701 = f0701Repo.GetDatasByF0701Ids(f0701_IDs).ToList();
				//A.	如果[D]無任何資料
				if (!f0701.Any())
				{
					return new PickContainerPutIntoOutContainerResult
					{
						IsSuccessed = false,
						IsPickContainerError = true,
						Message = $"此揀貨容器{ pickContainer.ContainerCode }已釋放，不可使用，請重新刷入揀貨容器"
					};
				}
				//B.	如果[D]有資料，但[D]的筆數與<參數4>的明細筆數不同
				else
				{
					if (f0701.Count != pickContainer.PickContainerInfos.Count)
					{
						return new PickContainerPutIntoOutContainerResult
						{
							IsSuccessed = false,
							IsPickContainerError = true,
							Message = $"此揀貨容器{ pickContainer.ContainerCode }內容有異動，請重新刷入揀貨容器"
						};
					}
				}

				//(7)	[E] = 取得使用中跨庫出貨容器揀貨容器綁定[F0530]
				var f0530s = f0530Repo.GetF0530s(f0701_IDs).ToList();
				//A.	如果[E]有資料
				if (f0530s.Any())
				{
					return new PickContainerPutIntoOutContainerResult
					{
						IsSuccessed = false,
						IsPickContainerError = true,
						Message = $"此揀貨容器{ pickContainer.ContainerCode }，已放入稽核箱，不可重複放入，請重新刷入揀貨容器"
					};
				}

				//(8)	如果<參數5>.目的地!=null(不指定) and <參數4>.目的地 <> <參數5>.目的地
				if (!string.IsNullOrWhiteSpace(outContainer.OutContainerInfo.MOVE_OUT_TARGET) && pickContainer.MoveOutTargetName != outContainer.MoveOutTargetName)
				{
					return new PickContainerPutIntoOutContainerResult
					{
						IsSuccessed = false,
						IsOutContainerError = true,
						Message = $"此揀貨容器{ pickContainer.ContainerCode }與跨庫箱號{ outContainer.ContainerCode }目的地不同，不可放入，請重新刷入跨庫箱號"
					};
				}
				var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
				//(9)	檢查[A]揀貨單中是否有取消的訂單，若有筆數則回傳[此揀貨容器XXX揀貨單中含有取消訂單商品，請到新稽核出庫功能作業]
				if (f050801Repo.IsCancelOrdByPickOrdNo(pickContainer.PickContainerInfos.Select(s => s.PICK_ORD_NO).ToList()))
					return new PickContainerPutIntoOutContainerResult { IsSuccessed = false, IsPickContainerError = true, Message = $"此揀貨容器{ pickContainer.ContainerCode }含有取消訂單商品，請到新稽核出庫功能作業" };

				//(10)	[B] = 取得容器商品明細
				var pickContainerDetails = f070102Repo.GetContainerDetailByF0701Ids(f0701_IDs).ToList();

				//(11)	[D] = 取得容器所有序號
				var snItemList = pickContainerDetails.Where(w => !string.IsNullOrWhiteSpace(w.SERIAL_NO)).Select(s => s.SERIAL_NO).Distinct().ToList();

				//(12)	如果[D]有資料
				if (snItemList.Any())
				{
					//A.	[D1]=取得商品序號資料ComonService.GetItemSerialList(<參數2>,<參數3>,[D])
					var f2501s = CommonService.GetItemSerialList(gupCode, custCode, snItemList);

					//B.	檢查[D1]商品序號狀態是否含有非在庫序號，若有一筆狀態不是可出貨(!=A1)，則顯示[揀貨容器XXX含有非在庫商品序號，請到新稽核出庫功能作業]
					if (f2501s.Any(a => a.STATUS != "A1"))
						return new PickContainerPutIntoOutContainerResult { IsSuccessed = false, IsPickContainerError = true, Message = $"此揀貨容器{ pickContainer.ContainerCode }含有非在庫商品序號，請到新稽核出庫功能作業" };

					//C.	檢查[D1]商品序號是否含有不良品序號(ACTIVATED=1)，若有一筆商品序號為不良品序號，則回傳訊息[揀貨容器XXX含有不良品商品序號，請到新稽核出庫功能作業]
					if (f2501s.Any(a => a.ACTIVATED == "1"))
						return new PickContainerPutIntoOutContainerResult { IsSuccessed = false, IsPickContainerError = true, Message = $"此揀貨容器{ pickContainer.ContainerCode }含有不良品商品序號，請到新稽核出庫功能作業" };

					//D.	檢查[D]商品序號是否含有凍結序號，若有一筆序號凍結，則回傳訊息[揀貨容器XXX商品序號中有被凍結，請到新稽核出庫功能作業]
					var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
					var freezeSerialList = f2501Repo.GetSerialIsFreeze(gupCode, custCode, "02", snItemList).ToList();
					if (freezeSerialList.Any())
						return new PickContainerPutIntoOutContainerResult { IsSuccessed = false, IsPickContainerError = true, Message = $"此揀貨容器{ pickContainer.ContainerCode }商品序號中有被凍結，請到新稽核出庫功能作業" };
				}


				//(13)	新增F0530 從<參數4>
				var addF0530List = new List<F0530>();
				foreach (var pickContainerInfo in pickContainer.PickContainerInfos)
				{
					addF0530List.Add(new F0530
					{
						F0701_ID = pickContainerInfo.F0701_ID,
						DC_CODE = pickContainerInfo.DC_CODE,
						GUP_CODE = pickContainerInfo.GUP_CODE,
						CUST_CODE = pickContainerInfo.CUST_CODE,
						PICK_ORD_NO = pickContainerInfo.PICK_ORD_NO,
						CONTAINER_CODE = pickContainerInfo.CONTAINER_CODE,
						MOVE_OUT_TARGET = pickContainerInfo.MOVE_OUT_TARGET,
						DEVICE_TYPE = pickContainerInfo.DEVICE_TYPE,
						TOTAL = pickContainerInfo.TOTAL,
						WORK_TYPE = "0",
					});
				}
				f0530Repo.BulkInsert(addF0530List);

				//(14)	新增F053201 從<參數4>
				var addF053201List = new List<F053201>();
				foreach (var pickContainerInfo in pickContainer.PickContainerInfos)
				{
					addF053201List.Add(new F053201
					{
						F0531_ID = outContainer.OutContainerInfo.F0531_ID,
						F0701_ID = pickContainerInfo.F0701_ID,
						DC_CODE = pickContainerInfo.DC_CODE,
						GUP_CODE = pickContainerInfo.GUP_CODE,
						CUST_CODE = pickContainerInfo.CUST_CODE,
						PICK_ORD_NO = pickContainerInfo.PICK_ORD_NO,
						CONTAINER_CODE = pickContainerInfo.CONTAINER_CODE,
						STATUS = "0",
					});
				}
				f053201Repo.BulkInsert(addF053201List);

				//(15)	新增F053202 從[F]
				var addF053202List = new List<F053202>();
				foreach (var containerDetail in pickContainerDetails)
				{
					addF053202List.Add(new F053202
					{
						F0531_ID = outContainer.OutContainerInfo.F0531_ID,
						F0701_ID = containerDetail.F0701_ID,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						PICK_ORD_NO = containerDetail.PICK_ORD_NO,
						ITEM_CODE = containerDetail.ITEM_CODE,
						SERIAL_NO = containerDetail.SERIAL_NO,
						QTY = containerDetail.QTY,
					});
				}
				f053202Repo.BulkInsert(addF053202List);

				var currentMoveOutTarget = pickContainer.PickContainerInfos.FirstOrDefault().MOVE_OUT_TARGET;
				outContainer.OutContainerInfo.MOVE_OUT_TARGET = currentMoveOutTarget;

				//(16)	更新F0531.TOTAL+= SUM(<參數4>.TOTAL) WHERE ID=<參數5>.ID
				f0531Repo.UpdateTotalAndTargetById(outContainer.OutContainerInfo.F0531_ID
													, pickContainer.PickContainerInfos.Sum(s => s.TOTAL)
													, currentMoveOutTarget);
				outContainer.TotalPcs += pickContainer.PickContainerInfos.Sum(s => s.TOTAL);
				outContainer.MoveOutTargetName = pickContainer.MoveOutTargetName;

				//(17)	更新F0532.TOTAL+= SUM(<參數4>.TOTAL) WHERE F0531_ID=<參數5>.ID
				f0532Repo.UpdateTotalAndTargetById(outContainer.OutContainerInfo.F0531_ID
													, pickContainer.PickContainerInfos.Sum(s => s.TOTAL)
													, currentMoveOutTarget);

				//(18)	新增F0533 從<參數4>+<參數5>  STATUS=1(放入揀貨容器)
				var addF0533List = new List<F0533>();
				foreach (var pickContainerInfo in pickContainer.PickContainerInfos)
				{
					addF0533List.Add(new F0533
					{
						F0531_ID = outContainer.OutContainerInfo.F0531_ID,
						DC_CODE = dcCode,
						OUT_CONTAINER_CODE = outContainer.ContainerCode,
						MOVE_OUT_TARGET = currentMoveOutTarget,
						WORK_TYPE = "0",
						STATUS = "1",
						CONTAINER_CODE = pickContainer.ContainerCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						PICK_ORD_NO = pickContainerInfo.PICK_ORD_NO,
						TOTAL_QTY = pickContainerInfo.TOTAL
					});
				}
				f0533Repo.BulkInsert(addF0533List);

				//回傳
				return new PickContainerPutIntoOutContainerResult { IsSuccessed = true, UpdateOutContainerResult = outContainer };
			}
			catch (Exception ex)
			{
				return new PickContainerPutIntoOutContainerResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockContainerProcess(new List<string>() { pickContainer.ContainerCode });
				UnlockOutContainerProcess(new List<string>() { outContainer.ContainerCode });
			}

		}

		public IQueryable<F0532Ex> GetF0532Ex(string dcCode, string gupCode, string custCode, DateTime startDate, DateTime endDate, string status, string containerSowType, string outContainerCode, string workType)
		{
			var f0532Repo = new F0532Repository(Schemas.CoreSchema);
			return f0532Repo.GetF0532Ex(dcCode, gupCode, custCode, startDate, endDate, status, containerSowType, outContainerCode, workType);
		}

		public IQueryable<F053202Ex> GetF053202Ex(long F0531_ID)
		{
			var f053202Repo = new F053202Repository(Schemas.CoreSchema);
			return f053202Repo.GetF053202Ex(F0531_ID);
		}

		public IQueryable<P0808050000_PrintData> GetPrintData(long F0531ID)
		{
			var f0532Repo = new F0532Repository(Schemas.CoreSchema);
			return f0532Repo.GetPrintData(F0531ID);
		}

		public IQueryable<P0808050000_CancelPrintData> GetCancelPrintData(long F0531ID)
		{
			var f0532Repo = new F0532Repository(Schemas.CoreSchema);
			return f0532Repo.GetCancelPrintData(F0531ID);
		}

		/// <summary>
		/// 重新裝箱
		/// </summary>
		/// <param name="outContainerInfo"></param>
		/// <returns></returns>
		public ExecuteResult RePackingBox(OutContainerInfo outContainerInfo)
		{
			try
			{
				var f0530Repo = new F0530Repository(Schemas.CoreSchema, wmsTransaction);
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);
				var f053202Repo = new F053202Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	跨庫箱號鎖定
				var lockResult = LockOutContainer(outContainerInfo.OUT_CONTAINER_CODE);
				if (!lockResult.IsSuccessed)
					return new ExecuteResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	[A]=檢查是否有存在作業中的跨庫箱號
				var f0531 = f0531Repo.GetDataById(outContainerInfo.F0531_ID);
				//(3)	若[A]不存在
				if (f0531 == null)
					return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號{ outContainerInfo.OUT_CONTAINER_CODE }已關箱或已出貨，不可進行重新裝箱" };
				//(4)	若[A]存在
				else
				{
					//A.	新增F0533 STATUS=9(取消本次裝箱)
					var f0533 = new F0533
					{
						F0531_ID = f0531.ID,
						DC_CODE = f0531.DC_CODE,
						OUT_CONTAINER_CODE = f0531.OUT_CONTAINER_CODE,
						MOVE_OUT_TARGET = f0531.MOVE_OUT_TARGET,
						WORK_TYPE = f0531.WORK_TYPE,
						STATUS = "9",
						GUP_CODE = f0531.GUP_CODE,
						CUST_CODE = f0531.CUST_CODE,
						TOTAL_QTY = f0531.TOTAL,
					};
					f0533Repo.Add(f0533);

					//B.	[B] = 取得F053201.F0701_ID WHERE F0531_ID =<參數1>
					//C.	刪除F0530 WHERE F0701_ID IN([B])
					f0530Repo.DeleteByF0531Id(f0531.ID);

					//D.	刪除F0532、F053201、F053202 WHERE F0531_ID=<參數1>
					f0532Repo.Delete(x => x.F0531_ID == outContainerInfo.F0531_ID);
					f053201Repo.Delete(x => x.F0531_ID == outContainerInfo.F0531_ID);
					f053202Repo.Delete(x => x.F0531_ID == outContainerInfo.F0531_ID);

					//E.	刪除F0531 WHERE ID=<參數1>
					f0531Repo.Delete(x => x.ID == outContainerInfo.F0531_ID);

					return new ExecuteResult { IsSuccessed = true };
				}
			}
			catch (Exception ex)
			{
				return new ExecuteResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockOutContainerProcess(new List<string>() { outContainerInfo.OUT_CONTAINER_CODE });
			}
		}

		public ExecuteResult CloseBox(OutContainerInfo outContainerInfo)
		{
			try
			{
				var f0530Repo = new F0530Repository(Schemas.CoreSchema, wmsTransaction);
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
				var f053202Repo = new F053202Repository(Schemas.CoreSchema, wmsTransaction);
				var f0533Repo = new F0533Repository(Schemas.CoreSchema, wmsTransaction);
				var f0534Repo = new F0534Repository(Schemas.CoreSchema, wmsTransaction);
				var f2501Repo = new F2501Repository(Schemas.CoreSchema, wmsTransaction);
				var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
        var f060302Repo = new F060302Repository(Schemas.CoreSchema, wmsTransaction);

				//(1)	跨庫箱號鎖定
				var lockResult = LockOutContainer(outContainerInfo.OUT_CONTAINER_CODE);
				if (!lockResult.IsSuccessed)
					return new ExecuteResult { IsSuccessed = false, Message = lockResult.Message };

				//(2)	[A]=檢查是否有存在作業中的跨庫箱號
				var f0531 = f0531Repo.GetDataById(outContainerInfo.F0531_ID);
				//(3)	若[A]不存在
				if (f0531 == null)
					return new ExecuteResult { IsSuccessed = false, Message = $"此跨庫箱號{ outContainerInfo.OUT_CONTAINER_CODE }已關箱或已出貨，不可進行關箱" };
				//(4)	若[A]存在
				else
				{
					//A.	[B]=取得F053202 WHERE F0531_ID=<參數1>
					var f053202s = f053202Repo.GetDataByF0531Id(outContainerInfo.F0531_ID);

					//B.	[C]=取得[B]的商品序號清單 條件:[B].SERIAL_NO 不是NULL OR 空白
					var serialList = f053202s.Where(w => !string.IsNullOrWhiteSpace(w.SERIAL_NO)).ToList();

					//C.	如果[C]有資料，檢查[C]的商品序號狀態，呼叫CommonSerivce.GetSnList
					if (serialList.Any())
					{
						var f2501s = CommonService.GetItemSerialList(f0531.GUP_CODE, f0531.CUST_CODE, serialList.Select(s => s.SERIAL_NO).ToList());

						//a. 檢查商品序號狀態是否可在庫(A1)，若有一筆序號非在庫序號(C1 OR D2)
						if (f2501s.Any(a => a.STATUS != "A1"))
						{
							return new ExecuteResult
							{
								IsSuccessed = false,
								Message = $"此跨庫箱號{ outContainerInfo.OUT_CONTAINER_CODE }含有非在庫序號，不可關箱，請先進行[重新裝箱]後，再到新稽核出庫作業"
							};
						}

						//c.	檢查商品序號.ACTIVEED=1(不良品序號)，若有一筆序號為不良品序號
						if (f2501s.Any(a => a.ACTIVATED == "1"))
						{
							return new ExecuteResult
							{
								IsSuccessed = false,
								Message = $"此跨庫箱號{ outContainerInfo.OUT_CONTAINER_CODE }含有不良品序號，不可關箱，請先進行[重新裝箱]後，再到新稽核出庫作業"
							};
						}

						//d.	檢查商品序號是否凍結
						var freezeSerialList = f2501Repo.GetSerialIsFreeze(outContainerInfo.GUP_CODE, outContainerInfo.CUST_CODE, "02", serialList.Select(s => s.SERIAL_NO).ToList()).ToList();
						if (freezeSerialList.Any())
						{
							return new ExecuteResult
							{
								IsSuccessed = false,
								Message = $"此跨庫箱號{ outContainerInfo.OUT_CONTAINER_CODE }含有凍結序號，不可關箱，請先進行[重新裝箱]後，再到新稽核出庫作業"
							};
						}

						//e.如果以上檢核通過，更新F2501.STATUS = C1(出庫)
						f2501s.ForEach(f => f.STATUS = "C1");
						f2501Repo.BulkUpdate(f2501s);
					}

					//D.	更新F0532.STATUS=1(關箱),CLOSE_STAFF=目前登入者帳號,CLOSE_NAME=目前登入者姓名,CLOSE_DATE=系統時間 WHERE F0531_ID=<參數1>
					f0532Repo.UpdateCloseInfoByF0531Id(outContainerInfo.F0531_ID);

					//E.	新增F0533 STATUS=2(已關箱)
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

					//F.	更新F0534 STATUS=1(已放入並關閉稽核箱待分配) WHERE F0701_ID IN(SELECT F0701_ID FROM F053201 WHERE F0531_ID = <參數1>)
					f0534Repo.UpdateCloseInfoByF0531Id(outContainerInfo.F0531_ID);

					//G.	釋放使用中揀貨容器 : 刪除 F0530 WHERE F0701_ID IN(SELECT F0701_ID FROM F053201 WHERE F0531_ID =< 參數1 >)
					f0530Repo.DeleteByF0531Id(outContainerInfo.F0531_ID);

          // 寫入F060302釋放容器
          f060302Repo.AddReleaseRecord2(outContainerInfo.F0531_ID);

          //H.	釋放容器綁定 DELETE  F0701 WHERE ID IN(SELECT F0701_ID FROM F053201 WHERE F0531_ID = <參數1>)
          f0701Repo.DeleteByF0531Id(outContainerInfo.F0531_ID);

					//I.	刪除F0531 WHERE ID=<參數1>
					f0531Repo.Delete(x => x.ID == outContainerInfo.F0531_ID);

					return new ExecuteResult { IsSuccessed = true };
				}
			}
			catch (Exception ex)
			{
				return new ExecuteResult { IsSuccessed = false, Message = ex.Message };
			}
			finally
			{
				UnlockOutContainerProcess(new List<string>() { outContainerInfo.OUT_CONTAINER_CODE });
			}
		}
	}
}
