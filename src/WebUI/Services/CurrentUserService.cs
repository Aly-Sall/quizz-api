using System.Security.Claims;

using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

namespace _Net6CleanArchitectureQuizzApp.WebUI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var stringId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(stringId, out var id) ? id : null;
        }
    }
}
