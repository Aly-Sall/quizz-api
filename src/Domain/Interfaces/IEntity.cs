using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Net6CleanArchitectureQuizzApp.Domain.Interfaces;

public interface IEntity
{
    [Key]
    int Id { get; set; }
}
