using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Models;

namespace RekvalifikaceApp.Services
{
    public class StudentService
    {
        private readonly RekvalifikaceDbContext _dbContext;
        private readonly IMapper _mapper;

        public StudentService(RekvalifikaceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Metoda pro přidání nového studenta do databáze.
        /// </summary>
        /// <param name="studentDto">DTO obsahující informace o studentovi</param>
        /// <returns>Task</returns>
        public async Task AddStudentAsync(StudentDto studentDto)
        {
            var student = _mapper.Map<Student>(studentDto);
            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Metoda pro získání seznamu všech studentů z databáze.
        /// </summary>
        /// <returns>Seznam DTO objektů reprezentujících studenta</returns>
        public async Task<IEnumerable<StudentDto>> GetStudentsAsync()
        {
            var students = await _dbContext.Students.ToListAsync();
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        /// <summary>
        /// Metoda pro získání studenta z databáze podle jeho ID.
        /// </summary>
        /// <param name="id">ID hledaného studenta</param>
        /// <returns>DTO objekt reprezentující studenta nebo null, pokud student s daným ID nebyl nalezen</returns>
        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            if (student == null)
                return null;

            return _mapper.Map<StudentDto>(student);
        }

        /// <summary>
        /// Metoda pro aktualizaci informací o existujícím studentovi v databázi.
        /// </summary>
        /// <param name="id">ID upravovaného studenta</param>
        /// <param name="studentDto">DTO objekt obsahující nové informace o studentovi</param>
        /// <returns>Task</returns>
        public async Task UpdateStudentAsync(int id, StudentDto studentDto)
        {
            var student = await GetStudentEntityByIdAsync(id);
            _mapper.Map(studentDto, student);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Metoda pro ověření existence studenta v databázi podle jeho ID.
        /// </summary>
        /// <param name="id">ID studenta</param>
        /// <returns>True, pokud student s daným ID existuje; jinak false</returns>
        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _dbContext.Students.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Metoda pro smazání studenta z databáze.
        /// </summary>
        /// <param name="id">ID studenta, který má být smazán</param>
        /// <returns>Task</returns>
        public async Task DeleteStudentAsync(int id)
        {
            var student = await GetStudentEntityByIdAsync(id);
            _dbContext.Students.Remove(student);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Pomocná metoda pro získání entity studenta z databáze podle jeho ID.
        /// Vyvolá výjimku KeyNotFoundException, pokud student s daným ID neexistuje.
        /// </summary>
        /// <param name="id">ID studenta</param>
        /// <returns>Instance třídy Student</returns>
        /// <exception cref="KeyNotFoundException">Vyvolána, pokud student s daným ID neexistuje</exception>
        private async Task<Student> GetStudentEntityByIdAsync(int id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student s ID {id} nebyl nalezen.");
            }
            return student;
        }
    }
}
