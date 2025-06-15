using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}