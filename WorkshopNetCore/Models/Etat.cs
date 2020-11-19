using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopNetCore.Models
{
    public class Etat
    {
        public int idEtat { get; set; }
        public Feu feu { get; set; }
        public JourEnum jour { get; set; }
        public DateTime horaire { get; set; }
        public int nbPassant { get; set; }
        public bool etat { get; set; }
    }
}
