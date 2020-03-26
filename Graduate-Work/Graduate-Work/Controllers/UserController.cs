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
using Microsoft.Extensions.Logging;

namespace Graduate_Work.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private AccountService _accountService;
        private UserService _userService;
        private ILogger _logger;

        public UserController(AccountService accountService, UserService userService, ILogger logger)
        {
            _accountService = accountService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public OperationResult GetUser(int id)
        {
            try
            {
                var user = _userService.Read(id);
                _logger.LogInformation("User read successfully");
                return new OperationResult { Result = user };
            }
            catch (Exception e)
            {
                return new OperationResult { Error = new Error { Description = e.Message } };
            }
        }

        [HttpPost]
        public UserDTO Register(UserDTO user)
        {
            try
            {
                var newUser = _accountService.Register(user);
                _logger.LogInformation($"User {newUser.Login} was register");
                return newUser;
            }
            catch
            {
                return null;
            }
        }
    }
}