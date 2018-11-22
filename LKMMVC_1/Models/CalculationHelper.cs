using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace LKMMVC_1.Models
{
    public class CalculationHelper
    {
        //CommonContext db = new CommonContext();

        //pakeiciamas ivedamas pavadinimas lietuviskos raides keiciamos lotyniskomis
        //tarpas keiciamas i bruksneli ir taip sukuriamas katalogas is tokio pavadinimo
        public static string ChangeNewsTitle(string Title)
        {
            Regex rgx1 = new Regex("[?:Ą]");
            Regex rgx2 = new Regex("[?:Č]");
            Regex rgx3 = new Regex("[?:ĘĖ]");
            Regex rgx4 = new Regex("[?:Į]");
            Regex rgx5 = new Regex("[?:Š]");
            Regex rgx6 = new Regex("[?:ŲŪ]");
            Regex rgx7 = new Regex("[?:Ž]");
            Regex rgx8 = new Regex("[^a-zA-Z0-9 -.]");

            Title = Title.ToUpper();
            Title = rgx1.Replace(Title, "A");
            Title = rgx2.Replace(Title, "C");
            Title = rgx3.Replace(Title, "E");
            Title = rgx4.Replace(Title, "I");
            Title = rgx5.Replace(Title, "S");
            Title = rgx6.Replace(Title, "U");
            Title = rgx7.Replace(Title, "Z");
            Title = rgx8.Replace(Title, "");
            Title = Title.Replace(" ", "-");
            return Title;
        }
    }
}