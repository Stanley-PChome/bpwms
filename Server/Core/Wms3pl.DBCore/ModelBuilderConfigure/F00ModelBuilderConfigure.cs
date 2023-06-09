using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F00;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
    private static void SetF00ModelRelation(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<F0000>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F0001>().HasKey(key => new { key.CROSS_CODE });
      modelBuilder.Entity<F0002>().HasKey(key => new { key.DC_CODE, key.LOGISTIC_CODE });
      modelBuilder.Entity<F0003>().HasKey(key => new { key.AP_NAME, key.CUST_CODE, key.GUP_CODE, key.DC_CODE });
      modelBuilder.Entity<F0005>().HasKey(key => new { key.SET_NAME, key.DC_CODE });
      modelBuilder.Entity<F0006>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F0009>().HasKey(key => new { key.ORD_TYPE });
      modelBuilder.Entity<F000901>().HasKey(key => new { key.ORD_TYPE });
      modelBuilder.Entity<F000902>().HasKey(key => new { key.SOURCE_TYPE });
      modelBuilder.Entity<F00090201>().HasKey(key => new { key.WORK_TYPE, key.SOURCE_TYPE, key.THIS_STATUS });
      modelBuilder.Entity<F000903>().HasKey(key => new { key.ORD_PROP });
      modelBuilder.Entity<F000904>().HasKey(key => new { key.TOPIC, key.SUBTOPIC, key.VALUE });
      modelBuilder.Entity<F000904_I18N>().HasKey(key => new { key.TOPIC, key.SUBTOPIC, key.VALUE, key.LANG });
      modelBuilder.Entity<F000906>().HasKey(key => new { key.TICKET_CLASS });
      modelBuilder.Entity<F0010>().HasKey(key => new { key.HELP_NO, key.DC_CODE });
      modelBuilder.Entity<F001001>().HasKey(key => new { key.HELP_TYPE });
      modelBuilder.Entity<F0011>().HasKey(key => new { key.ID, key.DC_CODE, key.CUST_CODE, key.GUP_CODE });
      modelBuilder.Entity<F0020>().HasKey(key => new { key.MSG_NO });
      modelBuilder.Entity<F0050>().HasKey(key => new { key.SEQ_NO });
      modelBuilder.Entity<F005001>().HasKey(key => new { key.SEQ_NO });
      modelBuilder.Entity<F0060>().HasKey(key => new { key.IMPORT_LOG_ID });
      modelBuilder.Entity<F0070>().HasKey(key => new { key.CONNECTID });
      modelBuilder.Entity<F0080>().HasKey(key => new { key.MESSAGE_ID });
      modelBuilder.Entity<F0090>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F009001>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F009002>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F009003>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F009004>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F009005>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F0091>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F0092>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F0093>().HasKey(key => new { key.ID });
    }
  }
}
