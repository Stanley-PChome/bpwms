using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
    public partial class P191206Service
    {
        private WmsTransaction _wmsTransaction;
        public P191206Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        /// <summary>
        /// 寫入PK區相關資料至資料庫，F19120601、F19120602的DC_CODE＆PK_AREA會用f191206的
        /// </summary>
        /// <param name="front_f191206"></param>
        /// <param name="front_f19120601s"></param>
        /// <param name="front_f19120602s"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public ExecuteResult InsertOrUpdateF191206(F191206 front_f191206, F19120601[] front_f19120601s, F19120602[] front_f19120602s, bool isAdd)
        {
            //↓此回傳訊息如有調整需要連帶調整Wms3pl.WpfClient.P19的resource P1901810000_Code_Exist此設定維持一致性
            String P1901810000_Code_Exist = "該儲位前5碼\r\n{0}\r\n已經存在";
            string strLocCodeExistList;
            int? LastLINE_SEQ = null;

            var f1912repo = new F1912Repository(Schemas.CoreSchema);
            var LocCodeList = new List<string>();


            var f191206repo = new F191206Repository(Schemas.CoreSchema, _wmsTransaction);
            var f19120601repo = new F19120601Repository(Schemas.CoreSchema, _wmsTransaction);
            var f19120602repo = new F19120602Repository(Schemas.CoreSchema, _wmsTransaction);

            var dbF19120601s = f19120601repo.Filter(x => x.DC_CODE == front_f191206.DC_CODE).ToList();
            var dbF19120602s = f19120602repo.Filter(x => x.DC_CODE == front_f191206.DC_CODE).ToList();

            String tmpBEGIN_LOC_CODE, tmpEND_LOC_CODE;//自動將頭、尾碼轉換成小到大用

            //避免前端丟進來的資料有問題，強制把DC_CODE＆PK_AREA都用f191206他的資料
            foreach (var item in front_f19120601s)
            {
                item.DC_CODE = front_f191206.DC_CODE;
                item.PK_AREA = front_f191206.PK_AREA;
            }
            foreach (var item in front_f19120602s)
            {
                item.DC_CODE = front_f191206.DC_CODE;
                item.PK_AREA = front_f191206.PK_AREA;
                item.PICK_FLOOR = front_f191206.PICK_FLOOR;
            }

            if (isAdd)
            {
                //處理新增模式
                if (f191206repo.CheckF191206Duplicate(front_f191206))
                    return new ExecuteResult(false, "PK區編號已經存在");
                f191206repo.Add(front_f191206);

                for (int i = 0; i < front_f19120601s.Count(); i++)
                {
                    var check_dbF19120601sExist = CheckF19120601Duplicate(dbF19120601s, front_f191206.DC_CODE, front_f19120601s[i].BEGIN_LOC_CODE, front_f19120601s[i].END_LOC_CODE);
                    if (check_dbF19120601sExist.Any())
                    {
                        strLocCodeExistList = string.Join("\r\n", check_dbF19120601sExist);
                        return new ExecuteResult(false, String.Format(P1901810000_Code_Exist, strLocCodeExistList));
                    }

                    front_f19120601s[i].LINE_SEQ = i + 1;
                    f19120601repo.Add(front_f19120601s[i]);
                }

                var check_dbF19120602sExist = CheckF19120602Duplicate(dbF19120602s, front_f19120602s.ToList(), front_f191206.DC_CODE);
                if (check_dbF19120602sExist.Any())
                {
                    strLocCodeExistList = string.Join("\r\n", check_dbF19120602sExist);
                    return new ExecuteResult(false, String.Format(P1901810000_Code_Exist, strLocCodeExistList));
                }

                foreach (var item in front_f19120602s)
                    f19120602repo.Add(item);

            }
            else
            {
                //處理編輯模式
                var chkF191206Exist = f191206repo.Find(x => x.DC_CODE == front_f191206.DC_CODE && x.PK_AREA == front_f191206.PK_AREA);
                //前端User只能修改這兩個東西
                if (chkF191206Exist.PICK_FLOOR != front_f191206.PICK_FLOOR ||
                    chkF191206Exist.PK_NAME != front_f191206.PK_NAME)
                {
                    //下方的資料處理僅比對儲位是否存在，因此這邊要檢查樓層異動
                    if (chkF191206Exist.PICK_FLOOR != front_f191206.PICK_FLOOR)
                        foreach (var front02_item in front_f19120602s)
                            f19120602repo.Update(front02_item);

                    chkF191206Exist.PICK_FLOOR = front_f191206.PICK_FLOOR;
                    chkF191206Exist.PK_NAME = front_f191206.PK_NAME;
                    f191206repo.Update(chkF191206Exist);
                }

                //檢查是否有資料庫有，但前端傳進來的沒有，代表要刪除該資料
                foreach (var dbF19120601Item in dbF19120601s.Where(x => x.PK_AREA == front_f191206.PK_AREA).ToList())
                {
                    if (!front_f19120601s.Any(x => x.DC_CODE == front_f191206.DC_CODE
                         && x.PK_AREA == front_f191206.PK_AREA
                         && x.BEGIN_LOC_CODE == dbF19120601Item.BEGIN_LOC_CODE
                         && x.END_LOC_CODE == dbF19120601Item.END_LOC_CODE))
                    {
                        f19120601repo.Delete(dbF19120601Item);
                        dbF19120601s.Remove(dbF19120601Item);

                        ReverseLocCode(dbF19120601Item.BEGIN_LOC_CODE, dbF19120601Item.END_LOC_CODE, out tmpBEGIN_LOC_CODE, out tmpEND_LOC_CODE);

                        var dbf19120602_01 = dbF19120602s.Where(x => x.DC_CODE == front_f191206.DC_CODE
                                            && x.PK_AREA == front_f191206.PK_AREA
                                            && x.CHK_LOC_CODE.CompareTo(tmpBEGIN_LOC_CODE) >= 0
                                            && x.CHK_LOC_CODE.CompareTo(tmpEND_LOC_CODE) <= 0).ToList();
                        foreach (var db02_item in dbf19120602_01)
                        {
                            f19120602repo.Delete(db02_item);
                            dbF19120602s.Remove(db02_item);

                        }
                    }
                }
                //檢查是否有前端有，資料庫沒有，代表新增資料
                foreach (var front01_item in front_f19120601s)
                {
                    if (!dbF19120601s.Any(x => x.DC_CODE == front_f191206.DC_CODE
                         && x.PK_AREA == front_f191206.PK_AREA
                         && x.BEGIN_LOC_CODE == front01_item.BEGIN_LOC_CODE
                         && x.END_LOC_CODE == front01_item.END_LOC_CODE))
                    {

                        var check_dbF19120601sExist = CheckF19120601Duplicate(dbF19120601s, front_f191206.DC_CODE, front01_item.BEGIN_LOC_CODE, front01_item.END_LOC_CODE);
                        if (check_dbF19120601sExist.Any())
                        {
                            strLocCodeExistList = string.Join("\r\n", check_dbF19120601sExist);
                            return new ExecuteResult(false, String.Format(P1901810000_Code_Exist, strLocCodeExistList));
                        }

                        if (!LastLINE_SEQ.HasValue)
                        {
                            var tmpResult = dbF19120601s.Where(x => x.DC_CODE == front_f191206.DC_CODE && x.PK_AREA == front_f191206.PK_AREA);
                            LastLINE_SEQ = tmpResult.Any() ? tmpResult.Max(x => x.LINE_SEQ) : 0;
                        }
                        front01_item.LINE_SEQ = LastLINE_SEQ.Value + 1;
                        LastLINE_SEQ++;
                        f19120601repo.Add(front01_item);

                        ReverseLocCode(front01_item.BEGIN_LOC_CODE, front01_item.END_LOC_CODE, out tmpBEGIN_LOC_CODE, out tmpEND_LOC_CODE);

                        var frontf19120602_01 = front_f19120602s.Where(x => x.DC_CODE == front01_item.DC_CODE
                                                                  && x.CHK_LOC_CODE.CompareTo(tmpBEGIN_LOC_CODE) >= 0
                                                                  && x.CHK_LOC_CODE.CompareTo(tmpEND_LOC_CODE) <= 0).ToList();

                        var check_dbF19120602sExist = CheckF19120602Duplicate(dbF19120602s, frontf19120602_01, front_f191206.DC_CODE);
                        if (check_dbF19120602sExist.Any())
                        {
                            strLocCodeExistList = string.Join("\r\n", check_dbF19120602sExist);
                            return new ExecuteResult(false, String.Format(P1901810000_Code_Exist, strLocCodeExistList));
                        }
                        foreach (var front02_item in frontf19120602_01)
                            f19120602repo.Add(front02_item);

                    }
                }

            }

            _wmsTransaction.Complete();
            return new ExecuteResult(true);
        }

        //檢查F19120601是否有重複，有就直接把全部的項目都列出
        private List<String> CheckF19120601Duplicate(List<F19120601> db_f19120601s, String front_DC_CODE, String front_BEGIN_LOC_CODE, String front_END_LOC_CODE)
        {
            var f1912repo = new F1912Repository(Schemas.CoreSchema);

            List<string> DuplicateLocCodeList = new List<string>();
            DuplicateLocCodeList.AddRange(f1912repo.GEtF1912LocCodeList(front_DC_CODE, front_BEGIN_LOC_CODE, front_END_LOC_CODE).ToList());

            var check_dbF19120601sExist = db_f19120601s.Where(x => x.DC_CODE == front_DC_CODE && x.BEGIN_LOC_CODE == front_BEGIN_LOC_CODE && x.END_LOC_CODE == front_END_LOC_CODE);
            if (check_dbF19120601sExist.Any())
                return DuplicateLocCodeList;

            return new List<string>();
        }

        private List<String> CheckF19120602Duplicate(List<F19120602> dbf19120602s, List<F19120602> front_f19120602s, String front_DC_CODE)
        {
            List<string> DuplicateLocCodeList = new List<string>();
            foreach (var front_item in front_f19120602s)
            {
                if (dbf19120602s.Any(x => x.DC_CODE == front_DC_CODE && x.CHK_LOC_CODE == front_item.CHK_LOC_CODE))
                    DuplicateLocCodeList.Add(front_item.CHK_LOC_CODE);
            }
            return DuplicateLocCodeList;
        }

        /// <summary>
        /// 判斷頭碼、尾碼是否有是反過來打的，是的話就把他反過來,前端也有用到此功能如果要改這邏輯記得改
        /// </summary>
        private void ReverseLocCode(String BeginCode, String EndCode, out String SmallBeginCode, out String LargeCode)
        {
            if (BeginCode.CompareTo(EndCode) <= 0)
            {
                SmallBeginCode = BeginCode;
                LargeCode = EndCode;
            }
            else
            {
                LargeCode = BeginCode;
                SmallBeginCode = EndCode;
            }
        }

    }
}
