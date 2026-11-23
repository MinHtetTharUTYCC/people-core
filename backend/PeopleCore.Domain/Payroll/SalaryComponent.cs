using PeopleCore.Domain.Common;

namespace PeopleCore.Domain.Payroll;

public class SalaryComponent : BaseEntity
{
    public int EmployeeId { get; set; }
    public SalaryType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsFixed { get; set; }

    // Navigation properties
    public virtual Employees.Employee Employee { get; set; } = null!;
}

public enum SalaryType
{
    Basic,
    Allowance,
    Deduction
}
