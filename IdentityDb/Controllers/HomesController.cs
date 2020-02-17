using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IdentityDb.Controllers
{
    public class HomesController : Controller
    {
        private readonly UserManager<IdentityUser> _manager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        public HomesController(UserManager<IdentityUser> manager, SignInManager<IdentityUser> signInManager, IEmailService emailService)
        {
            _emailService = emailService;
            _signInManager = signInManager;
            _manager = manager;


        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
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

            var user = await _manager.FindByNameAsync(username);
            if (user != null)
            {
                // sign in 
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
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
            var user = new IdentityUser
            {
                UserName = username,
                Email = ""
            };

            var result = await _manager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                //generation of the email token
                var code = await _manager.GenerateEmailConfirmationTokenAsync(user);

                var link = Url.Action(nameof(VerifyEmail), "Homes", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());

                await _emailService.SendAsync("test@test.com", "email verify", $"<a href={link}>Verify Email</a>", true);

                return RedirectToAction("EmailVerification");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            ViewBag.Message = userId;
            var user = await _manager.FindByIdAsync(userId);
            if(user == null) return BadRequest();
            var result = await _manager.ConfirmEmailAsync(user, code);

            if(result.Succeeded)
            {
                return View();
            }
            return BadRequest();
        }
        public IActionResult EmailVerification() => View();
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}