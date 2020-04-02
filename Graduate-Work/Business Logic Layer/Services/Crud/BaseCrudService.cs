using AutoMapper;
using Business_Logic_Layer.Models;
using Data_Access_Layer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Services.Crud
{
    public abstract class BaseCrudService<T> : BaseService
        where T: class
    {

        public BaseCrudService(ILogger logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {
        }

        public abstract OperationResult Create(T model);
        public abstract OperationResult Read(int id);
        public abstract OperationResult ReadAll();
        public abstract OperationResult Update(int id, T model);
        public abstract OperationResult Delete(int id);
    }
}
