using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
	[TestClass]
	public class F1963RepositoryTest : BaseRepositoryTest
	{
		private F1963Repository _f1963Repo;
		public F1963RepositoryTest()
		{
			_f1963Repo = new F1963Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetNewId()
		{
			#region Params
			#endregion

			_f1963Repo.GetNewId();
		}

		[TestMethod]
		public void CheckDuplicateByIdName()
		{
			#region Params
			var workgroupId = 1;
			var groupName = "NK良品倉";
			#endregion

			_f1963Repo.CheckDuplicateByIdName(workgroupId, groupName);
		}
	}
}
