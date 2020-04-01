using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Services.Crud
{
    public class EmployeeService : BaseCrudService<EmployeeDTO>
    {
        public EmployeeService(ILogger logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {

        }

        public override OperationResult Create(EmployeeDTO model)
        {
            throw new NotImplementedException();
        }

        public override OperationResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override OperationResult Read(int id)
        {
            throw new NotImplementedException();
        }

        public override OperationResult ReadAll()
        {
            throw new NotImplementedException();
        }

        public override OperationResult Update(int id, EmployeeDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
