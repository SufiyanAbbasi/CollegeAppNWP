using CollegeApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
         _configuration = configuration;
        }
        [HttpPost]
        public ActionResult Login(LoginDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Please provide username and password");
            }
            LoginResponseDTO response = new() { Username = model.Username};
            if (model.Username == "Venkat" && model.Password == "Venkat123")
            {
                var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecret"));
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        //username
                        new Claim(ClaimTypes.Name, model.Username),

                        //role
                        new Claim(ClaimTypes.Role, "Admin"),
                    }),
                    Expires = DateTime.Now.AddHours(4),
                    SigningCredentials = new (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };
                var token = tokenhandler.CreateToken(tokenDescriptor);
                response.token = tokenhandler.WriteToken(token);
            }
            else
            {
                return Ok("Invalid Credentials");
            }
            return Ok(response);
        }
    }

 


}
