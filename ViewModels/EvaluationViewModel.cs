using RekvalifikaceApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.ViewModels
{
    public class EvaluationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Název předmětu")]
        public string SubjectName { get; set; } = string.Empty;

        [Display(Name = "Jméno žáka")]
        public string StudentName { get; set; } = string.Empty;

        [Display(Name = "Forma")]
        public EvaluationType Type { get; set; }

        [Display(Name = "Splněno?")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Datum splnění")]
        public DateOnly CompletionDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
