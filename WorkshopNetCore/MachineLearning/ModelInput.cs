using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopNetCore.MachineLearning
{
    public class ModelInput
    {
        public float Semaine { get; set; }

        public float Jour { get; set; }

        public string Feu { get; set; }

        public float NbPassants { get; set; }
    }
}
