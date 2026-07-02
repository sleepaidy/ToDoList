using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ToDoList.Data
{
    public class WebContextFactory : IDesignTimeDbContextFactory<WebContext>
    {
        public WebContext CreateDbContext(string[] args)
        {
            var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ToDoList"));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<WebContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultDbConnection"));

            return new WebContext(optionsBuilder.Options);
        }
    }
}
