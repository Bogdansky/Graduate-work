using AutoMapper;
using Business_Logic_Layer.Models;
using Data_Access_Layer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services.Crud
{
    public abstract class BaseCrudService<T> : BaseService
        where T: class
    {

        public BaseCrudService(ILogger<BaseCrudService<T>> logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {
        }

        public abstract OperationResult Create(T model);
        public abstract OperationResult Read(int id);
        public abstract OperationResult ReadAll();
        public abstract OperationResult Update(int id, T model);
        public abstract OperationResult Delete(int id);
        public async virtual Task<OperationResult> CreateAsync(T model)
        {
            return await Task.Factory.StartNew(() => Create(model));
        }
        public async virtual Task<OperationResult> ReadAsync(int id)
        {
            return await Task.Factory.StartNew(() => Read(id));
        }
        public async virtual Task<OperationResult> ReadAllAsync()
        {
            return await Task.Factory.StartNew(() => ReadAll());
        }
        public async virtual Task<OperationResult> UpdateAsync(int id, T model)
        {
            return await Task.Factory.StartNew(() => Update(id, model));
        }
        public async virtual Task<OperationResult> DeleteAsync(int id)
        {
            return await Task.Factory.StartNew(() => Delete(id));
        }
    }
}
