using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LKMMVC_1.Models;

namespace LKMMVC_1.Areas.Admin.ViewModel
{
    public class NewsViewModel
    {
        public int NewsID { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        //public string FileName { get; set; }
        public int NewsPhotoID { get; set; }
        //nuotraukos kelias serveryje kur fiziskai padeta
        public virtual ICollection<NewsPhotoDetail> NewsPhotos { get; set; }

    }
}