using Microsoft.EntityFrameworkCore;
using ToDoList.Data.Models;

namespace ToDoList.Data
{
    public class WebContext : DbContext
    {
        public DbSet<TaskData> Tasks { get; set; }

        public WebContext(DbContextOptions<WebContext> options):base(options) { }
    }
}
