
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;
using System.Data.Objects;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P91.Services
{
	public partial class P910302Service
	{
		private WmsTransaction _wmsTransaction;
		public P910302Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 以報價單主檔取得除了 F910403 欄位外，也帶出商品名稱跟大分類名稱
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		public IQueryable<F910403Data> GetF910403DataByQuoteNo(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var f910403Repo = new F910403Repository(Schemas.CoreSchema, _wmsTransaction);
			return f910403Repo.GetF910403DataByQuoteNo(dcCode, gupCode, custCode, quoteNo);
		}

		/// <summary>
		/// 以商品編號取得商品名稱跟大分類名稱
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="custCode"></param>
		/// /// <returns></returns>
		public IQueryable<F1903WithF1915> GetF1903WithF1915(string gupCode, string itemCode,string custCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			return f1903Repo.GetF1903WithF1915(gupCode, itemCode,custCode);
		} 

		/// <summary>
		/// 取得包含所有動作的附約資料，主要要取得委外商與動作金額。
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="processIds"></param>
		/// <returns></returns>
		public IQueryable<F910302WithF1928> GetF910302ByProcessIds(string dcCode, string gupCode, DateTime enableDate, DateTime disableDate, IEnumerable<string> processIds)
		{
			enableDate = enableDate.Date;
			disableDate = disableDate.Date;
			// 生效日期不能大於失效日期
			if (enableDate > disableDate)
			{
				return new List<F910302WithF1928>().AsQueryable();
			}

			var f910302Repo = new F910302Repository(Schemas.CoreSchema, _wmsTransaction);
			return f910302Repo.GetF910302ByProcessIds(dcCode, gupCode, enableDate, disableDate, processIds);
		}

		/// <summary>
		/// 建立報價單
		/// </summary>
		/// <param name="f910401"></param>
		/// <param name="f910402s"></param>
		/// <param name="f910403s"></param>
		/// <returns></returns>
		public Wms3pl.Datas.Shared.Entities.ExecuteResult InsertP910302(F910401 f910401, IEnumerable<F910402> f910402s, IEnumerable<F910403> f910403s)
		{
			// 0. 檢核資料
			if (f910401 == null || string.IsNullOrEmpty(f910401.DC_CODE) || string.IsNullOrEmpty(f910401.GUP_CODE) || string.IsNullOrEmpty(f910401.CUST_CODE)
				|| f910402s == null || !f910402s.Any() || f910402s.Where(item => string.IsNullOrEmpty(item.PROCESS_ID)).Any()) // || f910403s == null || !f910403s.Any() || f910403s.Where(item => string.IsNullOrEmpty(item.ITEM_CODE)).Any()
			{
				return new ExecuteResult() { Message = Properties.Resources.P910302Service_PARAM_DATA_ERROR };
			}

			var repo910401 = new F910401Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo910402 = new F910402Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo910403 = new F910403Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);

			f910401.QUOTE_NO = sharedService.GetNewOrdCode("Q");
			f910401.STATUS = "0";	// 待處理
			repo910401.Add(f910401);

			foreach (var item in f910402s)
			{
				item.DC_CODE = f910401.DC_CODE;
				item.GUP_CODE = f910401.GUP_CODE;
				item.CUST_CODE = f910401.CUST_CODE;
				item.QUOTE_NO = f910401.QUOTE_NO;
				repo910402.Add(item);
			}

			foreach (var item in f910403s)
			{
				item.DC_CODE = f910401.DC_CODE;
				item.GUP_CODE = f910401.GUP_CODE;
				item.CUST_CODE = f910401.CUST_CODE;
				item.QUOTE_NO = f910401.QUOTE_NO;
				repo910403.Add(item);
			}

			return new ExecuteResult()
			{
				IsSuccessed = true,
				Message = Properties.Resources.P910302Service_ADD_SUCCESS
			};
		}

		/// <summary>
		/// 更新報價單
		/// </summary>
		/// <param name="f910401"></param>
		/// <param name="f910402s"></param>
		/// <param name="f910403s"></param>
		/// <param name="isApproved">是否核准</param>
		/// <returns></returns>
		public Wms3pl.Datas.Shared.Entities.ExecuteResult UpdateP910302(F910401 f910401, IEnumerable<F910402> f910402s, IEnumerable<F910403> f910403s, bool isApproved)
		{
			// 0. 檢核資料
			if (f910401 == null || string.IsNullOrEmpty(f910401.DC_CODE) || string.IsNullOrEmpty(f910401.GUP_CODE) || string.IsNullOrEmpty(f910401.CUST_CODE)
				|| f910402s == null || !f910402s.Any() || f910402s.Where(item => string.IsNullOrEmpty(item.PROCESS_ID)).Any())	//
			{
				return new ExecuteResult() { Message = Properties.Resources.P910302Service_PARAM_DATA_ERROR };
			}

			if (isApproved)
			{
				if ((f910401.STATUS != "0" || f910401.APPROVED_PRICE.HasValue == false || f910401.APPROVED_PRICE.Value < 0))
				{
					return new ExecuteResult() { Message = Properties.Resources.P910302Service_APPROVED_PRICE_NEEDVALUE };
				}
			}

			var repo910401 = new F910401Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo910402 = new F910402Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo910403 = new F910403Repository(Schemas.CoreSchema, _wmsTransaction);

			// 1. 更新估價單主檔
			var f910401Entity = repo910401.Find(item => item.DC_CODE == f910401.DC_CODE && item.GUP_CODE == f910401.GUP_CODE && item.CUST_CODE == f910401.CUST_CODE && item.QUOTE_NO == f910401.QUOTE_NO);
			f910401Entity.QUOTE_NAME = f910401.QUOTE_NAME;
			f910401Entity.NET_RATE = f910401.NET_RATE;
			f910401Entity.COST_PRICE = f910401.COST_PRICE;
			f910401Entity.ENABLE_DATE = f910401.ENABLE_DATE;
			f910401Entity.DISABLE_DATE = f910401.DISABLE_DATE;
			f910401Entity.APPLY_PRICE = f910401.APPLY_PRICE;
			f910401Entity.APPROVED_PRICE = f910401.APPROVED_PRICE;
			f910401Entity.OUTSOURCE_ID = f910401.OUTSOURCE_ID;

			if (isApproved)
			{
				f910401Entity.STATUS = "1";	// 已核准
			}

			repo910401.Update(f910401Entity);

			// 2. 更新動作與耗材明細
			// 2.1 先刪除不存在於編輯後的項目
			repo910402.DeleteNotInProcessIds(f910401.DC_CODE, f910401.GUP_CODE, f910401.CUST_CODE, f910401.QUOTE_NO, f910402s.Select(item => item.PROCESS_ID));
			if (f910403s.Any())
			{
				repo910403.DeleteNotItemCodeList(f910401.DC_CODE, f910401.GUP_CODE, f910401.CUST_CODE, f910401.QUOTE_NO, f910403s.Select(item => item.ITEM_CODE));
			}

			// 2.2 將已存在的更新，不存在的新增
			foreach (var item in f910402s)
			{
				item.DC_CODE = f910401.DC_CODE;
				item.GUP_CODE = f910401.GUP_CODE;
				item.CUST_CODE = f910401.CUST_CODE;
				item.QUOTE_NO = f910401.QUOTE_NO;

				var f910402Entity = repo910402.Find(s => s.DC_CODE == item.DC_CODE
												&& s.GUP_CODE == item.GUP_CODE
												&& s.CUST_CODE == item.CUST_CODE
												&& s.QUOTE_NO == item.QUOTE_NO
												&& s.PROCESS_ID == item.PROCESS_ID);
				if (f910402Entity == null)
				{
					repo910402.Add(item);
				}
				else
				{
					f910402Entity.UNIT_ID = item.UNIT_ID;
					f910402Entity.WORK_COST = item.WORK_COST;
					f910402Entity.WORK_HOUR = item.WORK_HOUR;

					repo910402.Update(f910402Entity);
				}
			}

			foreach (var item in f910403s)
			{
				item.DC_CODE = f910401.DC_CODE;
				item.GUP_CODE = f910401.GUP_CODE;
				item.CUST_CODE = f910401.CUST_CODE;
				item.QUOTE_NO = f910401.QUOTE_NO;

				var f910403Entity = repo910403.Find(s => s.DC_CODE == item.DC_CODE
												&& s.GUP_CODE == item.GUP_CODE
												&& s.CUST_CODE == item.CUST_CODE
												&& s.QUOTE_NO == item.QUOTE_NO
												&& s.ITEM_CODE == item.ITEM_CODE);
				if (f910403Entity == null)
				{
					repo910403.Add(item);
				}
				else
				{
					f910403Entity.STANDARD = item.STANDARD;
					f910403Entity.STANDARD_COST = item.STANDARD_COST;
					f910403Entity.UNIT_ID = item.UNIT_ID;

					repo910403.Update(f910403Entity);
				}
			}

			return new ExecuteResult()
			{
				IsSuccessed = true,
				Message = Properties.Resources.P910302Service_EDIT_SUCCESS
			};
		}

		public ExecuteResult InsertContractData(F910301Data contractMain, List<F910302Data> contractItems)
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var repo910301 = new F910301Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo910302 = new F910302Repository(Schemas.CoreSchema, _wmsTransaction);

			// 新增主檔
			var f910301 = repo910301.Find(x => x.DC_CODE == EntityFunctions.AsNonUnicode(contractMain.DC_CODE) &&
												x.GUP_CODE == EntityFunctions.AsNonUnicode(contractMain.GUP_CODE) &&
												x.CONTRACT_NO == EntityFunctions.AsNonUnicode(contractMain.CONTRACT_NO));
			if (f910301 != null)
			{
				result.Message = Properties.Resources.P910302Service_DATA_CONTRACTNO_EXIST;
				return result;
			}

			var errorMsg = ValidateF910301(repo910301, contractMain);
			if (!string.IsNullOrEmpty(errorMsg))
			{
				return new ExecuteResult(false, errorMsg);
			}

			var newF910301 = new F910301()
			{
				CONTRACT_NO = contractMain.CONTRACT_NO,
				ENABLE_DATE = contractMain.ENABLE_DATE,
				DISABLE_DATE = contractMain.DISABLE_DATE,
				OBJECT_TYPE = contractMain.OBJECT_TYPE,
				UNI_FORM = contractMain.UNI_FORM,
				STATUS = contractMain.STATUS,
				MEMO = contractMain.MEMO,
				DC_CODE = contractMain.DC_CODE,
				GUP_CODE = contractMain.GUP_CODE,
				CLOSE_CYCLE = contractMain.CLOSE_CYCLE,
				CYCLE_DATE = contractMain.CYCLE_DATE
			};

			repo910301.Add(newF910301);

			//新增副檔
			var f910302s = repo910302.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(contractMain.DC_CODE) &&
																			 x.GUP_CODE == EntityFunctions.AsNonUnicode(contractMain.GUP_CODE) &&
																			 x.CONTRACT_NO == EntityFunctions.AsNonUnicode(contractMain.CONTRACT_NO)).ToList();

			if (f910302s != null && f910302s.Any())
			{
				result.Message = Properties.Resources.P910302Service_DATA_CONTRACTOBJECT_EXIST;
				return result;
			}

			var newF910302s = contractItems.Select(x => new F910302()
														{
															CONTRACT_NO = contractMain.CONTRACT_NO,
															CONTRACT_SEQ = Convert.ToInt16(x.CONTRACT_SEQ),
															SUB_CONTRACT_NO = x.SUB_CONTRACT_NO,
															CONTRACT_TYPE = x.CONTRACT_TYPE,
															ENABLE_DATE = x.ENABLE_DATE,
															DISABLE_DATE = x.DISABLE_DATE,
															ITEM_TYPE = x.ITEM_TYPE,
															QUOTE_NO = x.QUOTE_NO,
															UNIT_ID = x.UNIT_ID,
															TASK_PRICE = Convert.ToDecimal(x.TASK_PRICE),
															WORK_HOUR = x.WORK_HOUR,
															PROCESS_ID = x.PROCESS_ID,
															OUTSOURCE_COST = Convert.ToDecimal(x.OUTSOURCE_COST),
															APPROVE_PRICE = Convert.ToDecimal(x.APPROVE_PRICE),
															DC_CODE = contractMain.DC_CODE,
															GUP_CODE = contractMain.GUP_CODE,
															CONTRACT_FEE = x.CONTRACT_FEE
														}).ToList();
			foreach (var newF910302 in newF910302s)
				repo910302.Add(newF910302);

			result.IsSuccessed = true;
			result.Message = Properties.Resources.P192018Service_Update;
			return result;
		}

		public ExecuteResult UpdateContractData(F910301Data contractMain, List<F910302Data> contractItems, List<int> delContractIds)
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var repo910301 = new F910301Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo910302 = new F910302Repository(Schemas.CoreSchema, _wmsTransaction);

			// 更新主檔
			var f910301 = repo910301.Find(x => x.DC_CODE == contractMain.DC_CODE &&
												x.GUP_CODE == contractMain.GUP_CODE &&
												x.CONTRACT_NO == contractMain.CONTRACT_NO);
			if (f910301 == null)
			{
				result.Message = Properties.Resources.P910301Service_DataContractNotExists;
				return result;
			}

			var errorMsg = ValidateF910301(repo910301, contractMain);
			if (!string.IsNullOrEmpty(errorMsg))
			{
				return new ExecuteResult(false, errorMsg);
			}

			f910301.ENABLE_DATE = contractMain.ENABLE_DATE;
			f910301.DISABLE_DATE = contractMain.DISABLE_DATE;
			f910301.OBJECT_TYPE = contractMain.OBJECT_TYPE;
			f910301.UNI_FORM = contractMain.UNI_FORM;
			f910301.STATUS = contractMain.STATUS;
			f910301.MEMO = contractMain.MEMO;
			f910301.CLOSE_CYCLE = contractMain.CLOSE_CYCLE;
			f910301.CYCLE_DATE = contractMain.CYCLE_DATE;

			repo910301.Update(f910301);

			//更新副檔
			var f910302s = repo910302.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(contractMain.DC_CODE) &&
													x.GUP_CODE == EntityFunctions.AsNonUnicode(contractMain.GUP_CODE) &&
													x.CONTRACT_NO == EntityFunctions.AsNonUnicode(contractMain.CONTRACT_NO))
									 .ToList();

			//刪除被清除項目
			foreach (var seq in delContractIds)
			{
				repo910302.Delete(x => x.DC_CODE == EntityFunctions.AsNonUnicode(contractMain.DC_CODE) &&
										x.GUP_CODE == EntityFunctions.AsNonUnicode(contractMain.GUP_CODE) &&
										x.CONTRACT_NO == EntityFunctions.AsNonUnicode(contractMain.CONTRACT_NO) &&
										x.CONTRACT_SEQ == seq);
			}


			//更新或新增
			var newF910302s = contractItems.Select(x => new F910302()
														{
															CONTRACT_NO = x.CONTRACT_NO,
															CONTRACT_SEQ = Convert.ToInt16(x.CONTRACT_SEQ),
															SUB_CONTRACT_NO = x.SUB_CONTRACT_NO,
															CONTRACT_TYPE = x.CONTRACT_TYPE,
															ENABLE_DATE = x.ENABLE_DATE,
															DISABLE_DATE = x.DISABLE_DATE,
															ITEM_TYPE = x.ITEM_TYPE,
															QUOTE_NO = x.QUOTE_NO,
															UNIT_ID = x.UNIT_ID,
															TASK_PRICE = Convert.ToDecimal(x.TASK_PRICE),
															WORK_HOUR = x.WORK_HOUR,
															PROCESS_ID = x.PROCESS_ID,
															OUTSOURCE_COST = Convert.ToDecimal(x.OUTSOURCE_COST),
															APPROVE_PRICE = Convert.ToDecimal(x.APPROVE_PRICE),
															DC_CODE = contractMain.DC_CODE,
															GUP_CODE = contractMain.GUP_CODE,
															CONTRACT_FEE = x.CONTRACT_FEE
														}).ToList();

			foreach (var item in newF910302s)
			{
				var f910302 = f910302s.FirstOrDefault(x => x.CONTRACT_SEQ == item.CONTRACT_SEQ);
				if (f910302 == null)
					repo910302.Add(item);
				else
				{
					f910302.SUB_CONTRACT_NO = item.SUB_CONTRACT_NO;
					f910302.CONTRACT_TYPE = item.CONTRACT_TYPE;
					f910302.ENABLE_DATE = item.ENABLE_DATE;
					f910302.DISABLE_DATE = item.DISABLE_DATE;
					f910302.ITEM_TYPE = item.ITEM_TYPE;
					f910302.QUOTE_NO = item.QUOTE_NO;
					f910302.UNIT_ID = item.UNIT_ID;
					f910302.TASK_PRICE = item.TASK_PRICE;
					f910302.WORK_HOUR = item.WORK_HOUR;
					f910302.PROCESS_ID = item.PROCESS_ID;
					f910302.OUTSOURCE_COST = item.OUTSOURCE_COST;
					f910302.APPROVE_PRICE = item.APPROVE_PRICE;
					f910302.CONTRACT_FEE = item.CONTRACT_FEE;
					repo910302.Update(f910302);
				}
			}

			result.IsSuccessed = true;
			result.Message = Properties.Resources.P192018Service_Update;
			return result;
		}

		string ValidateF910301(F910301Repository repo910301, F910301Data contractMain)
		{
			// ※一個DC+貨主+生效區間只能有一份合約
			if (repo910301.Filter(x => x.OBJECT_TYPE == EntityFunctions.AsNonUnicode(contractMain.OBJECT_TYPE)
										&& x.STATUS != EntityFunctions.AsNonUnicode("9")
										&& x.DC_CODE == EntityFunctions.AsNonUnicode(contractMain.DC_CODE)
										&& x.UNI_FORM == EntityFunctions.AsNonUnicode(contractMain.UNI_FORM)
										&& x.CONTRACT_NO != EntityFunctions.AsNonUnicode(contractMain.CONTRACT_NO)
										&& ((x.ENABLE_DATE <= contractMain.ENABLE_DATE && contractMain.ENABLE_DATE <= x.DISABLE_DATE)
											|| (x.ENABLE_DATE <= contractMain.DISABLE_DATE && contractMain.DISABLE_DATE <= x.DISABLE_DATE)
											|| (contractMain.ENABLE_DATE <= x.ENABLE_DATE && x.DISABLE_DATE <= contractMain.DISABLE_DATE)))
							  .Any())
			{
				if (contractMain.OBJECT_TYPE == "0")
					return Properties.Resources.P910302Service_ContractMain_ONLYONE_CUST;
				else
					return Properties.Resources.P910302Service_ContractMain_ONLYONE_SUB;
			}

			return string.Empty;
		}

		/// <summary>
		/// 取得報價單報表主檔
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		public IQueryable<F910401Report> GetF910401Report(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var repo = new F910401Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetF910401Report(dcCode, gupCode, custCode, quoteNo);
		}

		/// <summary>
		/// 取得報價單動作分析明細報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		public IQueryable<F910402Report> GetF910402Reports(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var repo = new F910402Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetF910402Reports(dcCode, gupCode, custCode, quoteNo);
		}

		/// <summary>
		/// 取得報價單耗材項目報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		public IQueryable<F910403Report> GetF910403Reports(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var repo = new F910403Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetF910403Reports(dcCode, gupCode, custCode, quoteNo);
		}


		public ExecuteResult UploadFileF910404(F910404 f910404)
		{
			var sharedService = new SharedService();
			var repF910404 = new F910404Repository(Schemas.CoreSchema);

			var f910404Item = repF910404.Find(o => o.QUOTE_NO == f910404.QUOTE_NO && o.DC_CODE == f910404.DC_CODE && o.GUP_CODE == f910404.GUP_CODE && o.CUST_CODE == f910404.CUST_CODE);
			if (f910404Item == null)
				repF910404.Add(f910404);
			return new ExecuteResult(true, f910404.QUOTE_NO);
		}
	}
}