using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LKMMVC_1.Models
{
    public class CommonInitializer:DropCreateDatabaseAlways<CommonContext>
    {
        protected override void Seed(CommonContext context)
        {
            context.News.Add(new News
            {
                Content = "Turinys 1",
                Title = "Pavadinimas 1",
                PostDate = DateTime.Now,
                NewsPhotoDetails = new List<NewsPhotoDetail>
                {
                    new NewsPhotoDetail
                    {
                        NewsID=1,
                        PhotoLocation=@"Photo\2018\10\grazios foto",
                        IsChecked=false,
                        FileName="1.jpg",
                    },
                    new NewsPhotoDetail
                    {
                        NewsID=1,
                        PhotoLocation=@"Photo\2018\11\grazios foto1",
                        IsChecked=true,
                        FileName="2.jpg",
                    }
                }
            });
            context.News.Add(new News
            {
                Content = "Turinys 2",
                Title = "Pavadinimas 2",
                PostDate = DateTime.Now,
                NewsPhotoDetails = new List<NewsPhotoDetail>
                {
                    new NewsPhotoDetail
                    {
                        NewsID=2,
                        PhotoLocation=@"Photo\2018\10\grazios foto22",
                        IsChecked=true,
                        FileName="1.jpg",
                    },
                    new NewsPhotoDetail
                    {
                        NewsID=2,
                        PhotoLocation=@"Photo\2018\11\grazios foto22",
                        IsChecked=false,
                        FileName="2.jpg",
                    }
                }
            });



            base.Seed(context);
        }
    }
}