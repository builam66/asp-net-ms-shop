using System.Security.Claims;

namespace BuildingBlocks.Security;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
}

public interface IJwtTokenService
{
    string GenerateToken(string userId, string userName, string email, IEnumerable<string> roles);
    bool ValidateToken(string token);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

public static class SecurityConstants
{
    public const string DefaultAuthenticationScheme = "Bearer";
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    public const string CustomerRole = "Customer";
    
    public static class Permissions
    {
        public const string ReadProducts = "products:read";
        public const string WriteProducts = "products:write";
        public const string ReadOrders = "orders:read";
        public const string WriteOrders = "orders:write";
        public const string ReadBaskets = "baskets:read";
        public const string WriteBaskets = "baskets:write";
        public const string AdminAccess = "admin:access";
    }
}