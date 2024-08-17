namespace EntityFrameworkExample.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Foreign key property
        public int StudentId { get; set; } // Öğrenci id si
        public Student Student { get; set; } // Navigation Property
    }
}
