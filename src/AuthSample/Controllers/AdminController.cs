using System;
using AuthSample.Models;
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
        public IActionResult MakeUserAdmin(MakeUserAdminModel makeUserAdminModel)
        {
            string userName = makeUserAdminModel.UserName;
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
        public IActionResult ResetUserPassword(ResetUserPasswordModel resetUserPasswordModel)
        {
            string userName = resetUserPasswordModel.UserName;
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