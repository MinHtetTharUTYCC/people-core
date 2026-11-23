using PeopleCore.Domain.Common;

namespace PeopleCore.Domain.Departments;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }

    // Navigation properties
    public virtual Department? ParentDepartment { get; set; }
    public virtual ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
    public virtual ICollection<Employees.Employee> Employees { get; set; } = new List<Employees.Employee>();
}
