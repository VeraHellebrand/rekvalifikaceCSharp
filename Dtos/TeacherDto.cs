using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.Dtos
{
    /// <summary>
    /// Data Transfer Object (DTO) pro manipulaci se učitelem v rámci aplikace.
    /// Slouží k předávání dat mezi vrstvami aplikace a validaci vstupních dat.
    /// </summary>
    public class TeacherDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(50, ErrorMessage = "Jméno nesmí být delší než 50 znaků.")]
        [Display(Name = "Jméno")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Příjmení je povinné.")]
        [StringLength(50, ErrorMessage = "Příjmení nesmí být delší než 50 znaků.")]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum narození je povinné.")]
        [DataType(DataType.Date, ErrorMessage = "Neplatný formát data.")]
        [Display(Name = "Datum narození")]
        public DateOnly DateOfBirth { get; set; }

        [EmailAddress(ErrorMessage = "Neplatná emailová adresa.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Neplatné telefonní číslo.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
    }
}

