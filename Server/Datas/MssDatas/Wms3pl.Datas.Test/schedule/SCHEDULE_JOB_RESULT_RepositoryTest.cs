using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F70;
using System;
using System.Collections.Generic;
using Wms3pl.Datas.Schedule;

namespace Wms3pl.Datas.Test.F70
{
    [TestClass]
    public class SCHEDULE_JOB_RESULT_RepositoryTest : BaseRepositoryTest
    {
        private SCHEDULE_JOB_RESULTRepository _SCHEDULE_JOB_RESULT_Repo;
        public SCHEDULE_JOB_RESULT_RepositoryTest()
        {
            _SCHEDULE_JOB_RESULT_Repo = new SCHEDULE_JOB_RESULTRepository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void InsertLog()
        {
            var r = _SCHEDULE_JOB_RESULT_Repo.InsertLog("001", "01", "010001", "schName","1","message");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void UpdateIsSuccess()
        {
            _SCHEDULE_JOB_RESULT_Repo.UpdateIsSuccess(1, "1","message");
            //Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
