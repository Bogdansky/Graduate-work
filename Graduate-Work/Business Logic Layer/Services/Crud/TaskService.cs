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
        private Context _dbContext;
        private Context _readonlyDbContext;

        public TaskService(ILogger logger, IMapper mapper, ContextFactory contextFactory, TaskProfile taskProfile) : base(logger, mapper)
        {
            _dbContext = contextFactory.CreateDbContext();
            _readonlyDbContext = contextFactory.CreateReadonlyDbContext();
        }

        public override OperationResult Create(TaskDTO model)
        {
            try
            {
                if (_dbContext.Tasks.All(t => t.ProjectId == model.ProjectId && t.Title != model.Title))
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

        public override OperationResult Update(int id, TaskDTO model)
        {
            throw new NotImplementedException();
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
                    errors.Add("Такого задания не существует");
                }
                if (employeeFIO == null)
                {
                    errors.Add("Такого пользователя нет");
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
                var errorText = "Произошла ошибка при привязке задания";
                _logger.LogError(e, errorText);
                result.Error = new Error { Title = "Внутренняя ошибка", Description = errorText};
                return result;
            }
        }
    }
}
