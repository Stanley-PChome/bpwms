using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
    public partial class P192019Service
    {
        private WmsTransaction _wmsTransaction;
        public P192019Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        public IQueryable<P192019Item> GetP192019SearchData(string gupCode, string custCode, string clsCode, string clsName, string clsType)
        {
            var results = new List<P192019Item>();
            switch (clsType.ToUpper())
            {
                case "A":
                    var repA = new F1915Repository(Schemas.CoreSchema);
                    results = repA.GetF1915SearchData(gupCode, custCode, clsCode, clsName);
                    break;
                case "B":
                    var repB = new F1916Repository(Schemas.CoreSchema);
                    results = repB.GetF1916SearchData(gupCode, custCode, clsCode, clsName);
                    break;
                case "C":
                    var repC = new F1917Repository(Schemas.CoreSchema);
                    results = repC.GetF1917SearchData(gupCode, custCode, clsCode, clsName);
                    break;
            }

            return results.AsQueryable();
        }
        public ExecuteResult DeleteCls(P192019Data data)
        {
            var f1909Repo = new F1909Repository(Schemas.CoreSchema);

            var custData = f1909Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == data.GupCode && x.CUST_CODE == data.CustCode).FirstOrDefault();
            if (custData == null)
                return new ExecuteResult(false, Properties.Resources.P192019Service_Cust_CodeNotFound);
            var isExist = CheckIsExits(data);
            if (!isExist)
                return new ExecuteResult(false, string.Format(Properties.Resources.P192019Service_Deleted, GetTypeName(data.ClaType), GetTypeValue(data, data.ClaType)));
            var otherCustItemCategoryShareList = new List<F1909>();
            if (custData.ALLOWGUP_ITEMCATEGORYSHARE == "1")
                otherCustItemCategoryShareList = GetOtherCustItemCategorySharedList(data.GupCode, data.CustCode);
            otherCustItemCategoryShareList.Add(custData);

            if (otherCustItemCategoryShareList.Any())
            {
                var f1903Repo = new F1903Repository(Schemas.CoreSchema);
                var f1915Repo = new F1915Repository(Schemas.CoreSchema, _wmsTransaction);
                var f1916Repo = new F1916Repository(Schemas.CoreSchema, _wmsTransaction);
                var f1917Repo = new F1917Repository(Schemas.CoreSchema, _wmsTransaction);

                switch (data.ClaType.ToUpper())
                {
                    case "A":
                        if (f1903Repo.IsExits(data.GupCode, data.CustCode, data.ACode))
                            return new ExecuteResult(false, string.Format(Properties.Resources.P192019Service_ItemSetExist, GetTypeName(data.ClaType), GetTypeValue(data, data.ClaType)));

                        f1915Repo.DeleteByCustCodes(data.GupCode, data.ACode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());
                        f1916Repo.DeleteACodeByCustCodes(data.GupCode, data.ACode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());
                        f1917Repo.DeleteACodeByCustCodes(data.GupCode, data.ACode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());
                        break;
                    case "B":
                        if (f1903Repo.IsExits(data.GupCode, data.CustCode, data.ACode, data.BCode))
                            return new ExecuteResult(false, string.Format(Properties.Resources.P192019Service_ItemSetExist, GetTypeName(data.ClaType), GetTypeValue(data, data.ClaType)));

                        f1916Repo.DeleteByCustCodes(data.GupCode, data.ACode, data.BCode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());
                        f1917Repo.DeleteABCodeByCustCodes(data.GupCode, data.ACode, data.BCode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());
                        break;
                    case "C":
                        if (f1903Repo.IsExits(data.GupCode, data.CustCode, data.ACode, data.BCode, data.CCode))
                            return new ExecuteResult(false, string.Format(Properties.Resources.P192019Service_ItemSetExist, GetTypeName(data.ClaType), GetTypeValue(data, data.ClaType)));

                        f1917Repo.DeleteByCustCodes(data.GupCode, data.ACode, data.BCode, data.CCode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());
                        break;
                }
            }
            return new ExecuteResult(true);
        }

        public ExecuteResult InsertOrUpdateCls(P192019Data data, bool isAdd)
        {
            var addDatas = new List<object>();
            var addDatasF1915 = new List<object>();
            var addDatasF1916 = new List<object>();
            var modifyDatas = new List<object>();
            var modifyDatasF1915 = new List<object>();
            var modifyDatasF1916 = new List<object>();
            var f1909Repo = new F1909Repository(Schemas.CoreSchema);

            var custData = f1909Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == data.GupCode && x.CUST_CODE == data.CustCode).FirstOrDefault();
            if (custData == null)
                return new ExecuteResult(false, Properties.Resources.P192019Service_Cust_CodeNotFound);

            var isExist = CheckIsExits(data);

            if (isAdd && isExist)
                return new ExecuteResult(false, string.Format(Properties.Resources.P192019Service_Exist_CannotInsert, GetTypeName(data.ClaType), GetTypeValue(data, data.ClaType)));

            if (!isAdd && !isExist)
                return new ExecuteResult(false, string.Format(Properties.Resources.P192019Service_Deleted_CannotModify, GetTypeName(data.ClaType), GetTypeValue(data, data.ClaType)));

            var otherCustItemCategoryShareList = new List<F1909>();
            if (custData.ALLOWGUP_ITEMCATEGORYSHARE == "1")
                otherCustItemCategoryShareList = GetOtherCustItemCategorySharedList(data.GupCode, data.CustCode);
            otherCustItemCategoryShareList.Add(custData);
            var f1915Repo = new F1915Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1916Repo = new F1916Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1917Repo = new F1917Repository(Schemas.CoreSchema, _wmsTransaction);

            switch (data.ClaType.ToUpper())
            {
                case "A":
                    var updateDatasA = f1915Repo.AsForUpdate().GetDatas(data.GupCode, data.ACode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());
                    foreach (var item in otherCustItemCategoryShareList)
                    {
                        var updateItem = updateDatasA.FirstOrDefault(x => x.CUST_CODE == item.CUST_CODE);
                        if (updateItem == null)
                            CreateCls(data, item.CUST_CODE, ref addDatas);
                        else
                            UpdateCls(data, updateItem, ref modifyDatas);
                    }
                    break;
                case "B":
                    var listCustCodeB = otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList();
                    listCustCodeB.Add(data.CustCode);
                    var updateDatasF1915B = f1915Repo.AsForUpdate().GetDatas(data.GupCode, data.ACode, listCustCodeB);
                    var updateDatasB = f1916Repo.AsForUpdate().GetDatas(data.GupCode, data.ACode, data.BCode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());
                    foreach (var item in otherCustItemCategoryShareList)
                    {
                        var updateItem = updateDatasB.FirstOrDefault(x => x.CUST_CODE == item.CUST_CODE);
                        if (updateItem == null)
                            CreateCls(data, item.CUST_CODE, ref addDatas);
                        else
                            UpdateCls(data, updateItem, ref modifyDatas);

                        var backupCalType = data.ClaType;
                        var backupCalName = data.ClaName;
                        var backupCheckPercent = data.CheckPercent;
                        var nowCustF1915B = updateDatasF1915B.First(x => x.CUST_CODE == data.CustCode);
                        data.ClaType = "A";
                        data.ClaName = nowCustF1915B.CLA_NAME;
                        data.CheckPercent = nowCustF1915B.CHECK_PERCENT;
                        var updateItemF1915 = updateDatasF1915B.FirstOrDefault(x => x.CUST_CODE == item.CUST_CODE);
                        if (updateItemF1915 == null)
                            CreateCls(data, item.CUST_CODE, ref addDatasF1915);
                        else
                            UpdateCls(data, updateItemF1915, ref modifyDatasF1915);

                        data.ClaType = backupCalType;
                        data.ClaName = backupCalName;
                        data.CheckPercent = backupCheckPercent;
                    }
                    break;
                case "C":
                    var listCustCodeC = otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList();
                    listCustCodeC.Add(data.CustCode);

                    var updateDatasF1916C = f1916Repo.AsForUpdate().GetDatas(data.GupCode, data.ACode, data.BCode, listCustCodeC);
                    var updateDatasF1915C = f1915Repo.AsForUpdate().GetDatas(data.GupCode, data.ACode, listCustCodeC);
                    var updateDatasC = f1917Repo.AsForUpdate().GetDatas(data.GupCode, data.ACode, data.BCode, data.CCode, otherCustItemCategoryShareList.Select(x => x.CUST_CODE).ToList());

                    foreach (var item in otherCustItemCategoryShareList)
                    {
                        var updateItem = updateDatasC.FirstOrDefault(x => x.CUST_CODE == item.CUST_CODE);
                        if (updateItem == null)
                            CreateCls(data, item.CUST_CODE, ref addDatas);
                        else
                            UpdateCls(data, updateItem, ref modifyDatas);

                        var backupCalType = data.ClaType;
                        var backupCalName = data.ClaName;
                        var backupCheckPercent = data.CheckPercent;

                        var nowCustF1916C = updateDatasF1916C.First(x => x.CUST_CODE == data.CustCode);
                        data.ClaType = "B";
                        data.ClaName = nowCustF1916C.CLA_NAME;
                        data.CheckPercent = nowCustF1916C.CHECK_PERCENT;
                        var updateItemF1916 = updateDatasF1916C.FirstOrDefault(x => x.CUST_CODE == item.CUST_CODE);
                        if (updateItemF1916 == null)
                            CreateCls(data, item.CUST_CODE, ref addDatasF1916);
                        else
                            UpdateCls(data, updateItemF1916, ref modifyDatasF1916);

                        var nowCustF1915C = updateDatasF1915C.First(x => x.CUST_CODE == data.CustCode);
                        data.ClaType = "A";
                        data.ClaName = nowCustF1915C.CLA_NAME;
                        data.CheckPercent = nowCustF1915C.CHECK_PERCENT;
                        var updateItemF1915 = updateDatasF1915C.FirstOrDefault(x => x.CUST_CODE == item.CUST_CODE);
                        if (updateItemF1915 == null)
                            CreateCls(data, item.CUST_CODE, ref addDatasF1915);
                        else
                            UpdateCls(data, updateItemF1915, ref modifyDatasF1915);

                        data.ClaType = backupCalType;
                        data.ClaName = backupCalName;
                        data.CheckPercent = backupCheckPercent;
                    }
                    break;
            }
            BulkInsertOrUpdate(addDatas, data.ClaType, true);
            BulkInsertOrUpdate(modifyDatas, data.ClaType, false);
            BulkInsertOrUpdate(addDatasF1915, "A", true);
            BulkInsertOrUpdate(modifyDatasF1915, "A", false);
            BulkInsertOrUpdate(addDatasF1916, "B", true);
            BulkInsertOrUpdate(modifyDatasF1916, "B", false);            
            return new ExecuteResult(true);
        }
        private string GetTypeName(string claType)
        {
            switch (claType.ToUpper())
            {
                case "A":
                    return Properties.Resources.P190102Service_LType;
                case "B":
                    return Properties.Resources.P190102Service_MType;
                case "C":
                    return Properties.Resources.P190102Service_SType;
            }
            return string.Empty;
        }
        private string GetTypeValue(P192019Data data, string claType)
        {
            switch (claType.ToUpper())
            {
                case "A":
                    return data.ACode;
                case "B":
                    return data.BCode;
                case "C":
                    return data.CCode;
            }
            return string.Empty;
        }
        private bool CheckIsExits(P192019Data data)
        {
            switch (data.ClaType.ToUpper())
            {
                case "A":
                    var f1915Repo = new F1915Repository(Schemas.CoreSchema, _wmsTransaction);
                    return f1915Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == data.GupCode && x.CUST_CODE == data.CustCode && x.ACODE == data.ACode).Any();
                case "B":
                    var f1916Repo = new F1916Repository(Schemas.CoreSchema, _wmsTransaction);
                    return f1916Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == data.GupCode && x.CUST_CODE == data.CustCode && x.ACODE == data.ACode && x.BCODE == data.BCode).Any();
                case "C":
                    var f1917Repo = new F1917Repository(Schemas.CoreSchema, _wmsTransaction);
                    return f1917Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == data.GupCode && x.CUST_CODE == data.CustCode && x.ACODE == data.ACode && x.BCODE == data.BCode && x.CCODE == data.CCode).Any();
            }
            return false;
        }

        private void BulkInsertOrUpdate(List<object> datas, string claType, bool isAdd)
        {
            if (datas.Any())
            {
                switch (claType.ToUpper())
                {
                    case "A":
                        var f1915Repo = new F1915Repository(Schemas.CoreSchema, _wmsTransaction);
                        if (isAdd)
                            f1915Repo.BulkInsert(datas.Select(x => (F1915)x));
                        else
                            f1915Repo.BulkUpdate(datas.Select(x => (F1915)x));
                        break;
                    case "B":
                        var f1916Repo = new F1916Repository(Schemas.CoreSchema, _wmsTransaction);
                        if (isAdd)
                            f1916Repo.BulkInsert(datas.Select(x => (F1916)x));
                        else
                            f1916Repo.BulkUpdate(datas.Select(x => (F1916)x));
                        break;
                    case "C":
                        var f1917Repo = new F1917Repository(Schemas.CoreSchema, _wmsTransaction);
                        if (isAdd)
                            f1917Repo.BulkInsert(datas.Select(x => (F1917)x));
                        else
                            f1917Repo.BulkUpdate(datas.Select(x => (F1917)x));
                        break;
                }
            }
        }

        private void CreateCls(P192019Data data, string custCode, ref List<object> addDatas)
        {
            switch (data.ClaType.ToUpper())
            {
                case "A":
                    addDatas.Add(new F1915
                    {
                        ACODE = data.ACode,
                        CLA_NAME = data.ClaName,
                        CHECK_PERCENT = data.CheckPercent,
                        GUP_CODE = data.GupCode,
                        CUST_CODE = custCode
                    });
                    break;
                case "B":
                    addDatas.Add(new F1916
                    {
                        ACODE = data.ACode,
                        BCODE = data.BCode,
                        CLA_NAME = data.ClaName,
                        CHECK_PERCENT = data.CheckPercent,
                        GUP_CODE = data.GupCode,
                        CUST_CODE = custCode
                    });
                    break;
                case "C":
                    addDatas.Add(new F1917
                    {
                        ACODE = data.ACode,
                        BCODE = data.BCode,
                        CCODE = data.CCode,
                        CLA_NAME = data.ClaName,
                        CHECK_PERCENT = data.CheckPercent,
                        GUP_CODE = data.GupCode,
                        CUST_CODE = custCode
                    });
                    break;
            }
        }

        private void UpdateCls(P192019Data data, object oldData, ref List<object> modifyDatas)
        {
            switch (data.ClaType.ToUpper())
            {
                case "A":
                    var updateData = oldData as F1915;
                    updateData.CLA_NAME = data.ClaName;
                    updateData.CHECK_PERCENT = data.CheckPercent;
                    modifyDatas.Add(updateData);
                    break;
                case "B":
                    var updateData1 = oldData as F1916;
                    updateData1.CLA_NAME = data.ClaName;
                    updateData1.CHECK_PERCENT = data.CheckPercent;
                    modifyDatas.Add(updateData1);
                    break;
                case "C":
                    var updateData2 = oldData as F1917;
                    updateData2.CLA_NAME = data.ClaName;
                    updateData2.CHECK_PERCENT = data.CheckPercent;
                    modifyDatas.Add(updateData2);
                    break;
            }
        }

        private List<F1909> GetOtherCustItemCategorySharedList(string gupCode, string custCode)
        {
            var f1909Repo = new F1909Repository(Schemas.CoreSchema);
            return f1909Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.ALLOWGUP_ITEMCATEGORYSHARE == "1").Where(x => x.CUST_CODE != custCode).ToList();
        }
    }
}

