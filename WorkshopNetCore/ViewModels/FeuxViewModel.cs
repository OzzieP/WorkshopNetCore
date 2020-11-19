using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkshopNetCore.Models;

namespace WorkshopNetCore.ViewModels
{
    public class FeuxViewModel
    {
        public List<Feu> Feux { get; set; }


        public FeuxViewModel()
        {
            Feux = new List<Feu>
            {
                new Feu("C1-VR1"),
                new Feu("C1-VR2"),
                new Feu("C1-HR1"),
                new Feu("C1-HR2")
            };
        }
    }
}
