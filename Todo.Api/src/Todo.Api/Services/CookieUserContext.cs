namespace Todo.Api.Services;

public class CookieUserContext(IHttpContextAccessor accessor) : IUserContext
{
    private const string CookieName = "todo-user-id";

    public string? GetCurrentUserId()
    {
        var ctx = accessor.HttpContext;
        if (ctx == null) return null;

        if (ctx.Request.Cookies.TryGetValue(CookieName, out var id))
            return id;

        id = Guid.NewGuid().ToString();

        ctx.Response.Cookies.Append(CookieName, id, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Secure = true 
        });

        return id;
    }
}
