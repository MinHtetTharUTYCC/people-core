using PeopleCore.Domain.Common;

namespace PeopleCore.Domain.Leave;

public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation properties
    public virtual Employees.Employee Employee { get; set; } = null!;
    public virtual LeaveType LeaveType { get; set; } = null!;
}

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected
}
