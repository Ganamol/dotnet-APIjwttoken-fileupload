using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace webapiii.Models
{
    public class BrandContext:DbContext
    {
        public BrandContext(DbContextOptions<BrandContext> options):base(options)
        {
                
        }
        public DbSet<Brand> brands { get; set; }
        public DbSet<Fileupload> files { get; set; }
    }
}
