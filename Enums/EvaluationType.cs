using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.Enums
{
    public enum EvaluationType
    {
        [Display(Name = "Zkouška")]
        Exam,

        [Display(Name = "Projekt")]
        Project
    }
}
