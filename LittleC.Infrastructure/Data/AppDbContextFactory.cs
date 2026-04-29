using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LittleC.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=littlecarsarsdb_local;User=root;Password=Sh0wn100;",
                ServerVersion.Parse("9.5.0-mysql")
            );
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}