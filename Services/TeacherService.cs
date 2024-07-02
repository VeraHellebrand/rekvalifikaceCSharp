using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Models;

namespace RekvalifikaceApp.Services
{
    public class TeacherService
    {
        private readonly RekvalifikaceDbContext _dbContext;
        private readonly IMapper _mapper;

        public TeacherService(RekvalifikaceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Metoda pro přidání nového učitele do databáze.
        /// </summary>
        /// <param name="teacherDto">DTO obsahující informace o učiteli</param>
        /// <returns>Task</returns>
        public async Task AddTeacherAsync(TeacherDto teacherDto)
        {
            var teacher = _mapper.Map<Teacher>(teacherDto);
            await _dbContext.Teachers.AddAsync(teacher);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Metoda pro získání seznamu všech učitelů z databáze.
        /// </summary>
        /// <returns>Seznam DTO objektů reprezentujících učitele</returns>
        public async Task<IEnumerable<TeacherDto>> GetTeachersAsync()
        {
            var teachers = await _dbContext.Teachers.ToListAsync();
            return _mapper.Map<IEnumerable<TeacherDto>>(teachers);
        }

        /// <summary>
        /// Metoda pro získání učitele z databáze podle jeho ID.
        /// </summary>
        /// <param name="id">ID hledaného učitele</param>
        /// <returns>DTO objekt reprezentující učitele nebo null, pokud učitel s daným ID nebyl nalezen</returns>
        public async Task<TeacherDto?> GetTeacherByIdAsync(int id)
        {
            var teacher = await _dbContext.Teachers.FindAsync(id);
            if (teacher == null)
                return null;

            return _mapper.Map<TeacherDto>(teacher);
        }

        /// <summary>
        /// Metoda pro aktualizaci informací o existujícím učitelovi v databázi.
        /// </summary>
        /// <param name="id">ID upravovaného učitele</param>
        /// <param name="teacherDto">DTO objekt obsahující nové informace o učiteli</param>
        /// <returns>Task</returns>
        public async Task UpdateTeacherAsync(int id, TeacherDto teacherDto)
        {
            var teacher = await GetTeacherEntityByIdAsync(id);
            _mapper.Map(teacherDto, teacher);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Metoda pro ověření existence učitele v databázi podle jeho ID.
        /// </summary>
        /// <param name="id">ID učitele</param>
        /// <returns>True, pokud učitel s daným ID existuje; jinak false</returns>
        public async Task<bool> TeacherExistsAsync(int id)
        {
            return await _dbContext.Teachers.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Metoda pro smazání učitele z databáze.
        /// </summary>
        /// <param name="id">ID učitele, který má být smazán</param>
        /// <returns>Task</returns>
        public async Task DeleteTeacherAsync(int id)
        {
            var teacher = await GetTeacherEntityByIdAsync(id);
            _dbContext.Teachers.Remove(teacher);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Pomocná metoda pro získání entity učitele z databáze podle jeho ID.
        /// Vyvolá výjimku KeyNotFoundException, pokud učitel s daným ID neexistuje.
        /// </summary>
        /// <param name="id">ID učitele</param>
        /// <returns>Instance třídy Učitel</returns>
        /// <exception cref="KeyNotFoundException">Vyvolána, pokud učitel s daným ID neexistuje</exception>
        private async Task<Teacher> GetTeacherEntityByIdAsync(int id)
        {
            var teacher = await _dbContext.Teachers.FindAsync(id);
            if (teacher == null)
            {
                throw new KeyNotFoundException($"Teacher s ID {id} nebyl nalezen.");
            }
            return teacher;
        }
    }
}
