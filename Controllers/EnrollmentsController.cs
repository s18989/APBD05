using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APBD05.DTOs;
using APBD05.DTOs.Requests;
using APBD05.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APBD05.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        public EnrollmentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private IStudentDBService _service;

        public EnrollmentsController(IStudentDBService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudnet(EnrollStudentRequest student)
        {
            return Created("",_service.EnrollStudent(student));
        }
        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudents(string studies, int semester)
        {
            return Created("",_service.PromoteStudents(semester, studies));
        }
    }
}