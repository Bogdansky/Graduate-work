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
    public class EmployeeService : BaseCrudService<EmployeeDTO>
    {
        public EmployeeService(ILogger<EmployeeService> logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {

        }

        // добавить пользователя в проект
        public OperationResult AcceptInvite(EmployeeDTO model)
        {
            var projectExists = _readonlyDbContext.Projects.Any(p => p.Id == model.OrganizationId);
            if (!projectExists)
            {
                return new OperationResult { Error = new Error { Description = "Извините, данной организации уже не существует!" } };
            }
            return Create(model);
        }

        public override OperationResult Create(EmployeeDTO model)
        {
            var result = new OperationResult();
            try
            {
                var employee = _mapper.Map<Employee>(model);
                result.Result = _dbContext.Employees.Add(employee).Entity;
                if (_dbContext.SaveChanges() == 0)
                {
                    _logger.LogWarning("Не удалось оформить сотрудника");
                }
                else
                {
                    _logger.LogInformation("Сотрудник успешно оформлен");
                    return new OperationResult { Result = new { Success = true } };
                }
                return result;
            }
            catch (Exception e)
            {
                var originMessage = "Произошла ошибка при оформлении сотрудника";
                _logger.LogError(e, originMessage);
                return new OperationResult
                {
                    Error = new Error { Description = originMessage }
                };
            }
        }

        public override OperationResult Delete(int id)
        {
            try
            {
                var employee = _dbContext.Employees.Find(id);
                if (employee == null)
                {
                    return new OperationResult
                    {
                        Error = new Error
                        {
                            Title = "Ошибка получения задания",
                            Description = "Такого задания нет."
                        }
                    };
                }
                using var transaction = _dbContext.Database.BeginTransaction();
                _dbContext.Entry(employee).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                var success = _dbContext.SaveChanges() > 0;
                transaction.Commit();
                return new OperationResult { Result = new { success } };
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при увольнении сотрудника";
                _logger.LogError(e, "{0} c id = {1}", originMessage, id);
                return new OperationResult
                {
                    Error = new Error
                    {
                        Description = originMessage
                    }
                };
            }
        }

        public override OperationResult Read(int id)
        {
            try
            {
                var employee = _readonlyDbContext.Employees.Find(id);
                if (employee == null)
                {
                    return new OperationResult
                    {
                        Error = new Error
                        {
                            Title = "Ошибка получения сотрудника",
                            Description = "Такого сотрудника нет."
                        }
                    };
                }
                _logger.LogInformation("Получен сотрудник с id={0}", id);
                return new OperationResult
                {
                    Result = _mapper.Map<EmployeeDTO>(employee)
                };
            }
            catch (Exception e)
            {
                var errorText = "При получении сотрудника произошла неожиданная ошибка.";
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
            throw new NotImplementedException();
        }
        public OperationResult ReadAll(int organizationId)
        {
            try
            {
                var employees = _readonlyDbContext.Employees.Where(e => e.OrganizationId == organizationId).Select(e => new { e.Id, e.FullName, e.Birthday }).ToArray();
                if (employees == null || employees.Count() == 0)
                {
                    return new OperationResult
                    {
                        Error = new Error
                        {
                            Title = "Ошибка получения сотрудников",
                            Description = "В данной организации нет сотрудников!"
                        }
                    };
                }
                _logger.LogInformation("Получены сотрудники организации с id={0}", organizationId);
                return new OperationResult
                {
                    Result = employees
                };
            }
            catch (Exception e)
            {
                var errorText = "При получении сотрудников организации произошла неожиданная ошибка.";
                _logger.LogError(e, errorText + " (id организации = {0})", organizationId);
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

        public override OperationResult Update(int id, EmployeeDTO model)
        {
            OperationResult result = new OperationResult();
            try
            {
                var exists = _dbContext.Employees.Any(p => p.Id == id);
                if (!exists)
                {
                    result.Error = new Error { Title = "Ошибка при обновлении", Description = "Такого сотрудника не существует!" };
                }
                else
                {
                    var employee = _mapper.Map<Employee>(model);
                    using var transaction = _dbContext.Database.BeginTransaction();
                    var entity = _dbContext.Employees.Update(employee);
                    result.Result = _mapper.Map<EmployeeDTO>(entity.Entity);
                }
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при обновлении сотрудника";
                _logger.LogError(e, "{0} c id = {1}", originMessage, id);
                result.Error = new Error { Description = originMessage };
            }
            return result;
        }

        public OperationResult GetRoles()
        {
            var roles = _readonlyDbContext.Roles.Select(r => r.Name);
            return new OperationResult { Result = roles };
        }
    }
}
