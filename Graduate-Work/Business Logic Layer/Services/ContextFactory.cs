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

        private Context CreateNewContext(bool trackChanges = true)
        {
            var context = new Context(_config);
            context.ChangeTracker.QueryTrackingBehavior = trackChanges ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
            return context;
        }
    }
}
