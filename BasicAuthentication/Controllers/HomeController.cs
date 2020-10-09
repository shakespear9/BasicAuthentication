using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize] /// guard an action = are you allowed ?
        public IActionResult Secret()
        {
            return View();
        } 
        
        [Authorize(Policy = "Claim.DoB")] /// guard an action = are you allowed ?
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        } 
        
        
        [Authorize(Roles = "Admin")] /// guard an action = are you allowed ?
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email ,"Bob@fmail.com"),
                new Claim(ClaimTypes.DateOfBirth ,"11/11/2000"),
                new Claim(ClaimTypes.Role ,"Admin"),
                new Claim("Grandma.Says" ,"Very nice boi.")

            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email , "Bob@fmail.com"),
                new Claim(ClaimTypes.Email , "Bob@fmail.com")
            };

            var grandmaIdentity = new ClaimsIdentity(grandmaClaims,"Grandma Identity");

            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity,licenseIdentity });  //Collection of Identity

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");

        }


        public async Task< IActionResult> DoStuff(
            [FromServices] IAuthorizationService authorizationService)
        {

            //FromService Dependency Injection in Method

            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();

            //await _authorizationService.AuthorizeAsync(User, "Claim.DoB"); //HttpContext.User
            var result = await _authorizationService.AuthorizeAsync(User, customPolicy); //HttpContext.User

            if(result.Succeeded)
            {
                return View("Index");
            }

            return View("Index");
        }

    }
}
