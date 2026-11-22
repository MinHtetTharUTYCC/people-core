# PeopleCore - HRIS REST API

> A production-grade Human Resource Information System built with .NET Core and Layered Architecture.

---

## Overview & Goals

Build a full-featured HRIS REST API from scratch using .NET Core with layered architecture. This project serves as both a real-world application and a comprehensive learning experience for .NET Core development patterns.

### What it covers

- **Departments & Positions** - Organizational structure management (departments, sub-departments, job positions)
- **Employee Management** - Employee CRUD, personal info, employment details, organizational assignment
- **Leave Management** - Leave types, requests, approval workflow, balance tracking
- **Attendance & Time Tracking** - Clock-in/out, work hours, overtime calculation
- **Payroll Basics** - Salary structures, pay runs, deductions
- **Authentication & Authorization** - JWT tokens, role-based access (Admin, HR, Employee)
- **API Validation** - FluentValidation for request validation
- **Error Handling** - Global exception handling, proper HTTP status codes
- **Database** - SQL Server Express with EF Core migrations and seed data
- **Testing** - xUnit unit tests for services, integration tests with TestServer

---

## Project Structure

```
people-core/
+-- plan.md                 [This file]
+-- backend/                [PeopleCore .NET API - layered architecture]
|   +-- PeopleCore.sln
|       +-- PeopleCore.Domain/
|       |   +-- Common/           # Base entities, audit fields
|       |   +-- Departments/      # Department, Position entities
|       |   +-- Employees/        # Employee entity
|       |   +-- Leave/            # LeaveType, LeaveRequest entities
|       |   +-- Attendance/       # AttendanceRecord entity
|       |   +-- Payroll/          # SalaryComponent, PayRun entities
|       |   +-- Identity/         # ApplicationUser, Roles
|       +-- PeopleCore.Application/
|       |   +-- Common/           # Result wrapper, paging
|       |   +-- Departments/      # DTOs, Services, Validators
|       |   +-- Employees/        # DTOs, Services, Validators
|       |   +-- Leave/            # DTOs, Services, Validators
|       |   +-- Attendance/       # DTOs, Services, Validators
|       |   +-- Payroll/          # DTOs, Services, Validators
|       |   +-- Identity/         # Auth DTOs, Services, Validators
|       +-- PeopleCore.Infrastructure/
|       |   +-- Data/             # DbContext, Configurations, Migrations
|       |   +-- Repositories/     # Generic + specific repositories
|       |   +-- Security/         # JWT token services
|       +-- PeopleCore.Api/
|       |   +-- Controllers/      # API controllers
|       |   +-- Middleware/       # Exception handling, auth
|       |   +-- Program.cs        # Dependency injection, pipeline
|       |   +-- appsettings.json  # Configuration
|       +-- PeopleCore.Tests/     # xUnit tests
|           +-- Unit/             # Service tests
|           +-- Integration/      # Controller + DB tests
+-- frontend/               [Frontend app - not yet implemented]
```

---

## Tech Stack

| Concern              | Technology                                      |
|-----------------------|-------------------------------------------------|
| Framework             | .NET 10                                         |
| Database              | SQL Server Express + EF Core                    |
| Authentication        | JWT Bearer tokens + ASP.NET Identity            |
| Validation            | FluentValidation                                |
| Testing               | xUnit + Moq + TestServer                        |
| Architecture          | Layered (Domain -> Application -> Infrastructure -> API) |
| API Style             | RESTful with standard HTTP methods and status codes |
| Documentation         | Swagger / OpenAPI                               |

---

## Core Entities

### Department
- `Id`, `Name`, `Code`, `ParentDepartmentId` (self-referencing for hierarchy), `ManagerId`

### Position
- `Id`, `Title`, `Description`, `DepartmentId`, `SalaryGrade`

### Employee
- `Id`, `FirstName`, `LastName`, `Email`, `PhoneNumber`, `DateOfBirth`, `HireDate`, `DepartmentId`, `PositionId`, `Status` (Active/Inactive/Terminated)

### LeaveType
- `Id`, `Name`, `Code`, `DefaultDays`, `IsPaid`

### LeaveRequest
- `Id`, `EmployeeId`, `LeaveTypeId`, `StartDate`, `EndDate`, `Reason`, `Status` (Pending/Approved/Rejected), `ApprovedBy`, `ApprovedAt`

### AttendanceRecord
- `Id`, `EmployeeId`, `Date`, `ClockIn`, `ClockOut`, `BreakMinutes`, `Status` (Present/Absent/Late/HalfDay)

### SalaryComponent
- `Id`, `EmployeeId`, `Type` (Basic/Allowance/Deduction), `Name`, `Amount`, `IsFixed`

### PayRun
- `Id`, `EmployeeId`, `PeriodStart`, `PeriodEnd`, `GrossPay`, `TotalDeductions`, `NetPay`, `Status` (Draft/Processed/Paid), `ProcessedAt`

---

## API Response Wrapper

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IEnumerable<ValidationError> Errors { get; set; }
}
```

---

## Authentication Flow

1. User calls `POST /api/auth/login` with credentials
2. System validates against ASP.NET Identity
3. JWT token generated with user claims and roles
4. Subsequent requests include `Authorization: Bearer <token>`
5. JWT middleware validates token and populates `HttpContext.User`
6. Authorization policies check roles (Admin, HR, Employee)

---

## Architecture Diagram

```
+----------------+     +----------------------------------+
|  API Client    |---->|      PeopleCore.Api              |
|                |     |  (Controllers + Middleware)       |
+----------------+     +----------------------------------+
                                  |
                                  v
                        +----------------------------------+
                        |    PeopleCore.Application         |
                        |  (Services + DTOs + Validators)   |
                        +----------------------------------+
                                     |
                   +-----------------+-----------------+
                   v                 v                 v
         +------------------+ +------------------+ +----------------+
         | PeopleCore.Domain| |PeopleCore.Infra. | |  SQL Server   |
         | (Entities + I/F) | |(EF Core + Repos) | |   Express     |
         +------------------+ +------------------+ +----------------+
```

---

## Delivery Plan

### Stage 1: Setup project structure and infrastructure
Create the layered solution with all four projects, configure SQL Server connection, and set up the foundation.

- Create solution `PeopleCore.sln` with four projects: `PeopleCore.Domain`, `PeopleCore.Application`, `PeopleCore.Infrastructure`, `PeopleCore.Api`
- Configure project references: Api -> Application -> Domain, Infrastructure -> Domain + Application
- Add NuGet packages: EF Core SQL Server, ASP.NET Core OpenApi/Swagger, JWT authentication, FluentValidation, xUnit
- Create `PeopleCore.Infrastructure/Data/HrisDbContext` with SQL Server connection string in `appsettings.json`
- Set up `Program.cs` with dependency injection for all layers, Swagger, and CORS
- Create common base classes in `PeopleCore.Domain/Common/` (BaseEntity with Id, CreatedAt, UpdatedAt)
- Create global exception handling middleware in `PeopleCore.Api/Middleware/`
- Create `ApiResponse<T>` wrapper in `PeopleCore.Application/Common/`
- Add first EF Core migration and seed database
- Verify solution builds and Swagger UI loads

### Stage 2: Implement authentication and authorization
Add JWT-based authentication with ASP.NET Identity and role-based authorization for Admin, HR, and Employee roles.

- Create `ApplicationUser` entity in `PeopleCore.Domain/Identity/` with ASP.NET Identity
- Configure Identity in `PeopleCore.Infrastructure/Data/` with password hasher and role manager
- Create JWT token service in `PeopleCore.Infrastructure/Security/` to generate and validate tokens
- Create auth DTOs in `PeopleCore.Application/Identity/`: `LoginRequest`, `RegisterRequest`, `AuthResponse`
- Create `AuthService` in `PeopleCore.Application/Identity/` with login, register, and token refresh
- Create `AuthController` in `PeopleCore.Api/Controllers/` with login and register endpoints
- Configure JWT bearer authentication in `Program.cs`
- Add authorization policies for Admin, HR, and Employee roles
- Create FluentValidation validators for auth DTOs
- Add unit tests for `AuthService` and integration tests for `AuthController`
- Verify login flow returns valid JWT token and protected endpoints work

### Stage 3: Implement Departments & Positions module
Build the organizational structure management with departments (including hierarchy) and job positions.

- Create `Department` entity with self-referencing `ParentDepartmentId` for hierarchy
- Create `Position` entity with `DepartmentId` foreign key
- Create DTOs: `CreateDepartmentRequest`, `UpdateDepartmentRequest`, `DepartmentResponse`, and similar for Position
- Create `IDepartmentService` and `IDepartmentRepository` interfaces
- Implement `DepartmentService` with CRUD operations and hierarchy traversal
- Implement `DepartmentRepository` in Infrastructure layer
- Create `DepartmentsController` and `PositionsController` with full CRUD endpoints
- Add FluentValidation validators (department code uniqueness, circular hierarchy prevention)
- Add EF Core configurations for relationships and indexes
- Add EF Core migration for new tables
- Add unit tests for service and integration tests for controllers
- Verify API endpoints via Swagger

### Stage 4: Implement Employee Management module
Build employee CRUD with personal info, employment details, and organizational assignment.

- Create `Employee` entity with `DepartmentId`, `PositionId`, `Status` (Active/Inactive/Terminated)
- Create DTOs: `CreateEmployeeRequest`, `UpdateEmployeeRequest`, `EmployeeResponse`, `EmployeeFilter`
- Create `IEmployeeService` and `IEmployeeRepository` interfaces
- Implement `EmployeeService` with CRUD, filtering by department, and pagination
- Implement `EmployeeRepository` with complex queries (employees by department with hierarchy)
- Create `EmployeesController` with endpoints: GET all (with pagination/filter), GET by ID, POST, PUT, DELETE
- Add FluentValidation validators (email uniqueness, date validation, required fields)
- Add business rules: cannot delete active employee, must assign valid department/position
- Add EF Core migration and seed sample employees
- Add unit tests for service and integration tests for controllers
- Verify API endpoints and filtering work correctly

### Stage 5: Implement Leave Management module
Build leave request workflow with leave types, approval process, and balance tracking.

- Create `LeaveType` entity with `Name`, `Code`, `DefaultDays`, `IsPaid`
- Create `LeaveRequest` entity with `EmployeeId`, `LeaveTypeId`, `StartDate`, `EndDate`, `Status`, `ApprovedBy`
- Create DTOs: `CreateLeaveRequest`, `LeaveRequestResponse`, `LeaveBalanceResponse`
- Create `ILeaveService` interface with methods: submit request, approve, reject, get balance, get by employee
- Implement `LeaveService` with business logic: overlap detection, balance calculation, approval workflow
- Implement `LeaveRepository` with queries for pending requests, employee history, and balance
- Create `LeaveController` with endpoints: submit request, approve/reject (HR only), get by employee, get pending (HR)
- Add authorization: only HR and Admin can approve/reject, employees can only see their own
- Add FluentValidation validators (date ranges, leave type existence)
- Add EF Core migration and seed leave types
- Add unit tests for service (overlap detection, balance calculation) and integration tests
- Verify approval workflow and authorization work correctly

### Stage 6: Implement Attendance & Time Tracking module
Build attendance tracking with clock-in/out, work hours calculation, and overtime detection.

- Create `AttendanceRecord` entity with `EmployeeId`, `Date`, `ClockIn`, `ClockOut`, `BreakMinutes`, `Status`
- Create DTOs: `ClockInRequest`, `ClockOutRequest`, `AttendanceResponse`, `AttendanceFilter`
- Create `IAttendanceService` interface with methods: clock in, clock out, get by employee, get by date range
- Implement `AttendanceService` with business logic: prevent double clock-in, calculate work hours, detect overtime
- Implement `AttendanceRepository` with queries for daily records, monthly summaries
- Create `AttendanceController` with endpoints: clock in, clock out, get by employee, get by date range (HR)
- Add authorization: employees can only clock themselves, HR can view all
- Add FluentValidation validators (date validation, time format)
- Add business rules: cannot clock out before clock-in, cannot clock in twice on same day
- Add EF Core migration
- Add unit tests for service (time calculations, edge cases) and integration tests
- Verify clock-in/out flow and authorization work correctly

### Stage 7: Implement Payroll Basics module
Build basic payroll processing with salary components, pay runs, and deduction calculation.

- Create `SalaryComponent` entity with `EmployeeId`, `Type` (Basic/Allowance/Deduction), `Name`, `Amount`, `IsFixed`
- Create `PayRun` entity with `EmployeeId`, `PeriodStart`, `PeriodEnd`, `GrossPay`, `TotalDeductions`, `NetPay`, `Status`
- Create DTOs: `CreateSalaryComponentRequest`, `PayRunResponse`, `GeneratePayRunRequest`
- Create `IPayrollService` interface with methods: manage salary components, generate pay run, process pay run
- Implement `PayrollService` with business logic: calculate gross pay, apply deductions, calculate net pay, prevent double processing
- Implement `PayrollRepository` with queries for employee salary structure, pay run history
- Create `PayrollController` with endpoints: manage salary components, generate pay run, process pay run (HR/Admin only)
- Add authorization: only HR and Admin can manage payroll
- Add FluentValidation validators (amount validation, period validation)
- Add business rules: cannot process overlapping pay periods, must have salary components
- Add EF Core migration and seed sample salary components
- Add unit tests for service (pay calculations) and integration tests
- Verify payroll calculation and processing workflow work correctly

### Stage 8: Finalize with testing and documentation
Add comprehensive tests across all modules, polish the API, and verify the complete system works end-to-end.

- Add integration tests for all controller endpoints across all modules
- Add unit tests for all service business logic (edge cases, validation rules)
- Add validator tests for FluentValidation rules
- Create test helpers: `IntegrationTestBase` with TestServer, `TestFactory` with in-memory database
- Add seed data for all entities (departments, positions, employees, leave types, salary components)
- Configure Swagger with JWT authentication support
- Add API documentation comments to controllers
- Verify all migrations apply cleanly and database schema is correct
- Run full test suite and verify all tests pass
- Verify end-to-end flow: register -> login -> create department -> create employee -> submit leave -> approve -> clock in -> generate payroll
- Final build and verification that the solution compiles and runs without errors

---

## User Stories

- As an **Admin**, I want to manage departments and positions so that I can define the organizational structure.
- As an **HR Manager**, I want to manage employee records so that I can maintain accurate employee data.
- As an **Employee**, I want to request leave so that I can manage my time off.
- As an **HR Manager**, I want to approve/reject leave requests so that I can control leave policies.
- As an **Employee**, I want to clock in and out so that my attendance is tracked.
- As an **HR Manager**, I want to process payroll so that employees get paid correctly.
- As a **System**, I want to authenticate and authorize users so that only authorized users can access resources.

---

## Validation Strategy

- **FluentValidation** validators in `PeopleCore.Application` for each DTO
- Validators registered with ASP.NET Core in `Program.cs`
- Validation errors returned in `ApiResponse.Errors` with field-level details
- Domain validation in entities (e.g., leave dates must not overlap)

---

## Testing Strategy

- **Unit Tests**: Service methods with mocked repositories (xUnit + Moq)
- **Integration Tests**: Full request pipeline with TestServer and test database
- **Test Coverage**: Focus on business logic (services) and API endpoints (controllers)

---

## Frontend (Planned)

The `frontend/` directory is reserved for the future frontend application. No implementation yet - the API will be built first, then a frontend will consume it.
