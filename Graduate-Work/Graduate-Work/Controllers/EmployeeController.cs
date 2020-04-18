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
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("{id}")]
        public IActionResult Read(int? id)
        {
            return id.HasValue ? (IActionResult)Ok(_employeeService.ReadByUserId(id.Value)) : BadRequest();
        }

        [HttpGet("enums")]
        public IActionResult ReadEnums()
        {
            return Ok(_employeeService.GetEnums());
        }

        [HttpPost]
        public IActionResult Create(EmployeeDTO model)
        {
            return Ok(_employeeService.Create(model));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int? id, EmployeeDTO model)
        {
            return id.HasValue ? (IActionResult)Ok(_employeeService.Update(id.Value, model)) : BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int? id)
        {
            return id.HasValue ? (IActionResult)Ok(_employeeService.Delete(id.Value)) : BadRequest(); ;
        }

        [HttpGet("{id}/projects")]
        public IActionResult GetEmployeeProjects(int id)
        {
            return id != 0 ? Ok(_employeeService.ReadProjects(id)) : (IActionResult)BadRequest();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchForEmployees([FromQuery]EmployeeFilter filter)
        {
            return Ok(await _employeeService.SearchAsync(filter));
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> ReadTimeStatistics(int id)
        { 
            try
            {
                var stat = await _employeeService.GetTaskTimeStatisticsAsync(id);
                return Ok(stat);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}