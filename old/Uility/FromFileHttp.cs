using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace RCDWeb.Areas.RCD.Uility
{
    public class ModelFileForm
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Lenght { get; set; }
        public byte[] FileContent { get; set; }
        public string err { get; set; }
    }
    public class FromFileHttp
    {
        //private string GetContentType(string path)
        //{
        //    var types = GetMimeTypes();
        //    var ext = Path.GetExtension(path).ToLowerInvariant();
        //    return types[ext];
        //}
        //private Dictionary<string, string> GetMimeTypes()
        //{
        //    return new Dictionary<string, string>
        //    {
        //        {".txt", "text/plain"},
        //        {".pdf", "application/pdf"},
        //        {".doc", "application/vnd.ms-word"},
        //        {".docx", "application/vnd.ms-word"},
        //        {".xls", "application/vnd.ms-excel"},
        //        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        //        {".png", "image/png"},
        //        {".jpg", "image/jpeg"},
        //        {".jpeg", "image/jpeg"},
        //        {".gif", "image/gif"},
        //        {".csv", "text/csv"}
        //    };
        //}

        //[NotMapped]
        //public HttpPostedFileBase _FileForm;
        //[Display(Name = "file", Prompt = "file", Description = "file")]
        //[NotMapped]
        //public string Error { get; set; }
        //[NotMapped]
        //public HttpPostedFileBase FileForm
        //{
        //    get
        //    {
        //        return _FileForm;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                if (value.ContentLength < 20971520)
        //                {
        //                    MemoryStream target = new MemoryStream();
        //                    value.InputStream.CopyTo(target);
        //                    FileContent = target.ToArray();
        //                    FileName = value.FileName;
        //                    ContentType = value.ContentType;
        //                    Lenght = value.ContentLength;
        //                }
        //                else
        //                {
        //                    Console.WriteLine("The file is too large.");
        //                }
        //            }
        //        }
        //        _FileForm = value;
        //    }
        //}

        public ModelFileForm FileForm(HttpPostedFileBase value)
        {

            if (value != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var fileForm = new ModelFileForm();
                    fileForm.FileName = value.FileName;
                    fileForm.ContentType = value.ContentType;
                    fileForm.Lenght = value.ContentLength;
                    fileForm.FileContent = null;
                    fileForm.err = "File troppo grande:"+fileForm.Lenght;

                    if (fileForm.Lenght < 20971520)
                    {
                        MemoryStream target = new MemoryStream();
                        value.InputStream.CopyTo(target);
                        fileForm.FileContent = target.ToArray();
                        fileForm.err = "";
                    }
                    return fileForm;
                }
            }
            return null;
        }
    }
}
