using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;

public class Tentative : IEntity
    {
        public int Id { get; set; }
        public float ScoreObtenu { get; set; }
        public DateTime PassingDate { get; set; }
        public int TestId { get; set; }
        public QuizTest? Test { get; set; }
    //public int CandidatId { get; set; }
    //public User? Candidat { get; set; }

    //<<<<<<< Updated upstream
    //        public ICollection<Surveillance>? Surveillances { get; set; }
    public ICollection<Surveillance>? Surveillances { get; set; }

    }

