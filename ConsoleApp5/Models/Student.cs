namespace ConsoleApp5.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int Age { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
    //public string Email { get; set; } = "";
    //public int EnrollmentYear { get; set; }
    //public bool IsActive { get; set; }
}
