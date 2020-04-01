using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business_Logic_Layer.Services.Crud
{
    public class OrganizationService : BaseCrudService<OrganizationDTO>
    {
        public OrganizationService(ILogger<TaskService> logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {

        }

        public override OperationResult Create(OrganizationDTO model)
        {
            OperationResult result = new OperationResult();
            if (model.Name == default || model.Name == "")
            {
                result.Error = new Error { Title = "Ошибка создания организации", Description = "Неверно указано наименование!" };
                return result;
            }
            try
            {
                var exists = _dbContext.Organizations.Any(o => o.Name == model.Name);
                if (exists)
                {
                    result.Error = new Error { Title = "Ошибка организации", Description = "Такая организация уже есть в системе!" };
                    return result;
                }
                var organization = _mapper.Map<Organization>(model);
                result.Result = _dbContext.Organizations.Add(organization).Entity;
                if (_dbContext.SaveChanges() == 0)
                {
                    _logger.LogWarning("Не удалось сохранить организацию \"{0}\"", model.Name);
                }
                else
                {
                    _logger.LogInformation("Организация \"{0}\" успешно сохранено", model.Name);
                    return new OperationResult { Result = new { Success = false } };
                }
                return result;
            }
            catch (Exception e)
            {
                var originMessage = "Произошла ошибка при создании задания";
                _logger.LogError(e, "{0} \"{1}\"", originMessage, model.Name ?? "Unknown");
                return new OperationResult
                {
                    Error = new Error { Description = "Произошла неожиданная ошибка при создании задания" }
                };
            }
        }

        public override OperationResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override OperationResult Read(int id)
        {
            try
            {
                var organization = _readonlyDbContext.Organizations.Find(id);
                if (organization == null)
                {
                    return new OperationResult
                    {
                        Error = new Error
                        {
                            Title = "Ошибка получения организации",
                            Description = "Такой организации нет."
                        }
                    };
                }
                _logger.LogInformation("Прочтена организация с id={0}", id);
                return new OperationResult
                {
                    Result = _mapper.Map<OrganizationDTO>(organization)
                };
            }
            catch (Exception e)
            {
                var errorText = "При чтении организации произошла неожиданная ошибка.";
                _logger.LogError(e, errorText);
                return new OperationResult
                {
                    Error = new Error
                    {
                        Title = "Внутренняя ошибка",
                        Description = errorText
                    }
                };
            }
        }

        public override OperationResult ReadAll()
        {
            return new OperationResult { Result = _readonlyDbContext.Organizations.Select(o => o.Name)};
        }

        public override OperationResult Update(int id, OrganizationDTO model)
        {
            OperationResult result = new OperationResult();
            try
            {
                var exists = _dbContext.Organizations.Any(p => p.Id == id);
                if (!exists)
                {
                    result.Error = new Error { Title = "Ошибка при обновлении", Description = "Такой организаци не существует!" };
                }
                else
                {
                    var organization = _mapper.Map<Organization>(model);
                    using var transaction = _dbContext.Database.BeginTransaction();
                    var entity = _dbContext.Organizations.Update(organization);
                    result.Result = _mapper.Map<OrganizationDTO>(entity.Entity);
                }
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при обновлении организации";
                _logger.LogError(e, "{0} \"{1}\" c id = {2}", originMessage, model.Name, id);
                result.Error = new Error { Description = originMessage };
            }
            return result;
        }
    }
}
