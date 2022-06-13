using System;
using System.Linq;
using AuthSample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthSample.Utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HeaderAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private IAuthService authService;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (authService is null)
            {
                authService = (IAuthService)context.HttpContext.RequestServices.GetService(typeof(IAuthService));
            }

            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token is null)
            {
                context.Result = new UnauthorizedResult();
            }

            try
            {
                var doesHavePermission = authService.DoesHavePermission(token,context.HttpContext.Request.Path);
                if (doesHavePermission)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Header authorize resolve error: " + ex.Message);
                context.Result = new UnauthorizedResult();
            }
        }
    }
}