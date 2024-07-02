using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Jméno je povinné.")]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Heslo je povinné.")]
        public string Password { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public bool Remember {  get; set; }
    }
}
