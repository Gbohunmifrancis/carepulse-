# Pharmacy Inventory & Prescription Management System

## Team Task Delegation & Guidelines

**Project**: SolveStation Pharmacy Management System  
**Timeline**: 1.5 weeks MVP  
**Tech Stack**: C# (.NET 8+), PostgreSQL, JWT Authentication  
**Date**: September 3, 2025

---

## 🎯 Team Member 1: Authentication & Security Specialist

### **Primary Responsibility**: `SolveStation.Authentication` Module

### **Detailed Tasks**:

#### **Core Deliverables**:

1. **JWT Authentication Service**
    - Implement `IJwtTokenService` and `JwtTokenService`
    - Token generation with 1-hour expiry (per PRD requirement)
    - Token validation and refresh mechanisms
    - Claims-based user identity management

2. **Authentication Controllers**
    - `AuthController` with login/logout endpoints
    - Password reset functionality
    - Token refresh endpoints
    - Input validation for all auth requests

3. **Security Middleware**
    - JWT authentication middleware
    - Role-based authorization attributes
    - Request logging for audit trails
    - Rate limiting for auth endpoints

4. **Password Security**
    - Secure password hashing (bcrypt/Argon2)
    - Password strength validation
    - Salt generation and management

5. **Authorization Infrastructure**
    - Custom authorization attributes for roles (Doctor, Pharmacist, Patient, Admin)
    - Role-based endpoint protection
    - Permission-based access control

### **Models to Create**:

```csharp
// Example structure - follow coding conventions
public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, DateTime ExpiresAt, UserRole Role);
public record TokenRefreshRequest(string RefreshToken);
```

### **Key Security Requirements**:

- All endpoints must use HTTPS
- Implement input validation and sanitization
- Log all authentication attempts
- Handle JWT expiry gracefully
- Prevent brute force attacks

### **Applicable Coding Conventions**:

#### **Naming Conventions**:

- **Classes**: `PascalCase` → `JwtTokenService`, `AuthController`
- **Methods**: `PascalCase` → `GenerateTokenAsync`, `ValidateCredentialsAsync`
- **Fields**: `camelCase` with underscore → `private readonly IJwtTokenService _jwtTokenService;`
- **Interfaces**: Prefix with 'I' → `IJwtTokenService`, `IAuthenticationService`

#### **Modern C# Features to Use**:

```csharp
// Use records for DTOs
public record AuthResult(bool IsSuccess, string? Token, string? ErrorMessage);

// Pattern matching for validation
public ValidationResult ValidateCredentials(LoginRequest request) => request switch
{
    { Email: null or "" } => ValidationResult.Failure("Email is required"),
    { Password: null or "" } => ValidationResult.Failure("Password is required"),
    _ when !IsValidEmail(request.Email) => ValidationResult.Failure("Invalid email format"),
    _ => ValidationResult.Success()
};

// Nullable reference types
public async Task<User?> FindUserByEmailAsync(string? email)
```

#### **Error Handling**:

```csharp
// Use custom exceptions
public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message) { }
}

// Result pattern for expected failures
public class AuthResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
    
    public static AuthResult<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static AuthResult<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}
```

#### **Async Best Practices**:

```csharp
// Always use Async suffix and CancellationToken
public async Task<AuthResult> AuthenticateUserAsync(
    LoginRequest request, 
    CancellationToken cancellationToken = default)
{
    return await _userRepository
        .FindByEmailAsync(request.Email)
        .ConfigureAwait(false);
}
```

### **Git Workflow for Team Member 1**:

#### **Branch Naming**:

```bash
# Feature branches
feature/AUTH-001-jwt-token-service
feature/AUTH-002-login-controller
feature/AUTH-003-password-security

# Bug fixes
bugfix/AUTH-BUG-001-token-validation-issue

# Example workflow
git checkout -b feature/AUTH-001-jwt-token-service
# Work on feature
git add .
git commit -m "feat(auth): implement JWT token generation service

- Add IJwtTokenService interface with token generation
- Implement JwtTokenService with 1-hour expiry
- Add token validation and refresh mechanisms
- Include comprehensive input validation"
```

#### **Commit Message Format**:

```bash
# Format: <type>(scope): <description>
feat(auth): add JWT token generation service
fix(auth): resolve token expiry validation bug
refactor(auth): simplify password hashing logic
test(auth): add unit tests for authentication service
```

---

## 🎯 Team Member 2: User Management Developer

### **Primary Responsibility**: `SolveStation.UserManagement` Module

### **Detailed Tasks**:

#### **Core Deliverables**:

1. **User CRUD Operations**
    - `IUserService` and `UserService` implementation
    - `UserController` with all REST endpoints
    - User registration with role assignment
    - Profile management and updates

2. **User Models & DTOs**
    - User entity with proper relationships
    - Create/Update request DTOs
    - User response models with role information
    - User search and filtering capabilities

3. **Role Management**
    - Doctor, Pharmacist, Patient, Admin role logic
    - Role assignment and validation
    - Role-based data filtering
    - Permission hierarchies

4. **User Repository**
    - `IUserRepository` and `UserRepository`
    - Advanced querying (search, filter, pagination)
    - Bulk operations for user management
    - Performance-optimized queries

5. **Validation Layer**
    - User input validation using FluentValidation
    - Business rule validation
    - Duplicate email prevention
    - Role assignment validation

### **Models to Create**:

```csharp
// Follow DDD patterns from conventions
public class User
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }
    
    // Domain events
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
}

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    UserRole Role);
```

### **Key Business Rules**:

- Email must be unique across all users
- Role assignment requires admin privileges
- Doctors can only view their own prescriptions
- Patients can only view their own data
- Soft delete for user deactivation

### **Applicable Coding Conventions**:

#### **Repository Pattern Implementation**:

```csharp
public interface IUserRepository : IRepository<User, UserId>
{
    Task<User?> FindByEmailAsync(Email email);
    Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
    Task<PagedResult<User>> SearchUsersAsync(
        string searchTerm, 
        UserRole? role,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
```

#### **LINQ Best Practices**:

```csharp
// Use LINQ for readable collection operations
var activePharmacists = await _context.Users
    .Where(u => u.IsActive)
    .Where(u => u.Role == UserRole.Pharmacist)
    .OrderBy(u => u.LastName)
    .ThenBy(u => u.FirstName)
    .ToListAsync(cancellationToken);
```

#### **Validation with FluentValidation**:

```csharp
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email too long");
            
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name too long");
    }
}
```

### **Git Workflow for Team Member 2**:

#### **Branch Naming**:

```bash
# User management features
feature/USER-001-user-crud-operations
feature/USER-002-role-management-system
feature/USER-003-user-search-filtering

# Example workflow
git checkout -b feature/USER-001-user-crud-operations
# Work on feature
git add .
git commit -m "feat(user): implement user CRUD operations

- Add User entity with domain events
- Implement UserService with full CRUD
- Create UserController with REST endpoints
- Add comprehensive validation rules
- Include pagination and search capabilities"
```

---

## 🎯 Team Member 3: Testing & Documentation Lead

### **Primary Responsibility**: `tests/` and `docs/` Directories

### **Detailed Tasks**:

#### **Core Deliverables**:

1. **Unit Testing Framework**
    - Set up NUnit/xUnit testing framework
    - Create test base classes and utilities
    - Mock setup for dependencies (Moq)
    - Test data builders and factories

2. **Comprehensive Test Coverage**
    - Unit tests for all services and controllers
    - Test coverage minimum 80%
    - Edge case and error condition testing
    - Performance benchmark tests

3. **Integration Testing**
    - API integration test setup
    - Database integration tests
    - End-to-end workflow testing
    - Authentication flow testing

4. **Documentation**
    - API documentation with Swagger/OpenAPI
    - Developer setup guides
    - Architecture decision records (ADRs)
    - Code documentation standards

5. **Test Data Management**
    - Test database seeding
    - Mock data generation
    - Test environment configuration
    - Automated test data cleanup

### **Testing Structure**:

```
tests/
├── SolveStation.PrescriptionManagement.Tests/
│   ├── Services/
│   ├── Controllers/
│   ├── Repositories/
│   └── TestUtilities/
├── SolveStation.InventoryManagement.Tests/
├── SolveStation.UserManagement.Tests/
└── SolveStation.PharmacyApi.IntegrationTests/
```

### **Applicable Coding Conventions**:

#### **Test Structure & Naming**:

```csharp
[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ILogger<UserService>> _loggerMock;
    private UserService _userService;
    
    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
    }
    
    [Test]
    public async Task CreateUserAsync_ValidInput_ReturnsUser()
    {
        // Arrange
        var request = new CreateUserRequest("John", "Doe", "john@example.com", UserRole.Doctor);
        var expectedUser = UserTestBuilder.CreateDoctor("john@example.com");
        
        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(expectedUser);
        
        // Act
        var result = await _userService.CreateUserAsync(request);
        
        // Assert
        Assert.That(result.Email.Value, Is.EqualTo("john@example.com"));
        Assert.That(result.Role, Is.EqualTo(UserRole.Doctor));
        
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }
    
    [Test]
    public void CreateUserAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            () => _userService.CreateUserAsync(null!));
    }
}
```

#### **Integration Test Setup**:

```csharp
[TestFixture]
public class UserControllerIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task CreateUser_ValidRequest_ReturnsCreatedUser()
    {
        // Arrange
        var request = new CreateUserRequest("John", "Doe", "john@example.com", UserRole.Doctor);
        
        // Act
        var response = await Client.PostAsJsonAsync("/api/users", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        createdUser.Should().NotBeNull();
        createdUser!.FullName.Should().Be("John Doe");
    }
}
```

#### **Documentation Standards**:

```csharp
/// <summary>
/// Creates a new user in the system with the specified role.
/// Validates input data and ensures email uniqueness.
/// </summary>
/// <param name="request">User creation request with required fields</param>
/// <param name="cancellationToken">Cancellation token for async operation</param>
/// <returns>The created user with generated ID and timestamps</returns>
/// <exception cref="ArgumentNullException">Thrown when request is null</exception>
/// <exception cref="ValidationException">Thrown when validation fails</exception>
/// <exception cref="DuplicateEmailException">Thrown when email already exists</exception>
public async Task<User> CreateUserAsync(
    CreateUserRequest request,
    CancellationToken cancellationToken = default)
```

### **Git Workflow for Team Member 3**:

#### **Branch Naming**:

```bash
# Testing branches
feature/TEST-001-unit-test-framework-setup
feature/TEST-002-user-service-unit-tests
feature/TEST-003-integration-test-suite

# Documentation branches
docs/DOC-001-api-documentation-setup
docs/DOC-002-developer-setup-guide
docs/DOC-003-architecture-decisions

# Example workflow
git checkout -b feature/TEST-001-unit-test-framework-setup
# Work on tests
git add .
git commit -m "test: setup comprehensive unit testing framework

- Configure NUnit with Moq for dependency mocking
- Create base test classes and utilities
- Add test data builders and factories
- Setup test database configuration
- Include code coverage reporting"
```

---

## 🔄 **Universal Git Workflow Guidelines**

### **Branch Naming Convention**:

```bash
# Pattern: <type>/<ticket-number>-<short-description>
feature/AUTH-001-jwt-token-service
bugfix/USER-BUG-001-email-validation-error
hotfix/SEC-001-authentication-bypass
refactor/INV-001-inventory-service-cleanup
docs/API-001-swagger-documentation
```

### **Commit Message Format**:

```bash
# Format: <type>(scope): <description>
#
# <optional body>
#
# <optional footer>

feat(auth): add JWT token generation with refresh capability

Implements secure JWT token generation with configurable expiry.
Includes refresh token mechanism and proper claims management.

Closes #AUTH-001
```

### **Commit Types**:

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation only changes
- `test`: Adding missing tests
- `refactor`: Code change that neither fixes bug nor adds feature
- `perf`: Performance improvements
- `chore`: Changes to build process or auxiliary tools

### **Pre-Commit Checklist**:

- [ ] Code follows established conventions
- [ ] All tests pass locally
- [ ] No compiler warnings
- [ ] XML documentation updated
- [ ] Nullable reference types handled
- [ ] Security considerations addressed

### **Pull Request Guidelines**:

1. Create feature branch from `main`
2. Complete all work and tests
3. Create PR with descriptive title and body
4. Include screenshots for UI changes
5. Link related issues/tickets
6. Request review from team lead
7. Address all review comments
8. Merge only after approval

---

## 🏗️ **Architecture & Integration Guidelines**

### **Dependency Injection Setup**:

```csharp
// Each module should register its services
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        return services;
    }
}
```

### **Configuration Standards**:

```csharp
// Use strongly-typed configuration
public class JwtConfiguration
{
    [Required]
    public string SecretKey { get; set; } = string.Empty;
    
    [Range(1, 24)]
    public int ExpiryHours { get; set; } = 1;
    
    [Required]
    public string Issuer { get; set; } = string.Empty;
}
```

### **Common Interfaces to Implement**:

```csharp
// From SolveStation.Common
public interface IRepository<TEntity, TKey>
    where TEntity : class
{
    Task<TEntity?> FindByIdAsync(TKey id);
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey id);
}

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
}
```

---

## 📝 **Success Criteria & Deliverables**

### **Definition of Done**:

- [ ] All code follows established conventions
- [ ] Unit tests with >80% coverage
- [ ] Integration tests for happy path
- [ ] XML documentation for public APIs
- [ ] No compiler warnings or errors
- [ ] Code review approved
- [ ] Manual testing completed
- [ ] Performance requirements met (<2s response time)

### **Quality Gates**:

1. **Code Quality**: StyleCop and analyzer compliance
2. **Security**: No hardcoded secrets, input validation
3. **Performance**: Response time benchmarks met
4. **Testing**: Comprehensive test coverage
5. **Documentation**: Complete API documentation

---

## 🚀 **Getting Started Checklist**

### **For All Team Members**:

1. [ ] Clone repository and create your feature branch
2. [ ] Set up development environment (.NET 8+, PostgreSQL)
3. [ ] Review coding conventions document thoroughly
4. [ ] Set up IDE with StyleCop analyzers
5. [ ] Create your first test branch and commit
6. [ ] Coordinate with other team members for interface contracts

### **Communication Protocol**:

- Daily standup at 9:00 AM
- Code reviews within 24 hours
- Blockers escalated immediately
- Architecture decisions documented in ADRs
- Weekly demo of completed features

---

**Remember**: Follow the coding conventions strictly, write comprehensive tests, and maintain clear documentation.
Quality over speed - we're building a healthcare system that requires reliability and security.

**Questions?** Reach out to the project lead immediately for clarification.
