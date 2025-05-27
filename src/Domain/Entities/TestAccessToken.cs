using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;
public class TestAccessToken : IEntity
{
    public TestAccessToken()
    {
        Token = Guid.NewGuid().ToString();
    }
    public int Id { get; set; }
    public string Token { get; set; } 
    public string CandidateEmail { get; set; } = null!;
    public DateTime ExpirationTime { get; set; }
    public bool IsUsed { get; set; } = false;
    public int TestId { get; set; }
    public int? TentativeId { get; set; } 

}