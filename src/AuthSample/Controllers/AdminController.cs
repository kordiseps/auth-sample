using System;
using AuthSample.Services;
using AuthSample.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AuthSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [HeaderAuthorize]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        
        [HttpPost(nameof(MakeUserAdmin))]
        public IActionResult MakeUserAdmin(string userName)
        {
            try
            {
                _authService.MakeUserAdmin(userName);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        
        [HttpPost(nameof(ResetUserPassword))]
        public IActionResult ResetUserPassword(string userName)
        {
            try
            {
                _authService.ResetUserPassword(userName);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}