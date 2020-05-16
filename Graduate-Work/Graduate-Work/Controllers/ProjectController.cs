using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Services.Crud;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Transport_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly IConfiguration _config;
        public ProjectController(ProjectService projectService, IConfiguration config)
        {
            _projectService = projectService;
            _config = config;
        }

        [HttpPost("{id}")]
        public IActionResult Create([FromRoute]int id, ProjectDTO model)
        {
            try
            {
                var result = _projectService.Create(id, model);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut("{projectId}/employee/{employeeId}")]
        public async Task<IActionResult> InviteEmployee(int projectId, int employeeId, EmployeeDTO invitedEmployee)
        {
            try
            {
                var res = await _projectService.InviteUser(projectId, employeeId, invitedEmployee);
                return Ok(res);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{projectId}/employee/{employeeId}")]
        public IActionResult AddEmployee(int projectId, int employeeId)
        {
            try
            {
                _ = _projectService.AddEmployee(projectId, employeeId);
                var audience = _config.GetSection("AuthOptions").GetValue<string>("Audience");
                return Redirect(audience);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult ReadAll([FromQuery]ProjectFilter filter)
        {
            try
            {
                var res = _projectService.ReadAll(filter);
                return Ok(res);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{projectId}/employees")]
        public async Task<IActionResult> ReadAllEmployees(int projectId)
        {
            try
            {
                var res = await _projectService.ReadAllEmployees(projectId);
                return Ok(res);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}