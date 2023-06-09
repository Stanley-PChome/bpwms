using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.View;

namespace Wms3pl.DBCore
{
    public partial class ModelBuilderConfigure
    {
        private static void SetViewModelRelation(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Query<VW_F000904_LANG>().ToView("VW_F000904_LANG");
            //modelBuilder.Entity<VW_F000904_LANG>().HasKey(key => new { key.TOPIC, key.SUBTOPIC, key.VALUE, key.LANG });
        }
    }
}
