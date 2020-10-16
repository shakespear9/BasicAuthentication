using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Controllers
{

    // Operation Authorization Requirement
    public class OperationsController : Controller
    {
        private readonly  IAuthorizationService _authorizationService;

        public OperationsController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Open()
        {

            var cookieJar = new CookieJar(); // get cookie jar from DB
            //var requirement = new OperationAuthorizationRequirement()
            //{
            //    Name = CookieJarOpearations.Open
            //};

            await _authorizationService.AuthorizeAsync(User,cookieJar,CookieJarAuthOperations.Open);
            



            return View();
        }
    }

    public class CookieJarAuthorizationHandler 
        : AuthorizationHandler<OperationAuthorizationRequirement,CookieJar>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            CookieJar resource)
        {
            if(requirement.Name == CookieJarOpearations.Look)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }
            else if(requirement.Name == CookieJarOpearations.ComeNear)
            {
                if (context.User.HasClaim("Friend","Good"))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }

    public static class CookieJarAuthOperations
    {

        public static OperationAuthorizationRequirement Open = new OperationAuthorizationRequirement
        {
            Name = CookieJarOpearations.Open
        };
        public static OperationAuthorizationRequirement TakeCookie = new OperationAuthorizationRequirement
        {
            Name = CookieJarOpearations.TakeCookie
        };
        public static OperationAuthorizationRequirement ComeNear = new OperationAuthorizationRequirement
        {
            Name = CookieJarOpearations.ComeNear
        };
        public static OperationAuthorizationRequirement Look = new OperationAuthorizationRequirement
        {
            Name = CookieJarOpearations.Look
        };

 
    }


    public static class CookieJarOpearations
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";
        public static string ComeNear = "ComeNear";
        public static string Look = "Look";


    }
    
    public class CookieJar
    {
        public string Name { get; set; }

    }

}
