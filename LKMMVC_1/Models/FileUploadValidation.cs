using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace LKMMVC_1.Models
{
    //nustatymui ar geras failas ikeltas
    public enum SuportedTypes
    {
        Documents,
        Images
    }
    public class FileUploadValidation
    {
        public FileUploadValidation()
        {
            IsValid = true;
        }
        
        string[] supportedFileTypes;

        public string Message { get; set; }
        public decimal filesize { get; set; }
        public bool IsValid { get; private set; }
        public void ValidateUploadedUserFile(HttpFileCollectionBase file, SuportedTypes suportedTypes)
        {
            try
            {
                if (suportedTypes.Equals(SuportedTypes.Documents))
                {
                    supportedFileTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx" };
                }
                else
                {
                    supportedFileTypes = new[] { "jpg", "jpeg", "png" };

                }
                for (int i = 0; i < file.Count; i++)
                {

                    var fileExtension = Path.GetExtension(file[i].FileName).Substring(1);
                    if (!supportedFileTypes.Contains(fileExtension))
                    {
                        switch (suportedTypes)
                        {
                            case SuportedTypes.Documents:
                                Message = "Blogas prisegtas failas \"" + file[i].FileName + "\" - galima prisegti tik WORD/PDF/EXCEL/TXT failus.";
                                break;
                            case SuportedTypes.Images:
                                Message = "Blogas prisegtas failas \""+ file[i].FileName + "\"- galima prisegti tik paveikslėlius JPG/JPEG/PNG.";
                                break;
                        }
                        IsValid = false;
                        break;
                    }
                    else if (file[i].ContentLength > (filesize * 1024))
                    {
                        Message = "Per didelis prisegto failo \""+ file[i].FileName + "\" dydis, turi būti ne didesnis nei: " + filesize + " KB.";
                        IsValid = false;
                        break;
                    }
                    else
                    {
                        Message = "Failas(-ai) sėkmingai patalpinti.";
                        IsValid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "Upload Container Should Not Be Empty or Contact Admin";
            }
        }

    }
}