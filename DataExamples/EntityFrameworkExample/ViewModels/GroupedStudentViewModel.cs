using EntityFrameworkExample.Models;

namespace EntityFrameworkExample.ViewModels
{
    public class GroupedStudentViewModel
    {
        public string Department { get; set; }
        public List<Student> Students { get; set; }
    }
}
