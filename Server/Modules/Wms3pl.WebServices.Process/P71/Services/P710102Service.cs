using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P71.Services
{
    public partial class P710102Service
    {
        private WmsTransaction _wmsTransaction;
        public P710102Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        public IQueryable<F1919Data> GetF1919Datas(string dcCode, string gupCode, string custCode, string warehourseId, string areaCode)
        {
            var f1919Repo = new F1919Repository(Schemas.CoreSchema);
            return f1919Repo.GetF1919Datas(dcCode, gupCode, custCode, warehourseId, areaCode);
        }

        public ExecuteResult CheckF1919Data(F1919Data f1919Data, List<string> selectLocs)
        {
            var result = new ExecuteResult { IsSuccessed = true };
            if (f1919Data.AREA_CODE == null)
                f1919Data.ATYPE_CODE = "-1";
            var f1919Repo = new F1919Repository(Schemas.CoreSchema);
            var f1912Repo = new F1912Repository(Schemas.CoreSchema);
            //同一個倉別 儲區不可有重複的儲位區間範圍
            //var repeat = f1912Repo.Filter(o => selectLocs.Contains(o.LOC_CODE) && (!o.AREA_CODE.Contains("-1") && !o.AREA_CODE.Contains(f1919Data.AREA_CODE)) && o.WAREHOUSE_ID == f1919Data.WAREHOUSE_ID).OrderBy(o => o.LOC_CODE).ToList();
            var splitLoc = SplitList(selectLocs);
            List<F1912> repeat = new List<F1912>();
            foreach (var item in splitLoc)
            {
                repeat.AddRange(f1912Repo.Filter(o => item.Contains(o.LOC_CODE) && (!o.AREA_CODE.Contains("-1") && !o.AREA_CODE.Contains(f1919Data.AREA_CODE)) && o.WAREHOUSE_ID == f1919Data.WAREHOUSE_ID && o.DC_CODE == f1919Data.DC_CODE).OrderBy(o => o.LOC_CODE));
            }

            result.IsSuccessed = !repeat.Any();
            if (!result.IsSuccessed)
            {
                result.Message = "【儲位範圍重複，請重新設定】" + Environment.NewLine + " 重複如下:";
                result.Message += Environment.NewLine +
                                                    string.Format("[倉別]:{0} {1} [業主]:{2} [貨主]:{3} [儲區]:{4} [儲區範圍]:{5} ~ {6}", f1919Data.WAREHOUSE_ID,
                                                        f1919Data.WAREHOUSE_Name, f1919Data.GUP_NAME, f1919Data.CUST_NAME, f1919Data.AREA_NAME,
                                                        repeat.FirstOrDefault().LOC_CODE, repeat.LastOrDefault().LOC_CODE);
            }
            else
            {
                //修改儲區要檢查原儲區儲位是否有商品
                if (f1919Data.AREA_CODE != "-1")
                {
                    //新儲位資料
                    //var newAreaf1912Data =  GetNewAreaF1912Datas(f1919Data);
                    //原儲位資料
                    var orginalAreaF1912Data = GetOrginalAreaF1912Datas(f1919Data.DC_CODE, f1919Data.AREA_CODE).Select(o => o.LOC_CODE).ToList();

                    //取得原儲位不存在新儲位區間上儲位
                    var allAreaLocCodeList = orginalAreaF1912Data.Except(selectLocs).ToList();

                    var f1913Repo = new F1913Repository(Schemas.CoreSchema);
                    //result.IsSuccessed = !f1913Repo.Filter(o => o.DC_CODE == EntityFunctions.AsNonUnicode(f1919Data.DC_CODE)
                    //&& allAreaLocCodeList.Contains(o.LOC_CODE)).Any();
                    var splitAllAreaLocCodeList = SplitList(allAreaLocCodeList);
                    result.IsSuccessed = true;
                    foreach (var item in splitAllAreaLocCodeList)
                        result.IsSuccessed &= !f1913Repo.CheckF1913HasData(f1919Data.DC_CODE, item);

                    if (!result.IsSuccessed)
                        result.Message = "儲區已存在商品";
                }
            }
            return result;
        }

        private List<List<string>> SplitList(List<string> source, int chunkSize = 2000)
        {
            var result = new List<List<string>>();
            var sourceCount = source.Count;
            for (var i = 0; i < sourceCount; i += chunkSize)
            {
                result.Add(source.GetRange(i, Math.Min(chunkSize, sourceCount - i)));
            }
            return result;
        }

        private int ChangeEnglishToNumber(string english)
        {
            var upString = english.ToUpper();
            int result = 0;
            for (int i = 0; i < english.Length; i++)
            {
                int letter = (int)upString[i];
                int a = 1;

                for (int x = 1; x < (english.Length - i); x++)
                    a *= 36;
                if (i + 1 != english.Length)
                {
                    if (letter >= 65 && letter <= 90)
                        result += (letter - 55) * a;
                    else if (letter >= 48 && letter <= 57)
                        result += (letter - 48) * a;
                }
                else
                {
                    if (letter >= 65 && letter <= 90)
                        result += (letter - 55);
                    else if (letter >= 48 && letter <= 57)
                        result += (letter - 48);
                }
            }

            return result;
        }

        public ExecuteResult InsertF1919Data(F1919Data f1919Data, string userId, List<string> selectLocs)
        {
            var result = new ExecuteResult { IsSuccessed = true };
            var newF191202List = new List<F191202>();
            var f1919Repo = new F1919Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1919Item = f1919Repo.Filter(o => o.DC_CODE == f1919Data.DC_CODE && o.AREA_CODE.Contains(f1919Data.ATYPE_CODE))
				.OrderByDescending(o => o.AREA_CODE)
				.FirstOrDefault();
			int seq = 1;
            if (f1919Item != null)
                seq = int.Parse(f1919Item.AREA_CODE.Replace(f1919Data.ATYPE_CODE, "")) + 1;
            //儲區代號 =英文字(儲區型態編號)+兩碼數字(自動遞增)
            var areaCode = f1919Data.ATYPE_CODE + seq.ToString().PadLeft(2, '0');
            //新增儲區設定
            var f1919 = new F1919
            {
                AREA_CODE = areaCode,
                AREA_NAME = f1919Data.AREA_NAME,
                ATYPE_CODE = f1919Data.ATYPE_CODE,
                WAREHOUSE_ID = f1919Data.WAREHOUSE_ID,
                DC_CODE = f1919Data.DC_CODE,
                CRT_STAFF = userId,
                CRT_DATE = DateTime.Now,
                UPD_STAFF = userId,
                UPD_DATE = DateTime.Now
            };
            f1919Repo.Add(f1919);

            //判斷是否要新增儲區設定檔
            if (f1919Data.IsCreateStorageAreaPickSetting)
            {
                //新增儲區設定
                var f191902Repo = new F191902Repository(Schemas.CoreSchema, _wmsTransaction);

                var f191902 = new F191902
                {
                    AREA_CODE = areaCode,
                    DC_CODE = f1919Data.DC_CODE,
                    GUP_CODE = f1919Data.GUP_CODE,
                    CUST_CODE = f1919Data.CUST_CODE,
                    WAREHOUSE_ID = f1919Data.WAREHOUSE_ID,
                    PICK_TYPE = f1919Data.PICK_TYPE,
                    PICK_TOOL = f1919Data.PICK_TOOL,
                    PUT_TOOL = f1919Data.PUT_TOOL,
                    PICK_SEQ = f1919Data.PICK_SEQ,
                    SORT_BY = f1919Data.SORT_BY,
                    SINGLE_BOX = f1919Data.SINGLE_BOX,
                    PICK_CHECK = f1919Data.PICK_CHECK,
                    PICK_UNIT = f1919Data.PICK_UNIT,
                    PICK_MARTERIAL = f1919Data.PICK_MARTERIAL,
                    DELIVERY_MARTERIAL = f1919Data.DELIVERY_MARTERIAL,
                    CRT_DATE = DateTime.Now,
                    CRT_STAFF = userId,
                    UPD_DATE = DateTime.Now,
                    UPD_STAFF = userId,
                    MOVE_TOOL = f1919Data.MOVE_TOOL
                };
                f191902Repo.Add(f191902);
            }

            //更新儲位設定儲區代號
            var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
            var f191202Repo = new F191202Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1912upList = new List<F1912>();
            var commonsvr = new CommonService();
            //var newAreaf1912Data = GetNewAreaF1912Datas(f1919Data).ToList();
            //var newAreaf1912Data = f1912Repo.Filter(o => selectLocs.Contains(o.LOC_CODE) && o.DC_CODE == f1919Data.DC_CODE);
            var newAreaf1912Data = commonsvr.GetLocListUseSplit(f1919Data.DC_CODE, selectLocs);
            if (!newAreaf1912Data.Any())
                return new ExecuteResult(false, "此倉別找不到可新增的儲位，請確認資料是否異常!");

            foreach (var f1912 in newAreaf1912Data)
            {
                AddF191202Log(userId, "2", "0", f1912, newF191202List);
                //f191202Repo.Log(f1912, userId, "2", "0");
                f1912.AREA_CODE = areaCode;
                f1912.UPD_DATE = DateTime.Now;
                f1912.UPD_STAFF = userId;
                f1912upList.Add(f1912);
                //f1912Repo.Update(f1912);
                AddF191202Log(userId, "2", "1", f1912, newF191202List);
                //f191202Repo.Log(f1912, userId, "2", "1");
            }
            f1912Repo.BulkUpdate(f1912upList);
            f191202Repo.BulkInsert(newF191202List);
            return result;
        }

        public ExecuteResult UpdateF1919Data(F1919Data f1919Data, string userId, List<string> selectLocs)
        {
            var result = new ExecuteResult { IsSuccessed = true };
            var newF191202List = new List<F191202>();

            var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
            var f191202Repo = new F191202Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1912upList = new List<F1912>();
            //新儲位資料
            //var newAreaf1912Data = f1912Repo.Filter(o => selectLocs.Contains(o.LOC_CODE) && o.DC_CODE == f1919Data.DC_CODE).ToList();
            //GetNewAreaF1912Datas(f1919Data).ToList();
            //原儲位資料
            //var splitLocs = SplitList(selectLocs);

            var newAreaf1912Data = f1912Repo.GetF1912DataSQL(f1919Data.DC_CODE, selectLocs).ToList();

            var orginalAreaF1912Data = GetOrginalAreaF1912Datas(f1919Data.DC_CODE, f1919Data.AREA_CODE).ToList();

            //取得原儲位不存在新儲位區間上儲位
            var exceptareaF1912Data = orginalAreaF1912Data.Except(newAreaf1912Data, new F1912Comparer());

            //更新儲位為空儲區
            foreach (var f1912 in exceptareaF1912Data)
            {
                AddF191202Log(userId, "2", "0", f1912, newF191202List);
                //f191202Repo.Log(f1912, userId, "2", "0");
                f1912.AREA_CODE = "-1";
                f1912.UPD_DATE = DateTime.Now;
                f1912.UPD_STAFF = userId;
                f1912upList.Add(f1912);
                //f1912Repo.Update(f1912);
                AddF191202Log(userId, "2", "1", f1912, newF191202List);
                //f191202Repo.Log(f1912, userId, "2", "1");
            }
            //取得新儲位不存在原儲位區間上儲位
            var exceptF1912Data = newAreaf1912Data.Except(orginalAreaF1912Data, new F1912Comparer());
            //更新儲位設定儲區代號
            foreach (var f1912 in exceptF1912Data)
            {
                AddF191202Log(userId, "2", "0", f1912, newF191202List);
                //f191202Repo.Log(f1912, userId, "2", "0");
                f1912.AREA_CODE = f1919Data.AREA_CODE;
                f1912.UPD_DATE = DateTime.Now;
                f1912.UPD_STAFF = userId;
                f1912upList.Add(f1912);
                //f1912Repo.Update(f1912);
                AddF191202Log(userId, "2", "1", f1912, newF191202List);
                //f191202Repo.Log(f1912, userId, "2", "1");
            }
            f1912Repo.BulkUpdate(f1912upList);
            f191202Repo.BulkInsert(newF191202List);

            var f191902Repo = new F191902Repository(Schemas.CoreSchema, _wmsTransaction);
            var upF191902 = new F191902();

            var gupCode = f1919Data.GUP_CODE ?? "0";
            var custCode = f1919Data.CUST_CODE ?? "0";


            upF191902 = f191902Repo.Find(o => o.AREA_CODE == f1919Data.AREA_CODE &&
o.DC_CODE == f1919Data.DC_CODE &&
o.GUP_CODE == gupCode &&
o.CUST_CODE == custCode &&
o.WAREHOUSE_ID == f1919Data.WAREHOUSE_ID);

            //判斷是否要新增(更新)儲區設定檔
            if (f1919Data.IsCreateStorageAreaPickSetting)
            {
                //編輯時如果砍掉要在新增一個新的
                if (upF191902 == null)
                {
                    var f191902 = new F191902
                    {
                        AREA_CODE = f1919Data.AREA_CODE,
                        DC_CODE = f1919Data.DC_CODE,
                        GUP_CODE = gupCode,
                        CUST_CODE = custCode,
                        WAREHOUSE_ID = f1919Data.WAREHOUSE_ID,
                        PICK_TYPE = f1919Data.PICK_TYPE,
                        PICK_TOOL = f1919Data.PICK_TOOL,
                        PUT_TOOL = f1919Data.PUT_TOOL,
                        PICK_SEQ = f1919Data.PICK_SEQ,
                        SORT_BY = f1919Data.SORT_BY,
                        SINGLE_BOX = f1919Data.SINGLE_BOX,
                        PICK_CHECK = f1919Data.PICK_CHECK,
                        PICK_UNIT = f1919Data.PICK_UNIT,
                        PICK_MARTERIAL = f1919Data.PICK_MARTERIAL,
                        DELIVERY_MARTERIAL = f1919Data.DELIVERY_MARTERIAL,
                        CRT_DATE = DateTime.Now,
                        CRT_STAFF = userId,
                        UPD_DATE = DateTime.Now,
                        UPD_STAFF = userId,
                        MOVE_TOOL = f1919Data.MOVE_TOOL
                    };
                    f191902Repo.Add(f191902);
                }
                //如果沒砍掉就正常更新
                else
                {
                    upF191902.PICK_TYPE = f1919Data.PICK_TYPE;
                    upF191902.PICK_TOOL = f1919Data.PICK_TOOL;
                    upF191902.PUT_TOOL = f1919Data.PUT_TOOL;
                    upF191902.PICK_SEQ = f1919Data.PICK_SEQ;
                    upF191902.SORT_BY = f1919Data.SORT_BY;
                    upF191902.SINGLE_BOX = f1919Data.SINGLE_BOX;
                    upF191902.PICK_CHECK = f1919Data.PICK_CHECK;
                    upF191902.PICK_UNIT = f1919Data.PICK_UNIT;
                    upF191902.PICK_MARTERIAL = f1919Data.PICK_MARTERIAL;
                    upF191902.DELIVERY_MARTERIAL = f1919Data.DELIVERY_MARTERIAL;
                    upF191902.UPD_DATE = DateTime.Now;
                    upF191902.UPD_STAFF = userId;
                    upF191902.MOVE_TOOL = f1919Data.MOVE_TOOL;

                    f191902Repo.Update(upF191902);
                }
            }
            else
            {
                if (upF191902 != null)
                {
                    f191902Repo.Delete(o => o.AREA_CODE == f1919Data.AREA_CODE &&
                   o.DC_CODE == f1919Data.DC_CODE &&
                   o.GUP_CODE == gupCode &&
                   o.CUST_CODE == custCode &&
                   o.WAREHOUSE_ID == f1919Data.WAREHOUSE_ID);
                }
            }

            return result;
        }

        /// <summary>
        /// 新儲區儲位資料
        /// </summary>
        /// <param name="f1919Data"></param>
        /// <returns></returns>
        private IQueryable<F1912> GetNewAreaF1912Datas(F1919Data f1919Data)
        {
            var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
            return f1912Repo.GetF1912DatasByRange(f1919Data.DC_CODE, f1919Data.GUP_CODE, f1919Data.CUST_CODE,
                f1919Data.FLOOR, f1919Data.MINCHANNEL, f1919Data.MAXCHANNEL, f1919Data.MINPLAIN, f1919Data.MAXPLAIN,
                f1919Data.MINLOC_LEVEL, f1919Data.MAXLOC_LEVEL, f1919Data.MINLOC_TYPE, f1919Data.MAXLOC_TYPE)
                .Where(o => o.WAREHOUSE_ID == f1919Data.WAREHOUSE_ID);
        }

        /// <summary>
        /// 原儲區儲位資料
        /// </summary>
        /// <param name="areaCode">儲區代號</param>
        /// <returns></returns>
        private IQueryable<F1912> GetOrginalAreaF1912Datas(string dcCode, string areaCode)
        {
            var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
            return f1912Repo.GetF1912DatasByAreaCode(dcCode,areaCode);
        }

        private void AddF191202Log(string userId, string status, string way, F1912 f1912, List<F191202> f191202s)
        {
            var tmp = Mapper.DynamicMap<F191202>(f1912);
            tmp.TRANS_DATE = DateTime.Now;
            tmp.TRANS_STAFF = userId;
            tmp.TRANS_STATUS = status;
            tmp.TRANS_WAY = way;
            f191202s.Add(tmp);
        }

        private class F1912Comparer : IEqualityComparer<F1912>
        {
            public bool Equals(F1912 x, F1912 y)
            {

                if (Object.ReferenceEquals(x, y)) return true;

                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                return x.LOC_CODE == y.LOC_CODE && x.DC_CODE == y.DC_CODE;
            }


            public int GetHashCode(F1912 f1912)
            {
                if (Object.ReferenceEquals(f1912, null)) return 0;

                int hashProductName = f1912.LOC_CODE == null ? 0 : f1912.LOC_CODE.GetHashCode();

                int hashProductCode = f1912.DC_CODE.GetHashCode();

                return hashProductName ^ hashProductCode;
            }
        }
    }
}