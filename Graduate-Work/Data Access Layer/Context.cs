using Data_Access_Layer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Data_Access_Layer
{
    public class Context : DbContext
    {
        private IConfiguration _config;
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }

        public Context(IConfiguration config)
        {
            _config = config;

            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databaseSettings = _config.GetSection("DBSettings");
            var connectionString = string.Format("server={0};UserId={1};Password={2};database={3};",
                databaseSettings["Server"], databaseSettings["User"], databaseSettings["Password"], databaseSettings["Database"]);
            optionsBuilder.UseMySql(connectionString);
        }
    }
}
