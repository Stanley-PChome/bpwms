using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;


namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P197001Service
	{
		private WmsTransaction _wmsTransaction;
		public P197001Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF197001(List<F197001> f197001Data)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f197001Repo = new F197001Repository(Schemas.CoreSchema, _wmsTransaction);
			var gupCode = f197001Data.Select(x => x.GUP_CODE).FirstOrDefault();
			var custCode = f197001Data.Select(x => x.CUST_CODE).FirstOrDefault();

			var seq = GetF197001Seq(gupCode, custCode);
			foreach (var items in f197001Data)
			{
				//檢查是否重複新增 條件 LabelCode / ItemCode / VNRCode
				var resultData = f197001Repo.GetLabelData(items);

				if (resultData.Count() > 0)
				{
					result.IsSuccessed = false;
					result.Message = Properties.Resources.P197001Service_Label_Duplicate;
				}
				else
				{
					var f197001 = new F197001();
					f197001.LABEL_SEQ = seq;
					f197001.LABEL_CODE = items.LABEL_CODE;
					f197001.ITEM_CODE = items.ITEM_CODE;
					f197001.VNR_CODE = items.VNR_CODE;
					f197001.WARRANTY = items.WARRANTY;
					f197001.WARRANTY_S_Y = items.WARRANTY_S_Y;
					f197001.WARRANTY_S_M = items.WARRANTY_S_M;
					f197001.WARRANTY_Y = items.WARRANTY_Y;
					f197001.WARRANTY_M = items.WARRANTY_M;
					f197001.WARRANTY_D = items.WARRANTY_D;
					f197001.OUTSOURCE = items.OUTSOURCE;
					f197001.CHECK_STAFF = items.CHECK_STAFF;
					f197001.ITEM_DESC_A = items.ITEM_DESC_A;
					f197001.ITEM_DESC_B = items.ITEM_DESC_B;
					f197001.ITEM_DESC_C = items.ITEM_DESC_C;
					f197001.CUST_CODE = items.CUST_CODE;
					f197001.GUP_CODE = items.GUP_CODE;
					f197001Repo.Add(f197001);
					seq += 1;
				}
			}

			return result;
		}

		public int GetF197001Seq(string gupCode, string custCode)
		{
			var f197001Repo = new F197001Repository(Schemas.CoreSchema, _wmsTransaction);
			var resultRepo = f197001Repo.GetF197001Seq(gupCode, custCode);
			var resultData = resultRepo.FirstOrDefault();
			return resultData == null ? 1 : resultData.LABEL_SEQ + 1;

		}

		public IQueryable<F197001Data> GetF197001Data(string gupCode, string custCode, string labelCode, string itemCode, string vnrCode)
		{
			var f197001Repo = new F197001Repository(Schemas.CoreSchema, _wmsTransaction);
			return f197001Repo.GetF197001Data(gupCode, custCode, labelCode, itemCode, vnrCode);

		}

		public ExecuteResult UpdateF197001(F197001 f197001Data)
		{
			var f197001Repo = new F197001Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = f197001Repo.UpdateF197001(f197001Data);
			return result;
		}


		public ExecuteResult DelF197001s(F197001Data[] f197001Datas)
		{
			var f197001Repo = new F197001Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f197001Data in f197001Datas)
			{
				var result = f197001Repo.DelF197001(f197001Data);
				if (!result.IsSuccessed)
					return result;
			}

			return new ExecuteResult(true);
		}


		/// <summary>
		/// 解析匯入的標籤內容
		/// </summary>
		/// <param name="f1970"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public IQueryable<F197001Data> ParseImportF197001Data(F1970 f1970, List<F197001Data> list)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);

			var warrantys = f000904Repo.GetF000904Data("F1970", "WARRANTY").ToList();
			var f1908s = f1908Repo.InWithTrueAndCondition("VNR_CODE", list.Select(x => x.VNR_CODE).Distinct().ToList(), x => x.GUP_CODE == f1970.GUP_CODE).ToList();
			var f1903s = f1903Repo.InWithTrueAndCondition("ITEM_CODE", list.Select(x => x.ITEM_CODE).Distinct().ToList(), x => x.GUP_CODE == f1970.GUP_CODE && x.CUST_CODE == f1970.CUST_CODE).ToList();

			list.ForEach(x =>
			{
				x.WARRANTY_Y = ConvertToWarrantyYear(x.WarrantyDate);
				x.WARRANTY_M = ConvertToWarrantyMonth(x.WarrantyDate);
				x.WARRANTY_D = Convert.ToInt16(x.WarrantyDate.Day);
				x.WARRANTY_S_Y = Convert.ToInt16(x.WarrantyDate.Year);
				x.WARRANTY_S_M = Convert.ToString(x.WarrantyDate.Month);
				// 匯入時，是中文，這邊解析為系統字元
				x.WARRANTY = warrantys.Where(w => w.NAME == x.WARRANTY).Select(w => w.VALUE).FirstOrDefault();

				var f1908 = f1908s.Find(item => item.GUP_CODE == x.GUP_CODE && item.VNR_CODE == x.VNR_CODE);
				x.VNR_NAME = (f1908 != null) ? f1908.VNR_NAME : string.Empty;

				var f1903 = f1903s.Find(item => item.GUP_CODE == x.GUP_CODE && item.ITEM_CODE == x.ITEM_CODE && item.CUST_CODE == x.CUST_CODE);
				if (f1903 != null)
				{
					x.ITEM_COLOR = f1903.ITEM_COLOR;
					x.ITEM_NAME = f1903.ITEM_NAME;
					x.ITEM_SIZE = f1903.ITEM_SIZE;
					x.ITEM_SPEC = f1903.ITEM_SPEC;
				}
			});

			return list.AsQueryable();
		}

		#region 保固年、月公式
		string ConvertToWarrantyYear(DateTime date)
		{
			var year = Convert.ToChar(((date.Year % 100) % 26) + 64).ToString();
			return year;
		}

		string ConvertToWarrantyMonth(DateTime date)
		{
			var month = Convert.ToChar(DateTime.Today.Month + 64).ToString();
			return month;
		}
		#endregion

		/// <summary>
		/// 驗證匯入的標籤設定內容
		/// </summary>
		/// <param name="f1970"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public ExecuteResult ValidateImportF197001Data(F1970 f1970, List<F197001Data> list)
		{
			string errorMeg = ValidateImportF197001DataImpl(f1970, list);
			if (!string.IsNullOrEmpty(errorMeg))
			{
				return new ExecuteResult(false, errorMeg);
			}

			return new ExecuteResult(true);
		}

		string ValidateImportF197001DataImpl(F1970 f1970, List<F197001Data> list)
		{
			// 物料標：料號	廠商代碼	列印數量
			// 保固貼：廠商代碼	列印數量	保固日期	保固年限
			// 保固貼：列印數量	保固日期	保固年限
			switch (f1970.LABEL_TYPE)
			{
				case "0":	// 物料標
					if (list.Any(x => string.IsNullOrEmpty(x.ITEM_CODE)))
						return Properties.Resources.P197001Service_ITEM_CODE_Required;

					if (f1970.GUP_CODE == "01" && list.Any(x => string.IsNullOrEmpty(x.VNR_CODE)))
						return Properties.Resources.P197001Service_VNR_Code_Required;

					if (list.Any(x => string.IsNullOrEmpty(x.ITEM_NAME)))
						return string.Format(Properties.Resources.P197001Service_ITEM_CODE_NotFound, list.Where(x => string.IsNullOrEmpty(x.ITEM_NAME)).Select(x => x.ITEM_CODE).FirstOrDefault());

					if (f1970.GUP_CODE == "01" && list.Any(x => string.IsNullOrEmpty(x.VNR_NAME)))
						return string.Format(Properties.Resources.P197001Service_VNR_Code_NotFound, list.Where(x => string.IsNullOrEmpty(x.VNR_NAME)).Select(x => x.VNR_CODE).FirstOrDefault());

					break;
				case "1": // 保固貼
					if (list.Any(x => string.IsNullOrEmpty(x.VNR_CODE)))
						return Properties.Resources.P197001Service_VNR_Code_Required;

					if (list.Any(x => string.IsNullOrEmpty(x.VNR_NAME)))
						return string.Format(Properties.Resources.P197001Service_VNR_Code_NotFound, list.Where(x => string.IsNullOrEmpty(x.VNR_NAME)).Select(x => x.VNR_CODE).FirstOrDefault());

					var f000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransaction);
					var warrantys = f000904Repo.GetF000904Data("F1970", "WARRANTY").ToList();
					var warrantyValues = warrantys.Select(x => x.VALUE).ToList();
					if (list.Any(x => !warrantyValues.Contains(x.WARRANTY)))
						return Properties.Resources.P197001Service_warrantyYear_Required;

					if (list.Any(x => x.WarrantyDate == default(DateTime)))
						return Properties.Resources.P197001Service_warrantyDate_Required;

					break;
			}

			if (list.Any(x => x.Qty <= 0))
				return Properties.Resources.P197001Service_PrintCount_Required;

			return string.Empty;
		}

		public ExecuteResult InsertOrUpdateF197001s(F1970 f1970, List<F197001> list)
		{
			// 物料標
			if (f1970.LABEL_TYPE == "0")
			{
				InsertOrUpdateF197001sByLabelType0(f1970, list);
			}
			// 保固貼
			else if (f1970.LABEL_TYPE == "1")
			{
				InsertOrUpdateF197001sByLabelType1(f1970, list);
			}
			return new ExecuteResult(true);
		}

		void InsertOrUpdateF197001sByLabelType0(F1970 f1970, List<F197001> list)
		{
			var f197001Repo = new F197001Repository(Schemas.CoreSchema, _wmsTransaction);
			var seq = GetF197001Seq(f1970.GUP_CODE, f1970.CUST_CODE);
			foreach (var item in list)
			{
				F197001 f197001 = null;
				if (item.LABEL_SEQ > 0)
				{
					f197001 = f197001Repo.Find(x => x.GUP_CODE == item.GUP_CODE
								 && x.CUST_CODE == item.CUST_CODE
								 && x.LABEL_SEQ == item.LABEL_SEQ);
				}

				if (f197001 == null)
				{
					f197001 = f197001Repo.Find(x => x.GUP_CODE == item.GUP_CODE
												   && x.CUST_CODE == item.CUST_CODE
												   && x.LABEL_CODE == item.LABEL_CODE
												   && x.ITEM_CODE == item.ITEM_CODE
												   && x.VNR_CODE == item.VNR_CODE);
				}

				if (f197001 == null)
				{
					item.LABEL_SEQ = seq;
					seq++;
				}

				InsertOrUpdateF197001(f197001Repo, f197001, item);
			}
		}

		void InsertOrUpdateF197001sByLabelType1(F1970 f1970, List<F197001> list)
		{
			var f197001Repo = new F197001Repository(Schemas.CoreSchema, _wmsTransaction);
			var seq = GetF197001Seq(f1970.GUP_CODE, f1970.CUST_CODE);

			// 每個廠商都能有一張保固貼
			foreach (var g in list.GroupBy(x => new { x.GUP_CODE, x.CUST_CODE, x.LABEL_CODE, x.VNR_CODE }))
			{
				var f197001 = f197001Repo.Find(x => x.GUP_CODE == g.Key.GUP_CODE
												 && x.CUST_CODE == g.Key.CUST_CODE
												 && x.LABEL_CODE == g.Key.LABEL_CODE
												 && x.VNR_CODE == g.Key.VNR_CODE);
				var item = g.Last();
				if (f197001 == null)
				{
					item.LABEL_SEQ = seq;
					seq++;
				}

				InsertOrUpdateF197001(f197001Repo, f197001, item);
			}
		}

		void InsertOrUpdateF197001(F197001Repository f197001Repo, F197001 f197001Entity, F197001 item)
		{
			if (f197001Entity == null)
			{
				f197001Repo.Add(item);
			}
			else
			{
				f197001Entity.CHECK_STAFF = item.CHECK_STAFF;
				f197001Entity.ITEM_DESC_A = item.ITEM_DESC_A;
				f197001Entity.ITEM_DESC_B = item.ITEM_DESC_B;
				f197001Entity.ITEM_DESC_C = item.ITEM_DESC_C;
				f197001Entity.OUTSOURCE = item.OUTSOURCE;
				f197001Entity.WARRANTY = item.WARRANTY;
				f197001Entity.WARRANTY_D = item.WARRANTY_D;
				f197001Entity.WARRANTY_M = item.WARRANTY_M;
				f197001Entity.WARRANTY_S_M = item.WARRANTY_S_M;
				f197001Entity.WARRANTY_S_Y = item.WARRANTY_S_Y;
				f197001Entity.WARRANTY_Y = item.WARRANTY_Y;
				f197001Repo.Update(f197001Entity);
			}
		}


	}
}
