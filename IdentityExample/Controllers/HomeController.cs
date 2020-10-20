using IdentityExample.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
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
        private readonly IEmailService _emailService;

        //private readonly RoleManager<User> _roleManager;

        public HomeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;   
            //_roleManager = roleManager;
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

            //await _userManager.CreateAsync(user);

            //await _userManager.AddPasswordAsync(user, password);


            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {

                // this line of codes using for no Email Confirmation
                //var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                //if (signInResult.Succeeded)
                //    return RedirectToAction("Index");

                // sign user here

                ///*************************************************************************************************///
                //generation of the email token or confirmation token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

             

                var link = Url.Action(nameof(VerifyEmail),"Home",new { userId = user.Id, code },Request.Scheme,Request.Host.ToString());

                await _emailService.SendAsync($"{user.UserName}@test.com","email verify",$"<a href=\"{link}\">Verify Email</a>",true);

                //_userManager.ConfirmEmailAsync(user, code);  // use for confirm emial

                return RedirectToAction("EmailVerification");




            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> VerifyEmail(string userId,string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return BadRequest();

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {

                return View();

            }

            return BadRequest();

        }

        public IActionResult EmailVerification() => View();


        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

    }
}
