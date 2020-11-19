using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopNetCore.MachineLearning
{
    public class FeuForecast
    {
        public int IdFeu { get; set; }

        public string Matricule { get; set; }

        public string Jour { get; set; }

        public float Semaine { get; set; }

        public int Heure { get; set; }

        public float PassantsActuel { get; set; }

        public float EstimationInferieure { get; set; }

        public float EstimationSuperieure { get; set; }

        public float Estimation { get; set; }


        public FeuForecast()
        {

        }

        public FeuForecast(int idFeu, string matricule, string jour, float semaine, int heure, float passantsActuel, float estimationInferieure, float estimationSuperieure, float estimation)
        {
            IdFeu = idFeu;
            Matricule = matricule;
            Jour = jour;
            Semaine = semaine;
            Heure = heure;
            PassantsActuel = passantsActuel;
            EstimationInferieure = estimationInferieure;
            EstimationSuperieure = estimationSuperieure;
            Estimation = estimation;
        }
    }
}
