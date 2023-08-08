using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
    public class Wms3plDbContextBase : DbContext
    {
        public Wms3plDbContextBase() : base()
        {

        }

        public Wms3plDbContextBase(DbContextOptions options) : base(options)
        {

        }
        public bool IsSqlQuery { get; set; }
    }
}
