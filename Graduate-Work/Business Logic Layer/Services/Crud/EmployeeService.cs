using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Enums;
using Business_Logic_Layer.Helpers;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Models;
using Microsoft.EntityFrameworkCore;
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

        public OperationResult ReadByUserId(int id)
        {
            try
            {
                var employee = _readonlyDbContext.Users.Where(u => u.Id == id).Select(u => u.Employee).FirstOrDefault();
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
        public OperationResult ReadAll(int projectId)
        {
            try
            {
                var employees = _readonlyDbContext.TeamMembers.Where(t => t.ProjectId == projectId).Select(t => new { t.Employee.Id, t.Employee.FullName, t.Employee.Birthday }).ToArray();
                if (employees == null || employees.Count() == 0)
                {
                    return new OperationResult
                    {
                        Error = new Error
                        {
                            Title = "Ошибка получения сотрудников",
                            Description = "На данном проекте никого нет!"
                        }
                    };
                }
                _logger.LogInformation("Получены участники проекта с id={0}", projectId);
                return new OperationResult
                {
                    Result = employees
                };
            }
            catch (Exception e)
            {
                var errorText = "При получении сотрудников проекта произошла неожиданная ошибка.";
                _logger.LogError(e, errorText + " (id проекта = {0})", projectId);
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
                var exists = _dbContext.Employees.Where(p => p.Id == id).Count() > 0;
                if (!exists)
                {
                    result.Error = new Error { Title = "Ошибка при обновлении", Description = "Такого сотрудника не существует!" };
                }
                else
                {
                    var employee = _mapper.Map<Employee>(model);
                    employee.Id = id;   
                    using var transaction = _dbContext.Database.BeginTransaction();
                    var entity = _dbContext.Employees.Update(employee);
                    _dbContext.SaveChanges();
                    result.Result = _mapper.Map<EmployeeDTO>(entity.Entity);
                    transaction.Commit();
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

        public OperationResult ReadProjects(int id)
        {
            var projects = _readonlyDbContext.TeamMembers.Include(t => t.Employee).Include(t => t.Project).ThenInclude(p => p.TeamMembers).Where(t => t.EmployeeId == id)
                .Select(t => new { t.Project.Id, t.Project.Name, Team = t.Project.TeamMembers.Select(t => t.Employee).ToArray() }).ToArray();
            return new OperationResult { Result = projects };
        }

        public OperationResult GetEnums()
        {
            var roles = new[]
            {
                new Role { Id = (int)RoleEnum.None, Name = RoleEnum.None.GetDescription()},
                new Role { Id = (int)RoleEnum.JuniorSoftwareEngineer, Name = RoleEnum.JuniorSoftwareEngineer.GetDescription() },
                new Role { Id = (int)RoleEnum.MiddleSoftwareEngineer, Name = RoleEnum.MiddleSoftwareEngineer.GetDescription()},
                new Role { Id = (int)RoleEnum.SeniorSoftwareEngineer, Name = RoleEnum.SeniorSoftwareEngineer.GetDescription() },
                new Role { Id = (int)RoleEnum.TeamLeadSoftwareEngineer, Name = RoleEnum.TeamLeadSoftwareEngineer.GetDescription()},
                new Role { Id = (int)RoleEnum.QAEngineer, Name = RoleEnum.QAEngineer.GetDescription() },
                new Role { Id = (int)RoleEnum.QATeamLeader, Name = RoleEnum.QATeamLeader.GetDescription()},
                new Role { Id = (int)RoleEnum.BusinessAnalyst, Name = RoleEnum.BusinessAnalyst.GetDescription() },
                new Role { Id = (int)RoleEnum.GUIDesigner, Name = RoleEnum.GUIDesigner.GetDescription()},
                new Role { Id = (int)RoleEnum.DataScientist, Name = RoleEnum.DataScientist.GetDescription()},
                new Role { Id = (int)RoleEnum.QAAutomationEngineer, Name = RoleEnum.QAAutomationEngineer.GetDescription()},
                new Role { Id = (int)RoleEnum.ProjectManager, Name = RoleEnum.ProjectManager.GetDescription()},
                new Role { Id = (int)RoleEnum.DataEngineer, Name = RoleEnum.DataEngineer.GetDescription()},
                new Role { Id = (int)RoleEnum.DataAnalyst, Name = RoleEnum.DataAnalyst.GetDescription()}
            };
            var taskStatuses = new []
            {
                new TaskStatus { Id = (int)TaskStatusEnum.New, Name = TaskStatusEnum.New.GetDescription()},
                new TaskStatus { Id = (int)TaskStatusEnum.Active, Name = TaskStatusEnum.Active.GetDescription() },
                new TaskStatus { Id = (int)TaskStatusEnum.ReadyForQA, Name = TaskStatusEnum.ReadyForQA.GetDescription() },
                new TaskStatus { Id = (int)TaskStatusEnum.Closed, Name = TaskStatusEnum.Closed.GetDescription() }
            };
            var taskTypes = new[]
            {
                new TaskType { Id = (int)TaskTypeEnum.Task, Name = TaskTypeEnum.Task.GetDescription() },
                new TaskType { Id = (int)TaskTypeEnum.Bug, Name = TaskTypeEnum.Bug.GetDescription() }
            };

            return new OperationResult { Result = new { roles, taskTypes, taskStatuses } };
        }
    }
}
