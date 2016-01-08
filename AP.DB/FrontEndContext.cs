using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AP.DB
{
    public class FrontEndContext : DbContext
    {
        public DbSet<AstroObject> AstroObjects { get; set; }
    }
}