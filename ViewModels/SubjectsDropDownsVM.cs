
namespace RekvalifikaceApp.ViewModels
{
    
    public class SubjectsDropDownsVM
    {
        public List<TeacherSelectItem> Teachers { get; set; }

        public SubjectsDropDownsVM()
        {
            Teachers = new List<TeacherSelectItem>();
        }
    }
}
