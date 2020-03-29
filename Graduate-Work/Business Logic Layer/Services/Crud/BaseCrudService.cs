using AutoMapper;
using Business_Logic_Layer.Models;
using Data_Access_Layer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Services.Crud
{
    public abstract class BaseCrudService<T> : IDisposable
        where T: class
    {
        public ILogger _logger;
        public IMapper _mapper;
        public Context _dbContext;
        public Context _readonlyDbContext;

        public BaseCrudService(ILogger logger, IMapper mapper, ContextFactory contextFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = contextFactory.CreateDbContext();
            _readonlyDbContext = contextFactory.CreateReadonlyDbContext();
        }

        public abstract OperationResult Create(T model);
        public abstract OperationResult Read(int id);
        public abstract OperationResult ReadAll();
        public abstract OperationResult Update(int id, T model);
        public abstract OperationResult Delete(int id);
        public abstract void Dispose();
    }
}
