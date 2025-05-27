using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;
public class QuizTest : IEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public Category Category { get; set; }
    public Mode Mode { get; set; }
    public bool TryAgain { get; set; }
    public bool ShowTimer { get; set; }
    //if the admin wants to make a training session , workers can see their asnwers 
    //public bool makeRealTimeCorrection { get; set; }    
    public Level Level { get; set; }
    public bool IsActive { get; set; }  
    public int Duration { get; set; }   
    //you can't modify the test while it is active !! for example , making changes while it is in use by others 

    public ICollection<Question>? Questions { get; set; }
    //public ICollection<AnalyseIA>? AnalyseIAs { get; set; }
}
