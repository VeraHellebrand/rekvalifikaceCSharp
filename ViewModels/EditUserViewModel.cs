using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(50, ErrorMessage = "Jméno nesmí být delší než 50 znaků.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je povinný.")]
        [EmailAddress(ErrorMessage = "Neplatná emailová adresa.")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Heslo musí mít alespoň {2} a maximálně {1} znaků dlouhé.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "Heslo musí obsahovat minimálně jedno velké písmeno, jedno malé písmeno, jedno číslo a jeden speciální znak.")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hesla se neshodují.")]
        public string? ConfirmPassword { get; set; }
    }
}
