using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.Crud;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Graduate_Work.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost]
        public IActionResult Register(UserDTO model)
        {
            var registeredUser = _accountService.Register(model);
            if (registeredUser != null)
            {
                var token = _accountService.GenerateToken(registeredUser);
                Response.Cookies.Delete("access_token");
                Response.Cookies.Append("access_token", token);
                var authorizationModel = new AuthorizationModel
                {
                    UserId = registeredUser.Id,
                    Login = registeredUser.Login,
                    Token = _accountService.GenerateToken(registeredUser)
                };
                return Ok(authorizationModel);
            }
            else
            {
                var x = new OperationResult { Error = new Error { Description = "Пользователь с такой электронной почтой уже зарегистрирован!" } };
                return Ok(x);
            }
        }
        [HttpPost("login")]
        public IActionResult Login(UserDTO user)
        {
            var registeredUser = _accountService.GetUser(user.Login, user.Password);
            if (registeredUser != null)
            {
                var token = _accountService.GenerateToken(registeredUser);
                Response.Cookies.Delete("access_token");
                Response.Cookies.Append("access_token", token);
                var model = new AuthorizationModel
                {
                    UserId = registeredUser.Id,
                    Login = registeredUser.Login,
                    Token = token
                };
                return Ok(model);
            }
            else
            {
                var result = new OperationResult { Error = new Error { Description = "Электронная почта или пароль не верные" } };
                return Ok(result);
            }
        }
    }
}