using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RekvalifikaceApp.Models;
using RekvalifikaceApp.ViewModels;

namespace RekvalifikaceApp.Controllers
{
    /// <summary>
    /// Řadič pro správu účtů uživatelů.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        /// <summary>
        /// Konstruktor třídy AccountController.
        /// </summary>
        /// <param name="userManager">UserManager pro správu uživatelů.</param>
        /// <param name="signInManager">SignInManager pro správu přihlašování uživatelů.</param>
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Zobrazí formulář pro přihlášení uživatele.
        /// </summary>
        /// <param name="returnUrl">Adresa URL pro přesměrování po úspěšném přihlášení.</param>
        /// <returns>ActionResult pro zobrazení formuláře pro přihlášení.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            LoginViewModel loginVM = new LoginViewModel();
            loginVM.ReturnUrl = returnUrl;
            return View(loginVM);
        }

        /// <summary>
        /// Zpracuje POST požadavek pro přihlášení uživatele.
        /// </summary>
        /// <param name="login">Model obsahující údaje pro přihlášení.</param>
        /// <returns>ActionResult pro přesměrování nebo zobrazení formuláře pro přihlášení při chybě.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByNameAsync(login.UserName);
                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(appUser, login.Password, login.Remember, false);
                    if (result.Succeeded)
                    {
                        return Redirect(login.ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(login.UserName), "Login Failed: Invalid UserName or password");
            }
            return View(login);
        }

        /// <summary>
        /// Odhlásí aktuálního uživatele.
        /// </summary>
        /// <returns>ActionResult pro přesměrování na domovskou stránku.</returns>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
