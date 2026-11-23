namespace PeopleCore.Application.Departments.DTOs;

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }
}

public class UpdateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }
}

public class DepartmentResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public string? ParentDepartmentName { get; set; }
    public int? ManagerId { get; set; }
    public DateTime CreatedAt { get; set; }
}
