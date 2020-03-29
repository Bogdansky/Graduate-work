using Data_Access_Layer;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Services
{
    public class ContextFactory
    {
        private IConfiguration _config;

        public ContextFactory(IConfiguration config)
        {
            _config = config;
        }

        public Context CreateReadonlyDbContext()
        {
            return CreateNewContext(false);
        }

        public Context CreateDbContext()
        {
            return CreateNewContext();
        }

        private DbContextOptionsBuilder GetOptionsBuilder()
        {
            var databaseSettings = _config.GetSection("DBSettings");
            var connectionString = string.Format("server={0};UserId={1};Password={2};database={3};",
                databaseSettings["Server"], databaseSettings["User"], databaseSettings["Password"], databaseSettings["Database"]);
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseMySql(connectionString);
            return optionsBuilder;
        }

        private Context CreateNewContext(bool trackChanges = true)
        {
            var context = new Context(GetOptionsBuilder().Options);
            context.ChangeTracker.QueryTrackingBehavior = trackChanges ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
            return context;
        }
    }
}
