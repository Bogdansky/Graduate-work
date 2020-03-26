using Data_Access_Layer.Models;
using System;
using Business_Logic_Layer.Helpers;
using Business_Logic_Layer.DTO;
using System.Linq;
using Business_Logic_Layer.Models;
using Data_Access_Layer;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Business_Logic_Layer.Services.Crud
{
    public class UserService : BaseCrudService<UserDTO>
    {
        private Context _dbContext;
        private Context _readonlyDbContext;
        public UserService(ILogger<UserService> logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper)
        {
            _dbContext = contextFactory.CreateDbContext();
            _readonlyDbContext = contextFactory.CreateReadonlyDbContext();
        }
        public override OperationResult Create(UserDTO model)
        {
            throw new NotImplementedException();
        }

        public UserDTO CreateUser(UserDTO user)
        {
            if (_dbContext.Users.Any(u => u.Login == user.Login))
            {
                var saltBytes = new byte[32];
                new Random().NextBytes(saltBytes);
                var newUser = new User
                {
                    Login = user.Login ?? throw new ArgumentNullException(nameof(user.Login)),
                    Salt = Convert.ToBase64String(saltBytes),
                    Password = user.Password == null ? throw new ArgumentNullException(nameof(user.Password)) : ByteHelper.ComputeHash(ByteHelper.Concat(user.Password, saltBytes))
                };
                _dbContext.Users.Add(newUser);
                return _dbContext.SaveChanges() > 0 ? EntityHelper.SetId(user, newUser.Id) as UserDTO : null;
            }
            return null;
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

        public override OperationResult Update(int id, UserDTO model)
        {
            throw new NotImplementedException();
        }
        public override void Dispose()
        {
            _dbContext.Dispose();
            _readonlyDbContext.Dispose();
        }
    }
}
