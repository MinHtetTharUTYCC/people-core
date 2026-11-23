namespace PeopleCore.Application.Departments.DTOs;

public class CreatePositionRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DepartmentId { get; set; }
    public string? SalaryGrade { get; set; }
}

public class UpdatePositionRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DepartmentId { get; set; }
    public string? SalaryGrade { get; set; }
}

public class PositionResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? SalaryGrade { get; set; }
    public DateTime CreatedAt { get; set; }
}
