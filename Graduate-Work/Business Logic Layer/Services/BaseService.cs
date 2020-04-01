using AutoMapper;
using Data_Access_Layer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Services
{
    public abstract class BaseService : IDisposable
    {
        public ILogger _logger;
        public IMapper _mapper;
        public Context _dbContext;
        public Context _readonlyDbContext;

        public BaseService(ILogger logger, IMapper mapper, ContextFactory contextFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = contextFactory.CreateDbContext();
            _readonlyDbContext = contextFactory.CreateReadonlyDbContext();
        }
        public async virtual void Dispose()
        {
            await _dbContext.DisposeAsync();
            await _readonlyDbContext.DisposeAsync();
        }
    }
}
