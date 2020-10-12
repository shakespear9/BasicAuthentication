using IdentityExample.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {

        //private readonly UserManager<IdentityUser> _userManager;
        //private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<User> _roleManager;

        public HomeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,RoleManager<User> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            /// login functionality

           

            var user = await _userManager.FindByNameAsync(username);


            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user,password,false,false);

                if (signInResult.Succeeded)
                    return RedirectToAction("Index");
                
            }

            return RedirectToAction("Index");
        }


        public IActionResult Register()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            /// register functionality
            //var user = new IdentityUser
            //{
            //    UserName = username,
            //    Email = ""
            //};

            var user = new User
            {
                UserName = username,
                FirstName = "Nuttakorn",
                LastName = "Tedthong"
                //password = password

            };


            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {

                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (signInResult.Succeeded)
                    return RedirectToAction("Index");

                /// sign user here
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

    }
}
