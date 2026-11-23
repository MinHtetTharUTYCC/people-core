using PeopleCore.Domain.Common;

namespace PeopleCore.Domain.Payroll;

public class PayRun : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal GrossPay { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public PayRunStatus Status { get; set; } = PayRunStatus.Draft;
    public DateTime? ProcessedAt { get; set; }

    // Navigation properties
    public virtual Employees.Employee Employee { get; set; } = null!;
}

public enum PayRunStatus
{
    Draft,
    Processed,
    Paid
}
