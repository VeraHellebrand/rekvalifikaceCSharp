using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Services;

namespace RekvalifikaceApp.Controllers
{
    public class TeachersController : Controller
    {
        private readonly TeacherService _teacherService;

        public TeachersController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        /// <summary>
        /// GET Zobrazí seznam všech učitelů DTO.
        /// </summary>
        /// <returns>ActionResult pro zobrazení seznamu učitelů</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            IEnumerable<TeacherDto> allTeachers = await _teacherService.GetTeachersAsync();
            return View(allTeachers);
        }

        /// <summary>
        /// GET Zobrazí detaily konkrétního učitele podle ID.
        /// </summary>
        /// <param name="id">ID učitele</param>
        /// <returns>ActionResult pro zobrazení detailů učitele</returns>
        [HttpGet]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            return await GetTeacherViewByIdAsync(id);
        }
        /// <summary>
        /// GET CREATE Zobrazí formulář pro vytvoření nového učitele.
        /// </summary>
        /// <returns>ActionResult pro zobrazení formuláře pro vytvoření učitele</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST CREATE Zpracuje POST požadavek pro vytvoření nového učitele.
        /// </summary>
        /// <param name="teacherDto">DTO objekt obsahující informace o novém učiteli</param>
        /// <returns>ActionResult pro přesměrování na akci Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> CreateAsync(TeacherDto teacherDto)
        {
            await _teacherService.AddTeacherAsync(teacherDto);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// GET EDIT Zobrazí formulář pro editaci učitele podle ID.
        /// </summary>
        /// <param name="id">ID učitele</param>
        /// <returns>ActionResult pro zobrazení formuláře pro editaci učitele</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(int? id)
        {
            return await GetTeacherViewByIdAsync(id);
        }

        /// <summary>
        /// POST EDIT Zpracuje POST požadavek pro editaci učitele podle ID.
        /// </summary>
        /// <param name="id">ID učitele</param>
        /// <param name="teacherDto">DTO objekt obsahující aktualizované informace o učiteli</param>
        /// <returns>ActionResult pro přesměrování na akci Index, pokud je editace úspěšná</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(int id, TeacherDto teacherDto)
        {
            if (id != teacherDto.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _teacherService.UpdateTeacherAsync(id, teacherDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _teacherService.TeacherExistsAsync(teacherDto.Id))
                    {
                        return View("NotFound");
                    }
                    ModelState.AddModelError(string.Empty, "Došlo k problému s konkurenční aktualizací. Zkuste akci znovu.");
                    return View(teacherDto);
                }
                
            }
            return View(teacherDto);
        }

        /// <summary>
        /// GET DELETE Zobrazí formulář pro potvrzení smazání učitele podle ID.
        /// </summary>
        /// <param name="id">ID učitele</param>
        /// <returns>ActionResult pro zobrazení formuláře pro potvrzení smazání učitele</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            return await GetTeacherViewByIdAsync(id);
        }

        /// <summary>
        /// POST DELETE Zpracuje POST požadavek pro smazání učitele podle ID.
        /// </summary>
        /// <param name="id">ID učitele</param>
        /// <returns>ActionResult pro přesměrování na akci Index po úspěšném smazání učitele</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmedAsync(int id)
        {
            await _teacherService.DeleteTeacherAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Pomocná metoda pro získání učitele podle ID a vrácení odpovídajícího View.
        /// </summary>
        /// <param name="id">ID učitele</param>
        /// <returns>ActionResult pro zobrazení View učitele, nebo NotFoundResult, pokud teacher neexistuje</returns>
        private async Task<IActionResult> GetTeacherViewByIdAsync(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var teacher = await _teacherService.GetTeacherByIdAsync(id.Value);
            if (teacher == null)
            {
                return View("NotFound");
            }

            return View(teacher);
        }
    }
}
