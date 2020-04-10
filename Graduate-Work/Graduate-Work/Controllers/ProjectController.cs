using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Services.Crud;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Graduate_Work.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("{id}")]
        public IActionResult Create([FromRoute]int id, ProjectDTO model)
        {
            var result = _projectService.Create(id, model);
            return Ok(result);
        }
        [HttpPut("{projectId}/employee/{employeeId}")]
        public async Task<IActionResult> InviteEmployee(int projectId, int employeeId, UserDTO model)
        {
            var res = await _projectService.InviteUser(projectId, employeeId, model.Login);
            return Ok(res);
        }

        [HttpGet("{projectId}/employee/{employeeId}")]
        public IActionResult AddEmployee(int projectId, int employeeId)
        {   
            return Ok(_projectService.AddEmployee(projectId, employeeId));
        }

        [HttpGet]
        public IActionResult ReadAll([FromQuery]ProjectFilter filter)
        {
            var res = _projectService.ReadAll(filter);
            return Ok(res);
        }

        [HttpGet("{projectId}/employees")]
        public IActionResult ReadAllEmployees(int projectId)
        {
            var res = _projectService.ReadAllEmployees(projectId);
            return Ok(res);
        }
    }
}