namespace PeopleCore.Application.Employees.DTOs;

public class CreateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public int DepartmentId { get; set; }
    public int PositionId { get; set; }
}

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public int DepartmentId { get; set; }
    public int PositionId { get; set; }
    public string? Status { get; set; }
}

public class EmployeeResponse
{
    public int Id { get; init; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int PositionId { get; set; }
    public string? PositionTitle { get; set; }
    public string Status { get; set; } = string.Empty;
}
