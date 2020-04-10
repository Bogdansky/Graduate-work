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
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public IActionResult Create(TaskDTO model)
        {
            try
            {
                return Ok(_taskService.Create(model));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Read(int id)
        {
            return id == 0 ? Ok(_taskService.Read(id)) : (IActionResult)BadRequest();
        }

        [HttpGet("{employeeId}/filter")]
        public IActionResult ReadAll(int employeeId, [FromQuery]TaskFilterTypes filter, [FromQuery]int? projectId)
        {
            try
            {
                var result = _taskService.ReadAll(new TaskFilter { EmployeeId = employeeId, TaskFilterType = filter, ProjectId = projectId });
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}