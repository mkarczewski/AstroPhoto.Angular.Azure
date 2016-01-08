using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AP.DB
{
    public class AstroObject
    {
        [Key]
        [StringLength(255)]
        public string Uid { get; set; }
        public string BigPhotoUrl { get; set; }
        public int BigPhotoX { get; set; }
        public int BigPhotoY { get; set; }
        public string CroppedPhotoUrl { get; set; }
        public string Name { get; set; }
    }
}