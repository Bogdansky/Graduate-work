using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Business_Logic_Layer.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic_Layer.Services.Crud
{
    public class ProjectService : BaseCrudService<ProjectDTO>
    {
        private readonly IConfiguration _config;
        private ServiceCache _cache;
        private EmailService _emailService;
        public ProjectService(IMapper mapper, ILogger<ProjectService> logger, ContextFactory contextFactory, ServiceCache cache, EmailService emailService, IConfiguration config) : base(logger, mapper, contextFactory)
        {
            _config = config;
            _cache = cache;
            _emailService = emailService;
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
            var teamMember = new TeamMemberDTO { EmployeeId = employeeId, Role = employeeDTO.Role, IsAdmin = true };
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
                var existsByName = _readonlyDbContext.Projects.Where(p => p.Name == model.Name).Count() > 0;
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
                IQueryable<Project> query = null;
                if (filter.EmployeeId.HasValue)
                {
                    query = _readonlyDbContext.TeamMembers.Where(t => t.EmployeeId == filter.EmployeeId)
                        .Select(t => t.Project).Include(p => p.TeamMembers);
                }
                //if (filter.Like != default)
                //{
                //    query = query == null ? _readonlyDbContext.Projects.Where(p => p.Name.Contains(filter.Like)) : query.Where(p => p.Name.Contains(filter.Like));
                //}

                var projects = query?.ToList();
                return new OperationResult { Result = _mapper.Map<ICollection<ProjectDTO>>(projects) };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ошибка при чтении проектов для сотрудника id={0}", filter.EmployeeId);
                return new OperationResult { Error = new Error { Description = "Неожиданная ошибка при получении всех проектов сотрудника" } };
            }
        }

        public OperationResult ReadAllEmployees(int projectId)
        {
            try
            {
                var team = _readonlyDbContext.TeamMembers.Include(t => t.Employee).Where(t => t.ProjectId == projectId).ToArray();
                var res = team.Select(t => new
                {
                    t.Employee.FullName,
                    Role = t.RoleId.Value.GetDescriptionByValue<RoleEnum>(),
                    t.IsAdmin
                });
                return new OperationResult { Result = res};
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ошибка при чтении сотрудников проекта (id={0})", projectId);
                return new OperationResult { Error = new Error { Description = "Неожиданная ошибка при получении сотрудников проекта" } };
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

        public async Task<OperationResult> InviteUser(int projectId, int employeeId, string email)
        {
            try
            {
                var employee = _readonlyDbContext.Employees.Where(e => e.User.Login == email).FirstOrDefault();
                if (employee == null)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при приглашении сотрудника на проект", Description = "Пользователь не зарегистрирован!" } };
                }
                var projectName = _readonlyDbContext.Projects.Where(p => p.Id == projectId).Select(p => p.Name).FirstOrDefault();
                if (projectName == null)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при приглашении сотрудника на проект", Description = "Проекта уже не нет!" } };
                }
                var teamContainsEmployee = _readonlyDbContext.TeamMembers.Where(tm => tm.EmployeeId == employee.Id && tm.ProjectId == projectId).Count() > 0;
                if (teamContainsEmployee)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при приглашении сотрудника на проект", Description = "Сотрудник уже является членом команды проекта, в которую вы хотите его добавить!" } };
                }
                var teamMember = _readonlyDbContext.TeamMembers.Where(tm => tm.EmployeeId == employeeId && tm.ProjectId == projectId).FirstOrDefault();
                if (teamMember == null)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при приглашении сотрудника на проект", Description = "Вы не являетсь членом команды проекта, в которую вы хотите добавить сотрудника!" } };
                }
                if (teamMember.RoleId != (int)RoleEnum.ProjectManager)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при приглашении сотрудника на проект", Description = "Вы не являетесь проектным менеджером!" } };
                }
                if (employee.RoleId == (int)RoleEnum.ProjectManager)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при приглашении сотрудника на проект", Description = "Пользователь является проектным менеджером!" } };
                }
                var member = new TeamMember { EmployeeId = employee.Id, ProjectId = projectId, RoleId = employee.RoleId };
                var key = KeysHelper.InviteKey(employee.Id, projectId);
                _cache.SetForInvite(key, member);
                var success = await _emailService.SendEmailAsync(email, "Приглашение на проект", GetHtml(projectId, employee.Id, projectName));
                return new OperationResult { Result = new { Success = success } };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Неожиданная ошибка при приглашении на проект(id={0}) сотрудником(id={1})", projectId, employeeId);
                return new OperationResult { Error = new Error { Description = "Неожиданная ошибка при приглашении на проект сотрудника" } };
            }
        }

        /// <summary>
        /// Добавляет пользователя в проект
        /// </summary>
        /// <param name="employeeId">Добавляющий пользователя</param>
        /// <param name="model">Добавляемый пользователь</param>
        /// <returns></returns>
        public OperationResult AddEmployee(int projectId, int employeeId)
        {
            try
            {
                var teamContainsEmployee = _dbContext.TeamMembers.Where(tm => tm.EmployeeId == employeeId && tm.ProjectId == projectId).Count() > 0;
                if (teamContainsEmployee)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при добавлении проект", Description = "Вы уже являетесь членом команды проекта, в которую добавляетсь!" } };
                }
                var key = KeysHelper.InviteKey(employeeId, projectId);
                if (!_cache.TryGetValue(key, out TeamMember newTeamMember))
                {
                    _logger.LogError("Время приглашения истекло либо вовсе не было отослано! (id проекта = {0}, id сотрудника = {1})", projectId, employeeId);
                    return new OperationResult { Error = new Error { Title = "Ошибка добавления на проект", Description = "Время приглашения истекло либо вовсе не было отослано!" } };
                }
                var entity = _dbContext.TeamMembers.Add(newTeamMember);
                if (_dbContext.SaveChanges() > 0)
                {
                    _logger.LogInformation("Сотрудник(id={0}) успешно добавлен в проект(id={1})", employeeId, projectId);
                }
                else
                {
                    _logger.LogWarning("Не удалось добавить сотрудника (id={0}) в проект(id={1})", employeeId, projectId);
                    return new OperationResult { Result = new { Success = false } };
                }
                return new OperationResult { Result = _mapper.Map<TeamMemberDTO>(entity.Entity) };
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Неожиданная ошибка при добавлении на проект(id={0}) сотрудника(id={1})", projectId, employeeId);
                return new OperationResult { Error = new Error { Description = "Неожиданная ошибка при добавлении на проект сотрудника" } };
            }
        }

        public string GetHtml(int projectId, int employeeId, string projectName)
        {
            var issuer = _config.GetSection("AuthOptions").GetValue<string>("Issuer");
            return new StringBuilder()
                .AppendLine("<h1>Здравствуй, Пользователь Task scheduler!</h1>")
                .AppendFormat("<p><img src='https://img.icons8.com/plasticine/100/000000/task.png'/>Чтобы закончить регистрацию в проекте {0}, нажмите на ссылку ", projectName)
                .AppendFormat("<a href='{0}api/project/{1}/employee/{2}'>Принять</a></p>", issuer, projectId, employeeId).ToString();
        }
    }
}
