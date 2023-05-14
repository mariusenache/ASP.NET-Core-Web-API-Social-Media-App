using Microsoft.EntityFrameworkCore;
using PracticaCST.Data.Entities;

namespace PracticaCST.Data
{
    public class SocMediaDb : DbContext
    {
        public SocMediaDb(DbContextOptions<SocMediaDb> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }  
        public DbSet<Post> Posts { get; set; }

    }
}
