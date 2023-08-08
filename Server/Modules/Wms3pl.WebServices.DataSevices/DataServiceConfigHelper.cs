using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;

namespace Wms3pl.WebServices.DataSevices
{
    public class DataServiceConfigHelper
    {
        internal static void SetEntitiesAccess(DataServiceConfiguration config, string preName)
        {
            typeof(Wms3plDbContext).GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) && p.Name.StartsWith(preName)).ToList().ForEach(t =>
            {
                config.SetEntitySetAccessRule(t.Name, EntitySetRights.All);
            });
        }
    }
}
