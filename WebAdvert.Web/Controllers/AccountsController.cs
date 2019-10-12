using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;
        public AccountsController(SignInManager<CognitoUser> singInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = singInManager;
            _userManager = userManager;
            _pool = pool;

        }
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _pool.GetUser(model.Email);
            if (user != null)
            {
                ModelState.AddModelError("UserExists", "User with this email already exists");
                return View(model);
            }

            user.Attributes.Add(CognitoAttribute.Name.ToString(), model.Email);
            var userCreated =  await _userManager.CreateAsync(user, model.Password);
            if (userCreated.Succeeded)
            {
              return  RedirectToAction("Confirm");
            }
            return View(model);
        }

        public async Task<IActionResult> Confirm()
        {
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                ModelState.AddModelError("NotFound","A user with the given email address was not found");
                return View(model);
            }

            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }

                return View(model);
            }


        }

        public async Task<IActionResult> Login()
        {
            var model = new LoginModel();
            return View(model);
        }
        
        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var restult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false).ConfigureAwait(false);
                if (restult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("LoginError","Email and password do not match");
                }


            }
            return View(model);
        }
    }
}