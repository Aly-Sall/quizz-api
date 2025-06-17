// src/Domain/Entities/CandidateAccessToken.cs - Nouveau
public class CandidateAccessToken : IEntity
{
    public int Id { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExpirationTime { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}