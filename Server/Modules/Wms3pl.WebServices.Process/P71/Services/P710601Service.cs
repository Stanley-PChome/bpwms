using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using AutoMapper;
using Wms3pl.Datas.F00;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710601Service
	{
		private WmsTransaction _wmsTransaction;
		public P710601Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<P710601LangData> GetP710601LangData(string topic, string subtopic, string lang)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransaction);
			return f000904Repo.GetLangDatas(topic, subtopic, lang);
		}

		public ExecuteResult AddP710601Data(List<F000904> f000904s)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransaction);
			var f000904I18NRepo = new F000904_I18NRepository(Schemas.CoreSchema, _wmsTransaction);
			var f000904I18Ns = new List<F000904_I18N>();
			f000904s.ForEach(o =>
			{
				var f000904I18NData = new F000904_I18N
				{
					LANG = "zh-TW",
					NAME = o.NAME,
					SUBTOPIC = o.SUBTOPIC,
					SUB_NAME = o.SUB_NAME,
					TOPIC = o.TOPIC,
					VALUE = o.VALUE
				};
				f000904I18Ns.Add(f000904I18NData);
			});
			f000904Repo.BulkInsert(f000904s);
			f000904I18NRepo.BulkInsert(f000904I18Ns);
			return new ExecuteResult(true);
		}

		public ExecuteResult InsertOrUpdateLang(string topic, string subtopic, string lang, List<P710601LangData> data)
		{
			var f000904I18NRepo = new F000904_I18NRepository(Schemas.CoreSchema, _wmsTransaction);
			var nowData = f000904I18NRepo.AsForUpdate().GetDatasByTrueAndCondition(o => o.LANG == lang).ToList();
			if (!string.IsNullOrEmpty(topic))
				nowData = nowData.Where(o=>o.TOPIC == topic).ToList();
			if (!string.IsNullOrEmpty(subtopic))
				nowData = nowData.Where(o => o.SUBTOPIC == subtopic).ToList();

			var updateList = new List<F000904_I18N>();
			var insertList = new List<F000904_I18N>();
			foreach (var d in data)
			{
				var existData = nowData.FirstOrDefault(o => o.TOPIC == d.TOPIC && o.SUBTOPIC == d.SUBTOPIC && o.VALUE == d.VALUE);
				if (existData == null && !string.IsNullOrWhiteSpace(d.LANGNAME))
				{
					var newData = new F000904_I18N
					{
						LANG = d.LANG,
						NAME = d.LANGNAME,
						SUBTOPIC = d.SUBTOPIC,
						SUB_NAME = d.SUB_NAME,
						TOPIC = d.TOPIC,
						VALUE = d.VALUE
					};
					insertList.Add(newData);
				}
				else if (existData != null && !string.IsNullOrWhiteSpace(d.LANGNAME))
				{
					existData.SUB_NAME = d.LANGNAME;
					updateList.Add(existData);
				}
				else if (existData != null && string.IsNullOrWhiteSpace(d.LANGNAME))
				{
					f000904I18NRepo.Delete(o=>o.TOPIC == d.TOPIC && o.SUBTOPIC == d.SUBTOPIC && o.VALUE == d.VALUE && o.LANG == d.LANG);
				}
			}

			if (updateList.Any())
				f000904I18NRepo.BulkUpdate(updateList);
			if (insertList.Any())
				f000904I18NRepo.BulkInsert(insertList);
			return new ExecuteResult(true);
		}

		public ExecuteResult DeletedOrUpdateP710601(bool isDeleted, List<F000904> f000904s)
		{
			var f000904Repo = new F000904Repository(Schemas.CoreSchema, _wmsTransaction);
			var f000904I18NRepo = new F000904_I18NRepository(Schemas.CoreSchema, _wmsTransaction);
			if (isDeleted)
			{
				f000904s.ForEach(o =>
				{
					f000904Repo.Delete(x => x.TOPIC == o.TOPIC && x.SUBTOPIC == o.SUBTOPIC && x.VALUE == o.VALUE);
					f000904I18NRepo.Delete(x => x.TOPIC == o.TOPIC && x.SUBTOPIC == o.SUBTOPIC && x.VALUE == o.VALUE);
				});
			}
			else
			{
				var upI18NDatas = new List<F000904_I18N>();
				var upF000904Datas = new List<F000904>();
				var i18NData = f000904I18NRepo.AsForUpdate().GetDatasByTrueAndCondition(o => o.LANG == "zh-TW");
				var update = f000904Repo.AsForUpdate().GetDatasByTrueAndCondition();
				foreach (var up in f000904s)
				{
					var data = update.FirstOrDefault(o => o.TOPIC == up.TOPIC && o.SUBTOPIC == up.SUBTOPIC && o.VALUE == up.VALUE);
					if (data != null)
					{
						data.NAME = up.NAME;
						data.ISUSAGE = up.ISUSAGE;
						upF000904Datas.Add(data);
					}
				}
				f000904Repo.BulkUpdate(upF000904Datas);
				foreach (var up in f000904s)
				{
					var data = i18NData.FirstOrDefault(o => o.TOPIC == up.TOPIC && o.SUBTOPIC == up.SUBTOPIC && o.VALUE == up.VALUE);
					if (data != null)
					{
						data.NAME = up.NAME;
						upI18NDatas.Add(data);
						//f000904I18NRepo.Update(data);
					}
				}
				f000904I18NRepo.BulkUpdate(i18NData);
			}
			return new ExecuteResult(true);
		}
	}
}
