using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0070Repository : RepositoryBase<F0070, Wms3plDbContext, F0070Repository>
    {
        public F0070Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
        //public void RemoveAllByGroupNameAndUserName(string groupName, string userName)
        //{
        //    base.Delete(x => x.GROUPNAME == groupName && x.USERNAME == userName);
        //}

        public F0070 Get(string groupName, string connectId)
        {
            return _db.F0070s.Where(x => x.GROUPNAME == groupName
                            && x.CONNECTID == connectId)
                            .Select(x=>new F0070 {
                                CONNECTID = x.CONNECTID,
                                CRT_DATE = x.CRT_DATE,
                                CRT_NAME = x.CRT_NAME,
                                CRT_STAFF = x.CRT_STAFF,
                                GROUPNAME = x.GROUPNAME,
                                HOSTNAME = x.HOSTNAME,
                                UNLOCKTIME = x.UNLOCKTIME,
                                UPD_DATE = x.UPD_DATE,
                                UPD_NAME = x.UPD_NAME,
                                UPD_STAFF = x.UPD_STAFF,
                                USERNAME = x.USERNAME,
                            }).FirstOrDefault();
        }

        public IQueryable<F0070> FindNameByExcludeCurrentConnectId(string groupName, string userName, string excludeConnectId)
        {
            var query = _db.F0070s.Where(x => x.GROUPNAME == groupName && x.USERNAME == userName);
            if (excludeConnectId != null)
                query = query.Where(x => x.CONNECTID != excludeConnectId);
            return query.Select(x => new F0070(){
                CONNECTID = x.CONNECTID,
                USERNAME = x.USERNAME,
                HOSTNAME = x.HOSTNAME,
                UNLOCKTIME = x.UNLOCKTIME,
                GROUPNAME = x.GROUPNAME,
                CRT_DATE = x.CRT_DATE,
                CRT_STAFF = x.CRT_STAFF,
                CRT_NAME = x.CRT_NAME,
                UPD_DATE = x.UPD_DATE,
                UPD_STAFF = x.UPD_STAFF,
                UPD_NAME = x.UPD_NAME
            });
        }
        public void UpdateUnLockDate(string groupName, string connectId, DateTime? date)
        {
            var f0070 = _db.F0070s.Where(x => x.GROUPNAME == groupName && x.CONNECTID == connectId).FirstOrDefault();
            f0070.UNLOCKTIME = date;
            _db.F0070s.Update(f0070);
            _db.SaveChanges();
        }
        public IQueryable<F0070> GetAllByGroup(string groupName)
        {
            return _db.F0070s.Where(x => x.GROUPNAME == groupName)
                .Select(x => new F0070()
                {
                    CONNECTID = x.CONNECTID,
                    USERNAME = x.USERNAME,
                    HOSTNAME = x.HOSTNAME,
                    UNLOCKTIME = x.UNLOCKTIME,
                    GROUPNAME = x.GROUPNAME,
                    CRT_DATE = x.CRT_DATE,
                    CRT_STAFF = x.CRT_STAFF,
                    CRT_NAME = x.CRT_NAME,
                    UPD_DATE = x.UPD_DATE,
                    UPD_STAFF = x.UPD_STAFF,
                    UPD_NAME = x.UPD_NAME
                });
        }
        /// <summary>
        /// 取得登入紀錄
        /// </summary>
        /// <returns></returns>
        public IQueryable<F0070LoginData> GetLoninData()
        {
            return _db.F0070s.Where(x => x.USERNAME != "wms")
                .Select(x => new F0070LoginData()
                {
                    CONNECTID = x.CONNECTID,
                    USERNAME = x.USERNAME,
                    HOSTNAME = x.HOSTNAME,
                    UNLOCKTIME = x.UNLOCKTIME,
                    CRT_DATE = x.CRT_DATE
                })
                .OrderByDescending(x => x.CRT_DATE);
        }

        /// <summary>
        /// 檢核帳號是否已登入在其他裝置
        /// </summary>
        /// <param name="accNo">帳號</param>
        /// <returns></returns>
        public bool CheckLoginLog(string accNo,string mcCode)
        {
            // var isAllowMultiLogin = 取得WebConfig的AppSetting
            // key = IsAllowSameAccountMultiLogin 值
            //  如果isAllowMultiLogin = 1，直接回傳false
            //   否則
            // SELECT F0070
            // WHERE USERNAME =< 參數1 > AND UNLOCKDATE > 系統時間
            // 如果有找到則回傳true 否則回傳false

            var isAllowMultiLogin = ConfigurationManager.AppSettings["IsAllowSameAccountMultiLogin"].ToString();

            if (isAllowMultiLogin.Equals("1"))
            {
                return false;
            }
            else
            {
                var parm = new List<SqlParameter>();

                parm.Add(new SqlParameter(":p0", accNo));

                var result = _db.F0070s.AsNoTracking().Where(x =>x.GROUPNAME == "pda" && 
                                                             x.CONNECTID != mcCode && 
                                                             x.USERNAME == accNo && 
                                                             x.UNLOCKTIME > DateTime.Now).Count();

                return result > 0;
            }
        }
    }
}

