using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.DTO;
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
        [HttpPut("{id}")]
        public IActionResult AddEmployee(int id, TeamMemberDTO model)
        {
            return Ok(_projectService.AddEmployee(id, model));
        }
    }
}