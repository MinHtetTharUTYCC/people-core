namespace PeopleCore.Application.Payroll.DTOs;

public class CreateSalaryComponentRequest
{
    public int EmployeeId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsFixed { get; set; }
}

public class SalaryComponentResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsFixed { get; set; }
}

public class GeneratePayRunRequest
{
    public int EmployeeId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class PayRunResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal GrossPay { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ProcessedAt { get; set; }
}
