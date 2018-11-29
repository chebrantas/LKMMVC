using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LKMMVC_1.Models
{
    public class NewsPhotoDetail
    {
        public int NewsPhotoDetailID { get; set; }
        public string FileName { get; set; }
        public int NewsID { get; set; }
        public virtual News News { get; set; }
        //nuotraukos kelias serveryje kur fiziskai padeta
        public string PhotoLocation { get; set; }
        //nuotraukos kelias serveryje kur fiziskai padeta sumazinta
        public string PhotoLocationThumb { get; set; }
        public bool IsChecked { get; set; }

    }
}