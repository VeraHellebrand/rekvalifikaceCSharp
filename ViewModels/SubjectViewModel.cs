
using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.ViewModels
{
    public class SubjectViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Název")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Jméno učitele")]
        public string TeacherName { get; set; } = string.Empty ;

        [Display(Name = "Zkouška")]
        public bool HasExam { get; set; }

        [Display(Name = "Projekt")]

        public bool HasProject { get; set; }
        [Display(Name = "Má zkoušku")]
        public string HasExamText => HasExam ? "Ano" : "Ne";

        [Display(Name = "Má projekt")]
        public string HasProjectText => HasProject ? "Ano" : "Ne";
    }
}
