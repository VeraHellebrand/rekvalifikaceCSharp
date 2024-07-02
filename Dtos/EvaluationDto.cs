
using RekvalifikaceApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.Dtos
{
    public class EvaluationDto
    {
        public int Id { get; set; }

        [Display(Name = "Předmět")]
        public int SubjectId { get; set; }

        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Forma je povinná.")]
        [Display(Name = "Forma")]
        public EvaluationType Type { get; set; }

        [Required(ErrorMessage = "Splněno je povinné.")]
        [Display(Name = "Splněno")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Datum splnění")]
        public DateOnly CompletionDate { get; set; }

        public EvaluationDto()
        {
            CompletionDate = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
