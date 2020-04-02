using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Data_Access_Layer
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var databaseSettings = configuration.GetSection("DBSettings");
            var connectionString = string.Format("server={0};UserId={1};Password={2};database={3};",
                databaseSettings["Server"], databaseSettings["User"], databaseSettings["Password"], databaseSettings["Database"]);
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseMySQL(connectionString);
            return new Context(optionsBuilder.Options);
        }
    }
}
