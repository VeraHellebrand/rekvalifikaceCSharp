using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Models;
using RekvalifikaceApp.ViewModels;

namespace RekvalifikaceApp.Services
{
    public class SubjectService
    {
        private readonly RekvalifikaceDbContext _dbContext;
        private readonly IMapper _mapper;

        public SubjectService(RekvalifikaceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// View model obsahující seznam učitelů seřazených podle příjmení.
        /// Každý učitel je reprezentován svým ID a plným jménem (Příjmení + Jméno).
        /// </summary>
        /// <returns>
        /// Objekt typu <see cref="SubjectsDropDownsVM"/>,který obsahuje seznam učitelů pro výběrové pole.
        /// </returns>
        public async Task<SubjectsDropDownsVM> GetSubjectsDropDownsVMAsync()
        {
            var teachers = await _dbContext.Teachers
                .OrderBy(x => x.LastName)
                .ToListAsync();

            var teacherSelectItems = _mapper.Map<List<TeacherSelectItem>>(teachers);

            return new SubjectsDropDownsVM
            {
                Teachers = teacherSelectItems
            };
        }
        /// <summary>
        /// CREATE Metoda pro přidání nového předmětu do databáze.
        /// </summary>
        /// <param name="subjectDto">DTO obsahující informace o předmětu</param>
        /// <returns>Task</returns>
        public async Task AddSubjectAsync(SubjectDto subjectDto)
        {
            var subject = _mapper.Map<Subject>(subjectDto);
            var teacher = await _dbContext.Teachers.FindAsync(subjectDto.TeacherId);
          
            if (teacher == null)
            {
                throw new Exception($"Učitel s ID {subjectDto.TeacherId} nebyl nalezen.");
            }
            subject.Teacher = teacher;

            await _dbContext.Subjects.AddAsync(subject);
            await _dbContext.SaveChangesAsync();
        }


        /// <summary>
        /// GET Metoda pro získání seznamu všech předmětů z databáze.
        /// </summary>
        /// <returns>Seznam DTO objektů reprezentujících studenta</returns>
        public async Task<IEnumerable<SubjectViewModel>> GetSubjectsViewModelAsync()
        {
            var subjects = await _dbContext.Subjects
                .Include(s => s.Teacher)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SubjectViewModel>>(subjects);
        }

        /// <summary>
        /// Metoda pro získání předmětu z databáze podle jeho ID a vrácení jako SubjectViewModel.
        /// </summary>
        /// <param name="id">ID hledaného předmětu</param>
        /// <returns>ViewModel objekt reprezentující předmětu nebo null, pokud předmět s daným ID nebyl nalezen</returns>
        public async Task<SubjectViewModel?> GetSubjectViewModelByIdAsync(int id)
        {
            var subject = await _dbContext.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
                return null;

            return _mapper.Map<SubjectViewModel>(subject);
        }

        /// <summary>
        /// Metoda pro získání předmětu z databáze podle jeho ID a vrácení jako SubjectDto.
        /// </summary>
        /// <param name="id">ID hledaného předmětu</param>
        /// <returns>ViewModel objekt reprezentující předmětu nebo null, pokud předmět s daným ID nebyl nalezen</returns>
        public async Task<SubjectDto?> GetSubjectDtoByIdAsync(int id)
        {
            var subject = await _dbContext.Subjects.FindAsync(id);

            if (subject == null)
                return null;

            return _mapper.Map<SubjectDto>(subject);
        }



        /// <summary>
        /// Metoda pro aktualizaci informací o existujícím předmětu v databázi.
        /// </summary>
        /// <param name="id">ID upravovaného předmětu</param>
        /// <param name="studentDto">DTO objekt obsahující nové informace o předmětu</param>
        /// <returns>Task</returns>
        public async Task UpdateSubjectAsync(int id, SubjectDto subjectDto)
        {
            // Načtení existující entity předmětu, včetně navigační vlastnosti Teacher
            var subject = await _dbContext.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Předmět s ID {id} nebyl nalezen.");
            }
            // Pokud je potřeba aktualizovat učitele
            if (subject.Teacher.Id != subjectDto.TeacherId)
            {
                var newTeacher = await _dbContext.Teachers.FindAsync(subjectDto.TeacherId);
                if (newTeacher == null)
                {
                    throw new Exception($"Učitel s ID {subjectDto.TeacherId} nebyl nalezen.");
                }

                subject.Teacher = newTeacher; // Nastavíme nového učitele
            }    

            _mapper.Map(subjectDto, subject);

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Metoda pro ověření existence předmětu v databázi podle jeho ID.
        /// </summary>
        /// <param name="id">ID předmětu</param>
        /// <returns>True, pokud předmět s daným ID existuje; jinak false</returns>
        public async Task<bool> SubjectExistsAsync(int id)
        {
            return await _dbContext.Subjects.AnyAsync(s => s.Id == id);
        }

        /// <summary>
        /// Metoda pro smazání předmětu z databáze.
        /// </summary>
        /// <param name="id">ID předmětu, který má být smazán</param>
        /// <returns>Task</returns>
        public async Task DeleteSubjectAsync(int id)
        {
            var subject = await GetSubjectEntityByIdAsync(id);
            _dbContext.Subjects.Remove(subject);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Pomocná metoda pro získání entity předmětu z databáze podle jeho ID.
        /// Vyvolá výjimku KeyNotFoundException, pokud předmět s daným ID neexistuje.
        /// </summary>
        /// <param name="id">ID předmětu</param>
        /// <returns>Instance třídy Subject</returns>
        /// <exception cref="KeyNotFoundException">Vyvolána, pokud předmětu s daným ID neexistuje</exception>
        private async Task<Subject> GetSubjectEntityByIdAsync(int id)
        {
            var subject = await _dbContext.Subjects.FindAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Předmětu s ID {id} nebyl nalezen.");
            }
            return subject;
        }
    }

}
