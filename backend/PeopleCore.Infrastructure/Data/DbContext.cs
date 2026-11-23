using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeopleCore.Domain.Attendance;
using PeopleCore.Domain.Departments;
using PeopleCore.Domain.Employees;
using PeopleCore.Domain.Identity;
using PeopleCore.Domain.Leave;
using PeopleCore.Domain.Payroll;

namespace PeopleCore.Infrastructure.Data;

public class DbContext(DbContextOptions<DbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<SalaryComponent> SalaryComponents => Set<SalaryComponent>();
    public DbSet<PayRun> PayRuns => Set<PayRun>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply Identity configurations
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("ApplicationUsers");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        // Department configurations
        builder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.HasOne(d => d.ParentDepartment)
                .WithMany(d => d.ChildDepartments)
                .HasForeignKey(d => d.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Position configurations
        builder.Entity<Position>(entity =>
        {
            entity.ToTable("Positions");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.SalaryGrade).HasMaxLength(50);
            entity.HasOne(p => p.Department)
                .WithMany(d => d.Positions)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Employee configurations
        builder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // LeaveRequest configurations
        builder.Entity<LeaveRequest>(entity =>
        {
            entity.ToTable("LeaveRequests");
            entity.Property(e => e.Reason).HasMaxLength(1000);
            entity.Property(e => e.ApprovedBy).HasMaxLength(100);
            entity.HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(lr => lr.LeaveType)
                .WithMany(lt => lt.LeaveRequests)
                .HasForeignKey(lr => lr.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AttendanceRecord configurations
        builder.Entity<AttendanceRecord>(entity =>
        {
            entity.ToTable("AttendanceRecords");
            entity.HasIndex(e => new { e.EmployeeId, e.Date }).IsUnique();
            entity.HasOne(ar => ar.Employee)
                .WithMany(e => e.AttendanceRecords)
                .HasForeignKey(ar => ar.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // SalaryComponent configurations
        builder.Entity<SalaryComponent>(entity =>
        {
            entity.ToTable("SalaryComponents");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(sc => sc.Employee)
                .WithMany(e => e.SalaryComponents)
                .HasForeignKey(sc => sc.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PayRun configurations
        builder.Entity<PayRun>(entity =>
        {
            entity.ToTable("PayRuns");
            entity.Property(e => e.GrossPay).HasPrecision(18, 2);
            entity.Property(e => e.TotalDeductions).HasPrecision(18, 2);
            entity.Property(e => e.NetPay).HasPrecision(18, 2);
            entity.HasOne(pr => pr.Employee)
                .WithMany(e => e.PayRuns)
                .HasForeignKey(pr => pr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // LeaveType configurations
        builder.Entity<LeaveType>(entity =>
        {
            entity.ToTable("LeaveTypes");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(50);
        });

        // Seed leave types
        builder.Entity<LeaveType>().HasData(
            new LeaveType { Id = 1, Name = "Annual Leave", Code = "ANNUAL", DefaultDays = 20, IsPaid = true, CreatedAt = new DateTime(2026, 1, 1) },
            new LeaveType { Id = 2, Name = "Sick Leave", Code = "SICK", DefaultDays = 10, IsPaid = true, CreatedAt = new DateTime(2026, 1, 1) },
            new LeaveType { Id = 3, Name = "Unpaid Leave", Code = "UNPAID", DefaultDays = 5, IsPaid = false, CreatedAt = new DateTime(2026, 1, 1) }
        );
    }
}
