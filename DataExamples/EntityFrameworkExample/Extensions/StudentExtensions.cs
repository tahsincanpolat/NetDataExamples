using EntityFrameworkExample.Models;

namespace EntityFrameworkExample.Extensions
{
    public static class StudentExtensions
    {
        // Öğrencileri yaş aralıklarına göre gruplayan bir extension 
        public static IDictionary<string,List<Student>> GroupByAgeRange(this IEnumerable<Student> students)
        {
            return students
                .GroupBy(s =>
                {
                    if (s.Age<=18) return "18 ve altı";
                    if (s.Age<=25) return "19 ve 24 aralığı";
                    if (s.Age<=35) return "25 ve 35 aralığı";
                    return "35 üstü";
                })
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
