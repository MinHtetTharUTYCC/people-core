using PeopleCore.Domain.Common;

namespace PeopleCore.Domain.Departments;

public class Position : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DepartmentId { get; set; }
    public string? SalaryGrade { get; set; }

    // Navigation properties
    public virtual Department Department { get; set; } = null!;
    public virtual ICollection<Employees.Employee> Employees { get; set; } = new List<Employees.Employee>();
}
