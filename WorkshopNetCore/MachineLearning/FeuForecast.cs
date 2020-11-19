using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopNetCore.MachineLearning
{
    public class FeuForecast
    {
        public string Jour { get; set; }

        public float Semaine { get; set; }

        public float PassantsActuel { get; set; }

        public float EstimationInferieure { get; set; }

        public float EstimationSuperieure { get; set; }

        public float Estimation { get; set; }

        public FeuForecast()
        {

        }

        public FeuForecast(string jour, float semaine, float passantsActuel, float estimationInferieure, float estimation, float estimationSuperieure)
        {
            Jour = jour;
            Semaine = semaine;
            PassantsActuel = passantsActuel;
            EstimationInferieure = estimationInferieure;
            Estimation = estimation;
            EstimationSuperieure = estimationSuperieure;
        }
    }
}
