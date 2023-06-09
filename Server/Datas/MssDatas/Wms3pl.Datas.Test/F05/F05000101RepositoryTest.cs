using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using System.Collections.Generic;
using System;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05000101RepositoryTest : BaseRepositoryTest
    {
        private F05000101Repository _f05000101Repo;
        public F05000101RepositoryTest()
        {
            _f05000101Repo = new F05000101Repository(Schemas.CoreSchema);
        }


        [TestMethod]
        public void BulkDelete()
        {
            #region Params
            var data = new List<F05000101>
            {
                new F05000101{ ID = 6003 },
                new F05000101{ ID = 6004 }
            };
            #endregion

            _f05000101Repo.BulkDelete(data);
        }

        [TestMethod]
        public void BulkInsertData()
        {
            #region Params
            var data = new List<F05000101>
            {
                new F05000101
                {
                    DC_CODE = "001",
                    GUP_CODE = "01",
                    CUST_CODE = "010002",
                    CHANNEL = "EHS",
                    FOREIGN_ORDNO = "E83160021",
                    ORDDATA = "" ,
                    STATUS = "7",
                    ERRMSG = ""
                },
                new F05000101
                {
                    DC_CODE = "001",
                    GUP_CODE = "01",
                    CUST_CODE = "010002",
                    CHANNEL = "EHS",
                    FOREIGN_ORDNO = "E83160021",
                    ORDDATA = "" ,
                    STATUS = "9",
                    ERRMSG = ""
                }
            };
            #endregion

            _f05000101Repo.BulkInsertData(data);
        }

        [TestMethod]
        public void BulkUpdateData()
        {
            #region Params
            var data = new List<F05000101>
            {
                new F05000101
                {
                    ID = 6006,
                    DC_CODE = "001",
                    GUP_CODE = "01",
                    CUST_CODE = "010001",
                    CHANNEL = "EHS",
                    FOREIGN_ORDNO = "E83160021",
                    ORDDATA = "" ,
                    STATUS = "9",
                    ERRMSG = "",
                    CRT_DATE = DateTime.Now,
                    CRT_STAFF = Current.Staff,
                    CRT_NAME = Current.StaffName
                },
                new F05000101
                {
                    ID = 6007,
                    DC_CODE = "001",
                    GUP_CODE = "01",
                    CUST_CODE = "010001",
                    CHANNEL = "EHS",
                    FOREIGN_ORDNO = "E83160021",
                    ORDDATA = "" ,
                    STATUS = "9",
                    ERRMSG = "",
                    CRT_DATE = DateTime.Now,
                    CRT_STAFF = Current.Staff,
                    CRT_NAME = Current.StaffName
                }
            };
            #endregion

            _f05000101Repo.BulkUpdateData(data);
        }
    }
}
