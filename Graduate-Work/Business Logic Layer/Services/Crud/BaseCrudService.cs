using AutoMapper;
using Business_Logic_Layer.Models;
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
        public BaseCrudService(ILogger logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public abstract OperationResult Create(T model);
        public abstract OperationResult Read(int id);
        public abstract OperationResult ReadAll();
        public abstract OperationResult Update(int id, T model);
        public abstract OperationResult Delete(int id);
        public abstract void Dispose();
    }
}
