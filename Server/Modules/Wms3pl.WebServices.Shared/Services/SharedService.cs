using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.OData.Query.SemanticAst;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.Datas.F70;
using Wms3pl.Common;
using System.Net;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.Datas.F02;
using System.IO;
using System.Transactions;
using Wms3pl.Datas.F20;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class SharedService
	{
		private WmsTransaction _wmsTransaction;
		public SharedService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 取得單據類型的唯一單號
		/// <summary>
		/// 取得單據類型的唯一單號
		/// </summary>
		/// <param name="ordType">單據類型
		///A 進倉單      B 商品歸還
		///C 商品召回單  D 銷毀單
		///E 換貨單      F 商品外送
		///H 內部交易單
		///I 盤點單      J 調整單
		///K 揀貨彙總批單
		///L 商品借出
		///M 調撥申請單
		///O 出貨單      P 揀貨單
		///Q 報價單      R 退貨單
		///S 系統訂單    T 調撥單
		///U AGV揀貨彙總單  V 廠商退貨單
		///W 流通加工單  X 報廢單
		///Z 系統保留單號
		///ZN 退貨點收不明件  ZR 退貨彙總表
		///ZP 採購單          ZW 加工撿料單
		///ZA 系統作業通知		ZO 廠退出貨單
		///ZN 退貨點收不明件
		/// </param>		
		/// <returns></returns>
		public string GetNewOrdCode(string ordType)
		{
			return GetNewOrdCodes(ordType, count: 1).FirstOrDefault();
		}

		/// <summary>
		/// 可一次取回多筆單號
		/// </summary>
		/// <param name="ordType"></param>
		/// <param name="count">一次取回多筆的數量</param>
		/// <returns>回傳 Stack 類型</returns>
		public Stack<string> GetNewOrdStackCodes(string ordType, int count)
		{
			return new Stack<string>(GetNewOrdCodes(ordType, count));
		}

		/// <summary>
		/// 可一次取回多筆單號
		/// </summary>
		/// <param name="ordType"></param>
		/// <param name="count">一次取回多筆的數量</param>
		/// <returns></returns>
		private IEnumerable<string> GetNewOrdCodes(string ordType, int count)
		{
			if (count < 0)
			{
				throw new ArgumentException("取得單號的數量不可小於0!", "count");
			}
			else if (count == 0)
			{
				return new List<string>();
			}

			var f0009 = GetNewF0009(ordType, count);
			if (ordType == "BOX" || ordType == "GOX")
				return CombineNewOrdCodes(f0009, count, noFormat: "yyMMdd");

			return CombineNewOrdCodes(f0009, count);
		}

		public F0009 GetNewF0009(string ordType, int count)
		{
			var f0009Rep = new F0009Repository(Schemas.CoreSchema);
			var f9 = f0009Rep.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
			() =>
			{
				var lockF0009 = f0009Rep.LockF0009();
				var f0009 = f0009Rep.Find(item => item.ORD_TYPE == ordType, isForUpdate: true, isByCache: false);
				if (f0009 == null)
				{
					f0009 = new F0009()
					{
						ORD_TYPE = ordType,
						ORD_DATE = DateTime.Today,
						ORD_SEQ = count
					};

					f0009Rep.Add(f0009);
				}
				else
				{
					if (f0009.ORD_DATE.Date != DateTime.Today)
					{
						f0009.ORD_DATE = DateTime.Today;
						f0009.ORD_SEQ = count;
					}
					else
					{
						f0009.ORD_SEQ += count;
					}

					f0009Rep.Update(f0009);
				}
				return f0009;
			});
			return f9;
		}


		/// <summary>
		/// 組合單據單號格式
		/// </summary>
		/// <param name="f0009"></param>
		/// <returns></returns>
		public static IEnumerable<string> CombineNewOrdCodes(F0009 f0009, int count, string noFormat = "yyyyMMdd")
		{
			var noType = f0009.ORD_TYPE;
			var noDate = f0009.ORD_DATE.ToString(noFormat);

			return Enumerable.Range(f0009.ORD_SEQ - count + 1, count)
							 .Select(seq => string.Format("{0}{1}{2}", noType,
																		 noDate,
																		 seq.ToString("D6"))).Reverse();
		}


		#endregion 取得單據類型的唯一單號


		#region 流通加工單
		public ExecuteResult InsertF910201(string dcCode, string gupCode, string custCode, string processSource, string outsourceId, DateTime finishDate, string itemCode, string itemCodeBom, int processQty, int boxQty, int caseQty, string orderNo, string memo, string quoteNo, string finishTime, string procType = "0")
		{
			ExecuteResult result = null;
			var repo = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);
			string processNo = sharedService.GetNewOrdCode("W");

			var f910201 = new F910201
			{
				PROCESS_NO = processNo,
				QUOTE_NO = quoteNo,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				STATUS = "0",
				PROC_STATUS = "0",
				PROCESS_SOURCE = processSource,
				OUTSOURCE_ID = outsourceId,
				FINISH_DATE = finishDate,
				ITEM_CODE = itemCode,
				ITEM_CODE_BOM = itemCodeBom,
				PROCESS_QTY = processQty,
				BOX_QTY = boxQty,
				CASE_QTY = caseQty,
				ORDER_NO = orderNo,
				BREAK_QTY = 0,
				A_PROCESS_QTY = 0,
				MEMO = memo,
				FINISH_TIME = finishTime,
				PROC_TYPE = procType
			};
			repo.Add(f910201);

			if (result == null)
				result = new ExecuteResult() { IsSuccessed = true, Message = processNo };

			return result;
		}
		#endregion 流通加工單

		#region 統編檢核
		/// <summary>
		/// 檢查統編是否已存在
		/// </summary>
		/// <param name="uniForm"></param>
		/// <returns></returns>
		public bool ExistsUniForm(string uniForm)
		{
			if (string.IsNullOrWhiteSpace(uniForm))
				return false;

			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909 = f1909Repo.Filter(item => item.UNI_FORM == EntityFunctions.AsNonUnicode(uniForm) && item.STATUS != "9");
			if (f1909.Any())
				return true;

			var f1928Repo = new F1928Repository(Schemas.CoreSchema);
			var f1928 = f1928Repo.Filter(item => item.UNI_FORM == EntityFunctions.AsNonUnicode(uniForm) && item.STATUS != "9");
			if (f1928.Any())
				return true;

			return false;
		}
		#endregion

		#region 取 Ticket ID

		public decimal GetTicketID(string dcCode, string gupCode, string custCode, string ticketClass)
		{
			var f190001repo = new F190001Repository(Schemas.CoreSchema);
			var f190001Data = f190001repo.GetTicketID(dcCode, gupCode, custCode, ticketClass).ToList().FirstOrDefault();
			return f190001Data == null ? 0 : f190001Data.TICKET_ID;
		}

		#endregion 取 Ticket ID

		#region 檢查是否有此 MileStone

		public bool CheckHasMileStone(decimal ticketId, string mileStoneNo)
		{
			var f19000101repo = new F19000101Repository(Schemas.CoreSchema);
			return f19000101repo.GetDatasByTrueAndCondition(a => a.TICKET_ID == ticketId && a.MILESTONE_NO == mileStoneNo).Any();
		}

		#endregion 檢查是否有此 MileStone


		#region 由來源單據取出貨單清單
		public IQueryable<F050801WmsOrdNo> GetF050801ListBySourceNo(string dcCode, string gupCode, string custCode, string sourceNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF050801ListBySourceNo(dcCode, gupCode, custCode, sourceNo);
		}
		#endregion

		#region 作業類別轉單據類型
		public string ConvertOrdPropToTicketType(string ordProp)
		{
			switch (ordProp)
			{
				case "A1":
				case "A3":
				case "A4":
				case "A5":
				case "A6":
					return "A1";
				case "A2":
					return "A2";
				case "R1":
				case "R2":
				case "R3":
				case "R4":
				case "R5":
				case "R6":
				case "R7":
				case "R8":
				case "R9":
					return "R1";
				default:
					return "";
			}
		}
		#endregion 作業類別轉單據類型

		#region 取得貨主有權限或共用倉別的來源儲位

		/// <summary>
		/// 取得貨主有權限或共用倉別的來源儲位
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="warehouseType">倉別型態F198001</param>
		/// <returns></returns>
		public F1912 GetSrcLoc(string dcCode, string gupCode, string custCode, string warehouseType)
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			//1.取貨主有權的倉別型態儲位
			var datas = f1912Repo.GetDatas(dcCode, warehouseType, gupCode, custCode);
			if (datas.Any())
				return datas.First();
			//2.取業主有權貨主共用的倉別型態儲位
			datas = f1912Repo.GetDatas(dcCode, warehouseType, gupCode);
			if (datas.Any())
				return datas.First();
			//3.取得物流中心共用業主貨主的倉別型態儲位
			datas = f1912Repo.GetDatas(dcCode, warehouseType);
			if (datas.Any())
				return datas.First();
			return null;
		}

		#endregion

		#region 更新來源單號狀態

		private List<F151001> _cacheF151001s;
		public List<F151001> GetF151001sByCache(string dcCode, string gupCode, string custCode, List<string> allocationNos)
		{
			if (_cacheF151001s == null)
				_cacheF151001s = new List<F151001>();
			var datas = _cacheF151001s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && allocationNos.Contains(x.ALLOCATION_NO)).ToList();
			var exceptAllocations = allocationNos.Except(datas.Select(x => x.ALLOCATION_NO)).ToList();
			if (exceptAllocations.Any())
			{
				var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
				var f151001s = f151001Repo.GetDatas(dcCode, gupCode, custCode, exceptAllocations).ToList();
				_cacheF151001s.AddRange(f151001s);
				datas.AddRange(f151001s);
			}
			return datas;
		}

		/// <summary>
		/// 更新來源單號狀態
		/// </summary>
		/// <param name="sourceType">來源單據類型</param>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="wmsNo">各類單據單號(非來源單號)</param>
		/// <param name="wmsNoStatus">單據單號狀態(非來源單號)</param>
		public ExecuteResult UpdateSourceNoStatus(SourceType sourceType, string dcCode, string gupCode, string custCode, string wmsNo, string wmsNoStatus)
		{
			return UpdateSourceNoStatus(sourceType, dcCode, gupCode, custCode, new List<string> { wmsNo }, wmsNoStatus);
		}

		/// <summary>
		/// 更新來源單號狀態
		/// </summary>
		/// <param name="sourceType">來源單據類型</param>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="wmsNos">各類單據單號(非來源單號)</param>
		/// <param name="wmsNoStatus">單據單號狀態(非來源單號)</param>
		public ExecuteResult UpdateSourceNoStatus(SourceType sourceType, string dcCode, string gupCode, string custCode, IEnumerable<string> wmsNos, string wmsNoStatus)
		{
			var sourceList = new List<SourceItem>();
			/* 這邊用來尋找來源單據 */
			string wmsNo = wmsNos.FirstOrDefault();
			switch (sourceType)
			{
				case SourceType.Stock:  //進倉
					var f010201Repo = new F010201Repository(Schemas.CoreSchema);
					var f010201 = f010201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.STOCK_NO == wmsNo);
					if (f010201 != null)
						sourceList.Add(new SourceItem { SourceNo = f010201.SOURCE_NO, SourceType = f010201.SOURCE_TYPE });
					break;
				case SourceType.Return: //退貨
					var f161201Repo = new F161201Repository(Schemas.CoreSchema);
					var f161201 = f161201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.RETURN_NO == wmsNo);
					if (f161201 != null && !string.IsNullOrEmpty(f161201.SOURCE_NO))
						sourceList.Add(new SourceItem { SourceNo = f161201.SOURCE_NO, SourceType = f161201.SOURCE_TYPE });
					break;
				case SourceType.Order: //出貨
					var f050301Repo = new F050301Repository(Schemas.CoreSchema);
					var statusParam = Convert.ToInt16(wmsNoStatus);
					// 批次扣帳可有多張出貨單，需一次帶入處理，避免Transaction問題
					foreach (var curWmsOrdNo in wmsNos)
					{
						// 取得該出貨單關聯的所有訂單與出貨單
						var f050301Datas = f050301Repo.GetWmsOrdNoWithF050301Data(dcCode, gupCode, custCode, curWmsOrdNo).ToList();

						// 排除此次要扣帳的出貨單以外，都必須已完成扣帳才能更新來源單據(除 SourceType ="09" 內部交易不應在這裡結案，沒有來源單據的也不用更新 )
						if (f050301Datas.Where(x => !wmsNos.Contains(x.WMS_ORD_NO)).All(x => x.STATUS == statusParam))
						{
							foreach (var order in f050301Datas.GroupBy(x => new { x.ORD_NO, x.SOURCE_NO, x.SOURCE_TYPE })
																.Where(g => g.Key.SOURCE_TYPE != "09")
																.Where(g => !string.IsNullOrEmpty(g.Key.SOURCE_NO)))
							{
								sourceList.Add(new SourceItem
								{
									SourceNo = order.Key.SOURCE_NO,
									SourceType = order.Key.SOURCE_TYPE
								});
							}
						}
					}
					break;
				case SourceType.Allocation: //調撥
					var f151001Repo = new F151001Repository(Schemas.CoreSchema);
					var f151001 = f151001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == wmsNo);
					if (f151001 != null)
						sourceList.Add(new SourceItem { SourceNo = f151001.SOURCE_NO, SourceType = f151001.SOURCE_TYPE });
					break;
			}

			/* 這邊負責更新來源單據的狀態 */
			foreach (var sourceItem in sourceList.Where(x => !string.IsNullOrEmpty(x.SourceNo)))
			{
				//抓取設定檔 f00090201 
				//THIS_STATUS : SourceType (進倉/退貨/出貨/調撥) 單據狀態
				var f00090201Repo = new F00090201Repository(Schemas.CoreSchema, _wmsTransaction);
				var sourceTypeId = (int)sourceType;
				var f00090201Result = f00090201Repo.Find(o => o.WORK_TYPE == sourceTypeId
														 && o.SOURCE_TYPE == sourceItem.SourceType
														 && o.THIS_STATUS == wmsNoStatus);
				if (f00090201Result == null)
					return new ExecuteResult(false, string.Format("F00090201未定義 WORK_TYPE:{0}, SOURCE_TYPE:{1}, THIS_STATUS:{2}", sourceTypeId, sourceItem.SourceType, wmsNoStatus));

				var updateStatus = f00090201Result.UPDATE_STATUS;

				#region 更新所有來源 Table 程式
				switch (sourceItem.SourceType)
				{
					case "04":
						//1.取得F02020107 Where DC_CODE = dcCode AND GUP_CODE = gupCode AND CUST_CODE = custCode AND ALLOCATION_NO = wmsNo
						var f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransaction);
						var f02020107 = f02020107Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode
																								&& o.ALLOCATION_NO == wmsNo);

						if (f02020107 != null)
						{
							//2.寫入F010205(當不存在才新增)
							//     STOCK_NO = SOURCE_NO,RT_NO = F02020107.RT_NO,ALLOCATION_NO = F02020107.ALLOCATION_NO,STATUS = 3,,PROC_FLAG = 0
							var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction);
							var f010205 = f010205Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode
																								&& o.STOCK_NO == sourceItem.SourceNo && o.RT_NO == f02020107.RT_NO && o.ALLOCATION_NO == f02020107.ALLOCATION_NO
																								&& o.STATUS == "3" && o.PROC_FLAG == "0");

							if (f010205 == null)
							{
								f010205Repo.Add(new F010205
								{
									DC_CODE = dcCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									STOCK_NO = sourceItem.SourceNo,
									RT_NO = f02020107.RT_NO,
									ALLOCATION_NO = f02020107.ALLOCATION_NO,
									STATUS = "3",
									PROC_FLAG = "0"
								});
							}
						}

						break;
					case "05": //外送
					case "08": //銷毀
						var f160501Repo = new F160501Repository(Schemas.CoreSchema, _wmsTransaction);
						var f160501 = f160501Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode
														&& o.DESTROY_NO == sourceItem.SourceNo);
						if (f160501 != null)
						{
							if (updateStatus == "2")
								f160501.POSTING_DATE = DateTime.Now;

							//銷毀明細若全是虛擬商品時直接押結案 Status =4
							var itemData = f160501Repo.GetF160501ItemType(dcCode, gupCode, custCode, sourceItem.SourceNo);
							if (itemData != null && itemData.Any())
							{
								//若全部都虛擬商品則押結案
								if (!itemData.Any(o => string.IsNullOrEmpty(o.VIRTUAL_TYPE)))
								{
									updateStatus = "4";
								}
							}
							f160501.STATUS = updateStatus;
							f160501Repo.Update(f160501);
						}


						break;
					case "10": //加工
						var f910201Repo = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);
						var f910201 = f910201Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROCESS_NO == sourceItem.SourceNo);
						if (f910201.STATUS == "1") //加工單狀態為加工中=>處理揀料單調撥，如果所有揀料單調撥都已結案就更新加工作業狀態為4(啟動加工)
						{
							var f91020502Repo = new F91020502Repository(Schemas.CoreSchema, _wmsTransaction);
							var allocationNos = f91020502Repo.GetPickAllocationNos(dcCode, gupCode, custCode, sourceItem.SourceNo).ToList();
							var f151001s = GetF151001sByCache(dcCode, gupCode, custCode, allocationNos);
							var nowF151001 = f151001s.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALLOCATION_NO == wmsNo);
							if (nowF151001 != null)
								nowF151001.STATUS = "5";
							if (f151001s.All(x => x.STATUS == "5"))
							{
								f910201.PROC_STATUS = "4"; //啟動加工
								f910201.PROC_BEGIN_DATE = DateTime.Now;
								f910201Repo.Update(f910201);
							}
						}
						if (f910201.STATUS == "2") //加工單狀態為加工完成=>處理上架回倉調撥單，如果所有上架回倉調撥單都完成就將狀態改為3(結案)、作業狀態改為6(上架完成)
						{
							var f91020601Repo = new F91020601Repository(Schemas.CoreSchema, _wmsTransaction);
							var allocationNos = f91020601Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PROCESS_NO == sourceItem.SourceNo).Select(x => x.ALLOCATION_NO).Distinct().ToList();
							var f151001s = GetF151001sByCache(dcCode, gupCode, custCode, allocationNos);
							var nowF151001 = f151001s.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALLOCATION_NO == wmsNo);
							if (nowF151001 != null)
								nowF151001.STATUS = "5";
							if (f151001s.All(x => x.STATUS == "5"))
							{
								f910201.PROC_STATUS = "6"; //上架完成
								f910201.STATUS = "3";//結案
								f910201Repo.Update(f910201);
							}
						}
						break;
					case "11": //代採購
						var f010101Repo = new F010101Repository(Schemas.CoreSchema, _wmsTransaction);
						var f010101 = f010101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode
														&& o.SHOP_NO == sourceItem.SourceNo);
						if (f010101 != null)
						{
							f010101.STATUS = updateStatus;
							f010101Repo.Update(f010101);
						}
						break;
					case "12": //報廢						
						var f160401Repo = new F160401Repository(Schemas.CoreSchema, _wmsTransaction);
						var f160401 = f160401Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode
														&& o.SCRAP_NO == sourceItem.SourceNo);
						if (f160401 != null)
						{
							f160401.STATUS = updateStatus;
							f160401Repo.Update(f160401);
						}
						break;
					case "13": //廠退出貨
						var f160201Repo = new F160201Repository(Schemas.CoreSchema, _wmsTransaction);
						var f160201s = f160201Repo.AsForUpdate()
													.GetVnrQtyEqWmsQtyF160201(dcCode,
																			gupCode,
																			custCode,
																			rtnWmsNo: sourceItem.SourceNo,
																			filterWmsOrdNos: wmsNos)
													.ToList();

						foreach (var f160201 in f160201s)
						{
							f160201.STATUS = updateStatus;
							f160201.POSTING_DATE = DateTime.Now;

							f160201Repo.Update(f160201);
						}
						break;
				}
				#endregion

			}

			return new ExecuteResult(true);
		}
		#endregion

		#region 派車
		/// <summary>
		/// 產生派車單
		/// </summary>
		/// <param name="f700101"></param>
		/// <param name="f700102s"></param>
		public void CreateDistributeCar(IEnumerable<F700101> f700101s, IEnumerable<F700102> f700102s)
		{
			var f700101Rep = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700102Rep = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);

			f700101Rep.BulkInsert(f700101s);
			f700102Rep.BulkInsert(f700102s);
		}

		#endregion 派車

		#region 取郵遞區號及地址格式化
		/// <summary>
		/// 解析地址，解析成功的話，會取得郵遞區號與解析地址字串，沒成功就是地址錯誤或者有例外錯誤
		/// </summary>
		/// <param name="result"></param>
		public void ParseAddress(AddressParsedResult result)
		{
			//郵局
			try
			{
				var postParser = new PostOfficeAddressParser(result.ADDRESS);
				postParser.Run();
				result.ZIP_CODE = postParser.ShortZipCode;
				result.ADDRESS_PARSE = postParser.ParsedAddress;
				result.Exception = null;
			}
			catch (Exception ex)
			{
				// 使用郵局解析失敗，繼續解析
				result.Exception = ex;
			}

			if (result.IsSucceedParsedZipCode)
			{
				// 解析地址成功就直接回傳
				return;
			}

			try
			{
				//Google
				var googleOriginalAddress = result.ADDRESS;
				if (!googleOriginalAddress.Contains("台灣") && !googleOriginalAddress.Contains("臺灣"))
					googleOriginalAddress = string.Format("台灣{0}", googleOriginalAddress);
				var googleParser = new GoogleMapAddressParser(googleOriginalAddress);
				googleParser.Run();
				result.ZIP_CODE = googleParser.ShortZipCode;
				result.ADDRESS_PARSE = googleParser.ParsedAddress;
				result.Exception = null;
			}
			catch (Exception ex)
			{
				result.Exception = ex;
				return;
			}

			result.HasParsedSucceed = true;
		}

		#endregion 取郵遞區號及地址格式化

		#region 訊息池
		/// <summary>
		/// 新增內部的訊息池
		/// </summary>
		/// <param name="ticketType"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="messageContent"></param>
		/// <param name="notifyOrdNo">單號</param>
		/// <param name="workNo">作業別代號</param>
		/// <param name="loMainNotify"></param>
		/// <param name="exTransaction">獨立執行的WmsTransaction物件</param>
		public void AddMessagePool(string ticketType, string dcCode, string gupCode, string custCode, string msgNo, string messageContent, string notifyOrdNo, string targetType, string targetCode, bool isNoTransaction = false)
		{
			WmsTransaction wmsTransaction = null;
			if (!isNoTransaction)
				wmsTransaction = _wmsTransaction;
			var messageService = new MessageService(wmsTransaction);
			if (string.IsNullOrEmpty(notifyOrdNo))
			{
				notifyOrdNo = GetNewOrdCode(ticketType);
			}

			// Sequence 預設由 Tigger 來增加
			decimal messageId = 0;
			// 但若需要寫入行事曆的話，就從這邊讀取，分別將 loMessagePoolId 寫入到訊息池與行事曆
			switch (ticketType)
			{
				case "9":   // 配庫
					if (targetType == "0")//發送對象為物流中心的才要寫到行事曆
					{
						messageId = GetTableSeqId("SEQ_MESSAGE_ID");
						AddLoMessageToF700501Schedule(wmsTransaction, dcCode, gupCode, custCode, msgNo, messageContent, targetCode,
							messageId);
					}
					break;
			}
			messageService.AddWmsMessage(dcCode, gupCode, custCode, msgNo, messageContent, targetType, targetCode, null, null, messageId);
		}

		public ExecuteResult AddLoMessageToF700501Schedule(WmsTransaction wmsTransaction, string dcCode, string gupCode, string custCode, string msgNo, string messageContent, string targetCode, decimal messageId)
		{
			var f0020Repo = new F0020Repository(Schemas.CoreSchema, wmsTransaction);
			var f190003Repo = new F190003Repository(Schemas.CoreSchema, wmsTransaction);

			var subject = f0020Repo.Filter(x => x.MSG_NO == EntityFunctions.AsNonUnicode(msgNo))
									 .Select(x => x.MSG_SUBJECT)
									 .FirstOrDefault();

			if (subject == "API訊息") // API不再寫入行事曆
				return new ExecuteResult(true);

			var f700501 = new F700501
			{
				DC_CODE = dcCode,
				SCHEDULE_DATE = DateTime.Today,
				SCHEDULE_TIME = DateTime.Now.ToString("HH:mm"),
				SCHEDULE_TYPE = "W", // W: 待辦事項
				IMPORTANCE = "1", // 1: 重要性一般
				SUBJECT = subject,
				CONTENT = messageContent,
				MESSAGE_ID = messageId
			};

			var f70050101s = f190003Repo.GetGrpId1s(dcCode, gupCode, custCode, targetCode)
										.Select(x => new F70050101 { GRP_ID = x })
										.ToArray();

			return InsertF700501(wmsTransaction, f700501, f70050101s);
		}

		#endregion 訊息池

		/// <summary>
		/// 新增物流中心行事曆
		/// </summary>
		/// <param name="wmsTransaction">由於Lo Message Pool會新增，不能用類別的 _wmsTransaction</param>
		/// <param name="addF700501"></param>
		/// <param name="addF70050101s"></param>
		/// <returns></returns>
		public ExecuteResult InsertF700501(WmsTransaction wmsTransaction, F700501 addF700501, F70050101[] addF70050101s)
		{
			var f700501Repo = new F700501Repository(Schemas.CoreSchema, wmsTransaction);
			var f70050101Repo = new F70050101Repository(Schemas.CoreSchema, wmsTransaction);
			var newOrdCode = GetNewOrdCode("ZS");
			addF700501.SCHEDULE_NO = newOrdCode;
			f700501Repo.Add(addF700501);

			foreach (var item in addF70050101s)
			{
				item.SCHEDULE_NO = newOrdCode;
				item.DC_CODE = addF700501.DC_CODE;
				f70050101Repo.Add(item);
			}

			return new ExecuteResult() { IsSuccessed = true, Message = "已新增行事曆" };
		}

		#region 取Tabel 自動編號 Seq
		public static decimal GetTableSeqId(string tableSeqId)
		{
			var f0000Repo = new F0000Repository(Schemas.CoreSchema);
			var returnId = f0000Repo.GetTableSeqId(tableSeqId).FirstOrDefault();
			return returnId;
		}
		#endregion

		#region 商品搬動儲位時 : 更新儲位使用容量 -(調撥)
		/// <summary>
		/// 調撥完成時，就更新該調撥單相關儲位的已使用量
		/// </summary>
		/// <param name="f151001"></param>
		/// <param name="f151002s"></param>
		public void UpdateAllocationLocVolumn(F151001 f151001, List<F151002> f151002s)
		{
			// 可能不同物流中心
			var dcGroups = f151002s.Select(x => new
			{
				DC_CODE = f151001.DC_CODE,
				GUP_CODE = f151001.GUP_CODE,
				CUST_CODE = f151001.CUST_CODE,
				LOC_CODE = x.SRC_LOC_CODE
			}).Concat(f151002s.Select(x => new
			{
				DC_CODE = f151001.TAR_DC_CODE,
				GUP_CODE = f151001.GUP_CODE,
				CUST_CODE = f151001.CUST_CODE,
				LOC_CODE = x.TAR_LOC_CODE
			})).Where(x => !string.IsNullOrEmpty(x.LOC_CODE))
				 .GroupBy(x => new
				 {
					 x.DC_CODE,
					 x.GUP_CODE,
					 x.CUST_CODE
				 });

			foreach (var g in dcGroups)
			{
				var locCodes = g.Select(x => x.LOC_CODE).Distinct().ToList();
				UpdateUsedVolumnByLocCodes(g.Key.DC_CODE, g.Key.GUP_CODE, g.Key.CUST_CODE, locCodes);
			}
		}
		#endregion

		#region 商品搬動儲位時 : 更新儲位使用容量 -(揀貨)
		/// <summary>
		/// 更新要揀貨的儲位已使用容積量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNos"></param>
		public void UpdatePickOrdNoLocVolumn(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
		{
			//var pickLocCodes = GetPickLocCodes(dcCode, gupCode, custCode, pickOrdNos);
			//UpdateUsedVolumnByLocCodes(dcCode, gupCode, custCode, pickLocCodes);
		}

		private List<KeyValuePair<string, List<string>>> dcCacheLocs;
		/// <summary>
		/// 從現有商品庫存中+尚未揀貨，更新指定的儲位已用容積(此功能已取消無作用)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="locCodes"></param>
		public void UpdateUsedVolumnByLocCodes(string dcCode, string gupCode, string custCode, IEnumerable<string> locCodes)
		{
      //換新的方法去重算容積，直接return
      return;
      /*
      if (locCodes.Any())
			{
				var f191205Repo = new F191205Repository(Schemas.CoreSchema, _wmsTransaction);
				//2020/02/24 要去重複Loc
				locCodes = locCodes.Where(x=> x != "000000000").Distinct().ToList();
				if (dcCacheLocs == null)
				{
					dcCacheLocs = new List<KeyValuePair<string, List<string>>>();
				}
				var item = dcCacheLocs.Find(x => x.Key == dcCode);
				//2022/04/21 同一個transation 要排除重複新增的儲位
				var cacheLocs = item.Equals(default(KeyValuePair<string, List<string>>)) ? new List<string>() : item.Value;
				var insertLocs = locCodes.Except(cacheLocs).ToList();

				var batchCnt = 1000;
				var page = Math.Ceiling(insertLocs.Count() / (decimal)batchCnt);

				for(var i =0; i<page;i++)
				{
					var pageLocCodes = insertLocs.Skip(i * batchCnt).Take(batchCnt);
					// 2022/4/16 調整為先刪後新增避免同時執行時發生sql exception pk錯誤問題
					f191205Repo.DeleteInWithTrueAndCondition<string>(x => x.DC_CODE == dcCode, x => x.LOC_CODE, pageLocCodes);
          f191205Repo.BulkInsert(pageLocCodes.Select(x => new F191205
          {
            DC_CODE = dcCode,
            LOC_CODE = x
          }).ToList());
        }
				if (item.Equals(default(KeyValuePair<string, List<string>>)))
				{
					dcCacheLocs.Add(new KeyValuePair<string, List<string>>(dcCode, insertLocs));
				}
				else
				{
					item.Value.AddRange(insertLocs);
				}
			}
      */
		}

		/// <summary>
		/// 取得揀貨明細的儲位編號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNoList"></param>
		/// <returns></returns>
		public List<string> GetPickLocCodes(string dcCode, string gupCode, string custCode, List<string> pickOrdNoList)
		{
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var locCodes = f051202Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
												&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
												&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
												&& pickOrdNoList.Contains(x.PICK_ORD_NO))
										.Select(x => x.PICK_LOC)
										.Distinct()
										.ToList();
			return locCodes;
		}

		#endregion

		#region 取得商品資訊(以F1903業主主檔為主)
		public IQueryable<F1903Plus> GetItemInfo(string gupCode, string custCode, string itemCode)
		{
			var repo = new F1903Repository(Schemas.CoreSchema);
			return repo.GetItemInfo(gupCode, custCode, itemCode);
		}
		#endregion

		#region 自取檢查

		/// <summary>
		/// 地址是否符合與物流中心地址相同
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public bool IsMatchAddressIsDCAddress(string dcCode, string address)
		{
			if (string.IsNullOrEmpty(address)) return false;
			var f1901Repo = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
			return f1901Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
										&& x.ADDRESS == EntityFunctions.AsNonUnicode(address)).Any();
		}

		#endregion

		public int GetQtyByRatio(int itemQty, string itemCode, string gupCode, string custCode, string orderTicketNo)
		{
			var qty = 0;
			var f1903Rep = new F1903Repository(Schemas.CoreSchema);
			var ratio = f1903Rep.GetRatio(itemCode, gupCode, custCode, orderTicketNo);
			if (ratio.HasValue)
				qty = (int)Math.Round((Decimal)(itemQty * (ratio.Value / 100)), 0, MidpointRounding.AwayFromZero);
			if (itemQty == 0)
				qty = 0;
			else if (qty == 0)
				qty = 1;
			return qty;
		}

		#region 包材回復
		/// <summary>
		/// 回復包材庫存
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdCode"></param>
		public ExecuteResult ReturnBoxQty(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			var f055001Repo = new F055001Repository(Schemas.CoreSchema);
			//只有PRINT_FLAG=1 才有扣包材庫存 所以只回復PRINT_FLAG=1的包材數
			var tmpF055001Data = f055001Repo.GetDatas(wmsOrdCode, gupCode, custCode, dcCode);
			// 原箱商品不需要回復包材
			tmpF055001Data = tmpF055001Data.Where(x => x.PRINT_FLAG == 1 && x.BOX_NUM != "ORI");
			var tmpF055001MultiBoxData = tmpF055001Data.GroupBy(a => a.BOX_NUM).ToList();

			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1912repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1912 = f1912repo.GetReturnF055001Datas(dcCode, gupCode, custCode, "S");// 'S' =>耗材倉
			if (f1912.Any())
			{
				//增加多包材狀況的資料
				foreach (var a in tmpF055001MultiBoxData)
				{
					if (!string.IsNullOrWhiteSpace(a.Key))
					{
						int tmpCnt = a.Count();
						var f1913 = f1913Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
																						&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
																						&& x.ITEM_CODE == EntityFunctions.AsNonUnicode(a.Key))
																	.FirstOrDefault();
						if (f1913 == null)
						{
							f1913 = new F1913
							{
								CUST_CODE = custCode,
								DC_CODE = dcCode,
								ENTER_DATE = DateTime.Today,
								GUP_CODE = gupCode,
								ITEM_CODE = a.Key,
								LOC_CODE = f1912.FirstOrDefault().LOC_CODE,
								QTY = tmpCnt,
								VALID_DATE = Convert.ToDateTime("9999/12/31"),
								VNR_CODE = "000000",
								BOX_CTRL_NO = "0",
								PALLET_CTRL_NO = "0",
								MAKE_NO = "0"
							};
							f1913Repo.Add(f1913);
						}
						else
						{
							f1913.QTY += tmpCnt;
							f1913Repo.Update(f1913);
						}
					}
				}

				return new ExecuteResult() { IsSuccessed = true };
			}
			else
			{
				return new ExecuteResult() { IsSuccessed = false, Message = "尚未設定耗材倉的儲位!" };

			}

		}


		#endregion

		#region 取得單一商品包裝參考資料
		/// <summary>
		/// 取得單一商品包裝參考資料
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="stockQty"></param>
		/// <returns></returns>
		public string GetSingleVolumeUnit(string gupCode, string custCode, string itemCode, int stockQty)
		{
			var itemService = new ItemService();
			var itemPackageRef = itemService.CountItemPackageRefList(gupCode, custCode, new List<ItemCodeQtyModel> { new ItemCodeQtyModel { ItemCode = itemCode, Qty = stockQty } }).FirstOrDefault();
			return itemPackageRef == null ? null : itemPackageRef.PackageRef;
		}
		#endregion

		#region 快速驗收

		/// <summary>
		/// 取得驗收單號
		/// 依照大買家邏輯, 取yyyyMMdd0000. 取回之後刪除前一日的資料.
		/// 當日序號為GROUP BY DC/GUP/CUR_DATE
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public string GetRtNo(string dcCode, string gupCode, string custCode, string userId = "")
		{
			var repo = new F02020103Repository(Schemas.CoreSchema);
			var f02020103 = repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
				() =>
				{
					var lockF02020103 = repo.LockF02020103();
					var tmp = repo.Find(x => x.DC_CODE.Equals(EntityFunctions.AsNonUnicode(dcCode))
				&& x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
				&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
				&& x.CUR_DATE.Equals(DateTime.Today));

					// 1. 產生/更新序號
					if (tmp == null)
					{
						tmp = new F02020103()
						{
							DC_CODE = dcCode,
							CUST_CODE = custCode,
							GUP_CODE = gupCode,
							CUR_DATE = DateTime.Today,
							CUR_VAL = 1,
							CRT_DATE = DateTime.Now,
							CRT_STAFF = string.IsNullOrWhiteSpace(userId) ? Current.Staff : userId
						};
						repo.Add(tmp);
					}
					else
					{
						tmp.CUR_VAL += 1;

						tmp.UPD_DATE = DateTime.Now;
						tmp.UPD_STAFF = Current.Staff;
						repo.Update(tmp);
					}

					// 2. 刪除2天前的舊序號
					repo.DeleteBeforeDate(dcCode, gupCode, custCode, DateTime.Today.AddDays(-2));
					return tmp;
				});

			var rtNo = DateTime.Now.ToString("yyyyMMdd");
			rtNo += f02020103.CUR_VAL.ToString().PadLeft(4, '0');
			return rtNo;
		}
		#endregion

		#region
		public IQueryable<F000904> GetF000904List(string topic, string subtopic)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			return f000904Repo.GetDatas(topic, subtopic);
		}
		#endregion

		/// <summary>
		/// 取得範本檔案
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public ByteData GetSampleExcel(string filePath, string fileName)
		{
			var result = new ByteData();

			//存資料夾底下所有檔案的名稱(含副檔名，不含路徑)
			List<string> dataFileName = new List<string>();
			string serviceDataPath = HttpContext.Current.Server.MapPath(string.Format("./{0}", filePath));

			//判斷資料夾是否存在
			if (Directory.Exists(serviceDataPath))
			{
				//判斷檔案是否存在
				if (Directory.GetFiles(serviceDataPath).Count() != 0)
				{
					foreach (var item in Directory.GetFiles(serviceDataPath))
						dataFileName.Add(Path.GetFileName(item));

					try
					{
						//目前暫時取第一筆寫入
						result.Data = string.Join(",", File.ReadAllBytes(Path.Combine(serviceDataPath, string.IsNullOrWhiteSpace(fileName) ? dataFileName.FirstOrDefault() : fileName)));
						result.FileName = Path.GetFileName(string.IsNullOrWhiteSpace(fileName) ? dataFileName.FirstOrDefault() : fileName);
						result.IsSucess = true;
					}
					catch (Exception)
					{
						result.IsSucess = false;
						result.Message = "系統錯誤";
						return result;
					}
				}
				else
				{
					result.IsSucess = false;
					result.Message = "檔案不存在";
				}
			}
			else
			{
				result.IsSucess = false;
				result.Message = "該路徑資料夾不存在";
			}

			return result;
		}
	}
}

