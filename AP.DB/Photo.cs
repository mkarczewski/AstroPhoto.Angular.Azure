using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP.DB
{
    public class Photo
    {
        public string PhotoUrl { get; set; }
        public string PhotoUid { get; set; }
        public PhotoItem[] Items { get; set; }
    }
}
