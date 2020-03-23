using Data_Access_Layer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Business_Logic_Layer.Helpers;
using Business_Logic_Layer.DTO;
using System.Linq;

namespace Business_Logic_Layer.Services.Crud
{
    public class UserService : BaseCrudService<UserDTO>
    {
        private ContextFactory _contextFactory;
        public UserService(ContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public override UserDTO Create(UserDTO model)
        {
            throw new NotImplementedException();
        }

        public UserDTO CreateUser(UserDTO user)
        {
            var context = _contextFactory.CreateDbContext();
            if (context.Users.Any(u => u.Login == user.Login))
            {
                var saltBytes = new byte[32];
                new Random().NextBytes(saltBytes);
                var newUser = new User
                {
                    Login = user.Login ?? throw new ArgumentNullException(nameof(user.Login)),
                    Salt = Convert.ToBase64String(saltBytes),
                    Password = user.Password == null ? throw new ArgumentNullException(nameof(user.Password)) : ByteHelper.ComputeHash(ByteHelper.Concat(user.Password, saltBytes))
                };
                context.Users.Add(newUser);
                return context.SaveChanges() > 0 ? EntityHelper.SetId(user, newUser.Id) as UserDTO : null;
            }
            return null;
        }

        public override UserDTO Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override UserDTO Read(int id)
        {
            throw new NotImplementedException();
        }

        public override ICollection<UserDTO> ReadAll()
        {
            throw new NotImplementedException();
        }

        public override UserDTO Update(int id, UserDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
