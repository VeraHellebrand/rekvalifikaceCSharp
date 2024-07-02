using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RekvalifikaceApp.Models;
using RekvalifikaceApp.ViewModels;

namespace RekvalifikaceApp.Controllers
{
    /// <summary>
    /// Controller pro správu rolí.
    /// </summary>
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }



        /// <summary>
        /// GET Zobrazí seznam všech rolí.
        /// </summary>
        /// <returns>View s seznamem rolí.</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            if (User.IsInRole("SuperAdmin"))
            {
                return View(roles);
            }
            else
            {
                // Filtruje roli SuperAdmin pro Adminy
                roles = roles.Where(r => r.Name != "SuperAdmin").ToList();
                return View(roles);
            }
        }

        /// <summary>
        /// GET CREATEZobrazí formulář pro vytvoření nové role.
        /// </summary>
        /// <returns>Formulář pro vytvoření nové role.</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST CREATE Zpracuje vytvoření nové role.
        /// </summary>
        /// <param name="name">String se jménem role.</param>
        /// <returns>View s formulářem pro vytvoření nové role nebo přesměrování na Index, pokud vytvoření proběhlo úspěšně.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> CreateAsync(string name)
        {
            if (ModelState.IsValid)
            {

                var result = await _roleManager.CreateAsync(new IdentityRole(name));

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                AddErrors(result);
            }
            else
            {
                ModelState.AddModelError("", "Chyba při zpracování formuláře. Zkontrolujte, zda jsou všechna pole správně vyplněna.");
            }

            return View(name);
        }

        /// <summary>
        /// GET EDIT Zobrazí formulář pro úpravu role.
        /// </summary>
        /// <param name="id">id role k úpravě.</param>
        /// <returns>Formulář pro úpravu role nebo NotFound, pokud uživatel s daným ID neexistuje.</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> Edit(string id)
        {
            var roleToEdit = await _roleManager.FindByIdAsync(id);

            if (roleToEdit == null || roleToEdit.Name == null)
            {
                ModelState.AddModelError("", "Role nebyla nalezena nebo její jméno je neplatné.");
                return View("NotFound");
            }

            if (roleToEdit.Name == "SuperAdmin" && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            var members = new List<AppUser>();
            var nonmembers = new List<AppUser>();

            foreach (var user in users.OrderBy(u => u.UserName))
            {
                if (await _userManager.IsInRoleAsync(user, roleToEdit.Name))
                {
                    members.Add(user);
                }
                else
                {
                    nonmembers.Add(user);
                }
            }

            return View(new RoleEdit
            {
                Role = roleToEdit,
                RoleMembers = members,
                RoleNonMembers = nonmembers
            });
        }

        /// <summary>
        /// POST EDIT Zpracuje úpravu rolí.
        /// </summary>
        /// <param name="editUserViewModel">Data pro úpravu uživatele.</param>
        /// <returns>View s formulářem pro úpravu uživatele nebo přesměrování na Index, pokud úprava proběhla úspěšně.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(RoleModifications roleModifications)
        {
            foreach (var id in roleModifications.AddIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, roleModifications.RoleName);
                    if (result != IdentityResult.Success)
                    {
                        AddErrors(result);
                    }

                }
            }
            foreach (var id in roleModifications.DeleteIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, roleModifications.RoleName);
                    if (result != IdentityResult.Success)
                    {
                        AddErrors(result);
                    }

                }
            }

            return RedirectToAction("Index");           
        }

        /// <summary>
        /// POST DELETE Zpracuje smazání role.
        /// </summary>
        /// <param name="id">ID role k smazání.</param>
        /// <returns>Přesměrování na Index, pokud smazání proběhlo úspěšně, jinak vrátí chybovou zprávu.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
            {
                ModelState.AddModelError("", "Role nebyla nalezena.");
                return RedirectToAction("NotFound", "Error");
            }

            if (roleToDelete.Name == "SuperAdmin" && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("Index", _roleManager.Roles);
        }

        /// <summary>
        /// Přidá chyby z IdentityResult do ModelState pro zobrazení ve view.
        /// </summary>
        /// <param name="result">IdentityResult obsahující chyby k přidání.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
