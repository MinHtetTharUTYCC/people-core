using PeopleCore.Domain.Common;

namespace PeopleCore.Domain.Attendance;

public class AttendanceRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public int BreakMinutes { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;

    // Navigation properties
    public virtual Employees.Employee Employee { get; set; } = null!;
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    HalfDay
}
