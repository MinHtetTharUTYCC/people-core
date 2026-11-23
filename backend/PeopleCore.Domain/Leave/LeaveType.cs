using PeopleCore.Domain.Common;

namespace PeopleCore.Domain.Leave;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DefaultDays { get; set; }
    public bool IsPaid { get; set; }
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
