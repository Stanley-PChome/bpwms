using Wms3pl.Datas.Shared.ApiEntities;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
    public class BaseService
    {
        public string WcsConvertMessage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
                msg = msg.Substring(0, (msg.Length > 30 ? 30 : msg.Length));
            return msg;
        }
    }
}
