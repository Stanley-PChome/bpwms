using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F07;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
    private static void SetF07ModelRelation(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<F0701>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F070101>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F070102>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F070103>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F070104>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F07010401>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F075101>().HasKey(key => new { key.CUST_CODE, key.CUST_ORD_NO });
      modelBuilder.Entity<F075102>().HasKey(key => new { key.CUST_CODE, key.CUST_ORD_NO });
      modelBuilder.Entity<F075103>().HasKey(key => new { key.CUST_CODE, key.CUST_ORD_NO });
      modelBuilder.Entity<F075105>().HasKey(key => new { key.DC_CODE, key.DOC_ID });
      modelBuilder.Entity<F075106>().HasKey(key => new { key.DC_CODE, key.DOC_ID });
      modelBuilder.Entity<F075107>().HasKey(key => new { key.DC_CODE, key.DOC_ID });
      modelBuilder.Entity<F075108>().HasKey(key => new { key.DC_CODE, key.DOC_ID });
			modelBuilder.Entity<F075109>().HasKey(key => new { key.DC_CODE, key.DOC_ID });
			modelBuilder.Entity<F076101>().HasKey(key => new { key.CONTAINER_CODE });
			modelBuilder.Entity<F076102>().HasKey(key => new { key.CONTAINER_CODE });
			modelBuilder.Entity<F077101>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F077102>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F076103>().HasKey(key => new { key.DC_CODE, key.CUST_ORD_NO });
      modelBuilder.Entity<F076104>().HasKey(key => new { key.CONTAINER_CODE });
    }
  }
}
