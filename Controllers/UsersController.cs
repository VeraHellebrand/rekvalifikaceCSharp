using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RekvalifikaceApp.Models;
using RekvalifikaceApp.ViewModels;
using System.Threading.Tasks;

namespace RekvalifikaceApp.Controllers
{
    /// <summary>
    /// Controller pro správu uživatelů.
    /// </summary>
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Zobrazí seznam všech uživatelů.
        /// </summary>
        /// <returns>View s seznamem uživatelů.</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public IActionResult Index()
        {
            var sortedUsers = _userManager.Users.OrderBy(u => u.UserName).ToList();

            return View(sortedUsers);
        }

        /// <summary>
        /// Zobrazí formulář pro vytvoření nového uživatele.
        /// </summary>
        /// <returns>Formulář pro vytvoření nového uživatele.</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Zpracuje vytvoření nového uživatele.
        /// </summary>
        /// <param name="createUserViewModel">Data pro vytvoření nového uživatele.</param>
        /// <returns>View s formulářem pro vytvoření nového uživatele nebo přesměrování na Index, pokud vytvoření proběhlo úspěšně.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> CreateAsync(CreateUserViewModel createUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUser
                {
                    UserName = createUserViewModel.UserName,
                    Email = createUserViewModel.Email,
                };
                var result = await _userManager.CreateAsync(newUser, createUserViewModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "Chyba při zpracování formuláře. Zkontrolujte, zda jsou všechna pole správně vyplněna.");
            }

            return View(createUserViewModel);
        }

        /// <summary>
        /// Zobrazí formulář pro úpravu uživatele.
        /// </summary>
        /// <param name="id">ID uživatele k úpravě.</param>
        /// <returns>Formulář pro úpravu uživatele nebo NotFound, pokud uživatel s daným ID neexistuje.</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> Edit(string id)
        {
            var userToEdit = await _userManager.FindByIdAsync(id);
            if (userToEdit == null)
            {
                ModelState.AddModelError("", "Uživatel nebyl nalezen.");
                return View("NotFound");
            }
            var userViewModel = new EditUserViewModel
            {
                Id = userToEdit.Id,
                UserName = userToEdit.UserName ?? string.Empty,
                Email = userToEdit.Email ?? string.Empty
            };

            return View(userViewModel);
        }

        /// <summary>
        /// Zpracuje úpravu uživatele.
        /// </summary>
        /// <param name="editUserViewModel">Data pro úpravu uživatele.</param>
        /// <returns>View s formulářem pro úpravu uživatele nebo přesměrování na Index, pokud úprava proběhla úspěšně.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(EditUserViewModel editUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Chyba při zpracování formuláře. Zkontrolujte, zda jsou všechna pole správně vyplněna.");
                return View(editUserViewModel);
            }

            var user = await _userManager.FindByIdAsync(editUserViewModel.Id);
            if (user == null)
            {
                ModelState.AddModelError("", "Uživatel nebyl nalezen.");
                return View("NotFound");
            }

            user.UserName = editUserViewModel.UserName;
            user.Email = editUserViewModel.Email;

            // Změna hesla, pokud je vyplněno
            if (!string.IsNullOrEmpty(editUserViewModel.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordChangeResult = await _userManager.ResetPasswordAsync(user, token, editUserViewModel.Password);

                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(editUserViewModel);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(editUserViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Zpracuje smazání uživatele.
        /// </summary>
        /// <param name="id">ID uživatele k smazání.</param>
        /// <returns>Přesměrování na Index, pokud smazání proběhlo úspěšně, jinak vrátí chybovou zprávu.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id);

            if (userToDelete == null)
            {
                ModelState.AddModelError("", "Uživatel nebyl nalezen.");
                return RedirectToAction("NotFound", "Error");
            }

            var result = await _userManager.DeleteAsync(userToDelete);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("Index", _userManager.Users);
        }
    }
}
