using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Services;

namespace RekvalifikaceApp.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentService _studentService;

        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// GET Zobrazí seznam všech studentů DTO.
        /// </summary>
        /// <returns>ActionResult pro zobrazení seznamu studentů</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            IEnumerable<StudentDto> allStudents = await _studentService.GetStudentsAsync();
            return View(allStudents);
        }

        /// <summary>
        /// GET Zobrazí detaily konkrétního studenta podle ID.
        /// </summary>
        /// <param name="id">ID studenta</param>
        /// <returns>ActionResult pro zobrazení detailů studenta</returns>
        [HttpGet]
        [Authorize(Roles ="Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            return await GetStudentViewByIdAsync(id);
        }

        /// <summary>
        /// GET CREATE Zobrazí formulář pro vytvoření nového studenta.
        /// </summary>
        /// <returns>ActionResult pro zobrazení formuláře pro vytvoření studenta</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST CREATE Zpracuje POST požadavek pro vytvoření nového studenta.
        /// </summary>
        /// <param name="studentDto">DTO objekt obsahující informace o novém studentovi</param>
        /// <returns>ActionResult pro přesměrování na akci Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> CreateAsync(StudentDto studentDto)
        {
            await _studentService.AddStudentAsync(studentDto);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// GET EDIT Zobrazí formulář pro editaci studenta podle ID.
        /// </summary>
        /// <param name="id">ID studenta</param>
        /// <returns>ActionResult pro zobrazení formuláře pro editaci studenta</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(int? id)
        {
            return await GetStudentViewByIdAsync(id);
        }

        /// <summary>
        /// POST EDIT Zpracuje POST požadavek pro editaci studenta podle ID.
        /// </summary>
        /// <param name="id">ID studenta</param>
        /// <param name="studentDto">DTO objekt obsahující aktualizované informace o studentovi</param>
        /// <returns>ActionResult pro přesměrování na akci Index, pokud je editace úspěšná</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(int id, StudentDto studentDto)
        {
            if (id != studentDto.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _studentService.UpdateStudentAsync(id, studentDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _studentService.StudentExistsAsync(studentDto.Id))
                    {
                        return View("NotFound");
                    }
                    ModelState.AddModelError(string.Empty, "Došlo k problému s konkurenční aktualizací. Zkuste akci znovu.");
                }
                
            }
            return View(studentDto);
        }

        /// <summary>
        /// GET DELETE Zobrazí formulář pro potvrzení smazání studenta podle ID.
        /// </summary>
        /// <param name="id">ID studenta</param>
        /// <returns>ActionResult pro zobrazení formuláře pro potvrzení smazání studenta</returns>
        [HttpGet]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            return await GetStudentViewByIdAsync(id);
        }

        /// <summary>
        /// POST DELETE Zpracuje POST požadavek pro smazání studenta podle ID.
        /// </summary>
        /// <param name="id">ID studenta</param>
        /// <returns>ActionResult pro přesměrování na akci Index po úspěšném smazání studenta</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admini, SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmedAsync(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }
        
        /// <summary>
        /// Pomocná metoda pro získání studenta podle ID a vrácení odpovídajícího View.
        /// </summary>
        /// <param name="id">ID studenta</param>
        /// <returns>ActionResult pro zobrazení View studenta, nebo NotFoundResult, pokud student neexistuje</returns>
        private async Task<IActionResult> GetStudentViewByIdAsync(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var student = await _studentService.GetStudentByIdAsync(id.Value);
            if (student == null)
            {
                return View("NotFound");
            }

            return View(student);
        }
    }
}
