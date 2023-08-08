using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F00;
using Wms3pl.Common.Security;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P19.Services
{
	/// <summary>
	/// 系統功能
	/// </summary>
	public partial class P190501Service
	{
		private WmsTransaction _wmsTransaction;
    private CommonService _commonService;

		public P190501Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 傳回貨主所屬的物流中心 (F190101), 包含DC層
		/// </summary>
		/// <param name="gupId"></param>
		/// <returns></returns>
		public IQueryable<F190101Data> GetF190101MappingTable(string dcCode,string gupId)
		{
			var repo = new F190101Repository(Schemas.CoreSchema, _wmsTransaction);

			var result = repo.GetF190101MappingTable(dcCode,gupId);

			return result;
		}

		public ExecuteResult InsertP190501(string gupCode, string custCode, F1924 emp, List<string> groups, List<F192402> custs, string userId, string newMenuName,string selectedDcCode)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var repoF1924 = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF192401 = new F192401Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF192402 = new F192402Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1952 = new F1952Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF060301 = new F060301Repository(Schemas.CoreSchema, _wmsTransaction);

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

            // 0. 先檢查輸入的帳號是否已存在
            if (repoF1924.Filter(x => x.EMP_ID.Equals(emp.EMP_ID)).Any())
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P190501Service_EmpExist;
				return result;
			}
			var f1924Data = new F1924()
			{
				EMP_ID = emp.EMP_ID,
				EMP_NAME = emp.EMP_NAME,
				EMAIL = emp.EMAIL,
				ISCOMMON = emp.ISCOMMON,
				DEP_ID = emp.DEP_ID,
				TEL_EXTENSION = emp.TEL_EXTENSION,
				MOBILE = emp.MOBILE,
				SHORT_MOBILE = emp.SHORT_MOBILE,
				MENUSTYLE = emp.MENUSTYLE
			};
			if (emp.MENUSTYLE == "1")
			{
				if (string.IsNullOrWhiteSpace(newMenuName))
					f1924Data.MENU_CODE = emp.MENU_CODE;
				else
				{
					f1924Data.MENU_CODE = CopyMenuToNewMenu(emp.MENU_CODE, newMenuName);
				}
			}
			else
				f1924Data.MENU_CODE = null;
			// 1. 新增F1924
			repoF1924.Add(f1924Data);

			// 2. 新增F192401
			foreach (var p in groups.Where(x => !string.IsNullOrEmpty(x)))
			{
				repoF192401.Add(new F192401()
				{
					EMP_ID = emp.EMP_ID,
					GRP_ID = Convert.ToDecimal(p)
				});
			}

			// 3. 新增F192402
			foreach (var p in custs)
			{
				repoF192402.Add(new F192402()
				{
					EMP_ID = emp.EMP_ID,
					DC_CODE = p.DC_CODE,
					CUST_CODE = p.CUST_CODE,
					GUP_CODE = p.GUP_CODE
				});

                // 新增人員主檔派發作業(F060301)
                repoF060301.Add(new F060301
                {
                    DC_CODE = p.DC_CODE,
                    WAREHOUSE_ID = "ALL",
                    EMP_ID = emp.EMP_ID,
                    CMD_TYPE = "1",
                    STATUS = "0"
                });
            }

			// 4. 新增預設密碼
			var defPass = _commonService.GetSysGlobalValue("DefaultPassword");

			if (!string.IsNullOrWhiteSpace(defPass))
			{
				repoF1952.Add(new F1952()
				{
					EMP_ID = emp.EMP_ID,
					PASSWORD = defPass
        });
			}

			return result;
		}

		public ExecuteResult UpdateP190501(F1924 emp, List<string> groups, List<F192402> custs, string gupCode, string userId, string newMenuName, string selectedDcCode)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var repoF1924 = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF192401 = new F192401Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF192402 = new F192402Repository(Schemas.CoreSchema, _wmsTransaction);
            var repoF060301 = new F060301Repository(Schemas.CoreSchema, _wmsTransaction);

            // 0. 先檢查是否已被刪除
            var f1924Data = repoF1924.Find(x => x.EMP_ID.Equals(emp.EMP_ID));
			if (f1924Data == null)
			{
				// 資料已被刪除
				result.IsSuccessed = false;
				result.Message = Properties.Resources.DataDelete;
				return result;
			}
            
            var addDatas = new List<F060301>();
            var updDatas = new List<F060301>();
            var thirdPartDatas = repoF060301.GetDatas(emp.EMP_ID).ToList();

            // 若有更改姓名，將新增，並且排除資料庫存在的
            var dcCodes = custs.Select(x => x.DC_CODE).ToList();
            if (f1924Data.EMP_NAME != emp.EMP_NAME)
            {
                var exclude = thirdPartDatas.Where(x => x.CMD_TYPE == "1" && dcCodes.Contains(x.DC_CODE)).Select(x => x.DC_CODE);
                addDatas.AddRange(custs.Where(x => !exclude.Contains(x.DC_CODE)).Select(x => new F060301
                {
                    DC_CODE = x.DC_CODE,
                    WAREHOUSE_ID = "ALL",
                    EMP_ID = emp.EMP_ID,
                    CMD_TYPE = "1",
                    STATUS = "0"
                }).ToList());
            }

			// 1. 更新主檔
			f1924Data.EMP_NAME = emp.EMP_NAME;
			f1924Data.EMAIL = emp.EMAIL;
			f1924Data.ISCOMMON = emp.ISCOMMON;
			f1924Data.DEP_ID = emp.DEP_ID;
			f1924Data.TEL_EXTENSION = emp.TEL_EXTENSION;
			f1924Data.MOBILE = emp.MOBILE;
			f1924Data.SHORT_MOBILE = emp.SHORT_MOBILE;
			f1924Data.MENUSTYLE = emp.MENUSTYLE;
			if (emp.MENUSTYLE == "1")
			{
				if (string.IsNullOrWhiteSpace(newMenuName))
					f1924Data.MENU_CODE = emp.MENU_CODE;
				else
				{
					f1924Data.MENU_CODE = CopyMenuToNewMenu(emp.MENU_CODE, newMenuName);
				}
			}
			else
				f1924Data.MENU_CODE = null;
		
			repoF1924.Update(f1924Data);

			// 2. 更新F192401 - 刪除不在選取範圍裡的資料
			foreach (var p in repoF192401.Filter(x => x.EMP_ID.Equals(emp.EMP_ID)))
			{
				if (!groups.Contains(p.GRP_ID.ToString()))
					repoF192401.Delete(x => x.EMP_ID == emp.EMP_ID && x.GRP_ID == p.GRP_ID);
			}
			// 2.1.更新F192401 - 寫入新資料
			foreach (var p in groups.Where(x => !string.IsNullOrEmpty(x)))
			{
				decimal tmpGrpId = Convert.ToDecimal(p);
				if (repoF192401.Find(x => x.EMP_ID.Equals(emp.EMP_ID) && x.GRP_ID.Equals(tmpGrpId)) != null) continue;
				repoF192401.Add(new F192401()
				{
					EMP_ID = emp.EMP_ID,
					GRP_ID = tmpGrpId
				});
			}
			var f192402Data = repoF192402.Filter(x => x.EMP_ID.Equals(emp.EMP_ID) && x.GUP_CODE.Equals(gupCode));
			if (!string.IsNullOrEmpty(selectedDcCode))
				f192402Data = f192402Data.Where(x => x.DC_CODE == selectedDcCode);
			// 3. 更新F192402 - 刪除不在選取範圍內的資料 *只處理目前的GUP CODE
			foreach (var p in f192402Data)
			{
				string dcCode = p.DC_CODE;
				string gupCode1 = p.GUP_CODE;
				string custCode = p.CUST_CODE;
				if (!custs.Any(x => x.DC_CODE.Equals(dcCode) && x.CUST_CODE.Equals(custCode) && x.GUP_CODE.Equals(gupCode1)))
				{
					// 在F192402裡找不到該項目, 所以要做刪除的動作
					repoF192402.Delete(x => x.EMP_ID == emp.EMP_ID && x.DC_CODE == dcCode && x.CUST_CODE == custCode && x.GUP_CODE == gupCode1);

                    // 找出尚未執行的人員主檔派發作業停用
                    var datas = thirdPartDatas.Where(x => x.DC_CODE == dcCode).ToList();
                    if (datas.Any())
                    {
                        datas.ForEach(f060301 => { f060301.STATUS = "9"; });
                        updDatas.AddRange(datas);
                    }
                    else
                    {
                        // 新增人員主檔派發作業(刪除)
                        addDatas.Add(new F060301
                        {
                            DC_CODE = dcCode,
                            WAREHOUSE_ID = "ALL",
                            EMP_ID = emp.EMP_ID,
                            CMD_TYPE = "2",
                            STATUS = "0"
                        });
                    }
                }
			}

			// 3.1 更新F192402 - 寫入新資料
			foreach (var p in custs)
			{
				if (!repoF192402.Filter(x => x.DC_CODE.Equals(p.DC_CODE)
					&& x.EMP_ID.Equals(emp.EMP_ID)
					&& x.CUST_CODE.Equals(p.CUST_CODE)
					&& x.GUP_CODE.Equals(p.GUP_CODE)).Any())
				{
					// 在原始資料找不到該項目時做新增動作
					repoF192402.Add(new F192402()
					{
						EMP_ID = emp.EMP_ID,
						DC_CODE = p.DC_CODE,
						GUP_CODE = p.GUP_CODE,
						CUST_CODE = p.CUST_CODE
					});

                    // 將資料庫有要刪除的改成9
                    var updData = thirdPartDatas.Where(x => x.DC_CODE == p.DC_CODE && x.CMD_TYPE == "2").FirstOrDefault();
                    if (updData != null)
                    {
                        updData.STATUS = "9";
                        updDatas.Add(updData);
                    }

                    // 避免跟已經加入要新增的資料重複加入
                    if (!addDatas.Where(x => x.DC_CODE == p.DC_CODE && x.CMD_TYPE == "1").Any())
                    {
                        // 新增人員主檔派發作業(新增)
                        addDatas.Add(new F060301
                        {
                            DC_CODE = p.DC_CODE,
                            WAREHOUSE_ID = "ALL",
                            EMP_ID = emp.EMP_ID,
                            CMD_TYPE = "1",
                            STATUS = "0"
                        });
                    }
                }
			}

            if (addDatas.Any())
                repoF060301.BulkInsert(addDatas);
            if (updDatas.Any())
                repoF060301.BulkUpdate(updDatas);

            return result;
		}

        public ExecuteResult DeleteP190501(string empId)
        {
            ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
            var repoF1924 = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);
            var repoF060301 = new F060301Repository(Schemas.CoreSchema, _wmsTransaction);

            var f1924 = repoF1924.Find(o => o.EMP_ID == empId);

            if (f1924 != null)
            {
                f1924.ISDELETED = "1";
                repoF1924.Update(f1924);

                #region 新增或修改F060301
                var f060301s = repoF060301.GetDatasByTrueAndCondition(x => x.EMP_ID == empId);

                if (f060301s.Any())
                {
                    List<string> statusList = new List<string> { "0", "T" };

                    // 將尚未發送新增的改為取消
                    var notExecuteF060301s = f060301s.Where(x => x.CMD_TYPE == "1" && statusList.Contains(x.STATUS)).ToList();
                    notExecuteF060301s.ForEach(obj =>
                    {
                        obj.STATUS = "9";
                        repoF060301.Update(obj);
                    });

                    // 將其他已送過的加入刪除
                    var dcCodes = f060301s.Where(x => x.CMD_TYPE == "1" && !notExecuteF060301s.Select(z => z.DC_CODE).Contains(x.DC_CODE)).Select(x => x.DC_CODE).Distinct().ToList();
                    dcCodes.ForEach(dcCode => {
                        repoF060301.Add(new F060301
                        {
                            DC_CODE = dcCode,
                            WAREHOUSE_ID = "ALL",
                            EMP_ID = empId,
                            CMD_TYPE = "2",
                            STATUS = "0"
                        });
                    });

                }
                #endregion
            }

            return result;
        }

        private string CopyMenuToNewMenu(string oldMenuCode,string newMenuName)
		{
			var f195402Repo = new F195402Repository(Schemas.CoreSchema,_wmsTransaction);
			string newMenuCode = f195402Repo.GetNewMenuCode();
			f195402Repo.Add(new F195402
			{
				MENU_CODE = newMenuCode,
				MENU_DESC = newMenuName
			});

			var f19540201Repo = new F19540201Repository(Schemas.CoreSchema, _wmsTransaction);
			f19540201Repo.CopyMenuToNewMenu(oldMenuCode, newMenuCode);
			var f19540202Repo = new F19540202Repository(Schemas.CoreSchema, _wmsTransaction);
			f19540202Repo.CopyMenuCategoryToNewMenuCategory(oldMenuCode, newMenuCode);
			return newMenuCode;
		}
		/// <summary>
		/// 刪除F1924, 不刪除F192401/ F192402
		/// </summary>
		/// <param name="empId"></param>
		/// <returns></returns>
		public ExecuteResult DeleteUser(string empId, string userId)
		{
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var repoF1924 = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);

			repoF1924.Delete(empId, userId);

			return result;
		}
	}
}

