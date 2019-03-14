using Microsoft.EntityFrameworkCore;
using Security2.Database.Entities;

namespace Security2.Database
{
    public class DatabaseContext : DbContext
    {
        /// <inheritdoc />
        public DatabaseContext(DbContextOptions<DatabaseContext> opt) : base(opt)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
    }
}
