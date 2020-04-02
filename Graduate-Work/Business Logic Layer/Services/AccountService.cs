using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Services.Crud;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using static Business_Logic_Layer.Helpers.ByteHelper;

namespace Business_Logic_Layer.Services
{
    public class AccountService
    {
        private IMapper _mapper;
        private UserService _userService;
        private IConfiguration _configuration;
        private IMemoryCache _cache;

        public AccountService(IMapper mapper, UserService userService, ContextFactory contextFactory, IConfiguration configuration, IMemoryCache cache)
        {
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
            _cache = cache;
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
            var newUser = _userService.CreateUser(user);
            return newUser;
        }

        public string GenerateToken(UserDTO user)
        {
            var identity = GetIdentity(user);

            var now = DateTime.UtcNow;
            var section = _configuration.GetSection("AuthOptions");
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: section["Issuer"],
                    audience: section["Audience"],
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(Convert.ToDouble(section["LifeTime"]))),
                    signingCredentials: new SigningCredentials(GenerateSecurityKey(user), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        private ClaimsIdentity GetIdentity(UserDTO user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login)
            };
            var claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        private SymmetricSecurityKey GenerateSecurityKey(UserDTO user)
        {
            
            return new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes("SymmetricSecurityKey"));
        }
    }
}
