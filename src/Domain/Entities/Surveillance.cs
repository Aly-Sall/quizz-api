using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;

public class Surveillance : IEntity
{
    public int Id { get; set; }
    public bool ComportementSuspect { get; set; }
    public DateTime TimeStamp { get; set; }
    public string CaptureEcran { get; set; }
    public int TentativeId { get; set; }
    public Tentative? Tentative { get; set; }
}

