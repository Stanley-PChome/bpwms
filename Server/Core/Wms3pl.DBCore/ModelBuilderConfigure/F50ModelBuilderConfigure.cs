using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F50;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF50ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F500101>().HasKey(key => new { key.QUOTE_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F500102>().HasKey(key => new { key.QUOTE_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F500103>().HasKey(key => new { key.QUOTE_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F500104>().HasKey(key => new { key.QUOTE_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F500105>().HasKey(key => new { key.QUOTE_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F500201>().HasKey(key => new { key.CNT_DATE,key.CONTRACT_NO,key.QUOTE_NO });
        modelBuilder.Entity<F500202>().HasKey(key => new { key.REPORT_NO });
     }
  }
}
