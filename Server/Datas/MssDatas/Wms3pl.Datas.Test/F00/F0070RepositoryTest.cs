
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F00;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F00
{
    [TestClass]
    public class F0070RepositoryTest : BaseRepositoryTest
    {

        private F0070Repository _f0070Repo;

        public F0070RepositoryTest()
        {
            _f0070Repo = new F0070Repository(Schemas.CoreSchema);
        }
        //private Wms3plDbContext _wms3plDbContext;

        //public F0070Repository(string connName, WmsTransaction wmsTransaction = null)
        //    : base(connName, wmsTransaction)
        //{
        //    //DbContextOptionsBuilder dbOptBuilder = new DbContextOptionsBuilder();
        //    //var dbOptions = dbOptBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
        //    //_wms3plDbContext = new Wms3plDbContext(dbOptions.Options);
        //}

        [TestMethod]
        public void RemoveAllByGroupNameAndUserName()
        {
            //string groupName = "";
            //string userName = "";
            //var parms = new List<SqlParameter>();
            //parms.Add(new SqlParameter(":p0", groupName));
            //parms.Add(new SqlParameter(":p1", userName));
            //var sql = @" DELETE 
            //         FROM F0070
            //        WHERE GROUPNAME = :p0
            //          AND USERNAME = :p1 ";
            //ExecuteSqlCommand(sql, parms.ToArray());
        }

        [TestMethod]
        public void InsertLoginLog()
        {
            #region Params
            var mcCode = "A12345";
            var accNo = "A12345";
            var devCode = "A12345";
            #endregion

          _f0070Repo.InsertLoginLog(mcCode, accNo, devCode);
        }

        [TestMethod]
        public void DeleteLoginLog()
        {
            #region Params
            var accNo = "A12345";
            var devCode = "A12345";
            #endregion

            _f0070Repo.DeleteLoginLog(accNo, devCode);
        }
    }
}
