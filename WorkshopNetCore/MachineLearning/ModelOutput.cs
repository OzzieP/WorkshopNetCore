using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopNetCore.MachineLearning
{
    public class ModelOutput
    {
        public float[] ForecastedPassants { get; set; }

        public float[] LowerBoundPassants { get; set; }

        public float[] UpperBoundPassants { get; set; }
    }
}
