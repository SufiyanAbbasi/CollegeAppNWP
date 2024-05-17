namespace CollegeApp.Models
{
    public static class CollegeRepositiory
    {
        public static List<Student> Students {  get; set; } = new List<Student> {
                new Student
                {
                    Id = 1,
                    StudentName = "Sufiyan",
                    Email = "Sufi@gmail.com",
                    Address = "Atd"
                },
                new Student
                {
                    Id = 2,
                    StudentName = "Shayan",
                    Email = "Shayan@gmail.com",
                    Address = "Pindi"
                }
            };

    }
}
