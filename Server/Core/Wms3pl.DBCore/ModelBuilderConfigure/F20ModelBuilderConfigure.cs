using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F20;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF20ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F200101>().HasKey(key => new { key.ADJUST_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F200102>().HasKey(key => new { key.ADJUST_NO,key.ADJUST_SEQ,key.GUP_CODE,key.CUST_CODE,key.DC_CODE });
        modelBuilder.Entity<F20010201>().HasKey(key => new { key.DISTR_CAR_NO,key.DISTR_CAR_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F20010202>().HasKey(key => new { key.CONSIGN_ID });
        modelBuilder.Entity<F200103>().HasKey(key => new { key.ADJUST_NO,key.ADJUST_SEQ,key.GUP_CODE,key.CUST_CODE,key.DC_CODE });
        modelBuilder.Entity<F20010301>().HasKey(key => new { key.ADJUST_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
     }
  }
}
