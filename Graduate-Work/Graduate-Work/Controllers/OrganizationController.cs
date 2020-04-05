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
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationService _organizationService;
        public OrganizationController(OrganizationService organizationService)
        {
            _organizationService = organizationService;
        }
        
        [HttpGet]
        public IActionResult Read()
        {
            return Ok(_organizationService.ReadAll());
        }

        [HttpGet("{id}")]
        public IActionResult Read(int? id)
        {
            return id.HasValue ? (IActionResult)Ok(_organizationService.Read(id.Value)) : BadRequest() ;
        }

        [HttpPost]
        public IActionResult Create(OrganizationDTO model)
        {
            return Ok(_organizationService.Create(model));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int? id, OrganizationDTO model)
        {
            return id.HasValue ? (IActionResult)Ok(_organizationService.Update(id.Value, model)) : BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int? id)
        {
            return id.HasValue ? (IActionResult)Ok(_organizationService.Delete(id.Value)) : BadRequest(); ;
        }
    }
}