namespace RekvalifikaceApp.ViewModels
{
    public class EvaluationDropDownsVM
    {
        public List<SubjectSelectItem> Subjects { get; set; }
        public List<StudentSelectItem> Students { get; set; }

        public EvaluationDropDownsVM()
        {
            Subjects = new List<SubjectSelectItem>();
            Students = new List<StudentSelectItem>();
        }
    }
}
