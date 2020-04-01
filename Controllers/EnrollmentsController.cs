using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD05.DTOs.Requests;
using APBD05.DTOs.Responses;
using APBD05.Models;
using APBD05.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD05.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDBService _service;

        public EnrollmentsController(IStudentDBService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult EnrollStudnet(EnrollStudentRequest student)
        {
            return Created("",_service.EnrollStudent(student));
        }
        [HttpPost("promotions")]
        public IActionResult PromoteStudents(string studies, int semester)
        {
            return Created("",_service.PromoteStudents(semester, studies));
        }
    }
}