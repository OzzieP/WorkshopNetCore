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
        
        public float Estimation { get; set; }

        public float EstimationSuperieure { get; set; }


        public FeuForecast()
        {

        }

        public FeuForecast(int idFeu, string matricule, int jour, float semaine, int heure, float passantsActuel, float estimationInferieure, float estimation, float estimationSuperieure)
        {
            IdFeu = idFeu;
            Matricule = matricule;
            Semaine = semaine;
            Heure = heure;
            PassantsActuel = passantsActuel;
            EstimationInferieure = estimationInferieure;
            Estimation = estimation;
            EstimationSuperieure = estimationSuperieure;

            switch (jour)
            {
                case (int)DayOfWeek.Monday:
                    Jour = "Lundi";
                    break;
                case (int)DayOfWeek.Tuesday:
                    Jour = "Mardi";
                    break;
                case (int)DayOfWeek.Wednesday:
                    Jour = "Mercredi";
                    break;
                case (int)DayOfWeek.Thursday:
                    Jour = "Jeudi";
                    break;
                case (int)DayOfWeek.Friday:
                    Jour = "Vendredi";
                    break;
                case (int)DayOfWeek.Saturday:
                    Jour = "Samedi";
                    break;
                case (int)DayOfWeek.Sunday:
                    Jour = "Dimanche";
                    break;
            }
        }
    }
}
