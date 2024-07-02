using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Models;
using RekvalifikaceApp.ViewModels;

namespace RekvalifikaceApp.Services
{
    public class EvaluationService
    {
        private readonly RekvalifikaceDbContext _dbContext;
        private readonly IMapper _mapper;

        public EvaluationService(RekvalifikaceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// View model obsahující seznam studentů a předmětů.
        /// Každý student je reprezentován svým ID a plným jménem (Příjmení + Jméno).
        ///  Každý předmět je reprezentován svým ID a názvem
        /// </summary>
        /// <returns>
        /// Objekt typu <see cref="EvaluationDropDownsVM"/>,který obsahuje seznam studentů a předmětů pro výběrové pole.
        /// </returns>
        public async Task<EvaluationDropDownsVM> GetEvaluationDropDownsVMAsync()
        {
            var students = await _dbContext.Students
                .OrderBy(x => x.LastName)
                .ToListAsync();
            var subjects = await _dbContext.Subjects
                .OrderBy(x => x.Name)
                .ToListAsync();

            var studentSelectItems = _mapper.Map<List<StudentSelectItem>>(students);
            var subjectSelectItems = _mapper.Map<List<SubjectSelectItem>>(subjects);

            return new EvaluationDropDownsVM
            {
                Students = studentSelectItems,
                Subjects = subjectSelectItems
            };
        }

        /// <summary>
        /// CREATE Metoda pro přidání nového hodnocení do databáze.
        /// </summary>
        /// <param name="evaluationDto">DTO obsahující informace o hodnocení</param>
        /// <returns>Task</returns>
        public async Task AddEvaluationAsync(EvaluationDto evaluationDto)
        {
            var evaluation = _mapper.Map<Evaluation>(evaluationDto);

            var student = await _dbContext.Students.FindAsync(evaluationDto.StudentId);
            if (student == null)
            {
                throw new Exception($"Student s ID {evaluationDto.StudentId} nebyl nalezen.");
            }
            evaluation.Student = student;
            var subject = await _dbContext.Subjects.FindAsync(evaluationDto.SubjectId);
            if (subject == null)
            {
                throw new Exception($"Předmět s ID {evaluationDto.SubjectId} nebyl nalezen.");
            }
            evaluation.Subject = subject;

            await _dbContext.Evaluations.AddAsync(evaluation);
            await _dbContext.SaveChangesAsync();
        }


        /// <summary>
        /// GET Metoda pro získání seznamu všech hodnocení z databáze.
        /// </summary>
        /// <returns>Seznam DTO objektů reprezentujících hodnocení</returns>
        public async Task<IEnumerable<EvaluationViewModel>> GetEvaluationViewModelsAsync()
        {
            var evalutions = await _dbContext.Evaluations
                .Include(e => e.Student)
                .Include(e => e.Subject)
                .ToListAsync();
            return _mapper.Map<IEnumerable<EvaluationViewModel>>(evalutions);
        }

        /// <summary>
        /// Metoda pro získání hodnocení z databáze podle jeho ID a vrácení jako SubjectDto.
        /// </summary>
        /// <param name="id">ID hledaného hodnocení</param>
        /// <returns>ViewModel objekt reprezentující hodnocení nebo null, pokud předmět s daným ID nebyl nalezen</returns>
        public async Task<EvaluationDto?> GetEvaluationDtoByIdAsync(int id)
        {
            var evaluation = await _dbContext.Evaluations.FindAsync(id);

            if (evaluation == null)
                return null;

            return _mapper.Map<EvaluationDto>(evaluation);
        }
        /// <summary>
        /// EDIT Metoda pro aktualizaci informací o existujícím hodnocení v databázi.
        /// </summary>
        /// <param name="id">ID upravovaného hodnocení</param>
        /// <param name="studentDto">DTO objekt obsahující nové informace o hodnocení</param>
        /// <returns>Task</returns>
        public async Task UpdateEvaluationAsync(int id, EvaluationDto evaluationDto)
        {
            var evaluation = await _dbContext.Evaluations
                .Include(e => e.Student)
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (evaluation == null)
            {
                throw new KeyNotFoundException($"Hodnocení s ID {id} nebylo nalezeno.");
            }

            if (evaluation.Student.Id != evaluationDto.StudentId)
            {
                var newStudent = await _dbContext.Students.FindAsync(evaluationDto.StudentId);
                if (newStudent == null)
                {
                    throw new Exception($"Student s ID {evaluationDto.StudentId} nebyl nalezen.");
                }

                evaluation.Student = newStudent;
            }
            if (evaluation.Subject.Id != evaluationDto.SubjectId)
            {
                var newSubject = await _dbContext.Subjects.FindAsync(evaluationDto.SubjectId);
                if (newSubject == null)
                {
                    throw new Exception($"Předmět s ID {evaluationDto.SubjectId} nebyl nalezen.");
                }

                evaluation.Subject = newSubject;
            }

            _mapper.Map(evaluationDto, evaluation);

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// DELETE Metoda pro smazání hodnocení z databáze.
        /// </summary>
        /// <param name="id">ID hodnocení, který má být smazáno</param>
        /// <returns>Task</returns>
        public async Task DeleteEvaluationAsync(int id)
        {
            var evaluation = await GetEvaluationEntityByIdAsync(id);
            _dbContext.Evaluations.Remove(evaluation);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Pomocná metoda pro ověření existence hodnocení v databázi podle jeho ID.
        /// </summary>
        /// <param name="id">ID hodnocení</param>
        /// <returns>True, pokud hodnocení s daným ID existuje; jinak false</returns>
        public async Task<bool> EvaluationExistsAsync(int id)
        {
            return await _dbContext.Evaluations.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Pomocná metoda pro získání entity hodnocení z databáze podle jeho ID.
        /// Vyvolá výjimku KeyNotFoundException, pokud předmět s daným ID neexistuje.
        /// </summary>
        /// <param name="id">ID hodnocení</param>
        /// <returns>Instance třídy Evoluation</returns>
        /// <exception cref="KeyNotFoundException">Vyvolána, pokud hodnocení s daným ID neexistuje</exception>
        private async Task<Evaluation> GetEvaluationEntityByIdAsync(int id)
        {
            var evaluation = await _dbContext.Evaluations.FindAsync(id);
            if (evaluation == null)
            {
                throw new KeyNotFoundException($"Hodnocení s ID {id} nebylo nalezen.");
            }
            return evaluation;
        }

    }
}
