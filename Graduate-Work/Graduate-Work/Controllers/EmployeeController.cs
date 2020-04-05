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
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        public IActionResult Read()
        {
            return Ok(_employeeService.ReadAll());
        }

        [HttpGet("{id}")]
        public IActionResult Read(int? id)
        {
            return id.HasValue ? (IActionResult)Ok(_employeeService.Read(id.Value)) : BadRequest();
        }

        [HttpGet("roles")]
        public IActionResult ReadRoles()
        {
            return Ok(_employeeService.GetRoles());
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
    }
}