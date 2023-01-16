using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RCDWeb.Areas.RCD.Models
{
    public class EmailSettings
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }
        public string SmtpServer { get; set; }
        public int? SmtpPort { get; set; }
        public bool? SmtpSSL { get; set; }
        public string SenderName { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        //public string PopServer { get; set; }
        //public int? PopPort { get; set; }
        //public bool? PopSSL { get; set; }
        //public string PopUsername { get; set; }
        //public string PopPassword { get; set; }
    }
}