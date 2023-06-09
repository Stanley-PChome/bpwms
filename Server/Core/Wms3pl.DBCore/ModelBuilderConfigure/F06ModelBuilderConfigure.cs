using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F06;

namespace Wms3pl.DBCore
{
	public partial class ModelBuilderConfigure
	{
		private static void SetF06ModelRelation(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<F060101>().HasKey(key => new { key.DOC_ID, key.CMD_TYPE });
			modelBuilder.Entity<F060102>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060201>().HasKey(key => new { key.DOC_ID, key.CMD_TYPE });
			modelBuilder.Entity<F060202>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060203>().HasKey(key => new { key.DOC_ID });
			modelBuilder.Entity<F060204>().HasKey(key => new { key.DOC_ID });
			modelBuilder.Entity<F060205>().HasKey(key => new { key.DOC_ID });
			modelBuilder.Entity<F060206>().HasKey(key => new { key.DOC_ID });
			modelBuilder.Entity<F060207>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F06020701>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F06020702>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060208>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060301>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060302>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060401>().HasKey(key => new { key.DOC_ID, key.CMD_TYPE });
			modelBuilder.Entity<F060402>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060403>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060404>().HasKey(key => new { key.DOC_ID, key.CMD_TYPE });
			modelBuilder.Entity<F060405>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060406>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060501>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060601>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060602>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060701>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060702>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060801>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060802>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F060209>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F06020901>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F06020902>().HasKey(key => new { key.ID });
    }
	}
}
