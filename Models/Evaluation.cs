
using RekvalifikaceApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace RekvalifikaceApp.Models
{
    public class Evaluation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Předmět je povinný.")]
        [Display(Name = "Předmět")]
        public Subject Subject { get; set; } = new Subject();

        [Required(ErrorMessage = "Student je povinný.")]
        [Display(Name = "Student")]
        public Student Student { get; set; } = new Student();

        [Required(ErrorMessage = "Forma je povinná.")]
        [Display(Name = "Forma")]
        public EvaluationType Type { get; set; }

        [Required(ErrorMessage = "Splněno je povinné.")]
        [Display(Name = "Splněno")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Datum splnění")]
        public DateOnly CompletionDate { get; set; }

        public Evaluation()
        {
            CompletionDate = DateOnly.FromDateTime(DateTime.Now);
        }
    }

   
}
