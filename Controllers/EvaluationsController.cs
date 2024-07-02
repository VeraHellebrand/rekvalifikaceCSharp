
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Models;
using RekvalifikaceApp.Services;
using RekvalifikaceApp.ViewModels;

namespace RekvalifikaceApp.Controllers
{
    public class EvaluationsController : Controller
    {
        private readonly EvaluationService _evaluationService;

        public EvaluationsController(EvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        /// <summary>
        /// GET Zobrazí indexovou stránku se seznamem hodnocení.
        /// </summary>
        /// <returns>View s seznamem hodnocení</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var viewModel = await _evaluationService.GetEvaluationViewModelsAsync();
            return View(viewModel);
        }

        /// <summary>
        /// GET CREATE Zobrazí formulář pro vytvoření nového hodnocení.
        /// </summary>
        /// <returns>ActionResult pro zobrazení formuláře pro vytvoření hodnocení</returns>
        [HttpGet]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> CreateAsync()
        {
            var evaluationDropDownsData = await _evaluationService.GetEvaluationDropDownsVMAsync();
            ViewBag.Students = new SelectList(evaluationDropDownsData.Students, "Id", "FullName");
            ViewBag.Subjects = new SelectList(evaluationDropDownsData.Subjects,"Id", "Name");
            return View();
        }
        /// <summary>
        /// POST CREATE Zpracuje POST požadavek pro vytvoření nového hodnocení.
        /// </summary>
        /// <param name="subjectDto">DTO objekt obsahující informace o novém hodnocení</param>
        /// <returns>
        /// Pokud je vytvoření úspěšné, přesměruje na akci Index. 
        /// Pokud dojde k chybě při vytváření, vrátí pohled s formulářem pro nový předmět s chybovou zprávou.
        /// Pokud model není validní, opětovně načte data pro dropdowny a vrátí pohled pro úpravu s chybovými zprávami.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> CreateAsync(EvaluationDto evaluationDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _evaluationService.AddEvaluationAsync(evaluationDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Nepodařilo se přidat předmět: {ex.Message}");
                }
            }
            var evaluationDropDownsData = await _evaluationService.GetEvaluationDropDownsVMAsync();
            ViewBag.Students = new SelectList(evaluationDropDownsData.Students, "Id", "FullName");
            ViewBag.Subjects = new SelectList(evaluationDropDownsData.Subjects, "Id", "Name");

            return View(evaluationDto);
        }

        

        /// <summary>
        /// GET EDIT Zobrazí formulář pro editaci hodnocení podle ID.
        /// </summary>
        /// <param name="id">ID hodnocení</param>
        /// <returns>ActionResult pro zobrazení formuláře pro editaci hodnocení</returns>
        [HttpGet]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(int? id)
        {
            var evaluationDropDownsData = await _evaluationService.GetEvaluationDropDownsVMAsync();
            ViewBag.Students = new SelectList(evaluationDropDownsData.Students, "Id", "FullName");
            ViewBag.Subjects = new SelectList(evaluationDropDownsData.Subjects, "Id", "Name");

            return await GetEvaluationViewByIdAsync(id);
        }

        /// <summary>
        /// POST EDIT Zpracuje POST požadavek pro editaci hodnocení podle ID.
        /// </summary>
        /// <param name="id">ID hodnocení</param>
        /// <param name="evaluationDto">DTO objekt obsahující aktualizované informace o hodnocení</param>
        /// <returns>ActionResult pro přesměrování na akci Index, pokud je editace úspěšná</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> EditAsync(int id, EvaluationDto evaluationDto)
        {
            EvaluationDropDownsVM evaluationDropDownsData;
            if (id != evaluationDto.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    await _evaluationService.UpdateEvaluationAsync(id, evaluationDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _evaluationService.EvaluationExistsAsync(evaluationDto.Id))
                    {
                        return View("NotFound");
                    }
                    ModelState.AddModelError(string.Empty, "Došlo k problému s konkurenční aktualizací. Zkuste akci znovu.");
                }
            }
            evaluationDropDownsData = await _evaluationService.GetEvaluationDropDownsVMAsync();
            ViewBag.Students = new SelectList(evaluationDropDownsData.Students, "Id", "FullName");
            ViewBag.Subjects = new SelectList(evaluationDropDownsData.Subjects, "Id", "Name");

            return View(evaluationDto);
        }


        /// <summary>
        /// POST DELETE Zpracuje POST požadavek pro smazání hodnocení podle ID.
        /// </summary>
        /// <param name="id">ID hodnocení</param>
        /// <returns>ActionResult pro přesměrování na akci Index po úspěšném smazání hodnocení</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Učitelé, Admini, SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmedAsync(int id)
        {
            await _evaluationService.DeleteEvaluationAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Pomocná metoda pro získání hodnocení podle ID a vrácení odpovídajícího View.
        /// </summary>
        /// <param name="id">ID hodnocenía</param>
        /// <returns>ActionResult pro zobrazení View hodnocení, nebo NotFoundResult, pokud předmět neexistuje</returns>
        private async Task<IActionResult> GetEvaluationViewByIdAsync(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var evaluation = await _evaluationService.GetEvaluationDtoByIdAsync(id.Value);
            if (evaluation == null)
            {
                return View("NotFound");
            }

            return View(evaluation);
        }


    }
}
