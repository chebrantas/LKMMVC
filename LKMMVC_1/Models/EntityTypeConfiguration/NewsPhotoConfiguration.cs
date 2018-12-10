using System.Data.Entity.ModelConfiguration;

namespace LKMMVC_1.Models.EntityTypeConfiguration
{
    public class NewsPhotoConfiguration:EntityTypeConfiguration<NewsPhotoDetail>
    {
        public NewsPhotoConfiguration()
        {
            //np=NewsPhotos trumpinys
            HasRequired(np => np.News).WithMany(n => n.NewsPhotoDetails).HasForeignKey(np => np.NewsID);
            //ToTable("NewsPhotos");
        }
    }
}