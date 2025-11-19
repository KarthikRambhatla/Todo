using System.Security.Claims;

namespace Todo.Api.Services;

public class HttpUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? GetCurrentUserId()
        => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
}
