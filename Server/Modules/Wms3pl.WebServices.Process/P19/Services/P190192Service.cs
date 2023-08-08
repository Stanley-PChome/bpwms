using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
    public class P190192Service
    {
        private WmsTransaction _wmsTransaction;
        public P190192Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        /// <summary>
        /// 檢查是否可以調整集貨格數量(含刪除)，可以的話回傳true
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="CollectionCode"></param>
        /// <param name="CellType"></param>
        /// <param name="CellNum">縮小後數量，刪除的話塞0</param>
        /// <returns></returns>
        public Boolean IsCellNumCanReduce(string dcCode, string CollectionCode, String CellType = null, int CellNum = 0)
        {
            var f051401Repo = new F051401Repository(Schemas.CoreSchema);
            F1945Repository f1945Repo;
            if (CellNum == 0)
            {
                var chkF051401= f051401Repo.Filter(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode && x.STATUS != "0").ToList();
                if (!String.IsNullOrWhiteSpace(CellType))
                    chkF051401 = chkF051401.Where(x => x.CELL_TYPE == CellType).ToList();
                if (chkF051401.Any())
                    return false;
                else
                    return true;
            }
            else
            {
                f1945Repo = new F1945Repository(Schemas.CoreSchema);
                var f1945 = f1945Repo.Find(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode && x.CELL_TYPE == CellType);
                //如果是增加集貨格就不用檢查，減少才要
                if (f1945.CELL_NUM <= CellNum)
                    return true;

                if (f051401Repo.Filter(x =>
                        x.DC_CODE == dcCode &&
                        x.COLLECTION_CODE == CollectionCode &&
                        x.CELL_TYPE == CellType &&
                        x.STATUS != "0" &&
                        x.CELL_CODE.CompareTo(f1945.CELL_START_CODE + CellNum.ToString("0000")) >= 0)
                        .Any())
                    return false;
                else
                    return true;
            }
        }

        public ExecuteResult DeleteF1945Collection(string dcCode, string CollectionCode)
        {
            var result = new ExecuteResult(true);
            var f051401Repo = new F051401Repository(Schemas.CoreSchema, _wmsTransaction);
            var f051402Repo = new F051402Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1945Repo = new F1945Repository(Schemas.CoreSchema, _wmsTransaction);

            var f1945s = f1945Repo.Filter(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode);
            if (!IsCellNumCanReduce(dcCode, CollectionCode))
            {
                result.IsSuccessed = false;
                result.Message = $"此集貨場{f1945s.First().COLLECTION_NAME}，集貨格正在集貨中，不可刪除";
                return result;
            }
            else
            {
                f051402Repo.UpdateFields(new { COLLECTION_NAME = f1945s.First().COLLECTION_NAME }, x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode);
                f051401Repo.Delete(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode);
                f1945Repo.Delete(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode);
                return result;
            }
        }

        public ExecuteResult InsertF1945(string dcCode, string gupCode, string custCode, string CollectionType, string CollectionCode, string CollectionName, List<F1945CellList> Addf1945CellLists)
        {
            var f1945Repo = new F1945Repository(Schemas.CoreSchema, _wmsTransaction);
            var f051401Repo = new F051401Repository(Schemas.CoreSchema, _wmsTransaction);

            foreach (var item in Addf1945CellLists)
            {
                f1945Repo.Add(new F1945()
                {
                    DC_CODE = dcCode,
                    COLLECTION_CODE = CollectionCode,
                    COLLECTION_NAME = CollectionName,
                    COLLECTION_TYPE = CollectionType,
                    CELL_START_CODE = item.CELL_START_CODE,
                    CELL_TYPE = item.CELL_TYPE,
                    CELL_NUM = item.CELL_NUM
                });

                for (int i = 1; i <= item.CELL_NUM; i++)
                {
                    f051401Repo.Add(new F051401()
                    {
                        DC_CODE = dcCode,
                        GUP_CODE = gupCode,
                        CUST_CODE = custCode,
                        COLLECTION_CODE = CollectionCode,
                        CELL_CODE = item.CELL_START_CODE + i.ToString("0000"),
                        CELL_TYPE = item.CELL_TYPE,
                        STATUS = "0",
                    });
                }
            }
            return new ExecuteResult(true);
        }

        public ExecuteResult UpdateF051401(string dcCode, string gupCode, string custCode, string CollectionType, string CollectionCode, string CollectionName, List<F1945CellList> F1945CellLists)
        {
            var f051401Repo = new F051401Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1945Repo = new F1945Repository(Schemas.CoreSchema, _wmsTransaction);

            var Getf1945CollectionName = f1945Repo.AsForUpdate().Filter(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode).FirstOrDefault();
            if (Getf1945CollectionName.COLLECTION_NAME != CollectionName)
                f1945Repo.UpdateFields(new { COLLECTION_NAME = CollectionName }, x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode);

            foreach (var item in F1945CellLists.Where(x => x.MODIFY_MODE == "D"))
            {
                f1945Repo.Delete(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode && x.CELL_TYPE == item.CELL_TYPE);
                f051401Repo.Delete(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode && x.CELL_TYPE == item.CELL_TYPE);
            }

            foreach (var item in F1945CellLists.Where(x => x.MODIFY_MODE != "D"))
            {

                if (item.MODIFY_MODE == "A")
                {
                    InsertF1945(dcCode, gupCode, custCode, CollectionType, CollectionCode, CollectionName, new[] { item }.ToList());
                }
                else if (item.MODIFY_MODE == "C")
                {
                    var Getf1945CellNum = f1945Repo.Find(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode && x.CELL_TYPE == item.CELL_TYPE);
                    //增加集貨格
                    if (Getf1945CellNum.CELL_NUM <= item.CELL_NUM)
                    {
                        var lastCellCode = f051401Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode && x.CELL_TYPE == item.CELL_TYPE).OrderByDescending(x => x.CELL_CODE).FirstOrDefault()?.CELL_CODE;
                        int NewStartCellNum = 1;
                        if (!String.IsNullOrWhiteSpace(lastCellCode))
                          NewStartCellNum = Convert.ToInt32(lastCellCode.Substring(1)) + 1;
                        for (int i = NewStartCellNum; i <= item.CELL_NUM; i++)
                        {
                            f051401Repo.Add(new F051401()
                            {
                                DC_CODE = dcCode,
                                GUP_CODE = gupCode,
                                CUST_CODE = custCode,
                                COLLECTION_CODE = CollectionCode,
                                CELL_CODE = item.CELL_START_CODE + i.ToString("0000"),
                                CELL_TYPE = item.CELL_TYPE,
                                STATUS = "0",
                            });
                        }
                    }
                    else //減少
                    {
                        f051401Repo.AdjustCellCount(dcCode, gupCode, custCode, CollectionCode, item.CELL_START_CODE + item.CELL_NUM.ToString("0000"), item.CELL_TYPE);
                    }
                    f1945Repo.UpdateFields(new { item.CELL_NUM }, x => x.DC_CODE == dcCode && x.COLLECTION_CODE == CollectionCode && x.CELL_TYPE == item.CELL_TYPE);
                }
            }
            return new ExecuteResult(true);
        }

    }
}
