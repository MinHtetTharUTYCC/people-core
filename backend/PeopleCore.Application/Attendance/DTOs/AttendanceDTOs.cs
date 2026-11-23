namespace PeopleCore.Application.Attendance.DTOs;

public class AttendanceResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public int BreakMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
}
