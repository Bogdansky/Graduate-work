using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using System.Linq.Expressions;
using System.Text;
using TTasks = System.Threading.Tasks;

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

        public OperationResult Search(EmployeeFilter filter)
        {
            try
            {
                var maxRate = filter.MaxRate;
                var minRate = filter.MinRate;

                #region filters
                Expression<Func<Employee, bool>> roleFilter = e => e.RoleId == (int)filter.Role;

                Expression<Func<Employee, bool>> rateFilter = e => e.Tasks.Any(t => t.TaskStatusId == (int)TaskStatusEnum.Closed)
                        && minRate < e.Tasks.Where(t => t.TaskStatusId == (int)TaskStatusEnum.Closed).Select(t => (t.Effort - t.Recent) <= t.Effort ? 100 : t.Effort / (double)(t.Effort - t.Recent) * 100).Average()
                        && maxRate > e.Tasks.Where(t => t.TaskStatusId == (int)TaskStatusEnum.Closed).Select(t => (t.Effort - t.Recent) <= t.Effort ? 100 : t.Effort / (double)(t.Effort - t.Recent) * 100).Average();

                Expression<Func<Employee, bool>> projectsFilter = e => e.TeamMembers.Count() >= filter.MinProjectNumber && e.TeamMembers.Count() <= filter.MaxProjectNumber;
                #endregion

                if (filter.MaxRate < filter.MinRate)
                {
                    minRate = filter.MaxRate;
                    maxRate = filter.MinRate;
                }
                var ids = _readonlyDbContext.TeamMembers.Where(t => t.ProjectId == filter.ProjectId).Select(t => t.EmployeeId).ToArray();
                IQueryable<Employee> query = _readonlyDbContext.Employees.Where(e => e.RoleId != (int)RoleEnum.ProjectManager && !ids.Contains(e.Id));

                if (filter.Role != RoleEnum.None)
                    query = query.Where(roleFilter);

                if (!(minRate == 0 && maxRate == 100))
                    query = query.Where(rateFilter);

                if (filter.MinProjectNumber != filter.MaxProjectNumber)
                    query = query.Where(projectsFilter);

                var result = query.Select(e => new
                {
                    Employee = e,
                    Rate = e.Tasks.Count(t => t.TaskStatusId == (int)TaskStatusEnum.Closed) == 0 ? default : e.Tasks.Where(t => t.TaskStatusId == (int)TaskStatusEnum.Closed)
                    .Select(t => (t.Effort - t.Recent) <= t.Effort ? 100 : t.Effort / (double)(t.Effort - t.Recent) * 100).Average(),
                    ProjectNumber = e.TeamMembers.Count
                }).ToArray();
                return new OperationResult { Result = result };
            }
            catch(Exception e)
            {
                var message = "Неожиданная ошибка при поиске сотрудников";
                _logger.LogError(e, message);
                return new OperationResult { Error = new Error { Description = message} };
            }
        }

        public TTasks.Task<OperationResult> SearchAsync(EmployeeFilter filter)
        {
            return TTasks.Task.Factory.StartNew(() => Search(filter));
        }

        public async TTasks.Task<OperationResult> GetTaskTimeStatisticsAsync(int id, DateTime? dateStart, DateTime? dateEnd)
        {
            var result = new OperationResult();
            var months = new []
            {
                new { Id = MonthEnum.January, Value = MonthEnum.January.GetDescription()},
                new { Id = MonthEnum.February, Value = MonthEnum.February.GetDescription()},
                new { Id = MonthEnum.March, Value = MonthEnum.March.GetDescription()},
                new { Id = MonthEnum.April, Value = MonthEnum.April.GetDescription()},
                new { Id = MonthEnum.May, Value = MonthEnum.May.GetDescription()},
                new { Id = MonthEnum.June, Value = MonthEnum.June.GetDescription()},
                new { Id = MonthEnum.July, Value = MonthEnum.July.GetDescription()},
                new { Id = MonthEnum.August, Value = MonthEnum.August.GetDescription()},
                new { Id = MonthEnum.September, Value = MonthEnum.September.GetDescription()},
                new { Id = MonthEnum.October, Value = MonthEnum.October.GetDescription()},
                new { Id = MonthEnum.November, Value = MonthEnum.November.GetDescription()},
                new { Id = MonthEnum.December, Value = MonthEnum.December.GetDescription()}
            };
            try
            {
                var history = await _readonlyDbContext.TrackingHistory.Where(t => t.EmployeeId == id && t.Date >= dateStart && t.Date <= dateEnd)
                    .ToListAsync(); //.ProjectTo<TrackingHistoryDTO>(_mapper.ConfigurationProvider)
                var taskIds = history.Select(r => r.TaskId).Distinct();
                var tasks = await _readonlyDbContext.Tasks.Where(t => taskIds.Contains(t.Id)).ToArrayAsync();
                if (history.Count == 0)
                {
                    result.Result = new { Message = $"У вас нет задач за период с {dateStart:dd.MM.yyyy} по {dateEnd:dd.MM.yyyy}" };
                    return result;
                }
                var groupedTasks = history.GroupBy(t => t.Date.Year).Select(t => 
                {
                    var key = t.Key;
                    var values = t.GroupBy(i => i.Date.Month).Select(i =>
                    {
                        var key = i.Key;
                        var history = i.Select(x => x).ToList();
                        return new KeyValuePair<int, List<TrackingHistory>>(key, history);
                    });
                    return new KeyValuePair<int, IEnumerable<KeyValuePair<int, List<TrackingHistory>>>>(key, values);
                }).ToList();
                result.Result = new
                {
                    Total = history.GroupBy(t => t.TaskId).Count(),
                    TotalTime = GetFormattedTime(new TimeSpan(history.Sum(t => t.TrackedTime) * TimeSpan.TicksPerMillisecond)),
                    OverallRate = tasks.Select(t => (t.Effort - t.Recent) <= t.Effort ? 100 : t.Effort / (double)(t.Effort - t.Recent) * 100).Average().ToString("#.##"),
                    historyByYears = groupedTasks.Select(t => new { Year = t.Key, Months = t.Value.Select(tm => new { Month = tm.Key.GetDescriptionByValue<MonthEnum>(), Value = Math.Round(new TimeSpan(tm.Value.Sum(rm => rm.TrackedTime) * TimeSpan.TicksPerMillisecond).TotalHours, 2) })})
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Неожиданная ошибка при получении статистики (id=" + id + ")");
                result.Error = new Error { Description = "Неожиданная ошибка" };
            }
            return result;
        }

        public string GetFormattedTime(TimeSpan time)
        {
            var days = Math.Truncate(time.TotalDays) == 0 ? null : time.TotalDays == 1 ? $"{time:%d} день" : time.TotalDays < 5 ? $"{time:%d} дня" : $"{time:%d} дней";
            var hours = time.Hours == 0 ? null : time.Hours == 1 ? $"{time:%h} час" : time.Hours < 5 ? $"{time:%h} часа" : $"{time:%h} часов";
            var minutes = time.Minutes == 0 ? null : time.Minutes == 1 ? $"{time:%m} минута" : time.Minutes < 5 ? $"{time:%m} минуты" : $"{time:%m} минут";
            var seconds = time.Seconds == 0 ? null : time.Seconds == 1 ? $"{time:%s} секунда" : time.Seconds < 5 ? $"{time:%s} секунды" : $"{time:%s} секунд";
            var dates = new string[] { days, hours, minutes, seconds }.Where(d => d != null);
            return string.Join(", ", dates);
        }
    }
}
