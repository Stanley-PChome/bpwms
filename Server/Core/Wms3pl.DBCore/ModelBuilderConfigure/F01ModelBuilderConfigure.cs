using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F01;

namespace Wms3pl.DBCore
{
    public partial class ModelBuilderConfigure
    {
        private static void SetF01ModelRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<F010101>().HasKey(key => new { key.SHOP_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
            modelBuilder.Entity<F010102>().HasKey(key => new { key.SHOP_NO, key.SHOP_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
            modelBuilder.Entity<F010201>().HasKey(key => new { key.STOCK_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
            modelBuilder.Entity<F010202>().HasKey(key => new { key.STOCK_NO, key.STOCK_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
            modelBuilder.Entity<F010203>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.STICKER_NO });
            modelBuilder.Entity<F010204>().HasKey(key => new { key.ID });
            modelBuilder.Entity<F010205>().HasKey(key => new { key.ID });
            modelBuilder.Entity<F010301>().HasKey(key => new { key.ID });
            modelBuilder.Entity<F010301_HISTORY>().HasKey(key => new { key.ID });
            modelBuilder.Entity<F010302>().HasKey(key => new { key.ID });
            modelBuilder.Entity<F010302_HISTORY>().HasKey(key => new { key.ID });
        }
    }
}
