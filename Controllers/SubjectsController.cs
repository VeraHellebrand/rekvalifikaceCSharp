
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Services;
using RekvalifikaceApp.ViewModels;


namespace RekvalifikaceApp.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly SubjectService _subjectService;

        public SubjectsController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// GET Zobrazí seznam všech předmětů DTO.
        /// </summary>
        /// <returns>ActionResult pro zobrazení seznamu předmětů</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = await _subjectService.GetSubjectsViewModelAsync();
            return View(viewModel);
        }

        /// <summary>
        /// GET Zobrazí detaily konkrétního předmětu podle ID.
        /// </summary>
        /// <param name="id">ID předmětu</param>
        /// <returns>ActionResult pro zobrazení detailů předmětu</returns>
        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _subjectService.GetSubjectViewModelByIdAsync(id.Value);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        /// <summary>
        /// Zobrazí formulář pro vytvoření nového předmětu.
        /// </summary>
        /// <returns>ActionResult pro zobrazení formuláře pro vytvoření předmětu</returns>
        [HttpGet]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            var subjectDropDownsData = await _subjectService.GetSubjectsDropDownsVMAsync();
            ViewBag.Teachers = new SelectList(subjectDropDownsData.Teachers, "Id", "FullName");
            return View();
        }

        /// <summary>
        /// CREATE Zpracuje POST požadavek pro vytvoření nového předmětu.
        /// </summary>
        /// <param name="subjectDto">DTO objekt obsahující informace o novém předmětu</param>
        /// <returns>
        /// Pokud je vytvoření úspěšné, přesměruje na akci Index. 
        /// Pokud dojde k chybě při vytváření, vrátí pohled s formulářem pro nový předmět s chybovou zprávou.
        /// Pokud model není validní, opětovně načte data pro dropdowny a vrátí pohled pro úpravu s chybovými zprávami.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> CreateAsync(SubjectDto subjectDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _subjectService.AddSubjectAsync(subjectDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Nepodařilo se přidat předmět: {ex.Message}");
                }
            }
            var subjectDropDownsData = await _subjectService.GetSubjectsDropDownsVMAsync();
            ViewBag.Teachers = new SelectList(subjectDropDownsData.Teachers, "Id", "FullName");
            
            return View(subjectDto);
        }

        /// <summary>
        /// GET EDIT Zobrazí formulář pro editaci předmětu podle ID.
        /// </summary>
        /// <param name="id">ID předmětu</param>
        /// <returns>ActionResult pro zobrazení formuláře pro editaci předmětu</returns>
        [HttpGet]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var subjectDropDownsData = await _subjectService.GetSubjectsDropDownsVMAsync();
            ViewBag.Teachers = new SelectList(subjectDropDownsData.Teachers, "Id", "FullName");
            return await GetSubjectViewByIdAsync(id);
        }


        /// <summary>
        /// POST EDIT Zpracuje POST požadavek pro editaci předmětu podle ID.
        /// </summary>
        /// <param name="id">ID předmětu</param>
        /// <param name="subjectDto">DTO objekt obsahující aktualizované informace o předmětu</param>
        /// <returns>ActionResult pro přesměrování na akci Index, pokud je editace úspěšná</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(int id, SubjectDto subjectDto)
        {
            SubjectsDropDownsVM subjectDropDownsData;
            if (id != subjectDto.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _subjectService.UpdateSubjectAsync(id, subjectDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _subjectService.SubjectExistsAsync(subjectDto.Id))
                    {
                        return View("NotFound");
                    }
                    ModelState.AddModelError(string.Empty, "Došlo k problému s konkurenční aktualizací. Zkuste akci znovu.");
                }
            }
            subjectDropDownsData = await _subjectService.GetSubjectsDropDownsVMAsync();
            ViewBag.Teachers = new SelectList(subjectDropDownsData.Teachers, "Id", "FullName");
            return View(subjectDto);
        }

        /// <summary>
        /// GET DELETE Zobrazí formulář pro potvrzení smazání předmětu podle ID.
        /// </summary>
        /// <param name="id">ID předmětu</param>
        /// <returns>ActionResult pro zobrazení formuláře pro potvrzení smazání předmětu</returns>

        [HttpGet]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetSubjectViewByIdAsync(id);
        }

        /// <summary>
        /// POST DELETE Zpracuje POST požadavek pro smazání předmětu podle ID.
        /// </summary>
        /// <param name="id">ID předmětu</param>
        /// <returns>ActionResult pro přesměrování na akci Index po úspěšném smazání předmětu</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _subjectService.DeleteSubjectAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Pomocná metoda pro získání předmětu podle ID a vrácení odpovídajícího View.
        /// </summary>
        /// <param name="id">ID předmětua</param>
        /// <returns>ActionResult pro zobrazení View předmětu, nebo NotFoundResult, pokud předmět neexistuje</returns>
        private async Task<IActionResult> GetSubjectViewByIdAsync(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var subject = await _subjectService.GetSubjectDtoByIdAsync(id.Value);
            if (subject == null)
            {
                return View("NotFound");
            }

            return View(subject);
        }
    }
}
