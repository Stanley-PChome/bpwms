using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F16;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF16ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F160201>().HasKey(key => new { key.RTN_VNR_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F160202>().HasKey(key => new { key.RTN_VNR_NO,key.RTN_VNR_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F160203>().HasKey(key => new { key.RTN_VNR_TYPE_ID });
        modelBuilder.Entity<F160204>().HasKey(key => new { key.RTN_WMS_NO,key.RTN_WMS_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F160401>().HasKey(key => new { key.SCRAP_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F160402>().HasKey(key => new { key.SCRAP_NO,key.SCRAP_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F160501>().HasKey(key => new { key.DESTROY_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F160502>().HasKey(key => new { key.DESTROY_NO,key.DESTROY_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F160503>().HasKey(key => new { key.UPLOAD_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F16050301>().HasKey(key => new { key.UPLOAD_SEQ,key.DESTROY_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F160504>().HasKey(key => new { key.DESTROY_NO,key.SERIAL_SEQ,key.GUP_CODE,key.CUST_CODE,key.DC_CODE });
        modelBuilder.Entity<F161201>().HasKey(key => new { key.RETURN_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F161202>().HasKey(key => new { key.RETURN_NO,key.RETURN_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F16120201>().HasKey(key => new { key.RETURN_NO,key.RETURN_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F161203>().HasKey(key => new { key.RTN_TYPE_ID });
        modelBuilder.Entity<F161204>().HasKey(key => new { key.RETURN_NO,key.PAST_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F161301>().HasKey(key => new { key.RTN_CHECK_NO,key.DC_CODE });
        modelBuilder.Entity<F161302>().HasKey(key => new { key.RTN_CHECK_NO,key.RTN_CHECK_SEQ,key.DC_CODE });
        modelBuilder.Entity<F161401>().HasKey(key => new { key.RETURN_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F16140101>().HasKey(key => new { key.RETURN_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F16140102>().HasKey(key => new { key.LOG_ID });
        modelBuilder.Entity<F161402>().HasKey(key => new { key.RETURN_NO,key.RETURN_AUDIT_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F16140201>().HasKey(key => new { key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.RETURN_NO,key.RETURN_AUDIT_SEQ,key.RTN_DTL_SEQ });
        modelBuilder.Entity<F161501>().HasKey(key => new { key.GATHER_NO,key.DC_CODE });
        modelBuilder.Entity<F161502>().HasKey(key => new { key.GATHER_NO,key.GATHER_SEQ,key.DC_CODE });
        modelBuilder.Entity<F161601>().HasKey(key => new { key.RTN_APPLY_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F161602>().HasKey(key => new { key.RTN_APPLY_NO,key.RTN_APPLY_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
     }
  }
}
