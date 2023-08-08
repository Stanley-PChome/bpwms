using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P50.ExDataSources
{
	public partial class P50ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}
		
		public IQueryable<F199007Data> F199007Datas
		{
			get { return new List<F199007Data>().AsQueryable(); }
		}

		public IQueryable<F500101QueryData> F500101QueryDatas
		{
			get { return new List<F500101QueryData>().AsQueryable(); }
		}

		public IQueryable<F500104QueryData> F500104QueryDatas
		{
			get { return new List<F500104QueryData>().AsQueryable(); }
		}

		public IQueryable<F500103QueryData> F500103QueryDatas
		{
			get { return new List<F500103QueryData>().AsQueryable(); }
		}
		public IQueryable<F500102QueryData> F500102QueryDatas
		{
			get { return new List<F500102QueryData>().AsQueryable(); }
		}

		public IQueryable<F500105QueryData> F500105QueryDatas
		{
			get { return new List<F500105QueryData>().AsQueryable(); }
		}

		public IQueryable<F500201ClearingData> F500201ClearingDatas
		{
			get { return new List<F500201ClearingData>().AsQueryable(); }
		}

		public IQueryable<RP7105100001> RP7105100001s
		{
			get { return new List<RP7105100001>().AsQueryable(); }
		}

		public IQueryable<RP7105100002> RP7105100002s
		{
			get { return new List<RP7105100002>().AsQueryable(); }
		}

		public IQueryable<RP7105100003> RP7105100003s
		{
			get { return new List<RP7105100003>().AsQueryable(); }
		}

		public IQueryable<RP7105100004> RP7105100004s
		{
			get { return new List<RP7105100004>().AsQueryable(); }
		}

		public IQueryable<RP7105100005> RP7105100005s
		{
			get { return new List<RP7105100005>().AsQueryable(); }
		}

	}
}
