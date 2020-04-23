using APBD05.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APBD05.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginControler : ControllerBase
    {
        private IConfiguration Configuration;
        public LoginControler(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(LoginRequestDTO request)
        {
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
                    salt = dr["Password"].ToString();
                }
                var s = Program.Create(request.Haslo, salt);
                if (!Program.Validate(request.Haslo, salt, password))
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
                com.Parameters.AddWithValue("index", request.Login);
            }


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
