using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Services.Crud
{
    public class ProjectService : BaseCrudService<ProjectDTO>
    {
        private readonly ContextFactory _contextFactory;
        public ProjectService(IMapper mapper, ILogger logger, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {

        }
        public override OperationResult Create(ProjectDTO model)
        {
            throw new NotImplementedException();
        }

        public override OperationResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
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

        public override OperationResult Update(int id, ProjectDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
