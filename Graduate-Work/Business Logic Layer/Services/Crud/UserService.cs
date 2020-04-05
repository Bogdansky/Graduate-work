using Data_Access_Layer.Models;
using System;
using Business_Logic_Layer.Helpers;
using Business_Logic_Layer.DTO;
using System.Linq;
using Business_Logic_Layer.Models;
using Data_Access_Layer;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Business_Logic_Layer.Services.Crud
{
    public class UserService : BaseCrudService<UserDTO>
    {

        public UserService(ILogger<UserService> logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {
        }
        public override OperationResult Create(UserDTO model)
        {
            var user = CreateUser(model);
            return user == null ?
                new OperationResult { Error = new Error { Title = "Ошибка при создании пользователя", Description = $"Логин \"{model.Login}\" недоступен" } } : new OperationResult { Result = user };
        }

        public UserDTO CreateUser(UserDTO user)
        {
            try
            {
                var exists = _dbContext.Users.Where(u => u.Login == user.Login).Count() > 0;
                if (!exists)
                {
                    var saltBytes = new byte[32];
                    new Random().NextBytes(saltBytes);
                    var newUser = new User
                    {
                        Login = user.Login ?? throw new ArgumentNullException(nameof(user.Login)),
                        Salt = Convert.ToBase64String(saltBytes),
                        Password = user.Password == null ? throw new ArgumentNullException(nameof(user.Password)) : ByteHelper.ComputeHash(ByteHelper.Concat(user.Password, saltBytes))
                    };
                    var entity = _dbContext.Users.Add(newUser);
                    return _dbContext.SaveChanges() > 0 ? _mapper.Map<UserDTO>(entity.Entity) : null;
                }
                return null;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Неожиданная ошибка при создании пользователя");
                return null;
            }
        }

        public override OperationResult Delete(int id)
        {
            try
            {
                var user = _dbContext.Users.Find(id);
                if (user == null)
                {
                    return new OperationResult
                    {
                        Error = new Error
                        {
                            Title = "Ошибка получения пользователя",
                            Description = "Такого пользователя нет."
                        }
                    };
                }
                using var transaction = _dbContext.Database.BeginTransaction();
                _dbContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                var success = _dbContext.SaveChanges() > 0;
                transaction.Commit();
                return new OperationResult { Result = new { success } };
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при удалении пользователя";
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
                var user = _readonlyDbContext.Users.Find(id);
                if (user == null)
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
                _logger.LogInformation("Получен пользователь с id={0}", id);
                return new OperationResult
                {
                    Result = _mapper.Map<UserDTO>(user)
                };
            }
            catch (Exception e)
            {
                var errorText = "При получении пользователя произошла неожиданная ошибка.";
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
            var users = _readonlyDbContext.Users.ToList();
            return new OperationResult { Result = _mapper.Map<List<UserDTO>>(users) };
        }

        public OperationResult ReadAll(int projectId)
        {
            var users = _readonlyDbContext.TeamMembers.Where(t => t.ProjectId == projectId).Select(t => t.Employee.User).ToList();
            return new OperationResult { Result = _mapper.Map<List<UserDTO>>(users) };
        }

        public override OperationResult Update(int id, UserDTO model)
        {
            OperationResult result = new OperationResult();
            try
            {
                var exists = _dbContext.Users.Any(u => u.Id == id);
                if (!exists)
                {
                    result.Error = new Error { Title = "Ошибка при обновлении", Description = "Такого пользователя не существует!" };
                }
                else
                {
                    var user = _mapper.Map<User>(model);
                    using var transaction = _dbContext.Database.BeginTransaction();
                    var entity = _dbContext.Users.Update(user);
                    result.Result = _mapper.Map<UserDTO>(entity.Entity);
                }
            }
            catch (Exception e)
            {
                var originMessage = "Неожиданная ошибка при обновлении пользователя";
                _logger.LogError(e, "{0} c id = {1}", originMessage, id);
                result.Error = new Error { Description = originMessage };
            }
            return result;
        }
    }
}
