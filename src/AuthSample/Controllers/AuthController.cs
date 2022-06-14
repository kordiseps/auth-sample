using System;
using AuthSample.Models;
using AuthSample.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost(nameof(Login))]
        public IActionResult Login(LoginModel loginModel)
        {
            string userName = loginModel.UserName;
            string password = loginModel.Password;
            try
            {
                return Ok(_authService.Login(userName, password));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("Username or password is not correct");
            }
        }

        [HttpPost(nameof(Register))]
        public IActionResult Register(RegisterModel registerModel)
        {
            string userName = registerModel.UserName;
            string password= registerModel.Password;
            string displayName = registerModel.DisplayName;
            try
            {
                _authService.Register(userName, password, displayName);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("Username or password is not correct");
            }
        }
    }
}