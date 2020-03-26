using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Services;
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
        public AuthorizationModel Register(UserDTO model)
        {
            var registeredUser = _accountService.Register(model);
            if (registeredUser != null)
            {
                return new AuthorizationModel
                {
                    UserId = registeredUser.Id,
                    Login = registeredUser.Login,
                    Token = _accountService.GenerateToken(registeredUser)
                };
            }
            Response.StatusCode = 401;
            return null;
        }
    }
}