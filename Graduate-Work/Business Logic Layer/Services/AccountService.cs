using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Services.Crud;
using System;
using System.Linq;
using static Business_Logic_Layer.Helpers.ByteHelper;

namespace Business_Logic_Layer.Services
{
    public class AccountService
    {
        private ContextFactory _contextFactory;
        private IMapper mapper;
        private UserService _userService;
        public AccountService(ContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Проверяет, совпадает ли введённый пароль с тем, который хранится в хешированном виде.
        /// </summary>
        /// <param name="inputPassword">Проверяемый пароль</param>
        /// <param name="storedPassword">Пароль, представленный в виде хеша</param>
        /// <param name="storedSalt">Строка, хранимая вместе с паролем (соль)</param>
        /// <returns></returns>
        
        public bool Verify(string inputPassword, string storedPassword, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var anyBytes = Concat(inputPassword, saltBytes);
            var hashAttempt = ComputeHash(anyBytes);
            return storedPassword == hashAttempt;
        }

        public UserDTO Register(UserDTO user)
        {
            var context = _contextFactory.CreateDbContext();
            if (!context.Users.Any(u => u.Login == user.Login))
            {
                var newUser = _userService.CreateUser(user);
               
            }
            return null;
        }


    }
}
