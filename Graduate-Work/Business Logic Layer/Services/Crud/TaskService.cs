using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Profiles;
using Data_Access_Layer;
using Data_Access_Layer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business_Logic_Layer.Services.Crud
{
    public class TaskService : BaseCrudService<TaskDTO>
    {
        public TaskService(ILogger<TaskService> logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {
            
        }

        public override OperationResult Create(TaskDTO model)
        {
            try
            {
                if (_dbContext.Tasks.Any(t => t.ProjectId == model.ProjectId && t.Title == model.Title))
                {
                    var task = _mapper.Map<Task>(model);
                    var newEntity = _dbContext.Tasks.Add(task);
                    if (_dbContext.SaveChanges() == 0)
                    {
                        _logger.LogWarning("Не удалось сохранить задание \"{0}\"", model.Title);
                    }
                    else
                    {
                        _logger.LogInformation("Задание \"{0}\" успешно сохранено", model.Title);
                    }
                    return new OperationResult { Result = new { id = newEntity.Entity.Id } };
                }
                return new OperationResult { Error = new Error { Title = "Ошибка задания", Description = "Задание с таким заголовком уже существует" } };
            }
            catch
            {
                _logger.LogError("Произошла ошибка при создании задания \"{0}\"", model.Title ?? "Unknown");
                return new OperationResult
                {
                    Error = new Error { Description = "Произошла неожиданная ошибка при создании задания" }
                };
            }
        }

        public override OperationResult Delete(int id)
        {
            var task = _dbContext.Tasks.Find(id);
            if (task == null)
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                _dbContext.Entry(task).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                _dbContext.SaveChanges();
                transaction.Rollback();
            }
            return new OperationResult();
        }

        public override OperationResult Read(int id)
        {
            try
            {
                var task = _readonlyDbContext.Tasks.Find(id);
                if (task == null)
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
                _logger.LogInformation("Прочтено задания с id={0}", id);
                return new OperationResult
                {
                    Result = _mapper.Map<TaskDTO>(task)
                };
            }
            catch(Exception e)
            {
                var errorText = "При чтении задания произошла неожиданная ошибка.";
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
            return new OperationResult { Result = _readonlyDbContext.Tasks.ToArray() };
        }

        public override OperationResult Update(int id, TaskDTO model)
        {
            try
            {
                if (!_dbContext.Tasks.Any(t => t.Id == id))
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
                var task = _mapper.Map<Task>(model);

                if (task.Id == default)
                {
                    task.Id = id;
                }

                _dbContext.Tasks.Attach(task);
                _dbContext.Entry(task).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                if (_dbContext.SaveChanges() > 0)
                {
                    _logger.LogInformation("Задание с id={0} было успешно обновлено", id);
                }

                return new OperationResult
                {
                    Result = model
                };
            }
            catch (Exception e)
            {
                var errorText = "При чтении задания произошла неожиданная ошибка.";
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

        public OperationResult AttachTask(int taskId, int employeeId)
        {
            var result = new OperationResult();
            try
            {
                var errors = new List<string>();
                var task = _dbContext.Tasks.Find(taskId);
                var employeeFIO = _dbContext.Employees.Where(e => e.Id == employeeId).Select(e => e.FIO).FirstOrDefault();
                if (task == null)
                {
                    errors.Add("Такого задания не существует.");
                }
                if (employeeFIO == null)
                {
                    errors.Add("Такого пользователя нет.");
                }
                if (errors.Count == 0)
                {
                    task.EmployeeId = employeeId;
                    _dbContext.Tasks.Update(task);
                    if (_dbContext.SaveChanges() > 0)
                    {
                        _logger.LogInformation("Задание \"{0}\" успешно привязано сотруднику \"{1}\"", task.Title, employeeFIO);
                    }
                    result.Result = new { Id = employeeId, FIO = employeeFIO };
                }
                return result;
            }
            catch(Exception e)
            {
                var errorText = "Произошла ошибка при привязке задания.";
                _logger.LogError(e, errorText);
                result.Error = new Error { Title = "Внутренняя ошибка", Description = errorText};
                return result;
            }
        }

        public override void Dispose()
        {
            _dbContext.Dispose();
            _readonlyDbContext.Dispose();
        }
    }
}
