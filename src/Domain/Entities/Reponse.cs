using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;

public class Reponse : IEntity
{
    //baha wake up , how are you going to save candidate answers , of course i am going to use this class 
    //candidate answer ya baha !! 
    public int Id { get; set; }
    public int ChoiceId { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public int QuizTestId { get; set; }
    public QuizTest QuizTest { get; set; } = null!;

}

