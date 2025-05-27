using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;

namespace _Net6CleanArchitectureQuizzApp.Application.SurveillanceDev.Queries.GetSurveillanceByTentative;
public class GetSurveillanceDto: IMapFrom<Surveillance>
{
    public bool ComportementSuspect { get; set; }
    public DateTime TimeStamp { get; set; }
    public string CaptureEcran { get; set; }
}

