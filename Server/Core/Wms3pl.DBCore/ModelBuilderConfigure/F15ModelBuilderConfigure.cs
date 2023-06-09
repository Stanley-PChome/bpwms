using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F15;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF15ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F151001>().HasKey(key => new { key.ALLOCATION_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F15100101>().HasKey(key => new { key.ALLOCATION_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F151002>().HasKey(key => new { key.ALLOCATION_NO,key.ALLOCATION_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F151003>().HasKey(key => new { key.LACK_SEQ });
        modelBuilder.Entity<F151004>().HasKey(key => new { key.ALLOCATION_NO,key.MOVE_BOX_NO,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F1511>().HasKey(key => new { key.ORDER_NO,key.ORDER_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F151201>().HasKey(key => new { key.BATCH_ALLOC_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F151202>().HasKey(key => new { key.BATCH_ALLOC_NO,key.BATCH_ALLOC_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F151203>().HasKey(key => new { key.BATCH_ALLOC_NO,key.ALLOCATION_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
     }
  }
}
