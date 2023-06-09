using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F25;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF25ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F2501>().HasKey(key => new { key.SERIAL_NO,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F250101>().HasKey(key => new { key.LOG_SEQ });
        modelBuilder.Entity<F250102>().HasKey(key => new { key.LOG_SEQ,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F25010201>().HasKey(key => new { key.FREEZE_LOG_SEQ,key.CONTROL,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F250103>().HasKey(key => new { key.LOG_SEQ });
        modelBuilder.Entity<F250104>().HasKey(key => new { key.LOG_SEQ });
        modelBuilder.Entity<F250105>().HasKey(key => new { key.LOG_SEQ });
        modelBuilder.Entity<F250106>().HasKey(key => new { key.LOG_SEQ });
     }
  }
}
