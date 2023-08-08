using System.Linq;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P02.Services
{
    public partial class P020105Service
    {
        private WmsTransaction _wmsTransaction;
        public P020105Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        public ScanCargoData InsertF010301AndGetNewID(F010301 f010301Data)
        {
            var repo = new F010301Repository(Schemas.CoreSchema, _wmsTransaction);
            var result = repo.InsertF010301AndGetNewID(f010301Data);
            if (result.ID < 0) //只有成功 id>=0 失敗 id=-9999
                return result;
            var F010302Result = UpdateF010302CheckBoxCnt(f010301Data);
            _wmsTransaction.Complete();

            if (!F010302Result)
                return new ScanCargoData() { ID = -9000 }; //更新F010302失敗-9000
            else
                return result;
        }

        public ScanReceiptData InsertF010302AndReturnValue(ScanReceiptData f010302Data)
        {
            var f010301repo = new F010301Repository(Schemas.CoreSchema, _wmsTransaction);
            var f010302repo = new F010302Repository(Schemas.CoreSchema, _wmsTransaction);
            var CheckDupResult = f010302repo.Filter(x => x.DC_CODE == f010302Data.DC_CODE && x.ALL_ID == f010302Data.ALL_ID && x.SHIP_ORD_NO == f010302Data.SHIP_ORD_NO).ToList();
            if (CheckDupResult?.Any() ?? true)
                return new ScanReceiptData() { ID = -1 }; //已有重複資料


            var GetCHECK_BOX_CNT = f010301repo.Filter(x => x.DC_CODE == f010302Data.DC_CODE && x.ALL_ID == f010302Data.ALL_ID && x.SHIP_ORD_NO == f010302Data.SHIP_ORD_NO).ToList();
            if (GetCHECK_BOX_CNT.Any())
            {
                f010302Data.CHECK_BOX_CNT = (short)GetCHECK_BOX_CNT.Sum(x => x.BOX_CNT);
                f010302Data.SHIP_BOX_CNT = (short)GetCHECK_BOX_CNT.Sum(x => x.BOX_CNT);
            }
            else
                return new ScanReceiptData() { ID = -9000 }; //查無此貨運單號

            f010301repo.UpdateFields(new { CHECK_STATUS = GetCheckStatus(f010302Data) }, x => x.DC_CODE == f010302Data.DC_CODE && x.ALL_ID == f010302Data.ALL_ID && x.SHIP_ORD_NO == f010302Data.SHIP_ORD_NO);

            //f010302Data.SHIP_BOX_CNT = 1;
            //f010302Data.CHECK_STATUS = (f010302Data.CHECK_BOX_CNT == f010302Data.SHIP_BOX_CNT) ? "1" : "0";
            f010302Data.CHECK_STATUS = GetCheckStatus(f010302Data);

            f010302repo.Add(f010302Data);
            _wmsTransaction.Complete();

            return f010302repo.GetNewF010302Data();
        }

        public ExecuteResult UpdateF010302ShipBoxCnt(ScanReceiptData f010302Data)
        {
            var f010301repo = new F010301Repository(Schemas.CoreSchema, _wmsTransaction);
            f010301repo.UpdateFields(new { CHECK_STATUS = GetCheckStatus(f010302Data) }, x => x.DC_CODE == f010302Data.DC_CODE && x.ALL_ID == f010302Data.ALL_ID && x.SHIP_ORD_NO == f010302Data.SHIP_ORD_NO);

            var f010302repo = new F010302Repository(Schemas.CoreSchema, _wmsTransaction);
            f010302repo.UpdateFields(new { f010302Data.SHIP_BOX_CNT }, x => x.ID == f010302Data.ID);
            f010302repo.UpdateFields(new { CHECK_STATUS = GetCheckStatus(f010302Data) }, x => x.ID == f010302Data.ID);

            _wmsTransaction.Complete();
            return new ExecuteResult(true);
        }

        private string GetCheckStatus(F010302 f010302)
        {
            return f010302.SHIP_BOX_CNT == f010302.CHECK_BOX_CNT ? "1" : "0";
        }

        /// <summary>
        /// 刪除F010301資料時同步調整F010302內容
        /// </summary>
        /// <param name="front_f010301s"></param>
        /// <returns></returns>
        public ExecuteResult DeleteF010301ScanCargoDatas(ScanCargoData[] front_f010301s)
        {
            var f010301repo = new F010301Repository(Schemas.CoreSchema, _wmsTransaction);
            var f010302repo = new F010302Repository(Schemas.CoreSchema, _wmsTransaction);
            foreach (var item in front_f010301s)
                f010301repo.Delete(d => d.ID == item.ID);
            _wmsTransaction.Complete();

            UpdateF010302CheckBoxCnt(front_f010301s);
            _wmsTransaction.Complete();

            return new ExecuteResult(true);
        }

        public ExecuteResult UpdateF010301BoxCount(ScanCargoData front_f010301s)
        {
            var f010301repo = new F010301Repository(Schemas.CoreSchema, _wmsTransaction);
            var f010302repo = new F010302Repository(Schemas.CoreSchema, _wmsTransaction);
            f010301repo.UpdateFields(new { front_f010301s.BOX_CNT }, x => x.ID == front_f010301s.ID);
            _wmsTransaction.Complete();

            UpdateF010302CheckBoxCnt(front_f010301s);
            _wmsTransaction.Complete();

            return new ExecuteResult(true);
        }

        /// <summary>
        /// 依據傳入的F010301更新F010302的CheckBoxCnt、CHECK_STATUS
        /// </summary>
        /// <param name="f010301s"></param>
        /// <returns></returns>
        private bool UpdateF010302CheckBoxCnt(F010301 f010301s)
        {
            var f010301repo = new F010301Repository(Schemas.CoreSchema, _wmsTransaction);
            var f010302repo = new F010302Repository(Schemas.CoreSchema, _wmsTransaction);
            var findRemainData = f010301repo.Filter(x => x.DC_CODE == f010301s.DC_CODE && x.ALL_ID == f010301s.ALL_ID && x.SHIP_ORD_NO == f010301s.SHIP_ORD_NO);
            //如果都沒資料的話就把對應的F010302單號資料刪除
            if (!findRemainData.Any())
                f010302repo.Delete(x => x.DC_CODE == f010301s.DC_CODE && x.ALL_ID == f010301s.ALL_ID && x.SHIP_ORD_NO == f010301s.SHIP_ORD_NO);
            else
            {
                var findF010302 = f010302repo.Find(x => x.DC_CODE == f010301s.DC_CODE && x.ALL_ID == f010301s.ALL_ID && x.SHIP_ORD_NO == f010301s.SHIP_ORD_NO);
                if (findF010302 == null)
                    return true;
                findF010302.CHECK_BOX_CNT = (short)findRemainData.Sum(x => x.BOX_CNT);

                f010302repo.UpdateFields(
                      new { CHECK_BOX_CNT = findRemainData.Sum(x => x.BOX_CNT) },
                      x => x.DC_CODE == f010301s.DC_CODE && x.ALL_ID == f010301s.ALL_ID && x.SHIP_ORD_NO == f010301s.SHIP_ORD_NO);

                //更新f010301&f010302的
                var CheckStatus = GetCheckStatus(findF010302);
                f010301repo.UpdateFields(
                      new { CHECK_STATUS = CheckStatus },
                      x => x.DC_CODE == f010301s.DC_CODE && x.ALL_ID == f010301s.ALL_ID && x.SHIP_ORD_NO == f010301s.SHIP_ORD_NO);
                f010302repo.UpdateFields(
                      new { CHECK_STATUS = CheckStatus },
                      x => x.DC_CODE == f010301s.DC_CODE && x.ALL_ID == f010301s.ALL_ID && x.SHIP_ORD_NO == f010301s.SHIP_ORD_NO);

            }
            return true;
        }

        /// <summary>
        /// 依據傳入的F010301更新F010302的CheckBoxCnt、CHECK_STATUS
        /// </summary>
        /// <param name="f010301s"></param>
        /// <returns></returns>
        private bool UpdateF010302CheckBoxCnt(F010301[] f010301s)
        {
            bool execResult = true;
            var groupf010301 = f010301s.GroupBy(x => new { x.DC_CODE, x.ALL_ID, x.SHIP_ORD_NO, x.RECV_DATE })
                .Select(x => new F010301() { DC_CODE = x.Key.DC_CODE, ALL_ID = x.Key.ALL_ID, SHIP_ORD_NO = x.Key.SHIP_ORD_NO, RECV_DATE = x.Key.RECV_DATE });
            foreach (var item in groupf010301)
            {
                execResult &= UpdateF010302CheckBoxCnt(item);
                if (!execResult)
                    return false;
            }
            return true;
        }

    }
}
