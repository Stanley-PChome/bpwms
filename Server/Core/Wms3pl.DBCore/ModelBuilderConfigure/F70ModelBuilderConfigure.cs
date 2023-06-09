using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F70;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF70ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F700101>().HasKey(key => new { key.DISTR_CAR_NO,key.DC_CODE });
        modelBuilder.Entity<F70010101>().HasKey(key => new { key.CUST_NAME });
        modelBuilder.Entity<F700102>().HasKey(key => new { key.DISTR_CAR_NO,key.DISTR_CAR_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F70010201>().HasKey(key => new { key.DC_CODE,key.DISTR_CAR_NO,key.DETAIL_NO });
        modelBuilder.Entity<F700201>().HasKey(key => new { key.COMPLAINT_NO,key.DC_CODE });
        modelBuilder.Entity<F700501>().HasKey(key => new { key.SCHEDULE_NO,key.DC_CODE });
        modelBuilder.Entity<F70050101>().HasKey(key => new { key.SCHEDULE_NO,key.GRP_ID,key.DC_CODE });
        modelBuilder.Entity<F700701>().HasKey(key => new { key.IMPORT_DATE,key.GRP_ID,key.DC_CODE });
        modelBuilder.Entity<F700702>().HasKey(key => new { key.CNT_DATE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F700703>().HasKey(key => new { key.CNT_DATE,key.ITEM_CODE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F700705>().HasKey(key => new { key.CNT_DATE,key.EMP_ID,key.EMP_NAME,key.GRP_ID,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F700706>().HasKey(key => new { key.CNT_DATE,key.DC_CODE });
        modelBuilder.Entity<F700707>().HasKey(key => new { key.CNT_DATE,key.DC_CODE });
        modelBuilder.Entity<F700708>().HasKey(key => new { key.CNT_DATE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.GRP_ID });
        modelBuilder.Entity<F700709>().HasKey(key => new { key.CNT_DATE,key.ORD_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F700801>().HasKey(key => new { key.DC_CODE,key.DELIVERY_NO });
        modelBuilder.Entity<F700802>().HasKey(key => new { key.DC_CODE,key.DELIVERY_NO,key.GUP_CODE,key.CUST_CODE,key.WMS_NO });
     }
  }
}
