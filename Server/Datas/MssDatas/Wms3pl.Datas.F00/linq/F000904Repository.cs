using Microsoft.EntityFrameworkCore;
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
    public partial class F000904Repository : RepositoryBase<F000904, Wms3plDbContext, F000904Repository>
    {
        public F000904Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IEnumerable<F000904Data> GetF000904Data(string topic, string subTopic, string isUsage = "1")
        {

            return _db.VW_F000904_LANGs.Where(x => x.TOPIC == topic &&
                                                                x.SUBTOPIC == subTopic &&
                                                                x.ISUSAGE == isUsage &&
                                                                x.LANG == Current.Lang)
                                                    .Select(x => new F000904Data
                                                    {
                                                        VALUE = x.VALUE,
                                                        NAME = x.NAME
                                                    })
                                                    .OrderBy(x => x.VALUE)
                                                    .AsEnumerable();
        }

        public IQueryable<P710601LangData> GetLangDatas(string topic, string subtopic, string lang)
        {
            var query =
                from a in _db.F000904s
                join b in _db.F000904_I18N
                on new { a.TOPIC, a.SUBTOPIC, a.VALUE } equals new { b.TOPIC, b.SUBTOPIC, b.VALUE } into ab
                from c in ab.DefaultIfEmpty()
                where c.LANG == lang
                select new { a, c };
            if (!string.IsNullOrWhiteSpace(topic))
                query = query.Where(x => x.a.TOPIC == topic);
            if (!string.IsNullOrWhiteSpace(subtopic))
                query = query.Where(x => x.a.SUBTOPIC == subtopic);
            return query.Select(x => new P710601LangData()
            {
                TOPIC = x.a.TOPIC,
                SUBTOPIC = x.a.SUBTOPIC,
                SUB_NAME = x.a.SUB_NAME,
                VALUE = x.a.VALUE,
                NAME = x.a.NAME,
                LANGNAME = x.c.NAME,
                LANG = (x.c.LANG ?? lang)
            });
        }

        public IQueryable<F000904Data> GetAGVStations(string isUsage = "1")
        {
            IQueryable<F000904Data> result;
            var query = _db.VW_F000904_LANGs
                            .Where(x => x.TOPIC == "P081201")
                            .Where(x => x.SUBTOPIC == "Workstation")
                            .Where(x => x.ISUSAGE == isUsage)
                            .Where(x => x.LANG == Current.Lang)
                            .Select(x =>
                                new
                                {
                                    VALUE = x.VALUE,
                                    NAME = x.NAME,
                                    SUBSTRNAME =
                                        x.NAME.Substring(1, 1)
                                        .Replace("一", "1")
                                        .Replace("二", "2")
                                        .Replace("三", "3")
                                        .Replace("四", "4")
                                        .Replace("五", "5")
                                        .Replace("六", "6")
                                        .Replace("七", "7")
                                        .Replace("八", "8")
                                        .Replace("九", "9")
                                })
                             .OrderBy(x => x.SUBSTRNAME);
            return result = query.Select(x => new F000904Data()
                                {
                                    VALUE = x.VALUE,
                                    NAME = x.NAME
                                });
        }

        /// <summary>
        /// 取得狀態名稱
        /// </summary>
        /// <param name="topic">程式編號(資料表)</param>
        /// <param name="subTopic">選單ID</param>
        /// <param name="value">參數值</param>
        /// <returns></returns>
        public string GetTopicValueName(string topic, string subTopic, string value)
        {
            var result = _db.F000904s.AsNoTracking().Where(x => x.TOPIC == topic
                                                        && x.SUBTOPIC == subTopic
                                                        && x.VALUE == value)
                                                    .Select(x=>x.NAME).SingleOrDefault();

            return result;
        }

        public IQueryable<F000904> GetDatasBySubTopic(string topic, string subTopic, List<string> values)
        {
            return _db.F000904s.Where(x => x.TOPIC == topic &&
                                           x.SUBTOPIC == subTopic &&
                                           values.Contains(x.VALUE));
        }

		/// <summary>
		/// 取得狀態名稱
		/// </summary>
		/// <param name="topic">程式編號(資料表)</param>
		/// <param name="subTopic">選單ID</param>
		/// <param name="value">參數值</param>
		/// <returns></returns>
		public string GetTopicValueNameByVW(string topic, string subTopic, string value)
		{
			var result = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == topic
																													&& x.SUBTOPIC == subTopic
																													&& x.VALUE == value
																													&& x.LANG == Current.Lang)
																							.Select(x => x.NAME).SingleOrDefault();

			return result;
		}
	}
}

