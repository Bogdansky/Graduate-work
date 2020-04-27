using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Profiles;
using Data_Access_Layer;
using Data_Access_Layer.Models;
using Microsoft.EntityFrameworkCore;
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
                if (model.TaskStatus == 0)
                {
                    model.TaskStatus = Enums.TaskStatusEnum.New;
                }

                var exists = _dbContext.Tasks.Count(t => t.ProjectId == model.ProjectId && t.Title == model.Title) > 0;
                if (!exists)
                {
                    var task = _mapper.Map<Task>(model);
                    task.UpdateDate = DateTime.Now;
                    var newEntity = _dbContext.Tasks.Add(task);
                    if (_dbContext.SaveChanges() == 0)
                    {
                        _logger.LogWarning("Не удалось сохранить задание \"{0}\"", model.Title);
                    }
                    else
                    {
                        _logger.LogInformation("Задание \"{0}\" успешно сохранено", model.Title);
                        return new OperationResult { Result = _mapper.Map<TaskDTO>(newEntity.Entity) };
                    }
                    return new OperationResult { Result = _mapper.Map<TaskDTO>(newEntity.Entity) };
                }
                return new OperationResult { Error = new Error { Title = "Ошибка задания", Description = "Задание с таким заголовком уже существует" } };
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Произошла ошибка при создании задания \"{0}\"", model.Title ?? "Unknown");
                return new OperationResult
                {
                    Error = new Error { Description = "Произошла неожиданная ошибка при создании задания" }
                };
            }
        }

        public override OperationResult Delete(int id)
        {
            try
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
                using var transaction = _dbContext.Database.BeginTransaction();
                _dbContext.Entry(task).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return new OperationResult { Result = new { Success = _dbContext.SaveChanges() > 0 } };
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при удалении задания";
                _logger.LogError(e, "{0} c id = {1}", originMessage, id);
                return new OperationResult
                {
                    Error = new Error
                    {
                        Title = "Ошибка удаления задания",
                        Description = "Произошла неожиданная ошибка"
                    }
                };
            }
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
                _logger.LogInformation("Прочтено задание с id={0}", id);
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

        public OperationResult ReadAll(TaskFilter filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (filter.ProjectId == 0)
                {
                    var ids = _readonlyDbContext.TeamMembers.Include(t => t.Project).Where(t => t.EmployeeId == filter.EmployeeId && t.ProjectId.HasValue).Select(t => t.ProjectId).ToArray();
                    var projects = _readonlyDbContext.Projects.Include(p => p.TeamMembers).Include(p => p.Tasks)
                        .Where(p => ids.Contains(p.Id)).ToArray();
                    var teams = _readonlyDbContext.TeamMembers.Include(t => t.Employee).Where(t => ids.Contains(t.ProjectId)).Select(t => new { t.ProjectId, t.Employee }).GroupBy(t => t.ProjectId).Select(g => new { Id = g.Key, Team = g.Select(t => t.Employee) });
                    result.Result = new { teams, projects = _mapper.Map<ProjectDTO[]>(projects) };
                    return result;
                }
                IQueryable<Task> data;

                data = filter.TaskFilterType switch
                {
                    TaskFilterTypes.AllMine => _readonlyDbContext.Tasks.Where(t => t.EmployeeId == filter.EmployeeId),
                    TaskFilterTypes.AllInProject => _readonlyDbContext.Tasks.Where(t => t.ProjectId == filter.ProjectId),
                    TaskFilterTypes.MineInProject => _readonlyDbContext.Tasks.Where(t => t.EmployeeId == filter.EmployeeId && t.ProjectId == filter.ProjectId),
                    _ => null
                };
                result.Result = data.ToArray();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ошибка при чтении заданий");
                return new OperationResult { Error = new Error { Description = "Неожиданная ошибка при получении заданий" } };
            }
        }

        public override OperationResult Update(int id, TaskDTO model)
        {
            try
            {

                if (_dbContext.Tasks.Count(t => t.Id == id) == 0)
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
                task.UpdateDate = DateTime.Now;

                if (task.Id == default)
                {
                    task.Id = id;
                }

                _dbContext.Tasks.Attach(task);
                _dbContext.Entry(task).State = EntityState.Modified;
                if (_dbContext.SaveChanges() > 0)
                {
                    _logger.LogInformation("Задание с id={0} было успешно обновлено", id);
                }

                return new OperationResult
                {
                    Result = _mapper.Map<TaskDTO>(task)
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
                var employeeFIO = _dbContext.Employees.Where(e => e.Id == employeeId).Select(e => e.FullName).FirstOrDefault();
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
                    task.UpdateDate = DateTime.Now;
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

        public async void UpdateTrackedAsync(int taskId, long recent)
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);
            if (task != null)
            {
                task.Recent = recent;
                _dbContext.Entry(task).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
