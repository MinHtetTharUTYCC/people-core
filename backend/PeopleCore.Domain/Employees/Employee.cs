using PeopleCore.Domain.Common;
using PeopleCore.Domain.Departments;

namespace PeopleCore.Domain.Employees;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public int DepartmentId { get; set; }
    public int PositionId { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    // Navigation properties
    public virtual Department Department { get; set; } = null!;
    public virtual Position Position { get; set; } = null!;
    public virtual ICollection<Leave.LeaveRequest> LeaveRequests { get; set; } = new List<Leave.LeaveRequest>();
    public virtual ICollection<Attendance.AttendanceRecord> AttendanceRecords { get; set; } = new List<Attendance.AttendanceRecord>();
    public virtual ICollection<Payroll.SalaryComponent> SalaryComponents { get; set; } = new List<Payroll.SalaryComponent>();
    public virtual ICollection<Payroll.PayRun> PayRuns { get; set; } = new List<Payroll.PayRun>();
}

public enum EmployeeStatus
{
    Active,
    Inactive,
    Terminated
}
