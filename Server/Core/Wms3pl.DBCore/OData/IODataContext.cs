using System;
using System.Collections.Generic;
using System.Data.Services.Providers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.DBCore.OData
{
    public interface IODataContext
    {
        IQueryable GetQueryable(ResourceSet set);
        object CreateResource(ResourceType resourceType);
        void AddResource(ResourceType resourceType, object resource);
        void DeleteResource(object resource);
        // void SaveChanges();
    }
}
