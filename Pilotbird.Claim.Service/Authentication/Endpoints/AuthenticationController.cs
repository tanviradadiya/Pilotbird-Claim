using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pilotbird.Claim.Service.Authentication.Commands;
using Pilotbird.Claim.Service.Authentication.Requests;
using Pilotbird.Claim.Service.Controllers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using static Common.Enums.IdentityEnums;

namespace Pilotbird.Claim.Service.Authentication.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApiControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AuthenticationController(IMediator mediator, IConfiguration config, IWebHostEnvironment hostEnvironment) : base(mediator)
        {
            _config = config;
            _hostEnvironment = hostEnvironment;
        }

        //#region GenerateJWT  
        //[Route("GenerateJSONWebToken")]
        //[HttpGet]
        //public string GenerateJSONWebToken(LoginRequest loginRequest)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(_config["Jwt:Issuer"],
        //      _config["Jwt:Issuer"],
        //      null,
        //      expires: DateTime.Now.AddMinutes(120),
        //      signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
        //#endregion

        #region AuthenticateUser 
        //private async Task<LoginRequest> AuthenticateUser(LoginRequest login)
        //{
        //    LoginRequest user = null;
        //    if (login.Email != null)
        //        user = new LoginRequest { Email = "Test@Yahoo.com", Password = "111111", IPAddress = "117.217.126.34", TimeZone = "India Standard Time" };
        //    return user;
        //}
        #endregion

        #region Login 
        //[AllowAnonymous]
        //[Route("Login")]
        //[HttpPost]
        //public async Task<IActionResult> Login([FromBody] LoginRequest data)
        //{
        //    IActionResult response = Unauthorized();
        //    var user = await AuthenticateUser(data);
        //    if (data != null)
        //    {
        //        //var tokenString = GenerateJSONWebToken(user);
        //        response = Ok(new { user, Message = "Success" });
        //    }
        //    return response;
        //}
        #endregion

        #region GetAcessToken  
        ///// <summary>  
        ///// Authorize the Method  
        ///// </summary>  
        ///// <returns></returns>  
        //[Route("GetAcessToken")]
        //[HttpGet]
        //public async Task<IEnumerable<string>> GetAcessToken()
        //{
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    return new string[] { accessToken };
        //}
        //#endregion

        //#region JsonProperties  
        ///// <summary>  
        ///// Json Properties  
        ///// </summary>  
        //public class LoginRequest
        //{
        //    public string Email { get; set; }
        //    public string Password { get; set; }
        //    public string IPAddress { get; set; }
        //    public string TimeZone { get; set; }
        //}
        #endregion

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginModel)
        {
            var loginResult = await Send(new LogInApplicationAutheticationCommand(loginModel.Email, loginModel.Password, loginModel.IPAddress));
            if (loginResult.LoginStatus == LoginStatus.Success)
            {
                return (IActionResult)loginResult;
            }
            return (IActionResult)loginResult; 
        }

    }
}
