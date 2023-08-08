using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml;
using System.Configuration;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Shared.Services
{
	/// <summary>
	/// 託運單服務
	/// </summary>
	public partial class ConsignService
	{
		private WmsTransaction _wmsTransaction;
		private List<string> _tempTCATUsedConsignNo { get; set; }
		/// <summary>
		/// 託運單號 Pool，已產生且可用的託運單號 
		/// </summary>
		public List<F050901> F050901Pool { get; set; }

		private List<F050901> _tempF050901List { get; set; }

		public ConsignService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 取得第一箱託運單號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <returns></returns>
		private F050901 GetFirstBoxConsign(string dcCode,string gupCode,string custCode,string wmsNo)
		{
			if (_tempF050901List == null)
				_tempF050901List = new List<F050901>();
			var item = _tempF050901List.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_NO == wmsNo);
			if (item != null)
				return item;
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			item = f050901Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode
																														 && o.GUP_CODE == gupCode
																														 && o.CUST_CODE == custCode
																														 && o.WMS_NO == wmsNo).OrderBy(x=> x.CRT_DATE).FirstOrDefault();
			if (item != null)
				_tempF050901List.Add(item);
			return item;
		}

		/// <summary>
		/// 會取得託運單F050901，但還還要在額外呼叫 Add(F050901)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allId"></param>
		/// <param name="ordAddresses"></param>
		public void CreateConsign(string dcCode, string gupCode, string custCode, string allId, List<OrdAddress> ordAddresses,
			string collectAmt, string ordNo ="")
		{
			// 取得路線編號與名稱與新竹貨運取得到著站
			GetOrdRoutes(dcCode, allId, ordAddresses);

			// 只針對有取得路線編號與名稱的產生託運單
			foreach (var item in ordAddresses.Where(x => x.HasFindZipCodeWithDelvTimes))
			{
				F050901 consign;
				var addBoxGetConsignNo = "0";
				if (string.IsNullOrWhiteSpace(item.ConsignNo))
				{
					var message = string.Empty;
				
					// 沒給託運單號，則由系統產生託運單號
					consign = GetConsignNoByAllId(dcCode, gupCode, custCode, allId, item.DelvTimes, item.WmsNo, collectAmt, ref message,
						ref addBoxGetConsignNo,
						item.PackageBoxNo, ordNo);
					item.ErrorMessage = message;
				}
				else
				{
					// 匯入時有給託運單號的話，使用自訂的託運單號來建立託運單，透過 IMPORT_CUSTOM 註記在 F050901後，編輯相同物流單號時，能繼續沿用匯入的託運單號
					consign = new F050901 {CONSIGN_NO = item.ConsignNo, DELIVID_SEQ_NAME = "IMPORT_CUSTOM"};
				}
				item.AddBoxGetConsignNo = addBoxGetConsignNo;

				item.F050901 = new F050901
				{
					CONSIGN_NO = consign.CONSIGN_NO,
					DELIVID_SEQ_NAME = consign.DELIVID_SEQ_NAME,
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					ERST_NO = item.ErstNo,
					ROUTE_CODE = item.RouteCode,
					ROUTE = item.RouteName,
					WMS_NO = item.WmsNo,
					DELV_TIMES = item.DelvTimes,
					DISTR_USE = item.DistrUse,
					DISTR_SOURCE = item.DistrSource,
					BOXQTY = 1
				};

				// 已經包裝後，託運單重新產生得重新勾稽託運單號
				if (item.OriginalF055001 != null)
					item.OriginalF055001.PAST_NO = consign.CONSIGN_NO;
			}
		}

		/// <summary>
		/// 用於出貨包裝箱子有多箱時，每個箱子都要有一個託運單。會取得託運單F050901，但還還要在額外呼叫 Add(F050901)
		/// 如果F194704.加箱取號方式為1 且非第一箱 請記得要更新F050901
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="allId"></param>
		/// <param name="ordAddress"></param>
		/// <param name="boxSeq"></param>
		/// <param name="f050901"></param>
		public void CreateConsignForF055001(string dcCode, string gupCode, string custCode, string allId,
			OrdAddress ordAddress, int boxSeq,ref string errorMessage , F050901 f050901 = null)
		{
			//當 wmsNo 已經有一筆託運單號 直接用那筆設定取新單號就好
			if (f050901 == null)
			{
				f050901 = GetFirstBoxConsign(dcCode, gupCode, custCode, ordAddress.WmsNo);
			}
			//加箱=>代收金額固定設0 =>讓他取得一般託運單(只有第一箱託運單才使用代收託運單)
			var addNewBoxGetConsignNo = "0";
			var consign = GetConsignNoByAllId(dcCode, gupCode, custCode, allId, f050901.DELV_TIMES, f050901.WMS_NO, "0", ref errorMessage,ref addNewBoxGetConsignNo, boxSeq);
			if(addNewBoxGetConsignNo == "0")
			{
				ordAddress.F050901 = AutoMapper.Mapper.DynamicMap<F050901>(f050901);
				ordAddress.F050901.CONSIGN_NO = consign.CONSIGN_NO;
				ordAddress.F050901.DELIVID_SEQ_NAME = consign.DELIVID_SEQ_NAME;
				ordAddress.F050901.BOXQTY = 1;
			}
			else
			{
				ordAddress.F050901 = consign;
			}
			ordAddress.AddBoxGetConsignNo = addNewBoxGetConsignNo;

		}

		/// <summary>
		/// 取得路線編號與名稱與新竹貨運取得到著站
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="allId"></param>
		/// <param name="ordAddresses"></param>
		public void GetOrdRoutes(string dcCode, string allId, List<OrdAddress> ordAddresses)
		{
			#region 從郵遞區號與配次來取得所有路線編號的可能

			var f194705Rep = new F194705Repository(Schemas.CoreSchema, _wmsTransaction);
			var zipCodes = ordAddresses.Select(a => a.ZipCode).Distinct().ToList();
			var routes = f194705Rep.GetRoutes(zipCodes, allId, dcCode).ToList();

			#endregion

			#region 取得新竹貨運取得到著站

			Dictionary<string, string> addressErstonDict = null;
			if (allId == "HCT")
			{
				var addresses = ordAddresses.Select(x => x.Address).Distinct().ToList();

				// 這裡會呼叫服務，暫時將平行處理拿掉
				addressErstonDict = addresses.Select(address => new {Address = address, ErstNo = HCTCompAddr(address)[1]})
					.ToDictionary(x => x.Address, x => x.ErstNo);
			}

			#endregion

			#region 取宅配通 ZIP + '/' + 宅配通配送區域

			if (allId == "PELICAN")
			{
				var addresses = ordAddresses.Select(x => x.Address).Distinct().ToList();
				// 這裡會呼叫服務，暫時將平行處理拿掉
				addressErstonDict = addresses.Select(
					address => new {Address = address, ErstNo = PELICANCompAddr(address)[0] + "/" + PELICANCompAddr(address)[1]})
					.ToDictionary(x => x.Address, x => x.ErstNo);
			}

			#endregion

			// 主要設定的部分
			foreach (var ordAddress in ordAddresses)
			{
				// 設定該郵遞區號與配次的路線編號與名稱
				var route = routes.FirstOrDefault(a => a.ZipCode == ordAddress.ZipCode && a.DelvTimes == ordAddress.DelvTimes);
				if (route != null)
				{
					ordAddress.RouteCode = route.RouteCode;
					ordAddress.RouteName = route.RouteName;
				}

				// 設定新竹貨運取得到著站 Or 宅配通取得--宅配通 ZIP + '/' + 宅配通配送區域
				if (addressErstonDict != null && addressErstonDict.ContainsKey(ordAddress.Address))
					ordAddress.ErstNo = addressErstonDict[ordAddress.Address];
			}
		}

		/// <summary>
		/// 從派車單號來取得已產生過的託運單Pool
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="distrCarNo"></param>
		public void SetConsignNoPool(string dcCode, string gupCode, string custCode, IEnumerable<string> distrCarNos)
		{
			var f050901Rep = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			if (F050901Pool == null)
				F050901Pool = new List<F050901>();
			F050901Pool.AddRange(f050901Rep.FromF700102GetConsignNos(dcCode, gupCode, custCode, distrCarNos));
		}

		/// <summary>
		/// 取託運單號
		/// </summary>
		/// <param name="allId">配送商代號</param>
		/// <param name="delvTimes">配次</param>		
		/// <param name="wmsNo"></param>
		/// <param name="boxSeq"></param>
		/// <returns></returns>
		public F050901 GetConsignNoByAllId(string dcCode, string gupCode, string custCode, string allId, string delvTimes,
			string wmsNo, string collectAmt, ref string errormessage,ref string addBoxGetConsignNo, int boxSeq = 1,string ordNo="")
		{
			var f194704Repo = new F194704Repository(Schemas.CoreSchema);
			var f194704 = f194704Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALL_ID == allId).FirstOrDefault();
			var getConsignNo = "1"; //預設系統取號
			//取得取號方式
			if (f194704 != null)
			{
				getConsignNo = f194704.GET_CONSIGN_NO;
				if (boxSeq > 1)
					addBoxGetConsignNo = f194704.ADDBOX_GET_CONSIGN_NO;
				else
					addBoxGetConsignNo = "0";
			}
				

			switch(getConsignNo)
			{
				case "1"://系統取號
					#region 大榮貨運
					if (allId == "KTJ")
					{
						var logId = "DELIVID_KTJ";
						// 從已產生過的託運單號尋找
						F050901 poolF050901;
						if (TryFromPoolPopConsignNo(dcCode, gupCode, custCode, wmsNo, logId, out poolF050901))
						{
							return poolF050901;
						}

						//// 從 DB 的 SEQUENCE 產生新的託運單號
						//var f1947Rep = new F1947Repository(Schemas.CoreSchema);
						//var delivId = f1947Rep.GetDelivId(sequenceName);
						var delivId = GetRangeConsignNumber(dcCode, gupCode, custCode, allId, delvTimes, wmsNo, logId);
						return new F050901
						{
							CONSIGN_NO = delivId.NOW_NUMBER,
							DELIVID_SEQ_NAME = logId
						};
					}
					#endregion
					#region 新竹貨運
					if (allId == "HCT")
					{
						//20160722 因平鎮倉開倉,將取號邏輯改由table控管
						//var ordhSequence = GetOrdhSequence(delvTimes, wmsNo);
						//var sequenceName = Enum.GetName(typeof(OrdhSequence), ordhSequence);

						var logId = string.Empty;
						// 無單派車為派車單類型開頭
						if (wmsNo.StartsWith("ZC"))
							logId = "NOHAVEORDELIVID";

						// 一日二配
						if (delvTimes == "12")
							logId = "TODAYDELIVID";
						else
							logId = "TOMORROWDELIVID";

						if(boxSeq > 1 && addBoxGetConsignNo == "1")//加箱取號為原託運單號
						{
							var f050901 = GetFirstBoxConsign(dcCode, gupCode, custCode, wmsNo);
							if(f050901!=null)
							{
								f050901.DELIVID_SEQ_NAME = "MUTILBOXDELIVID";
								f050901.BOXQTY += 1;
							}
							return f050901;
						}
						else
						{
							// 從已產生過的託運單號尋找
							F050901 poolF050901;
							if (TryFromPoolPopConsignNo(dcCode, gupCode, custCode, wmsNo, logId, out poolF050901))
							{
								return poolF050901;
							}

							//// 從 DB 的 SEQUENCE 產生新的託運單號
							//var f1947Rep = new F1947Repository(Schemas.CoreSchema);
							//var delivId = f1947Rep.GetDelivId(sequenceName);
							var delivId = GetRangeConsignNumber(dcCode, gupCode, custCode, allId, delvTimes, wmsNo, logId);
							return new F050901
							{
								CONSIGN_NO = string.Format("{0}{1}", delivId.NOW_NUMBER, (Convert.ToInt64(delivId.NOW_NUMBER) % 7)),
								DELIVID_SEQ_NAME = logId
							};
						}
						
					}
					#endregion

					#region "宅配通"
					if (allId == "PELICAN")
					{
						//var ordhSequence = GetPelicanConsignNumber(delvTimes, wmsNo);
						//var sequenceName = Enum.GetName(typeof(OrdhSequence), ordhSequence);

						// 從已產生過的託運單號尋找
						F050901 poolF050901;
						if (TryFromPoolPopConsignNo(dcCode, gupCode, custCode, wmsNo, "PELICANConsign", out poolF050901))
						{
							return poolF050901;
						}

						// 從 DB 的 SEQUENCE 產生新的託運單號
						//var f1947Rep = new F1947Repository(Schemas.CoreSchema);
						//var delivId = f1947Rep.GetDelivId(sequenceName);
						var f194711 = GetPelicanConsignNumber(dcCode, gupCode, custCode, allId);
						return new F050901
						{
							CONSIGN_NO = string.Format("{0}{1}", f194711.NOW_NUMBER, (Convert.ToInt64(f194711.NOW_NUMBER) % 7).ToString()),
							DELIVID_SEQ_NAME = "PELICANConsign"
						};

					}
					#endregion

					#region 禾頡
					if (allId == "HLSC")
					{
						return new F050901
						{
							CONSIGN_NO = string.Format("{0}{1}", wmsNo, (boxSeq == 1) ? "" : boxSeq.ToString())
						};
					}

					#endregion

					#region 統一速達
					if (allId == "TCAT")
					{
						if (_tempTCATUsedConsignNo == null)
							_tempTCATUsedConsignNo = new List<string>();
						F050901 poolF050901;
						if (TryFromPoolPopConsignNo(dcCode, gupCode, custCode, wmsNo, "TCATConsign", out poolF050901))
						{
							return poolF050901;
						}
						var channel = "00";
						var f050301Repo = new F050301Repository(Schemas.CoreSchema);
						var f050301 = f050301Repo.GetDataByWmsOrdNo(dcCode, gupCode, custCode, wmsNo);
						if (f050301 != null)
							channel = f050301.CHANNEL;
						else
						{
							f050301 = f050301Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo).FirstOrDefault();
							if (f050301 != null)
								channel = f050301.CHANNEL;
							else
							{
								var f050001Repo = new F050001Repository(Schemas.CoreSchema);
								var f050001 = f050001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo).FirstOrDefault();
								if (f050001 != null)
									channel = f050001.CHANNEL;
							}
						}

						//A:一般託運單 B:代收託運單 如果代收金額>0 就代表是代收託運單
						var consignType = "A";
						if (string.IsNullOrWhiteSpace(collectAmt))
						{
							var f050801Repo = new F050801Repository(Schemas.CoreSchema);
							var wmsItem = f050801Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_ORD_NO == wmsNo);
							consignType = wmsItem == null ? "A" : (wmsItem.COLLECT_AMT == 0 ? "A" : "B");
						}
						else
							consignType = decimal.Parse(collectAmt) > 0 ? "B" : "A";
						var f194712Repo = new F194712Repository(Schemas.CoreSchema, _wmsTransaction);
						var f194712 = f194712Repo.Get(dcCode, gupCode, custCode, channel, allId, consignType);
						var f19471201Repo = new F19471201Repository(Schemas.CoreSchema, _wmsTransaction);
						//如果有設定在F194712 CHANNEL=訂單CHANNEL 就取此CHANNEL的託運單(就算沒有託運單號也不抓CHANNEL='00') 如果沒設定才取CHANNEL='00'(不指定)
						var consign = f19471201Repo.AsForUpdate().GetData(dcCode, gupCode, custCode, allId, f194712 == null ? "00" : channel, consignType, "0", _tempTCATUsedConsignNo);

						if (consign != null)
						{
							//紀錄已經被取過的託運單號必免重覆取得
							_tempTCATUsedConsignNo.Add(consign.CONSIGN_NO);
							consign.ISUSED = "1";
							f19471201Repo.Update(consign);
							return new F050901
							{
								CONSIGN_NO = consign.CONSIGN_NO,
								DELIVID_SEQ_NAME = "TCATConsign"
							};
						}
						else
						{
							var f1909Repo = new F1909Repository(Schemas.CoreSchema);
							var f1909 = f1909Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).FirstOrDefault();
							if (f194712 == null)
								f194712 = f194712Repo.Get(dcCode, gupCode, custCode, "00", allId, consignType);
							var errorList = errormessage.Split(Environment.NewLine.ToArray()).ToList();
							var message = string.Format("貨主:{0} 客代:{1} 託運單類別:{2} 數量不足，請洽系統管理員", f1909?.SHORT_NAME, f194712?.CUSTOMER_ID, consignType == "A" ? "一般託運單" : "代收託運單");
							if (!errorList.Any(x => x == message))
								errorList.Add(message);
							errormessage = string.Join(Environment.NewLine, errorList);
						}

					}
					#endregion

					break;
				case "2": //虛擬取號-出貨單號+XXX(3碼)當成託運單號
					return new F050901
					{
						CONSIGN_NO = string.Format("{0}{1}", wmsNo.Substring(wmsNo.Length - 13, 13), boxSeq.ToString().PadLeft(3, '0')),
						DELIVID_SEQ_NAME = "VirtualConsign"
					};
				case "3": //外部取號
					var f050304Repo = new F050304Repository(Schemas.CoreSchema);
					var f050304 = f050304Repo.GetDataByWmsNo(dcCode, gupCode, custCode, wmsNo);
					if (f050304 == null || string.IsNullOrEmpty(f050304.CONSIGN_NO))
					{
						var errorList = errormessage.Split(Environment.NewLine.ToArray()).ToList();
						var message = string.Format("尚未提供外部託運單號");
						if (!errorList.Any(x => x == message))
							errorList.Add(message);
						errormessage = string.Join(Environment.NewLine, errorList);
					}
					else
					{
						return new F050901
						{
							CONSIGN_NO = f050304.CONSIGN_NO,
							DELIVID_SEQ_NAME = allId + "Consign"
						};
					}
					break;
			}
			

			// 尚未支援其它配送商
			return new F050901();
		}

		public OrdhSequence GetOrdhSequence(string delvTimes, string wmsNo)
		{
			// 無單派車為派車單類型開頭
			if (wmsNo.StartsWith("ZC"))
				return OrdhSequence.SEQ_ORDH_NOHAVEORDELIVID;

			// 一日二配
			if (delvTimes == "12")
				return OrdhSequence.SEQ_ORDH_TODAYDELIVID;

			// 非一日二配
			return OrdhSequence.SEQ_ORDH_TOMORROWDELIVID;
		}

		//public OrdhSequence GetOrdhSequence_PELICAN(string delvTimes, string wmsNo)
		//{
		//    // 宅配通
		//    return OrdhSequence.SEQ_ORDH_PELICAN;
		//}

		public F194711 GetPelicanConsignNumber(string dcCode, string gupCode, string custCode, string allId)
		{
			var f194711Rep = new F194711Repository(Schemas.CoreSchema);
			var f194711 =
				f194711Rep.Find(
					item => item.DC_CODE == dcCode && item.GUP_CODE == gupCode && item.CUST_CODE == custCode && item.ALL_ID == allId);
			if (f194711 != null)
			{
				if (Convert.ToInt64(f194711.NOW_NUMBER) < Convert.ToInt64(f194711.END_NUMBER))
				{
					f194711.NOW_NUMBER = (Convert.ToInt64(f194711.NOW_NUMBER) + 1).ToString();
					f194711Rep.Update(f194711);
				}
				else
				{
					return null;
				}

			}
			return f194711;
		}

		public F194710 GetRangeConsignNumber(string dcCode, string gupCode, string custCode, string allId, string delvTimes, string wmsNo, string logId)
		{
			var f194710Rep = new F194710Repository(Schemas.CoreSchema);
#if DEBUG
			var isTest = "1";
#else
			var isTest = "0";
#endif
            //依照物流中心、業主、貨主搜尋新竹貨運指定客代及區間，換言之，若客代是屬於某一個貨主時，業主與貨主要完整定義
			var f194710 = f194710Rep.Find(item => item.DC_CODE == dcCode && item.GUP_CODE == gupCode && item.CUST_CODE == custCode && item.ALL_ID == allId && item.LOG_ID == logId && item.ISTEST == isTest);
            if (f194710 == null)
            {
                //依照物流中心 搜尋新竹貨運指定客代及區間，換言之，若客代是屬於物流中心時，業主與貨主設定為00即可
                f194710 = f194710Rep.Find(item => item.DC_CODE == dcCode && item.GUP_CODE == "00" && item.CUST_CODE == "00" && item.ALL_ID == allId && item.LOG_ID == logId && item.ISTEST == isTest);
            }

            if (f194710 != null)
			{
				if (Convert.ToInt64(f194710.NOW_NUMBER) <= Convert.ToInt64(f194710.END_NUMBER))
					f194710.NOW_NUMBER = (Convert.ToInt64(f194710.NOW_NUMBER) + 1).ToString();
				else
					f194710.NOW_NUMBER = Convert.ToInt64(f194710.START_NUMBER).ToString();

				f194710Rep.Update(f194710);

			}
			return f194710;
		}

		/// <summary>
		/// 嘗試從託運單Pool尋找已產生過的託運單號，若有找到的話，則會回傳，並從 Pool 移除。
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo">尋找匯入的託運單號用</param>
		/// <param name="sequenceName"></param>
		/// <param name="poolConsignNo"></param>
		/// <returns></returns>
		public bool TryFromPoolPopConsignNo(string dcCode, string gupCode, string custCode, string wmsNo, string sequenceName,
			out F050901 poolF050901)
		{
			poolF050901 = null;
			if (F050901Pool == null)
				return false;

			// 先從是否有手動匯入的託運單找起，分辨出手動匯入的託運單號對應哪個物流單號，編輯後才能為相同託運單號
			poolF050901 = F050901Pool.Where(x => x.DC_CODE == dcCode
			                                     && x.GUP_CODE == gupCode
			                                     && x.CUST_CODE == custCode
			                                     && x.WMS_NO == wmsNo
			                                     && x.DELIVID_SEQ_NAME == "IMPORT_CUSTOM").FirstOrDefault();


			if (poolF050901 == null)
			{
				// 尋找在該區間的託運單號。因編輯後都要重新產生託運單F591，為了避免重複產生 CONSIGN_NO，故從舊有可用的 CONSIGN_NO 找起
				poolF050901 = F050901Pool.Where(x => x.DC_CODE == dcCode
				                                     && x.GUP_CODE == gupCode
				                                     && x.CUST_CODE == custCode
				                                     && x.DELIVID_SEQ_NAME == sequenceName).FirstOrDefault();
			}

			if (poolF050901 != null)
			{
				F050901Pool.Remove(poolF050901);
			}

			return poolF050901 != null;
		}

		/// <summary>
		/// 取得新竹貨運到著站代碼
		/// </summary>
		/// <param name="addr">地址</param>
		/// <returns>到著站相關資訊 Array</returns>    
		public static string[] HCTCompAddr(string addr)
		{
			Encoding myEncoding = Encoding.GetEncoding("big5");
			WebClient client = new WebClient();
			addr = "http://webservices.hct.com.tw/addr/ADDR_COMPARE.aspx?addr=" + HttpUtility.UrlEncode(addr, myEncoding);

			//byte[] bResult = client.DownloadData(addr);
			byte[] bResult;
			string result = "";
			string erstno_4 = "";
			string erstno = "";
			string post = "";
			string erstno_name = "";
			string zone = "";
			try
			{
				bResult = client.DownloadData(addr);
				result = Encoding.GetEncoding(950).GetString(bResult);
				erstno_4 = result.Substring(result.IndexOf("到著站四碼：") + 6, 4);
				erstno = result.Substring(result.IndexOf("到著站簡碼：") + 6, 3);
				post = result.Substring(result.IndexOf("郵遞區號：") + 5, 3);
				erstno_name = result.Substring(result.IndexOf("到著站中文：") + 6, 2);
				zone = result.Substring(result.IndexOf("配區：") + 3, 3);

				if (erstno_4 == "<BR>")
				{
					erstno_4 = "";
				}
				if (erstno == "<BR")
				{
					erstno = "";
				}
				if (post == "<BR")
				{
					post = "";
				}
				if (erstno_name == "<B")
				{
					erstno_name = "";
				}
				if (zone == "<BR")
				{
					zone = "";
				}

			}
			catch (Exception ex)
			{
				ExceptionPolicy.HandleException(ex, "Default Policy");
				//竹運取道著站出錯，就回傳空白，不影響系統運作
			}

			string[] strData = new string[5];
			strData[0] = erstno_4;
			strData[1] = erstno;
			strData[2] = post;
			strData[3] = erstno_name;
			strData[4] = zone;
			return strData;

		}






		/// <summary>
		/// 取宅配通 ZIP + '/' + 宅配通配送區域
		/// </summary>
		/// <param name="addr">地址</param>
		/// <returns>到著站相關資訊 Array</returns>    
		/// 
		public static string[] PELICANCompAddr(string addr)
		{
			string zip = "";
			string area = "";

			try
			{
				WebClient wc = new WebClient();
				wc.Encoding = Encoding.UTF8;
				addr = "http://query2.e-can.com.tw:8080/Datasnap/Rest/TServerMT/LookupZip/=" + addr;
				/*載入JSON字串*/
				string jsonStr = wc.DownloadString(addr);

				Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
				object value = values["result"];
				JArray ja = (JArray) JsonConvert.DeserializeObject(value.ToString());

				zip = ja[0]["PZip5"].ToString() == "" ? " " : ja[0]["PZip5"].ToString();
				area = ja[0]["Area"].ToString() == "" ? " " : ja[0]["Area"].ToString();
			}
			catch (Exception ex)
			{
				ExceptionPolicy.HandleException(ex, "Default Policy");
				//取道zip & area出錯，就回傳空白，不影響系統運作
			}

			string[] strData = new string[2];
			strData[0] = zip;
			strData[1] = area;
			return strData;

		}

		public List<F050901> GetUpdateF050901DataForLogId(string customerId, string logId, List<string> consignNos)
		{
			var f050901repos = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			return f050901repos.GetUpDataForLogId(customerId, logId, consignNos);
		}

		public ExecuteResult UpdateStatusForSOD(List<F050901> datas)
		{
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var d3 = datas.Where(o => o.STATUS == "3").ToList();
			if (d3 != null && d3.Any())
				f050901Repo.BulkUpdateDistrEdiStatusSod(d3, "3");
			var d = datas.Where(o => o.STATUS != "3" && o.STATUS != "4").ToList();
			if (d != null && d.Any())
				f050901Repo.BulkUpdateDistrEdiStatusSod(d, "2");
			var d4 = datas.Where(o => o.STATUS == "4").ToList();
			if (d4 != null && d4.Any())
				f050901Repo.BulkUpdateDistrEdiStatusSod(d4, "4");
			return new ExecuteResult { IsSuccessed = true };
		}
	}
}
