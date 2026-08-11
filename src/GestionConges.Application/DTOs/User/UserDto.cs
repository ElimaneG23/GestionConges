namespace GestionConges.Application.DTOs.User;

public record CreateEmployeeDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role,          // "Manager" ou "Employee"
    Guid? ManagerId,
    string? JobTitle,
    DateTime? HireDate
);

public record UpdateEmployeeDto(
    string FirstName,
    string LastName,
    string? JobTitle,
    Guid? ManagerId,
    bool IsActive
);

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool IsActive,
    Guid? ManagerId,
    string? ManagerName,
    string? JobTitle,
    DateTime? HireDate,
    DateTime CreatedAt
);
