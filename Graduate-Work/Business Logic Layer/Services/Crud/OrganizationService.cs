using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Helpers;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business_Logic_Layer.Services.Crud
{
    public class OrganizationService : BaseCrudService<OrganizationDTO>
    {
        private readonly MemoryCache _cache;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        const string EmployeeKey = "user_{0}";
        public OrganizationService(ILogger<OrganizationService> logger, IMapper mapper, ContextFactory contextFactory, MemoryCache cache, IConfiguration config, EmailService emailService) : base(logger, mapper, contextFactory)
        {
            _cache = cache;
            _config = config;
            _emailService = emailService;
        }

        public OperationResult Create(int userId, OrganizationDTO model)
        {
            var user = _readonlyDbContext.Users.Find(userId);
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
                var exists = _dbContext.Organizations.Where(o => o.Name == model.Name).Count() > 0;
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
                    return new OperationResult { Result = new { Success = false } };
                }
                else
                {
                    _logger.LogInformation("Организация \"{0}\" успешно сохранено", model.Name);
                    return new OperationResult { Result = new { Success = true } };
                }
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
            try
            {
                var organization = _dbContext.Organizations.Find(id);
                if (organization == null)
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
                _dbContext.Entry(organization).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                var success = _dbContext.SaveChanges() > 0;
                transaction.Commit();
                return new OperationResult { Result = new { success } };
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при удалении организации";
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
            return new OperationResult { Result = _readonlyDbContext.Organizations.Select(o => new { o.Name, o.Id })};
        }

        public override OperationResult Update(int id, OrganizationDTO model)
        {
            OperationResult result = new OperationResult();
            try
            {
                var exists = _dbContext.Organizations.Where(p => p.Id == id).Count() > 0;
                if (!exists)
                {
                    result.Error = new Error { Title = "Ошибка при обновлении", Description = "Такой организации не существует!" };
                }
                else
                {
                    var organization = _mapper.Map<Organization>(model);
                    using var transaction = _dbContext.Database.BeginTransaction();
                    var entity = _dbContext.Organizations.Update(organization);
                    _dbContext.SaveChanges();
                    result.Result = _mapper.Map<OrganizationDTO>(entity.Entity);
                }
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при обновлении организации";
                _logger.LogError(e, "{0} \"{1}\"-новое имя c id = {2}", originMessage, model.Name, id);
                result.Error = new Error { Description = originMessage };
            }
            return result;
        }

        public async OperationResult InviteUser(int organizationId, int userId)
        {
            try
            {
                var exists = _dbContext.Employees.Where(e => e.UserId == userId).Select(e => new { e.OrganizationId, e.UserId}).FirstOrDefault();

                if (exists != null)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при приглашении пользователя", Description = exists.OrganizationId == organizationId ? "Пользователь уже состоит в данной организации" : "Пользователь уже состоит в одной из организаций" } };
                }
                var name = _dbContext.Organizations.Where(o => o.Id == organizationId).Select(o => new { o.Id, o.Name }).FirstOrDefault();
                if (name == null)
                {
                    return new OperationResult { Error = new Error { Title = "Ошибка при приглашении пользователя", Description = "Такой организации нет" } };
                }
                await _emailService.SendEmailAsync(GetHtml(userId, name.Name));
                return new OperationResult { Result = new { Success = true } };
            }
            catch(Exception e)
            {
                var errorText = "При приглашении сотрудника произошла неожиданная ошибка.";
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

        public OperationResult AddEmployee(int userId)
        {
            try
            {
                var exists = _dbContext.Users.Where(u => u.Id == userId).Count() > 0;
                if (exists)
                {
                    return new OperationResult { Error = { Title = "Ошибка регистрации сотрудника", Description = "Такого пользователя нет" } };
                }
                _cache.TryGetValue(string.Format(EmployeeKey, userId), out EmployeeDTO employee);
            }
            catch(Exception e)
            {
                var errorText = "При добавлении сотрудника произошла неожиданная ошибка.";
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

        public string GetHtml(int userId, string organizationName)
        {
            var issuer = _config.GetSection("AuthOptions").GetValue<string>("Issuer");
            return new StringBuilder()
                .AppendLine("<h1>Здравствуй, Пользователь!</h1>")
                .AppendFormat("Пройдите по ссылке, чтобы принять приглашение на членство в организации {0}:<br/>", organizationName)
                .AppendFormat("{0}/api/organization/user/{1}", organizationName, userId).ToString();
        }
    }
}
