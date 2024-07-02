using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný.")]
        [StringLength(50, ErrorMessage = "Název nesmí být delší než 50 znaků.")]
        [Display(Name = "Název")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Učitel")]
        public Teacher Teacher { get; set; } = new Teacher();

        [Required(ErrorMessage = "Informace o zkoušce je povinná.")]
        [Display(Name = "Má zkoušku")]
        public bool HasExam {  get; set; }

        [Required(ErrorMessage = "Informace o projektu je povinná.")]
        [Display(Name = "Má projekt")]
        public bool HasProject { get; set; }
    }
}
