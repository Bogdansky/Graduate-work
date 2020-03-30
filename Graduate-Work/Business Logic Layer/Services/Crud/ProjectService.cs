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
    public class ProjectService : BaseCrudService<ProjectDTO>
    {
        public ProjectService(IMapper mapper, ILogger logger, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {

        }
        public override OperationResult Create(ProjectDTO model)
        {
            OperationResult result = new OperationResult();
            try
            {
                var existsById = _readonlyDbContext.Projects.Find(model.Id);
                if (existsById != null)
                {
                    result.Error = new Error { Title = "Ошибка проекта", Description = "Такой проект уже есть!" };
                    return result;
                }
                var existsByName = _readonlyDbContext.Projects.Where(p => p.Name == model.Name && p.OrganizationId == model.OrganizationId).Any();
                if (existsByName)
                {
                    result.Error = new Error { Title = "Ошибка проекта", Description = "Проект с таким именем уже есть" };
                    return result;
                }
                var project = _mapper.Map<Project>(model);
                var entity = _dbContext.Projects.Add(project);
                if (_dbContext.SaveChanges() > 0)
                {
                    _logger.LogInformation("Проект \"{0}\" успешно сохранён", model.Name);
                }
                else
                {
                    _logger.LogWarning("Не удалось сохранить проект \"{0}\"", model.Name);
                }
                result.Result = _mapper.Map<ProjectDTO>(entity.Entity);
            }
            catch(Exception e)
            {
                var originMessage = "Неожиданная ошибка при создании проекта";
                _logger.LogError(e, "{0} \"{1}\"", originMessage, model.Name ?? "<not set>");
                result.Error = new Error { Description = originMessage };
            }
            return result;
        }

        public override OperationResult Delete(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                var project = _dbContext.Projects.Find(id);
                if (project == null)
                {
                    result.Error = new Error { Title = "Ошибка при удалении", Description = "Такого проекта не существует!" };
                }
                using var transaction = _dbContext.Database.BeginTransaction();
                var projectTeam = _dbContext.TeamMembers.Where(tm => tm.ProjectId == id);
                _dbContext.TeamMembers.RemoveRange(projectTeam);
                _dbContext.Projects.Remove(project);
                if (_dbContext.SaveChanges() > 0)
                {
                    _logger.LogInformation("Проект \"{0}\" успешно удалён", project.Name);
                }
                else
                {
                    _logger.LogWarning("Не удалось удалить проект \"{0}\"", project.Name);
                }
                transaction.Commit();
                result.Result = new { Success = true };
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при удалении проекта";
                _logger.LogError(e, "{0} c id = {1}", originMessage, id);
                result.Error = new Error { Description = originMessage };
            }
            return result;
        }

        public override OperationResult Read(int id)
        {
            try
            {
                var project = _readonlyDbContext.Projects.Find(id);
                return project == null ? new OperationResult { Error = new Error { Title = "Ошибка при получении проекта", Description = "Такого проекта нет!" } } : 
                    new OperationResult { Result = _mapper.Map<ProjectDTO>(project) };
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при получении проекта";
                _logger.LogError(e, "{0} c id = {1}", originMessage, id);
                return new OperationResult { Error = new Error { Description = originMessage } };
            }
        }

        public override OperationResult ReadAll()
        {
            try
            {
                var projects = _readonlyDbContext.Projects.ToList();
                return new OperationResult { Result = _mapper.Map<ICollection<ProjectDTO>>(projects) };
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Ошибка при чтении проектов");
                return new OperationResult { Error = new Error { Description = "Неожиданная ошибка при получении всех проектов" } };
            }
        }

        public OperationResult ReadAll(ProjectFilter filter)
        {
            try
            {
                var query = _readonlyDbContext.Projects.Where(p => p.OrganizationId == filter.OrganizationId);
                if (filter.Like != default)
                {
                    query = query.Where(p => p.Name.Contains(filter.Like));
                }
                if (filter.Filter == ProjectFilterEnum.Mine)
                {
                    query = query.Where(p => p.Team.Any(t => t.EmployeeId == filter.EmployeeId));
                }
                var projects = query.ToList();
                return new OperationResult { Result = _mapper.Map<ICollection<ProjectDTO>>(projects) };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ошибка при чтении проектов организации id={} для сотрудника id={0}", filter.OrganizationId, filter.EmployeeId);
                return new OperationResult { Error = new Error { Description = "Неожиданная ошибка при получении всех проектов сотрудника" } };
            }
        }

        public override OperationResult Update(int id, ProjectDTO model)
        {
            OperationResult result = null;
            try
            {
                var exists = _dbContext.Projects.Any(p => p.Id == id);
                if (!exists)
                {
                    result.Error = new Error { Title = "Ошибка при обновлении", Description = "Такого проекта не существует!" };
                }
                else
                {
                    var project = _mapper.Map<Project>(model);
                    using var transaction = _dbContext.Database.BeginTransaction();
                    var entity = _dbContext.Projects.Update(project);
                    result.Result = _mapper.Map<ProjectDTO>(entity.Entity);
                }
            }
            catch(Exception e)
            {
                var originMessage = "Неожиданная ошибка при обновлении проекта";
                _logger.LogError(e, "{0} \"{1}\" c id = {2}", originMessage, model.Name, id);
                result.Error = new Error { Description = originMessage };
            }
            return result;
        }
    }
}
