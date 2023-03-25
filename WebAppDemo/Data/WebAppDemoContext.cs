using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppDemo.Models;

namespace WebAppDemo.Data
{
    public class WebAppDemoContext : DbContext
    {
        public WebAppDemoContext (DbContextOptions<WebAppDemoContext> options)
            : base(options)
        {
        }

        public DbSet<WebAppDemo.Models.Booking> Booking { get; set; } = default!;
    }
}
