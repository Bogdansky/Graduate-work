using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Business_Logic_Layer.Helpers;

namespace Business_Logic_Layer.Services.Crud
{
    public class ProjectService : BaseCrudService<ProjectDTO>
    {
        public ProjectService(IMapper mapper, ILogger logger, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {

        }

        public OperationResult Create(int employeeId, ProjectDTO model)
        {
            var employee = _readonlyDbContext.Employees.Find(employeeId);
            if (employee == null)
            {
                return new OperationResult { Error = new Error { Title = "Ошибка при создании проекта", Description = "Создатель не найден!" } };
            }
            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
            if (employeeDTO.Role != RoleEnum.ProjectManager)
            {
                return new OperationResult { Error = new Error { Title = "Ошибка при создании проекта", Description = "Пользователь не является проектным менеджером" } };
            }
            if (model.Administrators == null)
            {
                model.Administrators = new List<EmployeeDTO> { employeeDTO };
            }
            else
            {
                model.Administrators.Add(employeeDTO);
            }
            var teamMember = new TeamMemberDTO { EmployeeId = employeeId, Employee = employeeDTO };
            if (model.Team == null)
            {
                model.Team = new List<TeamMemberDTO> { teamMember };
            }
            else
            {
                model.Team.Add(teamMember);
            }

            return Create(model);
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
                    return new OperationResult { Result = new { Success = false } };
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
                    transaction.Rollback();
                    return new OperationResult { Result = new { success = false } };
                }
                transaction.Commit();
                result.Result = new { success = true };
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
            OperationResult result = new OperationResult();
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
        /// <summary>
        /// Добавляет пользователя в проект
        /// </summary>
        /// <param name="employeeId">Добавляющий пользователя</param>
        /// <param name="model">Добавляемый пользователь</param>
        /// <returns></returns>
        public OperationResult AddEmployee(int employeeId, TeamMemberDTO model)
        {
            try
            {
                var teamMember = _dbContext.TeamMembers.Where(tm => tm.EmployeeId == model.EmployeeId && tm.ProjectId == model.ProjectId).FirstOrDefault();
                if (teamMember != null)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при добавлении сотрудника в проект", Description = "Вы не являетсь членом команды проекта, в который хотите добавить сотрудника!" } };
                }
                if (teamMember.Role.Id != (int)RoleEnum.ProjectManager)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при добавлении сотрудника в проект", Description = "Вы не являетесь проектным менеджером!" } };
                }
                var newTeamMember = _mapper.Map<TeamMember>(model);
                var entity = _dbContext.TeamMembers.Add(newTeamMember);
                if (_dbContext.SaveChanges() > 0)
                {
                    _logger.LogInformation("Сотрудник(id=) успешно добавлен в проект(id=)", model.EmployeeId, model.ProjectId);
                }
                else
                {
                    _logger.LogWarning("Не удалось добавить добавить в проект(id=)", model.EmployeeId, model.ProjectId);
                    return new OperationResult { Result = new { Success = false } };
                }
                return new OperationResult { Result = _mapper.Map<TeamMemberDTO>(entity.Entity) };
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Неожиданная ошибка при добавлении на проект(id={0}) сотрудника(id={1})", model.ProjectId, model.EmployeeId);
                return new OperationResult { Error = new Error { Description = "Неожиданная ошибка при добавлении на проект сотрудника" } };
            }
        }
    }
}
