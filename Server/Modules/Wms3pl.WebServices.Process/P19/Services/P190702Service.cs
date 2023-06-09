
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices.Process.P19.Services
{
  public partial class P190702Service
  {
    private WmsTransaction _wmsTransaction;
    public P190702Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

	  public Dictionary<string,List<KeyValuePair<string,string>>> GetComboQueryData()
	  {
		  var f190703Repo = new F190703Repository(Schemas.CoreSchema);
			var data = f190703Repo.Filter(o => o.FUN_ID.HasValue && o.PTYPE == "2").Select(o => o.FUN_ID).Distinct().ToList();
			var list = new Dictionary<string, List<KeyValuePair<string, string>>>();
			foreach (var funId in data)
			{
				var item = GetQueryData(funId.Value, new List<object>());
				if (item.Item1 != null)
				{
					var ds = item.Item1;
					var listSub = (from DataRow row in ds.Tables[0].Rows select new KeyValuePair<string, string>(row["Value"].ToString(), row["Name"].ToString())).ToList();
					list.Add(funId.Value.ToString(), listSub);
				}
				else
				{
					list.Add(funId.Value.ToString(), new List<KeyValuePair<string,string>>());
				}
			}
		  return list;
	  }

	  public Tuple<DataSet, string> GetQueryData(int funId, List<object> listParameters)
	  {
			var repF190702 = new F190702Repository(Schemas.CoreSchema);
			var f190702 = repF190702.GetF190702DataByFunId(funId);
			if (f190702 == null) return new Tuple<DataSet, string>(null, Properties.Resources.P190702Service_FUN_IDNotFound);

			//var repF190703 = new F190703Repository(Schemas.CoreSchema);
			//var f190703s = repF190703.Filter(p => p.QID == qid).OrderBy(p => p.SEQ).ToList();


			string[] parameterNames = f190702.FUN_PARM_LIST == null ? new string[] { } : f190702.FUN_PARM_LIST.Split(',');
			//if (parameterNames.Length > listParameters.Count)
			//  return new Tuple<DataSet, string>(null, "查詢參數數量錯誤");

			//var newParameters = new List<ParametersEntity>();
			//for (int i = 0; i < parameterNames.Length; i++)
			//  newParameters.Add(new ParametersEntity
			//  {
			//    ParamsName = parameterNames[i],
			//    ParamsValue = listParameters[i],
			//    ParamsType = f190703s[i].PTYPE,
			//    ParamsFormat = f190703s[i].FORMAT
			//  });
			var newParameters = new Dictionary<string, object>();
			if (listParameters.Count > 0)
		  {
			  for (int i = 0; i < parameterNames.Length; i++)
				  newParameters.Add(parameterNames[i], listParameters[i]);
		  }
		  if (string.IsNullOrEmpty(f190702.FUN_SQL)) return new Tuple<DataSet, string>(null, Properties.Resources.P190702Service_FUN_SQLNotFound);

			return repF190702.GetQueryData(f190702.FUN_SQL, newParameters);
	  }

	  public Tuple<DataSet, string> GetQueryData(decimal qid, List<object> listParameters)
    {
      var repF190701 = new F190701Repository(Schemas.CoreSchema);
      var f190701 = repF190701.Find(p => p.QID == qid);
      if (f190701 == null) return new Tuple<DataSet, string>(null, Properties.Resources.P190702Service_QIDNotFound);
      if (f190701.FUN_ID == null) return new Tuple<DataSet, string>(null, Properties.Resources.P190702Service_FUN_IDUnSet);

		  return GetQueryData(f190701.FUN_ID.Value, listParameters);
    }

  }
}
