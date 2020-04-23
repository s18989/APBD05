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

        [HttpPost]
        public IActionResult Login(LoginRequestDTO request)
        {
            var index = String.Empty;
            var refreshToken = Guid.NewGuid();

            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s18989;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "SELECT Password, Salt FROM Student WHERE IndexNumber = @index";
                com.Parameters.AddWithValue("index", request.Login);

                var salt = String.Empty;
                var password = String.Empty;

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    throw new Exception();
                }
                else
                {
                    password = dr["Password"].ToString();
                    index = dr["IndexNumber"].ToString();
                    salt = dr["Password"].ToString();
                }
                var s = Program.Create(request.Haslo, salt);
                if (!Program.Validate(request.Haslo,salt,password))
                {
                    throw new Exception();
                }

            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "student"),
                new Claim(ClaimTypes.Role, "employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
                );

            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s18989;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "INSERT INTO Student (RefreshToken) VALUES (@refreshToken) WHERE IndexNumber = @index";
                com.Parameters.AddWithValue("refreshToken", refreshToken);
                com.Parameters.AddWithValue("index", index);
            }

            Console.WriteLine(refreshToken);
            Console.WriteLine(Program.Create("s1234", Program.CreateSalt()));


            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken
            }); ;

        }

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string refToken)
        {
            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s18989;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "SELECT RefreshToken FROM Student WHERE RefreshToken = @refreshToken";
                com.Parameters.AddWithValue("refreshToken", refToken);

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    throw new Exception();
                } 
                else
                {
                    var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "student"),
                    new Claim(ClaimTypes.Role, "employee")
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "Gakko",
                        audience: "Students",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(10),
                        signingCredentials: creds
                        );

                    return Ok(new
                    {
                        accessToken = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }

            }
        }
    }
}